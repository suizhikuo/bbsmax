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
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using System.Collections.Specialized;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 密码保存器,访问保存器 等等
    /// </summary>
    public class TempDataBox
    {
        public readonly AuthUser Owner;

        public TempDataBox(AuthUser owner)
        {
            Owner = owner;
        }

        private HybridDictionary m_Data = null;

        public string GetData(string key)
        {
            if (Owner.UserID > 0)
            {
                if (m_Data == null)
                    return null;

                if (m_Data.Contains(key))
                    return m_Data[key].ToString();
                else
                    return null;
            }
            else
            {
#if Passport
                throw new NotSupportedException("在passport中，游客无法访问TempDataBox");
#else
                return OnlineUserPool.Instance.GetGuestTempData(Owner.BuildGuestID(), key);
#endif
            }
        }

        public void SetData(string key, string value)
        {
            if (Owner.UserID > 0)
            {
                if (m_Data == null)
                    m_Data = new HybridDictionary();

                if (m_Data.Contains(key))
                    m_Data[key] = value;
                else
                    m_Data.Add(key, value);
            }
            else
            {
#if Passport
                throw new NotSupportedException("在passport中，游客无法访问TempDataBox");
#else
                OnlineUserPool.Instance.SetGuestTempData(Owner.BuildGuestID(), key, value);
#endif
            }
        }
    }
}