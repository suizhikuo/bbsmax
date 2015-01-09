ALTER TABLE [bx_AdminSessions] ADD 
	CONSTRAINT [FK_bx_AdminSessions_UserID] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [bx_Users] (
		[UserID]
	) ON DELETE CASCADE 