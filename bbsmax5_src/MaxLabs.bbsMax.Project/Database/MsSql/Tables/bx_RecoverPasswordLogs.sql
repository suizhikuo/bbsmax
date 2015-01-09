CREATE TABLE [dbo].[bx_RecoverPasswordLogs](
	[Id]			int  IDENTITY(1,1) NOT NULL,
	[UserID]		int NOT NULL,
	[CreateDate]	datetime NULL		CONSTRAINT [DF_bx_RecoverPasswordLogs_CreateDate]	DEFAULT (getdate()),
	[Successed]		bit NULL			CONSTRAINT [DF_bx_RecoverPasswordLogs_Successed]	DEFAULT (0),
	[Email]			nvarchar(200) NULL,
	[Serial]		varchar(100) NULL,
	[IP]			varchar(150) NOT NULL,
	CONSTRAINT [PK_bx_RecoverPasswordLogs] PRIMARY KEY ([Id] ASC)
);
GO
