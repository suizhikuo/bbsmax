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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Logs
{
	[OperationType(AdminLoginOperation.TYPE_NAME)]
	public class User_IPChange : Operation
	{
        public const string TYPE_NAME = "用户IP变更";

        public User_IPChange(int operatorID, string operatorName, string oldIP, string newIP)
			: base(operatorID, operatorName, newIP)
		{
            OldIP = oldIP;
		}

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

        public string OldIP
        {
            get;
            private set;
        }

		public override string Message
		{
			get
			{
				return string.Format(
					"<a href=\"{0}\" target=\"_blank\">{1}</a> 的IP地址由 {2} 变为 {3}"
					, BbsRouter.GetUrl("space/" + OperatorID)
					, OperatorName
                    , this.OldIP
                    , this.OperatorIP
				);
			}
		}
	}
}