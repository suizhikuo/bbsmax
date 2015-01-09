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
	public class SearchSettings : SettingBase
	{
        public SearchSettings()
		{
            this.SearchPageSize = 20;
            this.SearchType = SearchType.LikeStatement;
            this.HighlightColor = "#FF0000";

            EnableSearch = new Exceptable<bool>(true);
            CanSearchTopicContent = new Exceptable<bool>(true);
            CanSearchAllPost = new Exceptable<bool>(true);
            CanSearchUserTopic = new Exceptable<bool>(true);
            CanSearchUserPost = new Exceptable<bool>(true);

            EnableGuestSearch = false;
            GuestCanSearchTopicContent = false;
            GuestCanSearchAllPost = false;
            GuestCanSearchUserPost = false;
            GuestCanSearchUserTopic = false;

            MaxResultCount = new Exceptable<int>(500);

            SearchTime = new Exceptable<int>(15);
            GuestSearchTime = 30;
            GuestMaxResultCount = 500;
		}

        public override bool Serializable
        {
            get { return false; }
        }

        /// <summary>
        /// 搜索时关键字高亮
        /// </summary>
        [SettingItem]
        public string HighlightColor { get; set; }



        private bool m_hasSetFullTextIndex = false;

        private SearchType m_SearchType;
        /// <summary>
        /// 搜索方式
        /// </summary>
        [SettingItem]
        public SearchType SearchType 
        {
            get
            {
                if (m_hasSetFullTextIndex == false)
                {
                    if (m_SearchType == SearchType.FullTextIndex)
                    {
                        if (false == PostBOV5.Instance.StartPostFullTextIndex())
                            m_SearchType = SearchType.LikeStatement;

                    }
                    else
                    {
                        PostBOV5.Instance.StopPostFullTextIndex();
                    }
                    m_hasSetFullTextIndex = true;
                }

                return m_SearchType;
            }
            set { m_SearchType = value; }
        }

        /// <summary>
        /// 搜索结果页大小
        /// </summary>
        [SettingItem]
        public int SearchPageSize { get; set; }

        [SettingItem]
        public Exceptable<bool> EnableSearch { get; set; }

        [SettingItem]
        public Exceptable<bool> CanSearchTopicContent { get; set; }

        [SettingItem]
        public Exceptable<bool> CanSearchAllPost { get; set; }

        [SettingItem]
        public Exceptable<bool> CanSearchUserTopic { get; set; }

        [SettingItem]
        public Exceptable<bool> CanSearchUserPost { get; set; }

        [SettingItem]
        public bool EnableGuestSearch { get; set; }

        [SettingItem]
        public bool GuestCanSearchTopicContent { get; set; }

        [SettingItem]
        public bool GuestCanSearchAllPost { get; set; }

        [SettingItem]
        public bool GuestCanSearchUserTopic { get; set; }

        [SettingItem]
        public bool GuestCanSearchUserPost { get; set; }


        [SettingItem]
        public Exceptable<int> MaxResultCount { get; set; }


        /// <summary>
        /// 搜索时间间隔 单位秒
        /// </summary>
        [SettingItem]
        public Exceptable<int> SearchTime { get; set; }


        [SettingItem]
        public int GuestMaxResultCount { get; set; }
        /// <summary>
        /// 搜索时间间隔 单位秒
        /// </summary>
        [SettingItem]
        public int GuestSearchTime { get; set; }
	}
}