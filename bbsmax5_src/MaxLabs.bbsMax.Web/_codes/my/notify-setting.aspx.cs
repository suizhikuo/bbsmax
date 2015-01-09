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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Web;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;

public partial class max_pages_icenter_notify_setting : CenterPageBase
{
    protected override string PageTitle
    {
        get { return "通知接收设置 - " + base.PageTitle; }
    }

    protected override string PageName
    {
        get { return "notify-setting"; }
    }

    protected override string NavigationKey
    {
        get { return "notify-setting"; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        //AddNavigationItem("设置中心", BbsRouter.GetUrl("my/setting"));
        AddNavigationItem("通知接收设置");

        if (_Request.IsClick("savesetting"))
        {
            SaveSetting();
        }
    }

    protected string GetNotifyName(int id)
    {
        return NotifyBO.AllNotifyTypes[id].TypeName;
    }

    private void SaveSetting()
    {
        UserNotifySetting setting = new UserNotifySetting(string.Empty);

        foreach (NotifySettingItem item in setting.AllNotify)
        {
            if (item.OpenState == NotifyState.DefaultClose || item.OpenState == NotifyState.DefaultOpen)
            {
                item.OpenState = _Request.Get<NotifyState>(item.NotifyType.ToString(), Method.Post, NotifyState.DefaultClose);
            }
        }

        UserBO.Instance.SetNotifySetting(MyUserID, setting);

        MessageDisplay msgDispaly = CreateMessageDisplay();
    }


    protected NotifySettingItemCollection m_NotifyTypeList;

    protected NotifySettingItemCollection NotifyTypeList
    {
        get
        {
            if (m_NotifyTypeList == null)
            {
                m_NotifyTypeList = new NotifySettingItemCollection();
                UserNotifySetting myNotifySettings;
                myNotifySettings = UserBO.Instance.GetNotifySetting(MyUserID);

                foreach (NotifySettingItem item in myNotifySettings.AllNotify)
                {
                    if (item.OpenState == NotifyState.DefaultOpen || item.OpenState == NotifyState.AlwaysOpen)
                    {
                        m_NotifyTypeList.Add(item);
                    }
                }
            }
            return m_NotifyTypeList;
        }
    }


    private UserNotifySetting m_setting;
    protected UserNotifySetting Setting
    {
        get
        {
            if (m_setting == null)
            {
             
                m_setting = My.NotifySetting;
            }
            return m_setting;
        }
    }

    protected NotifySettingItemCollection NotifyList
    {
        get
        {
            return Setting.AllNotify;
        }
    }
}