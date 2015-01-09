--访问日志

CREATE PROCEDURE [bx_VisitBlogArticle]
      @ArticleID            int
     ,@UserID               int
AS
BEGIN

	SET NOCOUNT ON; 

	BEGIN TRANSACTION
	
	--是否不是日志作者本人访问日志,如果不是,则要更新访问者记录
	UPDATE [bx_BlogArticles] SET [TotalViews] = [TotalViews] + 1 WHERE [ArticleID] = @ArticleID AND [UserID] <> @UserID;
	IF @@ROWCOUNT > 0 BEGIN
	
	    IF EXISTS (SELECT [UserID] FROM [bx_BlogArticleVisitors] WHERE [BlogArticleID] = @ArticleID AND [UserID] = @UserID)
			UPDATE [bx_BlogArticleVisitors] SET [ViewDate] = GETDATE() WHERE [BlogArticleID] = @ArticleID AND [UserID] = @UserID;
		ELSE BEGIN
			INSERT INTO [bx_BlogArticleVisitors] ([BlogArticleID], [UserID]) VALUES (@ArticleID, @UserID); --写入访问者本次的访问记录
			DELETE FROM [bx_BlogArticleVisitors] WHERE [BlogArticleID] = @ArticleID AND [UserID] NOT IN (SELECT TOP 10 [UserID] FROM [bx_BlogArticleVisitors] WHERE [BlogArticleID] = @ArticleID ORDER BY [ViewDate] DESC); --清除该日志不在前10条的访问记录
		END

	END
	
	IF(@@error<>0)
		GOTO Cleanup;

	GOTO CommitTrans;
	
	
CommitTrans:
	BEGIN
		COMMIT TRANSACTION
		RETURN (0);
	END
                    
Cleanup:
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

END