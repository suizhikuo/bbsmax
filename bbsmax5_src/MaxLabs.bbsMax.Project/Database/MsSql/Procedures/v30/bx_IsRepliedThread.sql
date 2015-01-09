CREATE PROCEDURE [bx_IsRepliedThread]
	@ThreadID int,
	@UserID int
AS
BEGIN

	SET NOCOUNT ON;
	IF EXISTS (SELECT 1 FROM bx_Posts WHERE UserID = @UserID AND ThreadID = @ThreadID AND SortOrder < 5000000000000000)
		RETURN (0);
	ELSE
		RETURN (-1);

END


