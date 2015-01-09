EXEC bx_Drop 'bx_JobStatus';

CREATE TABLE [bx_JobStatus] (	 
     [Type]             varchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_JobStatus_Type]            DEFAULT ('')

	,[LastExecuteTime]  datetime                                         NOT NULL    CONSTRAINT [DF_bx_JobStatus_LastExecuteTime]	DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_JobStatus] PRIMARY KEY ([Type])
);

/*
Name: JOB执行时间表
Columns:
    [Type]					任务类型

    [LastExecuteTime]       上次执行时间
*/

GO

