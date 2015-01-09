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

using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class ValidateCodeDao : ValidateCodes.ValidateCodeDao
    {
        #region 存储过程 bx_GetValidateCodeActionRecords
        [StoredProcedure(Name = "bx_GetValidateCodeActionRecords", Script = @"
CREATE PROCEDURE {name}
    @IP varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_ValidateCodeActionRecords] WHERE [IP]=@IP ORDER BY [ID];
END
"
            )]
        #endregion
        public override ValidateCodeActionRecordCollection GetValidateCodeActionRecords(string IP)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetValidateCodeActionRecords";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@IP", IP, SqlDbType.VarChar, 50);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ValidateCodeActionRecordCollection(reader);
                }
            }
        }

        #region 存储过程 bx_CreateValidateCodeActionRecord
        [StoredProcedure(Name = "bx_CreateValidateCodeActionRecord", Script = @"
CREATE PROCEDURE {name}
     @IP            varchar(50)
    ,@Action        varchar(200)
    ,@CreateDate    datetime
    ,@LimitedTime   datetime
    ,@LimitedCount  int
AS
BEGIN
	SET NOCOUNT ON;
    INSERT INTO [bx_ValidateCodeActionRecords]([IP], [Action], [CreateDate]) VALUES (@IP, @Action, @CreateDate);

    SELECT @@IDENTITY;

        
    EXEC('
    DECLARE @Temp table([ID] int);
    INSERT INTO @Temp SELECT TOP ' + @LimitedCount + ' [ID] FROM [bx_ValidateCodeActionRecords] WHERE [IP]=''' + @IP + ''' AND [Action]=''' + @Action + '''
        AND [CreateDate] > ''' + @LimitedTime + ''' ORDER BY [ID] DESC;

    DELETE [bx_ValidateCodeActionRecords] WHERE [ID]<(SELECT ISNULL(MIN([ID]),2147483647) FROM @Temp) AND  [IP]=''' + @IP + ''' AND [Action]=''' + @Action + ''';
    ');
END
"
            )]
        #endregion
        public override int CreateValidateCodeActionRecord(string IP, string action, DateTime createDate, int limitedTime, int limitedCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreateValidateCodeActionRecord";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@IP", IP, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@Action", action, SqlDbType.VarChar, 200);
                query.CreateParameter<DateTime>("@CreateDate", createDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@LimitedTime",DateTimeUtil.Now.AddSeconds(0 - limitedTime),SqlDbType.DateTime);
                query.CreateParameter<int>("@LimitedCount", limitedCount, SqlDbType.Int);

                return query.ExecuteScalar<int>();
            }
        }

        public override void DeleteExperisValidateCodeActionRecord(List<string> actions, List<int> limitedTimes)
        {
            using (SqlSession db = new SqlSession())
            {
                using (SqlQuery query = db.CreateQuery(QueryMode.Batch))
                {
                    StringBuilder sql = new StringBuilder();

                    for (int i = 0; i < actions.Count; i++)
                    {
                        sql.AppendFormat(@"
DELETE [bx_ValidateCodeActionRecords] WHERE [Action] = @Action_{0} AND [CreateDate] < @LimitedTime_{0};
", i);
                        query.CreateParameter<string>("@Action_" + i, actions[i], SqlDbType.VarChar, 200);
                        query.CreateParameter<DateTime>("@LimitedTime_" + i, DateTimeUtil.Now.AddSeconds(0 - limitedTimes[i]), SqlDbType.DateTime);

                        //query.ExecuteNonQuery();
                    }

                    //query.Submit();

                    if (actions.Count > 0)
                    {
                        query.CommandText += "DELETE [bx_ValidateCodeActionRecords] WHERE [Action] NOT IN (@Actions);";

                        query.CreateInParameter<string>("@Actions", actions);
                    }
                    else
                        query.CommandText += "DELETE [bx_ValidateCodeActionRecords]";

                    query.ExecuteNonQuery();
                }

                
            }
        }

    }
}