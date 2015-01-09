EXEC bx_Drop 'bx_FriendGroups';

CREATE TABLE bx_FriendGroups (
	[GroupID]            int            IDENTITY(1,1)                    NOT NULL
	,[UserID]            int                                             NOT NULL
	,[GroupName]         nvarchar(50)   COLLATE Chinese_PRC_CI_AS_WS     NOT NULL
	,[TotalFriends]      int                                             NOT NULL    CONSTRAINT  [DF_bx_FriendGroups_TotalFriends]    DEFAULT (0)
	,[IsShield]          bit                                             NOT NULL    CONSTRAINT  [DF_bx_FriendGroups_IsShield]    DEFAULT (0)
	,[CreateDate]        datetime                                        NOT NULL    CONSTRAINT  [DF_bx_FriendGroups_CreateDate]      DEFAULT (GETDATE())

	,CONSTRAINT [PK_bx_FriendGroups] PRIMARY KEY ([GroupID])
)

/*
Name:好友分组
Columns:

	[GroupID]		   好友分组ID
    [UserID]           用户ID
    [GroupName]        好友分组名称
    [TotalFriends]     本组的好友个数
    [CreateDate]       成为好友时间
*/

GO

EXEC bx_Drop 'IX_bx_FriendGroups_User';
CREATE  INDEX [IX_bx_FriendGroups_User] ON [bx_FriendGroups]([UserID], [GroupName])

GO
