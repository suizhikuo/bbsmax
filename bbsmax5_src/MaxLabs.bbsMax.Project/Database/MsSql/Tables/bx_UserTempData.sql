EXEC bx_Drop 'bx_UserTempData'

GO

CREATE TABLE [bx_UserTempData] (
	 [UserID]          int                                      NOT NULL 
	,[DataType]        tinyint	                                 NOT NULL 
	,[CreateDate]      datetime                                 NULL             CONSTRAINT [DF_bx_UserTempData_CreatDate]  DEFAULT (GETDATE())
	,[ExpiresDate]     datetime                                 NULL 
	,[Data]            ntext COLLATE Chinese_PRC_CI_AS_WS NULL 
)
GO


ALTER TABLE [bx_UserTempData] ADD 
	CONSTRAINT [PK_bx_UserTempData] PRIMARY KEY  CLUSTERED 
	(
		[UserID],
		[DataType]
	)
GO