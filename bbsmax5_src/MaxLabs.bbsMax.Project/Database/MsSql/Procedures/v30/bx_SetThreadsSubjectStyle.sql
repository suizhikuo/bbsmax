-- =============================================
-- Author:		SEK
-- Create date: <2006/12/30>
-- Description:	<标题添加样式>
-- =============================================
CREATE PROCEDURE [bx_SetThreadsSubjectStyle]
	@ForumID int,
	@ThreadIdentities varchar(8000),
	@Style nvarchar(300)
AS
BEGIN
	SET NOCOUNT ON
	EXEC ('UPDATE [bx_Threads] SET SubjectStyle='''+@Style+''' WHERE [ThreadID] IN (' + @ThreadIdentities + ') AND ForumID = ' + @ForumID) 
	IF @@ROWCOUNT > 0
		RETURN (0)
	ELSE
		RETURN (1)
END


