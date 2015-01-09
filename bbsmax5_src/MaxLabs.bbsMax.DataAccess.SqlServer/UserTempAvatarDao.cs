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
using MaxLabs.bbsMax.DataAccess;
using System.Data;
using System.Data.SqlClient;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class UserTempAvatarDao : DataAccess.UserTempAvatarDao
    {
        public override UserTempAvatarCollection GetUserTempAvatars(int pageSize, int pageNumber, out int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_UserTempAvatar";
                query.Pager.PageSize = pageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PrimaryKey = "UserID";
                query.Pager.SortField = "[CreateDate]";
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                totalCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserTempAvatarCollection tempAvatars = new UserTempAvatarCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }

                    return tempAvatars;
                }
            }
        }

        public override UserTempAvatarCollection GetUserTempAvatars()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_UserTempAvatar ORDER BY [CreateDate] DESC";
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserTempAvatarCollection tempAvatars = new UserTempAvatarCollection(reader);
                    return tempAvatars;
                }
            }
        }


        public override bool HasUncheckAvatar()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT COUNT(*) FROM bx_UserTempAvatar";
                return query.ExecuteScalar<int>() > 0;
            }
        }

        public override UserTempAvatarCollection GetUserTempAvatars(IEnumerable<int> userIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_UserTempAvatar WHERE UserID IN ( @UserIds)";
                query.CommandType = CommandType.Text;
                query.CreateInParameter<int>("@UserIds", userIds);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserTempAvatarCollection tempAvatars = new UserTempAvatarCollection(reader);
                    return tempAvatars;
                }
            }
        }
    }
}