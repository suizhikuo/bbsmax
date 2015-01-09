//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.Logs
{
	public abstract class OperationLogDao : DaoBase<OperationLogDao>
	{
		public abstract void CreateOperationLog(Operation op);

		public abstract OperationLogCollection GetOperationLogsBySearch(int pageNumber, OperationLogFilter filter);

		public abstract void DeleteOperationLogs(JobDataClearMode mode, DateTime dateTime, int saveRows);



        public abstract void LogUserIPChanged(UserIPLog log);

        public abstract UserIPLogCollection GetUserIPLogsBySearch(UserIPSearchFilter filter,int pagenumber);

        public abstract void DeleteUserIPLog(JobDataClearMode mode, DateTime dateTime, int saveRows);

        public abstract UserIPLogCollection GetUserIPLogsByIP(string IP);


        public abstract UserIPLogCollection GetUserIPLogsByIP(string IP, int pageNumber, int pageSize, out int total);

#if !Passport
        public abstract BanUserOperationCollection GetBanUserLogsByUserID(int userid);
        public abstract void DeleteBanUserOperationLogs(JobDataClearMode mode, DateTime datetime, int saveRows);
        public abstract List<BanForumInfo> GetBanForumInfos(int banLogID);
        public abstract BanUserOperationCollection GetBanUserLogsBySearch(BanUserLogFilter filter, int pagenumber);
#endif
        //获取手机验证用户日志.
        public abstract UserMobileLogCollection GetUserMobileLogsBySearch(UserMobileSearchFilter filter, int pagenumber);


        public abstract void DeleteUserMobileOperationLogs(JobDataClearMode mode, DateTime datetime, int saveRows);


        public abstract UserGetPropCollection GetUserGetPropCollection(UserGetPropFilter filter, int pageNumber);

	}
}