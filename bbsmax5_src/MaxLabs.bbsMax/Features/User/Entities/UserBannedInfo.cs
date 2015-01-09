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

namespace MaxLabs.bbsMax.Entities
{
    public class UserBannedInfo
    {

        private bool m_BannedInSite = false;
        private DateTime m_BannedInSiteEndDate = DateTime.MinValue;
        private bool m_BannedInAnyForum = false;
        private DateTime m_BannedInAnyForumLastEndDate = DateTime.MinValue;
        private Dictionary<int, DateTime> m_BannedInForumEndDates = null;

        public void Fill(BannedUser bannedUser)
        {
            //是全站屏蔽的
            if (bannedUser.ForumID == 0)
            {
                m_BannedInSite = true;
                if (m_BannedInSiteEndDate < bannedUser.EndDate)
                    m_BannedInSiteEndDate = bannedUser.EndDate;
            }
            //在指定的板块屏蔽
            else
            {
                m_BannedInAnyForum = true;
                if (m_BannedInAnyForumLastEndDate < bannedUser.EndDate)
                    m_BannedInAnyForumLastEndDate = bannedUser.EndDate;

                if (m_BannedInForumEndDates == null)
                {
                    m_BannedInForumEndDates = new Dictionary<int, DateTime>();
                    m_BannedInForumEndDates.Add(bannedUser.ForumID, bannedUser.EndDate);
                }
                else
                {
                    DateTime endDate;
                    if (m_BannedInForumEndDates.TryGetValue(bannedUser.ForumID, out endDate))
                    {
                        if (endDate < bannedUser.EndDate)
                            m_BannedInForumEndDates[bannedUser.ForumID] = bannedUser.EndDate;
                    }
                }
            }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 用户是否在全站被屏蔽
        /// </summary>
        public bool BannedInSite
        {
            get
            {
                if (m_BannedInSite && m_BannedInSiteEndDate > DateTimeUtil.Now)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 用户是否在某一个版块被屏蔽
        /// </summary>
        public bool BannedInAnyForum
        {
            get
            {
                if (m_BannedInAnyForum && m_BannedInAnyForumLastEndDate > DateTimeUtil.Now)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 此用户是否在指定的版块被屏蔽
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public bool BannedInForum(int forumID)
        {
            if (BannedInAnyForum == false)
                return false;

            if (m_BannedInForumEndDates == null)
                return false;

            DateTime endDate;

            if (m_BannedInForumEndDates.TryGetValue(forumID, out endDate) && endDate > DateTimeUtil.Now)
                return true;
            else
                return false;
        }
    }
}