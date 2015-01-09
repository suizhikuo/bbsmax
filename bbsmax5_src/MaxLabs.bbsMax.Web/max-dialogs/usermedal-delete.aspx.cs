//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class usermedal_delete : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (AllSettings.Current.ManageUserPermissionSet.Can(My, ManageUserPermissionSet.ActionWithTarget.EditUserMedal, UserID) == false)
            {
                ShowError("您所在的用户组没有权限对该用户进行此操作");
            }

            if (_Request.IsClick("delete"))
            {
                Delete();
            }
        }

        private int UserID
        {
            get
            {
                return _Request.Get<int>("userID", Method.Get, 0);
            }
        }

        private void Delete()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                int medalID = _Request.Get<int>("medalID", Method.Get, 0);

                try
                {
                    bool success = UserBO.Instance.DeleteMedalUsers(My, medalID, new int[] { UserID });
                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                        return;
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                    return;
                }

                Return(medalID, true);
            }
        }
    }
}