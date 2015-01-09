//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web.Security;
using System.Collections.Generic;
using MaxLabs.bbsMax.Enums;

namespace Max.Installs
{
    public class Settings
    {
        Settings()
        {
        }

        static Settings()
        {
        }

        static readonly Settings instance = new Settings();

        public static Settings Current
        {
            get
            {
                return instance;
            }
        }

        public static string Version
        {
            get
            {
                return Versions[Versions.Length - 1];
            }
        }

        private string rootPath;
        public string RootPath
        {
            get
            {
                return rootPath;
            }
            set
            {
                rootPath = value;
            }
        }

        public static string[] Versions
        {
            get
            {
#if SQLSERVER
                return new string[] { "2.0.5800.0000", "2.3.0229.0000", "3.0.0730.0000", "3.0.0.0820", "3.0.0.0825", "3.0.0.0909", "3.0.0.1122", "3.0.0.1127", "3.0.0.1201", "4.0.0.0803", "4.0.0.0824", "4.0.0.0826", "4.0.0.0901", "4.0.0.0902", "4.0.0.0908", "4.0.3.0914", "4.0.4.0928", "4.0.5.1017", "4.0.6.1020", "4.1.0.1116", "4.1.0.1130", "4.2.0.1218", "4.2.0110", "4.2.1.0126", "4.2.2.0130", "4.2.3.0303", "5.0.0.0331", "5.0.0.0806", "5.0.0.0926", "5.0.1.1008" };
#endif
#if SQLITE
                return new string[] { "2.3.0229.0000", "3.0.0.1122" };
#endif
            }
        }


#if SQLSERVER
        private static string sqlserver = "Server={0};DataBase={1};User ID={2};Password={3};Connect Timeout=6;";
        private static string windows = "Server={0};Database={1};Trusted_Connection=True;Connect Timeout=6;";

        public string IConnectionString
        {
            get
            {

                return ConnectionStringFormat(IServerAddress, IUserID, IPassword, IDatabase, IsIWindows);
            }
        }

        public static string ConnectionStringFormat(string server, string uid, string password, string database, bool isWindows)
        {
            string connectionString;
            if (isWindows)
                connectionString = string.Format(windows, server, database);
            else
                connectionString = string.Format(sqlserver, server, database, uid, password);
            return connectionString;
        }

        private string iDatabase;
        public string IDatabase
        {
            get
            {
                return iDatabase;
            }
            set
            {
                iDatabase = value;
            }
        }

        private string iUserId;
        public string IUserID
        {
            get
            {
                return iUserId;
            }
            set
            {
                iUserId = value;
            }
        }

        private string iPassword;
        public string IPassword
        {
            get
            {
                return iPassword;
            }
            set
            {
                iPassword = value;
            }
        }

        public bool? DynamicCompress { get; set; }

        public bool? StaticCompress { get; set; }

        public string Licence { get; set; }

        public UrlFormat UrlFormat { get; set; }

        private string iServerAddress;
        public string IServerAddress
        {
            get
            {
                return iServerAddress;
            }
            set
            {
                iServerAddress = value;
            }
        }

        private bool isIWindows;
        public bool IsIWindows
        {
            get
            {
                return isIWindows;
            }
            set
            {
                isIWindows = value;
            }
        }



        public string IMasterConnectionString
        {
            get
            {
                return ConnectionStringFormat(IServerAddress, IUserID, IPassword, "master", IsIWindows);
            }
        }
#endif
#if SQLITE
        public string idMaxConnectionString
        {
            get
            {
                return string.Format("Data Source={0};Version=3", RootPath + IdMaxFilePath);
            }
        }

        public string bbsMaxConnectionString
        {
            get
            {
                return string.Format("Data Source={0};Version=3", RootPath + BbsMaxFilePath);
            }
        }



        private string bbsMaxDatabase;
        public string BbsMaxDatabase
        {
            get
            {
                return bbsMaxDatabase;
            }
            set
            {
                bbsMaxDatabase = value;
            }
        }

        private string idMaxDatabase;
        public string IdMaxDatabase
        {
            get
            {
                return idMaxDatabase;
            }
            set
            {
                idMaxDatabase = value;
            }
        }

        private string bbsMaxFilepath;
        public string BbsMaxFilePath
        {
            get
            {
                return bbsMaxFilepath;
            }
            set
            {
                bbsMaxFilepath = value;
            }
        }

        private string idMaxFilepath;
        public string IdMaxFilePath
        {
            get
            {
                return idMaxFilepath;
            }
            set
            {
                idMaxFilepath = value;
            }
        }
#endif

        private int timeZone;
        public int TimeZone
        {
            get
            {
                return timeZone;
            }
            set
            {
                timeZone = value;
            }
        }

        private string siteName;
        public string SiteName
        {
            get
            {
                return siteName;
            }
            set
            {
                siteName = value;
            }
        }

        private string siteUrl;
        public string SiteUrl
        {
            get
            {
                return siteUrl;
            }
            set
            {
                siteUrl = value;
            }
        }

        private string bbsName;
        public string BBSName
        {
            get
            {
                return bbsName;
            }
            set
            {
                bbsName = value;
            }
        }

        //private string bbsUrl;
        //public string BBSUrl
        //{
        //    get
        //    {
        //        return bbsUrl;
        //    }
        //    set
        //    {
        //        bbsUrl = value;
        //    }
        //}

        private string adminName;
        public string AdminName
        {
            get
            {
                return adminName;
            }
            set
            {
                adminName = value;
            }
        }

        //private string adminNickName;
        //public string AdminNickName
        //{
        //    get
        //    {
        //        return adminNickName;
        //    }
        //    set
        //    {
        //        adminNickName = value;
        //    }
        //}

        private string adminPassword;
        public string AdminPassword
        {
            get
            {
                return adminPassword;
            }
            set
            {
                adminPassword = value;
            }
        }

        private bool isThreadAlive;
        public bool IsThreadAlive
        {
            get
            {
                return isThreadAlive;
            }
            set
            {
                isThreadAlive = value;
            }
        }

        private bool isCompleted;
        public bool IsCompleted
        {
            get
            {
                return isCompleted;
            }
            set
            {
                isCompleted = value;
            }
        }

        private Max.Installs.SetupMode setupMode;
        public Max.Installs.SetupMode SetupMode
        {
            get
            {
                return setupMode;
            }
            set
            {
                setupMode = value;
            }
        }

        private int error;
        public int Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
            }
        }

        private int commandTimeout;
        public int CommandTimeout
        {
            get
            {
                return commandTimeout;
            }
            set
            {
                commandTimeout = value;
            }
        }

        private Progress progressAccess;
        public Progress ProgressAccess
        {
            get
            {
                if (progressAccess != null)
                    return progressAccess as Progress;
                return Progress.Notset;
            }
            set
            {
                progressAccess = value;
            }
        }

        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }
    }
}