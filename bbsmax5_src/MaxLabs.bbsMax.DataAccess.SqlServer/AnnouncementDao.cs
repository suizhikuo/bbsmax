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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class AnnouncementDao : DataAccess.AnnouncementDao
    {
        /// <summary>
        /// 取得单条公告
        /// </summary>
        /// <param name="AnnouncementID"></param>
        /// <returns></returns>
        public override Announcement GetAnnouncement(int AnnouncementID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_Announcements WHERE AnnouncementID = @AnnouncementID";
                query.CreateParameter<int>("@AnnouncementID", AnnouncementID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new Announcement(reader);
                }
            }
            return null;
        }

        public override void DeleteAnnouncement(int announcementID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_Announcements WHERE AnnouncementID=@AnnouncementID";
                query.CreateParameter<int>("@AnnouncementID", announcementID, SqlDbType.Int);
                query.ExecuteNonQuery();
            }
        }

        public override void DeleteAnnouncements(IEnumerable<int> announcementIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_Announcements WHERE AnnouncementID IN (@AnnouncementIDs)";
                query.CreateInParameter<int>("@AnnouncementIDs", announcementIds);
                query.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// 取得所有公告
        /// </summary>
        /// <returns></returns>
        public override AnnouncementCollection GetAnnouncements()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_Announcements ORDER BY SortOrder ASC, BeginDate ASC";
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AnnouncementCollection(reader);
                }
            }
        }

        /// <summary>
        /// 取得所有有效的公告， 就是没有过期的
        /// </summary>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_Announcement_GetAvailableAnnouncements", Script = @"
CREATE PROCEDURE {name}
 @AnnouncementID         int
,@PostUserID             int
,@Subject                nvarchar(200)
,@Content                ntext
,@AnnouncementType       tinyint
,@BeginDate              datetime
,@EndDate                datetime 
,@SortOrder              int
AS
BEGIN

    SET NOCOUNT ON;

    SELECT * FROM bx_Announcements WHERE EndDate > GETDATE() ORDER BY SortOrder ASC;

END
")]
        public override AnnouncementCollection GetAvailableAnnouncements()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Announcement_GetAvailableAnnouncements";

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AnnouncementCollection(reader);
                }
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_Announcement_Save", Script = @"
CREATE PROCEDURE {name}
 @AnnouncementID         int
,@PostUserID             int
,@Subject                nvarchar(200)
,@Content                ntext
,@AnnouncementType       tinyint
,@BeginDate              datetime
,@EndDate                datetime 
,@SortOrder              int
AS
BEGIN

    SET NOCOUNT ON;

    IF @AnnouncementID<>0 AND  @AnnouncementID  IS NOT NULL BEGIN
        UPDATE bx_Announcements SET PostUserID = @PostUserID, [Subject] = @Subject ,[Content] = @Content,AnnouncementType = @AnnouncementType, BeginDate = @BeginDate, EndDate = @EndDate,SortOrder = @SortOrder WHERE AnnouncementID = @AnnouncementID;
    END
    ELSE BEGIN
        INSERT INTO bx_Announcements(PostUserID, [Subject], [Content] , AnnouncementType , BeginDate, EndDate, SortOrder) VALUES( @PostUserID, @Subject,@Content, @AnnouncementType, @BeginDate, @EndDate, @SortOrder);
        SET @AnnouncementID = (SELECT @@Identity);
    END

    SELECT * FROM bx_Announcements WHERE AnnouncementID = @AnnouncementID;
END
")]
        #endregion
        public override Announcement SaveAnnouncement(int announcementID, int postUserID, string subject, string content, DateTime beginDate, DateTime endDate, MaxLabs.bbsMax.Enums.AnnouncementType announcementType, int sortOrder)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Announcement_Save";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@AnnouncementID", announcementID, SqlDbType.Int);
                query.CreateParameter<int>("@PostUserID", postUserID, SqlDbType.Int);
                query.CreateParameter<string>("@Subject", subject, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@Content", content, SqlDbType.NText);
                query.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);
                query.CreateParameter<int>("@SortOrder", sortOrder, SqlDbType.Int);
                query.CreateParameter<AnnouncementType>("@AnnouncementType", announcementType, SqlDbType.TinyInt);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new Announcement(reader);
                }
            }
            return null;
        }
    }
}