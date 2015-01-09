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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax
{
    public class DoingBO : SpaceAppBO<DoingBO>
	{
		protected override SpacePermissionSet.Action UseAction
		{
			get { return SpacePermissionSet.Action.UseDoing; }
		}

		protected override BackendPermissions.ActionWithTarget ManageAction
		{
            get { return BackendPermissions.ActionWithTarget.Manage_Doing; }
		}

		#region =========↓记录↓====================================================================================================

        /// <summary>
        /// 仅在顶部下拉筐中更新记录时使用
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="operatorIP"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool UpdateDoing(AuthUser operatorUser, string operatorIP, string content)
        {
            int doingID;

            if (string.IsNullOrEmpty(content))
            {
                DoingDao.Instance.AddDoing(operatorUser.UserID, string.Empty, string.Empty, out doingID);

                operatorUser.Doing = string.Empty;
                operatorUser.DoingDate = DateTimeUtil.Now;

                return true;
            }
            else
                return CreateDoing(operatorUser.UserID, operatorIP, content);
        }

        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="operatorID">操作者ID</param>
        /// <param name="operatorIP">操作者IP</param>
        /// <param name="content">记录内容</param>
        /// <returns></returns>
        public bool CreateDoing(int operatorID, string operatorIP, string content)
        {
            bool validated = ValidateUserID(operatorID) && ValidateUsePermission<NoPermissionUseDoingError>(operatorID) && ValidateDoingContent(content);

            if (validated == false)
                return validated;

            //content = UbbUtil.UbbToHtml(content , ParserType.Simple);



            int todayCount = DoingDao.Instance.GetTodayPostDoingCount(operatorID, DateTimeUtil.Now.Date);

            int maxCount = AllSettings.Current.DoingSettings.EveryDayPostLimit.GetValue(operatorID);

            if (maxCount > 0 && todayCount >= maxCount)
            {
                ThrowError(new DoingPostCountLimitedError(maxCount));
                return false;
            }

            int doingID = 0;

            bool succeed = DoingPointAction.Instance.UpdateUserPoint(operatorID, DoingPointType.PostDoing, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
                    return false;

                DoingDao.Instance.AddDoing(operatorID, operatorIP, content, out doingID);

                return true;
            });


            KeywordReplaceRegulation keywordReg = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;
            content = keywordReg.Replace(content);


            FeedBO.Instance.CreateUpdateDoingFeed(operatorID, doingID, content);


            User user = UserBO.Instance.GetUserFromCache(operatorID);

            if (user != null)
            {
                user.Doing = content;
                user.DoingDate = DateTimeUtil.Now;
            }

            ClearCachedEveryoneData();

            ClearCachedUserData(operatorID);
#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                try
                {
                    Globals.PassportClient.PassportService.User_UpdateDoing(user.UserID, content);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                }
            }
#endif

            return true;
        }

		/**************************************
		 *       Get开头的函数获取数据        *
		 **************************************/

		public int GetPostDoingCount(int userID, DateTime beginDate, DateTime endDate)
		{
			return DoingDao.Instance.GetPostDoingCount(userID, beginDate, endDate);
		}

		public Doing GetDoing(int doingID)
		{
			Doing result = DoingDao.Instance.GetDoing(doingID);

			ProcessKeyword(result, ProcessKeywordMode.TryUpdateKeyword);

			return result;
		}

		public Doing GetDoingForDelete(int operatorID, int doingID)
		{
			if (ValidateUserID(operatorID) == false)
				return null;

			Doing doing = DoingDao.Instance.GetDoing(doingID);

			if (doing == null)
				return null;

			if (ValidateDoingDeletePermission(operatorID, doing) == false)
				return null;

			return doing;
		}

		/// <summary>
		/// 获取用户的记录包含当前页的记录的评论（通过记录的CommentList属性访问）
		/// </summary>
		/// <param name="visitorID">访问者ID</param>
		/// <param name="doingOwnerID">数据所有者ID</param>
		/// <param name="pageNumber">数据分页每页条数</param>
		/// <param name="pageSize">数据分页当前页码</param>
		/// <returns></returns>
		public DoingCollection GetUserDoingsWithComments(int visitorID, int doingOwnerID, int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			if (ValidateUserID(visitorID) == false || ValidateUserID(doingOwnerID) == false)
				return null;

			DoingCollection doings = null;

			DataAccessLevel dataAccessLevel = GetDataAccessLevel(visitorID, doingOwnerID);

			#region 获取TotalCount缓存

			int? totalCount = null;

			string totalCountCacheKey = GetCacheKeyForUserDoingsTotalCount(doingOwnerID, dataAccessLevel);

			bool totalCountCached = CacheUtil.TryGetValue(totalCountCacheKey, out totalCount);

			#endregion

			doings = DoingDao.Instance.GetUserDoingsWithComments(doingOwnerID, dataAccessLevel, pageNumber, pageSize, ref totalCount);

			#region 设置TotalCount缓存

			if (totalCountCached == false)
				CacheUtil.Set(totalCountCacheKey, totalCount);

			#endregion

			ProcessKeyword(doings, ProcessKeywordMode.TryUpdateKeyword);

			return doings;
		}

		/// <summary>
		/// 获取用户好友的记录包含当前页的记录的评论（通过记录的CommentList属性访问）
		/// </summary>
		/// <param name="friendOwnerID">好友所有者ID</param>
		/// <param name="pageNumber">数据分页每页条数</param>
		/// <param name="pageSize">数据分页当前页码</param>
		/// <returns></returns>
		public DoingCollection GetFriendDoingsWithComments(int friendOwnerID, int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			if (ValidateUserID(friendOwnerID) == false)
				return null;

			int? totalCount = null;

			DoingCollection doings = DoingDao.Instance.GetFriendDoingsWithComments(friendOwnerID, pageNumber, pageSize, ref totalCount);

			ProcessKeyword(doings, ProcessKeywordMode.TryUpdateKeyword);

			return doings;
		}

		/// <summary>
		/// 获取大家的记录包含当前页的记录的评论（通过记录的CommentList属性访问）
		/// </summary>
		/// <param name="visitorID">访问者ID</param>
		/// <param name="friendOwnerID">好友所有者ID</param>
		/// <param name="pageNumber">数据分页每页条数</param>
		/// <param name="pageSize">数据分页当前页码</param>
		/// <returns></returns>
		public DoingCollection GetEveryoneDoingsWithComments(int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			DoingCollection doings = null;

			#region 获取Doings缓存

			string doingsCacheKey = GetCacheKeyForEveryoneDoings(pageNumber, pageSize);

			bool doingsNeedCache = pageNumber <= Consts.ListCachePageCount;

			bool doingsCached = doingsNeedCache && CacheUtil.TryGetValue(doingsCacheKey, out doings);

			if (doingsCached)
				return doings;

			#endregion

			#region 获取TotalCount缓存

			int? totalCount = null;

			string totalCountCacheKey = GetCacheKeyForEveryoneDoingsTotalCount();

			bool totalCountCached = CacheUtil.TryGetValue(totalCountCacheKey, out totalCount);

			#endregion

			doings = DoingDao.Instance.GetEveryoneDoingsWithComments(pageNumber, pageSize, ref totalCount);

			#region 设置TotalCount缓存

			if (totalCountCached == false)
				CacheUtil.Set(totalCountCacheKey, totalCount);

			#endregion

			#region 设置Articles缓存

			if (doingsNeedCache)
				CacheUtil.Set(doingsCacheKey, doings);

			#endregion

			ProcessKeyword(doings, ProcessKeywordMode.TryUpdateKeyword);

			return doings;
		}

		/// <summary>
		/// 获取指定用户评论过的记录包含当前页的记录的评论（通过记录的CommentList属性访问）
		/// </summary>
		/// <param name="userID">评论者ID</param>
		/// <param name="pageNumber">数据分页每页条数</param>
		/// <param name="pageSize">数据分页当前页码</param>
		/// <returns></returns>
		public DoingCollection GetUserCommentedDoingsWithComments(int userID, int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			if (ValidateUserID(userID) == false)
				return null;

			int? totalCount = null;

			DoingCollection doings = DoingDao.Instance.GetUserCommentedDoingsWithComments(userID, pageNumber, pageSize, ref totalCount);

			ProcessKeyword(doings, ProcessKeywordMode.TryUpdateKeyword);

			return doings;
		}

		/// <summary>
		/// 获取记录用于管理
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="filter">文章搜索条件</param>
		/// <param name="pageNumber">数据分页页码</param>
		/// <returns></returns>
		public DoingCollection GetDoingsForAdmin(int operatorID, AdminDoingFilter filter, int pageNumber)
		{
			if (ValidateDoingAdminPermission(operatorID) == false)
				return null;

            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

			DoingCollection doings = DoingDao.Instance.GetDoingsBySearch(excludeRoleIDs, filter, pageNumber);

			ProcessKeyword(doings, ProcessKeywordMode.FillOriginalText);

			return doings;
		}

		/**************************************
		 *      Delete开头的函数删除数据      *
		 **************************************/

		/// <summary>
		/// 删除记录
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="doingID">所要删除的记录ID</param>
		public bool DeleteDoing(int operatorID, int doingID)
		{
			if (ValidateUserID(operatorID) == false)
				return false;

			Doing doing = DoingBO.Instance.GetDoing(doingID);

			if (ValidateDoingDeletePermission(operatorID, doing) == false)
				return false;

			DoingDao.Instance.DeleteDoing(doingID);

			ClearCachedEveryoneData();

			ClearCachedUserData(doing.UserID);

            FeedBO.Instance.DeleteFeed(AppActionType.UpdateDoing, doing.UserID);

			Logs.LogManager.LogOperation(
				new Doing_DeleteDoing(
					operatorID,
					UserBO.Instance.GetUser(operatorID).Name,
					IPUtil.GetCurrentIP(),
					doingID,
					doing.UserID,
					UserBO.Instance.GetUser(doing.UserID).Name
				)
			);

			return true;
		}

		/// <summary>
		/// 后台批量删除记录
		/// </summary>
		/// <param name="doingIDs"></param>
		public bool DeleteDoings(int operatorID, int[] doingIDs, bool isUpdatePoint)
		{
			bool result = ProcessDeleteDoings(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs)
			{
				return DoingDao.Instance.DeleteDoings(operatorID, doingIDs, excludeRoleIDs);
			});

			if (result)
			{
				Logs.LogManager.LogOperation(
					new Doing_DeleteDoingByIDs(
						operatorID,
						UserBO.Instance.GetUser(operatorID).Name,
						IPUtil.GetCurrentIP(),
						doingIDs
					)
				);
			}

			return result;
		}

		/// <summary>
		/// 删除搜索结果
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public bool DeleteDoingsForAdmin(int operatorID, AdminDoingFilter filter, bool isUpdatePoint, int deleteTopCount, out int deletedCount)
		{
			if (filter == null)
				throw new ArgumentNullException("filter");

			deletedCount = 0;

			int deleteCountTemp = 0;

			bool succeed = ProcessDeleteDoings(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs)
			{
				return DoingDao.Instance.DeleteDoingsBySearch(filter, excludeRoleIDs, deleteTopCount, out deleteCountTemp);
			});

			deletedCount = deleteCountTemp;

			return succeed;
		}

		private delegate DeleteResult DeleteDoingsCallback(Guid[] excludeRoleIDs);

		private bool ProcessDeleteDoings(int operatorID, bool isUpdatePoint, DeleteDoingsCallback deleteAction)
		{
			if (ValidateDoingAdminPermission(operatorID) == false)
				return false;

            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

			DeleteResult deleteResult = null;

			if (isUpdatePoint)
			{
				bool succeed = DoingPointAction.Instance.UpdateUsersPoints(delegate(PointActionManager.TryUpdateUserPointState state, out PointResultCollection<DoingPointType> pointResults)
				{
					pointResults = null;

					if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
						return false;

					deleteResult = deleteAction(excludeRoleIDs);

					if (deleteResult != null && deleteResult.Count > 0)
					{
						pointResults = new PointResultCollection<DoingPointType>();

						foreach (DeleteResultItem item in deleteResult)
						{
							pointResults.Add(item.UserID, item.UserID == operatorID ? DoingPointType.DoingWasDeletedBySelf : DoingPointType.DoingWasDeletedByAdmin, item.Count);
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

		#region =========↓检查↓====================================================================================================

		/**************************************
		 *  Validate开头的函数会抛出错误信息  *
		 **************************************/

		/// <summary>
		/// 验证记录的内容合法性
		/// </summary>
		/// <param name="content">记录的内容</param>
		/// <returns></returns>
		private bool ValidateDoingContent(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				Context.ThrowError(new EmptyDoingContentError("content"));
				return false;
			}

			if (StringUtil.GetByteCount(content) > Consts.Doing_Length)
			{
				Context.ThrowError(new InvalidDoingContentLengthError("content", content, Consts.Doing_Length));
				return false;
			}

            ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

            if (keywords.BannedKeywords.IsMatch(content, out keyword))
            {
                ThrowError(new DoingContentBannedKeywordsError("content", keyword));
                return false;
            }

			return true;
		}

		/// <summary>
		/// 验证操作者的记录管理权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <returns></returns>
		private bool ValidateDoingAdminPermission(int operatorID)
		{
			return ValidateAdminPermission<NoPermissionManageDoingError>(operatorID);
		}

		/// <summary>
		/// 验证操作者是否具有删除某条记录的全新
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="doing">所要删除的Doing</param>
		/// <returns>具有删除权限就返回true，否则返回false</returns>
		private bool ValidateDoingDeletePermission(int operatorID, Doing doing)
		{
			if (CheckDoingDeletePermission(operatorID, doing))
				return true;

			ThrowError(new NoPermissionDeleteDoingError());

			return false;
		}

		/**************************************
		 *    Check开头的函数只检查不抛错     *
		 **************************************/

		/// <summary>
		/// 验证操作者是否具有删除某条记录的全新
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="doing">所要删除的Doing</param>
		/// <returns>具有删除权限就返回true，否则返回false</returns>
		public bool CheckDoingDeletePermission(int operatorID, Doing doing)
		{
			return doing != null && CheckDeletePermission(operatorID, doing.UserID);
		}

		#endregion

		#region =========↓缓存↓====================================================================================================

		/**************************************
		 *   GetCacheKey开头的函获取缓存键    *
		 **************************************/

		/// <summary>
		/// 获取用户记录总条数的缓存键
		/// </summary>
		/// <param name="doingOwnerID">记录所有者的ID</param>
		/// <param name="dataAccessLevel">浏览者的数据访问级别</param>
		/// <returns></returns>
		private string GetCacheKeyForUserDoingsTotalCount(int doingOwnerID, DataAccessLevel dataAccessLevel)
		{
			return "Doing/" + doingOwnerID + "/Count/" + dataAccessLevel; 
		}

		/// <summary>
		/// 获取“大家的记录”的缓存键
		/// </summary>
		/// <param name="pageNumber">数据分页当前页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <returns></returns>
		private string GetCacheKeyForEveryoneDoings(int pageNumber, int pageSize)
		{
			return "Doing/All/" + pageSize + "/" + pageNumber;
		}

		/// <summary>
		/// 获取“大家的记录”总条数的缓存键
		/// </summary>
		/// <returns></returns>
		private string GetCacheKeyForEveryoneDoingsTotalCount()
		{
			return "Doing/All/DoingCount";
		}

		/**************************************
		 *      Clear开头的函数清除缓存       *
		 **************************************/

		/// <summary>
		/// 清除“大家的记录”的缓存数据
		/// </summary>
		internal void ClearCachedEveryoneData()
		{
			CacheUtil.RemoveBySearch("Doing/All/");
		}

		/// <summary>
		/// 清除按用户缓存的数据
		/// </summary>
		/// <param name="userID">用户ID</param>
		private void ClearCachedUserData(int userID)
		{
			CacheUtil.RemoveBySearch("Doing/" + userID);
		}

		#endregion

		#region =========↓关键字↓==================================================================================================

		private void ProcessKeyword(Doing doing, ProcessKeywordMode mode)
		{
            //更新关键字模式，如果这个记录并不需要处理，直接退出
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                if (AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.NeedUpdate<Doing>(doing) == false)
                    return;
            }

			DoingCollection doings = new DoingCollection();

			doings.Add(doing);

			ProcessKeyword(doings, mode);
		}

		private void ProcessKeyword(DoingCollection doings, ProcessKeywordMode mode)
		{
			if (doings.Count == 0)
				return;

			KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

			bool needProcess = false;

			//更新关键字模式，只在必要的情况下才取恢复信息并处理
			if (mode == ProcessKeywordMode.TryUpdateKeyword)
			{
				needProcess = keyword.NeedUpdate<Doing>(doings);
			}
			//填充原始内容模式，始终都要取恢复信息，但不处理
			else
			{
				needProcess = true;
			}

			if (needProcess)
			{
				RevertableCollection<Doing> doingsWithReverter = DoingDao.Instance.GetDoingsWithReverters(doings.GetKeys());

				if (doingsWithReverter != null)
				{
					if (keyword.Update(doingsWithReverter))
					{
						DoingDao.Instance.UpdateDoingKeywords(doingsWithReverter);
					}

					//将新数据填充到旧的列表
					doingsWithReverter.FillTo(doings);
				}
			}

			if (mode == ProcessKeywordMode.TryUpdateKeyword)
			{
				foreach (Doing doing in doings)
				{
					CommentBO.Instance.ProcessKeyword(doing.CommentList, mode);
				}
			}
		}

		#endregion

	}
}