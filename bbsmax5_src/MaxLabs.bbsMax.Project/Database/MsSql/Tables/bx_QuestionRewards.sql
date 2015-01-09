EXEC bx_Drop 'bx_QuestionRewards';

CREATE TABLE [bx_QuestionRewards]
(
[ThreadID] [int] NOT NULL,
[PostID] [int] NOT NULL,
[Reward] [int] NOT NULL,
CONSTRAINT [PK_bx_QuestionRewards] PRIMARY KEY CLUSTERED  ([ThreadID], [PostID])
) 

GO

CREATE INDEX [IX_bx_QuestionRewards_List] ON [bx_QuestionRewards] ([ThreadID], [Reward]);

GO
