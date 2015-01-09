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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;

using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine.Template;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class skin_enable : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Template; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("sure"))
            {
				string skinID = _Request.Get("skin");

				if (skinID == null)
					ShowError("缺少必要参数");

                Skin skin = TemplateManager.GetSkin(skinID, true);

				if (skin == null)
                    ShowError("您所操作的的皮肤不存在");

                if (_Request.Get("enable") == "1")
                {
                    AllSettings.Current.SkinSettings.EnableSkin(skinID);
                }
                else
                {
                    if (AllSettings.Current.SkinSettings.DefaultSkin != skin.SkinID)
                    {
                        AllSettings.Current.SkinSettings.DisableSkin(skinID);
                    }
                    else
                    {
                        ShowError("不可直接禁用当前的默认皮肤");
                    }
                }

                Return("id", skinID);
            }
        }

        public string Name
        {
            get { return _Request.Get("enable") == "1" ? "启用" : "禁用"; }
        }
    }
}