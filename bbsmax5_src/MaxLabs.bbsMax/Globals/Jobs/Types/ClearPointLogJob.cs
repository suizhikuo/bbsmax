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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Jobs
{
    public  class ClearPointLogJob:JobBase
    {

        public override ExecuteType ExecuteType
        {
            get { return ExecuteType.AfterRequest; }
        }

        public override TimeType TimeType
        {
            get { return TimeType.Day; }
        }

        public override bool Enable
        {
            get { return true; }
        }

        protected override void SetTime()
        {
            SetDayExecuteTime(Settings.AllSettings.Current.PointLogClearSettings.ExecuteHour, AllSettings.Current.PointLogClearSettings.ExecuteMinute, 0);
        }

        public override void Action()
        {
            try
            {
                  PointLogBO.Instance.ClearPointLogData();
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }
        }
    }
}