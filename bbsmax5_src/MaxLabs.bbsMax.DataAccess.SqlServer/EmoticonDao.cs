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
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class EmoticonDao : DataAccess.EmoticonDao
    {
        public override UserEmoticonInfoCollection AdminGetUserEmoticonInfos(EmoticonFilter filter, int pageIndex, IEnumerable<Guid> excludeRoleIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_UserEmoticonInfo";
                query.Pager.PageSize = filter.Pagesize;
                query.Pager.PageNumber = pageIndex;
                query.Pager.SortField = "[UserID]";
                if (filter.UserName == null)
                    filter.UserName = string.Empty;
                if (filter.Order != null)
                {
                    switch (filter.Order.Value)
                    {
                        case EmoticonFilter.OrderBy.SpaceSize:
                            query.Pager.SortField = "[TotalSizes]";
                            break;
                        case EmoticonFilter.OrderBy.EmoticonCount:
                            query.Pager.SortField = "[TotalEmoticons]";
                            break;
                    }
                }

                query.Pager.Condition = " Username LIKE '%'+@Username+'%'";

                string excludeRoleUserIds = DaoUtil.GetExcludeRoleSQL("UserID", excludeRoleIDs, query);
                if (!string.IsNullOrEmpty(excludeRoleUserIds))
                {
                    query.Pager.Condition += " AND " + excludeRoleUserIds;
                }

                query.CreateParameter<string>("@Username", filter.UserName, SqlDbType.NVarChar, 50);
                query.Pager.SelectCount = true;
                query.Pager.PrimaryKey = "[UserID]";
                query.Pager.IsDesc = filter.IsDesc;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserEmoticonInfoCollection groups = new UserEmoticonInfoCollection(reader);
                    if (reader.NextResult())
                        if (reader.Read())
                            groups.TotalRecords = reader.Get<int>(0);
                    return groups;
                }
            }
        }


        [StoredProcedure(Name = "bx_Emoticon_AdminDeleteEmoticons", Script = @"
CREATE PROCEDURE {name}
     @UserID             int 
    ,@EmoticonIDs        varchar(8000)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @BeAboutToDelete TABLE( EmoticonID int,ImageSrc varchar(256) COLLATE Chinese_PRC_CI_AS_WS);
    INSERT @BeAboutToDelete SELECT EmoticonID,ImageSrc FROM bx_Emoticons WHERE EmoticonID IN ( SELECT item FROM bx_GetIntTable(@EmoticonIDs,','))  AND GroupID IN ( SELECT GroupID FROM bx_EmoticonGroups WHERE UserID = @UserID);
    DELETE FROM bx_Emoticons WHERE EmoticonID IN ( SELECT EmoticonID FROM @BeAboutToDelete );
    SELECT  ImageSrc FROM @BeAboutToDelete WHERE ImageSrc NOT IN ( SELECT ImageSrc FROM bx_Emoticons WHERE ImageSrc IN ( SELECT ImageSrc FROM @BeAboutToDelete ));
END
")]
        public override List<string> AdminiDeleteEmoticons(int userID, IEnumerable<int> emoticonIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Emoticon_AdminDeleteEmoticons";
                
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@EmoticonIDs", StringUtil.Join(emoticonIds), SqlDbType.VarChar, 8000);

                List<string> filenames = new List<string>();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                        filenames.Add(reader.GetString(0));
                }
                return filenames;
            }
        }

        #region 存储过程bx_DeleteUserAllEmoticons
        [StoredProcedure(Name = "bx_Emoticon_DeleteUserAllEmoticons", Script = @"
CREATE PROCEDURE {name}
    @UserID     int
AS 
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeleteTable TABLE(TempID int IDENTITY, EmoticonID int, ImageSrc varchar(255) COLLATE Chinese_PRC_CI_AS_WS);
    INSERT  @DeleteTable SELECT EmoticonID, ImageSrc FROM bx_Emoticons WHERE GroupID IN(SELECT GroupID FROM bx_EmoticonGroups WHERE UserID = @UserID)
    DELETE  bx_EmoticonGroups WHERE UserID = @UserID;
    SELECT  ImageSrc FROM @DeleteTable WHERE ImageSrc NOT IN ( SELECT ImageSrc FROM bx_Emoticons WHERE ImageSrc IN ( SELECT ImageSrc FROM @DeleteTable ));
END
")]
        #endregion
        public override List<string> AdminDeleteUserAllEmoticon(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Emoticon_DeleteUserAllEmoticons";
                
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                List<string> filenames = new List<string>();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                        filenames.Add(reader.GetString(0));
                }
                return filenames;
            }
        }

        public override EmoticonCollection AdminGetUserEmoticons(int userID, int pageSize, int pageIndex)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Emoticons";
                query.Pager.Condition = " GroupID IN (SELECT GroupID FROM bx_EmoticonGroups WHERE UserID = @UserID)";


                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.Pager.PageSize = pageSize;
                query.Pager.PageNumber = pageIndex;
                query.Pager.SortField = "SortOrder";
                query.Pager.PrimaryKey = "EmoticonID";
                query.Pager.SelectCount = true;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    EmoticonCollection Emoticons = new EmoticonCollection(reader);
                    if (reader.NextResult())
                        if (reader.Read())
                            Emoticons.TotalRecords = reader.Get<int>(0);

                    return Emoticons;
                }
            }
        }

        [StoredProcedure(Name = "bx_Emoticon_GetEmoticonsByUserID", Script = @"
CREATE PROCEDURE {name}
    @UserID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_Emoticons WHERE GroupID IN (SELECT GroupID FROM bx_EmoticonGroups WHERE UserID = @UserID);
END")]
        public override EmoticonCollection GetEmoticons(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Emoticon_GetEmoticonsByUserID";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new EmoticonCollection(reader);
                }
            }
        }

        [StoredProcedure(Name = "bx_Emoticon_GetEmoticonsByGroupID", Script = @"
CREATE PROCEDURE {name}
    @UserID int
    ,@GroupID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_Emoticons WHERE GroupID = (SELECT GroupID FROM bx_EmoticonGroups WHERE GroupID = @GroupID AND UserID = @UserID);
END")]
        public override EmoticonCollection GetEmoticons(int userID, int groupID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Emoticon_GetEmoticonsByGroupID";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@GroupID", groupID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new EmoticonCollection(reader);
                }
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_Emoticon_CreateEmoticonGroup", Script = @"
CREATE PROCEDURE {name}
     @UserID         int
    ,@GroupName      nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO bx_EmoticonGroups( UserID, GroupName) VALUES (@UserID, @GroupName);
    SELECT * FROM bx_EmoticonGroups WHERE GroupID = @@IDENTITY;
END
")]
        #endregion
        public override EmoticonGroup CreateGroup(int userID, string groupName)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Emoticon_CreateEmoticonGroup";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@GroupName", groupName, SqlDbType.NVarChar, 50);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new EmoticonGroup(reader);
                }
            }
            return null;
        }

        [StoredProcedure(Name = "bx_Emoticon_GetEmoticonGroup", Script = @"
CREATE PROCEDURE {name}
    @UserID int
    ,@GroupID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_EmoticonGroups WHERE GroupID = @GroupID AND UserID = @UserID;
END")]
        public override EmoticonGroup GetEmoticonGroup(int userID, int groupID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Emoticon_GetEmoticonGroup";

                query.CreateParameter<int>("@GroupID", groupID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new EmoticonGroup(reader);
                }
            }
            return null;
        }

        public override EmoticonCollection GetEmoticons(int userID, IEnumerable<int> emoticonID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_Emoticons WHERE EmoticonID IN (@EmoticonIDs) AND GroupID IN (SELECT GroupID FROM bx_EmoticonGroups WHERE UserID = @UserID);";
                query.CreateInParameter<int>("@EmoticonIDs", emoticonID);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new EmoticonCollection(reader);
                }
            }
        }

        public override EmoticonGroup CreateGroup(int userID, string groupName, EmoticonCollection emotions)
        {
            throw new NotImplementedException();
        }

        #region 删除分组

        [StoredProcedure(Name = "bx_Emoticon_DeleteEmoticonGroup", Script = @"
CREATE PROCEDURE {name}
 @UserID         int
,@GroupID        int
AS 
BEGIN
    SET NOCOUNT ON;
    IF EXISTS(SELECT * FROM bx_EmoticonGroups WHERE UserID = @UserID AND GroupID = @GroupID)   BEGIN
        DECLARE @DeleteTable TABLE(TempID int IDENTITY, EmoticonID int, ImageSrc varchar(255) COLLATE Chinese_PRC_CI_AS_WS);
        INSERT  @DeleteTable SELECT EmoticonID, ImageSrc FROM bx_Emoticons WHERE GroupID = @GroupID;
        DELETE  bx_EmoticonGroups WHERE GroupID = @GroupID;
        SELECT  ImageSrc FROM @DeleteTable WHERE ImageSrc NOT IN ( SELECT ImageSrc FROM bx_Emoticons WHERE ImageSrc IN ( SELECT ImageSrc FROM @DeleteTable ) );
    END
END
")]

        #endregion
        public override List<string> DeleteGroup(int userID, int groupID)
        {
            List<string> canDeleteFiles = new List<string>();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Emoticon_DeleteEmoticonGroup";
                
                query.CreateParameter<int>("@GroupID", groupID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        canDeleteFiles.Add(reader.GetString(0));
                    }
                }
            }
            return canDeleteFiles;
        }

        /// <summary>
        /// 删除表情文件， 返回删除后， 允许删除的文件路径
        /// </summary>
        /// <param name="emoticonIDs"></param>
        #region 存储过程
        [StoredProcedure(Name = "bx_Emoticon_DeleteEmoticons", Script = @"
CREATE PROCEDURE {name}
 @GroupID            int
,@UserID             int
,@EmoticonIDs        varchar(8000)
AS 
BEGIN 

    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM bx_EmoticonGroups WHERE UserID = @UserID AND GroupID = @GroupID)   BEGIN
        DECLARE @DeleteTable TABLE(TempID int IDENTITY, EmoticonID int, ImageSrc varchar(255) COLLATE Chinese_PRC_CI_AS_WS);
        INSERT  @DeleteTable SELECT EmoticonID, ImageSrc FROM bx_Emoticons WHERE EmoticonID IN ( SELECT item FROM bx_GetIntTable(@EmoticonIDs,',')) AND GroupID = @GroupID;
        DELETE  bx_Emoticons WHERE EmoticonID IN ( SELECT EmoticonID FROM @DeleteTable );
        SELECT  ImageSrc FROM @DeleteTable WHERE ImageSrc NOT IN ( SELECT ImageSrc FROM bx_Emoticons WHERE ImageSrc IN ( SELECT ImageSrc FROM @DeleteTable ) );
    END
END
")]
        #endregion
        public override List<string> DeleteEmoticons(int userID, int groupId, IEnumerable<int> emoticonIDs)
        {
            List<string> deleteFiles = new List<string>();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Emoticon_DeleteEmoticons";

                query.CreateParameter<string>("@EmoticonIDs", StringUtil.Join(emoticonIDs), SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@GroupID", groupId, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deleteFiles.Add(reader.GetString(0));
                    }
                }
            }
            return deleteFiles;
        }

        [StoredProcedure(Name = "bx_Emoticon_RenameGroup", Script = @"
CREATE PROCEDURE {name}
 @GroupID            int
,@UserID             int
,@GroupName        nvarchar(50)
AS 
BEGIN 

    SET NOCOUNT ON;

    UPDATE bx_EmoticonGroups SET GroupName = @GroupName WHERE GroupID = @GroupID AND UserID = @UserID;
END
")]
        public override void RenameGroup(int userID, int groupID, string groupName)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Emoticon_RenameGroup";

                query.CreateParameter<int>("@GroupID", groupID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@GroupName", groupName, SqlDbType.NVarChar, 50);
                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Emoticon_GetGroups", Script = @"
CREATE PROCEDURE {name}
    @UserID             int
AS 
BEGIN 

    SET NOCOUNT ON;

    SELECT * FROM bx_EmoticonGroups WHERE UserID = @UserID;
END
")]
        public override EmoticonGroupCollection GetEmoticonGroups(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Emoticon_GetGroups";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    EmoticonGroupCollection groups = new EmoticonGroupCollection(reader);
                    return groups;
                }
            }
        }

        public override EmoticonCollection GetEmoticons(int userID, int GroupID, int pageSize, int pageNumber, bool isDesc, out int totalCount)
        {
            totalCount = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Emoticons";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.PrimaryKey = "EmoticonID";
                query.Pager.SelectCount = true;
                query.Pager.SortField = "SortOrder";
                query.Pager.IsDesc = isDesc;
                query.Pager.Condition = " GroupID = @GroupID AND EXISTS( SELECT * FROM bx_EmoticonGroups WHERE GroupID = @GroupID AND UserID = @UserID)";
                query.CreateParameter<int>("@GroupID", GroupID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    EmoticonCollection emoticons = new EmoticonCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            emoticons.TotalRecords = reader.Get<int>(0);
                    }
                    totalCount = emoticons.TotalRecords;

                    return emoticons;
                }
            }
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="emoticons"></param>
        public override void CreateEmoticons(EmoticonCollection emoticons)
        {
            if (emoticons.Count == 0)
                return;
            using (SqlQuery query = new SqlQuery())
            {
                int i = 0;
                StringBuilder buider = new StringBuilder();
                foreach (Emoticon emote in emoticons)
                {
                    buider.AppendFormat("INSERT INTO bx_Emoticons( GroupID, UserID, Shortcut, ImageSrc, FileSize, MD5, SortOrder) VALUES( @GroupID{0}, @UserID{0}, @Shortcut{0}, @ImageSrc{0}, @FileSize{0}, @MD5{0}, @SortOrder{0});", i);
                    query.CreateParameter<int>(string.Format("@GroupID{0}", i), emote.GroupID, SqlDbType.Int);
                    query.CreateParameter<int>(string.Format("@UserID{0}", i), emote.UserID, SqlDbType.Int);
                    query.CreateParameter<string>(string.Format("@Shortcut{0}", i), emote.Shortcut, SqlDbType.NVarChar, 100);
                    query.CreateParameter<string>(string.Format("@ImageSrc{0}", i), emote.ImageSrc, SqlDbType.NVarChar, 255);
                    query.CreateParameter<int>(string.Format("@FileSize{0}", i), emote.FileSize, SqlDbType.Int);
                    query.CreateParameter<string>(string.Format("@MD5{0}", i), emote.MD5, SqlDbType.VarChar, 50);
                    query.CreateParameter<int>(string.Format("@SortOrder{0}", i), emote.SortOrder, SqlDbType.Int);
                    i++;
                }

                query.CommandText = buider.ToString();
                query.ExecuteNonQuery();
            }
        }

        /*---------------------------------- 3.0 -----------------------------*/
        /// <summary>
        /// 修改表情名称
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="emoticonID"></param>
        /// <param name="newNickName">新的名称</param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_RenameEmoticonShortcut", FileName = "V30/bx_RenameEmoticonShortcut.sql")]
        public override bool RenameEmoticonShortcut(int userID, int groupID, List<int> emoticonIDs, List<string> newShortcuts)
        {
            using (SqlQuery query = new SqlQuery())
            {
                //SqlParameter errorCode=new SqlParameter();

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@GroupID", groupID, SqlDbType.Int);
                query.CreateParameter<string>("@EmoticonIDs", StringUtil.Join(emoticonIDs, "|"), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@NewShortcuts", StringUtil.Join(newShortcuts, "|"), SqlDbType.NVarChar, 4000);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.CommandText = "bx_RenameEmoticonShortcut";
                query.CommandType = CommandType.StoredProcedure;
                query.ExecuteNonQuery();

                return Convert.ToInt32(query.Parameters["@ErrorCode"].Value) == 0;
            }
        }

        [StoredProcedure(Name = "bx_CreateEmoticonsAndGroups", FileName = "V30/bx_CreateEmoticonsAndGroups.sql")]
        public override bool CreateEmoticonsAndGroups(int userID, Dictionary<string, EmoticonCollection> emoticons)
        {
            StringBuilder groupNames = new StringBuilder();
            StringBuilder groupOrders = new StringBuilder();
            StringBuilder shortCuts = new StringBuilder();
            StringBuilder fileNames = new StringBuilder();
            StringBuilder fileSizes = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, EmoticonCollection> pair in emoticons)
            {
                i++;//i must = 1 start
                groupNames.Append(pair.Key + "|");
                foreach (Emoticon emoticon in pair.Value)
                {
                    string shortCut = emoticon.Shortcut == null ? "" : emoticon.Shortcut;
                    groupOrders.Append(i.ToString() + "|");
                    shortCuts.Append(shortCut + "|");
                    fileNames.Append(emoticon.ImageSrc + "|");
                    fileSizes.Append(emoticon.FileSize + "|");
                }
                //shortCuts.Remove(shortCuts.Length-1,1).Append(";");
                //fileNames.Remove(fileNames.Length - 1, 1).Append(";");
                //fileSizes.Remove(fileSizes.Length - 1, 1).Append(";");
            }

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_CreateEmoticonsAndGroups";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@GroupNames", groupNames.ToString(0, groupNames.Length - 1), SqlDbType.NText);
                query.CreateParameter<string>("@GroupOrders", groupOrders.ToString(0, groupOrders.Length - 1), SqlDbType.Text);
                query.CreateParameter<string>("@Shortcuts", shortCuts.ToString(0, shortCuts.Length - 1), SqlDbType.NText);
                query.CreateParameter<string>("@FileNames", fileNames.ToString(0, fileNames.Length - 1), SqlDbType.Text);
                query.CreateParameter<string>("@FileSizes", fileSizes.ToString(0, fileSizes.Length - 1), SqlDbType.Text);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);


                query.ExecuteNonQuery();
                return Convert.ToInt32(query.Parameters[6].Value) == 0;

            }
        }

        /// <summary>
        /// 批量移动表情到另一组
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="groupID">组ID</param>
        /// <param name="newGroupID">新组ID</param>
        /// <param name="emoticonIdentities">要移动的emoticonID</param>
        /// <returns></returns>
        /// 
        [StoredProcedure(Name = "bx_MoveEmoticons", FileName = "V30/bx_MoveEmoticons.sql")]
        public override bool MoveEmoticons(int userID, int groupID, int newGroupID, IEnumerable<int> emoticonIdentities)
        {
            using (SqlQuery query = new SqlQuery())
            {
                bool success;
                string IdentitiesString = StringUtil.Join(emoticonIdentities);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@GroupID", groupID, SqlDbType.Int);
                query.CreateParameter<int>("@NewGroupID", newGroupID, SqlDbType.Int);
                query.CreateParameter<string>("@EmoticonIdentities", IdentitiesString, SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.CommandText = "bx_MoveEmoticons";
                query.CommandType = CommandType.StoredProcedure;

                query.ExecuteNonQuery();
                success = (Convert.ToInt32(query.Parameters[4].Value) == 0);
                return success;
            }
        }

    }
}