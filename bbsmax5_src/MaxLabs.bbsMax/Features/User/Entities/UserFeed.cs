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

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 动态相关联的用户
    /// </summary>
    public class UserFeed : IPrimaryKey<string>
    {

        public UserFeed()
        {
        }

        public UserFeed(DataReaderWrap readerWrap)
        {
            FeedID = readerWrap.Get<int>("FeedID");
            UserID = readerWrap.Get<int>("UserID");
            Realname = readerWrap.Get<string>("Realname");
            CreateDate = readerWrap.Get<DateTime>("CreateDate");


        }

        /// <summary>
        /// 动态ID
        /// </summary>
        public int FeedID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.UserID);
            }
        }


        /// <summary>
        /// 呢称
        /// </summary>
        public string Realname { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return FeedID + "-" + UserID;
        }

        #endregion
    }

    public class UserFeedCollection : EntityCollectionBase<string, UserFeed>
    {
        public UserFeedCollection()
        {
        }

        public UserFeedCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                this.Add(new UserFeed(readerWrap));
            }
        }

        public UserFeed GetValue(int feedID, int userID)
        {
            return this.GetValue(feedID + "-" + userID);
        }
    }
}