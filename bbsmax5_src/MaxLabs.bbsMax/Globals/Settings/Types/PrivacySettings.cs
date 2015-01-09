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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    /// <summary>
    /// 用户默认的隐私设置，新注册的用户按照此隐私设置
    /// </summary>
    public class PrivacySettings : SettingBase
    {
        public PrivacySettings()
        {

            //NetworkPublic = false;

            BlogPrivacy = SpacePrivacyType.All;
            FeedPrivacy = SpacePrivacyType.All;
            BoardPrivacy = SpacePrivacyType.All;
            DoingPrivacy = SpacePrivacyType.All;
            AlbumPrivacy = SpacePrivacyType.All;
            SpacePrivacy = SpacePrivacyType.All;
            SharePrivacy = SpacePrivacyType.All;
            InformationPrivacy = SpacePrivacyType.All;
            FriendListPrivacy = SpacePrivacyType.All;
            FeedSendItems = new FeedSendItemCollection();
        }

        ///// <summary>
        ///// 游客开放浏览
        ///// </summary>
        //[SettingItem]
        //public bool NetworkPublic { get; set; }


        /// <summary>
        /// 个人主页隐私类型
        /// </summary>
        [SettingItem]
        public SpacePrivacyType BlogPrivacy { get; set; }

        /// <summary>
        /// 个人动态隐私类型
        /// </summary>
        [SettingItem]
        public SpacePrivacyType FeedPrivacy { get; set; }

        /// <summary>
        /// 留言板隐私类型
        /// </summary>
        [SettingItem]
        public SpacePrivacyType BoardPrivacy { get; set; }

        /// <summary>
        /// 记录隐私类型
        /// </summary>
        [SettingItem]
        public SpacePrivacyType DoingPrivacy { get; set; }

        /// <summary>
        /// 相册隐私类型
        /// </summary>
        [SettingItem]
        public SpacePrivacyType AlbumPrivacy { get; set; }

        /// <summary>
        /// 个人主页隐私类型
        /// </summary>
        [SettingItem]
        public SpacePrivacyType SpacePrivacy { get; set; }

        /// <summary>
        /// 分享的隐私类型
        /// </summary>
        public SpacePrivacyType SharePrivacy { get; set; }

        /// <summary>
        /// 个人资料隐私类型
        /// </summary>
        [SettingItem]
        public SpacePrivacyType InformationPrivacy { get; set; }

        /// <summary>
        /// 好友列表隐私类型
        /// </summary>
        [SettingItem]
        public SpacePrivacyType FriendListPrivacy { get; set; }

        [SettingItem]
        public FeedSendItemCollection FeedSendItems { get; set; }

    }
}