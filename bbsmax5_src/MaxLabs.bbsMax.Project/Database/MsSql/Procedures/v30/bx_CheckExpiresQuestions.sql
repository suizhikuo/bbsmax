---- =============================================
---- Author:		SEK
---- Create date: <2007/1/23>
---- Description:	<检查过期的提问，全部取得并交给服务器代码处理>
---- =============================================
--CREATE PROCEDURE [bx_CheckExpiresQuestions] 
--AS
	--SET NOCOUNT ON
	
	----清除搜索结果数据
	--DELETE bx_SearchPostResults WHERE UpdateDate < DATEADD(minute, -30, getdate());
	
	------查处过期的投票
	----SELECT ThreadID FROM [bx_Polls] WITH(NOLOCK) WHERE IsExpired = 0 AND ExpiresDate<getdate()
	
	------直接结束过期的投票
	----UPDATE [bx_Polls] SET IsExpired = 1 WHERE IsExpired = 0 AND ExpiresDate<getdate()
	
	----查出过期的提问，交给服务器代码处理
	--SELECT Q.*, T.PostUserID,T.ForumID FROM [bx_Questions] Q INNER JOIN [bx_Threads] T ON Q.ThreadID = T.ThreadID WHERE Q.IsClosed = 0 AND Q.ExpiresDate < getdate()