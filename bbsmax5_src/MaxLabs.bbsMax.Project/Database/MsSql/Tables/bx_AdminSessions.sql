CREATE TABLE [bx_AdminSessions] (
	[SessionID]			uniqueidentifier			NOT NULL									CONSTRAINT [DF_bx_AdminSessions_SessionID] DEFAULT (newid())
	,[IpAddress]		varchar(100)		COLLATE Chinese_PRC_CI_AS_WS		NOT NULL			CONSTRAINT [DF_bx_AdminSessions_IpAddress] DEFAULT ('')
	,[UserID]			int							NOT NULL 
	,[CreateDate]		datetime					NOT NULL									CONSTRAINT [DF_bx_AdminSessions_CreateDate] DEFAULT (getdate())
	,[UpdateDate]		datetime					NOT NULL									CONSTRAINT [DF_bx_AdminSessions_UpdateDate] DEFAULT (getdate())
	,[Available]		bit							NOT NULL									CONSTRAINT [DF_bx_AdminSessions_Available] DEFAULT (1)
	
	CONSTRAINT [PK_bx_AdminSessions] PRIMARY KEY  CLUSTERED ([SessionID])
)