
EXEC bx_Drop 'FK_bx_Polls_ThreadID';

ALTER TABLE [bx_Polls] ADD 
   CONSTRAINT [FK_bx_Polls_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

