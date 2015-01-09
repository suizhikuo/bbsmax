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

namespace MaxLabs.bbsMax.Settings
{
    public class UsernameKeywordRegulation : ISettingItem
    {
        private string m_Value;
        private string[] m_Keywords;

        public string GetValue()
        {
            return m_Value;
        }

        public UsernameKeywordRegulation()
        {
            m_Keywords = new string[]{

             "游客"
            ,"管理员"
            ,"隐身用户"
            ,"被删除用户"
            ,"创始人"
            ,"Admin"
            ,"版主"

            };
            m_Value = string.Join("\r\n",m_Keywords);
        }

        public bool IsMach(string input)
        {
            if (m_Keywords == null || m_Keywords.Length < 1)
                return false;
            foreach (string word in m_Keywords)
            {
                if (string.IsNullOrEmpty(word))
                    continue;
                if (input.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetValue(string value)
        {
            m_Value = value;
            if (string.IsNullOrEmpty(m_Value))
            {
                //m_Keywords = new string[00];
                return;
            }
            m_Keywords = m_Value.Replace("\n", string.Empty).Split('\r');
            return;
        }

        public override string ToString()
        {
            return GetValue();
        }
    }
}