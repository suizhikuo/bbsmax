//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Providers;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Ubb;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax
{
	/// <summary>
	/// 博客的业务逻辑
	/// </summary>
    public class BlogBO : SpaceAppBO<BlogBO>
    {
		protected override SpacePermissionSet.Action UseAction
		{
			get { return SpacePermissionSet.Action.UseBlog; }
		}

        protected override BackendPermissions.ActionWithTarget ManageAction
		{
			get { return BackendPermissions.ActionWithTarget.Manage_Blog; }
		}

		#region =========↓日志↓====================================================================================================

		public void SaveBlogArticlePassword(int operatorID, int articleID, string password)
		{
			AuthUser user = UserBO.Instance.GetAuthUser(operatorID);

			if (user == null)
				return;

			string newPwd = SecurityUtil.Encrypt(EncryptFormat.bbsMax, password);

			user.TempDataBox.SetData(GetPasswordBoxKey(articleID), newPwd);
		}

		/// <summary>
		/// 发布日志
		/// </summary>
		/// <param name="creatorID">创建者ID</param>
		/// <param name="creatorIP">创建者IP</param>
		/// <param name="subject">日志标题</param>
		/// <param name="content">日志内容</param>
		/// <param name="categoryID">日志分类ID</param>
		/// <param name="tagNames">日志的Tag集合</param>
		/// <param name="enableComment">日志是否允许评论</param>
		/// <param name="privacyType">日志的隐私设置</param>
		/// <param name="password">日志隐私设置为密码保护时，需要提供访问密码</param>
		/// <param name="newArticleID">新日志记录的ID</param>
		/// <returns>发布成功返回true，因为任何原因发布失败则返回false，可以通过ErrorScope捕获具体错误信息</returns>
		public bool CreateBlogArticle(int operatorID, string operatorIP, string subject, string content, int? categoryID, IEnumerable<string> tagNames, bool enableComment, PrivacyType privacyType, string password, bool useHtml, bool useUbb, out int newArticleID)
		{
			newArticleID = -1;

			bool validated = ValidateBlogUsePermission(operatorID) && ValidateBlogArticleSubject(subject) && ValidateBlogArticleContent(content);

			if (privacyType == PrivacyType.NeedPassword)
				validated = validated && ValidateBlogArticlePassword(password);

			if (validated == false)
				return false;

            useHtml = AllSettings.Current.BlogSettings.AllowHtml.GetValue(operatorID) && useHtml;
            useUbb = AllSettings.Current.BlogSettings.AllowUbb.GetValue(operatorID) && useUbb;

			content = BlogArticleUbbParser.ParseForSave(
                operatorID,
				content, 
				useHtml, 
				useUbb
			);

			if (HasUnCatchedError)
				return false;

			bool isApproved = true;

			string thumbnail = GetBlogArticleThumbnail(content);

			int tempArticleID = 0;

			BlogPointType pointType = BlogPointType.PostArticle;

			bool success = BlogPointAction.Instance.UpdateUserPoint(operatorID, pointType, delegate(PointActionManager.TryUpdateUserPointState state)
			{
				if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
					return false;

				tempArticleID = BlogDao.Instance.CreateBlogArticle(operatorID, operatorIP, subject, content, thumbnail, categoryID, enableComment, privacyType, EncryptArticlePassword(password), isApproved);

				if (tempArticleID <= 0)
					return false;

				return true;
			});

			if (success == false)
				return false;

			newArticleID = tempArticleID;

			TagBO.Instance.SaveTags(tagNames, TagType.Blog, newArticleID, operatorID);

			User operatorUser = UserBO.Instance.GetUser(operatorID);

			PrivacyType feedPrivacyType = GetPrivacyTypeForFeed(operatorUser.SpacePrivacy, operatorUser.BlogPrivacy, privacyType);

            KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            subject = keyword.Replace(subject);
            content = keyword.Replace(content);

            FeedBO.Instance.CreateWriteArticleFeed(operatorID, newArticleID, feedPrivacyType, null, thumbnail, subject, content);

			ClearCachedUserData(operatorID);

			ClearCachedEveryoneData();

			return true;
		}

		/// <summary>
		/// 编辑日志
		/// </summary>
		/// <param name="categoryID">日志分类</param>
		/// <param name="isApproved">是否已经审核</param>
		/// <param name="enableComment">是否允许评论</param>
		/// <param name="privacyType">隐私类型</param>
		/// <param name="createIP">写日志的用户的IP</param>
		/// <param name="thumbnail">日志缩略图</param>
		/// <param name="subject">日志标题</param>
		/// <param name="password">需要凭密码查看时的密码</param>
		/// <param name="content">正文内容</param>
		public bool UpdateBlogArticle(int operatorID, string operatorIP, int articleID, string subject, string content, int? categoryID, IEnumerable<string> tagNames, bool enableComment, PrivacyType privacyType, string password, bool useHtml, bool useUbb)
		{
			BlogArticle article = GetBlogArticleForEdit(operatorID, articleID);

			if (article == null)
				return false;

			bool validated = ValidateBlogArticleSubject(subject) && ValidateBlogArticleContent(content);

			if (privacyType == PrivacyType.NeedPassword)
				validated = validated && ValidateBlogArticlePassword(password);

			if (validated == false)
				return false;

			bool isApproved = true;

            useHtml = AllSettings.Current.BlogSettings.AllowHtml.GetValue(operatorID) && useHtml;
            useUbb = AllSettings.Current.BlogSettings.AllowUbb.GetValue(operatorID) && useUbb;

            content = BlogArticleUbbParser.ParseForSave(
                operatorID,
                content,
                useHtml,
                useUbb
            );

			string thumbnail = GetBlogArticleThumbnail(content);

			bool updated = BlogDao.Instance.UpdateBlogArticle(operatorID, operatorIP, articleID, subject, content, thumbnail, categoryID, enableComment, privacyType, EncryptArticlePassword(password), isApproved);

			if (updated)
			{
				TagBO.Instance.SaveTags(tagNames, TagType.Blog, articleID, operatorID);

				if (privacyType != article.PrivacyType)
					FeedBO.Instance.UpdateFeedPrivacyType(AppActionType.WriteArticle, articleID, privacyType, null);

				ClearCachedEveryoneData();

				User operatorUser = UserBO.Instance.GetUser(operatorID);

				PrivacyType feedPrivacyType = GetPrivacyTypeForFeed(operatorUser.SpacePrivacy, operatorUser.BlogPrivacy, privacyType);

				FeedBO.Instance.UpdateFeedPrivacyType(AppActionType.WriteArticle, articleID, feedPrivacyType, null);
			}

			return true;
		}

		/// <summary>
		/// 获取日志内容中的第一张图
		/// </summary>
		/// <param name="content">日志内容</param>
		/// <returns></returns>
		private string GetBlogArticleThumbnail(string content)
		{
			Regex imageRegex = new ImageRegex();

			MatchCollection imageMatches = imageRegex.Matches(content);

			if (imageMatches.Count > 0)
				return imageMatches[0].Groups[1].Value;

			return string.Empty;
		}

		public bool ExistsBlogArticle(int articleID)
		{
			return BlogDao.Instance.ExistsBlogArticle(articleID);
		}

		/**************************************
		 *       Get开头的函数获取数据        *
		 **************************************/

		/// <summary>
		/// 获取某用户在某个日期以后发表过多少篇日志
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="beginDate">起始时间</param>
		/// <returns></returns>
		public int GetPostBlogArticleCount(int userID, DateTime beginDate, DateTime endDate)
		{
			return BlogDao.Instance.GetPostBlogArticleCount(userID, beginDate, endDate);
		}

		/// <summary>
		/// 获取某个用户某个日期以后对某篇日志的评论数量
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="articleID"></param>
		/// <returns></returns>
		public int GetCommentCountForArticle(int userID, int articleID, DateTime beginDate, DateTime endDate)
		{
			return BlogDao.Instance.GetCommentCountForArticle(userID, articleID, beginDate, endDate);
		}

		/// <summary>
		/// 获取某个用户某个日期以后对另一个用户的博客日志的评论数量
		/// </summary>
		/// <param name="userID">评论者ID</param>
		/// <param name="targetUserID">被评论者的ID</param>
		/// <returns></returns>
		public int GetCommentCountForUser(int userID, int targetUserID, DateTime beginDate, DateTime endDate)
		{
			return BlogDao.Instance.GetCommentCountForUser(userID, targetUserID, beginDate, endDate);
		}

		/// <summary>
		/// 获取指定用户的日志（只返回浏览者可以查看的数据）
		/// </summary>
		/// <param name="visitorID">浏览者ID</param>
		/// <param name="articleOwnerID">日志所有者的ID</param>
		/// <param name="categoryID">日志所属分类的ID，传NULL则不以分类作为条件</param>
		/// <param name="tagID">日志所含标签的ID，传null则不以分类作为条件</param>
		/// <param name="pageNumber">指定数据的分页页码，分页页码从1开始</param>
		/// <param name="pageSize">指定数据的每页数据条数，必须是大于0的正整数</param>
		/// <param name="totalCount">总的数据条数（注意：不是当前返回集合的条数），用于分页用途</param>
		public BlogArticleCollection GetUserBlogArticles(int visitorID, int articleOwnerID, int? categoryID, int? tagID, int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			if (ValidateUserID(visitorID) == false || ValidateUserID(articleOwnerID) == false)
				return null;

			DataAccessLevel dataAccessLevel;

            if (AllSettings.Current.BackendPermissions.Can(visitorID, BackendPermissions.ActionWithTarget.Manage_Blog, articleOwnerID))
                dataAccessLevel = DataAccessLevel.DataOwner;
            else
                dataAccessLevel = GetDataAccessLevel(visitorID, articleOwnerID);

			#region 获取TotalCount缓存

			int? totalCount = null;

			string totalCountCacheKey = GeCacheKeyForUserBlogArticlesTotalCount(articleOwnerID, categoryID, tagID, dataAccessLevel);

			bool totalCountCached = CacheUtil.TryGetValue(totalCountCacheKey, out totalCount);

			#endregion

			BlogArticleCollection articles = BlogDao.Instance.GetUserBlogArticles(articleOwnerID, categoryID, tagID, dataAccessLevel, pageNumber, pageSize, ref totalCount);

			#region 设置TotalCount缓存

			if (totalCountCached == false)
				CacheUtil.Set(totalCountCacheKey, totalCount);

			#endregion

			ProcessKeyword(articles, ProcessKeywordMode.TryUpdateKeyword);

			return articles;
		}

		/// <summary>
		/// 获取操作者好友的相册
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="pageNumber">数据分页当前页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		public BlogArticleCollection GetFriendBlogArticles(int operatorID, int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			if (ValidateUserID(operatorID) == false)
				return null;

			BlogArticleCollection articles = BlogDao.Instance.GetFriendBlogArticles(operatorID, pageNumber, pageSize);

			ProcessKeyword(articles, ProcessKeywordMode.TryUpdateKeyword);

			return articles;
		}

		/// <summary>
		/// 获取“大家的日志”
		/// </summary>
		/// <param name="pageNumber">数据分页页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <returns></returns>
		public BlogArticleCollection GetEveryoneBlogArticles(int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			BlogArticleCollection articles = null;

			#region 获取Articles缓存

			string articlesCacheKey = GetCacheKeyForEveryoneBlogArticles(pageNumber, pageSize);

			bool articlesNeedCache = pageNumber <= Consts.ListCachePageCount;

			bool articlesCached = articlesNeedCache && CacheUtil.TryGetValue(articlesCacheKey, out articles);

			if (articlesCached)
				return articles;

			#endregion

			#region 获取TotalCount缓存

			int? totalCount = null;

			string totalCountCacheKey = GetCacheKeyForEveryoneBlogArticlesTotalCount();

			bool totalCountCached = CacheUtil.TryGetValue(articlesCacheKey, out totalCount);

			#endregion

			articles = BlogDao.Instance.GetEveryoneBlogArticles(pageNumber, pageSize, ref totalCount);

			#region 设置TotalCount缓存

			if (totalCountCached == false)
				CacheUtil.Set(articlesCacheKey, totalCount);

			#endregion

			#region 设置Articles缓存

			if (articlesNeedCache)
				CacheUtil.Set(articlesCacheKey, articles);

			#endregion

			ProcessKeyword(articles, ProcessKeywordMode.TryUpdateKeyword);

			return articles;
		}

		/// <summary>
		/// 获取用户访问过的日志
		/// </summary>
		/// <param name="visitorID">访问者ID</param>
		/// <param name="pageNumber">数据分页页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <returns></returns>
		public BlogArticleCollection GetVisitedBlogArticles(int visitorID, int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			BlogArticleCollection articles = BlogDao.Instance.GetVisitedBlogArticles(visitorID, pageNumber, pageSize);

			ProcessKeyword(articles, ProcessKeywordMode.TryUpdateKeyword);

			return articles;
		}

		/// <summary>
		/// 获取类似标签的日志
		/// </summary>
		/// <param name="articleID">指定日志ID</param>
		public BlogArticleCollection GetSimilarArticles(int visitorID, int articleOwnerID, int articleID, int number)
		{
			if (ValidateUserID(visitorID) == false || ValidateUserID(articleOwnerID) == false)
				return null;

			DataAccessLevel dataAccessLevel = GetDataAccessLevel(visitorID, articleOwnerID);

			BlogArticleCollection articles = BlogDao.Instance.GetSimilarArticles(articleID, number, dataAccessLevel);

			return articles;
		}

		/// <summary>
		/// 获取日志用于管理
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="filter">文章搜索条件</param>
		/// <param name="pageNumber">数据分页页码</param>
		/// <returns></returns>
		public BlogArticleCollection GetBlogArticlesForAdmin(int operatorID, AdminBlogArticleFilter filter, int pageNumber)
		{
			if (ValidateBlogAdminPermission(operatorID) == false)
				return null;

            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

			BlogArticleCollection articles = BlogDao.Instance.GetBlogArticlesBySearch(operatorID, excludeRoleIDs, filter, pageNumber);

			ProcessKeyword(articles, ProcessKeywordMode.FillOriginalText);

			return articles;
		}

		public BlogArticle GetBlogArticleForDelete(int operatorID, int articleID)
		{
			bool validated = this.ValidateUserID(operatorID) && this.ValidateBlogArticleID(articleID);

			if (validated == false)
				return null;

			BlogArticle article = BlogDao.Instance.GetBlogArticle(articleID);

			if (ValidateBlogArticleDeletePermission(operatorID, article) == false)
				return null;

			return article;
		}

		/// <summary>
		/// 获取日志（会判断操作者是否具有编辑权限）
		/// </summary>
		/// <param name="editorID">操作者ID</param>
		/// <param name="articleID">日志ID</param>
		/// <returns></returns>
		public BlogArticle GetBlogArticleForEdit(int editorID, int articleID)
		{
			bool validated = this.ValidateUserID(editorID) && this.ValidateBlogArticleID(articleID);

			if (validated == false)
				return null;

			BlogArticle article = BlogDao.Instance.GetBlogArticle(articleID);

			if (ValidateBlogArticleEditPermission(editorID, article) == false)
				return null;

			ProcessKeyword(article, ProcessKeywordMode.FillOriginalText);

			article.SetOriginalText2(BlogArticleUbbParser.ParseForEdit(
                editorID,
				article.OriginalContent, 
				AllSettings.Current.BlogSettings.AllowHtml.GetValue(editorID), 
				AllSettings.Current.BlogSettings.AllowUbb.GetValue(editorID)
			));

            article.Content = ProcessArticleContent(article.Content);

			return article;
		}


        private string ProcessArticleContent(string content)
        {
            content = content.Replace("{$root}",Globals.AppRoot);
            return content;
        }

		/// <summary>
		/// 获取日志（会判断操作者是否具有浏览权限）
		/// </summary>
		/// <param name="visitorID">操作者ID</param>
		/// <param name="articleID">日志ID</param>
		/// <param name="password">日志的访问密码</param>
		/// <returns>对应的日志</returns>
		public BlogArticle GetBlogArticleForVisit(int visitorID, int articleID, string password)
		{
			bool validated = this.ValidateUserID(visitorID) && this.ValidateBlogArticleID(articleID);

			if (validated == false)
				return null;

			BlogArticle article = BlogDao.Instance.GetBlogArticle(articleID);

			if (ValidateBlogArticleVisitPermission(visitorID, password, article))
			{
				string blogVisitorKey = GetCacheKeyForBlogArticleVisitor(visitorID, IPUtil.GetCurrentIP(), articleID);

				AuthUser user = UserBO.Instance.GetAuthUser(visitorID);

				if (user == null)
					return null;

				DateTime lastViewTime = DateTime.MinValue;

				if (user.TempDataBox.GetData(blogVisitorKey) != null)
				{
					lastViewTime = DateTime.Parse(user.TempDataBox.GetData(blogVisitorKey));
				}

				//在一定时间间隔内不重复计算访问次数
				if ((DateTimeUtil.Now - lastViewTime).TotalSeconds > Consts.BlogArticle_VisitorTimeScope)
				{
                    user.TempDataBox.SetData(blogVisitorKey, DateTimeUtil.Now.ToString());

					if(user.UserID != 0)
						BlogDao.Instance.VisitArticle(visitorID, articleID);
				}

				ProcessKeyword(article, ProcessKeywordMode.TryUpdateKeyword);

                article.Content = ProcessArticleContent(article.Content);

				return article;
			}

			return null;
		}

		/**************************************
		 *      Delete开头的函数删除数据      *
		 **************************************/

		/// <summary>
		/// 删除指定的日志
		/// </summary>
		/// <param name="operatorID"></param>
		/// <param name="articleID"></param>
		/// <param name="isUpdatePoint"></param>
		/// <returns></returns>
		public bool DeleteBlogArticle(int operatorID, int articleID, bool isUpdatePoint)
		{
			bool validated = this.ValidateUserID(operatorID) && this.ValidateBlogArticleID(articleID);

			if (validated == false)
				return false;

			BlogArticle article = BlogDao.Instance.GetBlogArticle(articleID);

			if (ValidateBlogArticleDeletePermission(operatorID, article) == false)
				return false;

			bool result = DeleteBlogArticlesInner(operatorID, new int[] { articleID }, isUpdatePoint, false);

			if (result)
			{
                FeedBO.Instance.DeleteFeed(AppActionType.WriteArticle, articleID);

				Logs.LogManager.LogOperation(new Logs.Blog_DeleteBlogArticle(
					operatorID
					, UserBO.Instance.GetUser(operatorID).Name
					, IPUtil.GetCurrentIP()
					, articleID
					, article.UserID
					, UserBO.Instance.GetUser(article.UserID).Name
					, article.Subject
				));
			}
			return result;
		}

		/// <summary>
		/// 删除指定的多篇日志
		/// </summary>
		/// <param name="articleIDs">日志ID集</param>
		/// <param name="isUpdatePoint">是否更新积分</param>
		public bool DeleteBlogArticles(int operatorID, int[] articleIDs, bool isUpdatePoint)
		{
			return DeleteBlogArticlesInner(operatorID, articleIDs, isUpdatePoint, true);
		}

		private bool DeleteBlogArticlesInner(int operatorID, int[] articleIDs, bool isUpdatePoint, bool log)
		{
			bool result = ProcessDeleteBlogArticles(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs)
			{
				return BlogDao.Instance.DeleteBlogArticles(articleIDs, operatorID, excludeRoleIDs);
			});

			if (result && log)
			{
				Logs.LogManager.LogOperation(
					new Blog_DeleteBlogArticleByIDs(operatorID, UserBO.Instance.GetUser(operatorID).Name, IPUtil.GetCurrentIP(), articleIDs)
				);
			}

			return result;
		}

		/// <summary>
		/// 删除博客文章用于管理
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="filter">文章搜索条件，符合条件的都将被删除</param>
		/// <param name="isUpdatePoint">是否更新积分</param>
		/// <param name="deleteTopCount">指定删除前多少条数据</param>
		/// <param name="deletedCount">真实删除的数据条数</param>
		/// <returns>操作成功返回true，否则返回false</returns>
		public bool DeleteBlogArticlesForAdmin(int operatorID, AdminBlogArticleFilter filter, bool isUpdatePoint, int deleteTopCount, out int deletedCount)
		{
			if (filter == null)
				throw new ArgumentNullException("filter");

			deletedCount = 0;

			if (ValidateBlogAdminPermission(operatorID) == false)
				return false;

			int deleteCountTemp = 0;

			bool succeed = ProcessDeleteBlogArticles(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs)
			{
				return BlogDao.Instance.DeleteBlogArticlesBySearch(filter, operatorID, excludeRoleIDs, deleteTopCount, out deleteCountTemp);
			});

			deletedCount = deleteCountTemp;

			return succeed;
		}

		private delegate DeleteResult DeleteBlogArticlesCallback(Guid[] excludeRoleIDs);

		private bool ProcessDeleteBlogArticles(int operatorID, bool isUpdatePoint, DeleteBlogArticlesCallback deleteAction)
		{
            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

			DeleteResult deleteResult = null;

			if (isUpdatePoint)
			{
				bool succeed = BlogPointAction.Instance.UpdateUsersPoints(delegate(PointActionManager.TryUpdateUserPointState state, out PointResultCollection<BlogPointType> pointResults)
				{
					pointResults = null;

					if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
						return false;

					deleteResult = deleteAction(excludeRoleIDs);

					if (deleteResult != null && deleteResult.Count > 0)
					{
						pointResults = new PointResultCollection<BlogPointType>();

						foreach (DeleteResultItem item in deleteResult)
						{
							pointResults.Add(item.UserID, item.UserID == operatorID ? BlogPointType.ArticleWasDeletedBySelf : BlogPointType.ArticleWasDeletedByAdmin, item.Count);
						}

						return true;
					}

					return false;
				});
			}
			else
			{
				deleteResult = deleteAction(excludeRoleIDs);
			}

			if (deleteResult != null && deleteResult.Count > 0)
			{
				ClearCachedEveryoneData();

				foreach (DeleteResultItem item in deleteResult)
				{
					ClearCachedUserData(item.UserID);
				}
			}

			return true;
		}

		#endregion

		#region =========↓分类↓====================================================================================================

		/// <summary>
		/// 创建日志分类
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="categoryName">日志分类名称</param>
		/// <param name="newCategoryID">新建的分类ID</param>
		/// <returns>发布成功返回true，因为任何原因发布失败则返回false，可以通过ErrorScope捕获具体错误信息</returns>
		public bool CreateBlogCategory(int operatorID, string categoryName, out int newCategoryID)
		{
			newCategoryID = 0;

			if (ValidateBlogUsePermission(operatorID) == false || ValidateCategoryName(categoryName) == false)
				return false;

			bool isCreated = BlogDao.Instance.CreateBlogCategory(operatorID, categoryName, out newCategoryID);

			if (isCreated)
				ClearCachedUserBlogCategories(operatorID);

			return true;
		}

		/// <summary>
		/// 更新日志分类
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="categoryID">日志分类ID</param>
		/// <param name="categoryName">新的分类名称</param>
		/// <returns>发布成功返回true，因为任何原因发布失败则返回false，可以通过ErrorScope捕获具体错误信息</returns>
		public bool UpdateBlogCategory(int operatorID, int categoryID, string categoryName)
		{
			BlogCategory category = BlogDao.Instance.GetBlogCategory(categoryID);

			if (ValidateBlogCategoryEditPermission(operatorID, category) == false || ValidateCategoryName(categoryName) == false)
				return false;

			bool isUpdated = BlogDao.Instance.UpdateBlogCategory(categoryID, categoryName);

			if (isUpdated)
				ClearCachedUserBlogCategories(category.UserID);

			return true;
		}

		/**************************************
		 *       Get开头的函数获取数据        *
		 **************************************/

		/// <summary>
		/// 获取指定用户的指定日志分类
		/// </summary>
		/// <param name="userID">分类所有者的ID</param>
		/// <param name="categoryID">指定日志分类ID</param>
		public BlogCategory GetUserBlogCategory(int userID, int categoryID)
		{
			BlogCategoryCollection categories = this.GetUserBlogCategories(userID);

			BlogCategory category = categories.GetValue(categoryID);

			return category != null ? category : new BlogCategory();
		}

		/// <summary>
		/// 获取指定用户所有日志分类
		/// </summary>
		/// <param name="userId">分类所有者的ID</param>
		public BlogCategoryCollection GetUserBlogCategories(int userID)
		{
			if (ValidateUserID(userID) == false)
				return null;

			string cacheKey = GetCacheKeyForUserBlogCategories(userID);

			BlogCategoryCollection categories = null;

			if (CacheUtil.TryGetValue<BlogCategoryCollection>(cacheKey, out categories))
				return categories;

			categories = BlogDao.Instance.GetUserBlogCategories(userID);

			CacheUtil.Set<BlogCategoryCollection>(cacheKey, categories);

			ProcessKeyword(categories, ProcessKeywordMode.TryUpdateKeyword);

			return categories;
		}

		public BlogCategory GetBlogCategoryForEdit(int operatorID, int categoryID)
		{
			if (ValidateUserID(operatorID) == false)
				return null;

			if (ValidateCategoryID(categoryID) == false)
				return null;

			BlogCategory category = BlogDao.Instance.GetBlogCategory(categoryID);

			if (category == null)
				return null;

			if (ValidateBlogCategoryEditPermission(operatorID, category) == false)
				return null;

			return category;
		}

		public BlogCategory GetBlogCategoryForDelete(int operatorID, int categoryID)
		{
			if (ValidateUserID(operatorID) == false)
				return null;

			if (ValidateCategoryID(categoryID) == false)
				return null;

			BlogCategory category = BlogDao.Instance.GetBlogCategory(categoryID);

			if (category == null)
				return null;

			if (ValidateBlogCategoryDeletePermission(operatorID, category) == false)
				return null;

			return category;
		}

		/// <summary>
		/// 获取博客分类用于管理
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="filter">文章搜索条件</param>
		/// <param name="pageNumber">数据分页页码</param>
		/// <param name="pageSize">数据分页每页记录数</param>
		/// <returns></returns>
		public BlogCategoryCollection GetBlogCategoriesForAdmin(int operatorID, AdminBlogCategoryFilter filter, int pageNumber)
		{
			if (ValidateBlogAdminPermission(operatorID) == false)
				return null;

            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

			BlogCategoryCollection categories = BlogDao.Instance.GetBlogCategoriesBySearch(excludeRoleIDs, filter, pageNumber);

			ProcessKeyword(categories, ProcessKeywordMode.FillOriginalText);

			return categories;
		}

		/**************************************
		 *      Delete开头的函数删除数据      *
		 **************************************/

		/// <summary>
		/// 删除指定日志分类
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="categoryID">日志分类ID</param>
		/// <param name="deleteArticle">是否删除日志分类的文章</param>
		/// <returns>操作成功返回true,否则返回false</returns>
		public bool DeleteBlogCategory(int operatorID, int categoryID, bool isDeleteArticle, bool isUpdatePoint)
		{
			if (ValidateUserID(operatorID) == false)
				return false;

			if (ValidateCategoryID(categoryID) == false)
				return false;

			BlogCategory category = BlogDao.Instance.GetBlogCategory(categoryID);

			if (ValidateBlogCategoryDeletePermission(operatorID, category) == false)
				return false;

			return DeleteBlogCategories(operatorID, new int[] { categoryID }, isDeleteArticle, isUpdatePoint);
		}

		/// <summary>
		/// 删除指定日志分类
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="categoryIDs">日志分类ID集合</param>
		/// <param name="deleteArticle">是否删除日志分类的文章</param>
		/// <returns>操作成功返回true,否则返回false</returns>
		public bool DeleteBlogCategories(int operatorID, int[] categoryIDs, bool isDeleteArticle, bool isUpdatePoint)
		{
			return ProcessDeleteBlogArticles(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs)
			{
				return BlogDao.Instance.DeleteBlogCategories(categoryIDs, isDeleteArticle, operatorID, excludeRoleIDs);
			});
		}

		/// <summary>
		/// 删除日志分类用于管理
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="filter">分类搜索条件，符合条件的都将被删除</param>
		/// <param name="deleteTopCount">指定删除前多少条数据</param>
		/// <param name="deletedCount">真实删除的数据条数</param>
		/// <returns>操作成功返回true，否则返回false</returns>
		public bool DeleteBlogCategoriesForAdmin(int operatorID, AdminBlogCategoryFilter filter, bool isDeleteArticle, bool isUpdatePoint, int deleteTopCount, out int deletedCount)
		{
			if (filter == null)
				throw new ArgumentNullException("filter");

			deletedCount = 0;

			if (ValidateBlogAdminPermission(operatorID) == false)
				return false;

			int deleteCountTemp = 0;

			bool succeed = ProcessDeleteBlogArticles(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs)
			{
				return BlogDao.Instance.DeleteBlogCategoriesBySearch(filter, isDeleteArticle, operatorID, excludeRoleIDs, deleteTopCount, out deleteCountTemp);
			});

			deletedCount = deleteCountTemp;

			return succeed;
		}

		#endregion

		#region=========↓检查↓=====================================================================================================

		/**************************************
		 *  Validate开头的函数会抛出错误信息  *
		 **************************************/

		/// <summary>
		/// 检查日志ID数据合法性
		/// </summary>
		/// <param name="articleID"></param>
		/// <returns></returns>
		private bool ValidateBlogArticleID(int articleID)
		{
			if (articleID <= 0)
			{
				ThrowError(new InvalidParamError("articleID"));
				return false;
			}

			return true;
		}

		/// <summary>
		/// 基本检查日志标题
		/// </summary>
		private bool ValidateBlogArticleSubject(string subject)
		{
			int maxSubjectLength = 100;

			if (string.IsNullOrEmpty(subject))
			{
				ThrowError(new BlogArticleSubjectEmptyError("subject"));
				return false;
			}

			if (subject[0] == ' ' || subject[subject.Length - 1] == ' ')
			{
				ThrowError(new BlogArticleSubjectFormatError("subject"));
			}

			if (StringUtil.GetByteCount(subject) > maxSubjectLength)
			{
				ThrowError(new BlogArticleSubjectLengthError("subject", subject, maxSubjectLength));
				return false;
			}

			ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

			if (keywords.BannedKeywords.IsMatch(subject, out keyword))
			{
				ThrowError(new BlogArticleSubjectBannedKeywordsError("subject", keyword));
				return false;
			}

			return true;
		}

		/// <summary>
		/// 基本检查日志正文
		/// </summary>
		private bool ValidateBlogArticleContent(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				ThrowError(new BlogArticleContentEmptyError("content"));
				return false;
			}

            //if (StringUtil.GetByteCount(content) > maxContentLength)
            //{
            //    ThrowError(new BlogArticleContentLengthError("content", content, maxContentLength));
            //    return false;
            //}

			ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

			if (keywords.BannedKeywords.IsMatch(content, out keyword))
			{
				ThrowError(new BlogArticleContentBannedKeywordsError("content", keyword));
				return false;
			}

			return true;
		}

		/// <summary>
		/// 检查日志密码
		/// </summary>
		private bool ValidateBlogArticlePassword(string password)
		{
			if (string.IsNullOrEmpty(password))
			{
				Context.ThrowError(new BlogArticlePasswordEmptyError("password"));
			}

			return true;
		}

		/// <summary>
		/// 检查日志分类ID
		/// </summary>
		private bool ValidateCategoryID(int categoryID)
		{
			if (categoryID <= 0)
			{
				ThrowError(new InvalidParamError("categoryID"));
				return false;
			}
			return true;
		}

		/// <summary>
		/// 检查日志分类名称
		/// </summary>
		private bool ValidateCategoryName(string categoryName)
		{
			int maxLength = 10;

			if (string.IsNullOrEmpty(categoryName))
			{
				ThrowError(new BlogCategoryNameEmptyError("categoryName"));
				return false;
			}

			if (categoryName[0] == ' ' || categoryName[categoryName.Length - 1] == ' ')
			{
				ThrowError(new BlogCategoryNameFormatError("categoryName"));
			}

			if (StringUtil.GetByteCount(categoryName) > maxLength)
			{
				ThrowError(new BlogCategoryNameLengthError("categoryName", categoryName, maxLength));
				return false;
			}

			ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

			if (keywords.BannedKeywords.IsMatch(categoryName, out keyword))
			{
				ThrowError(new BlogCategoryNameBannedKeywordsError("categoryName", keyword));
				return false;
			}

			return true;
		}

		/// <summary>
		/// 验证操作者的博客使用权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <returns></returns>
		private bool ValidateBlogUsePermission(int operatorID)
		{
			return ValidateUsePermission<NoPermissionUseBlogError>(operatorID);
		}

		/// <summary>
		/// 验证操作者的博客管理权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <returns></returns>
		private bool ValidateBlogAdminPermission(int operatorID)
		{
			return ValidateAdminPermission<NoPermissionManageBlogError>(operatorID);
		}

		/// <summary>
		/// 验证操作者对某日志的浏览权限
		/// </summary>
		/// <param name="visitorID">操作者ID</param>
		/// <param name="password">操作者提供的密码，为null时将自动从PasswordBox获取已缓存的密码</param>
		/// <param name="article">所要浏览的日志</param>
		/// <returns>有浏览权限时返回true，否则返回false</returns>
		private bool ValidateBlogArticleVisitPermission(int visitorID, string password, BlogArticle article)
		{
			if (article == null)
				return false;

			CheckVisitPermissionResult result = CheckVisitPermission(visitorID, article.UserID, article.PrivacyType, article.ArticleID, article.Password, password);

			switch (result)
			{
				case CheckVisitPermissionResult.CanVisit:
					return true;

				case CheckVisitPermissionResult.OnlyDataOwnerCanVisit:
					ThrowError(new NoPermissionVisitBlogArticleBeacuseNotOwnerError());
					return false;

				case CheckVisitPermissionResult.OnlyDataOwnerFriendsCanVisit:
					ThrowError(new NoPermissionVisitBlogArticleBeacuseNotFriendError());
					return false;

				case CheckVisitPermissionResult.OnlyDataPasswordHolderCanVisit:
					ThrowError(new NoPermissionVisitBlogArticleBeacuseNeedPasswordError());
					return false;
			}

			return true;
		}

		/// <summary>
		/// 验证操作者是否具有某日志的编辑权限
		/// </summary>
		/// <param name="editorID">操作者ID</param>
		/// <param name="article">所要操作的日志</param>
		/// <returns>有编辑权限返回true，否则返回false</returns>
		private bool ValidateBlogArticleEditPermission(int editorID, BlogArticle article)
		{
			if (CheckBlogArticleEditPermission(editorID, article))
				return true;

			ThrowError(new NoPermissionEditBlogArticleError());

			return false;
		}

		/// <summary>
		/// 验证操作者是否具有某日志的删除权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="article">所要操作的日志</param>
		/// <returns>有删除权限返回true，否则返回false</returns>
		private bool ValidateBlogArticleDeletePermission(int operatorID, BlogArticle article)
		{
			if (CheckBlogArticleDeletePermission(operatorID, article))
				return true;

			ThrowError(new NoPermissionDeleteBlogArticleError());

			return false;
		}

		private bool ValidateBlogCategoryEditPermission(int operatorID, BlogCategory category)
		{
			if (CheckBlogCategoryEditPermission(operatorID, category))
				return true;

			ThrowError(new NoPermissionEditBlogCategoryError());

			return false;
		}

		/// <summary>
		/// 验证操作者是否具有某日志分类的删除权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="category">所要操作的日志分类</param>
		/// <returns>有删除权限返回true，否则返回false</returns>
		private bool ValidateBlogCategoryDeletePermission(int operatorID, BlogCategory category)
		{
			if (CheckBlogCategoryDeletePermission(operatorID, category))
				return true;

			ThrowError(new NoPermissionDeleteBlogCategoryError());

			return false;
		}

		/**************************************
		 *    Check开头的函数只检查不抛错     *
		 **************************************/

		public bool CheckBlogArticleAddPermission(int operatorID)
		{
			return Permission.Can(operatorID, UseAction);
		}

		/// <summary>
		/// 验证操作者对某日志的浏览权限
		/// </summary>
		/// <param name="visitorID">操作者ID</param>
		/// <param name="password">操作者提供的密码，为null时将自动从PasswordBox获取已缓存的密码</param>
		/// <param name="article">所要浏览的日志</param>
		/// <returns>有浏览权限时返回true，否则返回false</returns>
		public bool CheckBlogArticleVisitPermission(int visitorID, BlogArticle article)
		{
			return article != null && CheckVisitPermission(visitorID, article.UserID, article.PrivacyType, article.ArticleID, article.Password, null) == CheckVisitPermissionResult.CanVisit;
		}

		/// <summary>
		/// 验证操作者是否具有某日志的编辑权限
		/// </summary>
		/// <param name="editorID">操作者ID</param>
		/// <param name="article">所要操作的日志</param>
		/// <returns>有编辑权限返回true，否则返回false</returns>
		public bool CheckBlogArticleEditPermission(int editorID, BlogArticle article)
		{
			return article != null && CheckEditPermission(editorID, article.UserID, article.LastEditUserID);
		}

		/// <summary>
		/// 验证操作者是否具有某日志的删除权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="article">所要操作的日志</param>
		/// <returns>有删除权限返回true，否则返回false</returns>
		public bool CheckBlogArticleDeletePermission(int operatorID, BlogArticle article)
		{
			return article != null && CheckDeletePermission(operatorID, article.UserID, article.LastEditUserID);
		}

		public bool CheckBlogCategoryAddPermission(int operatorID)
		{
			return Permission.Can(operatorID, UseAction);
		}

		/// <summary>
		/// 验证操作者是否具有某日志的编辑权限
		/// </summary>
		/// <param name="editorID">操作者ID</param>
		/// <param name="article">所要操作的日志</param>
		/// <returns>有编辑权限返回true，否则返回false</returns>
		public bool CheckBlogCategoryEditPermission(int editorID, BlogCategory category)
		{
			return category != null && CheckEditPermission(editorID, category.UserID);
		}

		/// <summary>
		/// 验证操作者是否具有某日志分类的删除权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="category">所要操作的日志分类</param>
		/// <returns>有删除权限返回true，否则返回false</returns>
		public bool CheckBlogCategoryDeletePermission(int operatorID, BlogCategory category)
		{
			return category != null && CheckDeletePermission(operatorID, category.UserID);
		}

		#endregion

		#region=========↓缓存↓====================================================================================================

		/**************************************
		 *   GetCacheKey开头的函获取缓存键    *
		 **************************************/

		private string GetCacheKeyForUserBlogCategories(int userID)
		{
			return "Blog/Categories/" + userID;
		}

		/// <summary>
		/// 获取用户博客标签的缓存键
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <returns></returns>
		private string GetCacheKeyForUserBlogTags(int userID)
		{
			return "Blog/" + userID + "/Tags";
		}

		/// <summary>
		/// 获取“大家的日志”的缓存键
		/// </summary>
		/// <param name="pageNumber">数据分页当前页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <returns></returns>
		private string GetCacheKeyForEveryoneBlogArticles(int pageNumber, int pageSize)
		{
			return "Blog/All/" + pageSize + "/" + pageNumber;
		}

		/// <summary>
		/// 获取“大家的日志”总记录数的缓存键
		/// </summary>
		/// <returns></returns>
		private string GetCacheKeyForEveryoneBlogArticlesTotalCount()
		{
			return "Blog/All/ArticleCount";
		}

		/// <summary>
		/// 获取日志总条数的缓存键
		/// </summary>
		/// <param name="articleOwnerID">日志所有者ID</param>
		/// <param name="categoryID">分类ID</param>
		/// <param name="dataAccessLevel">数据访问级别</param>
		/// <returns></returns>
		private string GeCacheKeyForUserBlogArticlesTotalCount(int articleOwnerID, int? categoryID, int? tagID, DataAccessLevel dataAccessLevel)
		{
			if (categoryID.HasValue)
			{
				if (tagID.HasValue)
					return "Blog/" + articleOwnerID + "/ArticleCount1/" + categoryID + "/" + tagID + "/" + dataAccessLevel;
				else
					return "Blog/" + articleOwnerID + "/ArticleCount2/" + categoryID + "/" + dataAccessLevel;
			}
			else
			{
				if (tagID.HasValue)
					return "Blog/" + articleOwnerID + "/ArticleCount3/" + tagID + "/" + dataAccessLevel;
				else
					return "Blog/" + articleOwnerID + "/ArticleCount4/" + dataAccessLevel;
			}
		}

		/// <summary>
		/// 获取博客访问者的缓存键，用于VisitorBox
		/// </summary>
		/// <param name="visitorID">访问者ID</param>
		/// <param name="visitorIP">访问者IP</param>
		/// <param name="articleID">访问的日志ID</param>
		/// <returns></returns>
		private string GetCacheKeyForBlogArticleVisitor(int visitorID, string visitorIP, int articleID)
		{
			return "Blog/V/" + visitorID + "/" + visitorIP + "/" + articleID;
		}

		/**************************************
		 *      Clear开头的函数清除缓存       *
		 **************************************/

		/// <summary>
		/// 清除按用户缓存的日志分类
		/// </summary>
		/// <param name="userID"></param>
		private void ClearCachedUserBlogCategories(int userID)
		{
			CacheUtil.Remove(GetCacheKeyForUserBlogCategories(userID));
		}

		/// <summary>
		/// 清除所有按用户缓存的数据
		/// </summary>
		/// <param name="userID">用户ID</param>
		private void ClearCachedUserData(int userID)
		{
			CacheUtil.RemoveBySearch("Blog/" + userID);
		}

		/// <summary>
		/// 清除所有“大家的”缓存数据
		/// </summary>
		private void ClearCachedEveryoneData()
		{
			CacheUtil.RemoveBySearch("Blog/All/");
		}

		#endregion

		#region=========↓关键字↓==================================================================================================

		public void ProcessKeyword(BlogArticle article, ProcessKeywordMode mode)
		{
            //更新关键字模式，如果这个文章并不需要处理，直接退出
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                if (AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.NeedUpdate2(article) == false)
                    return;
            }

			BlogArticleCollection articles = new BlogArticleCollection();

			articles.Add(article);

			ProcessKeyword(articles, mode);
		}

		public void ProcessKeyword(BlogArticleCollection articles, ProcessKeywordMode mode)
		{
			if (articles.Count == 0)
				return;

			KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

			bool needProcess = false;

			//更新关键字模式，只在必要的情况下才取恢复信息并处理
			if (mode == ProcessKeywordMode.TryUpdateKeyword)
			{
                needProcess = keyword.NeedUpdate2<BlogArticle>(articles);
			}
			//填充原始内容模式，始终都要取恢复信息，但不处理
			else
			{
				needProcess = true;
			}

			if (needProcess)
			{
				Revertable2Collection<BlogArticle> articlesWithReverter = BlogDao.Instance.GetBlogArticlesWithReverters(articles.GetKeys());

				if (articlesWithReverter != null)
				{
					if (keyword.Update2(articlesWithReverter))
					{
						BlogDao.Instance.UpdateBlogArticleKeywords(articlesWithReverter);
					}

					//将新数据填充到旧的列表
					articlesWithReverter.FillTo(articles);
				}
			}
		}

		private void ProcessKeyword(BlogCategory category, ProcessKeywordMode mode)
		{
            //更新关键字模式，如果这个分类并不需要处理，直接退出
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                if (AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.NeedUpdate<BlogCategory>(category) == false)
                    return;
            }

			BlogCategoryCollection categories = new BlogCategoryCollection();

			categories.Add(category);

			ProcessKeyword(categories, mode);
		}

		private void ProcessKeyword(BlogCategoryCollection categories, ProcessKeywordMode mode)
		{
			if (categories.Count == 0)
				return;

			KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

			bool needProcess = false;

			//更新关键字模式，只在必要的情况下才取恢复信息并处理
			if (mode == ProcessKeywordMode.TryUpdateKeyword)
			{
				needProcess = keyword.NeedUpdate<BlogCategory>(categories);
			}
			//填充原始内容模式，始终都要取恢复信息，但不处理
			else
			{
				needProcess = true;
			}

			if (needProcess)
			{
				RevertableCollection<BlogCategory> categoriesWithReverter = BlogDao.Instance.GetBlogCategoriesWithReverters(categories.GetKeys());

				if (categoriesWithReverter != null)
				{
					if (keyword.Update(categoriesWithReverter))
					{
						BlogDao.Instance.UpdateBlogCategoryKeywords(categoriesWithReverter);
					}

					//将新数据填充到旧的列表
					categoriesWithReverter.FillTo(categories);
				}
			}
		}

		#endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleID"></param>
        /// <returns></returns>
        public BlogArticle GetBlogArticle(int articleID)
        {
            string key = "BlogArticle/" + articleID;
            BlogArticle article;

            if(PageCacheUtil.TryGetValue<BlogArticle>(key,out article) == false)
            {
                article = BlogDao.Instance.GetBlogArticle(articleID);
                article.Content = ProcessArticleContent(article.Content);
                if (article != null)
                    PageCacheUtil.Set(key, article);
            }

            return article;
        }

        /// <summary>
        /// 更新日志访问记录
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="articleID"></param>
        public bool UpdateVisitCount(AuthUser visitor, int articleID)
        {
            string blogVisitorKey = GetCacheKeyForBlogArticleVisitor(visitor.UserID, visitor.LastVisitIP, articleID);

            DateTime lastViewTime = DateTime.MinValue;

            if (visitor.TempDataBox.GetData(blogVisitorKey) != null)
            {
                lastViewTime = DateTime.Parse(visitor.TempDataBox.GetData(blogVisitorKey));
            }

            //在一定时间间隔内不重复计算访问次数
            if ((DateTimeUtil.Now - lastViewTime).TotalSeconds > Consts.BlogArticle_VisitorTimeScope)
            {
                visitor.TempDataBox.SetData(blogVisitorKey, DateTimeUtil.Now.ToString());

                if (visitor.UserID != 0)
                {
                    BlogDao.Instance.VisitArticle(visitor.UserID, articleID);
                    return true;
                }
            }

            return false;
        }

        private string EncryptArticlePassword(string password)
        {
            return SecurityUtil.Encrypt(EncryptFormat.bbsMax, password);
        }

        /// <summary>
        /// 是否已经输过密码
        /// </summary>
        /// <param name="my"></param>
        /// <param name="articleID"></param>
        /// <returns></returns>
        public bool HasArticlePassword(AuthUser my, int articleID)
        {
            return CheckArticlePassword(my, articleID, null);
        }

        /// <summary>
        /// 检查密码是否正确
        /// </summary>
        /// <param name="my"></param>
        /// <param name="articleID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckArticlePassword(AuthUser my, int articleID, string password)
        {
            BlogArticle article = GetBlogArticle(articleID);
            if (article.PrivacyType != PrivacyType.NeedPassword || article.UserID == my.UserID)
                return true;

            string key = "BlogArticle_password/" + articleID;
            bool needSetPassword = true;
            if (password == null)
            {
                needSetPassword = false;
                password = my.TempDataBox.GetData(key);
            }

            if (password == null)
                return false;


            string tempPassword = EncryptArticlePassword(password);

            if (tempPassword != article.Password)
            {
                return false;
            }

            if (needSetPassword)
                my.TempDataBox.SetData(key, password);

            return true;
        }


        public BlogArticleCollection GetCommentedArticles(int userID, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            BlogArticleCollection articles = BlogDao.Instance.GetCommentedArticles(userID, pageNumber, pageSize);

            ProcessKeyword(articles, ProcessKeywordMode.TryUpdateKeyword);

            return articles;
        }
	}
}