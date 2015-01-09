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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using System.IO;
using System.Text;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class manage_realnameattach : AdminDialogPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get { return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Manage_NameCheck; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bool isFaceFile = _Request.Get<bool>("face", Method.Get, false);
            int userID = _Request.Get<int>("userid", Method.Get, 0);
            AuthenticUser user = UserBO.Instance.GetAuthenticUserInfo(My, userID);

            if (user != null)
            {
                string filePath;

                if (isFaceFile)
                    filePath = user.IDCardFileFace;

                else
                    filePath = user.IDCardFileBack;

                if (user.HasIDCardFileFace)
                {

                    string path = Globals.GetPath(SystemDirecotry.Root, filePath);

                    if (File.Exists(path))
                    {
                        string filename = user.Realname + ".jpg";
                        if (Request.Headers["Accept-Charset"] == null)
                            filename = HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(filename)).Replace("+", "%20");

                        FileInfo file = new FileInfo(path);
                        Response.Clear();
                        Response.ClearHeaders();
                        Response.Buffer = false;
                        Response.ContentType = "application/octet-stream";
                        //Response.AppendHeader("Content-Disposition", "inline;filename=" + HttpUtility.UrlEncode(user.Realname + ".jpg", System.Text.Encoding.UTF8));
                        Response.AppendHeader("Content-Disposition", "inline;filename=" + filename);
                        Response.AppendHeader("Content-Length", file.Length.ToString());
                        Response.TransmitFile(filePath);
                        Response.Flush();
                        return;
                    }
                    else
                    {
                        Response.ClearContent();
                        Response.Write("文件不存在：" + filePath);
                        Response.End();
                    }
                }
            }
            else
            {
                Response.ClearContent();
                Response.Write("用户不存在");
                Response.End();
            }
        }
    }
}