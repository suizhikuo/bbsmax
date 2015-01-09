//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Common;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

#if !Passport
using MaxLabs.bbsMax.PassportServerInterface;
using System.Threading;
#endif
namespace MaxLabs.bbsMax
{
    public partial class NotifyBO : BOBase<NotifyBO>
    {

        #region events

        public static event UserNotifyCountChanged OnUserNotifyCountChanged;
        public static event UserIgnoreSystemNotify OnUserIgnoreSystemNotify;

        public static event SystemNotifyUpdated OnSystemNotifyUpdated;
        public static event SystemNotifyCreate OnSystemNotifyCreated;
        public static event SystemnotifyDeleted OnSystemNotifyDeleted;

        #endregion

        #region passport client only

        public void Client_SetUserUnreadNotifies( UnreadNotifies unreadNotifies )
        {
            AuthUser user = UserBO.Instance.GetUserFromCache<AuthUser>(unreadNotifies.UserID);
            if (user == null)
                return;
            user.UnreadNotify = unreadNotifies;
        }

        #endregion

        const string cacheKey_UserNotifyRoot = "notify/user/{0}";
        const string cacheKey_UserNotifyType = cacheKey_UserNotifyRoot + "/type/{1}";
        const string cacheKey_pagedNotify = cacheKey_UserNotifyType + "/pagesize/{2}/pagenumber/{3}";

        public BackendPermissions ManagePermission
        {
            get { return AllSettings.Current.BackendPermissions; }
        }

        public NotifyCollection AdminGetNotifiesBySearch(int operatorUserID, AdminNotifyFilter notifyFilter, int pageNumber)
        {
            IEnumerable<Guid> excludeRoleIds = ManagePermission.GetNoPermissionTargetRoleIds(operatorUserID, BackendPermissions.ActionWithTarget.Manage_Notify);

            return NotifyDao.Instance.AdminGetNotifiesBySearch( notifyFilter,  pageNumber, excludeRoleIds);            
        }

        public Notify GetNotify(int operatorID, int notifyID)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return null;
            }

            if (notifyID <= 0)
            {
                ThrowError(new InvalidParamError("notifyID"));
                return null;
            }
#if !Passport
            PassportClientConfig config = Globals.PassportClient;
            if (config.EnablePassport)
            {
                NotifyProxy notify = config.PassportService.Notify_GetNotify(operatorID, notifyID);
               

                if (notify == null)
                    return null;
                //if(T is friend
                return GetNotify(notify); 
            }
            else
#endif
            {
                Notify notify = NotifyDao.Instance.GetNotify<Notify>(null, notifyID, false);

                if (notify != null)
                {
                    if (notify.UserID == operatorID)
                        return notify;

                    if (ManagePermission.Can(operatorID, BackendPermissions.ActionWithTarget.Manage_Notify, notify.UserID))
                    {
                        return notify;
                    }
                }

                return null;
            }
        }

        public bool RegisterNotifyType(string typeName, bool keep, string description, out NotifyType type)
        {
            type = null;
            if (string.IsNullOrEmpty(typeName))
            {
                ThrowError(new CustomError("typename", "通知类型名称不能空"));
                return false;
            }

            bool result = NotifyDao.Instance.RegisterNotifyType(typeName, keep, description, out type);

            if (result == false)
            {
                ThrowError(new CustomError("typename", "类型已经存在"));
                return false;
            }
            else
            {
                m_AllNotifyTypes = null;
            }


            return result;
        }

#if !Passport
        private static Service PassportService
        {
            get
            {
                return Globals.PassportClient.PassportService;
            }
        }

#endif

        private static object Locker = new object();
        private static NotifyTypeCollection m_AllNotifyTypes;
        public static NotifyTypeCollection AllNotifyTypes
        {
            get
            {
                if (m_AllNotifyTypes == null)
                {
                    lock (Locker)
                    {
                        if (m_AllNotifyTypes == null)
                        {
#if !Passport
                            if (Globals.PassportClient.EnablePassport)
                            {
                                NotifyTypeProxy[] types = PassportService.Notify_GetAllNotifyTypes();


                                m_AllNotifyTypes = new NotifyTypeCollection(  );

                                foreach (NotifyTypeProxy nt in types)
                                {
                                    NotifyType type = new NotifyType();
                                    type.TypeID = nt.TypeID;
                                    type.TypeName = nt.TypeName;
                                    type.Keep = nt.Keep;
                                    m_AllNotifyTypes.Add(type);
                                }
                            }
                            else
#endif
                            {
                                m_AllNotifyTypes = NotifyDao.Instance.LoadAllNotifyType();
                            }

                        }
                    }
                }
                return m_AllNotifyTypes;
            }
        }

        public MaxLabs.bbsMax.Entities.NotifyType GetNotifyType(int typeID)
        {
            foreach (MaxLabs.bbsMax.Entities.NotifyType t in AllNotifyTypes)
            {
                if (t.TypeID == typeID)
                    return t;
            }

            return null;
        }

        public MaxLabs.bbsMax.Entities.NotifyType GetNotifyType(string typeName)
        {
            foreach (MaxLabs.bbsMax.Entities.NotifyType t in AllNotifyTypes)
            {
                if (t.TypeName == typeName)
                    return t;
            }

            return null;
        }

        /// <summary>
        /// 自动清除
        /// </summary>
        /// <param name="days"></param>
        public void ClearNotify()
        {
            JobDataClearMode mode = AllSettings.Current.NotifySettings.DataClearMode;
            int saveDays = AllSettings.Current.NotifySettings.NotifySaveDays;
            int saveRows = AllSettings.Current.NotifySettings.NotifySaveRows;

            if (mode == JobDataClearMode.Disabled)
                return;

            if (mode == JobDataClearMode.ClearByDay && saveDays <= 0)
                return;
            if (mode == JobDataClearMode.ClearByRows && saveRows <= 0)
                return;
            if (mode == JobDataClearMode.CombinMode && saveRows <= 0 && saveDays == 0)
                return;


            NotifyDao.Instance.ClearNotify(saveDays, saveRows, mode);
        }

        public NotifyCollection GetTopNotifys(int userID, int count, int type)
        {
#if !Passport 
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                NotifyProxy[] notifys = new NotifyProxy[0];
                try
                {
                    notifys = settings.PassportService.Notify_GetTopNotify(userID, count, type);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                }

                NotifyCollection result = new NotifyCollection();
                foreach (NotifyProxy proxy in notifys)
                {
                    result.Add(GetNotify(proxy));
                }

                return result;
            }
            else
#endif
            {
                NotifyCollection notifys;
                //string key = string.Format(cacheKey_UserNotifyType, userID, type, count, 1);
                //if (!CacheUtil.TryGetValue<NotifyCollection>(key, out notifys))
                //{
                    notifys = NotifyDao.Instance.GetTopNotifys(userID, type, count);
                 //   CacheUtil.Set<NotifyCollection>(key, notifys);
                //}
                return notifys;
            }
        }

        /// <summary>
        /// 获取指定用户指定类型的所有通知
        /// </summary>
        /// <param name="notifyType">指定类型</param>
        /// <returns>指定用户指定类型的所有通知集合</returns>
        public NotifyCollection GetNotifiesByType(int userID, int notifyType, int pageSize, int pageNumber, ref int? count)
        {
            if (userID <= 0)
            {
                ThrowError(new NotLoginError());
                return new NotifyCollection();
            }

            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

            if (HasUnCatchedError)
            {
                return new NotifyCollection();
            }

#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                NotifyProxy[] proxys = settings.PassportService.Notify_GetNotifiesByType(userID, notifyType, pageSize, pageNumber, ref count);
                
               
                NotifyCollection notifies = new NotifyCollection();
                foreach (NotifyProxy proxy in proxys)
                {
                    notifies.Add(GetNotify(proxy));
                }

                return notifies;
            }
            else
#endif
            {
                NotifyCollection notifys;
                string cacheKey = string.Format(cacheKey_pagedNotify, userID, notifyType, pageSize, pageNumber);
                if (!CacheUtil.TryGetValue<NotifyCollection>(cacheKey, out notifys))
                {
                    notifys = NotifyDao.Instance.GetNotifiesByType(userID, notifyType, pageSize, pageNumber, ref count);
                    CacheUtil.Set<NotifyCollection>(cacheKey, notifys);
                }
                count = notifys.TotalRecords;
                return notifys;
            }
        }

#if !Passport
        private Notify GetNotify(NotifyProxy proxy)
        {
            if (proxy == null)
                return null;

            Notify notify = new Notify();

            notify.NotifyID = proxy.NotifyID;
            notify.Content = proxy.Content;
            //notifyProxy.Url = notify.Url;
            notify.UserID = proxy.UserID;

            foreach (NotifyActionProxy action in proxy.Actions)
            {
                notify.Actions.Add(new NotifyAction(action.Title, action.Url, action.IsDialog));
            }

            notify.TypeName = proxy.TypeName;
            notify.ClientID = proxy.ClientID;
            notify.TypeID = proxy.TypeID;
            notify.Keyword = proxy.Keyword;
            notify.IsRead = proxy.IsRead;
            notify.UpdateDate = proxy.UpdateDate;
            notify.CreateDate = proxy.PostDate;
            notify.DataTable = new StringTable();

            foreach (StringKeyValueProxy item in proxy.DataTable)
            {
                notify.DataTable.Add(item.Key, item.Value);
            }

            return notify;
        }
#endif

        /// <summary>
        /// 获取所有通知
        /// </summary>
        /// <returns>返回所有通知集合</returns>
        public NotifyCollection GetAllNotifies(int pageSize, int pageNumber, ref int? count)
        {
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                NotifyProxy[] notifys = settings.PassportService.Notify_GetAllNotifies(pageSize, pageNumber, ref count);


                NotifyCollection result = new NotifyCollection();
                foreach (NotifyProxy proxy in notifys)
                {
                    result.Add(GetNotify(proxy));
                }

                return result;
            }
            else
#endif
            {
                pageNumber = pageNumber <= 0 ? 1 : pageNumber;
                pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

                if (HasUnCatchedError)
                {
                    return new NotifyCollection();
                }

                return NotifyDao.Instance.GetNotifies(null, pageSize, pageNumber, ref count);
            }
        }

        /// <summary>
        /// 搜索通知
        /// </summary>
        public NotifyCollection GetNotifiesBySearch(AdminNotifyFilter notifyFilter, int pageNumber)
        {
            if (SafeMode)
            {
                if (!IsExecutorLogin)
                {
                    ThrowError(new NotLoginError());
                    return new NotifyCollection();
                }
            }

            if (notifyFilter == null)
            {
                notifyFilter = new AdminNotifyFilter();
            }
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            return NotifyDao.Instance.GetNotifiesBySearch(notifyFilter, pageNumber);
        }

        #region 系统通知部分

        public SystemNotifyCollection GetUserSystemNotifies(AuthUser operateUser,out List<int> unreadIDs)
        {
            SystemNotifyCollection results = SystemNotifyProvider.GetMyAllSystemNotifies(operateUser, out unreadIDs);
            return results;
        }

        /// <summary>
        /// 获取系统通知列表
        /// </summary>
        /// <returns></returns>
        public SystemNotifyCollection GetSystemNotifys()
        {
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                SystemNotifyProxy[] proxys =   settings.PassportService.Notify_GetSystemNotifys();
              
                SystemNotifyCollection result = new SystemNotifyCollection();
                foreach (SystemNotifyProxy proxy in proxys)
                {
                    result.Add(ProxyToSystemNotify(proxy));
                }
                return result;
            }
            else
#endif
            {
                return NotifyDao.Instance.GetSystemNotifies();
            }
        }

        public SystemNotify GetSystemNotify(int notifyID)
        {
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                SystemNotifyProxy proxy =  settings.PassportService.Notify_GetSystemNotify(notifyID);

                return ProxyToSystemNotify(proxy);
            }
            else
#endif
            {
                return NotifyDao.Instance.GetSystemNotify(notifyID);
            }
        }

#if !Passport

        public SystemNotify ProxyToSystemNotify(SystemNotifyProxy proxy)
        {
            if (proxy == null)
                return null;

            SystemNotify result = new SystemNotify();

            foreach (NotifyActionProxy action in proxy.Actions)
            {
                result.Actions.Add(new NotifyAction(action.Title, action.Url, action.IsDialog));
            }

            result.BeginDate = proxy.BeginDate;
            result.ClientID = proxy.ClientID;
            result.Content = proxy.Content;
            result.CreateDate = proxy.CreateDate;

            result.DataTable = new StringTable();

            foreach (StringKeyValueProxy keyValue in proxy.DataTable)
            {
                result.DataTable.Add(keyValue.Key, keyValue.Value);
            }

            result.DispatcherID = proxy.DispatcherID;
            result.DispatcherIP = proxy.DispatcherIP;
            result.EndDate = proxy.EndDate;
            result.IsRead = proxy.IsRead;
            result.Keyword = proxy.Keyword;
            result.NotifyID = proxy.NotifyID;
            result.CreateDate = proxy.PostDate;
            result.ReadUserIDs = proxy.ReadUserIDs;
            result.ReceiveUserIDs = new List<int>(proxy.ReceiveUserIDs);
            result.ReceiveRoles = new List<Guid>(proxy.ReceiveRoles);
            result.Subject = proxy.Subject;
            result.TypeID = proxy.TypeID;
            result.TypeName = proxy.TypeName;
            result.UpdateDate = proxy.UpdateDate;
            result.UserID = proxy.UserID;

            return result;
        }

#endif

        private bool ValidateSystemNotifyData(string Content, IEnumerable<Guid> receiveRoles, IEnumerable<int> receiveUserIDs, DateTime beginDate, DateTime endDate)
        {
            bool ok = true;

            if (string.IsNullOrEmpty(Content))
            {
                ThrowError(new SystemNotifyContentEmptyError("content"));
                ok = false;
            }

            if (Content.Length > 2000)
            {
                ThrowError(new SystemNotifyContentOverflow("content"));
                ok = false;
            }

            if (endDate <= DateTimeUtil.Now)
            {
                ThrowError(new SystemNotifyEndDateError("enddate"));
                ok = false;
            }

            if (!ValidateUtil.HasItems<int>(receiveUserIDs) && !ValidateUtil.HasItems<Guid>(receiveRoles))
            {
                ThrowError(new SystemNotifyNoReceive());
                ok = false;
            }

            if (beginDate >= endDate)
            {
                ThrowError(new SystemNotifyTimespanError());
                ok = false;
            }

            return ok;
        }

        public void CreateSystemNotify(int operatorUserID, string subject, string Content, IEnumerable<Guid> receiveRoles, IEnumerable<int> receiveUserIDs, DateTime beginDate, DateTime endDate)
        {
            if (subject == string.Empty)
            {
                subject = string.Format("{0:yyyy-MM-dd HH:mm}", DateTimeUtil.Now);
            }
            if (AllSettings.Current.BackendPermissions.Can(operatorUserID, BackendPermissions.Action.Manage_SystemNotify))
            {
                if (ValidateSystemNotifyData(Content, receiveRoles, receiveUserIDs, beginDate, endDate))
                {
                    SystemNotify newNotify = NotifyDao.Instance.CreateSystemNotify(subject, Content, receiveRoles, receiveUserIDs, beginDate, endDate, operatorUserID, IPUtil.GetCurrentIP());
                    SystemNotifyProvider.Update();

                    if (OnSystemNotifyCreated != null) OnSystemNotifyCreated(newNotify);
                }
            }
            else
            {
                ThrowError(new NoPermissionManageSystemNoyify());
            }
        }

        public void UpdateSystemNotify(int operatorUserID, string subject, int notifyID, string Content, IEnumerable<Guid> receiveRoles, IEnumerable<int> receiveUserIDs, DateTime beginDate, DateTime endDate)
        {

            if (subject == string.Empty)
            {
                subject = string.Format("{0:yyyy-MM-dd HH:mm}", DateTimeUtil.Now);
            }

            if (AllSettings.Current.BackendPermissions.Can(operatorUserID, BackendPermissions.Action.Manage_SystemNotify))
            {
                if (ValidateSystemNotifyData(Content, receiveRoles, receiveUserIDs, beginDate, endDate))
                {
                    SystemNotify notify = NotifyDao.Instance.UpdateSystemNotify(notifyID, subject, Content, receiveRoles, receiveUserIDs, beginDate, endDate, operatorUserID, IPUtil.GetCurrentIP());
                    SystemNotifyProvider.Update();

                    if (OnSystemNotifyUpdated != null)
                        OnSystemNotifyUpdated(notify);
                }
            }
            else
            {
                ThrowError(new NoPermissionManageSystemNoyify());
            }
        }

        public void DeleteSystemNotifys(int operatorUserID, IEnumerable<int> notifyIDs)
        {
            if (AllSettings.Current.BackendPermissions.Can(operatorUserID, BackendPermissions.Action.Manage_SystemNotify))
            {
                if (ValidateUtil.HasItems<int>(notifyIDs))
                {
                    NotifyDao.Instance.DeleteSystemNotifys(notifyIDs);
                    SystemNotifyProvider.Update();

                    if (OnSystemNotifyDeleted != null)
                        OnSystemNotifyDeleted(notifyIDs);
                }
            }
            else
            {
                ThrowError(new NoPermissionManageSystemNoyify());
            }
        }

        #endregion


        /// <summary>
        /// 判断用户的通知设置
        /// </summary>
        /// <param name="userID">用户</param>
        /// <param name="notifyType">类型</param>
        /// <returns></returns>
        private bool CheckUserNotifySettings(int userID, int notifyType)
        {
            NotifyState SysState = AllSettings.Current.NotifySettings.GetNotifySystemState(notifyType);
            SystemNotifyProvider.Update();

            //判断系统设置
            switch (SysState)
            {
                case NotifyState.AlwaysClose:
                    return false;
                case NotifyState.DefaultClose:
                case NotifyState.DefaultOpen:
                    //判断用户设置
                    UserNotifySetting userSetting = UserBO.Instance.GetNotifySetting(userID);
                    if (userSetting != null && userSetting.GetNotifyState( notifyType ) == NotifyState.DefaultClose)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }


        public bool AddNotify(AuthUser oprateUser, Notify notify)
        {
            #region 基础参数检查

            if (notify == null)
                return false;


            if (notify.UserID <= 0)
            {
                return false;
            }

            if (notify.UserID == oprateUser.UserID)
            {
                return true;
            }

            #endregion

            UnreadNotifies UnreadNotifies = null;
#if !Passport
            PassportClientConfig setting = Globals.PassportClient;

            if (setting.EnablePassport)
            {
                NotifyType type = AllNotifyTypes[notify.TypeID];


                NotifyActionProxy[] proxys = new NotifyActionProxy[notify.Actions.Count];
                int i = 0;
                foreach (NotifyAction action in notify.Actions)
                {
                    NotifyActionProxy nap = new NotifyActionProxy();
                    nap.Url = "{clienturl}" + action.Url;
                    nap.Title =action.Title;
                    nap.IsDialog = action.IsDialog;
                    proxys[i] = nap;
                    i++;
                }

                ThreadPool.QueueUserWorkItem(delegate(object a) {
                    try
                    {
                        setting.PassportService.Notify_Send(notify.UserID, type.TypeName, notify.Content, notify.DataTable.ToString(), proxys, notify.Keyword);
                    }
                    catch
                    {
                    }
                });
            }
            else
#endif
            {
                NotifyState SysState = AllSettings.Current.NotifySettings.GetNotifySystemState(notify.TypeID);
                SystemNotifyProvider.Update();

                //判断系统设置
                switch (SysState)
                {
                    case NotifyState.AlwaysClose:
                        return false;
                    case NotifyState.DefaultClose:
                    case NotifyState.DefaultOpen:
                        //判断用户设置
                        UserNotifySetting userSetting = UserBO.Instance.GetNotifySetting(notify.UserID);
                        if (userSetting != null && userSetting.GetNotifyState(notify.TypeID) == NotifyState.DefaultClose)
                        {
                            return false;
                        }
                        break;
                }

                StringTable actions = new StringTable();
                if (notify.Actions != null)
                {
                    foreach (NotifyAction na in notify.Actions)
                        actions.Add(na.Title, (na.IsDialog ? "*" : "") + na.Url);
                }
                NotifyDao.Instance.AddNotify(notify.UserID, notify.TypeID, notify.Content, notify.Keyword, notify.DataTable.ToString(), 0, actions.ToString(), out UnreadNotifies);
                
                AuthUser user;
               user = UserBO.Instance.GetUserFromCache<AuthUser>(notify.UserID);
               if (user != null)
               {
                   user.UnreadNotify = UnreadNotifies;
               }

               if (OnUserNotifyCountChanged != null)
               {
                   OnUserNotifyCountChanged(UnreadNotifies);
               }
              
                RemoveCacheByType(notify.UserID, 0);
                return true;
            }

            return true;

        }

        private void RemoveCacheByType(int userID, int type)
        {
            if (type == 0)
            {
                CacheUtil.RemoveBySearch(string.Format(cacheKey_UserNotifyRoot, userID));
            }
            else
            {
                CacheUtil.RemoveBySearch(string.Format(cacheKey_UserNotifyType, userID, type));
            }
        }

        /// <summary>
        /// 删除通知
        /// </summary>
        /// <param name="messageID">要删除的通知的ID</param>
        public bool DeleteNotify(AuthUser operatorUser, int notifyID)
        {
#if !Passport 
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                APIResult result = settings.PassportService.Notify_DeleteNotify(operatorUser.UserID, notifyID);


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
                Notify notify = NotifyDao.Instance.GetNotify<Notify>(null, notifyID, false);

                if (notify != null)
                {
                    if (notify.UserID != operatorUser.UserID && ManagePermission.Can(operatorUser, BackendPermissions.ActionWithTarget.Manage_Notify, notify.UserID) == false)
                    {
                        ThrowError(new NoPermissionDeleteNotifyError());
                        return false;
                    }

                    if (HasUnCatchedError)
                    {
                        return false;
                    }

                    UnreadNotifies unread;
                    NotifyDao.Instance.DeleteNotify(null, notifyID, out unread);

                    if (unread != null)
                    {
                        if (OnUserNotifyCountChanged != null)
                            OnUserNotifyCountChanged(unread);

                        AuthUser user = UserBO.Instance.GetUserFromCache<AuthUser>(unread.UserID);
                        if (user != null)
                            user.UnreadNotify = unread;

                        RemoveCacheByType(unread.UserID, 0);
                    }
                }

                //bool isDeleted = NotifyDao.Instance.DeleteNotify(userID, notifyID);

                //if (isDeleted)
                //{
                //    UserBO.Instance.RemoveUserDataCache(notify.UserID);
                //}
                return true;
            }
        }

        public bool IgnoreNotifiesByType(int userID, int typeID)
        {
            UnreadNotifies unreads;
            return IgnoreNotifiesByType(userID, typeID, out unreads);
        }

        /// <summary>
        /// 忽略所有通知
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public bool IgnoreNotifiesByType ( int userID  ,int typeID,out UnreadNotifies unreads )
        {
            unreads=null;

            AuthUser user = UserBO.Instance.GetUserFromCache<AuthUser>(userID);
#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                UnreadNotifiesProxy proxy = Globals.PassportClient.PassportService.Notify_IgnoreNotifiesByType(userID, typeID);
   
                if(proxy==null)
                    return false;
                unreads = new UnreadNotifies();
                foreach( UnreadNotifyItemProxy item in proxy.Items )
                {
                    unreads[item.TypeID] = item.Count;
                }
            }
            else
#endif
            {
                NotifyDao.Instance.IgnoreNotifyByType(userID, typeID, out unreads);
            }

            if (user != null)
                    user.UnreadNotify = unreads;
            if (OnUserNotifyCountChanged != null)
                OnUserNotifyCountChanged(unreads);

            RemoveCacheByType(userID, typeID);

            return true;
        }

        public bool IgnoreNotifies(AuthUser oparetorUser, IEnumerable<int> notifyIDs)
        {
             UnreadNotifies unread;
            return IgnoreNotifies(oparetorUser.UserID, notifyIDs,out unread);
        }


#if !Passport
        private UnreadNotifies ProxyToUnreadNotifies(UnreadNotifiesProxy proxy)
        {
            UnreadNotifies unread = new UnreadNotifies();
            unread.UserID = proxy.UserID;
            foreach (UnreadNotifyItemProxy item in proxy.Items)
            {
                unread[item.TypeID] = item.Count;
            }

            return unread;
        }

#endif

        public bool IgnoreNotifies(int oparetorUserID, IEnumerable<int> notifyIDs, out UnreadNotifies unread)
        {
            unread = new UnreadNotifies();

            if (!ValidateUtil.HasItems<int>(notifyIDs))
                return true;
#if !Passport 
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                List<int> ids = new List<int>();
                foreach (int id in notifyIDs)
                    ids.Add(id);

                int[] t = new int[ids.Count];
                ids.CopyTo(t);

                UnreadNotifiesProxy proxy = settings.PassportService.Notify_IgnoreNotifies(oparetorUserID, t);
                
                unread = ProxyToUnreadNotifies(proxy);
            }
            else
#endif
            {
                NotifyDao.Instance.IgnoreNotify(oparetorUserID, notifyIDs, out unread);
            }

            if (!unread.IsEmpty||unread.UserID>0)
            {
                if (OnUserNotifyCountChanged != null)
                    OnUserNotifyCountChanged(unread);

                AuthUser oparetorUser = UserBO.Instance.GetUserFromCache<AuthUser>(oparetorUserID);
                if (oparetorUser != null) oparetorUser.UnreadNotify = unread;

                RemoveCacheByType(oparetorUserID, 0);
            }
            
            return true;
        }

        public void IgnoreSystemNotify(int userID, int systemNotifyID)
        {
            SystemNotifyProvider.IgnoreNotify(userID, systemNotifyID);
            if (OnUserIgnoreSystemNotify != null)
                OnUserIgnoreSystemNotify(userID, systemNotifyID);
        }

        /// <summary>
        /// 删除多个通知
        /// </summary>
        /// <param name="notifyIDs">要删除的通知的ID集</param>
        public bool DeleteNotifies(int operatorUserID, IEnumerable<int> notifyIDs)
        {
            if (notifyIDs == null)
            {
                ThrowError(new NoSelectedNotifiesError("notifyIDs"));
                return false;
            }

            if (!ValidateUtil.HasItems<int>(notifyIDs))
            {
                return true;
            }
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                List<int> ids = new List<int>();
                foreach (int id in notifyIDs)
                    ids.Add(id);

                int[] t = new int[ids.Count];
                ids.CopyTo(t);

                APIResult result = null;

                try
                {
                    result = settings.PassportService.Notify_DeleteNotifies(operatorUserID, t);
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
                List<int> deleteNotifyIds = new List<int>();
                NotifyCollection notifies = NotifyDao.Instance.GetNotifies(notifyIDs);

                ///如果集合里没有数据， 会出现没有权限的误报， 因此直接返回
                if (notifies.Count == 0)
                {
                    return true;
                }

                foreach (Notify notify in notifies)
                {
                    if (notify.UserID == operatorUserID || ManagePermission.Can(operatorUserID, BackendPermissions.ActionWithTarget.Manage_Notify, notify.UserID))
                    {
                        deleteNotifyIds.Add(notify.NotifyID);
                    }
                }

                if (deleteNotifyIds.Count == 0)
                {
                    ThrowError(new NoPermissionDeleteNotifyError());
                    return false;
                }

                UnreadNotifyCollection unread;

                NotifyDao.Instance.DeleteNotifies(null, deleteNotifyIds, out unread);

                foreach (UnreadNotifies un in unread)
                {
                    RemoveCacheByType(un.UserID, 0);

                    if (OnUserNotifyCountChanged != null)
                    {
                        OnUserNotifyCountChanged(un);
                    }
                    AuthUser user = UserBO.Instance.GetUserFromCache<AuthUser>(un.UserID);
                    if (user != null)
                    {
                        user.UnreadNotify = un;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 全部忽略某类型的通知
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="type"></param>
        public void DeleteNotifysByType(AuthUser operatorUser, int type)
        {
            if (operatorUser == User.Guest)
                return;
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                APIResult result = null;

                try
                {
                    result = settings.PassportService.Notify_DeleteNotifysByType(operatorUser.UserID, type);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return;
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                }
                else if (result.IsSuccess == false)
                {
                    ThrowError<CustomError>(new CustomError("", result.Messages[0]));
                }
            }
            else
#endif
            {
                if (type == 0)
                {
                    int maxID = 0;
                    foreach (SystemNotify sn in operatorUser.SystemNotifys)
                    {
                        if (maxID < sn.NotifyID)
                            maxID = sn.NotifyID;
                    }
                    operatorUser.SystemNotifys.Clear();
                    UserBO.Instance.UpdateMaxSystemNotifyID(operatorUser, maxID);
                }

                NotifyDao.Instance.DeleteNotifysByType(operatorUser.UserID, type);
                RemoveCacheByType(operatorUser.UserID, type);
            }
        }

        /// <summary>
        /// 删除符合指定条件的通知
        /// </summary>
        public bool DeleteNotifiesBySearch(int operatorUserID, AdminNotifyFilter notifyFilter)
        {
            if (notifyFilter == null)
            {
                notifyFilter = new AdminNotifyFilter();
            }

            Guid[] excludeRoleIds = ManagePermission.GetNoPermissionTargetRoleIds(operatorUserID, PermissionTargetType.Content);

            NotifyDao.Instance.DeleteNotifiesBySearch(notifyFilter, excludeRoleIds);
            //bool isDeleted = NotifyDao.Instance.DeleteNotifiesBySearch(notifyFilter);

            //if (isDeleted)
            //{
            //    UserBO.Instance.ClearUserDataCache();
            //}

            return true;
        }

        #region Privates

        #region Check

        private bool ValidateNotifyContent(string content)
        {
            if (StringUtil.GetByteCount(content) > 2000)
            {
                return false;
            }

            return true;
        }

        private bool ValidateNotifyParameters(string parameters)
        {
            if (StringUtil.GetByteCount(parameters) > 2000)
            {
                return false;
            }

            return true;
        }

        #endregion

        #endregion
    }
}