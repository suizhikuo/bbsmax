EXEC bx_Drop 'bx_FeedTemplates';

CREATE TABLE [bx_FeedTemplates] (
     [AppID]                uniqueidentifier                                NOT NULL
 
    ,[ActionType]           tinyint                                         NOT NULL
 
    ,[Title]                nvarchar(1000)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_FeedTemplates_Title]          DEFAULT ('')
    ,[IconUrl]              nvarchar(200)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_FeedTemplates_IconUrl]        DEFAULT ('')
    ,[Description]          nvarchar(2500)  COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_FeedTemplates_Description]    DEFAULT ('')
 
    ,CONSTRAINT [PK_bx_FeedTemplates] PRIMARY KEY ([AppID],[ActionType])
	
);

/*
Name: 通知模板表
Columns:
    [AppID]            应用ID
    
    [ActionType]       APP的动作枚举值
    
    [Title]            通知标题模板  例如：{用户}发表了日志{日志标题} --({自定义变量})
    [IconUrl]          图标URL  
    [Description]      简介模板  例如： {用户}分享日志 {日志标题}<br />来自：{日志作者} <br />{描述} <br />评论该分享
*/

GO
