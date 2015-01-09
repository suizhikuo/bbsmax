EXEC bx_Drop 'bx_Comments';

CREATE TABLE bx_Comments(
     [CommentID]          int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_Comments]                    PRIMARY KEY ([CommentID])
    ,[Type]               int                                               NOT NULL    CONSTRAINT [DF_bx_Comments_Type]            DEFAULT (0)    
    ,[UserID]             int                                               NOT NULL    CONSTRAINT [DF_bx_Comments_UserID]          DEFAULT (0)
    ,[TargetID]           int                                               NOT NULL    CONSTRAINT [DF_bx_Comments_TargetID]        DEFAULT (0)
    ,[TargetUserID]       int                                               NULL
    ,[LastEditUserID]     int                                               NOT NULL    CONSTRAINT [DF_bx_Comments_LastEditUserID]  DEFAULT (0)
     
    ,[IsApproved]         bit                                               NOT NULL    CONSTRAINT [DF_bx_Comments_IsApproved]      DEFAULT (1)
     
    ,[Content]            nvarchar(3000)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Comments_Content]         DEFAULT ('')
    
    ,[CreateIP]           varchar(50)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Comments_CreateIP]        DEFAULT ('')
    
    ,[CreateDate]         datetime                                          NOT NULL    CONSTRAINT [DF_bx_Comments_CreateDate]      DEFAULT (GETDATE())
    ,[KeywordVersion]     varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Comments_ContentVersion]  DEFAULT ('')
)

/*
Name:评论
Columns:
    [CommentID]                 自动标识
    [Type]               评论应用类型 1.留言板 2.相片 3.日志 4.状态
    [UserID]             评论者ID 留言者ID
    [TargetID]           被评论的ID 相片ID 日志ID 状态ID 被留言用户ID
    [TargetUserID]       被评论的用户ID 留言时冗余
    [LastEditUserID]     最后编辑者
    
    [IsApproved]         评论审核 如果包含禁用关键则需审核 IsApproved=false 默认为true
    
    [Content]            评论内容
    [ContentReverter]    用于替换后还原的内容
    
    [CreateIP]           评论者的IP
    [KeywordVersion]     关键字版本
    
    [CreateDate]         评论时间
*/

GO

EXEC bx_Drop 'IX_bx_Comments_TargetID';
CREATE  INDEX [IX_bx_Comments_TargetID] ON [bx_Comments]([TargetID])

GO
