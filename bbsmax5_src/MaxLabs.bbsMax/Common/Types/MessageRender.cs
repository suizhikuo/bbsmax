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

using MaxLabs.bbsMax.Settings;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax
{

    /// <summary>
    /// Email消息标签替换器
    /// </summary>
    public class MessageRender
    {
        private const string variableBegin = "\\{";
        private const string variableEnd = "\\}";
        private Dictionary<string, string> replaceVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public MessageRender()
        {
            RegisterVariable("site", AllSettings.Current.SiteSettings.SiteUrl);
            RegisterVariable("sitename", AllSettings.Current.SiteSettings.SiteName);
            RegisterVariable("siteurl", Globals.FullAppRoot);
            RegisterVariable("now",DateTimeUtil.FormatDateTime(DateTimeUtil.Now,false));
            RegisterVariable("date", DateTimeUtil.FormatDate(DateTimeUtil.Now));
        }

        public string this[string key]
        {
            get
            {
                return replaceVariables[key];
            }
            set
            {
                RegisterVariable(key, value);
            }
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <returns></returns>
        public string Render(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            foreach (KeyValuePair<string, string> item in replaceVariables)
            {
                if (!string.IsNullOrEmpty(item.Key) && text.IndexOf(item.Key, StringComparison.OrdinalIgnoreCase) != -1)
                    text = Regex.Replace(text,string.Format("{0}{1}{2}", variableBegin , item.Key , variableEnd), item.Value==null?"":item.Value, RegexOptions.IgnoreCase);
            }
            return text;
        }
        public void RegisterVariable(string varName, string value)
        {
            if (varName != null)
            {
                if (replaceVariables.ContainsKey(varName))
                    replaceVariables[varName] = value;
                else
                    replaceVariables.Add(varName, value);
            }
        }
    }
}