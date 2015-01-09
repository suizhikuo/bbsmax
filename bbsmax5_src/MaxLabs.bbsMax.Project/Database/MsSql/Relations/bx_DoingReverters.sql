EXEC bx_Drop 'FK_bx_DoingReverters_DoingID';

ALTER TABLE [bx_DoingReverters] ADD 
CONSTRAINT [FK_bx_DoingReverters_DoingID] FOREIGN KEY ([DoingID]) REFERENCES [bx_Doings] ([DoingID]) ON UPDATE CASCADE ON DELETE CASCADE

GO