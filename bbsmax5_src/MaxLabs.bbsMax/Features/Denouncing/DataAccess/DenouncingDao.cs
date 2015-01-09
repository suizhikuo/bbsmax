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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.DataAccess
{
	public abstract class DenouncingDao : DaoBase<DenouncingDao>
    {
		public abstract void CreateDenouncing(int userID, int targetID, int targetUserID, DenouncingType type, string content, string createIP);

		public abstract void DeleteDenouncing(int denouncingID);

		public abstract void DeleteDenouncings(int[] denouncingIDs);

		public abstract void IgnoreDenouncing(int denouncingID);

		public abstract void IgnoreDenouncings(int[] denouncingIDs);

		public abstract Denouncing GetDenouncing(int denouncingID);

		public abstract CheckDenouncingStateResult CheckDenouncingState(int operatorID, int targetID, DenouncingType targetType);

		public abstract DenouncingCollection GetDenouncingBySearch(DenouncingFilter filter, int pageNumber);

        public abstract void GetDenouncingCount(
            out int? denouncingPhotoCount,
            out int? denouncingArticleCount,
            out int? denouncingShareCount,
            out int? denouncingUserCount,
            out int? denouncingTopicCount,
            out int? denouncingReplyCount);

        public abstract DenouncingCollection GetDenouncingWithArticle(DenouncingFilter filter, int pageNumber);

        public abstract DenouncingCollection GetDenouncingWithPhoto(DenouncingFilter filter, int pageNumber);

        public abstract DenouncingCollection GetDenouncingWithShare(DenouncingFilter filter, int pageNumber);

        public abstract DenouncingCollection GetDenouncingWithUser(DenouncingFilter filter, int pageNumber);

        public abstract DenouncingCollection GetDenouncingWithTopic(DenouncingFilter filter, int pageNumber);

        public abstract DenouncingCollection GetDenouncingWithReply(DenouncingFilter filter, int pageNumber);
    }
}