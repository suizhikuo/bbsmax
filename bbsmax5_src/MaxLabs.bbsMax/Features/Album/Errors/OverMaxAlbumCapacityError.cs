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
    public class OverMaxAlbumCapacityError : ErrorInfo
    {
        private long m_MaxLength;

        public OverMaxAlbumCapacityError(long maxLength)
        {
            m_MaxLength = maxLength;
        }

        public override string Message
        {
            get
            {
                return string.Format("加上当前上传的文件，您的相册容量将超过上限{0},所以不能进行上传！", StringUtil.FriendlyCapacitySize(m_MaxLength));
            }
        }

        public long MaxLength { get { return m_MaxLength; } }

    }

    public class OverMaxPhotoFileSizeError : ErrorInfo
    {
        private long m_MaxFileSize;

        public OverMaxPhotoFileSizeError(long maxFileSize)
        {
            m_MaxFileSize = maxFileSize;
        }

        public override string Message
        {
            get { return string.Format("您上传的照片大小超过了照片文件不能大于{0}的限制", StringUtil.FriendlyCapacitySize(m_MaxFileSize)); }
        }
    }
}