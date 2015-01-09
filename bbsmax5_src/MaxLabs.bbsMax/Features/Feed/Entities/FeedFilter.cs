//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 通知过滤
    /// </summary>
    public class FeedFilter
    {
        public FeedFilter()
        { 
        }

        public FeedFilter(DataReaderWrap readerWrap)
        {

            AppID = readerWrap.Get<Guid>("AppID");

            ID = readerWrap.Get<int>("ID");
            UserID = readerWrap.Get<int>("UserID");

            FriendUserID = readerWrap.GetNullable<int>("FriendUserID");
            ActionType = readerWrap.GetNullable<byte>("ActionType");
        }

        /// <summary>
        /// 应用ID
        /// </summary>
        public Guid AppID { get; set; }

        public int ID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }


        /// <summary>
        /// 好友用户ID
        /// </summary>
        public int? FriendUserID { get; set; }

        /// <summary>
        /// 应用的动作类型
        /// </summary>
        public int? ActionType { get; set; }



        /// <summary>
        /// 过滤类型
        /// </summary>
        public FilterType FilterType 
        {
            get
            {
                if (AppID != Guid.Empty && FriendUserID != null && ActionType != null)
                    return FilterType.FilterUserAppAction;
                else if (FriendUserID != null && AppID == Guid.Empty)
                    return FilterType.FilterUser;
                else if (AppID != Guid.Empty && ActionType != null)
                    return FilterType.FilterAppAction;
                else
                    return FilterType.FilterApp;
            }
        }

        public User FriendUser
        {
            get
            {
                if (FriendUserID != null)
                {
                    return UserBO.Instance.GetUser(FriendUserID.Value);
                }
                return null;
            }
        }
    }


    /// <summary>
    /// 通知过滤对象集合
    /// </summary>
    public class FeedFilterCollection : Collection<FeedFilter>
    {
        public FeedFilterCollection()
        {
        }

        public FeedFilterCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                this.Add(new FeedFilter(readerWrap));
            }
        }
    }

    
}