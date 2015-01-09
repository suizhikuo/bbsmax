EXEC bx_Drop 'bx_OperationLogs';

CREATE TABLE bx_OperationLogs(
     [LogID]            int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_OperationLogs]               PRIMARY KEY ([LogID])
    ,[OperatorID]       int                                               NOT NULL
    ,[TargetID_1]       int                                               NOT NULL    CONSTRAINT [DF_bx_OperationLogs_TargetID_1]    DEFAULT (0)
    ,[TargetID_2]       int                                               NOT NULL    CONSTRAINT [DF_bx_OperationLogs_TargetID_2]    DEFAULT (0)
    ,[TargetID_3]       int                                               NOT NULL    CONSTRAINT [DF_bx_OperationLogs_TargetID_3]    DEFAULT (0)
    
    ,[OperatorIP]       varchar(50)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    ,[OperationType]    varchar(100)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    ,[Message]          nvarchar(1000)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    
    ,[CreateTime]       datetime                                          NOT NULL
)

GO

EXEC bx_Drop 'IX_bx_OperationLogs_OperatorID';
CREATE  INDEX [IX_bx_OperationLogs_OperatorID] ON [bx_OperationLogs]([OperatorID]);

EXEC bx_Drop 'IX_bx_OperationLogs_OperatorIP';
CREATE  INDEX [IX_bx_OperationLogs_OperatorIP] ON [bx_OperationLogs]([OperatorIP]);

EXEC bx_Drop 'IX_bx_OperationLogs_OperationType';
CREATE  INDEX [IX_bx_OperationLogs_OperationType] ON [bx_OperationLogs]([OperationType]);

EXEC bx_Drop 'IX_bx_OperationLogs_CreateTime';
CREATE  INDEX [IX_bx_OperationLogs_CreateTime] ON [bx_OperationLogs]([CreateTime]);

GO