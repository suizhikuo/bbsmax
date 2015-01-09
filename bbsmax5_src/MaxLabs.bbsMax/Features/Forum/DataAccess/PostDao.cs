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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class PostDaoV5 : DaoBase<PostDaoV5>
    {

        public abstract PostAuthorExtendInfo GetPostAuthorExtendInfo(int userID, DataAccessLevel dataAccessLevel);


        public abstract BasicThread GetThread(int threadID);

        public abstract void GetThreadWithReplies(int threadID, int totalCount, bool getThread, int pageNumber, int pageSize, out BasicThread thread, out PostCollectionV5 replies);

        public abstract bool GetThread(int threadID, bool normalOnly, int totalCount, bool getThread, bool getReplies, int postUserID, int pageIndex, int pageSize, out BasicThread thread, out PostCollectionV5 replies, out int repliesCountForUser);

        public abstract ThreadCollectionV5 GetThreads(int? forumID, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, int offSet, bool includeStick, int pageNumber, int pageSize, ref int totalCount);

        public abstract int GetThreadCount(int forumID, DateTime? beginDate, DateTime? endDate, bool includeStick);

        public abstract ThreadCollectionV5 GetMyParticipantThreads(int userID, int pageNumber, int pageSize, out int totalThreads);

        public abstract ThreadCollectionV5 GetMyThreads(int userID, bool isApproved, int pageNumber, int pageSize, int offset, out int totalThreads);

        public abstract ThreadCollectionV5 GetStickThreads(int forumID);

        public abstract ThreadCollectionV5 GetStickThreads(IEnumerable<int> forumIDs);

        public abstract ThreadCollectionV5 GetGlobalThreads();

        public abstract ThreadCollectionV5 GetThreadsByThreadCatalogID(int forumID, int threadCatalogID, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, int offset, ref int totalThreads);

        public abstract ThreadCollectionV5 GetValuedThreads(IEnumerable<int> forumIDs, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, bool returnTotalThreads, bool includeStick, int offset, out int totalThreads);

        public abstract ThreadCollectionV5 GetThreads(int forumID, ThreadType threadType, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, bool returnTotalThreads, int offset, out int totalThreads);

        public abstract ThreadCollectionV5 GetThreads(ThreadSortField sortType, int count, IEnumerable<int> forumIDs);

        public abstract ThreadCollectionV5 GetHotThreads(IEnumerable<int> forumIDs, int reqireReplies, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, bool returnTotalThreads, out int totalThreads);

        public abstract ThreadCollectionV5 GetNewThreads(IEnumerable<int> forumIDs, int totalThreads, int pageNumber, int pageSize);

        public abstract ThreadCollectionV5 GetNewThreads(IEnumerable<int> forumIDs, int count);

        public abstract ThreadCollectionV5 GetTopViewThreads(IEnumerable<int> forumIDs, int count, DateTime? beginDate, DateTime? endDate);

        public abstract ThreadCollectionV5 GetUserQuestionThreads(int userID, int count, int exceptThreadID);

        public abstract void GetPolemizeWithReplies(int threadID, PostType? postType, int pageNumber, int pageSize, bool getExtendedInfo, bool getThread, bool getThreadContent, bool checkThreadType, ref BasicThread thread, out PostCollectionV5 posts, ref ThreadType threadType, out int totalCount);

        public abstract bool DeletePosts(int forumID, int threadID, IEnumerable<int> postIdentities, int userID, bool isDeleteAnyPost);

        public abstract ThreadCollectionV5 GetThreads(IEnumerable<int> threadids);

        public abstract bool DeleteThreads(int forumID, ThreadStatus threadStatus, IEnumerable<int> threadIdentities);

        public abstract bool SetThreadsStatus(int forumID, IEnumerable<int> stickForumIDs, IEnumerable<int> threadIdentities, ThreadStatus threadLocation, StickSortType? sortType);

		public abstract bool SetThreadsLock(int forumID, IEnumerable<int> threadIdentities, bool isLock);
        public abstract void SetThreadExtendData(BasicThread thread);

        public abstract void DeleteTopicStatus(IEnumerable<int> ids);

        public abstract void DeleteTopicStatus(IEnumerable<int> threadIDs, TopicStatuType type);

        public abstract void CreateTopicStatus(IEnumerable<int> threadIDs, TopicStatuType type, DateTime endDate);

        public abstract ThreadCollectionV5 GetThreads(int pageNumber, TopicFilter filter, Guid[] excludeRoleIDs, ref int totalCount);

        public abstract DeleteResult DeleteSearchTopics(TopicFilter filter, IEnumerable<Guid> excludeRoleIDs, bool getDeleteResult, int topCount, out int deletedCount, out List<int> threadIDs);

        public abstract bool SetThreadNotUpdateSortOrder(int forumID, IEnumerable<int> threadIds, bool updateSortOrder);

        public abstract PostCollectionV5 GetPosts(int threadID);

        public abstract PostCollectionV5 GetPosts(IEnumerable<int> postIds);

        public abstract bool JoinThreads(int oldThreadID,int newThreadID, bool isKeepLink);

        public abstract ThreadCollectionV5 GetThreadsByStatus(ThreadStatus status, int? forumID, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, int pageNumber, int pageSize, ref int totalCount);

        public abstract Dictionary<BasicThread, PostCollectionV5> GetUnapprovedPosts(int forumID);

        public abstract ThreadCollectionV5 GetUnapprovedPostThreads(int? forumID, int? userID, int pageNumber, int pageSize);

        public abstract void GetUnapprovedPostThread(int threadID, int? userID, int pageNumber, int pageSize, out BasicThread thread, out PostCollectionV5 posts, out int totalCount);

        /// <summary>
        /// 加入/解除精华
        /// </summary>
        /// <param name="threadIdentities"></param>
        /// <returns></returns>
        public abstract bool SetThreadsValued(int forumID, IEnumerable<int> threadIdentities, bool isValued);


        /// <summary>
        /// 移动帖子
        /// </summary>
        /// <param name="OldForumID"></param>
        /// <param name="NewForumID"></param>
        /// <param name="IsKeepLink">是否在原版块保持链接</param>
        /// <returns></returns>
        public abstract bool MoveThreads(int oldForumID, int newForumID,IEnumerable<int>  threadIdentities, bool isKeepLink);

        /// <summary>
        /// 提升主题
        /// </summary>
        /// <param name="threadID"></param>
        /// <returns></returns>
        public abstract bool SetThreadsUp(int forumID, IEnumerable<int> threadIdentities);


        /// <summary>
        /// 添加标题样式
        /// </summary>
        /// <param name="threadIdentities"></param>
        /// <param name="Style"></param>
        /// <returns></returns>
        public abstract bool SetThreadsSubjectStyle(int forumID, IEnumerable<int> threadIdentities, string style);


        /// <summary>
        /// 更改主题分类
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="threadIDs"></param>
        /// <param name="threadCatalogID"></param>
        /// <returns></returns>
        public abstract bool UpdateThreadCatalog(int forumID, IEnumerable<int> threadIDs, int threadCatalogID);

        public abstract List<int> ApprovePosts(IEnumerable<int> postIDs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount">回复总数 加 主题内容</param>
        /// <param name="getExtendedInfo"></param>
        /// <returns></returns>
        public abstract void GetPosts(int threadID, bool onlyNormal, int pageNumber, int pageSize, int? totalCount, bool getExtendedInfo, bool getThread, bool getThreadContent, bool checkThreadType, ref BasicThread thread, out PostCollectionV5 posts, ref ThreadType threadType);

        public abstract void GetUserPosts(int threadID, int userID, int pageNumber, int pageSize, bool getExtendedInfo, bool getThread, bool checkThreadType, ref BasicThread thread, out PostCollectionV5 posts, ref ThreadType threadType, out int totalCount);
        
        public abstract void GetPosts(int threadID, int totalReplies, int getOldCount, int getNewCount, ref BasicThread thread, out PostV5 threadContent, out PostCollectionV5 topPosts);

        public abstract PostV5 GetPost(int postID, bool getExtendedInfo);

        public abstract PostCollectionV5 GetPosts(int pageNumber, PostFilter filter, Guid[] excludeRoleIDs, ref int totalCount);

        public abstract DeleteResult DeleteSearchPosts(PostFilter filter, IEnumerable<Guid> excludeRoleIDs, bool getDeleteResult, int topCount, out int deletedCount, out List<int> threadIDs);
        

        public abstract PostCollectionV5 GetUserPosts(int userID, DateTime beginDate, DateTime endDate);

        public abstract PostV5 GetThreadFirstPost(int threadID, bool getExtendedInfo);

        public abstract Dictionary<int, PostV5> GetThreadContents(IEnumerable<int> threadIDs);

        public abstract List<int> GetPostUserIDsFormThreads(IEnumerable<int> threadIDs);

        public abstract bool SplitThread(int threadID, IEnumerable<int> postIdentities, string newSubject);


        #region 附件

        public abstract AttachmentCollection GetAttachments(int userID, IEnumerable<int> attachmentIDs);

        public abstract AttachmentCollection GetUserTodayAttachments(int userID);

        public abstract void GetUserTodayAttachmentInfo(int userID, int? excludePostID, out int totalCount, out long totalFileSize);

        /// <summary>
        /// 通过postID获取附件列表
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public abstract AttachmentCollection GetAttachments(int postID);

        public abstract AttachmentCollection GetAttachments(int operatorUserID, DateTime? beginDate, DateTime? endDate, string keyword, int pageNumber, int pageSize, ExtensionList fileTypes);


        public abstract Attachment GetAttachment(int attachmentID, bool updateTotalDownloads, out int threadID);

        public abstract void GetAttachment(int diskFileID, int postID, out Attachment attachment, out PostV5 post, out BasicThread thread);

        public abstract bool CreateAttachmentExchange(int attachmentID, int userID, int price);

        public abstract bool IsBuyedAttachment(int userID, int attachmentID);

        public abstract AttachmentExchangeCollection GetAttachmentExchanges(int attachmentID, int pageNumber, int pageSize, out int totalCount, out int totalSellMoney);

        public abstract ThreadExchangeCollection GetThreadExchanges(int threadID, int pageNumber, int pageSize, out int totalCount, out int totalSellMoney);


        public abstract AttachmentCollection GetAttachments(int pageNumber, AttachmentFilter filter, Guid[] excludeRoleIDs, ref int totalCount);

        public abstract void DeleteAttachments(int forumID, IEnumerable<int> attachmentIDs, IEnumerable<Guid> excludeRoleIDs, out List<int> threadIDs);

        public abstract void DeleteSearchAttachments(AttachmentFilter filter, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount, out List<int> threadIDs);
        
        #endregion

        #region 添加  更新

        public abstract bool CreateThread(int forumID, int threadCatalogID, ThreadStatus threadStatus, int iconID
            , string subject, string subjectStyle, int price, int postUserID, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
            , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
            , ThreadAttachType attachType, string words, out BasicThread thread, out PostV5 post, out int totalThreads, out int totalPosts, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs);

        public abstract bool CreatePoll(string pollItems, int pollMultiple, bool pollIsAlwaysEyeable, DateTime pollExpiresDate
            , int forumID, int threadCatalogID, ThreadStatus threadStatus, int iconID
            , string subject, string subjectStyle, int postUserID, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
            , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
            , ThreadAttachType attachType, string words, out BasicThread thread, out PostV5 post, out int totalThreads, out int totalPosts, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs);
        

        public abstract bool CreateQuestion(int questionReward, int questionRewardCount, bool questionIsAlwaysEyeable, DateTime questionExpiresDate
           , int forumID, int threadCatalogID, ThreadStatus threadStatus, int iconID
           , string subject, string subjectStyle, int postUserID, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
           , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
           , ThreadAttachType attachType, string words, out BasicThread thread, out PostV5 post, out int totalThreads, out int totalPosts, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs);
        

        public abstract bool CreatePolemize(string agreeViewPoint, string againstViewPoint, DateTime polemizeExpiresDate
           , int forumID, int threadCatalogID, ThreadStatus threadStatus, int iconID
           , string subject, string subjectStyle, int postUserID, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
           , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
           , ThreadAttachType attachType, string words, out BasicThread thread, out PostV5 post, out int totalThreads, out int totalPosts, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs);

        public abstract bool UpdateThread(int threadID, int postID, bool isApproved, int threadCatalogID, int iconID
            , string subject, int price, int lastEditorID, string lastEditorName, string content, bool enableHtml
            , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
            , ThreadAttachType attachType, string words, out BasicThread thread, out PostV5 post, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs);

        public abstract bool UpdatePostContent(int postID, string content);

        public abstract bool CreatePost(int threadID, bool getPost, bool isApproved, PostType postType, int iconID, string subject, string content
            , bool enableHtml, bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, int forumID, int postUserID, string userName
            , string ipAddress, int parentID, bool updateSortOrder, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
            , BasicThread thread
            , out PostV5 post, out int totalPosts, out List<int> AttachmentIDs, out Dictionary<string, int> fileIDs, out bool threadEnableReplyNotice);

        public abstract bool UpdatePost(int postID, bool getExtendedInfo, bool isApproved, int iconID
           , string subject, int lastEditorID, string lastEditorName, string content, bool enableHtml
           , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
           , out PostV5 post, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs);


        public abstract int RepairTotalReplyCount(int threadID);

        #endregion

        public abstract int SetPostLoveHate(int userID, int postID, bool isLove, bool canSetMore, out int threadID);

        public abstract int SetThreadRank(int threadID, int userID, int addRank, out int threadRank);

        public abstract ThreadRankCollection GetThreadRanks(int threadID, int pageNumber, int pageSize, out int totalCount);

        public abstract bool CreateThreadExchange(int threadID, int userID, int price);

        public abstract bool IsBuyedThread(int userID, int threadID);

        public abstract int Vote(IEnumerable<int> itemIDs, int threadID, int userID, string nickName, PollThreadV5 poll);

        public abstract PollItemDetailsCollectionV5 GetPollItemDetails(int threadID);

        public abstract int Polemize(int threadID, int userID, ViewPointType viewPointType, PolemizeThreadV5 polemize);

        public abstract int FinalQuestion(int threadID, int bestPostID, Dictionary<int, int> rewards, QuestionThread question);

        public abstract int VoteQuestionBestPost(int threadID, int userID, bool isUseful, QuestionThread question);

        public abstract bool IsRepliedThread(int threadID, int userID);

        public abstract int CreatePostMark(int postID, int userID, string username, DateTime createDate, int[] points, string reason, out PostMark postMark);

        public abstract PostMarkCollection GetUserTodayPostMarks(int userID);

        public abstract void CancelRates(int postID, IEnumerable<int> postMarkIDs);

        public abstract PostMarkCollection GetPostMarks(IEnumerable<int> postMarkIDs);

        public abstract PostMarkCollection GetPostMarks(int postID, int pageNumber, int pageSize, out int totalCount);

        public abstract void CreateThreadManageLog(int userID, string username, string realname, string ipAddress, ModeratorCenterAction actionType
            , IEnumerable<int> postUserIDs
            , int forumID, IEnumerable<int> threadIDs, IEnumerable<string> subjects, string reason, bool isPublic, out string threadLog);

        public abstract ThreadManageLogCollectionV5 GetThreadManageLogs(int threadID);

        public abstract StickThreadCollection GetAllStickThreadForums();

        public abstract void ClearSearchResult();

        public abstract ThreadCollectionV5 GetThreadsByPostCreateDate(DateTime postCreateDate);

        //public abstract void DeleteTopicStatus(IEnumerable<int> ids);

        public abstract void UpdateThreadViews(Dictionary<int, int> updateList);

        public abstract bool JudgementThreads(IEnumerable<int> threadIds, int forumID, int judgementID);

        public abstract bool UpdatePostsShielded(IEnumerable<int> postIDs, bool IsShielded);

        public abstract bool ReCountTopicsAndPosts(bool recountToday, bool recountYestoday);

        public abstract bool StartPostFullTextIndex();

        public abstract bool StopPostFullTextIndex();


        public abstract void SearchTopics(Guid searchID, int pageSize, int pageNumber, SearchType searchType, int maxResult, List<int> canVisitForumIDs, List<int> allForumIDs, out int totalCount, out string keyword, out SearchMode mode, out ThreadCollectionV5 threads, out PostCollectionV5 posts);

        public abstract Guid SearchTopics(int operatorUserID, string ip, List<int> forumIDs, List<int> canVisitForumIDs, List<int> allForumIDs, IEnumerable<string> keyword, int targetUserID, SearchMode mode, SearchType searchType, DateTime? postDate, bool isBefore, bool isDesc, int maxResultCount, int intervalTime);

        public abstract void CheckThreads(int operatorUserID, IEnumerable<int> threadIDs, out List<int> buyedIds, out List<int> repliedIDs);

        public abstract void UpdatePostsForumID(IEnumerable<int> postIDs, int forumID);

        public abstract void SetThreadImage(int threadID, int attchmentID, string imageUrl, int imageCount);
        public abstract void DeleteThreadImage(int threadID);

        #region 关键字 ==================================

        public abstract Revertable2Collection<PostV5> GetPostsWithReverters(IEnumerable<int> postIDs);

        public abstract void UpdatePostKeywords(Revertable2Collection<PostV5> processlist);

        public abstract RevertableCollection<BasicThread> GetThreadWithReverters(IEnumerable<int> threadIDs);

        public abstract void UpdateThreadKeywords(RevertableCollection<BasicThread> processlist);

        #endregion
       
    }
}