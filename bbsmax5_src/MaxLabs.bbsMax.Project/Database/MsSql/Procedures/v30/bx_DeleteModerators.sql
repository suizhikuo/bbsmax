CREATE PROCEDURE [bx_DeleteModerators]
	@ForumID INT
AS
	
	SET NOCOUNT ON 
	/*
	--返回需要移除角色的用户--
	SELECT UserID,MappedRoleID FROM [bx_Moderators] WITH(NOLOCK) WHERE ForumID = @ForumID AND MappedRoleID<>0 AND UserID NOT IN(
		SELECT M1.UserID FROM [bx_Moderators] M1 WITH(NOLOCK) INNER JOIN [bx_Moderators] M2 WITH(NOLOCK) ON M1.MappedRoleID=M2.MappedRoleID WHERE M1.ForumID<>@ForumID AND M2.ForumID=@ForumID
	)
	
	DELETE [bx_Moderators] WHERE ForumID=@ForumID
	*/