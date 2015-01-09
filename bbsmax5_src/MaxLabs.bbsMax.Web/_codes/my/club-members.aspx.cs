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

public partial class max_pages_icenter_club_members : CenterPageBase
{

    protected override string PageName
    {
        get
        {
            return "setting";
        }
    }
	protected void Page_Load(object sender, EventArgs e)
	{
		int? clubID = _Request.Get<int>("id");

		if (clubID.HasValue)
		{
			if (_Request.IsClick("update"))
			{
				using (ErrorScope es = new ErrorScope())
				{
					MessageDisplay md = CreateMessageDisplay();

					string[] ids = _Request.Get("MemberIDs", Method.Post, string.Empty).Split(',');

					if (ids.Length == 1 && ids[0] == string.Empty)
						md.AddError("请至少选择一个用户");
					else
					{
						int[] userIDs = new int[ids.Length];

						for (int i = 0; i < ids.Length; i++)
						{
							userIDs[i] = int.Parse(ids[i]);
						}

						if (_Request.Get("SetStatus", Method.Post) == "Delete")
						{
							ClubBO.Instance.RemoveClubMembers(My, clubID.Value, userIDs);
						}
						else
						{
							ClubMemberStatus? setStatus = _Request.Get<ClubMemberStatus>("SetStatus", Method.Post, ClubMemberStatus.Normal);

							if (setStatus != null)
							{
								if (ClubBO.Instance.UpdateClubMemberStatus(My, clubID.Value, userIDs, setStatus.Value))
								{
									BbsRouter.JumpToCurrentUrl();//.JumpTo(BbsRouter.GetCurrentUrl());
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
				}
			}

			ClubMemberStatus status = _Request.Get<ClubMemberStatus>("status", ClubMemberStatus.Normal);
			int page = _Request.Get<int>("page", 1);
			int pageSize = 30;

			m_ClubMemberList = ClubBO.Instance.GetClubMembersForEdit(My, clubID.Value, status, page, pageSize);

			if (m_ClubMemberList == null)
			{
				ShowError("群组不存在或没权限管理");
			}

			m_TotalClubMemberCount = m_ClubMemberList.TotalRecords;

			UserBO.Instance.WaitForFillSimpleUsers<ClubMember>(m_ClubMemberList);
		}
		else
		{
			ShowError("群组不存在");
		}
	}

	private ClubMemberCollection m_ClubMemberList;

	public ClubMemberCollection ClubMemberList
	{
		get { return m_ClubMemberList; }
	}

	private int m_TotalClubMemberCount;

	public int TotalClubmemberCount
	{
		get { return m_TotalClubMemberCount; }
		set { m_TotalClubMemberCount = value; }
	}
}