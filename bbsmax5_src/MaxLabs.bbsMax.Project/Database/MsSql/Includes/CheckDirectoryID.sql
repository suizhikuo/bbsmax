    --如果目录ID不是0，则表示一个已经存在的目录，那么需要进一步检查
    IF @DirectoryID > 0 BEGIN

        --这个ID的目录并不存在，或者所有者的身份和传入的@UserID不一致，则把DirectoryID置0，当作根目录处理
        IF NOT EXISTS (SELECT * FROM bx_DiskDirectories WHERE DirectoryID = @DirectoryID AND UserID = @UserID)
            SET @DirectoryID = 0;

    END

    --不清楚目录ID，用0表示根目录，所以要查处真实的根目录ID。如果根目录不存在则需要建立一个根目录（ParentID为0表示根目录）
    IF @DirectoryID = 0 BEGIN

        SELECT @DirectoryID = DirectoryID FROM bx_DiskDirectories WHERE UserID = @UserID AND ParentID = 0;
        --如果根目录并不存在
        IF @@ROWCOUNT = 0 BEGIN
            INSERT INTO bx_DiskDirectories([ParentID], [Name], UserID) VALUES (0, '\', @UserID);
            SET @DirectoryID = @@IDENTITY;
        END

    END