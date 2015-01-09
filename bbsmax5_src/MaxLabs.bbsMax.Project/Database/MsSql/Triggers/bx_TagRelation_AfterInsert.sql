--标签触发器
EXEC bx_Drop 'bx_TagRelation_AfterInsert';

GO

CREATE TRIGGER [bx_TagRelation_AfterInsert]
	ON [bx_TagRelation]
	AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @C        int;
	DECLARE @TagID    nvarchar(50);
	DECLARE @Type     tinyint;
	
	SET @TagID = (SELECT TOP 1 [TagID] FROM [INSERTED]);
	SET @Type = (SELECT TOP 1 [Type] FROM [INSERTED]);
	
	IF @Type = 1 BEGIN
		SET @C = (SELECT COUNT(*) FROM [bx_TagRelation] WITH (NOLOCK) WHERE [TagID] = @TagID AND [TargetID] IN (SELECT [ArticleID] FROM [bx_BlogArticles] WHERE [PrivacyType] IN (0,3)));
	  END
	ELSE BEGIN
		SET @C = (SELECT COUNT(*) FROM [bx_TagRelation] WITH (NOLOCK) WHERE [TagID] = @TagID);
	  END
	
	UPDATE [bx_Tags] SET [TotalElements] = @C WHERE [ID] = @TagID;

END