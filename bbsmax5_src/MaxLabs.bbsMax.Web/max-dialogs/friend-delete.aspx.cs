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
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class friend_delete : DialogPageBase
    {
        private int[] m_FriendUserIds;
        private FriendCollection m_FriendListToDelete;
        private string m_FriendUserIdsText;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_FriendUserIdsText = _Request.Get("uid");
            m_FriendUserIds = StringUtil.Split<int>(m_FriendUserIdsText, ',');

            if (_Request.IsClick("deletefriend"))
            {
                Delete();
            }

            using (ErrorScope es = new ErrorScope())
            {
                m_FriendListToDelete = FriendBO.Instance.GetFriends(MyUserID, m_FriendUserIds);

                WaitForFillSimpleUsers<Friend>(m_FriendListToDelete);

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

        protected string FriendUserIdsText
        {
            get { return m_FriendUserIdsText; }
        }

        protected FriendCollection FriendListToDelete
        {
            get { return m_FriendListToDelete; }
        }


        private void Delete()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = FriendBO.Instance.DeleteFriends(MyUserID, m_FriendUserIds);
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
                Return("targetUserIds", m_FriendUserIds);
        }
    }
}