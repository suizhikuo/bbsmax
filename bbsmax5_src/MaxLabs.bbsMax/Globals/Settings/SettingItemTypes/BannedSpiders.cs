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
using MaxLabs.bbsMax.Enums;
using System.Reflection;

namespace MaxLabs.bbsMax.Settings
{
    public class BannedSpiders : ISettingItem//, IEnumerable<SpiderType>
    {
        private StringList m_BannedNames = null;
        private bool[] m_BannedSpiders = null;// = new bool[];

        public string GetValue()
        {
            return m_BannedNames.ToString();
        }

        public void SetValue(string value)
        {
            m_BannedNames = StringList.Parse(value);
            m_BannedSpiders = null;
        }

        public StringList GetBannedSpiderNames()
        {
            return m_BannedNames;
        }

        public void SetBannedSpiders(IEnumerable<string> spiderNames)
        {
            StringList bannedNames = new StringList();

            if (spiderNames != null)
            {
                foreach (string spiderName in spiderNames)
                {
                    bannedNames.Add(spiderName);
                }
            }
            m_BannedNames = bannedNames;
            m_BannedSpiders = null;
        }

        public bool IsBanned(SpiderType spiderType)
        {
            bool[] bannedSpiders = m_BannedSpiders;

            if (bannedSpiders == null)
            {
                StringList bannedNames = m_BannedNames;

                FieldInfo[] fields = typeof(SpiderType).GetFields();

                bannedSpiders = new bool[fields.Length - 1];

                int i = 0;
                foreach (FieldInfo field in fields)
                {
                    if (i != 0)
                    {
                        if (bannedNames != null && bannedNames.Contains(field.Name))
                            bannedSpiders[i - 1] = true;
                        else
                            bannedSpiders[i - 1] = false;
                    }
                    i++;
                }

                m_BannedSpiders = bannedSpiders;
            }

            return bannedSpiders[(int)spiderType];
        }
    }
}