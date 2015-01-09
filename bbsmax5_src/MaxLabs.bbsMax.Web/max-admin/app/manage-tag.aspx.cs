//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;

using MaxLabs.bbsMax.Filters;

using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_tag : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Tag; }
        }

        protected void Page_Load(object sender, EventArgs e)
		{
			if (TagBO.Instance.CheckTagManagePermission(MyUserID) == false)
			{
				NoPermission();
			}

            if (_Request.IsClick("advancedsearch"))
            {
                SearchTags();
            }
            else if (_Request.IsClick("deletechecked"))
            {
                DeleteChecked();
            }
            else if (_Request.IsClick("deletesearched"))
            {
                DeleteSearched();
            }
        }

        private void SearchTags()
        {
            AdminTagFilter filter = AdminTagFilter.GetFromForm();

            filter.Apply("filter", "page");
        }

        private void DeleteChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    int[] tagIDs = StringUtil.Split<int>(_Request.Get("tagids", Method.Post));

                    if (tagIDs == null || tagIDs.Length == 0)
                    {
                        msgDisplay.AddError("请至少选择一条要删除的数据");
                        return;
                    }

                    bool success = TagBO.Instance.DeleteTags(tagIDs);
                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {

                    }

                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                }
            }
        }

        private void DeleteSearched()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    AdminTagFilter filter = AdminTagFilter.GetFromFilter("filter");
                    if (filter != null)
                    {

                        bool success = TagBO.Instance.DeleteTags(filter);
                        if (!success)
                        {
                            CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error);
                            });
                        }
                        else
                        {

                        }
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