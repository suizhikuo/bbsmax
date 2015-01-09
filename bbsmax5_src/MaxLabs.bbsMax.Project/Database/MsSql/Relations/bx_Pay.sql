--用户 相册关系表外键关系
EXEC bx_Drop 'FK_bx_Pay_UserID';

ALTER TABLE [bx_Pay] ADD 
      CONSTRAINT [FK_bx_Pay_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO