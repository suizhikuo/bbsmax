//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using System.Web;
using System.IO;
using System.Xml;
using System.Text;
using System.Security.AccessControl;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using System.Resources;
using Max.Installs;
using Max.Installs.VersionService;
#if SQLSERVER
using System.Data.SqlClient;
using MaxLabs.DatabaseDeployTool;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
#endif
#if SQLITE
using System.Data.SQLite;
using MaxLabs.bbsMax.Enums;
#endif

namespace Max.Installs
{
    public class SetupManager
    {
        public static int StepChecker = -1;
        private static string SqlPath = string.Empty;

        public static List<string> ErrorMessages = new List<string>();

        private SetupManager()
        {
        }

        public delegate void InstallEventHandler(int percent, string message);
        //��װ���ݿ�
        public static void InstallDatabase(InstallEventHandler onProcess)
        {
            ResourceManager rm = Install_Bin.ResourceManager;
#if SQLSERVER

            bool isSql2000 = IsSQL2000();

            bool isNew = (SetupMode.New == Settings.Current.SetupMode);
            //string connStr = GetConnectionString();// + "Max pool size=200;";
            if (isNew)//ȫ�°�װ������ԭ�����ݿ������°�װ
            {
                //����������򴴽����ݿ�
                onProcess(1, "�����Ƿ����");
                CheckCreateDatabase();

                //�����ǰ�������ݿ��еı���洢���̡�������������
                onProcess(5, "����ԭ�нṹ������");

                if (isSql2000)
                    ExecuteSqlFromFile(rm, "clean_all_2000");
                else
                    ExecuteSqlFromFile(rm, "clean_all");

                //�����������
                onProcess(30, "����bbsmax���ݿ�");
                ExecuteSqlFromFile(rm, "create", 31, 97, onProcess);

                UpdateUrlFormat(Settings.Current.UrlFormat);
                ////�����������
                //onProcess(17, "�����������");
                //ExecuteSqlFromFile(rm, "schemes");

                ////�����ʹ洢���̡����� Sql05����
                //onProcess(40, "�����洢����");
                //ExecuteSqlFromFile(rm, "procedures");

                ////����Ĭ������ Vs����
                //onProcess(70, "����Ĭ������");
                //ExecuteSqlFromFile(rm, "data");

                onProcess(98, "��¼��ǰ�汾");
                UpdateVersion(Settings.Version);

                onProcess(100, "���");

            }
            else//���°�װ2.0->2.3->3.0 �Ժ�İ汾Ҫ�жϵ�ǰ�ǵڼ����汾��
            {
                onProcess(1, "׼������");

                try
                {
                    if (isSql2000)
                        ExecuteSqlFromFile(rm, "first_2000");
                    else
                        ExecuteSqlFromFile(rm, "first");
                }
                catch
                { }


                //�������ݿ��
                string[] versions = Settings.Versions;
                string currentVersion = GetCurrentVersion();

                int currentV = int.Parse(currentVersion[0].ToString());

                if (currentV < 4)
                {
                    if (V4Exists() == false)
                    {
                        //�����������
                        onProcess(1, "����bbsmax4���ݿ�");
                        ExecuteSqlFromFile(rm, "create", 1, 51, onProcess);
                    }
                }

                UpdateUrlFormat(Settings.Current.UrlFormat);



                //������� �����ǰ�������ݿ��еĴ洢���̡�������������
                onProcess(55, "������ڵĴ洢���̡�������������");

                if (isSql2000)
                    ExecuteSqlFromFile(rm, "clean_upgrade_2000");
                else
                    ExecuteSqlFromFile(rm, "clean_upgrade");


                bool updateStarted = false;
                foreach (string version in versions)
                {
                    if (!updateStarted && string.Compare(version, currentVersion, true) == 0)
                    {
                        //�汾�����ҵ��˵�ǰ�汾,��ô���¸��汾��ʼ����
                        updateStarted = true;
                        continue;
                    }

                    if (updateStarted)
                    {
                        try
                        {
                            ExecuteSqlFromFile(rm, "_" + version.Replace(".", "_"));
                            UpdateVersion(version);
                        }
                        catch (Exception ex)
                        {
                            CreateLog(ex);
                            throw new Exception("�������ݿ�汾�� " + version + " ʱʧ�ܣ�" + ex.Message);
                        }
                    }
                }

                if (!updateStarted)
                    throw new Exception("������ʹ����δ�������еİ汾���������޷��������������ϵ�ٷ���Ա�ṩ����");


                ////���´����洢���̡�������������
                //onProcess(60, "�ؽ��洢���̡�������������");
                //ExecuteSqlFromFile(rm, "procedures");

                //����ȫ������
                onProcess(80, "��������ȫ������");
                CheckResetFulltext();

                ExecuteSqlFromFile(rm, "lastsql");


                ////���첿�� ȫ�°�װ����
                //onProcess(80, "����������У�ԺͲ���");


                //Stream dbStream = new MemoryStream(Encoding.UTF8.GetBytes(Install_Bin.scheme_xml));
                //string errorLog;
                //DbDeployer.Update(Settings.Current.IConnectionString, dbStream, out errorLog);
                //if (!string.IsNullOrEmpty(errorLog))
                //    CreateLog(errorLog);
            }
#endif
#if SQLITE
            bool isNew = (SetupMode.New == Settings.Current.SetupMode);
            if (isNew)
            {
                try
                {
                    onProcess(20, "��װ���ݿ�");
                    //CanelReadOnly(Globals.RootPath() + Settings.Current.BbsMaxFilePath);
                    //CanelReadOnly(Globals.RootPath() + Settings.Current.IdMaxFilePath);
                    //File.WriteAllBytes(Globals.RootPath() + Settings.Current.BbsMaxFilePath, Install_Bin.sqlite_bbsMax);
                    onProcess(50, "��װ���ݿ�");
                    //File.WriteAllBytes(Globals.RootPath() + Settings.Current.IdMaxFilePath, Install_Bin.sqlite_idMax);
                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("��װ���ݿ�ʧ�ܡ�" + ex.Message);
                }
            }
            else
            {
                onProcess(1, "�������ݿ�");
                //CheckResetFulltext();

                //�������ݿ��
                string[] versions = Settings.Versions;
                string currentVersion = GetCurrentVersion();
                onProcess(20, "�������ݿ�");

                bool updateStarted = false;
                foreach (string version in versions)
                {
                    if (!updateStarted && string.Compare(version, currentVersion, true) == 0)
                    {
                        //�汾�����ҵ��˵�ǰ�汾,��ô���¸��汾��ʼ����
                        updateStarted = true;
                        continue;
                    }

                    if (updateStarted)
                    {
                        try
                        {
                            ExecuteSqlFromFile(rm, "sqlite_bbsMax_" + version.Replace(".", "_"));
                            ExecuteSqlFromFile(rm, "sqlite_idMax_" + version.Replace(".", "_"));
                            UpdateVersion(version);
                        }
                        catch (Exception ex)
                        {
                            CreateLog(ex);
                            throw new Exception("�������ݿ�汾�� " + version + " ʱʧ�ܣ�" + ex.Message);
                        }
                    }
                }
                onProcess(95, "�������ݿ�");
                if (!updateStarted)
                    throw new Exception("������ʹ����δ�������еİ汾���������޷��������������ϵ�ٷ���Ա�ṩ����");
            }
#endif
        }

        public static void AutoDeployDatabase(DbDeployer.InstallEventHandler onProcess, IEnumerable<string> tablePrefixs)
        {
            Stream dbStream = new MemoryStream(Encoding.UTF8.GetBytes(Install_Bin.scheme_xml));
            string errorLog;
            DbDeployer.Update(Settings.Current.IConnectionString, dbStream, onProcess, tablePrefixs, out errorLog);
            if (!string.IsNullOrEmpty(errorLog))
                CreateLog(errorLog);

            UpdateVersion(Settings.Versions[Settings.Versions.Length - 1]);
        }

#if SQLITE
        public static string SetDatabase()
        {
            try
            {
                CanelReadOnly(Settings.Current.RootPath + Settings.Current.BbsMaxFilePath);
                CanelReadOnly(Settings.Current.RootPath + Settings.Current.IdMaxFilePath);
                File.WriteAllBytes(Settings.Current.RootPath + Settings.Current.BbsMaxFilePath, Install_Bin.sqlite_bbsMax);
                File.WriteAllBytes(Settings.Current.RootPath + Settings.Current.IdMaxFilePath, Install_Bin.sqlite_idMax);

                CreateAdministrator();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }
#endif

        private static bool IsSQL2000()
        {
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                if (ConnectionState.Closed == connection.State)
                    connection.Open();

                SqlCommand command = new SqlCommand(@"
IF CHARINDEX('Microsoft SQL Server  2000', @@VERSION) = 1
	SELECT 1;
ELSE
	SELECT 0;
", connection);
                command.CommandTimeout = 60;
                int result = (int)command.ExecuteScalar();

                return result == 1;
            }
        }

        private static bool V4Exists()
        {
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                if (ConnectionState.Closed == connection.State)
                    connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM sysobjects WHERE [type]=N'U' AND [name] LIKE 'bx_%'", connection);
                command.CommandTimeout = 60;
                int count = (int)command.ExecuteScalar();

                return count > 80;
            }
        }

        private static void UpdateVersion(string Version)
        {
#if SQLSERVER
            string sqlString = @"IF OBJECT_ID ( 'bx_Version', 'U' ) IS NULL 
CREATE TABLE [bx_Version] (
[Version] [varchar] (16) COLLATE Chinese_PRC_CI_AS NOT NULL
)
IF EXISTS(SELECT * from [bx_Version])
    UPDATE bx_Version SET [Version]='" + Version + @"';
ELSE
    INSERT INTO bx_Version(Version) VALUES ('" + Version + @"');

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_Version')
    DROP TABLE Max_Version;
";

            string connStr = Settings.Current.IConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (ConnectionState.Closed == conn.State)
                    conn.Open();
                SqlCommand cmd;
                cmd = new SqlCommand(sqlString, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
#endif
#if SQLITE
            string sqlStr = string.Empty;
            using (SQLiteConnection conn = new SQLiteConnection(Settings.Current.idMaxConnectionString))
            {
                if (ConnectionState.Closed == conn.State)
                    conn.Open();
                SQLiteCommand cmd;
                cmd = new SQLiteCommand("SELECT * From Max_Version", conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    sqlStr = "UPDATE Max_Version SET Version='" + Version + "'";
                }
                catch
                {
                    cmd = new SQLiteCommand("CREATE TABLE [Max_Version] ([Version] TEXT NOT NULL)", conn);
                    cmd.ExecuteNonQuery();
                    sqlStr = "INSERT INTO Max_Version(Version) VALUES ('" + Version + "')";
                }
                cmd.CommandText = sqlStr;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
#endif
        }

        private static void UpdateUrlFormat(UrlFormat urlFormat)
        {
            FriendlyUrlSettings setting = new FriendlyUrlSettings();
            setting.UrlFormat = urlFormat;
            string sqlString = @"
IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bx_Settings') BEGIN
    IF EXISTS(SELECT * from [bx_Settings] WHERE [TypeName]='MaxLabs.bbsMax.Settings.FriendlyUrlSettings')
        UPDATE [bx_Settings] SET [Value]='" + setting.ToString() + @"' WHERE [TypeName]='MaxLabs.bbsMax.Settings.FriendlyUrlSettings';
    ELSE
        INSERT INTO [bx_Settings]([Key],[TypeName],[Value]) VALUES('*','MaxLabs.bbsMax.Settings.FriendlyUrlSettings','" + setting.ToString() + @"');
END
";
            string connStr = Settings.Current.IConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (ConnectionState.Closed == conn.State)
                    conn.Open();
                SqlCommand cmd;
                cmd = new SqlCommand(sqlString, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private static void ExecuteSqlFromFile(ResourceManager rm, string fileName)
        {
            ExecuteSqlFromFile(rm, fileName, 0, 0, null);
        }

        private static void ExecuteSqlFromFile(ResourceManager rm, string fileName, int beginPercent, int endPercent, InstallEventHandler onProcess)
        {
            string[] sqlAll = null;
            string sqlFromFile = rm.GetString(fileName);

            if (string.IsNullOrEmpty(sqlFromFile))
                return;

            CreateLog("ִ�нű���" + fileName);

            Regex regSql = new Regex(@"\n\bGO\b", RegexOptions.IgnoreCase);//����+go(GO)

            if (regSql.IsMatch(sqlFromFile))
                sqlAll = regSql.Split(sqlFromFile);
            else
                sqlAll = new string[] { sqlFromFile };

#if SQLSERVER
            if (sqlAll != null)
            {
                int totalPercent = endPercent - beginPercent;

                float unitPercent = (float)totalPercent / (float)sqlAll.Length;

                using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
                {
                    if (ConnectionState.Closed == connection.State)
                        connection.Open();
                    for (int i = 0; i < sqlAll.Length; i++)
                    {
                        try
                        {
                            if (sqlAll[i] != "")
                            {
                                if (onProcess != null)
                                    onProcess(beginPercent + (int)(unitPercent * i), "����ִ�нű�");

                                SqlCommand cmd = new SqlCommand(sqlAll[i], connection);
                                cmd.CommandTimeout = 3600;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (StringUtil.ContainsIgnoreCase(sqlAll[i], "--ignore error"))
                            {
                                CreateLog("ִ��Sql�����\r\n" + ex.Message);
                            }
                            else if (StringUtil.ContainsIgnoreCase(sqlAll[i], "sp_changeobjectowner"))
                            {
                                CreateLog("�޸ı�������߳����\r\n" + ex.Message);
                            }
                            else
                            {
                                connection.Close();
                                CreateLog(ex);
                                throw new Exception("���ݿ����ʧ�ܣ�\r\n" + sqlAll[i] + "\r\n" + ex.Message);
                            }
                        }
                    }
                }
            }
#endif
#if SQLITE
            //�ֱ�����������ݿ�
            string connStr = "";
            if (sqlAll != null)
            {
                if (fileName.StartsWith("sqlite_bbsMax", false, null))
                    connStr = Settings.Current.bbsMaxConnectionString;
                else if ((fileName.StartsWith("sqlite_idMax", false, null)))
                    connStr = Settings.Current.idMaxConnectionString;
                using (SQLiteConnection connection = new SQLiteConnection(connStr))
                {
                    if (ConnectionState.Closed == connection.State)
                        connection.Open();
                    for (int i = 0; i < sqlAll.Length; i++)
                    {
                        try
                        {
                            if (sqlAll[i] != "")
                            {
                                SQLiteCommand cmd = new SQLiteCommand(sqlAll[i], connection);
                                cmd.CommandTimeout = 3600;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            connection.Close();
                            CreateLog(ex);
                            throw new Exception("���ݿ����ʧ�ܣ�" + ex.Message);
                        }
                    }
                }
            }
#endif
        }

#if SQLSERVER

        public static string CheckCreateDatabase()
        {
            return CheckCreateDatabase(Settings.Current.IMasterConnectionString, Settings.Current.IDatabase);
        }

        //��鴴�����ݿ�
        public static string CheckCreateDatabase(string connectionString, string databaseName)
        {
            //string dbName = Settings.Current.IDatabase;
            string sql = @"IF DB_ID(N'" + databaseName + @"') IS NULL BEGIN
    CREATE DATABASE [" + databaseName + @"] COLLATE Chinese_PRC_CI_AS;
    SELECT 1;
END
ELSE
    SELECT 0;";
            string result;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.CommandTimeout = 60;
                    connection.Open();

                    int resultStatus = (int)command.ExecuteScalar();

                    switch (resultStatus)
                    {
                        case 1:
                            result = string.Empty;
                            break;

                        case 0:
                        default:
                            result = "���ݿ��Ѿ�����";
                            break;
                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
                connection.Close();
            }
            return result;
        }

        private static void CheckResetFulltext()
        {
            string sql = @"
IF EXISTS (SELECT * FROM bx_Settings WHERE TypeName = 'MaxLabs.bbsMax.Settings.SearchSettings' AND [Key] = 'SearchType' AND [Value] LIKE 'FullTextIndex') BEGIN
    IF (SELECT FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')) = 1 BEGIN
        IF (SELECT DATABASEPROPERTY (db_name(),'IsFulltextEnabled')) <> 1
		    EXEC sp_fulltext_database 'enable';

	    IF NOT EXISTS(SELECT * FROM sysfulltextcatalogs WHERE [name] = 'FTCatalog_bx_Posts')
		    EXEC sp_fulltext_catalog 'FTCatalog_bx_Posts','create';
        IF NOT EXISTS(SELECT t.name FROM sysobjects t INNER JOIN sysfulltextcatalogs ftc ON ftc.ftcatid=objectproperty(t.id,'TableFulltextCatalogID') WHERE t.name='bx_Posts')
	    BEGIN
		    EXEC sp_fulltext_table @tabname=N'[bx_Posts]', @action=N'create', @keyname=N'PK_bx_Posts', @ftcat=N'FTCatalog_bx_Posts';
		    EXEC sp_fulltext_column @tabname=N'[bx_Posts]', @colname=N'Content', @action=N'add';
		    EXEC sp_fulltext_column @tabname=N'[bx_Posts]', @colname=N'Subject', @action=N'add';
        END
	    IF NOT EXISTS(SELECT * FROM sysfulltextcatalogs WHERE [name]='FTCatalog_bx_Threads')
		    EXEC sp_fulltext_catalog 'FTCatalog_bx_Threads','create';
        IF NOT EXISTS(SELECT t.name FROM sysobjects t INNER JOIN sysfulltextcatalogs ftc ON ftc.ftcatid=objectproperty(t.id,'TableFulltextCatalogID') WHERE t.name='bx_Threads')
	    BEGIN
		    EXEC sp_fulltext_table @tabname=N'[bx_Threads]', @action=N'create', @keyname=N'PK_bx_Threads', @ftcat=N'FTCatalog_bx_Threads';
		    EXEC sp_fulltext_column @tabname=N'[bx_Threads]', @colname=N'Subject', @action=N'add';
	    END

	    EXEC sp_fulltext_table @tabname=N'[bx_Threads]', @action=N'start_change_tracking';
	    EXEC sp_fulltext_table @tabname=N'[bx_Threads]', @action=N'start_background_updateindex';
	    EXEC sp_fulltext_table @tabname=N'[bx_Posts]', @action=N'start_change_tracking';
	    EXEC sp_fulltext_table @tabname=N'[bx_Posts]', @action=N'start_background_updateindex';
    END
    ELSE BEGIN
        UPDATE bx_Settings SET [Value]='LikeStatement' WHERE TypeName = 'MaxLabs.bbsMax.Settings.SearchSettings' AND [Key] = 'SearchType';
        IF @@ROWCOUNT = 0
            INSERT INTO bx_Settings ([TypeName], [Key], [Value]) VALUES ('MaxLabs.bbsMax.Settings.SearchSettings', 'SearchType', 'LikeStatement');
    END
END
ELSE BEGIN
    IF (SELECT FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')) = 1 BEGIN
        IF (SELECT DATABASEPROPERTY (db_name(),'IsFulltextEnabled')) = 1 BEGIN
            EXEC sp_fulltext_table @tabname=N'[bx_Threads]', @action=N'stop_change_tracking';
            EXEC sp_fulltext_table @tabname=N'[bx_Threads]', @action=N'stop_background_updateindex';
            EXEC sp_fulltext_table @tabname=N'[bx_Posts]', @action=N'stop_change_tracking';
            EXEC sp_fulltext_table @tabname=N'[bx_Posts]', @action=N'stop_background_updateindex';
	        EXEC sp_fulltext_database 'disable';
        END
    END
    UPDATE bx_Settings SET [Value]='LikeStatement' WHERE TypeName = 'MaxLabs.bbsMax.Settings.SearchSettings' AND [Key] = 'SearchType';
    IF @@ROWCOUNT = 0
        INSERT INTO bx_Settings ([TypeName], [Key], [Value]) VALUES ('MaxLabs.bbsMax.Settings.SearchSettings', 'SearchType', 'LikeStatement');
END";

            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {

                SqlCommand command;
                try
                {
                    command = new SqlCommand(sql, connection);
                    command.CommandTimeout = 3600;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch
                {
                    sql = @"
UPDATE bx_Settings SET [Value]='LikeStatement' WHERE [TypeName] = 'MaxLabs.bbsMax.Settings.SearchSettings' AND [Key] = 'SearchType';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([TypeName], [Key], [Value]) VALUES ('MaxLabs.bbsMax.Settings.SearchSettings', 'SearchType', 'LikeStatement');";
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
            }

        }

#endif

        //Ҫ�������ݿ�İ汾
        public static string GetCurrentVersion()
        {
            string currentVersion = null;

            string cacheKey = "maxinstall_currentversion";
            if (HttpContext.Current == null || HttpContext.Current.Items[cacheKey] == null)
            {
#if SQLSERVER
                bool isSql2000 = IsSQL2000();

                string sqlString;
                if (isSql2000)
                {
                    sqlString = @"
DECLARE @schemaName nvarchar(256);
SELECT top 1 @schemaName = user_name(o.uid) FROM sysobjects o 
WHERE o.type='U' and o.name in('bx_Version','Max_Version','bx_Notifies','bbsMax_ThreadExchanges','bbsMax_AdminSessions','Max_Jobs','Max_SuperVariables','bbsMax_ShieldedUsers','System_Max_Jobs','bx_Users'); 
";
                }
                else
                {
                    sqlString = @"
DECLARE @schemaName nvarchar(256);
SELECT top 1 @schemaName = schema_name(o.schema_id) FROM sys.objects o 
WHERE o.type='U' and o.name in('bx_Version','Max_Version','bx_Notifies','bbsMax_ThreadExchanges','bbsMax_AdminSessions','Max_Jobs','Max_SuperVariables','bbsMax_ShieldedUsers','System_Max_Jobs','bx_Users'); 
";
                }

                sqlString += @"
EXEC( '
IF OBJECT_ID (''['+@schemaName+'].[bx_Version]'', ''U'' ) IS NOT NULL BEGIN
   IF EXISTS (SELECT * FROM ['+@schemaName+'].[bx_Version]) BEGIN
        SELECT Max([Version]) FROM ['+@schemaName+'].[bx_Version];
        RETURN;
    END
END
ELSE IF OBJECT_ID (''['+@schemaName+'].[Max_Version]'', ''U'' ) IS NOT NULL BEGIN
    IF EXISTS (SELECT * FROM ['+@schemaName+'].[Max_Version]) BEGIN
        SELECT Max([Version]) FROM ['+@schemaName+'].[Max_Version];
        RETURN;
    END
END
IF OBJECT_ID (''['+@schemaName+'].[bx_Notifies]'', ''U'' ) IS NOT NULL
    SELECT ''4.0.0.0908'';
ELSE IF OBJECT_ID (''['+@schemaName+'].[bbsMax_ThreadExchanges]'', ''U'' ) IS NOT NULL AND OBJECT_ID (''['+@schemaName+'].[bbsMax_AdminSessions]'', ''U'' ) IS NOT NULL
    SELECT ''3.0.0.1122'';
ELSE IF OBJECT_ID (''['+@schemaName+'].[Max_Jobs]'', ''U'' ) IS NOT NULL AND OBJECT_ID ( ''['+@schemaName+'].[Max_Urls]'', ''U'' ) IS NOT NULL
    SELECT ''3.0.0730.0000'';
ELSE IF OBJECT_ID (''['+@schemaName+'].[Max_SuperVariables]'', ''U'' ) IS NOT NULL AND OBJECT_ID ( ''['+@schemaName+'].[Max_ActivationSerials]'', ''U'' ) IS NOT NULL
    SELECT ''2.3.0229.0000'';
ELSE IF OBJECT_ID (''['+@schemaName+'].[bbsMax_ShieldedUsers]'', ''U'' ) IS NOT NULL
    SELECT ''2.0.5800.0000'';
ELSE IF OBJECT_ID (''['+@schemaName+'].[System_Max_Jobs]'', ''U'' ) IS NOT NULL
    SELECT ''2.0.2600.0000'';
ELSE
    SELECT '''';
');
";

                using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlString, connection);
                        command.CommandTimeout = 60;

                        currentVersion = (string)command.ExecuteScalar();

                        //if (string.IsNullOrEmpty(currentVersion))
                        //    throw new Exception("�޷��õ�bbsmax��İ汾,�޷�����,");

                    }
                    finally
                    {
                        connection.Close();
                    }
                }
#endif
#if SQLITE
                using (SQLiteConnection connection = new SQLiteConnection(Settings.Current.idMaxConnectionString))
                {
                    try
                    {
                        connection.Open();
                        SQLiteCommand command = new SQLiteCommand("SELECT Max([Version]) FROM [Max_Version];", connection);
                        currentVersion = (string)command.ExecuteScalar();
                    }
                    catch 
                    {
                        try
                        {
                            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM [Max_Jobs];", connection);
                            command.ExecuteNonQuery();
                            currentVersion = "3.0.0.1120";
                        }
                        catch
                        {
                            currentVersion = "2.3.0229.0000";
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
#endif
                if (currentVersion == null && HttpContext.Current != null)
                    HttpContext.Current.Items[cacheKey] = currentVersion;
            }
            else
                currentVersion = (string)HttpContext.Current.Items[cacheKey];
            return currentVersion;
        }

        public static Settings GetSettings()
        {
            Settings settings = Settings.Current;
#if SQLSERVER
            string sqlString = @"
IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='System_Max_Settings') BEGIN
    SELECT [SettingKey] AS [Key], [SettingValue] AS [Value] FROM [System_Max_Settings] WHERE [SettingKey] IN ('SiteName','Siteurl')
    Union All SELECT [SettingKey] AS [Key], [SettingValue] AS [Value] FROM [System_bbsMax_Settings] WHERE [SettingKey] = 'bbsName';

    SELECT [SettingValue] AS [Value] FROM [System_Max_Settings] WHERE [SettingKey] = 'Extension';
END
ELSE BEGIN
    SELECT [Key], [Value] FROM bx_Settings WHERE TypeName = 'MaxLabs.bbsMax.Settings.SiteSettings' AND [Key] IN ('SiteName', 'SiteUrl', 'BbsName');

    SELECT [Value] FROM bx_Settings WHERE TypeName = 'MaxLabs.bbsMax.Settings.FriendlyUrlSettings';
END


IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_UsersInRoles') BEGIN
    exec('SELECT u.UserName AS Username, u.Password FROM Max_UsersInRoles m INNER JOIN Max_UserProfiles n ON m.UserID = n.UserID INNER JOIN Max_Users u on m.UserID = u.UserID where m.RoleID = 1;')
END
ELSE IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bx_UserVars') BEGIN
    exec('SELECT u.Username, uv.Password FROM bx_UserRoles m INNER JOIN bx_UserVars uv ON m.UserID = uv.UserID INNER JOIN bx_Users u ON m.UserID = u.UserID WHERE m.RoleID = ''db0ca05e-c107-40f2-a52d-0d486b5d6cb0'';')
END
ELSE BEGIN
    exec('SELECT u.Username, u.Password FROM bx_UserRoles m INNER JOIN bx_Users u ON m.UserID = u.UserID WHERE m.RoleID = ''db0ca05e-c107-40f2-a52d-0d486b5d6cb0'';')
END
";

            try
            {
                using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlString, connection);
                    command.CommandTimeout = 60;
                    SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        string key = dr["Key"].ToString().ToUpper();
                        string value = dr["Value"].ToString();
                        switch (key)
                        {
                            case "SITENAME":
                                settings.SiteName = value;
                                break;
                            case "SITEURL":
                                settings.SiteUrl = value;
                                break;
                            case "BBSNAME":
                                settings.BBSName = value;
                                break;
                            //case "BBSURL":
                            //    settings.BBSUrl = value;
                            //break;
                            default:
                                break;
                        }
                    }
                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            string str = dr["Value"].ToString().ToLower();
                            if (str == ".aspx")
                            {
                                settings.UrlFormat = UrlFormat.Aspx;
                            }
                            else if (str == ".html")
                                settings.UrlFormat = UrlFormat.Html;
                            else if (str.EndsWith("|aspx"))
                                settings.UrlFormat = UrlFormat.Aspx;
                            else if (str.EndsWith("|html"))
                                settings.UrlFormat = UrlFormat.Html;
                            else if (str.EndsWith("|Folder"))
                                settings.UrlFormat = UrlFormat.Folder;
                            else
                                settings.UrlFormat = UrlFormat.Query;

                        }
                    }
                    if (dr.NextResult())
                    {
                        //settings.AdminNickName = string.Empty;
                        settings.AdminName = string.Empty;
                        while (dr.Read())
                        {
                            settings.AdminName += dr["Username"].ToString() + ",";
                            //settings.AdminNickName += dr["NickName"].ToString() + ",";
                            settings.AdminPassword = dr["Password"].ToString();
                        }
                    }
                    settings.AdminName = settings.AdminName.TrimEnd(',');
                    //settings.AdminNickName = settings.AdminNickName.TrimEnd(',');
                    dr.Close();
                    connection.Close();
                }
            }
            catch
            {
                settings.SiteName = "";
                settings.SiteUrl = "";
                settings.BBSName = "";
                //settings.BBSUrl = "";
                settings.AdminName = "";
                //settings.AdminNickName = "";
            }
#endif
#if SQLITE

            string sqlString = " SELECT [SettingValue],[SettingKey] FROM [System_bbsMax_Settings] WHERE [SettingKey] = 'BbsUrl' or [SettingKey] ='BbsName';";
            using (SQLiteConnection cn = new SQLiteConnection(Settings.Current.bbsMaxConnectionString))
            {
                cn.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlString, cn);
                SQLiteDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string key = dr["SettingKey"].ToString().ToUpper();
                    string value = dr["SettingValue"].ToString();
                    switch (key)
                    {
                        case "BBSNAME":
                            settings.BBSName = value;
                            break;
                        case "BBSURL":
                            settings.BBSUrl = value;
                            break;
                        default:
                            break;
                    }
                }
                dr.Close();
            }

            sqlString = @"SELECT [SettingValue],[SettingKey] FROM [System_Max_Settings] WHERE [SettingKey] = 'SiteName' or [SettingKey] = 'SiteUrl';
SELECT u.UserName,n.NickName FROM Max_UsersInRoles m INNER JOIN Max_UserProfiles n ON m.UserID = n.UserID INNER JOIN Max_Users u on m.UserID=u.UserID where m.RoleID=1;";
            using (SQLiteConnection conn = new SQLiteConnection(Settings.Current.idMaxConnectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlString, conn);
                SQLiteDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string key = dr["SettingKey"].ToString().ToUpper();
                    string value = dr["SettingValue"].ToString();
                    switch (key)
                    {
                        case "SITENAME":
                            settings.SiteName = value;
                            break;
                        case "SITEURL":
                            settings.SiteUrl = value;
                            break;
                        default:
                            break;
                    }
                }
                if (dr.NextResult())
                {
                    settings.AdminNickName = string.Empty;
                    settings.AdminName = string.Empty;
                    while (dr.Read())
                    {
                        settings.AdminName += dr["UserName"].ToString() + ",";
                        settings.AdminNickName += dr["NickName"].ToString() + ",";
                    }
                }
                settings.AdminName = settings.AdminName.TrimEnd(',');
                settings.AdminNickName = settings.AdminNickName.TrimEnd(',');
                dr.Close();
                conn.Close();
            }
#endif

            if (string.IsNullOrEmpty(settings.SiteName))
                settings.SiteName = "��վ��ҳ";

            if (string.IsNullOrEmpty(settings.SiteUrl))
                settings.SiteUrl = "-";

            if (string.IsNullOrEmpty(settings.BBSName))
                settings.BBSName = "bbsMax��̳";

            if (string.IsNullOrEmpty(settings.AdminName))
                settings.AdminName = "admin";

            return settings;
        }

        //��������Ա
        public static void CreateAdministrator()
        {
#if SQLSERVER
            string sql = @"
IF EXISTS( SELECT * FROM bx_Users WHERE UserID = 1) BEGIN
    UPDATE bx_Users SET Username = @Username WHERE UserID = 1
END
ELSE BEGIN

    DECLARE @UserID int;

    INSERT INTO bx_Users([Username]) VALUES (@Username);

    SELECT @UserID = @@IDENTITY;

    IF (@UserID IS NULL)
        SET @UserID = 1;

    DELETE bx_UserRoles WHERE UserID = @UserID AND RoleID = 'db0ca05e-c107-40f2-a52d-0d486b5d6cb0';

    INSERT INTO bx_UserRoles(UserID, RoleID, BeginDate, EndDate) VALUES (@UserID, 'db0ca05e-c107-40f2-a52d-0d486b5d6cb0', '1753-1-1 0:0:01', '9999-12-31 23:59:59');

END

IF EXISTS( SELECT * FROM bx_UserVars WHERE UserID = 1) BEGIN
    UPDATE bx_UserVars SET Password = @Password, PasswordFormat = 3 WHERE UserID = 1
END
ELSE BEGIN
    INSERT INTO bx_UserVars([Password], [PasswordFormat]) VALUES (@Password, 3);
END

UPDATE bx_Settings SET [Value] = @SiteName WHERE TypeName='MaxLabs.bbsMax.Settings.SiteSettings' AND [Key] = 'SiteName';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([Key], [TypeName], [Value]) VALUES ('SiteName', 'MaxLabs.bbsMax.Settings.SiteSettings', @SiteName);

UPDATE bx_Settings SET [Value] = @SiteUrl WHERE TypeName='MaxLabs.bbsMax.Settings.SiteSettings' AND [Key] = 'SiteUrl';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([Key], [TypeName], [Value]) VALUES ('SiteUrl', 'MaxLabs.bbsMax.Settings.SiteSettings', @SiteUrl);

UPDATE bx_Settings SET [Value] = @BbsName WHERE TypeName='MaxLabs.bbsMax.Settings.SiteSettings' AND [Key] = 'BbsName';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([Key], [TypeName], [Value]) VALUES ('BbsName', 'MaxLabs.bbsMax.Settings.SiteSettings', @BbsName);
";

            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                command.Parameters.AddWithValue("@Username", Settings.Current.AdminName);
                command.Parameters.AddWithValue("@Password", Settings.Current.AdminPassword);
                command.Parameters.AddWithValue("@SiteName", Settings.Current.SiteName);
                command.Parameters.AddWithValue("@SiteUrl", Settings.Current.SiteUrl);
                command.Parameters.AddWithValue("@BbsName", Settings.Current.BBSName);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("��������Աʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }
#endif
#if SQLITE
            using (SQLiteConnection cn = new SQLiteConnection(Settings.Current.idMaxConnectionString))
            {
                cn.Open();
                string sql = "SELECT * FROM Max_Users where UserID=1";
                SQLiteCommand cmd = new SQLiteCommand(sql, cn);
                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        sql = "Update Max_Users SET UserName=@userName,Password=@password,PasswordFormat=3,Question=0,Answer='',AnswerFormat=3 where UserID=1;";
                    }
                    else
                    {
                        sql = "INSERT INTO Max_Users([UserID],[UserName],[Password],[PasswordFormat],[Question],[Answer],[AnswerFormat])VALUES(1,@userName,@password,3,0,'',3);";
                    }
                }
                cmd = new SQLiteCommand("select * from Max_UsersInRoles Where UserID=1 and RoleID=1", cn);
                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                    }
                    else
                    {
                        sql += "INSERT INTO Max_UsersInRoles(UserID,RoleID,ExpiresDate)VALUES(1,1,'9999-12-31 23:59:59');";
                    }
                }
                cmd = new SQLiteCommand("select * from Max_UserProfiles where UserID=1", cn);
                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        sql += "Update Max_UserProfiles Set NickName=@userNickName Where UserID=1;";
                    }
                    else
                    {
                        sql += "INSERT INTO Max_UserProfiles ([UserID], [NickName], [PublicEmail], [Gender], [BirthdayYear], [BirthdayDate], [Signature], [Avatar], [AvatarSize], [CreateDate], [UpdateDate], [LastSignInDate], [CreateIP], [LastSignInIP], [Currency1], [Currency2], [Currency3],[UnreadMessages], [ExtendedProfiles]) VALUES (1, @userNickName, '', 1, 1984, 104, 'bbsmax���ܺã���ǿ��', '', 0, datetime('now','localtime'), datetime('now','localtime'), datetime('now','localtime'), '127.0.0.1', '', 0.0000, 0.0000, 0.0000,1, '');";
                    }
                }
                sql +=
                  @"UPDATE [System_Max_Settings] SET [SettingValue]=@SiteName WHERE [Catalog]='GlobalSetting' AND [SettingKey]='SiteName';"
                + @"UPDATE [System_Max_Settings] SET [SettingValue]=@SiteUrl WHERE [Catalog]='GlobalSetting' AND [SettingKey]='SiteUrl';";
                cmd = new SQLiteCommand(sql, cn);
                cmd.Parameters.AddWithValue("@userName", Settings.Current.AdminName);
                cmd.Parameters.AddWithValue("@password", Settings.Current.AdminPassword);
                cmd.Parameters.AddWithValue("@userNickName", Settings.Current.AdminNickName);
                cmd.Parameters.AddWithValue("@SiteName", Settings.Current.SiteName);
                cmd.Parameters.AddWithValue("@SiteUrl", Settings.Current.SiteUrl);
                cmd.ExecuteNonQuery();
            }
                using (SQLiteConnection cn = new SQLiteConnection(Settings.Current.bbsMaxConnectionString))
                {
                    string sqlText =
                          @"UPDATE [System_bbsMax_Settings] SET [SettingValue]=@BBSName WHERE [Catalog]='BBSSetting' AND [SettingKey]='BbsName';"
                        + @"UPDATE [System_bbsMax_Settings] SET [SettingValue]=@BBSUrl WHERE [Catalog]='BBSSetting' AND [SettingKey]='BbsUrl';";
                    cn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(sqlText, cn);
                    cmd.Parameters.AddWithValue("@BBSName", Settings.Current.BBSName);
                    cmd.Parameters.AddWithValue("@BBSUrl", Settings.Current.BBSUrl);
                    cmd.ExecuteNonQuery();
                }
#endif
        }

        public static void CanelReadOnly(string path)
        {
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.IsReadOnly)
                    fileInfo.IsReadOnly = false;
            }
        }

        public static void CreateFile(string path, Stream stream)
        {
            path = path.Replace("/", "\\");
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            else
                CanelReadOnly(path);

            try
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                byte[] data = new byte[10240];
                long size = stream.Length;
                int sizetoread;
                string ff = path;
                while (size != 0)
                {

                    sizetoread = size > 10240 ? 10240 : (int)size;

                    stream.Read(data, 0, sizetoread);
                    //data = reader.ReadBytes((int)sizetoread);

                    size -= sizetoread;
                    fs.Write(data, 0, sizetoread);

                }
                fs.Close();
            }
            catch
            {
            }
        }

        //�޸������ļ�
        public static string ConfigConnectionString()
        {
            try
            {
                string configFile = Globals.RootPath() + "bbsmax.config";
                CanelReadOnly(configFile);
                string configFileContent = Install_Bin.bbsmax_config;
#if SQLSERVER
                configFileContent = configFileContent.Replace("$(connectionString)", HttpUtility.HtmlEncode(Settings.Current.IConnectionString));

                if (Settings.Current.DynamicCompress != null)
                {
                    if (Settings.Current.DynamicCompress.Value)
                        configFileContent = configFileContent.Replace("$(DynamicCompress)", "ON");
                    else
                        configFileContent = configFileContent.Replace("$(DynamicCompress)", "OFF");
                }
                else
                    configFileContent = configFileContent.Replace("$(DynamicCompress)", "OFF");

                if (Settings.Current.Licence != null)
                {
                    configFileContent = configFileContent.Replace("$(Licence)", Settings.Current.Licence);
                }
                else
                    configFileContent = configFileContent.Replace("$(Licence)", string.Empty);

                if (Settings.Current.StaticCompress != null)
                {
                    if (Settings.Current.StaticCompress.Value)
                        configFileContent = configFileContent.Replace("$(StaticCompress)", "ON");
                    else
                        configFileContent = configFileContent.Replace("$(StaticCompress)", "OFF");
                }
                else
                    configFileContent = configFileContent.Replace("$(StaticCompress)", "OFF");
#endif
#if SQLITE
                configFileContent = configFileContent.Replace("$(connectionString_idmax)", Settings.Current.IdMaxFilePath);
#endif
                string rootConfig;
                if (File.Exists(configFile))
                {
                    rootConfig = File.ReadAllText(configFile, Encoding.UTF8);
                }
                else
                    rootConfig = string.Empty;

                Regex reg = new Regex(@"\<maxplugins\>(?is)(.*?|\s*?)\</maxplugins\>", RegexOptions.IgnoreCase);
                Match match = reg.Match(rootConfig);
                if (match.Success)
                {
                    configFileContent = configFileContent.Replace("<!--[maxPlugins]-->", match.Value);
                }
                else
                    configFileContent = configFileContent.Replace("<!--[maxPlugins]-->", string.Empty);

                //<passportConnectionSetting enable="true" server="http://my.chinaz.com" clientid="5" accesskey="BYMZMGM63N" timeout="5" />
                reg = new Regex(@"\<passportConnectionSetting(?is)(.*?|\s*?)/>", RegexOptions.IgnoreCase);
                match = reg.Match(rootConfig);
                if (match.Success)
                {
                    configFileContent = configFileContent.Replace("<!--[passportConnectionSetting]-->", match.Value);
                }
                else
                    configFileContent = configFileContent.Replace("<!--[passportConnectionSetting]-->", string.Empty);

                File.WriteAllText(configFile, configFileContent, Encoding.UTF8);
            }
            catch (Exception e)
            {
                CreateLog(e);
                return e.Message;
            }
            return string.Empty;
        }

        private static string tryDeleteFile(string fileName)
        {
            string rootPath = Globals.RootPath();
            if (!File.Exists(rootPath + fileName))
                return string.Empty;
            try
            {
                CanelReadOnly(rootPath + fileName);
                File.Delete(rootPath + fileName);
            }
            catch
            {

                return "�� " + fileName + " �ļ�ɾ��<br />";
            }
            return string.Empty;
        }

        private static string tryDeleteDirectory(string directoryName)
        {
            string rootPath = Globals.RootPath();
            if (!Directory.Exists(rootPath + directoryName))
                return string.Empty;

            try
            {
                Directory.Delete(rootPath + directoryName, true);
            }
            catch
            {
                return "�� " + directoryName + " Ŀ¼ɾ��<br />";
            }
            return string.Empty;
        }

        public static string DeleteSetupFiles(bool deleteImages)
        {
            StringBuilder message = new StringBuilder();

            #region ��װ�����ļ�
            //message.Append(tryDeleteFile("bin\\MaxInstall.dll"));
            //message.Append(tryDeleteFile("Install.aspx"));
            //message.Append(tryDeleteFile("bin\\Max.WebUI.dll"));
            //message.Append(tryDeleteFile("bin\\Max.Installs.XmlSerializers.dll"));
            #endregion

            message.Append(tryDeleteFile("bin\\MaxLab.bbsMaxDataProvider.Sqlite3.dll"));
            message.Append(tryDeleteFile("bin\\MaxLab.CommonDataProvider.Sqlite3.dll"));
            message.Append(tryDeleteFile("bin\\System.Data.SQLite.dll"));
            message.Append(tryDeleteFile("bin\\MaxLab.MaxUbb.dll"));
            message.Append(tryDeleteFile("bin\\zzbird.bbsMax.Core.dll"));
            message.Append(tryDeleteFile("bin\\zzbird.bbsMax.dll"));
            message.Append(tryDeleteFile("bin\\zzbird.bbsMaxDataProvider.SqlServer2005.dll"));
            message.Append(tryDeleteFile("bin\\zzbird.Common.dll"));
            message.Append(tryDeleteFile("bin\\zzbird.Common.XmlSerializers.dll"));
            message.Append(tryDeleteFile("bin\\zzbird.CommonDataProvider.SqlServer2005.dll"));
            message.Append(tryDeleteFile("bin\\zzbird.bbsMax.dll"));

            //��ʼɾ��bbsmax 2.3 ���ļ���Ŀ¼
            message.Append(tryDeleteFile("zzbird.config"));
            message.Append(tryDeleteFile("zzbird.bbsmax.config"));
            message.Append(tryDeleteFile("zzbird.idmax.config"));

            //message.Append(tryDeleteFile("Common"));


#if !DEBUG
            message.Append(tryDeleteFile("InstallLog.txt"));
            if (HttpContext.Current != null)
            {
                message.Append(tryDeleteFile(Path.GetFileName(HttpContext.Current.Request.PhysicalPath)));
            }
            message.Append(tryDeleteFile("bin\\MaxInstall.dll"));
            
#endif

            message.Append(tryDeleteDirectory("idMax_Themes"));
            message.Append(tryDeleteDirectory("idMax_Languages"));
            message.Append(tryDeleteDirectory("idMax_Management"));
            message.Append(tryDeleteDirectory("idMax_Resources"));

            message.Append(tryDeleteDirectory("bbsMax_Themes"));
            message.Append(tryDeleteDirectory("bbsMax_Languages"));
            message.Append(tryDeleteDirectory("bbsMax_Management"));
            message.Append(tryDeleteDirectory("bbsMax_Resources"));

            message.Append(tryDeleteDirectory("Common"));
            message.Append(tryDeleteDirectory("Web_Code"));
            message.Append(tryDeleteDirectory("App_Code"));

            //ɾ��3.0�ļ�
            //if (deleteImages)
            //    message.Append(tryDeleteDirectory("Images"));

            message.Append(tryDeleteDirectory("Languages"));
            message.Append(tryDeleteDirectory("Admin"));
            message.Append(tryDeleteDirectory("Themes"));
            message.Append(tryDeleteDirectory("MaxTemp"));



            //ɾ�����
            if (message.Length > 0)
                message.Insert(0, "��Ҫ��ʾ��Ϊ�����ķ�������ȫ���룺<br /><br />");

            return message.ToString();
        }


        //ɾ����ǰ��û�õ��ļ�
        public static void DeleteOldFiles()
        {
            string fileListContent = Install_Bin.filelist;
            if (fileListContent != string.Empty)
            {
                string[] fileList = Regex.Split(fileListContent, @"\r\n");
                string rootPath = Globals.RootPath().Replace("/", "\\");
                deleteOldFiles(rootPath, fileList, "_codes");
                deleteOldFiles(rootPath, fileList, "max-admin");
                deleteOldFiles(rootPath, fileList, "max-dialogs");

                List<string> templets = new List<string>();
                foreach (string newFile in fileList)
                {
                    if (newFile.StartsWith("max-templates\\"))
                    {
                        //max-templates\default\
                        string str = newFile.Substring(newFile.IndexOf('\\') + 1);
                        string temp = "max-templates\\" + str.Substring(0, str.IndexOf('\\'));
                        if (templets.Contains(temp) == false)
                            templets.Add(temp);
                    }
                }

                string[] paths = Directory.GetDirectories(rootPath + "max-templates", "*", SearchOption.TopDirectoryOnly);
                foreach (string path in paths)
                {
                    string dir = path.Substring(rootPath.Length);
                    foreach (string temp in templets)
                    {
                        if (dir.StartsWith(temp))
                        {
                            deleteOldFiles(rootPath, fileList, dir);
                            break;
                        }
                    }
                }

                //deleteOldFiles(rootPath, fileList, "max-templates\\default");
                deleteOldFiles(rootPath, fileList, "max-temp\\parsed-template\\max-plugins");
                deleteOldFiles(rootPath, fileList, "max-temp\\parsed-template\\max-templates\\default");

                try
                {
                    Directory.Delete(rootPath + "max-temp\\parsed-template\\archiver", true);
                }
                catch { }

                try
                {
                    Directory.Delete(rootPath + "max-temp\\parsed-template\\max-admin", true);
                }
                catch { }

                try
                {
                    Directory.Delete(rootPath + "max-temp\\parsed-template\\max-dialogs", true);
                }
                catch { }
            }
        }
        private static void deleteOldFiles(string rootPath, string[] fileList, string dir)
        {
            string[] oldFiles = Directory.GetFiles(rootPath + dir, "*", SearchOption.AllDirectories);
            foreach (string oldFile in oldFiles)
            {
                string tempOldFile = oldFile.Replace("/","\\");
                
                if (dir .StartsWith("max-templates"))//�»��߿�ͷ�Ĳ�����
                {
                    int m = tempOldFile.LastIndexOf("\\");
                    string str = tempOldFile.Substring(m + 1).ToLower();
                    //if (str.StartsWith("_"))
                    //    continue;
                    if (str.EndsWith(".aspx") == false && str.EndsWith(".ascx") == false)
                        continue;
                }
                bool has = false;
                foreach (string newFile in fileList)
                {
                    string temp;
                    //if (dir.StartsWith("max-templates") && dir != "max-templates\\default")//
                    //{
                    //    temp = newFile.Replace("max-templates\\default", dir);
                    //}
                    //else
                        temp = newFile;

                    if (string.Compare(tempOldFile, rootPath + temp.Replace("/", "\\"), true) == 0)
                    {
                        has = true;
                        break;
                    }
                }
                if (has == false)
                {
                    try
                    {
                        File.Delete(oldFile);
                    }
                    catch
                    {
                    }
                }
            }
        }


        public static string AlterGolbals()
        {
            return AlterGolbals(Globals.RootPath());
        }

        public static string AlterGolbals(string rootPath)
        {
#if !DEBUG
            try
            {
                rootPath = rootPath + "Global.asax";
                CanelReadOnly(rootPath);
                File.WriteAllText(rootPath, Install_Bin.Global_asax);
            }
            catch (Exception e)
            {
                CreateLog(e);
                return e.Message;
            }
#endif
            return string.Empty;
        }

        #region ��bbsmax 3.0 ������ 4.0 Ҫ�ر����

        public static void ConvertRoles()
        {
            RoleCollection newRoles = new RoleCollection();

            string sql = @"
--�����û������ݵĴ�����

IF EXISTS(SELECT * FROM sysobjects WHERE [type]=N'TR' AND [name]=N'bx_UserRoles_AfterUpdate')
	DROP TRIGGER bx_UserRoles_AfterUpdate;
--GO
";
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("ɾ��������bx_UserRoles_AfterUpdateʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }



            Guid vipRoleID = new Guid(new byte[] { 152, 198, 223, 228, 218, 198, 221, 78, 191, 59, 129, 195, 81, 168, 105, 207 });

            //�����ο͡�everyone
            sql = @"
IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'Max_Roles') AND EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_UsersInRoles') BEGIN

	IF NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('Max_Roles') AND [name] = 'NewRoleID')
		ALTER TABLE Max_Roles add [NewRoleID] uniqueidentifier NOT NULL DEFAULT(NEWID());

    SELECT * FROM Max_Roles WHERE RoleID > 0;

    IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'bbsMax_PointLevels')
        SELECT * FROM bbsMax_PointLevels WHERE RequireRoleID = 0;
END
ELSE
    SELECT -9999 AS RoleID;
";
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            #region �����߼�

                            int roleID = (int)reader["RoleID"];

                            //Max_Roles������ڣ��˳��û���ת��
                            if (roleID == -9999)
                                return;

                            Guid newRoleID = (Guid)reader["NewRoleID"];
                            string roleName = (string)reader["RoleName"];
                            string logoUrl = (string)reader["LogoUrl"];
                            string displayColor = (string)reader["DisplayColor"];

                            Role role;

                            switch (roleID)
                            {
                                case 1:
                                    role = Role.Owners.Clone();//Role.CreateManagerRole();
                                    //role.RoleID = newRoleID;
                                    role.Title = roleName;
                                    break;

                                case 2:
                                    role = Role.Administrators.Clone();
                                    //role.RoleID = newRoleID;
                                    role.Title = roleName;
                                    break;

                                case 3:
                                    role = Role.SuperModerators.Clone();
                                    //role.RoleID = newRoleID;
                                    role.Title = roleName;
                                    break;

                                case 5:
                                    role = Role.CreateNormalRole();
                                    role.RoleID = vipRoleID;
                                    role.Name = roleName;
                                    role.Title = roleName;
                                    break;

                                default:
                                    if (roleID > 8)
                                    {
                                        role = Role.CreateNormalRole();
                                        role.RoleID = newRoleID;
                                        role.Name = roleName;
                                    }
                                    else
                                        continue;
                                    break;
                            }

                            newRoles.Set(role);

                            #endregion
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                Role role = Role.CreateLevelRole();

                                role.RequiredPoint = (int)reader["RequirePoints"];
                                role.RoleID = Guid.NewGuid();
                                role.Name = (string)reader["LevelName"];
                                role.Title = role.Name;
                                role.StarLevel = (int)reader["Stars"];

                                newRoles.Add(role);
                            }
                        }

                        newRoles.Sort();
                    }

                    sql = @"
UPDATE bx_Settings SET [Value] = @RolesString WHERE TypeName = 'MaxLabs.bbsMax.Settings.RoleSettings' AND [Key] = 'Roles';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([Key], [Value], [TypeName]) VALUES ('Roles', @RolesString, 'MaxLabs.bbsMax.Settings.RoleSettings');

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_UsersInRoles') BEGIN
	truncate table [bx_UserRoles];
    
    UPDATE Max_Roles SET NewRoleID = @NewRoleID1 WHERE RoleID = 1;
    UPDATE Max_Roles SET NewRoleID = @NewRoleID2 WHERE RoleID = 2;
    UPDATE Max_Roles SET NewRoleID = @NewRoleID3 WHERE RoleID = 3;
    UPDATE Max_Roles SET NewRoleID = @NewRoleID5 WHERE RoleID = 5;

	INSERT INTO [bx_UserRoles]
           ([UserID]
           ,[RoleID]
           ,[BeginDate]
           ,[EndDate])
     SELECT 
           u.UserID,--(<UserID, int,>
           r.NewRoleID,--,<RoleID, uniqueidentifier,>
           u.EnabledDate,--,<BeginDate, datetime,>
           u.ExpiresDate--,<EndDate, datetime,>)
		FROM Max_UsersInRoles u WITH (NOLOCK)
        INNER JOIN Max_Roles r WITH (NOLOCK) ON u.RoleID = r.RoleID
        WHERE u.RoleID IN (1,2,3,5) OR u.RoleID > 8;

	DROP TABLE Max_UsersInRoles;
END

DROP TABLE Max_Roles;
";
                    command.CommandTimeout = 3600;
                    command.CommandText = sql;

                    command.Parameters.AddWithValue("@NewRoleID1", Role.Owners.RoleID);
                    command.Parameters.AddWithValue("@NewRoleID2", Role.Administrators.RoleID);
                    command.Parameters.AddWithValue("@NewRoleID3", Role.SuperModerators.RoleID);
                    command.Parameters.AddWithValue("@NewRoleID5", vipRoleID);

                    SqlParameter param = new SqlParameter("@RolesString", SqlDbType.NText);
                    param.Value = newRoles.GetValue();
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("�����û�������ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }


            }
        }

        public static void ConvertPoints()
        {

            string sql = @"

IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'bbsMax_ExtendedPoints') AND EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_ExtendedPoints') BEGIN
    SELECT * FROM bbsMax_ExtendedPoints;
END
ELSE
    SELECT -9999 AS PointID;

IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'System_bbsMax_Settings') AND EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='System_bbsMax_Settings') BEGIN
    SELECT * FROM System_bbsMax_Settings;
END
ELSE
    SELECT '-9999' AS Catalog;

";
            PointSettings pointSetting = new PointSettings();

            UserPointCollection points = new UserPointCollection();

            Dictionary<int, bool> allowImports = new Dictionary<int, bool>();
            Dictionary<int, bool> allowExports = new Dictionary<int, bool>();
            Dictionary<int, int> ratios = new Dictionary<int, int>();

            int exchangeMinBalance = 0;
            double exchangeTax = 0.2;
            int tradePointID = 0;
            double tradingTax = 0.2;
            int transferMinBalance = 0;
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int pointID = reader.GetInt32(reader.GetOrdinal("PointID"));

                            if (pointID == -9999)
                                return;

                            if (pointID > 0 && pointID < 9)
                            {
                                pointID = pointID - 1;

                                if (allowExports.ContainsKey(pointID))
                                    continue;

                                UserPoint userPoint = new UserPoint();

                                allowExports.Add(pointID, reader.GetBoolean(reader.GetOrdinal("AllowExport")));
                                allowImports.Add(pointID, reader.GetBoolean(reader.GetOrdinal("AllowImport")));
                                ratios.Add(pointID, reader.GetInt32(reader.GetOrdinal("Ratio")));

                                userPoint.Type = (MaxLabs.bbsMax.Enums.UserPointType)pointID;
                                userPoint.Name = reader.GetString(reader.GetOrdinal("PointName"));
                                userPoint.UnitName = reader.GetString(reader.GetOrdinal("PointUnit"));
                                userPoint.InitialValue = reader.GetInt32(reader.GetOrdinal("DefaultPoint"));
                                userPoint.Display = reader.GetBoolean(reader.GetOrdinal("IsPublic"));
                                userPoint.Enable = reader.GetBoolean(reader.GetOrdinal("IsEnabled"));
                                userPoint.MaxValue = reader.GetInt32(reader.GetOrdinal("MaxValue"));
                                userPoint.MinValue = reader.GetInt32(reader.GetOrdinal("MinValue"));

                                points.Add(userPoint);
                            }
                        }

                        if (reader.NextResult())
                        {
                            string pointformula = string.Empty;
                            while (reader.Read())
                            {
                                string catalog = reader.GetString(reader.GetOrdinal("Catalog"));
                                if (catalog == "-9999")
                                    break;

                                if (string.Compare(catalog, "PointSetting", true) == 0)
                                {
                                    string key = reader.GetString(reader.GetOrdinal("SettingKey"));
                                    string value = reader.GetString(reader.GetOrdinal("SettingValue"));
                                    try
                                    {
                                        switch (key.ToLower())
                                        {
                                            case "exchangeminbalance": exchangeMinBalance = int.Parse(value); break;
                                            case "exchangetax": exchangeTax = double.Parse(value); break;
                                            case "tradepointid": tradePointID = int.Parse(value) - 1; break;
                                            case "tradingtax": tradingTax = double.Parse(value); break;
                                            case "transferminbalance": transferMinBalance = int.Parse(value); break;
                                            case "pointname": pointSetting.GeneralPointName = value; break;
                                            case "pointformula": pointformula = value; break;
                                            default: break;
                                        }
                                    }
                                    catch { }
                                }
                            }
                            if (string.IsNullOrEmpty(pointformula) == false)
                                pointSetting.GeneralPointExpression = GetGeneralPointExpression(pointformula);

                        }
                    }

                    pointSetting.UserPoints = points;


                    if (allowImports.Count > 0)
                    {
                        //�һ�����
                        PointExchangeProportionCollection ExchangeProportions = new PointExchangeProportionCollection();

                        for (int i = 0; i < 8; i++)
                        {
                            if (ratios.ContainsKey(i))
                            {
                                ExchangeProportions.Add((UserPointType)i, ratios[i] == 0 ? 1 : ratios[i]);
                            }
                            else
                                ExchangeProportions.Add((UserPointType)i, 1);
                        }

                        pointSetting.ExchangeProportions = ExchangeProportions;

                        //�һ�����
                        PointExchangeRuleCollection PointExchangeRules = new PointExchangeRuleCollection();

                        foreach (KeyValuePair<int, bool> pair in allowImports)
                        {
                            if (pair.Value)//�������
                            {
                                foreach (KeyValuePair<int, bool> tempPair in allowExports)
                                {
                                    if (tempPair.Key == pair.Key)
                                        continue;

                                    if (tempPair.Value)//����ҳ�
                                    {
                                        PointExchangeRule rule = new PointExchangeRule();
                                        rule.PointType = (UserPointType)tempPair.Key;
                                        rule.TargetPointType = (UserPointType)pair.Key;
                                        try
                                        {
                                            rule.TaxRate = (int)(exchangeTax * 100);
                                        }
                                        catch { }

                                        PointExchangeRules.Add(rule);
                                    }
                                }
                            }
                        }

                        pointSetting.PointExchangeRules = PointExchangeRules;
                        try
                        {
                            pointSetting.TradeRate = (int)(tradingTax * 100);
                        }
                        catch { }

                        //PointTransferRuleCollection PointTransferRules = new PointTransferRuleCollection();
                        //PointTransferRule tRule = new PointTransferRule();
                        //tRule.CanTransfer = true;
                        //try
                        //{
                        //    tRule.PointType = (UserPointType)tradePointID;
                        //}
                        //catch
                        //{
                        //}
                        //tRule.TaxRate = pointSetting.TradeRate;

                        //PointTransferRules.Add(tRule);
                        //pointSetting.PointTransferRules = PointTransferRules;

                        pointSetting.PointTransferRules = new PointTransferRuleCollection();
                        pointSetting.PointIcons = new PointIconCollection();
                    }

                    sql = @"
UPDATE bx_Settings SET [Value] = @PointString WHERE TypeName = 'MaxLabs.bbsMax.Settings.PointSettings' AND [Key] = '*';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([Key], [Value], [TypeName]) VALUES ('*', @PointString, 'MaxLabs.bbsMax.Settings.PointSettings');


DROP TABLE bbsMax_ExtendedPoints;
";
                    command.CommandText = sql;

                    SqlParameter param = new SqlParameter("@PointString", SqlDbType.NText);
                    param.Value = pointSetting.ToString();
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("������չ��������ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }


        }

        public static void ConvertDefaultEmotions()
        {
            string sourceDir = MaxLabs.bbsMax.UrlUtil.JoinUrl(MaxLabs.bbsMax.Globals.ApplicationPath, "Images/Emoticons/Default/");

            if (Directory.Exists(sourceDir) == false)
                return;


            string targetDir = MaxLabs.bbsMax.UrlUtil.JoinUrl(MaxLabs.bbsMax.Globals.ApplicationPath, "max-assets/icon-emoticon/");

            try
            {
                if (Directory.Exists(targetDir))
                {
                    Directory.Delete(targetDir, true);
                    Directory.CreateDirectory(targetDir);
                }

                FileInfo[] files = new DirectoryInfo(sourceDir).GetFiles();

                foreach (FileInfo file in files)
                {
                    file.MoveTo(targetDir + file.Name);
                }

            }
            catch
            {
                ErrorMessages.Add("����Ĭ�ϱ���ͼ��ʧ��,���ֶ��ѡ�/max-assets/icon-emoticon/��Ŀ¼�µ������ļ�ɾ����Ȼ��ѡ�Images/Emoticons/Default/��Ŀ¼�µ�����ͼ�긴�Ƶ���/max-assets/icon-emoticon/��Ŀ¼��");
            }
        }

        public static void ProcessUsernames()
        {

            string sql = @"
IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'bx_Users') BEGIN
    SELECT UserID,Username,Email FROM bx_Users WHERE Username Like'%&%' OR Username Like'%#%';
END
ELSE
    SELECT -9999 AS UserID;
";
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {
                    Dictionary<int, string> usernames = new Dictionary<int, string>();
                    Dictionary<int, string> useremails = new Dictionary<int, string>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userID = reader.GetInt32(reader.GetOrdinal("UserID"));

                            if (userID == -9999)
                                return;

                            string userName = reader.GetString(reader.GetOrdinal("Username"));
                            string email = reader.GetString(reader.GetOrdinal("Email"));

                            string tempName = HttpUtility.HtmlDecode(userName);
                            string tempEmail = HttpUtility.HtmlDecode(email);
                            if (userName != tempName || email != tempEmail)
                            {
                                usernames.Add(userID, tempName);
                                useremails.Add(userID, tempEmail);
                            }
                        }
                    }

                    if (usernames.Count > 0)
                    {
                        sql = @"
SELECT UserID,Username FROM bx_Users;
";
                        command = new SqlCommand(sql, connection);

                        List<string> tempUsernames = new List<string>();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int userID = reader.GetInt32(reader.GetOrdinal("UserID"));
                                string userName = reader.GetString(reader.GetOrdinal("Username"));
                                if (usernames.ContainsKey(userID) == false)
                                    tempUsernames.Add(userName.ToLower());
                            }
                        }

                        //Dictionary<string, string> changeNamesLog = new Dictionary<string, string>();

                        StringBuilder sb = new StringBuilder();
                        sb.Append(@"
DECLARE @T table(uid int,uname nvarchar(50),uemail nvarchar(200));
");
                        StringBuilder sb2 = new StringBuilder();
                        foreach (KeyValuePair<int, string> pair in useremails)
                        {
                            string name = usernames[pair.Key];
                            string tempName = name;
                            GetUserName(tempUsernames, ref name);
                            if (name != tempName)
                            {
                                //changeNamesLog.Add(tempName, name);
                                sb2.AppendFormat("�û�ID��{0}��ԭ�û�����{1}�����û�����{2} <br />", pair.Key, HttpUtility.HtmlEncode(tempName), HttpUtility.HtmlEncode(name));
                            }
                            sb.AppendFormat("INSERT INTO @T (uid,uname,uemail)values({0},{1},{2});", pair.Key, name, pair.Value);
                        }

                        sb.Append(@"
UPDATE bx_Users SET Username=uname,Email=uemail FROM @T WHERE UserID=uid;
");
                        if (sb2.Length > 0)
                        {
                            sb.AppendFormat(@"
INSERT INTO bx_Announcements (AnnouncementType,PostUserID,Subject,Content,EndDate) Values(0,1,'��½��ʾ���������û��뿴����','{0}','9999-1-1');
", @"
����������bbsmax���������Զ�����<br /><br />
��������������Ҫ�������û����û����ѱ�����Ϊ���û������������û�ʹ�����û�����½������Ҫ�����û�������ϵ����Ա<br /><br />
" + sb2.ToString());
                        }

                        sql = sb.ToString();

                        command = new SqlCommand(sb.ToString(), connection);
                        command.ExecuteNonQuery();

                    }

                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("������Ա����ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        private static void GetUserName(List<string> usernames, ref string username)
        {
            if (usernames.Contains(username.ToLower()))
            {
                username += "1";
                GetUserName(usernames, ref username);
            }
        }

        public static void ConvertLinks()
        {

            string sql = @"

IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'bbsMax_Links') BEGIN
    SELECT * FROM bbsMax_Links;
END
ELSE
    SELECT -9999 AS LinkID;
";
            LinkSettings linkSetting = new LinkSettings();
            LinkCollection links = new LinkCollection();

            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {
                    bool hasCreateErrorLog = false;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int linkID = reader.GetInt32(reader.GetOrdinal("LinkID"));

                            if (linkID == -9999)
                                return;

                            Link link = new Link();
                            link.ID = linkID;
                            link.Name = reader.GetString(reader.GetOrdinal("LinkName"));
                            link.Description = reader.GetString(reader.GetOrdinal("LinkDescription"));
                            link.Url = reader.GetString(reader.GetOrdinal("Url"));


                            string imgUrl = reader.GetString(reader.GetOrdinal("LogoUrl")).Replace("\\", "/").Trim();

                            if (imgUrl.StartsWith("http://"))
                                link.ImageUrlSrc = imgUrl;
                            else
                            {
                                int i = imgUrl.LastIndexOf("/") + 1;

                                string iconName = string.Empty;
                                if (imgUrl.Length > i)
                                {
                                    iconName = imgUrl.Substring(i, imgUrl.Length - i);


                                    string targetDir = MaxLabs.bbsMax.UrlUtil.JoinUrl(MaxLabs.bbsMax.Globals.ApplicationPath, "/max-assets/logo-link/");
                                    string targetIconUrl = targetDir + iconName;

                                    imgUrl = MaxLabs.bbsMax.UrlUtil.JoinUrl(MaxLabs.bbsMax.Globals.ApplicationPath, imgUrl);
                                    if (File.Exists(imgUrl))
                                    {
                                        if (File.Exists(targetIconUrl))
                                        {
                                        }
                                        else
                                        {
                                            try
                                            {
                                                if (Directory.Exists(targetDir) == false)
                                                    Directory.CreateDirectory(targetDir);
                                                File.Move(imgUrl, targetIconUrl);
                                            }
                                            catch
                                            {
                                                if (hasCreateErrorLog == false)
                                                {
                                                    hasCreateErrorLog = true;
                                                    ErrorMessages.Add("�ƶ���������ͼ��ʧ��,���ֶ��ѡ�/Images/Logos/��Ŀ¼�µ�����ͼ�긴�Ƶ���/max-assets/logo-link/��Ŀ¼��");
                                                }
                                            }
                                        }
                                    }

                                    link.ImageUrlSrc = "~/max-assets/logo-link/" + iconName;
                                }
                                else
                                    link.ImageUrlSrc = string.Empty;

                            }

                            link.Index = reader.GetInt32(reader.GetOrdinal("SortOrder"));

                            links.Add(link);

                            if (linkSetting.MaxID < link.ID)
                                linkSetting.MaxID = link.LinkID;
                        }
                    }

                    linkSetting.Links = links;

                    sql = @"
UPDATE bx_Settings SET [Value] = @LinkString WHERE TypeName = 'MaxLabs.bbsMax.Settings.LinkSettings' AND [Key] = '*';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([Key], [Value], [TypeName]) VALUES ('*', @LinkString, 'MaxLabs.bbsMax.Settings.LinkSettings');


DROP TABLE bbsMax_Links;
";
                    command.CommandText = sql;

                    SqlParameter param = new SqlParameter("@LinkString", SqlDbType.NText);
                    param.Value = linkSetting.ToString();
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();


                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("����������������ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static void ConvertJudgements()
        {

            string sql = @"

IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'bbsMax_Judgements') BEGIN
    SELECT * FROM bbsMax_Judgements;
END
ELSE
    SELECT -9999 AS JudgementID;
";
            JudgementSettings setting = new JudgementSettings();
            JudgementCollection judgements = new JudgementCollection();

            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {
                    bool hasCreateErrorLog = false;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("JudgementID"));

                            if (id == -9999)
                                return;

                            Judgement judgement = new Judgement();
                            judgement.ID = id;
                            judgement.Description = reader.GetString(reader.GetOrdinal("JudgementDescription"));
                            //judgement.LogoUrlSrc = reader.GetString(reader.GetOrdinal("Url"));


                            string imgUrl = reader.GetString(reader.GetOrdinal("JudgementLogoUrl")).Replace("\\", "/").Trim();


                            int i = imgUrl.LastIndexOf("/") + 1;

                            string iconName = string.Empty;
                            if (imgUrl.Length > i)
                            {
                                iconName = imgUrl.Substring(i, imgUrl.Length - i);


                                string targetDir = MaxLabs.bbsMax.UrlUtil.JoinUrl(MaxLabs.bbsMax.Globals.ApplicationPath, "/max-assets/icon-judgement/");
                                string targetIconUrl = targetDir + iconName;

                                imgUrl = MaxLabs.bbsMax.UrlUtil.JoinUrl(MaxLabs.bbsMax.Globals.ApplicationPath, imgUrl);
                                if (File.Exists(imgUrl))
                                {
                                    if (File.Exists(targetIconUrl))
                                    {
                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (Directory.Exists(targetDir) == false)
                                                Directory.CreateDirectory(targetDir);
                                            File.Move(imgUrl, targetIconUrl);
                                        }
                                        catch
                                        {
                                            if (hasCreateErrorLog == false)
                                            {
                                                hasCreateErrorLog = true;
                                                ErrorMessages.Add("�ƶ��������ͼ��ʧ��,���ֶ��ѡ�/Images/Judgements/��Ŀ¼�µ�����ͼ�긴�Ƶ���/max-assets/icon-judgement/��Ŀ¼��");
                                            }
                                        }
                                    }
                                }

                                judgement.LogoUrlSrc = "~/max-assets/icon-judgement/" + iconName;
                            }
                            else
                                judgement.LogoUrlSrc = string.Empty;

                            judgements.Add(judgement);

                            if (setting.MaxId < judgement.ID)
                                setting.MaxId = judgement.ID;
                        }
                    }

                    setting.Judgements = judgements;

                    sql = @"
UPDATE bx_Settings SET [Value] = @SettingString WHERE TypeName = 'MaxLabs.bbsMax.Settings.JudgementSettings' AND [Key] = '*';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([Key], [Value], [TypeName]) VALUES ('*', @SettingString, 'MaxLabs.bbsMax.Settings.JudgementSettings');


DROP TABLE bbsMax_Judgements;
";
                    command.CommandText = sql;

                    SqlParameter param = new SqlParameter("@SettingString", SqlDbType.NText);
                    param.Value = setting.ToString();
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();


                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("�����������ͼ������ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        public static void ConvertEmailSettingsAndKeywords()
        {

            string sql = @"

IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'System_Max_Settings') BEGIN
    SELECT * FROM System_Max_Settings WHERE Catalog='EmailSetting' OR Catalog='KeywordSetting';
END
ELSE
    SELECT '' AS SettingKey;
";
            EmailSettings setting = new EmailSettings();

            EmailSendServer server = new EmailSendServer();

            setting.SendServers.Add(server);

            ContentKeywordSettings keywordsetting = new ContentKeywordSettings();

            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            string key = reader.GetString(reader.GetOrdinal("SettingKey"));

                            if (key == string.Empty)
                                return;

                            string value = reader.GetString(reader.GetOrdinal("SettingValue"));

                            switch (key.ToLower())
                            {
                                case "emailaddress":
                                    server.SenderEmail = value;
                                    break;
                                case "openemail":
                                    setting.EnableSendEmail = (value.ToLower() == "true");
                                    break;
                                case "stmpserver":
                                    server.SmtpServer = value;
                                    break;
                                case "stmpserverpassword":
                                    server.SmtpServerPassword = value;
                                    break;
                                case "stmpserverusername":
                                    server.SmtpServerAccount = value;
                                    break;
                                case "denykeywords":
                                    keywordsetting.BannedKeywords = new KeywordRegulation();
                                    keywordsetting.BannedKeywords.SetValue(value);
                                    break;
                                case "requireapprovedkeywords":
                                    keywordsetting.ApprovedKeywords = new KeywordRegulation();
                                    keywordsetting.ApprovedKeywords.SetValue(value);
                                    break;
                                case "replacekeywords":
                                    keywordsetting.ReplaceKeywords = new KeywordReplaceRegulation();
                                    keywordsetting.ReplaceKeywords.SetValue(value);
                                    break;
                                default: break;
                            }
                        }
                    }

                    server.Port = 25;

                    sql = @"
UPDATE bx_Settings SET [Value] = @SettingString WHERE TypeName = 'MaxLabs.bbsMax.Settings.EmailSettings' AND [Key] = '*';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([Key], [Value], [TypeName]) VALUES ('*', @SettingString, 'MaxLabs.bbsMax.Settings.EmailSettings');

UPDATE bx_Settings SET [Value] = @KeywordSettingString WHERE TypeName = 'MaxLabs.bbsMax.Settings.ContentKeywordSettings' AND [Key] = '*';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([Key], [Value], [TypeName]) VALUES ('*', @KeywordSettingString, 'MaxLabs.bbsMax.Settings.ContentKeywordSettings');
";
                    command.CommandText = sql;

                    SqlParameter param = new SqlParameter("@SettingString", SqlDbType.NText);
                    param.Value = setting.ToString();
                    command.Parameters.Add(param);

                    SqlParameter param2 = new SqlParameter("@KeywordSettingString", SqlDbType.NText);
                    param2.Value = keywordsetting.ToString();
                    command.Parameters.Add(param2);

                    command.ExecuteNonQuery();


                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("����Emailϵͳ���ú͹ؼ�������ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        private static string GetUpdatePointCondition(IEnumerable<string> colums)
        {
            StringBuilder condition = new StringBuilder();
            foreach (string colum in colums)
            {
                if (colum.ToLower().IndexOf("point_") == 0)
                    continue;

                condition.Append(" UPDATE ([").Append(colum).Append("]) OR");
            }
            if (condition.Length > 0)
            {
                return condition.ToString(0, condition.Length - 2);
            }
            return "1>1";
        }

        public static PointExpressionColumCollection GetGeneralPointExpressionColums()
        {
            PointExpressionColumCollection colums = new PointExpressionColumCollection();

            //KEY ����ֶ��� һ�� ���ִ�Сд
            for (int index = 1; index < 9; index++)
            {
                //int index = (int)point.Type + 1;
                colums.Add("Point_" + index, "p" + index, "");
            }
            colums.Add("TotalOnlineTime", "online", "");
            colums.Add("TotalTopics", "topics", "");
            colums.Add("TotalPosts", "posts", "");
            colums.Add("DeletedTopics", "deletedTopics", "");
            colums.Add("DeletedReplies", "deletedReplies", "");
            colums.Add("ValuedTopics", "valuedTopics", "");


            return colums;
        }

        private static string GetGeneralPointExpression(string expression)
        {
            expression = expression.ToLower().Replace("[", "").Replace("]", "");
            for (int i = 1; i < 9; i++)
            {
                expression = expression.Replace("point" + i, "p" + i);
            }
            expression = expression.Replace("threads", "topics").Replace("deletedthreads", "deletedTopics").Replace("deletedposts", "deletedReplies").Replace("valuedthreads", "valuedTopics");
            return expression;
        }

        public static void ProcessPointsExpression()
        {


            string sql = @"
SELECT [Value] FROM bx_Settings WHERE TypeName = 'MaxLabs.bbsMax.Settings.PointSettings' AND [Key] = '*';
";
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                PointSettings setting = null;
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            setting = new PointSettings();
                            setting.Parse(reader.GetString(0));
                        }
                    }
                    if (setting == null)
                        return;

                    PointExpressionColumCollection colums = GetGeneralPointExpressionColums();
                    List<string> columNames = new List<string>();

                    string expression = setting.GeneralPointExpression;

                    if (expression == string.Empty)
                        return;

                    for (int i = 0; i < colums.Count; i++)
                    {
                        string pattern = @"\b" + colums[i].FriendlyShow + @"\b";
                        Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
                        if (reg.IsMatch(expression))
                        {
                            columNames.Add(colums[i].Colum);
                            expression = reg.Replace(expression, "[" + colums[i].Colum + "]");
                            //expression = Regex.Replace(expression, pattern, "[" + colums[i].Colum + "]", RegexOptions.IgnoreCase);
                        }
                    }


                    sql = @"
ALTER TRIGGER [bx_Users_Exp_AfterUpdate]
	ON [bx_Users]
	AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

	IF (" + GetUpdatePointCondition(columNames) + @") BEGIN
	    DECLARE @MaxValue int;
	    SET @MaxValue = 2147483647;

		SET ARITHABORT OFF;
		SET ANSI_WARNINGS OFF;
		UPDATE [bx_Users] SET Points = ISNULL(" + expression + @",@MaxValue) WHERE [UserID] IN(SELECT DISTINCT [UserID] FROM [INSERTED]);
	END

	IF (UPDATE([IsActive])) BEGIN
		DECLARE @NewUserID int,@NewUsername nvarchar(50),@DeletedCount int,@InsertCount int;
		
		SELECT @InsertCount=COUNT(*) FROM [INSERTED] WHERE [IsActive]=1;
		SELECT @DeletedCount=COUNT(*) FROM [DELETED] WHERE [IsActive]=1;
		
		SELECT TOP 1 @NewUserID = UserID,@NewUsername = Username FROM [bx_Users] WITH (NOLOCK) WHERE [IsActive] = 1 ORDER BY [UserID] DESC;
		
		UPDATE [bx_Vars] SET  NewUserID = @NewUserID, NewUsername = @NewUsername, TotalUsers = TotalUsers + @InsertCount - @DeletedCount WHERE [ID]=(SELECT TOP 1 ID FROM [bx_Vars]);

		IF @@ROWCOUNT = 0 BEGIN
			DECLARE @TotalUsers int;
			SELECT @TotalUsers = COUNT(*) FROM [bx_Users] WITH (NOLOCK) WHERE [IsActive] = 1 AND [UserID]<>0;
			INSERT [bx_Vars] (NewUserID,NewUsername,TotalUsers)VALUES(@NewUserID,@NewUsername,@TotalUsers);
		END
		
		SELECT 'ResetVars' AS XCMD;
	END

END
";
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();


                    sql = @"
ALTER PROCEDURE bx_UpdateUserGeneralPoint
     @UserID       int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MaxValue int;
	SET @MaxValue = 2147483647;
	SET ARITHABORT OFF;
	SET ANSI_WARNINGS OFF;
		
    UPDATE bx_Users SET Points = ISNULL(" + expression + @",@MaxValue) WHERE [UserID] = @UserID;
END
";

                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();


                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    ErrorMessages.Add("�����ܻ���ʧ��,������̨�ֶ��޸��ܻ��ֹ�ʽ��ԭ��ʽ��" + setting.GeneralPointExpression + "��");
                    //throw new Exception("�����ܻ��ֵĴ������ʹ洢����ʧ��"+ "(��ʽ" + setting.GeneralPointExpression + ")" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }


        }




        /// <summary>
        /// ����������� ��
        /// </summary>
        public static void ProcessSetting()
        {
            string sql = @"
SELECT [ForumID],[ParentID],[ExtendedAttributes] FROM bx_Forums;
SELECT [Value] FROM bx_Settings WHERE TypeName = 'MaxLabs.bbsMax.Settings.ForumSettings' AND [Key] = '*';
";
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 60;
                try
                {
                    Dictionary<int, StringTable> forumsExtendedDatas = new Dictionary<int, StringTable>();
                    ForumSettings forumSetting = new ForumSettings();
                    List<TempForum> forums = new List<TempForum>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int forumID = reader.GetInt32(0);
                            TempForum forum = new TempForum();
                            forum.ForumID = forumID;
                            forum.ParentID = reader.GetInt32(1);

                            forums.Add(forum);
                            try
                            {
                                if (reader.IsDBNull(2) == false)
                                {
                                    StringTable st = SettingBase.ParseStringTable(reader.GetString(2));
                                    forumsExtendedDatas.Add(forumID, st);
                                }
                            }
                            catch
                            { }

                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                forumSetting = new ForumSettings();
                                forumSetting.Parse(reader.GetString(0));
                            }
                        }
                    }
                    if (forumsExtendedDatas.Count == 0)
                        return;

                    StringBuilder sbSql = new StringBuilder();
                    ForumSettings tempForumSettings = new ForumSettings();

                    ForumSettingItem temp0Item = forumSetting.Items.GetForumSettingItem(0);
                    tempForumSettings.Items.Add(temp0Item);

                    foreach (KeyValuePair<int, StringTable> pair in forumsExtendedDatas)
                    {
                        string allowGuestVisitForumValue = null;
                        string displayInListForGuestValue = null;
                        string visitForumValue = null;
                        string displayInListValue = null;

                        if (pair.Value.ContainsKey("AllowGuestVisitForum"))
                        {
                            allowGuestVisitForumValue = pair.Value["AllowGuestVisitForum"];
                            pair.Value.Remove("AllowGuestVisitForum");
                        }
                        if (pair.Value.ContainsKey("DisplayInListForGuest"))
                        {
                            displayInListForGuestValue = pair.Value["DisplayInListForGuest"];
                            pair.Value.Remove("DisplayInListForGuest");
                        }
                        if (pair.Value.ContainsKey("VisitForum"))
                        {
                            visitForumValue = pair.Value["VisitForum"];
                            pair.Value.Remove("VisitForum");
                        }
                        if (pair.Value.ContainsKey("DisplayInList"))
                        {
                            displayInListValue = pair.Value["DisplayInList"];
                            pair.Value.Remove("DisplayInList");
                        }

                        ForumSettingItem fSetting = null;

                        bool? isNew = null;

                        fSetting = GetForumSettingItem(forums, pair.Key, forumSetting, ref isNew);

                        if (allowGuestVisitForumValue == null && displayInListForGuestValue == null
                            && visitForumValue == null && displayInListValue == null)
                        {
                            if (isNew.Value == false)
                            {
                                tempForumSettings.Items.Add(fSetting);
                            }
                            continue;
                        }


                        //bool mustAddNew = false;
                        if (allowGuestVisitForumValue != null)
                        {
                            if (fSetting.AllowGuestVisitForum.ToString().ToLower() != allowGuestVisitForumValue.ToLower())
                            {
                                fSetting.AllowGuestVisitForum = (allowGuestVisitForumValue.ToLower() == "true");
                                //mustAddNew = true;
                            }
                        }
                        if (visitForumValue != null)
                        {
                            if (fSetting.VisitForum.GetValue() != visitForumValue)
                            {
                                fSetting.VisitForum = new Exceptable<bool>(true);
                                fSetting.VisitForum.SetValue(visitForumValue);
                                //mustAddNew = true;
                            }
                        }
                        if (displayInListForGuestValue != null)
                        {
                            if (fSetting.DisplayInListForGuest.ToString().ToLower() != displayInListForGuestValue.ToLower())
                            {
                                fSetting.DisplayInListForGuest = (displayInListForGuestValue.ToLower() == "true");
                                //mustAddNew = true;
                            }
                        }
                        if (displayInListValue != null)
                        {
                            if (fSetting.DisplayInList.GetValue() != displayInListValue)
                            {
                                fSetting.DisplayInList = new Exceptable<bool>(true);
                                fSetting.DisplayInList.SetValue(displayInListValue);
                                //mustAddNew = true;
                            }
                        }

                        //if (isNew.Value && mustAddNew)
                        //    forumSetting.Items.Add(fSetting);

                        fSetting.ForumID = pair.Key;
                        tempForumSettings.Items.Add(fSetting);

                        sbSql.AppendFormat("UPDATE bx_Forums SET ExtendedAttributes = '{0}' WHERE ForumID = {1};", SettingBase.FormatStringTable(pair.Value), pair.Key);

                    }

                    ForumSettings resultSettings = new ForumSettings();
                    foreach (ForumSettingItem item in tempForumSettings.Items)
                    {
                        bool? isParentSetting = null;
                        ForumSettingItem temp = GetForumSettingItem(forums, item.ForumID, forumSetting, ref isParentSetting);

                        if (isParentSetting.Value)//����Ǹ�����һ����  ��ʹ�ü̳е�
                        {
                            if (temp.ToString() == item.ToString())
                            {
                                continue;
                            }
                        }

                        resultSettings.Items.Add(item);
                    }

                    sbSql.AppendFormat(@"
IF EXISTS(SELECT * FROM bx_Settings WHERE TypeName = 'MaxLabs.bbsMax.Settings.ForumSettings' AND [Key] = '*')
    UPDATE bx_Settings SET [Value] = '{0}' WHERE TypeName = 'MaxLabs.bbsMax.Settings.ForumSettings' AND [Key] = '*';
ELSE
    INSERT INTO bx_Settings ([Value],[TypeName],[Key]) VALUES('{0}','MaxLabs.bbsMax.Settings.ForumSettings','*');
"
                        , resultSettings.ToString());

                    command.CommandText = sbSql.ToString();
                    sql = command.CommandText;

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("�����������ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private static ForumSettingItem GetForumSettingItem(List<TempForum> forums, int forumID, ForumSettings forumSettings, ref bool? isParentSetting)
        {
            foreach (ForumSettingItem item in forumSettings.Items)
            {
                if (item.ForumID == forumID)
                {
                    if (isParentSetting == null)
                    {
                        isParentSetting = false;
                    }
                    return item;
                }
            }

            isParentSetting = true;

            foreach (TempForum temp in forums)
            {
                if (temp.ForumID == forumID)
                {
                    return SettingManager.CloneSetttings<ForumSettingItem>(GetForumSettingItem(forums, temp.ParentID, forumSettings, ref isParentSetting));
                }
            }

            return SettingManager.CloneSetttings<ForumSettingItem>(GetForumSettingItem(forums, 0, forumSettings, ref isParentSetting));
        }





        public static void ConvertMedals()
        {

            MedalSettings medalSetting = new MedalSettings();
            MedalCollection medals = new MedalCollection();

            string sql = @"
IF EXISTS(SELECT * FROM sysobjects WHERE [type]=N'TR' AND [name]=N'bx_UserMedals_AfterUpdate')
	DROP TRIGGER bx_UserMedals_AfterUpdate;

--GO
";
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("ɾ��������bx_UserMedals_AfterUpdateʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }



            sql = @"

IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'Max_Medals') AND EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_UserMedals') BEGIN
    SELECT * FROM Max_Medals;
END
ELSE
    SELECT -9999 AS MedalID;
";
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {
                    bool hasCreateErrorLog = false;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int medalID = reader.GetInt32(reader.GetOrdinal("MedalID"));

                            if (medalID == -9999)
                                return;

                            Medal tempMedal = new Medal();
                            tempMedal.Condition = string.Empty;
                            tempMedal.Enable = reader.GetBoolean(reader.GetOrdinal("IsEnabled"));
                            tempMedal.IsCustom = true;
                            tempMedal.ID = medalID;
                            tempMedal.MaxLevelID = 1;
                            tempMedal.Name = reader.GetString(reader.GetOrdinal("MedalName"));
                            tempMedal.SortOrder = 0;

                            tempMedal.Levels = new MedalLevelCollection();
                            MedalLevel tempLevel = new MedalLevel();
                            tempLevel.Condition = string.Empty;

                            string iconUrl = reader.GetString(reader.GetOrdinal("LogoUrl")).Replace("\\", "/");

                            int i = iconUrl.LastIndexOf("/") + 1;

                            string iconName = string.Empty;
                            if (iconUrl.Length > i)
                                iconName = iconUrl.Substring(i, iconUrl.Length - i);
                            else
                                continue;

                            string targetDir = MaxLabs.bbsMax.UrlUtil.JoinUrl(MaxLabs.bbsMax.Globals.ApplicationPath, "/max-assets/icon-medal/");
                            string targetIconUrl = targetDir + iconName;

                            iconUrl = MaxLabs.bbsMax.UrlUtil.JoinUrl(MaxLabs.bbsMax.Globals.ApplicationPath, iconUrl);
                            if (File.Exists(iconUrl))
                            {
                                if (File.Exists(targetIconUrl))
                                {
                                }
                                else
                                {
                                    try
                                    {
                                        if (Directory.Exists(targetDir) == false)
                                            Directory.CreateDirectory(targetDir);
                                        File.Move(iconUrl, targetIconUrl);
                                    }
                                    catch
                                    {
                                        if (hasCreateErrorLog == false)
                                        {
                                            hasCreateErrorLog = true;
                                            ErrorMessages.Add("�ƶ�ѫ��ͼ��ʧ��,���ֶ��ѡ�/Images/Medals/��Ŀ¼�µ�����ͼ�긴�Ƶ���/max-assets/icon-medal/��Ŀ¼��");
                                        }
                                    }
                                }
                            }

                            tempLevel.IconSrc = "~/max-assets/icon-medal/" + iconName;

                            tempLevel.Name = string.Empty;
                            tempLevel.ID = 1;
                            tempMedal.Levels.Add(tempLevel);

                            medals.Add(tempMedal);

                            if (medalSetting.MaxMedalID < tempMedal.ID)
                                medalSetting.MaxMedalID = tempMedal.ID;
                        }
                    }

                    foreach (Medal medal in new MedalSettings().Medals)
                    {
                        medal.ID = medalSetting.MaxMedalID + 1;
                        medals.Add(medal);
                        medalSetting.MaxMedalID += 1;
                    }

                    medalSetting.Medals = medals;


                    sql = @"
UPDATE bx_Settings SET [Value] = @MedalString WHERE TypeName = 'MaxLabs.bbsMax.Settings.MedalSettings' AND [Key] = '*';
IF @@ROWCOUNT = 0
    INSERT INTO bx_Settings ([Key], [Value], [TypeName]) VALUES ('*', @MedalString, 'MaxLabs.bbsMax.Settings.MedalSettings');

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_UserMedals') BEGIN
	truncate table [bx_UserMedals];
	INSERT INTO [bx_UserMedals]
           ([UserID]
           ,[MedalID]
           ,[MedalLevelID]
           ,[EndDate]
           ,[CreateDate])
     SELECT 
           UserID
           ,MedalID
           ,1
           ,ExpiresDate
           ,CreateDate
		FROM Max_UserMedals WITH (NOLOCK);

	DROP TABLE Max_UserMedals;
END

DROP TABLE Max_Medals;
";

                    command.CommandText = sql;
                    command.CommandTimeout = 3600;
                    //command.Parameters.AddWithValue("@MedalString", medalSetting.ToString());


                    SqlParameter param = new SqlParameter("@MedalString", SqlDbType.NText);
                    param.Value = medalSetting.ToString();
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                    //AllSettings.Current.MedalSettings = medalSetting;

                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("����ѫ������ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }


            }
        }

        public static void ConvertForumLogos()
        {
            string sql = @"

IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'bx_Forums') BEGIN
    SELECT * FROM bx_Forums;
END
ELSE
    SELECT -9999 AS ForumID;
";

            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {
                    bool hasCreateErrorLog = false;
                    Dictionary<int, string> logos = new Dictionary<int, string>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int forumID = reader.GetInt32(reader.GetOrdinal("ForumID"));

                            if (forumID == -9999)
                                return;

                            string logoUrl = reader.GetString(reader.GetOrdinal("LogoSrc")).Replace("\\", "/");

                            if (logoUrl == string.Empty)
                                continue;

                            int i = logoUrl.LastIndexOf("/") + 1;

                            string iconName = string.Empty;
                            if (logoUrl.Length > i)
                                iconName = logoUrl.Substring(i, logoUrl.Length - i);
                            else
                                continue;

                            string targetDir = MaxLabs.bbsMax.UrlUtil.JoinUrl(MaxLabs.bbsMax.Globals.ApplicationPath, "/max-assets/logo-forum/");
                            string targetIconUrl = targetDir + iconName;

                            logoUrl = MaxLabs.bbsMax.UrlUtil.JoinUrl(MaxLabs.bbsMax.Globals.ApplicationPath, logoUrl);
                            if (File.Exists(logoUrl))
                            {
                                if (File.Exists(targetIconUrl))
                                {
                                }
                                else
                                {
                                    try
                                    {
                                        if (Directory.Exists(targetDir) == false)
                                            Directory.CreateDirectory(targetDir);
                                        File.Move(logoUrl, targetIconUrl);
                                    }
                                    catch
                                    {
                                        if (hasCreateErrorLog == false)
                                        {
                                            hasCreateErrorLog = true;
                                            ErrorMessages.Add("�������LOGOͼ��ʧ��,���ֶ��ѡ�/Images/ForumLogos/��Ŀ¼�µ�����ͼ�긴�Ƶ���/max-assets/logo-forum/��Ŀ¼��");
                                        }
                                    }
                                }
                            }

                            string logoSrc = "~/max-assets/logo-forum/" + iconName;

                            if (logos.ContainsKey(forumID) == false)
                            {
                                logos.Add(forumID, logoSrc);
                            }
                        }
                    }

                    StringBuilder sqlString = new StringBuilder();
                    foreach (KeyValuePair<int, string> logo in logos)
                    {
                        sqlString.Append("UPDATE bx_Forums SET LogoSrc = '" + logo.Value + "' WHERE ForumID = " + logo.Key + ";");

                    }
                    if (sqlString.Length > 0)
                    {
                        sql = sqlString.ToString();
                        command.CommandText = sql;
                        command.CommandTimeout = int.MaxValue;
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("���°��ͼ������ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static void UpdateUserInfos()
        {
            /*
            string sql = @"
UPDATE bx_Users
	   SET UserInfo =
						+ CAST(T.[InviterID] AS varchar(10)) + '|'			--0
						
						+ CAST(T.[TotalFriends] AS varchar(10)) + '|'		--1
						
						+ CAST(T.[UnreadMessages] AS varchar(10)) + '|'		--2
						+ CAST(T.[UnreadBoardNotifies] AS varchar(10)) + '|'
						+ CAST(T.[UnreadPostNotifies] AS varchar(10)) + '|'
						+ CAST(T.[UnreadGroupInviteNotifies] AS varchar(10)) + '|'
						+ CAST(T.[UnreadFriendNotifies] AS varchar(10)) + '|'--6
						
						+ CAST(T.[UnreadHailNotifies] AS varchar(10)) + '|'	--7
						+ CAST(T.[UnreadAppInviteNotifies] AS varchar(10)) + '|'	--8
						+ CAST(T.[UnreadAppActionNotifies] AS varchar(10)) + '|'
						+ CAST(T.[UnreadBidUpNotifies] AS varchar(10)) + '|'
						+ CAST(T.[UnreadBirthdayNotifies] AS varchar(10)) + '|'		--11
						
						+ CAST(T.[LastSystemMessageID] AS varchar(10)) + '|'		--12
						+ CAST(T.[UsedAlbumSize] AS varchar(22)) + '|'				--13
						+ CAST(T.[AddedAlbumSize] AS varchar(22)) + '|'
						+ ISNULL( CAST(T.[TimeZone] AS varchar(10)) ,'')+ '|'				--15
						
						+ CAST(T.[Birthday] AS varchar(4)) + '|'
						+ CAST(T.[BirthYear] AS varchar(4)) + '|'					--17
						
						+ CAST(T.[BlogPrivacy] AS varchar(10)) + '|'				--18
						+ CAST(T.[FeedPrivacy] AS varchar(10)) + '|'
						+ CAST(T.[BoardPrivacy] AS varchar(10)) + '|'
						+ CAST(T.[DoingPrivacy] AS varchar(10)) + '|'
						+ CAST(T.[AlbumPrivacy] AS varchar(10)) + '|'
						+ CAST(T.[SpacePrivacy] AS varchar(10)) + '|'
						+ CAST(T.[SharePrivacy] AS varchar(10)) + '|'
						+ CAST(T.[FriendListPrivacy] AS varchar(10)) + '|'
						+ CAST(T.[InformationPrivacy] AS varchar(10)) + '|'			   --26
						+ CAST(T.[EverNameChecked] AS varchar(1)) +'|'                 --27
						+ CAST(T.[EverAvatarChecked] AS varchar(1)) +'|'               --28
						+ CAST(T.[EnableDisplaySidebar] AS varchar(2)) + '|'				--29	
						+ CAST(T.[OnlineStatus] AS varchar(2))	+ '|'			--30	        
						+ CAST(T.[NotifySetting] AS varchar(4000))	+ '|'			--31
						+ CAST(T.[SkinID] AS nvarchar(256))   + '|'                 --32
						+ CAST(T.UnreadSystemNotifies AS varchar(4)) + '|'          --33
						+ CAST(T.[LastReadSystemNotifyID] AS varchar(8)) + '|'      --34
						+ CAST(T.[UnreadPropNotifies] AS varchar(10))		--35
	  FROM [bx_UserInfos] T WHERE T.UserID = bx_Users.UserID;
";

            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = int.MaxValue;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("�����û���Ϣʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }

            */
        }




        public static void ProcessUserExtendData()
        {
            string sql = @"

IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'bx_UserRoles')  BEGIN
    SELECT * FROM bx_UserRoles;
END
ELSE
    SELECT -9999 AS UserID;

IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'bx_UserMedals')  BEGIN
    SELECT * FROM bx_UserMedals;
END
ELSE
    SELECT -9999 AS UserID;


IF EXISTS (SELECT * FROM sysobjects WHERE [type] = N'U' AND [name] = N'bx_UserExtendedValues')  BEGIN
    SELECT UserID, ExtendedFieldID AS FieldID, Value FROM bx_UserExtendedValues
END
ELSE
    SELECT -9999 AS UserID;

";

            Dictionary<int, string> datas = new Dictionary<int, string>();
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 60;
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userID = reader.GetInt32(reader.GetOrdinal("UserID"));

                            if (userID == -9999)
                                break;

                            Guid roleID = reader.GetGuid(reader.GetOrdinal("RoleID"));
                            DateTime beginDate = reader.GetDateTime(reader.GetOrdinal("BeginDate"));
                            DateTime endDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));

                            string userRoleString = "R" + roleID.ToString().Length
                                + "," + beginDate.ToString().Length
                                + "," + endDate.ToString().Length
                                + ":" + roleID.ToString()
                                + beginDate.ToString()
                                + endDate.ToString();

                            string str;

                            if (datas.TryGetValue(userID, out str))
                            {
                                str = str + userRoleString;
                                datas[userID] = str;
                            }
                            else
                                datas.Add(userID, userRoleString);
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                int userID = reader.GetInt32(reader.GetOrdinal("UserID"));

                                if (userID == -9999)
                                    break;

                                int medalID = reader.GetInt32(reader.GetOrdinal("MedalID"));
                                int medalLevelID = reader.GetInt32(reader.GetOrdinal("MedalLevelID"));
                                DateTime endDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                                DateTime createDate = reader.GetDateTime(reader.GetOrdinal("CreateDate"));

                                
                                string url;

                                try
                                {
                                    if (reader.IsDBNull(reader.GetOrdinal("Url")))
                                        url = string.Empty;
                                    else
                                        url = reader.GetString(reader.GetOrdinal("Url"));
                                }
                                catch
                                {
                                    url = string.Empty;
                                }

                                string medalString = "M" + medalID.ToString().Length
                                    + "," + medalLevelID.ToString().Length
                                    + "," + endDate.ToString().Length
                                    + "," + createDate.ToString().Length
                                    + "," + url.Length
                                    + ":" + medalID.ToString()
                                    + medalLevelID.ToString()
                                    + endDate.ToString()
                                    + createDate.ToString()
                                    + url;

                                string str;

                                if (datas.TryGetValue(userID, out str))
                                {
                                    str = str + medalString;
                                    datas[userID] = str;
                                }
                                else
                                    datas.Add(userID, medalString);
                            }
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                int userID = reader.GetInt32(reader.GetOrdinal("UserID"));

                                if (userID == -9999)
                                    break;

                                string fieldID = reader.GetString(reader.GetOrdinal("FieldID"));
                                string valueString = reader.GetString(reader.GetOrdinal("Value")).Trim();

                                string extendFieldString = "F" + fieldID.Length
                                    + "," + valueString.Length
                                    + ":" + fieldID
                                    + valueString;

                                string str;

                                if (datas.TryGetValue(userID, out str))
                                {
                                    str = str + extendFieldString;
                                    datas[userID] = str;
                                }
                                else
                                    datas.Add(userID, extendFieldString);
                            }
                        }
                    }

                    int index = 0;
                    int total = datas.Count;

                    StringBuilder sqlString = new StringBuilder();
                    foreach (KeyValuePair<int, string> data in datas)
                    {
                        if (sqlString.Length == 0)
                        {
                            sqlString.Append(@"
DECLARE @DataTable table(UID int,DataStr ntext);
");
                        }

                        sqlString.Append("INSERT INTO @DataTable(UID,DataStr) VALUES (" + data.Key + ",'" + data.Value.Replace("'", "''") + "');");

                        //ÿ300������һ��
                        if ((index != 0 && index % 300 == 0) || index == (total - 1))
                        {
                            sqlString.Append("UPDATE bx_Users SET ExtendedData = DataStr FROM @DataTable WHERE UserID = UID;");
                            sql = sqlString.ToString();

                            command.CommandText = sqlString.ToString();
                            command.CommandTimeout = int.MaxValue;
                            command.ExecuteNonQuery();

                            sqlString = new StringBuilder();
                        }

                        index++;
                    }

                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("�����û���չ��Ϣʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }


        }

        internal static string GetSrcAvatarPath(int userID, string stringInDatabase)
        {
            string avatarBasePath = MaxLabs.bbsMax.IOUtil.JoinPath(Globals.RootPath(), "UserFiles\\Avatars");

            return MaxLabs.bbsMax.IOUtil.JoinPath(avatarBasePath, GetAvatarFilename(userID, stringInDatabase));
        }

        internal static string GetAvatarFilename(int userID, string stringInDatabase)
        {
            string userIDStr = userID.ToString();

            int index = stringInDatabase.LastIndexOf(".");

            string ext = string.Empty;

            if (index >= 0 && stringInDatabase.Length > index)
            {
                ext = stringInDatabase.Substring(index, stringInDatabase.Length - index);
            }

            string avatarPath;

            if (userIDStr.Length > 3)
            {
                avatarPath = userIDStr[0].ToString() + "/" + userIDStr[1].ToString() + "/" + userIDStr[2].ToString() + "/" + userIDStr + ext;
            }
            else if (userIDStr.Length > 2)
            {
                avatarPath = userIDStr[0].ToString() + "/" + userIDStr[1].ToString() + "/" + userIDStr + ext;
            }
            else
            {
                avatarPath = userIDStr[0].ToString() + "/" + userIDStr + ext;
            }

            return avatarPath;
        }

        public static void ConvertAvatars(InstallEventHandler onProcess)
        {
            string rootPath = Globals.RootPath();

            AvatarBuilder.SetAvatarDirectory(rootPath + "UserFiles\\Avatar");

            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();

            //�����ο͡�everyone
            string sql = @"SELECT UserID,AvatarSrc FROM bx_Users WITH (NOLOCK) WHERE AvatarSrc LIKE 'upload://.%'";
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 600;
                connection.Open();
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userID = (int)reader["UserID"];
                            string avatarSrc = (string)reader["AvatarSrc"];


                            list.Add(new KeyValuePair<int, string>(userID, avatarSrc));
                        }
                    }
                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("����ͷ������ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }

            int count = list.Count;
            if (count > 0)
            {
                int i = 0;
                foreach (KeyValuePair<int, string> item in list)
                {
                    i++;

                    int percent = 1;
                    if (i == count - 1)
                        percent = 100;
                    else
                    {
                        float a = (float)i / (float)count;


                        string b = a.ToString();
                        if (b.Length >= 4)
                            percent = int.Parse(b.Substring(2, 2));
                        else if (b.Length >= 3)
                            percent = int.Parse(b.Substring(2, 1));
                        else
                            percent = 100;
                    }

                    onProcess(percent, "�Ѵ���" + i + "��ͷ��");

                    try
                    {
                        string path = GetSrcAvatarPath(item.Key, item.Value);
                        if (File.Exists(path))
                        {
                            AvatarBuilder.GenerateAvatar(item.Key, item.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        CreateLog(ex.Message);
                    }
                }

//                sql = @"
//UPDATE bx_Users SET AvatarSrc = '' WHERE AvatarSrc NOT LIKE 'upload://.%'
//UPDATE bx_Users SET AvatarSrc = SUBSTRING(AvatarSrc, 11, 5) WHERE AvatarSrc LIKE 'upload://.%'";
//                using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
//                {
//                    connection.Open();
//                    SqlCommand command = new SqlCommand(sql, connection);
//                    command.CommandTimeout = 3600;
//                    command.ExecuteNonQuery();
//                }
            }

        }




        public static void ProcessAvatars()
        {
            //�����ο͡�everyone
            string sql = @"SELECT UserID,AvatarSrc FROM bx_Users WITH (NOLOCK) WHERE AvatarSrc <> '' ORDER BY UserID ASC;";

            Dictionary<int, string> avatars = new Dictionary<int, string>();
            using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 600;
                connection.Open();
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userID = (int)reader["UserID"];
                            string avatarSrc = (string)reader["AvatarSrc"];
                            string ext = avatarSrc.ToLower().Trim();

                            if (ext.StartsWith("~") || ext.StartsWith("/") || ext.StartsWith("\\"))
                                continue;



                            if(ext.EndsWith("jpg"))
                                ext = "jpg";
                            else if(ext.EndsWith("jpeg"))
                                ext = "jpeg";
                            else if(ext.EndsWith("gif"))
                                ext = "gif";
                            else if(ext.EndsWith("png"))
                                ext = "png";
                            else if(ext.EndsWith("bmp"))
                                ext = "bmp";
                            else
                            {
                                continue;
                            }

                            //===================
                            string userID2 = userID.ToString();
                            string extend = ext;

                            string newAva = "";
                            //12
                            if (userID > 9)
                            {
                                StringBuilder newSrc = new StringBuilder();
                                int i = 0;
                                char[] chars = userID2.ToCharArray();

                                foreach (char c in chars)
                                {
                                    if (i < 2)
                                    {
                                        newSrc.Append("/" + c.ToString());
                                    }
                                    i++;
                                }

                                newAva = newSrc.ToString();
                            }
                            else
                                newAva = "/0/" + userID2;

                            newAva = newAva + "/" + userID2 + "." + extend;

                            if ("/"+avatarSrc.ToLower() == newAva.ToLower())
                                continue;
                            //=======================

                            avatars.Add(userID, ext);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CreateLog(ex);
                    throw new Exception("����ͷ������ʧ��" + ex.Message + sql);
                }
                finally
                {
                    connection.Close();
                }
            }

            if (avatars.Count > 0)
            {
                StringBuilder sqlText = new StringBuilder();
                foreach (KeyValuePair<int, string> pair in avatars)
                {
                    string userID2 = pair.Key.ToString();
                    string extend = pair.Value;

                    string oldAva = "";
                    string newAva = "";
                    //12
                    if (pair.Key > 9)
                    {
                        StringBuilder oldSrc = new StringBuilder();
                        StringBuilder newSrc = new StringBuilder();
                        int i = 0;
                        char[] chars = userID2.ToCharArray();

                        foreach (char c in chars)
                        {
                            if (i < chars.Length && i < 3)
                            {
                                oldSrc.Append("/" + c.ToString());
                            }

                            if (i < 2)
                            {
                                newSrc.Append("/" + c.ToString());
                            }
                            i++;
                        }

                        newAva = newSrc.ToString();
                        oldAva = oldSrc.ToString();
                    }
                    else
                        newAva = "/0/" + userID2;

                    oldAva = oldAva + "/" + userID2 + "." + extend;
                    newAva = newAva + "/" + userID2 + "." + extend;


                    bool has;
                    try
                    {
                        has = CopyFile("UserFiles/Avatar/Default" + oldAva, "UserFiles/A/D" + newAva);
                        has = CopyFile("UserFiles/Avatar/Big" + oldAva, "UserFiles/A/B" + newAva);
                        has = CopyFile("UserFiles/Avatar/Small" + oldAva, "UserFiles/A/S" + newAva);
                    }
                    catch (Exception ex)
                    {
                        CreateLog(ex.Message);
                        has = false;
                    }

                    if (has == true)
                    {
                        sqlText.AppendFormat(@"
UPDATE bx_Users SET AvatarSrc = '{0}' WHERE UserID = {1};
", newAva.Substring(1), userID2);

                    }
                    else
                    {
                        sqlText.AppendFormat(@"
UPDATE bx_Users SET AvatarSrc = '' WHERE UserID = {0};
", userID2);
                    }

                }

                using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlText.ToString(), connection);
                    command.CommandTimeout = 3600;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        CreateLog(ex.Message);
                        return;
                    }
                }
            }
        }

        private static bool CopyFile(string src, string target)
        {
            src = MaxLabs.bbsMax.IOUtil.JoinPath(Globals.RootPath(), src);
            target = MaxLabs.bbsMax.IOUtil.JoinPath(Globals.RootPath(), target);
            if (File.Exists(src))
            {
                target = target.Replace("/", "\\");
                string dir = target.Substring(0, target.LastIndexOf("\\"));
                if (Directory.Exists(dir) == false)
                    Directory.CreateDirectory(dir);

                File.Copy(src, target, true);

                return true;
            }

            return false;
        }

        #endregion

        public static void CreateLog(string msg)
        {
            try
            {
                string filepath = Globals.RootPath() + "InstallLog.txt";
                CanelReadOnly(filepath);
                File.AppendAllText(filepath, msg + "\r\nTime:" + DateTime.Now.ToString() + "\r\n----------------------------------------------------------------------------------\r\n");
            }
            catch
            { }
        }

        public static void CreateLog(Exception ex)
        {

            string filepath = Globals.RootPath() + "InstallLog.txt";

            StreamWriter streamWriter = null;


            try
            {
                streamWriter = new StreamWriter(filepath, true, Encoding.UTF8);
            }
            catch
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
                return;
            }


            streamWriter.WriteLine("Message:" + ex.Message);

            try
            {
                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;

                    string userVar = request.RawUrl;
                    streamWriter.WriteLine("Path:" + userVar);

                    if (request.UrlReferrer != null)
                    {
                        userVar = request.UrlReferrer.ToString();
                        streamWriter.WriteLine("Referrer:" + userVar);
                    }

                    userVar = System.Web.HttpContext.Current.Request.UserAgent;
                    streamWriter.WriteLine("UserAgent:" + userVar);

                    userVar = request.UserHostAddress;
                    streamWriter.WriteLine("Address:" + userVar);

                    if (request.Cookies.Count > 0)
                        streamWriter.WriteLine("Cookie:");
                    for (int i = 0; i < request.Cookies.Count; i++)
                    {
                        System.Web.HttpCookie cookie = request.Cookies[i];
                        streamWriter.WriteLine("    name:" + cookie.Name + "    value:" + cookie.Value);
                    }

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < HttpContext.Current.Request.Form.Count; i++)
                    {
                        if (i > 0)
                            sb.Append("&");
                        sb.Append(request.Form.GetKey(i));
                        sb.Append("=");
                        sb.Append(request.Form[i].ToString());

                    }
                    streamWriter.WriteLine("PostData:" + sb.ToString());
                }
            }
            catch
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
                return;
            }

            streamWriter.WriteLine("Source:" + ex.Source);
            streamWriter.WriteLine("StackTrace:");
            streamWriter.WriteLine(ex.StackTrace);
            streamWriter.WriteLine("Method:" + ex.TargetSite.Name);
            streamWriter.WriteLine("Class:" + ex.TargetSite.DeclaringType.FullName);
            streamWriter.WriteLine("Time:" + DateTime.Now.ToString());
            streamWriter.WriteLine("----------------------------------------------------------------------------------");
            try
            {
                if (streamWriter != null)
                {
                    streamWriter.Flush();
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
            catch { }
        }
    }
}