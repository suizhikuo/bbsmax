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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;


namespace MaxLabs.bbsMax.Entities
{
    /*
    #region  旧的不用了

    public class ThreadManageLog : EntityBase
    {
        private int userID;
        private string userName;
        private string nickName;
        private string iPAddress;
        private int postUserID;
        private List<int> postUserIDs;
        private ModeratorCenterAction actionType;
        private int forumID;
        private int threadID;
        private List<int> threadIDs;
        private string threadSubject;
        private List<string> threadSubjects;
        private string reason;
        private DateTime createDate;
        private bool isPublic;

        public ThreadManageLog()
        { }
        public ThreadManageLog(int p_logID, int p_userID, string p_userName, string p_nickName, string p_iPAddress, int p_postUserID, ModeratorCenterAction p_actionType, int p_forumID, int p_threadID, string p_threadSubject, string p_reason, DateTime p_createDate,bool p_isPublic)
        {
            ID = p_logID;
            userID = p_userID;
            userName = p_userName;
            nickName = p_nickName;
            iPAddress = p_iPAddress;
            postUserID = p_postUserID;
            actionType = p_actionType;
            forumID = p_forumID;
            threadID = p_threadID;
            threadSubject = p_threadSubject;
            reason = p_reason;
            createDate = p_createDate;
            isPublic = p_isPublic;
        }

        public int LogID
        {
            get { return ID; }
            set { ID = value; }
        }
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        public string NickName
        {
            get { return nickName; }
            set { nickName = value; }
        }
        public string IPAddress
        {
            get { return iPAddress; }
            set { iPAddress = value; }
        }
        public int PostUserID
        {
            get { return postUserID; }
            set { postUserID = value; }
        }
        public List<int> PostUserIDs
        {
            get { return postUserIDs; }
            set { postUserIDs = value; }
        }
        public ModeratorCenterAction ActionType
        {
            get { return actionType; }
            set { actionType = value; }
        }
        public int ForumID
        {
            get { return forumID; }
            set { forumID = value; }
        }
        public List<int> ThreadIDs
        {
            get { return threadIDs; }
            set { threadIDs = value; }
        }
        public int ThreadID
        {
            get { return threadID; }
            set { threadID = value; }
        }
        public string Reason
        {
            get { return reason; }
            set { reason = value; }
        }
        public System.DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }
        // 通过版块ID获取版块名称
        const string cache_key_ForumNameInThreadLog = "ForumNameCacheInThreadLog_";
        public string ForumName
        {
            get
            {
                string retStr = string.Empty;
                try
                {
                    if (!CacheUtil.TryGetValue<string>(cache_key_ForumNameInThreadLog + this.forumID, out retStr))
                    {
                        Forum forum = ForumManager.GetForum(this.forumID);
                        //retStr = "<a href=\"" +
                        //    UrlHelper.GetUrl("Forum", forum.CodeName, "normal", 1) +
                        //    "\">" + forum.ForumName + "</a>";
                        retStr = forum.ForumName;
                        CacheUtil.Set<string>(cache_key_ForumNameInThreadLog + this.forumID, retStr);
                    }
                }
                catch
                {
                    retStr = "获取版块出错";
                }
                return retStr;
            }
        }
        // 通过主题ID获取主题标题名称
        const string cache_key_ThreadNameInThreadLog = "ForumNameCacheInThreadLog_";
        public string ThreadSubject
        {
            get
            {
                return threadSubject;
                //string retStr = string.Empty;
                //try
                //{
                //    if(!CacheManager.TryGetValue<string>(cache_key_ThreadNameInThreadLog+this.threadID,out retStr))
                //    {
                //        Thread thread = PostManager.GetThread(this.threadID);
                //        //string forumCodeName = ForumManager.GetForum(thread.ForumID, true).CodeName;
                //        //retStr = "<a href=\"" +
                //        //    UrlHelper.GetUrl("Thread", forumCodeName, threadID, 1) +
                //        //    "\">" + thread.Subject + "</a>";
                //        retStr = thread.Subject;
                //        if (retStr.Length > 13)
                //        {
                //            retStr = retStr.Substring(0, 12) + "....";
                //        }
                //    }
                //}
                //catch
                //{
                //    retStr = "获取主题标题出错";
                //}
                //return retStr;
            }
            set
            {
                threadSubject = value;
            }
        }
        public List<string> ThreadSubjects
        {
            get
            {
                return threadSubjects;
            }
            set
            {
                threadSubjects = value;
            }
        }
        // 通过ActionType获得动作名称
        public string ActionTypeName(Thread thread)
        {
            return PostManager.GetActionName(this.actionType, thread);

        }

        public bool IsPublic
        {
            get
            {
                return isPublic;
            }
            set
            {
                isPublic = value;
            }
        }
    }

    #endregion

    */





    public class ThreadManageLogV5 : IPrimaryKey<int>
    {
        public ThreadManageLogV5()
        { }
        public ThreadManageLogV5(DataReaderWrap readerWrap)
        {
            LogID = readerWrap.Get<int>("LogID");
            UserID = readerWrap.Get<int>("UserID");
            UserName = readerWrap.Get<string>("UserName");
            NickName = readerWrap.Get<string>("NickName");
            IPAddress = readerWrap.Get<string>("IPAddress");
            PostUserID = readerWrap.Get<int>("PostUserID");
            ActionType = readerWrap.Get<ModeratorCenterAction>("ActionType");
            ForumID = readerWrap.Get<int>("ForumID");
            ThreadID = readerWrap.Get<int>("ThreadID");
            Reason = readerWrap.Get<string>("Reason");
            CreateDate = readerWrap.Get<DateTime>("CreateDate");
            ThreadSubject = readerWrap.Get<string>("ThreadSubject");
            IsPublic = readerWrap.Get<bool>("IsPublic");
        }

        public int LogID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string IPAddress { get; set; }
        public int PostUserID { get; set; }
        public ModeratorCenterAction ActionType { get; set; }
        public int ForumID { get; set; }
        public int ThreadID { get; set; }
        public string Reason { get; set; }
        public DateTime CreateDate { get; set; }
        ////// 通过版块ID获取版块名称
        ////const string cache_key_ForumNameInThreadLog = "ForumNameCacheInThreadLog_";
        ////public string ForumName
        ////{
        ////    get
        ////    {
        ////        string retStr = string.Empty;
        ////        try
        ////        {
        ////            if (!CacheUtil.TryGetValue<string>(cache_key_ForumNameInThreadLog + this.forumID, out retStr))
        ////            {
        ////                Forum forum = ForumManager.GetForum(this.forumID);
        ////                //retStr = "<a href=\"" +
        ////                //    UrlHelper.GetUrl("Forum", forum.CodeName, "normal", 1) +
        ////                //    "\">" + forum.ForumName + "</a>";
        ////                retStr = forum.ForumName;
        ////                CacheUtil.Set<string>(cache_key_ForumNameInThreadLog + this.forumID, retStr);
        ////            }
        ////        }
        ////        catch
        ////        {
        ////            retStr = "获取版块出错";
        ////        }
        ////        return retStr;
        ////    }
        ////}
        // 通过主题ID获取主题标题名称
        public string ThreadSubject { get; set; }
        // 通过ActionType获得动作名称
        public string ActionTypeName(BasicThread thread)
        {
            return PostBOV5.Instance.GetActionName(ActionType, thread);

        }

        public bool IsPublic { get; set; }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return LogID;
        }

        #endregion
    }

    public class ThreadManageLogCollectionV5 : EntityCollectionBase<int, ThreadManageLogV5>
    {
        public ThreadManageLogCollectionV5()
        { }

        public ThreadManageLogCollectionV5(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                ThreadManageLogV5 threadManageLog = new ThreadManageLogV5(readerWrap);

                this.Add(threadManageLog);
            }
        }
    }
}