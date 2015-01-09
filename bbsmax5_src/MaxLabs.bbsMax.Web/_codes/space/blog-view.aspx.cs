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
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
    public partial class blog_view : SpaceBlogPageBase
    {
		protected override void OnLoadComplete(EventArgs e)
		{
			using (ErrorScope es = new ErrorScope())
			{
				int? articleID = _Request.Get<int>("id");
				int pageNumber = _Request.Get<int>("page", 1);

				string password = _Request.Get("password");

				m_CommentListPageSize = Consts.DefaultPageSize;

				if (articleID.HasValue)
				{
					m_Article = BlogBO.Instance.GetBlogArticleForVisit(MyUserID, articleID.Value, password);

					if (m_Article != null)
					{
						m_ArticleTagList = m_Article.Tags;

						m_ArticleVisitorList = m_Article.LastVisitors;

						m_CommentTargetID = m_Article.ArticleID;

						if (_Request.IsClick("addcomment"))
							AddComment("space/" + SpaceOwnerID + "/blog/article-" + articleID,"#comment");

						if (m_ArticleVisitorList != null)
							UserBO.Instance.WaitForFillSimpleUsers<BlogArticleVisitor>(m_ArticleVisitorList);

						m_CommentList = CommentBO.Instance.GetComments(articleID.Value, CommentType.Blog, pageNumber, m_CommentListPageSize, true, out m_TotalCommentCount);

						if (m_CommentList != null)
							UserBO.Instance.WaitForFillSimpleUsers<Comment>(m_CommentList);

						m_ArticleSimilarList = BlogBO.Instance.GetSimilarArticles(MyUserID, m_Article.UserID, articleID.Value, 10);
					}
					else
					{
						es.CatchError<NoPermissionVisitBlogArticleBeacuseNeedPasswordError>(
							delegate(NoPermissionVisitBlogArticleBeacuseNeedPasswordError error)
							{

								m_DisplayPasswordForm = true;
							}
						);

						if(m_DisplayPasswordForm == false)
							ShowError("所访问的日志不存在。");
					}
				}
				else
				{
					ShowError("所访问的日志不存在。");
				}
			}

			base.OnLoadComplete(e);
		}

        public override string SpaceTitle
        {
            get
            {
                if (m_Article != null && this.Article.CanVisit)
                    return m_Article.Subject + " - " + SpaceOwner.Name + "的日志";

                return SpaceOwner.Name + "的日志";
            }
        }

		//protected override void ProcessActionRequest()
		//{
		//    if (_Request.IsClick("sendcomment"))
		//    {
		//        SendComment();
		//    }
		//}

		//private void SendComment()
		//{
		//    int? targetID = _Request.Get<int>("targetid", Method.Post);
		//    CommentType type =  CommentType.Blog;
		//    string content = _Request.Get("content", Method.Post,string.Empty,false);
		//    int userID = UserBO.Instance.GetUserID();

		//    using (new ErrorScope())
		//    {
		//        MessageDisplay msgDisplay = new MessageDisplay();

		//        if (targetID == null)
		//        {
		//            msgDisplay.AddError(new InvalidParamError("targetID").Message);
		//            return;
		//        }

		//        try
		//        {
		//            int newCommentId;
		//            bool success = CommentBO.Instance.AddComment(userID,targetID.Value, 0, type, content, IPUtil.GetCurrentIP(), out newCommentId);
		//            if (!success)
		//            {
		//                CatchError<ErrorInfo>(delegate(ErrorInfo error)
		//                {
		//                    msgDisplay.AddError(error);
		//                });
		//            }
		//            else
		//            {
		//                _Request.Remove("content",  Method.Post);
		//            }
		//        }
		//        catch (Exception ex)
		//        {
		//            msgDisplay.AddError(ex.Message);
		//        }
		//    }
		//}

		private bool m_DisplayPasswordForm;

		public bool DisplayPasswordForm
		{
			get { return m_DisplayPasswordForm; }
		}

		private BlogArticle m_Article;

		/// <summary>
		/// 当前阅读的文章
		/// </summary>
		public BlogArticle Article
		{
			get { return m_Article; }
		}

		private TagCollection m_ArticleTagList;

		/// <summary>
		/// 当前文章的Tag列表
		/// </summary>
		public TagCollection ArticleTagList
		{
			get { return this.m_ArticleTagList; }
		}

		private BlogArticleVisitorCollection m_ArticleVisitorList;

		public BlogArticleVisitorCollection ArticleVisitorList
		{
			get { return m_ArticleVisitorList; }
		}

		private CommentCollection m_CommentList;

		public CommentCollection CommentList
		{
			get { return m_CommentList; }
		}

		private int m_CommentListPageSize;

		public int CommentListPageSize
		{
			get { return m_CommentListPageSize; }
		}

		private int m_TotalCommentCount;

		public int TotalCommentCount
		{
			get { return m_TotalCommentCount; }
		}

		private int m_CommentTargetID;

		public override int CommentTargetID
		{
			get
			{
				return m_CommentTargetID;
			}
		}

		protected override CommentType CommentType
		{
			get
			{
				return CommentType.Blog;
			}
		}

		private BlogArticleCollection m_ArticleSimilarList;

		public BlogArticleCollection ArticleSimilarList
		{
			get { return m_ArticleSimilarList; }
		}
	}
}