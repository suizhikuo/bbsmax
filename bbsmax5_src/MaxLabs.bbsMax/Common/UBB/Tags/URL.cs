//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * 创建者: 达达
 * 创建时间: 2008-3-19 15:35
 * 版权归属: MaxLab.
 */

using System;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Ubb {
	/// <summary>
	/// [URL]address[/URL]
	/// [URL=address]content[/URL]
	/// </summary>
	public class URL : UbbTagHandler{
		
		private const string REGEX_CHECK_URL1 = "^[\\w]+://|^/";  				//检查是否以网络协议格式开头
		private const string REGEX_CHECK_URL2 = "[\\w\\d-]+\\.[\\w\\d-]+|^/";  	//检查是否是最基本域名格式

        private bool m_AllowURL;

        public URL()
        {
            m_AllowURL = true;
        }

        public URL(bool allowURL)
        {
            m_AllowURL = allowURL;
        }

        public override string BuildHtml(UbbParser parser, string param, string content)
        {
            if (m_AllowURL)
                return base.BuildHtml(parser, param, content);
            else
            {
                if (string.IsNullOrEmpty(param))
                    return content;
                else if (StringUtil.EqualsIgnoreCase(param, content))
                    return string.Concat("链接:", param);
                else
                    return string.Concat("链接:", param, " (", content, ")");
            }
        }

		/// <summary>
		/// UBB标签名（url）
		/// </summary>
		public override string UbbTagName {
			get { return "url"; }
		}
		
		/// <summary>
		/// HTML标签名（a）
		/// </summary>
		public override string HtmlTagName {
			get { return "a"; }
		}
		
		protected override void BuildAttribute(HtmlTextWriter writer, string param, string content) {
			writer.WriteAttribute("href", this.GetURL(param, content));
			writer.WriteAttribute("target", "_blank");
		}
		

		/// <summary>
		/// 获取URL
		/// </summary>
		/// <param name="param">参数部分</param>
		/// <param name="content">标签内容</param>
		/// <returns>URL</returns>
        protected string GetURL(string param, string content) {
			if(string.IsNullOrEmpty(param)) {

                if (Regex.IsMatch(content, REGEX_FILE, RegexOptions.IgnoreCase))
                    return content;

                else if (Regex.IsMatch(content, REGEX_ATTACHMENT, RegexOptions.IgnoreCase))
                    return content;

				else if(Regex.IsMatch(content, REGEX_CHECK_URL1))
					return content;
				else if(Regex.IsMatch(content, REGEX_CHECK_URL2))
					return "http://" + System.Web.HttpUtility.UrlEncode(content);
				else
					return "#";
			} else {
                if (Regex.IsMatch(param, REGEX_FILE, RegexOptions.IgnoreCase))
                    return param;

                else if (Regex.IsMatch(param, REGEX_ATTACHMENT, RegexOptions.IgnoreCase))
                    return param;
				else if(Regex.IsMatch(param, REGEX_CHECK_URL1))
					return param;
				else
					return "http://" + System.Web.HttpUtility.UrlPathEncode(param);
			}
		}
	}
}