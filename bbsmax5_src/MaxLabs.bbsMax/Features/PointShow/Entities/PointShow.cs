//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class PointShowBase:IPrimaryKey<int>, IFillSimpleUser
    {
        public PointShowBase()
        {

        }

        public PointShowBase(DataReaderWrap readerWrap)
        {

            this.UserID = readerWrap.Get<int>("UserID");
            this.ShowPoints = readerWrap.Get<int>("ShowPoints");
            this.Price = readerWrap.Get<int>("Price");
            this.Content = readerWrap.Get<string>("Content");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
        }

        public int UserID { get; set; }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.UserID);
            }
        }

        public int ShowPoints { get; set; }

        public string Content { get; set; }

        public int Price { get; set; }

        public DateTime CreateDate { get; set; }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.UserID;
        }

        #endregion

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return this.UserID;
        }

        #endregion
    }

    public class PointShow:PointShowBase
    {
        public PointShow(DataReaderWrap readerWrap)
            :base(readerWrap)
        {
            this.Rank = readerWrap.Get<int>("Rank");
        }

        public int Rank { get; set; }
    }

    public class PointShowCollection : EntityCollectionBase<int, PointShowBase>
    {

        public PointShowCollection()
        {

        }

        public PointShowCollection(  DataReaderWrap readerWrap)
        {
            while ( readerWrap.Next )
            {
                this.Add(new PointShowBase(readerWrap));
            }
        }

        #region IFillSimpleUsers 成员

        public int[] GetUserIdsForFill(int actionType)
        {
            List<int> userids = new List<int>();

            foreach (PointShowBase ps in this)
            {
                if (userids.Contains(ps.UserID)) continue;

                userids.Add(ps.UserID);
            }

            int[] result = new int[userids.Count];

            userids.CopyTo(result);

            return result;
        }

        #endregion
    }
}