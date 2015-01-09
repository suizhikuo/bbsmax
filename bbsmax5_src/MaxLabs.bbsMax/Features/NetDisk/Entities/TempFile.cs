//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MaxLabs.bbsMax.Entities
//{
//    public class TempFile : EntityBase
//    {
//        private int userID;
//        private string guid;
//        private long fileSize;
//        private string md5Code;
//        private bool isCoverFile;
//        private bool isAutoRename;

//        public string Guid
//        {
//            get { return guid; }
//            set { guid = value; }
//        }
//        public int UserID
//        {
//            get { return userID; }
//            set { userID = value; }
//        }

//        public string FileName
//        {
//            get { return Name; }
//            set { Name = value; }
//        }
//        public long FileSize
//        {
//            get { return fileSize; }
//            set { fileSize = value; }
//        }
//        public string Md5Code
//        {
//            get { return md5Code; }
//            set { md5Code = value; }
//        }
//        public bool IsCoverFile
//        {
//            get { return isCoverFile; }
//            set { isCoverFile = value; }
//        }
//        public bool IsAutoRename
//        {
//            get { return isAutoRename; }
//            set { isAutoRename = value; }
//        }
//        private string filePath;
//        /// <summary>
//        /// 文件物理路径 E:\aa\aa
//        /// </summary>
//        public string FilePath
//        {
//            get
//            {
//                if (filePath == null)
//                {
//                    if (FileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
//                    {
//                        filePath = Globals.TempPath + "\\uploading-" + guid + ".exe";
//                    }
//                    else
//                        filePath = Globals.TempPath + "\\uploading-" + guid;
//                }
//                return filePath;
//            }
//        }
//    }
//}