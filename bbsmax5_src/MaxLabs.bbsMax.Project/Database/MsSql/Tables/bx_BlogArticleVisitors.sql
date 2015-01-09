EXEC bx_Drop bx_BlogArticleVisitors;


CREATE TABLE [bx_BlogArticleVisitors] (
       [ID]                       int        IDENTITY(1,1)       NOT NULL   
      ,[BlogArticleID]            int                            NOT NULL
      ,[UserID]                   int                            NOT NULL
      
      ,[ViewDate]                 datetime                       NOT NULL    CONSTRAINT [DF_bx_bx_BlogArticleVisitors_ViewDate]    DEFAULT(GETDATE())
      
      ,CONSTRAINT [PK_bx_BlogArticleVisitors] PRIMARY KEY([ID])
)


/*
Name: 日志访问表, 同一篇日志
Column:
	  [ID]
      [BlogArticleID]            日志ID
      [UserID]                   访问该日志的用户ID
      
      [ViewDate]                 访问时间
*/

GO