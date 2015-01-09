EXEC bx_Drop 'bx_DenouncingContents';

CREATE TABLE bx_DenouncingContents(
     [DenouncingID]       int                                               NOT NULL
    ,[UserID]             int                                               NOT NULL

    ,[Content]            nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    
    ,[CreateDate]         datetime                                          NOT NULL    CONSTRAINT [DF_bx_DenouncingContents_CreateDate]      DEFAULT (GETDATE())

    ,CONSTRAINT [PK_bx_DenouncingContents] PRIMARY KEY ([DenouncingID],[UserID])
)

GO

