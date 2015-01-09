--好友 用户关系表外键关系
EXEC bx_Drop 'FK_bx_FriendGroups_UserID';

ALTER TABLE [bx_FriendGroups] ADD 
       CONSTRAINT [FK_bx_FriendGroups_UserID]          FOREIGN KEY ([UserID])          REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO