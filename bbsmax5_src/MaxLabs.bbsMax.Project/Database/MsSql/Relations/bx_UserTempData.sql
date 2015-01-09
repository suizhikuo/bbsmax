
EXEC bx_Drop 'FK_bx_UserTempData_bx_Users';


ALTER TABLE [bx_UserTempData] ADD 
	CONSTRAINT [FK_bx_UserTempData_bx_Users] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [bx_Users] (
		[UserID]
	) ON DELETE CASCADE 
GO
