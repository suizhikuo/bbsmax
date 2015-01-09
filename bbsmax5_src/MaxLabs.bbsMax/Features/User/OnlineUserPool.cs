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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using System.Web;

namespace MaxLabs.bbsMax
{
    public class OnlineUserPool
    {
        #region 定义的私有变量

        private object m_OnlineMembers_Locker = new object();
        private Dictionary<int, OnlineMember> m_OnlineMemberTable = new Dictionary<int, OnlineMember>();
        private OnlineMemberCollection m_OnlineMembers = new OnlineMemberCollection();

        //==

        private object m_OnlineGuests_Locker = new object();
        private Dictionary<string, OnlineGuest> m_OnlineGuestTable = new Dictionary<string, OnlineGuest>();
        //private Dictionary<string, List<string>> m_OnlineGuestIdsByIP = new Dictionary<string, List<string>>();
        private OnlineGuestCollection m_OnlineGuests = new OnlineGuestCollection();

        //==

        private object m_ForumOnlineMembers_AllLocker = new object();

        private object m_ForumOnlineMembers_LockerList_Locker = new object();
        private Dictionary<int, object> m_ForumOnlineMembers_LockerList = new Dictionary<int, object>();

        private Dictionary<int, OnlineMemberCollection> m_ForumOnlineMembers = new Dictionary<int, OnlineMemberCollection>();

        //==

        private object m_ForumOnlineGuests_AllLocker = new object();

        private object m_ForumOnlineGuests_LockerList_Locker = new object();
        private Dictionary<int, object> m_ForumOnlineGuests_LockerList = new Dictionary<int, object>();

        private Dictionary<int, OnlineGuestCollection> m_ForumOnlineGuests = new Dictionary<int, OnlineGuestCollection>();

        //==

        private object m_OnlineGuestsInIps_Locker = new object();

        private Dictionary<string, OnlineGuestsInIp> m_OnlineGuestsInIps = new Dictionary<string, OnlineGuestsInIp>();

        //==

        private static OnlineUserPool s_Instance = new OnlineUserPool();

        #endregion

        /// <summary>
        /// 在线列表池的唯一实例
        /// </summary>
        public static OnlineUserPool Instance
        {
            get { return s_Instance; }
        }

        #region 对外提供数据的方法


        public OnlineMemberCollection GetOnlineMembers()
        {
            return m_OnlineMembers;
        }

        public Dictionary<int, OnlineMember> GetOnlineMemberTable()
        {
            return m_OnlineMemberTable;
        }

        public Dictionary<int, OnlineMemberCollection> GetForumOnlineMembers()
        {
            return m_ForumOnlineMembers;
        }

        public OnlineGuestCollection GetOnlineGuests()
        {
            return m_OnlineGuests;
        }

        public Dictionary<string, OnlineGuest> GetOnlineGuestTable()
        {
            return m_OnlineGuestTable;
        }

        public Dictionary<string, OnlineGuestsInIp> GetOnlineGuestsInIps()
        {
            return m_OnlineGuestsInIps;
        }

        public Dictionary<int, OnlineGuestCollection> GetForumOnlineGuests()
        {
            return m_ForumOnlineGuests;
        }

        

        #endregion

        #region 内部助手方法

        private object GetForumOnlineMemberLocker(int forumID)
        {
            object locker;

            if (m_ForumOnlineMembers_LockerList.TryGetValue(forumID, out locker) == false)
            {
                lock (m_ForumOnlineMembers_LockerList_Locker)
                {
                    if (m_ForumOnlineMembers_LockerList.TryGetValue(forumID, out locker) == false)
                    {
                        locker = new object();
                        m_ForumOnlineMembers_LockerList.Add(forumID, locker);
                    }
                }
            }

            return locker;
        }

        /// <summary>
        /// 把一个用户加入新的版块。警告：本方法不是线程安全的，需自行加锁
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="onlineMember"></param>
        private void AddForumOnlineMember(int forumID, OnlineMember onlineMember)
        {
            bool newForum = false;

            OnlineMemberCollection onlineMembersInForum;

            //新板块还没有建立列表，那么立即创建一个
            if (m_ForumOnlineMembers.TryGetValue(forumID, out onlineMembersInForum) == false)
            {
                lock (m_ForumOnlineMembers_AllLocker)
                {
                    if (m_ForumOnlineMembers.TryGetValue(forumID, out onlineMembersInForum) == false)
                    {
                        onlineMembersInForum = new OnlineMemberCollection();
                        onlineMembersInForum.Add(onlineMember);
                        m_ForumOnlineMembers.Add(forumID, onlineMembersInForum);

                        newForum = true;
                    }
                }
            }

            //就算是更新原来的值，也不对原列表进行改动，而是复制一份新列表出来，并增加新项，以免发生“集合已更改”的问题
            if (newForum == false)
            {
                OnlineMemberCollection newOnlineMembersInForum = new OnlineMemberCollection(onlineMembersInForum);
                newOnlineMembersInForum.Add(onlineMember);
                m_ForumOnlineMembers[forumID] = newOnlineMembersInForum;
            }
        }

        /// <summary>
        /// 把一个用户从版块中移除。警告：本方法不是线程安全的，需自行加锁
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="onlineMember"></param>
        private void RemoveForumOnlineMember(int forumID, OnlineMember onlineMember)
        {
            OnlineMemberCollection onlineMembersInForum;

            //从原来的版块中移除
            if (m_ForumOnlineMembers.TryGetValue(forumID, out onlineMembersInForum))
            {
                OnlineMemberCollection newOnlineMembersInForum = new OnlineMemberCollection(onlineMembersInForum);
                newOnlineMembersInForum.Remove(onlineMember);
                m_ForumOnlineMembers[forumID] = newOnlineMembersInForum;
            }
        }

        //==================================================================================

        private object GetForumOnlineGuestLocker(int forumID)
        {
            object locker;

            if (m_ForumOnlineGuests_LockerList.TryGetValue(forumID, out locker) == false)
            {
                lock (m_ForumOnlineGuests_LockerList_Locker)
                {
                    if (m_ForumOnlineGuests_LockerList.TryGetValue(forumID, out locker) == false)
                    {
                        locker = new object();
                        m_ForumOnlineGuests_LockerList.Add(forumID, locker);
                    }
                }
            }

            return locker;
        }

        /// <summary>
        /// 把一个用户加入新的版块。警告：本方法不是线程安全的，需自行加锁
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="onlineGuest"></param>
        private void AddForumOnlineGuest(int forumID, OnlineGuest onlineGuest)
        {
            bool newForum = false;

            OnlineGuestCollection onlineGuestsInForum;

            //新板块还没有建立列表，那么立即创建一个
            if (m_ForumOnlineGuests.TryGetValue(forumID, out onlineGuestsInForum) == false)
            {
                lock (m_ForumOnlineGuests_AllLocker)
                {
                    if (m_ForumOnlineGuests.TryGetValue(forumID, out onlineGuestsInForum) == false)
                    {
                        onlineGuestsInForum = new OnlineGuestCollection();
                        onlineGuestsInForum.Add(onlineGuest);
                        m_ForumOnlineGuests.Add(forumID, onlineGuestsInForum);

                        newForum = true;
                    }
                }
            }

            //就算是更新原来的值，也不对原列表进行改动，而是复制一份新列表出来，并增加新项，以免发生“集合已更改”的问题
            if (newForum == false)
            {
                OnlineGuestCollection newOnlineGuestsInForum = new OnlineGuestCollection(onlineGuestsInForum);
                newOnlineGuestsInForum.Add(onlineGuest);
                m_ForumOnlineGuests[forumID] = newOnlineGuestsInForum;
            }
        }

        /// <summary>
        /// 把一个用户从版块中移除。警告：本方法不是线程安全的，需自行加锁
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="onlineGuest"></param>
        private void RemoveForumOnlineGuest(int forumID, OnlineGuest onlineGuest)
        {
            OnlineGuestCollection onlineGuestsInForum;

            //从原来的版块中移除
            if (m_ForumOnlineGuests.TryGetValue(forumID, out onlineGuestsInForum))
            {
                OnlineGuestCollection newOnlineGuestsInForum = new OnlineGuestCollection(onlineGuestsInForum);
                newOnlineGuestsInForum.Remove(onlineGuest);
                m_ForumOnlineGuests[forumID] = newOnlineGuestsInForum;
            }
        }

        /// <summary>
        /// 将一个用户从在线列表中移除。本方法线程安全，无需自行加锁
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private bool RemoveOnlineMember(int userID)
        {
            OnlineMember onlineMember;
            return RemoveOnlineMember(userID, out onlineMember);
        }

        /// <summary>
        /// 将一个用户从在线列表中移除。本方法线程安全，无需自行加锁
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="onlineMember"></param>
        /// <returns></returns>
        private bool RemoveOnlineMember(int userID, out OnlineMember onlineMember)
        {
            bool result = false;
            onlineMember = null;

            if (m_OnlineMemberTable.ContainsKey(userID))
            {
                lock (m_OnlineMembers_Locker)
                {
                    //OnlineMember onlineMember;
                    if (m_OnlineMemberTable.TryGetValue(userID, out onlineMember))
                    {
                        result = true;
                        lock (onlineMember)
                        {
                            int oldForumID = onlineMember.ForumID;

                            //-1表示其实已经被移除了
                            if (oldForumID == -1)
                            {
                                LogHelper.CreateDebugLog("线程安全问题1");
                                return false;
                            }

                            if (oldForumID != 0)
                            {
                                lock (GetForumOnlineMemberLocker(oldForumID))
                                {
                                    //从原来的版块移除
                                    RemoveForumOnlineMember(oldForumID, onlineMember);
                                }
                            }

                            onlineMember.ForumID = -1;

                            m_OnlineMemberTable.Remove(userID);
                            OnlineMemberCollection newOnlineMembers = new OnlineMemberCollection(m_OnlineMembers);
                            newOnlineMembers.Remove(onlineMember);
                            m_OnlineMembers = newOnlineMembers;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 将一个游客从在线列表中移除。本方法线程安全，无需自行加锁
        /// </summary>
        /// <param name="guestID"></param>
        /// <returns></returns>
        private bool RemoveOnlineGuest(string guestID)
        {
            OnlineGuest onlineGuest;
            return RemoveOnlineGuest(guestID, out onlineGuest);
        }

        /// <summary>
        /// 将一个游客从在线列表中移除。本方法线程安全，无需自行加锁
        /// </summary>
        /// <param name="guestID"></param>
        /// <param name="onlineGuest"></param>
        /// <returns></returns>
        private bool RemoveOnlineGuest(string guestID, out OnlineGuest onlineGuest)
        {
            bool result = false;
            onlineGuest = null;

            if (m_OnlineGuestTable.ContainsKey(guestID))
            {
                lock (m_OnlineGuests_Locker)
                {
                    if (m_OnlineGuestTable.TryGetValue(guestID, out onlineGuest))
                    {
                        result = true;
                        lock (onlineGuest)
                        {
                            int oldForumID = onlineGuest.ForumID;

                            //-1表示其实已经被移除了
                            if (oldForumID == -1)
                            {
                                LogHelper.CreateDebugLog("线程安全问题1");
                                return false;
                            }

                            if (oldForumID != 0)
                            {
                                lock (GetForumOnlineGuestLocker(oldForumID))
                                {
                                    //从原来的版块移除
                                    RemoveForumOnlineGuest(oldForumID, onlineGuest);
                                }
                            }

                            onlineGuest.ForumID = -1;

                            //把这个游客的IP从游客IP表中去除
                            lock (m_OnlineGuestsInIps_Locker)
                            {
                                OnlineGuestsInIp onlineGuestIp;
                                if (m_OnlineGuestsInIps.TryGetValue(onlineGuest.IP, out onlineGuestIp))
                                {
                                    onlineGuestIp.GuestIds.Remove(onlineGuest.GuestID);

                                    if (onlineGuestIp.GuestIds.Count == 0)
                                        m_OnlineGuestsInIps.Remove(onlineGuest.IP);
                                }
                            }

                            m_OnlineGuestTable.Remove(guestID);
                            OnlineGuestCollection newOnlineGuests = new OnlineGuestCollection(m_OnlineGuests);
                            newOnlineGuests.Remove(onlineGuest);
                            m_OnlineGuests = newOnlineGuests;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// (此方法必须在lock住游客列表后方可调用)
        /// </summary>
        /// <param name="guestID"></param>
        /// <param name="oldIP"></param>
        /// <param name="newIP"></param>
        /// <param name="isSpider"></param>
        /// <returns></returns>
        private bool UpdateOnlineGuestIP(string guestID, string oldIP, string newIP, bool isSpider)
        {
            //如果IP未发生变化，无需继续运行下去
            if (oldIP == newIP)
                return false;

            OnlineGuestsInIp onlineGuestIp;

            //if (onlineGuestIp!=null)

            lock (m_OnlineGuestsInIps_Locker)
            {

                //如果是IP临时发生了变化，那么要从原来的IP中删除
                if (string.IsNullOrEmpty(oldIP) == false)
                {
                    if (m_OnlineGuestsInIps.TryGetValue(oldIP, out onlineGuestIp))
                    {
                        onlineGuestIp.GuestIds.Remove(guestID);

                        if (onlineGuestIp.GuestIds.Count == 0)
                            m_OnlineGuestsInIps.Remove(oldIP);
                    }
                }

                if (m_OnlineGuestsInIps.TryGetValue(newIP, out onlineGuestIp))
                {
                    //如果是蜘蛛  那么onlineGuestIp 中所有都是蜘蛛
                    if (isSpider && OnlineSettings.IsShowAllSpidersInSameIP == false && onlineGuestIp.GuestIds.Count > 0)
                        return false;

                    if (OnlineSettings.ShowSameIPCount <= onlineGuestIp.GuestIds.Count)
                        return false;

                    if (onlineGuestIp.GuestIds.Contains(guestID) == false)
                        onlineGuestIp.GuestIds.Add(guestID);
                }
                else
                {
                    onlineGuestIp = new OnlineGuestsInIp();
                    onlineGuestIp.IsSpider = isSpider;
                    onlineGuestIp.GuestIds.Add(guestID);

                    m_OnlineGuestsInIps.Add(newIP, onlineGuestIp);
                }
            }

            return true;
        }

        //private void RemoveGuestFromIP(string guestID, string ip)
        //{
        //    OnlineGuestsInIp onlineGuestIp;// = m_GuestIps.GetValue(ip);

        //    //if (onlineGuestIp != null)
        //    if (m_GuestIps.TryGetValue(ip, out onlineGuestIp))
        //    {

        //        lock (onlineGuestIp)
        //        {
        //            onlineGuestIp.GuestIds.Remove(guestID);

        //            if (onlineGuestIp.GuestIds.Count == 0)
        //            {
        //                lock (m_GuestIps_Locker)
        //                {
        //                    m_GuestIps.Remove(ip);
        //                }
        //            }
        //        }

        //    }
        //}

        public string ActionName(OnlineAction action)
        {
            switch (action)
            {
                case OnlineAction.CreateThread:
                    return "发表主题";
                case OnlineAction.ReplyThread:
                    return "回复主题";
                case OnlineAction.ViewIndexPage:
                    return "浏览首页";
                case OnlineAction.ViewThread:
                    return "浏览主题";
                case OnlineAction.ViewThreadList:
                    return "查看列表页";
                case OnlineAction.ViewSpace:
                    return "查看空间";
            }
            return "未知";
        }

        #endregion

        #region 修改在线状态

        public void Update(AuthUser my, RequestVariable request, OnlineAction action, int forumID, int threadID, string subject)
        {

            subject = StringUtil.CutString(subject, 20);



            int userID = my.UserID;
            string ip = request.IpAddress;

            bool addnew = false;

            //已经登录的用户
            if (userID > 0)
            {
                #region 已经登录的用户的处理

                OnlineMember onlineMember;
                if (m_OnlineMemberTable.TryGetValue(userID, out onlineMember) == false)
                {
                    string location = IPUtil.GetIpArea(ip);
                    RoleInOnline role = GetUserRoleInOnline(userID);
                    lock (m_OnlineMembers_Locker)
                    {
                        if (m_OnlineMemberTable.TryGetValue(userID, out onlineMember) == false)
                        {
                            #region 增加一个OnlineMember

                            DateTime now = DateTimeUtil.Now;

                            onlineMember = new OnlineMember();
                            onlineMember.UserID = userID;

                            onlineMember.Action = action;

                            onlineMember.Username = my.Username;
                            onlineMember.IsInvisible = my.IsInvisible;

                            onlineMember.IP = ip;
                            onlineMember.Location = location;

                            onlineMember.ForumID = forumID;
                            onlineMember.ThreadID = threadID;
                            onlineMember.ThreadSubject = subject;


                            onlineMember.Platform = request.Platform;
                            onlineMember.Browser = request.Browser;
                            onlineMember.CreateDate = now;
                            onlineMember.UpdateDate = now;
                            onlineMember.RoleSortOrder = role.SortOrder;
                            onlineMember.RoleIdentityIDString = my.RoleIdentityIDString;

                            m_OnlineMemberTable.Add(userID, onlineMember);
                            OnlineMemberCollection newOnlineMembers = new OnlineMemberCollection(m_OnlineMembers);
                            newOnlineMembers.Add(onlineMember);
                            m_OnlineMembers = newOnlineMembers;

                            if (forumID != 0)
                            {
                                lock (GetForumOnlineMemberLocker(forumID))
                                {
                                    //加入新的版块
                                    AddForumOnlineMember(forumID, onlineMember);
                                }
                            }

                            #endregion

                            addnew = true;
                        }
                    }
                }

                if (addnew == false)
                {
                    lock (onlineMember)
                    {
                        //ForumID为-1意味着其实已经删除，无需再更新
                        if (onlineMember.ForumID != -1)
                        {
                            #region 更新原来OnlineMember的值

                            onlineMember.Action = action;

                            onlineMember.Username = my.Username;
                            onlineMember.IsInvisible = my.IsInvisible;

                            if (my.RoleIdentityIDString != onlineMember.RoleIdentityIDString)
                            {
                                RoleInOnline role = GetUserRoleInOnline(userID);
                                onlineMember.RoleSortOrder = role.SortOrder;
                                onlineMember.RoleIdentityIDString = my.RoleIdentityIDString;
                            }

                            if (onlineMember.IP != ip)
                            {
                                onlineMember.IP = ip;
                                onlineMember.Location = IPUtil.GetIpArea(ip);
                            }

                            //-----------------

                            int oldForumID = onlineMember.ForumID;
                            onlineMember.ThreadID = threadID;
                            onlineMember.ThreadSubject = subject;

                            if (oldForumID != forumID)
                            {
                                if (oldForumID != 0 && forumID != 0)
                                {
                                    lock (GetForumOnlineMemberLocker(oldForumID))
                                    {
                                        lock (GetForumOnlineMemberLocker(forumID))
                                        {
                                            //从原来的版块移除
                                            RemoveForumOnlineMember(oldForumID, onlineMember);
                                            //加入新的版块
                                            AddForumOnlineMember(forumID, onlineMember);
                                        }
                                    }
                                }
                                else if (oldForumID != 0)
                                {
                                    lock (GetForumOnlineMemberLocker(oldForumID))
                                    {
                                        //从原来的版块移除
                                        RemoveForumOnlineMember(oldForumID, onlineMember);
                                    }
                                }
                                else if (forumID != 0)
                                {
                                    lock (GetForumOnlineMemberLocker(forumID))
                                    {
                                        //加入新的版块
                                        AddForumOnlineMember(forumID, onlineMember);
                                    }
                                }
                                onlineMember.ForumID = forumID;
                            }


                            onlineMember.Platform = request.Platform;
                            onlineMember.Browser = request.Browser;
                            onlineMember.UpdateDate = DateTimeUtil.Now;

                            #endregion
                        }
                        else
                            LogHelper.CreateDebugLog("OnlineMember线程同步监视1");
                    }
                }

                #endregion

                //如果是已登陆状态，但客户端仍然提交了GuestID，则把这个GuestID移除
                //if (my.MachineIDIsNew == false)

                string guestID = my.GuestID;

                if (guestID != null)
                {
                    RemoveOnlineGuest(guestID);
                }
            }
            else
            //尚未登录的用户
            {
                //TODO : 暂不统计游客在线情况，因为可能存在溢出，下个版本改进



                #region 尚未登录的用户的处理


                bool isSpider = request.IsSpider;
                string guestID = my.BuildGuestID();

                OnlineGuest onlineGuest;
                if (m_OnlineGuestTable.TryGetValue(guestID, out onlineGuest) == false)
                {
                    string location = IPUtil.GetIpArea(ip);

                    lock (m_OnlineGuests_Locker)
                    {
                        //假如游客列表中还没有这个游客
                        if (m_OnlineGuestTable.TryGetValue(guestID, out onlineGuest) == false)
                        {
                            bool success = UpdateOnlineGuestIP(guestID, null, ip, isSpider);// AddGuestToIP(guestID, ip, request.IsSpider);
                            //bool success = true;

                            if (success)
                            {
                                #region 增加一个OnlineGuest

                                DateTime now = DateTimeUtil.Now;

                                onlineGuest = new OnlineGuest();
                                onlineGuest.GuestID = my.GuestID;
                                onlineGuest.IsSpider = isSpider;

                                onlineGuest.Action = action;

                                onlineGuest.IP = ip;
                                onlineGuest.Location = location;


                                onlineGuest.Platform = request.Platform;
                                onlineGuest.Browser = request.Browser;
                                onlineGuest.CreateDate = now;
                                onlineGuest.UpdateDate = now;

                                onlineGuest.ForumID = forumID;
                                onlineGuest.ThreadID = threadID;
                                onlineGuest.ThreadSubject = subject;

                                m_OnlineGuestTable.Add(guestID, onlineGuest);
                                OnlineGuestCollection newOnlineGuests = new OnlineGuestCollection(m_OnlineGuests);
                                newOnlineGuests.Add(onlineGuest);
                                m_OnlineGuests = newOnlineGuests;

                                if (forumID != 0)
                                {
                                    lock (GetForumOnlineGuestLocker(forumID))
                                    {
                                        //加入新的版块
                                        AddForumOnlineGuest(forumID, onlineGuest);
                                    }
                                }

                                #endregion

                                addnew = true;
                            }
                            else
                                return;
                        }
                    }
                }

                //如果只是更新原来的值，那么开始更新
                if (addnew == false)
                {
                    lock (onlineGuest)
                    {
                        //ForumID为-1表示其实这个对象已经从在线列表中移除了
                        if (onlineGuest.ForumID != -1)
                        {
                            #region 更新原来OnlineGuest的值

                            onlineGuest.Action = action;

                            //如果这个游客仅仅发生了IP变化，那么要更新IP表
                            if (onlineGuest.IP != ip)
                            {
                                UpdateOnlineGuestIP(guestID, onlineGuest.IP, ip, isSpider);

                                //RemoveGuestFromIP(guestID, onlineGuest.IP);
                                onlineGuest.IsSpider = isSpider;
                                onlineGuest.IP = ip;
                                onlineGuest.Location = IPUtil.GetIpArea(ip);
                                //AddGuestToIP(guestID, onlineGuest.IP, request.IsSpider);
                            }

                            //----------------------

                            int oldForumID = onlineGuest.ForumID;

                            if (oldForumID != forumID)
                            {
                                if (oldForumID != 0 && forumID != 0)
                                {
                                    lock (GetForumOnlineGuestLocker(oldForumID))
                                    {
                                        lock (GetForumOnlineGuestLocker(forumID))
                                        {
                                            //从原来的版块移除
                                            RemoveForumOnlineGuest(oldForumID, onlineGuest);
                                            //加入新的版块
                                            AddForumOnlineGuest(forumID, onlineGuest);
                                        }
                                    }
                                }
                                else if (oldForumID != 0)
                                {
                                    lock (GetForumOnlineMemberLocker(oldForumID))
                                    {
                                        //从原来的版块移除
                                        RemoveForumOnlineGuest(oldForumID, onlineGuest);
                                    }
                                }
                                else if (forumID != 0)
                                {
                                    lock (GetForumOnlineMemberLocker(forumID))
                                    {
                                        //加入新的版块
                                        AddForumOnlineGuest(forumID, onlineGuest);
                                    }
                                }

                                onlineGuest.ForumID = forumID;
                            }


                            onlineGuest.Platform = request.Platform;
                            onlineGuest.Browser = request.Browser;
                            onlineGuest.UpdateDate = DateTimeUtil.Now;

                            onlineGuest.ThreadID = threadID;
                            onlineGuest.ThreadSubject = subject;
                            #endregion
                        }
                        else
                            LogHelper.CreateDebugLog("OnlineGuest线程同步监视1");
                    }

                }

                #endregion
            }


        }

        /// <summary>
        /// 设置当前用户的隐身状态
        /// </summary>
        /// <param name="my"></param>
        /// <param name="isInvisible"></param>
        public void Update(AuthUser my, bool isInvisible)
        {
            int userID = my.UserID;

            if (userID > 0)
            {
                OnlineMember onlineMember;

                if (m_OnlineMemberTable.TryGetValue(userID, out onlineMember))
                    onlineMember.IsInvisible = isInvisible;
            }
        }

        public void Remove(AuthUser my)
        {
            int userID = my.UserID;

            //已经登录的用户
            if (userID > 0)
            {
                OnlineMember onlineMember;
                if (RemoveOnlineMember(userID, out onlineMember))
                    UserBO.Instance.UpdateOnlineTime(onlineMember);
            }
            else
            {
                string guestID = my.GuestID;

                if (guestID != null)
                    RemoveOnlineGuest(my.GuestID);
            }
        }

        /// <summary>
        /// 当更新在线角色图标设置时 要调用此方法
        /// </summary>
        public void UpdateUsersOnlineRole()
        {
            OnlineMemberCollection members = GetAllOnlineMembers();

            UserCollection users = UserBO.Instance.GetUsers(members.GetKeys());

            foreach (User user in users)
            {
                RoleInOnline roleInOnline = GetUserRoleInOnline(user.UserID);
                OnlineMember member = GetOnlineMember(user.UserID);
                member.RoleSortOrder = roleInOnline.SortOrder;
            }
        }


        ///TODO : 应该不依赖原来的 m_GuestIps，而是直接根据 AllOnlineGuests 生成新的 m_GuestIps
        /// <summary>
        /// 保存在线列表设置的时候 调用此方法
        /// </summary>
        /// <param name="updateNormal">是否检查更新一般的</param>
        /// <param name="updateSpider">是否检查更新蜘蛛的</param>
        public void UpdateSameIpOnlineGuest(bool updateNormal, bool updateSpider)
        {
            if (updateNormal == false && updateSpider == false)
                return;

            //lock (m_OnlineGuestsInIps_Locker)
            //{
            //    //OnlineGuestIpCollection ips = new OnlineGuestIpCollection(m_GuestIps);

            //    foreach (OnlineGuestsInIp onlineGuestIp in m_OnlineGuestsInIps.Values)
            //    {
            //        //OnlineGuestIp temp = ips.GetValue(onlineGuestIp.Ip);
            //        //if (temp == null)
            //        //    continue;

            //        if (onlineGuestIp.IsSpider && updateSpider && OnlineSettings.IsShowAllSpidersInSameIP == false)
            //        {
            //            lock(onlineGuestIp)
            //            {
            //                if (onlineGuestIp.GuestIds.Count > 1)//只能保留一个 其余的要移除掉
            //                {
            //                    List<string> guestIDs = new List<string>();

            //                    int i = 0;
            //                    foreach (string guestID in onlineGuestIp.GuestIds)
            //                    {
            //                        if (i == 0)
            //                        {
            //                            guestIDs.Add(guestID);
            //                            continue;
            //                        }

            //                        //TODO:移除 OnlineGuest
            //                        RemoveOnlineGuest(guestID);

            //                        i++;
            //                    }

            //                    onlineGuestIp.GuestIds = guestIDs;
            //                }
            //            }
            //        }

            //        if (updateNormal)
            //        {
            //            if (onlineGuestIp.GuestIds.Count > OnlineSettings.ShowSameIPCount) //只能保留 ShowSameIPCount 个 其余的要移除掉
            //            {
            //                lock (onlineGuestIp)
            //                {

            //                    List<string> guestIDs = new List<string>();

            //                    int i = 0;
            //                    foreach (string guestID in onlineGuestIp.GuestIds)
            //                    {
            //                        if (i < OnlineSettings.ShowSameIPCount)
            //                        {
            //                            guestIDs.Add(guestID);
            //                            continue;
            //                        }

            //                        //TODO:移除 OnlineGuest
            //                        RemoveOnlineGuest(guestID);

            //                        i++;
            //                    }

            //                    onlineGuestIp.GuestIds = guestIDs;

            //                }
            //            }
            //        }
            //    }

                //m_GuestIps = ips;
            //}
        }

        public void RebuildOnlineData()
        {
            
        }

        #endregion

        #region 用于后台线程使用的定期清理过期在线用户的方法

        public void ClearExpiredData()
        {
            //算出总在线人数（包括会员和游客）
            int totalOnline = m_OnlineMembers.Count + m_OnlineGuests.Count;

            //现在是史上最高在线，记录之
            if (VarsManager.Stat.MaxOnlineCount < totalOnline)
                VarsManager.UpdateMaxOnline(totalOnline, DateTimeUtil.Now);

            UserBO userbo = UserBO.Instance;

            //============================================
            //开始处理会员在线列表
            //============================================

            //超时的时间，在这个时间后都没有活动过的，属于已经超时
            DateTime expiresDateTime = DateTimeUtil.Now.AddMinutes(0 - OnlineSettings.OverTime);

            List<List<UserOnlineInfo>> allOnlineInfos = new List<List<UserOnlineInfo>>();
            List<UserOnlineInfo> onlineInfos = new List<UserOnlineInfo>();

            //循环所有的在线会员
            foreach (OnlineMember member in m_OnlineMembers)
            {
                //收集超时的会员，以备后续更新这些用户的在线时长，并同时从列表中移除
                if (member.UpdateDate < expiresDateTime)
                {
                    #region 收集要更新的在线时长信息

                    //收集应该被移除的userID、在线时间和最后更新的时间
                    UserOnlineInfo info = new UserOnlineInfo();
                    info.UserID = member.UserID;
                    //在线时间只记创建时间和最后更新时间的时间长度
                    info.OnlineMinutes = (int)(member.UpdateDate - member.CreateDate).TotalMinutes;
                    info.UpdateDate = member.UpdateDate;

                    onlineInfos.Add(info);

                    if (onlineInfos.Count >= 300)
                    {
                        allOnlineInfos.Add(onlineInfos);
                        onlineInfos = new List<UserOnlineInfo>();
                    }

                    #endregion

                    //从在线列表中移除
                    RemoveOnlineMember(member.UserID);
                }
            }

            //将最后一次循环收集到的在线时长信息加入总列表
            if (onlineInfos.Count > 0)
                allOnlineInfos.Add(onlineInfos);

            //============================================
            //开始处理游客在线列表
            //============================================

            //更新一下超时的时间，因为之前的循环操作已经花掉一些时间，不更新将造成较大延迟
            expiresDateTime = DateTimeUtil.Now.AddMinutes(0 - OnlineSettings.OverTime);

            //lock (m_GuestIps_Locker)
            //{
            foreach (OnlineGuest guest in m_OnlineGuests)
            {
                if (guest.UpdateDate < expiresDateTime)
                {
                    //OnlineGuestsInIp tempOnlineGuestIp;
                    //if (m_OnlineGuestsInIps.TryGetValue(guest.IP, out tempOnlineGuestIp))
                    //{
                    //    tempOnlineGuestIp.GuestIds.Remove(guest.GuestID);

                    //    if (tempOnlineGuestIp.GuestIds.Count == 0)
                    //        m_OnlineGuestsInIps.Remove(guest.IP);
                    //}

                    //移除 OnlineGuest
                    RemoveOnlineGuest(guest.GuestID);
                }
            }
            //}

            //=====================================
            //开始对数据库的操作，主要是更新用户的在线时长

            try
            {
                foreach (List<UserOnlineInfo> infos in allOnlineInfos)
                {
                    userbo.UpdateOnlineTime300(infos);
                }

                userbo.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.DayOnlineTime, ActiveUserType.WeekOnlineTime });
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }
        }

        #endregion

        public void Backup()
        { }

        public void Restore()
        { }

        #region 获取存储在在线列表中的临时数据

        public string GetGuestTempData(string guestID, string key)
        {
            if (guestID == string.Empty)
                return null;

            OnlineGuest onlineGuest;

            if (m_OnlineGuestTable.TryGetValue(guestID, out onlineGuest))
            {
                return onlineGuest.TempDataBox[key];
            }

            return null;
        }

        public void SetGuestTempData(string guestID, string key, string value)
        {
            if (guestID == string.Empty)
                return;

            OnlineGuest onlineGuest;

            if (m_OnlineGuestTable.TryGetValue(guestID, out onlineGuest))
            {
                onlineGuest.TempDataBox[key] = value;
            }
        }

        #endregion

        #region 获得列表

        /// <summary>
        /// 获取整个网站的在线已注册用户
        /// </summary>
        /// <returns></returns>
        public OnlineMemberCollection GetAllOnlineMembers()
        {
            return m_OnlineMembers;
        }

        /// <summary>
        /// 获取指定板块的在线已注册用户
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public OnlineMemberCollection GetOnlineMembers(int forumID)
        {
            OnlineMemberCollection onlineMembers;

            if (m_ForumOnlineMembers.TryGetValue(forumID, out onlineMembers))
                return onlineMembers;

            return new OnlineMemberCollection();
        }

        /// <summary>
        /// 获取整个网站的在线游客
        /// </summary>
        /// <returns></returns>
        public OnlineGuestCollection GetAllOnlineGuests()
        {
            return m_OnlineGuests;
        }

        /// <summary>
        /// 获取指定板块的在线游客
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public OnlineGuestCollection GetOnlineGuests(int forumID)
        {
            OnlineGuestCollection onlineGuests;

            if (m_ForumOnlineGuests.TryGetValue(forumID, out onlineGuests))
                return onlineGuests;

            return new OnlineGuestCollection();
        }

        #endregion

        #region 判断在线状态

        /// <summary>
        /// 判断指定的用户是否在线（不包括隐身）
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsOnline(int userID)
        {
            if (userID <= 0)
                return false;

            OnlineMember onlineMember;

            if (m_OnlineMemberTable.TryGetValue(userID, out onlineMember))
                return !onlineMember.IsInvisible;

            return false;
        }

        /// <summary>
        /// 判断指定的用户是否在线或隐身
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsOnlineOrInvisible(int userID)
        {
            if (userID <= 0)
                return false;

            return m_OnlineMemberTable.ContainsKey(userID);
        }

        /// <summary>
        /// 判断指定的用户是否隐身
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsInvisible(int userID)
        {
            if (userID <= 0)
                return false;

            OnlineMember onlineMember;

            if (m_OnlineMemberTable.TryGetValue(userID, out onlineMember))
                return onlineMember.IsInvisible;

            return false;
        }

        #endregion

        public OnlineMember GetOnlineMember(int userID)
        {
            OnlineMember onlineMember;

            if (m_OnlineMemberTable.TryGetValue(userID, out onlineMember))
                return onlineMember;

            return onlineMember;
        }

        public OnlineSettings OnlineSettings
        {
            get { return AllSettings.Current.OnlineSettings; }
        }

        #region  角色图标

        public RoleInOnline GetEveryoneRole()
        {
            RoleInOnline roleInOnline;
            if (AllSettings.Current.OnlineSettings.RolesInOnline.TryGetValue(Role.Users.RoleID, out roleInOnline))
                return roleInOnline;

            return null;
        }

        public RoleInOnline GetGuestRole()
        {
            RoleInOnline roleInOnline;
            if (AllSettings.Current.OnlineSettings.RolesInOnline.TryGetValue(Role.Guests.RoleID, out roleInOnline))
                return roleInOnline;

            return null;
        }

        public RoleInOnline GetUserRoleInOnline(int userID)
        {

            RoleInOnlineCollection rolesInOnline = AllSettings.Current.OnlineSettings.RolesInOnline;

            if (userID == 0)
            {
                return rolesInOnline.GetValue(Role.Guests.RoleID);
            }
            else
            {
                User user = UserBO.Instance.GetUser(userID);

                if (user == null)
                    return rolesInOnline.GetValue(Role.Users.RoleID);

                string roleIDString = user.Roles.GetJoinedIds();

                if (roleIDString == Role.Users.RoleID.ToString())
                {
                    return rolesInOnline.GetValue(Role.Users.RoleID);
                }
                else
                {
                    if (!string.IsNullOrEmpty(roleIDString))
                    {
                        roleIDString = "," + roleIDString + ",";

                        foreach (RoleInOnline roleInOnline in rolesInOnline)
                        {
                            if (roleIDString.Contains("," + roleInOnline.RoleID.ToString() + ","))
                                return roleInOnline;
                        }
                    }
                }

                //在不是游客的情况下，到了这里还没返回，oh my god，直接返回every图标吧
                return rolesInOnline.GetValue(Role.Users.RoleID);
            }

        }

        public RoleInOnline GetRoleInOnline(Guid roleID)
        {
            RoleInOnline roleInOnline;
            if (AllSettings.Current.OnlineSettings.RolesInOnline.TryGetValue(roleID, out roleInOnline))
            {
                return roleInOnline;
            }
            else
                return null;
        }

        #endregion
    }

    public struct UserOnlineInfo
    {
        public int UserID;
        public DateTime UpdateDate;
        public int OnlineMinutes;
    }
}