--相片 相册关系表外键关系
--相片 文件关系表外键关系
EXEC bx_Drop 'FK_bx_Photos_AlbumID';
EXEC bx_Drop 'FK_bx_Photos_FileID';

ALTER TABLE [bx_Photos] ADD 
     
    CONSTRAINT [FK_bx_Photos_AlbumID]             FOREIGN KEY ([AlbumID])    REFERENCES [bx_Albums]       ([AlbumID])    ON UPDATE CASCADE  ON DELETE CASCADE
    --,CONSTRAINT [FK_bx_Photos_FileID]              FOREIGN KEY ([FileID])     REFERENCES [bx_Files]        ([FileID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO
