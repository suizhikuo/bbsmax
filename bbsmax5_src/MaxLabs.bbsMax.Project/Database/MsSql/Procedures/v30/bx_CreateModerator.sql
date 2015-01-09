-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/21>
-- Description:	<创建一个版主>
-- =============================================
CREATE PROCEDURE bx_CreateModerator 
	@ForumID int,
	@UserID int,
	@MappedRoleID UniqueIdentifier,
	@ExpiresDate datetime
AS
	SET NOCOUNT ON
	
	IF EXISTS (SELECT * FROM [bx_Moderators] WITH (NOLOCK) WHERE UserID=@UserID AND ForumID=@ForumID)
			RETURN (1) 
	ELSE
		BEGIN
		declare @SortOrder int,@Condition varchar(100)
		SET @Condition='ForumID='+str(@ForumID)
		EXECUTE bx_Common_GetSortOrder 'bx_Moderators', @Condition,@SortOrder output
		INSERT INTO [bx_Moderators] (
		[ForumID],
		[UserID],
		[MappedRoleID],
		[ExpiresDate],
		[SortOrder]
		) VALUES (
		@ForumID,
		@UserID,
		@MappedRoleID,
		@ExpiresDate,
		@SortOrder
		)
		RETURN (0)
		END


