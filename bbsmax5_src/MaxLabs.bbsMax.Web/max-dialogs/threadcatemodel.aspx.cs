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

using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class threadcatemodel : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_ThreadCate; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("save"))
            {
                Save();
            }
        }

        protected int CateID
        {
            get
            {
                return _Request.Get<int>("cateID", Method.Get, 0);
            }
        }

        protected void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("modelname");

            string modelName = _Request.Get("modelName", Method.Post, string.Empty);
            int sortOrder = _Request.Get<int>("SortOrder", Method.Post, 0);
            bool enable = _Request.Get<int>("Enable", Method.Post, 1) == 1;

            bool success;
            try
            {
                success = ThreadCateBO.Instance.CreateModel(My, CateID, modelName, enable, sortOrder);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
                return;
            }

            if (success == false)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                });
            }
            else
            {
                Return(true);
            }
        }
    }
}