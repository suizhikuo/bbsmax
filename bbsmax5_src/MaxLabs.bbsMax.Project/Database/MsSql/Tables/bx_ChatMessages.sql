--消息表
EXEC bx_Drop bx_ChatMessages;

CREATE TABLE [bx_ChatMessages] (
     [MessageID]            int                    IDENTITY(1,1)                        NOT NULL

	,[UserID]               int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_UserID]             DEFAULT(0)
	,[TargetUserID]         int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_TargetUserID]       DEFAULT(0)
	,[IsReceive]            bit                                                         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_IsReceive]          DEFAULT(0)
	,[IsRead]               bit                                                         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_IsRead]             DEFAULT(0)
	,[FromMessageID]        int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_FromMessageID]      DEFAULT(0)

	,[Content]              nvarchar(3000)         COLLATE Chinese_PRC_CI_AS_WS         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_Content]            DEFAULT('')
	,[KeywordVersion]       varchar(32)            COLLATE Chinese_PRC_CI_AS_WS         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_KeywordVersion]     DEFAULT('')

	,[CreateIP]             varchar(50)            COLLATE Chinese_PRC_CI_AS_WS         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_CreateIP]           DEFAULT('')
	,[CreateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_ChatMessages_CreateDate]         DEFAULT(GETDATE())

	
	,CONSTRAINT [PK_bx_ChatMessages] PRIMARY KEY ([MessageID])
);

/*
Name:     消息表,包括用户消息,系统消息,通知提醒消息
Columns:
          [MessageID]
          [UserID]                              始终表示这个消息的拥有者的ID。
          [TargetUserID]                        表示对方的UserID
          [IsReceive]                           true表示这是一条接收到的消息。如果true，表示这是UserID接收自TargetUserID的消息，否则就是从UserID发送给TargetUserID的消息
          [IsRead]                              [IsReceive]为true的时候这个字段才有意义。表示消息是否已读，否则且值始终保持1
          [FromMessageID]                       [IsReceive]为true的时候这个字段才有意义。表示接收到的这条消息来自于哪条消息，即
          
          [Content]                             消息内容
          [ReplaceVersion]                      文本替换版本，用来效验

          [CreateIP]                            发件人的IP 
          [CreateDate]                          消息发送时间
*/

GO

--用户ID索引
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_ChatMessages_List] ON [bx_ChatMessages]([UserID], [IsRead], [MessageID])
GO