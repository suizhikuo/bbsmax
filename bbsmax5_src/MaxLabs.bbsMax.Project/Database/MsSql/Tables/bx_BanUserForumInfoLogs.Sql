EXEC bx_Drop 'bx_BanUserLogForumInfos';

CREATE	TABLE	bx_BanUserLogForumInfos(
	[LogID]				int					NOT NULL				CONSTRAINT	[DF_bx_BanUserForumInfoLogs_ID]				DEFAULT(0)
	
	,[ForumID]			int					NOT NULL				CONSTRAINT	[DF_bx_BanUserForumInfoLogs_ForumID]		DEFAULT(0)
	
	,[ForumName]		nvarchar(50)		NOT NULL				CONSTRAINT  [DF_bx_BanUserForumInfoLogs_ForumName]		DEFAULT(N'')	

	,[EndDate]			datetime			NOT NULL				CONSTRAINT  [DF_bx_BanUserForumInfoLogs_EndDate]		DEFAULT(getdate())
)