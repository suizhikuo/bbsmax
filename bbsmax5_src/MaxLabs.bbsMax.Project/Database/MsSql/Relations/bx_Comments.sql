--评论 用户关系表外键关系
EXEC bx_Drop 'FK_bx_Comments_UserID';

ALTER TABLE [bx_Comments] ADD 
      CONSTRAINT [FK_bx_Comments_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO