-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/21>
-- Description:	<删除版主>
-- =============================================
CREATE PROCEDURE [bx_DeleteModerator]
	@ForumID int,
	@UserID int
AS
	SET NOCOUNT ON 
	/*
	DECLARE @MappedRoleID uniqueIdentifier
	
	SELECT @MappedRoleID=MappedRoleID FROM [bx_Moderators] WITH(NOLOCK) WHERE ForumID=@ForumID AND UserID=@UserID
	delete bx_Moderators where ForumID = @ForumID AND UserID = @UserID;
		IF @@ROWCOUNT > 0 BEGIN
			IF @MappedRoleID<>00000000  --如果COUNT(*)大于0 则不需要移除用户角色
				SELECT COUNT(1),@MappedRoleID FROM [bx_Moderators] WITH(NOLOCK) WHERE  MappedRoleID=@MappedRoleID AND UserID=@UserID
			RETURN (0);
		END
		ELSE
			RETURN (1);
	*/



