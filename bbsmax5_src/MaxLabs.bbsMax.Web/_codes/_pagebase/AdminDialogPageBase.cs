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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web
{
    public class AdminDialogPageBase:DialogPageBase
    {
        protected override bool NeedCheckForumClosed
        {
            get { return false; }
        }

        //#if DEBUG
        protected virtual BackendPermissions.Action BackedPermissionAction { get { return BackendPermissions.Action.None; } }
        //#else
        //        protected abstract BackendPermissions.Action BackedPermissionAction { get; }
        //#endif

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (BackedPermissionAction != BackendPermissions.Action.None && AllSettings.Current.BackendPermissions.Can(My, BackedPermissionAction) == false)
            {
                ShowError("您没有进入本管理页面的权限");
                return;
            }
        }


        protected override void OnCheckAdminLoginFailed()
        {
            if (_Request.Get("isdialog", Method.Get) == "1")
                Display("~/max-dialogs/console-login.aspx", new NameObjectCollection("isdialog", "1"));

            else
                base.OnCheckAdminLoginFailed();
        }

        protected override bool NeedAdminLogin
        {
            get { return true; }
        }

        protected override bool NeedLogin
        {
            get { return true; }
        }


    }
}