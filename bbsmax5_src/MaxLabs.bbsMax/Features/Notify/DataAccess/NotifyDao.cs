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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess
{
    //<summary>
    //通知提醒的相关数据操作
    //</summary>
    public abstract class NotifyDao : DaoBase<NotifyDao>
    {
        /// <summary>
        /// 检查某个用户是否已经有接收当前用户发送的此类通知
        /// <param name="userID">当前用户</param>
        /// <param name="relatedUserID">通知接收者</param>
        /// </summary>
        //public abstract bool CheckIfNotified(int userID, int senderUserID, NotifyType notifyType);

        public abstract bool RegisterNotifyType(string notifyType, bool keep, string description, out NotifyType type);
        public abstract NotifyCollection AdminGetNotifiesBySearch(AdminNotifyFilter notifyFilter, int pageNumber, IEnumerable<Guid> excludeRoleIds);

        public abstract bool IgnoreNotifyByType(int userID, int typeID, out UnreadNotifies unreads);

        public abstract NotifyTypeCollection LoadAllNotifyType();

        public abstract void ClearNotify(int days, int rows, JobDataClearMode clearMode);

        public abstract NotifyCollection GetTopNotifys(int userID, int typeID, int count);

        public abstract SystemNotifyCollection GetSystemNotifies();

        /// <summary>
        /// 根据通知ID获取用户ID
        /// </summary>
        /// <param name="notifyID">通知ID</param>
        public abstract int GetUserIDByNotifyID(int notifyID);

        /// <summary>
        /// 获取用户的某条通知
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="notifyID">消息ID</param>
        public abstract T GetNotify<T>(int? userID, int notifyID, bool isRead) where T : Notify, new();

        public abstract SystemNotify UpdateSystemNotify(int notifyID, string subject, string Content, IEnumerable<Guid> receiveRoles, IEnumerable<int> receiveUserIDs, DateTime beginDate, DateTime endDate, int dispatcherID, string dispatcherIP);
        public abstract SystemNotify CreateSystemNotify(string subject, string Content, IEnumerable<Guid> receiveRoles, IEnumerable<int> receiveUserIDs, DateTime beginDate, DateTime endDate, int dispatcherID, string dispatcherIP);
        public abstract SystemNotify GetSystemNotify(int notifyID);

        public abstract void DeleteSystemNotifys(IEnumerable<int> notifyIDs);
        public abstract void SetSystemNotifyReadUserIDs(int notifyID, string userIDsString);

        /// <summary>
        /// 获取指定用户指定类型的所有通知
        /// </summary>
        /// <param name="notifyType">指定类型</param>
        /// <param name="userID">指定的用户的ID</param>
        /// <returns>指定用户指定类型的所有通知集合</returns>
        public abstract NotifyCollection GetNotifiesByType(int userID, int type, int pageSize, int pageNumber, ref int? count);

        ////<summary>
        ////获取指定用户/所有用户的所有通知
        ////</summary>
        ////<param name="userID">指定用户的ID,可以为空,为空则为要获取所有用户</param>
        ////<returns>返回所有通知集合</returns>
        public abstract NotifyCollection GetNotifies(int? userID, int pageSize, int pageNumber, ref int? count);

        /// <summary>
        /// 获取几个通知
        /// </summary>
        public abstract NotifyCollection GetNotifies(IEnumerable<int> notifyIDs);

        ////<summary>
        ////高级搜索
        ////</summary>
        public abstract NotifyCollection GetNotifiesBySearch(AdminNotifyFilter notifyFilter, int pageNumber);

        ////<summary>
        ////发送通知
        ////</summary>
        ////<param name="userID">所有者ID</param>
        ////<param name="relatedUserID">触发通知的用户ID</param>
        ////<param name="uniteNotify">是否合并同类的通知</param>
        ////<param name="tagetID">目标内容的ID，比如说回帖的话就是帖子ID</param>
        ////<param name="notifyType">通知类型</param>
        ////<param name="senderIP">IP地址</param>
        ////<param name="parameters">参数</param>
        ////<returns></returns>
        public abstract bool AddNotify(int userID, int typeID, string Content, string keyword, string datas, int passportClientID , string actions, out UnreadNotifies unreadNotifies);

        /// <summary>
        /// 删除通知
        /// </summary>
        /// <param name="userID">通知的用户ID</param>
        /// <param name="notifyID">要删除的通知的ID</param>
        public abstract bool DeleteNotify(int? userID, int notifyID,out UnreadNotifies unreadNotifies);

        public abstract bool DeleteNotifysByType(int? userID, int type);

        ///// <summary>
        ///// 删除指定用户指定类型的多个通知
        ///// </summary>
        ///// <param name="userID">指定用户ID</param>
        ///// <param name="notifyType">指定类型</param>
        //public abstract bool DeleteUserNotifiesByType(int? userID, int type);

        /// <summary>
        /// 删除多个通知
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="notifyIDs">要删除的通知的ID集</param>
        public abstract bool DeleteNotifies(int? userID, IEnumerable<int> notifyIDs, out UnreadNotifyCollection unreads);

        /// <summary>
        /// 删除符合指定条件的通知
        /// </summary>
        public abstract bool DeleteNotifiesBySearch(AdminNotifyFilter notifyFilter, Guid[] excludeRoleIds);

        /// <summary>
        /// 忽略通知
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="notifyIDs"></param>
        /// <param name="unreadNotifies"></param>
        /// <returns></returns>
        public abstract bool IgnoreNotify(int userID, IEnumerable<int> notifyIDs, out UnreadNotifies unreadNotifies);

    }
}