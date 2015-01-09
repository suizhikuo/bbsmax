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
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_roles_other : RoleSettingPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Roles_Other; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savesetting"))
                SaveSettings();
        }

        protected override RoleCollection RoleList
        {
            get
            {
                return m_RoleList == null ? RoleSettings.GetNormalRoles() : m_RoleList;
            }
            set
            {
                m_RoleList = value;
            }
        }

        protected Guid FullSiteBannedRoleID
        {
            get
            {
                return Role.FullSiteBannedUsers.RoleID;
            }
        }

        protected Guid ForumBannedUserID
        {
            get
            {
                return Role.ForumBannedUsers.RoleID;
            }
        }

        protected override Role CreateRole()
        {
            //MessageDisplay msgDisplay = new MessageDisplay.MessageItem();//TODO ???
            return Role.CreateNormalRole();
        }
	}
}