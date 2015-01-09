//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;
using System.Web.UI;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.WebEngine;
using System.Text;
using System.Data;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Errors;
namespace MaxLabs.bbsMax.Web.max_pages.forums
{
    public partial class signinforum : ForumPageBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {

            if (Forum == null)
            {
                ShowError("版块不存在");
            }

            AddNavigationItem("版块登录");
            //SetPageTitle("版块登录");


            if (_Request.IsClick("SignInButton"))
            {
                ProcessSignInForum();
            }
        }

        protected override bool CheckPermission()
        {
            return true;
        }

        protected override void ProcessForumType()
        {
        }

        protected void ProcessSignInForum()
        {
            string validateCodeAction = "SignInForum";
            MessageDisplay msgDisplay = CreateMessageDisplay();
            if (CheckValidateCode(validateCodeAction, msgDisplay))
            {
                string password = _Request.Get("Password", Method.Post, string.Empty);
                if (password == string.Empty)
                {
                    msgDisplay.AddError("请输入版块密码");
                    return;
                }
                if (password == Forum.Password)
                {
                    My.AddValidatedForumID(Forum.ForumID, password);
                    //Response.Redirect(_Request.Get("UrlReferrer", Method.Post, BbsUrlHelper.GetForumUrl(Forum.CodeName)));
                    int threadID = _Request.Get<int>("threadid", Method.Get, 0);
                    if (threadID > 0)
                    {
                        Response.Redirect(BbsUrlHelper.GetThreadUrl(Forum.CodeName, threadID, 1));
                    }
                    else
                        Response.Redirect(BbsUrlHelper.GetForumUrl(Forum.CodeName));
                }
                else
                {
                    msgDisplay.AddError("密码不正确");
                }
            }
        }
    }
}