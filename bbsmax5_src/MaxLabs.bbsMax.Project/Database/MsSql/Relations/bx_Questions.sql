
EXEC bx_Drop 'FK_bx_Questions_ThreadID';

ALTER TABLE [bx_Questions] ADD 
   CONSTRAINT [FK_bx_Questions_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

