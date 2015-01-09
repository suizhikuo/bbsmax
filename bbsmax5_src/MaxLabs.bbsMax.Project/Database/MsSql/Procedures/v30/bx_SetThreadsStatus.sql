-- =============================================
-- Author:		<zzbird>
-- Create date: <2009/10/29>
-- Description:	<将主题修改为@ThreadStatus指定的状态>
-- =============================================
CREATE PROCEDURE [bx_SetThreadsStatus]
	@ForumID int,
	@ThreadIdentities varchar(8000),
	@ThreadStatus tinyint --1正常,2置顶,3总置顶,4待审核,5回收站
AS
BEGIN
	SET NOCOUNT ON;

	IF @ForumID = 0 BEGIN
		EXEC ('UPDATE bx_Threads SET SortOrder = {# exp_GetSortOrder SortOrder,' + @ThreadStatus + ' #} WHERE ThreadID IN (' + @ThreadIdentities + ')')
		 
	END
	ELSE BEGIN
		EXEC ('UPDATE bx_Threads SET SortOrder = {# exp_GetSortOrder SortOrder,' + @ThreadStatus + ' #} WHERE ForumID = ' + @ForumID + ' AND ThreadID IN (' + @ThreadIdentities + ')')
	END
END
