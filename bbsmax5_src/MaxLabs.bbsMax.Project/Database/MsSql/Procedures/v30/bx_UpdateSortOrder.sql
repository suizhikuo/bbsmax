CREATE PROCEDURE [bx_UpdateSortOrder]
	@PostType tinyint,--1正常,2置顶,3总置顶,4待审核,5回收站 
	@OldSortOrder bigint,
	@SortOrder bigint output
AS
BEGIN
	SET NOCOUNT ON;

	SET @SortOrder = {# exp_GetSortOrder @OldSortOrder,@PostType #};

END