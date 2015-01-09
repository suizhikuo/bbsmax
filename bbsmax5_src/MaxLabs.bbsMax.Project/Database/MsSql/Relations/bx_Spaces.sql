----空间 用户关系表外键关系
--EXEC bx_Drop 'FK_bx_Spaces_UserID';

--ALTER TABLE [bx_Spaces] ADD 
      --CONSTRAINT [FK_bx_Spaces_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE

--GO