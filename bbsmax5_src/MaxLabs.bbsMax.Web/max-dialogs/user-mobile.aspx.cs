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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs.user
{
    public partial class user_mobile : UserDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!UserBO.Instance.CanEditUserProfile(My, UserID))
            {
                ShowError(new NoPermissionEditUserProfileError());
                return;
            }

            m_User = UserBO.Instance.GetUser(UserID);

            if (m_User == null)
            {
                ShowError(new UserNotExistsError("id", UserID));
                return;
            }

            if (_Request.IsClick("bind"))
            {
                PhoneBind();
            }
            else if (_Request.IsClick("unbind"))
            {
                UserBO.Instance.RemoveUserPhone( My, User);
            }

            
        }

        private User m_User = null;
        protected new User User
        {
            get { return m_User; }
        }

        public bool IsBound
        {
            get {return User.MobilePhone != 0; }
        }

        private void PhoneBind()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("mobilephone");
            string phone = _Request.Get("mobilephone", Method.Post);
            using (ErrorScope es = new ErrorScope())
            {
                UserBO.Instance.UpdateUserPhone(My, User, phone);
                es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                {
                    msgDisplay.AddError(err.TatgetName, err.Message);
                });
            }
        }
    }
}