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
    public partial class threadcate : AdminDialogPageBase
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


        protected string ActionName
        {
            get
            {
                if (IsEdit)
                    return "编辑分类主题";
                else
                    return "添加分类主题";
            }
        }

        private bool? m_IsEdit;
        protected bool IsEdit
        {
            get
            {
                if (m_IsEdit == null)
                {
                    m_IsEdit = _Request.Get("action", Method.Get, "").ToLower().Trim() == "edit";
                }

                return m_IsEdit.Value;
            }
        }

        private ThreadCate m_ThreadCate;
        protected ThreadCate ThreadCate
        {
            get
            {
                if (m_ThreadCate == null)
                {
                    m_ThreadCate = ThreadCateBO.Instance.GetAllCates().GetValue(CateID);
                }
                return m_ThreadCate;
            }
        }

        protected int CateID
        {
            get
            {
                return _Request.Get<int>("cateID", Method.Get, 0);
            }
        }

        protected string CateName
        {
            get
            {
                if (ThreadCate == null)
                    return string.Empty;
                else
                    return ThreadCate.CateName;
            }
        }

        protected int SortOrder
        {
            get
            {
                if (ThreadCate == null)
                    return 0;
                else
                    return ThreadCate.SortOrder;
            }
        }

        protected bool IsEnable
        {
            get
            {
                if (ThreadCate == null)
                    return true;
                else
                    return ThreadCate.Enable;
            }
        }

        private ThreadCateModelCollection m_Models;
        protected ThreadCateModelCollection ModelList
        {
            get
            {
                if (m_Models == null)
                {
                    if (IsEdit)
                        m_Models = ThreadCateBO.Instance.GetModels(CateID);
                    else
                        m_Models = new ThreadCateModelCollection();
                }

                return m_Models;
            }
        }

        protected void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("catename");

            string catename = _Request.Get("CateName", Method.Post, string.Empty);
            int sortOrder = _Request.Get<int>("SortOrder", Method.Post, 0);
            bool enable = _Request.Get<int>("Enable", Method.Post, 1) == 1;

            int[] enableIDs = _Request.GetList<int>("modelIDs", Method.Post, new int[] { });

            bool success;
            try
            {
                if (IsEdit)
                {
                    success = ThreadCateBO.Instance.UpdateThreadCate(My, CateID, catename, enable, sortOrder, enableIDs);
                }
                else
                    success = ThreadCateBO.Instance.CreateThreadCate(My, catename, enable, sortOrder);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
                return;
            }

            if (success == false)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error) {
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