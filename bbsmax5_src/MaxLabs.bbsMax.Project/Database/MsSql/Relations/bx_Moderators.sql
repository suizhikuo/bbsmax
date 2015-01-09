EXEC bx_Drop '[FK_bx_Moderators_bx_Forums]';

ALTER TABLE [bx_Moderators] ADD 
	CONSTRAINT [FK_bx_Moderators_bx_Forums] FOREIGN KEY 
	(
		[ForumID]
	) REFERENCES [bx_Forums] (
		[ForumID]
	) ON DELETE CASCADE ,
	CONSTRAINT [FK_bx_Moderators_bx_Users] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [bx_Users] (
		[UserID]
	) ON DELETE CASCADE 