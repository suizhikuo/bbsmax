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
using System.DirectoryServices;

namespace Max.WinUI
{
    public class SiteInfo
    {
        static readonly SiteInfo instance = new SiteInfo();

        public static SiteInfo Current
        {
            get
            {
                return instance;
            }
        }

        private string bindString;
        public string BindString
        {
            get
            {
                return bindString;
            }
            set
            {
                bindString = value;
            }
        }

        private string serverComment;
        public string ServerComment
        {
            get
            {
                return serverComment;
            }
            set
            {
                serverComment = value;
            }
        }

        private string ip;
        public string IP
        {
            get
            {
                return ip;
            }
            set
            {
                ip = value;
            }
        }

        private string port;
        public string Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }

        private string host;
        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value;
            }
        }

        private string defaultDoc;
        public string DefaultDoc
        {
            get
            {
                return defaultDoc;
            }
            set
            {
                defaultDoc = value;
            }
        }
        // ../1/root/bbsmax/ss
        private string virPath;//bbsmax/ss
        public string VirPath
        {
            get
            {
                return virPath;
            }
            set
            {
                virPath = value;
            }
        }

        private string webPath;
        public string WebPath
        {
            get
            {
                return webPath;
            }
            set
            {
                webPath = value;
            }
        }

        private string virtualName;
        public string VirtualName
        {
            get
            {
                return virtualName;
            }
            set
            {
                virtualName = value;
            }
        }

        private string userName_iusr;
        public string UserName_iusr
        {
            get
            {
                return userName_iusr;
            }
            set
            {
                userName_iusr = value;
            }
        }

        private string passWord_iusr;
        public string PassWord_iusr
        {
            get
            {
                return passWord_iusr;
            }
            set
            {
                passWord_iusr = value;
            }
        }

        private string userName_iwam;
        public string UserName_iwam
        {
            get
            {
                return userName_iwam;
            }
            set
            {
                userName_iwam = value;
            }
        }

        private string passWord_iwam;
        public string PassWord_iwam
        {
            get
            {
                return passWord_iwam;
            }
            set
            {
                passWord_iwam = value;
            }
        }

        private string appPool;
        public string AppPool
        {
            get
            {
                return appPool;
            }
            set
            {
                appPool = value;
            }
        }

        private string domainName;
        public string DomainName
        {
            get
            {
                return domainName;
            }
            set
            {
                domainName = value;
            }
        }

        private IISVersion iVersion;
        public IISVersion IVersion
        {
            get
            {
                return iVersion;
            }
            set
            {
                iVersion = value;
            }
        }

        private DirectoryEntry currentSite;
        public DirectoryEntry CurrentSite
        {
            get
            {
                return currentSite;
            }
            set
            {
                currentSite = value;
            }
        }
    }
}