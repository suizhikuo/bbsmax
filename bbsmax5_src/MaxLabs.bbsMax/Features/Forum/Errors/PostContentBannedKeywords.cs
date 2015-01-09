//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Errors
{
    public class PostContentBannedKeywords : ErrorInfo
    {
        public PostContentBannedKeywords(string target, string keyword)
            : base(target)
        {
            Keyword = keyword;
        }

        public string Keyword
        {
            get;
            private set;
        }

        private string m_Message;
        public override string Message
        {
            get
            {
                if (m_Message == null)
                {
                    if (IsShowKeywordContent)
                        m_Message = "帖子内容包含了被禁止的关键字：" + Keyword;
                    else
                        m_Message = "帖子内容包含了被禁止的关键字。";
                }
                return m_Message;
            }
        }

        public override bool HtmlEncodeMessage
        {
            get { return IsShowKeywordContent; }
        }

        private bool IsShowKeywordContent
        {
            get { return AllSettings.Current.ContentKeywordSettings.IsShowKeywordContent; }
        }
    }
}