--用户隶属的用户组和用户表的外键关系
EXEC bx_Drop 'FK_bx_UserRoles_UserID';

ALTER TABLE [bx_UserRoles] ADD 
     CONSTRAINT [FK_bx_UserRoles_UserID]     FOREIGN KEY ([UserID])     REFERENCES [bx_Users]       ([UserID])  ON UPDATE CASCADE ON DELETE CASCADE

GO

