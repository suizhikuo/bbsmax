//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class NotifyTemplateMembers
    {
        /*
        private int m_CurrentUserID = UserBO.Instance.GetCurrentUserID();

        #region 模板成员参数类及委托

        public class NotifyListHeadFootParams
        {
            private int m_CurrentUserID = UserBO.Instance.GetCurrentUserID();

            private NotifyType m_Type = NotifyType.PostNotify;
            private int? m_TotalNotifies;
            private int m_PageSize = Consts.DefaultPageSize;
            private AdminNotifyFilter m_Filter;

            public NotifyListHeadFootParams() { }

            public NotifyListHeadFootParams(int? totalNotifies, AdminNotifyFilter filter)
            {
                m_TotalNotifies = totalNotifies;
                m_Filter = filter;
            }

            public NotifyListHeadFootParams(int? totalNotifies, AdminNotifyFilter filter, int pageSize)
            {
                m_TotalNotifies = totalNotifies;
                m_Filter = filter;
                m_PageSize = pageSize;
            }

            public NotifyType NotifyType
            {
                get
                {
                    return this.m_Type;
                }
            }

            public NotifyListHeadFootParams(NotifyType type, int? totalNotifies, int pageSize)
            {
                m_Type = type;
                m_TotalNotifies = totalNotifies;
                m_PageSize = pageSize;
            }

            public int PageSize
            {
                get { return m_PageSize; }
            }

            public int TotalNotifies
            {
                get { return m_TotalNotifies != null ? m_TotalNotifies.Value : 0; }
            }

            public bool HasItems
            {
                get { return m_TotalNotifies != null && m_TotalNotifies > 0; }
            }

            public AdminNotifyFilter SearchForm
            {
                get { return m_Filter; }
            }
        }

        /*
        public class NotifyListItemParams
        {
            private NotifyBase m_Notify;
            private string m_Content;

            public NotifyListItemParams() { }

            public NotifyListItemParams(NotifyBase notify, string content)
            {
                m_Notify = notify;
                m_Content = content;
            }

            public NotifyBase Notify
            {
                get { return m_Notify; }
            }

            public bool IsSelected(string type)
            {
                return m_Notify.Type == StringUtil.TryParse<NotifyType>(type, NotifyType.FriendNotify);
            }

            public string Content
            {
                get { return m_Content; }
            }
        }
         public delegate void NotifyListItemTemplate(NotifyListItemParams _this, int i);


        public delegate void NotifyListHeadFootTemplate(NotifyListHeadFootParams _this);

        #endregion
*/
        /*
        #region 列表

        /// <summary>
        /// 前台通知列表
        /// </summary>
        [TemplateTag]
        public void NotifyList(
              string type
            , int pageNumber
            , GlobalTemplateMembers.CannotDoTemplate cannotList
            , GlobalTemplateMembers.NodataTemplate nodata
            , NotifyListHeadFootTemplate head
            , NotifyListHeadFootTemplate foot
            , NotifyListItemTemplate item)
        {
            #region 前台通知列表

            User currentUser = User.Current;

            int? totalNotifies = null;
            int pageSize = Consts.DefaultPageSize;
            if (pageNumber <= 1) { pageNumber = 1; }
            NotifyType notifyType = NotifyType.FriendNotify;
            User userData = User.Current;

            if (string.IsNullOrEmpty(type))
            {
                //if (userData != null)
                //{
                //    if (userData.UnreadFriendNotifies <= 0)
                //    {
                //        notifyType = NotifyType.HailNotify;
                //    }
                //    if (notifyType == NotifyType.HailNotify && userData.UnreadHailNotifies <= 0)
                //    {
                //        notifyType = NotifyType.PostNotify;
                //    }
                //    if (notifyType == NotifyType.PostNotify && userData.UnreadPostNotifies <= 0)
                //    {
                //        notifyType = NotifyType.BidUpNotify;
                //    }
                //    if (notifyType == NotifyType.BidUpNotify && userData.UnreadBidUpNotifies <= 0)
                //    {
                //        notifyType = NotifyType.FriendNotify;
                //    }
                //}
                //else
                //{
                    notifyType = NotifyType.All;
                //}
            }
            else
            {
                notifyType = StringUtil.TryParse<NotifyType>(type, NotifyType.FriendNotify);
            }

            NotifyCollection notifies = NotifyBO.Instance.GetNotifiesByType( UserBO.Instance.GetCurrentUserID(), notifyType, pageSize, pageNumber, ref totalNotifies);

            

            NotifyListHeadFootParams headFootParams = new NotifyListHeadFootParams(notifyType, totalNotifies, pageSize);

            head(headFootParams);

            if (notifies != null)
            {
                int i = 0;

                foreach (NotifyBase notify in notifies)
                {

                    NotifyListItemParams listItemParams = new NotifyListItemParams(notify, notify.Content);

                    item(listItemParams, i++);
                }

            }

            foot(headFootParams);

            #endregion
        }
        
        /// <summary>
        /// 后台管理页面通知列表
        /// </summary>
        [TemplateTag]
        public void NotifySearchList(
              string mode
            , string filter
            , int pageNumber
            , GlobalTemplateMembers.CannotDoTemplate cannotList
            , GlobalTemplateMembers.NodataTemplate nodata
            , NotifyListHeadFootTemplate head
            , NotifyListHeadFootTemplate foot
            , NotifyListItemTemplate item)
        {
            #region 后台管理页面通知列表

            int? totalNotifies = null;
            int pageSize = Consts.DefaultPageSize;
            if (pageNumber <= 1) { pageNumber = 1; }

            NotifyCollection notifies = null;
            User currentUser = User.Current;

            NotifyListHeadFootParams headFootParams;

            #region 获取搜索表单数据

            AdminNotifyFilter notifyFilter = AdminNotifyFilter.GetFromFilter(filter);

            if (notifyFilter != null)
            {
                notifies = NotifyBO.Instance.GetNotifiesBySearch(notifyFilter, pageNumber);
                pageSize = notifyFilter.PageSize;
                headFootParams = new NotifyListHeadFootParams(totalNotifies, notifyFilter, pageSize);
            }
            else
            {
                notifies = NotifyBO.Instance.GetAllNotifies(pageSize, pageNumber, ref totalNotifies);
                notifyFilter = new AdminNotifyFilter();
                notifyFilter.SearchMode = 1;
                headFootParams = new NotifyListHeadFootParams(totalNotifies, notifyFilter);
            }

            #endregion

            head(headFootParams);

            if (notifies != null && notifies.Count > 0)
            {
                int i = 0;

                foreach (NotifyBase notify in notifies)
                {
                    NotifyListItemParams listItemParams = new NotifyListItemParams(notify, notify.Content);

                    item(listItemParams , i++);
                }
            }

            foot(headFootParams);

            #endregion
        }
         
         #endregion

*/
        
    }
}