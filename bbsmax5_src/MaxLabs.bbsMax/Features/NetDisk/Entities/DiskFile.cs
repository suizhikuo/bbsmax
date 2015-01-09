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
using System.IO;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Enums;



namespace MaxLabs.bbsMax.Entities
{
    public class DiskFile :IFile,IPrimaryKey<int>,IFillSimpleUser
    {
        private int userID;
        private int directoryID;
        private long contentLength;
        private string contentType;
        private DateTime createDate;
        private DateTime updateDate;
        private FileProperty exPro = new FileProperty();
        private string exProShowString;

        public DiskFile() { }
        public DiskFile( DataReaderWrap readerWrap )
        {
            this.FileName = readerWrap.Get<string>("FileName");
            this.DiskFileID = readerWrap.Get<int>("DiskFileID");
            this.DirectoryID = readerWrap.Get<int>("DirectoryID");
            this.Size = readerWrap.Get<long>("FileSize");
            this.FileID = readerWrap.Get<string>("FileID");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.UserID = readerWrap.Get<int>("UserID");
            this.UpdateDate = readerWrap.Get<DateTime>("UpdateDate");
            //this.ThumbPath = readerWrap.Get<string>("ThumbPath");
        }

        //public string ThumbPath { 
        //    get; 
        //    set; 
        //}

        public bool IsFile { get { return true; } }

        public int DiskFileID { get; set; }

        [Obsolete]
        public int ID { get { return DiskFileID; } }

        public string TypeName { get { return this.ExtensionName; } }

        public string FileName { get; set; }

        public string Name
        {
            get { return this.FileName; }
            set { this.FileName = value; }
        }

        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        public int DirectoryID
        {
            get { return directoryID; }
            set { directoryID = value; }
        }

        public long Size
        {
            get { return contentLength; }
            set { contentLength = value; }
        }

        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        public DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }

        public DateTime UpdateDate
        {
            get { return updateDate; }
            set { updateDate = value; }
        }

        public FileProperty ExPro
        {
            get { return this.exPro; }
            set { exPro = value; }
        }

        public string ExProShowString
        {
            get { return this.exProShowString; }
            set { exProShowString = value; }
        }

        private string m_ExtensionName;
        public string ExtensionName
        {
            get
            {
                if (m_ExtensionName == null)
                {
                    m_ExtensionName=  Path.GetExtension(this.FileName);
                }
                return m_ExtensionName;
            }
        }

        private string fileID = string.Empty;
        public string FileID
        {
            get
            {
                return this.fileID;
            }
            set
            {
                this.fileID = value;
            }
        }

        private string icon = string.Empty;

        private string m_smallIcon;
        public string SmallIcon
        {
            get
            {
                if (string.IsNullOrEmpty(m_smallIcon))
                {
                    m_smallIcon = FileManager.GetExtensionsIcon(this.FileName, FileIconSize.SizeOf16);
                }
                return m_smallIcon;
            }
        }

        public string Icon
        {
            get
            {

                if (string.IsNullOrEmpty(icon))
                {
                   icon = FileManager.GetExtensionsIcon(this.FileName,FileIconSize.SizeOf32);
                }

                return icon;
            }
            set { icon = value; }
        }

        private string m_bigIcon;
        public virtual string BigIcon
        {
            get
            {
                if (string.IsNullOrEmpty(m_bigIcon))
                {
                    //if (!string.IsNullOrEmpty(this.ThumbPath))
                    //{
                    //    m_bigIcon = UrlUtil.ResolveUrl(ThumbPath);
                    //}
                    //else
                    //{
                        m_bigIcon = FileManager.GetExtensionsIcon(this.FileName, FileIconSize.SizeOf48);
                    //}
                }
                return m_bigIcon;
            }
        }

        //private string thumbnail = string.Empty;
        //public string Thumbnail
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(thumbnail))
        //        {
        //            return Icon;
        //        }
        //        return thumbnail;
        //    }
        //    set { thumbnail = value; }
        //}

        private string iconImage = string.Empty;

        public virtual string IconImage
        {
            get
            {
                if (string.IsNullOrEmpty(iconImage))
                {
                    string fileName = Path.GetExtension(this.FileName);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        fileName = fileName.ToLower();
                        if (fileName == ".exe")
                        {
                            if (System.Web.HttpContext.Current.Request.Browser != null && System.Web.HttpContext.Current.Request.Browser.Browser == "IE")
                            {
                                if (System.Web.HttpContext.Current.Request.Browser.MajorVersion < 7)
                                {
                                    iconImage = "<b style=\"display:inline-block;height:16px;width:16px;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + Icon + "')\"></b>";
                                }
                            }
                        }

                        iconImage = "<img src=\"" + Icon + "\" alt=\"\" />";
                    }
                }
                return iconImage;
            }
            set { iconImage = value; }
        }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(UserID);
            }
        }

#region IPrimaryKey<int> ��Ա

public int  GetKey()
{
 	return this.DiskFileID;
}

#endregion

#region IFillSimpleUser ��Ա

public int GetUserIDForFill()
{
    return UserID;
}

#endregion
    }
    public class DiskFileCollection : EntityCollectionBase<int, DiskFile>
    {
        public DiskFileCollection() { }
        public DiskFileCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
                Add(new DiskFile(readerWrap));
        }
    }
}