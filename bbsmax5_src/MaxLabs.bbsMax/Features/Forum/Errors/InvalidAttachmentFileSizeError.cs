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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Errors
{
    public class InvalidAttachmentFileSizeError : ErrorInfo
    {
        public InvalidAttachmentFileSizeError(string target, string fileName, long maxFileSize, long currentFileSize)
            : base(target)
        {
            MaxFileSize = maxFileSize;
            CurrentFileSize = currentFileSize;
            FileName = fileName;
        }

        public long MaxFileSize { get; private set; }
        public long CurrentFileSize { get; private set; }
        public string FileName { get; private set; }
        public override string Message
        {
            get
            {
                return string.Format("您上传的附件{0}的大小超过了系统允许的最大单个文件大小{1}", FileName, ConvertUtil.FormatSize(MaxFileSize));
            }
        }

        public override bool HtmlEncodeMessage
        {
            get { return true; }
        }
    }
}