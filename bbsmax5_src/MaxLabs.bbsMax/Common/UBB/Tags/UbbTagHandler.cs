//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * 创建者: 达达
 * 创建时间: 2008-3-20 15:27
 * 版权归属: MaxLab.
 */

using System;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Ubb {
	/// <summary>
	/// UBB标签处理器
	/// </summary>
	public abstract class UbbTagHandler {

        protected const string REGEX_FILE = @"#file:([a-zA-Z0-9]+)#";
        protected const string REGEX_ATTACHMENT = @"#attach:([0-9]+)#";

		public UbbTagHandler()
		{
			ParseTimesLimit = 0;
		}

		public UbbTagHandler(int parseTimesLimit)
		{
			ParseTimesLimit = parseTimesLimit;
		}

		/// <summary>
		/// UBB标签名
		/// </summary>
		public abstract string UbbTagName { get; }
		
		/// <summary>
		/// HTML标签名
		/// </summary>
		public abstract string HtmlTagName { get; }
		
		/// <summary>
		/// 是否是单标签(默认返回false)
		/// </summary>
		public virtual bool IsSingleHtmlTag {
			get { return false; }
		}

		/// <summary>
		/// 是否是单UBB标签（默认返回false)
		/// </summary>
		public virtual bool IsSingleUbbTag {
			get { return false; }
		}
		
		/// <summary>
		/// 获取实例 (默认返回当前实例)
		/// </summary>
		public virtual UbbTagHandler GetInstance { 
			get { return this; }
		}
		
		/// <summary>
		/// 是否自动清除头部多余文本（默认返回false）
		/// </summary>
		public virtual bool CleanHeader {
			get {return false;}
		}
		
		/// <summary>
		/// 是否自动清除尾部多余文本（默认返回false）
		/// </summary>
		public virtual bool CleanFooter {
			get {return false;}
		}
		
		/// <summary>
		/// 是否在html中保留ubb原文本，如果允许，UbbParser将为其生成一段html注释(默认返回false)
		/// </summary>
		public virtual bool SaveUbbText {
			get {return false;}
		}

		private int m_ParseTimesLimit;

		/// <summary>
		/// 解析次数闲置，0为不限制
		/// </summary>
		public virtual int ParseTimesLimit
		{
			get { return m_ParseTimesLimit; }
			set {
				if (value == 0)
					m_ParseTimesLimit = int.MaxValue;
				else
					m_ParseTimesLimit = value; 
			}
		}

		private int m_ParseTimes;

		/// <summary>
		/// 解析次数计数
		/// </summary>
		public int ParseTimes
		{
			get { return m_ParseTimes; }
			set { m_ParseTimes = value; }
		}
		
		/// <summary>
		/// 创建HTML
		/// </summary>
		/// <param name="parser">解析器</param>
		/// <param name="param">UBB参数</param>
		/// <param name="content">UBB内容</param>
		/// <returns>HTML结果</returns>
        public virtual string BuildHtml(UbbParser parser, string param, string content)
		{
			HtmlTextWriter writer = parser.GetHtmlTextWriter();

			writer.WriteBeginTag(this.HtmlTagName);

			BuildAttribute(writer, param, content);

			if (IsSingleHtmlTag)
				writer.Write(HtmlTextWriter.SelfClosingTagEnd);
			else
			{
				writer.Write(HtmlTextWriter.TagRightChar);

                writer.Write(parser.DoUbbToHtml(content, CleanFooter));
				writer.WriteEndTag(this.HtmlTagName);
			}

			return writer.InnerWriter.ToString();
		}
		
		/// <summary>
		/// 创建HTML标签属性
		/// </summary>
		/// <param name="writer">HtmlTextWriter</param>
		/// <param name="param">UBB参数</param>
		/// <param name="content">UBB内容</param>
		protected virtual void BuildAttribute(HtmlTextWriter writer, string param, string content) {}
	}
	
	/// <summary>
	/// 基于文件的类似mp3,wma等标签的基类
	/// </summary>
	public abstract class FileTagHandler : UbbTagHandler
	{
		protected const string REGEX_CHECK_URL = "^[\\w]+://";

        public FileTagHandler()
        { }

		/// <summary>
		/// 是否是单标签（true）
		/// </summary>
		public override bool IsSingleHtmlTag {
			get { return true; }
		}
		
		/// <summary>
		/// 获取URL
		/// </summary>
		/// <param name="content">标签内容</param>
		/// <returns>URL</returns>
		protected string GetURL(string content) 
		{
            if (content == null)
                return string.Empty;

            content = content.Trim();

            if (content == string.Empty)
                return string.Empty;

            if (Regex.IsMatch(content, REGEX_FILE, RegexOptions.IgnoreCase))
                return content;

            if (Regex.IsMatch(content, REGEX_ATTACHMENT, RegexOptions.IgnoreCase))
                return content;

			if(StringUtil.StartsWith(content, '/'))
				return content;
			
			if(Regex.IsMatch(content, REGEX_CHECK_URL))
				return content;
			else
				return "http://" + System.Web.HttpUtility.UrlPathEncode(content);
		}

        protected string GetLink(string content, bool allowURL)
        {
            if (allowURL)
                return string.Format(@"<a href=""{0}"" target=""_target"">{0}</a>", content);
            else
                return GetURL(content);
        }
	}
	
	public abstract class MediaTagHandler : FileTagHandler
	{
		protected const string REGEX_PARSE_PARAM = @"(?:(?<width>\d+),(?<height>\d+),(?<auto>\d+))|(?:(?<width>\d+),(?<height>\d+))|(?<auto>\d+)";
		protected const string GN_AUTO = "auto";
		protected const string GN_WIDTH = "width";
		protected const string GN_HEIGHT = "height";
		
		public override string HtmlTagName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool SaveUbbText {
			get { return true; }
		}
		
		protected MediaInfo GetMediaInfo(string param, string content)
		{
			Match match = Regex.Match(param, REGEX_PARSE_PARAM);
			
			MediaInfo result = new MediaInfo();
			result.Width = match.Groups[GN_WIDTH].Value;
			result.Height = match.Groups[GN_HEIGHT].Value;
			result.AutoPlay = match.Groups[GN_AUTO].Value;
			
			result.URL = this.GetURL(content);
			
			return result;
		}

        public abstract string BuildHtml(object width, object height, bool isAuto, string url);
		
		public class MediaInfo
		{
			public string m_URL;
			
			public string URL {
				get { return m_URL; }
				set { m_URL = value; }
			}
			
			public string m_Width;
			
			public string Width {
				get { return m_Width; }
				set { m_Width = value; }
			}
			
			public string m_Height = "450";
			
			public string Height {
				get { return m_Height; }
				set { m_Height = value; }
			}
			
			public string m_AutoPlay = "1";
			
			public string AutoPlay {
				get { return m_AutoPlay; }
				set { m_AutoPlay = value; }
			}
		}
	}
}