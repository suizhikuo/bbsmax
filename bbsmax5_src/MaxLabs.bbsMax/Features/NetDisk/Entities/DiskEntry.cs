//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Specialized;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Entities
{
    public class DiskEntry
    {
        int diskID;
        public int DiskID
        {
            get
            {
                return this.diskID;
            }
            set
            {
                this.diskID = value;
            }
        }

        int userID;
        public int UserID{
            get
            {
                return this.userID;
            }
            set
            {
                this.userID = value;
            }
        }

        string fileName;
        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
            }
        }

        DateTime createDate;
        public DateTime CreateDate
        {
            get
            {
                return this.createDate;
            }
            set
            {
                this.createDate = value;
            }
        }

        string directoryName;
        public string DirectoryName
        {
            get
            {
                return this.directoryName;
            }
            set
            {
                this.directoryName = value;
            }
        }

        int directoryID;
        public int DirectoryID
        {
            get
            {
                return this.directoryID;
            }
            set
            {
                this.directoryID = value;
            }
        }

        string path;
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }

        public string Icon
        {
            get
            {
                return string.Empty;
            }
        }
    }
}