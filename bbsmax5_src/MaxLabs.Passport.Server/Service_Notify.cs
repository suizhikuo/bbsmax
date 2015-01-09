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
using System.Web.Services;
using MaxLabs.bbsMax;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.Passport.Proxy;
using System.Web.Services.Protocols;

namespace MaxLabs.Passport.Server
{
    public partial class Service : ServiceBase
    {

        [WebMethod(Description="发送系统通知")]
        [SoapHeader("clientinfo")]
        public bool Notify_SendSystemNotify(Guid[] roleIDs, string subject, string content, DateTime begindate, DateTime enddate)
        {
            if (!CheckClient())
                return false;
            NotifyBO.Instance.CreateSystemNotify(1, subject, content, roleIDs, new int[0], begindate, enddate);
            return true;
        }

        [WebMethod(Description = "获取系统通知列表")]
        [SoapHeader("clientinfo")]
        public SystemNotifyProxy[] Notify_SystemNotifies()
        {
            if (!CheckClient())
                return null;
            SystemNotifyProxy[] systemnotifies = new SystemNotifyProxy[SystemNotifyProvider.CurrentSystemNotifys.Count];
            int i = 0;
            foreach (SystemNotify sn in SystemNotifyProvider.CurrentSystemNotifys)
            {
                systemnotifies[i++] = ProxyConverter.GetSystemNotifyProxy(sn);
            }

            return systemnotifies;
        }

        [WebMethod(Description = "获取系统通知")]
        [SoapHeader("clientinfo")]
        public List<SystemNotifyProxy> Notify_GetSystemNotifys()
        {
            if (!CheckClient())
                return null;

            SystemNotifyCollection temp = NotifyBO.Instance.GetSystemNotifys();
            List<SystemNotifyProxy> result = new List<SystemNotifyProxy>();
            foreach (SystemNotify notify in temp)
            {
                result.Add(ProxyConverter.GetSystemNotifyProxy(notify));
            }

            return result;
        }

        [WebMethod(Description = "获取系统通知")]
        [SoapHeader("clientinfo")]
        public SystemNotifyProxy Notify_GetSystemNotify(int notifyID)
        {
            if (!CheckClient())
                return null;

            SystemNotify temp = NotifyBO.Instance.GetSystemNotify(notifyID);
            return ProxyConverter.GetSystemNotifyProxy(temp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        [WebMethod(Description = "获取通知")]
        [SoapHeader("clientinfo")]
        public List<NotifyProxy> Notify_GetTopNotify(int userID, int topCount, int type)
        {
            if (!CheckClient())
                return null;

            List<NotifyProxy> notifys = new List<NotifyProxy>();

            NotifyCollection usernotifys = NotifyBO.Instance.GetTopNotifys(userID, topCount, 0);

            foreach (Notify nb in usernotifys)
                notifys.Add(ProxyConverter.GetNotifyProxy(nb));

            return notifys;
        }

        [WebMethod(Description = "发送通知接口")]
        [SoapHeader("clientinfo")]
        public bool Notify_Send(int userID, string notifyType, string content, string datas, NotifyActionProxy[] actions, string keyword)
        {
            if (!CheckClient())
                return false;
            List<NotifyAction> notifyActions = new List<NotifyAction>();

            if (actions != null)
            {
                foreach (NotifyActionProxy proxy in actions)
                    notifyActions.Add(new NotifyAction(proxy.Title, proxy.Url, proxy.IsDialog));
            }

            return NotifyBO.Instance.Server_SendNotify(userID, notifyType, content, datas, keyword, notifyActions, CurrentClient.ClientID);
        }

        [WebMethod(Description="获取所有的通知类型")]
        [SoapHeader("clientinfo")]
        public List<NotifyTypeProxy> Notify_GetAllNotifyTypes()
        {
            if (!CheckClient()) return null;
            NotifyTypeCollection types = NotifyBO.AllNotifyTypes;

            return ProxyConverter.GetNotifyTypeProxyList(types);
        }

        [WebMethod(Description="获取指定类型的通知")]
        [SoapHeader("clientinfo")]
        public List<NotifyProxy> Notify_GetNotifiesByType(int userID, int notifyType, int pageSize, int pageNumber, ref int? count)
        {
            if (!CheckClient()) return null;

            NotifyCollection notifys = NotifyBO.Instance.GetNotifiesByType(userID, notifyType, pageSize, pageNumber, ref count);

            List<NotifyProxy> result = new List<NotifyProxy>();
            foreach (Notify notify in notifys)
            {
                result.Add(ProxyConverter.GetNotifyProxy(notify));
            }

            return result;
        }


        [WebMethod]
        [SoapHeader("clientinfo")]
        public List<NotifyProxy> Notify_GetAllNotifies(int pageSize, int pageNumber, ref int? count)
        {
            if (!CheckClient()) return null;

            NotifyCollection notifys = NotifyBO.Instance.GetAllNotifies(pageSize, pageNumber, ref count);

            List<NotifyProxy> result = new List<NotifyProxy>();
            foreach (Notify notify in notifys)
            {
                result.Add(ProxyConverter.GetNotifyProxy(notify));
            }

            return result;
        }


        [WebMethod]
        [SoapHeader("clientinfo")]
        public NotifyProxy Notify_GetNotify(int userID, int notifyID)
        {
            if (!CheckClient()) return null;

            Notify notify = NotifyBO.Instance.GetNotify(userID, notifyID);

            return ProxyConverter.GetNotifyProxy(notify);
        }


        [WebMethod]
        [SoapHeader("clientinfo")]
        public UnreadNotifiesProxy Notify_IgnoreNotifiesByType(int userID, int typeID)
        {
            CheckClient();
            UnreadNotifiesProxy unreadresults;
            UnreadNotifies unreads;
            NotifyBO.Instance.IgnoreNotifiesByType(userID, typeID,out unreads);
            unreadresults = ProxyConverter.GetUnreadNotifiesProxy(unreads);
            return unreadresults;
        }


        [WebMethod]
        [SoapHeader("clientinfo")]
        public APIResult Notify_DeleteNotifies(int userID, List<int> notifyIDs)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = NotifyBO.Instance.DeleteNotifies(userID, notifyIDs);

                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Consts.ExceptionCode;
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        [WebMethod]
        [SoapHeader("clientinfo")]
        public APIResult Notify_DeleteNotifysByType(int userID, int type)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    NotifyBO.Instance.DeleteNotifysByType(UserBO.Instance.GetAuthUser(userID), type);
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Consts.ExceptionCode;
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        [WebMethod]
        [SoapHeader("clientinfo")]
        public UnreadNotifiesProxy Notify_IgnoreNotifies(int userID, List<int> notifyIDs)
        {
            if (!CheckClient()) return null;
            UnreadNotifies unreads;
            using (ErrorScope es = new ErrorScope())
            {
                NotifyBO.Instance.IgnoreNotifies(userID, notifyIDs,out unreads);
            }

            UnreadNotifiesProxy proxy = ProxyConverter.GetUnreadNotifiesProxy(unreads);
            return proxy;
        }

        [WebMethod]
        [SoapHeader("clientinfo")]
        public APIResult Notify_DeleteNotify(int userID, int notifyID)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = NotifyBO.Instance.DeleteNotify(UserBO.Instance.GetAuthUser(userID), notifyID);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Consts.ExceptionCode;
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                }
            }

            return result;
        }
    }
}