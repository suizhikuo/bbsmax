EXEC bx_Drop bx_ImpressionTypeReverters;

GO

CREATE TABLE [bx_ImpressionTypeReverters](
	[TypeID]		    int				NOT NULL,
	[TextReverter]	    nvarchar(1000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_ImpressionTypeRevertersReverters] PRIMARY KEY([TypeID])
)

GO