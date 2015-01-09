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
using System.IO;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class skin_import : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Template; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("sure"))
            {
                if (Request.Files.Count > 0)
                {
                    string path = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Skins), Path.GetFileName(Request.Files[0].FileName));

                    Request.Files[0].SaveAs(path);

                    TemplateManager.ImportSkin(path);

                    File.Delete(path);

                    Return("ID", 0);
                }
            }
        }
    }
}