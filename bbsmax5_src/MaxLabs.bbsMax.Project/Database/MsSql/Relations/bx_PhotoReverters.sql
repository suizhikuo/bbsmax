EXEC bx_Drop 'FK_bx_PhotoReverters_PhotoID';

ALTER TABLE [bx_PhotoReverters] ADD 
CONSTRAINT [FK_bx_PhotoReverters_PhotoID] FOREIGN KEY ([PhotoID]) REFERENCES [bx_Photos] ([PhotoID]) ON UPDATE CASCADE ON DELETE CASCADE

GO