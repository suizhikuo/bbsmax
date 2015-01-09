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
using System.IO;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class friend_verify : DialogPageBase
    {
        private int m_NotifyID;
        private FriendNotify m_Notify;
        private SimpleUser m_TryAddUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_NotifyID = _Request.Get<int>("notifyid", Method.Get, 0);

            using (ErrorScope es = new ErrorScope())
            {
                Notify temp = NotifyBO.Instance.GetNotify(MyUserID, m_NotifyID);
                if (temp == null)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        ShowError(error);
                    });

                    ShowError(new NotExistsNotifyError(m_NotifyID));
                }

                m_Notify = new FriendNotify(temp);

                m_TryAddUser = UserBO.Instance.GetSimpleUser(m_Notify.RelateUserID,true);

                if (m_TryAddUser == null)
                {
                    NotifyBO.Instance.DeleteNotify(My, m_NotifyID);
                    ShowError(new UserNotExistsError(string.Empty, string.Empty));
                }
            }

            if (_Request.IsClick("accept"))
            {
                Verify();
            }

            //if (!FriendPO.GetInstance(userID).CanAddFriend(friendUserID))
            //{
            //    //没权限添加好友
            //}
        }


        protected bool CreateGroup
        {
            get
            {
                return _Request.Get<bool>("createGroup", Method.Post, false);
            }
        }


        //protected FriendGroupCollection FriendGroupList
        //{
        //    get
        //    {
        //        return My.FriendGroups;
        //    }
        //}


        protected string PostScript
        {
            get
            {
                return m_Notify.DataTable["PostScript"] + "";
            }
        }

        protected bool IsMyFriend
        {
            get
            {
                return FriendBO.Instance.IsFriend(MyUserID, m_TryAddUser.UserID);
            }
        }


        private int? m_FriendGroupID;
        protected int FriendGroupID
        {
            get
            {
                if (IsMyFriend)
                {
                    if (m_FriendGroupID == null)
                    {
                        Friend friend = FriendBO.Instance.GetFriend(MyUserID, TryAddUser.UserID);
                        m_FriendGroupID = friend.GroupID;
                    }
                    return m_FriendGroupID.Value;
                }
                else
                {
                    return 0;
                }
            }
        }

        protected SimpleUser TryAddUser
        {
            get { return m_TryAddUser; }
        }

        protected FriendGroupCollection FriendGroupList
        {
            get { return FriendBO.Instance.GetFriendGroups(MyUserID); }
        }

        private void Verify()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int groupID = _Request.Get<int>("ToFriendGroupID", Method.Post, 0);
            int notifyID = _Request.Get<int>("notifyid", Method.Get, 0);

            if (CreateGroup)
            {
                string groupName = _Request.Get("newgroup", Method.Post);
                groupID = FriendBO.Instance.AddFriendGroup(MyUserID, groupName);

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
                    if (IsMyFriend)
                    {
                        success = FriendBO.Instance.MoveFriends(MyUserID, new int[] { TryAddUser.UserID }, groupID);
                        NotifyBO.Instance.DeleteNotify(My, notifyID);
                    }
                    else
                    {
                        success = FriendBO.Instance.AcceptAddFriend(My, notifyID, groupID);
                    }
                    //NotifyBO.Instance.DeleteNotify(My, notifyID);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                });
            }


            if (success)
            {
                Return(notifyID);
            }
        }
    }
}