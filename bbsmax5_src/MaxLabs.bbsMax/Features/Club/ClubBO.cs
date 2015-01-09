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

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax
{
	public class ClubBO : BOBase<ClubBO>
	{
		public string GetClubCategoryName(int categoryID)
		{
			return GetClubCategories().GetValue(categoryID).Name;
		}

		public ClubCategoryCollection GetClubCategories()
		{
			string cacheKey = GetCacheKeyForClubCategories();

			ClubCategoryCollection result = null;

			if (CacheUtil.TryGetValue<ClubCategoryCollection>(cacheKey, out result))
				return result;

			result = ClubDao.Instance.GetClubCategories();

			CacheUtil.Set<ClubCategoryCollection>(cacheKey, result);

			return result;
		}

		public bool CreateClub(int operatorID, int categoryID, string clubName, out int newClubID)
		{
			newClubID = 0;

			if (ValidateUserID(operatorID) == false)
				return false;

			if (ValidateClubCategoryID(categoryID) == false)
				return false;

			if (ValidateClubName(clubName) == false)
				return false;

			//TODO:检查是否达到创建群组数上限

			bool isApproced = true;
			string operatorIP = IPUtil.GetCurrentIP();

			CreateClubResult result = ClubDao.Instance.CreateClub(operatorID, categoryID, clubName, isApproced, operatorIP, out newClubID);

			//TODO:创建动态

			if (result == CreateClubResult.HasSameNameClub)
			{
				//TODO:抛错
				return false;
			}
				
			return true;
		}

		public Club GetClub(int clubID)
		{
			if (ValidateClubID(clubID) == false)
				return null;

			Club club = null;

			string cacheKey = GetCacheKeyForClub(clubID);

			if (CacheUtil.TryGetValue<Club>(cacheKey, out club) == false)
			{
				club = ClubDao.Instance.GetClub(clubID);

				if (club != null)
					CacheUtil.Set<Club>(cacheKey, club);
			}

			return club;
		}

		public bool CheckIsMember(ClubMemberStatus? status)
		{
			return !(status == null || status.Value == ClubMemberStatus.Invited || status.Value == ClubMemberStatus.WaitForApprove);
		}

		public bool CheckIsManager(ClubMemberStatus? status)
		{
			return status.Value == ClubMemberStatus.Manager || status.Value == ClubMemberStatus.ClubOwner;
		}

		public Club GetClubForVisit(AuthUser operatorUser, int clubID, out ClubMemberStatus? memberStatus)
		{
			memberStatus = null;

			Club club = GetClub(clubID);

			if (club == null)
				return null;

			if (club.AccessMode == ClubAccessMode.Freely)
				return club;

			memberStatus = GetClubMemberStatus(clubID, operatorUser.UserID);

			if (CheckIsMember(memberStatus))
				return club;
			else
			{
				//TODO:抛出错误
				return null;
			}
		}

		public Club GetClubForEdit(AuthUser operatorUser, int clubID)
		{
			Club club = GetClub(clubID);

			if (ValidateClubEditPermission(operatorUser.UserID, club) == false)
				return null;

			return club;
		}

		public ClubCollection GetAllClubs(int? categoryID, int pageSize, int pageNumber)
		{
			pageSize = pageSize > 0 ? pageSize : 1;
			pageNumber = pageNumber > 0 ? pageNumber : 1;

			return ClubDao.Instance.GetAllClubs(categoryID, pageSize, pageNumber);
		}

		public ClubCollection GetUserClubs(int operatorID, int pageSize, int pageNumber)
		{
			pageSize = pageSize > 0 ? pageSize : 1;
			pageNumber = pageNumber > 0 ? pageNumber : 1;

			return ClubDao.Instance.GetUserClubs(operatorID, pageSize, pageNumber);
		}

		public bool UpdateClub(AuthUser operatorUser, int clubID, string description, ClubJoinMethod joinMethod, ClubAccessMode accessMode, bool isNeedManager)
		{
			Club club = GetClubForEdit(operatorUser, clubID);

			if (club == null)
				return false;

			ClubDao.Instance.UpdateClub(clubID, description, joinMethod, accessMode, isNeedManager);

			ClearClubCache(clubID);

			return true;
		}

		public ClubMemberCollection GetClubMembersForEdit(AuthUser operatorUser, int clubID, ClubMemberStatus? status, int page, int pageSize)
		{
			Club club = GetClubForEdit(operatorUser, clubID);

			if (club == null)
				return null;

			page = page > 0 ? page : 1;
			
			return ClubDao.Instance.GetClubMembers(clubID, pageSize, page, status);
		}

		public ClubMemberCollection GetTopClubMembers(int clubID, int top)
		{
			top = top > 0 ? top : 1;

			return ClubDao.Instance.GetTopClubMembers(clubID, top);
		}

		public ClubMemberCollection GetTopClubManagers(int clubID, int top)
		{
			top = top > 0 ? top : 1;

			return ClubDao.Instance.GetTopClubManagers(clubID, top);
		}

		public bool InviteClubMembers(AuthUser operatorUser, int clubID, int[] userIDs)
		{
			Club club = GetClub(clubID);

			if (club == null)
				return false;

			ClubMemberStatus? status = GetClubMemberStatus(clubID, operatorUser.UserID);

			if (status == null)
			{
				return false;
			}

			ClubDao.Instance.InviteClubMembers(clubID, userIDs);

            //foreach (int userID in userIDs)
            //{
            //    NotifyBO.Instance.AddNotify(userID, new ClubInviteNotify(operatorUser.UserID, operatorUser.Username, clubID, club.Name));
            //}

			return true;
		}

		public void AcceptClubInvite(Club club, User user)
		{
			ClubDao.Instance.AcceptClubInvite(club.ClubID, user.UserID);

			//NotifyBO.Instance.AddNotify(club.UserID, new ClubJoinNotify(user.UserID, user.Username, club.ClubID, club.Name, false));
		}

		public void JoinClub(Club club, User user)
		{
			ClubDao.Instance.JoinClub(club.ClubID, user.UserID);

			//NotifyBO.Instance.AddNotify(club.UserID, new ClubJoinNotify(user.UserID, user.Username, club.ClubID, club.Name, false));
		}

		public void PetitionJoinClub(Club club, User user)
		{
			ClubDao.Instance.PetitionJoinClub(club.ClubID, user.UserID);

			//NotifyBO.Instance.AddNotify(club.UserID, new ClubJoinNotify(user.UserID, user.Username, club.ClubID, club.Name, true));
		}

		public void LeaveClub(int clubID, int userID)
		{
			ClubDao.Instance.LeaveClub(clubID, userID);

			ClearClubMemberStatusCache(userID, clubID);
		}

		public void PetitionClubManager(Club club, User user)
		{
			if (club == null || user == null)
				return;

			if (club.IsNeedManager == false)
				return;

			ClubDao.Instance.PetitionClubManager(club.ClubID, user.UserID);

			//NotifyBO.Instance.AddNotify(club.UserID, new ClubPetitionManagerNotify(user.UserID, user.Username, club.ClubID, club.Name));
		}

		public bool RemoveClubMembers(AuthUser operatorUser, int clubID, int[] userIDs)
		{
			Club club = GetClubForEdit(operatorUser, clubID);

			if (club == null)
				return false;

			ClubDao.Instance.RemoveClubMembers(clubID, userIDs);

			foreach (int userID in userIDs)
			{
                //ClearClubMemberStatusCache(userID, clubID);

                //NotifyBO.Instance.AddNotify(userID, new ClubRemoveMemberNotify(operatorUser.UserID, operatorUser.Username, clubID, club.Name));
			}

			return true;
		}

		public bool UpdateClubMemberStatus(AuthUser operatorUser, int clubID, int[] userIDs, ClubMemberStatus status)
		{
			Club club = GetClubForEdit(operatorUser, clubID);

			if (club == null)
				return false;

			ClubDao.Instance.UpdateClubMemberStatus(clubID, userIDs, status);

			foreach (int userID in userIDs)
			{
				ClearClubMemberStatusCache(userID, clubID);
			}

			return true;
		}

		public ClubMemberStatus? GetClubMemberStatus(int clubID, int userID)
		{
			if (userID <= 0)
				return null;

			string cachekey = GetCacheKeyForClubMemberStatus(userID, clubID);

			ClubMemberStatus cache = ClubMemberStatus.Normal;

			if(CacheUtil.TryGetValue<ClubMemberStatus>(cachekey, out cache))
				return cache;

			ClubMemberStatus? result = ClubDao.Instance.GetClubMemberStatus(clubID, userID);

			if (result != null)
				CacheUtil.Set<ClubMemberStatus>(cachekey, result.Value);

			return result;
		}

		private bool ValidateClubEditPermission(int operatorID, Club club)
		{
			if (CheckClubEditPermission(operatorID, club) == false)
			{
				ThrowError(new NoPermissionEditClubError());
				return false;
			}

			return true;
		}

		private bool CheckClubEditPermission(int operatorID, Club club)
		{
			if (club.UserID == operatorID)
				return true;
			else
			{
				ClubMemberStatus? status = GetClubMemberStatus(club.ClubID, operatorID);

				if (status == null)
					return false;

				if (status.Value == ClubMemberStatus.Manager)
					return true;
				
				return false;
			}
		}

		private bool ValidateClubCategoryID(int categoryID)
		{
			if (categoryID <= 0)
			{
				ThrowError(new Errors.InvalidParamError("categoryID"));
				return false;
			}

			return true;
		}

		private bool ValidateClubID(int clubID)
		{
			if (clubID <= 0)
			{
				ThrowError(new Errors.InvalidParamError("clubID"));
				return false;
			}

			return true;
		}

		private bool ValidateClubName(string clubName)
		{
			if (string.IsNullOrEmpty(clubName))
			{
				ThrowError(new Errors.ClubNameEmptyError("clubName"));
				return false;
			}

			//TODO:全空白检查和关键字处理

			return true;
		}

		private void ClearClubCache(int clubID)
		{
			CacheUtil.Remove(GetCacheKeyForClub(clubID));
		}

		private void ClearClubMemberStatusCache(int userID, int clubID)
		{
			CacheUtil.Remove(GetCacheKeyForClubMemberStatus(userID, clubID));
		}

		private string GetCacheKeyForClubCategories()
		{
			return "Club/Cats";
		}

		private string GetCacheKeyForClub(int clubID)
		{
			return "Club/" + clubID;
		}

		private string GetCacheKeyForClubMemberStatus(int userID, int clubID)
		{
			return "ClubMemberStatus/" + userID + "/" + clubID;
		}
	}
}