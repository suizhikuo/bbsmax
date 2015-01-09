//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.DataAccess;
using System.Collections;
using System.Text;
using MaxLabs.bbsMax.FileSystem;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 帖子附件的信息
    /// </summary>
    public class Attachment : IPrimaryKey<int>
    {
        public Attachment()
        {
        }

        public Attachment(DataReaderWrap readerWrap)
        {
            this.AttachmentID = readerWrap.Get<int>("AttachmentID");
            this.PostID = readerWrap.Get<int>("PostID");
            this.FileID = readerWrap.Get<string>("FileID");
            this.FileName = readerWrap.Get<string>("FileName");
            this.FileType = readerWrap.Get<string>("FileType");
            this.FileSize = readerWrap.Get<long>("FileSize");
            this.TotalDownloads = readerWrap.Get<int>("TotalDownloads");
            this.TotalDownloadUsers = readerWrap.Get<int>("TotalDownloadUsers");
            this.Price = readerWrap.Get<int>("Price");
            this.FileExtendedInfo = readerWrap.Get<string>("FileExtendedInfo");
            this.UserID = readerWrap.Get<int>("UserID");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
        }

        public int AttachmentID { get; set; }

        public int PostID { get; set; }

        public string FileID { get; set; }

        public string FileName { get; set; }


        /// <summary>
        /// 文件类型 如"jpg"  不带点
        /// </summary>
        public string FileType { get; set; }

        public long FileSize { get; set; }

        public int TotalDownloads { get; set; }

        public int TotalDownloadUsers { get; set; }

        public int Price { get; set; }

        public string FileExtendedInfo { get; set; }

        public int UserID { get; set; }

        public DateTime CreateDate { get; set; }



        private string m_fileIcon;
        public string FileIcon
        {
            get
            {

                if (m_fileIcon == null)
                {
                    m_fileIcon = FileManager.GetExtensionsIcon(this.FileName, FileIconSize.SizeOf16);
                }
                return m_fileIcon;
            }
        }

        public string FileSizeFormat
        {
            get
            {
                return ConvertUtil.FormatSize(FileSize);
            }
        }


        public AttachType AttachType{ get; set; }

        /// <summary>
        /// 是否在内容中出现过
        /// </summary>
        public bool IsInContent { get; set; }

        /// <summary>
        /// 仅在 附件是从网络硬盘插入时 使用
        /// </summary>
        public int DiskFileID { get; set; }

        /// <summary>
        /// 是否到了免费期 如果是出售的附件 超出了出售期限 就自动变为免费
        /// </summary>
        /// <param name="forumSetting"></param>
        /// <returns></returns>
        public bool IsOverSellDays(ForumSettingItem forumSetting)
        {
            if (forumSetting.SellAttachmentDays == 0)
                return false;
            return CreateDate.AddSeconds(forumSetting.SellAttachmentDays) <= DateTimeUtil.Now;
        }

        /// <summary>
        /// 附件是否买过
        /// </summary>
        public bool IsBuyed(AuthUser operatorUser)
        {
            return PostBOV5.Instance.IsBuyedAttachment(operatorUser, this);
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return AttachmentID;
        }

        #endregion
    }

    public class AttachmentCollection : EntityCollectionBase<int, Attachment>
    {
        public AttachmentCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                Attachment attachment = new Attachment(readerWrap);

                this.Add(attachment);
            }
        }
        public AttachmentCollection() : base()
        { }

        public AttachmentCollection(IEnumerable<Attachment> list)
            : base(list)
        {
            
        }

        public string BuildIds()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Attachment attachment in this)
            {
                builder.Append("|");
                builder.Append(attachment.AttachmentID);
            }

            if (builder.Length > 0)
                builder.Remove(0, 1);

            return builder.ToString();
        }

        public string BuildFileNames()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Attachment attachment in this)
            {
                builder.Append("|");
                builder.Append(attachment.FileName);
            }

            if (builder.Length > 0)
                builder.Remove(0, 1);

            return builder.ToString();
        }

        public string BuildFileExtNames()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Attachment attachment in this)
            {
                builder.Append("|");
                builder.Append(attachment.FileType);
            }

            if (builder.Length > 0)
                builder.Remove(0, 1);

            return builder.ToString();
        }

        public string BuildFileIds()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Attachment attachment in this)
            {
                builder.Append("|");
                builder.Append(attachment.FileID);
            }

            if (builder.Length > 0)
                builder.Remove(0, 1);

            return builder.ToString();
        }

        public string BuildFileSizes()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Attachment attachment in this)
            {
                builder.Append("|");
                builder.Append(attachment.FileSize);
            }

            if (builder.Length > 0)
                builder.Remove(0, 1);

            return builder.ToString();
        }

        public string BuildPrices()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Attachment attachment in this)
            {
                builder.Append("|");
                builder.Append(attachment.Price);
            }

            if (builder.Length > 0)
                builder.Remove(0, 1);

            return builder.ToString();
        }
    }
}