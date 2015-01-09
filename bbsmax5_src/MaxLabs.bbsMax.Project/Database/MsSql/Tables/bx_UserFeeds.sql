EXEC bx_Drop 'bx_UserFeeds';

CREATE TABLE [bx_UserFeeds] (
	 [ID]				int                 IDENTITY (1, 1)                 NOT NULL 
    ,[FeedID]           int                                                 NOT NULL 
    ,[UserID]           int                                                 NOT NULL
    
    ,[Realname]         nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_UserFeeds_Username]       DEFAULT ('') 
    
	,[CreateDate]       datetime                                            NOT NULL    CONSTRAINT [DF_bx_UserFeeds_CreateDate]     DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_UserFeeds] PRIMARY KEY ([UserID],[FeedID])
);

/*
Name: 用户动态表
      记录每条动态相关的用户
      比如 a 评论了f的日志"伤感" b也评论了f的日志"伤感"  那么这里就记a和b的用户ID并且对应同一个FeedID
      如果 a和f成为了好友 b也和f成为了好友 那么这里除了记录a和b的用户ID外 还需记录f的用户ID (就这个特殊与其它不同)
Columns:
	[ID]
    [FeedID]           动态ID
    [UserID]           用户ID  如果是全局动态时为-1
        
    [Realname]         昵称
    
    [CreateDate]       时间
*/

EXEC bx_Drop 'IX_bx_UserFeeds_CreateDate';
CREATE  INDEX [IX_bx_UserFeeds_CreateDate] ON [bx_UserFeeds]([CreateDate])

EXEC bx_Drop 'IX_bx_UserFeeds_ID';
CREATE  UNIQUE  INDEX [IX_bx_UserFeeds_ID] ON [bx_UserFeeds]([ID])

EXEC bx_Drop 'IX_bx_UserFeeds_FeedID';
CREATE  INDEX [IX_bx_UserFeeds_FeedID] ON [bx_UserFeeds]([FeedID])

GO
