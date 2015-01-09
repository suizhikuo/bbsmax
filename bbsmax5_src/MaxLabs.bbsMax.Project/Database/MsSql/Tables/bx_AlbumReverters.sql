EXEC bx_Drop bx_AlbumReverters;

GO

CREATE TABLE [bx_AlbumReverters](
	[AlbumID]				int				NOT NULL,
	[NameReverter]			nvarchar(1500)	COLLATE Chinese_PRC_CI_AS_WS  NOT NULL,
	[DescriptionReverter]   nvarchar(2500)	COLLATE Chinese_PRC_CI_AS_WS  NULL,

	CONSTRAINT [PK_bx_AlbumReverters] PRIMARY KEY([AlbumID])
)

/*
Name:标签
Columns:
    [AlbumID]		       可恢复的相册ID
	[NameReverter]         相册名复原关键信息，可根据此信息恢复相册名的原始内容
*/

GO