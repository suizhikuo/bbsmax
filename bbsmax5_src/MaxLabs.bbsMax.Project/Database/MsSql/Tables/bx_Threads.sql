EXEC bx_Drop 'bx_Threads';

CREATE TABLE [bx_Threads]
(
[ThreadID] [int] NOT NULL IDENTITY(1, 1),
[ForumID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_ForumID] DEFAULT ((0)),
[ThreadCatalogID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_ThreadCatalogID] DEFAULT ((0)),
[ThreadType] [tinyint] NOT NULL CONSTRAINT [DF_bx_Threads_ThreadType] DEFAULT ((0)),
--[ThreadStatus] [tinyint] NOT NULL CONSTRAINT [DF_bx_Threads_ThreadStatus] DEFAULT ((0)),
[IconID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_EmoticonID] DEFAULT ((0)),
[Subject] [nvarchar] (256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[SubjectStyle] [nvarchar] (300) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Threads_SubjectStyle] DEFAULT ((0)),
[TotalReplies] [int] NOT NULL CONSTRAINT [DF_bx_Threads_TotalReplies] DEFAULT ((0)),
[TotalViews] [int] NOT NULL CONSTRAINT [DF_bx_Threads_TotalViews] DEFAULT ((0)),
[TotalAttachments] [int] NOT NULL CONSTRAINT [DF_bx_Threads_TotalAttachments] DEFAULT ((0)),
[Price] [int] NOT NULL CONSTRAINT [DF_bx_Threads_Price] DEFAULT ((0)),
[Rank] [tinyint] NOT NULL CONSTRAINT [DF_bx_Threads_Rank] DEFAULT ((0)),
[PostUserID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_PostUserID] DEFAULT ((0)),
[PostNickName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Threads_PostNickName] DEFAULT (''),
[LastPostUserID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_LastPostUserID] DEFAULT ((0)),
[LastPostNickName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Threads_LastPostNickName] DEFAULT (''),
[IsLocked] [bit] NOT NULL CONSTRAINT [DF_bx_Threads_IsLocked] DEFAULT ((0)),
[IsValued] [bit] NOT NULL CONSTRAINT [DF_bx_Threads_IsValued] DEFAULT ((0)),
[Perorate] [nvarchar] (32) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Threads_Perorate] DEFAULT (''),
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_Threads_CreateDate] DEFAULT (getdate()),
[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_Threads_UpdateDate] DEFAULT (getdate()),
[SortOrder] [bigint] NOT NULL,
[ThreadStatus] [tinyint] NOT NULL CONSTRAINT [DF_bx_Posts_ThreadStatus] DEFAULT (1),
[UpdateSortOrder] [bit] NOT NULL CONSTRAINT [DF_bx_Threads_UpdateSortOrder] DEFAULT ((1)),
[ThreadLog] [nvarchar] (128) NOT NULL CONSTRAINT [DF_bx_Threads_ThreadLog] DEFAULT (''),
[JudgementID] [int] CONSTRAINT [DF_bx_Threads_JudgementID] DEFAULT (0),
[ShareCount] [int] CONSTRAINT [DF_bx_Threads_ShareCount] DEFAULT (0),
[CollectionCount] [int] CONSTRAINT [DF_bx_Threads_CollectionCount] DEFAULT (0),
[KeywordVersion]      varchar(32)      COLLATE Chinese_PRC_CI_AS_WS    NULL,
[Words]      nvarchar(400)      COLLATE Chinese_PRC_CI_AS_WS    NULL,
[LastPostID] [int] CONSTRAINT [DF_bx_Threads_LastPostID] DEFAULT (0),
[ContentID] [int] CONSTRAINT [DF_bx_Threads_ContentID] DEFAULT (0),
[PostedCount] [int] NULL, 
[ExtendData] [ntext] COLLATE Chinese_PRC_CI_AS_WS NULL,
[AttachmentType] [tinyint] NOT NULL CONSTRAINT [DF_bx_Posts_AttachmentType] DEFAULT (0), --0没有附件 1一般附件 2有图片附件
CONSTRAINT [PK_bx_Threads] PRIMARY KEY ([ThreadID])
)

GO



CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_SortOrder] ON [bx_Threads] ([SortOrder] DESC);

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_ThreadStatus] ON [bx_Threads] ([ThreadStatus],[SortOrder] DESC);

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Forum] ON [bx_Threads] ([ForumID],[ThreadStatus],[SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Catalog] ON [bx_Threads] ([ThreadCatalogID], [ForumID], [ThreadStatus], [SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Type] ON [bx_Threads] ([ThreadType], [ForumID],[ThreadStatus], [SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_User] ON [bx_Threads] ([PostUserID], [ThreadStatus], [SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Valued] ON [bx_Threads] ([IsValued], [ForumID],[ThreadStatus], [SortOrder] DESC);

CREATE INDEX [IX_bx_Threads_CreateDate] ON [bx_Threads] ([CreateDate] DESC);
CREATE INDEX [IX_bx_Threads_UpdateDate] ON [bx_Threads] ([UpdateDate] DESC);
CREATE INDEX [IX_bx_Threads_ForumTotalReplies] ON [bx_Threads] ([ForumID],[ThreadStatus], [TotalReplies] DESC);
CREATE INDEX [IX_bx_Threads_ForumTotalViews] ON [bx_Threads] ([ForumID],[ThreadStatus], [TotalViews] DESC);
CREATE INDEX [IX_bx_Threads_TotalReplies] ON [bx_Threads] ([ThreadStatus], [TotalReplies] DESC);
CREATE INDEX [IX_bx_Threads_TotalViews] ON [bx_Threads] ([ThreadStatus], [TotalViews] DESC);

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_LastPostID] ON [bx_Threads] ([ThreadStatus],[LastPostID] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Catalog2] ON [bx_Threads] ([ThreadCatalogID], [ForumID], [ThreadStatus], [LastPostID] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Forum2] ON [bx_Threads] ([ForumID],[ThreadStatus],[LastPostID] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Type2] ON [bx_Threads] ([ThreadType], [ForumID],[ThreadStatus], [LastPostID] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_User2] ON [bx_Threads] ([PostUserID], [ThreadStatus], [LastPostID] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Valued2] ON [bx_Threads] ([IsValued], [ForumID],[ThreadStatus], [LastPostID] DESC);

GO