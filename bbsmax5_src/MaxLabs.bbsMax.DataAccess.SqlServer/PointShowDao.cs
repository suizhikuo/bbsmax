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

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class PointShowDao : DataAccess.PointShowDao
    {

        #region 获取信息
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>

        [StoredProcedure(Name = "bx_GetMyPointShowInfo", Script = @"
CREATE PROCEDURE {name}
@UserID  int
AS 
BEGIN

SET NOCOUNT ON;

    DECLARE @Rank int
    SET @Rank=(SELECT COUNT(Price) FROM bx_PointShows WHERE Price>( SELECT Price FROM bx_PointShows WHERE  UserID = @UserID));
    SELECT *,@Rank+1 AS RANK FROM bx_PointShows WHERE UserID =@UserID;

END
")]
        public override PointShow GetMyPointShowInfo(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetMyPointShowInfo";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PointShow showinfo = null;
                    while (reader.Next)
                    {
                        showinfo = new PointShow(reader);
                    }
                    return showinfo;
                }

            }
        }


        #endregion

        #region 扣积分
        /// <summary>
        /// 扣除竞价积分
        /// </summary>
        /// <param name="userID"></param>
        [StoredProcedure(Name = "bx_DeductPointShowPoint", Script = @"
CREATE PROCEDURE {name} 
@UserID int
AS
BEGIN
SET NOCOUNT ON;
    DECLARE @OldPrice int,@OldPoints int;
    SELECT @OldPoints = ShowPoints, @OldPrice = Price FROM bx_PointShows WHERE UserID = @UserID;
    UPDATE bx_PointShows SET ShowPoints = ShowPoints - Price WHERE UserID = @UserID AND ShowPoints > 0;
    DELETE FROM bx_PointShows WHERE UserID = @UserID AND ShowPoints <= 0; 
    SELECT @OldPoints-@OldPrice,@OldPrice;
END
")]
        public override void DeductPointShowPoint(int userID, out int overage, out int oldPrice)
        {
            overage = 0;
            oldPrice = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeductPointShowPoint";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        overage = reader.Get<int>(0);
                        oldPrice = reader.Get<int>(1);
                    }
                }
            }
            if (overage < 0)
                overage = 0;
        }

        #endregion

        #region 判断是否上榜

        /// <summary>
        /// 返回该用户是否在竞价排行榜
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_IsPointShowUser", Script = @"
CREATE PROCEDURE {name}

@UserID int 
AS
BEGIN
SET NOCOUNT ON;

SELECT COUNT(UserID) FROM bx_PointShows WHERE UserID = @UserID;

END

")]
        public override bool IsPointShowUser(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_IsPointShowUser";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                return (int)query.ExecuteScalar() > 0;
            }

        }


        #endregion

        #region 更新单价

        /// <summary>
        /// 积分秀修改点击单价
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_UpdatePointShowPrice", Script = @"
CREATE PROCEDURE {name}
 @UserID       int
,@AddPoints    int
,@Price        int
,@Content  nvarchar(500)
AS
BEGIN
SET NOCOUNT ON;
IF EXISTS( SELECT * FROM bx_PointShows WHERE UserID = @UserID ) BEGIN
    EXEC bx_SubjoinPointShowPrice @UserID,@AddPoints
    UPDATE bx_PointShows SET Price = @Price ,Content = @Content WHERE UserID = @UserID  AND ShowPoints >= @Price;
    IF  @@ROWCOUNT>0 BEGIN
    EXEC bx_GetMyPointShowInfo @UserID
        RETURN 0;
    END
    ELSE BEGIN
        RETURN 1;
    END
END
ELSE BEGIN
 RETURN -1;
END

END
")]
        public override int UpdatePointShow(int userId, int addPoints, int price, string content, out PointShow showinfo)
        {
            using (SqlQuery query = new SqlQuery())
            {

                query.CommandText = "bx_UpdatePointShowPrice";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@Price", price, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userId, SqlDbType.Int);
                query.CreateParameter<int>("@AddPoints", addPoints, SqlDbType.Int);
                query.CreateParameter<string>("@Content", content, SqlDbType.NVarChar, 500);

                SqlParameter parameter = query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    showinfo = null;
                    while (reader.Next)
                    {
                        showinfo = new PointShow(reader);
                    }
                }

                return (int)parameter.Value;
            }
        }

        #endregion

        #region 充值
        /// <summary>
        /// 积分秀充值
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_SubjoinPointShowPrice", Script = @"
CREATE PROCEDURE {name}
 @UserID   int
,@Point    int
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE bx_PointShows SET ShowPoints = ShowPoints + @Point WHERE UserID = @UserID;
    RETURN @@ROWCOUNT;
END
")]
        public override bool SubjoinPointShowPoint(int userID, int point)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_SubjoinPointShowPrice";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@Point", point, SqlDbType.Int);
                SqlParameter returnValue = query.CreateParameter<int>("@Return", SqlDbType.Int, ParameterDirection.ReturnValue);
                query.ExecuteNonQuery();

                return (int)returnValue.Value > 0;
            }
        }

        #endregion

        #region 上榜
        [StoredProcedure(Name = "bx_CreatePointShow", Script = @"
CREATE PROCEDURE {name}
     @UserID int
    ,@Points int
    ,@Price  int
    ,@Content nvarchar(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF Exists( SELECT * FROM bx_PointShows WHERE UserID = @UserID) BEGIN
        RETURN;
    END
    INSERT INTO bx_PointShows( ShowPoints, Price, UserID, Content) VALUES ( @Points, @Price, @UserID, @Content);
    EXEC bx_GetMyPointShowInfo @UserID
END
")]

        public override PointShow CreatePointShow(int userID, int point, int price, string content)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreatePointShow";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@Points", point, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@Price", price, SqlDbType.Int);
                query.CreateParameter<string>("@Content", content, SqlDbType.NVarChar, 100);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PointShow pointShowInfo = null;
                    while (reader.Next)
                    {
                        pointShowInfo = new PointShow(reader);
                    }
                    return pointShowInfo;
                }
            }
        }
        #endregion

        #region 帮好友上榜
        [StoredProcedure(Name = "bx_UpdateFriendPointShow", Script = @"
CREATE PROCEDURE bx_UpdateFriendPointShow
     @UserID int
    ,@Points int
    ,@Username nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ShowUserID int;
    DECLARE @FriendUserID int;

    SELECT @ShowUserID = UserID FROM bx_Users WHERE Username = @Username;

    IF @ShowUserID IS NULL BEGIN
        SELECT 4;--用户不存在
        RETURN;
    END

    SELECT @FriendUserID = FriendUserID FROM bx_Friends WHERE UserID = @UserID AND FriendUserID = @ShowUserID
    
    IF @FriendUserID IS NULL BEGIN
        SELECT 3;--不是好友
        RETURN;
    END


    IF EXISTS(SELECT * FROM bx_PointShows WHERE UserID = @FriendUserID)
        BEGIN
            UPDATE bx_PointShows SET ShowPoints = ShowPoints + @Points WHERE UserID = @FriendUserID
            
            SELECT 1;--更新
        END
    ELSE
        BEGIN
            INSERT INTO bx_PointShows(ShowPoints,UserID,Content) VALUES (@Points,@FriendUserID,'')
           
            SELECT 2;--新上榜
        END

END
")]

        public override int AddPointShow(int userID, string username, int point, int price)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateFriendPointShow";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@Points", point, SqlDbType.Int);
                query.CreateParameter<string>("@Username", username, SqlDbType.NVarChar, 50);

                return query.ExecuteScalar<int>();
            }
        }

        #endregion

        #region 获取列表
        public override UserCollection GetUserShows(int userID, int pageNumber, int pageSize, out int userTotalCount)
        {
            userTotalCount = 0;

            UserCollection users = new UserCollection();

            using (SqlSession db = new SqlSession())
            {
                using (SqlQuery query = db.CreateQuery())
                {

                    query.Pager.TableName = "bx_PointShowUsers";
                    query.Pager.SortField = "Price";
                    query.Pager.IsDesc = true;
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = pageSize;
                    query.Pager.SelectCount = true;
                    query.Pager.PrimaryKey = "UserID";
                    query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        users = new UserCollection(reader);

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                users.TotalRecords = reader.Get<int>(0);
                                userTotalCount = users.TotalRecords;
                            }
                        }
                    }
                }

                return users;
            }
        }

        public override PointShowCollection GetPointShowList(int pageSize, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.SortField = "[Price]";
                query.Pager.IsDesc = true;
                query.Pager.PrimaryKey = "[UserID]";
                query.Pager.PageSize = pageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "bx_PointShows";

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PointShowCollection result = new PointShowCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Read()) result.TotalRecords = reader.Get<int>(0);
                    }
                    return result;
                }
            }
        }

        public override int RemoveFomList(IEnumerable<int> pointshowUserids)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_PointShows WHERE UserID IN( @UserIDs)";
                query.CreateInParameter<int>("@UserIDs", pointshowUserids);
                return query.ExecuteScalar<int>();
            }
        }

        public override UserCollection GetUserShows(int userID, int pageNumber, int pageSize, out Dictionary<int, int> points, out Dictionary<int, string> contents)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_PointShowUsers";
                query.Pager.SortField = "[Price]";
                query.Pager.IsDesc = true;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.PrimaryKey = "[UserID]";

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserCollection users = new UserCollection();
                    points = new Dictionary<int, int>();
                    contents = new Dictionary<int, string>();
                    while (reader.Read())
                    {
                        User user = new User(reader);
                        users.Add(user);
                        points.Add(user.UserID, reader.Get<int>("Price"));
                        contents.Add(user.UserID, reader.Get<string>("Content"));
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            users.TotalRecords = reader.Get<int>(0);
                        }
                    }
                    return users;
                }

            }
        }
        #endregion
    }
}