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

namespace MaxLabs.WebEngine.Plugin
{
    /// <summary>
    /// 动作处理器返回结果
    /// </summary>
    public class ActionHandlerResult
    {
        public ActionHandlerResult()
        {
            Bubble = true;
        }

        /// <summary>
        /// 是否冒泡执行，默认为true，如果不冒泡执行将终止后续处理器的调用
        /// </summary>
        public bool Bubble { get; set; }

		/// <summary>
		/// 获取插件执行过程中是否发生错误
		/// </summary>
		public bool HasError
		{
			get { return ErrorMessage != null; }
		}

		/// <summary>
		/// 获取或设置插件执行过程中发生的错误
		/// </summary>
		public string ErrorMessage { get; set; }
    }
}