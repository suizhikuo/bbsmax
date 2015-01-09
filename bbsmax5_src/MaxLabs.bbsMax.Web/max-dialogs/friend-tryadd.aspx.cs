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
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class friend_tryadd : DialogPageBase
    {
        private int m_UserIDToAdd;
        private SimpleUser m_UserToAdd;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_UserIDToAdd = _Request.Get<int>("uid", Method.Get, 0);

            if (_Request.IsClick("addfriend"))
            {
                Add();
            }

            m_UserToAdd = UserBO.Instance.GetSimpleUser(m_UserIDToAdd);

            if (m_UserToAdd == null)
                ShowError(new UserNotExistsError("uid", m_UserIDToAdd));

            //if (!FriendPO.GetInstance(userID).CanAddFriend(friendUserID))
            //{
            //    //没权限添加好友 已经是好友 不能添加自己为好友...
            //}
        }

        protected FriendGroupCollection FriendGroupList
        {
            get { return FriendBO.Instance.GetFriendGroups(MyUserID); }
        }


        protected string validateActionName
        {
            get { return "addfriend"; }
        }


        /// <summary>
        /// 将要添加为好友的用户ID
        /// </summary>
        protected int UserIDToAdd
        {
            get { return m_UserIDToAdd; }
        }

        private bool? m_IsMyFirend;
        protected bool IsMyFriend
        {
            get
            {
                if (m_IsMyFirend == null)
                {
                    m_IsMyFirend = FriendBO.Instance.IsFriend(MyUserID, UserIDToAdd);
                }
                return m_IsMyFirend.Value;
            }
            set { m_IsMyFirend = value; }
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
                        Friend friend = FriendBO.Instance.GetFriend(MyUserID, UserIDToAdd);
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

        /// <summary>
        /// 将要添加为好友的用户
        /// </summary>
        protected SimpleUser UserToAdd
        {
            get { return m_UserToAdd; }
        }

        protected bool CreateGroup
        {
            get
            {
                return _Request.Get<bool>("createGroup", Method.Post, false);
            }
        }

        private void Add()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay(GetValidateCodeInputName(validateActionName));

            int friendGroupID = _Request.Get<int>("ToFriendGroupID", Method.Post, 0);
            string message = _Request.Get("Note", Method.Post);
            string IP = _Request.IpAddress;

            //if (!FriendPO.GetInstance(MyUserID).CanAddFriend(friendUserID))
            //{
            //    //没权限添加好友
            //}

            ValidateCodeManager.CreateValidateCodeActionRecode(validateActionName);

            if ( IsMyFriend ==false && !CheckValidateCode(validateActionName, msgDisplay))
            {
                return;
            }

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
                    if (IsMyFriend)
                        success = FriendBO.Instance.MoveFriends(MyUserID, new int[] { m_UserIDToAdd }, friendGroupID);
                    else
                        success = FriendBO.Instance.TryAddFriend(My, m_UserIDToAdd, friendGroupID, message);
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
                //JsonBuilder json = new JsonBuilder();
                //json.Set("friendUserID", friendUserID);
                //json.Set("friendGroupID", friendGroupID);
                //Return(json);
                if (IsMyFriend)
                {
                    Return(true);
                }
                else
                {
                    ShowSuccess("已经申请添加对方为好友，请等待对方答复");
                }
            }
        }
    }
}