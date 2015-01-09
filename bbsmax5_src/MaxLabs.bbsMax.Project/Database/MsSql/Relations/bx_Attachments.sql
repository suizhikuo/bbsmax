
EXEC bx_Drop 'FK_bx_Attachments_PostID';

ALTER TABLE [bx_Attachments] ADD 
   CONSTRAINT [FK_bx_Attachments_PostID]        FOREIGN KEY ([PostID]) REFERENCES [bx_Posts] ([PostID]) ON DELETE CASCADE ON UPDATE CASCADE
GO

