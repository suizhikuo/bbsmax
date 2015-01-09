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
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.RegExp
{
    /// <summary>
    /// 邮箱验证码匹配正则
    /// </summary>
    public class EmailValidateCodeRegex:Regex
    {
        private const string EmailValidateCodePattern = @"^(\d+)\D(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2})@([a-z0-9A-Z]+[\w\-\.]*@\w+(?>\.\w+){1,4})@(.+)$";
        public EmailValidateCodeRegex()
            : base(EmailValidateCodePattern, RegexOptions.Compiled)
        {

        }
    }
}