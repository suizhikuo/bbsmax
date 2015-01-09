EXEC bx_Drop 'bx_Vars';

CREATE TABLE [bx_Vars] (
	 [ID]                   int                 IDENTITY(1, 1)              NOT NULL    CONSTRAINT [PK_bx_Vars]                         PRIMARY KEY ([ID])
    ,[MaxPosts]				int                                             NOT NULL    CONSTRAINT [DF_bx_Vars_MaxPosts]                DEFAULT (0)
    ,[NewUserID]			int                                             NOT NULL    CONSTRAINT [DF_bx_Vars_NewUserID]               DEFAULT (0) 
    ,[TotalUsers]			int                                             NOT NULL    CONSTRAINT [DF_bx_Vars_TotalUsers]              DEFAULT (0)
    ,[YestodayPosts]        int												NOT NULL    CONSTRAINT [DF_bx_Vars_YestodayPosts]           DEFAULT (0)
    ,[YestodayTopics]       int												NOT NULL    CONSTRAINT [DF_bx_Vars_YestodayTopics]          DEFAULT (0)
    ,[MaxOnlineCount]       int                                             NOT NULL    CONSTRAINT [DF_bx_Vars_MaxOnlineCount]          DEFAULT (0)
    
    ,[MaxPostDate]          datetime                                        NOT NULL    CONSTRAINT [DF_bx_Vars_MaxPostDate]             DEFAULT (GETDATE())
    ,[MaxOnlineDate]        datetime                                        NOT NULL    CONSTRAINT [DF_bx_Vars_MaxOnlineDate]           DEFAULT (GETDATE())
	,[LastResetDate]        datetime                                        NOT NULL    CONSTRAINT [DF_bx_Vars_LastResetDate]           DEFAULT (GETDATE())

    ,[NewUsername]          nvarchar(50)	  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_Vars_NewUsername]             DEFAULT ('')
);

/*
Name: 通知表
Columns:
    [MaxPosts]         最大发帖数
    [NewUserID]        新用户ID
    [TotalUsers]       用户总数
	[YestodayPosts]    昨日发帖数
	[YestodayTopics]   昨日主题数
    [MaxOnlineCount]   最大在线用户数
    [MaxPostDate]      最大发帖日期
    
    [MaxOnlineDate]    最大在线时间
    [LastResetDate]    最后更新昨日发帖数时间
    
    [NewUsername]      新用户 用户名
*/

GO
