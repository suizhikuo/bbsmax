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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.FileSystem
{
    /// <summary>
    /// 真实文件实体类,与数据库数据一一对应
    /// </summary>
    public class PhysicalFile : IPrimaryKey<string>
    {

        public PhysicalFile() { }

        /// <summary>
        /// 实体化一个物理文件类,并将一个reader传入转换处理赋值给它的各个属性
        /// </summary>
        /// <param name="reader"></param>
        public PhysicalFile(DataReaderWrap readerWrap)
        {
            this.FileID = readerWrap.Get<string>("FileID");
            this.ServerFilePath = readerWrap.Get<string>("ServerFilePath");
            this.MD5 = readerWrap.Get<string>("MD5");
            this.FileSize = readerWrap.Get<long>("FileSize");
        }


        #region Properties


        /// <summary>
        /// 物理文件唯一ID
        /// </summary>
        public string FileID { get; private set; }

        [Obsolete]
        public string ID { get { return FileID; } }

        /// <summary>
        /// 物理文件保存在服务器上的真实相对路径
        /// </summary>
        public string ServerFilePath { get; private set; }

        /// <summary>
        /// 物理文件的MD5值
        /// </summary>
        public string MD5 { get; private set; }

        /// <summary>
        /// 物理文件的大小
        /// </summary>
        public long FileSize { get; private set; }

        public string PhysicalFilePath { get { return IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Upload_File), ServerFilePath); } }


        #endregion

        #region IPrimaryKey<int> 成员

        public string GetKey()
        {
            return FileID;
        }

        #endregion
    }

    /// <summary>
    /// FileEntity的集合
    /// </summary>
    public class PhysicalFileCollection : EntityCollectionBase<string, PhysicalFile>
    {

        #region Constructors

        public PhysicalFileCollection() { }

        public PhysicalFileCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new PhysicalFile(readerWrap));
            }
        }

        #endregion

    }

    public class PhysicalFileFromTemp : PhysicalFile
    {
        public PhysicalFileFromTemp(DataReaderWrap readerWrap)
            : base(readerWrap)
        {

            TempUploadFileID = readerWrap.Get<int>("TempUploadFileID");
            TempUploadFileName = readerWrap.Get<string>("TempUploadFileName");
            TempUploadServerFileName = readerWrap.Get<string>("TempUploadServerFileName");

        }

        public int TempUploadFileID { get; set; }

        public string TempUploadFileName { get; set; }

        public string TempUploadServerFileName { get; set; }
    }

    public class PhysicalFileFromTempCollection : EntityCollectionBase<string, PhysicalFileFromTemp>
    {
        public PhysicalFileFromTempCollection()
        { }

        public PhysicalFileFromTempCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                this.Add(new PhysicalFileFromTemp(readerWrap));
            }

        }

        public string[] GetFileIDs()
        {
            string[] result = new string[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                result[i] = this[i].FileID;
            }

            return result;
        }

        public string[] GetServerFileNames()
        {
            string[] result = new string[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                result[i] = this[i].TempUploadServerFileName;
            }

            return result;
        }

        public string[] GetMD5Codes()
        {
            string[] result = new string[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                result[i] = this[i].MD5;
            }

            return result;
        }
    }

}