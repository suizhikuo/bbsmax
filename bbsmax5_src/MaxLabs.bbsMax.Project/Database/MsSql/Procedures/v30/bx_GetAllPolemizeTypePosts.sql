CREATE PROCEDURE [bx_GetAllPolemizeTypePosts]
	@ThreadID int, 
	@AgreePageIndex int,
	@AgainstPageIndex int,
	@NeutralPageIndex int,
	@PageSize int
AS
	SET NOCOUNT ON;
	
	SELECT * FROM bx_Polemizes WHERE ThreadID=@ThreadID
	
	EXEC bx_GetPolemizePosts @ThreadID,@AgreePageIndex,@PageSize,2
	
	EXEC bx_GetPolemizePosts @ThreadID,@AgainstPageIndex,@PageSize,3
	
	EXEC bx_GetPolemizePosts @ThreadID,@NeutralPageIndex,@PageSize,4