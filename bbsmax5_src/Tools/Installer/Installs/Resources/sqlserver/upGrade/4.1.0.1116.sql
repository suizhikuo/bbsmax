
IF NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('bx_Files') AND [name] = 'FileID') BEGIN
	EXEC ('ALTER TABLE bx_Files ADD [FileID] varchar(50) COLLATE Chinese_PRC_CI_AS_WS NOT NULL DEFAULT('''')');

	EXEC ('UPDATE bx_Files SET FileID = MD5 + CAST(FileSize as varchar(30))');

END

GO

--处理照片----------------------------------------------------------------------

--删除Photos表的FileID索引
IF EXISTS (SELECT * FROM sysindexes WHERE name='IX_bx_Photos_FileID')
	DROP INDEX bx_Photos.IX_bx_Photos_FileID;

GO

--删除Photos表的FileID外建
IF EXISTS (SELECT * FROM sysobjects WHERE name='FK_bx_Photos_FileID' AND type='F')
	ALTER TABLE bx_Photos DROP CONSTRAINT [FK_bx_Photos_FileID];

GO

--如果FileID还不是varchar类型的，做以下事情
IF NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('bx_Photos') AND [name] = 'FileID' AND [xtype] = 167) BEGIN

	IF NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('bx_Photos') AND [name] = 'TmpFileID')
		EXEC ('ALTER TABLE bx_Photos ADD [TmpFileID] varchar(50) COLLATE Chinese_PRC_CI_AS_WS NOT NULL DEFAULT('''')');

	EXEC ('UPDATE bx_Photos SET bx_Photos.TmpFileID = F.FileID FROM bx_Files F WHERE bx_Photos.FileID = F.ID');
	
	EXEC ('ALTER TABLE bx_Photos DROP COLUMN [FileID]');

	EXEC sp_rename 'bx_Photos.TmpFileID','FileID';

END

GO

--处理附件----------------------------------------------------------------------

--删除Attachments表的FileID索引
IF EXISTS (SELECT * FROM sysindexes WHERE name='IX_bx_Attachments_FileID')
	DROP INDEX bx_Attachments.IX_bx_Attachments_FileID;

GO

--如果FileID还不是varchar类型的，做以下事情
IF NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('bx_Attachments') AND [name] = 'FileID' AND [xtype] = 167) BEGIN

	IF NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('bx_Attachments') AND [name] = 'TmpFileID')
		EXEC ('ALTER TABLE bx_Attachments ADD [TmpFileID] varchar(50) COLLATE Chinese_PRC_CI_AS_WS NOT NULL DEFAULT('''')');

	EXEC ('UPDATE bx_Attachments SET bx_Attachments.TmpFileID = F.FileID FROM bx_Files F WHERE bx_Attachments.FileID = F.ID');
	
	EXEC ('ALTER TABLE bx_Attachments DROP COLUMN [FileID]');

	EXEC sp_rename 'bx_Attachments.TmpFileID','FileID';

END

GO

IF NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('bx_Emoticons') AND [name] = 'ImageSrc') BEGIN

	EXEC sp_rename 'bx_Emoticons.ImageUrlSrc','ImageSrc';

END

GO

--处理网络硬盘----------------------------------------------------------------------

--删除DiskFiles表的FileID索引
IF EXISTS (SELECT * FROM sysindexes WHERE name='IX_bx_DiskFiles_FileID')
	DROP INDEX bx_DiskFiles.IX_bx_DiskFiles_FileID;

GO

--删除DiskFiles表的FileID外建
IF EXISTS (SELECT * FROM sysobjects WHERE name='FK_bx_DiskFiles_FileID' AND type='F')
	ALTER TABLE bx_DiskFiles DROP CONSTRAINT [FK_bx_DiskFiles_FileID];

GO

--删除DiskFiles表的FileID默认值
IF EXISTS (SELECT * FROM sysobjects WHERE name='DF_bx_DiskFiles_FileID' AND type='D')
	ALTER TABLE bx_DiskFiles DROP CONSTRAINT [DF_bx_DiskFiles_FileID];

GO

--如果FileID还不是varchar类型的，做以下事情
IF NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('bx_DiskFiles') AND [name] = 'FileID' AND [xtype] = 167) BEGIN

	IF NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('bx_DiskFiles') AND [name] = 'TmpFileID')
		EXEC ('ALTER TABLE bx_DiskFiles ADD [TmpFileID] varchar(50) COLLATE Chinese_PRC_CI_AS_WS NOT NULL DEFAULT('''')');

	EXEC ('UPDATE bx_DiskFiles SET bx_DiskFiles.TmpFileID = F.FileID FROM bx_Files F WHERE bx_DiskFiles.FileID = F.ID');
	
	EXEC ('ALTER TABLE bx_DiskFiles DROP COLUMN [FileID]');

	EXEC sp_rename 'bx_DiskFiles.TmpFileID','FileID';

END

GO

IF NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('bx_Emoticons') AND [name] = 'UserID') BEGIN

	EXEC ('ALTER TABLE bx_Emoticons ADD [UserID] int NOT NULL DEFAULT(0)');
	
	EXEC ('UPDATE bx_Emoticons SET bx_Emoticons.UserID = G.UserID FROM bx_EmoticonGroups G WHERE bx_Emoticons.GroupID = G.GroupID');

END

GO

--道具默认数据----------------------------------------------------------------------

GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE [name]='bx_Props' AND type='U') OR NOT EXISTS (SELECT [name] FROM syscolumns WHERE id = object_id('bx_Props') AND [name] = 'ReplenishLimit') BEGIN	

	IF EXISTS (SELECT * FROM sysobjects WHERE [name]='bx_Props' AND type='U')
		DROP TABLE bx_Props;

		EXEC ('CREATE TABLE [bx_Props] (
   [PropID]            int           IDENTITY(1,1)                NOT NULL
  ,[Icon]              nvarchar(255) COLLATE Chinese_PRC_CI_AS_WS NOT NULL
  ,[Name]              nvarchar(100) COLLATE Chinese_PRC_CI_AS_WS NOT NULL
  ,[Price]             int                                        NOT NULL
  ,[PriceType]         int                                        NOT NULL
  ,[PropType]          nvarchar(512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL
  ,[PropParam]         ntext         COLLATE Chinese_PRC_CI_AS_WS NOT NULl
  ,[Description]       nvarchar(255) COLLATE Chinese_PRC_CI_AS_WS NOT NULL
  ,[PackageSize]       int                                        NOT NULL
  ,[TotalNumber]       int                                        NOT NULL
  ,[SaledNumber]       int                                        NOT NULL DEFAULT(0)
  ,[AllowExchange]     bit                                        NOT NULL
  ,[AutoReplenish]     bit                                        NOT NULl
  ,[ReplenishNumber]   int                                        NOT NULL
  ,[ReplenishTimeSpan] int                                        NOT NULL
  ,[LastReplenishTime] datetime                                   NOT NULL DEFAULT(GETDATE())
  ,[BuyCondition]      ntext                                      NOT NULL
  ,[Enable]            bit                                        NOT NULL DEFAULT(1)
  ,[ReplenishLimit]    int                                        NOT NULL DEFAULT(0)
  ,[SortOrder]         int                                        NOT NULL DEFAULT(0)
  
  ,CONSTRAINT [PK_bx_Props] PRIMARY KEY ([PropID])
);');

END