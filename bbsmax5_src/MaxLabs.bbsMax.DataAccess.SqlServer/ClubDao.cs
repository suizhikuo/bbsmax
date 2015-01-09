//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

using System.Data;
using System.Data.SqlClient;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class ClubDao : DataAccess.ClubDao
    {
        [StoredProcedure(Name = "bx_Club_GetClubCategories", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM bx_ClubCategories;
END")]
        public override ClubCategoryCollection GetClubCategories()
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_GetClubCategories";
                db.CommandType = System.Data.CommandType.StoredProcedure;

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    return new ClubCategoryCollection(reader);
                }
            }
        }

        [StoredProcedure(Name = "bx_Club_CreateClub", Script = @"
CREATE PROCEDURE {name}
	@UserID		int,
	@CategoryID	int,
	@IsApproved	bit,
	@CreateIP	varchar(50),
	@Name		nvarchar(50),
	@NewClubID	int output
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM bx_Clubs WHERE [Name] = @Name) BEGIN
		RETURN 0;
	END

	INSERT INTO bx_Clubs ([UserID],[CategoryID],[IsApproved],[CreateIP],[Name]) VALUES (@UserID, @CategoryID, @IsApproved, @CreateIP, @Name);

	SELECT @NewClubID = @@IDENTITY;

	INSERT INTO bx_ClubMembers (ClubID, UserID, Status) VALUES (@NewClubID, @UserID, 5);

	RETURN 1;
END")]
        public override CreateClubResult CreateClub(int operatorID, int categoryID, string clubName, bool isApproved, string operatorIP, out int newClubID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_CreateClub";
                db.CommandType = System.Data.CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", operatorID, SqlDbType.Int);
                db.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);
                db.CreateParameter<bool>("@IsApproved", isApproved, SqlDbType.Bit);
                db.CreateParameter<string>("@CreateIP", operatorIP, SqlDbType.VarChar, 50);
                db.CreateParameter<string>("@Name", clubName, SqlDbType.NVarChar, 50);

                SqlParameter newID = db.CreateParameter<int>("@NewClubID", SqlDbType.Int, ParameterDirection.Output);

                SqlParameter result = db.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

                db.ExecuteNonQuery();

                switch ((int)result.Value)
                {
                    case 0:
                        newClubID = 0;
                        return CreateClubResult.HasSameNameClub;

                    default:
                        newClubID = (int)newID.Value;
                        return CreateClubResult.Succeed;
                }
            }
        }

        public override ClubCollection GetAllClubs(int? categoryID, int pageSize, int pageNumber)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.Pager.TableName = "bx_Clubs";
                db.Pager.SortField = "TotalMembers";
                db.Pager.PrimaryKey = "ClubID";
                db.Pager.PageSize = pageSize;
                db.Pager.PageNumber = pageNumber;
                db.Pager.Condition = "IsApproved = 1";

                if (categoryID != null)
                {
                    db.Pager.Condition += " AND CategoryID = @CategoryID";

                    db.CreateParameter<int>("@CategoryID", categoryID.Value, SqlDbType.Int);
                }

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    return new ClubCollection(reader);
                }
            }
        }

        public override ClubCollection GetUserClubs(int userID, int pageSize, int pageNumber)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.Pager.TableName = "bx_Clubs";
                db.Pager.SortField = "ClubID";
                db.Pager.PageSize = pageSize;
                db.Pager.PageNumber = pageNumber;
                db.Pager.Condition = "ClubID IN (SELECT A.ClubID FROM bx_ClubMembers A WHERE UserID = @UserID AND Status != 1)";

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    return new ClubCollection(reader);
                }
            }
        }

        [StoredProcedure(Name = "bx_Club_GetClubByID", Script = @"
CREATE PROCEDURE {name}
	@ClubID	int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM bx_Clubs WHERE ClubID = @ClubID;
END")]
        public override Club GetClub(int clubID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_GetClubByID";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    if (reader.Read())
                        return new Club(reader);
                    else
                        return null;
                }
            }
        }

        [StoredProcedure(Name = "bx_Club_Update", Script = @"
CREATE PROCEDURE {name}
	@ClubID			int,
	@Description	nvarchar(200),
	@JoinMethod		tinyint,
	@AccessMode		tinyint,
	@IsNeedManager	bit
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE bx_Clubs SET 
		Description = @Description, 
		JoinMethod = @JoinMethod, 
		AccessMode = @AccessMode,
		IsNeedManager = @IsNeedManager,
		UpdateDate = GETDATE(),
		KeywordVersion = ''
	WHERE
		ClubID = @ClubID;
END")]
        public override void UpdateClub(int clubID, string description, ClubJoinMethod joinMethod, ClubAccessMode accessMode, bool isNeedManager)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_Update";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);
                db.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 200);
                db.CreateParameter<ClubJoinMethod>("@JoinMethod", joinMethod, SqlDbType.TinyInt);
                db.CreateParameter<ClubAccessMode>("@AccessMode", accessMode, SqlDbType.TinyInt);
                db.CreateParameter<bool>("@IsNeedManager", isNeedManager, SqlDbType.Bit);

                db.ExecuteNonQuery();
            }
        }

        public override ClubMemberCollection GetTopClubMembers(int clubID, int top)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "SELECT TOP (@Top) * FROM bx_ClubMembers WHERE ClubID = @ClubID AND Status IN (0, 3, 6)";

                db.CreateTopParameter("@Top", top);
                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    ClubMemberCollection result = new ClubMemberCollection(reader);

                    return result;
                }
            }
        }

        public override ClubMemberCollection GetTopClubManagers(int clubID, int top)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "SELECT TOP (@Top) * FROM bx_ClubMembers WHERE ClubID = @ClubID AND Status IN (4, 5)";

                db.CreateTopParameter("@Top", top);
                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    ClubMemberCollection result = new ClubMemberCollection(reader);

                    return result;
                }
            }
        }

        public override ClubMemberCollection GetClubMembers(int clubID, int pageSize, int pageNumber, ClubMemberStatus? status)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.Pager.TableName = "bx_ClubMembers";
                db.Pager.SortField = "SortOrder";
                db.Pager.PageSize = pageSize;
                db.Pager.PageNumber = pageNumber;
                db.Pager.Condition = "ClubID = @ClubID";
                db.Pager.SelectCount = true;

                if (status != null)
                {
                    db.Pager.Condition += " AND Status = @Status";
                    db.CreateParameter<ClubMemberStatus>("@Status", status.Value, SqlDbType.TinyInt);
                }
                else
                {
                    db.Pager.Condition += " AND Status <> 1 AND Status <> 2";
                }

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    ClubMemberCollection result = new ClubMemberCollection(reader);

                    reader.NextResult();
                    reader.Read();

                    result.TotalRecords = reader.Get<int>(0);

                    return result;
                }
            }
        }

        public override void RemoveClubMembers(int clubID, int[] userIDs)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "DELETE FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID IN (@UserIDs);";

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);
                db.CreateInParameter<int>("@UserIDs", userIDs);

                db.ExecuteNonQuery();
            }
        }

        public override void UpdateClubMemberStatus(int clubID, int[] userIDs, ClubMemberStatus status)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "UPDATE bx_ClubMembers SET Status = @Status WHERE ClubID = @ClubID AND UserID IN (@UserIDs);";

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);
                db.CreateParameter<ClubMemberStatus>("@Status", status, SqlDbType.TinyInt);
                db.CreateInParameter<int>("@UserIDs", userIDs);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Club_GetClubInvokes", Script = @"
CREATE PROCEDURE {name}
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM bx_Clubs WHERE ClubID IN (
		SELECT A.ClubID FROM bx_ClubMembers A WHERE Status = 1 AND UserID = @UserID
	);
END")]
        public override ClubCollection GetClubInvokes(int userID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_GetClubInvokes";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    return new ClubCollection(reader);
                }
            }
        }

        [StoredProcedure(Name = "bx_Club_InvokeMember", Script = @"
CREATE PROCEDURE {name}
	@ClubID		int,
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT * FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID = @UserID) BEGIN

		INSERT INTO bx_ClubMembers (ClubID, UserID, Status) VALUES (@ClubID, @UserID, 1);
	END
	ELSE BEGIN
		
		UPDATE bx_ClubMembers SET Status = 1 WHERE ClubID = @ClubID AND UserID = @UserID AND Status = 2;
	END
END")]
        public override void InviteClubMembers(int clubID, int[] userIDs)
        {
            using (SqlQuery db = new SqlQuery(QueryMode.Prepare))
            {
                db.CommandText = "bx_Club_InvokeMember";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);

                for (int i = 0; i < userIDs.Length; i++)
                {
                    db.CreateParameter<int>("@UserID", userIDs[i], SqlDbType.Int);

                    db.ExecuteNonQuery();
                }
            }
        }

        [StoredProcedure(Name = "bx_Club_AcceptInvoke", Script = @"
CREATE PROCEDURE {name}
	@ClubID		int,
	@UserID		int
AS
BEGIN

	SET NOCOUNT ON;
	
	UPDATE bx_ClubMembers SET Status = 0, CreateDate = GETDATE() WHERE ClubID = @ClubID AND UserID = @UserID AND Status = 1;

	UPDATE bx_Clubs SET TotalMembers = TotalMembers + 1 WHERE ClubID = @ClubID;
END")]
        public override void AcceptClubInvite(int clubID, int userID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_AcceptInvoke";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);
                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Club_JoinClub", Script = @"
CREATE PROCEDURE {name}
	@ClubID		int,
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT * FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID = @UserID) BEGIN
		INSERT INTO bx_ClubMembers (ClubID, UserID, Status) VALUES (@ClubID, @UserID, 0);
	END;
END")]
        public override void JoinClub(int clubID, int userID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_JoinClub";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);
                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Club_PetitionJoinClub", Script = @"
CREATE PROCEDURE {name}
	@ClubID		int,
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT * FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID = @UserID) BEGIN
		INSERT INTO bx_ClubMembers (ClubID, UserID, Status) VALUES (@ClubID, @UserID, 2);
	END;
END")]
        public override void PetitionJoinClub(int clubID, int userID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_PetitionJoinClub";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);
                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Club_PetitionAsManager", Script = @"
CREATE PROCEDURE {name}
	@ClubID		int,
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE bx_ClubMembers SET Status = 6 WHERE ClubID = @ClubID AND UserID = @UserID;
END")]
        public override void PetitionClubManager(int clubID, int userID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_PetitionAsManager";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);
                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Club_IgnoreAllInvokes", Script = @"
CREATE PROCEDURE {name}
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM bx_ClubMembers WHERE Status = 1 AND UserID = @UserID;
END")]
        public override void IgnoreAllClubInvokes(int userID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_IgnoreAllInvokes";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Club_LeaveClub", Script = @"
CREATE PROCEDURE {name}
	@ClubID		int,
	@UserID		int
AS
BEGIN

	SET NOCOUNT ON;
	
	DELETE FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID = @UserID;

	UPDATE bx_Clubs SET TotalMembers = TotalMembers - 1 WHERE ClubID = @ClubID;
END")]
        public override void LeaveClub(int clubID, int userID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_LeaveClub";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);
                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Club_GetClubMemberStatus", Script = @"
CREATE PROCEDURE {name}
	@ClubID		int,
	@UserID		int
AS
BEGIN

	SET NOCOUNT ON;

	SELECT Status FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID = @UserID;

END")]
        public override ClubMemberStatus? GetClubMemberStatus(int clubID, int userID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Club_GetClubMemberStatus";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ClubID", clubID, SqlDbType.Int);
                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                byte? result = db.ExecuteScalar<byte?>();

                if (result == null)
                    return null;

                return (ClubMemberStatus)result.Value;
            }
        }
    }
}