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
using System.Data.SqlClient;
using System.Collections.Generic;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using System.Collections;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class AlbumDao : DataAccess.AlbumDao
    {
        #region =========↓相册↓====================================================================================================

        #region Stored Procedure
        [StoredProcedure(Name = "bx_Album_CreateAlbum", Script = @"
CREATE PROCEDURE {name}
      @UserID         int
     ,@PrivacyType    tinyint
     ,@Name           nvarchar(50)
    ,@Description       nvarchar(100)
     ,@Cover          nvarchar(200)
     ,@Password       nvarchar(50)
AS BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS(SELECT * FROM bx_Albums WHERE UserID = @UserID AND Name = @Name) BEGIN
        SELECT -1;
        RETURN;
    END

    INSERT INTO
        [bx_Albums](
             [UserID]
            ,[PrivacyType]
            ,[Name]
            ,[Description]
            ,[Cover]
            ,[Password]
			,[LastEditUserID]
        ) VALUES (
             @UserID
            ,@PrivacyType
            ,@Name
            ,@Description
            ,@Cover
            ,@Password
			,@UserID
        );

    SELECT @@IDENTITY;
END")]
        #endregion
        public override int CreateAlbum(int userID, PrivacyType privacyType, string name, string description, string cover, string password)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_CreateAlbum";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<PrivacyType>("@PrivacyType", privacyType, SqlDbType.TinyInt);
                query.CreateParameter<string>("@Name", name, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 100);
                query.CreateParameter<string>("@Cover", cover, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@Password", password, SqlDbType.NVarChar, 50);


                return query.ExecuteScalar<int>();
            }
        }

        #region Stored Procedure
        [StoredProcedure(Name = "bx_Album_UpdateAlbum", Script = @"
CREATE PROCEDURE {name}
     @AlbumID              int
    ,@Name            nvarchar(50)
    ,@Description       nvarchar(100)
    ,@PrivacyType     tinyint
    ,@Password        nvarchar(50)
    ,@LastEditUserID    int
AS BEGIN
    SET NOCOUNT ON;

    UPDATE 
        [bx_Albums]
    SET
        [Name] = @Name
        ,[Description] = @Description
       ,[KeywordVersion] = N''
       ,[PrivacyType] = @PrivacyType
       ,[Password] = @Password
       ,[UpdateDate] = GETDATE()
       ,[LastEditUserID] = @LastEditUserID
    WHERE
        [AlbumID] = @AlbumID;

	UPDATE
		[bx_Photos]
	SET
		[LastEditUserID] = @LastEditUserID
	WHERE
		[AlbumID] = @AlbumID;
END")]
        #endregion
        public override bool UpdateAlbum(int albumID, string name, string description, PrivacyType privacyType, string password, int editUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_UpdateAlbum";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@AlbumID", albumID, SqlDbType.Int);
                query.CreateParameter<string>("@Name", name, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 100);
                query.CreateParameter<PrivacyType>("@PrivacyType", privacyType, SqlDbType.TinyInt);
                query.CreateParameter<string>("@Password", password, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@LastEditUserID", editUserID, SqlDbType.Int);

                query.ExecuteNonQuery();

                return true;
            }
        }

        #region Stored Procedure
        [StoredProcedure(Name = "bx_Album_UpdateAlbumCover", Script = @"
CREATE PROCEDURE {name}
     @AlbumID           int
    ,@PhotoID           int
    ,@LastEditUserID    int
AS BEGIN
    SET NOCOUNT ON;

    UPDATE [bx_Albums] SET [Cover]=[ThumbPath],[LastEditUserID]=@LastEditUserID,[CoverPhotoID]=@PhotoID FROM [bx_Photos] WHERE [bx_Albums].[AlbumID]=@AlbumID AND [PhotoID]=@PhotoID;
     
	UPDATE [bx_Photos] SET [LastEditUserID] = @LastEditUserID WHERE [AlbumID] = @AlbumID;
END")]
        #endregion
        public override bool UpdateAlbumCover(int editUserID, int albumID, int coverPhotoID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_UpdateAlbumCover";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@AlbumID", albumID, SqlDbType.Int);
                query.CreateParameter<int>("@PhotoID", coverPhotoID, SqlDbType.Int);
                query.CreateParameter<int>("@LastEditUserID", editUserID, SqlDbType.Int);

                query.ExecuteNonQuery();

                return true;
            }
        }

        /**************************************
         *       Get开头的函数获取数据        *
         **************************************/

        #region Stored Procedure
        [StoredProcedure(Name = "bx_Album_GetAlbum", Script = @"
CREATE PROCEDURE {name}
    @AlbumID              int
AS BEGIN
    SET NOCOUNT ON;

    SELECT 
        *
    FROM
        [bx_Albums]
    WHERE
        [AlbumID]=@AlbumID;
END")]
        #endregion
        public override Album GetAlbum(int albumID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_GetAlbum";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@AlbumID", albumID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Album(reader);
                    }
                    return null;
                    //return new Album();
                }
            }
        }

        #region Stored Procedure
        [StoredProcedure(Name = "bx_Album_GetAlbumPhotoIDs", Script = @"
CREATE PROCEDURE {name}
    @AlbumID              int
AS BEGIN
    SET NOCOUNT ON;

    SELECT [PhotoID] FROM [bx_Photos] WHERE [AlbumID] = @AlbumID ORDER BY [PhotoID] DESC;
END")]
        #endregion
        public override int[] GetAlbumPhotoIDs(int albumID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_GetAlbumPhotoIDs";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@AlbumID", albumID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    List<int> ids = new List<int>();

                    while (reader.Read())
                    {
                        ids.Add(reader.Get<int>(0));
                    }

                    return ids.ToArray();
                }
            }
        }

        #region Stored Procedure
        [StoredProcedure(Name = "bx_Album_GetUserAlbums_All", Script = @"
CREATE PROCEDURE {name}
    @UserID              int
AS BEGIN
    SET NOCOUNT ON;

    SELECT * FROM [bx_Albums] WHERE [UserID] = @UserID;
END")]
        #endregion
        public override AlbumCollection GetUserAlbums(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_GetUserAlbums_All";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AlbumCollection(reader);
                }
            }
        }

        public override AlbumCollection GetUserAlbums(int userID, DataAccessLevel dataAccessLevel, int pageNumber, int pageSize, ref int? totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_Albums]";
                query.Pager.SortField = "[UpdateDate]";
                query.Pager.PrimaryKey = "AlbumID";
                query.Pager.IsDesc = true;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;

                query.Pager.Condition = "[UserID] = @UserID";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                if (dataAccessLevel == DataAccessLevel.Normal)
                {
                    query.Pager.Condition += " AND [PrivacyType] IN (0, 3)";
                }
                else if (dataAccessLevel == DataAccessLevel.Friend)
                {
                    query.Pager.Condition += " AND [PrivacyType] IN (0, 1, 3)";
                }

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    AlbumCollection albums = new AlbumCollection(reader);

                    if (totalCount == null && reader.NextResult() && reader.Read())
                    {
                        totalCount = reader.Get<int>(0);
                    }

                    albums.TotalRecords = totalCount.GetValueOrDefault();

                    return albums;
                }
            }
        }

        public override AlbumCollection GetFriendAlbums(int userID, int pageNumber, int pageSize)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Albums";
                query.Pager.SortField = "UpdateDate";
                query.Pager.PrimaryKey = "AlbumID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.Condition = "[UserID] IN (SELECT [FriendUserID] FROM [bx_Friends] WHERE [UserID] = @UserID) AND ([PrivacyType] IN (0,1,3))";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    AlbumCollection albums = new AlbumCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            albums.TotalRecords = reader.Get<int>(0);
                        }
                    }
                    return albums;
                }
            }
        }

        public override AlbumCollection GetEveryoneAlbums(int pageNumber, int pageSize, ref int? totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_Albums]";
                query.Pager.SortField = "[UpdateDate]";
                query.Pager.PrimaryKey = "[AlbumID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.Condition = " ([PrivacyType] IN (0,3))";

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    AlbumCollection albums = new AlbumCollection(reader);

                    if (totalCount == null && reader.NextResult() && reader.Read())
                    {
                        totalCount = reader.Get<int>(0);
                    }

                    albums.TotalRecords = totalCount.GetValueOrDefault();

                    return albums;
                }
            }
        }

        public override AlbumCollection GetAlbumsBySearch(int operatorID, IEnumerable<Guid> excludeRoleIDs, AdminAlbumFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string sqlCondition = BuildConditionsByFilter(query, filter, operatorID, excludeRoleIDs, false);

                query.Pager.TableName = "bx_Albums";
                query.Pager.SortField = filter.Order.ToString();

                if (filter.Order != AdminAlbumFilter.OrderBy.AlbumID)
                {
                    query.Pager.PrimaryKey = "AlbumID";
                }

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SelectCount = true;
                query.Pager.IsDesc = filter.IsDesc;

                query.Pager.Condition = sqlCondition;


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    AlbumCollection albums = new AlbumCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            albums.TotalRecords = reader.Get<int>(0);
                        }
                    }

                    return albums;
                }
            }
        }

        /**************************************
         *      Delete开头的函数删除数据      *
         **************************************/

        public override DeleteResult DeleteAlbums(IEnumerable<int> albumIDs, int operatorID, IEnumerable<Guid> excludeRoleIDs, out int[] deletePhotoIDs)
        {

            using (SqlQuery query = new SqlQuery())
            {
                string excludeRolesSql = DaoUtil.GetExcludeRoleSQL("[UserID]", "[LastEditUserID]", operatorID, excludeRoleIDs, query);

                if (string.IsNullOrEmpty(excludeRolesSql) == false)
                    excludeRolesSql = " AND ([UserID] = @UserID OR " + excludeRolesSql + ")";

                string sql = @"
DECLARE @DeleteData table (UserID int, AlbumID int, PhotoCount int);

INSERT INTO @DeleteData SELECT [A].[UserID],[A].[AlbumID],(SELECT COUNT(*) FROM [bx_Photos] AS [B] WHERE [B].[AlbumID]=[A].[AlbumID]) FROM [bx_Albums] AS [A]  WHERE [A].[AlbumID] IN (@AlbumIDs)" + excludeRolesSql + @";

DECLARE @DeletePhotos table (PhotoID int);

INSERT INTO @DeletePhotos SELECT PhotoID from bx_Photos WHERE AlbumID IN (SELECT [AlbumID] FROM @DeleteData);

DELETE [bx_Albums] WHERE [AlbumID] IN (SELECT [AlbumID] FROM @DeleteData);

SELECT [UserID],SUM([PhotoCount]) AS [Count] FROM @DeleteData GROUP BY [UserID];

SELECT [PhotoID] FROM @DeletePhotos;";

                query.CommandText = sql;

                query.CreateInParameter<int>("@AlbumIDs", albumIDs);
                query.CreateParameter<int>("@UserID", operatorID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    //List<string> deletedFileIDList = new List<string>();

                    //while (reader.Read())
                    //{
                    //    deletedFileIDList.Add(reader.Get<string>(0));
                    //}

                    //deletedFileIDs = deletedFileIDList.ToArray();

                    //if (reader.NextResult())
                    //{
                    while (reader.Read())
                    {
                        deleteResult.Add(reader.Get<int>("UserID"), reader.Get<int>("Count"));
                    }

                    reader.NextResult();

                    List<int> deletePhotos = new List<int>();

                    while (reader.Read())
                    {
                        deletePhotos.Add(reader.Get<int>(0));
                    }

                    deletePhotoIDs = deletePhotos.ToArray();
                    //}

                    return deleteResult;
                }
            }
        }

        public override DeleteResult DeleteAlbumsBySearch(AdminAlbumFilter filter, int operatorID, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount, out int[] deletedPhotoIDs)
        {
            deletedCount = 0;

            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, operatorID, excludeRoleIDs, true);

                StringBuffer sql = new StringBuffer();

                sql += @"
DECLARE @DeleteData table (UserID int, AlbumID int, PhotoCount int);

INSERT INTO @DeleteData SELECT TOP (@TopCount) [UserID],[A].[AlbumID],(SELECT COUNT(*) FROM [bx_Photos] AS [B] WHERE [B].[AlbumID]=[A].[AlbumID]) FROM [bx_Albums] AS [A] " + conditions + @";

DECLARE @DeletePhotos table (PhotoID int);

INSERT INTO @DeletePhotos SELECT PhotoID from bx_Photos WHERE AlbumID IN (SELECT [AlbumID] FROM @DeleteData);

DELETE [bx_Albums] WHERE [AlbumID] IN (SELECT [AlbumID] FROM @DeleteData);

SELECT @@ROWCOUNT;

SELECT [UserID],SUM([PhotoCount]) AS [Count] FROM @DeleteData GROUP BY [UserID];

SELECT [PhotoID] FROM @DeletePhotos;";

                query.CreateTopParameter("@TopCount", topCount);

                query.CommandText = sql.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    if (reader.Read())
                        deletedCount = reader.Get<int>(0);

                    reader.NextResult();

                    while (reader.Read())
                    {
                        deleteResult.Add(reader.Get<int>("UserID"), reader.Get<int>("Count"));
                    }

                    reader.NextResult();

                    List<int> deletePhotos = new List<int>();

                    while (reader.Read())
                    {
                        deletePhotos.Add(reader.Get<int>(0));
                    }

                    deletedPhotoIDs = deletePhotos.ToArray();

                    return deleteResult;
                }
            }
        }

        private string BuildConditionsByFilter(SqlQuery query, AdminAlbumFilter filter, int operatorUserID, IEnumerable<Guid> excludeRoleIds, bool startWithWhere)
        {

            StringBuffer sqlConditions = new StringBuffer();

            if (filter.AlbumID != null)
            {
                sqlConditions += " AND [AlbumID] = @AlbumID";
                query.CreateParameter<int?>("@AlbumID", filter.AlbumID, SqlDbType.Int);
            }
            if (filter.AuthorID != null)
            {
                sqlConditions += " AND [UserID] = @UserID";
                query.CreateParameter<int?>("@UserID", filter.AuthorID, SqlDbType.Int);
            }
            if (filter.PrivacyType != null)
            {
                sqlConditions += " AND [PrivacyType] = @PrivacyType";
                query.CreateParameter<PrivacyType?>("@PrivacyType", filter.PrivacyType, SqlDbType.TinyInt);
            }
            if (filter.BeginDate != null)
            {
                sqlConditions += " AND [CreateDate] >= @BeginDate";
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
            }
            if (filter.EndDate != null)
            {
                sqlConditions += " AND [CreateDate] <= @EndDate";
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);
            }
            if (string.IsNullOrEmpty(filter.Username) == false)
            {
                sqlConditions += " AND [UserID] = (SELECT [UserID] FROM [bx_Users] WHERE [Username] = @Username)";
                query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
            }
            if (string.IsNullOrEmpty(filter.Name) == false)
            {
                sqlConditions += " AND ([Name] LIKE '%' + @Key + '%')";
                query.CreateParameter<string>("@Key", filter.Name, SqlDbType.NVarChar, 50);
            }

            string excludeRolesSql = DaoUtil.GetExcludeRoleSQL("[UserID]", "[LastEditUserID]", operatorUserID, excludeRoleIds, query);

            if (string.IsNullOrEmpty(excludeRolesSql) == false)
                sqlConditions += " AND " + excludeRolesSql;

            if (sqlConditions.Length > 0)
            {
                sqlConditions.Remove(0, 5);
                if (startWithWhere)
                    sqlConditions.InnerBuilder.Insert(0, " WHERE ");
            }

            return sqlConditions.ToString();
        }

        #endregion

        #region =========↓相片↓====================================================================================================

        //public override IEnumerable<string> GetUsedFiles(IEnumerable<string> fileIDs)
        //{
        //    using (SqlQuery query = new SqlQuery())
        //    {
        //        query.CommandText = "SELECT FileID FROM bx_Photos WHERE FileID IN (@FileIDs);";

        //        query.CreateInParameter<string>("@FileIDs", fileIDs);

        //        using (XSqlDataReader reader = query.ExecuteReader())
        //        {
        //            List<string> result = new List<string>();

        //            while (reader.Read())
        //            {
        //                result.Add(reader.GetString(0));
        //            }

        //            return result;
        //        }
        //    }
        //}

        #region Stored Procedure
        [StoredProcedure(Name = "bx_Album_CreatePhotos", Script = @"
CREATE PROCEDURE {name}
	@AlbumID				int
	,@UserID				int
	,@UserIP				varchar(50)
	,@PhotoNames			ntext
	,@PhotoDescriptions		ntext
	,@PhotoFileTypes        text
	,@PhotoFileIds			text
    ,@PhotoFileSizes	    text
AS BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM [bx_Albums] WHERE [UserID]=@UserID AND [AlbumID]=@AlbumID) BEGIN

		IF DATALENGTH(@PhotoFileIds) > 0 BEGIN

			DECLARE @PhotoTable table(TempID int identity(1,1), Name nvarchar(50), Description nvarchar(1500) default(''), Type varchar(10), FileID varchar(50), FileSize bigint);

			INSERT INTO @PhotoTable (Name) SELECT item FROM bx_GetStringTable_ntext(@PhotoNames, N'/');

			UPDATE @PhotoTable SET [Description] = T.item FROM bx_GetStringTable_ntext(@PhotoDescriptions, N'/') T WHERE TempID = T.id;

			UPDATE @PhotoTable SET [Type] = T.item FROM bx_GetStringTable_text(@PhotoFileTypes, '/') T WHERE TempID = T.id;

			UPDATE @PhotoTable SET FileID = T.item FROM bx_GetStringTable_text(@PhotoFileIds, '/') T WHERE TempID = T.id;

            UPDATE @PhotoTable SET FileSize = T.item FROM bx_GetStringTable_text(@PhotoFileSizes, '/') T WHERE TempID = T.id;
			--UPDATE @PhotoTable SET FileSize = (SELECT T.FileSize FROM bx_Files T WHERE T.ID = FileID);

			INSERT INTO [bx_Photos] ([AlbumID],[UserID],[CreateIP],[Name],[Description],[FileType],[FileID],[FileSize],[LastEditUserID])
			SELECT @AlbumID, @UserID, @UserIP, A.[Name], A.[Description], A.[Type], A.[FileID], A.[FileSize], @UserID FROM @PhotoTable AS A;
		END;

		RETURN 1;
	END;

	RETURN -1;
END")]
        #endregion
        public override bool CreatePhotos(int albumID, int userID, string userIP, string[] photoNames, string[] photoDescriptions, string[] fileTypes, string[] fileIds, long[] fileSizes, out int[] photoIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_CreatePhotos";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@AlbumID", albumID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@UserIP", userIP, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@PhotoNames", StringUtil.Join(photoNames, "/"), SqlDbType.NText);
                query.CreateParameter<string>("@PhotoDescriptions", StringUtil.Join(photoDescriptions, "/"), SqlDbType.NText);
                query.CreateParameter<string>("@PhotoFileTypes", StringUtil.Join(fileTypes, "/"), SqlDbType.Text);
                query.CreateParameter<string>("@PhotoFileIds", StringUtil.Join(fileIds, "/"), SqlDbType.Text);
                query.CreateParameter<string>("@PhotoFileSizes", StringUtil.Join(fileSizes, "/"), SqlDbType.Text);

                SqlParameter result = query.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    List<int> photoIDList = new List<int>();

                    while (reader.Read())
                    {
                        photoIDList.Add(reader.Get<int>(0));
                    }

                    photoIds = photoIDList.ToArray();
                }

                return (int)result.Value == 1;
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Album_UpdatePhoto", Script = @"
CREATE PROCEDURE {name}
    @PhotoID           int
   ,@Name              nvarchar(50)
   ,@Description       nvarchar(1500)
   ,@LastEditUserID    int
AS BEGIN
    SET NOCOUNT ON;

    UPDATE 
        [bx_Photos] 
    SET 
         [Name] = @Name
        ,[Description] = @Description
        ,[KeywordVersion] = N''
        ,[UpdateDate] = GETDATE()
		,[LastEditUserID] = @LastEditUserID
    WHERE
        [PhotoID] = @PhotoID;

	DECLARE @AlbumID int;

	SELECT @AlbumID = AlbumID FROM [bx_Photos] WHERE [PhotoID] = @PhotoID;

	UPDATE [bx_Albums] SET [LastEditUserID] = @LastEditUserID WHERE AlbumID = @AlbumID;
END")]
        #endregion
        public override bool UpdatePhoto(int photoID, string photoName, string photoDescription, int editUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_UpdatePhoto";

                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@PhotoID", photoID, SqlDbType.Int);
                query.CreateParameter<string>("@Name", photoName, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Description", photoDescription, SqlDbType.NVarChar, 1500);
                query.CreateParameter<int>("@LastEditUserID", editUserID, SqlDbType.Int);

                query.ExecuteNonQuery();

                return true;
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Album_UpdatePhotos", Script = @"
CREATE PROCEDURE {name}
    @AlbumID            int
   ,@PhotoIDs           varchar(8000)
   ,@PhotoNames         ntext
   ,@PhotoDescs         ntext
   ,@LastEditUserID     int
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @TempData table(TempID int identity(1,1), _PhotoID int, _PhotoName varchar(50), _PhotoDesc varchar(1500));

	INSERT INTO @TempData (_PhotoID) SELECT item FROM bx_GetIntTable(@PhotoIDs, '|');

	IF NOT EXISTS(SELECT * FROM (SELECT * FROM [bx_Photos] WHERE PhotoID IN (SELECT _PhotoID FROM @TempData)) A WHERE A.AlbumID != @AlbumID) BEGIN

		UPDATE @TempData SET _PhotoName = T.item FROM bx_GetStringTable_ntext(@PhotoNames, '|') T WHERE TempID = T.id;

		UPDATE @TempData SET _PhotoDesc = T.item FROM bx_GetStringTable_ntext(@PhotoDescs, '|') T WHERE TempID = T.id;

		UPDATE [bx_Photos] SET [Name] = _PhotoName, [Description] = ISNULL(_PhotoDesc,''), [LastEditUserID] = @LastEditUserID FROM @TempData WHERE AlbumID=@AlbumID AND PhotoID = _PhotoID;

		UPDATE [bx_Albums] SET [LastEditUserID] = @LastEditUserID WHERE AlbumID = @AlbumID;

		RETURN 1;
	END;

	RETURN 0;
END")]
        #endregion
        public override bool UpdatePhotos(int albumID, int[] photoIDs, string[] photoNames, string[] photoDescs, int editUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_UpdatePhotos";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@AlbumID", albumID, SqlDbType.Int);
                query.CreateParameter<string>("@PhotoIDs", StringUtil.Join(photoIDs, "|"), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@PhotoNames", StringUtil.Join(photoNames, "|"), SqlDbType.NText);
                query.CreateParameter<string>("@PhotoDescs", StringUtil.Join(photoDescs, "|"), SqlDbType.NText);
                query.CreateParameter<int>("@LastEditUserID", editUserID, SqlDbType.Int);

                SqlParameter result = query.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (int)result.Value == 1;
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Album_UpdatePhotoThumbInfos", Script = @"
CREATE PROCEDURE {name}
    @PhotoIDs           varchar(8000)
   ,@ThumbPaths         text
   ,@ThumbWidths        varchar(8000)
   ,@ThumbHeights       varchar(8000)
   ,@AsAlbumCovers      varchar(8000)
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @TempData table(TempID int identity(1,1), PID int, Path varchar(256), Width int, Height int, AsAlbumCover int);

	INSERT INTO @TempData (PID) SELECT item FROM bx_GetIntTable(@PhotoIDs, '|');

	UPDATE @TempData SET Path = T.item FROM bx_GetStringTable_text(@ThumbPaths, '|') T WHERE TempID = T.id;

	UPDATE @TempData SET Width = T.item FROM bx_GetIntTable(@ThumbWidths, '|') T WHERE TempID = T.id;

	UPDATE @TempData SET Height = T.item FROM bx_GetIntTable(@ThumbHeights, '|') T WHERE TempID = T.id;

	UPDATE @TempData SET AsAlbumCover = T.item FROM bx_GetIntTable(@AsAlbumCovers, '|') T WHERE TempID = T.id;

	UPDATE [bx_Photos] SET ThumbPath = T.Path, ThumbWidth = T.Width, ThumbHeight = T.Height FROM @TempData T WHERE PhotoID = T.PID;

	DECLARE @Cover AS varchar(256);

	SELECT TOP 1 @Cover = Path FROM @TempData ORDER BY TempID DESC;

	UPDATE [bx_Albums] SET [Cover] = ThumbPath, [CoverPhotoID] = [PhotoID] FROM [bx_Photos] WHERE [PhotoID] IN (SELECT PID FROM @TempData WHERE AsAlbumCover = 1) AND [bx_Albums].AlbumID = [bx_Photos].AlbumID;

END")]
        #endregion
        public override void UpdatePhotoThumbInfos(int[] photoIDs, string[] thumbPaths, int[] thumbWidths, int[] thumbHeights, bool[] asAlbumCovers)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuffer asAlbumCoversValue = new StringBuffer();

                for (int i = 0; i < asAlbumCovers.Length; i++)
                {
                    asAlbumCoversValue += asAlbumCovers[i] ? "1" : "0";

                    if (i < asAlbumCovers.Length - 1)
                        asAlbumCoversValue += "|";
                }

                query.CommandText = "bx_Album_UpdatePhotoThumbInfos";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@PhotoIDs", StringUtil.Join(photoIDs, "|"), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@ThumbPaths", StringUtil.Join(thumbPaths, "|"), SqlDbType.Text);
                query.CreateParameter<string>("@ThumbWidths", StringUtil.Join(thumbWidths, "|"), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@ThumbHeights", StringUtil.Join(thumbHeights, "|"), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@AsAlbumCovers", asAlbumCoversValue.ToString(), SqlDbType.VarChar, 8000);

                query.ExecuteNonQuery();
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Album_MovePhotos", Script = @"
CREATE PROCEDURE {name}
    @SrcAlbumID         int
   ,@DesAlbumID         int
   ,@PhotoIDs           varchar(8000)
   ,@LastEditUserID     int
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @TempData table(TempID int identity(1,1), _PhotoID int);

	INSERT INTO @TempData (_PhotoID) SELECT item FROM bx_GetIntTable(@PhotoIDs, '|');

	--相片和相册都存在才执行操作

	IF NOT EXISTS(SELECT * FROM (SELECT * FROM [bx_Photos] WHERE PhotoID IN (SELECT _PhotoID FROM @TempData)) A WHERE A.AlbumID != @SrcAlbumID) BEGIN

		--如果移动的图片中包含了相册封面，则将相册设置成无封面

		UPDATE [bx_Albums] SET [CoverPhotoID] = NULL, [Cover] = '' WHERE AlbumID = @SrcAlbumID AND CoverPhotoID IN (SELECT _PhotoID FROM @TempData);

		--两个相册的最后编辑者都更新为当前操作者

		UPDATE [bx_Albums] SET [LastEditUserID] = @LastEditUserID WHERE AlbumID IN (@SrcAlbumID, @DesAlbumID);

		UPDATE [bx_Photos] SET [AlbumID] = @DesAlbumID, [LastEditUserID] = @LastEditUserID FROM @TempData WHERE AlbumID = @SrcAlbumID AND PhotoID = _PhotoID;

		RETURN 1;
	END;

	RETURN 0;
END")]
        #endregion
        public override bool MovePhotos(int srcAlbumID, int desAlbumID, int[] photoIDs, int editUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_MovePhotos";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@SrcAlbumID", srcAlbumID, SqlDbType.Int);
                query.CreateParameter<int>("@DesAlbumID", desAlbumID, SqlDbType.Int);
                query.CreateParameter<string>("@PhotoIDs", StringUtil.Join(photoIDs, "|"), SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@LastEditUserID", editUserID, SqlDbType.Int);

                SqlParameter result = query.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (int)result.Value == 1;
            }
        }

        /**************************************
         *       Get开头的函数获取数据        *
         **************************************/

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Album_GetUploadPhotoCount", Script = @"
CREATE PROCEDURE {name}
	@UserID		int,
	@BeginDate	datetime,
	@EndDate	datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_Photos WHERE [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate;
END")]
        #endregion
        public override int GetUploadPhotoCount(int userID, DateTime beginDate, DateTime endDate)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Album_GetUploadPhotoCount";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
                db.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);

                return db.ExecuteScalar<int>();
            }
        }

        #region Stored Procedure
        [StoredProcedure(Name = "bx_Album_GetPhoto", Script = @"
CREATE PROCEDURE {name}
    @PhotoID       int
AS BEGIN
    SET NOCOUNT ON;

    SELECT
        *
    FROM
        [bx_Photos]
    WHERE
        [PhotoID] = @PhotoID;
END")]
        #endregion
        public override Photo GetPhoto(int photoID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Album_GetPhoto";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@PhotoID", photoID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new Photo(reader);

                    return null;
                }
            }
        }

        public override Hashtable GetPhotos(int[] albumids, int count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                for (int i = 0; i < albumids.Length; i++)
                {
                    query.CommandText += "\r\nSELECT TOP (@Count) * FROM bx_Photos WHERE AlbumID = @AlbumID_" + i;

                    query.CreateParameter<int>("@AlbumID_" + i, albumids[i], SqlDbType.Int);
                }

                query.CreateTopParameter("@Count", count);

                Hashtable result = new Hashtable();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    int lastAlbumID = -1;
                    PhotoCollection list = null;

                    do
                    {
                        while (reader.Read())
                        {
                            Photo photo = new Photo(reader);

                            if (lastAlbumID != photo.AlbumID)
                            {
                                list = new PhotoCollection();
                                lastAlbumID = photo.AlbumID;

                                result.Add(lastAlbumID, list);
                            }

                            list.Add(photo);
                        }

                    } while (reader.NextResult());

                    return result;
                }
            }
        }

        public override PhotoCollection GetPhotos(int albumID, int[] photodis, int photoPageNumber, int photoPageSize, ref int? totalPhotoCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_Photos]";
                query.Pager.SortField = "[PhotoID]";
                query.Pager.IsDesc = true;
                query.Pager.PageNumber = photoPageNumber;
                query.Pager.PageSize = photoPageSize;
                query.Pager.TotalRecords = totalPhotoCount;
                query.Pager.SelectCount = true;
                query.Pager.Condition = "[AlbumID] = @AlbumID";

                query.CreateParameter<int>("@AlbumID", albumID, SqlDbType.Int);

                if (photodis != null && photodis.Length > 0)
                {
                    query.Pager.Condition += " AND [PhotoID] IN(" + StringUtil.Join(photodis) + ")";
                }

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PhotoCollection photos = new PhotoCollection(reader);

                    if (totalPhotoCount == null && reader.NextResult() && reader.Read())
                    {
                        totalPhotoCount = reader.Get<int>(0);
                    }

                    photos.TotalRecords = totalPhotoCount.GetValueOrDefault();

                    return photos;
                }
            }
        }

        public override PhotoCollection GetPhotosBySearch(AdminPhotoFilter filter, int operatorID, IEnumerable<Guid> excludeRoleIDs, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string sqlCondition = BuildConditionsByFilter(query, filter, operatorID, excludeRoleIDs, false);

                query.Pager.TableName = "bx_Photos";
                query.Pager.SortField = filter.Order.ToString();

                if (filter.Order != AdminPhotoFilter.OrderBy.PhotoID)
                {
                    query.Pager.PrimaryKey = "PhotoID";
                }

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SelectCount = true;
                query.Pager.IsDesc = filter.IsDesc;

                query.Pager.Condition = sqlCondition;


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PhotoCollection photos = new PhotoCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            photos.TotalRecords = reader.Get<int>(0);
                        }
                    }

                    return photos;
                }
            }
        }

        /**************************************
         *      Delete开头的函数删除数据      *
         **************************************/

        public override DeleteResult DeletePhotos(IEnumerable<int> deleteIDs, int operatorID, IEnumerable<Guid> excludeRoleIDs, out int[] deletedPhotoIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string excludeRolesSql = DaoUtil.GetExcludeRoleSQL("[UserID]", "[LastEditUserID]", operatorID, excludeRoleIDs, query);

                if (string.IsNullOrEmpty(excludeRolesSql) == false)
                    excludeRolesSql = " AND ([UserID] = @UserID OR " + excludeRolesSql + ")";

                string sql = @"
DECLARE @DeleteData table (UserID int, PhotoID int);

INSERT INTO @DeleteData SELECT [UserID],[PhotoID] FROM [bx_Photos] WHERE [PhotoID] IN (@PhotoIDs)" + excludeRolesSql + @";

UPDATE [bx_Albums] SET [CoverPhotoID] = NULL, [Cover] = '' WHERE CoverPhotoID IN (SELECT [PhotoID] FROM @DeleteData);

DELETE [bx_Photos] WHERE PhotoID IN (SELECT [PhotoID] FROM @DeleteData);

SELECT [UserID],COUNT(*) AS [Count] FROM @DeleteData GROUP BY [UserID];

SELECT [PhotoID] FROM @DeleteData";

                query.CommandText = sql;

                query.CreateInParameter<int>("@PhotoIDs", deleteIDs);
                query.CreateParameter<int>("@UserID", operatorID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    while (reader.Read())
                    {
                        deleteResult.Add(reader.Get<int>("UserID"), reader.Get<int>("Count"));
                    }

                    reader.NextResult();

                    List<int> deletedPhotos = new List<int>();

                    while (reader.Read())
                    {
                        deletedPhotos.Add(reader.Get<int>(0));
                    }

                    deletedPhotoIDs = deletedPhotos.ToArray();

                    return deleteResult;
                }
            }
        }

        public override DeleteResult DeletePhotosBySearch(AdminPhotoFilter filter, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount, out int[] deletedPhotoIDs)
        {
            deletedCount = 0;

            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, operatorUserID, excludeRoleIDs, true);

                StringBuffer sql = new StringBuffer();

                sql += @"
DECLARE @DeleteData table (UserID int, PhotoID int);

INSERT INTO @DeleteData SELECT TOP (@TopCount) [UserID],[PhotoID] FROM [bx_Photos] " + conditions + @";

UPDATE [bx_Albums] SET [CoverPhotoID] = NULL, [Cover] = '' WHERE CoverPhotoID IN (SELECT [PhotoID] FROM @DeleteData);

DELETE [bx_Photos] WHERE PhotoID IN (SELECT [PhotoID] FROM @DeleteData);

SELECT @@ROWCOUNT;

SELECT [UserID],COUNT(*) AS [Count] FROM @DeleteData GROUP BY [UserID];

SELECT [PhotoID] FROM @DeleteData";

                query.CreateTopParameter("@TopCount", topCount);

                query.CommandText = sql.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    if (reader.Read())
                        deletedCount = reader.Get<int>(0);

                    while (reader.Read())
                    {
                        deleteResult.Add(reader.Get<int>("UserID"), reader.Get<int>("Count"));
                    }

                    reader.NextResult();

                    List<int> deletedPhotos = new List<int>();

                    while (reader.Read())
                    {
                        deletedPhotos.Add(reader.Get<int>(0));
                    }

                    deletedPhotoIDs = deletedPhotos.ToArray();

                    return deleteResult;
                }
            }

        }

        private string BuildConditionsByFilter(SqlQuery query, AdminPhotoFilter filter, int operatorUserID, IEnumerable<Guid> excludeRoleIds, bool startWithWhere)
        {

            StringBuffer sqlConditions = new StringBuffer();

            if (filter.PhotoID != null)
            {
                sqlConditions += " AND [PhotoID] = @PhotoID";
                query.CreateParameter<int?>("@PhotoID", filter.PhotoID, SqlDbType.Int);
            }
            if (filter.AuthorID != null)
            {
                sqlConditions += " AND [UserID] = @UserID";
                query.CreateParameter<int?>("@UserID", filter.AuthorID, SqlDbType.Int);
            }
            if (filter.AlbumID != null)
            {
                sqlConditions += " AND [AlbumID] = @AlbumID";
                query.CreateParameter<int?>("@AlbumID", filter.AlbumID, SqlDbType.Int);
            }
            if (filter.BeginDate != null)
            {
                sqlConditions += " AND [CreateDate] >= @BeginDate";
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
            }
            if (filter.EndDate != null)
            {
                sqlConditions += " AND [CreateDate] <= @EndDate";
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);
            }
            if (string.IsNullOrEmpty(filter.Username) == false)
            {
                sqlConditions += " AND [UserID] = (SELECT [UserID] FROM [bx_Users] WHERE [Username] = @Username)";
                query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
            }
            if (string.IsNullOrEmpty(filter.CreateIP) == false)
            {
                sqlConditions += " AND [CreateIP] LIKE '%' + @CreateIP + '%'";
                query.CreateParameter<string>("@CreateIP", filter.CreateIP, SqlDbType.VarChar, 50);
            }
            if (string.IsNullOrEmpty(filter.SearchKey) == false)
            {
                sqlConditions += " AND ([Name] LIKE '%' + @Name + '%' OR [Description] LIKE '%' + @Name + '%')";
                query.CreateParameter<string>("@Name", filter.SearchKey, SqlDbType.NVarChar, 50);
            }

            string excludeRolesSql = DaoUtil.GetExcludeRoleSQL("[UserID]", "[LastEditUserID]", operatorUserID, excludeRoleIds, query);

            if (string.IsNullOrEmpty(excludeRolesSql) == false)
                sqlConditions += " AND " + excludeRolesSql;

            if (sqlConditions.Length > 0)
            {
                sqlConditions.Remove(0, 5);
                if (startWithWhere)
                    sqlConditions.InnerBuilder.Insert(0, " WHERE ");
            }

            return sqlConditions.ToString();
        }

        #endregion

        #region =========↓关键字↓==================================================================================================

        public override Revertable2Collection<Album> GetAlbumsWithReverters(IEnumerable<int> albumIds)
        {
            if (ValidateUtil.HasItems(albumIds) == false)
                return null;

            Revertable2Collection<Album> albums = new Revertable2Collection<Album>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"SELECT A.*,
NameReverter = ISNULL(R.NameReverter, ''),
DescriptionReverter = ISNULL(R.DescriptionReverter, '')
FROM bx_Albums A WITH(NOLOCK)
LEFT JOIN bx_AlbumReverters R WITH(NOLOCK) ON R.AlbumID = A.AlbumID
WHERE A.AlbumID IN (@AlbumIds)";

                query.CreateInParameter<int>("@AlbumIds", albumIds);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string nameReverter = reader.Get<string>("NameReverter");
                        string descriptionReverter = reader.Get<string>("DescriptionReverter");

                        Album album = new Album(reader);

                        albums.Add(album, nameReverter, descriptionReverter);
                    }
                }
            }

            return albums;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Album_UpdateAlbumKeywords", Script = @"
CREATE PROCEDURE {name}
    @AlbumID               int,
    @KeywordVersion        varchar(32),
    @Name                  nvarchar(50),
    @Description            nvarchar(100),
    @NameReverter          nvarchar(1500),
    @DescriptionReverter   nvarchar(2500)
AS BEGIN

/* include : Procedure_UpdateKeyword2.sql
    {PrimaryKey} = AlbumID
    {PrimaryKeyParam} = @AlbumID

    {Table} = bx_Albums

    {Text1} = Name
    {Text1Param} = @Name

    {Text2} = Description
    {Text2Param} = @Description

    {RevertersTable} = bx_AlbumReverters

    {Text1Reverter} = NameReverter
    {Text1ReverterParam} = @NameReverter

    {Text2Reverter} = DescriptionReverter
    {Text2ReverterParam} = @DescriptionReverter
*/

END")]
        #endregion
        public override void UpdateAlbumKeywords(Revertable2Collection<Album> processlist)
        {
            string procedure = "bx_Album_UpdateAlbumKeywords";
            string table = "bx_Albums";
            string primaryKey = "AlbumID";

            SqlDbType text1_Type = SqlDbType.NVarChar; int text1_Size = 50;
            SqlDbType reve1_Type = SqlDbType.NVarChar; int reve1_Size = 4000;
            SqlDbType text2_Type = SqlDbType.NVarChar; int text2_Size = 100;
            SqlDbType reve2_Type = SqlDbType.NVarChar; int reve2_Size = 4000;


            if (processlist == null || processlist.Count == 0)
                return;

            //有一部分项是不需要更新文本的（例如：只有版本或恢复信息发生了变化），把这部分的ID取出来，一次性更新以提高性能
            List<int> needUpdateButTextNotChangedIds = processlist.GetNeedUpdateButTextNotChangedKeys();

            using (SqlQuery query = new SqlQuery())
            {
                StringBuffer sql = new StringBuffer();

                //前面取出的可以一次性更新版本而无需更新文本的部分项，在此批量更新
                if (needUpdateButTextNotChangedIds.Count > 0)
                {
                    sql += @"UPDATE " + table + " SET KeywordVersion = @NewVersion WHERE " + primaryKey + " IN (@NeedUpdateButTextNotChangedIds);";

                    query.CreateParameter<string>("@NewVersion", processlist.Version, SqlDbType.VarChar, 32);
                    query.CreateInParameter("@NeedUpdateButTextNotChangedIds", needUpdateButTextNotChangedIds);
                }

                int i = 0;
                foreach (Revertable2<Album> item in processlist)
                {
                    //此项确实需要更新，且不只是版本发生了变化
                    if (item.NeedUpdate && item.OnlyVersionChanged == false)
                    {
                        sql.InnerBuilder.AppendFormat(@"EXEC {1} @ID_{0}, @KeywordVersion_{0}, @Text1_{0}, @Reverter1_{0}, @Text2_{0}, @Reverter2_{0};", i, procedure);

                        query.CreateParameter<int>("@ID_" + i, item.Value.GetKey(), SqlDbType.Int);

                        //如果文本1或文本2发生了变化，更新
                        if (item.Text1Changed || item.Text2Changed)
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, item.Value.KeywordVersion, SqlDbType.VarChar, 32);

                            //文本1发生了变化
                            if (item.Text1Changed)
                                query.CreateParameter<string>("@Text1_" + i, item.Value.Text1, text1_Type, text1_Size);
                            else
                                query.CreateParameter<string>("@Text1_" + i, null, text1_Type, text1_Size);

                            //文本2发生了变化
                            if (item.Text2Changed)
                                query.CreateParameter<string>("@Text2_" + i, item.Value.Text2, text2_Type, text2_Size);
                            else
                                query.CreateParameter<string>("@Text2_" + i, null, text2_Type, text2_Size);
                        }
                        else
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, null, SqlDbType.VarChar, 32);

                            query.CreateParameter<string>("@Text1_" + i, null, text1_Type, text1_Size);
                            query.CreateParameter<string>("@Text2_" + i, null, text2_Type, text2_Size);

                        }

                        //如果恢复信息1发生了变化，更新
                        if (item.Reverter1Changed)
                            query.CreateParameter<string>("@Reverter1_" + i, item.Reverter1, reve1_Type, reve1_Size);
                        else
                            query.CreateParameter<string>("@Reverter1_" + i, null, reve1_Type, reve1_Size);

                        //如果恢复信息2发生了变化，更新
                        if (item.Reverter2Changed)
                            query.CreateParameter<string>("@Reverter2_" + i, item.Reverter2, reve2_Type, reve2_Size);
                        else
                            query.CreateParameter<string>("@Reverter2_" + i, null, reve2_Type, reve2_Size);

                        i++;

                    }
                }

                query.CommandText = sql.ToString();
                query.ExecuteNonQuery();
            }
        }

        public override Revertable2Collection<Photo> GetPhotosWithReverters(IEnumerable<int> photoIds)
        {
            if (ValidateUtil.HasItems(photoIds) == false)
                return null;

            Revertable2Collection<Photo> photos = new Revertable2Collection<Photo>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"SELECT P.*,
NameReverter = ISNULL(R.NameReverter, ''),
DescriptionReverter = ISNULL(R.DescriptionReverter, '')
FROM bx_Photos P WITH(NOLOCK) LEFT JOIN bx_PhotoReverters R WITH(NOLOCK) ON P.PhotoID = R.PhotoID
WHERE P.PhotoID IN (@PhotoIds)";

                query.CreateInParameter<int>("@PhotoIds", photoIds);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string nameReverter = reader.Get<string>("NameReverter");
                        string descriptionReverter = reader.Get<string>("DescriptionReverter");
                        Photo photo = new Photo(reader);
                        photos.Add(photo, nameReverter, descriptionReverter);
                    }
                }
            }

            return photos;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Album_UpdatePhotoKeywords", Script = @"
CREATE PROCEDURE {name}
    @PhotoID               int,
    @KeywordVersion        varchar(32),
    @Name                  nvarchar(50),
    @NameReverter          nvarchar(1500),
    @Description           nvarchar(1500),
    @DescriptionReverter   nvarchar(2500)
AS BEGIN

/* include : Procedure_UpdateKeyword2.sql

    {PrimaryKey} = PhotoID
    {PrimaryKeyParam} = @PhotoID


    {Table} = bx_Photos
    {Text1} = Name
    {Text1Param} = @Name

    {Text2} = Description
    {Text2Param} = @Description


    {RevertersTable} = bx_PhotoReverters
    {Text1Reverter} = NameReverter
    {Text1ReverterParam} = @NameReverter

    {Text2Reverter} = DescriptionReverter
    {Text2ReverterParam} = @DescriptionReverter

*/

END")]
        #endregion
        public override void UpdatePhotoKeywords(Revertable2Collection<Photo> processlist)
        {
            string procedure = "bx_Album_UpdatePhotoKeywords";
            string table = "bx_Photos";
            string primaryKey = "PhotoID";

            SqlDbType text1_Type = SqlDbType.NVarChar; int text1_Size = 50;
            SqlDbType reve1_Type = SqlDbType.NVarChar; int reve1_Size = 4000;
            SqlDbType text2_Type = SqlDbType.NVarChar; int text2_Size = 1500;
            SqlDbType reve2_Type = SqlDbType.NText; //int reve2_Size = 4000;

            if (processlist == null || processlist.Count == 0)
                return;

            //有一部分项是不需要更新文本的（例如：只有版本或恢复信息发生了变化），把这部分的ID取出来，一次性更新以提高性能
            List<int> needUpdateButTextNotChangedIds = processlist.GetNeedUpdateButTextNotChangedKeys();

            using (SqlQuery query = new SqlQuery())
            {
                StringBuffer sql = new StringBuffer();

                //前面取出的可以一次性更新版本而无需更新文本的部分项，在此批量更新
                if (needUpdateButTextNotChangedIds.Count > 0)
                {
                    sql += @"UPDATE " + table + " SET KeywordVersion = @NewVersion WHERE " + primaryKey + " IN (@NeedUpdateButTextNotChangedIds);";

                    query.CreateParameter<string>("@NewVersion", processlist.Version, SqlDbType.VarChar, 32);
                    query.CreateInParameter("@NeedUpdateButTextNotChangedIds", needUpdateButTextNotChangedIds);
                }

                int i = 0;
                foreach (Revertable2<Photo> item in processlist)
                {
                    //此项确实需要更新，且不只是版本发生了变化
                    if (item.NeedUpdate && item.OnlyVersionChanged == false)
                    {
                        sql.InnerBuilder.AppendFormat(@"EXEC {1} @ID_{0}, @KeywordVersion_{0}, @Text1_{0}, @Reverter1_{0}, @Text2_{0}, @Reverter2_{0};", i, procedure);

                        query.CreateParameter<int>("@ID_" + i, item.Value.GetKey(), SqlDbType.Int);

                        //如果文本1或文本2发生了变化，更新
                        if (item.Text1Changed || item.Text2Changed)
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, item.Value.KeywordVersion, SqlDbType.VarChar, 32);

                            //文本1发生了变化
                            if (item.Text1Changed)
                                query.CreateParameter<string>("@Text1_" + i, item.Value.Text1, text1_Type, text1_Size);
                            else
                                query.CreateParameter<string>("@Text1_" + i, null, text1_Type, text1_Size);

                            //文本2发生了变化
                            if (item.Text2Changed)
                                query.CreateParameter<string>("@Text2_" + i, item.Value.Text2, text2_Type, text2_Size);
                            else
                                query.CreateParameter<string>("@Text2_" + i, null, text2_Type, text2_Size);
                        }
                        else
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, null, SqlDbType.VarChar, 32);

                            query.CreateParameter<string>("@Text1_" + i, null, text1_Type, text1_Size);
                            query.CreateParameter<string>("@Text2_" + i, null, text2_Type, text2_Size);

                        }

                        //如果恢复信息1发生了变化，更新
                        if (item.Reverter1Changed)
                            query.CreateParameter<string>("@Reverter1_" + i, item.Reverter1, reve1_Type, reve1_Size);
                        else
                            query.CreateParameter<string>("@Reverter1_" + i, null, reve1_Type, reve1_Size);

                        //如果恢复信息2发生了变化，更新
                        if (item.Reverter2Changed)
                            query.CreateParameter<string>("@Reverter2_" + i, item.Reverter2, reve2_Type);
                        else
                            query.CreateParameter<string>("@Reverter2_" + i, null, reve2_Type);

                        i++;

                    }
                }

                query.CommandText = sql.ToString();
                query.ExecuteNonQuery();
            }
        }

        #region 旧的处理关键字的代码

        //        public override void FillAlbumReverters(TextRevertableCollection processlist)
        //        {
        //            if (processlist == null || processlist.Count == 0)
        //                return;

        //            using (SqlQuery query = new SqlQuery())
        //            {
        //                query.CommandText = "SELECT Name, TextVersion, NameReverter = (SELECT NameReverter FROM bx_AlbumReverters WHERE AlbumID = T.AlbumID) FROM bx_Albums T WHERE AlbumID IN (@AlbumIds)";

        //                query.CreateInParameter<int>("@AlbumIds", processlist.GetIds());

        //                using (XSqlDataReader reader = query.ExecuteReader())
        //                {
        //                    SqlDataReaderWrap readerWrap = new SqlDataReaderWrap(reader);

        //                    while (readerWrap.Next)
        //                    {
        //                        int albumID = readerWrap.Get<int>("AlbumID");
        //                        string textVersion = readerWrap.Get<string>("TextVersion");
        //                        string name = readerWrap.Get<string>("Name");
        //                        string nameReverter = readerWrap.Get<string>("NameReverter");
        //                        processlist.FillReverter(albumID, name, textVersion, nameReverter);
        //                    }
        //                }
        //            }
        //        }

        //        #region StoredProcedure
        //        [StoredProcedure(Name = "bx_Album_UpdateReverter", Script = @"
        //CREATE PROCEDURE {name}
        //    @AlbumID               int,
        //    @Name                  nvarchar(50),
        //    @NameVersion           varchar(32),
        //    @NameReverter          nvarchar(4000)
        //AS BEGIN
        //
        //    SET NOCOUNT ON;
        //
        //    UPDATE [bx_Albums] SET Name = @Name, NameVersion = @NameVersion WHERE AlbumID = @AlbumID;
        //
        //    UPDATE [bx_AlbumReverters] SET NameReverter = @NameReverter WHERE AlbumID = @AlbumID;
        //    IF @@ROWCOUNT = 0
        //        INSERT INTO [bx_AlbumReverters] (AlbumID, NameReverter) VALUES (@AlbumID, @NameReverter);
        //
        //END")]
        //        #endregion
        //        public override void UpdateAlbumKeywords(TextRevertableCollection processlist)
        //        {
        //            if (processlist == null || processlist.Count == 0)
        //                return;

        //            using (SqlQuery query = new SqlQuery())
        //            {
        //                StringBuilder sql = new StringBuilder();

        //                int i = 0;
        //                foreach (ITextRevertable content in processlist)
        //                {
        //                    string reverter = processlist.GetReverter(content.GetKey());
        //                    if (reverter == null)
        //                        continue;

        //                    sql.AppendFormat(@"EXEC bx_Album_UpdateReverter @AlbumID_{0}, @Name_{0}, @NameVersion_{0}, @NameReverter_{0};", i);

        //                    query.CreateParameter<int>("@AlbumID_" + i, content.GetKey(), SqlDbType.Int);
        //                    query.CreateParameter<string>("@Name_" + i, content.Text, SqlDbType.NVarChar, 50);
        //                    query.CreateParameter<string>("@NameVersion_" + i, content.TextVersion, SqlDbType.VarChar, 32);
        //                    query.CreateParameter<string>("@NameReverter_" + i, reverter, SqlDbType.NVarChar, 4000);

        //                    i++;
        //                }

        //                query.CommandText = sql.ToString();
        //                query.ExecuteNonQuery();
        //            }
        //        }

        //        public override void FillPhotoReverters(TextRevertable2Collection processlist)
        //        {
        //            if (processlist == null || processlist.Count == 0)
        //                return;

        //            using (SqlQuery query = new SqlQuery())
        //            {
        //                query.CommandText = "SELECT * FROM bx_PhotoReverters WHERE PhotoID IN (@PhotoIds)";

        //                query.CreateInParameter<int>("@PhotoIds", processlist.GetIds());

        //                using (XSqlDataReader reader = query.ExecuteReader())
        //                {
        //                    SqlDataReaderWrap readerWrap = new SqlDataReaderWrap(reader);

        //                    while (readerWrap.Next)
        //                    {
        //                        int photoID = readerWrap.Get<int>("PhotoID");
        //                        string reverter1 = readerWrap.Get<string>("NameReverter");
        //                        string reverter2 = readerWrap.Get<string>("DescriptionReverter");
        //                        processlist.FillReverter(photoID, reverter1, reverter2);
        //                    }
        //                }
        //            }
        //        }

        //        #region StoredProcedure
        //        [StoredProcedure(Name = "bx_Album_UpdatePhotoReverter", Script = @"
        //CREATE PROCEDURE {name}
        //    @PhotoID               int,
        //    @Name                  nvarchar(50),
        //    @NameVersion           varchar(32),
        //    @NameReverter          nvarchar(4000),
        //    @Description           nvarchar(1500),
        //    @DescriptionVersion    varchar(32),
        //    @DescriptionReverter   ntext
        //AS BEGIN
        //    SET NOCOUNT ON;
        //
        //    IF (@Name IS NOT NULL AND @Description IS NOT NULL)
        //        UPDATE [bx_Photos] SET Name = @Name, NameVersion = @NameVersion, Description = @Description, DescriptionVersion = @DescriptionVersion WHERE PhotoID = @PhotoID;
        //    ELSE IF (@Name IS NOT NULL)
        //        UPDATE [bx_Photos] SET Name = @Name, NameVersion = @NameVersion WHERE PhotoID = @PhotoID;
        //    ELSE IF (@Description IS NOT NULL)
        //        UPDATE [bx_Photos] SET Description = @Description, DescriptionVersion = @DescriptionVersion WHERE PhotoID = @PhotoID;
        //
        //    IF (@NameReverter IS NOT NULL AND @DescriptionReverter IS NOT NULL) BEGIN
        //        UPDATE [bx_PhotoReverters] SET NameReverter = @NameReverter, DescriptionReverter = @DescriptionReverter WHERE PhotoID = @PhotoID;
        //        IF @@ROWCOUNT = 0
        //            INSERT INTO bx_PhotoReverters (PhotoID, NameReverter, DescriptionReverter) VALUES (@PhotoID, @NameReverter, @DescriptionReverter);
        //    END
        //    ELSE IF (@NameReverter IS NOT NULL) BEGIN
        //        UPDATE [bx_PhotoReverters] SET NameReverter = @NameReverter WHERE PhotoID = @PhotoID;
        //        IF @@ROWCOUNT = 0
        //            INSERT INTO bx_PhotoReverters (PhotoID, NameReverter, DescriptionReverter) VALUES (@PhotoID, @NameReverter, N'');
        //    END
        //    ELSE IF (@DescriptionReverter IS NOT NULL) BEGIN
        //        UPDATE [bx_PhotoReverters] SET DescriptionReverter = @DescriptionReverter WHERE PhotoID = @PhotoID;
        //        IF @@ROWCOUNT = 0
        //            INSERT INTO bx_PhotoReverters (PhotoID, NameReverter, DescriptionReverter) VALUES (@PhotoID, N'', @DescriptionReverter);
        //    END
        //
        //END")]
        //        #endregion
        //        public override void UpdatePhotoKeywords(TextRevertable2Collection processlist)
        //        {
        //            if (processlist == null || processlist.Count == 0)
        //                return;

        //            using (SqlQuery query = new SqlQuery())
        //            {
        //                StringBuilder sql = new StringBuilder();

        //                string reverter;

        //                int i = 0;
        //                foreach (ITextRevertable2 content in processlist)
        //                {

        //                    sql.AppendFormat(@"EXEC bx_Album_UpdatePhotoReverter @PhotoID_{0}, @Name_{0}, @NameVersion_{0}, @NameReverter_{0}, @Description_{0}, @DescriptionVersion_{0}, @DescriptionReverter_{0};", i);

        //                    query.CreateParameter<int>("@PhotoID_" + i, content.GetKey(), SqlDbType.Int);

        //                    reverter = processlist.GetReverter1(content.GetKey());
        //                    if (reverter == null)
        //                    {
        //                        query.CreateParameter<string>("@Name_" + i, null, SqlDbType.NVarChar, 50);
        //                        query.CreateParameter<string>("@NameVersion_" + i, null, SqlDbType.VarChar, 32);
        //                        query.CreateParameter<string>("@NameReverter_" + i, null, SqlDbType.NVarChar, 4000);
        //                    }
        //                    else
        //                    {
        //                        query.CreateParameter<string>("@Name_" + i, content.Text1, SqlDbType.NVarChar, 50);
        //                        query.CreateParameter<string>("@NameVersion_" + i, content.TextVersion1, SqlDbType.VarChar, 32);
        //                        query.CreateParameter<string>("@NameReverter_" + i, reverter, SqlDbType.NVarChar, 4000);
        //                    }



        //                    reverter = processlist.GetReverter2(content.GetKey());
        //                    if (reverter == null)
        //                    {
        //                        query.CreateParameter<string>("@Description_" + i, null, SqlDbType.NVarChar, 1500);
        //                        query.CreateParameter<string>("@DescriptionVersion_" + i, null, SqlDbType.VarChar, 32);
        //                        query.CreateParameter<string>("@DescriptionReverter_" + i, null, SqlDbType.NText);
        //                    }
        //                    else
        //                    {
        //                        query.CreateParameter<string>("@Description_" + i, content.Text2, SqlDbType.NVarChar, 1500);
        //                        query.CreateParameter<string>("@DescriptionVersion_" + i, content.TextVersion2, SqlDbType.VarChar, 32);
        //                        query.CreateParameter<string>("@DescriptionReverter_" + i, reverter, SqlDbType.NText);
        //                    }

        //                    i++;
        //                }

        //                if (sql.Length > 0)
        //                {
        //                    query.CommandText = sql.ToString();

        //                    query.ExecuteNonQuery();
        //                }
        //            }
        //        }

        #endregion

        #endregion

        //==========================================================分割线以上是整理过的，分割线一下是未整理过的===================================================================

        //以下开始为相册部分

        #region Stored Procedure
        [StoredProcedure(Name = "bx_UpdateAlbumLogo", Script = @"
CREATE PROCEDURE {name}
       @AlbumID        int
     , @Cover     nvarchar(200)
AS BEGIN
    SET NOCOUNT ON;

    UPDATE
        [bx_Albums]
    SET
        [Cover] = @Cover
       ,[UpdateDate] = GETDATE()
    WHERE
        [AlbumID] = @AlbumID;
END")]
        #endregion
        public override bool UpdateAlbumLogo(int albumID, string cover)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateAlbumLogo";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@AlbumID", albumID, SqlDbType.Int);
                query.CreateParameter<string>("@Cover", cover, SqlDbType.NVarChar, 200);


                query.ExecuteNonQuery();
            }

            return true;
        }


        /// <summary>
        /// 获取指定用户的所有相册
        /// </summary>
        /// <param name="userID">指定用户ID</param>
        /// <param name="isGetPrivacyType">是否获取隐私类型数据</param>
        #region Stored Procedure
        [StoredProcedure(Name = "bx_GetAlbumsByUserID", Script = @"
CREATE PROCEDURE {name}
    @UserID         int
AS BEGIN
    SET NOCOUNT ON;

    SELECT * FROM [bx_Albums] WHERE [UserID] = @UserID;    
END")]
        #endregion
        public override AlbumCollection GetAlbums(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAlbumsByUserID";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AlbumCollection(reader);
                }
            }
        }


        /// <summary>
        /// 随便看看首页的相册列表
        /// </summary>
        /// <param name="number">要显示的数据个数</param>
        public override AlbumCollection GetTopAlbums(int number)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = string.Format("SELECT TOP {0} * FROM [bx_Albums] WHERE [PrivacyType] IN (0,3) ORDER BY [AlbumID] DESC", number);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AlbumCollection(reader);
                }
            }
        }


        /*

        /// <summary>
        /// 相册搜索
        /// </summary>
        public override AlbumCollection GetAlbumsByFilter(AlbumFilter filter, bool isGetPrivacyType, int pageNumber, int pageSize, ref int? count)
        {
            #region 数据条件

            string qUsernames = filter.Username;
            string searchKeys = filter.SearchKey;

            DateTime now = DateTimeUtil.Now;
            DateTime? endDate = filter.EndDate;
            DateTime? beginDate = filter.BeginDate;

            IEnumerable<string> usernames = null;
            if (!string.IsNullOrEmpty(qUsernames))
            {
                usernames = StringUtil.Split(qUsernames);
            }

            #endregion

            #region 组合SQL语句

            StringBuffer condition = new StringBuffer();
            condition += "([Name] LIKE '%' + @Key + '%') ";
            if (beginDate != null)
            {
                condition += " AND [CreateDate] >= @BeginDate ";
            }
            if (endDate != null)
            {
                condition += " AND [CreateDate] <= @EndDate ";
            }
            if (usernames != null)
            {
                condition += " AND [UserID] IN (SELECT [UserID] FROM [bx_Users] WHERE [Username] IN (@Usernames)) ";
            }
            if (!isGetPrivacyType)
            {
                condition += " AND ([PrivacyType] IN (0,3))";
            }

            #endregion

            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Albums";
                query.Pager.SortField = "[AlbumID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = count;
                query.Pager.SelectCount = true;
                query.Pager.IsDesc = true;

                query.Pager.Condition = condition.ToString();

                query.CreateParameter<string>("@Key", searchKeys, SqlDbType.NVarChar, 50);
                query.CreateParameter<DateTime?>("@BeginDate", beginDate, SqlDbType.DateTime);
				query.CreateParameter<DateTime?>("@EndDate", endDate, SqlDbType.DateTime);


				query.CreateInParameter("@Usernames", usernames);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    SqlDataReaderWrap readerWrap = new SqlDataReaderWrap(reader);
                    AlbumCollection albums = new AlbumCollection(readerWrap);
                    if (count == null && readerWrap.NextResult())
                    {
                        if (readerWrap.Next)
                        {
                            count = readerWrap.Get<int>(0);
                        }
                    }
                    return albums;
                }
            }
        }

        */

        /// <summary>
        /// 高级相册搜索
        /// </summary>


        /// <summary>
        /// 批量移动相片到新相册
        /// </summary>
        /// <param name="photoIDs">要移动的相片ID集</param>
        /// <param name="targetAlbumID">新相册ID</param>
        public override bool MovePhotos(IEnumerable<int> photoIds, int targetAlbumID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"UPDATE [bx_Photos] SET [AlbumID] = @AlbumID WHERE [PhotoID] IN (@PhotoIds)";


                query.CreateParameter<int>("@AlbumID", targetAlbumID, SqlDbType.Int);

                query.CreateInParameter<int>("@PhotoIds", photoIds);

                query.ExecuteNonQuery();

            }

            return true;
        }

        /// <summary>
        /// 获取某张相片
        /// </summary>
        /// <param name="photoID">相片ID</param>
        /// <summary>
        /// 获取随便看看首页的一些相片
        /// </summary>
        public override PhotoCollection GetTopPhotos(int number)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = string.Format(@"
SELECT 
    TOP {0} 
* FROM 
     [bx_Photos]
WHERE 
    [AlbumID] IN (SELECT [AlbumID] FROM [bx_Albums] WHERE [PrivacyType] IN (0,3)) 
ORDER BY [PhotoID] DESC;"
                , number);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PhotoCollection(reader);
                }
            }
        }

        /// <summary>
        /// 获取用户最近几张照片
        /// </summary>
        /// <param name="userID">用户</param>
        /// <param name="number">指定张数的照片</param>
        public override PhotoCollection GetUserTopPhotos(int userID, int number)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = string.Format(@"
SELECT 
    TOP (@Top)
* FROM
    [bx_Photos]
WHERE
    [AlbumID] IN (SELECT [AlbumID] FROM [bx_Albums] WHERE [PrivacyType] IN (0,3)) 
AND
    [UserID] = @UserID
ORDER BY [PhotoID] DESC;", number);


                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateTopParameter("@Top", number);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PhotoCollection(reader);
                }
            }
        }


        /// <summary>
        /// 获取指定用户的所有相片
        /// </summary>
        #region StoredProcedure
        [StoredProcedure(Name = "bx_GetUserAllPhotos", Script = @"
CREATE PROCEDURE {name}
    @UserID        int
AS BEGIN
    SET NOCOUNT ON;    

    SELECT * FROM [bx_Photos] WHERE [UserID] = @UserID;
END")]
        #endregion
        public override PhotoCollection GetPhotos(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUserAllPhotos";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PhotoCollection(reader);
                }
            }
        }

        /// <summary>
        /// 获取几张相片
        /// </summary>
        /// <param name="photoIDs">相片ID集</param>
        public override PhotoCollection GetPhotos(IEnumerable<int> photoIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM [bx_Photos] WHERE [PhotoID] IN (@PhotoIDs);";

                query.CreateInParameter<int>("@PhotoIDs", photoIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PhotoCollection(reader);
                }
            }
        }

        /// <summary>
        /// 获取指定相册/所有相册的所有相片
        /// </summary>
        /// <param name="albumID">指定相册,不指定就是获取所有相册的所有相片</param>
        /// <param name="isGetPrivacyType">是否获取隐私类型相册的相片</param>
        public override PhotoCollection GetPhotos(int? albumID, bool isGetPrivacyType, int pageNumber, int pageSize, ref int? count)
        {
            StringBuffer condition = new StringBuffer();
            if (albumID != null)
            {
                condition += "[AlbumID] = @AlbumID ";
            }
            if (albumID != null && !isGetPrivacyType)
            {
                condition += " AND [AlbumID] IN (SELECT [AlbumID] FROM [bx_Albums] WHERE [PrivacyType] IN (0,3)) ";
            }
            else
            {
                if (!isGetPrivacyType)
                {
                    condition += " [AlbumID] IN (SELECT [AlbumID] FROM [bx_Albums] WHERE [PrivacyType] IN (0,3)) ";
                }
            }

            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_Photos]";
                query.Pager.SortField = "[PhotoID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.IsDesc = true;
                query.Pager.TotalRecords = count;
                query.Pager.SelectCount = true;
                query.Pager.Condition = condition.ToString();

                query.CreateParameter<int?>("@AlbumID", albumID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PhotoCollection photos = new PhotoCollection(reader);

                    if (count == null && reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            count = reader.Get<int>(0);
                        }
                    }

                    photos.TotalRecords = count.Value;

                    return photos;
                }
            }
        }

        /*

        /// <summary>
        /// 搜索相片
        /// </summary>
        public override PhotoCollection GetPhotosByFilter(PhotoFilter filter, bool isGetPrivacyType, int pageNumber, int pageSize, ref int? count)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Photos";
                query.Pager.SortField = "[PhotoID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.TotalRecords = count;
                query.Pager.IsDesc = true;

                query.Pager.Condition = BuildConditionsByFilter(query, filter);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    SqlDataReaderWrap readerWrap = new SqlDataReaderWrap(reader);

                    PhotoCollection photos = new PhotoCollection(readerWrap);
                    if (count == null && readerWrap.NextResult())
                    {
                        if (readerWrap.Next)
                        {
                            count = readerWrap.Get<int>(0);
                        }
                    }
                    return photos;
                }
            }
        }

        */

        /// <summary>
        /// 根据Filter来获得Sql用的条件，并填充参数
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private string BuildConditionsByFilter(PhotoFilter filter, SqlQuery query)
        {
            StringBuffer sqlConditions = new StringBuffer();

            if (filter.BeginDate != null)
            {
                sqlConditions += " AND [CreateDate] >= @BeginDate";
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
            }
            if (filter.EndDate != null)
            {
                sqlConditions += " AND [CreateDate] <= @EndDate";
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);
            }
            if (string.IsNullOrEmpty(filter.Usernames) == false)
            {
                sqlConditions += " AND [UserID] = (SELECT [UserID] FROM [bx_Users] WHERE [Username] = @Username)";
                query.CreateParameter<string>("@Username", filter.Usernames, SqlDbType.NVarChar, 50);
            }
            if (string.IsNullOrEmpty(filter.SearchKey) == false)
            {
                sqlConditions += " AND ([Name] LIKE '%' + @Name + '%' OR [Description] LIKE '%' + @Name + '%')";
                query.CreateParameter<string>("@Name", filter.SearchKey, SqlDbType.NVarChar, 50);
            }

            if (sqlConditions.Length > 0)
                sqlConditions.Remove(0, 5);

            return sqlConditions.ToString();
        }


        #region StoredProcedure
        [StoredProcedure(Name = "bx_GetPhotosAndAlbum", Script = @"
CREATE PROCEDURE {name}
    @PhotoID        int
AS BEGIN
    SET NOCOUNT ON;   
    DECLARE @AlbumID int;
    SELECT @AlbumID = AlbumID FROM [bx_Photos] WHERE [PhotoID] = @PhotoID;
    SELECT * FROM [bx_Photos] WHERE AlbumID=@AlbumID;
    SELECT * FROM [bx_Albums] WHERE AlbumID=@AlbumID;
END")]
        #endregion
        public override PhotoCollection GetPhotos(int photoID, out Album album)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetPhotosAndAlbum";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@PhotoID", photoID, SqlDbType.Int);

                album = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PhotoCollection photos = new PhotoCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            album = new Album(reader);
                        }
                    }

                    return photos;
                }
            }
        }
    }
}