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
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Jobs;
using System.Web.Caching;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Ubb;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.FileSystem;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Filters;
using System.Data;
using MaxLabs.bbsMax.RegExp;
using PanGu;
using PanGu.Match;

namespace MaxLabs.bbsMax
{
    public class PostBOV5 : BOBase<PostBOV5>
    {
        #region 版块最后更新主题  缓存

        private static Dictionary<int, BasicThread> s_ForumLastThreads = new Dictionary<int, BasicThread>();
        public BasicThread GetForumLastThreadFromCache(Forum forum)
        {
            //始终先从 ThreadCachePool 里找
            BasicThread thread = ThreadCachePool.GetThread(forum.LastThreadID);
            if (thread != null)
                return thread;

            if (s_ForumLastThreads.TryGetValue(forum.ForumID, out thread))
            {
                if (thread.ThreadID == forum.LastThreadID)
                    return thread;
            }

            return null;
        }


        private static object s_SetForumLastThreadLocker = new object();
        /// <summary>
        /// 存在则更新 不存在则加入
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="thread"></param>
        public void SetForumLastThreadCache(int forumID, BasicThread thread)
        {
            lock (s_SetForumLastThreadLocker)
            {
                if (s_ForumLastThreads.ContainsKey(forumID))
                {
                    s_ForumLastThreads[forumID] = thread;
                }
                else
                {
                    s_ForumLastThreads.Add(forumID, thread);
                }
            }
        }

        #endregion



        private string cacheKey_List_Topic_Search_Count = "Topic/List/Search/Count/{0}";
        private string cacheKey_List_Post_Search_Count = "Post/List/Search/Count/{0}";

        private string cacheKey_List_Attachment_Search_Count = "Attachment/List/Search/Count/{0}";

        private  Dictionary<int, List<int>> GetForumIDsThreadIDs(IEnumerable<int> threadIdentities)
        {
            ThreadCollectionV5 threads = GetThreads(threadIdentities);
            Dictionary<int, List<int>> tempThreads = new Dictionary<int, List<int>>();
            foreach (BasicThread thread in threads)
            {
                if (tempThreads.ContainsKey(thread.ForumID))
                {
                    tempThreads[thread.ForumID].Add(thread.ThreadID);
                }
                else
                {
                    tempThreads.Add(thread.ForumID, new List<int>());
                    tempThreads[thread.ForumID].Add(thread.ThreadID);
                }
            }
            return tempThreads;
        }

        public ManageForumPermissionSetNode GetForumPermissonSet(Forum forum)
        {
            ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID);
            return managePermission;
        }

        #region 帖子管理

        #region 移动主题 MoveThreads

        public bool MoveThreads(AuthUser operatorUser, int newForumID, IEnumerable<int> threadIDs, bool isKeepLink, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {
            Dictionary<int, List<int>> forumThreads = GetForumIDsThreadIDs(threadIDs);
            foreach (KeyValuePair<int, List<int>> group in forumThreads)
            {
                if (false == MoveThreads(operatorUser, group.Key, newForumID, group.Value, isKeepLink, ignorePermission, createManageLog, sendNotify, actionReason))
                    return false;
            }
            return true;
        }


        /// <summary>
        /// 移动帖子
        /// </summary>
        /// <param name="OldForumID"></param>
        /// <param name="NewForumID"></param>
        /// <param name="IsKeepLink">是否在原版块保持链接</param>
        /// <returns></returns>
        public bool MoveThreads(AuthUser operatorUser, int oldForumID, int newForumID, IEnumerable<int> threadIDs, bool isKeepLink, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {
            if (oldForumID == newForumID)
            {
                ThrowError<MoveThreadNoSelectNewForumError>(new MoveThreadNoSelectNewForumError());
                return false;
            }

            ThreadCollectionV5 threads = GetThreads(threadIDs);
            if (threads.Count == 0)
            {
                ThrowError(new MoveThreadNoSelectThreadError());
                return false;
            }

            ThreadCollectionV5 tempThreads = new ThreadCollectionV5();
            List<int> tempThreadIDs = new List<int>();

            ManageForumPermissionSetNode managerPermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(oldForumID);

            if (threads.Count == 1)
            {
                if (threads[0].ThreadType == ThreadType.Move)
                {
                    ThrowError(new MoveThreadHadMovedError());
                    return false;
                }
            }
            foreach (BasicThread thread in threads)
            {
                if (thread.ThreadType == ThreadType.Move)
                {
                    continue;
                }

                if (thread.ForumID != oldForumID)
                {
                    continue;
                }
                if (!ignorePermission)
                {
                    if (false == managerPermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.MoveThreads, thread.PostUserID))
                    {
                        ThrowError(new NoPermissionMoveThreadError());
                        return false;
                    }
                }

                tempThreadIDs.Add(thread.ThreadID);
                tempThreads.Add(thread);
            }

            if (tempThreadIDs.Count == 0)
            {
                if (!ignorePermission)
                {
                    ThrowError(new NoPermissionMoveThreadError());
                    return false;
                }
                else
                {
                    ThrowError(new MoveThreadNoSelectThreadError());
                    return false;
                }
            }

            bool success = PostDaoV5.Instance.MoveThreads(oldForumID, newForumID, tempThreadIDs, isKeepLink);
            if (success)
            {
                ForumBO.Instance.ClearAllCache();
                ThreadCachePool.ClearAllCache();


                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, tempThreads, ModeratorCenterAction.MoveThread, actionReason);
                if (createManageLog)
                    WriteThreadOperateLog(operatorUser, tempThreads, ModeratorCenterAction.MoveThread, actionReason);

            }

            return success;
        }


        #endregion
         
        #region 批量删除主题


        public bool DeleteThreads(AuthUser operatorUser, IEnumerable<int> threadIDs, bool ignorePermission, bool updatePoint, bool sendNotify, bool createThreadManageLog, string actionReason)
        {
            if (ValidateUtil.HasItems<int>(threadIDs) == false)
            {
                ThrowError<NotSellectDeleteThreadIDsError>(new NotSellectDeleteThreadIDsError());
                return false;
            }

            ThreadCollectionV5 threads = GetThreads(threadIDs);

            if (threads.Count == 0)
                return true;

            Dictionary<int, List<int>> normalThreads = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> recycledThreads = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> unApprovedThreads = new Dictionary<int, List<int>>();
            foreach (BasicThread thread in threads)
            {
                if (thread.ThreadStatus == ThreadStatus.Recycled)
                {
                    if (recycledThreads.ContainsKey(thread.ForumID))
                    {
                        recycledThreads[thread.ForumID].Add(thread.ThreadID);
                    }
                    else
                    {
                        List<int> tempThreadIDs = new List<int>();
                        tempThreadIDs.Add(thread.ThreadID);
                        recycledThreads.Add(thread.ForumID, tempThreadIDs);
                    }
                }
                else if (thread.ThreadStatus == ThreadStatus.UnApproved)
                {
                    if (unApprovedThreads.ContainsKey(thread.ForumID))
                    {
                        unApprovedThreads[thread.ForumID].Add(thread.ThreadID);
                    }
                    else
                    {
                        List<int> tempThreadIDs = new List<int>();
                        tempThreadIDs.Add(thread.ThreadID);
                        unApprovedThreads.Add(thread.ForumID, tempThreadIDs);
                    }
                }
                else
                {
                    if (normalThreads.ContainsKey(thread.ForumID))
                    {
                        normalThreads[thread.ForumID].Add(thread.ThreadID);
                    }
                    else
                    {
                        List<int> tempThreadIDs = new List<int>();
                        tempThreadIDs.Add(thread.ThreadID);
                        normalThreads.Add(thread.ForumID, tempThreadIDs);
                    }
                }
            }

            bool success = false;

            foreach (KeyValuePair<int, List<int>> pair in normalThreads)
            {
                success = DeleteThreads(operatorUser, pair.Key, ThreadStatus.Normal, pair.Value, false, updatePoint, sendNotify, createThreadManageLog, actionReason);
                if (success == false)
                    return false;
            }
            foreach (KeyValuePair<int, List<int>> pair in recycledThreads)
            {
                success = DeleteThreads(operatorUser, pair.Key, ThreadStatus.Recycled, pair.Value, false, updatePoint, sendNotify, createThreadManageLog, actionReason);
                if (success == false)
                    return false;
            }
            foreach (KeyValuePair<int, List<int>> pair in unApprovedThreads)
            {
                success = DeleteThreads(operatorUser, pair.Key, ThreadStatus.UnApproved, pair.Value, false, updatePoint, sendNotify, createThreadManageLog, actionReason);
                if (success == false)
                    return false;
            }
            return success;
        }

        /// <summary>
        /// 批量删除主题
        /// </summary>
        public bool DeleteThreads(AuthUser operatorUser, int forumID, ThreadStatus threadStatus, IEnumerable<int> threadIDs, bool ignorePermission, bool updatePoint, bool sendNotify, bool createThreadManageLog, string actionReason)
        {

            if (ValidateUtil.HasItems<int>(threadIDs) == false)
            {
                ThrowError<NotSellectDeleteThreadIDsError>(new NotSellectDeleteThreadIDsError());
                return false;
            }

            ThreadCollectionV5 threads = GetThreads(threadIDs);
            if (threads.Count == 0)
            {
                return true;
            }


            List<int> tempThreadIDs = new List<int>(threadIDs);

            if (!ignorePermission)
            {
                Forum forum = ForumBO.Instance.GetForum(forumID, false);

                ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);

                TimeSpan? interval = null;
                if (forumSetting.DeleteOwnThreadsIntervals[operatorUser] > 0)
                {
                    interval = TimeSpan.FromSeconds(forumSetting.DeleteOwnThreadsIntervals[operatorUser]);
                }

                ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID);

                foreach (BasicThread thread in threads)
                {
                    //如果是未审核的主题  只要有审核权限 就可以删除该主题
                    if (thread.ThreadStatus == ThreadStatus.UnApproved && managePermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads, thread.PostUserID))
                    {
                        continue;
                    }
                    if (false == managePermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, thread.PostUserID))
                    {
                        if (thread.PostUserID == operatorUser.UserID)
                        {
                            if (false == AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID).Can(operatorUser, ForumPermissionSetNode.Action.DeleteOwnThreads))
                            {
                                ThrowError(new NoPermissionDeleteThreadError(forum));
                                return false;
                            }

                            if (interval != null)
                                if (DateTimeUtil.Now - thread.CreateDate > interval.Value)
                                    tempThreadIDs.Remove(thread.ThreadID);
                        }
                        else
                            tempThreadIDs.Remove(thread.ThreadID);
                    }
                }
                if (tempThreadIDs.Count == 0)
                {
                    ThrowError(new NoPermissionDeleteThreadError(forum));
                    return false;
                }
            }

            List<int> userIDs = GetPostUserIDsFormThreads(tempThreadIDs);

            bool success = false;
            if (threadStatus != ThreadStatus.Recycled && threadStatus != ThreadStatus.UnApproved && updatePoint)
            {
                Dictionary<int, int> results = new Dictionary<int, int>();
                foreach (BasicThread thread in threads)
                {
                    if (!tempThreadIDs.Contains(thread.ThreadID))
                        continue;
                    if (results.ContainsKey(thread.PostUserID))
                        results[thread.PostUserID] += 1;
                    else
                        results.Add(thread.PostUserID, 1);
                }

                ForumPointType pointType = ForumPointType.DeleteAnyThreads;

                if (results.Count == 1 && results.ContainsKey(operatorUser.UserID))
                    pointType = ForumPointType.DeleteOwnThreads;

                success = ForumPointAction.Instance.UpdateUsersPoint(results, pointType, true, forumID, delegate(PointActionManager.TryUpdateUserPointState state)
                {
                    if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                    {
                        success = PostDaoV5.Instance.DeleteThreads(forumID, threadStatus, tempThreadIDs);
                        if (success)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                });
            }
            else
            {
                success = PostDaoV5.Instance.DeleteThreads(forumID, threadStatus, tempThreadIDs);
            }

            if (success)
            {
                FeedBO.Instance.DeleteFeeds(AppActionType.CreateTopic, tempThreadIDs);


                //只删除一个帖子
                if (tempThreadIDs.Count == 1)
                {
                    BasicThread deletedThread = null;
                    foreach (BasicThread thread in threads)
                    {
                        if (thread.ThreadID == tempThreadIDs[0])
                        {
                            deletedThread = thread;
                            break;
                        }
                    }

                    if (deletedThread != null)
                    {
                        Logs.LogManager.LogOperation(
                            new Topic_DeleteTopic(operatorUser.UserID, operatorUser.Name, operatorUser.LastVisitIP, deletedThread.ThreadID, deletedThread.PostUserID, forumID, deletedThread.PostUsername, deletedThread.SubjectText)
                        );
                    }
                }
                //批量删除帖子
                else
                {
                    Logs.LogManager.LogOperation(
                        new Topic_DeleteTopicByIDs(operatorUser.UserID, operatorUser.Name, forumID, operatorUser.LastVisitIP, tempThreadIDs)
                    );
                }

                ForumBO.Instance.ClearForumThreadCatalogsCache();
                ForumBO.Instance.ClearAllCache();

                ThreadCachePool.ClearAllCache();

                UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts, ActiveUserType.DayPosts, ActiveUserType.MonthPosts });

                //所有受影响的用户都更新缓存以便显示最新的主题数
                UserBO.Instance.RemoveUsersCache(userIDs);

                RemoveTopicSearchCount();

                ModeratorCenterAction action;
                if (threads.Count == 1 && threads[0].PostUserID == operatorUser.UserID)
                    action = ModeratorCenterAction.DeleteOwnThread;
                else
                    action = ModeratorCenterAction.DeleteThread;

                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, threads, action, actionReason);
                if(createThreadManageLog)
                    WriteThreadOperateLog(operatorUser, threads, action, actionReason);
            }

            return success;


        }


        #endregion

        #region 合并主题 JoinThreads


        /// <summary>
        /// 合并主题
        /// </summary>
        /// <param name="OldThreadID">要被合并到其它主题的主题ID</param>
        /// <param name="NewThreadID"></param>
        /// <param name="IsKeepLink"></param>
        /// <returns></returns>
        public bool JoinThreads(AuthUser operatorUser, int oldThreadID, int newThreadID, bool isKeepLink, bool ignorePermission, bool createThreadManageLog, bool sendNotify, string actionReason)
        {
            if (newThreadID == 0)
            {
                ThrowError<JoinThreadInvalidNewThreadIDError>(new JoinThreadInvalidNewThreadIDError(true));
                return false;
            }
            if (oldThreadID == newThreadID)
            {
                ThrowError<JoinThreadInvalidNewThreadIDError>(new JoinThreadInvalidNewThreadIDError(false));
                return false;
            }

            List<int> threadIdentities = new List<int>();
            threadIdentities.Add(oldThreadID);
            threadIdentities.Add(newThreadID);

            ThreadCollectionV5 threads = GetThreads(threadIdentities);

            BasicThread thread = null;
            BasicThread newThread = null;

            foreach (BasicThread tempThread in threads)
            {
                if (tempThread.ThreadID == oldThreadID)
                    thread = tempThread;
                else if (tempThread.ThreadID == newThreadID)
                    newThread = tempThread;
            }

            if (thread == null)
            {
                ThrowError<JoinThreadOldThreadNotExistsError>(new JoinThreadOldThreadNotExistsError());
                return false;
            }
            if (newThread == null)
            {
                ThrowError<JoinThreadNewThreadNotExistsError>(new JoinThreadNewThreadNotExistsError());
                return false;
            }

            Forum tempForum = ForumBO.Instance.GetForum(thread.ForumID, false);
            Forum newForum = ForumBO.Instance.GetForum(newThread.ForumID, false);

            List<int> forumIDs = new List<int>();
            forumIDs.Add(tempForum.ForumID);
            if (tempForum.ForumID != newForum.ForumID)
                forumIDs.Add(newForum.ForumID);

            if (!ignorePermission)
            {
                if (false == AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(tempForum.ForumID).Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.JoinThreads, thread.PostUserID))
                {
                    ThrowError(new NoPermissionJoinThreadError());
                    return false;
                }
                if (false == AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(newForum.ForumID).Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.JoinThreads, newThread.PostUserID))
                {
                    ThrowError(new NoPermissionJoinThreadError());
                    return false;
                }
            }
            bool success = PostDaoV5.Instance.JoinThreads(oldThreadID, newThreadID, isKeepLink);
            if (success)
            {
                UserBO.Instance.RemoveUserCache(thread.PostUserID);

                ThreadCachePool.ClearAllCache();
                ForumBO.Instance.ClearAllCache();
                ForumBO.Instance.ClearForumThreadCatalogsCache();


                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, threads, ModeratorCenterAction.JoinThread, actionReason);

                if (createThreadManageLog)
                {
                    ThreadCollectionV5 tempThreads = new ThreadCollectionV5();
                    tempThreads.Add(newThread);
                    WriteThreadOperateLog(operatorUser, tempThreads, ModeratorCenterAction.JoinThread, actionReason);
                }
            }

            return success;
        }

        #endregion

        #region 分割主题 SplitThread

        /// <summary>
        /// 分割主题
        /// </summary>
        /// <param name="threadID">原主题ID</param>
        /// <param name="postIdentities">要做为分割出来的主题的postID</param>
        /// <param name="newSubject">分割出来的主题标题</param>
        /// <returns></returns>
        public bool SplitThread(AuthUser operatorUser, int threadID, IEnumerable<int> postIDs, string newSubject, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {
            if (string.IsNullOrEmpty(newSubject))
            {
                ThrowError(new EmptyPostSubjectError("newSubject"));
                return false; 
            }


            BasicThread thread = GetThread(threadID);
            if (thread == null)
            {
                ThrowError(new SplitThreadThreadNotExistsError());
                return false;
            }

            if (!ignorePermission)
            {
                ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(thread.ForumID);

                ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(thread.ForumID);

                if (false == managePermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SplitThread, thread.PostUserID))
                {
                    ThrowError(new NoPermissionSplitThreadError());
                    return false;
                }

                int subjectLength = StringUtil.GetByteCount(newSubject);
                int subjectMaxLength = forumSetting.PostSubjectLengths[operatorUser].MaxValue;
                int subjectMinLength = forumSetting.PostSubjectLengths[operatorUser].MinValue;

                if (subjectMaxLength > 256)//数据库允许的长度
                    subjectMaxLength = 256;

                if (subjectLength > subjectMaxLength
                || subjectLength < subjectMinLength)
                {
                    ThrowError<InvalidPostSubjectLengthError>(new InvalidPostSubjectLengthError("newSubject", subjectMaxLength, subjectMinLength, subjectLength));
                    return false;
                }

            }
            if (thread.ThreadType == ThreadType.Poll)
            {
                ThrowError(new SplitThreadConnotSplitPollError());
                return false;
            }

            if (ValidateUtil.HasItems<int>(postIDs) == false)
            {
                ThrowError(new SplitThreadInvalidPostCountError());
                return false;
            }

            List<int> tempPostIDs = new List<int>(postIDs);
            PostCollectionV5 posts = GetPosts(threadID);
            if (tempPostIDs.Count >= posts.Count)
            {
                ThrowError(new CustomError("", "被分割出去的帖子数必须小于该主题的回复数"));
                return false;
            }

            bool success = PostDaoV5.Instance.SplitThread(threadID, postIDs, newSubject);

            if (success)
            {
                ForumBO.Instance.ClearAllCache();
                ThreadCachePool.ClearAllCache();
                ThreadCollectionV5 threads = new ThreadCollectionV5();
                threads.Add(thread);
                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, threads, ModeratorCenterAction.SplitThread, actionReason);

                if (createManageLog)
                    WriteThreadOperateLog(operatorUser, threads, ModeratorCenterAction.SplitThread, actionReason);
            }

            return success;
        }


        #endregion

        #region  删除帖子/回复

        public bool DeletePosts(AuthUser operatorUser, IEnumerable<int> postIDs, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {
            PostCollectionV5 posts = GetPosts(postIDs);
            Dictionary<int,PostCollectionV5> tempPosts = new Dictionary<int,PostCollectionV5>();
            Dictionary<int,int> forumIDs = new Dictionary<int,int>();
            foreach (PostV5 post in posts)
            {
                if (tempPosts.ContainsKey(post.ThreadID))
                {
                    tempPosts[post.ThreadID].Add(post);
                }
                else
                {
                    PostCollectionV5 t = new PostCollectionV5();
                    t.Add(post);
                    tempPosts.Add(post.ThreadID, t);
                    forumIDs.Add(post.ThreadID, post.ForumID);
                }
            }

            foreach (KeyValuePair<int, PostCollectionV5> pair in tempPosts)
            {
                if (false == DeletePosts(operatorUser, forumIDs[pair.Key], pair.Key, pair.Value, ignorePermission, createManageLog, sendNotify, actionReason))
                {
                    return false;
                }
            }

            return true;
        }

        public bool DeletePosts(AuthUser operatorUser, int forumID, int threadID, IEnumerable<int> postIDs, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {
            PostCollectionV5 posts = GetPosts(postIDs);
            return DeletePosts(operatorUser, forumID, threadID, posts, ignorePermission, createManageLog, sendNotify, actionReason);
        }
        /// <summary>
        /// 批量删除任何人回复
        /// </summary>
        /// <param name="postIdentities"></param>
        /// <returns></returns>
        private bool DeletePosts(AuthUser operatorUser, int forumID, int threadID, PostCollectionV5 posts, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {
            bool isDeleteAnyPost = true;

            if (posts.Count == 0)
                return true;

            List<int> tempPostIDs = new List<int>();

            if (!ignorePermission)
            {
                ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID);
                ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID);

                int count = 0;
                foreach (PostV5 post in posts)
                {
                    if (post.ThreadID != threadID || post.ForumID != forumID)
                    {
                        //tempPostIDs.Remove(post.PostID);
                        continue;
                    }

                    count++;

                    if (!post.IsApproved)
                    {
                        if (false == managePermission.Can(operatorUser, ManageForumPermissionSetNode.Action.ApprovePosts))
                        {
                            ThrowError(new NoPermissionDeletePostError());
                            return false;
                        }

                    }
                    else
                    {
                        if (false == managePermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts, post.UserID, post.LastEditorID))
                        {
                            if (post.UserID == operatorUser.UserID)
                            {
                                if (!permission.Can(operatorUser, ForumPermissionSetNode.Action.DeleteOwnPosts))
                                {
                                    ThrowError(new NoPermissionDeletePostError());
                                    return false;
                                }

                                isDeleteAnyPost = false;
                            }
                            else
                            {
                                ThrowError(new NoPermissionDeletePostError());
                                return false;
                            }
                        }
                    }
                }

                if (count == 0)
                {
                    ThrowError(new NoPermissionDeletePostError());
                    return false; 
                }
            }


            Dictionary<int, int> deleteResults = new Dictionary<int, int>();
            PostCollectionV5 tempPosts = new PostCollectionV5();
            Dictionary<int, PostCollectionV5> threadPosts = new Dictionary<int, PostCollectionV5>();
            foreach (PostV5 post in posts)
            {
                if (post.ThreadID != threadID || post.ForumID != forumID)
                {
                    continue;
                }

                tempPostIDs.Add(post.PostID);

                if (deleteResults.ContainsKey(post.UserID))
                    deleteResults[post.UserID] += 1;
                else
                    deleteResults.Add(post.UserID, 1);

                tempPosts.Add(post);

                if (threadPosts.ContainsKey(post.ThreadID))
                {
                    threadPosts[post.ThreadID].Add(post);
                }
                else
                {
                    PostCollectionV5 ps = new PostCollectionV5();
                    ps.Add(post);
                    threadPosts.Add(post.ThreadID, ps);
                }
            }
            ForumPointType pointType;
            ModeratorCenterAction action;
            if (deleteResults.Count == 1 && deleteResults.ContainsKey(operatorUser.UserID))
            {
                action = ModeratorCenterAction.DeletePostSelf;
                pointType = ForumPointType.DeleteOwnPosts;
            }
            else
            {
                action = ModeratorCenterAction.DeletePost;
                pointType = ForumPointType.DeleteAnyPosts;
            }

            bool success = ForumPointAction.Instance.UpdateUsersPoint(deleteResults, pointType, true, forumID, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    success = PostDaoV5.Instance.DeletePosts(forumID, threadID, tempPostIDs, operatorUser.UserID, isDeleteAnyPost);
                    if (success)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            });

            if (!success)
                return false;

            
            if (tempPostIDs.Count == 1)
            {
                PostV5 deletedPost = null;
                foreach (PostV5 post in posts)
                {
                    if (post.PostID == tempPostIDs[0])
                    {
                        deletedPost = post;
                        break;
                    }
                }

                Logs.LogManager.LogOperation(
                    new Topic_DeletePost(operatorUser.UserID, operatorUser.Name, operatorUser.LastVisitIP, deletedPost.PostID, forumID, deletedPost.UserID, deletedPost.Username, deletedPost.ContentText)
                );
            }
            else
            {
                Logs.LogManager.LogOperation(
                    new Topic_DeletePostByIDs(operatorUser.UserID, forumID, operatorUser.Name, operatorUser.LastVisitIP, tempPostIDs)
                );
            }

            


            RemovePostSearchCount();

            ForumBO.Instance.ClearAllCache();
            ThreadCachePool.ClearThreadPostCache(new int[] { threadID });

            ThreadCachePool.UpdateCache(new int[] { threadID });

            ClearHotThreadCache(forumID, new int[] { threadID });

            UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts, ActiveUserType.DayPosts, ActiveUserType.MonthPosts });

            List<int> userIDs = new List<int>();
            foreach (PostV5 post in posts)
            {
                if (!userIDs.Contains(post.UserID))
                {
                    userIDs.Add(post.UserID);
                }
            }
            UserBO.Instance.RemoveUsersCache(userIDs);

            if (sendNotify)
            {
                foreach (KeyValuePair<int, PostCollectionV5> pair in threadPosts)
                {
                    SendManagerOperationNotify(operatorUser, null, pair.Key, pair.Value, action, actionReason);
                }
            }
            if (createManageLog)
                WriteThreadOperateLog(operatorUser, tempPosts, action, actionReason);

            return true;
        }


        #endregion

        #region 批量回收主题
        public bool RecycleThreads(AuthUser operatorUser, IEnumerable<int> threadIDs, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {
            Dictionary<int, List<int>> forumIDThreadIDs = GetForumIDsThreadIDs(threadIDs);
            foreach (KeyValuePair<int, List<int>> pair in forumIDThreadIDs)
            {
                if (RecycleThreads(operatorUser, pair.Key, pair.Value, ignorePermission, createManageLog, sendNotify, actionReason) == false)
                    return false;
            }

            return true;
        }
        //public bool RecycleThreads(AuthUser operatorUser, int forumID, IEnumerable<int> threadIds,bool sendNotify, string actionReason)
        //{
        //    return RecycleThreads(operatorUser, forumID, threadIds, false, sendNotify, actionReason);
        //}


        /// <summary>
        /// 回收主题
        /// </summary>
        /// <param name="threadIdentities"></param>
        /// <returns></returns>
        public bool RecycleThreads(AuthUser operatorUser, int forumID, IEnumerable<int> threadIDs, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {

            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);


            ThreadCollectionV5 threads = GetThreads(threadIDs);
            if (threads.Count == 0)
                return true;

            List<int> tempThreadIDs = new List<int>(threadIDs);

            if (!ignorePermission)
            {
                ManageForumPermissionSetNode managerPermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID);
                ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID);

                TimeSpan? interval = null;
                if (forumSetting.RecycleOwnThreadsIntervals[operatorUser] > 0)
                {
                    interval = TimeSpan.FromSeconds(forumSetting.RecycleOwnThreadsIntervals[operatorUser]);
                }
                foreach (BasicThread thread in threads)
                {
                    if (false == managerPermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled, thread.PostUserID))
                    {
                        if (thread.PostUserID == operatorUser.UserID)
                        {
                            if (!permission.Can(operatorUser, ForumPermissionSetNode.Action.RecycleAndRestoreOwnThreads))
                            {
                                ThrowError(new NoPermissionRecycleThreadError());
                                return false;
                            }
                            //开始处理删除自己帖子的情况
                            if (interval != null)
                            {
                                if (DateTimeUtil.Now - thread.CreateDate > interval.Value)
                                {
                                    tempThreadIDs.Remove(thread.ThreadID);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            ThrowError(new NoPermissionRecycleThreadError());
                            return false;
                        }
                    }
                }

                if (tempThreadIDs.Count == 0)
                {
                    ThrowError(new NoPermissionRecycleThreadError());
                    return false;
                }
            }


            Dictionary<int, int> results = new Dictionary<int, int>();
            ThreadCollectionV5 tempThreads = new ThreadCollectionV5();
            foreach (BasicThread thread in threads)
            {
                if (tempThreadIDs.Contains(thread.ThreadID) == false)
                    continue;

                if (thread.ForumID != forumID)
                {
                    tempThreadIDs.Remove(thread.ThreadID);
                    continue;
                }

                tempThreads.Add(thread);

                if (results.ContainsKey(thread.PostUserID))
                    results[thread.PostUserID] += 1;
                else
                    results.Add(thread.PostUserID, 1);
            }

            if (tempThreadIDs.Count == 0)
            {
                return true;
            }

            ForumPointType pointType = ForumPointType.DeleteAnyThreads;
            if (results.Count == 1 && results.ContainsKey(operatorUser.UserID))
                pointType = ForumPointType.DeleteOwnThreads;

            bool success = ForumPointAction.Instance.UpdateUsersPoint(results, pointType, true, forumID, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    if (PostDaoV5.Instance.SetThreadsStatus(forumID, null, tempThreadIDs, ThreadStatus.Recycled, null))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            });

            if (success)
            {

                RemoveTopicSearchCount();
                List<int> userIDs = GetPostUserIDsFormThreads(tempThreadIDs);
                UserBO.Instance.RemoveUsersCache(userIDs);

                ForumBO.Instance.ClearAllCache();
                ThreadCachePool.ClearAllCache();

                ForumBO.Instance.ClearForumThreadCatalogsCache();

                FeedBO.Instance.DeleteFeeds(AppActionType.CreateTopic, tempThreadIDs);

                ModeratorCenterAction action;
                if (tempThreads.Count == 1 && tempThreads[0].PostUserID == operatorUser.UserID)
                    action = ModeratorCenterAction.RecycleOwnThread;
                else
                    action = ModeratorCenterAction.RecycleThread;

                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, tempThreads, action, actionReason);
                if (createManageLog)
                    WriteThreadOperateLog(operatorUser, tempThreads, action, actionReason);
            }

            return success;
        }



        #endregion

        #region  发送操作通知

        private void WriteThreadOperateLog(AuthUser operatorUser, PostCollectionV5 posts, ModeratorCenterAction Action, string actionReason)
        {
            List<int> threadids = new List<int>();
            foreach (PostV5 p in posts)
                if (!threadids.Contains(p.ThreadID)) threadids.Add(p.ThreadID);
            ThreadCollectionV5 threads = GetThreads(threadids);
            WriteThreadOperateLog(operatorUser, threads, Action, actionReason);
        }


        /// <summary>
        /// 帖子操作日志
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="threads"></param>
        /// <param name="Action"></param>
        /// <param name="actionReason"></param>
        private void WriteThreadOperateLog(AuthUser operatorUser,ThreadCollectionV5 threads, ModeratorCenterAction Action, string actionReason)
        {
            bool threadManagelogIsPublic  =true;

            switch (Action)
            {
                case ModeratorCenterAction.RescindShieldPost:
                case ModeratorCenterAction.ShieldPost:
                case ModeratorCenterAction.ApprovePost:
                case ModeratorCenterAction.ApprovePostByThreadIDs:
                case ModeratorCenterAction.DeletePost:
                case ModeratorCenterAction.DeleteUnapprovedPost:
                    threadManagelogIsPublic = false;
                    break;
            }


            List<int> postUserIDs = new List<int>();
            List<string> subjects = new List<string>();
            List<int> threadIDs = new List<int>();
            int forumID = 0;
            foreach (BasicThread thread in threads)
            {
                forumID = thread.ForumID;
                postUserIDs.Add(thread.PostUserID);
                subjects.Add(thread.SubjectText);
                threadIDs.Add(thread.ThreadID);
            }

            string threadLog;
            CreateThreadManageLog(operatorUser, operatorUser.LastVisitIP, Action, postUserIDs, forumID, threadIDs, subjects, actionReason, threadManagelogIsPublic, out threadLog);

            if (threadLog != null)
            {
                foreach (BasicThread thread in threads)
                {
                    thread.ThreadLog = threadLog;
                }
            }
        }

        private void SendManagerOperationNotify(AuthUser operatorUser, ThreadCollectionV5 threads,  ModeratorCenterAction Action,  string reason)
        {
            SendManagerOperationNotify(operatorUser, threads, 0, null, Action,  reason);
        }

        private void SendManagerOperationNotify(AuthUser operatorUser, PostCollectionV5 posts, ModeratorCenterAction Action,  string reason)
        {
            SendManagerOperationNotify(operatorUser, null, 0, posts, Action,  reason);
        }

        private void SendManagerOperationNotify(AuthUser operatorUser, ThreadCollectionV5 threads, int threadID, PostCollectionV5 posts, ModeratorCenterAction Action, string reason)
        {
            string actionTitle = string.Empty;
            switch (Action)
            {
                case ModeratorCenterAction.UnJudgementTread:
                    actionTitle = "取消主题鉴定";
                    break;
                case ModeratorCenterAction.JudgementThread:
                    actionTitle = "鉴定主题";
                    break;
                case ModeratorCenterAction.RescindShieldUser:
                    actionTitle = "解除屏蔽用户";
                    break;

                case ModeratorCenterAction.ShieldUser:
                    actionTitle = "屏蔽用户";
                    break;

                case ModeratorCenterAction.RescindShieldPost:
                    actionTitle = "解除屏蔽帖子";
                    break;

                case ModeratorCenterAction.ShieldPost:
                    actionTitle = "屏蔽帖子";
                    break;
                case ModeratorCenterAction.DeletePost:
                case ModeratorCenterAction.DeleteUnapprovedPost:
                    actionTitle = "删除回复";
                    break;
                case ModeratorCenterAction.DeleteUnapprovedPostByThreadIDs:
                    actionTitle = "删除主题的未审核回复";
                    break;
                case ModeratorCenterAction.ApprovePost:
                    actionTitle = "审核回复";
                    break;
                case ModeratorCenterAction.ApprovePostByThreadIDs:
                    actionTitle = "审核该主题的所有回复";
                    break;
                case ModeratorCenterAction.RecycleThread:
                    actionTitle = "回收主题";
                    break;
                case ModeratorCenterAction.RevertThread:
                    actionTitle = "还原主题";
                    break;

                case ModeratorCenterAction.DeleteThread:
                    actionTitle = "删除主题";
                    break;

                case ModeratorCenterAction.JoinThread:
                    actionTitle = "合并主题";
                    break;

                case ModeratorCenterAction.MoveThread:
                    actionTitle = "移动主题";
                    break;

                case ModeratorCenterAction.LockThread:
                    actionTitle = "锁定主题";
                    break;

                case ModeratorCenterAction.SetThreadSubjectStyle:
                    actionTitle = "设置主题标题样式";
                    break;
                case ModeratorCenterAction.SetThreadIsTop:
                    actionTitle = "置顶主题";
                    break;

                case ModeratorCenterAction.UpThread:
                    actionTitle = "提升主题";
                    break;

                case ModeratorCenterAction.CheckThread:
                    actionTitle = "审核主题";
                    break;

                case ModeratorCenterAction.SetThreadElite:
                    actionTitle = "设置精华";
                    break;

                case ModeratorCenterAction.UpdateThreadCatalog:
                    actionTitle = "设置分类";
                    break;

                case ModeratorCenterAction.SetThreadNotUpdateSortOrder:
                    actionTitle = "设置自动沉帖";
                    break;

                case ModeratorCenterAction.SplitThread:
                    actionTitle = "分割主题";
                    break;

                case ModeratorCenterAction.CancelThreadSubjectStyle:
                    actionTitle = "取消标题样式";
                    break;

                case ModeratorCenterAction.CancelTop:
                    actionTitle = "取消置顶";
                    break;

                case ModeratorCenterAction.CancelValued:
                    actionTitle = "取消精华";
                    break;

                default:
                    break;
            }



            Dictionary<int, string> subjects = null, urls = null, nickNames = null;
            switch (Action)
            {
                case ModeratorCenterAction.RescindShieldUser:
                    break;
                case ModeratorCenterAction.ShieldUser:
                    break;
                case ModeratorCenterAction.RescindShieldPost:
                case ModeratorCenterAction.ShieldPost:
                case ModeratorCenterAction.ApprovePost:
                case ModeratorCenterAction.ApprovePostByThreadIDs:
                case ModeratorCenterAction.DeletePost:
                case ModeratorCenterAction.DeleteUnapprovedPost:
                    subjects = new Dictionary<int, string>();
                    urls = new Dictionary<int, string>();
                    nickNames = new Dictionary<int, string>();

                    string threadSubject;
                    BasicThread tempThread = GetThread(threadID);
                    if (tempThread != null)
                    {
                        threadSubject = "您对主题“" + tempThread.SubjectText + "”的回复:";
                    }
                    else
                    {
                        threadSubject = string.Empty;
                    }
                    foreach (PostV5 post in posts)
                    {
                        if (subjects.ContainsKey(post.UserID))
                        {
                            subjects[post.UserID] += "," + post.SubjectText;
                        }
                        else
                        {
                            subjects.Add(post.UserID, threadSubject + post.SubjectText);
                            urls.Add(post.UserID, BbsUrlHelper.GetThreadUrl(post.Forum.CodeName, threadID, 1));
                            nickNames.Add(post.UserID, post.Username);
                        }
                    }

                    break;
                case ModeratorCenterAction.JoinThread:
                case ModeratorCenterAction.SplitThread:
                case ModeratorCenterAction.CheckThread:
                case ModeratorCenterAction.RecycleThread:
                case ModeratorCenterAction.RevertThread:
                case ModeratorCenterAction.DeleteThread:
                case ModeratorCenterAction.MoveThread:
                case ModeratorCenterAction.LockThread:
                case ModeratorCenterAction.SetThreadSubjectStyle:
                case ModeratorCenterAction.SetThreadIsTop:
                case ModeratorCenterAction.UpThread:
                case ModeratorCenterAction.SetThreadElite:
                case ModeratorCenterAction.UpdateThreadCatalog:
                case ModeratorCenterAction.UnJudgementTread:
                case ModeratorCenterAction.JudgementThread:
                case ModeratorCenterAction.CancelValued:
                case ModeratorCenterAction.CancelTop:
                case ModeratorCenterAction.CancelThreadSubjectStyle:

                    subjects = new Dictionary<int, string>();
                    urls = new Dictionary<int, string>();
                    foreach (BasicThread thread in threads)
                    {
                        if (subjects.ContainsKey(thread.PostUserID))
                        {
                            subjects[thread.PostUserID] += "," + thread.SubjectText;
                        }
                        else
                        {
                            subjects.Add(thread.PostUserID,"您的帖子："+ thread.SubjectText);
                            urls.Add(thread.PostUserID, BbsUrlHelper.GetThreadUrl(thread.Forum.CodeName, thread.ThreadID, thread.ThreadTypeString, 1));
                        }
                    }
                    break;
                default: break;
            }
            if (subjects != null && Action != ModeratorCenterAction.RescindShieldUser && Action != ModeratorCenterAction.ShieldUser && Action != ModeratorCenterAction.UpdateForumReadme && Action != ModeratorCenterAction.GetThreadManageLog && Action != ModeratorCenterAction.GetShieldedUsersLog)//屏蔽用户不发送短消息
            {
                foreach (KeyValuePair<int, string> pair in subjects)
                {
                    int userID = pair.Key;

                    User profile = UserBO.Instance.GetUser(userID);

                    if (profile == null)
                        continue;
                    if (pair.Key == operatorUser.UserID || pair.Key == 0)//不给自己发送
                        continue;

                    string content = string.Empty;
                    if (Action == ModeratorCenterAction.DeleteThread||Action== ModeratorCenterAction.RecycleThread)
                    {
                        content = @"
{0} 已经被 {1} 执行操作:{2} {3}
";
                        content = string.Format(content, pair.Value
                        , operatorUser.PopupNameLink
                        , actionTitle
                        , string.IsNullOrEmpty(reason) ? string.Empty : "原因:" + reason + "<br />"
                        );
                    }
                    else
                    {
                        content = @"
                            <a href=""{5}{1}"" target=""_blank"">{0}</a> 已经被 {2} 执行操作:{3} {4}
                            ";
                        content = string.Format(content, pair.Value
, urls[pair.Key]
                            // , "<a href=\"" + urls[pair.Key] + "\"  target=\"_blank\">" + urls[pair.Key] + "</a>"
, operatorUser.PopupNameLink
, actionTitle
, string.IsNullOrEmpty(reason) ? string.Empty : "原因:" + reason + "<br />"
, "{clienturl}"
);
                    }



                    //您的帖子已经被管理员处理<br />
                    //帖子标题:{0}<br />
                    //帖子地址:{1}<br />
                    //操作者:{2}<br />
                    //操作:{3}<br />{4}
                    //";

                    Notify notify = new AdminManageNotify(operatorUser.UserID, content);
                    notify.UserID = userID;
                    NotifyBO.Instance.AddNotify(operatorUser, notify);
                }
            }
        }


        #endregion

        #region

        /// <summary>
        /// 获取发帖用户ＩＤ，（用户ID有重复）
        /// </summary>
        /// <param name="threadIDs"></param>
        /// <returns></returns>
        public List<int> GetUserIdentitiesByThreadIdentities(IEnumerable<int> threadIDs)
        {
            return GetUserIdentitiesByThreadIdentities(threadIDs, true);
        }

        public List<int> GetUserIdentitiesByThreadIdentities(IEnumerable<int> threadIDs, bool includeRepeated)
        {
            ThreadCollectionV5 threads = GetThreads(threadIDs);
            List<int> userIDs = new List<int>();
            foreach (BasicThread thread in threads)
            {
                if (includeRepeated)
                    userIDs.Add(thread.PostUserID);
                else if (!userIDs.Contains(thread.PostUserID))
                {
                    userIDs.Add(thread.PostUserID);
                }
            }
            return userIDs;
        }

        public List<int> GetUserIdentitiesByPostIdentities(List<int> postIDs, bool includeRepeated)
        {
            PostCollectionV5 posts = GetPosts(postIDs);

            List<int> userIDs = new List<int>();
            foreach (PostV5 post in posts)
            {
                if (includeRepeated)
                    userIDs.Add(post.UserID);
                else if (!userIDs.Contains(post.UserID))
                {
                    userIDs.Add(post.UserID);
                }
            }
            return userIDs;
        }

        #region 鉴定主题
        public bool JudgementThreads(AuthUser operatorUser, IEnumerable<int> threadIds, int forumID, int judgementID, bool ignorePermission, bool sendNotify, bool createLog, string actionReason)
        {
            if (!ignorePermission)
            {
                List<int> posterIDs = GetUserIdentitiesByThreadIdentities(threadIds, false);

                if (posterIDs != null && posterIDs.Count < 1)
                {
                    ManageForumPermissionSetNode managerPermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID);

                    if (managerPermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.JudgementThreads) == false)
                    {
                        ThrowError<NoPermissionJudgementThreadError>(new NoPermissionJudgementThreadError(null));
                        return false;
                    }
                    foreach (int userID in posterIDs)
                    {
                        if (false == managerPermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.JudgementThreads, userID))
                        {
                            ThrowError<NoPermissionJudgementThreadError>(new NoPermissionJudgementThreadError(userID));
                            return false;
                        }
                    }

                }
            }
            bool success = PostDaoV5.Instance.JudgementThreads(threadIds, forumID, judgementID);
            if (success)
            {
                ThreadCachePool.UpdateCache(threadIds);

                ThreadCollectionV5 threads = null;

                if (sendNotify || createLog)
                    threads = GetThreads(threadIds);

                ModeratorCenterAction action;
                if (judgementID > 0)
                    action = ModeratorCenterAction.JudgementThread;
                else
                    action = ModeratorCenterAction.UnJudgementTread;

                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, threads, action, actionReason);

                if (createLog)
                    WriteThreadOperateLog(operatorUser, threads, action, actionReason);

                return true;
            }
            else
                return false;
        }

        #endregion
        #endregion


        #region 锁定/解锁主题 SetThreadsLockStatus

        public bool SetThreadsLock(AuthUser operatorUser, IEnumerable<int> threadIDs, bool lockThread, DateTime? endDate, bool ignorePermission, bool isUseProp, bool sendNotify, string actionReason)
        {
            Dictionary<int, List<int>> forumThreads = GetForumIDsThreadIDs(threadIDs);

            foreach(KeyValuePair<int,List<int>> group in forumThreads)
            {
                if (false == SetThreadsLock(operatorUser, group.Key, group.Value, lockThread, endDate,ignorePermission, isUseProp, sendNotify, actionReason))
                    return false;
            }
            return true;
        }

        public bool SetThreadsLock(AuthUser operatorUser, int forumID, IEnumerable<int> threadIDs, bool lockThread, DateTime? endDate, bool ignorePermission, bool isUseProp, bool sendNotify, string actionReason)
        {
            bool isSuccess = false;

            isSuccess = SetThreadsLock(operatorUser, forumID, threadIDs, lockThread, ignorePermission, isUseProp == false, sendNotify, actionReason);

            if (isSuccess && endDate != null)
            {
                if (lockThread)
                {
                    CreateTopicStatus(threadIDs, TopicStatuType.Lock, endDate.Value);
                }
                else
                    DeleteTopicStatus(threadIDs, TopicStatuType.Lock);
            }
            else if (isSuccess && endDate == null)
            {
                DeleteTopicStatus(threadIDs, TopicStatuType.Lock);
            }

            if (isSuccess && isUseProp)
            {
                CreateThreadManageLogs(forumID, threadIDs, ModeratorCenterAction.SetThreadLockByUseProp, endDate);
            }
            return isSuccess;
        }


        /// <summary>
        /// 解锁/锁定的主题
        /// </summary>
        /// <param name="threadIdentities"></param>
        /// <param name="lockThread"></param>
        /// <returns></returns>
        private bool SetThreadsLock(AuthUser operatorUser, int forumID, IEnumerable<int> threadIDs, bool lockThread, bool ignorePermission, bool createManage, bool sendNotify, string actionReason)
        {
            ThreadCollectionV5 threads = GetThreads(threadIDs);

            ManageForumPermissionSetNode permisionSet = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID);
            ThreadCollectionV5 tempThreads = new ThreadCollectionV5();
            foreach (BasicThread thread in threads)
            {
                if (thread.ForumID != forumID)
                    continue;

                if (!ignorePermission)
                {
                    if (false == permisionSet.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsLock, thread.PostUserID))
                    {
                        ThrowError(new NoPermissionLockThreadError(lockThread));
                        return false;
                    }
                }
                tempThreads.Add(thread);
            }


            bool success = PostDaoV5.Instance.SetThreadsLock(forumID, threadIDs, lockThread);
            if (success)
            {
                ThreadCachePool.UpdateCache(threadIDs);

                ModeratorCenterAction action;

                if (lockThread)
                    action = ModeratorCenterAction.LockThread;
                else
                    action = ModeratorCenterAction.UnlockThread;

                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, tempThreads, action, actionReason);

                if (createManage)
                    WriteThreadOperateLog(operatorUser, tempThreads, action, actionReason);
                return true;
            }
            else
                return false;
        }


        #endregion

        #region 自动沉帖子

        /// <summary>
        /// 自动沉帖（仍然可以回复，但不会被顶到前面）
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="threadIdentities"></param>
        /// <returns></returns>
        public bool SetThreadNotUpdateSortOrder(AuthUser operatorUser, int forumID, IEnumerable<int> threadIds, bool updateSortOrder, bool ignorePermission, bool createManangeLog, bool sendNotify, string actionReason)
        {
            ThreadCollectionV5 threads = GetThreads(threadIds);

            if (threads.Count == 0)
            {
                ThrowError(new SetThreadNotUpdateSortOrderNotSellectThreadError());
                return false;
            }

            ThreadCollectionV5 tempThreads = new ThreadCollectionV5();
            ManageForumPermissionSetNode permissonSet = GetForumPermissionSet(forumID);
            foreach (BasicThread thread in threads)
            {
                if (thread.ForumID != forumID)
                    continue;

                if (!ignorePermission)
                {
                    if (false == permissonSet.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetThreadNotUpdateSortOrder, thread.PostUserID))
                    {
                        ThrowError(new NoPermissionSetThreadNotUpdateSortOrderError());
                        return false;
                    }
                }

                tempThreads.Add(thread);
            }

            bool success = PostDaoV5.Instance.SetThreadNotUpdateSortOrder(forumID, threadIds, updateSortOrder);
            if (success)
            {
                if (sendNotify)
                {
                    SendManagerOperationNotify(operatorUser, tempThreads, ModeratorCenterAction.SetThreadNotUpdateSortOrder, actionReason);
                }

                if (createManangeLog)
                    WriteThreadOperateLog(operatorUser, tempThreads, ModeratorCenterAction.SetThreadNotUpdateSortOrder, actionReason);

                ForumBO.Instance.ClearAllCache();
                ThreadCachePool.UpdateCache(threadIds);
            }

            return success;
        }

        #endregion

        #region 设置置顶状态 SetThreadStickyStatus / SetThreadsStickyStatus

       /// <summary>
       /// 
       /// </summary>
       /// <param name="operatorUser"></param>
       /// <param name="forumID"></param>
        /// <param name="stickForumIDs">要在哪些版块置顶 如果只在当前版块置顶 传NULL</param>
       /// <param name="threadIdentities"></param>
       /// <param name="stickyStatus"></param>
       /// <param name="endDate">结束时间</param>
       /// <param name="ignorePermission"></param>
       /// <param name="isUseProp">是否是用道具置顶的</param>
       /// <param name="sendNotify"></param>
       /// <param name="actionReason"></param>
       /// <returns></returns>
       public bool SetThreadsStickyStatus(AuthUser operatorUser, int forumID, IEnumerable<int> stickForumIDs, IEnumerable<int> threadIdentities, ThreadStatus stickyStatus, DateTime? endDate, bool ignorePermission, bool isUseProp, bool sendNotify, string actionReason)
       {
           bool success = SetThreadsStickyStatus(operatorUser, forumID, stickForumIDs, threadIdentities, stickyStatus, ignorePermission, sendNotify, isUseProp == false, actionReason);
           if (success && endDate != null)
           {
               if (stickyStatus == ThreadStatus.GlobalSticky || stickyStatus == ThreadStatus.Sticky)
               {
                   CreateTopicStatus(threadIdentities, TopicStatuType.Stick, endDate.Value);
               }
               else
                   DeleteTopicStatus(threadIdentities, TopicStatuType.Stick);
           }
           else if (success && endDate == null)
           {
               DeleteTopicStatus(threadIdentities, TopicStatuType.Stick);
           }

           if (success && isUseProp)
           {
               CreateThreadManageLogs(forumID, threadIdentities, ModeratorCenterAction.SetThreadStickByUseProp, endDate);
           }

           return success;
       }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="forumID"></param>
        /// <param name="stickForumIDs">要在哪些版块置顶 如果只在当前版块置顶 传NULL</param>
        /// <param name="threadIdentities"></param>
        /// <param name="stickyStatus"></param>
        /// <param name="ignorePermission"></param>
        /// <param name="sendNotify"></param>
        /// <param name="createLog"></param>
        /// <param name="actionReason"></param>
        /// <returns></returns>
        private bool SetThreadsStickyStatus(AuthUser operatorUser, int forumID, IEnumerable<int> stickForumIDs, IEnumerable<int> threadIdentities, ThreadStatus stickyStatus, bool ignorePermission,bool sendNotify, bool createLog,string actionReason)
        {
            if (stickyStatus == ThreadStatus.Recycled || stickyStatus == ThreadStatus.UnApproved)
                return  false ;
            ThreadCollectionV5 threads = GetThreads(threadIdentities);
            List<int> threadIDs = new List<int>(threadIdentities);
            if (!ignorePermission)
            {
                ManageForumPermissionSetNode permissionSet = GetForumPermissionSet(forumID);
                Forum forum = ForumBO.Instance.GetForum(forumID);
                if (forum == null)
                    return false;

                if (stickyStatus == ThreadStatus.GlobalSticky)//设为总置顶
                {
                    if (false ==permissionSet.HasPermissionForSomeone(operatorUser , ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick))
                    {
                        ThrowError(new NoPermissionGlobalStickAnyThreadError());
                        return false;
                    }
                }
                else
                {
                    if (stickyStatus == ThreadStatus.Normal)//设为正常状态
                    {
                        foreach (BasicThread thread in threads)
                        {
                            if (thread.ForumID != forumID && threadIDs.Contains(thread.ThreadID))
                            {
                                threadIDs.Remove(thread.ThreadID);
                            }

                            if (thread.ThreadStatus == ThreadStatus.GlobalSticky)//原来是总置顶的 
                            {
                                //是否有撤消总置顶的权限
                                if (false == permissionSet.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick, thread.PostUserID))
                                {
                                    ThrowError(new NoPermissionGlobalStickAnyThreadError());
                                    return false;
                                }
                            }
                            else///原来不是总置顶的
                            {
                                //是否有撤消置顶的权限
                                if (false == permissionSet.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsStick, thread.PostUserID))
                                {
                                    ThrowError(new NoPermissionGlobalStickAnyThreadError());
                                    return false;
                                }
                            }
                        }
                    }
                    else if (stickyStatus == ThreadStatus.Sticky)//设为置顶
                    {
                        foreach (BasicThread thread in threads)
                        {
                            if (thread.ForumID != forumID && threadIDs.Contains(thread.ThreadID))
                            {
                                threadIDs.Remove(thread.ThreadID);
                            }

                            if (thread.ThreadStatus == ThreadStatus.GlobalSticky)//原来是总置顶的 
                            {
                                //是否有撤消总置顶以及设为置顶的权限
                                if (false == permissionSet.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick, thread.PostUserID))
                                {
                                    ThrowError(new NoPermissionGlobalStickAnyThreadError());
                                    return false;
                                }
                            }
                            //else//原来不是总置顶的
                            //{
                            if (false == permissionSet.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsStick, thread.PostUserID))
                            {
                                return false;
                            }
                            //}
                        }
                    }
                }
            }

            List<int> tempForumIDs = null;
            if (stickyStatus == ThreadStatus.Sticky && stickForumIDs != null)
            {
                tempForumIDs = new List<int>(stickForumIDs);
                if (tempForumIDs.Contains(forumID))
                    tempForumIDs.Remove(forumID);
            }

            StickSortType? sortType;
            if (stickyStatus == ThreadStatus.Sticky)
                sortType = AllSettings.Current.BbsSettings.StickSortType;
            else if (stickyStatus == ThreadStatus.GlobalSticky)
                sortType = AllSettings.Current.BbsSettings.GloableStickSortType;
            else
                sortType = null;

            bool success = PostDaoV5.Instance.SetThreadsStatus(forumID, tempForumIDs, threadIDs, stickyStatus, sortType);
            if (success)
            {
                ClearAllStickThreadInForumsCache();
                ForumBO.Instance.ClearAllCache();
                ThreadCachePool.ClearAllCache();

                ModeratorCenterAction action;
                if (stickyStatus == ThreadStatus.Normal)
                    action = ModeratorCenterAction.CancelTop;
                else
                    action = ModeratorCenterAction.SetThreadIsTop;

                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, threads, action, actionReason);

                if (createLog)
                    WriteThreadOperateLog(operatorUser, threads, action, actionReason);

            }

            return success;
        }

        #endregion

        #region 更改主题分类 UpdateThreadCatalog

        public bool UpdateThreadCatalog(AuthUser operatorUser, int forumID, IEnumerable<int> threadIDs, int threadCatalogID, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {
            ThreadCollectionV5 threads = GetThreads(threadIDs);

            
            Forum forum = ForumBO.Instance.GetForum(forumID, false);

            if (threadCatalogID != 0 && forum.ThreadCatalogStatus == ThreadCatalogStatus.DisEnable)
            {
                ThrowError(new UpdateThreadCatalogDisabledError());
                return false;
            }
            if (threadCatalogID == 0 && forum.ThreadCatalogStatus == ThreadCatalogStatus.EnableAndMust)
            {
                ThrowError(new UpdateThreadCatalogMustSellectThreadCatalogError());
                return false;
            }

            ThreadCollectionV5 tempThreads = new ThreadCollectionV5();

            ManageForumPermissionSetNode permissionSet = GetForumPermissionSet(forum.ForumID);

            foreach (BasicThread thread in threads)
            {
                if (thread.ForumID != forumID)
                    continue;

                if (!ignorePermission)
                {
                    if (false == permissionSet.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreadCatalog, thread.PostUserID))
                    {
                        ThrowError(new NoPermissionUpdateThreadCatalogError());
                        return false;
                    }
                }
                tempThreads.Add(thread);
            }
            if (tempThreads.Count == 0)
            {
                ThrowError(new UpdateThreadCatalogNotSellectThreadError());
                return false;
            }

            bool success = PostDaoV5.Instance.UpdateThreadCatalog(forumID, threadIDs, threadCatalogID);
            if (success)
            {
                ForumBO.Instance.ClearForumThreadCatalogsCache();
                ThreadCachePool.UpdateCache(threadIDs);
                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, threads, ModeratorCenterAction.UpdateThreadCatalog, actionReason);
                if (createManageLog)
                    WriteThreadOperateLog(operatorUser, threads, ModeratorCenterAction.UpdateThreadCatalog, actionReason);

            }

            return success;
        }

        #endregion

        #region 添加标题样式 SetThreadsSubjectStyle / SetThreadSubjectStyle

        public bool SetThreadsSubjectStyle(AuthUser operatorUser, int forumID, IEnumerable<int> threadIDs, string style, DateTime? endDate, bool ignorePermission, bool isUseProp, bool sendNotify,string actionReason)
        {
            bool success = SetThreadsSubjectStyle(operatorUser, forumID, threadIDs, style, ignorePermission, isUseProp == false, sendNotify, actionReason);

            if (success  && endDate != null)
            {
                if (string.IsNullOrEmpty(style) == false)
                {
                    CreateTopicStatus(threadIDs, TopicStatuType.HeightLight, endDate.Value);
                }
                else
                    DeleteTopicStatus(threadIDs, TopicStatuType.HeightLight);
            }
            else if (success && endDate == null)
            {
                DeleteTopicStatus(threadIDs, TopicStatuType.HeightLight);
            }

            if (success && isUseProp)
            {
                CreateThreadManageLogs(forumID, threadIDs, ModeratorCenterAction.SetThreadSubjectStyleByUseProp, endDate);
            }

            return success;
        }


        /// <summary>
        /// 添加标题样式
        /// </summary>
        /// <param name="threadIdentities"></param>
        /// <param name="Style"></param>
        /// <returns></returns>
        private bool SetThreadsSubjectStyle(AuthUser operatorUser, int forumID, IEnumerable<int> threadIDs, string style, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {
            ManageForumPermissionSetNode permissionSet = GetForumPermissionSet(forumID);

            if (!ignorePermission)
            {
                if (false == permissionSet.Can(operatorUser, ManageForumPermissionSetNode.Action.SetThreadsSubjectStyle))
                {
                    ThrowError(new NoPermissionSetThreadSubjectStyleError());
                    return false;
                }
            }

            bool success = PostDaoV5.Instance.SetThreadsSubjectStyle(forumID, threadIDs, style);


            if (success)
            {
                ThreadCachePool.UpdateCache(threadIDs);
                ThreadCollectionV5 threads = null;

                if (sendNotify || createManageLog)
                {
                    threads = new ThreadCollectionV5();
                    ThreadCollectionV5 tempThreads = GetThreads(threadIDs);

                    foreach (BasicThread thread in tempThreads)
                    {
                        if (thread.ForumID != forumID)
                            continue;
                        threads.Add(thread);
                    }
                }

                ModeratorCenterAction action;
                if (string.IsNullOrEmpty(style))
                    action = ModeratorCenterAction.CancelThreadSubjectStyle;
                else
                    action = ModeratorCenterAction.SetThreadSubjectStyle;

                if (sendNotify)
                {

                    SendManagerOperationNotify(operatorUser, threads, action, actionReason);
                }
                if(createManageLog)
                    WriteThreadOperateLog(operatorUser, threads, action, actionReason);

            }

            return success;
        }

        #region 提升主题 SetThreadsUp


        /// <summary>
        /// 提升主题
        /// </summary>
        /// <param name="threadID"></param>
        /// <returns></returns>
        public  bool SetThreadsUp(AuthUser operatorUser, int forumID, IEnumerable<int> threadIDs, bool ignorePermission,bool isUseProp,bool sendNotify,string actionReason)
        {
            ManageForumPermissionSetNode permissionSet = GetForumPermissionSet(forumID);
            if (!ignorePermission)
            {
                if (false == permissionSet.Can(operatorUser, ManageForumPermissionSetNode.Action.SetThreadsUp))
                {
                    ThrowError(new NoPermissionSetThreadsUpError());
                    return false;
                }
            }

            bool success = PostDaoV5.Instance.SetThreadsUp(forumID, threadIDs);
            if (success)
            {
                ThreadCachePool.ClearForumCache(forumID, null);

                ThreadCollectionV5 threads = new ThreadCollectionV5();


                ThreadCollectionV5 tempThreads = GetThreads(threadIDs);

                foreach (BasicThread thread in tempThreads)
                {
                    if (thread.ForumID != forumID)
                        continue;

                    threads.Add(thread);
                }

                if (sendNotify)
                {
                    SendManagerOperationNotify(operatorUser, threads, ModeratorCenterAction.UpThread, actionReason);
                }

                if (isUseProp)
                    CreateThreadManageLogs(forumID, threadIDs, ModeratorCenterAction.UpThreadByUseProp, null);
                else
                    WriteThreadOperateLog(operatorUser, threads, ModeratorCenterAction.UpThread, actionReason);

            }

            return success;
        }

        #endregion

        #endregion


        #region 设置精华状态 SetThreadsValuedStatus

        /// <summary>
        /// 加入/解除精华
        /// </summary>
        /// <param name="threadIdentities"></param>
        /// <returns></returns>
        public bool SetThreadsValuedStatus(AuthUser operatorUser, int forumID, IEnumerable<int> threadIDs, bool isValued, bool ignorePermission, bool createManageLog, bool sendNotify,string actionReason)
        {
            ThreadCollectionV5 threads = GetThreads(threadIDs);

            ManageForumPermissionSetNode permissionSet = GetForumPermissionSet(forumID);
            if (!ignorePermission)
            {

                foreach (BasicThread thread in threads)
                {
                    if (isValued)
                    {
                        if (false == permissionSet.HasPermissionForSomeone(operatorUser , ManageForumPermissionSetNode.ActionWithTarget.SetThreadsValued))
                        {
                            ThrowError(new NoPermissionSetThreadsValuedError());
                            return false;
                        }
                    }
                    else
                    {
                        if (false == permissionSet.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsValued, thread.PostUserID))
                        {
                            ThrowError(new NoPermissionSetThreadsValuedError());
                            return false;
                        }
                    }
                }

            }

            Dictionary<int, int> results = new Dictionary<int, int>();
            ThreadCollectionV5 tempThreads = new ThreadCollectionV5();
            foreach (BasicThread thread in threads)
            {
                if (thread.ForumID != forumID)
                    continue;

                tempThreads.Add(thread);
                if (thread.IsValued == isValued)
                    continue;
                if (results.ContainsKey(thread.PostUserID))
                    results[thread.PostUserID] += 1;
                else
                    results.Add(thread.PostUserID, 1);
            }

            if (tempThreads.Count == 0)
            {
                ThrowError(new SetThreadValuedNotSellectThreadError());
                return false;
            }

            bool success = ForumPointAction.Instance.UpdateUsersPoint(results, ForumPointType.SetThreadsValued, isValued, forumID, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    if (PostDaoV5.Instance.SetThreadsValued(forumID, threadIDs, isValued))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            });

            if (success)
            {
                ThreadCachePool.UpdateCache(threadIDs);

                ThreadCachePool.ClearAllForumThreadsCache(ThreadCachePool.ThreadOrderType.AllForumTopValuedThreads);
                ThreadCachePool.ClearForumThreadsCache(forumID, ThreadCachePool.ThreadOrderType.ForumTopValuedThreads);

                ModeratorCenterAction action;
                if (isValued)
                    action = ModeratorCenterAction.SetThreadElite;
                else
                    action = ModeratorCenterAction.CancelValued;

                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, tempThreads,action, actionReason);
                if (createManageLog)
                    WriteThreadOperateLog(operatorUser, tempThreads, action, actionReason);

            }

            return success;
        }

        #endregion



        #region 还原主题 RestoreThread / RestoreThreads

        public bool RestoreThreads(AuthUser operatorUser, IEnumerable<int> threadIDs, bool ignorePermission, bool sendNotify, bool createThreadManageLog, string actionReason)
        {
            if (ValidateUtil.HasItems<int>(threadIDs) == false)
            {
                ThrowError<NotSellectRestoreThreadIDsError>(new NotSellectRestoreThreadIDsError());
                return false;
            }

            ThreadCollectionV5 threads = GetThreads(threadIDs);

            if (threads.Count == 0)
                return true;

            Dictionary<int, List<int>> normalThreads = new Dictionary<int, List<int>>();
            foreach (BasicThread thread in threads)
            {
                if (normalThreads.ContainsKey(thread.ForumID))
                {
                    normalThreads[thread.ForumID].Add(thread.ThreadID);
                }
                else
                {
                    List<int> tempThreadIDs = new List<int>();
                    tempThreadIDs.Add(thread.ThreadID);
                    normalThreads.Add(thread.ForumID, tempThreadIDs);
                }
            }

            bool success = false;

            foreach (KeyValuePair<int, List<int>> pair in normalThreads)
            {
                success = RestoreThreads(operatorUser, pair.Key, pair.Value, ignorePermission, sendNotify, createThreadManageLog, actionReason);
                if (success == false)
                    return false;
            }

            return success;
        }

        /// <summary>
        /// 还原任何人的帖子
        /// </summary>
        /// <param name="threadIdentities"></param>
        /// <returns></returns>
        public bool RestoreThreads(AuthUser operatorUser, int forumID, IEnumerable<int> threadIDs, bool ignorePermission, bool sendNotify, bool createLog, string actionReason)
        {

            int userID = UserBO.Instance.GetCurrentUserID();
            ThreadCollectionV5 threads = GetThreads(threadIDs);
            if (!ignorePermission)
            {

                ManageForumPermissionSetNode managerPermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID);
                ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID);

                foreach (BasicThread thread in threads)
                {
                    if (false == managerPermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled, thread.PostUserID))
                    {
                        if (thread.PostUserID == operatorUser.UserID)
                        {
                            if (!permission.Can(operatorUser, ForumPermissionSetNode.Action.RecycleAndRestoreOwnThreads))
                            {
                                ThrowError<NoPermissionRestoreOwnThreadError>(new NoPermissionRestoreOwnThreadError(operatorUser.UserID));
                                return false;
                            }
                        }
                        else
                        {
                            ThrowError<NoPermissionRestoreAnyThreadError>(new NoPermissionRestoreAnyThreadError(operatorUser.UserID));
                            return false;
                        }
                    }
                }
            }

            Dictionary<int, int> results = new Dictionary<int, int>();

            ThreadCollectionV5 resultThreads = new ThreadCollectionV5();
            foreach (BasicThread thread in threads)
            {
                if (thread.ForumID == forumID)
                {
                    resultThreads.Add(thread);
                    if (results.ContainsKey(thread.PostUserID))
                        results[thread.PostUserID] += 1;
                    else
                        results.Add(thread.PostUserID, 1);
                }
            }

            ForumPointType pointType = ForumPointType.DeleteAnyThreads;
            if (results.Count == 1 && results.ContainsKey(userID))
                pointType = ForumPointType.DeleteOwnThreads;

            bool success = ForumPointAction.Instance.UpdateUsersPoint(results, pointType, false, forumID, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    if (PostDaoV5.Instance.SetThreadsStatus(forumID, null, threadIDs, ThreadStatus.Normal, null))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            });

            if (success)
            {

                RemoveTopicSearchCount();

                List<int> userIDs = GetPostUserIDsFormThreads(threadIDs);

                ForumBO.Instance.ClearAllCache();
                ForumBO.Instance.ClearForumThreadCatalogsCache();

                ThreadCachePool.ClearAllCache();

                UserBO.Instance.RemoveUsersCache(userIDs);

                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, resultThreads, ModeratorCenterAction.RevertThread, actionReason);

                if (createLog)
                    WriteThreadOperateLog(operatorUser, resultThreads, ModeratorCenterAction.RevertThread, actionReason);

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region 审核主题 回复

        public bool ApproveThreads(AuthUser operatorUser, IEnumerable<int> threadIDs, bool ignorePermission, bool sendNotify, bool createThreadManageLog, string actionReason)
        {
            if (ValidateUtil.HasItems<int>(threadIDs) == false)
            {
                ThrowError<NoSellectedApproveThreadIDError>(new NoSellectedApproveThreadIDError());
                return false;
            }

            ThreadCollectionV5 threads = GetThreads(threadIDs);

            if (threads.Count == 0)
                return true;

            Dictionary<int, List<int>> normalThreads = new Dictionary<int, List<int>>();
            foreach (BasicThread thread in threads)
            {
                if (normalThreads.ContainsKey(thread.ForumID))
                {
                    normalThreads[thread.ForumID].Add(thread.ThreadID);
                }
                else
                {
                    List<int> tempThreadIDs = new List<int>();
                    tempThreadIDs.Add(thread.ThreadID);
                    normalThreads.Add(thread.ForumID, tempThreadIDs);
                }
            }

            bool success = false;

            foreach (KeyValuePair<int, List<int>> pair in normalThreads)
            {
                success = ApproveThreads(operatorUser, pair.Key, pair.Value, ignorePermission, sendNotify, createThreadManageLog, actionReason);
                if (success == false)
                    return false;
            }

            return success;
        }

        
        


        public bool ApproveThreads(AuthUser operatorUser, int forumID, IEnumerable<int> threadIDs, bool ignorePermission, bool sendNotify, bool createLog, string actionReason)
        {
            ThreadCollectionV5 threads = GetThreads(threadIDs);
            if (!ignorePermission)
            {
                ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID);

                foreach (BasicThread thread in threads)
                {
                    if (false == managePermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads, thread.PostUserID))
                    {
                        ThrowError<NoPermissionApproveThreadError>(new NoPermissionApproveThreadError());
                        return false;
                    }
                }
            }

            Dictionary<int, int> results = new Dictionary<int, int>();
            ThreadCollectionV5 resultThreads = new ThreadCollectionV5();
            foreach (BasicThread thread in threads)
            {
                if (thread.ThreadStatus != ThreadStatus.UnApproved)
                    continue;

                if (thread.ForumID == forumID)
                {
                    resultThreads.Add(thread);
                    if (results.ContainsKey(thread.PostUserID))
                        results[thread.PostUserID] += 1;
                    else
                        results.Add(thread.PostUserID, 1);
                }
            }

            if (resultThreads.Count == 0)
            {
                ThrowError<NoSellectedApproveThreadIDError>(new NoSellectedApproveThreadIDError());
                return false;
            }

            bool success = ForumPointAction.Instance.UpdateUsersPoint(results, ForumPointType.CreateThread, true, forumID, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    if (PostDaoV5.Instance.SetThreadsStatus(forumID, null, threadIDs, ThreadStatus.Normal, null))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            });


            if (success)
            {
                RemoveTopicSearchCount();

                List<int> tempUserIDs = GetPostUserIDsFormThreads(threadIDs);
                UserBO.Instance.RemoveUsersCache(tempUserIDs);

                ForumBO.Instance.ClearAllCache();
                ForumBO.Instance.ClearForumThreadCatalogsCache();

                ThreadCachePool.ClearAllCache();

                UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts, ActiveUserType.DayPosts, ActiveUserType.MonthPosts });

                if (sendNotify)
                    SendManagerOperationNotify(operatorUser, resultThreads, ModeratorCenterAction.CheckThread, actionReason);

                if (createLog)
                    WriteThreadOperateLog(operatorUser, resultThreads, ModeratorCenterAction.CheckThread, actionReason);

                return true;
            }
            else
                return false;

        }



        /// <summary>
        /// 删除主题的所有未审核回复
        /// </summary>
        /// <param name="threadIDs"></param>
        /// <returns></returns>
        public bool DeleteUnApprovedPosts(AuthUser operatorUser, IEnumerable<int> threadIDs, bool sendNotify, bool createManageLog, string actionReason)
        {
            foreach (int threadID in threadIDs)
            {
                BasicThread thread;
                PostCollectionV5 posts;
                int total;
                GetUnapprovedPostThread(threadID, null, 1, int.MaxValue, out thread, out posts, out total);

                Forum forum = ForumBO.Instance.GetForum(thread.ForumID);
                if (false == AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID).Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts, thread.PostUserID))
                {
                    ThrowError<NoPermissionDeletePostError>(new NoPermissionDeletePostError());
                    return false;
                }
                List<int> postIDs = new List<int>();
                foreach (PostV5 post in posts)
                {
                    if (!post.IsApproved)
                        postIDs.Add(post.PostID);
                }


                bool success = PostDaoV5.Instance.DeletePosts(thread.ForumID, threadID, postIDs, operatorUser.UserID, true);
                if (success == false)
                    return false;

                RemovePostSearchCount();

                Logs.LogManager.LogOperation(
                       new Topic_DeletePostByIDs(operatorUser.UserID, forum.ForumID, operatorUser.Name, operatorUser.LastVisitIP, postIDs)
                   );

                if (sendNotify)
                {
                    SendManagerOperationNotify(operatorUser, null, threadID, posts, ModeratorCenterAction.DeletePost, actionReason);
                }

                if (createManageLog)
                    WriteThreadOperateLog(operatorUser, posts, ModeratorCenterAction.DeletePost, actionReason);
            }
            return true;
        }


        public bool ApprovePostsByThreadIDs(AuthUser operatorUser, IEnumerable<int> threadIDs, bool sendNotify, bool createManageLog, string actionReason)
        {
            foreach (int threadID in threadIDs)
            {
                bool success = ApprovePosts(operatorUser, threadID, sendNotify, createManageLog, actionReason);
                if (!success)
                    return false;
            }
            return true;
        }
        public bool ApprovePosts(AuthUser operatorUser, int threadID, bool sendNotify, bool createManageLog, string actionReason)
        {
            BasicThread thread;
            PostCollectionV5 posts;
            int total;
            GetUnapprovedPostThread(threadID, null, 1, int.MaxValue, out thread, out posts, out total);
            List<int> postIDs = new List<int>();
            if (posts.Count > 0)
            {
                for (int i = 0; i < posts.Count; i++)
                {
                    postIDs.Add(posts[i].PostID);
                }
            }
            else
                return true;
            return ApprovePosts(operatorUser, postIDs, sendNotify, createManageLog, actionReason);
        }
        public bool ApprovePosts(AuthUser operatorUser, IEnumerable<int> postIDs, bool sendNotify, bool createManageLog, string actionReason)
        {

            PostCollectionV5 posts = GetPosts(postIDs);

            List<int> resultIDs = new List<int>();
            PostCollectionV5 resultPosts = new PostCollectionV5();

            List<int> forumIDs = new List<int>();
            Dictionary<int, PostCollectionV5> threadPosts = new Dictionary<int, PostCollectionV5>();
            foreach (PostV5 post in posts)
            {
                if (!post.IsApproved)
                {
                    resultIDs.Add(post.PostID);
                    resultPosts.Add(post);

                    if (threadPosts.ContainsKey(post.ThreadID))
                    {
                        threadPosts[post.ThreadID].Add(post);
                    }
                    else
                    {
                        PostCollectionV5 ps = new PostCollectionV5();
                        ps.Add(post);
                        threadPosts.Add(post.ThreadID, ps);
                    }
                }
                if (forumIDs.Contains(post.ForumID) == false)
                {
                    if (AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(post.ForumID).Can(operatorUser, ManageForumPermissionSetNode.Action.ApprovePosts) == false)
                    {
                        forumIDs.Add(post.ForumID);
                        ThrowError<NoPermissionApprovePostError>(new NoPermissionApprovePostError(post.ForumID));
                        return false;
                    }
                }
            }

            if (resultIDs.Count == 0)
            {
                ThrowError<NotSellectApprovePostIDsError>(new NotSellectApprovePostIDsError());
                return false;
            }

            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    List<int> threadIDs = PostDaoV5.Instance.ApprovePosts(resultIDs);

                    if (threadIDs == null || threadIDs.Count < 1)
                    {
                        context.RollbackTransaction();
                        ThrowError<CustomError>(new CustomError("CustomError", "未知错误"));
                        return false;
                    }
                    else
                    {
                        //-------------------------

                        ThreadCollectionV5 threads = GetThreads(threadIDs);

                        Dictionary<int, Dictionary<int, int>> temp = new Dictionary<int, Dictionary<int, int>>();
                        foreach (BasicThread thread in threads)
                        {
                            Dictionary<int, int> userPoints;// = new Dictionary<int, int>();
                            if (temp.ContainsKey(thread.ForumID))
                                userPoints = temp[thread.ForumID];
                            else
                            {
                                userPoints = new Dictionary<int, int>();
                                temp.Add(thread.ForumID, userPoints);
                            }
                            foreach (PostV5 post in resultPosts)
                            {
                                if (post.ThreadID == thread.ThreadID)
                                {
                                    if (userPoints.ContainsKey(post.UserID))
                                        userPoints[post.UserID] += 1;
                                    else
                                        userPoints.Add(post.UserID, 1);
                                }
                            }
                        }
                        foreach (KeyValuePair<int, Dictionary<int, int>> pair in temp)
                        {
                            ForumPointAction.Instance.UpdateUsersPoint(pair.Value, ForumPointType.ReplyThread, true, pair.Key, null);
                        }
                        context.CommitTransaction();
                        //-----------------------
                        ForumBO.Instance.ClearAllCache();

                        //这里不只移除主题的回复是因为还要更新主题的回复数
                        ThreadCachePool.UpdateCache(threadIDs);

                        ThreadCachePool.ClearAllForumThreadsCache(ThreadCachePool.ThreadOrderType.AllForumTopWeekHotThreads);
                        foreach (int forumID in temp.Keys)
                        {
                            ThreadCachePool.ClearForumThreadsCache(forumID, ThreadCachePool.ThreadOrderType.ForumTopWeekHotThreads);
                        }

                        UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts, ActiveUserType.DayPosts, ActiveUserType.MonthPosts });

                        UpdatePostsUsersProfile(posts);

                        RemovePostSearchCount();

                        try
                        {
                            Dictionary<int, List<int>> logForumids = new Dictionary<int, List<int>>();
                            foreach (PostV5 post in resultPosts)
                            {
                                if (logForumids.ContainsKey(post.ForumID))
                                {
                                    if (logForumids[post.ForumID].Contains(post.PostID) == false)
                                    {
                                        logForumids[post.ForumID].Add(post.PostID);
                                    }
                                }
                                else
                                {
                                    List<int> tempPostIDs = new List<int>();
                                    tempPostIDs.Add(post.PostID);
                                    logForumids.Add(post.ForumID, tempPostIDs);
                                }
                            }

                            foreach (KeyValuePair<int, List<int>> pair in logForumids)
                            {
                                Logs.LogManager.LogOperation(
                                      new Topic_ApprovePost(operatorUser.UserID, pair.Key, operatorUser.Name, operatorUser.LastVisitIP, pair.Value)
                                  );
                            }

                            if (sendNotify)
                            {
                                foreach (KeyValuePair<int, PostCollectionV5> pair in threadPosts)
                                {
                                    SendManagerOperationNotify(operatorUser, null, pair.Key, pair.Value, ModeratorCenterAction.ApprovePost, actionReason);
                                }
                            }

                            if (createManageLog)
                                WriteThreadOperateLog(operatorUser, posts, ModeratorCenterAction.ApprovePost, actionReason);
                        }
                        catch { }

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }
            }
        }
        
        
        private void UpdatePostsUsersProfile(PostCollectionV5 posts)
        {
            List<int> userIDs = new List<int>();
            foreach (PostV5 post in posts)
            {
                if (!userIDs.Contains(post.UserID))
                    userIDs.Add(post.UserID);
            }
            UserBO.Instance.RemoveUsersCache(userIDs);
        }



        #endregion


        /// <summary>
        /// 屏蔽帖子
        /// </summary>
        /// <param name="postIDs"></param>
        /// <param name="IsShielded"></param>
        /// <returns></returns>
        public bool UpdatePostsShielded(AuthUser operatorUser, int forumID, IEnumerable<int> postIDs, bool IsShielded, bool ignorePermission, bool createManageLog, bool sendNotify, string actionReason)
        {
            PostCollectionV5 posts = GetPosts(postIDs);
            if (posts.Count == 0)
            {
                ThrowError(new UpdatePostsShieldedNotSellectPostError());
                return false;
            }
            if (!ignorePermission)
            {
                ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID);
                foreach (PostV5 post in posts)
                {
                    if (false == managePermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetPostShield, post.UserID, post.LastEditorID))
                    {
                        return false;
                    }
                }
            }

            Dictionary<int, int> results = new Dictionary<int, int>();
            List<int> threadIDs = new List<int>();
            Dictionary<int, PostCollectionV5> threadPostIDs = new Dictionary<int, PostCollectionV5>();
            List<int> tempPostIDs = new List<int>();
            PostCollectionV5 tempPosts = new PostCollectionV5();
            foreach (PostV5 post in posts)
            {
                if (post.ForumID != forumID)
                    continue;
                if (post.IsShielded == IsShielded)
                    continue;

                tempPostIDs.Add(post.PostID);
                tempPosts.Add(post);
                if (threadIDs.Contains(post.ThreadID) == false)
                    threadIDs.Add(post.ThreadID);
                if (results.ContainsKey(post.UserID))
                    results[post.UserID] += 1;
                else
                    results.Add(post.UserID, 1);

                if (threadPostIDs.ContainsKey(post.ThreadID))
                {
                    threadPostIDs[post.ThreadID].Add(post);
                }
                else
                {
                    PostCollectionV5 ps = new PostCollectionV5();
                    ps.Add(post);
                    threadPostIDs.Add(post.ThreadID, ps);
                }
            }

            if (tempPostIDs.Count == 0)
                return true;


            bool success = ForumPointAction.Instance.UpdateUsersPoint(results, ForumPointType.ShieldPost, IsShielded, forumID, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    if (PostDaoV5.Instance.UpdatePostsShielded(tempPostIDs, IsShielded))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            });


            if (success)
            {
                ThreadCachePool.ClearThreadPostCache(threadIDs);

                ModeratorCenterAction action;
                if (IsShielded == false)
                    action = ModeratorCenterAction.RescindShieldPost;
                else
                    action = ModeratorCenterAction.ShieldPost;

                if (sendNotify)
                {
                    foreach (KeyValuePair<int, PostCollectionV5> pair in threadPostIDs)
                    {
                        SendManagerOperationNotify(operatorUser, null, pair.Key, pair.Value, action, actionReason);
                    }
                }
                if (createManageLog)
                    WriteThreadOperateLog(operatorUser, tempPosts, action, actionReason);
            }

            return success;
        }

        #endregion


        private TopicFilter ProcessTopicFilter(TopicFilter filter)
        {
            if (filter == null)
                return null;

            TopicFilter topicFilter = (TopicFilter)filter.Clone();

            User user = null;
            if (!string.IsNullOrEmpty(topicFilter.Username))
            {
                user = UserBO.Instance.GetUser(topicFilter.Username);
                if (user == null)
                    return null;
            }
            if (user != null)
            {
                if (topicFilter.UserID != null && topicFilter.UserID.Value != user.UserID)
                    return null;
                else
                    topicFilter.UserID = user.UserID;
            }
            return topicFilter;
        }

        private PostFilter ProcessPostFilter(PostFilter filter)
        {
            if (filter == null)
                return null;

            PostFilter postFilter = (PostFilter)filter.Clone();

            User user = null;
            if (!string.IsNullOrEmpty(postFilter.Username))
            {
                user = UserBO.Instance.GetUser(postFilter.Username);
                if (user == null)
                    return null;
            }
            if (user != null)
            {
                if (postFilter.UserID != null && postFilter.UserID.Value != user.UserID)
                    return null;
                else
                    postFilter.UserID = user.UserID;
            }
            return postFilter;
        }

        public BasicThread GetThread(int threadID)
        {
            if (threadID < 1)
                return null;

            string key = string.Format("thread/{0}", threadID);

            BasicThread thread;
            if (PageCacheUtil.TryGetValue<BasicThread>(key, out thread) == false)
            {
                thread = ThreadCachePool.GetThread(threadID);

                if (thread == null)
                {
                    thread = PostDaoV5.Instance.GetThread(threadID);
                    ThreadCachePool.SetThreadCache(thread);
                }

                if (thread != null)
                    PageCacheUtil.Set(key, thread);
            }

            return thread;
        }

        public void SetThreadPageCache(BasicThread thread)
        {
            if (thread == null)
                return;

            string key = string.Format("thread/{0}", thread.ThreadID);
            PageCacheUtil.Set(key, thread);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="getExtendedInfo">是否获取 附件 评分等信息</param>
        /// <returns></returns>
        public PostV5 GetPost(int postID, bool getExtendedInfo)
        {
            string key = string.Format("post/{0}/{1}", postID, getExtendedInfo);

            PostV5 post;
            if (PageCacheUtil.TryGetValue<PostV5>(key, out post) == false)
            {
                post = PostDaoV5.Instance.GetPost(postID, getExtendedInfo);
                if (post != null)
                    PageCacheUtil.Set(key, post);
            }
            return post;
        }


        /// <summary>
        /// 获取主题内容
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="getExtendedInfo">是否获取 附件 评分等信息</param>
        /// <returns></returns>
        public PostV5 GetThreadFirstPost(int threadID, bool getExtendedInfo)
        {
            string key = string.Format("threadFirstPost/{0}/{1}", threadID, getExtendedInfo);

            PostV5 post;
            if (PageCacheUtil.TryGetValue<PostV5>(key, out post) == false)
            {
                post = PostDaoV5.Instance.GetThreadFirstPost(threadID, getExtendedInfo);
                if (post != null)
                    PageCacheUtil.Set(key, post);
            }
            return post;
        }

        public Dictionary<int, PostV5> GetThreadContents(IEnumerable<int> threadIDs)
        {
            if (ValidateUtil.HasItems<int>(threadIDs) == false)
                return new Dictionary<int, PostV5>();
            return PostDaoV5.Instance.GetThreadContents(threadIDs);
        }

        /// <summary>
        /// 根据ForumID和页索引取得主题列表
        /// </summary>
        /// <param name="forumID">论坛ID</param>
        /// <param name="sortType">为null则按SortOrder排序</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <returns></returns>
        public ThreadCollectionV5 GetThreads(int forumID, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, int pageNumber, int pageSize, bool isSpider, out int totalCount)
        {
            if (isSpider)
            {
                if (beginDate != null)
                    beginDate = null;
                if (endDate != null)
                    endDate = null;

                if (sortType != null && sortType.Value != AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID).DefaultThreadSortField)
                    sortType = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID).DefaultThreadSortField;
            }



            ThreadCollectionV5 threads = null;

            ThreadCollectionV5 globalThreads = GetGlobalThreads();
            ThreadCollectionV5 stickThreads = GetStickThreads(forumID);

            

            int globalCount = globalThreads.Count;
            int stickCount = stickThreads.Count;
            int offSet = 0;
            totalCount = -1;


            Forum forum = ForumBO.Instance.GetForum(forumID);

            int pageIndex = pageNumber - 1;
            int pageLowerBound = pageIndex * pageSize;
            int pageUpperBound = pageLowerBound + pageSize;

            //先从置顶里取
            if (pageLowerBound < stickCount + globalCount)
            {
                threads = new ThreadCollectionV5();
                ThreadCollectionV5 temp = new ThreadCollectionV5();
                temp.AddRange(globalThreads);
                temp.AddRange(stickThreads);

                for (int i = pageLowerBound; i < temp.Count; i++)
                {
                    threads.Add(temp[i]);

                    //取够了该页 就返回
                    if (i == pageUpperBound - 1)
                    {
                        if (beginDate == null && endDate == null)
                            totalCount = globalCount + forum.TotalThreads - GetGlobalStickCount(forum.ForumID);
                        else
                            totalCount = GetThreadCount(forumID, beginDate, endDate, false) + stickCount;

                        threads = ProcessMovedThreads(threads);
                        return threads;
                    }
                }

                pageLowerBound = 0;
                pageUpperBound = pageSize - threads.Count;
            }
            else
            {
                pageLowerBound = pageLowerBound - stickCount - globalCount;
                pageUpperBound = pageLowerBound + pageSize;
            }


            //只有这样才 才可能使用缓存
            if (beginDate == null && endDate == null && isDesc && sortType == null)
            {
                int cacheCount = ThreadCachePool.GetTotalCacheCount(ThreadCachePool.ThreadOrderType.ForumTopThreadsBySortOrder);

                if (pageUpperBound <= cacheCount)
                {
                    if (threads == null)
                        threads = new ThreadCollectionV5();

                    ThreadCollectionV5 topThreads = GetTopThreads(forumID);
                    int topCount = topThreads.Count;

                    //totalCount = topCount + globalCount + stickCount;
                    totalCount = globalCount + forum.TotalThreads - GetGlobalStickCount(forumID);

                    for (int i = pageLowerBound; i < topCount; i++)
                    {
                        threads.Add(topThreads[i]);
                        //取够了该页 就返回
                        if (i == pageUpperBound - 1)
                        {
                            threads = ProcessMovedThreads(threads);
                            return threads;
                        }
                    }

                    //还到这里 说明该页还没取够 但是数据库里已经没有了 所以直接返回
                    threads = ProcessMovedThreads(threads);
                    return threads;
                }
            }

            int tempTotal = 0;//只包含 一般主题的总数
            if (beginDate == null && endDate == null)
            {
                tempTotal = forum.TotalThreads;
                foreach (BasicThread thread in stickThreads)
                {
                    if (thread.ForumID == forumID)
                        tempTotal--;
                }
                tempTotal = tempTotal - GetGlobalStickCount(forumID);
            }
            else
                tempTotal = -1;

            offSet = (stickCount + globalCount) % pageSize;
            

            ThreadCollectionV5 tempThreads = GetForumThreads(forumID, sortType, beginDate, endDate, isDesc, offSet, false, pageNumber, pageSize, ref tempTotal);

            totalCount = tempTotal + stickCount + globalCount;

            if (threads == null)
                return tempThreads;
            else
            {
                if (tempThreads.Count > 0)
                    threads.AddRange(tempThreads);
            }
            return threads;
        }

        public ThreadCollectionV5 GetThreads( IEnumerable<int> threadIds)
        {
            if (!ValidateUtil.HasItems<int>(threadIds))
                return new ThreadCollectionV5();

            ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreads(threadIds);

            return threads;
        }

        public ThreadCollectionV5 GetThreads(int forumID, ThreadType threadType, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, bool returnTotalThreads, out int totalThreads)
        {
            ThreadCollectionV5 result = new ThreadCollectionV5();
            ThreadCollectionV5 gloablThreads = GetGlobalThreads();
            foreach (BasicThread thread in gloablThreads)
            {
                if (thread.ThreadType != threadType || thread.ForumID!= forumID)
                    continue;
                if (beginDate != null && thread.CreateDate < beginDate.Value)
                    continue;
                if (endDate != null && thread.CreateDate > endDate.Value)
                    continue;

                result.Add(thread);
            }
            ThreadCollectionV5 stickThreads = GetStickThreads(forumID);

            foreach (BasicThread thread in stickThreads)
            {
                if (thread.ThreadType != threadType || thread.ForumID != forumID)
                    continue;
                if (beginDate != null && thread.CreateDate < beginDate.Value)
                    continue;
                if (endDate != null && thread.CreateDate > endDate.Value)
                    continue;

                result.Add(thread);
            }

            ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreads(forumID, threadType, pageNumber, pageSize, sortType, beginDate, endDate, isDesc, returnTotalThreads, result.Count, out totalThreads);

            threads = ProcessSortFieldResult(threads, sortType);

            result.AddRange(threads);

            totalThreads = totalThreads + result.Count;

            threads = ProcessMovedThreads(threads);
            return result;
        }

        public ThreadCollectionV5 GetThreads(int forumID, int threadCatalogID, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, out int totalThreads)
        {
            totalThreads = -1;
            if (beginDate == null && endDate == null)
            {
                if (threadCatalogID == 0)
                {
                    Forum forum = ForumBO.Instance.GetForum(forumID);
                    int count = 0;
                    foreach (ThreadCatalog tempThreadCatalog in ForumBO.Instance.GetThreadCatalogs(forumID))
                    {
                        count += tempThreadCatalog.ThreadCount;
                    }
                    totalThreads = forum.TotalThreads - count;
                }
                else
                {
                    ThreadCatalog threadCatalog = ForumBO.Instance.GetForumThreadCatalog(forumID, threadCatalogID);
                    if (threadCatalog == null)
                    {
                        totalThreads = 0;
                        return new ThreadCollectionV5();
                    }
                    else
                        totalThreads = threadCatalog.ThreadCount;
                }
            }

            ThreadCollectionV5 tempThreads = new ThreadCollectionV5();
            ThreadCollectionV5 gloablThreads = GetGlobalThreads();
            foreach (BasicThread thread in gloablThreads)
            {
                if (thread.ForumID == forumID && thread.ThreadCatalogID == threadCatalogID)
                {
                    if (beginDate != null && thread.CreateDate < beginDate.Value)
                        continue;

                    if (endDate != null && thread.CreateDate > endDate.Value)
                        continue;

                    tempThreads.Add(thread);
                }
            }
            ThreadCollectionV5 stickThreads = GetStickThreads(forumID);
            foreach (BasicThread thread in stickThreads)
            {
                if (thread.ForumID == forumID && thread.ThreadCatalogID == threadCatalogID)
                {
                    if (beginDate != null && thread.CreateDate < beginDate.Value)
                        continue;

                    if (endDate != null && thread.CreateDate > endDate.Value)
                        continue;

                    tempThreads.Add(thread);
                }
            }

            if (totalThreads != -1)
            {
                totalThreads = totalThreads - tempThreads.Count;
            }

            ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreadsByThreadCatalogID(forumID, threadCatalogID, pageNumber, pageSize, sortType, beginDate, endDate, isDesc, tempThreads.Count, ref totalThreads);
            threads = ProcessSortFieldResult(threads, sortType);

            tempThreads.AddRange(threads);

            totalThreads = totalThreads + tempThreads.Count;

            tempThreads = ProcessMovedThreads(tempThreads);

            return tempThreads;
        }

        private int GetGlobalStickCount(int forumID)
        {
            int count = 0;
            ThreadCollectionV5 threads = GetGlobalThreads();
            foreach (BasicThread thread in threads)
            {
                if (thread.ForumID == forumID)
                {
                    count++;
                }
            }

            return count;
        }

        public ThreadCollectionV5 GetThreads(AuthUser operatorUser, TopicFilter filter, int pageNumber)
        {
            int totalCount = -1;

            TopicFilter topicFilter = ProcessTopicFilter(filter);

            if (topicFilter == null)
                return new ThreadCollectionV5();

            if (pageNumber < 1)
                pageNumber = 1;

            string cacheKey = string.Format(cacheKey_List_Topic_Search_Count, topicFilter.ToString());

            bool haveTotalCountCache = false;

            if (CacheUtil.TryGetValue<int>(cacheKey, out totalCount))
            {
                haveTotalCountCache = true;
            }
            else
                totalCount = -1;


            Guid[] excludeRoleIDs = null;

            if (filter.ForumID != null && filter.ForumID.Value != 0)
            {
                Forum forum = ForumBO.Instance.GetForum(filter.ForumID.Value);
                if (forum == null)
                {
                    return new ThreadCollectionV5();
                }

                ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID);

                if (false == managePermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads))
                {
                    return new ThreadCollectionV5();
                }
                excludeRoleIDs = managePermission.GetNoPermissionTargetRoleIds(operatorUser, PermissionTargetType.Content);
            }
            else
            {
                if (operatorUser.IsOwner == false)
                {
                    //TODO:
                    //您不是创始人  没有权限搜索所有版块的主题
                    return new ThreadCollectionV5();
                }
            }

            ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreads(pageNumber, topicFilter, excludeRoleIDs, ref totalCount);


            if (!haveTotalCountCache)
            {
                CacheUtil.Set<int>(cacheKey, totalCount, CacheTime.Normal, CacheExpiresType.Sliding);
            }

            threads.TotalRecords = totalCount;

            return threads;
        }


        private ThreadCollectionV5 ProcessMovedThreads(ThreadCollectionV5 threads)
        {
            ThreadCollectionV5 movedThreads = null;
            bool hasGet = false;
            Dictionary<int, int> redirectIDs = new Dictionary<int, int>();
            foreach (BasicThread thread in threads)
            {
                if (thread.ThreadType == ThreadType.Move)
                {
                    if (movedThreads == null && hasGet == false)
                    {
                        movedThreads = new ThreadCollectionV5(ThreadCachePool.GetAllForumThreads(ThreadCachePool.ThreadOrderType.MovedThreads));
                        hasGet = true;
                    }
                    if (movedThreads != null)
                    {
                        BasicThread targetThread = movedThreads.GetValue(thread.RedirectThreadID);
                        if (targetThread == null)
                        {
                            targetThread = ThreadCachePool.GetThread(thread.RedirectThreadID);
                            if (targetThread != null)
                            {
                                ThreadCachePool.AddAllForumThread(ThreadCachePool.ThreadOrderType.MovedThreads, targetThread);
                                movedThreads.Add(targetThread);
                            }
                        }
                        if (targetThread != null)
                        {
                            thread.TotalViews = targetThread.TotalViews;
                            thread.TotalReplies = targetThread.TotalReplies;
                            thread.PostedCount = targetThread.PostedCount;
                            continue;
                        }
                    }

                    redirectIDs.Add(thread.RedirectThreadID, thread.ThreadID);
                }
            }
            if (redirectIDs.Count > 0)
            {
                ThreadCollectionV5 tempThreads = PostDaoV5.Instance.GetThreads(redirectIDs.Keys);
                foreach (BasicThread thread in tempThreads)
                {
                    BasicThread temp = threads.GetValue(redirectIDs[thread.ThreadID]);
                    if (temp != null)
                    {
                        temp.TotalReplies = thread.TotalReplies;
                        temp.TotalViews = thread.TotalViews;
                        temp.PostedCount = thread.PostedCount;
                    }
                }
                movedThreads.AddRange(tempThreads);
                ThreadCachePool.SetAllForumThreads(ThreadCachePool.ThreadOrderType.MovedThreads, movedThreads);
            }
            return threads;
        }


        public bool DeleteSearchTopics(AuthUser operatorUser, TopicFilter filter, bool updatePoint, int deleteTopCount, out int deletedCount)
        {
            deletedCount = 0;

            TopicFilter topicFilter = ProcessTopicFilter(filter);
            if (topicFilter == null)
                return true;

            Guid[] excludeRoleIDs = null;

            Forum forum = null;

            if (filter.ForumID != null && filter.ForumID.Value != 0)
            {
                forum = ForumBO.Instance.GetForum(filter.ForumID.Value);
                if (forum == null)
                    return false;

                if (forum.ManagePermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads) == false)
                {
                    //
                    return false;
                }

                excludeRoleIDs = forum.ManagePermission.GetNoPermissionTargetRoleIds(operatorUser, PermissionTargetType.Content);
            }
            else
            {
                if (operatorUser.IsOwner == false)
                {
                    //TODO:
                    //您不是创始人  没有权限删除所有版块的主题
                    return false;
                }
            }

            DeleteResult deleteResult = null;

            List<int> threadIDs = null;
            if (updatePoint)
            {
                int tempDeleteCount = 0;
                bool success = ForumPointAction.Instance.UpdateUsersPoints(delegate(PointActionManager.TryUpdateUserPointState state, out PointResultCollection<ForumPointType> pointResults) //(ForumPointType.DeleteAnyThreads, delegate(PointActionManager.TryUpdateUserPointState state, out Dictionary<int, int> userIDs)
                {
                    if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                    {
                        deleteResult = PostDaoV5.Instance.DeleteSearchTopics(topicFilter, excludeRoleIDs, true, deleteTopCount, out tempDeleteCount, out threadIDs);

                        pointResults = new PointResultCollection<ForumPointType>();
                        foreach (DeleteResultItem item in deleteResult)
                        {
                            if (item.NodeID > 0)//不是回收站 或者 不是未审核的帖子 才更新积分
                                pointResults.Add(item.UserID, ForumPointType.DeleteAnyThreads, item.Count, item.NodeID);
                        }
                        return true;
                    }
                    else
                    {
                        pointResults = null;
                        return false;
                    }
                });

                deletedCount = tempDeleteCount;

                if (!success)
                    return false;
            }
            else
            {
                deleteResult = PostDaoV5.Instance.DeleteSearchTopics(topicFilter, excludeRoleIDs, true, deleteTopCount, out deletedCount, out threadIDs);
            }
            foreach (DeleteResultItem item in deleteResult)
            {
                UserBO.Instance.RemoveUserCache(item.UserID);
            }

            ForumBO.Instance.ClearAllCache();
            ForumBO.Instance.ClearForumThreadCatalogsCache();
            ThreadCachePool.ClearAllCache();

            UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts, ActiveUserType.DayPosts, ActiveUserType.MonthPosts });

            RemoveTopicSearchCount();

            return true;
        }

       


        public PostAuthorExtendInfo GetPostAuthorExtendInfo(AuthUser operatorUser, int userID)
        {

            PostAuthorExtendInfo userinfo = PostDaoV5.Instance.GetPostAuthorExtendInfo(userID, operatorUser.IsMyFriend(userID) ? DataAccessLevel.Friend : DataAccessLevel.Normal);
            return userinfo;
        }

        public bool ApproveSearchTopics(AuthUser operatorUser, TopicFilter filter, int approveTopCount, out int approvedCount)
        {
            approvedCount = 0;

            TopicFilter topicFilter = ProcessTopicFilter(filter);
            if (topicFilter == null)
                return true;

            Guid[] excludeRoleIDs = null;

            Forum forum = null;

            if (filter.ForumID != null && filter.ForumID.Value != 0)
            {
                forum = ForumBO.Instance.GetForum(filter.ForumID.Value);
                if (forum == null)
                    return false;

                if (forum.ManagePermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads) == false)
                {
                    //
                    return false;
                }

                excludeRoleIDs = forum.ManagePermission.GetNoPermissionTargetRoleIds(operatorUser, PermissionTargetType.Content);
            }
            else
            {
                if (operatorUser.IsOwner == false)
                {
                    //TODO:
                    //您不是创始人  没有权限删除所有版块的主题
                    return false;
                }
            }

            int count = -1;
            filter.PageSize = approveTopCount;
            ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreads(1, filter, excludeRoleIDs, ref count);

            approvedCount = threads.Count;

            List<int> threadIDs = new List<int>();
            foreach(BasicThread thread in threads)
            {
                threadIDs.Add(thread.ThreadID);
            }

            if (threadIDs.Count > 0)
                return ApproveThreads(operatorUser, threadIDs, false, true, true, string.Empty);
            else
                return true;
        }

        public bool RestoreSearchTopics(AuthUser operatorUser, TopicFilter filter, int restoreTopCount, out int restoredCount)
        {
            restoredCount = 0;

            TopicFilter topicFilter = ProcessTopicFilter(filter);
            if (topicFilter == null)
                return true;

            Guid[] excludeRoleIDs = null;

            Forum forum = null;

            if (filter.ForumID != null && filter.ForumID.Value != 0)
            {
                forum = ForumBO.Instance.GetForum(filter.ForumID.Value);
                if (forum == null)
                    return false;

                if (forum.ManagePermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled) == false)
                {
                    //
                    return false;
                }

                excludeRoleIDs = forum.ManagePermission.GetNoPermissionTargetRoleIds(operatorUser, PermissionTargetType.Content);
            }
            else
            {
                if (operatorUser.IsOwner == false)
                {
                    //TODO:
                    //您不是创始人  没有权限删除所有版块的主题
                    return false;
                }
            }

            int count = -1;
            filter.PageSize = restoreTopCount;
            ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreads(1, filter, excludeRoleIDs, ref count);

            restoredCount = threads.Count;

            List<int> threadIDs = new List<int>();
            foreach (BasicThread thread in threads)
            {
                threadIDs.Add(thread.ThreadID);
            }

            if (threadIDs.Count > 0)
                return RestoreThreads(operatorUser, threadIDs, false, true, true, string.Empty);
            else
                return true;
        }


        public void RemoveTopicSearchCount()
        {
            CacheUtil.RemoveBySearch("Topic/List/Search/Count/");
        }


        public PostCollectionV5 GetPosts(AuthUser operatorUser, PostFilter filter, int pageNumber, out int totalCount)
        {
            totalCount = -1;

            PostFilter postFilter = ProcessPostFilter(filter);
            if (postFilter == null)
                return new PostCollectionV5();

            if (pageNumber < 1)
                pageNumber = 1;

            string cacheKey = string.Format(cacheKey_List_Post_Search_Count, postFilter.ToString());
            bool haveTotalCountCache = false;
            if (CacheUtil.TryGetValue<int>(cacheKey, out totalCount))
            {
                haveTotalCountCache = true;
            }
            else
                totalCount = -1;


            Guid[] excludeRoleIDs = null;

            if (filter.ForumID != null && filter.ForumID.Value != 0)
            {
                Forum forum = ForumBO.Instance.GetForum(filter.ForumID.Value);
                if (forum == null)
                {
                    return new PostCollectionV5();
                }

                ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID);


                if (false == managePermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads))
                {
                    return new PostCollectionV5();
                }
                excludeRoleIDs = managePermission.GetNoPermissionTargetRoleIds(operatorUser, PermissionTargetType.Content);
            }
            else
            {
                if (operatorUser.IsOwner == false)
                {
                    //TODO:
                    //您不是创始人  没有权限搜索所有版块的主题
                    return new PostCollectionV5();
                }
            }

            PostCollectionV5 posts = PostDaoV5.Instance.GetPosts(pageNumber, postFilter, excludeRoleIDs, ref totalCount);


            if (!haveTotalCountCache)
            {
                CacheUtil.Set<int>(cacheKey, totalCount, CacheTime.Normal, CacheExpiresType.Sliding);
            }

            return posts;
        }


        public bool DeleteSearchPosts(AuthUser operatorUser, PostFilter filter, bool updatePoint, int deleteTopCount, out int deletedCount)
        {
            deletedCount = 0;

            PostFilter postFilter = ProcessPostFilter(filter);
            if (postFilter == null)
                return true;

            Guid[] excludeRoleIDs = null;

            Forum forum = null;

            if (filter.ForumID != null && filter.ForumID.Value != 0)
            {
                ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(filter.ForumID.Value);

                if (forum == null)
                    return false;

                if (managePermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts) == false)
                {
                    //
                    return false;
                }

                excludeRoleIDs = managePermission.GetNoPermissionTargetRoleIds(operatorUser, PermissionTargetType.Content);
            }
            else
            {
                if (operatorUser.IsOwner == false)
                {
                    //TODO:
                    //您不是创始人  没有权限删除所有版块的主题
                    return false;
                }
            }

            DeleteResult deleteResult = null;

            List<int> threadIDs = null;
            if (updatePoint)
            {
                int tempDeleteCount = 0;
                bool success = ForumPointAction.Instance.UpdateUsersPoints(delegate(PointActionManager.TryUpdateUserPointState state, out PointResultCollection<ForumPointType> pointResults) //(ForumPointType.DeleteAnyThreads, delegate(PointActionManager.TryUpdateUserPointState state, out Dictionary<int, int> userIDs)
                {
                    if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                    {
                        deleteResult = PostDaoV5.Instance.DeleteSearchPosts(postFilter, excludeRoleIDs, true, deleteTopCount, out tempDeleteCount, out threadIDs);

                        pointResults = new PointResultCollection<ForumPointType>();
                        foreach (DeleteResultItem item in deleteResult)
                        {
                            if (item.NodeID > 0)//不是回收站 或者 不是未审核的帖子 才更新积分
                                pointResults.Add(item.UserID, ForumPointType.DeleteAnyPosts, item.Count, item.NodeID);
                        }
                        return true;
                    }
                    else
                    {
                        pointResults = null;
                        return false;
                    }
                });

                deletedCount = tempDeleteCount;

                if (!success)
                    return false;
            }
            else
            {
                deleteResult = PostDaoV5.Instance.DeleteSearchPosts(postFilter, excludeRoleIDs, true, deleteTopCount, out deletedCount, out threadIDs);
            }
            foreach (DeleteResultItem item in deleteResult)
            {
                UserBO.Instance.RemoveUserCache(item.UserID);
            }

            ForumBO.Instance.ClearAllCache();
            ThreadCachePool.ClearAllCache();

            RemovePostSearchCount();

            UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts, ActiveUserType.DayPosts, ActiveUserType.MonthPosts });


            return true;
        }

        public bool ApproveSearchPosts(AuthUser operatorUser, PostFilter filter, int approvedTopCount, out int approvedCount)
        {
            approvedCount = 0;

            PostFilter postFilter = ProcessPostFilter(filter);
            if (postFilter == null)
                return true;

            Guid[] excludeRoleIDs = null;

            Forum forum = null;

            if (filter.ForumID != null && filter.ForumID.Value != 0)
            {
                ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(filter.ForumID.Value);

                if (forum == null)
                    return false;

                if (managePermission.Can(operatorUser, ManageForumPermissionSetNode.Action.ApprovePosts) == false)
                {
                    //
                    return false;
                }
            }
            else
            {
                if (operatorUser.IsOwner == false)
                {
                    //TODO:
                    //您不是创始人  没有权限删除所有版块的主题
                    return false;
                }
            }

            int count = -1;
            filter.PageSize = approvedTopCount;
            PostCollectionV5 posts = PostDaoV5.Instance.GetPosts(1, filter, null, ref count);

            approvedCount = posts.Count;

            List<int> postIDs = new List<int>();
            foreach(PostV5 post in posts)
            {
                postIDs.Add(post.PostID);
            }

            if (postIDs.Count > 0)
                return ApprovePosts(operatorUser, postIDs, true, true, string.Empty);
            else
                return true;
        }

        public void RemovePostSearchCount()
        {
            CacheUtil.RemoveBySearch("Post/List/Search/Count/");
        }

        #region 关键字=======================


        public void ProcessKeyword(PostCollectionV5 posts, ProcessKeywordMode mode)
        {
            if (posts == null || posts.Count == 0)
                return;

            KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            bool needProcess = false;

            //更新关键字模式，只在必要的情况下才取恢复信息并处理
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                needProcess = keyword.NeedUpdate2<PostV5>(posts);
            }
            //填充原始内容模式，始终都要取恢复信息，但不处理
            else
            {
                needProcess = true;
            }

            if (needProcess)
            {
                List<int> postIDs = new List<int>();
                PostCollectionV5 tempPosts = new PostCollectionV5();
                foreach (PostV5 post in posts)
                {
                    postIDs.Add(post.PostID);
                    tempPosts.Add(post);
                }

                Revertable2Collection<PostV5> postsWithReverter = PostDaoV5.Instance.GetPostsWithReverters(postIDs);

                if (keyword.Update2(postsWithReverter))
                {
                    PostDaoV5.Instance.UpdatePostKeywords(postsWithReverter);
                }

                //将新数据填充到旧的列表
                postsWithReverter.FillTo(tempPosts);

                posts.Clear();

                foreach (PostV5 post in tempPosts)
                {
                    posts.Add(post);
                }
            }
        }

        //public void ProcessKeyword(Thread thread, ProcessKeywordMode mode)
        //{
        //    //更新关键字模式，只在必要的情况下才取恢复信息并处理
        //    if (mode == ProcessKeywordMode.TryUpdateKeyword)
        //    {
        //        if (AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.NeedUpdate(thread) == false)
        //            return;
        //    }

        //    List<Thread> threads = new List<Thread>();
        //    threads.Add(thread);

        //    ProcessKeyword(threads, mode);

        //    thread.OriginalSubject = threads[0].OriginalSubject;
        //    thread.KeywordVersion = threads[0].KeywordVersion;
        //    thread.TempSubject = threads[0].TempSubject;
        //}

        public void ProcessKeyword(ThreadCollectionV5 threads, ProcessKeywordMode mode)
        {
            if (threads == null || threads.Count == 0)
                return;

            KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            bool needProcess = false;

            //更新关键字模式，只在必要的情况下才取恢复信息并处理
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                needProcess = keyword.NeedUpdate<BasicThread>(threads);
            }
            //填充原始内容模式，始终都要取恢复信息，但不处理
            else
            {
                needProcess = true;
            }

            if (needProcess)
            {
                List<int> threadIDs = new List<int>();
                ThreadCollectionV5 tempThreads = new ThreadCollectionV5();
                foreach (BasicThread thread in threads)
                {
                    threadIDs.Add(thread.ThreadID);
                    tempThreads.Add(thread);
                }

                RevertableCollection<BasicThread> threadsWithReverter = PostDaoV5.Instance.GetThreadWithReverters(threadIDs);

                if (keyword.Update(threadsWithReverter))
                {
                    PostDaoV5.Instance.UpdateThreadKeywords(threadsWithReverter);
                }

                //将新数据填充到旧的列表
                threadsWithReverter.FillTo(tempThreads);

                threads.Clear();
                foreach (BasicThread thread in tempThreads)
                {
                    threads.Add(thread);
                }
            }
        }


        public void ProcessKeyword(BasicThread thread, ProcessKeywordMode mode)
        {
            //更新关键字模式，如果这个文章并不需要处理，直接退出
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                if (AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.NeedUpdate(thread) == false)
                    return;
            }

            ThreadCollectionV5 threads = new ThreadCollectionV5();

            threads.Add(thread);

            ProcessKeyword(threads, mode);
        }


        public void ProcessKeyword(PostV5 post, ProcessKeywordMode mode)
        {
            //更新关键字模式，如果这个文章并不需要处理，直接退出
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                if (AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.NeedUpdate2(post) == false)
                    return;
            }

            PostCollectionV5 posts = new PostCollectionV5();

            posts.Add(post);

            ProcessKeyword(posts, mode);
        }
        /*
        public void ProcessKeyword(PostCollectionV5 posts, ProcessKeywordMode mode)
        {
            if (posts.Count == 0)
                return;

            KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            bool needProcess = false;

            //更新关键字模式，只在必要的情况下才取恢复信息并处理
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                needProcess = keyword.NeedUpdate2<Post>(posts);
            }
            //填充原始内容模式，始终都要取恢复信息，但不处理
            else
            {
                needProcess = true;
            }

            if (needProcess)
            {
                Revertable2Collection<Post> postsWithReverter = PostDao.Instance.GetPostsWithReverters(posts.GetKeys());

                if (postsWithReverter != null)
                {
                    if (keyword.Update2(postsWithReverter))
                    {
                        PostDao.Instance.UpdatePostKeywords(postsWithReverter);
                    }

                    //将新数据填充到旧的列表
                    postsWithReverter.FillTo(posts);
                }
            }
        }
        */

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="includeStick">是否包括一般的置顶主题</param>
        /// <returns></returns>
        private int GetThreadCount(int forumID, DateTime? beginDate, DateTime? endDate, bool includeStick)
        {
            return PostDaoV5.Instance.GetThreadCount(forumID, beginDate, endDate, includeStick);
        }

        ///// <summary>
        ///// 简洁版 专用
        ///// </summary>
        ///// <param name="threadID"></param>
        ///// <param name="pageNumber"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="updateView"></param>
        ///// <param name="ignorePermission"></param>
        ///// <param name="thread"></param>
        ///// <param name="replies"></param>
        ///// <returns></returns>
        //public bool GetThreadWithReplies2(int threadID, int pageNumber, int pageSize, bool updateView, bool cacheReplyUsers, out BasicThread thread, out PostCollectionV5 replies)
        //{
        //    replies = null;

        //    bool getThread = true;

        //    bool isInListCache;

        //    thread = ThreadCachePool.GetThread(threadID, out isInListCache);
        //    if (thread != null)
        //    {
        //        //thread.FirstPost = null;
        //        getThread = false;
        //    }


        //    int totalReplies;
        //    if (thread == null)
        //        totalReplies = -1;
        //    else
        //        totalReplies = thread.TotalReplies + 1;

        //    BasicThread tempThread;

        //    PostDaoV5.Instance.GetThreadWithReplies(threadID, totalReplies, getThread, pageNumber, pageSize, out tempThread, out replies);

        //    if (tempThread != null)
        //    {
        //        ThreadCachePool.SetThreadCache(tempThread);
        //        thread = tempThread;
        //    }

        //    int lastPage = 0;
        //    if (replies != null)
        //    {
        //        //这种情况发生在 输入的页码大于总页数 
        //        //这时候要使其显示最后一页的数据
        //        if (replies.Count < 1)
        //        {
        //            BasicThread nouseThread;
        //            int totalPostCount = thread.TotalReplies + 1;
        //            lastPage = totalPostCount % pageSize == 0 ? (totalPostCount / pageSize) : (totalPostCount / pageSize + 1);
        //            if (lastPage < 1)
        //                lastPage = 1;
        //            PostDaoV5.Instance.GetThreadWithReplies(threadID, thread.TotalReplies + 1, false, lastPage - 1, pageSize, out nouseThread, out replies);
        //        }
        //    }

        //    if (thread == null)
        //    {
        //        ThrowError<ThreadNotExistsError>(new ThreadNotExistsError());
        //        return false;
        //    }

        //    User my = User.Current;

        //    //==================================================================
        //    List<int> userIdentities = new List<int>();

        //    if (lastPage == 0)
        //        lastPage = (thread.TotalReplies + 1) % pageSize == 0 ? (thread.TotalReplies + 1) / pageSize : ((thread.TotalReplies + 1) / pageSize + 1);
        //    for (int index = 0; index < replies.Count; index++)
        //    {
        //        if (pageNumber > lastPage)
        //            replies[index].PostIndex = ((lastPage - 1) * pageSize) + index;
        //        else
        //            replies[index].PostIndex = ((pageNumber-1) * pageSize) + index;

        //        if (cacheReplyUsers)
        //        {
        //            int userID = replies[index].UserID;
        //            if (!userIdentities.Contains(userID))
        //                userIdentities.Add(userID);
        //        }
        //    }

        //    if (userIdentities.Count > 0)
        //        UserBO.Instance.GetUsers(userIdentities, GetUserOption.WithAll);

        //    //    /*
        //    //    Dictionary<int, UserProfile> userProfiles = UserManager.GetUserProfiles(userIdentities, true);
        //    //    */

        //    //    //foreach(Post post in threadReplies)
        //    //    //{
        //    //    //    foreach (UserProfile userProfile in userProfiles.Values)
        //    //    //    {
        //    //    //        if (post.UserID == userProfile.UserID)
        //    //    //        {
        //    //    //            post.UserProfile = userProfile;
        //    //    //            break;
        //    //    //        }
        //    //    //    }
        //    //    //}

        //    if (updateView)
        //    {
        //        thread.TotalViews++;

        //        UpdateThreadViewsJob.AddView(threadID);
        //    }

        //    return true;

        //}

        /// <summary>
        /// 默认排序的缓存
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public ThreadCollectionV5 GetTopThreads(int forumID)
        {
            ThreadCollectionV5 topThreads = ThreadCachePool.GetForumThreads(forumID, ThreadCachePool.ThreadOrderType.ForumTopThreadsBySortOrder);
            if (topThreads == null)
            {
                int globalCount = 0;
                ThreadCollectionV5 globalThreads = GetGlobalThreads();
                foreach (BasicThread thread in globalThreads)
                {
                    if (thread.ForumID == forumID)
                        globalCount++;
                }

                int stickCount = 0;
                ThreadCollectionV5 stickThreads = GetStickThreads(forumID);
                foreach (BasicThread thread in stickThreads)
                {
                    if (thread.ForumID == forumID)
                        stickCount++;
                }

                int total = ForumBO.Instance.GetForum(forumID, false).TotalThreads - stickCount - globalCount;

                topThreads = GetForumThreads(forumID, null, null, null, true, 0, false, 1, ThreadCachePool.GetTotalCacheCount(ThreadCachePool.ThreadOrderType.ForumTopThreadsBySortOrder), ref total);

                ThreadCachePool.SetForumThreadsCache(forumID, ThreadCachePool.ThreadOrderType.ForumTopThreadsBySortOrder, topThreads);
            }

            return topThreads;
        }

        /// <summary>
        /// 置顶 主题  包含了其它版块置顶到这个版块的主题 ，
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public ThreadCollectionV5 GetStickThreads(int forumID)
        {
            ThreadCollectionV5 threads = ThreadCachePool.GetForumThreads(forumID, ThreadCachePool.ThreadOrderType.ForumStickThreads);
            if (threads == null)
            {
                threads = PostDaoV5.Instance.GetStickThreads(forumID);
                ThreadCachePool.SetForumThreadsCache(forumID, ThreadCachePool.ThreadOrderType.ForumStickThreads, threads);
            }

            return threads;
        }

      
        /// <summary>
        ///  此方法不设置缓存 
        /// </summary>
        /// <param name="forumIDs"></param>
        /// <returns></returns>
        private ThreadCollectionV5 GetStickThreads(IEnumerable<int> forumIDs)
        {
            ThreadCollectionV5 threads = new ThreadCollectionV5();
            List<int> tempForumIDs = new List<int>();
            foreach (int forumID in forumIDs)
            {
                ThreadCollectionV5 tempThreads = ThreadCachePool.GetForumThreads(forumID, ThreadCachePool.ThreadOrderType.ForumStickThreads);
                if (tempThreads == null)
                {
                    tempForumIDs.Add(forumID);
                }
                else
                    threads.AddRange(tempThreads);
            }

            if (tempForumIDs.Count > 0)
            {
                ThreadCollectionV5 tempThreads = PostDaoV5.Instance.GetStickThreads(forumIDs);
                threads.AddRange(tempThreads);
            }

            return threads;
        }

        private const string cacheKey_AllStickThreadInForums = "Topic/AllStickThreadInForums";
        public StickThreadCollection GetAllStickThreadInForums()
        {
            StickThreadCollection sticks;
            if (CacheUtil.TryGetValue(cacheKey_AllStickThreadInForums, out sticks) == false)
            {
                sticks = PostDaoV5.Instance.GetAllStickThreadForums();
                CacheUtil.Set<StickThreadCollection>(cacheKey_AllStickThreadInForums, sticks);
            }
            return sticks;
        }
        private void ClearAllStickThreadInForumsCache()
        {
            CacheUtil.Remove(cacheKey_AllStickThreadInForums);
        }

        public ThreadCollectionV5 GetGlobalThreads()
        {
            ThreadCollectionV5 threads = ThreadCachePool.GetAllForumThreads(ThreadCachePool.ThreadOrderType.GlobalStickThreads);
            if (threads == null)
            {
                threads = PostDaoV5.Instance.GetGlobalThreads();
                ThreadCachePool.SetAllForumThreads(ThreadCachePool.ThreadOrderType.GlobalStickThreads, threads);
            }
            return threads;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="sortType">为null则按sortOrder排序</param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isDesc"></param>
        /// <param name="offSet"></param>
        /// <param name="includeStick"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public ThreadCollectionV5 GetForumThreads(int forumID, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, int offSet, bool includeStick, int pageNumber, int pageSize, ref int totalCount)
        {
            ThreadCollectionV5 stickThreads = null;
            if (includeStick)
            {
                stickThreads = GetGloablAndForumStickThreads(forumID, beginDate, endDate);
                offSet = offSet + stickThreads.Count;
                if (totalCount > 0)
                    totalCount = totalCount - stickThreads.Count;
            }
            ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreads(forumID, sortType, beginDate, endDate, isDesc, offSet, false, pageNumber, pageSize, ref totalCount);

            threads = ProcessSortFieldResult(threads, sortType);

            if (includeStick)
            {
                stickThreads.AddRange(threads);
                return stickThreads;
            }

            return threads;
        }

        /// <summary>
        /// 包括总置顶
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        private ThreadCollectionV5 GetGloablAndForumStickThreads(int forumID, DateTime? beginDate, DateTime? endDate)
        {
            ThreadCollectionV5 threads = new ThreadCollectionV5();

            ThreadCollectionV5 stickThreads = GetStickThreads(forumID);

            ThreadCollectionV5 gloablThreads = GetGlobalThreads();

            foreach (BasicThread thread in gloablThreads)
            {
                if (thread.ForumID == forumID)
                {
                    if ((beginDate == null || thread.CreateDate > beginDate.Value) && (endDate == null || thread.CreateDate < endDate))
                        threads.Add(thread);
                }
            }

            foreach (BasicThread thread in stickThreads)
            {
                if (thread.ForumID == forumID)
                {
                    if ((beginDate == null || thread.CreateDate > beginDate.Value) && (endDate == null || thread.CreateDate < endDate))
                        threads.Add(thread);
                }
            }

            return threads;
        }

        public ThreadCollectionV5 GetValuedThreads(int forumID, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, bool returnTotalThreads, out int totalThreads)
        {
            List<int> forumIDs = new List<int>();
            forumIDs.Add(forumID);

            return GetValuedThreads(forumIDs, pageNumber, pageSize, sortType, beginDate, endDate, isDesc, returnTotalThreads, out totalThreads);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forumIDs">为null则取所有版块</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortType"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isDesc"></param>
        /// <param name="returnTotalThreads"></param>
        /// <param name="totalThreads"></param>
        /// <returns></returns>
        public ThreadCollectionV5 GetValuedThreads(IEnumerable<int> forumIDs, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, bool returnTotalThreads, out int totalThreads)
        {
            ThreadCollectionV5 tempThreads = new ThreadCollectionV5();
            ThreadCollectionV5 gloablThreads = GetGlobalThreads();

            foreach (BasicThread thread in gloablThreads)
            {
                if (thread.IsValued == false)
                    continue;

                if (beginDate != null && thread.CreateDate < beginDate.Value)
                    continue;

                if (endDate != null && thread.CreateDate < endDate.Value)
                    continue;

                if (forumIDs != null)
                {
                    foreach (int forumID in forumIDs)
                    {
                        if (thread.ForumID == forumID)
                        {
                            tempThreads.Add(thread);
                            break;
                        }
                    }
                }
                else
                    tempThreads.Add(thread);
            }

            IEnumerable<int> tempForumIDs = forumIDs;
            if (forumIDs == null)
            {
                tempForumIDs = ForumBO.Instance.GetAllForums().GetKeys();
            }
            foreach (int forumID in tempForumIDs)
            {
                ThreadCollectionV5 stickThreads = GetStickThreads(forumID);
                foreach (BasicThread thread in stickThreads)
                {
                    if (thread.IsValued == false)
                        continue;

                    if (beginDate != null && thread.CreateDate < beginDate.Value)
                        continue;

                    if (endDate != null && thread.CreateDate < endDate.Value)
                        continue;

                    tempThreads.Add(thread);
                }
            }

            ThreadCollectionV5 threads = PostDaoV5.Instance.GetValuedThreads(forumIDs, pageNumber, pageSize, sortType, beginDate, endDate, isDesc, returnTotalThreads, false, tempThreads.Count, out totalThreads);
            threads = ProcessSortFieldResult(threads, sortType);

            totalThreads = totalThreads + tempThreads.Count;
            tempThreads.AddRange(threads);


            tempThreads = ProcessMovedThreads(tempThreads);
            return tempThreads;
        }

        /// <summary>
        /// 获取精华帖子
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="days">最近几天以内的</param>
        /// <returns></returns>
        public ThreadCollectionV5 GetValuedThreads(AuthUser operatorUser, int pageNumber, int pageSize, int days, ThreadSortField? sortType, bool isDesc)
        {
            List<int> forumIDs = ForumBO.Instance.GetForumIdsForVisit(operatorUser);

            if (forumIDs != null && forumIDs.Count == 0)
                return new ThreadCollectionV5();

            DateTime beginDate = DateTimeUtil.Now.AddDays(-days);

            int total;
            return GetValuedThreads(forumIDs, pageNumber, pageSize, sortType, beginDate, null, isDesc, false, out total);
            
        }

        public ThreadCollectionV5 GetUserQuestionThreads(int userID, int count, int exceptThreadID)
        {
            return PostDaoV5.Instance.GetUserQuestionThreads(userID, count, exceptThreadID);
        }
        /*
        public ThreadCollectionV5 GetHotThreads(AuthUser operatorUser, int? forumID, int pageNumber, int pageSize, int days, ThreadSortField? sortType, bool isDesc)
        {
            int reqireReplies = AllSettings.Current.BbsSettings.HotThreadRequireReplies;
            return GetHotThreads(operatorUser, forumID, pageNumber, pageSize, days, reqireReplies, sortType, isDesc);
        }
        */
        /*
        /// <summary>
        /// 获取版块热门帖子
        /// </summary>
        /// <param name="forumID">版块ID，null表示全部版块</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="days">几天之内 小于等于0表示全部</param>
        /// <param name="ignorePermission">为true时  包括没权限的主题</param>
        /// <returns></returns>
        private ThreadCollectionV5 GetHotThreads(AuthUser operatorUser, int? forumID, int pageNumber, int pageSize, int days, int reqireReplies, ThreadSortField? sortType, bool isDesc)
        {
            List<int> forumIDs = new List<int>();
            if (forumID != null)
            {
                if (ForumBO.Instance.IsVisitCheckPassed(operatorUser, forumID.Value) == false)
                    return new ThreadCollectionV5();
                else
                    forumIDs.Add(forumID.Value);
            }
            else
            {
                forumIDs = ForumBO.Instance.GetForumIdsForVisit(operatorUser);

                if (forumIDs != null && forumIDs.Count == 0)
                    return new ThreadCollectionV5();
            }

            DateTime beginDate = DateTimeUtil.Now.AddDays(-days);


            int total;


            ThreadCollectionV5 threads = PostDaoV5.Instance.GetHotThreads(forumIDs, reqireReplies, pageNumber, pageSize, sortType, beginDate, null, isDesc, false, out total);

            return ProcessSortFieldResult(threads, sortType);
        }
        */
        private ThreadCollectionV5 ProcessSortFieldResult(ThreadCollectionV5 threads, ThreadSortField? sortType)
        {
            if (sortType != null && sortType == ThreadSortField.Views) //把缓存中的点击数加上去 并重新排序
            {
                Dictionary<int, int> views = UpdateThreadViewsJob.GetThreadViews();

                bool hasAddViews = false;
                foreach (BasicThread thread in threads)
                {
                    if (views.ContainsKey(thread.ThreadID))
                    {
                        thread.TotalViews += views[thread.ThreadID];
                        hasAddViews = true;
                    }
                }
                if (hasAddViews == false)
                    return threads;

                ThreadCollectionV5 tempThreads = new ThreadCollectionV5();

                for (int i = 0; i < threads.Count; i++)
                {
                    int totalViews = threads[i].TotalViews;

                    int index = int.MaxValue;
                    for (int j = 0; j < tempThreads.Count; j++)
                    {
                        if (totalViews > tempThreads[j].TotalViews)
                            index = j - 1;
                    }

                    if (index == int.MaxValue)
                        tempThreads.Add(threads[i]);
                    else
                    {
                        if (index == -1)
                            index = 0;
                        tempThreads.Insert(index, threads[i]);
                    }
                }

                return tempThreads;
            }

            return threads;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="forumID">null为全站最新主题</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public ThreadCollectionV5 GetNewThreads(AuthUser operatorUser, int? forumID, int pageNumber, int pageSize, out int total)
        {
            if (forumID == null)
            {
                ThreadCollectionV5 threads = ProcessThreadsInCache(operatorUser, forumID, pageNumber, pageSize, ThreadCachePool.ThreadOrderType.AllForumTopThreadsByThreadID
                    , delegate(int tempTotal, List<int> tempForumIDs, int tempPageNumber, int tempPageSize)
                    {
                        return PostDaoV5.Instance.GetNewThreads(tempForumIDs, tempTotal, tempPageNumber, tempPageSize);
                    }, out total);

                return threads;
            }
            else
            {
                ThreadCollectionV5 threads = ProcessForumThreadsInCache(operatorUser, forumID.Value, pageNumber, pageSize, ThreadCachePool.ThreadOrderType.ForumTopThreadsByThreadID
                                   , delegate(int tempTotal, int tempForumID, int tempPageNumber, int tempPageSize)
                                   {
                                       return PostDaoV5.Instance.GetNewThreads(new int[] { tempForumID }, tempTotal, tempPageNumber, tempPageSize);
                                   }, out total);

                return threads;
            }
        }

        public ThreadCollectionV5 GetNewThreads(AuthUser operatorUser, int? forumID, int count)
        {
            int total;
            ThreadCollectionV5 threads;
            if (forumID == null)
            {
                threads = ProcessThreadsInCache(operatorUser, forumID, 1, count, ThreadCachePool.ThreadOrderType.AllForumTopThreadsByThreadID
                , delegate(int tempTotal, List<int> tempForumIDs, int tempPageNumber, int tempPageSize)
                {
                    int cacheCount = ThreadCachePool.GetTotalCacheCount(ThreadCachePool.ThreadOrderType.AllForumTopThreadsByThreadID);
                    if (cacheCount < count)
                        cacheCount = count;

                    ThreadCollectionV5 result = PostDaoV5.Instance.GetNewThreads(tempForumIDs, cacheCount);

                    result = ProcessStickThreads(result, null, cacheCount, ThreadSortField.CreateDate, delegate(BasicThread thread)
                    {
                        if (tempForumIDs == null)
                            return true;
                        if (tempForumIDs != null && tempForumIDs.Contains(thread.ForumID))
                            return true;
                        else
                            return false;
                    });

                    return result;

                }, out total);


            }
            else
            {
                threads = ProcessForumThreadsInCache(operatorUser, forumID.Value, 1, count, ThreadCachePool.ThreadOrderType.ForumTopThreadsByThreadID
               , delegate(int tempTotal, int tempForumID, int tempPageNumber, int tempPageSize)
               {
                   int cacheCount = ThreadCachePool.GetTotalCacheCount(ThreadCachePool.ThreadOrderType.ForumTopThreadsByThreadID);
                   if (cacheCount < count)
                       cacheCount = count;
                   ThreadCollectionV5 result = PostDaoV5.Instance.GetNewThreads(new int[] { tempForumID }, cacheCount);

                   result = ProcessStickThreads(result, tempForumID, cacheCount, ThreadSortField.CreateDate, delegate(BasicThread thread)
                   {
                       return true;
                   });

                   return result;
               }, out total);

            }

            if (threads.Count > count)
            {
                ThreadCollectionV5 tempThreads = new ThreadCollectionV5();
                int i = 0;
                foreach (BasicThread thread in threads)
                {
                    if (i >= count)
                        break;
                    tempThreads.Add(thread);
                    i++;
                }
                return tempThreads;
            }

            return threads;
        }

        public ThreadCollectionV5 GetNewRepliedThreads(AuthUser operatorUser, int? forumID, int count)
        {
            int total;
            if (forumID == null)
            {
                ThreadCollectionV5 threads = ProcessThreadsInCache(operatorUser, forumID, 1, count, ThreadCachePool.ThreadOrderType.AllForumTopThreadsByLastPostID
                    , delegate(int tempTotal, List<int> tempForumIDs, int tempPageNumber, int tempPageSize)
                    {
                        return GetThreads(ThreadSortField.LastReplyDate, tempPageSize, tempForumIDs);
                    }, out total);

                return threads;
            }
            else
            {
                ThreadCollectionV5 threads = ProcessForumThreadsInCache(operatorUser, forumID.Value, 1, count, ThreadCachePool.ThreadOrderType.ForumTopThreadsByLastPostID
                                   , delegate(int tempTotal, int tempForumID, int tempPageNumber, int tempPageSize)
                                   {
                                       return GetThreads(ThreadSortField.LastReplyDate, tempPageSize, new int[] { tempForumID });
                                   }, out total);

                return threads;
            }
        }

        public ThreadCollectionV5 GetValuedThreads(AuthUser operatorUser, int? forumID, int count)
        {
            int total;
            if (forumID == null)
            {
                ThreadCollectionV5 threads = ProcessThreadsInCache(operatorUser,forumID,1,count,ThreadCachePool.ThreadOrderType.AllForumTopValuedThreads
                                , delegate(int tempTotal, List<int> tempForumIDs, int tempPageNumber, int tempPageSize)
                                {
                                    ThreadCollectionV5 result = PostDaoV5.Instance.GetValuedThreads(tempForumIDs, tempPageNumber, tempPageSize, ThreadSortField.CreateDate, null, null, true, false, false, 0, out total);

                                    result = ProcessStickThreads(result, null, count, ThreadSortField.CreateDate, delegate(BasicThread thread) 
                                    {
                                        if (thread.IsValued && tempForumIDs == null)
                                            return true;
                                        if (thread.IsValued && (tempForumIDs!=null && tempForumIDs.Contains(thread.ForumID)))
                                            return true;
                                        else
                                            return false;
                                    });

                                    return result;

                                }, out total);

                return threads;
            }
            else
            {
                ThreadCollectionV5 threads = ProcessForumThreadsInCache(operatorUser, forumID.Value, 1, count, ThreadCachePool.ThreadOrderType.ForumTopThreadsByLastPostID
                                   , delegate(int tempTotal, int tempForumID, int tempPageNumber, int tempPageSize)
                                   {
                                       ThreadCollectionV5 result = PostDaoV5.Instance.GetValuedThreads(new int[] { tempForumID }, tempPageNumber, tempPageSize, ThreadSortField.CreateDate, null, null, true, false, false, 0, out total);

                                       result = ProcessStickThreads(result, tempForumID, count, ThreadSortField.CreateDate, delegate(BasicThread thread)
                                       {
                                           if (thread.IsValued)
                                               return true;
                                           else
                                               return false;
                                       });

                                       return result;
                                   }, out total);

                return threads;
            }
        }

        public ThreadCollectionV5 GetWeekHotThreads(AuthUser operatorUser, int? forumID, int count)
        {
            int total;
            if (forumID == null)
            {
                ThreadCollectionV5 threads = ProcessThreadsInCache(operatorUser, forumID, 1, count, ThreadCachePool.ThreadOrderType.AllForumTopWeekHotThreads
                    , delegate(int tempTotal, List<int> tempForumIDs, int tempPageNumber, int tempPageSize)
                    {
                        ThreadCollectionV5 result = PostDaoV5.Instance.GetHotThreads(tempForumIDs, 0, tempPageNumber, tempPageSize, ThreadSortField.Replies, DateTimeUtil.GetMonday().AddDays(-1), null, true, false, out total);
                        result = ProcessStickThreads(result, null, count, ThreadSortField.Replies, delegate(BasicThread thread)
                        {
                            if (thread.CreateDate > DateTimeUtil.GetMonday().Date && (tempForumIDs == null || (tempForumIDs!=null && tempForumIDs.Contains(thread.ForumID))))
                                return true;
                            else
                                return false;
                        });

                        return result;

                    }, out total);

                threads = ProcessMovedThreads(threads);
                return threads;
            }
            else
            {
                ThreadCollectionV5 threads = ProcessForumThreadsInCache(operatorUser, forumID.Value, 1, count, ThreadCachePool.ThreadOrderType.ForumTopWeekHotThreads
                                   , delegate(int tempTotal, int tempForumID, int tempPageNumber, int tempPageSize)
                                   {
                                       ThreadCollectionV5 result = PostDaoV5.Instance.GetHotThreads(new int[] { tempForumID }, 0, tempPageNumber, tempPageSize, ThreadSortField.Replies, DateTimeUtil.GetMonday().AddDays(-1), null, true, false, out total);

                                       result = ProcessStickThreads(result, tempForumID, count, ThreadSortField.Replies, delegate(BasicThread thread)
                                       {
                                           if (thread.CreateDate > DateTimeUtil.GetMonday().Date)
                                               return true;
                                           else
                                               return false;
                                       });

                                       return result;
                                   }, out total);

                threads = ProcessMovedThreads(threads);
                return threads;
            }
        }

        public ThreadCollectionV5 GetDayHotThreads(AuthUser operatorUser, int? forumID, int count)
        {
            int total;
            if (forumID == null)
            {
                ThreadCollectionV5 threads = ProcessThreadsInCache(operatorUser, forumID, 1, count, ThreadCachePool.ThreadOrderType.AllForumTopDayHotThreads
                    , delegate(int tempTotal, List<int> tempForumIDs, int tempPageNumber, int tempPageSize)
                    {
                        ThreadCollectionV5 result = PostDaoV5.Instance.GetHotThreads(tempForumIDs, 0, tempPageNumber, tempPageSize, ThreadSortField.Replies, DateTimeUtil.Now.AddDays(-1), null, true, false, out total);
                        result = ProcessStickThreads(result, null, count, ThreadSortField.Replies, delegate(BasicThread thread) 
                        {
                            if (thread.CreateDate > DateTimeUtil.Now.Date &&(tempForumIDs == null || (tempForumIDs!=null && tempForumIDs.Contains(thread.ForumID))))
                                return true;
                            else
                                return false;
                        });

                        return result;
                    }, out total);

                return threads;
            }
            else
            {
                ThreadCollectionV5 threads = ProcessForumThreadsInCache(operatorUser, forumID.Value, 1, count, ThreadCachePool.ThreadOrderType.ForumTopDayHotThreads
                                   , delegate(int tempTotal, int tempForumID, int tempPageNumber, int tempPageSize)
                                   {
                                       ThreadCollectionV5 result = PostDaoV5.Instance.GetHotThreads(new int[] { tempForumID }, 0, tempPageNumber, tempPageSize, ThreadSortField.Replies, DateTimeUtil.Now.AddDays(-1), null, true, false, out total);
                                       result = ProcessStickThreads(result, tempForumID, count, ThreadSortField.Replies, delegate(BasicThread thread)
                                       {
                                           if (thread.CreateDate > DateTimeUtil.Now.Date)
                                               return true;
                                           else
                                               return false;
                                       });

                                       return result;
                                   }, out total);

                return threads;
            }
        }


        public ThreadCollectionV5 GetWeekTopViewThreads(AuthUser operatorUser, int? forumID, int count)
        {
            int total;
            if (forumID == null)
            {
                ThreadCollectionV5 threads = ProcessThreadsInCache(operatorUser, forumID, 1, count, ThreadCachePool.ThreadOrderType.AllForumTopWeekViewThreads
                    , delegate(int tempTotal, List<int> tempForumIDs, int tempPageNumber, int tempPageSize)
                    {
                        return GetTopViewThreads(tempForumIDs, tempPageSize, DateTimeUtil.GetMonday().AddDays(-1), null);
                    }, out total);

                threads = ProcessMovedThreads(threads);
                return threads;
            }
            else
            {
                ThreadCollectionV5 threads = ProcessForumThreadsInCache(operatorUser, forumID.Value, 1, count, ThreadCachePool.ThreadOrderType.ForumTopWeekViewThreads
                                   , delegate(int tempTotal, int tempForumID, int tempPageNumber, int tempPageSize)
                                   {
                                       return GetTopViewThreads(new int[] { tempForumID }, tempPageSize, DateTimeUtil.GetMonday().AddDays(-1), null);
                                   }, out total);

                threads = ProcessMovedThreads(threads);
                return threads;
            }
        }

        public ThreadCollectionV5 GetDayTopViewThreads(AuthUser operatorUser, int? forumID, int count)
        {
            int total;
            if (forumID == null)
            {
                ThreadCollectionV5 threads = ProcessThreadsInCache(operatorUser, forumID, 1, count, ThreadCachePool.ThreadOrderType.AllForumTopDayViewThreads
                    , delegate(int tempTotal, List<int> tempForumIDs, int tempPageNumber, int tempPageSize)
                    {
                        return GetTopViewThreads(tempForumIDs, tempPageSize, DateTimeUtil.Now.AddDays(-1), null);
                    }, out total);

                threads = ProcessMovedThreads(threads);
                return threads;
            }
            else
            {
                ThreadCollectionV5 threads = ProcessForumThreadsInCache(operatorUser, forumID.Value, 1, count, ThreadCachePool.ThreadOrderType.ForumTopDayViewThreads
                                   , delegate(int tempTotal, int tempForumID, int tempPageNumber, int tempPageSize)
                                   {
                                       return GetTopViewThreads(new int[] { tempForumID }, tempPageSize, DateTimeUtil.Now.AddDays(-1), null);
                                   }, out total);

                threads = ProcessMovedThreads(threads);
                return threads;
            }
        }

        private ThreadCollectionV5 GetTopViewThreads(IEnumerable<int> forumIDs, int count, DateTime? beginDate, DateTime? endDate)
        {
            ThreadCollectionV5 threads = PostDaoV5.Instance.GetTopViewThreads(forumIDs, count, beginDate, endDate);
            ProcessSortFieldResult(threads, ThreadSortField.Views);

            int? forumID = null;

            if (forumIDs != null)
            {
                int i = 0;
                foreach (int id in forumIDs)
                {
                    forumID = id;
                    i++;
                    if (i > 1)
                        break;
                }
                if (i != 1)
                    forumID = null;
            }

            threads = ProcessStickThreads(threads, forumID, count, ThreadSortField.Views, delegate(BasicThread thread)
            {
                if(beginDate!=null && thread.CreateDate<beginDate.Value)
                    return false;
                if (endDate!=null && thread.CreateDate>endDate.Value)
                    return false;
                if (forumID == null && forumIDs != null)
                {
                    foreach (int id in forumIDs)
                    {
                        if (id == forumID.Value)
                            return true;
                    }
                    return false;
                }
                else
                    return true;
            });

            threads = ProcessMovedThreads(threads);
            return threads;
        }


        delegate ThreadCollectionV5 GetForumThreadsFromCache(int total, int forumID, int pageNumber, int pageSize);
        private ThreadCollectionV5 ProcessForumThreadsInCache(AuthUser operatorUser, int forumID, int pageNumber, int pageSize, ThreadCachePool.ThreadOrderType orderType, GetForumThreadsFromCache getThreads, out int total)
        {
            total = 0;
            if (ForumBO.Instance.IsVisitCheckPassed(operatorUser, forumID) == false)
            {
                return new ThreadCollectionV5();
            }

            Forum forum = ForumBO.Instance.GetForum(forumID);
            if (forum == null)
                return new ThreadCollectionV5();

            total = forum.TotalThreads;


            int cachedCount = ThreadCachePool.GetTotalCacheCount(orderType);

            ThreadCollectionV5 threads = ThreadCachePool.GetForumThreads(forumID, orderType);
            if (threads == null)
            {

                threads = getThreads(total, forumID, 1, cachedCount);

                ThreadCachePool.SetForumThreadsCache(forumID, orderType, threads);
            }

            if (pageNumber * pageSize < cachedCount || threads.Count < cachedCount)
            {
                ThreadCollectionV5 resultThreads = new ThreadCollectionV5();
                for (int i = (pageNumber - 1) * pageSize; i < pageNumber * pageSize; i++)
                {
                    if (i >= threads.Count)
                    {
                        break;
                    }

                    resultThreads.Add(threads[i]);
                }

                return resultThreads;
            }
            else
                return getThreads(total, forumID, pageNumber, pageSize);

        }

        delegate ThreadCollectionV5 GetAllForumThreadsFromCache(int total, List<int> forumIDs, int pageNumber, int pageSize);
        private ThreadCollectionV5 ProcessThreadsInCache(AuthUser operatorUser, int? forumID, int pageNumber, int pageSize, ThreadCachePool.ThreadOrderType orderType, GetAllForumThreadsFromCache getThreads, out int total)
        {
 
            total = 0;
            List<int> forumIDs;
            if (forumID == null)
            {
                forumIDs = ForumBO.Instance.GetForumIdsForVisit(operatorUser);
            }
            else
            {
                forumIDs = new List<int>();
                if (ForumBO.Instance.IsVisitCheckPassed(operatorUser, forumID.Value))
                {
                    forumIDs.Add(forumID.Value);
                }
                else
                    return new ThreadCollectionV5();
            }

            if (forumIDs != null && forumIDs.Count == 0)
                return new ThreadCollectionV5();

            ForumCollection forums;
            if (forumIDs != null)
            {
                forums = ForumBO.Instance.GetForums(delegate(Forum forum)
                {
                    if (forumIDs.Contains(forum.ForumID))
                        return true;
                    else
                        return false;
                });
            }
            else
            {
                forums = ForumBO.Instance.GetAllForums();
            }

            foreach (Forum forum in forums)
            {
                total += forum.TotalThreads;
            }


            int cachedCount = ThreadCachePool.GetTotalCacheCount(orderType);

            ThreadCollectionV5 threads = ThreadCachePool.GetAllForumThreads(orderType);
            if (threads == null)
            {
                int tempTotal = 0;
                foreach (Forum forum in ForumBO.Instance.GetAllForums())
                {
                    tempTotal += forum.TotalThreads;
                }

                threads = getThreads(tempTotal, null, 1, cachedCount);
                //threads = PostDaoV5.Instance.GetNewThreads(null, tempTotal, 1, cachedCount);

                ThreadCachePool.SetAllForumThreads(orderType, threads);
            }

            if (pageNumber * pageSize < cachedCount || threads.Count < cachedCount)
            {
                ThreadCollectionV5 tempThreads;
                if (forumIDs == null)
                {
                    tempThreads = threads;
                }
                else
                {
                    tempThreads = new ThreadCollectionV5();
                    foreach (BasicThread thread in threads)
                    {
                        if (forumIDs.Contains(thread.ForumID))
                            tempThreads.Add(thread);
                    }
                }

                int tempThreadCount = tempThreads.Count;
                if (pageNumber * pageSize < tempThreadCount || threads.Count < cachedCount)
                {
                    ThreadCollectionV5 resultThreads = new ThreadCollectionV5();
                    for (int i = (pageNumber - 1) * pageSize; i < pageNumber * pageSize; i++)
                    {
                        if (i >= tempThreadCount)
                        {
                            break;
                        }

                        resultThreads.Add(tempThreads[i]);
                    }

                    return resultThreads;
                }
            }

            return getThreads(total, forumIDs, pageNumber, pageSize);
            //return PostDaoV5.Instance.GetNewThreads(forumIDs, total, pageNumber, pageSize);
        }


        private ThreadCollectionV5 GetThreads(ThreadSortField sortType, int count, IEnumerable<int> forumIDs)
        {
            ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreads(sortType, count, forumIDs);
            ProcessSortFieldResult(threads, sortType);

             int? forumID = null;
             if (forumIDs != null)
             {
                 int i = 0;
                 foreach (int id in forumIDs)
                 {
                     forumID = id;
                     i++;
                     if (i > 1)
                         break;
                 }
                 if (i != 1)
                     forumID = null;
             }

            threads = ProcessStickThreads(threads, forumID, count, sortType, delegate (BasicThread thread)
            {
                if (forumID == null && forumIDs != null)
                {
                    foreach (int id in forumIDs)
                    {
                        if (thread.ForumID == id)
                            return true;
                    }
                    return false;
                }
                else
                    return true;
            });

            threads = ProcessMovedThreads(threads);
            return threads;
        }

        public void GetPollWithReplies(int threadID, int pageNumber, int pageSize, bool getExtendedInfo, bool updateView, out PollThreadV5 thread, out PostCollectionV5 posts, out ThreadType realThreadType)
        {
            thread = null;
            BasicThread tempThread;
            GetThreadWithReplies(threadID, ThreadType.Poll, pageNumber, pageSize, getExtendedInfo, updateView, true, true, out tempThread, out posts, out realThreadType);

            if (tempThread != null)
            {
                thread = (PollThreadV5)tempThread;
            }

        }
        public void GetQuestionWithReplies(int threadID, int pageNumber, int pageSize, bool getExtendedInfo, bool updateView, out QuestionThread thread, out PostCollectionV5 posts, out ThreadType realThreadType)
        {
            thread = null;

            BasicThread tempThread;
            GetThreadWithReplies(threadID, ThreadType.Question, pageNumber, pageSize, getExtendedInfo, updateView, true, true, out tempThread, out posts, out realThreadType);

            if (tempThread != null)
                thread = (QuestionThread)tempThread;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="postType">只允许 Polemize_Against,Polemize_Agree,Polemize_Neutral 其它当NULL处理 NULL所有回复</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="getExtendedInfo"></param>
        /// <param name="updateView"></param>
        /// <param name="checkThreadType"></param>
        /// <param name="thread"></param>
        /// <param name="posts"></param>
        /// <param name="realThreadType"></param>
        public void GetPolemizeWithReplies(int threadID, PostType? postType, int pageNumber, int pageSize, bool getExtendedInfo, bool updateView, out PolemizeThreadV5 thread, out PostCollectionV5 posts, out ThreadType realThreadType, out int totalCount)
        {
            thread = null;
            posts = null;
            totalCount = 0;

            if (postType == null)
            {
                BasicThread tempThread;
                GetThreadWithReplies(threadID, ThreadType.Polemize, pageNumber, pageSize, getExtendedInfo, updateView, true, true, out tempThread, out posts, out realThreadType);

                if (realThreadType != ThreadType.Polemize)
                    return;

                if (tempThread != null)
                {
                    thread = (PolemizeThreadV5)tempThread;
                    totalCount = thread.TotalReplies + 1;
                }
            }
            else
            {
                BasicThread tempThread = ThreadCachePool.GetThread(threadID);

                realThreadType = ThreadType.Polemize;
                bool getThread = true;
                if (tempThread != null)
                {
                    if (checkThreadType(tempThread.ThreadType, ref realThreadType) == false)
                    {
                        return;
                    }
                    getThread = false;
                }

                PostDaoV5.Instance.GetPolemizeWithReplies(threadID, postType, pageNumber, pageSize, getExtendedInfo, getThread, true, true, ref tempThread, out posts, ref realThreadType, out totalCount);

                if (checkThreadType(tempThread.ThreadType, ref realThreadType) == false)
                {
                    return;
                }

                if (tempThread != null)
                {
                    thread = (PolemizeThreadV5)tempThread;
                }

                if (thread.ExtendDataIsNull)
                {
                    SetThreadExtendData(thread);
                }
            }
        }

        /// <summary>
        /// 获取主题和回复
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="getExtendedInfo">是否获取 附件，评分等</param>
        /// <param name="thread"></param>
        /// <param name="posts"></param>
        /// <param name="realThreadType">返回主题类型  normal,poll,questiong  不包括 joing, move 等</param>
        public void GetThreadWithReplies(int threadID, int pageNumber, int pageSize, bool getExtendedInfo, bool updateView, bool mustCheckThreadType, out BasicThread thread, out PostCollectionV5 posts, out ThreadType realThreadType)
        {
            GetThreadWithReplies(threadID, ThreadType.Normal, pageNumber, pageSize, getExtendedInfo, updateView, false, mustCheckThreadType, out thread, out posts, out realThreadType);
        }

        /// <summary>
        /// 如果不是正常的主题  返回的thread是 NULL
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="currentThreadType"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="getExtendedInfo"></param>
        /// <param name="updateView"></param>
        /// <param name="getThreadContent">如果是 一般主题 即使设为true 也不会取出主题内容 如要修改 需要修改DAO 和存储过程</param>
        /// <param name="mustCheckThreadType"></param>
        /// <param name="thread"></param>
        /// <param name="posts"></param>
        /// <param name="realThreadType"></param>
        public void GetThreadWithReplies(int threadID, ThreadType currentThreadType, int pageNumber, int pageSize, bool getExtendedInfo, bool updateView, bool getThreadContent, bool mustCheckThreadType, out BasicThread thread, out PostCollectionV5 posts, out ThreadType realThreadType)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            posts = null;
            bool isInListCache;
            thread = ThreadCachePool.GetThread(threadID, out isInListCache);
            realThreadType = currentThreadType;
            if (thread != null)
            {
                if (mustCheckThreadType && checkThreadType(thread.ThreadType, ref realThreadType) == false)
                {
                    return;
                }

            }


            string flag = "0";

            if (thread != null && isInListCache)
            {
                pageNumber = GetPageNumber(pageNumber, pageSize, thread);

                posts = thread.GetPosts(pageNumber, pageSize, getExtendedInfo);

                if (thread.IsGetFromDataBase)
                    flag = "1:true";
                else
                    flag = "1:false";
            }
            else
            {
                BasicThread tempThread = null;

                if (thread == null)
                {
                    flag = "2:true,pageNumber:" + pageNumber;
                    GetPosts(threadID, true, pageNumber, pageSize, null, getExtendedInfo, true, getThreadContent, mustCheckThreadType, ref tempThread, out posts, ref realThreadType);

                    ThreadCachePool.SetThreadCache(tempThread);
                    SetThreadPageCache(tempThread);
                    thread = tempThread;
                }
                else
                {
                    flag = "3:true,totalreplies:" + thread.TotalReplies+"pageNumber:"+pageNumber;
                    GetPosts(threadID, true, pageNumber, pageSize, thread.TotalReplies + 1, getExtendedInfo, false, getThreadContent, false, ref thread, out posts, ref realThreadType);
                    tempThread = thread;
                }

                if (mustCheckThreadType && realThreadType != currentThreadType)
                {
                    return;
                }

                if (tempThread == null)
                    return;

                tempThread.TotalViews += UpdateThreadViewsJob.GetViews(tempThread.ThreadID);


                pageNumber = GetPageNumber(pageNumber, pageSize, thread);
            }

            if ((thread.ThreadType == ThreadType.Poll
                || thread.ThreadType == ThreadType.Question
                || thread.ThreadType == ThreadType.Polemize) && thread.ExtendDataIsNull)
            {
                SetThreadExtendData(thread);
            }

            List<int> postIDs = null;

            int index = (pageNumber - 1) * pageSize;

            bool hasRecode = false;
            foreach (PostV5 post in posts)
            {

                if (thread.PostedCount > 0)
                {
                    post.PostIndex = post.FloorNumber - 1;
                    if (hasRecode == false && thread.ThreadType == ThreadType.Normal && index != post.FloorNumber - 1)
                    {
                        if (thread.TotalReplies + 1 == thread.PostedCount)//没有被删的回复  或者 未审核的回复
                        {


                            hasRecode = true;

                            thread.ClearPostsCache();
                        }
                    }
                }
                else
                {
                    post.PostIndex = index;
                }

                index++;

                //如果ForumID 是错误的 则记录下来  并更新
                if (post.ForumID != thread.ForumID)
                {
                    post.ForumID = thread.ForumID;
                    if (postIDs == null)
                        postIDs = new List<int>();
                    postIDs.Add(post.PostID);
                }
            }

            if (thread is QuestionThread)
            {
                PostV5 tempPost = ((QuestionThread)thread).BestPost;

                if (thread.PostedCount > 0 && tempPost != null)
                    tempPost.PostIndex = tempPost.FloorNumber - 1;

                if (tempPost != null && tempPost.ForumID != thread.ForumID)
                {
                    if (postIDs == null)
                        postIDs = new List<int>();
                    postIDs.Add(tempPost.PostID);
                }
            }
            if (thread.ThreadContent != null && thread.ThreadContent.ForumID != thread.ForumID)
            {
                if (postIDs == null)
                    postIDs = new List<int>();
                postIDs.Add(thread.ThreadContent.PostID);
            }

            if (postIDs != null)
            {
                PostDaoV5.Instance.UpdatePostsForumID(postIDs, thread.ForumID);
            }

            if (updateView)
            {
                thread.TotalViews++;

                UpdateThreadViewsJob.AddView(threadID);
            }
        }

        /// <summary>
        /// 如果页码大于最大页  就取最后一页
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        private int GetPageNumber(int pageNumber, int pageSize, BasicThread thread)
        {
            int total;
            if (thread is QuestionThread)
            {
                total = ((QuestionThread)thread).TotalRepliesForPage;
            }
            else
                total = thread.TotalRepliesForPage;

            if ((pageNumber - 1) * pageSize > total)
            {
                pageNumber = total / pageSize;
                if (total % pageSize > 0)
                    pageNumber += 1;
            }

            if (pageNumber == 0)
                pageNumber = 1;

            return pageNumber;
        }

        /// <summary>
        /// 检查主题类型
        /// </summary>
        /// <param name="threadType">当前主题类型</param>
        /// <param name="realType"> 进去:希望的主题类型 出来:实际的主题类型 </param>
        /// <returns></returns>
        private bool checkThreadType(ThreadType threadType, ref ThreadType realType)
        {
            ThreadType? type = GetThreadType(threadType);
            if (type == null)
            {
                threadType = ThreadType.Normal;
            }
            else
                threadType = type.Value;

            if (realType != threadType)
            {
                realType = threadType;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="onlyNormal">是否只获取正常的回复，false为获取包括未审核的回复</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount">回复数加主题内容</param>
        /// <param name="getExtendedInfo">是否获取附件 评分</param>
        /// <param name="getThread"></param>
        /// <param name="getThreadContent">如果为true 则 返回的thread 中 包含ThreadContent</param>
        /// <param name="checkThreadType"></param>
        /// <param name="thread"></param>
        /// <param name="posts"></param>
        /// <param name="threadType"></param>
        public void GetPosts(int threadID, bool onlyNormal, int pageNumber, int pageSize, int? totalCount, bool getExtendedInfo, bool getThread, bool getThreadContent, bool checkThreadType, ref BasicThread thread, out PostCollectionV5 posts, ref ThreadType threadType)
        {
            PostDaoV5.Instance.GetPosts(threadID, onlyNormal, pageNumber, pageSize, totalCount, getExtendedInfo, getThread, getThreadContent, checkThreadType, ref thread, out posts, ref threadType);
        }

        public void GetUserPosts(int threadID, int userID, ThreadType currentType, int pageNumber, int pageSize, bool getExtendedInfo, bool mustCheckThreadType, out BasicThread thread, out PostCollectionV5 posts, out ThreadType realThreadType, out int totalCount)
        {
            totalCount = 0;
            posts = null;
            thread = ThreadCachePool.GetThread(threadID);
            bool getThread = true;
            realThreadType = currentType;
            if(thread != null)
            {
                if (mustCheckThreadType && checkThreadType(thread.ThreadType, ref realThreadType) == false)
                {
                    return;
                }
                getThread = false;
                mustCheckThreadType = false;
            }
            PostDaoV5.Instance.GetUserPosts(threadID, userID, pageNumber, pageSize, getExtendedInfo, getThread, mustCheckThreadType, ref thread, out posts, ref realThreadType, out totalCount);

            if (thread == null)
                return;

            if (mustCheckThreadType && realThreadType != currentType)
            {
                return;
            }

            if ((thread.ThreadType == ThreadType.Poll
                || thread.ThreadType == ThreadType.Question
                || thread.ThreadType == ThreadType.Polemize) && thread.ExtendDataIsNull)
            {
                SetThreadExtendData(thread);
            }
        }

        public  PostCollectionV5 GetPosts(int threadID)
        {
            PostCollectionV5 posts = PostDaoV5.Instance.GetPosts(threadID);
            return posts;
        }

        public PostCollectionV5 GetPosts(IEnumerable<int> postIDs)
        {
            if (!ValidateUtil.HasItems<int>(postIDs))
                return new PostCollectionV5();
            PostCollectionV5 posts = PostDaoV5.Instance.GetPosts(postIDs);
            return posts;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="totalReplies">回复数</param>
        /// <param name="getOldCount"></param>
        /// <param name="getNewCount"></param>
        /// <param name="thread"></param>
        /// <param name="threadContent"></param>
        /// <param name="topPosts"></param>
        public void GetPosts(int threadID, int totalReplies, int getOldCount, int getNewCount, ref BasicThread thread, out PostV5 threadContent, out PostCollectionV5 topPosts)
        {
            PostDaoV5.Instance.GetPosts(threadID, totalReplies, getOldCount, getNewCount, ref thread, out threadContent, out topPosts);
        }

        #region 添加  更新

        public void SetThreadExtendData(BasicThread thread)
        {
            PostDaoV5.Instance.SetThreadExtendData(thread);
        }


        /// <summary>
        /// 发完帖子成功后调用
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="attachments"></param>
        private void SetThreadImage(int threadID, AttachmentCollection attachments, List<int> attacmentIDs, Dictionary<string,int> fileIDs)
        {
            int count = 0;
            string imgurl = null;
            //ThreadAttachType? attachType = null;
            int attachmentID = 0;

            //int j = 0;
            int m = 0;
            foreach (Attachment attach in attachments)
            {
                if (attach.AttachType == AttachType.History)
                {
                    continue;
                }
                switch (attach.FileType.ToLower())
                {
                    case "jpg":
                    case "jpeg":
                    case "gif":
                    case "bmp":
                    case "png":
                        count++;
                        if (imgurl == null)
                        {
                            if (fileIDs.Count > m)
                            {
                                int n = 0;
                                foreach (int id in fileIDs.Values)
                                {
                                    if (n == m)
                                    {
                                        attachmentID = id;
                                        break;
                                    }
                                    n++;
                                }
                            }
                            //if (attach.AttachType == AttachType.TempAttach)
                            //{
                            //    attachmentID = attacmentIDs[j];
                            //}
                            //else
                            //{
                            //    attachmentID = fileIDs[attach.FileID];
                            //}

                            imgurl = string.Empty;
                        }
                        break;
                    default: break;
                }
            }

            if (imgurl != null)
            {
                PostDaoV5.Instance.SetThreadImage(threadID, attachmentID, imgurl, count);
            }
        }

        private AttachRegex attachRegex = new AttachRegex();
        private ThreadAttachType GetThreadAttachType(AttachmentCollection attachments, string content, int postUserID)
        {
            if (attachments != null)
            {
                foreach (Attachment attach in attachments)
                {
                    switch (attach.FileType.ToLower())
                    {
                        case "jpg":
                        case "jpeg":
                        case "gif":
                        case "bmp":
                        case "png":
                            return ThreadAttachType.Image;
                        default:
                            break;
                    }
                }
            }

            MatchCollection matchs = attachRegex.Matches(content);

            List<int> attachmentIDs = new List<int>();
            foreach (Match match in matchs)
            {
                int id;
                if (int.TryParse(match.Groups["id"].Value, out id))
                {
                    if (attachments.GetValue(id) == null)
                        attachmentIDs.Add(id);
                }
            }

            AttachmentCollection historyAttachs = GetAttachments(postUserID, attachmentIDs);

            if (attachments != null && attachments.Count == 0 && historyAttachs.Count == 0)
                return ThreadAttachType.NoAttach;

            foreach (Attachment attach in historyAttachs)
            {
                switch (attach.FileType.ToLower())
                {
                    case "jpg":
                    case "jpeg":
                    case "gif":
                    case "bmp":
                    case "png":
                        return ThreadAttachType.Image;
                    default:
                        break;
                }
            }


            return ThreadAttachType.Normal;

        }


        public bool CreateThread(AuthUser operatorUser, bool ignorePermission, bool enableEmoticons, int forumID, int threadCatalogID, int iconID
             , string subject, string subjectStyle, int price, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
             , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments
             , out int threadID, out int postID)
        {
            threadID = 0;
            postID = 0;
            int operatorUserID = operatorUser.UserID;

            ForumPermissionSetNode forumPermission = null;

            Forum forum = ForumBO.Instance.GetForum(forumID, true);
            if (forum == null)
            {
                ThrowError<ForumNotExistsError>(new ForumNotExistsError("ForumNotExistsError", forumID));
                return false;
            }


            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);

            if (operatorUserID != 0 && forum.IsShieldedUser(operatorUserID))
            {
                ThrowError<CreateThreadIsShieldedUserError>(new CreateThreadIsShieldedUserError());
                return false;
            }


            bool isApprove = true;

            if (!ignorePermission)
            {
                forumPermission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID);

                if (!forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.CreateThread))
                {
                    ThrowError<NoPermissionCreateThreadError>(new NoPermissionCreateThreadError(operatorUserID));
                    return false;
                }

                if (enableSignature)
                {
                    enableSignature = forumSetting.ShowSignatureInThread.GetValue(operatorUser);
                }
            }

            if (false == validateCreateThread(operatorUser, forumID, price, threadCatalogID, forum, forumSetting, forumPermission, ignorePermission, ref isApprove))
                return false;


            if (false == validateCreateUpdatePost(operatorUser, forumID, enableEmoticons, attachments, forumSetting, forumPermission, ignorePermission, true, false, ref subject, ref content, ref enableHtml, ref enableMaxCode3, ref enableReplyNotice, ref isApprove))
            {
                return false;
            }


            int userTotalThreads = 0, userTotalPosts = 0;

            bool hasCreate = false;

            TempUploadFileCollection tempUploadFiles = null;


            bool success = false;


            BasicThread thread = null;
            PostV5 post = null;

            ThreadAttachType attachType = GetThreadAttachType(attachments, content, operatorUserID);

            ThreadStatus threadStatus = ThreadStatus.Normal;
            if (!isApprove)
                threadStatus = ThreadStatus.UnApproved;
            else
            {
                if (operatorUserID != 0)
                {
                    success = ForumPointAction.Instance.UpdateUserPoint(operatorUserID, ForumPointType.CreateThread, 1, true, forum.ForumID, delegate(PointActionManager.TryUpdateUserPointState state)
                    {
                        if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                        {
                            List<int> historyAttachmentIDs;
                            bool tempSuccess = ProcessAttachments(operatorUser, forumID, null, forumSetting, content, attachments, false, true, out historyAttachmentIDs, out tempUploadFiles);
                            if (!tempSuccess)
                                return false;

                            List<int> attachmentIDs;
                            Dictionary<string, int> fileIDs;


                            tempSuccess = PostDaoV5.Instance.CreateThread(
                                forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, price, operatorUserID, postNickName, isLocked
                                , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs
                                , attachType, GetWords(subject), out thread, out post, out userTotalThreads, out userTotalPosts, out attachmentIDs, out fileIDs);

                            if (tempSuccess)
                            {
                                if (false == ProcessPostContentAttachTag(post.PostID, forumID, operatorUserID, attachments, content, attachmentIDs, fileIDs))
                                    return false;

                                SetThreadImage(thread.ThreadID, attachments, attachmentIDs, fileIDs);

                                return true;
                            }
                            else
                                return false;

                        }
                        else
                            return false;
                    });

                    if (success && tempUploadFiles != null)
                        tempUploadFiles.Save();


                    if (success == false)
                    {
                        return false;
                    }

                    hasCreate = true;
                }
            }

            if (hasCreate == false)
            {
                List<int> historyAttachmentIDs;
                success = ProcessAttachments(operatorUser, forumID, postID, forumSetting, content, attachments, false, true, out historyAttachmentIDs, out tempUploadFiles);
                if (success)
                {
                    List<int> attachmentIDs;

                    Dictionary<string, int> fileIDs;

                    success = PostDaoV5.Instance.CreateThread(
                        forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, price, operatorUserID, postNickName, isLocked
                        , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs
                        , attachType, GetWords(subject), out thread, out post, out userTotalThreads, out userTotalPosts, out attachmentIDs, out fileIDs);

                    if (success)
                    {
                        if (false == ProcessPostContentAttachTag(post.PostID, forumID, operatorUserID, attachments, content, attachmentIDs, fileIDs))
                            return false;

                        SetThreadImage(thread.ThreadID, attachments, attachmentIDs, fileIDs);
                    }

                }

                if (success && tempUploadFiles != null)
                    tempUploadFiles.Save();
            }


            if (success)
            {
                RemoveTopicSearchCount();
                threadID = thread.ThreadID;
                postID = post.PostID;

                return ProcessAfterCreateThread(thread, post, forum, operatorUser, threadStatus, userTotalThreads, userTotalPosts);
            }
            else
                return false;

        }


        private string GetWords(string input)
        {
            ICollection<PanGu.WordInfo> words = new Segment().DoSegment(input);
            StringBuilder result = new StringBuilder();

            List<string> list = new List<string>();
            foreach (WordInfo word in words)
            {
                if (word != null && word.Word.Length > 1)
                {
                    bool has = false;
                    foreach (string w in list)
                    {
                        if (string.Compare(w, word.Word, true) == 0)
                        {
                            has = true;
                            break;
                        }
                    }
                    if (has)
                        continue;

                    list.Add(word.Word);
                    result.Append(word.Word).Append(",");
                }
            }
            if (result.Length > 0)
                return result.ToString(0, result.Length - 1);
            else
                return string.Empty;
        }


        public bool CreatePoll(string pollItemString, int pollMultiple, bool pollIsAlwaysEyeable, TimeSpan pollExpiresTime
             , AuthUser operatorUser, bool ignorePermission, bool enableEmoticons, int forumID, int threadCatalogID, int iconID
             , string subject, string subjectStyle, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
             , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments
             , out int threadID, out int postID)
        {
            threadID = 0;
            postID = 0;

            int operatorUserID = operatorUser.UserID;

            ForumPermissionSetNode forumPermission = null;

            Forum forum = ForumBO.Instance.GetForum(forumID, true);
            if (forum == null)
            {
                ThrowError<ForumNotExistsError>(new ForumNotExistsError("ForumNotExistsError", forumID));
                return false;
            }

            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);

            if (operatorUserID != 0 && forum.IsShieldedUser(operatorUserID))
            {
                ThrowError<CreateThreadIsShieldedUserError>(new CreateThreadIsShieldedUserError());
                return false;
            }


            bool isApprove = true;

            if (!ignorePermission)
            {
                forumPermission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID);

                if (!forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.CreatePoll))
                {
                    ThrowError(new NoPermissionCreatePollError(operatorUserID));
                    return false;
                }

                if (enableSignature)
                {
                    enableSignature = forumSetting.ShowSignatureInThread.GetValue(operatorUser);
                }
            }

            if (false == validateCreateThread(operatorUser, forumID, 0, threadCatalogID, forum, forumSetting, forumPermission, ignorePermission, ref isApprove))
                return false;


            string[] pollItems;
            if (pollItemString.IndexOf("\r\n") > 0)
                pollItems = System.Text.RegularExpressions.Regex.Split(pollItemString, "\r\n");
            else
                pollItems = System.Text.RegularExpressions.Regex.Split(pollItemString, "\n");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int pollItemCount = 0;
            for (int i = 0; i < pollItems.Length; i++)
            {
                if (pollItems[i].Trim() != "")
                {
                    sb.Append(pollItems[i].Trim());
                    if (i != pollItems.Length - 1)
                        sb.Append("\r\n");
                    pollItemCount++;
                }
            }

            

            if (pollItemCount > AllSettings.Current.BbsSettings.MaxPollItemCount || pollItemCount < 2)
            {
                ThrowError(new InvalidPollItemCountError("pollItem", AllSettings.Current.BbsSettings.MaxPollItemCount, 2, pollItemCount));
                return false;
            }

            if (pollMultiple < 1)
            {
                ThrowError(new InvalidPollMultipleError("pollMultiple", pollMultiple));
                return false;
            }
            
            long seconds = forumSetting.PollValidDays[operatorUser];
            if (seconds != 0 && pollExpiresTime.TotalSeconds > seconds)
            {
                string time = getTime(seconds);
                ThrowError(new InvalidPollExpiresDateError("pollExpiresTime", time, pollExpiresTime));
                return false;
            }
            DateTime pollExpiresDate;
            if(pollExpiresTime.TotalSeconds == 0)
                pollExpiresDate = DateTimeUtil.Now.AddSeconds(seconds);
            else
                pollExpiresDate = DateTimeUtil.Now.Add(pollExpiresTime);

            if (sb.Length > 4000)//数据库字段 允许存4000
            {
                ThrowError(new InvalidPollItemContentLengthError("pollItemContentLengths"));
                return false;
            }
            pollItemString = sb.ToString();



            if (false == validateCreateUpdatePost(operatorUser, forumID, enableEmoticons, attachments, forumSetting, forumPermission, ignorePermission, true, false, ref subject, ref content, ref enableHtml, ref enableMaxCode3, ref enableReplyNotice, ref isApprove))
            {
                return false;
            }


            int userTotalThreads = 0, userTotalPosts = 0;

            bool hasCreate = false;

            TempUploadFileCollection tempUploadFiles = null;


            bool success = false;


            BasicThread thread = null;
            PostV5 post = null;

            ThreadAttachType attachType = GetThreadAttachType(attachments, content, operatorUserID);

            ThreadStatus threadStatus = ThreadStatus.Normal;

            if (!isApprove)
                threadStatus = ThreadStatus.UnApproved;
            else
            {
                if (operatorUserID != 0)
                {
                    success = ForumPointAction.Instance.UpdateUserPoint(operatorUserID, ForumPointType.CreateThread, 1, true, forum.ForumID, delegate(PointActionManager.TryUpdateUserPointState state)
                    {
                        if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                        {
                            List<int> historyAttachmentIDs;
                            bool tempSuccess = ProcessAttachments(operatorUser, forumID, null, forumSetting, content, attachments, false, true, out historyAttachmentIDs, out tempUploadFiles);
                            if (!tempSuccess)
                                return false;

                            List<int> attachmentIDs;
                            Dictionary<string, int> fileIDs;

                            tempSuccess = PostDaoV5.Instance.CreatePoll(pollItemString, pollMultiple, pollIsAlwaysEyeable, pollExpiresDate
                                , forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, operatorUserID, postNickName, isLocked
                                , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs
                                , attachType, GetWords(subject), out thread, out post, out userTotalThreads, out userTotalPosts, out attachmentIDs, out fileIDs);

                            if (tempSuccess)
                            {
                                if (false == ProcessPostContentAttachTag(post.PostID, forumID, operatorUserID, attachments, content, attachmentIDs, fileIDs))
                                    return false;

                                SetThreadImage(thread.ThreadID, attachments, attachmentIDs, fileIDs);
                                return true;
                            }
                            else
                                return false;

                        }
                        else
                            return false;
                    });

                    if (success && tempUploadFiles != null)
                        tempUploadFiles.Save();


                    if (success == false)
                    {
                        return false;
                    }

                    hasCreate = true;
                }
            }

            if (hasCreate == false)
            {
                List<int> historyAttachmentIDs;
                success = ProcessAttachments(operatorUser, forumID, postID, forumSetting, content, attachments, false, true, out historyAttachmentIDs, out tempUploadFiles);
                if (success)
                {
                    List<int> attachmentIDs;

                    Dictionary<string, int> fileIDs;

                    success = PostDaoV5.Instance.CreatePoll(pollItemString, pollMultiple, pollIsAlwaysEyeable, pollExpiresDate
                        , forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, operatorUserID, postNickName, isLocked
                        , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs
                        , attachType, GetWords(subject), out thread, out post, out userTotalThreads, out userTotalPosts, out attachmentIDs, out fileIDs);

                    if (success)
                    {
                        if (false == ProcessPostContentAttachTag(post.PostID, forumID, operatorUserID, attachments, content, attachmentIDs, fileIDs))
                            return false;
                        SetThreadImage(thread.ThreadID, attachments, attachmentIDs, fileIDs);
                    }

                }

                if (success && tempUploadFiles != null)
                    tempUploadFiles.Save();
            }


            if (success)
            {
                RemoveTopicSearchCount();
                threadID = thread.ThreadID;
                postID = post.PostID;

                return ProcessAfterCreateThread(thread, post, forum, operatorUser, threadStatus, userTotalThreads, userTotalPosts);
            }
            else
                return false;

        }


        public bool CreateQuestion(int questionReward, int questionRewardCount, bool questionIsAlwaysEyeable, TimeSpan questionExpiresTime
             , AuthUser operatorUser, bool ignorePermission, bool enableEmoticons, int forumID, int threadCatalogID, int iconID
             , string subject, string subjectStyle, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
             , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments
             , out int threadID, out int postID)
        {
            threadID = 0;
            postID = 0;

            int operatorUserID = operatorUser.UserID;

            if (operatorUserID == 0)
            {
                ThrowError(new NoPermissionCreateQuestionError(operatorUserID));
                return false;
            }

            ForumPermissionSetNode forumPermission = null;

            Forum forum = ForumBO.Instance.GetForum(forumID, true);
            if (forum == null)
            {
                ThrowError<ForumNotExistsError>(new ForumNotExistsError("ForumNotExistsError", forumID));
                return false;
            }

            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);

            if (operatorUserID != 0 && forum.IsShieldedUser(operatorUserID))
            {
                ThrowError<CreateThreadIsShieldedUserError>(new CreateThreadIsShieldedUserError());
                return false;
            }

            bool isApprove = true;

            if (!ignorePermission)
            {
                forumPermission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID);

                if (!forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.CreateQuestion))
                {
                    ThrowError(new NoPermissionCreateQuestionError(operatorUserID));
                    return false;
                }

                if (enableSignature)
                {
                    enableSignature = forumSetting.ShowSignatureInThread.GetValue(operatorUser);
                }
            }

            if (false == validateCreateThread(operatorUser, forumID, 0, threadCatalogID, forum, forumSetting, forumPermission, ignorePermission, ref isApprove))
                return false;


            long seconds = forumSetting.QuestionValidDays[operatorUser];
            if (seconds != 0 && questionExpiresTime.TotalSeconds > seconds)
            {
                string time = getTime(seconds);
                ThrowError(new InvalidQuestionExpiresDateError("questionExpiresTime", time, questionExpiresTime));
                return false;
            }

            DateTime questionExpiresDate = DateTimeUtil.Now.Add(questionExpiresTime);

            if (questionReward < 1)
            {
                ThrowError(new InvalidQuestionRewardError("questionReward", questionReward));
                return false;
            }
            if (questionRewardCount < 1)
            {
                ThrowError(new InvalidQuestionRewardCountError("questionRewardCount", questionRewardCount));
                return false;
            }
            if (questionReward < questionRewardCount)
            {
                ThrowError(new InvalidQuestionRewardAndRewardCountError("questionReward"));
                return false;
            }



            if (false == validateCreateUpdatePost(operatorUser, forumID, enableEmoticons, attachments, forumSetting, forumPermission, ignorePermission, true, false, ref subject, ref content, ref enableHtml, ref enableMaxCode3, ref enableReplyNotice, ref isApprove))
            {
                return false;
            }


            int userTotalThreads = 0, userTotalPosts = 0;


            TempUploadFileCollection tempUploadFiles = null;


            bool success = false;


            BasicThread thread = null;
            PostV5 post = null;

            Dictionary<ForumPointType, int> noNeedValuePoints = null;

            ThreadStatus threadStatus = ThreadStatus.Normal;

            if (!isApprove)
            {
                threadStatus = ThreadStatus.UnApproved;
            }
            else
            {
                noNeedValuePoints = new Dictionary<ForumPointType, int>();
                noNeedValuePoints.Add(ForumPointType.CreateThread, 1);
            }

            ThreadAttachType attachType = GetThreadAttachType(attachments, content, operatorUserID);

            Dictionary<ForumPointValueType, int> needValuePoints = new Dictionary<ForumPointValueType, int>();
            needValuePoints.Add(ForumPointValueType.QuestionReward, -questionReward);

            success = ForumPointAction.Instance.UpdateUserPoints(operatorUserID, noNeedValuePoints, needValuePoints, forum.ForumID, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    List<int> historyAttachmentIDs;
                    bool tempSuccess = ProcessAttachments(operatorUser, forumID, null, forumSetting, content, attachments, false, true, out historyAttachmentIDs, out tempUploadFiles);
                    if (!tempSuccess)
                        return false;

                    List<int> attachmentIDs;
                    Dictionary<string, int> fileIDs;

                    tempSuccess = PostDaoV5.Instance.CreateQuestion(questionReward, questionRewardCount, questionIsAlwaysEyeable, questionExpiresDate
                        , forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, operatorUserID, postNickName, isLocked
                        , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs
                        , attachType, GetWords(subject), out thread, out post, out userTotalThreads, out userTotalPosts, out attachmentIDs, out fileIDs);

                    if (tempSuccess)
                    {
                        if (false == ProcessPostContentAttachTag(post.PostID, forumID, operatorUserID, attachments, content, attachmentIDs, fileIDs))
                            return false;

                        SetThreadImage(thread.ThreadID, attachments, attachmentIDs, fileIDs);
                        return true;
                    }
                    else
                        return false;

                }
                else
                    return false;
            });

            if (success && tempUploadFiles != null)
                tempUploadFiles.Save();


            if (success)
            {
                RemoveTopicSearchCount();
                threadID = thread.ThreadID;
                postID = post.PostID;

                return ProcessAfterCreateThread(thread, post, forum, operatorUser, threadStatus, userTotalThreads, userTotalPosts);
            }
            else
                return false;

        }


        public bool CreatePolemize(string agreeViewPoint, string againstViewPoint, TimeSpan polemizeExpiresTime
            , AuthUser operatorUser, bool ignorePermission, bool enableEmoticons, int forumID, int threadCatalogID, int iconID
            , string subject, string subjectStyle, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
            , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments
            , out int threadID, out int postID)
        {
            threadID = 0;
            postID = 0;

            ForumPermissionSetNode forumPermission = null;

            Forum forum = ForumBO.Instance.GetForum(forumID, true);
            if (forum == null)
            {
                ThrowError<ForumNotExistsError>(new ForumNotExistsError("ForumNotExistsError", forumID));
                return false;
            }

            int operatorUserID = operatorUser.UserID;

            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);

            if (operatorUserID != 0 && forum.IsShieldedUser(operatorUserID))
            {
                ThrowError<CreateThreadIsShieldedUserError>(new CreateThreadIsShieldedUserError());
                return false;
            }

            bool isApprove = true;

            if (!ignorePermission)
            {
                forumPermission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID);

                if (!forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.CreatePolemize))
                {
                    ThrowError(new NoPermissionCreatePolemizeError(operatorUserID));
                    return false;
                }

                if (enableSignature)
                {
                    enableSignature = forumSetting.ShowSignatureInThread.GetValue(operatorUser);
                }
            }

            if (false == validateCreateThread(operatorUser, forumID, 0, threadCatalogID, forum, forumSetting, forumPermission, ignorePermission, ref isApprove))
                return false;


            long seconds = forumSetting.PolemizeValidDays[operatorUser];
            if (seconds != 0 && polemizeExpiresTime.TotalSeconds > seconds)
            {
                string time = getTime(seconds);

                ThrowError(new InvalidPolemizeExpiresDateError("polemizeExpiresTime", time, polemizeExpiresTime));
                return false;
            }

            DateTime polemizeExpiresDate = DateTimeUtil.Now.Add(polemizeExpiresTime);

            if (string.IsNullOrEmpty(agreeViewPoint))
            {
                ThrowError(new EmptyAgreeViewPointError("agreeViewPoint"));
                return false;
            }

            if (string.IsNullOrEmpty(againstViewPoint))
            {
                ThrowError(new EmptyAgainstViewPointError("againstViewPoint"));
                return false;
            }

            if (false == validateCreateUpdatePost(operatorUser, forumID, enableEmoticons, attachments, forumSetting, forumPermission, ignorePermission, true, false, ref subject, ref content, ref enableHtml, ref enableMaxCode3, ref enableReplyNotice, ref isApprove))
            {
                return false;
            }


            int userTotalThreads = 0, userTotalPosts = 0;

            bool hasCreate = false;

            TempUploadFileCollection tempUploadFiles = null;


            bool success = false;


            BasicThread thread = null;
            PostV5 post = null;

            ThreadAttachType attachType = GetThreadAttachType(attachments, content, operatorUserID);
            ThreadStatus threadStatus = ThreadStatus.Normal;
            if (!isApprove)
                threadStatus = ThreadStatus.UnApproved;
            else
            {
                if (operatorUserID != 0)
                {
                    success = ForumPointAction.Instance.UpdateUserPoint(operatorUserID, ForumPointType.CreateThread, 1, true, forum.ForumID, delegate(PointActionManager.TryUpdateUserPointState state)
                    {
                        if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                        {
                            List<int> historyAttachmentIDs;
                            bool tempSuccess = ProcessAttachments(operatorUser, forumID, null, forumSetting, content, attachments, false, true, out historyAttachmentIDs, out tempUploadFiles);
                            if (!tempSuccess)
                                return false;

                            List<int> attachmentIDs;
                            Dictionary<string, int> fileIDs;

                            tempSuccess = PostDaoV5.Instance.CreatePolemize(agreeViewPoint, againstViewPoint, polemizeExpiresDate
                                , forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, operatorUserID, postNickName, isLocked
                                , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs
                                , attachType, GetWords(subject), out thread, out post, out userTotalThreads, out userTotalPosts, out attachmentIDs, out fileIDs);

                            if (tempSuccess)
                            {
                                if (false == ProcessPostContentAttachTag(post.PostID, forumID, operatorUserID, attachments, content, attachmentIDs, fileIDs))
                                    return false;

                                SetThreadImage(thread.ThreadID, attachments, attachmentIDs, fileIDs);
                                return true;
                            }
                            else
                                return false;

                        }
                        else
                            return false;
                    });

                    if (success && tempUploadFiles != null)
                        tempUploadFiles.Save();


                    if (success == false)
                    {
                        return false;
                    }

                    hasCreate = true;
                }
            }

            if (hasCreate == false)
            {
                List<int> historyAttachmentIDs;
                success = ProcessAttachments(operatorUser, forumID, postID, forumSetting, content, attachments, false, true, out historyAttachmentIDs, out tempUploadFiles);
                if (success)
                {
                    List<int> attachmentIDs;

                    Dictionary<string, int> fileIDs;

                    success = PostDaoV5.Instance.CreatePolemize(agreeViewPoint, againstViewPoint, polemizeExpiresDate
                        , forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, operatorUserID, postNickName, isLocked
                        , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs
                        , attachType, GetWords(subject), out thread, out post, out userTotalThreads, out userTotalPosts, out attachmentIDs, out fileIDs);

                    if (success)
                    {
                        if (false == ProcessPostContentAttachTag(post.PostID, forumID, operatorUserID, attachments, content, attachmentIDs, fileIDs))
                            return false;
                        SetThreadImage(thread.ThreadID, attachments, attachmentIDs, fileIDs);
                    }

                }

                if (success && tempUploadFiles != null)
                    tempUploadFiles.Save();
            }


            if (success)
            {

                RemoveTopicSearchCount();

                threadID = thread.ThreadID;
                postID = post.PostID;

                return ProcessAfterCreateThread(thread, post, forum, operatorUser, threadStatus, userTotalThreads, userTotalPosts);
            }
            else
                return false;

        }

        private string getTime(long seconds)
        {
            int timeValue;
            string timeUnit;
            int days = (int)seconds / (60 * 60 * 24);
            int hours = (int)seconds / (60 * 60);
            int minutes = (int)seconds / 60;
            if (days > 0)
            {
                timeValue = days;
                timeUnit = "天";
            }
            else if (hours > 0)
            {
                timeValue = hours;
                timeUnit = "小时";
            }
            else if (minutes > 0)
            {
                timeValue = minutes;
                timeUnit = "分钟";
            }
            else
            {
                timeValue = (int)seconds;
                timeUnit = "秒";
            }
            return timeValue + timeUnit;
        }

        public bool UpdateThread(AuthUser operatorUser, int threadID, int threadCatalogID, int iconID
            , string subject, int price, int lastEditorID, string lastEditorName, string content, bool enableEmoticons, bool enableHtml
            , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, AttachmentCollection attachments, bool ignorePermission)
        {

            PostV5 threadContent = null;

            BasicThread thread = GetThread(threadID);
            if(thread == null)
            {
                ThrowError(new ThreadNotExistsError());
                return false;
            }
            if (thread.ThreadContent != null)
                threadContent = thread.ThreadContent;
            else
                threadContent = GetThreadFirstPost(threadID, false);
            if (threadContent == null)
            {
                ThrowError(new ThreadNotExistsError());
                return false;
            }

            Forum forum = ForumBO.Instance.GetForum(thread.ForumID);
            if (forum == null)
            {
                ThrowError<ForumNotExistsError>(new ForumNotExistsError("ForumNotExistsError", forum.ForumID));
                return false;
            }

            int operatorUserID = operatorUser.UserID;

            if (forum.IsShieldedUser(operatorUserID))
            {
                ThrowError<UpdateThreadIsShieldedUserError>(new UpdateThreadIsShieldedUserError());
                return false;
            }


            bool oldIsApproved = (thread.ThreadStatus != ThreadStatus.UnApproved);

            ManageForumPermissionSetNode manageForumPermission = null;
            ForumPermissionSetNode forumPermission = null;
            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forum.ForumID);

            if (!ignorePermission)
            {
                manageForumPermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID);
                forumPermission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forum.ForumID);

                if (!manageForumPermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreads, thread.PostUserID, threadContent.LastEditorID))
                {
                    if (threadContent.UserID != operatorUserID)
                    {
                        ThrowError(new NoPermissionUpdateThreadError());
                        return false;
                    }
                    else if (!forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.UpdateOwnThread))
                    {
                        ThrowError(new NoPermissionUpdateThreadError());
                        return false;
                    }
                }

                if (thread.PostUserID == operatorUserID && manageForumPermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreads) == false)//自己的帖子
                {
                    int intervals = forumSetting.UpdateOwnPostIntervals[operatorUser];
                    if (intervals != 0 && thread.CreateDate < DateTimeUtil.Now.AddSeconds(0 - intervals))
                    {
                        ThrowError<OverUpdatePostIntervalsError>(new OverUpdatePostIntervalsError(intervals));
                        return false;
                    }
                }

                if (enableSignature)
                {
                    enableSignature = forumSetting.ShowSignatureInThread.GetValue(operatorUser);
                }
            }

            if (false == validateUpdateThread(operatorUser, forum.ForumID, price, threadCatalogID, forum, forumSetting, forumPermission, ignorePermission))
                return false;

            List<int> threadIDs = new List<int>();
            threadIDs.Add(threadID);
            List<int> userIDs = GetPostUserIDsFormThreads(threadIDs);

            bool isApproved = oldIsApproved;

            if (false == validateCreateUpdatePost(operatorUser, forum.ForumID, enableEmoticons, attachments, forumSetting, forumPermission, ignorePermission, true, true,
                 ref subject, ref content, ref enableHtml, ref enableMaxCode3, ref enableReplyNotice, ref isApproved))
            {
                return false;
            }

            if (isApproved)
            {
                if (forumSetting.CreateThreadNeedApprove[operatorUser])
                    isApproved = oldIsApproved;
            }

            int oldThreadCatalogID = thread.ThreadCatalogID;

            bool updatePoint = false, isNormal;

            if (isApproved && oldIsApproved == false)
                updatePoint = true;
            else if (isApproved == false && oldIsApproved)
                updatePoint = true;

            isNormal = isApproved;

            TempUploadFileCollection tempUploadFiles = null;

            PostV5 updatedPost;
            BasicThread updatedThread = null;

            ThreadAttachType attachType = GetThreadAttachType(attachments, content, thread.PostUserID);
            ThreadAttachType oldAttachType = thread.AttachmentType;
            if (updatePoint)
            {
                ForumPointType pointType = ForumPointType.CreateThread;

                bool success;
                success = ForumPointAction.Instance.UpdateUserPoint(threadContent.UserID, pointType, 1, isNormal, forum.ForumID, delegate(PointActionManager.TryUpdateUserPointState state)
                {
                    if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                    {
                        List<int> historyAttachmentIDs;
                        bool tempSuccess = ProcessAttachments(operatorUser, forum.ForumID, threadContent.PostID, forumSetting, content, attachments, true
                            , true, out historyAttachmentIDs, out tempUploadFiles);
                        if (tempSuccess == false)
                            return false;

                        List<int> newAttachmentIDs;
                        Dictionary<string, int> fileIDs;
                        tempSuccess = PostDaoV5.Instance.UpdateThread(threadID, threadContent.PostID, isApproved, threadCatalogID
                            , iconID, subject, price, lastEditorID, lastEditorName, content, enableHtml, enableMaxCode3, enableSignature
                            , enableReplyNotice, attachments, historyAttachmentIDs, attachType, GetWords(subject), out updatedThread, out updatedPost, out newAttachmentIDs, out fileIDs);
                        if (tempSuccess)
                        {
                            if (false == ProcessPostContentAttachTag(updatedPost.PostID, forum.ForumID, operatorUserID, attachments, content, newAttachmentIDs, fileIDs))
                                return false;
                            else
                            {
                                if (oldAttachType == ThreadAttachType.Image && attachType != ThreadAttachType.Image)
                                {
                                    PostDaoV5.Instance.DeleteThreadImage(threadID);
                                }
                                else
                                    SetThreadImage(threadID, attachments, newAttachmentIDs, fileIDs);
                                return true;
                            }
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                });

                if (success && tempUploadFiles != null)
                    tempUploadFiles.Save();

                if (success == false)
                {
                    //TODO:
                    return false;
                    //if (WebEngine.Context.Current.Errors.HasUnCatchedError)
                    //{
                    //    if (isNormal)//积分超出下限
                    //        return CreateUpdatePostStatus.OverMinPoint;
                    //    else // TODO: 由于从正常变成未审核 而扣积分 积分超出下限
                    //        return CreateUpdatePostStatus.OverMinPoint;
                    //}
                    //return status;
                }
            }
            else
            {
                List<int> historyAttachmentIDs;
                bool tempSuccess = ProcessAttachments(operatorUser, forum.ForumID, threadContent.PostID, forumSetting, content, attachments, true
                    , true, out historyAttachmentIDs, out tempUploadFiles);
                if (tempSuccess == false)
                    return false;

                List<int> newAttachmentIDs;
                Dictionary<string, int> fileIDs;
                tempSuccess = PostDaoV5.Instance.UpdateThread(threadID, threadContent.PostID, isApproved, threadCatalogID
                    , iconID, subject, price, lastEditorID, lastEditorName, content, enableHtml, enableMaxCode3, enableSignature
                    , enableReplyNotice, attachments, historyAttachmentIDs, attachType, GetWords(subject), out updatedThread, out updatedPost, out newAttachmentIDs, out fileIDs);
                if (tempSuccess)
                {
                    if (false == ProcessPostContentAttachTag(updatedPost.PostID, forum.ForumID, operatorUserID, attachments, content, newAttachmentIDs, fileIDs))
                        return false;
                    if (oldAttachType == ThreadAttachType.Image && attachType != ThreadAttachType.Image)
                    {
                        PostDaoV5.Instance.DeleteThreadImage(threadID);
                    }
                    else
                        SetThreadImage(threadID, attachments, newAttachmentIDs, fileIDs);
                }
                else
                    return false;

                if (tempUploadFiles != null)
                    tempUploadFiles.Save();
            }

            RemoveTopicSearchCount();

            if (isApproved)
            {
                if (oldIsApproved)
                {
                    ThreadCachePool.UpdateThreadCache(thread, updatedThread);
                    thread.ClearPostsCache();

                    if (oldThreadCatalogID != threadCatalogID)
                    {
                        ThreadCatalog threadCatalog = ForumBO.Instance.GetForumThreadCatalog(forum.ForumID, oldThreadCatalogID);
                        if (threadCatalog != null)
                            threadCatalog.ThreadCount--;
                        ThreadCatalog threadCatalog2 = ForumBO.Instance.GetForumThreadCatalog(forum.ForumID, threadCatalogID);
                        if (threadCatalog2 != null)
                            threadCatalog2.ThreadCount++;
                    }
                }
                else
                {
                    ThreadCachePool.ClearAllCache();
                    ForumBO.Instance.ClearAllCache();

                    UserBO.Instance.RemoveUsersCache(userIDs);

                    if (oldThreadCatalogID != threadCatalogID)
                    {
                        ThreadCatalog threadCatalog = ForumBO.Instance.GetForumThreadCatalog(forum.ForumID, threadCatalogID);
                        if (threadCatalog != null)
                            threadCatalog.ThreadCount++;
                    }

                    if (thread.CreateDate >= new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, DateTimeUtil.Now.Day))
                    {
                        User user = UserBO.Instance.GetUser(thread.PostUserID);
                        if (user != null)
                            UserBO.Instance.UpdateMostActiveUsersCacheWhenPost(user);
                    }
                    else if (thread.CreateDate >= DateTimeUtil.GetMonday())
                    {
                        UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts });
                    }
                    else if (thread.CreateDate.Year == DateTimeUtil.Now.Year && thread.CreateDate.Month == DateTimeUtil.Now.Month)
                    {
                        UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.MonthPosts });
                    }
                }

                return true;
            }
            else
            {
                if (oldIsApproved)//原来是正常的现在变未审核
                {
                    ThreadCachePool.ClearAllCache();
                    UserBO.Instance.RemoveUsersCache(userIDs);
                    ForumBO.Instance.ClearAllCache();

                    ThreadCatalog threadCatalog = ForumBO.Instance.GetForumThreadCatalog(forum.ForumID, oldThreadCatalogID);
                    if (threadCatalog != null)
                        threadCatalog.ThreadCount--;

                    if (thread.CreateDate >= new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, DateTimeUtil.Now.Day))
                    {
                        UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts, ActiveUserType.DayPosts, ActiveUserType.MonthPosts });
                    }
                    else if (thread.CreateDate >= DateTimeUtil.GetMonday())
                    {
                        UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts });
                    }
                    else if (thread.CreateDate.Year == DateTimeUtil.Now.Year && thread.CreateDate.Month == DateTimeUtil.Now.Month)
                    {
                        UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.MonthPosts });
                    }
                }

                ThrowError(new UnapprovedError());
                return false;
            }
        }



        public bool ReplyThread(AuthUser operatorUser, int threadID, PostType postType, int iconID, string subject, string content
            , bool enableEmoticons, bool enableHtml, bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, int forumID, string userName
            , string ipAddress, int parentID, AttachmentCollection attachments, bool ignorePermission
            , out int postID)
        {
            postID = 0;

            BasicThread thread = GetThread(threadID);
            if (thread == null)
            {
                ThrowError(new ThreadNotExistsError());
                return false;
            }

            int operatorUserID = operatorUser.UserID;

            if (thread.IsLocked)
            {
                ThrowError(new ThreadIsLockedCannotReplyError());
                return false;
            }
            if (thread.ThreadStatus == ThreadStatus.Recycled)
            {
                ThrowError(new ThreadIsRecycledCannotReplyError());
                return false;
            }
            else if (thread.ThreadStatus == ThreadStatus.UnApproved)
            {
                ThrowError(new ThreadIsUnapprovedCannotReplyError());
                return false;
            }

            if (thread.ThreadType == ThreadType.Polemize &&
                (postType != PostType.Polemize_Against && postType != PostType.Polemize_Agree && postType != PostType.Polemize_Neutral))
            {
                ThrowError<NotSellectPolemizePointError>(new NotSellectPolemizePointError());
                return false;
            }

            ForumPermissionSetNode forumPermission = null;
            Forum forum = ForumBO.Instance.GetForum(forumID);
            if (forum == null)
            {
                ThrowError(new ForumNotExistsError("ForumNotExistsError", forumID));
                return false;
            }

            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);


            if (operatorUserID != 0 && forum.IsShieldedUser(operatorUserID))
            {
                ThrowError(new ReplyThreadIsShieldedUserError());
                return false;
            }

            bool updateSortOrder = thread.UpdateSortOrder;

            if (updateSortOrder)
            {
                //如果默认排序按发表时间排序  则不顶上去
                if (forumSetting.DefaultThreadSortField == ThreadSortField.CreateDate)
                    updateSortOrder = false;
            }

            if (!ignorePermission)
            {
                //回复时间间隔，允许匿名，连续可以回复多少个,
                forumPermission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID);

                if (forum.CanVisit(operatorUser) == false)
                {
                    ThrowError(new NoPermissionReplyThreadError(operatorUserID));
                    return false;
                }

                if (!forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.ReplyThread))
                {
                    ThrowError(new NoPermissionReplyThreadError(operatorUserID));
                    return false;
                }
                if (thread.ThreadType == ThreadType.Polemize)
                {
                    if (!forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.CanPolemize))//没有权限参与辩论
                    {
                        ThrowError(new NoPermissionPolemizeError(operatorUserID));
                        return false;
                    }
                }
                //回复时间间隔
                int intervals = forumSetting.CreatePostIntervals[operatorUser];
                if (intervals > 0)
                {
                    TimeSpan PostInterval = TimeSpan.FromSeconds(intervals);
                    if (DateTimeUtil.Now - operatorUser.LastPostDate < PostInterval)
                    {
                        ThrowError<OverCreatePostIntervalsError>(new OverCreatePostIntervalsError("PostInterval", PostInterval, operatorUser.LastPostDate));
                        return false;
                    }
                }

                if (enableSignature)
                {
                    enableSignature = forumSetting.ShowSignatureInPost.GetValue(operatorUser);
                }

                if (updateSortOrder)
                {
                    // 离贴子更新时间多久回复主题 主题不会被顶上去
                    int seconds = forumSetting.UpdateThreadSortOrderIntervals.GetValue(thread.PostUserID);
                    if (seconds > 0)
                    {
                        if (DateTimeUtil.Now - thread.UpdateDate > TimeSpan.FromSeconds(seconds))
                        {
                            updateSortOrder = false;
                            List<int> threadIDs = new List<int>();
                            threadIDs.Add(thread.ThreadID);
                            SetThreadNotUpdateSortOrder(operatorUser, thread.ForumID, threadIDs, false, true, false, false, string.Empty);
                        }
                    }
                }

            }

            bool isApprove = true;



            if (false == validateCreateUpdatePost(operatorUser, forumID, enableEmoticons, attachments, forumSetting, forumPermission, ignorePermission, false, false,
                 ref subject, ref content, ref enableHtml, ref enableMaxCode3, ref enableReplyNotice, ref isApprove))
                return false;


            bool hasCreate = false;
            int userTotalPosts = 0;

            TempUploadFileCollection tempUploadFiles = null;

            bool threadEnableReplyNotice = false;
            PostV5 post = null;

            bool mustGetPost = true;//thread.MustGetPostAfterReply();

            bool stickUpdateSortOrder;

            if (thread.ThreadStatus == ThreadStatus.GlobalSticky
                && AllSettings.Current.BbsSettings.GloableStickSortType == StickSortType.StickDate)
                stickUpdateSortOrder = false;
            else if (thread.ThreadStatus == ThreadStatus.Sticky
                && AllSettings.Current.BbsSettings.StickSortType == StickSortType.StickDate)
                stickUpdateSortOrder = false;
            else
                stickUpdateSortOrder = true;

            if (isApprove)
            {
                Dictionary<int, ForumPointType> userActions = new Dictionary<int, ForumPointType>();
                userActions.Add(operatorUserID, ForumPointType.ReplyThread);
                if (operatorUserID != thread.PostUserID)
                    userActions.Add(thread.PostUserID, ForumPointType.ThreadIsReplied);

                //int tempPostID = 0;
                bool success;

                success = ForumPointAction.Instance.UpdateUsersPoint(userActions, 1, forum.ForumID, delegate(PointActionManager.TryUpdateUserPointState state)
                {
                    if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                    {
                        List<int> historyAttachmentIDs;

                        if (false == ProcessAttachments(operatorUser, forumID, null, forumSetting, content, attachments, false, false, out historyAttachmentIDs, out tempUploadFiles))
                            return false;

                        List<int> newAttachmentIDs;
                        Dictionary<string, int> fileIDs;

                        bool createSuccess = PostDaoV5.Instance.CreatePost(threadID, mustGetPost, isApprove, postType, iconID, subject, content, enableHtml, enableMaxCode3
                            , enableSignature, enableReplyNotice, forumID, operatorUserID, operatorUser.Username, ipAddress, parentID, updateSortOrder && stickUpdateSortOrder
                            , attachments, historyAttachmentIDs, thread, out post, out userTotalPosts, out newAttachmentIDs, out fileIDs, out threadEnableReplyNotice);

                        if (createSuccess)
                        {

                            if (false == ProcessPostContentAttachTag(post.PostID, forumID, operatorUserID, attachments, content, newAttachmentIDs, fileIDs))
                                return false;


                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                });


                if (success && tempUploadFiles != null)
                    tempUploadFiles.Save();

                if (success == false)
                {
                    postID = 0;
                    //if (WebEngine.Context.Current.Errors.HasUnCatchedError)
                    //    return CreateUpdatePostStatus.OverMinPoint;
                    //TODO:积分超出下限    
                    return false;
                }
                postID = post.PostID;

                hasCreate = true;
            }
            if (hasCreate == false)
            {
                List<int> historyAttachmentIDs;

                if (false == ProcessAttachments(operatorUser, forumID, null, forumSetting, content, attachments, false, false, out historyAttachmentIDs, out tempUploadFiles))
                    return false;


                List<int> newAttachmentIDs;
                Dictionary<string, int> fileIDs;
                bool success = PostDaoV5.Instance.CreatePost(threadID, mustGetPost, isApprove, postType, iconID, subject, content, enableHtml, enableMaxCode3
                        , enableSignature, enableReplyNotice, forumID, operatorUserID, operatorUser.Username, ipAddress, parentID, updateSortOrder && stickUpdateSortOrder
                        , attachments, historyAttachmentIDs, thread, out post, out userTotalPosts, out newAttachmentIDs, out fileIDs, out threadEnableReplyNotice);

                if (success)
                {
                    if (false == ProcessPostContentAttachTag(post.PostID, forumID, operatorUserID, attachments, content, newAttachmentIDs, fileIDs))
                        return false;
                }
                else
                    return false;

                if (tempUploadFiles != null)
                    tempUploadFiles.Save();
            }



            RemovePostSearchCount();

            if (isApprove)
            {
                BasicThread cachedThread = ThreadCachePool.GetThread(threadID);
                if (cachedThread != null)
                {
                    if (cachedThread != thread)
                    {

                    }
                    thread = cachedThread;
                }

                lock (thread)
                {
                    if (thread.PostedCount > 0)
                        thread.PostedCount++;

                    thread.TotalReplies++;

                    bool mustAddToCache = thread.MustGetPostAfterReply();

                    thread.UpdateDate = DateTimeUtil.Now;
                    thread.LastReplyUserID = operatorUser.UserID;
                    thread.LastReplyUsername = operatorUser.Username;
                    thread.LastPostID = post.PostID;

                    if (mustAddToCache)
                        thread.AddPostToCache(post);
                }

                if (updateSortOrder)
                {
                    if (thread.ThreadStatus == ThreadStatus.GlobalSticky)
                    {
                        if (AllSettings.Current.BbsSettings.GloableStickSortType == StickSortType.LastReplyDate)
                            ThreadCachePool.AddAllForumThread(ThreadCachePool.ThreadOrderType.GlobalStickThreads, thread);
                    }
                    else if (thread.ThreadStatus == ThreadStatus.Sticky)
                    {
                        if (AllSettings.Current.BbsSettings.StickSortType == StickSortType.LastReplyDate)
                            ThreadCachePool.AddForumThread(forumID, ThreadCachePool.ThreadOrderType.ForumStickThreads, thread);
                    }
                    else
                    {
                        //ThreadCachePool.AddForumThread(forumID, ThreadCachePool.ThreadOrderType.ForumTopThreadsByLastPostID, thread);
                        ThreadCachePool.AddForumThread(forumID, ThreadCachePool.ThreadOrderType.ForumTopThreadsBySortOrder, thread);
                    }

                    ThreadCachePool.AddAllForumThread(ThreadCachePool.ThreadOrderType.AllForumTopThreadsByLastPostID, thread);
                    ThreadCachePool.AddForumThread(thread.ForumID, ThreadCachePool.ThreadOrderType.ForumTopThreadsByLastPostID, thread);
                }

                ProcessHotThread(thread);

                if (operatorUserID != 0)
                {
                    //UserBO.Instance.UpdateMostActiveUsersCacheWhenPost(operatorUser);
                    bool oldReplied;
                    if (operatorUser.RepliedThreads.TryGetValue(threadID, out oldReplied))
                    {
                        if (!oldReplied)
                            operatorUser.RepliedThreads[threadID] = true;
                    }

                    operatorUser.TotalPosts = userTotalPosts;
                    UpdateUserPostCount(operatorUser);
                    operatorUser.LastPostDate = DateTimeUtil.Now;

                    UserBO.Instance.UpdateMostActiveUsersCacheWhenPost(operatorUser);

                }
                forum.TotalPosts++;
                forum.TodayPosts++;

                if (updateSortOrder)
                {
                    forum.LastThreadID = thread.ThreadID;
                    SetForumLastThreadCache(forum.ForumID, thread);
                }

                if (thread.PostUserID != operatorUserID)
                {
                    if (threadEnableReplyNotice && forumPermission.Can(thread.PostUserID, ForumPermissionSetNode.Action.PostEnableReplyNotice))
                    {
                        PostNotify notify = new PostNotify(thread.SubjectText, operatorUserID, thread.ThreadID, forum.CodeName);
                        notify.UserID = thread.PostUserID;
                        NotifyBO.Instance.AddNotify(operatorUser, notify);

                    }
                }
                return true;
            }
            else
            {
                if (thread.PostedCount > 0)
                    thread.PostedCount++;
                ThrowError(new UnapprovedError());
                return false;
            }

        }

        private void UpdateUserPostCount(AuthUser user)
        {
            if (user.LastPostDate < DateTimeUtil.GetMonday())
                user.WeekPosts = 1;
            else
                user.WeekPosts++;

            if (user.LastPostDate < DateTimeUtil.Now.Date)
                user.DayPosts = 1;
            else
                user.DayPosts++;

            if (user.LastPostDate < new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, 1))
                user.MonthPosts = 1;
            else
                user.MonthPosts++;
        }


        public bool UpdatePost(AuthUser operatorUser, int postID, int iconID
           , string subject, int lastEditorID, string lastEditorName, string content, bool enableEmoticons, bool enableHtml
           , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, AttachmentCollection attachments
           , bool ignorePermission)
        {

            PostV5 post = GetPost(postID, false);

            ForumPermissionSetNode forumPermission = null;
            Forum forum = ForumBO.Instance.GetForum(post.ForumID);
            if (forum == null)
            {
                ThrowError(new ForumNotExistsError("ForumNotExistsError", forum.ForumID));
                return false;
            }

            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forum.ForumID);

            int operatorUserID = operatorUser.UserID;

            if (forum.IsShieldedUser(operatorUserID))
            {
                ThrowError(new UpdatePostIsShieldedUserError());
                return false;
            }

            if (!ignorePermission)
            {
                forumPermission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forum.ForumID);

                ManageForumPermissionSetNode manageForumPermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID);

                //如果没有管理权限,则进入
                if (!manageForumPermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.UpdatePosts, post.UserID, post.LastEditorID))
                {
                    //如果是自己的帖子,则继续进入
                    if (post.UserID == operatorUserID)
                    {
                        //如果自己发表的帖子自己并没有管理权限,则提示没有权限管理自己的帖子
                        if (!forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.UpdateOwnPost))
                        {
                            ThrowError(new NoPermissionUpdatePostError());
                            return false;
                        }
                    }
                    //如果不是自己的帖子,直接提示没有权限管理其他用户的帖子
                    else
                    {
                        ThrowError(new NoPermissionUpdatePostError());
                        return false;
                    }
                }

                if (post.UserID == operatorUserID && manageForumPermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.UpdatePosts) == false)//自己的帖子
                {
                    int intervals = forumSetting.UpdateOwnPostIntervals[operatorUser];
                    if (intervals != 0 && post.CreateDate < DateTimeUtil.Now.AddSeconds(0 - intervals))
                    {
                        ThrowError<OverUpdatePostIntervalsError>(new OverUpdatePostIntervalsError(intervals));
                        return false;
                    }
                }


                if (enableSignature)
                {
                    enableSignature = forumSetting.ShowSignatureInPost.GetValue(operatorUser);
                }
            }

            bool oldIsApproved = post.IsApproved;
            bool isApproved = post.IsApproved;

            if (false == validateCreateUpdatePost(operatorUser, forum.ForumID, enableEmoticons, attachments, forumSetting, forumPermission, ignorePermission, false, true
                , ref subject, ref content, ref enableHtml, ref enableMaxCode3, ref enableReplyNotice, ref isApproved))
                return false;


                bool updatePoint = false, isNormal;


                if (isApproved && oldIsApproved == false)
                    updatePoint = true;
                else if (isApproved == false && oldIsApproved == true)
                    updatePoint = true;


                isNormal = isApproved;

                TempUploadFileCollection tempUploadFiles = null;


                bool getEntendedInfo = false;
                BasicThread thread = ThreadCachePool.GetThread(post.ThreadID);
                if (thread != null && thread.ContainPostCache(post.PostID))
                {
                    getEntendedInfo = true;
                }

                PostV5 updatedPost = null;

                if (updatePoint)
                {
                    bool success;
                    success = ForumPointAction.Instance.UpdateUserPoint(post.UserID, ForumPointType.ReplyThread, 1, isNormal, forum.ForumID, delegate(PointActionManager.TryUpdateUserPointState state)
                    {
                        if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                        {
                            List<int> historyAttachmentIDs;
                            bool tempSuccess = ProcessAttachments(operatorUser, forum.ForumID, post.PostID, forumSetting, content, attachments, true, false, out historyAttachmentIDs, out tempUploadFiles);

                            if (tempSuccess == false)
                                return false;

                            List<int> newAttachmentIDs;
                            Dictionary<string, int> fileIDs;

                            tempSuccess = PostDaoV5.Instance.UpdatePost(post.PostID, getEntendedInfo, isApproved, iconID, subject, lastEditorID, lastEditorName, content, enableHtml
                                , enableMaxCode3, enableSignature, enableReplyNotice, attachments, historyAttachmentIDs, out updatedPost, out newAttachmentIDs, out fileIDs);
                            if (tempSuccess)
                            {
                                if (false == ProcessPostContentAttachTag(post.PostID,forum.ForumID,operatorUserID,attachments,content,newAttachmentIDs,fileIDs))
                                    return false;

                                return true;
                            }
                            else
                                return false;
                        }
                        else
                            return false;
                    });

                    if (success && tempUploadFiles != null)
                        tempUploadFiles.Save();

                    if (success == false)
                    {
                        //TODO:
                        return false;
                        //if (WebEngine.Context.Current.Errors.HasUnCatchedError)
                        //{
                        //    if (isNormal)//积分超出下限
                        //        return CreateUpdatePostStatus.OverMinPoint;
                        //    else // TODO: 由于从正常变成未审核 而扣积分 积分超出下限
                        //        return CreateUpdatePostStatus.OverMinPoint;
                        //}
                        //return status;
                    }
                }
                else
                {

                    List<int> historyAttachmentIDs;
                    bool tempSuccess = ProcessAttachments(operatorUser, forum.ForumID, post.PostID, forumSetting, content, attachments, true, false, out historyAttachmentIDs, out tempUploadFiles);

                    if (tempSuccess == false)
                        return false;

                    List<int> newAttachmentIDs;
                    Dictionary<string, int> fileIDs;

                    tempSuccess = PostDaoV5.Instance.UpdatePost(post.PostID, getEntendedInfo, isApproved, iconID, subject, lastEditorID, lastEditorName, content, enableHtml
                        , enableMaxCode3, enableSignature, enableReplyNotice, attachments, historyAttachmentIDs, out updatedPost, out newAttachmentIDs, out fileIDs);
                    if (tempSuccess)
                    {
                        if (false == ProcessPostContentAttachTag(post.PostID, forum.ForumID, operatorUserID, attachments, content, newAttachmentIDs, fileIDs))
                            return false;
                    }
                    else
                        return false;

                    if (tempUploadFiles != null)
                        tempUploadFiles.Save();
                }


                RemovePostSearchCount();

                if (isApproved)
                {
                    if (oldIsApproved)
                    {
                        if (thread != null)
                            thread.UpdatePostCache(updatedPost);
                    }
                    else//从未审核变正常
                    {
                        if (thread != null)//说明缓存里有
                        {
                            if (thread.ContainPostCache(postID))
                                thread.ClearPostsCache();
                            thread.TotalReplies++;
                            ProcessHotThread(thread);
                        }
                        ForumBO.Instance.ClearAllCache();
                        UserBO.Instance.RemoveUserCache(post.UserID);

                        if (post.CreateDate >= new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, DateTimeUtil.Now.Day))
                        {
                            User user = UserBO.Instance.GetUser(post.UserID);
                            if (user != null)
                                UserBO.Instance.UpdateMostActiveUsersCacheWhenPost(user);
                        }
                        else if(post.CreateDate>= DateTimeUtil.GetMonday())
                        {
                            UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts });
                        }
                        else if (post.CreateDate.Year == DateTimeUtil.Now.Year && post.CreateDate.Month == DateTimeUtil.Now.Month)
                        {
                            UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.MonthPosts });
                        }
                    }

                    return true;
                }
                else
                {
                    if (oldIsApproved)//从正常变未审核
                    {
                        if (thread != null)//说明缓存里有
                        {
                            if (thread.ContainPostCache(postID))
                                thread.ClearPostsCache();
                            thread.TotalReplies--;
                            ClearHotThreadCache(post.ForumID, new int[] { post.ThreadID });
                        }
                        ForumBO.Instance.ClearAllCache();
                        UserBO.Instance.RemoveUserCache(post.UserID);

                        if (post.CreateDate >= new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, DateTimeUtil.Now.Day))
                        {
                            UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts, ActiveUserType.DayPosts, ActiveUserType.MonthPosts });
                        }
                        else if (post.CreateDate >= DateTimeUtil.GetMonday())
                        {
                            UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.WeekPosts });
                        }
                        else if (post.CreateDate.Year == DateTimeUtil.Now.Year && post.CreateDate.Month == DateTimeUtil.Now.Month)
                        {
                            UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.MonthPosts });
                        }
                    }

                    ThrowError(new UnapprovedError());
                    return false;
                }
        }



        public bool RepairTotalReplyCount(AuthUser operatorUser, int threadID)
        {
            if (operatorUser.IsManager == false)
            {
                ThrowError<NoPermissionRepairTotalReplyCountError>(new NoPermissionRepairTotalReplyCountError());
                return false;
            }
            int total = PostDaoV5.Instance.RepairTotalReplyCount(threadID);

            BasicThread thread = ThreadCachePool.GetThread(threadID);
            if (thread != null)
            {
                thread.TotalReplies = total;
                thread.ClearPostsCache();
            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="post"></param>
        /// <param name="forum"></param>
        /// <param name="operatorUser"></param>
        /// <param name="threadStatus"></param>
        /// <param name="userTotalThreads"></param>
        /// <param name="userTotalPosts"></param>
        /// <returns></returns>
        private bool ProcessAfterCreateThread(BasicThread thread, PostV5 post, Forum forum, AuthUser operatorUser, ThreadStatus threadStatus
            , int userTotalThreads, int userTotalPosts)
        {

            if (threadStatus == ThreadStatus.GlobalSticky)
            {
                ThreadCachePool.AddAllForumThread(ThreadCachePool.ThreadOrderType.GlobalStickThreads, thread);
            }
            else if (threadStatus == ThreadStatus.Sticky)
            {
                ThreadCachePool.AddForumThread(forum.ForumID, ThreadCachePool.ThreadOrderType.ForumStickThreads, thread);
            }

            if (thread.ThreadStatus != ThreadStatus.UnApproved)
            {
                ThreadCachePool.AddForumThread(forum.ForumID, ThreadCachePool.ThreadOrderType.ForumTopThreadsBySortOrder, thread);
                ThreadCachePool.AddAllForumThread(ThreadCachePool.ThreadOrderType.AllForumTopThreadsByThreadID, thread);
                ThreadCachePool.AddForumThread(forum.ForumID, ThreadCachePool.ThreadOrderType.ForumTopThreadsByThreadID, thread);

                ProcessHotThread(thread);

                forum.TotalThreads++;
                forum.TotalPosts++;
                forum.TodayThreads++;
                forum.TodayPosts++;
                forum.LastThreadID = thread.ThreadID;
                SetForumLastThreadCache(forum.ForumID, thread);

                ThreadCatalog threadCatalog = ForumBO.Instance.GetForumThreadCatalog(forum.ForumID, thread.ThreadCatalogID);
                if (threadCatalog != null)
                    threadCatalog.ThreadCount++;

                if (operatorUser != null)
                {
                    //UserBO.Instance.UpdateMostActiveUsersCacheWhenPost(operatorUser);
                    operatorUser.TotalTopics = userTotalThreads;
                    operatorUser.TotalPosts = userTotalPosts;
                    UpdateUserPostCount(operatorUser);
                    operatorUser.LastPostDate = DateTimeUtil.Now;
                    UserBO.Instance.UpdateMostActiveUsersCacheWhenPost(operatorUser);

                    FeedBO.Instance.CreateTopicFeed(operatorUser.UserID, thread.ThreadID, forum, thread.SubjectText, GetPublicContent(thread, post));

                }
                return true;
            }
            else
            {
                ThrowError(new UnapprovedError());
                return false;
            }
        }



        private static object weekHotThreadsLocker = new object();
        private static object dayHotThreadsLocker = new object();
        private void ProcessHotThread(BasicThread thread)
        {
            if (thread.TotalReplies == 0)
                return;

            if (thread.CreateDate >= DateTimeUtil.GetMonday())
            {
                lock (weekHotThreadsLocker)
                {
                    ThreadCollectionV5 tempThreads = ThreadCachePool.GetAllForumThreads(ThreadCachePool.ThreadOrderType.AllForumTopWeekHotThreads);
                    int mustCacheCount = ThreadCachePool.GetTotalCacheCount(ThreadCachePool.ThreadOrderType.AllForumTopWeekHotThreads);
                    ThreadCollectionV5 result = GetProcessedHotThreads(tempThreads, thread, mustCacheCount);
                    if (result != null)
                    {
                        ThreadCachePool.SetAllForumThreads(ThreadCachePool.ThreadOrderType.AllForumTopWeekHotThreads, result);
                    }

                    ThreadCollectionV5 forumThreads = ThreadCachePool.GetForumThreads(thread.ForumID, ThreadCachePool.ThreadOrderType.ForumTopWeekHotThreads);
                    mustCacheCount = ThreadCachePool.GetTotalCacheCount(ThreadCachePool.ThreadOrderType.ForumTopWeekHotThreads);
                    ThreadCollectionV5 forumResult = GetProcessedHotThreads(forumThreads, thread, mustCacheCount);
                    if (forumResult != null)
                    {
                        ThreadCachePool.SetForumThreadsCache(thread.ForumID, ThreadCachePool.ThreadOrderType.ForumTopWeekHotThreads, forumResult);
                    }
                }
            }

            if (thread.CreateDate >= new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, DateTimeUtil.Now.Day))
            {
                lock (dayHotThreadsLocker)
                {
                    ThreadCollectionV5 tempThreads = ThreadCachePool.GetAllForumThreads(ThreadCachePool.ThreadOrderType.AllForumTopDayHotThreads);
                    int mustCacheCount = ThreadCachePool.GetTotalCacheCount(ThreadCachePool.ThreadOrderType.AllForumTopDayHotThreads);
                    ThreadCollectionV5 result = GetProcessedHotThreads(tempThreads, thread, mustCacheCount);
                    if (result != null)
                    {
                        ThreadCachePool.SetAllForumThreads(ThreadCachePool.ThreadOrderType.AllForumTopDayHotThreads, result);
                    }

                    ThreadCollectionV5 forumThreads = ThreadCachePool.GetForumThreads(thread.ForumID, ThreadCachePool.ThreadOrderType.ForumTopDayHotThreads);
                    mustCacheCount = ThreadCachePool.GetTotalCacheCount(ThreadCachePool.ThreadOrderType.ForumTopDayHotThreads);
                    ThreadCollectionV5 forumResult = GetProcessedHotThreads(forumThreads, thread, mustCacheCount);
                    if (forumResult != null)
                    {
                        ThreadCachePool.SetForumThreadsCache(thread.ForumID, ThreadCachePool.ThreadOrderType.ForumTopDayHotThreads, forumResult);
                    }
                }
            }
        }

        private void ClearHotThreadCache(int forumID, IEnumerable<int> threadIDs)
        {
            ThreadCollectionV5 tempThreads = ThreadCachePool.GetAllForumThreads(ThreadCachePool.ThreadOrderType.AllForumTopWeekHotThreads);
            if (tempThreads != null)
            {
                foreach (int id in threadIDs)
                {
                    if (tempThreads.ContainsKey(id))
                    {
                        ThreadCachePool.ClearAllForumThreadsCache(ThreadCachePool.ThreadOrderType.AllForumTopWeekHotThreads);
                        break;
                    }
                }
            }
            tempThreads = ThreadCachePool.GetForumThreads(forumID, ThreadCachePool.ThreadOrderType.ForumTopWeekHotThreads);
            if (tempThreads != null)
            {
                foreach (int id in threadIDs)
                {
                    if (tempThreads.ContainsKey(id))
                    {
                        ThreadCachePool.ClearForumThreadsCache(forumID, ThreadCachePool.ThreadOrderType.ForumTopWeekHotThreads);
                        break;
                    }
                }
            }
        }

        private ThreadCollectionV5 GetProcessedHotThreads(ThreadCollectionV5 tempThreads, BasicThread thread, int mustCacheCount)
        {
            if (tempThreads != null)
            {
                if (tempThreads.Count == 0)
                {
                    ThreadCollectionV5 weekHotThreads = new ThreadCollectionV5();
                    weekHotThreads.Add(thread);
                    return weekHotThreads;
                }
                else
                {
                    ThreadCollectionV5 weekHotThreads = new ThreadCollectionV5(tempThreads);
                    int minReplies = weekHotThreads[0].TotalReplies;

                    if (weekHotThreads.ContainsKey(thread.ThreadID) == false && thread.TotalReplies < minReplies)
                    {
                        if (weekHotThreads.Count < mustCacheCount)
                            weekHotThreads.Add(thread);
                        else
                            return null;
                    }
                    else
                    {
                        weekHotThreads = ProcessThreadsSortByReplies(weekHotThreads, thread, mustCacheCount);
                    }

                    return weekHotThreads;
                }
            }

            return null;
        }

        private ThreadCollectionV5 ProcessThreadsSortByReplies(ThreadCollectionV5 threads, BasicThread thread, int cachedCount)
        {
            ThreadCollectionV5 result = new ThreadCollectionV5();

            bool hasAdd = false;
            for (int i = 0; i < threads.Count; i++)
            {
                if (result.Count == cachedCount)
                    break;

                if (threads[i].ThreadID == thread.ThreadID)
                {
                    continue;
                }

                if (hasAdd == false && thread.TotalReplies >= threads[i].TotalReplies)
                {
                    result.Add(thread);
                    hasAdd = true;
                }
                result.Add(threads[i]);
            }

            return result;
        }



        private bool validateCreateUpdatePost(
            AuthUser operatorUser, int forumID, bool enableEmoticons, AttachmentCollection attachments
            , ForumSettingItem forumSetting, ForumPermissionSetNode forumPermission, bool ignorePermission, bool isThread, bool isEdit, ref string subject
            , ref string content, ref bool enableHtml, ref bool enableMaxCode3, ref bool enableReplyNotice, ref bool isApprove)
        {
            if (isThread && string.IsNullOrEmpty(subject))
            {
                ThrowError<EmptyPostSubjectError>(new EmptyPostSubjectError("subject"));
                return false;
            }

            subject = subject.TrimEnd(' ', '\r', '\n', '\t');

            int subjectLength = StringUtil.GetByteCount(subject);

            if (string.IsNullOrEmpty(content))
            {
                ThrowError(new EmptyPostContentError("content"));
                return false;
            }

            content = content.TrimEnd(' ', '\r', '\n', '\t');

            if (!ignorePermission)
            {
                //回复的时候  如果标题为空  不检查长度
                if (isThread || subject.Trim() != string.Empty)
                {
                    Int32Scope subjectLengths = forumSetting.PostSubjectLengths[operatorUser];
                    int subjectMaxLength = subjectLengths.MaxValue;
                    int subjectMinLength = subjectLengths.MinValue;

                    if(subjectMaxLength > 256)//数据库允许的长度
                        subjectMaxLength = 256;

                    if (subjectLength > subjectMaxLength
                    || subjectLength < subjectMinLength)
                    {
                        ThrowError<InvalidPostSubjectLengthError>(new InvalidPostSubjectLengthError("subject", subjectMaxLength, subjectMinLength, subjectLength));
                        return false;
                    }
                }
                int contentLength = StringUtil.GetByteCount(content);

                Int32Scope contentLengths = forumSetting.PostContentLengths[operatorUser];
                int contentMaxLength = contentLengths.MaxValue;
                int contentMinLength = contentLengths.MinValue;
                if (contentLength > contentMaxLength
                  || contentLength < contentMinLength)
                {
                    ThrowError<InvalidPostContentLengthError>(new InvalidPostContentLengthError("content", contentMaxLength, contentMinLength, contentLength));
                    return false;
                }

                enableHtml = enableHtml && forumSetting.CreatePostAllowHTML[operatorUser];
                enableMaxCode3 = enableMaxCode3 && forumSetting.CreatePostAllowMaxcode[operatorUser];
                enableReplyNotice = enableReplyNotice && forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.PostEnableReplyNotice);


                ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

                string bannedKeyword;
                if (keywords.BannedKeywords.IsMatch(subject, out bannedKeyword))
                {
                    Context.ThrowError<PostSubjectBannedKeywords>(new PostSubjectBannedKeywords("subject", bannedKeyword));
                    return false;
                }

                if (keywords.BannedKeywords.IsMatch(content, out bannedKeyword))
                {
                    Context.ThrowError<PostContentBannedKeywords>(new PostContentBannedKeywords("content", bannedKeyword));
                    return false;
                }

                //bool oldIsApproved = isApprove;

                isApprove = true;

                if (isThread == false)
                {
                    if (forumSetting.ReplyNeedApprove[operatorUser])
                    {
                        //if (isEdit)
                        //{
                        //    isApprove = false;
                        //    //isApprove = oldIsApproved;
                        //}
                        //else
                        isApprove = false;
                    }
                }
                else
                {
                    if (forumSetting.CreateThreadNeedApprove[operatorUser])
                    {
                        isApprove = false;
                    }
                }

                if (isApprove)
                {
                    if (keywords.ApprovedKeywords.IsMatch(subject))
                    {
                        isApprove = false;
                    }
                    if (isApprove)
                    {
                        if (keywords.ApprovedKeywords.IsMatch(content))
                        {
                            isApprove = false;
                        }
                    }
                }

            }

            content = PostUbbParserV5.ParseWhenSave(operatorUser.UserID, enableEmoticons, forumID, content, enableHtml, enableMaxCode3, attachments);

            return true;
        }

        private bool validateCreateThread(
            AuthUser operatorUser, int forumID, int price, int threadCatalogID, Forum forum, ForumSettingItem forumSetting, ForumPermissionSetNode forumPermission, bool ignorePermission, ref bool isApprove)
        {
            if (forum.ParentID == 0)
            {
                ThrowError<CategoryForumCannotCreateThreadError>(new CategoryForumCannotCreateThreadError(""));
                return false;
            }

            if (!ignorePermission)
            {
                //发贴时间间隔
                if (forumSetting.CreatePostIntervals[operatorUser] > 0)
                {
                    TimeSpan PostInterval = TimeSpan.FromSeconds(forumSetting.CreatePostIntervals[operatorUser]);
                    if (DateTimeUtil.Now - operatorUser.LastPostDate < PostInterval)
                    {
                        ThrowError<OverCreatePostIntervalsError>(new OverCreatePostIntervalsError("PostInterval", PostInterval, operatorUser.LastPostDate));
                        return false;
                    }
                }

            }

            return validateUpdateThread(operatorUser, forumID, price, threadCatalogID, forum, forumSetting, forumPermission, ignorePermission);

        }

        private bool validateUpdateThread(
            AuthUser operatorUser, int forumID, int price, int threadCatalogID, Forum forum, ForumSettingItem forumSetting, ForumPermissionSetNode forumPermission, bool ignorePermission)
        {
            if (price != 0)
            {
                if (!ignorePermission)
                {
                    if (price > 0 && forumSetting.EnableSellAttachment[operatorUser] == false)//forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.SellThread) == false)
                    {
                        ThrowError<CannotSellThreadError>(new CannotSellThreadError("price"));
                        return false;
                    }
                }

                int? maxPoint, minRemain;
                int minPoint;

                ForumPointAction.Instance.GetUserPoint(operatorUser.UserID, ForumPointValueType.SellThread, forumID, out minRemain, out minPoint, out maxPoint);

                if (maxPoint != null && price > maxPoint.Value)
                {
                    Context.ThrowError<OverMaxSellThreadPoint>(new OverMaxSellThreadPoint("OverMaxSellThreadPoint", price, maxPoint.Value));
                    return false;
                }
                if (price < minPoint)
                {
                    Context.ThrowError<OverMinSellThreadPoint>(new OverMinSellThreadPoint("OverMinSellThreadPoint", price, minPoint));
                    return false;
                }
            }

            if (forum.ThreadCatalogStatus == ThreadCatalogStatus.EnableAndMust && threadCatalogID == 0)
            {
                ThrowError<NotSellectThreadCatalogError>(new NotSellectThreadCatalogError("threadCatalogID"));
                return false;
            }

            if (threadCatalogID != 0)
            {
                if (null == ForumBO.Instance.GetForumThreadCatalog(forumID, threadCatalogID))
                {
                    ThrowError<ForumThreadCatalogNotExistsError>(new ForumThreadCatalogNotExistsError("threadCatalogID", threadCatalogID));
                    return false;
                }
            }

            return true;
        }

        private bool ProcessAttachments(AuthUser operatorUser, int forumID, int? postID, ForumSettingItem forumSetting, string content
            , AttachmentCollection attachments, bool editMode, bool isThread
            , out List<int> historyAttachmentIDs, out TempUploadFileCollection tempUploadFiles)
        {
            historyAttachmentIDs = null;

            tempUploadFiles = null;

            long maxSingleFileSize = forumSetting.MaxSignleAttachmentSize[operatorUser];
            if (maxSingleFileSize == 0)
                maxSingleFileSize = long.MaxValue;

            ExtensionList allowExtnames = forumSetting.AllowFileExtensions[operatorUser];

            //处理插入的历史附件
            List<int> tempHistoryAttachmentIDs = new List<int>();
            MatchCollection ms = PostUbbParserV5.regex_AllAttach.Matches(content);

            foreach (Match m in ms)
            {
                bool isHistoryAttachment = true;
                int attachID = int.Parse(m.Groups["id"].Value);
                if (attachments != null)
                {
                    foreach (Attachment attach in attachments)
                    {
                        if (attachID == attach.AttachmentID)
                        {
                            isHistoryAttachment = false;
                            break;
                        }
                    }
                }
                if (isHistoryAttachment)
                    tempHistoryAttachmentIDs.Add(attachID);
            }

            AttachmentCollection historyAttachments = GetAttachments(operatorUser.UserID, tempHistoryAttachmentIDs);

            //最多允许插入 20个历史附件
            if (tempHistoryAttachmentIDs.Count > 20)
            {
                ThrowError(new InvalidHistoryAttachmentCountError("historyAttachment", 20, tempHistoryAttachmentIDs.Count));
                return false;
            }

            List<int> realHistoryAttachmentIDs = new List<int>();
            foreach (Attachment attach in historyAttachments)
            {
                realHistoryAttachmentIDs.Add(attach.AttachmentID);

                if (allowExtnames.Contains(attach.FileType) == false)
                {
                    ThrowError(new InvalidAttachmentFileTypeError("attachmentFileType",allowExtnames,attach.FileType));
                    return false;
                }

                if (attach.FileSize > maxSingleFileSize)
                {
                    ThrowError(new InvalidAttachmentFileSizeError("attachmentFileSize", attach.FileName, maxSingleFileSize, attach.FileSize));
                    return false;
                }
            }

            historyAttachmentIDs = realHistoryAttachmentIDs;



            if (attachments == null || attachments.Count == 0)
            {
                return true;
            }


            List<int> tempUploadFileIds = new List<int>();

            if (forumSetting.AllowAttachment[operatorUser] == false)
            {
                ThrowError(new NoPermissionUploadAttachmentError(operatorUser.UserID));
                return false;
            }

            AttachmentCollection todayAttachments = GetUserTodayAttachments(operatorUser.UserID);


            //今天已经上传过的附件个数 如果是编辑不包括被编辑帖子的附件
            int usedAttachmentCount = 0;
            //今天已经上传过的附件大小 如果是编辑不包括被编辑帖子的附件
            long usedSize = 0;

            //当前帖子 今天上传的附件
            List<int> postAttachmentIDs = new List<int>();
            foreach (Attachment attachment in todayAttachments)
            {
                if (editMode && attachment.PostID == postID.Value)
                {
                    postAttachmentIDs.Add(attachment.AttachmentID);
                    continue;
                }
                usedAttachmentCount++;
                usedSize += attachment.FileSize;
            }

            //今天还允许上传的个数
            int todayAllowCount = AllSettings.Current.BbsSettings.MaxAttachmentCountInDay[operatorUser];
            if (todayAllowCount == 0)
                todayAllowCount = int.MaxValue;
            else
                todayAllowCount = todayAllowCount - usedAttachmentCount;


            //今天还允许上传的总大小
            long todayAllowTotalSize = AllSettings.Current.BbsSettings.MaxTotalAttachmentsSizeInDay[operatorUser];

            if (todayAllowTotalSize == 0)
                todayAllowTotalSize = long.MaxValue;
            else
                todayAllowTotalSize = todayAllowTotalSize - usedSize;

            int? maxPointValue, minRemainPoint;
            int minPointValue;

            ForumPointAction.Instance.GetUserPoint(operatorUser.UserID, ForumPointValueType.SellAttachment, forumID, out minRemainPoint, out minPointValue, out maxPointValue);

            long totalFileSize = 0;
            int attachCount = 0;
            bool canSellAttachment = forumSetting.EnableSellAttachment[operatorUser];

            List<int> diskFileIDs = null;

            InvalidFileNameRegex regex = new InvalidFileNameRegex();
            foreach (Attachment attach in attachments)
            {
                if (regex.IsMatch(attach.FileName))
                {
                    Context.ThrowError<InvalidAttachmentNameError>(new InvalidAttachmentNameError("InvalidAttachmentNameError"));
                    return false;
                }

                if (attach.Price != 0)
                {
                    if (canSellAttachment == false)
                    {
                        Context.ThrowError<CannotSellAttachError>(new CannotSellAttachError("CannotSellAttachError"));
                        return false;
                    }

                    if (maxPointValue != null && attach.Price > maxPointValue.Value)
                    {
                        Context.ThrowError<OverMaxSellAttachPoint>(new OverMaxSellAttachPoint("OverMaxSellAttachPoint", attach.Price, maxPointValue.Value));
                        return false;
                    }
                    if (attach.Price < minPointValue)
                    {
                        Context.ThrowError<OverMinSellAttachPoint>(new OverMinSellAttachPoint("OverMinSellAttachPoint", attach.Price, minPointValue));
                        return false;
                    }
                }

                //ID小于0的暂时先收集，留待下次处理。经处理后，附件ID不可能小于0，等于0的表示新建，大于0的表示原有附件
                if (attach.AttachmentID < 0)
                {

                    int tempUploadFileID = 0 - attach.AttachmentID;
                    tempUploadFileIds.Add(tempUploadFileID);

                    if (allowExtnames.Contains(attach.FileType) == false)
                    {
                        ThrowError(new InvalidAttachmentFileTypeError("attachmentFileType", allowExtnames, attach.FileType));
                        return false;
                    }
                    attachCount++;
                }
                //ID大于0，表示原有的附件
                else if (attach.AttachmentID > 0)
                {
                    //编辑模式才可能有原有附件
                    if (editMode == false)
                    {
                        ThrowError(new InvalidAttachmentError("InvalidAttachmentError"));
                        return false;
                    }

                    //编辑的时候  这个帖子原来发的附件 如果是今天发的  才加1
                    if (postAttachmentIDs.Contains(attach.AttachmentID))
                    {
                        totalFileSize += attach.FileSize;
                        attachCount++;
                    }

                }
                else // ID=0 是网络硬盘插入的附件
                {
                    totalFileSize += attach.FileSize;

                    if (string.IsNullOrEmpty(attach.FileName))
                    {
                        if (diskFileIDs == null)
                            diskFileIDs = new List<int>();
                        diskFileIDs.Add(attach.DiskFileID);
                    }

                    attachCount++;
                }


            }

            if (attachCount > todayAllowCount)
            {
                ThrowError(new OverTodayAlowAttachmentCountError(todayAllowCount + usedAttachmentCount, todayAllowCount, attachCount));
                return false;
            }

            if (totalFileSize > todayAllowTotalSize)
            {
                ThrowError(new OverTodayAlowAttachmentFileSizeError("todayAllowTotalSize", todayAllowTotalSize + usedSize, todayAllowTotalSize, totalFileSize));
                return false;
            }
            if (isThread)
            {
                int count = forumSetting.MaxTopicAttachmentCount[operatorUser];
                if (count == 0)
                    count = int.MaxValue;
                if (attachCount > count)
                {
                    ThrowError(new OverMaxTopicAttachmentCountError(forumID, count));
                    return false;
                }
            }
            else
            {
                int count = forumSetting.MaxPostAttachmentCount[operatorUser];
                if (count == 0)
                    count = int.MaxValue;

                if (attachCount > count)
                {
                    ThrowError(new OverMaxPostAttachmentCountError(forumID, count));
                    return false;
                }
            }



            //然后才开始循环处理临时文件
            tempUploadFiles = FileManager.GetUserTempUploadFiles(operatorUser.UserID, tempUploadFileIds);

            //然后才开始循环处理临时文件
            //PhysicalFileFromTempCollection physicalFiles = saver.PreSave(userID, tempUploadFileIds);


            foreach (TempUploadFile file in tempUploadFiles)
            {

                if (file.FileSize > maxSingleFileSize)
                {
                    ThrowError(new InvalidAttachmentFileSizeError("attachmentFileSize", file.FileName, maxSingleFileSize, file.FileSize));
                    return false;
                }

                int attachID = 0 - file.TempUploadFileID;

                Attachment attach = attachments.GetValue(attachID);  //BuildAttachmentFromForm(attachID, post);

                //attach.AttachmentID = attachID;
                attach.FileID = file.FileID;
                attach.FileSize = file.FileSize;
                totalFileSize += file.FileSize;

                if (string.IsNullOrEmpty(attach.FileName))
                    attach.FileName = file.FileName;

            }

            if (totalFileSize > todayAllowTotalSize)
            {
                ThrowError(new OverTodayAlowAttachmentFileSizeError("todayAllowTotalSize", todayAllowTotalSize + usedAttachmentCount, todayAllowTotalSize, totalFileSize));
                return false;
            }



            if (diskFileIDs != null)
            {
                DiskFileCollection diskFiles = DiskBO.Instance.GetDiskFiles(diskFileIDs);
                foreach (Attachment temp in attachments)
                {
                    if (temp.DiskFileID > 0 && string.IsNullOrEmpty(temp.FileName))
                    {
                        DiskFile diskFile = diskFiles.GetValue(temp.DiskFileID);
                        if (diskFile != null)
                        {
                            temp.FileName = diskFile.FileName;
                        }
                    }
                }
            }


            return true;
        }



        private bool ProcessPostContentAttachTag(int postID, int forumID, int userID, AttachmentCollection attachments, string content, List<int> attachmentIDs, Dictionary<string, int> fileIDs)
        {
            if (attachments == null || attachments.Count == 0)
                return true;

            if (attachmentIDs.Count == 0 && fileIDs.Count == 0)//如果没有新加的附件 就不需要处理 [local]标签
                return true;

            //数据库中出来是倒序的  现在倒回来按顺序
            attachmentIDs.Reverse();

            content = PostUbbParserV5.ParseLocalAttachTag(content, attachments, attachmentIDs, fileIDs);

            return PostDaoV5.Instance.UpdatePostContent(postID, content);
        }



        #endregion

        #region  审核

        ///// <summary>
        ///// 获得所有版块的未审核的帖子
        ///// </summary>
        ///// <returns></returns>
        //public ThreadCollectionV5 GetAllUnapprovedThreads(int pageIndex, int pageSize, bool ignorePermission, out int totalThreads)
        //{
        //    totalThreads = 0;
        //    ForumV5 forum;
        //    if (ignorePermission)
        //        forum = ForumBOV5.Instance.GetForum((int)SystemForum.UnapproveThreads);
        //    else
        //        forum = ForumBOV5.Instance.GetForum((int)SystemForum.UnapproveThreads);
        //    if (forum == null)
        //        return new ThreadCollectionV5();

        //    //totalThreads = forum.TotalThreads;

        //    ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreadsByStatus(ThreadStatus.UnApproved, 0, pageIndex+1, pageSize, true);

        //    ProcessKeyword(threads, ProcessKeywordMode.FillOriginalText);
        //    return threads;
        //}

        /// <summary>
        /// 根据版块ID获得未审核的帖子
        /// </summary>
        /// <returns></returns>
        public ThreadCollectionV5 GetThreadsByStatus(ThreadStatus threadStatus, int? forumID, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = -1;
            ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreadsByStatus(threadStatus, forumID, sortType, beginDate, endDate, isDesc, pageNumber, pageSize, ref totalCount);
            //ProcessKeyword(threads, ProcessKeywordMode.FillOriginalText);
            threads = ProcessMovedThreads(threads);
            return threads;
        }

        /// <summary>
        /// 未审核回复的主题
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalThreads"></param>
        /// <returns></returns>
        public ThreadCollectionV5 GetUnapprovedPostThreads(int forumID, int pageNumber, int pageSize)
        {
            ThreadCollectionV5 threads = PostDaoV5.Instance.GetUnapprovedPostThreads(forumID, null, pageNumber, pageSize);
            //ProcessKeyword(threads, ProcessKeywordMode.FillOriginalText);
            return threads;
        }

        public ThreadCollectionV5 GetAllUnapprovedPostThreads(int pageNumber, int pageSize)
        {
            ThreadCollectionV5 threads = PostDaoV5.Instance.GetUnapprovedPostThreads(null, null, pageNumber, pageSize);
            //ProcessKeyword(threads, ProcessKeywordMode.FillOriginalText);
            return threads;
        }

        public ThreadCollectionV5 GetMyUnapprovedPostThreads(int userID, int pageNumber, int pageSize)
        {
            return PostDaoV5.Instance.GetUnapprovedPostThreads(null, userID, pageNumber, pageSize);
        }

        public ThreadCollectionV5 GetMyThreads(int userID, bool isApproved, int pageNumber, int pageSize, out int totalThreads)
        {
            ThreadCollectionV5 stickThreads = GetUserStickThreads(userID);

            ThreadCollectionV5 threads = PostDaoV5.Instance.GetMyThreads(userID, isApproved, pageNumber, pageSize, stickThreads.Count, out totalThreads);

            totalThreads += stickThreads.Count;
            stickThreads.AddRange(threads);

            return stickThreads;
        }

        private ThreadCollectionV5 GetUserStickThreads(int userID)
        {
            ThreadCollectionV5 threads = new ThreadCollectionV5();
            ThreadCollectionV5 gloablThreads = GetGlobalThreads();
            foreach(BasicThread thread in gloablThreads)
            {
                if(thread.PostUserID == userID)
                {
                    threads.Add(thread);
                }
            }
            ForumCollection forums = ForumBO.Instance.GetAllForums();
            List<int> tempForumIDs = new List<int>();
            foreach(Forum forum in forums)
            {
                if (forum.ForumID > 0 && forum.ParentID > 0)
                {
                    tempForumIDs.Add(forum.ForumID);
                }
            }

            ThreadCollectionV5 stickThreads = GetStickThreads(tempForumIDs);
            foreach (BasicThread thread in stickThreads)
            {
                if (thread.PostUserID == userID)
                {
                    threads.Add(thread);
                }
            }

            return threads;
        }

        public ThreadCollectionV5 GetMyParticipantThreads(int userID, int pageNumber, int pageSize, out int totalThreads)
        {
            return PostDaoV5.Instance.GetMyParticipantThreads(userID, pageNumber, pageSize, out totalThreads);
        }

        public void GetUnapprovedPostThread(int threadID, int? userID, int pageNumber, int pageSize, out BasicThread thread, out PostCollectionV5 posts, out int totalCount)
        {
            PostDaoV5.Instance.GetUnapprovedPostThread(threadID, userID, pageNumber, pageSize, out thread, out posts, out totalCount);

            //if (posts.Count > 0)
            //    thread.ThreadContent = posts[0];
        }

        #endregion

        #region   附件

        /// <summary>
        /// 获取用户今天上传的附件
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public AttachmentCollection GetUserTodayAttachments(int userID)
        {
            return PostDaoV5.Instance.GetUserTodayAttachments(userID);
        }

        public void GetUserTodayAttachmentInfo(int userID, int? excludePostID, out int totalCount, out long totalFileSize)
        {
            PostDaoV5.Instance.GetUserTodayAttachmentInfo(userID, excludePostID, out totalCount, out totalFileSize);
        }

        public AttachmentCollection GetAttachments(int userID, IEnumerable<int> attachmentIDs)
        {
            if (ValidateUtil.HasItems<int>(attachmentIDs) == false)
                return new AttachmentCollection();

            return  PostDaoV5.Instance.GetAttachments(userID, attachmentIDs);
        }

        public AttachmentCollection GetAttachments(int operatorUserID, int? year, int? month, int? day, string keyword, int pageNumber, int pageSize, ExtensionList fileTypes)
        {
            DateTime? beginDate = null, endDate = null;

            if (year != null && month != null && day != null)
            {
                beginDate = new DateTime(year.Value, month.Value, day.Value);
                endDate = new DateTime(year.Value, month.Value, day.Value, 23, 59, 59);
            }
            else if (year != null && month != null && day == null)
            {
                beginDate = new DateTime(year.Value, month.Value, 1);
                endDate = beginDate.Value.AddMonths(1).AddSeconds(-1);
            }
            else if (year != null && month == null && day == null)
            {
                beginDate = new DateTime(year.Value, 1, 1);
                endDate = beginDate.Value.AddYears(1).AddSeconds(-1);
            }

            return PostDaoV5.Instance.GetAttachments(operatorUserID, beginDate, endDate, keyword, pageNumber, pageSize, fileTypes);
        }

        public AttachmentCollection GetAttachments(int postID)
        {
            return PostDaoV5.Instance.GetAttachments(postID);
        }

        /// <summary>
        /// 通过AttachmentID获取附件
        /// </summary>
        /// <param name="AttachmentID"></param>
        /// <returns></returns>
        public Attachment GetAttachment(int attachmentID, bool updateTotalDownloads)
        {
            Attachment attachment;
            int threadID;
            //updateTotalDownloads 为true时 threadID 才有值
            attachment = PostDaoV5.Instance.GetAttachment(attachmentID, updateTotalDownloads, out threadID);

            if (attachment != null && updateTotalDownloads && threadID > 0)
            {
                PostV5 post = ThreadCachePool.GetPost(threadID, attachment.PostID);
                if (post != null)
                {
                    if (post.Attachments != null)
                    {
                        Attachment temp = post.Attachments.GetValue(attachmentID);
                        if (temp != null)
                        {
                            temp.TotalDownloadUsers = attachment.TotalDownloadUsers;
                            temp.TotalDownloads = attachment.TotalDownloads;
                        }
                    }
                }
            }

            return attachment;
        }

        public Attachment GetAttachment(int attachmentID)
        {
            return GetAttachment(attachmentID, false);
        }

        public void GetAttachment(int diskFileID, int postID, out Attachment attachment, out PostV5 post, out BasicThread thread)
        {
            PostDaoV5.Instance.GetAttachment(diskFileID, postID, out attachment, out post, out thread);
        }

        public bool CreateAttachmentExchange(int attachmentID, int userID, int price)
        {
            return PostDaoV5.Instance.CreateAttachmentExchange(attachmentID, userID, price);
        }

        public void SetAttachmentBuyedInCache(AuthUser operatorUser, int attachmentID, bool isBuyed)
        {
            //如果缓存了太多，清理超出部分
            if (operatorUser.BuyedAttachments.Count >= 30)
            {
                int deletedAttachmentID = 0;
                //循环一次就退出，因为只删除最顶部的一个
                foreach (int key in operatorUser.BuyedAttachments.Keys)
                {
                    deletedAttachmentID = key;
                    break;
                }

                if (deletedAttachmentID != 0)
                    operatorUser.BuyedAttachments.Remove(deletedAttachmentID);
            }

            if (!operatorUser.BuyedAttachments.ContainsKey(attachmentID))
                operatorUser.BuyedAttachments.Add(attachmentID, isBuyed);
            else
                operatorUser.BuyedAttachments[attachmentID] = isBuyed;
        }

        public bool IsBuyedAttachment(AuthUser operatorUser, Attachment attachment)
        {
            //价格为0的始终返回true
            if (attachment.Price < 1)
                return true;

            //游客始终返回false
            else if (operatorUser == User.Guest)
                return false;

            //作者始终返回true
            else if (operatorUser.UserID == attachment.UserID)
                return true;

            int attachmentID = attachment.AttachmentID;

            bool isBuyed;
            if (operatorUser.BuyedAttachments.TryGetValue(attachmentID, out isBuyed))
                return isBuyed;

            isBuyed = PostDaoV5.Instance.IsBuyedAttachment(operatorUser.UserID, attachmentID);

            SetAttachmentBuyedInCache(operatorUser, attachmentID, isBuyed);

            return isBuyed;
        }

        /// <summary>
        /// 获取购买记录
        /// </summary>
        /// <param name="attachmentID"></param>
        /// <returns></returns>
        public AttachmentExchangeCollection GetAttachmentExchanges(int attachmentID,int pageNumber,int pageSize, out int totalCount,out int totalSellMoney)
        {
            return PostDaoV5.Instance.GetAttachmentExchanges(attachmentID, pageNumber, pageSize, out totalCount, out totalSellMoney);
        }

        /// <summary>
        /// 获取购买记录
        /// </summary>
        /// <param name="attachmentID"></param>
        /// <returns></returns>
        public ThreadExchangeCollection GetThreadExchanges(int threadID, int pageNumber, int pageSize, out int totalCount, out int totalSellMoney)
        {
            return PostDaoV5.Instance.GetThreadExchanges(threadID, pageNumber, pageSize, out totalCount, out totalSellMoney);
        }


        public AttachmentCollection GetAttachments(AuthUser operatorUser, AttachmentFilter filter, int pageNumber, out int totalCount)
        {

            totalCount = 0;

            AttachmentFilter tempFilter = ProcessAttachmentFilter(filter);
            if (tempFilter == null)
                return new AttachmentCollection();

            if (pageNumber < 1)
                pageNumber = 1;

            string cacheKey = string.Format(cacheKey_List_Attachment_Search_Count, tempFilter.ToString());
            bool haveTotalCountCache = false;
            if (CacheUtil.TryGetValue<int>(cacheKey, out totalCount))
            {
                haveTotalCountCache = true;
            }
            else
                totalCount = -1;


            Guid[] excludeRoleIDs = null;

            if (filter.ForumID != null && filter.ForumID.Value != 0)
            {
                Forum forum = ForumBO.Instance.GetForum(filter.ForumID.Value, false);
                if (forum == null)
                    return new AttachmentCollection();

                excludeRoleIDs = forum.ManagePermission.GetNoPermissionTargetRoleIds(operatorUser, PermissionTargetType.Content);
            }
            else
            {
                if (operatorUser.IsOwner == false)
                {
                    //TODO:
                    //您不是创始人  没有权限搜索所有版块的主题
                    return new AttachmentCollection();
                }
            }

            AttachmentCollection attachments = PostDaoV5.Instance.GetAttachments(pageNumber, tempFilter, excludeRoleIDs, ref totalCount);


            if (!haveTotalCountCache)
            {
                CacheUtil.Set<int>(cacheKey, totalCount, CacheTime.Normal, CacheExpiresType.Sliding);
            }

            return attachments;
        }

        public bool DeleteSearchAttachments(AuthUser operatorUser, AttachmentFilter filter, int deleteTopCount, out int deletedCount)
        {
            deletedCount = 0;

            AttachmentFilter tempFilter = ProcessAttachmentFilter(filter);
            if (tempFilter == null)
                return true;

            Guid[] excludeRoleIDs = null;

            Forum forum = null;

            if (filter.ForumID != null && filter.ForumID.Value != 0)
            {
                forum = ForumBO.Instance.GetForum(filter.ForumID.Value);
                if (forum == null)
                    return false;

                if (false == forum.ManagePermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads))
                    return false;

                excludeRoleIDs = forum.ManagePermission.GetNoPermissionTargetRoleIds(operatorUser.UserID, PermissionTargetType.Content);
            }
            else
            {
                if (operatorUser.IsOwner == false)
                {
                    //TODO:
                    //您不是创始人  没有权限删除所有版块的主题
                    return false;
                }
            }


            List<int> threadIDs;

            PostDaoV5.Instance.DeleteSearchAttachments(tempFilter, excludeRoleIDs, deleteTopCount, out deletedCount, out threadIDs);

            CacheUtil.RemoveBySearch("Attachment/List/");

            ThreadCachePool.ClearThreadPostCache(threadIDs);

            return true;
        }

        private AttachmentFilter ProcessAttachmentFilter(AttachmentFilter filter)
        {
            if (filter == null)
                return null;

            AttachmentFilter tempFilter = (AttachmentFilter)filter.Clone();

            User user = null;
            if (!string.IsNullOrEmpty(tempFilter.Username))
            {
                user = UserBO.Instance.GetUser(tempFilter.Username);
                if (user == null)
                    return null;
            }
            if (user != null)
            {
                if (tempFilter.UserID != null && tempFilter.UserID.Value != user.UserID)
                    return null;
                else
                    tempFilter.UserID = user.UserID;
            }

            return tempFilter;
        }

        public bool DeleteAttachments(AuthUser operatorUser, int forumID, IEnumerable<int> attachmentIDs)
        {
            Guid[] excludeRoleIDs = null;
            if (forumID == 0)
            {
                if (operatorUser.IsOwner == false)
                {
                    //TODO:
                    //您不是创始人  没有权限删除所有版块的附件
                    return false;
                }
            }
            else
            {
                Forum forum = ForumBO.Instance.GetForum(forumID);
                if (forum == null)
                    return false;

                if (false == forum.ManagePermission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads))
                    return false;

                excludeRoleIDs = forum.ManagePermission.GetNoPermissionTargetRoleIds(operatorUser, PermissionTargetType.Content);
            }

            List<int> threadIDs;
            //List<string> fileIDs;

            PostDaoV5.Instance.DeleteAttachments(forumID, attachmentIDs, excludeRoleIDs, out threadIDs);

            CacheUtil.RemoveBySearch("Attachment/List/");

            ThreadCachePool.ClearThreadPostCache(threadIDs);

            List<int> tempAttachmentIDs = new List<int>(attachmentIDs);


            Logs.LogManager.LogOperation(
                    new Topic_DeleteAttachmentByIDs(operatorUser.UserID, operatorUser.Name, operatorUser.LastVisitIP, tempAttachmentIDs)
                );

            return true;
        }

        #endregion

        /// <summary>
        /// 获取主题的所有回复用户包括主题用户
        /// </summary>
        /// <param name="threadIDs"></param>
        /// <returns></returns>
        public List<int> GetPostUserIDsFormThreads(IEnumerable<int> threadIDs)
        {
            if (ValidateUtil.HasItems<int>(threadIDs) == false)
                return new List<int>();

            return PostDaoV5.Instance.GetPostUserIDsFormThreads(threadIDs);
        }

        /// <summary>
        /// 帖子的公开内容  用于分享 动态等 （如果是 隐藏内容，或需要购买的帖子 不能直接显示内容）
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public string GetPublicContent(BasicThread thread, PostV5 post)
        {
            if (thread.Price > 0)
                return "[******需要购买后才能查看******]";

            if (AllSettings.Current.ForumSettings.Items.GetForumSettingItem(thread.ForumID).EnableHiddenTag)
                return MaxLabs.bbsMax.Ubb.PostUbbParserV5.regex_hide.Replace(post.Content, "[******隐藏内容******]");
            else
                return post.Content;
        }

        public string GetActionName(ModeratorCenterAction action, BasicThread thread)
        {
            switch (action)
            {
                case ModeratorCenterAction.ApprovePost:
                    return "审核回复";
                case ModeratorCenterAction.ApprovePostByThreadIDs:
                    return "审核回复";
                case ModeratorCenterAction.CheckThread:
                    return "审核主题";
                case ModeratorCenterAction.CopyThread:
                    return "复制主题";
                case ModeratorCenterAction.DeletePost:
                    return "删除回复";
                case ModeratorCenterAction.DeleteUnapprovedPostByThreadIDs:
                    return "删除回复";
                case ModeratorCenterAction.DeleteUnapprovedPost:
                    return "删除回复";
                case ModeratorCenterAction.DeletePostSelf:
                    return "删除自己回复";
                case ModeratorCenterAction.DeleteOwnThread:
                    return "删除自己主题";
                case ModeratorCenterAction.DeleteThread:
                    return "删除主题";
                case ModeratorCenterAction.JoinThread:
                    return "合并主题";
                case ModeratorCenterAction.LockThread:
                    return "锁定主题";
                case ModeratorCenterAction.MoveThread:
                    return "移动主题";
                case ModeratorCenterAction.RecycleOwnThread:
                    return "回收自己主题";
                case ModeratorCenterAction.RecycleThread:
                    return "回收主题";
                case ModeratorCenterAction.RescindShieldPost:
                    return "解除单帖屏蔽";
                case ModeratorCenterAction.RescindShieldUser:
                    return "解除屏蔽用户";
                case ModeratorCenterAction.RevertThread:
                    return "还原主题";
                case ModeratorCenterAction.SetThreadElite:
                    return "设置精华";
                case ModeratorCenterAction.UpdateThreadCatalog:
                    return "设置分类";
                case ModeratorCenterAction.SetThreadIsTop:
                    return "设置置顶";
                case ModeratorCenterAction.SetThreadSubjectStyle:
                    return "标题样式";
                case ModeratorCenterAction.ShieldPost:
                    return "单帖屏蔽";
                case ModeratorCenterAction.ShieldUser:
                    return "屏蔽用户";
                case ModeratorCenterAction.SplitThread:
                    return "分割主题";
                case ModeratorCenterAction.UnlockThread:
                    return "解锁主题";
                case ModeratorCenterAction.UpThread:
                    return "提升主题";
                case ModeratorCenterAction.JudgementThread:
                    return "鉴定主题";
                case ModeratorCenterAction.UnJudgementTread:
                    return "解除鉴定";
                case ModeratorCenterAction.CanacelRate:
                    return "撤消帖子评分";
                case ModeratorCenterAction.CancelThreadSubjectStyle:
                    return "取消标题样式";
                case ModeratorCenterAction.CancelTop:
                    return "取消置顶";
                case ModeratorCenterAction.CancelValued:
                    return "取消精华";
                case ModeratorCenterAction.SetThreadStickByUseProp:
                    return "使用道具设置置顶";
                case ModeratorCenterAction.SetThreadSubjectStyleByUseProp:
                    return "使用道具设置标题样式";
                case ModeratorCenterAction.SetThreadLockByUseProp:
                    return "使用道具锁定主题";
                case ModeratorCenterAction.SetThreadNotUpdateSortOrder:
                    if (thread != null)
                    {
                        ManageForumPermissionSetNode permission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(thread.ForumID);
                        if (false == permission.Can(User.Current, ManageForumPermissionSetNode.ActionWithTarget.SetThreadNotUpdateSortOrder, thread.PostUserID))
                        {
                            return "其他操作";
                        }
                    }
                    return "自动沉帖";
                default:
                    return "其他操作";
            }
        }

        /// <summary>
        /// 获取 主题类型  如果不是特殊类型主题（投票 问题 辩论） 返回null
        /// </summary>
        /// <param name="threadType"></param>
        /// <returns></returns>
        private ThreadType? GetThreadType(ThreadType threadType)
        {
            if (threadType == ThreadType.Join || threadType == ThreadType.Move || threadType == ThreadType.Redirect || threadType == ThreadType.Normal)
                return null;
            else
                return threadType;
        }

        /// <summary>
        /// 用于 主题页面URL地址 如: "..../poll-2-1";
        /// </summary>
        public string GetThreadTypeString(ThreadType threadType)
        {
            ThreadType? tempThreadType = GetThreadType(threadType);
            if (tempThreadType == null)
                return "thread";
            else
                return tempThreadType.Value.ToString().ToLower();
        }
        private  void DeleteTopicStatus(IEnumerable<int> threadIDs, TopicStatuType type)
        {
            PostDaoV5.Instance.DeleteTopicStatus(threadIDs, type);
        }

        private  void CreateTopicStatus(IEnumerable<int> threadIDs, TopicStatuType type, DateTime endDate)
        {
            PostDaoV5.Instance.CreateTopicStatus(threadIDs, type, endDate);
        }

        private void CreateThreadManageLogs(int forumID, IEnumerable<int> threadIDs, ModeratorCenterAction action, DateTime? endDate)
        {
            //ThreadManageLog threadManageLog = new ThreadManageLog();
            //threadManageLog.ActionType = action;

            ThreadCollectionV5 threads = GetThreads(threadIDs);

            //threadManageLog.ThreadSubjects = new List<string>();
            //threadManageLog.PostUserIDs = new List<int>();

            //foreach (BasicThreadV5 thread in threads)
            //{
            //    threadManageLog.ThreadSubjects.Add(thread.Subject);
            //    threadManageLog.PostUserIDs.Add(thread.PostUserID);
            //}
            //threadManageLog.ForumID = forumID;
            //threadManageLog.IPAddress = IPUtil.GetCurrentIP();
            //threadManageLog.NickName = User.Current.Realname;
            //threadManageLog.ThreadIDs =new List<int>( threadIDs);
            //threadManageLog.UserID = User.Current.UserID;
            //threadManageLog.UserName = User.Current.Username;
            //threadManageLog.IsPublic = true;

            string reason = string.Empty;
            switch (action)
            {
                case ModeratorCenterAction.SetThreadStickByUseProp:
                    if (endDate == null)
                        reason = "使用道具对帖子进行了置顶操作";
                    else
                        reason = string.Format("使用道具对帖子进行了限时置顶操作，到期时间为：{0}", DateTimeUtil.FormatDateTime(endDate.Value));
                    break;

                case ModeratorCenterAction.SetThreadSubjectStyleByUseProp:
                    if (endDate == null)
                        reason = "使用道具对帖子进行了加亮操作";
                    else
                        reason = string.Format("使用道具对帖子进行了限时加亮操作，到期时间为：{0}", DateTimeUtil.FormatDateTime(endDate.Value));
                    break;

                case ModeratorCenterAction.SetThreadLockByUseProp:
                    if (endDate == null)
                        reason = "使用道具对帖子进行了锁定操作";
                    else
                        reason = string.Format("使用道具对帖子进行了限时锁定操作，到期时间为：{0}", DateTimeUtil.FormatDateTime(endDate.Value));
                    break;

                case ModeratorCenterAction.UpThreadByUseProp:
                    if (endDate == null)
                        reason = "使用道具对帖子进行了提升操作";
                    break;
                default: 
                    break;
            }

            WriteThreadOperateLog(User.Current, threads, action, reason);

            //LogManager.CreateThreadManageLog(threadManageLog);
        }


        private ManageForumPermissionSetNode GetForumPermissionSet(int forumID)
        {
            ManageForumPermissionSetNode permission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID);

            return permission;
        }

        #region PostIcons

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, PostIcon> GetAllPostIcons()
        {

            Dictionary<int, PostIcon> allPostIcons;
            string cacheKey = "AllPostIcons";

            if (PageCacheUtil.TryGetValue<Dictionary<int, PostIcon>>(cacheKey, out allPostIcons) == false)
            {
                allPostIcons = new Dictionary<int, PostIcon>();

                foreach (PostIcon icon in AllSettings.Current.PostIconSettings.SortedIcons)
                {
                    allPostIcons.Add(icon.IconID, icon);
                }

                PageCacheUtil.Set(cacheKey, allPostIcons);
            }

            return allPostIcons;
        }

        /// <summary>
        /// 获取单个
        /// </summary>
        /// <param name="FileID"></param>
        /// <returns></returns>
        public PostIcon GetPostIcon(int IconID)
        {
            PostIcon icon;
            if (GetAllPostIcons().TryGetValue(IconID, out icon))
                return icon;
            return null;
        }



        #endregion


        #region 杂项

        /// <summary>
        /// 对帖子的支持反对
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="postID"></param>
        /// <param name="isLove">是否支持</param>
        /// <param name="canSetMore">是否可以重复支持反对（一般情况下是false）</param>
        /// <returns></returns>
        public bool SetPostLoveHate(int userID, int postID, bool isLove, bool canSetMore)
        {
            if (userID == 0)
            {
                ThrowError<SetPostLoveHatePostGuestCannotSetError>(new SetPostLoveHatePostGuestCannotSetError("postID"));
                return false;
            }

            int threadID;
            int returnValue = PostDaoV5.Instance.SetPostLoveHate(userID, postID, isLove, canSetMore, out threadID);

            switch (returnValue)
            {
                case 1:
                    ThrowError<SetPostLoveHatePostNotExistsError>(new SetPostLoveHatePostNotExistsError("postID", postID));
                    return false;
                case 2:
                    ThrowError<SetPostLoveHatePostCannotSetMoreError>(new SetPostLoveHatePostCannotSetMoreError("postID", postID, isLove));
                    return false;
                default:
                    break;
            }

            PostV5 post = ThreadCachePool.GetPost(threadID, postID);
            if (post != null)
            {
                if (isLove)
                    post.LoveCount++;
                else
                    post.HateCount++;
            }
            return true;
        }

        /// <summary>
        /// 给帖子评级
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="threadID"></param>
        /// <param name="userID"></param>
        /// <param name="addRank"></param>
        /// <param name="ignorePermission">是否忽略权限(一般为false)</param>
        /// <returns></returns>
        public bool SetThreadRank(AuthUser operatorUser, int forumID, int threadID, int addRank, bool ignorePermission)
        {
            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);

            if (forumSetting.EnableThreadRank == false)
            {
                ThrowError(new SetThreadRankNotEnableError("SetThreadRankNotEnableError", forumID));
                return false;
            }

            if (!ignorePermission)
            {
                ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID);

                if (!permission.Can(operatorUser, ForumPermissionSetNode.Action.RankThread))
                {
                    ThrowError(new NopermissionSetThreadRankError());
                    return false;
                }
            }

            int newRank;
            int returnValue = PostDaoV5.Instance.SetThreadRank(threadID, operatorUser.UserID, addRank, out newRank);

            switch (returnValue)
            {
                case 1:
                    ThrowError(new ContnotSetOwnThreadRankError());
                    return false;
                default: break;
            }

            BasicThread thread = ThreadCachePool.GetThread(threadID);
            if (thread != null)
            {
                thread.Rank = newRank;
            }
            return true;
        }

        public ThreadRankCollection GetThreadRanks(int threadID, int pageNumber, int pageSize, out int totalCount)
        {
            return PostDaoV5.Instance.GetThreadRanks(threadID, pageNumber, pageSize, out totalCount);
        }

        /// <summary>
        /// 添加购买记录
        /// </summary>
        public bool CreateThreadExchange(int threadID, int userID, int price)
        {
            return PostDaoV5.Instance.CreateThreadExchange(threadID, userID, price);
        }


        public void SetThreadBuyedInCache(AuthUser operatorUser, int threadID, bool isBuyed)
        {
            //如果缓存了太多，清理超出部分
            if (operatorUser.BuyedThreads.Count >= 30)
            {
                int deletedThreadID = 0;
                //循环一次就退出，因为只删除最顶部的一个
                foreach (int key in operatorUser.BuyedThreads.Keys)
                {
                    deletedThreadID = key;
                    break;
                }

                if (deletedThreadID != 0)
                    operatorUser.BuyedThreads.Remove(deletedThreadID);
            }

            if (!operatorUser.BuyedThreads.ContainsKey(threadID))
                operatorUser.BuyedThreads.Add(threadID, isBuyed);
            else
                operatorUser.BuyedThreads[threadID] = isBuyed;
        }

        /// <summary>
        /// 是否购买过帖子
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="threadID"></param>
        /// <returns></returns>
        public bool IsBuyedThread(AuthUser operatorUser, BasicThread thread)
        {
            //价格为0的始终返回true
            if (thread.Price < 1)
                return true;

            //游客始终返回false
            else if (operatorUser == User.Guest)
                return false;

            //作者始终返回true
            else if (operatorUser.UserID == thread.PostUserID)
                return true;

            int threadID = thread.ThreadID;

            bool isBuyed;
            if (operatorUser.BuyedThreads.TryGetValue(threadID, out isBuyed))
                return isBuyed;

            isBuyed = PostDaoV5.Instance.IsBuyedThread(operatorUser.UserID, threadID);

            SetThreadBuyedInCache(operatorUser, threadID, isBuyed);

            return isBuyed;

        }

        public void SetThreadRepliedInCache(AuthUser operatorUser, int threadID, bool isReplied)
        {
            //如果缓存了太多，清理超出部分
            if (operatorUser.RepliedThreads.Count >= 30)
            {
                int deleteThreadID = 0;
                //循环一次就退出，因为只删除最顶部的一个
                foreach (int key in operatorUser.RepliedThreads.Keys)
                {
                    deleteThreadID = key;
                    break;
                }

                if (deleteThreadID != 0)
                    operatorUser.RepliedThreads.Remove(deleteThreadID);
            }

            if (!operatorUser.RepliedThreads.ContainsKey(threadID))
                operatorUser.RepliedThreads.Add(threadID, isReplied);
            else
                operatorUser.RepliedThreads[threadID] = isReplied;
        }

        public bool IsRepliedThread(AuthUser operatorUser, BasicThread thread)
        {
            if (operatorUser == User.Guest)
                return false;

            int threadID = thread.ThreadID;

            bool isReplied;
            if (operatorUser.RepliedThreads.TryGetValue(threadID, out isReplied))
                return isReplied;

            isReplied = PostDaoV5.Instance.IsRepliedThread(threadID, operatorUser.UserID);

            SetThreadRepliedInCache(operatorUser, threadID, isReplied);

            return isReplied;
        }

        public bool Vote(AuthUser operatorUser, IEnumerable<int> itemIDs, int threadID)
        {
            BasicThread thread = GetThread(threadID);
            if (thread == null)
            {
                ThrowError<ThreadNotExistsError>(new ThreadNotExistsError());
                return false;
            }


            if (BannedUserProvider.IsFullSiteBanned(operatorUser.UserID) || BannedUserProvider.IsBanned(operatorUser.UserID, thread.ForumID))
            {
                ThrowError<BannedUserCannotVoteError>(new BannedUserCannotVoteError());
                return false;
            }
            

            if (thread is PollThreadV5)
            { }
            else
            {
                ThrowError<IsNotPollCannotVoteError>(new IsNotPollCannotVoteError());
                return false;
            }
            PollThreadV5 poll = (PollThreadV5)thread;

            if (AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(poll.ForumID).Can(operatorUser, ForumPermissionSetNode.Action.Vote) == false)
            {
                ThrowError<NoPermissionVoteError>(new NoPermissionVoteError(operatorUser.UserID));
                return false;
            }

            if (poll.IsLocked)
            {
                ThrowError<VotePollIsLockedError>(new VotePollIsLockedError());
                return false;
            }

            if (poll.IsClosed)
            {
                ThrowError<VotePollIsClosedError>(new VotePollIsClosedError());
                return false;
            }

            List<int> realItemIDs = new List<int>();
            List<int> tempItemIDs = new List<int>(itemIDs);
            foreach(PollItem item in poll.PollItems)
            {
                if (tempItemIDs.Contains(item.ItemID))
                    realItemIDs.Add(item.ItemID);
            }

            if (realItemIDs.Count == 0)
            {
                ThrowError<VotePollNotSellectItemError>(new VotePollNotSellectItemError());
                return false;
            }


            if (realItemIDs.Count > poll.Multiple)
            {
                ThrowError<VotePollInvalidSellectedCountError>(new VotePollInvalidSellectedCountError(poll.Multiple, realItemIDs.Count));
                return false;
            }

            bool success = ForumPointAction.Instance.UpdateUserPoint(operatorUser.UserID, ForumPointType.Vote, 1, true, poll.ForumID, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    int returnValue = PostDaoV5.Instance.Vote(realItemIDs, threadID, operatorUser.UserID, operatorUser.Name, poll);
                    switch (returnValue)
                    {
                        case 1:
                            ThrowError<VotePollIsClosedError>(new VotePollIsClosedError());
                            return false;
                        case 2:
                            ThrowError<VotePollHadVotedError>(new VotePollHadVotedError());
                            return false;
                        case 3:
                            ThrowError<VotePollIsLockedError>(new VotePollIsLockedError());
                            return false;
                        default:
                            return true;
                    }
                }
                else
                    return false;
            });

            return success;
        }

        public PollItemDetailsCollectionV5 GetPollItemDetails(int threadID)
        {
            return PostDaoV5.Instance.GetPollItemDetails(threadID);
        }

        public bool Polemize(AuthUser operatorUser, int threadID, ViewPointType viewPointType)
        {
            if (operatorUser.UserID == 0)
            {
                ThrowError<GuestCannotPolemizeError>(new GuestCannotPolemizeError());
                return false;
            }

            BasicThread thread = GetThread(threadID);
            if (thread == null)
            {
                ThrowError<ThreadNotExistsError>(new ThreadNotExistsError());
                return false;
            }


            if (BannedUserProvider.IsFullSiteBanned(operatorUser.UserID) || BannedUserProvider.IsBanned(operatorUser.UserID, thread.ForumID))
            {
                ThrowError<BannedUserCannotPolemizeError>(new BannedUserCannotPolemizeError());
                return false;
            }

            if (thread is PolemizeThreadV5)
            { }
            else
            {
                ThrowError<IsNotPolemizeCannotPolemizeError>(new IsNotPolemizeCannotPolemizeError());
                return false;
            }
            PolemizeThreadV5 polemize = (PolemizeThreadV5)thread;

            ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(polemize.ForumID);

            if (permission.Can(operatorUser, ForumPermissionSetNode.Action.CanPolemize) == false || permission.Can(operatorUser, ForumPermissionSetNode.Action.ReplyThread) == false)
            {
                ThrowError<NoPermissionPolemizeError>(new NoPermissionPolemizeError(operatorUser.UserID));
                return false;
            }

            if (polemize.IsLocked)
            {
                ThrowError<PolemizeIsLockedError>(new PolemizeIsLockedError());
                return false;
            }

            if (polemize.IsClosed)
            {
                ThrowError<PolemizeIsClosedError>(new PolemizeIsClosedError());
                return false;
            }

            int returnValue = PostDaoV5.Instance.Polemize(threadID, operatorUser.UserID, viewPointType, polemize);
            switch (returnValue)
            {
                case 1:
                    ThrowError<PolemizeIsClosedError>(new PolemizeIsClosedError());
                    return false;
                case 2:
                    ThrowError<HadPolemizedError>(new HadPolemizedError(ViewPointType.Agree, viewPointType));
                    return false;
                case 3:
                    ThrowError<HadPolemizedError>(new HadPolemizedError(ViewPointType.Against, viewPointType));
                    return false;
                case 4:
                    ThrowError<HadPolemizedError>(new HadPolemizedError(ViewPointType.Neutral, viewPointType));
                    return false;
                default:
                    return true;
            }

        }

        public bool FinalQuestion(AuthUser operatorUser, int threadID, int bestPostID, Dictionary<int, int> rewards)
        {
            BasicThread thread = GetThread(threadID);
            if (thread == null)
            {
                ThrowError<ThreadNotExistsError>(new ThreadNotExistsError());
                return false;
            }


            if (BannedUserProvider.IsFullSiteBanned(operatorUser.UserID) || BannedUserProvider.IsBanned(operatorUser.UserID, thread.ForumID))
            {
                ThrowError<BannedUserCannotFinalQuestionError>(new BannedUserCannotFinalQuestionError());
                return false;
            }

            if (thread is QuestionThread)
            { }
            else
            {
                ThrowError<IsNotQuestionCannotFinalError>(new IsNotQuestionCannotFinalError());
                return false;
            }

            if (thread.IsClosed)
            {
                ThrowError<QuestionIsClosedError>(new QuestionIsClosedError());
                return false;
            }

            QuestionThread question = (QuestionThread)thread;

            ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(question.ForumID);

            if (operatorUser.UserID != question.PostUserID && !AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(question.ForumID).Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.FinalQuestion, thread.PostUserID))
            {
                ThrowError<NoPermissionFinalQuestionError>(new NoPermissionFinalQuestionError());
                return false;
            }

            if (rewards.Count > question.RewardCount)
            {
                ThrowError<OverQuestionMaxRewardCountError>(new OverQuestionMaxRewardCountError(question.RewardCount, rewards.Count));
                return false;
            }

            if (bestPostID == 0)
            {
                ThrowError<InvalidQuestionBestPostIDError>(new InvalidQuestionBestPostIDError());
                return false;
            }

            int maxReward = 0;
            int bestReward = 0;
            int totalReward = 0;
            foreach (KeyValuePair<int, int> reward in rewards)
            {
                if (reward.Value == 0)
                {
                    ThrowError<InvalidQuestionRewardValueError>(new InvalidQuestionRewardValueError());
                    return false;
                }
                if (reward.Value > maxReward)
                    maxReward = reward.Value;
                if (reward.Key == bestPostID)
                {
                    bestReward = reward.Value;
                }
                if (bestReward > 0)
                {
                    if (maxReward > bestReward)
                    {
                        ThrowError<InvalidQuestionBestRewardValueError>(new InvalidQuestionBestRewardValueError());
                        return false;
                    }
                }

                totalReward += reward.Value;
            }

            if (totalReward != question.Reward)
            {
                ThrowError<InvalidQuestionTotalRewardsError>(new InvalidQuestionTotalRewardsError(question.Reward, totalReward));
                return false;
            }

            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    int returnValue = PostDaoV5.Instance.FinalQuestion(threadID, bestPostID, rewards, question);

                    bool success;
                    switch (returnValue)
                    {
                        case 1:
                            ThrowError<IsNotQuestionCannotFinalError>(new IsNotQuestionCannotFinalError());
                            success = false;
                            break;
                        case 2:
                            ThrowError<QuestionIsClosedError>(new QuestionIsClosedError());
                            success = false;
                            break;
                        case 3:
                            ThrowError<QuestionBestPostNotExistsError>(new QuestionBestPostNotExistsError());
                            success = false;
                            break;
                        case 4:
                            ThrowError<InvalidQuestionTotalRewardsError>(new InvalidQuestionTotalRewardsError(question.Reward, totalReward));
                            success = false;
                            break;
                        case 5:
                            ThrowError<OverQuestionMaxRewardCountError>(new OverQuestionMaxRewardCountError(question.RewardCount, rewards.Count));
                            success = false;
                            break;
                        default:
                            success = true;
                            break;
                    }

                    if (success == false)
                    {
                        context.RollbackTransaction();
                        return false;
                    }

                    List<int> userIDs;// = new List<int>();
                    List<int> points = new List<int>();
                    List<int> postIDs = new List<int>();
                    foreach (KeyValuePair<int, int> pair in rewards)
                    {
                        postIDs.Add(pair.Key);
                        points.Add(pair.Value);
                    }
                    userIDs = GetUserIdentitiesByPostIdentities(postIDs, true);

                    Dictionary<int, int> userPoints = new Dictionary<int, int>();

                    int i = 0;
                    foreach (int userID in userIDs)
                    {
                        if (userPoints.ContainsKey(userID))
                        {
                            userPoints[userID] += points[i];
                        }
                        else
                            userPoints.Add(userID, points[i]);

                        i++;
                    }

                    UserPoint point = ForumPointAction.Instance.GetUserPoint(thread.PostUserID, ForumPointValueType.QuestionReward, thread.ForumID);
                    success = true;
                    foreach (KeyValuePair<int, int> pair in userPoints)
                    {
                        int[] tempPoints = new int[8];
                        tempPoints[(int)point.Type] = pair.Value;

                        success = UserBO.Instance.UpdateUserPoints(pair.Key, false, false, tempPoints, "论坛", "提问贴结贴：主题ID" + thread.ThreadID);
                        if (success == false)
                            break;
                    }
                    if (success == false)
                    {
                        context.RollbackTransaction();
                        return false;
                    }
                    else
                        context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }

                thread.IsClosed = true;
                question.BestPost = GetPost(bestPostID, true);
                thread.ClearPostsCache();

                return true;
            }
        }

        public bool VoteQuestionBestPost(AuthUser operatorUser, int threadID, bool isUseful)
        {
            BasicThread thread = GetThread(threadID);
            if (thread == null)
            {
                ThrowError<ThreadNotExistsError>(new ThreadNotExistsError());
                return false;
            }

            if (BannedUserProvider.IsFullSiteBanned(operatorUser.UserID) || BannedUserProvider.IsBanned(operatorUser.UserID, thread.ForumID))
            {
                ThrowError<BannedUserCannotVoteBestPostError>(new BannedUserCannotVoteBestPostError());
                return false;
            }

            if (thread is QuestionThread)
            { }
            else
            {
                ThrowError<IsNotQuestionCannotVoteBestPostError>(new IsNotQuestionCannotVoteBestPostError());
                return false;
            }

            if (thread.IsClosed == false)
            {
                ThrowError<QuestionIsNotClosedCannotVoteBestPostError>(new QuestionIsNotClosedCannotVoteBestPostError());
                return false;
            }

            QuestionThread question = (QuestionThread)thread;

            if (!AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(question.ForumID).Can(operatorUser, ForumPermissionSetNode.Action.ReplyThread))
            {
                ThrowError<NoPermissionVoteQuestionBestPostError>(new NoPermissionVoteQuestionBestPostError(operatorUser.UserID));
                return false;
            }

            int returnValue = PostDaoV5.Instance.VoteQuestionBestPost(threadID, operatorUser.UserID, isUseful, question);
            bool success;
            switch (returnValue)
            {
                case 1:
                    ThrowError<ThreadNotExistsError>(new ThreadNotExistsError());
                    success = false;
                    break;
                case 2:
                    ThrowError<QuestionIsNotClosedCannotVoteBestPostError>(new QuestionIsNotClosedCannotVoteBestPostError());
                    success = false;
                    break;
                case 3:
                    ThrowError<HadVotedBestPostError>(new HadVotedBestPostError());
                    success = false;
                    break;
                default:
                    success = true;
                    break;
            }

            return success;
        }

        /// <summary>
        /// 给帖子评分
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="targetUserID"></param>
        /// <param name="postID"></param>
        /// <param name="points">始终传8个 没有评的为0</param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool RatePost(AuthUser operatorUser, int targetUserID, PostV5 post, int[] points, string reason, bool sendMessage)
        {
            RateSetItemCollection rateSetItems = AllSettings.Current.RateSettings.RateSets.GetRateSet(post.ForumID).GetRateItems(operatorUser.UserID);

            int pointID = 0;
            foreach (int point in points)
            {
                if (point != 0)
                {
                    bool hasChecked = false;
                    foreach (RateSetItem item in rateSetItems)
                    {
                        if ((int)item.PointType == pointID)
                        {
                            if (item.MaxValue == 0 && item.MinValue == 0)
                            {
                                ThrowError<NoPermissionRatePointError>(new NoPermissionRatePointError(item.UserPoint.Name));
                                return false;
                            }
                            if (point > item.MaxValue)
                            {
                                ThrowError<PostRateOverMaxValueError>(new PostRateOverMaxValueError(item.UserPoint.Name, point, item.MaxValue));
                                return false;
                            }
                            if (point < item.MinValue)
                            {
                                ThrowError<PostRateOverMinValueError>(new PostRateOverMinValueError(item.UserPoint.Name, point, item.MinValue));
                                return false;
                            }
                            hasChecked = true;
                            break;
                        }
                    }

                    if (hasChecked == false)
                    {
                        ThrowError<NoPermissionRatePointError>(new NoPermissionRatePointError(AllSettings.Current.PointSettings.GetUserPoint((UserPointType)point).Name));
                        return false;
                    }
                }

                pointID++;
            }

            PostMarkCollection postMarks = GetUserTodayPostMarks(operatorUser.UserID);
            int[] values = new int[8];
            foreach (PostMark tempPostMark in postMarks)
            {
                for (int i = 0; i < 8; i++)
                {
                    values[i] += tempPostMark.Points[i];
                }
            }

            RateSetItemCollection topRateSets = AllSettings.Current.RateSettings.RateSets.GetRateSet(0).GetRateItems(operatorUser.UserID);

            for (int i = 0; i < 8; i++)
            {
                if (points[i] != 0)
                {
                    foreach (RateSetItem rateItem in topRateSets)
                    {
                        if ((int)rateItem.PointType == i)
                        {
                            if (Math.Abs(values[i]) + Math.Abs(points[i]) > rateItem.MaxValueInTime)
                            {
                                ThrowError<PostRateOverMaxValueInTimeError>(new PostRateOverMaxValueInTimeError(rateItem.UserPoint.Name, points[i], rateItem.MaxValueInTime));
                                return false;
                            }
                            break;
                        }
                    }
                }
            }

            PostMark newPostMark = null;
            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    int result = PostDaoV5.Instance.CreatePostMark(post.PostID, operatorUser.UserID, operatorUser.Name, DateTimeUtil.Now, points, reason, out newPostMark);
                    if (result == 1)
                    {
                        ThrowError<PostRateHadRatedError>(new PostRateHadRatedError());
                        context.RollbackTransaction();
                        return false;
                    }

                    bool success = UserBO.Instance.UpdateUserPoints(targetUserID, true, true, points, "论坛", string.Concat("帖子评分：", "帖子ID:" + post.PostID, "评分用户：", operatorUser.Username));

                    if (success == false)
                    {
                        context.RollbackTransaction();
                        return false;
                    }

                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }

            }

            if (sendMessage)
            {
                BasicThread thread = GetThread(post.ThreadID);
                Notify notify = new RatePostNotify(operatorUser.UserID, post.ThreadID, thread.SubjectText, ForumBO.Instance.GetForum(post.ForumID).CodeName);
                notify.UserID = post.UserID;
                NotifyBO.Instance.AddNotify(operatorUser , notify);
            }

            PostV5 cachedPost = ThreadCachePool.GetPost(post.ThreadID, post.PostID);
            if (cachedPost != null && newPostMark!=null)
            {
                cachedPost.MarkCount += 1;
                cachedPost.PostMarks.Add(newPostMark);
                if (cachedPost.PostMarks.Count > AllSettings.Current.BbsSettings.ShowMarksCount)
                {
                    cachedPost.PostMarks.RemoveAt(cachedPost.PostMarks.Count - 1);
                }
            }

            BasicThread cachedThread = ThreadCachePool.GetThread(post.ThreadID);
            if (cachedThread != null && cachedThread is QuestionThread)
            {
                QuestionThread question = (QuestionThread)cachedThread;
                if (question.BestPost != null)
                {
                    question.BestPost.MarkCount += 1;
                    question.BestPost.PostMarks.Add(newPostMark);
                    if (question.BestPost.PostMarks.Count > AllSettings.Current.BbsSettings.ShowMarksCount)
                    {
                        question.BestPost.PostMarks.RemoveAt(question.BestPost.PostMarks.Count - 1);
                    }
                }
            }

            return true;
        }

        public PostMarkCollection GetUserTodayPostMarks(int userID)
        {
            return PostDaoV5.Instance.GetUserTodayPostMarks(userID);
        }

        public bool CancelRates(AuthUser operatorUser, PostV5 post, IEnumerable<int> postMarkIDs, string reason, bool sendMessage, string ipAddress)
        {
            if (post == null)
            {
                ThrowError<PostNotExistsError>(new PostNotExistsError());
                return false;
            }

            ManageForumPermissionSetNode permission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(post.ForumID);
            if (false == permission.HasPermissionForSomeone(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.CancelRate))
            {
                ThrowError<NoPermissionCancelRateError>(new NoPermissionCancelRateError());
                return false;
            }

            if (ValidateUtil.HasItems<int>(postMarkIDs) == false)
            {
                ThrowError<NotSelectedPostMarksError>(new NotSelectedPostMarksError("postmarkIDs"));
                return false;
            }

            PostMarkCollection postMarks = PostDaoV5.Instance.GetPostMarks(postMarkIDs);

            if (postMarks.Count == 0)
            {
                ThrowError<PostRateNotExistsError>(new PostRateNotExistsError());
                return false;
            }

            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction(IsolationLevel.ReadUncommitted);

                try
                {
                    List<int> resultPostMarkIDs = new List<int>();
                    foreach (PostMark postMark in postMarks)
                    {
                        if (postMark.PostID != post.PostID)
                            continue;

                        if (false == permission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.CancelRate, postMark.UserID))
                        {
                            continue;
                        }

                        int[] points = new int[8];
                        for (int i = 0; i < 8; i++)
                        {
                            points[i] = -postMark.Points[i];
                        }

                        bool success = UserBO.Instance.UpdateUserPoints(post.UserID, false, false, points, "论坛", string.Concat("撤销评分：PostID = ", post.PostID, ",操作者:" + operatorUser.Username));

                        if (success == false)
                        {
                            context.RollbackTransaction();

                            return false;
                        }

                        resultPostMarkIDs.Add(postMark.PostMarkID);
                    }

                    if (resultPostMarkIDs.Count == 0)
                    {
                        ThrowError<NoPermissionCancelRateError>(new NoPermissionCancelRateError());
                        context.RollbackTransaction();

                        return false;
                    }

                    PostDaoV5.Instance.CancelRates(post.PostID, postMarkIDs);

                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }

                BasicThread thread = ThreadCachePool.GetThread(post.ThreadID);

                if (thread != null)
                {
                    thread.ClearPostsCache();
                }
                else
                    thread = GetThread(post.ThreadID);

                if (sendMessage)
                {
                    Notify notify = new CancelRateNotify(operatorUser.UserID, thread.Forum.CodeName, post.PostID, reason);
                    notify.UserID = post.UserID;
                    NotifyBO.Instance.AddNotify( operatorUser , notify);
                }

                string threadLog;
                CreateThreadManageLog(operatorUser, ipAddress, ModeratorCenterAction.CanacelRate, new int[1] { thread.PostUserID }, thread.ForumID, new int[1] { thread.ThreadID }
                    , new string[1] { thread.SubjectText }, reason, false, out threadLog);

                if (threadLog != null)
                    thread.ThreadLog = threadLog;
                return true;
            }
        }

        public PostMarkCollection GetPostMarks(int postID, int pageNumber, int pageSize, out int totalCount)
        {
            return PostDaoV5.Instance.GetPostMarks(postID, pageNumber, pageSize, out totalCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="ipAddress"></param>
        /// <param name="actionType"></param>
        /// <param name="postUserIDs"></param>
        /// <param name="forumID"></param>
        /// <param name="threadIDs"></param>
        /// <param name="subjects"></param>
        /// <param name="reason"></param>
        /// <param name="isPublic"></param>
        /// <param name="threadLog"></param>
        public void CreateThreadManageLog(AuthUser operatorUser, string ipAddress, ModeratorCenterAction actionType
            , IEnumerable<int> postUserIDs
            , int forumID, IEnumerable<int> threadIDs, IEnumerable<string> subjects, string reason, bool isPublic, out string threadLog)
        {
            List<string> tempSubjects = new List<string>();
            foreach (string subject in subjects)
            {
                tempSubjects.Add(subject.Replace(',', ' ').Replace('，', ' '));
            }

            PostDaoV5.Instance.CreateThreadManageLog(operatorUser.UserID, operatorUser.Username, operatorUser.Realname, ipAddress, actionType, postUserIDs, forumID, threadIDs, tempSubjects, reason, isPublic, out threadLog);
        }

        public ThreadManageLogCollectionV5 GetThreadManageLogs(int threadID)
        {
            return PostDaoV5.Instance.GetThreadManageLogs(threadID);
        }


        public void ClearSearchResult()
        {
            PostDaoV5.Instance.ClearSearchResult();
        }

        public ThreadCollectionV5 GetThreadsByLastPostCreateDate(DateTime postCreateDate)
        {
            ThreadCollectionV5 threads = PostDaoV5.Instance.GetThreadsByPostCreateDate(postCreateDate);
            threads = ProcessStickThreads(threads, null, int.MaxValue, ThreadSortField.LastReplyDate, delegate(BasicThread thread)
            {
                if (thread.CreateDate < postCreateDate)
                    return false;
                else
                    return true;
            });

            threads = ProcessMovedThreads(threads);

            return threads;
        }


        public void ProcessExperiesTopicStatus(TopicStatusCollection status)
        {
            if (status.Count == 0)
                return;

            List<int> threadIDs = new List<int>();
            foreach (TopicStatus statu in status)
            {
                if (threadIDs.Contains(statu.ThreadID) == false)
                    threadIDs.Add(statu.ThreadID);
            }

            ThreadCollectionV5 threads = GetThreads(threadIDs);

            List<int> ids = new List<int>();

            Dictionary<int, List<int>> locks = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> sticks = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> heightlights = new Dictionary<int, List<int>>();
            foreach (TopicStatus statu in status)
            {
                ids.Add(statu.ID);
                foreach (BasicThread thread in threads)
                {
                    if (statu.ThreadID == thread.ThreadID)
                    {
                        Dictionary<int, List<int>> temp;
                        if (statu.Type == TopicStatuType.HeightLight)
                        {
                            temp = heightlights;
                        }
                        else if (statu.Type == TopicStatuType.Lock)
                            temp = locks;
                        else
                            temp = sticks;

                        if (temp.ContainsKey(thread.ForumID) == false)
                        {
                            temp.Add(thread.ForumID, new List<int>());
                        }
                        temp[thread.ForumID].Add(statu.ThreadID);
                        break;
                    }
                }
            }

            if (locks.Count != 0)
            {
                foreach (KeyValuePair<int, List<int>> pair in locks)
                {
                    SetThreadsLock(User.Guest, pair.Key, pair.Value, false, true, false, false, "");
                }
            }
            if (heightlights.Count != 0)
            {
                foreach (KeyValuePair<int, List<int>> pair in heightlights)
                {
                    SetThreadsSubjectStyle(User.Guest, pair.Key, pair.Value, string.Empty, true, false, false, "");
                }
            }
            if (sticks.Count != 0)
            {
                foreach (KeyValuePair<int, List<int>> pair in sticks)
                {
                    SetThreadsStickyStatus(User.Guest, pair.Key, null, pair.Value, ThreadStatus.Normal, true, false, false, "");
                }
            }

            PostDaoV5.Instance.DeleteTopicStatus(ids);
        }

        /// <summary>
        /// 检查过期的投票和过期的提问，并进行相应处理
        /// </summary>
        public void AutoFinalQuestion(List<int> questionThreadIds, Dictionary<int, Dictionary<int, int>> forumIDAndRewards)
        {
            foreach (KeyValuePair<int, Dictionary<int, int>> pair in forumIDAndRewards)
            {
                UserPoint userPoint = ForumPointAction.Instance.GetUserPoint(pair.Key, ForumPointValueType.QuestionReward, pair.Key);

                foreach (KeyValuePair<int, int> tempPair in pair.Value)
                {
                    int[] points = new int[8];

                    points[(int)userPoint.Type] = tempPair.Value;
                    UserBO.Instance.UpdateUserPoints(tempPair.Key, false, false, points,"论坛","论坛自动结贴，主题ID："+StringUtil.Join(questionThreadIds));
                }
            }

            //clearThreadsCache(questionThreadIds);
        }
        #endregion



        public PostCollectionV5 GetUserPosts(int userID, DateTime beginDate, DateTime endDate)
        {
            return PostDaoV5.Instance.GetUserPosts(userID, beginDate, endDate);
        }

        /// <summary>
        /// 重新统计 今日昨日主题数帖子数
        /// </summary>
        public bool ReCountTopicsAndPosts(bool recountToday, bool recountYestoday)
        {
            if (recountYestoday == false && recountToday == false)
                return true;

            bool success = PostDaoV5.Instance.ReCountTopicsAndPosts(recountToday, recountYestoday);
            if (success)
            {
                ForumBO.Instance.ClearAllCache();
                VarsManager.ResetVars();
            }

            return success;
        }


        public bool StartPostFullTextIndex()
        {
            return PostDaoV5.Instance.StartPostFullTextIndex();
        }

        public bool StopPostFullTextIndex()
        {
            return PostDaoV5.Instance.StopPostFullTextIndex();
        }


        public Guid SearchTopics(AuthUser operatorUser, IEnumerable<int> forumIDs, string keyword, SearchMode mode, SearchType searchType, DateTime? postDate, bool isBefore, bool isDesc, int maxResultCount)
        {
            SearchSettings setting = AllSettings.Current.SearchSettings;
            int targetUserID = 0;
            List<string> keywords = new List<string>();

            #region 检查权限
            if (operatorUser.UserID == 0 && setting.EnableGuestSearch == false)
            {
                ThrowError<NoPermissionSearchPostError>(new NoPermissionSearchPostError());
                return Guid.Empty;
            }
            else if (operatorUser.UserID != 0 && setting.EnableSearch[operatorUser] == false)
            {
                ThrowError<NoPermissionSearchPostError>(new NoPermissionSearchPostError());
                return Guid.Empty;
            }

            switch (mode)
            {
                case SearchMode.Content:
                    if (operatorUser.UserID == 0 && setting.GuestCanSearchAllPost == false)
                    {
                        ThrowError<NoPermissionSearchAllPostsError>(new NoPermissionSearchAllPostsError());
                    }
                    else if (operatorUser.UserID != 0 && setting.CanSearchAllPost[operatorUser] == false)
                    {
                        ThrowError<NoPermissionSearchAllPostsError>(new NoPermissionSearchAllPostsError());
                    }
                    break;
                case SearchMode.TopicContent:
                    if (operatorUser.UserID == 0 && setting.GuestCanSearchTopicContent == false)
                    {
                        ThrowError<NoPermissionSearchTopicContentsError>(new NoPermissionSearchTopicContentsError());
                    }
                    else if (operatorUser.UserID != 0 && setting.CanSearchTopicContent[operatorUser] == false)
                    {
                        ThrowError<NoPermissionSearchTopicContentsError>(new NoPermissionSearchTopicContentsError());
                    }
                    break;

                case SearchMode.UserThread:
                    if (operatorUser.UserID == 0 && setting.GuestCanSearchUserTopic == false)
                    {
                        ThrowError<NoPermissionSearchUserTopicError>(new NoPermissionSearchUserTopicError());
                    }
                    else if (operatorUser.UserID != 0 && setting.CanSearchUserTopic[operatorUser] == false)
                    {
                        ThrowError<NoPermissionSearchUserTopicError>(new NoPermissionSearchUserTopicError());
                    }
                    break;

                case SearchMode.UserPost:
                    if (operatorUser.UserID == 0 && setting.GuestCanSearchUserPost == false)
                    {
                        ThrowError<NoPermissionSearchUserPostError>(new NoPermissionSearchUserPostError());
                    }
                    else if (operatorUser.UserID != 0 && setting.CanSearchUserPost[operatorUser] == false)
                    {
                        ThrowError<NoPermissionSearchUserPostError>(new NoPermissionSearchUserPostError());
                    }
                    break;
                default: break;
            }

            if (HasUnCatchedError)
                return Guid.Empty;

            #endregion

            if (mode == SearchMode.Content || mode == SearchMode.Subject || mode == SearchMode.TopicContent)
            {
                keywords = GetSearchKeywords(keyword);
                if (keywords.Count == 0)
                {
                    ThrowError<SearchInvalidKeywordError>(new SearchInvalidKeywordError(keyword));
                    return Guid.Empty;
                }
            }
            else
            //if (mode == SearchMode.UserPost || mode == SearchMode.UserThread)
            {
                if (keyword == string.Empty)
                {
                    ThrowError<SearchInvalidKeywordError>(new SearchInvalidKeywordError(keyword));
                    return Guid.Empty;
                }
                User user = UserBO.Instance.GetUser(keyword.Trim());

                if (user == null)
                {
                    ThrowError<SearchUserNotExistsError>(new SearchUserNotExistsError(keyword));
                    return Guid.Empty;
                }

                targetUserID = user.UserID;
            }

            int intervalTime;
            if (operatorUser.UserID == 0)
                intervalTime = setting.GuestSearchTime;
            else
                intervalTime = setting.SearchTime[operatorUser];

            //去除没有权限查看的版快
            bool isAllForum = false;
            List<int> canVisitForumIDs = ForumBO.Instance.GetForumIdsForVisit(operatorUser);

            if (canVisitForumIDs == null)
                isAllForum = true;

            List<int> allForumIDs = new List<int>(ForumBO.Instance.GetAllForums().GetKeys());
            List<int> resultForumIDs = new List<int>();
            if (!isAllForum)
            {
                if (forumIDs != null && ValidateUtil.HasItems<int>(forumIDs))
                {
                    foreach (int forumID in forumIDs)
                    {
                        if (canVisitForumIDs.Contains(forumID))
                            resultForumIDs.Add(forumID);
                    }

                }
                else
                    resultForumIDs = canVisitForumIDs;

                if (resultForumIDs.Count == 0)
                {
                    ThrowError<SearchNoPermissionForumError>(new SearchNoPermissionForumError());
                    return Guid.Empty;
                }
            }
            else
            {
                canVisitForumIDs = allForumIDs;
                if (forumIDs != null)
                {
                    foreach (int forumID in forumIDs)
                        resultForumIDs.Add(forumID);
                }
                else
                    resultForumIDs = null;
            }



            Guid searchID = PostDaoV5.Instance.SearchTopics(operatorUser.UserID, IPUtil.GetCurrentIP(), resultForumIDs, canVisitForumIDs, allForumIDs, keywords, targetUserID, mode, searchType, postDate, isBefore, isDesc, maxResultCount, intervalTime);

            if (searchID == Guid.Empty)
            {
                ThrowError<SearchOverSearchTimeError>(new SearchOverSearchTimeError(intervalTime));
            }
            return searchID;
        }

        public void SearchTopics(Guid searchID, int pageSize, int pageNumber, SearchType searchType, int maxResultCount, out int totalCount, out string keyword, out SearchMode mode, out ThreadCollectionV5 threads, out PostCollectionV5 posts)
        {
            List<int> allForumIDs = new List<int>(ForumBO.Instance.GetAllForums().GetKeys());
            List<int> canVisitForumIDs = ForumBO.Instance.GetForumIdsForVisit(User.Current);
            if (canVisitForumIDs == null)
                canVisitForumIDs = allForumIDs;


            PostDaoV5.Instance.SearchTopics(searchID, pageSize, pageNumber, searchType, maxResultCount, canVisitForumIDs, allForumIDs, out totalCount, out keyword, out mode, out threads, out posts);

        }

        /// <summary>
        /// 查找已经购买过的主题ID 和 已经回复过的主题
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="threadIDs"></param>
        /// <param name="buyedIds"></param>
        /// <param name="repliedIDs"></param>
        public void CheckThreads(int operatorUserID, IEnumerable<int> threadIDs, out List<int> buyedIds, out List<int> repliedIDs)
        {
            buyedIds = new List<int>();
            repliedIDs = new List<int>();

            if (ValidateUtil.HasItems<int>(threadIDs) == false)
                return;

            PostDaoV5.Instance.CheckThreads(operatorUserID, threadIDs, out buyedIds, out repliedIDs);
        }


        public List<string> GetSearchKeywords(string keyword)
        {
            List<string> words = new List<string>();

            keyword = FullTextIndexFilter(keyword);

            int maxLength = 20;
            string[] keywords = keyword.Split(' ');

            int length = 0;
            foreach (string word in keywords)
            {
                string tempWord = word;
                if (tempWord == string.Empty)
                    continue;

                if (tempWord.Length + length > maxLength)
                {
                    tempWord = tempWord.Substring(0, maxLength - length);
                }
                length += tempWord.Length;

                words.Add(tempWord);

                if (length == maxLength)
                    break;
            }

            return words;
        }

        private static Regex filter = new Regex("[~`!@#$%^&*()_\\-+=|\\{}\\[\\]:;\"'<>,?/]", RegexOptions.Compiled);
        private string FullTextIndexFilter(string searchText)
        {
            if (searchText.Length > 256)
                searchText = searchText.Substring(0, 256);

            searchText = filter.Replace(searchText, " ");

            return searchText;
        }



        public void ClearShowChargePointLinks()
        {
            s_ShowChargePointLinks = null;
        }

        private static bool?[] s_ShowChargePointLinks;// = new bool[8];
        /// <summary>
        /// 是否显示充值链接
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsShowChargePointLink(UserPoint point)
        {
            if (AllSettings.Current.PaySettings.EnablePointRecharge)
            {
                if (s_ShowChargePointLinks == null)
                {
                    s_ShowChargePointLinks = new bool?[8];
                    //showChargePointLinks = new Dictionary<UserPointType, bool>();
                }

                bool? show = s_ShowChargePointLinks[(int)point.Type];
                if (show == null)
                {
                    foreach (PointRechargeRule rule in AllSettings.Current.PointSettings.PointRechargeRules)
                    {
                        if (rule.UserPointType == point.Type)
                        {
                            s_ShowChargePointLinks[(int)point.Type] = rule.Enable;
                            return rule.Enable;
                        }
                    }

                    s_ShowChargePointLinks[(int)point.Type] = false;
                    //showChargePointLinks.Add(point.Type, false);
                    return false;
                }
                return show.Value;
            }

            return false;
        }


        /// <summary>
        /// 没有判断权限
        /// </summary>
        /// <param name="threads"></param>
        /// <param name="forumID"></param>
        /// <param name="sortType">如果为null则按SortOrder排序</param>
        /// <returns></returns>
        private ThreadCollectionV5 ProcessStickThreads(ThreadCollectionV5 threads, int? forumID, int cacheCount, ThreadSortField? sortType, CheckThread checkThread)
        {
            ThreadCollectionV5 stickThreads = GetGlobalThreads();
            threads = ProcessStickThreads(stickThreads, threads, forumID, cacheCount, sortType, checkThread);
            if (forumID != null)
            {
                stickThreads = GetStickThreads(forumID.Value);
                threads = ProcessStickThreads(stickThreads, threads, forumID, cacheCount, sortType, checkThread);
            }
            else
            {
                int[] forumIDs = ForumBO.Instance.GetAllForums().GetKeys();
                stickThreads = GetStickThreads(forumIDs);
                threads = ProcessStickThreads(stickThreads, threads, null, cacheCount, sortType, checkThread);
            }

            return threads;
        }

        private delegate bool CheckThread(BasicThread thread);

        private ThreadCollectionV5 ProcessStickThreads(ThreadCollectionV5 stickThreads, ThreadCollectionV5 threads, int? forumID, int cacheCount, ThreadSortField? sortType, CheckThread checkThread)
        {
            int count = threads.Count;
            BasicThread lastThread;

            if (count > 0)
                lastThread = threads[count - 1];
            else
                lastThread = null;
            foreach (BasicThread stickThread in stickThreads)
            {
                if (forumID != null && stickThread.ForumID != forumID.Value)
                    continue;

                if (checkThread(stickThread) == false)
                    continue;

                //if (beginDate != null && stickThread.CreateDate < beginDate.Value)
                //    continue;

                if (lastThread!=null && Compare(stickThread, lastThread, sortType) == false)//比最后一个还小  不加入
                    continue;

                if (threads.ContainsKey(stickThread.ThreadID))
                    continue;


                int i = 0;
                foreach (BasicThread thread in threads)
                {
                    if (Compare(stickThread, thread, sortType))
                    {
                        break;
                    }
                    i++;
                }
                if (i < cacheCount)
                {
                    threads.Insert(i, stickThread);
                    //count++;
                }
            }

            int removeCount = threads.Count - cacheCount;
            if (removeCount > 0)
            {
                for (int i = 0; i < removeCount; i++)
                {
                    threads.RemoveAt(threads.Count - 1);
                }
            }

            return threads;
        }

        private bool Compare(BasicThread thread1, BasicThread thread2, ThreadSortField? sortType)
        {
            if (sortType == null)
                return thread1.SortOrder > thread2.SortOrder;
            else if (sortType.Value == ThreadSortField.LastReplyDate)
                return thread1.LastPostID > thread2.LastPostID;
            else if (sortType.Value == ThreadSortField.CreateDate)
                return thread1.ThreadID > thread2.ThreadID;
            else if (sortType.Value == ThreadSortField.Replies)
                return thread1.TotalReplies > thread2.TotalReplies;
            else
                return thread1.TotalViews > thread2.TotalViews;
        }


        #region 关键字




        #endregion
    }
}