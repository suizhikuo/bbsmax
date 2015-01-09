EXEC bx_Drop 'bx_IPLogs';

CREATE TABLE bx_IPLogs(
     [LogID]                 int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_IPLogs]					PRIMARY KEY ([LogID])
    ,[UserID]             int                                               NOT NULL    CONSTRAINT [DF_bx_IPLogs_UserID]			DEFAULT(0)
  
    ,[Username]           nvarchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_IPLogs_Username]			DEFAULT(N'')
    
    ,[NewIP]              varchar(50)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_IPLogs_NewIP]			DEFAULT('')
    
    ,[CreateDate]         datetime                                          NOT NULL    CONSTRAINT [DF_bx_IPLogs_CreateDate]		DEFAULT(GETDATE())
    
    ,[OldIP]              varchar(50)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_IPLogs_OldIP]             DEFAULT('')
    
    ,[VisitUrl]           varchar(200)                                      NOT NULL    CONSTRAINT [DF_bx_IPLogs_VisitUrl]          DEFAULT('')
)

GO 

CREATE INDEX [IX_bx_IPlogs_NewIP] ON [bx_IPLogs]([NewIP]);

/*
Name:用户IP变更日志
Columns:
    [ID]            
    [UserID]          用户ID
    
    [Username]        用户名
    
    [IPAddress]       IP地址
    
    [CreateDate]      时间
*/

GO
