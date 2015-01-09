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

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class AuctionDao : DataAccess.AuctionDao
    {
        public override void CreateAution(int sortOrder
            , string title
            , string description
            , int lowestPrice
            , int increasePrice
            , int usePoint
            , bool isOpen
            ,int keepHours, string beginTime, int timeLength,string allowedRoles)
        {
            string sql;

            sql = @"INSERT INTO Chinaz_Auctions( SortOrder, Title,Description, LowestPrice, IncreasePrice, UsePoint, BeginTime, TimeLength, IsOpen, KeepHours, AllowedRoles) 
VALUES( @SortOrder, @Title, @Description, @LowestPrice, @IncreasePrice,  @UsePoint, @BeginTime, @TimeLength, @IsOpen, @KeepHours, @AllowedRoles)";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql;
                query.CreateParameter<int>("@SortOrder",sortOrder,SqlDbType.Int);
                query.CreateParameter<string>("@Title",title,SqlDbType.NVarChar,200);
                query.CreateParameter<string>("@Description",description,SqlDbType.NVarChar,1000);
                query.CreateParameter<int>("@LowestPrice",lowestPrice,SqlDbType.Int);
                query.CreateParameter<int>("@IncreasePrice", increasePrice, SqlDbType.Int);
                query.CreateParameter<int>("@TimeLength",timeLength,SqlDbType.Int);
                query.CreateParameter<bool>("@IsOpen",isOpen,SqlDbType.Bit);
                query.CreateParameter<int>("@KeepHours", keepHours, SqlDbType.Int);
                query.CreateParameter<string>("@AllowedRoles",allowedRoles,SqlDbType.VarChar,4000);
                query.CreateParameter<string>("@BeginTime", beginTime, SqlDbType.VarChar, 10);
                query.CreateParameter<int>("@UsePoint",usePoint,SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        public override void BuildAuctionStage(IEnumerable<AuctionStage> newStages)
        {
            using( SqlQuery query = new SqlQuery() )
            {
                StringBuilder builer = new StringBuilder();
                int i = 0;
                foreach (AuctionStage s in newStages)
                {
                    string stageid = "@StageID_"+ i; 
                    string aucid = "@AuctionID_"+i;
                    string begintime = "@BeginTime_"+i;
                    string price = "@CurrentPrice_" + i;
                    string endTime = "@EndTime_"+i;
                    string usePoint = "@UsePoint_"+i;
                    builer.AppendFormat(@"IF NOT EXISTS( SELECT * FROM Chinaz_AuctionStages WHERE StageID = {0})
                                             INSERT INTO Chinaz_AuctionStages( StageID, AuctionID, BeginTime, EndTime, UsePoint,CurrentPrice) VALUES({0},{1},{2},{3},{4}, {5});"
                        , stageid, aucid, begintime, endTime, usePoint,price);

                    query.CreateParameter<int>(stageid, s.StageID, SqlDbType.Int);
                    query.CreateParameter<int>(aucid, s.AuctionID, SqlDbType.Int);
                    query.CreateParameter<DateTime>(begintime, s.BeginTime, SqlDbType.DateTime);
                    query.CreateParameter<DateTime>(endTime, s.EndTime, SqlDbType.DateTime);
                    query.CreateParameter<int>(usePoint, (int)s.UsePoint, SqlDbType.Int);
                    query.CreateParameter<int>(price, s.CurrentPrice, SqlDbType.Int);

                    i++;
                }

                query.CommandText = builer.ToString();
  
                query.ExecuteNonQuery();
            }
        }

        public override Auction GetAuction(int auctionID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM Chinaz_Auctions WHERE AuctionID = @AuctionID";
                query.CreateParameter<int>("@AuctionID",auctionID, SqlDbType.Int);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    Auction result = null;
                    while (reader.Next)
                        result = new Auction(reader);

                    return result;
                }
            }
        }

        public override void UpdateAution(int auctionID
        , int sortOrder
        , string title
        , string description
        , int usePoint
        , int lowestPrice
        , int increasePrice
        , string beginTime
        , int timeLength
        , int keepHours
        , string allowedRoles)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE Chinaz_Auctions SET SortOrder = @SortOrder, Title = @Title,LowestPrice = @LowestPrice,Description = @Description, IncreasePrice = @IncreasePrice, UsePoint = @UsePoint, BeginTime = @BeginTime, KeepHours = @KeepHours, TimeLength = @TimeLength, AllowedRoles = @AllowedRoles WHERE AuctionID = @AuctionID;";

                query.CreateParameter<int>("@AuctionID", auctionID, SqlDbType.Int);
                query.CreateParameter<int>("@SortOrder",sortOrder,SqlDbType.Int);
                query.CreateParameter<string>("@Title",title,SqlDbType.NVarChar,255);
                query.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 1000);
                query.CreateParameter<int>("@UsePoint", usePoint, SqlDbType.Int);
                query.CreateParameter<int>("@LowestPrice", lowestPrice,SqlDbType.Int);
                query.CreateParameter<int>("@IncreasePrice", increasePrice, SqlDbType.Int);
                query.CreateParameter<string>("@BeginTime", beginTime, SqlDbType.VarChar, 10);
                query.CreateParameter<int>("@KeepHours", keepHours, SqlDbType.Int);
                query.CreateParameter<string>("@AllowedRoles", allowedRoles, SqlDbType.VarChar, 4000);
                query.CreateParameter<int>("@TimeLength", timeLength, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        public override AuctionBidInfo GetAuctionBid(int userID, int auctionID, int stageID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM Chinaz_AuctionBids WHERE UserID = @UserID AND AuctionID = @AuctionID AND StageID";


                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@AuctionID", auctionID, SqlDbType.Int);
                query.CreateParameter<int>("@StageID", stageID, SqlDbType.Int);

                using( XSqlDataReader reader = query.ExecuteReader() )
                {
                    AuctionBidInfo bidInfo = null;

                    while (reader.Next)
                        bidInfo = new AuctionBidInfo(reader);

                    return bidInfo;
                }
            }
        }

        public override AuctionHistoryCollection GetAuctionHistoryList(DateTime firstDay)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"SELECT Chinaz_AuctionBids.BidID, Chinaz_Auctions.AuctionID, Chinaz_AuctionStages.StageID, Chinaz_Auctions.Title, 
                      Chinaz_AuctionBids.Url, Chinaz_AuctionBids.Title AS SiteName, Chinaz_AuctionStages.UsePoint, Chinaz_AuctionBids.Price, 
                      Chinaz_AuctionStages.EndTime
FROM Chinaz_Auctions INNER JOIN
                      Chinaz_AuctionStages ON Chinaz_Auctions.AuctionID = Chinaz_AuctionStages.AuctionID LEFT OUTER JOIN
                      Chinaz_AuctionBids ON Chinaz_AuctionStages.StageID = Chinaz_AuctionBids.StageID
WHERE (Chinaz_AuctionStages.IsEnd = 1) AND (Chinaz_AuctionBids.Successful = 1) AND (Chinaz_AuctionStages.EndTime > @FirstDay)
ORDER BY Chinaz_Auctions.AuctionID, Chinaz_AuctionStages.StageID DESC";

                query.CreateParameter<DateTime>("@FirstDay", firstDay, SqlDbType.DateTime);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AuctionHistoryCollection(reader);
                }
            }
        }

        public override AuctionBidInfo GetLastBid( int auctionID, int stageID )
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT TOP 1 * FROM Chinaz_AuctionBids WHERE AuctionID = @AuctionID AND StageID = @StageID ORDER BY Price DESC;";

                query.CreateParameter<int>("@AuctionID", auctionID, SqlDbType.Int);
                query.CreateParameter<int>("@StageID", stageID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    AuctionBidInfo bid = null;

                    while (reader.Next)
                        bid = new AuctionBidInfo(reader);

                    return bid;
                }
            }
        }

        public override BidResultCollection GetAuctionResults()
        {
            string sql = @"SELECT AuctionID, Url, Title, ShowBeginTime, ShowEndTime FROM Chinaz_AuctionBids WHERE BidID IN(SELECT BidID FROM(SELECT AuctionID,MAX(BidID) AS BidID FROM Chinaz_AuctionBids  WHERE Successful =1 GROUP BY AuctionID) as a )";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new BidResultCollection(reader);
                }
            }
        }

        public override bool Delete(int auctionID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM Chinaz_Auctions WHERE AuctionID = @AuctionID";
                query.CreateParameter<int>("@AuctionID", auctionID, SqlDbType.Int);
                return query.ExecuteNonQuery() > 0;
            }
        }

        public override AuctionBidInfo GetAuctionBidResult(int auctionID, int stageID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM Chinaz_AuctionBids WHERE Successful = 1 AND AuctionID = @AuctionID AND StageID";

                query.CreateParameter<int>("@AuctionID", auctionID, SqlDbType.Int);
                query.CreateParameter<int>("@StageID", stageID, SqlDbType.Int);

                using( XSqlDataReader reader = query.ExecuteReader() )
                {
                    AuctionBidInfo bidInfo = null;

                    while (reader.Next)
                        bidInfo = new AuctionBidInfo(reader);

                    return bidInfo;
                }
            }
        }


        [StoredProcedure( Name="Chinaz_LoadAuctions",Script= @"
CREATE PROCEDURE {name}
@GetLastStage bit
AS
BEGIN
SET NOCOUNT ON;

SELECT * FROM Chinaz_Auctions ORDER BY SortOrder ASC;

IF @GetLastStage =1 BEGIN
    DECLARE @LastStage table(aid int,sid int);
    INSERT @LastStage SELECT AuctionID, MAX(StageID) FROM Chinaz_AuctionStages GROUP BY AuctionID;
    SELECT * FROM Chinaz_AuctionStages  WHERE StageID IN(SELECT sid FROM @LastStage);
    SELECT * FROM Chinaz_AuctionBids as a,@LastStage as b WHERE  a.AuctionID = b.aid AND a.StageID = b.sid ORDER BY a.BidID DESC;
END
END
")]

        public override AuctionCollection GetAuctions(bool openOnly,bool getLastStage)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "Chinaz_LoadAuctions";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<bool>("@GetLastStage",getLastStage, SqlDbType.Bit);

                AuctionCollection auctions = new AuctionCollection();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    auctions = new AuctionCollection(reader);

                    if (getLastStage && reader.NextResult())
                    {
                        while (reader.Next)
                        {
                            AuctionStage stage = new AuctionStage(reader);

                            foreach (Auction auc in auctions)
                            {
                                if (auc.AuctionID == stage.AuctionID)
                                {
                                    auc.LastStage = stage;
                                }
                            }
                        }

                        if (reader.NextResult())
                        {
                            AuctionBidInfo bid;
                            while (reader.Next)
                            {
                                bid = new AuctionBidInfo(reader);

                                foreach (Auction auc in auctions)
                                {
                                    if (auc.LastStage != null
                                        && auc.LastStage.StageID == bid.StageID
                                        && auc.AuctionID == bid.AuctionID)
                                    {
                                        if (auc.LastStage.LastBid == null)
                                        {
                                            auc.LastStage.LastBid = bid;
                                        }
                                        else if (auc.LastStage.LastBid.Price < bid.Price)
                                        {
                                            auc.LastStage.LastBid = bid;
                                        }
                                        if (auc.LastStage.EndTime > DateTimeUtil.Now)
                                            auc.LastStage.BidList.Add(bid);
                                    }
                                }
                            }
                        }
                    }

                    return auctions;
                }
            }
        }

        public override AuctionBidInfo GetMyAuctionBid(int userId, int auctionId, int stageId)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT TOP 1 * FROM Chinaz_AuctionBids WHERE UserID = @UserID AND AuctionID = @AuctionID AND StageID = @StageID ORDER BY BidID DESC";
                query.CreateParameter<int>("@UserID", userId, SqlDbType.Int);
                query.CreateParameter<int>("@AuctionID", auctionId, SqlDbType.Int);
                query.CreateParameter<int>("@StageID", stageId, SqlDbType.Int);

                AuctionBidInfo bid = null;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Next)
                        bid = new AuctionBidInfo(reader);
                    return bid;
                }
            }
        }

        public override AuctionBidInfoCollection GetMyAuctionBids(int userID, int auctionID, int pageSize, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "Chinaz_AuctionBids";
                query.Pager.PrimaryKey = "BidID";
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.Condition  ="UserID = @UserID AND AuctionID = @AuctionID";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@AuctionID", auctionID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    AuctionBidInfoCollection bids = new AuctionBidInfoCollection(reader);
                    if (reader.NextResult())
                        while (reader.Next)
                            bids.TotalRecords = reader.Get<int>(0);

                    return bids;
                }
            }
        }

        [StoredProcedure(Name="Chinaz_EndAuction",Script= @"
CREATE PROCEDURE {name}
@AuctionID   int
,@StageID    int
,@ShowBeginTime datetime
,@ShowEndTime   datetime
AS
BEGIN
SET NOCOUNT ON;

UPDATE Chinaz_AuctionBids SET Successful = 1, ShowBeginTime = @ShowBeginTime, ShowEndTime = @ShowEndTime WHERE AuctionID = @AuctionID AND StageID = @StageID AND Price = 
(SELECT MAX(Price) FROM Chinaz_AuctionBids WHERE AuctionID = @AuctionID AND StageID = @StageID);
UPDATE Chinaz_AuctionStages SET IsEnd = 1 WHERE  AuctionID = @AuctionID AND StageID = @StageID;
END
")]
        public override void EndAuction(int auctionID, int stageID,DateTime showBeginTime, DateTime showEndTime, AuctionStage nextStage )
        {
            if (nextStage != null)
                BuildAuctionStage(new AuctionStage[] { nextStage });

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "Chinaz_EndAuction";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@AuctionID", auctionID, SqlDbType.Int);
                query.CreateParameter<int>("@StageID", stageID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@ShowBeginTime", showBeginTime, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@ShowEndTime", showEndTime, SqlDbType.DateTime);

                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name="Chinaz_AuctionBid",Script= @"
CREATE PROCEDURE {name}
 @UserID     int
,@AuctionID  int
,@StageID    int
,@Price      int
,@Title      nvarchar(150)
,@Url        nvarchar(150)
,@Remarks    nvarchar(200)
AS
BEGIN
SET NOCOUNT ON;

IF EXISTS( SELECT * FROM Chinaz_AuctionBids WHERE AuctionID = @AuctionID AND StageID = @StageID AND Price > @Price )  BEGIN --已有更高出价
    SELECT -1;
    RETURN;
END

INSERT INTO Chinaz_AuctionBids( UserID , AuctionID, StageID, Title, Url, Price, Remarks) VALUES (@UserID , @AuctionID, @StageID, @Title, @Url, @Price, @Remarks);

DECLARE @NewBidID int;
SET @NewBidID = @@IDENTITY;

UPDATE Chinaz_AuctionStages SET CurrentPrice = @Price WHERE AuctionID =  @AuctionID AND StageID = @StageID;
UPDATE Chinaz_AuctionBids SET  Refund = 1 WHERE StageID = @StageID AND AuctionID = @AuctionID AND  Refund = 0 AND Successful = 0 AND BidID < @NewBidID AND UserID = @UserID;

SELECT 0;

SELECT UserID, UsePoint, SUM(Price) AS Price
FROM (SELECT Chinaz_AuctionBids.UserID, Chinaz_AuctionStages.UsePoint, Chinaz_AuctionBids.Price
                       FROM Chinaz_AuctionBids INNER JOIN
                       Chinaz_AuctionStages ON Chinaz_AuctionBids.StageID = Chinaz_AuctionStages.StageID
                       WHERE Chinaz_AuctionBids.Refund = 0 
                        AND Chinaz_AuctionBids.Successful = 0 
                        AND Chinaz_AuctionStages.StageID = @StageID
                        AND Chinaz_AuctionStages.AuctionID = @AuctionID
                        AND Chinaz_AuctionBids.BidID < @NewBidID
                        ) AS a GROUP BY UserID, UsePoint HAVING UserID <> @UserID ;


END
")]
        public override int AuctionBid(int userID, int price, int auctionID, int stageID, string title, string url, string remarks,out Dictionary<int, int[]> unrefundPoints)
        {
            unrefundPoints = new Dictionary<int, int[]>();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "Chinaz_AuctionBid";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@AuctionID", auctionID, SqlDbType.Int);
                query.CreateParameter<int>("@StageID", stageID, SqlDbType.Int);
                query.CreateParameter<int>("@Price", price, SqlDbType.Int);
                query.CreateParameter<string>("@Title", title, SqlDbType.NVarChar, 150);
                query.CreateParameter<string>("@Url", url, SqlDbType.NVarChar, 150);
                query.CreateParameter<string>("@Remarks", remarks, SqlDbType.NVarChar, 200);
                int result = -1;
                using( XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Next)
                        result = reader.Get<int>(0);
                    if (result == 0)
                    {
                        if (reader.NextResult())
                        {
                            while (reader.Next)
                            {
                                int uid = reader.Get<int>("UserID");
                                int pindex = reader.Get<int>("UsePoint");
                                int pvalue = reader.Get<int>("Price");

                                int[] points =new int[]{0,0,0,0,0,0,0,0};
                                points[pindex] = pvalue;
                                if (unrefundPoints.ContainsKey(uid))
                                {
                                    points = unrefundPoints[uid];
                                    points[pindex] += pvalue;
                                }
                                else
                                {
                                    unrefundPoints.Add(uid, points);
                                }
                            }
                        }
                    }
                }
                return result;
            }
        }

        public override void RefundPoint(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
                UPDATE Chinaz_AuctionBids SET Refund = 1  WHERE UserID = @UserID AND Refund = 0 AND Chinaz_AuctionBids.Successful = 0
                AND StageID IN( SELECT StageID FROM Chinaz_AuctionStages WHERE IsEnd = 1)";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        public override void RefundPoint(IEnumerable<int> userIDs,int auctionID, int stageID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
                UPDATE Chinaz_AuctionBids SET Refund = 1  WHERE UserID IN( @UserIDs) AND AuctionID = @AuctionID AND StageID = @StageID AND Refund = 0 AND Chinaz_AuctionBids.Successful = 0;";

                query.CreateInParameter<int>("@UserIDs", userIDs);
                query.CreateParameter<int>("@AuctionID", auctionID, SqlDbType.Int);
                query.CreateParameter<int>("@StageID", stageID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "Chinaz_GetUnrefundPoints", Script = @"
CREATE PROCEDURE {name}
@UserID int
AS 
BEGIN
SET NOCOUNT ON;

SELECT UsePoint, SUM(Price) AS Price
FROM (SELECT Chinaz_AuctionStages.UsePoint, Chinaz_AuctionBids.Price
                       FROM Chinaz_AuctionBids INNER JOIN
                       Chinaz_AuctionStages ON Chinaz_AuctionBids.StageID = Chinaz_AuctionStages.StageID
                       WHERE  Chinaz_AuctionBids.UserID = @UserID 
                        AND Chinaz_AuctionBids.Refund = 0 
                        AND Chinaz_AuctionBids.Successful = 0 
                        AND Chinaz_AuctionStages.IsEnd = 1) AS a GROUP BY UsePoint
END
")]
        public override List<KeyValuePair<int, int>> GetUnrefundPoints(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "Chinaz_GetUnrefundPoints";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    List<KeyValuePair<int, int>> results = new List<KeyValuePair<int, int>>();

                    while (reader.Next)
                    {
                        KeyValuePair<int, int> item = new KeyValuePair<int, int>(reader.Get<int>(0), reader.Get<int>(1));
                        results.Add(item);
                    }

                    return results;
                }

            }
        }


        /// <summary>
        /// 是否有未退还的积分
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public override bool HasUnrefundPoints(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
                IF EXISTS( SELECT * FROM Chinaz_AuctionBids WHERE UserID = @UserID AND Refund = 0 AND Chinaz_AuctionBids.Successful = 0
            AND StageID IN( SELECT StageID FROM Chinaz_AuctionStages WHERE IsEnd = 1))
	                SELECT 1;
                ELSE
	                SELECT 0;
                ";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                return query.ExecuteScalar<int>() > 0;
            }
        }
    }
}