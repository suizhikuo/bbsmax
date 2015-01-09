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

namespace MaxLabs.bbsMax.Entities
{
    public class ChatMessage : IPrimaryKey<int>, ITextRevertable, IFillSimpleUsers
    {
        public ChatMessage()
        {
        }
        public ChatMessage(DataReaderWrap readerWrap)
        {
            this.MessageID = readerWrap.Get<int>("MessageID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.TargetUserID = readerWrap.Get<int>("TargetUserID");
            this.IsReceive = readerWrap.Get<bool>("IsReceive");
            this.IsRead = readerWrap.Get<bool>("IsRead");
            this.FromMessageID = readerWrap.Get<int>("FromMessageID");
            this.Content = UrlUtil.ReplaceRootVar(readerWrap.Get<string>("Content"));
            this.CreateIP = readerWrap.Get<string>("CreateIP");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.KeywordVersion = readerWrap.Get<string>("KeywordVersion");

        }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(UserID);
            }
        }

        [JsonItem]
        public int MessageID { get; set; }

        public int UserID { get; set; }

        public int TargetUserID { get; set; }

        [JsonItem]
        public bool IsReceive { get; set; }

        public bool IsRead { get; set; }

        public int FromMessageID { get; set; }

        [JsonItem]
        public string Content { get; set; }

        public string CreateIP { get; set; }

        [JsonItem]
        public DateTime CreateDate { get; set; }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return MessageID;
        }

        #endregion

        #region 仅供模版使用的成员，在代码中调用将违背设计原则

        public SimpleUser TargetUser
        {
            get { return UserBO.Instance.GetFilledSimpleUser(TargetUserID); }
        }

        [Obsolete("这个属性供模版中使用。不允许在代码中使用")]
        public string FriendlyCreateDate
        {
            get { return DateTimeUtil.GetFriendlyDate(CreateDate); }
        }

        #endregion

        #region ITextRevertable 成员

        public string Text
        {
            get { return Content; }
        }

        public string KeywordVersion
        {
            get;
            set;
        }

        public void SetNewRevertableText(string text, string textVersion)
        {
            this.Content = text;
            this.KeywordVersion = textVersion;
        }

        public void SetOriginalText(string originalText)
        {
            this.OriginalContent = originalText;
        }

        public string OriginalContent
        {
            get;
            internal set;
        }

        #endregion

        #region IFillSimpleUsers 成员

        public int[] GetUserIdsForFill(int actionType)
        {
            return new int[] { UserID, TargetUserID };
        }

        #endregion
    }

    public class ChatMessageCollection : EntityCollectionBase<int, ChatMessage>
    {
        public ChatMessageCollection()
        {
        }

        public ChatMessageCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new ChatMessage(readerWrap));
            }
        }

        public override IEnumerator<ChatMessage> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public bool DateChanged
        {
            get;
            private set;
        }


        public struct Enumerator : IEnumerator<ChatMessage>
        {
            private ChatMessageCollection m_InnerCollection;
            private ChatMessage m_Current;
            private int m_Index;

            internal Enumerator(ChatMessageCollection collection)
            {
                m_InnerCollection = collection;

                m_Index = 0;
                m_Current = null;
            }

            #region IEnumerator<ChatMessage> 成员

            public ChatMessage Current
            {
                get { return m_Current; }
            }

            #endregion

            #region IDisposable 成员

            public void Dispose()
            {
                
            }

            #endregion

            #region IEnumerator 成员

            object System.Collections.IEnumerator.Current
            {
                get { return m_Current; }
            }

            public bool MoveNext()
            {
                if (m_Index >= m_InnerCollection.Count)
                    return false;


                if (m_Current == null
                    || m_Current.CreateDate.Year != m_InnerCollection[m_Index].CreateDate.Year
                    || m_Current.CreateDate.DayOfYear != m_InnerCollection[m_Index].CreateDate.DayOfYear)
                    m_InnerCollection.DateChanged = true;

                else
                    m_InnerCollection.DateChanged = false;


                m_Current = m_InnerCollection[m_Index];
                m_Index++;

                return true;
            }

            public void Reset()
            {
                m_Index = 0;
                m_Current = null;
            }

            #endregion
        }

    }
}