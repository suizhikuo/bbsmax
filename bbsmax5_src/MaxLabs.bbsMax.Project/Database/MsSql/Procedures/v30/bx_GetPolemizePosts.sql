CREATE PROCEDURE [bx_GetPolemizePosts]
	@ThreadID int,
	@PageIndex int,
	@PageSize int,
	@PostType tinyint
AS
	SET NOCOUNT ON;
	
	DECLARE @TotalCount INT;
	DECLARE @Condition nvarchar(4000);
	
	SELECT @TotalCount=COUNT(*) FROM [bx_Posts] WHERE [ThreadID] = @ThreadID AND  PostType = @PostType AND [SortOrder]<4000000000000000 ;
	
	
	SET @Condition = '[ThreadID]=' + CAST(@ThreadID as varchar(16))+' AND  PostType = '+CAST(@PostType AS VARCHAR(16))+' AND [SortOrder]<4000000000000000 ';
	
	DECLARE @SQLString Nvarchar(4000);
		
	SET @SQLString='';
	
	DECLARE @ResetOrder bit
	EXECUTE bx_Common_GetRecordsByPageSQLString 
				@PageIndex,
				@PageSize,
				N'bx_Posts',
				N'*',
				@Condition,
				N'[SortOrder]',
				1,
				@TotalCount,
				@ResetOrder OUTPUT,
				@SQLString OUTPUT
	
	EXECUTE (@SQLString)
	
	SELECT @TotalCount,@ResetOrder;
	
	
	EXECUTE bx_Common_GetRecordsByPageSQLString 
			@PageIndex,
			@PageSize,
			N'bx_Posts',
			N'PostID,HistoryAttachmentIDs',
			@Condition,
			N'[SortOrder]',
			1,
			@TotalCount,
			@ResetOrder OUTPUT,
			@SQLString OUTPUT
	
	EXEC ('DECLARE @PostIDTable table(ID int identity(1,1), PostID int NOT NULL, HistoryAttachmentIDs varchar(500) NULL);

	INSERT INTO @PostIDTable (PostID,HistoryAttachmentIDs) ' + @SQLString + ';
	
	DECLARE @HistoryAttach table(PostID int,AttachID int);
	DECLARE @Count int,@I int;
	SELECT @Count = COUNT(*) FROM @PostIDTable;
	SET @I = 1;
	
	WHILE(@I < @Count+1) BEGIN
		DECLARE @PID int,@HistoryAttachmentString varchar(500);
		SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs FROM @PostIDTable WHERE ID = @I;
		IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
			INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
		SET @I = @I + 1;
	END
	

	--SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
	
	SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;
	SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a, @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
	
	SELECT m.* FROM [bx_PostMarks] m WITH (NOLOCK) INNER JOIN @PostIDTable i ON m.PostID = i.PostID ORDER BY m.PostMarkID DESC;')
	
	--EXEC ('DECLARE @PostIDTable table(PostID int NOT NULL);

	--INSERT INTO @PostIDTable ' + @SQLString + ';

	----SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
	--SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;
	--SELECT m.* FROM [bx_PostMarks] m WITH (NOLOCK) INNER JOIN @PostIDTable i ON m.PostID = i.PostID ORDER BY m.PostMarkID DESC;')