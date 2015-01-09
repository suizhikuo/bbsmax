//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_threadcatemodel : AdminPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_ThreadCate; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (ModelID == 0)
            {

                if (ModelList.Count > 0)
                    BbsRouter.JumpToCurrentUrl(true, "?cateID=" + CateID + "&modelID=" + ModelList[0].ModelID);
            }

            if (_Request.IsClick("save"))
            {
                Save();
            }

        }


        private ThreadCateModelCollection m_ModelList;
        protected ThreadCateModelCollection ModelList
        {
            get
            {
                if (m_ModelList == null)
                    m_ModelList = ThreadCateBO.Instance.GetModels(CateID);
                return m_ModelList;
            }
        }

        private int? m_CateID;
        protected int CateID
        {
            get
            {
                if (m_CateID == null)
                    m_CateID = _Request.Get<int>("cateID", Method.Get, 0);
                return m_CateID.Value;
            }
        }

        private ThreadCateModelFieldCollection m_FieldList;
        protected ThreadCateModelFieldCollection FieldList
        {
            get
            {
                if (m_FieldList == null)
                    m_FieldList = ThreadCateBO.Instance.GetFields(ModelID);
                return m_FieldList;
            }
        }

        private int? m_ModelID;
        protected int ModelID
        {
            get
            {
                if (m_ModelID == null)
                    m_ModelID = _Request.Get<int>("modelID", Method.Get, 0);

                return m_ModelID.Value;
            }
        }

        protected ThreadCateModelCollection GetEnableModelList(int cateID)
        {
            return ThreadCateBO.Instance.GetModels(cateID);
        }

        protected ExtendedFieldType GetExtendedFieldType(string fieldType)
        {
            return UserBO.Instance.GetExtendedFieldType(fieldType);
        }

        protected string IncludeExtendFiled(ThreadCateModelField field)
        {
            ExtendedFieldType type = UserBO.Instance.GetExtendedFieldType(field.FieldType);

            this.Include(this.HtmlTextWriter, new object[] { "src", type.FrontendControlSrc, "value", "", "field", field.ExtendedField });

            return null;
        }

        protected string GetFieldTypeName(string fieldType)
        {
            ExtendedFieldType type = GetExtendedFieldType(fieldType);
            if (type == null)
                return string.Empty;
            else
                return type.DisplayName;
        }


        private void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] ids = _Request.GetList<int>("ids", Method.Post, new int[] { });

            List<int> sortOrders = new List<int>();
            List<bool> enables = new List<bool>();
            List<bool> searchs = new List<bool>();
            List<bool> advancedSearchs = new List<bool>();
            List<bool> displayInLists = new List<bool>();
            List<bool> mustFilleds = new List<bool>();
            foreach (int id in ids)
            {
                int sortOrder = _Request.Get<int>("sortorder_" + id, Method.Post, 0);
                bool enable = _Request.Get("isenable_" + id, Method.Post, "") == "true";
                bool search = _Request.Get("search_" + id, Method.Post, "") == "true";
                bool advancedSearch = _Request.Get("AdvancedSearch_" + id, Method.Post, "") == "true";
                bool displayInList = _Request.Get("DisplayInList_" + id, Method.Post, "") == "true";
                bool mustFilled = _Request.Get("MustFilled_" + id, Method.Post, "") == "true";
                sortOrders.Add(sortOrder);
                enables.Add(enable);
                searchs.Add(search);
                advancedSearchs.Add(advancedSearch);
                displayInLists.Add(displayInList);
                mustFilleds.Add(mustFilled);
            }

            bool success;
            try
            {
                success = ThreadCateBO.Instance.UpdateThreadCateModelFields(My, ids, enables, sortOrders, searchs, advancedSearchs
                    , displayInLists, mustFilleds);
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
        }
    }
}