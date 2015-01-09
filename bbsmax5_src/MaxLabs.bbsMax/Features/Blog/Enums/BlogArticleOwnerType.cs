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

namespace MaxLabs.bbsMax.Enums
{
	/// <summary>
	/// 日志/相册/文件等等的拥有者枚举
	/// </summary>
	public enum BlogArticleOwnerType
	{
		/// <summary>
		/// 我的
		/// </summary>
		Self = 0,
		/// <summary>
		/// 好友的
		/// </summary>
		Friend = 1,
		/// <summary>
		/// 我访问过的
		/// </summary>
		Trace = 2,
		/// <summary>
		/// 大家的
		/// </summary>
		All = 3
	}
}