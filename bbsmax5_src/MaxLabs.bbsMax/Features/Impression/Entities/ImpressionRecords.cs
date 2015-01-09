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

using MaxLabs.bbsMax.DataAccess;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    public class ImpressionRecord : IPrimaryKey<int>, ITextRevertable, IFillSimpleUsers
    {
        public ImpressionRecord(DataReaderWrap reader)
        {
            this.RecordID = reader.Get<int>("RecordID");
            this.TypeID = reader.Get<int>("TypeID");
            this.UserID = reader.Get<int>("UserID");
            this.TargetUserID = reader.Get<int>("TargetUserID");
            this.CreateDate = reader.Get<DateTime>("CreateDate");
            this.Text = reader.Get<string>("Text");
            this.KeywordVersion = reader.Get<string>("KeywordVersion");
        }

        public int ID
        {
            get { return RecordID; }
        }

        public int RecordID
        {
            get;
            set;
        }

        public int TypeID
        {
            get;
            set;
        }

        public int UserID
        {
            get;
            set;
        }

        public int TargetUserID
        {
            get;
            set;
        }

        public DateTime CreateDate
        {
            get;
            set;
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.TypeID; //注意这里故意返回TypeID，给关键字处理用的！！
        }

        #endregion

        #region ITextRevertable 成员

        public string OriginalText
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public string KeywordVersion
        {
            get;
            set;
        }

        public void SetNewRevertableText(string text, string textVersion)
        {
            Text = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText(string originalText)
        {
            OriginalText = originalText;
        }

        #endregion

        #region IFillSimpleUsers 成员

        public SimpleUser User
        {
            get{ return UserBO.Instance.GetFilledSimpleUser(this.UserID);}
        }

        public SimpleUser TargetUser
        {
            get { return UserBO.Instance.GetFilledSimpleUser(this.TargetUserID); }
        }

        public int[] GetUserIdsForFill(int actionType)
        {
            if (actionType == 0)
                return new int[] { this.UserID, this.TargetUserID };
            else if (actionType == 1)
                return new int[] { this.UserID };
            else if(actionType == 2)
                return new int[] { this.TargetUserID };

            return null;
        }

        #endregion
    }

    public class ImpressionRecordCollection : EntityCollectionBase<int, ImpressionRecord>
    {
        public ImpressionRecordCollection()
        {
        }
        public ImpressionRecordCollection(DataReaderWrap reader)
        {
            while (reader.Next)
            {
                this.Add(new ImpressionRecord(reader));
            }
        }
    }
}