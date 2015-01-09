--用户用户分享关系表外键关系
EXEC bx_Drop 'FK_bx_PostReverters_PostID';

ALTER TABLE [bx_PostReverters] ADD 
     CONSTRAINT [FK_bx_PostReverters_PostID]        FOREIGN KEY ([PostID])         REFERENCES [bx_Posts]         ([PostID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO

