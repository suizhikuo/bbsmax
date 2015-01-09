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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.RegExp;
using System.Web;

namespace MaxLabs.bbsMax
{
    public class EmoticonParser
    {
        //private static readonly string HtmlFormat = "<img src=\"{1}\" emoticon=\"{0}\" />";

        public static string ParseToHtml(int userID, string content, bool parseUserEmoticon, bool parseDefaultEmoticon)
        {
            Dictionary<string, string> parsedShortcats = new Dictionary<string, string>();

            StringBuilder builder = new StringBuilder(content);

#if !Passport

            if (parseUserEmoticon)
            {
                foreach (IEmoticonBase emote in EmoticonBO.Instance.GetEmoticons(userID))
                {
                    
                    if (parsedShortcats.ContainsKey(emote.Shortcut))
                        continue;

                    string result = emote.ImageSrc;
                    if (StringUtil.StartsWith(result, '~'))
                        result = string.Concat("<img src=\"{$root}", result.Remove(0, 1), "\" emoticon=\"", emote.Shortcut, "\" alt=\"\" />");
                    else
                        result = string.Concat("<img src=\"", result.Remove(0, 1), "\" emoticon=\"", emote.Shortcut, "\" alt=\"\" />");

                    builder.Replace(emote.Shortcut, result);
                    parsedShortcats.Add(emote.Shortcut, string.Empty);
                }
            }

            if (parseDefaultEmoticon)
            {
                foreach (EmoticonGroupBase group in AllSettings.Current.DefaultEmotSettings.AvailableGroups)
                {

                    foreach (IEmoticonBase emote in (group as DefaultEmoticonGroup).Emoticons)
                    {
                        string result = emote.ImageSrc;

                        if (parsedShortcats.ContainsKey(emote.Shortcut))
                            continue;

                        if (StringUtil.StartsWith(result, '~'))
                            result = string.Concat("<img src=\"{$root}", result.Remove(0, 1), "\" emoticon=\"", emote.Shortcut, "\" alt=\"\" />");
                        else
                            result = string.Concat("<img src=\"", result.Remove(0, 1), "\" emoticon=\"", emote.Shortcut, "\" alt=\"\" />");

                        builder.Replace(emote.Shortcut, result);
                        parsedShortcats.Add(emote.Shortcut, string.Empty);
                    }
                }
            }

#endif

            return builder.ToString();
        }

        public static string ParseToHtml(int userID, string content)
        {
            return ParseToHtml(userID, content, true, true);
        }


        private static Regex s_EmoticonRegex = null;
        /// <summary>
        /// 把表情转回快捷方式
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string HtmlToShortcut(int userID, string content, bool isUbbMode)
        {
            if (isUbbMode)
            {
                if (s_EmoticonRegex == null)
                    s_EmoticonRegex = new EmoticonRegex();

                content = s_EmoticonRegex.Replace(content, "$1");
                return content;
            }
            return content;
        }
    }
}