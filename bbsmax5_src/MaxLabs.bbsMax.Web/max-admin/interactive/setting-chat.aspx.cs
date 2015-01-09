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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class max_admin_setting_chat : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Chat; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SaveSetting<ChatSettings>("savesetting");
        }

        protected List<ChatSettings.SoundFileItem> SoundList
        {
            get
            {
                return AllSettings.Current.ChatSettings.SoundList;
            }
        }

        protected string CurrentSound
        {
            get
            {
                return ChatSettings.MessageSoundSrc;
            }
        }

        protected bool EnableUserEmoticon
        {
            get
            {
                return ChatSettings.EnableUserEmoticon;
            }
        }

        protected bool EnableDefaultEmoticon
        {
            get
            {
                return ChatSettings.EnableDefaultEmoticon;
            }
        }


        protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
        {
            if (property.Name == "EnableDefaultEmoticon" || property.Name == "EnableUserEmoticon")
            {
                property.SetValue(setting, _Request.Get<bool>(property.Name, MaxLabs.WebEngine.Method.Post, false), null);
                return true;
            }
            else if (property.Name == "SaveMessageDays")
            {
                property.SetValue(setting, _Request.Get<int>("SaveDays", 0), null);
                return true;

            }
            else if (property.Name == "SaveMessageRows")
            {
                property.SetValue(setting, _Request.Get<int>("SaveRows", 0), null);
                return true;
            }
            else if (property.Name == "DataClearMode")
            {
                if (_Request.Get<bool>("IsCombin",false))
                {
                    property.SetValue(setting, JobDataClearMode.CombinMode, null);
                    return true;
                }
            }

            return base.SetSettingItemValue(setting, property);
        }


        public JobDataClearMode DataClearMode
        {
            get
            {
                return ChatSettings.DataClearMode;
            }
            
        }


        public int SaveMessageRows
        {
            get
            {
                return ChatSettings.SaveMessageRows;
            }
        }

        protected int SaveMessageDays
        {
            get
            {
                return ChatSettings.SaveMessageDays;
            }
        }

        protected int ClearMassgeExecuteTime
        {
            get
            {
                return ChatSettings.ClearMassgeExecuteTime;
            }
        }
    }

}