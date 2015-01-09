//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Xml;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

using System.Collections.ObjectModel;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    internal class DaoUtil
    {
        internal static string GetSafeString(string input)
        {
            string safeString = input.Replace("'", "''");
            safeString = safeString.Replace(";", "");
            safeString = safeString.Replace("%", "");
            safeString = safeString.Replace("#", "");
            //safeString = safeString.Replace("@", "");
            safeString = safeString.Replace("&", "");
            safeString = safeString.Replace("|", "");
            safeString = safeString.Replace("\"", "");
            safeString = safeString.Replace("=", "");
            safeString = safeString.Replace("(", "");
            safeString = safeString.Replace(")", "");

            return safeString;
        }

        /// <summary>
        /// 被排除用户组 SQL
        /// </summary>
        /// <param name="userIDFieldName">用户ID字段 名称 如"[UserID]"</param>
        /// <param name="excludeRoleIDs"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        internal static string GetExcludeRoleSQL(string userIDFieldName, IEnumerable<Guid> excludeRoleIDs, SqlQuery query)
        {

            if (excludeRoleIDs == null || !ValidateUtil.HasItems<Guid>(excludeRoleIDs))
            {
                return string.Empty;
            }
            else
            {
                //query.CreateInParameter<Guid>("@ExcludeRoleIds", excludeRoleIDs);
                return " " + userIDFieldName + " NOT IN(SELECT UserID FROM [bx_UserRoles] WHERE [RoleID] IN ('" + StringUtil.Join(excludeRoleIDs, "','") + "') AND [BeginDate] < getdate() AND [EndDate] > getdate())";
            }
        }


        /// <summary>
        /// 被排除用户组 SQL
        /// </summary>
        /// <param name="userIDFieldName">用户ID字段 名称 如"[UserID]"</param>
        /// <param name="editUserIDFieldName">编辑者字段名称 如"[LastEditUserID]" </param>
        /// <param name="operatorUserID">操作者</param>
        /// <param name="excludeRoleIDs"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        internal static string GetExcludeRoleSQL(string userIDFieldName, string editUserIDFieldName, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, SqlQuery query)
        {

            if (excludeRoleIDs == null || !ValidateUtil.HasItems<Guid>(excludeRoleIDs))
            {
                return string.Empty;
            }
            else
            {
                //query.CreateInParameter<Guid>("@ExcludeRoleIds", excludeRoleIDs);
                //query.CreateParameter<int>("@OperatorUserID", operatorUserID, SqlDbType.Int);

                return @"  
                            (
                               " + editUserIDFieldName + @" NOT IN(SELECT UserID FROM [bx_UserRoles] WHERE [RoleID] IN('" + StringUtil.Join(excludeRoleIDs, "','") + @"') AND [BeginDate]<getdate() AND [EndDate]>getdate())
                            OR (" + editUserIDFieldName + @" = " + operatorUserID + @" AND 
                                " + userIDFieldName + @" NOT IN(SELECT UserID FROM [bx_UserRoles] WHERE [RoleID] IN('" + StringUtil.Join(excludeRoleIDs, "','") + @"') AND [BeginDate]<getdate() AND [EndDate]>getdate())
                                )
                            )";
            }
        }
    }
}