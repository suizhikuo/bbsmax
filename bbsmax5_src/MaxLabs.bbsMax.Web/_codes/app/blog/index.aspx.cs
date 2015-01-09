//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web.App_Blog
{
    public partial class index : CenterBlogPageBase
    {

        protected override string PageTitle
        {
            get
            {
                if (IsSpace)
                {
                    return string.Concat(AppOwner.Name, " - ", FunctionName, " - ", base.PageTitle);
                }
                else if (SelectedMy)
                    return string.Concat("我的", FunctionName, " - ", base.PageTitle);
                else if (SelectedCommented)
                    return string.Concat("评论过的", FunctionName, " - ", base.PageTitle);
                else if (SelectedEveryone)
                    return string.Concat("大家的", FunctionName, " - ", base.PageTitle);
                else //if (SelectedFriend)
                    return string.Concat("好友的", FunctionName, " - ", base.PageTitle);

            }
        }

        protected override string PageName
        {
            get { return "blog"; }
        }

        protected override string NavigationKey
        {
            get { return "blog"; }
        }

		protected override void OnLoadComplete(EventArgs e)
		{
            m_CategoryID = _Request.Get<int>("cid");

			m_TagID = _Request.Get<int>("tid");

			int pageNumber = _Request.Get<int>("page", 1);

			using (ErrorScope es = new ErrorScope())
			{
                if (IsSpace)
                {
                    m_ArticleList = BlogBO.Instance.GetUserBlogArticles(MyUserID, AppOwnerUserID, m_CategoryID, m_TagID, pageNumber, ArticleListPageSize);
                    m_CategoryList = BlogBO.Instance.GetUserBlogCategories(AppOwnerUserID);
                    m_TagList = TagBO.Instance.GetUserTags(AppOwnerUserID, TagType.Blog);
                }
                else
                {

                    if (SelectedMy)
                    {
                        m_ArticleList = BlogBO.Instance.GetUserBlogArticles(MyUserID, MyUserID, m_CategoryID, m_TagID, pageNumber, ArticleListPageSize);

                        m_CategoryList = BlogBO.Instance.GetUserBlogCategories(MyUserID);

                        m_TagList = TagBO.Instance.GetUserTags(MyUserID, TagType.Blog);
                    }
                    else if (SelectedFriend)
                    {
                        m_ArticleList = BlogBO.Instance.GetFriendBlogArticles(MyUserID, pageNumber, ArticleListPageSize);
                    }
                    else if (SelectedCommented)
                    {
                        m_ArticleList = BlogBO.Instance.GetCommentedArticles(MyUserID, pageNumber, ArticleListPageSize);
                    }
                    else if (SelectedEveryone)
                    {
                        m_ArticleList = BlogBO.Instance.GetEveryoneBlogArticles(pageNumber, ArticleListPageSize);
                    }
                }

                if (m_ArticleList != null)
                {
                    m_TotalArticleCount = m_ArticleList.TotalRecords;

                    UserBO.Instance.WaitForFillSimpleUsers<BlogArticle>(m_ArticleList);

                    m_ArticleAuthorList = UserBO.Instance.GetUsers(m_ArticleList.GetUserIds());
                }
				if (es.HasUnCatchedError)
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
					{
						ShowError(error);
					});
				}
			}

            base.OnLoadComplete(e);

            SetPager("pager1", null, pageNumber, ArticleListPageSize, TotalArticleCount);

            if (IsSpace == false)
            {
                AddNavigationItem(FunctionName, BbsRouter.GetUrl("app/blog/index"));

                if (SelectedMy)
                    AddNavigationItem(string.Concat("我的", FunctionName));
                else if (SelectedFriend)
                    AddNavigationItem(string.Concat("好友的", FunctionName));
                else if (SelectedEveryone)
                    AddNavigationItem(string.Concat("大家的", FunctionName));
                else
                    AddNavigationItem(string.Concat("评论过的", FunctionName));
            }
            else
            {
                AddNavigationItem(string.Concat(AppOwner.Username, "的空间"), UrlHelper.GetSpaceUrl(AppOwner.UserID));
                AddNavigationItem(string.Concat("主人的", FunctionName));

            }
		}

		private BlogArticleCollection m_ArticleList;

		/// <summary>
		/// 日志列表
		/// </summary>
		public BlogArticleCollection ArticleList
		{
			get { return m_ArticleList; }
		}

		private UserCollection m_ArticleAuthorList;

		/// <summary>
		/// 日志作者列表
		/// </summary>
		public UserCollection ArticleAuthorList
		{
			get { return m_ArticleAuthorList; }
		}

		private int? m_TotalArticleCount;

		/// <summary>
		/// 日志总数（不只是当前页面上的日志列表中的条数，而是包含没显示在当前页的数据）
		/// </summary>
		public int TotalArticleCount
		{
			get { return m_TotalArticleCount.GetValueOrDefault(); }
		}

		/// <summary>
		/// 日志列表分页尺度
		/// </summary>
		public int ArticleListPageSize
		{
			get { return 5; }
		}

		private BlogCategoryCollection m_CategoryList;

		/// <summary>
		/// 日志分类列表
		/// </summary>
		public BlogCategoryCollection CategoryList
		{
			get { return m_CategoryList; }
		}

		private int? m_CategoryID;

		public int CategoryID
		{
			get { return m_CategoryID.GetValueOrDefault(-1); }
		}

		private int? m_TagID;

		public int TagID
		{
			get { return m_TagID.GetValueOrDefault(); }
		}

		private TagCollection m_TagList;

		public TagCollection TagList
		{
			get { return m_TagList; }
		}


        private bool? m_CanManageCategory;
        protected bool CanManageCategory
        {
            get
            {
                if (m_CanManageCategory == null)
                {
                    if (MyUserID == AppOwnerUserID || AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Manage_Blog, AppOwnerUserID))
                        m_CanManageCategory = true;
                    else
                        m_CanManageCategory = false;
                }

                return m_CanManageCategory.Value;
            }
        }

        protected bool CanManageBlog
        {
            get
            {
                return CanManageCategory;
            }
        }

        protected bool CanSee(BlogArticle article)
        {
            if (IsSpace == false && SelectedMy)
                return true;

            if(article.UserID == MyUserID)
                return true;

            if (article.PrivacyType == PrivacyType.AllVisible)
                return true;

            if (IsSpace)
            {
                if (CanManageBlog)
                    return true;
            }

            if (article.PrivacyType == PrivacyType.FriendVisible)
            {
                if (SelectedFriend)
                    return true;
                else if (FriendBO.Instance.IsFriend(MyUserID, article.UserID))
                    return true;
            }
            else if (article.PrivacyType == PrivacyType.NeedPassword)
            {
                if (BlogBO.Instance.HasArticlePassword(My, article.ArticleID))
                    return true;
            }

            if (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Manage_Blog, article.UserID))
                return true;

            return false;
            
        }
    }
}