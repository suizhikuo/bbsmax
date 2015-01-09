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

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;

using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax
{
    public class TagBO : BOBase<TagBO>
    {
		public BackendPermissions ManagePermission
		{
			get { return AllSettings.Current.BackendPermissions; }
		}

		public bool CheckTagManagePermission(int userID)
		{
			return ManagePermission.Can(userID, BackendPermissions.Action.Manage_Tag);
		}

		public TagCollection GetUserTags(int userID, TagType type)
		{
			if (ValidateUserID(userID) == false)
				return null;

			string cacheKey = GetCacheKeyForUserTags(type, userID);

			TagCollection tags = null;

			if (CacheUtil.TryGetValue<TagCollection>(cacheKey, out tags))
				return tags;

			tags = TagDao.Instance.GetUserTags(userID, type);

			CacheUtil.Set<TagCollection>(cacheKey, tags);

			return tags;
		}

		private string GetCacheKeyForUserTags(TagType type, int userID)
		{
			return "Tags/" + type + "/" + userID;
		}

		#region =========↓未整理的代码↓============================================================================================
		#region CacheKey

		private const string CacheKey_TagPrefix = "Tags/List";
        private const string CacheKey_Tag_List_All = "Tags/List/All/{0}/{1}";
        private const string CacheKey_TagCount = "Tags/List/All/Count";

        #endregion

        /// <summary>
        /// 获取指定标签
        /// </summary>
        /// <param name="tagID">指定标签的ID</param>
        public Tag GetTag(int tagID)
        {
            if (tagID <= 0)
            {
                ThrowError(new InvalidParamError("tagID"));
                return new Tag();
            }

            Tag tag = TagDao.Instance.GetTag(tagID);

            return tag;
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        public TagCollection GetUnlockTags(int pageNumber, int pageSize, ref int? count)
        {
            TagCollection tags = null;
            string cacheKey = string.Format(CacheKey_Tag_List_All, pageSize, pageNumber);
            if (!CacheUtil.TryGetValue<TagCollection>(cacheKey, out tags))
            {
                tags = TagDao.Instance.GetMostTags(false, pageNumber, pageSize, ref count);
                CacheUtil.Set<TagCollection>(cacheKey, tags);
            }

            return tags;
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        public TagCollection GetAllTags(int pageNumber, int pageSize, ref int? count)
        {
            TagCollection tags = null;

            tags = TagDao.Instance.GetAllTags(pageNumber, pageSize, ref count);

            return tags;
        }

        /// <summary>
        /// 获取指定类型指定对象的标签
        /// </summary>
        /// <param name="type">标签类型</param>
        /// <param name="targetID">对象ID</param>
        public TagCollection GetTags(TagType type, int targetID)
        {
            TagCollection tags = null;

            tags = TagDao.Instance.GetTags(type, targetID);

            return tags;
        }

        /// <summary>
        /// 保存标签
        /// </summary>
        /// <param name="tags">标签集</param>
        /// <param name="type">类型,如日志标签等</param>
        /// <param name="targetID">标签的对象ID</param>
        public bool SaveTags(TagCollection tags, TagType type, int targetID)
        {
            if (SafeMode)
            {
                if (!IsExecutorLogin)
                {
                    ThrowError(new NotLoginError());
                    return false;
                }
            }

            if (targetID <= 0)
            {
                ThrowError(new InvalidParamError("targetID"));
                return false;
            }

            bool success = TagDao.Instance.SaveTags(tags, type, targetID);
            if (success)
            {
                CacheUtil.RemoveBySearch(CacheKey_TagPrefix);
            }

            return true;
        }

		public bool SaveTags(IEnumerable<string> tagNames, TagType type, int targetID, int operatorID)
		{
			if (tagNames != null)
			{
				TagCollection tags = new TagCollection();

				foreach (string tagName in tagNames)
				{
					string t = tagName.Trim();

					if (string.IsNullOrEmpty(t))
						continue;

					Tag tag = new Tag();

					tag.Name = t;
					tag.IsLock = false;

					tags.Add(tag);
				}

				string cacheKey = GetCacheKeyForUserTags(type, operatorID);

				CacheUtil.Remove(cacheKey);

				return SaveTags(tags, type, targetID);
			}

			return false;
		}

        /// <summary>
        /// 搜索标签
        /// </summary>
        public TagCollection GetTagsBySearch(AdminTagFilter filter, int pageNumber, ref int? count)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            //int pageSize = filter.PageSize;
            //TagOrderKey orderBy = filter.OrderBy;

            if (pageNumber <= 0) { pageNumber = 1; }
            //if (pageSize <= 0) { pageSize = Consts.DefaultPageSize; }

            TagCollection tags = TagDao.Instance.GetTagsByFilter(filter, pageNumber, ref count);

            return tags;
        }

        /// <summary>
        /// 锁定标签
        /// </summary>
        /// <param name="tagID">标签ID</param>
        public bool LockTag(int tagID)
        {
            if (SafeMode)
            {
                if (!IsExecutorLogin)
                {
                    ThrowError(new NotLoginError());
                    return false;
                }
            }

            if (tagID <= 0)
            {
                ThrowError(new InvalidParamError("tagID"));
                return false;
            }

            if (SafeMode)
            {
                //TODO:
            }

            bool success = TagDao.Instance.LockTag(tagID);

            if (success)
            {
                CacheUtil.RemoveBySearch(CacheKey_TagPrefix);
            }

            return true;
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagID">删除标签ID</param>
        public bool DeleteTag(int tagID)
        {
            if (SafeMode)
            {
                if (!IsExecutorLogin)
                {
                    ThrowError(new NotLoginError());
                    return false;
                }
            }

            if (tagID <= 0)
            {
                ThrowError(new InvalidParamError("tagID"));
                return false;
            }

            if (SafeMode)
            {
                //TODO:
            }

            bool success = TagDao.Instance.DeleteTag(tagID);

            if (success)
            {
                CacheUtil.RemoveBySearch(CacheKey_TagPrefix);
            }

            return true;
        }

        /// <summary>
        /// 删除多个标签
        /// </summary>
        public bool DeleteTags(IEnumerable<int> tagIDs)
        {
            if (SafeMode)
            {
                if (!IsExecutorLogin)
                {
                    ThrowError(new NotLoginError());
                    return false;
                }
            }

            if (tagIDs == null || ValidateUtil.HasItems<int>(tagIDs) == false)
            {
                ThrowError(new NotSelectedTagsError("tagIDs"));
                return false;
            }

            if (SafeMode)
            {
                //TODO:
            }

            bool success = TagDao.Instance.DeleteTags(tagIDs);

            if (success)
            {
                CacheUtil.RemoveBySearch(CacheKey_TagPrefix);
            }

            return true;

        }

        /// <summary>
        /// 高级删除
        /// </summary>
        public bool DeleteTags(AdminTagFilter filter)
        {
            if (SafeMode)
            {
                if (!IsExecutorLogin)
                {
                    ThrowError(new NotLoginError());
                    return false;
                }
            }

            if (filter == null)
            {
                filter = new AdminTagFilter();
            }

            if (SafeMode)
            {
                //TODO:
            }

            bool success = TagDao.Instance.DeleteTagsByFilter(filter);

            if (success)
            {
                CacheUtil.RemoveBySearch(CacheKey_TagPrefix);
            }

            return true;
		}

		#endregion
	}
}