//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Logs
{
	[OperationType(Feed_DeleteFeedBySearch.TYPE_NAME)]
	public class Feed_DeleteFeedBySearch : Operation
	{
        public const string TYPE_NAME = "删除动态";

        public Feed_DeleteFeedBySearch(int operatorID, string operatorName, string operatorIP, Filters.FeedSearchFilter filter, int totalDeleted)
			: base(operatorID, operatorName, operatorIP)
		{
			Filter = filter;
			TotalDeleted = totalDeleted;
		}

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

		public Filters.FeedSearchFilter Filter
		{
			get;
			private set;
		}

		public int TotalDeleted
		{
			get;
			private set;
		}

		public override string Message
		{
			get
			{
				StringBuffer sb = new StringBuffer();



                if (Filter.AppID != null)
                {
                    string appName = null, appActionName = null;
                    AppBase app = AppManager.GetApp(Filter.AppID.Value);
                    if (app == null)
                    {
                        appName = app.AppID.ToString();
                    }

                    sb += "，并且应用名称为“" + appName + "”";

                    if (Filter.AppActionType != null)
                    {
                        if (app != null)
                        {
                            AppAction appAction = app.AppActions.GetValue(Filter.AppActionType.Value);
                            if (appAction != null)
                                appActionName = appAction.ActionName;
                            else
                                appActionName = Filter.AppActionType.Value.ToString();
                        }
                        else
                            appActionName = Filter.AppActionType.Value.ToString();

                        sb += "，并且动作类型为“" + appActionName + "”";
                    }
                }

				if (Filter.UserID != null)
					sb += "，并且作者ID等于" + Filter.UserID;

                if (Filter.Username != null && Filter.Username != string.Empty)
                {
                    sb += "，并且作者名称是" + Filter.Username;
                }

				if (Filter.BeginDate != null && Filter.EndDate != null)
				{
					sb += "，并且创建时间在" + DateTimeUtil.FormatDateTime(Filter.BeginDate.Value) + "和" + DateTimeUtil.FormatDateTime(Filter.EndDate.Value) + "之间";
				}
				else if (Filter.BeginDate != null)
				{
					sb += "，并且创建时间晚于或等于" + DateTimeUtil.FormatDateTime(Filter.BeginDate.Value);
				}
				else if (Filter.EndDate != null)
				{
					sb += "，并且创建时间早于或等于" + DateTimeUtil.FormatDateTime(Filter.EndDate.Value);
				}


				string con = sb.ToString();

				if (sb.Length > 0)
					con = con.Substring(3);

				return string.Format(
					"<a href=\"{0}\">{1}</a> 使用删除搜索结果功能，删除了所有 {2} 的主题，共计{3}条数据"
					, BbsRouter.GetUrl("space/" + OperatorID)
					, OperatorName
					, con
					, TotalDeleted
				);
			}
		}
	}
}