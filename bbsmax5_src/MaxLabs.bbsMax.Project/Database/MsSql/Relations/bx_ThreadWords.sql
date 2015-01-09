--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_ThreadWords_ThreadID';

ALTER TABLE [bx_ThreadWords] ADD 
   CONSTRAINT [FK_bx_ThreadWords_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

