EXEC bx_Drop 'bx_TopicStatus';

CREATE TABLE [bx_TopicStatus] (
	 [ID]           int             IDENTITY (1, 1)                   NOT NULL 
	,[ThreadID]		int				NOT NULL    CONSTRAINT [DF_bx_TopicStatus_ThreadID]         DEFAULT (0)
    ,[Type]			tinyint         NOT NULL    CONSTRAINT [DF_bx_TopicStatus_Type]             DEFAULT (0)
	,[EndDate]		datetime        NOT NULL    CONSTRAINT [DF_bx_TopicStatus_EndDate]			DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_TopicStatus] PRIMARY KEY ([ID])
);

/*
Name: 主题定时状态
Columns:
    [ThreadID]				主题ID
	[Type]					类型 （置顶，高亮，锁定）
    [EndDate]               过期时间
*/

CREATE UNIQUE INDEX [IX_bx_TopicStatus_ThreadType] ON [bx_TopicStatus]([ThreadID],[Type]);
CREATE INDEX [IX_bx_TopicStatus_EndDate] ON [bx_TopicStatus]([EndDate]);

GO

