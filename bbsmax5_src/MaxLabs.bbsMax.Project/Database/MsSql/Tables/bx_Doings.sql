EXEC bx_Drop 'bx_Doings';

CREATE TABLE bx_Doings(
     [DoingID]            int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_Doings]                  PRIMARY KEY ([DoingID])
    ,[UserID]             int                                               NOT NULL
    ,[TotalComments]      int                                               NOT NULL    CONSTRAINT [DF_bx_Doings_TotalComments]    DEFAULT(0)
  
    ,[Content]            nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    
    ,[CreateIP]           varchar(50)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Doings_CreateIP]         DEFAULT('')
    
    ,[CreateDate]         datetime                                          NOT NULL    CONSTRAINT [DF_bx_Doings_CreateDate]       DEFAULT (GETDATE())
	,[LastCommentDate]    datetime                                          NOT NULL    CONSTRAINT [DF_bx_Doings_LastCommentDate]  DEFAULT ('1753-1-1')
	
    ,[KeywordVersion]     varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Doings_ContentVersion]   DEFAULT('')
)

/*
Name:状态
Columns:
    [DoingID]            
    [UserID]            用户ID
    [TotalComments]     回复数
    
    [Content]           状态信息
    [ContentReverter]      
    
    [CreateIP]          IP地址
    
    [CreateDate]        添加时间
*/

GO

EXEC bx_Drop 'IX_bx_Doings_UserID';
CREATE  INDEX [IX_bx_Doings_UserID] ON [bx_Doings]([UserID])

GO

