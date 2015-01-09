EXEC bx_Drop 'bx_Friends';

CREATE TABLE bx_Friends (
 
	[UserID]            int                                             NOT NULL
	,[FriendUserID]     int                                             NOT NULL
	,[GroupID]          int                                             NOT NULL    CONSTRAINT  [DF_bx_Friends_GroupID]			DEFAULT(0)
	,[Hot]              int                                             NOT NULL    CONSTRAINT  [DF_bx_Friends_Hot]				DEFAULT(0)
	,[CreateDate]       datetime                                        NOT NULL    CONSTRAINT  [DF_bx_Friends_CreateDate]		DEFAULT(GETDATE())

	,CONSTRAINT [PK_bx_Friends] PRIMARY KEY ([UserID],[FriendUserID])
)

/*
Name:好友
Columns:

    [UserID]           用户ID
    [FriendUserID]     好友ID
    [GroupID]          好友分组ID
	[Hot]              好友之间的热度 访问空间(+1) 回复日志(+2) 回复相册(+2) 回复状态(+2) 留言(+2) 打招呼(+1)
    [CreateDate]       成为好友时间
*/

GO

EXEC bx_Drop 'IX_bx_Friends_UserID';
CREATE  INDEX [IX_bx_Friends_UserID] ON [bx_Friends]([UserID])

GO
