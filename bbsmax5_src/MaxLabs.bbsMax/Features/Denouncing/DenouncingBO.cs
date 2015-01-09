//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax
{
	public class DenouncingBO : BOBase<DenouncingBO>
    {
		public BackendPermissions ManagePermission
		{
			get
			{
				return AllSettings.Current.BackendPermissions;
			}
		}

		public bool CreateDenouncing(int operatorID, int targetID, int targetUserID, DenouncingType type, string content, string createIP)
        {
            if (targetID <= 0)
            {
                ThrowError(new InvalidParamError("targetID"));
                return false;
            }

            if (StringUtil.GetByteCount("content") > Consts.Report_Length)
            {
                Context.ThrowError(new InvalidReportLengthError("content", content, Consts.Report_Length));
                return false;
            }

			DenouncingDao.Instance.CreateDenouncing(operatorID, targetID, targetUserID, type, content, createIP);

            CacheUtil.Remove("Denouncing/Count");

            return true;
        }

		public bool DeleteDenouncingWithData(int operatorID, int denouncingID)
		{
			if (ManagePermission.Can(operatorID, BackendPermissions.Action.Manage_Report) == false)
			{
				ThrowError(new NoPermissionDeleteDenouncingError());
				return false;
			}

			Denouncing denouncing = DenouncingDao.Instance.GetDenouncing(denouncingID);

			if (denouncing == null)
				return false;
			else
			{
				DenouncingDao.Instance.DeleteDenouncing(denouncingID);

				switch (denouncing.Type)
				{
					case DenouncingType.Blog:
						BlogBO.Instance.DeleteBlogArticle(operatorID, denouncing.TargetID, true);
						break;

					case DenouncingType.Photo:
						AlbumBO.Instance.DeletePhoto(operatorID, denouncing.TargetID, true);
						break;

					case DenouncingType.Share:
						ShareBO.Instance.DeleteShare(operatorID, denouncing.TargetID, true);
						break;

					case DenouncingType.Space:
						//老达TODO:怎么搞？
						break;

                    case DenouncingType.Topic:

                        PostBOV5.Instance.DeleteThreads(UserBO.Instance.GetAuthUser(operatorID), new int[] { denouncing.TargetID }, false, true, false, false, string.Empty);

                        break;

                    case DenouncingType.Reply:
                        PostV5 post = PostBOV5.Instance.GetPost(denouncing.TargetID, false);
                        PostBOV5.Instance.DeletePosts(UserBO.Instance.GetAuthUser(operatorID), post.ForumID, post.ThreadID, new int[] { post.PostID }, false, true, false, string.Empty);

                        break;
				}

                //CacheUtil.Remove("Denouncing/Count");

				return true;
			}
		}

		public bool DeleteDenouncings(int operatorID, int[] denouncingIDs)
		{
            if (ManagePermission.Can(operatorID, BackendPermissions.Action.Manage_Report) == false)
			{
				ThrowError(new NoPermissionIgnoreDenouncingError());
				return false;
			}

			DenouncingDao.Instance.DeleteDenouncings(denouncingIDs);

            //CacheUtil.Remove("Denouncing/Count");

			return true;
		}

		public bool IgnoreDenouncing(int operatorID, int denouncingID)
		{
            if (ManagePermission.Can(operatorID, BackendPermissions.Action.Manage_Report) == false)
			{
				ThrowError(new NoPermissionIgnoreDenouncingError());
				return false;
			}

			DenouncingDao.Instance.IgnoreDenouncing(denouncingID);

            CacheUtil.Remove("Denouncing/Count");

			return true;
		}

		public bool IgnoreDenouncings(int operatorID, int[] denouncingIDs)
		{
            if (ManagePermission.Can(operatorID, BackendPermissions.Action.Manage_Report) == false)
			{
				ThrowError(new NoPermissionIgnoreDenouncingError());
				return false;
			}

			DenouncingDao.Instance.IgnoreDenouncings(denouncingIDs);

            CacheUtil.Remove("Denouncing/Count");

			return true;
		}

		public CheckDenouncingStateResult CheckDenouncingState(int operatorID, int targetID, DenouncingType targetType)
		{
			return DenouncingDao.Instance.CheckDenouncingState(operatorID, targetID, targetType);
		}

		public DenouncingCollection GetDenouncingBySearch(DenouncingFilter filter, int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 1;

			DenouncingCollection reports = DenouncingDao.Instance.GetDenouncingBySearch(filter, pageNumber);

            return reports;
        }

        public void GetDenouncingCount(
            out int? denouncingPhotoCount, 
            out int? denouncingArticleCount, 
            out int? denouncingShareCount,
            out int? denouncingUserCount,
            out int? denouncingTopicCount,
            out int? denouncingReplyCount)
        {
            string cacheKey = "Denouncing/Count";

            int?[] cachedata = null;

            if (CacheUtil.TryGetValue<int?[]>(cacheKey, out cachedata) == false)
            {
                DenouncingDao.Instance.GetDenouncingCount(
                    out denouncingPhotoCount,
                    out denouncingArticleCount,
                    out denouncingShareCount,
                    out denouncingUserCount,
                    out denouncingTopicCount,
                    out denouncingReplyCount);

                cachedata = new int?[] { 
                    denouncingPhotoCount,
                    denouncingArticleCount,
                    denouncingShareCount,
                    denouncingUserCount,
                    denouncingTopicCount,
                    denouncingReplyCount
                };

                CacheUtil.Set<int?[]>(cacheKey, cachedata);
            }
            else
            {
                int i = 0;

                denouncingPhotoCount = cachedata[i ++];
                denouncingArticleCount = cachedata[i ++];
                denouncingShareCount = cachedata[i ++];
                denouncingUserCount = cachedata[i ++];
                denouncingTopicCount = cachedata[i ++];
                denouncingReplyCount = cachedata[i++];
            }
        }

        public DenouncingCollection GetDenouncingWithArticle(DenouncingFilter filter, int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            DenouncingCollection result = DenouncingDao.Instance.GetDenouncingWithArticle(filter, pageNumber);

            return result;
        }

        public DenouncingCollection GetDenouncingWithPhoto(DenouncingFilter filter, int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            DenouncingCollection result = DenouncingDao.Instance.GetDenouncingWithPhoto(filter, pageNumber);

            return result;
        }

        public DenouncingCollection GetDenouncingWithShare(DenouncingFilter filter, int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            DenouncingCollection result = DenouncingDao.Instance.GetDenouncingWithShare(filter, pageNumber);

            return result;
        }

        public DenouncingCollection GetDenouncingWithUser(DenouncingFilter filter, int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            DenouncingCollection result = DenouncingDao.Instance.GetDenouncingWithUser(filter, pageNumber);

            return result;
        }

        public DenouncingCollection GetDenouncingWithTopic(DenouncingFilter filter, int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            DenouncingCollection result = DenouncingDao.Instance.GetDenouncingWithTopic(filter, pageNumber);

            return result;
        }

        public DenouncingCollection GetDenouncingWithReply(DenouncingFilter filter, int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            DenouncingCollection result = DenouncingDao.Instance.GetDenouncingWithReply(filter, pageNumber);

            return result;
        }
    }
}