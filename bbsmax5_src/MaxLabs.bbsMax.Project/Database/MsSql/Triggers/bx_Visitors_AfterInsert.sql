----最近访客触发器
--CREATE TRIGGER [bx_Visitors_AfterInsert]
	--ON [bx_Visitors]
	--AFTER INSERT
--AS
--BEGIN

	--SET NOCOUNT ON;

	--DECLARE @tempTable table(UserID int, VisitorCount int);

	--INSERT INTO @tempTable 
		--SELECT UserID ,COUNT(*)
		--FROM [INSERTED] T
		--GROUP BY UserID;

	--UPDATE [bx_UserInfos]
		--SET
			--TotalVisitors = TotalVisitors + VisitorCount
		--FROM @tempTable T
		--WHERE
			--T.UserID = [bx_UserInfos].UserID;

--END