EXEC bx_Drop bx_PostReverters;

GO

CREATE TABLE [bx_PostReverters](
	[PostID]				int				NOT NULL,
	[SubjectReverter]		nvarchar(4000)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,
	[ContentReverter]		ntext			COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_PostReverters] PRIMARY KEY([PostID])
)

/*
Name:标签
Columns:
    [ArticleID]	       可恢复的博客文章ID
	[SubjectReverter]         标题复原关键信息，可根据此信息恢复标题的原始内容
	[ContentReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO


