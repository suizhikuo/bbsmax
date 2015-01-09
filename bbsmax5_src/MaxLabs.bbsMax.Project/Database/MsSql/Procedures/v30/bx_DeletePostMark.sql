CREATE PROCEDURE [bx_DeletePostMark]
	@PostMarkIDs varchar(8000)
AS
	SET NOCOUNT ON;
	EXEC ('DELETE [bx_PostMarks] WHERE [PostMarkID] IN (' + @PostMarkIDs + ') ') 
	IF @@ROWCOUNT > 0
		RETURN (0)
	ELSE
		RETURN (1)


