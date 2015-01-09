EXEC bx_Drop 'FK_bx_BlogArticles_CategoryID';

ALTER TABLE [bx_BlogArticles] ADD 
CONSTRAINT [FK_bx_BlogArticles_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [bx_BlogCategories] ([CategoryID]) ON UPDATE CASCADE ON DELETE CASCADE

GO