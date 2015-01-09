EXEC bx_Drop 'bx_Forums';

CREATE TABLE [bx_Forums] (
		[ForumID]				int													NOT NULL IDENTITY(1, 1),
		[ParentID]				int													NOT NULL,
		[ClubID]			    int													NOT NULL CONSTRAINT [DF_bx_Forums_ClubID]									    DEFAULT (0),
		[ForumType]				tinyint												NOT NULL,
		[ForumStatus]			tinyint			    								NOT NULL CONSTRAINT [DF_bx_Forums_ForumStatus]									DEFAULT (0),
		[ThreadCatalogStatus]	tinyint												NOT NULL CONSTRAINT [DF_bx_Forums_ThreadCatalogStatus]							DEFAULT (2),

		[VisitPaths]			nvarchar(2000)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL CONSTRAINT [DF_bx_Forums_VisitPaths]									DEFAULT (''),

		--[SubDomain]				nvarchar(100)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL CONSTRAINT [DF_bx_Forums_SubDomain]									DEFAULT (''),
		[CodeName]				nvarchar(100)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL CONSTRAINT [DF_bx_Forums_CodeName]										DEFAULT (''),

		[ForumName]				nvarchar(500)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL,
		[Description]			ntext				COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL,
		[Readme]				ntext				COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL,
		[LogoSrc]				nvarchar(200)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL CONSTRAINT [DF_bx_Forums_LogoSrc]										DEFAULT (''),
		[ThemeID]				nvarchar(100)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL CONSTRAINT [DF_bx_Forums_ThemeID]										DEFAULT (''),
		[ColumnSpan]			tinyint												NOT NULL CONSTRAINT [DF_bx_Forums_ColumnSpan]									DEFAULT (0),
		[TotalThreads]			int													NOT NULL CONSTRAINT [DF_bx_Forums_ThreadCount]									DEFAULT (0),
		[TotalPosts]			int													NOT NULL CONSTRAINT [DF_bx_Forums_PostCount]									DEFAULT (0),
		[TodayThreads]			int													NOT NULL CONSTRAINT [DF_bx_Forums_TodayThreadCount]								DEFAULT (0),
		[TodayPosts]			int													NOT NULL CONSTRAINT [DF_bx_Forums_TodayPostCount]								DEFAULT (0),
		[LastThreadID]			int													NOT NULL CONSTRAINT [DF_bx_Forums_LastPostID]									DEFAULT (0),
		[YestodayLastThreadID]	int													NOT NULL CONSTRAINT [DF_bx_Forums_YestodayLastThreadID]							DEFAULT (0),
		[YestodayLastPostID]	int													NOT NULL CONSTRAINT [DF_bx_Forums_YestodayLastPostID]							DEFAULT (0),
		[Password]				nvarchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL CONSTRAINT [DF_bx_Forums_Password]										DEFAULT (''),
		[SortOrder]				int													NOT NULL CONSTRAINT [DF_bx_Forums_SortOrder]									DEFAULT (0),
		
		[ExtendedAttributes]	ntext				COLLATE Chinese_PRC_CI_AS_WS	NULL,
		CONSTRAINT [PK_bx_Forms] PRIMARY KEY ([ForumID])
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Forums_CodeName] ON [bx_Forums] ([CodeName]);
CREATE INDEX [IX_bx_Forums_SortOrder] ON [bx_Forums]([ParentID], [SortOrder]);
GO
