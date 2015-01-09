ALTER TABLE [bx_Emoticons] ADD 
	CONSTRAINT [FK_bx_Emoticons_GroupID] FOREIGN KEY 
	(
		[GroupID]
	) REFERENCES [bx_EmoticonGroups] (
		[GroupID]
	) ON DELETE CASCADE 
GO