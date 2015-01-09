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
    public class InvitationSettings : SettingBase
    {
        public InvitationSettings()
        {
            //EnableInvitation = true;
            //InviteMode= InviteMode.Optional;
            //EnableInviteSerial = false;
            InviteMode = InviteMode.Close;

            AddToUserRoleWhenHasInvite = Guid.Empty;
            AddToUserRoleWhenNoInvite = Guid.Empty;

            ShowRegisterInviteInput = true;

            InviteEffectiveHours = 0;
            IntiveSerialPoint = 10;
            
            InviteSerialBuyCount = 100;
            Interval = InviteBuyInterval.Disable;

            InviteEmailTitle = "{username} 邀请您加入 {sitename}";
            InviteEmailContent = @"
Hi，我是{username}，我在{sitename}上建立了个人主页，请你也加入并成为我的好友。<br/>
请加入到我的好友中，你就可以通过我的个人主页了解我的近况，分享我的照片，随时与我保持联系。<br/>
邀请附言：<br/>
    {message}<br/>
请你点击以下链接，接受好友邀请：<br/>
    {url}<br/>
如果你拥有我的空间上面的账号，请点击以下链接查看我的个人主页：<br/>
    {space}
"; 

            InviteSerialEmailTitle = "{username} 给您发送了 {sitename} 的邀请码";
            InviteSerialEmailContent = @"
您好:<br/><br/>
{username}&nbsp;邀请您加入&nbsp;{sitename}!<br/><br/>
如果你愿意接受邀请,请点击以下地址(或复制到浏览器地址栏)开始注册帐号:<br/>
{url}<br/><br/>
==========================================================<br/>
{sitename}<br/>{site}";
        }

        ///// <summary>
        ///// 是否开启邀请功能
        ///// </summary>
        //[SettingItem]
        //public bool EnableInvitation { get; set; }

        /// <summary>
        /// 邀请模式
        /// </summary>
        [SettingItem]
        public InviteMode InviteMode { get; set; }

        /// <summary>
        /// 邀请码所需积分
        /// </summary>
        [SettingItem]
        public int IntiveSerialPoint { get; set; }

        [SettingItem]
        public UserPointType PointFieldIndex { get; set; }

        private Guid m_AddToUserRoleWhenNoInvite;
        private Guid m_AddToUserRoleWhenHasInvite;

        /// <summary>
        /// 当提供了邀请码或推广链接时，自动加入这个用户组（关闭邀请或推广功能时无效）
        /// </summary>
        [SettingItem]
        public Guid AddToUserRoleWhenHasInvite
        {
            get
            {
                //系统关闭了邀请模式
                if (InviteMode == InviteMode.Close)
                    return Guid.Empty;

                Role role = AllSettings.Current.RoleSettings.GetRole(m_AddToUserRoleWhenHasInvite);

                if (role == null || role.IsVirtualRole || role.IsNormal == false)
                    return Guid.Empty;

                return m_AddToUserRoleWhenHasInvite;
            }
            set { m_AddToUserRoleWhenHasInvite = value; }
        }

        /// <summary>
        /// 当没有提供邀请码或推广链接时，自动加入这个用户组（关闭邀请或推广功能时无效）
        /// </summary>
        [SettingItem]
        public Guid AddToUserRoleWhenNoInvite
        {
            get
            {
                //邀请码不是选填的，用户不可能没有提供邀请码就注册，故返回空
                if (InviteMode != InviteMode.InviteSerialOptional
                    &&
                    InviteMode != InviteMode.InviteLinkOptional
                    )
                    return Guid.Empty;

                Role role = AllSettings.Current.RoleSettings.GetRole(m_AddToUserRoleWhenNoInvite);

                if (role == null || role.IsVirtualRole || role.IsNormal == false)
                    return Guid.Empty;

                return m_AddToUserRoleWhenNoInvite;
            }
            set { m_AddToUserRoleWhenNoInvite = value; }
        }

        ///// <summary>
        ///// 开启邀请码功能
        ///// </summary>
        //[SettingItem]
        //public bool EnableInviteSerial { get; set; }

        /// <summary>
        /// 邀请码的有效时间(小时)
        /// </summary>
        [SettingItem]
        public int InviteEffectiveHours { get; set; }

        /// <summary>
        /// 发送邀请地址邮件标题
        /// </summary>
        [SettingItem]
        public string InviteEmailTitle { get; set; }

        /// <summary>
        /// 发送邀请地址邮件内容
        /// </summary>
        [SettingItem]
        public string InviteEmailContent { get; set; }

        /// <summary>
        /// 发送邀请码邮件标题
        /// </summary>
        [SettingItem]
        public string InviteSerialEmailTitle { get; set; }

        /// <summary>
        /// 发送邀请码邮件内容
        /// </summary>
        [SettingItem]
        public string InviteSerialEmailContent { get; set; }

        /// <summary>
        /// 购买时间间隔限制
        /// </summary>
        [SettingItem]
        public InviteBuyInterval Interval
        {
            get;
            set;
        }

        /// <summary>
        /// 购买数量限制
        /// </summary>
        [SettingItem]
        public int InviteSerialBuyCount
        {
            get;
            set;
        }

        /// <summary>
        /// 是否显示注册页面中邀请码的输入框
        /// </summary>
        [SettingItem]
        public bool ShowRegisterInviteInput
        {
            get;
            set;
        }
    }


    public enum InviteMode
    {
        /// <summary>
        /// 关闭邀请码或推广功能
        /// </summary>
        Close,

        /// <summary>
        /// 必须通过邀请码注册
        /// </summary>
        InviteSerialRequire,

        /// <summary>
        /// 可以通过邀请码注册，但不是必需的
        /// </summary>
        InviteSerialOptional,

        /// <summary>
        /// 必须通过推广链接注册
        /// </summary>
        InviteLinkRequire,

        /// <summary>
        /// 可以通过推广链接注册，但不是必需的
        /// </summary>
        InviteLinkOptional
    }
}