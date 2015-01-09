EXEC bx_Drop 'bx_ThreadExchanges';

CREATE TABLE [bx_ThreadExchanges]
(
	[ThreadID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[Price] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	CONSTRAINT [PK_bx_ThreadExchanges] PRIMARY KEY ([ThreadID], [UserID])
)

GO
