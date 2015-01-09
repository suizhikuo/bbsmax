CREATE PROCEDURE [bx_CheckModeratorRole]
	@UserID int,
	@MappedRoleID uniqueIdentifier
AS

	SET NOCOUNT ON;
	
	SELECT COUNT(1) FROM bx_Moderators WITH (NOLOCK) WHERE UserID=@UserID AND MappedRoleID=@MappedRoleID