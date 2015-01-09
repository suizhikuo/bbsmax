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
    public class WebsiteDao : DataAccess.WebsiteDao
    {
        #region 存储过程
        [StoredProcedure(Name = "bx_Chinaz_DeleteWebsite", Script = @"
        CREATE PROCEDURE {name} 
        @WebsiteID    int
        AS

        BEGIN
        SET NOCOUNT ON;

            DELETE FROM Chinaz_Websites WHERE WebsiteID = @WebsiteID;

            IF @@ROWCOUNT > 0
                RETURN 1;
        
        RETURN 0;

        END
        ")]
        #endregion
        public override bool DeleteWebsite(int websiteID)
        {
            using (SqlQuery query = new SqlQuery())
            {

                query.CommandText = "bx_Chinaz_DeleteWebsite";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@WebsiteID", websiteID, SqlDbType.Int);
                query.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);
                query.ExecuteNonQuery();

                return (int)query.Parameters[1].Value > 0;

            }
        }

        

        public override Website GetWebsite(string url)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.Text;
                query.CommandText = "SELECT * FROM Chinaz_Websites WHERE Url = @Url";
                query.CreateParameter<string>("@Url", url, SqlDbType.NVarChar, 200);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    Website website = null;
                    while (reader.Next)
                    {
                        website = new Website(reader);
                    }
                    return website;
                }
            }
        }


        public override List<Website> GetWebsites(int pageSize, int pageNumber, out int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "Chinaz_Websites";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.PrimaryKey = "WebsiteID";
                query.Pager.SortField = "WebsiteID";
                query.Pager.SelectCount = true;
                query.Pager.IsDesc = true;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    totalCount = 0;
                    List<Website> results = new List<Website>();
                    while (reader.Next)
                    {
                        results.Add(new Website(reader));
                    }
                    if (reader.NextResult())
                        while (reader.Next)
                            totalCount = reader.Get<int>(00);
                    return results;
                }
            }
        }



        public override UserWebsiteCollection GetUserWebsites(int userID, IEnumerable<int> websiteIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM Chinaz_UserWebsitesView WHERE UserID = @UserID AND WebsiteID IN( @WebsiteIDs)";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateInParameter<int>("@WebsiteIDs", websiteIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserWebsiteCollection result = new UserWebsiteCollection(reader);
                    return result;
                }
            }
        }

        /// <summary>
        /// 该重载只获取用户网站列表，不获取网站消息列表和网站服务列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageNumber"></param>
        /// <param name="isGetMessages"></param>
        /// <param name="isGetServices"></param>
        /// <returns></returns>
        public override UserWebsiteCollection GetUserWebsites(int userID, int pageSize, int pageNumber)
        {
            return GetUserWebsites(userID, pageSize, pageNumber, false, false);
        }

        /// <summary>
        /// 获取用户网站列表信息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pageSize"></param>
        /// <param name="isGetMessages">是否获取消息列表</param>
        /// <param name="isGetServices">是否获取服务列表</param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public override UserWebsiteCollection GetUserWebsites(int userID, int pageSize, int pageNumber, bool isGetMessages, bool isGetServices)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "Chinaz_UserWebsitesView";
                query.Pager.PageSize = pageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PrimaryKey = "UserWebsiteID";
                query.Pager.SelectCount = true;
                query.Pager.SortField = "UserWebsiteID";
                query.Pager.IsDesc = true;

                query.Pager.Condition = "UserID = @UserID";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                UserWebsiteCollection result = new UserWebsiteCollection();
                using (XSqlDataReader reader = query.ExecuteReader())
                {  
                    result = new UserWebsiteCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Next)
                            result.TotalRecords = reader.Get<int>(0);
                    }
                }
                


                if (isGetMessages || isGetServices)
                {
                    query.CommandText = "";
                    if (isGetMessages)
                    {
                        if (result.Count > 0)
                        {
                            WebsiteService service = null;
                            string url;
                            foreach (UserWebsite web in result)
                            {
                                for (int i = 0; i < WebsiteServiceList.ServiceList.Count; i++)
                                {
                                    service = WebsiteServiceList.ServiceList[i];
                                    url = RemoveUrlPrefix(web.Url);
                                    web.Services.Add(service.ID, new WebsiteService(service.ID, service.Content, string.Format(service.Url, url), service.LightImgPath, service.UnLightImgPath, service.IsPreView, service.OnLight));
                                }
                            }

                        }
                    }
                    if (isGetMessages)
                    {
                        query.CommandText += "SELECT *  FROM Chinaz_UserWebsiteMessages WHERE UserWebsiteID IN(@UserWebsiteIDs) Order BY UserWebsiteID;";
                    }
                    
                    if(isGetServices)
                    {
                        query.CommandText += "SELECT *  FROM Chinaz_UserWebsiteServices WHERE UserWebsiteID IN(@UserWebsiteIDs) Order BY UserWebsiteID;";
                    }

                    query.CommandType = CommandType.Text;

                    query.CreateInParameter<int>("@UserWebsiteIDs", result.GetKeys());

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        UserWebsite site = null;
                        if (isGetMessages)
                        {
                            while (reader.Next)
                            {
                                UserWebsiteMessage message = new UserWebsiteMessage(reader);
                                if (site != null && site.UserWebsiteID == message.UserWebsiteID)
                                    site.Messages.Add(message);
                                else
                                {
                                    site = result.GetValue(message.UserWebsiteID);
                                    site.Messages.Add(message);
                                }
                            }
                        }

                        if (isGetServices)
                        {
                            if (isGetMessages == false)
                                reader.NextResult();

                            int websiteid, serviceid;
                            while (reader.Next)
                            {
                                serviceid = reader.Get<int>("ServiceID");
                                websiteid = reader.Get<int>("UserWebsiteID");
                                if (site != null && site.UserWebsiteID == websiteid)
                                    site.Services[serviceid].OnLight = true;
                                else
                                {
                                    site = result.GetValue(websiteid);
                                    site.Services[serviceid].OnLight = true;
                                }

                            }
                        }
                    }

                }

                return result;

            }
        }

        public override UserWebsiteCollection AdminSearchUserWebsites(AdminWebsiteFilter filter,int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string SordField = filter.Order.ToString();

                StringBuilder condition = new StringBuilder();

                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.Username))
                    {
                        condition.Append(" AND UserID IN( SELECT UserID FROM bx_Users WHERE Username LIKE '%' + @Username +'%')");
                        query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                    }
                    if (filter.IsHaveImage < 2)
                    {
                        condition.Append(" AND IsHaveImage = @IsHaveImage");
                        query.CreateParameter<bool>("@IsHaveImage", (filter.IsHaveImage == 1), SqlDbType.Bit);
                    }
                    if (!string.IsNullOrEmpty(filter.WebsiteName))
                    {
                        condition.Append(" AND WebsiteName LIKE '%' + @WebsiteName +'%'");
                        query.CreateParameter<string>("@WebsiteName", filter.WebsiteName, SqlDbType.NVarChar, 50);
                    }
                    if (!string.IsNullOrEmpty(filter.Url))
                    {
                        condition.Append(" AND Url LIKE '%' + @Url + '%'");
                        query.CreateParameter<string>("@Url", filter.Url, SqlDbType.NVarChar, 200);
                    }
                    if (filter.categoryID > 0)
                    {
                        condition.Append(" AND CategoryID = @CategoryID");
                        query.CreateParameter<int>("@CategoryID", filter.categoryID, SqlDbType.Int);
                    }
                    if (filter.IsVerified < 2)
                    {
                        condition.Append(" AND Verified = @Verified");
                        query.CreateParameter<bool>("@Verified", (filter.IsVerified == 1), SqlDbType.Bit);
                    }
                    if (filter.BeginDate != null)
                    {
                        condition.Append(" AND CreateDate >= @BeginDate");
                        query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                    }

                    if (filter.EndDate != null)
                    {
                        condition.Append(" AND CreateDate <= @EndDate");
                        query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                    }
                }
                if (condition.Length > 5)
                    condition.Remove(0, 5);

                query.Pager.TableName = "Chinaz_UserWebsitesView";
                query.Pager.PageSize = filter.Pagesize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PrimaryKey = "UserWebsiteID";
                query.Pager.SortField = SordField;
                query.Pager.Condition = condition.ToString();
                query.Pager.SelectCount = true;
                query.Pager.IsDesc = filter.IsDesc;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserWebsiteCollection websitelist = new UserWebsiteCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Next)
                            websitelist.TotalRecords = reader.Get<int>(0);
                    }

                    return websitelist;
                }
            }
        }

        public override bool AdminUpdateUserWebsite(int userWebsiteID, string websiteName, int categoryID, bool isVerified, string websiteIntro)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE Chinaz_UserWebsites SET WebsiteName = @WebsiteName,CategoryID = @CategoryID,Verified = @Verified,WebsiteIntro = @WebsiteIntro WHERE UserWebsiteID = @UserWebsiteID";
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@UserWebsiteID", userWebsiteID, SqlDbType.Int);
                query.CreateParameter<string>("@WebsiteName", websiteName, SqlDbType.NVarChar, 50);
                query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);
                query.CreateParameter<bool>("@Verified", isVerified, SqlDbType.Bit);
                query.CreateParameter<string>("@WebsiteIntro", websiteIntro, SqlDbType.NVarChar, 200);
                return query.ExecuteNonQuery() > 0;
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_Chinaz_DeleteWebsiteItem", Script = @"
CREATE PROCEDURE {name}
@UserID          int
,@WebsiteID      int
AS
BEGIN
SET NOCOUNT ON;

    DELETE FROM Chinaz_UserWebsites WHERE UserID = @UserID AND WebsiteID = @WebsiteID;

IF @@ROWCOUNT>0 
    RETURN 1;

RETURN 0;

END
")]
        #endregion
        public override List<int> DeleteUserWebsites(int userID, IEnumerable<int> websiteIds)
        {
            List<int> list = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
DELETE FROM Chinaz_UserWebsites WHERE UserID = @UserID AND WebsiteID IN(@WebsiteIDs);
DECLARE @Temp table(id int);
";

                StringBuilder sql = new StringBuilder();
                foreach (int id in websiteIds)
                {
                    sql.AppendFormat(@"
            IF NOT EXISTS(SELECT *  FROM Chinaz_UserWebsites WHERE WebsiteID = @WebsiteID_{0}) BEGIN
                DELETE FROM Chinaz_Websites WHERE WebsiteID = @WebsiteID_{0};
                INSERT INTO @Temp(id) VALUES(@WebsiteID_{0});
            END
", id);
                    query.CreateParameter<int>(string.Format("@WebsiteID_{0}", id), id, SqlDbType.Int);
                }

                sql.Append("SELECT id FROM @Temp;");
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateInParameter<int>("@WebsiteIDs", websiteIds);

                query.CommandText += sql.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Next)
                    {
                        if (list == null)
                            list = new List<int>();
                        list.Add(reader.Get<int>("id"));
                    }
                }
            }
            return list;
        }

        public override List<int> AdminDeleteUserWebsites(IEnumerable<int> userWebsiteIDs)
        {
            List<int> list = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"DECLARE @Temp table(id int);";
                StringBuilder sql = new StringBuilder();
                foreach (int id in userWebsiteIDs)
                {
                    sql.AppendFormat(@"
            DECLARE @WebsiteID_{0} int;
            SELECT @WebsiteID_{0} = WebsiteID From Chinaz_UserWebsites where UserWebsiteID = @UserWebsiteID_{0};
            DELETE FROM Chinaz_UserWebsites WHERE UserWebsiteID = @UserWebsiteID_{0};
            IF NOT EXISTS(SELECT  *  FROM Chinaz_UserWebsites WHERE WebsiteID = @WebsiteID_{0})
            BEGIN
                DELETE FROM Chinaz_Websites WHERE WebsiteID = @WebsiteID_{0};
                INSERT INTO @Temp(id) VALUES(@WebsiteID_{0});
            END
", id);
                    query.CreateParameter<int>(string.Format("@UserWebsiteID_{0}", id), id, SqlDbType.Int);
                }

                sql.Append(@"SELECT id FROM @Temp;");
                query.CommandText += sql.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Next)
                    {
                        if (list == null)
                            list = new List<int>();
                        list.Add(reader.Get<int>("id"));
                    }
                }
            }
            return list;
        }

        public override bool DeleteUserWebsite(int userID, int websiteID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Chinaz_DeleteWebsiteItem";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@WebsiteID", websiteID, SqlDbType.Int);
                query.CreateParameter<int>("@Return", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (int)query.Parameters[2].Value > 0;
            }
        }

        public override UserWebsite GetUserWebsite(int userID, int websiteID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.Text;
                query.CommandText = "SELECT * FROM Chinaz_UserWebsitesView WHERE UserID = @UserID AND WebsiteID = @WebsiteID;";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@WebsiteID", websiteID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserWebsite result = null;
                    while (reader.Next)
                    {
                        result = new UserWebsite(reader);
                    }
                    return result;
                }
            }
        }

        public override UserWebsite AdminGetUserWebsite(int userWebsiteID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.Text;
                query.CommandText = "SELECT * FROM Chinaz_UserWebsitesView WHERE UserWebsiteID = @UserWebsiteID";
                query.CreateParameter<int>("@UserWebsiteID", userWebsiteID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserWebsite result = null;
                    while (reader.Next)
                    {
                        result = new UserWebsite(reader);
                    }
                    return result;
                }
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_Chinaz_CreateWebsiteItem", Script = @"
CREATE PROCEDURE {name}
@UserID   int
,@CategoryID   int
,@Url          nvarchar(200)
,@WebsiteName   nvarchar(50)
,@VerifyFilename varchar(50)
,@WebsiteIntro   nvarchar(200)
AS
BEGIN

SET NOCOUNT ON;

    DECLARE @WebsiteID int;

    SET @WebsiteID =(SELECT WebsiteID FROM Chinaz_Websites WHERE Url = @Url);
    
    IF @WebsiteID IS NULL BEGIN
        INSERT INTO  Chinaz_Websites( Url ) VALUES( @Url );
        SET @WebsiteID = @@Identity;
    END

    IF EXISTS( SELECT * FROM Chinaz_UserWebsites WHERE UserID = @UserID AND WebsiteID = @WebsiteID) BEGIN
        UPDATE Chinaz_UserWebsites SET CategoryID = @CategoryID , WebsiteName = @WebsiteName,WebsiteIntro = @WebsiteIntro WHERE UserID = @UserID AND WebsiteID = @WebsiteID;
    END
    ELSE BEGIN
        INSERT INTO Chinaz_UserWebsites(UserID,WebsiteID,CategoryID, WebsiteName, VerifyFilename,WebsiteIntro) VALUES( @UserID,@WebsiteID, @CategoryID, @WebsiteName,@VerifyFilename,@WebsiteIntro);
    END    

    SELECT * FROM Chinaz_UserWebsitesView WHERE UserID = @UserID AND WebsiteID = @WebsiteID;
END
")]
        #endregion
        public override UserWebsite CreateUserWebsite(int userID, int categoryID, string url, string websiteName, string verifyFilename, string websiteIntro)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Chinaz_CreateWebsiteItem";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Url", url, SqlDbType.NVarChar, 200);
                query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);
                query.CreateParameter<string>("@WebsiteName", websiteName, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@VerifyFilename", verifyFilename, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@WebsiteIntro", websiteIntro, SqlDbType.NVarChar, 200);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserWebsite item = null;
                    while (reader.Next)
                        item = new UserWebsite(reader);
                    return item;
                }
            }
        }

        public override bool UpdateUserWebsite(int userID, int websiteID, int categoryID, string websiteName, string websiteIntro)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE Chinaz_UserWebsites SET CategoryID = @CategoryID,WebsiteName = @WebsiteName,WebsiteIntro = @WebsiteIntro WHERE UserID = @UserID AND WebsiteID = @WebsiteID;";
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@WebsiteID", websiteID, SqlDbType.Int);
                query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);
                query.CreateParameter<string>("@WebsiteName", websiteName, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@WebsiteIntro", websiteIntro, SqlDbType.NVarChar, 200);
                return query.ExecuteNonQuery() > 0;
            }
        }

        public override bool SetUserWebsiteVerified(int userID, int websiteID, bool isVerified)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE Chinaz_UserWebsites SET Verified = @Verified WHERE UserID = @UserID AND WebsiteID = @WebsiteID;";
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@WebsiteID", websiteID, SqlDbType.Int);
                query.CreateParameter<bool>("@Verified", isVerified, SqlDbType.Bit);
                return query.ExecuteNonQuery() > 0;
            }
        }

        public override bool UpdateUserWebsiteImageUrl(int websiteID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE Chinaz_Websites SET IsHaveImage = 1 WHERE  WebsiteID = @WebsiteID;";
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@WebsiteID", websiteID, SqlDbType.Int);
                return query.ExecuteNonQuery() > 0;
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_Chinaz_UpdateWebsiteMessage", Script = @"
CREATE PROCEDURE {name}
@UserWebsiteID int
,@Key nvarchar(50)
,@Content nvarchar(200)
AS
BEGIN

SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM Chinaz_UserWebsiteMessages WHERE UserWebsiteID = @UserWebsiteID AND [Key] = @Key) BEGIN
        UPDATE Chinaz_UserWebsiteMessages SET [Key] = @Key,Content = @Content WHERE UserWebsiteID = @UserWebsiteID;
    END
    ELSE BEGIN
        INSERT INTO Chinaz_UserWebsiteMessages(UserWebsiteID,[Key],Content) Values(@UserWebsiteID,@Key,@Content);
    END

END
")]
        #endregion
        public override void UpdateUserWebsiteMessage(int userWebsiteID, string key, string content)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Chinaz_UpdateWebsiteMessage";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserWebsiteID", userWebsiteID, SqlDbType.Int);
                query.CreateParameter<string>("@Key", key, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@Content", content, SqlDbType.VarChar, 200);
                query.ExecuteNonQuery();
            }
        }


        public override List<int> GetWebsiteServicsIDs(int userWebsiteID)
        {
            List<int> list = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT [ServiceID] FROM [Chinaz_UserWebsiteServices] WHERE [UserWebsiteID]=@UserWebsiteID";
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@UserWebsiteID", userWebsiteID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Next)
                    {
                        if (list == null)
                            list = new List<int>();
                        list.Add(reader.Get<int>("ServiceID"));
                    }
                }

            }
            return list;
        }

        private string RemoveUrlPrefix(string url)
        {
            if (url.StartsWith("http://"))
                url = url.Substring(url.IndexOf("http://"));

            if(url.StartsWith("https://"))
                url= url.Substring(url.IndexOf("https://"));

            return url;
        
        }

    }
}