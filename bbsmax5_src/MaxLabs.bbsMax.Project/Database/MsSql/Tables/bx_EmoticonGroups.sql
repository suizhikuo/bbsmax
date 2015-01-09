CREATE TABLE [bx_EmoticonGroups] (
	 [GroupID]			int					IDENTITY (1, 1)					NOT NULL		 CONSTRAINT  [PK_bx_EmoticonGroups]					PRIMARY KEY ([GroupID])
	,[GroupName]		nvarchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL		 
	,[UserID]			int													NOT NULL		 
	,[TotalEmoticons]	int													NOT NULL		CONSTRAINT  [DF_bx_EmoticonGroups_TotalEmoticons]	DEFAULT(0)
	,[TotalSizes]		int													NOT NULL		CONSTRAINT  [DF_bx_EmoticonGroups_TotalSizes]		DEFAULT(0)
) 

GO

--分组索引
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_EmoticonGroups_UserID] ON [bx_EmoticonGroups]([UserID],[GroupName])
