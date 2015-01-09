EXEC bx_Drop 'FK_bx_TempUploadFiles_UserID';

ALTER TABLE [bx_TempUploadFiles]  WITH CHECK ADD  CONSTRAINT [FK_bx_TempUploadFiles_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE