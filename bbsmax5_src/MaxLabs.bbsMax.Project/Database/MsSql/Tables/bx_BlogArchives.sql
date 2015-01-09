EXEC bx_Drop bx_BlogArchives;

CREATE TABLE bx_BlogArchives(
     [Year]            int                                              NOT NULL
    ,[Month]           int                                              NOT NULL
    ,[UserID]          int                                              NOT NULL 
    ,[TotalBlogs]      int                                              NOT NULL    CONSTRAINT [DF_bx_BlogArchives_TotalBlogs]    DEFAULT (0)
    
    ,CONSTRAINT [PK_bx_BlogArchives] PRIMARY KEY ([UserID], [Year], [Month])
)

/*
Name:日志存档
Columns:
    [Year]           存档的年份
    [Month]          存档的月份
    [UserID]         存档属于者的用户ID
    [TotalBlogs]     该月存档的文章数
*/

GO
