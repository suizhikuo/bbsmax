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

using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.StepByStepTasks.Types
{
    public class DeleteImpressionRecordTask : StepByStepTaskBase
	{
#if DEBUG

		const int stepDeleteCount = 10;

#else

        const int stepDeleteCount = 200;

#endif

		public override StepByStepTaskBase CreateInstance()
		{
            return new DeleteImpressionRecordTask();
		}

		public override TaskType InstanceMode
		{
			get { return TaskType.SystemSingleInstance; }
		}

		public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
		{
			StringList paramData = StringList.Parse(param);

            AdminImpressionRecordFilter filter = AdminImpressionRecordFilter.Parse(paramData[0]);

            AuthUser operatorUser = UserBO.Instance.GetAuthUser(operatorUserID);

            ImpressionRecordCollection types = ImpressionBO.Instance.GetImpressionRecordsForAdmin(operatorUser, filter, 1);

			if (types == null || types.Count == 0)
			{
				title = "没有数据可以删除";
				return true;
			}

			totalCount = types.TotalRecords;

			title = "将删除 " + totalCount + " 条好友印象记录";

			return true;
		}

		public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
		{
			StringList paramData = StringList.Parse(param);

            AdminImpressionRecordFilter filter = AdminImpressionRecordFilter.Parse(paramData[0]);

			int stepCount;

            AuthUser operatorUser = UserBO.Instance.GetAuthUser(operatorUserID);

            if (ImpressionBO.Instance.DeleteImpressionRecordsForAdmin(operatorUser, filter, stepDeleteCount, out stepCount)) // .DeleteDoingsBySearch(filter, 200);
			{
				finishedCount += stepCount;

				isLastStep = stepCount < stepDeleteCount;

				title = "正在删除好友印象记录，总数 " + totalCount + "，已删 " + finishedCount;

				return true;
			}
			else
			{
				isLastStep = true;
				title = string.Empty;
			}

			return true;
		}

		public override void AfterExecute(int operatorUserID, string param, bool success, int totalCount, int finishedCount, out string title)
		{
			if (success)
                title = "删除好友印象记录成功，共删除 " + finishedCount + " 条好友印象记录";
			else
                title = "删除好友印象记录失败";
		}
	}
}