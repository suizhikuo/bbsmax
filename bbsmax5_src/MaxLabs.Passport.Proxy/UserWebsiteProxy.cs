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

namespace MaxLabs.Passport.Proxy
{
    public class UserWebsiteProxy : ProxyBase
    {
        public UserWebsiteProxy() { }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName
        {
            get;
            set;
        }
        /// <summary>
        /// 网站名称
        /// </summary>
        public string WebsiteName { get; set; }

        /// <summary>
        /// 网站编号
        /// </summary>
        public int WebsiteID { get; set; }

        /// <summary>
        /// 分类编号
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// 网站所属地区
        /// </summary>
        public int AreaID { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 缩略图地址
        /// </summary>
        public string SmallImageUrl { get; set; }
        
        /// <summary>
        /// 105x85缩略图地址
        /// </summary>
        public string BiggerImageUrl { get; set; }

        /// <summary>
        /// 200x200缩略图地址
        /// </summary>
        public string BiggestImageUrl { get; set; }


        /// <summary>
        /// 网站地址（域名、或者二级目录）
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否有缩略图
        /// </summary>
        public bool IsHaveImage { get; set; }

        /// <summary>
        /// 是否通过验证
        /// </summary>
        public bool Verified { get; set; }

        /// <summary>
        /// 显示颜色(#FFFFFF格式)
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 网站简介
        /// </summary>
        public string WebsiteIntro { get; set; }
    }
}