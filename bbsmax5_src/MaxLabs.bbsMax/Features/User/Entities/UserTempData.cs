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
using System.Collections;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    public class UserTempData
    {
        public UserTempData(){}

        public UserTempData( DataReaderWrap readerWrap)
        {
            this.UserID = readerWrap.Get<int>("UserID");
            this.DataType = readerWrap.Get<UserTempDataType>("DataType");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.Data = readerWrap.Get<string>("Data");
        }

        public int UserID
        {
            get;
            set;
        }

        public UserTempDataType DataType
        {
            get;
            set;
        }

        public DateTime CreateDate
        {
            get;
            set;
        }

        public virtual string Data
        {
            get;
            set;
        }

        public DateTime ExpiresDate
        {
            get;
            set;
        }
    }

    public class TemporaryDataCollection : Collection<UserTempData>
    {
        public TemporaryDataCollection()
        {

        }

        public TemporaryDataCollection(DataReaderWrap readWrap)
        {
            while (readWrap.Next)
            {
                this.Add(new UserTempData(readWrap));
            }
        }
    }
}