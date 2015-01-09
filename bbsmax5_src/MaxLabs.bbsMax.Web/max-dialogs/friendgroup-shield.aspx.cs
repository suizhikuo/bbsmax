//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class friendgroup_shield : DialogPageBase
    {
        private int m_GroupID;
        private FriendGroup m_Group;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_GroupID = _Request.Get<int>("groupID", Method.Get, 0);

            if (m_GroupID <= 0)
                ShowError(new NotExistsFriendGroupError(m_GroupID));

            if (_Request.IsClick("shieldgroup"))
            {
                Shield();
            }

            m_Group = FriendBO.Instance.GetFriendGroup(MyUserID, m_GroupID);
        }

        protected FriendGroup Group
        {
            get { return m_Group; }
        }

        private void Shield()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            bool isShield = _Request.Get<bool>("isShield", Method.Post, false) ? false : true;

            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = FriendBO.Instance.ShieldFriendGroup(MyUserID, m_GroupID, isShield);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (success == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }

            if (success)
            {
                JsonBuilder json = new JsonBuilder();
                json.Set("groupID", m_GroupID);
                json.Set("isShield", isShield);
                Return(json);
            }
        }
    }
}