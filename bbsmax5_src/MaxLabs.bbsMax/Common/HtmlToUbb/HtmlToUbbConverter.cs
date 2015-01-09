//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Ubb.HtmlTags;
using System.Text.RegularExpressions;
using System.Web;
/*
 * 创建者:  温泉
 * 创建时间: 2009-10-25 16:12
 * 版权归属: MaxLab.
*/
namespace MaxLabs.bbsMax.Ubb
{
    public delegate string ContentConvert(string content);

    public class HtmlToUbbConverter
    {
        private static Regex regTag = new Regex("</?(\\w+).*?/?>", RegexOptions.Compiled);
        private static Regex regScript = new Regex("<script.*?>.*?</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase);//清除脚本
        private static Regex regStyle = new Regex("<style.*?>.*?</style>", RegexOptions.Compiled | RegexOptions.IgnoreCase);   //清除样式

        public static string ConvertToUbb(string htmlContent)
        {
            return ConvertToUbb(htmlContent, null, null);
        }
        public static string ConvertToUbb(string htmlContent, ContentConvert beforeConvert, ContentConvert afterConvert)
        {
            string msg = string.Empty;
            htmlContent = htmlContent.Replace("\n", string.Empty);
            htmlContent = htmlContent.Replace("\r", string.Empty);
            htmlContent = regScript.Replace(htmlContent, string.Empty);
            htmlContent = regStyle.Replace(htmlContent, string.Empty);
           // htmlContent = htmlContent.Trim();

            if (beforeConvert != null)
                htmlContent = beforeConvert(htmlContent);

            MatchCollection matchs = regTag.Matches(htmlContent);
            int tagIndex = 0;

            List<HtmlTagBase> tags = new List<HtmlTagBase>()
                , beginTags = new List<HtmlTagBase>()
                , endTages = new List<HtmlTagBase>()
                , singleTags = new List<HtmlTagBase>();

            foreach (Match m in matchs)
            {
                HtmlTagBase t;
                string tagName = m.Groups[1].Value;

                //这里也可以根据HTML标签来反射生成Html标签类， 代码可以少很多， 但是这样性能不好。
                switch (tagName.ToLower())
                {
                    case "img":
                        t = new ImgTag(tagIndex, m.Value, m.Index);
                        break;
                    case "a":
                        t = new ATag(tagIndex, m.Value, m.Index);
                        break;
                    case "font":
                        t = new FontTag(tagIndex, m.Value, m.Index);
                        break;
                    case "span":
                        t = new SpanTag(tagIndex, m.Value, m.Index);
                        break;
                    case "br":
                        t = new BrTag(tagIndex, m.Value, m.Index);
                        break;
                    case "div":
                        t = new DivTag(tagIndex, m.Value, m.Index);
                        break;
                    case "p":
                        t = new PTag(tagIndex, m.Value, m.Index);
                        break;
                    case "table":
                        t = new TableTag(tagIndex, m.Value, m.Index);
                        break;
                    case "ol":
                    case "ul":
                        t = new ListTag(tagIndex, m.Value, m.Index);
                        break;
                    case "td":
                    case "tr":
                    case "s":
                    case "b":
                    case "i":
                    case "sub":
                    case "sup":
                    case "u":
                        t = new PairTag(tagIndex, m.Value, m.Index);
                        break;
                    case "th":
                        t = new PairTag(tagIndex, m.Value, m.Index);
                        ((PairTag)t).UbbTag = "td";
                        break;
                    case "strong":
                        t = new PairTag(tagIndex, m.Value, m.Index);
                        ((PairTag)t).UbbTag = "b";
                        break;
                    case "em":
                        t = new PairTag(tagIndex, m.Value, m.Index);
                        ((PairTag)t).UbbTag = "i";
                        break;
                    case "strike":
                        t = new PairTag(tagIndex, m.Value, m.Index);
                        ((PairTag)t).UbbTag = "s";
                        break;
                    case "blockquote":
                        t = new PairTag(tagIndex, m.Value, m.Index);
                        ((PairTag)t).UbbTag = "indent";
                        break;
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                        t = new HTag(tagIndex, m.Value, m.Index);
                        break;
                    case "embed":
                        t = new EmbedTag(tagIndex, m.Value, m.Index);
                        break;
                    default:
                        t = new HtmlTagBase(tagIndex, m.Value, m.Index);
                        break;
                }
                tagIndex++;

                t.TagList = tags;
                tags.Add(t);

                if (t.IsSingleTag)
                {
                    singleTags.Add(t);
                }
                else
                {
                    if (!t.IsEndTag)
                    {
                        beginTags.Add(t);
                    }
                    else
                    {
                        int flag = -1;
                        for (int i = beginTags.Count - 1; i >= 0; i--)
                        {
                            if (beginTags[i].TagName == t.TagName)//匹配开始标记和结束标记
                            {
                                flag = i;
                                beginTags[i].Pair = t;
                                t.Pair = beginTags[i];
                                break;
                            }
                        }

                        if (flag >= 0)
                        {
                            for (int i = beginTags.Count - 1; i >= flag; i--)
                            {
                                beginTags.RemoveAt(i);
                            }
                        }
                    }
                }
            }

            StringBuilder builder = new StringBuilder(htmlContent);

            for (int i = 0; i < tags.Count; i++)
            {
                if (!tags[i].IsSingleTag)
                {
                    if (tags[i].Pair == null)
                    {
                        msg += tags[i].TagName + "没有对应的" + (tags[i].IsEndTag ? "开始" : "结束") + "标记\r\n";
                    }
                }
                if (!tags[i].IsEndTag || tags[i].IsSingleTag)
                    builder = tags[i].ConvertToUBB(builder);
            }

            //builder.Replace(" ", string.Empty);
            builder.Replace("\t", string.Empty);

            htmlContent = builder.ToString();
            htmlContent = new Regex("<.*?>").Replace(htmlContent, string.Empty);
            htmlContent = HttpUtility.HtmlDecode(htmlContent);


            if (afterConvert != null)
                htmlContent = afterConvert(htmlContent);

            return htmlContent;
        }
    }
}