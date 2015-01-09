
EXEC bx_Drop 'bx_Shares_AfterInsert';

GO


CREATE TRIGGER [bx_Shares_AfterInsert]
	ON [bx_UserShares]
	AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(UserID int, ShareCount int,CollectionCount int);

	INSERT INTO @tempTable 
		SELECT DISTINCT UserID
			,(SELECT COUNT(*) FROM [bx_UserShares] WITH (NOLOCK) WHERE [PrivacyType] = 2 AND [UserID] = T.UserID)
			,(SELECT COUNT(*) FROM [bx_UserShares] WITH (NOLOCK) WHERE [PrivacyType] < 2 AND [UserID] = T.UserID)
		FROM [INSERTED] T;
	
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
