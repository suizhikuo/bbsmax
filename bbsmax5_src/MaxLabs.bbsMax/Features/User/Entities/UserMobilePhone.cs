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
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    public class UserMobilePhone
    {

        public UserMobilePhone()
        { }

        public UserMobilePhone(DataReaderWrap readerWrap)
        {
            this.UserID = readerWrap.Get<int>("UserID");
            this.DataType = readerWrap.Get<MobilePhoneAction>("DataType");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.MobilePhone = readerWrap.Get<long>("MobilePhone");
            this.PhoneCode = readerWrap.Get<string>("PhoneCode");
            this.ExpiresDate = readerWrap.Get<DateTime>("ExpiresDate");
        }

        public int UserID
        {
            get;
            set;
        }

        public MobilePhoneAction DataType
        {
            get;
            set;
        }

        public DateTime CreateDate
        {
            get;
            set;
        }

        public long MobilePhone
        {
            get;
            set;
        }

        public string PhoneCode
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

    public class UserMobilePhoneCollection : Collection<UserMobilePhone>
    {
        public UserMobilePhoneCollection() { }

        public UserMobilePhoneCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new UserMobilePhone(readerWrap));
            }
        }
    }
}