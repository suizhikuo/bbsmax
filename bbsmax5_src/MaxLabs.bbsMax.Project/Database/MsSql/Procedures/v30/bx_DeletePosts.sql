-- =============================================
-- Author:		SEK
-- Create date: <2006/12/27>
-- Description:	<删除回复>
-- =============================================
CREATE PROCEDURE [bx_DeletePosts] 
	@ForumID int,
	@ThreadID int,
	@UserID int,
	@IsDeleteAnyPost bit,
	@PostIdentities varchar(8000)
AS
BEGIN

	SET NOCOUNT ON 

	IF(@IsDeleteAnyPost=1)
	BEGIN
		EXEC ('DELETE [bx_Posts] WHERE [ForumID]=' + @ForumID + ' AND [ThreadID]=' + @ThreadID + ' AND [PostID] IN (' + @PostIdentities + ')')
	END
	ELSE
		BEGIN
			DECLARE @Count int,@SQLString nvarchar(4000)
			SET @SQLString = N'SELECT @Count=count(*) FROM [bx_Posts] WITH(NOLOCK) WHERE UserID <> '+str(@UserID)+' AND [ForumID]=' + str(@ForumID) + ' AND [ThreadID]=' + str(@ThreadID) + ' AND [PostID] IN (' + @PostIdentities + ')'
			EXECUTE sp_executesql @SQLString,N'@Count int output',@Count output
			IF(@Count>0)
				RETURN 101
			ELSE
				EXEC ('DELETE [bx_Posts] WHERE [UserID]=' + @UserID + ' AND [ForumID]=' + @ForumID + ' AND [ThreadID]=' + @ThreadID + ' AND [PostID] IN (' + @PostIdentities + ')') 
		END
	DECLARE @RowCount int
	SET @RowCount = @@ROWCOUNT
	IF @RowCount > 0 BEGIN
		----------增加删除回复数---------------
		EXEC [bx_DoCreateStat] @ForumID,8, @RowCount
		--------------------------
		RETURN (0)
	END
	ELSE
		RETURN (1)

END