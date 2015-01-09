
EXEC bx_Drop 'FK_bx_ThreadExchanges_ThreadID';

ALTER TABLE [bx_ThreadExchanges] ADD 
   CONSTRAINT [FK_bx_ThreadExchanges_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

