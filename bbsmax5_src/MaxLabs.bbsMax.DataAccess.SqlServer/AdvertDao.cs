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
using System.Data;
using MaxLabs.bbsMax.Entities;
using System.Data.SqlClient;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class AdvertDao : DataAccess.AdvertDao
    {

        #region 存储过程
        [StoredProcedure(Name = "bx_SaveAdvert", Script = @"
CREATE PROCEDURE {name} 
 @ADID            int
,@CategoryID      int
,@Position        tinyint
,@Index           int
,@AdType          tinyint
,@Available       bit
,@Title           nvarchar(50)
,@Href            nvarchar(500)
,@Text            nvarchar(200)
,@FontSize        int
,@Color           varchar(50)
,@Src             nvarchar(500)
,@Width           int
,@Height          int
,@BeginDate       datetime
,@EndDate         datetime
,@Targets         ntext
,@Code            ntext
,@Floor           varchar(1000)  
AS
BEGIN
SET NOCOUNT ON;

    IF @ADID IS NULL OR @ADID = 0 BEGIN
        INSERT INTO bx_Adverts( CategoryID, [Index], Position, Title, Href, [Text], Color, FontSize, ADType, Available, ResourceHref, Width, Height, BeginDate, EndDate, Code, Targets,Floor ) VALUES( @CategoryID, @Index, @Position, @Title, @Href, @Text, @Color, @FontSize, @AdType, @Available, @Src, @Width, @Height, @BeginDate, @EndDate, @Code, @Targets, @Floor );
        SET @ADID = (SELECT @@IDENTITY)
    END
    ELSE BEGIN
        UPDATE bx_Adverts SET CategoryID = @CategoryID, [Index] = @Index, Position = @Position, ADType = @AdType, Title = @Title, [Text] = @Text, Available = @Available, Href = @Href, ResourceHref = @Src, FontSize = @FontSize, Color = @Color, Width = @Width, Height = @Height, BeginDate = @BeginDate, EndDate = @EndDate, Code = @Code, Targets = @Targets, Floor = @Floor  WHERE ADID = @ADID;       
    END
    
    SELECT * FROM bx_Adverts WHERE [ADID] = @ADID;
END
")]
        #endregion
        public override Advert SaveAdvert(
              int id, int index, int categoryID, ADPosition position, ADType adType, bool available, string title
            , string href, string text, int fontsize, string color, string src
            , int width, int height, DateTime beginDate, DateTime endDate, string code, string targets, string floor)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SaveAdvert";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ADID", id, SqlDbType.Int);
                query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);
                query.CreateParameter<byte>("@Position", (byte)position, SqlDbType.TinyInt);
                query.CreateParameter<byte>("@AdType", (byte)adType, SqlDbType.TinyInt);
                query.CreateParameter<bool>("@Available", available, SqlDbType.Bit);
                query.CreateParameter<string>("@Title", title, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Href", href, SqlDbType.NVarChar, 500);
                query.CreateParameter<string>("@Text", text, SqlDbType.NVarChar, 200);
                query.CreateParameter<int>("@FontSize", fontsize, SqlDbType.Int);
                query.CreateParameter<string>("@Color", color, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@Src", src, SqlDbType.NVarChar, 500);
                query.CreateParameter<int>("@Height", height, SqlDbType.Int);
                query.CreateParameter<int>("@Width", width, SqlDbType.Int);
                query.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);
                query.CreateParameter<string>("@Code", code, SqlDbType.NText);
                query.CreateParameter<string>("@Targets", targets, SqlDbType.NText);
                query.CreateParameter<int>("@Index", index, SqlDbType.Int);
                query.CreateParameter<string>("@Floor", floor, SqlDbType.VarChar, 1000);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Advert(reader);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 批量启用不启用
        /// </summary>
        /// <param name="adids"></param>
        /// <param name="available"></param>
        public override void SetAdvertAvailable(IEnumerable<int> adids, bool available)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_Adverts SET Available = @Available WHERE ADID IN (@ADIds)";
                query.CommandType = CommandType.Text;

                query.CreateParameter<bool>("@Available", available, SqlDbType.Bit);
                query.CreateInParameter<int>("@ADIds", adids);

                query.ExecuteNonQuery();
            }
        }

        public override AdvertCollection GetAdverts(int categoryID, ADPosition adPosition, int pageSize, int pageNumber, out int totalCount)
        {
            totalCount = 0;

            using (SqlQuery query = new SqlQuery())
            {

                query.Pager.TableName = "bx_Adverts";
                query.Pager.SortField = "ADID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.PageSize = Consts.DefaultPageSize;
                query.Pager.SelectCount = true;
                query.Pager.ResultFields = "*";

                if (categoryID != 0)
                {
                    query.Pager.Condition = "CategoryID = @CategoryID";
                    query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);
                }

                if (adPosition != ADPosition.None)
                {
                    query.Pager.Condition += " AND [Position] = @Position";
                    query.CreateParameter<byte>("@Position", (byte)adPosition, SqlDbType.TinyInt);
                }

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    AdvertCollection ads = new AdvertCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                    return ads;
                }
            }
        }

        public override AdvertCollection GetAdverts()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_Adverts";
                query.CommandType = CommandType.Text;

         
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AdvertCollection(reader);
                }
            }
        }

        public override AdvertCollection GetAdverts(IEnumerable<int> adids)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_Adverts WHERE ADID IN ( @ADIDs )";
                query.CommandType = CommandType.Text;

                query.CreateInParameter<int>("@ADIDs", adids);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AdvertCollection(reader);
                }
            }
        }

        public override AdvertCollection GetAdverts(int categoryID, IEnumerable<int> excludeAdverts)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_Adverts WHERE CategoryID = @CategoryID AND Available = 1 AND ADID NOT IN ( @ADIDs )";
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);
                query.CreateInParameter<int>("@ADIDs", excludeAdverts);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AdvertCollection(reader);
                }
            }
        }

        /// <summary>
        /// 取得单条广告
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Advert GetAdvert(int id)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_Adverts WHERE ADID = @ID";

                query.CreateParameter<int>("@ID", id, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Advert(reader);
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// 根据类型取得广告
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public override AdvertCollection GetAdvertByCategory(int categoryID, bool avalible)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string conditions = null;

                if (categoryID != 0)
                {
                    conditions = "CategoryID = @CategoryID";
                    query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);
                }

                if (avalible)
                {
                    if (conditions == null)
                        conditions += " EndDate > GETDATE() AND Available = 1";
                    else
                        conditions += " AND EndDate > GETDATE() AND Available = 1";
                }

                if (conditions == null)
                    query.CommandText = "SELECT * FROM bx_Adverts";
                else
                    query.CommandText = "SELECT * FROM bx_Adverts WHERE " + conditions;

                query.CommandType = CommandType.Text;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AdvertCollection(reader);
                }
            }
        }

        public override AdvertCollection GetAdvertByCategory(int categoryID)
        {
            return GetAdvertByCategory(categoryID, true);
        }

        public override void DeleteAdverts(IEnumerable<int> adIds)
        {
            string sql = @"DELETE FROM bx_Adverts WHERE ADID IN (@Ids);";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql;

                query.CreateInParameter<int>("@Ids", adIds);

                query.ExecuteNonQuery();
            }
        }

        //public override ADCategory GetAdvertCategory(int classID)
        //{
        //    return null;
        //}

        //public override ADCategoryCollection GetAdvertCategorys()
        //{
        //    return new ADCategoryCollection();
        //}
    }
}