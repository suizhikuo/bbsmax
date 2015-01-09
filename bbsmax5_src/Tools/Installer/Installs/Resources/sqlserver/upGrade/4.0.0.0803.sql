


--关闭所有主键插入

declare @tname varchar(255);
declare @sql varchar(255);
declare dropsql_cursor cursor for
select [name] FROM sysobjects WHERE [type] = 'U'
  
open dropsql_cursor
  
fetch dropsql_cursor into @tname
  
while @@fetch_status=0   
begin  

	IF ident_current(@tname) IS NOT NULL BEGIN
		SET @sql = 'SET IDENTITY_INSERT [' + @tname + '] OFF';
		execute(@sql);
	END
  
    fetch dropsql_cursor into @tname   
  
end  
close dropsql_cursor
deallocate dropsql_cursor

GO
--end

--删除所有外键

declare @sql varchar(255)   
declare dropsql_cursor cursor for    
select 'alter table '+object_name(fkeyid)+' drop constraint '+object_name(constid)+char(10) from sysreferences   
  
open dropsql_cursor
  
fetch dropsql_cursor into @sql   
  
while @@fetch_status=0   
begin  
       
    execute(@sql)   
       
    if @@error <> 0   
    begin  
        rollback  
        return  
    end  
  
    fetch dropsql_cursor into @sql   
  
end  
close dropsql_cursor
deallocate dropsql_cursor

GO
--end

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_Users') BEGIN

	DELETE bx_Users WHERE UserID > 0;
	SET IDENTITY_INSERT [bx_Users] ON;
	INSERT INTO [bx_Users]
           ([UserID]
           ,[Username]
           ,[Realname]
           ,[Password]
           ,[PasswordFormat]
           ,[Email]
           ,[EmailValidated]
           ,[PublicEmail]
           ,[SpaceTheme]
           ,[Doing]
           ,[DoingDate]
           ,[AvatarSrc]
           ,[LastAvatarUpdateDate]
           ,[Gender]
           ,[Signature]
           ,[SignatureFormat]
           ,[FriendGroups]
           ,[CreateIP]
           ,[LastVisitIP]
           ,[LastVisitDate]
           ,[LastPostDate]
           ,[CreateDate]
           ,[UpdateDate]
           ,[SpaceViews]
           ,[LoginCount]
           ,[IsActive]
           ,[Points]
           ,[Point_1]
           ,[Point_2]
           ,[Point_3]
           ,[Point_4]
           ,[Point_5]
           ,[Point_6]
           ,[Point_7]
           ,[Point_8]
           ,[TotalInvite]
           ,[TotalTopics]
           ,[TotalPosts]
           ,[TotalComments]
           ,[TotalShares]
           ,[TotalCollections]
           ,[ValuedTopics]
           ,[DeletedTopics]
           ,[DeletedReplies]
           ,[TotalBlogArticles]
           ,[TotalAlbums]
           ,[TotalPhotos]
           ,[TotalDoings]
           ,[TotalOnlineTime]
           ,[MonthOnlineTime]
           ,[ExtendedData]
           ,[ExtendedFieldVersion]
           ,[UserInfo]
           ,[UsedDiskSpaceSize]
           ,[TotalDiskFiles]
           ,[KeywordVersion])
     SELECT
            u.[UserID]
           ,[UserName]
           ,''   --Realname
           ,[Password]  --Password
           ,[PasswordFormat]  --PasswordFormat
           ,SUBSTRING(ISNULL([Email],''),1,200)  --<Email>
           ,0             --<EmailValidated, bit,>
           ,SUBSTRING(ISNULL(PublicEmail,''),1,200)  --<PublicEmail, nvarchar(200),>
           ,''--<SpaceTheme, nvarchar(200),>
           ,SUBSTRING(ISNULL(ShortSignature,''),1,200)--<Doing, nvarchar(200),>
           ,getdate()--<DoingDate, datetime,>
           ,SUBSTRING(ISNULL(Avatar,''),1,200)--<AvatarSrc, nvarchar(200),>
           ,getdate()--<LastAvatarUpdateDate, datetime,>
           ,ISNULL(Gender,0)--<Gender, tinyint,>
           ,SUBSTRING(ISNULL([Signature],''),1,1500)--<Signature, nvarchar(1500),>
           ,ISNULL([SignatureContentFormat],0)--<SignatureFormat, tinyint,>
           ,''--<FriendGroups, nvarchar(500),>
           ,ISNULL(CreateIP,'')--<CreateIP, varchar(50),>
           ,ISNULL(LastSignInIP,'')--<LastVisitIP, varchar(50),>
           ,ISNULL(LastUpdateOnlineTime,getdate())--<LastVisitDate, datetime,>
           ,ISNULL(LastCreatePostDate,getdate())--<LastPostDate, datetime,>
           ,ISNULL(Max_UserProfiles.CreateDate,getdate())--<CreateDate, datetime,>
           ,ISNULL(Max_UserProfiles.UpdateDate,getdate())--<UpdateDate, datetime,>
           ,0--<SpaceViews, int,>
           ,0--<LoginCount, int,>
           ,IsActive--<IsActive, bit,>
           ,ISNULL(Points,0)--<Points, int,>
           ,ISNULL([ExtendedPoints_1],0)--<Point_1, int,>
           ,ISNULL([ExtendedPoints_2],0)--<Point_2, int,>
           ,ISNULL([ExtendedPoints_3],0)--<Point_3, int,>
           ,ISNULL([ExtendedPoints_4],0)--<Point_4, int,>
           ,ISNULL([ExtendedPoints_5],0)--<Point_5, int,>
           ,ISNULL([ExtendedPoints_6],0)--<Point_6, int,>
           ,ISNULL([ExtendedPoints_7],0)--<Point_7, int,>
           ,ISNULL([ExtendedPoints_8],0)--<Point_8, int,>
           ,0--<TotalInvite, int,>
           ,ISNULL(TotalThreads,0)--<TotalTopics, int,>
           ,ISNULL(TotalPosts,0)--<TotalPosts, int,>
           ,0--<TotalComments, int,>
           ,0--<TotalShares, int,>
           ,0--<TotalCollections, int,>
           ,ISNULL([ValuedThreads],0)--<ValuedTopics, int,>
           ,ISNULL([DeletedThreads],0)--<DeletedTopics, int,>
           ,(ISNULL([DeletedPosts],0)-ISNULL([DeletedThreads],0)) --<DeletedReplies, int,>
           ,0--<TotalBlogArticles, int,>
           ,0--<TotalAlbums, int,>
           ,0--<TotalPhotos, int,>
           ,0--<TotalDoings, int,>
           ,ISNULL([TotalOnlineTime],0)--<TotalOnlineTime, int,>
           ,ISNULL([MonthOnlineTime],0)--<MonthOnlineTime, int,>
           ,''--<ExtendedData, ntext,>
           ,''--<ExtendedFieldVersion, nchar(36),>
           ,''--<UserInfo, varchar(800),>
           ,0--<UsedDiskSpaceSize, bigint,>
           ,0--<TotalDiskFiles, int,>
           ,'' FROM bbsMax_UserProfiles RIGHT OUTER JOIN
					  Max_UserProfiles ON bbsMax_UserProfiles.UserID = Max_UserProfiles.UserID RIGHT JOIN
					  Max_Users u ON Max_UserProfiles.UserID = u.UserID LEFT JOIN
					  Max_UserEmails ON u.UserID = Max_UserEmails.UserID
				WHERE u.UserID > 0
	SET IDENTITY_INSERT [bx_Users] OFF;

	DELETE bx_UserInfos WHERE UserID > 0;
	INSERT INTO [bx_UserInfos]
           ([UserID]
           ,[InviterID]
           ,[TotalFriends]
           ,[UnreadMessages]
           ,[UnreadBoardNotifies]
           ,[UnreadPostNotifies]
           ,[UnreadGroupInviteNotifies]
           ,[UnreadFriendNotifies]
           ,[UnreadHailNotifies]
           ,[UnreadAppInviteNotifies]
           ,[UnreadAppActionNotifies]
           ,[UnreadBidUpNotifies]
           ,[UnreadBirthdayNotifies]
           ,[LastSystemMessageID]
           ,[UsedAlbumSize]
           ,[AddedAlbumSize]
           ,[TimeZone]
           ,[Birthday]
           ,[BirthYear]
           ,[BlogPrivacy]
           ,[FeedPrivacy]
           ,[BoardPrivacy]
           ,[DoingPrivacy]
           ,[AlbumPrivacy]
           ,[SpacePrivacy]
           ,[SharePrivacy]
           ,[FriendListPrivacy]
           ,[InformationPrivacy]
           ,[EverNameChecked]
           ,[EverAvatarChecked]
           ,[EnableDisplaySidebar]
           ,[OnlineStatus])
		SELECT 
           m.UserID--<UserID, int,>
           ,0--<InviterID, int,>
           ,0--<TotalFriends, int,>
           ,0--<UnreadMessages, int,>
           ,0--<UnreadBoardNotifies, int,>
           ,0--<UnreadPostNotifies, int,>
           ,0--<UnreadGroupInviteNotifies, int,>
           ,0--<UnreadFriendNotifies, int,>
           ,0--<UnreadHailNotifies, int,>
           ,0--<UnreadAppInviteNotifies, int,>
           ,0--<UnreadAppActionNotifies, int,>
           ,0--<UnreadBidUpNotifies, int,>
           ,0--<UnreadBirthdayNotifies, int,>
           ,0--<LastSystemMessageID, int,>
           ,(ISNULL((SELECT SUM(TotalSize) FROM Max_DiskDirectories WHERE UserID = m.UserID), 0))--<UsedAlbumSize, bigint,>
           ,0--<AddedAlbumSize, bigint,>
           ,NULL--<TimeZone, real(24,0),>
           ,ISNULL(m.BirthdayDate,0) --<Birthday, smallint,>
           ,ISNULL(m.BirthdayYear,0)--<BirthYear, smallint,>
           ,0--<BlogPrivacy, tinyint,>
           ,0--<FeedPrivacy, tinyint,>
           ,0--<BoardPrivacy, tinyint,>
           ,0--<DoingPrivacy, tinyint,>
           ,0--<AlbumPrivacy, tinyint,>
           ,0--<SpacePrivacy, tinyint,>
           ,0--<SharePrivacy, tinyint,>
           ,0--<FriendListPrivacy, tinyint,>
           ,0--<InformationPrivacy, tinyint,>
           ,0--<EverNameChecked, bit,>
           ,0--<EverAvatarChecked, bit,>
           ,1--<EnableDisplaySidebar, tinyint,>
           ,1 FROM Max_UserProfiles m WITH (NOLOCK) RIGHT JOIN Max_Users u WITH (NOLOCK) ON m.UserID = u.UserID WHERE m.UserID > 0;--<OnlineStatus, tinyint,>)

END

GO

----------------------------------------------------------------------------------------------------------------------------------------------------

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_InviteSerials') BEGIN

	truncate table [bx_InviteSerials];
	SET IDENTITY_INSERT bx_InviteSerials ON;
	INSERT INTO [bx_InviteSerials]
           ([ID]
		   ,[Serial]
           ,[ToEmail]
           ,[CreateDate]
           ,[ExpiresDate]
           ,[UserID]
           ,[ToUserID]
           ,[Status])
		 SELECT
		   ID,
           Serial,--<Serial, uniqueidentifier,>
           SUBSTRING(ToEmail,1,200),--,<ToEmail, nvarchar(200),>
           CreateDate,--,<CreateDate, datetime,>
           ExpiresDate,--,<ExpiresDate, datetime,>
           UserID,--,<UserID, int,>
           ToUserID,--,<ToUserID, int,>
           [Status]--,<Status, tinyint,>)
		FROM Max_InviteSerials WITH (NOLOCK)
	SET IDENTITY_INSERT bx_InviteSerials OFF;

END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_DiskDirectories') BEGIN

	truncate table [bx_DiskDirectories];
	SET IDENTITY_INSERT [bx_DiskDirectories] ON;
	INSERT INTO [bx_DiskDirectories]
           ([DirectoryID]
		   ,[Name]
           ,[Password]
           ,[PrivacyType]
           ,[UserID]
           ,[ParentID]
           ,[TotalFiles]
           ,[TotalSize]
           ,[CreateDate]
           ,[UpdateDate])
     SELECT
           DirectoryID
		   ,DirectoryName
           ,N''
           ,0
           ,UserID
           ,ParentID
           ,(SELECT COUNT(*) FROM Max_DiskFiles WITH (NOLOCK) WHERE DirectoryID = T.DirectoryID)--<TotalFiles, int,>
           ,TotalSize--<TotalSize, bigint,>
           ,CreateDate--<CreateDate, datetime,>
           ,UpdateDate--<UpdateDate, datetime,>)
			FROM Max_DiskDirectories T WITH (NOLOCK);
	SET IDENTITY_INSERT [bx_DiskDirectories] OFF;

	--DROP TABLE Max_DiskDirectories;
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_DiskFiles') BEGIN

	truncate table [bx_DiskFiles];
	SET IDENTITY_INSERT [bx_DiskFiles] ON;
	INSERT INTO [bx_DiskFiles]
           ([DiskFileID]
		   ,[FileID]
           ,[FileName]
           ,[Extension]
           ,[FileSize]
           ,[UserID]
           ,[DirectoryID]
           ,[TotalDownloads]
           ,[CreateDate]
           ,[UpdateDate])
     SELECT
			df.DiskFileID
           ,f.[MD5Code] + CAST(f.[ContentLength] as varchar(30))
           ,df.FileName--<FileName, nvarchar(256),>
           ,LEFT(RIGHT(df.[FileName], LEN(df.[FileName]) - CHARINDEX(N'.', df.[FileName])), 5)--<Extension, nvarchar(10),>
           ,ISNULL(f.[ContentLength], 0)
           ,d.UserID--<UserID, int,>
           ,df.DirectoryID--<DirectoryID, int,>
           ,0--<TotalDownloads, int,>
           ,df.CreateDate--<CreateDate, datetime,>
           ,df.UpdateDate--<UpdateDate, datetime,>)
			FROM Max_DiskFiles df WITH (NOLOCK)
			LEFT JOIN Max_Files f WITH (NOLOCK) ON df.FileID = f.FileID
			LEFT JOIN Max_DiskDirectories d WITH (NOLOCK) ON df.DirectoryID = d.DirectoryID WHERE f.[MD5Code] IS NOT NULL;

	SET IDENTITY_INSERT [bx_DiskFiles] OFF;

	--DROP TABLE Max_DiskFiles;
	--IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_DiskDirectories')
		--DROP TABLE Max_DiskDirectories;
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_AttachmentExchanges') BEGIN
	DROP TABLE bx_AttachmentExchanges;
	EXEC sp_rename 'bbsMax_AttachmentExchanges','bx_AttachmentExchanges';
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_Attachments') BEGIN

	truncate table bx_Attachments;
	SET IDENTITY_INSERT bx_Attachments ON;
	INSERT INTO [bx_Attachments]
           ([AttachmentID]
		   ,[PostID]
           ,[FileID]
           ,[FileName]
           ,[FileType]
           ,[FileSize]
           ,[TotalDownloads]
           ,[TotalDownloadUsers]
           ,[Price]
           ,[FileExtendedInfo]
           ,[UserID]
           ,[CreateDate])
		SELECT
		   AttachmentID,
           PostID,--(<PostID, int,>
           f.[MD5Code] + CAST(f.[ContentLength] as varchar(30)),
           [FileName],--<FileName, nvarchar(256),>
           LEFT(RIGHT([FileName], LEN([FileName]) - CHARINDEX(N'.', [FileName])), 5),--<FileType, varchar(10),>
           ISNULL(f.[ContentLength], 0),--<FileSize, bigint,>
           TotalDownloads,--<TotalDownloads, int,>
           TotalDownloadUsers,--<TotalDownloadUsers, int,>
           Price,--<Price, int,>
           FileExtendedInfo,--<FileExtendedInfo, nvarchar(1000),>
           UserID,--<UserID, int,>
           DATEADD(day, -2, getdate()) FROM bbsMax_Attachments a WITH (NOLOCK)--,<CreateDate, datetime,>)
				LEFT JOIN Max_Files f WITH (NOLOCK) ON a.FileID = f.FileID WHERE f.FileID IS NOT NULL;
	SET IDENTITY_INSERT bx_Attachments OFF;

	--DROP TABLE bbsMax_Attachments

END

GO


IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_Files') BEGIN

	truncate table bx_Files;
	INSERT INTO [bx_Files]
           ([FileID]
		   ,[ServerFilePath]
           ,[MD5]
           ,[FileSize])
     SELECT
		   [MD5Code] + CAST([ContentLength] as varchar(30)),
           ServerFileName,--<ServerFilePath, nvarchar(256),>
           MD5Code,--,<MD5, char(32),>
           ContentLength--,<FileSize, bigint,>)
		   FROM Max_Files WITH (NOLOCK)

	--DROP TABLE Max_Files;

END

GO

--转换回复数据
--------------------------------------------
IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_PostMarks') BEGIN
	DROP TABLE bx_PostMarks;
	EXEC sp_rename 'bbsMax_PostMarks','bx_PostMarks';
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_Posts') BEGIN
	DROP TABLE bx_Posts;
	EXEC sp_rename 'bbsMax_Posts','bx_Posts';
	EXEC sp_rename N'bx_Posts.[PK_bbsMax_Posts]', N'PK_bx_Posts', N'INDEX';
	EXEC sp_rename N'bx_Posts.[IX_bbsMax_Posts_Forum]', N'IX_bx_Posts_Forum', N'INDEX';
	EXEC sp_rename N'bx_Posts.[IX_bbsMax_Posts_SortOrder]', N'IX_bx_Posts_SortOrder', N'INDEX';
	EXEC sp_rename N'bx_Posts.[IX_bbsMax_Posts_Thread]', N'IX_bx_Posts_Thread', N'INDEX';
	EXEC sp_rename N'bx_Posts.[IX_bbsMax_Posts_User]', N'IX_bx_Posts_User', N'INDEX';
	ALTER TABLE bx_Posts add [HistoryAttachmentIDs] varchar(500)       COLLATE Chinese_PRC_CI_AS_WS    NULL;
	ALTER TABLE bx_Posts add [KeywordVersion] varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NULL;
END

GO


--转换特殊主题数据
--------------------------------------------
--投票
IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_PollItemDetails') BEGIN
	DROP TABLE bx_PollItemDetails;
	EXEC sp_rename 'bbsMax_PollItemDetails','bx_PollItemDetails';
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_PollItems') BEGIN
	DROP TABLE bx_PollItems;
	EXEC sp_rename 'bbsMax_PollItems','bx_PollItems';
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_Polls') BEGIN
	DROP TABLE bx_Polls;
	EXEC sp_rename 'bbsMax_Polls','bx_Polls';
END

GO

--提问
IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_QuestionUsers') BEGIN
	DROP TABLE bx_QuestionUsers;
	EXEC sp_rename 'bbsMax_QuestionUsers','bx_QuestionUsers';
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_QuestionRewards') BEGIN
	DROP TABLE bx_QuestionRewards;
	EXEC sp_rename 'bbsMax_QuestionRewards','bx_QuestionRewards';
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_Questions') BEGIN
	DROP TABLE bx_Questions;
	EXEC sp_rename 'bbsMax_Questions','bx_Questions';
END

GO

--辩论
IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_Polemizes') BEGIN
	DROP TABLE bx_Polemizes;
	EXEC sp_rename 'bbsMax_Polemizes','bx_Polemizes';
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_PolemizeUsers') BEGIN
	DROP TABLE bx_PolemizeUsers;
	EXEC sp_rename 'bbsMax_PolemizeUsers','bx_PolemizeUsers';
END

GO




--转换主题配套数据
--------------------------------------------
IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_ThreadRanks') BEGIN
	DROP TABLE bx_ThreadRanks;
	EXEC sp_rename 'bbsMax_ThreadRanks','bx_ThreadRanks';
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_ThreadExchanges') BEGIN
	DROP TABLE bx_ThreadExchanges;
	EXEC sp_rename 'bbsMax_ThreadExchanges','bx_ThreadExchanges'
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_ThreadManageLogs') BEGIN
	DROP TABLE bx_ThreadManageLogs;
	EXEC sp_rename 'bbsMax_ThreadManageLogs','bx_ThreadManageLogs'
END

GO

--转换主题数据
--------------------------------------------

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_Threads') BEGIN
	DROP TABLE bx_Threads;
	EXEC sp_rename 'bbsMax_Threads','bx_Threads';
	EXEC sp_rename 'bx_Threads.[PK_bbsMax_Threads]', 'PK_bx_Threads', 'INDEX';
	EXEC sp_rename 'bx_Threads.[IX_bbsMax_Threads_Catalog]', 'IX_bx_Threads_Catalog', 'INDEX';
	EXEC sp_rename 'bx_Threads.[IX_bbsMax_Threads_Forum]', 'IX_bx_Threads_Forum', 'INDEX';
	EXEC sp_rename 'bx_Threads.[IX_bbsMax_Threads_SortOrder]', 'IX_bx_Threads_SortOrder', 'INDEX';
	EXEC sp_rename 'bx_Threads.[IX_bbsMax_Threads_Type]', 'IX_bx_Threads_Type', 'INDEX';
	EXEC sp_rename 'bx_Threads.[IX_bbsMax_Threads_User]', 'IX_bx_Threads_User', 'INDEX';
	EXEC sp_rename 'bx_Threads.[IX_bbsMax_Threads_Valued]', 'IX_bx_Threads_Valued', 'INDEX';
	ALTER TABLE bx_Threads add [KeywordVersion]   varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NULL;
END

GO

--转换版块\主题分类
--------------------------------------------
IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_ThreadCatalogsInForums') BEGIN
	DROP TABLE bx_ThreadCatalogsInForums;
	EXEC sp_rename 'bbsMax_ThreadCatalogsInForums','bx_ThreadCatalogsInForums'
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_Forums') BEGIN
	DROP TABLE bx_Forums;
	EXEC sp_rename 'bbsMax_Forums','bx_Forums';
	EXEC sp_rename 'bx_Forums.LogoUrl','LogoSrc';
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_ThreadCatalogs') BEGIN
	DROP TABLE bx_ThreadCatalogs;
	EXEC sp_rename 'bbsMax_ThreadCatalogs','bx_ThreadCatalogs';
END


GO


IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_Announcements') BEGIN
	DROP TABLE bx_Announcements;
	EXEC sp_rename 'bbsMax_Announcements','bx_Announcements';
	EXEC sp_rename 'bx_Announcements.StartDate','BeginDate';
	ALTER TABLE [bx_Announcements] DROP COLUMN [PostUserName]
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_EmoticonGroups') BEGIN

	truncate table bx_EmoticonGroups;
	SET IDENTITY_INSERT bx_EmoticonGroups ON;
	INSERT INTO [bx_EmoticonGroups]
           ([GroupID]
		   ,[GroupName]
           ,[UserID]
           ,[TotalEmoticons]
           ,[TotalSizes])
		SELECT
			GroupID
           ,GroupName--(<GroupName, nvarchar(50),>
           ,UserID--,<UserID, int,>
           ,TotalEmoticons--,<TotalEmoticons, int,>
           ,TotalSizes--,<TotalSizes, int,>)
		FROM Max_EmoticonGroups;
	SET IDENTITY_INSERT bx_EmoticonGroups OFF;

	--DROP TABLE Max_EmoticonGroups;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_Emoticons') BEGIN

	truncate table bx_Emoticons;
	SET IDENTITY_INSERT bx_Emoticons ON;
	INSERT INTO [bx_Emoticons]
			(EmoticonID
           ,[GroupID]
           ,[UserID]
           ,[Shortcut]
           ,[ImageSrc]
           ,[FileSize]
           ,[MD5]
           ,[SortOrder])
		 SELECT
		   e.EmoticonID,
           e.GroupID,--(<GroupID, int,>
           g.UserID,
           e.Shortcut,--,<Shortcut, nvarchar(100),>
           e.[FileName],--,<ImageUrlSrc, varchar(255),>
           e.FileSize,--,<FileSize, int,>
           '',--,<MD5, varchar(50),>
           e.SortOrder--,<SortOrder, int,>)
		FROM Max_Emoticons e LEFT JOIN Max_EmoticonGroups g ON e.GroupID = g.GroupID

	SET IDENTITY_INSERT bx_Emoticons OFF;

--	DROP TABLE Max_Emoticons;
END

GO


IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_Moderators') BEGIN

	truncate table bx_Moderators;
	INSERT INTO [bx_Moderators]
           ([ForumID]
           ,[UserID]
           ,[BeginDate]
           ,[EndDate]
           ,[ModeratorType]
           ,[SortOrder]
           ,[AppointorID])
     SELECT
           ForumID,--(<ForumID, int,>
           UserID,--,<UserID, int,>
           '1753-1-1',--,<BeginDate, datetime,>
           '9999-12-31',--<EndDate, datetime,>
           2,--,<ModeratorType, tinyint,>
           SortOrder,--,<SortOrder, int,>
           1--,<AppointorID, int,>)
		FROM bbsMax_Moderators;

--	DROP TABLE bbsMax_Moderators;
END

GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='System_Max_Settings') AND EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='System_bbsMax_Settings') BEGIN

	DELETE bx_Settings WHERE TypeName = 'MaxLabs.bbsMax.Settings.SiteSettings' AND [Key] IN ('SiteName', 'SiteUrl', 'BbsName');

	INSERT INTO bx_Settings ([Key], [Value], [TypeName])
		SELECT [SettingKey], [SettingValue], 'MaxLabs.bbsMax.Settings.SiteSettings' FROM [System_Max_Settings] WHERE [SettingKey] IN ('SiteName','Siteurl')
		Union All SELECT [SettingKey],[SettingValue],'MaxLabs.bbsMax.Settings.SiteSettings' FROM [System_bbsMax_Settings] WHERE [SettingKey] = 'bbsName';

END

GO

------开始删除3.0表(已经不需要，因为差异部署会自动删除)
--IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_Users')
	--DROP TABLE Max_Users;

--GO

--IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_UserProfiles')
	--DROP TABLE Max_UserProfiles;

--GO

--IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_UserOptions')
	--DROP TABLE Max_UserOptions;

--GO

--IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='bbsMax_UserProfiles')
	--DROP TABLE bbsMax_UserProfiles;

--GO

--IF EXISTS (SELECT * FROM [sysobjects] WHERE [type]='U' AND [name]='Max_InviteSerials')	
	--DROP TABLE Max_InviteSerials;