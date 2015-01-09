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
    public class UsernameLengthError : ParamError<string>
    {
        private int m_MaxLength, m_MinLength;

        public UsernameLengthError(string target, string username, int maxLength, int minLength) : base(target, username)
        {
            m_MaxLength = maxLength;
            m_MinLength = minLength;
        }

        public override string Message
        {
            get
            {
                int usernameLength = StringUtil.GetByteCount(ParamValue);
                if (usernameLength < MinLength)
                    return string.Format("用户名至少需要{1}个字符（1个汉字相当于2个字符）", ParamValue, MinLength);
                else if (usernameLength > MaxLength)
                    return string.Format("用户名不能超过{1}个字符（1个汉字相当于2个字符）", ParamValue, MaxLength);
                else
                    return "管理员疏忽导致的错误，请联系管理员（用户名允许的最大长度不能小于最小长度）";
            }
        }

        public int MaxLength { get { return m_MaxLength; } }

        public int MinLength { get { return m_MinLength; } }
    }
}