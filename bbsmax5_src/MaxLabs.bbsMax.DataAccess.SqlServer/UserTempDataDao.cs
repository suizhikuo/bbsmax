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

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using System.Data.SqlClient;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class UserTempDataDao:DataAccess.UserTempDataDao
    {

        public override void Delete(int userID, UserTempDataType dataType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM  bx_UserTempData WHERE UserID = @UserID AND DataType = @DataType";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<short>("@DataType", (short)dataType, SqlDbType.SmallInt);
                query.CommandType = CommandType.Text;
                query.ExecuteNonQuery();
            }
        }

        public override void Delete(UserTempDataType dataType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM  bx_UserTempData WHERE  DataType = @DataType";
                query.CreateParameter<short>("@DataType", (short)dataType, SqlDbType.SmallInt);
                query.CommandType = CommandType.Text;
                query.ExecuteNonQuery();
            }
        }

        public override void Delete(IEnumerable<int> userIds, UserTempDataType dataType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_UserTempData WHERE UserID IN (@UserIds) AND DataType = @DataType";
                query.CommandType = CommandType.Text;
                query.CreateParameter<short>("@DataType" ,(short)dataType , SqlDbType.SmallInt );
                query.CreateInParameter<int>("@UserIds" ,userIds);
                query.ExecuteNonQuery();
            }
        }

        public override void DeleteByType(UserTempDataType dataType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = " DELETE FROM  bx_UserTempData WHERE DataType = @DataType";
                query.CreateParameter<short>("@DataType", (short)dataType, SqlDbType.SmallInt);
                query.CommandType = CommandType.Text;
                query.ExecuteNonQuery();
            }
        }

        public override void DeleteUserDatas(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = " DELETE FROM  bx_UserTempData WHERE UserID = @UserID";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CommandType = CommandType.Text;
                query.ExecuteNonQuery();
            }
        }

        public override UserTempData GetTemporaryData(int userID, UserTempDataType dataType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_UserTempData WHERE UserID = @UserID AND DataType = @DataType";
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<short>("@DataType", (short)dataType, SqlDbType.SmallInt);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserTempData(reader);
                    }
                }

                return null;
            }
        }

        public override UserTempData GetTemporaryDataWithExpiresDate(int userID, UserTempDataType dataType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_UserTempData WHERE UserID = @UserID AND DataType = @DataType AND ExpiresDate>=GetDate()";
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<short>("@DataType", (short)dataType, SqlDbType.SmallInt);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserTempData(reader);
                    }
                }

                return null;
            }
        }

        public override void SaveData(int userID, UserTempDataType dataType, object data, bool overrideOldData)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SaveUserTemporaryData";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<short>("@DataType", (short)dataType, SqlDbType.SmallInt);
                query.CreateParameter<string>("@Data", data.ToString(), SqlDbType.NText);
                query.CreateParameter<bool>("@OverrideOld", overrideOldData, SqlDbType.Bit);
                query.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 存储新数据， 默认覆盖原来的数据
        /// </summary>
        /// <param name="data"></param>
        public override void SaveData(UserTempData data)
        {
            SaveData(data, true);
        }

        /// <summary>
        /// 存储数据， 是否覆盖原来的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="overrideOldData">是否覆盖原来的数据</param>
        #region 存储过程
        [StoredProcedure( Name="bx_SaveUserTemporaryData", Script= @"
CREATE PROCEDURE {name}
 @Data            ntext
,@UserID          int
,@DataType        smallint
,@OverrideOld     bit
AS

BEGIN
    SET NOCOUNT ON;

    IF @OverrideOld = 1
        DELETE FROM bx_UserTempData WHERE DataType = @DataType AND UserID = @UserID;
    INSERT INTO bx_UserTempData( UserID , DataType , Data) VALUES( @UserID,@DataType , @Data);
END

")]
        #endregion
        public override void SaveData(UserTempData data, bool overrideOldData)
        {
            SaveData(data.UserID,data.DataType,data.Data,overrideOldData);
        }

        /// <summary>
        /// 删除到期数据
        /// </summary>
        public override void DeleteExpiresData()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_UserTempData WHERE ExpiresDate <= GETDATE()";
                query.CommandType = CommandType.Text;
                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_SaveBindOrUnbindSmsCode", Script = @"
CREATE PROCEDURE {name}
 @UserID                int
,@Action                tinyint
,@MobilePhone           bigint
,@ChangedMobilePhone    bigint
,@SmsCode             varchar(10)  
,@ChangedSmsCode      varchar(10)
AS

BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TodayCount int;
    DECLARE @Today datetime;
    DECLARE @ExpiresDate datetime;

    IF (@Action=1 AND EXISTS(SELECT * FROM bx_Users WHERE MobilePhone=@MobilePhone)) OR
        (@Action=3 AND EXISTS(SELECT * FROM bx_Users WHERE MobilePhone=@MobilePhone))
        RETURN(2);

    SET @Today = CONVERT(varchar(12) , GETDATE(), 102);
    SET @ExpiresDate = DATEADD(hour, 24, GETDATE());

    SELECT @TodayCount = COUNT(*) FROM (SELECT TOP 3 SmsCodeID FROM [bx_SmsCodes] WHERE UserID = @UserID AND ActionType = @Action AND CreateDate >= @Today) as t;
    
    IF @TodayCount < 3 BEGIN
        INSERT INTO [bx_SmsCodes]( UserID , ActionType , ExpiresDate , MobilePhone, ChangedMobilePhone, SmsCode, ChangedSmsCode) VALUES( @UserID,@Action ,@ExpiresDate, @MobilePhone, @ChangedMobilePhone, @SmsCode, @ChangedSmsCode);
        RETURN (0);
    END
    ELSE
        RETURN (1);

END

")]
        public override int SaveBindOrUnbindSmsCode(int userID, MobilePhoneAction action, long mobilePhone, string smsCode, long changedMobilePhone, string changedSmsCode)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SaveBindOrUnbindSmsCode";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<byte>("@Action", (byte)action, SqlDbType.TinyInt);
                query.CreateParameter<long>("@MobilePhone", mobilePhone, SqlDbType.BigInt);
                query.CreateParameter<long>("@ChangedMobilePhone", changedMobilePhone, SqlDbType.BigInt);
                query.CreateParameter<string>("@SmsCode", smsCode, SqlDbType.VarChar, 10);
                query.CreateParameter<string>("@ChangedSmsCode", changedSmsCode, SqlDbType.VarChar, 10);

                SqlParameter returnParam = query.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (int)returnParam.Value;
            }
        }

        [StoredProcedure(Name = "bx_ChangePhoneBySmsCode", Script = @"
CREATE PROCEDURE {name}
@UserID             int,
@Action             tinyint,
@OldMobilePhone     bigint,
@NewMobilePhone     bigint,
@OldSmsCode         varchar(10),
@NewSmsCode         varchar(10),
@OldSuccess         bit out,
@NewSuccess         bit out

AS
BEGIN
    SET NOCOUNT ON;

    SET @OldSuccess=0;
    SET @NewSuccess=0;

    IF EXISTS (SELECT * FROM bx_SmsCodes
            WHERE UserID = @UserID
              AND ActionType = @Action
              AND ExpiresDate >= GETDATE()
              AND IsUsed = 0
              AND ChangedMobilePhone = @OldMobilePhone
              AND ChangedSmsCode = @OldSmsCode)
        SET @OldSuccess=1;

    IF EXISTS (SELECT * FROM bx_SmsCodes
            WHERE UserID = @UserID
              AND ActionType = @Action
              AND ExpiresDate >= GETDATE()
              AND IsUsed = 0
              AND MobilePhone = @NewMobilePhone
              AND SmsCode = @NewSmsCode)
        SET @NewSuccess=1;

    IF(@OldSuccess = 1 AND @NewSuccess = 1) BEGIN
        UPDATE bx_Users SET MobilePhone = @NewMobilePhone WHERE UserID = @UserID;
        UPDATE bx_SmsCodes SET IsUsed = 1 WHERE UserID = @UserID AND IsUsed = 0;
        INSERT INTO bx_UserMobileOperationLogs(UserID, Username, MobilePhone, OperationType) SELECT UserID, Username, @NewMobilePhone, @Action FROM bx_Users WITH (NOLOCK) WHERE UserID = @UserID;
    END
END

")]
        public override void ChangePhoneBySmsCode(int userID, long newMobilePhone, string newSmsCode, long oldMobilePhone, string oldSmsCode, out bool oldSuccess, out bool newSuccess)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_ChangePhoneBySmsCode";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<byte>("@Action",(byte)MobilePhoneAction.Change,SqlDbType.TinyInt);
                query.CreateParameter<long>("@OldMobilePhone",oldMobilePhone,SqlDbType.BigInt);
                query.CreateParameter<long>("@NewMobilePhone", newMobilePhone, SqlDbType.BigInt);
                query.CreateParameter<string>("@OldSmsCode",oldSmsCode,SqlDbType.VarChar,10);
                query.CreateParameter<string>("@NewSmsCode",newSmsCode,SqlDbType.VarChar,10);
                
                SqlParameter oldSuccessParam = query.CreateParameter<bool>("@OldSuccess", SqlDbType.Bit, ParameterDirection.Output);
                SqlParameter newSuccessParam = query.CreateParameter<bool>("@NewSuccess", SqlDbType.Bit, ParameterDirection.Output);

                query.ExecuteNonQuery();

                oldSuccess = (bool)oldSuccessParam.Value;
                newSuccess = (bool)newSuccessParam.Value;
            }
        }


        [StoredProcedure(Name = "bx_SetPhoneBySmsCode", Script = @"
CREATE PROCEDURE {name}
@UserID             int,
@Action             tinyint,
@MobilePhone        bigint,
@SmsCode            varchar(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
            SELECT * FROM [bx_SmsCodes]
                WHERE UserID = @UserID
                  AND IsUsed = 0
                  AND ActionType = @Action
                  AND MobilePhone = @MobilePhone
                  AND SmsCode = @SmsCode
                  
                  AND ExpiresDate >= GETDATE()
            ) BEGIN

        IF @Action = 2
            UPDATE bx_Users SET MobilePhone = 0 WHERE UserID = @UserID;
        ELSE IF @Action = 1
            UPDATE bx_Users SET MobilePhone = @MobilePhone WHERE UserID = @UserID; 

        UPDATE bx_SmsCodes SET IsUsed = 1 WHERE UserID = @UserID AND IsUsed = 0;
        INSERT INTO bx_UserMobileOperationLogs(UserID,Username,MobilePhone,OperationType) SELECT UserID,Username,@MobilePhone,@Action FROM bx_Users WHERE UserID = @UserID;

        RETURN (1);
    END
    ELSE
        RETURN (0);
END
")]
        public override void SetPhoneBySmsCode(int userID, long mobilePhone, string smsCode, MobilePhoneAction action, out bool success)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetPhoneBySmsCode";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<byte>("@Action", (byte)action, SqlDbType.TinyInt);
                query.CreateParameter<long>("@MobilePhone", mobilePhone, SqlDbType.BigInt);
                query.CreateParameter<string>("@SmsCode", smsCode, SqlDbType.VarChar, 10);
                
                SqlParameter returnParam = query.CreateParameter<bool>("@Success", SqlDbType.Bit, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                success = ((int)returnParam.Value) == 1;
            }
        }
    }
}