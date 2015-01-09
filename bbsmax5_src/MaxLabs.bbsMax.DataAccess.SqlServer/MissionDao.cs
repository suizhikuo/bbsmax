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
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;


using MaxLabs.bbsMax.Enums;
using System.Data.SqlClient;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class MissionDao : DataAccess.MissionDao
    {
        #region 存储过程 bx_CreateMission
        [StoredProcedure(Name = "bx_CreateMission", Script = @"
CREATE PROCEDURE {name}
     @CycleTime         int
    ,@SortOrder         int

    ,@Type              nvarchar(200)             
    ,@Name              nvarchar(100)
    ,@IconUrl           nvarchar(200)
    ,@DeductPoint       nvarchar(100)

    ,@Prize             ntext
    ,@Description       ntext
    ,@ApplyCondition    ntext
    ,@FinishCondition   ntext

    ,@EndDate           DateTime
    ,@BeginDate         DateTime
    ,@IsEnable          bit

    ,@CategoryID        int
    ,@ParentID          int
AS
BEGIN

	SET NOCOUNT ON;
    INSERT INTO [bx_Missions](
             [CycleTime]
            ,[SortOrder]

            ,[Type]
            ,[Name]
            ,[IconUrl]
            ,[DeductPoint]

            ,[Prize]
            ,[Description]
            ,[ApplyCondition]
            ,[FinishCondition]

            ,[EndDate]
            ,[BeginDate]
            ,[IsEnable]

            ,[CategoryID]
            ,[ParentID]
            ) VALUES (
             @CycleTime
            ,@SortOrder

            ,@Type
            ,@Name
            ,@IconUrl
            ,@DeductPoint

            ,@Prize
            ,@Description
            ,@ApplyCondition
            ,@FinishCondition

            ,@EndDate
            ,@BeginDate
            ,@IsEnable

            ,@CategoryID
            ,@ParentID
            );

    SELECT @@IDENTITY;
END
")]
        #endregion
        public override int CreateMission(Mission mission)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreateMission";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@CycleTime", mission.CycleTime, SqlDbType.Int);
                query.CreateParameter<int>("@SortOrder", mission.SortOrder, SqlDbType.Int);

                query.CreateParameter<string>("@Type", mission.Type, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@Name", mission.Name, SqlDbType.NVarChar, 100);
                query.CreateParameter<string>("@IconUrl", mission.IconUrl, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@DeductPoint", StringUtil.Join(mission.DeductPoint), SqlDbType.NVarChar, 100);

                query.CreateParameter<string>("@Prize", mission.Prize.ConvertToString(), SqlDbType.NText);
                query.CreateParameter<string>("@Description", mission.Description, SqlDbType.NText);
                query.CreateParameter<string>("@ApplyCondition", mission.ApplyCondition.ConvertToString(), SqlDbType.NText);
                query.CreateParameter<string>("@FinishCondition", mission.FinishCondition.ToString(), SqlDbType.NText);

                query.CreateParameter<DateTime>("@EndDate", mission.EndDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@BeginDate", mission.BeginDate, SqlDbType.DateTime);

                query.CreateParameter<bool>("@IsEnable", mission.IsEnable, SqlDbType.Bit);

                query.CreateParameter<int?>("@CategoryID", mission.CategoryID, SqlDbType.Int);
                query.CreateParameter<int?>("@ParentID", mission.ParentID, SqlDbType.Int);

                return query.ExecuteScalar<int>();
            }

        }
        #region 存储过程 bx_UpdateMission
        [StoredProcedure(Name = "bx_UpdateMission", Script = @"
CREATE PROCEDURE {name}
     @ID                int
    ,@CycleTime         int
    ,@SortOrder         int
           
    ,@Name              nvarchar(100)
    ,@IconUrl           nvarchar(200)
    ,@DeductPoint       nvarchar(100)

    ,@Prize             ntext
    ,@Description       ntext
    ,@ApplyCondition    ntext
    ,@FinishCondition   ntext

    ,@EndDate           DateTime
    ,@BeginDate         DateTime
    ,@IsEnable          bit

    ,@CategoryID        int
AS
BEGIN

	SET NOCOUNT ON;
    UPDATE [bx_Missions] SET
             [CycleTime] = @CycleTime
            ,[SortOrder] = @SortOrder

            ,[Name] = @Name
            ,[IconUrl] = @IconUrl
            ,[DeductPoint] = @DeductPoint

            ,[Prize] = @Prize
            ,[Description] = @Description
            ,[ApplyCondition] = @ApplyCondition
            ,[FinishCondition] = @FinishCondition

            ,[EndDate] = @EndDate
            ,[BeginDate] = @BeginDate
            ,[IsEnable] = @IsEnable
            ,[CategoryID] = @CategoryID
    WHERE 
            [ID]=@ID;
END
")]
        #endregion
        public override void UpdateMission(Mission mission)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateMission";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ID", mission.ID, SqlDbType.Int);
                query.CreateParameter<int>("@CycleTime", mission.CycleTime, SqlDbType.Int);
                query.CreateParameter<int>("@SortOrder", mission.SortOrder, SqlDbType.Int);

                query.CreateParameter<string>("@Name", mission.Name, SqlDbType.NVarChar, 100);
                query.CreateParameter<string>("@IconUrl", mission.IconUrl, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@DeductPoint", StringUtil.Join(mission.DeductPoint), SqlDbType.NVarChar, 100);

                query.CreateParameter<string>("@Prize", mission.Prize.ConvertToString(), SqlDbType.NText);
                query.CreateParameter<string>("@Description", mission.Description, SqlDbType.NText);
                query.CreateParameter<string>("@ApplyCondition", mission.ApplyCondition.ConvertToString(), SqlDbType.NText);
                query.CreateParameter<string>("@FinishCondition", mission.FinishCondition.ToString(), SqlDbType.NText);

                query.CreateParameter<DateTime>("@EndDate", mission.EndDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@BeginDate", mission.BeginDate, SqlDbType.DateTime);

                query.CreateParameter<bool>("@IsEnable", mission.IsEnable, SqlDbType.Bit);

                query.CreateParameter<int?>("@CategoryID", mission.CategoryID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }


        public override void UpdateMissions(List<int> missionIDs, List<string> names, List<int?> categoryIDs, List<int> sortOrders, List<string> iconUrls, List<DateTime> beginDates, List<DateTime> endDates, List<bool> isEnables)
        {
            using (SqlQuery query = new SqlQuery(QueryMode.Prepare))
            {
                query.CommandText = @"
UPDATE [bx_Missions] SET 
             [Name] = @Name
            ,[SortOrder] = @SortOrder
            ,[IconUrl] = @IconUrl
            ,[BeginDate] = @BeginDate
            ,[EndDate] = @EndDate
            ,[IsEnable] = @IsEnable
            ,[CategoryID] = @CategoryID
            WHERE [ID] = @ID;
";

                for (int i = 0; i < missionIDs.Count; i++)
                {
                    query.CreateParameter<string>("@Name", names[i], SqlDbType.NVarChar, 100);
                    query.CreateParameter<string>("@IconUrl", iconUrls[i], SqlDbType.NVarChar, 200);
                    query.CreateParameter<DateTime>("@BeginDate", beginDates[i], SqlDbType.DateTime);
                    query.CreateParameter<DateTime>("@EndDate", endDates[i], SqlDbType.DateTime);
                    query.CreateParameter<int>("@SortOrder", sortOrders[i], SqlDbType.Int);
                    query.CreateParameter<int>("@ID", missionIDs[i], SqlDbType.Int);
                    query.CreateParameter<bool>("@IsEnable", isEnables[i], SqlDbType.Bit);
                    query.CreateParameter<int?>("@CategoryID", categoryIDs[i], SqlDbType.Int);

                    query.ExecuteNonQuery();
                }

                //query.Submit();
            }
        }

        #region 存储过程 bx_GetAllMissions
        [StoredProcedure(Name="bx_GetAllMissions",Script= @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_Missions] ORDER BY [SortOrder] ASC,[ID] DESC;
END
"
            )]
        #endregion
        public override MissionCollection GetAllMissions()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAllMissions";
                query.CommandType = CommandType.StoredProcedure;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    MissionCollection missions = new MissionCollection(reader);

                    MissionCollection result = new MissionCollection();

                    foreach (Mission mission in missions)
                    {
                        if (mission.ParentID == null)
                        {
                            result.Add(mission);

                            MissionCollection childMissions = new MissionCollection();

                            foreach (Mission childMission in missions)
                            {
                                if (childMission.ParentID != null && childMission.ParentID == mission.ID)
                                {
                                    childMissions.Add(childMission);
                                }
                            }

                            mission.ChildMissions = childMissions;
                        }
                    }

                    return result;
                }
            }
        }

        public override void DeleteMissions(IEnumerable<int> missionIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
DELETE [bx_Missions] WHERE [ID] IN (@MissionIDs);
DELETE [bx_Missions] WHERE [ParentID] IN (@MissionIDs);
";

                query.CreateInParameter("@MissionIDs", missionIDs);


                query.ExecuteNonQuery();
            }
        }

        public override UserMissionCollection GetMissionUsers(int missionID, int pageNumber, int pageSize,out int totalUsers)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.SortField = "[CreateDate]";
                query.Pager.PrimaryKey = "[ID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_UserMissions]";
                query.Pager.Condition = @"
[MissionID] = @MissionID AND [Status] <> 3
";

                query.CreateParameter<int>("@MissionID", missionID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserMissionCollection userMissions = new UserMissionCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            totalUsers = reader.Get<int>(0);
                        }
                        else
                            totalUsers = 0;
                    }
                    else
                        totalUsers = 0;

                    return userMissions;
                }
            }
        }

        public override UserMissionCollection GetUserMissions(int userID, int pageNumber, int pageSize, out int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.SortField = "[CreateDate]";
                query.Pager.PrimaryKey = "[ID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_UserMissionsView]";
                query.Pager.Condition = @"UserID=@UserID AND IsEnable = 1 AND Status <> 1";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserMissionCollection missions = new UserMissionCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            totalCount = reader.GetInt32(0);
                        }
                        else
                            totalCount = 0;
                    }
                    else
                        totalCount = 0;

                    return missions;
                }
            }
        }


        #region 存储过程 bx_ApplyMission
        [StoredProcedure(Name = "bx_ApplyMission", FileName = "bx_ApplyMission.sql")]
        #endregion
        public override int ApplyMission(int userID, int missionID,double percent)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_ApplyMission";
                query.CommandType = CommandType.StoredProcedure;

                SqlParameter errorCode = query.CreateParameter<int>("@Code", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@MissionID", missionID, SqlDbType.Int);
                query.CreateParameter<double>("@Percent", percent, SqlDbType.Float);




                query.ExecuteNonQuery();
                return Convert.ToInt32(errorCode.Value);
            }
        }

        #region 存储过程 bx_GetUserMission
        [StoredProcedure(Name = "bx_GetUserMission", Script = @"
CREATE PROCEDURE {name}
     @UserID       int
    ,@MissionID    int
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
END
"
            )]
        #endregion
        public override UserMission GetUserMission(int userID, int missionID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUserMission";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@MissionID", missionID, SqlDbType.Int);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new UserMission(reader);
                    }
                    return null;
                }
            }
        }

        public override UserMission GetStepUserMission(int userID, int[] missionIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [Status] =0 AND [MissionID] IN (@MissionIDs);";
                
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateInParameter<int>("@MissionIDs", missionIDs);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new UserMission(reader);
                    }
                    return null;
                }
            }
        }

        #region 存储过程 bx_SetUserMissionIsPrized
        [StoredProcedure(Name = "bx_SetUserMissionIsPrized", Script = @"
CREATE PROCEDURE {name}
     @UserID       int
    ,@MissionID    int
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @FinishPercent float;
    IF EXISTS(SELECT * FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID AND [IsPrized] = 1)
        RETURN 3; --已经奖励过
    SELECT @FinishPercent = [FinishPercent] FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
    IF @FinishPercent IS NULL
        RETURN 1; -- 任务不存在
    ELSE IF @FinishPercent < 1
        RETURN 2; -- 任务未完成

    UPDATE [bx_UserMissions] SET [Status] = 1, [IsPrized] = 1, [FinishDate] = getdate() WHERE [UserID] = @UserID AND [MissionID] = @MissionID;

    RETURN 0;
END
"
            )]
        #endregion
        public override int SetUserMissionIsPrized(int userID, int missionID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetUserMissionIsPrized";
                query.CommandType = CommandType.StoredProcedure;

                SqlParameter code = query.CreateParameter<int>("@Code", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@MissionID", missionID, SqlDbType.Int);


                query.ExecuteNonQuery();
                return Convert.ToInt32(code.Value);
            }
        }

        #region 存储过程 bx_UpdateUserMissionStatus
        [StoredProcedure(Name = "bx_UpdateUserMissionStatus", Script = @"
CREATE PROCEDURE {name}
     @UserID       int
    ,@MissionIDs   varchar(8000)
    ,@Status       tinyint
AS
BEGIN
	SET NOCOUNT ON;
    IF CHARINDEX(',',@MissionIDs) = -1
        UPDATE [bx_UserMissions] SET [Status] = @Status WHERE [UserID] = @UserID AND [MissionID] = @MissionIDs;
    ELSE BEGIN
		DECLARE @sql varchar(200);
		SET @sql = 'UPDATE [bx_UserMissions] SET [Status] = ' + CAST(@Status as varchar(5)) + ' WHERE [UserID] = ' + CAST(@UserID as varchar(10)) + ' AND [MissionID] IN('+@MissionIDs+')';
        EXEC(@sql);
	END
END
"
            )]
        #endregion
        public override void UpdateUserMissionStatus(int userID, IEnumerable<int> missionIDs, MissionStatus status)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateUserMissionStatus";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@MissionIDs", StringUtil.Join(missionIDs), SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@Status", (int)status, SqlDbType.TinyInt);


                query.ExecuteNonQuery();
            }
        }

        #region 存储过程 bx_AbandonMission
        [StoredProcedure(Name = "bx_AbandonMission", Script = @"
CREATE PROCEDURE {name}
     @UserID       int
    ,@MissionID    int
AS
BEGIN
	SET NOCOUNT ON;

    DELETE FROM bx_UserMissions WHERE UserID=@UserID AND MissionID = @MissionID;

    DELETE FROM bx_UserMissions WHERE UserID=@UserID AND MissionID IN(SELECT MissionID FROM bx_Missions WHERE ParentID=@MissionID);

    RETURN 0;
END
")]
        #endregion
        public override bool AbandonMission(int userID, int missionID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_AbandonMission";
                query.CommandType = CommandType.StoredProcedure;

                SqlParameter resultCodeParm = query.CreateParameter<int>("@ResultCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@MissionID", missionID, SqlDbType.Int);


                query.ExecuteNonQuery();
                return resultCodeParm.Value.ToString() == "0";
            }
        }


        public override UserMissionCollection GetUserMissions(int userID, IEnumerable<int> missionIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"SELECT * FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] IN (@MissionIDs);";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateInParameter("@MissionIDs", missionIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new UserMissionCollection(reader);
                }
            }
        }

        #region 存储过程 bx_UpdateUserMissionFinishPercents
        [StoredProcedure(Name = "bx_UpdateUserMissionFinishPercents", Script = @"
CREATE PROCEDURE {name}
     @UserID             int
    ,@MissionIDs         varchar(8000)
    ,@Percents           varchar(8000)
AS
BEGIN
	SET NOCOUNT ON;
    
    IF CHARINDEX(',',@MissionIDs) = -1 BEGIN
        UPDATE [bx_UserMissions] SET [FinishPercent] = @Percents WHERE [UserID] = @UserID AND [MissionID] = @MissionIDs;
    END
    ELSE BEGIN
        DECLARE @MissionPercents table([MID] int IDENTITY(1, 1), [MissionID] int, [Percent] float);
        INSERT INTO @MissionPercents (MissionID) SELECT item FROM bx_GetIntTable(@MissionIDs, ',');
        UPDATE @MissionPercents SET [Percent] = P.[item] FROM bx_GetStringTable_varchar(@Percents, ',') P WHERE P.[id] = [MID];

        UPDATE [bx_UserMissions] SET 
                 [FinishPercent] = ISNULL(M.[Percent],0)  
        FROM @MissionPercents M 
        WHERE [UserID] = @UserID AND [bx_UserMissions].[MissionID] = M.[MissionID];
    END
END
"
            )]
        #endregion
        public override void UpdateUserMissionFinishPercents(int userID, Dictionary<int, double> missionPercents)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateUserMissionFinishPercents";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@MissionIDs", StringUtil.Join(missionPercents.Keys), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@Percents", StringUtil.Join(missionPercents.Values), SqlDbType.VarChar, 8000);


                query.ExecuteNonQuery();
            }
        }

        #region 存储过程 bx_GetNewUserMission
        [StoredProcedure(Name = "bx_GetNewUserMission", Script = @"
CREATE PROCEDURE {name}
     @UserID int
AS BEGIN
	SET NOCOUNT ON;
    
    SELECT TOP 1 * FROM [bx_Missions] WHERE
    getdate() > [BeginDate] AND getdate() < [EndDate] AND [IsEnable] = 1 
    AND
    (
            ([ID] NOT IN (SELECT [MissionID] FROM [bx_UserMissions] WHERE [UserID] = @UserID)
        OR
            ([CycleTime] > 0 AND [ID] IN(
                SELECT [MissionID] FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [FinishPercent]=1 AND [Status]<>3 AND [CreateDate]<DATEADD(second,0-[CycleTime],getdate())
                )
            ))
    ) 
    ORDER BY [SortOrder] ASC,[ID] DESC;
END
"
            )]
        #endregion
        public override Mission GetNewUserMission(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetNewUserMission";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Mission(reader);
                    }
                }
            }
            return null;
        }
        public override MissionCollection GetNewMissions(int userID, int? categoryID, int pageNumber, int pageSize, out int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = false;
                query.Pager.SortField = "[SortOrder]";
                query.Pager.PrimaryKey = "[ID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_Missions]";
                if (categoryID != null)
                {
                    query.Pager.Condition = "CategoryID = @CategoryID AND ";
                    query.CreateParameter<int>("@CategoryID", categoryID.Value, SqlDbType.Int);
                }
                query.Pager.Condition += @"
ParentID IS NULL AND
    ( getdate() > [BeginDate] AND getdate() < [EndDate] AND [IsEnable] = 1 AND [ID] NOT IN (SELECT [MissionID] FROM [bx_UserMissions] WHERE [UserID] = @UserID)
OR
    ([CycleTime] > 0 AND [IsEnable] = 1 AND [ID] IN(
        SELECT [MissionID] FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [FinishPercent]=1 AND [Status]<>3 AND [CreateDate]<DATEADD(second,0-[CycleTime],getdate()) AND getdate() > [BeginDate] AND getdate() < [EndDate]
        )
    ))
";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    MissionCollection missions = new MissionCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                        else
                            totalCount = 0;
                    }
                    else
                        totalCount = 0;

                    return missions;
                }
            }
        }


        #region 存储过程 bx_GetUserMissionCount
        [StoredProcedure(Name = "bx_GetUserMissionCount", Script = @"
CREATE PROCEDURE {name}
     @UserID   int
    ,@Status   tinyint
AS BEGIN
	SET NOCOUNT ON;
    
    SELECT COUNT(*) FROM bx_UserMissions UM INNER JOIN bx_Missions M ON UM.MissionID = M.ID  WHERE  M.IsEnable = 1 AND UserID = @UserID AND Status = @Status;
END
"
            )]
        #endregion
        public override int GetUserMissionCount(int userID, MissionStatus status)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUserMissionCount";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@Status", (int)status, SqlDbType.TinyInt);

                return query.ExecuteScalar<int>();
            }

        }


        public override UserMissionCollection GetNewMissionUsers(int getCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
DECLARE @Users table(UserID int,CreateDate DateTime)
INSERT INTO @Users([CreateDate],[UserID]) SELECT MAX([CreateDate]),[UserID] FROM [bx_UserMissions] WHERE [Status] <> 3 GROUP BY [UserID];
SELECT TOP (@TopCount) UM.* FROM @Users U INNER JOIN [bx_UserMissions] UM
    ON U.UserID = UM.[UserID] AND U.[CreateDate] = UM.[CreateDate]
    ORDER BY U.[CreateDate] DESC;
";

                query.CreateTopParameter("@TopCount", getCount);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new UserMissionCollection(reader);
                }
            }
        }

        [StoredProcedure(Name = "bx_Mission_GetCategories", Script = @"
CREATE PROCEDURE {name} 
AS
BEGIN
    SET NOCOUNT ON

    SELECT * FROM bx_MissionCategories;
END")]
        public override MissionCategoryCollection GetMissionCategories()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Mission_GetCategories";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new MissionCategoryCollection(reader);
                }
            }
        }

        [StoredProcedure(Name="bx_Mission_GetCategory", Script= @"
CREATE PROCEDURE {name}
    @CategoryID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_MissionCategories WHERE [ID] = @CategoryID;
END")]
        public override MissionCategory GetMissionCategory(int categoryID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Mission_GetCategory";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if(reader.Read())
                        return new MissionCategory(reader);

                    return null;
                }
            }
        }

        [StoredProcedure(Name="bx_Mission_CreateCategory", Script= @"
CREATE PROCEDURE {name}
    @Name nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS(SELECT * FROM bx_MissionCategories WHERE [Name] = @Name) BEGIN
        INSERT INTO bx_MissionCategories ([Name]) VALUES (@Name);
	END
	ELSE BEGIN
		SELECT -1;
	END

    SELECT @@IDENTITY;
END")]
        public override bool CreateMissionCategory(int userID, string categoryName, out int newCategoryID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Mission_CreateCategory";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@Name", categoryName, SqlDbType.NVarChar, 20);

                newCategoryID = query.ExecuteScalar<int>();

                if (newCategoryID == -1)
                    return false;

                return true;
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Mission_UpdateCategory", Script = @"
CREATE PROCEDURE {name}
    @CategoryID      int
   ,@Name            nvarchar(50)
AS BEGIN
    SET NOCOUNT ON;

    UPDATE 
        [bx_MissionCategories]
    SET 
        [Name] = @Name
    WHERE
        [ID] = @CategoryID;
END")]
        #endregion
        public override bool UpdateMissionCategory(int categoryID, string categoryName)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Mission_UpdateCategory";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);
                query.CreateParameter<string>("@Name", categoryName, SqlDbType.NVarChar, 50);


                query.ExecuteNonQuery();
            }

            return true;
        }


        public override bool DeleteMissionCategories(int[] ids)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
DELETE bx_MissionCategories WHERE [ID] IN (@IDs);
DELETE bx_Missions WHERE CategoryID IN (@IDs);";

                query.CreateInParameter<int>("@IDs", ids);

                query.ExecuteNonQuery();

                return true;
            }
        }
    }
}