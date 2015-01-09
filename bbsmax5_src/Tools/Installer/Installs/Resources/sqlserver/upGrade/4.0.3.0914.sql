

---UPDATE bx_Users SET UsedDiskSpaceSize=TotalFileSize FROM (SELECT SUM(FileSize) AS TotalFileSize,UserID FROM bx_DiskFiles GROUP BY UserID) AS D WHERE bx_Users.UserID = D.UserID;