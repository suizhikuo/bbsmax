--日志访问者记录表的日志ID外键关系
--日志访问者记录的用户ID外键关系
EXEC bx_Drop 'FK_bx_BlogArticleVisitors_BlogArticleID';
EXEC bx_Drop 'FK_bx_BlogArticleVisitors_UserID';

ALTER TABLE [bx_BlogArticleVisitors] ADD
        CONSTRAINT [FK_bx_BlogArticleVisitors_BlogArticleID]        FOREIGN KEY ([BlogArticleID])      REFERENCES [bx_BlogArticles]     ([ArticleID])         ON DELETE CASCADE   
       ,CONSTRAINT [FK_bx_BlogArticleVisitors_UserID]               FOREIGN KEY ([UserID])             REFERENCES [bx_Users]            ([UserID])          

GO