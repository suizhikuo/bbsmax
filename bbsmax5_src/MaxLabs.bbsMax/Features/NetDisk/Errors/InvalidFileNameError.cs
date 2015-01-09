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

namespace MaxLabs.bbsMax.NetDisk
{
    /// <summary>
    /// 文件名格式错误
    /// </summary>
    public class InvalidFileNameError : Errors.ParamError<string>
    {
        public InvalidFileNameError(string target, string filename)
            : base(target, filename)
        { }

        public override string Message
        {
            get { return "文件名不能包含下列任何字符 \" | / \\ < > * ? "; }
        }

        /// <summary>
        /// 不符合要求的原始文件名
        /// </summary>
        public string FileName
        {
            get { return this.ParamValue; }
        }
    }
}