CREATE PROCEDURE [bx_GetPostMarks]
	@PostID int
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT * From [bx_PostMarks] WITH (NOLOCK) Where PostID=@PostID order by CreateDate DESC

END
