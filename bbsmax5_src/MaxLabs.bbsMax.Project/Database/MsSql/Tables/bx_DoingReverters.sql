EXEC bx_Drop bx_DoingReverters;

GO

CREATE TABLE [bx_DoingReverters](
	[DoingID]				int					NOT NULL,
	[ContentReverter]		nvarchar(4000)  	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_DoingReverters] PRIMARY KEY([DoingID])
)

/*
Name:标签
Columns:
    [DoingID]	              可恢复的评论ID
	[ContentReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO


