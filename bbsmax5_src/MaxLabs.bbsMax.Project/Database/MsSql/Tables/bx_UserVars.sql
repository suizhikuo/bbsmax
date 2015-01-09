EXEC bx_Drop 'bx_UserVars';

CREATE TABLE bx_UserVars (
     [UserID]					 int												NOT NULL

    ,[Password]					 nvarchar(50)		COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    ,[PasswordFormat]			 tinyint            								NOT NULL    CONSTRAINT [DF_bx_UserVars_PasswordFormat]             	DEFAULT (3)
    
    ,[UnreadMessages]            int												NOT NULL    CONSTRAINT [DF_bx_UserVars_UnreadMessages]             	DEFAULT(0)

	,[LastReadSystemNotifyID]    int            									NOT NULL    CONSTRAINT [DF_bx_UserVars_LostReadSystemNotifyID]      DEFAULT(0)


    ,[UsedAlbumSize]             bigint         									NOT NULL    CONSTRAINT [DF_bx_UserVars_UsedAlbumSize]              	DEFAULT(0)
    ,[AddedAlbumSize]            bigint         									NOT NULL    CONSTRAINT [DF_bx_UserVars_AddedAlbumSize]             	DEFAULT(0)
	,[TimeZone]                  real           									NOT NULL	CONSTRAINT [DF_bx_UserVars_TimeZone]				    DEFAULT(9999)
    ,[EverAvatarChecked]		 bit                								NOT NULL    CONSTRAINT [DF_bx_UserVars_EverAvatarChecked]          	DEFAULT(0)
	,[EnableDisplaySidebar]		 tinyint            								NOT NULL    CONSTRAINT [DF_bx_UserVars_EnableDisplaySidebar]       	DEFAULT(0)
	,[OnlineStatus]				 tinyint											NOT NULL    CONSTRAINT [DF_bx_UserVars_OnlineStatus]			    DEFAULT(0)

	,[SkinID]					 nvarchar(256)		COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_UserVars_SkinID]						DEFAULT('')

	,[TotalDiskFiles]			 int												NOT NULL    CONSTRAINT [DF_bx_UserVars_TotalDiskFiles]			    DEFAULT (0)
	,[UsedDiskSpaceSize]		 bigint												NOT NULL    CONSTRAINT [DF_bx_UserVars_UsedDiskSpaceSize]			DEFAULT(0)
    ,[LastAvatarUpdateDate]		 datetime           								NOT NULL    CONSTRAINT [DF_bx_UserVars_LastAvatarUpdateDate]		DEFAULT(GETDATE())
	,[LastImpressionDate]		 datetime           								NOT NULL															DEFAULT('1980-1-1')
    ,[SelectFriendGroupID]       int												NOT NULL    CONSTRAINT [DF_bx_UserVars_SelectFriendGroupID]			DEFAULT (-1)
	,[ReplyReturnThreadLastPage] bit                                                NULL    
    ,CONSTRAINT [PK_bx_UserVars] PRIMARY KEY ([UserID])
)


CREATE INDEX [IX_bx_UserVars_LastImpressionDate] ON [bx_UserVars] ([LastImpressionDate]);