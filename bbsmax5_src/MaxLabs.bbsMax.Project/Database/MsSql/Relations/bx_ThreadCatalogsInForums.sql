
EXEC bx_Drop 'FK_bx_ThreadCatalogsInForums_ThreadCatalogID';

ALTER TABLE [bx_ThreadCatalogsInForums] ADD
	CONSTRAINT [FK_bx_ThreadCatalogsInForums_ForumID]        FOREIGN KEY ([ForumID])         REFERENCES [bx_Forums]         ([ForumID])   ON UPDATE CASCADE  ON DELETE CASCADE
	,CONSTRAINT [FK_bx_ThreadCatalogsInForums_ThreadCatalogID]        FOREIGN KEY ([ThreadCatalogID])         REFERENCES [bx_ThreadCatalogs]         ([ThreadCatalogID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

