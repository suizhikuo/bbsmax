//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Compilation;
using System.Collections;
using System.Collections.Specialized;
using MaxLabs.bbsMax.Settings;
using System.Web;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.WebEngine.Template
{
	internal class TemplateParser : IDisposable
	{
		private StringBuffer m_CodeHead;
		private StringBuffer m_CodeBody;

		private TemplateElement m_Document;

		private static readonly Regex s_MatchComma = new Regex("^ *,");
        //private static readonly Regex s_MatchThemeDir = new Regex(@"~/");

        private static readonly Type s_TypeOfStringCollection = typeof(StringCollection);

		#region 模板成员处理

		#region 注册页面模板成员

        private Dictionary<string, MemberInfo> m_PropertyBasedVariables = new Dictionary<string, MemberInfo>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, List<RuntimeMethodHandle>> m_MethodBasedTags = new Dictionary<string, List<RuntimeMethodHandle>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, RuntimeMethodHandle> m_MethodBasedFunctions = new Dictionary<string, RuntimeMethodHandle>(StringComparer.OrdinalIgnoreCase);

        private List<TemplateExtensionMember> m_ExtensionFunctions = new List<TemplateExtensionMember>();
        private List<TemplateExtensionMember> m_ExtensionProperties = new List<TemplateExtensionMember>();

        private static Assembly s_WebAssembly = null;
        private static Assembly s_CommonAssembly = null;

		private void ParseCodeFile(string typeName)
		{
			Type type = null;

            try
            {
                if (BuildManager.CodeAssemblies != null)
                {
                    foreach (Assembly assembly in BuildManager.CodeAssemblies)
                    {
                        type = assembly.GetType(typeName);

                        if (type != null)
                            break;
                    }
                }

                if (type == null)
                    type = BuildManager.GetType(typeName, false);
            }
            catch { }

            if (type == null)
            {
                if (s_WebAssembly == null)
                {
                    s_WebAssembly = Assembly.LoadFrom(IOUtil.JoinPath(Globals.BinDirectory, "MaxLabs.bbsMax.Web.dll"));
                }

                type = s_WebAssembly.GetType(typeName);

                if (type == null)
                {
                    if (s_CommonAssembly == null)
                    {
                        s_CommonAssembly = Assembly.LoadFrom(IOUtil.JoinPath(Globals.BinDirectory, "MaxLabs.bbsMax.dll"));
                    }

                    type = s_CommonAssembly.GetType(typeName);
                }
            }

			if (type != null)
				RegisterTemplateMembers(type);

		}

		private void RegisterTemplateMembers(Type type)
		{
			if (type != null)
            {

				#region 加载模板变量、标签、函数

                Type pageBaseType = typeof(PageBase);
                Type pagePartBaseType = typeof(PagePartBase);

				foreach (MemberInfo member in type.GetMembers( BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy))
				{
					//#if DEBUG
					//                    if (property.GetSetMethod(true) != null)
					//                        throw new Exception(Resources.TemplateVairableAttribute_PropertyMustBeReadonly);
					//#endif

                    if (member.DeclaringType.IsSubclassOf(pageBaseType) == false && member.DeclaringType.IsSubclassOf(pagePartBaseType) == false
                        &&
                        member.DeclaringType != pageBaseType && member.DeclaringType != pagePartBaseType
                        )
                        continue;

                    if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                    {
                        if (member.MemberType == MemberTypes.Property)
                        {
                            PropertyInfo property = member as PropertyInfo;

                            if (property == null || property.GetGetMethod(true) == null || property.GetGetMethod(true).IsPrivate)
                                continue;
                        }
                        else
                        {
                            FieldInfo field = member as FieldInfo;

                            if (field == null || field.IsPrivate)
                                continue;
                        }

                        TemplateVariableAttribute info = TemplateVariableAttribute.GetFromMember(member);

                        RegisterPageTemplateVariable(info == null || info.Name == null ? member.Name : info.Name, member);
                    }
                    else
                    {
                        MethodInfo method = member as MethodInfo;

                        if (method == null || method.IsPrivate || StringUtil.StartsWith(method.Name, "get_") || StringUtil.StartsWith(method.Name, "set_"))
                            continue;

                        else if (method.IsDefined(typeof(TemplateTagAttribute), true))
                        {
                            TemplateTagAttribute info = TemplateTagAttribute.GetFromMethod(method);

                            RegisterPageTemplateTag(info.Name == null ? method.Name : info.Name, method);
                        }
                        else if (method.IsDefined(typeof(TemplateExtensionFunctionAttribute), true))
                        {
                            TemplateExtensionFunctionAttribute info = TemplateExtensionFunctionAttribute.GetFromMethod(method);

                            RegisterPageExtensionFunction(method.GetParameters()[0].ParameterType, info.Name == null ? method.Name : info.Name, method);
                        }
                        else if (method.IsDefined(typeof(TemplateExtensionPropertyAttribute), true))
                        {
                            TemplateExtensionPropertyAttribute info = TemplateExtensionPropertyAttribute.GetFromMethod(method);

                            RegisterPageExtensionProperty(method.GetParameters()[0].ParameterType, info.Name == null ? method.Name : info.Name, method);
                        }
                        else
                        {
                            TemplateFunctionAttribute info = TemplateFunctionAttribute.GetFromMethod(method);

                            RegisterPageTemplateFunction(info == null || info.Name == null ? method.Name : info.Name, method);
                        }

                    }
                    
				}

				#endregion
			}
		}

		private void RegisterPageTemplateTag(string tagName, MethodInfo method)
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

		private void RegisterPageTemplateFunction(string functionName, MethodInfo method)
		{
			//functionName = functionName.ToLower();

			if (m_MethodBasedFunctions.ContainsKey(functionName) == false)
				m_MethodBasedFunctions.Add(functionName, method.MethodHandle);

			//TODO:判断重复注册
		}

		private void RegisterPageTemplateVariable(string variableName, MemberInfo member)
		{
			//variableName = variableName.ToLower();

			if (m_PropertyBasedVariables.ContainsKey(variableName) == false)
                m_PropertyBasedVariables.Add(variableName, member);

			//TODO:判断重复注册
		}

		private void RegisterPageExtensionFunction(Type targetType, string name, MethodInfo method)
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

		private void RegisterPageExtensionProperty(Type targetType, string name, MethodInfo method)
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


		/// <summary>
		/// 获取模板标签所对应的方法
		/// </summary>
		/// <param name="tag">模板标签</param>
		/// <returns></returns>
		private MethodInfo SearchMethodForTag(TemplateElement tag)
		{
			string name = ((string)tag.Items[TemplateElement.KEY_NAME]);

			if (m_MethodBasedTags.ContainsKey(name))
			{
				return SearchMethod(m_MethodBasedTags[name], tag.ChildNodes[0], true);
			}
			else if (TemplateMembers.CachedInstance.MethodBasedTags.ContainsKey(name))
			{
                return SearchMethod(TemplateMembers.CachedInstance.MethodBasedTags[name], tag.ChildNodes[0], true);
			}
			else
			{
				//TODO: Report Error
			}

			return null;
		}

		/// <summary>
		/// 获取模板函数所对应的方法
		/// </summary>
		/// <param name="function">模板函数名（小写）</param>
		/// <returns></returns>
		private MethodInfo SearchMethodForFunction(string functionName)
		{
			if (m_MethodBasedFunctions.ContainsKey(functionName))
			{
				return (MethodInfo)MethodInfo.GetMethodFromHandle(m_MethodBasedFunctions[functionName]);
			}
            else if (TemplateMembers.CachedInstance.MethodBasedFunctions.ContainsKey(functionName))
			{
                return (MethodInfo)MethodInfo.GetMethodFromHandle(TemplateMembers.CachedInstance.MethodBasedFunctions[functionName]);
			}
			else
			{
				//TODO: Report Error
			}

			return null;
		}

		/// <summary>
		/// 获取模板变量所对应的属性
		/// </summary>
		/// <param name="variable">模板变量（小写）</param>
		/// <returns></returns>
		private MemberInfo SearchMemberForVariable(string variableName)
		{
			if (m_PropertyBasedVariables.ContainsKey(variableName))
			{
				return m_PropertyBasedVariables[variableName];
			}
            else if (TemplateMembers.CachedInstance.PropertyBasedVariables.ContainsKey(variableName))
			{
                return TemplateMembers.CachedInstance.PropertyBasedVariables[variableName];
			}
			else
			{
				//TODO: Report Error
			}

			return null;
		}

		/// <summary>
		/// 获取类型的扩展方法
		/// </summary>
		/// <param name="name">方法名</param>
		/// <param name="type">类型</param>
		/// <returns></returns>
		private MethodInfo SearchExtensionFunction(string name, Type type)
		{
            foreach (TemplateExtensionMember item in m_ExtensionFunctions)
			{
				if (item.GetNameMatcher().IsMatch(name) && (item.TargetTypeHandle.Equals(type.TypeHandle) || type.IsSubclassOf(Type.GetTypeFromHandle(item.TargetTypeHandle))))
					return (MethodInfo)MethodInfo.GetMethodFromHandle(item.Method);
			}

            foreach (TemplateExtensionMember item in TemplateMembers.CachedInstance.ExtensionFunctions)
			{
				if (item.GetNameMatcher().IsMatch(name) && (item.TargetTypeHandle.Equals(type.TypeHandle) || type.IsSubclassOf(Type.GetTypeFromHandle(item.TargetTypeHandle))))
					return (MethodInfo)MethodInfo.GetMethodFromHandle(item.Method);
			}

			return null;
		}

		/// <summary>
		/// 获取类型的扩展属性
		/// </summary>
		/// <param name="name">属性名</param>
		/// <param name="type">类型</param>
		/// <returns></returns>
		private MethodInfo SearchExtensionProperty(string name, Type type)
		{
            foreach (TemplateExtensionMember item in m_ExtensionProperties)
			{
				if (item.GetNameMatcher().IsMatch(name) && (item.TargetTypeHandle.Equals(type.TypeHandle) || type.IsSubclassOf(Type.GetTypeFromHandle(item.TargetTypeHandle))))
					return (MethodInfo)MethodInfo.GetMethodFromHandle(item.Method);
			}

            foreach (TemplateExtensionMember item in TemplateMembers.CachedInstance.ExtensionProperties)
			{
				if (item.GetNameMatcher().IsMatch(name) && (item.TargetTypeHandle.Equals(type.TypeHandle) || type.IsSubclassOf(Type.GetTypeFromHandle(item.TargetTypeHandle))))
					return (MethodInfo)MethodInfo.GetMethodFromHandle(item.Method);
			}

			return null;
		}

		#endregion

        public void GenerateAspxCode(string skinID, TemplateFile templateFile, string[] templateImports)
        {
            string template = templateFile.GetFullTemplate(skinID);

            string checkString = ReadCheckString(templateFile);
            string newCheckString = GetNewCheckString(template);

            if (checkString != newCheckString)
            {
                #region 需要重新解析模板    

                m_Document = TemplateElement.CreateDocument(template, templateFile);

                m_CodeHead = new StringBuffer();
                m_CodeBody = new StringBuffer(template.Length);

                GenerateAspxCodeHead1(templateFile);

                GenerateAspxCode(m_Document, new ScopeData(null));

                GenerateAspxCodeHead2(templateFile, templateImports, skinID);

                string aspxCode = string.Concat("<%--", newCheckString, "--%>\r\n", m_CodeHead.ToString(), m_CodeBody.ToString());

                try
                {
                    using (StreamWriter writer = new StreamWriter(templateFile.ParsedFilePath, false, Encoding.UTF8))
                    {
                        writer.Write(aspxCode);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.CreateErrorLog(ex, templateFile.FileName + "写入文件时出错");
                }

                LogHelper.CreateDebugLog(templateFile.FileName + "发生了重新解析，并写入磁盘");

                #endregion
            }

            TryBuildTempWebConfig();
        }

		private void GenerateAspxCode(TemplateElement element, ScopeData scopeData)
		{
			foreach (TemplateElement node in element.ChildNodes)
			{
				switch (node.Type)
				{
					case TemplateElementTypes.Tag:
						GenerateTagAspxCode(node, scopeData);
						break;

					case TemplateElementTypes.Literal:
						GenerateLiteralAspxCode(node);
						break;

					case TemplateElementTypes.Variable:
						GenerateVariableAspxCode(node, scopeData);
						break;

					case TemplateElementTypes.Function:
						GenerateFunctionAspxCode(node, scopeData);
						break;

					case TemplateElementTypes.CodeBlock:
						GenerateCodeBlockAspxCode(node, scopeData);
						break;

					case TemplateElementTypes.IfExpression:
					case TemplateElementTypes.ElseExpression:
					case TemplateElementTypes.ElseIfExpression:
						GenerateIfElseEpxressionAspxCode(node, scopeData);
						break;

					case TemplateElementTypes.LoadExpression:
						GenerateIncludeExpressionAspxCode(node, scopeData);
						break;

					//case TemplateElementTypes.PreIncludeExpression:
					//	GenerateAspxCode(node.ChildNodes[1], scopeData);
					//	break;

					case TemplateElementTypes.LoopExpression:
						GenerateLoopExpressionAspxCode(node, scopeData);
						break;

					case TemplateElementTypes.AjaxPanel:
						GenerateAjaxPanelAspxCode(node, scopeData);
						break;
				}
			}
		}

		private void GenerateAspxCode(StringBuffer codeBody, TemplateElement element, ScopeData scopeData)
		{
			foreach (TemplateElement node in element.ChildNodes)
			{
				Type returnType = null;

				switch (node.Type)
				{
					case TemplateElementTypes.Literal:
						GenerateLiteralAspxCode(codeBody, node);
						break;

					case TemplateElementTypes.Variable:
						GenerateVariableAspxCode(codeBody, node, out returnType, scopeData);
						break;

					case TemplateElementTypes.Function:
						GenerateFunctionAspxCode(codeBody, node, out returnType, scopeData);
						break;

					case TemplateElementTypes.CodeBlock:
						GenerateCodeBlockAspxCode(codeBody, node, scopeData);
						break;

					case TemplateElementTypes.DoubleQuteString:
						GenerateDoubleQuteStringAspxCode(codeBody, node, scopeData);
						break;

					case TemplateElementTypes.SingleQuteString:
						GenerateSingleQuteStringAspxCode(codeBody, node, scopeData);
						break;
				}
			}
		}

		private void GenerateAspxCodeHead1(TemplateFile templateFile)
		{
			m_CodeHead += "<%@ ";

			if (templateFile.IsControl)
				m_CodeHead += "Control Language=\"C#\" ";
			else
				m_CodeHead += "Page Language=\"C#\" ";

			string inherits = null;

			if(m_Document.ChildNodes.Count > 0)
			{
				TemplateElement firstNode = m_Document.ChildNodes[0];

				if(firstNode.Type == TemplateElementTypes.Tag)
				{
					string tagName = (string)firstNode.Items[TemplateElement.KEY_NAME];

					if (string.Compare(tagName, "page", true) == 0)
					{
						foreach (TemplateElement element in firstNode.ChildNodes[0].ChildNodes)
						{
							string name = (string)element.Items[TemplateElement.KEY_NAME];

							if (string.Compare(name, "inherits", true) == 0)
								inherits = element.SourceTemplate.Substring(element.Index, element.Length);
						}
					}
				}
			}

			if (inherits == null)
			{
				m_CodeHead += "Inherits=\"" + templateFile.CodeFileInherits + "\" %>";
				ParseCodeFile(templateFile.CodeFileInherits);
			}
			else
			{
				m_CodeHead += "Inherits=\"" + inherits + "\" %>";
				ParseCodeFile(inherits);
			}
		}

		private void GenerateAspxCodeHead2(TemplateFile templateFile, string[] templateImports, string skinID)
		{
			foreach (string nameSpace in templateImports)
			{
				m_CodeHead += "<%@ Import Namespace=\"" + nameSpace + "\" %>";
			}

            m_CodeHead += "<%\r\n DirectoryVirtualPath = \"" + templateFile.VirtualDirectoryPath + "\";\r\nthis.SubmitFillUsers(); %>" + Environment.NewLine;
            m_CodeHead += "<script runat=\"server\">" + Environment.NewLine;
            m_CodeHead += "protected override void OnLoad(System.EventArgs e)" + Environment.NewLine;
            m_CodeHead += "{" + Environment.NewLine;
            m_CodeHead += "this.DirectoryVirtualPath = \"" + templateFile.VirtualDirectoryPath + "\";" + Environment.NewLine;
            m_CodeHead += "base.OnLoad(e);" + Environment.NewLine;
            m_CodeHead += "}";
            
			if (m_DeclaredVariables.Count > 0)
			{
				foreach (KeyValuePair<Type, string> variable in m_DeclaredVariables)
				{
                    string typeName = ReflectionUtil.GetCSharpTypeName(variable.Key);

                    m_CodeHead += Environment.NewLine + typeName + " " + variable.Value + " = new " + typeName + "();";
				}
			}
            m_CodeHead += Environment.NewLine + "</script>";

		}

		#region 输出模板标签

		private void GenerateTagAspxCode(TemplateElement tag, ScopeData scopeData)
		{
            string name = ((string)tag.Items[TemplateElement.KEY_NAME]);

            if (string.Compare(name, "page", true) == 0)
                return;
            else if (string.Compare(name, "break", true) == 0)
            {
                m_CodeBody += "<% break; %>";
                return;
            }
            else if (string.Compare(name, "continue", true) == 0)
            {
                m_CodeBody += "<% continue; %>";
                return;
            }

			MethodInfo method = SearchMethodForTag(tag);

			if (method != null)
			{
				string instanceName = DeclaringVariable(method.DeclaringType);

                m_CodeBody += "<%\r\n" + instanceName + "." + method.Name + "(";

				int templateIndex = -1;		//模板委托类型参数的起始索引
				int templateCount = 0;		//模板委托类型参数的个数

				bool needRemoveDot = false;

				ParameterInfo[] parameters = method.GetParameters();

				#region 将模板标签属性输出为方法参数

				for (int i = 0; i < parameters.Length; i++)
				{
					if (parameters[i].ParameterType.IsSubclassOf(typeof(Delegate)) == false)
					{
						TemplateElement attributeListItem = GetAttributeListItem(parameters[i], tag.ChildNodes[0]);

						if (attributeListItem != null)
						{
							GenerateAttributeListItemAspxCode(parameters[i], attributeListItem, scopeData);
						}

						m_CodeBody += ",";

						needRemoveDot = true;
					}
					else
					{
						if (templateIndex == -1)
							templateIndex = i;

						templateCount++;
					}
				}

				if (needRemoveDot == true)
					m_CodeBody.InnerBuilder.Remove(m_CodeBody.InnerBuilder.Length - 1, 1);

				#endregion

				#region 将模板标签的模板内容输出为匿名委托

				for (int i = templateIndex; i - templateIndex < templateCount; i++)
				{
					ScopeData scope = new ScopeData(scopeData);

					if (needRemoveDot == true)
					{
						m_CodeBody += ",";
					}
					else
					{
						needRemoveDot = true;
					}

					m_CodeBody += "delegate(";

					ParameterInfo[] args = parameters[i].ParameterType.GetMethod("Invoke").GetParameters();

					for (int j = 0; j < args.Length; j++)
					{
						m_CodeBody += ReflectionUtil.GetCSharpTypeName(args[j].ParameterType) + " " + scope.DeclaringScopeVariable(args[j]);

						if (j < args.Length - 1)
							m_CodeBody += ",";
					}

					m_CodeBody += "){\r\n%>";

					if (templateCount == 1)
					{
						GenerateAspxCode(tag, scope);
					}
					else
					{
						for (int j = 1; j < tag.ChildNodes.Count; j++)
						{
							TemplateElement element = tag.ChildNodes[j];

							if (element.Type == TemplateElementTypes.Tag)
							{
								string tagName = (string)element.Items[TemplateElement.KEY_NAME];

								if (StringUtil.EqualsIgnoreCase(parameters[i].Name, tagName))
								{
									GenerateAspxCode(element, scope);
									break;
								}
							}
						}
					}

					m_CodeBody += "<%\r\n}";

					//for (int j = 0; j < args.Length; j++)
					//{
					//    UndeclaringScopeVariable(args[j]);
					//}
				}

				#endregion

				m_CodeBody += ");%>";
			}
			else
			{
                //未能找到合适的注册标签，直接输出
                //if (tag.Index != tag.Length)
                //    m_CodeBody += tag.SourceTemplate.Substring(tag.Index, tag.Length);

                //string name = tag.Items[TemplateElement.KEY_NAME] as string;

                //if (StringUtil.EqualsIgnoreCase(name, "ajaxpanel"))
                //{

                //}
			}
		}

		private void GenerateIfElseEpxressionAspxCode(TemplateElement expression, ScopeData scopeData)
		{
			if (expression.Type == TemplateElementTypes.IfExpression)
			{
                m_CodeBody += "<%\r\n if (";

				GenerateAspxCode(m_CodeBody, expression.ChildNodes[0], scopeData);

				m_CodeBody += ") { %>";

				GenerateAspxCode(expression, scopeData);

                m_CodeBody += "<%\r\n } %>";
			}
			else if (expression.Type == TemplateElementTypes.ElseExpression)
			{
                m_CodeBody += "<%\r\n } else { %>";
			}
			else if (expression.Type == TemplateElementTypes.ElseIfExpression)
			{
                m_CodeBody += "<%\r\n } else if(";

				GenerateAspxCode(m_CodeBody, expression.ChildNodes[0], scopeData);

				m_CodeBody += ") { %>";
			}
		}

		private void GenerateIncludeExpressionAspxCode(TemplateElement node, ScopeData scopeData)
		{
            m_CodeBody += "<%\r\n Include(__w, ";

			for (int i = 0; i < node.ChildNodes[0].ChildNodes.Count; i++)
			{
				TemplateElement element = node.ChildNodes[0].ChildNodes[i];

				m_CodeBody += "\"" + element.Items[TemplateElement.KEY_NAME] + "\",";

				for (int j = 0; j < element.ChildNodes.Count; j++)
				{
					TemplateElement element2 = element.ChildNodes[j];

					Type returnType = null;

					switch (element2.Type)
					{
						case TemplateElementTypes.Literal:
							m_CodeBody += "\"";
							GenerateLiteralAspxCode(element2);
							m_CodeBody += "\"";
							break;

						case TemplateElementTypes.Variable:
							GenerateVariableAspxCode(m_CodeBody, element2, out returnType, scopeData);
							break;

						case TemplateElementTypes.Function:
							GenerateFunctionAspxCode(m_CodeBody, element2, out returnType, scopeData);
							break;

						case TemplateElementTypes.CodeBlock:
							GenerateCodeBlockAspxCode(m_CodeBody, element2, scopeData);
							break;
					}

					if (j < element.ChildNodes.Count - 1)
						m_CodeBody += "+";
				}

				if (i < node.ChildNodes[0].ChildNodes.Count - 1)
					m_CodeBody += ",";
			}
			
			m_CodeBody += "); %>";
		}

		private void GenerateLoopExpressionAspxCode(TemplateElement node, ScopeData scopeData)
		{
			switch(node.ChildNodes[0].Type)
			{
				case TemplateElementTypes.LoopExpressionParam:
					GenerateLoopExpressionParamAspxCode(node.ChildNodes[0], scopeData);
					break;

				case TemplateElementTypes.LoopExpressionParam2:
					GenerateLoopExpressionParam2AspxCode(node.ChildNodes[0], scopeData);
					break;
			}
		}

		private void GenerateLoopExpressionParamAspxCode(TemplateElement node, ScopeData scopeData)
		{
			if (node.ChildNodes.Count == 2)
			{
				if (node.ChildNodes[0].Type == TemplateElementTypes.Variable)
				{
					TemplateElement variable = node.ChildNodes[0];
					TemplateElement target = node.ChildNodes[1];

					StringBuffer codeBody = new StringBuffer();

					Type returnType = null;

					switch (target.Type)
					{
						case TemplateElementTypes.Variable:
							GenerateVariableAspxCode(codeBody, target, out returnType, scopeData);
							break;

						case TemplateElementTypes.Function:
							GenerateFunctionAspxCode(codeBody, target, out returnType, scopeData);
							break;
					}

					if (returnType != null)
					{
						Type enumerType = returnType.GetInterface("IEnumerable`1");

                        if (enumerType == null)
                            enumerType = returnType.GetInterface("IEnumerator`1");

                        Type varType = null;

                        if (enumerType != null)
                            varType = enumerType.GetGenericArguments()[0];
                        else
                        {
                            if (returnType == s_TypeOfStringCollection || returnType.IsSubclassOf(s_TypeOfStringCollection))
                                varType = typeof(string);
                        }

						if (varType != null)
						{
							string varName = variable.Items[TemplateElement.KEY_NAME] as string;

							string i = node.Items["i"] as string;

							if (varName != null)
							{
								ScopeData scope = new ScopeData(scopeData);

								string iVarName = null;
								
								if(i != null)
									iVarName = scope.DeclaringScopeVariable(i, typeof(int));

                                m_CodeBody += "<%\r\nif(" + codeBody + " != null)\r\n{%>";

								if (iVarName != null)
                                    m_CodeBody += "<%\r\nint " + iVarName + "=0;\r\n%>";

                                m_CodeBody += "<%\r\nforeach(" + ReflectionUtil.GetCSharpTypeName(varType) + " " + scope.DeclaringScopeVariable(varName, varType) + " in " + codeBody + "){%>";

								GenerateAspxCode(node.Parent, scope);

                                m_CodeBody += "<%";

								if (iVarName != null)
                                    m_CodeBody += "\r\n" + iVarName + "+=1;";

                                m_CodeBody += "\r\n}\r\n}";

                                m_CodeBody += "%>";
							}
						}
					}
				}
			}
		}

		private void GenerateLoopExpressionParam2AspxCode(TemplateElement node, ScopeData scopeData)
		{
			if (node.ChildNodes.Count == 2)
			{
				TemplateElement from = node.ChildNodes[0];
				TemplateElement to = node.ChildNodes[1];

				StringBuffer fromCode = new StringBuffer();

				Type fromType = null;

				switch (from.Type)
				{
					case TemplateElementTypes.Variable:
						GenerateVariableAspxCode(fromCode, from, out fromType, scopeData);
						break;

					case TemplateElementTypes.Function:
						GenerateFunctionAspxCode(fromCode, from, out fromType, scopeData);
						break;

					case TemplateElementTypes.Literal:
						string fromValue = from.SourceTemplate.Substring(from.Index, from.Length);
						
						int temp = -1;

						if (int.TryParse(fromValue, out temp))
						{
							fromCode += fromValue;
							fromType = typeof(int);
						}
						break;
				}

				StringBuffer toCode = new StringBuffer();

				Type toType = null;

				switch (to.Type)
				{
					case TemplateElementTypes.Variable:
						GenerateVariableAspxCode(toCode, to, out toType, scopeData);
						break;

					case TemplateElementTypes.Function:
						GenerateFunctionAspxCode(toCode, to, out toType, scopeData);
						break;

					case TemplateElementTypes.Literal:
						string toValue = to.SourceTemplate.Substring(to.Index, to.Length);

						int temp = -1;

						if (int.TryParse(toValue, out temp))
						{
							toCode += toValue;
							toType = typeof(int);
						}
						break;
				}

				if (fromType == typeof(int) && toType == typeof(int))
				{
					string i = node.Items["i"] as string;
					string step = node.Items["step"] as string;

					uint temp = 0;

					if (i != null && (step == null || (uint.TryParse(step, out temp) && temp > 0)))
					{
						if (step == null)
							step = "1";

						ScopeData scope = new ScopeData(scopeData);

						string iVarName = scope.DeclaringScopeVariable(i, typeof(int));

						//example: for (int i = a; a > b ? i >= b : i <= b; i += a > b ? -c : c)

                        m_CodeBody += "<%\r\nfor(int " + iVarName + "=" + fromCode + ";" + fromCode + " > " + toCode + " ? " + iVarName + " >= " + toCode + " : " + iVarName + " <= " + toCode + ";" + iVarName + " += " + fromCode + " > " + toCode + " ? -" + step + " : " + step + "){%>";

						GenerateAspxCode(node.Parent, scope);

                        m_CodeBody += "<%\r\n}%>";
					}
				}
			}
		}

		private int m_AjaxPanelIDCount = 0;

		private string NewAjaxPanelID()
		{
			return "AjaxPanel" + m_AjaxPanelIDCount ++;
		}

		private void GenerateAjaxPanelAspxCode(TemplateElement node, ScopeData scopeData)
		{
			TemplateElement attributeList = node.ChildNodes[0];

			string autoID = NewAjaxPanelID();

			string id = null;
			string tag = null;
			string onUpdate = null;
			string idOnly = "false";
			string ajaxOnly = "false";
			Hashtable others = new Hashtable(2);

			foreach (TemplateElement item in attributeList.ChildNodes)
			{
				string name = item.Items[TemplateElement.KEY_NAME] as string;

				name = name.ToLower();

				switch (name)
				{
					case "id":
                        StringBuffer sb = new StringBuffer();

						GenerateDoubleQuteStringAspxCode(sb, item, scopeData);

                        id = sb.ToString();
						break;

					case "tag":
						tag = item.SourceTemplate.Substring(item.Index, item.Length);
						break;

					case "idonly":
						idOnly = "true";
						break;

					case "ajaxonly":
						ajaxOnly = "true";
						break;

					case "onupdate":
						StringBuffer jscode = new StringBuffer();

						GenerateAspxCode(jscode, item, scopeData);

						onUpdate = jscode.ToString();
						break;

					default:
						StringBuffer code = new StringBuffer();

						GenerateAspxCode(code, item, scopeData);

						others.Add(name, code.ToString());
						break;
				}
			}

			if (id == null)
				id = "\"" + autoID + "\"";

			if (tag == null)
				tag = "div";

			m_CodeBody += "<" + tag + " id=\"<%=" + id + "%>\"";

			if (others.Count > 0)
			{
				foreach (DictionaryEntry item in others)
				{
					m_CodeBody += " " + item.Key + "=\"" + item.Value + "\"";
				}
			}

			m_CodeBody += ">";

            m_CodeBody += "<%\r\nusing(MaxLabs.WebEngine.AjaxPanel " + autoID + " = new MaxLabs.WebEngine.AjaxPanel(" + id + ", " + idOnly + ", " + ajaxOnly + ", this.AjaxPanelContext, this.HtmlTextWriter)){%>";

            m_CodeBody += "<%\r\nHtmlTextWriter " + autoID + "__w = __w; if(__w.InnerWriter is AjaxPanelWriter == false) __w = " + autoID + ".Writer; %>";
			
			GenerateAspxCode(node, scopeData);

            m_CodeBody += "<%\r\n__w = " + autoID + "__w; }%>";

			m_CodeBody += "</" + tag + ">";

			if (onUpdate != null)
			{
                m_CodeBody += "<script type=\"text/javascript\">var __<%=" + id + "%>__ = document.getElementById('<%=" + id + "%>'); __<%=" + id + "%>__.onUpdate = function(panel){ " + onUpdate + " }</script>";
			}
		}

		private void GenerateAttributeListItemAspxCode(ParameterInfo parameterInfo, TemplateElement attributeListItem, ScopeData scopeData)
		{
			TemplateDefaultValueAttribute defaultValue = TemplateDefaultValueAttribute.GetFromParameter(parameterInfo);

			if (attributeListItem.ChildNodes.Count == 0)
			{
				if (defaultValue != null)
					m_CodeBody += defaultValue.DefaultValue;
				else
					m_CodeBody += "default(" + ReflectionUtil.GetCSharpTypeName(parameterInfo.ParameterType) + ")";
			}
			else if (attributeListItem.ChildNodes.Count == 1)
			{
				StringBuffer newCodeBody = new StringBuffer();

				TemplateElement element = attributeListItem.ChildNodes[0];

				Type returnType = null;

				switch (element.Type)
				{
					case TemplateElementTypes.Literal:
						if (parameterInfo.ParameterType == typeof(string))
						{
							m_CodeBody += "\"";

							GenerateLiteralAspxCode(attributeListItem.ChildNodes[0]);

							m_CodeBody += "\"";
						}
						else if (parameterInfo.ParameterType.IsEnum)
						{
							m_CodeBody += "TryParse<" + ReflectionUtil.GetCSharpTypeName(parameterInfo.ParameterType) + ">(\"";

							GenerateLiteralAspxCode(attributeListItem.ChildNodes[0]);

							m_CodeBody += "\")";
						}
						else if (parameterInfo.ParameterType == typeof(char))
						{
							m_CodeBody += "\'";

							GenerateLiteralAspxCode(attributeListItem.ChildNodes[0]);

							m_CodeBody += "\'";
						}
						else
						{
							GenerateLiteralAspxCode(element);
						}
						break;

					case TemplateElementTypes.Variable:
						GenerateVariableAspxCode(newCodeBody, element, out returnType, scopeData);
						break;

					case TemplateElementTypes.Function:
						GenerateFunctionAspxCode(newCodeBody, element, out returnType, scopeData);
						break;

					case TemplateElementTypes.CodeBlock:
						GenerateCodeBlockAspxCode(newCodeBody, element, scopeData);
						break;
				}

				if (returnType != null && returnType != parameterInfo.ParameterType)
				{
					if (returnType == typeof(string))
					{
						m_CodeBody += "TryParse<" + ReflectionUtil.GetCSharpTypeName(parameterInfo.ParameterType) + ">(" + newCodeBody + ")";
					}
                    else if (parameterInfo.ParameterType.IsClass)
                    {
                        m_CodeBody += newCodeBody + " as " + ReflectionUtil.GetCSharpTypeName(parameterInfo.ParameterType);
                    }
                    else
                    {
                        m_CodeBody += "(" + ReflectionUtil.GetCSharpTypeName(parameterInfo.ParameterType) + ")" + newCodeBody;
                    }
				}
				else
				{
					m_CodeBody += newCodeBody;
				}
			}
			else if(attributeListItem.ChildNodes.Count > 1)
			{
				StringBuffer newCodeBody = new StringBuffer();

				GenerateDoubleQuteStringAspxCode(newCodeBody, attributeListItem, scopeData);

				if (parameterInfo.ParameterType != typeof(string))
				{
					m_CodeBody += "TryParse<" + ReflectionUtil.GetCSharpTypeName(parameterInfo.ParameterType) + ">(" + newCodeBody + ")";
				}
				else
				{
					m_CodeBody += newCodeBody;
				}
			}
		}

		#endregion

		#region 输出模板文本

		private void GenerateLiteralAspxCode(TemplateElement literal)
		{
			GenerateLiteralAspxCode(m_CodeBody, literal);
		}

		private void GenerateLiteralAspxCode(StringBuffer codeBody, TemplateElement literal)
		{
			codeBody.InnerBuilder.Append(literal.SourceTemplate, literal.Index, literal.Length);
		}

		#endregion

		#region 输出模板变量

		private void GenerateVariableAspxCode(TemplateElement variable, ScopeData scopeData)
		{
			StringBuffer varBody = new StringBuffer();

			Type returnType = null;

			GenerateVariableAspxCode(varBody, variable, out returnType, scopeData);

			if(returnType != null && returnType != typeof(void))
				m_CodeBody += "<%=";
			else
                m_CodeBody += "<%\r\n";

			m_CodeBody += varBody;

			m_CodeBody += "%>";
		}

		private void GenerateVariableAspxCode(StringBuffer codeBody, TemplateElement variable, out Type returnType, ScopeData scopeData)
		{
			returnType = null;

			Type varType = null;
			string varName = null;
			string paramName = null;
			string name = (string)variable.Items[TemplateElement.KEY_NAME];

            if (scopeData.SearchScopeVariable(name, out varType, out varName))
			{
				StringBuffer newCodeBody = new StringBuffer();

				newCodeBody += varName;

				if (variable.ChildNodes.Count > 0)
					GenerateMemberInvokeAspxCode(newCodeBody, variable.ChildNodes[0], varType, out returnType, scopeData);

				if (returnType == null)
					returnType = varType;

				codeBody += newCodeBody.ToString();
			}
			else if (scopeData.SearchThisVariableProperty(name, out varType, out varName, out paramName))
			{
				StringBuffer newCodeBody = new StringBuffer();

				newCodeBody += varName + "." + paramName;

				if (variable.ChildNodes.Count > 0)
					GenerateMemberInvokeAspxCode(newCodeBody, variable.ChildNodes[0], varType, out returnType, scopeData);

				if (returnType == null)
					returnType = varType;

				codeBody += newCodeBody.ToString();
			}
			else
			{
				MemberInfo member = SearchMemberForVariable(name);

                if (member != null)
                {
                    string instanceName = DeclaringVariable(member.DeclaringType);

                    StringBuffer newCodeBody = new StringBuffer();

                    newCodeBody += instanceName + "." + member.Name;

                    Type memberType;

                    if (member.MemberType == MemberTypes.Property)
                        memberType = ((PropertyInfo)member).PropertyType;
                    else
                        memberType = ((FieldInfo)member).FieldType;

                    if (variable.ChildNodes.Count > 0)
                        GenerateMemberInvokeAspxCode(newCodeBody, variable.ChildNodes[0], memberType, out returnType, scopeData);

                    if (returnType == null)
                        returnType = memberType;

                    codeBody += newCodeBody.ToString();
                }
                else
                {

                    throw new TemplateVariableNotExistsException(name, variable.TemplateFile.FilePath, variable.SourceTemplate, variable.Index);
                    //codeBody += name;
                    //LogHelper.CreateErrorLog(new TemplateVariableNotExistsException(name, variable.TemplateFile.FilePath, variable.SourceTemplate, variable.Index));

                    //if (returnType == null)
                    //    returnType = typeof(string);
                }
			}
		}

		#endregion

		#region 输出模板函数

		private void GenerateFunctionAspxCode(TemplateElement function, ScopeData scopeData)
		{
            string name = (string)function.Items[TemplateElement.KEY_NAME];
			
            //if (StringUtil.EqualsIgnoreCase(name, "url"))
            //{
            //    GenerateUrlFunctionAspxCode(null, function, scopeData);
            //    return;
            //}

			StringBuffer funcBody = new StringBuffer();

			Type returnType = null;

			GenerateFunctionAspxCode(funcBody, function, out returnType, scopeData);

			if (returnType != null && returnType != typeof(void))
				m_CodeBody += "<%=";
			else
                m_CodeBody += "<%\r\n";

			m_CodeBody += funcBody;

			m_CodeBody += "%>";
		}

        private void GenerateUrlFunctionAspxCode(StringBuffer codeBody, TemplateElement function, ScopeData scopeData)
        {
            codeBody += "string.Concat(_Url_Before, ";

            for (int i = 0; i < function.ChildNodes[0].ChildNodes.Count; i++)
            {
                TemplateElement node = function.ChildNodes[0].ChildNodes[i];

                Type returnType;

                switch (node.Type)
                {
                    case TemplateElementTypes.Literal:
                        codeBody += "\"" + node.Text + "\"";
                        break;

                    case TemplateElementTypes.Variable:
                        GenerateVariableAspxCode(codeBody, node, out returnType, scopeData);
                        break;

                    case TemplateElementTypes.Function:
                        GenerateFunctionAspxCode(codeBody, node, out returnType, scopeData);
                        break;

                    case TemplateElementTypes.CodeBlock:
                        GenerateCodeBlockAspxCode(codeBody, node, scopeData);
                        break;

                    case TemplateElementTypes.DoubleQuteString:
                        GenerateDoubleQuteStringAspxCode(codeBody, node, scopeData);
                        break;

                    case TemplateElementTypes.SingleQuteString:
                        GenerateSingleQuteStringAspxCode(codeBody, node, scopeData);
                        break;
                }

                if (i < function.ChildNodes[0].ChildNodes.Count - 1)
                    codeBody += " + ";
            }

            codeBody += ", _Url_After)";
            //}
        }

		private void GenerateFunctionAspxCode(StringBuffer codeBody, TemplateElement function, out Type returnType, ScopeData scopeData)
		{
			returnType = null;

			Type funcReturnType = null;

			string thisVarName = null;
			string thisFuncName = null;

			string name = (string)function.Items[TemplateElement.KEY_NAME];

			if (StringUtil.EqualsIgnoreCase(name, "url"))
			{
				GenerateUrlFunctionAspxCode(codeBody, function, scopeData);
				returnType = typeof(string);
			}
			else if (StringUtil.EqualsIgnoreCase(name, "parent"))
			{
				bool onlyOneParam = true;
				TemplateElement funcParam = null;

				foreach (TemplateElement param in function.ChildNodes[0].ChildNodes)
				{
					if (param.Type == TemplateElementTypes.Literal)
						continue;

                    if (funcParam != null)
					{
						onlyOneParam = false;
						break;
					}

					funcParam = param;
				}

				if (onlyOneParam)
				{
					if (funcParam.Type == TemplateElementTypes.Variable)
						GenerateVariableAspxCode(codeBody, funcParam, out returnType, scopeData.Previous);
					else if (funcParam.Type == TemplateElementTypes.Function)
                        GenerateFunctionAspxCode(codeBody, funcParam, out returnType, scopeData.Previous);
				}
			}
            else if (StringUtil.EqualsIgnoreCase(name, "this"))
            {
                bool onlyOneParam = true;
                TemplateElement funcParam = null;

                foreach (TemplateElement param in function.ChildNodes[0].ChildNodes)
                {
                    if (param.Type == TemplateElementTypes.Literal)
                        continue;

                    if (funcParam != null)
                    {
                        onlyOneParam = false;
                        break;
                    }

                    funcParam = param;
                }

                if (onlyOneParam)
                {
                    if (funcParam.Type == TemplateElementTypes.Variable)
                        GenerateVariableAspxCode(codeBody, funcParam, out returnType, scopeData.Last);
                    else if (funcParam.Type == TemplateElementTypes.Function)
                        GenerateFunctionAspxCode(codeBody, funcParam, out returnType, scopeData.Last);
                }
            }
            else if (StringUtil.EqualsIgnoreCase(name, "out"))
            {
                funcReturnType = typeof(string);

                StringBuffer newCodeBody = new StringBuffer();

                GenerateOutFunctionAspxCode(newCodeBody, function, scopeData);

                if (function.ChildNodes.Count > 1)
                    GenerateMemberInvokeAspxCode(newCodeBody, function.ChildNodes[1], funcReturnType, out returnType, scopeData);

                if (returnType == null)
                    returnType = funcReturnType;

                codeBody += newCodeBody.ToString();
            }
            else if (scopeData.SearchThisVariableMethod(name, out funcReturnType, out thisVarName, out thisFuncName))
            {
                StringBuffer newCodeBody = new StringBuffer();

                newCodeBody += thisVarName + "." + thisFuncName + "(";

                GenerateAspxCode(newCodeBody, function.ChildNodes[0], scopeData);

                newCodeBody += ")";

                if (function.ChildNodes.Count > 1)
                    GenerateMemberInvokeAspxCode(newCodeBody, function.ChildNodes[1], funcReturnType, out returnType, scopeData);

                if (returnType == null)
                    returnType = funcReturnType;

                codeBody += newCodeBody.ToString();
            }
            else
            {
                MethodInfo method = SearchMethodForFunction(name);

                if (method != null)
                {
                    StringBuffer newCodeBody = new StringBuffer();

                    string instanceName = DeclaringVariable(method.DeclaringType);

                    newCodeBody += instanceName + "." + method.Name + "(";

                    GenerateAspxCode(newCodeBody, function.ChildNodes[0], scopeData);

                    newCodeBody += ")";

                    if (function.ChildNodes.Count > 1)
                        GenerateMemberInvokeAspxCode(newCodeBody, function.ChildNodes[1], method.ReturnType, out returnType, scopeData);

                    if (returnType == null)
                        returnType = method.ReturnType;

                    codeBody += newCodeBody.ToString();
                }
            }
		}

		private void GenerateOutFunctionAspxCode(StringBuffer codeBody, TemplateElement function, ScopeData scopeData)
		{
			if (function.ChildNodes[0].ChildNodes.Count > 0)
			{
				int defaultValueStartIndex = -1;

				TemplateElement param1 = null;

				if (function.ChildNodes[0].ChildNodes[0].Type == TemplateElementTypes.Variable || function.ChildNodes[0].ChildNodes[0].Type == TemplateElementTypes.Function)
				{
					param1 = function.ChildNodes[0].ChildNodes[0];

					if (function.ChildNodes[0].ChildNodes.Count >= 2)
						defaultValueStartIndex = 1;
				}
				else if (function.ChildNodes[0].ChildNodes.Count >= 2)
				{
					param1 = function.ChildNodes[0].ChildNodes[1];

					if (function.ChildNodes[0].ChildNodes.Count >= 3)
						defaultValueStartIndex = 2;
				}

				if (param1 != null && (param1.Type == TemplateElementTypes.Variable || param1.Type == TemplateElementTypes.Function))
				{
					codeBody += "TryToString(";

					Type returnType = null;

					if (param1.Type == TemplateElementTypes.Variable)
					{
						TemplateElement param0 = new TemplateElement(TemplateElementTypes.Variable, param1.Items);

						GenerateVariableAspxCode(codeBody, param0, out returnType, scopeData);
					}
					else
					{
						TemplateElement param0 = new TemplateElement(TemplateElementTypes.Function, param1.Items);

						param0.ChildNodes.Add(param1.ChildNodes[0]);

						GenerateFunctionAspxCode(codeBody, param0, out returnType, scopeData);
					}

					codeBody += ", delegate(){ return ";

					if (param1.Type == TemplateElementTypes.Variable)
						GenerateVariableAspxCode(codeBody, param1, out returnType, scopeData);
					else
						GenerateFunctionAspxCode(codeBody, param1, out returnType, scopeData);

					codeBody += "; }";

					if (defaultValueStartIndex > 0)
					{
						for (int i = defaultValueStartIndex; i < function.ChildNodes[0].ChildNodes.Count; i++)
						{
							TemplateElement node = function.ChildNodes[0].ChildNodes[i];

							switch (node.Type)
							{
								case TemplateElementTypes.Literal:
									GenerateLiteralAspxCode(codeBody, node);
									break;

								case TemplateElementTypes.Variable:
									GenerateVariableAspxCode(codeBody, node, out returnType, scopeData);
									break;

								case TemplateElementTypes.Function:
									GenerateFunctionAspxCode(codeBody, node, out returnType, scopeData);
									break;

								case TemplateElementTypes.CodeBlock:
									GenerateCodeBlockAspxCode(codeBody, node, scopeData);
									break;

								case TemplateElementTypes.SingleQuteString:
									GenerateSingleQuteStringAspxCode(codeBody, node, scopeData);
									break;

								case TemplateElementTypes.DoubleQuteString:
									GenerateDoubleQuteStringAspxCode(codeBody, node, scopeData);
									break;
							}
						}
					}

					codeBody += ")";
				}
			}
		}

		#endregion

		#region 输出代码块

		private void GenerateCodeBlockAspxCode(TemplateElement codeBlock, ScopeData scopeData)
		{
			bool isOutput = (bool)codeBlock.Items[TemplateElement.KEY_OUTPUT];

            m_CodeBody += isOutput ? "<%=" : "<%\r\n";

			GenerateCodeBlockAspxCode(m_CodeBody, codeBlock, scopeData);
			
			m_CodeBody += isOutput ? "%>" : ";%>";
		}

		private void GenerateCodeBlockAspxCode(StringBuffer codeBody, TemplateElement codeBlock, ScopeData scopeData)
		{
			foreach (TemplateElement element in codeBlock.ChildNodes)
			{
				Type returnType = null;

				switch (element.Type)
				{
					case TemplateElementTypes.Literal:
						GenerateLiteralAspxCode(codeBody, element);
						break;

					case TemplateElementTypes.Variable:
						GenerateVariableAspxCode(codeBody, element, out returnType, scopeData);
						break;

					case TemplateElementTypes.Function:
						GenerateFunctionAspxCode(codeBody, element, out returnType, scopeData);
						break;
				}
			}
		}

		#endregion

		#region 输出成员调用

		private void GenerateMemberInvokeAspxCode(StringBuffer codeBody, TemplateElement memberInvoke, Type type, out Type returnType, ScopeData scopeData)
		{
			returnType = null;

			switch (memberInvoke.Type)
			{

				case TemplateElementTypes.IndexInvoke:
					GenerateIndexInvokeAspxCode(codeBody, memberInvoke, type, out returnType, scopeData);
					break;

				case TemplateElementTypes.PropertyInvoke:
					GeneratePropertyInvokeAspxCode(codeBody, memberInvoke, type, out returnType, scopeData);
					break;

				case TemplateElementTypes.FunctionInvoke:
					GenerateFunctionInvokeAspxCode(codeBody, memberInvoke, type, out returnType, scopeData);
					break;
			}
		}

		private void GenerateIndexInvokeAspxCode(StringBuffer codeBody, TemplateElement indexInvoke, Type type, out Type returnType, ScopeData scopeData)
		{
			returnType = null;

			Type indexType = null;

			string name = indexInvoke.Items[TemplateElement.KEY_NAME] as string;

			if (type.IsArray)
			{
				indexType = type.GetElementType();
			}
			else
			{
				PropertyInfo property = type.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

				if (property != null)
					indexType = property.PropertyType;
			}

			if (indexType != null)
			{
				codeBody += "[";

				GenerateAspxCode(codeBody, indexInvoke.ChildNodes[0], scopeData);

				codeBody += "]";

				if (indexInvoke.ChildNodes.Count > 1)
					GenerateMemberInvokeAspxCode(codeBody, indexInvoke.ChildNodes[1], indexType, out returnType, scopeData);

				if (returnType == null)
					returnType = indexType;
			}
		}

		private void GeneratePropertyInvokeAspxCode(StringBuffer codeBody, TemplateElement propertyInvoke, Type type, out Type returnType, ScopeData scopeData)
		{
			returnType = null;

			string name = propertyInvoke.Items[TemplateElement.KEY_NAME] as string;

			MethodInfo extensionProperty = SearchExtensionProperty(name, type);

			if (extensionProperty != null)
			{
				StringBuffer sb = new StringBuffer(ReflectionUtil.GetCSharpTypeName(extensionProperty.DeclaringType));

				sb += "." + extensionProperty.Name + "((";

				codeBody.InnerBuilder.Insert(0, sb);

				codeBody += "),\"" + name + "\")";

				if (propertyInvoke.ChildNodes.Count > 0)
					GenerateMemberInvokeAspxCode(codeBody, propertyInvoke.ChildNodes[0], extensionProperty.ReturnType, out returnType, scopeData);
				
				if(returnType == null)
					returnType = extensionProperty.ReturnType;
			}
			else
			{
				MemberInfo member = SearchPropertyAndField(name, type);

				Type magicPropertyType = null;

				if (member != null)
				{
					codeBody += "." + member.Name;

                    Type memberType;

                    if (member.MemberType == MemberTypes.Property)
                        memberType = ((PropertyInfo)member).PropertyType;

                    else
                        memberType = ((FieldInfo)member).FieldType;

					if (propertyInvoke.ChildNodes.Count > 0)
                        GenerateMemberInvokeAspxCode(codeBody, propertyInvoke.ChildNodes[0], memberType, out returnType, scopeData);

					if(returnType == null)
                        returnType = memberType;
				}
				else if (HasMagicProperty(type, out magicPropertyType))
				{
					codeBody += "[\"" + name + "\"]";

					if (propertyInvoke.ChildNodes.Count > 0)
						GenerateMemberInvokeAspxCode(codeBody, propertyInvoke.ChildNodes[0], magicPropertyType, out returnType, scopeData);
				
					if(returnType == null)
						returnType = magicPropertyType;
				}
			}
		}

		private void GenerateFunctionInvokeAspxCode(StringBuffer codeBody, TemplateElement functionInvoke, Type type, out Type returnType, ScopeData scopeData)
		{
			returnType = null;

			string name = functionInvoke.Items[TemplateElement.KEY_NAME] as string;

			if (name == null)
			{
				#region 委托数组的调用
				if (type.IsSubclassOf(typeof(Delegate)) == false)
				{
					codeBody.InnerBuilder.Append(functionInvoke.SourceTemplate, functionInvoke.Index, functionInvoke.Length);
				}
				else
				{
					codeBody += "(";

					GenerateAspxCode(codeBody, functionInvoke.ChildNodes[0], scopeData);

					codeBody += ")";

					if (functionInvoke.ChildNodes.Count > 1)
						GenerateMemberInvokeAspxCode(codeBody, functionInvoke.ChildNodes[1], type.GetMethod("Invoke").ReturnType, out returnType, scopeData);
					
					if(returnType == null)
						returnType = type.GetMethod("Invoke").ReturnType;
				}
				#endregion
			}
			else
			{
				MethodInfo extensionFunction = SearchExtensionFunction(name, type);

				if (extensionFunction != null)
				{
					#region 扩展函数的调用

                    StringBuffer sb = new StringBuffer(ReflectionUtil.GetCSharpTypeName(extensionFunction.DeclaringType));

					sb += "." + extensionFunction.Name + "(";

					codeBody.InnerBuilder.Insert(0, sb);

					codeBody += ",\"" + name + "\"";

					if (extensionFunction.GetParameters().Length > 2)
					{
						codeBody += ",";

						GenerateAspxCode(codeBody, functionInvoke.ChildNodes[0], scopeData);
					}

					codeBody += ")";

					if (functionInvoke.ChildNodes.Count > 1)
						GenerateMemberInvokeAspxCode(codeBody, functionInvoke.ChildNodes[1], extensionFunction.ReturnType, out returnType, scopeData);
					
					if(returnType == null)
						returnType = extensionFunction.ReturnType;

					#endregion
				}
				else
				{
					#region 普通函数调用

					MethodInfo method = SearchMethod(name, type);

					if (method != null)
					{
						codeBody += "." + method.Name + "(";

						GenerateAspxCode(codeBody, functionInvoke.ChildNodes[0], scopeData);

						codeBody += ")";

						if (functionInvoke.ChildNodes.Count > 1)
							GenerateMemberInvokeAspxCode(codeBody, functionInvoke.ChildNodes[1], method.ReturnType, out returnType, scopeData);
						
						if(returnType == null)
							returnType = method.ReturnType;
					}

					#endregion
				}
			}
		}

		#endregion

		#region 输出字符串

		private void GenerateSingleQuteStringAspxCode(StringBuffer codeBody, TemplateElement node, ScopeData scopeData)
		{
			if (node.ChildNodes.Count == 0)
			{
				codeBody += "\"\"";
				return;
			}

			for (int j = 0; j < node.ChildNodes.Count; j++)
			{
				TemplateElement element2 = node.ChildNodes[j];

				Type returnType = null;

				switch (element2.Type)
				{
					case TemplateElementTypes.Literal:
						codeBody += "\"";

						string content = element2.Text.Replace("\\'", "'").Replace("\"", "\\\"");

						codeBody += content;

						codeBody += "\"";
						break;

					case TemplateElementTypes.Variable:
						GenerateVariableAspxCode(codeBody, element2, out returnType, scopeData);
						break;

					case TemplateElementTypes.Function:
						GenerateFunctionAspxCode(codeBody, element2, out returnType, scopeData);
						break;

					case TemplateElementTypes.CodeBlock:
						GenerateCodeBlockAspxCode(codeBody, element2, scopeData);
						break;
				}

				if (j < node.ChildNodes.Count - 1)
					codeBody += " + ";
			}
		}

		private void GenerateDoubleQuteStringAspxCode(StringBuffer codeBody, TemplateElement node, ScopeData scopeData)
		{
			if (node.ChildNodes.Count == 0)
			{
				codeBody += "\"\"";
				return;
			}

			for (int j = 0; j < node.ChildNodes.Count; j++)
			{
				TemplateElement element2 = node.ChildNodes[j];

				Type returnType = null;

				switch (element2.Type)
				{
					case TemplateElementTypes.Literal:
						codeBody += "\"";
						GenerateLiteralAspxCode(codeBody, element2);
						codeBody += "\"";
						break;

					case TemplateElementTypes.Variable:
						GenerateVariableAspxCode(codeBody, element2, out returnType, scopeData);
						break;

					case TemplateElementTypes.Function:
						GenerateFunctionAspxCode(codeBody, element2, out returnType, scopeData);
						break;

					case TemplateElementTypes.CodeBlock:
						GenerateCodeBlockAspxCode(codeBody, element2, scopeData);
						break;
				}

				if (j < node.ChildNodes.Count - 1)
					codeBody += " + ";
			}
		}

		#endregion

		#region 处理生产后的代码中的变量

		private static readonly Type typeofPageBase = typeof(PageBase);
		private static readonly Type typeofPagePartBase = typeof(PagePartBase);

		private Dictionary<Type, string> m_DeclaredVariables = new Dictionary<Type, string>(5);

		/// <summary>
		/// 声明变量（不会重复声明同一类型的变量）
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private string DeclaringVariable(Type type)
		{
			if (type == typeofPageBase || type == typeofPagePartBase 
			|| type.IsSubclassOf(typeofPageBase) || type.IsSubclassOf(typeofPagePartBase))
				return "this";

			string variableName = null;

			if(m_DeclaredVariables.TryGetValue(type, out variableName) == false)
			{
				variableName = "var" + m_DeclaredVariables.Count; // type.FullName.Replace("_", "__").Replace(".", "_").Replace("+", "_");

				m_DeclaredVariables.Add(type, variableName);
			}
			
			return variableName;
		}

		private class ScopeData
		{
			public ScopeData(ScopeData previous)
			{
				Previous = previous;
                if (previous != null)
                    Previous.Next = this;
			}

            public ScopeData Last
            {
                get
                {
                    if (Next == null)
                        return this;

                    return Next.Last;
                }
            }

            public ScopeData Next
            {
                get;
                private set;
            }

			public ScopeData Previous
			{
				get;
				private set;
			}

			private Hashtable m_DeclaredScopeVariables;

			private Hashtable DeclaredScopeVariables
			{
				get
				{
					if(m_DeclaredScopeVariables == null)
						m_DeclaredScopeVariables = CollectionsUtil.CreateCaseInsensitiveHashtable();

					return m_DeclaredScopeVariables;
				}
			}

			private int? m_VarID = null;

			private int GetScopeVariableID()
			{
				if (Previous != null)
					return Previous.GetScopeVariableID()  + 1;

				return 0;
			}

			public string DeclaringScopeVariable(string name, Type type)
			{
				if (m_VarID == null)
					m_VarID = GetScopeVariableID();

				string result = name.Replace("_", "__") + "_" + m_VarID;

				DeclaredScopeVariables.Add(name, new KeyValuePair<string, RuntimeTypeHandle>(result, type.TypeHandle));

				if (name == "_this")
					DeclaringThisVariable(result, type);

				return result;
			}

			public string DeclaringScopeVariable(ParameterInfo parameter)
			{
				return DeclaringScopeVariable(parameter.Name, parameter.ParameterType);
			}

			public bool SearchScopeVariable(string name, out Type varType, out string varName)
			{
				object info = DeclaredScopeVariables[name];

				if (info != null)
				{
					KeyValuePair<string, RuntimeTypeHandle> varInfo = (KeyValuePair<string, RuntimeTypeHandle>)info;

					varType = Type.GetTypeFromHandle(varInfo.Value);
					varName = varInfo.Key;

					return true;
				}
				else if (Previous != null)
				{
					return Previous.SearchScopeVariable(name, out varType, out varName);
				}

				varType = null;
				varName = null;

				return false;
			}

			private Type m_thisVarType;
			private string m_thisVarName;

			private void DeclaringThisVariable(string thisVarName, Type thisVarType)
			{
				m_thisVarName = thisVarName;
				m_thisVarType = thisVarType;
			}

			public bool SearchThisVariableProperty(string name, out Type propertyType, out string thisVarName, out string propertyName)
			{
				if (m_thisVarName != null)
				{
					MemberInfo member = SearchPropertyAndField(name, m_thisVarType);

                    if (member != null)
					{
                        if (member.MemberType == MemberTypes.Property)
    						propertyType = ((PropertyInfo)member).PropertyType;
                        else
                            propertyType = ((FieldInfo)member).FieldType;

						thisVarName = m_thisVarName;
                        propertyName = member.Name;

                        return true;
					}
				}

				propertyType = null;
				thisVarName = null;
				propertyName = null;

				return false;
			}

			public bool SearchThisVariableMethod(string name, out Type returnType, out string thisVarName, out string methodName)
			{
				if (m_thisVarName != null)
				{
					MethodInfo method = SearchMethod(name, m_thisVarType);

					if (method != null)
					{
						returnType = method.ReturnType;
						thisVarName = m_thisVarName;
						methodName = method.Name;

                        return true;
					}
				}

				returnType = null;
				thisVarName = null;
				methodName = null;

				return false;
			}
		}

		#endregion

		#region 助手方法

        private void TryBuildTempWebConfig()
        {
            string webConfigFilePath = IOUtil.JoinPath(IOUtil.MapPath(Config.Current.TemplateOutputPath), "web.config");

            if (File.Exists(webConfigFilePath) == false)
            {
                using (StreamWriter writer = new StreamWriter(webConfigFilePath, false, Encoding.UTF8))
                {
                    #region 输出 web.config 文件的内容

                    writer.Write(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
    <system.web>
    <pages enableSessionState=""true"" enableViewState=""true"" validateRequest=""false"" pageBaseType=""MaxLabs.WebEngine.PageBase"" userControlBaseType=""MaxLabs.WebEngine.PagePartBase"">
            <namespaces>
                <clear/>
                <add namespace=""System""/>
                <add namespace=""System.Collections""/>
                <add namespace=""System.Collections.Specialized""/>
                <add namespace=""System.Configuration""/>
                <add namespace=""System.Text""/>
                <add namespace=""System.Text.RegularExpressions""/>
                <add namespace=""System.Web""/>
                <add namespace=""System.Web.Caching""/>
                <add namespace=""System.Web.SessionState""/>
                <add namespace=""System.Web.Security""/>
                <add namespace=""System.Web.UI""/>
                <add namespace=""System.Web.UI.WebControls""/>
                <add namespace=""System.Web.UI.HtmlControls""/>

                <add namespace=""System.IO""/>
                <add namespace=""System.Drawing""/>
                <add namespace=""System.Drawing.Text""/>
                <add namespace=""System.Drawing.Imaging""/>

                <add namespace=""MaxLabs.bbsMax""/>
                <add namespace=""MaxLabs.bbsMax.Common""/>
                <add namespace=""MaxLabs.bbsMax.Errors""/>
                <add namespace=""MaxLabs.bbsMax.Enums""/>
                <add namespace=""MaxLabs.bbsMax.Entities""/>
                <add namespace=""MaxLabs.bbsMax.DataAccess""/>
                <add namespace=""MaxLabs.bbsMax.Extensions""/>
                <add namespace=""MaxLabs.bbsMax.Extensions.Actions""/>
                <add namespace=""MaxLabs.bbsMax.Settings""/>
                <add namespace=""MaxLabs.bbsMax.Email""/>
                <add namespace=""MaxLabs.bbsMax.ValidateCodes""/>

                <add namespace=""MaxLabs.WebEngine""/>
                <add namespace=""MaxLabs.WebEngine.Plugin""/>
                <add namespace=""MaxLabs.WebEngine.Template""/>
            </namespaces>
        </pages>
    </system.web>
</configuration>
");

                    #endregion
                }
            }
        }

        private string GetNewCheckString(string fullTemplate)
        {
            //DateTime fileLastWriteTime = File.GetLastWriteTimeUtc(path);
            string md5 = SecurityUtil.MD5(fullTemplate);

            DateTime libLastWriteTime = File.GetLastWriteTimeUtc(IOUtil.JoinPath(Globals.BinDirectory, "MaxLabs.bbsMax.dll"));
            DateTime webLibLastWriteTime = File.GetLastWriteTimeUtc(IOUtil.JoinPath(Globals.BinDirectory, "MaxLabs.bbsMax.Web.dll"));

            DateTime result;

            if (webLibLastWriteTime >= libLastWriteTime)
                result = webLibLastWriteTime;

            else
                result = libLastWriteTime;

            return md5 + result.ToFileTime().ToString();
        }

        private string ReadCheckString(TemplateFile templateFile)
        {
            string checkString = null;

            if (File.Exists(templateFile.ParsedFilePath) == false)
                return null;

            using (FileStream stream = File.Open(templateFile.ParsedFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                {
                    checkString = reader.ReadLine();
                }
            }

            if (string.IsNullOrEmpty(checkString) == false && StringUtil.StartsWith(checkString, "<%--") && StringUtil.EndsWith(checkString, "--%>"))
            {
                return checkString.Substring(4, checkString.Length - 8);
            }

            return null;
        }

        //private string GetTemplateFileMD5(string path)
        //{
        //    string md5 = null;
        //    using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
        //        {
        //            md5 = reader.ReadLine();
        //        }
        //    }

        //    if (string.IsNullOrEmpty(md5) == false && md5.Length == 40 && md5.StartsWith("<%--") && md5.EndsWith("--%>"))
        //    {

        //        md5 = md5.Substring(4, 32);

        //        return md5;
        //    }

        //    return null;
        //}

		private static bool HasMagicProperty(Type type, out Type propertyType)
		{
			foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
			{
				if (property.IsDefined(typeof(TemplateMagicPropertyAttribute), true))
				{
					propertyType = property.PropertyType;
					return true;
				}
			}

			propertyType = null;

			return false;
		}

		/// <summary>
		/// 获取对应于指定参数的模板标签属性
		/// </summary>
		/// <param name="parameterInfo">参数信息</param>
		/// <param name="attributeList">模板标签属性列表</param>
		/// <returns></returns>
		private static TemplateElement GetAttributeListItem(ParameterInfo parameterInfo, TemplateElement attributeList)
		{
			foreach (TemplateElement attributeListItem in attributeList.ChildNodes)
			{
				string name = (string)attributeListItem.Items[TemplateElement.KEY_NAME];

				if (StringUtil.EqualsIgnoreCase(name, parameterInfo.Name))
				{
					return attributeListItem;
				}
			}

			return null;
		}

		private static MemberInfo SearchPropertyAndField(string name, Type type)
		{

            MemberInfo[] members = type.GetMember(name, BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (members != null && members.Length > 0)
            {

                foreach (MemberInfo member in members)
                {
                    if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                        return member;
                }

            }

            //foreach (MemberInfo property in type.GetMembers( BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            //{
            //    if (StringUtil.EqualsIgnoreCase(name, property.Name))
            //        return property;
            //}

            return null;
		}

		/// <summary>
		/// 获取类型中的方法（不区分大小写）
		/// </summary>
		/// <param name="name">方法名</param>
		/// <param name="type">类型</param>
		/// <returns></returns>
		private static MethodInfo SearchMethod(string name, Type type)
		{
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                if (StringUtil.EqualsIgnoreCase(name, method.Name))
                    return method;
            }

            return null;
		}

		/// <summary>
		/// 在一组方法中寻找参数和模板标签属性列表一致的方法
		/// </summary>
		/// <param name="methods">方法集合</param>
		/// <param name="attributeList">模板标签属性列表</param>
		/// <param name="ignoreDelegate">是否忽略委托类型参数</param>
		/// <returns></returns>
		private static MethodInfo SearchMethod(IList<RuntimeMethodHandle> methodHandles, TemplateElement attributeList, bool ignoreDelegate)
		{
            foreach (RuntimeMethodHandle methodHandle in methodHandles)
			{
                MethodInfo method = (MethodInfo)MethodInfo.GetMethodFromHandle(methodHandle);

				ParameterInfo[] parameters = method.GetParameters();

				if (ignoreDelegate)
				{
					List<ParameterInfo> noDelegateParameters = new List<ParameterInfo>();

					foreach (ParameterInfo param in parameters)
					{
						if (param.ParameterType.IsSubclassOf(typeof(Delegate)))
							continue;

						noDelegateParameters.Add(param);
					}

					parameters = noDelegateParameters.ToArray();
				}

				if (parameters.Length != attributeList.ChildNodes.Count)
					continue;

				if (parameters.Length == 0 && attributeList.ChildNodes.Count == 0)
					return method;

				bool foundMethod = true;

				foreach (ParameterInfo parameter in parameters)
				{
					bool foundParam = false;

					foreach (TemplateElement attributeListItem in attributeList.ChildNodes)
					{
						string name = (string)attributeListItem.Items[TemplateElement.KEY_NAME];

						if (StringUtil.EqualsIgnoreCase(name, parameter.Name))
						{
							foundParam = true;
							break;
						}
					}

					if (foundParam == false)
					{
						foundMethod = false;
						break;
					}
				}

				if (foundMethod)
					return method;
			}

			return null;
		}

		#endregion

        #region IDisposable 成员

        public void Dispose()
        {
            m_Document = null;
            m_CodeBody = null;
            m_CodeHead = null;
            m_DeclaredVariables = null;
            m_ExtensionFunctions = null;
            m_ExtensionProperties = null;
            m_MethodBasedFunctions = null;
            m_MethodBasedTags = null;
            m_PropertyBasedVariables = null;

            try
            {
                System.GC.Collect();
            }
            catch
            {
                LogHelper.CreateDebugLog("模板处理完毕，垃圾回收失败，可能会占用较高内存");
            }
        }

        #endregion
    }
}