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

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class mission_category_add : AdminDialogPageBase
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
            if (_Request.IsClick("add"))
            {
                AddCategory();
            }
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

        private void AddCategory()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            string categoryName = _Request.Get("name", Method.Post);

            int newCategoryId = 0;
            bool success = false;


            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = MissionBO.Instance.CreateMissionCategory(MyUserID, categoryName, out newCategoryId);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (success == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }

            if (success)
            {
                JsonBuilder json = new JsonBuilder();
                json.Set("id", newCategoryId);
                json.Set("name", categoryName);
                Return(json);
            }
        }
    }
}