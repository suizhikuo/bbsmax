EXEC bx_Drop 'bx_ThreadCates';

CREATE TABLE [bx_ThreadCates] (
	 [CateID]               int                 IDENTITY(1, 1)              NOT NULL    CONSTRAINT [PK_bx_ThreadCates]                  PRIMARY KEY ([CateID])
    ,[CateName]				nvarchar(50)	  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_ThreadCates_CateName]         DEFAULT ('')
    ,[Enable]				bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCates_Enable]           DEFAULT (1) 
    ,[SortOrder]			int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCates_SortOrder]        DEFAULT (0)
);

/*
Name: 分类主题
Columns:
    [CateID]       ID
    [CateName]     名称
    [Enable]       是否启用
	[SortOrder]    排序小的排在前面
    
*/

GO
