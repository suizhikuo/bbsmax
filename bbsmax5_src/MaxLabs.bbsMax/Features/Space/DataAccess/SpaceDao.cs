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
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class SpaceDao : DaoBase<SpaceDao>
    {
		public abstract SpaceData GetSpaceDataForVisit(int spaceOwnerID, DataAccessLevel dataAccessLevel);

		public abstract VisitorCollection GetSpaceVisitors(int spaceOwnerID, int pageSize, int pageNumber);

		public abstract VisitorCollection GetSpaceVisitTrace(int visitorID, int pageSize, int pageNumber);

        /// <summary>
        /// 更新个人空间 隐私设置
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="space"></param>
        public abstract void UpdateSpacePrivacy(int userID, SpacePrivacyType blogPrivacy, SpacePrivacyType feedPrivacy, SpacePrivacyType boardPrivacy, SpacePrivacyType doingPrivacy, SpacePrivacyType albumPrivacy, SpacePrivacyType spacePrivacy, SpacePrivacyType sharePrivacy, SpacePrivacyType friendListPrivacy, SpacePrivacyType informationPrivacy);

        /// <summary>
        /// 更新访问者
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="visitorUserID"></param>
        public abstract void UpdateVisitor(int userID, int visitorUserID, string createIP);

        ///// <summary>
        ///// 个人隐私设置
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <returns></returns>
        //public abstract Space SelectSpace(int userID);

        /// <summary>
        /// 最近访客
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public abstract VisitorCollection SelectVisitors(int userID, int pageNumber, int pageSize, out int totalCount);

        /// <summary>
        /// 返回指定用户的最近访问者数
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract VisitorCollection SelectVisitors(int userID, int count);


        public abstract void UpdateSpaceTheme(int userID, string spaceTheme);
    }
}