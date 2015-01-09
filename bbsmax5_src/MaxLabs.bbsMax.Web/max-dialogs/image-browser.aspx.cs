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

using System.IO;
using MaxLabs.WebEngine;
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class image_browser :AdminDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Dir == null)//???
            {
                ShowError(new InvalidParamError("Dir"));
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
                    return;
            }

            if (_Request.IsClick("delete"))
            {
                DeleteFile();
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

        protected string DirName
        {
            get
            {
                if (Dir != null)
                {
                    switch (Dir.Value)
                    {
                        case SystemDirecotry.Assets_Judgement:
                            return "帖子鉴定";
                        case SystemDirecotry.Assets_LinkIcon:
                            return "友情链接";
                        case SystemDirecotry.Assets_OnlineIcon:
                            return "在线图标";
                        case SystemDirecotry.Assets_PostIcon:
                            return "帖子图标";
                        case SystemDirecotry.Assets_RoleIcon:
                            return "用户组图标";
                        case SystemDirecotry.Assets_AdvertIcon:
                            return "广告图片";
                        case SystemDirecotry.Assets_PropIcons:
                            return "道具图标";
                    }
                }
                return "";
            }
        }

        private void DeleteFile()
        {
            if (Dir != null)
            {
                string[] fileName=StringUtil.Split( _Request.Get( "selected", Method.Post));
                if (fileName != null && fileName.Length > 0)
                {
                    foreach (string s in fileName)
                    {
                        if(IOUtil.FileIsInDirectory(Globals.GetPath(Dir.Value),s))
                            IOUtil.DeleteFile(s);
                    }
                }
            }
        }

        protected override bool EnableClientBuffer
        {
            get
            {
                return false;
            }
        }

        protected string GetRelativeUrl(string filename)
        {
            if (this.Dir == null) return "";
            return UrlUtil.JoinUrl( Globals.GetRelativeUrl(this.Dir .Value),filename); 
        }

        protected string GetUrl( string fileName)
        {
            if (Dir == null) return "";
            return UrlUtil.JoinUrl(Globals.GetVirtualPath(Dir.Value), HttpUtility.UrlEncode(fileName));
        }

        private SystemDirecotry? m_dir=null;
        private SystemDirecotry? Dir
        {
            get
            {
                if (m_dir == null)
                {
                    SystemDirecotry? dir =null;
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
                            ||dir.Value == SystemDirecotry.Assets_AdvertIcon
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

        protected IList< FileInfo> Files
        {
            get
            {
                if (Dir == null)
                {
                    return null;
                }
                else
                {
                    if (!Directory.Exists(Globals.GetPath(Dir.Value)))
                        Directory.CreateDirectory(Globals.GetPath(Dir.Value));
                    return IOUtil.GetImagFiles(Dir.Value);
                }
            }
        }
    }
}