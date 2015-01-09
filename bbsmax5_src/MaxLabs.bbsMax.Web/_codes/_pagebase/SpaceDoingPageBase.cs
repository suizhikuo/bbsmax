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
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web
{
	public class SpaceDoingPageBase : SpacePageBase
	{
		protected override void OnLoadComplete(EventArgs e)
		{
			m_VisitorIsAdmin = DoingBO.Instance.ManagePermission.Can(My, BackendPermissions.ActionWithTarget.Manage_Doing, SpaceOwnerID);

			base.OnLoadComplete(e);
		}


		protected override bool EnableFunction
		{
			get { return EnableDoingFunction; }
		}

		private bool m_VisitorIsAdmin;

		protected override bool VisitorIsAdmin
		{
			get { return m_VisitorIsAdmin; }
		}

		protected override MaxLabs.bbsMax.Enums.SpacePrivacyType FunctionPrivacy
		{
			get { return SpaceOwner.DoingPrivacy; }
		}

		public override bool IsSelectedDoing
		{
			get
			{
				return true;
			}
		}
	}
}