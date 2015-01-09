EXEC bx_Drop 'bx_UserMissions';

CREATE TABLE [bx_UserMissions] (
	 [ID]				int                 IDENTITY (1, 1)                 NOT NULL 
    ,[UserID]           int                                                 NOT NULL
    ,[MissionID]        int                                                 NOT NULL
    
    ,[FinishPercent]    float                                               NOT NULL    CONSTRAINT [DF_bx_UserMissions_FinishPercent]  DEFAULT (0)
    
    ,[Status]           tinyint                                             NOT NULL    CONSTRAINT [DF_bx_UserMissions_Status]         DEFAULT (0)
    
    ,[IsPrized]         bit                                                 NOT NULL    CONSTRAINT [DF_bx_UserMissions_IsPrized]       DEFAULT (0)

    ,[FinishDate]       datetime                                            NOT NULL    CONSTRAINT [DF_bx_UserMissions_FinishDate]     DEFAULT (GETDATE())
    ,[CreateDate]       datetime                                            NOT NULL    CONSTRAINT [DF_bx_UserMissions_CreateDate]     DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_UserMissions] PRIMARY KEY ([UserID],[MissionID])
);

/*
Name: 用户任务表
Columns:
    [ID]               
    [UserID]           用户ID
    [MissionID]        任务ID
    
    [FinishPercent]    完成百分比
    
    [Status]           任务状态: 0进行中 1完成 2超时未完成 3放弃任务
    
    [IsPrized]         是否领取过奖励了
    
    [FinishDate]       任务完成时间
    [CreateDate]       申请任务时间
*/

GO


EXEC bx_Drop 'IX_bx_UserMissions_CreateDate';
CREATE  INDEX [IX_bx_UserMissions_CreateDate] ON [bx_UserMissions]([CreateDate] DESC)

EXEC bx_Drop 'IX_bx_UserMissions_ID';
CREATE  UNIQUE  INDEX [IX_bx_UserMissions_ID] ON [bx_UserMissions]([ID])

GO
