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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.RegExp;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;

#if !Passport
using MaxLabs.bbsMax.PassportServerInterface;
using System.Threading;

#endif
namespace MaxLabs.bbsMax
{
    public class ChatBO : BOBase<ChatBO>
    {

        #region

        public static event Chat_UsersMessageCountChanged OnUsersMessageCountChanged;


        #endregion
        const int MaxContentLength = 200;
        const int MaxContentRows = 10;

        public void ClearMessage()
        {
            JobDataClearMode mode = AllSettings.Current.ChatSettings.DataClearMode;

            int saveDay = AllSettings.Current.ChatSettings.SaveMessageDays;
            int saveRows = AllSettings.Current.ChatSettings.SaveMessageRows;

            if (mode == JobDataClearMode.Disabled)
                return;

            if (mode == JobDataClearMode.ClearByDay)
            {
                if (saveDay <= 0)
                    return;
            }
            if (mode == JobDataClearMode.ClearByRows)
            {
                if (saveRows <= 0)
                    return;
            }
            if (mode == JobDataClearMode.CombinMode)
            {
                if (saveDay == 0 && saveRows == 0)
                    return;
            }

            ChatDao.Instance.ClearMessage(saveDay, saveRows, AllSettings.Current.ChatSettings.ClearNoReadMessage, mode);
        }

        public bool IgnoreAllMessage(int userID)
        {
#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                try
                {
                    Globals.PassportClient.PassportService.Chat_IgnoreAll(userID);
                    Thread.Sleep(200);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }
                return true;
            }
            else
#endif
            {
                return ChatDao.Instance.IgnoreAllMessage(userID);
            }
        }

        public void AdminDeleteMessage(int operatorUserID, IEnumerable<int> messageIDs)
        {
            if (ValidateUtil.HasItems<int>(messageIDs))
            {
                if (PermissionSet.HasPermissionForSomeone(operatorUserID, BackendPermissions.ActionWithTarget.Manage_Chat))
                {
                    ChatDao.Instance.AdminDeleteMessage(messageIDs, GetNoPermissionRoleIDs(operatorUserID));
                }
            }
        }

        public ChatMessageCollection GetLastChatMessages(int userID, int targetUserID, int lastMessageID, int messageCount)
        {
            if (!AllSettings.Current.ChatSettings.EnableChatFunction)
                return new ChatMessageCollection();

            messageCount = messageCount <= 0 ? 200 : messageCount;

#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {

                ChatMessageProxy[] messages = settings.PassportService.Chat_GetLastChatMessages(userID, targetUserID, lastMessageID, messageCount);


                ChatMessageCollection result = new ChatMessageCollection();
                foreach(ChatMessageProxy m in messages)
                {
                    result.Add(GetChatMessage(m));
                }

                return result;
            }
            else
#endif
            {
                ChatMessageCollection messages = ChatDao.Instance.GetLastChatMessages(userID, targetUserID, lastMessageID, messageCount);
                ProcessKeyword(messages, ProcessKeywordMode.TryUpdateKeyword);

                if (messages.Count > 0)
                {
                    if (OnUsersMessageCountChanged != null)
                    {
                        AuthUser user = UserBO.Instance.GetAuthUser(userID);
                        if (user != null)
                        {
                            Dictionary<int, int> counts = new Dictionary<int, int>();
                            counts.Add(userID, user.UnreadMessages);
                            OnUsersMessageCountChanged(counts);
                        }
                    }
                }
                return messages;
            }

            //if (OnChatUserMessageCountChanged != null) OnChatUserMessageCountChanged(userID);

        }


#if !Passport
        private ChatMessage GetChatMessage(ChatMessageProxy proxy)
        {
            ChatMessage message = new ChatMessage();
            message.Content = proxy.Content;
            message.CreateDate = proxy.CreateDate;
            message.CreateIP = proxy.CreateIP;
            message.FromMessageID = proxy.FromMessageID;
            message.IsRead = proxy.IsRead;
            message.IsReceive = proxy.IsReceive;
            message.MessageID = proxy.MessageID;
            message.OriginalContent = proxy.OriginalContent;
            message.TargetUserID = proxy.TargetUserID;
            message.UserID = proxy.UserID;

            return message;
        }

#endif

        public ChatMessageCollection AdminGetChatMessages(int operatorID, int targetUserID, int pageNumber, int pageSize)
        {
            return this.GetChatMessages(operatorID, targetUserID, pageNumber, pageSize, false, false);
        }

        private Guid[] GetNoPermissionRoleIDs(int operatorUserID)
        {
            return PermissionSet.GetNoPermissionTargetRoleIds(operatorUserID, PermissionTargetType.Content);
        }

        private BackendPermissions PermissionSet
        {
            get
            {
                return AllSettings.Current.BackendPermissions;
            }
        }

        public void IgnoreSession(int userID, int targetUserID)
        {
#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                try
                {
                    Globals.PassportClient.PassportService.Chat_IgnoreSession(userID, targetUserID);
                    Thread.Sleep(200);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                }
            }
            else
#endif
            {
                ChatDao.Instance.IgnoreSession(userID, targetUserID);
            }
        }

        public ChatSessionCollection Server_AdminGetSessions(int operatorUserID,  MaxLabs.Passport.Proxy.DataForSearchChatSession filter, int pageNumber)
        {
            ChatSessionFilter f = new ChatSessionFilter();

            f.BeginDate = filter.BeginDate;
            f.Contains = filter.Contains;
            f.EndDate = filter.EndDate;
            f.IsDesc = filter.IsDesc;
            f.PageSize = filter.PageSize;
            f.ShowAll = filter.ShowAll;
            f.UserID = filter.UserID;
            f.Username = filter.Username;

            return AdminGetSessions(operatorUserID, f, pageNumber);

        }

#if !Passport
        private static PassportServerInterface.DataForSearchChatSession GetChatSessionFilterProxy(ChatSessionFilter filter)
        {
            if (filter == null)
                return null;
            PassportServerInterface.DataForSearchChatSession proxy = new PassportServerInterface.DataForSearchChatSession();
            proxy.BeginDate = filter.BeginDate;
            proxy.Contains = filter.Contains;
            proxy.EndDate = filter.EndDate;
            proxy.IsDesc = filter.IsDesc;
            proxy.PageSize = filter.PageSize;
            proxy.ShowAll = filter.ShowAll;
            proxy.UserID = filter.UserID;
            proxy.Username = filter.Username;

            return proxy;
        }
#endif

        public ChatSessionCollection AdminGetSessions(int operatorUserID, ChatSessionFilter filter, int pageNumber)
        {
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                ChatSessionProxy[] sessons = settings.PassportService.Chat_AdminGetSessions(operatorUserID, GetChatSessionFilterProxy(filter), pageNumber);


                ChatSessionCollection result = new ChatSessionCollection();
                foreach (ChatSessionProxy s in sessons)
                {
                    result.Add(GetChatSession(s));
                }

                return result;
            }
            else
#endif
            {
                Guid[] ExcludeRoleIds = PermissionSet.GetNoPermissionTargetRoleIds(operatorUserID, PermissionTargetType.Content);
                return ChatDao.Instance.AdminGetSessions(filter, pageNumber, ExcludeRoleIds);
            }
        }

#if !Passport

        private ChatSession GetChatSession(PassportServerInterface.ChatSessionProxy proxy)
        {
            if (proxy == null)
                return null;

            ChatSession s = new ChatSession();
            s.ChatSessionID = proxy.ChatSessionID;
            s.CreateDate = proxy.CreateDate;
            s.LastMessage = proxy.LastMessage;
            s.OwnerID = proxy.OwnerID;
            s.TotalMessages = proxy.TotalMessages;
            s.UnreadMessages = proxy.UnreadMessages;
            s.UpdateDate = proxy.UpdateDate;
            s.UserID = proxy.UserID;

            return s;
        }

#endif

        public void AdminDeleteSessions(int operatorUserID, IEnumerable<int> sessionIds)
        {
            if (ValidateUtil.HasItems<int>(sessionIds))
            {
#if !Passport
                PassportClientConfig settings = Globals.PassportClient;
                if (settings.EnablePassport)
                {
                    List<int> ids = new List<int>();
                    foreach (int id in sessionIds)
                        ids.Add(id);

                    int[] t = new int[ids.Count];
                    ids.CopyTo(t);

                    try
                    {
                        settings.PassportService.Chat_AdminDeleteSessions(operatorUserID, t);
                    }
                    catch (Exception ex)
                    {
                        ThrowError(new APIError(ex.Message));
                    }
                }
                else
#endif
                {
                    Guid[] excludeRoleIDs = PermissionSet.GetNoPermissionTargetRoleIds(operatorUserID, PermissionTargetType.Content);
                    ChatDao.Instance.AdminDeleteSessions(sessionIds, excludeRoleIDs);
                }
            }
        }

        public ChatSession GetChatSession(int SessionID)
        {
            return ChatDao.Instance.GetChatSession(SessionID);
        }

        public ChatSessionCollection GetChatSessionsWithUnreadMessages(int operatorID, int topCount)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return new ChatSessionCollection();
            }

            if (topCount <= 0)
            {
                return new ChatSessionCollection();
            }
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                ChatSessionProxy[] sessiongs = settings.PassportService.Chat_GetChatSessionsWithUnreadMessages(operatorID, topCount);

                ChatSessionCollection result = new ChatSessionCollection();
                foreach (ChatSessionProxy s in sessiongs)
                {
                    result.Add(GetChatSession(s));
                }
                return result;
            }
            else
#endif
            {
                return ChatDao.Instance.GetChatSessionsWithUnreadMessages(operatorID, topCount);
            }
        }

        public ChatSession GetChatSession(int operatorID, int targetUserID)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return null;
            }

            if (targetUserID <= 0)
            {
                return null;
            }
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                ChatSessionProxy session = settings.PassportService.Chat_GetChatSessionByUserID(operatorID, targetUserID);


                return GetChatSession(session);
            }
            else
#endif
            {
                return ChatDao.Instance.GetChatSession(operatorID, targetUserID);
            }
        }

        /// <summary>
        /// 获取聊天会话，最近聊天的用户将出现在列表顶部
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ChatSessionCollection GetChatSessions(int operatorID, int pageNumber, int pageSize)
        {

            if (!AllSettings.Current.ChatSettings.EnableChatFunction)
                return new ChatSessionCollection();

            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return new ChatSessionCollection();
            }

#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                int totalCount;

                ChatSessionProxy[] sessiongs = settings.PassportService.Chat_GetChatSessions(operatorID, pageNumber, pageSize, out totalCount);

                ChatSessionCollection result = new ChatSessionCollection();
                foreach (ChatSessionProxy s in sessiongs)
                {
                    result.Add(GetChatSession(s));
                }
                result.TotalRecords = totalCount;
                return result;
            }
            else
#endif
            {
                ChatSessionCollection chatSessions = ChatDao.Instance.GetChatSessions(operatorID, pageNumber, pageSize);

                //UserBO.Instance.getsi

                return chatSessions;
            }
        }

        //public ChatMessageCollection GetLastChatMessages(int operatorID, int targetUserID, int lastCount, bool returnTotalRecords, bool setIsRead)
        //{
        //    if (operatorID <= 0)
        //    {
        //        ThrowError(new NotLoginError());
        //        return new ChatMessageCollection();
        //    }

        //    if (targetUserID <= 0)
        //    {
        //        return new ChatMessageCollection();
        //    }

        //    ChatMessageCollection Messages = ChatDao.Instance.GetLastChatMessages(operatorID, targetUserID, lastCount, returnTotalRecords, setIsRead);

        //    ProcessKeyword(Messages, ProcessKeywordMode.TryUpdateKeyword);

        //    return Messages;
        //}


        public ChatMessageCollection GetChatMessages(int operatorID, int targetUserID, int pageNumber, int pageSize, bool updateIsReaded)
        {
            return this.GetChatMessages(operatorID, targetUserID, pageNumber, pageSize, true, updateIsReaded);
        }

        /// <summary>
        /// 获取和某人的聊天记录，如果pageNumber为0则取最后一页
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="targetUserID"></param>
        /// <param name="pageNumber">0表示取最后一页</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private ChatMessageCollection GetChatMessages(int operatorID, int targetUserID, int pageNumber, int pageSize, bool processKeyword, bool updateIsReaded)
        {
            if (!AllSettings.Current.ChatSettings.EnableChatFunction)
                return new ChatMessageCollection();

            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return new ChatMessageCollection();
            }

            if (targetUserID <= 0)
            {
                return new ChatMessageCollection();
            }
            
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                int totalCount;
                ChatMessageProxy[] messages = settings.PassportService.Chat_GetChatMessages(operatorID, targetUserID, pageNumber, pageSize, processKeyword, updateIsReaded, out totalCount);

                ChatMessageCollection result = new ChatMessageCollection();
                foreach (ChatMessageProxy m in messages)
                {
                    result.Add(GetChatMessage(m));
                }
                result.TotalRecords = totalCount;
                return result;
            }
            else
#endif
            {
                ChatMessageCollection Messages = ChatDao.Instance.GetChatMessages(operatorID, targetUserID, pageNumber, pageSize, updateIsReaded);

                if (updateIsReaded)
                {
                    if (OnUsersMessageCountChanged != null)
                    {
                        AuthUser user = UserBO.Instance.GetAuthUser(operatorID);
                        if (user != null)
                        {
                            Dictionary<int, int> counts = new Dictionary<int, int>();
                            counts.Add(operatorID, user.UnreadMessages);
                            OnUsersMessageCountChanged(counts);
                        }
                    }
                }

                if (processKeyword)
                    ProcessKeyword(Messages, ProcessKeywordMode.TryUpdateKeyword);
                else
                    ProcessKeyword(Messages, ProcessKeywordMode.FillOriginalText);
               
                return Messages;
            }
        }

        /// <summary>
        /// Web Service发送消息接口
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="targetUserID"></param>
        /// <param name="contentWithoutEncode"></param>
        /// <param name="ip"></param>
        /// <param name="getNewMessages"></param>
        /// <param name="lastMessageID"></param>
        /// <returns></returns>
        public ChatMessageCollection Server_SendMessage(int operatorID, int targetUserID, string contentWithoutEncode, string ip, bool getNewMessages, int lastMessageID)
        {
            return SendMessage(operatorID, targetUserID, contentWithoutEncode, ip, false, true, lastMessageID);
        }

        public ChatMessageCollection SendMessage(int operatorID, int targetUserID, string contentWithoutEncode, string ip, bool getNewMessages, int lastMessageID)
        {
            return SendMessage(operatorID, targetUserID, contentWithoutEncode, ip, true, true, lastMessageID);
        }
        

        /// <summary>
        /// 发送一个对话消息，并且根据指定参数决定是否需要同时返回最新消息
        /// </summary>
        /// <param name="operatorID">发送者</param>
        /// <param name="targetUserID">接受者</param>
        /// <param name="contentWithoutEncode">未经html编码的内容</param>
        /// <param name="ip">发送者IP</param>
        /// <param name="processContent">是否处理内容</param>
        /// <param name="getNewMessages">是否需要同时返回最新消息</param>
        /// <param name="lastMessageID">如果需要返回最新消息，则传入客户端</param>
        /// <returns></returns>
        public ChatMessageCollection SendMessage(int operatorID, int targetUserID, string contentWithoutEncode, string ip,bool processContent, bool getNewMessages, int lastMessageID)
        {

#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                ChatMessageProxy[] chatMessages;
                APIResult result = null;
                string content = ProcessChatContent(operatorID, contentWithoutEncode);
                content = content.Replace("{$root}", Globals.FullAppRoot);

                try
                {
                    result = settings.PassportService.Chat_SendMessage(operatorID, targetUserID, content, ip, getNewMessages, lastMessageID, out chatMessages);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return new ChatMessageCollection();
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                    {
                        ThrowError(new CustomError("远程服务器错误， 请稍后重试." + (result.Messages.Length > 0 ? result.Messages[0] : string.Empty)));
                    }
                    return new ChatMessageCollection();
                }
                else if (result.IsSuccess == false)
                {
                    ThrowError<CustomError>(new CustomError("", result.Messages[0]));
                    return new ChatMessageCollection();
                }

                ChatMessageCollection messages = new ChatMessageCollection();
                foreach (ChatMessageProxy m in chatMessages)
                {
                    messages.Add(GetChatMessage(m));
                }
                return messages;
            }
            else
#endif
            {

                if (!AllSettings.Current.ChatSettings.EnableChatFunction)
                {
                    return new ChatMessageCollection();
                }

                if (operatorID <= 0)
                {
                    ThrowError(new NotLoginError());
                    return new ChatMessageCollection();
                }

                if (targetUserID <= 0)
                {
                    ThrowError(new UserNotExistsError("targetUserID", targetUserID));
                    return new ChatMessageCollection();
                }

                if (operatorID == targetUserID)
                {
                    ThrowError(new CustomError("targetUserID", "您不能和自己交谈"));  //临时的
                    return new ChatMessageCollection();
                }

                if (string.IsNullOrEmpty(contentWithoutEncode) || (contentWithoutEncode = contentWithoutEncode.Trim()) == string.Empty)
                {
                    ThrowError(new EmptyMessageContentError("contentWithoutEncode"));
                    return new ChatMessageCollection();
                }

                if (contentWithoutEncode.Length > MaxContentLength)
                {
                    ThrowError(new MessageContentLengthError("contentWithoutEncode", contentWithoutEncode, MaxContentLength));
                    return new ChatMessageCollection();
                }

                if (FriendBO.Instance.InMyBlacklist(targetUserID, operatorID))
                {
                    ThrowError(new InBlackListError());
                    return new ChatMessageCollection();
                }



                //if (AllSettings.Current.UserPermissionSet.Can(operatorID, UserPermissionSet.Action.UseMessage) == false)
                //{
                //    ThrowError(new NoPermissionUseMessageError());
                //    return false;
                //}
                string content = contentWithoutEncode;
                if (processContent)
                {
                    content = ProcessChatContent(operatorID, content);
                }

                ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

                string keyword = null;

                if (keywords.BannedKeywords.IsMatch(content, out keyword))
                {
                    ThrowError(new ChatMessageBannedKeywordsError("content", keyword));
                    return null;
                }
                ChatMessageCollection newMessages = ChatDao.Instance.SendMessage(operatorID, targetUserID, content, ip, getNewMessages, lastMessageID);

                AuthUser user = UserBO.Instance.GetUserFromCache<AuthUser>(targetUserID);
                if (user != null)
                    user.UnreadMessages++;
                if (OnUsersMessageCountChanged != null)
                {
                    if (user == null)
                        user = UserBO.Instance.GetAuthUser(targetUserID);
                    if (user != null)
                    {
                        Dictionary<int, int> counts = new Dictionary<int, int>();
                        counts.Add(user.UserID, user.UnreadMessages);

                        OnUsersMessageCountChanged(counts);
                    }
                }

                ProcessKeyword(newMessages, ProcessKeywordMode.TryUpdateKeyword);

                return newMessages;
            }
        }

        private string ProcessChatContent(int senderUserID, string contentNoEncode)
        {

            string content = contentNoEncode;

            content = StringUtil.HtmlEncode(content);

            content = EmoticonParser.ParseToHtml(senderUserID, content
                , AllSettings.Current.ChatSettings.EnableUserEmoticon
                , AllSettings.Current.ChatSettings.EnableDefaultEmoticon
                );

            int replaceCount = 0;

            content = Pool<SplitLineRegex>.Instance.Replace(content, delegate(Match match)
            {
                string result;

                if (replaceCount < MaxContentRows - 1)
                    result = "<br />";
                else
                    result = string.Empty;

                replaceCount++;

                return result;
            }, MaxContentRows);

            if (replaceCount >= (MaxContentRows - 1))
            {
                ThrowError(new MessageContentRowsError("contentWithoutEncode", contentNoEncode, MaxContentRows));
                return string.Empty;
            }
            return content;
        }



        //public void ProcessKeyword(ChatMessageCollection chatMessages)
        //{
        //    foreach (ChatMessage message in chatMessages)
        //    {
        //        //message
        //    }
        //    //chatMessages.Add();
        //}

        public bool DeleteChatSession(int operatorID, int targetUserID)
        {
            bool success = DeleteChatSessions(operatorID, new int[1] { targetUserID });

            //if (success)
                //if (OnChatUserMessageCountChanged != null) OnChatUserMessageCountChanged(operatorID);

            return success;
        }

        public bool DeleteChatSessions(int operatorID, IEnumerable<int> targetUserIds)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            //if (ValidateUtil.HasItems(targetUserIds) == false)
            //{
            //    ThrowError(new NotSelectedMessagesError("targetUserIds"));
            //    return false;
            //}
            
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {

                List<int> ids = new List<int>();
                foreach (int id in targetUserIds)
                    ids.Add(id);

                int[] t = new int[ids.Count];
                ids.CopyTo(t);

                APIResult result = null;

                try
                {
                    result = settings.PassportService.Chat_DeleteChatSessions(operatorID, t);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                    return false;
                }
                else if (result.IsSuccess == false)
                {
                    ThrowError<CustomError>(new CustomError("", result.Messages[0]));
                    return false;
                }

                return true;
            }
            else
#endif
            {
                bool success = ChatDao.Instance.DeleteChatSessions(operatorID, targetUserIds);

                if (success)
                {
                    if (OnUsersMessageCountChanged != null)
                    {
                        AuthUser user = UserBO.Instance.GetAuthUser(operatorID);
                        if (user != null)
                        {
                            Dictionary<int, int> counts = new Dictionary<int, int>();
                            counts.Add(operatorID, user.UnreadMessages);
                            OnUsersMessageCountChanged(counts);
                        }
                    }
                }
                return success;
            }
        }

        public void ResetUserUnreadMessageCounts(Dictionary<int, int> usersUnreadMessageCount)
        {
            if (OnUsersMessageCountChanged != null)
                OnUsersMessageCountChanged(usersUnreadMessageCount);
        }

        public void ProcessKeyword(ChatMessage chatMessage, ProcessKeywordMode mode)
        {
            if (chatMessage == null)
                return;

            ChatMessageCollection messages = new ChatMessageCollection();

            messages.Add(chatMessage);

            ProcessKeyword(messages, mode);
        }

        public void ProcessKeyword(ChatMessageCollection messages, ProcessKeywordMode mode)
        {
            if (messages.Count == 0)
                return;

            KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            bool needProcess = false;

            //更新关键字模式，只在必要的情况下才取恢复信息并处理
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                needProcess = keyword.NeedUpdate<ChatMessage>(messages);
            }
            //填充原始内容模式，始终都要取恢复信息，但不处理
            else
            {
                needProcess = true;
            }

            if (needProcess)
            {
                RevertableCollection<ChatMessage> messagesWithReverter = ChatDao.Instance.GetChatMessageWithReverters(messages.GetKeys());

                if (messagesWithReverter != null)
                {
                    if (keyword.Update(messagesWithReverter))
                    {
                        ChatDao.Instance.UpdateChatMessageKeywords(messagesWithReverter);
                    }

                    //将新数据填充到旧的列表
                    messagesWithReverter.FillTo(messages);
                }
            }
        }


        public void Client_UpdateUserUnReadMessageCount(Dictionary<int,int> counts)
        {
            foreach (KeyValuePair<int, int> count in counts)
            {
                AuthUser user = UserBO.Instance.GetUserFromCache<AuthUser>(count.Key);
                if (user != null)
                    user.UnreadMessages = count.Value;
            }
        }
    }
}