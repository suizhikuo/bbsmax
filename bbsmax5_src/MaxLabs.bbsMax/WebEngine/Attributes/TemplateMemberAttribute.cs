//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * 创建者: 达达
 * 创建时间: 2008-3-26 13:56
 * 版权归属: MaxLabs.
 */

using System;

namespace MaxLabs.WebEngine
{
	/// <summary>
	/// 用于标识模板成员
	/// </summary>
	public abstract class TemplateMemberAttribute : Attribute
	{
		private string m_Name;

		/// <summary>
		/// 对应的模板成员名称
		/// </summary>
		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}
	}
}