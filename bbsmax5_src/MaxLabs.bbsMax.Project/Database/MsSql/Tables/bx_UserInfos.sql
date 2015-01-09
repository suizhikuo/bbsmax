EXEC bx_Drop 'bx_UserInfos';

CREATE TABLE bx_UserInfos (
     [UserID]			      int               NOT NULL

    ,[InviterID]              int               NOT NULL    CONSTRAINT [DF_bx_UserInfos_InviterID]	         DEFAULT (0)

	,[TotalFriends]           int               NOT NULL    CONSTRAINT [DF_bx_UserInfos_TotalFriends]               DEFAULT (0)

    ,[Birthday]               smallint          NOT NULL    CONSTRAINT [DF_bx_UserInfos_Birthday]                   DEFAULT (0)
    ,[BirthYear]              smallint          NOT NULL    CONSTRAINT [DF_bx_UserInfos_BirthYear]                  DEFAULT (0)

    ,[BlogPrivacy]           tinyint            NOT NULL    CONSTRAINT [DF_bx_UserInfos_BlogPrivacy]               DEFAULT (0)
    ,[FeedPrivacy]           tinyint            NOT NULL    CONSTRAINT [DF_bx_UserInfos_FeedPrivacy]               DEFAULT (0)
    ,[BoardPrivacy]          tinyint            NOT NULL    CONSTRAINT [DF_bx_UserInfos_BoardPrivacy]              DEFAULT (0)
    ,[DoingPrivacy]          tinyint            NOT NULL    CONSTRAINT [DF_bx_UserInfos_DoingPrivacy]              DEFAULT (0)
    ,[AlbumPrivacy]          tinyint            NOT NULL    CONSTRAINT [DF_bx_UserInfos_AlbumPrivacy]              DEFAULT (0)
    ,[SpacePrivacy]          tinyint            NOT NULL    CONSTRAINT [DF_bx_UserInfos_SpacePrivacy]              DEFAULT (0)
    ,[SharePrivacy]          tinyint            NOT NULL    CONSTRAINT [DF_bx_UserInfos_SharePrivacy]              DEFAULT (0)
    ,[FriendListPrivacy]     tinyint            NOT NULL    CONSTRAINT [DF_bx_UserInfos_FriendListPrivacy]         DEFAULT (1)
    ,[InformationPrivacy]    tinyint            NOT NULL    CONSTRAINT [DF_bx_UserInfos_InformationPrivacy]        DEFAULT (1)

	,[NotifySetting]         varchar(200)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_UserInfos_NotifySetting] DEFAULT ('')
    ,CONSTRAINT [PK_bx_UserInfos] PRIMARY KEY ([UserID])
)
 
CREATE INDEX [IX_bx_UserInfos_TotalFriends] ON [bx_UserInfos] ([TotalFriends]);
CREATE INDEX [IX_bx_UserInfos_BirthYear] ON [bx_UserInfos] ([BirthYear]);
CREATE INDEX [IX_bx_UserInfos_Birthday] ON [bx_UserInfos] ([Birthday]);
CREATE INDEX [IX_bx_UserInfos_InviterID] ON [bx_UserInfos] ([InviterID]);