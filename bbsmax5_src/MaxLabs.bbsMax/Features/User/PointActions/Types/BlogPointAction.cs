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

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.PointActions
{
    public class BlogPointAction : PointActionBase<BlogPointAction, BlogPointType, NullEnum>
    {
        public override string Name
        {
            get { return Lang.BlogPointTypeName; }
        }

        public override bool Enable
        {
            get
            {
                return AllSettings.Current.BlogSettings.EnableBlogFunction;
            }
        }
    }

    public enum BlogPointType
    {
		[PointActionItem(Lang.BlogPointType_PostArticle, false, true)]
        PostArticle,

		[PointActionItem(Lang.BlogPointType_ArticleWasDeletedBySelf, false, false)]
        ArticleWasDeletedBySelf,

		[PointActionItem(Lang.BlogPointType_ArticleWasDeletedByAdmin, false, false)]
        ArticleWasDeletedByAdmin,

		[PointActionItem(Lang.BlogPointType_ArticleWasCommented, false, false)]
        ArticleWasCommented
    }
}