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
using MaxLabs.WebEngine;

using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Errors
{
    public class MessageContentLengthError : ParamError<string>
    {
        private int m_MaxLength;

        public MessageContentLengthError(string target, string value, int maxLength)
            : base(target, value)
        {
            m_MaxLength = maxLength;
        }

        public override string Message
        {
            get
            {
                int length = StringUtil.GetByteCount(ParamValue);

                return string.Format("消息内容最大允许{2}个字符（1个汉字相当于2个字符）", ParamValue, length, m_MaxLength);
            }
        }

        public int MaxLength { get { return m_MaxLength; } }

    }
}