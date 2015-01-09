EXEC bx_Drop 'bx_UserExtendedValues';

CREATE TABLE [bx_UserExtendedValues] (
     [UserID]            int										      NOT NULL
    ,[ExtendedFieldID]   varchar(36)        COLLATE Chinese_PRC_CI_AS_WS  NOT NULL
    
    ,[Value]             nvarchar(3950)     COLLATE Chinese_PRC_CI_AS_WS  NOT NULL
    ,[PrivacyType]		 tinyint										  NOT NULL     CONSTRAINT [DF_bx_PrivacyType]          DEFAULT (0)
    
    ,CONSTRAINT [PK_bx_UserExtendedValues] PRIMARY KEY ([UserID],[ExtendedFieldID],[PrivacyType])
);

GO


CREATE INDEX [IX_bx_UserExtendedValues] ON [bx_UserExtendedValues] ([ExtendedFieldID],[PrivacyType] DESC);

/*
Name:用户的扩展字段表
Columns:
	[UserID]              用户ID
	[ExtendedFieldID]     扩展字段ID
	[Value]               扩展字段的值
*/