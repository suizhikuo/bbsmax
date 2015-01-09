-- =============================================
-- Author:		zzbird
-- Create date: 2007/1/22
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_FinalQuestion]
	@ThreadID int,
	@BestPostID int,
	@PostRewards varchar(8000)
AS
BEGIN

	SET NOCOUNT ON;

    DECLARE @ThreadType tinyint;
	SELECT @ThreadType = ThreadType FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID;

	IF @ThreadType <> 2
		RETURN (1); -- FinalQuestionStatus.NotQuestion
	ELSE BEGIN
		DECLARE @IsClosed bit,@ExpiresDate datetime;
		SELECT @IsClosed = IsClosed,@ExpiresDate=ExpiresDate FROM bx_Questions WITH (NOLOCK) WHERE ThreadID = @ThreadID;
		IF @IsClosed = 1 OR @ExpiresDate<GETDATE()
			RETURN (2);  --FinalQuestionStatus.Finaled
		ELSE BEGIN
			--TODO:Postid:Reward, 12:60,13:70,1:2,
---------------------------------------------
			DECLARE @i int,@j int,@PostID int,@Reward int,@TotalReward int,@RewardCount int,@CanGetRewardCount int;--@ErrorCode int
			SET @PostRewards=@PostRewards+N','
			SET @TotalReward=0
			SET @RewardCount=0
			
			SELECT @j=CHARINDEX(':',@PostRewards)
			SELECT @i=CHARINDEX(',',@PostRewards)
			
			--BEGIN TRANSACTION
			WHILE(@j>0 AND @i>2)
				BEGIN	
					SELECT @PostID=SUBSTRING(@PostRewards,0, @j)
					SELECT @PostRewards=SUBSTRING(@PostRewards,@j+1,len(@PostRewards)-@j)
					
					SELECT @i=CHARINDEX(',',@PostRewards)
					SELECT @Reward=SUBSTRING(@PostRewards,0, @i)
					SELECT @PostRewards=SUBSTRING(@PostRewards,@i+1,len(@PostRewards)-@i)
					
					SELECT @j=CHARINDEX(':',@PostRewards)
					SELECT @i=CHARINDEX(',',@PostRewards)
					
					SET @TotalReward=@TotalReward+@Reward
					SET @RewardCount=@RewardCount+1
					
					INSERT INTO [bx_QuestionRewards](ThreadID,PostID,Reward) VALUES(@ThreadID,@PostID,@Reward)
					
					IF(@@error<>0)
						BEGIN
							RETURN -1;
							--SET @ErrorCode=-1
							--GOTO Cleanup;
						END
				END
				
			SELECT @Reward=Reward,@CanGetRewardCount=RewardCount FROM [bx_Questions] WITH(NOLOCK) WHERE ThreadID=@ThreadID
			IF(@Reward<>@TotalReward)
				BEGIN
					RETURN 4;
					--SET @ErrorCode=4
					--GOTO Cleanup;
				END
			
			IF(@RewardCount>@CanGetRewardCount)
				BEGIN
					RETURN 3;
					--SET @ErrorCode=3
					--GOTO Cleanup;
				END
				
			UPDATE [bx_Questions] SET IsClosed=1,BestPostID=@BestPostID WHERE ThreadID=@ThreadID
			IF(@@error<>0)
				BEGIN
					RETURN -1;
    				--SET @ErrorCode=-1
					--GOTO Cleanup;
				END
			
			--COMMIT TRANSACTION
			RETURN (0)
			
			--Cleanup:
				--BEGIN
    				--ROLLBACK TRANSACTION
    				--RETURN (@ErrorCode)
				--END
---------------------------------------------
		END
	END

END


