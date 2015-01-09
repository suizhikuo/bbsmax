EXEC bx_Drop 'bx_UserMedals';

CREATE TABLE [bx_UserMedals] (
     [UserID]            int										      NOT NULL
    ,[MedalID]           int										      NOT NULL 
    ,[MedalLevelID]      int											  NOT NULL
    ,[Url]               nvarchar(200)	  COLLATE Chinese_PRC_CI_AS_WS    NULL    
    ,[EndDate]           datetime                                         NULL
    ,[CreateDate]        datetime                                         NOT NULL    CONSTRAINT [DF_bx_UserMedals_CreateDate]      DEFAULT (GETDATE())
    
	,CONSTRAINT [PK_bx_UserMedals] PRIMARY KEY ([UserID],[MedalID],[MedalLevelID])
);

GO

CREATE NONCLUSTERED INDEX [IX_bx_UserMedals_EndDate] ON [bx_UserMedals] ([EndDate]);

GO
/*
Name:用户的勋章表
Columns:
	[UserID]              用户ID
	[MedalID]             勋章	
	[MedalID]             勋章等级ID
	
	[CreateDate]          创建时间
	[EndDate]             过期时间
*/