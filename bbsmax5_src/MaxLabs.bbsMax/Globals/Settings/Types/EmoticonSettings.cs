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

namespace MaxLabs.bbsMax.Settings
{
    public class EmoticonSettings : SettingBase
    {

        public EmoticonSettings() 
        {
            ThumbImageHeight = 24;
            ThumbImageWidth = 24;

            EnableUserEmoticons = true;
            MaxEmoticonCount = new Exceptable<int>(200);            //最大表情数限制200个
            MaxEmoticonFileSize = new Exceptable<long>(100 *  1024); //最大文件限制100K
            MaxEmoticonSpace = new Exceptable<long>(5 * 1024 * 1024);//最大空间限制5M
            Import = new Exceptable<bool>(true);
            Export = new Exceptable<bool>(true);
        }

        /// <summary>
        /// 是否启用用户自定义表情
        /// </summary>
        [SettingItem]
        public bool EnableUserEmoticons { get; set; }

        ///// <summary>
        ///// 用户可创建的分组数量
        ///// </summary>
        //[SettingItem]
        //public Exceptable<int> MaxGroupCount { get; set; }


        /// <summary>
        /// 单个表情大小限制
        /// </summary>
        [SettingItem]
        public Exceptable<long > MaxEmoticonFileSize { get; set; }

        /// <summary>
        /// 最大表情数
        /// </summary>
        [SettingItem]
        public Exceptable<int> MaxEmoticonCount { get; set; }

        /// <summary>
        /// 最大空间大小
        /// </summary>
        [SettingItem]
        public Exceptable<long> MaxEmoticonSpace { get; set; }

        /// <summary>
        /// 导出
        /// </summary>
        [SettingItem]
        public Exceptable<bool> Import { set; get; }

        /// <summary>
        /// 导出
        /// </summary>
        [SettingItem]
        public Exceptable<bool> Export { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ThumbImageWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ThumbImageHeight { get; set; }
    }
}