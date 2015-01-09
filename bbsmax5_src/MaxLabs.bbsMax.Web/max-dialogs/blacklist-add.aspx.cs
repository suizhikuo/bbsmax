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
    public partial class blacklist_add : DialogPageBase
    {
        private string m_UserIdsToAddText;
        private int[] m_UserIdsToAdd;
        private SimpleUserCollection m_UserListToAdd;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_UserIdsToAddText = _Request.Get("uid");
            m_UserIdsToAdd = StringUtil.Split<int>(m_UserIdsToAddText, ',');

            if (m_UserIdsToAdd.Length == 0)
                ShowError(new NotSelectedUserError("uid"));

            if (_Request.IsClick("add"))
            {
                Add();
            }

            m_UserListToAdd = UserBO.Instance.GetSimpleUsers(m_UserIdsToAdd);

            if (m_UserListToAdd.Count == 0)
                ShowError(new NotSelectedUserError("uid"));
        }

        protected string UserIdsToAddText
        {
            get { return m_UserIdsToAddText; }
        }

        protected SimpleUserCollection UserListToAdd
        {
            get { return m_UserListToAdd; }
        }

        private void Add()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = FriendBO.Instance.AddUsersToBlacklist(MyUserID, m_UserIdsToAdd);
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
                Return(true);

        }
    }
}