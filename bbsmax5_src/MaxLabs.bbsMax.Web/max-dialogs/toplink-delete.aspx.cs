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
using System.Collections.Generic;

using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class toplink_delete : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_TopLinks; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("delete"))
            {
                DeleteLink();
            }
        }


        protected void DeleteLink()
        {
            int linkid = _Request.Get<int>("linkid", Method.Get, 0);
            {
                if (linkid != 0)
                {
                    TopLinkSettings settings = new TopLinkSettings();
                    settings.Links = new TopLinkCollection();
                    settings.MaxID = AllSettings.Current.TopLinkSettings.MaxID;

                    foreach (TopLink link in AllSettings.Current.TopLinkSettings.Links)
                    {
                        if (link.LinkID != linkid || link.LinkID < 0)
                            settings.Links.Add(link);
                    }
                    SettingManager.SaveSettings(settings);

                    Return(linkid);
                }
            }
        }
    }
}