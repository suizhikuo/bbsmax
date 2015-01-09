EXEC bx_Drop bx_BlogArticles;

GO

CREATE TABLE bx_BlogArticles(
     [ArticleID]        int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT  [PK_bx_BlogArticles]                    PRIMARY KEY ([ArticleID])
    ,[UserID]           int                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_UserID]             DEFAULT (0)
    ,[CategoryID]       int                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_CategoryID]         DEFAULT (0)
    ,[TotalViews]       int                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_TotalViews]         DEFAULT (0)
    ,[TotalComments]    int                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_TotalComments]      DEFAULT (0)
    ,[LastEditUserID]   int                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_LastEditUserID]     DEFAULT (0)

    ,[IsApproved]       bit                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_IsApproved]         DEFAULT (1)        
    ,[EnableComment]    bit                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_EnableComment]      DEFAULT (1)

    ,[PrivacyType]      tinyint                                           NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_PrivacyType]        DEFAULT (0)

    ,[CreateIP]         varchar(50)		  COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_CreateIP]           DEFAULT ('')

    ,[Thumb]            nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_Thumb]              DEFAULT ('')
    ,[Subject]          nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_Subject]            DEFAULT ('')

    ,[Content]          ntext             COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_Content]            DEFAULT ('')

    ,[Password]         nvarchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_Password]           DEFAULT ('')

    ,[CreateDate]       datetime                                          NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_CreateDate]         DEFAULT (GETDATE())
    ,[UpdateDate]       datetime                                          NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_UpdateDate]         DEFAULT (GETDATE())
	,[LastCommentDate]  datetime                                          NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_LastCommentDate]    DEFAULT ('1753-1-1')

    ,[KeywordVersion]   varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_KeywordVersion]     DEFAULT ('')
)

GO

--创用户ID的索引
CREATE INDEX [IX_bx_BlogArticles_UserID] ON [bx_BlogArticles]([UserID]);

--创建日志隐私类型的索引
CREATE INDEX [IX_bx_BlogArticles_PrivacyType] ON [bx_BlogArticles]([PrivacyType]);

--创建日志分类ID的索引
CREATE INDEX [IX_bx_BlogArticles_CategoryID] ON [bx_BlogArticles]([CategoryID]);

--创建日志发布日期的索引
--CREATE INDEX [IX_bx_BlogArticles_CreateDate] ON [bx_BlogArticles]([CreateDate]);


/*
Name:日志
Columns:
    [ArticleID]        自动标识
    [UserID]           作者ID
    [CategoryID]       分类ID
    [TotalViews]       查看数
    [TotalComments]    回复数
    [LastEditUserID]   最后编辑者ID
    
    [IsApproved]       日志审核 如果包含禁用关键则需审核 IsApproved=false 默认为true
    [EnableComment]    是否允许评论
    
    [PrivacyType]      权限类型 0.所有用户见 1.全好友可见 2.仅自己可见 3.凭密码查看
    
    [CreateIP]         创建者的IP
    
    [Thumb]            日志略缩图
    [Subject]          标题
    [Password]         凭密码查看时的密码 
    [SubjectReverter]  标题还原关键字信息
    
    [Content]          内容
    [ContentReverter]  内容还原关键字信息
    
    [CreateDate]       添加时间
    [UpdateDate]       编辑时间
    [LastCommentDate]  最后评论时间
*/