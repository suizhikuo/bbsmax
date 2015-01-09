-- =============================================
-- Author:		<zzbird>
-- Create date: <2007/2/13>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_DeleteThreads]
	@ForumID int,
	@ThreadStatus tinyint,
	@ThreadIdentities varchar(8000)
AS
	SET NOCOUNT ON 
--	BEGIN TRANSACTION
--	EXECUTE bx_UpdateUserThreadsCount
--			0,
--			0,
--			0,
--			@ThreadIdentities
--	IF(@@error<>0)
--		GOTO Cleanup; 
	IF @ThreadStatus<4
		EXEC ('DELETE [bx_Threads] WHERE  [ForumID]=' + @ForumID+' AND [ThreadID] IN (' + @ThreadIdentities + ')')
	ELSE IF @ThreadStatus=4
		EXEC ('DELETE [bx_Threads] WHERE  [ForumID]=' + @ForumID+' AND [ThreadID] IN (' + @ThreadIdentities + ') AND SortOrder>=4000000000000000 AND SortOrder<5000000000000000')
	ELSE
		EXEC ('DELETE [bx_Threads] WHERE  [ForumID]=' + @ForumID+' AND [ThreadID] IN (' + @ThreadIdentities + ') AND SortOrder>=5000000000000000')
--	IF(@@error<>0)
--		GOTO Cleanup; 

	DECLARE @RowCount int
	SET @RowCount=@@ROWCOUNT
	IF @RowCount> 0
	BEGIN
		----------记录统计数据:增加删除主题数---------------
		EXEC [bx_DoCreateStat] @ForumID,7, @RowCount
		--------------------------
--		COMMIT TRANSACTION;
		RETURN (0);
	END
	ELSE
	begin
--		ROLLBACK TRANSACTION
		RETURN (1)
	end

	
--
--Cleanup:
--    BEGIN
--    	ROLLBACK TRANSACTION
--    	RETURN (-1)
--    END


