//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Settings
{
    public class BaseSeoSettings : SettingBase
    {
        public BaseSeoSettings()
        {
            TitleAttach = string.Empty;
            OtherHeadMessage = string.Empty;
            MetaKeywords = string.Empty;
            MetaDescription = string.Empty;
            EncodePostUrl = false;
            TitleAddPageNumber = true;
            TitleAddPageNumberFormat = "第{0}页";
        }

        [SettingItem]
        public string TitleAttach { get; set; }

        [SettingItem]
        public string OtherHeadMessage { get; set; }

        [SettingItem]
        public string MetaKeywords { get; set; }

        [SettingItem]
        public string MetaDescription { get; set; }


        /// <summary>
        /// 标题中加入页码
        /// </summary>
        [SettingItem]
        public bool TitleAddPageNumber { get; set; }


        /// <summary>
        /// 页码格式
        /// </summary>
        [SettingItem]
        public string TitleAddPageNumberFormat { get; set; }

        /// <summary>
        /// 加密帖子,签名中链接的URL
        /// </summary>
        [SettingItem]
        public bool EncodePostUrl { get; set; }

        /// <summary>
        /// “第1页” 如果未开启在标题显示页码 则返回 空 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public string FormatPageNumber(int pageNumber)
        {
            if (TitleAddPageNumber == false)
                return string.Empty;
            else
                return string.Format(TitleAddPageNumberFormat, pageNumber);
        }
    }

    public class GoogleSeoSettings : SettingBase
    {
        public GoogleSeoSettings()
        {

        }
        
        [SettingItem]
        public bool EnableSitMap
        {
            set;
            get;
        }

        private int m_SiteMapUpdateTime = 6;
        [SettingItem]
        public int SiteMapUpdateTime
        {
            get { return m_SiteMapUpdateTime; }
            set {
                if (value > 50)
                    m_SiteMapUpdateTime = 50;
                else if (value < 0)
                    m_SiteMapUpdateTime = 1;
                else
                    m_SiteMapUpdateTime = value;

            
            }
        }
    }
}