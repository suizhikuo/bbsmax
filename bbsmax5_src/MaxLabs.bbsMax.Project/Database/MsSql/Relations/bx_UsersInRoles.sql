--用户隶属的用户组和用户表的外键关系
EXEC bx_Drop 'FK_bx_UsersInRoles_UserID';

ALTER TABLE bx_UsersInRoles ADD 
     CONSTRAINT FK_bx_UsersInRoles_UserID     FOREIGN KEY ([UserID])     REFERENCES [bx_Users]       ([UserID])  ON UPDATE CASCADE ON DELETE CASCADE

GO

