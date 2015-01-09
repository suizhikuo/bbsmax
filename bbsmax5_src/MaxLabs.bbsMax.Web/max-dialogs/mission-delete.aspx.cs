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
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;

using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class mission_delete : AdminDialogPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get { return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Manage_Mission; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("delete"))
            {
                DeleteMission();
            }

        }

        private void DeleteMission()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int missionID = _Request.Get<int>("MissionID", Method.Get, 0);
            if (missionID == 0)
            {
                msgDisplay.AddError(new InvalidParamError("MissionID").Message);
                return;
            }
            else
            {
                int[] missionIDs = new int[] { missionID };
                try
                {
                    using (new ErrorScope())
                    {
                        bool success = MissionBO.Instance.DeleteMissions(MyUserID, missionIDs);

                        if (!success)
                        {
                            CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error);
                            });
                        }
                        else
                        {
                            Return("id", missionID);
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