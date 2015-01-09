
CREATE TABLE [bx_Emoticons] (
	 [EmoticonID]		int		IDENTITY (1, 1)							NOT NULL	CONSTRAINT [PK_bx_Emoticons] PRIMARY KEY  ([EmoticonID])
	,[UserID]			int												NOT NULL	CONSTRAINT [DF_bx_Emoticons_UserID]			DEFAULT(0)
	,[GroupID]			int												NOT NULL	CONSTRAINT [DF_bx_Emoticons_GroupID]		DEFAULT(0)
	,[Shortcut]			nvarchar(100)	COLLATE Chinese_PRC_CI_AS_WS	NULL		CONSTRAINT [DF_bx_Emoticons_Shortcut]		DEFAULT('')
	,[ImageSrc]			varchar(255)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL
	,[FileSize]			int												NOT NULL	CONSTRAINT [DF_bx_Emoticons_FileSize]		DEFAULT(0)
	,[MD5]				varchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NULL	
	,[SortOrder]		int												NOT NULL	CONSTRAINT [DF_bx_Emoticons_SortOrder]		DEFAULT(0)
)

GO

--同一个用户只能创建一个同样的表情
CREATE NONCLUSTERED INDEX [IX_bx_Emoticons_List] ON [bx_Emoticons] ([UserID], [ImageSrc])

--分组索引
CREATE INDEX [IX_bx_Emoticons_GroupID] ON [bx_Emoticons] ([GroupID])