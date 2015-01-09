EXEC bx_Drop 'bx_ThreadCatalogsInForums';

CREATE TABLE [bx_ThreadCatalogsInForums]
(
[ForumID] [int] NOT NULL,
[ThreadCatalogID] [int] NOT NULL,
[TotalThreads] [int]  NOT NULL CONSTRAINT [DF_bx_ThreadCatalogsInForums_TotalThreads] DEFAULT ((0)),
[SortOrder] [int] NOT NULL,
CONSTRAINT [PK_bx_ThreadCatalogsInForums] PRIMARY KEY ([ForumID], [ThreadCatalogID])
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_ThreadCatalogsInForums] ON [bx_ThreadCatalogsInForums] ([ForumID], [SortOrder]);

GO
