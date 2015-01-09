--以下是idmax升级脚本

DECLARE @pk_name NVARCHAR(50)
SELECT TOP 1 @pk_name = object_name(pks.constid) FROM sysconstraints pks INNER JOIN sysobjects ts ON pks.id = ts.id WHERE OBJECTPROPERTY(pks.constid,'IsPrimaryKey') = 1 AND ts.name = 'Max_SystemMessages' AND ts.type='U'
IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='' +@pk_name+'')
EXEC ('ALTER TABLE [Max_SystemMessages] DROP CONSTRAINT[' +@pk_name+']')

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='FK_Max_UserMedals_Medals')
ALTER TABLE [Max_UserMedals] DROP CONSTRAINT [FK_Max_UserMedals_Medals]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_ContactGroups') AND [name]='IX_Max_ContactGroups_List')
DROP INDEX [Max_ContactGroups].[IX_Max_ContactGroups_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Contacts') AND [name]='IX_Max_Contacts_List')
DROP INDEX [Max_Contacts].[IX_Max_Contacts_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DefaultEmoticons') AND [name]='IX_Max_DefaultEmoticons_SortOrder')
DROP INDEX [Max_DefaultEmoticons].[IX_Max_DefaultEmoticons_SortOrder]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DeleteFileQueue') AND [name]='IX_Max_DeleteFileQueue_TryTimes')
DROP INDEX [Max_DeleteFileQueue].[IX_Max_DeleteFileQueue_TryTimes]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DiskDirectories') AND [name]='IX_Max_DiskDirectories_List')
DROP INDEX [Max_DiskDirectories].[IX_Max_DiskDirectories_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DiskFiles') AND [name]='IX_Max_DiskFiles_List')
DROP INDEX [Max_DiskFiles].[IX_Max_DiskFiles_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_EmoticonGroups') AND [name]='IX_Max_EmoticonGroups_List')
DROP INDEX [Max_EmoticonGroups].[IX_Max_EmoticonGroups_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Emoticons') AND [name]='IX_Max_Emoticons_List')
DROP INDEX [Max_Emoticons].[IX_Max_Emoticons_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_FavoriteDirectories') AND [name]='IX_Max_FavoriteDirectories_List')
DROP INDEX [Max_FavoriteDirectories].[IX_Max_FavoriteDirectories_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_FavoriteLinks') AND [name]='IX_Max_FavoriteLinks_List')
DROP INDEX [Max_FavoriteLinks].[IX_Max_FavoriteLinks_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_InviteSerials') AND [name]='IX_Max_InviteSerials_Expires')
DROP INDEX [Max_InviteSerials].[IX_Max_InviteSerials_Expires]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Phrases') AND [name]='IX_Max_Phrases_List')
DROP INDEX [Max_Phrases].[IX_Max_Phrases_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Roles') AND [name]='IX_Max_Roles')
DROP INDEX [Max_Roles].[IX_Max_Roles]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_SendMailQueue') AND [name]='IX_Max_SendMailQueue_SendCount')
DROP INDEX [Max_SendMailQueue].[IX_Max_SendMailQueue_SendCount]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_UserMedals') AND [name]='IX_Max_UserMedals_UserList')
DROP INDEX [Max_UserMedals].[IX_Max_UserMedals_UserList]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_ContactGroups') AND [name]='IX_Max_ContactGroups_List2')
DROP INDEX [Max_ContactGroups].[IX_Max_ContactGroups_List2]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Contacts') AND [name]='IX_Max_Contacts')
DROP INDEX [Max_Contacts].[IX_Max_Contacts]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DiskDirectories') AND [name]='IX_Max_DiskDirectories_List2')
DROP INDEX [Max_DiskDirectories].[IX_Max_DiskDirectories_List2]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DiskFiles') AND [name]='IX_Max_DiskFiles_List2')
DROP INDEX [Max_DiskFiles].[IX_Max_DiskFiles_List2]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_EmoticonGroups') AND [name]='IX_Max_EmoticonGroups_List2')
DROP INDEX [Max_EmoticonGroups].[IX_Max_EmoticonGroups_List2]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Emoticons') AND [name]='IX_Max_Emoticons_List2')
DROP INDEX [Max_Emoticons].[IX_Max_Emoticons_List2]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_FavoriteDirectories') AND [name]='IX_Max_FavoriteDirectories_List2')
DROP INDEX [Max_FavoriteDirectories].[IX_Max_FavoriteDirectories_List2]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_FavoriteLinks') AND [name]='IX_Max_FavoriteLinks_List2')
DROP INDEX [Max_FavoriteLinks].[IX_Max_FavoriteLinks_List2]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Phrases') AND [name]='IX_Max_Phrases_List2')
DROP INDEX [Max_Phrases].[IX_Max_Phrases_List2]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_UserMedals') AND [name]='IX_Max_UserMedals_UserList2')
DROP INDEX [Max_UserMedals].[IX_Max_UserMedals_UserList2]

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='DF_System_Max_AdvertisementCatalogs_IsRandomSort')
ALTER TABLE [System_Max_AdvertisementCatalogs] DROP CONSTRAINT [DF_System_Max_AdvertisementCatalogs_IsRandomSort]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('IsRandomSort') AND [name] ='System_Max_AdvertisementCatalogs')
ALTER TABLE [System_Max_AdvertisementCatalogs] DROP COLUMN [IsRandomSort]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_Jobs') AND [name] ='ExecuteTime')
ALTER TABLE [System_Max_Jobs] DROP COLUMN [ExecuteTime]

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='IX_Max_Users_UserName')
ALTER TABLE [Max_Users] DROP CONSTRAINT [IX_Max_Users_UserName]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Users') AND [name] ='UserName')
ALTER TABLE [Max_Users] ALTER COLUMN [UserName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='IX_Max_Users_UserName')
ALTER TABLE [Max_Users] ADD  CONSTRAINT [IX_Max_Users_UserName] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_AdminLoginLogs') AND [name] ='UserIP')
ALTER TABLE [Max_AdminLoginLogs] ALTER COLUMN [UserIP] [nvarchar](15) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_AdminLoginLogs') AND [name] ='ErrorPassword')
ALTER TABLE [Max_AdminLoginLogs] ALTER COLUMN [ErrorPassword] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ChangeNickNameLogs') AND [name] ='UserName')
ALTER TABLE [Max_ChangeNickNameLogs] ALTER COLUMN [UserName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL
GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ChangeNickNameLogs') AND [name] ='OldNickName')
ALTER TABLE [Max_ChangeNickNameLogs] ALTER COLUMN [OldNickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL
GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ChangePasswordLogs') AND [name] ='UserName')
ALTER TABLE [Max_ChangePasswordLogs] ALTER COLUMN [UserName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ChangePasswordLogs') AND [name] ='OldPassword')
ALTER TABLE [Max_ChangePasswordLogs] ALTER COLUMN [OldPassword] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ContactGroups') AND [name] ='GroupName')
ALTER TABLE [Max_ContactGroups] ALTER COLUMN [GroupName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Contacts') AND [name] ='ContactNickName')
ALTER TABLE [Max_Contacts] ALTER COLUMN [ContactNickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_CurrencyFields') AND [name] ='CurrencyName')
ALTER TABLE [Max_CurrencyFields] ALTER COLUMN [CurrencyName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_CurrencyFields') AND [name] ='CurrencyUnit')
ALTER TABLE [Max_CurrencyFields] ALTER COLUMN [CurrencyUnit] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_CurrencyFields') AND [name] ='Ratio1')
ALTER TABLE [Max_CurrencyFields] ALTER COLUMN [Ratio1] [varchar](32) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_CurrencyFields') AND [name] ='Ratio2')
ALTER TABLE [Max_CurrencyFields] ALTER COLUMN [Ratio2] [varchar](32) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_CurrencyFields') AND [name] ='Ratio3')
ALTER TABLE [Max_CurrencyFields] ALTER COLUMN [Ratio3] [varchar](32) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DefaultEmoticons') AND [name]='IX_Max_DefaultEmoticons_Shortcut')
DROP INDEX [Max_DefaultEmoticons].[IX_Max_DefaultEmoticons_Shortcut]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_DefaultEmoticons') AND [name] ='Shortcut')
ALTER TABLE [Max_DefaultEmoticons] ALTER COLUMN [Shortcut] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DefaultEmoticons') AND [name]='IX_Max_DefaultEmoticons_Shortcut')
CREATE NONCLUSTERED INDEX [IX_Max_DefaultEmoticons_Shortcut] ON [Max_DefaultEmoticons] 
(
	[Shortcut] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_DefaultEmoticons') AND [name] ='FileName')
ALTER TABLE [Max_DefaultEmoticons] ALTER COLUMN [FileName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_DeleteFileQueue') AND [name] ='ServerFileName')
ALTER TABLE [Max_DeleteFileQueue] ALTER COLUMN [ServerFileName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_DiskDirectories') AND [name] ='DirectoryName')
ALTER TABLE [Max_DiskDirectories] ALTER COLUMN [DirectoryName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_DiskFiles') AND [name] ='FileName')
ALTER TABLE [Max_DiskFiles] ALTER COLUMN [FileName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_DiskFiles') AND [name] ='ExProFormat')
ALTER TABLE [Max_DiskFiles] ALTER COLUMN [ExProFormat] [nvarchar](2048) COLLATE Chinese_PRC_CI_AS_WS NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_EmoticonGroups') AND [name] ='GroupName')
ALTER TABLE [Max_EmoticonGroups] ALTER COLUMN [GroupName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Emoticons') AND [name]='IX_Max_Emoticons_UserShortcut')
DROP INDEX [Max_Emoticons].[IX_Max_Emoticons_UserShortcut]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Emoticons') AND [name] ='Shortcut')
ALTER TABLE [Max_Emoticons] ALTER COLUMN [Shortcut] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Emoticons') AND [name]='IX_Max_Emoticons_UserShortcut')
CREATE NONCLUSTERED INDEX [IX_Max_Emoticons_UserShortcut] ON [Max_Emoticons] 
(
	[GroupID] ASC,
	[Shortcut] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Emoticons') AND [name]='IX_Max_Emoticons_UserFileName')
DROP INDEX [Max_Emoticons].[IX_Max_Emoticons_UserFileName]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Emoticons') AND [name]='IX_Max_Emoticons_FileName')
DROP INDEX [Max_Emoticons].[IX_Max_Emoticons_FileName]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Emoticons') AND [name] ='FileName')
ALTER TABLE [Max_Emoticons] ALTER COLUMN [FileName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Emoticons') AND [name]='IX_Max_Emoticons_FileName')
CREATE NONCLUSTERED INDEX [IX_Max_Emoticons_FileName] ON [Max_Emoticons] 
(
	[FileName] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Emoticons') AND [name]='IX_Max_Emoticons_UserFileName')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_Emoticons_UserFileName] ON [Max_Emoticons] 
(
	[GroupID] ASC,
	[FileName] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ErrorPasswordLogs') AND [name] ='UserName')
ALTER TABLE [Max_ErrorPasswordLogs] ALTER COLUMN [UserName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ErrorPasswordLogs') AND [name] ='ErrorPassword')
ALTER TABLE [Max_ErrorPasswordLogs] ALTER COLUMN [ErrorPassword] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='PK_Max_ExtendedUserProfileFields')
ALTER TABLE [Max_ExtendedUserProfileFields] DROP CONSTRAINT [PK_Max_ExtendedUserProfileFields]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ExtendedUserProfileFields') AND [name] ='FieldName')
ALTER TABLE [Max_ExtendedUserProfileFields] ALTER COLUMN [FieldName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='PK_Max_ExtendedUserProfileFields')
ALTER TABLE [Max_ExtendedUserProfileFields] ADD  CONSTRAINT [PK_Max_ExtendedUserProfileFields] PRIMARY KEY CLUSTERED 
(
	[FieldName] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ExtendedUserProfileFields') AND [name] ='Constraint')
ALTER TABLE [Max_ExtendedUserProfileFields] ALTER COLUMN [Constraint] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ExtendedUserProfileFields') AND [name] ='InputControlAttribute')
ALTER TABLE [Max_ExtendedUserProfileFields] ALTER COLUMN [InputControlAttribute] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO


IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ExtendedUserProfileFields') AND [name] ='DisplayName')
ALTER TABLE [Max_ExtendedUserProfileFields] ALTER COLUMN [DisplayName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_FavoriteDirectories') AND [name] ='DirectoryName')
ALTER TABLE [Max_FavoriteDirectories] ALTER COLUMN [DirectoryName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_FavoriteLinks') AND [name] ='Subject')
ALTER TABLE [Max_FavoriteLinks] ALTER COLUMN [Subject] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_FavoriteLinks') AND [name] ='Url')
ALTER TABLE [Max_FavoriteLinks] ALTER COLUMN [Url] [nvarchar](512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL
GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_FavoriteLinks') AND [name] ='PostNickName')
ALTER TABLE [Max_FavoriteLinks] ALTER COLUMN [PostNickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL
GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Files') AND [name] ='ServerFileName')
ALTER TABLE [Max_Files] ALTER COLUMN [ServerFileName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Files') AND [name]='IX_Max_Files_MD5')
DROP INDEX [Max_Files].[IX_Max_Files_MD5]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Files') AND [name] ='MD5Code')
ALTER TABLE [Max_Files] ALTER COLUMN [MD5Code] [char](32) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Files') AND [name]='IX_Max_Files_MD5')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_Files_MD5] ON [Max_Files] 
(
	[MD5Code] ASC,
	[ContentLength] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Files') AND [name] ='ContentType')
ALTER TABLE [Max_Files] ALTER COLUMN [ContentType] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL
	
GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Files') AND [name] ='ExProFormat')
ALTER TABLE [Max_Files] ALTER COLUMN [ExProFormat] [nvarchar](2048) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_InviteSerials') AND [name] ='ToUserNickName')
ALTER TABLE [Max_InviteSerials] ALTER COLUMN [ToUserNickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_IpAddresses') AND [name] ='Location1')
ALTER TABLE [Max_IpAddresses] ALTER COLUMN [Location1] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_IpAddresses') AND [name] ='Location2')
ALTER TABLE [Max_IpAddresses] ALTER COLUMN [Location2] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO


IF OBJECT_ID('Max_LinkedApplications', 'U') IS NOT NULL AND EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='PK_Max_LinkedApplications')
ALTER TABLE [Max_LinkedApplications] DROP CONSTRAINT [PK_Max_LinkedApplications]

GO

IF OBJECT_ID('Max_LinkedApplications', 'U') IS NOT NULL AND EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_LinkedApplications') AND [name] ='ApplicationID')
ALTER TABLE [Max_LinkedApplications] ALTER COLUMN [ApplicationID] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF OBJECT_ID('Max_LinkedApplications', 'U') IS NOT NULL AND EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_LinkedApplications') AND [name] ='Url')
ALTER TABLE [Max_LinkedApplications] ALTER COLUMN [Url] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF OBJECT_ID('Max_LinkedApplications', 'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='PK_Max_LinkedApplications')
ALTER TABLE [Max_LinkedApplications] ADD  CONSTRAINT [PK_Max_LinkedApplications] PRIMARY KEY CLUSTERED 
(
	[ApplicationID] ASC,
	[Url] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_LinkedApplications') AND [name] ='DisplayName')
ALTER TABLE [Max_LinkedApplications] ALTER COLUMN [DisplayName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Medals') AND [name] ='MedalName')
ALTER TABLE [Max_Medals] ALTER COLUMN [MedalName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Medals') AND [name] ='LogoUrl')
ALTER TABLE [Max_Medals] ALTER COLUMN [LogoUrl] [nvarchar](512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Messages') AND [name] ='OtherNickName')
ALTER TABLE [Max_Messages] ALTER COLUMN [OtherNickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Messages') AND [name] ='Subject')
ALTER TABLE [Max_Messages] ALTER COLUMN [Subject] [nvarchar](512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_PasswordRecoveryLogs') AND [name] ='UserName')
ALTER TABLE [Max_PasswordRecoveryLogs] ALTER COLUMN [UserName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO


IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Roles') AND [name]='IX_Max_Roles_RoleName')
DROP INDEX [Max_Roles].[IX_Max_Roles_RoleName]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Roles') AND [name] ='RoleName')
ALTER TABLE [Max_Roles]  ALTER COLUMN [RoleName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Roles') AND [name]='IX_Max_Roles_RoleName')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_Roles_RoleName] ON [Max_Roles] 
(
	[RoleName] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Roles') AND [name] ='InternalName')
ALTER TABLE [Max_Roles]  ALTER COLUMN [InternalName] [nvarchar](512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Roles') AND [name] ='LogoUrl')
ALTER TABLE [Max_Roles]  ALTER COLUMN [LogoUrl] [nvarchar](512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Roles') AND [name] ='DisplayColor')
ALTER TABLE [Max_Roles]  ALTER COLUMN [DisplayColor] [nvarchar](16) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_SendMailQueue') AND [name] ='Subject')
ALTER TABLE [Max_SendMailQueue] ALTER COLUMN [Subject] [nvarchar](200) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO


IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_SendMailQueue') AND [name] ='ReceiveAddress')
ALTER TABLE [Max_SendMailQueue] ALTER COLUMN [ReceiveAddress] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_SignInLog') AND [name] ='UserName')
ALTER TABLE [Max_SignInLog] ALTER COLUMN [UserName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_SignInLog') AND [name] ='Password')
ALTER TABLE [Max_SignInLog] ALTER COLUMN [Password] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_SignInLog') AND [name] ='IPAddress')
ALTER TABLE [Max_SignInLog] ALTER COLUMN [IPAddress] [varchar](15) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_SignInLog') AND [name] ='UserAgent')
ALTER TABLE [Max_SignInLog] ALTER COLUMN [UserAgent] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='IX_Max_UserEmails_Email')
ALTER TABLE [Max_UserEmails] DROP CONSTRAINT [IX_Max_UserEmails_Email]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserEmails') AND [name] ='Email')
ALTER TABLE [Max_UserEmails] ALTER COLUMN [Email] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='IX_Max_UserEmails_Email')
ALTER TABLE [Max_UserEmails] ADD  CONSTRAINT [IX_Max_UserEmails_Email] UNIQUE CLUSTERED 
(
	[Email] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserOptions') AND [name] ='ThemeID')
ALTER TABLE [Max_UserOptions] ALTER COLUMN [ThemeID] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserOptions') AND [name] ='LanguageID')
ALTER TABLE [Max_UserOptions] ALTER COLUMN [LanguageID] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_UserProfiles') AND [name]='IX_Max_UserProfiles_NickName')
DROP INDEX [Max_UserProfiles].[IX_Max_UserProfiles_NickName]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserProfiles') AND [name] ='NickName')
ALTER TABLE [Max_UserProfiles] ALTER COLUMN [NickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserProfiles') AND [name] ='IX_Max_UserProfiles_NickName')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_UserProfiles_NickName] ON [Max_UserProfiles] 
(
	[NickName] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserProfiles') AND [name] ='PublicEmail')
ALTER TABLE [Max_UserProfiles] ALTER COLUMN [PublicEmail] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserProfiles') AND [name] ='Avatar')
ALTER TABLE [Max_UserProfiles] ALTER COLUMN [Avatar] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO
IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserProfiles') AND [name] ='CreateIP')
ALTER TABLE [Max_UserProfiles] ALTER COLUMN [CreateIP] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO
IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserProfiles') AND [name] ='LastSignInIP')
ALTER TABLE [Max_UserProfiles] ALTER COLUMN [LastSignInIP] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='IX_Max_Users_UserName')
ALTER TABLE [Max_Users] DROP CONSTRAINT [IX_Max_Users_UserName]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Users') AND [name] ='UserName')
ALTER TABLE [Max_Users] ALTER COLUMN [UserName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='IX_Max_Users_UserName')
ALTER TABLE [Max_Users] ADD  CONSTRAINT [IX_Max_Users_UserName] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Users') AND [name] ='Password')
ALTER TABLE [Max_Users] ALTER COLUMN [Password] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_Users') AND [name] ='Answer')
ALTER TABLE [Max_Users] ALTER COLUMN [Answer] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserSignInLogs') AND [name] ='UserName')
ALTER TABLE [Max_UserSignInLogs] ALTER COLUMN [UserName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserSignInLogs') AND [name] ='IPAddress')
ALTER TABLE [Max_UserSignInLogs] ALTER COLUMN [IPAddress] [nvarchar](32) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_UserSignInLogs') AND [name] ='UserAgent')
ALTER TABLE [Max_UserSignInLogs] ALTER COLUMN [UserAgent] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ValidateCodes') AND [name] ='CodeName')
ALTER TABLE [Max_ValidateCodes] ALTER COLUMN [CodeName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_ValidateCodes') AND [name] ='Copyright')
ALTER TABLE [Max_ValidateCodes] ALTER COLUMN [Copyright] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF OBJECT_ID('System_Max_AdvertisementCatalogs', 'U') IS NOT NULL AND EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('System_Max_AdvertisementCatalogs') AND [name]='IX_System_Max_AdvertisementCatalogs')
DROP INDEX [System_Max_AdvertisementCatalogs].[IX_System_Max_AdvertisementCatalogs]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_AdvertisementCatalogs') AND [name] ='CatalogKey')
ALTER TABLE [System_Max_AdvertisementCatalogs] ALTER COLUMN [CatalogKey] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF OBJECT_ID('System_Max_AdvertisementCatalogs', 'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('System_Max_AdvertisementCatalogs') AND [name]='IX_System_Max_AdvertisementCatalogs')
CREATE UNIQUE NONCLUSTERED INDEX [IX_System_Max_AdvertisementCatalogs] ON [System_Max_AdvertisementCatalogs] 
(
	[CatalogKey] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_AdvertisementCatalogs') AND [name] ='CatalogName')
ALTER TABLE [System_Max_AdvertisementCatalogs] ALTER COLUMN [CatalogName] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_Advertisements') AND [name] ='AdvertisementName')
ALTER TABLE [System_Max_Advertisements] ALTER COLUMN [AdvertisementName] [nvarchar](100) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO


IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_Jobs') AND [name] ='AssemblyName')
ALTER TABLE [System_Max_Jobs] ALTER COLUMN [AssemblyName] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_Jobs') AND [name] ='TypeName')
ALTER TABLE [System_Max_Jobs] ALTER COLUMN [TypeName] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='PK_System_Max_Settings')
ALTER TABLE [System_Max_Settings] DROP CONSTRAINT [PK_System_Max_Settings]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_Settings') AND [name] ='Catalog')
ALTER TABLE [System_Max_Settings] ALTER COLUMN [Catalog] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_Settings') AND [name] ='SettingKey')
ALTER TABLE [System_Max_Settings] ALTER COLUMN [SettingKey] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='PK_System_Max_Settings')
ALTER TABLE [System_Max_Settings] ADD  CONSTRAINT [PK_System_Max_Settings] PRIMARY KEY CLUSTERED 
(
	[Catalog] ASC,
	[SettingKey] ASC
) ON [PRIMARY]

GO


IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_Urls') AND [name] ='Url')
ALTER TABLE [System_Max_Urls] ALTER COLUMN [Url] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_Urls') AND [name] ='FriendlyUrl')
ALTER TABLE [System_Max_Urls] ALTER COLUMN [FriendlyUrl] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_Urls') AND [name] ='DefaultValues')
ALTER TABLE [System_Max_Urls] ALTER COLUMN [DefaultValues] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_MedalLogs') AND [name] ='IP')
ALTER TABLE [Max_MedalLogs] ADD [IP] [nvarchar](64) COLLATE Chinese_PRC_CI_AS NULL

GO

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_MedalLogs') AND [name] ='Handlers')
ALTER TABLE [Max_MedalLogs] ADD [Handlers] [int] NULL

GO

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_RoleLogs') AND [name] ='IP')
ALTER TABLE [Max_RoleLogs] ADD [IP] [nvarchar](64) COLLATE Chinese_PRC_CI_AS NULL

GO

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('Max_RoleLogs') AND [name] ='Handlers')
ALTER TABLE [Max_RoleLogs] ADD [Handlers] [int] NULL

GO

IF OBJECT_ID('System_Max_AdvertisementCatalogs', 'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_AdvertisementCatalogs') AND [name] ='OrderType')
ALTER TABLE [System_Max_AdvertisementCatalogs] ADD [OrderType] [tinyint] NOT NULL CONSTRAINT [DF_System_Max_AdvertisementCatalogs_OrderType]  DEFAULT ((0))

GO

IF OBJECT_ID('System_Max_Jobs', 'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_Max_Jobs') AND [name] ='ExecuteTimeString')
ALTER TABLE [System_Max_Jobs] ADD [ExecuteTimeString] [varchar](400) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF_System_Max_Jobs_ExecuteTimeString]  DEFAULT ('')

GO

IF OBJECT_ID('Max_ActivationSerials', 'U') IS NULL
CREATE TABLE [Max_ActivationSerials](
	[Serial] [uniqueidentifier] NOT NULL,
	[UserID] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF__Max_Activ__Creat__7D78A4E7]  DEFAULT (getdate()),
	[ExpiresDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Max_ActivationSerials] PRIMARY KEY CLUSTERED 
(
	[Serial] ASC,
	[UserID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

IF OBJECT_ID('Max_SuperVariables', 'U') IS NULL
CREATE TABLE [Max_SuperVariables](
	[Key] [nvarchar](256) COLLATE Chinese_PRC_CS_AS NOT NULL,
	[Type] [nvarchar](256) COLLATE Chinese_PRC_CS_AS NOT NULL,
	[Data] [image] NOT NULL,
 CONSTRAINT [PK_Max_SuperVariable] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_ActivationSerials') AND [name]='IX_Max_ActivationSerials_Expires')
CREATE NONCLUSTERED INDEX [IX_Max_ActivationSerials_Expires] ON [Max_ActivationSerials]
(
	[ExpiresDate] DESC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_ContactGroups') AND [name]='IX_Max_ContactGroups_List')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_ContactGroups_List] ON [Max_ContactGroups] 
(
	[UserID] ASC,
	[GroupName] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Contacts') AND [name]='IX_Max_Contacts_List')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_Contacts_List] ON [Max_Contacts] 
(
	[GroupID] ASC,
	[ContactUserID] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DefaultEmoticons') AND [name]='IX_Max_DefaultEmoticons_SortOrder')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_DefaultEmoticons_SortOrder] ON [Max_DefaultEmoticons] 
(
	[SortOrder] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DeleteFileQueue') AND [name]='IX_Max_DeleteFileQueue_TryTimes')
CREATE NONCLUSTERED INDEX [IX_Max_DeleteFileQueue_TryTimes] ON [Max_DeleteFileQueue] 
(
	[TryTimes] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DiskDirectories') AND [name]='IX_Max_DiskDirectories_List')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_DiskDirectories_List] ON [Max_DiskDirectories] 
(
	[UserID] ASC,
	[ParentID] ASC,
	[DirectoryName] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_DiskFiles') AND [name]='IX_Max_DiskFiles_List')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_DiskFiles_List] ON [Max_DiskFiles] 
(
	[DirectoryID] ASC,
	[FileName] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_EmoticonGroups') AND [name]='IX_Max_EmoticonGroups_List')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_EmoticonGroups_List] ON [Max_EmoticonGroups] 
(
	[UserID] ASC,
	[GroupName] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Emoticons') AND [name]='IX_Max_Emoticons_List')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_Emoticons_List] ON [Max_Emoticons] 
(
	[GroupID] ASC,
	[SortOrder] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_FavoriteDirectories') AND [name]='IX_Max_FavoriteDirectories_List')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_FavoriteDirectories_List] ON [Max_FavoriteDirectories] 
(
	[UserID] ASC,
	[DirectoryName] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_FavoriteLinks') AND [name]='IX_Max_FavoriteLinks_List')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_FavoriteLinks_List] ON [Max_FavoriteLinks] 
(
	[DirectoryID] ASC,
	[SortOrder] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_InviteSerials') AND [name]='IX_Max_InviteSerials_Expires')
CREATE NONCLUSTERED INDEX [IX_Max_InviteSerials_Expires] ON [Max_InviteSerials] 
(
	[ExpiresDate] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Phrases') AND [name]='IX_Max_Phrases_List')
CREATE NONCLUSTERED INDEX [IX_Max_Phrases_List] ON [Max_Phrases] 
(
	[UserID] ASC,
	[TotalClick] DESC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_Roles') AND [name]='IX_Max_Roles')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_Roles] ON [Max_Roles] 
(
	[SortOrder] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_SendMailQueue') AND [name]='IX_Max_SendMailQueue_SendCount')
CREATE NONCLUSTERED INDEX [IX_Max_SendMailQueue_SendCount] ON [Max_SendMailQueue] 
(
	[SendCount] DESC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('Max_UserMedals') AND [name]='IX_Max_UserMedals_UserList')
CREATE NONCLUSTERED INDEX [IX_Max_UserMedals_UserList] ON [Max_UserMedals] 
(
	[UserID] ASC,
	[CreateDate] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='FK_Max_UserMedals_Medals')
ALTER TABLE [Max_UserMedals]  WITH CHECK ADD  CONSTRAINT [FK_Max_UserMedals_Medals] FOREIGN KEY([MedalID])
REFERENCES [Max_Medals] ([MedalID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='FK_Max_Messages_Users')
ALTER TABLE [Max_Messages]  WITH CHECK ADD  CONSTRAINT [FK_Max_Messages_Users] FOREIGN KEY([UserID])
REFERENCES [Max_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE


--以下是bbsmax升级脚本

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='PK_bbsMax_ShieldedUsers')
ALTER TABLE [bbsMax_ShieldedUsers] DROP CONSTRAINT [PK_bbsMax_ShieldedUsers]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Attachments') AND [name]='IX_bbsMax_Attachments_List2')
DROP INDEX [bbsMax_Attachments].[IX_bbsMax_Attachments_List2]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_PostMarks') AND [name]='IX_bbsMax_PostMarks_List2')
DROP INDEX [bbsMax_PostMarks].[IX_bbsMax_PostMarks_List2]

GO
	
IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_QuestionRewards') AND [name]='IX_bbsMax_QuestionRewards_List1')
DROP INDEX [bbsMax_QuestionRewards].[IX_bbsMax_QuestionRewards_List1]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_ThreadManageLogs') AND [name]='IX_ThreadManageLog_UserID ActionType')
DROP INDEX [bbsMax_ThreadManageLogs].[IX_ThreadManageLog_UserID ActionType]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Forums') AND [name]='IX_bbsMax_Forums_SortOrder')
DROP INDEX [bbsMax_Forums].[IX_bbsMax_Forums_SortOrder]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Moderators') AND [name]='IX_bbsMax_Moderators_List')
DROP INDEX [bbsMax_Moderators].[IX_bbsMax_Moderators_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_PollItems') AND [name]='IX_bbsMax_PollItems_List')
DROP INDEX [bbsMax_PollItems].[IX_bbsMax_PollItems_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Posts') AND [name]='IX_bbsMax_Posts_SortOrder')
DROP INDEX [bbsMax_Posts].[IX_bbsMax_Posts_SortOrder]
 
GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_QuestionRewards') AND [name]='IX_bbsMax_QuestionRewards_List')
DROP INDEX [bbsMax_QuestionRewards].[IX_bbsMax_QuestionRewards_List]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Threads') AND [name]='IX_bbsMax_Threads_SortOrder')
DROP INDEX [bbsMax_Threads].[IX_bbsMax_Threads_SortOrder]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Users') AND [name]='IX_bbsMax_Users')
DROP INDEX [bbsMax_Users].[IX_bbsMax_Users]

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='DF_System_bbsMax_AdvertisementCatalogs_IsRandomSort')
ALTER TABLE [System_bbsMax_AdvertisementCatalogs] DROP CONSTRAINT [DF_System_bbsMax_AdvertisementCatalogs_IsRandomSort]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_AdvertisementCatalogs') AND [name] ='IsRandomSort')
ALTER TABLE [System_bbsMax_AdvertisementCatalogs] DROP COLUMN [IsRandomSort]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_Jobs') AND [name] ='ExecuteTime')
ALTER TABLE [System_bbsMax_Jobs] DROP COLUMN [ExecuteTime]

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='DF_bbsMax_Attachments_FileExtendedInfo')
ALTER TABLE [bbsMax_Attachments] DROP CONSTRAINT [DF_bbsMax_Attachments_FileExtendedInfo]

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='DF_bbsMax_Attachments_FileExtendedInfo')
ALTER TABLE [bbsMax_Attachments] ADD CONSTRAINT [DF_bbsMax_Attachments_FileExtendedInfo] DEFAULT (N'') FOR [FileExtendedInfo]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_ShieldedUsers') AND [name] ='ForumID')
ALTER TABLE [bbsMax_ShieldedUsers] ALTER COLUMN [ForumID] [int] NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Announcements') AND [name] ='PostUserName')
ALTER TABLE [bbsMax_Announcements] ALTER COLUMN [PostUserName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Announcements') AND [name] ='Subject')
ALTER TABLE [bbsMax_Announcements] ALTER COLUMN [Subject] [nvarchar](200) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO


IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Attachments') AND [name] ='FileName')
ALTER TABLE [bbsMax_Attachments] ALTER COLUMN [FileName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Attachments') AND [name] ='FileExtendedInfo')
ALTER TABLE [bbsMax_Attachments] ALTER COLUMN [FileExtendedInfo] [nvarchar](1000) COLLATE Chinese_PRC_CI_AS_WS NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_CurrencyExtendedPointRatio') AND [name] ='Currency1')
ALTER TABLE [bbsMax_CurrencyExtendedPointRatio] ALTER COLUMN [Currency1] [nvarchar](50) COLLATE Chinese_PRC_CI_AS_WS NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_CurrencyExtendedPointRatio') AND [name] ='Currency2')
ALTER TABLE [bbsMax_CurrencyExtendedPointRatio] ALTER COLUMN [Currency2] [nvarchar](50) COLLATE Chinese_PRC_CI_AS_WS NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_CurrencyExtendedPointRatio') AND [name] ='Currency3')
ALTER TABLE [bbsMax_CurrencyExtendedPointRatio] ALTER COLUMN [Currency3] [nvarchar](50) COLLATE Chinese_PRC_CI_AS_WS NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_ExtendedPoints') AND [name] ='PointName')
ALTER TABLE [bbsMax_ExtendedPoints] ALTER COLUMN [PointName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_ExtendedPoints') AND [name] ='PointUnit')
ALTER TABLE [bbsMax_ExtendedPoints] ALTER COLUMN [PointUnit] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Forums') AND [name]='IX_bbsMax_Forums_CodeName')
DROP INDEX [bbsMax_Forums].[IX_bbsMax_Forums_CodeName]

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Forums') AND [name]='CodeName')
ALTER TABLE [bbsMax_Forums] ALTER COLUMN [CodeName] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Forums') AND [name]='IX_bbsMax_Forums_CodeName')
CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Forums_CodeName] ON [bbsMax_Forums] 
(
	[CodeName] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Forums') AND [name] ='ForumName')
ALTER TABLE [bbsMax_Forums] ALTER COLUMN [ForumName] [nvarchar](1024) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Forums') AND [name] ='LogoUrl')
ALTER TABLE [bbsMax_Forums] ALTER COLUMN [LogoUrl] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Forums') AND [name] ='ThemeID')
ALTER TABLE [bbsMax_Forums] ALTER COLUMN [ThemeID] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Forums') AND [name] ='Password')
ALTER TABLE [bbsMax_Forums] ALTER COLUMN [Password] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Keywords') AND [name]='IX_bbsMax_KeyWord')
DROP INDEX [bbsMax_Keywords].[IX_bbsMax_KeyWord]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Keywords') AND [name] ='Find')
ALTER TABLE [bbsMax_Keywords] ALTER COLUMN [Find] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Keywords') AND [name]='IX_bbsMax_KeyWord')
CREATE UNIQUE CLUSTERED INDEX [IX_bbsMax_KeyWord] ON [bbsMax_Keywords] 
(
	[Find] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Keywords') AND [name] ='Replacement')
ALTER TABLE [bbsMax_Keywords] ALTER COLUMN [Replacement] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Links') AND [name] ='LinkName')
ALTER TABLE [bbsMax_Links] ALTER COLUMN [LinkName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Links') AND [name] ='LinkDescription')
ALTER TABLE [bbsMax_Links] ALTER COLUMN [LinkDescription] [nvarchar](4000) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Links') AND [name] ='Url')
ALTER TABLE [bbsMax_Links] ALTER COLUMN [Url] [nvarchar](512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Links') AND [name] ='LogoUrl')
ALTER TABLE [bbsMax_Links] ALTER COLUMN [LogoUrl] [nvarchar](512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_PointLevels') AND [name] ='LevelName')
ALTER TABLE [bbsMax_PointLevels] ALTER COLUMN [LevelName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_PointLevels') AND [name] ='LogoUrl')
ALTER TABLE [bbsMax_PointLevels] ALTER COLUMN [LogoUrl] [nvarchar](512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_PointSchemes') AND [name] ='SchemeType')
ALTER TABLE [bbsMax_PointSchemes] ALTER COLUMN [SchemeType] [varchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_PointSchemes') AND [name] ='SchemeName')
ALTER TABLE [bbsMax_PointSchemes] ALTER COLUMN [SchemeName] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO


IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_PollItemDetails') AND [name] ='NickName')
ALTER TABLE [bbsMax_PollItemDetails] ALTER COLUMN [NickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_PollItems') AND [name] ='ItemName')
ALTER TABLE [bbsMax_PollItems] ALTER COLUMN [ItemName] [nvarchar](200) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_PosterLevels') AND [name] ='LevelName')
ALTER TABLE [bbsMax_PosterLevels] ALTER COLUMN [LevelName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_PosterLevels') AND [name] ='LevelNameColor')
ALTER TABLE [bbsMax_PosterLevels] ALTER COLUMN [LevelNameColor] [nchar](7) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_PostIcons') AND [name] ='IconUrl')
ALTER TABLE [bbsMax_PostIcons] ALTER COLUMN [IconUrl] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_PostIndexAlias') AND [name] ='Alias')
ALTER TABLE [bbsMax_PostIndexAlias] ALTER COLUMN [Alias] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

--ALTER TABLE [bbsMax_Posts] ALTER COLUMN [Subject] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

--ALTER TABLE [bbsMax_Posts] ALTER COLUMN [Content] [ntext] COLLATE Chinese_PRC_CI_AS_WS NOT NULL 

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Posts') AND [name] ='NickName')
ALTER TABLE [bbsMax_Posts] ALTER COLUMN [NickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Posts') AND [name] ='LastEditor')
ALTER TABLE [bbsMax_Posts] ALTER COLUMN [LastEditor] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Posts') AND [name] ='IPAddress')
ALTER TABLE [bbsMax_Posts] ALTER COLUMN [IPAddress] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_RolesInOnline') AND [name] ='LogoUrl')
ALTER TABLE [bbsMax_RolesInOnline] ALTER COLUMN [LogoUrl] [nvarchar](512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_ThreadCatalogs') AND [name] ='ThreadCatalogName')
ALTER TABLE [bbsMax_ThreadCatalogs] ALTER COLUMN [ThreadCatalogName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL
	
GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_ThreadCatalogs') AND [name] ='LogoUrl')
ALTER TABLE [bbsMax_ThreadCatalogs] ALTER COLUMN [LogoUrl] [nvarchar](512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_ThreadCatalogs') AND [name] ='UserName')
ALTER TABLE [bbsMax_ThreadManageLogs] ALTER COLUMN [UserName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_ThreadCatalogs') AND [name] ='NickName')
ALTER TABLE [bbsMax_ThreadManageLogs] ALTER COLUMN [NickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_ThreadCatalogs') AND [name] ='IPAddress')
ALTER TABLE [bbsMax_ThreadManageLogs] ALTER COLUMN [IPAddress] [varchar](15) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_ThreadCatalogs') AND [name] ='Reason')
ALTER TABLE [bbsMax_ThreadManageLogs] ALTER COLUMN [Reason] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

--ALTER TABLE [bbsMax_Threads] ALTER COLUMN [Subject] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Threads') AND [name] ='SubjectStyle')
ALTER TABLE [bbsMax_Threads] ALTER COLUMN [SubjectStyle] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Threads') AND [name] ='PostNickName')
ALTER TABLE [bbsMax_Threads] ALTER COLUMN [PostNickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Threads') AND [name] ='LastPostNickName')
ALTER TABLE [bbsMax_Threads] ALTER COLUMN [LastPostNickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_Threads') AND [name] ='Perorate')
ALTER TABLE [bbsMax_Threads] ALTER COLUMN [Perorate] [nvarchar](32) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_UserOptions') AND [name] ='ThemeID')
ALTER TABLE [bbsMax_UserOptions] ALTER COLUMN [ThemeID] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_UserOptions') AND [name] ='LanguageID')
ALTER TABLE [bbsMax_UserOptions] ALTER COLUMN [LanguageID] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bbsMax_UserProfiles') AND [name] ='NickName')
ALTER TABLE [bbsMax_UserProfiles] ALTER COLUMN [NickName] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('System_bbsMax_AdvertisementCatalogs') AND [name]='IX_System_bbsMax_AdvertisementCatalogs')
DROP INDEX [System_bbsMax_AdvertisementCatalogs].[IX_System_bbsMax_AdvertisementCatalogs]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_AdvertisementCatalogs') AND [name] ='CatalogKey')
ALTER TABLE [System_bbsMax_AdvertisementCatalogs] ALTER COLUMN [CatalogKey] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF OBJECT_ID('System_bbsMax_AdvertisementCatalogs', 'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('System_bbsMax_AdvertisementCatalogs') AND [name]='IX_System_bbsMax_AdvertisementCatalogs')
CREATE UNIQUE NONCLUSTERED INDEX [IX_System_bbsMax_AdvertisementCatalogs] ON [System_bbsMax_AdvertisementCatalogs] 
(
	[CatalogKey] ASC
) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_AdvertisementCatalogs') AND [name] ='CatalogName')
ALTER TABLE [System_bbsMax_AdvertisementCatalogs] ALTER COLUMN [CatalogName] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_Advertisements') AND [name] ='AdvertisementName')
ALTER TABLE [System_bbsMax_Advertisements] ALTER COLUMN [AdvertisementName] [nvarchar](100) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_Jobs') AND [name] ='AssemblyName')
ALTER TABLE [System_bbsMax_Jobs] ALTER COLUMN [AssemblyName] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_Jobs') AND [name] ='TypeName')
ALTER TABLE [System_bbsMax_Jobs] ALTER COLUMN [TypeName] [nvarchar](128) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='PK_System_bbsMax_Settings')
ALTER TABLE [System_bbsMax_Settings] DROP CONSTRAINT [PK_System_bbsMax_Settings]

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_Settings') AND [name] ='Catalog')
ALTER TABLE [System_bbsMax_Settings] ALTER COLUMN [Catalog] [nvarchar](64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_Settings') AND [name] ='SettingKey')
ALTER TABLE [System_bbsMax_Settings] ALTER COLUMN [SettingKey] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO


IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='PK_System_bbsMax_Settings')
ALTER TABLE [System_bbsMax_Settings] ADD  CONSTRAINT [PK_System_bbsMax_Settings] PRIMARY KEY CLUSTERED 
(
	[Catalog] ASC,
	[SettingKey] ASC
) ON [PRIMARY]

GO


IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_Urls') AND [name] ='Url')
ALTER TABLE [System_bbsMax_Urls] ALTER COLUMN [Url] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_Urls') AND [name] ='FriendlyUrl')
ALTER TABLE [System_bbsMax_Urls] ALTER COLUMN [FriendlyUrl] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_Urls') AND [name] ='DefaultValues')
ALTER TABLE [System_bbsMax_Urls] ALTER COLUMN [DefaultValues] [nvarchar](256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL

GO

IF OBJECT_ID('System_bbsMax_AdvertisementCatalogs', 'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_AdvertisementCatalogs') AND [name] ='OrderType')
ALTER TABLE [System_bbsMax_AdvertisementCatalogs] ADD [OrderType] [tinyint] NOT NULL CONSTRAINT [DF_System_bbsMax_AdvertisementCatalogs_OrderType] DEFAULT ((0))

GO

IF OBJECT_ID('System_bbsMax_Jobs', 'U') IS NOT NULL AND NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('System_bbsMax_Jobs') AND [name] ='ExecuteTimeString')
ALTER TABLE [System_bbsMax_Jobs] ADD [ExecuteTimeString] [varchar](400) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF_System_bbsMax_Jobs_ExecuteTimeString] DEFAULT ('')

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('IX_bbsMax_Forums_SortOrder') AND [name]='bbsMax_Forums')
CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Forums_SortOrder] ON [bbsMax_Forums] 
(
	[ParentID] ASC,
	[SortOrder] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Moderators') AND [name]='IX_bbsMax_Moderators_List')
CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Moderators_List] ON [bbsMax_Moderators] 
(
	[ForumID] ASC,
	[SortOrder] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_PollItems') AND [name]='IX_bbsMax_PollItems_List')
CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_PollItems_List] ON [bbsMax_PollItems] 
(
	[ThreadID] ASC,
	[ItemID] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Posts') AND [name]='IX_bbsMax_Posts_SortOrder')
CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Posts_SortOrder] ON [bbsMax_Posts] 
(
	[SortOrder] DESC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_QuestionRewards') AND [name]='IX_bbsMax_QuestionRewards_List')
CREATE NONCLUSTERED INDEX [IX_bbsMax_QuestionRewards_List] ON [bbsMax_QuestionRewards] 
(
	[ThreadID] ASC,
	[Reward] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Threads') AND [name]='IX_bbsMax_Threads_SortOrder')
CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Threads_SortOrder] ON [bbsMax_Threads] 
(
	[SortOrder] DESC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_Users') AND [name]='IX_bbsMax_Users')
CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Users] ON [bbsMax_Users] 
(
	[idMaxID] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_ThreadManageLogs') AND [name]='IX_ThreadManageLog_UserID_ActionType')
CREATE NONCLUSTERED INDEX [IX_ThreadManageLog_UserID_ActionType] ON [bbsMax_ThreadManageLogs] 
(
	[UserID] ASC,
	[ActionType] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='PK_bbsMax_CurrencyExtendedPointRatio')
ALTER TABLE [bbsMax_CurrencyExtendedPointRatio] ADD  CONSTRAINT [PK_bbsMax_CurrencyExtendedPointRatio] PRIMARY KEY CLUSTERED 
(
	[ExtendedPointID] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysindexes WHERE [id]=object_id('bbsMax_ShieldedUsers') AND [name]='IX_bbsMax_ShieldedUsers')
CREATE CLUSTERED INDEX [IX_bbsMax_ShieldedUsers] ON [bbsMax_ShieldedUsers] 
(
	[ForumID] ASC,
	[UserID] ASC
) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='FK_bbsMax_ThreadCatalogsInForums_bbsMax_Forums')
ALTER TABLE [bbsMax_ThreadCatalogsInForums]  WITH CHECK ADD  CONSTRAINT [FK_bbsMax_ThreadCatalogsInForums_bbsMax_Forums] FOREIGN KEY([ForumID])
REFERENCES [bbsMax_Forums] ([ForumID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='FK_bbsMax_ThreadCatalogsInForums_bbsMax_ThreadCatalogs')
ALTER TABLE [bbsMax_ThreadCatalogsInForums]  WITH CHECK ADD  CONSTRAINT [FK_bbsMax_ThreadCatalogsInForums_bbsMax_ThreadCatalogs] FOREIGN KEY([ThreadCatalogID])
REFERENCES [bbsMax_ThreadCatalogs] ([ThreadCatalogID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO

IF NOT EXISTS(SELECT * FROM sysconstraints WHERE object_name(sysconstraints.constid)='FK_bbsMax_Threads_Forums')
ALTER TABLE [bbsMax_Threads]  WITH CHECK ADD  CONSTRAINT [FK_bbsMax_Threads_Forums] FOREIGN KEY([ForumID])
REFERENCES [bbsMax_Forums] ([ForumID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO