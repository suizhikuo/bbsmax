//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Web;

//using MaxLabs.WebEngine;
//using MaxLabs.bbsMax.Entities;
//using MaxLabs.bbsMax.Enums;
//using MaxLabs.bbsMax.Settings;
//using MaxLabs.bbsMax.Filters;


//namespace MaxLabs.bbsMax.Templates
//{
//    [TemplatePackage]
//    public class BlogTemplateMembers
//    {
//        /// <summary>
//        /// 当前用户ID
//        /// </summary>
//        private int m_CurrentUserID = UserBO.Instance.GetUserID();

//        #region 模板成员参数类及委托

//        #region 日志

//        public class BlogArticleListHeadFootParams
//        {
//            private int? m_TotalArticles;
//            private string m_View;
//            private BlogArticle m_Article;
//            private int m_pageSize = Consts.DefaultPageSize;
//            private AdminBlogFilter m_AdvFilter;
//            private BlogFilter m_Filter;

//            public BlogArticleListHeadFootParams()
//            {
//            }

//            public BlogArticleListHeadFootParams(BlogArticle aritlce)
//            {
//                m_Article = aritlce;
//            }

//            public BlogArticleListHeadFootParams(int? totalArticles)
//            {
//                m_TotalArticles = totalArticles;
//            }

//            public BlogArticleListHeadFootParams(string view, int? totalArticles, int pageSize)
//            {
//                m_View = view;
//                m_TotalArticles = totalArticles;
//                m_pageSize = pageSize;
//            }

//            public BlogArticleListHeadFootParams(string view, int? totalArticles,  int pageSize, AdminBlogFilter filter)
//            {
//                m_View = view;
//                m_TotalArticles = totalArticles;
//                m_pageSize = pageSize;
//                m_AdvFilter = filter;
//            }

//            public BlogArticleListHeadFootParams(string view, int? totalArticles,  int pageSize, BlogFilter filter)
//            {
//                m_View = view;
//                m_TotalArticles = totalArticles;
//                m_pageSize = pageSize;
//                m_Filter = filter;
//            }

//            public int TotalArticles
//            {
//                get { return m_TotalArticles != null ? m_TotalArticles.Value : 0; }
//            }

//            public int PageSize
//            {
//                get { return m_pageSize; }
//            }

//            public BlogFilter SearchForm
//            {
//                get { return m_Filter; }
//            }

//            public AdminBlogFilter AdminForm
//            {
//                get { return m_AdvFilter; }
//            }

//            /// <summary>
//            /// 是否没有数据
//            /// </summary>
//            public bool HasItems
//            {
//                get
//                {
//                    return m_TotalArticles != null && m_TotalArticles.Value > 0;
//                }
//            }

//            /// <summary>
//            /// 是否能显示好友的日志
//            /// </summary>
//            public bool CanDisplayFriend
//            {
//                get
//                {
//                    return string.IsNullOrEmpty(m_View) || string.Compare(m_View, "Friend", true) == 0;
//                }
//            }

//            /// <summary>
//            /// 是否能显示我的日志
//            /// </summary>
//            public bool CanDisplayMine
//            {
//                get
//                {
//                    return string.Compare(m_View, "Self", true) == 0;
//                }
//            }

//            /// <summary>
//            /// 是否能显示我看过的日志
//            /// </summary>
//            public bool CanDisplayUserTrace
//            {
//                get
//                {
//                    return string.Compare(m_View, "Trace", true) == 0;
//                }
//            }

//            /// <summary>
//            /// 是否能显示大家的日志
//            /// </summary>
//            public bool CanDisplayAll
//            {
//                get
//                {
//                    return string.Compare(m_View, "All", true) == 0;
//                }
//            }
//        }

//        public class BlogArticleListItemParams
//        {
//            /// <summary>
//            /// 当前用户ID
//            /// </summary>
//            private int m_CurrentUserID = UserBO.Instance.GetUserID();
//            private int m_UserID = 0;
//            private int m_CategoryID = 0;
//            private string m_Action = string.Empty;
//            private string m_Password = string.Empty;

//            private bool m_HasManagePermissionForSomeone;
//            private BlogArticle m_BlogArticle;
//            public BlogArticleListItemParams(BlogArticle article,bool hasManagePermissionForSomeone)
//            {
//                m_UserID = article.UserID;
//                m_BlogArticle = article;
//                m_CategoryID = article.CategoryID;
//                m_Tags = article.Tags;
//                m_HasManagePermissionForSomeone = hasManagePermissionForSomeone;
//            }

//            public BlogArticleListItemParams(BlogArticle article, int userID)
//            {
//                m_UserID = article.UserID;
//                m_BlogArticle = article;
//                m_UserID = userID;
//                m_CategoryID = article.CategoryID;
//                m_Tags = article.Tags;
//            }

//            public BlogArticleListItemParams(BlogArticle article, string password, string action)
//            {
//                m_UserID = article.UserID;
//                m_BlogArticle = article;
//                m_CategoryID = article.CategoryID;
//                m_Password = password;
//                m_Action = action;
//                m_Tags = article.Tags;
//            }

//            public BlogArticleListItemParams(BlogArticle article, int userID, string action)
//            {
//                m_UserID = article.UserID;
//                m_BlogArticle = article;
//                m_UserID = userID;
//                m_CategoryID = article.CategoryID;
//                m_Action = action;
//                m_Tags = article.Tags;
//            }

//            public BlogArticle Article
//            {
//                get { return m_BlogArticle; }
//            }

//            public bool HasTags
//            {
//                get { return !string.IsNullOrEmpty(TagLinks) || !string.IsNullOrEmpty(Tags); }
//            }


//            private TagCollection m_Tags = null;
//            /// <summary>
//            /// 标签
//            /// </summary>
//            public string TagLinks
//            {
//                get
//                {
//                    if (string.Compare(m_Action, "View", true) != 0 && string.Compare(m_Action, "Edit", true) != 0)
//                    {
//                        return string.Empty;
//                    }
//                    if (m_Tags == null)
//                    {
//                        m_Tags = TagBO.Instance.GetTags(TagType.Blog, m_BlogArticle.ArticleID);
//                    }
//                    StringBuffer sbTags = new StringBuffer();
//                    foreach (Tag t in m_Tags)
//                    {
//                        string url = BbsRouter.GetUrl("tag/" + t.ID + "/blog");

//                        sbTags += string.Format("<a href=\"{0}\" title=\"{1}\">{1}</a>", url, t.Name);
//                        sbTags += ",";
//                    }
//                    string val = sbTags.ToString();
//                    if (val.Length > 0)
//                    {
//                        val = val.Substring(0, val.Length - 1);
//                    }
//                    return val;
//                }
//            }

//            /// <summary>
//            /// 标签
//            /// </summary>
//            public string Tags
//            {
//                get
//                {
//                    if (string.Compare(m_Action, "View", true) != 0 && string.Compare(m_Action, "Edit", true) != 0)
//                    {
//                        return string.Empty;
//                    }
//                    if (m_Tags == null)
//                    {
//                        m_Tags = TagBO.Instance.GetTags(TagType.Blog, m_BlogArticle.ArticleID);
//                    }
//                    StringBuffer sbTags = new StringBuffer();
//                    foreach (Tag t in m_Tags)
//                    {
//                        sbTags += t.Name;
//                        sbTags += ",";
//                    }
//                    string val = sbTags.ToString();
//                    if (val.Length > 0)
//                    {
//                        val = val.Substring(0, val.Length - 1);
//                    }
//                    return val;
//                }
//            }

//            private bool? m_IsAccess;
//            public bool IsAccess
//            {
//                get 
//                {
//                    if (m_IsAccess == null)
//                    {
//                        m_IsAccess = BlogBO.Instance.IsAccess(m_CurrentUserID, m_BlogArticle, m_Password);
//                    }
//                    return m_IsAccess.Value;
//                }
//            }

//            public bool IsEditMode
//            {
//                get { return string.Compare(m_Action, "edit", true) == 0; }
//            }

//            public bool IsNeedPassword
//            {
//                get { return m_BlogArticle.UserID != m_CurrentUserID && m_BlogArticle.PrivacyType == PrivacyType.NeedPassword; }
//            }

//            public int CategoryID
//            {
//                get { return m_CategoryID; }
//            }

//            /// <summary>
//            /// 当篇日志的分类
//            /// </summary>
//            public BlogCategory Category
//            {
//                get
//                {
//                    return BlogBO.Instance.GetOwnCategory(m_UserID, m_CategoryID);
//                }
//            }

//            public bool CanDisplayThumb
//            {
//                get
//                {
//                    return m_BlogArticle.Thumb.Length > 0;
//                }
//            }

//            /// <summary>
//            /// 日志是否为需要密码的隐私类型
//            /// </summary>
//            public bool CanDisplayNeedPassword
//            {
//                get { return (m_BlogArticle.PrivacyType == PrivacyType.NeedPassword) && IsAccess == false; }
//            }

//            /// <summary>
//            /// 日志是否为仅好友可见的隐私类型
//            /// </summary>
//            public bool CanDisplayFriendVisible
//            {
//                get { return (m_BlogArticle.PrivacyType == PrivacyType.FriendVisible) && IsAccess == false; }
//            }

//            /// <summary>
//            /// 日志是否为仅自己可见的隐私类型
//            /// </summary>
//            public bool CanDisplaySelfVisible
//            {
//                get { return (m_BlogArticle.PrivacyType == PrivacyType.SelfVisible) && IsAccess == false; }
//            }

//            private bool? m_DisplayAdminCanSeeInfo;
//            /// <summary>
//            /// 显示管理员可见的信息
//            /// </summary>
//            public bool DisplayAdminCanSeeInfo
//            {
//                get
//                {
//                    if (m_DisplayAdminCanSeeInfo == null)
//                    {
//                        if (m_BlogArticle.PrivacyType == PrivacyType.NeedPassword || m_BlogArticle.PrivacyType == PrivacyType.FriendVisible || m_BlogArticle.PrivacyType == PrivacyType.SelfVisible)
//                        {
//                            if (Article.UserID != m_CurrentUserID && BlogBO.Instance.ManagePermission.Can(m_CurrentUserID, ManageSpacePermissionSet.ActionWithTarget.ManageBlog, Article.UserID))
//                            {
//                                m_DisplayAdminCanSeeInfo = true;
//                            }
//                            else
//                                m_DisplayAdminCanSeeInfo = false;
//                        }
//                        else
//                            m_DisplayAdminCanSeeInfo = false;
//                    }
//                    return m_DisplayAdminCanSeeInfo.Value;
//                }
//            }

//            private bool? m_CanEdit;
//            /// <summary>
//            /// 当前用户是否能编辑列表中的该日志
//            /// </summary>
//            public bool CanEdit
//            {
//                get
//                {
//                    if (m_CanEdit == null)
//                    {
//                        if (m_CurrentUserID == Article.UserID)
//                            m_CanEdit = true;
//                        else
//                            m_CanEdit = m_HasManagePermissionForSomeone;
//                        //m_CanEdit = BlogBO.Instance.ManagePermission.Can(m_CurrentUserID, ManageSpacePermissionSet.ActionWithTarget.ManageBlog, Article.UserID, Article.LastEditUserID);
//                    }
//                    return m_CanEdit.Value;
//                }
//            }

            
//            /// <summary>
//            /// 当前用户是否能删除列表中的该日志
//            /// </summary>
//            public bool CanDelete
//            {
//                get
//                {
//                    if (m_CurrentUserID == m_BlogArticle.UserID)
//                        return true;
//                    else
//                        return CanEdit;
//                }
//            }

//        }

//        public delegate void BlogArticleListHeadFootTemplate(BlogArticleListHeadFootParams _this);
//        public delegate void BlogArticleListItemTemplate(BlogArticleListItemParams _this, int i);
        
//        #endregion

//        #region 日志分类
//        public class BlogCategoryListHeadFootParams
//        {
//            private int? m_TotalCategories;
//            private int m_PageSize = Consts.DefaultPageSize;
//            private int m_SelectedCategoryID = 0;
//            private BlogCategory m_Category;
//            private AdminBlogCategoryFilter m_Filter;

//            public BlogCategoryListHeadFootParams()
//            {

//            }

//            public BlogCategoryListHeadFootParams(BlogCategory category)
//            {
//                m_Category = category;
//            }

//            public BlogCategoryListHeadFootParams(int? totalCategories, int selectedCategoryID)
//            {
//                m_TotalCategories = totalCategories;
//                m_SelectedCategoryID = selectedCategoryID;
//            }

//            public BlogCategoryListHeadFootParams(int? totalCategories, int pageSize, AdminBlogCategoryFilter filter)
//            {
//                m_TotalCategories = totalCategories;
//                m_Filter = filter;
//                m_PageSize = pageSize;
//            }

//            public int TotalCategories
//            {
//                get { return m_TotalCategories != null ? m_TotalCategories.Value : 0; }
//            }

//            public int PageSize
//            {
//                get { return m_PageSize; } 
//            }

//            public AdminBlogCategoryFilter AdminForm
//            {
//                get { return m_Filter; }
//            }

//            /// <summary>
//            /// 是否没有数据
//            /// </summary>
//            public bool HasItems
//            {
//                get { return m_TotalCategories != null && m_TotalCategories.Value > 0; }
//            }

//            /// <summary>
//            /// 是否显示所有日志
//            /// </summary>
//            public bool CanDisplayAll
//            {
//                get { return m_SelectedCategoryID <= 0; }
//            }
//        }

//        public class BlogCategoryListItemParams
//        {
//            private int m_CurrentUserID = UserBO.Instance.GetUserID();
//            private int m_SelectedID = 0;
//            private BlogCategory m_Category;

//            private bool m_HasManagePermissionForSomeOne;
//            public BlogCategoryListItemParams(BlogCategory category, int selectedID,bool hasManagePermissionForSomeOne)
//            {
//                m_Category = category;
//                m_SelectedID = selectedID;
//                m_HasManagePermissionForSomeOne = hasManagePermissionForSomeOne;
//            }

//            public BlogCategory Category
//            {
//                get { return m_Category; }
//            }

//            /// <summary>
//            /// 当前日志分类是否被选中,根据网址传参判断
//            /// </summary>
//            public bool IsActive
//            {
//                get { return m_Category.CategoryID == m_SelectedID; }
//            }

//            private bool? m_CanEdit;
//            /// <summary>
//            /// 当前用户是否可编辑该日志分类
//            /// </summary>
//            public bool CanEdit
//            {
//                get
//                {
//                    if (m_CanEdit == null)
//                    {
//                        if (Category.UserID == m_CurrentUserID)
//                            m_CanEdit = true;
//                        else
//                            m_CanEdit = m_HasManagePermissionForSomeOne;
//                        //else
//                        //    m_CanEdit = BlogBO.Instance.ManagePermission.Can(m_CurrentUserID, ManageSpacePermissionSet.ActionWithTarget.ManageBlog, Category.UserID);
//                    }
//                    return m_CanEdit.Value;
//                }
//            }

//            /// <summary>
//            /// 当前用户是否可删除该日志分类
//            /// </summary>
//            public bool CanDelete
//            {
//                get
//                {
//                    return CanEdit;
//                }
//            }
//        }

//        public delegate void BlogCategoryListHeadFootTemplate(BlogCategoryListHeadFootParams _this);
//        public delegate void BlogCategoryListItemTemplate(BlogCategoryListItemParams _this, int i);
//        #endregion

//        #region 按作者查看
//        public class RelatedAuthorListHeadFootParams
//        {
//            private int? m_TotalRelatedAuthors;

//            public RelatedAuthorListHeadFootParams() { }

//            public RelatedAuthorListHeadFootParams(int? totalRelatedAuthors)
//            {
//                m_TotalRelatedAuthors = totalRelatedAuthors;
//            }

//            /// <summary>
//            /// 是否没有数据
//            /// </summary>
//            public bool NoData
//            {
//                get
//                {
//                    return m_TotalRelatedAuthors == null || m_TotalRelatedAuthors.Value <= 0;
//                }
//            }
//        }

//        public class RelatedAuthorListItemParams
//        {
//            private User m_Author;

//            public RelatedAuthorListItemParams() { }

//            public RelatedAuthorListItemParams(User author) 
//            {
//                m_Author = author;
//            }

//            public User Author
//            {
//                get { return m_Author; }
//            }
//        }

//        public delegate void RelatedAuthorListHeadFootTemplate(RelatedAuthorListHeadFootParams _this);
//        public delegate void RelatedAuthorListItemTemplate(RelatedAuthorListItemParams _this, int i);
//        #endregion

//        #region 日志访问者
//        public class ArticleVisitorListHeadFootParams
//        {
//            private int? m_TotalVisitors;

//            public ArticleVisitorListHeadFootParams() { }

//            public ArticleVisitorListHeadFootParams(int? totalVisitors)
//            {
//                m_TotalVisitors = totalVisitors;
//            }

//            /// <summary>
//            /// 是否没有数据
//            /// </summary>
//            public bool NoData
//            {
//                get
//                {
//                    return m_TotalVisitors == null || m_TotalVisitors.Value <= 0;
//                }
//            }
//        }

//        public class ArticleVisitorListItemParams
//        {
//            private BlogArticleVisitor m_Visitor;

//            public ArticleVisitorListItemParams() { }

//            public ArticleVisitorListItemParams(BlogArticleVisitor visitor)
//            {
//                m_Visitor = visitor;
//            }

//            public BlogArticleVisitor Visitor
//            {
//                get { return m_Visitor; }
//            }
//        }

//        public delegate void ArticleVisitorListHeadFootTemplate(ArticleVisitorListHeadFootParams _this);
//        public delegate void ArticleVisitorListItemTemplate(ArticleVisitorListItemParams _this, int i);
//        #endregion

//        #endregion

//        #region 列表

//        #region 日志

//        /// <summary>
//        /// 仅用户自己可见的页面日志列表
//        /// </summary>
//        [TemplateTag]
//        public void BlogArticleList(
//              string view
//            , int categoryID
//            , int pageNumber
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , BlogArticleListHeadFootTemplate head
//            , BlogArticleListHeadFootTemplate foot
//            , BlogArticleListItemTemplate item)
//        {
//            #region 仅用户自己可见的页面日志列表

//            int? totalArticles = null;
//            int pageSize = AllSettings.Current.BlogSettings.BlogListPageSize;

//            BlogArticleOwnerType ownerType = StringUtil.TryParse<BlogArticleOwnerType>(view, BlogArticleOwnerType.Friend);
//            BlogArticleCollection articles = null;

//            switch (ownerType)
//            {
//                case BlogArticleOwnerType.Friend:
//                default:
//                    articles = BlogBO.Instance.GetArticlesByFriends(pageNumber, pageSize, ref totalArticles);

//                    break;

//                case BlogArticleOwnerType.Self:
//                    if (categoryID > 0)
//                    {
//                        articles = BlogBO.Instance.GetArticles(categoryID, pageNumber, pageSize, ref totalArticles);
//                    }
//                    else
//                    {
//                        articles = BlogBO.Instance.GetArticles(null ,pageNumber, pageSize, ref totalArticles);
//                    }

//                    break;

//                case BlogArticleOwnerType.Trace:
//                    articles = BlogBO.Instance.GetAriticlesOfUserTrace(pageNumber, pageSize, ref totalArticles);

//                    break;

//                case BlogArticleOwnerType.All:
//                    articles = BlogBO.Instance.GetMostArticles(pageNumber, pageSize, ref totalArticles);

//                    break;
//            }
//            #region 内容处理

//            m_RelatedAuthorList = new UserCollection();

//            BlogArticleListHeadFootParams headParams = new BlogArticleListHeadFootParams(view, totalArticles, pageSize);

//            head(headParams);

//            if (articles != null && articles.Count > 0)
//            {
//                int i = 0;

//                foreach (BlogArticle article in articles)
//                {
//                    #region 按作者查看的列表

//                    User articleAuthor = UserBO.Instance.GetUser(article.UserID);

//                    if (!m_RelatedAuthorList.Contains(articleAuthor))
//                    {
//                        m_RelatedAuthorList.Add(articleAuthor);
//                    }

//                    #endregion

//                    #region 输出列表模板

//                    BlogArticleListItemParams listParams = new BlogArticleListItemParams(article, m_CurrentUserID);

//                    item(listParams, i++);

//                    #endregion

//                }
//            }

//            BlogArticleListHeadFootParams footParams = new BlogArticleListHeadFootParams(string.Empty, totalArticles, pageSize);

//            foot(footParams);

//            #endregion

//            #endregion
//        }

//        /// <summary>
//        /// 用户个人主页上的日志列表
//        /// </summary>
//        /// <param name="userID">用户ID</param>
//        /// <param name="categoryID">日志分类ID,如果为0则不筛选分类</param>
//        [TemplateTag]
//        public void BlogArticleList(
//              int userID
//            , int categoryID
//            , int pageNumber
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , BlogArticleListHeadFootTemplate head
//            , BlogArticleListHeadFootTemplate foot
//            , BlogArticleListItemTemplate item)
//        {
//            #region 用户个人主页上的日志列表

//            int? totalArticles = null;
//            int pageSize =  AllSettings.Current.BlogSettings.BlogListPageSize;
//            BlogArticleCollection articles = null;

//            if (categoryID > 0)
//            {
//                articles = BlogBO.Instance.GetArticles(userID, categoryID, pageNumber, pageSize, ref totalArticles);
//            }
//            else
//            {
//                articles = BlogBO.Instance.GetArticles(userID, null, pageNumber, pageSize, ref totalArticles);
//            }

//            #region 内容处理

//            BlogArticleListHeadFootParams headParams = new BlogArticleListHeadFootParams(string.Empty, totalArticles, pageSize);

//            head(headParams);

//            if (articles != null && articles.Count > 0)
//            {
//                int i = 0;
                
//                foreach (BlogArticle article in articles)
//                {

//                    #region 输出列表模板

//                    BlogArticleListItemParams listParams = new BlogArticleListItemParams(article, userID <= 0 ? m_CurrentUserID : userID);

//                    item(listParams, i++);

//                    #endregion

//                }
//            }

//            BlogArticleListHeadFootParams footParams = new BlogArticleListHeadFootParams(string.Empty, totalArticles, pageSize);

//            foot(footParams);

//            #endregion

//            #endregion
//        }


//        /// <summary>
//        /// 使用了指定标签的日志列表
//        /// </summary>
//        [TemplateTag]
//        public void BlogArticleList(
//              int tagID
//            , int pageNumber
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , BlogArticleListHeadFootTemplate head
//            , BlogArticleListHeadFootTemplate foot
//            , BlogArticleListItemTemplate item)
//        {
//            #region 使用了指定标签的日志列表

//            int? totalArticles = null;
//            int pageSize = AllSettings.Current.BlogSettings.BlogListPageSize;
//            BlogArticleCollection articles = null;

//            articles = BlogBO.Instance.GetArticlesByTag(tagID, pageNumber, pageSize, ref totalArticles);

//            #region 内容处理

//            BlogArticleListHeadFootParams headParams = new BlogArticleListHeadFootParams(string.Empty, totalArticles, pageSize);

//            head(headParams);

//            if (articles != null && articles.Count > 0)
//            {
//                int i = 0;

//                bool hasManagePermission = BlogBO.Instance.ManagePermission.HasPermissionForSomeone(UserBO.Instance.GetUserID(), ManageSpacePermissionSet.ActionWithTarget.ManageBlog);

//                foreach (BlogArticle article in articles)
//                {

//                    #region 输出列表模板

//                    BlogArticleListItemParams listParams = new BlogArticleListItemParams(article, hasManagePermission);

//                    item(listParams, i++);

//                    #endregion

//                }
//            }

//            BlogArticleListHeadFootParams footParams = new BlogArticleListHeadFootParams(string.Empty, totalArticles, pageSize);

//            foot(footParams);

//            #endregion

//            #endregion
//        }

//        /// <summary>
//        /// 相似标签日志列表
//        /// </summary>
//        /// <param name="articleID">目标日志</param>
//        [TemplateTag]
//        public void BlogArticleSimilarList(
//              int articleID
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , BlogArticleListHeadFootTemplate head
//            , BlogArticleListHeadFootTemplate foot
//            , BlogArticleListItemTemplate item)
//        {
//            #region 相似标签日志列表

//            int? totalArticles = null;

//            BlogArticleCollection articles = BlogBO.Instance.GetSimilarArticles(articleID);

//            totalArticles = articles != null ? articles.Count : 0;

//            BlogArticleListHeadFootParams headFootParams = new BlogArticleListHeadFootParams(totalArticles);

//            head(headFootParams);

//            if (articles != null && articles.Count > 0)
//            {
//                int i = 0;
//                bool hasManagePermission = BlogBO.Instance.ManagePermission.HasPermissionForSomeone(UserBO.Instance.GetUserID(), ManageSpacePermissionSet.ActionWithTarget.ManageBlog);
//                foreach (BlogArticle article in articles)
//                {
//                    BlogArticleListItemParams listItemParams = new BlogArticleListItemParams(article,hasManagePermission);

//                    item(listItemParams, i++);
//                }
//            }

//            foot(headFootParams);

//            #endregion
//        }

//        /// <summary>
//        /// 随便看看首页日志列表
//        /// </summary>
//        [TemplateTag]
//        public void BlogArticleTopList(
//              int number
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , BlogArticleListHeadFootTemplate head
//            , BlogArticleListHeadFootTemplate foot
//            , BlogArticleListItemTemplate item)
//        {
//            #region 随便看看首页日志列表

//            int? totalArticles = null;

//            BlogArticleCollection articles = BlogBO.Instance.GetTopArticles(number);

//            totalArticles = articles != null ? articles.Count : 0;

//            BlogArticleListHeadFootParams headFootParams = new BlogArticleListHeadFootParams(totalArticles);

//            head(headFootParams);

//            if (articles != null && articles.Count > 0)
//            {
//                int i = 0;
//                bool hasManagePermission = BlogBO.Instance.ManagePermission.HasPermissionForSomeone(UserBO.Instance.GetUserID(), ManageSpacePermissionSet.ActionWithTarget.ManageBlog);
//                foreach (BlogArticle article in articles)
//                {
//                    BlogArticleListItemParams listItemParams = new BlogArticleListItemParams(article,hasManagePermission);

//                    item(listItemParams, i++);
//                }
//            }

//            foot(headFootParams);

//            #endregion
//        }

//        /// <summary>
//        /// 搜索页的日志列表
//        /// </summary>
//        /// <param name="pageType">页面类型,Admin表示是后台管理的搜索列表</param>
//        /// <param name="filter">搜索条件过滤器QueryString键名</param>
//        [TemplateTag]
//        public void BlogArticleSearchList(
//              string mode
//            , string filter
//            , int pageNumber
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , BlogArticleListHeadFootTemplate head
//            , BlogArticleListHeadFootTemplate foot
//            , BlogArticleListItemTemplate item)
//        {
//            #region 搜索页的日志列表

//            int? totalArticles = null;
//            int pageSize = AllSettings.Current.BlogSettings.BlogListPageSize;
//            BlogArticleCollection articles = null;
//            BlogArticleListHeadFootParams headFootParams;

//            #region 获取数据

//            switch (StringUtil.TryParse<BlogSearchPageMode>(mode, BlogSearchPageMode.Default))
//            {
//                case BlogSearchPageMode.Admin:

//                    AdminBlogFilter advBlogFilter = AdminBlogFilter.GetFromFilter(filter);

//                    articles = BlogBO.Instance.SearchArticlesByAdmin(UserBO.Instance.GetUserID(), advBlogFilter, pageNumber, ref totalArticles);
//                    pageSize = advBlogFilter.PageSize;
//                    headFootParams = new BlogArticleListHeadFootParams(string.Empty, totalArticles, pageSize, advBlogFilter);

//                    break;
//                case BlogSearchPageMode.Default:
//                default:

//                    BlogFilter blogFilter = BlogFilter.GetFromFilter(filter);

//                    articles = BlogBO.Instance.SearchArticles(blogFilter, pageNumber, pageSize, ref totalArticles);
//                    headFootParams = new BlogArticleListHeadFootParams(string.Empty, totalArticles, pageSize, blogFilter);


//                    break;
//            }

//            #endregion
            
//            head(headFootParams);

//            if (articles != null && articles.Count > 0)
//            {
//                int i = 0;
//                bool hasManagePermission = BlogBO.Instance.ManagePermission.HasPermissionForSomeone(UserBO.Instance.GetUserID(), ManageSpacePermissionSet.ActionWithTarget.ManageBlog);
//                foreach (BlogArticle article in articles)
//                {

//                    BlogArticleListItemParams listItemParams = new BlogArticleListItemParams(article,hasManagePermission);

//                    item(listItemParams, i++);

//                }
//            }

//            foot(headFootParams);

//            #endregion
//        }

//        #endregion

//        #region 日志分类

//        /// <summary>
//        /// 仅仅用户自己可见的日志分类列表,发表日志时供选择的分类列表
//        /// </summary>
//        [TemplateTag]
//        public void BlogCategoryList(
//              GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , BlogCategoryListHeadFootTemplate head
//            , BlogCategoryListHeadFootTemplate foot
//            , BlogCategoryListItemTemplate item)
//        {
//            #region 仅仅用户自己可见的列表,发表日志时供选择的分类列表

//            BlogCategoryCollection categories = BlogBO.Instance.GetBlogCategories(UserBO.Instance.GetUserID());
//            int totalCategories = categories == null ? 0 : categories.Count;

//            BlogCategoryListHeadFootParams headFootParams = new BlogCategoryListHeadFootParams();

//            head(headFootParams);

//            if (categories != null && categories.Count > 0)
//            {
//                int i = 0;
//                bool hasManagePermission = BlogBO.Instance.ManagePermission.HasPermissionForSomeone(UserBO.Instance.GetUserID(), ManageSpacePermissionSet.ActionWithTarget.ManageBlog);
//                foreach (BlogCategory category in categories)
//                {
//                    BlogCategoryListItemParams listItemParams = new BlogCategoryListItemParams(category, 0, hasManagePermission);

//                    item(listItemParams, i++);
//                }

//            }

//            foot(headFootParams);

//            #endregion
//        }

//        /// <summary>
//        /// 仅仅用户自己可见的日志分类列表
//        /// </summary>
//        [TemplateTag]
//        public void BlogCategoryList(
//              string view
//            , int selectedID
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , BlogCategoryListHeadFootTemplate head
//            , BlogCategoryListHeadFootTemplate foot
//            , BlogCategoryListItemTemplate item)
//        {
//            #region 仅仅用户自己可见的列表

//            //如果不是我的日志页面，不显示分类
//            if (string.Compare(view, "Self", true) != 0)
//            {
//                return;
//            }

//            BlogCategoryCollection categories = BlogBO.Instance.GetBlogCategories(UserBO.Instance.GetUserID());
//            int totalCategories = categories == null ? 0 : categories.Count;

//            BlogCategoryListHeadFootParams headFootParams = new BlogCategoryListHeadFootParams(totalCategories, selectedID);

//            head(headFootParams);

//            if (categories != null && categories.Count > 0)
//            {
//                int i = 0;
//                bool hasManagePermission = BlogBO.Instance.ManagePermission.HasPermissionForSomeone(UserBO.Instance.GetUserID(), ManageSpacePermissionSet.ActionWithTarget.ManageBlog);
//                foreach (BlogCategory category in categories)
//                {
//                    BlogCategoryListItemParams listItemParams = new BlogCategoryListItemParams(category, selectedID,hasManagePermission);

//                    item(listItemParams, i++);
//                }

//            }

//            foot(headFootParams);

//            #endregion
//        }

//        /// <summary>
//        /// 用户个人主页上的日志分类列表
//        /// </summary>
//        /// <param name="userID">用户ID</param>
//        [TemplateTag]
//        public void BlogCategoryList(
//              int userID
//            , int selectedID
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , BlogCategoryListHeadFootTemplate head
//            , BlogCategoryListHeadFootTemplate foot
//            , BlogCategoryListItemTemplate item)
//        {
//            #region 用户个人主页上的日志分类列表

//            BlogCategoryCollection categories = BlogBO.Instance.GetBlogCategories(userID);
//            int totalCategories = categories.Count;

//            BlogCategoryListHeadFootParams headFootParams = new BlogCategoryListHeadFootParams(totalCategories, selectedID);

//            head(headFootParams);

//            if (categories != null && categories.Count > 0)
//            {
                
//                int i = 0;
//                bool hasManagePermission = BlogBO.Instance.ManagePermission.HasPermissionForSomeone(UserBO.Instance.GetUserID(), ManageSpacePermissionSet.ActionWithTarget.ManageBlog);
//                foreach (BlogCategory category in categories)
//                {
//                    BlogCategoryListItemParams listItemParams = new BlogCategoryListItemParams(category, selectedID,hasManagePermission);

//                    item(listItemParams, i++);
//                }

//            }

//            foot(headFootParams);

//            #endregion
//        }

//        /// <summary>
//        /// 后台管理页面日志分类列表
//        /// </summary>
//        [TemplateTag]
//        public void BlogCategorySearchList(
//              string mode
//            , string filter
//            , int pageNumber
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , BlogCategoryListHeadFootTemplate head
//            , BlogCategoryListHeadFootTemplate foot
//            , BlogCategoryListItemTemplate item)
//        {
//            #region 后台管理页面日志分类列表

//            int? totalCategories = null;
//            int pageSize = Consts.DefaultPageSize;

//            BlogCategoryCollection categories = null;
//            BlogCategoryListHeadFootParams headFootParams;

//            #region 获取数据

//            AdminBlogCategoryFilter blogCategoryFilter = AdminBlogCategoryFilter.GetFromFilter(filter);

//            categories = BlogBO.Instance.GetBlogCategoriesBySearch(UserBO.Instance.GetUserID(), blogCategoryFilter, pageNumber, ref totalCategories);
//            pageSize = blogCategoryFilter.PageSize;
//            headFootParams = new BlogCategoryListHeadFootParams(totalCategories, pageSize, blogCategoryFilter);

//            #endregion

//            head(headFootParams);

//            if (categories != null && categories.Count > 0)
//            {
//                int i = 0;
//                bool hasManagePermission = BlogBO.Instance.ManagePermission.HasPermissionForSomeone(UserBO.Instance.GetUserID(), ManageSpacePermissionSet.ActionWithTarget.ManageBlog);
//                foreach (BlogCategory category in categories)
//                {
//                    BlogCategoryListItemParams listItemParams = new BlogCategoryListItemParams(category, 0, hasManagePermission);

//                    item(listItemParams, i++);
//                }
//            }

//            foot(headFootParams);

//            #endregion
//        }

//        #endregion

//        #region 作者列表

//        private UserCollection m_RelatedAuthorList = null;
//        /// <summary>
//        /// 相关作者列表，此列表必须依赖于日志列表先执行！否则取不到数据！
//        /// </summary>
//        [TemplateTag]
//        public void RelatedAuthorList(
//              string view
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , RelatedAuthorListHeadFootTemplate head
//            , RelatedAuthorListHeadFootTemplate foot
//            , RelatedAuthorListItemTemplate item)
//        {
//            #region 相关作者列表，此列表必须依赖于日志列表先执行！否则取不到数据！

//            //如果是自己的页面或是用户列表为空则不显示
//            if ((view != null && string.Compare(view, "Self", true) == 0) || m_RelatedAuthorList == null || m_RelatedAuthorList.Count <= 0)
//            {
//                return;
//            }

//            RelatedAuthorListHeadFootParams headFootParams = new RelatedAuthorListHeadFootParams(m_RelatedAuthorList.Count);

//            head(headFootParams);

//            if (m_RelatedAuthorList != null && m_RelatedAuthorList.Count > 0)
//            {

//                int i = 0;

//                foreach (User author in m_RelatedAuthorList)
//                {
//                    RelatedAuthorListItemParams listItemParams = new RelatedAuthorListItemParams(author);
//                    item(listItemParams, i++);
//                }
//            }

//            foot(headFootParams);
//            #endregion
//        }

//        #endregion

//        #region 访问者

//        private BlogArticleVisitorCollection m_ArticleVisitorList = null;
//        /// <summary>
//        /// 日志的最近访问者列表，必须依赖于单条日志先执行，否则会增加数据库查询次数
//        /// </summary>
//        /// <param name="articleID">日志ID</param>
//        [TemplateTag]
//        public void ArticleVisitorList(
//              int articleID
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , ArticleVisitorListHeadFootTemplate head
//            , ArticleVisitorListHeadFootTemplate foot
//            , ArticleVisitorListItemTemplate item
//            )
//        {
//            #region 日志的最近访问者列表

//            if (m_ArticleVisitorList == null)
//            {
//                m_ArticleVisitorList = BlogBO.Instance.GetArticleVisitors(articleID);
//            }

//            if (m_ArticleVisitorList == null || m_ArticleVisitorList.Count <= 0)
//                return;

//            ArticleVisitorListHeadFootParams headFootParams = new ArticleVisitorListHeadFootParams(m_ArticleVisitorList == null ? 0 : m_ArticleVisitorList.Count);
//            head(headFootParams);

//            if (m_ArticleVisitorList != null)
//            {
//                int i = 0;

//                foreach (BlogArticleVisitor visitor in m_ArticleVisitorList)
//                {
//                    ArticleVisitorListItemParams listItemParams = new ArticleVisitorListItemParams(visitor);
//                    item(listItemParams, i);
//                }

//            }

//            foot(headFootParams);

//            #endregion
//        }

//        #endregion

//        #endregion

//        #region 单条数据

//        /// <summary>
//        /// 单篇日志数据
//        /// </summary>
//        /// <param name="articleID">日志ID</param>
//        [TemplateTag]
//        public void BlogArticleView(
//              int articleID
//            , string action
//            , string password
//            , BlogArticleListItemTemplate item)
//        {
//            #region 单篇日志数据
//            BlogArticle article = null;
            
//            if (string.Compare(action, "View", true) == 0)
//            {
//                article = BlogBO.Instance.VisitArticle(articleID);
//            }
//            else
//            {
//                article = BlogBO.Instance.GetArticle(articleID);
//            }

//            if (article != null && string.Compare(action, "View", true) == 0)
//            {
//                //这里调用可以避免最近访问者列表的数据库查询
//                m_ArticleVisitorList = article.LastVisitors;
//            }

//            if (article == null) { article = new BlogArticle(); }

//            BlogArticleListItemParams listItemParams = new BlogArticleListItemParams(article, password, action);
//            item(listItemParams, 0);

//            #endregion
//        }

//        /// <summary>
//        /// 单篇日志数据
//        /// </summary>
//        /// <param name="articleID">日志ID</param>
//        [TemplateTag]
//        public void BlogArticleView(
//              int articleID
//            , string action
//            , BlogArticleListItemTemplate item)
//        {
//            #region 单篇日志数据
//            BlogArticle article = null;

//            if (string.Compare(action, "View", true) == 0)
//            {
//                article = BlogBO.Instance.VisitArticle(articleID);
//            }
//            else
//            {
//                article = BlogBO.Instance.GetArticle(articleID);
//            }

//            if (article != null && string.Compare(action, "View", true) == 0)
//            {
//                //这里调用可以避免最近访问者列表的数据库查询
//                m_ArticleVisitorList = article.LastVisitors;
//            }

//            if (article == null) { article = new BlogArticle(); }

//            BlogArticleListItemParams listItemParams = new BlogArticleListItemParams(article, m_CurrentUserID, action);
//            item(listItemParams, 0);

//            #endregion
//        }

//        #endregion

//    }
//}