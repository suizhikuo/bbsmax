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
    public class InvalidPostSubjectLengthError : ErrorInfo
    {
        public InvalidPostSubjectLengthError(string target, int maxLength, int minLength, int currentLength)
            : base(target) 
        {
            CurrentLength = currentLength;
            MaxLength = maxLength;
            MinLength = minLength;
        }

        public int MaxLength { get; private set; }
        public int MinLength { get; private set; }
        public int CurrentLength { get; private set; }
        public override string Message
        {
            get
            {
                if (CurrentLength > MaxLength)
                    return string.Format("标题长度不能超过{0}个字符(1个汉字等于两个字符)", MaxLength);
                else
                    return string.Format("标题长度不能小于{0}个字符(1个汉字等于两个字符)", MinLength);
            }
        }
    }
}