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

using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web.App_Blog
{
    public class view_aspx : CenterBlogPageBase
    {
        protected override string PageName
        {
            get
            {
                return "blog";
            }
        }
        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            int commentPageSize = 10;
            int page = _Request.Get<int>("page", 1);

            if (_Request.IsClick("addcomment"))
            {
                MessageDisplay md = CreateMessageDisplay(GetValidateCodeInputName("CreateComment"));

                if (!CheckValidateCode("CreateComment", md))
                {
                    return;
                }
                string content = _Request.Get("content");
                int replyCommentID = _Request.Get<int>("replycommentid", Method.Post, 0);
                int replyUserID = _Request.Get<int>("replyUserID", Method.Post, 0);

                int commentID = 0;
                string newContent;

                using (ErrorScope es = new ErrorScope())
                {
                    bool succeed;

                    if (replyCommentID > 0)
                        succeed = CommentBO.Instance.ReplyComment(My, ArticleID, replyCommentID, replyUserID, CommentType.Blog, content, _Request.IpAddress, out commentID, out newContent);
                    else
                        succeed = CommentBO.Instance.AddComment(My, ArticleID, 0, MaxLabs.bbsMax.Enums.CommentType.Blog, content, _Request.IpAddress, out commentID);

                    if (succeed == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            if (error is UnapprovedError)
                                AlertWarning(error.Message);
                            else
                                md.AddError(error);
                        });
                    }
                    else
                    {
                        Article.TotalComments++;
                        MaxLabs.bbsMax.ValidateCodes.ValidateCodeManager.CreateValidateCodeActionRecode("CreateComment");
                    }

                    if (succeed)
                    {
                        page = Article.TotalComments / commentPageSize;
                        if (Article.TotalComments % commentPageSize != 0)
                            page++;
                    }
                }

                
            }

            if (Article.PrivacyType == PrivacyType.SelfVisible 
                && Article.UserID != MyUserID
                && HasManagePermission == false)
            {
                ShowError("由于日志主人的隐私设置，您不能查看该篇日志");
            }

            if (Article.PrivacyType == PrivacyType.FriendVisible
                && Article.UserID != MyUserID
                && FriendBO.Instance.IsFriend(MyUserID, Article.UserID) == false
                && HasManagePermission == false)
            {
                ShowError("由于日志主人的隐私设置，您不能查看该篇日志");
            }


            if (_Request.IsClick("submitPassword"))
            {
                ProcessPassword();
            }

            if (IsShowPasswordBox == false && Article.UserID != MyUserID)
            {
                if (BlogBO.Instance.UpdateVisitCount(My, ArticleID))
                    Article.TotalViews += 1;
            }
                
            

            ////指定好友可见  暂时还没有该功能
            //if (Article.PrivacyType == MaxLabs.bbsMax.Enums.PrivacyType.AppointUser)
            //{ 
            //}





            //m_Article = BlogBO.Instance.GetBlogArticleForVisit(MyUserID, id.Value, password);

            //if (m_Article == null)
            //{
            //    ShowError("指定的日志不存在");
            //}

            m_ArticleList = BlogBO.Instance.GetSimilarArticles(MyUserID, Article.UserID, Article.ArticleID, 5);

            m_CommentList = CommentBO.Instance.GetComments(Article.ArticleID, MaxLabs.bbsMax.Enums.CommentType.Blog, page, 10, false, out m_CommentTotalCount);

            WaitForFillSimpleUsers<Comment>(m_CommentList);

            SetPager("commentlist", null, page, commentPageSize, m_CommentTotalCount);

            if (IsSpace == false)
            {
                AddNavigationItem(FunctionName, BbsRouter.GetUrl("app/blog/index"));
                AddNavigationItem(string.Concat("我的", FunctionName), BbsRouter.GetUrl("app/blog/index"));
                AddNavigationItem(Article.Subject);
            }
            else
            {
                AddNavigationItem(string.Concat(AppOwner.Username, "的空间"), UrlHelper.GetSpaceUrl(AppOwner.UserID));
                AddNavigationItem(string.Concat("主人的", FunctionName),UrlHelper.GetBlogIndexUrl(AppOwnerUserID));
                AddNavigationItem(Article.Subject);
            }
        }

        protected override string PageTitle
        {
            get
            {
                if (IsSpace)
                {
                    return string.Concat(Article.Subject, " - ", AppOwner.Username, "的", FunctionName, " - ", base.PageTitle);
                }
                else
                {
                    return string.Concat(Article.Subject, " - 我的", FunctionName, " - ", base.PageTitle);
                }
            }
        }

        protected override int AppOwnerUserID
        {
            get
            {
                return Article.UserID;
            }
        }

        //protected override bool IsSpace
        //{
        //    get
        //    {
        //        if (Article.UserID == MyUserID)
        //            return false;
        //        else
        //            return true;
        //    }
        //}


        private int? m_ArticleID;
        protected int ArticleID
        {
            get
            {
                if (m_ArticleID == null)
                {
                    m_ArticleID = _Request.Get<int>("id", Method.Get, 0);
                }

                return m_ArticleID.Value;
            }
        }

        private BlogArticle m_Article;

        public BlogArticle Article
        {
            get 
            {
                if (m_Article == null)
                {
                    if (ArticleID < 1)
                        ShowError(new InvalidParamError("id"));

                    m_Article = BlogBO.Instance.GetBlogArticle(ArticleID);

                    if (m_Article == null)
                        ShowError("您要查看的日志不存在");


                    BlogBO.Instance.ProcessKeyword(m_Article, ProcessKeywordMode.TryUpdateKeyword);
                }

                return m_Article;
            }
        }

        private BlogArticleCollection m_ArticleList;

        public BlogArticleCollection ArticleList
        {
            get { return m_ArticleList; }
        }

        private CommentCollection m_CommentList;

        public CommentCollection CommentList
        {
            get { return m_CommentList; }
        }

        private int m_CommentTotalCount;

        public int CommentTotalCount
        {
            get { return m_CommentTotalCount; }
        }








        private bool? m_HasManagePermission;
        protected bool HasManagePermission
        {
            get
            {
                if (m_HasManagePermission == null)
                {
                    m_HasManagePermission = AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Manage_Blog, Article.UserID);
                }

                return m_HasManagePermission.Value;
            }
        }

        private bool? m_IsShowPasswordBox;
        protected bool IsShowPasswordBox
        {
            get
            {
                if (m_IsShowPasswordBox == null)
                {
                    if (Article.PrivacyType == MaxLabs.bbsMax.Enums.PrivacyType.NeedPassword
                        && Article.UserID != MyUserID
                        && BlogBO.Instance.HasArticlePassword(My, ArticleID) == false
                        && HasManagePermission == false)
                        m_IsShowPasswordBox = true;
                    else
                        m_IsShowPasswordBox = false;
                }

                return m_IsShowPasswordBox.Value;
            }
        }

        private bool? m_IsShowAdminCanSeeNot;
        protected bool IsShowAdminCanSeeNot
        {
            get
            {
                if (m_IsShowAdminCanSeeNot == null)
                {
                    if (Article.UserID == MyUserID)
                    {
                        m_IsShowAdminCanSeeNot = false;
                    }
                    else
                    {
                        if (Article.PrivacyType == PrivacyType.FriendVisible
                            && FriendBO.Instance.IsFriend(MyUserID, Article.UserID) == false
                            && HasManagePermission)
                            m_IsShowAdminCanSeeNot = true;

                        if (Article.PrivacyType == PrivacyType.NeedPassword
                            && HasManagePermission)
                            m_IsShowAdminCanSeeNot = true;

                        if (Article.PrivacyType == PrivacyType.SelfVisible
                            && HasManagePermission)
                            m_IsShowAdminCanSeeNot = true;

                        if (m_IsShowAdminCanSeeNot == null)
                            m_IsShowAdminCanSeeNot = false;
                    }
                }

                return m_IsShowAdminCanSeeNot.Value;
            }
        }

        private void ProcessPassword()
        {
            MessageDisplay msgDisplay = CreateMessageDisplayForForm("passwordform", "password");

            string password = _Request.Get("password", Method.Post, string.Empty);

            if (BlogBO.Instance.CheckArticlePassword(My, ArticleID, password) == false)
            {
                msgDisplay.AddError("password", "密码错误");
            }
        }
    }
}