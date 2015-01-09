EXEC bx_Drop 'bx_InviteSerials';

CREATE TABLE [bx_InviteSerials] (
   [ID]               int        IDENTITY(1, 1)                        NOT NULL                 
 
  ,[Serial]           uniqueidentifier                                 NOT NULL    CONSTRAINT [DF_bx_InviteSerials_Serial]          DEFAULT (NEWID())
  
  ,[ToEmail]          nvarchar(200)      COLLATE Chinese_PRC_CI_AS_WS       NULL
   
  ,[CreateDate]       datetime                                         NOT NULL    CONSTRAINT [DF_bx_InviteSerials_BeginDate]       DEFAULT (GETDATE())
  ,[ExpiresDate]      datetime                                         NOT NULL
    
  ,[UserID]           int                                              NOT NULL
  ,[ToUserID]         int                                              NOT NULL    CONSTRAINT [DF_bx_InviteSerials_ToUserID]        DEFAULT (0)
  
  ,[Remark]			  nvarchar(200)									   NOT NULL	   CONSTRAINT [DF_bx_InviteSerials_Remark]			DEFAULT('')
  
  ,[Status]           tinyint                                          NOT NULL    CONSTRAINT [DF_bx_InviteSerials_Status]          DEFAULT (0)
  
  ,CONSTRAINT [PK_bx_InviteSerials] PRIMARY KEY ([Serial])
  );

GO  

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_InviteSerials_ID] ON [bx_InviteSerials] ([ID]);
CREATE NONCLUSTERED INDEX [IX_bx_InviteSerials_UserID] ON [bx_InviteSerials] ([UserID]);
CREATE NONCLUSTERED INDEX [IX_bx_InviteSerials_Expires] ON [bx_InviteSerials] ([ExpiresDate]);


CREATE INDEX [IX_bx_InviteSerials_Serial] ON [bx_InviteSerials] ([Serial]);
GO

/*
Name:邀请码表
Columns:
	[ID]               自增长,用于分页
    [Serial]           邀请码(主键)
    
    [ToEmail]          发送到的Email       
    [CreateDate]       创建时间
    [ExpiresDate]      过期时间
    
    [UserID]           用户ID
    [ToUserID]         发送给的用户ID
    
    [Status]           状态
*/

GO