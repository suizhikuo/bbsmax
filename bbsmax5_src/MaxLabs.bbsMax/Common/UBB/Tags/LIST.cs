//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * 创建者: 达达
 * 创建时间: 2008-4-1 15:17
 * 版权归属: MaxLab.
 */

using System;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Ubb {
	/// <summary>
	/// [LIST]
	///     [*]the first item
	///     [*]the second item
	///     [*]the third item
	/// [/LIST]
	/// </summary>
	public class LIST : UbbTagHandler 
	{
		
		public override string UbbTagName {
			get { return "list"; }
		}
		
		private string m_HtmlTagName;
		
		public override string HtmlTagName {
			get { return m_HtmlTagName; }
		}

        public override string BuildHtml(UbbParser parser, string param, string content) 
		{
			if(string.IsNullOrEmpty(param))
				m_HtmlTagName = "ul";
			else
				m_HtmlTagName = "ol";
			
			HtmlTextWriter writer = parser.GetHtmlTextWriter();
			
			writer.WriteBeginTag(this.HtmlTagName);
			
			//TODO:重载
			BuildAttribute(writer, param, content);
			
			writer.Write(HtmlTextWriter.TagRightChar);
			
			content = content.TrimStart();
			
			if(StringUtil.StartsWith(content, "[*]") == false)
			{
				int tmp = content.IndexOf("[*]");
				
				if(tmp > 0)
					content = content.Substring(tmp);
			}

            string parsedContent = parser.DoUbbToHtml(content, false);
			int itemCount = ParseListItem(parsedContent, out parsedContent);
			
			writer.Write(parsedContent);
			
			if(itemCount > 0)
				writer.WriteEndTag("li");
			
			writer.WriteEndTag(this.HtmlTagName);
			
			return writer.InnerWriter.ToString();
		}
		
		protected override void BuildAttribute(HtmlTextWriter writer, string param, string content)
		{
			if(string.IsNullOrEmpty(param))
				return;
			
			//TODO:严格匹配 1, A, a, I, i
			writer.WriteAttribute("type", param);
		}
		
		private int ParseListItem(string content, out string result) {
			int itemCounter = 0;
			
			result = Regex.Replace(content, "\\[\\*\\]", delegate(Match match) {
				itemCounter ++;

				if(itemCounter == 1)
					return "<li>";

				return "</li><li>";
			});
			
			return itemCounter;
		}
	}
}