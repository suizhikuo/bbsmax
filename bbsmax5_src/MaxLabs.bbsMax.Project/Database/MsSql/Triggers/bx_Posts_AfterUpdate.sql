--发表新日志的触发器
EXEC bx_Drop 'bx_Posts_AfterUpdate';
GO

CREATE TRIGGER [bx_Posts_AfterUpdate] 
   ON  [bx_Posts] 
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
		--DECLARE @tempForumID1 int,@tempForumID2 int
		--SELECT @tempForumID1 = TOP 1 ForumID FROM [deleted]
		--SELECT @tempForumID2 = TOP 1 ForumID FROM [inserted]
		--IF @tempForumID1 <> -2 AND @tempForumID2<>-2 --说明不是审核相关 退出
			--RETURN
			
		--更新之前的版快
		DECLARE @tempTable TABLE(ForumID INT,PostCount1 INT,PostCount2 INT,TodayPosts1 INT,TodayPosts2 INT)--,LastThreadID INT)--,PostCount2 INT)
		insert into @tempTable(ForumID,PostCount1,PostCount2,TodayPosts1,TodayPosts2)--,LastThreadID)--,PostCount2)
		select distinct ForumID,
			(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [deleted] D1 WHERE  D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND DATEDIFF(day, D1.CreateDate, getdate())=0)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND DATEDIFF(day, D1.CreateDate, getdate())=0)
			--,(SELECT ThreadID FROM [bx_Threads] T WHERE T.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2 WITH(NOLOCK) WHERE T2.ForumID=D.ForumID AND SortOrder<2000000000000000))
			FROM [deleted] D --Inner join [bx_Threads] T  on D.ThreadID=T.ThreadID 

		UPDATE bx_Forums SET 
					TotalPosts=TotalPosts+(t.PostCount2-t.PostCount1)
					,TodayPosts=TodayPosts+(t.TodayPosts2-t.TodayPosts1)
					--,LastThreadID=ISNULL(t.LastThreadID,0)
					from @tempTable as t WHERE bx_Forums.ForumID=t.ForumID

		


		---
		DECLARE @tempTable0 TABLE(ForumID INT,PostCount1 INT,PostCount2 INT,TodayPosts1 INT,TodayPosts2 INT)--,LastThreadID INT)--,PostCount2 INT)
		insert into @tempTable0(ForumID,PostCount1,PostCount2,TodayPosts1,TodayPosts2)--,LastThreadID)--,PostCount2)
		select distinct ForumID,
			(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [deleted] D1 WHERE  D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND DATEDIFF(day, D1.CreateDate, getdate())=0)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE  D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND  DATEDIFF(day, D1.CreateDate, getdate())=0)
			--,(SELECT ThreadID FROM [bx_Threads] T WHERE T.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=D.ForumID AND SortOrder<2000000000000000))
			FROM [inserted] D 
			
		UPDATE bx_Forums SET 
					TotalPosts=TotalPosts+(t.PostCount2-t.PostCount1)
					,TodayPosts=TodayPosts+(t.TodayPosts2-t.TodayPosts1)
					--,LastThreadID=ISNULL(t.LastThreadID,0)
					from @tempTable0 as t WHERE bx_Forums.ForumID=t.ForumID	
			
		
		
		SELECT 'RecodeTodayPosts' AS XCMD,'updateForumID' as AC,T.ForumID as ForumID,(T.TodayPosts1-T.TodayPosts2) as ReducePosts,F.TodayPosts,F.CodeName FROM @tempTable AS T inner join bx_Forums as F on T.ForumID = F.ForumID WHERE T.TodayPosts1>T.TodayPosts2
		UNION ALL 
		SELECT 'RecodeTodayPosts' AS XCMD,'updateForumID' as AC,T.ForumID as ForumID,(T.TodayPosts1-T.TodayPosts2) as ReducePosts,F.TodayPosts,F.CodeName FROM @tempTable0 AS T inner join bx_Forums as F on T.ForumID = F.ForumID WHERE T.TodayPosts1>T.TodayPosts2
		;

	END
	
	IF UPDATE(SortOrder) BEGIN
	
	
		DECLARE @tempTable4 TABLE(ForumID INT,PostCount1 INT,PostCount2 INT,TodayPosts1 INT,TodayPosts2 INT)--,LastThreadID INT)--,PostCount2 INT)
		insert into @tempTable4(ForumID,PostCount1,PostCount2,TodayPosts1,TodayPosts2)--,LastThreadID)--,PostCount2)
		select distinct ForumID,
			(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [deleted] D1 WHERE  D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND DATEDIFF(day, D1.CreateDate, getdate())=0)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE  D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND  DATEDIFF(day, D1.CreateDate, getdate())=0)
			--,(SELECT ThreadID FROM [bx_Threads] T WHERE T.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=D.ForumID AND SortOrder<2000000000000000))-- 更新最后回复的主题（此处用[bx_Threads]不准确（忽略），只是用于当主题被回收被还原等操作时更新）
			FROM [inserted] D 
			
		UPDATE bx_Forums SET 
					TotalPosts=TotalPosts+(t.PostCount2-t.PostCount1)
					,TodayPosts=TodayPosts+(t.TodayPosts2-t.TodayPosts1)
					--,LastThreadID=ISNULL(t.LastThreadID,0)
					from @tempTable4 as t WHERE bx_Forums.ForumID=t.ForumID	
	
	
		SELECT 'RecodeTodayPosts' AS XCMD,'updateSortOrder' as AC,T.ForumID as ForumID,(T.TodayPosts1-T.TodayPosts2) as ReducePosts,F.TodayPosts,F.CodeName FROM @tempTable4 AS T inner join bx_Forums as F on T.ForumID = F.ForumID WHERE T.TodayPosts1>T.TodayPosts2


	
		DECLARE @FirstPostSortOrder TABLE(SortOrder BIGINT)
		INSERT INTO @FirstPostSortOrder(SortOrder)
		SELECT MIN(P.SortOrder) FROM [inserted] I INNER JOIN [bx_Posts] P ON I.ThreadID=P.ThreadID  GROUP BY I.ThreadID 
		
	
	
		DECLARE @tempTable2 TABLE(ThreadID INT,PostCount INT,LastPostUserID INT,LastPostUserNickName nvarchar(64))
		insert into @tempTable2(ThreadID,PostCount,LastPostUserID,LastPostUserNickName)
		select distinct D.ThreadID,
					(SELECT COUNT(1) FROM [bx_Posts] t1 where /*t1.ForumID = D.ForumID AND*/ t1.ThreadID=D.ThreadID AND t1.SortOrder<5000000000000000) --(此处用5000000000000000是因为回收站的里的主题也统计回复数，而回收站里的主题的回复的SortOrder是大于4000000000000000，而正常的主题回复的SortOrder是不可能有大于4000000000000000小于5000000000000000的）
					--(SELECT COUNT(1) FROM [bx_Posts] p1 inner join [bx_Threads] t1 on p1.ThreadID=t1.ThreadID where t1.ThreadID=T.ThreadID AND p1.IsApproved=1) 
--					(SELECT COUNT(1) FROM [deleted] D1  Inner join [bx_Threads] T1  on D1.ThreadID=T1.ThreadID WHERE T1.ThreadID=T.ThreadID AND ThreadLocation=0 AND IsApproved=1)
--					,(SELECT COUNT(1) FROM [inserted] D1  Inner join [bx_Threads] T1  on D1.ThreadID=T1.ThreadID WHERE T1.ThreadID=T.ThreadID AND ThreadLocation=0 AND IsApproved=1)
					,(SELECT top 1 UserID FROM [bx_Posts]  where /*ForumID = D.ForumID AND*/ ThreadID=D.ThreadID AND SortOrder<5000000000000000 Order by SortOrder desc)
					,(SELECT top 1 NickName FROM [bx_Posts]  where /*ForumID = D.ForumID AND*/ ThreadID=D.ThreadID AND SortOrder<5000000000000000 Order by SortOrder desc)
					FROM [inserted] D WHERE D.ThreadID NOT IN(SELECT ThreadID FROM [inserted] WHERE SortOrder IN(SELECT SortOrder FROM @FirstPostSortOrder)) --Inner join [bx_Threads] T  on D.ThreadID=T.ThreadID --AND D.IsApproved=1
											--WHERE后面 表示如果是主题被回收，还原，等操作则不修改主题的相关信息--
		UPDATE bx_Threads SET 
						TotalReplies=t.PostCount-1,
						LastPostUserID=ISNULL(t.LastPostUserID,0),
						LastPostNickName=ISNULL(t.LastPostUserNickName,'') 
						from @tempTable2 as t WHERE bx_Threads.ThreadID=t.ThreadID 


		--IF EXISTS(SELECT * FROM [deleted] WHERE SortOrder IN(SELECT SortOrder FROM @FirstPostSortOrder)) BEGIN
			--这表示是回收主题时触发的，这时候只要重新统计主题用户的DeletedPosts，其他用户不重新统计--
			
			DECLARE @Today DateTime,@Monday DateTime;
			SET @Today = CONVERT(varchar(12) , getdate(), 102);
			
			DECLARE @m int;
			SELECT @m = DATEPART(weekday, @Today);
			IF @m = 1
				SELECT @m = 8;
			SELECT @Monday = DATEADD(day, 2-@m, @Today);
			
			-- 回收主题，还原主题，审核主题 审核回复 --
			DECLARE @tempTable3 TABLE(userID INT,postCount INT,postCount2 INT,weekPostCount INT,weekPostCount2 INT,dayPostCount INT,dayPostCount2 INT,monthPostCount INT,monthPostCount2 INT,TempLastPostDate DateTime)
			insert into @tempTable3(userID,postCount,postCount2,weekPostCount,weekPostCount2,dayPostCount,dayPostCount2,monthPostCount,monthPostCount2,TempLastPostDate)
			select distinct UserID
								,(SELECT COUNT(*) FROM [deleted] POST /*INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000) 
								,(SELECT COUNT(*) FROM [inserted] POST /*INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000) 
								
								,(SELECT COUNT(*) FROM [deleted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND CreateDate>=@Monday) 
								,(SELECT COUNT(*) FROM [inserted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND CreateDate>=@Monday) 
								
								,(SELECT COUNT(*) FROM [deleted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND CreateDate>=@Today) 
								,(SELECT COUNT(*) FROM [inserted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND CreateDate>=@Today) 
								
								,(SELECT COUNT(*) FROM [deleted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND DATEPART(year, GETDATE()) = DATEPART(year,CreateDate) AND DATEPART(month, GETDATE()) = DATEPART(month,CreateDate)) 
								,(SELECT COUNT(*) FROM [inserted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND DATEPART(year, GETDATE()) = DATEPART(year,CreateDate) AND DATEPART(month, GETDATE()) = DATEPART(month,CreateDate)) 

								,(SELECT MAX(CreateDate) FROM [inserted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000) 
					FROM [deleted] p  

			UPDATE bx_Users SET
					 TotalPosts=TotalPosts+(t.postCount2-t.postCount)
					,WeekPosts=WeekPosts+(t.weekPostCount2-t.weekPostCount)
					,DayPosts=DayPosts+(t.dayPostCount2-t.dayPostCount)
					,MonthPosts = MonthPosts +(t.monthPostCount2 - t.monthPostCount)
					FROM @tempTable3 as t where bx_Users.UserID=t.userID
			
			UPDATE bx_Users SET
					 LastPostDate = TempLastPostDate
					 FROM @tempTable3 as t WHERE bx_Users.UserID=t.userID AND TempLastPostDate IS NOT NULL AND bx_Users.LastPostDate<TempLastPostDate; 

			UPDATE bx_Users SET
					DeletedReplies=DeletedReplies+t.postCount
					FROM @tempTable3 as t where bx_Users.UserID=t.userID  AND t.userID IN(SELECT DISTINCT UserID FROM [deleted] WHERE SortOrder IN(SELECT SortOrder FROM @FirstPostSortOrder))
		--END 
		--ELSE BEGIN
			----这表示是回收回复时触发的--
			--DECLARE @tempTable5 TABLE(userID INT,postCount INT,postCount2 INT) 
			--insert into @tempTable5(userID,postCount,postCount2)
			--select distinct UserID
								--,(SELECT COUNT(1) FROM [deleted] POST /*INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */WHERE Post.UserID=p.UserID AND POST.SortOrder<4000000000000000) 
								--,(SELECT COUNT(1) FROM [inserted] POST /*INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */WHERE Post.UserID=p.UserID AND POST.SortOrder<4000000000000000) 
					--FROM [deleted] p  

			--UPDATE bx_Users SET
					--TotalPosts=TotalPosts+(t.PostCount2-t.PostCount),
					--DeletedPosts=DeletedPosts+t.PostCount
					--FROM @tempTable5 as t where bx_Users.UserID=t.userID
		--END
	END
END
