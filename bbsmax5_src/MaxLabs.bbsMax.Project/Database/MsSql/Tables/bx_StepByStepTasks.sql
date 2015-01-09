EXEC bx_Drop 'bx_StepByStepTasks';

CREATE TABLE [bx_StepByStepTasks] (
     [TaskID]           uniqueidentifier                                 NOT NULL
    ,[Type]             varchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_Type]             DEFAULT ('')
	,[UserID]			int												 NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_UserID]           DEFAULT (0)
	,[Param]			nvarchar(3500)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_Param]            DEFAULT ('')
	,[TotalCount]		int												 NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_TotalCount]       DEFAULT (0)
	,[FinishedCount]	int												 NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_FinishedCount]    DEFAULT (0)
	,[Offset]			bigint											 NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_Offset]           DEFAULT (0)
	,[Title]			nvarchar(100)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_Title]            DEFAULT ('')
	,[LastExecuteTime]  datetime                                         NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_LastExecuteTime]	 DEFAULT (GETDATE())
	,[InstanceMode]     tinyint											 NOT NULL

    ,CONSTRAINT [PK_bx_StepByStepTasks] PRIMARY KEY ([TaskID])
);

/*
Name: JOB执行时间表
Columns:
    [Type]					任务类型

    [LastExecuteTime]       上次执行时间
*/

GO

