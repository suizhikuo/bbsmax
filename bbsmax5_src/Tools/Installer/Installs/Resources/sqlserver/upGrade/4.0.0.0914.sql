
DECLARE @NewUserID int,@NewUsername nvarchar(50),@TotalUser int;
 
 SELECT TOP 1 @NewUserID=UserID,@NewUsername=Username FROM [bx_Users] WHERE [IsActive] = 1 AND [UserID]<>0 ORDER BY [UserID] DESC;
 
 SELECT @TotalUser=Count(*) FROM [bx_Users] WHERE [IsActive] = 1 AND [UserID]<>0;

 IF @NewUserID IS NOT NULL BEGIN
     IF EXISTS(SELECT * FROM [bx_Vars])
         UPDATE [bx_Vars] SET  NewUserID = @NewUserID, NewUsername = @NewUsername, TotalUsers = @TotalUser WHERE [ID]=(SELECT TOP 1 ID FROM [bx_Vars]);
     ELSE
         INSERT [bx_Vars] (NewUserID,NewUsername,TotalUsers)VALUES(@NewUserID,@NewUsername,@TotalUser);

END



UPDATE bx_Users SET UsedDiskSpaceSize=TotalFileSize FROM (SELECT SUM(FileSize) AS TotalFileSize,UserID FROM bx_DiskFiles GROUP BY UserID) AS D WHERE bx_Users.UserID = D.UserID;