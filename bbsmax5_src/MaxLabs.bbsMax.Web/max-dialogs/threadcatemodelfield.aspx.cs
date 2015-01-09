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
    public partial class threadcatemodelfield : AdminDialogPageBase
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
                    return "编辑字段";
                else
                    return "添加字段";
            }
        }

        protected int ModelID
        {
            get
            {
                return _Request.Get<int>("modelID", Method.Get, 0);
            }
        }

        protected int FieldID
        {
            get
            {
                return _Request.Get<int>("FieldID", Method.Get, 0);
            }
        }

        private ThreadCateModelField m_ThreadCateModelField;
        protected ThreadCateModelField Field
        {
            get
            {
                if (m_ThreadCateModelField == null)
                {
                    if (IsEdit)
                        m_ThreadCateModelField = ThreadCateBO.Instance.GetField(ModelID, FieldID);
                    else
                    {
                        m_ThreadCateModelField = new ThreadCateModelField();
                        m_ThreadCateModelField.AdvancedSearch = false;
                        m_ThreadCateModelField.DisplayInList = false;
                        m_ThreadCateModelField.Enable = true;
                        m_ThreadCateModelField.FieldName = string.Empty;
                        m_ThreadCateModelField.FieldType = string.Empty;
                        m_ThreadCateModelField.FieldTypeSetting = string.Empty;
                        m_ThreadCateModelField.ModelID = ModelID;
                        m_ThreadCateModelField.MustFilled = false;
                        m_ThreadCateModelField.Search = false;
                    }
                }

                return m_ThreadCateModelField;
            }
        }

        protected bool IsEdit
        {
            get
            {
                return _Request.Get("action", Method.Get, "").Trim().ToLower() == "edit";
            }
        }

        protected ExtendedFieldType[] ExtendedFieldTypeList
        {
            get
            {
                return UserBO.Instance.GetRegistedExtendedFieldTypes();
            }
        }

        private ExtendedField m_CurrentField;
        protected ExtendedField CurrentField
        {
            get
            {
                if (m_CurrentField == null)
                {
                    if (IsEdit)
                    {
                        m_CurrentField = Field.ExtendedField;
                    }
                    else
                    {
                        m_CurrentField = new ExtendedField();
                        m_CurrentField.FieldTypeName = _Request.Get("type", Method.Get, string.Empty);
                        m_CurrentField.Name = "";
                        m_CurrentField.Description = "";
                    }
                }

                return m_CurrentField;
            }
        }

        protected void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("modelname");

            string fieldName = _Request.Get("fieldName",Method.Post,string.Empty);
            string fieldType;
            if (IsEdit)
                fieldType = Field.FieldType;
            else
                fieldType = _Request.Get("fieldType", Method.Post, string.Empty);


            ExtendedFieldType ftype = UserBO.Instance.GetExtendedFieldType(fieldType);
            

            if (ftype == null)
            {
                msgDisplay.AddError("请选择字段类型");
                return;
            }

            int sortOrder = _Request.Get<int>("SortOrder", Method.Post, 0);
            bool enable = _Request.Get<int>("Enable", Method.Post, 1) == 1;
            bool search = _Request.Get<int>("Search", Method.Post, 1) == 1;
            bool advancedSearch = _Request.Get<int>("AdvancedSearch", Method.Post, 1) == 1;
            bool displayInList = _Request.Get<int>("DisplayInList", Method.Post, 1) == 1;
            bool mustFilled = _Request.Get<int>("MustFilled", Method.Post, 1) == 1;
            string description = _Request.Get("Description", Method.Post, string.Empty);

            bool success;
            try
            {
                if (IsEdit == false)
                {
                    success = ThreadCateBO.Instance.CreateThreadCateModelField(My, ModelID, fieldName, enable, sortOrder, ftype.TypeName, ftype.LoadSettingsFromRequest().ToString()
                        , search, advancedSearch, displayInList, mustFilled, description);
                }
                else
                {
                    success = ThreadCateBO.Instance.UpdateThreadCateModelField(My, FieldID, fieldName, enable, sortOrder, ftype.LoadSettingsFromRequest().ToString()
                        , search, advancedSearch, displayInList, mustFilled, description);
                }
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