//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using System.IO;
using MaxLabs.bbsMax.Settings;


namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class _default : AdminPageBase
    {

        #region 字段,属性

        protected string OSVersion, NETVersion, IISVersion;
        protected DataBaseInfo dataBaseInfo;
        protected CPUInfo cpuInfo;
        protected MemoryInfo memoryInfo;
        
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            WaitForFillSimpleUsers<ConsoleLoginLog>(ConsoleLog);

            OSVersion = Environment.OSVersion.ToString();
            if (OSVersion.IndexOf("Microsoft Windows NT 5.0") > -1)
            {
                OSVersion = string.Concat("Microsoft Windows 2000 (", OSVersion, ")");
                IISVersion = "IIS 5";
            }
            else if (OSVersion.IndexOf("Microsoft Windows NT 5.1") > -1)
            {
                OSVersion = string.Concat("Microsoft Windows XP (", OSVersion, ")");
                IISVersion = "IIS 5.1";
            }
            else if (OSVersion.IndexOf("Microsoft Windows NT 5.2") > -1)
            {
                OSVersion = string.Concat("Microsoft Windows 2003 (", OSVersion, ")");
                IISVersion = "IIS 6";
            }
            else if (OSVersion.IndexOf("Microsoft Windows NT 6.0") > -1)
            {
                OSVersion = string.Concat("Microsoft Windows Vista (", OSVersion, ")");
                IISVersion = "IIS 7";
            }
            else if (OSVersion.IndexOf("Microsoft Windows NT 6.1") > -1)
            {
                OSVersion = string.Concat("Microsoft Windows 7 (", OSVersion, ")");
                IISVersion = "IIS 7.5";
            }

            NETVersion = Environment.Version.ToString();

                dataBaseInfo = DataAccess.DatabaseInfoDao.Instance.GetDataBaseInfo();
    
            try
            {
                memoryInfo = new MemoryInfo().GetMemoryInfo();
            }
            catch
            {
                memoryInfo = new MemoryInfo();
            }
        }



        protected string FormatFileSize(long size)
        {
            return ConvertUtil.FormatSize(size);
        }

        private ConsoleLoginLogCollection m_ConsoleLog;
        protected ConsoleLoginLogCollection ConsoleLog
        {
            get
            {
                if (m_ConsoleLog == null)
                    m_ConsoleLog = UserBO.Instance.GetConsoleLoginLogs(5);
                return m_ConsoleLog;
            }
        }

        protected string SiteUrl
        {
            get
            {
                return AllSettings.Current.SiteSettings.SiteUrl;
            }
        }

        #region 首页统计变量

        private int? m_TotalThreads;
        protected int TotalThreads
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
        protected int TotalPosts
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
        protected int TodayPosts
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

#if !Passport
            foreach (Forum forum in ForumBO.Instance.GetAllForums())
            {
                if (forum.ForumID > 0)
                {
                    totalThreads += forum.TotalThreads;
                    totalPosts += forum.TotalPosts;
                    todayPosts += forum.TodayPosts;
                }
            }
#endif
            m_TotalPosts = totalPosts;
            m_TodayPosts = todayPosts;
            m_TotalThreads = totalThreads;

        }


        private int onlineMemberCount = -1;
        protected int OnlineMemberCount
        {
            get
            {
                if (onlineMemberCount == -1)
                {
#if !Passport
                    onlineMemberCount = OnlineUserPool.Instance.GetAllOnlineMembers().Count;
#endif
                }
                return onlineMemberCount;
            }
        }

        private int onlineGuestCount = -1;
        protected int OnlineGuestCount
        {
            get
            {
                if (onlineGuestCount == -1)
                {
#if !Passport
                    onlineGuestCount = OnlineUserPool.Instance.GetAllOnlineGuests().Count;
#endif
                }
                return onlineGuestCount;
            }
        }

        protected int YestodayPosts
        {
            get
            {
                return VarsManager.Stat.YestodayPosts;
            }
        }
        private int dayMaxPosts = -1;
        protected int DayMaxPosts
        {
            get
            {
                if (dayMaxPosts == -1)
                {
                    dayMaxPosts = VarsManager.Stat.MaxPosts;

                    if (TodayPosts > dayMaxPosts)
                    {
                        dayMaxPosts = TodayPosts;
                    }
                }
                return dayMaxPosts;
            }
        }

        protected int TotalUsers
        {
            get
            {
                return VarsManager.Stat.TotalUsers;
            }
        }

        protected string NewUser
        {
            get
            {
                return VarsManager.Stat.NewUsername;
            }
        }
        protected int NewUserID
        {
            get
            {
                return VarsManager.Stat.NewUserID;
            }
        }

        protected string Version
        {
            get
            {
                return Globals.Version;
            }
        }

        protected string InternalVersion
        {
            get
            {
                return Globals.InternalVersion;
            }
        }

        #endregion
    }
}