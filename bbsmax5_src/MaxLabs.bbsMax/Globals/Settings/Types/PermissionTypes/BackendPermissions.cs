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
    /// 后台的“全局”分类下的权限
    /// </summary>
    public class BackendPermissions : PermissionSetBase<BackendPermissions.Action, BackendPermissions.ActionWithTarget>
    {

        //public override PermissionSetWithTargetType PermissionSetWithTargetType
        //{
        //    get { return PermissionSetWithTargetType.ContentActions; }
        //}

        public override bool CanSetDeny
        {
            get { return false; }
        }

        public override bool IsManagement
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "后台管理权限分配"; }
        }


        public enum Action
        {
            [PermissionItem("没有权限")]
            None,

            #region "全局"选项卡

            #region "设置"分组

            [PermissionItem("站点设置")]
            [BackendPage("global/setting-site.aspx")]
            Setting_Site,

            [PermissionItem("邮件设置")]
            [BackendPage("global/setting-email.aspx")]
            Setting_Email,

            [PermissionItem("IP地址限制")]
            [BackendPage("global/setting-accesslimit.aspx")]
            Setting_AccessLimit,

            [PermissionItem("在线列表")]
            [BackendPage("global/setting-onlinelist.aspx")]
            Setting_OnlineList,

            [PermissionItem("默认表情")]
            [BackendPage("global/setting-emoticon-group.aspx")]
            Setting_Emoticon,

            [PermissionItem("敏感关键字")]
            [BackendPage("global/setting-bannedword.aspx")]
            Setting_BannedWord,

            [PermissionItem("防盗链设置")]
            [BackendPage("global/setting-download.aspx")]
            Setting_Download,

            [PermissionItem("登录设置")]
            [BackendPage("global/setting-logintype.aspx")]
            Setting_Login,


            [PermissionItem("缓存设置")]
            [BackendPage("global/setting-cache.aspx")]
            Setting_Cache,

            #endregion

            #region "注册"分组

            [PermissionItem("注册设置")]
            [BackendPage("global/setting-register.aspx")]
            Setting_Register,

            [PermissionItem("找回密码设置")]
            [BackendPage("global/setting-recoverpassword.aspx")]
            Setting_RecoverPassword,

            [PermissionItem("邀请设置")]
            [BackendPage("global/setting-invitation.aspx")]
            Setting_Invitation,

            [PermissionItem("实名设置")]
            [BackendPage("global/setting-namecheck.aspx")]
            Setting_NameCheck,

            [PermissionItem("默认隐私设置")]
            [BackendPage("global/setting-privacy.aspx")]
            Setting_Privacy,

            #endregion

            #region "管理"分组

            [PermissionItem("管理公告")]
            [BackendPage("global/manage-announcement.aspx")]
            Manage_Announcement,

            [PermissionItem("管理举报")]
            [BackendPage("global/manage-report.aspx")]
            Manage_Report,

            [PermissionItem("友情链接")]
            [BackendPage("global/setting-links.aspx")]
            Setting_Links,


            [PermissionItem("用户任务")]
            [BackendPage("interactive/manage-mission-list.aspx")]
            Manage_Mission,


            [PermissionItem("用户任务")]
            [BackendPage("interactive/manage-mission-category.aspx")]
            Manage_Mission_Category,

            #endregion

            #region "动态"分类

            [PermissionItem("动态管理")]
            [BackendPage("global/manage-feed-data.aspx")]
            Manage_Feed_Data,

            [PermissionItem("动态模板")]
            [BackendPage("global/manage-feed-template.aspx")]
            Manage_Feed_Template,

            [PermissionItem("发布动态")]
            [BackendPage("global/manage-feed-sitefeedlist.aspx")]
            Manage_Feed_SiteFeed,

            [PermissionItem("定时清理")]
            [BackendPage("global/setting-feedjob.aspx")]
            Setting_FeedJob,

            #endregion

            #region "体验"分组

            [PermissionItem("路径模式")]
            [BackendPage("global/setting-friendlyurl.aspx")]
            Setting_FriendlyUrl,

            [PermissionItem("验证码设置")]
            [BackendPage("global/setting-validatecode.aspx")]
            Setting_ValidateCode,

            [PermissionItem("Ajax设置")]
            [BackendPage("global/setting-ajax.aspx")]
            Setting_Ajax,

            [PermissionItem("日期与时间")]
            [BackendPage("global/setting-datetime.aspx")]
            Setting_Datetime,

            #endregion

            #region "搜索引擎"分组

            [PermissionItem("基本设置")]
            [BackendPage("global/setting-seobasic.aspx")]
            Setting_SeoBasic,

            [PermissionItem("百度优化")]
            [BackendPage("global/setting-baiduseo.aspx")]
            Setting_BaiduSeo,

            [PermissionItem("搜索引擎屏蔽")]
            [BackendPage("global/setting-shieldspider.aspx")]
            Setting_ShieldSpider,

            #endregion

            #endregion

            #region "用户"选项卡

            #region "管理"分组

            [PermissionItem("用户管理")]
            [BackendPage("user/manage-user.aspx")]
            Manage_User,

            [PermissionItem("实名审查")]
            [BackendPage("user/manage-namecheck.aspx")]
            Manage_NameCheck,


            [PermissionItem("添加用户")]
            [BackendPage("user/manage-user-add.aspx")]
            Manage_User_Add,

            [PermissionItem("头像审查")]
            [BackendPage("user/manage-avatarcheck.aspx")]
            Manage_AvatarCheck,

 
            #endregion

            #region "手机绑定"分组
            [PermissionItem("手机认证")]
            [BackendPage("user/setting-phonevalidate.aspx")]
            Setting_PhoneValidate,

            //[PermissionItem("历史记录")]
            //[BackendPage("other/manage-mobilelog.aspx")]
            //Manage_MobileBindLog,
            #endregion

            #region "设置"分组

            [PermissionItem("点亮图标")]
            [BackendPage("interactive/setting-medals.aspx")]
            Setting_Medals,

            [PermissionItem("用户签名")]
            [BackendPage("user/setting-user.aspx")]
            Setting_User,

            [PermissionItem("扩展字段")]
            [BackendPage("user/setting-extendedfield.aspx")]
            Setting_ExtendedField,

            [PermissionItem("好友数量")]
            [BackendPage("user/setting-friend.aspx")]
            Setting_Friend,
            //[PermissionItem("用户排行")] [BackendPage("setting-memberlist.aspx")]
            #endregion

            #region "用户组管理"分组


            [PermissionItem("基本组")]
            [BackendPage("user/setting-roles-basic.aspx")]
            Setting_Roles_Basic,

            [PermissionItem("等级组")]
            [BackendPage("user/setting-roles-level.aspx")]
            Setting_Roles_Level,

            [PermissionItem("自定义组")]
            [BackendPage("user/setting-roles-other.aspx")]
            Setting_Roles_Other,

            #endregion

            #region "积分"分组

            [PermissionItem("基本设置")]
            [BackendPage("user/setting-userpoint.aspx")]
            Setting_UserPoint,

            [PermissionItem("积分策略")]
            [BackendPage("user/setting-pointaction.aspx")]
            Setting_PointAction,

            [PermissionItem("积分兑换")]
            [BackendPage("user/setting-pointexchange.aspx")]
            Setting_PointExchange,

            [PermissionItem("积分转帐")]
            [BackendPage("user/setting-pointtransfer.aspx")]
            Setting_PointTransfer,

            [PermissionItem("积分充值")]
            [BackendPage("user/setting-pointrecharge.aspx")]
            Setting_PointRecharge,

            [PermissionItem("充值记录")]
            [BackendPage("user/manage-paylogs.aspx")]
            Setting_PayLogs,
            #endregion

            #region "邀请码"分组

            [PermissionItem("拥有数排行")]
            [BackendPage("user/manage-inviteserial-order.aspx")]
            Manage_InviteSerialOrder,

            [PermissionItem("邀请码管理")]
            [BackendPage("user/manage-inviteserial.aspx")]
            Manage_InviteSerial,

            #endregion

            #region "印象"分类

            [PermissionItem("好友印象设置")]
            [BackendPage("user/setting-impression.aspx")]
            Setting_Impression,


            [PermissionItem("好友印象词库")]
            [BackendPage("user/manage-impressiontype.aspx")]
            Manage_ImpressionType,

            [PermissionItem("好友印象管理")]
            [BackendPage("user/manage-impressionrecord.aspx")]
            Manage_ImpressionRecord,

            #endregion

            #region "屏蔽用户"分组
            [PermissionItem("屏蔽记录")]
            [BackendPage("other/manage-banuserlog.aspx")]
            Manage_BanUserLog, 
            #endregion

            #endregion

            #region "版块"选项卡


            [PermissionItem("版块管理")]
            [BackendPage("bbs/manage-forum.aspx")]
            Manage_Forum,

            [PermissionItem("版主管理")]
            [BackendPage("{moderator}")]
            Manage_Moderator,

            [PermissionItem("各版块发帖选项")]
            [BackendPage("bbs/manage-forum-detail.aspx?action=editsetting")]
            Manage_ForumDetail_EditSetting,

            [PermissionItem("各版块用户权限分配")]
            [BackendPage("bbs/manage-forum-detail.aspx?action=editusepermission")]
            Manage_ForumDetail_EditUserPermission,



            [PermissionItem("各版块积分策略")]
            [BackendPage("bbs/manage-forum-detail.aspx?action=editpoint")]
            Manage_ForumDetail_EditPoint,

            [PermissionItem("各版块评分控制")]
            [BackendPage("bbs/manage-forum-detail.aspx?action=editrate")]
            Manage_ForumDetail_EditRate,

            [PermissionItem("各版块管理权限分配")]
            [BackendPage("bbs/manage-forum-detail.aspx?action=editmanagepermission")]
            Manage_ForumDetail_EditManagePermission,


            [PermissionItem("分类信息")]
            [BackendPage("bbs/manage-threadcate.aspx")]
            Manage_ThreadCate,



            [PermissionItem("基本设置")]
            [BackendPage("bbs/setting-bbs.aspx")]
            Setting_Bbs,

            [PermissionItem("搜索设置")]
            [BackendPage("bbs/setting-search.aspx")]
            Setting_Search,

            [PermissionItem("楼层别名")]
            [BackendPage("bbs/setting-postaliasname.aspx")]
            Setting_PostAliasName,

            [PermissionItem("主题鉴定")]
            [BackendPage("bbs/setting-judgement.aspx")]
            Setting_Judgement,

            [PermissionItem("帖子图标")]
            [BackendPage("bbs/setting-posticon.aspx")]
            Setting_PostIcon,


            #endregion

            #region "应用"选项卡

            #region "空间"分类

            [PermissionItem("设置")]
            [BackendPage("app/setting-space.aspx")]

            Setting_Space,

            #endregion

            #region "日志"分类

            //[PermissionItem("文章管理")] [BackendPage("app/manage-blog.aspx")]

            [PermissionItem("设置")]
            [BackendPage("app/setting-blog.aspx")]
            Setting_Blog,

            //[PermissionItem("分类管理")] [BackendPage("app/manage-blogcategory.aspx")]

            #endregion

            #region "相册"分类

            //[PermissionItem("照片管理")] [BackendPage("app/manage-photo.aspx")]
            [PermissionItem("设置")]
            [BackendPage("app/setting-album.aspx")]
            Setting_Album,

            //[PermissionItem("分类管理")] [BackendPage("app/manage-album.aspx")]

            #endregion

            #region "记录"分类

            //[PermissionItem("记录管理")] [BackendPage("app/manage-doing.aspx")]
            [PermissionItem("选项设置")]
            [BackendPage("app/setting-doing.aspx")]
            Setting_Doing,

            #endregion

            #region "收藏"分类

            //[PermissionItem("收藏管理")] [BackendPage("app/manage-share-data.aspx?type=favorite")]
            [PermissionItem("选项设置")]
            [BackendPage("app/setting-favorite.aspx")]
            Setting_Favorite,


            #endregion

            #region "分享"分类

            //[PermissionItem("分享管理")] [BackendPage("app/manage-share-data.aspx?type=share")]
            [PermissionItem("选项设置")]
            [BackendPage("app/setting-share.aspx")]
            Setting_Share,

            #endregion

            #region "网络硬盘"分类

            //[PermissionItem("文件管理")] [BackendPage("app/manage-netdisk.aspx")]
            [PermissionItem("选项设置")]
            [BackendPage("app/setting-netdisk.aspx")]
            Setting_NetDisk,

            #endregion

            #region "自定义表情"分类

            //[PermissionItem("表情管理")] [BackendPage("app/manage-emoticon.aspx")] [BackendPage("manage-emoticon-icon.aspx")]
            [PermissionItem("选项设置")]
            [BackendPage("app/setting-useremoticon.aspx")]
            Setting_UserEmoticon,

            #endregion

            #region "通知"分类

            [PermissionItem("选项设置")]
            [BackendPage("interactive/setting-notify.aspx")]
            Setting_Notify,

            [PermissionItem("群发")]
            [BackendPage("interactive/manage-systemnotify.aspx")]
            Manage_SystemNotify,

            #endregion

            #region "对话"

            //[PermissionItem("对话管理")] [BackendPage("interactive/manage-chatsession.aspx")] [BackendPage("interactive/manage-chatmessage.aspx")]
            [PermissionItem("选项设置")]
            [BackendPage("interactive/setting-chat.aspx")]
            Setting_Chat,

            #endregion

            #region "竞价排名"
            [PermissionItem("竞价排名")]
            [BackendPage("app/setting-pointshow.aspx")]
            Setting_PointShow,

            [PermissionItem("榜单管理")]
            [BackendPage("app/manage-pointshow.aspx")]
            Manage_PointShow,

            #endregion

            #region "其他"分类

            [PermissionItem("标签管理")]
            [BackendPage("app/manage-tag.aspx")]
            Manage_Tag,

            #endregion

            #endregion

            #region "维护"选项卡

            #region "系统日志"分组

            [PermissionItem("运行日志")]
            [BackendPage("other/manage-operationlog.aspx")]
            Manage_OperationLog,

            [PermissionItem("定时清理")]
            [BackendPage("other/setting-deleteoperationlogjob.aspx")]
            Setting_DeleteOperationLogJob,

            [PermissionItem("积分日志")]
            [BackendPage("other/manage-pointlog.aspx")]
            Manage_PointLog,

            [PermissionItem("IP日志")]
            [BackendPage("other/manage-iplog.aspx")]
            Manage_IPLog,

            [PermissionItem("手机绑定日志")]
            [BackendPage("other/manage-mobilelog.aspx")]
            Manage_MobileLog,

            [PermissionItem("找回密码日志")]
            [BackendPage("other/manage-recoverpasswordlog.aspx")]
            Manage_RecoverPassword,

            #endregion

            #region "界面"分组

            [PermissionItem("模板管理")]
            [BackendPage("other/manage-template.aspx")]
            Manage_Template,

            [PermissionItem("顶部链接")]
            [BackendPage("global/setting-navigation.aspx?istoplink=1")]
            Setting_TopLinks,

            [PermissionItem("导航菜单")]
            [BackendPage("global/setting-navigation.aspx?istoplink=0")]
            Setting_Navigation,
            #endregion

            #region "道具"分组

            [PermissionItem("道具管理")]
            [BackendPage("interactive/manage-prop.aspx")]
            Manage_Prop,

            [PermissionItem("道具设置")]
            [BackendPage("interactive/setting-prop.aspx")]
            Setting_Prop,

            [PermissionItem("用户道具管理")]
            [BackendPage("interactive/manage-userprop.aspx")]
            Manage_UserProp,

            [PermissionItem("获取物品记录")]
            [BackendPage("interactive/manage-usergetprop.aspx")]
            Manage_PropGet,

            #endregion

            #region "扩展性"分组

            [PermissionItem("插件管理")]
            [BackendPage("other/manage-plugin.aspx")]
            Manage_Plugin,

            [PermissionItem("远程调用")]
            [BackendPage("other/manage-invoker.aspx")]
            Manage_Invoker,
            #endregion

            #region "广告设置"分组

            [PermissionItem("广告设置")]
            [BackendPage("other/setting-a.aspx")]
            Setting_A,

            #endregion

            #region "工具"分组

            [PermissionItem("重新启动")]
            [BackendPage("other/tool-restart.aspx")]
            Tool_Restart,
            
            [PermissionItem("重新统计数据")]
            [BackendPage("other/tool-updatedatas.aspx")]
            Tool_UpdateDatas,

            [PermissionItem("内存整理")]
            [BackendPage("other/tool-freememory.aspx")]
            Tool_FreeMemory

            #endregion

            #endregion
        }

        [PermissionTarget(TargetType = PermissionTargetType.Content)]
        public enum ActionWithTarget
        {
            [PermissionItem("没有权限")]
            None,

            [PermissionTarget(TargetType = PermissionTargetType.User)]
            [PermissionItem("屏蔽用户")]
            [BackendPage("user/manage-shielduers.aspx")]
            Manage_BanUsers,

            [PermissionTarget(TargetType = PermissionTargetType.User)]
            [PermissionItem("管理员组")]
            [BackendPage("user/setting-roles-manager.aspx")]
            Setting_Roles_Manager,


            #region "权限设置"选项卡

            [PermissionItem("用户权限分配")]
            [BackendPage("user/setting-permissions.aspx?t=user")]
            Setting_Permissions_User,

            [PermissionItem("管理权限分配")]
            [BackendPage("user/setting-permissions.aspx?t=manager")]
            Setting_Permission_Manager,

            #endregion

            [PermissionItem("文章管理")]
            [BackendPage("app/manage-blog.aspx")]
            Manage_Blog,

            [PermissionItem("照片管理")]
            [BackendPage("app/manage-photo.aspx")]
            Manage_Album,

            [PermissionItem("记录管理")]
            [BackendPage("app/manage-doing.aspx")]
            Manage_Doing,


            [PermissionItem("收藏管理")]
            [BackendPage("app/manage-share-data.aspx?type=favorite")]
            Manage_Favorite,

            [PermissionItem("分享管理")]
            [BackendPage("app/manage-share-data.aspx?type=share")]
            Manage_Share,

            [PermissionItem("文件管理")]
            [BackendPage("app/manage-netdisk.aspx")]
            Manage_NetDisk,

            [PermissionItem("表情管理")]
            [BackendPage("app/manage-emoticon.aspx")]
            Manage_Emoticon,

            [PermissionItem("对话管理")]
            [BackendPage("interactive/manage-chatsession.aspx")]
            Manage_Chat,

            [PermissionItem("通知管理")]
            [BackendPage("interactive/manage-notify.aspx")]
            Manage_Notify,

            [PermissionItem("评论管理")]
            [BackendPage("app/manage-comment.aspx")]
            Manage_Comment,
        }
    }
}