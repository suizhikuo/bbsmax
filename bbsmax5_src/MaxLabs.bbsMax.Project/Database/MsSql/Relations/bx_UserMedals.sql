--用户的扩展字段表外键关系
EXEC bx_Drop 'FK_bx_UserMedals_UserID';

ALTER TABLE [bx_UserMedals] ADD 
     CONSTRAINT [FK_bx_UserMedals_UserID]     FOREIGN KEY ([UserID])     REFERENCES [bx_Users]       ([UserID])  ON UPDATE CASCADE ON DELETE CASCADE

GO

