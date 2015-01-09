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
    public enum TaskType : byte
    {
        /// <summary>
        /// 用户单实例模式，即每个用户只能同时运行一个该类型的任务，自己只能查看自己发起的任务
        /// </summary>
        UserSingleInstance = 1,

        /// <summary>
        /// 系统单实例模式，即整个系统中只能同时运行一个该类型的任务，可以查看其他人发起的任务
        /// </summary>
        SystemSingleInstance = 2,

        /// <summary>
        /// 用户多实例模式，即每个用户可同时运行多个该类型的任务，但自己只能查看自己发起的任务
        /// </summary>
        UserMultipleInstances = 3,

        /// <summary>
        /// 系统多实例模式，即整个系统中可同时运行多个该类型的任务，且可以查看其他人发起的任务
        /// </summary>
        SystemMultipleInstances = 4,
    }
}