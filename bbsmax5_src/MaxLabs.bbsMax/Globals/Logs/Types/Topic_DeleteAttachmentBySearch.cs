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
	[OperationType(Topic_DeleteAttachmentBySearch.TYPE_NAME)]
	public class Topic_DeleteAttachmentBySearch : Operation
	{
        public const string TYPE_NAME = "删除附件";

        public Topic_DeleteAttachmentBySearch(int operatorID, string operatorName, string operatorIP, Filters.AttachmentFilter filter, int totalDeleted)
			: base(operatorID, operatorName, operatorIP)
		{
			Filter = filter;
			TotalDeleted = totalDeleted;
		}

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

		public Filters.AttachmentFilter Filter
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


				if (Filter.UserID != null)
					sb += "，并且所有者ID等于" + Filter.UserID;

                if (Filter.Username != null && Filter.Username != string.Empty)
                {
                    sb += "，并且所有用户名是" + Filter.Username;
                }

                if (Filter.FileType == null && Filter.FileType != string.Empty)
                    sb += "，并且文件类型为“"+Filter.FileType+"”";



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

                if (Filter.MinFileSize != null && Filter.MaxFileSize != null)
				{
                    sb += "，并且文件大小在" + ConvertUtil.GetFileSize(Filter.MinFileSize.Value, Filter.MinFileSizeUnit) + "和" + ConvertUtil.GetFileSize(Filter.MaxFileSize.Value, Filter.MaxFileSizeUnit) + "之间";
				}
                else if (Filter.MinFileSize != null)
				{
                    sb += "，并且文件大于或等于" + ConvertUtil.GetFileSize(Filter.MinFileSize.Value, Filter.MinFileSizeUnit);
				}
                else if (Filter.MaxFileSize != null)
				{
                    sb += "，并且并且文件小于或等于" + ConvertUtil.GetFileSize(Filter.MaxFileSize.Value, Filter.MaxFileSizeUnit);
				}

                if (Filter.MaxPrice != null && Filter.MinPrice != null)
                {
                    sb += "，并且价格在" + Filter.MinPrice + "和" + Filter.MaxPrice + "之间";
                }
                else if (Filter.MinPrice != null)
                {
                    sb += "，并且价格大于或等于" + Filter.MinPrice;
                }
                else if (Filter.MaxPrice != null)
                {
                    sb += "，并且价格小于或等于" + Filter.MaxPrice;
                }


                if (Filter.MinTotalDownload != null && Filter.MaxTotalDownload != null)
                {
                    sb += "，并且下载次数在" + Filter.MinTotalDownload + "和" + Filter.MaxTotalDownload + "之间";
                }
                else if (Filter.MinTotalDownload != null)
                {
                    sb += "，并且下载次数大于或等于" + Filter.MinTotalDownload;
                }
                else if (Filter.MaxTotalDownload != null)
                {
                    sb += "，并且下载次数小于或等于" + Filter.MaxTotalDownload;
                }


                if (Filter.KeyWord != null && Filter.KeyWord != string.Empty)
                {
                    sb += "，并且附件名中包含关键字“" + Filter.KeyWord + "”";
                }

				

				string con = sb.ToString();

				if (sb.Length > 0)
					con = con.Substring(3);

				return string.Format(
					"<a href=\"{0}\">{1}</a> 使用删除搜索结果功能，删除了所有 {2} 的附件，共计{3}条数据"
					, BbsRouter.GetUrl("space/" + OperatorID)
					, OperatorName
					, con
					, TotalDeleted
				);
			}
		}
	}
}