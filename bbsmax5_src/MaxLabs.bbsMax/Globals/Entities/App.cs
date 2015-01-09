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

namespace MaxLabs.bbsMax.Entities
{
    public class BasicApp : AppBase
    {
        public override string AppName
        {
            get { return "基本功能"; }
        }

        public override Guid AppID
        {
            get { return Consts.App_BasicAppID; }
        }

        public override AppActionCollection AppActions
        {
            get 
            {
                AppActionCollection appActions = new AppActionCollection();

                AppAction appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.Share;
                appAction.DisplayComments = true;
                appAction.ActionName = "分享";
                appAction.IconSrc = "~/max-assets/icon/share.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 分享了{catagory} <span class=\"date\">{dateTime}</span></div>";//{title} {dateTime}";
                appAction.TitleTemplateTags = new string[] { "{catagory}" };
                appAction.DescriptionTemplate = @"
{content}
<div class=""description"">
    <div class=""description-inner"">{description}</div>
</div>
";
                appAction.DescriptionTemplateTags = new string[] { "{content}", "{description}" };

                appActions.Add(appAction);


                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.AddFriend;
                appAction.ActionName = "加好友";
                appAction.IconSrc = "~/max-assets/icon/friend.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 和 {targetUser} 成为好友 <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = null;
                appAction.DescriptionTemplate = null;
                appAction.DescriptionTemplateTags = null;

                appActions.Add(appAction);

                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.UploadPicture;
                appAction.ActionName = "上传相片";
                appAction.IconSrc = "~/max-assets/icon/album.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 上传了新图片  <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = null;
                appAction.DescriptionTemplate = @"
<div class=""photothumb"">
{pictures}
</div>
<div class=""detail"" style=""clear: both;"">
<b>{album}</b><br>共 {picturesCount} 张图片
</div>
";
                appAction.DescriptionTemplateTags = new string[] { "{pictures}", "{album}", "{picturesCount}" };

                appActions.Add(appAction);

                appAction = new AppAction();
                appAction.CanJoin = false;
                appAction.ActionType = (int)AppActionType.AddComment;
                appAction.ActionName = "评论";
                appAction.IconSrc = "~/max-assets/icon/comment.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 评论了 {targetUser} {type} {title}   <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = new string[] { "{type}", "{title}"};
                appAction.DescriptionTemplate = "{content}";
                appAction.DescriptionTemplateTags = new string[] { "{content}" };

                appActions.Add(appAction);

                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.LeaveMessage;
                appAction.ActionName = "留言";
                appAction.IconSrc = "~/max-assets/icon/leavemsg.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 在 {targetUser} 的留言板留了言 <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = null;
                appAction.DescriptionTemplate = "";
                appAction.DescriptionTemplateTags = null;

                appActions.Add(appAction);

                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.UpdateDoing;
                appAction.DisplayComments = true;
                appAction.ActionName = "更新心情";
                appAction.IconSrc = "~/max-assets/icon/doing.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 更新了心情： {content} <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = new string[] { "{content}" };
                appAction.DescriptionTemplate = null;
                appAction.DescriptionTemplateTags = null;

                appActions.Add(appAction);



                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.WriteArticle;
                appAction.DisplayComments = true;
                appAction.ActionName = "发表日志";
                appAction.IconSrc = "~/max-assets/icon/blog.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 发表了新日志 <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = null;
                appAction.DescriptionTemplate = @"
{content}
";
                appAction.DescriptionTemplateTags = new string[] { "{content}"};

                appActions.Add(appAction);


                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.CreateTopic;
                appAction.ActionName = "发表主题";
                appAction.IconSrc = "~/max-assets/icon/topic.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 发表了新主题 <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = null;
                appAction.DescriptionTemplate = @"
{content}
";
                appAction.DescriptionTemplateTags = new string[] { "{content}" };

                appActions.Add(appAction);


                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.ApplyMission;
                appAction.ActionName = "参与任务";
                appAction.IconSrc = "~/max-assets/icon/task.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 参与了任务 {missionName} <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = new string[] { "{missionName}" };
                appAction.DescriptionTemplate = null;
                appAction.DescriptionTemplateTags = null;

                appActions.Add(appAction);


                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.OpenSpace;
                appAction.ActionName = "开通主页";
                appAction.IconSrc = "~/max-assets/icon/space.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 开通了自己的个人主页 <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = null;
                appAction.DescriptionTemplate = null;
                appAction.DescriptionTemplateTags = null;

                appActions.Add(appAction);


                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.UpdateUserProfile;
                appAction.ActionName = "更新资料";
                appAction.IconSrc = "~/max-assets/icon/vcard.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 更新了自己的个人资料 <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = null;
                appAction.DescriptionTemplate = null;
                appAction.DescriptionTemplateTags = null;

                appActions.Add(appAction);


                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.UpdateAvatar;
                appAction.ActionName = "更新头像";
                appAction.IconSrc = "~/max-assets/icon/people.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 更新了自己的头像 <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = null;
                appAction.DescriptionTemplate = null;
                appAction.DescriptionTemplateTags = null;

                appActions.Add(appAction);

                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.SiteFeed;
                appAction.CanJoin = false;
                appAction.ActionName = "全局动态";
                appAction.IconSrc = "~/max-assets/icon/feeds.gif";
                appAction.TitleTemplate = "<div class=\"title\">{title} <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = new string[] { "{title}" };
                appAction.DescriptionTemplate = @"
{images}
<div class=""detail"" style=""clear: both;"">
{content}
</div>
<div class=""description"">
    <div class=""description-inner"">{description}</div>
</div>
";
                appAction.DescriptionTemplateTags = new string[] { "{content}", "{description}", "{images}" };

                appActions.Add(appAction);


                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.BidUpSelf;
                appAction.ActionName = "增加自己竞价";
                appAction.IconSrc = "~/max-assets/icon/user_up.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 增加竞价积分 {point} 个，提升自己在{link}中的名次 <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = new string[] { "{point}", "{link}" };
                appAction.DescriptionTemplate = @"
<div class=""description"">
    <div class=""description-inner"">{description}</div>
</div>
";
                appAction.DescriptionTemplateTags = new string[] { "{description}" };

                appActions.Add(appAction);



                appAction = new AppAction();
                appAction.ActionType = (int)AppActionType.BidUpFriend;
                appAction.ActionName = "增加好友竞价";
                appAction.IconSrc = "~/max-assets/icon/user_up.gif";
                appAction.TitleTemplate = "<div class=\"title\">{actor} 赠送给 {targetUser} 竞价积分 {point} 个，帮助好友提升在{link}中的名次 <span class=\"date\">{dateTime}</span></div>";
                appAction.TitleTemplateTags = new string[] { "{point}", "{link}" };
                appAction.DescriptionTemplate = null;
                appAction.DescriptionTemplateTags = null;

                appActions.Add(appAction);

                return appActions;
            }
        }

    }
    /// <summary>
    /// 内置应用动作类型
    /// </summary>
    public enum AppActionType
    {
        /// <summary>
        /// 分享
        /// </summary>
        Share = 0,

        /// <summary>
        /// 加好友
        /// </summary>
        AddFriend = 1,

        /// <summary>
        /// 上传图片
        /// </summary>
        UploadPicture = 2,

        /// <summary>
        /// 添加评论
        /// </summary>
        AddComment = 3,

        /// <summary>
        /// 留言
        /// </summary>
        LeaveMessage = 4,

        /// <summary>
        /// 更新记录
        /// </summary>
        UpdateDoing = 5,

        /// <summary>
        /// 发表日志
        /// </summary>
        WriteArticle = 6,

        /// <summary>
        /// 参与任务
        /// </summary>
        ApplyMission = 7,

        /// <summary>
        /// 更新用户资料
        /// </summary>
        UpdateUserProfile = 8,

        /// <summary>
        /// 更新头像
        /// </summary>
        UpdateAvatar = 9,

        /// <summary>
        /// 开通个人空间
        /// </summary>
        OpenSpace = 10,


        /// <summary>
        /// 全局动态
        /// </summary>
        SiteFeed = 11,


        /// <summary>
        /// 增加竞价
        /// </summary>
        BidUpSelf = 12,


        /// <summary>
        /// 赠送竞价
        /// </summary>
        BidUpFriend = 13,

        /// <summary>
        /// 发表新主题
        /// </summary>
        CreateTopic = 14,

    }
}