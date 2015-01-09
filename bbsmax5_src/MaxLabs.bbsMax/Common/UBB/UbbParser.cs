//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * 创建者: 达达
 * 创建时间: 2008-3-21 16:12
 * 版权归属: MaxLab.
 */

using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Ubb;

namespace MaxLabs.bbsMax.Ubb {
	/// <summary>
	/// UBB解析器
	/// </summary>
	public class UbbParser
	{
		#region 常量
		private const string GN_TAG = "tag";
		private const string GN_TAG_BEGIN = "begin";
		private const string GN_TAG_END = "end";
		private const string GN_TAG_SINGLE = "single";
		private const string GN_PARAM = "param";
		private const string GN_CONTENT = "content";
		private const string REGEX_PARSE_UBB = "(?<begin>\\[(?<tag>[\\w\\*]+)(?>\\x20*=\\x20*(?>(?>\"(?<param>[^\"\\\\\r\n]*(?:\\\\.[^\"\\\\\r\n]*)*)\")|(?>'(?<param>[^'\\\\\r\n]*(?:\\\\.[^'\\\\\r\n]*)*)')|(?<param>[^\\]]+))){0,1}\\])(?<content>(?>\\[\\k<tag>(?>\\x20*=\\x20*(?>(?>\"[^\"\\\\\r\n]*(?:\\\\.[^\"\\\\\r\n]*)*\")|(?>'[^'\\\\\r\n]*(?:\\\\.[^'\\\\\r\n]*)*')|[^\\]]+)){0,1}\\](?<n>)|\\[/\\k<tag>\\](?<-n>)|(?!\\[\\k<tag>\\]|\\[/\\k<tag>\\]).)*)(?(n)(?!))(?<end>\\[/\\k<tag>\\])|(?<single>\\[(?<tag>[\\w\\*]+)\\])";
		#region 原正则
//		(?<begin>
//		\[
//		  (?<tag>[\w\*]+)
//		  (?>
//		    \x20*=\x20*
//		    (?>
//		      (?>"(?<param>[^"\\\r\n]*(?:\\.[^"\\\r\n]*)*)")
//		      |
//		      (?>'(?<param>[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*)')
//		      |
//		      (?<param>
//		        [^\]]+
//		      )
//		    )
//		  ){0,1}
//		\]
//		)
//		(?<content>
//		  (?>
//		    \[
//		      \k<tag>
//		      (?>
//		        \x20*=\x20*
//		        (?>
//		          (?>"[^"\\\r\n]*(?:\\.[^"\\\r\n]*)*")
//		          |
//		          (?>'[^'\\\r\n]*(?:\\.[^'\\\r\n]*)*')
//		          |
//		          [^\]]+
//		        )
//		      ){0,1}
//		    \](?<n>)
//		    |
//		    \[/\k<tag>\](?<-n>)
//		    |
//		    (?!
//		      \[\k<tag>\]
//		      |
//		      \[/\k<tag>\]
//		    ).
//		  )*
//		)
//		(?(n)(?!))
//		(?<end>\[/\k<tag>\])
//		|
//		(?<single>\[(?<tag>[\w\*]+)\])
		#endregion
		
		private const string GN_UBB = "ubb";
		private const string GN_BODY = "body";
		//解析UBB解析得到的HTML中的UBB反向解析辅助代码
		private const string REGEX_PARSE_HTML = "<!--ubb-begin--><!--(?<ubb>(?>(?<n><!--)|(?<-n>-->)|(?!<!--|-->).)*(?(n)(?!)))-->(?<body>(?>(?<n1><!--ubb-begin-->)|(?<-n1><!--ubb-end-->)|(?!<!--ubb-begin-->|<!--ubb-end-->).)*(?(n1)(?!)))<!--ubb-end-->";
		#region 原正则
//<!--ubb-begin-->
//<!--
//	(?<ubb>
//		(?>
//			(?<n><!--)
//			|
//			(?<-n>-->)
//			|
//			(?!
//				<!--
//				|
//				-->
//			).
//		)*
//		(?(n)(?!))
//	)
//-->
//(?<body>
//	(?>
//		(?<n1><!--ubb-begin-->)
//		|
//		(?<-n1><!--ubb-end-->)
//		|
//		(?!
//			<!--ubb-begin-->
//			|
//			<!--ubb-end-->
//		).
//	)*
//	(?(n1)(?!))
//)
//<!--ubb-end-->
		#endregion
		
		//解析html的code标签
		private const string REGEX_PARSE_CODE_TAG = "<code>(?<body>(?>(?<n><code>)|(?<-n></code>)|(?!<code>|</code>).)*)</code>";
		#region 原正则
//<code>
//(?<body>
//	(?>
//		(?<n><code>)
//		|
//		(?<-n></code>)
//		|
//		(?!
//			<code>
//			|
//			</code>
//		).
//	)*
//)
//</code>
		#endregion
		
		#endregion
		
		#region 静态成员
		private static readonly Regex s_RegexParseUbb = new Regex(REGEX_PARSE_UBB, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		private static readonly Regex s_RegexParseHtml = new Regex(REGEX_PARSE_HTML, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		private static readonly Regex s_RegexParseCodeTag = new Regex(REGEX_PARSE_CODE_TAG, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex s_RegexParseQuoteTag = new Regex(@"<div\x20class=""maxcode-quote"">
(
(?>
	<div.*?>(?<n>)
	|
	</div>(?<-n>)
	|
	(?!<div.*?>|</div>).
)*
(?(n)(?!))
)
</div>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

		#endregion
		
		public UbbParser()
		{

		}

		public UbbParser(IEnumerable<UbbTagHandler> handlers)
		{
            if (ValidateUtil.HasItems(handlers))
                m_UbbTagHandlers = new Dictionary<string, UbbTagHandler>(StringComparer.OrdinalIgnoreCase);

			foreach(UbbTagHandler handler in handlers)
			{
				m_UbbTagHandlers.Add(handler.UbbTagName, handler);
			}
		}
		
		private bool m_EncodeHtml = true;
		
		/// <summary>
		/// 是否对html进行编码
		/// </summary>
		public bool EncodeHtml
		{
			get { return m_EncodeHtml; }
			set { m_EncodeHtml = value; }
		}

        private Dictionary<string, UbbTagHandler> m_UbbTagHandlers = null;// = new Dictionary<string, UbbTagHandler>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, int> m_UbbTagHanlderRefCount = null;// = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// 添加UBB标签处理器
		/// </summary>
		/// <param name="handler"></param>
		/// <returns></returns>
		public UbbTagHandler AddTagHandler(UbbTagHandler handler)
		{
            if (m_UbbTagHandlers == null)
                m_UbbTagHandlers = new Dictionary<string, UbbTagHandler>(StringComparer.OrdinalIgnoreCase); ;

            if (m_UbbTagHandlers.ContainsKey(handler.UbbTagName))
            {
                if (m_UbbTagHanlderRefCount == null)
                    m_UbbTagHanlderRefCount = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

                if (m_UbbTagHanlderRefCount.ContainsKey(handler.UbbTagName) == false)
                    m_UbbTagHanlderRefCount.Add(handler.UbbTagName, 1);
                else
                    m_UbbTagHanlderRefCount[handler.UbbTagName] = m_UbbTagHanlderRefCount[handler.UbbTagName] + 1;

                return handler;
            }

			m_UbbTagHandlers.Add(handler.UbbTagName, handler);
			return handler;
		}
		
		/// <summary>
		/// 移除UBB标签处理器
		/// </summary>
		/// <param name="handler"></param>
		/// <returns></returns>
        public bool RemoveTagHandler(UbbTagHandler handler)
        {
            if (m_UbbTagHanlderRefCount != null && m_UbbTagHanlderRefCount.ContainsKey(handler.UbbTagName))
            {
                int count = m_UbbTagHanlderRefCount[handler.UbbTagName];

                if (count == 1)
                {
                    m_UbbTagHanlderRefCount.Remove(handler.UbbTagName);
                }
                else
                {
                    m_UbbTagHanlderRefCount[handler.UbbTagName] = count - 1;
                }

                return false;
            }
            else if (m_UbbTagHandlers != null)
            {
                return m_UbbTagHandlers.Remove(handler.UbbTagName);
            }

            return false;
        }
		
		/// <summary>
		/// 将UBB转换为HTML
		/// </summary>
		/// <param name="ubb">UBB文本</param>
		/// <param name="encodeHtml">是否对已存在的html内容进行编码</param>
		/// <returns>HTML文本</returns>
		public string UbbToHtml(string ubb)
		{
			if(m_EncodeHtml)
				ubb = System.Web.HttpUtility.HtmlEncode(ubb);

            if (m_UbbTagHandlers == null || m_UbbTagHandlers.Count == 0)
            {
                if (m_EncodeHtml)
                    ubb = FixHtml(ubb);

                return ubb;
            }
			
			return DoUbbToHtml(ubb,false);
		}

        public string QuoteToUbb(string html)
        {
            return s_RegexParseQuoteTag.Replace(html, delegate(Match match) { 
                
                string content = match.Groups[1].Value;

                return string.Concat("[quote]", QuoteToUbb(content), "[/quote]<br />");
            });
        }
		
		/// <summary>
		/// 将HTML转换回UBB
		/// </summary>
		/// <param name="html">html文本</param>
		/// <returns>ubb文本</returns>
		public string HtmlToUbb(string html)
		{
            //html = Regex.Replace(html, @"<(b|i|s|u)>");


            StringBuilder result = new StringBuilder(html.Length);

            int currentIndex = 0;

            foreach (Match match in s_RegexParseHtml.Matches(html))
            {
                if (match.Index > currentIndex)
                {
                    string header = html.Substring(currentIndex, match.Index - currentIndex);
                    result.Append(header);
                }

                currentIndex = match.Index + match.Length;

                string ubb = match.Groups[GN_UBB].Value;

                if (StringUtil.StartsWithIgnoreCase(ubb, "[code]"))
                {
                    string body = match.Groups[GN_BODY].Value;

                    Match matchCodeTag = s_RegexParseCodeTag.Match(body);

                    if (matchCodeTag.Success)
                        result.AppendFormat(ubb, matchCodeTag.Groups[GN_BODY].Value);
                }
                else
                {
                    result.Append(ubb);
                }
            }

            if (currentIndex < html.Length)
            {
                string footer = html.Substring(currentIndex);
                result.Append(footer);
            }
			
			return result.ToString();
		}
		
		/// <summary>
		/// 将UBB转换为HTML(处理器使用)
		/// </summary>
		/// <param name="ubb">UBB文本</param>
		/// <param name="cleanFooter">是否清除尾部的多余文本</param>
		/// <returns>HTML文本</returns>
		public string DoUbbToHtml(string ubb, bool cleanFooter)
		{
			StringBuilder result = new StringBuilder(ubb.Length);
			
			int currentIndex = 0;
			
			foreach(Match match in s_RegexParseUbb.Matches(ubb))
			{
				string tagName = match.Groups[GN_TAG].Value;
				string tagParam = match.Groups[GN_PARAM].Value;
				string tagContent = match.Groups[GN_CONTENT].Value;
				
				UbbTagHandler handler = LookupUbbTagHandler(tagName);
				
				#region 头部文本
				if(handler == null || (handler.CleanHeader == false && match.Index > currentIndex)) 
				{
					string header = ubb.Substring(currentIndex, match.Index - currentIndex);
					
					if(m_EncodeHtml)
						result.Append(FixHtml(header));
					else
						result.Append(header);
				}
				
				currentIndex = match.Index + match.Length;
				#endregion
				
				#region UBB文本
				if (handler != null && handler.IsSingleUbbTag == match.Groups[GN_TAG_SINGLE].Success && handler.ParseTimes < handler.ParseTimesLimit)
				{
					handler.ParseTimes++;

					if (handler.SaveUbbText)
					{
						if (tagParam == null || string.IsNullOrEmpty(tagParam.Trim()))
						{
							result.AppendFormat(
								"<!--ubb-begin--><!--[{0}]{1}[/{0}]-->{2}<!--ubb-end-->"
								, tagName
								, tagContent
								, handler.BuildHtml(this, tagParam, tagContent)
							);
						}
						else
						{
							result.AppendFormat(
								"<!--ubb-begin--><!--[{0}={1}]{2}[/{0}]-->{3}<!--ubb-end-->"
								, tagName
								, tagParam
								, tagContent
								, handler.BuildHtml(this, tagParam, tagContent)
							);
						}
					}
					else
					{
						string body = handler.BuildHtml(this, tagParam, tagContent);
						result.Append(body);
					}
				}
				else if (match.Groups[GN_TAG_BEGIN].Success) 				//没有相应的标签处理器时
				{
					result.Append(match.Groups[GN_TAG_BEGIN].Value);  		//原样输出起始标签
					result.Append(DoUbbToHtml(tagContent, false));			//继续解析内容
					result.Append(match.Groups[GN_TAG_END].Value);  		//原样输出结束标签
				}
				else
				{
					if (m_EncodeHtml)
						result.Append(FixHtml(match.Value));				//单标签，原样输出
					else
						result.Append(match.Value);
				}
				#endregion
			}
			
			if(cleanFooter == false &&currentIndex < ubb.Length)
			{
				string footer = ubb.Substring(currentIndex);
				
				if(m_EncodeHtml)
					result.Append(FixHtml(footer));
				else
					result.Append(footer);
			}
			
			return result.ToString();
		}
		
		/// <summary>
		/// 获取HtmlTextWriter实例
		/// </summary>
		/// <returns>HtmlTextWriter实例</returns>
		internal HtmlTextWriter GetHtmlTextWriter()
		{
			return new HtmlTextWriter(new StringWriter(new StringBuilder()));
		}
		
		/// <summary>
		/// 寻找对应的标签处理器
		/// </summary>
		/// <param name="tagName">ubb标签名</param>
		/// <returns>标签处理器或null</returns>
		private UbbTagHandler LookupUbbTagHandler(string tagName)
		{
            if (m_UbbTagHandlers == null)
                return null;

            UbbTagHandler handler;

            if (m_UbbTagHandlers.TryGetValue(tagName, out handler))
                return handler;
			
			return null;
		}
		
		/// <summary>
		/// 将换行空格等特殊符号替换为html标签
		/// </summary>
		/// <param name="text">文本内容</param>
		/// <returns>html结果</returns>
		private static string FixHtml(string text)
		{
			StringBuilder result = new StringBuilder(text);

            result.Replace("  ", " &nbsp;");
			result.Replace("\t", "&nbsp; &nbsp;&nbsp;");
			result.Replace("\r\n", "\n");
			result.Replace("\n", "<br />");
			
			return result.ToString();
		}
	}
}