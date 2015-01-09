EXEC bx_Drop 'bx_DayLastThreads';

CREATE TABLE [bx_DayLastThreads]
(
[Day] [int] NOT NULL, --- 2010-4-15  则为 2010415
[LastThreadID] [int]  NOT NULL,
CONSTRAINT [PK_bx_DayLastThreads] PRIMARY KEY ([Day])
)

GO

