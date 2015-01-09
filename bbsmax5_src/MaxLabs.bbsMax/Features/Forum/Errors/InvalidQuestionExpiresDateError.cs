//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Text;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Errors
{
    public class InvalidQuestionExpiresDateError : ErrorInfo
    {
        public InvalidQuestionExpiresDateError(string target, string times, TimeSpan expiresTime)
            : base(target) 
        {
            Times = times;
            ExpiresTime = expiresTime;
        }

        public string Times { get; private set; }
        public TimeSpan ExpiresTime { get; private set; }
        public override string Message
        {
            get
            {
                return string.Format("有效天数不能超过{0}", Times);
            }
        }
    }
}