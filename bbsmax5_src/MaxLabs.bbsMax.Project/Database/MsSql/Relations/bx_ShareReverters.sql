EXEC bx_Drop 'FK_bx_ShareReverters_ShareID';

ALTER TABLE [bx_ShareReverters] ADD 
CONSTRAINT [FK_bx_ShareReverters_ShareID] FOREIGN KEY ([ShareID]) REFERENCES [bx_Shares] ([ShareID]) ON UPDATE CASCADE ON DELETE CASCADE

GO

ALTER TABLE [bx_UserShareReverters] ADD 
CONSTRAINT [FK_bx_UserShareReverters_UserShareID] FOREIGN KEY ([UserShareID]) REFERENCES [bx_UserShares] ([UserShareID]) ON UPDATE CASCADE ON DELETE CASCADE

GO