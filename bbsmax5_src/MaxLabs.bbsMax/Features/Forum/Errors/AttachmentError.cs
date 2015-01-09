//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Errors
{
    public abstract class CreateAttachmentError : ErrorInfo
    {
        public CreateAttachmentError(int forumID)
        {
            m_ForumID = forumID;
        }

        private int m_ForumID;
        private Forum m_Forum;
        public Forum CurrentForum
        {
            get
            {
                if (m_Forum == null)
                {
                    m_Forum = ForumBO.Instance.GetForum(m_ForumID);
                }
                return m_Forum;
            }
        }
    }

    public class NoPermissionCreateAttachmentError : CreateAttachmentError
    {
        public NoPermissionCreateAttachmentError(int forumID)
            : base(forumID)
        {
        }

        public override string Message
        {
            get
            {
                return string.Format(Lang_Error.Permission_NoPermissionCreateAttachmentError, CurrentForum.ForumNameText);
            }
        }

    }

    public class NotAllowAttachmentFileTypeError : CreateAttachmentError
    {
        public NotAllowAttachmentFileTypeError(int forumID, string notAllowExtNames)
            :base(forumID)
        {
            NotAllowExtNames = notAllowExtNames;
        }

        public string NotAllowExtNames
        {
            get;
            private set;
        }
        public override string Message
        {
            get { return string.Format(Lang_Error.Attachment_NotAllowFileTypeError, CurrentForum.ForumNameText, NotAllowExtNames); }
        }
    }


    //public class InvalidAttachmentError : ErrorInfo
    //{
    //    public InvalidAttachmentError()
    //    {
    //    }

    //    public override string Message
    //    {
    //        get
    //        {
    //            return Lang_Error.Attachment_InvalidAttachmentError;
    //        }
    //    }

    //}

    //public class OverTodayAlowAttachmentCountError : ErrorInfo
    //{
    //    public OverTodayAlowAttachmentCountError(int maxAttachmentCount)
    //    {
    //        MaxAttachmentCount = maxAttachmentCount;
    //    }

    //    public int MaxAttachmentCount { get; private set; }

    //    public override string Message
    //    {
    //        get
    //        {
    //            return string.Format(Lang_Error.Attachment_OverTodayAlowAttachmentCount, MaxAttachmentCount);
    //        }
    //    }

    //}
    //public class OverTodayAlowAttachmentFileSizeError : ErrorInfo
    //{
    //    public OverTodayAlowAttachmentFileSizeError(long todayUploadedSize,long residualSize,long currentFileSize)
    //    {
    //        TodayUploadedSize = todayUploadedSize;
    //        ResidualSize = residualSize;
    //        CurrentFileSize = currentFileSize;
    //    }

    //    public long TodayUploadedSize { get; private set; }
    //    public long ResidualSize { get; private set; }
    //    public long CurrentFileSize { get; private set; }

    //    public override string Message
    //    {
    //        get
    //        {
    //            return string.Format(Lang_Error.Attachment_OverTodayAlowAttachmentFileSize, ConvertUtil.FormatSize(TodayUploadedSize), ConvertUtil.FormatSize(ResidualSize), ConvertUtil.FormatSize(CurrentFileSize));
    //        }
    //    }

    //}

    public class OverAttachmentMaxSingleFileSizeError : CreateAttachmentError
    {
        public OverAttachmentMaxSingleFileSizeError(int forumID, long maxFileSize, string fileName, long fileSize)
            : base(forumID)
        {
            MaxSignleFileSize = maxFileSize;
            FileSize = fileSize;
            FileName = fileName;
        }

        public long MaxSignleFileSize { get; private set; }
        public long FileSize { get; private set; }
        public string FileName { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format(Lang_Error.Attachment_OverAttachmentMaxSingleFileSize, CurrentForum.ForumName, ConvertUtil.FormatSize(MaxSignleFileSize), FileName, ConvertUtil.FormatSize(FileSize));
            }
        }

    }



    public class OverMaxTopicAttachmentCountError : CreateAttachmentError
    {
        public OverMaxTopicAttachmentCountError(int forumID, long maxAttachmentCount)
            : base(forumID)
        {
            MaxAttachmentCount = maxAttachmentCount;
        }

        public long MaxAttachmentCount { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format(Lang_Error.Attachment_OverMaxTopicAttachmentCount, CurrentForum.ForumNameText, MaxAttachmentCount);
            }
        }

    }


    public class OverMaxPostAttachmentCountError : CreateAttachmentError
    {
        public OverMaxPostAttachmentCountError(int forumID, long maxAttachmentCount)
            : base(forumID)
        {
            MaxAttachmentCount = maxAttachmentCount;
        }

        public long MaxAttachmentCount { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format(Lang_Error.Attachment_OverMaxPostAttachmentCount, CurrentForum.ForumNameText, MaxAttachmentCount);
            }
        }

    }
}