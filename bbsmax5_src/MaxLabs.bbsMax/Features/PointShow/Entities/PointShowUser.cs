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
    public class PointShowUser:IPrimaryKey<int>, IFillSimpleUser
    {
        public PointShowUser()
        {

        }

        public PointShowUser(DataReaderWrap readerWrap)
        {
        }

        public int UserID { get; set; }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.UserID);
            }
        }

        public string Content { get; set; }

        public int Price { get; set; }


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


    public class PointShowUserCollection : EntityCollectionBase<int, PointShowUser>
    {

        public PointShowUserCollection()
        {

        }

        public PointShowUserCollection(DataReaderWrap readerWrap)
        {
            while ( readerWrap.Next )
            {
                this.Add(new PointShowUser(readerWrap));
            }
        }

        #region IFillSimpleUsers 成员

        public int[] GetUserIdsForFill(int actionType)
        {
            List<int> userids = new List<int>();

            foreach (PointShowUser ps in this)
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