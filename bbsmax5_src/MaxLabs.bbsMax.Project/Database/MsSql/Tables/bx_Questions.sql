EXEC bx_Drop 'bx_Questions';

CREATE TABLE [bx_Questions]
(
[ThreadID] [int] NOT NULL,
[IsClosed] [bit] NOT NULL CONSTRAINT [DF_bx_Questions_IsClosed] DEFAULT ((0)),
[Reward] [int] NOT NULL CONSTRAINT [DF_bx_Questions_Reward] DEFAULT ((0)),
[RewardCount] [int] NOT NULL CONSTRAINT [DF_bx_Questions_RewardCount] DEFAULT ((0)),
[AlwaysEyeable] [bit] NOT NULL CONSTRAINT [DF_bx_Questions_AlwaysEyeable] DEFAULT ((0)),
[ExpiresDate] [datetime] NOT NULL,
[BestPostID] [int] NOT NULL CONSTRAINT [DF_bx_Questions_BestPostID] DEFAULT ((0)),
[UsefulCount] [int] NOT NULL CONSTRAINT [DF_bx_Questions_UsefulCount] DEFAULT ((0)),
[UnusefulCount] [int] NOT NULL CONSTRAINT [DF_bx_Questions_UnusefulCount] DEFAULT ((0)),
CONSTRAINT [PK_bx_Questions] PRIMARY KEY CLUSTERED  ([ThreadID])
) 

GO

CREATE INDEX [IX_bx_Questions_ExpiresDate] ON [bx_Questions]([IsClosed], [ExpiresDate]);

GO
