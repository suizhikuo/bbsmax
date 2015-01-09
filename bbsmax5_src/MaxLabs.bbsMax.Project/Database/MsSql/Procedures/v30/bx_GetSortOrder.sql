CREATE PROCEDURE [bx_GetSortOrder]
	@PostType tinyint,--1正常,2置顶,3总置顶,5待审核,4回收站
	@PostRandNumber bigint,
	@PostDate datetime,
	@IsThread bit,
	@SortOrder bigint output
AS
BEGIN
	SET NOCOUNT ON;
	IF @PostRandNumber > 0
		SELECT @SortOrder = (CAST(DATEDIFF(second, '1970-01-01 00:00:00', @PostDate) AS bigint) * 100000) + (DATEPART(millisecond, @PostDate) * 100) + @PostRandNumber % 100
	ELSE
		SELECT @SortOrder = (CAST(DATEDIFF(second, '1970-01-01 00:00:00', @PostDate) AS bigint) * 100000) + (DATEPART(millisecond, @PostDate) * 100)
	
	IF @IsThread = 1
		SET @SortOrder = @SortOrder;
	ELSE
		SET @SortOrder = @SortOrder+@PostType*1000000000000000;

END