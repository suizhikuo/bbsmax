-- =============================================
-- Author:		SEK
-- Create date: <2007/1/4>
-- Description:	<修改精华>
-- =============================================
CREATE PROCEDURE [bx_SetThreadsValued] 
	@ForumID INT,
	@ThreadIdentities varchar(8000),
	@IsValued bit
AS
	SET NOCOUNT ON

	DECLARE @RowCount int

	EXEC ('UPDATE bx_Threads SET IsValued=' + @IsValued + ' WHERE ThreadID IN (' + @ThreadIdentities + ') AND ForumID = ' + @ForumID + ' AND IsValued<>' + @IsValued) 
	
	SET @RowCount=@@ROWCOUNT
	IF @RowCount> 0
		
		----------统计：增加删除精华数---------------
		IF @IsValued=0
			SELECT @RowCount= 0 - @RowCount;

		EXEC [bx_DoCreateStat] @ForumID,2, @RowCount
		--------------------------
	RETURN


