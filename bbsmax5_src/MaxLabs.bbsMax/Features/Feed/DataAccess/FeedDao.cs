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
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;


namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class FeedDao : DaoBase<FeedDao>
    {

        /// <summary>
        /// 获取通知模板
        /// </summary>
        /// <returns></returns>
        public abstract FeedTemplateCollection GetFeedTemplates();

        /// <summary>
        /// 设置动态模板，存在就更新，不存在就插入
        /// </summary>
        /// <param name="feedTemplete"></param>
        public abstract void SetTemplates(FeedTemplateCollection feedTemplates);


        /// <summary>
        /// 获取全站动态
        /// </summary>
        /// <param name="feedID">起始FeedID</param>
        /// <param name="getCount"></param>
        /// <param name="appID">指定应用（如果是所有应用就设为Guid.Empty）</param>
        /// <param name="actionType">指定应用动作（如果是所有动作就设为null）</param>
        public abstract FeedCollection GetAllUserFeeds(int feedID, int getCount, Guid appID, int? actionType);

        /// <summary>
        /// 获取好友通知
        /// </summary>
        /// <param name="friends"></param>
        /// <param name="feedID">通知ID,取小于该ID的通知</param>
        /// <param name="getCount">获取条数</param>
        /// <param name="appID">指定应用（如果是所有应用就设为Guid.Empty）</param>
        /// <param name="actionType">指定应用动作（如果是所有动作就设为null）</param>
        public abstract FeedCollection GetFriendFeeds(int userID, IEnumerable<int> friendUserIDs, int feedID, int getCount, Guid appID, int? actionType);


        /// <summary>
        /// 获取用户通知
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="feedID"></param>
        /// <param name="getCount"></param>
        /// <param name="appID">指定应用（如果是所有应用就设为Guid.Empty）</param>
        /// <param name="actionType">指定应用动作（如果是所有动作就设为null）</param>
        /// <returns></returns>
        public abstract FeedCollection GetUserFeeds(int userID, int feedID, int getCount, Guid appID, int? actionType);


        public abstract FeedCollection GetFeeds(IEnumerable<int> feedIDs);

        /// <summary>
        /// 删除一个用户的多个动态
        /// </summary>
        /// <param name="userID">feed的userID,为null的时候 删除该动态的所有用户（全站动态的时候 管理员删除）</param>
        /// <param name="feedID"></param>
        public abstract void DeleteFeeds(int? userID, IEnumerable<int> feedIDs);


        /// <summary>
        /// 删除指定时间以前的所有动态
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="count">保留条数</param>
        public abstract void DeleteFeeds(DateTime? dateTime, int count);


        /// <summary>
        /// 添加一个动态 返回FeedID
        /// </summary>
        /// <param name="feed"></param>
        public abstract int CreateFeed(Feed feed, UserFeed userFeed, FeedSendItem.SendType sendType, bool canJoin);


        /// <summary>
        /// 用户设置自己的动态不加入通知
        /// </summary>
        /// <param name="userNoAddFeedApp">不记录动态的应用</param>
        public abstract void SetUserNoAddFeedApps(int userID, UserNoAddFeedAppCollection userNoAddFeedApp);


        /// <summary>
        /// 获取用户 不加入通知的设置
        /// </summary>
        /// <param name="userID"></param>
        public abstract UserNoAddFeedAppCollection GetUserNoAddFeedApps(int userID);



        /// <summary>
        ///  用户过滤某个动态
        /// </summary>
        public abstract void CreateFeedFilter(FeedFilter feedFilter);


        /// <summary>
        /// 获取 动态过滤设置
        /// </summary>
        public abstract FeedFilterCollection GetFeedFilters(int userID);

        /// <summary>
        /// 删除动态过滤设置
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="feedFilterIDs"></param>
        public abstract void DeleteFeedFilters(int userID,IEnumerable<int> feedFilterIDs);

        /// <summary>
        /// 更新动态隐私类型
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <param name="targetID"></param>
        /// <param name="privacyType"></param>
        /// <param name="visibleUserIDs">可见的用户</param>
        public abstract void UpdateFeedPrivacyType(Guid appID, int actionType, int targetID, PrivacyType privacyType, List<int> visibleUserIDs);

        /// <summary>
        /// 删除动态
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <param name="targetID"></param>
        public abstract void DeleteFeeds(Guid appID, int actionType, IEnumerable<int> targetIDs, IEnumerable<List<int>> userIDs);

        public abstract FeedCollection SearchFeeds(int pageNumber,FeedSearchFilter filter,ref int totalCount);

        public abstract void DeleteSearchFeeds(FeedSearchFilter filter,int deleteTopCount, out int deletedCount);

    }
}