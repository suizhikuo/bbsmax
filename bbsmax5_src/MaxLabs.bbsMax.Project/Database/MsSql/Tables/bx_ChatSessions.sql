--消息表
EXEC bx_Drop bx_ChatSessions;

CREATE TABLE [bx_ChatSessions] (
	 [ChatSessionID]        int                   IDENTITY(1, 1)                        NOT NULL
	,[UserID]               int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatSessions_UserID]             DEFAULT(0)
	,[TargetUserID]         int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatSessions_TargetUserID]       DEFAULT(0)

	,[TotalMessages]        int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatSessions_TotalMessages]      DEFAULT(0)
	,[UnreadMessages]       int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatSessions_UnreadMessages]     DEFAULT(0)
	
	,[LastMessage]          nvarchar(3000)         COLLATE Chinese_PRC_CI_AS_WS         NOT NULL    CONSTRAINT [DF_bx_ChatSessions_LastMessage]        DEFAULT(N'')

	,[CreateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_ChatSessions_CreateDate]         DEFAULT(GETDATE())
	,[UpdateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_ChatSessions_UpdateDate]         DEFAULT(GETDATE())

	,CONSTRAINT [PK_bx_ChatSessions] PRIMARY KEY ([ChatSessionID])
);

/*
Name:     
Columns:

*/

GO

--用户ID索引
CREATE NONCLUSTERED INDEX [IX_bx_ChatSessions_List] ON [bx_ChatSessions]([UserID], [UpdateDate])

CREATE NONCLUSTERED INDEX [IX_bx_ChatSessions_Unread] ON [bx_ChatSessions]([UserID], [UnreadMessages], [UpdateDate])

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_ChatSessions_User] ON [bx_ChatSessions]([UserID], [TargetUserID])
GO