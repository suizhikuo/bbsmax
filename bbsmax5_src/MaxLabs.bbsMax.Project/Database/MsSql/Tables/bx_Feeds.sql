EXEC bx_Drop 'bx_Feeds';

CREATE TABLE [bx_Feeds] (
     [ID]               int                 IDENTITY (1, 1)                 NOT NULL 
    ,[TargetID]         int                                                 NULL
    ,[TargetUserID]     int                                                 NOT NULL 
    ,[CommentTargetID]  int                                                 NOT NULL    CONSTRAINT [DF_bx_Feeds_CommentTargetID]   DEFAULT (0)

    ,[ActionType]       tinyint                                             NOT NULL 
    ,[PrivacyType]      tinyint                                             NOT NULL 
    
    ,[AppID]            uniqueidentifier                                    NOT NULL 

    ,[Title]            nvarchar(1000)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Feeds_Title]          DEFAULT ('')
	,[Description]      nvarchar(2500)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Feeds_Description]    DEFAULT ('')
    ,[TargetNickname]   nvarchar(50)        COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Feeds_TargetNickname] DEFAULT ('')
	
	,[VisibleUserIDs]   varchar(800)        COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Feeds_VisibleUserIDs] DEFAULT ('')
	,[CommentInfo]		varchar(50)         COLLATE Chinese_PRC_CI_AS_WS    NULL		--CONSTRAINT [DF_bx_Feeds_CommentInfo]	DEFAULT ('')
	
	,[CreateDate]       datetime                                            NOT NULL    CONSTRAINT [DF_bx_Feeds_CreateDate]     DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_Feeds] PRIMARY KEY ([ID])
);

/*
Name: 通知表
Columns:
    [ID]               唯一标志
    [TargetID]         通用的目标ID（有需要时使用 例如:相册上传图片，就记相册ID 方便处理上传多张图片时只记一个通知）
    [TargetUserID]     相关联的目标UserID,如aa评论了bb的日志，就是bb的userID;cc和dd成为好友,就是dd的userID
 
    [ActionType]       APP的动作枚举值
    [PrivacyType]      隐私类型
    
    [AppID]            应用ID
    
    [Title]            通知标题
    [Description]      通知简介
    
    [CreateDate]       时间
*/

GO

EXEC bx_Drop 'IX_bx_Feeds_TargetUserID';
CREATE  INDEX [IX_bx_Feeds_TargetUserID] ON [bx_Feeds]([TargetUserID])

EXEC bx_Drop 'IX_bx_Feeds_Action';
CREATE  INDEX [IX_bx_Feeds_Action] ON [bx_Feeds]([AppID],[ActionType])

EXEC bx_Drop 'IX_bx_Feeds_TargetID';
CREATE  INDEX [IX_bx_Feeds_TargetID] ON [bx_Feeds]([TargetID])

EXEC bx_Drop 'IX_bx_Feeds_CreateDate';
CREATE  INDEX [IX_bx_Feeds_CreateDate] ON [bx_Feeds]([CreateDate])

GO
