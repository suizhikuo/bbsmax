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
using System.Text;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class friend_move : DialogPageBase
    {
        private int[] m_FriendUserIds;
        private FriendCollection m_FriendListToMove;
        private string m_FriendUserIdsText;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_FriendUserIdsText = _Request.Get("uid");
            m_FriendUserIds = StringUtil.Split<int>(m_FriendUserIdsText, ',');

            if (_Request.IsClick("movefriend"))
            {
                Move();
            }

            using (ErrorScope es = new ErrorScope())
            {
                m_FriendListToMove = FriendBO.Instance.GetFriends(MyUserID, m_FriendUserIds);

                WaitForFillSimpleUsers<Friend>(m_FriendListToMove);

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

        protected FriendCollection FriendListToMove
        {
            get { return m_FriendListToMove; }
        }

        protected int CurrentGroupID
        {
            get { return m_FriendListToMove[0].GroupID; }
        }

        protected FriendGroupCollection FriendGroupList
        {
            get { return FriendBO.Instance.GetFriendGroups(MyUserID); }
        }

        protected bool CreateGroup
        {
            get
            {
                return _Request.Get<bool>("createGroup", Method.Post, false);
            }
        }

        private void Move()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int friendGroupID = _Request.Get<int>("groupid", Method.Post, 0);

            //int[] ids = StringUtil.Split<int>(friendUserIDs, ',');

            if (CreateGroup)
            {
                string groupName = _Request.Get("newgroup", Method.Post);
                friendGroupID = FriendBO.Instance.AddFriendGroup(MyUserID, groupName);

                if (HasUnCatchedError)
                {

                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                    return;
                }
            }


            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = FriendBO.Instance.MoveFriends(MyUserID, m_FriendUserIds, friendGroupID);
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

            //成功就返回
            if (success)
            {
                JsonBuilder json = new JsonBuilder();
                json.Set("newGroupID", friendGroupID);
                json.Set("userIds", m_FriendUserIds);
                Return(json);
                //msgDisplay.ShowInfo(this);
            }
        }
    }
}