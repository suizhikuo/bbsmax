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
using MaxLabs.bbsMax.Web;
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
using MaxLabs.WebEngine;

public partial class max_admin_setting_notify : AdminPageBase
{
    protected override BackendPermissions.Action BackedPermissionAction
    {
        get { return BackendPermissions.Action.Setting_Notify; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        SaveSetting<NotifySettings>("savesetting");
    }

    protected NotifyState GetState(int typeid)
    {
       return  AllSettings.Current.NotifySettings.GetNotifySystemState(typeid);
    }

    public NotifyTypeCollection NotifyTypeList
    {
        get
        {
            return  NotifyBO.AllNotifyTypes;
        }
    }

    public int ClearJobExecuteTime
    {
        get
        {
            return AllSettings.Current.NotifySettings.ClearJobExecuteTime;
        }
    }

    protected JobDataClearMode DataClearMode
    {
        get
        {
            return NotifySettings.DataClearMode;
        }
    }


    public int NotifySaveDays
    {
        get
        {
            return AllSettings.Current.NotifySettings.NotifySaveDays;
        }
    }
    protected int NotifySaveRows
    {
        get
        {
            return NotifySettings.NotifySaveRows;
        }
    }

    protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
    {
        if (property.Name == "AllNotify")
        {
            NotifySettingItemCollection items = new NotifySettingItemCollection();

            NotifySettingItem newItem;

            foreach ( NotifyType nt in NotifyBO.AllNotifyTypes)
            {
                newItem = new NotifySettingItem();

                newItem.NotifyType = nt.TypeID;                               
                newItem.OpenState = _Request.Get<NotifyState>( string.Format("{0}_OpenState",nt.TypeID), Method.Post, NotifyState.DefaultOpen);
                items.Add( newItem );
            }

            ((NotifySettings)setting).AllNotify = items;
            return true;
        }
        else if (property.Name == "NotifySaveDays")
        {
            property.SetValue(setting, _Request.Get<int>("saveDays", 0), null);
            return true;
        }
        else if (property.Name == "NotifySaveRows")
        {
            property.SetValue(setting, _Request.Get<int>("SaveRows", 0), null);
            return true;
        }
        else if (property.Name == "DataClearMode")
        {
            if (_Request.Get<bool>("IsCombin",false))
            {
                property.SetValue(setting, JobDataClearMode.CombinMode, null);
            }
        }

        return base.SetSettingItemValue(setting, property);
    }
}