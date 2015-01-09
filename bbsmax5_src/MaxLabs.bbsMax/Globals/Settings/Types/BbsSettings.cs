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

namespace MaxLabs.bbsMax.Settings
{
	/// <summary>
	/// 论坛设置
	/// </summary>
	public class BbsSettings : SettingBase
	{
        public BbsSettings()
		{
            RssShowThreadCount = 20;

            this.PostsPageSize = 10;
            this.ThreadsPageSize = 20;
            this.HotThreadRequireReplies = 20;
            this.AllowQuicklyCreateThread = true;
            this.AllowQuicklyReply = true;
            this.DisplaySignature = new Exceptable<bool>(true);

            this.DisplayAvatar = true;

            this.NewThreadTime = 3600;
            this.NewThreadPageCount = 10;
            this.ShowMarksCount = 5;

            this.DisplaySubforumsInIndexpage = true;

            this.EnableGuestNickName = true;

            //this.SearchPageSize = 20;
            //this.SearchType = SearchType.LikeStatement;
            //this.HighlightColor = "#FF0000";

            MaxAttachmentCountInDay = new Exceptable<int>(10);
            MaxTotalAttachmentsSizeInDay = new Exceptable<long>(1024 * 1024 * 20);
            DisplaySideBar = true;
            DefaultTextMode = true;

            MaxPollItemCount = 10;

            EnableShowLoginDialog = true;
            StickSortType = StickSortType.StickDate;
            GloableStickSortType = StickSortType.StickDate;

		}

        ///// <summary>
        ///// 在导航栏显示站点名称
        ///// </summary>
        //[SettingItem]
        //public bool DisplaySiteNameInNavigation { get; set; }


        //待整理============================

        /// <summary>
        /// 编辑器默认选中  true为可视化
        /// </summary>
        [SettingItem]
        public bool DefaultTextMode { get; set; }

        /// <summary>
        /// 回复每页多少个
        /// </summary>
        [SettingItem]
        public int PostsPageSize { get; set; }

        /// <summary>
        /// 列表页主题数
        /// </summary>
        [SettingItem]
        public int ThreadsPageSize { get; set; }

        /// <summary>
        /// 热帖标准
        /// </summary>
        [SettingItem]
        public int HotThreadRequireReplies { get; set; }



        /// <summary>
        /// Rss中显示主题个数
        /// </summary>
        [SettingItem]
        public int RssShowThreadCount { get; set; }


        ///// <summary>
        ///// 搜索时关键字高亮
        ///// </summary>
        //[SettingItem]
        //public string HighlightColor { get; set; }

        ///// <summary>
        ///// 默认短消息声音
        ///// </summary>
        //public string DefaultMessageSound { get; set; }

        [SettingItem]
        public bool AllowQuicklyReply { get; set; }

        [SettingItem]
        public bool AllowQuicklyCreateThread { get; set; }

        [SettingItem]
        public Exceptable<bool> DisplaySignature { get; set; }

        [SettingItem]
        public bool DisplayAvatar { get; set; }

        ///// <summary>
        ///// 星星升级阀值
        ///// </summary>
        //[SettingItem]
        //public int StarsUpgradeValve { get; set; }

        /// <summary>
        /// 多久时间以内的为 最新主题 单位秒
        /// </summary>
        [SettingItem]
        public long NewThreadTime { get; set; }

        /// <summary>
        /// 最新主题显示页数
        /// </summary>
        [SettingItem]
        public int NewThreadPageCount { get; set; }


        /// <summary>
        /// 贴子中显示最新多少个评分
        /// </summary>
        [SettingItem]
        public int ShowMarksCount { get; set; }

        /// <summary>
        /// 是否在首页显示子版块
        /// </summary>
        [SettingItem]
        public bool DisplaySubforumsInIndexpage { get; set; }

        //public string SiteName { get; set; }

        //public string BbsName { get; set; }


        ///// <summary>
        ///// 搜索方式
        ///// </summary>
        //[SettingItem]
        //public SearchType SearchType { get; set; }

        ///// <summary>
        ///// 搜索结果页大小
        ///// </summary>
        //[SettingItem]
        //public int SearchPageSize { get; set; }

        ///// <summary>
        ///// 不允许搜索的用户组
        ///// </summary>
        //[SettingItem]
        //public RoleCollection DenySearchRoles { get; set; }

        /// <summary>
        /// 游客发帖是否允许使用昵称
        /// </summary>
        [SettingItem]
        public bool EnableGuestNickName { get; set; }

        ///// <summary>
        ///// 加密帖子,签名中链接的URL
        ///// </summary>
        //[SettingItem]
        //public bool EncodePostUrl { get; set; }


        //[SettingItem]
        //public bool SetCookieDomain { get; set; }

        //[SettingItem] 
        //public bool OpenGZipStaticFile { get; set; }

        /// <summary>
        /// 显示侧边栏
        /// </summary>
        //[SettingItem]
        //public bool ShowSidehead { get; set; }

        private int m_MaxPollItemCount;
        /// <summary>
        /// 投票最大选项数  由于数据库限制  不能超过30
        /// </summary>
        [SettingItem]
        public int MaxPollItemCount 
        {
            get
            {
                if (m_MaxPollItemCount > 30)
                    m_MaxPollItemCount = 30;
                return m_MaxPollItemCount;
            }
            set { m_MaxPollItemCount = value; } 
        }

        /// <summary>
        /// 每天最大上传附件个数
        /// </summary>
        [SettingItem]
        public Exceptable<int> MaxAttachmentCountInDay { get; set; }

        /// <summary>
        /// 每天允许上传附件最大大小（字节）
        /// </summary>
        [SettingItem]
        public Exceptable<long> MaxTotalAttachmentsSizeInDay { get; set; }


        ///// <summary>
        ///// 可以查看IP的段数 (如果有屏蔽IP权限 应始终显示4段)
        ///// </summary>
        //[SettingItem]
        //public int ViewIPFields { get; set; }


        /// <summary>
        /// 默认是否显示首页侧边栏  //TODO:界面
        /// </summary>
        [SettingItem]
        public bool DisplaySideBar { get; set; }



        /// <summary>
        /// 当主题含有图片附件时 是否显示登陆对话筐
        /// </summary>
        [SettingItem]
        public bool EnableShowLoginDialog { get; set; }


        /// <summary>
        /// 置顶帖 排序方式
        /// </summary>
        [SettingItem]
        public StickSortType StickSortType { get; set; }



        /// <summary>
        /// 总置顶帖 排序方式
        /// </summary>
        [SettingItem]
        public StickSortType GloableStickSortType { get; set; }

        ///// <summary>
        ///// 启用cookie域
        ///// </summary>
        //[SettingItem]
        //public bool EnableCookieDomain { get; set; }


        ///// <summary>
        ///// 用户中心首页默认显示动态类型
        ///// </summary>
        //[SettingItem]
        //public FeedType DefaultFeedType { get; set; }

	}
}