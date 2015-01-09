EXEC bx_Drop 'FK_bx_BlogArticleReverters_ArticleID';

ALTER TABLE [bx_BlogArticleReverters] ADD 
CONSTRAINT [FK_bx_BlogArticleReverters_ArticleID] FOREIGN KEY ([ArticleID]) REFERENCES [bx_BlogArticles] ([ArticleID]) ON UPDATE CASCADE ON DELETE CASCADE

GO