//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages
{
	public partial class chat : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "我的消息 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "chat"; }
        }
        protected override string NavigationKey
        {
            get { return "chat"; }
        }

        const int c_SessionPageSize = 10;
        const int c_MessagePageSize = 10;

        private int m_BeginChatUserID = 0;

        private int m_PageNumber;
        private int m_MessagePageNumber;
        private int m_TargetUserID;
        private ChatSession m_SelectedChatSession;
        private ChatSessionCollection m_ChatSessionList;
        private ChatMessageCollection m_ChatMessageList;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            AddNavigationItem("对话");

            if (!EnableChatFunction)
            {
                ShowError("管理员已关闭对话功能！");
                return;
            }

            m_PageNumber = _Request.Get<int>("page", Method.All, 0);
            m_MessagePageNumber = _Request.Get<int>("msgpage", Method.All, 0);
            m_TargetUserID = _Request.Get<int>("to", Method.All, 0);

            if (m_TargetUserID > 0)
            {
                m_SelectedChatSession = ChatBO.Instance.GetChatSession(MyUserID, m_TargetUserID);

                if (m_SelectedChatSession == null)
                    m_TargetUserID = 0;

                else
                    WaitForFillSimpleUser<ChatSession>(m_SelectedChatSession, 0);

                bool updateIsReade = false;
                if (m_MessagePageNumber == 1)
                    updateIsReade = true;

                m_ChatMessageList = ChatBO.Instance.GetChatMessages(MyUserID, m_TargetUserID, m_MessagePageNumber, c_MessagePageSize, updateIsReade);

                m_ChatMessageList.Reverse();

                int totalPageNumber = (int)Math.Ceiling((double)m_ChatMessageList.TotalRecords / (double)c_MessagePageSize);
                if (m_MessagePageNumber == 0)
                {
                    m_MessagePageNumber = totalPageNumber;
                }
                else
                {
                    m_MessagePageNumber = (totalPageNumber + 1) - m_MessagePageNumber;
                }

                WaitForFillSimpleUsers<ChatMessage>(m_ChatMessageList, 0);

                SetPager("messagelist", BbsRouter.GetUrl("my/chat", "page=" + m_PageNumber + "&to=" + TargetUserID + "&msgpage={0}"), m_MessagePageNumber, c_MessagePageSize, m_ChatMessageList.TotalRecords);
            }

            if (_Request.IsClick("beginChat"))
            {
                ProcessBeginChat();
            }

            m_ChatSessionList = ChatBO.Instance.GetChatSessions(MyUserID, m_PageNumber, c_SessionPageSize);
            WaitForFillSimpleUsers<ChatSession>(m_ChatSessionList, 0);

            SetPager("chatlist", BbsRouter.GetUrl("my/chat", "page={0}"), m_PageNumber, c_SessionPageSize, m_ChatSessionList.TotalRecords);
        }


        protected int PageNumber
        {
            get
            {
                return m_PageNumber;
            }
        }

        protected int MessagePageNumber
        {
            get
            {
                return m_MessagePageNumber;
            }
        }

        protected ChatSessionCollection ChatSessionList
        {
            get { return m_ChatSessionList; }
        }

        protected ChatMessageCollection ChatMessageList
        {
            get { return m_ChatMessageList; }
        }

        protected int SessionPageSize
        {
            get { return c_SessionPageSize; }
        }

        protected int MessagePageSize
        {
            get { return c_MessagePageSize; }
        }

        protected int BeginChatUserID
        {
            get { return m_BeginChatUserID; }
        }

        protected int SelectedChatSessionID
        {
            get
            {
                if (SelectedChatSession != null)
                    return SelectedChatSession.ChatSessionID;
                return 0;
            }
        }

        protected ChatSession SelectedChatSession
        {
            get { return m_SelectedChatSession; }
        }

        protected int TargetUserID
        {
            get
            {
                return m_TargetUserID;
            }
        }

        private void ProcessBeginChat()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            string username = _Request.Get("usernameToChat", Method.Post, string.Empty, false);
            username = username.Trim();
            if (string.IsNullOrEmpty(username))
            {
                msgDisplay.AddError("请输入您要进行对话的用户名");
            }
            else
            {
                int userIDToChat = UserBO.Instance.GetUserID(username);

                if (userIDToChat <= 0)
                {
                    msgDisplay.AddError("不存在这个用户");
                    return;
                }
                m_BeginChatUserID = userIDToChat;
            }
        }

        //private void ProcessSend()
        //{
        //    string content = _Request.Get("message", Method.Post);

        //    MessageDisplay msgDisplay = CreateMessageDisplayForForm("send");

        //    using (new ErrorScope())
        //    {
        //        if (false == ChatBO.Instance.SendMessage(MyUserID, m_TargetUserID, content, IPUtil.GetCurrentIP()))
        //        {
        //            CatchError<ErrorInfo>(delegate(ErrorInfo error){
        //                msgDisplay.AddError(error);
        //            });
        //        }
        //    }
        //}
	}
}