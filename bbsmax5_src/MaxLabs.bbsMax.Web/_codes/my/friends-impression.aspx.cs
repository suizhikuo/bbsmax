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
using System.Collections;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class friends_impression : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return string.Concat("好友印象 - ", base.PageTitle); }
        }

        protected override string PageName
        {
            get { return "friends"; }
        }

        protected override string NavigationKey
        {
            get { return "friends-impression"; }
        }

        private int m_PageNumber;
        private int m_PageSize;
        private int? m_GroupID;
        private FriendGroup m_CurrentGroup = null;
        private FriendCollection m_FriendList = null;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //AddNavigationItem("好友", BbsRouter.GetUrl("my/friends"));
            AddNavigationItem("好友印象");

            if (_Request.IsClick("query"))
            {
                Query();
                return;
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

            List<int> userids = new List<int>();

            foreach(Friend firend in m_FriendList)
            {
                userids.Add(firend.UserID);
            }

            m_ImpressionData = ImpressionBO.Instance.GetTopImressionTypesForUsers(userids.ToArray(), 12);

            SetPager("pager1", null, m_PageNumber, PageSize, m_FriendList.TotalRecords);
        }



        private Hashtable m_ImpressionData;

        protected string GetImpressionList(int userID)
        {
            ImpressionTypeCollection impressions = m_ImpressionData[userID] as ImpressionTypeCollection;

            if (impressions != null)
            {
                return StringUtil.Join(impressions, ", ");
            }

            return string.Empty;
        }

        protected int? GroupID
        {
            get { return m_GroupID; }
        }

        protected string GroupTitle
        {
            get { return "好友印象"; }
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

        private void Query()
        {
            int groupID = _Request.Get<int>("groupid", -1);
            string queryName = _Request.Get("queryname");

			BbsRouter.JumpTo("friends/group-" + groupID + "/" + queryName);

            //Response.Redirect("~/max-templates/default/friends.aspx?groupid=" + groupID + "&queryname=" + queryName + "");
        }


    }
}