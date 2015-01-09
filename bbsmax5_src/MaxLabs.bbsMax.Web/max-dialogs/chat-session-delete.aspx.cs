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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class chat_session_delete : DialogPageBase
    {
        private int m_TargetUserID;
        private ChatSession m_ChatSession;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_TargetUserID = _Request.Get<int>("tuid", Method.Get, 0);
            if (m_TargetUserID <= 0)
                ShowError(new InvalidParamError("tuid"));


            if (_Request.IsClick("delete"))
            {
                DeleteChatSession();
            }

            m_ChatSession = ChatBO.Instance.GetChatSession(MyUserID, m_TargetUserID);

            if (m_ChatSession == null)
                ShowError(new NotExistsChatSessionError(m_TargetUserID));

            WaitForFillSimpleUser<ChatSession>(m_ChatSession, 0);
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
                    success = ChatBO.Instance.DeleteChatSession(MyUserID, m_TargetUserID);
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
                Return("targetUserID", m_TargetUserID);
        }

    }
}