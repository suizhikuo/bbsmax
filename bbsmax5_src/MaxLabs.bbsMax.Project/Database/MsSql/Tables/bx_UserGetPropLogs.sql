EXEC bx_Drop 'bx_UserGetPropLogs';

CREATE TABLE [bx_UserGetPropLogs](
	 LogID						int							IDENTITY(1,1)				CONSTRAINT [PK_bx_UserGetPropLogs_LogID]		PRIMARY KEY ([LogID])
	,UserID						int							NOT NULL
	,Username					nvarchar(50)				NOT NULL
	,GetPropType				tinyint						NOT NULL
	,PropID						int							NOT NULL
	,PropName					nvarchar(50)				NOT NULL
	,PropCount					int							NOT NULL					CONSTRAINT [DF_bx_UserGetPropLogs_PropCount]	DEFAULT(0)
	,CreateDate					datetime					NOT NULL					CONSTRAINT [DF_bx_UserGetPropLogs_CreateDate]	DEFAULT(getdate())
)