//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Web;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;

public partial class max_pages_icenter_club_invite : CenterPageBase
{

    protected override string PageName
    {
        get
        {
            return "setting";
        }
    }
	private int m_PageNumber;
	private int m_PageSize;
	private int? m_GroupID;
	private FriendGroup m_CurrentGroup = null;
	private FriendCollection m_FriendList = null;

	protected void Page_Load(object sender, EventArgs e)
	{
		int? clubID = _Request.Get<int>("id");

		if (clubID.HasValue)
		{
			ClubMemberStatus? status = ClubBO.Instance.GetClubMemberStatus(clubID.Value, MyUserID);

			if (status == null || status.Value == ClubMemberStatus.Invited || status.Value == ClubMemberStatus.WaitForApprove)
				ShowError("不是群组成员");

			if (_Request.IsClick("send"))
			{
				using (ErrorScope es = new ErrorScope())
				{
					MessageDisplay md = CreateMessageDisplay();

					string[] ids = _Request.Get("uid", Method.Post, string.Empty).Split(',');

					if (ids.Length == 1 && ids[0] == string.Empty)
						md.AddError("请至少选择一个好友");
					else
					{
						int[] userIDs = new int[ids.Length];

						for (int i = 0; i < ids.Length; i++)
						{
							userIDs[i] = int.Parse(ids[i]);
						}

						if (ClubBO.Instance.InviteClubMembers(My, clubID.Value, userIDs))
						{
                            BbsRouter.JumpToCurrentUrl();// (BbsRouter.GetCurrentUrl());
						}
						else
						{
							es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
							{
								md.AddError(error);
							});
						}
					}
				}
			}

			m_GroupID = _Request.Get<int>("groupid", Method.Get);
			m_PageNumber = _Request.Get<int>("page", Method.Get, 1);
			m_PageSize = MaxLabs.bbsMax.Consts.DefaultPageSize;
			
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
		}
		else
		{
			ShowError("群组不存在");
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

	protected int? GroupID
	{
		get { return m_GroupID; }
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


}