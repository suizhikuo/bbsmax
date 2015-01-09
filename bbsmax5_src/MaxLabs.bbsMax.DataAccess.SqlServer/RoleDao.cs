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
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class RoleDao:DataAccess.RoleDao
    {
  
        public override void DeleteRole(int roleid)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM [bx_Roles] WHERE RoleID=@RoleID";
                query.CreateParameter<int>("@RoleID", roleid, SqlDbType.Int);
                query.ExecuteNonQuery();
            }
        }

        public override RoleCollection GetAllRoles()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_Roles";
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    RoleCollection rolegroups = new RoleCollection(reader);
                    return rolegroups;
                }
            }
        }

        public override Role GetRoleByID(int id)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_Roles WHERE RoleID=@RoleID";
                query.CreateParameter("@RoleID", id, SqlDbType.Int);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Next)
                        return new Role(reader);
                    else
                        return null;
                }
            }
        }

        [StoredProcedure(Name="bx_AddRole",Script= @"
CREATE PROCEDURE {name}
    @RoleID         int,
    @Name           nvarchar(50),
    @Title          nvarchar(50),
    @Color          varchar(50),
    @IconUrl        varchar(200),
    @RoleType       int,
    @Level          int,
    @StarLevel      int,
    @RequiredPoint  int
AS
BEGIN
    
    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM bx_Roles WHERE RoleID=@RoleID)
        
        UPDATE bx_Roles SET [Name]=@Name,Title=@Title,Color=@Color,IconUrl=@IconUrl,RoleType=@RoleType,[Level]=@Level,StarLevel=@StarLevel,RequiredPoint=@RequiredPoint WHERE RoleID=@RoleID;
    
    ELSE

        INSERT INTO [bx_Roles]([Name],Title,Color,IconUrl,RoleType,[Level],StarLevel,RequiredPoint) VALUES(@Name,@Title,@Color,@IconUrl,@RoleType,@Level,@StarLevel,@RequiredPoint);

    END
    
")]

       public override void AddRole(int roleID,string name,string title,string color,string iconUrl,RoleType type,int level,int starLevel,int requiredPoint)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_AddRole";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@RoleID", roleID, SqlDbType.Int);
                query.CreateParameter<string>("@Name", name, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Title",title, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Color", color, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@IconUrl", iconUrl, SqlDbType.VarChar, 200);
                query.CreateParameter<int>("@RoleType", (int)type, SqlDbType.TinyInt);
                query.CreateParameter<int>("@Level",level, SqlDbType.Int);
                query.CreateParameter<int>("@StarLevel", starLevel, SqlDbType.Int);
                query.CreateParameter<int>("@RequiredPoint", requiredPoint, SqlDbType.Int);

                query.ExecuteNonQuery();
               
            }
        }



        [StoredProcedure(Name = "bx_AddUserToRole_xh", Script = @"
CREATE PROCEDURE {name}
     @UserID      int
    ,@RoleID      int
    ,@BeginDate   datetime
    ,@EndDate     datetime
AS BEGIN

    SET NOCOUNT ON;

    IF EXISTS ( SELECT * FROM bx_Users WHERE UserID = @UserID ) BEGIN

        IF EXISTS ( SELECT * FROM bx_UsersInRoles WHERE UserID = @UserID AND RoleID = @RoleID )
            UPDATE bx_UsersInRoles SET BeginDate = @BeginDate, EndDate = @EndDate WHERE UserID = @UserID AND RoleID = @RoleID;
        ELSE
            INSERT INTO bx_UsersInRoles (UserID, RoleID, BeginDate, EndDate) VALUES (@UserID, @RoleID, @BeginDate, @EndDate);

    END

END
")]
        public override int AddUsersToRoles(RoleUserCollection roleUsers)
        {
            using (SqlQuery query = new SqlQuery())
            {
                int count=0;
                query.CommandText = "bx_AddUserToRole_xh";
                query.CommandType = CommandType.StoredProcedure;
                foreach (RoleUser item in roleUsers)
                {
                    query.CreateParameter<int>("@UserID", item.UserID, SqlDbType.Int);
                    query.CreateParameter<int>("@RoleID",item.RoleID,SqlDbType.Int);
                    query.CreateParameter<DateTime>("@BeginDate", item.BeginDate, SqlDbType.DateTime);
                    query.CreateParameter<DateTime>("@EndDate", item.EndDate, SqlDbType.DateTime);

                    count += query.ExecuteNonQuery();
                }
                return count;
            }

            
        }


        private void AddUsersToRoles(IEnumerable<string> userName, IEnumerable<string> roleName)
        { 
        
            
        }


        public override int RemoveUsersFromRoles(IEnumerable<int> userIds, IEnumerable<int> roleIds)
        {
            if (userIds == null || ValidateUtil.HasItems(userIds) == false)
                return 0;

            if (roleIds == null || ValidateUtil.HasItems(roleIds) == false)
                return 0;

            int rows = 0;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE bx_UsersInRoles WHERE RoleID=@RoleID AND UserID IN (@UserIds)";

                foreach (int roleID in roleIds)
                {
                    query.CreateParameter<int>("@RoleID",roleID,SqlDbType.Int);

                    query.CreateInParameter<int>("@UserIds", userIds);

                    rows += query.ExecuteNonQuery();
                }
            }

            return rows;
        }

        public override UserCollection GetRoleMembers(int roleID, int pageSize, int pageNumber, out int totalCount)
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
                query.Pager.Condition = " UserID IN( SELECT UserID FROM bx_UsersInRoles WHERE RoleID = @RoleID AND BeginDate <= GETDATE() AND EndDate >= GETDATE())";
                query.CreateParameter<int>("@RoleID", roleID, SqlDbType.Int);

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

        public override UserRole GetUserRole(int userID, Guid roleID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_UserRoles WHERE UserID=@UserID AND RoleID=@RoleID";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<Guid>("@RoleID", roleID, SqlDbType.UniqueIdentifier);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserRole userRole = null;
                    if (reader.Next)
                    {
                        userRole = new UserRole(reader);
                    }
                    return userRole;
                }
            }
        }

    }
}