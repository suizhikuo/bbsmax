EXEC bx_Drop 'bx_QuestionUsers';

CREATE TABLE [bx_QuestionUsers](
	[ThreadID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[BestPostIsUseful] [bit] NOT NULL CONSTRAINT [DF_bx_QuestionUsers_BestPostIsUseful]  DEFAULT ((0))
) 

GO

