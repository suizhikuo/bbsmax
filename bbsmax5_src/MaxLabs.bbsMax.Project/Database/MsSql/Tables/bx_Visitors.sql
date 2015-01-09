EXEC bx_Drop 'bx_Visitors';

CREATE TABLE bx_Visitors (
    [ID]               int            IDENTITY(1,1)                   NOT NULL  
   ,[UserID]           int                                            NOT NULL
   ,[VisitorUserID]    int                                            NOT NULL
   
   ,[CreateIP]         varchar(50)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
   
   ,[CreateDate]       datetime                                       NOT NULL    CONSTRAINT  [DF_bx_Visitors_CreateDate]    DEFAULT (GETDATE())
   
   ,CONSTRAINT [PK_bx_Visitors] PRIMARY KEY ([ID])
)

/*
Name:好友访问列表
Columns:
    [UserID]           用户ID
    [VisitorID]        访问者ID
    
    [CreateIP]         IP地址
    
    [CreateDate]       访问时间
*/

GO

--EXEC bx_Drop 'IX_bx_Visitors_UserID';
CREATE NONCLUSTERED INDEX [IX_bx_Visitors_UserID] ON [bx_Visitors]([UserID], [CreateDate])
CREATE NONCLUSTERED INDEX [IX_bx_Visitors_VisitorID] ON [bx_Visitors]([VisitorUserID], [CreateDate])
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Visitors_Key] ON [bx_Visitors]([UserID], [VisitorUserID])

GO
