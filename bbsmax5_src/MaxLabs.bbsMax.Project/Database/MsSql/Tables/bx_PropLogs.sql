CREATE TABLE bx_PropLogs (
   PropLogID  int		IDENTITY(1,1)					NOT NULL CONSTRAINT [PK_bx_PropLogs]            PRIMARY KEY (PropLogID)
  ,UserID     int										NOT NULL CONSTRAINT [DF_bx_PropLogs_UserID]		DEFAULT(0)
  ,Type       tinyint									NOT NULL CONSTRAINT [DF_bx_PropLogs_Type]		DEFAULT(0)
  ,[Log]      ntext		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL CONSTRAINT [DF_bx_PropLogs_Log]		DEFAULT('')
  ,CreateDate datetime									NOT NULL CONSTRAINT [DF_bx_PropLogs_CreateDate]	DEFAULT(GETDATE())
);

CREATE INDEX [IX_bx_PropLogs_UserID_Type] ON bx_PropLogs (UserID, Type);