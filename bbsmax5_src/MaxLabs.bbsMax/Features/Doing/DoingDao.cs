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
    public abstract class DoingDao : DaoBase<DoingDao>
	{
        public abstract int GetTodayPostDoingCount(int userID, DateTime today);

		public abstract int GetPostDoingCount(int userID, DateTime beginDate, DateTime endDate);

		public abstract Doing GetDoing(int doingID);

		/// <summary>
		/// 获取指定ID的记录
		/// </summary>
		/// <param name="doingIDs"></param>
		/// <returns></returns>
		public abstract DoingCollection GetDoings(IEnumerable<int> doingIDs);

		/// <summary>
		/// 获取用户的记录包含当前页的记录的评论（通过记录的CommentList属性访问）
		/// </summary>
		/// <param name="doingOwnerID">记录所有者ID</param>
		/// <param name="dataAccessLevel">数据访问级别，将根据此级别返回数据</param>
		/// <param name="pageNumber">数据分页每页条数</param>
		/// <param name="pageSize">数据分页当前页码</param>
		/// <param name="totalCount">总记录数</param>
		/// <returns></returns>
		public abstract DoingCollection GetUserDoingsWithComments(int doingOwnerID, DataAccessLevel dataAccessLevel, int pageNumber, int pageSize, ref int? totalCount);

		/// <summary>
		/// 获取用户好友的记录包含当前页的记录的评论（通过记录的CommentList属性访问）
		/// </summary>
		/// <param name="friendOwnerID">好友所有者ID</param>
		/// <param name="pageNumber">数据分页每页条数</param>
		/// <param name="pageSize">数据分页当前页码</param>
		/// <param name="totalCount">总记录数</param>
		/// <returns></returns>
		public abstract DoingCollection GetFriendDoingsWithComments(int friendOwnerID, int pageNumber, int pageSize, ref int? totalCount);

		/// <summary>
		/// 获取用户好友的记录包含当前页的记录的评论（通过记录的CommentList属性访问）
		/// </summary>
		/// <param name="pageNumber">数据分页每页条数</param>
		/// <param name="pageSize">数据分页当前页码</param>
		/// <param name="totalCount">总记录数</param>
		/// <returns></returns>
		public abstract DoingCollection GetEveryoneDoingsWithComments(int pageNumber, int pageSize, ref int? totalCount);

		/// <summary>
		/// 获取指定用户评论过的记录包含当前页的记录的评论（通过记录的CommentList属性访问）
		/// </summary>
		/// <param name="userID">评论者ID</param>
		/// <param name="pageNumber">数据分页每页条数</param>
		/// <param name="pageSize">数据分页当前页码</param>
		/// <param name="totalCount">总记录数</param>
		/// <returns></returns>
		public abstract DoingCollection GetUserCommentedDoingsWithComments(int userID, int pageNumber, int pageSize, ref int? totalCount);

		public abstract DoingCollection GetDoingsBySearch(Guid[] excludeRoleIDs, AdminDoingFilter filter, int pageNumber);

        /// <summary>
        /// 创建记录
        /// </summary>
		/// <param name="creatorID">创建者ID</param>
		/// <param name="creatorIP">创建者IP</param>
        /// <param name="content">记录内容</param>
		public abstract void AddDoing(int creatorID, string creatorIP, string content, out int doingID);

		/// <summary>
		/// 删除记录
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="id"></param>
		public abstract void DeleteDoing(int doingID);

		/// <summary>
		/// 删除记录
		/// </summary>
		/// <param name="doingIDs"></param>
		public abstract DeleteResult DeleteDoings(int operatorID, IEnumerable<int> doingIDs, IEnumerable<Guid> excludeRoleIDs);

		/// <summary>
		/// 删除搜索结果
		/// </summary>
		/// <param name="filter">搜索条件</param>
		/// <param name="excludeRoleIDs">操作者没有权限管理的用户组ID</param>
		/// <param name="topCount">删除搜索结果的前几条</param>
		/// <param name="deletedCount">真实删除的条数</param>
		/// <returns></returns>
		public abstract DeleteResult DeleteDoingsBySearch(AdminDoingFilter filter, IEnumerable<Guid> excludeRoleIDs, int deleteTopCount, out int deletedCount);

		public abstract RevertableCollection<Doing> GetDoingsWithReverters(IEnumerable<int> doingIDs);

		public abstract void UpdateDoingKeywords(RevertableCollection<Doing> processlist);
	}
}