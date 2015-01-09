IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_PointShows') AND [name] ='Price') BEGIN
	EXEC (' ALTER TABLE [bx_PointShows] ADD [Price] [int] NOT NULL DEFAULT (0)');
	EXEC (' UPDATE [bx_PointShows] SET [Price] = 1; ');
END

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_PostMarks') AND [name] ='Username') BEGIN
	EXEC (' ALTER TABLE [bx_PostMarks] ADD [Username] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS_WS NOT NULL  DEFAULT ('''') ');
	EXEC (' UPDATE [bx_PostMarks] SET [Username] = [bx_Users].Username FROM [bx_Users] WHERE [bx_PostMarks].UserID = [bx_Users].UserID; ');
END

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_Posts') AND [name] ='MarkCount') BEGIN
	EXEC (' ALTER TABLE [bx_Posts] ADD [MarkCount] [int] NULL ');
	EXEC ('
 UPDATE [bx_Posts] SET [MarkCount] = D.Count FROM
  (SELECT Count(*) AS Count,P.PostID as PID FROM bx_PostMarks P GROUP BY P.PostID) AS D 
  WHERE PostID = D.PID;
  ');
END


IF EXISTS(SELECT * FROM sysobjects WHERE [name]='bx_ShareReverters' AND type='U') BEGIN
	DROP TABLE [bx_ShareReverters]
END



IF EXISTS(SELECT * FROM sysobjects WHERE [name]='bx_Shares' AND type='U') BEGIN

IF NOT EXISTS(SELECT * FROM sysobjects WHERE [name]='bx_UserShares' AND type='U') BEGIN
			EXEC ('
CREATE TABLE [bx_UserShares] (
	 [UserShareID]		int				IDENTITY (1,1)					NOT NULL
	,[UserID]			int												NOT NULL    CONSTRAINT [DF_bx_UserShares_UserID]		DEFAULT (0)
	,[ShareID]			int												NOT NULL    CONSTRAINT [DF_bx_UserShares_ShareID]		DEFAULT (0)
    ,[PrivacyType]      tinyint                                         NOT NULL    CONSTRAINT [DF_bx_UserShares_PrivacyType]   DEFAULT (0)
    ,[Subject]			nvarchar(100)	collate Chinese_PRC_CI_AS_WS	not null	constraint [DF_bx_UserShares_Subject]		default ('''')
	,[Description]		nvarchar(1000)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_UserShares_Decription]	DEFAULT ('''')
	,[CreateDate]		datetime										NOT NULL	CONSTRAINT [DF_bx_UserShares_CreateDate]	DEFAULT (GETDATE())
	,[CommentCount]		int												NOT NULL	CONSTRAINT [DF_bx_UserShares_CommentCount]	DEFAULT (0)
	
    ,[KeywordVersion]           varchar(32)       COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_UserShares_KeywordVersion]       DEFAULT ('''')
    
    ,CONSTRAINT [PK_bx_UserShares] PRIMARY KEY ([UserShareID])
);

INSERT INTO [bx_UserShares]([UserID],[ShareID],[PrivacyType],[Description],[CreateDate],[CommentCount]) 
	SELECT [UserID],[ShareID],[PrivacyType],[Description],[CreateDate],[TotalComments] FROM bx_Shares;
');
END


END



IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_Threads') AND [name] ='LastPostID') BEGIN
	EXEC (' ALTER TABLE [bx_Threads] ADD [LastPostID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_LastPostID] DEFAULT (0) ');
	EXEC ('
 UPDATE [bx_Threads] SET [LastPostID] = D.PID FROM
  (SELECT Max(PostID) AS PID,P.ThreadID as TID FROM bx_Posts P WHERE P.SortOrder<4000000000000000 GROUP BY P.ThreadID) AS D 
  WHERE ThreadID = D.TID;
  ');
END

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_Threads') AND [name] ='ContentID') BEGIN
	EXEC (' ALTER TABLE [bx_Threads] ADD [ContentID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_ContentID] DEFAULT (0) ');
	EXEC ('
UPDATE bx_Threads SET ContentID = T.PID FROM (
SELECT MIN(PostID) AS PID,ThreadID AS TID FROM bx_Posts GROUP by ThreadID
) AS T WHERE ThreadID = T.TID;
  ');
END


IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_Threads') AND [name] ='ThreadStatus') BEGIN
	EXEC (' ALTER TABLE [bx_Threads] ADD [ThreadStatus] [tinyint] NULL ');
	EXEC ('
drop index bx_Threads.IX_bx_Threads_Catalog;
drop index bx_Threads.IX_bx_Threads_Forum;
drop index bx_Threads.IX_bx_Threads_SortOrder;
drop index bx_Threads.IX_bx_Threads_Type;
drop index bx_Threads.IX_bx_Threads_User;
drop index bx_Threads.IX_bx_Threads_Valued;
UPDATE bx_Threads SET [ThreadStatus] = Floor(SortOrder / 1000000000000000),SortOrder = SortOrder - Floor(SortOrder / 1000000000000000) * 1000000000000000 where SortOrder > 1000000000000000 AND SortOrder<10000000000000000;

UPDATE bx_Threads SET [ThreadStatus] = 1 where SortOrder>=10000000000000000;

--处理重复的SortOrder
declare @thread table(id int IDENTITY(1,1),threadid int,sortorder bigint)
insert into @thread(threadid,sortorder) select ThreadID,SortOrder FROM bx_Threads
where SortOrder in(
select sortorder from bx_Threads group by sortorder having(count(sortorder) > 1)
)

declare @MaxSortOrder bigint,@count int,@i int;
select @MaxSortOrder = MAX(SortOrder) FROM bx_Threads;

select @count = count(*) from @thread
select @i = 0;

while(@i<@count) begin
    select @MaxSortOrder = @MaxSortOrder + 1,@i=@i+1;
    declare @threadID int;
    select @threadID = threadid from @thread where id = @i;
    update bx_Threads set SortOrder = @MaxSortOrder where threadID = @threadID;
end
  ');
END

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_UserInfos') AND [name] ='LastReadSystemNotifyID') BEGIN
	EXEC (' ALTER TABLE [bx_UserInfos] ADD [LastReadSystemNotifyID] [int] NULL ');
END

IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_Users') AND [name] ='LastImpressionDate') BEGIN
	EXEC (' ALTER TABLE [bx_Users] ADD [LastImpressionDate] [datetime] NULL ');
END

IF NOT EXISTS(SELECT * FROM sysobjects WHERE [name]='bx_UserVars' AND type='U') BEGIN
			EXEC ('
CREATE TABLE [bx_UserVars]
(
[UserID] [int] NOT NULL,
[Password] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[PasswordFormat] [tinyint] NOT NULL CONSTRAINT [DF_bx_UserVars_PasswordFormat] DEFAULT (3),
[UnreadMessages] [int] NOT NULL CONSTRAINT [DF_bx_UserVars_UnreadMessages] DEFAULT (0),
[LastReadSystemNotifyID] [int] NOT NULL CONSTRAINT [DF_bx_UserVars_LostReadSystemNotifyID] DEFAULT (0),
[UsedAlbumSize] [bigint] NOT NULL CONSTRAINT [DF_bx_UserVars_UsedAlbumSize] DEFAULT (0),
[AddedAlbumSize] [bigint] NOT NULL CONSTRAINT [DF_bx_UserVars_AddedAlbumSize] DEFAULT (0),
[TimeZone] [real] NOT NULL CONSTRAINT [DF_bx_UserVars_TimeZone] DEFAULT (9999),
[EverAvatarChecked] [bit] NOT NULL CONSTRAINT [DF_bx_UserVars_EverAvatarChecked] DEFAULT (0),
[EnableDisplaySidebar] [tinyint] NOT NULL CONSTRAINT [DF_bx_UserVars_EnableDisplaySidebar] DEFAULT (0),
[OnlineStatus] [tinyint] NOT NULL CONSTRAINT [DF_bx_UserVars_OnlineStatus] DEFAULT (0),
[SkinID] [nvarchar] (256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_UserVars_SkinID] DEFAULT (''''),
[TotalDiskFiles] [int] NOT NULL CONSTRAINT [DF_bx_UserVars_TotalDiskFiles] DEFAULT (0),
[UsedDiskSpaceSize] [bigint] NOT NULL CONSTRAINT [DF_bx_UserVars_UsedDiskSpaceSize] DEFAULT (0),
[LastAvatarUpdateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_UserVars_LastAvatarUpdateDate] DEFAULT (getdate()),
[LastImpressionDate] [datetime] NOT NULL CONSTRAINT [DF__bx_UserVars_LastImpressionDate] DEFAULT (''1980-1-1''),
[SelectFriendGroupID] [int] NOT NULL CONSTRAINT [DF_bx_UserVars_SelectFriendGroupID] DEFAULT ((-1)),
[ReplyReturnThreadLastPage] bit NULL,
CONSTRAINT [PK_bx_UserVars] PRIMARY KEY ([UserID])
);

INSERT INTO [bx_UserVars]([UserID],[Password],[PasswordFormat],[UnreadMessages],[LastReadSystemNotifyID],[UsedAlbumSize],[AddedAlbumSize],[TimeZone]
,[EverAvatarChecked],[EnableDisplaySidebar],[OnlineStatus],[SkinID],[TotalDiskFiles],[UsedDiskSpaceSize],[LastAvatarUpdateDate],[LastImpressionDate]) 
	SELECT U.[UserID],[Password],[PasswordFormat],[UnreadMessages],ISNULL([LastReadSystemNotifyID],0),[UsedAlbumSize],[AddedAlbumSize],ISNULL([TimeZone],9999)
,[EverAvatarChecked],[EnableDisplaySidebar],[OnlineStatus],[SkinID],[TotalDiskFiles],
[UsedDiskSpaceSize]
,[LastAvatarUpdateDate],ISNULL([LastImpressionDate],''1753-1-1'')
	 FROM bx_Users U INNER JOIN bx_UserInfos UI ON U.UserID = UI.UserID;

IF EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id(''bx_Users'') AND [name] =''TotalDiskFiles'') BEGIN

	INSERT INTO bx_UserVars([UserID],[Password],[PasswordFormat],[TotalDiskFiles],[LastAvatarUpdateDate],[LastImpressionDate]) 
		SELECT U.[UserID],[Password],[PasswordFormat],[TotalDiskFiles],[LastAvatarUpdateDate],[LastImpressionDate]
		 FROM bx_Users U WHERE U.UserID NOT IN (SELECT UserID FROM bx_Users);
	 
END
	
');
END

UPDATE bx_Users
   SET UserInfo = (
					CAST(T.[InviterID] AS varchar(10)) + '|'			--0
					
					+ CAST(T.[TotalFriends] AS varchar(10)) + '|'		--1
					
					+ CAST(T.[Birthday] AS varchar(4)) + '|'			--2
					+ CAST(T.[BirthYear] AS varchar(4)) + '|'			--3
					
					+ CAST(T.[BlogPrivacy] AS varchar(10)) + '|'		--4
					+ CAST(T.[FeedPrivacy] AS varchar(10)) + '|'		--5
					+ CAST(T.[BoardPrivacy] AS varchar(10)) + '|'		--6
					+ CAST(T.[DoingPrivacy] AS varchar(10)) + '|'		--7
					+ CAST(T.[AlbumPrivacy] AS varchar(10)) + '|'		--8
					+ CAST(T.[SpacePrivacy] AS varchar(10)) + '|'		--9
					+ CAST(T.[SharePrivacy] AS varchar(10)) + '|'		--10
					
					+ CAST(T.[FriendListPrivacy] AS varchar(10)) + '|'	--11
					+ CAST(T.[InformationPrivacy] AS varchar(10)) + '|'	--12
					
					+ CAST(T.[NotifySetting] AS varchar(4000))			--13
					)
  FROM [bx_UserInfos] T WHERE T.UserID = bx_Users.UserID;
  




    
IF NOT EXISTS(SELECT * FROM sysobjects WHERE [name]='bx_NotifyTypes' AND type='U') BEGIN
	EXEC ('
	CREATE TABLE bx_NotifyTypes(
	 [TypeID]       int IDENTITY(1,1)    not null    
	,[TypeName]     nvarchar(50)		 not null
	,[Keep]         bit                  not null    CONSTRAINT [DF_bx_NotifyTypes_Keep] DEFAULT (1)
	,[Description]  nvarchar(200)        null  
	,CONSTRAINT [PK_bx_NotifyTypes] PRIMARY KEY ([TypeID])       
	);
	
----------------内置通知类型
SET IDENTITY_INSERT [bx_NotifyTypes] ON
INSERT INTO bx_NotifyTypes(TypeID , TypeName, [Keep]) VALUES( 1 , N''管理通知'', 1 );
INSERT INTO bx_NotifyTypes(TypeID , TypeName, [Keep]) VALUES( 2 , N''好友验证'', 1 );
INSERT INTO bx_NotifyTypes(TypeID , TypeName, [Keep]) VALUES( 3 , N''打招呼'', 1 );
INSERT INTO bx_NotifyTypes(TypeID , TypeName, [Keep]) VALUES( 101 , N''评论类的通知'', 0 );
INSERT INTO bx_NotifyTypes(TypeID , TypeName, [Keep]) VALUES( 102 , N''道具通知'', 0 );
SET IDENTITY_INSERT [bx_NotifyTypes] OFF

	')
END


IF NOT EXISTS(SELECT * FROM sysobjects WHERE [name]='bx_PassportClients' AND type='U') BEGIN
	EXEC ('
CREATE TABLE bx_PassportClients (
 
	 [ClientID]          int		    IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_PassportClients]                 PRIMARY KEY ([ClientID])
	,[ClientName]        nvarchar(50)                                   NOT NULL
	,[AccessKey]         nvarchar(50)									NULL
	,[Url]               nvarchar(1000)                                 NOT NULL
	,[APIFilePath]       nvarchar(200)                                  NOT NULL
	,[CreateDate]        datetime                                       NOT NULL    CONSTRAINT  [DF_bx_PassportClients_CreateDate]		DEFAULT(GETDATE())
	,[Deleted]           bit											NOT NULL	CONSTRAINT  [DF_bx_PassportClients_Deleted]			DEFAULT(0)
	,[InstructTypes]     text			COLLATE Chinese_PRC_CI_AS_WS	NULL
);
	
SET IDENTITY_INSERT [bx_PassportClients] ON
INSERT INTO bx_PassportClients(ClientID  ,ClientName, Deleted,Url,APIFilePath) VALUES( 0 , N''Passport Server'', 1,'''','''');
SET IDENTITY_INSERT [bx_PassportClients] OFF

	')
END







  DELETE bx_Visitors FROM 
(
SELECT UserID ,VisitorUserID FROM bx_Visitors GROUP BY UserID,VisitorUserID 
HAVING COUNT(*)>1
) AS t
WHERE t.UserID = bx_Visitors.UserID and t.VisitorUserID = bx_Visitors.VisitorUserID;