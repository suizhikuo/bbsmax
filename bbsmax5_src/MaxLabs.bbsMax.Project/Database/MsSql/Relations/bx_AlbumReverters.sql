EXEC bx_Drop 'FK_bx_AlbumReverters_AlbumID';

ALTER TABLE [bx_AlbumReverters] ADD 
CONSTRAINT [FK_bx_AlbumReverters_AlbumID] FOREIGN KEY ([AlbumID]) REFERENCES [bx_Albums] ([AlbumID]) ON UPDATE CASCADE ON DELETE CASCADE

GO