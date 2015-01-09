--外键关系
EXEC bx_Drop 'FK_bx_Notify_UserID';
EXEC bx_Drop 'FK_bx_Notify_TypeID';

ALTER TABLE [bx_Notify] ADD 
     CONSTRAINT [FK_bx_Notify_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE,
     CONSTRAINT [FK_bx_Notify_TypeID]    FOREIGN KEY ([TypeID])    REFERENCES [bx_NotifyTypes] ([TypeID]) ON UPDATE CASCADE  ON DELETE CASCADE,
     CONSTRAINT [FK_bx_Notify_Client]    FOREIGN KEY ([ClientID])  REFERENCES [bx_PassportClients] ([ClientID]) ON UPDATE CASCADE  ON DELETE CASCADE
GO