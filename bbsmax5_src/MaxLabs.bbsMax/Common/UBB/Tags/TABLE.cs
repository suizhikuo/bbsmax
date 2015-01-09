//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * 创建者: 达达
 * 创建时间: 2008-3-31 15:16
 * 版权归属: MaxLab.
 */

using System;
using System.Text.RegularExpressions;
namespace MaxLabs.bbsMax.Ubb
{
    /// <summary>
    /// [TABLE]
    ///     [TR]
    ///         [TD]Column1[/TD]
    ///         [TD]Column2[/TD]
    ///     [/TR]
    ///     [TR]
    ///         [TD]1[/TD]
    ///         [TD]2[/TD]
    ///     [/TR]
    /// [/TABLE]
    /// </summary>
    public class TABLE : UbbTagHandler
    {
        public override string UbbTagName
        {
            get { return "table"; }
        }

        public override string HtmlTagName
        {
            get { return "table"; }
        }

        public override bool CleanFooter
        {
            get { return true; }
        }

        public override string BuildHtml(UbbParser parser, string param, string content)
        {
            UbbTagHandler handler = parser.AddTagHandler(new TR());

            string result = base.BuildHtml(parser, param, content);

            parser.RemoveTagHandler(handler);

            return result;
        }

        protected class TR : UbbTagHandler
        {
            public override string UbbTagName
            {
                get { return "tr"; }
            }

            public override string HtmlTagName
            {
                get { return "tr"; }
            }

            public override bool CleanHeader
            {
                get { return true; }
            }

            public override bool CleanFooter
            {
                get { return true; }
            }

            public override string BuildHtml(UbbParser parser, string param, string content)
            {
                UbbTagHandler handler = parser.AddTagHandler(new TD());

                string result = base.BuildHtml(parser, param, content);

                parser.RemoveTagHandler(handler);

                return result;
            }

            protected class TD : UbbTagHandler
            {
                private static Regex regTdAttr = new Regex(@"^\d+%?(?>,\d+,\d+%?)?$", RegexOptions.Compiled);

                protected override void BuildAttribute(System.Web.UI.HtmlTextWriter writer, string param, string content)
                {
                    param=(param+"").Trim();
                    if(param=="")
                        return ;

                    if(regTdAttr.IsMatch(param))
                    {
                        string[] attr = param.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        if(attr.Length==1)
                        {
                            writer.WriteAttribute("width",attr[0]);
                        }
                        if(attr.Length==3)
                        {
                            writer.WriteAttribute("colspan",attr[0]);
                            writer.WriteAttribute("rowspan",attr[1]);
                            writer.WriteAttribute("width",attr[2]);
                        }
                    }                  
                }

                public override string UbbTagName
                {
                    get { return "td"; }
                }

                public override string HtmlTagName
                {
                    get { return "td"; }
                }

                public override bool CleanHeader
                {
                    get { return true; }
                }
            }
        }

    }

}