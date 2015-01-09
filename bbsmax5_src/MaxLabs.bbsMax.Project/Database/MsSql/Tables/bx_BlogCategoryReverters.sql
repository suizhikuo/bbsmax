EXEC bx_Drop bx_BlogCategoryReverters;

GO

CREATE TABLE [bx_BlogCategoryReverters](
	[CategoryID]		    int				NOT NULL,
	[NameReverter]			nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_BlogCategoryReverters] PRIMARY KEY([CategoryID])
)

/*
Name:标签
Columns:
    [AlbumID]		       可恢复的相册ID
	[NameReverter]         相册名复原关键信息，可根据此信息恢复相册名的原始内容
*/

GO