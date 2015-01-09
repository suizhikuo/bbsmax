//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.App_Blog
{
    public partial class write : CenterBlogPageBase
    {
        protected override string PageTitle
        {
            get
            {
                if (IsEditMode)
                    return string.Concat("编辑日志", " - ", base.PageTitle);
                else
                    return string.Concat("发表新日志", " - ", base.PageTitle);
            }
        }

        public override bool SelectedFriend
        {
            get
            {
                return string.Compare(_Request.Get("view", Method.Get, ""), "friend", false) == 0;
            }
        }

        protected override string PageName
        {
            get { return "blog"; }
        }

		protected override void OnLoadComplete(EventArgs e)
		{
            if (AllSettings.Current.SpacePermissionSet.Can(My, SpacePermissionSet.Action.UseBlog) == false)
            {
                ShowError("您所在的用户组没有发表日志的权限");
            }

            if (My.Roles.IsInRole(Role.FullSiteBannedUsers))
                ShowError("您已经被整站屏蔽不能发表日志");

			if (_Request.IsClick("save"))
			{
				#region 页面提交时

				int id = _Request.Get<int>("id", 0);
				int? categoryID = _Request.Get<int>("category");

				string subject = _Request.Get("subject");
				string content = _Request.Get("content", Method.Post, string.Empty, false);
				string password = _Request.Get("password");
				string tagNames = _Request.Get("tag", Method.Post, string.Empty);
				string currentIP = _Request.IpAddress;

				bool enableComment = _Request.IsChecked("enablecomment", Method.Post, true);

				PrivacyType privacyType = _Request.Get<PrivacyType>("privacytype", PrivacyType.SelfVisible);

				using (ErrorScope es = new ErrorScope())
				{
					MessageDisplay md = CreateMessageDisplay(GetValidateCodeInputName("CreateBlogArticle"));

					if (CheckValidateCode("CreateBlogArticle", md))
					{
						bool succeed = false;

                        bool useHtml = _Request.Get("format") == "html";
                        bool useUbb = _Request.Get("format") == "ubb";

						if (IsEditMode)
						{
							succeed = BlogBO.Instance.UpdateBlogArticle(MyUserID, currentIP, id, subject, content, categoryID, tagNames.Split(','), enableComment, privacyType, password, useHtml, useUbb);
						}
						else
						{
							succeed = BlogBO.Instance.CreateBlogArticle(MyUserID, currentIP, subject, content, categoryID, tagNames.Split(','), enableComment, privacyType, password, useHtml, useUbb, out id);
						}

						if (succeed)
						{
							ValidateCodeManager.CreateValidateCodeActionRecode("CreateBlogArticle");

							BbsRouter.JumpTo("app/blog/index");
						}
						else
						{
							if (es.HasError)
							{
								es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
								{
									md.AddError(error);
								});
							}
						}
					}
				}

				#endregion
            }
            else if (_Request.IsClick("addcategory"))
            {
                AddCategory();
            }
            

				#region 正常页面加载

				if (IsEditMode)
				{
					using (ErrorScope es = new ErrorScope())
					{
						int? articleID = _Request.Get<int>("id");

						if (articleID.HasValue)
						{
							m_Article = BlogBO.Instance.GetBlogArticleForEdit(MyUserID, articleID.Value);

							if (m_Article != null)
							{
								string[] tagNames = new string[m_Article.Tags.Count];

								for (int i = 0; i < tagNames.Length; i++ )
								{
									tagNames[i] = m_Article.Tags[i].Name;
								}

								m_ArticleTagList = StringUtil.Join(tagNames);

								m_CategoryList = BlogBO.Instance.GetUserBlogCategories(m_Article.UserID);
							}
						}

						if (m_Article == null)
						{
							ShowError("日志不存在");
						}

						if (es.HasUnCatchedError)
						{
							es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
							{
								ShowError(error);
							});
						}

						base.OnLoadComplete(e);
					}

                    AddNavigationItem(FunctionName, BbsRouter.GetUrl("app/blog/index"));
                    AddNavigationItem("编辑日志");
				}
				else
				{
					m_Article = new BlogArticle();

					m_ArticleTagList = string.Empty;

					m_CategoryList = BlogBO.Instance.GetUserBlogCategories(MyUserID);

                    AddNavigationItem(FunctionName, BbsRouter.GetUrl("app/blog/index"));
                    AddNavigationItem("发表新日志");
				}


                m_ArticleList = BlogBO.Instance.GetUserBlogArticles(MyUserID, MyUserID, null, null, 1, 5);
				#endregion
        }

        private void AddCategory()
        {
            using (new ErrorScope())
            {
                string categoryName = _Request.Get("catname", Method.Post);

                int newCategoryId;

                BlogBO.Instance.CreateBlogCategory(MyUserID, categoryName, out newCategoryId);

                _Request.Modify("category", newCategoryId.ToString());
            }
        }

        public bool CanUseHtml
        {
            get { return AllSettings.Current.BlogSettings.AllowHtml.GetValue(My); }
        }

        public bool CanUseUbb
        {
            get { return AllSettings.Current.BlogSettings.AllowUbb.GetValue(My); }
        }

		private BlogArticle m_Article;

		/// <summary>
		/// 当前编辑的日志
		/// </summary>
		public BlogArticle Article
		{
			get { return m_Article; }
        }

        private BlogArticleCollection m_ArticleList;

        public BlogArticleCollection ArticleList
        {
            get { return m_ArticleList; }
        }

		/// <summary>
		/// 当前操作是否是编辑，不是编辑就是新建
		/// </summary>
		public bool IsEditMode
		{
			get { return _Request.Get("id") != null; }
		}

		private string m_ArticleTagList;

		/// <summary>
		/// 当前日志的标签列表
		/// </summary>
		public string ArticleTagList
		{
			get { return m_ArticleTagList; }
		}

		private BlogCategoryCollection m_CategoryList;

		/// <summary>
		/// 日志作者的日志分类
		/// </summary>
		public BlogCategoryCollection CategoryList
		{
			get { return m_CategoryList; }
		}

		//==========================================================分割线以上是整理过的，分割线一下是未整理过的===================================================================

        private int m_AlbumID = -1;
        private string m_PhotoName = string.Empty;
        private string m_PhotoDescription = string.Empty;
        private int m_NewPhotoID = -1;


        private void SetPermission()
        {
            //TODO:
            BlogArticle article = BlogBO.Instance.GetBlogArticleForEdit(MyUserID, _Request.Get<int>("id", Method.Get, 0));

			if (article == null)
			{
				m_HasEditPermission = false;
				m_IsEditedByAdmin = false;
			}
			else
			{
				m_HasEditPermission = true;

				m_IsEditedByAdmin = article.UserID != MyUserID;
			}
        }

        protected bool? m_IsEditedByAdmin;
        protected bool IsEditedByAdmin
        {
            get
            {
                if (m_IsEditedByAdmin == null)
                {
                    SetPermission();
                }
                return m_IsEditedByAdmin.Value;
            }
        }

        private bool? m_HasEditPermission;
        protected bool HasEditPermission
        {
            get
            {
                if (m_HasEditPermission == null)
                {
                    SetPermission();
                }
                return m_HasEditPermission.Value;
            }
        }
    }
}