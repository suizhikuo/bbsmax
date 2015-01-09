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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class blacklist_remove : DialogPageBase
    {
        private int m_UserIDToRemove;
        private SimpleUser m_UserToRemove;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_UserIDToRemove = _Request.Get<int>("uid", Method.Get, 0);

            if (_Request.IsClick("delete"))
            {
                Delete();
            }

            m_UserToRemove = UserBO.Instance.GetSimpleUser(m_UserIDToRemove);

            if (m_UserToRemove == null)
                ShowError(new UserNotExistsError("uid", m_UserIDToRemove));
        }

        protected SimpleUser UserToRemove
        {
            get { return m_UserToRemove; }
        }

        private void Delete()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = FriendBO.Instance.DeleteFromBlacklist(MyUserID, m_UserIDToRemove);
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
                Return("removedUserID", m_UserIDToRemove);
        }
    }
}