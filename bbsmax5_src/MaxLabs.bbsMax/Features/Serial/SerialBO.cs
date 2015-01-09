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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax
{
    public class SerialBO : BOBase<SerialBO>
    {
        public MaxSerial GetSerial(Guid serialGuid, SerialType serialType)
        {
            MaxSerial serial = SerialDao.Instance.GetSerial(serialGuid, serialType);

            return serial;
        }

        public MaxSerial GetSerial(string serial, SerialType serialType)
        {
            Guid serialGuid;
            try
            {
                serialGuid = new Guid(serial);
            }
            catch
            {
                return null;
            }
            return GetSerial(serialGuid, serialType);
        }

        public MaxSerial CreateSerial(int ownerUserId, DateTime expriseDate, SerialType serialType)
        {
            bool success=false;
            return CreateSerial(ownerUserId, expriseDate, serialType, null, out success);
        }

        public MaxSerial CreateSerial(int ownerUserId, DateTime expriseDate, SerialType serialType, string data, out bool success)
        {
            return SerialDao.Instance.CreateSerial(ownerUserId, expriseDate, serialType, data, out success);
        }


        public bool DeleteSerial(int userID, SerialType serialType)
        {
            return SerialDao.Instance.DeleteSerial(userID, serialType);
        }

        //public bool DeleteSerial(Guid serialGuid)
        //{
        //    return SerialDao.Instance.DeleteSerial(serialGuid);
        //}

        //public bool DeleteSerial(string serial)
        //{
        //    Guid serialGuid;
        //    try
        //    {
        //        serialGuid = new Guid(serial);
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return DeleteSerial(serialGuid);
        //}
    }
}