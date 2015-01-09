--发表新日志的触发器
EXEC bx_Drop 'bx_Posts_AfterDelete';
GO

CREATE TRIGGER [bx_Posts_AfterDelete] 
   ON  [bx_Posts] 
   AFTER DELETE
AS 
BEGIN
	SET NOCOUNT ON;
	DECLARE @tempTable TABLE(ForumID INT,PostCount INT,ReduceTodayPosts INT)--,LastThreadID INT)

	DECLARE @Today DateTime,@Monday DateTime;
	SET @Today = CONVERT(varchar(12) , getdate(), 102);
	
	DECLARE @m int;
	SELECT @m = DATEPART(weekday, @Today);
	IF @m = 1
		SELECT @m = 8;
	SELECT @Monday = DATEADD(day, 2-@m, @Today);

	--IF NOT EXISTS (SELECT TOP 1 * FROM bx_Posts WHERE ThreadID IN (SELECT DISTINCT ThreadID FROM deleted))
		--RETURN;

	insert into @tempTable(ForumID,PostCount,ReduceTodayPosts)--,LastThreadID)
	select distinct D.ForumID,
				(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
				,(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND D1.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=D.ForumID))
				--,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=D.ForumID AND SortOrder<2000000000000000))
				--,(SELECT ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WITH (NOLOCK) WHERE T1.ForumID = D.ForumID))
				FROM [deleted] D --Inner join [bx_Threads] T on D.ThreadID=T.ThreadID WHERE D.IsApproved=1

	----如果没有，说明这是对直接删除整个主题，可以直接终止 
	--IF NOT EXISTS (SELECT * FROM @tempTable)
			--RETURN;

	UPDATE bx_Forums SET 
					TotalPosts=TotalPosts-t.PostCount
					,TodayPosts=TodayPosts-t.ReduceTodayPosts
					--,LastThreadID=ISNULL(t.LastThreadID,0)
					from @tempTable as t WHERE bx_Forums.ForumID=t.ForumID


	
	 
	SELECT 'RecodeTodayPosts' AS XCMD, 'delete' as AC, T.ForumID AS ForumID,T.ReduceTodayPosts AS ReducePosts,F.TodayPosts,F.CodeName FROM @tempTable AS T INNER JOIN bx_Forums as F on T.ForumID = F.ForumID WHERE T.ReduceTodayPosts>0;



	DECLARE @tempTable2 TABLE(ThreadID INT,PostCount INT,LastPostUserID INT,LastPostUserNickName nvarchar(64))
	insert into @tempTable2(ThreadID,PostCount,LastPostUserID,LastPostUserNickName)
	select distinct D.ThreadID,
				--(SELECT COUNT(1) FROM [deleted] D1  Inner join [bx_Threads] T1 on D1.ThreadID=T1.ThreadID WHERE D1.IsApproved=1 AND T1.ThreadID=T.ThreadID)
				(SELECT COUNT(1) FROM [bx_Posts] P WITH(NOLOCK) WHERE P.ThreadID=D.ThreadID AND P.SortOrder<4000000000000000)
				,(SELECT top 1 UserID FROM [bx_Posts] where ThreadID=D.ThreadID  AND SortOrder<4000000000000000 Order by SortOrder desc)
				,(SELECT top 1 NickName FROM [bx_Posts] where ThreadID=D.ThreadID AND SortOrder<4000000000000000 Order by SortOrder desc)
				FROM [deleted] D --WHERE D.ForumID>0-- Inner join [bx_Threads] T on D.ThreadID=T.ThreadID

	UPDATE bx_Threads SET 
					TotalReplies=t.PostCount-1,--TotalReplies-t.PostCount,
					LastPostUserID=ISNULL(t.LastPostUserID,0),
					LastPostNickName=ISNULL(t.LastPostUserNickName,'') 
					from @tempTable2 as t WHERE bx_Threads.ThreadID=t.ThreadID


	DECLARE @FirstPostSortOrder TABLE(SortOrder BIGINT)
	INSERT INTO @FirstPostSortOrder(SortOrder)
	SELECT MIN(P.SortOrder) FROM [deleted] I INNER JOIN [bx_Posts] P ON I.ThreadID=P.ThreadID  GROUP BY I.ThreadID 

	IF EXISTS(SELECT * FROM [deleted] WHERE SortOrder IN(SELECT SortOrder FROM @FirstPostSortOrder)) BEGIN
		--这表示是删除主题时触发的，这时候只要重新统计主题用户的DeletedPosts，其他用户不重新统计--
		
		DECLARE @tempTable3 TABLE(userID INT,postCount INT,weekPostCount INT,dayPostCount INT, monthPostCount INT)
		insert into @tempTable3(userID,postCount,weekPostCount,dayPostCount,monthPostCount)
		select distinct UserID
						,(SELECT COUNT(*) FROM [deleted] POST /* INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */ WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND CreateDate>=@Monday)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND CreateDate>=@Today)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND DATEPART(year, GETDATE()) = DATEPART(year,CreateDate) AND DATEPART(month, GETDATE()) = DATEPART(month,CreateDate))
				FROM [deleted] p /* inner join [bx_Threads] t on p.ThreadID=t.ThreadID */

		UPDATE bx_Users SET
				TotalPosts=TotalPosts-t.postCount
				,WeekPosts = WeekPosts - t.weekPostCount
				,DayPosts = DayPosts - t.dayPostCount
				,MonthPosts = MonthPosts - t.monthPostCount
			FROM @tempTable3 as t where bx_Users.UserID = t.userID
		
		UPDATE bx_Users SET
				DeletedReplies=DeletedReplies+t.postCount
			FROM @tempTable3 as t where bx_Users.UserID = t.userID AND t.userID IN(SELECT DISTINCT UserID FROM [deleted] WHERE SortOrder IN(SELECT SortOrder FROM @FirstPostSortOrder))
	END 
	ELSE BEGIN
		--这表示是删除回复时触发的--
		DECLARE @tempTable4 TABLE(userID INT,postCount INT,weekPostCount INT,dayPostCount INT, monthPostCount INT)
		insert into @tempTable4(userID,postCount,weekPostCount,dayPostCount,monthPostCount)
		select distinct UserID
						,(SELECT COUNT(*) FROM [deleted] POST /* INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */ WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND CreateDate>=@Monday)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND CreateDate>=@Today)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND DATEPART(year, GETDATE()) = DATEPART(year,CreateDate) AND DATEPART(month, GETDATE()) = DATEPART(month,CreateDate))
				FROM [deleted] p /* inner join [bx_Threads] t on p.ThreadID=t.ThreadID */

		UPDATE bx_Users SET
				TotalPosts=TotalPosts-t.postCount
				,DeletedReplies=DeletedReplies+t.postCount
				,WeekPosts = WeekPosts - t.weekPostCount
				,DayPosts = DayPosts - t.dayPostCount
				,MonthPosts = MonthPosts - t.monthPostCount
			FROM @tempTable4 as t where bx_Users.UserID = t.userID
	END
	
	DELETE bx_Denouncings WHERE Type=7 AND TargetID IN (SELECT PostID FROM [DELETED]);
	
END



