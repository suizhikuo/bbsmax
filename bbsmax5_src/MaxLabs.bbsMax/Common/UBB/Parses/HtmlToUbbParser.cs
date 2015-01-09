//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web;
using MaxLabs.bbsMax.Ubb;


namespace MaxLabs.bbsMax
{
    public class HtmlToUbbParser
    {
        private static Regex reg_Pre = new Regex("<pre>(?is).*?</pre>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Html2Ubb(int userID, string html)
        {
            return Html2Ubb(userID, html, true);
        }

        public static string Html2Ubb(int userID, string html, bool parseEmoticon)
        {


            //******************新的HTML转UBB的代码*************
            if (userID >= 0 && parseEmoticon)
                html = EmoticonParser.HtmlToShortcut(userID, html, true); //表情转换(放在IMG转换之前)
            return HtmlToUbbConverter.ConvertToUbb(html);
            //****************** end.新的HTML转UBB的代码********

            /* 旧的HTML转UBB的代码
            Hashtable mapSize = new Hashtable();
            mapSize.Add("xx-small", "1");
            mapSize.Add("8pt", "1");
            mapSize.Add("x-small", "2");
            mapSize.Add("10pt", "2");
            mapSize.Add("small", "3");
            mapSize.Add("12pt", "3");
            mapSize.Add("medium", "4");
            mapSize.Add("14pt", "4");
            mapSize.Add("large", "5");
            mapSize.Add("18pt", "5");
            mapSize.Add("x-large", "6");
            mapSize.Add("24pt", "6");
            mapSize.Add("xx-large", "7");
            mapSize.Add("36pt", "7");

            Hashtable preTable = new Hashtable();
            

           //PRE处理
            string sUBB = html;
            reg_Pre.Replace(sUBB, delegate(Match m) {

                Guid g = Guid.NewGuid();
                preTable.Add(g, m.Value);
                return g.ToString();
            });

            sUBB = Regex.Replace(sUBB, @"\r?\n", "");
            sUBB = Regex.Replace(sUBB, @"<(\/?)(b|u|i|s)(\s+[^>]+)?>", "[$1$2]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<(\/?)strong(\s+[^>]+)?>", "[$1b]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<(\/?)em(\s+[^>]+)?>", "[$1i]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<(\/?)(strike|del)(\s+[^>]+)?>", "[$1s]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<(\/?)(sup|sub)(\s+[^>]+)?>", "[$1$2]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<font\s+color=""(.+?)"">(.*?)<\/font>", "[color=$1]$2[/color]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<font\s+size=""(.+?)"">(.*?)<\/font>", "[size=$1]$2[/size]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<font\s+face=""(.+?)"">(.*?)<\/font>", "[font=$1]$2[/font]", RegexOptions.IgnoreCase);

            for (int i = 0; i < 3; i++)
                sUBB = Regex.Replace(sUBB, @"<(span|div|font)(?>\s+[^>]+)? style=""((?>[^""]*?;)*\s*(?>font-weight|font-style|text-decoration|text-align|font-family|font-size|color|background|background-color)\s*:[^""]*)""(?> [^>]+)?>(((?!<\1(\s+[^>]+)?>)[\s\S]|<\1(\s+[^>]+)?>((?!<\1(\s+[^>]+)?>)[\s\S]|<\1(\s+[^>]+)?>((?!<\1(\s+[^>]+)?>)[\s\S])*?<\/\1>)*?<\/\1>)*?)<\/\1>", delegate(Match match)
                {

                    string style = match.Groups[2].Value;
                    string weight = Regex.Match(style, @"(?<=(?>^|;)\s*font-weight\s*:\s*)([^"";]+)").Value;
                    string italic = Regex.Match(style, @"(?<=(?>^|;)\s*font-style\s*:\s*)([^;""]+)").Value;
                    string deco = Regex.Match(style,   @"(?<=(?>^|;)\s*text-decoration\s*:\s*)([^"";]+)").Value;
                    string align = Regex.Match(style,  @"(?<=(?>^|;)\s*text-align\s*:\s*)([^"";]+)").Value;
                    string face = Regex.Match(style,   @"(?<=(?>^|;)\s*font-family\s*:\s*)([^"";]+)").Value;
                    string size = Regex.Match(style,   @"(?<=(?>^|;)\s*font-size\s*:\s*)([^"";]+)").Value;
                    string color = Regex.Match(style,  @"(?<=(?>^|;)\s*color\s*:\s*)([^"";]+)").Value;
                    string back = Regex.Match(style,   @"(?<=(?>^|;)\s*(?>background|background-color)\s*:\s*)([^"";]+)").Value;
                    string str = match.Groups[3].Value;

                    if (!string.IsNullOrEmpty(weight))
                    {
                        if (weight != "" && weight.ToLower() != "normal") str = "[b]" + str + "[/b]";
                    }
                    if (!string.IsNullOrEmpty(italic))
                    {
                        if (italic.ToLower() == "{italic") str = "[i]" + str + "[/i]";
                    }
                    if (!string.IsNullOrEmpty(deco))
                    {
                        if (deco.ToLower() == "underline") str = "[u]" + str + "[/u]";
                        else if (deco.ToLower() == "line-through") str = "[s]" + str + "[/s]";
                    }
                    if (!string.IsNullOrEmpty(align))
                    {
                        align = align.ToLower();
                        if (align == "left" || align == "center" || align == "right") str = "[align=" + align + "]" + str + "[/align]";
                    }
                    if (!string.IsNullOrEmpty(face)) str = "[font=" + face + "]" + str + "[/font]";
                    if (!string.IsNullOrEmpty(size))
                    {
                        size = mapSize[size.ToLower()].ToString();
                        if (!string.IsNullOrEmpty(size)) str = "[size=" + size + "]" + str + "[/size]";
                    }
                    if (!string.IsNullOrEmpty(color)) str = "[color=" + color + "]" + str + "[/color]";

                    if (!string.IsNullOrEmpty(back)) str = "[bgcolor=" + back + "]" + str + "[/bgcolor]";
                    return str;
                });

            for (int i = 0; i < 3; i++)
                sUBB = Regex.Replace(sUBB, @"<(div|p)(?>\s+[^>]+?)?\s+align=""(left|center|right)""[^>]*>(((?!<\1(\s+[^>]+)?>)[\s\S])+?)<\/\1>", "[align=$2]$3[/align]");

            sUBB = Regex.Replace(sUBB, @"<a\s+[^>]*?href=""\s*([^""]+?)\s*""[^>]*>([\s\S]+?)<\/a>", delegate(Match match)
            {
                string url = match.Groups[1].Value, text = match.Groups[2].Value;
                string tag = "url", str;
                if (url.ToLower().Contains("^mailto:"))
                {
                    tag = "email";
                    url = Regex.Replace(url, "mailto:(.+?)", "$1");
                }
                str = "[" + tag;
                if (url != text) str += "=" + url;
                return str + "]" + text + "[/" + tag + "]";
            });

            if(userID >= 0&& parseEmoticon)
            sUBB = EmoticonParser.HtmlToShortcut(userID, sUBB, true); //表情转换(放在IMG转换之前)
            

            sUBB = Regex.Replace(sUBB, @"<img(\s+[^>]+?)\/?>", delegate(Match match)
            {
                string attr = match.Groups[1].Value;

                string url = Regex.Match(attr, @"(?<=\s+src="")[^""]+").Value;
                string w = Regex.Match(attr,   @"(?<=\s+width="")\d+%?").Value;
                string h = Regex.Match(attr,   @"(?<=\s+height="")\d+%?").Value;
                string str = "[img";
                if (!string.IsNullOrEmpty(w) && !string.IsNullOrEmpty(h)) str += "=" + w + "," + h;
                str += "]" + url;
                return str + "[/img]";
            });


            sUBB = Regex.Replace(sUBB, @"<blockquote(?> [^>]+)?>([\s\S]+?)<\/blockquote>", "[indent]$1[/indent]");
            sUBB = Regex.Replace(sUBB, @"<code(?> [^>]+)?>([\s\S]+?)<\/code>", "[code]$1[/code]");

            //******** 处理  Flash WMA  RM *********
            ConvertEmbed(ref sUBB, "application/x-shockwave-flash", "flash");
            ConvertEmbed(ref sUBB, "application/x-mplayer2", "wmv");
            ConvertEmbed(ref sUBB, "audio/x-pn-realaudio-plugin", "rm");
            

            Regex regbg = new Regex(@"(?>background|background-color|bgcolor)\s*[:=]\s*([""']?)\s*((rgb\s*\(\s*\d{1,3}%?,\s*\d{1,3}%?\s*,\s*\d{1,3}%?\s*\))|(#[0-9a-f]{3,6})|([a-z]{1,20}))\s*\1", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            sUBB = Regex.Replace(sUBB, @"<table(\s+[^>]+|)?>", delegate(Match match)
            {
                string str = "[table";
                string attr = match.Groups[1].Value;
                if (!string.IsNullOrEmpty(attr))
                {
                    string w = Regex.Match(attr, @"(?<=\s+width\s*=\s*"")\s*([^""]+)\s*").Value;
                    Match b = regbg.Match(attr);
                    if (!string.IsNullOrEmpty(w))
                    {
                        str += "=" + w;
                        if (b.Success) str += "," + b.Groups[2].Value;
                    }
                }
                return str + "]";
            });

            sUBB = Regex.Replace(sUBB, @"<(/?)tr\s?[^>]*?>", "[$1tr]", RegexOptions.IgnoreCase);// function(all, attr) {
            //    var str = '[tr';
            //    if (attr) {
            //        var bg = attr.match(regbg)
            //        if (bg) str += '=' + bg[2];
            //    }
            //    return str + ']';
            //});

            sUBB = Regex.Replace(sUBB, @"<(?>th|td)(\s+[^>]+|)?>", delegate(Match match)
            {
                string attr = match.Groups[1].Value;
                string str = "[td";
                if (!string.IsNullOrEmpty(attr))
                {
                    string col = Regex.Match(attr, @"(?<=\s+colspan\s*=\s*""\s*)[^""]+(?=\s*"")").Value;
                    string row = Regex.Match(attr, @"(?<=\s+rowspan\s*=\s*""\s*)[^""]+(?=\s*"")").Value;
                    string w = Regex.Match(attr, @"\s+width\s*=\s*""\s*([^""]+)\s*""").Value;
                    col = string.IsNullOrEmpty(col) ? "1" : col;
                    row = string.IsNullOrEmpty(row) ? "1" : row;
                    if (col.CompareTo("1") > 0 || row.CompareTo("1") > 0 || !string.IsNullOrEmpty(w))
                    {
                        str += "=" + col + "," + row;
                        if (!string.IsNullOrEmpty(w)) str += ',' + w;
                    }
                }
                return str + "]";
            });
            sUBB = Regex.Replace(sUBB, @"<\/(table|tr)>", "[/$1]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<\/(?>th|td)>", "[/td]", RegexOptions.IgnoreCase);

            sUBB = Regex.Replace(sUBB, @"<ul(\s+[^>]+|)?>", delegate(Match match)
            {
                string attr = match.Groups[1].Value;
                string t = "";
                if (!string.IsNullOrEmpty(attr))
                    t = Regex.Match(attr, @"(?<=\s+type\s*=\s*"")[^""]+(?="")").Value;
                return "[list" + (!string.IsNullOrEmpty(t) ? "=" + t : "") + "]";
            });

            sUBB = Regex.Replace(sUBB, @"<ol(\s+[^>]+)?>", "[list=1]");
            sUBB = Regex.Replace(sUBB, "<ol(\\s+[^>]+)?>", "[list=1]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<li(\s+[^>]+)?>", "\r\n[*]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<\/li>", "", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<\/(?>ul|ol)>", "\r\n[/list]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<h([1-6])[^>]*?>", delegate(Match match)
            { return "\r\n[size=" + (7 - int.Parse(match.Groups[1].Value)) + "][b]"; });
            sUBB = Regex.Replace(sUBB, @"<\/h[1-6]>", "[/b][/size]\r\n");
            sUBB = Regex.Replace(sUBB, @"<pre(\s+[^>]+)?>", "\r\n[code]", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<\/pre>", "[/code]\r\n", RegexOptions.IgnoreCase);


            sUBB = Regex.Replace(sUBB, @"<(p)(?>\s+[^>]+)?>(((?!<\1(\s+[^>]+)?>)[\s\S]|<\1(\s+[^>]+)?>((?!<\1(\s+[^>]+)?>)[\s\S]|<\1(\s+[^>]+)?>((?!<\1(\s+[^>]+)?>)[\s\S])*?<\/\1>)*?<\/\1>)*?)<\/\1>", "\r\n$2\r\n");
            sUBB = Regex.Replace(sUBB, @"<(div)(?>\s+[^>]+)?>(((?!<\1(\s+[^>]+)?>)[\s\S]|<\1(\s+[^>]+)?>((?!<\1(\s+[^>]+)?>)[\s\S]|<\1(\s+[^>]+)?>((?!<\1(\s+[^>]+)?>)[\s\S])*?<\/\1>)*?<\/\1>)*?)<\/\1>", "\r\n$2\r\n");
            sUBB = Regex.Replace(sUBB, @"<br\s*?/?>", "\r\n", RegexOptions.IgnoreCase);
            sUBB = Regex.Replace(sUBB, @"<.*?>", ""); //删除所有无法转换的HTML标记

            ////sUBB = sUBB.replace(/&;/ig, ' ');
            sUBB = Regex.Replace(sUBB, @"(?>\s*?\r?\n){3,}", "\r\n"); //限制最多2次换行
            sUBB = HttpUtility.HtmlDecode(sUBB);

            foreach (object o in preTable.Keys)//还原pre内容
            {
                sUBB = sUBB.Replace(o.ToString(), preTable[o].ToString());
            }

            return sUBB.Trim();
           
        }

        private static void ConvertEmbed(ref string html, string applicationType, string tag)
        {
            html = Regex.Replace(html, @"<embed\s+[^>]*?type=""" + applicationType + @"""[^>]*?>", delegate(Match match)
            {
                string attr = match.Groups[0].Value;
                string url = Regex.Match(attr, @"(?<=\s+src\s*=\s*""\s*)[^""]+(?=\s*"")").Value;
                string w = Regex.Match(attr,   @"(?<=\s+width\s*=\s*""\s*)[^""]+(?=\s*"")").Value;
                string h = Regex.Match(attr,   @"(?<=\s+height\s*=\s*""\s*)[^""]+(?=\s*"")").Value;
                string p = Regex.Match(attr,   @"(?<=\s+(?>autostart|play)\s*=\s*""\s*)[^""]+(?=\s*"")").Value;
                string str = "[" + tag, auto = "0";
                if (!string.IsNullOrEmpty(p)) if (p == "true") auto = "1";
                if (!string.IsNullOrEmpty(w) && !string.IsNullOrEmpty(h)) str += "=" + w + "," + h + "," + auto;
                str += "]" + url;
                return str + "[/" + tag + "]";
            });
        }
       */
        }
    }
}