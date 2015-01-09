CREATE PROCEDURE [bx_ApprovePosts]
	@PostIdentities varchar(8000)
AS
	SET NOCOUNT ON
	
	EXEC('SELECT DISTINCT(ThreadID) FROM [bx_Posts] WHERE PostID IN('+@PostIdentities+') AND SortOrder >= 5000000000000000')
	
	
	DECLARE @TempTable table(tempId int IDENTITY(1, 1),TempPostID int,TempSortOrder bigint);
	
	INSERT INTO @TempTable(TempPostID) SELECT item FROM bx_GetIntTable(@PostIdentities, ',');
	
	UPDATE @TempTable SET TempSortOrder = SortOrder FROM [bx_Posts] WHERE TempPostID = PostID;
	
	DECLARE @i int,@Total int;
	SET @i = 0;
	SELECT @Total = COUNT(*) FROM @TempTable;
	
	WHILE(@i<@Total) BEGIN
		SET @i = @i + 1;
		DECLARE @SortOrder bigint,@OldSortOrder bigint;
	
		SELECT @OldSortOrder = TempSortOrder FROM @TempTable WHERE tempId = @i;
		EXEC [bx_UpdateSortOrder] 1, @OldSortOrder, @SortOrder OUTPUT;
		
		UPDATE @TempTable SET TempSortOrder = @SortOrder WHERE tempId = @i; 
	END
	
	UPDATE [bx_Posts] SET SortOrder=TempSortOrder FROM @TempTable WHERE PostID = TempPostID AND SortOrder >= 5000000000000000;
	
	--EXEC('UPDATE [bx_Posts] SET SortOrder=[dbo].bx_UpdateSortOrder(1,SortOrder) WHERE PostID IN('+@PostIdentities+') AND bx_Posts.SortOrder >= 5000000000000000')
