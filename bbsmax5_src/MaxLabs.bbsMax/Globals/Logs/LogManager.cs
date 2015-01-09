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

using MaxLabs.bbsMax.Common;
using System.Reflection;
using MaxLabs.bbsMax.Settings;
using System.Collections;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.Logs
{
	public static class LogManager
	{
        public static void LogUserIPChanged(UserIPLog log)
        {
            OperationLogDao.Instance.LogUserIPChanged(log);
        }

        public static UserIPLogCollection GetUserIPLogsBySearch(UserIPSearchFilter filter, int pagenumber)
        {
            return OperationLogDao.Instance.GetUserIPLogsBySearch(filter, pagenumber);
        }

        public static UserIPLogCollection GetUserIPLogsByIP(string IP)
        {
            return OperationLogDao.Instance.GetUserIPLogsByIP(IP);
        }

        public static UserIPLogCollection GetUserIPLogsByIP(string IP, int pageNumber, int pageSize, out int total)
        {
            return OperationLogDao.Instance.GetUserIPLogsByIP(IP, pageNumber, pageSize, out total);
        }

        public static void DeleteUserIPLog(JobDataClearMode mode, DateTime dateTime, int saveRows)
        {
            OperationLogDao.Instance.DeleteUserIPLog(mode, dateTime, saveRows);
        }

		public static string LogException(Exception ex)
		{
			return LogHelper.CreateErrorLog(ex);
		}

		public static void LogOperation(Operation op)
		{
			OperationLogDao.Instance.CreateOperationLog(op);
		}


		private static object s_GetOperationTypeInfoLocker = new object();

		public static OperationTypeInfo[] GetOperationTypeInfos()
		{
			OperationTypeInfo[] result = null;

			if (CacheUtil.TryGetValue<OperationTypeInfo[]>("OperationTypeInfos", out result))
				return result;

			lock (s_GetOperationTypeInfoLocker)
			{
				if (CacheUtil.TryGetValue<OperationTypeInfo[]>("OperationTypeInfos", out result))
					return result;

				List<OperationTypeInfo> infos = new List<OperationTypeInfo>();
                Hashtable flags = new Hashtable();

				Assembly currentAssembly = Assembly.GetExecutingAssembly();

				Type typeOfOperation = typeof(Operation);
				Type typeOfOperationTypeAttribute = typeof(OperationTypeAttribute);

				foreach (Type type in currentAssembly.GetTypes())
				{
					if (type.IsDefined(typeOfOperationTypeAttribute, true) && type.IsSubclassOf(typeOfOperation))
					{
						OperationTypeAttribute info = (OperationTypeAttribute)OperationTypeAttribute.GetCustomAttribute(type, typeof(OperationTypeAttribute), true);

						OperationTypeInfo i = new OperationTypeInfo(type.FullName, info);

                        if (flags.ContainsKey(info.DisplayName) == false)
                        {
                            infos.Add(i);
                            flags.Add(info.DisplayName, true);
                        }
					}
				}

				infos.Sort();

				result = infos.ToArray();

				CacheUtil.Set<OperationTypeInfo[]>("OperationTypeInfos", result);

				return result;
			}
		}

		public static OperationLogCollection GetOperationLogsBySearch(int searchOperatorID, int pageNumber, OperationLogFilter filter)
		{
			return OperationLogDao.Instance.GetOperationLogsBySearch(pageNumber, filter);
		}

		internal static void DeleteOperationLogs(JobDataClearMode mode, DateTime dateTime, int saveRows)
		{
			OperationLogDao.Instance.DeleteOperationLogs(mode, dateTime, saveRows);
		}

#if !Passport
        public static BanUserOperationCollection GetBanUserOperationCollectionByUserID(int userid)
        {
            return OperationLogDao.Instance.GetBanUserLogsByUserID(userid);
        }

        public static List<BanForumInfo> GetBanForumInfos(int banLogID)
        {
            return  OperationLogDao.Instance.GetBanForumInfos(banLogID);
        }

        public static BanUserOperationCollection GetBanUserLogsBySearch(BanUserLogFilter filter, int pagenumber)
        {
            return OperationLogDao.Instance.GetBanUserLogsBySearch(filter, pagenumber);
        }

        public static void DeleteBanUserOperationLogs(JobDataClearMode mode, DateTime datetime, int saveRows)
        {
             OperationLogDao.Instance.DeleteBanUserOperationLogs(mode, datetime, saveRows);
        }
#endif
        public static UserMobileLogCollection GetUserMobileLogsBySearch(UserMobileSearchFilter filter, int pagenumber)
        {
           return OperationLogDao.Instance.GetUserMobileLogsBySearch(filter, pagenumber);
        }

        public static void DeleteUserMobileOperationLogs(JobDataClearMode mode, DateTime datetime, int saveRows)
        {
            OperationLogDao.Instance.DeleteUserMobileOperationLogs(mode, datetime, saveRows);
        }

        public static UserGetPropCollection GetUserGetPropCollection(UserGetPropFilter filter, int page)
        {
            return OperationLogDao.Instance.GetUserGetPropCollection(filter, page);
        }


	}
}