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
    public sealed class PermissionItem : ICloneable<PermissionItem>
    {

        public string Name { get; internal set; }

        public string InputName { get; internal set; }

        public string FieldName { get; internal set; }

        public Role Role { get; internal set; }

        private bool m_IsAllow, m_IsDeny;

        public bool IsAllow
        {
            get { return m_IsAllow; }
            set
            {
                if (value)
                {
                    m_IsAllow = true;
                    m_IsDeny = false;
                }
                else
                    m_IsAllow = false;
            }
        }

        public bool IsDeny
        {
            get { return m_IsDeny; }
            set
            {
                if (value)
                {
                    m_IsDeny = true;
                    m_IsAllow = false;
                }
                else
                    m_IsDeny = false;
            }
        }


        public bool IsNotset
        {
            get
            {
                if (m_IsAllow || m_IsDeny)
                    return false;

                return true;
            }
            set
            {
                if (value)
                {
                    m_IsAllow = false;
                    m_IsDeny = false;
                }
                else
                {
                    m_IsAllow = false;
                    m_IsDeny = true;
                }
            }
        }

        public bool IsDisabled { get; set; }


        public PermissionItem Clone()
        {
            PermissionItem item = new PermissionItem();

            item.FieldName = this.FieldName;
            item.InputName = this.InputName;
            item.m_IsAllow = m_IsAllow;
            item.m_IsDeny = m_IsDeny;
            item.IsDisabled = this.IsDisabled;
            item.Name = this.Name;
            item.Role = this.Role;

            return item;
        }

    }
}