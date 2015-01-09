EXEC bx_Drop 'bx_PolemizeUsers';

CREATE TABLE [bx_PolemizeUsers](
	[ThreadID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[ViewPointType] [tinyint] NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_PolemizeUsers_CreateDate]  DEFAULT (getdate()),
	CONSTRAINT [PK_bx_PolemizeUsers] PRIMARY KEY CLUSTERED  ([ThreadID] ASC,[UserID] ASC)
) 

GO
