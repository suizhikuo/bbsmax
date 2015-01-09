-- =============================================
-- Author:		zzbird
-- Create date: <2007/4/29>
-- Description:	<提升主题>
-- =============================================
CREATE PROCEDURE [bx_SetThreadsUp]
	@ThreadIdentities varchar(8000),
	@ForumID int
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ThreadID int;
	--DECLARE @SortOrder bigint
	DECLARE @i int
	DECLARE @j int
	
	DECLARE @SortOrder BIGINT,@PostDate datetime
	
	SET @PostDate = getdate();
	
	
	SET @ThreadIdentities = @ThreadIdentities + N','
	SELECT @i = CHARINDEX(',', @ThreadIdentities)
	
	SET @j = 0
	
	WHILE ( @i > 1 ) BEGIN
			SELECT @ThreadID = SUBSTRING(@ThreadIdentities,0, @i)	
			
			SELECT @SortOrder = SortOrder FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID

			IF @SortOrder < 2000000000000000 -- 原来是普通主题
				EXEC [bx_GetSortOrder] 1, @j, @PostDate, @SortOrder OUTPUT;
				--SELECT @SortOrder = [dbo].bx_GetSortOrder(1,@j, getdate());
			ELSE IF (@SortOrder < 3000000000000000) -- 原来是置顶主题
				EXEC [bx_GetSortOrder] 2, @j, @PostDate, @SortOrder OUTPUT;
				--SELECT @SortOrder = [dbo].bx_GetSortOrder(2,@j, getdate());-- + 400000000000000;
			ELSE IF(@SortOrder < 4000000000000000)  -- 原来是总普通主题
				EXEC [bx_GetSortOrder] 3, @j, @PostDate, @SortOrder OUTPUT;
				--SELECT @SortOrder = [dbo].bx_GetSortOrder(3,@j, getdate()) --+ 700000000000000;


			UPDATE bx_Threads SET SortOrder = @SortOrder WHERE ForumID=@ForumID AND ThreadID=@ThreadID
			
			SELECT @ThreadIdentities = SUBSTRING(@ThreadIdentities, @i + 1, LEN(@ThreadIdentities) - @i)
			SELECT @i = CHARINDEX(',',@ThreadIdentities)
			SELECT @j = @j + 1
	END
	RETURN
END


