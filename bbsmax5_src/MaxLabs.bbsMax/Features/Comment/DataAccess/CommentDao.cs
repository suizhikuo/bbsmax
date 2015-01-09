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
    public abstract class CommentDao : DaoBase<CommentDao>
    {
		/// <summary>
		/// 获取给某用户的最新评论
		/// </summary>
		/// <param name="targetUserID"></param>
		/// <param name="type"></param>
		/// <param name="top"></param>
		/// <returns></returns>
		public abstract CommentCollection GetLastestCommentsForSomeone(int targetUserID, CommentType type, int top);

		/// <summary>
		/// 搜索评论
		/// </summary>
		/// <param name="excludeRoleIDs"></param>
		/// <param name="filter"></param>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public abstract CommentCollection GetCommentsBySearch(int operatorID, Guid[] excludeRoleIDs, AdminCommentFilter filter, int pageNumber);

		/// <summary>
		/// 删除搜索结果
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="operatorID"></param>
		/// <param name="excludeRoleIDs"></param>
		/// <param name="topCount"></param>
		/// <param name="deletedCount"></param>
		/// <returns></returns>
		public abstract DeleteResult DeleteCommentsBySearch(AdminCommentFilter filter, int operatorID, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount);

		#region=========↓关键字↓==================================================================================================

		/// <summary>
		/// 为需要更新的关键字填充恢复关键信息
		/// </summary>
		/// <param name="processlist">要处理的列表</param>
		public abstract RevertableCollection<Comment> GetCommentsWithReverters(IEnumerable<int> commentDs);

		/// <summary>
		/// 更新关键字
		/// </summary>
		/// <param name="processlist">要处理的列表</param>
		public abstract void UpdateCommentKeywords(RevertableCollection<Comment> processlist);

		#endregion

        /// <summary>
        /// 添加评论 在触发器中更新该对象的评论数
        /// </summary>
        /// <param name="userID">评论用户的ID</param>
        /// <param name="targetID">评论对象ID</param>
        /// <param name="type">评论类型</param>
        /// <param name="isApproved">是否需要审核</param>
        /// <param name="content">内容</param>
        /// <param name="createIP">IP</param>
        public abstract void AddComment(int userID, int targetID, CommentType type, bool isApproved, string content, /* string contentReverter, */ string createIP, out int targetUserID, out int newCommentId);

        /// <summary>
        /// 删除评论 在触发器中更新该对象的评论数
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="commentID"></param>
        public abstract void DeleteComment(int userID, int commentID, out int commentUserID);

        /// <summary>
        /// 批量删除评论 后台 在触发器中更新该对象的评论数
        /// </summary>
        /// <param name="commentIDs"></param>
        public abstract void DeleteComments(IEnumerable<int> commentIDs);

        /// <summary>
        /// 删除搜索结果
        /// </summary>
        /// <param name="filter"></param>
        public abstract DeleteResult DeleteCommentsByFilter(AdminCommentFilter filter, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, bool getDeleteResult);

		///// <summary>
		///// 为需要更新的关键字填充恢复关键信息
		///// </summary>
		///// <param name="processlist"></param>
		//public abstract void FillCommentReverters(TextRevertableCollection processlist);

		///// <summary>
		///// 更新关键字
		///// </summary>
		///// <param name="processlist">要处理的列表</param>
		//public abstract void UpdateCommentKeywords(TextRevertableCollection processlist);

        /// <summary>
        /// 批量审核评论 后台
        /// </summary>
        /// <param name="commentIDs"></param>
        public abstract void ApproveComments(IEnumerable<int> commentIDs);

		/// <summary>
		/// 评论编辑
		/// </summary>
		/// <param name="commentID"></param>
		/// <param name="userID"></param>
		/// <param name="isApproved"></param>
		/// <param name="content"></param>
		/// <param name="contentReverter"></param>
		public abstract void UpdateComment(int commentID, int userID, bool isApproved, string content, string contentReverter, out int targetID);

        /// <summary>
        /// 某个对象的评论 分页
        /// </summary>
        /// <param name="targetID"></param>
        /// <returns></returns>
        public abstract CommentCollection GetCommentsByTargetID(int targetID, CommentType type, int pageNumber, int pageSize, bool isDesc, out int totalCount);

        /// <summary>
        /// 某人的评论
        /// </summary>
        /// <param name="targetID"></param>
        /// <returns></returns>
        public abstract CommentCollection GetCommentsByUserID(int userID, CommentType type, int pageNumber, int pageSize, out int totalCount);

        /// <summary>
        /// 搜索评论 后台
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public abstract CommentCollection GetCommentsByFilter(AdminCommentFilter filter, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, int pageNumber, out int totalCount);

        public abstract Comment GetComment(int commentID);

        /// <summary>
        /// 按条数取评论
        /// </summary>
        /// <param name="targetID"></param>
        /// <param name="type"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract CommentCollection GetComments(int targetID, CommentType type, int count);


        public abstract CommentCollection GetComments(IEnumerable<int> commentIDs);

        public abstract CommentCollection GetComments(int targetID, CommentType type, int getCount, bool isGetAll);

	}
}