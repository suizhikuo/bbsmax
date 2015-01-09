//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using System.Collections;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class ShareDao : DaoBase<ShareDao>
    {
		public abstract int GetPostShareCount(int userID, DateTime beginDate, DateTime endDate);

		/// <summary>
		/// 获取指定用户的分享
		/// </summary>
		/// <param name="shareOwnerID">分享所有者的ID</param>
		/// <param name="shareType">分享的类型，如果为null将获取所有类型的分析</param>
		/// <param name="dataAccessLevel">数据访问级别，将根据此参数返回数据</param>
		/// <param name="pageNumber">数据分页当前页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <param name="totalCount">总记录数，传递此参数时可提高查询性能，传递null时将由内部返回</param>
		/// <returns></returns>
		public abstract ShareCollection GetUserShares(int shareOwnerID, ShareType? shareType, DataAccessLevel dataAccessLevel, int pageNumber, int pageSize, ref int? totalCount);

		/// <summary>
		/// 获取指定用户的好友的分享
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="shareType">分享的类型，如果为null将获取所有类型的分析</param>
		/// <param name="pageNumber">数据分页当前页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <returns></returns>
		public abstract ShareCollection GetFriendShares(int userID, ShareType? shareType, int pageNumber, int pageSize);

		/// <summary>
		/// 获取“大家的分享”
		/// </summary>
		/// <param name="shareType">分享的类型，如果为null将获取所有类型的分析</param>
		/// <param name="pageNumber">数据分页每页条数</param>
		/// <param name="pageSize">数据分页当前页码</param>
		/// <returns></returns>
		public abstract ShareCollection GetEveryoneShares(ShareType? shareType, int pageNumber, int pageSize, ref int? totalCount);

		/// <summary>
		/// 通过搜索获取分享
		/// </summary>
		/// <param name="excludeRoleIDs"></param>
		/// <param name="filter"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public abstract ShareCollection GetSharesBySearch(Guid[] excludeRoleIDs, ShareFilter filter, int pageNumber);

		/// <summary>
		/// 删除分享
		/// </summary>
		/// <param name="userID">登陆用户ID,如果为null是管理员删除</param>
		/// <param name="shareIDs">分享ID</param>
		public abstract DeleteResult DeleteShares(int operatorID, int[] shareIDs, IEnumerable<Guid> excludeRoleIDs);

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
		public abstract DeleteResult DeleteSharesBySearch(ShareFilter filter, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount);

		/// <summary>
		/// 获取用户收藏
		/// </summary>
		/// <param name="favOwnerID"></param>
		/// <param name="favType"></param>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalCount"></param>
		/// <returns></returns>
		public abstract ShareCollection GetUserFavorites(int favOwnerID, ShareType? favType, int pageNumber, int pageSize, ref int? totalCount);
		
        ///// <summary>
        ///// 为需要更新的关键字填充恢复关键信息
        ///// </summary>
        ///// <param name="processlist">要处理的列表</param>
        //public abstract void FillShareReverters(TextRevertable2Collection processlist);

        ///// <summary>
        ///// 更新关键字
        ///// </summary>
        ///// <param name="processlist">要处理的列表</param>
        //public abstract void UpdateShareKeywords(TextRevertable2Collection processlist);

        /// <summary>
        /// 添加一个分享
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="shareType"></param>
        /// <param name="privacyType"></param>
        /// <param name="content">简要内容</param>
        /// <param name="description">评论</param>
        public abstract int CreateShare(int userID, ShareType shareType, PrivacyType privacyType, string title, string url, string content, string description, string descriptionReverter, int targetID);


        /// <summary>
        /// 获取一个分享
        /// </summary>
        /// <param name="shareID"></param>
        /// <returns></returns>
        public abstract Share GetShare(int shareID);

        public abstract ShareCollection GetShares(IEnumerable<int> shareIDs);

        /// <summary>
        /// 获取用户分享
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="shareType">为null时 表示获取所有类型分享</param>
        /// <param name="privacyType"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public abstract ShareCollection GetUserShares(int userID, ShareType shareType, bool? includeFriendVisiable, int pageNumber, int pageSize, out int totalCount);

        /// <summary>
        /// 获取好友分享
        /// </summary>
        /// <param name="friendUserIDs">好友用户ID</param>
        /// <param name="shareType">为null时 表示获取所有类型分享</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public abstract ShareCollection GetFriendShares(int userID, ShareType shareType, int pageNumber, int pageSize, out int totalCount);

        public abstract ShareCollection GetUserCommentedShares(int userID, ShareType? shareType, int pageNumber, int pageSize);
        /// <summary>
        /// 获取所有用户分享
        /// </summary>
        /// <param name="shareType">为null时 表示获取所有类型分享</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public abstract ShareCollection GetAllUserShares(ShareType shareType, int pageNumber, int pageSize, out int totalCount);

        public abstract ShareCollection SearchShares(int pageNumber, ShareFilter filter, IEnumerable<Guid> excludeRoleIDs, ref int totalCount);

        public abstract DeleteResult DeleteSearchShares(ShareFilter filter, IEnumerable<Guid> excludeRoleIDs, bool getDeleteResult);

        //public abstract void TryUpdateKeyword(TextReverterCollection_Temp replaceKeywordContents);

        /// <summary>
        /// 为需要更新的关键字填充恢复关键信息
        /// </summary>
        /// <param name="processlist">要处理的列表</param>
        public abstract Revertable2Collection<Share> GetSharesWithReverters(IEnumerable<int> shareIDs);

        /// <summary>
        /// 为需要更新的关键字填充恢复关键信息
        /// </summary>
        /// <param name="processlist">要处理的列表</param>
        public abstract RevertableCollection<Share> GetSharesWithReverters1(IEnumerable<int> shareIDs);

        /// <summary>
        /// 更新关键字
        /// </summary>
        /// <param name="processlist">要处理的列表</param>
        public abstract void UpdateShareKeywords(Revertable2Collection<Share> processlist);
        /// <summary>
        /// 更新关键字
        /// </summary>
        /// <param name="processlist">要处理的列表</param>
        public abstract void UpdateShareKeywords1(RevertableCollection<Share> processlist);


        public abstract void AgreeShare(int userID, int shareID);

        public abstract void OpposeShare(int userID, int shareID);

        public abstract void ReShare(int userID, int shareID, PrivacyType privacyType, string subject, string description);

        public abstract Hashtable GetAgreeStates(int userID, int[] shareIDs);

        //public abstract CommentCollection GetShareComments(int shareID);

        public abstract ShareCollection GetFriendSharesOrderByRank(int userID, ShareType? shareType, DateTime? beginDate, int pageNumber, int pageSize);

        public abstract ShareCollection GetEveryoneSharesOrderByRank(ShareType? shareType, DateTime? beginDate, int pageSize, int pageNumber);

        public abstract ShareCollection GetHotShares(ShareType? shareType, DateTime? beginDate, HotShareSortType sortType, int pageNumber, int pageSize, out int totalCount);
        
        public abstract ShareCollection GetCommentedSharesOrderByRank(int userID, ShareType? shareType, int pageSize, int pageNumber);

        public abstract Share GetUserShares(int userShareID);
    }
}