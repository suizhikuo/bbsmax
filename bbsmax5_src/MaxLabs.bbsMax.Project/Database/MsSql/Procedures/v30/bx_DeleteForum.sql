-- =============================================
-- Author:		<sek>
-- Create date: <2007/1/10>
-- Description:	<删除>
-- =============================================
CREATE PROCEDURE [bx_DeleteForum] 
	@ForumID int
AS
	SET NOCOUNT ON
	
	BEGIN TRANSACTION
	DECLARE @SortOrder int,@ParentID int,@ErrorNum int
	SET @ErrorNum=0
	
	DELETE [bx_Threads] WHERE ForumID=@ForumID 
	IF @@error<>0
		SET @ErrorNum=@ErrorNum+1
		
	--TODO DELETE [bx_Threads] WHERE ForumID=@ForumID  删除屏蔽用户
	--IF @@error<>0
	--	SET @ErrorNum=@ErrorNum+1
	
	SELECT @ParentID=ParentID FROM [bx_Forums] WITH(NOLOCK) WHERE ForumID=@ForumID 
	SELECT @SortOrder=MAX(SortOrder)+1 FROM [bx_Forums] WITH(NOLOCK) WHERE ParentID=@ParentID  
	IF @SortOrder IS NULL
			SET @SortOrder=0
			
	UPDATE [bx_Forums] SET ParentID=@ParentID,SortOrder=SortOrder+@SortOrder WHERE ParentID=@ForumID
	IF @@error<>0
		SET @ErrorNum=@ErrorNum+1
	
	DELETE [bx_Forums] WHERE ForumID=@ForumID 
	IF @@error<>0
		SET @ErrorNum=@ErrorNum+1
		
	IF @ErrorNum=0
		BEGIN
			COMMIT TRANSACTION
			--EXECUTE bx_UpdateForumData @ParentID
			RETURN (0)
		END
		ELSE
		BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
		END


