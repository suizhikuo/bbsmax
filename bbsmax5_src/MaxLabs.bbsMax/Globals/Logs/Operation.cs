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
using System.Collections;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Logs
{
	public abstract class Operation
	{
		public Operation(int operatorID, string operatorName, string operatorIP)
		{
			OperatorID = operatorID;
			OperatorName = operatorName;
			OperatorIP = operatorIP;

			CreateTime = DateTimeUtil.Now;
		}

		public Operation(int operatorID, string operatorName, string operatorIP, int targetID_1)
		{
			OperatorID = operatorID;
			OperatorName = operatorName;
			OperatorIP = operatorIP;
			TargetID_1 = targetID_1;

			CreateTime = DateTimeUtil.Now;
		}

		public Operation(int operatorID, string operatorName, string operatorIP, int targetID_1, int targetID_2)
		{
			OperatorID = operatorID;
			OperatorName = operatorName;
			OperatorIP = operatorIP;
			TargetID_1 = targetID_1;
			TargetID_2 = targetID_2;

			CreateTime = DateTimeUtil.Now;
		}

		public Operation(int operatorID, string operatorName, string operatorIP, int targetID_1, int targetID_2, int targetID_3)
		{
			OperatorID = operatorID;
			OperatorName = operatorName;
			OperatorIP = operatorIP;
			TargetID_1 = targetID_1;
			TargetID_2 = targetID_2;
			TargetID_3 = targetID_3;

			CreateTime = DateTimeUtil.Now;
		}

		public int OperatorID
		{
			get;
			private set;
		}

		public string OperatorIP
		{
			get;
			private set;
		}

		public string OperatorName
		{
			get;
			private set;
		}

		public int? TargetID_1
		{
			get;
			private set;
		}

		public int? TargetID_2
		{
			get;
			private set;
		}

		public int? TargetID_3
		{
			get;
			private set;
		}

		public DateTime CreateTime
		{
			get;
			private set;
		}

		public abstract string Message
		{
			get;
		}

        public abstract string TypeName
        {
            get;
        }
	}
}