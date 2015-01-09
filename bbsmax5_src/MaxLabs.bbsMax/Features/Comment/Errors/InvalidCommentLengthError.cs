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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Errors
{
    public class InvalidCommentLengthError : ParamError<string>
    {
        int m_MaxLength;
        public InvalidCommentLengthError(string target, string content, int MaxLength)
            : base(target, content)
        {
            this.m_MaxLength = MaxLength;
        }

        private int MaxLength { get { return m_MaxLength; } }

        public override string Message
        {
            get
            {
                return string.Format(Lang_Error.Comment_InvalidCommentLengthError, ParamValue, MaxLength);
            }
        }
    }
}