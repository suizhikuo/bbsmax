//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Configuration;
using System.Text;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Common;
using MaxLabs.WebEngine.Template;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.Web
{
    public class BbsPageBase : WebEngine.PageBase
    {

        protected const string AdminSessionKey = "adminsession";
        private const string DefaultReturnText = "如果您的浏览器没有自动跳转，请点击此处";
        private const string NavigationSeparator = "<span class=\"separator\">&raquo;</span> ";

        #region OnInit 和 OnLoadComplete 事件

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (System.Web.HttpContext.Current.Items["InitChecked"] == null)
            {




                CheckVisit();
                CheckLogin();
                CheckAdminLogin();
                CheckAccess();
                CheckRequiredUserInfo();
                
                
#if !Passport
                ShieldSpider();
                CheckForumClosed();
                //处理隐身请求
                ProcessInvisible();
#endif

                if (SiteSettings.DisplaySiteNameInNavigation)
                    AddRootNavigationItem(SiteSettings.SiteName, SiteSettings.SiteUrl,0);

                 AddRootNavigationItem (BbsName, IndexUrl,1);

                System.Web.HttpContext.Current.Items["InitChecked"] = true;
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            if (System.Web.HttpContext.Current.Items["LoadCompleted"] == null)
            {

                //if (My.TotalUnreadNotifies > 0)
                //{
                //    m_MyTopNotifyList = NotifyBO.Instance.GetTopNotifys(MyUserID, 8, NotifyType.All);
                //    WaitForFillSimpleUsers<NotifyBase>(m_MyTopNotifyList, 0);
                //}

                //if (My.UnreadMessages > 0)
                //{
                //    m_MyTopChatSessionList = ChatBO.Instance.GetChatSessionsWithUnreadMessages(MyUserID, 8);
                //    WaitForFillSimpleUsers(m_MyTopChatSessionList,0);
                //}

                System.Web.HttpContext.Current.Items["LoadCompleted"] = true;
            }
            base.OnLoadComplete(e);
        }

        #endregion

        #region 环境变量

        private string m_MainDomain = null;
        private string m_CookieDomain = null;

        /// <summary>
        /// 当前访问的url的主域
        /// </summary>
        public string MainDomain
        {
            get
            {
                if (m_MainDomain == null)
                {
                    string domain = HttpContext.Current.Request.Url.DnsSafeHost;

                    UrlUtil.BuildMainDomain(domain, true, out m_MainDomain, out m_CookieDomain);

                    if (Request.Url.Port != 80)
                        m_MainDomain += ":" + Request.Url.Port;
                }
                return m_MainDomain;
            }
        }

        /// <summary>
        /// 当前访问的Cookie域（始终使用根节点的）
        /// </summary>
        public string CookieDomain
        {
            get
            {
                if (m_CookieDomain == null)
                {
                    string domain = HttpContext.Current.Request.Url.DnsSafeHost;

                    UrlUtil.BuildMainDomain(domain, true, out m_MainDomain, out m_CookieDomain);
                }
                return m_CookieDomain;
            }
        }

        #endregion

        #region 属性 NeedCheckAccess / NeedLogin / NeedCheckRequiredUserInfo / NeedAdminLogin

        /// <summary>
        /// 当前页面是否需要检查网站被关闭的情况
        /// </summary>
        protected virtual bool NeedCheckForumClosed
        {
            get { return true; }
        }

        /// <summary>
        /// 当前页面是否需要检查浏览本站的权限
        /// </summary>
        protected virtual bool NeedCheckVisit
        {
            get { return true; }
        }

        /// <summary>
        /// 当前页面是否需要检查访问权限（例如IP被屏蔽）
        /// </summary>
        protected virtual bool NeedCheckAccess
        {
            get { return true; }
        }

        /// <summary>
        /// 当前页面是否需要登陆才能查看
        /// </summary>
        protected virtual bool NeedLogin
        {
            get { return false; }
        }

        /// <summary>
        /// 当前页面是否需要检查必填字段
        /// </summary>
        protected virtual bool NeedCheckRequiredUserInfo
        {
            get { return true; }
        }

        /// <summary>
        /// 当前页面需要检查是否登录了后台
        /// </summary>
        protected virtual bool NeedAdminLogin
        {
            get { return false; }
        }

        /// <summary>
        /// 是否使用对话框登录
        /// </summary>
        protected bool UseDialogLogin
        {
            get
            {
                return AllSettings.Current.LoginSettings.UseDialog;
            }
        }

        #endregion

        #region 方法 CheckAccess / CheckLogin / CheckRequiredUserInfo / CheckAdminLogin



        protected virtual void CheckForumClosed()
        {
#if !Passport
            if (NeedCheckForumClosed)
            {
                if (SiteSettings.ForumClosed == ForumState.Closed ||
                    (SiteSettings.ForumClosed == ForumState.TimingClosed && SiteSettings.ScopeList.CompareDateTime(DateTimeUtil.Now)))
                {
                    JumpLinkCollection jumpLinks = new JumpLinkCollection();
                    jumpLinks.Add(SiteSettings.ForumCloseReason, "");
                    ShowError("网站暂停访问", jumpLinks, false, 0);
                }
            }
#endif
        }

        private void CheckAccess()
        {
            if (NeedCheckAccess)
            {
                if (AllSettings.Current.AccessLimitSettings.AccessIPLimitList.IsMatch(_Request.IpAddress))
                {
                    if (IsLogin == false ||
                        (IsLogin && My.IsOwner == false))
                        ShowError("您的IP：" + _Request.IpAddress + " 已被禁止访问");
                }
            }
        }

        private void CheckVisit()
        {
            if (NeedCheckVisit)
            {
                if (AllSettings.Current.UserPermissionSet.Can(My, UserPermissionSet.Action.Visit) == false)
                {
                    if (IsLogin == false)
                    {
                        JumpLinkCollection jumpLinks = new JumpLinkCollection();
                        jumpLinks.Add("游客不能浏览本站，请您登录。", "");
                        ShowError("请您登录", jumpLinks, false, 0);
                    }
                    else
                    {
                        JumpLinkCollection jumpLinks = new JumpLinkCollection();
                        jumpLinks.Add("您所在的用户组不能浏览本站。", "");
                        ShowError("权限不够", jumpLinks, false, 0);
                    }
                }
            }
        }

        private void CheckLogin()
        {
            if (NeedLogin && IsLogin == false)
            {
                OnCheckLoginFailed();
            }
        }

        protected virtual void OnCheckLoginFailed()
        {
            ShowError(new NotLoginError());
        }

        /// <summary>
        /// 检查必填的用户信息是否填写
        /// </summary>
        private void CheckRequiredUserInfo()
        {
            if (NeedCheckRequiredUserInfo && IsLogin)
            {
                if (AllSettings.Current.ExtendedFieldSettings.UserNeedCompleteInfo(My))
                {
                    OnCheckRequiredUserInfoFailed();
                }
            }
        }

        protected virtual void OnCheckRequiredUserInfoFailed()
        {
            BbsRouter.JumpTo("my/setting");
        }

        private void CheckAdminLogin()
        {
            if (NeedAdminLogin)
            {
                bool result = false;

                //有资格登陆后台
                if (My.CanLoginConsole)
                {
                    Guid sessionID;
                    string ip = _Request.IpAddress;
                    if (Session[AdminSessionKey] != null)
                    {
                        AdminSessionStruct AdminSession = (AdminSessionStruct)Session[AdminSessionKey];
                        if (((TimeSpan)(DateTimeUtil.Now - AdminSession.LastUpdate)).TotalMinutes > 1.0d)
                        {

                            if (UserBO.Instance.HasAdminSession(MyUserID, out sessionID,ip))
                            {
                                if (AdminSession.AdminSessionID == sessionID)
                                {
                                    AdminSession.LastUpdate = DateTimeUtil.Now;
                                    Session[AdminSessionKey] = AdminSession;
                                    result = true;
                                }
                            }
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        if (UserBO.Instance.HasAdminSession(MyUserID, out sessionID,ip))
                        {
                            AdminSessionStruct session = new AdminSessionStruct(sessionID, DateTimeUtil.Now, My.Password);
                            Session[AdminSessionKey] = session;
                            result = true;
                        }
                    }
                }

                if (result == false)
                    OnCheckAdminLoginFailed();
            }
        }

        protected virtual void OnCheckAdminLoginFailed()
        {
            Display("~/max-admin/login.aspx");
        }

        #endregion

        #region 所有模版中都可用的常用变量和常用函数

        protected string GetBasePageTitle()
        {
if (string.IsNullOrEmpty(PageTitleAttach) == false)
                return string.Concat(BbsName, " - ", PageTitleAttach, " - powered by bbsmax");

            else
                return BbsName + " - powered by bbsmax";
        }

        /// <summary>
        /// 浏览器标题栏
        /// </summary>
        protected virtual string PageTitle
        {
            get
            {
                return GetBasePageTitle();
            }
        }

        /// <summary>
        /// 浏览器标题栏附加的文字
        /// </summary>
        protected virtual string PageTitleAttach
        {
            get { return AllSettings.Current.BaseSeoSettings.TitleAttach; }
        }

        /// <summary>
        /// meta中的关键字
        /// </summary>
        protected virtual string MetaKeywords
        {
            get { return AllSettings.Current.BaseSeoSettings.MetaKeywords; }
        }

        /// <summary>
        /// meta中的简介
        /// </summary>
        protected virtual string MetaDescription
        {
            get { return AllSettings.Current.BaseSeoSettings.MetaDescription; }
        }

        /// <summary>
        /// 是否需要输出base64js 通常在需要加密链接的URL的页面 需要输出
        /// </summary>
        protected virtual bool IncludeBase64Js
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 页面头部的html
        /// </summary>
        protected virtual string HeadHtml
        {
            get
            {
                string encodePostUrlJs;

                if (IncludeBase64Js && AllSettings.Current.BaseSeoSettings.EncodePostUrl)
                    encodePostUrlJs = string.Concat(@"
                <script type=""text/javascript"" src=""", Root, @"/max-assets/javascript/base64.js", _FastAspx, @"""></script>
");
                else
                    encodePostUrlJs = null;

                return string.Concat(@"
<meta name=""Author"" content=""maxlabs"" />
<meta name=""Copyright"" content=""bbsmax.com"" />
<script type=""text/javascript"">
var root = '", Root, "';debugMode=", IsDebug?"true;":"false;", /*(Globals.PassportClient.EnablePassport
                                  || AllSettings.Current.PassportServerSettings.EnablePassportService
                                  ? "document.domain='" + MainDomain+ "';"
                                  : "") ,*/
                                           @"
//var cookieid = '';
var staticExt = '", _FastAspx, @"';
var cookieDomain = '", CookieUtil.CookieDomain, @"';
var isExecuteJobTime = ", (IsExecuteJobTime ? "true" : "false"), @";
var jobUrl = '", JobUrl, @"';
</script>
",
 encodePostUrlJs == null ? "" : encodePostUrlJs
 , AllSettings.Current.BaseSeoSettings.OtherHeadMessage);

            }
        }


        //======================================================================================

        /// <summary>
        /// 当前bbsmax程序的版本
        /// </summary>
        protected string Version
        {
            get { return Globals.Version; }
        }

        private AuthUser m_My;

        /// <summary>
        /// 得到当前登陆用户的对象
        /// </summary>
        protected AuthUser My
        {
            get
            {
                if (m_My == null)
                    m_My = MaxLabs.bbsMax.Entities.User.Current;

                return m_My;
            }
        }

        private int? m_MyUserID;

        /// <summary>
        /// 得到我的UserID
        /// </summary>
        protected int MyUserID
        {
            get
            {
                if (m_MyUserID.HasValue == false)
                    m_MyUserID = UserBO.Instance.GetCurrentUserID();

                return m_MyUserID.Value;
            }
        }

        /// <summary>
        /// 判断当前用户是否登录
        /// </summary>
        protected bool IsLogin
        {
            get { return MyUserID > 0; }
        }


        private string m_IndexUrl = null;

        /// <summary>
        /// 网站首页的地址
        /// </summary>
        protected virtual string IndexUrl
        {
            get
            {
                if (m_IndexUrl == null)
                    m_IndexUrl = BbsRouter.GetIndexUrl();

                return m_IndexUrl;

            }
        }

        /// <summary>
        /// 完整的应用程序根目录
        /// </summary>
        protected string FullAppRoot
        {
            get { return Globals.FullAppRoot; }
        }

        /// <summary>
        /// 论坛名称
        /// </summary>
        protected string BbsName
        {
            get
            {

                    return SiteSettings.BbsName;
            }
        }

        /// <summary>
        /// 是否显示侧边栏
        /// </summary>
        protected virtual bool HasSideBar
        {
            get { return false; }
        }

        /// <summary>
        /// HTTP请求限制字节数
        /// </summary>
        protected int MaxRequestLength
        {
            get { return 4096; }
        }

        /// <summary>
        /// 页面的执行时间
        /// </summary>
        protected string ProcessTime
        {
            get
            {
                object value = HttpContext.Current.Items["MaxLabs.bbsMax.ProcessTimer"];

                if (value != null)
                    return ((Stopwatch)value).Elapsed.TotalSeconds.ToString();

                return "0";
            }
        }

        /// <summary>
        /// 页面的数据库查询次数
        /// </summary>
        protected int QueryTimes
        {
            get
            {
                object queryTimesObject = HttpContext.Current.Items["MaxLabs.bbsMax.DataAccess.QueryTimes"];

                if (queryTimesObject == null)
                    return 0;

                return (int)queryTimesObject;
            }
        }

        /// <summary>
        /// 远程接口调用次数
        /// </summary>
        protected int RemoteCallCount
        {
            get
            {
                return MaxLabs.bbsMax.PassportServerInterface.Service.ServiceCallCount;
            }
        }


        /// <summary>
        /// 获取有关客户端上次请求的 URL 的信息，该请求链接到当前的 URL。
        /// </summary>
        protected string UrlReferrer
        {
            get
            {
                if (HttpContext.Current.Request.UrlReferrer == null)
                    return string.Empty;

                return HttpContext.Current.Request.UrlReferrer.ToString();
            }
        }

        /// <summary>
        /// 第三方统计系统的代码
        /// </summary>
        protected string StatCode
        {
            get { return SiteSettings.StatCode; }
        }

        /// <summary>
        /// ICP证
        /// </summary>
        protected string ForumIcp
        {
            get { return SiteSettings.ForumIcp; }
        }

        /// <summary>
        /// 总积分的名称
        /// </summary>
        protected string GeneralPointName
        {
            get { return AllSettings.Current.PointSettings.GeneralPointName; }
        }

        /// <summary>
        /// 标识用户身份的cookie值
        /// </summary>
        protected string UserAuthCookie
        {
            get { return HttpUtility.UrlEncode(UserBO.Instance.GetAuthCookie()); }
        }

        #endregion

        #region 输出广告

        protected AdvertSettings AdSettings { get { return AllSettings.Current.AdvertSettings; } }

        private AdvertDeferItemCollection m_AdDeferList;

        private int ADIndex = 0;

        /// <summary>
        /// 广告后输出缓存
        /// </summary>
        protected AdvertDeferItemCollection AdDeferList
        {
            get
            {
                if (m_AdDeferList == null)
                    m_AdDeferList = new AdvertDeferItemCollection();
                return m_AdDeferList;
            }
        }

        /// <summary>
        /// 返回一个空的DIV，后期替换该ID
        /// </summary>
        /// <param name="controlid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected string AddAdDeferItem(string categotyID, string code)
        {
            string key = string.Format("bugbear_{0}_{1}", categotyID.Replace('-', '_'), ADIndex++);
            AdDeferList.Add(new AdvertDeferItem(key, code));
            return string.Format("<div style=\"display:none\" id=\"{0}\"></div>", key);
        }




        /// <summary>
        /// 需要子类重写才有值（只有在版块中才有值） 否则为0
        /// </summary>
        protected virtual int CurrentForumID
        {
            get
            {
                return 0;
            }
        }

        private MenuPermissions m_MenuPermission = null;
        protected virtual MenuPermissions MenuPermission
        {
            get
            {
                if (m_MenuPermission == null)
                {
                    m_MenuPermission = new MenuPermissions(My, 0);
                }
                return m_MenuPermission;
            }
        }


        protected string GetTarget(int forumId)
        {
            string target = "all";
            if (forumId == 0)
            {
                string Url = Request.Url.ToString();

                foreach (ADTarget t in ADTargetCommonPages.All)
                {
                    if (StringUtil.ContainsIgnoreCase(Url, t.Target))
                    {
                        target = t.Target;
                        break;
                    }
                }
            }
            else
            {
                target = ADTarget.ForumPrefix + forumId;
            }
            return target;
        }


        protected string Ad(int category)
        {
            string target = GetTarget(CurrentForumID);
            return AdvertBO.Instance.ShowAdvert(category, target, ADPosition.None);
        }

        protected bool HasHeaderAD
        {
            get
            {
                return AdvertBO.Instance.HasAdvert(ADCategory.HeaderAd.ID, GetTarget(CurrentForumID), ADPosition.None);
            }
        }

        protected bool HasInListAD
        {
            get
            {
                return AdvertBO.Instance.HasAdvert(ADCategory.InListAd.ID, GetTarget(CurrentForumID), ADPosition.None); 
            }
        }

        protected bool HasFooterAD
        {
            get
            {
                return AdvertBO.Instance.HasAdvert(ADCategory.FooterAd.ID, GetTarget(CurrentForumID), ADPosition.None);
            }
        }

        protected bool HasFloatAD
        {
            get
            {
                return AdvertBO.Instance.HasAdvert(ADCategory.FloatAd.ID, GetTarget(CurrentForumID), ADPosition.None);
            }
        }

        protected bool HasDoubleAD
        {
            get
            {
                return AdvertBO.Instance.HasAdvert(ADCategory.DoubleAd.ID, GetTarget(CurrentForumID), ADPosition.None);
            }
        }


        protected bool HasInPostTopAD(int floor, bool isLastFloor)
        {
            return AdvertBO.Instance.HasInPostAD(GetTarget(CurrentForumID), ADPosition.Top, floor, isLastFloor);
        }

        protected bool HasInPostBottomAD(int floor, bool isLastFloor)
        {
            return AdvertBO.Instance.HasInPostAD(GetTarget(CurrentForumID), ADPosition.Bottom, floor, isLastFloor);
        }

        protected bool HasInPostRightAD(int floor, bool isLastFloor)
        {
            return AdvertBO.Instance.HasInPostAD(GetTarget(CurrentForumID), ADPosition.Right, floor, isLastFloor);
        }

        protected bool HasInForumAD
        {
            get
            {
                return AdvertBO.Instance.HasAdvert(ADCategory.InForumAd.ID, GetTarget(CurrentForumID), ADPosition.None);
            }
        }

        protected bool HasPageWordAD
        {
            get
            {
                return AdvertBO.Instance.HasAdvert(ADCategory.PageWordAd.ID, GetTarget(CurrentForumID), ADPosition.None);
            }
        }

        protected bool HasPostLeaderboardAD
        {
            get
            {
                return AdvertBO.Instance.HasAdvert(ADCategory.PostLeaderboardAd.ID, GetTarget(CurrentForumID), ADPosition.None);
            }

        }

        protected bool HasSignatureAd
        {
            get
            {
                return AdvertBO.Instance.HasAdvert(ADCategory.SignatureAd.ID, GetTarget(CurrentForumID), ADPosition.None);
            }
        }

        /// <summary>
        /// 头部广告
        /// </summary>
        /// <param name="forumId">版块</param>
        /// <returns></returns>
        protected string HeaderAD
        {
            get
            {
                if (AdSettings.EnableDefer)
                    return AddAdDeferItem(ADCategory.HeaderAd.ID.ToString(), Ad(ADCategory.HeaderAd.ID));
                return Ad(ADCategory.HeaderAd.ID);
            }
        }

        /// <summary>
        /// 页面底部广告
        /// </summary>
        protected string FooterAD
        {
            get
            {
                if (AdSettings.EnableDefer)
                    return AddAdDeferItem(ADCategory.FooterAd.ID.ToString(), Ad(ADCategory.FooterAd.ID));
                return Ad(ADCategory.FooterAd.ID);
            }
        }

        /// <summary>
        /// 右下角浮动广告
        /// </summary>
        protected string FloatAD
        {
            get
            {
                return Ad(ADCategory.FloatAd.ID);
            }
        }

        /// <summary>
        /// 对联广告
        /// </summary>
        protected string DoubleAD
        {
            get
            {
                return Ad(ADCategory.DoubleAd.ID);
            }
        }

        /// <summary>
        /// 顶部横幅广告
        /// </summary>
        protected string TopBannerAD
        {
            get
            {
                return Ad(ADCategory.TopBanner.ID);
            }
        }

        /// <summary>
        /// 当前位置是否有顶部横幅广告
        /// </summary>
        protected bool HasTopBannerAD
        {
            get
            {
                return AdvertBO.Instance.HasAdvert(ADCategory.TopBanner.ID, GetTarget(CurrentForumID), ADPosition.None);

            }
        }


        /// <summary>
        /// 分类间广告
        /// </summary>
        protected string InForumAD
        {
            get
            {
                if (AdSettings.EnableDefer)
                    return AddAdDeferItem(ADCategory.InForumAd.ID.ToString(), Ad(ADCategory.InForumAd.ID));
                return Ad(ADCategory.InForumAd.ID);
            }
        }


        /// <summary>
        /// 列表页广告
        /// </summary>
        public string InListAD
        {
            get
            {
                if (AdSettings.EnableDefer)
                    return AddAdDeferItem(ADCategory.InListAd.ID.ToString(), Ad(ADCategory.InListAd.ID));
                return Ad(ADCategory.InListAd.ID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns></returns>
        protected string[] PageWordAD(int count)
        {
            return AdvertBO.Instance.GetAdList(ADCategory.PageWordAd.ID, GetTarget(CurrentForumID), count);
        }

        /// <summary>
        /// 帖间通栏广告
        /// 主题和第一回帖间
        /// </summary>
        /// <param name="forumId"></param>
        /// <returns></returns>
        protected string PostLeaderboardAD
        {
            get
            {
                if (AdSettings.EnableDefer)
                    return AddAdDeferItem(ADCategory.PostLeaderboardAd.ID.ToString(), Ad(ADCategory.PostLeaderboardAd.ID));
                return Ad(ADCategory.PostLeaderboardAd.ID);
            }
        }

        /// <summary>
        /// 签名广告
        /// </summary>
        protected string SignatureAd
        {
            get
            {
                if (AdSettings.EnableDefer)
                    return AddAdDeferItem(ADCategory.SignatureAd.ID.ToString(), Ad(ADCategory.SignatureAd.ID));
                return Ad(ADCategory.SignatureAd.ID);
            }
        }
        #endregion

        #region 显示错误和正确提示信息 ShowError / ShowSuccess

        protected virtual string InfoPageSrc
        {
            get { return "~/max-templates/default/_inc/info.aspx"; }
        }

        #region ShowError

        /// <summary>
        /// 显示错误提示页面
        /// </summary>
        /// <param name="error">错误类型</param>
        protected void ShowError(ErrorInfo error)
        {
            //如果是未登陆的错误，或者没权限的错误且当前未登陆，则要求用户登陆
            if (error is NotLoginError || (error is NoPermissionError && IsLogin == false))
                _ShowInfo("error", error.Message, error.HtmlEncodeMessage, false, null, 0, true,null);
            else
                _ShowInfo("error", error.Message, error.HtmlEncodeMessage, false, null, 0, false,null);
        }

        /// <summary>
        /// 显示错误提示页面
        /// </summary>
        protected void ShowError(string message, string returnUrl, bool tipLogin)
        {
            JumpLinkCollection returnUrls = new JumpLinkCollection();
            returnUrls.Add(DefaultReturnText, returnUrl);

            _ShowInfo("error", message, true, false, returnUrls, 0, tipLogin,null);
        }
        /// <summary>
        /// 显示错误提示页面
        /// </summary>
        /// <param name="error">错误类型</param>
        protected void ShowError(ErrorInfo error, string returnUrl)
        {
            JumpLinkCollection returnUrls = new JumpLinkCollection();
            returnUrls.Add(DefaultReturnText, returnUrl);

            //如果是未登陆的错误，或者没权限的错误且当前未登陆，则要求用户登陆
            if (error is NotLoginError || (error is NoPermissionError && IsLogin == false))
                _ShowInfo("error", error.Message, error.HtmlEncodeMessage, false, returnUrls, 0, true,null);
            else
                _ShowInfo("error", error.Message, error.HtmlEncodeMessage, false, returnUrls, 0, false,null);
        }

        /// <summary>
        /// 显示错误提示页面
        /// </summary>
        /// <param name="error">错误类型</param>
        protected void ShowError(ErrorInfo error, string returnUrl, int autoJumpSeconds)
        {
            JumpLinkCollection returnUrls = new JumpLinkCollection();
            returnUrls.Add(DefaultReturnText, returnUrl);

            //如果是未登陆的错误，或者没权限的错误且当前未登陆，则要求用户登陆
            if (error is NotLoginError || (error is NoPermissionError && IsLogin == false))
                _ShowInfo("error", error.Message, error.HtmlEncodeMessage, false, returnUrls, autoJumpSeconds, true,null);
            else
                _ShowInfo("error", error.Message, error.HtmlEncodeMessage, false, returnUrls, autoJumpSeconds, false,null);
        }

        protected void ShowError(ErrorInfo error, JumpLinkCollection returnUrls)
        {
            //如果是未登陆的错误，或者没权限的错误且当前未登陆，则要求用户登陆
            if (error is NotLoginError || (error is NoPermissionError && IsLogin == false))
                _ShowInfo("error", error.Message, error.HtmlEncodeMessage, false, returnUrls, 0, true,null);
            else
                _ShowInfo("error", error.Message, error.HtmlEncodeMessage, false, returnUrls, 0, false,null);
        }

        protected void ShowError(ErrorInfo error, JumpLinkCollection returnUrls, int autoJumpSeconds)
        {
            //如果是未登陆的错误，或者没权限的错误且当前未登陆，则要求用户登陆
            if (error is NotLoginError || (error is NoPermissionError && IsLogin == false))
                _ShowInfo("error", error.Message, error.HtmlEncodeMessage, false, returnUrls, autoJumpSeconds, true,null);
            else
                _ShowInfo("error", error.Message, error.HtmlEncodeMessage, false, returnUrls, autoJumpSeconds, false,null);
        }

        /// <summary>
        /// 显示错误提示页面
        /// </summary>
        /// <param name="message">错误消息</param>
        protected void ShowError(string message)
        {
            ShowError(message, true);
        }

        protected void ShowError(string message, bool htmlEncode)
        {
            _ShowInfo("error", message, htmlEncode, false, null, 0, false, null);
        }

        /// <summary>
        /// 显示错误提示页面
        /// </summary>
        /// <param name="message">错误消息</param>
        protected void ShowError(string message, JumpLinkCollection returnUrls, bool requiredLogin)
        {
            _ShowInfo("error", message, true, false, returnUrls, 0, requiredLogin,null);
        }

        /// <summary>
        /// 显示错误提示页面
        /// </summary>
        /// <param name="message">错误消息</param>
        protected void ShowError(string message, JumpLinkCollection returnUrls, bool requiredLogin, int autoJumpSeconds)
        {
            _ShowInfo("error", message, true, false, returnUrls, autoJumpSeconds, requiredLogin,null);
        }

        #endregion

        #region ShowSuccess

        /// <summary>
        /// 显示操作成功提示页面
        /// </summary>
        protected void ShowSuccess()
        {
            _ShowInfo("success", null, true, false, null, 2, false,null);
        }

        protected void ShowSuccess(string message)
        {
            ShowSuccess(true, message);
        }
        /// <summary>
        /// 显示操作成功提示页面
        /// </summary>
        protected void ShowSuccess(bool htmlEncodeMessage, string message)
        {
            _ShowInfo("success", message, htmlEncodeMessage, false, null, 2, false, null);
        }

        /// <summary>
        /// 显示操作成功提示页面
        /// </summary>
        protected void ShowSuccess(string message,object returnValue)
        {
            _ShowInfo("success", message, true, false, null, 2, false, returnValue);
        }

      
        protected void ShowSuccess(string message, string returnUrl)
        {
            JumpLinkCollection returnUrls = new JumpLinkCollection();
            returnUrls.Add(DefaultReturnText, returnUrl);
            _ShowInfo("success", message, true, false, returnUrls, 2, false, null);
        }

        /// <summary>
        /// 显示操作成功提示页面
        /// </summary>
        /// <param name="message"></param>
        protected void ShowSuccess(string message, JumpLinkCollection returnUrls)
        {
            _ShowInfo("success", message, true, false, returnUrls, 2, false,null);
        }

        #endregion

        protected void ShowWarning(string message)
        {
            ShowWarning(true, message);
        }
        protected void ShowWarning(bool htmlEncodeMessage, string message)
        {
            _ShowInfo("success", message, htmlEncodeMessage, true, null, 5, false, null);
        }

        protected void ShowWarning(string message, object returnValue)
        {
            _ShowInfo("success", message, true, true, null, 5, false, returnValue);
        }

        protected void ShowWarning(string message, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                ShowWarning(message);
                return;
            }
            JumpLinkCollection returnUrls = new JumpLinkCollection();
            returnUrls.Add(DefaultReturnText, returnUrl);
            ShowWarning(message, returnUrls);
        }

        protected void ShowWarning(string message, JumpLinkCollection returnUrls)
        {
            _ShowInfo("success", message, true, true, returnUrls, 3, false, null);
        }

        #region ShowInfo

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="message"></param>
        /// <param name="warning"></param>
        /// <param name="returnUrls"></param>
        /// <param name="autoJumpSeconds"></param>
        /// <param name="tipLogin"></param>
        /// <param name="returnValue">对话框的返回值</param>
        internal protected void _ShowInfo(string mode, string message, bool htmlEncodeMessage, bool warning, JumpLinkCollection returnUrls, int autoJumpSeconds, bool tipLogin,object returnValue)
        {


            if (message == null)
            {
                if (mode == "success")
                    message = "操作成功";
                else if (mode == "error")
                    message = "发生错误";
            }

            if (returnUrls == null)
                returnUrls = new JumpLinkCollection();

            if (returnUrls.Count == 0)
            {
                string defaultReturn = null;

                if (Request.UrlReferrer != null)
                {
                    if (StringUtil.StartsWithIgnoreCase(Request.UrlReferrer.OriginalString, Globals.FullAppRoot + "/"))
                        defaultReturn = Request.UrlReferrer.PathAndQuery;
                }

                if (defaultReturn == null)
                    defaultReturn = BbsRouter.GetIndexUrl();

                returnUrls.Add(DefaultReturnText, defaultReturn, true);
            }

            if (htmlEncodeMessage)
                message = StringUtil.HtmlEncode(message);

            message = scriptRegex.Replace(message, "<$1bbsmax");
            message = iframeRegex.Replace(message, "<$1bbsmax");

            NameObjectCollection parms = new NameObjectCollection();
            parms.Add("mode", mode);
            parms.Add("message", message);
            parms.Add("returnUrls", returnUrls);
            parms.Add("autoJumpSeconds", autoJumpSeconds);
            parms.Add("tipLogin", tipLogin);
            parms.Add("warning", warning);
            parms.Add("ReturnValue", returnValue);
            Display(InfoPageSrc, true, parms);
        }

        private ScriptRegex scriptRegex = new RegExp.ScriptRegex();
        private IframeRegex iframeRegex = new RegExp.IframeRegex();
        #endregion

        protected MessageDisplay CreateMessageDisplay(params string[] names)
        {
            return CreateMessageDisplayForForm(null, names);
        }

        protected MessageDisplay CreateMessageDisplayForForm(string form, params string[] names)
        {
            MessageDisplay d = new MessageDisplay(form, names);
            return d;
        }

        #endregion

        #region 验证码相关

        /// <summary>
        /// 检查验证码 是否正确
        /// </summary>
        protected bool CheckValidateCode(string actionType, MessageDisplay msgDisplay)
        {
            return CheckValidateCode(actionType, null, msgDisplay);
        }
        /// <summary>
        /// 检查验证码 是否正确
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="msgDisplay"></param>
        /// <param name="id">
        /// 如果同一个页面 出现两个及两个以上相同动作的验证码 
        /// 需要指定一个区别的标志（如： 输入框名字必须为 "{$inputName}id" id(a-zA-Z\d_)任意指定 不重复）
        /// 如果没有相同动作的验证码 则传null 
        /// </param>
        /// <returns></returns>
        protected bool CheckValidateCode(string actionType, string id, MessageDisplay msgDisplay)
        {
            return ValidateCodeManager.CheckValidateCode(actionType, id, msgDisplay);
        }

        protected string GetValidateCodeInputName(string validateCodeAction)
        {
            return GetValidateCodeInputName(validateCodeAction, string.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="validateCodeAction"></param>
        /// <param name="id">
        /// 如果同一个页面 出现两个及两个以上相同动作的验证码 
        /// 需要指定一个区别的标志（如： 输入框名字必须为 "{$inputName}id" id任意指定 不重复）
        /// 如果没有相同动作的验证码 则传null 
        /// </param>
        /// <returns></returns>
        protected string GetValidateCodeInputName(string validateCodeAction, string id)
        {
            return MaxLabs.bbsMax.ValidateCodes.ValidateCodeManager.GetValidateCodeInputName(validateCodeAction, id);
        }

        #endregion

        #region 输出指定长度字符串、IP、所在地、文件大小、日期和时间

        protected string CutString(string text, int length)
        {
            return StringUtil.CutString(text, length);
        }

        private int? m_OutputIpPartCount;
        protected int OutputIpPartCount
        {
            get
            {
                if (m_OutputIpPartCount == null)
                {
                    if (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Setting_AccessLimit))
                        m_OutputIpPartCount = int.MaxValue;
                    else
                        m_OutputIpPartCount = AllSettings.Current.SiteSettings.ViewIPFields.GetValue(My);
                }

                return m_OutputIpPartCount.Value;
            }
        }


        /// <summary>
        /// 根据权限输出指定段数的IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        protected string OutputIP(string ip)
        {
            return IPUtil.OutputIP(My, ip, OutputIpPartCount);
        }

        /// <summary>
        /// 输出IP的所在地
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        protected string OutputIPAddress(string ip)
        {
            return IPUtil.GetIpArea(ip);
        }

        /// <summary>
        /// 输出友好的文件大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        protected string OutputFileSize(long size)
        {
            return ConvertUtil.FormatSize(size);
        }

        /// <summary>
        /// 返回用户与服务器之间的时差（小时）
        /// </summary>
        private float UserTimeDifference
        {
            get
            {
                return UserBO.Instance.GetUserTimeDiffrence(My);
            }
        }

        protected string OutputTime(DateTime time)
        {
            if (time.Year > 1753 && time.Year < 9999)
            {
                DateTime newDateTime = time.AddHours(UserTimeDifference);
                return newDateTime.ToString(AllSettings.Current.DateTimeSettings.TimeFormat);
            }
            return "00:00:00";
        }


        protected string OutputDate(DateTime date)
        {
            if (date <= DateTimeUtil.SQLMinValue)
                return Lang.Common_Indefinitely;
            else if (date.Year == 9999)
                return Lang.Common_Indefinitely;
            DateTime newDateTime = date.AddHours(UserTimeDifference);
            return DateTimeUtil.FormatDate(newDateTime);
        }

        protected string OutputDateTime(DateTime dateTime)
        {
            return OutputDateTime(dateTime, Lang.Common_Indefinitely, Lang.Common_Indefinitely);
        }
        protected string OutputDateTime(DateTime dateTime, string outputMinString, string outputMaxString)
        {
            if (dateTime <= DateTimeUtil.SQLMinValue)
                return outputMinString;
            else if (dateTime.Year == 9999)
                return outputMaxString;

            DateTime newDateTime = dateTime.AddHours(UserTimeDifference);
            return DateTimeUtil.FormatDateTime(newDateTime);
            //return newDateTime.ToString(AllSettings.Current.DateTimeSettings.DateFormat + " " + AllSettings.Current.DateTimeSettings.TimeFormat);
        }

        protected string OutputFriendlyDateTime(DateTime dateTime)
        {
            return DateTimeUtil.GetFriendlyDateTime(My, dateTime);
        }

        protected string OutputFriendlyDate(DateTime dateTime)
        {
            return DateTimeUtil.GetFriendlyDate(My, dateTime);
        }

        protected string OutputTotalTime(int minute)
        {
            return DateTimeUtil.FormatMinute(minute);
        }

        protected string OutputTotalTime(int minute, TimeUnit unit)
        {
            return DateTimeUtil.FormatMinute(minute, unit);
        }

        protected string UserNow
        {
            get
            {
                return DateTimeUtil.FormatDateTime(DateTimeUtil.Now.AddHours(UserTimeDifference));
            }
        }

        #endregion

        #region Navigation - 导航条相关的方法

        private string m_Navigation = null;
        protected string Navigation
        {
            get
            {
                if (m_Navigation == null)
                {
                    int i = 0;
                    int total = m_NavigationItems.Count;
                    foreach (string item in m_NavigationItems)
                    {
                        string temp = item;
                        i++;
                        if (i == total)
                        {
                            temp = string.Concat("<span class=\"current\"><span>", temp, "</span></span>");
                        }
                        if (m_Navigation == null)
                            m_Navigation = temp;
                        else
                            m_Navigation = string.Concat(m_Navigation, NavigationSeparator, temp);
                    }

                }
                return m_Navigation;
            }
        }

        StringCollection m_NavigationItems = new StringCollection();

        /// <summary>
        /// 增加一个导航项
        /// </summary>
        /// <param name="text"></param>
        /// <param name="linkUrl"></param>
        /// <param name="isNewWindow"></param>
        protected void AddNavigationItem(string text, string linkUrl, bool isNewWindow)
        {
            if (string.IsNullOrEmpty(linkUrl))
            {
                m_NavigationItems.Add(text);
            }
            else
            {
                string navigationItem;
                if (isNewWindow)
                    navigationItem = string.Concat("<a href=\"", linkUrl, "\" targer=\"_blank\"><span>", text, "</span></a> ");
                else
                    navigationItem = string.Concat("<a href=\"", linkUrl, "\"><span>", text, "</span></a> ");

                m_NavigationItems.Add(navigationItem);
            }
        }

        private void AddRootNavigationItem(string name, string url,int index)
        {
            m_NavigationItems.Add(string.Concat(@"<a href=""", url,@""" id=""max_nav_root_" ,index, @"""><span>", name, "</span></a> "));
        }

        protected void AddNavigationItem(string text, string linkUrl)
        {
            AddNavigationItem(text, linkUrl, false);
        }

        protected void AddNavigationItem(string text)
        {
            AddNavigationItem(text, string.Empty, false);
        }

        protected void InsertNavigationItem(int index, string text, string linkUrl, bool isNewWindow)
        {
            if (string.IsNullOrEmpty(linkUrl))
            {
                //navigationBuilder.Append(text);
                m_NavigationItems.Insert(index, text);
            }
            else
            {
                string navigationItem;
                if (isNewWindow)
                    navigationItem = string.Concat("<a href=\"", linkUrl, "\" targer=\"_blank\"><span>", text, "<span></a> ");
                else
                    navigationItem = string.Concat("<a href=\"", linkUrl, "\"><span>", text, "</span></a> ");

                m_NavigationItems.Insert(index, navigationItem);
            }
        }

        protected void InsertNavigationItem(int index, string text, string linkUrl)
        {
            InsertNavigationItem(index, text, linkUrl, false);
        }

        protected void InsertNavigationItem(int index, string text)
        {
            InsertNavigationItem(index, text, string.Empty, false);
        }


        protected void ClearNavigationItem()
        {
            m_NavigationItems.Clear();
        }


        #endregion

        /// <summary>
        /// （用小写）用来标记当前页是属于导航菜单中的哪一项
        /// </summary>
        protected virtual string NavigationKey
        {
            get { return null; }
        }

        protected bool EnableChatFunction
        {
            get { return AllSettings.Current.ChatSettings.EnableChatFunction; }
        }

#if Passport
        protected string SetOnlineUrl
        {
            get { return string.Empty; }
        }

        protected bool CanManageDenouncing { get { return false; } }

        protected int DenouncingCount { get { return 0; } }

        protected int DenouncingPhotoCount { get { return 0; } }

        protected int DenouncingArticleCount { get { return 0; } }

        protected int DenouncingShareCount { get { return 0; } }

        protected int DenouncingUserCount;

        protected int DenouncingTopicCount;

        protected int DenouncingReplyCount;

#endif

#if !Passport
        protected bool IsShowSideBar
        {
            get { return EnableDisplaySideBar == EnableStatus.Enabled; }
        }


        protected void ShieldSpider()
        {

            if (
                AllSettings.Current.ShieldSpiderSettings.EnableShield
                &&
                _Request.IsSpider
                &&
                AllSettings.Current.ShieldSpiderSettings.BannedSpiders.IsBanned(_Request.SpiderType)
                )
            {
                ShowError("该网站已禁止特定类型的蜘蛛访问");
            }
        }


        protected EnableStatus EnableDisplaySideBar
        {
            get
            {
                if (MyUserID == 0)
                    return (AllSettings.Current.BbsSettings.DisplaySideBar ? EnableStatus.Enabled : EnableStatus.Disabled);

                if (My.EnableDisplaySidebar == EnableStatus.NotSet)
                {
                    return (AllSettings.Current.BbsSettings.DisplaySideBar ? EnableStatus.Enabled : EnableStatus.Disabled);
                }

                if (My.EnableDisplaySidebar != EnableStatus.NotSet && My.EnableDisplaySidebar != EnableStatus.Enabled && My.EnableDisplaySidebar != EnableStatus.Disabled)
                    return EnableStatus.NotSet;

                return My.EnableDisplaySidebar;
            }
        }

        #region 用于判断功能是否启用的变量

        protected bool EnableAlbumFunction
        {
            get { return AllSettings.Current.AlbumSettings.EnableAlbumFunction; }
        }

        protected bool EnableBlogFunction
        {
            get { return AllSettings.Current.BlogSettings.EnableBlogFunction; }
        }

        protected bool EnableShareFunction
        {
            get { return AllSettings.Current.ShareSettings.EnableShareFunction; }
        }

        protected bool EnableFavoriteFunction
        {
            get { return AllSettings.Current.FavoriteSettings.EnableFavoriteFunction; }
        }


        protected bool EnableDoingFunction
        {
            get { return AllSettings.Current.DoingSettings.EnableDoingFunction; }
        }

        protected bool EnableNetDiskFunction
        {
            get { return DiskBO.Instance.IsEnableDisk; }
        }

        //protected bool EnableUserEmoticon
        //{

        //    get { return EmoticonBO.Instance.IsEnableEmotion; }
        //}

        protected bool EnableMissionFunction
        {
            get { return AllSettings.Current.MissionSettings.EnableMissionFunction; }
        }

        protected bool EnableImpressionFunction
        {
            get { return AllSettings.Current.ImpressionSettings.EnableImpressionFunction; }
        }

        protected bool EnablePropFunction
        {
            get { return AllSettings.Current.PropSettings.EnablePropFunction; }
        }

        protected bool EnableEmoticonFunction
        {
            get { return AllSettings.Current.EmoticonSettings.EnableUserEmoticons; }
        }

        protected bool EnablePassportClient
        {
            get
            {
                return Globals.PassportClient.EnablePassport;
            }
        }

        protected bool EnablePassportServer
        {
            get;
            set;
        }

        protected bool EnablePointExchange
        {
            get { return AllSettings.Current.PointSettings.EnablePointExchange; }
        }

        protected bool EnablePointTransfer
        {
            get { return AllSettings.Current.PointSettings.EnablePointTransfer; }
        }

        protected bool EnablePointRecharge
        {
            get { return AllSettings.Current.PaySettings.EnablePointRecharge; }
        }

        #endregion

        #region 空间等权限判断

        protected SpacePermissionSet SpacePermission
        {
            get
            {
                return AllSettings.Current.SpacePermissionSet;
            }
        }

        private bool? m_CanUseShare;
        protected bool CanUseShare
        {
            get
            {
                if (m_CanUseShare == null)
                {
                    m_CanUseShare = SpacePermission.Can(My, SpacePermissionSet.Action.UseShare);
                }
                return m_CanUseShare.Value;
            }
        }

        private bool? m_CanUseCollection;
        protected bool CanUseCollection
        {
            get
            {
                if (m_CanUseCollection == null)
                {
                    m_CanUseCollection = SpacePermission.Can(My, SpacePermissionSet.Action.UseCollection);
                }
                return m_CanUseCollection.Value;
            }
        }

        protected bool IsShowShareLink
        {
            get
            {
                return CanUseShare || CanUseCollection;
            }
        }

        private bool? m_CanUseBlog;
        protected bool CanUseBlog
        {
            get
            {
                if (m_CanUseBlog == null)
                {
                    m_CanUseBlog = SpacePermission.Can(My, SpacePermissionSet.Action.UseBlog);
                }
                return m_CanUseBlog.Value;
            }
        }

        private bool? m_CanAddComment;
        protected bool CanAddComment
        {
            get
            {
                if (m_CanAddComment == null)
                {
                    m_CanAddComment = SpacePermission.Can(My, SpacePermissionSet.Action.AddComment);
                }
                return m_CanAddComment.Value;
            }
        }

        #endregion

        #region  顶部导航菜单 TopLink

        protected NavigationItemCollection ParentTopLinks
        {
            get
            {
                if (IsLogin)
                    return AllSettings.Current.TopLinkSettings2.ParentItems;
                else
                    return AllSettings.Current.TopLinkSettings2.GuestParentItems;
            }
        }

        protected int ChildTopLinkCount
        {
            get
            {
                if (IsLogin)
                    return AllSettings.Current.TopLinkSettings2.ItemsCount;
                else
                    return AllSettings.Current.TopLinkSettings2.GuestItemsCount;
            }
        }

        protected NavigationItemCollection GetChildItems(NavigationItem item)
        {
            if (IsLogin)
                return item.ChildItems;
            else
                return item.GuestChildItems;
        }

        private int? m_ParentSelectedTopLinkID;
        protected int ParentSelectedTopLinkID
        {
            get
            {
                if (m_ParentSelectedTopLinkID == null)
                {
                    int currentItemID, parentSelectedItemID;
                    SetCurrentNavigationID(false, out currentItemID, out parentSelectedItemID);
                    m_CurrentTopLinkID = currentItemID;
                    m_ParentSelectedTopLinkID = parentSelectedItemID;
                }
                return m_ParentSelectedTopLinkID.Value;
            }
        }

        private int? m_CurrentTopLinkID;
        protected int CurrentTopLinkID
        {
            get
            {
                if (m_CurrentTopLinkID == null)
                {
                    int currentItemID, parentSelectedItemID;
                    SetCurrentNavigationID(false, out currentItemID, out parentSelectedItemID);
                    m_CurrentTopLinkID = currentItemID;
                    m_ParentSelectedTopLinkID = parentSelectedItemID;
                }

                return m_CurrentTopLinkID.Value;
            }
        }


        #endregion


        #region  导航菜单 NavigationItems

        protected NavigationItemCollection ParentNavigations
        {
            get
            {
                if (IsLogin)
                    return AllSettings.Current.NavigationSettings.ParentItems;
                else
                    return AllSettings.Current.NavigationSettings.GuestParentItems; 
            }
        }

        protected int ChildNavigationItemCount
        {
            get
            {
                if (IsLogin)
                    return AllSettings.Current.NavigationSettings.ItemsCount;
                else
                    return AllSettings.Current.NavigationSettings.GuestItemsCount;
            }
        }


        /// <summary>
        /// 用来标记当前页是属于导航菜单中的哪一项(版块ID)
        /// </summary>
        protected virtual int NavigationForumID
        {
            get
            {
                return 0;
            }
        }


        private int? m_ParentSelectedNavigatonItemID;
        protected int ParentSelectedNavigatonItemID
        { 
            get
            {
                if (m_ParentSelectedNavigatonItemID == null)
                {
                    int currentItemID, parentSelectedItemID;
                    SetCurrentNavigationID(false, out currentItemID, out parentSelectedItemID);
                    m_CurrentNavigatonItemID = currentItemID;
                    m_ParentSelectedNavigatonItemID = parentSelectedItemID;
                }
                return m_ParentSelectedNavigatonItemID.Value;
            }
        }

        private int? m_CurrentNavigatonItemID;
        protected int CurrentNavigatonItemID
        {
            get
            {
                if (m_CurrentNavigatonItemID == null)
                {
                    int currentItemID,parentSelectedItemID;
                    SetCurrentNavigationID(false, out currentItemID, out parentSelectedItemID);
                    m_CurrentNavigatonItemID = currentItemID;
                    m_ParentSelectedNavigatonItemID = parentSelectedItemID;
                }

                return m_CurrentNavigatonItemID.Value;
            }
        }

        private void SetCurrentNavigationID(bool isTopLink, out int currentItemID, out int parentSelectedItemID)
        {
            if (NavigationKey == null && NavigationForumID == 0)
            {
                currentItemID = 0;
                parentSelectedItemID = 0;
            }
            else
            {
                string key;
                if (NavigationKey != null)
                {
                    key = NavigationKey;
                }
                else
                {
                    key = NavigationForumID.ToString();
                }

                NavigationItem item;
                if(isTopLink)
                    item = (NavigationItem)AllSettings.Current.TopLinkSettings2.HashNavigations[key];
                else
                    item = (NavigationItem)AllSettings.Current.NavigationSettings.HashNavigations[key];

                if (item == null)
                {
                    currentItemID = 0;
                    parentSelectedItemID = 0;
                }
                else
                {
                    currentItemID = item.ID;
                    if (item.ParentID == 0)
                        parentSelectedItemID = item.ID;
                    else
                        parentSelectedItemID = item.ParentID;
                }
            }
        }

        #endregion

        #region 在线列表相关

        /// <summary>
        /// 更新在线状态
        /// </summary>
        /// <param name="action"></param>
        /// <param name="threadID"></param>
        public void UpdateOnlineStatus(OnlineAction action, int threadID, string subject)
        {
            OnlineUserPool.Instance.Update(My, _Request, action, this.CurrentForumID, threadID, subject);
        }

        protected string SetOnlineUrl
        {
            get { return AttachQueryString("invisible=0"); }
        }

        private void ProcessInvisible()
        {
            if (_Request.Get("invisible", Method.Get) != null)
            {
                if (MyUserID == 0)
                    return;

                bool invisible = _Request.Get<int>("invisible", Method.Get, 0) == 1;

                OnlineUserPool.Instance.Update(My, invisible);
            }
        }

        #region 检查在线状态的函数

        /// <summary>
        /// 检查指定的用户是否在线
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsOnline(User user)
        {
            return OnlineUserPool.Instance.IsOnline(user.UserID);
        }

        /// <summary>
        /// 检查指定的用户是否在线
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsOnline(int userID)
        {
            return OnlineUserPool.Instance.IsOnline(userID);
        }

        /// <summary>
        /// 检查指定的用户是否在线或隐身
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsOnlineOrInvisible(User user)
        {
            return OnlineUserPool.Instance.IsOnlineOrInvisible(user.UserID);
        }

        /// <summary>
        /// 检查指定的用户是否在线或隐身
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsOnlineOrInvisible(int userID)
        {
            return OnlineUserPool.Instance.IsOnlineOrInvisible(userID);
        }

        /// <summary>
        /// 检查指定的用户是否隐身
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsInvisible(User user)
        {
            return OnlineUserPool.Instance.IsInvisible(user.UserID);
        }

        /// <summary>
        /// 检查指定的用户是否隐身
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsInvisible(int userID)
        {
            return OnlineUserPool.Instance.IsInvisible(userID);
        }





        protected string GetOnlineActionName(OnlineAction onlineAction)
        {
            return OnlineUserPool.Instance.ActionName(onlineAction);
        }

        #endregion

        #endregion

        #region 举报

        private bool? m_CanManageDenouncing;

        protected bool CanManageDenouncing
        {
            get
            {
                if (m_CanManageDenouncing == null)
                {
                    m_CanManageDenouncing = AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Manage_Report);
                }

                return m_CanManageDenouncing.Value;
            }
        }

        private int? m_DenouncingCount;

        private int? m_DenouncingPhotoCount;
        private int? m_DenouncingArticleCount;
        private int? m_DenouncingShareCount;
        private int? m_DenouncingUserCount;
        private int? m_DenouncingTopicCount;
        private int? m_DenouncingReplyCount;

        private void EnsureDenouncingCountGet()
        {
            if (m_DenouncingCount == null)
            {
                DenouncingBO.Instance.GetDenouncingCount(
                    out m_DenouncingPhotoCount,
                    out m_DenouncingArticleCount,
                    out m_DenouncingShareCount,
                    out m_DenouncingUserCount,
                    out m_DenouncingTopicCount,
                    out m_DenouncingReplyCount
                );

                m_DenouncingCount =
                    m_DenouncingPhotoCount +
                    m_DenouncingArticleCount +
                    m_DenouncingShareCount +
                    m_DenouncingUserCount +
                    m_DenouncingTopicCount +
                    m_DenouncingReplyCount;
            }
        }

        protected int DenouncingCount
        {
            get
            {
                EnsureDenouncingCountGet();

                return m_DenouncingCount.Value;
            }
        }


        protected int DenouncingPhotoCount
        {
            get
            {
                EnsureDenouncingCountGet();

                return m_DenouncingPhotoCount.Value;
            }
        }

        protected int DenouncingArticleCount
        {
            get
            {
                EnsureDenouncingCountGet();

                return m_DenouncingArticleCount.Value;
            }
        }

        protected int DenouncingShareCount
        {
            get
            {
                EnsureDenouncingCountGet();

                return m_DenouncingShareCount.Value;
            }
        }

        protected int DenouncingUserCount
        {
            get
            {
                EnsureDenouncingCountGet();

                return m_DenouncingUserCount.Value;
            }
        }

        protected int DenouncingTopicCount
        {
            get
            {
                EnsureDenouncingCountGet();

                return m_DenouncingTopicCount.Value;
            }
        }

        protected int DenouncingReplyCount
        {
            get
            {
                EnsureDenouncingCountGet();

                return m_DenouncingReplyCount.Value;
            }
        }

        #endregion

        #region 论坛相关

        private ForumCollection m_ForumCatalogs;
        protected ForumCollection ForumCatalogs
        {
            get
            {
                if (m_ForumCatalogs == null)
                {
                    m_ForumCatalogs = ForumBO.Instance.GetCategories();
                }
                return m_ForumCatalogs;
            }
        }


        protected string GetForumUrl(string codeName)
        {
            return MaxLabs.bbsMax.Common.BbsUrlHelper.GetForumUrl(codeName);
        }

        #endregion

#endif

#if Passport
        
        protected bool EnableAlbumFunction
        {
            get { return  false; }
        }

        protected bool EnableBlogFunction
        {
            get { return false; }
        }

        protected bool EnableShareFunction
        {
            get { return false; }
        }

        protected bool EnableFavoriteFunction
        {
            get { return false; }
        }


        protected bool EnableDoingFunction
        {
            get { return false; }
        }

        protected bool EnableNetDiskFunction
        {
            get { return false; }
        }

        //protected bool EnableUserEmoticon
        //{

        //    get { return EmoticonBO.Instance.IsEnableEmotion; }
        //}

        protected bool EnableMissionFunction
        {
            get { return false; }
        }

        protected bool EnableImpressionFunction
        {
            get { return AllSettings.Current.ImpressionSettings.EnableImpressionFunction; }
        }

        protected bool EnablePropFunction
        {
            get { return false; }
        }

        protected bool EnableEmoticonFunction
        {
            get { return false; }
        }

        protected bool EnablePointExchange
        {
            get { return AllSettings.Current.PointSettings.EnablePointExchange; }
        }
        protected bool EnablePointTransfer
        {
            get { return AllSettings.Current.PointSettings.EnablePointTransfer; }
        }

        protected bool EnablePointRecharge
        {
            get { return AllSettings.Current.PointSettings.EnablePointRecharge; }
        }



#endif
        protected bool IsEnableRealname
        {
            get
            {
                return AllSettings.Current.NameCheckSettings.EnableRealnameCheck;
            }
        }

        protected bool EnableMobileBind
        {
            get { return AllSettings.Current.PhoneValidateSettings.Open; }
        }

        /// <summary>
        /// 是否开启实名认证
        /// </summary>
        protected bool EnableRealnameCheck
        {
            get
            {
                return AllSettings.Current.NameCheckSettings.EnableRealnameCheck;
            }
        }

        /// <summary>
        /// 清除文本中的html部分。例如：“<span>text</span>”使用本函数后将变为“text”
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string ClearHTML(string text)
        {
            return StringUtil.ClearAngleBracket(text);
        }

        protected string SiteName
        {
            get
            {
                return AllSettings.Current.SiteSettings.SiteName;
            }
        }
        /// <summary>
        /// 邀请功能是否打开
        /// </summary>
        protected bool EnableInvitation
        {
            get { return AllSettings.Current.InvitationSettings.InviteMode != InviteMode.Close; }
        }

        protected SiteSettings SiteSettings
        {
            get { return AllSettings.Current.SiteSettings; }
        }

        protected PassportClientConfig PassportClient
        {
            get
            {
                return Globals.PassportClient;
            }
        }

        protected NotifyTypeCollection NotifyTypeList
        {
            get
            {
                return NotifyBO.AllNotifyTypes;
            }
        }

        protected string AttachQueryString(string queryString)
        {
            return AttachQueryString(queryString, true);
        }

        protected string AttachQueryString(string queryString, bool urlEncodeQueryString)
        {
            UrlScheme scheme = BbsRouter.GetCurrentUrlScheme();
            scheme.AttachQuery(queryString);
            return scheme.ToString(urlEncodeQueryString);
        }

        protected string SetInvisibleUrl
        {
            get { return AttachQueryString("invisible=1"); }
        }

        protected string GetRoleNames(UserRoleCollection roles, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (UserRole role in roles)
            {
                if (!role.Role.IsHidden)
                    sb.Append(role.RoleName + separator);
            }
            if (sb.Length > 0)
            {
                return sb.ToString(0, sb.Length - separator.Length);
            }
            return string.Empty;
        }

        /*
        protected TopLinkCollection TopLinks
        {
            get { return AllSettings.Current.TopLinkSettings.Links; }
        }

        protected string GetTopLinksString(string forumLink, string spaceLink, string linkStyle)
        {
            StringBuilder linkStrings = new StringBuilder();
            foreach (TopLink link in TopLinks)
            {
                if (link.LinkID == -1)
                {
                    if (string.IsNullOrEmpty(forumLink))
                        continue;
                    linkStrings.AppendFormat(forumLink, IndexUrl);
                }
                else if (link.LinkID == -2)
                {
                    if (string.IsNullOrEmpty(spaceLink))
                        continue;
                    if (My.UserID == 0)
                        continue;
                    linkStrings.AppendFormat(spaceLink, BbsRouter.GetUrl("space/" + MyUserID));
                }
                else
                {
                    if (link.OnlyLoginCanSee)
                    {
                        if (My.UserID == 0)
                            continue;
                    }

                    string target = "";
                    if (link.NewWindow)
                        target = "_blank";

                    linkStrings.AppendFormat(linkStyle, link.Url, link.Name, target);
                }

                linkStrings.Append(" | ");
            }

            if (linkStrings.Length > 3)
                return linkStrings.ToString(0, linkStrings.Length - 3);
            else
                return "";
        }

        */
        private bool? m_CanShowIpArea;
        /// <summary>
        /// 能否显示IP所在地
        /// </summary>
        protected bool CanShowIpArea
        {
            get
            {
                if (m_CanShowIpArea == null)
                {
                    if (SiteSettings.ViewIPFields.GetValue(My) > 2)
                        m_CanShowIpArea = true;
                    else if (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Setting_AccessLimit))
                        m_CanShowIpArea = true;
                    else
                        m_CanShowIpArea = false;
                }
                return m_CanShowIpArea.Value;
            }
        }

        /// <summary>
        /// 弹出错误提示
        /// </summary>
        /// <param name="message"></param>
        protected void AlertError(string message)
        {
            if (IsAjaxRequest)
            {
                JsonBuilder json = new JsonBuilder();
                json.Set("iserror", true);
                json.Set("message", message);
                AjaxPanelContext.SetAjaxResult(json);
            }
            else
            {
                //TODO:
            }
        }

        /// <summary>
        /// 弹出警告提示
        /// </summary>
        /// <param name="message"></param>
        protected void AlertWarning(string message)
        {
            if (IsAjaxRequest)
            {
                JsonBuilder json = new JsonBuilder();
                json.Set("iswarning", true);
                json.Set("message", message);
                AjaxPanelContext.SetAjaxResult(json);
            }
            else
            {
                //TODO:
            }
        }

        /// <summary>
        /// 弹出成功提示
        /// </summary>
        protected void AlertSuccess(string message)
        {
            if (IsAjaxRequest)
            {
                JsonBuilder json = new JsonBuilder();
                json.Set("issuccess", true);
                json.Set("message", message);
                AjaxPanelContext.SetAjaxResult(json);
            }
            else
            {
                //TODO:
            }
        }

        protected string CurrentSkinID
        {
            get { return WebEngine.Context.Current.Skin.SkinID; }
        }

        private SkinCollection m_SkinList;

        protected SkinCollection TheSkinList
        {
            get
            {
                if (m_SkinList == null)
                {
                    m_SkinList = TemplateManager.GetEnabledSkins();
                }

                return m_SkinList;
            }
        }

        protected string FormatLink(string link)
        {
            return UrlUtil.FormatLink(link);
        }

        #region 短消息或者通知提示音代码

        protected string MessageSound
        {
            get
            {
                if (AllSettings.Current.ChatSettings.HasMessageSound)
                {
                    return string.Concat("<bgsound src=\"", AllSettings.Current.ChatSettings.MessageSound, "\" />");
                }
                return string.Empty;
            }
        }

        #endregion

        protected virtual string PageName
        {
            get
            {
                return string.Empty;
            }
        }

        protected string GetSafeJs(string text)
        {
            return StringUtil.ToJavaScriptString(text);
        }


        protected bool IsShow(int privacyType, User user)
        {
            ExtendedFieldDisplayType displayType = (ExtendedFieldDisplayType)privacyType;
            if (user.UserID == MyUserID)
                return true;
            if (displayType == ExtendedFieldDisplayType.AllVisible)
                return true;
            else if (displayType == ExtendedFieldDisplayType.FriendVisible)
            {
                if (FriendBO.Instance.IsFriend(MyUserID, user.UserID))
                    return true;
                else
                    return false;
            }

            return false;
        }


        protected double GetPercent(int count, int totalCount)
        {
            return MathUtil.GetPercent(count, totalCount);
        }



        #region Medal

        protected MedalCollection Medals
        {
            get
            {
                return AllSettings.Current.MedalSettings.Medals;
            }
        }

        protected bool IsShowMedal(User user)
        {
            if (Medals.Count == 0)
                return false;

            foreach (Medal medal in Medals)
            {
                if (medal.GetMedalLevel(user) != null)
                    return true;
            }

            return false;
        }

        private Dictionary<int, string> medalImages = new Dictionary<int, string>();
        protected string GetMedals(string urlFormat, string imgFormat, User user)
        {
            string imgstring;
            if (medalImages.TryGetValue(user.UserID, out imgstring))
                return imgstring;

            StringBuilder imgs = new StringBuilder();
            foreach (Medal medal in Medals)
            {
                MedalLevel medalLevel = medal.GetMedalLevel(user);
                if (medalLevel != null)
                {
                    string title;
                    if (medalLevel.Name != string.Empty)
                    {
                        title = medal.Name + "(" + medalLevel.Name + ")";
                    }
                    else
                        title = medal.Name;

                    UserMedal userMedal = user.UserMedals.GetValue(medal.ID, medalLevel.ID);
                    if (userMedal != null && string.IsNullOrEmpty(userMedal.ShowUrl) == false)
                    {
                        string url = userMedal.ShowUrl;
                        if (string.IsNullOrEmpty(userMedal.UrlTitle) == false)
                            title = userMedal.UrlTitle;
                        imgs.Append(string.Format(urlFormat, url, string.Format(imgFormat, medalLevel.LogoUrl, title)));
                    }
                    else
                        imgs.Append(string.Format(imgFormat, medalLevel.LogoUrl, title));
                }
            }

            imgstring = imgs.ToString();

            medalImages.Add(user.UserID, imgstring);

            return imgstring;
        }

        #endregion


        protected bool IsShowUserExtendProfile(User user, params string[] keys)
        {
            if (user == null || user.UserID <= 0)
                return false;

            foreach (string key in keys)
            {
                if (string.IsNullOrEmpty(GetUserExtendProfile(user, key)) == false)
                {
                    return true;
                }
            }
            return false;
        }

        protected string GetUserExtendProfile(User user, string key)
        {
            UserExtendedValue extendedValue = user.ExtendedFields.GetValue(key);
            if (extendedValue == null)
                return string.Empty;

            if (user.UserID == MyUserID)
                return extendedValue.Value;

            if (extendedValue.PrivacyType == ExtendedFieldDisplayType.AllVisible)
                return extendedValue.Value;
            else if (extendedValue.PrivacyType == ExtendedFieldDisplayType.FriendVisible)
            {
                if (FriendBO.Instance.IsFriend(MyUserID, user.UserID))
                    return extendedValue.Value;
                else
                    return string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }



        #region Job
        protected string JobUrl
        {
            get
            {
                return BbsRouter.GetUrl(MaxLabs.bbsMax.Consts.Handler_JobUrl);
            }
        }

        private bool? m_IsExecuteJobTime;
        protected bool IsExecuteJobTime
        {
            get
            {
                if (m_IsExecuteJobTime == null)
                {
                    m_IsExecuteJobTime = MaxLabs.bbsMax.Jobs.JobManager.IsAfterRequestJobsExecuteTime();
                }
                return m_IsExecuteJobTime.Value;
            }
        }

        #endregion

        private Sys s_Sys;
        protected Sys Sys
        {
            get
            {
                if (s_Sys == null)
                    s_Sys = new Sys();

                return s_Sys;
            }
        }

        public bool IsDebug
        {
            get
            {
#if Debug
                return true;
#else
                return false;
#endif
            }
        }

    }
}