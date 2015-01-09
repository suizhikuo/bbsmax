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
	/// 搜索文章的方式
	/// </summary>
	public enum SearchArticleMethod
	{
        /// <summary>
        /// 不限制
        /// </summary>
        Default = 0,
		/// <summary>
		/// 搜索标题
		/// </summary>
        Subject = 1,
		/// <summary>
		/// 搜索正文
		/// </summary>
        FullText = 2
	}
}