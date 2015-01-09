--EXEC bx_Drop 'bx_Medals';

--CREATE TABLE [bx_Medals] (
     --[ID]                int              IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_Medals]                 PRIMARY KEY ([ID])
 
    --,[Name]              nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    --,[IconUrl]           nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NULL
    
    --,[IsEnabled]         bit                                              NOT NULL    CONSTRAINT [DF_bx_Medals_IsEnabled]       DEFAULT (1)        
--);

--GO

--/*
--Name:勋章表
--Columns:
	--[Name]              勋章名称
	--[IconUrl]           图标
	--[IsEnbaled]         是否启用
--*/