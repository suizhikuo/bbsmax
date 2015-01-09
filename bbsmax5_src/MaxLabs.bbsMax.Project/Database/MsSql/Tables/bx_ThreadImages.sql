EXEC bx_Drop 'bx_ThreadImages';

CREATE TABLE [bx_ThreadImages] (
	 [ThreadID]		int			NOT NULL    CONSTRAINT [DF_bx_ThreadImages_ThreadID]         DEFAULT (0)
	,[AttachmentID] int			NOT NULL    CONSTRAINT [DF_bx_ThreadImages_AttachmentID]     DEFAULT (0)
    ,[ImageUrl]     varchar(200)  COLLATE Chinese_PRC_CI_AS_WS NOT NULL
    ,[ImageCount]   int			NOT NULL    CONSTRAINT [DF_bx_ThreadImages_ImageCount]       DEFAULT (0)
	
    ,CONSTRAINT [PK_bx_ThreadImages] PRIMARY KEY ([ThreadID])
);


GO

