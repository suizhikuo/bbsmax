--用户用户分享关系表外键关系
EXEC bx_Drop 'FK_bx_ThreadReverters_ThreadID';

ALTER TABLE [bx_ThreadReverters] ADD 
     CONSTRAINT [FK_bx_ThreadReverters_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO

