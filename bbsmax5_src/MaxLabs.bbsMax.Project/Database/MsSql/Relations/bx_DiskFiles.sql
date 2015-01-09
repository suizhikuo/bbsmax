--网络硬盘文件表外键关系
--EXEC bx_Drop 'FK_bx_DiskFiles_FileID';
EXEC bx_Drop 'FK_bx_DiskFiles_DirectoryID';
--EXEC bx_Drop 'FK_bx_DiskFiles_UserID';

ALTER TABLE [bx_DiskFiles] ADD 
     --CONSTRAINT [FK_bx_DiskFiles_FileID]        FOREIGN KEY ([FileID])         REFERENCES [bx_Files]           ([ID])      ON UPDATE CASCADE    ON DELETE CASCADE,
    CONSTRAINT [FK_bx_DiskFiles_DirectoryID]   FOREIGN KEY ([DirectoryID])    REFERENCES [bx_DiskDirectories] ([DirectoryID])      ON UPDATE CASCADE    ON DELETE CASCADE
GO
