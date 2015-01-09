//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class TagDao : DataAccess.TagDao
    {
		public override TagCollection GetUserTags(int userID, TagType type)
		{
			switch (type)
			{
				case TagType.Blog:
					return GetUserBlogTags(userID);

				default:
					return new TagCollection();
			}
		}

		#region StoredProcuedure
		[StoredProcedure(Name = "bx_Tag_GetUserBlogTags", Script = @"
CREATE PROCEDURE {name} 
	@UserID int
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM
        [bx_Tags]
    WHERE
        [ID] IN (
			SELECT [TagID] FROM [bx_TagRelation] WHERE [Type] = 1 AND [TargetID] IN (
				SELECT [ArticleID] FROM [bx_BlogArticles] WHERE [UserID] = @UserID
			)
		)
    AND 
        [IsLock] = 0;

END")]
		#endregion
		private TagCollection GetUserBlogTags(int userID)
		{
			using (SqlQuery db = new SqlQuery())
			{
				db.CommandText = "bx_Tag_GetUserBlogTags";
				db.CommandType = CommandType.StoredProcedure;

				db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

				using (XSqlDataReader reader = db.ExecuteReader())
				{
                    return new TagCollection(reader);
				}
			}
		}

        #region 获取数据

        /// <summary>
        /// 获取指定标签
        /// </summary>
        /// <param name="tagID">指定标签的ID</param>
        #region StoredProcedure
        [StoredProcedure(Name = "bx_GetTag", Script = @"
CREATE PROCEDURE {name}
    @ID          int
AS BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [bx_Tags] WHERE [ID] = @ID;
END")]
        #endregion
        public override Tag GetTag(int tagID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetTag";
                query.CommandType = CommandType.StoredProcedure;

				query.CreateParameter<int>("@ID", tagID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Tag(reader);
                    }
                    return new Tag();
                }
            }
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        public override TagCollection GetAllTags(int pageNumber, int pageSize, ref int? count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Tags";
                query.Pager.SortField = "ID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = count;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    TagCollection tags = new TagCollection(reader);

                    if (count == null && reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            count = reader.Get<int>(0);
                        }
                    }
                    return tags;
                }
            }
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        public override TagCollection GetMostTags(bool isLock, int pageNumber, int pageSize, ref int? count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Tags";
                query.Pager.SortField = "ID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = count;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.Condition = "[IsLock] = @IsLock";

               
				query.CreateParameter<bool>("@IsLock" , isLock, SqlDbType.Bit);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    TagCollection tags = new TagCollection(reader);

                    if (count == null && reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            count = reader.Get<int>(0);
                        }
                    }
                    return tags;
                }
            }
        }

        /// <summary>
        /// 获取指定类型的标签
        /// </summary>
        /// <param name="type">类型,如日志标签等</param>
        public override TagCollection GetTags(TagType type, int pageNumber, int pageSize, ref int? count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Tags";
                query.Pager.SortField = "ID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = count;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.Condition = "[ID] IN (SELECT [TagID] FROM [bx_TagRelation] WHERE [Type] = @Type)";

				query.CreateParameter<TagType>("@Type", type, SqlDbType.TinyInt);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    TagCollection tags = new TagCollection(reader);

                    if (count == null && reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            count = reader.Get<int>(0);
                        }
                    }
                    return tags;
                }
            }
        }

        /// <summary>
        /// 获取指定类型指定对象的标签
        /// </summary>
        /// <param name="type">标签类型</param>
        /// <param name="targetID">对象ID</param>
        #region StoredProcedure
        [StoredProcedure(Name = "bx_GetTagsByTargetID", Script = @"
CREATE PROCEDURE {name}
    @Type      tinyint,
    @TargetID  int
AS BEGIN
    SET NOCOUNT ON;
    SELECT * FROM
        [bx_Tags]
    WHERE
        [ID] IN (SELECT [TagID] FROM [bx_TagRelation] WHERE [Type] = @Type AND [TargetID] = @TargetID)
    AND 
        [IsLock] = 0;
END
")]
        #endregion
        public override TagCollection GetTags(TagType type, int targetID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetTagsByTargetID";

                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<TagType>("@Type", type, SqlDbType.TinyInt);
				query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
                

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new TagCollection(reader);
                }
            }
        }

        /// <summary>
        /// 搜索标签
        /// </summary>
        public override TagCollection GetTagsByFilter(AdminTagFilter filter, int pageNumber, ref int? count)
        {
            
            using (SqlQuery query = new SqlQuery())
            {

                string conditions = BuildConditionsByFilter(query, filter);

                query.Pager.TableName = "bx_Tags";
                
                //只有要排序的字段不唯一的时候才需要指定主键
                if (filter.OrderBy != TagOrderKey.ID)
                    query.Pager.PrimaryKey = "ID";

                query.Pager.SortField = filter.OrderBy.ToString();
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.TotalRecords = count;
                query.Pager.IsDesc = filter.IsDesc;
                query.Pager.SelectCount = true;
                query.Pager.Condition = conditions;


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    TagCollection tags = new TagCollection(reader);

                    if (count == null && reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            count = reader.Get<int>(0);
                        }
                    }
                    return tags;
                }
            }
        }

        #endregion

        #region 操作数据

        /// <summary>
        /// 保存标签
        /// </summary>
        /// <param name="tags">标签集</param>
        /// <param name="type">类型,如日志标签等</param>
        /// <param name="targetID">标签的对象ID</param>
        public override bool SaveTags(TagCollection tags, TagType type, int targetID)
        {
			using (SqlSession db = new SqlSession())
			{
				using (SqlQuery query = db.CreateQuery())
				{

					query.CommandText = @"DELETE FROM [bx_TagRelation] WHERE [Type]=@Type AND [TargetID] = @TargetID;";

					query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
					query.CreateParameter<TagType>("@Type", type, SqlDbType.TinyInt);

					query.ExecuteNonQuery();
				}

				if (tags == null || tags.Count == 0)
					return true;
				
				using (SqlQuery query = db.CreateQuery(QueryMode.Prepare))
				{

					query.CommandText = @"
IF EXISTS (SELECT * FROM [bx_Tags] WHERE [Name] = @TagName) BEGIN
    DECLARE   @TagID   int;
    SET @TagID = (SELECT TOP 1 [ID] FROM [bx_Tags] WHERE [Name] = @TagName);
    DECLARE  @PrivacyType    tinyint;
    IF @Type = 1 
        SET @PrivacyType = (SELECT [PrivacyType] FROM [bx_BlogArticles] WHERE [ArticleID] = @TargetID);
    ELSE 
        SET @PrivacyType = 0;
        
    IF NOT EXISTS (SELECT * FROM [bx_TagRelation] WHERE [TagID] = @TagID) BEGIN
        IF @PrivacyType = 0 OR @PrivacyType = 3 BEGIN
            INSERT INTO [bx_TagRelation]([TagID], [Type], [TargetID]) VALUES(@TagID, @Type, @TargetID);
          END
      END
    ELSE BEGIN
        IF EXISTS (SELECT * FROM [bx_TagRelation] WHERE [TagID] = @TagID AND [Type] = @Type AND [TargetID] = @TargetID) BEGIN
            IF @PrivacyType <> 0 AND @PrivacyType <> 3 BEGIN
                DELETE FROM [bx_TagRelation] WHERE [TagID] = @TagID AND [Type] = @Type AND [TargetID] = @TargetID;
              END
          END
        ELSE BEGIN
            IF @PrivacyType = 0 OR @PrivacyType = 3 BEGIN
                INSERT INTO [bx_TagRelation]([TagID], [Type], [TargetID]) VALUES(@TagID, @Type, @TargetID);
              END
          END
      END
  END
ELSE BEGIN
    DECLARE   @NewTagID   int;
    INSERT INTO [bx_Tags]([Name]) VALUES (@TagName);
    SET @NewTagID = @@IDENTITY;
    IF @Type = 1 AND EXISTS (SELECT * FROM [bx_BlogArticles] WHERE [ArticleID] = @TargetID AND [PrivacyType] IN (0,3)) BEGIN
        INSERT INTO [bx_TagRelation]([TagID], [Type], [TargetID]) VALUES(@NewTagID, @Type, @TargetID);
      END
 END";


					foreach (Tag tag in tags)
					{
						query.CreateParameter<string>("@TagName", tag.Name, SqlDbType.NVarChar, 50);
						query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
						query.CreateParameter<TagType>("@Type", type, SqlDbType.TinyInt);

						query.ExecuteNonQuery();
					}
				}
			}
            return true;
        }


        /// <summary>
        /// 锁定标签
        /// </summary>
        /// <param name="tagID">标签ID</param>
        #region StoredProcedure
        [StoredProcedure(Name = "bx_LockTag", Script = @"
CREATE PROCEDURE {name}
     @ID       int
AS BEGIN
    SET NOCOUNT ON;
    UPDATE [bx_Tags] SET [IsLock] = 1 WHERE [ID] = @ID;
END")]
        #endregion
        public override bool LockTag(int tagID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_LockTag";
                query.CommandType = CommandType.StoredProcedure;

				query.CreateParameter<int>("@ID", tagID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }

            return true;
        }

        #endregion

        #region 删除

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagID">标签ID</param>
        #region StoredProcedure
        [StoredProcedure(Name = "bx_DeleteTag", Script = @"
CREATE PROCEDURE {name}
     @ID       int
AS BEGIN
    SET NOCOUNT ON;
    DELETE FROM [bx_Tags] WHERE [ID] = @ID;
END")]
        #endregion
        public override bool DeleteTag(int tagID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeleteTag";
                query.CommandType = CommandType.StoredProcedure;

				query.CreateParameter<int>("@ID", tagID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }

            return true;
        }


        /// <summary>
        /// 删除多个标签
        /// </summary>
        public override bool DeleteTags(IEnumerable<int> tagIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM [bx_Tags] WHERE [ID] IN (@IDs);";

				query.CreateInParameter<int>("@IDs", tagIDs);

                query.ExecuteNonQuery();
            }
            return true;
        }


        /// <summary>
        /// 高级删除
        /// </summary>
        public override bool DeleteTagsByFilter(AdminTagFilter filter)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter);

                query.CommandText = "DELETE FROM [bx_Tags] WHERE " + conditions;

                query.ExecuteNonQuery();
            }

            return true;
        }

        #endregion


        private string BuildConditionsByFilter(SqlQuery query, AdminTagFilter filter)
        {
            StringBuffer sqlConditions = new StringBuffer();

            if (string.IsNullOrEmpty(filter.Name))
            {
                sqlConditions += " AND [Name] Like '%' + @Name + '%'";
                query.CreateParameter<string>("@Name", filter.Name, SqlDbType.NVarChar, 50);
            }

            if (filter.Type != null)
            {
                sqlConditions += " AND ([ID] IN (SELECT [TagID] FROM [bx_TagRelation] WHERE [Type] = @Type))";
                query.CreateParameter<TagType?>("@Type", filter.Type, SqlDbType.TinyInt);
            }

            if (filter.IsLock != null)
            {
                sqlConditions += " AND [IsLock] = @IsLock";
                query.CreateParameter<bool?>("@IsLock", filter.IsLock, SqlDbType.Bit);
            }

            if (filter.TotalElementsScopeBegin != null && filter.TotalElementsScopeEnd != null)
            {
                sqlConditions += " AND ([TotalElements] >= @TotalElementsBegin AND [TotalElements] <= @TotalElementsEnd)";
                query.CreateParameter<int?>("@TotalElementsBegin", filter.TotalElementsScopeBegin, SqlDbType.Int);
                query.CreateParameter<int?>("@TotalElementsEnd", filter.TotalElementsScopeEnd, SqlDbType.Int);
            }

            if (sqlConditions.Length > 0)
                sqlConditions.Remove(0, 5);

            return sqlConditions.ToString();
        }
    }
}