//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
#if SQLSERVER
using System.Data.SqlClient;
#endif
#if SQLITE
using System.Data.SQLite;
#endif
using Max.Installs;

namespace Max.Installs
{
    /// <summary>
    /// ���Ӳ��ԡ�
    /// </summary>
    public class ConnectionTester
    {
        private ConnectionTester()
        {
        }
#if SQLSERVER

        //public static void DatabaseExists(string databaseName)
        //{
        //    string dbName = Settings.Current.IDatabase;
        //    string sql = "SELECT @State = COUNT(*) FROM master..sysdatabases WHERE [name]='" + dbName + "';";

        //    using (SqlConnection connection = new SqlConnection(Settings.Current.IMasterConnectionString + "Pooling=false;"))
        //    {
        //        SqlCommand command = new SqlCommand(sql, connection);
        //        connection.Open();
        //        command.ExecuteNonQuery();
        //        connection.Close();
        //    }
        //}


        /// <summary>
        /// ����bbsMax���ݿ�������Ƿ�������ӳɹ���
        /// </summary>
        /// <returns>���ش�����Ϣ��null��</returns>
        public static string Check()
        { 
            string result = string.Empty;
            string dbName = Settings.Current.IDatabase;
            try
            {
                using (SqlConnection connection = new SqlConnection(Settings.Current.IMasterConnectionString + "Pooling=false;"))
                {
                    connection.Open();
                    string sql = @"IF DB_ID(N'" + dbName + @"') IS NULL
    SELECT 1;
ELSE IF DATABASEPROPERTYEX(N'" + dbName + @"','Status') <> N'ONLINE'
    SELECT 2;
ELSE
    SELECT 0";

                    SqlCommand command = new SqlCommand(sql, connection);
                    int state = (int)command.ExecuteScalar();

                    if (state == 1)
                        return "���ݿ� " + dbName + " ������";
                    else if (state == 2)
                        return "���ݿ� " + dbName + " �����Ѿ���";
                }

                using (SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString + "Pooling=false;"))
                {
                    connection.Open();
                }
            }
            catch(Exception e)
            {
                result = e.Message;
            }
            return result;
        }

        /// <summary>
        /// �ж����ݿ���bbsmax��صı���Ѿ�����
        /// </summary>
        /// <returns></returns>
        public static bool IsMaxExists()
        {
            int i = 0;
            SqlConnection connection = new SqlConnection(Settings.Current.IConnectionString + "Pooling=false;");
                if (ConnectionState.Closed == connection.State)
                    connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT COUNT(Db_ID('"+Settings.Current.IDatabase+"'));";
                if (Globals.ToInt32(cmd.ExecuteScalar()) == 1)
                {
                    connection.Close();
                    connection.ConnectionString = Settings.Current.IConnectionString;
                    connection.Open();
                    cmd.CommandText = "SELECT COUNT(*) FROM  sysobjects  WHERE id = OBJECT_ID('bbsMax_Posts') OR id = OBJECT_ID('bx_Posts');";
                    i = Globals.ToInt32(cmd.ExecuteScalar(), 0);
                }
            return i > 0;
        }
#endif
        public static string GetUpgradeOption()
        {
            string upgradeOption;
#if SQLSERVER
            if (IsMaxExists())
#endif
#if SQLITE
            if (IsSqlLiteMaxExists())
#endif
            {
                if (Settings.Version == Max.Installs.SetupManager.GetCurrentVersion())
                    upgradeOption = "~�޸���װ(ǿ�ҽ����ȱ������ݿ�) - �汾��" + Settings.Version;
                else
                {
                    bool canUpgrade = false;
                    foreach (string version in Settings.Versions)
                    {
                        if (Max.Installs.SetupManager.GetCurrentVersion() == version)
                        {
                            canUpgrade = true;
                            break;
                        }
                    }
                    if (canUpgrade)
                        upgradeOption = "~������װ(ǿ�ҽ����ȱ������ݿ�) - �汾��" + Max.Installs.SetupManager.GetCurrentVersion() + " -> " + Settings.Version;
                    else
                        upgradeOption = "�޷����������޸���װ����ʹ�õĿ����Ƿǹ����汾 - �汾��" + Max.Installs.SetupManager.GetCurrentVersion();
                }
            }
            else
                upgradeOption = "����/�޸���װ������";

            return upgradeOption;
        }

#if SQLITE
        /// <summary>
        /// �ж����ݿ���bbsmax/idmax��صı���Ѿ�����
        /// </summary>
        /// <returns></returns>
        public static bool IsSqlLiteMaxExists()
        {
            int i = 0;
            SQLiteConnection connection = new SQLiteConnection(Settings.Current.bbsMaxConnectionString);
            try
            {
                if (ConnectionState.Closed == connection.State)
                    connection.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM  bbsMax_Posts;", connection);//System_Max_Settings bbsMax_Posts
                i = Globals.ToInt32(cmd.ExecuteScalar(), 0);
                return 1 == i;
            }
            catch(Exception ex)
            {
                SetupManager.CreateLog("���ݿ��Ƿ����" + ex.Message + Settings.Current.bbsMaxConnectionString);
            }
            finally
            {
                connection.Close();
            }
            return false;
        }
#endif
    }
}