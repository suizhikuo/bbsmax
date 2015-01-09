--发表新日志的触发器
EXEC bx_Drop 'bx_BlogArticles_AfterDelete';

GO


CREATE TRIGGER [bx_BlogArticles_AfterDelete]
	ON [bx_BlogArticles]
	AFTER DELETE
AS
BEGIN
	
	SET NOCOUNT ON;
	
	DELETE bx_Denouncings WHERE Type=3 AND TargetID IN (SELECT ArticleID FROM [DELETED]);
	
	DECLARE @tempTable table(CategoryID int, TotalArticles int);

	INSERT INTO @tempTable 
		SELECT DISTINCT CategoryID,
				(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.CategoryID = T.CategoryID), 0))
		FROM [DELETED] T;
	
	UPDATE [bx_BlogCategories]
		SET
			bx_BlogCategories.TotalArticles = T.TotalArticles
		FROM @tempTable T
		WHERE
			T.CategoryID = bx_BlogCategories.CategoryID;


	DECLARE @tempTable2 table(UserID int, TotalBlogArticles int);

	INSERT INTO @tempTable2 
		SELECT DISTINCT UserID,
			(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
		FROM [DELETED] T;

	UPDATE [bx_Users]
		SET
			bx_Users.TotalBlogArticles = T.TotalBlogArticles
		FROM @tempTable2 T
		WHERE
			T.UserID = [bx_Users].UserID;
	
	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, TotalBlogArticles FROM @tempTable2;
	
END
