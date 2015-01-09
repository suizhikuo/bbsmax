//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class blog_blogarticle_delete : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
		{
			articleID = _Request.Get<int>("id", Method.All);

			if (articleID.HasValue == false)
				ShowError("缺少必要参数");

			BlogArticle article = BlogBO.Instance.GetBlogArticleForDelete(MyUserID, articleID.Value);

			if (article == null)
				ShowError("您要删除的日志不存在");

            if (_Request.IsClick("delete"))
			{
                DeleteBlogArticle();
            }
        }

		int? articleID = null;

        private void DeleteBlogArticle()
        {
            using (ErrorScope es = new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                if (BlogBO.Instance.DeleteBlogArticle(MyUserID, articleID.Value, true))
                {
                    Return("id", articleID.Value);
                }
                else
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }
        }
    }
}