-- =============================================
-- Author:		帅帅
-- Create date: <2007/3/13>
-- Description:	<移动版面>
-- =============================================
CREATE PROCEDURE [bx_MoveForum]
@forumID int,
@parentID int
AS
BEGIN
	SET NOCOUNT ON;
IF (EXISTS(SELECT * FROM [bx_Forums] WITH(NOLOCK) WHERE ForumID=@parentID ) or @parentID=0)
	BEGIN
		DECLARE @MaxSortOrder int
		SELECT @MaxSortOrder=MAX(SortOrder) FROM bx_Forums WITH(NOLOCK) WHERE ParentID=@parentID
		IF @MaxSortOrder IS NULL
			SET @MaxSortOrder=0
		UPDATE bx_Forums 
		SET ParentID=@parentID, SortOrder=@MaxSortOrder+1
		WHERE ForumID = @forumID
		RETURN (0)
	END
ELSE
	RETURN (1)
END

