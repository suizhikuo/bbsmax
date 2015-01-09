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

using MaxLabs.bbsMax.DataAccess;
using System.Collections;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities 
{
    /// <summary>
    /// 分享内容(用于站外分享时抓取内容插件)
    /// </summary>
    public class ShareContent
    {

        public ShareContent()
        { }


        /// <summary>
        /// （新闻，视频等）网页地址
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 分享类型
        /// </summary>
        public ShareType Catagory { get; set; }


        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 网址域名（用来标志是哪个网站的视频）
        /// </summary>
        public string Domain { get; set; }


        /// <summary>
        /// 缩略图地址
        /// </summary>
        public string ImgUrl { get; set; }
    }

}