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

namespace MaxLabs.bbsMax.Rescourses
{
    public partial class Lang
    {
        public const string AD_Double = "对联广告";

        public const string AD_Double_Description = "对联广告以长方形图片的形式显示于页面顶部两侧，形似一幅对联，通常使用宽小高大的长方形图片或 Flash 的形式。对联广告一般只在使用像素约定主表格宽度的情况下使用，如使用超过 90% 以上的百分比约定主表格宽度时，可能会影响访问者的正常浏览。当访问者浏览器宽度小于 800 像素时，自动不显示此类广告。当前页面有多个对联广告时，系统会随机选取其中之一显示。<font color=\"red\">注意：</font>漂浮广告和对联广告是通过JS输出的， 因此不能放JS的广告代码。";

        public const string AD_Format_Image = "图片";

        public const string AD_Format_Flash = "Flash";

        public const string AD_Format_Link = "文字链接";

        public const string AD_Format_Code = "HTML代码";

        public const string AD_Float = "漂浮广告"; 

        public const string AD_Float_Description = "漂浮广告展现于页面右下角，当页面滚动时广告会自行移动以保持原来的位置，通常使用小图片或 Flash 的形式。当前页面有多个漂浮广告时，系统会随机选取其中之一显示。<font color=\"red\">注意：</font>漂浮广告和对联广告是通过JS输出的， 因此不能放JS的广告代码。";

        public const string AD_Header = "头部横幅";

        public const string AD_Header_Description = "头部横幅广告显示于论坛页面右上方，通常使用 468x60 图片或 Flash 的形式。当前页面有多个头部横幅广告时，系统会随机选取其中之一显示。";

        public const string AD_Footer = "底部横幅";

        public const string AD_Footer_Description = "底部横幅广告显示于论坛页面中下方，通常使用 480×60 或其他尺寸图片、Flash 的形式。当前页面有多个底部横幅广告时，系统会随机选取其中之一显示。";

        public const string AD_InForum = "分类间广告";

        public const string AD_InForum_Description = "分类间广告显示于论坛首页相邻的两个论坛分类之间，可使用 468x60 或其他尺寸图片和 Flash 的形式。当前页面有多个分类间广告时，系统会从中抽取与论坛分类数相等的条目进行顺序显示。";

        public const string AD_InList = "置顶区横幅广告";

        public const string AD_InList_Description = "置顶区横幅广告于主题列表页置顶主题和普通主题间，可使用 468x60 或其他尺寸图片和 Flash 的形式。当前页面有多个置顶区横幅广告时，系统会从中抽取与论坛分类数相等的条目进行显示。";

        public const string AD_TopBanner = "顶部通栏";

        public const string AD_TopBanner_Description = "顶部通栏广告显示于导航链接和版块规则之间，可使用 468x60, 728x90 或其他尺寸图片和 Flash 的形式。当前页面有多个顶部通栏广告时，系统会随机选取其中之一显示。";

        public const string AD_PostLeaderboard = "帖间通栏";

        public const string AD_PostLeaderboard_Description = "帖间通栏广告显示于主题帖和第一个回帖之间，可使用 468x60, 728x90 或其他尺寸图片和 Flash 的形式。当前页面有多个帖间通栏广告时，系统会随机选取其中之一显示。";

        public const string AD_Signature = "签名广告";

        public const string AD_Signature_Description = "签名广告显示于主题帖用户签名下方，可使用 650×60 像素或其他尺寸图片和 Flash 的形式。 当前页面有多个签名广告是，系统会随机选取其中之一显示。";

        public const string AD_InPost = "帖内广告";

        public const string AD_InPost_Description = "帖内广告显示于帖子内容的上方、下方或右方，帖子内容的上方和下方通常使用文字的形式，帖子内容右方通常使用图片的形式。当前页面有多个帖内广告时，系统会从中抽取与每页帖数相等的条目进行随机显示。";

        public const string AD_PageWord = "页内文字";

        public const string AD_PageWord_Description = "页内文字广告以表格的形式，显示于首页、主题列表和帖子内容三个页面的中上方，通常使用文字的形式，也可使用小图片和 Flash。当前页面有多个文字广告时，系统会以表格的形式按照设定的显示顺序全部展现，同时能够对表格列数在 3～5 的范围内动态排布，以自动实现最佳的广告排列效果。";

        public const string Common_AtChar = @"@符号";

        public const string Common_Blank = @"空格（不包括前后空格）";

        public const string Common_BeforeYesterday = @"前天";

        public const string Common_ChineseChar = @"中文字符";

        public const string Common_Day = @"天";

        public const string Common_DotChar = @"小数点";

        public const string Common_EnglishChar = @"英文字母";

        public const string Common_Expires = @"已过期";

        public const string Common_Hour = @"小时";

        public const string Common_HourAgo = @"{0}小时前";//N小时前

        public const string Common_Indefinitely = @"无限期";

        public const string Common_Month = @"月";

        public const string Common_Minute = @"分钟";

        public const string Common_MineteAgo = @"{0}分钟前";//N分钟前

        public const string Common_NumberChar = @"数字";

        public const string Common_Now = @"刚才";

        public const string Common_Second = @"秒";

        public const string Common_SecondAgo = @"{0}秒前"; //N秒钟前

        public const string Common_UnderlineChar = @"下划线";

        public const string Common_Unused = @"未使用";

        public const string Common_Used = @"已使用";

        public const string Common_Year = @"年";

        public const string Common_Yesterday = @"昨天";

        public const string Emoticon_DefaultGroupName = "我的表情";

        public const string Error_NoPermissionSetFeedTemplate = @"您没有权限设置动态的模板";

        public const string Gender_Female = @"美女";

        public const string Gender_Male = @"帅哥";

        public const string Gender_NotSet = @"保密";

        public const string Invite = @"邀请";

        public const string Invite_NewUser = @"邀请新用户";

        public const string Invite_SerialPrice = @"购买邀请码";



		public const string PermissionItem_BackendManageSettings = "系统设置管理权限";
		public const string PermissionItem_BackendManageSettings_ManageGlobalSettings = "全局设置";
		public const string PermissionItem_BackendManageSettings_ManageRegisterAndAccessSettings = "注册与访问设置";
		public const string PermissionItem_BackendManageSettings_ManageUserAndInteractiveSettings = "用户与交互设置";
		public const string PermissionItem_BackendManageSettings_ManageDisplaySettings = "显示与体验设置";
		public const string PermissionItem_BackendManageSettings_ManageSearchEngineOptimizeSettings = "SEO设置";
        public const string PermissionItem_BackendManageSettings_ManageValidateCodeSettings = "验证码设置";
		public const string PermissionItem_BackendManageUser = "系统设置管理用户";
		public const string PermissionItem_BackendManageUser_ManageUser = "管理用户";
		public const string PermissionItem_BackendManageUser_ManageUserGroup = "管理用户组";
		public const string PermissionItem_BackendManageUser_ManageUserPoint = "管理积分与等级";
		public const string PermissionItem_BackendManageUser_ManageExtendedField = "管理扩展字段";
		public const string PermissionItem_BackendManageUser_ManageMedal = "管理勋章";
		public const string PermissionItem_BackendManageUser_ManageInviteSerial = "管理邀请码";
		public const string PermissionItem_BackendManageUser_ManagePermission = "管理权限";
        //public const string PermissionItem_BackendManageSettings_UserEmoticonSettings = "用户表情设置";
        public const string PermissionItem_BackendManageSettings_NetDiskSettings = "网络硬盘设置";

        //public const string PermissionItem_ManageInviteSerialPermissionSet = "邀请码管理";
        public const string PermissionItem_ManageSpacePermission = "应用管理权限";
        public const string PermissionItem_ManageSpacePermissionSet_ManageDoing = "管理记录";
        public const string PermissionItem_ManageSpacePermissionSet_ManageBlog = "管理日志";
        public const string PermissionItem_ManageSpacePermissionSet_ManageAlbum = "管理相册";
        public const string PermissionItem_ManageSpacePermissionSet_ManageShareAndCollection = "管理分享和收藏";
        public const string PermissionItem_ManageSpacePermissionSet_ManageComment = "管理评论";
        //public const string PermissionItem_ManageUserPermissionSet = "用户管理权限";
        public const string PermissionItem_ManageUserPermissionSet_DeleteUser  = "删除用户";
        public const string PermissionItem_ManageUserPermissionSet_ManageUser  = "管理用户资料";
        public const string PermissionItem_ManageUserPermissionSet_ChangeGroup = "修改用户组";
        //public const string PermissionItem_ManageUserPermissionSet_EditPoint   = "修改积分";
        //public const string PermissionItem_ManageUserPermissionSet_Validation  = "实名与头像认证";
        public const string PermissionItem_SpacePermission = "应用权限";
        public const string PermissionItem_SpacePermissionSet_UseDoing = "使用记录";
        public const string PermissionItem_SpacePermissionSet_UseBlog = "使用日志";
        public const string PermissionItem_SpacePermissionSet_UseAlbum = "使用相册";
        public const string PermissionItem_SpacePermissionSet_UseShare = "使用分享";
        public const string PermissionItem_SpacePermissionSet_UseCollection = "使用收藏";
        public const string PermissionItem_SpacePermissionSet_AddComment = "发表评论";
        public const string PermissionItem_SpacePermissionSet_UseImpression = "使用好友印象";
        //public const string PermissionItem_UserPermissionSet_Login = "登录";
		public const string PermissionItem_UserPermissionSet_UseMessage = "使用短消息功能";
        //public const string PermissionItem_UserPermissionSet_RatePoint = "评{0}";

        public const string PermissionItem_ManageOtherPermission = "其它权限";
        public const string PermissionItem_ManageOtherPermission_ManageMission = "管理任务";
        public const string PermissionItem_ManageOtherPermission_ManageFeed = "管理动态";
        public const string PermissionItem_ManageOtherPermission_ManageLink = "管理友情链接";
        //public const string PermissionItem_ManageOtherPermission_ManageAnnouncement = @"管理公告";
        public const string PermissionItem_ManageOtherPermission_ManageOnlineList = "管理在线列表";
        public const string PermissionItem_ManageOtherPermission_ManageJudgement = "管理主题鉴定图标";
        public const string PermissionItem_ManageOtherPermission_ManagePostIcon = "管理帖子图标";

        //public const string PermissionItem_ManageAdvertPermission = "广告管理";

        public const string Role_Everyone = @"任何人";

        public const string Role_Guests = @"游客";

        public const string Role_Users = @"注册用户";

        public const string Role_NewUsers = @"见习用户";

        public const string Role_RealnameNotProvedUsers = @"未通过实名认证用户";

        public const string Role_AvatarNotProvedUsers = @"头像未认证用户";

        public const string Role_EmailNotProvedUsers = @"Email未认证用户";

        public const string Role_InviteLessUsers = @"未通过邀请码认证用户";

        public const string Role_JackarooModerators = "实习版主";

        public const string Role_Moderators = @"版主";

        public const string Role_CategoryModerators = @"分类版主";

        public const string Role_SuperModerators = @"超级版主";

        public const string Role_Administrators = @"管理员";

        public const string Role_Owner = @"创始人";

        public const string Role_BannedUsers = @"版块屏蔽用户";

        public const string Role_FullSiteBannedUsers = @"整站屏蔽用户";

        public const string RoleType_Custom = @"自定义";

        public const string RoleType_Level = @"用户等级";

        public const string RoleType_System = @"系统内置";

        public const string RoleType_Admin = @"管理员";

        public const string Role_Beggar = @"丐帮弟子";


        public const string Setting_ForumSetting_ForumCloseReason = @"本论坛暂时关闭，正在进行系统维护，请稍后再尝试访问。";

        public const string Setting_RegisterSetting_ActivationEmailContent = @"亲爱的{username}： <br />
&nbsp;&nbsp;&nbsp;&nbsp;您的帐号激活邮件<br />
请复制下面的激活链接到浏览器进行访问，以便激活您的帐号。<br />
邮箱激活链接:
<br />
{url}<br/><br/>
{sitename}<br/>
{site}<br />
{now}<br />
==========================================================<br />
此邮件为系统自动发出的邮件，请勿直接回复。";

        public const string Setting_RegisterSetting_ActivationEmailTitle = @"[{sitename}] 您的帐号激活邮件";

        public const string Setting_RegisterSetting_EmailValidationContent = @"亲爱的{username}：<br />
&nbsp;&nbsp;&nbsp;&nbsp;您的邮箱激活邮件<br />
请复制下面的激活链接到浏览器进行访问，以便激活您的邮箱。<br />
邮箱激活链接:
<br />
{url}<br/><br />
{sitename}<br />
{site}<br />
{now}<br />
==========================================================<br />
此邮件为系统自动发出的邮件，请勿直接回复。";

        public const string Setting_RegisterSetting_EmailValidationTitle = @"[{sitename}] 您的邮箱激活邮件";

        public const string Setting_RegisterSetting_WelcomeMailTitle = "欢迎加入{username}{sitename}";

        public const string Setting_RegisterSetting_WelcomeContent = @"亲爱的{username}:<br />

欢迎您加入{sitename}，请牢记您在本站的用户名和密码。<br />
用户名：{username}<br />
密  码：{password}<br />
==========================================================<br />
{sitename} {siteurl}<br />
{now}
";

        public const string ShareCatagory_Album = @"一个相册";

        public const string ShareCatagory_Blog = @"一篇日志";

        public const string ShareCatagory_Flash = @"一个Flash";

        public const string ShareCatagory_Forum = @"一个群组";

        public const string ShareCatagory_Music = @"一个音乐";

        public const string ShareCatagory_News = @"一篇新闻";

        public const string ShareCatagory_Picture = @"一张图片";

        public const string ShareCatagory_Tag = @"一个TAG";

        public const string ShareCatagory_Topic = @"一个主题";

        public const string ShareCatagory_URL = @"一个网址";

        public const string ShareCatagory_User = @"一个用户";

        public const string ShareCatagory_Video = @"一个视频";

        public const string SharePointType_CreateCollection = @"添加收藏";

        public const string SharePointType_CreateShare = @"添加分享";

        public const string SharePointType_ShareWasCommeted = @"分享被评论";

        public const string SharePointType_ShareWasDeletedByAdmin = @"分享被别人删除";

        public const string SharePointType_ShareWasDeletedBySelf = @"分享被自己删除";

        public const string SharePointTypeName = @"分享";

        public const string TotalPointName = @"总积分";

        public const string User = @"用户";

        public const string User_AvatarUpdatePoint = @"更新头像";

        public const string User_EmailCheckedPoint = @"通过邮箱验证";

        public const string User_InitalPoint = @"新用户初始值";

        public const string User_NoEditProfilePermission = @"您没有修改{0}用户信息的权限";

        public const string User_PerfectInfomationPoint = @"完善用户资料";

        public const string User_RealnameCheckPoint = @"通过实名认证";

        public const string User_RegisterAgreement = @"继续注册前请先阅读以下协议<br /><br /></p><p>欢迎您加入本站点参加交流和讨论，为维护网上公共秩序和社会稳定，请您自觉遵守以下条款：<br />

一、 不得利用本站危害国家安全、泄露国家秘密，不得侵犯国家社会集体的和公民的合法权益，不得利用本站制作、复制和传播下列信息： 
<br />      （一）煽动抗拒、破坏宪法和法律、行政法规实施的；
<br />      （二）煽动颠覆国家政权，推翻社会主义制度的；
<br />      （三）煽动分裂国家、破坏国家统一的；
<br />      （四）煽动民族仇恨、民族歧视，破坏民族团结的；
<br />      （五）捏造或者歪曲事实，散布谣言，扰乱社会秩序的；
<br />      （六）宣扬封建迷信、淫秽、色情、赌博、暴力、凶杀、恐怖、教唆犯罪的；
<br />      （七）公然侮辱他人或者捏造事实诽谤他人的，或者进行其他恶意攻击的；
<br />      （八）损害国家机关信誉的；
<br />      （九）其他违反宪法和法律行政法规的；

<br />      （十）进行商业广告行为的。

<br /></p><p>二、互相尊重，对自己的言论和行为负责。</p><p>三、您必需同意不发表带有辱骂,淫秽,粗俗,诽谤,带有仇恨性,恐吓的,不健康的或是任何违反法律的内容. 如果您这样做将导致您的账户将立即和永久性的被封锁.(您的网络服务提供商也会被通知). 在这个情况下,这个IP地址的所有用户都将被记录.您必须同意系统管理成员们有在任何时间删除,修改,移动或关闭任何内容的权力. 作为一个使用者, 您必须同意您所提供的任何资料都将被存入数据库中,这些资料除非有您的同意,系统管理员们绝不会对第三方公开,然而我们不能保证任何可能导致资料泄露的骇客入侵行为.

本系统使用cookie来储存您的个人信息(在您使用的本地计算机), 这些cookie不包含任何您曾经输入过的信息,它们只为了方便您能更方便的浏览. 电子邮件地址只用来确认您的注册和发送密码使用.(如果您忘记了密码,将会发送新密码的地址)

点击下面的按钮代表您同意受到这些服务条款的约束. 

";

        public const string User_UserNameFormat = @"用户名由{0}到{1}位的{2}组成";

        public const string User_UserPointDeletedRepliesColumName = @"被删帖子数";

        public const string User_UserPointDeletedTopicsColumName = @"被删主题数";

        public const string User_UserPointIconUpgradeDescription = @"{0}每满 {1} 个，就会拥有一个初级图标 {2}
每满 {3} 个当前图标就升级为 1 个上级图标
图标等级由高到低为：{4}";

        public const string User_UserPointNotSetIcon = @"";

        public const string User_UserPointOnlineColumName = @"总在线时间(单位分钟)";

        public const string User_UserPointTotalRepliesColumName = @"发帖总数";

        public const string User_UserPointTotalTopicsColumName = @"主题总数";

        public const string User_UserPointValuedTopicsColumName = @"精华帖子数";

        public const string User_PhoneBindSmsContent = @"尊敬的用户,您手机绑定的验证码为{0},请输入验证码完成绑定.";

        public const string User_PhoneUnbindSmsContent = @"尊敬的用户,您解除手机绑定的验证码为{0},请输入验证码解除绑定.";

        public const string User_PhoneChangeSmsForOldPhoneContent = @"尊敬的用户,您更改手机绑定的验证码为{0},请输入验证码完成更改绑定.";

        public const string User_PhoneChangeSmsForNewPhoneContent = @"尊敬的用户,您要绑定的新手机验证码为{0},请输入验证码完成更改绑定.";
    }
}