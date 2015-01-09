
EXEC bx_Drop 'bx_Threads_AfterDelete';
GO

CREATE TRIGGER [bx_Threads_AfterDelete]
   ON bx_Threads
   AFTER DELETE--INSTEAD OF DELETE
AS 
BEGIN

SET NOCOUNT ON;

	DECLARE @ForumIDs nvarchar(64)
	EXEC bx_GetDisabledTriggerForumIDs
			@ForumIDs output

	IF(@ForumIDs<>'') BEGIN
		--如果是删除版面，就忽略下面的数据统计，直接结束--
		DECLARE @ForumID int,@ForumIDString nvarchar(64)
		SELECT top 1 @ForumID=ForumID FROM [deleted]
		SET @ForumIDString=Replace(','+STR(@ForumID)+',', ' ', '')
		SET @ForumIDs=','+@ForumIDs+','
		IF(CHARINDEX(@ForumIDString,@ForumIDs)>0) BEGIN
			--DELETE [bx_Posts] WHERE ThreadID in(SELECT ThreadID FROM [deleted])
			--delete [bx_Threads] where ThreadID in(SELECT ThreadID FROM [deleted])
			RETURN;
		END
	END
	DECLARE @tempTable2 TABLE(userID INT,threadCount INT,valuedThreadCount int/*,PostCount int*/)
	insert into @tempTable2(userID,threadCount,valuedThreadCount/*,PostCount*/)
	select distinct PostUserID
						,(SELECT COUNT(1) FROM [deleted] WHERE PostUserID=p.PostUserID AND ThreadStatus<4)
						,(SELECT COUNT(1) FROM [deleted] WHERE PostUserID=p.PostUserID AND ThreadStatus<4 AND IsValued=1)
						--,(SELECT COUNT(1) FROM [bx_Posts] POST WITH (NOLOCK) INNER JOIN [deleted] D  ON POST.ThreadID=D.ThreadID WHERE POST.UserID=P.PostUserID AND POST.SortOrder<4000000000000000) 
			FROM [deleted] p

	UPDATE bx_Users SET
			TotalTopics=TotalTopics-t.threadCount,
			--TotalPosts=TotalPosts-t.PostCount,
			--DeletedPosts=DeletedPosts+t.PostCount,--t.threadCount,
			ValuedTopics=ValuedTopics-t.valuedThreadCount FROM @tempTable2 as t where bx_Users.UserID=t.userID


--------------------------
	--delete [bx_Threads] where ThreadID in(SELECT ThreadID FROM [deleted])

	DECLARE @tempTable TABLE(ForumID INT,ThreadCount INT,/*PostCount INT,*/TodayThreads INT/*,TodayPosts INT*/,LastThreadID INT)
	insert into @tempTable(ForumID,ThreadCount,/*PostCount,*/TodayThreads/*,TodayPosts*/,LastThreadID)
	select distinct T.ForumID,
					(SELECT COUNT(1) FROM [deleted] D WITH (NOLOCK) WHERE D.ForumID = T.ForumID AND  D.ThreadStatus<4)
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [deleted] D WITH (NOLOCK) ON P.ThreadID = D.ThreadID WHERE P.ForumID = T.ForumID AND P.SortOrder<4000000000000000)
					,(SELECT COUNT(1) FROM [deleted] D WITH (NOLOCK) WHERE D.ForumID = T.ForumID AND  D.ThreadStatus<4 AND  DATEDIFF(day, D.CreateDate, getdate())=0)--D.ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [deleted] D ON P.ThreadID=D.ThreadID WHERE D.ForumID = T.ForumID AND  P.SortOrder<4000000000000000 AND  DATEDIFF(day, P.CreateDate, getdate())=0)--P.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					--,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=T.ForumID AND SortOrder<2000000000000000))
					--,(SELECT ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WHERE T1.ForumID = T.ForumID AND T1.ThreadID NOT IN(SELECT ThreadID FROM [deleted] WITH(NOLOCK))))
					,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=T.ForumID AND T2.ThreadStatus = 1))
					FROM [deleted] T;

	UPDATE bx_Forums SET 
					TotalThreads=TotalThreads-t.ThreadCount 
					--,TotalPosts=TotalPosts-t.PostCount 
					,bx_Forums.TodayThreads=bx_Forums.TodayThreads-t.TodayThreads
					--,bx_Forums.TodayPosts=bx_Forums.TodayPosts-t.TodayPosts
					,LastThreadID=ISNULL(t.LastThreadID,0)
					from @tempTable as t WHERE bx_Forums.ForumID=t.ForumID
	
	
	--DELETE [bx_Posts] FROM [deleted] d WHERE [bx_Posts].ThreadID = d.ThreadID
	--DELETE [bx_Posts] WHERE ThreadID in(SELECT ThreadID FROM [deleted])
	--delete [bx_Threads] where ThreadID in(SELECT ThreadID FROM [deleted])
	
	DECLARE @tempCatalogTable TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
	INSERT INTO @tempCatalogTable(TempForumID,TempCatalogID,TempThreadCount)
	SELECT distinct ForumID,
					ThreadCatalogID,
					(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadCatalogID=D2.ThreadCatalogID AND D1.ThreadStatus<4) 
					FROM [deleted] D2
	
	UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads-TempThreadCount FROM @tempCatalogTable WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID
	
	DELETE bx_Denouncings WHERE Type=6 AND TargetID IN (SELECT [ThreadID] FROM [DELETED]);
	
END


