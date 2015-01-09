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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Errors;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class navigation : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                if (IsTopLink)
                    return BackendPermissions.Action.Setting_TopLinks;
                else
                    return BackendPermissions.Action.Setting_Navigation;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (false == AllSettings.Current.BackendPermissions.Can(My,BackendPermissions.Action.Setting_Navigation))
            //{
            //    ShowError("您没有权限添加导航菜单");
            //    return;
            //}

            if (_Request.IsClick("save"))
            {
                Save();
            }
         
        }

        private bool? m_IsTopLink;
        protected bool IsTopLink
        {
            get
            {
                if (m_IsTopLink == null)
                {
                    m_IsTopLink = _Request.Get("IsTopLink", Method.Get, "false").ToLower() == "true";
                }
                return m_IsTopLink.Value;
            }
        }

        protected NavigationItemCollection NvigationItems
        {
            get
            {
                if(IsTopLink)
                    return AllSettings.Current.TopLinkSettings2.Navigations;
                else
                    return AllSettings.Current.NavigationSettings.Navigations;
            }
        }

        private int? m_ParentID;
        protected int ParentID
        {
            get
            {
                if (m_ParentID == null)
                {
                    m_ParentID = _Request.Get<int>("id", Method.Get, 0);
                }
                return m_ParentID.Value;
            }
        }

        private Dictionary<string, string> m_InternalLinks;
        protected Dictionary<string, string> InternalLinks
        {
            get
            {
                if (m_InternalLinks == null)
                {
                    m_InternalLinks = NavigationSettingsBase.InternalLinks();
                }
                return m_InternalLinks;
            }
        }

        protected Dictionary<string,string>.KeyCollection InternalLinkKeys
        {
            get
            {
                return InternalLinks.Keys;
            }
        }

        protected string GetInternalName(string key)
        {
            return InternalLinks[key];
        }

        private ForumCollection m_Forums;
        protected ForumCollection Forums
        {
            get
            {
                if (m_Forums == null)
                {
                    GetForums();
                }
                return m_Forums;
            }
        }

        private List<string> m_ForumSeparators;
        protected List<string> ForumSeparators
        {
            get
            {
                if (m_ForumSeparators == null)
                {
                    GetForums();
                }
                return m_ForumSeparators;
            }
        }

        private void GetForums()
        {
            ForumBO.Instance.GetTreeForums("<span></span>", delegate(Forum forum) { return true; }, out m_Forums, out m_ForumSeparators);
        }

        protected NavigationItemCollection TempNavigations = new NavigationItemCollection();


        private void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("name", "url");
            int parentid = _Request.Get<int>("parent", Method.Post, 0);
            int[] newids = _Request.GetList<int>("newid", Method.Post, new int[] { });

            int maxID;
            if (IsTopLink)
                maxID = AllSettings.Current.TopLinkSettings2.MaxID;
            else
                maxID  = AllSettings.Current.NavigationSettings.MaxID;
            NavigationItemCollection items = new NavigationItemCollection();
            int i = 0;

            int[] oldids = _Request.GetList<int>("old_id", Method.Post, new int[] { });

            foreach (int id in oldids)
            {
                NavigationItem item = new NavigationItem();
                item.ParentID = parentid;
                item.SortOrder = _Request.Get<int>("old_sortorder_" + id, Method.Post, 0);
                item.Type = (NavigationType)_Request.Get<int>("old_type_" + id, Method.Post, 0);
                item.Name = _Request.Get("old_name_" + id, Method.Post, "");
                if (item.Type == NavigationType.Custom)
                    item.UrlInfo = _Request.Get("old_url_" + id, Method.Post, "");
                else
                    item.UrlInfo = _Request.Get("old_urlinfo_" + id, Method.Post, "");

                item.NewWindow = _Request.IsChecked("old_newwindow_" + id, Method.Post, false);
                item.OnlyLoginCanSee = _Request.IsChecked("old_logincansee_" + id, Method.Post, false);

                TempNavigations.Add(item);
                i++;
                if (string.IsNullOrEmpty(item.Name) && string.IsNullOrEmpty(item.UrlInfo))
                {
                    continue;
                }
                else
                {
                    if (string.IsNullOrEmpty(item.Name))
                        msgDisplay.AddError("name", i-1, "名称不能为空");
                    if (string.IsNullOrEmpty(item.UrlInfo))
                        msgDisplay.AddError("url", i-1, "地址不能为空");
                }
                maxID++;
                item.ID = maxID;
                item.IsTopLink = IsTopLink;
                items.Add(item);
            }


            foreach (int newid in newids)
            {
                NavigationItem item = new NavigationItem();
                
                item.ParentID = parentid;
                item.SortOrder = _Request.Get<int>("sortorder_" + newid, Method.Post, 0);
                item.Type = (NavigationType)_Request.Get<int>("type_" + newid, Method.Post, 0);
                item.Name = _Request.Get("name_" + newid, Method.Post, "");
                if (item.Type == NavigationType.Custom)
                    item.UrlInfo = _Request.Get("url_" + newid, Method.Post, "");
                else
                    item.UrlInfo = _Request.Get("urlinfo_" + newid, Method.Post, "");

                item.NewWindow = _Request.IsChecked("newwindow_" + newid, Method.Post, false);
                item.OnlyLoginCanSee = _Request.IsChecked("logincansee_" + newid, Method.Post, false);

                TempNavigations.Add(item);
                i++;
                if (string.IsNullOrEmpty(item.Name) && string.IsNullOrEmpty(item.UrlInfo))
                {
                    continue;
                }
                else
                {
                    if (string.IsNullOrEmpty(item.Name))
                        msgDisplay.AddError("name", i-1, "名称不能为空");
                    if (string.IsNullOrEmpty(item.UrlInfo))
                        msgDisplay.AddError("url", i-1, "地址不能为空");
                }

                maxID++;
                item.ID = maxID;
                item.IsTopLink = IsTopLink;
                items.Add(item);
            }

            if (msgDisplay.HasAnyError())
                return;

            NavigationSettingsBase settings;
            if (IsTopLink)
                settings = SettingManager.CloneSetttings<TopLinkSettings2>(AllSettings.Current.TopLinkSettings2);
            else
                settings = SettingManager.CloneSetttings<NavigationSettings>(AllSettings.Current.NavigationSettings);

            settings.MaxID = maxID;
            settings.Navigations.AddRange(items);

            try
            {
                SettingManager.SaveSettings(settings);
                if (IsTopLink)
                    AllSettings.Current.TopLinkSettings2.ClearCache();
                else
                    AllSettings.Current.NavigationSettings.ClearCache();
                Return(true);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }
    }
}