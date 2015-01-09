ALTER TABLE [bx_EmoticonGroups] ADD 
	CONSTRAINT [FK_bx_EmoticonGroup_UserID] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [bx_Users] (
		[UserID]
	) ON DELETE CASCADE 
GO

