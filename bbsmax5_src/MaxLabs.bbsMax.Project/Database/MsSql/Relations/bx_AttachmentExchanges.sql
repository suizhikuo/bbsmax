
EXEC bx_Drop 'FK_bx_AttachmentExchanges_AttachmentID';

ALTER TABLE [bx_AttachmentExchanges] ADD 
   CONSTRAINT [FK_bx_AttachmentExchanges_AttachmentID]        FOREIGN KEY ([AttachmentID]) REFERENCES [bx_Attachments] ([AttachmentID]) ON DELETE CASCADE ON UPDATE CASCADE
GO

