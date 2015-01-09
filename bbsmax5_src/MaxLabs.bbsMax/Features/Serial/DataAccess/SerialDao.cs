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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class SerialDao : DaoBase<SerialDao>
    {

        public abstract MaxSerial CreateSerial(int ownerUserId, DateTime expriseDate, SerialType type, string data,out bool success);

        public abstract MaxSerial GetSerial(Guid serial, SerialType serialType);

        //public abstract bool DeleteSerial(Guid serialGuid);

        public abstract bool DeleteSerial(int userID, SerialType serialType);
    }
}