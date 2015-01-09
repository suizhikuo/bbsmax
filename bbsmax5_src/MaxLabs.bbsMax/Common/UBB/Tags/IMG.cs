//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * 创建者: 达达
 * 创建时间: 2008-3-22 9:08
 * 版权归属: MaxLab.
 */

using System;
using System.Web.UI;
using System.Text.RegularExpressions;

using MaxLabs.bbsMax.Ubb;

namespace MaxLabs.bbsMax.Ubb
{
    /// <summary>
    /// [IMG]url[/IMG]
    /// [IMG=size]url[/IMG]
    /// [IMG=size%]url[/IMG]
    /// [IMG=width,height]url[/IMG]
    /// [IMG=width%,height%]url[/IMG]
    /// </summary>
    public class IMG : FileTagHandler
    {
        private const string GN_SIZE = "size";
        private const string GN_WIDTH = "width";
        private const string GN_HEIGHT = "height";

        private const string REGEX_PARSE_PARAM = "(?>(?<width>\\d+%*)\\x20*,\\x20*(?<height>\\d+%*))|(?>(?<size>\\d+%*).*)";
        //private const string REGEX_CHECK_URL = "^[\\w]+://";

        private bool m_AllowIMG;
        private bool m_AllowURL;

        public IMG()
        {
            m_AllowIMG = true;
            m_AllowURL = true;
        }

        public IMG(bool allowIMG, bool allowURL)
        {
            m_AllowIMG = allowIMG;
            m_AllowURL = allowURL;
        }

        public override string BuildHtml(UbbParser parser, string param, string content)
        {
            if (m_AllowIMG)
                return base.BuildHtml(parser, param, content);
            else
                return "图片:" + GetLink(content, m_AllowURL);
        }

        /// <summary>
        /// UBB标签名（img）
        /// </summary>
        public override string UbbTagName
        {
            get { return "img"; }
        }

        /// <summary>
        /// HTML标签名（img）
        /// </summary>
        public override string HtmlTagName
        {
            get { return "img"; }
        }

        ///// <summary>
        ///// 是否是单标签（true）
        ///// </summary>
        //public override bool IsSingleHtmlTag {
        //    get { return true; }
        //}

        protected override void BuildAttribute(HtmlTextWriter writer, string param, string content)
        {
            if (content != null && content.Trim() != string.Empty)
            {
                writer.WriteAttribute("src", this.GetURL(content));

                Match match = Regex.Match(param, REGEX_PARSE_PARAM);

                if (match.Groups[GN_WIDTH].Success)
                {
                    string w = match.Groups[GN_WIDTH].Value;

                    string h = match.Groups[GN_HEIGHT].Value;

                    if (!(w == "28" && h == "30"))
                    {
                        writer.WriteAttribute("width", w);
                        writer.WriteAttribute("height", h);
                    }
                }
                else if (match.Groups[GN_SIZE].Success)
                {
                    string w = match.Groups[GN_SIZE].Value;

                    string h = match.Groups[GN_SIZE].Value;

                    if (!(w == "28" && h == "30"))
                    {
                        writer.WriteAttribute("width", w);
                        writer.WriteAttribute("height", h);
                    }
                }

                writer.WriteAttribute("alt", "");
            }
        }

        ///// <summary>
        ///// 获取URL
        ///// </summary>
        ///// <param name="content">标签内容</param>
        ///// <returns>URL</returns>
        //protected string GetURL(string content) 
        //{
        //    if(string.IsNullOrEmpty(content))
        //        return string.Empty;

        //    if (Regex.IsMatch(content,REGEX_FILE,RegexOptions.IgnoreCase))
        //        return content;

        //    if (Regex.IsMatch(content, REGEX_ATTACHMENT, RegexOptions.IgnoreCase))
        //        return content;

        //    if(content.StartsWith("/"))
        //        return content;

        //    if(Regex.IsMatch(content, REGEX_CHECK_URL))
        //        return content;
        //    else
        //        return "http://" + System.Web.HttpUtility.UrlPathEncode(content);
        //}
    }
}