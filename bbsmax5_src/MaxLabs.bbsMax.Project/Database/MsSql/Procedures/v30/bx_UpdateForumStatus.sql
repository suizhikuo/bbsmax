-- =============================================
-- Author:		<sek>
-- Create date: <2007/1/10>
-- Description:	<修改版块状态>
-- =============================================
CREATE PROCEDURE [bx_UpdateForumStatus] 
	@ForumIdentities varchar(8000),
	@ForumStatus tinyint
AS
	SET NOCOUNT ON

	BEGIN TRANSACTION
	
	IF @ForumStatus<2 BEGIN
		EXEC ('Alter PROCEDURE bx_GetDisabledTriggerForumIDs
				@ForumIDs nvarchar(64) output
			AS
			BEGIN
				SET NOCOUNT ON;
				set @ForumIDs='''';
			END')
	END
	ELSE BEGIN
--		DECLARE @SQLString nvarchar(4000),@Count INT
--		SET @SQLString=N'SELECT @Count=count(*) FROM [bx_Forums] WHERE ForumStatus>1 AND ForumID Not in('+@ForumIdentities+')'
--		EXECUTE sp_executesql @SQLString,N'@Count int output',@Count output
--		IF @Count>0 BEGIN--存在不正常的版面
--			COMMIT TRANSACTION
--			RETURN 20
--		END

		EXEC ('Alter PROCEDURE bx_GetDisabledTriggerForumIDs
				@ForumIDs nvarchar(64) output
			AS
			BEGIN
				SET NOCOUNT ON;
				set @ForumIDs='''+@ForumIdentities+''';
			END')
	END

	IF @@ERROR<>0 BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

	EXEC ('Update [bx_Forums] SET ForumStatus='+@ForumStatus+' WHERE [ForumID] IN (' + @ForumIdentities + ') ') 
	
	IF @@ERROR<>0 BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

COMMIT TRANSACTION
RETURN 0;


