EXEC bx_Drop 'FK_bx_RecoverPasswordLogs_bx_Users';

ALTER TABLE bx_RecoverPasswordLogs  WITH CHECK ADD  CONSTRAINT [FK_bx_RecoverPasswordLogs_bx_Users] FOREIGN KEY([UserID])
REFERENCES bx_Users([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE bx_RecoverPasswordLogs CHECK CONSTRAINT [FK_bx_RecoverPasswordLogs_bx_Users]
GO