-- =============================================
-- Author:		sek
-- Create date: 2008/1/14
-- Description:	<按回复时间获取主题>
-- =============================================
CREATE PROCEDURE [bx_GetThreadsByPostCreateDate]
	@CreateDate datetime
AS
BEGIN
	SET NOCOUNT ON;
	--DECLARE @T Table(TempPostID int,TempThreadID int)
	--INSERT INTO @T(TempPostID,TempThreadID)
	--SELECT Max(PostID),threadID FROM bx_Posts WHERE CreateDate>@CreateDate AND SortOrder < 4000000000000000 Group By ThreadID

	--SELECT T.*,(SELECT CreateDate From bx_Posts WHERE TempPostID=PostID) AS LastPostDate FROM bx_Threads T INNER JOIN @T ON ThreadID = TempThreadID 
	
	------------以下注释
	DECLARE @SortOrder BIGINT;
	
	EXEC [bx_GetSortOrder] 1, 0, @CreateDate, @SortOrder OUTPUT;
	
	--DECLARE @SortOrder BIGINT
	--SET @SortOrder = [dbo].bx_GetSortOrder(1,0, @CreateDate)
	
	SELECT T1.*
	,(SELECT TOP 1 PostID FROM bx_Posts WHERE ThreadID=T1.ThreadID ORDER BY SortOrder) AS FirstPostID
	--(SELECT MIN(PostID) FROM bx_Posts WHERE ThreadID=T1.ThreadID) AS FirstPostID
	--(SELECT Max(PostID) FROM bx_Posts WHERE ThreadID=T1.ThreadID AND SortOrder < 4000000000000000) AS LastPostID
	 FROM bx_Threads T1 WHERE SortOrder>@SortOrder AND SortOrder < 4000000000000000 ORDER BY SortOrder DESC
	
	/*
	SELECT t1.*,
	(SELECT CreateDate From bx_Posts WHERE t2.PostID=PostID) AS LastPostDate,
	(select min(PostID) from bx_Posts where ThreadID=t1.ThreadID) AS FirstPostID 
	--(select [Content] FROM bx_Posts where PostID =(select min(PostID) from bx_Posts where ThreadID=t1.ThreadID)) as [content] 
	 FROM bx_Threads t1 INNER JOIN (SELECT Max(PostID) as PostID,ThreadID as ThreadID FROM bx_Posts WHERE CreateDate>@CreateDate AND SortOrder < 4000000000000000 Group By ThreadID) t2 ON t1.ThreadID = t2.ThreadID ORDER BY LastPostDate DESC
	 
	 */
END