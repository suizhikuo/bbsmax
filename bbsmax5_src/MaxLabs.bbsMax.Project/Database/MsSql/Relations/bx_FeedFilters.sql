--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_FeedFilters_UserID';

ALTER TABLE [bx_FeedFilters] ADD 
   CONSTRAINT [FK_bx_FeedFilters_UserID]        FOREIGN KEY ([UserID])         REFERENCES [bx_Users]         ([UserID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

