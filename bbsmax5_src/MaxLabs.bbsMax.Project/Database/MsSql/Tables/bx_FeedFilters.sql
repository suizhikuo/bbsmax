EXEC bx_Drop 'bx_FeedFilters';

CREATE TABLE [bx_FeedFilters] (
     [ID]               int                 IDENTITY (1, 1)                 NOT NULL 
    ,[UserID]           int                                                 NOT NULL 
    ,[FriendUserID]     int                                                 NULL
    
    ,[AppID]            uniqueidentifier                                    NOT NULL 
    
    ,[ActionType]       tinyint                                             NULL
    
    ,CONSTRAINT [PK_bx_FeedFilters] PRIMARY KEY ([ID])
);

/*
Name: 通知过滤表  不允许AppID,FriendUserID同时为null
Columns:
    [ID]               唯一标志
    [UserID]           用户ID
    [FriendUserID]     好友用户ID 为null时 过滤所有好友
    
    [AppID]            应用ID 为Guid.Empty时 过滤所有应用
    
    [ActionType]       应用的动作类型 为null时候过滤该应用的所有动作
*/
 
GO

EXEC bx_Drop 'IX_bx_FeedFilters_UserFriend';
CREATE  INDEX [IX_bx_FeedFilters_UserFriend] ON [bx_FeedFilters]([UserID],[FriendUserID]) 


EXEC bx_Drop 'IX_bx_FeedFilters_Action';
CREATE  INDEX [IX_bx_FeedFilters_Action] ON [bx_FeedFilters]([AppID],[ActionType])
GO
