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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax
{
    public class PointLogBO:BOBase<PointLogBO>
    {
        public PointLogCollection GetPointLogs(AuthUser operateUser, PointLogFilter filter,int pageNumber)
        {
            if (pageNumber <= 0)
                pageNumber = 1;

            if (filter.PageSize <= 0)
                filter.PageSize = Consts.DefaultPageSize;

            return PointLogDao.Instance.GetPointLogs(filter, pageNumber);
        }

        public PointLogTypeCollection GetPointLogTypes()
        {
            return PointLogDao.Instance.GetPointLogTypes();
        }

        public void GetPointStatInfo(PointLogFilter filter, UserPointType pointIndex, out int count, out int produceCount, out int consumeCount)
        {
            PointLogDao.Instance.GetPointStatInfo(filter,(int) pointIndex, out count, out produceCount, out consumeCount);
        }

        public void ClearPointLogData()
        {
            PointLogClearSettings settings = AllSettings.Current.PointLogClearSettings;
            PointLogDao.Instance.ClearPointLogs(settings.SaveLogDays, settings.SaveLogRows, settings.DataClearMode);
        }
    }
}