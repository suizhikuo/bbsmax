CREATE TABLE [bx_BannedUsers] (
	 [UserID]			int			NOT NULL 
	,[ForumID]			int			NOT NULL 
	,[BeginDate]		datetime	NULL				CONSTRAINT [DF_bx_BannedUsers_BeginDate] DEFAULT('1753-1-1')
	,[EndDate]			datetime	NOT NULL 			CONSTRAINT [DF_bx_BannedUsers_EndDate] DEFAULT('9999-12-31 23:59:59')
	,[Cause]			nvarchar(1000) COLLATE Chinese_PRC_CI_AS_WS  NULL             CONSTRAINT [DF_bx_BannedUsers_Cause] DEFAULT('')
		CONSTRAINT [PK_bx_BannedUsers] PRIMARY KEY  CLUSTERED ([UserID],[ForumID])
) 