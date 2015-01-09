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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class friend_shield : DialogPageBase
    {
        private int m_FriendUserID;
        private Friend m_FriendToShield;

        protected void Page_Load(object sender, EventArgs e)
        {

            m_FriendUserID = _Request.Get<int>("uid", Method.Get, 0);

            if (_Request.IsClick("shieldfriend"))
            {
                Shield();
                return;
            }

            using (ErrorScope es = new ErrorScope())
            {
                m_FriendToShield = FriendBO.Instance.GetFriend(MyUserID, m_FriendUserID);
                WaitForFillSimpleUser<Friend>(m_FriendToShield);

                if (es.HasUnCatchedError)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        ShowError(error);
                        return;
                    });
                }
            }

        }

        protected int FriendUserID
        {
            get { return m_FriendUserID; }
        }

        protected Friend FriendToShield
        {
            get { return m_FriendToShield; }
        }

        private void Shield()
        {
            bool isShield = _Request.Get<bool>("isShield", Method.Post, false) ? false : true;

            MessageDisplay msgDisplay = CreateMessageDisplay();

            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    if (isShield)
                        success = FeedBO.Instance.FiltrateFeed(MyUserID, m_FriendUserID);
                    else
                        success = FeedBO.Instance.UnFiltrateFeed(m_FriendUserID);
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
                json.Set("targetUserID", m_FriendUserID);
                json.Set("isShield", isShield);
                Return(json);
                //msgDisplay.ShowInfo(this);
            }

        }
    }
}