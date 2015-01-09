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

namespace MaxLabs.bbsMax.StepByStepTasks
{

    public abstract class StepByStepTaskBase
    {

        public abstract StepByStepTaskBase CreateInstance();

        /// <summary>
        /// 任务实例的运行方式
        /// </summary>
        public abstract TaskType InstanceMode { get; }

        /// <summary>
        /// 单步执行的超时时间，单位为秒，如果超过这个时间服务器仍没有响应，则刷新重试
        /// </summary>
        public virtual int Timeout { get { return 60; } }

        /// <summary>
        /// 在分批执行前执行的一次性操作
        /// </summary>
        /// <param name="userID"></param>
        public abstract bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title);

        /// <summary>
        /// 根据传入的偏移位置执行本批次的任务，并得到下批执行的起始偏移位置。
        /// 同时返回本次操作执行了多少数据量，如果不返回将无法正确显示当前的执行进度
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="offset"></param>
        /// <param name="nextOffset"></param>
        /// <returns></returns>
        public abstract bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep);

        /// <summary>
        /// 在分批执行后执行的一次性操作（无论分批执行是否中途发生错误都始终会执行）
        /// </summary>
        /// <param name="userID"></param>
        public abstract void AfterExecute(int operatorUserID, string param, bool success, int totalCount, int finishedCount, out string title);
    }
}