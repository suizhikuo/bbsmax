EXEC bx_Drop 'bx_SmsCodes';

GO

CREATE TABLE [bx_SmsCodes] (
	[SmsCodeID]             int      IDENTITY(1, 1)                  NOT NULL         CONSTRAINT [PK_bx_SmsCodes]                      PRIMARY KEY ([SmsCodeID])
	,[UserID]				int                                      NOT NULL
	,[IsUsed]				bit										 NOT NULL		  CONSTRAINT [DF_bx_SmsCodes_IsUsed]               DEFAULT(0)
	,[ActionType]           tinyint                                  NOT NULL
	,[CreateDate]			datetime                                 NOT NULL         CONSTRAINT [DF_bx_SmsCodes_CreatDate]			   DEFAULT (GETDATE())
	,[ExpiresDate]			datetime                                 NOT NULL
	,[MobilePhone]			bigint                                   NOT NULL
	,[SmsCode]				varchar(10)								 NOT NULL
	,[ChangedMobilePhone]   bigint									 NOT NULL		  CONSTRAINT [DF_bx_SmsCodes_ChangedMobilePhone]   DEFAULT(0)
	,[ChangedSmsCode]		varchar(10)								 NOT NULL		  CONSTRAINT [DF_bx_SmsCodes_ChangedSmsCode]       DEFAULT('')
)

CREATE INDEX [IX_bx_SmsCodes_Key] ON [bx_SmsCodes] ([UserID]);

CREATE INDEX [IX_bx_SmsCodes_ExpiresDate] ON [bx_SmsCodes] ([ExpiresDate]);

--CREATE INDEX [IX_bx_SmsCodes_] ON [bx_SmsCodes] ([UserID], [ExpiresDate]);

GO

