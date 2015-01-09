//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 任务 类型
    /// </summary>
    public class DeleteOperationLogJob : JobBase
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
            get 
            {
				return Settings.AllSettings.Current.DeleteOperationLogJobSettings.DataClearMode != MaxLabs.bbsMax.Enums.JobDataClearMode.Disabled;
            }
        }

        public override void Action()
        {
            try
            {
				Logs.LogManager.DeleteOperationLogs(
                    AllSettings.Current.DeleteOperationLogJobSettings.DataClearMode,
                    DateTimeUtil.Now.AddDays(0 - AllSettings.Current.DeleteOperationLogJobSettings.SaveLogDays),
                    AllSettings.Current.DeleteOperationLogJobSettings.SaveLogRows);
                //删除用户IP日志
                Logs.LogManager.DeleteUserIPLog(
                   AllSettings.Current.DeleteOperationLogJobSettings.DataClearMode,
                   DateTimeUtil.Now.AddDays(0 - AllSettings.Current.DeleteOperationLogJobSettings.SaveLogDays),
                   AllSettings.Current.DeleteOperationLogJobSettings.SaveLogRows);
                //删除用户屏蔽日志
                Logs.LogManager.DeleteBanUserOperationLogs(
                   AllSettings.Current.DeleteOperationLogJobSettings.DataClearMode,
                   DateTimeUtil.Now.AddDays(0 - AllSettings.Current.DeleteOperationLogJobSettings.SaveLogDays),
                   AllSettings.Current.DeleteOperationLogJobSettings.SaveLogRows);
                Logs.LogManager.DeleteUserMobileOperationLogs(
                   AllSettings.Current.DeleteOperationLogJobSettings.DataClearMode,
                   DateTimeUtil.Now.AddDays(0 - AllSettings.Current.DeleteOperationLogJobSettings.SaveLogDays),
                   AllSettings.Current.DeleteOperationLogJobSettings.SaveLogRows);
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }
        }

        protected override void SetTime()
        {
			SetDayExecuteTime(Settings.AllSettings.Current.DeleteOperationLogJobSettings.ExecuteTime, 0, 0);
        }
    }

}