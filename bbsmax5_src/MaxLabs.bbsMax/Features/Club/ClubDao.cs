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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess
{
	public abstract class ClubDao : DaoBase<ClubDao>
	{
		/// <summary>
		/// 获取群组分类
		/// </summary>
		/// <returns></returns>
		public abstract ClubCategoryCollection GetClubCategories();

		/// <summary>
		/// 创建群组
		/// </summary>
		/// <param name="operatorID"></param>
		/// <param name="categoryID"></param>
		/// <param name="clubName"></param>
		/// <param name="isApproved"></param>
		/// <param name="operatorIP"></param>
		/// <param name="newClubID"></param>
		/// <returns></returns>
		public abstract CreateClubResult CreateClub(int operatorID, int categoryID, string clubName, bool isApproved, string operatorIP, out int newClubID);

		/// <summary>
		/// 获取所有群组
		/// </summary>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public abstract ClubCollection GetAllClubs(int? categoryID, int pageSize, int pageNumber);

		/// <summary>
		/// 获取用户的群组，包括加入的和创建的
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public abstract ClubCollection GetUserClubs(int userID, int pageSize, int pageNumber);

		/// <summary>
		/// 获取群组信息
		/// </summary>
		/// <param name="clubID"></param>
		/// <returns></returns>
		public abstract Club GetClub(int clubID);

		/// <summary>
		/// 更新群组信息
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="description"></param>
		/// <param name="joinMethod"></param>
		/// <param name="accessMode"></param>
		/// <param name="isNeedManager"></param>
		public abstract void UpdateClub(int clubID, string description, ClubJoinMethod joinMethod, ClubAccessMode accessMode, bool isNeedManager);

		/// <summary>
		/// 获取群组成员列表（普通）
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public abstract ClubMemberCollection GetTopClubMembers(int clubID, int top);

		/// <summary>
		/// 获取群组成员列表（普通）
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public abstract ClubMemberCollection GetTopClubManagers(int clubID, int top);

		/// <summary>
		/// 获取群组成员列表
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public abstract ClubMemberCollection GetClubMembers(int clubID, int pageSize, int pageNumber, ClubMemberStatus? status);

		/// <summary>
		/// 批量移除群组成员
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="userIDs"></param>
		public abstract void RemoveClubMembers(int clubID, int[] userIDs);

		/// <summary>
		/// 批量修改群组成员的身份
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="userIDs"></param>
		/// <param name="status"></param>
		public abstract void UpdateClubMemberStatus(int clubID, int[] userIDs, ClubMemberStatus status);

		/// <summary>
		/// 获取对某用户的群组邀请
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public abstract ClubCollection GetClubInvokes(int userID);

		/// <summary>
		/// 邀请群组成员
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="userID"></param>
		public abstract void InviteClubMembers(int clubID, int[] userIDs);

		/// <summary>
		/// 接收群组邀请
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="userID"></param>
		public abstract void AcceptClubInvite(int clubID, int userID);

		/// <summary>
		/// 申请加入群组
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="userID"></param>
		public abstract void JoinClub(int clubID, int userID);

		/// <summary>
		/// 申请加入群组
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="userID"></param>
		public abstract void PetitionJoinClub(int clubID, int userID);

		/// <summary>
		/// 申请成为群组管理员
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="userID"></param>
		public abstract void PetitionClubManager(int clubID, int userID);

		/// <summary>
		/// 忽略群组邀请
		/// </summary>
		/// <param name="userID"></param>
		public abstract void IgnoreAllClubInvokes(int userID);

		/// <summary>
		/// 退出群组
		/// </summary>
		/// <param name="clubID"></param>
		/// <param name="userID"></param>
		public abstract void LeaveClub(int clubID, int userID);

		public abstract ClubMemberStatus? GetClubMemberStatus(int clubID, int userID);
	}
}