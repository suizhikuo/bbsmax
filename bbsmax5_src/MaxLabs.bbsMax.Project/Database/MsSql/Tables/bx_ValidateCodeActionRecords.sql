EXEC bx_Drop 'bx_ValidateCodeActionRecords';

CREATE TABLE [bx_ValidateCodeActionRecords] (
	 [ID]               int              IDENTITY (1, 1)                 NOT NULL 
	 
    ,[IP]               varchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_ValidateCodeActionRecords_IP]            DEFAULT ('')
    ,[Action]           varchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_ValidateCodeActionRecords_Action]        DEFAULT ('')

	,[CreateDate]       datetime                                         NOT NULL    CONSTRAINT [DF_bx_ValidateCodeActionRecords_CreateDate]	DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_ValidateCodeActionRecords] PRIMARY KEY ([ID])
);

/*
Name: 通知表
Columns:
    [ID]               唯一标志
    [IP]			   用户IP
    [Action]		   动作

    [CreateDate]       时间
*/

GO

EXEC bx_Drop 'IX_bx_ValidateCodeActionRecords';
CREATE  INDEX [IX_bx_ValidateCodeActionRecords] ON [bx_ValidateCodeActionRecords]([IP],[Action],[CreateDate])

GO
