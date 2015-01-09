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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_bannedword : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_BannedWord; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SaveSetting<ContentKeywordSettings>("savesetting");
        }

        protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
        {
            bool result = base.SetSettingItemValue(setting, property);
            if (property.Name == "BannedKeywords")
            {
                ContentKeywordSettings keywordSettings = (ContentKeywordSettings)setting;
                keywordSettings.BannedKeywords.SetValue(SecurityUtil.Base64Decode(keywordSettings.BannedKeywords.GetValue()));
            }
            else if (property.Name == "ReplaceKeywords")
            {
                ContentKeywordSettings keywordSettings = (ContentKeywordSettings)setting;
                keywordSettings.ReplaceKeywords.SetValue(SecurityUtil.Base64Decode(keywordSettings.ReplaceKeywords.GetValue()));
            }
            else if (property.Name == "ApprovedKeywords")
            {
                ContentKeywordSettings keywordSettings = (ContentKeywordSettings)setting;
                keywordSettings.ApprovedKeywords.SetValue(SecurityUtil.Base64Decode(keywordSettings.ApprovedKeywords.GetValue()));
            }
            return result;
        }

        protected string BannedKeywords
        {
            get { return SecurityUtil.Base64Encode(ContentKeywordSettings.BannedKeywords.GetValue()); }
        }

        protected string ReplaceKeywords
        {
            get { return SecurityUtil.Base64Encode(ContentKeywordSettings.ReplaceKeywords.GetValue()); }
        }

        protected string ApprovedKeywords
        {
            get { return SecurityUtil.Base64Encode(ContentKeywordSettings.ApprovedKeywords.GetValue()); }
        }
    }
}