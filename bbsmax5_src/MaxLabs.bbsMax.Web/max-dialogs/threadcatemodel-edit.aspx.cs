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
using MaxLabs.bbsMax.Web.max_pages.forums;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class threadcatemodel_edit : AdminDialogPageBase
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
            else if (_Request.Get("action", Method.Get, "") == "delete")
            {
                Delete();
            }
        }


        protected int CateID
        {
            get
            {
                return _Request.Get<int>("cateID", Method.Get, 0);
            }
        }

        private ThreadCateModelCollection m_ModelList;
        protected ThreadCateModelCollection ModelList
        {
            get
            {
                if (m_ModelList == null)
                {
                    m_ModelList = ThreadCateBO.Instance.GetModels(CateID);
                }

                return m_ModelList;
            }
        }

        private void Delete()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int id = _Request.Get<int>("modelID", Method.Get, 0);

            bool success;
            try
            {
                success = ThreadCateBO.Instance.DeleteModels(My, new int[] { id });
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
                Response.Redirect(Dialog + "/threadcatemodel-edit.aspx?isdialog=1&cateid=" + CateID);
            }
        }

        private void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("modelname");

            int[] modelIDs = _Request.GetList<int>("modelIDs", Method.Post, new int[] { });
            int[] enableIDs = _Request.GetList<int>("enableIDs", Method.Post, new int[] { });

            List<int> sortOrders = new List<int>();
            List<string> names = new List<string>();
            foreach (int modelID in modelIDs)
            {
                int sortOrder = _Request.Get<int>("sortorder_" + modelID, Method.Post, 0);
                string name = _Request.Get("modelname_" + modelID, Method.Post, string.Empty);

                sortOrders.Add(sortOrder);
                names.Add(name);
            }

            bool success;
            try
            {
                success = ThreadCateBO.Instance.UpdateModels(My, modelIDs, sortOrders, names, enableIDs);
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