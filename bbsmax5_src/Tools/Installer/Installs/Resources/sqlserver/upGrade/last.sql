
-- 插入系统需要的一个默认用户 --
IF NOT EXISTS (SELECT * FROM bx_Users WHERE UserID = 0) BEGIN

	SET IDENTITY_INSERT [bx_Users] ON;

	INSERT INTO [bx_Users] ([UserID], [Username]) VALUES(0, '{#bxguest#}');

	SET IDENTITY_INSERT [bx_Users] OFF;

END

ELSE IF NOT EXISTS (SELECT * FROM bx_Users WHERE UserID = 0 AND Username = '{#bxguest#}')
	UPDATE bx_Users SET Username = '{#bxguest#}' WHERE UserID = 0;

GO

-- 插入系统需要的两个默认好友分组 --
IF NOT EXISTS (SELECT * FROM [bx_FriendGroups] WHERE GroupID = -1) BEGIN

	SET IDENTITY_INSERT [bx_FriendGroups] ON;

	INSERT INTO [bx_FriendGroups] ([GroupID], [UserID], [GroupName]) VALUES (-1, 0, 'blacklist');

	SET IDENTITY_INSERT [bx_FriendGroups] OFF;
	
END

IF NOT EXISTS (SELECT * FROM [bx_FriendGroups] WHERE GroupID = 0) BEGIN

	SET IDENTITY_INSERT [bx_FriendGroups] ON;

	INSERT INTO [bx_FriendGroups] ([GroupID], [UserID], [GroupName]) VALUES (0, 0, 'none');

	SET IDENTITY_INSERT [bx_FriendGroups] OFF;
	
END

GO

-- 插入系统需要的一个默认日志分组 --
IF NOT EXISTS (SELECT * FROM [bx_BlogCategories] WHERE CategoryID = 0) BEGIN

	SET IDENTITY_INSERT [bx_BlogCategories] ON;

	INSERT INTO [bx_BlogCategories] ([CategoryID], [UserID], [Name]) VALUES (0, 0, 'none');

	SET IDENTITY_INSERT [bx_BlogCategories] OFF;

END

GO
 
DECLARE @NewUserID int, @NewUsername nvarchar(50), @TotalUser int;
 
SELECT TOP 1 @NewUserID = UserID, @NewUsername = Username FROM [bx_Users] WHERE [IsActive] = 1 AND [UserID] <> 0 ORDER BY [UserID] DESC;
 
SELECT @TotalUser = COUNT(*) FROM [bx_Users] WHERE [IsActive] = 1 AND [UserID] <> 0;

IF @NewUserID IS NOT NULL BEGIN
     IF EXISTS(SELECT * FROM [bx_Vars])
         UPDATE [bx_Vars] SET  NewUserID = @NewUserID, NewUsername = @NewUsername, TotalUsers = @TotalUser WHERE [ID] = (SELECT TOP 1 ID FROM [bx_Vars]);
     ELSE
         INSERT [bx_Vars] (NewUserID, NewUsername, TotalUsers) VALUES (@NewUserID, @NewUsername, @TotalUser);

END

GO

DELETE [bx_Moderators] WHERE ForumID < 0;

GO

UPDATE [bx_Moderators] SET ModeratorType = 3 WHERE ForumID IN (SELECT ForumID FROM [bx_Forums] WHERE ParentID = 0);

GO

UPDATE bx_UserVars SET UsedDiskSpaceSize = TotalFileSize FROM (SELECT SUM(FileSize) AS TotalFileSize, UserID FROM bx_DiskFiles GROUP BY UserID) AS D WHERE bx_UserVars.UserID = D.UserID;

GO

DELETE bx_TempUploadFiles;