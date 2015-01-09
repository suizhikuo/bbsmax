EXEC bx_Drop 'FK_bx_BlogCategoryReverters_CategoryID';

ALTER TABLE [bx_BlogCategoryReverters] ADD 
CONSTRAINT [FK_bx_BlogCategoryReverters_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [bx_BlogCategories] ([CategoryID]) ON UPDATE CASCADE ON DELETE CASCADE

GO