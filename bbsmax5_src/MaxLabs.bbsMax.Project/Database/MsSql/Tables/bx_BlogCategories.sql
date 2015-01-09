EXEC bx_Drop bx_BlogCategories;

GO

CREATE TABLE [bx_BlogCategories] (
     [CategoryID]      int             IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_BlogCategories]                 PRIMARY KEY ([CategoryID])
    ,[UserID]          int                                             NOT NULL 
    ,[TotalArticles]   int                                             NOT NULL    CONSTRAINT [DF_bx_BlogCategories_TotalBlogs]      DEFAULT (0)
    
    ,[Name]            nvarchar(50)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_BlogCategories_Name]            DEFAULT ('')
    
    ,[CreateDate]      datetime                                        NOT NULL    CONSTRAINT [DF_bx_BlogCategories_CreateDate]      DEFAULT (GETDATE())
    ,[KeywordVersion]  varchar(32)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_BlogCategories_KeywordVersion]  DEFAULT ('')
)

/*
Name:日志分类
Columns:
    [CategoryID]            自动标识
    [UserID]        用户ID
    [TotalArticles] 该分类文章数
    
    [Name]          日志分类名
    
    [CreateDate]    添加时间
*/

GO


--创建日志分类的用户ID索引
CREATE INDEX [IX_bx_BlogCategories_UserID] ON [bx_BlogCategories]([UserID]) 
