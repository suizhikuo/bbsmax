IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bx_UserRoles') AND [name]='IX_bx_UserRoles_EndDate') BEGIN
	CREATE NONCLUSTERED INDEX [IX_bx_UserRoles_EndDate] ON [bx_UserRoles] ([EndDate]);
END

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bx_UserInfos') AND [name]='IX_bx_UserInfos_TotalFriends') BEGIN
	CREATE INDEX [IX_bx_UserInfos_TotalFriends] ON [bx_UserInfos] ([TotalFriends]);
END

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bx_UserInfos') AND [name]='IX_bx_UserInfos_BirthYear') BEGIN
	CREATE INDEX [IX_bx_UserInfos_BirthYear] ON [bx_UserInfos] ([BirthYear]);
END

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bx_UserInfos') AND [name]='IX_bx_UserInfos_Birthday') BEGIN
	CREATE INDEX [IX_bx_UserInfos_Birthday] ON [bx_UserInfos] ([Birthday]);
END

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bx_Users') AND [name]='IX_bx_Users_Realname') BEGIN
	CREATE INDEX [IX_bx_Users_Realname] ON [bx_Users] ([Realname]);
END

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bx_Users') AND [name]='IX_bx_Users_SpaceViews') BEGIN
	CREATE INDEX [IX_bx_Users_SpaceViews] ON [bx_Users] ([SpaceViews]);
END

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bx_Users') AND [name]='IX_bx_Users_Points') BEGIN
	CREATE INDEX [IX_bx_Users_Points] ON [bx_Users] ([Points]);
END

GO

-----------------------------

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_DiskFiles') AND [name] ='FileSize') BEGIN
	ALTER TABLE [bx_DiskFiles] ADD [FileSize] [bigint] NOT NULL  CONSTRAINT [DF_bx_DiskFiles_FileSize]  DEFAULT(0);
END

GO

UPDATE [bx_DiskFiles] SET [FileSize] = F.[FileSize] FROM [bx_Files] F WHERE bx_DiskFiles.FileID = F.FileID;
GO

----------------------------

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bx_PointShows') AND [name]='IX_bx_PointShows_UserID') BEGIN
	CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_PointShows_UserID] ON [bx_PointShows] ([UserID]);
END

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bx_PointShows') AND [name]='IX_bx_PointShows_ShowPoints') BEGIN
	CREATE INDEX [IX_bx_PointShows_ShowPoints] ON [bx_PointShows] ([ShowPoints]);
END

GO

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_Vars') AND [name] ='YestodayTopics') BEGIN
	ALTER TABLE [bx_Vars] ADD [YestodayTopics] [int] NOT NULL   CONSTRAINT [DF_bx_Vars_YestodayTopics]   DEFAULT (0);
END

GO

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_Vars') AND [name] ='LastResetDate') BEGIN
	ALTER TABLE [bx_Vars] ADD [LastResetDate] [datetime] NOT NULL   CONSTRAINT [DF_bx_Vars_LastResetDate]   DEFAULT (GETDATE());
END

GO

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_UserInfos') AND [name] ='NotifySetting') BEGIN
	ALTER TABLE [bx_UserInfos] ADD [NotifySetting] [varchar](4000) NOT NULL    CONSTRAINT [DF_bx_UserInfos_NotifySetting]   DEFAULT ('');
END

GO

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_ChatMessages') AND [name] ='KeywordVersion') BEGIN
	ALTER TABLE [bx_ChatMessages] ADD [KeywordVersion] [varchar](32)   COLLATE Chinese_PRC_CI_AS_WS  NOT NULL  CONSTRAINT [DF_bx_ChatMessages_KeywordVersion]     DEFAULT('');
END

GO

IF NOT EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bx_UserReverters') BEGIN
	CREATE TABLE bx_UserReverters(
	[UserID]					int				NOT NULL,
	[SignatureReverter]			nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_UserReverters] PRIMARY KEY([UserID])
	);
END

GO






