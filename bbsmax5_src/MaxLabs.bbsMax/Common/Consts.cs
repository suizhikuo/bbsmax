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

namespace MaxLabs.bbsMax
{
    public class Consts
    {

        public const string DefaultUserAvatar_Small = "/max-assets/avatar/avatar_24.gif";
        public const string DefaultUserAvatar_Default = "/max-assets/avatar/avatar_48.gif";
        public const string DefaultUserAvatar_Big = "/max-assets/avatar/avatar_120.gif";

        public const string ExtendedFieldID = "field_{0}";

        /// <summary>
        /// 空文件的MD5值
        /// </summary>
        public const string EmptyFileMD5 = "00000000000000000000000000000000";

        public const string EmptyMD5 = "00000000000000000000000000000000"; //"D41D8CD98F00B204E9800998ECF8427E";


        /// <summary>
        /// 游客ID
        /// </summary>
        public const int GuestID = 0;
        public const int EveryoneID = -1;

        #region Skin 相关

        public const string DefaultSkinID = "default";
        public const string SkinConfigFile = "_skin.config";

        #endregion

        public const string Reg_Email_JS = @"^[^@]+@[^\\.@]+\\.\\w+$";
        public const string Reg_Url_JS = @"^http://\\w+\\.\\w+";

        public const int NotifyContent_Length = 2000;
        public const int Friend_GroupName_Length = 20;
        public const int Doing_Length = 50;
        public const int Comment_Length = 1000;
        public const int CreateIP_Length = 50;
        public const int Report_Length = 200;
        public const int BlogArticle_ShowVisitors = 10;
        public const int BlogArticle_VisitorTimeScope = 30;
		public const int Space_VisitorTimeScope = 30;

        /// <summary>
        /// 列表页缓存前10页
        /// </summary>
        public const int ListCachePageCount = 10;

        /// <summary>
        /// 默认的分页每页的记录数
        /// </summary>
        public const int DefaultPageSize = 20;

        /// <summary>
        /// 默认的标签列表分页每页的记录数
        /// </summary>
        public const int DefaultTagListPageSize = 100;

        /// <summary>
        /// 缓存最新全站动态条数
        /// </summary>
        public const int CacheAllUserFeedsCount = 100;

        /// <summary>
        /// 分享描述长度
        /// </summary>
        public const int Share_Description_Length = 500;

        /// <summary>
        /// 分享预览 内容长度 
        /// </summary>
        public const int Share_ReviewContentLength = 200;

        /// <summary>
        /// 动态图标长度
        /// </summary>
        public const int Feed_TemplateIconUrlLength = 200;

        /// <summary>
        /// 动态标题模板长度
        /// </summary>
        public const int Feed_TemplateTitleLength = 1000;

        /// <summary>
        /// 动态内容模板长度
        /// </summary>
        public const int Feed_TemplateDescriptionLength = 2500;


        public const int Forum_CodeNameLength = 128;
        public const int Forum_NameLength = 1024;
        public const int Forum_LogoSrcLength = 256;
        public const int Forum_PasswordLength = 64;
        public const int Forum_ThemeIDLength = 64;
        public const int Forum_ThreadCatalogNameLength = 64;
        public const int Forum_ThreadCatalogLogoUrlLength = 512;

        public const string User_UncheckAvatarSuffix = @"uncheck";

        /// <summary>
        /// 发表日志的动态 显示日志内容长度
        /// </summary>
        public const int Feed_ContentSummaryLength = 200;

        /// <summary>
        /// 添加全局动态 时添加的用户ID（所有用户的好友动态里都能收到这条动态）
        /// </summary>
        public const int Feed_SiteFeedUserID = -1;

        public const int Network_TopPhotoHeight = 320;
        public const int Network_TopPhotoWidth = 320;

        public const int Photo_ThumbnailWidth = 100;
        public const int Photo_ThumbnailHeight = 75;

        /// <summary>
        /// 任务完成条件 表单对象名称前缀
        /// </summary>
        public const string Mission_FinishConditionPrefix = "mission_";

        public const int Mission_MaxMissionNameLength = 100;

        public const int Mission_MaxMissionIconUrlLength = 200;

        public static readonly Guid App_BasicAppID = new Guid(new byte[] { 145, 29, 92, 245, 105, 201, 53, 75, 140, 197, 171, 129, 220, 175, 45, 229 });

        /// <summary>
        /// 扩展积分个数 包括未启用的积分
        /// </summary>
        public const int PointCount = 8;

        public const string ValidateCode_SessionKey_Prefix = "Max_ValidateCode_";

        /// <summary>
        /// 验证码输入框 名称 
        /// </summary>
        public const string ValidateCode_InputName = "validateCodeInput_{0}{1}";

        /// <summary>
        /// 输出验证码图片的地址
        /// </summary>
        public const string ValidateCode_ImageUrl = "handler/vcode";
		public const string ValidateCode_ImageUrlQuery = "type={0}&isstyletype={1}&id={2}&r={3}";

        public static readonly string Handler_JobUrl  = "handler/" + HandlerAction_ExecuteJobs;

        public const string HandlerAction_ExecuteJobs = "job";

        /// <summary>
        /// 保存文件的 扩展名（包括点 如".config"）
        /// </summary>
        public const string FileSystem_FileExtendName = "";

        /// <summary>
        /// 保存图片缩略图的 扩展名（包括点 如".png"）
        /// </summary>
        public const string FileSystem_ThumbnailImageExtendName = ".png";

        /// <summary>
        /// 缩略图的保存目录  如："UserFiles\MAXFS_Thumbnail_{0}\"    {0}:"宽_高"
        /// </summary>
        public const string FileSystem_ThumbnailImageDirecotry = @"UserFiles\MAXFS_Thumbnail_{0}";

        /// <summary>
        /// 临时文件扩展名
        /// </summary>
        public const string FileSystem_TempFileExtendName = ".config";

        /// <summary>
        /// 错误代码 表示系统抛出的错误
        /// </summary>
        public const int ExceptionCode = -1000000;

    }
}