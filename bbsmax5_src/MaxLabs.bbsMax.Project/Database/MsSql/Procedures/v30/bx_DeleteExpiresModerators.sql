-- =============================================
-- Author:		<SEK>
-- Create date: <2007/8/30> 
-- Description:	删除过期版主  并返回需要移除的用户角色
-- =============================================
CREATE PROCEDURE [bx_DeleteExpiresModerators]
AS
	SET NOCOUNT ON;
	DECLARE @TempTable TABLE(TempUserID int,TempMappedRoleID uniqueIdentifier,RemoveRole bit)
	
	INSERT INTO @TempTable 
		SELECT UserID,MappedRoleID,1 FROM bx_Moderators WITH(NOLOCK) WHERE ExpiresDate<=GETDATE()
		
	DELETE bx_Moderators WHERE ExpiresDate<=GETDATE()
	
	UPDATE @TempTable SET RemoveRole = 0 FROM bx_Moderators T WHERE T.UserID=TempUserID AND T.MappedRoleID=TempMappedRoleID
	
	SELECT TempUserID,TempMappedRoleID FROM @TempTable WHERE RemoveRole=1;