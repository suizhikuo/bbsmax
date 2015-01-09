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
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class ChatSession : IPrimaryKey<int>, IFillSimpleUsers
    {
        public ChatSession()
        { 
        }
        public ChatSession(DataReaderWrap readerWrap)
        {
            ChatSessionID = readerWrap.Get<int>("ChatSessionID");
            OwnerID = readerWrap.Get<int>("UserID");
            UserID = readerWrap.Get<int>("TargetUserID");
            TotalMessages = readerWrap.Get<int>("TotalMessages");
            UnreadMessages = readerWrap.Get<int>("UnreadMessages");
            LastMessage = StringUtil.CutString(StringUtil.ClearAngleBracket(readerWrap.Get<string>("LastMessage"),true), 20);
            CreateDate = readerWrap.Get<DateTime>("CreateDate");
            UpdateDate = readerWrap.Get<DateTime>("UpdateDate");
        }

        public int ChatSessionID { get; set; }

        public int OwnerID { get; set; }

        public int UserID { get; set; }

        public int TotalMessages { get; set; }

        public int UnreadMessages { get; set; }

        public string LastMessage { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public SimpleUser User
        {
            get { return UserBO.Instance.GetFilledSimpleUser(UserID); }
        }

        public SimpleUser Owner
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(OwnerID);
            }
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ChatSessionID;
        }

        #endregion

        #region 仅供模版使用的成员，在代码中调用将违背设计原则

        public int ID
        {
            get
            {
                return ChatSessionID;
            }
        }

        [Obsolete("这个属性供模版中使用。不允许在代码中使用")]
        public string FriendlyCreateDate
        {
            get { return DateTimeUtil.GetFriendlyDate(CreateDate); }
        }

        [Obsolete("这个属性供模版中使用。不允许在代码中使用")]
        public string FriendlyUpdateDate
        {
            get { return DateTimeUtil.GetFriendlyDate(UpdateDate); }
        }

        #endregion


        #region IFillSimpleUsers 成员

        public int[] GetUserIdsForFill(int actionType)
        {
            return new int[] { UserID, OwnerID };
        }

        #endregion
    }

    public class ChatSessionCollection : EntityCollectionBase<int, ChatSession>
    {
        public ChatSessionCollection()
        { }

        public ChatSessionCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new ChatSession(readerWrap));
            }
        }
    }
}