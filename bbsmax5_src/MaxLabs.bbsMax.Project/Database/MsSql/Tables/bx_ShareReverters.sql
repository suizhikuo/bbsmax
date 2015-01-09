EXEC bx_Drop bx_ShareReverters;

GO

CREATE TABLE bx_ShareReverters(
	[ShareID]				int				NOT NULL,
	[ContentReverter]		    nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL

	CONSTRAINT [PK_bx_ShareReverters] PRIMARY KEY([ShareID])
)

CREATE TABLE bx_UserShareReverters(
	[UserShareID]			int				NOT NULL,
	[SubjectReverter]		nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,
	[DescriptionReverter]	ntext                                           NOT NULL

	CONSTRAINT [PK_bx_UserShareReverters] PRIMARY KEY([UserShareID])
)

/*
Name:标签
Columns:
    [ShareID]	                  可恢复的分享ID
	[DescriptionReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO


