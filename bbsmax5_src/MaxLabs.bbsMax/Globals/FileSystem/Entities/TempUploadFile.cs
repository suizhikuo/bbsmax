//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;
using System.IO;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.FileSystem
{
    /// <summary>
    /// 临时文件
    /// </summary>
    public class TempUploadFile : IPrimaryKey<int>// : ITempUploadFile
    {
        private StringList m_CustomParams = null;

        public TempUploadFile(DataReaderWrap readerWrap)
        {
            TempUploadFileID = readerWrap.Get<int>("TempUploadFileID");
            UserID = readerWrap.Get<int>("UserID");
            FileName = readerWrap.Get<string>("FileName");
            ServerFileName = readerWrap.Get<string>("ServerFileName");
            MD5 = readerWrap.Get<string>("MD5");
            FileSize = readerWrap.Get<long>("FileSize");
            
            m_CustomParams = StringList.Parse(readerWrap.Get<string>("CustomParams"));
        }

        public TempUploadFile(string filename, string tempFilePath, long contentLength, string md5)
        {
            FileName = filename;
            ServerFileName = tempFilePath;
            FileSize = contentLength;
            MD5 = md5;

            m_CustomParams = new StringList();
        }

		[Obsolete("For Template Only!")]
		public int ID { get { return TempUploadFileID; } }

        public int TempUploadFileID { get; set; }

        /// <summary>
        /// 临时文件所属的用户
        /// </summary>
        public int UserID { get; private set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 临时文件相对路径
        /// </summary>
        public string ServerFileName { get; private set; }

        /// <summary>
        /// 可传递的自定义的参数
        /// </summary>
        //public StringTable CustomParams { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; private set; }

        /// <summary>
        /// 文件的MD5
        /// </summary>
        public string MD5 { get; private set; }

        //public string FileID { get; private set; }

        /// <summary>
        /// 文件绝对路径 e:\xx\xx.rar
        /// </summary>
        public string PhysicalFilePath
        {
            get { return Globals.GetPath(SystemDirecotry.Temp_Upload, ServerFileName); }
        }

        public string this[int index]
        {
            get { return m_CustomParams[index]; }
        }

        /// <summary>
        /// 获取这个临时文件保存为正式文件之后的文件ID（必须在文件上传完成后才能得到正确的值）
        /// </summary>
        public string FileID
        {
            get { return MD5 + FileSize; }
        }

        /// <summary>
        /// 判断这个文件是否图片
        /// </summary>
        public bool IsImage { get; private set; }

        public void BuildThumbnail()
        {
            
        }

        public PhysicalFileFromTemp Save()
        {
            PhysicalFileFromTemp file = FileManager.Save(this.UserID, this);

            return file;
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.TempUploadFileID;
        }

        #endregion
    }

    /// <summary>
    /// 临时文件的集合
    /// </summary>
    public class TempUploadFileCollection : EntityCollectionBase<int, TempUploadFile>
    {
        public TempUploadFileCollection()
        { }

        public TempUploadFileCollection(DataReaderWrap readerWrap)
            : this(readerWrap, 0)
        { }

        public TempUploadFileCollection(DataReaderWrap readerWrap, int userID)
        {
            while (readerWrap.Next)
            {
                TempUploadFile tempUploadFile = new TempUploadFile(readerWrap);

                this.Add(tempUploadFile);
            }

            this.UserID = userID;
        }

        public int UserID { get; set; }

		public long GetTotalFileSize()
		{
			long result = 0;

			foreach (TempUploadFile file in this)
			{
				result += file.FileSize;
			}

			return result;
		}

        public string[] GetFileIds()
        {
            string[] result = new string[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                result[i] = this[i].FileID;
            }

            return result;
        }

        public string[] GetMD5s()
        {
            string[] result = new string[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                result[i] = this[i].MD5;
            }

            return result;
        }

        public string[] GetFileTypes()
        {
            string[] result = new string[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                result[i] = Path.GetExtension(this[i].FileName).ToLower();
            }

            return result;
        }

        public long[] GetFileSizes()
        {
            long[] result = new long[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                result[i] = this[i].FileSize;
            }

            return result;
        }

        public PhysicalFileFromTempCollection Save()
        {
            PhysicalFileFromTempCollection files = FileManager.Save(this.UserID, this);

            return files;
        }
    }

}