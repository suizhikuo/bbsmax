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
    public abstract class UserTempDataDao : DaoBase<UserTempDataDao>
    {
        public abstract UserTempData GetTemporaryData(int userID, UserTempDataType dataType);
        public abstract UserTempData GetTemporaryDataWithExpiresDate(int userID, UserTempDataType dataType);
        
        public abstract void Delete(int userID, UserTempDataType dataType);
        public abstract void Delete(IEnumerable<int> userIds, UserTempDataType dataType);
        public abstract void Delete(UserTempDataType dataType);
        public abstract void DeleteUserDatas(int userID);
        public abstract void DeleteByType(UserTempDataType dataType);
        public abstract void DeleteExpiresData();
        public abstract void SaveData(UserTempData data);
        public abstract void SaveData(UserTempData data, bool overrideOldData);
        public abstract void SaveData(int userID, UserTempDataType dataType, object data, bool overrideOld);

        public abstract int SaveBindOrUnbindSmsCode(int userID, MobilePhoneAction action, long newMobilePhone, string newSmsCode, long oldMobilePhone, string oldSmsCode);
        public abstract void ChangePhoneBySmsCode(int userID, long newMobilePhone, string newSmsCode, long oldMobilePhone, string oldSmsCode, out bool oldSuccess, out bool newSuccess);
        public abstract void SetPhoneBySmsCode(int userID, long mobilePhone, string smsCode, MobilePhoneAction action, out bool success);
    }
}