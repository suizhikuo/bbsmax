
EXEC bx_Drop 'bx_Shares_AfterDelete';

GO


CREATE TRIGGER [bx_Shares_AfterDelete]
	ON [bx_UserShares]
	AFTER DELETE
AS
BEGIN
	
	SET NOCOUNT ON;
			
	DELETE bx_Denouncings WHERE Type=5 AND TargetID IN (SELECT ShareID FROM [DELETED]);
	
	--删除评论
	DELETE [bx_Comments] WHERE [Type] = 5 AND [TargetID] IN (SELECT [ShareID] FROM [DELETED]);
	
	DECLARE @tempTable table(UserID int, ShareCount int,CollectionCount int);

	INSERT INTO @tempTable
		SELECT DISTINCT UserID
			,(SELECT COUNT(*) FROM [bx_UserShares] WITH (NOLOCK) WHERE [PrivacyType] = 2 AND [UserID] = D.UserID)
			,(SELECT COUNT(*) FROM [bx_UserShares] WITH (NOLOCK) WHERE [PrivacyType] < 2 AND [UserID] = D.UserID)
		FROM [DELETED] D
	
	UPDATE [bx_Users]
		SET
			  TotalShares = T.ShareCount
			, TotalCollections = T.CollectionCount
		FROM @tempTable T
		WHERE
			T.UserID = [bx_Users].UserID;
	
	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, ShareCount AS TotalShares ,CollectionCount AS TotalCollections FROM @tempTable;
	
END
