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
using MaxLabs.bbsMax.Providers;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    public class BasicThread : IPrimaryKey<int>, ITextRevertable, IFillSimpleUsers
    {
        public BasicThread()
        { }

        public BasicThread(DataReaderWrap readerWrap)
        {
            ThreadID = readerWrap.Get<int>("ThreadID");
            ForumID = readerWrap.Get<int>("ForumID");
            ThreadCatalogID = readerWrap.Get<int>("ThreadCatalogID");
            ThreadType = (ThreadType)readerWrap.Get<byte>("ThreadType");
            IconID = readerWrap.Get<int>("IconID");
            Subject = readerWrap.Get<string>("Subject");
            SubjectStyle = readerWrap.Get<string>("SubjectStyle");
            TotalReplies = readerWrap.Get<int>("TotalReplies");
            TotalViews = readerWrap.Get<int>("TotalViews");
            TotalAttachments = readerWrap.Get<int>("TotalAttachments");
            Price = readerWrap.Get<int>("Price");
            Rank = (int)readerWrap.Get<byte>("Rank");
            PostUserID = readerWrap.Get<int>("PostUserID");
            PostUsername = readerWrap.Get<string>("PostNickName");
            LastReplyUserID = readerWrap.Get<int>("LastPostUserID");
            LastReplyUsername = readerWrap.Get<string>("LastPostNickName");
            IsValued = readerWrap.Get<bool>("IsValued");
            IsLocked = readerWrap.Get<bool>("IsLocked");
            Perorate = readerWrap.Get<string>("Perorate");
            CreateDate = readerWrap.Get<DateTime>("CreateDate");
            UpdateDate = readerWrap.Get<DateTime>("UpdateDate");
            SortOrder = readerWrap.Get<long>("SortOrder");
            UpdateSortOrder = readerWrap.Get<bool>("UpdateSortOrder");
            ThreadLog = readerWrap.Get<string>("ThreadLog");
            JudgementID = readerWrap.Get<int>("JudgementID");
            KeywordVersion = readerWrap.Get<string>("KeywordVersion");
            LastPostID = readerWrap.Get<int>("LastPostID");
            ShareCount = readerWrap.Get<int>("ShareCount");
            CollectionCount = readerWrap.Get<int>("CollectionCount");

            ContentID = readerWrap.Get<int>("ContentID");
            ThreadStatus = (ThreadStatus)readerWrap.Get<byte>("ThreadStatus");
            AttachmentType = (ThreadAttachType)readerWrap.Get<byte>("AttachmentType");

            PostedCount = readerWrap.Get<int>("PostedCount");
            Words = readerWrap.Get<string>("Words");
            //ExtendData = readerWrap.Get<string>("ExtendData");

            //ThreadStatus = (ThreadStatus)(SortOrder / 1000000000000000);
            //try
            //{
            //    int isClosedIndex = readerWrap.IndexOf("IsClosed");

            //    if (isClosedIndex == -1)
            //        IsClosed = false;
            //    else
            //        IsClosed = readerWrap.Get<int>(isClosedIndex) == 1;
            //}
            //catch
            //{
            //    IsClosed = false;
            //}

            string extendData = readerWrap.Get<string>("ExtendData");
            if (!string.IsNullOrEmpty(extendData))
            {
                try
                {
                    SetExtendData(extendData);
                    ExtendDataIsNull = false;
                }
                catch { ExtendDataIsNull = true; }
            }
            else
            {
                ExtendDataIsNull = true;
            }
        }

        public int ThreadID { get; set; }

        public int ForumID { get; set; }

        public int ThreadCatalogID { get; set; }

        public ThreadType ThreadType { get; set; }

        public int IconID { get; set; }

        private string m_Subject = null;

        public string Subject
        {
            get { return m_Subject; }
            set
            {
                m_SubjectText = null;
                m_Subject = value;
            }
        }

        public string SubjectStyle { get; set; }

        public int TotalReplies { get; set; }

        public int TotalViews { get; set; }

        public int TotalAttachments { get; set; }

        public int Price { get; set; }

        public int Rank { get; set; }

        public int PostUserID { get; set; }

        public string PostUsername { get; set; }

        public int LastReplyUserID { get; set; }

        public string LastReplyUsername { get; set; }

        public bool IsValued { get; set; }

        public bool IsLocked { get; set; }

        public string Perorate { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public long SortOrder { get; set; }

        public bool UpdateSortOrder { get; set; }

        public string ThreadLog { get; set; }

        public int JudgementID { get; set; }


        public int LastPostID { get; set; }

        public int ContentID { get; set; }

        public int ShareCount { get; set; }

        public int CollectionCount { get; set; }

        public int PostedCount { get; set; }

        public string Words { get; set; }

        //public string ExtendData { get; set; }

        private bool m_ExtendDataIsNull = true;
        public bool ExtendDataIsNull { get { return m_ExtendDataIsNull; } set { m_ExtendDataIsNull = value; } }

        public virtual void SetExtendData(string extendData)
        {

        }

        public virtual string GetExtendData()
        {
            return null;
        }

        private string m_SubjectText = null;
        /// <summary>
        /// 纯文本格式的标题 过滤过HTML
        /// </summary>
        public string SubjectText
        {
            get
            {
                string subjectText = m_SubjectText;
                if (subjectText == null)
                {
                    subjectText = Subject;
                    if (ThreadType == ThreadType.Move || ThreadType == ThreadType.Join)
                    {
                        int index = Subject.IndexOf(',');
                        if (index > 0)
                        {
                            subjectText = subjectText.Substring(index + 1);
                        }
                    }

                    subjectText = StringUtil.ClearAngleBracket(subjectText);
                    m_SubjectText = subjectText;
                }
                return subjectText;
            }
        }

        public ThreadStatus ThreadStatus { get; set; }

        public virtual bool IsClosed { get; set; }

        public ThreadAttachType AttachmentType { get; set; }

        public Forum Forum { get { return ForumBO.Instance.GetForum(ForumID); } }

        public int RedirectThreadID
        {
            get
            {
                if (ThreadType == ThreadType.Move || ThreadType == ThreadType.Join || ThreadType == ThreadType.Redirect)
                {
                    string key = "thread_RedirectThreadID_" + ThreadID;

                    int tempID;
                    if (PageCacheUtil.TryGetValue<int>(key, out tempID) == false)
                    {
                        int index = Subject.IndexOf(',');
                        if (index > 0)
                        {
                            string threadIDStr = Subject.Substring(0, index);

                            if (int.TryParse(threadIDStr, out tempID))
                            {
                            }
                        }

                        PageCacheUtil.Set(key, tempID);
                    }

                    return tempID;
                }
                return ThreadID;
            }
        }

        //public bool IsBuyed
        //{
        //    get { return PostBOV5.Instance.IsBuyThread(User.Current, this); }
        //}

        public bool IsBuyed(AuthUser operatorUser)
        {
            return PostBOV5.Instance.IsBuyedThread(operatorUser, this);
        }

        /// <summary>
        /// 只当纯判断是否回复过
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsReplied(AuthUser operatorUser)
        {
            return PostBOV5.Instance.IsRepliedThread(operatorUser, this);
        }

        public int TotalPages
        {
            get
            {
                return GetTotalPages(TotalRepliesForPage);
            }
        }

        private int GetTotalPages(int totalRepliesForPage)
        {
            int pageSize = AllSettings.Current.BbsSettings.PostsPageSize;

            return (totalRepliesForPage + 1) % pageSize == 0 ? ((totalRepliesForPage + 1) / pageSize) : ((totalRepliesForPage + 1) / pageSize + 1);

        }

        /// <summary>
        /// 用于 主题页面URL地址 如: "..../poll-2-1";
        /// </summary>
        public string ThreadTypeString
        {
            get { return PostBOV5.Instance.GetThreadTypeString(ThreadType); }
        }

        /// <summary>
        /// 仅当 获取未审核帖子的主题时才有值 GetUnapprovedPostThreads
        /// </summary>
        public int UnApprovedPostsCount { get; set; }

        public Judgement Judgement
        {
            get { return AllSettings.Current.JudgementSettings.GetJudgement(JudgementID); }
        }

        private string[] m_ThreadRecord;
        public string[] ThreadRecord
        {
            get
            {
                if (m_ThreadRecord == null)
                {
                    string[] temp = ThreadLog.Split('|');
                    if (temp.Length == 3)
                    {
                        m_ThreadRecord = temp;
                        m_ThreadRecord[2] = Convert.ToDateTime(m_ThreadRecord[2]).ToString();
                    }
                }
                return m_ThreadRecord;
            }
        }


        public bool IsOverUpdateSortOrderTime(ForumSettingItem forumSetting)
        {
            int seconds = forumSetting.UpdateThreadSortOrderIntervals.GetValue(PostUserID);
            if (seconds == 0)
                return false;
            return UpdateDate.AddSeconds(seconds) <= DateTimeUtil.Now;
        }

        /// <summary>
        /// 是否到了免费期 如果是出售的主题 超出了出售期限 就自动变为免费
        /// </summary>
        /// <param name="forumSetting"></param>
        /// <returns></returns>
        public bool IsOverSellDays(ForumSettingItem forumSetting)
        {
            if (forumSetting.SellThreadDays == 0)
                return false;
            return CreateDate.AddSeconds(forumSetting.SellThreadDays) <= DateTimeUtil.Now;
        }

        /// <summary>
        /// 从目标主题复制过来 
        /// </summary>
        /// <param name="thread"></param>
        public virtual void CopyFrom(BasicThread thread)
        {
            this.CreateDate = thread.CreateDate;
            this.ForumID = thread.ForumID;
            this.IconID = thread.IconID;
            this.IsClosed = thread.IsClosed;
            this.IsLocked = thread.IsLocked;
            this.IsValued = thread.IsValued;
            this.JudgementID = thread.JudgementID;
            this.KeywordVersion = thread.KeywordVersion;
            this.LastPostID = thread.LastPostID;
            this.LastReplyUserID = thread.LastReplyUserID;
            this.LastReplyUsername = thread.LastReplyUsername;
            this.PostUserID = thread.PostUserID;
            this.PostUsername = thread.PostUsername;
            this.Price = thread.Price;
            this.Rank = thread.Rank;
            this.SortOrder = thread.SortOrder;
            this.Subject = thread.Subject;
            this.SubjectStyle = thread.SubjectStyle;
            this.ThreadCatalogID = thread.ThreadCatalogID;
            this.ThreadID = thread.ThreadID;
            this.ThreadLog = thread.ThreadLog;
            this.ThreadStatus = thread.ThreadStatus;
            this.ThreadType = thread.ThreadType;
            this.TotalAttachments = thread.TotalAttachments;
            this.TotalReplies = thread.TotalReplies;
            this.TotalViews = thread.TotalViews;
            this.UnApprovedPostsCount = thread.UnApprovedPostsCount;
            this.UpdateDate = thread.UpdateDate;
            this.UpdateSortOrder = thread.UpdateSortOrder;
            this.ShareCount = thread.ShareCount;
            this.CollectionCount = thread.CollectionCount;
            this.ContentID = thread.ContentID;
            this.AttachmentType = thread.AttachmentType;
            this.PostedCount = thread.PostedCount;
            this.Words = thread.Words;
            //this.ExtendData = thread.ExtendData;
        }

        /*

        #region 帖子缓存

        /// <summary>
        /// 缓存后面几页条数
        /// </summary>
        private const int CacheNewCount = 10;

        /// <summary>
        /// 缓存前面几页条数
        /// </summary>
        private const int CacheOldCount = 10;


        private object m_InitPostsCacheLocker = new object();


        /// <summary>
        /// 只给从缓存里取回复的时候用 如问题帖 如果有最佳答案  则应该为 TotalReplies - 1
        /// </summary>
        public virtual int TotalRepliesForPage
        {
            get { return TotalReplies; }
        }

        /// <summary>
        /// 不包括主题内容
        /// </summary>
        private PostCollectionV5 m_TopPosts;
        private PostV5 m_ThreadContent;


        public PostV5 ThreadContent
        {
            get
            {
                PostV5 threadContent = m_ThreadContent;

                if (threadContent == null)
                {
                    PostCollectionV5 topPosts;
                    InitPostsCache(out threadContent, out topPosts);
                }
                return threadContent;
            }
            set 
            {
                lock (m_InitPostsCacheLocker)
                {
                    m_ThreadContent = value;
                }
            }
        }



        public PostCollectionV5 GetPosts(int pageNumber, int pageSize, bool getExtendedInfo)
        {
            PostCollectionV5 topPosts = m_TopPosts;
            PostV5 threadContent = m_ThreadContent;

            if (topPosts == null || threadContent == null)
            {
                InitPostsCache(out threadContent, out topPosts);
            }

            PostCollectionV5 posts = new PostCollectionV5();
            if (pageNumber == 1)//第一页
            {
                posts.Add(threadContent);
                pageSize = pageSize - 1;
            }

            int pageLowerBound = (pageNumber - 1) * pageSize;
            int pageUpperBound = pageLowerBound + pageSize;


            if (pageUpperBound <= CacheOldCount || TotalRepliesForPage <= (CacheOldCount + CacheNewCount))//前面的页面
            {
                int topPostsCount = topPosts.Count;
                for (int i = pageLowerBound; i < pageUpperBound; i++)
                {
                    if (pageNumber > 1)
                    {
                        if (topPostsCount > i-1)
                        {
                            posts.Add(topPosts[i-1]);
                        }
                        else
                        {
                            return posts;
                        }
                    }
                    else
                    {
                        if (topPostsCount > i)
                        {
                            posts.Add(topPosts[i]);
                        }
                        else
                        {
                            return posts;
                        }
                    }
                }
            }
            else if (pageLowerBound >= (TotalRepliesForPage - CacheNewCount))//后面的页面
            {
                int offset = TotalReplies - CacheNewCount;
                int startIndex = pageLowerBound - offset + CacheOldCount;

                int topPostsCount = topPosts.Count;
                for (int i = startIndex; i < startIndex + pageSize; i++)
                {
                    if (topPostsCount > i - 1)
                    {
                        posts.Add(topPosts[i - 1]);
                    }
                    else
                    {
                        return posts;
                    }
                }
            }
            else//中间的页面  没缓存
            {
                //上面减一了  所以这里加回一
                if (pageNumber == 1)
                    pageSize = pageSize + 1;

                BasicThread thread = this;
                ThreadType type = ThreadType;
                PostBOV5.Instance.GetPosts(ThreadID, true, pageNumber, pageSize, TotalReplies + 1, getExtendedInfo, false, true, false, ref thread, out posts, ref type);
            }

            return posts;
        }

        private void InitPostsCache(out PostV5 threadContent, out PostCollectionV5 topPosts)
        {
            threadContent = m_ThreadContent;
            topPosts = m_TopPosts;

            if (threadContent == null || topPosts == null)
            {
                lock (m_InitPostsCacheLocker)
                {
                    if (threadContent == null || topPosts == null)
                    {
                        BasicThread thread = this;
                        PostBOV5.Instance.GetPosts(ThreadID, TotalReplies, CacheOldCount, CacheNewCount, ref thread, out threadContent, out topPosts);

                        if (threadContent == null)
                        {
                            threadContent = new PostV5();
                            threadContent.PostType = PostType.ThreadContent;
                            threadContent.Subject = this.Subject;
                            threadContent.Content = string.Empty;
                            threadContent.Attachments = new AttachmentCollection();
                            threadContent.PostMarks = new PostMarkCollection();
                            threadContent.UpdateDate = this.UpdateDate;
                            threadContent.CreateDate = this.CreateDate;
                            threadContent.UserID = this.PostUserID;
                            threadContent.Username = this.PostUsername;
                            threadContent.IsApproved = this.ThreadStatus == ThreadStatus.UnApproved;
                            threadContent.IconID = this.IconID;
                        }

                        m_ThreadContent = threadContent;
                        m_TopPosts = topPosts;
                    }
                }
            }
        }


        public bool ContainPostCache(int postID)
        {
            PostV5 threadContent = ThreadContent;

            if (threadContent != null && threadContent.PostID == postID)
                return true;

            PostCollectionV5 topPosts = m_TopPosts;

            if (topPosts == null)
                return false;

            return topPosts.ContainsKey(postID);
        }

        public PostV5 GetPostFromCache(int postID)
        {
            PostV5 threadContent = ThreadContent;

            if (threadContent != null && threadContent.PostID == postID)
                return threadContent;

            PostCollectionV5 topPosts = m_TopPosts;

            if (topPosts == null)
                return null;

            PostV5 post;
            
            topPosts.TryGetValue(postID, out post);

            return post;
        }

        public void ClearPostsCache()
        {
            lock (m_InitPostsCacheLocker)
            {
                m_TopPosts = null;
                m_ThreadContent = null;
            }
        }


        /// <summary>
        /// 将回复加入缓存（仅当Topposts缓存列表存在的时候才加入）
        /// </summary>
        /// <param name="post"></param>
        public void AddPostToCache(PostV5 post)
        {
            lock (m_InitPostsCacheLocker)
            {
                PostCollectionV5 topPosts = m_TopPosts;

                if (topPosts == null)
                    return;

                if (topPosts.Count < CacheOldCount + CacheNewCount)
                {
                    PostCollectionV5 newTopPosts = new PostCollectionV5(topPosts);
                    newTopPosts.Add(post);
                    m_TopPosts = newTopPosts;
                }
                else
                {
                    PostCollectionV5 newTopPosts = new PostCollectionV5();

                    int i = 0;
                    foreach (PostV5 tempPost in m_TopPosts)
                    {
                        if (i != CacheOldCount)
                            newTopPosts.Add(tempPost);
                        i++;
                    }

                    newTopPosts.Add(post);

                    m_TopPosts = newTopPosts;
                }
            }
        }

        /// <summary>
        /// 更新缓存 如果缓存里不存在该帖子  则不进行任何操作
        /// </summary>
        /// <param name="post"></param>
        public virtual void UpdatePostCache(PostV5 post)
        {
            lock (m_InitPostsCacheLocker)
            {
                PostV5 threadContent = m_ThreadContent;

                if (threadContent != null && post.PostID == threadContent.PostID)
                    m_ThreadContent = post;

                else
                {
                    PostCollectionV5 topPosts = m_TopPosts;

                    if (topPosts != null && topPosts.ContainsKey(post.PostID))
                        topPosts.Set(post);
                }
            }
        }

        #endregion

        */

        #region 帖子缓存 做法2


        /// <summary>
        /// 只给从缓存里取回复的时候用 如问题帖 如果有最佳答案  则应该为 TotalReplies - 1
        /// </summary>
        public virtual int TotalRepliesForPage
        {
            get { return TotalReplies; }
        }

        public PostV5 ThreadContent
        {
            get;
            set;
        }

        private int CacheFirstPageCount = 3;
        private int CacheLastPageCount = 3;
        private object m_CachedPagePostsLocker = new object();
        private PostCollectionV5[] cachedPagePosts;// = new PostCollectionV5[CacheFirstPageCount + CacheLastPageCount];


        public bool IsGetFromDataBase = false;

        //private PostCollectionV5[] lastCachedPagePosts;
        /// <summary>
        /// 仅当这个主题有缓存的时候 外面才可以调这个方法 调用这个方法可能会缓存该主题的回复
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="getExtendedInfo"></param>
        /// <returns></returns>
        public PostCollectionV5 GetPosts(int pageNumber, int pageSize, bool getExtendedInfo)
        {
            IsGetFromDataBase = false;
            if (pageNumber < 1)
                pageNumber = 1;

            int defaultPostPageSize = AllSettings.Current.BbsSettings.PostsPageSize;
            if (pageSize != defaultPostPageSize)
            {
                return GetPostsFromDatabase(pageNumber, pageSize, getExtendedInfo);
            }

            PostCollectionV5 posts;
            if (pageNumber <= CacheFirstPageCount || pageNumber > TotalPages - CacheLastPageCount)
            {
                int index = GetCachedPageIndex(pageNumber);

                if (cachedPagePosts == null)
                {
                    if (getExtendedInfo) //只缓存 有带附件 评分等信息的帖子
                    {
                        cachedPagePosts = new PostCollectionV5[CacheFirstPageCount + CacheLastPageCount];
                        posts = GetPostsFromDatabase(pageNumber, defaultPostPageSize, getExtendedInfo);
                        if (cachedPagePosts != null)
                        {
                            if (cachedPagePosts.Length <= index)
                            {
                                CreateLog("索引超出了数组界限1(Length:" + cachedPagePosts.Length + ";index:" + index + ")", pageNumber);
                            }
                            else
                                cachedPagePosts[index] = posts;
                        }

                        /*
                        if (ThreadContent == null && ((ThreadType == ThreadType.Normal && pageNumber == 1) || ThreadType != ThreadType.Normal))
                        {
                            CreateLog("第一次缓存帖子，应取主题内容却没取,pageSize:" + pageSize.ToString() + ";getExtendInfo:" + getExtendedInfo.ToString(), pageNumber);
                        }
                        */
                    }
                    else
                        return GetPostsFromDatabase(pageNumber, defaultPostPageSize, getExtendedInfo);

                }
                else
                {
                    if (cachedPagePosts.Length <= index)
                    {
                        CreateLog("索引超出了数组界限2(Length:" + cachedPagePosts.Length + ";index:" + index + ")", pageNumber);
                        posts = null;
                    }
                    else
                        posts = cachedPagePosts[index];

                    if (posts == null)
                    {
                        if (getExtendedInfo) //只缓存 有带附件 评分等信息的帖子
                        {
                            posts = GetPostsFromDatabase(pageNumber, defaultPostPageSize, getExtendedInfo);

                            if (cachedPagePosts != null)
                            {
                                if (cachedPagePosts.Length <= index)
                                {
                                    CreateLog("索引超出了数组界限2(Length:" + cachedPagePosts.Length + ";index:" + index + ")", pageNumber);
                                    posts = null;
                                }
                                else
                                    cachedPagePosts[index] = posts;
                            }

                            /*
                            if (ThreadContent == null && ((ThreadType == ThreadType.Normal && pageNumber == 1) || ThreadType != ThreadType.Normal))
                            {
                                CreateLog("3", pageNumber);
                            }
                            */
                        }
                        else
                            return GetPostsFromDatabase(pageNumber, defaultPostPageSize, getExtendedInfo);
                    }
                }

                return new PostCollectionV5(posts);
            }
            else
            {
                posts = GetPostsFromDatabase(pageNumber, defaultPostPageSize, getExtendedInfo);


                //if (ThreadContent == null && ((ThreadType == ThreadType.Normal && pageNumber == 1) || ThreadType != ThreadType.Normal))
                //{
                //    CreateLog("4", pageNumber);
                //}

                return posts;
            }
        }

        private void CreateLog(string tag, int pageNumber)
        {

        }

        private PostCollectionV5 GetPostsFromDatabase(int pageNumber, int pageSize, bool getExtendedInfo)
        {
            IsGetFromDataBase = true;
            BasicThread thread;

            if (getExtendedInfo)
                thread = this;
            else//防止主题内容的被改了 导致attachment 等扩展信息变null了 
            {
                thread = new BasicThread();
                thread.CopyFrom(this);
                thread.IsGetFromDataBase = true;
            }
            ThreadType type = ThreadType;
            PostCollectionV5 posts;

            bool getContent;
            if ((this.ThreadType == ThreadType.Normal)
                && pageNumber > 1)
                getContent = false;
            else
                getContent = true;

            //string info = "101;getContent:" + getContent.ToString() + ";getExtendedInfo:" + getExtendedInfo.ToString() + ";hasThreadContent";

            PostBOV5.Instance.GetPosts(ThreadID, true, pageNumber, pageSize, TotalReplies + 1, getExtendedInfo, false, getContent, false, ref thread, out posts, ref type);

            return posts;
        }

        public void AddPostToCache(PostV5 post)
        {
            if (cachedPagePosts == null)
                return;

            int pageSize = AllSettings.Current.BbsSettings.PostsPageSize;

            lock (m_CachedPagePostsLocker)
            {
                int index = GetCachedPageIndex(TotalPages);
                //int total = TotalRepliesForPage + 1;
                //if (total % AllSettings.Current.BbsSettings.PostsPageSize > 0)
                //{
                //    //如果最后页 没缓存 那就不加入
                //    if (cachedPagePosts[index] == null)
                //    {
                //        return;
                //    }
                //}

                PostCollectionV5 posts = cachedPagePosts[index];

                if (posts == null)
                    posts = new PostCollectionV5();

                if (posts.Count > 0)
                {
                    int tempi = -1;
                    for (int i = posts.Count - 1; i > -1; i--)
                    {
                        if (post.PostID < posts[i].PostID)
                        {
                            tempi = i;
                        }
                        else
                            break;
                    }
                    if (tempi > -1)
                    {
                        posts.Insert(tempi, post);
                        post = posts[posts.Count - 1];
                    }
                }

                if (posts.Count >= pageSize) //需要新的页面 缓存
                {
                    PostCollectionV5 tempPosts = new PostCollectionV5();
                    tempPosts.Add(post);

                    if (index == CacheFirstPageCount + CacheLastPageCount - 1)//已经是最后一页  
                    {
                        //需要新的页面 则把LastCachePage的最前面一页 删除 后面的页面往前挪一个位置  新页加到后面
                        PostCollectionV5[] tempPages = new PostCollectionV5[CacheFirstPageCount + CacheLastPageCount];
                        for (int i = 0; i < CacheFirstPageCount + CacheLastPageCount; i++)
                        {
                            if (i < CacheFirstPageCount)
                                tempPages[i] = cachedPagePosts[i];
                            else
                            {
                                if (i + 1 < CacheFirstPageCount + CacheLastPageCount)
                                    tempPages[i] = cachedPagePosts[i + 1];
                            }
                        }

                        tempPages[CacheFirstPageCount + CacheLastPageCount - 1] = tempPosts;
                        cachedPagePosts = tempPages;
                    }
                    else
                    {
                        cachedPagePosts[index + 1] = tempPosts;
                    }
                }
                else
                {
                    posts.Add(post);
                    cachedPagePosts[index] = posts;
                }
            }
        }

        private int GetCachedPageIndex(int pageNumber)
        {
            int index;
            if (pageNumber <= CacheFirstPageCount)
                index = pageNumber - 1;
            else
                index = CacheFirstPageCount + CacheLastPageCount - (TotalPages - pageNumber) - 1;

            return index;
        }


        /// <summary>
        /// 临时 使用的
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public bool ContainPageCache(int pageNumber)
        {
            if (cachedPagePosts == null)
                return false;
            if (pageNumber <= CacheFirstPageCount || pageNumber > TotalPages - CacheLastPageCount)
            {
                PostCollectionV5 posts = cachedPagePosts[GetCachedPageIndex(pageNumber)];
                if (posts == null || posts.Count == 0)
                    return false;
                else
                    return true;
            }

            return false;
        }


        /// <summary>
        /// 更新缓存 如果缓存里不存在该帖子  则不进行任何操作
        /// </summary>
        /// <param name="post"></param>
        public virtual void UpdatePostCache(PostV5 post)
        {
            lock (m_CachedPagePostsLocker)
            {
                PostV5 threadContent = ThreadContent;

                if (threadContent != null && post.PostID == threadContent.PostID)
                    ThreadContent = post;

                else
                {
                    if (cachedPagePosts != null)
                    {
                        for (int i = 0; i < CacheFirstPageCount + CacheLastPageCount; i++)
                        {
                            PostCollectionV5 posts = cachedPagePosts[i];
                            if (posts != null && posts.ContainsKey(post.PostID))
                            {
                                posts.Set(post);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public bool ContainPostCache(int postID)
        {
            PostV5 threadContent = ThreadContent;

            if (threadContent != null && threadContent.PostID == postID)
                return true;

            PostCollectionV5[] cachedPosts = cachedPagePosts;

            if (cachedPosts == null)
                return false;

            for (int i = 0; i < CacheFirstPageCount + CacheLastPageCount; i++)
            {
                PostCollectionV5 tempPosts = cachedPosts[i];
                if (tempPosts != null && tempPosts.ContainsKey(postID))
                    return true;
            }

            return false;
        }

        public PostV5 GetPostFromCache(int postID)
        {
            PostV5 threadContent = ThreadContent;

            if (threadContent != null && threadContent.PostID == postID)
                return threadContent;

            PostCollectionV5[] cachedPosts = cachedPagePosts;

            if (cachedPosts == null)
                return null;

            PostV5 post;

            for (int i = 0; i < CacheFirstPageCount + CacheLastPageCount; i++)
            {
                PostCollectionV5 tempPosts = cachedPosts[i];
                if (tempPosts != null && tempPosts.TryGetValue(postID, out post))
                    return post;
            }
            return null;
        }

        public void ClearPostsCache()
        {
            lock (m_CachedPagePostsLocker)
            {
                cachedPagePosts = null;
                ThreadContent = null;
                ClearOtherPostsCache();
            }
        }

        protected virtual void ClearOtherPostsCache()
        {
        }


        /// <summary>
        /// 最新：必须放在Thread.TotalReplies++ 后面
        /// 过时：回复时 是否需要获得刚回复的帖子 加到缓存中 (只用于回复入库之前进行判断)
        /// </summary>
        /// <returns></returns>
        internal bool MustGetPostAfterReply()
        {
            if (cachedPagePosts == null)
                return false;

            int total = TotalRepliesForPage;// + 1;//此处加1 是因为已经回复了 但是缓存里的Thread.TotalReplies未加1

            int totalPages = GetTotalPages(total);
            int index = GetCachedPageIndex(totalPages);

            if (total % AllSettings.Current.BbsSettings.PostsPageSize > 0)
            {
                //如果最后页 有缓存 那就要加入
                if (cachedPagePosts[index] != null)
                {
                    return true;
                }
            }
            else
            {
                //最后一页的第一个
                return true;
            }

            return false;
        }
        #endregion



        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ThreadID;
        }

        #endregion

        #region ITextRevertable 成员

        public string OriginalSubject { get; set; }

        public string Text
        {
            get { return this.Subject; }
        }

        public string KeywordVersion { get; set; }

        public void SetNewRevertableText(string text, string textVersion)
        {
            this.Subject = text;
            this.KeywordVersion = textVersion;
        }

        public void SetOriginalText(string originalText)
        {
            this.OriginalSubject = originalText;
        }

        #endregion

        #region IFillSimpleUsers 成员

        public int[] GetUserIdsForFill(int actionType)
        {
            switch (actionType)
            {
                case 1:
                    return new int[1] { PostUserID };
                case 2:
                    return new int[1] { LastReplyUserID };
                case 3:
                    return new int[2] { PostUserID, LastReplyUserID };
                default:
                    return new int[] { };
            }
        }

        #endregion
    }

}