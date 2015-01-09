//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class threadcate_delete : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
		{
            if (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Manage_ThreadCate) == false)
                ShowError(new NoPermissionManageThreadCateError());

			cateid = _Request.Get<int>("cateid", Method.Get);

            if (cateid == null)
				ShowError("缺少必要参数");


            if (_Request.IsClick("delete"))
            {
                Delete();
            }
        }

		int? cateid = null;

        private void Delete()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    bool success = ThreadCateBO.Instance.DeleteThreadCates(My, new int[] { cateid.Value });

                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error);
                            }
                        );
                    }
                    else
                    {
                        Return("cateID", cateid.Value);
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