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
	[OperationType(Blog_DeleteBlogArticleBySearch.TYPE_NAME)]
	public class Blog_DeleteBlogArticleBySearch : Operation
	{
        public const string TYPE_NAME = "删除文章";

		public Blog_DeleteBlogArticleBySearch(int operatorID, string operatorName, string operatorIP, Filters.AdminBlogArticleFilter filter, int totalDeleted)
			: base(operatorID, operatorName, operatorIP)
		{
			Filter = filter;
			TotalDeleted = totalDeleted;
		}

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

		public Filters.AdminBlogArticleFilter Filter
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

				if (Filter.ArticleID != null)
					sb += "，并且ID等于" + Filter.ArticleID;

				if (Filter.AuthorID != null)
					sb += "，并且作者ID等于" + Filter.AuthorID;

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

				if(Filter.PrivacyType != null)
				{
					string privacyType = null;

					switch(Filter.PrivacyType.Value)
					{
						case MaxLabs.bbsMax.Enums.PrivacyType.AllVisible:
							privacyType = "“所有人都可见”";
							break;

						case MaxLabs.bbsMax.Enums.PrivacyType.AppointUser:
							privacyType = "“仅指定用户可见”";
							break;

						case MaxLabs.bbsMax.Enums.PrivacyType.FriendVisible:
							privacyType = "“仅好友可见”";
							break;

						case MaxLabs.bbsMax.Enums.PrivacyType.NeedPassword:
							privacyType = "“需密码访问”";
							break;

						case MaxLabs.bbsMax.Enums.PrivacyType.SelfVisible:
							privacyType = "“仅作者可见”";
							break;
					}

					sb += "，并且隐私类型是" + privacyType;
				}

				if (Filter.SearchKey != null && Filter.SearchKey != string.Empty)
				{
					if (Filter.SearchMode == MaxLabs.bbsMax.Enums.SearchArticleMethod.FullText)
						sb += "，并且内容中包含关键字“" + Filter.SearchKey + "”";
					else if (Filter.SearchMode == MaxLabs.bbsMax.Enums.SearchArticleMethod.Subject)
						sb += "，并且标题中包含关键字“" + Filter.SearchKey + "”";
					else
						sb += "，并且标题或内容中包含关键字“" + Filter.SearchKey + "”";
				}

				if (Filter.TotalCommentsScopeBegin != null && Filter.TotalCommentsScopeEnd != null)
				{
					sb += "，并且评论数在" + Filter.TotalCommentsScopeBegin + "和" + Filter.TotalCommentsScopeEnd + "之间"; 
				}
				else if (Filter.TotalCommentsScopeBegin != null)
				{
					sb += "，并且评论数大于或等于" + Filter.TotalCommentsScopeBegin;
				}
				else if (Filter.TotalCommentsScopeEnd != null)
				{
					sb += "，并且评论数小于或等于" + Filter.TotalCommentsScopeEnd;
				}

				if (Filter.TotaLViewsScopeBegin != null && Filter.TotalViewsScopeEnd != null)
				{
					sb += "，并且总阅读数在" + Filter.TotaLViewsScopeBegin + "和" + Filter.TotalViewsScopeEnd + "之间";
				}
				else if (Filter.TotaLViewsScopeBegin != null)
				{
					sb += "，并且总阅读数大于或等于" + Filter.TotaLViewsScopeBegin;
				}
				else if (Filter.TotalViewsScopeEnd != null)
				{
					sb += "，并且总阅读数小于或等于" + Filter.TotalViewsScopeEnd;
				}

				if (Filter.Username != null && Filter.Username != string.Empty)
				{
					sb += "，并且作者名称是" + Filter.Username;
				}

				string con = sb.ToString();

				if (sb.Length > 0)
					con = con.Substring(3);

				return string.Format(
					"<a href=\"{0}\">{1}</a> 使用删除搜索结果功能，删除了所有 {2} 的博客文章，共计{3}条数据"
					, BbsRouter.GetUrl("space/" + OperatorID)
					, OperatorName
					, con
					, TotalDeleted
				);
			}
		}
	}
}