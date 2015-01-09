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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
	public partial class max_dialogs_club_acceptinvite : DialogPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			int? clubID = _Request.Get<int>("id");
			int? notifyID = _Request.Get<int>("notifyid");

			if (clubID != null && notifyID != null)
			{
				Club club = ClubBO.Instance.GetClub(clubID.Value);

				if (club != null)
				{
					m_ClubName = club.Name;

					if (_Request.IsClick("join"))
					{
						ClubBO.Instance.AcceptClubInvite(club, My);

						NotifyBO.Instance.DeleteNotify(My, notifyID.Value);

						m_DisplayMessage = true;
						m_Message = "您已成功的加入群组“<a href=\"" + BbsRouter.GetUrl("club/" + clubID) + "\" target=\"_blank\">" + club.Name + "</a>”。";
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

		private string m_ClubName;

		public string ClubName
		{
			get { return m_ClubName; }
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