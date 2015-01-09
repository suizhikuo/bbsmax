EXEC bx_Drop '[FK_bx_SmsCodes_bx_Users]';

ALTER TABLE [bx_SmsCodes] ADD 
	CONSTRAINT [FK_bx_SmsCodes_bx_Users] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [bx_Users] (
		[UserID]
	) ON DELETE CASCADE 
GO