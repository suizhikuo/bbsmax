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
using System.Configuration;


using MaxLabs.WebEngine.Plugin;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Extensions;
using MaxLabs.bbsMax.Providers;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Jobs;
using MaxLabs.bbsMax.AppHandlers;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.XCmd;

namespace MaxLabs.bbsMax
{
	/// <summary>
	/// 业务逻辑初始化类
	/// </summary>
	public class Booter
	{
		/// <summary>
		/// 初始化业务逻辑层
		/// </summary>
        /// 
		public static void Init()
		{
			//TODO:错误处理，不能因为异常导致程序启动失败

            //注册应用程序
            AppManager.RegisterApp(new BasicApp());

            #region 注册AppHandler

            AppHandlerManager.RegisterAppHandler(new DownloadHandler());
            AppHandlerManager.RegisterAppHandler(new DeleteTempFileHandler());
            AppHandlerManager.RegisterAppHandler(new OutputValidateCodeHandler());
            AppHandlerManager.RegisterAppHandler(new ExecuteJobHandler());
            AppHandlerManager.RegisterAppHandler(new StepByStepTaskHandler());
            AppHandlerManager.RegisterAppHandler(new UploadTempFileHandler());
      
            AppHandlerManager.RegisterAppHandler(new RegValidateHandler());
            AppHandlerManager.RegisterAppHandler(new ChatMessageHandler());
            AppHandlerManager.RegisterAppHandler(new AvatarHandler());
            AppHandlerManager.RegisterAppHandler(new TempDataHandler());
#if !Passport
            AppHandlerManager.RegisterAppHandler(new Js_EmoticonHandler());
            AppHandlerManager.RegisterAppHandler(new NotifyHandler());
			AppHandlerManager.RegisterAppHandler(new DoingHandler());
            AppHandlerManager.RegisterAppHandler(new ChangeSkinHandler());
            AppHandlerManager.RegisterAppHandler(new OnlineInfoHandler());
            AppHandlerManager.RegisterAppHandler(new PostAuthorInfoHandler());
            //AppHandlerManager.RegisterAppHandler(new AttachmentHandler());
#endif

            #endregion
            
            #region 注册积分动作规则

            PointActionManager.RegisterPointActionType(UserPointAction.Instance);
            PointActionManager.RegisterPointActionType(InvitePointAction.Instance);
#if !Passport
            PointActionManager.RegisterPointActionType(SharePointAction.Instance);
            PointActionManager.RegisterPointActionType(BlogPointAction.Instance);
            //PointActionManager.RegisterPointActionType(PointShowPointAction.Instance);
            PointActionManager.RegisterPointActionType(CommentPointAction.Instance);
            PointActionManager.RegisterPointActionType(AlbumPointAction.Instance);
            PointActionManager.RegisterPointActionType(DoingPointAction.Instance);
            PointActionManager.RegisterPointActionType(ForumPointAction.Instance);
#endif
            #endregion

            #region 注册验证码样式

            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style1());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style2());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style3());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style4());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style5());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style6());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style7());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style8());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style9());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style10());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style11());
            ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style12());

            #endregion

			#region 注册验证码动作

			ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("注册", "Register", false));
			ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("登录", "Login", false));
			ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("管理员登陆", "ManageLogin", false));
			ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("找回密码", "recoverpassword", false));

            ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("发表分享", "CreateShare", true));
            //ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("发表收藏", "CreateCollection", true));
			ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("发表日志", "CreateBlogArticle", true));
			//ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("发表记录", "CreateDoing", true));
			ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("发表评论", "CreateComment", true));

            //论坛---
            ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("发表主题", "CreateTopic", true));
            ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("回复主题", "ReplyTopic", true));
            ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("参与投票", "Vote", true));
            ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("登陆带密码版块", "SignInForum", true));
            ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("加好友","AddFriend",true));
            ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("对话", "SendMessage", true));
            ValidateCodeManager.RegisterValidateCodeAction(new ValidateCodeAction("打招呼", "Hail", true));
            #endregion

            #region 注册XCmd

            XCmdManager.RegisterXCmd(new DeleteFileCmd());
            XCmdManager.RegisterXCmd(new ResetVarsCacheCmd());
            XCmdManager.RegisterXCmd(new ResetUserCacheCmd());

            XCmdManager.RegisterXCmd(new RecodeTodayPostsCmd());

#if !Passport
            
            XCmdManager.RegisterXCmd(new ResetAuthUserCacheCmd());
			XCmdManager.RegisterXCmd(new ResetAlbumCacheCmd());
            XCmdManager.RegisterXCmd(new ResetDenouncingCacheCmd());
            XCmdManager.RegisterXCmd(new ResetFeedCommentInfoCmd());

#endif

            #endregion

#if !Passport

            #region 注册 PermissionSetWithNode

            SettingManager.RegisterPermissionWithNode(new ForumPermissionSetNode().TypeName, new ForumPermissionSet());
            SettingManager.RegisterPermissionWithNode(new ManageForumPermissionSetNode().TypeName, new ManageForumPermissionSet());

            #endregion

            #region 注册UploadAction

            FileManager.RegisterUploadAction(new UploadAttachmentAction());
			FileManager.RegisterUploadAction(new UploadAlbumPhotoAction());
            FileManager.RegisterUploadAction(new UploadDiskFileAction());

            #endregion

            #region 注册 IShareProvider

            ProviderManager.Add<IShareProvider>(new ShareBlogArticleProvider());
            ProviderManager.Add<IShareProvider>(new ShareAlbumProvider());
            ProviderManager.Add<IShareProvider>(new SharePhotoProvider());
            ProviderManager.Add<IShareProvider>(new ShareUserProvider());
            ProviderManager.Add<IShareProvider>(new ShareThreadProvider());

            #endregion

#endif

            #region 取数据库时间和时区作为当前时间和时区，避免web和数据库时间不同步

            SetTimeAsDatabase();

            #endregion

            #region 注册计划任务

            JobManager.RegisterJob(new BeforeRequestInDay0AM());
            JobManager.RegisterJob(new BeforeRequestIn5M());
			


            JobManager.RegisterJob(new AfterRequestIn5M());
            JobManager.RegisterJob(new AfterRequestIn3H());
            JobManager.RegisterJob(new AfterRequestInDay3AM());

            JobManager.RegisterJob(new ClearNotifyJob());
            JobManager.RegisterJob(new ClearChatMessageJob());
            JobManager.RegisterJob(new ClearPointLogJob());

#if !Passport
            JobManager.RegisterJob(new UpdateThreadViewsJob());
            JobManager.RegisterJob(new SaveOnlineUserJob());

            JobManager.RegisterJob(new DeleteFeedJob());
            JobManager.RegisterJob(new DeletePropLogJob());
            JobManager.RegisterJob(new DeleteOperationLogJob());

            JobManager.RegisterJob(new BaiduPageOpJop());

#endif
            //在所有计划任务注册完之后  启动线程
            JobManager.StartJobThread();
            
            #endregion

            #region 初始化PASSPORT指令引擎

            if (AllSettings.Current.PassportServerSettings.EnablePassportService) 
            {
                PassportBO.StartInstructEngine();
            }

            #endregion
        }

        /// <summary>
        /// 设置DateTimeUtility的Now的时间值与数据库时间同步
        /// </summary>
        private static void SetTimeAsDatabase()
        {
            double timeIntervalFromDatabase = DatabaseInfoDao.Instance.GetTimeIntervalFromDatabase();

            DateTimeUtil.TimeIntervalFromDatabase = timeIntervalFromDatabase;

            //UTC标准时间与数据库同步
            if (timeIntervalFromDatabase == 0.0f)
            {
                DateTimeUtil.DatabaseTimeDifference = (float)(DateTime.Now - DateTime.UtcNow).TotalHours;
            }
            else
            {
                DateTimeUtil.DatabaseTimeDifference = (float)DatabaseInfoDao.Instance.GetDatabaseTimeDifference() / 60.0f;
            }
        }
	}
}