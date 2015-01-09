CREATE PROCEDURE [bx_UpdateForumThreadCatalogsData]
	@ForumID INT,
	@ThreadCatalogID INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [bx_ThreadCatalogsInForums] SET TotalThreads=(
		SELECT COUNT(1) FROM bx_Threads WITH(NOLOCK) WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID AND SortOrder<4000000000000000
		) WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID
END