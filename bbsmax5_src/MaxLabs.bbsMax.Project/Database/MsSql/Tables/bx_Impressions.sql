EXEC bx_Drop 'bx_Impressions';

CREATE TABLE [bx_Impressions] (
    [UserID]            int      NOT NULL
 
    ,[TypeID]	        int      NOT NULL
    
	,[Count]            int      NOT NULL    CONSTRAINT [DF_bx_Impressions_Count]     DEFAULT (1)
	
	,[UpdateDate]       datetime NOT NULL    CONSTRAINT [DF_bx_Impressions_UpdateDate]		DEFAULT(GETDATE())
	
    ,CONSTRAINT [PK_bx_Impressions] PRIMARY KEY ([UserID],[TypeID])
);