//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Ubb.HtmlTags
{
    public class HtmlTagBase
    {
        private static Regex regAttrSplite = new Regex("(?<=\")\\s+(?=\\w+=\")"); //切割HTML属性的正则表达式

        public List<HtmlTagBase> TagList { get; set; }
        public HtmlTagBase Pair { get; set; }

        public HtmlTagBase(int index, string tagHtml, int begin)
        {
            Index = index;
            Html = tagHtml;
            bool hasAttribute = false;
            this.Begin = begin;
            this.End = this.Begin + Html.Length;
            if (StringUtil.EndsWith(tagHtml, "/>"))
            {
                IsSingleTag = true;
            }

            if (StringUtil.StartsWith(tagHtml, "</"))
            {
                IsEndTag = true;
            }

            tagHtml = tagHtml.Remove(0, IsEndTag ? 2 : 1);
            tagHtml = tagHtml.Remove(tagHtml.Length - (IsSingleTag ? 2 : 1), IsSingleTag ? 2 : 1);


            if (tagHtml.Contains(" "))
            {
                this.TagName = tagHtml.Substring(0, tagHtml.IndexOf(' '));
                hasAttribute = true;
            }
            else
            {
                this.TagName = tagHtml;
            }
            this.TagName = this.TagName.ToLower();
            if (hasAttribute)
            {
                tagHtml = tagHtml.Remove(0, TagName.Length);
                tagHtml = tagHtml.Trim();
                string[] attributes = regAttrSplite.Split(tagHtml);
                foreach (string s in attributes)
                {
                    if (s.Contains("=") && s.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                    {
                        string[] temp = new string[] { string.Empty, string.Empty };//
                        int sIndex = s.IndexOf('=');
                        temp[0] = s.Substring(0, sIndex);
                        if (s.Length > sIndex)
                            temp[1] = s.Substring(sIndex + 1);
                        //s.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        string key = temp[0].Trim().ToLower();
                        if (!Attributes.ContainsKey(key))
                            Attributes.Add(key, temp[1].Trim('\"', ' '));
                    }
                }

                if (Attributes.ContainsKey("style"))
                {
                    string[] styleSet = (Attributes["style"] + "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string s in styleSet)
                    {
                        string[] styleItem = s.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (styleItem.Length == 2)
                        {
                            string key = styleItem[0].Trim().ToLower();
                            if (!Styles.ContainsKey(key))
                            {
                                Styles.Add(key, styleItem[1].Trim());
                            }
                        }
                    }
                }
            }

            //if (IsSingleTag && Attributes.Count == 0)
            //{
            //    IsEndTag = true;
            //}

            if (!IsSingleTag) IsSingleTag =
                TagName == "img"
                || TagName == "br"
                || TagName == "hr"
                || TagName == "input"
                || TagName == "param"
                || TagName == "meta"
                || TagName == "link";
        }

        public string TagName
        {
            get;
            set;
        }

        public int Begin
        {
            get;
            set;
        }

        public int End
        {
            get;
            set;
        }

        public bool IsEndTag
        {
            get;
            set;
        }

        private Hashtable _attributes = new Hashtable();
        public Hashtable Attributes
        {
            get { return _attributes; }
        }

        public int Index { get; set; }

        public virtual string StartUbb
        {
            get { return Html; }

        }

        private string _endUbb = string.Empty;

        public virtual string EndUbb
        {
            get
            {
                return string.Empty;
            }
        }

        public bool IsSingleTag
        {
            get;
            set;
        }

        public string Html
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Index + ":" + TagName.ToString();
        }

        public virtual StringBuilder ConvertToUBB(StringBuilder content)
        {
            if (!this.IsEndTag && StartUbb != Html)
            {
                if (!IsSingleTag)  //如果非单标签
                {
                    if (this.Pair == null) //结束标签没有正确闭合。 不解析，直接返回
                        return content;
                    else
                        content = this.Pair.ReplaceHtmlToUbb(content, EndUbb);//先从结束标签开始转换， 这样不会使标记位置发生错位
                }

                content = this.ReplaceHtmlToUbb(content, StartUbb);
            }

            return content;
        }

        public StringBuilder ReplaceHtmlToUbb(StringBuilder content, string ubb)
        {
            int l = this.Html.Length;
            content = content.Remove(this.Begin, l);
            content = content.Insert(this.Begin, ubb);

            l = ubb.Length - l;

            MoveTagPosition(this.Index, l);

            if (this.IsEndTag)
            {
                this.Begin = this.Begin + ubb.Length;
                this.Begin = this.End;
            }
            else
            {
                this.End = this.Begin;
            }
            return content;
        }

        /// <summary>
        /// 移动HTML标记的位置
        /// </summary>
        /// <param name="tagIndex"></param>
        /// <param name="length"></param>
        private void MoveTagPosition(int tagIndex, int length)
        {
            foreach (HtmlTagBase t in TagList)
            {
                if (t.Index > this.Index)
                {
                    t.Begin += length;
                    t.End += length;
                }
            }
        }

        private Hashtable _style = new Hashtable();
        public Hashtable Styles
        {
            get { return _style; }
        }
    }

    /// <summary>
    /// 从此类继承的标签会解析样式表
    /// </summary>
    public abstract class HasStyleTag : HtmlTagBase
    {

        Hashtable mapSize = new Hashtable();
        public HasStyleTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin)
        {

            mapSize.Add("xx-small", "1");
            mapSize.Add("small", "3");
            mapSize.Add("medium", "4");
            mapSize.Add("x-small", "2");
            mapSize.Add("large", "5");
            mapSize.Add("x-large", "6");
            mapSize.Add("xx-large", "7");
            mapSize.Add("6pt", "1");
            mapSize.Add("6.5pt", "1");
            mapSize.Add("7pt", "1");
            mapSize.Add("8pt", "1");
            mapSize.Add("9pt", "1");
            mapSize.Add("10pt", "2");
            mapSize.Add("11pt", "2");
            mapSize.Add("12pt", "3");
            mapSize.Add("13pt", "3");
            mapSize.Add("14pt", "4");
            mapSize.Add("15pt", "4");
            mapSize.Add("16pt", "4");
            mapSize.Add("17pt", "5");
            mapSize.Add("18pt", "5");
            mapSize.Add("19pt", "5");
            mapSize.Add("20pt", "6");
            mapSize.Add("21pt", "6");
            mapSize.Add("22pt", "6");
            mapSize.Add("24pt", "6");
            mapSize.Add("25pt", "7");
            mapSize.Add("36pt", "7");

            mapSize.Add("7px", "1");
            mapSize.Add("9px", "1");
            mapSize.Add("10px", "1");
            mapSize.Add("11px", "1");
            mapSize.Add("12px", "1");
            mapSize.Add("13px", "1");
            mapSize.Add("14px", "2");
            mapSize.Add("15px", "2");
            mapSize.Add("16px", "2");
            mapSize.Add("17px", "3");
            mapSize.Add("18px", "3");
            mapSize.Add("19px", "3");
            mapSize.Add("20px", "4");
            mapSize.Add("21px", "4");
            mapSize.Add("22px", "4");
            mapSize.Add("23px", "5");
            mapSize.Add("24px", "5");
            mapSize.Add("25px", "5");
            mapSize.Add("26px", "6");
            mapSize.Add("27px", "6");
            mapSize.Add("28px", "6");
            mapSize.Add("36px", "7");

            process();
        }

        private string _startUbb;
        string _endUbb;

        public override string EndUbb
        {
            get
            {
                return _endUbb;
            }
        }

        public override string StartUbb
        {
            get
            {
                return _startUbb;
            }
        }

        private void process()
        {
            string startUbb = string.Empty, endUbb = string.Empty;
            if (Styles.ContainsKey("color"))
            {
                startUbb += "[color=" + Styles["color"] + "]";
                startUbb = Util.ConvertRGB(startUbb);
                endUbb = "[/color]" + endUbb;
            }

            if (Styles.ContainsKey("font-size"))
            {
                string size = Styles["font-size"].ToString();
                startUbb += "[size=" + (mapSize.ContainsKey(size) ? mapSize[size].ToString() : size) + "]";
                endUbb = "[/size]" + endUbb;
            }

            if (Styles.ContainsKey("font-family"))
            {
                startUbb += "[font=" + Styles["font-family"] + "]";
                endUbb = "[/font]" + endUbb;
            }

            if (Styles.ContainsKey("font-style"))
            {
                string fontStyle = Styles["font-style"] + "";
                if (fontStyle == "italic")
                {
                    startUbb += "[i]";
                    endUbb = "[/i]" + endUbb;
                }
            }

            if (Styles.ContainsKey("font-weight"))
            {
                string fontWeight = Styles["font-weight"] + "";
                if (fontWeight != "normal")
                {
                    startUbb += "[b]";
                    endUbb = "[/b]" + endUbb;
                }
            }

            if (Styles.ContainsKey("text-decoration"))
            {
                string dec = Styles["text-decoration"] + "";
                if (dec == "underline")
                {
                    startUbb += "[u]";
                    endUbb = "[/u]" + endUbb;
                }
                else if (dec == "line-through")
                {
                    startUbb += "[s]";
                    endUbb = "[/s]" + endUbb;
                }
            }

            if (Styles.ContainsKey("text-align"))
            {
                string align = Styles["text-align"].ToString();
                if (align == "left" || align == "right" || align == "center")
                {
                    startUbb += "[align=" + align + "]";
                    endUbb = "[/align]" + endUbb;
                }
            }



            if (Styles.ContainsKey("background-color"))
            {
                startUbb += "[bgcolor=" + Styles["background-color"] + "]";
                startUbb = Util.ConvertRGB(startUbb);
                endUbb = "[/bgcolor]" + endUbb;

            }
            _startUbb = startUbb;
            _endUbb = endUbb;
        }
    }

    /// <summary>
    /// 成对出现的标签比如：tr,td,i,b等，无需解析属性和样式
    /// </summary>
    public class PairTag : HtmlTagBase
    {
        public PairTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }

        private string _ubbtag;
        public virtual string UbbTag
        {
            get
            {
                return string.IsNullOrEmpty(_ubbtag) ? TagName : _ubbtag;
            }
            set
            {
                _ubbtag = value;
            }
        }

        public override string StartUbb
        {
            get
            {
                return "[" + UbbTag + "]";
            }
        }

        public override string EndUbb
        {
            get
            {
                return "[/" + UbbTag + "]";
            }
        }
    }

    public class ImgTag : HtmlTagBase
    {
        public ImgTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin)
        { }

        private static Regex regWidth = new Regex("(?<=width[=:]\"?\\s*)\\d+", RegexOptions.IgnoreCase);
        private static Regex regHeight = new Regex("(?<=height[=:]\"?\\s*)\\d+", RegexOptions.IgnoreCase);

        public override string StartUbb
        {
            get
            {
                int width = 0, height = 0;

                if (regWidth.IsMatch(Html))
                {
                    width = int.Parse(regWidth.Match(Html).Value);
                }

                if (regWidth.IsMatch(Html))
                {
                    height = int.Parse(regHeight.Match(Html).Value);
                }

                string ubbTex = "[img";
                if (width > 0)
                {
                    ubbTex += "=" + width;
                    if (height > 0)
                        ubbTex += "," + height;
                }
                ubbTex += "]" + this.Attributes["src"] + "[/img]";
                return ubbTex;
            }
        }
    }

    public class ATag : HtmlTagBase
    {
        private static Regex emptyHref = new Regex("^#+$");

        public ATag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin)
        {
            if (string.IsNullOrEmpty(Href) || emptyHref.IsMatch(Href.Trim()))
            {
                isEmptyHref = true;
            }
        }

        private bool isEmptyHref = false;

        string _href = "";
        private string Href
        {
            get
            {
                if (string.IsNullOrEmpty(_href))
                {
                    _href = this.Attributes["href"] + "";
                }
                return _href;
            }
        }

        public override string StartUbb
        {
            get
            {
                if (isEmptyHref)
                    return string.Empty;

                return "[url=" + Href + "]";
            }
        }

        //string _endUbb;
        public override string EndUbb
        {
            get
            {
                if (isEmptyHref)
                    return string.Empty;
                return "[/url]";
            }
        }
    }

    public class FontTag : HtmlTagBase
    {
        public FontTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin)
        {

            process();
        }

        private string _startUbb;
        private string _endUbb;

        public override string EndUbb
        {
            get
            {
                return _endUbb;
            }
        }

        public override string StartUbb
        {
            get
            {
                return _startUbb;
            }
        }

        private void process()
        {
            string startUbb = string.Empty, endUbb = string.Empty;
            if (this.Attributes.ContainsKey("color"))
            {
                startUbb += "[color=" + Attributes["color"] + "]";
                startUbb = Util.ConvertRGB(startUbb);
                endUbb = "[/color]" + endUbb;
            }
            if (this.Attributes.ContainsKey("size"))
            {
                startUbb += "[size=" + Attributes["size"] + "]";
                endUbb = "[/size]" + endUbb;
            }

            if (this.Attributes.ContainsKey("face"))
            {
                startUbb += "[font=" + Attributes["face"] + "]";
                endUbb = "[/font]" + endUbb;
            }

            _startUbb = startUbb;
            _endUbb = endUbb;
        }
    }

    public class SpanTag : HasStyleTag
    {
        public SpanTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }
    }

    public class BrTag : HtmlTagBase
    {
        public BrTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }

        public override StringBuilder ConvertToUBB(StringBuilder content)
        {
            return ReplaceHtmlToUbb(content, "\r\n");
        }
    }

    public class DivTag : HasStyleTag
    {
        public DivTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }


        public override string StartUbb
        {
            get
            {
                string startUbb = string.Empty;

                if (this.Attributes.ContainsKey("align"))
                {
                    startUbb = "[align=" + Attributes["align"] + "]" + startUbb;
                }
                return startUbb + base.StartUbb;
            }
        }

        public override string EndUbb
        {
            get
            {
                string endUbb = string.Empty; ;

                if (this.Attributes.ContainsKey("align"))
                {
                    endUbb += "[/align]";
                }
                return base.EndUbb + endUbb;
            }
        }
    }

    public class PTag : DivTag
    {
        public PTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }

        public override string StartUbb
        {
            get
            {
                if (this.Begin == 0) return "    " + base.StartUbb;
                return base.StartUbb;
            }
        }

        public override string EndUbb
        {
            get
            {
                return base.EndUbb + "\r\n";
            }
        }
    }

    public class Center : HtmlTagBase
    {
        public Center(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }

        public override string StartUbb
        {
            get
            {
                return "[align=center]";
            }
        }

        public override string EndUbb
        {
            get
            {
                return "[/align]";
            }
        }
    }

    public class TdTag : HtmlTagBase
    {
        public TdTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }

        public override string EndUbb
        {
            get
            {
                return "[/td]";
            }
        }

        public override string StartUbb
        {
            get
            {
                string ubb = "[td";
                if (
                    (this.Attributes.ContainsKey("colspan") || this.Attributes.ContainsKey("rowspan"))
                    &&
                    ("1".Equals(Attributes["rowspan"]) || "1".Equals(Attributes["colspan"]))
                    )
                {
                    ubb += "=" + (Attributes.ContainsKey("colspan") ? Attributes["colspan"] : "1") + "," + (Attributes.ContainsKey("rowspan") ? Attributes["rowspan"] : "1");
                    if (Attributes.ContainsKey("width"))
                    {
                        ubb += "," + Attributes["width"];
                    }
                }
                else if (Attributes.ContainsKey("width"))
                {
                    ubb += "=" + Attributes["width"];
                }
                return ubb + "]";
            }
        }

    }

    public class TableTag : HtmlTagBase
    {
        public TableTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }


        public override string StartUbb
        {
            get
            {
                string ubbText = "[table";
                //if (this.Attributes.ContainsKey("border"))
                //{
                //    ubbText += "=" + this.Attributes["border"];
                //}
                ubbText += "]";
                return ubbText;
            }
        }

        public override string EndUbb
        {
            get
            {
                return "[/table]";
            }
        }
    }

    public class LiTag : HtmlTagBase
    {
        public LiTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }

        public override string StartUbb
        {
            get
            {
                return "[*]";
            }
        }

        public override string EndUbb
        {
            get
            {
                return string.Empty;
            }
        }
    }

    public class ListTag : HtmlTagBase
    {
        public ListTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }

        public override string StartUbb
        {
            get
            {
                string ubbText = "[list";

                if (this.TagName == "ol")
                    ubbText += "=1";
                else if ("decimal".Equals( this.Styles["list-style-type"]) )
                {
                    ubbText += "=1";
                }
                ubbText += "]";
                return ubbText;
            }
        }

        public override string EndUbb
        {
            get
            {
                return "[/list]";
            }
        }
    }

    public class HTag : HtmlTagBase
    {
        public HTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }


        public override string StartUbb
        {
            get
            {
                int size = int.Parse(TagName.Substring(1));
                size = 7 - size;
                return "[size=" + size + "][b]";
            }
        }

        public override string EndUbb
        {
            get
            {
                return "[/b][/size]";
            }
        }
    }

    public class EmbedTag : HtmlTagBase
    {
        private const string TypeOfFlash = "application/x-shockwave-flash";
        private const string TypeOfWMedia = "application/x-mplayer2";
        private const string TypeOfRealMedia = "audio/x-pn-realaudio-plugin";
        private static Regex fileTypeReg = new Regex(@"(?<=\.)\w+(?=\s*$)");
        public EmbedTag(int index, string tagHtml, int begin)
            : base(index, tagHtml, begin) { }

        private string Type
        {
            get
            {
                return Attributes["type"] + "";
            }
        }

        private int width
        {
            get
            {

                int width = 0;
                if (int.TryParse(Attributes["width"] + "", out width))
                {
                    return width;
                }
                return 550;
            }
        }

        private int height
        {
            get
            {

                int height = 0;
                if (int.TryParse(Attributes["height"] + "", out height))
                {
                    return height;
                }
                return 400;
            }
        }

        private bool AutoStart
        {
            get
            {
                if (Attributes.ContainsKey("autostart"))
                {
                    string auto = Attributes["autostart"].ToString();
                    if (auto == "1" || auto == "true")
                        return true;
                }

                return false;
            }
        }

        public override string StartUbb
        {
            get
            {
                string ubbText = string.Empty;
                string fileType = string.Empty;
                string tag = string.Empty;

                if (fileTypeReg.IsMatch(Src))
                {
                    fileType = fileTypeReg.Match(Src).Value;
                }

                switch (Type)
                {

                    case TypeOfFlash:          //flash
                        tag = "flash";
                        break;

                    case TypeOfWMedia:          //windows media
                        tag = "wmv";
                        switch (fileType)
                        {
                            case "mp3":
                            case "wav":
                            case "wma":
                            case "midi":
                                tag = "mp3";
                                break;
                        }
                        break;
                    case TypeOfRealMedia:       //realmedia
                        tag = "rm";
                        switch (fileType)
                        {
                            case "ra":
                                tag = "rm";
                                break;
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(tag))
                    return string.Format("[{0}={3},{4},{2}]{1}[/{0}]", tag, Src, AutoStart ? "1" : "0", width, height);

                return string.Empty;
            }
        }

        private string Src
        {
            get
            {
                return Attributes["src"] + "";
            }
        }
    }

    internal class Util
    {
        private static Regex regRGB = new Regex("rgb\\s*\\(\\s*(\\d+),\\s*(\\d+),\\s*(\\d+)\\s*\\)", RegexOptions.IgnoreCase);
        internal static string ConvertRGB(string htmltext)
        {
            //替换 rgb(255,255,255)
            return regRGB.Replace(htmltext, delegate(Match m)
            {
                int cr, cb, cg;
                string sr, sg, sb;
                cr = int.Parse(m.Groups[1].Value);
                cg = int.Parse(m.Groups[2].Value);
                cb = int.Parse(m.Groups[3].Value);
                sr = string.Format("{0:X}", cr);
                sg = string.Format("{0:X}", cg);
                sb = string.Format("{0:X}", cb);
                if (sr.Length < 2)
                    sr = "0" + sr;
                if (sg.Length < 2)
                    sg = "0" + sg;
                if (sb.Length < 2)
                    sb = "0" + sb;

                return string.Format("#{0}{1}{2}", sr, sg, sb);
            });
        }
    }
}