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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;

#if !Passport
using MaxLabs.bbsMax.PassportServerInterface;
#endif
namespace MaxLabs.bbsMax
{
    public class SystemNotifyProvider
    {
        private static object locker = new object();
        private SystemNotifyProvider() { }

        static SystemNotifyProvider() {
            Instance = new SystemNotifyProvider();
        }

        public long NotifyVersion { get; private set; }
        private SystemNotifyCollection m_AllSystemNotifys;
        public SystemNotifyCollection AllSystemNotifys
        {
            get
            {
//#if !Passport
//                PassportClientSettings settings = Globals.PassportClient;
//                if (settings.EnablePassport)
//                {
//                    SystemNotifyProxy[] temp = settings.PassportService.Notify_SystemNotifies();

//                    SystemNotifyCollection result = new SystemNotifyCollection();
//                    foreach (SystemNotifyProxy proxy in temp)
//                    {
//                        result.Add(NotifyBO.Instance.GetSystemNotify(proxy));
//                    }

//                    return result;
//                }
//                else
//#endif
                {
                    if (m_AllSystemNotifys == null)
                    {
                        lock (locker)
                        {
                            if (m_AllSystemNotifys == null)
                            {
                                SystemNotifyCollection temp = NotifyBO.Instance.GetSystemNotifys();
                                m_AllSystemNotifys = temp;
                                NotifyVersion = DateTimeUtil.Now.Ticks;
                            }
                        }
                    }
                    return m_AllSystemNotifys;
                }
            }
            private set
            {
                m_AllSystemNotifys = value;
            }
        }

        public static void Update()
        {
            Instance.AllSystemNotifys = null;
        }

        private static SystemNotifyProvider Instance;

        /// <summary>
        /// 获取用户的所有系统通知，包括已读未读的。
        /// </summary>
        /// <param name="operateUser">操作者</param>
        /// <param name="unreadIDs">返回集合内未读的系统通知编号</param>
        /// <returns></returns>
        public static SystemNotifyCollection GetMyAllSystemNotifies( AuthUser operateUser,out  List<int> unreadIDs )
        {
            SystemNotifyCollection allNotifys = Instance.AllSystemNotifys;

            SystemNotifyCollection myNotifys = new SystemNotifyCollection();
            UserRoleCollection myRoles = operateUser.Roles;
            unreadIDs = new List<int>();
            string myUserIDTag = string.Concat(",", operateUser.UserID, ",");

            foreach (SystemNotify notify in allNotifys)
            {
                if (!notify.Available)
                    continue;

                if (notify.ReceiveUserIDs.Contains(operateUser.UserID))
                {
                    if (!string.IsNullOrEmpty(notify.ReadUserIDs) && !notify.ReadUserIDs.Contains(myUserIDTag))//是否已读
                    {
                        unreadIDs.Add(notify.NotifyID);
                    }
                    myNotifys.Add(notify);
                }
                else
                {
                    foreach (UserRole ur in myRoles)
                    {
                        if (notify.ReceiveRoles.Contains(ur.RoleID))
                        {
                            if (!string.IsNullOrEmpty(notify.ReadUserIDs) && !notify.ReadUserIDs.Contains(myUserIDTag))//是否已读
                            {
                                unreadIDs.Add(notify.NotifyID);
                            }
                            myNotifys.Add(notify);
                            break;
                        }
                    }
                }
            }

            return myNotifys;
        }
        
        /// <summary>
        /// 获取用户的未读系统通知
        /// </summary>
        /// <param name="operatorUser"></param>
        public static void GetMySystemNotifys(AuthUser operatorUser)
        {
            int maxID = 0;
            SystemNotifyCollection allNotifys = Instance.AllSystemNotifys;
            if (operatorUser.SystemNotifyVersion == Instance.NotifyVersion)
                return;
            
            SystemNotifyCollection myNotifys=new SystemNotifyCollection();
            UserRoleCollection myRoles = operatorUser.Roles;

            string myUserIDTag = string.Concat(",", operatorUser.UserID, ",");

            foreach (SystemNotify notify in allNotifys)
            {
                if (maxID < notify.NotifyID)
                    maxID = notify.NotifyID;
                if (!notify.Available)
                    continue;

                if (notify.NotifyID <= operatorUser.LastReadSystemNotifyID)//
                    continue;

                if (!string.IsNullOrEmpty(notify.ReadUserIDs) && notify.ReadUserIDs.Contains(myUserIDTag))//是否已读
                    continue;
                

                if (notify.ReceiveUserIDs.Contains(operatorUser.UserID))
                {
                    myNotifys.Add(notify);
                }
                else
                {
                    foreach (UserRole ur in myRoles)
                    {
                        if (notify.ReceiveRoles.Contains(ur.RoleID))
                        {
                            myNotifys.Add(notify);
                            break;
                        }
                    }
                }
            }

            operatorUser.SystemNotifys = myNotifys;
            operatorUser.SystemNotifyVersion = Instance.NotifyVersion;
            //user.LastReadSystemNotifyID = maxid;
            //UserBO.Instance.UpdateMaxSystemNotifyID(user.UserID, maxid);               
            //Instance.AllSystemNotifys
        }

        public static SystemNotifyCollection CurrentSystemNotifys
        {
            get
            {
                return Instance.AllSystemNotifys;
            }
        }

        public static void IgnoreNotify(int userID, int notifyID)
        {
            SystemNotify notify = null;
            notifyID = Math.Abs(notifyID);
            foreach (SystemNotify sn in CurrentSystemNotifys)
            {
                if (sn.NotifyID == notifyID)
                {
                    notify = sn;
                    break;
                }
            }

            if (notify != null)
            {
                string s = notify.ReadUserIDs;
                if (string.IsNullOrEmpty(s)) s = string.Empty;
                if (!s.StartsWith(",")) s = "," + s;
                
                AuthUser user = UserBO.Instance.GetUserFromCache<AuthUser>(userID);
                if (user != null)
                {
                    user.SystemNotifyVersion = DateTimeUtil.Now.Ticks;//刷新用户的系统通知列表
                    s += user.UserID + ",";
                }
                notify.ReadUserIDs = s;
                NotifyDao.Instance.SetSystemNotifyReadUserIDs(notify.NotifyID, s);
            }
        }
    }
}