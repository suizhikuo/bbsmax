EXEC bx_Drop 'FK_bx_ThreadCateModels_CateID';

ALTER TABLE [bx_ThreadCateModels]  WITH CHECK ADD  CONSTRAINT [FK_bx_ThreadCateModels_CateID] FOREIGN KEY([CateID])
REFERENCES [bx_ThreadCates] ([CateID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO

EXEC bx_Drop 'FK_bx_ThreadCateModelFields_ModelID';

ALTER TABLE [bx_ThreadCateModelFields]  WITH CHECK ADD  CONSTRAINT [FK_bx_ThreadCateModelFields_ModelID] FOREIGN KEY([ModelID])
REFERENCES [bx_ThreadCateModels] ([ModelID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO