//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class image_browser_upload : AdminDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Dir == null)//???
            {
                Response.Clear();
                Response.End();
                return;
            }
            else
            {
                bool hasPermission = false;
                switch (Dir.Value)
                {
                    case SystemDirecotry.Assets:
                        //?????
                        break;
                    case SystemDirecotry.Assets_ForumLogo:
                        hasPermission = checkPermission(BackendPermissions.Action.Manage_Forum);
                        break;
                    case SystemDirecotry.Assets_Icon:
                        hasPermission = checkPermission(BackendPermissions.Action.Manage_Feed_Template);
                        break;
                    case SystemDirecotry.Assets_Judgement:
                        hasPermission = checkPermission(BackendPermissions.Action.Setting_Judgement);
                        break;
                    case SystemDirecotry.Assets_LinkIcon:
                        hasPermission = checkPermission(BackendPermissions.Action.Setting_Links);
                        break;
                    case SystemDirecotry.Assets_OnlineIcon:
                        hasPermission = checkPermission(BackendPermissions.Action.Setting_OnlineList);
                        break;
                    case SystemDirecotry.Assets_RoleIcon:

                        if (AllSettings.Current.BackendPermissions.Can(MyUserID, BackendPermissions.Action.Setting_Roles_Level)
                            || AllSettings.Current.BackendPermissions.Can(MyUserID, BackendPermissions.Action.Setting_Roles_Other)
                            || AllSettings.Current.BackendPermissions.HasPermissionForSomeone(MyUserID, BackendPermissions.ActionWithTarget.Setting_Roles_Manager))
                            hasPermission = true;
                        break;

                    case SystemDirecotry.Assets_PostIcon:
                        hasPermission = checkPermission(BackendPermissions.Action.Setting_PostIcon);
                        break;
                    case SystemDirecotry.Assets_MissionIcon:
                        hasPermission = checkPermission(BackendPermissions.Action.Manage_Mission);
                        break;
                    case SystemDirecotry.Assets_MedalIcon:
                        hasPermission = checkPermission(BackendPermissions.Action.Setting_Medals);
                        break;

                    case SystemDirecotry.Assets_AdvertIcon:
                        hasPermission = checkPermission(BackendPermissions.Action.Setting_A);
                        break;

                    case SystemDirecotry.Assets_PropIcons:
                        hasPermission = checkPermission(BackendPermissions.Action.Manage_Prop);
                        break;

                    default:
                        break;

                }
                if (hasPermission == false)
                {
                    Response.End();
                    return;
                }
            }

            if (_Request.IsClick("upload"))
            {
                Upload();
            }
        }

        private SystemDirecotry? m_dir = null;
        private SystemDirecotry? Dir
        {
            get
            {
                if (m_dir == null)
                {
                    SystemDirecotry? dir = null;
                    dir = _Request.Get<SystemDirecotry>("dir", Method.Get);

                    if (dir == null)
                    {
                        return null;
                    }
                    else
                    {
                        //文件夹限制
                        if (
                               dir.Value == SystemDirecotry.Assets
                            || dir.Value == SystemDirecotry.Assets_Judgement
                            || dir.Value == SystemDirecotry.Assets_LinkIcon
                            || dir.Value == SystemDirecotry.Assets_OnlineIcon
                            || dir.Value == SystemDirecotry.Assets_RoleIcon
                            || dir.Value == SystemDirecotry.Assets_PostIcon
                            || dir.Value == SystemDirecotry.Assets_ForumLogo
                            || dir.Value == SystemDirecotry.Assets_MissionIcon
                            || dir.Value == SystemDirecotry.Assets_MedalIcon
                            || dir.Value == SystemDirecotry.Assets_Icon
                            || dir.Value == SystemDirecotry.Assets_AdvertIcon
                            || dir.Value == SystemDirecotry.Assets_PropIcons
                           )
                            m_dir = dir;
                        else
                            return null;
                    }
                }
                return m_dir;
            }
        }

        private bool checkPermission(BackendPermissions.Action action)
        {
            if (false == AllSettings.Current.BackendPermissions.Can(MyUserID, action))
            {
                ShowError("您没有进入本管理页面的权限");
                return false;
            }
            return true;
        }

        private string GetUrl(string fileName)
        {
            if (Dir == null) return "";
            return UrlUtil.JoinUrl(Globals.GetVirtualPath(Dir.Value), HttpUtility.UrlEncode(fileName));
        }

        private string GetRelativeUrl(string filename)
        {
            if (this.Dir == null) return "";
            return UrlUtil.JoinUrl(Globals.GetRelativeUrl(this.Dir.Value), filename);
        }

        string m_FileJson="null";
        protected string FileJson
        {
            get
            {
                return m_FileJson;
            }
        }

        protected bool UploadSuccess
        {
            get;
            set;
        }


        private void Upload()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            if (Dir == null) return;

            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                bool isAllowFileType = false;
                string[] allowFileTypes = new string[] { ".jpg", ".gif", ".png", ".bmp", ".jpeg" };
                HttpPostedFile file = Request.Files[0];

                foreach (string s in allowFileTypes)
                {
                    if (file.FileName.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                    {
                        isAllowFileType = true;
                        break;
                    }
                }

                if (isAllowFileType)
                {
                    string filePath = Globals.GetPath(Dir.Value);

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    string fileName = Path.GetFileName(file.FileName);
                    filePath = IOUtil.JoinPath(filePath, fileName);
                    if (File.Exists(filePath))
                        IOUtil.DeleteFile(filePath);
                    try
                    {
                        file.SaveAs(filePath);
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddError("写入文件出错，错误信息： " + ex.Message + "  可能的情况是， 对该目录：" + Globals.GetPath(Dir.Value) + "  的IO操作被限制。");
                        return;
                    }
                    UploadSuccess = true;
                    this.m_FileJson = string.Format("{{url:'{0}',value:'{1}',filename:'{2}'}}", StringUtil.ToJavaScriptString( GetUrl(fileName)),  StringUtil.ToJavaScriptString( GetRelativeUrl(fileName)),StringUtil.ToJavaScriptString(fileName));
                }
                else
                {
                    //TODO Throw FiletypeInvalid
                }
            }
        }
    }
}