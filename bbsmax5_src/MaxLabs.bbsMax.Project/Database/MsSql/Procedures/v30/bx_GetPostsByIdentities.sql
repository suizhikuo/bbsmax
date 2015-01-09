CREATE PROCEDURE [bx_GetPostsByIdentities]
	@PostIdentities varchar(8000)
AS
BEGIN
	SET NOCOUNT ON;
	EXEC ('SELECT * FROM [bx_Posts] WITH (NOLOCK) WHERE PostID in (' + @PostIdentities +  ')');
END


