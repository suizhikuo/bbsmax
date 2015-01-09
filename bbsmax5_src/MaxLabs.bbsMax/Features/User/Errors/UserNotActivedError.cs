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
    public class UserNotActivedError : ErrorInfo
    {
        int m_Userid;
        string m_Email;
        string m_Code;
        string m_ActiveUrl;

        public UserNotActivedError(int userid, string email, string activeUrl)
            : base("noActive")
        {
            this.m_Userid = userid;
            this.m_Email = email;
            this.m_ActiveUrl = activeUrl;
        }

        public UserNotActivedError(string target, int userid, string email, string activeUrl)
            : base(target)
        {

            this.m_Userid = userid;
            this.m_Email = email;
            this.m_ActiveUrl = activeUrl;
        }

        public override string Message
        {
            get { return string.Format(Lang_Error.User_NotActiveError, m_Email, m_ActiveUrl); }
        }

        public string ActiveUrl
        {
            get { return m_ActiveUrl; }
        }
    }
}