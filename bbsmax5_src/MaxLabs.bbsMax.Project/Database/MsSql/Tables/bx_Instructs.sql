 EXEC bx_Drop 'bx_Instructs';

CREATE TABLE [bx_Instructs] (
		[InstructID]        bigint               IDENTITY(1, 1)               NOT NULL    CONSTRAINT  [PK_bx_Instructs]                    PRIMARY KEY ([InstructID])
		,[TargetID]         int                                               NOT NULL     
		,[ClientID]         int                                               NOT NULL    
		,[InstructType]     int						      NOT NULL 
		,[CreateDate]       datetime                                          NOT NULL    CONSTRAINT  [DF_bx_Instructs_CreateDate]         DEFAULT (GETDATE())
		,[Datas]            ntext             COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Instructs_Content]			   DEFAULT ('')
);  

CREATE INDEX [IX_bx_Instructs_ClientID]  ON [bx_Instructs]([ClientID]);