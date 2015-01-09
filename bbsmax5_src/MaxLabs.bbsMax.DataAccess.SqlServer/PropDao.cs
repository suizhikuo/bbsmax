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
using System.Data.SqlClient;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class PropDao : DataAccess.PropDao
    {
        [StoredProcedure(Name="bx_Prop_AddPropLog", Script=@"
CREATE PROCEDURE {name}
  @UserID int
 ,@Type   tinyint
 ,@Log    ntext
AS
BEGIN
    SET NOCOUNT ON;
  INSERT INTO bx_PropLogs (UserID, Type, Log) VALUES (@UserID, @Type, @Log);
END")]
        public override void AddPropLog(int userID, PropLogType logType, string log)
        {
            using(SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Prop_AddPropLog";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<PropLogType>("@Type", logType, SqlDbType.TinyInt);
                query.CreateParameter<string>("@Log", log, SqlDbType.NText);

                query.ExecuteNonQuery();
            }
        }

        public override void DeletePropLogs(JobDataClearMode clearMode, DateTime dateTime, int saveRows)
        {
            using (SqlQuery query = new SqlQuery())
            {
                switch(clearMode)
                {
                    case JobDataClearMode.ClearByDay:
                        query.CommandText = "DELETE FROM bx_PropLogs WHERE CreateDate <= @Time;";

                        query.CreateParameter<DateTime>("@Time", dateTime, SqlDbType.DateTime);
                        break;

                    case JobDataClearMode.ClearByRows:
                        query.CommandText = "DELETE FROM bx_PropLogs WHERE PropLogID < (SELECT MIN(O.PropLogID) FROM (SELECT TOP(@TopCount) PropLogID FROM bx_PropLogs ORDER BY PropLogID DESC) AS O)";
                        query.CreateTopParameter("@TopCount", saveRows);
                        break;

                    case JobDataClearMode.CombinMode:
                        query.CommandText = "DELETE FROM bx_PropLogs WHERE PropLogID < (SELECT MIN(O.PropLogID) FROM (SELECT TOP(@TopCount) PropLogID FROM bx_PropLogs ORDER BY PropLogID DESC) AS O) AND CreateDate >= @Time";
                        query.CreateTopParameter("@TopCount", saveRows);

                        query.CreateParameter<DateTime>("@Time", dateTime, SqlDbType.DateTime);
                        break;
                }

                query.ExecuteNonQuery();
            }
        }

        public override PropLogCollection GetPropLogs(int userID, PropLogType type, int pageNumber, int pageSize)
        {
            using(SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_PropLogs";
                query.Pager.SortField = "PropLogID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;

                query.Pager.Condition = "UserID = @UserID";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                
                if(type != PropLogType.All)
                {
                    query.CreateParameter<PropLogType>("@Type", type, SqlDbType.TinyInt);
                    query.Pager.Condition += " AND Type = @Type";
                }

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PropLogCollection result = new PropLogCollection(reader);

                    if(reader.NextResult() && reader.Read())
                        result.TotalRecords = reader.Get<int>(0);

                    return result;
                }
            }
        }

        [StoredProcedure(Name="bx_Prop_Replenish", Script= @"
CREATE PROCEDURE {name}
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE bx_Props 
    SET 
        TotalNumber = TotalNumber + ReplenishNumber * DATEDIFF(hh, LastReplenishTime, GETDATE()), 
        LastReplenishTime = GETDATE() 
    WHERE 
        AutoReplenish = 1 AND TotalNumber <= ReplenishLimit AND LastReplenishTime <= DATEADD(hh, 0 - ReplenishTimeSpan, GETDATE());
END")]
        public override void ReplenishProps()
        {
            using(SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Prop_Replenish";
                query.CommandType = CommandType.StoredProcedure;

                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name="bx_Prop_CreateProp", Script= @"
CREATE PROCEDURE {name}
 @Icon nvarchar(255)
 ,@Name nvarchar(100)
 ,@Price int
 ,@PriceType int
 ,@PropType nvarchar(512)
 ,@PropParam ntext
 ,@Description nvarchar(255)
 ,@PackageSize int
 ,@TotalNumber int
 ,@AllowExchange bit
 ,@AutoReplenish bit
 ,@ReplenishNumber int
 ,@ReplenishTimeSpan int
 ,@BuyCondition ntext
 ,@ReplenishLimit int
 ,@SortOrder int
AS
BEGIN
  SET NOCOUNT ON;
  INSERT INTO bx_Props (
    [Icon]
   ,[Name]
   ,[Price]
   ,[PriceType]
   ,[PropType]
   ,[PropParam]
   ,[Description]
   ,[PackageSize]
   ,[TotalNumber]
   ,[AllowExchange]
   ,[AutoReplenish]
   ,[ReplenishNumber]
   ,[ReplenishTimeSpan]
   ,[BuyCondition]
   ,[ReplenishLimit]
   ,[SortOrder]
  ) VALUES (
    @Icon
   ,@Name
   ,@Price
   ,@PriceType
   ,@PropType
   ,@PropParam
   ,@Description
   ,@PackageSize
   ,@TotalNumber
   ,@AllowExchange
   ,@AutoReplenish
   ,@ReplenishNumber
   ,@ReplenishTimeSpan
   ,@BuyCondition
   ,@ReplenishLimit
   ,@SortOrder
  );

END")]
        public override void CreateProp(string icon, string name, int price, int priceType, string propType, string propParam, string description, int packageSize, int totalNumber, bool allowExchange, bool autoReplenish, int replenishNumber, int replenishTimeSpan, int replenishLimit, string buyCondition, int sortOrder)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_CreateProp";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<string>("@Icon", icon, SqlDbType.NVarChar, 255);
                db.CreateParameter<string>("@Name", name, SqlDbType.NVarChar, 100);
                db.CreateParameter<int>("@Price", price, SqlDbType.Int);
                db.CreateParameter<int>("@PriceType", priceType, SqlDbType.Int);
                db.CreateParameter<string>("@PropType", propType, SqlDbType.NVarChar, 512);
                db.CreateParameter<string>("@PropParam", propParam, SqlDbType.NText);
                db.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 255);
                db.CreateParameter<int>("@PackageSize", packageSize, SqlDbType.Int);
                db.CreateParameter<int>("@TotalNumber", totalNumber, SqlDbType.Int);
                db.CreateParameter<bool>("@AllowExchange", allowExchange, SqlDbType.Bit);
                db.CreateParameter<bool>("@AutoReplenish", autoReplenish, SqlDbType.Bit);
                db.CreateParameter<int>("@ReplenishNumber", replenishNumber, SqlDbType.Int);
                db.CreateParameter<int>("@ReplenishTimeSpan", replenishTimeSpan, SqlDbType.Int);
                db.CreateParameter<string>("@BuyCondition", buyCondition, SqlDbType.NText);
                db.CreateParameter<int>("@ReplenishLimit", replenishLimit, SqlDbType.Int);
                db.CreateParameter<int>("@SortOrder", sortOrder, SqlDbType.Int);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Prop_UpdateProp", Script = @"
CREATE PROCEDURE {name}
  @PropID int
 ,@Icon nvarchar(255)
 ,@Name nvarchar(100)
 ,@Price int
 ,@PriceType int
 ,@PropType nvarchar(512)
 ,@PropParam ntext
 ,@Description nvarchar(255)
 ,@PackageSize int
 ,@TotalNumber int
 ,@AllowExchange bit
 ,@AutoReplenish bit
 ,@ReplenishNumber int
 ,@ReplenishTimeSpan int
 ,@BuyCondition ntext
 ,@ReplenishLimit int
 ,@SortOrder int
AS
BEGIN
  SET NOCOUNT ON;
  UPDATE bx_Props SET
    [Icon] = @Icon
   ,[Name] = @Name
   ,[Price] = @Price
   ,[PriceType] = @PriceType
   ,[PropType] = @PropType
   ,[PropParam] = @PropParam
   ,[Description] = @Description
   ,[PackageSize] = @PackageSize
   ,[TotalNumber] = @TotalNumber
   ,[AllowExchange] = @AllowExchange
   ,[AutoReplenish] = @AutoReplenish
   ,[ReplenishNumber] = @ReplenishNumber
   ,[ReplenishTimeSpan] = @ReplenishTimeSpan
   ,[BuyCondition] = @BuyCondition
   ,[ReplenishLimit] = @ReplenishLimit
   ,[SortOrder] = @SortOrder
  WHERE
   [PropID] = @PropID;

END")]
        public override void UpdateProp(int propID, string icon, string name, int price, int priceType, string propType, string propParam, string description, int packageSize, int totalNumber, bool allowExchange, bool autoReplenish, int replenishNumber, int replenishTimeSpan, int replenishLimit, string buyCondition, int sortOrder)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_UpdateProp";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@PropID", propID, SqlDbType.Int);
                db.CreateParameter<string>("@Icon", icon, SqlDbType.NVarChar, 255);
                db.CreateParameter<string>("@Name", name, SqlDbType.NVarChar, 100);
                db.CreateParameter<int>("@Price", price, SqlDbType.Int);
                db.CreateParameter<int>("@PriceType", priceType, SqlDbType.Int);
                db.CreateParameter<string>("@PropType", propType, SqlDbType.NVarChar, 512);
                db.CreateParameter<string>("@PropParam", propParam, SqlDbType.NText);
                db.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 255);
                db.CreateParameter<int>("@PackageSize", packageSize, SqlDbType.Int);
                db.CreateParameter<int>("@TotalNumber", totalNumber, SqlDbType.Int);
                db.CreateParameter<bool>("@AllowExchange", allowExchange, SqlDbType.Bit);
                db.CreateParameter<bool>("@AutoReplenish", autoReplenish, SqlDbType.Bit);
                db.CreateParameter<int>("@ReplenishNumber", replenishNumber, SqlDbType.Int);
                db.CreateParameter<int>("@ReplenishTimeSpan", replenishTimeSpan, SqlDbType.Int);
                db.CreateParameter<string>("@BuyCondition", buyCondition, SqlDbType.NText);
                db.CreateParameter<int>("@ReplenishLimit", replenishLimit, SqlDbType.Int);
                db.CreateParameter<int>("@SortOrder", sortOrder, SqlDbType.Int);

                db.ExecuteNonQuery();
            }
        }

        public override void PrizePropForMission(int userID, System.Collections.Hashtable prizes)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sb = new StringBuilder();

                foreach (int propID in prizes.Keys)
                {
                    sb.AppendFormat(@"
IF EXISTS(SELECT * FROM bx_UserProps WHERE UserID = {0} AND PropID = {1}) BEGIN
    UPDATE bx_UserProps SET [Count] = [Count] + {2} WHERE UserID = {0} AND PropID = {1};
END
ELSE BEGIN
    INSERT INTO bx_UserProps ([UserID],[PropID],[Count]) VALUES ({0}, {1}, {2});
END", userID, propID, prizes[propID]);
                }

                query.CommandText = sb.ToString();

                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name="bx_Prop_BuyProp", Script= @"
CREATE PROCEDURE {name}
  @UserID int
 ,@PropID int
 ,@BuyCount int
 ,@MaxPackageSize int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM bx_Props WHERE PropID = @PropID AND TotalNumber >= @BuyCount) BEGIN

        DECLARE @PackageSize int;

        SELECT @PackageSize = SUM(B.PackageSize * (A.[Count] + A.[SellingCount])) FROM bx_UserProps A LEFT JOIN bx_Props B ON B.PropID = A.PropID WHERE UserID = @UserID;

        SELECT @PackageSize = ISNULL(@PackageSize, 0) + PackageSize * @BuyCount FROM bx_Props WHERE PropID = @PropID;

        IF @PackageSize <= @MaxPackageSize BEGIN
            IF EXISTS(SELECT * FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID) BEGIN
                UPDATE bx_UserProps SET [Count] = [Count] + @BuyCount WHERE UserID = @UserID AND PropID = @PropID;
            END
            ELSE BEGIN
                INSERT INTO bx_UserProps ([UserID],[PropID],[Count]) VALUES (@UserID, @PropID, @BuyCount);
            END
        END
        ELSE BEGIN
            RETURN 3;
        END

        UPDATE bx_Props SET SaledNumber = SaledNumber + @BuyCount, TotalNumber = TotalNumber - @BuyCount WHERE PropID = @PropID;

        UPDATE bx_Props SET TotalNumber = 0 WHERE TotalNumber < 0;

        INSERT INTO [bx_UserGetPropLogs](UserID,[bx_Users].Username,GetPropType,PropID,PropName,PropCount) 
        SELECT @UserID,Username,1,@PropID,[Name],@BuyCount
        FROM [bx_Users] LEFT JOIN [bx_Props] ON [bx_Props].PropID=@PropID
        WHERE [bx_Users].UserID=@UserID;

        RETURN 1;
    END

    RETURN 2;
END")]
        public override int BuyProp(int userID, int propID, int buyCount, int maxPackageSize)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_BuyProp";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<int>("@PropID", propID, SqlDbType.Int);
                db.CreateParameter<int>("@BuyCount", buyCount, SqlDbType.Int);
                db.CreateParameter<int>("@MaxPackageSize", maxPackageSize, SqlDbType.Int);

                SqlParameter result = db.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                db.ExecuteNonQuery();

                return (int)result.Value;
            }
        }

        [StoredProcedure(Name="bx_Prop_GetPropByID", Script=@"
CREATE PROCEDURE {name}
  @PropID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_Props WHERE PropID = @PropID;

END")]
        public override Prop GetPropByID(int propID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_GetPropByID";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@PropID", propID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Prop(reader);
                    }

                    return null;
                }
            }
        }

        public override PropCollection GetProps(int pageNumber, int pageSize, bool all, ref int? totalCount)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.Pager.TableName = "bx_Props";
                db.Pager.PageNumber = pageNumber;
                db.Pager.PageSize = pageSize;
                db.Pager.SortField = "SortOrder";
                db.Pager.PrimaryKey = "PropID";
                db.Pager.IsDesc = false;
                
                if(all == false)
                    db.Pager.Condition = "Enable = 1";

                if (totalCount != null)
                {
                    db.Pager.TotalRecords = totalCount;
                    db.Pager.SelectCount = false;
                }
                else
                {
                    db.Pager.SelectCount = true;
                }

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    PropCollection result = new PropCollection(reader);

                    if (reader.NextResult() && reader.Read())
                    {
                        totalCount = reader.Get<int>(0);
                    }

                    result.TotalRecords = totalCount.Value;

                    return result;
                }
            }
        }

        [StoredProcedure(Name="bx_Prop_GetUserPropStatus", Script=@"
CREATE PROCEDURE {name}
    @UserID int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SUM([Count]) AS [Count], SUM([SellingCount]) AS [SellingCount], SUM(B.PackageSize * (A.[Count] + A.[SellingCount])) AS UsedPackageSize FROM bx_UserProps A LEFT JOIN bx_Props B ON B.PropID = A.PropID WHERE UserID = @UserID;
END")]
        public override UserPropStatus GetUserPropStatus(int userID)
        {
            using(SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_GetUserPropStatus";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using(XSqlDataReader reader = db.ExecuteReader())
                {
                    if(reader.Read())
                        return new UserPropStatus(reader);

                    return new UserPropStatus(0,0,0);
                }
            }
        }

        [StoredProcedure(Name="bx_Prop_GetUserProps", Script= @"
CREATE PROCEDURE {name}
    @UserID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_Props A
    LEFT JOIN bx_UserProps B ON A.PropID = B.PropID WHERE B.UserID = @UserID ORDER BY UserPropID DESC;

END")]
        public override UserPropCollection GetUserProps(int userID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_GetUserProps";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    UserPropCollection result = new UserPropCollection(reader);

                    return result;
                }
            }
        }

        public override UserPropCollection GetUserPropsForAdmin(UserPropFilter filter, int pageNumber)
        {
            using(SqlQuery db = new SqlQuery())
            {
                db.Pager.TableName = "bx_UserPropsView";
                db.Pager.PageSize = filter.PageSize;
                db.Pager.PageNumber = pageNumber;
                db.Pager.SelectCount = true;
                db.Pager.IsDesc = filter.IsDesc;

                if(filter.Order == UserPropFilter.OrderBy.Count)
                {
                    db.Pager.SortField = "Count";
                    db.Pager.PrimaryKey = "UserPropID";
                }
                else
                {
                    db.Pager.SortField = "UserPropID";
                }

                db.Pager.Condition = BuildCondition(db, filter);

                using(XSqlDataReader reader = db.ExecuteReader())
                {
                    UserPropCollection result = new UserPropCollection(reader);

                    if(reader.NextResult() && reader.Read())
                        result.TotalRecords = reader.Get<int>(0);

                    return result;
                }
            }
        }

        public override void DeleteUserPropsForAdmin(int[] propIDs)
        {
            using(SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_UserProps WHERE UserPropID IN (@PropIDs)";

                query.CreateInParameter<int>("@PropIDs", propIDs);

                query.ExecuteNonQuery();
            }
        }

        public override bool DeleteUserPropsForAdmin(UserPropFilter filter, int topCount, out int deletedCount)
        {
            deletedCount = 0;

            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildCondition(query, filter);

                StringBuffer sql = new StringBuffer();

                sql += @"
DECLARE @DeleteData table (UserPropID int);

INSERT INTO @DeleteData SELECT TOP (@TopCount) [UserPropID] FROM [bx_UserPropsView] WHERE " + conditions + @";

DELETE [bx_UserPropsView] WHERE UserPropID IN (SELECT [UserPropID] FROM @DeleteData);

SELECT @@ROWCOUNT;";

                query.CreateTopParameter("@TopCount", topCount);

                query.CommandText = sql.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    if (reader.Read())
                        deletedCount = reader.Get<int>(0);

                    return true;
                }
            }
        }

        private string BuildCondition(SqlQuery db, UserPropFilter filter)
        {
            StringBuffer sb = new StringBuffer();

            if(filter.UserID != null)
            {
                sb += " AND UserID = @UserID";
                db.CreateParameter<int>("@UserID", filter.UserID.Value, SqlDbType.Int);
            }
            else if(string.IsNullOrEmpty(filter.User) == false)
            {
                sb += " AND UserID = (SELECT UserID FROM bx_Users WHERE Username = @User)";
                db.CreateParameter<string>("@User", filter.User, SqlDbType.NVarChar, 50);
            }

            if(filter.PropID != null)
            {
                sb += " AND PropID = @PropID";
                db.CreateParameter<int>("@PropID", filter.PropID.Value, SqlDbType.Int);
            }

            if(filter.Selling != null)
            {
                if(filter.Selling.Value)
                    sb += " AND SellingCount > 0";
                else
                    sb += " AND SellingCount = 0";
            }

            if(sb.Length > 0)
                sb.InnerBuilder.Remove(0, 5);

            return sb.ToString();
        }

        public override UserPropCollection GetSellingUserProps(int pageNumber, int pageSize, ref int? totalCount)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.Pager.TableName = "bx_SellingProps";
                db.Pager.PageNumber = pageNumber;
                db.Pager.PageSize = pageSize;
                db.Pager.SortField = "SellingDate";
                db.Pager.PrimaryKey = "UserPropID";
                db.Pager.Condition = @" PropID IN(SELECT PropID FROM bx_Props WHERE Enable = 1) ";
                if (totalCount != null)
                {
                    db.Pager.TotalRecords = totalCount;
                    db.Pager.SelectCount = false;
                }
                else
                {
                    db.Pager.SelectCount = true;
                }

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    UserPropCollection result = new UserPropCollection(reader);

                    if (reader.NextResult() && reader.Read())
                    {
                        totalCount = reader.Get<int>(0);
                    }

                    result.TotalRecords = totalCount.Value;

                    return result;
                }
            }
        }

        [StoredProcedure(Name="bx_Prop_SaleUserProp", Script= @"
CREATE PROCEDURE {name}
    @UserID int
    ,@PropID int
    ,@Count int
    ,@Price int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID AND [Count] + [SellingCount] >= @Count) BEGIN
        UPDATE bx_UserProps
        SET
            [SellingDate] = GETDATE()
        WHERE
            UserID = @UserID AND PropID = @PropID AND SellingCount = 0;

        UPDATE bx_UserProps 
        SET 
            [Count] = [Count] + [SellingCount] - @Count, 
            [SellingCount] = @Count, 
            [SellingPrice] = @Price
        WHERE 
            UserID = @UserID AND PropID = @PropID;
    END
    ELSE BEGIN
        RETURN 2;
    END

    RETURN 1;
END")]
        public override int SaleUserProp(int userID, int propID, int count, int price)
        {
            using(SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_SaleUserProp";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<int>("@PropID", propID, SqlDbType.Int);
                db.CreateParameter<int>("@Count", count, SqlDbType.Int);
                db.CreateParameter<int>("@Price", price, SqlDbType.Int);

                SqlParameter result = db.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                db.ExecuteNonQuery();

                return (int)result.Value;
            }
        }

        [StoredProcedure(Name = "bx_Prop_BuyUserProp", Script = @"
CREATE PROCEDURE {name}
    @SalerUserID int
    ,@BuyerUserID int
    ,@PropID int
    ,@Count int
    ,@MaxPackageSize int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM bx_UserProps WHERE @Count > 0 AND UserID = @SalerUserID AND PropID = @PropID AND SellingCount >= @Count) BEGIN

        DECLARE @PackageSize int;

        SELECT @PackageSize = SUM(B.PackageSize * (A.[Count] + A.[SellingCount])) FROM bx_UserProps A LEFT JOIN bx_Props B ON B.PropID = A.PropID WHERE UserID = @BuyerUserID;

        SELECT @PackageSize = ISNULL(@PackageSize, 0) + PackageSize * @Count FROM bx_Props WHERE PropID = @PropID;

        IF @PackageSize <= @MaxPackageSize BEGIN
            IF EXISTS (SELECT * FROM bx_UserProps WHERE UserID = @BuyerUserID AND PropID = @PropID)BEGIN
                UPDATE bx_UserProps SET [Count] = [Count] + @Count WHERE UserID = @BuyerUserID AND PropID = @PropID;
            END
            ELSE BEGIN
                INSERT INTO bx_UserProps ([UserID], [PropID], [Count]) VALUES (@BuyerUserID, @PropID, @Count);
            END
        END
        ELSE BEGIN
            RETURN 3;
        END

        UPDATE bx_UserProps SET [SellingCount] = [SellingCount] - @Count WHERE UserID = @SalerUserID AND PropID = @PropID;

        DELETE FROM bx_UserProps WHERE UserID = @SalerUserID AND PropID = @PropID AND [SellingCount] = 0 AND [Count] = 0;

        RETURN 1;
    END

    RETURN 2;

END")]
        public override int BuyUserProp(int selerUserID, int buyerUserID,int propID, int count, int maxPackageSize)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_BuyUserProp";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@SalerUserID", selerUserID, SqlDbType.Int);
                db.CreateParameter<int>("@BuyerUserID", buyerUserID, SqlDbType.Int);
                db.CreateParameter<int>("@PropID", propID, SqlDbType.Int);
                db.CreateParameter<int>("@Count", count, SqlDbType.Int);
                db.CreateParameter<int>("@MaxPackageSize", maxPackageSize, SqlDbType.Int);

                SqlParameter result = db.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                db.ExecuteNonQuery();

                return (int)result.Value;
            }
        }


        [StoredProcedure(Name = "bx_Prop_GetUserSellingProp", Script = @"
CREATE PROCEDURE {name}
    @UserID int
    ,@PropID int
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM bx_UserPropsView WHERE UserID = @UserID AND PropID = @PropID;

END")]
        public override UserProp GetUserProp(int userID, int propID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_GetUserSellingProp";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<int>("@PropID", propID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    if(reader.Read())
                        return new UserProp(reader);

                    return null;
                }
            }
        }

        [StoredProcedure(Name = "bx_Prop_GiftProp", Script = @"
CREATE PROCEDURE {name}
    @UserID int
    ,@TargetUserID int
    ,@PropID int
    ,@Count int
    ,@MaxPackageSize int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM bx_UserProps WHERE @Count > 0 AND UserID = @UserID AND PropID = @PropID AND [Count] >= @Count) BEGIN

        DECLARE @PackageSize int;

        SELECT @PackageSize = SUM(B.PackageSize * (A.[Count] + A.SellingCount)) FROM bx_UserProps A LEFT JOIN bx_Props B ON B.PropID = A.PropID WHERE UserID = @TargetUserID;

        SELECT @PackageSize = ISNULL(@PackageSize, 0) + PackageSize * @Count FROM bx_Props WHERE PropID = @PropID;

        IF @PackageSize <= @MaxPackageSize BEGIN
            IF EXISTS(SELECT * FROM bx_UserProps WHERE UserID = @TargetUserID AND PropID = @PropID) BEGIN
                UPDATE bx_UserProps SET [Count] = [Count] + @Count WHERE UserID = @TargetUserID AND PropID = @PropID;
            END
            ELSE BEGIN
                INSERT INTO bx_UserProps ([UserID], [PropID], [Count]) VALUES (@TargetUserID, @PropID, @Count);
            END
        END
        ELSE BEGIN
            RETURN 3;
        END

        UPDATE bx_UserProps SET [Count] = [Count] - @Count WHERE UserID = @UserID AND PropID = @PropID;

        DELETE FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID AND [SellingCount] = 0 AND [Count] = 0;

        INSERT INTO [bx_UserGetPropLogs](UserID,[bx_Users].Username,GetPropType,PropID,PropName,PropCount) 
        SELECT @UserID,Username,2,@PropID,[Name],@Count
        FROM [bx_Users] LEFT JOIN [bx_Props] ON [bx_Props].PropID=@PropID
        WHERE [bx_Users].UserID=@TargetUserID;

        RETURN 1;
    END

    RETURN 2;

END")]
        public override int GiftProp(int userID, int targetUserID, int propID, int count, int maxPackageSize)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_GiftProp";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);
                db.CreateParameter<int>("@PropID", propID, SqlDbType.Int);
                db.CreateParameter<int>("@Count", count, SqlDbType.Int);
                db.CreateParameter<int>("@MaxPackageSize", maxPackageSize, SqlDbType.Int);

                SqlParameter result = db.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                db.ExecuteNonQuery();

                return (int)result.Value;
            }
        }

        [StoredProcedure(Name = "bx_Prop_DropProp", Script = @"
CREATE PROCEDURE {name}
    @UserID int
    ,@PropID int
    ,@Count int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM bx_UserProps WHERE @Count > 0 AND UserID = @UserID AND PropID = @PropID AND [Count] >= @Count) BEGIN

        UPDATE bx_UserProps SET [Count] = [Count] - @Count WHERE UserID = @UserID AND PropID = @PropID;

        DELETE FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID AND [SellingCount] = 0 AND [Count] = 0;

        RETURN 1;
    END

    RETURN 2;

END")]
        public override int DropUserProp(int userID, int propID, int count)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Prop_DropProp";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<int>("@PropID", propID, SqlDbType.Int);
                db.CreateParameter<int>("@Count", count, SqlDbType.Int);

                SqlParameter result = db.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                db.ExecuteNonQuery();

                return (int)result.Value;
            }
        }

        public override void DisableProps(int[] propIDs)
        {
            using(SqlQuery db = new SqlQuery())
            {
                db.CommandText = "UPDATE bx_Props SET Enable = 0 WHERE PropID IN(@PropIDs)";
                
                db.CreateInParameter<int>("@PropIDs", propIDs);

                db.ExecuteNonQuery();
            }
        }

        public override void EnableProps(int[] propIDs)
        {
            using(SqlQuery db = new SqlQuery())
            {
                db.CommandText = "UPDATE bx_Props SET Enable = 1 WHERE PropID IN(@PropIDs)";
                
                db.CreateInParameter<int>("@PropIDs", propIDs);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name="bx_Prop_GetUserPropByType", Script=@"
CREATE PROCEDURE {name}
    @PropType nvarchar(512)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 * FROM bx_UserProps A RIGHT JOIN bx_Props B ON B.PropID = A.PropID WHERE PropType = @PropType;
END")]
        public override UserProp GetUserProp(int userID, string propType)
        {
            using(SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Prop_GetUserPropByType";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@PropType", propType, SqlDbType.NVarChar, 512);

                using(XSqlDataReader reader = query.ExecuteReader())
                {
                    if(reader.Read())
                        return new UserProp(reader);

                    return null;
                }
            }
        }

        [StoredProcedure(Name="bx_Prop_UseProp", Script=@"
CREATE PROCEDURE {name}
    @UserID int,
    @PropID int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID AND Count > 0) BEGIN

        UPDATE bx_UserProps SET Count = Count - 1 WHERE UserID = @UserID AND PropID = @PropID AND Count > 0;

        DELETE FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID AND [SellingCount] = 0 AND [Count] = 0;
        
        RETURN 1;
    END

    RETURN 2;
END")]
        public override bool UseProp(int userID, int propID)
        {
            using(SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Prop_UseProp";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@PropID", propID, SqlDbType.Int);

                SqlParameter result = query.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return ((int)result.Value) == 1;
            }
        }

        public override void DeleteProps(int[] ids)
        {
            using(SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_Props WHERE PropID IN(@PropIDs)";

                query.CreateInParameter<int>("@PropIDs", ids);

                query.ExecuteNonQuery();
            }
        }
    }
}