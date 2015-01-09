
EXEC bx_Drop 'FK_bx_PollItems_ThreadID';

ALTER TABLE [bx_PollItems] ADD 
   CONSTRAINT [FK_bx_PollItems_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Polls]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

