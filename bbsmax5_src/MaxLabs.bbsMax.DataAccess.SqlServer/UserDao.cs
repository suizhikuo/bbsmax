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
using System.Data.SqlClient;
using System.Collections;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Rescourses;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class UserDao : DataAccess.UserDao
    {
        public override ConsoleLoginLogCollection GetConsoleLoginLogs(int count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT TOP (@TopCount) * FROM bx_AdminSessions ORDER BY CreateDate DESC";

                query.CreateTopParameter("@TopCount", count);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    ConsoleLoginLogCollection clc = new ConsoleLoginLogCollection(reader);
                    return clc;
                }
            }
        }

        public override Dictionary<string, int> GetUserRankInfo(int userID, params string[] fileds)
        {
            List<string> allowRankFields = new List<string>(new string[] { "Points", "Point_1", "Point_2", "Point_3", "Point_4", "Point_5", "Point_6", "Point_7", "Point_8", "TotalPoints", "TotalTopics", "TotalPosts", "TotalInvite", "TotalOnlineTime", "MonthOnlineTime", "SpaceViews", "TotalFriends" });
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder("SELECT ");
            foreach (string s in fileds)
            {

                if (!allowRankFields.Contains(s))
                {
                    throw new Exception(string.Format("无法获取字段{0}的排名", s));
                }

                sb.AppendFormat(@"DECLARE @{0} int;
                                DECLARE @R_{0} int;
                               SET @{0} = (SELECT {0} FROM bx_Members WHERE UserID = @UserID);
                               SET @R_{0} = (SELECT COUNT(UserID) FROM bx_Members WHERE {0} > @{0})+1;
                ", s);

                sb2.AppendFormat("@R_{0} AS {0},", s);
            }
            sb2.Remove(sb2.Length - 1, 1);

            string sql = string.Concat(sb, sb2);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql;
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    Dictionary<string, int> result = new Dictionary<string, int>();
                    while (reader.Next)
                    {
                        foreach (string s in fileds)
                        {
                            result.Add(s, reader.Get<int>(s));
                        }
                    }
                    return result;
                }
            }
        }

        [StoredProcedure(Name = "bx_UpdateMaxSystemNotifyID", Script = @"
CREATE PROCEDURE {name}
@UserID      int,
@NotifyID    int
AS
BEGIN
    SET NOCOUNT ON
    UPDATE bx_UserVars SET LastReadSystemNotifyID = @NotifyID WHERE UserID = @UserID;
END
")]
        public override void UpdateMaxSystemNotifyID(int userID, int sysNotifyID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateMaxSystemNotifyID";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@NotifyID", sysNotifyID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 取得用户的通知设置
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_GetUserNotifySetting", Script = @"
CREATE PROCEDURE {name}
@UserID      int
AS
BEGIN
    SET NOCOUNT ON
    SELECT NotifySetting FROM bx_UserInfos WITH (NOLOCK) WHERE UserID = @UserID;
END
")]
        public override UserNotifySetting GetUserNotifySetting(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUserNotifySetting";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                string value = query.ExecuteScalar<string>();
                return new UserNotifySetting(value);
            }
        }

        public override void SetUserNotifySetting(int userID, UserNotifySetting setting)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string sql = "UPDATE bx_UserInfos SET NotifySetting = @Setting WHERE UserID = @UserID";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Setting", setting.ToString(), SqlDbType.VarChar, 4000);
                query.CommandText = sql;
                query.ExecuteNonQuery();
            }
        }

        public override void AdminLogout(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_AdminSessions SET Available = 0 WHERE UserID = @UserID AND Available = 1";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.ExecuteNonQuery();
            }
        }

        public override bool HasAdminSession(int userID, int expiresMinute, string ip, out Guid sessionID)
        {
            sessionID = Guid.Empty;
            using (SqlQuery query = new SqlQuery())
            {
                DateTime ExpiresDate = DateTimeUtil.Now.AddMinutes(-expiresMinute);
                query.CommandText = "UPDATE bx_AdminSessions SET UpdateDate = GETDATE() WHERE IpAddress=@IP AND UserID = @UserID AND Available = 1 AND UpdateDate > @UpdateDate ; SELECT SessionID FROM bx_AdminSessions WHERE IpAddress=@IP AND UserID = @UserID AND UpdateDate > @UpdateDate AND Available =1";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@UpdateDate", ExpiresDate, SqlDbType.DateTime);
                query.CreateParameter<string>("@IP", ip, SqlDbType.VarChar, 100);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        sessionID = reader.GetGuid(0);
                        return true;
                    }
                }
            }

            return false;
        }

        public override Guid CreateAdminSession(int userID, string IpAddress)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = " UPDATE bx_AdminSessions SET Available = 0 WHERE UserID = @UserID; INSERT INTO bx_AdminSessions(SessionID,UserID,IpAddress) VALUES(@SessionID,@UserID,@IpAddress);";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@IpAddress", IpAddress, SqlDbType.VarChar, 100);
                Guid sessionID = Guid.NewGuid();
                query.CreateParameter<Guid>("@SessionID", sessionID, SqlDbType.UniqueIdentifier);
                query.ExecuteNonQuery();
                return sessionID;
            }
        }

        [StoredProcedure(Name = "bx_EmailIsExists", Script = @"
CREATE PROCEDURE {name} 
    @Email    nvarchar(200)
AS BEGIN

    SET NOCOUNT ON;

    IF EXISTS(SELECT [UserID] FROM bx_Users WITH(NOLOCK) WHERE [Email] = @Email) BEGIN
	    SELECT 1;
    END
    ELSE BEGIN
        SELECT 0;
    END
END
")]
        public override bool EmailIsExsits(string email)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CreateParameter<string>("@Email", email, SqlDbType.NVarChar, 200);
                query.CommandText = "bx_EmailIsExists";
                query.CommandType = CommandType.StoredProcedure;
                object result = query.ExecuteScalar();

                return Convert.ToInt32(result) == 1;
            }
        }

        public override void SetSidebarStatus(int userID, EnableStatus status)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_UserVars SET EnableDisplaySidebar = @Status WHERE UserID = @UserID";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<byte>("@Status", (byte)status, SqlDbType.TinyInt);
                query.ExecuteNonQuery();
            }
        }

        public override UserCollection GetRoleMembers(LevelLieOn levelLieOn, Int32Scope levelScope, int pageSize, int pageNumber, out int totalCount)
        {
            string SearchField = "";

            switch (levelLieOn)
            {
                case LevelLieOn.OnlineTime:
                    SearchField = "[TotalOnlineTime]";
                    break;
                case LevelLieOn.Point:
                    SearchField = "[Points]";
                    break;
                case LevelLieOn.Post:
                    SearchField = "[TotalPosts]";
                    break;
                case LevelLieOn.Topic:
                    SearchField = "[TotalTopics]";
                    break;
            }

            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Members";
                query.Pager.SortField = SearchField;
                query.Pager.PageSize = pageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PrimaryKey = "[UserID]";
                query.Pager.Condition = string.Format(" {0}>=@LowValue AND {0}<@HighValue", SearchField);
                query.CreateParameter<int>("@LowValue", levelScope.MinValue, SqlDbType.Int);
                query.CreateParameter<int>("@HighValue", levelScope.MaxValue, SqlDbType.Int);
                query.Pager.SelectCount = true;
                totalCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserCollection users = new UserCollection(reader);
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                    return users;
                }
            }
        }

        public override UserCollection GetRoleMembers(Guid roleID, int pageSize, int pageNumber, out int totalCount)
        {
            UserCollection users = new UserCollection();
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_Users]";
                query.Pager.PageSize = pageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.SelectCount = true;
                query.Pager.SortField = "[UserID]";
                query.Pager.PrimaryKey = "[UserID]";
                query.Pager.Condition = " UserID IN( SELECT UserID FROM bx_UserRoles WHERE RoleID = @RoleID AND BeginDate <= GETDATE() AND EndDate >= GETDATE())";
                query.CreateParameter<Guid>("@RoleID", roleID, SqlDbType.UniqueIdentifier);

                totalCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    users = new UserCollection(reader);
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                }
            }
            return users;
        }


        //public override UserCollection GetLastUpdateUsers(int count)
        //{
        //    using (SqlQuery query = new SqlQuery())
        //    {
        //        query.CommandText = "SELECT TOP (@Count) * FROM bx_Members ORDER BY [UpdateDate] DESC";

        //        query.CreateTopParameter("@Count", count);

        //        using (XSqlDataReader reader = query.ExecuteReader())
        //        {
        //            UserCollection users = new UserCollection(reader);
        //            return users;
        //        }
        //    }
        //}

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="username">用户名</param>
        /// <param name="email">邮箱</param>
        /// <param name="password">密码</param>
        /// <param name="passwordFormat">密码格式</param>
        /// <param name="ip">IP地址</param>
        /// <param name="serial">邀请码</param>
        /// <param name="IsActive">是否激活</param>
        /// <param name="ipInterval">相同IP地址注册间隔时间</param>
        /// <param name="checkRealname"></param>
        /// <returns></returns>
        #region 存储过程
        [StoredProcedure(Name = "bx_Register", FileName = @"bx_Register.sql")]
        #endregion
        public override int Register(ref int userID, string username, string email, string password, EncryptFormat passwordFormat, UserRoleCollection initRoles, string ip, Guid? serial, int inviterID, bool IsActive, int[] userPoints, int ipInterval)
        {
            int result = 0;

            using (SqlQuery query = new SqlQuery())
            {
                SqlParameter outputValue = query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                outputValue.Direction = ParameterDirection.InputOutput;

                SqlParameter returnValue = query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.CommandText = "bx_Register";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@Username", username, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Password", password, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Email", email, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@CreateIP", ip, SqlDbType.VarChar, 50);
                query.CreateParameter<int>("@IPSameInterval", ipInterval, SqlDbType.Int);
                query.CreateParameter<Int16>("@PasswordFormat", (Int16)passwordFormat, SqlDbType.SmallInt);
                query.CreateParameter<Guid?>("@Serial", serial, SqlDbType.UniqueIdentifier);
                query.CreateParameter<bool>("@IsActive", IsActive, SqlDbType.Int);

#if !Passport
                query.CreateParameter<byte>("@BlogPrivacy", (byte)AllSettings.Current.PrivacySettings.BlogPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<byte>("@FeedPrivacy", (byte)AllSettings.Current.PrivacySettings.FeedPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<byte>("@BoardPrivacy", (byte)AllSettings.Current.PrivacySettings.BoardPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<byte>("@DoingPrivacy", (byte)AllSettings.Current.PrivacySettings.DoingPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<byte>("@AlbumPrivacy", (byte)AllSettings.Current.PrivacySettings.AlbumPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<byte>("@SpacePrivacy", (byte)AllSettings.Current.PrivacySettings.SpacePrivacy, SqlDbType.TinyInt);
                query.CreateParameter<byte>("@SharePrivacy", (byte)AllSettings.Current.PrivacySettings.SharePrivacy, SqlDbType.TinyInt);
                query.CreateParameter<byte>("@FriendListPrivacy", (byte)AllSettings.Current.PrivacySettings.FriendListPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<byte>("@InformationPrivacy", (byte)AllSettings.Current.PrivacySettings.InformationPrivacy, SqlDbType.TinyInt);
#endif

                StringBuilder roleIDs = new StringBuilder();
                StringBuilder roleEndDates = new StringBuilder();
                if (initRoles != null)
                {
                    foreach (UserRole ur in initRoles)
                    {
                        roleIDs.Append(ur.RoleID);
                        roleIDs.Append(",");

                        roleEndDates.Append(ur.EndDate);
                        roleEndDates.Append(",");
                    }
                }

                if (roleIDs.Length > 0)
                {
                    roleIDs.Remove(roleIDs.Length - 1, 1);
                }

                if (roleEndDates.Length > 0)
                {
                    roleEndDates.Remove(roleEndDates.Length - 1, 1);
                }

                query.CreateParameter<string>("@RoleIDs", roleIDs.ToString(), SqlDbType.Text);
                query.CreateParameter<string>("@RoleEndDates", roleEndDates.ToString(), SqlDbType.Text);

                query.CreateParameter<int>("@InviterID", inviterID, SqlDbType.Int);


                for (int i = 0; i < 8; i++)
                {
                    query.CreateParameter<int>("@Point" + i, userPoints[i], SqlDbType.Int);
                }

                query.ExecuteScalar();

                if (outputValue.Value != DBNull.Value)
                    userID = Convert.ToInt32(outputValue.Value);

                result = Convert.ToInt32(returnValue.Value);
            }

            return result;
        }

        /// <summary>
        /// 应用临时的真实姓名
        /// </summary>
        /// <param name="userIds"></param>
        public override void UseTemporaryRealname(IEnumerable<int> userIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_UserTempRealname SET Realname = TempRealname WHERE UserID IN (@UserIds)";
                query.CommandType = CommandType.Text;
                query.CreateInParameter<int>("@UserIds", userIds);
                query.ExecuteNonQuery();
            }
        }

        #region 后台管理员更改用户资料
        [StoredProcedure(Name = "bx_AdminUpdateUserProfile", Script = @"
CREATE PROCEDURE {name} 
 @UserID          int
,@Realname        nvarchar(50)
,@Gender          int
,@Email           nvarchar(200)
,@BirthYear       smallint
,@Birthday        smallint
,@EmailValidated  bit
,@SignatureFormat int
,@Signature       nvarchar(1500) 
--,@ExtendFields    ntext
,@IsActive        bit
AS
BEGIN

    SET NOCOUNT ON;

    UPDATE bx_Users SET Realname = @Realname, Email = @Email, Gender = @Gender,  EmailValidated = @EmailValidated, Signature = @Signature, SignatureFormat = @SignatureFormat, IsActive = @IsActive WHERE [UserID] = @UserID;
    UPDATE bx_UserInfos SET  BirthYear = @BirthYear,  Birthday = @Birthday WHERE UserID = @UserID;

END
")]

        #endregion
        public override bool AdminUpdateUserProfile(
            int userID,
            string realname,
            string email,
            Gender gender,
            DateTime Birthday,
            bool isActive,
            bool emailValidated,
            string signature,
            SignatureFormat signatureFormat,
            UserExtendedValueCollection extendedFields
            )
        {
            using (SqlSession db = new SqlSession())
            {
                db.BeginTransaction();

                using (SqlQuery query = db.CreateQuery())
                {
                    query.CommandText = "bx_AdminUpdateUserProfile";
                    query.CommandType = CommandType.StoredProcedure;

                    query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                    query.CreateParameter<string>("@Realname", realname, SqlDbType.NVarChar, 50);
                    query.CreateParameter<string>("@Email", email, SqlDbType.VarChar, 200);
                    query.CreateParameter<int>("@Gender", (int)gender, SqlDbType.Int);
                    query.CreateParameter<short>("@BirthYear", (short)Birthday.Year, SqlDbType.SmallInt);
                    query.CreateParameter<short>("@Birthday", (short)(Birthday.Month * 100 + Birthday.Day), SqlDbType.SmallInt);
                    query.CreateParameter<bool>("@EmailValidated", emailValidated, SqlDbType.Bit);
                    query.CreateParameter<string>("@Signature", signature, SqlDbType.NVarChar, 2000);
                    query.CreateParameter<byte>("@SignatureFormat", (byte)signatureFormat, SqlDbType.TinyInt);
                    query.CreateParameter<bool>("@IsActive", isActive, SqlDbType.Bit);
                    //query.CreateParameter<string>("@ExtendFields", extendedFields.ToString(), SqlDbType.NText);

                    query.ExecuteNonQuery();
                }

                if (extendedFields != null && extendedFields.Count > 0)
                {
                    using (SqlQuery query = db.CreateQuery())
                    {
                        StringBuffer sql = new StringBuffer();

                        //更新用户的扩展字段UserExtendedValueCollection
                        int i = 0;
                        foreach (UserExtendedValue field in extendedFields)
                        {
                            sql += @"
IF(EXISTS(SELECT * FROM bx_UserExtendedValues WHERE [UserID] = @UserID AND [ExtendedFieldID] = @ExtendedFieldID_" + i + @"))
	UPDATE bx_UserExtendedValues SET [Value] = @Value_" + i + ",[PrivacyType] = @PrivacyType_" + i + " WHERE [UserID] = @UserID AND [ExtendedFieldID] = @ExtendedFieldID_" + i + @";
ELSE
	INSERT INTO bx_UserExtendedValues([UserID], [ExtendedFieldID], [Value], [PrivacyType]) VALUES(@UserID, @ExtendedFieldID_" + i + ", @Value_" + i + ", @PrivacyType_" + i + ");";

                            query.CreateParameter<string>("@ExtendedFieldID_" + i, field.ExtendedFieldID.ToString(), SqlDbType.VarChar, 36);
                            query.CreateParameter<string>("@Value_" + i, field.Value == null ? string.Empty : field.Value.ToString(), SqlDbType.NVarChar, 3950);
                            query.CreateParameter<int>("@PrivacyType_" + i, (int)field.PrivacyType, SqlDbType.TinyInt);

                            i++;
                        }

                        query.CommandText = sql.ToString();
                        query.CommandType = CommandType.Text;

                        query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                        query.ExecuteNonQuery();
                    }
                }

                db.CommitTransaction();
            }

            return true;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="targetUserID"></param>
        /// <param name="regDate"></param>
        /// <param name="totalOnlineTime"></param>
        /// <param name="totalMonthOnlineTime"></param>
        #region 存储过程
        [StoredProcedure(Name = "bx_AdminUpdateUserinfo", Script = @"
CREATE PROCEDURE {name}
@UserID      int,
@CreateDate  datetime,
@TotalOnlineTime  int,
@MonthOnlineTime  int
AS
BEGIN
    SET NOCOUNT ON
    UPDATE bx_Users SET CreateDate = @CreateDate , TotalOnlineTime = @TotalOnlineTime , MonthOnlineTime = @MonthOnlineTime WHERE [UserID] = @UserID;    
END
")]
        #endregion
        public override void AdminUpdateUserinfo(
           int targetUserID
         , DateTime regDate
         , int totalOnlineTime
         , int totalMonthOnlineTime
         )
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_AdminUpdateUserinfo";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", targetUserID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@CreateDate", regDate, SqlDbType.DateTime);
                query.CreateParameter<int>("@TotalOnlineTime", totalOnlineTime, SqlDbType.Int);
                query.CreateParameter<int>("@MonthOnlineTime", totalMonthOnlineTime, SqlDbType.Int);
                query.ExecuteNonQuery();
            }
        }


        #region 存储过程 用户登录后用户的一些参数记录
        [StoredProcedure(Name = "bx_UpdateLoginUserCount", Script = @"
CREATE PROCEDURE {name}
@UserID      int
,@LoginIP    varchar(50)
AS
BEGIN
    SET NOCOUNT ON
    UPDATE [bx_Users] SET LastVisitIP = @LoginIP,LoginCount = LoginCount + 1  WHERE [UserID] = @UserID;
    
END
")]
        #endregion
        public override void UpdateUserLoginCount(int userID, string LoginIP)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateLoginUserCount";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@LoginIP", LoginIP, SqlDbType.VarChar, 50);
                query.ExecuteNonQuery();
            }

        }

        public override UserCollection GetInvitees(int targetUserId, int pageSize, int pageNumber, out int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string condition = "InviterID = @UserID";
                query.CreateParameter<int>("@UserID", targetUserId, SqlDbType.Int);
                query.Pager.Condition = condition;
                query.Pager.TableName = "[bx_Members]";
                query.Pager.SortField = "[CreateDate]";
                query.Pager.IsDesc = true;
                query.Pager.PageSize = pageSize > 0 ? pageSize : 20;
                query.Pager.PageNumber = pageNumber > 0 ? pageNumber : 1;
                query.Pager.SelectCount = true;
                query.Pager.PrimaryKey = "[UserID]";

                totalCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserCollection users = new UserCollection(reader);
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            users.TotalRecords = reader.Get<int>(0);
                            totalCount = users.TotalRecords;
                        }
                    }
                    return users;
                }
            }
        }

        private void CheckUserBirthday(int birthYear, int birthMonth, ref int birthday)
        {
            if (birthMonth > 0 && birthMonth < 13)//天数检查
            {
                int temp = DateTime.DaysInMonth(birthYear <= 0 || birthYear > 9999 ? 2000 : birthYear, birthMonth);
                if (birthday > temp)
                {
                    birthday = (short)temp;
                }
            }
        }

        /// <summary>
        /// 修改用户资料
        /// </summary>
        /// <returns></returns>
        public override bool UpdateUserProfile(int userID, Gender gender, Int16 birthYear, Int16 birthMonth, Int16 birthday, string signature, SignatureFormat signatureFormat, float timeZone, UserExtendedValueCollection extendeds)
        {
            StringBuffer sql = new StringBuffer(@"
UPDATE
	[bx_Users]
SET 
    [Gender] = @Gender,[Signature] = @Signature,[SignatureFormat] = @SignatureFormat,[ExtendedFieldVersion] = @ExtendedFieldVersion,[KeywordVersion] = ''
WHERE
	[UserID] = @UserID;

UPDATE bx_UserInfos SET 
    [BirthYear] = @BirthYear,[Birthday] = @Birthday
WHERE UserID = @UserID;

UPDATE bx_UserVars SET
    [TimeZone] = @TimeZone
WHERE UserID = @UserID;
");

            using (SqlQuery query = new SqlQuery())
            {

                //更新用户的扩展字段
                int i = 0;
                foreach (UserExtendedValue field in extendeds)
                {
                    sql += @"
IF(EXISTS(SELECT * FROM bx_UserExtendedValues WHERE [UserID] = @UserID AND [ExtendedFieldID] = @ExtendedFieldID_" + i + @"))
	UPDATE bx_UserExtendedValues SET [Value] = @Value_" + i + ",[PrivacyType] = @PrivacyType_" + i + " WHERE [UserID] = @UserID AND [ExtendedFieldID] = @ExtendedFieldID_" + i + @";
ELSE
	INSERT INTO bx_UserExtendedValues([UserID], [ExtendedFieldID], [Value], [PrivacyType]) VALUES(@UserID, @ExtendedFieldID_" + i + ", @Value_" + i + ", @PrivacyType_" + i + ");";

                    query.CreateParameter<string>("@ExtendedFieldID_" + i, field.ExtendedFieldID, SqlDbType.VarChar, 36);
                    query.CreateParameter<string>("@Value_" + i, field.Value == null ? string.Empty : field.Value.ToString(), SqlDbType.NVarChar, 3950);
                    query.CreateParameter<int>("@PrivacyType_" + i, (int)field.PrivacyType, SqlDbType.TinyInt);

                    i++;
                }

                int _birthday = birthday;
                CheckUserBirthday(birthYear, birthMonth, ref _birthday);

                _birthday = birthMonth * 100 + _birthday;

                query.CommandText = sql.ToString();

                query.CreateParameter<string>("@Signature", signature, SqlDbType.NVarChar, 1500);
                query.CreateParameter<byte>("@SignatureFormat", (byte)signatureFormat, SqlDbType.TinyInt);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<Int16>("@Gender", (Int16)gender, SqlDbType.TinyInt);
                query.CreateParameter<Int16>("@BirthYear", birthYear, SqlDbType.SmallInt);
                query.CreateParameter<Int16>("@Birthday", (short)_birthday, SqlDbType.SmallInt);
                //query.CreateParameter<string>("@ExtendedFields", extendedFields.ToString(), SqlDbType.NText);
                query.CreateParameter<string>("@ExtendedFieldVersion", AllSettings.Current.ExtendedFieldSettings.Version, SqlDbType.NChar, 36);
                query.CreateParameter<float>("@TimeZone", timeZone, SqlDbType.Real);

                query.ExecuteNonQuery();
            }
            return true;
        }


        public override void UpdateUserProfile(int userID, UserExtendedValueCollection insertFields, UserExtendedValueCollection updateFields)
        {
            using (SqlQuery query = new SqlQuery())
            {
                //更新用户的扩展字段

                StringBuilder sql = new StringBuilder();

                if (updateFields.Count > 0)
                {
                    sql.Append(@"
DECLARE @UpdateTable table([Key] varchar(36) COLLATE Chinese_PRC_CI_AS_WS,PType tinyint,UValue nvarchar(3950));

");
                    foreach (UserExtendedValue field in updateFields)
                    {
                        sql.AppendFormat(@"
INSERT INTO @UpdateTable([Key],PType,UValue)Values('{0}',{1},'{2}');
", field.ExtendedFieldID, (int)field.PrivacyType, field.Value);
                    }

                    sql.Append(@"
UPDATE bx_UserExtendedValues SET [Value] = UValue,[PrivacyType] = PType FROM @UpdateTable WHERE ExtendedFieldID = [Key] AND UserID = @UserID; 
");
                }

                if (insertFields.Count > 0)
                {
                    sql.Append(@"
DECLARE @InsertTable table([Key] varchar(36) COLLATE Chinese_PRC_CI_AS_WS,PType tinyint,UValue nvarchar(3950));

");
                    foreach (UserExtendedValue field in insertFields)
                    {
                        sql.AppendFormat(@"
INSERT INTO @InsertTable([Key],PType,UValue)Values('{0}',{1},'{2}');
", field.ExtendedFieldID, (int)field.PrivacyType, field.Value);
                    }

                    sql.Append(@"
INSERT INTO bx_UserExtendedValues([Value],PrivacyType,ExtendedFieldID,UserID) SELECT UValue,PType,[Key],@UserID FROM @InsertTable; 
");
                }

                if (sql.Length > 0)
                {
                    query.CommandText = sql.ToString();
                    query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                    query.ExecuteNonQuery();
                }
            }
        }

        #region 存储过程 
        [StoredProcedure(Name = "bx_UpdateUserExtendProfilePrivacy", Script = @"
CREATE PROCEDURE {name}
@Key        varchar(36)
,@Privacy   tinyint
AS
BEGIN
    SET NOCOUNT ON
    UPDATE [bx_UserExtendedValues] SET PrivacyType = @Privacy  WHERE [ExtendedFieldID] = @Key;
    
END
")]
        #endregion
        public override void UpdateUserExtendProfilePrivacy(string key, ExtendedFieldDisplayType privacy)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateUserExtendProfilePrivacy";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@Key", key, SqlDbType.VarChar, 36);
                query.CreateParameter<int>("@Privacy", (int)privacy, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_DeleteUserExtendProfile", Script = @"
CREATE PROCEDURE {name}
@Key        varchar(36)
AS
BEGIN
    SET NOCOUNT ON
    DELETE [bx_UserExtendedValues]  WHERE [ExtendedFieldID] = @Key;
    
END
")]
        #endregion
        public override void DeleteUserExtendProfile(string key)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeleteUserExtendProfile";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@Key", key, SqlDbType.VarChar, 36);

                query.ExecuteNonQuery();
            }
        }

        public override bool UpdateUserSignature(int userID, string signature)
        {
            string sql = "UPDATE bx_Users SET Signature = @Signature WHERE UserID = @UserID";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql;
                query.CreateParameter<int>("@UserID", userID,SqlDbType.Int);
                query.CreateParameter<string>("@Signature", signature, SqlDbType.NVarChar, 1500);

               return  query.ExecuteNonQuery()==1;
            }
        }


        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="username"></param>
        #region 修改用户名存储过程
        [StoredProcedure(Name = "bx_UpdateUsername", Script = @"
CREATE PROCEDURE {name} 
@UserID     int
,@Username   nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON
    UPDATE bx_Users SET [Username] = @Username WHERE [UserID] = @UserID;
END
")]
        #endregion
        public override void AdminUpdateUsername(int userID, string username)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateUsername";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Username", username, SqlDbType.NVarChar, 50);
                query.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 取得从未通过头像认证的用户（为了再次提头像认证数据的时候不重复加积分）
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public override IList<int> GetNeverAvatarCheckedUsers(IEnumerable<int> userIds)
        {
            List<int> neverAvatarCheckedUserIds = new List<int>();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT UserID FROM bx_UserVars WHERE UserID IN (@UserIds) AND EverAvatarChecked = 0";
                query.CommandType = CommandType.Text;
                query.CreateInParameter<int>("@UserIds", userIds);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        neverAvatarCheckedUserIds.Add(reader.Get<int>(0));
                    }
                }
            }

            return neverAvatarCheckedUserIds;
        }
        [StoredProcedure(Name="bx_ActivingUser",Script= @"
CREATE PROCEDURE {name}
@Serial uniqueidentifier,
@UserID int out
AS
BEGIN
    SET NOCOUNT ON;

    SET @UserID = NULL;
    SELECT @UserID = UserID FROM bx_Serials Where Serial = @Serial AND Type = 1 AND ExpiresDate >= GETDATE();
    IF @UserID IS NULL
        RETURN(2); --无效的激活码

	DELETE FROM bx_Serials WHERE UserID = @UserID AND Type = 1;

	UPDATE bx_Users SET IsActive = 1, EmailValidated = 1 WHERE UserID = @UserID AND IsActive = 0;
	IF @@ROWCOUNT = 0
		RETURN(3);  --用户不需要激活

	RETURN(1);
END
")]

        public override int ActivingUsers(Guid serialGuid,out int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_ActivingUser";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<Guid>("@Serial", serialGuid, SqlDbType.UniqueIdentifier);
                SqlParameter returnParam = query.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);
                SqlParameter outPutParam = query.CreateParameter<int>("@UserID", SqlDbType.Int, ParameterDirection.Output);

                query.ExecuteNonQuery();


                userID = outPutParam.Value != DBNull.Value ? (int)outPutParam.Value : 0;

                return (int)returnParam.Value;
               

            }
        }

        /// <summary>
        /// 重新验证用户的邮箱（修改邮箱）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="email"></param>
        #region 存储过程
        [StoredProcedure(Name = "bx_ValidateUserEmail", Script = @"
CREATE PROCEDURE {name}
    @UserID   int,
    @Email   nvarchar(50)
AS BEGIN

    SET NOCOUNT ON;

    UPDATE bx_Users SET [Email] = @Email,[EmailValidated] = 1 WHERE [UserID] = @UserID;

END
")]

        #endregion
        public override void ValidateUserEmail(int userID, string email)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Email", email, SqlDbType.VarChar, 50);

                query.CommandText = "bx_ValidateUserEmail";
                query.CommandType = CommandType.StoredProcedure;
                query.ExecuteNonQuery();
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_User_UpdateLastVisitIP", Script = @"
CREATE PROCEDURE {name}
     @UserID     int
    ,@NewIP      varchar(50)
AS BEGIN

    SET NOCOUNT ON;

    UPDATE bx_Users SET LastVisitIP = @NewIP WHERE [UserID] = @UserID;

END")]
        #endregion
        public override bool UpdateLastVisitIP(int userID, string newIP)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_User_UpdateLastVisitIP";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@NewIP", newIP, SqlDbType.VarChar, 50);

                query.ExecuteNonQuery();
            }

            return true;
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_User_UpdateSkinID", Script = @"
CREATE PROCEDURE {name}
     @UserID     int
    ,@SkinID     nvarchar(256)
AS BEGIN

    SET NOCOUNT ON;

    UPDATE bx_UserVars SET SkinID = @SkinID WHERE [UserID] = @UserID;

END")]
        #endregion
        public override bool UpdateSkinID(int userID, string skinID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_User_UpdateSkinID";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@SkinID", skinID, SqlDbType.NVarChar, 256);

                query.ExecuteNonQuery();
            }

            return true;
        }

        /// <summary>
        /// 修改用户Email
        /// </summary>
        #region 存储过程
        [StoredProcedure(Name = "bx_UpdateEmail", Script = @"
CREATE PROCEDURE {name}
     @UserID     int
    ,@Email      nvarchar(200)
AS BEGIN

    SET NOCOUNT ON;

    IF (EXISTS (SELECT * FROM [bx_Users] WHERE Email = @Email AND [UserID] <> @UserID))
        SELECT 0;
    ELSE BEGIN
        UPDATE bx_Users SET Email = @Email WHERE [UserID] = @UserID;
        SELECT 1;
    END

END")]
        #endregion
        public override bool UpdateEmail(int userID, string email)
        {
            object result = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateEmail";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Email", email, SqlDbType.NVarChar, 200);

                result = query.ExecuteScalar();
            }

            if (result == null || result == DBNull.Value)
            {
                return false;
                //TODO: LOST new UnknownError();
            }
            if (Convert.ToInt32(result) == 0)
            {
                return false;
            }
            return true;
        }


        /*   public override void BanUser(BanUserOperation operation)
           {
               using (SqlQuery query = new SqlQuery())
               {
                   StringBuffer sqlBuffer = new StringBuffer();
                   string foruminfostring = operation.ForumInfosToString();
                   sqlBuffer += "DELETE FROM bx_BannedUsers WHERE UserID = @UserID;";
                   sqlBuffer += "INSERT INTO bx_BanUserLogs(OperationType,OperationTime,OperatorName,Cause,ForumInfos,UserID,UserName) Values(@OperationType,@OperationTime,@OperatorName,@Cause,@ForumInfos,@UserID,@UserName);DECLARE @logid int;SELECT @logid=@@IDENTITY;";
                    foreach (KeyValuePair<int, DateTime> forumBanInfo in operation.ForumInfos)
                   {
                       sqlBuffer += string.Format("INSERT INTO bx_BannedUsers( UserID, ForumID, EndDate, Cause ) VALUES(@UserID , @ForumID{0},@EndDate{0}, @Cause);", forumBanInfo.Key);
                       sqlBuffer += string.Format("INSERT INTO bx_BanUserForumInfo(ID,ForumID,EndDate) VALUES(@logid,@ForumID{0},@EndDate{0})", forumBanInfo.Key);
                       query.CreateParameter<int>(string.Format("@ForumID{0}", forumBanInfo.Key), forumBanInfo.Key, SqlDbType.Int);
                       query.CreateParameter<DateTime>(string.Format("@EndDate{0}", forumBanInfo.Key), forumBanInfo.Value, SqlDbType.DateTime);
                   }
                   query.CreateParameter<int>("@OperationType", (int)operation.OperationType,SqlDbType.Int);
                   query.CreateParameter<DateTime>("@OperationTime",operation.OperationTime,SqlDbType.DateTime);
                   query.CreateParameter<string>("@OperatorName", operation.OperatorName, SqlDbType.NVarChar, 50);
                   query.CreateParameter<string>("@Cause", operation.Cause, SqlDbType.NVarChar, 1000);
                   query.CreateParameter<string>("@ForumInfos", foruminfostring, SqlDbType.VarChar, 1000);
                   query.CreateParameter<int>("@UserID",operation.UserID,SqlDbType.Int);
                   query.CreateParameter<string>("@UserName",operation.UserName,SqlDbType.NVarChar,50);

                   query.CommandText = sqlBuffer.ToString();
                   query.ExecuteNonQuery();
               }
           }
         */





        private StringBuilder FilterToCondition<T>(UserFilterBase<T> filter, SqlQuery query)
            where T : FilterBase<T>, new()
        {
            # region 搜索条件
            StringBuilder _condition = new StringBuilder(" 1=1");

            if (filter == null)
                return _condition;


            bool isAdminSearch = (filter is AdminUserFilter);

            AdminUserFilter adminFilter = filter as AdminUserFilter;

            if (filter != null)
            {
                bool add32 = false;
                if (filter.Birthday == 0)
                    add32 = true;


                //模糊精确
                if (filter.FuzzySearch == null || filter.FuzzySearch.Value)
                {
                    if (!string.IsNullOrEmpty(filter.Username))
                    {
                        _condition.Append(" AND [Username] LIKE '%'+@Username+'%'");
                        query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                    }

                    if (!string.IsNullOrEmpty(filter.Realname))
                    {
                        _condition.Append(" AND [Realname] LIKE '%'+@Realname+'%'");
                        query.CreateParameter<string>("@Realname", filter.Realname, SqlDbType.NVarChar, 50);
                    }

                    if (isAdminSearch)
                    {

                        if (!string.IsNullOrEmpty(adminFilter.RegisterIP))
                        {
                            _condition.Append(" AND [CreateIP] LIKE '%'+@RegisterIP+'%'");
                            query.CreateParameter<string>("@RegisterIP", adminFilter.RegisterIP, SqlDbType.VarChar, 50);
                        }
                        if (!string.IsNullOrEmpty(adminFilter.LastVisitIP))
                        {
                            _condition.Append(" AND [LastVisitIP] LIKE '%'+@LastVisitIP +'%'");
                            query.CreateParameter<string>("@LastVisitIP", adminFilter.LastVisitIP, SqlDbType.VarChar, 50);
                        }
                    }

                    if (!string.IsNullOrEmpty(filter.Email))
                    {
                        _condition.Append(" AND [Email] LIKE '%'+@Email+'%'");
                        query.CreateParameter<string>("@Email", filter.Email, SqlDbType.NVarChar, 200);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter.Username))
                    {
                        _condition.Append(" AND [Username]=@Username");
                        query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                    }
                    if (!string.IsNullOrEmpty(filter.Realname))
                    {
                        _condition.Append(" AND [Realname]=@Realname");
                        query.CreateParameter<string>("@Realname", filter.Realname, SqlDbType.NVarChar, 50);
                    }

                    if (isAdminSearch)
                    {
                        if (!string.IsNullOrEmpty(adminFilter.RegisterIP))
                        {
                            _condition.Append(" AND [CreateIP]=@RegisterIP");
                            query.CreateParameter<string>("@RegisterIP", adminFilter.RegisterIP, SqlDbType.VarChar, 50);
                        }
                        if (!string.IsNullOrEmpty(adminFilter.LastVisitIP))
                        {
                            _condition.Append(" AND [LastVisitIP]=@LastVisitIP");
                            query.CreateParameter<string>("@LastVisitIP", adminFilter.LastVisitIP, SqlDbType.VarChar, 50);
                        }
                    }

                    if (!string.IsNullOrEmpty(filter.Email))
                    {
                        _condition.Append(" AND [Email] =@Email");
                        query.CreateParameter<string>("@Email", filter.Email, SqlDbType.NVarChar, 200);
                    }
                }

                if (isAdminSearch)
                {

                    if (adminFilter.LastVisitDate_1 != null)
                    {
                        _condition.Append(" AND [LastVisitDate]>=@LastVisitDate");
                        query.CreateParameter<DateTime>("@LastVisitDate", adminFilter.LastVisitDate_1.Value, SqlDbType.DateTime);
                    }
                    if (adminFilter.LastVisitDate_2 != null)
                    {
                        _condition.Append(" AND [LastVisitDate]<=@LastVisitDate2");
                        query.CreateParameter<DateTime>("@LastVisitDate2", adminFilter.LastVisitDate_2.Value, SqlDbType.DateTime);
                    }
                    if (adminFilter.RegisterDate_1 != null)
                    {
                        _condition.Append(" AND [CreateDate]>=@CreateDate");
                        query.CreateParameter<DateTime>("@CreateDate", adminFilter.RegisterDate_1.Value, SqlDbType.DateTime);
                    }
                    if (adminFilter.RegisterDate_2 != null)
                    {
                        _condition.Append(" AND [CreateDate]<=@CreateDate2");
                        query.CreateParameter<DateTime>("@CreateDate2", adminFilter.RegisterDate_2.Value, SqlDbType.DateTime);
                    }

                    if (adminFilter.EmailValidated == true)
                    {
                        _condition.Append(" AND [EmailValidated]=@EmailValidated");
                        query.CreateParameter<bool>("@EmailValidated", adminFilter.EmailValidated.Value, SqlDbType.Bit);
                    }
                    else if (adminFilter.EmailValidated == false)
                    {
                        _condition.Append(" AND [EmailValidated]<>@EmailValidated");
                        query.CreateParameter<bool>("@EmailValidated", true, SqlDbType.Bit);
                    }


                    if (adminFilter.IsActive == true)
                    {
                        _condition.Append(" AND [IsActive]=@IsActive");
                        query.CreateParameter<bool>("@IsActive", true, SqlDbType.Bit);
                    }
                    else if (adminFilter.IsActive == false)
                    {
                        _condition.Append(" AND [IsActive]<>@IsActive");
                        query.CreateParameter<bool>("@IsActive", true, SqlDbType.Bit);
                    }

                    if (filter.Gender == Gender.Male || filter.Gender == Gender.Female || filter.Gender == Gender.NotSet)
                    {
                        _condition.Append(" AND [Gender]=@Gender");
                        query.CreateParameter<byte>("@Gender", (byte)filter.Gender, SqlDbType.TinyInt);
                    }

                    if (adminFilter.Role != null)
                    {
                        _condition.Append(" AND UserID IN( SELECT UserID FROM bx_UserRoles WHERE RoleID = @RoleID AND BeginDate <= GETDATE() AND EndDate >= GETDATE()) ");
                        query.CreateParameter<Guid>("@RoleID", adminFilter.Role.Value, SqlDbType.UniqueIdentifier);
                    }

                }

                if (filter.BirthYear > 0)
                {
                    _condition.Append(" AND [BirthYear]=@BirthYear");
                    query.CreateParameter<short>("@BirthYear", filter.BirthYear.Value, SqlDbType.SmallInt);
                }
                if (filter.Birthday != null && filter.Birthday > 0)
                {
                    int day = (short)(filter.BirthMonth * 100 + filter.Birthday);

                    if (add32)
                        _condition.Append(" AND [Birthday]>=@Birthday AND [Birthday]<(@Birthday+32)");
                    else
                        _condition.Append(" AND [Birthday]=@Birthday");
                    query.CreateParameter<short>("@Birthday", (short)day, SqlDbType.SmallInt);
                }


                if (filter.UserID != null && filter.UserID.Value != 0)
                {
                    _condition.Append(" AND [UserID]=@UserID");
                    query.CreateParameter<int>("@UserID", filter.UserID.Value, SqlDbType.Int);
                }
                if (filter.BeginAge != null && filter.BeginAge >= 0)
                {
                    _condition.Append(" AND [BirthYear]<=@Age1");
                    query.CreateParameter<short>("@Age1", (short)(DateTimeUtil.Now.Year - filter.BeginAge), SqlDbType.SmallInt);
                }

                if (filter.EndAge != null && filter.EndAge >= 0)
                {
                    _condition.Append(" AND [BirthYear]>=@Age2");
                    query.CreateParameter<short>("@Age2", (short)(DateTimeUtil.Now.Year - filter.EndAge), SqlDbType.SmallInt);
                }




                //前台搜索和后台搜索区别对待的两个地方
                if (!isAdminSearch)
                {
                    _condition.Append(" AND [IsActive]=@IsActive");
                    query.CreateParameter<bool>("@IsActive", true, SqlDbType.Bit);

                    if (filter.Gender == Gender.Male || filter.Gender == Gender.Female)
                    {
                        _condition.Append(" AND [Gender]=@Gender");
                        query.CreateParameter<byte>("@Gender", (byte)filter.Gender, SqlDbType.TinyInt);
                    }
                }
            }
            #endregion

            if (filter.ExtendedFields != null && filter.ExtendedFields.Items != null)
            {
                int i = 0;

                foreach (ExtendedFieldSearchInfoItem item in filter.ExtendedFields.Items)
                {
                    _condition.Append(" AND UserID IN (SELECT [UserID] FROM [bx_UserExtendedValues] WHERE [ExtendedFieldID] = @ExtendFieldID_").Append(i).Append(" AND PrivacyType = 0 ").Append(" AND ");

                    if (item.NeedExactMatch)
                    {
                        _condition.Append("[Value] = @SearchValue_").Append(i).Append(") ");
                    }
                    else
                    {
                        _condition.Append("[Value] LIKE '%' + @SearchValue_").Append(i).Append(" + '%') ");
                    }

                    query.CreateParameter<string>("@ExtendFieldID_" + i, item.FieldKey, SqlDbType.VarChar, 36);
                    query.CreateParameter<string>("@SearchValue_" + i, item.SearchValue, SqlDbType.NVarChar, 3950);

                    i++;
                }
            }

            return _condition;
        }

        public override void UpdateUserDoing(int userID, string doing)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_Users SET Doing = @Content, DoingDate = GETDATE() WHERE UserID = @UserID;";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Content", doing, SqlDbType.NVarChar, 200);

                query.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public override SimpleUserCollection GetSimpleUsers(IEnumerable<int> userIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_SimpleUser WITH(NOLOCK) WHERE UserID IN( @UserIds )";
                query.CommandType = CommandType.Text;
                query.CreateInParameter<int>("@UserIds", userIds);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new SimpleUserCollection(reader);
                }
            }

            //return new SimpleUserCollection();
        }

        /// <summary>
        /// 获取简单用户对象
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_GetSimpleUser", Script = @"
CREATE PROCEDURE {name}
    @UserID int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM bx_SimpleUser WITH(NOLOCK) WHERE UserID = @UserID;
END
")]
        public override SimpleUser GetSimpleUser(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetSimpleUser";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new SimpleUser(reader);
                }
            }
            return null;
        }


        private string GetOrderFieldName(UserOrderBy? orderField)
        {
            #region 排序字段

            string OrderFieldString = "[UserID]";
            if (orderField != null)
            {
                switch (orderField.Value)
                {
                    case UserOrderBy.Username:
                        OrderFieldString = "[Username]";
                        break;
                    case UserOrderBy.Gender:
                        OrderFieldString = "[Gender]";
                        break;
                    case UserOrderBy.BirthDateTime:
                        OrderFieldString = "[BirthYear],[Birthday]";
                        break;
                    case UserOrderBy.LoginCount:
                        OrderFieldString = "[LoginCount]";
                        break;
                    case UserOrderBy.TotalFriends:
                        OrderFieldString = "[TotalFriends]";
                        break;
                    case UserOrderBy.TotalOnlineTime:
                        OrderFieldString = "[TotalOnlineTime]";
                        break;
                    case UserOrderBy.Points:
                        OrderFieldString = "[Points]";
                        break;
                    case UserOrderBy.Point1:
                        OrderFieldString = "[Point_1]";
                        break;
                    case UserOrderBy.Point2:
                        OrderFieldString = "[Point_2]";
                        break;
                    case UserOrderBy.Point3:
                        OrderFieldString = "[Point_3]";
                        break;
                    case UserOrderBy.Point4:
                        OrderFieldString = "[Point_4]";
                        break;
                    case UserOrderBy.Point5:
                        OrderFieldString = "[Point_5]";
                        break;
                    case UserOrderBy.Point6:
                        OrderFieldString = "[Point_6]";
                        break;
                    case UserOrderBy.Point7:
                        OrderFieldString = "[Point_7]";
                        break;
                    case UserOrderBy.Point8:
                        OrderFieldString = "[Point_8]";
                        break;
                    case UserOrderBy.CreateDate:
                        OrderFieldString = "[CreateDate]";
                        break;
                    case UserOrderBy.LastVisitDate:
                        OrderFieldString = "[LastVisitDate]";
                        break;
                    case UserOrderBy.TotalInvite:
                        OrderFieldString = "[TotalInvite]";
                        break;
                    case UserOrderBy.TotalPosts:
                        OrderFieldString = "[TotalPosts]";
                        break;
                    case UserOrderBy.TotalViews:
                        OrderFieldString = "[SpaceViews]";
                        break;
                    default:
                        OrderFieldString = "[UserID]";
                        break;
                }
            }
            #endregion
            return OrderFieldString;
        }


        #region 积分操作
        /*

        #region 存储过程
        [StoredProcedure(Name = "bx_UpdatePoints", Script = @"
CREATE PROCEDURE {name} 
      @UserID    int  
    , @Point1    int
    , @Point2    int
    , @Point3    int
    , @Point4    int
    , @Point5    int
    , @Point6    int
    , @Point7    int
    , @Point8    int
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [bx_Users] SET [Point_1] = @Point1, [Point_2] = @Point2, [Point_3] = @Point3
    , [Point_4] = @Point4, [Point_5] = @Point5, [Point_6] = @Point6, [Point_7] = @Point7, [Point_8] = @Point8 WHERE [UserID]=@UserID;
    
    SELECT [Points] FROM [bx_Users] WHERE [UserID] = @UserID;
END
")]
        #endregion
        public override int UpdateUserPoints(int UserID, int point1, int point2, int point3, int point4, int point5, int point6, int point7, int point8)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdatePoints";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", UserID, SqlDbType.Int);
                query.CreateParameter<int>("@Point1", point1, SqlDbType.Int);
                query.CreateParameter<int>("@Point2", point2, SqlDbType.Int);
                query.CreateParameter<int>("@Point3", point3, SqlDbType.Int);
                query.CreateParameter<int>("@Point4", point4, SqlDbType.Int);
                query.CreateParameter<int>("@Point5", point5, SqlDbType.Int);
                query.CreateParameter<int>("@Point6", point6, SqlDbType.Int);
                query.CreateParameter<int>("@Point7", point7, SqlDbType.Int);
                query.CreateParameter<int>("@Point8", point8, SqlDbType.Int);


                return query.ExecuteScalar<int>();
            }
        }

        */
        #region 存储过程
        [StoredProcedure(Name = "bx_UpdateAllUserPoints", Script = @"
CREATE PROCEDURE {name} 
      @MaxPoint1    int
    , @MaxPoint2    int
    , @MaxPoint3    int
    , @MaxPoint4    int
    , @MaxPoint5    int
    , @MaxPoint6    int
    , @MaxPoint7    int
    , @MaxPoint8    int
    , @MinPoint1    int
    , @MinPoint2    int
    , @MinPoint3    int
    , @MinPoint4    int
    , @MinPoint5    int
    , @MinPoint6    int
    , @MinPoint7    int
    , @MinPoint8    int
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [bx_Users] SET 
        [Point_1]=CASE WHEN [Point_1] > @MaxPoint1 THEN @MaxPoint1 ELSE 
            CASE WHEN [Point_1] < @MinPoint1 THEN @MinPoint1 ELSE [Point_1] END 
        END
        ,[Point_2]=CASE WHEN [Point_2] > @MaxPoint2 THEN @MaxPoint2 ELSE 
            CASE WHEN [Point_2] < @MinPoint2 THEN @MinPoint2 ELSE [Point_2] END 
        END
        ,[Point_3]=CASE WHEN [Point_3] > @MaxPoint3 THEN @MaxPoint3 ELSE 
            CASE WHEN [Point_3] < @MinPoint3 THEN @MinPoint3 ELSE [Point_3] END 
        END
        ,[Point_4]=CASE WHEN [Point_4] > @MaxPoint4 THEN @MaxPoint4 ELSE 
            CASE WHEN [Point_4] < @MinPoint4 THEN @MinPoint4 ELSE [Point_4] END 
        END
        ,[Point_5]=CASE WHEN [Point_5] > @MaxPoint5 THEN @MaxPoint5 ELSE 
            CASE WHEN [Point_5] < @MinPoint5 THEN @MinPoint5 ELSE [Point_5] END 
        END
        ,[Point_6]=CASE WHEN [Point_6] > @MaxPoint6 THEN @MaxPoint6 ELSE 
            CASE WHEN [Point_6] < @MinPoint6 THEN @MinPoint6 ELSE [Point_6] END 
        END
        ,[Point_7]=CASE WHEN [Point_7] > @MaxPoint7 THEN @MaxPoint7 ELSE 
            CASE WHEN [Point_7] < @MinPoint7 THEN @MinPoint7 ELSE [Point_7] END 
        END
        ,[Point_8]=CASE WHEN [Point_8] > @MaxPoint8 THEN @MaxPoint8 ELSE 
            CASE WHEN [Point_8] < @MinPoint8 THEN @MinPoint8 ELSE [Point_8] END 
        END;
END
")]
        #endregion
        public override void UpdateAllUserPoints(int[] maxPoints, int[] minPoints)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateAllUserPoints";

                for (int i = 0; i < 8; i++)
                {
                    query.CreateParameter<int>("MaxPoint" + (i + 1), maxPoints[i], SqlDbType.Int);
                    query.CreateParameter<int>("MinPoint" + (i + 1), minPoints[i], SqlDbType.Int);
                }

                query.CommandType = CommandType.StoredProcedure;
                query.ExecuteNonQuery();
            }
        }


        public override PointExpressionColumCollection GetGeneralPointExpressionColums()
        {
            return GetGeneralPointExpressionColums(AllSettings.Current.PointSettings);
        }
        /// <summary>
        /// 积分公式可用的字段
        /// </summary>
        /// <returns></returns>
        public override PointExpressionColumCollection GetGeneralPointExpressionColums(PointSettings pointSettings)
        {
            PointExpressionColumCollection colums = new PointExpressionColumCollection();

            //注意：以下如果修改   也需要相应的修改升级程序的积分公式处理
            //KEY 与表字段名 一样 区分大小写
            foreach (UserPoint point in pointSettings.EnabledUserPoints)
            {
                int index = (int)point.Type + 1;
                colums.Add("Point_" + index, "p" + index, point.Name);
            }
            colums.Add("TotalOnlineTime", "online", Lang.User_UserPointOnlineColumName);
            colums.Add("TotalTopics", "topics", Lang.User_UserPointTotalTopicsColumName);
            colums.Add("TotalPosts", "posts", Lang.User_UserPointTotalRepliesColumName);
            colums.Add("DeletedTopics", "deletedTopics", Lang.User_UserPointDeletedTopicsColumName);
            colums.Add("DeletedReplies", "deletedReplies", Lang.User_UserPointDeletedRepliesColumName);
            colums.Add("ValuedTopics", "valuedTopics", Lang.User_UserPointValuedTopicsColumName);


            return colums;
        }

        private string GetUpdatePointCondition(IEnumerable<string> colums)
        {
            //注意：以下如果修改   也需要相应的修改升级程序的积分公式处理
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

        private string GetExpression(string expression, out List<string> columNames)
        {
            PointExpressionColumCollection colums = GetGeneralPointExpressionColums();
            columNames = new List<string>();

            //注意：以下如果修改   也需要相应的修改升级程序的积分公式处理
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

            return expression;
        }

        public override void UpdatePointsExpression(string expression)
        {
            List<string> columNames;
            expression = GetExpression(expression, out columNames);

            using (SqlQuery query = new SqlQuery())
            {
                query.Session.BeginTransaction();

                try
                {

                    //--测试公式
                    query.CommandText = @"
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;
SELECT TOP 1 " + expression + @" FROM [bx_Users];";

                    query.ExecuteNonQuery();

                    query.CommandText = @"
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
                    query.CommandType = CommandType.Text;
                    query.ExecuteNonQuery();


                    query.CommandText = @"
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
                    query.CommandType = CommandType.Text;
                    query.ExecuteNonQuery();


                    //                    //更新所有用户的总积分
                    //                    query.CommandText = @"
                    //
                    //    SET NOCOUNT ON;
                    //	DECLARE @MaxValue int;
                    //	SET @MaxValue = 2147483647;
                    //
                    //    SET ARITHABORT OFF;
                    //    SET ANSI_WARNINGS OFF;
                    //	UPDATE bx_Users SET Points = ISNULL(" + expression + @",@MaxValue);
                    //";
                    //                    query.CommandTimeout = int.MaxValue;
                    //                    query.CommandType = CommandType.Text;
                    //                    query.ExecuteNonQuery();

                    query.Session.CommitTransaction();
                }
                catch (Exception ex)
                {
                    query.Session.RollbackTransaction();
                    throw ex;
                }
            }
        }


        public override void ReCountUsersPoints(string expression, int startUserID, int updateCount, out int endUserID, ref string resultExpression)
        {
            if (resultExpression == null)
            {
                List<string> columNames;
                resultExpression = GetExpression(expression, out columNames);
            }
            using (SqlQuery query = new SqlQuery())
            {
                //更新所有用户的总积分
                query.CommandText = @"

    SET NOCOUNT ON;
	DECLARE @MaxValue int;
	SET @MaxValue = 2147483647;

    SET ARITHABORT OFF;
    SET ANSI_WARNINGS OFF;

    DECLARE @EndUserID int;
    SELECT @EndUserID = MAX(UserID) FROM (SELECT TOP " + updateCount + @" UserID FROM bx_Users WHERE UserID>=@StartUserID ORDER BY UserID ASC) AS Temp
    SELECT @EndUserID;
    IF @EndUserID IS NULL
        RETURN;
    UPDATE bx_Users SET Points = ISNULL(" + resultExpression + @",@MaxValue) WHERE UserID >= @StartUserID AND UserID<=@EndUserID;
";
                query.CommandTimeout = int.MaxValue;
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@StartUserID", startUserID, SqlDbType.Int);

                int? userID = query.ExecuteScalar<int?>();

                if (userID == null)
                    endUserID = 0;
                else
                    endUserID = userID.Value;
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_CheckUserPoint", Script = @"
CREATE PROCEDURE {name} 
      @UserID    int  
    , @ThrowOverMinValueError   bit
    , @ThrowOverMaxValueError   bit

    , @Point1    int
    , @Point2    int
    , @Point3    int
    , @Point4    int
    , @Point5    int
    , @Point6    int
    , @Point7    int
    , @Point8    int

    , @MinPoint1    int
    , @MinPoint2    int
    , @MinPoint3    int
    , @MinPoint4    int
    , @MinPoint5    int
    , @MinPoint6    int
    , @MinPoint7    int
    , @MinPoint8    int

    , @MaxPoint1    int
    , @MaxPoint2    int
    , @MaxPoint3    int
    , @MaxPoint4    int
    , @MaxPoint5    int
    , @MaxPoint6    int
    , @MaxPoint7    int
    , @MaxPoint8    int
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserPoint1 int,@UserPoint2 int,@UserPoint3 int,@UserPoint4 int,@UserPoint5 int,@UserPoint6 int,@UserPoint7 int,@UserPoint8 int;
    SELECT @UserPoint1=Point_1,@UserPoint2=Point_2,@UserPoint3=Point_3,@UserPoint4=Point_4,@UserPoint5=Point_5,@UserPoint6=Point_6,@UserPoint7=Point_7,@UserPoint8=Point_8
            FROM [bx_Users] WHERE [UserID]=@UserID;

    IF @ThrowOverMinValueError = 1 BEGIN
        IF @Point1<0 AND (@UserPoint1+@Point1)<@MinPoint1 BEGIN
            SELECT -1 AS Code,@UserPoint1 AS Point;
            RETURN;
        END
        IF @Point2<0 AND (@UserPoint2+@Point2)<@MinPoint2 BEGIN
            SELECT -2 AS Code,@UserPoint2 AS Point;
            RETURN;
        END
        IF @Point3<0 AND (@UserPoint3+@Point3)<@MinPoint3 BEGIN
            SELECT -3 AS Code,@UserPoint3 AS Point;
            RETURN;
        END
        IF @Point4<0 AND (@UserPoint4+@Point4)<@MinPoint4 BEGIN
            SELECT -4 AS Code,@UserPoint4 AS Point;
            RETURN;
        END
        IF @Point5<0 AND (@UserPoint5+@Point5)<@MinPoint5 BEGIN
            SELECT -5 AS Code,@UserPoint5 AS Point;
            RETURN;
        END
        IF @Point6<0 AND (@UserPoint6+@Point6)<@MinPoint6 BEGIN
            SELECT -6 AS Code,@UserPoint6 AS Point;
            RETURN;
        END
        IF @Point7<0 AND (@UserPoint7+@Point7)<@MinPoint7 BEGIN
            SELECT -7 AS Code,@UserPoint7 AS Point;
            RETURN;
        END
        IF @Point8<0 AND (@UserPoint8+@Point8)<@MinPoint8 BEGIN
            SELECT -8 AS Code,@UserPoint8 AS Point;
            RETURN;
        END
    END
    IF @ThrowOverMaxValueError = 1 BEGIN
        IF @Point1>0 AND (@UserPoint1+@Point1)>@MaxPoint1 BEGIN
            SELECT 1 AS Code,@UserPoint1 AS Point;
            RETURN;
        END
        IF @Point2>0 AND (@UserPoint2+@Point2)>@MaxPoint2 BEGIN
            SELECT 2 AS Code,@UserPoint2 AS Point;
            RETURN;
        END
        IF @Point3>0 AND (@UserPoint3+@Point3)>@MaxPoint3 BEGIN
            SELECT 3 AS Code,@UserPoint3 AS Point;
            RETURN;
        END
        IF @Point4>0 AND (@UserPoint4+@Point4)>@MaxPoint4 BEGIN
            SELECT 4 AS Code,@UserPoint4 AS Point;
            RETURN;
        END
        IF @Point5>0 AND (@UserPoint5+@Point5)>@MaxPoint5 BEGIN
            SELECT 5 AS Code,@UserPoint5 AS Point;
            RETURN;
        END
        IF @Point6>0 AND (@UserPoint6+@Point6)>@MaxPoint6 BEGIN
            SELECT 6 AS Code,@UserPoint6 AS Point;
            RETURN;
        END
        IF @Point7>0 AND (@UserPoint7+@Point7)>@MaxPoint7 BEGIN
            SELECT 7 AS Code,@UserPoint7 AS Point;
            RETURN;
        END
        IF @Point8>0 AND (@UserPoint8+@Point8)>@MaxPoint8 BEGIN
            SELECT 8 AS Code,@UserPoint8 AS Point;
            RETURN;
        END
    END
    
END
")]
        #endregion
        public override int CheckUserPoint(int userID, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points, int[] minValues, int[] maxValues, out int point)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CheckUserPoint";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateParameter<bool>("@ThrowOverMinValueError", throwOverMinValueError, SqlDbType.Bit);
                query.CreateParameter<bool>("@ThrowOverMaxValueError", throwOverMaxValueError, SqlDbType.Bit);

                query.CreateParameter<int>("@Point1", points[0], SqlDbType.Int);
                query.CreateParameter<int>("@Point2", points[1], SqlDbType.Int);
                query.CreateParameter<int>("@Point3", points[2], SqlDbType.Int);
                query.CreateParameter<int>("@Point4", points[3], SqlDbType.Int);
                query.CreateParameter<int>("@Point5", points[4], SqlDbType.Int);
                query.CreateParameter<int>("@Point6", points[5], SqlDbType.Int);
                query.CreateParameter<int>("@Point7", points[6], SqlDbType.Int);
                query.CreateParameter<int>("@Point8", points[7], SqlDbType.Int);

                query.CreateParameter<int>("@MinPoint1", minValues[0], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint2", minValues[1], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint3", minValues[2], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint4", minValues[3], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint5", minValues[4], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint6", minValues[5], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint7", minValues[6], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint8", minValues[7], SqlDbType.Int);

                query.CreateParameter<int>("@MaxPoint1", maxValues[0], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint2", maxValues[1], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint3", maxValues[2], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint4", maxValues[3], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint5", maxValues[4], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint6", maxValues[5], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint7", maxValues[6], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint8", maxValues[7], SqlDbType.Int);


                int code = 0;
                point = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        code = reader.Get<int>("Code");
                        point = reader.Get<int>("Point");
                    }
                }

                return code;
            }
        }



        #region 存储过程
        [StoredProcedure(Name = "bx_UpdateUserPoint", Script = @"
CREATE PROCEDURE {name} 
      @UserID    int  
    , @ThrowOverMinValueError   bit
    , @ThrowOverMaxValueError   bit

    , @Point1    int
    , @Point2    int
    , @Point3    int
    , @Point4    int
    , @Point5    int
    , @Point6    int
    , @Point7    int
    , @Point8    int

    , @MinPoint1    int
    , @MinPoint2    int
    , @MinPoint3    int
    , @MinPoint4    int
    , @MinPoint5    int
    , @MinPoint6    int
    , @MinPoint7    int
    , @MinPoint8    int

    , @MaxPoint1    int
    , @MaxPoint2    int
    , @MaxPoint3    int
    , @MaxPoint4    int
    , @MaxPoint5    int
    , @MaxPoint6    int
    , @MaxPoint7    int
    , @MaxPoint8    int
    , @Operate      nvarchar(50)
    , @Remarks      nvarchar(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserPoint1 int,@UserPoint2 int,@UserPoint3 int,@UserPoint4 int,@UserPoint5 int,@UserPoint6 int,@UserPoint7 int,@UserPoint8 int;
    SELECT @UserPoint1=Point_1,@UserPoint2=Point_2,@UserPoint3=Point_3,@UserPoint4=Point_4,@UserPoint5=Point_5,@UserPoint6=Point_6,@UserPoint7=Point_7,@UserPoint8=Point_8
            FROM [bx_Users] WHERE [UserID]=@UserID;

    IF @Point1<0 AND (@UserPoint1+@Point1)<@MinPoint1 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -1 AS Code,@UserPoint1 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint1=@MinPoint1;
    END
    ELSE IF @Point1<0
        SET @UserPoint1=@UserPoint1+@Point1;

    IF @Point2<0 AND (@UserPoint2+@Point2)<@MinPoint2 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -2 AS Code,@UserPoint2 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint2=@MinPoint2;
    END
    ELSE IF @Point2<0
        SET @UserPoint2=@UserPoint2+@Point2;

    IF @Point3<0 AND (@UserPoint3+@Point3)<@MinPoint3 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -3 AS Code,@UserPoint3 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint3=@MinPoint3;
    END
    ELSE IF @Point3<0
        SET @UserPoint3=@UserPoint3+@Point3;

    IF @Point4<0 AND (@UserPoint4+@Point4)<@MinPoint4 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -4 AS Code,@UserPoint4 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint4=@MinPoint4;
    END
    ELSE IF @Point4<0
        SET @UserPoint4=@UserPoint4+@Point4;

    IF @Point5<0 AND (@UserPoint5+@Point5)<@MinPoint5 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -5 AS Code,@UserPoint5 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint5=@MinPoint5;
    END
    ELSE IF @Point5<0
        SET @UserPoint5=@UserPoint5+@Point5;

    IF @Point6<0 AND (@UserPoint6+@Point6)<@MinPoint6 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -6 AS Code,@UserPoint6 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint6=@MinPoint6;
    END
    ELSE IF @Point6<0
        SET @UserPoint6=@UserPoint6+@Point6;

    IF @Point7<0 AND (@UserPoint7+@Point7)<@MinPoint7 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -7 AS Code,@UserPoint7 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint7=@MinPoint7;
    END
    ELSE IF @Point7<0
        SET @UserPoint7=@UserPoint7+@Point7;

    IF @Point8<0 AND (@UserPoint8+@Point8)<@MinPoint8 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -8 AS Code,@UserPoint8 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint8=@MinPoint8;
    END
    ELSE IF @Point8<0
        SET @UserPoint8=@UserPoint8+@Point8;


    IF @Point1>0 AND (@UserPoint1+@Point1)>@MaxPoint1 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 1 AS Code,@UserPoint1 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint1=@MaxPoint1;
    END
    ELSE IF @Point1>0
        SET @UserPoint1=@UserPoint1+@Point1;

    IF @Point2>0 AND (@UserPoint2+@Point2)>@MaxPoint2 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 2 AS Code,@UserPoint2 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint2=@MaxPoint2;
    END
    ELSE IF @Point2>0
        SET @UserPoint2=@UserPoint2+@Point2;

    IF @Point3>0 AND (@UserPoint3+@Point3)>@MaxPoint3 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 3 AS Code,@UserPoint3 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint3=@MaxPoint3;
    END
    ELSE IF @Point3>0
        SET @UserPoint3=@UserPoint3+@Point3;

    IF @Point4>0 AND (@UserPoint4+@Point4)>@MaxPoint4 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 4 AS Code,@UserPoint4 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint4=@MaxPoint4;
    END
    ELSE IF @Point4>0
        SET @UserPoint4=@UserPoint4+@Point4;

    IF @Point5>0 AND (@UserPoint5+@Point5)>@MaxPoint5 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 5 AS Code,@UserPoint5 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint5=@MaxPoint5;
    END
    ELSE IF @Point5>0
        SET @UserPoint5=@UserPoint5+@Point5;

    IF @Point6>0 AND (@UserPoint6+@Point6)>@MaxPoint6 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 6 AS Code,@UserPoint6 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint6=@MaxPoint6;
    END
    ELSE IF @Point6>0
        SET @UserPoint6=@UserPoint6+@Point6;

    IF @Point7>0 AND (@UserPoint7+@Point7)>@MaxPoint7 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 7 AS Code,@UserPoint7 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint7=@MaxPoint7;
    END
    ELSE IF @Point7>0
        SET @UserPoint7=@UserPoint7+@Point7;

    IF @Point8>0 AND (@UserPoint8+@Point8)>@MaxPoint8 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 8 AS Code,@UserPoint8 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint8=@MaxPoint8;
    END
    ELSE IF @Point8>0
        SET @UserPoint8=@UserPoint8+@Point8;

    UPDATE [bx_Users] SET [Point_1] = @UserPoint1, [Point_2] = @UserPoint2, [Point_3] = @UserPoint3
    , [Point_4] = @UserPoint4, [Point_5] = @UserPoint5, [Point_6] = @UserPoint6, [Point_7] = @UserPoint7, [Point_8] = @UserPoint8 WHERE [UserID]=@UserID;
 

    EXECUTE bx_UpdateUserGeneralPoint @UserID;    

    SELECT [Points],[Point_1],[Point_2],[Point_3],[Point_4],[Point_5],[Point_6],[Point_7],[Point_8] FROM [bx_Users] WHERE [UserID] = @UserID;

    EXEC bx_CreatePointLogs @UserID, @Point1, @Point2 , @Point3, @Point4, @Point5 , @Point6  , @Point7 , @Point8
, @UserPoint1 ,@UserPoint2 ,@UserPoint3 ,@UserPoint4 ,@UserPoint5 ,@UserPoint6 ,@UserPoint7 ,@UserPoint8
, @Operate, @Remarks
END
")]
        #endregion
        public override int UpdateUserPoint(int userID, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points, int[] minValues, int[] maxValues, out int[] resultPoints, out int generalPoint, out int userPoint, string operateName, string remarks)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateUserPoint";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateParameter<bool>("@ThrowOverMinValueError", throwOverMinValueError, SqlDbType.Bit);
                query.CreateParameter<bool>("@ThrowOverMaxValueError", throwOverMaxValueError, SqlDbType.Bit);

                query.CreateParameter<int>("@Point1", points[0], SqlDbType.Int);
                query.CreateParameter<int>("@Point2", points[1], SqlDbType.Int);
                query.CreateParameter<int>("@Point3", points[2], SqlDbType.Int);
                query.CreateParameter<int>("@Point4", points[3], SqlDbType.Int);
                query.CreateParameter<int>("@Point5", points[4], SqlDbType.Int);
                query.CreateParameter<int>("@Point6", points[5], SqlDbType.Int);
                query.CreateParameter<int>("@Point7", points[6], SqlDbType.Int);
                query.CreateParameter<int>("@Point8", points[7], SqlDbType.Int);

                query.CreateParameter<int>("@MinPoint1", minValues[0], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint2", minValues[1], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint3", minValues[2], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint4", minValues[3], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint5", minValues[4], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint6", minValues[5], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint7", minValues[6], SqlDbType.Int);
                query.CreateParameter<int>("@MinPoint8", minValues[7], SqlDbType.Int);

                query.CreateParameter<int>("@MaxPoint1", maxValues[0], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint2", maxValues[1], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint3", maxValues[2], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint4", maxValues[3], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint5", maxValues[4], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint6", maxValues[5], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint7", maxValues[6], SqlDbType.Int);
                query.CreateParameter<int>("@MaxPoint8", maxValues[7], SqlDbType.Int);
                query.CreateParameter<string>("@Operate", operateName  , SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Remarks",  remarks , SqlDbType.NVarChar, 200);

                generalPoint = 0;
                resultPoints = new int[8];
                userPoint = 0;

                int code = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.ContainsField("Points"))
                        {
                            generalPoint = reader.Get<int>("Points");
                            resultPoints[0] = reader.Get<int>("Point_1");
                            resultPoints[1] = reader.Get<int>("Point_2");
                            resultPoints[2] = reader.Get<int>("Point_3");
                            resultPoints[3] = reader.Get<int>("Point_4");
                            resultPoints[4] = reader.Get<int>("Point_5");
                            resultPoints[5] = reader.Get<int>("Point_6");
                            resultPoints[6] = reader.Get<int>("Point_7");
                            resultPoints[7] = reader.Get<int>("Point_8");
                        }
                        else
                        {
                            code = reader.Get<int>("Code");
                            userPoint = reader.Get<int>("UserPoint");
                        }
                    }
                }

                return code;
            }
        }


        [StoredProcedure(Name = "bx_SetUserPoint", Script = @"
CREATE PROCEDURE {name}
      @UserID       int

    , @Point1       int
    , @Point2       int
    , @Point3       int
    , @Point4       int
    , @Point5       int
    , @Point6       int
    , @Point7       int
    , @Point8       int
    , @Operate      nvarchar(50)
    , @Remarks      nvarchar(200)
AS 
BEGIN
    SET NOCOUNT ON;

    DECLARE @Op0 int,@Op1 int,@Op2 int,@Op3 int,@Op4 int,@Op5 int,@Op6 int,@Op7 int;

    SELECT @Op0 = [Point_1],@Op1 = [Point_2],@Op2 = [Point_3],@Op3 = [Point_4],@Op4 = [Point_5],@Op5 = [Point_6],@Op6 = [Point_7],@Op7 = [Point_8] FROM bx_Users WHERE UserID = @UserID;

     UPDATE [bx_Users] SET [Point_1] = @Point1, [Point_2] = @Point2, [Point_3] = @Point3
    , [Point_4] = @Point4, [Point_5] = @Point5, [Point_6] = @Point6, [Point_7] = @Point7, [Point_8] = @Point8 WHERE [UserID]=@UserID; 

   
    EXECUTE bx_UpdateUserGeneralPoint @UserID;

    SELECT Points FROM [bx_Users] WHERE UserID=@UserID; 

    SET @Op0 = @Point1 - @Op0;
    SET @Op1 = @Point2 - @Op1;
    SET @Op2 = @Point3 - @Op2;
    SET @Op3 = @Point4 - @Op3;
    SET @Op4 = @Point5 - @Op4;
    SET @Op5 = @Point6 - @Op5;
    SET @Op6 = @Point7 - @Op6;
    SET @Op7 = @Point8 - @Op7;
 
    EXEC bx_CreatePointLogs @UserID
    ,@Op0, @Op1, @Op2, @Op3, @Op4, @Op5, @Op6, @Op7 
    ,@Point1, @Point2 , @Point3, @Point4, @Point5 , @Point6  , @Point7 , @Point8
    ,@Operate, @Remarks
END
")]
        public override void SetUserPoint(int userID, int[] points, out int generalPoint, string operateName,string remarks)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetUserPoint";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateParameter<int>("@Point1", points[0], SqlDbType.Int);
                query.CreateParameter<int>("@Point2", points[1], SqlDbType.Int);
                query.CreateParameter<int>("@Point3", points[2], SqlDbType.Int);
                query.CreateParameter<int>("@Point4", points[3], SqlDbType.Int);
                query.CreateParameter<int>("@Point5", points[4], SqlDbType.Int);
                query.CreateParameter<int>("@Point6", points[5], SqlDbType.Int);
                query.CreateParameter<int>("@Point7", points[6], SqlDbType.Int);
                query.CreateParameter<int>("@Point8", points[7], SqlDbType.Int);
                query.CreateParameter<string>("@Operate", operateName, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Remarks",  remarks , SqlDbType.NVarChar, 200);

                generalPoint = query.ExecuteScalar<int>();
            }
        }

        #endregion

        #region 用户搜索 相关

        //        /// <summary>
        //        /// 获取指定用户的私有数据
        //        /// </summary>
        //        /// <param name="userID">用户ID</param>
        //        #region 存储过程
        //        [StoredProcedure(Name = "bx_GetUserData", Script = @"
        //CREATE PROCEDURE {name}
        //    @UserID     int
        //AS BEGIN
        //    SET NOCOUNT ON;
        //
        //    SELECT * FROM [bx_UserDatas] WHERE [UserID]=@UserID;
        //END")]
        //        #endregion
        //        public override UserData GetUserData(int userID)
        //        {
        //            using (SqlQuery query = new SqlQuery())
        //            {
        //                query.CommandText = "bx_GetUserData";
        //                query.CommandType = CommandType.StoredProcedure;
        //                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);


        //                using (XSqlDataReader dr = query.ExecuteReader())
        //                {
        //                    SqlDataReaderWrap readerWrap = new SqlDataReaderWrap(dr);
        //                    if (readerWrap.Next)
        //                        return new UserData(readerWrap);
        //                }
        //            }
        //            return new UserData();
        //        }

        public override int GetUserID(string username)
        {
            int userID = 0;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT [UserID] FROM [bx_Users] WITH (NOLOCK) WHERE [Username] = @Username";

                query.CreateParameter<string>("@Username", username, SqlDbType.NVarChar, 50);

                userID = query.ExecuteScalar<int>();
            }

            return userID;
        }

        public override List<int> GetUserIDs(IEnumerable<string> usernames)
        {
            List<int> userIDs = new List<int>();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT [UserID] FROM [bx_Users] WITH (NOLOCK) WHERE [Username] IN (@Usernames)";

                query.CreateInParameter<string>("@Usernames", usernames);

                using (IDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userIDs.Add(reader.GetInt32(0));
                    }
                }
            }

            return userIDs;
        }

        public override UserCollection GetUsers(int userID, UserOrderBy orderFieldType, int pageNumber, int pageSize, out int userSortNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_Members]";

                string condition = " [IsActive] = 1 ";
                string sortField = null;

                bool getUserSortNumber = userID > 0;
                sortField = GetOrderFieldName(orderFieldType);

                query.Pager.PrimaryKey = "UserID";
                query.Pager.SortField = sortField;
                query.Pager.PageSize = pageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.SelectCount = true;
                query.Pager.IsDesc = true;
                query.Pager.Condition = condition;

                if (getUserSortNumber)
                {
                    query.Pager.AfterExecute = @"
DECLARE @Number int;
SELECT @Number = " + sortField + @" FROM bx_Members WITH (NOLOCK) WHERE UserID = @UserID AND " + condition + @";
IF @Number IS NOT NULL
    SELECT COUNT(*)+1 FROM bx_Members WITH (NOLOCK) WHERE " + sortField + @" > @Number AND " + condition + @";
";
                    query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                }

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserCollection users = new UserCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            users.TotalRecords = reader.Get<int>(0);
                        }
                    }
                    userSortNumber = 0;
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            userSortNumber = reader.Get<int>(0);
                        }
                    }

                    return users;
                }
            }
        }

        public override UserCollection GetUsers(UserFilter filter, int pageNumber, int? total)
        {
            UserCollection users = null;
            using (SqlQuery query = new SqlQuery())
            {

                query.Pager.TableName = "[bx_Members]";
                query.Pager.SortField = GetOrderFieldName(filter.Order);
                query.Pager.IsDesc = filter.IsDesc;
                query.Pager.PageSize = filter.Pagesize;
                query.Pager.PageNumber = pageNumber > 0 ? pageNumber : 1;
                query.Pager.SelectCount = true;
                if (total != null)
                    query.Pager.TotalRecords = total;
                query.Pager.PrimaryKey = "[UserID]";

                query.Pager.Condition = FilterToCondition(filter, query).ToString();// condition.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    users = new UserCollection(reader);
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            users.TotalRecords = reader.Get<int>(0);
                        }
                    }
                }
            }
            return users;
        }



        #region 存储过程
        [StoredProcedure(Name = "bx_GetUserPassword", Script = @"
CREATE PROCEDURE {name}
	@UserID                 int
AS
BEGIN

	SET NOCOUNT ON;

	SELECT UserID, Password, PasswordFormat FROM [bx_UserVars] WHERE UserID = @UserID;

END")]
        #endregion
        public override UserPassword GetUserPassword(int userID)
        {
            UserPassword user = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUserPassword";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", 0, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserPassword(reader);
                    }
                    else
                        user = null;
                }
            }

            return user;
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_GetAuthUser", Script = @"
CREATE PROCEDURE {name}
	 @Username               nvarchar(50)
	,@UserID                 int
AS
BEGIN

	SET NOCOUNT ON;

	IF @UserID <= 0
        SELECT @UserID = [UserID] FROM [bx_Users] WHERE [Username] = @Username;

    IF @UserID <= 0
        RETURN;

	SELECT * FROM [bx_AuthUsers] WHERE UserID = @UserID;

	SELECT * FROM [bx_Friends] WITH (NOLOCK) WHERE UserID = @UserID ORDER BY [Hot] DESC,[CreateDate] ASC;

    SELECT * FROM [bx_UnreadNotifies] WHERE UserID = @UserID;

END")]
        #endregion

        public override AuthUser GetAuthUser(string username)
        {
            AuthUser user = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAuthUser";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@Username", username, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@UserID", 0, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new AuthUser(reader);
                        if (reader.NextResult())
                        {
                            user.Friends = new FriendCollection(reader);
                        }

                        if (reader.NextResult())
                        {
                            user.UnreadNotify = new UnreadNotifies(reader);
                        }
                        else
                        {
                            user.UnreadNotify = new UnreadNotifies();
                        }
                    }
                    else
                        user = null;
                }
            }

            return user;
        }

        public override AuthUser GetAuthUser(int userID)
        {
            AuthUser user = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAuthUser";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@Username", string.Empty, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new AuthUser(reader);
                        if (reader.NextResult())
                        {
                            user.Friends = new FriendCollection(reader);
                        }

                        if (reader.NextResult())
                        {
                            user.UnreadNotify = new UnreadNotifies(reader);
                        }
                        else
                        {
                            user.UnreadNotify = new UnreadNotifies();
                        }
                    }
                    else
                        user = null;
                }
            }

            return user;
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_GetAuthUserByEmail", Script = @"
CREATE PROCEDURE {name}
     @Email                nvarchar(50)
--,    @RepeatCount           int  out
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @RepeatCount int;

    SELECT @RepeatCount = COUNT(*) FROM (SELECT TOP 2 UserID FROM bx_Users WHERE Email =@Email) AS T;
    
    IF @RepeatCount > 1
        RETURN (2);

    ELSE IF @RepeatCount < 1
        RETURN (3);

    ELSE BEGIN


        DECLARE @UserID int;
        --DECLARE @IsEmailValidated bit;
        SET @UserID = -1;
        --SET @IsEmailValidated = 0;

        SELECT @UserID = [UserID] FROM [bx_Users] WHERE [Email] = @Email;

        IF @UserID > 0 BEGIN

            SELECT * FROM [bx_AuthUsers] WHERE UserID = @UserID;
            SELECT * FROM [bx_Friends] WITH (NOLOCK) WHERE UserID = @UserID ORDER BY [Hot] DESC, [CreateDate] ASC;
            SELECT * FROM [bx_UnreadNotifies] WHERE UserID = @UserID;

        END
        ELSE
            RETURN (3);

    END

END")]
        #endregion
        public override AuthUser GetAuthUserByEmail(string email, out bool duplicateEmail)
        {
            AuthUser user = null;
            duplicateEmail = false;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAuthUserByEmail";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@Email", email, SqlDbType.NVarChar, 50);
                SqlParameter returnParam = query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new AuthUser(reader);
                        if (reader.NextResult())
                        {
                            user.Friends = new FriendCollection(reader);
                        }
                        if (reader.NextResult())
                        {
                            user.UnreadNotify = new UnreadNotifies(reader);
                        }
                        else
                        {
                            user.UnreadNotify = new UnreadNotifies();
                        }

                    }
                }

                if ((int)returnParam.Value == 2)
                    duplicateEmail = true;

            }
            return user;
        }


        #region 存储过程
        [StoredProcedure(Name = "bx_GetUser", Script = @"
CREATE PROCEDURE {name}
	 @Username               nvarchar(50)
	,@UserID                 int
    ,@GetFriends             bit
AS
BEGIN

	SET NOCOUNT ON;

	IF @UserID <= 0
        SELECT @UserID = [UserID] FROM [bx_Users] WITH(NOLOCK) WHERE [Username] = @Username;

    IF @UserID <= 0
        RETURN;

	SELECT * FROM [bx_Users] WITH(NOLOCK) WHERE UserID = @UserID;

    IF @GetFriends = 1
	    SELECT * FROM [bx_Friends] WITH (NOLOCK) WHERE UserID = @UserID ORDER BY [Hot] DESC,[CreateDate] ASC;

END")]
        #endregion
        public override User GetUser(string username, bool getFriends)
        {
            User user = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUser";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@Username", username, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@UserID", 0, SqlDbType.Int);
                query.CreateParameter<bool>("@GetFriends", getFriends, SqlDbType.Bit);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User(reader);
                        if (getFriends && reader.NextResult())
                        {
                            user.Friends = new FriendCollection(reader);
                        }

                    }
                    else
                        user = null;
                }
            }

            return user;
        }

        public override User GetUser(int userID, bool getFriends)
        {
            User user = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUser";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@Username", string.Empty, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<bool>("@GetFriends", getFriends, SqlDbType.Bit);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User(reader);
                        if (getFriends && reader.NextResult())
                        {
                            user.Friends = new FriendCollection(reader);
                        }

                    }
                    else
                        user = null;
                }
            }

            return user;
        }

        public override UserCollection GetUsers(IEnumerable<int> userIDs, bool getFriends)
        {
            UserCollection users;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"SELECT * FROM [bx_Users] WITH(NOLOCK) WHERE [UserID] IN (@UserIds);";

                if (getFriends)
                    query.CommandText += "SELECT * FROM [bx_Friends] WITH (NOLOCK) WHERE [UserID] IN (@UserIds) ORDER BY [UserID],[Hot] DESC;";

                query.CreateInParameter<int>("@UserIds", userIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    users = new UserCollection(reader);

                    if (getFriends && reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            if (reader.Get<int>("GroupID") < 0)
                            {
                                BlacklistItem blacklistItem = new BlacklistItem(reader);
                                users.GetValue(blacklistItem.OwnerID).Blacklist.Add(blacklistItem);
                            }
                            else
                            {
                                Friend friend = new Friend(reader);
                                users.GetValue(friend.OwnerID).Friends.Add(friend);
                            }
                        }
                    }
                }
            }

            return users;
        }

        public override UserCollection GetUsers(IEnumerable<string> usernames, bool getFriends)
        {

            if (!ValidateUtil.HasItems<string>(usernames))
            {
                return new UserCollection();
            }
            UserCollection users;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
DECLARE @UserTable table
(
    [UserID] int
);
INSERT INTO @UserTable SELECT [UserID] FROM [bx_Users] WITH(NOLOCK) WHERE [Username] IN (@Usernames);
SELECT * FROM [bx_Users] WITH(NOLOCK) WHERE UserID IN (SELECT UserID FROM @UserTable);
IF @GetFriends = 1
    SELECT * FROM [bx_Friends] WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM @UserTable);
";

                query.CreateInParameter<string>("@Usernames", usernames);
                query.CreateParameter<bool>("@GetFriends", getFriends, SqlDbType.Bit);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    users = new UserCollection(reader);

                    if (getFriends && reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            if (reader.Get<int>("GroupID") < 0)
                            {
                                BlacklistItem blacklistItem = new BlacklistItem(reader);
                                users.GetValue(blacklistItem.OwnerID).Blacklist.Add(blacklistItem);
                            }
                            else
                            {
                                Friend friend = new Friend(reader);
                                users.GetValue(friend.OwnerID).Friends.Add(friend);
                            }
                        }
                    }
                }
            }

            return users;
        }



        public override UserCollection AdminSearchUsers(AdminUserFilter searchFilter, Guid[] excludeRoles, int pageNumber, out int totalCount)
        {
            UserCollection users = new UserCollection();
            using (SqlQuery query = new SqlQuery())
            {

                StringBuilder _condition;
                _condition = FilterToCondition(searchFilter, query);

                if (excludeRoles != null && excludeRoles.Length > 0)
                    _condition.Append(" AND " + DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoles, query));

                if (searchFilter.Pagesize < 1) searchFilter.Pagesize = 20;

                query.Pager.TableName = "[bx_Members]";
                query.Pager.SortField = GetOrderFieldName(searchFilter.Order);
                query.Pager.IsDesc = searchFilter.IsDesc;
                query.Pager.PageSize = searchFilter.Pagesize;
                query.Pager.PageNumber = pageNumber > 0 ? pageNumber : 1;
                query.Pager.SelectCount = true;
                query.Pager.PrimaryKey = "[UserID]";

                if (_condition != null && _condition.Length > 0)
                    query.Pager.Condition = _condition.ToString();

                totalCount = 0;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    users = new UserCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                }
            }
            return users;
        }

        public override UserCollection SearchUsers(AdminUserFilter searchFilter, int userID, int pageNumber, out int totalCount)
        {
            UserCollection users = new UserCollection();

            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder _condition = null;

                if (searchFilter == null)
                {
                    searchFilter = new AdminUserFilter();
                }

                _condition = FilterToCondition(searchFilter, query);

                if (!searchFilter.ShowSelf)
                {
                    _condition.Append(" AND [UserID] <> " + userID + " ");
                }

                if (!searchFilter.ShowFriend)
                {
                    _condition.Append(" AND UserID NOT IN (SELECT [FriendUserID] FROM [bx_Friends] WITH (NOLOCK) WHERE [UserID]=" + userID + ") ");
                }


                if (searchFilter.Pagesize < 1)
                    searchFilter.Pagesize = 20;

                totalCount = 0;
                query.Pager.TableName = "bx_Members";
                query.Pager.SortField = GetOrderFieldName(searchFilter.Order);
                query.Pager.IsDesc = searchFilter.IsDesc;
                query.Pager.PageSize = searchFilter.Pagesize;
                query.Pager.PageNumber = pageNumber > 0 ? pageNumber : 1; query.Pager.SelectCount = true;
                query.Pager.PrimaryKey = "UserID";
                if (_condition != null && _condition.Length > 0)
                    query.Pager.Condition = _condition.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    users = new UserCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                }
            }
            return users;
        }
        /// <summary>
        /// 查询未使用ID
        /// </summary>
        /// <returns>每段未使用的ID(段起始ID, 段结束ID)</returns>
        #region 存储过程
        [StoredProcedure(Name = "bx_GetNotUseUserIDs", Script = @"
CREATE PROCEDURE {name}
     @BeginID     int
    ,@EndID       int
AS BEGIN
    SET NOCOUNT ON;

    SELECT (T1.UserID + 1) FROM bx_Users T1 WITH (NOLOCK)
     WHERE
	    (T1.UserID + 1) NOT IN 
            (SELECT T2.UserID FROM bx_Users T2 WITH (NOLOCK) WHERE T2.UserID > @BeginID AND T2.UserID < @EndID)
	    AND T1.UserID > @BeginID AND T1.UserID < @EndID
    ORDER BY T1.UserID;

    SELECT (T1.UserID - 1) FROM bx_Users T1 WITH (NOLOCK)
     WHERE 
	    (T1.UserID - 1) NOT IN 
            (SELECT T2.UserID FROM bx_Users T2 WITH (NOLOCK) WHERE T2.UserID > @BeginID AND T2.UserID < @EndID)
	    AND T1.UserID > @BeginID AND T1.UserID < @EndID
    ORDER BY T1.UserID;
END")]
        #endregion
        public override List<Int32Scope> GetNotUseUserIDs(int beginID, int endID)
        {
            List<int> beginIDs = new List<int>();
            List<int> endIDs = new List<int>();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetNotUseUserIDs";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@BeginID", beginID, SqlDbType.Int);
                query.CreateParameter<int>("@EndID", endID, SqlDbType.Int);


                using (IDataReader dr = query.ExecuteReader())
                {
                    while (dr.Read())
                        beginIDs.Add(dr.GetInt32(0));

                    if (dr.NextResult())
                    {
                        while (dr.Read())
                            endIDs.Add(dr.GetInt32(0));
                    }
                }
            }

            List<Int32Scope> result = new List<Int32Scope>();

            if (endIDs.Count > 0)
            {
                if (endIDs[0] != beginID)
                    result.Add(new Int32Scope(beginID, endIDs[0]));

                for (int i = 0; i < endIDs.Count - 1; i++)
                {
                    result.Add(new Int32Scope(beginIDs[i], endIDs[i + 1]));
                }

                if (beginIDs[beginIDs.Count - 1] != endID)
                    result.Add(new Int32Scope(beginIDs[beginIDs.Count - 1], endID));
            }

            if (result.Count == 0 && (endIDs.Count == 1 && endIDs[0] == beginID && beginIDs[0] == endID) == false)
                result.Add(new Int32Scope(beginID, endID));
            return result;
        }

        public override List<int> GetUserIDs(int beginID, int endID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT UserID FROM bx_Users WHERE UserID >= @BeginUserID AND UserID <= @EndUserID;";
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@BeginUserID", beginID, SqlDbType.Int);
                query.CreateParameter<int>("@EndUserID", endID, SqlDbType.Int);

                List<int> userIDs = new List<int>();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userIDs.Add(reader.GetInt32(0));
                    }
                }

                return userIDs;
            }
        }
        
        
        
        /// <summary>
        /// 在多少分钟内是否存在某IP
        /// </summary>
        #region 存储过程
        [StoredProcedure(Name = "bx_IPIsExistInMinutes", Script = @"
CREATE PROCEDURE {name}
     @IP        nvarchar(50)
    ,@TimeSpan  int
AS BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT [UserID] FROM [bx_Users] WITH (NOLOCK) WHERE CreateIP = @IP AND abs(datediff(mi,CreateDate,getdate())) < @TimeSpan AND [UserID]<>0)
	    SELECT 1;
    ELSE
	    SELECT 0;
END")]
        #endregion
        public override bool IPIsExistInMinutes(string ip, int timeSpan)
        {
            object result = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_IPIsExistInMinutes";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@IP", ip, SqlDbType.VarChar, 50);
                query.CreateParameter<int>("@TimeSpan", timeSpan, SqlDbType.Int);


                result = query.ExecuteScalar();
            }

            return Convert.ToInt32(result) == 1;
        }

        ///// <summary>
        ///// 用户列表
        ///// </summary>
        //[Obsolete]
        //public override UserCollection GetUserlist(UserOrderBy orderField, int pageNumber, int pageSize, bool desc)
        //{

        //    using (SqlQuery query = new SqlQuery())
        //    {
        //        query.Pager.TableName = "[bx_Users]";
        //        query.Pager.SortField = GetOrderFieldName(orderField);
        //        query.Pager.PrimaryKey = "UserID";
        //        query.Pager.PageSize = pageSize;
        //        query.Pager.PageNumber = pageNumber;
        //        query.Pager.SelectCount = false;
        //        query.Pager.IsDesc = desc;

        //        using (XSqlDataReader reader = query.ExecuteReader())
        //        {
        //            return new UserCollection(reader);
        //        }
        //    }
        //}
        #endregion

        [StoredProcedure(Name = "bx_DeleteUser", Script = @"

CREATE PROCEDURE {name}
 @UserID             int 
,@Step               int
AS 
BEGIN
    SET NOCOUNT ON;

DECLARE @TotalCount  int
SET @TotalCount=0;

    IF @Step = 1  BEGIN --动态
	    DELETE [bx_Feeds] WHERE [TargetUserID] = @UserID;
        DELETE [bx_UserFeeds] WHERE [UserID] = @UserID;
        SET @TotalCount= 0;
    END

    IF @Step = 2  BEGIN
        --删除评论
	    DELETE [bx_Comments] WHERE  [TargetID] = @UserID OR [UserID] = @UserID;
        SET @TotalCount= 0;
    END

    IF @Step = 3  BEGIN
       --通知
	   DELETE FROM [bx_Notify] WHERE [UserID] = @UserID;
       SET @TotalCount= 0;
    END	

    IF @Step = 4  BEGIN
    	--删除邀请码
	    DELETE FROM [bx_InviteSerials]  WHERE UserID = @UserID OR ToUserID = @UserID;
       SET @TotalCount= 0;
    END

    IF @Step = 5  BEGIN
    --去除邀请关系
       UPDATE [bx_UserInfos]  SET InviterID = 0 WHERE InviterID = @UserID;
       SET @TotalCount= 0;
    END

    IF @Step = 6  BEGIN
       --删除网络硬盘
       DECLARE @TopDirID int
       SET @TopDirID= (SELECT TOP 1 DirectoryID  FROM bx_DiskDirectories WHERE UserID = @UserID Order By DirectoryID DESC);
       DELETE FROM bx_DiskFiles WHERE DirectoryID = @TopDirID;
       DELETE FROM bx_DiskDirectories WHERE DirectoryID = @TopDirID;
       SET @TotalCount= (SELECT COUNT(*) FROM bx_DiskDirectories WHERE UserID = @UserID);
    END

    IF @Step = 7  BEGIN
       --删除自定义表情
       DECLARE @TopGroupID int
       SET @TopGroupID= (SELECT TOP 1 GroupID  FROM bx_EmoticonGroups WHERE UserID = @UserID Order By GroupID DESC);
       DELETE FROM bx_Emoticons WHERE GroupID = @TopGroupID;
       DELETE FROM bx_EmoticonGroups WHERE GroupID = @TopGroupID;
       SET @TotalCount= (SELECT COUNT(*) FROM bx_EmoticonGroups WHERE UserID = @UserID);
    END

    IF @Step = 8  BEGIN
       --删除对话记录
       DELETE FROM bx_ChatMessages WHERE MessageID IN(SELECT TOP 200 MessageID  FROM bx_ChatMessages WHERE UserID = @UserID OR TargetUserID = @UserID);
       SET @TotalCount= (SELECT COUNT(*) FROM bx_ChatMessages WHERE UserID = @UserID OR TargetUserID = @UserID);
       IF @TotalCount=0 
        DELETE FROM bx_ChatSessions WHERE  UserID = @UserID OR TargetUserID = @UserID;
    END
    
    IF @Step = 9  BEGIN
       --删除记录(DOING)
       DELETE FROM bx_Doings WHERE DoingID IN(SELECT TOP 200 DoingID  FROM bx_Doings WHERE UserID = @UserID);
       SET @TotalCount= (SELECT COUNT(*) FROM bx_Doings WHERE UserID = @UserID);
    END

    IF @Step = 10  BEGIN
       --删除分享、收藏
       DELETE FROM bx_Shares WHERE ShareID IN(SELECT TOP 200 ShareID  FROM bx_Shares WHERE UserID = @UserID);
       SET @TotalCount= (SELECT COUNT(*) FROM bx_Shares WHERE UserID = @UserID);
    END

    IF @Step = 11  BEGIN
	--删除空间、文章访问或者被访记录
	   DELETE FROM bx_Visitors WHERE UserID = @UserID OR VisitorUserID =@UserID;
       DELETE FROM bx_BlogArticleVisitors WHERE UserID = @UserID;
       SET @TotalCount= 0;
    END

    IF @Step = 12  BEGIN
    --删除博客文章
	Delete [bx_BlogArticles] WHERE ArticleID IN(SELECT TOP 200 ArticleID FROM [bx_BlogArticles] WHERE  [UserID] = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_BlogArticles] WHERE UserID = @UserID);
    END

    IF @Step = 13  BEGIN
    --删除博客相册
	Delete [bx_Albums] WHERE AlbumID IN(SELECT TOP 200 AlbumID FROM [bx_Albums] WHERE  [UserID] = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_Albums] WHERE UserID = @UserID);
    END

    IF @Step = 14  BEGIN
    --删附件
    Delete [bx_Attachments] WHERE AttachmentID IN(SELECT TOP 200 AttachmentID FROM [bx_Attachments] WHERE  [UserID] = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_Attachments] WHERE UserID = @UserID);
    END

    IF @Step = 15 BEGIN
    --删帖子
    Delete [bx_Posts] WHERE PostID IN(SELECT TOP 200 PostID FROM [bx_Posts] WHERE  [UserID] = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_Posts] WHERE UserID = @UserID);
    END

    IF @Step = 16 BEGIN
    --删主题
    Delete [bx_Threads] WHERE ThreadID IN(SELECT TOP 200 ThreadID FROM [bx_Threads] WHERE [PostUserID] = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_Threads] WHERE PostUserID = @UserID);
    END

    IF @Step = 17 BEGIN
    --删好友
    Delete [bx_Friends] WHERE FriendUserID = @UserID OR UserID = @UserID
    SET @TotalCount= 0;
    END    

    IF @Step = 18 BEGIN
    --删访问记录
    Delete [bx_Visitors] WHERE ID IN(SELECT TOP 200 ID FROM [bx_Visitors] WHERE UserID = @UserID OR VisitorUserID = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_Visitors] WHERE UserID = @UserID OR VisitorUserID = @UserID);

    END

    IF @Step = 19 BEGIN
    --删除用户积分明细
    Delete [bx_PointLogs] WHERE LogID IN(SELECT TOP 200 LogID FROM [bx_PointLogs] WHERE UserID = @UserID);
    SET @TotalCount = (SELECT Count(*) FROM [bx_PointLogs] WHERE UserID = @UserID);

    END

    IF @Step = 20 BEGIN
        --最后删除主用户记录
	    DELETE [bx_Users] WHERE [UserID] = @UserID;
        SET @TotalCount = 0;
    END

    IF @Step = 21 BEGIN
        --重新统计用户数据
        EXECUTE bx_UpdateNewUserVars 0;
        SET @TotalCount = 0;
    END
    SELECT @TotalCount;
END
")]

        public override int DeleteUser(int userID, int step)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeleteUser";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@Step", step, SqlDbType.Int);
                return query.ExecuteScalar<int>();
            }
        }

        [StoredProcedure(Name = "bx_SetRealnameCheck", Script = @"
            CREATE PROCEDURE {name} 
             @OperatorUserID   int
            ,@UserID           int
            ,@IsChecked        bit
            ,@Remark           nvarchar(1000)
            AS
BEGIN
            SET NOCOUNT ON;
            DECLARE @Success bit;
            SET  @Success = 0;
            IF EXISTS( SELECT * FROM bx_AuthenticUsers WHERE UserID = @UserID ) BEGIN 
                IF @IsChecked = 1 BEGIN
                    DECLARE @Realname nvarchar(50);
                    SET @Realname =( SELECT Realname FROM bx_AuthenticUsers WHERE UserID = @UserID) ;
                    UPDATE bx_Users SET Realname = @Realname WHERE UserID = @UserID;
                END
                ELSE BEGIN
                    UPDATE bx_Users SET Realname = '' WHERE UserID = @UserID;
                END

                UPDATE bx_AuthenticUsers SET Verified = @IsChecked, Processed = 1, OperatorUserID = @OperatorUserID,  Remark = @Remark  WHERE UserID = @UserID;
                SET @Success = 1;
            END
    SELECT @Success; 
END
")]

        public override bool SetRealnameChecked(int operatorUserID, int userID, bool nameChecked, string remark)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetRealnameCheck";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@OperatorUserID", operatorUserID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<bool>("@IsChecked", nameChecked, SqlDbType.Bit);
                query.CreateParameter<string>("@Remark", remark, SqlDbType.NVarChar, 1000);

                return query.ExecuteScalar<bool>();
            }
        }

        /// <summary>
        /// 如果从接口获取到照片则更新照片
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="photos"></param>
        /// <returns></returns>
        public override bool UpdateAuthenticUserPhoto(int userID, string photos, int state)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_AuthenticUsers SET Photo = @Photo,IsDetect=1,DetectedState=@DetectedState WHERE UserID = @UserID";
                query.CreateParameter<string>("@Photo", photos, SqlDbType.NVarChar, 100);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@DetectedState", state, SqlDbType.Int);

                return query.ExecuteNonQuery() > 0;
            }
        }


        /// <summary>
        /// 直接更新用户的Realname字段
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public override bool UpdateUserRealname(int userID, string realname)
        {
            string sql = "UPDATE bx_Users SET Realname = @Realname WHERE UserID = @UserID";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Realname", realname, SqlDbType.NVarChar, 50);

                return query.ExecuteNonQuery() > 0;
            }
        }


        #region 实名认证
        /// <summary>
        /// 获取真实的用户身份信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public override AuthenticUser GetAuthenticUser(int userid)
        {
            string cmd = "SELECT * FROM bx_AuthenticUsers WHERE UserID = @UserID";
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = cmd;
                query.CreateParameter<int>("@UserID", userid, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    AuthenticUser user = null;
                    if (reader.Next)
                    {
                        user = new AuthenticUser(reader);
                    }

                    return user;
                }
            }
        }

        /// <summary>
        /// 获取真实的用户身份信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public override AuthenticUserCollection GetAuthenticUsers(AuthenticUserFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder conditionBuilder = new StringBuilder();

                if (!string.IsNullOrEmpty(filter.TempRealname))
                {
                    conditionBuilder.Append(" AND Realname = @Realname");
                    query.CreateParameter<string>("@Realname", filter.TempRealname, SqlDbType.NVarChar, 50);
                }

                if (filter.Gender != null && filter.Gender.Value != Gender.NotSet)
                {
                    conditionBuilder.Append(" AND Gender = @Gender");
                    query.CreateParameter<byte>("@Gender", (byte)filter.Gender.Value, SqlDbType.TinyInt);
                }

                if (filter.Username != null && filter.Username.Trim() != "")
                {
                    conditionBuilder.Append(" AND UserID IN( SELECT UserID FROM bx_Users WHERE Username LIKE '%'+ @Username + '%')");
                    query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                }

                if (filter.Processed != null)
                {
                    conditionBuilder.Append(" AND Processed = @Processed");
                    query.CreateParameter<bool>("@Processed", filter.Processed.Value, SqlDbType.Bit);
                }

                if (filter.Verified != null)
                {
                    conditionBuilder.Append(" AND Verified = @Verified");
                    query.CreateParameter<bool>("@Verified", filter.Verified.Value, SqlDbType.Bit);
                }

                if (conditionBuilder.Length > 0)
                    conditionBuilder.Remove(0, 4);


                query.Pager.TableName = "bx_AuthenticUsers";
                query.Pager.PageSize = filter.PageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PrimaryKey = "UserID";
                query.Pager.SortField = "CreateDate";
                query.Pager.IsDesc = true;
                query.Pager.Condition = conditionBuilder.ToString();
                query.Pager.SelectCount = true;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    AuthenticUserCollection users = new AuthenticUserCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Next)
                            users.TotalRecords = reader.Get<int>(0);
                    }

                    return users;
                }
            }
        }

        /// <summary>
        /// 保存用户的实名认证信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="realname">真实姓名</param>
        /// <param name="idNumber">身份证号码</param>
        /// <param name="idCardFile">身份证扫描件保持路径</param>
        /// <param name="birthday">生日</param>
        /// <param name="gender">性别</param>
        /// <param name="area">所在地</param>
        [StoredProcedure(Name = "bx_SaveAuthenticUserInfo", Script = @"
CREATE PROCEDURE {name}
@UserID             int
,@Realname          nvarchar(50)
,@Birthday          datetime
,@Gender            tinyint
,@IDNumber          varchar(50)
,@IDCardFileFace    nvarchar(100)
,@IDCardFileBack    nvarchar(100)
,@Area              nvarchar(100) 
AS
BEGIN
SET NOCOUNT ON;
IF EXISTS( SELECT * FROM bx_AuthenticUsers WHERE UserID = @UserID) BEGIN
    UPDATE bx_AuthenticUsers SET [Realname] = @Realname, [Gender] = @Gender, [Birthday] = @Birthday, [IDNumber] = @IDNumber, [IDCardFileFace] = @IDCardFileFace, [IDCardFileBack]=@IDCardFileBack, [Area] = @Area, Processed = 0 WHERE UserID = @UserID
    SELECT 1;
END
ELSE BEGIN
    INSERT INTO bx_AuthenticUsers([UserID], [Realname], [Gender], [Birthday], [IDNumber], [IDCardFileFace],[IDCardFileBack], [Area]) VALUES(@UserID, @Realname, @Gender, @Birthday, @IDNumber, @IDCardFileFace,@IDCardFileBack, @Area)
    SELECT 0;
END
END 
")]
        public override bool SaveAuthenticUserInfo(int userID, string realname, string idNumber, string idCardFileFace, string idCardFileBack, DateTime birthday, Gender gender, string area)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SaveAuthenticUserInfo";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Realname", realname, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@IDNumber", idNumber, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@IDCardFileFace", idCardFileFace, SqlDbType.NVarChar, 100);
                query.CreateParameter<string>("@IDCardFileBack", idCardFileBack, SqlDbType.NVarChar, 100);
                query.CreateParameter<DateTime>("@Birthday", birthday, SqlDbType.DateTime);
                query.CreateParameter<byte>("@Gender", (byte)gender, SqlDbType.TinyInt);
                query.CreateParameter<string>("@Area", area, SqlDbType.NVarChar, 100);

                return query.ExecuteScalar<int>() == 0;
            }
        }

        [StoredProcedure(Name = "bx_CheckIdNumberExist", Script = @"
CREATE PROCEDURE {name}
@IDNumber       varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS(SELECT * FROM bx_AuthenticUsers WHERE IDNumber=@IDNumber AND Verified=1)
        RETURN(1);
    ELSE
        RETURN(0);
END
")]

        public override bool CheckIdNumberExist(string idNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CheckIdNumberExist";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@IDNumber", idNumber, SqlDbType.VarChar, 50);

                SqlParameter returnParam = query.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);
                query.ExecuteNonQuery();

                return (int)returnParam.Value == 1;
            }
        }

        #endregion

        /// <summary>
        /// 修改用户密码
        /// </summary>
        #region 存储过程
        [StoredProcedure(Name = "bx_ResetUserPassword", Script = @"
CREATE PROCEDURE {name}
     @UserID     int
    ,@Password   nvarchar(50)
    ,@Format     tinyint
AS BEGIN
    SET NOCOUNT ON;

    UPDATE [bx_UserVars] SET Password = @Password, PasswordFormat = @Format WHERE UserID = @UserID;
END")]
        #endregion
        public override void ResetUserPassword(int userID, string encodedPassword, EncryptFormat format)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_ResetUserPassword";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Password", encodedPassword, SqlDbType.NVarChar, 50);
                query.CreateParameter<byte>("@Format", (byte)format, SqlDbType.TinyInt);
                query.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="avatar">头像地址</param>
        #region StoredProcedure
        [StoredProcedure(Name = "bx_UpdateAvatar", Script = @"
CREATE PROCEDURE {name}
    @UserID           int
   ,@AvatarSrc        nvarchar(200)
   ,@Checked          bit
AS BEGIN
    SET NOCOUNT ON;

    UPDATE [bx_Users] SET [AvatarSrc] = @AvatarSrc WHERE [UserID] = @UserID; 
    UPDATE [bx_UserVars] SET EverAvatarChecked = @Checked WHERE [UserID] = @UserID;
    
END")]
        #endregion
        public override bool UpdateAvatar(int userID, string avatarSrc, bool avatarChecked)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateAvatar";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<bool>("@Checked", avatarChecked, SqlDbType.Bit);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@AvatarSrc", avatarSrc, SqlDbType.NVarChar, 200);

                query.ExecuteNonQuery();
            }
            return true;
        }

        #region 用户组操作

        [StoredProcedure(Name = "bx_AddUserToRole", Script = @"
CREATE PROCEDURE {name}
     @UserID      int
    ,@RoleID      uniqueIdentifier
    ,@BeginDate   datetime
    ,@EndDate     datetime
AS BEGIN

    SET NOCOUNT ON;

    IF EXISTS ( SELECT * FROM bx_Users WHERE UserID = @UserID ) BEGIN

        IF EXISTS ( SELECT * FROM bx_UserRoles WHERE UserID = @UserID AND RoleID = @RoleID )
            UPDATE bx_UserRoles SET BeginDate = @BeginDate, EndDate = @EndDate WHERE UserID = @UserID AND RoleID = @RoleID;
        ELSE
            INSERT INTO bx_UserRoles (UserID, RoleID, BeginDate, EndDate) VALUES (@UserID, @RoleID, @BeginDate, @EndDate);

    END

END
")]
        public override void AddUsersToRoles(UserRoleCollection userRoles)
        {
            if (userRoles == null || userRoles.Count == 0)
                return;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_AddUserToRole";
                query.CommandType = CommandType.StoredProcedure;

                foreach (UserRole userRole in userRoles)
                {
                    query.CreateParameter<int>("@UserID", userRole.UserID, SqlDbType.Int);
                    query.CreateParameter<Guid>("@RoleID", userRole.RoleID, SqlDbType.UniqueIdentifier);
                    query.CreateParameter<DateTime>("@BeginDate", userRole.BeginDate, SqlDbType.DateTime);
                    query.CreateParameter<DateTime>("@EndDate", userRole.EndDate, SqlDbType.DateTime);

                    query.ExecuteNonQuery();
                }
            }
        }

        public override int RemoveUsersFromRoles(IEnumerable<int> userIds, IEnumerable<Guid> roleIds)
        {
            if (userIds == null || ValidateUtil.HasItems(userIds) == false)
                return 0;

            if (roleIds == null || ValidateUtil.HasItems(roleIds) == false)
                return 0;

            int rows = 0;

            using (SqlQuery query = new SqlQuery())
            {
                foreach (Guid roleID in roleIds)
                {
                    query.CommandText = "DELETE bx_UserRoles WHERE RoleID = @RoleID AND UserID IN (@UserIds)";
                    query.Parameters.Clear();

                    query.CreateParameter<Guid>("@RoleID", roleID, SqlDbType.UniqueIdentifier);

                    query.CreateInParameter<int>("@UserIds", userIds);

                    rows += query.ExecuteNonQuery();
                }
            }

            return rows;
        }

        public override void UpdateUserRoles(int targetUserId, UserRoleCollection userRoles)
        {
            using (SqlSession session = new SqlSession())
            {
                using (SqlQuery query = session.CreateQuery())
                {
                    query.CommandText = @"DELETE bx_UserRoles WHERE UserID = @UserID;";

                    query.CreateParameter<int>("@UserID", targetUserId, SqlDbType.Int);

                    query.ExecuteNonQuery();
                }

                using (SqlQuery query = session.CreateQuery(QueryMode.Prepare))
                {
                    query.CommandText = @"INSERT INTO bx_UserRoles (UserID, RoleID, BeginDate, EndDate) VALUES (@UserID, @RoleID, @BeginDate, @EndDate)";

                    foreach (UserRole userRole in userRoles)
                    {
                        query.CreateParameter<int>("@UserID", targetUserId, SqlDbType.Int);
                        query.CreateParameter<Guid>("@RoleID", userRole.RoleID, SqlDbType.UniqueIdentifier);
                        query.CreateParameter<DateTime>("@BeginDate", userRole.BeginDate, SqlDbType.DateTime);
                        query.CreateParameter<DateTime>("@EndDate", userRole.EndDate, SqlDbType.DateTime);

                        query.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion

        [StoredProcedure(Name = "bx_UpdateUserExtendedFieldVersion", Script = @"
CREATE PROCEDURE {name}
	@UserID					int,
    @ExtendedFieldVersion	nchar(36)
AS BEGIN

	SET NOCOUNT ON;

	UPDATE [bx_Users] SET [ExtendedFieldVersion] = @ExtendedFieldVersion WHERE [UserID] = @UserID;

END")]
        public override void UpdateExtendedFieldVersion(int userID, string version)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateUserExtendedFieldVersion";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@ExtendedFieldVersion", version, SqlDbType.NChar, 36);

                query.ExecuteNonQuery();
            }
        }


        [StoredProcedure(Name = "bx_GetUserIDsBySearch", Script = @"
CREATE PROCEDURE {name}
	@Keyword   nvarchar(100)
AS BEGIN

	SET NOCOUNT ON;

	SELECT [UserID] FROM [bx_Users] WITH (NOLOCK) WHERE [Username] like'%'+@Keyword+'%';

END")]
        public override List<int> GetUserIDs(string keyword)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUserIDsBySearch";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@Keyword", keyword, SqlDbType.NVarChar, 100);

                List<int> userIDs = new List<int>();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                        userIDs.Add(reader.Get<int>(0));
                }

                return userIDs;
            }
        }

        [StoredProcedure(Name = "bx_UpdateUserSelectFriendGroupID", Script = @"
CREATE PROCEDURE {name}
	 @GroupID   int
    ,@UserID    int
AS BEGIN

	SET NOCOUNT ON;

	UPDATE [bx_UserVars] SET SelectFriendGroupID = @GroupID WHERE UserID = @UserID;

END")]
        public override void UpdateUserSelectFriendGroupID(int userID, int groupID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateUserSelectFriendGroupID";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@GroupID", groupID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }


        [StoredProcedure(Name = "bx_UpdateUserReplyReturnThreadLastPage", Script = @"
CREATE PROCEDURE {name}
	 @ReplyReturnThreadLastPage   bit
    ,@UserID    int
AS BEGIN

	SET NOCOUNT ON;

	UPDATE [bx_UserVars] SET ReplyReturnThreadLastPage = @ReplyReturnThreadLastPage WHERE UserID = @UserID;

END")]
        public override void UpdateUserReplyReturnThreadLastPage(int userID, bool returnLastPage)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateUserReplyReturnThreadLastPage";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<bool>("@ReplyReturnThreadLastPage", returnLastPage, SqlDbType.Bit);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        //        [StoredProcedure(Name = "bx_UpdateUserOnlineStatus", Script = @"
        //CREATE PROCEDURE {name}
        //	 @UserID         int
        //    ,@OnlineStatus   tinyint
        //AS BEGIN
        //
        //	SET NOCOUNT ON;
        //
        //	UPDATE [bx_UserInfos] SET [OnlineStatus] = @OnlineStatus WHERE [UserID]=@UserID;
        //
        //END")]
        //        public override void UpdateUserOnlineStatus(int userID, OnlineStatus onlineStatus)
        //        {
        //            using (SqlQuery query = new SqlQuery())
        //            {
        //                query.CommandText = "bx_UpdateUserOnlineStatus";
        //                query.CommandType = CommandType.StoredProcedure;

        //                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
        //                query.CreateParameter<int>("@OnlineStatus", (int)onlineStatus, SqlDbType.TinyInt);

        //                query.ExecuteNonQuery();
        //            }
        //        }

        [StoredProcedure(Name = "bx_UpdateOnlineTime", FileName = "v30\\bx_UpdateOnlineTime.sql")]
        public override bool UpdateOnlineTime(List<UserOnlineInfo> userOnlineInfos, out int[] totalOnlineTimes, out int[] monthOnlineTimes, out int[] weekOnlineTimes, out int[] dayOnlineTimes)
        {
            totalOnlineTimes = null;
            monthOnlineTimes = null;
            weekOnlineTimes = null;
            dayOnlineTimes = null;

            if (userOnlineInfos.Count == 0)
                return true;

            StringBuilder onlineTimesBuilder = new StringBuilder();
            StringBuilder updateOnlineTimesBuilder = new StringBuilder();
            StringBuilder userIdsBuilder = new StringBuilder();

            bool isFirst = true;
            foreach (UserOnlineInfo info in userOnlineInfos)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    onlineTimesBuilder.Append(',');
                    updateOnlineTimesBuilder.Append(',');
                    userIdsBuilder.Append(',');
                }

                onlineTimesBuilder.Append(info.OnlineMinutes);
                updateOnlineTimesBuilder.Append(info.UpdateDate);
                userIdsBuilder.Append(info.UserID);
            }

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateOnlineTime";
                query.CommandType = CommandType.StoredProcedure;
                query.CommandTimeout = int.MaxValue;

                query.CreateParameter<string>("@OnlineTimes", onlineTimesBuilder.ToString(), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@LastUpdateOnlineTimes", updateOnlineTimesBuilder.ToString(), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@UserIDs", userIdsBuilder.ToString(), SqlDbType.VarChar, 8000);
                string totalOnlineTimeString = "";
                string monthOnlineTimeString = "";
                string weekOnlineTimeString = "";
                string dayOnlineTimeString = "";

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        totalOnlineTimeString = reader.GetString(0);
                        monthOnlineTimeString = reader.GetString(1);
                        weekOnlineTimeString = reader.GetString(2);
                        dayOnlineTimeString = reader.GetString(3);
                    }
                }

                totalOnlineTimes = StringUtil.Split<int>(totalOnlineTimeString, ',');
                monthOnlineTimes = StringUtil.Split<int>(monthOnlineTimeString, ',');
                weekOnlineTimes = StringUtil.Split<int>(weekOnlineTimeString, ',');
                dayOnlineTimes = StringUtil.Split<int>(dayOnlineTimeString, ',');
            }
            return true;
        }

        [StoredProcedure(Name = "bx_AddMedalUsers", Script = @"
CREATE PROCEDURE {name}
     @MedalID        int
    ,@MedalLevelID   int
	,@UserIDs        varchar(8000)
    ,@EndDate        datetime
    ,@Url            nvarchar(200)
AS BEGIN

	SET NOCOUNT ON;

    BEGIN TRANSACTION    

    DECLARE @UserIDsTable table(ID int identity(1,1), UserID int);
    
    INSERT INTO @UserIDsTable (UserID) SELECT item FROM bx_GetIntTable(@UserIDs, ',');
    
    EXEC('DELETE [bx_UserMedals] WHERE MedalID='+@MedalID+' AND UserID in('+@UserIDs+')');
    IF(@@error<>0) BEGIN
        ROLLBACK TRANSACTION
        RETURN -1;
    END
    
    INSERT INTO [bx_UserMedals](MedalID,MedalLevelID,UserID,EndDate,Url) SELECT @MedalID,@MedalLevelID,UserID,@EndDate,@Url FROM @UserIDsTable;
    IF(@@error<>0) BEGIN
        ROLLBACK TRANSACTION
        RETURN -1;
    END

    COMMIT TRANSACTION

    RETURN 0;

END")]
        public override bool AddMedalUsers(int medalID, int medalLevelID, IEnumerable<int> userIDs, DateTime endDate, string url)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_AddMedalUsers";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@MedalID", medalID, SqlDbType.Int);
                query.CreateParameter<int>("@MedalLevelID", medalLevelID, SqlDbType.Int);
                query.CreateParameter<string>("@UserIDs", StringUtil.Join(userIDs), SqlDbType.VarChar, 8000);
                query.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);
                query.CreateParameter<string>("@Url", url, SqlDbType.NVarChar, 200);

                query.CreateParameter<int>("@ResultCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return query.Parameters["@ResultCode"].Value.ToString() == "0";
            }
        }


        [StoredProcedure(Name = "bx_AddMedalsToUser", Script = @"
CREATE PROCEDURE {name}
     @MedalIDs       varchar(8000)
    ,@MedalLevelIDs  varchar(8000)
	,@UserID         int
    ,@EndDates       varchar(8000)
AS BEGIN

	SET NOCOUNT ON;

    BEGIN TRANSACTION    

    DECLARE @tempTable table(TempID int identity(1,1), MedalID int, MedalLevelID int, EndDate datetime);
    
    INSERT INTO @tempTable (MedalID) SELECT item FROM bx_GetIntTable(@MedalIDs, ',');

    UPDATE @tempTable SET
			[MedalLevelID] = T.item
			FROM bx_GetIntTable(@MedalLevelIDs, N',') T
			WHERE TempID = T.id;

    UPDATE @tempTable SET
			[EndDate] = T.item
			FROM bx_GetStringTable_text(@EndDates, N',') T
			WHERE TempID = T.id;
    
    EXEC('DELETE [bx_UserMedals] WHERE UserID = ' + @UserID + ' AND MedalID in(' + @MedalIDs + ')');
    IF(@@error<>0) BEGIN
        ROLLBACK TRANSACTION
        RETURN -1;
    END
    
    INSERT INTO [bx_UserMedals](MedalID,MedalLevelID,UserID,EndDate) SELECT MedalID,MedalLevelID,@UserID,EndDate FROM @tempTable;
    IF(@@error<>0) BEGIN
        ROLLBACK TRANSACTION
        RETURN -1;
    END

    COMMIT TRANSACTION

    RETURN 0;

END")]
        public override bool AddMedalsToUser(int userID, Dictionary<int, int> medalIDs, List<DateTime> endDates)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_AddMedalsToUser";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@MedalIDs", StringUtil.Join(medalIDs.Keys), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@MedalLevelIDs", StringUtil.Join(medalIDs.Values), SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@EndDates", StringUtil.Join(endDates), SqlDbType.VarChar, 8000);

                query.CreateParameter<int>("@ResultCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return query.Parameters["@ResultCode"].Value.ToString() == "0";
            }
        }



        public override bool UpdateUserMedalEndDate(int medalID, Dictionary<int, DateTime> endDates, Guid[] excludeRoleIDs, Dictionary<int, string> urls)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string condition = DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query);
                if (condition != string.Empty)
                    condition = " AND " + condition;

                StringBuilder sql = new StringBuilder();
                sql.Append("DECLARE @UserIDsTable table(TempID int identity(1,1), UID int,EndDate datetime, Url nvarchar(200));");

                foreach (int uid in endDates.Keys)
                {
                    string url;
                    urls.TryGetValue(uid, out url);
                    if (url == null)
                        url = string.Empty;
                    sql.AppendFormat("INSERT INTO @UserIDsTable(UID,EndDate,Url) VALUES(@UserID_{0},@EndDate_{0},@Url_{0});", uid);

                    query.CreateParameter<int>("@UserID_" + uid, uid, SqlDbType.Int);
                    query.CreateParameter<DateTime>("@EndDate_" + uid, endDates[uid], SqlDbType.DateTime);
                    query.CreateParameter<string>("@Url_" + uid, url, SqlDbType.NVarChar, 200);
                }

                sql.Append(@"
UPDATE bx_UserMedals SET [EndDate] = T.EndDate,[Url] = T.Url FROM @UserIDsTable T WHERE UserID = T.UID AND MedalID = @MedalID " + condition + @";");

                query.CommandText = sql.ToString();
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@MedalID", medalID, SqlDbType.Int);

                query.ExecuteNonQuery();

                return true;
            }
        }


        public override bool UpdateUserMedals(int userID, Dictionary<int, DateTime> endDates, Dictionary<int, string> urls)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DECLARE @MedalTable table(MID int, TEndDate datetime, TUrl nvarchar(200));");

                foreach (int mid in urls.Keys)
                {
                    sql.AppendFormat("INSERT INTO @MedalTable(MID,TEndDate,TUrl) VALUES (@MedalID_{0},@EndDate_{0},@Url_{0});", mid);
                    DateTime date;
                    if (endDates.TryGetValue(mid, out date))
                    {
                        query.CreateParameter<DateTime>("@EndDate_" + mid, date, SqlDbType.DateTime);
                    }
                    else
                        query.CreateParameter<DateTime?>("@EndDate_" + mid, null, SqlDbType.DateTime);

                    query.CreateParameter<int>("@MedalID_" + mid, mid, SqlDbType.Int);
                    query.CreateParameter<string>("@Url_" + mid, urls[mid], SqlDbType.NVarChar, 200);

                }

                sql.Append(@"
UPDATE bx_UserMedals SET EndDate = ISNULL(T.TEndDate,EndDate),[Url] = T.TUrl FROM @MedalTable T WHERE MedalID = T.MID AND UserID = @UserID;
");
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CommandText = sql.ToString();
                query.CommandType = CommandType.Text;

                query.ExecuteNonQuery();

                return true;
            }
        }



        //        [StoredProcedure(Name = "bx_DeleteExperiesMedals", Script = @"
        //CREATE PROCEDURE {name}
        //	 @MedalIDs       varchar(8000)
        //    ,@EndDate        datetime
        //AS BEGIN
        //
        //	SET NOCOUNT ON;
        //
        //    EXEC('DELETE [bx_UserMedals] WHERE MedalID not in('+@MedalIDs+')');  
        //    
        //    DELETE [bx_UserMedals] WHERE [EndDate] < @EndDate; 
        //
        //END")]
        //        public override void DeleteExperiesMedals(IEnumerable<int> medalIDs)
        //        {
        //            using (SqlQuery query = new SqlQuery())
        //            {
        //                query.CommandText = "bx_DeleteExperiesMedals";
        //                query.CommandType = CommandType.StoredProcedure;

        //                query.CreateParameter<string>("@MedalIDs", StringUtil.Join(medalIDs), SqlDbType.VarChar, 8000);
        //                query.CreateParameter<DateTime>("@EndDate", DateTimeUtil.Now, SqlDbType.DateTime);

        //                query.ExecuteNonQuery();

        //            }
        //        }

        public override bool DeleteMedalUsers(int medalID, IEnumerable<int> userIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE bx_UserMedals WHERE MedalID = @MedalID AND UserID in(@UserIDs);";
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@MedalID", medalID, SqlDbType.Int);
                query.CreateInParameter("@UserIDs", userIDs);

                query.ExecuteNonQuery();

                return true;
            }
        }

        public override bool DeleteUserMedals(int userID, IEnumerable<int> medalIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE bx_UserMedals WHERE UserID = @UserID AND MedalID in(@MedalIDs);";
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateInParameter("@MedalIDs", medalIDs);

                query.ExecuteNonQuery();

                return true;
            }
        }

        public override UserCollection GetMedalUsers(int medalID, int? medalLevelID, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;
            using (SqlQuery query = new SqlQuery())
            {

                Medal medal = AllSettings.Current.MedalSettings.Medals.GetValue(medalID);

                string condition;
                string medalCondition = "[MedalID] = " + medalID;
                if (medalLevelID != null)
                    medalCondition += " AND [MedalLevelID] = " + medalLevelID.Value;

                if (medal.IsCustom == false)
                {
                    if (medal == null)
                        return new UserCollection();

                    int index = 0;
                    MedalLevel level = null;
                    if (medalLevelID != null)
                    {
                        foreach (MedalLevel tempLevel in medal.Levels)
                        {
                            if (tempLevel.ID == medalLevelID.Value)
                            {
                                level = tempLevel;
                                break;
                            }
                            index++;
                        }
                        if (level == null)
                            return new UserCollection();
                    }

                    int minValue = 0;
                    int maxValue = int.MaxValue;

                    if (level == null)
                    {
                        minValue = medal.Levels[0].Value;
                    }
                    else
                    {
                        minValue = level.Value;
                        if (index != medal.Levels.Count - 1)
                        {
                            maxValue = medal.Levels[index + 1].Value;
                        }
                    }

                    string colum;
                    if (string.Compare(medal.Condition, "Point_0", true) == 0)
                    {
                        colum = "[Points]";
                    }
                    else
                        colum = "[" + medal.Condition.Replace("point", "Point") + "]";

                    condition = colum + " >= " + minValue + " AND " + colum + " < " + maxValue;



                    condition = "(" + condition + @" AND UserID NOT IN(SELECT [UserID] FROM bx_UserMedals WHERE MedalID = " + medalID + @" AND EndDate > getdate()))
OR UserID IN(SELECT [UserID] FROM bx_UserMedals WHERE " + medalCondition + @" AND EndDate > getdate())

";
                }
                else
                {
                    condition = @"
UserID IN(SELECT [UserID] FROM bx_UserMedals WHERE " + medalCondition + @" AND EndDate > getdate())
";
                }

                query.Pager.TableName = "bx_Users";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.PrimaryKey = "[UserID]";
                query.Pager.SortField = "[CreateDate]";
                query.Pager.IsDesc = true;
                query.Pager.Condition = condition;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserCollection users = new UserCollection(reader);
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                    return users;
                }
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_UpdateUsersDatas", Script = @"
CREATE PROCEDURE {name}
    @StartUserID            int,
    @UpdateCount            int,
    @UpdatePostCount        bit,
    @UpdateBlogCount        bit,
    @UpdateInviteCount      bit,
    @UpdateCommentCount     bit,
    @UpdatePictureCount     bit,
    @UpdateShareCount       bit,
    @UpdateDoingCount       bit,
    @UpdateDiskFileCount    bit
AS BEGIN
    SET NOCOUNT ON;
    
    EXEC('
    DECLARE @UserDatas table(TempID int identity(1,1),UID int,TopicCount int,PostCount int, BlogCount int,InviteCount int,
    CommentCount int,ShareCount int,CollectionCount int,AlbumCount int,PhotoCount int,DoingCount int,DiskFileCount int);

    INSERT INTO @UserDatas(UID)
    SELECT TOP '+ @UpdateCount + ' UserID FROM bx_Users WHERE UserID>='+@StartUserID+' AND [IsActive]=1;
    IF @@RowCount = 0
        SELECT -1;    

    IF '+ @UpdatePostCount + '  = 1 BEGIN
        UPDATE @UserDatas SET TopicCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,PostUserID FROM bx_Threads as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.PostUserID = U.UID WHERE ThreadStatus < 4  Group by PostUserID
        ) AS T RIGHT JOIN @UserDatas AS U ON UID=T.PostUserID;

        UPDATE @UserDatas SET PostCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_Posts as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  WHERE SortOrder<4000000000000000 Group by UserID
        ) AS T RIGHT JOIN @UserDatas AS U ON UID=T.UserID;
    END

    IF '+ @UpdateBlogCount + '  = 1
        UPDATE @UserDatas SET BlogCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_BlogArticles as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  WHERE IsApproved=1 Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

    IF '+ @UpdateInviteCount + '  = 1
        UPDATE @UserDatas SET InviteCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,InviterID FROM bx_UserInfos as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.InviterID = U.UID  Group by InviterID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.InviterID;

    IF '+ @UpdateCommentCount + '  = 1
        UPDATE @UserDatas SET CommentCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_Comments as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  WHERE IsApproved=1 Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

    IF '+ @UpdateShareCount + '  = 1 BEGIN
        UPDATE @UserDatas SET CollectionCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_UserShares as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID WHERE PrivacyType=2 Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

        UPDATE @UserDatas SET ShareCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_UserShares as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID WHERE PrivacyType<>2 Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;
    END

    IF '+ @UpdatePictureCount + '  = 1 BEGIN
        UPDATE @UserDatas SET AlbumCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_Albums as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

        UPDATE @UserDatas SET PhotoCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_Photos as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;
    END

    IF '+ @UpdateDoingCount + '  = 1
        UPDATE @UserDatas SET DoingCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_Doings as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

    IF '+ @UpdateDiskFileCount + '  = 1
        UPDATE @UserDatas SET DiskFileCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_DiskFiles as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

    UPDATE bx_Users
    SET TotalInvite = ISNULL(InviteCount,TotalInvite)
        ,TotalTopics = ISNULL(TopicCount,TotalTopics)
        ,TotalPosts = ISNULL(PostCount,TotalPosts)
        ,TotalComments = ISNULL(CommentCount,TotalComments)
        ,TotalShares = ISNULL(ShareCount,TotalShares)
        ,TotalCollections = ISNULL(CollectionCount,TotalCollections)
        ,TotalBlogArticles = ISNULL(BlogCount,TotalBlogArticles)
        ,TotalAlbums = ISNULL(AlbumCount,TotalAlbums)
        ,TotalPhotos = ISNULL(PhotoCount,TotalPhotos)
        ,TotalDoings = ISNULL(DoingCount,TotalDoings)
    FROM @UserDatas
    WHERE UserID = UID;

    UPDATE bx_UserVars
    SET TotalDiskFiles = ISNULL(DiskFileCount,TotalDiskFiles)
    FROM @UserDatas
    WHERE UserID = UID;


    SELECT Max(UID)+1 FROM @UserDatas;
    ');
END
")]
        #endregion
        public override int UpdateUsersDatas(int startUserID, int updateCount, bool updatePostCount, bool updateBlogCount, bool updateInviteCount, bool updateCommentCount, bool updatePictureCount, bool updateShareCount, bool updateDoingCount, bool updateDiskFileCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateUsersDatas";
                query.CommandType = CommandType.StoredProcedure;
                query.CommandTimeout = 3 * 60;

                query.CreateParameter<int>("@StartUserID", startUserID, SqlDbType.Int);
                query.CreateParameter<int>("@UpdateCount", updateCount, SqlDbType.Int);

                query.CreateParameter<bool>("@UpdatePostCount", updatePostCount, SqlDbType.Int);
                query.CreateParameter<bool>("@UpdateBlogCount", updateBlogCount, SqlDbType.Int);
                query.CreateParameter<bool>("@UpdateInviteCount", updateInviteCount, SqlDbType.Int);
                query.CreateParameter<bool>("@UpdateCommentCount", updateCommentCount, SqlDbType.Int);
                query.CreateParameter<bool>("@UpdatePictureCount", updatePictureCount, SqlDbType.Int);
                query.CreateParameter<bool>("@UpdateShareCount", updateShareCount, SqlDbType.Int);
                query.CreateParameter<bool>("@UpdateDoingCount", updateDoingCount, SqlDbType.Int);
                query.CreateParameter<bool>("@UpdateDiskFileCount", updateDiskFileCount, SqlDbType.Int);

                return query.ExecuteScalar<int>();
            }
        }


        /// <summary>
        /// 清理过期数据（过期用户组、过期版主、过期屏蔽用户）
        /// </summary>
        [StoredProcedure(Name = "bx_ClearExpiresUserData", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN

    SET NOCOUNT ON;

    DELETE FROM bx_UserRoles WHERE EndDate < GETDATE();

    DELETE FROM bx_UserMedals WHERE EndDate < GETDATE();

    DELETE FROM bx_Moderators WHERE EndDate < GETDATE();

    --清理过期的屏蔽用户
    DELETE FROM bx_BannedUsers WHERE EndDate < GETDATE();

    DELETE FROM bx_Serials WHERE ExpiresDate < GETDATE();
    
    --清理短信临时数据
    DELETE  FROM bx_SmsCodes WHERE CreateDate <= DATEADD(hour, -24, GETDATE());

    --清理管理员会话
    DELETE FROM bx_AdminSessions WHERE UpdateDate < DATEADD(minute, 0 - 60, GETDATE());

END
")]
        public override void ClearExpiresUserData()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_ClearExpiresUserData";
                query.CommandType = CommandType.StoredProcedure;
                query.CommandTimeout = 60;

                query.ExecuteNonQuery();
            }
        }


        public override RevertableCollection<User> GetUserWithReverters(IEnumerable<int> userIDs)
        {
            if (ValidateUtil.HasItems(userIDs) == false)
                return null;

            RevertableCollection<User> users = new RevertableCollection<User>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
	A.*,
	[SignatureReverter] = ISNULL(R.[SignatureReverter], '')
FROM 
	bx_Users A
LEFT JOIN 
	bx_UserReverters R WITH (NOLOCK) ON R.UserID = A.UserID
WHERE 
	A.UserID IN (@UserIDs)";

                query.CreateInParameter<int>("@UserIDs", userIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string signatureReverter = reader.Get<string>("SignatureReverter");

                        User user = new User(reader);

                        users.Add(user, signatureReverter);
                    }
                }
            }

            return users;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_User_UpdateUserKeywords", Script = @"
CREATE PROCEDURE {name}
    @UserID               int,
    @KeywordVersion       varchar(32),
    @Signature            nvarchar(1500),
    @SignatureReverter    nvarchar(4000)
AS BEGIN

/* include : Procedure_UpdateKeyword.sql
    {PrimaryKey} = UserID
    {PrimaryKeyParam} = @UserID

    {Table} = bx_Users
    {Text} = Signature
    {TextParam} = @Signature

    {RevertersTable} = bx_UserReverters
    {TextReverter} = SignatureReverter
    {TextReverterParam} = @SignatureReverter
    
*/

END")]
        #endregion
        public override void UpdateUserKeywords(RevertableCollection<User> processlist)
        {
            string procedure = "bx_User_UpdateUserKeywords";
            string table = "bx_Users";
            string primaryKey = "UserID";

            SqlDbType text_Type = SqlDbType.NVarChar; int text_Size = 1500;
            SqlDbType reve_Type = SqlDbType.NVarChar; int reve_Size = 4000;

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
                foreach (Revertable<User> item in processlist)
                {
                    //此项确实需要更新，且不只是版本发生了变化
                    if (item.NeedUpdate && item.OnlyVersionChanged == false)
                    {

                        sql.InnerBuilder.AppendFormat(@"EXEC {1} @ID_{0}, @KeywordVersion_{0}, @Text_{0}, @TextReverter_{0};", i, procedure);

                        query.CreateParameter<int>("@ID_" + i, item.Value.GetKey(), SqlDbType.Int);

                        //如果文字发生了变化，更新
                        if (item.TextChanged)
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, item.Value.KeywordVersion, SqlDbType.VarChar, 32);
                            query.CreateParameter<string>("@Text_" + i, item.Value.Text, text_Type, text_Size);
                        }
                        else
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, null, SqlDbType.VarChar, 32);
                            query.CreateParameter<string>("@Text_" + i, null, text_Type, text_Size);
                        }

                        //如果恢复信息发生了变化，更新
                        if (item.ReverterChanged)
                            query.CreateParameter<string>("@TextReverter_" + i, item.Reverter, reve_Type, reve_Size);
                        else
                            query.CreateParameter<string>("@TextReverter_" + i, null, reve_Type, reve_Size);

                        i++;

                    }

                }

                query.CommandText = sql.ToString();
                query.ExecuteNonQuery();
            }
        }

        public override string GetUnValidatedEmail(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT Data FROM bx_Serials WHERE UserID = @UserID AND ExpiresDate > GETDATE()";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.Get<string>(0);
                    }
                }

                return null;
            }
        }

        public override void RemoveRepeatedEmail(int userid, string email)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_Users SET [Email]='' WHERE [Email]=@Email AND [UserID]<>@UserID";
                query.CommandType = CommandType.Text;

                query.CreateParameter<string>("@Email", email, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@UserID", userid, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }



        public override void UpdateUserPhone(int userID, long phoneNum)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_Users SET MobilePhone=@MobilePhone WHERE UserID=@UserID";
                query.CreateParameter<long>("@MobilePhone", phoneNum, SqlDbType.BigInt);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }


        public override void FillUserPoints(User user)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT Points,Point_1,Point_2,Point_3,Point_4,Point_5,Point_6,Point_7,Point_8 FROM bx_Users WHERE UserID=@UserID";
                query.CreateParameter<int>("@UserID", user.UserID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.Points = reader.Get<int>("Points");
                        for (int i = 0; i < 8; i++)  //8个扩展积分，特殊处理
                        {
                            user.ExtendedPoints[i] = reader.Get<int>("Point_" + (i + 1));
                        }
                    }
                }
            }
        }


        public override UserCollection GetMostActiveUsers(ActiveUserType type, int count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string order;
                if (type == ActiveUserType.WeekOnlineTime)
                {
                    order = "WeekOnlineTime";
                }
                else if (type == ActiveUserType.MonthOnlineTime)
                    order = "MonthOnlineTime";
                else if (type == ActiveUserType.DayOnlineTime)
                    order = "DayOnlineTime";
                else if (type == ActiveUserType.MonthPosts)
                    order = "MonthPosts";
                else if (type == ActiveUserType.WeekPosts)
                    order = "WeekPosts";
                else
                    order = "DayPosts";

                if (type == ActiveUserType.WeekOnlineTime || type == ActiveUserType.DayOnlineTime || type == ActiveUserType.MonthOnlineTime)
                {
                    query.CommandText = "SELECT TOP(@TopCount) * FROM bx_Users WITH(NOLOCK) WHERE [IsActive] = 1 AND LastVisitDate>=@DateTime AND " + order + " > 0 ORDER BY " + order + " DESC";
                }
                else
                    query.CommandText = "SELECT TOP(@TopCount) * FROM bx_Users WITH(NOLOCK) WHERE [IsActive] = 1 AND LastPostDate>=@DateTime AND " + order + " > 0  ORDER BY " + order + " DESC";

                DateTime dateTime;
                if (type == ActiveUserType.WeekPosts || type == ActiveUserType.WeekOnlineTime)
                {
                    dateTime = DateTimeUtil.GetMonday();
                }
                else if (type == ActiveUserType.MonthOnlineTime || type == ActiveUserType.MonthPosts)
                {
                    dateTime = new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, 1);
                }
                else
                {
                    dateTime = new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, DateTimeUtil.Now.Day);
                }

                query.CreateParameter<DateTime>("@DateTime", dateTime, SqlDbType.DateTime);

                query.CommandType = CommandType.Text;

                query.CreateTopParameter("@TopCount", count);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new UserCollection(reader);
                }
            }
        }

        #region 找回密码日志
        public override void SetRecoverPasswordLogSuccess(string serial, string ip)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_RecoverPasswordLogs SET Successed = 1, IP = @IP WHERE Serial = @Serial;";
                query.CreateParameter<string>("@IP", ip, SqlDbType.VarChar, 150);
                query.CreateParameter<string>("@Serial",serial,SqlDbType.VarChar,100);

                query.ExecuteNonQuery();
            }
        }

        public override void CreateRecoverPasswordLog(int userID, string email, string serial,string ip)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "INSERT INTO bx_RecoverPasswordLogs( UserID, Email, Serial, IP) VALUES( @UserID, @Email, @Serial, @IP);";
                query.CreateParameter<int>("@UserID",userID,SqlDbType.Int);
                query.CreateParameter<string>("@Email", email, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@Serial", serial, SqlDbType.VarChar, 100);
                query.CreateParameter<string>("@IP", ip, SqlDbType.VarChar, 150);

                query.ExecuteNonQuery();
            }
        }

        public override RecoverPasswordLogCollection GetRecoverPasswordLogs(RecoverPasswordLogFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sbCondition = new StringBuilder();

                if (!string.IsNullOrEmpty(filter.Username))
                {
                    sbCondition.Append(" AND UserID IN(SELECT Username FROM bx_Users WHERE Username LIKE '%'+ @Username +'%')");
                    query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                }

                if (!string.IsNullOrEmpty(filter.Email))
                {
                    sbCondition.Append(" AND Email LIKE '%'+ @Email + '%'");
                    query.CreateParameter<string>("@Email", filter.Email, SqlDbType.NVarChar, 150);
                }

                if (!string.IsNullOrEmpty(filter.IP))
                {
                    sbCondition.Append(" AND IP LIKE '%'+ @IP + '%'");
                    query.CreateParameter<string>("@IP", filter.IP, SqlDbType.VarChar, 150);
                }

                if (filter.BeginDate != null)
                {
                    sbCondition.Append(" AND CreateDate > @BeginDate");
                    query.CreateParameter<DateTime >("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                }

                if (filter.EndDate != null)
                {
                    sbCondition.Append(" AND CreateDate < @EndDate");
                    query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                }

                if (sbCondition.Length > 0)
                {
                    sbCondition.Remove(0, 4);
                }

                query.Pager.PageSize = filter.PageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.TableName = "bx_RecoverPasswordLogs";
                query.Pager.PrimaryKey = "Id";
                query.Pager.SortField = "Id";
                query.Pager.IsDesc = true;
                query.Pager.Condition = sbCondition.ToString();
                query.Pager.SelectCount = true;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    RecoverPasswordLogCollection results = new RecoverPasswordLogCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Next)
                            results.TotalRecords = reader.Get<int>(0);
                    }

                    return results;
                }
            }
        }

        #endregion

        public override void ClearExperiesExtendField(IEnumerable<string> keys)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
DELETE bx_UserExtendedValues WHERE ExtendedFieldID NOT IN(@Keys);
";
                query.CreateInParameter<string>("@Keys", keys);
                query.CommandType = CommandType.Text;
                query.CommandTimeout = int.MaxValue;
                query.ExecuteNonQuery();
            }
        }

#if DEBUG

        [StoredProcedure(Name = "bx_UpdateUserGeneralPoint", FileName = "bx_UpdateUserGeneralPoint.sql")]
        public void t1()
        { }
#endif
    }
}