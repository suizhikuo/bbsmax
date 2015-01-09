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

using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Errors
{
	public class InvalidIPMatchRegulationError : ErrorInfo
	{
		public InvalidIPMatchRegulationError(int[] lineNums)
		{
			LineNums = lineNums;
		}

		public int[] LineNums
		{
			get;
			private set;
		}

		public override string Message
		{
			get 
			{
				StringBuilder sb = new StringBuilder();

				sb.Append("第");

				for (int i = 0; i < LineNums.Length; i++)
				{
					sb.Append(i);

					if (i < LineNums.Length - 1)
						sb.Append(",");
				}

				sb.Append("行的IP匹配规则格式无效");

				return sb.ToString();
			}
		}
	}
}