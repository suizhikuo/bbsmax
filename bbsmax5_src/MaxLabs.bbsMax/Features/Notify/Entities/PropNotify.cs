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

namespace MaxLabs.bbsMax.Entities
{
    public class PropNotify:Notify
    {

        protected int PropID
        {
            get
            {
                return DataTable.ContainsKey("PropID") ?
                    StringUtil.TryParse<int>(DataTable["PropID"], 0)
                    : 0;
            }
            set
            {
                DataTable["PropID"] = value.ToString();
            }
        }

        protected int PropCount
        {
            get
            {
                return DataTable.ContainsKey("PropCount") ?
                    StringUtil.TryParse<int>(DataTable["PropCount"], 0)
                    : 0;
            }
            set
            {
                DataTable["PropCount"] = value.ToString();
            }
        }
        private SimpleUser m_RelateUser;
        protected SimpleUser RelateUser
        {
            get
            {
                if (m_RelateUser == null)
                {
                    m_RelateUser = UserBO.Instance.GetSimpleUser(this.RelateUserID);
                }
                return m_RelateUser;
            }
        }
        public override int TypeID
        {
            get
            {
                return (int)FixNotifies.PropNotify;
            }
        }
        public int RelateUserID
        {
            get;
            set;
        }
        public override string Keyword
        {
            get
            {
                return string.Format("{0}|{1}|{2}", this.PropID, this.RelateUserID, this.PropCount);
            }
        }
    }
}