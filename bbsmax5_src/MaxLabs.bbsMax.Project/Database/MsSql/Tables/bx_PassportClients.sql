EXEC bx_Drop 'bx_PassportClients';

CREATE TABLE bx_PassportClients (
 
	 [ClientID]          int		    IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_PassportClients]                 PRIMARY KEY ([ClientID])
	,[ClientName]        nvarchar(50)                                   NOT NULL
	,[AccessKey]         nvarchar(50)									NULL
	,[Url]               nvarchar(1000)                                 NOT NULL
	,[APIFilePath]       nvarchar(200)                                  NOT NULL
	,[CreateDate]        datetime                                       NOT NULL    CONSTRAINT  [DF_bx_PassportClients_CreateDate]		DEFAULT(GETDATE())
	,[Deleted]           bit											NOT NULL	CONSTRAINT  [DF_bx_PassportClients_Deleted]			DEFAULT(0)
	,[InstructTypes]     text			COLLATE Chinese_PRC_CI_AS_WS		NULL
)