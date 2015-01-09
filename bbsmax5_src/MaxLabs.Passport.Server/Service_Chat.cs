//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using MaxLabs.bbsMax;
using MaxLabs.WebEngine;
using System.Web.Services.Protocols;
using MaxLabs.Passport.Proxy;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.Passport.Server
{
    public partial class Service
    {
        [WebMethod(Description = "发送消息")]
        [SoapHeader("clientinfo")]
        public APIResult Chat_SendMessage(int senderUserID, int recoverUserID,  string contentWithoutEncode, string ip, bool getNewMessages, int lastMessageID, out List<ChatMessageProxy> chatMessages)
        {
            chatMessages = null;
            if (CheckClient())
            {
                APIResult result = new APIResult();
                using (ErrorScope es = new ErrorScope())
                {
                    try
                    {
                        ChatMessageCollection messages = ChatBO.Instance.Server_SendMessage(senderUserID, recoverUserID, contentWithoutEncode, ip, getNewMessages, lastMessageID);

                        chatMessages = new List<ChatMessageProxy>();
                        foreach (ChatMessage message in messages)
                        {
                            chatMessages.Add(ProxyConverter.GetChatMessageProxy(message));
                        }

                        bool hasError = false;
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            hasError = true;
                            result.AddError(error.TatgetName,error.Message);
                        });

                        result.IsSuccess = hasError == false;
                    }
                    catch (Exception ex)
                    {
                        result.ErrorCode = Consts.ExceptionCode;
                        result.AddError(ex.Message);
                        result.IsSuccess = false;
                    }
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        [WebMethod(Description = "获取最新的消息")]
        [SoapHeader("clientinfo")]
        public List<ChatMessageProxy> Chat_GetLastChatMessages(int userID, int targetUserID, int lastMessageID, int messageCount)
        {
            if (!CheckClient()) return null;
            
            ChatMessageCollection messages = ChatBO.Instance.GetLastChatMessages(userID, targetUserID, lastMessageID, messageCount);

            List<ChatMessageProxy> result = new List<ChatMessageProxy>();

            foreach (ChatMessage m in messages)
            {
                result.Add(ProxyConverter.GetChatMessageProxy(m));
            }

            return result;
        }

        [WebMethod(Description = "获取用户对话")]
        [SoapHeader("clientinfo")]
        public List<ChatMessageProxy> Chat_AdminGetChatMessages(int userID, int targetUserID, int pageNumber, int pageSize)
        {
            if (!CheckClient()) return null;


            ChatMessageCollection messages = ChatBO.Instance.AdminGetChatMessages(userID, targetUserID, pageNumber, pageSize);

            List<ChatMessageProxy> result = new List<ChatMessageProxy>();

            foreach (ChatMessage m in messages)
            {
                result.Add(ProxyConverter.GetChatMessageProxy(m));
            }

            return result;
        }



        [WebMethod(Description = "获取用户对话")]
        [SoapHeader("clientinfo")]
        public List<ChatSessionProxy> Chat_AdminGetSessions(int userID, MaxLabs.Passport.Proxy.DataForSearchChatSession filter, int pageNumber)
        {
            if (!CheckClient()) return null;

            ChatSessionCollection sessions = ChatBO.Instance.Server_AdminGetSessions(userID, filter, pageNumber);

            List<ChatSessionProxy> result = new List<ChatSessionProxy>();

            foreach (ChatSession s in sessions)
            {
                result.Add(ProxyConverter.GetChatSessionProxy(s));
            }

            return result;
        }

        [WebMethod(Description = "获取对话")]
        [SoapHeader("clientinfo")]
        public ChatSessionProxy Chat_GetChatSession(int sessionID)
        {
            if (!CheckClient()) return null;

            ChatSession session = ChatBO.Instance.GetChatSession(sessionID);

            return ProxyConverter.GetChatSessionProxy(session);
        }


        [WebMethod(Description = "获取用户对话")]
        [SoapHeader("clientinfo")]
        public List<ChatSessionProxy> Chat_GetChatSessionsWithUnreadMessages(int userID, int topCount)
        {
            if (!CheckClient()) return null;
            ChatSessionCollection sessions = ChatBO.Instance.GetChatSessionsWithUnreadMessages(userID, topCount);

            List<ChatSessionProxy> result = new List<ChatSessionProxy>();

            foreach (ChatSession s in sessions)
            {
                result.Add(ProxyConverter.GetChatSessionProxy(s));
            }

            return result;
        }

        [WebMethod(Description = "获取用户对话")]
        [SoapHeader("clientinfo")]
        public ChatSessionProxy Chat_GetChatSessionByUserID(int userID, int targetUserID)
        {
            if (!CheckClient()) return null;

            ChatSession session = ChatBO.Instance.GetChatSession(userID, targetUserID);

            return ProxyConverter.GetChatSessionProxy(session);
        }


        [WebMethod(Description = "获取用户对话")]
        [SoapHeader("clientinfo")]
        public List<ChatSessionProxy> Chat_GetChatSessions(int userID, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;
            if (!CheckClient()) return null;
            ChatSessionCollection sessions = ChatBO.Instance.GetChatSessions(userID, pageNumber, pageSize);

            List<ChatSessionProxy> result = new List<ChatSessionProxy>();

            foreach (ChatSession s in sessions)
            {
                result.Add(ProxyConverter.GetChatSessionProxy(s));
            }
            totalCount = sessions.TotalRecords;
            return result;
        }

        [WebMethod(Description = "获取对话")]
        [SoapHeader("clientinfo")]
        public List<ChatMessageProxy> Chat_GetChatMessages(int userID, int targetUserID, int pageNumber, int pageSize, bool processKeyword, bool updateIsReaded, out int totalCount)
        {
            totalCount = 0;
            if (!CheckClient()) return null;

            ChatMessageCollection messages = ChatBO.Instance.GetChatMessages(userID, targetUserID, pageNumber, pageSize, updateIsReaded);

            totalCount = messages.TotalRecords;

            List<ChatMessageProxy> result = new List<ChatMessageProxy>();

            foreach (ChatMessage m in messages)
            {
                result.Add(ProxyConverter.GetChatMessageProxy(m));
            }

            return result;
        }
        
        [WebMethod(Description = "管理员删除用户对话")]
        [SoapHeader("clientinfo")]
        public APIResult Chat_AdminDeleteSessions(int userID, List<int> sessionIds)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    ChatBO.Instance.AdminDeleteSessions(userID, sessionIds);
                    bool hasError = false;
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        hasError = true;
                        result.AddError(error.TatgetName,error.Message);
                    });

                    result.IsSuccess = hasError == false;
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Consts.ExceptionCode;
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        [WebMethod(Description = "忽略用户对话")]
        [SoapHeader("clientinfo")]
        public void Chat_IgnoreSession(int userID, int targetUserID)
        {
            if (!CheckClient()) return;
            ChatBO.Instance.IgnoreSession(userID, targetUserID);
        }

        [WebMethod(Description = "忽略所有用户对话")]
        [SoapHeader("clientinfo")]
        public void Chat_IgnoreAll(int userID)
        {
            if (!CheckClient()) return;
            ChatBO.Instance.IgnoreAllMessage(userID);
        }



        [WebMethod(Description = "删除用户对话")]
        [SoapHeader("clientinfo")]
        public APIResult Chat_DeleteChatSessions(int userID, List<int> targetUserIds)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = ChatBO.Instance.DeleteChatSessions(userID, targetUserIds);

                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Consts.ExceptionCode;
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                }
            }

            return result;
        }
    }
}