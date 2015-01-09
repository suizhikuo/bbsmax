-- =============================================
-- Author:		<zzbird>
-- Create date: <2006/12/8>
-- Description:	<获得主题和回复，并获得附件和评分记录>
-- =============================================
CREATE PROCEDURE [bx_GetThread]
    @ThreadID int,
    @NormalOnly bit,
    @GetThread bit,
    @GetReplies bit,
    @PageIndex int,
    @PageSize int,
	@PostUserID int = 0,
	@TotalCount INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @GetThread = 1 BEGIN
        SELECT *,IsClosed=CASE
WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
ELSE 0
END
          FROM [bx_Threads] WITH (NOLOCK)
         WHERE [ThreadID] = @ThreadID;

	END
		
	
    IF @GetReplies = 1 BEGIN
		DECLARE @BestPostID INT
		SET @BestPostID = 0
		
		IF @PostUserID<=0 AND @NormalOnly=1 BEGIN
			SELECT @BestPostID = ISNULL(BestPostID,0) FROM [bx_Questions] WITH (NOLOCK) WHERE ThreadID=@ThreadID
		END
			
        IF @PostUserID<=0 AND @NormalOnly=1 BEGIN
			IF @BestPostID > 0
				SELECT @TotalCount=COUNT(*) FROM [bx_Posts] WITH (NOLOCK) WHERE [ThreadID] = @ThreadID AND PostID<>@BestPostID AND [SortOrder]<4000000000000000;
			ELSE
				SELECT @TotalCount=COUNT(*) FROM [bx_Posts] WITH (NOLOCK) WHERE [ThreadID] = @ThreadID AND [SortOrder]<4000000000000000;
	    END
		ELSE IF @PostUserID<=0 AND @NormalOnly=0
			SELECT @TotalCount=COUNT(*) FROM [bx_Posts] WITH (NOLOCK) WHERE [ThreadID] = @ThreadID
			
			
		DECLARE @Condition nvarchar(4000);
		IF @PostUserID <= 0
			IF @NormalOnly=1
				IF @BestPostID > 0
					SET @Condition = '[ThreadID]=' + CAST(@ThreadID as varchar(16))+' AND PostID<>'+ CAST(@BestPostID as varchar(16))+' AND [SortOrder]<4000000000000000 ';
				ELSE
					SET @Condition = '[ThreadID]=' + CAST(@ThreadID as varchar(16))+' AND [SortOrder]<4000000000000000 ';
			ELSE
				SET @Condition = '[ThreadID]=' + CAST(@ThreadID as varchar(16))
		ELSE
			IF @NormalOnly=1
				SET @Condition = '[UserID] = ' + CAST(@PostUserID as varchar(16)) + ' AND [ThreadID]=' + CAST(@ThreadID as varchar(16))+' AND [SortOrder]<4000000000000000 ';
			ELSE
				SET @Condition = '[UserID] = ' + CAST(@PostUserID as varchar(16)) + ' AND [ThreadID]=' + CAST(@ThreadID as varchar(16));
				


		DECLARE @SQLString Nvarchar(4000);
		
		SET @SQLString='';
		
		IF @PostUserID>0
			SELECT @TotalCount=COUNT(*) FROM [bx_Posts] WITH (NOLOCK) WHERE [UserID]=@PostUserID AND [ThreadID] = @ThreadID AND [SortOrder]<4000000000000000;
		IF @TotalCount=0
			SET @TotalCount=1
		
		DECLARE @ResetOrder bit
		EXECUTE bx_Common_GetRecordsByPageSQLString 
					@PageIndex,
					@PageSize,
					N'bx_Posts',
					N'PostID,HistoryAttachmentIDs',
					@Condition,
					N'[SortOrder]',
					0,
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
		

		SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
		
		SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;
		SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
		
		SELECT m.* FROM [bx_PostMarks] m WITH (NOLOCK) INNER JOIN @PostIDTable i ON m.PostID = i.PostID ORDER BY m.PostMarkID DESC;')
		
		IF @PostUserID > 0
			SELECT @TotalCount

    END
     
END

