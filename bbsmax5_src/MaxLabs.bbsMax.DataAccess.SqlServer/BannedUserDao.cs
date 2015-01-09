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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class BannedUserDao : DataAccess.BannedUserDao
    {
        [StoredProcedure(Name = "bx_GetAllBannedUsers", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM bx_BannedUsers WHERE EndDate > GETDATE() ORDER BY ForumID;
END
")]
        public override BannedUserCollection GetAllBannedUsers()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAllBannedUsers";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new BannedUserCollection(reader);
                }

            }
        }



        public override void BanUser(string operatorname, BanType bantype, DateTime operationtime, string cause, Dictionary<int, DateTime> foruminfos, int userid, string targetname, string userip)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuffer sqlBuffer = new StringBuffer();
                sqlBuffer += "DELETE FROM bx_BannedUsers WHERE UserID = @UserID;";
                sqlBuffer += "INSERT INTO bx_BanUserLogs(OperationType,OperationTime,OperatorName,Cause,UserID,Username,UserIP,AllBanEndDate) Values(@OperationType,@OperationTime,@OperatorName,@Cause,@UserID,@Username,@UserIP,@AllBanEndDate);DECLARE @logid int;SELECT @logid=@@IDENTITY;";
                foreach (KeyValuePair<int, DateTime> forumBanInfo in foruminfos)
                {
                    sqlBuffer += string.Format("INSERT INTO bx_BannedUsers( UserID, ForumID, EndDate, Cause ) VALUES(@UserID , @ForumID{0},@EndDate{0}, @Cause);", forumBanInfo.Key);
                    sqlBuffer += string.Format("INSERT INTO bx_BanUserLogForumInfos(LogID,ForumID,ForumName,EndDate) Select @logid,@ForumID{0},ForumName,@EndDate{0} FROM [bx_Forums] WHERE ForumID=@ForumID{0};", forumBanInfo.Key);
                    query.CreateParameter<int>(string.Format("@ForumID{0}", forumBanInfo.Key), forumBanInfo.Key, SqlDbType.Int);
                    query.CreateParameter<DateTime>(string.Format("@EndDate{0}", forumBanInfo.Key), forumBanInfo.Value, SqlDbType.DateTime);
                }
                query.CreateParameter<int>("@OperationType", (int)bantype, SqlDbType.Int);
                query.CreateParameter<DateTime>("@OperationTime", operationtime, SqlDbType.DateTime);
                query.CreateParameter<string>("@OperatorName", operatorname, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Cause", cause, SqlDbType.NVarChar, 1000);
                query.CreateParameter<int>("@UserID", userid, SqlDbType.Int);
                query.CreateParameter<string>("@Username", targetname, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@UserIP", userip, SqlDbType.NVarChar, 50);

                DateTime allBanEndDate;
                if (foruminfos.TryGetValue(0, out allBanEndDate))
                {
                    query.CreateParameter<DateTime>("@AllBanEndDate", allBanEndDate, SqlDbType.DateTime);
                }
                else
                {
                    query.CreateParameter<DateTime>("@AllBanEndDate", DateTime.MaxValue, SqlDbType.DateTime);
                }

                query.CommandText = sqlBuffer.ToString();
                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_CancelBanByForumID", Script = @"
CREATE PROCEDURE {name}
 @UserIDs          varchar(8000)
,@ForumID          int
,@OperatorName    nvarchar(50)
,@OperationType    varchar(50)
,@UserIP            varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM bx_BannedUsers WHERE UserID IN( SELECT item FROM  bx_GetIntTable(@UserIDs,',')) AND ForumID = @ForumID;
    INSERT INTO bx_BanUserLogs(UserID,Username,UserIP,OperatorName,OperationType) SELECT item,Username,@UserIP,@OperatorName,@OperationType FROM  bx_GetIntTable(@UserIDs,',') LEFT JOIN bx_Users ON item=UserID;
END
")]
        public override void CancelBan(IEnumerable<int> userIds, int ForumID, string operationName, string userip)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CancelBanByForumID";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@UserIDs", StringUtil.Join(userIds), SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@ForumID", ForumID, SqlDbType.Int);
                query.CreateParameter<string>("@OperatorName", operationName, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@OperationType", (int)BanType.UnBan, SqlDbType.Int);
                query.CreateParameter<string>("@UserIP", userip, SqlDbType.VarChar, 50);

                query.ExecuteNonQuery();
            }
        }


        [StoredProcedure(Name = "bx_CancelBan", Script = @"
CREATE PROCEDURE {name}
 @UserIDs           varchar(8000)
,@OperatorName     nvarchar(50)
,@OperationType     varchar(50)
,@UserIP            varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM bx_BannedUsers WHERE UserID IN(SELECT item FROM bx_GetIntTable(@UserIDs,','));
    INSERT INTO bx_BanUserLogs(UserID,Username,UserIP,OperatorName,OperationType) SELECT item,Username,@UserIP,@OperatorName,@OperationType FROM  bx_GetIntTable(@UserIDs,',') LEFT JOIN bx_Users ON item=UserID;
END
")]
        public override void CancelBan(IEnumerable<int> userIds, string operationName, string userip)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CancelBan";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@UserIDs", StringUtil.Join(userIds), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@OperatorName", operationName, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@OperationType", (int)BanType.UnBan, SqlDbType.Int);
                query.CreateParameter<string>("@UserIP", userip, SqlDbType.VarChar, 50);

                query.ExecuteNonQuery();
            }
        }

        public override SimpleUserCollection GetBannedUsers(int ForumID, int pageSize, int pageNumber, out int totalCount)
        {
            totalCount = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_SimpleUser";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.SelectCount = true;
                query.Pager.PrimaryKey = "[UserID]";
                query.Pager.Condition = " UserID IN(SELECT UserID FROM bx_BannedUsers WHERE ForumID = @ForumID AND EndDate > GETDATE())";
                query.CreateParameter<int>("@ForumID", ForumID, SqlDbType.Int);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    SimpleUserCollection users = new SimpleUserCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            totalCount = reader.GetInt32(0);
                    }

                    return users;
                }

            }
        }

        [StoredProcedure(Name="bx_BanUsersWholeForum",Script= @"
CREATE PROCEDURE {name}
 @UserIDs            varchar(8000)
,@OperatorName      nvarchar(50)
,@UserIP            varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM bx_BannedUsers WHERE UserID IN(SELECT item FROM bx_GetIntTable(@UserIDs,','));
    INSERT INTO bx_BannedUsers(UserID,ForumID) SELECT item,0 FROM bx_GetIntTable(@UserIDs,',') INNER JOIN bx_Users ON item=UserID;
    INSERT INTO bx_BanUserLogs(UserID,Username,UserIP,OperatorName,OperationType) SELECT item,Username,@UserIP,@OperatorName,2 FROM bx_GetIntTable(@UserIDs,',') INNER JOIN bx_Users ON item=UserID;
END
")]

        public override void BanUsersWholeForum(string operatorName, IEnumerable<int> userIds, string userIP)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_BanUsersWholeForum";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@UserIDs", StringUtil.Join(userIds), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@OperatorName", operatorName, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@UserIP", userIP, SqlDbType.VarChar, 50);
                query.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// 屏蔽用户
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="forumID">版块ID（如果是单个0的话，代表整站屏蔽）</param>
        /// <param name="EndDate">解除屏蔽时间</param>
    /*    public override void BanUser(int userID, Dictionary<int, DateTime> forumInfos, string cause)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuffer sqlBuffer = new StringBuffer();
                sqlBuffer += "DELETE FROM bx_BannedUsers WHERE UserID = @UserID;";
                foreach (KeyValuePair<int, DateTime> forumBanInfo in forumInfos)
                {
                    sqlBuffer += string.Format("INSERT INTO bx_BannedUsers( UserID, ForumID, EndDate, Cause ) VALUES(@UserID , @ForumID{0},@EndDate{0}, @Cause);", forumBanInfo.Key);
                    query.CreateParameter<int>(string.Format("@ForumID{0}", forumBanInfo.Key), forumBanInfo.Key, SqlDbType.Int);
                    query.CreateParameter<DateTime>(string.Format("@EndDate{0}", forumBanInfo.Key), forumBanInfo.Value, SqlDbType.DateTime);
                }
                query.CreateParameter<string>("@Cause", cause, SqlDbType.NVarChar, 1000);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CommandText = sqlBuffer.ToString();
                query.ExecuteNonQuery();
            }
        }

        public override void BanUser(int userID, int forumID, DateTime endDate, string cause)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"DELETE FROM bx_BannedUsers WHERE UserID=@UserID AND ForumID=@ForumID;
                INSERT INTO bx_BannedUsers( UserID, ForumID, EndDate, Cause) VALUES( @UserID,@ForumID,@EndDate, @Cause );";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);
                query.CreateParameter<string>("@Cause", cause, SqlDbType.NVarChar, 1000);

                query.ExecuteNonQuery();
            }
        }
        */
                

    }
}