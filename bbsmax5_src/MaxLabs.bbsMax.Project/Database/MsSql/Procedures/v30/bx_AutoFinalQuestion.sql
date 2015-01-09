--CREATE PROCEDURE [bx_AutoFinalQuestion]
	--@ThreadID int, 
	--@UserID int,
	--@RewardCount int,
	--@TotalReward int
--AS
--BEGIN

	--SET NOCOUNT ON;

	--DECLARE @Reward int,@PostCount int
	--SELECT @PostCount = COUNT(*) FROM [bx_Posts] WITH (NOLOCK) WHERE ThreadID = @ThreadID AND SortOrder < 4000000000000000 AND UserID <> @UserID;
	
	--IF(@PostCount = 0)
	--BEGIN
		--UPDATE [bx_Questions] SET IsClosed = 1 WHERE ThreadID = @ThreadID
	--END
	--ELSE BEGIN
-----------------------------------------
		--IF(@PostCount < @RewardCount)
			--SET @RewardCount = @PostCount;

		--SET @Reward = @TotalReward / @RewardCount;
		
		--BEGIN TRANSACTION
		--EXEC('INSERT [bx_QuestionRewards](PostID,ThreadID,Reward) SELECT TOP '+@RewardCount+' PostID,'+@ThreadID+','+@Reward+' FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID='+@ThreadID+' AND SortOrder < 4000000000000000 AND UserID <> ' + @UserID + ' ORDER BY PostID')
		--IF(@@ERROR <> 0) BEGIN
		   --ROLLBACK TRANSACTION
		   --RETURN
		--END
		
		--UPDATE [bx_QuestionRewards] SET Reward = Reward + @TotalReward % @RewardCount WHERE PostID = (SELECT MIN(PostID) FROM [bx_QuestionRewards] WHERE ThreadID = @ThreadID)
		--IF(@@ERROR <> 0) BEGIN
		   --ROLLBACK TRANSACTION
		   --RETURN
		--END
		
		--UPDATE [bx_Questions] SET IsClosed = 1 WHERE ThreadID = @ThreadID
		--IF(@@ERROR <> 0) BEGIN
		   --ROLLBACK TRANSACTION
		   --RETURN
		--END
		
		--COMMIT TRANSACTION
		
		--SELECT UserID, SUM(Q.Reward) AS Reward FROM bx_Posts P INNER JOIN bx_QuestionRewards Q ON P.PostID = Q.PostID
			--WHERE P.ThreadID = @ThreadID GROUP BY UserID;
----------------------------------------------
	--END
--END