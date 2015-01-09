EXEC bx_Drop 'bx_Polls';

CREATE TABLE [bx_Polls]
(
[ThreadID] [int] NOT NULL,
[Multiple] [int] NOT NULL CONSTRAINT [DF_bx_Polls_IsMultiple] DEFAULT ((1)),
[AlwaysEyeable] [bit] NOT NULL CONSTRAINT [DF_bx_Polls_AlwaysEyeable] DEFAULT ((0)),
[ExpiresDate] [datetime] NOT NULL,
CONSTRAINT [PK_bx_Polls] PRIMARY KEY CLUSTERED  ([ThreadID] ASC)
) 

GO

CREATE INDEX [IX_bx_Polls_ExpiresDate] ON [bx_Polls] ([ExpiresDate]);

GO
