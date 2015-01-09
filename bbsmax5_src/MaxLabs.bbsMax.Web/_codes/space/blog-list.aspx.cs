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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
    public partial class blog_list : SpaceBlogPageBase
    {
		protected override void OnLoadComplete(EventArgs e)
		{
			if (_Request.IsClick("passwordsubmit"))
			{
				int articleID = _Request.Get<int>("id", Method.Post, 0);

				BlogBO.Instance.SaveBlogArticlePassword(MyUserID, articleID, _Request.Get("password", Method.Post));

				BbsRouter.JumpTo("space/" + SpaceOwnerID + "/blog/article-" + articleID);
			}

			using (ErrorScope es = new ErrorScope())
			{
				m_CategoryID = _Request.Get<int>("cat");

				m_TagID = _Request.Get<int>("tag");

				int pageNumber = _Request.Get<int>("page", 0);

				m_ArticleList = BlogBO.Instance.GetUserBlogArticles(MyUserID, SpaceOwnerID, m_CategoryID, m_TagID, pageNumber, ArticleListPageSize);

				if (es.HasUnCatchedError)
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
					{
						ShowError(error);
					});
				}
				else
				{
					m_TotalArticleCount = m_ArticleList.TotalRecords;

					m_CategoryList = BlogBO.Instance.GetUserBlogCategories(SpaceOwnerID);

					UserBO.Instance.WaitForFillSimpleUsers<BlogArticle>(m_ArticleList);
				}
			}

			base.OnLoadComplete(e);
		}

        public override string SpaceTitle
        {
            get
            {
                return SpaceOwner.Name + "的日志";
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
    }
}