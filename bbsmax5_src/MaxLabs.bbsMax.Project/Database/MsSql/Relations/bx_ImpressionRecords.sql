EXEC bx_Drop 'FK_bx_ImpressionRecords_TypeID';

ALTER TABLE [bx_ImpressionRecords] ADD 
CONSTRAINT [FK_bx_ImpressionRecords_TypeID] FOREIGN KEY ([TypeID]) REFERENCES [bx_ImpressionTypes] ([TypeID]) ON UPDATE CASCADE ON DELETE CASCADE

EXEC bx_Drop 'FK_bx_ImpressionRecords_UserID';

ALTER TABLE [bx_ImpressionRecords] ADD
CONSTRAINT [FK_bx_ImpressionRecords_UserID] FOREIGN KEY ([UserID]) REFERENCES [bx_Users] ([UserID]) ON UPDATE CASCADE ON DELETE CASCADE

GO
