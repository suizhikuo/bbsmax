EXEC bx_Drop 'FK_bx_InviteSerials_UserID';

ALTER TABLE [bx_InviteSerials]  WITH CHECK ADD  CONSTRAINT [FK_bx_InviteSerials_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE