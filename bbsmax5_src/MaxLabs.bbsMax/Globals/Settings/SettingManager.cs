//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;


using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Settings
{
	/// <summary>
	/// 设置相关的业务逻辑
	/// </summary>
	public class SettingManager
	{
		/// <summary>
		/// 构造业务逻辑对象的实例
		/// </summary>
		public static SettingManager New 
		{ 
			get { return new SettingManager(); } 
		}

		/// <summary>
		/// 保存设置，注意使用此方法必须保证Setting的类型和属性名保持一致
		/// </summary>
		/// <param name="settings">设置对象</param>
		public static bool SaveSettings(SettingBase settings)
		{
			FieldInfo property = typeof(AllSettings).GetField(settings.GetType().Name);

			if (property == null)
			{
				Context.ThrowError(new AllSettingsLostPropertyError(settings.GetType()));
				return false;

				//NOTE:最终用户不应该遇到此异常。
				//当遇到此异常时，说明新增加的设置类型没有对应到AllSettings类的一个同名属性
				//比如ForumSettings类，对应AllSettings类的ForumSettings属性
				//throw new Exception(error.Text);
			}

			SettingDao.Instance.SaveSettings(settings);

            //重新创建一个新的设置实例，以避免由于值变更导致的局部变量不同步等问题
            SettingBase newSettings = (SettingBase)Activator.CreateInstance(settings.GetType());
            //给新的设置实例赋值
            newSettings.Parse(settings.ToString());

			//保存完毕后更新AllSettings对象中对应的设置
            AllSettings.Current.SetSettingFieldValue(property, newSettings);

            return true;
		}

		/// <summary>
		/// 缓存当前系统的所有设置
		/// </summary>
		internal static void CacheAllSettings()
		{
			AllSettings.Current = SettingDao.Instance.LoadAllSettings();
		}


        /// <summary>
        /// 获取指定名称的PermissionSet实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IPermissionSet GetPermissionSet(string name,int nodeID)
        {

            if (string.IsNullOrEmpty(name))
                return null;

            Type allSettingsType = typeof(AllSettings);

            FieldInfo fieldInfo;

            try
            {
                fieldInfo = allSettingsType.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            }
            catch
            {
                return null;
            }

            if (fieldInfo.FieldType.BaseType.IsGenericType && fieldInfo.FieldType.BaseType.GetGenericTypeDefinition() == typeof(PermissionSetBase<,>))
            {
                return (IPermissionSet)fieldInfo.GetValue(AllSettings.Current);
            }
            else if (ReflectionUtil.HasInterface(fieldInfo.FieldType, typeof(IPermissionSetWithNode)))
            {
                return ((IPermissionSetWithNode)fieldInfo.GetValue(AllSettings.Current)).GetNode(nodeID);
            }

            return null;
        }

        public static List<IPermissionSet> GetPermissionSets()
        {
            Type allSettingsType = typeof(AllSettings);

            List<IPermissionSet> permissionSets = new List<IPermissionSet>();

            FieldInfo[] fieldInfos = allSettingsType.GetFields(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo propertyInfo in fieldInfos)
            {
                if (ReflectionUtil.HasInterface(propertyInfo.FieldType, typeof(IPermissionSet)))
                {
                    IPermissionSet permissionSet = (IPermissionSet)propertyInfo.GetValue(AllSettings.Current);

                    permissionSets.Add(permissionSet);
                }
                else if (ReflectionUtil.HasInterface(propertyInfo.FieldType, typeof(IPermissionSetWithNode)))
                {
                    IPermissionSet permissionSet = ((IPermissionSetWithNode)propertyInfo.GetValue(AllSettings.Current)).GetNode(0);

                    permissionSets.Add(permissionSet);
                }
            }

            return permissionSets;
        }

        private static Dictionary<string, IPermissionSetWithNode> permissionWithNodes = new Dictionary<string, IPermissionSetWithNode>();
        public static void RegisterPermissionWithNode(string nodeTypeName,IPermissionSetWithNode permissionWithNode)
        {
            nodeTypeName = nodeTypeName.ToLower();
            if (false == permissionWithNodes.ContainsKey(nodeTypeName))
                permissionWithNodes.Add(nodeTypeName, permissionWithNode);
        }

        public static IPermissionSetWithNode GetPermissionWithNode(string nodeTypeName)
        {
            IPermissionSetWithNode permission;

            permissionWithNodes.TryGetValue(nodeTypeName.ToLower(),out permission);

            return permission;
        }

        public static T CloneSetttings<T>(T setting) where T : SettingBase, new()
        {
            string settingsString = setting.ToString();

            T newSetting = new T();
            newSetting.Parse(settingsString);

            return newSetting;
        }

	}
}