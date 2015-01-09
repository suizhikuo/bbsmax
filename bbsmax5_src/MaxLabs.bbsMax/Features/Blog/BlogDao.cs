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

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class BlogDao : DaoBase<BlogDao>
	{
		#region =========↓日志↓====================================================================================================

		/// <summary>
		/// 检查一个日志是否存在
		/// </summary>
		/// <param name="articleID">日志ID</param>
		/// <returns></returns>
		public abstract bool ExistsBlogArticle(int articleID);

		/// <summary>
		/// 获取某用户在某段时间至今发过多少日志
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="beginDate">起始时间</param>
		/// <returns></returns>
		public abstract int GetPostBlogArticleCount(int userID, DateTime beginDate, DateTime endDate);

		/// <summary>
		/// 获取某个用户对谋篇日志的评论数量
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="articleID">日志ID</param>
		/// <returns></returns>
		public abstract int GetCommentCountForArticle(int userID, int articleID, DateTime beginDate, DateTime endDate);

		/// <summary>
		/// 获取某个用户对另一个用户的日志的评论数量
		/// </summary>
		/// <param name="userID">发表评论者的ID</param>
		/// <param name="targetUserID">被评论者的ID</param>
		/// <returns></returns>
		public abstract int GetCommentCountForUser(int userID, int targetUserID, DateTime beginDate, DateTime endDate);

		/// <summary>
		/// 获取指定的日志（注意：数据中包含了日志的标签列表和访问者列表的数据）
		/// </summary>
		/// <param name="articleID">日志ID</param>
		public abstract BlogArticle GetBlogArticle(int articleID);

		/// <summary>
		/// 获取指定用户的日志
		/// </summary>
		/// <param name="userID">日志所有者ID</param>
		/// <param name="categoryID">日志所属分类的ID，传NULL则不以分类作为条件</param>
		/// <param name="tagID">日志所含标签的ID，传null则不以分类作为条件</param>
		/// <param name="dataAccessLevel">数据访问级别，将根据此级别返回对应的数据</param>
		/// <param name="pageNumber">分页页码，从1开始计数</param>
		/// <param name="pageSize">分页尺度</param>
		/// <param name="totalCount">总数据条数（注意：不是本次返回的集合数据条数），用于分页</param>
		/// <returns>日志集合</returns>
		public abstract BlogArticleCollection GetUserBlogArticles(int userID, int? categoryID, int? tagID, DataAccessLevel dataAccessLevel, int pageNumber, int pageSize, ref int? totalCount);

		/// <summary>
		/// 获取指定用户好友的日志
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="pageNumber">数据分页当前页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		public abstract BlogArticleCollection GetFriendBlogArticles(int userID, int pageNumber, int pageSize);

		/// <summary>
		/// 获取“大家的日志”
		/// </summary>
		/// <param name="pageNumber">数据分页页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <param name="totalCount">总数据条数（注意：不是本次返回的集合数据条数），用于分页</param>
		/// <returns></returns>
		public abstract BlogArticleCollection GetEveryoneBlogArticles(int pageNumber, int pageSize, ref int? totalCount);

		/// <summary>
		/// 获取用户访问过的日志
		/// </summary>
		/// <param name="visitorID">访问者ID</param>
		/// <param name="pageNumber">数据分页页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <returns></returns>
		public abstract BlogArticleCollection GetVisitedBlogArticles(int visitorID, int pageNumber, int pageSize);

		/// <summary>
		/// 获取类似标签的日志
		/// </summary>
		/// <param name="articleID">指定日志ID</param>
		/// <param name="number">获取的篇数</param>
		public abstract BlogArticleCollection GetSimilarArticles(int articleID, int number, DataAccessLevel dataAccessLevel);

		/// <summary>
		/// 搜索博客日志（后台管理用）
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="excludeRoleIDs">在返回结果中要排除哪些用户组成员发布的日志</param>
		/// <param name="filter">搜索条件</param>
		/// <param name="pageNumber">数据分页页码</param>
		/// 
		/// <returns></returns>
		public abstract BlogArticleCollection GetBlogArticlesBySearch(int operatorID, Guid[] excludeRoleIDs, AdminBlogArticleFilter filter, int pageNumber);

		/// <summary>
		/// 更新日志的访问信息
		/// </summary>
		/// <param name="userID">访问日志的用户ID</param>
		/// <param name="articleID">日志ID</param>
		public abstract void VisitArticle(int userID, int articleID);

		/// <summary>
		/// 创建日志
		/// </summary>
		/// <param name="operatorID">创建者ID</param>
		/// <param name="operatorIP">创建者ID</param>
		/// <param name="subject">日志标题</param>
		/// <param name="content">日志内容</param>
		/// <param name="thumbnail">日志缩略图</param>
		/// <param name="categoryID">日志分类ID</param>
		/// <param name="enableComment">日志是否允许评论</param>
		/// <param name="privacyType">日志隐私设置</param>
		/// <param name="password">日志访问密码</param>
		/// <param name="isApproved">日志是否通过审核</param>
		/// <returns>新日志的ID</returns>
		public abstract int CreateBlogArticle(int operatorID, string operatorIP, string subject, string content, string thumbnail, int? categoryID, bool enableComment, PrivacyType privacyType, string password, bool isApproved);

		/// <summary>
		/// 更新日志
		/// </summary>
		/// <param name="operatorID">编辑者ID</param>
		/// <param name="operatorIP">编辑者IP</param>
		/// <param name="articleID">日志ID</param>
		/// <param name="subject">日志标题</param>
		/// <param name="content">日志内容</param>
		/// <param name="thumbnail">日志缩略图</param>
		/// <param name="categoryID">日志分类ID</param>
		/// <param name="enableComment">日志是否允许评论</param>
		/// <param name="privacyType">日志隐私设置</param>
		/// <param name="password">日志访问密码</param>
		/// <param name="isApproved">日志是否通过审核</param>
		/// <returns>操作是否成功</returns>
		public abstract bool UpdateBlogArticle(int operatorID, string operatorIP, int articleID, string subject, string content, string thumbnail, int? categoryID, bool enableComment, PrivacyType privacyType, string password, bool isApproved);

		/// <summary>
		/// 删除指定的多篇日志
		/// </summary>
		/// <param name="articleIDs">日志ID集</param>
		public abstract DeleteResult DeleteBlogArticles(IEnumerable<int> articleIDs, int operatorID, IEnumerable<Guid> excludeRoleIDs);

        /// <summary>
		/// 删除搜索结果
        /// </summary>
        /// <param name="filter">搜索条件</param>
        /// <param name="operatorID">操作者ID</param>
        /// <param name="excludeRoleIDs">操作者没有权限管理的用户组ID</param>
        /// <param name="isGetDeleteResult">是否获取删除结果</param>
        /// <param name="topCount">删除搜索结果的前几条</param>
        /// <param name="deletedCount">真实删除的条数</param>
        /// <returns></returns>
        public abstract DeleteResult DeleteBlogArticlesBySearch(AdminBlogArticleFilter filter, int operatorID, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount);


        public abstract BlogArticleCollection GetCommentedArticles(int userID, int pageNumber, int pageSize);
		#endregion

		#region =========↓分类↓====================================================================================================

		/// <summary>
		/// 获取指定日志分类
		/// </summary>
		/// <param name="categoryID">日志分类ID</param>
		public abstract BlogCategory GetBlogCategory(int categoryID);

		/// <summary>
		/// 获取指定用户的日志分类
		/// </summary>
		/// <param name="userID">用户ID</param>
		public abstract BlogCategoryCollection GetUserBlogCategories(int userID);

		/// <summary>
		/// 搜索博客分类（后台管理用）
		/// </summary>
		/// <param name="excludeRoleIDs">在返回结果中要排除哪些用户组成员发布的日志</param>
		/// <param name="filter">搜索条件</param>
		/// <param name="pageNumber">数据分页页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <returns></returns>
		public abstract BlogCategoryCollection GetBlogCategoriesBySearch(Guid[] excludeRoleIDs, AdminBlogCategoryFilter filter, int pageNumber);

		/// <summary>
		/// 添加日志分类
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="categoryName">分类名称</param>
		public abstract bool CreateBlogCategory(int userID, string categoryName, out int newCategoryID);

		/// <summary>
		/// 编辑日志分类
		/// </summary>
		/// <param name="categoryID">日志分类ID</param>
		/// <param name="categoryName">分类名称</param>
		public abstract bool UpdateBlogCategory(int categoryID, string categoryName);

		/// <summary>
		/// 删除指定日志分类
		/// </summary>
		/// <param name="categoryIDs">日志分类ID集合</param>
		/// <param name="deleteArticle">是否删除日志分类的文章</param>
		public abstract DeleteResult DeleteBlogCategories(IEnumerable<int> categoryIDs, bool deleteArticle, int operatorID, IEnumerable<Guid> excludeRoleIDs);

        /// <summary>
        /// 删除日志分类
        /// </summary>
		public abstract DeleteResult DeleteBlogCategoriesBySearch(AdminBlogCategoryFilter filter, bool deleteArticle, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount);

		#endregion

		#region=========↓关键字↓==================================================================================================
		
		/// <summary>
		/// 为需要更新的关键字填充恢复关键信息
		/// </summary>
		/// <param name="processlist">要处理的列表</param>
		public abstract Revertable2Collection<BlogArticle> GetBlogArticlesWithReverters(IEnumerable<int> articleIDs);

		/// <summary>
		/// 更新关键字
		/// </summary>
		/// <param name="processlist">要处理的列表</param>
		public abstract void UpdateBlogArticleKeywords(Revertable2Collection<BlogArticle> processlist);

		/// <summary>
		/// 为需要更新的关键字填充恢复关键信息
		/// </summary>
		/// <param name="processlist">要处理的列表</param>
		public abstract RevertableCollection<BlogCategory> GetBlogCategoriesWithReverters(IEnumerable<int> categoryIDs);

		/// <summary>
		/// 更新关键字
		/// </summary>
		/// <param name="processlist">要处理的列表</param>
		public abstract void UpdateBlogCategoryKeywords(RevertableCollection<BlogCategory> processlist);

		#endregion
    }
}