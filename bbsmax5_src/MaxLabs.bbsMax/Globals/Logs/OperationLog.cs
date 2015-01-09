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

using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Logs
{
	public class OperationLog
	{
		public OperationLog(DataReaderWrap reader)
		{
			OperatorID = reader.Get<int>("OperatorID");
			TargetID_1 = reader.Get<int>("TargetID_1");
			TargetID_2 = reader.Get<int>("TargetID_2");
			TargetID_3 = reader.Get<int>("TargetID_3");
			OperatorIP = reader.Get<string>("OperatorIP");
			CreateTime = reader.Get<DateTime>("CreateTime");
			Message = reader.Get<string>("Message");
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

		//private string m_Message;

		public string Message
		{
			get;
			private set;
		}
	}

	public class OperationLogCollection : Collection<OperationLog>
	{
		public OperationLogCollection(DataReaderWrap reader)
		{
			while (reader.Next)
			{
				this.Add(new OperationLog(reader));
			}
		}

		public int TotalRecords { get; set; }
	}
}