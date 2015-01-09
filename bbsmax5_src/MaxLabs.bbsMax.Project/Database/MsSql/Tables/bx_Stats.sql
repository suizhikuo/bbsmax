CREATE TABLE [bx_Stats]
(
[StatType] [tinyint] NOT NULL CONSTRAINT [DF_bx_Stats_StatType] DEFAULT ((0)),
[Year] [smallint] NOT NULL,
[Month] [tinyint] NOT NULL,
[Day] [tinyint] NOT NULL,
[Hour] [tinyint] NOT NULL,
[Count] [int] NOT NULL,
[Param] [int] NOT NULL CONSTRAINT [DF_bx_Stats_Param] DEFAULT ((0))
)