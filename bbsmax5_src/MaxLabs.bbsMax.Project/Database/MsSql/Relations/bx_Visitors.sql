--访问者 用户关系表外键关系
EXEC bx_Drop 'FK_bx_Visitors_UserID';
EXEC bx_Drop 'FK_bx_Visitors_VisitorUserID';

ALTER TABLE [bx_Visitors] ADD 
      CONSTRAINT [FK_bx_Visitors_UserID]           FOREIGN KEY ([UserID])           REFERENCES [bx_Users]         ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE
     ,CONSTRAINT [FK_bx_Visitors_VisitorUserID]    FOREIGN KEY ([VisitorUserID])    REFERENCES [bx_Users]         ([UserID])   

GO
