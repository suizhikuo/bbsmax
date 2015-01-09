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
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_bbs : AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Bbs; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int markCount = BbsSettings.ShowMarksCount;
            int postPageSize = BbsSettings.PostsPageSize;

            StickSortType oldStickSortType = BbsSettings.StickSortType;
            StickSortType oldGlobalStickSortType = BbsSettings.GloableStickSortType;

            if (SaveSetting<BbsSettings>("savesetting"))
            {
                if (BbsSettings.ShowMarksCount != markCount
                    || BbsSettings.PostsPageSize != postPageSize
                    || oldStickSortType != BbsSettings.StickSortType
                    || oldGlobalStickSortType != BbsSettings.GloableStickSortType)
                    ThreadCachePool.ClearAllCache();
            }
		}

        protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
        {
            MessageDisplay msgDisplay = MsgDisplayForSaveSettings;

            if (property.Name == "DisplaySignature")
            {
                BbsSettings temp = (BbsSettings)setting;
                temp.DisplaySignature = new ExceptableSetting.ExceptableItem_bool().GetExceptable("DisplaySignature", msgDisplay);

                if (msgDisplay.HasAnyError())
                {
                    //ThrowError<Errors.CustomError>(new Errors.CustomError("", ""));
                    return false;
                }
                return true;
            }
            else if (property.Name == "MaxAttachmentCountInDay")
            {
                BbsSettings temp = (BbsSettings)setting;
                temp.MaxAttachmentCountInDay = new ExceptableSetting.ExceptableItem_Int().GetExceptable("MaxAttachmentCountInDay", msgDisplay);

                if (msgDisplay.HasAnyError())
                {
                    //ThrowError<Errors.CustomError>(new Errors.CustomError("", ""));
                    return false;
                }
                return true;
            }
            else if (property.Name == "MaxTotalAttachmentsSizeInDay")
            {
                BbsSettings temp = (BbsSettings)setting;
                temp.MaxTotalAttachmentsSizeInDay = new ExceptableSetting.ExceptableItem_FileSize().GetExceptable("MaxTotalAttachmentsSizeInDay", msgDisplay);

                if (msgDisplay.HasAnyError())
                {
                    //ThrowError<Errors.CustomError>(new Errors.CustomError("", ""));
                    return false;
                }
                return true;
            }
            else
                return base.SetSettingItemValue(setting, property);
        }

        protected BbsSettings BbsSettings
        {
            get
            {
                return AllSettings.Current.BbsSettings;
            }
        }
    }
}