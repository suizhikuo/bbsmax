//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Errors
{
    public class SearchInvalidKeywordError : ErrorInfo
    {
        public SearchInvalidKeywordError(string keyword)
        {
            Keyword = keyword;
        }

        public string Keyword { private set; get; }

        public override string Message
        {
            get { return "关键字不能为空或者只有特殊字符！"; }
        }
    }
}