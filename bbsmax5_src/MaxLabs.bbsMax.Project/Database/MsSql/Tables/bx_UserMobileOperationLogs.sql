EXEC bx_Drop 'bx_UserMobileOperationLogs';

CREATE TABLE [bx_UserMobileOperationLogs](
	LogID					int					NOT NULL		IDENTITY(1,1)			CONSTRAINT [PK_bx_UserMobileOperationLogs_LogID]			PRIMARY KEY ([LogID])
	
	,UserID					int					NOT NULL
	
	,Username				nvarchar(50)		NOT NULL
	
	,MobilePhone			bigint				NOT NULL								CONSTRAINT [DF_bx_UserMobileOperationLogs_MobilePhone]		DEFAULT(0)
	
	,OperationType			tinyint				NOT NULL								CONSTRAINT [DF_bx_UserMobileOperationLogs_OperationType]	DEFAULT(0)
	
	,OperationDate			datetime			NOT NULL								CONSTRAINT [DF_bx_UserMobileOperationLogs_OperationDate]	DEFAULT(GETDATE())						

)