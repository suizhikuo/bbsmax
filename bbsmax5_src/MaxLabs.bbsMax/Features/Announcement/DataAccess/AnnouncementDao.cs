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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class AnnouncementDao:DaoBase<AnnouncementDao>
    {
        public abstract AnnouncementCollection GetAnnouncements();
        public abstract Announcement GetAnnouncement(int AnnouncementID);
        /// <summary>
        /// 获取未过期的
        /// </summary>
        /// <returns></returns>
        public abstract AnnouncementCollection GetAvailableAnnouncements();

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
        public abstract Announcement SaveAnnouncement(int announcementID, int postUserID, string subject, string content, DateTime beginDate, DateTime endDate, AnnouncementType announcementType,int sortOrder);

        public abstract void DeleteAnnouncement(int AnnouncementID);

        public abstract void DeleteAnnouncements(IEnumerable<int> AnnouncementIds);

    }
}