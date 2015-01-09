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
    public class OverTodayAlowAttachmentFileSizeError : ErrorInfo
    {
        public OverTodayAlowAttachmentFileSizeError(string target, long totalAllowSize, long todayAllowTotalSize, long currentTotalSize)
            : base(target)
        {
            TodayAllowTotalSize = todayAllowTotalSize;
            CurrentTotalSize = currentTotalSize;
            TotalAllowSize = totalAllowSize;
        }

        public long TotalAllowSize { get; private set; }
        public long TodayAllowTotalSize { get; private set; }
        public long CurrentTotalSize { get; private set; }
        public override string Message
        {
            get
            {
                return string.Format("超过了您今天允许上传的总附件大小{1}，您今天还允许上传{0}的附件", ConvertUtil.FormatSize(TodayAllowTotalSize), ConvertUtil.FormatSize(TotalAllowSize));
            }
        }
    }
}