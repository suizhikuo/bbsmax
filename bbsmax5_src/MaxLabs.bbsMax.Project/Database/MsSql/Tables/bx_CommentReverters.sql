EXEC bx_Drop bx_CommentReverters;

GO

CREATE TABLE [bx_CommentReverters](
	[CommentID]				int				NOT NULL,
	[ContentReverter]		ntext	COLLATE Chinese_PRC_CI_AS_WS		NOT NULL,

	CONSTRAINT [PK_bx_CommentReverters] PRIMARY KEY([CommentID])
)

/*
Name:标签
Columns:
    [CommentID]	              可恢复的评论ID
	[ContentReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO


