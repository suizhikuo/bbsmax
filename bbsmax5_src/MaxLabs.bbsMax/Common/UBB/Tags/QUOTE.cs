//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * ������: ���
 * ����ʱ��: 2008-5-26 10:34
 * ��Ȩ����: MaxLab.
 */

using System;
using System.Text;

namespace MaxLabs.bbsMax.Ubb
{
	/// <summary>
	/// [quote]hello world[/quote]
	/// </summary>
	public class QUOTE : UbbTagHandler
	{
		public override string UbbTagName {
			get { return "quote"; }
		}
		
		public override string HtmlTagName {
			get { return "div"; }
		}

        public override UbbTagHandler GetInstance
        {
            get { return this; }
        }

        public override string BuildHtml(UbbParser parser, string param, string content)
        {
            content = parser.DoUbbToHtml(content, CleanFooter);

            while(StringUtil.StartsWithIgnoreCase(content, "<br />"))
                content = content.Remove(0, 6);
            while (StringUtil.EndsWithIgnoreCase(content, "<br />"))
                content = content.Remove(content.Length-6);

            return string.Concat(
                @"<div class=""maxcode-quote"">"
                , content
                , "</div>"
                );
        }


	}
}