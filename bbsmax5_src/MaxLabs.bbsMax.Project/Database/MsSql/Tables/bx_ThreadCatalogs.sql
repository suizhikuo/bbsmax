EXEC bx_Drop 'bx_ThreadCatalogs';

CREATE TABLE [bx_ThreadCatalogs]
(
[ThreadCatalogID] [int] NOT NULL IDENTITY(1, 1),
[ThreadCatalogName] [nvarchar] (400) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[LogoUrl] [nvarchar] (512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
CONSTRAINT [PK_bx_ThreadCatalogs] PRIMARY KEY ([ThreadCatalogID])
)

GO
