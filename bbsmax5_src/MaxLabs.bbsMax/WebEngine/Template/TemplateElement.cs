//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using MaxLabs.bbsMax;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Rescourses;


namespace MaxLabs.WebEngine.Template
{
	/// <summary>
	/// 模板语法树的节点
	/// </summary>
	internal sealed class TemplateElement
	{
		public const string KEY_NAME = "name";
		public const string KEY_OUTPUT = "output";
		public const string KEY_CLOSED = "closed";

		private const string SNP_BEGIN = "$";
		private const string BLK_BEGIN = "{";
		private const string OUT_BEGIN = "{=";
		private const string TAG_BEGIN = "<!--[";
		private const string STR_BEGIN = "\"";
		private const string CHR_BEGIN = "'";

		private static readonly Regex TemplateTagEndRegex = new TemplateTagEndRegex();
		private static readonly Regex TemplateTagBeginRegex = new TemplateTagBeginRegex();
		private static readonly Regex TemplateCodeBlockRegex = new TemplateCodeBlockRegex();
		private static readonly Regex TemplateCodeSnippetRegex = new TemplateCodeSnippetRegex();
		private static readonly Regex TemplateMemberInvokeRegex = new TemplateMemberInvokeRegex();
		private static readonly Regex TemplateAttributeListRegex = new TemplateAttributeListRegex();
		private static readonly Regex TemplateDoubleQuteStringRegex = new TemplateDoubleQuteStringRegex();
		private static readonly Regex TemplateSingleQuteStringRegex = new TemplateSingleQuteStringRegex();
		private static readonly Regex TemplateLoopExpressionParamRegex = new TemplateLoopExpressionParamRegex();
		private static readonly Regex TemplateLoopExpressionParamRegex2 = new TemplateLoopExpressionParamRegex2();

		internal TemplateElement(TemplateElementTypes type, IDictionary items)
		{
			Type = type;

			m_Items = items;
		}

		private TemplateElement(string sourceTemplate, TemplateFile templateFile)
		{
			this.Document = this;
			this.TemplateFile = templateFile;

			Type = TemplateElementTypes.Document;

			Index = 0;
			Length = sourceTemplate.Length;

			SourceTemplate = sourceTemplate;

			if (sourceTemplate == string.Empty || Length == 0)
				return;

			ParseTemplateTag(Index, Length);
		}

        private TemplateElement(TemplateElement document, TemplateElementTypes type, int index, int length, IDictionary items)
		{
            this.Document = document;
            this.TemplateFile = document.TemplateFile;

			Type = type;

			Index = index;
			Length = length;

			m_Items = items;

            SourceTemplate = document.SourceTemplate;

            if (SourceTemplate == string.Empty || length == 0)
				return;

			switch (type)
			{
				case TemplateElementTypes.Tag:
					if ((bool)Items[KEY_CLOSED] == false)
						ParseTemplateTag(Index, Length);
					break;

				case TemplateElementTypes.Document:
				case TemplateElementTypes.AjaxPanel:
				case TemplateElementTypes.IfExpression:
				case TemplateElementTypes.LoopExpression:
					ParseTemplateTag(Index, Length);
					break;

				case TemplateElementTypes.AttributeList:
					ParseAttributeList(Index, Length);
					break;

				case TemplateElementTypes.IndexInvokeParam:
				case TemplateElementTypes.FunctionInvokeParam:
					ParseTemplateCode(Index, Length, false, true);
					break;

				case TemplateElementTypes.CodeBlock:
				case TemplateElementTypes.DoubleQuteString:
				case TemplateElementTypes.SingleQuteString:
				case TemplateElementTypes.AttributeListItem:
				case TemplateElementTypes.ConditionExpression:
					ParseTemplateCode(Index, Length);
				    break;

				case TemplateElementTypes.LoopExpressionParam:
					ParseLoopExpressionParam(Index, Length);
					break;
			}
		}

		private TemplateFile m_TemplateFile;

		public TemplateFile TemplateFile
		{
			get { return m_TemplateFile; }
			set { m_TemplateFile = value; }
		}

		private TemplateElement m_Document;

		public TemplateElement Document
		{
			get { return m_Document;  }
			set { m_Document = value; }
		}

		private TemplateElement m_Parent;

		public TemplateElement Parent
		{
			get { return m_Parent; }
			set { m_Parent = value; }
		}

		private TemplateElementTypes m_Type;

		public TemplateElementTypes Type 
		{
			get { return m_Type; }
			private set { m_Type = value; }
		}

		private int m_Index;

		public int Index 
		{
			get { return m_Index; }
			private set { m_Index = value; }
		}

		private int m_Length;

		public int Length 
		{
			get { return m_Length; }
			private set { m_Length = value; }
		}

		private string m_SourceTemplate;

		public string SourceTemplate 
		{
			get { return m_SourceTemplate; }
			private set { m_SourceTemplate = value; }
		}

		public string Text
		{
			get 
			{
				return SourceTemplate.Substring(Index, Length);
			}
		}

		private IDictionary m_Items;

		public IDictionary Items 
		{
			get
			{
				if (m_Items == null)
					m_Items = new ListDictionary();

				return m_Items;
			}
		}

		private TemplateElementCollection m_ChildNodes;

		public TemplateElementCollection ChildNodes
		{
			get
			{
				if (m_ChildNodes == null)
					m_ChildNodes = new TemplateElementCollection(this);

				return m_ChildNodes;
			}
		}

		#region 解析模板

		private void ParseTemplateTag(int index, int length)
		{
			int tagBegin = SourceTemplate.IndexOf(TAG_BEGIN, index, length);

			if (tagBegin >= 0)
			{
				ParseTemplateCode(index, tagBegin - index, true, false);

				length = length - (tagBegin - index);
				index = tagBegin;

				Match match = null;

				if ((match = TemplateTagBeginRegex.Match(SourceTemplate, index, length)).Success)
				{
					TemplateElement tag = CreateTag(match);

					ChildNodes.Add(tag);

                    //if (tag.Type == TemplateElementTypes.PreIncludeExpression)
                    //{
                    //    if (tag.ChildNodes[0].ChildNodes.Count >= 1)
                    //    {
                    //        TemplateElement element = tag.ChildNodes[0].ChildNodes[0];

                    //        if (element.ChildNodes[0].Type == TemplateElementTypes.Literal)
                    //        {
                    //            string path = element.ChildNodes[0].Text;

                    //            if (path.StartsWith("/") == false && path.StartsWith("~/") == false)
                    //            {
                    //                path = this.Document.TemplateFile.Owner.VirtualPath + path;
                    //            }

                    //            path = HttpContext.Current.Server.MapPath(path);

                    //            if (File.Exists(path))
                    //            {
                    //                string content = File.ReadAllText(path);

                    //                TemplateElement doc = CreateDocument(content, null);

                    //                tag.ChildNodes.Add(doc);

                    //                this.Document.TemplateFile.AddWatcher(path);
                    //            }
                    //        }
                    //    }
                    //}

                    if (tag.Items.Contains("END") == false)
                        throw new TemplateTagNotCloseException(tag.Items["name"].ToString(), TemplateFile.FilePath, SourceTemplate, index);

					int tagEndIndex = (int)tag.Items["END"];

					if (tagEndIndex > 0)
					{
						int leavingLength = SourceTemplate.Length - tagEndIndex;

						if (leavingLength > 0)
						{
							ParseTemplateTag(tagEndIndex, leavingLength);
						}
					}
				}
				else if ((match = TemplateTagEndRegex.Match(SourceTemplate, index, length)).Success)
				{
					if ((Type == TemplateElementTypes.Tag || Type == TemplateElementTypes.IfExpression || Type == TemplateElementTypes.LoopExpression || Type == TemplateElementTypes.AjaxPanel) 
					&& StringUtil.EqualsIgnoreCase(match.Groups["name"].Value, (string)Items[KEY_NAME]))
					{
						Length = match.Index - Index;

						Items["END"] = match.Index + match.Length;
					}
					else
					{
						ChildNodes.Add(CreateLiteral(match.Index, match.Length));

						int tagEnd = match.Index + match.Length;

						ParseTemplateTag(tagEnd, length - (tagEnd - index));
					}
				}
				else
				{
					ChildNodes.Add(CreateLiteral(tagBegin, TAG_BEGIN.Length));

					ParseTemplateTag(tagBegin + TAG_BEGIN.Length, length - TAG_BEGIN.Length);
				}
			}
			else
			{
				ParseTemplateCode(index, length, true, false);
			}
		}

		private void ParseLoopExpressionParam(int index, int length)
		{
			Match match = TemplateLoopExpressionParamRegex.Match(SourceTemplate, index, length);

			if (match.Success)
			{
				Group i = match.Groups["i"];
				Group variable = match.Groups["var"];
				Group target = match.Groups["target"];

				if (variable.Success && target.Success)
				{
					if(i.Success)
						Items["i"] = i.Value;

					ParseTemplateCode(variable.Index, variable.Length);
					ParseTemplateCode(target.Index, target.Length);
				}
			}
			else
			{
				match = TemplateLoopExpressionParamRegex2.Match(SourceTemplate, index, length);

				if (match.Success)
				{
					Group i = match.Groups["i"];
					Group to = match.Groups["to"];
					Group from = match.Groups["from"];
					Group step = match.Groups["step"];

					if (i.Success && to.Success && from.Success)
					{
						Items["i"] = i.Value;

						if(step.Success)
							Items["step"] = step.Value;

						ParseTemplateCode(from.Index, from.Length);
						ParseTemplateCode(to.Index, to.Length);
					}

					this.Type = TemplateElementTypes.LoopExpressionParam2;
				}
			}
		}

		private void ParseTemplateCode(int index, int length)
		{
			ParseTemplateCode(index, length, false, false);
		}

		private void ParseTemplateCode(int index, int length, bool output, bool processString)
		{
			if (length == 0)
				return;

			string b = string.Empty;

			string blkBegin = OUT_BEGIN;// output ? OUT_BEGIN : BLK_BEGIN;

			string[] beginTokens = null;

			if (processString)
				beginTokens = new string[] { blkBegin, SNP_BEGIN, STR_BEGIN, CHR_BEGIN };
			else
				beginTokens = new string[] { blkBegin, SNP_BEGIN};

			int begin = StringUtil.FirstIndexOf(SourceTemplate, index, length, out b, beginTokens);

			if (begin >= 0)
			{
				if (begin - index > 0)
				{
					ChildNodes.Add(CreateLiteral(index, begin - index));
				}

				length = length - (begin - index);
				index = begin;

				Match match = null;
				TemplateElement node = null;

				switch (b)
				{
					case STR_BEGIN:
						if ((match = TemplateDoubleQuteStringRegex.Match(SourceTemplate, index, length)).Success)
						{
							node = CreateDoubleQuteString(match);
						}
						break;

					case CHR_BEGIN:
						if ((match = TemplateSingleQuteStringRegex.Match(SourceTemplate, index, length)).Success)
						{
							node = CreateSingleQuteString(match);
						}
						break;

					case SNP_BEGIN:
						if ((match = TemplateCodeSnippetRegex.Match(SourceTemplate, index, length)).Success)
						{
							node = CreateCodeSnippet(match);
						}
						break;

					default:
						if ((match = TemplateCodeBlockRegex.Match(SourceTemplate, index, length)).Success)
						{
							node = CreateCodeBlock(match);
						}
						break;
				}

				if (node != null)
				{
					ChildNodes.Add(node);

					int endIndex = match.Index + match.Length;
					int leavingLength = length - (endIndex - index);

					if (leavingLength > 0)
						ParseTemplateCode(endIndex, leavingLength, output, processString);
				}
				else
				{
					ChildNodes.Add(CreateLiteral(index, b.Length));

					ParseTemplateCode(index + 1, length - 1, output, processString);
				}
			}
			else
			{
				ChildNodes.Add(CreateLiteral(index, length));
			}
		}

		private void ParseAttributeList(int index, int length)
		{
			Match match = TemplateAttributeListRegex.Match(SourceTemplate, index, length);

			while (match.Success)
			{
				ChildNodes.Add(CreateAttributeListItem(index, match));

				match = match.NextMatch();
			}
		}

		private void ParseMemberInvoke(int index, int length)
		{
			Match match = TemplateMemberInvokeRegex.Match(SourceTemplate, index, length);

			if (match.Success)
			{
				ChildNodes.Add(CreateMemberInvokeListItem(match));
			}
		}

		#endregion

		#region 创建节点

		public static TemplateElement CreateDocument(string template, TemplateFile templateFile)
		{
			TemplateElement doc = new TemplateElement(template, templateFile);

			return doc;
		}

		public TemplateElement CreateLiteral(int index, int length)
		{
            TemplateElement lit = new TemplateElement(this.Document, TemplateElementTypes.Literal, index, length, null);

			return lit;
		}

		private TemplateElement CreateTag(Match match)
		{
			Group param = match.Groups["param"];
			string name = match.Groups["name"].Value;

			bool closed = false;

			TemplateElementTypes type = TemplateElementTypes.Tag;

			name = name.ToLower();

			switch(name)
			{
				case "if":
					type = TemplateElementTypes.IfExpression;
					break;

				case "else":
					if (SourceTemplate.IndexOf("if ", param.Index, param.Length, StringComparison.OrdinalIgnoreCase) == param.Index)
						type = TemplateElementTypes.ElseIfExpression;
					else
						type = TemplateElementTypes.ElseExpression;
					break;

				case "load":
					type = TemplateElementTypes.LoadExpression;
					break;

                //case "pre-include":
                //    type = TemplateElementTypes.PreIncludeExpression;
                //    break;

				case "loop":
					type = TemplateElementTypes.LoopExpression;
					break;

				case "ajaxpanel":
					type = TemplateElementTypes.AjaxPanel;
					break;
			}

            closed = match.Groups["close"].Success
                || type == TemplateElementTypes.ElseExpression
                || type == TemplateElementTypes.ElseIfExpression
                || type == TemplateElementTypes.LoadExpression;
				//|| type == TemplateElementTypes.PreIncludeExpression;

			Hashtable items = new Hashtable(2);

			items.Add(KEY_NAME, name);
			items.Add(KEY_CLOSED, closed);

			int index = match.Index + match.Length;
			int contentLength = closed ? match.Length : SourceTemplate.Length - index;

			if (closed)
				items.Add("END", index);

            TemplateElement result = new TemplateElement(this.Document, type, index, contentLength, items);

			if (type == TemplateElementTypes.IfExpression || type == TemplateElementTypes.ElseIfExpression)
				result.ChildNodes.Insert(0, CreateConditionExpression(param, type == TemplateElementTypes.ElseIfExpression));
			else if (type == TemplateElementTypes.LoopExpression)
				result.ChildNodes.Insert(0, CreateLoopExpressionParam(param));
			else
				result.ChildNodes.Insert(0, CreateAttributeList(param));

			return result;
		}

		private TemplateElement CreateConditionExpression(Group param, bool isElseIf)
		{
			int index = param.Index;
			int length = param.Length;

			if (isElseIf)
			{
				index += 3;
				length -= 3;
			}

            TemplateElement result = new TemplateElement(this.Document, TemplateElementTypes.ConditionExpression, index, length, null);

			result.Document = this.Document;

			return result;
		}

		private TemplateElement CreateLoopExpressionParam(Group param)
		{
			int index = param.Index;
			int length = param.Length;

            TemplateElement result = new TemplateElement(this.Document, TemplateElementTypes.LoopExpressionParam, index, length, null);

			result.Document = this.Document;

			return result;
		}

		private TemplateElement CreateCodeBlock(Match match)
		{
			Group content = match.Groups["content"];

            TemplateElement result = new TemplateElement(this.Document, TemplateElementTypes.CodeBlock, content.Index, content.Length, null);

			result.Items.Add(KEY_OUTPUT, match.Groups["out"].Success);

			result.Document = this.Document;

			return result;
		}

		private TemplateElement CreateCodeSnippet(Match match)
		{
			Group memberInvoke = match.Groups["invoke"];
			Group funcParam = match.Groups["func_param"];

			string name = match.Groups["name"].Value;

			TemplateElement result = null;

			if (funcParam.Success)
			{
                result = new TemplateElement(this.Document, TemplateElementTypes.Function, match.Index, match.Length, null);

                TemplateElement funcParamNode = new TemplateElement(this.Document, TemplateElementTypes.FunctionInvokeParam, funcParam.Index, funcParam.Length, null);

				result.ChildNodes.Add(funcParamNode);
			}
			else
			{
                result = new TemplateElement(this.Document, TemplateElementTypes.Variable, match.Index, match.Length, null);
			}

			result.Items[KEY_NAME] = name;

			if (memberInvoke.Success && memberInvoke.Length > 0)
				result.ParseMemberInvoke(memberInvoke.Index, memberInvoke.Length);

			result.Document = this.Document;

			return result;
		}

		private TemplateElement CreateMemberInvokeListItem(Match match)
		{
			Group memberInvoke = match.Groups["invoke"];
			Group funcParam = match.Groups["func_param"];
			Group indexParam = match.Groups["index_param"];

			TemplateElement result = null;

			if (funcParam.Success)
			{
                result = new TemplateElement(this.Document, TemplateElementTypes.FunctionInvoke, match.Index, match.Length, null);

                TemplateElement funcParamNode = new TemplateElement(this.Document, TemplateElementTypes.FunctionInvokeParam, funcParam.Index, funcParam.Length, null);

				result.ChildNodes.Add(funcParamNode);
			}
			else if (indexParam.Success)
			{
                result = new TemplateElement(this.Document, TemplateElementTypes.IndexInvoke, match.Index, match.Length, null);

                TemplateElement indexParamNode = new TemplateElement(this.Document, TemplateElementTypes.IndexInvokeParam, indexParam.Index, indexParam.Length, null);

				result.ChildNodes.Add(indexParamNode);
			}
			else
			{
                result = new TemplateElement(this.Document, TemplateElementTypes.PropertyInvoke, match.Index, match.Length, null);
			}

			result.Items[KEY_NAME] = match.Groups["name"].Value;

			if (memberInvoke.Success && memberInvoke.Length > 0)
				result.ParseMemberInvoke(memberInvoke.Index, memberInvoke.Length);

			return result;
		}

		private TemplateElement CreateAttributeList(Capture capture)
		{
            TemplateElement result = new TemplateElement(this.Document, TemplateElementTypes.AttributeList, capture.Index, capture.Length, null);

			return result;
		}

		private TemplateElement CreateAttributeListItem(int index, Match match)
		{
			Group name = match.Groups["name"];
			Group value = match.Groups["value"];

            TemplateElement result = new TemplateElement(this.Document, TemplateElementTypes.AttributeListItem, value.Index, value.Length, null);

			result.Items[KEY_NAME] = name.Value;

			return result;
		}

		private TemplateElement CreateSingleQuteString(Match match)
		{
            TemplateElement result = new TemplateElement(this.Document, TemplateElementTypes.SingleQuteString, match.Index + 1, match.Length - 2, null);

			return result;
		}

		private TemplateElement CreateDoubleQuteString(Match match)
		{
            TemplateElement result = new TemplateElement(this.Document, TemplateElementTypes.DoubleQuteString, match.Index + 1, match.Length - 2, null);

			return result;
		}

		#endregion

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("Type:");
			sb.Append(this.Type);

			sb.Append(" ,Name:");
			sb.Append(Items[KEY_NAME]);

			sb.Append(" ,IsOutput:");
			sb.Append(Items[KEY_OUTPUT]);

			sb.Append(" ,Name:");
			sb.Append(Items[KEY_NAME]);

			sb.Append(" ,Text:");
			sb.Append(Text);

			sb.Replace("\\","\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t","\\t");

			return sb.ToString();
		}
	}

	enum TemplateElementTypes
	{
		Document

		, Literal

		, Tag
		, CodeBlock
		
		, Variable
		, Function

		, IndexInvoke
		, PropertyInvoke
		, FunctionInvoke

		, IndexInvokeParam
		, FunctionInvokeParam

		, AttributeList
		, AttributeListItem

		, IfExpression
		, ElseExpression
		, ElseIfExpression
		, ConditionExpression

		, LoadExpression
		//, PreIncludeExpression

		, DoubleQuteString
		, SingleQuteString

		, LoopExpression
		, LoopExpressionParam
		, LoopExpressionParam2

		, AjaxPanel
	}
}