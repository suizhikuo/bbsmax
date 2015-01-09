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
using MaxLabs.bbsMax.Rescourses;
using System.IO;
using MaxLabs.WebEngine;
using System.Collections.Generic;
using System.Web.Configuration;
using MaxLabs.bbsMax.Emoticons;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class emoticon_upload : AdminDialogPageBase 
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Emoticon; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Group == null)
            {
                ShowError("表情分组不存在");
            }

            if (_Request.IsClick("upload"))
            {
                ShowSuccess("表情导入成功", 1);
            }
        }

        protected void Upload()
        {
           
        }

        protected DefaultEmoticonGroup Group
        {
            get
            {
                return AllSettings.Current.DefaultEmotSettings.GetEmoticonGroupByID(_Request.Get<int>("groupid", Method.Get,0));
            }
        }
    }
}