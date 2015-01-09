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
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    public class UserMobileLog : IPrimaryKey<int>
    {
        public UserMobileLog() { }

        public UserMobileLog(DataReaderWrap wrap)
        {
            this.LogID = wrap.Get<int>("LogID");
            this.UserID = wrap.Get<int>("UserID");
            this.Username = wrap.Get<string>("Username");
            this.MobilePhone = wrap.Get<long>("MobilePhone");
            this.OperationType = wrap.Get<MobilePhoneAction>("OperationType");
            this.OperationDate = wrap.Get<DateTime>("OperationDate");
        }

        public int LogID { get; set; }

        public int UserID { get; set; }

        public string Username { get; set; }

        public long MobilePhone { get; set; }

        public MobilePhoneAction OperationType { get; set; }

        public DateTime OperationDate { get; set; }


        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return LogID;
        }

        #endregion
    }

    public class UserMobileLogCollection : EntityCollectionBase<int,UserMobileLog>
    {
        public UserMobileLogCollection() { }

        public UserMobileLogCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
            {
                this.Add(new UserMobileLog(wrap));
            }
        }
    }


}