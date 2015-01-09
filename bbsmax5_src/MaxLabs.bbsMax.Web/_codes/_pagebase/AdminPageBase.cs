//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.StepByStepTasks;
using MaxLabs.bbsMax.Entities;
using System.Collections.Specialized;
using System.Collections.Generic;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web
{
//#if DEBUG
    public class AdminPageBase : BbsPageBase
//#else
//    public abstract class AdminPageBase : BbsPageBase
//#endif
	{

        /// <summary>
        /// 后台的权限动作
        /// </summary>
//#if DEBUG
        protected virtual BackendPermissions.Action BackedPermissionAction { get { return BackendPermissions.Action.None; } }

        protected virtual BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget { get { return BackendPermissions.ActionWithTarget.None; } }
//#else
//        protected abstract BackendPermissions.Action BackedPermissionAction { get; }
//#endif

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (BackedPermissionAction != BackendPermissions.Action.None && AllSettings.Current.BackendPermissions.Can(My, BackedPermissionAction) == false)
            {
                ShowError("您没有进入本管理页面的权限");
                return;
            }

            if (BackedPermissionActionWithTarget != BackendPermissions.ActionWithTarget.None && AllSettings.Current.BackendPermissions.HasPermissionForSomeone(My, BackedPermissionActionWithTarget) == false)
            {
                ShowError("您没有进入本管理页面的权限");
                return;
            }
            HasBBS = true;
            string host = Request.Url.Host.ToLower();



#if Passport
 HasBBS = false;
#endif
        }



                /// <summary>
        /// 本系统是否有包括论坛部分。否则只有独立的passport服务器程序
        /// </summary>
        protected bool HasBBS
        {
            get;
            set;
        }

        protected override bool IncludeBase64Js
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 需要登陆
        /// </summary>
        protected override bool NeedLogin
        {
            get { return true; }
        }

        /// <summary>
        /// 需要管理员登陆
        /// </summary>
        protected override bool NeedAdminLogin
        {
            get { return true; }
        }

        /// <summary>
        /// 不检查必填项
        /// </summary>
        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        /// <summary>
        /// 后台页面不检查访问控制
        /// </summary>
        protected override bool NeedCheckAccess
        {
            get { return false; }
        }

        protected override bool NeedCheckVisit
        {
            get { return false; }
        }

        /// <summary>
        /// 不检查论坛是否关闭
        /// </summary>
        protected override bool NeedCheckForumClosed
        {
            get { return false; }
        }

        protected override string InfoPageSrc
        {

            get { return "~/max-admin/info.aspx";}
        }

        protected bool IsDefaultPage
        {
            get
            {
                return Request.Url.ToString().IndexOf("default.aspx", StringComparison.OrdinalIgnoreCase) > -1;
            }
        }

        protected void NoPermission()
        {
            //TODO;
            Response.Write("没有权限");
            Response.End();
        }

        protected MessageDisplay MsgDisplayForSaveSettings;

        protected bool SaveSetting<T>(string buttonName) where T : SettingBase, new()
        {
            if (_Request.IsClick(buttonName))
            {

                MessageDisplay msgDisplay = CreateMessageDisplay(GetSettingNames(typeof(T)));
                MsgDisplayForSaveSettings = msgDisplay;

                using (ErrorScope es = new ErrorScope())
                {
                    try
                    {
                        T settings = new T();

                        bool returnResult = false;
                        foreach (PropertyInfo property in typeof(T).GetProperties())
                        {
                            if (property.IsDefined(typeof(SettingItemAttribute), true))
                            {

                                if (SetSettingItemValue(settings, property) == false)
                                {
                                    es.CatchError<LostFormItemError>(delegate(LostFormItemError error)
                                    {
                                        msgDisplay.AddError(error);
                                        returnResult = true;
                                    });

                                    if (returnResult)
                                        return false;

                                    bool errorCatched = false;

                                    es.CatchError<TryParseFailedError>(delegate(TryParseFailedError error)
                                    {
                                        errorCatched = true;
                                        msgDisplay.AddError(property.Name, "输入的值格式不对");
                                    });

                                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                                    {
                                        errorCatched = true;
                                        msgDisplay.AddError(property.Name, error.Message);
                                    });

                                    if (errorCatched == false)
                                        msgDisplay.AddError(property.Name, "设置错误");
                                }
                            }
                        }

                        if (msgDisplay.HasAnyError())
                        {
                            return false;
                        }

                        if (!SettingManager.SaveSettings(settings))
                        {
                            es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error.Message);
                            });
                            return false;
                        }

                        //return true;

                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddException(ex);
                    }
                }

                //没有发生任何错误，操作成功，把表单Post值清除
                if (msgDisplay.HasAnyError() == false)
                    _Request.Clear(Method.Post);

                return true;
            }

            return false;
        }


        /// <summary>
        /// 收集表单数据并给指定设置项设置值
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        protected virtual bool SetSettingItemValue(SettingBase setting, PropertyInfo property)
        {

            string value = _Request.Get(property.Name, Method.All, null, false);

            if (value == null)
            {
                ThrowError(new LostFormItemError(property.Name));
                return false;
            }
            else
            {
                setting.SetPropertyValue(property, value, false);
                if (HasUnCatchedError)
                    return false;
            }

            return true;
        }


        private string[] GetSettingNames(Type settingType)
        {
            List<string> names = new List<string>();

            foreach (PropertyInfo property in settingType.GetProperties())
            {
                if (property.IsDefined(typeof(SettingItemAttribute), true))
                {
                    names.Add(property.Name);
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Exceptable<>))
                        names.Add("new_" + property.Name);
                }
            }

            return names.ToArray();
        }


        public AjaxSettings AjaxSettings { get { return AllSettings.Current.AjaxSettings; } }

        /// <summary>
        /// 系统中的所有用户组
        /// </summary>
        public RoleCollection AllRoleList { get { return AllSettings.Current.RoleSettings.Roles; } }

        /// <summary>
        /// 论坛设置
        /// </summary>
       //PageBase里已经有了 public SiteSettings SiteSettings { get { return AllSettings.Current.SiteSettings; } }

        /// <summary>
        /// Email系统设置
        /// </summary>
        public EmailSettings EmailSettings { get { return AllSettings.Current.EmailSettings; } }

        public ChatSettings ChatSettings { get { return AllSettings.Current.ChatSettings; } }

        /// <summary>
        /// 日期时间设置
        /// </summary>
        public DateTimeSettings DateTimeSettings { get { return AllSettings.Current.DateTimeSettings; } }

        public NotifySettings NotifySettings { get { return AllSettings.Current.NotifySettings; } }

        public UserSettings UserSettings { get { return AllSettings.Current.UserSettings; } }

        /// <summary>
        /// 头像设置
        /// </summary>
        public AvatarSettings AvatarSettings { get { return AllSettings.Current.AvatarSettings; } }

        /// <summary>
        /// 登录设置
        /// </summary>
        public LoginSettings LoginSettings { get { return AllSettings.Current.LoginSettings; } }

        /// <summary>
        /// 注册设置
        /// </summary>
        public RegisterSettings RegisterSettings { get { return AllSettings.Current.RegisterSettings; } }

        /// <summary>
        /// 找回密码设置
        /// </summary>
        public RecoverPasswordSettings RecoverPasswordSettings { get { return AllSettings.Current.RecoverPasswordSettings; } }

        /// <summary>
        /// 注册限制设置
        /// </summary>
        public RegisterLimitSettings RegisterLimitSettings { get { return AllSettings.Current.RegisterLimitSettings; } }

        /// <summary>
        /// 访问限制设置
        /// </summary>
        public AccessLimitSettings AccessLimitSettings { get { return AllSettings.Current.AccessLimitSettings; } }

        /// <summary>
        /// 邀请设置
        /// </summary>
        public InvitationSettings InvitationSettings { get { return AllSettings.Current.InvitationSettings; } }

        /// <summary>
        /// 打招呼动作设置
        /// </summary>
        public HailSettings HailSettings { get { return AllSettings.Current.HailSettings; } }

        /// <summary>
        /// 好友设置
        /// </summary>
        public FriendSettings FriendSettings { get { return AllSettings.Current.FriendSettings; } }

        /// <summary>
        /// 内容关键字过虑
        /// </summary>
        public ContentKeywordSettings ContentKeywordSettings { get { return AllSettings.Current.ContentKeywordSettings; } }

        /// <summary>
        /// 实名认证设置
        /// </summary>
        public NameCheckSettings NameCheckSettings { get { return AllSettings.Current.NameCheckSettings; } }

        /// <summary>
        /// 友好URL设置
        /// </summary>
        public FriendlyUrlSettings FriendlyUrlSettings { get { return AllSettings.Current.FriendlyUrlSettings; } }

        /// <summary>
        /// 用户扩展字段设置
        /// </summary>
        public ExtendedFieldSettings ExtendedFieldSettings { get { return AllSettings.Current.ExtendedFieldSettings; } }

        //public PointSettings PointSettings { get { return AllSettings.Current.ForumSettings; } }

        public LinkSettings LinkSettings { get { return AllSettings.Current.LinkSettings; } }

        /// <summary>
        /// 积分策略设置
        /// </summary>
        public PointActionSettings PointActionSettings { get { return AllSettings.Current.PointActionSettings; } }

        /// <summary>
        /// 积分类型设置
        /// </summary>
        public PointSettings PointSettings { get { return AllSettings.Current.PointSettings; } }

        /// <summary>
        /// 验证码设置
        /// </summary>
        public ValidateCodeSettings ValidateCodeSettings { get { return AllSettings.Current.ValidateCodeSettings; } }

        /// <summary>
        /// 用户组设置
        /// </summary>
        public RoleSettings RoleSettings { get { return AllSettings.Current.RoleSettings; } }

        /// <summary>
        /// 权限的设置
        /// </summary>
        public PermissionSettings PermissionSettings { get { return AllSettings.Current.PermissionSettings; } }

        /// <summary>
        /// 用户权限设置
        /// </summary>
        public UserPermissionSet UserPermissionSet { get { return AllSettings.Current.UserPermissionSet; } }

        /// <summary>
        /// 基本SEO设置
        /// </summary>
        public BaseSeoSettings BaseSeoSettings { get { return AllSettings.Current.BaseSeoSettings; } }

        public GoogleSeoSettings GoogleSeoSettings { get { return AllSettings.Current.GoogleSeoSettings; } }

        public DownloadSettings DownloadSettings { get { return AllSettings.Current.DownloadSettings; } }

        public ImpressionSettings ImpressionSettings { get { return AllSettings.Current.ImpressionSettings; } }


        public PhoneValidateSettings PhoneValidateSettings { get { return AllSettings.Current.PhoneValidateSettings; } }
       
        public PassportServerSettings PassportServerSettings { get { return AllSettings.Current.PassportServerSettings; } }

#if !Passport
        /// <summary>
        /// 搜索引擎屏蔽
        /// </summary>
        public ShieldSpiderSettings ShieldSpiderSettings { get { return AllSettings.Current.ShieldSpiderSettings; } }

        /// <summary>
        /// Possport设置
        /// </summary>
        public PassportClientConfig PassportClientSettings { get { return Globals.PassportClient; } }


        /// <summary>
        /// 表情设置
        /// </summary>
        public EmoticonSettings EmoticonSettings { get { return AllSettings.Current.EmoticonSettings; } }

        public PostIndexAliasSettings PostIndexAliasSettings { get { return AllSettings.Current.PostIndexAliasSettings; } }

        /// <summary>
        /// 网络硬盘设置
        /// </summary>
        public NetDiskSettings NetDiskSettings { get { return AllSettings.Current.NetDiskSettings; } }

        protected DiskSettings DiskSettings { get { return AllSettings.Current.DiskSettings; } }

        /// <summary>
        /// 论坛设置
        /// </summary>
        public ForumSettings ForumSettings { get { return AllSettings.Current.ForumSettings; } }

        /// <summary>
        /// 在线设置
        /// </summary>
        public OnlineSettings OnlineSettings { get { return AllSettings.Current.OnlineSettings; } }

        /// <summary>
        /// 空间设置
        /// </summary>
        public SpaceSettings SpaceSettings { get { return AllSettings.Current.SpaceSettings; } }

        /// <summary>
        /// 日志设置
        /// </summary>
        public BlogSettings BlogSettings { get { return AllSettings.Current.BlogSettings; } }

        /// <summary>
        /// 相册设置
        /// </summary>
        public AlbumSettings AlbumSettings { get { return AllSettings.Current.AlbumSettings; } }

        /// <summary>
        /// 分享的设置
        /// </summary>
        public ShareSettings ShareSettings { get { return AllSettings.Current.ShareSettings; } }

        /// <summary>
        /// 收藏的设置
        /// </summary>
        public FavoriteSettings FavoriteSettings { get { return AllSettings.Current.FavoriteSettings; } }

        /// <summary>
        /// 记录的设置
        /// </summary>
        public DoingSettings DoingSettings { get { return AllSettings.Current.DoingSettings; } }

        /// <summary>
        /// 默认个人隐私设置
        /// </summary>
        public PrivacySettings PrivacySettings { get { return AllSettings.Current.PrivacySettings; } }

        /// <summary>
        /// 竞价积分设置
        /// </summary>
        public PointShowSettings PointShowSettings { get { return AllSettings.Current.PointShowSettings; } }

        /// <summary>
        /// 主题鉴定图标
        /// </summary>
        public JudgementSettings JudgementSettings { get { return AllSettings.Current.JudgementSettings; } }

        /// <summary>
        /// 帖子图标
        /// </summary>
        public PostIconSettings PostIconSettings { get { return AllSettings.Current.PostIconSettings; } }

        public BaiduPageOpJopSettings BaiduPageOpJopSettings { get { return AllSettings.Current.BaiduPageOpJopSettings; } }

        public PropSettings PropSettings { get{ return AllSettings.Current.PropSettings; } }


        /// <summary>
        /// 支付设置
        /// </summary>
        public PaySettings PaySettings { get { return AllSettings.Current.PaySettings; } }

#endif

        [TemplateTag]
        public void AdminPager(string skin, int Count, string queryKey, int length, int pageSize)
        {
            Include(this.HtmlTextWriter
                , "src", skin
                , "rowCount", Count
                , "pageSize", pageSize
                , "querykey", queryKey
                , "length", length
                );
        }

        [TemplateTag]
        public void AdminPager(string skin, int Count, int length, int pageSize)
        {
            AdminPager(skin, Count, "page", length, pageSize);
        }

        [TemplateTag]
        public void AdminPager(string skin, int Count, int pageSize)
        {
            AdminPager(skin, Count, "page", 10, pageSize);
        }

        [TemplateTag]
        public void AdminPager(int Count, string queryKey, int length, int pageSize)
        {
            AdminPager("~/max-admin/pager_skins/default.ascx", Count, queryKey, length, pageSize);
        }

        [TemplateTag]
        public void AdminPager(int Count, int length, int pageSize)
        {
            AdminPager(Count, "page", length, pageSize);
        }

        [TemplateTag]
        public void AdminPager(int Count, int pageSize)
        {
            AdminPager(Count, "page", 10, pageSize);
        }


        private RunningTaskCollection m_RunningTaskList = null;
        [TemplateVariable]
        protected RunningTaskCollection RunningTaskList
        {
            get
            {
                if (m_RunningTaskList == null)
                    m_RunningTaskList = TaskManager.GetRunningTasks(MyUserID);

                return m_RunningTaskList;
            }
        }

        protected void JumpTo(string fileName)
        {
            Response.Redirect(UrlUtil.JoinUrl(Globals.AppRoot, "max-admin", fileName));
        }

    }
}