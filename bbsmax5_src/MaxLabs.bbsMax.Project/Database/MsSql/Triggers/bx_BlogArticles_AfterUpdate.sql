--发表新日志的触发器
EXEC bx_Drop 'bx_BlogArticles_AfterInsert';

GO


CREATE TRIGGER [bx_BlogArticles_AfterUpdate]
	ON [bx_BlogArticles]
	AFTER UPDATE
AS
BEGIN
	
	SET NOCOUNT ON;

	--如果CategoryID字段被更改，需要重新统计受影响的日志分类的TotalArticles值
	IF UPDATE([CategoryID]) BEGIN

		DECLARE @tempTable table(CategoryID int, TotalArticles int);

		INSERT INTO @tempTable 
			SELECT DISTINCT CategoryID,
				(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.CategoryID = T.CategoryID), 0))
			FROM [INSERTED] T;

		INSERT INTO @tempTable 
			SELECT DISTINCT CategoryID,
				(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.CategoryID = T.CategoryID), 0))
			FROM [DELETED] T
			WHERE CategoryID NOT IN (SELECT CategoryID FROM @tempTable);
		
		UPDATE [bx_BlogCategories]
			SET
				bx_BlogCategories.TotalArticles = T.TotalArticles
			FROM @tempTable T
			WHERE
				T.CategoryID = bx_BlogCategories.CategoryID;

	END

	--如果UserID字段被更改，需要重新统计受影响的用户的TotalBlogArticles值
	IF UPDATE([UserID]) BEGIN
		
		DECLARE @tempTable2 table(UserID int, TotalBlogArticles int);

		INSERT INTO @tempTable2 
			SELECT DISTINCT UserID,
				(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
			FROM [INSERTED] T;

		INSERT INTO @tempTable2 
			SELECT DISTINCT UserID,
				(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
			FROM [DELETED] T
			WHERE UserID NOT IN (SELECT UserID FROM @tempTable2);

		UPDATE [bx_Users]
			SET
				bx_Users.TotalBlogArticles = T.TotalBlogArticles
			FROM @tempTable2 T
			WHERE
				T.UserID = [bx_Users].UserID;
		
		--发出重新填充UserInfo的XCMD命令
		SELECT 'ResetUser' AS XCMD, UserID, TotalBlogArticles FROM @tempTable2;

	END
	
END
