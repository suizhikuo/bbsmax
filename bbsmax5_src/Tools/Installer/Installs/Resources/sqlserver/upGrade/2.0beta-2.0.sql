ALTER TABLE [bbsMax_Keywords] DROP CONSTRAINT [FK_bbsMax_KeyWord_bbsMax_KeyWord]

GO

ALTER TABLE [bbsMax_PointActions] DROP CONSTRAINT	[FK_System_bbsMax_PointActions_System_bbsMax_PointSchemes]

GO

ALTER TABLE [bbsMax_PointSchemes] DROP CONSTRAINT	[FK_System_bbsMax_PointSchemes_System_bbsMax_PointSchemes]

GO

ALTER TABLE [Max_Messages] DROP CONSTRAINT [FK_Max_Messages_MessageDirectories]

GO

ALTER TABLE [bbsMax_Posts] DROP CONSTRAINT [FK_bbsMax_Posts_Threads]

GO

DROP TABLE [Max_MessageDirectories]

GO

DROP TABLE [Max_UserBirthday]

GO

ALTER TABLE [bbsMax_PointActions] DROP CONSTRAINT [PK_System_bbsMax_PointActions]

GO

ALTER TABLE [bbsMax_PollItems] DROP CONSTRAINT [FK_bbsMax_PollItem_Poll]

GO

ALTER TABLE [bbsMax_Polls] DROP CONSTRAINT [PK_Poll]

GO

ALTER TABLE [Max_Contacts] DROP CONSTRAINT [FK_Max_Contacts_ContactGroups]

GO

ALTER TABLE [Max_ContactGroups] DROP CONSTRAINT [PK_MAX_ContactsGroups]

GO

ALTER TABLE [Max_Contacts] DROP CONSTRAINT [PK_MAX_Contacts]

GO

ALTER TABLE [Max_Messages] DROP CONSTRAINT [PK_Max_Messages]

GO

ALTER TABLE [bbsMax_Threads] DROP CONSTRAINT [IX_bbsMax_Threads_SortOrder]

GO

DROP INDEX [bbsMax_Posts].[IX_bbsMax_Posts_List]

GO

DROP INDEX [bbsMax_Posts].[IX_bbsMax_Posts_List2]

GO

DROP INDEX [bbsMax_Posts].[IX_bbsMax_Posts_ListByUser]

GO

DROP INDEX [bbsMax_Posts].[IX_bbsMax_Posts_PostType]

GO

DROP INDEX [bbsMax_Threads].[IX_bbsMax_Threads_List]

GO

DROP INDEX [bbsMax_Threads].[IX_bbsMax_Threads_List2]

GO

DROP INDEX [bbsMax_Threads].[IX_bbsMax_Threads_Poster]

GO

DROP INDEX [bbsMax_Threads].[IX_bbsMax_Threads_ThreadCatalog]

GO

DROP INDEX [bbsMax_Threads].[IX_bbsMax_Threads_ThreadType]

GO

DROP INDEX [Max_ContactGroups].[IX_MAX_ContactGroups_List]

GO

DROP INDEX [Max_ContactGroups].[IX_MAX_ContactGroups_List2]

GO

DROP INDEX [Max_Contacts].[IX_MAX_Contacts]

GO

DROP INDEX [Max_Contacts].[IX_MAX_Contacts_List]

GO

DROP INDEX [Max_Messages].[IX_Max_Messages_List2]

GO

DROP INDEX [Max_Messages].[IX_Max_Messages_Type]

GO

DROP INDEX [bbsMax_PointLevels].[IX_bbsMax_PointLevels]

GO

DROP INDEX [bbsMax_Threads].[IX_bbsMax_Threads_Valued]

GO

DROP INDEX [Max_DefaultEmoticons].[IX_Max_DefaultEmoticons_Shortcut]

GO

DROP INDEX [Max_Messages].[IX_Max_Messages_List]

GO

ALTER TABLE [bbsMax_Forums] DROP CONSTRAINT [DF_bbsMax_Forums_ForumSettingID]

GO

ALTER TABLE [bbsMax_Forums] DROP COLUMN [ForumSettingID]

GO

ALTER TABLE [bbsMax_Posts] DROP CONSTRAINT [DF_bbsMax_Posts_EnableEmoticons]

GO

ALTER TABLE [bbsMax_Posts] DROP COLUMN [EnableEmoticons]

GO

ALTER TABLE [bbsMax_Posts] DROP CONSTRAINT [DF_bbsMax_Posts_EnableHTML]

GO

ALTER TABLE [bbsMax_Posts] DROP COLUMN [EnableHTML]

GO

ALTER TABLE [bbsMax_Posts] DROP CONSTRAINT [DF_bbsMax_Posts_EnableMaxCode]

GO

ALTER TABLE [bbsMax_Posts] DROP COLUMN [EnableMaxCode]

GO

ALTER TABLE [bbsMax_Posts] DROP CONSTRAINT [DF_bbsMax_Posts_IsApproved]

GO

ALTER TABLE [bbsMax_Posts] DROP COLUMN [IsApproved]

GO

ALTER TABLE [bbsMax_Threads] DROP CONSTRAINT [DF_bbsMax_Threads_BackupForumID]

GO

ALTER TABLE [bbsMax_Threads] DROP COLUMN [BackupForumID]

GO

ALTER TABLE [bbsMax_Threads] DROP CONSTRAINT [DF_bbsMax_Threads_ThreadStatus]

GO

ALTER TABLE [bbsMax_Threads] DROP COLUMN [ThreadStatus]

GO

ALTER TABLE [bbsMax_UserOptions] DROP CONSTRAINT [DF_bbsMax_UserOptions_StyleID]

GO

ALTER TABLE [bbsMax_UserOptions] DROP COLUMN [StyleID]

GO

ALTER TABLE [Max_Messages] DROP COLUMN [DirectoryID]

GO

ALTER TABLE [Max_UserEmails] DROP CONSTRAINT [DF_Max_UserEmails_ReceiveSystemEmails]

GO

ALTER TABLE [Max_UserEmails] DROP COLUMN [ReceiveSystemEmails]

GO

ALTER TABLE [Max_UserProfiles] DROP CONSTRAINT [DF_Max_UserProfiles_Birthday]

GO

ALTER TABLE [Max_UserProfiles] DROP COLUMN [Birthday]

GO

ALTER TABLE [bbsMax_Forums] DROP COLUMN [ExtendedAttributes]

GO

ALTER TABLE [System_bbsMax_Permissions] DROP COLUMN [PermissionItems]

GO

ALTER TABLE [System_Max_Permissions] DROP COLUMN [PermissionItems]

GO

ALTER TABLE [bbsMax_Posts] ALTER COLUMN [LastEditorID] [int] NOT NULL

GO

ALTER TABLE [bbsMax_Posts] DROP CONSTRAINT [DF_bbsMax_Posts_LastEditor]

GO

ALTER TABLE [bbsMax_Posts] ALTER COLUMN [LastEditor] [nvarchar](64) NOT NULL

GO

ALTER TABLE [bbsMax_Posts] ADD  CONSTRAINT [DF_bbsMax_Posts_LastEditor] DEFAULT (N'') FOR [LastEditor]

GO

ALTER TABLE [bbsMax_UserOptions] ALTER COLUMN [ThemeID] [nvarchar](64) NOT NULL

GO

ALTER TABLE [bbsMax_UserOptions] ALTER COLUMN [LanguageID] [nvarchar](64) NOT NULL

GO

ALTER TABLE [bbsMax_UserOptions] DROP CONSTRAINT [DF_bbsMax_UserOptions_ExtendedOptions]

GO

ALTER TABLE [bbsMax_UserOptions] ALTER COLUMN [ExtendedOptions] [nvarchar](4000) COLLATE Chinese_PRC_CI_AS NULL

GO

ALTER TABLE [Max_DefaultEmoticons] ALTER COLUMN [Shortcut] [nvarchar](64) NOT NULL

GO

ALTER TABLE [Max_Messages] DROP CONSTRAINT [DF_Max_Messages_ContentFormat]

GO

ALTER TABLE [Max_Messages] ADD CONSTRAINT [DF_Max_Messages_ContentFormat] DEFAULT ((10)) FOR [ContentFormat]

GO

ALTER TABLE [Max_UserOptions] DROP CONSTRAINT [DF_Max_UserOptions_MessageSound]

GO

ALTER TABLE [Max_UserOptions] ALTER COLUMN [MessageSound] [nvarchar](256) COLLATE Chinese_PRC_CI_AS NOT NULL 

GO

ALTER TABLE [Max_UserOptions] ALTER COLUMN [ExtendedOptions] [nvarchar](4000) COLLATE Chinese_PRC_CI_AS NULL

GO

DELETE FROM [System_Max_PermissionSchemes]

GO

DELETE FROM [System_bbsMax_Permissions]

GO

ALTER TABLE [System_bbsMax_Permissions] ADD [PermissionItems] [ntext] COLLATE Chinese_PRC_CI_AS NOT NULL

GO

ALTER TABLE [bbsMax_Forums] ADD [ExtendedAttributes] [ntext] COLLATE Chinese_PRC_CI_AS NULL

GO

ALTER TABLE [bbsMax_Attachments] ADD [FileID] [int] NOT NULL

GO

ALTER TABLE [bbsMax_Forums] ADD [YestodayLastThreadID] [int] NOT NULL CONSTRAINT [DF_bbsMax_Forums_YestodayLastThreadID] DEFAULT ((0))

GO

-- ALTER TABLE [bbsMax_Forums] ADD  CONSTRAINT [DF_bbsMax_Forums_YestodayLastThreadID] DEFAULT ((0)) FOR [YestodayLastThreadID]

GO

ALTER TABLE [bbsMax_Forums] ADD [YestodayLastPostID] [int] NOT NULL CONSTRAINT [DF_bbsMax_Forums_YestodayLastPostID] DEFAULT ((0))

GO

-- ALTER TABLE [bbsMax_Forums] ADD  CONSTRAINT [DF_bbsMax_Forums_YestodayLastPostID] DEFAULT ((0)) FOR [YestodayLastPostID]

GO

ALTER TABLE [bbsMax_Moderators] ADD [CreateDate] [datetime] NOT NULL

GO

ALTER TABLE [bbsMax_Moderators] ADD  CONSTRAINT [DF_bbsMax_Moderators_CreateDate] DEFAULT (getdate()) FOR [CreateDate]

GO

ALTER TABLE [bbsMax_Moderators] ADD [ExpiresDate] [datetime] NOT NULL

GO

ALTER TABLE [bbsMax_Moderators] ADD  CONSTRAINT [DF_bbsMax_Moderators_ExpiresDate] DEFAULT ('9999-12-31 23:59:59') FOR [ExpiresDate]

GO

ALTER TABLE [bbsMax_PointLevels] ADD [RequireRoleID] [int] NOT NULL CONSTRAINT [DF_bbsMax_PointLevels_RequireRoleID] DEFAULT ((0))

GO

-- ALTER TABLE [bbsMax_PointLevels] ADD  CONSTRAINT [DF_bbsMax_PointLevels_RequireRoleID] DEFAULT ((0)) FOR [RequireRoleID]

GO

ALTER TABLE [bbsMax_Posts] ADD [ForumID] [int] NOT NULL

GO

ALTER TABLE [bbsMax_Posts] ADD [ContentFormat] [tinyint] NOT NULL

GO

ALTER TABLE [bbsMax_Posts] ADD  CONSTRAINT [DF_bbsMax_Posts_ContentFormat] DEFAULT ((10)) FOR [ContentFormat]

GO

ALTER TABLE [bbsMax_Posts] ADD [IsShielded] [bit] NOT NULL

GO

ALTER TABLE [bbsMax_Posts] ADD  CONSTRAINT [DF_bbsMax_Posts_IsShielded] DEFAULT ((0)) FOR [IsShielded]

GO

ALTER TABLE [bbsMax_Threads] ADD [TotalAttachments] [int] NOT NULL

GO

ALTER TABLE [bbsMax_Threads] ADD  CONSTRAINT [DF_bbsMax_Threads_TotalAttachments] DEFAULT ((0)) FOR [TotalAttachments]

GO

-- ALTER TABLE [bbsMax_Threads] ADD [SortOrder] [bigint] NOT NULL

GO

ALTER TABLE [bbsMax_Threads] ADD [ThreadLog] [nvarchar](128) NOT NULL

GO

ALTER TABLE [bbsMax_Threads] ADD  CONSTRAINT [DF_bbsMax_Threads_ThreadLog] DEFAULT ('') FOR [ThreadLog]

GO

ALTER TABLE [bbsMax_Posts] ADD [SortOrder] [bigint] NOT NULL

GO

ALTER TABLE [bbsMax_Questions] ADD [IsClosed] [bit] NOT NULL

GO

ALTER TABLE [bbsMax_Questions] ADD  CONSTRAINT [DF_bbsMax_Questions_IsClosed] DEFAULT ((0)) FOR [IsClosed]

GO

ALTER TABLE [bbsMax_ThreadCatalogsInForums] ADD [TotalThreads] [int] NOT NULL CONSTRAINT [DF_bbsMax_ThreadCatalogsInForums_TotalThreads] DEFAULT ((0))

GO

-- ALTER TABLE [bbsMax_ThreadCatalogsInForums] ADD  CONSTRAINT [DF_bbsMax_ThreadCatalogsInForums_TotalThreads] DEFAULT ((0)) FOR [TotalThreads]

GO

ALTER TABLE [Max_InviteSerials] ADD [ToEmail] [nvarchar](128) NULL

GO

ALTER TABLE [Max_InviteSerials] ADD [ToUserID] [int] NOT NULL

GO

ALTER TABLE [Max_InviteSerials] ADD  CONSTRAINT [DF_Max_InviteSerials_ToUserID] DEFAULT ((0)) FOR [ToUserID]

GO

ALTER TABLE [Max_InviteSerials] ADD [ToUserNickName] [nvarchar](64) NOT NULL

GO

ALTER TABLE [Max_InviteSerials] ADD  CONSTRAINT [DF_Max_InviteSerials_ToUserNickName] DEFAULT (N'') FOR [ToUserNickName]

GO

ALTER TABLE [Max_InviteSerials] ADD [IsAccept] [bit] NULL

GO

ALTER TABLE [Max_Messages] ADD [UserID] [int] NOT NULL

GO

ALTER TABLE [Max_Messages] ADD [DirectoryType] [tinyint] NOT NULL

GO

ALTER TABLE [Max_UserProfiles] ADD [BirthdayYear] [smallint] NOT NULL CONSTRAINT [DF_Max_UserProfiles_BirthdayYear]  DEFAULT ((1753))

GO

-- ALTER TABLE [Max_UserProfiles] ADD  CONSTRAINT [DF_Max_UserProfiles_BirthdayYear]  DEFAULT ((1753)) FOR [BirthdayYear]

GO

ALTER TABLE [Max_UserProfiles] ADD [BirthdayDate] [smallint] NOT NULL CONSTRAINT [DF_Max_UserProfiles_BirthdayDate]  DEFAULT ((101))

GO

-- ALTER TABLE [Max_UserProfiles] ADD  CONSTRAINT [DF_Max_UserProfiles_BirthdayDate]  DEFAULT ((101)) FOR [BirthdayDate]

GO

ALTER TABLE [Max_UserProfiles] ADD [SignatureContentFormat] [tinyint] NOT NULL CONSTRAINT [DF_Max_UserProfiles_SignatureContentFormat]  DEFAULT ((0))

GO

-- ALTER TABLE [Max_UserProfiles] ADD  CONSTRAINT [DF_Max_UserProfiles_SignatureContentFormat]  DEFAULT ((0)) FOR [SignatureContentFormat]

GO

ALTER TABLE [Max_UserProfiles] ADD [LastSystemMessageID] [int] NOT NULL CONSTRAINT [DF_Max_UserProfiles_LastSystemMessageID]  DEFAULT ((0))

GO

-- ALTER TABLE [Max_UserProfiles] ADD  CONSTRAINT [DF_Max_UserProfiles_LastSystemMessageID]  DEFAULT ((0)) FOR [LastSystemMessageID]

GO

ALTER TABLE [Max_UserProfiles] ADD [UnreadMessages] [int] NOT NULL CONSTRAINT [DF_Max_UserProfiles_UnreadMessages]  DEFAULT ((-1))

GO

-- ALTER TABLE [Max_UserProfiles] ADD  CONSTRAINT [DF_Max_UserProfiles_UnreadMessages]  DEFAULT ((-1)) FOR [UnreadMessages]

GO

CREATE TABLE [bbsMax_ShieldedUsers](
	[ForumID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_bbsMax_ShieldedUsers] PRIMARY KEY CLUSTERED 
(
	[ForumID] ASC,
	[UserID] ASC
) )

GO

CREATE TABLE [bbsMax_ShieldUserLogs](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[ShieldedUserID] [int] NOT NULL,
	[ForumID] [int] NOT NULL,
	[IsShielded] [bit] NOT NULL CONSTRAINT [DF_bbsMax_ShieldUserLogs_IsShielded] DEFAULT ((1)),
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bbsMax_ShieldUserLogs_CreateDate] DEFAULT (getdate()),
 CONSTRAINT [PK_bbsMax_ShieldUserLogs] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
) )

GO

CREATE TABLE [Max_SystemMessages](
	[MessageID] [int] IDENTITY(1,1) NOT NULL,
	[Roles] [nvarchar](100) NOT NULL,
	[Subject] [nvarchar](512) NOT NULL,
	[Content] [ntext] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Max_SystemMessages] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
) )

GO

CREATE TABLE [System_bbsMax_FaqCatalogs](
	[FaqCatalogID] [int] IDENTITY(1,1) NOT NULL,
	[FaqCatalogName] [nvarchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_System_bbsMax_FaqCatalogs] PRIMARY KEY CLUSTERED 
(
	[FaqCatalogID] ASC
) )

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_System_bbsMax_FaqCatalogs] ON [System_bbsMax_FaqCatalogs] 
(
	[SortOrder] ASC
) 

GO

CREATE TABLE [System_bbsMax_Faqs](
	[FaqID] [int] IDENTITY(1,1) NOT NULL,
	[FaqCatalogID] [int] NOT NULL,
	[FaqTitle] [nvarchar](500) NOT NULL,
	[FaqContent] [ntext] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_System_bbsMax_Faqs] PRIMARY KEY CLUSTERED 
(
	[FaqID] ASC
) 
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_System_bbsMax_Faqs] ON [System_bbsMax_Faqs]
(
	[FaqCatalogID] ASC,
	[SortOrder] ASC
) 

GO

CREATE TABLE [System_Max_FaqCatalogs](
	[FaqCatalogID] [int] IDENTITY(1,1) NOT NULL,
	[FaqCatalogName] [nvarchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_System_Max_FaqCatalogs] PRIMARY KEY CLUSTERED
(
	[FaqCatalogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_System_Max_FaqCatalogs] ON [System_Max_FaqCatalogs] 
(
	[SortOrder] ASC
) 

GO

CREATE TABLE [System_Max_Faqs](
	[FaqID] [int] IDENTITY(1,1) NOT NULL,
	[FaqCatalogID] [int] NOT NULL,
	[FaqTitle] [nvarchar](500) NOT NULL,
	[FaqContent] [ntext] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_System_Max_Faqs] PRIMARY KEY CLUSTERED
(
	[FaqID] ASC
) 
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_System_Max_Faqs] ON [System_Max_Faqs]
(
	[FaqCatalogID] ASC,
	[SortOrder] ASC
) 

GO

CREATE NONCLUSTERED INDEX [IX_bbsMax_Polls_ExpiresDate] ON [bbsMax_Polls] 
(
	[ExpiresDate] ASC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Posts_Forum] ON [bbsMax_Posts] 
(
	[ForumID] ASC,
	[SortOrder] DESC
) 

GO

CREATE UNIQUE CLUSTERED INDEX [IX_bbsMax_Posts_SortOrder] ON [bbsMax_Posts] 
(
	[SortOrder] DESC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Posts_Thread] ON [bbsMax_Posts] 
(
	[ThreadID] DESC,
	[SortOrder] DESC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Posts_User] ON [bbsMax_Posts] 
(
	[UserID] ASC,
	[ThreadID] DESC,
	[SortOrder] DESC
) 

GO

CREATE NONCLUSTERED INDEX [IX_bbsMax_Questions_ExpiresDate] ON [bbsMax_Questions] 
(
	[IsClosed] ASC,
	[ExpiresDate] ASC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Threads_Catalog] ON [bbsMax_Threads] 
(
	[ForumID] ASC,
	[ThreadCatalogID] ASC,
	[SortOrder] DESC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Threads_Forum] ON [bbsMax_Threads] 
(
	[ForumID] ASC,
	[SortOrder] DESC
) 

GO

CREATE UNIQUE CLUSTERED INDEX [IX_bbsMax_Threads_SortOrder] ON [bbsMax_Threads] 
(
	[SortOrder] DESC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Threads_Type] ON [bbsMax_Threads] 
(
	[ThreadType] ASC,
	[ForumID] ASC,
	[SortOrder] DESC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Threads_User] ON [bbsMax_Threads] 
(
	[PostUserID] ASC,
	[ForumID] ASC,
	[SortOrder] DESC
) 

GO

CREATE UNIQUE CLUSTERED INDEX [IX_Max_ContactGroups_List] ON [Max_ContactGroups] 
(
	[UserID] ASC,
	[GroupName] ASC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_ContactGroups_List2] ON [Max_ContactGroups] 
(
	[UserID] ASC,
	[GroupName] ASC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_Contacts] ON [Max_Contacts] 
(
	[GroupID] ASC,
	[ContactUserID] ASC
) 

GO

CREATE CLUSTERED INDEX [IX_Max_Contacts_List] ON [Max_Contacts] 
(
	[GroupID] ASC,
	[ContactNickName] ASC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_Files_MD5] ON [Max_Files] 
(
	[MD5Code] ASC,
	[ContentLength] ASC
) 

GO

CREATE NONCLUSTERED INDEX [IX_Max_UserProfiles_Birthday] ON [Max_UserProfiles] 
(
	[BirthdayDate] ASC
) 

GO

CREATE NONCLUSTERED INDEX [IX_bbsMax_PointLevels] ON [bbsMax_PointLevels] 
(
	[RequirePoints] ASC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bbsMax_Threads_Valued] ON [bbsMax_Threads] 
(
	[IsValued] ASC,
	[ForumID] ASC,
	[SortOrder] DESC
) 

GO

CREATE NONCLUSTERED INDEX [IX_Max_DefaultEmoticons_Shortcut] ON [Max_DefaultEmoticons] 
(
	[Shortcut] ASC
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Max_Messages_List] ON [Max_Messages] 
(
	[UserID] ASC,
	[DirectoryType] ASC,
	[MessageType] ASC,
	[MessageID] ASC
) 

GO

ALTER TABLE [Max_Messages] ADD  CONSTRAINT [PK_Max_Messages] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[MessageID] ASC
)

GO

ALTER TABLE [bbsMax_PointActions] ADD  CONSTRAINT [PK_bbsMax_PointActions] PRIMARY KEY CLUSTERED 
(
	[PointSchemeID] ASC,
	[RoleID] ASC,
	[ActionType] ASC
)

GO

ALTER TABLE [bbsMax_Polls] ADD  CONSTRAINT [PK_bbsMax_Poll] PRIMARY KEY CLUSTERED 
(
	[ThreadID] ASC
)

GO

ALTER TABLE [bbsMax_PollItems]  WITH CHECK ADD  CONSTRAINT [FK_bbsMax_PollItem_Poll] FOREIGN KEY([ThreadID])
REFERENCES [bbsMax_Polls] ([ThreadID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO

ALTER TABLE [Max_ContactGroups] ADD  CONSTRAINT [PK_Max_ContactsGroups] PRIMARY KEY NONCLUSTERED 
(
	[GroupID] ASC
)

GO

ALTER TABLE [Max_Contacts]  WITH CHECK ADD  CONSTRAINT [FK_Max_Contacts_ContactGroups] FOREIGN KEY([GroupID])
REFERENCES [Max_ContactGroups] ([GroupID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO

ALTER TABLE [Max_Contacts] ADD  CONSTRAINT [PK_Max_Contacts] PRIMARY KEY NONCLUSTERED 
(
	[ContactID] ASC
)

GO

ALTER TABLE [bbsMax_Posts]  WITH CHECK ADD  CONSTRAINT [FK_bbsMax_Posts_Threads] FOREIGN KEY([ThreadID])
REFERENCES [bbsMax_Threads] ([ThreadID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO

ALTER TABLE [bbsMax_PointActions]  WITH CHECK ADD  CONSTRAINT [FK_bbsMax_PointActions_PointSchemes] FOREIGN KEY([PointSchemeID])
REFERENCES [bbsMax_PointSchemes] ([SchemeID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO