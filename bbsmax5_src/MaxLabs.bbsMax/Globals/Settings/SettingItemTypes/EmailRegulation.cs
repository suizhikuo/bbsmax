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

using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Settings
{
    public class TextRegulation : ISettingItem
    {
        private string m_Value;
        private Regex m_Regex;

        public TextRegulation(string value)
        {
            SetValue(value);
        }

        public string GetValue()
        {
            return m_Value;
        }

        public bool IsMach(string text)
        {
            if (m_Regex == null)
                return false;
            return m_Regex.IsMatch(text);
        }

        public void SetValue(string value)
        {
            m_Value = value;

            if (string.IsNullOrEmpty(m_Value))
                return;
            
            string[] lines = m_Value.Replace("\n", string.Empty).Split('\r');

            StringBuilder builder = new StringBuilder();

            bool isFirst = true;
            foreach (string str in lines)
            {
                if (str.Trim() == "")
                    continue;
                if (isFirst)
                    isFirst = false;
                else
                    builder.Append('|');
                builder.Append('^');
                builder.Append(str);
                builder.Append('$');
            }

            builder.Replace(".", "\\.");      //.做普通字符处理
            builder.Replace("*", ".*");       //*做任意多个任意字符处理
            builder.Replace("?", ".?");
            m_Regex = new Regex(builder.ToString(), RegexOptions.IgnoreCase);
        }

        public override string ToString()
        {
            return GetValue();
        }
    }
}