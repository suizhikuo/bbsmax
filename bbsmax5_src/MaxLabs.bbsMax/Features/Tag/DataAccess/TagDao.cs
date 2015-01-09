//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class TagDao : DaoBase<TagDao>
	{
		/// <summary>
		/// 获取指定用户指定类型的标签
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="type">标签类型</param>
		/// <returns></returns>
		public abstract TagCollection GetUserTags(int userID, TagType type);

        /// <summary>
        /// 获取指定标签
        /// </summary>
        /// <param name="tagID">指定标签的ID</param>
        public abstract Tag GetTag(int tagID);

        /// <summary>
        /// 获取所有标签
        /// </summary>
        public abstract TagCollection GetAllTags(int pageNumber, int pageSize, ref int? count);

        /// <summary>
        /// 获取标签
        /// </summary>
        public abstract TagCollection GetMostTags(bool isLock, int pageNumber, int pageSize, ref int? count);

        /// <summary>
        /// 获取指定类型的标签
        /// </summary>
        /// <param name="type">类型,如日志标签等</param>
        public abstract TagCollection GetTags(TagType type, int pageNumber, int pageSize, ref int? count);

        /// <summary>
        /// 获取指定类型指定对象的标签
        /// </summary>
        /// <param name="type">标签类型</param>
        /// <param name="targetID">对象ID</param>
        public abstract TagCollection GetTags(TagType type, int targetID);

        /// <summary>
        /// 搜索标签
        /// </summary>
        public abstract TagCollection GetTagsByFilter(AdminTagFilter filter, int pageNumber, ref int? count);

        /// <summary>
        /// 保存标签
        /// </summary>
        /// <param name="tags">标签集</param>
        /// <param name="type">类型,如日志标签等</param>
        /// <param name="targetID">标签的对象ID</param>
        public abstract bool SaveTags(TagCollection tags, TagType type, int targetID);

        /// <summary>
        /// 锁定标签
        /// </summary>
        /// <param name="tagID">标签ID</param>
        public abstract bool LockTag(int tagID);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagID">标签ID</param>
        public abstract bool DeleteTag(int tagID);

        /// <summary>
        /// 删除多个标签
        /// </summary>
        public abstract bool DeleteTags(IEnumerable<int> tagIDs);

        /// <summary>
        /// 高级删除
        /// </summary>
        public abstract bool DeleteTagsByFilter(AdminTagFilter filter);
	}
}