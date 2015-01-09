EXEC bx_Drop 'FK_bx_ClubMembers_ClubID';
EXEC bx_Drop 'FK_bx_ClubMembers_UserID';

ALTER TABLE [bx_ClubMembers] ADD
CONSTRAINT [FK_bx_ClubMembers_ClubID] FOREIGN KEY ([ClubID]) REFERENCES [bx_Clubs] ([ClubID]) ON UPDATE CASCADE ON DELETE CASCADE,
CONSTRAINT [FK_bx_ClubMembers_UserID] FOREIGN KEY ([UserID]) REFERENCES [bx_Users] ([UserID]) ON UPDATE CASCADE ON DELETE CASCADE

GO
