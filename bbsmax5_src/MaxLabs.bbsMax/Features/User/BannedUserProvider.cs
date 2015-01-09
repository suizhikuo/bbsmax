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
using System.Data;
using MaxLabs.bbsMax.DataAccess;
using System.Threading;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax
{
    /// <summary>
    /// 屏蔽用户管理器
    /// 
    /// author:weu quan
    /// 2009-5-31 
    /// </summary>
    public static class BannedUserProvider
    {

        private static object locker = new object();

        /// <summary>
        /// 检查某用户是否在某个板块被屏蔽
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public static bool IsBanned(int userID, int forumID)
        {
            string cachekey = "IsBanned_{0}_{1}";
            cachekey = string.Format(cachekey, userID, forumID);

            bool isBanned;

            if (false == PageCacheUtil.TryGetValue<bool>(cachekey, out isBanned))
            {
                isBanned = false;
                long key = GetKey(userID, forumID);

                isBanned = AllBannedUsers.Limited.ContainsKey(userID) || AllBannedUsers.Limited.ContainsKey(key);
                PageCacheUtil.Set(cachekey, isBanned);
            }
            return isBanned;
        }

        private static Dictionary<int, List<int>> m_UserBannedForums;
        private static Dictionary<int, List<int>> UserBannedForums
        {
            get
            {
                if (m_UserBannedForums == null)
                {
                    BannedUserCollection allBannedUsers = AllBannedUsers;
                    lock (locker)
                    {
                        if (m_UserBannedForums == null)
                        {
                            m_UserBannedForums = new Dictionary<int, List<int>>();
                            foreach (BannedUser b in AllBannedUsers)
                            {
                                if (!m_UserBannedForums.ContainsKey(b.UserID))
                                {
                                    m_UserBannedForums.Add(b.UserID, new List<int>());
                                }
                                m_UserBannedForums[b.UserID].Add(b.ForumID);
                            }
                        }
                    }
                }

                return m_UserBannedForums;
            }
            set
            {
                m_UserBannedForums = null;
            }
        }
        private static BannedUserCollection m_allbannedUsers = null;
        private static Dictionary<int, BannedUserCollection> m_forumBannedUsers;
        private static Dictionary<int, BannedUserCollection> ForumBannedUsers
        {
            get
            {
                if (m_forumBannedUsers == null)
                {
                    BannedUserCollection allBannnedUsers = AllBannedUsers;
                    if (m_forumBannedUsers == null)
                    {
                        m_forumBannedUsers = new Dictionary<int, BannedUserCollection>();
                        foreach (BannedUser b in allBannnedUsers)
                        {
                            if (!m_forumBannedUsers.ContainsKey(b.ForumID))
                            {
                                m_forumBannedUsers.Add(b.ForumID, new BannedUserCollection());
                            }
                            m_forumBannedUsers[b.ForumID].Add(b);
                        }
                    }
                }

                return m_forumBannedUsers;
            }
            set
            {
                m_forumBannedUsers = value;
            }
        }
        private static BannedUserCollection AllBannedUsers
        {
            get
            {
                if (m_allbannedUsers == null)
                {
                    lock (locker)
                    {
                        if (m_allbannedUsers == null)
                        {
                            m_allbannedUsers = BannedUserDao.Instance.GetAllBannedUsers();
                        }
                    }
                }
                return m_allbannedUsers;
            }
            set
            {
                m_allbannedUsers = value;
            }
        }

        /// <summary>
        /// 某用户是否被屏蔽
        /// </summary>
        /// <param name="userID"></param>
        public static bool Contains(int userID)
        {
            return UserBannedForums.ContainsKey(userID);
        }

        /// <summary>
        /// 是否整站屏蔽
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static bool IsFullSiteBanned(int userID)
        {
            return IsBanned(userID, 0);
        }

        /// <summary>
        /// 是否在某个版块被屏蔽
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static bool IsForumBanned(int userID)
        {
            if (UserBannedForums.ContainsKey(userID))
            {
                List<int> forumIds = UserBannedForums[userID];
                foreach (int forumid in forumIds)
                {
                    if (forumid == 0) continue;

                    long key = GetKey(userID, forumid);
                    if (ForumBannedUsers.ContainsKey(forumid) && ForumBannedUsers[forumid].Limited.ContainsKey(key))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 某版块被屏蔽的用户数
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public static int GetBannedUserCount(int forumID)
        {
            if (ForumBannedUsers.ContainsKey(forumID))
            {
                return ForumBannedUsers[forumID].Count;
            }
            return 0;
        }

        public static void ClearInnerTable()
        {
            UserBannedForums = null;
            ForumBannedUsers = null;
            AllBannedUsers = null;
        }

        private static long GetKey(int userid, int forumID)
        {
            return forumID * 1000000000 + userid;
        }

        /// <summary>
        /// 取得某用户的所有被屏蔽的版块信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static BannedUserCollection GetUserBannedForumInfo(int userID)
        {
            BannedUserCollection banned = new BannedUserCollection();
            if (UserBannedForums.ContainsKey(userID))
            {
                List<int> bannedForumids = UserBannedForums[userID];
                foreach (int id in bannedForumids)
                {
                    long key = GetKey(userID, id);
                    BannedUser b = AllBannedUsers.GetValue(key);
                    if (b != null)
                        banned.Add(b);
                }
            }
            return banned;
        }

        /// <summary>
        /// 取得某版块所有被屏蔽的用户信息
        /// </summary>
        /// <returns></returns>
        public static BannedUserCollection GetBannedInfos(int forumID, int pageSize, int pageNumber, out int rowCount)
        {
            BannedUserCollection result;
            rowCount = 0;
            if (ForumBannedUsers.ContainsKey(forumID))
            {
                result = ForumBannedUsers[forumID].Limited;
                rowCount = result.Count;
                BannedUserCollection pagedUsers = new BannedUserCollection();
                int index = 0;
                for (int i = 0; i < pageSize; i++)
                {
                    index = i + (pageNumber - 1) * pageSize;

                    if (index >= result.Count)
                        break;
                    pagedUsers.Add(result[index]);
                }

                return pagedUsers;
            }
            else
            {
                return new BannedUserCollection();
            }
        }
    }
}