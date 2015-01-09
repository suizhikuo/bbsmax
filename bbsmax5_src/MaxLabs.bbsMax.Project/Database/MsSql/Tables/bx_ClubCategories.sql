EXEC bx_Drop bx_ClubCategories;

GO

CREATE TABLE [bx_ClubCategories] (
     [CategoryID]      int             IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_ClubCategories]                 PRIMARY KEY ([CategoryID])
    ,[SortOrder]       int                                             NOT NULL    CONSTRAINT [DF_bx_ClubCategories_SortOrder]       DEFAULT (0)
    ,[TotalClubs]      int                                             NOT NULL    CONSTRAINT [DF_bx_ClubCategories_TotalBlogs]      DEFAULT (0)
    
    ,[Name]            nvarchar(50)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    
    ,[CreateDate]      datetime                                        NOT NULL    CONSTRAINT [DF_bx_ClubCategories_CreateDate]      DEFAULT (GETDATE())
)

/*
Name:群组分类
Columns:
    [CategoryID]    主键
    [TotalClubs]    该分类群组数（冗余）
    
    [Name]          分类名称
    
    [CreateDate]    添加时间
*/

GO
