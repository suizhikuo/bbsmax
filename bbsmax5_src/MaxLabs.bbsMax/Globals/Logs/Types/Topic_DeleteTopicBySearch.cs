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

namespace MaxLabs.bbsMax.Logs
{
	[OperationType(Topic_DeleteTopicBySearch.TYPE_NAME)]
	public class Topic_DeleteTopicBySearch : Operation
	{
        public const string TYPE_NAME = "删除帖子";

        public Topic_DeleteTopicBySearch(int operatorID, string operatorName, string operatorIP, Filters.TopicFilter filter, int totalDeleted)
			: base(operatorID, operatorName, operatorIP)
		{
			Filter = filter;
			TotalDeleted = totalDeleted;
		}

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

		public Filters.TopicFilter Filter
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

                if (Filter.ForumID != null)
                    sb += "，并且版块ID等于" + Filter.ForumID;

                if (Filter.IncludeStick == true)
                    sb += "，并且包含置顶";

                if (Filter.IncludeValued == true)
                    sb += "，并且包含精华";

				if (Filter.TopicID != null)
					sb += "，并且主题ID等于" + Filter.TopicID;

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

				if (Filter.CreateIP != null && Filter.CreateIP != string.Empty)
					sb += "，并且作者IP是" + Filter.CreateIP;

                if (Filter.KeyWord != null && Filter.KeyWord != string.Empty)
				{
					if (Filter.SearchMode == MaxLabs.bbsMax.Enums.SearchArticleMethod.FullText)
                        sb += "，并且内容中包含关键字“" + Filter.KeyWord + "”";
					else if (Filter.SearchMode == MaxLabs.bbsMax.Enums.SearchArticleMethod.Subject)
                        sb += "，并且标题中包含关键字“" + Filter.KeyWord + "”";
					else
                        sb += "，并且标题或内容中包含关键字“" + Filter.KeyWord + "”";
				}

				if (Filter.MaxReplyCount != null && Filter.MinReplyCount != null)
				{
                    sb += "，并且回复数在" + Filter.MinReplyCount.Value + "和" + Filter.MaxReplyCount.Value + "之间"; 
				}
                else if (Filter.MinReplyCount != null)
				{
                    sb += "，并且回复数大于或等于" + Filter.MinReplyCount;
				}
                else if (Filter.MaxReplyCount != null)
				{
                    sb += "，并且回复数小于或等于" + Filter.MaxReplyCount;
				}

				if (Filter.MaxViewCount != null && Filter.MinViewCount != null)
				{
                    sb += "，并且总阅读数在" + Filter.MinViewCount + "和" + Filter.MaxViewCount + "之间";
				}
				else if (Filter.MinViewCount != null)
				{
                    sb += "，并且总阅读数大于或等于" + Filter.MinViewCount;
				}
				else if (Filter.MaxViewCount != null)
				{
                    sb += "，并且总阅读数小于或等于" + Filter.MaxViewCount;
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