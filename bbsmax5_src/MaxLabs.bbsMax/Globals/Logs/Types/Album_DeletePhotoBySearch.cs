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
	[OperationType(Album_DeletePhotoBySearch.TYPE_NAME)]
	public class Album_DeletePhotoBySearch : Operation
	{
        public const string TYPE_NAME = "删除相片";

		public Album_DeletePhotoBySearch(int operatorID, string operatorName, string operatorIP, Filters.AdminPhotoFilter filter, int totalDeleted)
			: base(operatorID, operatorName, operatorIP)
		{
			Filter = filter;
			TotalDeleted = totalDeleted;
		}

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

		public Filters.AdminPhotoFilter Filter
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

				if (Filter.AlbumID != null)
					sb += "，并且相册ID等于" + Filter.AlbumID;

				if (Filter.PhotoID != null)
					sb += "，并且相片ID等于" + Filter.PhotoID;

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

				if (Filter.SearchKey != null && Filter.SearchKey != string.Empty)
				{
					sb += "，并且名称中包含关键字“" + Filter.SearchKey + "”";
				}

				if (Filter.Username != null && Filter.Username != string.Empty)
				{
					sb += "，并且作者名称是" + Filter.Username;
				}

				string con = sb.ToString();

				if (sb.Length > 0)
					con = con.Substring(3);

				return string.Format(
					"<a href=\"{0}\">{1}</a> 使用删除搜索结果功能，删除了所有 {2} 的相片，共计{3}条数据"
					, BbsRouter.GetUrl("space/" + OperatorID)
					, OperatorName
					, con
					, TotalDeleted
				);
			}
		}
	}
}