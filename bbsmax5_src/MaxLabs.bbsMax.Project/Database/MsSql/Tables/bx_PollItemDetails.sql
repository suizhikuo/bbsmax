EXEC bx_Drop 'bx_PollItemDetails';

CREATE TABLE [bx_PollItemDetails]
(
[ItemID] [int] NOT NULL,
[UserID] [int] NOT NULL,
[NickName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_PollItemDetails_NickName] DEFAULT (''),
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_PollItemDetails_CreateDate] DEFAULT (getdate()),
CONSTRAINT [PK_bx_PollItemDetails] PRIMARY KEY CLUSTERED  ([ItemID], [UserID])
) 

GO
