//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;


using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Settings
{
   
	/// <summary>
	/// 论坛设置
	/// </summary>
	public class SiteSettings : SettingBase
	{
		public SiteSettings()
		{

			ForumCloseReason = Lang.Setting_ForumSetting_ForumCloseReason;
			SiteName        = "bbsMax";
			SiteUrl         = string.Empty;

            BbsUrls = "*";

            BbsName = "bbsMax论坛";

            this.ForumIcp = string.Empty;
            this.StatCode = string.Empty;

            this.DisplaySiteNameInNavigation = false;
            //this.EncodePostUrl = false;
            //this.OpenGZipStaticFile = false;
            this.ViewIPFields = new Exceptable<int>(2);
            //this.EnableCookieDomain = true;

#if !Passport
            ForumClosed      = ForumState.Open;
            this.DefaultFeedType = FeedType.FriendFeed;
#endif
            if (ScopeList == null)
                ScopeList = new ScopeBaseCollection();

		}

        /// <summary>
        /// 按照单个项保存本设置
        /// </summary>
        public override bool Serializable
        {
            get { return false; }
        }



        /// <summary>
        /// 定时关闭网站时间列表
        /// </summary>
        /// 
        [SettingItem]
        public ScopeBaseCollection ScopeList { get; set; }

		/// <summary>
		/// 论坛关闭原因
		/// </summary>
		[SettingItem]
		public string ForumCloseReason { get; set; }

		/// <summary>
		/// 站点名称
		/// </summary>
		[SettingItem]
		public string SiteName { get; set; }

        /// <summary>
        /// 论坛名称
        /// </summary>
        [SettingItem]
        public string BbsName { get; set; }


        private string m_SiteUrl;
		/// <summary>
        /// 站点URL
		/// </summary>
		[SettingItem]
		public string SiteUrl 
        {
            get 
            {
                if (string.IsNullOrEmpty(m_SiteUrl))
                    m_SiteUrl = Globals.FullAppRoot;

                return m_SiteUrl;
            }
            set
            {
                m_SiteUrl = value;
            }
        }




		/// <summary>
		/// 论坛的ICP备案信息
		/// </summary>
		[SettingItem]
		public string ForumIcp { get; set; }

        /// <summary>
        /// 第三方统计机构的统计代码（cnzz、51la等）
        /// </summary>
        [SettingItem]
        public string StatCode { get; set; }

        /// <summary>
        /// 在导航栏显示站点名称
        /// </summary>
        [SettingItem]
        public bool DisplaySiteNameInNavigation { get; set; }

       
        /// <summary>
        /// 论坛的域名(如果有多个域名可以访问，一行填写一个。第一行的始终为默认的域名)
        /// </summary>
        //[SettingItem]
        public string BbsUrls { get; set; }

        /// <summary>
        /// 可以查看IP的段数 (如果有屏蔽IP权限 应始终显示4段)
        /// </summary>
        [SettingItem]
        public Exceptable<int> ViewIPFields { get; set; }

#if !Passport

        /// <summary>
		/// 论坛是否已经关闭
		/// </summary>
		[SettingItem]
		public ForumState ForumClosed { get; set; }


        /// <summary>
        /// 用户中心首页默认显示动态类型
        /// </summary>
        [SettingItem]
        public FeedType DefaultFeedType { get; set; }

#endif

        private Dictionary<string, bool> m_BbsUrls = null;
        private bool m_AllowAllUrl = false;

        /// <summary>
        /// 查看
        /// </summary>
        /// <param name="bbsUrl"></param>
        /// <returns></returns>
        public bool InBbsUrls(string bbsUrl)
        {
            if (m_AllowAllUrl)
                return true;

            Dictionary<string, bool> bbsUrls = m_BbsUrls;

            if (bbsUrls == null)
            {
                string[] lines = BbsUrls.Split('\r');
                bbsUrls = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

                foreach (string line in lines)
                {
                    string item = line.Trim();

                    if (item.Length > 0)
                    {
                        if (line == "*")
                        {
                            m_AllowAllUrl = true;
                            return true;
                        }
                        else if (bbsUrl.Length == 0)
                            bbsUrls.Add(line, true);
                        else
                            bbsUrls.Add(line, false);
                    }
                }

                m_BbsUrls = bbsUrls;

            }

            return bbsUrls.ContainsKey(bbsUrl);
        }
	}
}