EXEC bx_Drop 'bx_ThreadCateModels';

CREATE TABLE [bx_ThreadCateModels] (
	 [ModelID]              int             IDENTITY(1, 1)					NOT NULL    CONSTRAINT [PK_bx_ThreadCateModels]                  PRIMARY KEY ([ModelID])
    ,[CateID]				int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModels_CateID]           DEFAULT (0)
    ,[ModelName]			nvarchar(50)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_ThreadCateModels_ModelName]        DEFAULT ('')
    ,[Enable]				bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModels_Enable]           DEFAULT (1) 
    ,[SortOrder]			int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModels_SortOrder]        DEFAULT (0) 
);

/*
Name: 分类主题
Columns:
	[ModeID]	   ID
    [CateID]       分类主题ID
    [ModelName]     名称
    [Enable]       是否启用
	[SortOrder]    排序小的排在前面
*/

GO
