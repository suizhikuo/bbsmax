EXEC bx_Drop 'bx_ImpressionTypes';

CREATE TABLE [bx_ImpressionTypes] (
	 [TypeID]		    int                 IDENTITY (1, 1)                 NOT NULL

    ,[Text]		        nvarchar(100)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_ImpressionTypes_Text]				DEFAULT (N'')
    
    ,[RecordCount]      int                                                 NOT NULL    CONSTRAINT  [DF_bx_ImpressionTypes_RecordCount]			DEFAULT (1)
    
    ,[KeywordVersion]   varchar(32)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_ImpressionTypes_KeywordVersion]		DEFAULT ('')
    
    ,CONSTRAINT [PK_bx_ImpressionTypes] PRIMARY KEY ([TypeID])
);
