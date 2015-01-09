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
    public partial class skin_export : AdminDialogPageBase
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
                    ShowError("缺少必要参数");

                string path = TemplateManager.ExportSkin(skinID);

                if (path != null)
                {
                    m_DownloadPath = Globals.GetVirtualPath(SystemDirecotry.Root, path.Substring(Globals.GetPath(SystemDirecotry.Root).Length));

                    m_ExportDone = true;

                    m_FileName = Path.GetFileName(path);
                }
            }
            else if(_Request.IsClick("delete"))
            {
                string fileName = _Request.Get("FileName", Method.Post);

                string path = Globals.GetPath(SystemDirecotry.Skins, Path.GetFileNameWithoutExtension(fileName) + ".zip");

                try
                {
                    File.Delete(path);
                }
                catch { }

                Return("Done", 1);
            }
        }

        private bool m_ExportDone;

        public bool ExportDone
        {
            get { return m_ExportDone; }
        }

        private string m_DownloadPath;

        public string DownloadPath
        {
            get { return m_DownloadPath; }
        }

        private string m_FileName;

        public string FileName
        {
            get { return m_FileName; }
        }
    }
}