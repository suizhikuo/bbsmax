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
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Errors
{
    public class AttachAvatarExistsError : ErrorInfo
    {
        private string m_ExistsAttachAvatarSrc;
        private DateTime m_ExistsAttachAvatarEndData;

        public AttachAvatarExistsError(string existsAttachAvatarSrc, DateTime existsAttachAvatarEndData)
        {
            m_ExistsAttachAvatarSrc = existsAttachAvatarSrc;
            m_ExistsAttachAvatarEndData = existsAttachAvatarEndData;
        }

        public override string Message
        {
            get
            {
                string message = "设置附加头像失败，因为附加头像已经存在";

                if (m_ExistsAttachAvatarEndData.Year == 9999)
                    message += "(将于 " + m_ExistsAttachAvatarEndData + " 过期)";

                return message;
            }
        }

        /// <summary>
        /// 已经存在的附加头像地址
        /// </summary>
        public string ExistsAttachAvatarSrc
        {
            get { return m_ExistsAttachAvatarSrc; }
        }

        /// <summary>
        /// 已经存在的附加头像将于什么时候过期（如果时间在9999年，则表示永不过期）
        /// </summary>
        public DateTime ExistsAttachAvatarEndData
        {
            get { return m_ExistsAttachAvatarEndData; }
        }
    }
}