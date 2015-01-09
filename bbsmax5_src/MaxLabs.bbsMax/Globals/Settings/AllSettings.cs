//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MaxLabs.bbsMax.Settings
{
    /// <summary>
    /// 系统中所有的设置
    /// </summary>
    public class AllSettings
    {
        /// <summary>
        /// 当前程序所有设置
        /// </summary>
        public static AllSettings Current
        {
            get;
            internal set;
        }

        public ChatSettings ChatSettings = new ChatSettings();

        public NotifySettings NotifySettings = new NotifySettings();

        public UserSettings UserSettings = new UserSettings();

        public AdvertSettings AdvertSettings = new AdvertSettings();

        public AjaxSettings AjaxSettings = new AjaxSettings();

        public SiteSettings SiteSettings = new SiteSettings();
#if !Passport
        public BbsSettings BbsSettings = new BbsSettings();
#endif
        public EmailSettings EmailSettings = new EmailSettings();

        public GoogleSeoSettings GoogleSeoSettings = new GoogleSeoSettings();

        public DateTimeSettings DateTimeSettings = new DateTimeSettings();

        public BaseSeoSettings BaseSeoSettings = new BaseSeoSettings();

        public AvatarSettings AvatarSettings = new AvatarSettings();

        public PointLogClearSettings PointLogClearSettings = new PointLogClearSettings();

        public PassportServerSettings PassportServerSettings = new PassportServerSettings();

        /// <summary>
        /// 手机认证设置
        /// </summary>
        public PhoneValidateSettings PhoneValidateSettings = new PhoneValidateSettings();

        /// <summary>
        /// 登录设置
        /// </summary>
        public LoginSettings LoginSettings = new LoginSettings();

        /// <summary> 
        /// 注册设置
        /// </summary>
        public RegisterSettings RegisterSettings = new RegisterSettings();

        /// <summary>
        /// 找回密码设置
        /// </summary>
        public RecoverPasswordSettings RecoverPasswordSettings = new RecoverPasswordSettings();

        /// <summary>
        /// 注册限制设置
        /// </summary>
        public RegisterLimitSettings RegisterLimitSettings = new RegisterLimitSettings();

        /// <summary>
        /// 访问限制设置
        /// </summary>
        public AccessLimitSettings AccessLimitSettings = new AccessLimitSettings();

        /// <summary>
        /// 邀请设置
        /// </summary>
        public InvitationSettings InvitationSettings = new InvitationSettings();

        /// <summary>
        /// 打招呼动作设置
        /// </summary>
        public HailSettings HailSettings = new HailSettings();

        /// <summary>
        /// 好友设置
        /// </summary>
        public FriendSettings FriendSettings = new FriendSettings();

        /// <summary>
        /// 友情链接
        /// </summary>
        public LinkSettings LinkSettings = new LinkSettings();

        /// <summary>
        /// 顶部导航
        /// </summary>
        //public TopLinkSettings TopLinkSettings = new TopLinkSettings();

        /// <summary>
        /// 顶部导航
        /// </summary>
        public TopLinkSettings2 TopLinkSettings2 = new TopLinkSettings2();

        /// <summary>
        /// 内容关键字过虑
        /// </summary>
        public ContentKeywordSettings ContentKeywordSettings = new ContentKeywordSettings();

        /// <summary>
        /// 实名认证设置
        /// </summary>
        public NameCheckSettings NameCheckSettings = new NameCheckSettings();

        /// <summary>
        /// 友好URL设置
        /// </summary>
        public FriendlyUrlSettings FriendlyUrlSettings = new FriendlyUrlSettings();

        /// <summary>
        /// 用户扩展字段设置
        /// </summary>
        public ExtendedFieldSettings ExtendedFieldSettings = new ExtendedFieldSettings();

        /// <summary>
        /// 积分策略设置
        /// </summary>
        public PointActionSettings PointActionSettings = new PointActionSettings();

        /// <summary>
        /// 积分类型设置
        /// </summary>
        public PointSettings PointSettings = new PointSettings();

        /// <summary>
        /// 验证码设置
        /// </summary>
        public ValidateCodeSettings ValidateCodeSettings = new ValidateCodeSettings();

        public DeleteOperationLogJobSettings DeleteOperationLogJobSettings = new DeleteOperationLogJobSettings();

        public PaySettings PaySettings = new PaySettings();

        /// <summary>
        /// 用户组设置
        /// </summary>
        public RoleSettings RoleSettings = new RoleSettings();

        public MedalSettings MedalSettings = new MedalSettings();

        public DownloadSettings DownloadSettings = new DownloadSettings();

        public ImpressionSettings ImpressionSettings = new ImpressionSettings();

        public BackendPermissions BackendPermissions = new BackendPermissions();

        public SkinSettings SkinSettings = new SkinSettings();

        public CacheSettings CacheSettings = new CacheSettings();

        /// <summary>
        /// 对权限的设置
        /// </summary>
        public PermissionSettings PermissionSettings = new PermissionSettings();


        //=================================================================================

        //public InviteSerialPermissionSet InviteSerialPermissionSet;
        /// <summary>
        /// 用户权限设置
        /// </summary>
        public UserPermissionSet UserPermissionSet = new UserPermissionSet();

        public ManageUserPermissionSet ManageUserPermissionSet = new ManageUserPermissionSet();


#if !Passport
        /// <summary>
        /// 搜索引擎屏蔽设置
        /// </summary>
        public ShieldSpiderSettings ShieldSpiderSettings = new ShieldSpiderSettings();
        public NavigationSettings NavigationSettings = new NavigationSettings();

        /// <summary>
        /// Passport
        /// </summary>
        //public PassportClientConfig PassportClientSettings = new PassportClientConfig();

        public RateSettings RateSettings = new RateSettings();

        public DiskSettings DiskSettings = new DiskSettings();

        public DefaultEmotSettings DefaultEmotSettings = new DefaultEmotSettings();

        public PostIndexAliasSettings PostIndexAliasSettings = new PostIndexAliasSettings();

        public EmoticonSettings EmoticonSettings = new EmoticonSettings();

        public SearchSettings SearchSettings = new SearchSettings();

        public ForumSettings ForumSettings = new ForumSettings();

        public JudgementSettings JudgementSettings = new JudgementSettings();

        public NetDiskSettings NetDiskSettings = new NetDiskSettings();

        public PointShowSettings PointShowSettings = new PointShowSettings();

        /// <summary>
        /// 日志设置
        /// </summary>
        public BlogSettings BlogSettings = new BlogSettings();

        /// <summary>
        /// 相册设置
        /// </summary>
        public AlbumSettings AlbumSettings = new AlbumSettings();

        /// <summary>
        /// 默认个人隐私设置
        /// </summary>
        public PrivacySettings PrivacySettings = new PrivacySettings();


        /// <summary>
        /// 帖子图标设置
        /// </summary>
        public PostIconSettings PostIconSettings = new PostIconSettings();

        public FeedJobSettings FeedJobSettings = new FeedJobSettings();

        public OnlineSettings OnlineSettings = new OnlineSettings();

        public BaiduPageOpJopSettings BaiduPageOpJopSettings = new BaiduPageOpJopSettings();

        public ShareSettings ShareSettings = new ShareSettings();

        public FavoriteSettings FavoriteSettings = new FavoriteSettings();

        public DoingSettings DoingSettings = new DoingSettings();

        public MissionSettings MissionSettings = new MissionSettings();

        public SpaceSettings SpaceSettings = new SpaceSettings();

        public PropSettings PropSettings = new PropSettings();

        //权限

        public SpacePermissionSet SpacePermissionSet = new SpacePermissionSet();

        public ForumPermissionSet ForumPermissionSet= new ForumPermissionSet();

        public ManageForumPermissionSet ManageForumPermissionSet = new ManageForumPermissionSet();

#endif


        public SettingBase GetSettingFieldValue(FieldInfo field)
        {
            return (SettingBase)field.GetValue(this);
        }

        public void SetSettingFieldValue(FieldInfo field, SettingBase value)
        {
            field.SetValue(this, value);
        }
    }
}