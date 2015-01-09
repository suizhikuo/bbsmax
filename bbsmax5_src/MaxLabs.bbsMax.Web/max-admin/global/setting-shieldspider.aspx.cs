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
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using System.Collections.Generic;
using System.Reflection;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_shieldspider : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return BackendPermissions.Action.Setting_ShieldSpider;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savesetting"))
            {
                SaveSetting<ShieldSpiderSettings>("savesetting");
            }
            
        }

        protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
        {
            if (property.Name == "BannedSpiders")
            {

                ShieldSpiderSettings spiderSettings = (ShieldSpiderSettings)setting;
                string bannedSpiderNamesString = _Request.Get("BannedSpiders", Method.Post);

                string[] bannedSpiders = StringUtil.Split(bannedSpiderNamesString);

                spiderSettings.BannedSpiders.SetBannedSpiders(bannedSpiders);

                return true;
            }
            else
                return base.SetSettingItemValue(setting, property);
        }

        protected bool IsSpiderBanned(SpiderType spiderType)
        {
            return AllSettings.Current.ShieldSpiderSettings.BannedSpiders.IsBanned(spiderType);
        }

        private SpiderType[] m_SpiderList = null;

        public SpiderType[] SpiderList
        {
            get
            {
                SpiderType[] spiderList = m_SpiderList;

                if (spiderList == null)
                {
                    FieldInfo[] fields = typeof(SpiderType).GetFields();

                    spiderList = new SpiderType[fields.Length - 2];

                    for (int i = 1; i < fields.Length - 1; i++)
                    {
                        spiderList[i - 1] = (SpiderType)i;
                    }

                    m_SpiderList = spiderList;
                }

                return spiderList;
            }
        }
     
    }
}