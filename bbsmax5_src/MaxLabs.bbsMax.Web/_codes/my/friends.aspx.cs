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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class friends : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "好友 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "friends"; }
        }
        
        protected override string NavigationKey
        {
            get { return "friends"; }
        }

        private int m_PageNumber;
        private int m_PageSize;
        private int? m_GroupID;
        private FriendGroup m_CurrentGroup = null;
        private FriendCollection m_FriendList = null;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            AddNavigationItem("好友");

            if (_Request.IsClick("query"))
            {
                Query();
                return;
            }
            else if (_Request.IsClick("addgroup"))
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                string groupName = _Request.Get("groupName", Method.Post);//"!"号作为特殊标记

                //bool success = false;
                int newGroupID = 0;
                try
                {
                    using (ErrorScope es = new ErrorScope())
                    {
                        newGroupID = FriendBO.Instance.AddFriendGroup(MyUserID, groupName);

                        if (newGroupID == 0)
                        {
                            es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                AlertError(error.Message);
                                msgDisplay.AddError(error);
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    AlertError(ex.Message);
                    msgDisplay.AddException(ex);
                }
            }

            m_GroupID = _Request.Get<int>("groupid", Method.Get);
            m_PageNumber = _Request.Get<int>("page", Method.Get, 1);
            m_PageSize = Consts.DefaultPageSize;


            if (m_GroupID != null)
            {
                m_CurrentGroup = FriendBO.Instance.GetFriendGroup(MyUserID, m_GroupID.Value);

                if (m_CurrentGroup == null)
                    m_GroupID = null;
            }

            if (m_GroupID == null)
                m_FriendList = FriendBO.Instance.GetFriends(MyUserID, m_PageNumber, m_PageSize);

            else
                m_FriendList = FriendBO.Instance.GetFriends(MyUserID, m_GroupID.Value, m_PageNumber, m_PageSize);

            //填充FriendList的用户资料
            this.WaitForFillSimpleUsers<Friend>(m_FriendList);

            SetPager("pager1", null, m_PageNumber, PageSize, m_FriendList.TotalRecords);
        }

        protected int? GroupID
        {
            get { return m_GroupID; }
        }

        protected string GroupTitle
        {
            get
            {
                if (m_GroupID == null)
                    return "所有好友";

                else
                    return m_CurrentGroup.Name;
            }
        }

        protected bool IsGroupShield
        {
            get
            {
                if (m_GroupID == null)
                    return false;
                else
                    return m_CurrentGroup.IsShield;
            }
        }
        
        protected FriendCollection FriendList
        {
            get { return m_FriendList; }
        }

        protected int PageSize
        {
            get { return m_PageSize; }
        }

        private FriendGroupCollection m_FriendGroupList = null;
        protected FriendGroupCollection FriendGroupList
        {
            get
            {
                if (m_FriendGroupList == null)
                {
                    m_FriendGroupList = FriendBO.Instance.GetFriendGroups(MyUserID);
                }

                return m_FriendGroupList;
            }
        }


        private void Query()
        {
            int groupID = _Request.Get<int>("groupid", -1);
            string queryName = _Request.Get("queryname");

			BbsRouter.JumpTo("friends/group-" + groupID + "/" + queryName);

            //Response.Redirect("~/max-templates/default/friends.aspx?groupid=" + groupID + "&queryname=" + queryName + "");
        }


    }
}