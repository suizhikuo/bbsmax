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
using System.Web.Caching;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax
{
    public class ThreadCachePool
    {
        private const string CacheKey_ForumThreadOrderType = CacheKey_ForumThreadOrderTypeStart + "{1}/{2}";//Threads/orderType/forumid/count
        private const string CacheKey_ForumThreadOrderTypeStart = "Threads/{0}/";//Threads/orderType/
        private const string CacheKey_Thread = "Threads/Thread/{0}";

        private static Dictionary<int, BasicThread> s_AllCachedThreads = null;
        private static Dictionary<ThreadOrderType, ThreadCollectionV5> s_AllForumTopThreads = null;

        public static Dictionary<int, BasicThread> GetAllCachedThreads()
        {
            return s_AllCachedThreads;
        }
        public static Dictionary<ThreadOrderType, ThreadCollectionV5> GetAllForumTopThreads()
        {
            return s_AllForumTopThreads;
        }

        /// <summary>
        /// 各版块主题的 缓存键
        /// </summary>
        private static Dictionary<int, List<string>> s_ForumCacheKeys = null;

        //============== locker

        private static object s_AllCachedThreadsLocker = new object();
        private static object s_AllForumTopThreadsLocker = new object();

        private static Dictionary<int, object> s_ForumLockers = null;
        private static Dictionary<ThreadOrderType, object> s_AllForumTopThreadsLockers = null;


        static ThreadCachePool()
        {
            s_AllForumTopThreadsLockers = GetAllForumTopThreadsLocks();

            InitForumLockers();
        }

        private static void InitForumLockers()
        {

            Dictionary<int, object>  forumLockers = new Dictionary<int, object>();

            ForumCollection forums = ForumBO.Instance.GetAllForums();

            for (int i = 0; i < forums.Count; i++)
            {
                forumLockers.Add(forums[i].ForumID, new object());
            }
            forumLockers.Add(0, new object());

            s_ForumLockers = forumLockers;
        }

        public static void ClearAllCache()
        {
            lock (s_AllCachedThreadsLocker)
            {
                CacheUtil.RemoveBySearch("Threads/");

                //int count = CacheUtil.GetBySearch<ThreadCollectionV5>("Threads/").Count;
                //if (count > 0)
                //{
                //    MaxLabs.bbsMax.Common.LogHelper.CreateErrorLog2("ClearAllCache", count.ToString());
                //}

                s_AllCachedThreads = null;
                s_AllForumTopThreads = null;
                s_ForumCacheKeys = null;
                //s_ForumLockers = null;

                InitForumLockers();
            }
        }

        /// <summary>
        /// 移除版块的主题列表缓存 同时更新主题缓存
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="threadIDs">要更新的主题缓存，如果没有传NULL</param>
        public static void ClearForumCache(int forumID, IEnumerable<int> threadIDs)
        {
            if (s_ForumCacheKeys == null)
                return;

            List<string> tempKeys;

            if (s_ForumCacheKeys.TryGetValue(forumID, out tempKeys))
            {
                List<string> keys = new List<string>(tempKeys);
                ThreadCollectionV5 cachedThreads = new ThreadCollectionV5();
                foreach (string key in keys)
                {
                    ThreadCollectionV5 tempThreads;
                    if (CacheUtil.TryGetValue<ThreadCollectionV5>(key, out tempThreads))
                    {
                        foreach (BasicThread thread in tempThreads)
                        {
                            if (cachedThreads.ContainsKey(thread.ThreadID) == false)
                                cachedThreads.Add(thread);
                        }
                        CacheUtil.Remove(key);
                    }
                }

                if (cachedThreads.Count > 0)
                    UpdateAllCachedThreadsRemoveThreads(cachedThreads, null, null);
            }

            if (threadIDs != null)
                UpdateCache(threadIDs);

        }

        /// <summary>
        /// 检查缓存里是否有这些主题的缓存 如果有则更新这些主题缓存
        /// </summary>
        /// <param name="threadIDs"></param>
        public static void UpdateCache(IEnumerable<int> threadIDs)
        {
            if(s_AllCachedThreads == null)
                return;

            List<int> cachedIDs = new List<int>();
            Dictionary<int, BasicThread> tempThread = new Dictionary<int, BasicThread>();
            foreach (int id in threadIDs)
            { 
                BasicThread thread;
                if (s_AllCachedThreads.TryGetValue(id, out thread))
                {
                    cachedIDs.Add(id);
                    tempThread.Add(id, thread);
                }
                else
                {
                    /*
                    RemoveThreadCache(id);
                    */
                }
            }

            if (cachedIDs.Count > 0)
            {
                ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreads(cachedIDs);
                foreach (BasicThread thread in threads)
                {
                    BasicThread temp;
                    if (tempThread.TryGetValue(thread.ThreadID, out temp))
                    {
                        temp.CopyFrom(thread);
                        temp.ClearPostsCache();
                    }
                }
            }
        }


        /// <summary>
        /// 清除主题的回复缓存
        /// </summary>
        public static void ClearThreadPostCache(int threadID)
        {
            ClearThreadPostCache(new int[] { threadID });
        }
        /// <summary>
        /// 清除主题的回复缓存
        /// </summary>
        public static void ClearThreadPostCache(IEnumerable<int> threadIDs)
        {
            if (s_AllCachedThreads != null)
            {
                foreach (int id in threadIDs)
                {
                    BasicThread thread;
                    if (s_AllCachedThreads.TryGetValue(id,out thread))
                    {
                        thread.ClearPostsCache();
                    }
                }
            }
        }

        public static bool Contains(int forumID, ThreadOrderType orderType)
        {
            return GetForumThreads(forumID, orderType) != null;
        }

        public static BasicThread GetThread(int threadID)
        {
            bool isInList;
            return GetThread(threadID, out isInList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="isInList">true 表示在列表缓存里 false表示是在单个的主题缓存里</param>
        /// <returns></returns>
        public static BasicThread GetThread(int threadID, out bool isInList)
        {
            isInList = true;
            if (s_AllCachedThreads == null)
                return null;

            BasicThread thread;
            if (s_AllCachedThreads.TryGetValue(threadID, out thread))
            {

                //ThreadCollectionV5 threads = GetForumThreads(thread.ForumID, ThreadOrderType.ForumTopThreadsBySortOrder);
                //if (threads != null)
                //{
                //    BasicThread temp = threads.GetValue(thread.ThreadID);

                //    if (temp != null && temp != thread)
                //    {
                //        CreateLog("", thread);
                //    }
                //}

                return thread;
            }
            return null;

            /*
            BasicThread thread;

            if (s_AllCachedThreads == null)
            {
                isInList = false;
                return GetThreadFromCache(threadID);
            }

            if (s_AllCachedThreads.TryGetValue(threadID, out thread))
            {
                isInList = true;
                return thread;
            }
            else
            {
                isInList = false;
                return GetThreadFromCache(threadID);
            }
            */
        }

        public static bool Contains(int threadID, int postID)
        {
            bool isInListCache;
            BasicThread thread = GetThread(threadID, out isInListCache);
            if (thread != null)
            {
                if (isInListCache)
                    return thread.ContainPostCache(postID);
                else
                    return false;
            }

            return false;
        }

        public static PostV5 GetPost(int threadID, int postID)
        {
            bool isInListCache;
            BasicThread thread = GetThread(threadID, out isInListCache);
            if (thread != null)
            {
                if (isInListCache)
                    return thread.GetPostFromCache(postID);
                else
                    return null;
            }

            return null;
        }

        public static void SetThreadCache(BasicThread thread)
        {
            /*
            if (thread != null)
            {
                if (s_AllCachedThreads != null && s_AllCachedThreads.ContainsKey(thread.ThreadID))
                    return;
                string cacheKey = string.Format(CacheKey_Thread, thread.ThreadID);
                CacheUtil.Set<BasicThread>(cacheKey, thread);
            }
            */
        }

        /*
        private static BasicThread GetThreadFromCache(int threadID)
        {
            string cacheKey = string.Format(CacheKey_Thread, threadID);
            return CacheUtil.Get<BasicThread>(cacheKey);
        }

        private static void RemoveThreadCache(int threadID)
        {
            string cacheKey = string.Format(CacheKey_Thread, threadID);
            CacheUtil.Remove(cacheKey);
        }

        */
         
        /// <summary>
        /// 更新缓存的主题olThread 为newThread ，只更新BasicThreadV5中的属性，没有更新子类的属性 不更新点击数
        /// </summary>
        /// <param name="olThread"></param>
        /// <param name="newThread"></param>
        public static void UpdateThreadCache(BasicThread olThread, BasicThread newThread)
        {
            int totalView = olThread.TotalViews;
            olThread.CopyFrom(newThread);
            olThread.TotalViews = totalView;
        }


        /// <summary>
        /// 已经被缓存的个数
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static int GetTopThreadCachedCount(ThreadOrderType orderType)
        {
            ThreadCollectionV5 threads = GetAllForumThreads(orderType);
            if (threads == null)
                return 0;
            else
                return threads.Count;
        }

        /// <summary>
        /// 已经被缓存的个数
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static int GetForumThreadCachedCount(int forumID, ThreadOrderType orderType)
        {
            ThreadCollectionV5 threads = GetForumThreads(forumID, orderType);
            if (threads == null)
                return 0;
            else
                return threads.Count;
        }


        /// <summary>
        /// 判断是否是在缓存里   还是在静态变量里
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        private static bool IsInCache(ThreadOrderType orderType)
        {
            if (s_AllForumTopThreadsLockers != null)
            {
                return s_AllForumTopThreadsLockers.ContainsKey(orderType) == false;
            }
            return true;
        }


        /// <summary>
        /// 如获取整站的最新帖子等  跟具体版块无关
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static ThreadCollectionV5 GetAllForumThreads(ThreadOrderType orderType)
        {
            if (IsInCache(orderType))
            {
                return GetAllForumThreadsFromCache(orderType);
            }

            if (s_AllForumTopThreads == null)
                return null;

            ThreadCollectionV5 threads;

            s_AllForumTopThreads.TryGetValue(orderType, out threads);

            return threads;
        }

        public static void SetAllForumThreads(ThreadOrderType orderType, ThreadCollectionV5 threads)
        {
            if (IsInCache(orderType))
            {
                SetAllForumThreadsToCache(orderType, threads);
                return;
            }

            lock (s_AllForumTopThreadsLocker)
            {
                if (s_AllForumTopThreads == null)
                    s_AllForumTopThreads = new Dictionary<ThreadOrderType, ThreadCollectionV5>();

                if (s_AllForumTopThreads.ContainsKey(orderType))
                {
                    ThreadCollectionV5 removedThreads = new ThreadCollectionV5();
                    foreach (BasicThread thread in s_AllForumTopThreads[orderType])
                    {
                        if (threads.ContainsKey(thread.ThreadID) == false)
                            removedThreads.Add(thread);
                    }

                    UpdateAllCachedThreadsRemoveThreads(removedThreads, null, orderType, null);
                    s_AllForumTopThreads[orderType] = threads;
                }
                else
                    s_AllForumTopThreads.Add(orderType, threads);
            }

            UpdateAllCachedThreadsAddThreads(threads);
        }

        private static void SetAllForumThreadsToCache(ThreadOrderType orderType, ThreadCollectionV5 threads)
        {
            SetForumThreadsCache(0, orderType, threads);
        }

        private static ThreadCollectionV5 GetAllForumThreadsFromCache(ThreadOrderType orderType)
        {
            return GetForumThreads(0, orderType);
        }

        private static void AddAllForumThreadToCache(ThreadOrderType orderType, BasicThread thread)
        {
            AddForumThread(0, orderType, thread);
        }

        /// <summary>
        /// 将主题加入列表  如果不存在则加入列表最前面  如果存在则提到最前面 并更新
        /// </summary>
        /// <param name="orderType"></param>
        /// <param name="thread"></param>
        public static void AddAllForumThread(ThreadOrderType orderType, BasicThread thread)
        {
            if (IsInCache(orderType))
            {
                AddAllForumThreadToCache(orderType, thread);
                return;
            }

            int count = GetTotalCacheCount(orderType);

            lock (s_AllForumTopThreadsLocker)
            {
                if (s_AllForumTopThreads == null)
                {
                    return;
                    //AllForumTopThreads = new Dictionary<ThreadOrderType, ThreadCollectionV5>();
                }

                Dictionary<ThreadOrderType, ThreadCollectionV5> tempAllForumTopThreads = new Dictionary<ThreadOrderType, ThreadCollectionV5>();

                foreach (KeyValuePair<ThreadOrderType, ThreadCollectionV5> pair in s_AllForumTopThreads)
                {
                    tempAllForumTopThreads.Add(pair.Key, pair.Value);
                }

                lock (s_AllForumTopThreadsLockers[orderType])
                {
                    ThreadCollectionV5 temp;
                    tempAllForumTopThreads.TryGetValue(orderType, out temp);

                    if (temp == null)
                        return;

                    ThreadCollectionV5 threads = new ThreadCollectionV5(temp);
                    if (threads.Count == 0)
                    {
                        threads.Add(thread);

                        tempAllForumTopThreads[orderType] = threads;

                        UpdateAllCachedThreadsAddThreads(thread);
                    }
                    else
                    {
                        BasicThread tempThread;
                        threads.TryGetValue(thread.ThreadID, out tempThread);

                        ThreadCollectionV5 removedThreads = new ThreadCollectionV5();
                        if (tempThread != null)
                        {
                            UpdateThreadCache(tempThread, thread);//此处是为了更新  AllCachedThreads 和其它缓存 里的这个主题
                            threads.RemoveByKey(thread.ThreadID);
                            //removedThreads.Add(tempThread);
                        }
                        else
                        {
                            UpdateAllCachedThreadsAddThreads(thread);
                        }

                        threads.Insert(0, thread);

                        if (threads.Count > count)
                        {
                            removedThreads.Add(threads[threads.Count - 1]);
                            threads.RemoveAt(threads.Count - 1);
                        }

                        UpdateAllCachedThreadsRemoveThreads(removedThreads, null, orderType, null);

                    }

                    tempAllForumTopThreads[orderType] = threads;
                }

                s_AllForumTopThreads = tempAllForumTopThreads;
            }
        }

        public static ThreadCollectionV5 GetForumThreads(int forumID, ThreadOrderType orderType)
        {
            string cacheKey = string.Format(CacheKey_ForumThreadOrderType, orderType.ToString(), forumID, GetTotalCacheCount(orderType));

            ThreadCollectionV5 threads;
            if (CacheUtil.TryGetValue<ThreadCollectionV5>(cacheKey, out threads))
            {
                return threads;
            }

            return null;
        }

        /// <summary>
        /// 将主题加入列表  如果不存在则加入列表最前面  如果存在则提到最前面 并更新
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="orderType"></param>
        /// <param name="thread"></param>
        public static void AddForumThread(int forumID, ThreadOrderType orderType, BasicThread thread)
        {
            object obj;
            if (s_ForumLockers.TryGetValue(forumID, out obj) == false)
            {
                obj = new object();
                s_ForumLockers.Add(forumID, obj);

            }

            lock (obj)
            {
                ThreadCollectionV5 cachedThreads = GetForumThreads(forumID, orderType);

                if (cachedThreads != null)
                {
                    ThreadCollectionV5 threads = new ThreadCollectionV5(cachedThreads);
                    int count = GetTotalCacheCount(orderType);
                    ThreadCollectionV5 removedThreads = new ThreadCollectionV5();

                    BasicThread tempThread = threads.GetValue(thread.ThreadID);
                    if (tempThread != null)
                    {
                        UpdateThreadCache(tempThread, thread);
                        threads.RemoveByKey(thread.ThreadID);
                        threads.Insert(0, tempThread);
                    }
                    else
                    {
                        UpdateAllCachedThreadsAddThreads(thread);
                        threads.Insert(0, thread);
                    }


                    if (threads.Count > count)
                    {
                        removedThreads.Add(threads[threads.Count - 1]);
                        threads.RemoveAt(threads.Count - 1);
                    }

                    if (removedThreads.Count > 0)
                    {
                        string cacheKey = string.Format(CacheKey_ForumThreadOrderType, orderType.ToString(), forumID, count);
                        UpdateAllCachedThreadsRemoveThreads(removedThreads, forumID, null, cacheKey);
                    }

                    SetForumThreadsCache(forumID, orderType, threads, false);
                }
            }
        }

        public static void SetForumThreadsCache(int forumID, ThreadOrderType orderType, ThreadCollectionV5 threads)
        {
            lock(s_ForumLockers[forumID])
            {
                SetForumThreadsCache(forumID, orderType, threads, true);
            }
        }

        private static void SetForumThreadsCache(int forumID, ThreadOrderType orderType, ThreadCollectionV5 threads, bool updateAllCachedThreads)
        {
            string cacheKey = string.Format(CacheKey_ForumThreadOrderType, orderType.ToString(), forumID, GetTotalCacheCount(orderType));

            List<string> cacheKeys = null;

            if (s_ForumCacheKeys != null)
            {
                if (!s_ForumCacheKeys.TryGetValue(forumID, out cacheKeys))
                {
                    cacheKeys = new List<string>();
                    cacheKeys.Add(cacheKey);
                    s_ForumCacheKeys.Add(forumID, cacheKeys);
                }
                else if (cacheKeys.Contains(cacheKey) == false)
                    cacheKeys.Add(cacheKey);
                else//原来就有的 移除
                {
                    if (updateAllCachedThreads)
                    {
                        ThreadCollectionV5 tempThreads;
                        if (CacheUtil.TryGetValue<ThreadCollectionV5>(cacheKey, out tempThreads))
                        {
                            //if (updateAllCachedThreads)
                                UpdateAllCachedThreadsRemoveThreads(tempThreads, forumID, null, new string[] { cacheKey });
                            //else
                            //    UpdateAllCachedThreadsRemoveThreads(tempThreads, forumID, null);
                        }
                    }
                }
            }
            else
            {
                s_ForumCacheKeys = new Dictionary<int, List<string>>();
                cacheKeys = new List<string>();
                cacheKeys.Add(cacheKey);
                s_ForumCacheKeys.Add(forumID, cacheKeys);
            }

            CacheTime? cacheTime;
            CacheExpiresType expiresType;
            int? minute;
            GetCacheType(orderType, out cacheTime, out expiresType, out minute);

            if (minute == null)
            {
                CacheUtil.Set<ThreadCollectionV5>(cacheKey, threads, cacheTime.Value, expiresType, null, CacheItemPriority.NotRemovable, delegate(string key, object value, CacheItemRemovedReason reason)
                {
                    //cacheKeys.Remove(key);
                    ThreadCollectionV5 removedThreads = (ThreadCollectionV5)value;

                    UpdateAllCachedThreadsRemoveThreads(removedThreads, forumID, null);
                });
            }
            else
            {
                CacheUtil.Set<ThreadCollectionV5>(cacheKey, threads, minute.Value, expiresType, null, CacheItemPriority.NotRemovable, delegate(string key, object value, CacheItemRemovedReason reason)
                {
                    //cacheKeys.Remove(key);
                    ThreadCollectionV5 removedThreads = (ThreadCollectionV5)value;

                    UpdateAllCachedThreadsRemoveThreads(removedThreads, forumID, null);
                });
            }

            if (updateAllCachedThreads)
                UpdateAllCachedThreadsAddThreads(threads);
        }

        public static void ClearForumThreadsCache(int forumID, ThreadOrderType orderType)
        {
            string cacheKey = string.Format(CacheKey_ForumThreadOrderType, orderType.ToString(), forumID, GetTotalCacheCount(orderType));
            ClearForumThreadsCache(cacheKey, forumID);
        }


        private static void ClearForumThreadsCache(string cacheKey, int forumID)
        {
            List<string> cacheKeys = null;
            if (s_ForumCacheKeys!=null && s_ForumCacheKeys.TryGetValue(forumID, out cacheKeys))
            {
                if (cacheKeys.Contains(cacheKey))
                {
                    cacheKeys.Remove(cacheKey);
                }
            }

            ThreadCollectionV5 threads;

            if (CacheUtil.TryGetValue<ThreadCollectionV5>(cacheKey, out threads))
            {
                CacheUtil.Remove(cacheKey);
                UpdateAllCachedThreadsRemoveThreads(threads, forumID, null);
            }
        }

        public static void ClearAllForumThreadsCache(ThreadOrderType orderType)
        {
            if (IsInCache(orderType))
            {
                ClearForumThreadsCache(0, orderType);
                return;
            }

            if (s_AllForumTopThreads == null)
                return;

            ThreadCollectionV5 threads;
            if (s_AllForumTopThreads.TryGetValue(orderType, out threads))
            {
                s_AllForumTopThreads.Remove(orderType);
                UpdateAllCachedThreadsRemoveThreads(threads, null, null);
            }
        }

        private static void UpdateAllCachedThreadsAddThreads(BasicThread thread)
        {
            ThreadCollectionV5 threads = new ThreadCollectionV5();
            threads.Add(thread);

            UpdateAllCachedThreadsAddThreads(threads);
        }

        private static void UpdateAllCachedThreadsAddThreads(ThreadCollectionV5 threads)
        {
            lock (s_AllCachedThreadsLocker)
            {
                if (s_AllCachedThreads == null)
                    s_AllCachedThreads = new Dictionary<int, BasicThread>();

                int i = 0;
                ThreadCollectionV5 temp = new ThreadCollectionV5(threads);
                foreach (BasicThread thread in temp)
                {
                    /*
                    RemoveThreadCache(thread.ThreadID);
                    */
                    BasicThread cached;
                    if (s_AllCachedThreads.TryGetValue(thread.ThreadID, out cached))
                    {
                        cached.CopyFrom(thread);
                        //使用同一个对象
                        threads[i] = cached;
                    }
                    else
                    {
                        s_AllCachedThreads.Add(thread.ThreadID, thread);
                    }

                    i++;
                }
            }
        }


        private static void UpdateAllCachedThreadsRemoveThread(BasicThread thread, int? forumID, ThreadOrderType? excludeOrderType, string excludeCacheKey)
        {
            ThreadCollectionV5 threads = new ThreadCollectionV5();

            threads.Add(thread);

            UpdateAllCachedThreadsRemoveThreads(threads, forumID, excludeOrderType, excludeCacheKey);
        }

        /// <summary>
        /// 移除AllCachedThreads里的缓存
        /// </summary>
        /// <param name="threads"></param>
        /// <param name="forumID"></param>
        /// <param name="excludeCacheKeys">这些缓存将不去检查(这样就会去移除AllCachedThreads)</param>
        private static void UpdateAllCachedThreadsRemoveThreads(ThreadCollectionV5 threads, int? forumID, ThreadOrderType? excludeOrderType, params string[] excludeCacheKeys)
        {
            if (s_AllCachedThreads == null)
                return;

            if (threads == null || threads.Count == 0)
                return;

            List<string> cacheKeys = null;


            if (/*forumID != null &&*/ s_ForumCacheKeys != null)
            {
                List<int> checkForumIDs = new List<int>();
                StickThreadCollection stickThreads = null;
                foreach (BasicThread thread in threads)
                {
                    if (thread.ThreadStatus == Enums.ThreadStatus.Sticky ||
                        thread.ThreadStatus == Enums.ThreadStatus.GlobalSticky)
                    {
                        if (stickThreads == null)
                            stickThreads = PostBOV5.Instance.GetAllStickThreadInForums();
                        foreach (StickThread stick in stickThreads)
                        {
                            if (stick.ThreadID == thread.ThreadID && checkForumIDs.Contains(stick.ForumID) == false)
                            {
                                checkForumIDs.Add(stick.ForumID);
                            }
                        }
                    }

                    if (forumID == null)
                    {
                        if (checkForumIDs.Contains(thread.ForumID) == false)
                            checkForumIDs.Add(thread.ForumID);
                    }
                }

                if (forumID != null)
                {
                    checkForumIDs.Add(forumID.Value);
                    if (forumID.Value != 0)//还要从MovedThreads里检查
                        checkForumIDs.Add(0);
                    else//MovedThreads
                    {
                        foreach (BasicThread thread in threads)
                        {
                            if (checkForumIDs.Contains(thread.ForumID))
                                continue;
                            checkForumIDs.Add(thread.ForumID);
                        }
                    }
                }
                else
                {
                    checkForumIDs.Add(0);
                }

                foreach (int tempForumID in checkForumIDs)
                {
                    List<string> tempKeys = null;
                    s_ForumCacheKeys.TryGetValue(tempForumID, out tempKeys);

                    if (tempKeys != null)
                    {
                        if (cacheKeys == null)
                            cacheKeys = new List<string>(tempKeys);
                        else
                            cacheKeys.AddRange(tempKeys);
                    } 
                }

                /*
                List<string> tempCacheKeys = null;
                s_ForumCacheKeys.TryGetValue(forumID.Value, out tempCacheKeys);

                if (tempCacheKeys != null)
                    cacheKeys = new List<string>(tempCacheKeys);
                //cacheKeys = s_ForumCacheKeys[forumID.Value];

                if (forumID.Value != 0)// 还要从MovedThreads里检查
                {
                    List<string> tempKeys = null;
                    s_ForumCacheKeys.TryGetValue(0, out tempKeys);
                    if (tempKeys != null)
                    {
                        if (cacheKeys == null)
                            cacheKeys = new List<string>(tempKeys);
                        else
                            cacheKeys.AddRange(tempKeys);
                    }
                }
                else//MovedThreads 
                {
                    List<int> tempForumIDs = new List<int>();
                    foreach (BasicThread thread in threads)
                    {
                        if (tempForumIDs.Contains(thread.ForumID))
                            continue;
                        tempForumIDs.Add(thread.ForumID);

                        List<string> tempKeys = null;
                        s_ForumCacheKeys.TryGetValue(thread.ForumID, out tempKeys);

                        if (tempKeys != null)
                        {
                            if (cacheKeys == null)
                                cacheKeys = new List<string>(tempKeys);
                            else
                                cacheKeys.AddRange(tempKeys);
                        }
                    }
                }
                */
            }


            lock (s_AllCachedThreadsLocker)
            {
                if (s_AllCachedThreads == null)
                    return;

                foreach (BasicThread thread in threads)
                {
                    //检查其他所有缓存  如果有该主题的缓存 就不从AllCachedThreads中移除  否则移除
                    if (s_AllCachedThreads != null && s_AllCachedThreads.ContainsKey(thread.ThreadID))
                    {
                        bool remove = true;
                        if (cacheKeys != null)
                        {
                            //检查当前版块中的所有缓存的主题 
                            foreach (string key in cacheKeys)
                            {
                                if (excludeCacheKeys != null)
                                {
                                    bool exclude = false;
                                    foreach (string eck in excludeCacheKeys)
                                    {
                                        if (string.Compare(eck, key, true) == 0)
                                        {
                                            exclude = true;
                                            break;
                                        }
                                    }

                                    if (exclude)
                                        continue;
                                }

                                ThreadCollectionV5 tempThreads;
                                if (CacheUtil.TryGetValue<ThreadCollectionV5>(key, out tempThreads))
                                {
                                    if (tempThreads.ContainsKey(thread.ThreadID))
                                    {
                                        remove = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (remove == true && s_AllForumTopThreads != null)
                        {
                            //检查所有版块中的所有缓存的主题 
                            foreach (KeyValuePair<ThreadOrderType, ThreadCollectionV5> pair in s_AllForumTopThreads)
                            {
                                if (excludeOrderType != null && excludeOrderType.Value == pair.Key)
                                    continue;

                                if (pair.Value.ContainsKey(thread.ThreadID))
                                {
                                    remove = false;
                                    break;
                                }
                            }
                        }

                        if (remove && s_AllCachedThreads != null)
                            s_AllCachedThreads.Remove(thread.ThreadID);
                    }
                }
            }
        }


        










        /*
         * =====================================
         * 新加一个缓存列表  需要 设置以下
         * =====================================
        */

        /// <summary>
        /// 在每天0点 必须移除的缓存
        /// </summary>
        public static void ClearCahceAt0AM()
        {
            //ClearAllForumThreadsCache(ThreadOrderType.AllForumTopDayHotThreads);
            //ClearAllForumThreadsCache(ThreadOrderType.AllForumTopDayViewThreads);
            //ClearAllForumThreadsCache(ThreadOrderType.AllForumTopWeekHotThreads);
            //ClearAllForumThreadsCache(ThreadOrderType.AllForumTopWeekViewThreads);

            //干脆全清了
            ClearAllCache();
            
        }



        /// <summary>
        /// 使用静态变量保存的列表的锁 
        /// </summary>
        /// <returns></returns>
        private static Dictionary<ThreadOrderType, object> GetAllForumTopThreadsLocks()
        {
            Dictionary<ThreadOrderType, object> locks = new Dictionary<ThreadOrderType, object>();
            locks.Add(ThreadOrderType.AllForumTopThreadsByLastPostID, new object());
            locks.Add(ThreadOrderType.AllForumTopThreadsByThreadID, new object());
            locks.Add(ThreadOrderType.GlobalStickThreads, new object());
            return locks;
        }


        /// <summary>
        /// 要缓存的条数
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static int GetTotalCacheCount(ThreadOrderType orderType)
        {
            switch (orderType)
            {
                case ThreadOrderType.ForumTopThreadsBySortOrder:
                    return 300;
                case ThreadOrderType.ForumTopThreadsByThreadID:
                case ThreadOrderType.ForumTopThreadsByLastPostID:
                case ThreadOrderType.ForumTopValuedThreads:
                case ThreadOrderType.ForumTopDayHotThreads:
                case ThreadOrderType.ForumTopWeekHotThreads:
                case ThreadOrderType.ForumTopDayViewThreads:
                case ThreadOrderType.ForumTopWeekViewThreads:
                    return 30;
                case ThreadOrderType.AllForumTopThreadsByThreadID:
                case ThreadOrderType.AllForumTopThreadsByLastPostID:
                case ThreadOrderType.AllForumTopValuedThreads:
                case ThreadOrderType.AllForumTopWeekHotThreads:
                case ThreadOrderType.AllForumTopDayHotThreads:
                case ThreadOrderType.AllForumTopWeekViewThreads:
                case ThreadOrderType.AllForumTopDayViewThreads:
                    return 50;
                case ThreadOrderType.MovedThreads:
                case ThreadOrderType.GlobalStickThreads:
                case ThreadOrderType.ForumStickThreads:
                    return int.MaxValue;
                default:
                    return 0;
            }
        }

        private static void GetCacheType(ThreadOrderType orderType, out CacheTime? cacheTime, out CacheExpiresType expiresType, out int? minute)
        {
            minute = null;
            switch (orderType)
            {
                case ThreadOrderType.AllForumTopWeekViewThreads:
                case ThreadOrderType.ForumTopWeekViewThreads:
                    cacheTime = null;
                    minute = AllSettings.Current.CacheSettings.WeekPostCacheTime;
                    expiresType = CacheExpiresType.Absolute;
                    break;
                case ThreadOrderType.ForumTopDayViewThreads:
                case ThreadOrderType.AllForumTopDayViewThreads:
                    cacheTime = null;
                    minute = AllSettings.Current.CacheSettings.DayPostCacheTime;
                    expiresType = CacheExpiresType.Absolute;
                    break;
                case ThreadOrderType.MovedThreads:
                    cacheTime = null;
                    minute = 20;
                    expiresType = CacheExpiresType.Absolute;
                    break;
                default:
                    cacheTime = CacheTime.Default;
                    expiresType = CacheExpiresType.Sliding;
                    break;
            }
        }


        public enum ThreadOrderType : byte
        {
            /// <summary>
            /// 按所有版快主题发表时间排序
            /// </summary>
            AllForumTopThreadsByThreadID,

            /// <summary>
            /// 按所有版快最后回复时间排序
            /// </summary>
            AllForumTopThreadsByLastPostID,

            AllForumTopValuedThreads,

            AllForumTopWeekHotThreads,

            AllForumTopDayHotThreads,

            AllForumTopWeekViewThreads,

            AllForumTopDayViewThreads,

            GlobalStickThreads,

            ForumStickThreads,

            ForumTopThreadsBySortOrder,

            /// <summary>
            /// 版块最新主题
            /// </summary>
            ForumTopThreadsByThreadID,

            /// <summary>
            /// 版块最新回复主题
            /// </summary>
            ForumTopThreadsByLastPostID,

            ForumTopValuedThreads,

            
            ForumTopDayHotThreads,

            ForumTopWeekHotThreads,

            ForumTopWeekViewThreads,

            ForumTopDayViewThreads,

            /// <summary>
            /// 被移动过的主题 
            /// </summary>
            MovedThreads,

        }


        public static void CreateLog(string tag,BasicThread thread)
        {

        }

    }
}