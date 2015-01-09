--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_Polemizes_ThreadID';

ALTER TABLE [bx_Polemizes] ADD 
   CONSTRAINT [FK_bx_Polemizes_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

