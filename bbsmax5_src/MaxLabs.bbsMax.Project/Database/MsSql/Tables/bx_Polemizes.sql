EXEC bx_Drop 'bx_Polemizes';

CREATE TABLE [bx_Polemizes](
	[ThreadID] [int] NOT NULL,
	[AgreeViewPoint] NTEXT COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
	[AgreeCount] [int] NOT NULL  CONSTRAINT [DF_bx_Polemizes_AgreeCount]  DEFAULT ((0)),
	[AgainstViewPoint] NTEXT COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
	[AgainstCount] [int] NOT NULL CONSTRAINT [DF_bx_Polemizes_AgainstCount]  DEFAULT ((0)),
	[NeutralCount] [int] NOT NULL CONSTRAINT [DF_bx_Polemizes_NeutralCount]  DEFAULT ((0)),
	[ExpiresDate] [datetime] NOT NULL CONSTRAINT [DF_bx_Polemizes_ExpiresDate]  DEFAULT (getdate()),
CONSTRAINT [PK_bx_Polemizes] PRIMARY KEY CLUSTERED  ([ThreadID] ASC)
) 

GO
