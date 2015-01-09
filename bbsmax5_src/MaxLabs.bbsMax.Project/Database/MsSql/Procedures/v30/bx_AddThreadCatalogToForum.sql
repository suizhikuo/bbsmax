-- =============================================
-- Author:		<Author,,帅帅>
-- Create date: <Create 07/04/19,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_AddThreadCatalogToForum]
	@ForumID int,
	@ThreadCatalogIDs varchar(8000),
	@SortOrders varchar(8000)
AS
BEGIN

	SET NOCOUNT ON;
	
	BEGIN TRANSACTION
	IF EXISTS (SELECT * FROM bx_ThreadCatalogsInForums WITH (NOLOCK) WHERE ForumID = @ForumID)
		---删除forumID下的主题分类
		delete bx_ThreadCatalogsInForums where ForumID=@ForumID
		IF(@@error<>0)
		BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
		END
		DECLARE @ThreadCatalogID int,@i int,@SortOrder int,@Condition varchar(100),@j int
	
		SET @SortOrders=@SortOrders+N','
		SELECT @j=CHARINDEX(',',@SortOrders)
		
		SET @ThreadCatalogIDs=@ThreadCatalogIDs+N','
		SET @Condition='ForumID='+str(@ForumID)
		SELECT @i=CHARINDEX(',',@ThreadCatalogIDs)
		
		WHILE(@i>1)
		BEGIN
			SELECT @ThreadCatalogID=SUBSTRING(@ThreadCatalogIDs,0, @i)
			SELECT @SortOrder=SUBSTRING(@SortOrders,0, @j)
			--EXECUTE bx_Common_GetSortOrder 'bx_ThreadCatalogsInForums', @Condition,@SortOrder output
			INSERT INTO bx_ThreadCatalogsInForums (ForumID, ThreadCatalogID,SortOrder)
				VALUES (@ForumID, @ThreadCatalogID,@SortOrder);
			
		
			SELECT @ThreadCatalogIDs=SUBSTRING(@ThreadCatalogIDs,@i+1,LEN(@ThreadCatalogIDs)-@i)
			SELECT @i=CHARINDEX(',',@ThreadCatalogIDs)
			
			SELECT @SortOrders=SUBSTRING(@SortOrders,@j+1,LEN(@SortOrders)-@j)
			SELECT @j=CHARINDEX(',',@SortOrders)
		END
		IF(@@error<>0)
				BEGIN
					ROLLBACK TRANSACTION
					RETURN (-1)
				END
		
		COMMIT TRANSACTION
		RETURN (0);
END


