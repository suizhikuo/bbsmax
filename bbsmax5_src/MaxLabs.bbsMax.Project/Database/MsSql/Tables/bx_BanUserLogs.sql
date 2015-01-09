EXEC bx_Drop 'bx_BanUserLogs';

CREATE TABLE bx_BanUserLogs(
	[LogID]				int			IDENTITY(1,1)			NOT NULL			CONSTRAINT	[PK_bx_BanUserLogs]					PRIMARY KEY ([LogID])
  
   ,[OperationType]		tinyint								NOT NULL			CONSTRAINT	[DF_bx_BanUserLogs_OperationType]	DEFAULT(0)
  
   ,[OperationTime]     datetime							NOT NULL			CONSTRAINT  [DF_bx_BanUserLogs_OperationTime]	DEFAULT(GETDATE())
   
   ,[OperatorName]		nvarchar(50)						NOT NULL			CONSTRAINT  [DF_bx_BanUserLogs_OperationName]	DEFAULT(N'')
   
   ,[Cause]				nvarchar(1000)						NOT NULL			CONSTRAINT	[DF_bx_BanUserLogs_Cause]			DEFAULT(N'')
      
   ,[UserID]			int									NOT NULL			CONSTRAINT	[DF_bx_BanUserLogs_UserID]			DEFAULT(0)

   ,[Username]			nvarchar(50)						NOT NULL			CONSTRAINT	[DF_bx_BanUserLogs_Username]		DEFAULT(N'')
   
   ,[UserIP]			varchar(50)							NOT NULL			CONSTRAINT  [DF_bx_BanUserLogs_UserIP]			DEFAULT('')
   
   ,[AllBanEndDate]		datetime							
);

GO

CREATE NONCLUSTERED INDEX [IX_bx_BanUserLogs_UserID] ON [bx_BanUserLogs]([UserID]);

GO