//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text.RegularExpressions;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class DatabaseInfoDao : DataAccess.DatabaseInfoDao
    {

        /// <summary>
        /// 数据库版本号信息
        /// </summary>
        public override string GetDatabaseVersion()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT @@VERSION;";
                return query.ExecuteScalar<string>();
            }
        }

        /// <summary>
        /// 获取数据库时间和标准时间的时差（秒）
        /// </summary>
        /// <returns></returns>
        public override int GetDatabaseTimeDifference()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT DATEDIFF(minute,GETUTCDATE(),GETDATE())";
                return query.ExecuteScalar<int>();
            }
        }

        /// <summary>
        /// 获取系统启动时数据库当前时间与程序时间的相差毫秒数
        /// </summary>
        public override double GetTimeIntervalFromDatabase()
        {
            using (SqlSession session = new SqlSession())
            {
                string connectionString = session.Connection.ConnectionString;

                Regex regexConnString1 = new Regex(@"^.*\s?(?>Server|Data\s+Source)\s*=\s*(?>\.|\(local\)|127\.0\.0\.1|localhost|::1)\s*;.*?", RegexOptions.IgnoreCase);

                if (regexConnString1.IsMatch(connectionString))
                {
                    return 0.0f;
                }

                using (SqlQuery query = new SqlQuery())
                {
                    query.CommandText = "SELECT GETDATE();";
                    return (query.ExecuteScalar<DateTime>() - DateTimeUtil.Now).TotalMilliseconds;
                }
            }
        }


        public override DataBaseInfo GetDataBaseInfo()
        {
            string version = "";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT size*8,(status & 0x40) as type FROM sysfiles;SELECT @@VERSION";
                DataBaseInfo dataBaseInfo = new DataBaseInfo();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    int dataSize = 0;
                    int logoSize = 0;
                    while (reader.Read())
                    {
                        if (reader.GetInt32(1) == 0)
                            dataSize += reader.GetInt32(0);
                        else
                            logoSize += reader.GetInt32(0);
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            version += "(" + reader.GetString(0) + ")";
                    }
                    dataBaseInfo.DataSize = dataSize * 1024;
                    dataBaseInfo.LogSize = logoSize * 1024;
                    dataBaseInfo.Version = version;
                }
                return dataBaseInfo;
            }
        }
    }
}