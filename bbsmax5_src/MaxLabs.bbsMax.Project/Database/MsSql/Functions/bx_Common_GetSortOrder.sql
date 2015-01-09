-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/14>
-- Description:	<获取SortOrder,（当添加数据时，表中有SortOrder字段时使用）>
-- =============================================
CREATE PROCEDURE [bx_Common_GetSortOrder]
	@TableName nvarchar(256),
	@Condition nvarchar(400),--查询条件
	@SortOrder int output
AS
	SET NOCOUNT ON
	DECLARE @SQLString nvarchar(4000),@Count int
	if(@Condition<>'')
		SET @Condition=' WHERE '+@Condition
		

	SET @SQLString=N'SELECT @SortOrder=ISNULL(Max(SortOrder),0) FROM '+@TableName+@Condition
	EXECUTE sp_executesql @SQLString,N'@SortOrder int output',@SortOrder output
	SET @SortOrder=@SortOrder+1


