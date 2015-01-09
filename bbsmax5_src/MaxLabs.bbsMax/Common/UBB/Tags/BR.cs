//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

 /*
 * ������: ���
 * ����ʱ��: 2008-5-26 10:58
 * ��Ȩ����: MaxLab.
 */

using System;

namespace MaxLabs.bbsMax.Ubb
{
	/// <summary>
	/// [br]
	/// </summary>
	public class BR: UbbTagHandler
	{
		public override string UbbTagName {
			get { return "br"; }
		}
		
		public override string HtmlTagName {
			get { return "br"; }
		}
		
		public override UbbTagHandler GetInstance {
			get { return this; }
		}
		
		public override bool IsSingleHtmlTag {
			get { return true; }
		}

		public override bool IsSingleUbbTag {
			get { return true; }
		}
	}
}