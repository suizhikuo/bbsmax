EXEC bx_Drop bx_ChatMessageReverters;

GO

CREATE TABLE bx_ChatMessageReverters(
	[MessageID]					int												NOT NULL,
	[ContentReverter]			ntext           COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_ChatMessageReverters] PRIMARY KEY([MessageID])
)

/*
Name:标签
Columns:
    [MessageID]	                  可恢复的消息ID
	[ContentReverter]             内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO


