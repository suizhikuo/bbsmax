EXEC bx_Drop 'FK_bx_Clubs_CategoryID';

ALTER TABLE [bx_Clubs] ADD 
CONSTRAINT [FK_bx_Clubs_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [bx_ClubCategories] ([CategoryID]) ON UPDATE CASCADE ON DELETE CASCADE
GO
