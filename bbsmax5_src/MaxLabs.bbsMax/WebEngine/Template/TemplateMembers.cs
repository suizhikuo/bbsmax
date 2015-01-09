//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MaxLabs.bbsMax;
using System.Web;
using System.Web.Caching;

namespace MaxLabs.WebEngine.Template
{
    internal class TemplateMembers : IDisposable
    {
        private const string cachedInstanceKey = "TemplateMembers.bbsMax";

        private static object instanceLocker = new object();

        internal static TemplateMembers CachedInstance
        {
            get
            {
                object instanceObj = HttpRuntime.Cache[cachedInstanceKey];

                TemplateMembers instance;

                if (instanceObj == null)
                {
                    lock (instanceLocker)
                    {
                        if (instanceObj == null)
                        {
                            instance = new TemplateMembers();

                            foreach (string package in Config.Current.TemplatePackages)
                            {
                                instance.ImportPackages(Assembly.Load(package));
                            }

                            instanceObj = instance;

                            HttpRuntime.Cache.Add(cachedInstanceKey, instance, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 2, 0), CacheItemPriority.Normal, delegate(string key, object cacheItem, System.Web.Caching.CacheItemRemovedReason reason)
                            {
                                try
                                {
                                    ((TemplateMembers)cacheItem).Dispose();
                                    System.GC.Collect();
                                }
                                catch { }
                            });
                        }
                        else
                            instance = instanceObj as TemplateMembers;
                    }
                }
                else
                    instance = instanceObj as TemplateMembers;

                return instance;
            }
        }


        #region 注册模板成员

        private Dictionary<string, MemberInfo> m_PropertyBasedVariables = new Dictionary<string, MemberInfo>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, List<RuntimeMethodHandle>> m_MethodBasedTags = new Dictionary<string, List<RuntimeMethodHandle>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, RuntimeMethodHandle> m_MethodBasedFunctions = new Dictionary<string, RuntimeMethodHandle>(StringComparer.OrdinalIgnoreCase);

        private List<TemplateExtensionMember> m_ExtensionFunctions = new List<TemplateExtensionMember>();
        private List<TemplateExtensionMember> m_ExtensionProperties = new List<TemplateExtensionMember>();

        internal void RegisterTemplateTag(string tagName, MethodInfo method)
        {
            List<RuntimeMethodHandle> list = null;

            //tagName = tagName.ToLower();

            if (m_MethodBasedTags.TryGetValue(tagName, out list) == false)
            {
                list = new List<RuntimeMethodHandle>();

                m_MethodBasedTags.Add(tagName, list);
            }

            list.Add(method.MethodHandle);
        }

        internal void RegisterTemplateFunction(string functionName, MethodInfo method)
        {
            //functionName = functionName.ToLower();

            if (m_MethodBasedFunctions.ContainsKey(functionName) == false)
                m_MethodBasedFunctions.Add(functionName, method.MethodHandle);

            //TODO:判断重复注册
        }

        internal void RegisterTemplateVariable(string variableName, MemberInfo member)
        {
            //variableName = variableName.ToLower();

            if (m_PropertyBasedVariables.ContainsKey(variableName) == false)
                m_PropertyBasedVariables.Add(variableName, member);

            //TODO:判断重复注册
        }

        internal void RegisterExtensionFunction(Type targetType, string name, MethodInfo method)
        {
            //TODO:判断重复注册

            //Regex regex = new Regex("^(" + name + ")$", RegexOptions.IgnoreCase);

            //m_ExtensionFunctions.Add(new KeyValuePair<Regex, RuntimeTypeHandle>(regex, targetType.TypeHandle), method.MethodHandle);

            TemplateExtensionMember extension = new TemplateExtensionMember();
            extension.Name = name;
            extension.TargetTypeHandle = targetType.TypeHandle;
            extension.Method = method.MethodHandle;

            m_ExtensionFunctions.Add(extension);
        }

        internal void RegisterExtensionProperty(Type targetType, string name, MethodInfo method)
        {
            //TODO:判断重复注册

            //Regex regex = new Regex("^(" + name + ")$", RegexOptions.IgnoreCase);

            //m_ExtensionProperties.Add(new KeyValuePair<Regex, RuntimeTypeHandle>(regex, targetType.TypeHandle), method.MethodHandle);

            TemplateExtensionMember extension = new TemplateExtensionMember();
            extension.Name = name;
            extension.TargetTypeHandle = targetType.TypeHandle;
            extension.Method = method.MethodHandle;

            m_ExtensionProperties.Add(extension);

        }

        #endregion


        private void ImportPackages(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsDefined(typeof(TemplatePackageAttribute), false) == false || type.IsAbstract)
                    continue;

                #region 加载模板标签和模板函数

                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                {
                    if (method.IsStatic == false)
                    {
                        if (method.IsDefined(typeof(TemplateTagAttribute), true))
                        {
                            TemplateTagAttribute info = TemplateTagAttribute.GetFromMethod(method);

                            RegisterTemplateTag(info.Name == null ? method.Name : info.Name, method);
                        }

                        if (method.IsDefined(typeof(TemplateFunctionAttribute), true))
                        {
                            TemplateFunctionAttribute info = TemplateFunctionAttribute.GetFromMethod(method);

                            RegisterTemplateFunction(info.Name == null ? method.Name : info.Name, method);
                        }
                    }
                    else
                    {
                        if (method.IsDefined(typeof(TemplateExtensionFunctionAttribute), true))
                        {
                            TemplateExtensionFunctionAttribute info = TemplateExtensionFunctionAttribute.GetFromMethod(method);

                            RegisterExtensionFunction(method.GetParameters()[0].ParameterType, info.Name == null ? method.Name : info.Name, method);
                        }

                        if (method.IsStatic && method.IsDefined(typeof(TemplateExtensionPropertyAttribute), true))
                        {
                            TemplateExtensionPropertyAttribute info = TemplateExtensionPropertyAttribute.GetFromMethod(method);

                            RegisterExtensionProperty(method.GetParameters()[0].ParameterType, info.Name == null ? method.Name : info.Name, method);
                        }
                    }
                }

                #endregion

                #region 加载模板变量

                foreach (MemberInfo member in type.GetMembers(BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
                {
                    if (member.IsDefined(typeof(TemplateVariableAttribute), true))
                    {
                        //#if DEBUG
                        //                        if (member.GetSetMethod(true) != null)
                        //                            throw new Exception(Resources.TemplateVairableAttribute_PropertyMustBeReadonly);
                        //#endif

                        TemplateVariableAttribute info = TemplateVariableAttribute.GetFromMember(member);

                        RegisterTemplateVariable(info.Name == null ? member.Name : info.Name, member);
                    }
                }

                #endregion
            }
        }


        internal Dictionary<string, MemberInfo> PropertyBasedVariables
        {
            get { return m_PropertyBasedVariables; }
        }

        internal Dictionary<string, List<RuntimeMethodHandle>> MethodBasedTags
        {
            get { return m_MethodBasedTags; }
        }

        internal Dictionary<string, RuntimeMethodHandle> MethodBasedFunctions
        {
            get { return m_MethodBasedFunctions; }
        }

        internal List<TemplateExtensionMember> ExtensionFunctions
        {
            get { return m_ExtensionFunctions; }
        }

        internal List<TemplateExtensionMember> ExtensionProperties
        {
            get { return m_ExtensionProperties; }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            m_ExtensionFunctions = null;
            m_ExtensionProperties = null;
            m_MethodBasedFunctions = null;
            m_MethodBasedTags = null;
            m_PropertyBasedVariables = null;
        }

        #endregion
    }
}