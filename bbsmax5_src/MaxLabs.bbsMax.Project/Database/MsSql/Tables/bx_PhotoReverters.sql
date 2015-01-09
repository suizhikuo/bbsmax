EXEC bx_Drop bx_PhotoReverters;

GO

CREATE TABLE bx_PhotoReverters(
	[PhotoID]				int				NOT NULL,
	[NameReverter]			nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,
	[DescriptionReverter]	ntext			COLLATE Chinese_PRC_CI_AS_WS    NOT NULL,

	CONSTRAINT [PK_bx_PhotoReverters] PRIMARY KEY([PhotoID])
)

/*
Name:标签
Columns:
    [PhotoID]			      可恢复的博客文章ID
	[NameReverter]         标题复原关键信息，可根据此信息恢复标题的原始内容
	[ContentReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO


