EXEC bx_Drop 'FK_bx_BannedUsers_UserID';

ALTER TABLE [bx_BannedUsers] ADD 
CONSTRAINT [FK_bx_BannedUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [bx_Users] ([UserID]) ON DELETE CASCADE

GO