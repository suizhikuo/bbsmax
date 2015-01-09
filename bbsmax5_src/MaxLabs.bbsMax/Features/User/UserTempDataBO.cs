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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax
{
   public class UserTempDataBO:BOBase<UserTempDataBO>
    {
       public void SaveData( int userID, UserTempDataType dataType, object data, bool overrideOld)
       {
           if (userID <= 0)
               return;

           UserTempDataDao.Instance.SaveData(userID, dataType, data, overrideOld);
       }

       public UserTempData GetTempData(int targetUserID, UserTempDataType dataType)
       {
           return UserTempDataDao.Instance.GetTemporaryData(targetUserID, dataType);
       }

       public void Delete(IEnumerable<int> userIds, UserTempDataType dataType)
       {
           if (ValidateUtil.HasItems<int>(userIds))
           {
               UserTempDataDao.Instance.Delete(userIds, dataType);
           }
       }

       public void GetDataByUserIds(IEnumerable<int> userIds, UserTempDataType dataType)
       {
           
       }

       public void Delete(int userID, UserTempDataType dataType)
       {
           if (userID <= 0) return;
           UserTempDataDao.Instance.Delete(userID, dataType);
       }

       public void Delete(UserTempDataType dataType)
       {
           UserTempDataDao.Instance.Delete( dataType);
       }
    }
}