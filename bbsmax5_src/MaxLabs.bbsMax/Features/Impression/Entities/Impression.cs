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
    public class Impression : IPrimaryKey<int>, ITextRevertable, IFillSimpleUser
    {
        public Impression(DataReaderWrap reader)
        {
            this.UserID = reader.Get<int>("UserID");
            this.TypeID = reader.Get<int>("TypeID");
            this.Count = reader.Get<int>("Count");
            this.Text = reader.Get<string>("Text");
            this.KeywordVersion = reader.Get<string>("KeywordVersion");
        }

        public int UserID
        {
            get;
            set;
        }

        public int TypeID
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.TypeID;
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

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return this.UserID;
        }

        #endregion
    }

    public class ImpressionCollection : Collection<Impression>
    {
        public ImpressionCollection()
        { 
        }
        public ImpressionCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new Impression(readerWrap));
            }
        }
    }
}