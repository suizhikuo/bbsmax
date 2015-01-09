--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_UserNoAddFeedApps_UserID';

ALTER TABLE [bx_UserNoAddFeedApps] ADD 
   CONSTRAINT [FK_bx_UserNoAddFeedApps_UserID]        FOREIGN KEY ([UserID])         REFERENCES [bx_Users]         ([UserID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

