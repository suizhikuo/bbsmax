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
	[OperationType(Blog_DeleteBlogArticleByIDs.TYPE_NAME)]
	public class Blog_DeleteBlogArticleByIDs : Operation
	{
        public const string TYPE_NAME = "删除文章";

		public Blog_DeleteBlogArticleByIDs(int operatorID, string operatorName, string operatorIP, int[] blogArticleIDs)
			: base(operatorID, operatorName, operatorIP)
		{
			BlogArticleIDs = blogArticleIDs;
		}

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

		public int[] BlogArticleIDs
		{
			get;
			private set;
		}

		public override string Message
		{
			get
			{

				return string.Format(
					"<a href=\"{0}\">{1}</a> 使用多选删除功能，删除了ID为：{2} 的博客文章"
					, BbsRouter.GetUrl("space/" + OperatorID)
					, OperatorName
					, StringUtil.Join(BlogArticleIDs, ",")
				);
			}
		}
	}
}