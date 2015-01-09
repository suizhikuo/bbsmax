EXEC bx_Drop bx_UserReverters;

GO

CREATE TABLE bx_UserReverters(
	[UserID]					int				NOT NULL,
	[SignatureReverter]			nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_UserReverters] PRIMARY KEY([UserID])
)

/*
Name:标签
Columns:
    [UserID]	                  ID
	[SignatureReverter]           内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO


