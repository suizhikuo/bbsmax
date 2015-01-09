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

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class VarDao : DataAccess.VarDao
    {
        #region 存储过程 bx_GetStat
        [StoredProcedure(Name = "bx_GetVars", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM [bx_Vars];
END
"
            )]
        #endregion
        public override Vars GetVars()
        {
            Vars stat = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetVars";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stat = new Vars(reader);
                    }
                }
            }

            if (stat == null)
            {
                stat = UpdateNewUserStat();
            }

            return stat;
        }


        #region 存储过程 UpdateStat
        [StoredProcedure(Name = "bx_UpdateVars", Script = @"
CREATE PROCEDURE {name}
     @MaxPosts         int
    ,@NewUserID        int
    ,@TotalUsers       int
    ,@YestodayPosts    int
    ,@YestodayTopics   int
    ,@MaxOnlineCount   int
    ,@MaxPostDate      datetime
    ,@MaxOnlineDate    datetime
    ,@LastResetDate    datetime
    ,@NewUsername      nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS(SELECT * FROM [bx_Vars]) BEGIN
        UPDATE [bx_Vars] SET [MaxPosts]=@MaxPosts,[NewUserID]=@NewUserID,[TotalUsers]=@TotalUsers,[YestodayPosts]=@YestodayPosts,[MaxOnlineCount]=@MaxOnlineCount
                              ,[MaxPostDate]=@MaxPostDate,[MaxOnlineDate]=@MaxOnlineDate,[NewUsername]=@NewUsername
                              ,[YestodayTopics]=@YestodayTopics,[LastResetDate]=@LastResetDate
        WHERE [ID]=(SELECT TOP 1 ID FROM [bx_Vars]) AND (DATEPART(year,[LastResetDate]) <> DATEPART (year, getdate()) OR DATEPART(month,[LastResetDate]) <> DATEPART (month, getdate()) OR DATEPART(day,[LastResetDate]) <> DATEPART (day, getdate()));
    END
    ELSE BEGIN
        INSERT INTO [bx_Vars]([MaxPosts],[NewUserID],[TotalUsers],[YestodayPosts],[YestodayTopics],[MaxOnlineCount],[MaxPostDate],[MaxOnlineDate],[LastResetDate],[NewUsername])VALUES(@MaxPosts,@NewUserID,@TotalUsers,@YestodayPosts,@YestodayTopics,@MaxOnlineCount,@MaxPostDate,@MaxOnlineDate,@LastResetDate,@NewUsername);
    END
END
"
            )]
        #endregion
        public override void UpdateVars(Vars vars)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateVars";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@MaxPosts", vars.MaxPosts, SqlDbType.Int);
                query.CreateParameter<int>("@NewUserID", vars.NewUserID, SqlDbType.Int);
                query.CreateParameter<int>("@TotalUsers", vars.TotalUsers, SqlDbType.Int);
                query.CreateParameter<int>("@YestodayPosts", vars.YestodayPosts, SqlDbType.Int);
                query.CreateParameter<int>("@YestodayTopics", vars.YestodayTopics, SqlDbType.Int);
                query.CreateParameter<int>("@MaxOnlineCount", vars.MaxOnlineCount, SqlDbType.Int);
                query.CreateParameter<DateTime>("@MaxPostDate", vars.MaxPostDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@MaxOnlineDate", vars.MaxOnlineDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@LastResetDate", vars.LastResetDate, SqlDbType.DateTime);
                query.CreateParameter<string>("@NewUsername", vars.NewUsername, SqlDbType.NVarChar, 50);

                query.ExecuteNonQuery();
            }
        }

        #region 存储过程 UpdateStat
        [StoredProcedure(Name = "bx_UpdateNewUserVars", Script = @"
CREATE PROCEDURE {name}
    @GetVars bit
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NewUserID int,@NewUsername nvarchar(50),@TotalUser int;
    
    SELECT TOP 1 @NewUserID=UserID,@NewUsername=Username FROM [bx_Users] WHERE [IsActive] = 1 AND [UserID]<>0 ORDER BY [UserID] DESC;
    
    SELECT @TotalUser=Count(*) FROM [bx_Users] WHERE [IsActive] = 1 AND [UserID]<>0;

    IF @NewUserID IS NOT NULL BEGIN
        IF EXISTS(SELECT * FROM [bx_Vars])
            UPDATE [bx_Vars] SET  NewUserID = @NewUserID, NewUsername = @NewUsername, TotalUsers = @TotalUser WHERE [ID]=(SELECT TOP 1 ID FROM [bx_Vars]);
        ELSE
            INSERT [bx_Vars] (NewUserID,NewUsername,TotalUsers)VALUES(@NewUserID,@NewUsername,@TotalUser);

        IF @GetVars = 1
            SELECT TOP 1 * FROM [bx_Vars];
        SELECT 'ResetVars' AS XCMD; --, * FROM [bx_Vars];
    END
END
"
            )]
        #endregion
        public override Vars UpdateNewUserStat()
        {
            Vars stat = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateNewUserVars";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<bool>("@GetVars", true, SqlDbType.Bit);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stat = new Vars(reader);
                    }
                }
            }

            if (stat == null)
            {
                stat = new Vars();
                stat.NewUserID = 1;
                stat.NewUsername = "admin";
            }

            return stat;
        }
    }
}