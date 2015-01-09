--系统通知表
EXEC bx_Drop bx_SystemNotifies;
GO
CREATE TABLE [bx_SystemNotifies] (
     [NotifyID]             int                    IDENTITY(1,1)                  NOT NULL   
	,[BeginDate]            datetime                                              NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_BeginDate]       DEFAULT(GETDATE())
	,[Subject]              nvarchar(200)      COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_Subject]		 DEFAULT(N'')
	,[EndDate]              datetime											  NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_EndDate]         DEFAULT('2099-1-1')
	,[ReceiveRoles]         text               COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_ReceiveRoles]    DEFAULT('')
	,[ReceiveUserIDs]       varchar(2000)      COLLATE Chinese_PRC_CI_AS_WS		  NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_ReceiveUserIDs]  DEFAULT('')
	,[Content]				nvarchar(1000)	   COLLATE Chinese_PRC_CI_AS_WS	      NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_Content]		 DEFAULT(N'')	
    ,[DispatcherID]         int													  NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_DispatcherID]    DEFAULT(0)
    ,[DispatcherIP]         varchar(200)       COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_DispatcherIP]    DEFAULT('')
    ,[CreateDate]           datetime											  NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_CreateDate]      DEFAULT(GETDATE())
    ,[ReadUserIDs]          text               COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_ReadUserIDs]     DEFAULT(',')  
	,CONSTRAINT [PK_bx_SystemNotifies] PRIMARY KEY ([NotifyID])
);
GO