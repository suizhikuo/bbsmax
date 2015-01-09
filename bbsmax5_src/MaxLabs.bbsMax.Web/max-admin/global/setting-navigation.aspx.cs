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
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MaxLabs.bbsMax.Web.max_pages.admin;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_navigation : AdminPageBase
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
            if (_Request.IsClick("save"))
                Save();
        }

        private bool? m_IsTopLink;
        protected bool IsTopLink
        {
            get
            {
                if (m_IsTopLink == null)
                {
                    if (_Request.Get<int>("istoplink", Method.Get, 0) == 1)
                        m_IsTopLink = true;
                    else
                        m_IsTopLink = false;
                }
                return m_IsTopLink.Value;
            }
        }

        private NavigationItemCollection m_Items;
        protected NavigationItemCollection NvigationItems
        {
            get
            {
                if (m_Items == null)
                {
                    m_Items = new NavigationItemCollection();
                    NavigationItemCollection temp;
                    if(IsTopLink)
                        temp = AllSettings.Current.TopLinkSettings2.Navigations;
                    else
                        temp = AllSettings.Current.NavigationSettings.Navigations;

                    foreach (NavigationItem item in temp)
                    {
                        if (item.ParentID == 0)
                        {
                            m_Items.Add(item);
                            NavigationItemCollection childs;
                            if(IsTopLink)
                                childs = AllSettings.Current.TopLinkSettings2.GetChildItems(item.ID);
                            else
                                childs = AllSettings.Current.NavigationSettings.GetChildItems(item.ID);
                            m_Items.AddRange(childs);
                        }
                    }
                }

                return m_Items;
            }
        }

        private void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int[] ids = _Request.GetList<int>("id", Method.Post, new int[] { });

            NavigationSettingsBase settings;
            if(IsTopLink)
                settings = SettingManager.CloneSetttings<TopLinkSettings2>(AllSettings.Current.TopLinkSettings2);
            else
                settings = SettingManager.CloneSetttings<NavigationSettings>(AllSettings.Current.NavigationSettings);

            NavigationItemCollection items = new NavigationItemCollection();

            int i = 0;
            foreach (int id in ids)
            {
                NavigationItem item = settings.Navigations.GetValue(id);
                if (item != null)
                {
                    item.SortOrder = _Request.Get<int>("parent_sortorder_" + id, Method.Post, 0);
                    item.Name = _Request.Get("parent_name_" + id);
                    if (item.Type == NavigationType.Custom)
                    {
                        item.UrlInfo = _Request.Get("parent_url_" + id, Method.Post, string.Empty);
                    }
                    item.NewWindow = _Request.IsChecked("parent_newwindow_" + id, Method.Post, false);
                    item.OnlyLoginCanSee = _Request.IsChecked("parent_logincansee_" + id, Method.Post, false);
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        msgDisplay.AddError("", i, "名称不能为空");
                    }
                    if(string.IsNullOrEmpty(item.UrlInfo))
                    {
                        msgDisplay.AddError("", i, "链接地址不能为空");
                    }
                    item.IsTopLink = IsTopLink;
                    i++;

                    items.Add(item);
                }

            }

            if(msgDisplay.HasAnyError())
                return;

            settings.Navigations = items;
            try
            {
                SettingManager.SaveSettings(settings);
                if (IsTopLink)
                    AllSettings.Current.TopLinkSettings2.ClearCache();
                else
                    AllSettings.Current.NavigationSettings.ClearCache();
            }
            catch(Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }
       
    }
}