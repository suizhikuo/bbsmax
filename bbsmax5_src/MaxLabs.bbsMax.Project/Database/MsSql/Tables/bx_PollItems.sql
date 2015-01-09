EXEC bx_Drop 'bx_PollItems';

CREATE TABLE [bx_PollItems]
(
[ItemID] [int] NOT NULL IDENTITY(1, 1),
[ThreadID] [int] NOT NULL,
[ItemName] [nvarchar] (200) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[PollItemCount] [int] NOT NULL CONSTRAINT [DF_bx_PollItems_PollItemCount] DEFAULT ((0)),
CONSTRAINT [PK_bx_PollItems] PRIMARY KEY CLUSTERED  ([ItemID])
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_PollItems_List] ON [bx_PollItems] ([ThreadID], [ItemID]);
GO
