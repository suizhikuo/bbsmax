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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class PayDao : DataAccess.PayDao
    {
        public override UserPay GetUserPay(string orderNo)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.Text;
                query.CommandText = "SELECT * FROM bx_Pay WHERE OrderNo=@OrderNo";
                query.CreateParameter<string>("@OrderNo", orderNo, SqlDbType.NVarChar, 200);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserPay usertype = null;
                    while (reader.Next)
                    {
                        usertype = new UserPay(reader);
                    }
                    return usertype;
                }
            }
        }

        public override UserPayCollection GetUserPays(int userID, PaylogFilter filter, int pageSize, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder condition = new StringBuilder(" AND UserID = @UserID");
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                
                if (filter != null)
                {
                    if (filter.State < 2)
                    {
                        condition.Append(" AND State=@State");
                        query.CreateParameter<bool>("@State", (filter.State == 1), SqlDbType.Bit);
                    }
                    if (filter.BeginDate != null)
                    {
                        condition.Append(" AND PayDate>= @BeginDate");
                        query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                    }

                    if (filter.EndDate != null)
                    {
                        condition.Append(" AND PayDate<= @EndDate");
                        query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                    }

                }
                if (condition.Length > 5)
                    condition.Remove(0, 5);

                query.Pager.TableName = "bx_Pay";
                query.Pager.PageSize = pageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PrimaryKey = "PayID";
                query.Pager.SelectCount = true;
                query.Pager.SortField = "PayID";
                query.Pager.Condition = condition.ToString();
                query.Pager.IsDesc = true;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserPayCollection result = new UserPayCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Next)
                            result.TotalRecords = reader.Get<int>(0);
                    }

                    return result;
                }
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_CreatePayItem", Script = @"
CREATE PROCEDURE {name}
 @UserID      int
,@OrderNo     varchar(50)
,@OrderAmount decimal(18,2)
,@Payment     tinyint
,@PayType     tinyint
,@PayValue    int
,@SubmitIp    varchar(50)
,@Remarks    nvarchar(50)
AS
BEGIN

SET NOCOUNT ON;
    
    IF NOT EXISTS( SELECT * FROM bx_Pay WHERE UserID = @UserID AND OrderNo = @OrderNo) BEGIN
        INSERT INTO bx_Pay(UserID,OrderNo,OrderAmount,Payment,PayType,PayValue,SubmitIp,Remarks) VALUES(@UserID,@OrderNo,@OrderAmount,@Payment,@PayType,@PayValue,@SubmitIp,@Remarks);
        RETURN (1);
    END
    ELSE
        RETURN (0);
END
")]
        #endregion
        public override bool CreateUserPayItem(int userID, string orderNo, decimal orderAmount, byte payment, byte payType, int payValue, string submitIp, string remarks)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_CreatePayItem";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@OrderNo", orderNo, SqlDbType.VarChar, 50);
                query.CreateParameter<decimal>("@OrderAmount", orderAmount, SqlDbType.Decimal);
                query.CreateParameter<Int16>("@Payment",(Int16)payment, SqlDbType.TinyInt);
                query.CreateParameter<Int16>("@PayType",(Int16)payType, SqlDbType.TinyInt);
                query.CreateParameter<int>("@PayValue", payValue, SqlDbType.Int);
                query.CreateParameter<string>("@SubmitIp", submitIp, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@Remarks", remarks, SqlDbType.NVarChar, 200);
                SqlParameter returnParam = query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);
                query.ExecuteNonQuery();
                return (int)returnParam.Value > 0;
            }
        }

        public override UserPayCollection AdminSearchUserPays(PaylogFilter filter, int pageSize, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string SordField = "PayID";

                StringBuilder condition = new StringBuilder();

                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.OrderNo))
                    {
                        condition.Append(" AND OrderNo LIKE '%' + @OrderNo +'%'");
                        query.CreateParameter<string>("@OrderNo", filter.OrderNo, SqlDbType.NVarChar,200);
                    }
                    if (!string.IsNullOrEmpty(filter.TransactionNo))
                    {
                        condition.Append(" AND TransactionNo LIKE '%' + @TransactionNo +'%'");
                        query.CreateParameter<string>("@TransactionNo", filter.TransactionNo, SqlDbType.NVarChar, 200);
                    }
                    if (filter.Payment > 0)
                    {
                        condition.Append(" AND Payment = @Payment");
                        query.CreateParameter<byte>("@Payment", filter.Payment, SqlDbType.TinyInt);
                    }
                    if (filter.BeginAmount > 0 && filter.EndAmount > 0)
                    {
                        condition.Append(" AND OrderAmount >= @BeginAmount AND OrderAmount <= @EndAmount");
                        query.CreateParameter<decimal>("@BeginAmount", filter.BeginAmount, SqlDbType.Decimal);
                        query.CreateParameter<decimal>("@EndAmount", filter.EndAmount, SqlDbType.Decimal);
                    }
                    if (filter.PayType > 0)
                    {
                        condition.Append(" AND PayType LIKE '%' + @PayType + '%'");
                        query.CreateParameter<byte>("@PayType", filter.PayType, SqlDbType.TinyInt);
                    }
                    if (filter.BeginValue>0 && filter.EndValue>0)
                    {
                        condition.Append(" AND PayValue >= @BeginValue  AND PayValue <= @EndValue");
                        query.CreateParameter<int>("@BeginValue", filter.BeginValue, SqlDbType.Int);
                        query.CreateParameter<int>("@EndValue", filter.EndValue, SqlDbType.Int);
                    }
                    if (filter.State < 2)
                    {
                        condition.Append(" AND State = @State");
                        query.CreateParameter<bool>("@State", (filter.State==1), SqlDbType.Bit);
                    }
                    if (filter.BeginDate != null)
                    {
                        condition.Append(" AND PayDate >= @BeginDate");
                        query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                    }

                    if (filter.EndDate != null)
                    {
                        condition.Append(" AND PayDate <= @EndDate");
                        query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                    }

                    if (!string.IsNullOrEmpty(filter.Username))
                    {
                        condition.Append(" AND UserID IN( SELECT UserID FROM bx_Users WHERE Username LIKE '%' + @Username +'%')");
                        query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                    }

                }
                if (condition.Length > 5)
                    condition.Remove(0, 5);
                query.Pager.TableName = "bx_Pay";
                query.Pager.PageSize = pageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PrimaryKey = "PayID";
                query.Pager.SortField = SordField;
                query.Pager.Condition = condition.ToString();
                query.Pager.SelectCount = true;
                query.Pager.IsDesc = true;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserPayCollection usertypelist = new UserPayCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Next)
                            usertypelist.TotalRecords = reader.Get<int>(0);
                    }

                    return usertypelist;
                }
            }
        }
        
        #region 存储过程
        [StoredProcedure(Name = "bx_UpdatePayItem", Script = @"
CREATE PROCEDURE {name}
 @BuyerEmail      nvarchar(50)
,@OrderNo         varchar(50)
,@TransactionNo   nvarchar(200)
,@PayIp           varchar(50)
,@PayDate         datetime
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @UserID int;
    DECLARE @PayType tinyint;
    DECLARE @PayValue int;
    DECLARE @State bit;
    SELECT @UserID = UserID, @PayType = PayType, @PayValue=PayValue, @State = [State]  FROM bx_Pay WHERE OrderNo = @OrderNo;
    
    IF @State = 0 BEGIN
        UPDATE bx_Pay SET BuyerEmail=@BuyerEmail,TransactionNo = @TransactionNo,PayIp=@PayIp,PayDate=@PayDate,State=1 WHERE OrderNo = @OrderNo;


        DECLARE @P0 int,@P1 int, @P2 int, @P3 int, @P4 int, @P5 int, @P6 int, @P7 int;

        SET @P0=0;
        SET @P1=0;
        SET @P2=0;
        SET @P3=0;
        SET @P4=0;
        SET @P5=0;
        SET @P6=0;
        SET @P7=0;

        IF @PayType = 0 BEGIN
            UPDATE bx_Users SET Point_1 = Point_1 + @PayValue WHERE UserID = @UserID;
            SET @P0 = @PayValue;
        END
        ELSE IF @PayType = 1 BEGIN
            UPDATE bx_Users SET Point_2 = Point_2 + @PayValue WHERE UserID = @UserID;
            SET @P1 = @PayValue;     
        END
        ELSE IF @PayType = 2 BEGIN
            UPDATE bx_Users SET Point_3 = Point_3 + @PayValue WHERE UserID = @UserID;
            SET @P2 = @PayValue;     
        END
        ELSE IF @PayType = 3 BEGIN
            UPDATE bx_Users SET Point_4 = Point_4 + @PayValue WHERE UserID = @UserID;
            SET @P3 = @PayValue;     
        END
        ELSE IF @PayType = 4 BEGIN
            UPDATE bx_Users SET Point_5 = Point_5 + @PayValue WHERE UserID = @UserID;
            SET @P4 = @PayValue;     
        END
        ELSE IF @PayType = 5 BEGIN
            UPDATE bx_Users SET Point_6 = Point_6 + @PayValue WHERE UserID = @UserID;
            SET @P5 = @PayValue;     
        END
        ELSE IF @PayType = 6 BEGIN
            UPDATE bx_Users SET Point_7 = Point_7 + @PayValue WHERE UserID = @UserID;
            SET @P6 = @PayValue;   
        END  
        ELSE IF @PayType = 7 BEGIN
            UPDATE bx_Users SET Point_8 = Point_8 + @PayValue WHERE UserID = @UserID;
            SET @P7 = @PayValue;     
        END

        DECLARE @Cp0 int,@Cp1 int, @Cp2 int, @Cp3 int, @Cp4 int, @Cp5 int, @Cp6 int, @Cp7 int;


        SELECT @Cp0 = Point_1,@Cp1 = Point_2,@Cp2 = Point_3,@Cp3 = Point_4,@Cp4 = Point_5,
                @Cp5 = Point_6,@Cp6 = Point_7,@Cp7 = Point_8 FROM bx_Users WHERE UserID = @UserID;

        SELECT @UserID;

DECLARE @Remarks nvarchar(200);
SET @Remarks = N'订单号' + @OrderNo;
        EXEC bx_CreatePointLogs @UserID
        ,@P0
        ,@P1
        ,@P2
        ,@P3
        ,@P4
        ,@P5
        ,@P6
        ,@P7
        ,@Cp0
        ,@Cp1
        ,@Cp2
        ,@Cp3
        ,@Cp4
        ,@Cp5
        ,@Cp6
        ,@Cp7
        ,N'积分充值'
        ,@Remarks

    RETURN (1);

    END

    RETURN(0)
END
")]
        #endregion
        public override bool UpdateUserPayItem(string buyerEmail, string orderNo, string transactionNo, string payIp,DateTime payDate,out int userID)
        {
            userID = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_UpdatePayItem";
                query.CreateParameter<string>("@BuyerEmail", buyerEmail, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@OrderNo", orderNo, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@TransactionNo", transactionNo, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@PayIp", payIp, SqlDbType.VarChar, 50);
                query.CreateParameter<DateTime>("@PayDate", payDate, SqlDbType.DateTime);
                SqlParameter returnParam = query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userID = reader.Get<int>(0);
                    }
                }

                return (int)returnParam.Value > 0;
            }
        }
    }
}