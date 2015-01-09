EXEC bx_Drop bx_Tags;

GO

CREATE TABLE [bx_Tags](
    [ID]               int              IDENTITY(1, 1)                  NOT NULL    CONSTRAINT  [PK_bx_Tags]                PRIMARY KEY ([ID])
   ,[IsLock]           bit                                              NOT NULL    CONSTRAINT  [DF_bx_Tags_IsLock]         DEFAULT(0)
   ,[Name]             nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Tags_Name]           DEFAULT('')
   ,[TotalElements]    int                                              NOT NULL    CONSTRAINT  [DF_bx_Tags_TotalElements]  DEFAULT(0)
)

/*
Name:标签
Columns:
    [ID]               自动标识ID
    [IsLock]	       是否被锁定
	[Name]             标签
	[TotalElements]    总使用数
*/

GO


--标签名称索引
CREATE INDEX [IX_bx_Tags_Name] ON [bx_Tags]([Name])

