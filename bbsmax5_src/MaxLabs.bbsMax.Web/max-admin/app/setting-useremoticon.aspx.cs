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
using MaxLabs.bbsMax.ExceptableSetting;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_useremoticon : AdminPageBase 
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_UserEmoticon; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("saveEmoticonSettings"))
            {
                SaveEmoticonSetting();
            }
        }

        private void SaveEmoticonSetting()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("Import", "Export", "MaxEmoticonCount", "MaxEmoticonFileSize", "MaxEmoticonSpace", "EnableUserEmoticons"
                , "new_Import", "new_Export", "new_MaxEmoticonCount", "new_MaxEmoticonFileSize", "new_MaxEmoticonSpace", "new_EnableUserEmoticons");

            EmoticonSettings emoticonSetting =new EmoticonSettings();

            emoticonSetting.Import = new ExceptableItem_bool().GetExceptable("Import", msgDisplay);
            emoticonSetting.Export = new ExceptableItem_bool().GetExceptable("Export", msgDisplay);
            emoticonSetting.MaxEmoticonCount = new ExceptableItem_Int_MoreThenZero().GetExceptable("MaxEmoticonCount", msgDisplay);
            emoticonSetting.MaxEmoticonFileSize = new ExceptableItem_FileSize().GetExceptable("MaxEmoticonFileSize", msgDisplay);
            emoticonSetting.MaxEmoticonSpace = new ExceptableItem_FileSize().GetExceptable("MaxEmoticonSpace", msgDisplay);
            emoticonSetting.EnableUserEmoticons = _Request.Get<bool>("EnableUserEmoticons", Method.Post, false);

            if (!msgDisplay.HasAnyError())
            {
                SettingManager.SaveSettings(emoticonSetting);
            }
        }
    }
}