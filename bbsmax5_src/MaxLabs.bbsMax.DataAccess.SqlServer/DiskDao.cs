//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using System.Collections;
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Util;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    /// <summary>
    /// ����������ݷ�������SqlServerʵ��
    /// </summary>
    public class DiskDao : DataAccess.DiskDao
    {

        public override DiskFileCollection AdminSearchFiles(DiskFileFilter filter, IEnumerable<Guid> exculdeRoles, int pageIndex)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder builder = new StringBuilder();

                if (filter.UserID != null)
                {
                    builder.Append(" AND UserID = @UserID");
                    query.CreateParameter<int>("@UserID", filter.UserID.Value, SqlDbType.Int);
                }

                if (filter.Username != null)
                {
                    builder.Append(" AND UserID IN (SELECT UserID FROM bx_Users WHERE Username LIKE '%' + @Username + '%')");
                    query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                }

                if (filter.DirectoryName != null)
                {
                    builder.Append(" AND DirectoryID IN (SELECT DirectoryID FROM bx_DiskDirectories WHERE [Name] LIKE '%' + @DirectoryName + '%')");
                    query.CreateParameter<string>("@DirectoryName", filter.DirectoryName, SqlDbType.NVarChar, 256);
                }

                if (filter.Filename != null)
                {
                    builder.Append(" AND FileName LIKE '%' + @FileName + '%'");
                    query.CreateParameter<string>("@FileName", filter.Filename, SqlDbType.NVarChar, 256);
                }

                if (filter.Size_1 != null)
                {
                    builder.Append(" AND FileSize >= @FileSize1");
                    long size = filter.Size_1.Value;
                    switch (filter.SizeUnit_1)
                    {
                        case FileSizeUnit.K:
                            size *= 1024;
                            break;
                        case FileSizeUnit.M:
                            size *= 1024 * 1024;
                            break;
                        case FileSizeUnit.G:
                            size *= 1024 * 1024 * 1024;
                            break;
                    }

                    query.CreateParameter<long>("@FileSize1", size, SqlDbType.BigInt);
                }

                if (filter.Size_2 != null)
                {
                    builder.Append(" AND FileSize <= @FileSize2");

                    long size = filter.Size_2.Value;
                    switch (filter.SizeUnit_2)
                    {
                        case FileSizeUnit.K:
                            size *= 1024;
                            break;
                        case FileSizeUnit.M:
                            size *= 1024 * 1024;
                            break;
                        case FileSizeUnit.G:
                            size *= 1024 * 1024 * 1024;
                            break;
                    }
                    query.CreateParameter<long>("@FileSize2", size, SqlDbType.BigInt);
                }

                if (filter.CreateDate_1 != null)
                {
                    builder.Append(" AND CreateDate >= @CreateDate1");
                    query.CreateParameter<DateTime>("@CreateDate1", filter.CreateDate_1.Value, SqlDbType.DateTime);
                }

                if (filter.CreateDate_2 != null)
                {
                    builder.Append(" AND CreateDate <= @CreateDate2");
                    query.CreateParameter<DateTime>("@CreateDate2", filter.CreateDate_2.Value, SqlDbType.DateTime);
                }

                string noSelectRoles = DaoUtil.GetExcludeRoleSQL("[UserID]", exculdeRoles, query);
                if (!string.IsNullOrEmpty(noSelectRoles))
                {
                    builder.Append(" AND ");
                    builder.Append(noSelectRoles);
                }

                if (builder.Length >= 5)
                    builder.Remove(0, 5);

                query.Pager.TableName = "bx_DiskFiles";
                query.Pager.PageSize = filter.PageSize;
                query.Pager.PageNumber = pageIndex;
                query.Pager.Condition = builder.ToString();
                query.Pager.IsDesc = filter.IsDesc == null ? true : filter.IsDesc.Value;
                query.Pager.PrimaryKey = "DiskFileID";
                query.Pager.SortField = "DiskFileID";
                query.Pager.SelectCount = true;

                if (filter.Order != null)
                {
                    switch (filter.Order.Value)
                    {
                        case FileOrderBy.CreateDate:
                            query.Pager.SortField = "CreateDate";
                            break;
                        case FileOrderBy.Name:
                            query.Pager.SortField = "FileName";
                            break;
                        case FileOrderBy.Size:
                            query.Pager.SortField = "FileSize";
                            break;
                        case FileOrderBy.Type:
                            query.Pager.SortField = "Extension";
                            break;
                    }
                }

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DiskFileCollection files = new DiskFileCollection(reader);
                    if (reader.NextResult())
                        if (reader.Read())
                            files.TotalRecords = reader.Get<int>(0);
                    return files;
                }
            }
        }

        #region �洢����
        [StoredProcedure(Name = "bx_SaveUploadFile", Script = @"
CREATE PROCEDURE {name}
 @UserID             int
,@DirectoryID        int
,@FileName           nvarchar(256)
,@FileID             varchar(50)
,@FileSize           bigint
,@CanUseSpaceSize    bigint
,@ReplaceExistFile   int

AS
BEGIN

    SET NOCOUNT ON;

    /* include:CheckDirectoryID.sql */

    --�û�ѡ���˸����Ѵ��ڵ�ͬ���ļ�
    IF @ReplaceExistFile = 1 BEGIN

        DECLARE @ExistDiskFileID int;
        DECLARE @ExistFileSize bigint;

        SET @ExistDiskFileID = -1;

        SELECT @ExistDiskFileID = DiskFileID, @ExistFileSize = FileSize FROM bx_DiskFiles WHERE DirectoryID = @DirectoryID AND [FileName] = @FileName;

        --����Ĵ���ͬ���ļ�,ɾ��֮,�����¼�������ʹ�õ�����Ӳ�̿ռ��С
        IF @ExistDiskFileID <> -1 BEGIN

            DELETE bx_DiskFiles WHERE DiskFileID = @ExistDiskFileID;
            SET @CanUseSpaceSize = @CanUseSpaceSize +  @ExistFileSize - @FileSize;

        END

    END

    --�������Ӳ�̿ռ��Ƿ񳬶�
    IF @CanUseSpaceSize > @FileSize BEGIN

        INSERT INTO bx_DiskFiles( FileID, [FileName],UserID,DirectoryID, [FileSize], ThumbPath) VALUES(@FileID ,@FileName,@UserID,@DirectoryID, @FileSize, '');
        RETURN 1;

    END
    ELSE
        RETURN 0;

END
")]
        #endregion
        public override bool SaveUploadFile(int userID, int directoryID, string fileID, string fileName, long fileSize, long canUseSpaceSize, bool replaceExistFile)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_SaveUploadFile";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@FileID", fileID, SqlDbType.VarChar, 50);
                query.CreateParameter<long>("@FileSize", fileSize, SqlDbType.BigInt);
                query.CreateParameter<string>("@FileName", fileName, SqlDbType.NVarChar, 256);
                query.CreateParameter<int>("@DirectoryID", directoryID, SqlDbType.Int);
                query.CreateParameter<long>("@CanUseSpaceSize", canUseSpaceSize, SqlDbType.BigInt);
                query.CreateParameter<int>("@ReplaceExistFile", replaceExistFile ? 1 : 0, SqlDbType.Int);
                //query.CreateParameter<string>("@ThumbPath", thumbPath, SqlDbType.NVarChar, 256);
                SqlParameter result = query.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                if (Convert.ToInt32(result.Value) == 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ɸѡָ��ƥ���ļ������ļ� zsy
        /// </summary>
        public override DiskFileCollection GetFilterNameDiskFiles(int userID, int directoryID, string namePattern)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.Text;
                query.CommandText = "SELECT * FROM bx_DiskFiles WHERE DirectoryID = @DirectoryID AND FileName LIKE @FileName + '%' AND UserID = @UserID";

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@DirectoryID", directoryID, SqlDbType.Int);
                query.CreateParameter("@FileName", namePattern, SqlDbType.NVarChar, 256);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new DiskFileCollection(reader);
                }
            }
        }


        /// <summary>
        /// ��ô����ļ��б���������ļ��к��ļ�
        /// </summary>
        /// <param name="userID">�û�ID</param>
        /// <param name="directoryID">�ļ���ID</param>
        /// <param name="directories">���ص��ļ����б�</param>
        /// <param name="files">���ص��ļ��б�</param>
        /// <returns></returns>
        #region �洢����
        [StoredProcedure(Name = "bx_GetDiskFiles", Script = @"
CREATE PROCEDURE {name}
@UserID int,
@DirectoryID int

AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @Run bit;

	IF @DirectoryID < 1 BEGIN
		SELECT @DirectoryID = DirectoryID FROM [bx_DiskDirectories] WITH (NOLOCK) WHERE UserID = @UserID AND ParentID = 0 AND Name = N'\';
		IF(@DirectoryID>0)
			SELECT @Run = 1;
		ELSE
		    SELECT @Run=0;
	END
	ELSE BEGIN
		IF EXISTS (SELECT * FROM [bx_DiskDirectories] WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND UserID = @UserID)
			SELECT @Run = 1;
		ELSE
			SELECT @Run = 0;
	END

    IF @Run = 1 BEGIN
		SELECT *
		    FROM bx_DiskDirectories WITH (NOLOCK)
		    WHERE UserID = @UserID AND ParentID = @DirectoryID
		    ORDER BY Name;

		SELECT * FROM bx_DiskFiles WITH (NOLOCK) WHERE (DirectoryID = @DirectoryID) ORDER BY [FileName];

		--SELECT ISNULL(SUM(TotalSize), 0) AS TotalSize FROM bx_DiskDirectories WITH (NOLOCK) WHERE [UserID] = @UserID;

	END
END
")]
        #endregion
        public override void GetDiskFiles(int userID, int directoryID, out DiskDirectoryCollection directories, out DiskFileCollection files)
        {
            files = null;
            directories = null;

            using (SqlQuery query = new SqlQuery())
            {

                query.CommandText = "bx_GetDiskFiles";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@DirectoryID", directoryID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    directories = new DiskDirectoryCollection(reader);

                    if (reader.NextResult())
                        files = new DiskFileCollection(reader);

                }
            }

            if (files == null)
                files = new DiskFileCollection();

            if (directories == null)
                directories = new DiskDirectoryCollection();
        }


        //public override DeleteStatus DeleteDiskFiles(IEnumerable<int> diskFileIds)
        //{
        //    using (SqlQuery query = new SqlQuery())
        //    {
        //        query.CommandText = "DELETE [bx_DiskFiles] WHERE [DiskFileID] IN (@DiskFileIds)";

        //        query.CreateInParameter("@DiskFileIds", diskFileIds);

        //        query.ExecuteNonQuery();
        //    }

        //    return DeleteStatus.Success;
        //}

        #region �洢����
        [StoredProcedure(Name = "bx_DeleteDiskFiles", Script = @"
CREATE PROCEDURE {name} 
	@UserID int,
	@DirectoryID int,
	@DiskFileIds text
AS
BEGIN

	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM [bx_DiskDirectories] WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND UserID = @UserID) BEGIN
        SELECT FileID FROM  [bx_DiskFiles] WHERE [DiskFileID] IN (SELECT item FROM bx_GetIntTable(@DiskFileIds,',')) AND [DirectoryID] = @DirectoryID;
		EXEC ('DELETE [bx_DiskFiles] WHERE [DiskFileID] IN (' + @DiskFileIds + ') AND [DirectoryID] = ' + @DirectoryID);
		RETURN (0);
	END
	ELSE
		RETURN (1);
END")]
        #endregion
        public override DeleteStatus DeleteDiskFiles(int userID, int directoryID, IEnumerable<int> diskFileIds)
        {
            if (ValidateUtil.HasItems(diskFileIds) == false)
                return DeleteStatus.Success;

            DeleteStatus status;
            using (SqlQuery query = new SqlQuery())
            {

                string diskFileIdsString = StringUtil.Join(diskFileIds);

                query.CommandText = "bx_DeleteDiskFiles";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@DirectoryID", directoryID, SqlDbType.Int);
                query.CreateParameter<string>("@DiskFileIds", diskFileIdsString, SqlDbType.Text);
                SqlParameter param = query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                status = (DeleteStatus)Convert.ToInt32(param.Value);
                return status;
            }
        }


        #region �洢����
        [StoredProcedure(Name = "bx_Disk_GetDiskFileByUserAndID", Script = @"
CREATE PROCEDURE {name}
	@UserID int,
	@DiskFileID int
AS
BEGIN

	SET NOCOUNT ON;

    SELECT * FROM bx_DiskFiles WHERE DiskFileID = @DiskFileID AND UserID = @UserID;

END
")]
        #endregion
        public override DiskFile GetDiskFile(int userID, int diskFileID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Disk_GetDiskFileByUserAndID";

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@DiskFileID", diskFileID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new DiskFile(reader);
                }

            }
            return null;
        }

        #region �洢����
        [StoredProcedure(Name = "bx_Disk_GetDiskFileByID", Script = @"
CREATE PROCEDURE {name}
	@DiskFileID int
AS
BEGIN

	SET NOCOUNT ON;

    SELECT * FROM bx_DiskFiles WHERE DiskFileID = @DiskFileID;

END
")]
        #endregion
        public override DiskFile GetDiskFile(int diskFileID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Disk_GetDiskFileByID";

                query.CreateParameter("@DiskFileID", diskFileID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new DiskFile(reader);
                }

            }
            return null;
        }


        #region �洢����
        [StoredProcedure(Name = "bx_Disk_GetDiskFileByFileName", Script = @"
CREATE PROCEDURE {name} 
	@UserID int,
	@DirectoryID int,
	@FileName nvarchar(256)
AS
BEGIN

	SET NOCOUNT ON;

    IF @DirectoryID < 1
		SELECT @DirectoryID = DirectoryID FROM bx_DiskDirectories WHERE ParentID = 0 AND UserID = @UserID;

    SELECT * FROM bx_DiskFiles WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND [FileName] = @FileName AND UserID = @UserID;

END
")]
        #endregion
        public override DiskFile GetDiskFile(int userID, int directoryID, string fileName)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Disk_GetDiskFileByFileName";

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@DirectoryID", directoryID, SqlDbType.Int);
                query.CreateParameter("@FileName", fileName, SqlDbType.NVarChar, 256);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new DiskFile(reader);
                }

            }
            return null;
        }

        public override DiskFileCollection GetDiskFiles(IEnumerable<int> diskFileIds)
        {
            //Dictionary<int, DiskFile> diskFiles = new Dictionary<int, DiskFile>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_DiskFiles WHERE DiskFileID IN (@DiskFileIds)";

                query.CreateInParameter("@DiskFileIds", diskFileIds);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new DiskFileCollection(reader);
                }
            }
        }

        #region �洢����
        [StoredProcedure(Name = "bx_RenameDiskFile", Script = @"
CREATE PROCEDURE {name}
	@UserID int,
	@DiskFileID int,
	@NewFileName nvarchar(256)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @DirectoryID int;
	DECLARE @FileName nvarchar(256);

	SELECT @DirectoryID = DirectoryID, @FileName = [FileName] FROM bx_DiskFiles WITH (NOLOCK) WHERE DiskFileID = @DiskFileID;

	IF EXISTS (SELECT * FROM bx_DiskDirectories WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND UserID = @UserID) BEGIN
		IF (@FileName = @NewFileName)
			RETURN (0);
		ELSE BEGIN
			IF EXISTS (SELECT * FROM bx_DiskDirectories WITH (NOLOCK) WHERE [UserID] = @UserID AND ParentID = @DirectoryID AND [Name] = @NewFileName)
				OR EXISTS (SELECT * FROM bx_DiskFiles WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND [FileName] = @NewFileName)
				RETURN (3);
			ELSE BEGIN
				UPDATE bx_DiskFiles SET [FileName] = @NewFileName WHERE DiskFileID = @DiskFileID;
				RETURN (0);
			END
		END
	END
	ELSE
		RETURN (-1);

END
")]
        #endregion
        public override CreateUpdateDiskFileStatus RenameDiskFile(int userID, int diskFileID, string newFileName)
        {
            CreateUpdateDiskFileStatus status;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_RenameDiskFile";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@DiskFileID", diskFileID, SqlDbType.Int);
                query.CreateParameter("@NewFileName", newFileName, SqlDbType.NVarChar, 256);
                SqlParameter returnParam = query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                status = (CreateUpdateDiskFileStatus)Convert.ToInt32(returnParam.Value);
            }

            return status;
        }


        #region �洢����
        [StoredProcedure(Name = "bx_CreateDiskDirectory", Script = @"
CREATE PROCEDURE {name}
	@UserID int,
	@ParentID int,
	@Name nvarchar(256),
	@DirectoryID int output
AS
BEGIN
	SET NOCOUNT ON;

    IF (@ParentID < 1)
		SELECT @ParentID = DirectoryID FROM bx_DiskDirectories WHERE UserID = @UserID AND ParentID = 0;
    IF (@ParentID < 1) BEGIN
		INSERT INTO [bx_DiskDirectories] (UserID, ParentID, Name) VALUES (@UserID, 0, N'\');
		SELECT @ParentID = @@Identity;
    END
	
	IF EXISTS (SELECT * FROM bx_DiskDirectories WITH (NOLOCK) WHERE UserID = @UserID AND ParentID = @ParentID AND [Name] = @Name)
		OR EXISTS (SELECT * FROM [bx_DiskFiles] WITH (NOLOCK) WHERE [DirectoryID] = @ParentID AND [FileName] = @Name)
		RETURN (2);
	ELSE BEGIN
		INSERT INTO [bx_DiskDirectories]([UserID], [ParentID], [Name]) VALUES (@UserID, @ParentID, @Name);
		SELECT @DirectoryID = @@IDENTITY;
		RETURN (1);
	END
END
")]

        #endregion
        public override int CreateDiskDirectory(int userID, int parentID, string name, out int directoryID)
        {
            int status;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_CreateDiskDirectory";

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@ParentID", parentID, SqlDbType.Int);
                query.CreateParameter("@Name", name, SqlDbType.NVarChar, 256);
                SqlParameter outParam = query.CreateParameter<int>("@DirectoryID", SqlDbType.Int, ParameterDirection.Output);
                SqlParameter returnParam = query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                status = Convert.ToInt32(returnParam.Value);

                if (status == 1)
                    directoryID = Convert.ToInt32(outParam.Value);
                else
                    directoryID = 0;
            }

            return status;
        }

        #region �洢����

        [StoredProcedure(Name = "bx_RenameDiskDirectory", Script = @"
CREATE PROCEDURE {name}
	@UserID int,
	@DirectoryID int,
	@NewName nvarchar(256)
AS
BEGIN

	SET NOCOUNT ON;

	--IF EXISTS (SELECT * FROM bx_DiskDirectories WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND [UserID] = @UserID) BEGIN

	DECLARE @ParentID int;

	SELECT @ParentID = [ParentID] FROM bx_DiskDirectories WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND [UserID] = @UserID;
	IF @@ROWCOUNT > 0 BEGIN
		IF EXISTS (SELECT * FROM bx_DiskDirectories WITH (NOLOCK) WHERE [UserID] = @UserID AND ParentID = @ParentID AND [Name] = @NewName)
			OR EXISTS (SELECT * FROM [bx_DiskFiles] WITH (NOLOCK) WHERE [DirectoryID] = @ParentID AND [FileName] = @NewName)
			RETURN (2);
		ELSE BEGIN

			UPDATE bx_DiskDirectories SET Name = @NewName WHERE DirectoryID = @DirectoryID;
			IF @@ROWCOUNT > 0
				RETURN (1);
			ELSE
				RETURN (-1);
		END
	END
	ELSE
		RETURN (-1);

END
")]
        #endregion
        public override int RenameDiskDirectory(int userID, int directoryID, string newName)
        {
            int result;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_RenameDiskDirectory";

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@DirectoryID", directoryID, SqlDbType.Int);
                query.CreateParameter("@NewName", newName, SqlDbType.NVarChar, 256);
                SqlParameter returnParam = query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                result = Convert.ToInt32(returnParam.Value);
            }

            return result;
        }

        #region myxbing add in
        [StoredProcedure(Name = "bx_GetDiskDirectoryByID", Script = @"
CREATE PROCEDURE {name} 
(
 @DirectoryID int,
 @UserID int
)
AS
BEGIN

    SET NOCOUNT ON;

	IF(@DirectoryID <= 0) BEGIN
	   SELECT * FROM [bx_DiskDirectories] WHERE ParentID = 0 AND UserID = @UserID;
	END 
	ELSE BEGIN
	   SELECT * FROM [bx_DiskDirectories] WHERE DirectoryID = @DirectoryID AND UserID = @UserID;
	END
END")]
        public override DiskDirectory GetDiskDirectory(int userID, int directoryID)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_GetDiskDirectoryByID";

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@DirectoryID", directoryID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new DiskDirectory(reader);
                    }
                }
            }

            return new DiskDirectory(0, 0, "\\"); ;
        }


        #region �洢����
        [StoredProcedure(Name = "bx_GetCurrentAndParentDirectories", Script = @"
CREATE PROCEDURE {name}
(
    @UserID int,
    @DirectoryID int
)
AS
BEGIN

    SET NOCOUNT ON;
 
	DECLARE @Run bit;

	IF @DirectoryID < 1 BEGIN
		SELECT @DirectoryID = DirectoryID FROM [bx_DiskDirectories] WITH (NOLOCK) WHERE UserID = @UserID AND ParentID = 0 AND Name = N'\';
		IF(@DirectoryID > 0)
			SELECT @Run = 1;
	    ELSE
			SELECT @Run = 0;
	END
	ELSE BEGIN
		IF EXISTS (SELECT * FROM [bx_DiskDirectories] WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND UserID = @UserID)
			SELECT @Run = 1;
		ELSE
			SELECT @Run = 0;
	END
	
	IF(@Run = 1) BEGIN
	    DECLARE @ParentID int;
	    SET @ParentID = 1;
	    DECLARE @CurrentDirectoryID int;
	    SET @CurrentDirectoryID = @DirectoryID;

	    WHILE(@ParentID > 0) BEGIN

			SELECT @ParentID = ParentID FROM [bx_DiskDirectories] WHERE DirectoryID = @CurrentDirectoryID;
			
			SELECT *
			FROM
				bx_DiskDirectories WITH (NOLOCK)
			WHERE UserID = @UserID AND ParentID = @CurrentDirectoryID
			ORDER BY Name;

			IF(@ParentID > 0) BEGIN
				SET @CurrentDirectoryID = @ParentID;
			END
			
	    END
	END
END
")]
        #endregion
        public override Dictionary<int, DiskDirectoryCollection> GetParentDirectories(int userID, int directoryID)
        {
            Dictionary<int, DiskDirectoryCollection> menuDirectories = new Dictionary<int, DiskDirectoryCollection>();
            int parentID = directoryID;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetCurrentAndParentDirectories";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@DirectoryID", directoryID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    do
                    {
                        DiskDirectoryCollection directories = new DiskDirectoryCollection(reader);

                        if (directories.Count != 0)
                        {
                            parentID = directories[0].ParentID;

                            if (!menuDirectories.ContainsKey(parentID))
                            {
                                menuDirectories.Add(parentID, directories);
                            }

                        }
                    }
                    while (reader.NextResult());

                }
            }

            return menuDirectories;
        }


        #region �洢����
        [StoredProcedure(Name = "bx_DeleteDiskTree", Script = @"
CREATE PROCEDURE {name} 
@UserID int,
@DirectoryIds varchar(8000)
AS
BEGIN

    SET NOCOUNT ON;

    --�õ�������Ŀ¼��ID

	--IsSelected:
	----2 completed
	----1 now selected
	----0 next selected
	--DECLARE @DirectoriesTable table(DirectoryID int NOT NULL,IsSelected int default(0));
    DECLARE @DirectoryIDTable TABLE(ID int IDENTITY (1,1) ,DirectoryID int ,ParentID int DEFAULT(0), IsSelected int DEFAULT(0));
    
	--Insert into the table of self instance
	INSERT INTO @DirectoryIDTable(DirectoryID, IsSelected) 
    SELECT DirectoryID,1 FROM [bx_DiskDirectories] WITH (NOLOCK)
        WHERE [DirectoryID] IN (SELECT item FROM bx_GetIntTable(@DirectoryIds,',')) AND [UserID] = @UserID;
    
	--While condition
	DECLARE @Num int;
	SET @Num = 1;
    
	WHILE(@Num > 0)
	BEGIN
		INSERT INTO @DirectoryIDTable(DirectoryID)
		SELECT DirectoryID FROM [bx_DiskDirectories] WITH (NOLOCK)
		    WHERE UserID = @UserID AND ParentID IN (SELECT DirectoryID FROM @DirectoryIDTable WHERE IsSelected = 1);
	    
		UPDATE @DirectoryIDTable SET IsSelected = IsSelected + 1 WHERE IsSelected <> 2;

		--If there any rows for now selected
		SELECT @Num = COUNT(*) FROM @DirectoryIDTable WHERE IsSelected = 1;
	END


	DECLARE @ParentIds TABLE(ID_P int IDENTITY (1,1),ParentID_P int DEFAULT 0)

	INSERT INTO @ParentIds(ParentID_P)
	SELECT ParentID FROM bx_DiskDirectories WHERE DirectoryID IN (SELECT DirectoryID FROM @DirectoryIDTable)
	
	UPDATE @DirectoryIDTable SET ParentID = ParentID_P FROM @ParentIds WHERE ID = ID_P;

	DECLARE @DirectoryID int;
	SET @DirectoryID = -1;

	SELECT @DirectoryID = DirectoryID FROM @DirectoryIDTable WHERE DirectoryID NOT IN 
	    (SELECT A.DirectoryID FROM @DirectoryIDTable AS A
	        INNER JOIN @DirectoryIDTable AS B
	        ON A.DirectoryID = B.ParentID);

	--�����ǰĿ¼��������Ŀ¼�ĸ�Ŀ¼����ɾ�����Դ�����
	BEGIN TRANSACTION
	WHILE(@DirectoryID > 0 AND EXISTS(SELECT DirectoryID FROM bx_DiskDirectories WHERE DirectoryID = @DirectoryID))
	BEGIN
		IF(@UserID > 0)
		BEGIN
			DELETE FROM bx_DiskDirectories WHERE DirectoryID = @DirectoryID AND [UserID] = @UserID;
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
			END
		END
		ELSE
		BEGIN
			DELETE FROM bx_DiskDirectories WHERE DirectoryID = @DirectoryID;
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
                RETURN -1;
			END
		END

		DELETE FROM @DirectoryIDTable WHERE DirectoryID = @DirectoryID;

		SET @DirectoryID = -1;

		SELECT @DirectoryID = DirectoryID FROM @DirectoryIDTable WHERE DirectoryID NOT IN 
		    (SELECT A.DirectoryID FROM @DirectoryIDTable AS A
		        INNER JOIN @DirectoryIDTable AS B
		        ON A.DirectoryID = B.ParentID);
	END
	COMMIT TRANSACTION
END
")]
        #endregion
        public override DeleteStatus DeleteDiskDirectories(int userID, IEnumerable<int> directoryIds)
        {
            if (ValidateUtil.HasItems(directoryIds) == false)
                return DeleteStatus.Success;

            //directoryIdentities.Sort(Comparer<int>.Default);
            //directoryIdentities.Reverse();

            using (SqlQuery query = new SqlQuery())
            {
                string directoryIdsString = StringUtil.Join(directoryIds);

                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_DeleteDiskTree";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@DirectoryIds", directoryIdsString, SqlDbType.VarChar, 8000);
                SqlParameter param = query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (DeleteStatus)Convert.ToInt32(param.Value);
            }

        }

        #region �洢����
        [StoredProcedure(Name = "bx_MoveDirectoriesAndFiles", Script = @"
CREATE PROCEDURE {name}
(
 	@UserID int,--user ID
	@DirectoryID int,--the parentID of @DiskFileIDs and @DiskDirectoryIDs 
	@NewDirectoryID int,--move to the directory ID
	@DiskFileIDs text=NULL,--the file ids will be moved
	@DiskDirectoryIDs text = NULL--the directoryIDs will be moved
)
AS
BEGIN
	SET NOCOUNT ON;
    
    IF(@DirectoryID<1) BEGIN
       SELECT @DirectoryID=DirectoryID FROM [bx_DiskDirectories] WHERE UserID=@UserID AND ParentID=0;
    END
    
    IF(@NewDirectoryID<1) BEGIN
       SELECT @NewDirectoryID=DirectoryID FROM [bx_DiskDirectories] WHERE UserID=@UserID AND ParentID=0;
    END
    
    IF(EXISTS(SELECT * FROM [bx_DiskDirectories] WITH(NOLOCK) WHERE DirectoryID=@DirectoryID AND UserID=@UserID)) 
    BEGIN
        
        DECLARE @MoveFileDirectoryNames table([FileDirectoryName] nvarchar(256) COLLATE Chinese_PRC_CI_AS_WS);
        
        IF(datalength(@DiskDirectoryIDs)<> 0) BEGIN
            INSERT INTO @MoveFileDirectoryNames([FileDirectoryName])
            SELECT [Name] AS FileDirectoryName FROM [bx_DiskDirectories] WITH(NOLOCK) WHERE DirectoryID IN (SELECT item FROM bx_GetIntTable(@DiskDirectoryIDs,',')) AND ParentID=@DirectoryID AND UserID=@UserID;          
        END
        
        IF(datalength(@DiskFileIDs)<>0) BEGIN
            INSERT INTO @MoveFileDirectoryNames([FileDirectoryName])
            SELECT [FileName] AS FileDirectoryName FROM [bx_DiskFiles] WITH(NOLOCK) WHERE DiskFileID IN(SELECT item FROM bx_GetIntTable(@DiskFileIDs,',')) AND DirectoryID=@DirectoryID;
        END      
        
        IF(NOT EXISTS(SELECT * FROM [bx_DiskDirectories] WHERE ParentID=@NewDirectoryID AND UserID=@UserID AND [Name] IN (SELECT FileDirectoryName FROM @MoveFileDirectoryNames))
           AND
           NOT EXISTS(SELECT * FROM [bx_DiskFiles] WHERE DirectoryID=@NewDirectoryID AND [FileName] IN (SELECT FileDirectoryName FROM @MoveFileDirectoryNames)))
        BEGIN
            --update bx_DiskFiles.DirectoryID to the new directoryID
            IF(datalength(@DiskFileIDs)<>0) BEGIN
			  UPDATE [bx_DiskFiles]
			  SET DirectoryID=@NewDirectoryID
			  WHERE DiskFileID IN (SELECT item FROM bx_GetIntTable(@DiskFileIDs,','))
	        END
	        
	        IF(datalength(@DiskDirectoryIDs)<> 0) BEGIN    
			--update bx_DiskDirectories.ParentID to the new directoryID
			  UPDATE [bx_DiskDirectories]
			  SET ParentID=@NewDirectoryID
			  WHERE DirectoryID IN (SELECT item FROM bx_GetIntTable(@DiskDirectoryIDs,','))
            END
            
            RETURN(0);            
        END 
        ELSE BEGIN 
            RETURN(-1);
        END
    END
END
")]
        #endregion
        public override MoveStatus MoveDiskDirectoriesAndFiles(int userID, int directoryID, int newDirectoryID, List<int> diskFileIDs, List<int> diskDirectoryIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_MoveDirectoriesAndFiles";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@DirectoryID", directoryID, SqlDbType.Int);
                query.CreateParameter("@NewDirectoryID", newDirectoryID, SqlDbType.Int);
                query.CreateParameter("@DiskFileIDs", StringUtil.Join(diskFileIDs, ","), SqlDbType.Text);
                query.CreateParameter("@DiskDirectoryIDs", StringUtil.Join(diskDirectoryIDs, ","), SqlDbType.Text);
                SqlParameter returnParam = query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (MoveStatus)Convert.ToInt32(returnParam.Value);
            }
        }

        [StoredProcedure(Name = "bx_GetRootDiskDirectory", Script = @"
CREATE PROCEDURE {name}
(
 	@UserID int
)
AS
BEGIN

	SET NOCOUNT ON;

    SELECT * FROM bx_DiskDirectories WHERE UserID = @UserID AND ParentID = 0;

END
")]
        public override DiskDirectory GetDiskRootDirectory(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_GetRootDiskDirectory";

                query.CreateParameter("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new DiskDirectory(reader);
                }
            }

            return null;
        }
        #endregion

        #region managers
        private DiskEntry PopulateFromDisk(IDataReader dr)
        {
            DiskEntry entry = new DiskEntry();
            entry.CreateDate = Convert.ToDateTime(dr["CreateDate"]);
            try
            {
                entry.DirectoryID = Convert.ToInt32(dr["DirectoryID"]);
                entry.DirectoryName = dr["Name"].ToString();
            }
            catch
            {
                entry.DiskID = Convert.ToInt32(dr["DiskID"]);
                entry.FileName = dr["FileName"].ToString();
            }
            entry.Path = dr["Path"].ToString();
            entry.UserID = Convert.ToInt32(dr["UserID"]);
            return entry;
        }


        public override void AdminDeleteDiskFiles(IEnumerable<int> ids, IEnumerable<Guid> excludeRoleIds)
        {
            using (SqlQuery query = new SqlQuery())
            {

                string sql = "DELETE FROM bx_DiskFiles WHERE DiskFileID IN (@FileIds)";

                query.CreateInParameter<int>("@FileIds", ids);

                string excludeRoleUsers = DaoUtil.GetExcludeRoleSQL("UserID", excludeRoleIds, query);

                if (!string.IsNullOrEmpty(excludeRoleUsers))
                {
                    sql += " AND " + excludeRoleUsers;
                }

                query.CommandText = sql;
                query.ExecuteNonQuery();
            }
        }

        #region �洢����
        [StoredProcedure(Name = "bx_RenameDiskDirectoriesAndDiskFiles", Script = @"
CREATE PROC {name}
@UserID INT,
	@ParentID INT,
	@DirectoryIDs VARCHAR(8000),
	@DiskFileIDs VARCHAR(8000),
	@DirectoryNames NVARCHAR(4000),
	@DiskFileNames NVARCHAR(4000)
AS
    SET NOCOUNT ON;

	DECLARE @T_Directories TABLE(T_Index INT IDENTITY(1,1),T_DirectoryID INT,T_DirectoryName NVARCHAR(256) COLLATE Chinese_PRC_CI_AS_WS DEFAULT '')
	DECLARE @T_DiskFiles TABLE(T_Index INT IDENTITY(1,1),T_DiskFileID INT,T_DiskFileName NVARCHAR(256) COLLATE Chinese_PRC_CI_AS_WS DEFAULT '')
	
	INSERT INTO @T_Directories(T_DirectoryID)
	SELECT item FROM bx_GetBigIntTable(@DirectoryIDs,'|')

	UPDATE @T_Directories
	SET T_DirectoryName=item
	FROM bx_GetStringTable_nvarchar(@DirectoryNames,'|')
	WHERE T_Index=id


	INSERT INTO @T_DiskFiles(T_DiskFileID)
	SELECT item FROM bx_GetIntTable(@DiskFileIDs,'|')

	UPDATE @T_DiskFiles
	SET T_DiskFileName=item
	FROM bx_GetStringTable_nvarchar(@DiskFileNames,'|')
	WHERE T_Index=id

	------׼�����
	--���
	IF NOT EXISTS(SELECT * FROM @T_Directories AS M INNER JOIN @T_DiskFiles AS S ON M.T_DirectoryName=S.T_DiskFileName)
	BEGIN
		IF EXISTS (
		SELECT * FROM bx_DiskDirectories 
		INNER JOIN @T_Directories
		ON [Name] = T_DirectoryName
		WHERE [UserID] = @UserID AND ParentID = @ParentID AND DirectoryID NOT IN (SELECT T_DirectoryID FROM @T_Directories))
		OR EXISTS (
		SELECT * FROM [bx_DiskFiles]
		INNER JOIN @T_Directories
		ON [FileName] = T_DirectoryName
		WHERE [DirectoryID] = @ParentID AND DiskFileID NOT IN (SELECT T_DiskFileID FROM @T_DiskFiles))
		OR EXISTS (
		SELECT * FROM bx_DiskDirectories 
		INNER JOIN @T_DiskFiles
		ON [Name] = T_DiskFileName
		WHERE [UserID] = @UserID AND ParentID = @ParentID AND DirectoryID NOT IN (SELECT T_DirectoryID FROM @T_Directories))
		OR EXISTS (
		SELECT * FROM [bx_DiskFiles]
		INNER JOIN @T_DiskFiles
		ON [FileName] = T_DiskFileName
		WHERE [DirectoryID] = @ParentID AND DiskFileID NOT IN (SELECT T_DiskFileID FROM @T_DiskFiles))
		--������ڳ��Լ�֮����ļ�������Ŀ¼��
		RETURN 3
		--����Ŀ¼
		BEGIN TRANSACTION
		SELECT * FROM @T_Directories
		UPDATE bx_DiskDirectories 
		SET [Name] = RAND()+T_DirectoryID--��ʱ����
		FROM @T_Directories
		WHERE DirectoryID = T_DirectoryID
		IF @@ERROR<>0
		BEGIN
			ROLLBACK TRANSACTION
			RETURN -1
		END

		UPDATE bx_DiskDirectories 
		SET [Name] = T_DirectoryName
		FROM @T_Directories
		WHERE DirectoryID = T_DirectoryID
		IF @@ERROR<>0
		BEGIN
			ROLLBACK TRANSACTION
			RETURN -1
		END
		--�����ļ�
		UPDATE bx_DiskFiles 
		SET [FileName] = RAND()+T_DiskFileID--��ʱ����
		FROM @T_DiskFiles
		WHERE DiskFileID = T_DiskFileID		
		IF @@ERROR<>0
		BEGIN
			ROLLBACK TRANSACTION
			RETURN -1
		END

		UPDATE bx_DiskFiles 
		SET [FileName] = T_DiskFileName
		FROM @T_DiskFiles
		WHERE DiskFileID = T_DiskFileID	
		IF @@ERROR<>0
		BEGIN
			ROLLBACK TRANSACTION
			RETURN -1
		END

		COMMIT TRANSACTION
		RETURN 0
	END

")]

        #endregion
        public override CreateUpdateDiskFileStatus RenameDiskDirectoryAndFiles(int userID, int parentID, IEnumerable<int> directoryIds, IEnumerable<int> diskFileIds, IEnumerable<string> names, IEnumerable<string> diskFileNames)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_RenameDiskDirectoriesAndDiskFiles";

                query.CreateParameter("@UserID", userID, SqlDbType.Int);
                query.CreateParameter("@ParentID", parentID, SqlDbType.Int);
                query.CreateParameter("@DirectoryIDs", StringUtil.Join(directoryIds, "|"), SqlDbType.VarChar, 8000);
                query.CreateParameter("@DiskFileIDs", StringUtil.Join(diskFileIds, "|"), SqlDbType.VarChar, 8000);
                query.CreateParameter("@DirectoryNames", StringUtil.Join(names, "|"), SqlDbType.NVarChar, 4000);
                query.CreateParameter("@DiskFileNames", StringUtil.Join(diskFileNames, "|"), SqlDbType.NVarChar, 4000);
                SqlParameter returnParam = query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (CreateUpdateDiskFileStatus)Convert.ToInt32(returnParam.Value);
            }
        }

        public override void GetDiskDirectoriesAndDiskFiles(int userID, int directoryID, IEnumerable<int> directoryIds, IEnumerable<int> diskFileIds, out DiskDirectoryCollection directories, out DiskFileCollection diskFiles)
        {
            directories = null; ;
            diskFiles = null;

            bool hasDirectories = ValidateUtil.HasItems(directoryIds);
            bool hasFiles = ValidateUtil.HasItems(diskFileIds);

            StringBuilder sqlBuilder = new StringBuilder();

            using (SqlQuery query = new SqlQuery())
            {
                if (hasDirectories)
                {
                    sqlBuilder.Append("SELECT * FROM bx_DiskDirectories WHERE DirectoryID IN (@DirectoryIds) ORDER BY Name;");
                    query.CreateInParameter("@DirectoryIds", directoryIds);
                }

                if (hasFiles)
                {
                    sqlBuilder.Append("SELECT * FROM bx_DiskFiles WHERE DiskFileID IN (@DiskFileIds) ORDER BY FileName;");
                    query.CreateInParameter("@DiskFileIds", diskFileIds);
                }

                query.CommandType = CommandType.Text;
                query.CommandText = sqlBuilder.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (hasDirectories)
                    {
                        directories = new DiskDirectoryCollection(reader);
                        //while (reader.Read())
                        //{
                        //    directories.Add(new DiskDirectory(reader));
                        //}
                    }

                    if (hasFiles)
                    {
                        if (hasDirectories)
                            reader.NextResult();

                        diskFiles = new DiskFileCollection(reader);
                        //while (reader.Read())
                        //{
                        //    diskFiles.Add(new DiskFile(reader));
                        //}
                    }
                }
            }

            if (directories == null)
                directories = new DiskDirectoryCollection();

            if (diskFiles == null)
                diskFiles = new DiskFileCollection();
        }


        #endregion
    }
}