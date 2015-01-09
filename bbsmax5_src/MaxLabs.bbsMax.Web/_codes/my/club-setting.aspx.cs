//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using MaxLabs.bbsMax.Web;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;

public partial class max_pages_icenter_club_setting : CenterPageBase
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
				string description = _Request.Get("description", MaxLabs.WebEngine.Method.Post, string.Empty);
				ClubJoinMethod joinMethod = _Request.Get<ClubJoinMethod>("JoinMethod", ClubJoinMethod.Freely);
				ClubAccessMode accessMode = _Request.Get<ClubAccessMode>("AccessMode", ClubAccessMode.Freely);
				bool isNeedManager = _Request.Get<bool>("IsNeedManager", true);

				using (ErrorScope es = new ErrorScope())
				{
					MessageDisplay md = CreateMessageDisplay();

					if (ClubBO.Instance.UpdateClub(My, clubID.Value, description, joinMethod, accessMode, isNeedManager))
					{
						BbsRouter.JumpTo("club/" + clubID + "/setting");
					}
					else
					{
						es.CatchError<ErrorInfo>(delegate(ErrorInfo error) {
							md.AddError(error);
						});
					}
				}
			}

			m_Club = ClubBO.Instance.GetClubForEdit(My, clubID.Value);

			if (m_Club == null)
			{
				ShowError("群组不存在或没权限管理");
			}
		}
		else
		{
			ShowError("群组不存在");
		}
	}

	private Club m_Club;

	public Club Club
	{
		get { return m_Club; }
	}
}