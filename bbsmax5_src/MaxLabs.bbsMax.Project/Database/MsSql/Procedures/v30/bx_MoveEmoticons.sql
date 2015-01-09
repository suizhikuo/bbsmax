-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/13>
-- Description:	<批量移动表情到另一组>
-- =============================================
CREATE PROCEDURE [bx_MoveEmoticons]
	@GroupID int,
	@NewGroupID int,
	@UserID int,
	@EmoticonIdentities varchar(8000)
AS
	SET NOCOUNT ON
	IF (EXISTS (SELECT * FROM [bx_EmoticonGroups] WITH (NOLOCK) WHERE GroupID=@GroupID and UserID=@UserID) AND 
	    EXISTS (SELECT * FROM [bx_EmoticonGroups] WITH (NOLOCK) WHERE GroupID=@NewGroupID and UserID=@UserID))
	BEGIN
		DECLARE @MaxSortOrder int
		SELECT @MaxSortOrder=MAX(SortOrder)+1 FROM [bx_Emoticons] WITH (NOLOCK) WHERE GroupID=@NewGroupID
		IF(@MaxSortOrder IS NULL)
			SET @MaxSortOrder=0
		EXEC('Update [bx_Emoticons] SET GroupID = ' + @NewGroupID + ',SortOrder=SortOrder+'+@MaxSortOrder+' WHERE EmoticonID IN ('+ @EmoticonIdentities +') and GroupID = '+@GroupID);
		RETURN (0)
	END
	ELSE
	RETURN (-1)