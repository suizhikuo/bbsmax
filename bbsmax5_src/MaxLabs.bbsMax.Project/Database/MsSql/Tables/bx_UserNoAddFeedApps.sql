EXEC bx_Drop 'bx_UserNoAddFeedApps';

CREATE TABLE [bx_UserNoAddFeedApps] (
     [AppID]        uniqueidentifier         NOT NULL
     
    ,[UserID]       int                      NOT NULL
    
    ,[ActionType]   tinyint                  NOT NULL 
    
    ,[Send]			bit						 NOT NULL
 
    ,CONSTRAINT [PK_bx_UserNoAddFeedApps] PRIMARY KEY ([UserID],[AppID],[ActionType])
);

/*
Name: 用户的该类应用动态不加入通知
Columns:
    [AppID]            应用ID
    
    [UserID]           用户ID
    
    [ActionType]       APP动作枚举值(如"评论日志" "发表日志")
    
    [Send]			   是否发送
*/

GO
