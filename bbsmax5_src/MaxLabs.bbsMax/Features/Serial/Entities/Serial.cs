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

namespace MaxLabs.bbsMax.Entities
{
    public class MaxSerial
    {

        public MaxSerial() { }

        public MaxSerial(DataReaderWrap wrap)
        {
            this.CreateDate = wrap.Get<DateTime>("CreateDate");
            this.ExpiresDate = wrap.Get<DateTime>("ExpiresDate");
            this.OwnerUserId = wrap.Get<int>("UserID");
            this.Serial = wrap.Get<Guid>("Serial");
            this.Type = wrap.Get<SerialType>("Type");
            this.Data = wrap.Get<string>("Data");
        }

        public string Data
        {
            get;
            set;
        }

        public int OwnerUserId
        {
            get;
            set;
        }
        public DateTime CreateDate
        {
            get;
            set;
        }
        public DateTime ExpiresDate
        {
            get;
            set;
        }
        public SerialType Type
        {
            get;
            set;
        }
        public Guid Serial
        {
            get;
            set;
        }
    }
}