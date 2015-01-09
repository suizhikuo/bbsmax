//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace MaxLabs.WebEngine.Plugin
{
    /// <summary>
    /// 动作处理器回调方法
    /// </summary>
    /// <param name="args">动作处理器调用参数</param>
    /// <returns>动作处理器返回结果</returns>
    public delegate ActionHandlerResult ActionHandler<Action>(Action args);

    /// <summary>
    /// 动作处理器信息
    /// </summary>
    public class ActionHandlerInfo<Action>
    {
        public ActionHandlerInfo(ActionHandler<Action> handler, int priority)
        {
            Priority = priority;
            Handler = handler;
        }

        /// <summary>
        /// 处理器回调方法
        /// </summary>
        public ActionHandler<Action> Handler { get; set; }

        /// <summary>
        /// 优先级，值低的先被调用
        /// </summary>
        public int Priority { get; set; }
    }
}