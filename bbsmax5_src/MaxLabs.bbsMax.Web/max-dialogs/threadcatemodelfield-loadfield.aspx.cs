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
    public partial class threadcatemodelfield_loadfield : AdminDialogPageBase
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

        protected ThreadCateCollection ThreadCateList
        {
            get
            {
                return ThreadCateBO.Instance.GetAllCates();
            }
        }

        protected ThreadCateModelCollection GetModels(int cateID)
        {
            return ThreadCateBO.Instance.GetModels(cateID);
        }

        protected int ModelID
        {
            get
            {
                return _Request.Get<int>("modelID", Method.Get, 0);
            }
        }

        protected int LoadModelID
        {
            get
            {
                return _Request.Get<int>("loadModelID", Method.Get, 0);
            }
        }

        private ThreadCateModelFieldCollection m_FieldList;
        protected ThreadCateModelFieldCollection FieldList
        {
            get
            {
                if (m_FieldList == null)
                {
                    m_FieldList = ThreadCateBO.Instance.GetFields(LoadModelID);
                }

                return m_FieldList;
            }
        }


        protected void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] ids = _Request.GetList<int>("fieldIDs", Method.Post, new int[] { });

            if (ValidateUtil.HasItems<int>(ids) == false)
            {
                msgDisplay.AddError("请选择要导入的字段");
                return;
            }

            bool success;
            try
            {
                success = ThreadCateBO.Instance.LoadModelFields(My, ModelID, ids);
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