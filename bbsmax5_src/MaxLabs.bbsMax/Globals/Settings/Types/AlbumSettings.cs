//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    /// <summary>
    /// 相册相关设置
    /// </summary>
    public class AlbumSettings : SettingBase
    {
        public AlbumSettings()
        {
			EnableAlbumFunction = true;
            MaxAlbumCapacity = new Exceptable<long>(1024 * 1024 * 50);
			MaxPhotoFileSize = new Exceptable<long>(1024 * 300);
            FunctionName = "相册";
        }

		[SettingItem]
		public bool EnableAlbumFunction
		{
			get;
			set;
		}

        [SettingItem]
        public Exceptable<long> MaxAlbumCapacity
        {
            get;
            set;
        }

		[SettingItem]
		public Exceptable<long> MaxPhotoFileSize
		{
			get;
			set;
		}

        [SettingItem]
        public string FunctionName
        {
            get;
            set;
        }
    }
}