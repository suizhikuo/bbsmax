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
//using MaxLabs.bbsMax.SystemMessages;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Ubb;

namespace MaxLabs.bbsMax
{
	public class CommentBO : SpaceAppBO<CommentBO>
	{
		private const string cacheKey_List_Space = "Comment/List/Space/{0}/{1}";

		protected override BackendPermissions.ActionWithTarget ManageAction
		{
            get { return BackendPermissions.ActionWithTarget.Manage_Comment; }
		}

		protected override SpacePermissionSet.Action UseAction
		{
			get { return SpacePermissionSet.Action.AddComment; }
		}

		/// <summary>
		/// 获取给某用户的最新评论
		/// </summary>
		/// <param name="targetUserID"></param>
		/// <param name="type"></param>
		/// <param name="top"></param>
		/// <returns></returns>
		public CommentCollection GetLastestCommentsForSomeone(int targetUserID, CommentType type, int top)
		{
			return CommentDao.Instance.GetLastestCommentsForSomeone(targetUserID, type, top);
		}

		/// <summary>
		/// 搜索评论
		/// </summary>
		/// <param name="operatorID"></param>
		/// <param name="filter"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public CommentCollection GetCommentsForAdmin(int operatorID, AdminCommentFilter filter, int pageNumber)
		{
			if (ValidateCommentAdminPermission(operatorID) == false)
				return null;

            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

			CommentCollection comments = CommentDao.Instance.GetCommentsBySearch(operatorID, excludeRoleIDs, filter, pageNumber);

			ProcessKeyword(comments, ProcessKeywordMode.FillOriginalText);

			return comments;
		}

		/// <summary>
		/// 删除搜索结果
		/// </summary>
		/// <param name="operatorID"></param>
		/// <param name="filter"></param>
		/// <param name="isDeleteArticle"></param>
		/// <param name="isUpdatePoint"></param>
		/// <param name="deleteTopCount"></param>
		/// <param name="deletedCount"></param>
		/// <returns></returns>
		public bool DeleteCommentsForAdmin(int operatorID, AdminCommentFilter filter, bool isUpdatePoint, int deleteTopCount, out int deletedCount)
		{
			if (filter == null)
				throw new ArgumentNullException("filter");

			deletedCount = 0;

			if (ValidateCommentAdminPermission(operatorID) == false)
				return false;

			int deleteCountTemp = 0;

			bool succeed = ProcessDeleteComments(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs)
			{
				return CommentDao.Instance.DeleteCommentsBySearch(filter, operatorID, excludeRoleIDs, deleteTopCount, out deleteCountTemp);
			});

			deletedCount = deleteCountTemp;

			return succeed;
		}

		private delegate DeleteResult DeleteCommentsCallback(Guid[] excludeRoleIDs);

		private bool ProcessDeleteComments(int operatorID, bool isUpdatePoint, DeleteCommentsCallback deleteAction)
		{
            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

			DeleteResult deleteResult = null;

			if (isUpdatePoint)
			{
				bool succeed = CommentPointAction.Instance.UpdateUsersPoints(delegate(PointActionManager.TryUpdateUserPointState state, out PointResultCollection<CommentPointType> pointResults)
				{
					pointResults = null;

					if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
						return false;

					deleteResult = deleteAction(excludeRoleIDs);

					if (deleteResult != null && deleteResult.Count > 0)
					{
						pointResults = new PointResultCollection<CommentPointType>();

						foreach (DeleteResultItem item in deleteResult)
						{
							pointResults.Add(item.UserID, item.UserID == operatorID ? CommentPointType.DeleteCommentBySelf : CommentPointType.DeleteCommentByAdmin, item.Count);
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

			return true;
		}

		#region=========↓检查↓=====================================================================================================

		/// <summary>
		/// 验证操作者的评论管理权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <returns></returns>
		private bool ValidateCommentAdminPermission(int operatorID)
		{
			return ValidateAdminPermission<NoPermissionManageCommentError>(operatorID);
		}

		private bool ValidateCommentEditPermission(int operatorID, Comment comment)
		{
			if (CheckCommentEditPermission(operatorID, comment) == false)
			{
				ThrowError(new NoPermissionEditCommentError());
				return false;
			}

			return true;
		}

		public bool CheckCommentAddPermission(int operatorID)
		{
			return Permission.Can(operatorID, UseAction);
		}

		public bool CheckCommentEditPermission(int operatorID, Comment comment)
		{
			if (comment == null)
				return false;

			return CheckEditPermission(operatorID, comment.UserID, comment.LastEditUserID);
		}

		/// <summary>
		/// 检查操作者的评论管理权限
		/// </summary>
		/// <param name="operatorID"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public bool CheckCommentDeletePermission(int operatorID, Comment comment)
		{
			if (comment == null)
				return false;

			return comment.TargetUserID == operatorID || CheckDeletePermission(operatorID, comment.UserID, comment.LastEditUserID);
		}

		#endregion

		#region=========↓关键字↓==================================================================================================

		public void ProcessKeyword(Comment comment, ProcessKeywordMode mode)
		{
            //更新关键字模式，如果这个评论并不需要处理，直接退出
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                if (AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.NeedUpdate<Comment>(comment) == false)
                    return;
            }

			CommentCollection comments = new CommentCollection();

			comments.Add(comment);

			ProcessKeyword(comments, mode);
		}

        public void ProcessKeyword(CommentCollection comments, ProcessKeywordMode mode)
		{
			if (comments.Count == 0)
				return;

			KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

			bool needProcess = false;

			//更新关键字模式，只在必要的情况下才取恢复信息并处理
			if (mode == ProcessKeywordMode.TryUpdateKeyword)
			{
				needProcess = keyword.NeedUpdate<Comment>(comments);
			}
			//填充原始内容模式，始终都要取恢复信息，但不处理
			else
			{
				needProcess = true;
			}

			if (needProcess)
			{
				RevertableCollection<Comment> commentsWithReverter = CommentDao.Instance.GetCommentsWithReverters(comments.GetKeys());

				if (commentsWithReverter != null)
				{
					if (keyword.Update(commentsWithReverter))
					{
						CommentDao.Instance.UpdateCommentKeywords(commentsWithReverter);
					}

					//将新数据填充到旧的列表
					commentsWithReverter.FillTo(comments);
				}
			}
		}

		#endregion

		#region =========↓未整理的代码↓============================================================================================

        public bool ReplyComment(AuthUser operatorUser, int targetID, int commentID, int targetUserID, CommentType type, string content, string createIP, out int newCommentId, out string newCommentContent)
        {
            bool result = AddComment(operatorUser, targetID, commentID, type, content, createIP, true, targetUserID, out newCommentId, out newCommentContent);

            return result;
        }

        public bool AddComment(AuthUser operatorUser, int targetID, int commentID, CommentType type, string content, string createIP, out int newCommentId)
		{
            string newCommentContent;

            return AddComment(operatorUser, targetID, commentID, type, content, createIP, false, 0, out newCommentId, out newCommentContent);
		}

		/// <summary>
		/// 添加评论 并更新缓存
		/// </summary>
		/// <param name="userID">评论者ID</param>
		/// <param name="targetID">被评论的记录、日志、相册的ID</param>
		/// <param name="commentID">被评论的评论ID</param>
		/// <param name="type">评论类型 记录、日志、相册</param>
		/// <param name="content"></param>
		/// <param name="createIP"></param>
		/// <returns></returns>
		public bool AddComment(AuthUser operatorUser, int targetID, int commentID, CommentType type, string content, string createIP, bool isReply, int replyTargetUserID, out int newCommentId, out string newCommentContent)
		{

            Notify notify;

			newCommentId = 0;
			newCommentContent = string.Empty;

			if (Permission.Can(operatorUser, SpacePermissionSet.Action.AddComment) == false)
			{
				ThrowError<NoPermissionAddCommentError>(new NoPermissionAddCommentError());
				return false;
			}

			if (targetID < 0)
			{
				ThrowError(new InvalidParamError("targetID"));
				return false;
			}

			if (commentID < 0)
			{
				ThrowError(new InvalidParamError("commentID"));
				return false;
			}

			if (string.IsNullOrEmpty(content))
			{
				ThrowError(new EmptyCommentError("content"));
				return false;
			}

			if (StringUtil.GetByteCount(content) > Consts.Comment_Length)
			{
				ThrowError(new InvalidCommentLengthError("content", content, Consts.Comment_Length));
				return false;
			}

            

			int commentTargetUserID = GetCommentTargetUserID(targetID, type);
            if (FriendBO.Instance.MyInBlacklist(operatorUser.UserID, commentTargetUserID))
			{
				ThrowError(new PrivacyError());
				return false;
			}

			bool isApproved = true;
			
			//string reverter, version;
			
			//content = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.Replace(content, out version, out reverter);

			CommentPointType commentPointType = CommentPointType.AddApprovedComment;


            KeywordReplaceRegulation keywordReg = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;
            content = keywordReg.Replace(content);

            string keyword = null;
			if (AllSettings.Current.ContentKeywordSettings.BannedKeywords.IsMatch(content, out keyword))
			{
				Context.ThrowError(new KeywordBannedError("content", keyword));
				return false;
			}


			if (AllSettings.Current.ContentKeywordSettings.ApprovedKeywords.IsMatch(content))
			{
				isApproved = false;
                commentPointType = CommentPointType.AddNoApprovedComment;
			}

			if (StringUtil.GetByteCount(content) > Consts.Comment_Length)
			{
				Context.ThrowError(new InvalidCommentLengthError("content", content, Consts.Comment_Length));
				return false;
			}

			int targetUserID = 0;

			int tempCommentID = 0;

			content = CommentUbbParser.ParseForSave(content);
			newCommentContent = content;

            bool success = CommentPointAction.Instance.UpdateUserPoint(operatorUser.UserID, commentPointType, delegate(PointActionManager.TryUpdateUserPointState state)
			   {
				   if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
				   {
					   return false;
				   }
				   else
				   {
                       //添加评论并发送动态与通知
                       switch (type)
                       {
                           case CommentType.Board:
                               SimpleUser user = UserBO.Instance.GetSimpleUser(targetID);

                               if (user == null)
                               {
                                   ThrowError(new UserNotExistsError("targetID", targetID));
                                   return false;
                               }

                               CommentDao.Instance.AddComment(operatorUser.UserID, targetID, type, isApproved, content, /* reverter, */ createIP, out targetUserID, out tempCommentID);
                               FeedBO.Instance.CreateLeveMessageFeed(operatorUser.UserID, targetID);

                               if (isReply && operatorUser.UserID != replyTargetUserID)
                               {
                                   notify = new BoardCommentNotify(operatorUser.UserID, tempCommentID, isReply, targetID);
                                   notify.UserID = replyTargetUserID;
                                   NotifyBO.Instance.AddNotify(operatorUser, notify);
                               }
                               else if (targetID != operatorUser.UserID)
                               {
                                   notify = new BoardCommentNotify(operatorUser.UserID, tempCommentID, false, targetID);
                                   notify.UserID = targetID;
                                   NotifyBO.Instance.AddNotify(operatorUser, notify);
                               }

                               CacheUtil.RemoveBySearch(string.Format(cacheKey_List_Space, targetID, type));
                               break;

                           case CommentType.Blog:
                               BlogArticle article = BlogBO.Instance.GetBlogArticle(targetID);

                               if (article == null)
                               {
                                   ThrowError(new NotExistsAlbumError(targetID));
                                   return false;
                               }

                               if (!article.EnableComment)
                               {
                                   ThrowError(new PrivacyError());
                                   return false;
                               }
                               else
                               {
                                   if (article.UserID != operatorUser.UserID)//如果不是作者评论自己的 给作者加积分
                                   {
                                       CommentDao.Instance.AddComment(operatorUser.UserID, targetID, type, isApproved, content, /* reverter, */ createIP, out targetUserID, out tempCommentID);

                                       success = BlogPointAction.Instance.UpdateUserPoint(article.UserID, BlogPointType.ArticleWasCommented);

                                       if (success == false)
                                           return false;
                                   }
                                   else
                                       CommentDao.Instance.AddComment(operatorUser.UserID, targetID, type, isApproved, content, /* reverter, */ createIP, out targetUserID, out tempCommentID);
                               }

                               FeedBO.Instance.CreatBlogCommentFeed(operatorUser.UserID, article.UserID, targetID, article.Subject);

                               if (isReply && operatorUser.UserID != replyTargetUserID)
                               {
                                   notify = new BlogCommentNotify(operatorUser.UserID, article.Subject, targetID, tempCommentID, isReply, article.UserID);
                                   notify.UserID = replyTargetUserID;
                                   NotifyBO.Instance.AddNotify(operatorUser, notify);
                               }
                               else if (article.UserID != operatorUser.UserID)
                               {
                                   notify = new BlogCommentNotify(operatorUser.UserID, article.Subject, targetID, tempCommentID, false, article.UserID);
                                   notify.UserID = article.UserID;
                                   NotifyBO.Instance.AddNotify(operatorUser, notify);
                               }
                               break;

                           case CommentType.Doing:
                               Doing doing = DoingBO.Instance.GetDoing(targetID);

                               if (doing == null)
                               {
                                   ThrowError(new NotExistsDoingError(targetID));
                                   return false;
                               }

                               CommentDao.Instance.AddComment(operatorUser.UserID, targetID, type, isApproved, content, /* reverter, */ createIP, out targetUserID, out tempCommentID);

                               if (isReply && operatorUser.UserID != replyTargetUserID)
                               {
                                   notify = new DoingPostNotify(operatorUser.UserID, doing.Content, doing.ID, tempCommentID, isReply, doing.UserID);
                                   notify.UserID = replyTargetUserID;
                                   NotifyBO.Instance.AddNotify(operatorUser, notify);
                               }
                               else if (doing.UserID != operatorUser.UserID)
                               {
                                   notify = new DoingPostNotify(operatorUser.UserID, doing.Content, doing.ID, tempCommentID, false, doing.UserID);
                                   notify.UserID = doing.UserID;
                                   NotifyBO.Instance.AddNotify(operatorUser, notify);

                               }
							   DoingBO.Instance.ClearCachedEveryoneData();
							   break;

						   case CommentType.Share:
							   Share share = ShareBO.Instance.GetUserShare(targetID);

							   if (share == null)
							   {
                                   ThrowError(new NotExistsShareError(targetID));
								   return false;
							   }

                               if (share.UserID != operatorUser.UserID)//如果不是分享作者评论自己的分享 给分享作者加积分
                               {
                                   CommentDao.Instance.AddComment(operatorUser.UserID, targetID, type, isApproved, content, /* reverter, */ createIP, out targetUserID, out tempCommentID);
								   success = SharePointAction.Instance.UpdateUserPoint(share.UserID, SharePointType.ShareWasCommeted);

								   if (!success)
									   return false;
							   }
							   else
							   {
                                   CommentDao.Instance.AddComment(operatorUser.UserID, targetID, type, isApproved, content, /* reverter, */ createIP, out targetUserID, out tempCommentID);
							   }

                               ShareBO.Instance.ClearCachedEveryoneData();

                               FeedBO.Instance.CreatShareCommentFeed(operatorUser.UserID, share.UserID, targetID, share.Type);

                               if (isReply && operatorUser.UserID != replyTargetUserID)
                               {
                                   notify = new SharePostNotify(operatorUser.UserID, targetID, tempCommentID, isReply, share.UserID);
                                   notify.UserID = replyTargetUserID;
                                   NotifyBO.Instance.AddNotify(operatorUser, notify);
                               }
                               else if (share.UserID != operatorUser.UserID)
                               {
                                   notify = new SharePostNotify(operatorUser.UserID, targetID, tempCommentID, false, share.UserID);
                                   notify.UserID = share.UserID;
                                   NotifyBO.Instance.AddNotify(operatorUser, notify);
                               }

							   break;

						   case CommentType.Photo:
							   Photo photo = AlbumBO.Instance.GetPhoto(targetID);

                               if (photo == null)
                               {
                                   ThrowError(new NotExistsPhotoError(targetID));
                                   return false;
                               }

                               CommentDao.Instance.AddComment(operatorUser.UserID, targetID, type, isApproved, content, /* reverter, */ createIP, out targetUserID, out tempCommentID);
                               FeedBO.Instance.CreatPictureCommentFeed(operatorUser.UserID, targetUserID, targetID, photo.ThumbSrc, photo.Name, content);

                               if (isReply && operatorUser.UserID != replyTargetUserID)
                               {
                                   notify = new PhotoCommentNotify(operatorUser.UserID, targetID, photo.Name, tempCommentID, isReply, photo.UserID);
                                   notify.UserID = replyTargetUserID;

                                   NotifyBO.Instance.AddNotify(operatorUser, notify);
                               }
                               else if (photo.UserID != operatorUser.UserID)
                               {
                                   notify = new PhotoCommentNotify(operatorUser.UserID, targetID, photo.Name, tempCommentID, false, photo.UserID);
                                   notify.UserID = photo.UserID;
                                   NotifyBO.Instance.AddNotify(operatorUser, notify);
                               }

							   break;
					   }
					   return true;
				   }
			   });

			if (success == false)
				return false;

			newCommentId = tempCommentID;

			//更新热度
			if (targetUserID != 0)
			{
				HotType hotType = HotType.Comment;
				
                if (type == CommentType.Board)
					hotType = HotType.Messag;

                FriendBO.Instance.UpdateFriendHot(operatorUser.UserID, hotType, targetUserID);
			}

            if (isApproved == false)
            {
                Context.ThrowError(new UnapprovedCommentError());
                return false;
            }

			return true;
		}

		public bool RemoveComment(int operatorUserID, int commentID)
		{
			int[] commentIDs = new int[] { commentID };
			return RemoveComments(operatorUserID, commentIDs, true);
		}

		/*
		/// <summary>
		/// 删除评论 包括删除自己的评论 别人对自己应用的评论 并更新缓存
		/// </summary>
		/// <param name="commentID"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool RemoveComment(int userID, int commentID, CommentType type)
		{
			if (commentID <= 0)
			{
				ThrowError(new InvalidParamError("commentID"));
				return false;
			}

			int commentUserID;

			CommentPointManager.Instance.UpdateUserPoint(userID, CommentPMType.DeleteComment, delegate(UserBO.TryUpdateUserPointState state)
			{
				return true;
			});

			CommentDao.Instance.DeleteComment(userID, commentID, out commentUserID);

			if (type == CommentType.Doing)
				CacheUtil.RemoveBySearch("Doing/List/All");
			if (type == CommentType.Board)
				CacheUtil.RemoveBySearch(string.Format(cacheKey_List_Space, commentUserID, type));

			//TODO;对comment用户的积分操作

			return true;
		}
		*/
		/// <summary>
		/// 删除评论单条或批量 并更新缓存 用于后台
		/// </summary>
		/// <param name="commentIDs"></param>
		public bool RemoveComments(int operatorUserID, IEnumerable<int> commentIDs, bool isUpdatePoint)
		{

			if (ValidateUtil.HasItems<int>(commentIDs) == false)
			{
				ThrowError(new NotSelectedCommentsError("commentIDs"));
				return false;
			}

			CommentCollection comments = CommentDao.Instance.GetComments(commentIDs);

			Dictionary<int, int> deleteResults = new Dictionary<int, int>();

			List<int> deleteCommentIDs = new List<int>();
			foreach (Comment comment in comments)
			{
				if (comment.UserID == operatorUserID || comment.TargetUserID == operatorUserID)//是自己的 或者 是别人评论自己的可以删除
				{
					deleteCommentIDs.Add(comment.CommentID);
					if (deleteResults.ContainsKey(comment.UserID))
					{
						deleteResults[comment.UserID] += 1;
					}
					else
						deleteResults.Add(comment.UserID, 1);
				}
				else//不是自己的判断权限
				{
                    if (ManagePermission.Can(operatorUserID, BackendPermissions.ActionWithTarget.Manage_Comment, comment.UserID, comment.LastEditUserID) == false)
					{
						//没权限 跳过
					}
					else
					{
						deleteCommentIDs.Add(comment.CommentID);
						if (deleteResults.ContainsKey(comment.UserID))
						{
							deleteResults[comment.UserID] += 1;
						}
						else
							deleteResults.Add(comment.UserID, 1);
					}
				}
			}

			if (deleteResults.Count == 0)
			{
				ThrowError<NoPermissionDeleteCommentError>(new NoPermissionDeleteCommentError());
				return false;
			}

			if (isUpdatePoint)
			{
				CommentPointType pointType;
				if (deleteResults.Count == 1 && deleteResults.ContainsKey(operatorUserID))//自己删除 
				{
					pointType = CommentPointType.DeleteCommentBySelf;
				}
				else
					pointType = CommentPointType.DeleteCommentByAdmin;

				bool success = CommentPointAction.Instance.UpdateUsersPoint(deleteResults, pointType, delegate(PointActionManager.TryUpdateUserPointState state)
				{
					if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
					{
						CommentDao.Instance.DeleteComments(deleteCommentIDs);
						return true;
					}
					else
						return false;
				});

				if (!success)
					return false;
			}
			else
			{
				CommentDao.Instance.DeleteComments(deleteCommentIDs);
			}


			if (comments.Count == 1)
			{
				if (comments[0].Type == CommentType.Doing)
					CacheUtil.RemoveBySearch("Doing/List/All");
				if (comments[0].Type == CommentType.Board)
					CacheUtil.RemoveBySearch(string.Format(cacheKey_List_Space, comments[0].UserID, CommentType.Board));

                FeedBO.Instance.DeleteFeed(AppActionType.AddComment, comments[0].TargetID, comments[0].UserID);

			}
			else
			{
				//TODO:优化
				CacheUtil.RemoveBySearch("Doing/List/All");
				CacheUtil.RemoveBySearch("Comment/List/Space");

				Dictionary<int, List<int>> deleteFeedIDs = new Dictionary<int,List<int>>();

				foreach(Comment comment in comments)
				{
					if(deleteFeedIDs.ContainsKey(comment.TargetID) == false)
						deleteFeedIDs.Add(comment.TargetID, new List<int>(new int[]{ comment.UserID }));
					else
						deleteFeedIDs[comment.TargetID].Add(comment.UserID);
				}

				FeedBO.Instance.DeleteFeeds(AppActionType.AddComment, deleteFeedIDs);
			}
			return true;
		}

		///// <summary>
		///// 删除搜索结果
		///// </summary>
		///// <param name="filter"></param>
		///// <returns></returns>
		//public bool RemoveCommentsBySearch(int operatorUserID, AdminCommentFilter filter, bool updatePoint)
		//{
		//    Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorUserID);
		//    //DeleteResult deleteResult = CommentDao.Instance.DeleteCommentsBySearch(filter, operatorUserID, excludeRoleIDs);

		//    DeleteResult deleteResult = null;
		//    if (updatePoint)
		//    {
		//        bool success = CommentPointAction.Instance.UpdateUsersPoint(CommentPointType.DeleteCommentByAdmin, delegate(PointActionManager.TryUpdateUserPointState state, out Dictionary<int, int> userIDs)
		//        {
		//            if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
		//            {
		//                deleteResult = CommentDao.Instance.DeleteCommentsByFilter(filter, operatorUserID, excludeRoleIDs, true);
		//                userIDs = new Dictionary<int, int>();
		//                foreach (DeleteResultItem item in deleteResult)
		//                {
		//                    userIDs.Add(item.UserID, item.Count);
		//                }
		//                return true;
		//            }
		//            else
		//            {
		//                userIDs = null;
		//                return false;
		//            }
		//        });

		//        if (!success)
		//            return false;
		//    }
		//    else
		//    {
		//        deleteResult = CommentDao.Instance.DeleteCommentsByFilter(filter, operatorUserID, excludeRoleIDs, true);
		//    }
		//    foreach (DeleteResultItem item in deleteResult)
		//    {
		//        UserBO.Instance.RemoveUserCache(item.UserID);
		//    }


		//    CacheUtil.RemoveBySearch("Doing/List/All");
		//    CacheUtil.RemoveBySearch("Comment/List/Space");

		//    return true;
		//}

		///// <summary>
		///// 更新关键字版本和评论内容
		///// </summary>
		///// <param name="comments"></param>
		//protected void TryUpdateKeyword(CommentCollection comments)
		//{
		//    TextRevertableCollection processlist = new TextRevertableCollection();

		//    KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;
		//    foreach (Comment comment in comments)
		//    {
		//        if (keyword.NeedUpdateText(comment))
		//        {
		//            processlist.Add(comment);
		//        }
		//    }

		//    if (processlist.Count > 0)
		//    {
		//        CommentDao.Instance.FillCommentReverters(processlist);

		//        keyword.Update(processlist);

		//        CommentDao.Instance.UpdateCommentKeywords(processlist);
		//    }
		//}

		/// <summary>
		/// 审核评论单条或多条 用于后台
		/// </summary>
		/// <param name="commentIDs"></param>
		public bool ApproveComments(int operatorUserID, IEnumerable<int> commentIDs)
		{
			if (ValidateUtil.HasItems<int>(commentIDs) == false)
			{
				ThrowError(new NotSelectedCommentsError("commentIDs"));
				return false;
			}

			CommentCollection comments = CommentDao.Instance.GetComments(commentIDs);
			if (comments.Count == 0)
			{
				return true;
			}


			List<int> canApproveCommentIDs = new List<int>();

			foreach (Comment comment in comments)
			{
                if (ManagePermission.Can(operatorUserID, BackendPermissions.ActionWithTarget.Manage_Comment, comment.UserID))
				{
					canApproveCommentIDs.Add(comment.CommentID);
				}
			}
			if (canApproveCommentIDs.Count == 0)
			{
				ThrowError<NoPermissionApproveCommentError>(new NoPermissionApproveCommentError());
				return false;
			}

			CommentDao.Instance.ApproveComments(canApproveCommentIDs);

			//TODO:优化
			CacheUtil.RemoveBySearch("Doing/List/All");
			CacheUtil.RemoveBySearch("Comment/List/Space");

			return true;
		}

		/// <summary>
		/// 评论编辑
		/// </summary>
		/// <param name="commentID"></param>
		/// <param name="content"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool ModifyComment(int operatorUserID, int commentID, string content, CommentType type, out string newContent)
		{
            newContent = content;

			if (commentID <= 0)
			{
				ThrowError(new InvalidParamError("commentID"));
				return false;
			}

			Comment comment = GetCommentByID(commentID);

			if (comment == null)
			{
				ThrowError<InvalidParamError>(new InvalidParamError("commentID"));
				return false;
			}

			if (ValidateCommentEditPermission(operatorUserID, comment) == false)
			{
				return false;
			}

			if (string.IsNullOrEmpty(content))
			{
				Context.ThrowError(new EmptyCommentError("content"));
				return false;
			}
			if (StringUtil.GetByteCount(content) > Consts.Comment_Length)
			{
				Context.ThrowError(new InvalidCommentLengthError("content", content, Consts.Comment_Length));
				return false;
			}

			bool isApproved = true;
			string reverter, version;
			content = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.Replace(content, out version, out reverter);

            string keyword = null;

			if (AllSettings.Current.ContentKeywordSettings.BannedKeywords.IsMatch(content, out keyword))
			{
				Context.ThrowError(new KeywordBannedError("content", keyword));
				return false;
			}
			else if (AllSettings.Current.ContentKeywordSettings.ApprovedKeywords.IsMatch(content))
				isApproved = false;

			content = CommentUbbParser.ParseForSave(content);

			if (StringUtil.GetByteCount(content) > Consts.Comment_Length)
			{
				Context.ThrowError(new InvalidCommentLengthError("content", content, Consts.Comment_Length));
				return false;
			}

			int targetID;

			CommentDao.Instance.UpdateComment(commentID, operatorUserID, isApproved, content, reverter, out targetID);

			if (type == CommentType.Doing)
				CacheUtil.RemoveBySearch("Doing/List/All");
			if (type == CommentType.Board)//更新留言所在用户缓存的数据而不是留言的用户！
				CacheUtil.RemoveBySearch(string.Format(cacheKey_List_Space, targetID, type));

            newContent = content;

            if (isApproved == false)
            {
                ThrowError<UnapprovedCommentError>(new UnapprovedCommentError());
                return false;
            }

			return true;
		}

		/// <summary>
		/// 取出某人回复的记录
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="type"></param>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalCount"></param>
		/// <returns></returns>
		public CommentCollection GetComments(int userID, CommentType type, int pageNumber, int pageSize, out int totalCount)
		{
			totalCount = 0;

			if (userID <= 0)
			{
				ThrowError(new InvalidParamError("userID"));
				return new CommentCollection();
			}

			if (pageNumber < 1)
				pageNumber = 1;

			CommentCollection comments = CommentDao.Instance.GetCommentsByUserID(userID, type, pageNumber, pageSize, out totalCount);

			ProcessKeyword(comments, ProcessKeywordMode.TryUpdateKeyword);

			return comments;
		}

		/// <summary>
		/// 评论分页
		/// </summary>
		/// <param name="targetID"></param>
		/// <param name="type"></param>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <param name="orderType"></param>
		/// <param name="totalCount"></param>
		/// <returns></returns>
		public CommentCollection GetComments(int targetID, CommentType type, int pageNumber, int pageSize, bool isDesc, out int totalCount)
		{
			totalCount = 0;

			if (targetID <= 0)
			{
				ThrowError(new InvalidParamError("targetID"));
				return new CommentCollection();
			}

			if (pageNumber < 1)
				pageNumber = 1;
			CommentCollection comments = CommentDao.Instance.GetCommentsByTargetID(targetID, type, pageNumber, pageSize, isDesc, out totalCount);

			ProcessKeyword(comments, ProcessKeywordMode.TryUpdateKeyword);

			return comments;
		}

		/// <summary>
		/// 取指定对象评论 条数
		/// </summary>
		/// <param name="targetID"></param>
		/// <param name="type"></param>
		/// <param name="count"></param>
		/// <param name="totalCount"></param>
		/// <returns></returns>
		public CommentCollection GetComments(int targetID, CommentType type, int count)
		{
			if (targetID <= 0)
			{
				ThrowError(new InvalidParamError("targetID"));
				return new CommentCollection();
			}
			string key = string.Format(cacheKey_List_Space, targetID, type);
			CommentCollection comments = new CommentCollection();
			if (CacheUtil.TryGetValue<CommentCollection>(key, out comments))
				return comments;

			comments = CommentDao.Instance.GetComments(targetID, type, count);

			CacheUtil.Set<CommentCollection>(key, comments);

			return comments;
		}

		//public CommentCollection SearchComments(int operatorUserID, AdminCommentFilter filter, int pageNumber, out int totalCount)
		//{
		//    totalCount = 0;

		//    if (pageNumber < 1)
		//        pageNumber = 1;

		//    Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorUserID);

		//    CommentCollection comments = CommentDao.Instance.GetCommentsByFilter(filter, operatorUserID, excludeRoleIDs, pageNumber, out totalCount);

		//    return comments;
		//}

		/// <summary>
		/// 根据评论ID取一条评论 只用于编辑 会还原到替换前的内容
		/// </summary>
		/// <param name="commentID"></param>
		/// <returns></returns>
		public Comment GetComment(int commentID)
		{
			if (commentID <= 0)
			{
				ThrowError(new InvalidParamError("commentID"));
				return null;
			}

			Comment comment = CommentDao.Instance.GetComment(commentID);

			return comment;
		}

		public Comment GetCommentForEdit(int operatorID, int commentID)
		{
			if (ValidateUserID(operatorID) == false)
				return null;

			if (commentID <= 0)
			{
				ThrowError(new InvalidParamError("commentID"));
				return null;
			}

			Comment comment = CommentDao.Instance.GetComment(commentID);

			if (comment == null)
				return null;

			if (CheckCommentEditPermission(operatorID, comment) == false)
				return null;

            comment.Content = CommentUbbParser.ParseForEdit(comment.Content);

			return comment;
		}

		public Comment GetCommentForDelete(int operatorID, int commentID)
		{
			if (ValidateUserID(operatorID) == false)
				return null;

			if(commentID <=0)
			{
				ThrowError(new InvalidParamError("commentID"));
				return null;
			}

			Comment comment = CommentDao.Instance.GetComment(commentID);

			if (comment == null)
				return null;

			if (CheckCommentDeletePermission(operatorID, comment) == false)
				return null;

			return comment;
		}

		public Comment GetCommentByID(int commentID)
		{
			if (commentID <= 0)
			{
				ThrowError(new InvalidParamError("commentID"));
				return null;
			}

			Comment comment = CommentDao.Instance.GetComment(commentID);

			return comment;
		}

		protected int GetCommentTargetUserID(int targetID, CommentType type)
		{
			int targetUserID = 0;

			switch (type)
			{
				case CommentType.Blog:
					targetUserID = BlogBO.Instance.GetBlogArticle(targetID).UserID;
					break;
				case CommentType.Board:
					targetUserID = targetID;
					break;
				case CommentType.Doing:
					targetUserID = DoingBO.Instance.GetDoing(targetID).UserID;
					break;
				case CommentType.Photo:
                    Photo p = AlbumBO.Instance.GetPhoto(targetID);
                    if (p != null)
                        targetUserID = p.UserID;
                    else
                        targetUserID = 0;
					//targetUserID = AlbumBO.Instance.GetAlbum(targetID).UserID;
					break;
				case CommentType.Share:
					targetUserID = ShareBO.Instance.GetUserShare(targetID).UserID;
					break;
				default:
					break;
			}

			return targetUserID;
		}

		#endregion



        public CommentType GetCommentType(Guid appID, int actionType)
        {
            if (appID == Consts.App_BasicAppID)//内置的应用
            {
                AppActionType type = (AppActionType)actionType;

                switch (type)
                {
                    case AppActionType.WriteArticle:
                        return CommentType.Blog;
                    case AppActionType.UploadPicture:
                        return CommentType.Photo;
                    case AppActionType.UpdateDoing:
                        return CommentType.Doing;
                    case AppActionType.Share:
                        return CommentType.Share;
                }
            }

            return CommentType.All;
        }


        public CommentCollection GetComments(IEnumerable<int> commentIDs)
        {
            if (ValidateUtil.HasItems<int>(commentIDs) == false)
                return new CommentCollection();
            else
                return CommentDao.Instance.GetComments(commentIDs);
        }


        public CommentCollection GetComments(int targetID, CommentType type, int getCount, bool isGetAll)
        {
            return CommentDao.Instance.GetComments(targetID, type, getCount, isGetAll);
        }
	}
}