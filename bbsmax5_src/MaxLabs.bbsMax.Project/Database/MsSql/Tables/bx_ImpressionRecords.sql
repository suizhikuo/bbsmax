EXEC bx_Drop 'bx_ImpressionRecords';

CREATE TABLE [bx_ImpressionRecords] (
	[RecordID]		    int      IDENTITY(1, 1)    NOT NULL
	
    ,[TypeID]	        int                        NOT NULL    CONSTRAINT [DF_bx_ImpressionRecords_TypeID]     DEFAULT(0)
    
    ,[UserID]           int                        NOT NULL    CONSTRAINT [DF_bx_ImpressionRecords_UserID]     DEFAULT(0)
    
    ,[TargetUserID]	    int                        NOT NULL    CONSTRAINT [DF_bx_ImpressionRecords_TargetUserID]     DEFAULT (0)
    
	,[CreateDate]       datetime                   NOT NULL    CONSTRAINT [DF_bx_ImpressionRecords_CreateDate]     DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_ImpressionRecords] PRIMARY KEY ([RecordID])
);


CREATE INDEX [IX_bx_ImpressionRecords_TargetUserID] ON [bx_ImpressionRecords]([TargetUserID]);
CREATE INDEX [IX_bx_ImpressionRecords_UserID_TargetUserID] ON [bx_ImpressionRecords]([UserID],[TargetUserID]);
