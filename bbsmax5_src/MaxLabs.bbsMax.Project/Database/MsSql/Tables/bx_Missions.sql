EXEC bx_Drop 'bx_Missions';

CREATE TABLE [bx_Missions] (
     [ID]               int               IDENTITY (1, 1)                   NOT NULL 
    ,[CycleTime]        int                                                 NOT NULL    CONSTRAINT [DF_bx_Missions_CycleTime]            DEFAULT (0)
    ,[SortOrder]        int                                                 NOT NULL    CONSTRAINT [DF_bx_Missions_SortOrder]            DEFAULT (0)
    ,[TotalUsers]       int                                                 NOT NULL    CONSTRAINT [DF_bx_Missions_TotalUsers]           DEFAULT (0)
    
    ,[IsEnable]         bit                                                 NOT NULL    CONSTRAINT [DF_bx_Missions_IsEnable]             DEFAULT (1)
    
    ,[Type]             nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_Type]                 DEFAULT ('')
    ,[Name]             nvarchar(100)     COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_Name]                 DEFAULT ('')
    ,[IconUrl]          nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_IconUrl]              DEFAULT ('')
    ,[DeductPoint]      nvarchar(100)     COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_DeductPoint]          DEFAULT ('')
    
    ,[Prize]            ntext             COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_Prize]                DEFAULT ('')
    ,[Description]      ntext             COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_Description]          DEFAULT ('')
    ,[ApplyCondition]   ntext             COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_ApplyCondition]       DEFAULT ('')
    ,[FinishCondition]  ntext             COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_FinishCondition]      DEFAULT ('')
    
    ,[EndDate]          datetime                                            NULL        CONSTRAINT [DF_bx_Missions_EndTime]              DEFAULT (NULL)
    ,[BeginDate]        datetime                                            NULL        CONSTRAINT [DF_bx_Missions_StartTime]            DEFAULT (NULL)
	,[CreateDate]       datetime                                            NOT NULL    CONSTRAINT [DF_bx_Missions_CreateDate]           DEFAULT (GETDATE())
	
	
	,[CategoryID]       int                                                 null        CONSTRAINT [DF_bx_Missions_CategoryID]              DEFAULT (NULL)
	,[ParentID]         int                                                 null        CONSTRAINT [DF_bx_Missions_ParentID]                DEFAULT (NULL)
	
    ,CONSTRAINT [PK_bx_Missions] PRIMARY KEY ([ID])
);

CREATE TABLE [bx_MissionCategories] (
     [ID]       int             identity(1,1)                   not null
    ,[Name]     nvarchar(20)    collate Chinese_PRC_CI_AS_WS    not null    default('')
    
    ,constraint [PK_bx_MissionCategories] primary key ([ID])
);

/*
Name: 任务表
Columns:
    [ID]               唯一标志
    [CycleTime]        周期 单位秒  0为不是周期任务
    [SortOrder]        排序 数字越大越靠前
    [TotalUsers]       申请人数
    
    [IsEnable]		   是否启用
    
    [Type]             任务对象类名 如maxLabs.bbsMax.TopicMission (帖子类,头像类等)
    [Name]             任务名称
    [Prize]            奖励 格式：值;值
    [IconUrl]          任务图标
    [DeductPoint]      用户申请任务后扣除积分(格式: pointID:值;pointID:值)
    [Description]      任务说明
    [ApplyCondition]   申请条件 格式：值;值
    [FinishCondition]  完成条件 格式：值;值
    
    [EndDate]          任务下线时间
    [BeginDate]        任务上线时间
    [CreateDate]       任务创建时间
 
*/

GO
