EXEC bx_Drop 'FK_bx_Impressions_TypeID';

ALTER TABLE [bx_Impressions] ADD 
CONSTRAINT [FK_bx_Impressions_TypeID] FOREIGN KEY ([TypeID]) REFERENCES [bx_ImpressionTypes] ([TypeID]) ON UPDATE CASCADE ON DELETE CASCADE

EXEC bx_Drop 'FK_bx_Impressions_UserID';

ALTER TABLE [bx_Impressions] ADD 
CONSTRAINT [FK_bx_Impressions_UserID] FOREIGN KEY ([UserID]) REFERENCES [bx_Users] ([UserID]) ON UPDATE CASCADE ON DELETE CASCADE

GO
