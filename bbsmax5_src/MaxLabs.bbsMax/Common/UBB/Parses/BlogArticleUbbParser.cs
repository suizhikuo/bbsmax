//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Ubb
{
	public class BlogArticleUbbParser : UbbParser
	{
		public BlogArticleUbbParser(bool useHteml, bool useUbb)
		{
			if (useHteml == false && useUbb)
			{
				AddTagHandler(new B());
				AddTagHandler(new I());
				AddTagHandler(new S());
				AddTagHandler(new U());
				AddTagHandler(new ALIGN());
				AddTagHandler(new FONT());
				AddTagHandler(new SIZE());
				AddTagHandler(new COLOR());
				AddTagHandler(new BGCOLOR());
				AddTagHandler(new EMAIL());
				AddTagHandler(new URL());
				AddTagHandler(new SUB());
				AddTagHandler(new SUP());
				AddTagHandler(new INDENT());
				AddTagHandler(new LIST());
				AddTagHandler(new QUOTE());
				AddTagHandler(new BR());
				AddTagHandler(new CODE());
				AddTagHandler(new TABLE());
				AddTagHandler(new IMG());
				AddTagHandler(new FLASH());
				AddTagHandler(new WMA());
				AddTagHandler(new MP3());
				AddTagHandler(new RA());
				AddTagHandler(new WMV());
				AddTagHandler(new RM());
			}

			//使用HTML则不编码
			EncodeHtml = !useHteml;
		}

		public static string ParseForSave(int postUserID, string content, bool allowHtml, bool allowUbb)
		{
			BlogArticleUbbParser parser = new BlogArticleUbbParser(allowHtml, allowUbb);
            content = parser.UbbToHtml(content);
            content = EmoticonParser.ParseToHtml(postUserID, content, true, true);

            return content;
		}

		public static string ParseForEdit(int postUserID, string content, bool allowHtml, bool allowUbb)
		{
            if (allowHtml == false)
            {
                if (allowUbb)
                {
                    content = HtmlToUbbParser.Html2Ubb(postUserID, content);
                }
                else
                {
                    content = StringUtil.ClearAngleBracket(content);
                }
            }

			//编辑时 为使[code]里的内容能正确显示  再编码一次[code]里的内容
			content = PostUbbParserV5.regex_code.Replace(content, delegate(Match m)
			{
				return string.Concat("[code]", m.Groups[2].Value, "[/code]");
			});

          //  content = EmoticonParser.HtmlToShortcut(postUserID, content, !allowHtml);

			content = System.Web.HttpUtility.HtmlEncode(content);

			return content;
		}
	}
}