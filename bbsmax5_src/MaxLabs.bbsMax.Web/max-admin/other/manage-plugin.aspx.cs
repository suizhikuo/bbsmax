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
using MaxLabs.WebEngine.Plugin;
using MaxLabs.bbsMax.Web;

public partial class max_admin_manage_plugin : AdminPageBase
{
    protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
    {
        get { return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Manage_Plugin; }
    }

	protected void Page_Load(object sender, EventArgs e)
	{
        if (_Request.IsClick("Disable"))
        {
            string[] names = _Request.Get("PluginNames", MaxLabs.WebEngine.Method.Post, string.Empty).Split(',');

            if (names[0] != string.Empty)
            {
                foreach (string name in names)
                {
                    PluginManager.DisablePlugin(name);
                }
            }

            Response.Redirect(Request.RawUrl);
        }
        else if(_Request.IsClick("Enable"))
        {
            string[] names = _Request.Get("PluginNames", MaxLabs.WebEngine.Method.Post, string.Empty).Split(',');

            if (names[0] != string.Empty)
            {
                foreach (string name in names)
                {
                    PluginManager.EnablePlugin(name);
                }
            }

            Response.Redirect(Request.RawUrl);
        }
	}

    public PluginCollection PluginList
    {
        get { return PluginManager.Plugins; }
    }
}