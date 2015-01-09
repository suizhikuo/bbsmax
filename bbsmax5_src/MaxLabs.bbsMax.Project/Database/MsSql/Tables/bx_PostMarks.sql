CREATE TABLE [bx_PostMarks]
(
[PostMarkID] [int] NOT NULL IDENTITY(1, 1),
[PostID] [int] NOT NULL,
[UserID] [int] NOT NULL,
[Username] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_PostMarks_CreateDate] DEFAULT (getdate()),
[ExtendedPoints_1] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_1] DEFAULT ((0)),
[ExtendedPoints_2] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_2] DEFAULT ((0)),
[ExtendedPoints_3] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_3] DEFAULT ((0)),
[ExtendedPoints_4] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_4] DEFAULT ((0)),
[ExtendedPoints_5] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_5] DEFAULT ((0)),
[ExtendedPoints_6] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_6] DEFAULT ((0)),
[ExtendedPoints_7] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_7] DEFAULT ((0)),
[ExtendedPoints_8] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_8] DEFAULT ((0)),
[Reason] [ntext] COLLATE Chinese_PRC_CI_AS_WS NULL,
CONSTRAINT [PK_bx_PostMarks] PRIMARY KEY ([PostMarkID])
)

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_PostMarks] ON [bx_PostMarks] ([PostID],[UserID]);
CREATE INDEX [IX_bx_PostMarks_CreateDate] ON [bx_PostMarks] ([CreateDate]);
CREATE INDEX [IX_bx_PostMarks_UserID] ON [bx_PostMarks] ([UserID]);

GO