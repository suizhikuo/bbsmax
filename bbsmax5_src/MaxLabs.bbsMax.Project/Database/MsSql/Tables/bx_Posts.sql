EXEC bx_Drop 'bx_Posts';

CREATE TABLE [bx_Posts]
(
[PostID] [int] NOT NULL IDENTITY(1, 1),
[ParentID] [int] NULL,
[ForumID] [int] NOT NULL,
[ThreadID] [int] NOT NULL,
[PostType] [tinyint] NOT NULL CONSTRAINT [DF_bx_Posts_PostType] DEFAULT ((0)),
[IconID] [int] NOT NULL CONSTRAINT [DF_bx_Posts_IconID] DEFAULT ((0)),
[Subject] [nvarchar] (256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[Content] [ntext] COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[ContentFormat] [tinyint] NOT NULL CONSTRAINT [DF_bx_Posts_ContentFormat] DEFAULT ((10)),
[EnableSignature] [bit] NOT NULL CONSTRAINT [DF_bx_Posts_EnableSignature] DEFAULT ((1)),
[EnableReplyNotice] [bit] NOT NULL CONSTRAINT [DF_bx_Posts_EnableReplyNotice] DEFAULT ((0)),
[IsShielded] [bit] NOT NULL CONSTRAINT [DF_bx_Posts_IsShielded] DEFAULT ((0)),
[UserID] [int] NOT NULL CONSTRAINT [DF_bx_Posts_UserID] DEFAULT ((0)),
[NickName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[LastEditorID] [int]  NOT NULL CONSTRAINT [DF_bx_Posts_LastEditorID] DEFAULT ((0)),
[LastEditor] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Posts_LastEditor] DEFAULT (N''),
[IPAddress] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Posts_IPAddress] DEFAULT ('000.000.000.000'),
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_Posts_CreateDate] DEFAULT (getdate()),
[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_Posts_UpdateDate] DEFAULT (getdate()),
[MarkCount] [int] NULL,
[LoveCount] [int] NULL,
[HateCount] [int] NULL,
[SortOrder] [bigint] NOT NULL,
[FloorNumber] [int] NULL, 
[KeywordVersion]   varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NULL,
[HistoryAttachmentIDs]   varchar(500)       COLLATE Chinese_PRC_CI_AS_WS    NULL,
CONSTRAINT [PK_bx_Posts] PRIMARY KEY ([PostID])
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Posts_Forum] ON [bx_Posts] ([ForumID] ASC, [SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Posts_SortOrder] ON [bx_Posts] ([SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Posts_User] ON [bx_Posts] ([UserID] ASC,[ThreadID] DESC,[SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Posts_Thread] ON [bx_Posts] ([ThreadID] DESC, [SortOrder] DESC);

CREATE INDEX [IX_bx_Posts_CreateDate] ON [bx_Posts] ([CreateDate] DESC);
GO