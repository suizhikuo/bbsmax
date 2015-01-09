//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Data.SqlClient;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;


using MaxLabs.bbsMax.FileSystem;


namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    /// <summary>
    /// 真实文件数据操作类,一个真实文件对应一条数据
    /// </summary>
    class FileDao : FileSystem.FileDao
    {
        //临时文件的生命周期（单位：小时）
        const int TempUploadFileLife = 3;

        #region 创建临时文件 CreateTempUploadFile

        [StoredProcedure(Name = "bx_CreateTempUploadFile", Script = @"
CREATE PROCEDURE {name}
    @UserID         int,
    @UploadAction   varchar(100),
    @SearchInfo     nvarchar(100),
    @CustomParams   nvarchar(3000),
    @FileName       nvarchar(256),
    @ServerFileName varchar(100),
    @MD5            char(32),
    @FileSize       bigint,
    @FileID         varchar(50),
    @TempUploadFileID int output
AS BEGIN

    SET NOCOUNT ON;

    INSERT INTO bx_TempUploadFiles (UserID, UploadAction, SearchInfo, CustomParams, FileName, ServerFileName, MD5, FileSize, FileID) VALUES (@UserID, @UploadAction, @SearchInfo, @CustomParams, @FileName, @ServerFileName, @MD5, @FileSize, @FileID);
    SELECT @TempUploadFileID = @@IDENTITY;

END
")]
        public override int CreateTempUploadFile(int userID, string uploadAction, string searchInfo, StringList customParamList, string filename, string serverFileName, string md5, long fileSize, string fileID)
        {
            int tempUploadFileID;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_CreateTempUploadFile";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@UploadAction", uploadAction, SqlDbType.VarChar, 100);
                query.CreateParameter<string>("@SearchInfo", searchInfo, SqlDbType.NVarChar, 100);
                query.CreateParameter<string>("@CustomParams", customParamList.ToString(), SqlDbType.NVarChar, 3000);
                query.CreateParameter<string>("@FileName", filename, SqlDbType.NVarChar, 256);
                query.CreateParameter<string>("@ServerFileName", serverFileName, SqlDbType.VarChar, 100);
                query.CreateParameter<string>("@MD5", md5, SqlDbType.Char, 32);
                query.CreateParameter<long>("@FileSize", fileSize, SqlDbType.Int);
                query.CreateParameter<string>("@FileID", fileID, SqlDbType.VarChar, 50);
                SqlParameter returnParam = query.CreateParameter<int>("@TempUploadFileID", SqlDbType.Int, ParameterDirection.Output);
                query.ExecuteNonQuery();

                tempUploadFileID = (int)returnParam.Value;
            }

            return tempUploadFileID;
        }

        #endregion

        #region 根据条件获取指定的临时文件

        [StoredProcedure(Name = "bx_GetUserTempUploadFiles", Script = @"
CREATE PROCEDURE {name}
    @UserID        int,
    @UploadAction  varchar(100),
    @SearchInfo   nvarchar(100),
    @TimeLife   int
AS BEGIN

    SET NOCOUNT ON;

    IF @SearchInfo IS NULL
        SELECT * FROM bx_TempUploadFiles WHERE UserID = @UserID AND UploadAction = @UploadAction AND CreateDate > DATEADD(hh, 0 - @TimeLife, GETDATE());
    ELSE
        SELECT * FROM bx_TempUploadFiles WHERE UserID = @UserID AND UploadAction = @UploadAction AND SearchInfo = @SearchInfo AND CreateDate > DATEADD(hh, 0 - @TimeLife, GETDATE());

END
")]
        public override TempUploadFileCollection GetUserTempUploadFiles(int userID, string uploadAction, string searchInfo)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_GetUserTempUploadFiles";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@UploadAction", uploadAction, SqlDbType.VarChar, 100);
                query.CreateParameter<string>("@SearchInfo", searchInfo, SqlDbType.NVarChar, 100);
                query.CreateParameter<int>("@TimeLife", TempUploadFileLife, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new TempUploadFileCollection(reader, userID);
                }
            }
        }


        [StoredProcedure(Name = "bx_GetUserTempUploadFile", Script = @"
CREATE PROCEDURE {name}
    @UserID        int,
    @TempUploadFileID int
AS BEGIN

    SET NOCOUNT ON;

    SELECT * FROM bx_TempUploadFiles WHERE TempUploadFileID = @TempUploadFileID AND UserID = @UserID;

END
")]
        public override TempUploadFile GetUserTempUploadFile(int userID, int tempUploadFileID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_GetUserTempUploadFile";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TempUploadFileID", tempUploadFileID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new TempUploadFile(reader);
                }
            }

            return null;
        }

        public override TempUploadFileCollection GetUserTempUploadFiles(int userID, IEnumerable<int> tempUploadFileIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_TempUploadFiles WHERE UserID = @UserID AND TempUploadFileID IN (@TempUploadFileIds);";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateInParameter<int>("@TempUploadFileIds", tempUploadFileIds);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new TempUploadFileCollection(reader, userID);
                }
            }
        }

        #endregion

        #region 用户删除自己的临时文件

        [StoredProcedure(Name = "bx_DeleteUserTempUploadFile", Script = @"
CREATE PROCEDURE {name}
    @UserID         int,
    @TempUploadFileID  int
AS BEGIN

    SET NOCOUNT ON;

    SELECT [ServerFileName] FROM bx_TempUploadFiles WHERE UserID = @UserID AND TempUploadFileID = @TempUploadFileID;
    DELETE bx_TempUploadFiles WHERE UserID = @UserID AND TempUploadFileID = @TempUploadFileID;

END
")]
        public override string DeleteUserTempUploadFile(int userID, int tempUploadFileID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_DeleteUserTempUploadFile";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TempUploadFileID", tempUploadFileID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                        return reader.Get<string>("ServerFileName");
                }

                return null;
            }
        }

        #endregion

        #region 将临时文件正式保存

        [StoredProcedure(Name = "bx_SaveFile", Script = @"
CREATE PROCEDURE {name}
    @UserID         int,
    @TempUploadFileID  int
AS BEGIN

    SET NOCOUNT ON;

    INSERT INTO bx_Files (FileID, ServerFilePath, MD5, FileSize)
        SELECT T.FileID, REPLACE(T.ServerFileName, '_', '\') AS ServerFilePath, T.MD5, T.FileSize
            FROM bx_TempUploadFiles T LEFT JOIN bx_Files F ON T.FileID = F.FileID
            WHERE T.UserID = @UserID AND T.TempUploadFileID = @TempUploadFileID
                AND F.FileID IS NULL;

    SELECT T.TempUploadFileID AS TempUploadFileID, T.FileName AS TempUploadFileName, T.ServerFileName AS TempUploadServerFileName, F.* FROM bx_TempUploadFiles T LEFT JOIN bx_Files F ON T.FileID = F.FileID WHERE T.UserID = @UserID AND T.TempUploadFileID = @TempUploadFileID;

    DELETE bx_TempUploadFiles WHERE UserID = @UserID AND TempUploadFileID = @TempUploadFileID;

END
")]
        public override PhysicalFileFromTemp SaveFile(int userID, int tempUploadFileID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = @"bx_SaveFile";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TempUploadFileID", tempUploadFileID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new PhysicalFileFromTemp(reader);
                }
            }

            return null;
        }

        public override PhysicalFileFromTempCollection SaveFiles(int userID, IEnumerable<int> allTempUploadFileIds, IEnumerable<int> saveTempUploadFileIds, IEnumerable<int> deleteTempUploadFileIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                //query.CommandType = CommandType.StoredProcedure;
                //query.CommandText = @"bx_SaveFiles";
                query.CommandText = @"
INSERT INTO bx_Files (FileID, ServerFilePath, MD5, FileSize)
    SELECT T.FileID, REPLACE(T.ServerFileName, '_', '\') AS ServerFilePath, T.MD5, T.FileSize
        FROM bx_TempUploadFiles T LEFT JOIN bx_Files F ON T.FileID = F.FileID
        WHERE T.UserID = @UserID AND T.TempUploadFileID IN (@SaveTempUploadFileIds)
            AND F.FileID IS NULL;
SELECT T.TempUploadFileID AS TempUploadFileID, T.FileName AS TempUploadFileName, T.ServerFileName AS TempUploadServerFileName, F.* FROM bx_TempUploadFiles T LEFT JOIN bx_Files F ON T.FileID = F.FileID WHERE T.UserID = @UserID AND T.TempUploadFileID IN (@SaveTempUploadFileIds)
DELETE bx_TempUploadFiles WHERE UserID = @UserID AND TempUploadFileID IN (@AllTempUploadFileIds)";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateInParameter<int>("@AllTempUploadFileIds", allTempUploadFileIds);
                query.CreateInParameter<int>("@SaveTempUploadFileIds", saveTempUploadFileIds);
                query.CreateInParameter<int>("@DeleteTempUploadFileIds", deleteTempUploadFileIds);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PhysicalFileFromTempCollection(reader);
                }
            }
        }

        #endregion

        #region 删除文件系统中的文件 DeleteFile

        ///// <summary>
        ///// 删除真实文件数据
        ///// </summary>
        ///// <param name="fileID">要删除的真实文件的唯一ID</param>
        ///// <returns>返回要删除的真实文件的路径</returns>
        //public override void DeleteFiles(IEnumerable<string> fileIds)
        //{
        //    using (SqlQuery query = new SqlQuery())
        //    {
        //        query.CommandText = @"DELETE [bx_Files] WHERE [FileID] IN (@FileIds);";

        //        query.CreateInParameter("@FileIds", fileIds);

        //        query.ExecuteNonQuery();
        //    }
        //}

        public override void SetFilesDeleted(IEnumerable<int> deletingFileIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"DELETE [bx_DeletingFiles] WHERE [DeletingFileID] IN (@FileIds);";

                query.CreateInParameter("@FileIds", deletingFileIds);

                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_GetTop300DeletingFiles", Script = @"
CREATE PROCEDURE {name}
AS BEGIN

    SET NOCOUNT ON;

    SELECT TOP 300 * FROM [bx_DeletingFiles] ORDER BY DeletingFileID DESC;

END
")]
        public override List<DeletingFile> GetDeletingFiles()
        {
            List<DeletingFile> result = new List<DeletingFile>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = @"bx_GetTop300DeletingFiles";

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new DeletingFile(reader));
                    }
                }
            }

            return result;
        }

        #endregion

        #region 根据一组文件ID获取文件系统中的文件

        [StoredProcedure(Name = "bx_GetFileByID", Script = @"
CREATE PROCEDURE {name}
    @FileID    varchar(50)
AS BEGIN

    SET NOCOUNT ON;

    SELECT * FROM [bx_Files] WHERE [FileID] = @FileID;

END
")]
        public override PhysicalFile GetFile(string fileID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = @"bx_GetFileByID";

                query.CreateParameter("@FileID", fileID, SqlDbType.VarChar, 50);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new PhysicalFile(reader);
                }
            }

            return null;
        }

        public override PhysicalFileCollection GetFiles(IEnumerable<string> fileIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"SELECT * FROM [bx_Files] WHERE [FileID] IN (@FileIds);";

                query.CreateInParameter("@FileIds", fileIds);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PhysicalFileCollection(reader);
                }
            }
        }

        #endregion

        #region 清理过期的临时文件用

        [StoredProcedure(Name = "bx_ClearExpiredTempUploadFiles", Script = @"
CREATE PROCEDURE {name}
    @HoursBefore    int
AS BEGIN

    SET NOCOUNT ON;

    DECLARE @ClearTempUploadFileIds table(ID int);

    INSERT INTO @ClearTempUploadFileIds (ID) SELECT TempUploadFileID FROM bx_TempUploadFiles WHERE CreateDate < DATEADD(hh, 0 - @HoursBefore, GETDATE());

    SELECT * FROM bx_TempUploadFiles T INNER JOIN @ClearTempUploadFileIds C ON T.TempUploadFileID = C.ID;

    DELETE bx_TempUploadFiles FROM @ClearTempUploadFileIds C WHERE bx_TempUploadFiles.TempUploadFileID = C.ID;

END
")]
        public override TempUploadFileCollection ClearExperisTempUploadFiles()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_ClearExpiredTempUploadFiles";

                query.CreateParameter<int>("@HoursBefore", TempUploadFileLife + 3, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new TempUploadFileCollection(reader);
                }
            }
        }

        #endregion

    }

}