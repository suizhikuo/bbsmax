EXEC bx_Drop 'bx_StickThreads';

CREATE TABLE [bx_StickThreads] (
	 [ID]           int             IDENTITY (1, 1)                   NOT NULL 
	,[ThreadID]		int				NOT NULL    CONSTRAINT [DF_bx_StickThreads_ThreadID]         DEFAULT (0)
    ,[ForumID]		int				NOT NULL    CONSTRAINT [DF_bx_StickThreads_ForumID]          DEFAULT (0)
	
    ,CONSTRAINT [PK_bx_StickThreads] PRIMARY KEY ([ID])
);

/*
Name: 一般置顶的主题
Columns:
    [ThreadID]				主题ID
	[ForumID]				版块ID
*/

CREATE UNIQUE INDEX [IX_bx_StickThreads_ForumThread] ON [bx_StickThreads]([ThreadID],[ForumID]);
CREATE INDEX [IX_bx_StickThreads_ForumID] ON [bx_StickThreads]([ForumID]);
GO

