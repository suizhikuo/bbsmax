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

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using System.Collections.ObjectModel;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Entities
{
    public class Sys 
    {

        public Sys()
        { }

        private int? m_TotalThreads;
        public int TotalThreads
        {
            get
            {
                if (m_TotalThreads == null)
                {
                    SetStats();
                }
                return m_TotalThreads.Value;
            }
        }
        private int? m_TotalPosts;
        public int TotalPosts
        {
            get
            {
                if (m_TotalPosts == null)
                {
                    SetStats();
                }
                return m_TotalPosts.Value;
            }
        }
        private int? m_TodayPosts;
        public int TodayPosts
        {
            get
            {
                if (m_TodayPosts == null)
                {
                    SetStats();
                }
                return m_TodayPosts.Value;
            }
        }

        private void SetStats()
        {
            int totalThreads = 0;
            int totalPosts = 0;
            int todayPosts = 0;
            foreach (Forum forum in ForumBO.Instance.GetAllForums())
            {
                if (forum.ForumID > 0)
                {
                    totalThreads += forum.TotalThreads;
                    totalPosts += forum.TotalPosts;
                    todayPosts += forum.TodayPosts;
                }
            }

            m_TotalPosts = totalPosts;
            m_TodayPosts = todayPosts;
            m_TotalThreads = totalThreads;

        }

        public int TotalUsers
        {
            get
            {
                return VarsManager.Stat.TotalUsers;
            }
        }
        public string NewUser
        {
            get
            {
                return VarsManager.Stat.NewUsername;
            }
        }
        public int NewUserID
        {
            get
            {
                return VarsManager.Stat.NewUserID;
            }
        }

        public int YestodayPosts
        {
            get
            {
                return VarsManager.Stat.YestodayPosts;
            }
        }
        private int? m_DayMaxPosts;
        public int DayMaxPosts
        {
            get
            {
                if (m_DayMaxPosts == null)
                {
                    m_DayMaxPosts = VarsManager.Stat.MaxPosts;

                    if (TodayPosts > m_DayMaxPosts.Value)
                    {
                        m_DayMaxPosts = TodayPosts;
                    }
                }
                return m_DayMaxPosts.Value;
            }
        }
      
    }

}