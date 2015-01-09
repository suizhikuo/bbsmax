--外键关系
EXEC bx_Drop 'FK_bx_UnreadNotifies_UserID';
EXEC bx_Drop 'FK_bx_UnreadNotifies_TypeID';

ALTER TABLE [bx_UnreadNotifies] ADD 
     CONSTRAINT [FK_bx_UnreadNotifies_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE,
     CONSTRAINT [FK_bx_UnreadNotifies_TypeID]    FOREIGN KEY ([TypeID])    REFERENCES [bx_NotifyTypes] ([TypeID]) ON UPDATE CASCADE  ON DELETE CASCADE
GO