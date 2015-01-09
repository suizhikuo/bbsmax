
EXEC bx_Drop 'bx_Threads_AfterUpdate';
GO

CREATE TRIGGER [bx_Threads_AfterUpdate] 
   ON  [bx_Threads] 
   AFTER UPDATE
AS 
BEGIN

	SET NOCOUNT ON;
	

	IF UPDATE(ForumID) BEGIN
		--如果是合并版面导致的移动主题，就忽略下面的数据统计，直接结束
		DECLARE @ForumIDs nvarchar(64)
		EXEC bx_GetDisabledTriggerForumIDs
				@ForumIDs output
		IF(@ForumIDs<>'') BEGIN
			DECLARE @ForumID int,@ForumIDString nvarchar(64)
			SELECT top 1 @ForumID=ForumID FROM [deleted]
			SET @ForumIDString=Replace(','+STR(@ForumID)+',', ' ', '')
			SET @ForumIDs=','+@ForumIDs+','
			IF(CHARINDEX(@ForumIDString,@ForumIDs)>0) BEGIN
				RETURN;
			END
		END

		--移动之前的版块
		DECLARE @tempTable3 TABLE(ForumID INT,ThreadCount INT,/*PostCount INT,*/TodayThreadCount INT/*,TodayPostCount INT*/,LastThreadID INT)
		insert into @tempTable3(ForumID,ThreadCount,TodayThreadCount,LastThreadID)
		select distinct T.ForumID,
					(SELECT COUNT(1) FROM [deleted] D WITH(NOLOCK) WHERE D.ForumID = T.ForumID AND D.ThreadStatus<4)
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [deleted] D ON P.ThreadID=D.ThreadID WHERE D.ForumID = T.ForumID AND P.IsApproved=1)
					,(SELECT COUNT(1) FROM [deleted] D WITH (NOLOCK) WHERE D.ForumID = T.ForumID AND D.ThreadStatus<4 AND  DATEDIFF(day, D.CreateDate, getdate())=0)--D.ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [deleted] D ON P.ThreadID=D.ThreadID WHERE D.ForumID = T.ForumID AND P.IsApproved=1 AND P.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2 WITH(NOLOCK) WHERE T2.ForumID=T.ForumID AND T2.ThreadStatus = 1))
					FROM [deleted] T;
/*
					(SELECT COUNT(1) FROM [bx_Threads] T1 WITH (NOLOCK) WHERE T1.ForumID = T.ForumID),
					(SELECT COUNT(1) FROM [bx_Posts] WITH (NOLOCK) INNER JOIN bx_Threads WITH (NOLOCK) ON bx_Posts.ThreadID = bx_Threads.ThreadID WHERE (bx_Posts.IsApproved=1 AND bx_Threads.ForumID = T.ForumID)),
					(SELECT COUNT(1) FROM [bx_Threads] D WITH (NOLOCK) WHERE D.ForumID = T.ForumID AND D.ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID)),
					(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] D ON P.ThreadID=D.ThreadID WHERE D.ForumID = T.ForumID AND P.IsApproved=1 AND P.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID)),
					--(SELECT COUNT(1) FROM [deleted] D WITH (NOLOCK) WHERE D.ForumID = T.ForumID AND D.ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID)),
					--(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [deleted] D ON P.ThreadID=D.ThreadID WHERE D.ForumID = T.ForumID AND P.IsApproved=1 AND P.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID)),
					(SELECT ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WITH (NOLOCK) INNER JOIN bx_Threads T3 ON T1.ThreadID=T3.ThreadID WHERE T3.ForumID = T.ForumID AND T1.IsApproved=1))
					FROM [deleted] T;
*/

				UPDATE bx_Forums SET 
						TotalThreads=TotalThreads-t.ThreadCount 
						--,TotalPosts=TotalPosts-t.PostCount 
						,TodayThreads=TodayThreads-TodayThreadCount
						--,TodayPosts=TodayPosts-TodayPostCount
						,LastThreadID=ISNULL(t.LastThreadID,0)
						from @tempTable3 as t WHERE bx_Forums.ForumID=t.ForumID

		DECLARE @tempCatalogTable TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
		INSERT INTO @tempCatalogTable(TempForumID,TempCatalogID,TempThreadCount)
		SELECT distinct ForumID,
						ThreadCatalogID,
						(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
						FROM [deleted] D2
		
		UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads-TempThreadCount FROM @tempCatalogTable WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID
		
		

		--移动之后的版块
		DECLARE @tempTable4 TABLE(ForumID INT,ThreadCount INT,TodayThreadCount INT,LastThreadID INT)
		insert into @tempTable4(ForumID,ThreadCount,TodayThreadCount,LastThreadID)
		select distinct T.ForumID,
					(SELECT COUNT(1) FROM [inserted] I WITH(NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4)
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [inserted] I ON P.ThreadID=I.ThreadID WHERE I.ForumID = T.ForumID AND P.IsApproved=1)
					,(SELECT COUNT(1) FROM [inserted] I WITH (NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4 AND DATEDIFF(day, I.CreateDate, getdate())=0) --I.ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [inserted] I ON P.ThreadID=I.ThreadID WHERE I.ForumID = T.ForumID AND P.IsApproved=1 AND P.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					--,(SELECT ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WITH (NOLOCK) INNER JOIN bx_Threads T3 ON T1.ThreadID=T3.ThreadID WHERE T3.ForumID = T.ForumID AND T1.IsApproved=1))
					,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2 WITH(NOLOCK) WHERE T2.ForumID=T.ForumID AND T2.ThreadStatus=1))
					FROM [inserted] T;

		UPDATE bx_Forums SET 
						TotalThreads=TotalThreads+t.ThreadCount 
						--,TotalPosts=TotalPosts+t.PostCount 
						,TodayThreads=TodayThreads+TodayThreadCount
						--,TodayPosts=TodayPosts+TodayPostCount
						,LastThreadID=ISNULL(t.LastThreadID,0)
						from @tempTable4 as t WHERE bx_Forums.ForumID=t.ForumID


		DECLARE @tempCatalogTable2 TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
		INSERT INTO @tempCatalogTable2(TempForumID,TempCatalogID,TempThreadCount)
		SELECT distinct ForumID,
						ThreadCatalogID,
						(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
						FROM [inserted] D2
		
		UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+TempThreadCount FROM @tempCatalogTable2 WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID


		DECLARE @TempForumIDTable Table(TempThreadID INT,TempNewForumID INT)
		INSERT INTO @TempForumIDTable 
		SELECT ThreadID,ForumID  FROM [inserted]
				--,(SELECT N.ForumID FROM [inserted] N WHERE N.ThreadID=D.ThreadID)
				--FROM [deleted] D
		UPDATE bx_Posts SET ForumID=T.TempNewForumID from @TempForumIDTable T WHERE bx_Posts.ThreadID=T.TempThreadID

	END

	IF UPDATE(IsValued) BEGIN
		DECLARE @tempTable5 TABLE(userID INT,ValuedThreadCount INT,ValuedThreadCount2 INT)
		insert into @tempTable5(userID,ValuedThreadCount,ValuedThreadCount2)
		select distinct PostUserID
							--更新前的精华帖子数
							,(SELECT COUNT(1) FROM [deleted] WHERE PostUserID=p.PostUserID AND ThreadStatus<4 AND IsValued=1)
							--更新后的精华帖子数
							,(SELECT COUNT(1) FROM [inserted] WHERE PostUserID=p.PostUserID AND ThreadStatus<4 AND IsValued=1)
				FROM [deleted] p  --WHERE ThreadLocation=0

		UPDATE bx_Users SET
				ValuedTopics=ValuedTopics+(t.ValuedThreadCount2-t.ValuedThreadCount)
			 FROM @tempTable5 as t where bx_Users.UserID=t.userID
	END
	
	IF UPDATE(ThreadCatalogID) BEGIN
		DECLARE @tempCatalogTable3 TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
		INSERT INTO @tempCatalogTable3(TempForumID,TempCatalogID,TempThreadCount)
		SELECT distinct ForumID,
						ThreadCatalogID,
						(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
						FROM [deleted] D2
		
		UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads-TempThreadCount FROM @tempCatalogTable3 WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID
		
		DECLARE @tempCatalogTable4 TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
		INSERT INTO @tempCatalogTable4(TempForumID,TempCatalogID,TempThreadCount)
		SELECT distinct ForumID,
						ThreadCatalogID,
						(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
						FROM [inserted] D2
		
		UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+TempThreadCount FROM @tempCatalogTable4 WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID
	END
	
	IF UPDATE(ThreadStatus) BEGIN
		DECLARE @OldThreadStatus bigint,@NewThreadStatus bigint
		SELECT top 1 @OldThreadStatus=ThreadStatus FROM [deleted]
		SELECT top 1 @NewThreadStatus=ThreadStatus FROM [inserted]
		IF (@OldThreadStatus<4 AND @NewThreadStatus>=4) or (@OldThreadStatus>=4 AND @NewThreadStatus<4) BEGIN
				 
		    /*
			DECLARE @TempSortOrderTable Table(tempId int IDENTITY(1, 1),TempThreadID INT,TempNewSortOrder BIGINT)
			INSERT INTO @TempSortOrderTable (TempThreadID,TempNewSortOrder)
			SELECT ThreadID,SortOrder  FROM [inserted]
			*/
					--,(SELECT N.SortOrder FROM [inserted] N WHERE N.ThreadID=D.ThreadID)
					--FROM [deleted] D
----------------------------------------------------------------		
			DECLARE @PostTable table(tempPostTableId int IDENTITY(1, 1),TempPostTableThreadID INT,TempPostTablePostID Int,TempPostTableSortOrder BIGINT,Status int)
			INSERT INTO @PostTable(TempPostTableThreadID,TempPostTablePostID,TempPostTableSortOrder)
				SELECT ThreadID,PostID,SortOrder FROM [bx_Posts] WHERE ThreadID in(SELECT ThreadID FROM [inserted])
			
			DECLARE @i int,@Total int;
			/*
			SET @i = 0;
			SELECT @Total = COUNT(*) FROM @TempSortOrderTable;
			
			WHILE(@i<@Total) BEGIN
				SET @i = @i + 1;
				DECLARE @TempOldSortOrder bigint,@TempThreadID int;
			
				SELECT @TempThreadID = TempThreadID, @TempOldSortOrder = TempNewSortOrder FROM @TempSortOrderTable WHERE tempId = @i;
				
				DECLARE @Status int;
				IF @TempOldSortOrder >= 5000000000000000
					SET @Status = 5;
				ELSE IF @TempOldSortOrder>=4000000000000000 AND @TempOldSortOrder<5000000000000000
					SET @Status = 4;
				ELSE
					SET @Status = 1;
				
				UPDATE @PostTable SET Status=@Status WHERE TempPostTableThreadID = @TempThreadID;
				
				
			END
			
			
			SET @i = 0;
			SELECT @Total = COUNT(*) FROM @PostTable;
			WHILE(@i<@Total) BEGIN
				SET @i = @i + 1;
				
				DECLARE @TempOldPostSortOrder bigint,@PostSortOrderStatus int,@TempNewSortOrder bigint;
				SELECT @TempOldPostSortOrder = TempPostTableSortOrder,@PostSortOrderStatus = Status FROM @PostTable WHERE tempPostTableId = @i;
	
				EXEC [bx_UpdateSortOrder] @PostSortOrderStatus, @TempOldPostSortOrder, @TempNewSortOrder OUTPUT;
				
				UPDATE @PostTable SET TempPostTableSortOrder = @TempNewSortOrder WHERE tempPostTableId = @i; 
			END
			*/
			
			SET @i = 0;
			SELECT @Total = COUNT(*) FROM @PostTable;
			WHILE(@i<@Total) BEGIN
				SET @i = @i + 1;
				
				DECLARE @TempOldPostSortOrder bigint,@TempNewSortOrder bigint;
				
				SELECT @TempOldPostSortOrder = TempPostTableSortOrder FROM @PostTable WHERE tempPostTableId = @i;
	
				EXEC [bx_UpdateSortOrder] @NewThreadStatus, @TempOldPostSortOrder, @TempNewSortOrder OUTPUT;
				
				UPDATE @PostTable SET TempPostTableSortOrder = @TempNewSortOrder WHERE tempPostTableId = @i; 
			END
			
			UPDATE bx_Posts SET SortOrder=TempPostTableSortOrder from @PostTable T WHERE bx_Posts.PostID=T.TempPostTablePostID;
			
---------------上面这段 只相当于原来底下这3句 -------------------------------------
			
			--UPDATE bx_Posts SET SortOrder=[dbo].bx_UpdateSortOrder(5,SortOrder) from @TempSortOrderTable T WHERE bx_Posts.ThreadID=T.TempThreadID AND TempNewSortOrder >= 5000000000000000
			
			--UPDATE bx_Posts SET SortOrder=[dbo].bx_UpdateSortOrder(4,SortOrder) from @TempSortOrderTable T WHERE bx_Posts.ThreadID=T.TempThreadID AND TempNewSortOrder>=4000000000000000 AND TempNewSortOrder<5000000000000000
			
			--UPDATE bx_Posts SET SortOrder=[dbo].bx_UpdateSortOrder(1,SortOrder) from @TempSortOrderTable T WHERE bx_Posts.ThreadID=T.TempThreadID AND TempNewSortOrder<4000000000000000
		
				 
			DECLARE @tempForumTable TABLE(ForumID INT,ThreadCount1 INT,TodayThreadCount1 INT,ThreadCount2 INT,TodayThreadCount2 INT,LastThreadID INT)
			insert into @tempForumTable(ForumID,ThreadCount1,TodayThreadCount1,ThreadCount2,TodayThreadCount2,LastThreadID)
			select distinct T.ForumID,
						(SELECT COUNT(1) FROM [deleted] I WITH(NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4)
						,(SELECT COUNT(1) FROM [deleted] I WITH (NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4 AND DATEDIFF(day, I.CreateDate, getdate())=0)
						,(SELECT COUNT(1) FROM [inserted] I WITH(NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4)
						,(SELECT COUNT(1) FROM [inserted] I WITH (NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4 AND DATEDIFF(day, I.CreateDate, getdate())=0)
						,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=T.ForumID AND T2.ThreadStatus = 1))
						FROM [inserted] T;

			UPDATE bx_Forums SET 
							TotalThreads=TotalThreads+(t.ThreadCount2-t.ThreadCount1)
							,TodayThreads=TodayThreads+(t.TodayThreadCount2-t.TodayThreadCount1)
							,LastThreadID=ISNULL(t.LastThreadID,0)
							from @tempForumTable as t WHERE bx_Forums.ForumID=t.ForumID



			DECLARE @tempTable2 TABLE(userID INT,threadCount INT,valuedThreadCount int,threadCount2 INT,valuedThreadCount2 int)
			insert into @tempTable2(userID,threadCount,valuedThreadCount,threadCount2,valuedThreadCount2)
			select distinct PostUserID
								--更新前的正常的帖子数
								,(SELECT COUNT(1) FROM [deleted]  WHERE PostUserID=p.PostUserID AND ThreadStatus<4)
								,(SELECT COUNT(1) FROM [deleted]  WHERE PostUserID=p.PostUserID AND ThreadStatus<4 AND IsValued=1)
								--,(SELECT COUNT(1) FROM [bx_Posts] POST  INNER JOIN [deleted] D  ON POST.ThreadID=D.ThreadID WHERE POST.UserID=P.PostUserID AND POST.IsApproved=1 AND D.ForumID>0) 
								--更新后的正常的帖子数
								,(SELECT COUNT(1) FROM [inserted]  WHERE PostUserID=p.PostUserID AND ThreadStatus<4)
								,(SELECT COUNT(1) FROM [inserted]  WHERE PostUserID=p.PostUserID AND ThreadStatus<4 AND IsValued=1)
								--,(SELECT COUNT(1) FROM [bx_Posts] POST  INNER JOIN [inserted] I  ON POST.ThreadID=I.ThreadID WHERE POST.UserID=P.PostUserID AND POST.IsApproved=1 AND I.ForumID>0) 
					FROM [deleted] p  

			UPDATE bx_Users SET
					TotalTopics=TotalTopics+(t.threadCount2-t.threadCount),
					--TotalPosts=TotalPosts+(t.PostCount2-t.PostCount),
					--DeletedPosts=DeletedPosts-(t.threadCount2-t.threadCount),
					--DeletedThreads=DeletedThreads-(t.threadCount2-t.threadCount),
					ValuedTopics=ValuedTopics+(t.valuedThreadCount2-t.valuedThreadCount)
				 FROM @tempTable2 as t where bx_Users.UserID=t.userID
				 
				 
				 
			DECLARE @tempCatalogTable9 TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount1 INT,TempThreadCount2 INT)
			INSERT INTO @tempCatalogTable9(TempForumID,TempCatalogID,TempThreadCount1,TempThreadCount2)
			SELECT distinct ForumID,
							ThreadCatalogID,
							(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
							,(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
							FROM [deleted] D2
			
			UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+(TempThreadCount1-TempThreadCount2) FROM @tempCatalogTable9 WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID

	/*	
			DECLARE @tempCatalogTable10 TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
			INSERT INTO @tempCatalogTable10(TempForumID,TempCatalogID,TempThreadCount)
			SELECT distinct ForumID,
							ThreadCatalogID,
							(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D2.ForumID AND D1.SortOrder<4000000000000000 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
							FROM [inserted] D2
			
			UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+TempThreadCount FROM @tempCatalogTable10 WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID
	*/		
		END	
		
		
		--原来是正常的  现在变为不正常
		IF (@OldThreadStatus<4 AND @NewThreadStatus>=4) BEGIN
			-- 删除举报
			DELETE bx_Denouncings WHERE Type = 6 AND TargetID in(SELECT ThreadID FROM [deleted]); 
		END
	END
END


