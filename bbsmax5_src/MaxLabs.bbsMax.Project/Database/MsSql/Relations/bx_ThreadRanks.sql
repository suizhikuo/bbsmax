
EXEC bx_Drop 'FK_bx_ThreadRanks_ThreadID';

ALTER TABLE [bx_ThreadRanks] ADD 
   CONSTRAINT [FK_bx_ThreadRanks_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

