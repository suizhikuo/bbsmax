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
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.StepByStepTasks
{
	public class DeleteAlbumTask : StepByStepTaskBase
	{
#if DEBUG

		const int stepDeleteCount = 1;

#else

        const int stepDeleteCount = 200;

#endif


		public override StepByStepTaskBase CreateInstance()
		{
			return new DeleteAlbumTask();
		}

		public override TaskType InstanceMode
		{
			get { return TaskType.SystemSingleInstance; }
		}

		public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
		{
			StringList paramData = StringList.Parse(param);

			AdminAlbumFilter filter = AdminAlbumFilter.Parse(paramData[0]);

			//只取一条数据测试下就可以
			filter.PageSize = 1;

			AlbumCollection albums = AlbumBO.Instance.GetAlbumsForAdmin(operatorUserID, filter, 1);

			if (albums == null || albums.Count == 0)
			{
				title = "没有数据可以删除";
				return false;
			}

			totalCount = albums.TotalRecords;

			title = "将删除 " + totalCount + " 个相册";

			return true;
		}

		public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
		{
			StringList paramData = StringList.Parse(param);

			AdminAlbumFilter filter = AdminAlbumFilter.Parse(paramData[0]);
			
			int stepCount;

			if (AlbumBO.Instance.DeleteAlbumsForAdmin(operatorUserID, filter, paramData[1] == "1", stepDeleteCount, out stepCount)) // .DeleteDoingsBySearch(filter, 200);
			{
				finishedCount += stepCount;

				isLastStep = stepCount < stepDeleteCount;

				title = "正在删除相册，总数 " + totalCount + "，已删 " + finishedCount;
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
			{
				title = "删除相册成功，共删除 " + finishedCount + " 个相册";

				StringList paramData = StringList.Parse(param);

				AdminAlbumFilter filter = AdminAlbumFilter.Parse(paramData[0]);
			
				if (success)
				{
                    User operatorUser = UserBO.Instance.GetUser(operatorUserID, GetUserOption.WithAll);

					Logs.LogManager.LogOperation(
						new Album_DeleteAlbumBySearch(operatorUserID, operatorUser.Name, IPUtil.GetCurrentIP(), filter, finishedCount)
					);
				}
			}
			else
				title = "删除相册失败";
		}
	}
}