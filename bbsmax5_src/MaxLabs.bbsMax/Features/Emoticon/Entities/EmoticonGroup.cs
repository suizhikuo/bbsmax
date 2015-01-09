//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System.Collections.Generic;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class EmoticonGroup :EmoticonGroupBase, IPrimaryKey<int>, IFillSimpleUser
    {
        public EmoticonGroup()
        { }

        public EmoticonGroup(DataReaderWrap readerWrap)
        {
            this.GroupID = readerWrap.Get<int>("GroupID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.GroupName = readerWrap.Get<string>("GroupName");
            this.TotalEmoticons = readerWrap.Get<int>("TotalEmoticons");
            this.TotalSizes = readerWrap.Get<int>("TotalSizes");
        }

        public int UserID
        {
            get;
            set;
        }

        public long TotalSizes
        {
            get;
            set;
        }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(UserID);
            }
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.GroupID;
        }

        #endregion

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return UserID;
        }

        #endregion
    }

    public class EmoticonGroupBase
    {
        public virtual int TotalEmoticons
        {
            get;
            set;
        }  
        public virtual  string GroupName
        {
            get;
            set;
        }

        public virtual bool IsDefault { get; set; }

        public virtual int  GroupID
        {
            get;
            set;
        }

        public virtual string PreviewUrl { get; set; }
    }

    public class EmoticonGroupCollection : EntityCollectionBase<int, EmoticonGroup>
    {
        public EmoticonGroupCollection() { }

        public EmoticonGroupCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new EmoticonGroup(readerWrap));
            }
        }
    }
}