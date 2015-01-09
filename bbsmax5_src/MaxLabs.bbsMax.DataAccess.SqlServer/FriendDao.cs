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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using System.Data.SqlClient;


namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class FriendDao : DataAccess.FriendDao
    {

        #region 为了 Passport 同步好友

        public override int AddFriendGroups(int userID, List<KeyValuePair<int, string>> friendGroup)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sbSql = new StringBuilder();
                int i=0;   
                string deleteExistGroupSql = "DELETE FROM bx_FriendGroups WHERE GroupID IN(";
                foreach (KeyValuePair<int, string> group in friendGroup)
                {
                    string groupName = "@GroupName_"+i;
                    string groupID = "@GroupID_"+i;
                 
                    sbSql.AppendFormat(" INSERT INTO bx_FriendGroups( GroupID, GroupName, UserID) VALUES({0},{1},@UserID);", groupID, groupName);
                    deleteExistGroupSql = string.Concat(deleteExistGroupSql,group.Key, ",");
                    query.CreateParameter<int>(groupID, group.Key, SqlDbType.Int);
                    query.CreateParameter<string>(groupName, group.Value, SqlDbType.NVarChar, 50);

                    i++;
                }

                deleteExistGroupSql = deleteExistGroupSql.TrimEnd(',') + ");";
                sbSql.Insert(0, deleteExistGroupSql); //删除已经存在的相同ID的好友分组--


                query.CommandText =sbSql.ToString();
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                return query.ExecuteNonQuery();
            }
        }

        public override int RenameFriendGroups(int userID, List<KeyValuePair<int, string>> newGroupNames)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sbSql = new StringBuilder();
                int i = 0;
                foreach (KeyValuePair<int, string> group in newGroupNames)
                {
                    string groupName = "@GroupName_" + i;
                    string groupID = "@GroupID_" + i;
                    sbSql.AppendFormat("UPDATE bx_FriendGroups SET GroupName = {1} WHERE GroupID = {0} AND UserID = @UserID;", groupID, groupName);

                    query.CreateParameter<int>(groupID, group.Key, SqlDbType.Int);
                    query.CreateParameter<string>(groupName, group.Value, SqlDbType.NVarChar, 50);

                    i++;
                }

                query.CommandText = sbSql.ToString();
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                return query.ExecuteNonQuery();
            }
        }

        public override int CreateFriends(int userID, List<KeyValuePair<int, int>> groupAndFriendUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sbSql = new StringBuilder();
                StringBuilder sbClear = new StringBuilder();

                sbClear.Append(" DELETE FROM  bx_Friends WHERE UserID = @UserID AND FriendUserID IN(");


                int i = 0;
                foreach (KeyValuePair<int, int> group in groupAndFriendUserID)
                {
                    string friendUserID = "@FriendUserID_" + i;
                    string groupID = "@GroupID_" + i;
                    sbSql.AppendFormat("INSERT INTO bx_Friends( UserID , FriendUserID , GroupID) VALUES(@UserID , {0}, {1});", friendUserID, groupID);
                    sbClear.Append(group.Value).Append(",");
                    query.CreateParameter<int>(groupID, group.Key, SqlDbType.Int);
                    query.CreateParameter<int>(friendUserID, group.Value, SqlDbType.Int);
                    i++;
                }

                if (groupAndFriendUserID.Count > 0)
                {
                    sbClear.Remove(sbClear.Length - 1, 1);
                    sbClear.Append(");");
                    sbSql.Insert(0, sbClear);
                }

                query.CommandText = sbSql.ToString();
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                return query.ExecuteNonQuery();
            }
        }


        public override int DeleteFriendGroups(int userID, IEnumerable<int> groupIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"DELETE FROM bx_Friends WHERE UserID = @UserID AND GroupID IN( @GroupIDs );
                                      DELETE FROM bx_FriendGroups WHERE UserID = @UserID AND GroupID IN( @GroupIDs );";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateInParameter<int>("@GroupIDs", groupIDs);

                return  query.ExecuteNonQuery();

            }
        }

        public override void DeleteFromBlackList(int userID, IEnumerable<int> blackUserIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_Friends WHERE GroupID =-1 AND UserID = @UserID AND FriendUserID IN (@FriendUserIDs)";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateInParameter<int>("@FriendUserIDs", blackUserIDs);

                query.ExecuteNonQuery();
            }
        }

        #endregion

        /*
        public override void Passport_WriteFriends(int userID, IEnumerable< FriendGroupProxy> groups) 
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder builder = new StringBuilder();
                int i = 0;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                builder.Append("SET IDENTITY_INSERT bx_FriendGroups ON ;");
                foreach (FriendGroupProxy fg in groups)
                {
                    int j = 0;
                    builder.AppendFormat("INSERT INTO bx_FriendGroups( GroupID, UserID, GroupName, TotalFriends, IsShield, CreateDate) VALUES( @GroupID_{0}, @UserID, @GroupName_{0}, @TotalFriends_{0}, @IsShield_{0},@CreateDate_{0} );", i);

                    foreach (MaxLabs.bbsMax.Passport.Friend f in fg.Friends)
                    {
                        builder.Append(" INSERT INTO  bx_Friends( UserID, FriendUserID,  GroupID, Hot, CreateDate) VALUES( @UserID, @FriendUserID_{0}_{1},  @GroupID_{0}, @Hot_{0}_{1}, @CreateDate_{0}_{1});", i, j);

                        query.CreateParameter<int>(string.Format("@FriendUserID_{0}_{1}", i, j), f.UserID, SqlDbType.Int);
                        query.CreateParameter<int>(string.Format("@Hot_{0}_{1}", i, j), f.Hot, SqlDbType.Int);
                        query.CreateParameter<DateTime>(string.Format("@CreateDate_{0}_{1}", i, j), f.CreateDate, SqlDbType.DateTime);
                        
                        j++;
                    }

                    query.CreateParameter<int>( string.Format("@GroupID_{0}",i),fg.GroupID,SqlDbType.Int);
                    query.CreateParameter<string>(string.Format("@GroupName_{0}", i), fg.Name, SqlDbType.NVarChar, 50);
                    query.CreateParameter<int>(string.Format("@TotalFriends_{0}", i), fg.TotalFriends, SqlDbType.Int);
                    query.CreateParameter<bool>(string.Format("@IsShield_{0}", i), fg.IsShield, SqlDbType.Bit);
                    query.CreateParameter<DateTime>(string.Format("@CreateDate_{0}", i), DateTimeUtil.Now, SqlDbType.DateTime);

                    i++;
                }
                builder.Append("SET IDENTITY_INSERT bx_FriendGroups OFF ;");

                query.CommandText = builder.ToString();
                query.CommandType = CommandType.Text;
                query.ExecuteNonQuery();
            }
        }
        */
        #region ====== 以下为好友分组的操作 =========

        [StoredProcedure(Name = "bx_GetFriendGroups", Script = @"
CREATE PROCEDURE {name}
    @UserID int
AS BEGIN

	SET NOCOUNT ON;

    SELECT * FROM [bx_FriendGroups] WITH (NOLOCK) WHERE UserID = @UserID;

END
")]
        public override FriendGroupCollection GetFriendGroups(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetFriendGroups";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new FriendGroupCollection(reader);
                }
            }
        }

        [StoredProcedure(Name = "bx_AddFriendGroup", Script = @"
CREATE PROCEDURE {name}
    @UserID int,
    @GroupID int,
    @GroupName nvarchar(50),
    @MaxGroupCount int
AS BEGIN

	SET NOCOUNT ON;
    
    IF EXISTS ( SELECT * FROM bx_FriendGroups WHERE UserID = @UserID AND GroupName = @GroupName )
        RETURN(2);

    IF (SELECT COUNT(*) FROM bx_FriendGroups WHERE UserID = @UserID) > @MaxGroupCount
        RETURN(3);

IF @GroupID IS NULL BEGIN
    INSERT INTO bx_FriendGroups ( UserID, GroupName ) VALUES ( @UserID, @GroupName );
    SELECT * FROM bx_FriendGroups WHERE GroupID = @@IDENTITY;
END
ELSE BEGIN 
    INSERT INTO bx_FriendGroups (GroupID, UserID, GroupName ) VALUES (@GroupID, @UserID, @GroupName );
    SELECT * FROM bx_FriendGroups WHERE GroupID = @GroupID;
END
    RETURN (0);

END
")]
        public override int AddFriendGroup(int userID, int? groupID, string friendGroupName, int maxGroupCount, out FriendGroup newGroup)
        {
            int result;
            newGroup = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_AddFriendGroup";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int?>("@GroupID", groupID, SqlDbType.Int);
                query.CreateParameter<string>("@GroupName", friendGroupName, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@MaxGroupCount", maxGroupCount, SqlDbType.Int);
                query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Next)
                        newGroup = new FriendGroup(reader);
                }

                result = (int)query.Parameters["@ReturnValue"].Value;
            }
            return result;
        }


        [StoredProcedure(Name = "bx_DeleteFriendGroup", Script = @"
CREATE PROCEDURE {name}
    @UserID int,
    @GroupID int,
    @DeleteFriends bit
AS BEGIN

	SET NOCOUNT ON;

    IF @GroupID = 0
        RETURN (2);

    IF @DeleteFriends = 0
        UPDATE bx_Friends SET GroupID = 0 WHERE UserID = @UserID AND GroupID = @GroupID;

    DELETE bx_FriendGroups WHERE UserID = @UserID AND GroupID = @GroupID;

    RETURN (0);

END
")]
        public override bool DeleteFriendGroup(int userID, int groupID, bool deleteFriends)
        {
            bool result;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_DeleteFriendGroup";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@GroupID", groupID, SqlDbType.Int);
                query.CreateParameter<bool>("@DeleteFriends", deleteFriends, SqlDbType.Bit);
                query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                result = (int)query.Parameters["@ReturnValue"].Value == 0;
            }
            return result;
        }


        public override void DeleteFromBlackList(int myId, int blackUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_Friends WHERE GroupID =-1 AND UserID = @UserID AND FriendUserID = @FriendUserID";
                query.CreateParameter<int>("@UserID", myId, SqlDbType.Int);
                query.CreateParameter<int>("@FriendUserID", blackUserID, SqlDbType.Int);
                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_RenameFriendGroup", Script = @"
CREATE PROCEDURE {name}
    @UserID int,
    @GroupID int,
    @NewGroupName nvarchar(50)
AS BEGIN

	SET NOCOUNT ON;

    IF @GroupID = 0
        RETURN (2);

    ELSE IF EXISTS (SELECT * FROM bx_FriendGroups WHERE UserID = @UserID AND GroupName = @NewGroupName AND GroupID <> @GroupID)
        RETURN (3);

    ELSE BEGIN
        UPDATE bx_FriendGroups SET GroupName = @NewGroupName WHERE UserID = @UserID AND GroupID = @GroupID;
        RETURN (0);
    END

END
")]
        public override int RenameFriendGroup(int userID, int groupID, string newGroupName)
        {
            int result;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_RenameFriendGroup";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@GroupID", groupID, SqlDbType.Int);
                query.CreateParameter<string>("@NewGroupName", newGroupName, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                result = (int)query.Parameters["@ReturnValue"].Value;
            }

            return result;
        }

        [StoredProcedure(Name = "bx_ShieldFriendGroup", Script = @"
CREATE PROCEDURE {name}
    @UserID int,
    @GroupID int,
    @IsShield bit
AS BEGIN

	SET NOCOUNT ON;

    IF @GroupID = 0
        RETURN (2);

    UPDATE bx_FriendGroups SET IsShield = @IsShield WHERE UserID = @UserID AND GroupID = @GroupID;

    IF @@ROWCOUNT = 1
        RETURN (0);
    ELSE
        RETURN (3);

END
")]
        public override bool ShieldFriendGroup(int userID, int groupID, bool isShield)
        {
            bool result;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_ShieldFriendGroup";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@GroupID", groupID, SqlDbType.Int);
                query.CreateParameter<bool>("@IsShield", isShield, SqlDbType.Bit);
                query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                result = (int)query.Parameters["@ReturnValue"].Value == 0;
            }

            return result;
        }

        public override void ShieldFriendGroups(int userID, IEnumerable<int> groupIds, bool isShield)
        {

            using (SqlQuery query = new SqlQuery())
            {
                if (ValidateUtil.HasItems<int>(groupIds) == false)
                {
                    query.CommandText = @"
IF @IsShield = 0
    UPDATE bx_FriendGroups SET IsShield = 1 WHERE UserID = @UserID;
ELSE
    UPDATE bx_FriendGroups SET IsShield = 0 WHERE UserID = @UserID;
";
                }
                else
                {
                    query.CommandText = @"
UPDATE bx_FriendGroups SET IsShield = @IsShield WHERE UserID = @UserID AND GroupID IN ( @GroupIds );
IF @IsShield = 0
    UPDATE bx_FriendGroups SET IsShield = 1 WHERE UserID = @UserID AND GroupID NOT IN ( @GroupIds );
ELSE
    UPDATE bx_FriendGroups SET IsShield = 0 WHERE UserID = @UserID AND GroupID NOT IN ( @GroupIds );
";
                    query.CreateInParameter<int>("@GroupIds", groupIds);
                }

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateParameter<bool>("@IsShield", isShield, SqlDbType.Bit);

                query.ExecuteNonQuery();

            }

        }

        #endregion

        #region ====== 以下为好友的操作 =========

        [StoredProcedure(Name = "bx_GetFriendsAndBlacklist", Script = @"
CREATE PROCEDURE {name}
    @UserID int
AS BEGIN

	SET NOCOUNT ON;

    SELECT * FROM [bx_Friends] WITH (NOLOCK) WHERE UserID = @UserID ORDER BY [Hot] DESC,[CreateDate] ASC;

END
")]
        public override FriendCollection GetFriendsAndBlacklist(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetFriendsAndBlacklist";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new FriendCollection(reader);
                }
            }
        }

        public override IList<int> SelectMayFriendIDs(int userID, int count)
        {
            List<int> mayFriendIDs = new List<int>();

            string sql = @"SELECT DISTINCT TOP (@Count) FriendUserID 
FROM bx_Friends WITH (NOLOCK)
WHERE UserID IN(SELECT FriendUserID FROM bx_Friends WHERE UserID=@UserID)
AND FriendUserID NOT IN(SELECT FriendUserID FROM bx_Friends WITH (NOLOCK) WHERE UserID=@UserID) AND FriendUserID <> @UserID;";
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql;

                query.CreateTopParameter("@Count", count);

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mayFriendIDs.Add(reader.Get<int>("FriendUserID"));
                    }
                }
            }
            return mayFriendIDs;
        }

        //public override void ModifyFriendGroupName(int userID, string friendGroups)
        //{
        //    using (SqlQuery query = new SqlQuery())
        //    {
        //        query.CommandText = "UPDATE [bx_Users] SET FriendGroups = @FriendGroups WHERE UserID = @UserID";

        //        query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
        //        query.CreateParameter<string>("@FriendGroups", friendGroups, SqlDbType.NVarChar, 500);

        //        query.ExecuteNonQuery();
        //    }
        //}

        public override void MoveFriends(int userID, IEnumerable<int> friendUserIds, int groupID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE [bx_Friends] SET GroupID = @FriendGroupID WHERE UserID = @UserID AND FriendUserID IN (@FriendUserIds)";

                query.CreateParameter<int>("@FriendGroupID", groupID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateInParameter<int>("@FriendUserIds", friendUserIds);

                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_MoveFriend", Script = @"
CREATE PROCEDURE {name}
    @UserID int,
    @FriendUserID int,
    @FriendGroupID int
AS BEGIN

	SET NOCOUNT ON;

    UPDATE [bx_Friends] SET GroupID = @FriendGroupID WHERE UserID = @UserID AND FriendUserID = @FriendUserID;

END
")]
        public override void MoveFriend(int userID, int friendUserID, int groupID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_MoveFriend";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@FriendUserID", friendUserID, SqlDbType.Int);
                query.CreateParameter<int>("@FriendGroupID", groupID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_DeleteFriend", Script = @"
CREATE PROCEDURE {name}
    @UserID int,
    @FriendUserID int
AS BEGIN

	SET NOCOUNT ON;

    DELETE FROM [bx_Friends] WHERE UserID = @UserID AND FriendUserID = @FriendUserID;
    DELETE FROM [bx_Friends] WHERE UserID = @FriendUserID AND FriendUserID = @UserID;

END
")]
        public override void DeleteFriend(int userID, int friendUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_DeleteFriend";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@FriendUserID", friendUserID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }


        public override void DeleteFriends(int userID, IEnumerable<int> friendUserIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"DELETE FROM [bx_Friends] WHERE UserID = @UserID AND FriendUserID IN (@FriendUserIds);
DELETE FROM [bx_Friends] WHERE UserID IN (@FriendUserIds) AND FriendUserID = @UserID;";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateInParameter<int>("@FriendUserIds", friendUserIds);

                query.ExecuteNonQuery();
            }
        }


        [StoredProcedure(Name = "bx_AcceptAddFriend", Script = @"
CREATE PROCEDURE {name}
    @UserID int,
    @FriendUserID int,
    @FromFriendGroupID int,
    @FriendGroupID int
AS BEGIN

SET NOCOUNT ON;

UPDATE bx_Friends SET GroupID = @FriendGroupID WHERE UserID = @UserID AND FriendUserID = @FriendUserID

IF @@RowCount=0 BEGIN
   INSERT INTO [bx_Friends] (Hot,UserID,FriendUserID,GroupID) VALUES (0, @UserID, @FriendUserID, @FriendGroupID); 
END

UPDATE bx_Friends SET GroupID = @FromFriendGroupID WHERE UserID = @FriendUserID AND FriendUserID = @UserID

IF @@RowCount=0 BEGIN
  INSERT INTO [bx_Friends] (Hot,UserID,FriendUserID,GroupID) VALUES (0, @FriendUserID, @UserID,@FromFriendGroupID );
END

END
")]
        public override void AcceptAddFriend(int operatorID, int tryAddUserID, int tryAddGroupID, int groupIDToAdd)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_AcceptAddFriend";

                query.CreateParameter<int>("@UserID", operatorID, SqlDbType.Int);
                query.CreateParameter<int>("@FriendUserID", tryAddUserID, SqlDbType.Int);
                query.CreateParameter<int>("@FromFriendGroupID", tryAddGroupID, SqlDbType.Int);
                query.CreateParameter<int>("@FriendGroupID", groupIDToAdd, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_UpdateFriendHot", Script = @"
CREATE PROCEDURE {name}
    @UserID int,
    @FriendUserID int,
    @Hot int
AS BEGIN

	SET NOCOUNT ON;

    UPDATE [bx_Friends] SET Hot = Hot + @Hot WHERE UserID = @UserID AND FriendUserID = @FriendUserID;

END
")]
        public override void UpdateFriendHot(HotType type, int userID, int friendUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_UpdateFriendHot";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@FriendUserID", friendUserID, SqlDbType.Int);
                query.CreateParameter<int>("@Hot", (int)type, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        #endregion


        #region 黑名单

        [StoredProcedure(Name = "bx_AddUsersToBlacklist", Script = @"
CREATE PROCEDURE {name}
     @UserID int
    ,@UserIdsToAdd text
AS
BEGIN

    --GroupID为-1的好友分组为黑名单

    SET NOCOUNT ON;

    DECLARE @AddTable table(TargetUserID int, IsUpdate bit DEFAULT(0));

    INSERT INTO @AddTable (TargetUserID) SELECT item FROM bx_GetIntTable(@UserIdsToAdd,',') AS T WHERE T.item != @UserID;

    --标记出那些用户本来在好友列表中。这部分只需更新GroupID为-1即可
    UPDATE @AddTable SET IsUpdate = 1 WHERE TargetUserID IN ( SELECT FriendUserID FROM bx_Friends WHERE UserID = @UserID);

    UPDATE bx_Friends SET GroupID = -1 WHERE UserID = @UserID AND FriendUserID IN ( SELECT TargetUserID FROM @AddTable WHERE IsUpdate = 1 );

    --插入好友列表
    INSERT INTO bx_Friends ( UserID, FriendUserID, GroupID, Hot ) SELECT @UserID, TargetUserID, -1, 0 FROM @AddTable T WHERE T.IsUpdate = 0;

    DELETE bx_Friends WHERE FriendUserID = @UserID AND GroupID != -1 AND UserID in(SELECT TargetUserID FROM @AddTable);
END")]
        public override bool AddUsersToBlacklist(int userID, IEnumerable<int> userIdsToAdd)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_AddUsersToBlacklist";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateParameter<string>("@UserIdsToAdd", StringUtil.Join(userIdsToAdd), SqlDbType.Text);

                query.ExecuteNonQuery();
            }

            return true;
        }

        #endregion

    }
}