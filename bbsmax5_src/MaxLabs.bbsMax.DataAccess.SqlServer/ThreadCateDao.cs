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
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;


using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class ThreadCateDao : DataAccess.ThreadCateDao
    {
        #region 存储过程 bx_GetAllCates
        [StoredProcedure(Name = "bx_GetAllCates", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_ThreadCates ORDER BY SortOrder;
END
"
            )]
        #endregion
        public override ThreadCateCollection GetAllCates()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAllCates";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ThreadCateCollection(reader);
                }
            }
        }

        #region 存储过程 bx_CreateThreadCate
        [StoredProcedure(Name = "bx_CreateThreadCate", Script = @"
CREATE PROCEDURE {name}
     @CateName   nvarchar(50)
    ,@Enable     bit
    ,@SortOrder  int
AS
BEGIN
	SET NOCOUNT ON;
    
    DECLARE @CateID int;
	INSERT INTO bx_ThreadCates(CateName,Enable,SortOrder) VALUES(@CateName,@Enable,@SortOrder);
    SET @CateID = @@IDENTITY;
	INSERT INTO bx_ThreadCateModels(CateID,ModelName,Enable,SortOrder) VALUES(@CateID,'默认模板',1,1);
END
"
            )]
        #endregion
        public override bool CreateThreadCate(string cateName, bool enable, int sortOrder)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreateThreadCate";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@CateName", cateName, SqlDbType.NVarChar, 50);
                query.CreateParameter<bool>("@Enable", enable, SqlDbType.Bit);
                query.CreateParameter<int>("@SortOrder", sortOrder, SqlDbType.Int);

                query.ExecuteNonQuery();

                return true;
            }
        }

        #region 存储过程 bx_UpdateThreadCate
        [StoredProcedure(Name = "bx_UpdateThreadCate", Script = @"
CREATE PROCEDURE {name}
     @CateID     int
    ,@CateName   nvarchar(50)
    ,@Enable     bit
    ,@SortOrder  int
AS
BEGIN
	SET NOCOUNT ON;
    IF NOT EXISTS(SELECT * FROM bx_ThreadCates WHERE CateID = @CateID) BEGIN
        SELECT 0;
    END
    ELSE BEGIN
	    UPDATE bx_ThreadCates SET CateName = @CateName, Enable = @Enable, SortOrder = @SortOrder WHERE CateID = @CateID;
        SELECT 1;
    END
END
"
            )]
        #endregion
        public override int UpdateThreadCate(int cateID, string cateName, bool enable, int sortOrder)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateThreadCate";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@CateID", cateID, SqlDbType.Int);
                query.CreateParameter<string>("@CateName", cateName, SqlDbType.NVarChar, 50);
                query.CreateParameter<bool>("@Enable", enable, SqlDbType.Bit);
                query.CreateParameter<int>("@SortOrder", sortOrder, SqlDbType.Int);

                return query.ExecuteScalar<int>();

            }
        }

        public override bool UpdateThreadCates(IEnumerable<int> cateIDs, IEnumerable<bool> enables, IEnumerable<int> sortOrders)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sql = new StringBuilder();

                int i = 0;
                List<bool> tempEnables = new List<bool>(enables);
                List<int> tempSortOrders = new List<int>(sortOrders);
                foreach (int id in cateIDs)
                {
                    sql.AppendFormat(@"
UPDATE bx_ThreadCates SET Enable = @Enable_{0}, SortOrder = @SortOrder_{0} WHERE CateID = @CateID_{0};
"
                        , i);

                    query.CreateParameter<bool>("@Enable_" + i, tempEnables[i], SqlDbType.Bit);
                    query.CreateParameter<int>("@SortOrder_" + i, tempSortOrders[i], SqlDbType.Int);
                    query.CreateParameter<int>("@CateID_" + i, id, SqlDbType.Int);

                    i++;
                }

                if (sql.Length == 0)
                    return true;

                query.CommandText = sql.ToString();
                query.CommandType = CommandType.Text;

                query.ExecuteNonQuery();

                return true;
            }
        }

        public override bool DeleteThreadCates(IEnumerable<int> cateIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE bx_ThreadCates WHERE CateID in(@CateIDs)";
                query.CommandType = CommandType.Text;

                query.CreateInParameter<int>("@CateIDs", cateIDs);

                query.ExecuteNonQuery();

                return true;
            }
        }

        #region 存储过程 bx_GetAllModels
        [StoredProcedure(Name = "bx_GetAllModels", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_ThreadCateModels ORDER BY SortOrder;
END
"
            )]
        #endregion
        public override Dictionary<int, ThreadCateModelCollection> GetAllModels()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAllModels";
                query.CommandType = CommandType.StoredProcedure;

                Dictionary<int, ThreadCateModelCollection> models = new Dictionary<int, ThreadCateModelCollection>();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ThreadCateModel model = new ThreadCateModel(reader);
                        ThreadCateModelCollection tempModels;
                        if (models.TryGetValue(model.CateID, out tempModels) == false)
                        {
                            tempModels = new ThreadCateModelCollection();
                            tempModels.Add(model);

                            models.Add(model.CateID, tempModels);
                        }
                        else
                        {
                            tempModels.Add(model);
                        }
                    }
                }

                return models;
            }
        }

        #region 存储过程 bx_CreateModel
        [StoredProcedure(Name = "bx_CreateModel", Script = @"
CREATE PROCEDURE {name}
     @CateID      int
    ,@ModelName   nvarchar(50)
    ,@Enable      bit
    ,@SortOrder   int
AS
BEGIN
	SET NOCOUNT ON;
    IF EXISTS(SELECT * FROM bx_ThreadCates WHERE CateID = @CateID) BEGIN
	    INSERT INTO bx_ThreadCateModels(CateID,ModelName,Enable,SortOrder) VALUES(@CateID,@ModelName,@Enable,@SortOrder);
        SELECT 1;
    END
    ELSE
        SELECT 0;
END
"
            )]
        #endregion
        public override int CreateModel(int cateID, string modelName, bool enable, int sortOrder)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreateModel";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@CateID", cateID, SqlDbType.Int);
                query.CreateParameter<string>("@ModelName", modelName, SqlDbType.NVarChar, 50);
                query.CreateParameter<bool>("@Enable", enable, SqlDbType.Bit);
                query.CreateParameter<int>("@SortOrder", sortOrder, SqlDbType.Int);

                return query.ExecuteScalar<int>();

            }
        }

        public override bool UpdateModels(IEnumerable<int> modelIDs, IEnumerable<int> sortOrders, IEnumerable<string> modelNames, IEnumerable<int> enableIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sql = new StringBuilder();

                int i = 0;
                List<string> tempModelNames = new List<string>(modelNames);
                List<int> tempSortOrders = new List<int>(sortOrders);
                List<int> tempEnableIDs = new List<int>(enableIDs);
                foreach (int id in modelIDs)
                {
                    sql.AppendFormat(@"
UPDATE bx_ThreadCateModels SET SortOrder = @SortOrder_{0}, ModelName = @ModelName_{0}, Enable = @Enable_{0} WHERE ModelID = @ModelID_{0};
"
                        , i);

                    query.CreateParameter<string>("@ModelName_" + i, tempModelNames[i], SqlDbType.NVarChar, 50);
                    query.CreateParameter<int>("@SortOrder_" + i, tempSortOrders[i], SqlDbType.Int);
                    query.CreateParameter<int>("@ModelID_" + i, id, SqlDbType.Int);
                    query.CreateParameter<bool>("@Enable_" + i, tempEnableIDs.Contains(id), SqlDbType.Bit);

                    i++;
                }

                if (sql.Length == 0)
                    return true;

                query.CommandText = sql.ToString();
                query.CommandType = CommandType.Text;

                query.ExecuteNonQuery();

                return true;
            }
        }

        public override bool DeleteModels(IEnumerable<int> modelIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE bx_ThreadCateModels WHERE ModelID in(@ModelIDs)";
                query.CommandType = CommandType.Text;

                query.CreateInParameter<int>("@ModelIDs", modelIDs);

                query.ExecuteNonQuery();

                return true;
            }
        }

        public override bool EnableModels(int cateID, IEnumerable<int> modelIDs)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
UPDATE bx_ThreadCateModels SET Enable = 0 WHERE CateID = @CateID;";

                if (modelIDs != null && ValidateUtil.HasItems<int>(modelIDs))
                {
                    query.CommandText += @"
UPDATE bx_ThreadCateModels SET Enable = 1 WHERE CateID = @CateID AND ModelID in(@ModelIDs)";
                    query.CreateInParameter<int>("@ModelIDs", modelIDs);
                }
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@CateID", cateID, SqlDbType.Int);

                query.ExecuteNonQuery();

                return true;
            }
        }


        #region 存储过程 bx_GetAllThreadCateModelField
        [StoredProcedure(Name = "bx_GetAllThreadCateModelField", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_ThreadCateModelFields ORDER BY SortOrder;
END
"
            )]
        #endregion
        public override Dictionary<int,ThreadCateModelFieldCollection> GetAllThreadCateModelField()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAllThreadCateModelField";
                query.CommandType = CommandType.StoredProcedure;

                Dictionary<int, ThreadCateModelFieldCollection> fields = new Dictionary<int, ThreadCateModelFieldCollection>();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ThreadCateModelField field = new ThreadCateModelField(reader);

                        ThreadCateModelFieldCollection tempFields;
                        if (fields.TryGetValue(field.ModelID, out tempFields) == false)
                        {
                            tempFields = new ThreadCateModelFieldCollection();
                            tempFields.Add(field);

                            fields.Add(field.ModelID, tempFields);
                        }
                        else
                        {
                            tempFields.Add(field);
                        }
                    }
                }

                return fields;
            }
        }



        #region 存储过程 bx_CreateThreadCateModelField
        [StoredProcedure(Name = "bx_CreateThreadCateModelField", Script = @"
CREATE PROCEDURE {name}
     @ModelID    int
    ,@FieldName  nvarchar(50)
    ,@Enable     bit
    ,@SortOrder  int
    ,@FieldType  nvarchar(50)
    ,@FieldTypeSetting  ntext
    ,@Search            bit
    ,@AdvancedSearch    bit
    ,@DisplayInList     bit
    ,@MustFilled        bit
    ,@Description       nvarchar(1000)
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM bx_ThreadCateModels WHERE ModelID = @ModelID) BEGIN
	    INSERT INTO bx_ThreadCateModelFields(ModelID,FieldName,Enable,SortOrder,FieldType,FieldTypeSetting,
                    Search,AdvancedSearch,DisplayInList,MustFilled,Description) VALUES(
                        @ModelID,@FieldName,@Enable,@SortOrder,@FieldType,@FieldTypeSetting,
                    @Search,@AdvancedSearch,@DisplayInList,@MustFilled,@Description)
        SELECT 1;
    END
    ELSE
        SELECT 0;
END
"
            )]
        #endregion
        public override int CreateThreadCateModelField(int modelID, string fieldName, bool enable, int sortOrder, string fieldType
            , string fieldTypeSetting, bool search, bool advancedSearch, bool displayInList, bool mustFilled, string description)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreateThreadCateModelField";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ModelID", modelID, SqlDbType.Int);
                query.CreateParameter<string>("@FieldName", fieldName, SqlDbType.NVarChar, 50);
                query.CreateParameter<bool>("@Enable", enable, SqlDbType.Bit);
                query.CreateParameter<int>("@SortOrder", sortOrder, SqlDbType.Int);
                query.CreateParameter<string>("@FieldType", fieldType, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@FieldTypeSetting", fieldTypeSetting, SqlDbType.NText);
                query.CreateParameter<bool>("@Search", search, SqlDbType.Bit);
                query.CreateParameter<bool>("@AdvancedSearch", advancedSearch, SqlDbType.Bit);
                query.CreateParameter<bool>("@DisplayInList", displayInList, SqlDbType.Bit);
                query.CreateParameter<bool>("@MustFilled", mustFilled, SqlDbType.Bit);
                query.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 1000);

                return query.ExecuteScalar<int>();
            }
        }

        #region 存储过程 bx_UpdateThreadCateModelField
        [StoredProcedure(Name = "bx_UpdateThreadCateModelField", Script = @"
CREATE PROCEDURE {name}
     @FieldID    int
    ,@FieldName  nvarchar(50)
    ,@Enable     bit
    ,@SortOrder  int
    ,@FieldTypeSetting  ntext
    ,@Search            bit
    ,@AdvancedSearch    bit
    ,@DisplayInList     bit
    ,@MustFilled        bit
    ,@Description       nvarchar(1000)
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM bx_ThreadCateModelFields WHERE FieldID = @FieldID) BEGIN
	    UPDATE bx_ThreadCateModelFields SET FieldName = @FieldName, Enable = @Enable, SortOrder = @SortOrder
                    ,FieldTypeSetting = @FieldTypeSetting , Search = @Search, AdvancedSearch = @AdvancedSearch, DisplayInList = @DisplayInList
                    ,MustFilled = @MustFilled, Description = @Description WHERE FieldID = @FieldID;
        SELECT 1;
    END
    ELSE
        SELECT 0;
END
"
            )]
        #endregion
        public override int UpdateThreadCateModelField(int fieldID, string fieldName, bool enable, int sortOrder
            , string fieldTypeSetting, bool search, bool advancedSearch, bool displayInList, bool mustFilled, string description)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateThreadCateModelField";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@FieldID", fieldID, SqlDbType.Int);
                query.CreateParameter<string>("@FieldName", fieldName, SqlDbType.NVarChar, 50);
                query.CreateParameter<bool>("@Enable", enable, SqlDbType.Bit);
                query.CreateParameter<int>("@SortOrder", sortOrder, SqlDbType.Int);
                query.CreateParameter<string>("@FieldTypeSetting", fieldTypeSetting, SqlDbType.NText);
                query.CreateParameter<bool>("@Search", search, SqlDbType.Bit);
                query.CreateParameter<bool>("@AdvancedSearch", advancedSearch, SqlDbType.Bit);
                query.CreateParameter<bool>("@DisplayInList", displayInList, SqlDbType.Bit);
                query.CreateParameter<bool>("@MustFilled", mustFilled, SqlDbType.Bit);
                query.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 1000);

                return query.ExecuteScalar<int>();
            }
        }


        public override bool UpdateThreadCateModelFields(IEnumerable<int> fieldIDs, IEnumerable<bool> enables, IEnumerable<int> sortOrders
            , IEnumerable<bool> searchs, IEnumerable<bool> advancedSearchs, IEnumerable<bool> displayInLists, IEnumerable<bool> mustFilleds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sql = new StringBuilder();
                int i = 0;

                List<int> tempFieldIDs = new List<int>(fieldIDs);
                List<bool> tempEnables = new List<bool>(enables);
                List<int> tempSortOrders = new List<int>(sortOrders);
                List<bool> tempSearchs = new List<bool>(searchs);
                List<bool> tempAdvancedSearchs = new List<bool>(advancedSearchs);
                List<bool> tempDisplayInLists = new List<bool>(displayInLists);
                List<bool> tempMustFilleds = new List<bool>(mustFilleds);
                foreach (int fieldID in fieldIDs)
                {
                    sql.AppendFormat(@"
UPDATE bx_ThreadCateModelFields SET Enable = @Enable_{0}, SortOrder = @SortOrder_{0}
                    ,Search = @Search_{0}, AdvancedSearch = @AdvancedSearch_{0}, DisplayInList = @DisplayInList_{0}
                    ,MustFilled = @MustFilled_{0} WHERE FieldID = @FieldID_{0};
"
                        , i);

                    query.CreateParameter<int>("@FieldID_" + i, tempFieldIDs[i], SqlDbType.Int);
                    query.CreateParameter<bool>("@Enable_" + i, tempEnables[i], SqlDbType.Bit);
                    query.CreateParameter<int>("@SortOrder_" + i, tempSortOrders[i], SqlDbType.Int);
                    query.CreateParameter<bool>("@Search_" + i, tempSearchs[i], SqlDbType.Bit);
                    query.CreateParameter<bool>("@AdvancedSearch_" + i, tempAdvancedSearchs[i], SqlDbType.Bit);
                    query.CreateParameter<bool>("@DisplayInList_" + i, tempDisplayInLists[i], SqlDbType.Bit);
                    query.CreateParameter<bool>("@MustFilled_" + i, tempMustFilleds[i], SqlDbType.Bit);

                    i++;
                }


                if (sql.Length == 0)
                    return true;

                query.CommandText = sql.ToString();
                query.CommandType = CommandType.Text;

                query.ExecuteNonQuery();

                return true;
            }
        }




        public override bool DeleteModelFields(IEnumerable<int> fieldIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE bx_ThreadCateModelFields WHERE FieldID in(@FieldIDs)";
                query.CommandType = CommandType.Text;

                query.CreateInParameter<int>("@FieldIDs", fieldIDs);

                query.ExecuteNonQuery();

                return true;
            }
        }

        public override bool LoadModelFields(int modelID, IEnumerable<int> fieldIDs)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
INSERT INTO bx_ThreadCateModelFields(ModelID,FieldName,Enable,FieldType,FieldTypeSetting,Search,AdvancedSearch,DisplayInList,MustFilled,[Description])
            SELECT @ModelID,FieldName,Enable,FieldType,FieldTypeSetting,Search,AdvancedSearch,DisplayInList,MustFilled,[Description] 
                FROM bx_ThreadCateModelFields WHERE FieldID in (@FieldIDs);
"
                    ;
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@ModelID", modelID, SqlDbType.Int);
                query.CreateInParameter<int>("@FieldIDs", fieldIDs);

                query.ExecuteNonQuery();

                return true;
            }
        }
    }
}