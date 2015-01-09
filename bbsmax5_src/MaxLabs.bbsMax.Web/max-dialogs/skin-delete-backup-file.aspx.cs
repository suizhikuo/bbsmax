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

using System.IO;
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
    public partial class skin_delete_backup_file : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Template; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("delete"))
            {
                string skinID = _Request.Get("skin");
                string filePath = _Request.Get("file");
                string version = _Request.Get("version");

                if (skinID == null || filePath == null || version == null)
                    ShowError("缺少必要参数");

                Skin skin = TemplateManager.GetSkin(skinID, true);

                if (skin == null)
                    ShowError("缺少必要参数");

                //string skinRoot = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Skins), skinID);

                string backupFile = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Temp), "__模板编辑历史记录__", skinID, filePath, version + ".config");

                if (File.Exists(backupFile))
                    File.Delete(backupFile);

                Return("id", skinID);
            }
        }
    }
}