//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;

using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
	public partial class friend_list : SpacePageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			int pageSize = 36;
			int pageNumber = _Request.Get<int>("page", 1);

            int count = 0;

			if (IsViewAll)
			{
				m_FriendList = FriendBO.Instance.GetFriends(SpaceOwnerID, pageNumber, pageSize);

				if (m_FriendList != null)
				{
					m_FriendTotalCount = m_FriendList.TotalRecords;
                    count = m_FriendTotalCount;
					UserBO.Instance.WaitForFillSimpleUsers<Friend>(m_FriendList);
				}
			}
			else if (IsViewVisitor)
			{
				m_VisitorList = SpaceBO.Instance.GetSpaceVisitors(SpaceOwnerID, pageSize, pageNumber);

				if (m_VisitorList != null)
				{
					m_VisitorTotalCount = m_VisitorList.TotalRecords;
                    count = m_VisitorTotalCount;

					UserBO.Instance.WaitForFillSimpleUsers<Visitor>(m_VisitorList, 0);
				}
			}
			else if (IsViewTrace)
			{
				m_TraceList = SpaceBO.Instance.GetSpaceVisitTrace(SpaceOwnerID, pageSize, pageNumber);

				if (m_TraceList != null)
				{
					m_TraceTotalCount = m_TraceList.TotalRecords;
                    count = m_TraceTotalCount;

					UserBO.Instance.WaitForFillSimpleUsers<Visitor>(m_TraceList, 1);
				}
			}

            SetPager("list", null, pageNumber, pageSize, count);
        }

        public override string SpaceTitle
        {
            get
            {
                string what = null;

                if (IsViewAll)
                    what = "好友";
                else if (IsViewVisitor)
                    what = "访客";
                else if (IsViewTrace)
                    what = "足迹";

                return SpaceOwner.Name + "的" + what;
            }
        }

		protected override MaxLabs.bbsMax.Enums.SpacePrivacyType FunctionPrivacy
		{
			get { return SpaceOwner.FriendListPrivacy; }
		}

		public override bool IsSelectedFriend
		{
			get
			{
				return true;
			}
		}

		protected override bool EnableFunction
		{
			get { return true; }
		}

		private FriendCollection m_FriendList;

		public FriendCollection FriendList
		{
			get { return m_FriendList; }
		}

		private int m_FriendTotalCount;

		public int FriendTotalCount
		{
			get { return m_FriendTotalCount; }
		}

		private VisitorCollection m_VisitorList;

		public VisitorCollection VisitorList
		{
			get { return m_VisitorList; }
		}

		private int m_VisitorTotalCount;

		public int VisitorTotalCount
		{
			get { return m_VisitorTotalCount; }
		}

		private VisitorCollection m_TraceList;

		public VisitorCollection TraceList
		{
			get { return m_TraceList; }
		}

		private int m_TraceTotalCount;

		public int TraceTotalCount
		{
			get { return m_TraceTotalCount; }
		}

		private bool? m_IsViewAll;

		public bool IsViewAll
		{
			get
			{
				if (m_IsViewAll.HasValue == false)
					m_IsViewAll = _Request.Get("view", Method.Get) == null;

				return m_IsViewAll.Value;
			}
		}

		private bool? m_IsViewVisitor;

		public bool IsViewVisitor
		{
			get
			{
				if (m_IsViewVisitor.HasValue == false)
					m_IsViewVisitor = _Request.Get("view", Method.Get) == "visitor";

				return m_IsViewVisitor.Value;
			}
		}

		private bool? m_IsViewTrace;

		public bool IsViewTrace
		{
			get
			{
				if (m_IsViewTrace.HasValue == false)
					m_IsViewTrace = _Request.Get("view", Method.Get) == "trace";

				return m_IsViewTrace.Value;
			}
		}



        protected bool IsShowAddFriendLink(int userID)
        {
            if (MyUserID == userID)
                return false;

            return FriendBO.Instance.IsFriend(MyUserID, userID) == false;
        }

        protected override string PageTitle
        {
            get
            {
                return string.Concat(SpaceOwner.Name, " - ", SpaceTitle, " - ", base.PageTitle);
            }
        }
	}
}