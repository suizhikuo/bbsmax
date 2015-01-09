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
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    [Serializable]
    public class Announcement : IPrimaryKey<int>, IComparable<Announcement>, IFillSimpleUser,ITimeLimit
    {
        public Announcement()
        {
            this.BeginDate = DateTimeUtil.Now;
            this.EndDate = DateTime.MaxValue;
        }

        public Announcement( DataReaderWrap readerWrap )
        {
            this.AnnouncementID     = readerWrap.Get<int>("AnnouncementID");
            this.Subject            = readerWrap.Get<string>("Subject");
            this.EndDate            = readerWrap.Get<DateTime>("EndDate");
            this.Content            = readerWrap.Get<string>("Content");
            this.SortOrder          = readerWrap.Get<int>("SortOrder");
            this.BeginDate          = readerWrap.Get<DateTime>("BeginDate");
            this.PostUserID         = readerWrap.Get<int>("PostUserID");
            this.AnnouncementType   = readerWrap.Get<AnnouncementType>("AnnouncementType");
        }

        [JsonItem]
        public int AnnouncementID
        {
            get;
            set;
        }

        [JsonItem]
        public AnnouncementType AnnouncementType
        {
            get;
            set;
        }

        [JsonItem]
        public int PostUserID
        {
            get;
            set;
        }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetSimpleUser(PostUserID, GetUserOption.WithAll);
            }
        }

        [JsonItem]
        public DateTime BeginDate
        {
            get;
            set;
        }

        [JsonItem]
        public string Subject
        {
            get;
            set;
        }

        [JsonItem]
        public string Content
        {
            get;
            set;
        }

        [JsonItem]
        public DateTime EndDate
        {
            get;
            set;
        }

        [JsonItem]
        public int SortOrder
        {
            get;
            set;
        }

        public int GetKey()
        {
            return this.AnnouncementID;
        }

        public int CompareTo(Announcement announcement)
        {
            if (this.SortOrder < announcement.SortOrder)
                return 1;
            else if (this.SortOrder > announcement.SortOrder)
                return -1;
            return 0;
        }

        #region 扩展属性

        [JsonItem]
        public string TypeName
        {
            get
            {
                return AnnouncementType == AnnouncementType.Text ? "文本" : "链接";
            }
        }

            
        #endregion



        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return PostUserID;
        }

        #endregion
    }

    public class AnnouncementCollection : HashedTimeLimitCollectionBase<int, Announcement, AnnouncementCollection>
    {
        public AnnouncementCollection() { }

        public AnnouncementCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                Add(new Announcement(readerWrap));
            }
        }


    }
}