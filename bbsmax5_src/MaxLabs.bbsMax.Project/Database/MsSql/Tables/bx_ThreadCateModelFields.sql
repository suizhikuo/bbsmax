EXEC bx_Drop 'bx_ThreadCateModelFields';

CREATE TABLE [bx_ThreadCateModelFields] (
	 [FieldID]              int             IDENTITY(1, 1)					NOT NULL    CONSTRAINT [PK_bx_ThreadCateModelFields]                  PRIMARY KEY ([FieldID])
    ,[ModelID]				int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_ModelID]          DEFAULT (0)
    ,[FieldName]			nvarchar(50)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_FieldName]        DEFAULT ('')
    ,[Enable]				bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_Enable]           DEFAULT (1) 
    ,[SortOrder]			int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_SortOrder]        DEFAULT (0)
	,[FieldType]			varchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_FieldType]        DEFAULT ('')
	,[Description]			nvarchar(1000)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_Description]      DEFAULT ('')
    ,[FieldTypeSetting]	    ntext		    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL	CONSTRAINT [DF_bx_ThreadCateModelFields_FieldSetting]     DEFAULT ('')
    ,[Search]				bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_Search]           DEFAULT (0) 
    ,[AdvancedSearch]		bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_AdvancedSearch]   DEFAULT (0) 
    ,[DisplayInList]		bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_DisplayInList]    DEFAULT (0) 
    ,[MustFilled]			bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_MustFilled]       DEFAULT (0)
    --,[ModelID]				int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_ModelID]          DEFAULT (0) 
);

/*
Name: 分类主题
Columns:
	[Field]		   ID
	[ModelID]	   模板ID
    [FieldName]    名称
    [Enable]       是否启用
	[SortOrder]    排序小的排在前面
	[FieldType]    字段类型
	[FieldTypeSetting]  发帖处显示内容
	[Search]	   可以默认搜索
    [AdvancedSearch]  高级搜索
    [DisplayInList]   是否在帖子列表中显示
    [MustFilled]	  是否必填 
*/

GO
