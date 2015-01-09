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
using System.Data;
using MaxLabs.bbsMax.StepByStepTasks;
using System.Data.SqlClient;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class StepByStepTaskDao : StepByStepTasks.StepByStepTaskDao
    {

        [StoredProcedure(Name = "bx_GetRunningStepByStepTask", Script = @"
CREATE PROCEDURE {name}
    @UserID int,
    @TaskID uniqueidentifier
AS
BEGIN

    SET NOCOUNT ON;

    SELECT * FROM bx_StepByStepTasks WHERE TaskID = @TaskID AND (UserID = @UserID OR InstanceMode IN (2, 4));

END
")]
        public override RunningTask GetRunningTask(int userID, Guid taskID)
        {
            RunningTask task = null;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_GetRunningStepByStepTask";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<Guid>("@TaskID", taskID, SqlDbType.UniqueIdentifier);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        task = new RunningTask(reader);
                }

                return task;
            }
        }


        [StoredProcedure(Name = "bx_GetRunningStepByStepTasks", Script = @"
CREATE PROCEDURE {name}
    @UserID int
AS
BEGIN

    SET NOCOUNT ON;

    SELECT * FROM bx_StepByStepTasks WHERE UserID = @UserID OR InstanceMode IN (2, 4);

END
")]
        public override RunningTaskCollection GetRunningTasks(int userID)
        {

            RunningTaskCollection tasks;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_GetRunningStepByStepTasks";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    tasks = new RunningTaskCollection(reader);
                }

                return tasks;
            }
        }

        [StoredProcedure(Name = "bx_GetRunningStepByStepTasksByType", Script = @"
CREATE PROCEDURE {name}
    @UserID int,
    @TaskType varchar(200),
    @Param nvarchar(3500)
AS
BEGIN

    SET NOCOUNT ON;


    IF @Param IS NULL
        SELECT * FROM bx_StepByStepTasks WHERE (UserID = @UserID OR InstanceMode IN (2, 4))
        AND Type = @TaskType;
    ELSE
        SELECT * FROM bx_StepByStepTasks WHERE (UserID = @UserID OR InstanceMode IN (2, 4))
        AND Type = @TaskType AND Param = @Param;

END
")]
        public override RunningTaskCollection GetRunningTasks(int userID, Type taskType, string param)
        {

            RunningTaskCollection tasks;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_GetRunningStepByStepTasksByType";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@TaskType", taskType.FullName, SqlDbType.VarChar, 200);
                query.CreateParameter<string>("@Param", param, SqlDbType.NVarChar, 3500);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    tasks = new RunningTaskCollection(reader);
                }

                return tasks;
            }
        }

        [StoredProcedure(Name = "bx_BeginStepByStepTasks", Script = @"
CREATE PROCEDURE {name}
    @TaskID uniqueidentifier,
    @Type varchar(200),
    @UserID int,
    @Param nvarchar(3500),
    @Offset bigint,
    @TotalCount int,
    @Title nvarchar(100),
    @InstanceMode tinyint
AS
BEGIN

    SET NOCOUNT ON;

    IF @InstanceMode = 1 BEGIN

        IF EXISTS ( SELECT * FROM bx_StepByStepTasks WHERE UserID = @UserID AND Type = @Type )
            RETURN 1;  --用户单实例模式，已经有在运行

    END
    ELSE IF @InstanceMode = 2 BEGIN

        IF EXISTS ( SELECT * FROM bx_StepByStepTasks WHERE Type = @Type )
            RETURN 2;  --系统单实例模式，已经有在运行

    END

    INSERT INTO [bx_StepByStepTasks]
        ([TaskID]
        ,[Type]
        ,[UserID]
        ,[Param]
        ,[Offset]
        ,[TotalCount]
        ,[Title]
        ,[InstanceMode])
     VALUES
        (@TaskID
        ,@Type
        ,@UserID
        ,@Param
        ,@Offset
        ,@TotalCount
        ,@Title
        ,@InstanceMode);

END
")]
        public override int BeginTask(Guid taskID, Type type, int userID, string param, int totalCount, long offset, string title, TaskType instanceMode)
        {

            int result;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_BeginStepByStepTasks";

                query.CreateParameter<Guid>("@TaskID", taskID, SqlDbType.UniqueIdentifier);
                query.CreateParameter<string>("@Type", type.FullName, SqlDbType.VarChar, 200);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Param", param, SqlDbType.NVarChar, 3500);
                query.CreateParameter<long>("@Offset", offset, SqlDbType.BigInt);
                query.CreateParameter<int>("@TotalCount", totalCount, SqlDbType.Int);
                query.CreateParameter<string>("@Title", title, SqlDbType.NVarChar, 100);
                query.CreateParameter<byte>("@InstanceMode", (byte)instanceMode, SqlDbType.TinyInt);
                query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                result = (int)query.Parameters["@ReturnValue"].Value;

            }

            return result;
        }

        [StoredProcedure(Name = "bx_UpdateStepByStepTasks", Script = @"
CREATE PROCEDURE {name}
    @TaskID uniqueidentifier,
    @Param nvarchar(3500),
    @TotalCount int,
    @FinishedCount int,
    @Offset bigint,
    @Title nvarchar(100)
AS
BEGIN

    SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM bx_StepByStepTasks WHERE TaskID = @TaskID) BEGIN

        UPDATE [bx_StepByStepTasks]
             SET [Param] = @Param,
                 [TotalCount] = @TotalCount,
                 [FinishedCount] = @FinishedCount,
                 [Offset] = @Offset,
                 [Title] = @Title,
                 [LastExecuteTime] = GETDATE()
           WHERE TaskID = @TaskID;

    END
    ELSE
        RETURN (1);

END
")]
        public override void UpdateRunnintTaskStatus(Guid taskID, string param, int totalCount, int finishedCount, long offset, string title)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_UpdateStepByStepTasks";

                query.CreateParameter<Guid>("@TaskID", taskID, SqlDbType.UniqueIdentifier);
                query.CreateParameter<string>("@Param", param, SqlDbType.NVarChar, 3500);
                query.CreateParameter<int>("@TotalCount", totalCount, SqlDbType.Int);
                query.CreateParameter<int>("@FinishedCount", finishedCount, SqlDbType.Int);
                query.CreateParameter<long>("@Offset", offset, SqlDbType.BigInt);
                query.CreateParameter<string>("@Title", title, SqlDbType.NVarChar, 100);

                query.ExecuteNonQuery();

            }
        }

        [StoredProcedure(Name = "bx_FinishTask", Script = @"
CREATE PROCEDURE {name}
    @TaskID uniqueidentifier
AS
BEGIN

    SET NOCOUNT ON;

    DELETE [bx_StepByStepTasks] WHERE TaskID = @TaskID;

END
")]
        public override void FinishTask(Guid taskID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_FinishTask";

                query.CreateParameter<Guid>("@TaskID", taskID, SqlDbType.UniqueIdentifier);

                query.ExecuteNonQuery();

            }
        }

        public override bool IsRunning(IEnumerable<string> taskTypes)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.Text;
                query.CommandText = @"
IF EXISTS(SELECT * FROM bx_StepByStepTasks WHERE [Type] in(@Types))
    SELECT 1;
ELSE
    SELECT 0;
";

                query.CreateInParameter<string>("@Types", taskTypes);

                return query.ExecuteScalar<int>() == 1;

            }
        }
    }
}