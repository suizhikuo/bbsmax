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
using MaxLabs.bbsMax.Entities;
using System.Data;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class PassportDao:DataAccess.PassportDao
    {

        public override bool UpdateClientInstructTypes( int clientID , IEnumerable<InstructType> instructTypes )
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_PassportClients SET InstructTypes = @InstructTypes WHERE ClientID = @ClientID;";
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@ClientID", clientID, SqlDbType.Int);
                query.CreateParameter<string>("@InstructTypes", StringUtil.Join(instructTypes), SqlDbType.Text);

                return query.ExecuteNonQuery() > 0;
            }
        }

        public override PassportClient GetPassportClient(int clientID)
        {
            using( SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_PassportClients WHERE ClientID = @ClientID";
                query.CreateParameter<int>("@ClientID", clientID, SqlDbType.Int);

                PassportClient client = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Next)
                        client = new PassportClient(reader);
                    return client;
                }
            }
        }

        public override List<Instruct> LoadClientInstruct(int clientID, int loadCount, out int laveCount)
        {
            List<Instruct> results = new List<Instruct>();
            laveCount = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"SELECT TOP (@LoadCount) * FROM bx_Instructs WHERE ClientID = @ClientID;
                SELECT Count(*)  FROM bx_Instructs WHERE ClientID = @ClientID;";
                query.CreateParameter<int>("@ClientID", clientID, SqlDbType.Int);
                query.CreateTopParameter("@LoadCount", loadCount);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Next)
                    {
                        results.Add(new Instruct(reader));
                    }

                    reader.NextResult();
                    if (reader.Next)
                        laveCount = reader.Get<int>(0);

                    laveCount-=loadCount;

                    if(laveCount<=0)
                        laveCount=0;
                }

                return results;
            }
        }

        [StoredProcedure( Name="bx_DeletePassportClient",Script= @"
CREATE PROCEDURE {name}
@ClientID        int

AS 
BEGIN
SET NOCOUNT ON;

UPDATE bx_PassportClients SET Deleted = 1 WHERE ClientID = @ClientID;
DELETE FROM bx_Instructs WHERE InstructID IN( SELECT TOP 200 InstructID FROM bx_Instructs WHERE ClientID = @ClientID );

DECLARE @InsCount int ;

SET @InsCount = 0;
--SET @InsCount = (SELECT COUNT(*) FROM bx_Instructs WHERE ClientID = @ClientID);

--IF  @InsCount = 0
    DELETE FROM bx_PassportClients WHERE ClientID = @ClientID

SELECT @InsCount;
END
")]
        public override bool DeleteClient(int clientID,out int instrcuLaveCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeletePassportClient";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@ClientID", clientID, SqlDbType.Int);

                instrcuLaveCount = query.ExecuteScalar<int>();

                return instrcuLaveCount == 0;
            }
        }

        public override PassportClient TryUpdateClientInfo(string name, string clientUrl, string apiFile, string accessKey, IEnumerable<InstructType> allowInstructTypes)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"UPDATE bx_PassportClients SET  ClientName = @ClientName,InstructTypes = @InstructTypes, AccessKey = @AccessKey WHERE Url = @Url AND APIFilePath = @APIFilePath; 
                SELECT * FROM  bx_PassportClients WHERE  Url = @Url AND APIFilePath = @APIFilePath;";

                query.CommandType = System.Data.CommandType.Text;
                query.CreateParameter<string>("@ClientName", name, System.Data.SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Url", clientUrl, System.Data.SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@AccessKey", accessKey, System.Data.SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@APIFilePath", apiFile, SqlDbType.NVarChar, 500);
                query.CreateParameter<string>("@InstructTypes", StringUtil.Join(allowInstructTypes), SqlDbType.Text);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PassportClient client = null;

                    while (reader.Next)
                        client = new PassportClient(reader);

                    return client;
                }
            }
        }

        /// <summary>
        /// 创建客户端不
        /// </summary>
        /// <param name="name"></param>
        /// <param name="apiUrl"></param>
        /// <param name="accessKey"></param>
        /// <param name="allowInstructTypes"></param>
        /// <returns></returns>
        public override PassportClient CreatePassportClient(string name, string url,string apiFilePath, string accessKey, IEnumerable<InstructType> allowInstructTypes)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "INSERT INTO bx_PassportClients( ClientName, Url, APIFilePath, AccessKey, InstructTypes) VALUES( @ClientName, @Url, @APIFilePath, @AccessKey, @InstructTypes); SELECT * FROM bx_PassportClients WHERE ClientID = @@IDENTITY;";

                query.CommandType = System.Data.CommandType.Text;
                query.CreateParameter<string>("@ClientName", name, System.Data.SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Url", url, System.Data.SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@AccessKey", accessKey, System.Data.SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@APIFilePath", apiFilePath, SqlDbType.NVarChar, 500);
                query.CreateParameter<string>("@InstructTypes", StringUtil.Join(allowInstructTypes), SqlDbType.Text);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PassportClient client = null;

                    while (reader.Next)
                        client = new PassportClient(reader);

                    return client;
                }
            }
        }

        /// <summary>
        /// 删除执行过的指令
        /// </summary>
        /// <param name="instructID"></param>
        public override void DeleteInstruct(IEnumerable<long> instructID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_Instructs WHERE InstructID IN( @InstructIDs );";
                query.CreateInParameter<long>("@InstructIDs", instructID);
                query.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 指令写入数据库
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="clientID"></param>
        /// <param name="type"></param>
        /// <param name="createDate"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public override long WriteInstruct(int targetID, int clientID, MaxLabs.bbsMax.Enums.InstructType type, DateTime createDate, string datas)
        {
            using( SqlQuery query=new SqlQuery())
            {
                query.CommandText = "INSERT INTO bx_Instructs( ClientID, TargetID, InstructType, CreateDate, Datas) VALUES( @ClientID, @TargetID, @InstructType, @CreateDate, @Datas); SELECT @@IDENTITY;";
                query.CommandType = System.Data.CommandType.Text;
                query.CreateParameter<int>("@ClientID", clientID, SqlDbType.Int);
                query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
                query.CreateParameter<int>("@InstructType", (int)type, SqlDbType.Int);
                query.CreateParameter<DateTime>("@CreateDate", createDate, SqlDbType.DateTime);
                query.CreateParameter<string>("@Datas", datas, SqlDbType.NText);
                return  query.ExecuteScalar<long>();
            }
        }
        
        /// <summary>
        /// 返回所有客户端列表
        /// </summary>
        /// <returns></returns>
        public override PassportClientCollection GetAllPassportClient()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_PassportClients WHERE Deleted = 0";

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PassportClientCollection(reader);
                }
            }
        }
    }
}