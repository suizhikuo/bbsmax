CREATE VIEW bx_BanUserViewLogs
AS
SELECT     bx_BanUserLogs.LogID, bx_BanUserLogs.OperationType, bx_BanUserLogs.OperationTime, bx_BanUserLogs.OperatorName, 
                      bx_BanUserLogs.Cause, bx_BanUserLogs.UserID, bx_BanUserLogs.Username, bx_BanUserLogs.UserIP, bx_BanUserLogForumInfos.ForumID, 
                      bx_BanUserLogForumInfos.EndDate, bx_Forums.ForumName, bx_BanUserLogs.AllBanEndDate
FROM         bx_BanUserLogs LEFT OUTER JOIN
                      bx_BanUserLogForumInfos ON bx_BanUserLogs.LogID = bx_BanUserLogForumInfos.LogID LEFT OUTER JOIN
                      bx_Forums ON bx_Forums.ForumID = bx_BanUserLogForumInfos.ForumID