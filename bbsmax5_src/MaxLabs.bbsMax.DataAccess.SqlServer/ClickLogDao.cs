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
using MaxLabs.bbsMax.Enums;
using System.Data;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class ClickLogDao : DataAccess.ClickLogDao
    {
        [StoredProcedure(Name = "bx_IsAvailibleClick", Script = @"
CREATE PROCEDURE {name} 
     @Ip                          varchar(50)
    ,@UserIdentify                varchar(200)
    ,@AllowUserLastClickDateTime  datetime
    ,@AllowIpLastClickDateTime    datetime
    ,@AllowIpTotalClicks            int
    ,@SourceType                  int
    ,@TargetID                      int
AS
BEGIN

 SET NOCOUNT ON;

DECLARE @Var1 int;

SET @Var1 = (SELECT COUNT(*) FROM bx_ClickLogs WHERE Ip = @Ip AND ClickDate >= @AllowIpLastClickDateTime AND SourceType = @SourceType AND TargetID = @TargetID);

IF @Var1>@AllowIpTotalClicks BEGIN
SELECT 0;
RETURN;
END

IF EXISTS (SELECT * FROM bx_ClickLogs WHERE UserIdentify = @UserIdentify  AND TargetID = @TargetID AND ClickDate > @AllowUserLastClickDateTime AND SourceType = @SourceType) BEGIN
    SELECT 0;
    RETURN;
END

INSERT INTO bx_ClickLogs(Ip,UserIdentify,SourceType, TargetID) VALUES(@Ip, @UserIdentify, @SourceType, @TargetID);
SELECT 1;

END
")]
        public override bool IsValidClick(string userOrGuestID, int targetId, string Ip, ClickSourceType type, int availibleUserInterval, DateTime AllowIpLastClickDateTime, int availibleIpClickCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_IsAvailibleClick";
                query.CommandType = System.Data.CommandType.StoredProcedure;
                query.CreateParameter<string>("@Ip", Ip, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@UserIdentify", userOrGuestID, SqlDbType.VarChar, 200);
                query.CreateParameter<DateTime>("@AllowUserLastClickDateTime", DateTimeUtil.Now.AddSeconds(-availibleUserInterval), SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@AllowIpLastClickDateTime", AllowIpLastClickDateTime, SqlDbType.DateTime);
                query.CreateParameter<int>("@AllowIpTotalClicks", availibleIpClickCount, SqlDbType.Int);
                query.CreateParameter<int>("@SourceType", (int)type, SqlDbType.Int);
                query.CreateParameter<int>("@TargetID", targetId, SqlDbType.Int);

                return (int)query.ExecuteScalar() == 1;
            }
        }
    }
}