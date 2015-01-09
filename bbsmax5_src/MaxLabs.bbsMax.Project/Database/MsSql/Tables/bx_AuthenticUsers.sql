

CREATE TABLE [bx_AuthenticUsers](
	[UserID]		int				NOT NULL,
	[Realname]		nvarchar(50)	NOT NULL,
	[Gender]		tinyint			NOT NULL,
	[Birthday]		datetime		NOT NULL,
	[IDNumber]		varchar(50)		NOT NULL,
	[IDCardFileFace]	nvarchar(100)	NOT NULL,
	[IDCardFileBack] nvarchar(100)  NOT NULL,
	[Verified]      bit				NOT NULL			CONSTRAINT  [DF_bx_AuthenticUsers_Verified]         DEFAULT (0),
	[Area]			nvarchar(100)	NULL,
	[CreateDate]	datetime		NOT NULL			CONSTRAINT  [DF_bx_AuthenticUsers_CreateDate]         DEFAULT (GETDATE()),
	[Photo]			nvarchar(100)   NULL,
	[Processed]     bit             NOT NULL			CONSTRAINT  [DF_bx_AuthenticUsers_Processed]         DEFAULT (0),
	[OperatorUserID] int            NULL,
	[Remark]		nvarchar(1000)	NULL,
	[DetectedState] int				NULL,
	[IsDetect]		bit				NOT NULL			CONSTRAINT	[DF_bx_AuthenticUsers_IsDetect]			DEFAULT(0),
 CONSTRAINT [PK_bx_AuthenticUsers] PRIMARY KEY CLUSTERED ([UserID] ASC)
)

GO

CREATE INDEX [IX_bx_AuthenticUsers_IDNumber] ON [bx_AuthenticUsers]([IDNumber]);

GO
