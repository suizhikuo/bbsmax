EXEC bx_Drop 'FK_bx_UserReverters_UserID';

ALTER TABLE [bx_UserReverters]  WITH CHECK ADD  CONSTRAINT [FK_bx_UserReverters_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE