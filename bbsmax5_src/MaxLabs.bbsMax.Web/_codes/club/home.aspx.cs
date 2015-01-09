//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web
{
	public partial class max_pages_club_home : BbsPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			m_ClubID = _Request.Get<int>("cid", 0);

			if(m_ClubID <= 0)
				ShowError("所访问的群组URL不是合法格式");

			ClubMemberStatus? status = null;

			Club club = ClubBO.Instance.GetClubForVisit(My, m_ClubID, out status);

			if (club != null)
			{
				if (status != null)
				{
					m_VisitorIsMember = ClubBO.Instance.CheckIsMember(status);

					m_VisitorIsManager = ClubBO.Instance.CheckIsManager(status);
				}

				m_ClubMemberList = ClubBO.Instance.GetTopClubMembers(m_ClubID, 9);

				m_ClubManagerList = ClubBO.Instance.GetTopClubManagers(m_ClubID, 9);

				m_ClubIsNeedManager = club.IsNeedManager;

				UserBO.Instance.WaitForFillSimpleUsers<ClubMember>(m_ClubMemberList);
				UserBO.Instance.WaitForFillSimpleUsers<ClubMember>(m_ClubManagerList);
			}
			else
			{
				ShowError("此群组只允许群组成员访问");
			}
		}

		private int m_ClubID;

		public int ClubID
		{
			get { return m_ClubID; }
		}

		private bool m_VisitorIsMember;

		public bool VisitorIsMember
		{
			get { return m_VisitorIsMember; }
		}

		private bool m_VisitorIsManager;

		public bool VisitorIsManager
		{
			get { return m_VisitorIsManager; }
		}

		private bool m_ClubIsNeedManager;

		public bool ClubIsNeedManager
		{
			get { return m_ClubIsNeedManager; }
		}

		private ClubMemberCollection m_ClubMemberList;

		public ClubMemberCollection ClubMemberList
		{
			get { return m_ClubMemberList; }
		}

		private ClubMemberCollection m_ClubManagerList;

		public ClubMemberCollection ClubManagerList
		{
			get { return m_ClubManagerList; }
		}
	}
}