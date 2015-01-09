EXEC bx_Drop 'FK_bx_PointLogs_UserID';

ALTER TABLE [bx_PointLogs] ADD 
CONSTRAINT [FK_bx_PointLogs_UserID] FOREIGN KEY ([UserID]) REFERENCES [bx_Users] ([UserID]) ON DELETE CASCADE,
CONSTRAINT [FK_bx_PointLogs_OperateID] FOREIGN KEY ([OperateID]) REFERENCES [bx_PointLogTypes] ([OperateID])

GO