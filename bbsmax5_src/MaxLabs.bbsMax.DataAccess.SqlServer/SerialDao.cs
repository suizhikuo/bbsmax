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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using System.Data;
using System.Data.SqlClient;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class SerialDao : DataAccess.SerialDao
    {

        [StoredProcedure(Name = "bx_CreateSerial", Script = @"
CREATE PROCEDURE {name}
@UserID         int
,@Type          tinyint
,@ExpiresDate   datetime
,@Data          nvarchar(1000)
,@Success		bit out
AS
BEGIN
    SET NOCOUNT ON;	 
	DECLARE @TodayCount int;
	DECLARE @Today datetime;
    DECLARE @Serial uniqueidentifier;

	SET @Success=1;
    SET @Today = CONVERT(varchar(12) , GETDATE(), 102);
    SET @Serial = NEWID(); 
    SELECT @TodayCount = COUNT(*) FROM (SELECT TOP 3 Serial FROM bx_Serials WHERE Type= @Type AND UserID = @UserID AND CreateDate >= @Today) as t;
	IF(@TodayCount < 3) BEGIN
		INSERT INTO bx_Serials( Serial, UserID, CreateDate, ExpiresDate, Type, Data) Values( @Serial, @UserID, GETDATE(), @ExpiresDate, @Type, @Data);
		SELECT * FROM bx_Serials WHERE Serial = @Serial;
	END
	ELSE
		SET @Success=0;
END
")]
        public override MaxSerial CreateSerial(int ownerUserId, DateTime expriseDate, SerialType type, string data,out bool success)
        {
            using (SqlQuery query = new SqlQuery())
            {
                Guid serial = Guid.NewGuid();

                DateTime createDate = DateTimeUtil.Now;

                query.CommandText = "bx_CreateSerial";
                query.CommandType = System.Data.CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", ownerUserId, SqlDbType.Int);
                query.CreateParameter<DateTime>("@ExpiresDate", expriseDate, SqlDbType.DateTime);
                query.CreateParameter<byte>("@Type", (byte)type, SqlDbType.TinyInt);
                query.CreateParameter<string>("@Data", data, SqlDbType.NVarChar, 1000);

                SqlParameter outputParam = query.CreateParameter<bool>("@Success", SqlDbType.Bit, ParameterDirection.Output);

                MaxSerial newSerial = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Next)
                    {
                        newSerial = new MaxSerial(reader);
                    }
                }

                success = (bool)outputParam.Value;
                return newSerial;
            }
        }

        [StoredProcedure(Name = "bx_GetSerial", Script = @"
CREATE PROCEDURE {name}
@Serial uniqueidentifier
,@Type tinyint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM bx_Serials Where Serial = @Serial AND ExpiresDate >= GETDATE();
END
")]
        public override MaxSerial GetSerial(Guid serial, SerialType serialType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = System.Data.CommandType.StoredProcedure;
                query.CommandText = "bx_GetSerial";
                query.CreateParameter<Guid>("@Serial", serial, System.Data.SqlDbType.UniqueIdentifier);
                query.CreateParameter<byte>("@Type", (byte)serialType, System.Data.SqlDbType.TinyInt);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    MaxSerial resault = null;
                    while (reader.Next)
                    {
                        resault = new MaxSerial(reader);
                    }
                    return resault;
                }
            }
        }

//        [StoredProcedure(Name = "bx_DeleteSerial", Script = @"
//CREATE PROCEDURE {name}
//@Serial uniqueidentifier
//AS
//BEGIN
//    SET NOCOUNT ON;
//    DELETE bx_Serials WHERE Serial = @Serial;
//END            
//")]
//        public override bool DeleteSerial(Guid serialGuid)
//        {
//            using (SqlQuery query = new SqlQuery())
//            {
//                query.CommandText = "DELETE FROM bx_Serials WHERE Serial = @Serial";
//                query.CommandType = System.Data.CommandType.StoredProcedure;
//                query.CreateParameter<Guid>("@Serial", serialGuid, System.Data.SqlDbType.UniqueIdentifier);
//                return query.ExecuteNonQuery() > 0;
//            }
//        }

        [StoredProcedure(Name = "bx_DeleteSerialByUser", Script = @"
CREATE PROCEDURE {name}
@UserID int
,@Type tinyint
AS
BEGIN
    SET NOCOUNT ON;
    DELETE bx_Serials WHERE UserID = @UserID AND Type = @Type;
END            
")]
        public override bool DeleteSerial(int userID, SerialType serialType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeleteSerialByUser";
                query.CommandType = System.Data.CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, System.Data.SqlDbType.Int);
                query.CreateParameter<byte>("@Type", (byte)serialType, System.Data.SqlDbType.TinyInt);
                return query.ExecuteNonQuery() > 0;
            }
        }
    }
}