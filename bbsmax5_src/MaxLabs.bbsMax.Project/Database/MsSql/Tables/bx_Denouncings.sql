EXEC bx_Drop 'bx_Denouncings';

CREATE TABLE bx_Denouncings(
     [DenouncingID]       int                   IDENTITY(1, 1)              NOT NULL    CONSTRAINT [PK_bx_Denouncings]                 PRIMARY KEY ([DenouncingID])
    
    ,[TargetID]           int                                               NOT NULL
    ,[TargetUserID]       int                                               NOT NULL
  
    ,[Type]               tinyint                                           NOT NULL
   
    ,[IsIgnore]           bit                                               NOT NULL    CONSTRAINT [DF_bx_Denouncings_IsIgnore]        DEFAULT(0)
    
    ,[CreateDate]         datetime                                          NOT NULL    CONSTRAINT [DF_bx_Denouncings_CreateDate]      DEFAULT (GETDATE())
)

CREATE INDEX [IX_bx_Denouncings_IsIgnore] ON [bx_Denouncings]([IsIgnore]);
CREATE INDEX [IX_bx_Denouncings_Type_TargetID] ON [bx_Denouncings]([Type],[TargetID]);

/*
Name:状态
Columns:
    [ReportID]
    [UserID]            用户ID
    [TargetID]          举报对象的ID
    
    [Type]              举报对象类型
    [Reason]            举报原因
    
    [IsIgnore]          是否已忽略
    
    [Content]           举报内容
    
    [CreateIP]          IP地址
    
    [CreateDate]        添加时间
*/

GO

