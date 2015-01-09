//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using MaxLabs.bbsMax.Entities;



namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class DatabaseInfoDao : DaoBase<DatabaseInfoDao>
    {
        /// <summary>
        /// 数据库版本号信息
        /// </summary>
        public abstract string GetDatabaseVersion();

        /// <summary>
        /// 获取系统启动时程序时间与数据库当前时间的相差毫秒数
        /// </summary>
        public abstract double GetTimeIntervalFromDatabase();

        /// <summary>
        /// 获取数据库时间和标准时间的时差（毫秒）
        /// </summary>
        /// <returns></returns>
        public abstract int GetDatabaseTimeDifference();

        public abstract DataBaseInfo GetDataBaseInfo();
    }
}