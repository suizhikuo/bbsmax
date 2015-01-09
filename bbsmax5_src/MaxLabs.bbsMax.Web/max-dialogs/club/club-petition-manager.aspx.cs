//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
	public partial class max_dialogs_club_petition_manager : DialogPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			int? clubID = _Request.Get<int>("id");

			if (clubID != null)
			{
				Club club = ClubBO.Instance.GetClub(clubID.Value);

				ClubMemberStatus? status = ClubBO.Instance.GetClubMemberStatus(clubID.Value, MyUserID);

				if (status == null)
				{
					ShowError("您不是该群组成员，不能申请成为管理员");
					return;
				}
				else if (status.Value != ClubMemberStatus.Normal)
				{
					ShowError("您当前的群组成员身份不能申请成为管理员");
					return;
				}

				if (club != null)
				{
					if (_Request.IsClick("join"))
					{
						ClubBO.Instance.PetitionClubManager(club, My);

						m_DisplayMessage = true;
						m_Message = "已经发生申请，请等待群主审核。";
					}
				}
				else
				{
					ShowError("群组不存在");
				}
			}
			else
			{
				ShowError("缺少必要参数");
			}
		}

		private bool m_DisplayMessage;

		public bool DisplayMessage
		{
			get { return m_DisplayMessage; }
		}

		private string m_Message;

		public string Message
		{
			get { return m_Message; }
		}
	}
}