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
	public partial class max_dialogs_club_leave : DialogPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			int? clubID = _Request.Get<int>("id");

			if (clubID != null)
			{
				if (_Request.IsClick("leave"))
				{
					ClubBO.Instance.LeaveClub(MyUserID, clubID.Value);

					m_DisplayMessage = true;
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
	}
}