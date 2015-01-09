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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class RoleUser:IPrimaryKey<int>
    {
        public RoleUser()
        {
            this.BeginDate = DateTime.MinValue;
            this.EndDate = DateTime.MaxValue;
        }

        public RoleUser(DataReaderWrap readerWrap)
        {
            this.RoleID = readerWrap.Get<int>("RoleID");
            this.BeginDate = readerWrap.Get<DateTime>("BeginDate");
            this.EndDate = readerWrap.Get<DateTime>("EndDate");
            this.UserID = readerWrap.Get<int>("UserID");
        }

        public int UserID { get; set; }

        public int RoleID { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }


        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return RoleID;
        }

        #endregion
    }

    public class RoleUserCollection : EntityCollectionBase<int, RoleUser>
    {
        public RoleUserCollection() { }

        public RoleUserCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
                Add(new RoleUser(wrap));
        }
    }


}