--用户 相册表外键关系
EXEC bx_Drop 'FK_bx_Albums_UserID';

ALTER TABLE [bx_Albums] ADD 
     CONSTRAINT [FK_bx_Albums_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE
     
GO
