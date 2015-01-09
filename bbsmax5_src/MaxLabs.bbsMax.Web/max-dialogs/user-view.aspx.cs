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
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs.user
{
    public partial class user_view : UserDialogPageBase
    {
        private SimpleUser m_User;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!UserBO.Instance.CanEditUserProfile(My, UserID))
            {
                ShowError(new NoPermissionEditUserProfileError());
                return ;
            }

            m_User = UserBO.Instance.GetSimpleUser(UserID);

            if (m_User == null)
            {
                ShowError(new UserNotExistsError("id", UserID));
                return;
            }

            if (_Request.IsClick("save"))
            {
                UpdateUserinfo();
            }
        }

        /// <summary>
        /// 是否开启邀请
        /// </summary>
        protected bool EnableInvite
        {
            get { return AllSettings.Current.InvitationSettings.InviteMode != InviteMode.Close; }
        }

        protected void UpdateUserinfo()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int totalOnlinetime = _Request.Get<int>("totalOnlineTime", Method.Post, 0);
            int monthOnlineTime = _Request.Get<int>("monthOnlineTime", Method.Post, 0);
            DateTime createDate = _Request.Get<DateTime>("createdate", Method.Post, DateTimeUtil.Now);

            using (ErrorScope es = new ErrorScope())
            {
                UserBO.Instance.AdminUpdateUserinfo(My, UserID, createDate, totalOnlinetime, monthOnlineTime);
                if (es.HasError)
                {
                    es.CatchError(delegate(ErrorInfo error){
                        msgDisplay.AddError(error);
                    
                    });
                }
            }
        }
    }
}