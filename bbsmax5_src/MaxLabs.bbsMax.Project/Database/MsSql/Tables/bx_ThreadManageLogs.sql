EXEC bx_Drop 'bx_ThreadManageLogs';

CREATE TABLE [bx_ThreadManageLogs]
(
[LogID] [int] NOT NULL IDENTITY(1, 1),
[UserID] [int] NOT NULL,
[UserName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[NickName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_NickName] DEFAULT (''),
[IPAddress] [varchar] (15) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[PostUserID] [int] NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_PostUserID] DEFAULT ((0)),
[ActionType] [tinyint] NOT NULL,
[ForumID] [int] NOT NULL,
[ThreadID] [int] NOT NULL,
[ThreadSubject] [nvarchar] (256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_ThreadSubject] DEFAULT (''),
[Reason] [nvarchar] (256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_Reason] DEFAULT (''),
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_CreatDate] DEFAULT (getdate()),
[IsPublic] [bit] NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_IsPublic] DEFAULT (1),
CONSTRAINT [PK_bx_ThreadManageLogs] PRIMARY KEY ([LogID])
)

GO

CREATE NONCLUSTERED INDEX [IX_ThreadManageLog_ActionType] ON [bx_ThreadManageLogs] ([ActionType]);
CREATE NONCLUSTERED INDEX [IX_ThreadManageLog_UserID_ActionType] ON [bx_ThreadManageLogs] ([UserID], [ActionType]);
CREATE NONCLUSTERED INDEX [IX_ThreadManageLog_UserID] ON [bx_ThreadManageLogs] ([UserID]);
GO
