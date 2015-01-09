//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class mission_category_edit : AdminDialogPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Manage_Mission_Category;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            categoryID = _Request.Get<int>("id", Method.All);

            if (categoryID.HasValue == false)
                ShowError("缺少必要参数");

            MissionCategory category = MissionBO.Instance.GetMissionCategory(categoryID.Value);

            if (category == null)
                ShowError("任务分类不存在");

            m_CategoryName = category.Name;

            if (_Request.IsClick("edit"))
            {
                EditCategory();
            }
        }

        private string m_CategoryName;

        protected string CategoryName
        {
            get { return m_CategoryName; }
        }

        protected override bool EnableClientBuffer
        {
            get
            {
                return false;
            }
        }

        protected int CategoryNameMaxLength
        {
            get
            {
                return 20; //老达TODO:常量
            }
        }

        int? categoryID = null;

        private void EditCategory()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                string categoryName = _Request.Get("name", Method.Post);

                if (categoryID == null)
                {
                    msgDisplay.AddError(new InvalidParamError("categoryID").Message);
                    return;
                }

                try
                {
                    if (MissionBO.Instance.UpdateMissionCategory(MyUserID, categoryID.Value, categoryName))
                    {
                        JsonBuilder json = new JsonBuilder();

                        json.Set("id", categoryID);
                        json.Set("name", categoryName);

                        Return(json);
                    }
                    else
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                }
            }
        }
    }
}