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
using MaxLabs.bbsMax.ExceptableSetting;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_netdisk : AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_NetDisk; }
        }

        protected void Page_Load(object sender, EventArgs e)
		{
            if (_Request.IsClick("savedisksetting"))
            {
                SaveDiskSetting();
            }
        }

        private void SaveDiskSetting()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("MaxFileCount", "MaxFileSize", "DiskSpaceSize", "EnableDisk", "AllowFileExtensions", "DefaultViewMode"
        , "new_MaxFileCount", "new_MaxFileSize", "new_DiskSpaceSize", "new_EnableDisk", "new_AllowFileExtensions", "new_DefaultViewMode");

            DiskSettings diskSetting = new DiskSettings();
            //diskSetting.Import = new ExceptableItem_bool().GetExceptable("Import", msgDisplay); 
            //diskSetting.Export = new ExceptableItem_bool().GetExceptable("Export", msgDisplay); 
            diskSetting.MaxFileCount = new ExceptableItem_Int_MoreThenZero().GetExceptable("MaxFileCount", msgDisplay);
            diskSetting.MaxFileSize = new ExceptableItem_FileSize().GetExceptable("MaxFileSize", msgDisplay);
            diskSetting.DiskSpaceSize = new ExceptableItem_FileSize().GetExceptable("DiskSpaceSize", msgDisplay);
            diskSetting.EnableDisk = _Request.Get<bool>("EnableDisk", Method.Post, false);
            diskSetting.AllowFileExtensions = new ExceptableItem_ExtensionList().GetExceptable("AllowFileExtensions",msgDisplay);
            diskSetting.DefaultViewMode = new ExceptableItem_Enum<FileViewMode>().GetExceptable("DefaultViewMode", msgDisplay);
            if (!msgDisplay.HasAnyError())
            {
                SettingManager.SaveSettings(diskSetting);

                JumpTo("app/setting-netdisk.aspx?success=1");
            }
        }

        protected bool Success
        {
            get
            {
                return _Request.Get<int>("success", Method.Get, 0) == 1;
            }
        }
    }
}