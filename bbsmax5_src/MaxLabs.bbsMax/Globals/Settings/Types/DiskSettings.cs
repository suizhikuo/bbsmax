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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    public class DiskSettings:SettingBase
    {
        public DiskSettings() 
        {
            this.DiskSpaceSize = new Exceptable<long>(1024 * 1024 * 1024);
            this.EnableDisk = true;
            this.MaxFileCount = new Exceptable<int>(0);
            this.MaxFileSize = new Exceptable<long>(10 * 1024 * 1024);
            this.PackZip = new Exceptable<bool>(true);
            this.UnpackRar = new Exceptable<bool>(true);
            this.UnpackZip = new Exceptable<bool>(true);
            this.AllowFileExtensions = new Exceptable<ExtensionList>(ExtensionList.Parse("*"));
            this.DefaultViewMode = new Exceptable<FileViewMode>(FileViewMode.Thumbnail);
        }

        [SettingItem]
        public bool EnableDisk { get; set; }
        [SettingItem]
        public Exceptable<long> DiskSpaceSize { get; set; }
        [SettingItem]
        public Exceptable<long> MaxFileSize { get; set; }
        [SettingItem]
        public Exceptable<int> MaxFileCount { get; set; }
        [SettingItem]
        public Exceptable<bool> UnpackZip { get; set; }
        [SettingItem]
        public Exceptable<bool> UnpackRar { get; set; }
        [SettingItem]
        public Exceptable<bool> PackZip { get; set; }
        [SettingItem]
        public Exceptable<FileViewMode> DefaultViewMode { get; set; }
        [SettingItem]
        public Exceptable<ExtensionList> AllowFileExtensions { get; set; }
    }
}