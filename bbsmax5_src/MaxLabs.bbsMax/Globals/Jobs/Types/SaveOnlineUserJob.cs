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

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 任务 类型
    /// </summary>
    public class SaveOnlineUserJob : JobBase
    {

        public override ExecuteType ExecuteType
        {
            get { return ExecuteType.InThread; }
        }

        public override TimeType TimeType
        {
            get { return TimeType.Interval; }
        }

        public override bool Enable
        {
            get 
            {
                return true;
            }
        }

        public override void Action()
        {
            try
            {
                OnlineUserPool instance = OnlineUserPool.Instance;

                //清理已经超时的用户
                instance.ClearExpiredData();

                //备份在线列表以便重启后恢复
                instance.Backup();
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }
        }

        protected override void SetTime()
        {
            SetIntervalExecuteTime(60);
        }
    }

}