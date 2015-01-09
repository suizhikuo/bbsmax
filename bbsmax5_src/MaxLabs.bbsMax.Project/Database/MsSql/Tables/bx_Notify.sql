--通知表
EXEC bx_Drop bx_Notify;

CREATE TABLE [bx_Notify] (
     [NotifyID]             int                    IDENTITY(1,1)                        NOT NULL   
	,[UserID]               int                                                         NOT NULL    CONSTRAINT [DF_bx_Notify_UserID]             DEFAULT(0)
	,[Content]				nvarchar(1000)         COLLATE Chinese_PRC_CI_AS_WS			NULL
	,[IsRead]               bit                                                         NOT NULL    CONSTRAINT [DF_bx_Notify_IsRead]             DEFAULT(0)
	,[TypeID]               int                                                         NOT NULL    CONSTRAINT [DF_bx_Notify_TypeID]             DEFAULT(0)
	,[Keyword]              varchar(200)												NULL
	,[NotifyDatas]          ntext														NULL
	,[CreateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_Notify_CreateDate]         DEFAULT(GETDATE())
	,[UpdateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_Notify_UpdateDate]         DEFAULT(GETDATE())
	,[ClientID]				int															NULL		CONSTRAINT [DF_bx_Notify_ClientID]           DEFAULT(0)
	,[Actions]              nvarchar(2000)			COLLATE Chinese_PRC_CI_AS_WS 		NULL
	,CONSTRAINT [PK_bx_Notify] PRIMARY KEY ([NotifyID])    
);

/*
Name:     消息表,包括用户消息,系统消息,通知提醒消息
Columns:
          [NotifyID]
          [UserID]                              始终表示这个通知的拥有者的ID
          [RelatedUserID]                       比如加好友,这里指的就是加我为好友的用户ID; 比如留言或回复,指的就是给我留言或回复的用户ID...
                   
          [IsRead]                              消息是否已读       
          
          [Type]                                类型, 1-留言及回复消息,2-群主邀请,3-好友验证消息,4-打招呼消息,5-应用邀请,6-应用通知    
          
          [Content]                             消息内容
          [SenderIP]                            发件人的IP 
          [Parameters]                          额外参数
          
          [PostDate]                            消息发送时间
*/

GO


--用户ID索引
CREATE INDEX [IX_bx_Notify_UserID] ON [bx_Notify]([UserID])

--通知类型索引
CREATE INDEX [IX_bx_Notify_Type] ON [bx_Notify]([TypeID])

--时间索引
CREATE INDEX [IX_bx_Notify_UpdateDate] ON [bx_Notify]([UpdateDate])

CREATE INDEX [IX_bx_Notify_Keyword] ON [bx_Notify]([Keyword])

GO