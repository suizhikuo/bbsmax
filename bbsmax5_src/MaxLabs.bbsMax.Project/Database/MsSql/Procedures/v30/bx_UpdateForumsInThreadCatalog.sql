-- =============================================
-- Author:		<Author,SEK>
-- Create date: <Create 07/05/18,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_UpdateForumsInThreadCatalog]
	@ThreadCatalogID int,
	@ForumIDs varchar(8000)
AS
BEGIN
	SET NOCOUNT ON;
	IF(@ForumIDs='')
	BEGIN
		DELETE [bx_ThreadCatalogsInForums] WHERE ThreadCatalogID=@ThreadCatalogID
		RETURN (0)
	END
		
	BEGIN TRANSACTION
	
	EXECUTE('DELETE [bx_ThreadCatalogsInForums] WHERE ForumID NOT IN('+@ForumIDs+') AND ThreadCatalogID='+@ThreadCatalogID)
	
	IF(@@error<>0)
		BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
		END
		
	DECLARE @ForumID int,@i int,@SortOrder int,@Condition varchar(100)
	
		SET @ForumIDs=@ForumIDs+N','
		SELECT @i=CHARINDEX(',',@ForumIDs)
		
		WHILE(@i>1)
		BEGIN
			SELECT @ForumID=SUBSTRING(@ForumIDs,0, @i)
			
			IF NOT EXISTS (SELECT * FROM bx_ThreadCatalogsInForums WITH (NOLOCK) WHERE ForumID=@ForumID AND ThreadCatalogID = @ThreadCatalogID)
			BEGIN
				SET @Condition='ForumID='+str(@ForumID)
				EXECUTE bx_Common_GetSortOrder 'bx_ThreadCatalogsInForums', @Condition,@SortOrder output
				INSERT INTO bx_ThreadCatalogsInForums (ForumID, ThreadCatalogID,SortOrder)
					VALUES (@ForumID, @ThreadCatalogID,@SortOrder)
				IF(@@error<>0)
				BEGIN
					ROLLBACK TRANSACTION
					RETURN (-1)
				END
			END
		
			SELECT @ForumIDs=SUBSTRING(@ForumIDs,@i+1,LEN(@ForumIDs)-@i)
			SELECT @i=CHARINDEX(',',@ForumIDs)
		END
		
		COMMIT TRANSACTION
		RETURN (0);
END

