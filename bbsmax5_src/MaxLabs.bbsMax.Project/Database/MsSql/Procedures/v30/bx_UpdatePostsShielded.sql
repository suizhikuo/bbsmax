CREATE PROCEDURE [bx_UpdatePostsShielded]
	@PostIDs varchar(8000),
	@IsShielded bit
AS
	SET NOCOUNT ON
	EXEC('UPDATE [bx_Posts] SET IsShielded='+@IsShielded+' WHERE PostID IN('+@PostIDs+') AND IsShielded<>'+@IsShielded)