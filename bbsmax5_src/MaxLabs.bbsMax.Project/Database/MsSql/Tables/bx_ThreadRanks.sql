EXEC bx_Drop 'bx_ThreadRanks';

CREATE TABLE [bx_ThreadRanks]
(
[ThreadID] [int] NOT NULL,
[UserID] [int] NOT NULL,
[Rank] [tinyint] NOT NULL CONSTRAINT [DF_bx_ThreadRanks_Rank] DEFAULT ((0)),
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_ThreadRanks_CreateDate] DEFAULT (getdate()),
[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_ThreadRanks_UpdateDate] DEFAULT (getdate()),
CONSTRAINT [PK_bx_ThreadRanks] PRIMARY KEY ([ThreadID], [UserID])
)

GO

CREATE NONCLUSTERED INDEX [IX_ThreadRanks_CreateDate] ON [bx_ThreadRanks] ([CreateDate] DESC);
GO
