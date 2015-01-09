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
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Errors
{
    public class EmailNotValidatedError:ErrorInfo
    {
        string m_Email;
        string m_ValidateUrl;
        public EmailNotValidatedError(string target,string email,string validateUrl):base(target) 
        {
            this.m_Email = email;
            this.m_ValidateUrl = validateUrl;
        }
        public EmailNotValidatedError(string email, string validateUrl)
            : base("EmailNoValidated")
        {
            this.m_Email = email;
            this.m_ValidateUrl = validateUrl;
        }

        public override string Message
        {
            get { return string.Format(Lang_Error.User_EmailNotValidatedError, m_Email, m_ValidateUrl); }
        }

        public string ValidateUrl
        {
            get { return m_ValidateUrl; }
        }
    }
}