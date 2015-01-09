//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class logout : BbsPageBase
    {
        protected override string PageName
        {
            get { return "logout"; }
        }

        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected override bool NeedCheckForumClosed
        {
            get { return false; }
        }

        protected override bool NeedCheckAccess
        {
            get { return false; }
        }

        protected override bool NeedCheckVisit
        {
            get { return false; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int userID = _Request.Get<int>("uid", Method.Get,0) ;

            if (userID == MyUserID)
            {
                AuthUser my = My;
                UserBO.Instance.Logout();
#if !Passport
                OnlineUserPool.Instance.Remove(my);
#endif
            }
#if !Passport
            PassportClientConfig setting = Globals.PassportClient;
            if (setting.EnablePassport)
            {
                Response.Redirect(setting.LogoutUrl + "?returnurl=" + HttpUtility.UrlEncode(ReturnUrl));
                return;
            }
#endif
            ShowSuccess("您已经安全退出", AllSettings.Current.PassportServerSettings.EnablePassportService?ReturnUrl: IndexUrl);
        }

        private string m_ReturnUrl = null;
        protected string ReturnUrl
        {
            get
            {
                if (m_ReturnUrl == null)
                {

                    string returnUrl = _Request.Get("returnurl", Method.All, null, false);

                    //如果有提交ReturnUrl且是本主域内的安全地址，则登陆成功后自动跳转到returnurl
                    if (!string.IsNullOrEmpty(returnUrl))//BbsRouter.IsUrlInMainDomain(returnUrl))
                    {
                        m_ReturnUrl = returnUrl; //TODO
                        //m_ReturnUrl = UrlUtil. GetFilteredUrl(returnUrl);
                    }
                    else
                    {
                        m_ReturnUrl = BbsRouter.GetUrl("login");
                    }

                    ////如果能得到上一页的地址且是本程序内的安全地址，则登陆成功后自动跳转到上一页
                    //else if (Request.UrlReferrer != null && Request.UrlReferrer.OriginalString.StartsWith(Globals.FullAppRoot + "/"))
                    //{
                    //    m_ReturnUrl = UrlUtil.GetFilteredUrl(Request.UrlReferrer.OriginalString);
                    //}

                    ////以上情况都不是，跳转到首页 
                    //else
                    //{
                    //    m_ReturnUrl = IndexUrl;
                    //}

                }
                return m_ReturnUrl;
            }
        }
    }
}