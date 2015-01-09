--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_UserFeeds_FeedID';

ALTER TABLE [bx_UserFeeds] ADD 
   CONSTRAINT [FK_bx_UserFeeds_FeedID]        FOREIGN KEY ([FeedID])         REFERENCES [bx_Feeds]         ([ID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO

