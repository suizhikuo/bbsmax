CREATE VIEW bx_FriendsHasImpressions
AS
	SELECT * FROM bx_Friends A where FriendUserID IN (
		SELECT UserID FROM bx_UserVars B WHERE B.LastImpressionDate > '1980-1-1'
	)
