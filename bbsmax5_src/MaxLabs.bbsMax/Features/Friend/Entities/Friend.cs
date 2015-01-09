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
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
    [Serializable]
    /// <summary>
    /// 好友
    /// </summary>
    public class Friend : IPrimaryKey<int>, IFillSimpleUser
    {
        public Friend()
        { }

        public Friend(DataReaderWrap readerWrap)
        {
            this.OwnerID = readerWrap.Get<int>("UserID");
            this.UserID = readerWrap.Get<int>("FriendUserID");
            this.GroupID = readerWrap.Get<int>("GroupID");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");

            this.Hot = readerWrap.Get<int>("Hot");
        }

        /// <summary>
        /// 好友热度
        /// </summary>
        public int Hot { get; set; }

        /// <summary>
        /// 好友的所有者ID
        /// </summary>
        public int OwnerID { get; set; }

        /// <summary>
        /// 好友ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 好友组
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// 成为好友时间
        /// </summary>
        public DateTime CreateDate { get; set; }


        //public OnlineStatus OnlineStatus
        //{
        //    get { return OnlineManager.GetOnlineStatus(UserID); }
        //}

#if !Passport

        /// <summary>
        /// 当前用户是否在线（不包括隐身）
        /// </summary>
        public bool IsOnline
        {
            get { return OnlineUserPool.Instance.IsOnline(UserID); }
        }

        /// <summary>
        /// 当前用户是否隐身
        /// </summary>
        public bool IsInvisible
        {
            get { return OnlineUserPool.Instance.IsInvisible(UserID); }
        }

        /// <summary>
        /// 当前用户是否在线或隐身
        /// </summary>
        public bool IsOnlineOrInvisible
        {
            get { return OnlineUserPool.Instance.IsOnlineOrInvisible(UserID); }
        }


        /// <summary>
        /// 当前用户是否已经被屏蔽动态
        /// </summary>
        public bool IsShield
        {
            get
            {
                int myUserID = UserBO.Instance.GetCurrentUserID();

                if (myUserID <= 0)
                    return false;

                if (!FriendBO.Instance.IsShieldFriendGroup(myUserID, GroupID))
                    return FeedBO.Instance.GetFiltratedFriendUserIDs(myUserID).Contains(UserID);
                
                return false;
            }
        }

#endif

        [Obsolete("模板专用，需要先填充方可使用")]
        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.UserID);
            }
        }

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return UserID;
        }

        #endregion

        #region IPrimaryKey<int> Members

        public int GetKey()
        {
            return UserID;
        }

        #endregion


    }

    /// <summary>
    /// 好友对象集合
    /// </summary>
    public class FriendCollection : EntityCollectionBase<int, Friend>
    {
        private Blacklist m_Blacklist = new Blacklist();

        public FriendCollection()
        {
        }

        public FriendCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                if (readerWrap.Get<int>("GroupID") == -1)
                    m_Blacklist.Add(new BlacklistItem(readerWrap));

                else
                    this.Add(new Friend(readerWrap));
            }
        }

        public override void Add(Friend item)
        {
            if (item.GroupID < 0)
                throw new ArgumentNullException("好友分组ID不合法");

            if (this.Count > 0)
            {
                if (this[0].OwnerID != item.OwnerID)
                    throw new ArgumentException("不支持在一个列表中添加不同用户的好友实例", "item");
            }

            
            base.Add(item);
        }


        public FriendCollection GetFromGroup(int friendGroupID)
        {
            FriendCollection friends = new FriendCollection();

            foreach(Friend friend in this)
            {
                if (friend.GroupID == friendGroupID)
                    friends.Add(friend);
            }

            return friends;
        }

        public Blacklist Blacklist
        { get { return m_Blacklist; } }
    }
}