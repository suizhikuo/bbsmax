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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Web;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class max_dialogs_chat_admindelete_session : AdminDialogPageBase
    {
        private int sessionID;
        private ChatSession m_ChatSession;

        protected void Page_Load(object sender, EventArgs e)
        {
            sessionID = _Request.Get<int>("sessionID", Method.Get, 0);
            m_ChatSession = ChatBO.Instance.GetChatSession(sessionID);

            if (m_ChatSession == null)
                ShowError(new NotExistsChatSessionError(0));

            if (!AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Manage_Chat, m_ChatSession.UserID))
            {
                ShowError("您没有权限删除该对话记录");
                return;
            }

            //UserBO.Instance.WaitForFillSimpleUsers<ChatSession>(new ChatSession[] { m_ChatSession }, 1);

            WaitForFillSimpleUser<ChatSession>(m_ChatSession, 1);
            
           
            if (_Request.IsClick("delete"))
            {
                DeleteChatSession();
            }
        }

        protected ChatSession ChatSession
        {
            get { return m_ChatSession; }
        }

        public void DeleteChatSession()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            bool success = false;


            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = ChatBO.Instance.DeleteChatSession(ChatSession.OwnerID, ChatSession.UserID);
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
                Return("sesionid", sessionID);
        }
    }

}