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

using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax
{
    /// <summary>
    /// 公告
    /// </summary>
    public class AnnouncementBO:BOBase<AnnouncementBO>
    {
        #region events

        public static event AnnouncementListChanged OnAnnouncementListChanged;

        #endregion

        public AnnouncementCollection GetAnnouncements()
        {
            return AnnouncementDao.Instance.GetAnnouncements();
        }

        public Announcement GetAnnouncement(int AnnouncementID)
        {
            if (AllAnnouncement.ContainsKey(AnnouncementID))
                return AllAnnouncement.GetValue(AnnouncementID);

            return AnnouncementDao.Instance.GetAnnouncement(AnnouncementID);
        }

        /// <summary>
        /// 获取未过期的
        /// </summary>
        /// <returns></returns>
        public AnnouncementCollection GetAvailableAnnouncements()
        {
            return AnnouncementDao.Instance.GetAvailableAnnouncements();
        }

        /// <summary>
        /// 保存公告
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="postUserID"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="announcementType"></param>
        /// <returns></returns>
        public Announcement SaveAnnouncement(int operatorUserID, int announcementID,  string subject, string content, DateTime beginDate, DateTime endDate, AnnouncementType announcementType, int sortOrder)
        {
            if (!CanManageAnnouncement(operatorUserID))
            {
                ThrowError(new NoPermissionManageAnnouncementError());
                return null;
            }

            if (string.IsNullOrEmpty(subject))
            {
                ThrowError(new EmptyAnnouncementSubjectError("subject"));
            }

            if (string.IsNullOrEmpty(content))
            {
                ThrowError(new EmptyAnnouncementContentError("content"));
            }

            if (HasUnCatchedError)
                return null;

            Announcement announcement = AnnouncementDao.Instance.SaveAnnouncement( announcementID,  operatorUserID,  subject,  content,  beginDate,  endDate,  announcementType,  sortOrder);
            AllAnnouncement = null;
            CallAnnouncementsChangedEvent();
            return announcement;
        }

        private void CallAnnouncementsChangedEvent()
        {
            if (OnAnnouncementListChanged != null)
                OnAnnouncementListChanged();
        }

        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="announcementID"></param>
        public void DeleteAnnouncement(int operatorUserID, int announcementID)
        {
            if (!CanManageAnnouncement(operatorUserID))
            {
                ThrowError(new NoPermissionManageAnnouncementError());
                return;
            }
            AnnouncementDao.Instance.DeleteAnnouncement(announcementID);            
            AllAnnouncement = null;
            CallAnnouncementsChangedEvent();
        }

        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="announcementIDs"></param>
        public void DeleteAnnouncements( int operatorUserID, IEnumerable<int> announcementIDs)
        {
            if (!CanManageAnnouncement(operatorUserID))
            {
                ThrowError(new NoPermissionManageAnnouncementError());
                return;
            }

            if (ValidateUtil.HasItems<int>(announcementIDs))
            {
                AnnouncementDao.Instance.DeleteAnnouncements(announcementIDs);
                AllAnnouncement = null;
                CallAnnouncementsChangedEvent();
            }
        }

        public bool CanManageAnnouncement(int operatorUserID)
        {
            return AllSettings.Current.BackendPermissions.Can(operatorUserID, BackendPermissions.Action.Manage_Announcement);
        }


        private static AnnouncementCollection m_allAnnouncement = null;

        static object locker = new object();

        public static AnnouncementCollection AllAnnouncement
        {
            get
            {
                if (m_allAnnouncement == null)
                {
                    lock (locker)
                    {
                        if (m_allAnnouncement == null)
                        {
                            m_allAnnouncement = AnnouncementBO.Instance.GetAnnouncements();
                        }
                    }
                }
                return m_allAnnouncement;
            }
            private set
            {
                m_allAnnouncement = value;
            }
        }

        /// <summary>
        /// 返回实时公告列表（在当前有效期内的公告列表）
        /// </summary>
        public static AnnouncementCollection CurrentAnnouncements
        {
            get
            {
                return AllAnnouncement.Limited;
            }
        }

    }
}