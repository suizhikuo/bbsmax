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
using MaxLabs.bbsMax.DataAccess;




namespace MaxLabs.bbsMax.Entities
{
    public class DiskDirectory : EntityBase, IFile, IPrimaryKey<int>
    {
        public DiskDirectory()
        {

        }

        public DiskDirectory(int directoryID, int parentID, string name)
        {
            this.DirectoryID = directoryID;
            this.ParentID = parentID;
            this.Name = name;
        }

        public DiskDirectory(DataReaderWrap readerWrap)
        {
            this.DirectoryID = readerWrap.Get<int>("DirectoryID");
            this.Name = readerWrap.Get<string>("Name");
            this.UserID = readerWrap.Get<int>("UserID");
            this.ParentID = readerWrap.Get<int>("ParentID");
            this.TotalSize = readerWrap.Get<long>("TotalSize");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.UpdateDate = readerWrap.Get<DateTime>("UpdateDate");
        }

        /// <summary>
        /// Ŀ¼ID
        /// </summary>
        public int DirectoryID
        {
            get { return ID; }
            private set { ID = value; }
        }

        /// <summary>
        /// Ŀ¼��
        /// </summary>
        public override string Name { get; set; }

        [Obsolete]
        public string DirectoryName
        {
            get { return Name; }
        }

        /// <summary>
        /// �ļ���������
        /// </summary>
        public int UserID { get; private set; }

        /// <summary>
        /// �ϼ�Ŀ¼
        /// </summary>
        public int ParentID { get; private set; }

        /// <summary>
        /// Ŀ¼��С
        /// </summary>
        public long TotalSize { get; private set; }

        /// <summary>
        /// Ŀ¼��С
        /// </summary>
        public long Size { get { return TotalSize; } }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateDate { get; private set; }

        /// <summary>
        /// ������ʱ��
        /// </summary>
        public DateTime UpdateDate { get; private set; }


        public string Icon
        {
            get;
            set;
        }
        public string SmallIcon
        {
            get { return string.Empty; }
        }

        public string BigIcon
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// �Ƿ��ļ�
        /// </summary>
        public bool IsFile
        {
            get { return false; }
        }

        /// <summary>
        /// �ļ�����
        /// </summary>
        public string TypeName
        {
            get { return "�ļ���"; }
        }


        #region IPrimaryKey<int> ��Ա

        public int GetKey()
        {
            return DirectoryID;
        }

        #endregion
    }

    public class DiskDirectoryCollection : EntityCollectionBase<int, DiskDirectory>
    {

        public DiskDirectoryCollection()
        { }

        public DiskDirectoryCollection(DataReaderWrap reader)
        {
            while (reader.Next)
            {
                this.Add(new DiskDirectory(reader));
            }
        }
    }
}