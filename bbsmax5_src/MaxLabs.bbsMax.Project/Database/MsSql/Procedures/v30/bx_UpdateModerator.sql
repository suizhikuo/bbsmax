CREATE PROCEDURE [bx_UpdateModerator]
	@ForumID int,
	@UserID int,
	@MappedRoleID uniqueidentifier,
	@ExpiresDate datetime
AS
	SET NOCOUNT ON 
	DECLARE @OldMappedRoleID uniqueidentifier
	
	SELECT @OldMappedRoleID=MappedRoleID FROM [bx_Moderators] WITH(NOLOCK) WHERE ForumID=@ForumID AND UserID=@UserID
	
	IF @OldMappedRoleID <> @MappedRoleID BEGIN
		IF @OldMappedRoleID<>'00000000-0000-0000-0000-000000000000' -- 如果COUNT(*) = 0 则要移除该用户的角色 @OldMappedRoleID
			SELECT COUNT(1),@OldMappedRoleID FROM [bx_Moderators] WITH(NOLOCK) WHERE ForumID<>@ForumID AND UserID=@UserID AND MappedRoleID=@OldMappedRoleID
	END	
	
	UPDATE [bx_Moderators] SET 
		MappedRoleID=@MappedRoleID,
		ExpiresDate=@ExpiresDate
		WHERE ForumID=@ForumID AND UserID=@UserID