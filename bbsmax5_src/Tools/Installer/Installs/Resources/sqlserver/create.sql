IF EXISTS (SELECT [name] FROM sysobjects WHERE [name]='bx_Drop' AND [type]='P')
   DROP PROCEDURE bx_Drop
GO

CREATE PROCEDURE bx_Drop @Name sysname
AS
BEGIN

	IF OBJECT_ID(@Name) IS NULL
		RETURN;


	/* 删除指定名称的表 */
	IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'U' AND [name] = @Name) BEGIN
		
		DECLARE cs_trgs CURSOR FOR SELECT [name] FROM [sysobjects] WHERE [type] = 'F' and parent_obj = OBJECT_ID(@Name) FOR READ ONLY
		DECLARE @t_name NVARCHAR(100)

		OPEN cs_trgs

		FETCH NEXT FROM cs_trgs INTO @t_name
		WHILE @@FETCH_STATUS = 0 BEGIN
			EXEC('ALTER TABLE [' + @Name + '] DROP CONSTRAINT [' + @t_name + '];');
			FETCH NEXT FROM cs_trgs INTO @t_name;
		END

		CLOSE cs_trgs;
		DEALLOCATE cs_trgs;
		
		EXEC('DROP TABLE ' + @Name);
    END

	/* 删除指定名称的外键 */
	ELSE IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'F' AND [name] = @Name) BEGIN

		DECLARE @TableName nvarchar(200)
		SELECT @TableName = OBJECT_NAME(fkeyid) FROM [sysforeignkeys] WHERE constid = OBJECT_ID(@Name)
		EXEC('ALTER TABLE [' + @TableName + '] DROP CONSTRAINT [' + @Name + '];');

	END

	/* 删除指定名称的存储过程 */
	ELSE IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = @Name)
		EXEC('DROP PROCEDURE [' + @Name + '];');

	/* 删除指定名称的自定义函数 */
	ELSE IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'TF' AND [name] = @Name)
		EXEC('DROP FUNCTION [' + @Name + '];');


END

GO
GO
-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/14>
-- Description:	<获取SortOrder,（当添加数据时，表中有SortOrder字段时使用）>
-- =============================================
CREATE PROCEDURE [bx_Common_GetSortOrder]
	@TableName nvarchar(256),
	@Condition nvarchar(400),--查询条件
	@SortOrder int output
AS
	SET NOCOUNT ON
	DECLARE @SQLString nvarchar(4000),@Count int
	if(@Condition<>'')
		SET @Condition=' WHERE '+@Condition
		

	SET @SQLString=N'SELECT @SortOrder=ISNULL(Max(SortOrder),0) FROM '+@TableName+@Condition
	EXECUTE sp_executesql @SQLString,N'@SortOrder int output',@SortOrder output
	SET @SortOrder=@SortOrder+1



GO
--根据一个字符串得到BigInt内存表
EXEC bx_Drop 'bx_GetBigIntTable';

GO

CREATE FUNCTION [bx_GetBigIntTable]
(
	@text varchar(8000),
	@separator varchar(2) = ','
)
RETURNS @ItemTable TABLE 
(
	 id   bigint IDENTITY(1, 1) 
	,item int
)
AS
BEGIN

	IF (@text = '')
		RETURN;

	INSERT @ItemTable
	SELECT SUBSTRING(@text, I ,CHARINDEX(@separator, @text + @separator, I) - I)   
	FROM [bx_Identities_8000] WITH (NOLOCK)
	WHERE I <= LEN(@text) + 1 AND CHARINDEX(@separator, @separator + @text, I) - I = 0   

	RETURN;

END

GO
GO
--根据一个字符串得到Int内存表
EXEC bx_Drop 'bx_GetIntTable';

GO

CREATE FUNCTION [bx_GetIntTable]
(
	@text varchar(8000),
	@separator varchar(2) = ','
)
RETURNS @ItemTable TABLE 
(
	 id   int IDENTITY(1, 1) 
	,item int
)
AS
BEGIN

	IF (@text = '')
		RETURN;

	INSERT @ItemTable
	SELECT SUBSTRING(@text, I ,CHARINDEX(@separator, @text + @separator, I) - I)   
	FROM [bx_Identities_8000] WITH (NOLOCK)
	WHERE I <= LEN(@text) + 1 AND CHARINDEX(@separator, @separator + @text, I) - I = 0   

	RETURN;

END

GO
GO
EXEC bx_Drop 'bx_GetStringTable_ntext';

GO

CREATE FUNCTION [bx_GetStringTable_ntext]
(
	@text ntext,
	@separator nvarchar(2) = N','
)
RETURNS @ItemTable TABLE 
(
	id int identity(1,1),
	item nvarchar(4000) 
)
AS
BEGIN

	DECLARE @s nvarchar(4000), @i int, @j int   
	SELECT @s = SUBSTRING(@text, 1, 4000), @i=1;  
	--IF (@s = '')
		--INSERT @ItemTable VALUES ('')
	--ELSE BEGIN
	IF (@s <> N'') BEGIN
		WHILE @s <> N'' BEGIN

			IF LEN(@s) = 4000   
				SELECT @j = 4000 - CHARINDEX(@separator, REVERSE(@s)), @i = @i + @j + 1, @s = LEFT(@s, @j)
			ELSE     
				SELECT @i = @i + 4000, @j = LEN(@s)

			INSERT @ItemTable
			SELECT SUBSTRING(@s, I ,CHARINDEX(@separator, @s + @separator, I) - I)
			FROM bx_Identities_4000 WITH (NOLOCK)
			WHERE I <= @j + 1 AND CHARINDEX(@separator, @separator + @s, I) - I = 0

			SELECT @s = SUBSTRING(@text, @i, 4000)

		END  
	END 
	RETURN;
END

GO



GO
EXEC bx_Drop 'bx_GetStringTable_nvarchar';

GO

CREATE FUNCTION [bx_GetStringTable_nvarchar]
(
	@text nvarchar(4000),
	@separator nvarchar(2) = N','
)
RETURNS @ItemTable TABLE 
(
	 id   int IDENTITY(1, 1) 
	,item nvarchar(4000)
)
AS
BEGIN

	IF (@text = N'')
		RETURN;

	INSERT @ItemTable
	SELECT SUBSTRING(@text, I ,CHARINDEX(@separator, @text + @separator, I) - I)   
	FROM [bx_Identities_8000] WITH (NOLOCK)
	WHERE I <= LEN(@text) + 1 AND CHARINDEX(@separator, @separator + @text, I) - I = 0   

	RETURN;

END

GO
GO
EXEC bx_Drop 'bx_GetStringTable_text';

GO

CREATE FUNCTION [bx_GetStringTable_text]
(
	@text text,
	@separator varchar(2) = ','
)
RETURNS @ItemTable TABLE 
(
	id int identity(1,1),
	item varchar(8000) 
)
AS
BEGIN

	DECLARE @s varchar(8000), @i int, @j int
	SELECT @s = SUBSTRING(@text, 1, 8000), @i=1;

	--IF (@s = '')
		--INSERT @ItemTable VALUES ('');
	--ELSE BEGIN

	IF (@s <> '') BEGIN
		WHILE @s <> '' BEGIN

			IF LEN(@s) = 8000   
				SELECT @j = 8000 - CHARINDEX(@separator, REVERSE(@s)), @i = @i + @j + 1, @s = LEFT(@s, @j)
			ELSE     
				SELECT @i = @i + 8000, @j = LEN(@s)

			INSERT @ItemTable
			SELECT SUBSTRING(@s, I ,CHARINDEX(@separator, @s + @separator, I) - I)
				FROM bx_Identities_8000 WITH (NOLOCK)
				WHERE I <= @j + 1 AND CHARINDEX(@separator, @separator + @s, I) - I = 0;

			SELECT @s = SUBSTRING(@text, @i, 8000);

		END
	END
	RETURN;
END

GO

GO
EXEC bx_Drop 'bx_GetStringTable_varchar';

GO

CREATE FUNCTION [bx_GetStringTable_varchar]
(
	@text varchar,
	@separator varchar(2) = ','
)
RETURNS @ItemTable TABLE 
(
	 id   int IDENTITY(1, 1) 
	,item varchar(8000)
)
AS
BEGIN

	IF (@text = '')
		RETURN;

	INSERT @ItemTable
	SELECT SUBSTRING(@text, I ,CHARINDEX(@separator, @text + @separator, I) - I)   
	FROM [bx_Identities_8000] WITH (NOLOCK)
	WHERE I <= LEN(@text) + 1 AND CHARINDEX(@separator, @separator + @text, I) - I = 0   

	RETURN;

END

GO
GO
--CREATE FUNCTION [bx_UpdateSortOrder]
--(
	--@PostType tinyint,--1正常,2置顶,3总置顶,4待审核,5回收站 
	--@OldSortOrder bigint
--)
--RETURNS @ItemTable TABLE 
--(
	--SortOrder bigint
--)
--AS
--BEGIN
	--INSERT @ItemTable (SortOrder) values( CAST(SUBSTRING(CAST(@OldSortOrder AS varchar(16)),2,15) AS bigint)+@PostType*1000000000000000);
	--RETURN;
--END
GO
EXEC bx_Drop 'bx_ADCategory';

CREATE TABLE bx_ADCategory (
	 [CategoryID]         int IDENTITY (1, 1)                            NOT NULL 
	,[CategoryName]       nvarchar(50)        COLLATE Chinese_PRC_CI_AS_WS  NOT NULL 
	,[Description]        nvarchar(1000)      COLLATE Chinese_PRC_CI_AS_WS  NULL 
	,[ShowInForum]        bit NULL 
	,[CommomPages]        varchar(500)        COLLATE Chinese_PRC_CI_AS_WS  NULL 
	
	CONSTRAINT [PK_bx_ADCategory] PRIMARY KEY ([CategoryID])
);
GO
CREATE TABLE [bx_AdminSessions] (
	[SessionID]			uniqueidentifier			NOT NULL									CONSTRAINT [DF_bx_AdminSessions_SessionID] DEFAULT (newid())
	,[IpAddress]		varchar(100)		COLLATE Chinese_PRC_CI_AS_WS		NOT NULL			CONSTRAINT [DF_bx_AdminSessions_IpAddress] DEFAULT ('')
	,[UserID]			int							NOT NULL 
	,[CreateDate]		datetime					NOT NULL									CONSTRAINT [DF_bx_AdminSessions_CreateDate] DEFAULT (getdate())
	,[UpdateDate]		datetime					NOT NULL									CONSTRAINT [DF_bx_AdminSessions_UpdateDate] DEFAULT (getdate())
	,[Available]		bit							NOT NULL									CONSTRAINT [DF_bx_AdminSessions_Available] DEFAULT (1)
	
	CONSTRAINT [PK_bx_AdminSessions] PRIMARY KEY  CLUSTERED ([SessionID])
)
GO
--广告表
EXEC bx_Drop 'bx_Adverts';

CREATE TABLE bx_Adverts(
	 [ADID]				int													NOT NULL											IDENTITY (1, 1)												
	,[CategoryID]		int													NOT NULL								
	,[Index]			int													NULL		CONSTRAINT [DF_bx_Adverts_Index]		DEFAULT(0)
	,[Position]			tinyint												NULL		CONSTRAINT [DF_bx_Adverts_Position]		DEFAULT(0)
	,[Targets]			ntext				COLLATE Chinese_PRC_CI_AS_WS	NULL		CONSTRAINT [DF_bx_Adverts_Targets]		DEFAULT('')
	,[ADType]			tinyint												NOT NULL				
	,[Available]		bit													NOT NULL	CONSTRAINT [DF_bx_Adverts_Available]	DEFAULT(1)
	,[Color]			varchar(50)			COLLATE Chinese_PRC_CI_AS_WS	NULL		CONSTRAINT [DF_bx_Adverts_Color]		DEFAULT('0')
	,[Title]			nvarchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT [DF_bx_Adverts_Title]		DEFAULT('')
	,[Code]				ntext				COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_Adverts_CODE]			DEFAULT('')
	,[Text]				nvarchar(200)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT [DF_bx_Adverts_Text]			DEFAULT('')
	,[Href]				nvarchar(500)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT [DF_bx_Adverts_Href]		    DEFAULT('')
	,[FontSize]			int													NOT NULL	CONSTRAINT [DF_bx_Adverts_FontSize]		DEFAULT(14)
	,[ResourceHref]		nvarchar(500)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT [DF_bx_Adverts_ResourceHref]	DEFAULT('')
	,[Height]			int													NOT NULL	CONSTRAINT [DF_bx_Adverts_Width]		DEFAULT(0)
	,[Width]			int													NOT NULL	CONSTRAINT [DF_bx_Adverts_Height]		DEFAULT(0)
	,[BeginDate]		datetime											NULL		CONSTRAINT [DF_bx_Adverts_BeginDate]	DEFAULT(GETDATE())
	,[EndDate]			datetime											NULL
	,[Floor]			varchar(1000)		COLLATE Chinese_PRC_CI_AS_WS	NULL		CONSTRAINT [DF_bx_Adverts_Floor]		DEFAULT(',0,')
	CONSTRAINT [PK_bx_Adverts] PRIMARY KEY ([ADID])
); 





/*
Name:     广告表
Columns:
        [ADID]
        
        [CategoryID]            广告类别
        [Index]                 显示顺序
        [Position]              显示位置（贴内广告  上、下右）
        
        [Targets]               投放目标（用逗号(,)隔开的字符串）
        
        [ADType]				广告形式（文字链接、图片、flash、HTML代码）
        [Available]				是否启用 					
        [Color]					颜色（针对文字链接）
        [Title]					标题    
        [Code]					广告代码   
        [Text]					文本（针对文字链接和图片的alt属性）   
        [Href]					广告目标地址   
        [FontSize]				字体大小（针对文字链接） 
        [ResourceHref]			广告资源地址（指图片或者FLASH的src）
        [Height]				高度（图片或者flash）   
        [Width]					宽度（同上）   
        [BeginDate]				开始日期 
        [EndDate]				结束日期
*/

GO
GO
EXEC bx_Drop bx_AlbumReverters;

GO

CREATE TABLE [bx_AlbumReverters](
	[AlbumID]				int				NOT NULL,
	[NameReverter]			nvarchar(1500)	COLLATE Chinese_PRC_CI_AS_WS  NOT NULL,
	[DescriptionReverter]   nvarchar(2500)	COLLATE Chinese_PRC_CI_AS_WS  NULL,

	CONSTRAINT [PK_bx_AlbumReverters] PRIMARY KEY([AlbumID])
)

/*
Name:标签
Columns:
    [AlbumID]		       可恢复的相册ID
	[NameReverter]         相册名复原关键信息，可根据此信息恢复相册名的原始内容
*/

GO
GO
EXEC bx_Drop bx_Albums;

GO

CREATE TABLE [bx_Albums](
    [AlbumID]          int              IDENTITY(1, 1)                  NOT NULL    CONSTRAINT  [PK_bx_Albums]                PRIMARY KEY ([AlbumID])
   ,[UserID]           int                                              NOT NULL    CONSTRAINT  [DF_bx_Albums_UserID]         DEFAULT(0)
   ,[TotalPhotos]      int                                              NOT NULL    CONSTRAINT  [DF_bx_Albums_TotalPhotos]    DEFAULT(0)
   ,[LastEditUserID]   int                                              NOT NULL    CONSTRAINT  [DF_bx_Albums_LastEditUserID] DEFAULT(0)
  
   ,[PrivacyType]      tinyint                                          NOT NULL    CONSTRAINT  [DF_bx_Albums_PrivacyType]    DEFAULT(0)
  
   ,[Name]             nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Albums_Name]           DEFAULT('')
   ,[Description]       nvarchar(100)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Albums_Description]       DEFAULT('')

   ,[Cover]            nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Albums_Cover]          DEFAULT('')
   ,[CoverPhotoID]     int
   
   ,[Password]         nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Albums_Password]       DEFAULT('')

   ,[CreateDate]       datetime                                         NOT NULL    CONSTRAINT  [DF_bx_Albums_CreateDate]     DEFAULT (GETDATE())
   ,[UpdateDate]       datetime                                         NOT NULL    CONSTRAINT  [DF_bx_Albums_UpdateDate]     DEFAULT (GETDATE())
   ,[KeywordVersion]   varchar(32)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Albums_KeywordVersion] DEFAULT('')
)

/*
Name:相册
Columns:
    [AlbumID]          自动标识ID
    [UserID]           用户ID
    [TotalPhotos]      些相册照片数
  
    [PrivacyType]      权限类型 0.所有用户见 1.全好友可见 2.仅自己可见 3.凭密码查看
     
    [Name]             相册名称
    [Cover]            相册封面图片
    [Password]         凭密码查看时的密码 
    [NameReverter]     相册名称关键字还原信息
   
    [CreateDate]       创建时间
    [UpdateDate]       更新时间
*/

GO

--相册表的用户ID索引
CREATE INDEX [IX_bx_Albums_UserID] ON [bx_Albums]([UserID])

--相册隐私类型的索引
CREATE INDEX [IX_bx_Albums_PrivacyType] ON [bx_Albums]([PrivacyType])

--相册表的创建时间索引
--CREATE INDEX [IX_bx_Albums_CreateDate] ON [bx_Albums]([CreateDate])

GO
EXEC bx_Drop 'bx_Announcements';

CREATE TABLE bx_Announcements (
	 [AnnouncementID]		int								IDENTITY (1, 1)								CONSTRAINT [PK_bx_Announcement]			PRIMARY KEY (AnnouncementID)
	,[AnnouncementType]		tinyint			NOT NULL													CONSTRAINT [DF_bx_Announcement_Type]						DEFAULT (0)
	,[PostUserID]			int				NOT NULL													
	,[Subject]				nvarchar(200)					COLLATE Chinese_PRC_CI_AS_WS NOT NULL		CONSTRAINT [DF_bx_Announcement_Subject]                     DEFAULT ('')
	,[Content]				ntext							COLLATE Chinese_PRC_CI_AS_WS NOT NULL		CONSTRAINT [DF_bx_Announcement_Content]                     DEFAULT ('')
	,[BeginDate]				datetime		NOT NULL													CONSTRAINT [DF_bx_Announcement_BeginDate]                   DEFAULT (GETDATE())
	,[EndDate]				datetime		NOT NULL
	,[SortOrder]				int				NOT NULL													CONSTRAINT [DF_bx_Announcement_SortOrder]                   DEFAULT (0)
)
--TODO 与USER表的级联删除
GO
EXEC bx_Drop 'bx_AttachmentExchanges';

CREATE TABLE [bx_AttachmentExchanges]
(
[AttachmentID] [int] NOT NULL,
[UserID] [int] NOT NULL,
[Price] [int] NOT NULL,
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_AttachmentExchanges_CreateDate] DEFAULT (getdate()),
CONSTRAINT [PK_bx_AttachmentExchanges] PRIMARY KEY ([AttachmentID], [UserID])
)
GO


GO
EXEC bx_Drop 'bx_Attachments';

CREATE TABLE bx_Attachments
(
AttachmentID		int				NOT NULL IDENTITY(1, 1),
PostID				int				NOT NULL,
--DiskFileID int NOT NULL,
FileID				varchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT DF_bx_Attachments_FileID				DEFAULT (''),
FileName			nvarchar(256)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT DF_bx_Attachments_FileName			DEFAULT (N''),
FileType			nvarchar (10)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT DF_bx_Attachments_FileType			DEFAULT (''),
FileSize			bigint											NOT NULL	CONSTRAINT DF_bx_Attachments_FileSize			DEFAULT (0),
TotalDownloads		int												NOT NULL	CONSTRAINT DF_bx_Attachments_TotalDownloads		DEFAULT (0),
TotalDownloadUsers	int												NOT NULL	CONSTRAINT DF_bx_Attachments_TotalDownloadUsers	DEFAULT (0),
Price				int												NOT NULL	CONSTRAINT DF_bx_Attachments_Price				DEFAULT (0),
FileExtendedInfo	nvarchar(1000)	COLLATE Chinese_PRC_CI_AS_WS	NULL		CONSTRAINT DF_bx_Attachments_FileExtendedInfo	DEFAULT (N''),
UserID				int												NOT NULL	CONSTRAINT DF_bx_Attachments_UserID				DEFAULT (0),
CreateDate			datetime										NOT NULL	CONSTRAINT DF_bx_Attachments_CreateDate			DEFAULT (getdate()),

CONSTRAINT PK_bx_Attachments PRIMARY KEY NONCLUSTERED  (AttachmentID)
)
GO

CREATE NONCLUSTERED INDEX IX_bx_Attachments_List ON bx_Attachments (PostID);
CREATE NONCLUSTERED INDEX IX_bx_Attachments_User ON bx_Attachments (UserID);
CREATE NONCLUSTERED INDEX IX_bx_Attachments_FileID ON bx_Attachments (FileID);

CREATE INDEX IX_bx_Attachments_CreateDate ON bx_Attachments (CreateDate);
GO


GO


CREATE TABLE [bx_AuthenticUsers](
	[UserID]		int				NOT NULL,
	[Realname]		nvarchar(50)	NOT NULL,
	[Gender]		tinyint			NOT NULL,
	[Birthday]		datetime		NOT NULL,
	[IDNumber]		varchar(50)		NOT NULL,
	[IDCardFileFace]	nvarchar(100)	NOT NULL,
	[IDCardFileBack] nvarchar(100)  NOT NULL,
	[Verified]      bit				NOT NULL			CONSTRAINT  [DF_bx_AuthenticUsers_Verified]         DEFAULT (0),
	[Area]			nvarchar(100)	NULL,
	[CreateDate]	datetime		NOT NULL			CONSTRAINT  [DF_bx_AuthenticUsers_CreateDate]         DEFAULT (GETDATE()),
	[Photo]			nvarchar(100)   NULL,
	[Processed]     bit             NOT NULL			CONSTRAINT  [DF_bx_AuthenticUsers_Processed]         DEFAULT (0),
	[OperatorUserID] int            NULL,
	[Remark]		nvarchar(1000)	NULL,
	[DetectedState] int				NULL,
	[IsDetect]		bit				NOT NULL			CONSTRAINT	[DF_bx_AuthenticUsers_IsDetect]			DEFAULT(0),
 CONSTRAINT [PK_bx_AuthenticUsers] PRIMARY KEY CLUSTERED ([UserID] ASC)
)

GO

CREATE INDEX [IX_bx_AuthenticUsers_IDNumber] ON [bx_AuthenticUsers]([IDNumber]);

GO

GO
CREATE TABLE [bx_BannedUsers] (
	 [UserID]			int			NOT NULL 
	,[ForumID]			int			NOT NULL 
	,[BeginDate]		datetime	NULL				CONSTRAINT [DF_bx_BannedUsers_BeginDate] DEFAULT('1753-1-1')
	,[EndDate]			datetime	NOT NULL 			CONSTRAINT [DF_bx_BannedUsers_EndDate] DEFAULT('9999-12-31 23:59:59')
	,[Cause]			nvarchar(1000) COLLATE Chinese_PRC_CI_AS_WS  NULL             CONSTRAINT [DF_bx_BannedUsers_Cause] DEFAULT('')
		CONSTRAINT [PK_bx_BannedUsers] PRIMARY KEY  CLUSTERED ([UserID],[ForumID])
) 
GO
EXEC bx_Drop 'bx_BanUserLogForumInfos';

CREATE	TABLE	bx_BanUserLogForumInfos(
	[LogID]				int					NOT NULL				CONSTRAINT	[DF_bx_BanUserForumInfoLogs_ID]				DEFAULT(0)
	
	,[ForumID]			int					NOT NULL				CONSTRAINT	[DF_bx_BanUserForumInfoLogs_ForumID]		DEFAULT(0)
	
	,[ForumName]		nvarchar(50)		NOT NULL				CONSTRAINT  [DF_bx_BanUserForumInfoLogs_ForumName]		DEFAULT(N'')	

	,[EndDate]			datetime			NOT NULL				CONSTRAINT  [DF_bx_BanUserForumInfoLogs_EndDate]		DEFAULT(getdate())
)
GO
EXEC bx_Drop 'bx_BanUserLogs';

CREATE TABLE bx_BanUserLogs(
	[LogID]				int			IDENTITY(1,1)			NOT NULL			CONSTRAINT	[PK_bx_BanUserLogs]					PRIMARY KEY ([LogID])
  
   ,[OperationType]		tinyint								NOT NULL			CONSTRAINT	[DF_bx_BanUserLogs_OperationType]	DEFAULT(0)
  
   ,[OperationTime]     datetime							NOT NULL			CONSTRAINT  [DF_bx_BanUserLogs_OperationTime]	DEFAULT(GETDATE())
   
   ,[OperatorName]		nvarchar(50)						NOT NULL			CONSTRAINT  [DF_bx_BanUserLogs_OperationName]	DEFAULT(N'')
   
   ,[Cause]				nvarchar(1000)						NOT NULL			CONSTRAINT	[DF_bx_BanUserLogs_Cause]			DEFAULT(N'')
      
   ,[UserID]			int									NOT NULL			CONSTRAINT	[DF_bx_BanUserLogs_UserID]			DEFAULT(0)

   ,[Username]			nvarchar(50)						NOT NULL			CONSTRAINT	[DF_bx_BanUserLogs_Username]		DEFAULT(N'')
   
   ,[UserIP]			varchar(50)							NOT NULL			CONSTRAINT  [DF_bx_BanUserLogs_UserIP]			DEFAULT('')
   
   ,[AllBanEndDate]		datetime							
);

GO

CREATE NONCLUSTERED INDEX [IX_bx_BanUserLogs_UserID] ON [bx_BanUserLogs]([UserID]);

GO
GO
EXEC bx_Drop bx_BlogArchives;

CREATE TABLE bx_BlogArchives(
     [Year]            int                                              NOT NULL
    ,[Month]           int                                              NOT NULL
    ,[UserID]          int                                              NOT NULL 
    ,[TotalBlogs]      int                                              NOT NULL    CONSTRAINT [DF_bx_BlogArchives_TotalBlogs]    DEFAULT (0)
    
    ,CONSTRAINT [PK_bx_BlogArchives] PRIMARY KEY ([UserID], [Year], [Month])
)

/*
Name:日志存档
Columns:
    [Year]           存档的年份
    [Month]          存档的月份
    [UserID]         存档属于者的用户ID
    [TotalBlogs]     该月存档的文章数
*/

GO

GO
EXEC bx_Drop bx_BlogArticles;

GO

CREATE TABLE bx_BlogArticles(
     [ArticleID]        int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT  [PK_bx_BlogArticles]                    PRIMARY KEY ([ArticleID])
    ,[UserID]           int                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_UserID]             DEFAULT (0)
    ,[CategoryID]       int                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_CategoryID]         DEFAULT (0)
    ,[TotalViews]       int                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_TotalViews]         DEFAULT (0)
    ,[TotalComments]    int                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_TotalComments]      DEFAULT (0)
    ,[LastEditUserID]   int                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_LastEditUserID]     DEFAULT (0)

    ,[IsApproved]       bit                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_IsApproved]         DEFAULT (1)        
    ,[EnableComment]    bit                                               NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_EnableComment]      DEFAULT (1)

    ,[PrivacyType]      tinyint                                           NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_PrivacyType]        DEFAULT (0)

    ,[CreateIP]         varchar(50)		  COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_CreateIP]           DEFAULT ('')

    ,[Thumb]            nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_Thumb]              DEFAULT ('')
    ,[Subject]          nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_Subject]            DEFAULT ('')

    ,[Content]          ntext             COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_Content]            DEFAULT ('')

    ,[Password]         nvarchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_Password]           DEFAULT ('')

    ,[CreateDate]       datetime                                          NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_CreateDate]         DEFAULT (GETDATE())
    ,[UpdateDate]       datetime                                          NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_UpdateDate]         DEFAULT (GETDATE())
	,[LastCommentDate]  datetime                                          NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_LastCommentDate]    DEFAULT ('1753-1-1')

    ,[KeywordVersion]   varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_BlogArticles_KeywordVersion]     DEFAULT ('')
)

GO

--创用户ID的索引
CREATE INDEX [IX_bx_BlogArticles_UserID] ON [bx_BlogArticles]([UserID]);

--创建日志隐私类型的索引
CREATE INDEX [IX_bx_BlogArticles_PrivacyType] ON [bx_BlogArticles]([PrivacyType]);

--创建日志分类ID的索引
CREATE INDEX [IX_bx_BlogArticles_CategoryID] ON [bx_BlogArticles]([CategoryID]);

--创建日志发布日期的索引
--CREATE INDEX [IX_bx_BlogArticles_CreateDate] ON [bx_BlogArticles]([CreateDate]);


/*
Name:日志
Columns:
    [ArticleID]        自动标识
    [UserID]           作者ID
    [CategoryID]       分类ID
    [TotalViews]       查看数
    [TotalComments]    回复数
    [LastEditUserID]   最后编辑者ID
    
    [IsApproved]       日志审核 如果包含禁用关键则需审核 IsApproved=false 默认为true
    [EnableComment]    是否允许评论
    
    [PrivacyType]      权限类型 0.所有用户见 1.全好友可见 2.仅自己可见 3.凭密码查看
    
    [CreateIP]         创建者的IP
    
    [Thumb]            日志略缩图
    [Subject]          标题
    [Password]         凭密码查看时的密码 
    [SubjectReverter]  标题还原关键字信息
    
    [Content]          内容
    [ContentReverter]  内容还原关键字信息
    
    [CreateDate]       添加时间
    [UpdateDate]       编辑时间
    [LastCommentDate]  最后评论时间
*/
GO
EXEC bx_Drop bx_BlogArticleReverters;

GO

CREATE TABLE [bx_BlogArticleReverters](
	[ArticleID]				int				NOT NULL,
	[SubjectReverter]		nvarchar(4000)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,
	[ContentReverter]		ntext			COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_BlogArticleReverters] PRIMARY KEY([ArticleID])
)

/*
Name:标签
Columns:
    [ArticleID]	       可恢复的博客文章ID
	[SubjectReverter]         标题复原关键信息，可根据此信息恢复标题的原始内容
	[ContentReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO



GO
EXEC bx_Drop bx_BlogArticleVisitors;


CREATE TABLE [bx_BlogArticleVisitors] (
       [ID]                       int        IDENTITY(1,1)       NOT NULL   
      ,[BlogArticleID]            int                            NOT NULL
      ,[UserID]                   int                            NOT NULL
      
      ,[ViewDate]                 datetime                       NOT NULL    CONSTRAINT [DF_bx_bx_BlogArticleVisitors_ViewDate]    DEFAULT(GETDATE())
      
      ,CONSTRAINT [PK_bx_BlogArticleVisitors] PRIMARY KEY([ID])
)


/*
Name: 日志访问表, 同一篇日志
Column:
	  [ID]
      [BlogArticleID]            日志ID
      [UserID]                   访问该日志的用户ID
      
      [ViewDate]                 访问时间
*/

GO
GO
EXEC bx_Drop bx_BlogCategories;

GO

CREATE TABLE [bx_BlogCategories] (
     [CategoryID]      int             IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_BlogCategories]                 PRIMARY KEY ([CategoryID])
    ,[UserID]          int                                             NOT NULL 
    ,[TotalArticles]   int                                             NOT NULL    CONSTRAINT [DF_bx_BlogCategories_TotalBlogs]      DEFAULT (0)
    
    ,[Name]            nvarchar(50)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_BlogCategories_Name]            DEFAULT ('')
    
    ,[CreateDate]      datetime                                        NOT NULL    CONSTRAINT [DF_bx_BlogCategories_CreateDate]      DEFAULT (GETDATE())
    ,[KeywordVersion]  varchar(32)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_BlogCategories_KeywordVersion]  DEFAULT ('')
)

/*
Name:日志分类
Columns:
    [CategoryID]            自动标识
    [UserID]        用户ID
    [TotalArticles] 该分类文章数
    
    [Name]          日志分类名
    
    [CreateDate]    添加时间
*/

GO


--创建日志分类的用户ID索引
CREATE INDEX [IX_bx_BlogCategories_UserID] ON [bx_BlogCategories]([UserID]) 

GO
EXEC bx_Drop bx_BlogCategoryReverters;

GO

CREATE TABLE [bx_BlogCategoryReverters](
	[CategoryID]		    int				NOT NULL,
	[NameReverter]			nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_BlogCategoryReverters] PRIMARY KEY([CategoryID])
)

/*
Name:标签
Columns:
    [AlbumID]		       可恢复的相册ID
	[NameReverter]         相册名复原关键信息，可根据此信息恢复相册名的原始内容
*/

GO
GO
EXEC bx_Drop bx_ChatMessageReverters;

GO

CREATE TABLE bx_ChatMessageReverters(
	[MessageID]					int												NOT NULL,
	[ContentReverter]			ntext           COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_ChatMessageReverters] PRIMARY KEY([MessageID])
)

/*
Name:标签
Columns:
    [MessageID]	                  可恢复的消息ID
	[ContentReverter]             内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO



GO
--消息表
EXEC bx_Drop bx_ChatMessages;

CREATE TABLE [bx_ChatMessages] (
     [MessageID]            int                    IDENTITY(1,1)                        NOT NULL

	,[UserID]               int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_UserID]             DEFAULT(0)
	,[TargetUserID]         int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_TargetUserID]       DEFAULT(0)
	,[IsReceive]            bit                                                         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_IsReceive]          DEFAULT(0)
	,[IsRead]               bit                                                         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_IsRead]             DEFAULT(0)
	,[FromMessageID]        int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_FromMessageID]      DEFAULT(0)

	,[Content]              nvarchar(3000)         COLLATE Chinese_PRC_CI_AS_WS         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_Content]            DEFAULT('')
	,[KeywordVersion]       varchar(32)            COLLATE Chinese_PRC_CI_AS_WS         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_KeywordVersion]     DEFAULT('')

	,[CreateIP]             varchar(50)            COLLATE Chinese_PRC_CI_AS_WS         NOT NULL    CONSTRAINT [DF_bx_ChatMessages_CreateIP]           DEFAULT('')
	,[CreateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_ChatMessages_CreateDate]         DEFAULT(GETDATE())

	
	,CONSTRAINT [PK_bx_ChatMessages] PRIMARY KEY ([MessageID])
);

/*
Name:     消息表,包括用户消息,系统消息,通知提醒消息
Columns:
          [MessageID]
          [UserID]                              始终表示这个消息的拥有者的ID。
          [TargetUserID]                        表示对方的UserID
          [IsReceive]                           true表示这是一条接收到的消息。如果true，表示这是UserID接收自TargetUserID的消息，否则就是从UserID发送给TargetUserID的消息
          [IsRead]                              [IsReceive]为true的时候这个字段才有意义。表示消息是否已读，否则且值始终保持1
          [FromMessageID]                       [IsReceive]为true的时候这个字段才有意义。表示接收到的这条消息来自于哪条消息，即
          
          [Content]                             消息内容
          [ReplaceVersion]                      文本替换版本，用来效验

          [CreateIP]                            发件人的IP 
          [CreateDate]                          消息发送时间
*/

GO

--用户ID索引
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_ChatMessages_List] ON [bx_ChatMessages]([UserID], [IsRead], [MessageID])
GO
GO
--消息表
EXEC bx_Drop bx_ChatSessions;

CREATE TABLE [bx_ChatSessions] (
	 [ChatSessionID]        int                   IDENTITY(1, 1)                        NOT NULL
	,[UserID]               int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatSessions_UserID]             DEFAULT(0)
	,[TargetUserID]         int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatSessions_TargetUserID]       DEFAULT(0)

	,[TotalMessages]        int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatSessions_TotalMessages]      DEFAULT(0)
	,[UnreadMessages]       int                                                         NOT NULL    CONSTRAINT [DF_bx_ChatSessions_UnreadMessages]     DEFAULT(0)
	
	,[LastMessage]          nvarchar(3000)         COLLATE Chinese_PRC_CI_AS_WS         NOT NULL    CONSTRAINT [DF_bx_ChatSessions_LastMessage]        DEFAULT(N'')

	,[CreateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_ChatSessions_CreateDate]         DEFAULT(GETDATE())
	,[UpdateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_ChatSessions_UpdateDate]         DEFAULT(GETDATE())

	,CONSTRAINT [PK_bx_ChatSessions] PRIMARY KEY ([ChatSessionID])
);

/*
Name:     
Columns:

*/

GO

--用户ID索引
CREATE NONCLUSTERED INDEX [IX_bx_ChatSessions_List] ON [bx_ChatSessions]([UserID], [UpdateDate])

CREATE NONCLUSTERED INDEX [IX_bx_ChatSessions_Unread] ON [bx_ChatSessions]([UserID], [UnreadMessages], [UpdateDate])

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_ChatSessions_User] ON [bx_ChatSessions]([UserID], [TargetUserID])
GO
GO
--点击记录
EXEC bx_Drop 'bx_ClickLogs';

CREATE TABLE [bx_ClickLogs] (
     [ID]            int                    IDENTITY(1,1)                               NOT NULL

	,[UserIdentify]         varchar(200)                                                NOT NULL    CONSTRAINT [DF_bx_ClickLogs_UserIdentify]          DEFAULT(0)
	,[Ip]                   varchar(50)                                                 NULL    
	,[ClickDate]            datetime                                                    NOT NULL    CONSTRAINT [DF_bx_ClickLogs_ClickDate]			   DEFAULT(getdate())
	,[SourceType]           int                                                         NOT NULL    CONSTRAINT [DF_bx_ClickLogs_SourceType]         DEFAULT(0)
	,[TargetID]             int                                                         NOT NULL    CONSTRAINT [DF_bx_ClickLogs_TargetID]              DEFAULT(0)

	,CONSTRAINT [PK_bx_ClickLogs] PRIMARY KEY ([ID])
);

/*
点击记录：
[UserIdentify]:可能是游客的GuestID 或者用户的 UserID
[SourceType]  :被点击的对象枚举
[TargetID]    :被点击的对象ID

*/

GO

GO
EXEC bx_Drop bx_ClubCategories;

GO

CREATE TABLE [bx_ClubCategories] (
     [CategoryID]      int             IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_ClubCategories]                 PRIMARY KEY ([CategoryID])
    ,[SortOrder]       int                                             NOT NULL    CONSTRAINT [DF_bx_ClubCategories_SortOrder]       DEFAULT (0)
    ,[TotalClubs]      int                                             NOT NULL    CONSTRAINT [DF_bx_ClubCategories_TotalBlogs]      DEFAULT (0)
    
    ,[Name]            nvarchar(50)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    
    ,[CreateDate]      datetime                                        NOT NULL    CONSTRAINT [DF_bx_ClubCategories_CreateDate]      DEFAULT (GETDATE())
)

/*
Name:群组分类
Columns:
    [CategoryID]    主键
    [TotalClubs]    该分类群组数（冗余）
    
    [Name]          分类名称
    
    [CreateDate]    添加时间
*/

GO

GO
EXEC bx_Drop bx_ClubMembers;

GO

CREATE TABLE [bx_ClubMembers] (
     [ClubID]          int                         NOT NULL    CONSTRAINT [DF_bx_ClubMembers_ClubID]          DEFAULT (0)
    ,[UserID]          int                         NOT NULL    CONSTRAINT [DF_bx_ClubMembers_UserID]          DEFAULT (0)
    ,[SortOrder]       int        identity(1,1)    NOT NULL
    ,[Status]          tinyint                     NOT NULL    CONSTRAINT [DF_bx_ClubMembers_Status]          DEFAULT (0)
    
    ,[CreateDate]      datetime                    NOT NULL    CONSTRAINT [DF_bx_ClubMembers_CreateDate]      DEFAULT (GETDATE())
    
    ,CONSTRAINT        [PK_bx_ClubMembers]   PRIMARY KEY ([ClubID], [UserID])
)

CREATE INDEX [IX_bx_ClubMembers_Status] ON [bx_ClubMembers]([Status]);

/*
Name:群组分类
Columns:
    [ClubID]        群组ID
    [UserID]        用户ID
    [Status]        成员状态：等待验证、禁言、普通会员、管理员、群主
    [CreateDate]    创见时间
*/

GO

GO
EXEC bx_Drop bx_Clubs;

GO

CREATE TABLE bx_Clubs(
     [ClubID]           int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT  [PK_bx_Clubs]                    PRIMARY KEY ([ClubID])
    ,[UserID]           int                                               NOT NULL    CONSTRAINT  [DF_bx_Clubs_UserID]             DEFAULT (0)
    ,[CategoryID]       int                                               NOT NULL    CONSTRAINT  [DF_bx_Clubs_CategoryID]         DEFAULT (0)
    ,[TotalViews]       int                                               NOT NULL    CONSTRAINT  [DF_bx_Clubs_TotalViews]         DEFAULT (0)
    ,[TotalMembers]     int                                               NOT NULL    CONSTRAINT  [DF_bx_Clubs_TotalMembers]       DEFAULT (0)
    
    ,[IsApproved]       bit                                               NOT NULL    CONSTRAINT  [DF_bx_Clubs_IsApproved]         DEFAULT (0)
    ,[IsNeedManager]    bit                                               NOT NULL    CONSTRAINT  [DF_bx_Clubs_IsNeedManager]      DEFAULT (1)
    
    ,[JoinMethod]       tinyint                                           NOT NULL    CONSTRAINT  [DF_bx_Clubs_JoinMethod]         DEFAULT (0)
    ,[AccessMode]       tinyint                                           NOT NULL    CONSTRAINT  [DF_bx_Clubs_AccessMode]         DEFAULT (0)
    
    
    ,[CreateIP]         varchar(50)		  COLLATE Chinese_PRC_CI_AS_WS	  NOT NULL    CONSTRAINT  [DF_bx_Clubs_CreateIP]           DEFAULT ('')
    
    ,[Name]             nvarchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Clubs_Name]               DEFAULT ('')
    ,[IconSrc]          nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Clubs_IconSrc]            DEFAULT ('')
    ,[Description]      nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Clubs_Description]        DEFAULT ('')

    ,[CreateDate]       datetime                                          NOT NULL    CONSTRAINT  [DF_bx_Clubs_CreateDate]         DEFAULT (GETDATE())
    ,[UpdateDate]       datetime                                          NOT NULL    CONSTRAINT  [DF_bx_Clubs_UpdateDate]         DEFAULT (GETDATE())
	
    ,[KeywordVersion]   varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Clubs_KeywordVersion]     DEFAULT ('')
)

GO

CREATE INDEX [IX_bx_Clubs_UserID] ON [bx_Clubs]([UserID]);

CREATE INDEX [IX_bx_Clubs_CategoryID] ON [bx_Clubs]([CategoryID]);

CREATE INDEX [IX_bx_Clubs_IsApproved] ON [bx_Clubs]([IsApproved]);

/*
Name:群组表
Columns:
    [ClubID]           主键，没什么好说的
    [UserID]           创建者ID
    [CategoryID]       群组分类ID
    [TotalViews]       群组总访问数（冗余）
    [TotalMembers]     群组总用户数（冗余）
    
    [IsApproved]       群组是否通过审核
    [IsNeedManager]    群组是否招纳管理员
    
    [JoinMethod]       群组加入方式（随便加或者要审批）
    [AccessMode]       群组访问模式（公开或不公开）
    
    
    [CreateIP]         创建者IP（给警察叔叔用）
    
    [Name]             群组名称
    [IconSrc]          群组图标的地址
    [Description]      群组的描述或者公告
    
    [CreateDate]       创建时间
    [UpdateDate]       修改时间
    
    [KeywordVersion]   关键字版本
*/
GO
EXEC bx_Drop bx_CommentReverters;

GO

CREATE TABLE [bx_CommentReverters](
	[CommentID]				int				NOT NULL,
	[ContentReverter]		ntext	COLLATE Chinese_PRC_CI_AS_WS		NOT NULL,

	CONSTRAINT [PK_bx_CommentReverters] PRIMARY KEY([CommentID])
)

/*
Name:标签
Columns:
    [CommentID]	              可恢复的评论ID
	[ContentReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO



GO
EXEC bx_Drop 'bx_Comments';

CREATE TABLE bx_Comments(
     [CommentID]          int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_Comments]                    PRIMARY KEY ([CommentID])
    ,[Type]               int                                               NOT NULL    CONSTRAINT [DF_bx_Comments_Type]            DEFAULT (0)    
    ,[UserID]             int                                               NOT NULL    CONSTRAINT [DF_bx_Comments_UserID]          DEFAULT (0)
    ,[TargetID]           int                                               NOT NULL    CONSTRAINT [DF_bx_Comments_TargetID]        DEFAULT (0)
    ,[TargetUserID]       int                                               NULL
    ,[LastEditUserID]     int                                               NOT NULL    CONSTRAINT [DF_bx_Comments_LastEditUserID]  DEFAULT (0)
     
    ,[IsApproved]         bit                                               NOT NULL    CONSTRAINT [DF_bx_Comments_IsApproved]      DEFAULT (1)
     
    ,[Content]            nvarchar(3000)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Comments_Content]         DEFAULT ('')
    
    ,[CreateIP]           varchar(50)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Comments_CreateIP]        DEFAULT ('')
    
    ,[CreateDate]         datetime                                          NOT NULL    CONSTRAINT [DF_bx_Comments_CreateDate]      DEFAULT (GETDATE())
    ,[KeywordVersion]     varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Comments_ContentVersion]  DEFAULT ('')
)

/*
Name:评论
Columns:
    [CommentID]                 自动标识
    [Type]               评论应用类型 1.留言板 2.相片 3.日志 4.状态
    [UserID]             评论者ID 留言者ID
    [TargetID]           被评论的ID 相片ID 日志ID 状态ID 被留言用户ID
    [TargetUserID]       被评论的用户ID 留言时冗余
    [LastEditUserID]     最后编辑者
    
    [IsApproved]         评论审核 如果包含禁用关键则需审核 IsApproved=false 默认为true
    
    [Content]            评论内容
    [ContentReverter]    用于替换后还原的内容
    
    [CreateIP]           评论者的IP
    [KeywordVersion]     关键字版本
    
    [CreateDate]         评论时间
*/

GO

EXEC bx_Drop 'IX_bx_Comments_TargetID';
CREATE  INDEX [IX_bx_Comments_TargetID] ON [bx_Comments]([TargetID])

GO

GO
EXEC bx_Drop 'bx_DayLastThreads';

CREATE TABLE [bx_DayLastThreads]
(
[Day] [int] NOT NULL, --- 2010-4-15  则为 2010415
[LastThreadID] [int]  NOT NULL,
CONSTRAINT [PK_bx_DayLastThreads] PRIMARY KEY ([Day])
)

GO


GO
EXEC bx_Drop bx_DeletingFiles;

CREATE TABLE [bx_DeletingFiles] (
	[DeletingFileID]			int	IDENTITY(1,1)							    NOT NULL
    ,[ServerFilePath]		nvarchar(256)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL
    
    ,CONSTRAINT [PK_bx_DeletingFiles] PRIMARY KEY ([DeletingFileID])
);

/*
Name:    已删除的文件表
Columns:
        [DeletingFileID]             主键，唯一标识，正在删除的文件ID
        [ServerFilePath]            文件保存路径,相对路径
*/

GO
GO
EXEC bx_Drop 'bx_DenouncingContents';

CREATE TABLE bx_DenouncingContents(
     [DenouncingID]       int                                               NOT NULL
    ,[UserID]             int                                               NOT NULL

    ,[Content]            nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    
    ,[CreateDate]         datetime                                          NOT NULL    CONSTRAINT [DF_bx_DenouncingContents_CreateDate]      DEFAULT (GETDATE())

    ,CONSTRAINT [PK_bx_DenouncingContents] PRIMARY KEY ([DenouncingID],[UserID])
)

GO


GO
EXEC bx_Drop 'bx_Denouncings';

CREATE TABLE bx_Denouncings(
     [DenouncingID]       int                   IDENTITY(1, 1)              NOT NULL    CONSTRAINT [PK_bx_Denouncings]                 PRIMARY KEY ([DenouncingID])
    
    ,[TargetID]           int                                               NOT NULL
    ,[TargetUserID]       int                                               NOT NULL
  
    ,[Type]               tinyint                                           NOT NULL
   
    ,[IsIgnore]           bit                                               NOT NULL    CONSTRAINT [DF_bx_Denouncings_IsIgnore]        DEFAULT(0)
    
    ,[CreateDate]         datetime                                          NOT NULL    CONSTRAINT [DF_bx_Denouncings_CreateDate]      DEFAULT (GETDATE())
)

CREATE INDEX [IX_bx_Denouncings_IsIgnore] ON [bx_Denouncings]([IsIgnore]);
CREATE INDEX [IX_bx_Denouncings_Type_TargetID] ON [bx_Denouncings]([Type],[TargetID]);

/*
Name:状态
Columns:
    [ReportID]
    [UserID]            用户ID
    [TargetID]          举报对象的ID
    
    [Type]              举报对象类型
    [Reason]            举报原因
    
    [IsIgnore]          是否已忽略
    
    [Content]           举报内容
    
    [CreateIP]          IP地址
    
    [CreateDate]        添加时间
*/

GO


GO
--网络硬盘目录表 
EXEC bx_Drop bx_DiskDirectories;

CREATE TABLE [bx_DiskDirectories] (
      [DirectoryID]                   int                     IDENTITY(1,1)                       NOT NULL
      
     ,[Name]                 nvarchar(256)           COLLATE Chinese_PRC_CI_AS_WS        NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_Name]                  DEFAULT('')
     ,[Password]             nvarchar(50)            COLLATE Chinese_PRC_CI_AS_WS        NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_Password]              DEFAULT('')
    
     ,[PrivacyType]          tinyint                                                     NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_PrivacyType]           DEFAULT(2)
     
     ,[UserID]               int                                                         NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_UserID]                DEFAULT(0)
     ,[ParentID]             int                                                         NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_ParentID]              DEFAULT(0)
     ,[TotalFiles]           int                                                         NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_TotalFiles]            DEFAULT(0)
     ,[TotalSubDirectories]  int                                                         NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_TotalSubDirectories]   DEFAULT(0)
     
     ,[TotalSize]            bigint                                                      NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_TotalSize]             DEFAULT(0)
     
     ,[CreateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_CreateDate]            DEFAULT(GETDATE())
     ,[UpdateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_UpdateDate]            DEFAULT(GETDATE())
     
     ,CONSTRAINT [PK_bx_DiskDirectories] PRIMARY KEY ([DirectoryID])
);  

/*
Name:     网络硬盘目录表
Columns:
          [ID]     
                      
          [Name]                                          目录名称
          [Password]                                      当目录隐私类型为3时,需要此密码才可以查看此目录
          
          [PrivacyType]                                   目录隐私类型,0 全站可见  1 好友可见  2 只有自己可见 3 提供密码可见
          
          [UserID]                                        目录拥有者ID
          [ParentID]                                      目录的父目录,若顶级目录,则为0
          [TotalFiles]                                    目录子文件个数
          [TotalSubDirectories]                           目录子目录个数
          
          [TotalSize]                                     目录总大小..只存该目录下的文件的总大小,内部用,不显示给用户
          
          [CreateDate]                                    目录创建日期
          [UpdateDate]                                    目录最后更新时间
*/

GO


--ID索引
EXEC bx_Drop 'IX_bx_DiskDirectories_ID';
CREATE INDEX [IX_bx_DiskDirectories_ID] ON [bx_DiskDirectories]([DirectoryID])

--用户ID索引
EXEC bx_Drop 'IX_bx_DiskDirectories_UserID';
CREATE INDEX [IX_bx_DiskDirectories_UserID] ON [bx_DiskDirectories]([UserID])

--父目录ID索引
EXEC bx_Drop 'IX_bx_DiskDirectories_ParentID';
CREATE INDEX [IX_bx_DiskDirectories_ParentID] ON [bx_DiskDirectories]([ParentID])

GO
GO
--网络硬盘文件表
EXEC bx_Drop bx_DiskFiles;

CREATE TABLE [bx_DiskFiles] (
     [DiskFileID]          int                     IDENTITY(1,1)                           NOT NULL
    ,[FileID]              varchar(50)             COLLATE Chinese_PRC_CI_AS_WS            NOT NULL     CONSTRAINT [DF_bx_DiskFiles_FileID]           DEFAULT('')
    
    ,[FileName]            nvarchar(256)           COLLATE Chinese_PRC_CI_AS_WS            NOT NULL     CONSTRAINT [DF_bx_DiskFiles_Filename]         DEFAULT(N'')
    ,[Extension]           nvarchar(10)            COLLATE Chinese_PRC_CI_AS_WS            NOT NULL     CONSTRAINT [DF_bx_DiskFiles_Extension]        DEFAULT(N'')
	,[ThumbPath]           nvarchar(256)           COLLATE Chinese_PRC_CI_AS_WS            NOT NULL     CONSTRAINT [DF_bx_DiskFiles_ThumbPath]        DEFAULT(N'')
    ,[FileSize]            bigint                                                          NOT NULL     CONSTRAINT [DF_bx_DiskFiles_FileSize]		  DEFAULT(0)

    ,[UserID]              int                                                             NOT NULL     CONSTRAINT [DF_bx_DiskFiles_UserID]           DEFAULT(0)
    ,[DirectoryID]         int                                                             NOT NULL     CONSTRAINT [DF_bx_DiskFiles_DirectoryID]      DEFAULT(0)
    ,[TotalDownloads]      int                                                             NOT NULL     CONSTRAINT [DF_bx_DiskFiles_TotalDownloads]   DEFAULT(0)
    
    ,[CreateDate]          datetime                                                        NOT NULL     CONSTRAINT [DF_bx_DiskFiles_CreateDate]       DEFAULT(GETDATE())
    ,[UpdateDate]          datetime                                                        NOT NULL     CONSTRAINT [DF_bx_DiskFiles_UpdateDate]       DEFAULT(GETDATE())
    ,[ExtensionInfo]       ntext				   COLLATE Chinese_PRC_CI_AS_WS			   NULL     	 
    ,CONSTRAINT [PK_bx_DiskFiles] PRIMARY KEY ([DiskFileID])
);

/*
Name:      网络硬盘文件表,此为虚拟文件,有与之相对应的真实文件表bx_Files
Columns:
        [ID]
        [FileID]                                      对应的真实文件的数据的ID
        
        [Filename]                                    文件名
        [Extension]                                   文件扩展名,如果如果超过10的不当作后缀名
        
        [UserID]                                      文件所属用户ID
        [DirectoryID]                                 文件所属目录ID
        [TotalDownloads]                              文件下载次数,自己浏览不计算进去
        
        [CreateDate]                                  文件创建时间
        [UpdateDate]                                  文件更新时间
*/

GO

--文件名索引
CREATE INDEX [IX_bx_DiskFiles_FileName] ON [bx_DiskFiles] ([FileName])

--文件夹索引
CREATE INDEX [IX_bx_DiskFiles_DirectoryID] ON [bx_DiskFiles] ([DirectoryID])

--文件ID索引
CREATE INDEX [IX_bx_DiskFiles_FileID] ON [bx_DiskFiles] ([FileID]);

CREATE INDEX [IX_bx_DiskFiles_UserID] ON [bx_DiskFiles] ([UserID]);
GO
EXEC bx_Drop bx_DoingReverters;

GO

CREATE TABLE [bx_DoingReverters](
	[DoingID]				int					NOT NULL,
	[ContentReverter]		nvarchar(4000)  	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_DoingReverters] PRIMARY KEY([DoingID])
)

/*
Name:标签
Columns:
    [DoingID]	              可恢复的评论ID
	[ContentReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO



GO
EXEC bx_Drop 'bx_Doings';

CREATE TABLE bx_Doings(
     [DoingID]            int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_Doings]                  PRIMARY KEY ([DoingID])
    ,[UserID]             int                                               NOT NULL
    ,[TotalComments]      int                                               NOT NULL    CONSTRAINT [DF_bx_Doings_TotalComments]    DEFAULT(0)
  
    ,[Content]            nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    
    ,[CreateIP]           varchar(50)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Doings_CreateIP]         DEFAULT('')
    
    ,[CreateDate]         datetime                                          NOT NULL    CONSTRAINT [DF_bx_Doings_CreateDate]       DEFAULT (GETDATE())
	,[LastCommentDate]    datetime                                          NOT NULL    CONSTRAINT [DF_bx_Doings_LastCommentDate]  DEFAULT ('1753-1-1')
	
    ,[KeywordVersion]     varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Doings_ContentVersion]   DEFAULT('')
)

/*
Name:状态
Columns:
    [DoingID]            
    [UserID]            用户ID
    [TotalComments]     回复数
    
    [Content]           状态信息
    [ContentReverter]      
    
    [CreateIP]          IP地址
    
    [CreateDate]        添加时间
*/

GO

EXEC bx_Drop 'IX_bx_Doings_UserID';
CREATE  INDEX [IX_bx_Doings_UserID] ON [bx_Doings]([UserID])

GO


GO
CREATE TABLE [bx_EmoticonGroups] (
	 [GroupID]			int					IDENTITY (1, 1)					NOT NULL		 CONSTRAINT  [PK_bx_EmoticonGroups]					PRIMARY KEY ([GroupID])
	,[GroupName]		nvarchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL		 
	,[UserID]			int													NOT NULL		 
	,[TotalEmoticons]	int													NOT NULL		CONSTRAINT  [DF_bx_EmoticonGroups_TotalEmoticons]	DEFAULT(0)
	,[TotalSizes]		int													NOT NULL		CONSTRAINT  [DF_bx_EmoticonGroups_TotalSizes]		DEFAULT(0)
) 

GO

--分组索引
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_EmoticonGroups_UserID] ON [bx_EmoticonGroups]([UserID],[GroupName])

GO

CREATE TABLE [bx_Emoticons] (
	 [EmoticonID]		int		IDENTITY (1, 1)							NOT NULL	CONSTRAINT [PK_bx_Emoticons] PRIMARY KEY  ([EmoticonID])
	,[UserID]			int												NOT NULL	CONSTRAINT [DF_bx_Emoticons_UserID]			DEFAULT(0)
	,[GroupID]			int												NOT NULL	CONSTRAINT [DF_bx_Emoticons_GroupID]		DEFAULT(0)
	,[Shortcut]			nvarchar(100)	COLLATE Chinese_PRC_CI_AS_WS	NULL		CONSTRAINT [DF_bx_Emoticons_Shortcut]		DEFAULT('')
	,[ImageSrc]			varchar(255)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL
	,[FileSize]			int												NOT NULL	CONSTRAINT [DF_bx_Emoticons_FileSize]		DEFAULT(0)
	,[MD5]				varchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NULL	
	,[SortOrder]		int												NOT NULL	CONSTRAINT [DF_bx_Emoticons_SortOrder]		DEFAULT(0)
)

GO

--同一个用户只能创建一个同样的表情
CREATE NONCLUSTERED INDEX [IX_bx_Emoticons_List] ON [bx_Emoticons] ([UserID], [ImageSrc])

--分组索引
CREATE INDEX [IX_bx_Emoticons_GroupID] ON [bx_Emoticons] ([GroupID])
GO

GO
EXEC bx_Drop 'bx_FeedFilters';

CREATE TABLE [bx_FeedFilters] (
     [ID]               int                 IDENTITY (1, 1)                 NOT NULL 
    ,[UserID]           int                                                 NOT NULL 
    ,[FriendUserID]     int                                                 NULL
    
    ,[AppID]            uniqueidentifier                                    NOT NULL 
    
    ,[ActionType]       tinyint                                             NULL
    
    ,CONSTRAINT [PK_bx_FeedFilters] PRIMARY KEY ([ID])
);

/*
Name: 通知过滤表  不允许AppID,FriendUserID同时为null
Columns:
    [ID]               唯一标志
    [UserID]           用户ID
    [FriendUserID]     好友用户ID 为null时 过滤所有好友
    
    [AppID]            应用ID 为Guid.Empty时 过滤所有应用
    
    [ActionType]       应用的动作类型 为null时候过滤该应用的所有动作
*/
 
GO

EXEC bx_Drop 'IX_bx_FeedFilters_UserFriend';
CREATE  INDEX [IX_bx_FeedFilters_UserFriend] ON [bx_FeedFilters]([UserID],[FriendUserID]) 


EXEC bx_Drop 'IX_bx_FeedFilters_Action';
CREATE  INDEX [IX_bx_FeedFilters_Action] ON [bx_FeedFilters]([AppID],[ActionType])
GO

GO
EXEC bx_Drop 'bx_Feeds';

CREATE TABLE [bx_Feeds] (
     [ID]               int                 IDENTITY (1, 1)                 NOT NULL 
    ,[TargetID]         int                                                 NULL
    ,[TargetUserID]     int                                                 NOT NULL 
    ,[CommentTargetID]  int                                                 NOT NULL    CONSTRAINT [DF_bx_Feeds_CommentTargetID]   DEFAULT (0)

    ,[ActionType]       tinyint                                             NOT NULL 
    ,[PrivacyType]      tinyint                                             NOT NULL 
    
    ,[AppID]            uniqueidentifier                                    NOT NULL 

    ,[Title]            nvarchar(1000)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Feeds_Title]          DEFAULT ('')
	,[Description]      nvarchar(2500)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Feeds_Description]    DEFAULT ('')
    ,[TargetNickname]   nvarchar(50)        COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Feeds_TargetNickname] DEFAULT ('')
	
	,[VisibleUserIDs]   varchar(800)        COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Feeds_VisibleUserIDs] DEFAULT ('')
	,[CommentInfo]		varchar(50)         COLLATE Chinese_PRC_CI_AS_WS    NULL		--CONSTRAINT [DF_bx_Feeds_CommentInfo]	DEFAULT ('')
	
	,[CreateDate]       datetime                                            NOT NULL    CONSTRAINT [DF_bx_Feeds_CreateDate]     DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_Feeds] PRIMARY KEY ([ID])
);

/*
Name: 通知表
Columns:
    [ID]               唯一标志
    [TargetID]         通用的目标ID（有需要时使用 例如:相册上传图片，就记相册ID 方便处理上传多张图片时只记一个通知）
    [TargetUserID]     相关联的目标UserID,如aa评论了bb的日志，就是bb的userID;cc和dd成为好友,就是dd的userID
 
    [ActionType]       APP的动作枚举值
    [PrivacyType]      隐私类型
    
    [AppID]            应用ID
    
    [Title]            通知标题
    [Description]      通知简介
    
    [CreateDate]       时间
*/

GO

EXEC bx_Drop 'IX_bx_Feeds_TargetUserID';
CREATE  INDEX [IX_bx_Feeds_TargetUserID] ON [bx_Feeds]([TargetUserID])

EXEC bx_Drop 'IX_bx_Feeds_Action';
CREATE  INDEX [IX_bx_Feeds_Action] ON [bx_Feeds]([AppID],[ActionType])

EXEC bx_Drop 'IX_bx_Feeds_TargetID';
CREATE  INDEX [IX_bx_Feeds_TargetID] ON [bx_Feeds]([TargetID])

EXEC bx_Drop 'IX_bx_Feeds_CreateDate';
CREATE  INDEX [IX_bx_Feeds_CreateDate] ON [bx_Feeds]([CreateDate])

GO

GO
EXEC bx_Drop 'bx_FeedTemplates';

CREATE TABLE [bx_FeedTemplates] (
     [AppID]                uniqueidentifier                                NOT NULL
 
    ,[ActionType]           tinyint                                         NOT NULL
 
    ,[Title]                nvarchar(1000)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_FeedTemplates_Title]          DEFAULT ('')
    ,[IconUrl]              nvarchar(200)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_FeedTemplates_IconUrl]        DEFAULT ('')
    ,[Description]          nvarchar(2500)  COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_FeedTemplates_Description]    DEFAULT ('')
 
    ,CONSTRAINT [PK_bx_FeedTemplates] PRIMARY KEY ([AppID],[ActionType])
	
);

/*
Name: 通知模板表
Columns:
    [AppID]            应用ID
    
    [ActionType]       APP的动作枚举值
    
    [Title]            通知标题模板  例如：{用户}发表了日志{日志标题} --({自定义变量})
    [IconUrl]          图标URL  
    [Description]      简介模板  例如： {用户}分享日志 {日志标题}<br />来自：{日志作者} <br />{描述} <br />评论该分享
*/

GO

GO
EXEC bx_Drop bx_Files;

CREATE TABLE [bx_Files] (
     [FileID]              varchar(50)          COLLATE Chinese_PRC_CI_AS_WS               NOT NULL    CONSTRAINT  [DF_bx_Files_FileID]				DEFAULT ('')
     
    ,[ServerFilePath]      nvarchar(256)        COLLATE Chinese_PRC_CI_AS_WS               NOT NULL    CONSTRAINT  [DF_bx_Files_ServerFilePath]		DEFAULT (N'')
    
    ,[MD5]                 char(32)             COLLATE Chinese_PRC_CI_AS_WS               NOT NULL    CONSTRAINT  [DF_bx_Files_MD5]				DEFAULT ('')
    ,[FileSize]            bigint                                                          NOT NULL    CONSTRAINT  [DF_bx_Files_FileSize]			DEFAULT (0)
    
    ,CONSTRAINT [PK_bx_Files] PRIMARY KEY ([FileID])
);

/*
Name:    真实文件表
Columns:
        [FileID]                    主键，唯一标识，文件ID
        
        [ServerFilePath]            文件保存路径,相对路径
        
        [MD5]                       文件MD5值
        [FileSize]                  文件大小
*/

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Files_Key] ON [bx_Files]([MD5], [FileSize])

GO
GO
EXEC bx_Drop 'bx_Forums';

CREATE TABLE [bx_Forums] (
		[ForumID]				int													NOT NULL IDENTITY(1, 1),
		[ParentID]				int													NOT NULL,
		[ClubID]			    int													NOT NULL CONSTRAINT [DF_bx_Forums_ClubID]									    DEFAULT (0),
		[ForumType]				tinyint												NOT NULL,
		[ForumStatus]			tinyint			    								NOT NULL CONSTRAINT [DF_bx_Forums_ForumStatus]									DEFAULT (0),
		[ThreadCatalogStatus]	tinyint												NOT NULL CONSTRAINT [DF_bx_Forums_ThreadCatalogStatus]							DEFAULT (2),

		[VisitPaths]			nvarchar(2000)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL CONSTRAINT [DF_bx_Forums_VisitPaths]									DEFAULT (''),

		--[SubDomain]				nvarchar(100)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL CONSTRAINT [DF_bx_Forums_SubDomain]									DEFAULT (''),
		[CodeName]				nvarchar(100)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL CONSTRAINT [DF_bx_Forums_CodeName]										DEFAULT (''),

		[ForumName]				nvarchar(500)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL,
		[Description]			ntext				COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL,
		[Readme]				ntext				COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL,
		[LogoSrc]				nvarchar(200)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL CONSTRAINT [DF_bx_Forums_LogoSrc]										DEFAULT (''),
		[ThemeID]				nvarchar(100)		COLLATE Chinese_PRC_CI_AS_WS 	NOT NULL CONSTRAINT [DF_bx_Forums_ThemeID]										DEFAULT (''),
		[ColumnSpan]			tinyint												NOT NULL CONSTRAINT [DF_bx_Forums_ColumnSpan]									DEFAULT (0),
		[TotalThreads]			int													NOT NULL CONSTRAINT [DF_bx_Forums_ThreadCount]									DEFAULT (0),
		[TotalPosts]			int													NOT NULL CONSTRAINT [DF_bx_Forums_PostCount]									DEFAULT (0),
		[TodayThreads]			int													NOT NULL CONSTRAINT [DF_bx_Forums_TodayThreadCount]								DEFAULT (0),
		[TodayPosts]			int													NOT NULL CONSTRAINT [DF_bx_Forums_TodayPostCount]								DEFAULT (0),
		[LastThreadID]			int													NOT NULL CONSTRAINT [DF_bx_Forums_LastPostID]									DEFAULT (0),
		[YestodayLastThreadID]	int													NOT NULL CONSTRAINT [DF_bx_Forums_YestodayLastThreadID]							DEFAULT (0),
		[YestodayLastPostID]	int													NOT NULL CONSTRAINT [DF_bx_Forums_YestodayLastPostID]							DEFAULT (0),
		[Password]				nvarchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL CONSTRAINT [DF_bx_Forums_Password]										DEFAULT (''),
		[SortOrder]				int													NOT NULL CONSTRAINT [DF_bx_Forums_SortOrder]									DEFAULT (0),
		
		[ExtendedAttributes]	ntext				COLLATE Chinese_PRC_CI_AS_WS	NULL,
		CONSTRAINT [PK_bx_Forms] PRIMARY KEY ([ForumID])
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Forums_CodeName] ON [bx_Forums] ([CodeName]);
CREATE INDEX [IX_bx_Forums_SortOrder] ON [bx_Forums]([ParentID], [SortOrder]);
GO

GO
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

GO
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

GO
EXEC bx_Drop 'bx_Identities_4000';

SELECT TOP 4000 I = IDENTITY(int,1,1) INTO [bx_Identities_4000]
FROM syscolumns a, syscolumns b
ALTER TABLE [bx_Identities_4000] ADD CONSTRAINT PK_bx_Identities_4000 PRIMARY KEY(I)

/*
Name: 序列表1-8000，此表可用作高效分割字符串
Columns:
    [I]      序列，存储1-8000的8000个顺序数字
*/

GO

GO
EXEC bx_Drop 'bx_Identities_8000';

SELECT TOP 8000 I = IDENTITY(int,1,1) INTO [bx_Identities_8000]
FROM syscolumns a, syscolumns b
ALTER TABLE [bx_Identities_8000] ADD CONSTRAINT PK_bx_Identities_8000 PRIMARY KEY(I)

/*
Name: 序列表1-8000，此表可用作高效分割字符串
Columns:
    [I]      序列，存储1-8000的8000个顺序数字
*/

GO

GO
EXEC bx_Drop 'bx_ImpressionRecords';

CREATE TABLE [bx_ImpressionRecords] (
	[RecordID]		    int      IDENTITY(1, 1)    NOT NULL
	
    ,[TypeID]	        int                        NOT NULL    CONSTRAINT [DF_bx_ImpressionRecords_TypeID]     DEFAULT(0)
    
    ,[UserID]           int                        NOT NULL    CONSTRAINT [DF_bx_ImpressionRecords_UserID]     DEFAULT(0)
    
    ,[TargetUserID]	    int                        NOT NULL    CONSTRAINT [DF_bx_ImpressionRecords_TargetUserID]     DEFAULT (0)
    
	,[CreateDate]       datetime                   NOT NULL    CONSTRAINT [DF_bx_ImpressionRecords_CreateDate]     DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_ImpressionRecords] PRIMARY KEY ([RecordID])
);


CREATE INDEX [IX_bx_ImpressionRecords_TargetUserID] ON [bx_ImpressionRecords]([TargetUserID]);
CREATE INDEX [IX_bx_ImpressionRecords_UserID_TargetUserID] ON [bx_ImpressionRecords]([UserID],[TargetUserID]);

GO
EXEC bx_Drop 'bx_Impressions';

CREATE TABLE [bx_Impressions] (
    [UserID]            int      NOT NULL
 
    ,[TypeID]	        int      NOT NULL
    
	,[Count]            int      NOT NULL    CONSTRAINT [DF_bx_Impressions_Count]     DEFAULT (1)
	
	,[UpdateDate]       datetime NOT NULL    CONSTRAINT [DF_bx_Impressions_UpdateDate]		DEFAULT(GETDATE())
	
    ,CONSTRAINT [PK_bx_Impressions] PRIMARY KEY ([UserID],[TypeID])
);
GO
EXEC bx_Drop bx_ImpressionTypeReverters;

GO

CREATE TABLE [bx_ImpressionTypeReverters](
	[TypeID]		    int				NOT NULL,
	[TextReverter]	    nvarchar(1000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_ImpressionTypeRevertersReverters] PRIMARY KEY([TypeID])
)

GO
GO
EXEC bx_Drop 'bx_ImpressionTypes';

CREATE TABLE [bx_ImpressionTypes] (
	 [TypeID]		    int                 IDENTITY (1, 1)                 NOT NULL

    ,[Text]		        nvarchar(100)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_ImpressionTypes_Text]				DEFAULT (N'')
    
    ,[RecordCount]      int                                                 NOT NULL    CONSTRAINT  [DF_bx_ImpressionTypes_RecordCount]			DEFAULT (1)
    
    ,[KeywordVersion]   varchar(32)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_ImpressionTypes_KeywordVersion]		DEFAULT ('')
    
    ,CONSTRAINT [PK_bx_ImpressionTypes] PRIMARY KEY ([TypeID])
);

GO
 EXEC bx_Drop 'bx_Instructs';

CREATE TABLE [bx_Instructs] (
		[InstructID]        bigint               IDENTITY(1, 1)               NOT NULL    CONSTRAINT  [PK_bx_Instructs]                    PRIMARY KEY ([InstructID])
		,[TargetID]         int                                               NOT NULL     
		,[ClientID]         int                                               NOT NULL    
		,[InstructType]     int						      NOT NULL 
		,[CreateDate]       datetime                                          NOT NULL    CONSTRAINT  [DF_bx_Instructs_CreateDate]         DEFAULT (GETDATE())
		,[Datas]            ntext             COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Instructs_Content]			   DEFAULT ('')
);  

CREATE INDEX [IX_bx_Instructs_ClientID]  ON [bx_Instructs]([ClientID]);
GO
EXEC bx_Drop 'bx_InviteSerials';

CREATE TABLE [bx_InviteSerials] (
   [ID]               int        IDENTITY(1, 1)                        NOT NULL                 
 
  ,[Serial]           uniqueidentifier                                 NOT NULL    CONSTRAINT [DF_bx_InviteSerials_Serial]          DEFAULT (NEWID())
  
  ,[ToEmail]          nvarchar(200)      COLLATE Chinese_PRC_CI_AS_WS       NULL
   
  ,[CreateDate]       datetime                                         NOT NULL    CONSTRAINT [DF_bx_InviteSerials_BeginDate]       DEFAULT (GETDATE())
  ,[ExpiresDate]      datetime                                         NOT NULL
    
  ,[UserID]           int                                              NOT NULL
  ,[ToUserID]         int                                              NOT NULL    CONSTRAINT [DF_bx_InviteSerials_ToUserID]        DEFAULT (0)
  
  ,[Remark]			  nvarchar(200)									   NOT NULL	   CONSTRAINT [DF_bx_InviteSerials_Remark]			DEFAULT('')
  
  ,[Status]           tinyint                                          NOT NULL    CONSTRAINT [DF_bx_InviteSerials_Status]          DEFAULT (0)
  
  ,CONSTRAINT [PK_bx_InviteSerials] PRIMARY KEY ([Serial])
  );

GO  

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_InviteSerials_ID] ON [bx_InviteSerials] ([ID]);
CREATE NONCLUSTERED INDEX [IX_bx_InviteSerials_UserID] ON [bx_InviteSerials] ([UserID]);
CREATE NONCLUSTERED INDEX [IX_bx_InviteSerials_Expires] ON [bx_InviteSerials] ([ExpiresDate]);


CREATE INDEX [IX_bx_InviteSerials_Serial] ON [bx_InviteSerials] ([Serial]);
GO

/*
Name:邀请码表
Columns:
	[ID]               自增长,用于分页
    [Serial]           邀请码(主键)
    
    [ToEmail]          发送到的Email       
    [CreateDate]       创建时间
    [ExpiresDate]      过期时间
    
    [UserID]           用户ID
    [ToUserID]         发送给的用户ID
    
    [Status]           状态
*/

GO
GO
EXEC bx_Drop 'bx_IPLogs';

CREATE TABLE bx_IPLogs(
     [LogID]                 int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_IPLogs]					PRIMARY KEY ([LogID])
    ,[UserID]             int                                               NOT NULL    CONSTRAINT [DF_bx_IPLogs_UserID]			DEFAULT(0)
  
    ,[Username]           nvarchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_IPLogs_Username]			DEFAULT(N'')
    
    ,[NewIP]              varchar(50)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_IPLogs_NewIP]			DEFAULT('')
    
    ,[CreateDate]         datetime                                          NOT NULL    CONSTRAINT [DF_bx_IPLogs_CreateDate]		DEFAULT(GETDATE())
    
    ,[OldIP]              varchar(50)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_IPLogs_OldIP]             DEFAULT('')
    
    ,[VisitUrl]           varchar(200)                                      NOT NULL    CONSTRAINT [DF_bx_IPLogs_VisitUrl]          DEFAULT('')
)

GO 

CREATE INDEX [IX_bx_IPlogs_NewIP] ON [bx_IPLogs]([NewIP]);

/*
Name:用户IP变更日志
Columns:
    [ID]            
    [UserID]          用户ID
    
    [Username]        用户名
    
    [IPAddress]       IP地址
    
    [CreateDate]      时间
*/

GO

GO
EXEC bx_Drop 'bx_JobStatus';

CREATE TABLE [bx_JobStatus] (	 
     [Type]             varchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_JobStatus_Type]            DEFAULT ('')

	,[LastExecuteTime]  datetime                                         NOT NULL    CONSTRAINT [DF_bx_JobStatus_LastExecuteTime]	DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_JobStatus] PRIMARY KEY ([Type])
);

/*
Name: JOB执行时间表
Columns:
    [Type]					任务类型

    [LastExecuteTime]       上次执行时间
*/

GO


GO
--EXEC bx_Drop 'bx_Medals';

--CREATE TABLE [bx_Medals] (
     --[ID]                int              IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_Medals]                 PRIMARY KEY ([ID])
 
    --,[Name]              nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    --,[IconUrl]           nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NULL
    
    --,[IsEnabled]         bit                                              NOT NULL    CONSTRAINT [DF_bx_Medals_IsEnabled]       DEFAULT (1)        
--);

--GO

--/*
--Name:勋章表
--Columns:
	--[Name]              勋章名称
	--[IconUrl]           图标
	--[IsEnbaled]         是否启用
--*/
GO
EXEC bx_Drop 'bx_Missions';

CREATE TABLE [bx_Missions] (
     [ID]               int               IDENTITY (1, 1)                   NOT NULL 
    ,[CycleTime]        int                                                 NOT NULL    CONSTRAINT [DF_bx_Missions_CycleTime]            DEFAULT (0)
    ,[SortOrder]        int                                                 NOT NULL    CONSTRAINT [DF_bx_Missions_SortOrder]            DEFAULT (0)
    ,[TotalUsers]       int                                                 NOT NULL    CONSTRAINT [DF_bx_Missions_TotalUsers]           DEFAULT (0)
    
    ,[IsEnable]         bit                                                 NOT NULL    CONSTRAINT [DF_bx_Missions_IsEnable]             DEFAULT (1)
    
    ,[Type]             nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_Type]                 DEFAULT ('')
    ,[Name]             nvarchar(100)     COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_Name]                 DEFAULT ('')
    ,[IconUrl]          nvarchar(200)     COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_IconUrl]              DEFAULT ('')
    ,[DeductPoint]      nvarchar(100)     COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_DeductPoint]          DEFAULT ('')
    
    ,[Prize]            ntext             COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_Prize]                DEFAULT ('')
    ,[Description]      ntext             COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_Description]          DEFAULT ('')
    ,[ApplyCondition]   ntext             COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_ApplyCondition]       DEFAULT ('')
    ,[FinishCondition]  ntext             COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Missions_FinishCondition]      DEFAULT ('')
    
    ,[EndDate]          datetime                                            NULL        CONSTRAINT [DF_bx_Missions_EndTime]              DEFAULT (NULL)
    ,[BeginDate]        datetime                                            NULL        CONSTRAINT [DF_bx_Missions_StartTime]            DEFAULT (NULL)
	,[CreateDate]       datetime                                            NOT NULL    CONSTRAINT [DF_bx_Missions_CreateDate]           DEFAULT (GETDATE())
	
	
	,[CategoryID]       int                                                 null        CONSTRAINT [DF_bx_Missions_CategoryID]              DEFAULT (NULL)
	,[ParentID]         int                                                 null        CONSTRAINT [DF_bx_Missions_ParentID]                DEFAULT (NULL)
	
    ,CONSTRAINT [PK_bx_Missions] PRIMARY KEY ([ID])
);

CREATE TABLE [bx_MissionCategories] (
     [ID]       int             identity(1,1)                   not null
    ,[Name]     nvarchar(20)    collate Chinese_PRC_CI_AS_WS    not null    default('')
    
    ,constraint [PK_bx_MissionCategories] primary key ([ID])
);

/*
Name: 任务表
Columns:
    [ID]               唯一标志
    [CycleTime]        周期 单位秒  0为不是周期任务
    [SortOrder]        排序 数字越大越靠前
    [TotalUsers]       申请人数
    
    [IsEnable]		   是否启用
    
    [Type]             任务对象类名 如maxLabs.bbsMax.TopicMission (帖子类,头像类等)
    [Name]             任务名称
    [Prize]            奖励 格式：值;值
    [IconUrl]          任务图标
    [DeductPoint]      用户申请任务后扣除积分(格式: pointID:值;pointID:值)
    [Description]      任务说明
    [ApplyCondition]   申请条件 格式：值;值
    [FinishCondition]  完成条件 格式：值;值
    
    [EndDate]          任务下线时间
    [BeginDate]        任务上线时间
    [CreateDate]       任务创建时间
 
*/

GO

GO
EXEC bx_Drop '[bx_Moderators]';

CREATE TABLE [bx_Moderators] (
	 [ForumID]				int NOT NULL 
	,[UserID]				int NOT NULL 
	,[BeginDate]			datetime NOT NULL				CONSTRAINT [DF_bx_Moderators_BeginDate]		DEFAULT('1753-1-1') 
	,[EndDate]				datetime NOT NULL				CONSTRAINT [DF_bx_Moderators_EndDate] DEFAULT ('9999-12-31 23:59:59')
	,[ModeratorType]		tinyint NOT NULL
	,[SortOrder]			int NOT NULL
	,[AppointorID]			int NULL
	CONSTRAINT [PK_bx_Moderators] PRIMARY KEY  CLUSTERED 		([ForumID],[UserID])
	
	)  

GO
--通知表
EXEC bx_Drop bx_Notify;

CREATE TABLE [bx_Notify] (
     [NotifyID]             int                    IDENTITY(1,1)                        NOT NULL   
	,[UserID]               int                                                         NOT NULL    CONSTRAINT [DF_bx_Notify_UserID]             DEFAULT(0)
	,[Content]				nvarchar(1000)         COLLATE Chinese_PRC_CI_AS_WS			NULL
	,[IsRead]               bit                                                         NOT NULL    CONSTRAINT [DF_bx_Notify_IsRead]             DEFAULT(0)
	,[TypeID]               int                                                         NOT NULL    CONSTRAINT [DF_bx_Notify_TypeID]             DEFAULT(0)
	,[Keyword]              varchar(200)												NULL
	,[NotifyDatas]          ntext														NULL
	,[CreateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_Notify_CreateDate]         DEFAULT(GETDATE())
	,[UpdateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_Notify_UpdateDate]         DEFAULT(GETDATE())
	,[ClientID]				int															NULL		CONSTRAINT [DF_bx_Notify_ClientID]           DEFAULT(0)
	,[Actions]              nvarchar(2000)			COLLATE Chinese_PRC_CI_AS_WS 		NULL
	,CONSTRAINT [PK_bx_Notify] PRIMARY KEY ([NotifyID])    
);

/*
Name:     消息表,包括用户消息,系统消息,通知提醒消息
Columns:
          [NotifyID]
          [UserID]                              始终表示这个通知的拥有者的ID
          [RelatedUserID]                       比如加好友,这里指的就是加我为好友的用户ID; 比如留言或回复,指的就是给我留言或回复的用户ID...
                   
          [IsRead]                              消息是否已读       
          
          [Type]                                类型, 1-留言及回复消息,2-群主邀请,3-好友验证消息,4-打招呼消息,5-应用邀请,6-应用通知    
          
          [Content]                             消息内容
          [SenderIP]                            发件人的IP 
          [Parameters]                          额外参数
          
          [PostDate]                            消息发送时间
*/

GO


--用户ID索引
CREATE INDEX [IX_bx_Notify_UserID] ON [bx_Notify]([UserID])

--通知类型索引
CREATE INDEX [IX_bx_Notify_Type] ON [bx_Notify]([TypeID])

--时间索引
CREATE INDEX [IX_bx_Notify_UpdateDate] ON [bx_Notify]([UpdateDate])

CREATE INDEX [IX_bx_Notify_Keyword] ON [bx_Notify]([Keyword])

GO
GO
exec bx_Drop 'bx_NotifyTypes';

CREATE TABLE bx_NotifyTypes(
 [TypeID]       int IDENTITY(1,1)    not null    
,[TypeName]     nvarchar(50)		 not null
,[Keep]         bit                  not null    CONSTRAINT [DF_bx_NotifyTypes_Keep] DEFAULT (1)
,[Description]  nvarchar(200)        null  
,CONSTRAINT [PK_bx_NotifyTypes] PRIMARY KEY ([TypeID])       
);
GO
EXEC bx_Drop 'bx_OperationLogs';

CREATE TABLE bx_OperationLogs(
     [LogID]            int               IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_OperationLogs]               PRIMARY KEY ([LogID])
    ,[OperatorID]       int                                               NOT NULL
    ,[TargetID_1]       int                                               NOT NULL    CONSTRAINT [DF_bx_OperationLogs_TargetID_1]    DEFAULT (0)
    ,[TargetID_2]       int                                               NOT NULL    CONSTRAINT [DF_bx_OperationLogs_TargetID_2]    DEFAULT (0)
    ,[TargetID_3]       int                                               NOT NULL    CONSTRAINT [DF_bx_OperationLogs_TargetID_3]    DEFAULT (0)
    
    ,[OperatorIP]       varchar(50)       COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    ,[OperationType]    varchar(100)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    ,[Message]          nvarchar(1000)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    
    ,[CreateTime]       datetime                                          NOT NULL
)

GO

EXEC bx_Drop 'IX_bx_OperationLogs_OperatorID';
CREATE  INDEX [IX_bx_OperationLogs_OperatorID] ON [bx_OperationLogs]([OperatorID]);

EXEC bx_Drop 'IX_bx_OperationLogs_OperatorIP';
CREATE  INDEX [IX_bx_OperationLogs_OperatorIP] ON [bx_OperationLogs]([OperatorIP]);

EXEC bx_Drop 'IX_bx_OperationLogs_OperationType';
CREATE  INDEX [IX_bx_OperationLogs_OperationType] ON [bx_OperationLogs]([OperationType]);

EXEC bx_Drop 'IX_bx_OperationLogs_CreateTime';
CREATE  INDEX [IX_bx_OperationLogs_CreateTime] ON [bx_OperationLogs]([CreateTime]);

GO
GO
EXEC bx_Drop 'bx_PassportClients';

CREATE TABLE bx_PassportClients (
 
	 [ClientID]          int		    IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_PassportClients]                 PRIMARY KEY ([ClientID])
	,[ClientName]        nvarchar(50)                                   NOT NULL
	,[AccessKey]         nvarchar(50)									NULL
	,[Url]               nvarchar(1000)                                 NOT NULL
	,[APIFilePath]       nvarchar(200)                                  NOT NULL
	,[CreateDate]        datetime                                       NOT NULL    CONSTRAINT  [DF_bx_PassportClients_CreateDate]		DEFAULT(GETDATE())
	,[Deleted]           bit											NOT NULL	CONSTRAINT  [DF_bx_PassportClients_Deleted]			DEFAULT(0)
	,[InstructTypes]     text			COLLATE Chinese_PRC_CI_AS_WS		NULL
)
GO
EXEC bx_Drop 'bx_Pay';

CREATE TABLE [bx_Pay] (
	 [PayID]		   int IDENTITY (1, 1)		NOT NULL    CONSTRAINT [PK_bx_Pay]   PRIMARY KEY ([PayID])
    ,[UserID]          int						NOT NULL
    ,[BuyerEmail]      nvarchar(50)
    ,[OrderNo]         varchar(50)		    COLLATE Chinese_PRC_CI_AS_WS  NOT NULL
    ,[TransactionNo]   nvarchar(200)			COLLATE Chinese_PRC_CI_AS_WS 
	,[OrderAmount]	   decimal(18, 2)			NOT NULL	
	,[Payment]         tinyint                  NOT NULL	          
	,[PayType]	       tinyint					NOT NULL
	,[PayValue]	       int						NOT NULL
	,[CreateDate]	   datetime				    NOT NULL	CONSTRAINT [DF_bx_Pay_CreateDate]   DEFAULT (GETDATE())
	,[PayDate]		   datetime
	,[SubmitIp]        varchar(50)              COLLATE Chinese_PRC_CI_AS_WS   NOT NULL    
	,[PayIp]           varchar(50)              COLLATE Chinese_PRC_CI_AS_WS 
	,[Remarks]		   nvarchar(50)             COLLATE Chinese_PRC_CI_AS_WS
	,[State]           bit                      CONSTRAINT [DF_bx_Pay_State]  DEFAULT ((0))
) 
GO


CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Pay_OrderNo] ON [bx_Pay] ([OrderNo]);
CREATE INDEX [IX_bx_Pay_TransactionNo] ON [bx_Pay] ([TransactionNo]);
CREATE INDEX [IX_bx_Pay_UserID] ON [bx_Pay] ([UserID],[State]);
GO

GO
EXEC bx_Drop bx_PhotoReverters;

GO

CREATE TABLE bx_PhotoReverters(
	[PhotoID]				int				NOT NULL,
	[NameReverter]			nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,
	[DescriptionReverter]	ntext			COLLATE Chinese_PRC_CI_AS_WS    NOT NULL,

	CONSTRAINT [PK_bx_PhotoReverters] PRIMARY KEY([PhotoID])
)

/*
Name:标签
Columns:
    [PhotoID]			      可恢复的博客文章ID
	[NameReverter]         标题复原关键信息，可根据此信息恢复标题的原始内容
	[ContentReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO



GO
EXEC bx_Drop bx_Photos;


GO

CREATE TABLE [bx_Photos] (
    [PhotoID]                int                 IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_Photos]                         PRIMARY KEY ([PhotoID])
   ,[UserID]                 int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_UserID]                 DEFAULT(0)
   ,[AlbumID]                int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_AlbumID]                DEFAULT(0)
   ,[TotalViews]             int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_TotalViews]             DEFAULT(0)
   ,[TotalComments]          int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_TotalComments]          DEFAULT(0)
   
   ,[FileID]                 varchar(50)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_FileID]                 DEFAULT ('')
   ,[FileSize]               bigint                                              NOT NULL    CONSTRAINT  [DF_bx_Photos_FileSize]               DEFAULT (0)
   ,[FileType]               varchar(10)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_FileType]               DEFAULT ('')
   ,[Width]					int													 NOT NULL    CONSTRAINT  [DF_bx_Photos_Width]				   DEFAULT(0)
   ,[Height]				int													 NOT NULL    CONSTRAINT  [DF_bx_Photos_Height]				   DEFAULT(0)
   ,[Exif]                   nvarchar(1500)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_Exif]	               DEFAULT ('')
   ,[Name]                   nvarchar(50)        COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_Name]                   DEFAULT ('')
   ,[Description]            nvarchar(1500)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_Description]            DEFAULT ('')
   
   ,[ThumbPath]              varchar(256)        COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_ThumbPath]              DEFAULT ('')
   ,[ThumbWidth]             int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_ThumbWidth]             DEFAULT(0)
   ,[ThumbHeight]            int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_ThumbHeight]            DEFAULT(0)
   
   ,[CreateIP]               varchar(50)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_CreateIP]               DEFAULT ('')
   
   ,[CreateDate]             datetime                                            NOT NULL    CONSTRAINT  [DF_bx_Photos_CreateDate]             DEFAULT (GETDATE())
   ,[UpdateDate]             datetime                                            NOT NULL    CONSTRAINT  [DF_bx_Photos_UpdateDate]             DEFAULT (GETDATE())
   ,[LastCommentDate]        datetime                                            NOT NULL    CONSTRAINT  [DF_bx_Photos_LastCommentDate]        DEFAULT ('1753-1-1')
   
   ,[LastEditUserID]         int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_LastEditUserID]         DEFAULT(0)
   
   ,[KeywordVersion]         varchar(32)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_KeywordVersion]         DEFAULT ('')
)

/*
Name:相片
Columns:
    [PhotoID]          自动标识
    [AlbumID]          相册ID
    [TotalViews]       总浏览数
    [TotalComments]    总相片数
    
    [FileID]           文件ID:由文件表提供
    
    [Exif]             相片扩展信息
    [Name]             相片名称
    [Description]      相片描述
    [NameReverter]     相片名称关键字还原信息
    [DescnReverter]    相片描述关键字还原信息
    
    [CreateIP]         上传IP
    
    [CreateDate]       上传时间
    [UpdateDate]       最后更新时间
    [LastCommentDate]  最后评论时间
*/

GO

--相片表的用户ID索引
CREATE INDEX [IX_bx_Photos_UserID] ON [bx_Photos]([UserID])

--相片表的相册ID索引
CREATE INDEX [IX_bx_Photos_AlbumID] ON [bx_Photos]([AlbumID])


CREATE INDEX [IX_bx_Photos_FileID] ON [bx_Photos]([FileID])
--相片表的创建时间索引
--CREATE INDEX [IX_bx_Photos_CreateDate] ON [bx_Photos]([CreateDate])

GO

CREATE TABLE [bx_PointLogs](
	[LogID] bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_bx_PointLogs    PRIMARY KEY (LogID),
	[UserID] int NOT NULL,
	[OperateID] int NOT NULL,
	[Remarks] nvarchar(200) NULL,
	[Point0] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P0]              DEFAULT (0),
	[Point1] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P1]              DEFAULT (0),
	[Point2] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P2]              DEFAULT (0),
	[Point3] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P3]              DEFAULT (0),
	[Point4] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P4]              DEFAULT (0),
	[Point5] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P5]              DEFAULT (0),
	[Point6] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P6]              DEFAULT (0),
	[Point7] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_P7]              DEFAULT (0),
	[Current0] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP0]              DEFAULT (0),
	[Current1] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP1]              DEFAULT (0),
	[Current2] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP2]              DEFAULT (0),
	[Current3] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP3]              DEFAULT (0),
	[Current4] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP4]              DEFAULT (0),
	[Current5] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP5]              DEFAULT (0),
	[Current6] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP6]              DEFAULT (0),
	[Current7] int NOT NULL   CONSTRAINT [DF_bx_PointLogs_CP7]              DEFAULT (0),
	[CreateTime] datetime NOT NULL    CONSTRAINT [DF_bx_PointLogs_CreateTime]              DEFAULT (GETDATE())
);
GO
GO
CREATE TABLE [bx_PointLogTypes](
	[OperateID]		int IDENTITY(1,1)	NOT NULL  CONSTRAINT PK_bx_PointLogTypes    PRIMARY KEY (OperateID),
	[OperateName]	nvarchar(50)		NOT NULL
);
GO

GO
EXEC bx_Drop 'bx_PointShows';

CREATE TABLE bx_PointShows (
   [UserID]        int              NOT NULL
   ,[ShowPoints]    int              NOT NULL
   ,[Price]         int              NOT NULL
   ,[Content]       nvarchar(100)  COLLATE Chinese_PRC_CI_AS_WS   NULL
   
   ,[CreateDate]    datetime         NOT NULL    CONSTRAINT  [DF_bx_PointShows_CreateDate]    DEFAULT (GETDATE())
)


CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_PointShows_UserID] ON [bx_PointShows] ([UserID]);
CREATE INDEX [IX_bx_PointShows_ShowPoints] ON [bx_PointShows] ([ShowPoints]);

/*
Name:上榜
Columns:
	[Points]            积分
    [UserID]           用户ID
    
    [Content]          内容
    
    [CreateDate]       时间
*/

GO

GO
EXEC bx_Drop 'bx_Polemizes';

CREATE TABLE [bx_Polemizes](
	[ThreadID] [int] NOT NULL,
	[AgreeViewPoint] NTEXT COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
	[AgreeCount] [int] NOT NULL  CONSTRAINT [DF_bx_Polemizes_AgreeCount]  DEFAULT ((0)),
	[AgainstViewPoint] NTEXT COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
	[AgainstCount] [int] NOT NULL CONSTRAINT [DF_bx_Polemizes_AgainstCount]  DEFAULT ((0)),
	[NeutralCount] [int] NOT NULL CONSTRAINT [DF_bx_Polemizes_NeutralCount]  DEFAULT ((0)),
	[ExpiresDate] [datetime] NOT NULL CONSTRAINT [DF_bx_Polemizes_ExpiresDate]  DEFAULT (getdate()),
CONSTRAINT [PK_bx_Polemizes] PRIMARY KEY CLUSTERED  ([ThreadID] ASC)
) 

GO

GO
EXEC bx_Drop 'bx_PolemizeUsers';

CREATE TABLE [bx_PolemizeUsers](
	[ThreadID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[ViewPointType] [tinyint] NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_PolemizeUsers_CreateDate]  DEFAULT (getdate()),
	CONSTRAINT [PK_bx_PolemizeUsers] PRIMARY KEY CLUSTERED  ([ThreadID] ASC,[UserID] ASC)
) 

GO

GO
EXEC bx_Drop 'bx_PollItemDetails';

CREATE TABLE [bx_PollItemDetails]
(
[ItemID] [int] NOT NULL,
[UserID] [int] NOT NULL,
[NickName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_PollItemDetails_NickName] DEFAULT (''),
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_PollItemDetails_CreateDate] DEFAULT (getdate()),
CONSTRAINT [PK_bx_PollItemDetails] PRIMARY KEY CLUSTERED  ([ItemID], [UserID])
) 

GO

GO
EXEC bx_Drop 'bx_PollItems';

CREATE TABLE [bx_PollItems]
(
[ItemID] [int] NOT NULL IDENTITY(1, 1),
[ThreadID] [int] NOT NULL,
[ItemName] [nvarchar] (200) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[PollItemCount] [int] NOT NULL CONSTRAINT [DF_bx_PollItems_PollItemCount] DEFAULT ((0)),
CONSTRAINT [PK_bx_PollItems] PRIMARY KEY CLUSTERED  ([ItemID])
) 

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_PollItems_List] ON [bx_PollItems] ([ThreadID], [ItemID]);
GO

GO
EXEC bx_Drop 'bx_Polls';

CREATE TABLE [bx_Polls]
(
[ThreadID] [int] NOT NULL,
[Multiple] [int] NOT NULL CONSTRAINT [DF_bx_Polls_IsMultiple] DEFAULT ((1)),
[AlwaysEyeable] [bit] NOT NULL CONSTRAINT [DF_bx_Polls_AlwaysEyeable] DEFAULT ((0)),
[ExpiresDate] [datetime] NOT NULL,
CONSTRAINT [PK_bx_Polls] PRIMARY KEY CLUSTERED  ([ThreadID] ASC)
) 

GO

CREATE INDEX [IX_bx_Polls_ExpiresDate] ON [bx_Polls] ([ExpiresDate]);

GO

GO
EXEC bx_Drop bx_PostLoveHates;

CREATE TABLE [bx_PostLoveHates] (
     [PostID]              int															  NOT NULL    CONSTRAINT  [DF_bx_PostLoveHates_PostID]       DEFAULT (0)
    ,[UserID]              int															  NOT NULL    CONSTRAINT  [DF_bx_PostLoveHates_UserID]       DEFAULT (0)
    ,[LoveCount]           int															  NOT NULL    CONSTRAINT  [DF_bx_PostLoveHates_LoveCount]    DEFAULT (0)
    ,[HateCount]           int															  NOT NULL    CONSTRAINT  [DF_bx_PostLoveHates_HateCount]    DEFAULT (0)
    
    ,CONSTRAINT [PK_bx_PostLoveHates] PRIMARY KEY ([PostID],[UserID])
);

/*
Name:    真实文件表
Columns:
        [PostID]                    帖子ID
        [UserID]                    用户ID
        
        [LoveCount]                 支持次数
        [HateCount]                 反对次数
*/

GO
GO
CREATE TABLE [bx_PostMarks]
(
[PostMarkID] [int] NOT NULL IDENTITY(1, 1),
[PostID] [int] NOT NULL,
[UserID] [int] NOT NULL,
[Username] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_PostMarks_CreateDate] DEFAULT (getdate()),
[ExtendedPoints_1] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_1] DEFAULT ((0)),
[ExtendedPoints_2] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_2] DEFAULT ((0)),
[ExtendedPoints_3] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_3] DEFAULT ((0)),
[ExtendedPoints_4] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_4] DEFAULT ((0)),
[ExtendedPoints_5] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_5] DEFAULT ((0)),
[ExtendedPoints_6] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_6] DEFAULT ((0)),
[ExtendedPoints_7] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_7] DEFAULT ((0)),
[ExtendedPoints_8] [int] NOT NULL CONSTRAINT [DF_bx_PostMarks_ExtendedPoints_8] DEFAULT ((0)),
[Reason] [ntext] COLLATE Chinese_PRC_CI_AS_WS NULL,
CONSTRAINT [PK_bx_PostMarks] PRIMARY KEY ([PostMarkID])
)

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_PostMarks] ON [bx_PostMarks] ([PostID],[UserID]);
CREATE INDEX [IX_bx_PostMarks_CreateDate] ON [bx_PostMarks] ([CreateDate]);
CREATE INDEX [IX_bx_PostMarks_UserID] ON [bx_PostMarks] ([UserID]);

GO
GO
EXEC bx_Drop bx_PostReverters;

GO

CREATE TABLE [bx_PostReverters](
	[PostID]				int				NOT NULL,
	[SubjectReverter]		nvarchar(4000)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,
	[ContentReverter]		ntext			COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_PostReverters] PRIMARY KEY([PostID])
)

/*
Name:标签
Columns:
    [ArticleID]	       可恢复的博客文章ID
	[SubjectReverter]         标题复原关键信息，可根据此信息恢复标题的原始内容
	[ContentReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO



GO
EXEC bx_Drop 'bx_Posts';

CREATE TABLE [bx_Posts]
(
[PostID] [int] NOT NULL IDENTITY(1, 1),
[ParentID] [int] NULL,
[ForumID] [int] NOT NULL,
[ThreadID] [int] NOT NULL,
[PostType] [tinyint] NOT NULL CONSTRAINT [DF_bx_Posts_PostType] DEFAULT ((0)),
[IconID] [int] NOT NULL CONSTRAINT [DF_bx_Posts_IconID] DEFAULT ((0)),
[Subject] [nvarchar] (256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[Content] [ntext] COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[ContentFormat] [tinyint] NOT NULL CONSTRAINT [DF_bx_Posts_ContentFormat] DEFAULT ((10)),
[EnableSignature] [bit] NOT NULL CONSTRAINT [DF_bx_Posts_EnableSignature] DEFAULT ((1)),
[EnableReplyNotice] [bit] NOT NULL CONSTRAINT [DF_bx_Posts_EnableReplyNotice] DEFAULT ((0)),
[IsShielded] [bit] NOT NULL CONSTRAINT [DF_bx_Posts_IsShielded] DEFAULT ((0)),
[UserID] [int] NOT NULL CONSTRAINT [DF_bx_Posts_UserID] DEFAULT ((0)),
[NickName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[LastEditorID] [int]  NOT NULL CONSTRAINT [DF_bx_Posts_LastEditorID] DEFAULT ((0)),
[LastEditor] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Posts_LastEditor] DEFAULT (N''),
[IPAddress] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Posts_IPAddress] DEFAULT ('000.000.000.000'),
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_Posts_CreateDate] DEFAULT (getdate()),
[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_Posts_UpdateDate] DEFAULT (getdate()),
[MarkCount] [int] NULL,
[LoveCount] [int] NULL,
[HateCount] [int] NULL,
[SortOrder] [bigint] NOT NULL,
[FloorNumber] [int] NULL, 
[KeywordVersion]   varchar(32)       COLLATE Chinese_PRC_CI_AS_WS    NULL,
[HistoryAttachmentIDs]   varchar(500)       COLLATE Chinese_PRC_CI_AS_WS    NULL,
CONSTRAINT [PK_bx_Posts] PRIMARY KEY ([PostID])
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Posts_Forum] ON [bx_Posts] ([ForumID] ASC, [SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Posts_SortOrder] ON [bx_Posts] ([SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Posts_User] ON [bx_Posts] ([UserID] ASC,[ThreadID] DESC,[SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Posts_Thread] ON [bx_Posts] ([ThreadID] DESC, [SortOrder] DESC);

CREATE INDEX [IX_bx_Posts_CreateDate] ON [bx_Posts] ([CreateDate] DESC);
GO
GO
CREATE TABLE bx_PropLogs (
   PropLogID  int		IDENTITY(1,1)					NOT NULL CONSTRAINT [PK_bx_PropLogs]            PRIMARY KEY (PropLogID)
  ,UserID     int										NOT NULL CONSTRAINT [DF_bx_PropLogs_UserID]		DEFAULT(0)
  ,Type       tinyint									NOT NULL CONSTRAINT [DF_bx_PropLogs_Type]		DEFAULT(0)
  ,[Log]      ntext		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL CONSTRAINT [DF_bx_PropLogs_Log]		DEFAULT('')
  ,CreateDate datetime									NOT NULL CONSTRAINT [DF_bx_PropLogs_CreateDate]	DEFAULT(GETDATE())
);

CREATE INDEX [IX_bx_PropLogs_UserID_Type] ON bx_PropLogs (UserID, Type);
GO
EXEC bx_Drop 'bx_Props';

CREATE TABLE [bx_Props] (
   [PropID]            int           IDENTITY(1,1)                NOT NULL
  ,[Icon]              nvarchar(255) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Props_Icon]                DEFAULT('')
  ,[Name]              nvarchar(100) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Props_Name]                DEFAULT('')
  ,[Price]             int                                        NOT NULL CONSTRAINT [DF_bx_Props_Price]               DEFAULT(0)
  ,[PriceType]         int                                        NOT NULL CONSTRAINT [DF_bx_Props_PriceType]           DEFAULT('')
  ,[PropType]          nvarchar(512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Props_PropType]            DEFAULT('')
  ,[PropParam]         ntext         COLLATE Chinese_PRC_CI_AS_WS NOT NULl CONSTRAINT [DF_bx_Props_PropParam]           DEFAULT('') 
  ,[Description]       nvarchar(255) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Props_Description]         DEFAULT(0)
  ,[PackageSize]       int                                        NOT NULL CONSTRAINT [DF_bx_Props_PackageSize]         DEFAULT(0)
  ,[TotalNumber]       int                                        NOT NULL CONSTRAINT [DF_bx_Props_TotalNumber]         DEFAULT(0)
  ,[SaledNumber]       int                                        NOT NULL CONSTRAINT [DF_bx_Props_SaledNumber]			DEFAULT(0)
  ,[AllowExchange]     bit                                        NOT NULL CONSTRAINT [DF_bx_Props_AllowExchange]       DEFAULT(0)
  ,[AutoReplenish]     bit                                        NOT NULl CONSTRAINT [DF_bx_Props_AutoReplenish]       DEFAULT(0)
  ,[ReplenishNumber]   int                                        NOT NULL CONSTRAINT [DF_bx_Props_ReplenishNumber]     DEFAULT(0)
  ,[ReplenishTimeSpan] int                                        NOT NULL CONSTRAINT [DF_bx_Props_ReplenishTimeSpan]   DEFAULT(0)
  ,[LastReplenishTime] datetime                                   NOT NULL CONSTRAINT [DF_bx_Props_LastReplenishTime]	DEFAULT(GETDATE())
  ,[BuyCondition]      ntext        COLLATE Chinese_PRC_CI_AS_WS  NOT NULL CONSTRAINT [DF_bx_Props_BuyCondition]        DEFAULT('')
  ,[Enable]            bit                                        NOT NULL CONSTRAINT [DF_bx_Props_Enable]				DEFAULT(1)
  ,[ReplenishLimit]    int                                        NOT NULL CONSTRAINT [DF_bx_Props_ReplenishLimit]		DEFAULT(0)
  ,[SortOrder]         int                                        NOT NULL CONSTRAINT [DF_bx_Props_SortOrder]			DEFAULT(0)
  
  ,CONSTRAINT [PK_bx_Props] PRIMARY KEY ([PropID])
);
GO
EXEC bx_Drop 'bx_QuestionRewards';

CREATE TABLE [bx_QuestionRewards]
(
[ThreadID] [int] NOT NULL,
[PostID] [int] NOT NULL,
[Reward] [int] NOT NULL,
CONSTRAINT [PK_bx_QuestionRewards] PRIMARY KEY CLUSTERED  ([ThreadID], [PostID])
) 

GO

CREATE INDEX [IX_bx_QuestionRewards_List] ON [bx_QuestionRewards] ([ThreadID], [Reward]);

GO

GO
EXEC bx_Drop 'bx_Questions';

CREATE TABLE [bx_Questions]
(
[ThreadID] [int] NOT NULL,
[IsClosed] [bit] NOT NULL CONSTRAINT [DF_bx_Questions_IsClosed] DEFAULT ((0)),
[Reward] [int] NOT NULL CONSTRAINT [DF_bx_Questions_Reward] DEFAULT ((0)),
[RewardCount] [int] NOT NULL CONSTRAINT [DF_bx_Questions_RewardCount] DEFAULT ((0)),
[AlwaysEyeable] [bit] NOT NULL CONSTRAINT [DF_bx_Questions_AlwaysEyeable] DEFAULT ((0)),
[ExpiresDate] [datetime] NOT NULL,
[BestPostID] [int] NOT NULL CONSTRAINT [DF_bx_Questions_BestPostID] DEFAULT ((0)),
[UsefulCount] [int] NOT NULL CONSTRAINT [DF_bx_Questions_UsefulCount] DEFAULT ((0)),
[UnusefulCount] [int] NOT NULL CONSTRAINT [DF_bx_Questions_UnusefulCount] DEFAULT ((0)),
CONSTRAINT [PK_bx_Questions] PRIMARY KEY CLUSTERED  ([ThreadID])
) 

GO

CREATE INDEX [IX_bx_Questions_ExpiresDate] ON [bx_Questions]([IsClosed], [ExpiresDate]);

GO

GO
EXEC bx_Drop 'bx_QuestionUsers';

CREATE TABLE [bx_QuestionUsers](
	[ThreadID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[BestPostIsUseful] [bit] NOT NULL CONSTRAINT [DF_bx_QuestionUsers_BestPostIsUseful]  DEFAULT ((0))
) 

GO


GO
CREATE TABLE [dbo].[bx_RecoverPasswordLogs](
	[Id]			int  IDENTITY(1,1) NOT NULL,
	[UserID]		int NOT NULL,
	[CreateDate]	datetime NULL		CONSTRAINT [DF_bx_RecoverPasswordLogs_CreateDate]	DEFAULT (getdate()),
	[Successed]		bit NULL			CONSTRAINT [DF_bx_RecoverPasswordLogs_Successed]	DEFAULT (0),
	[Email]			nvarchar(200) NULL,
	[Serial]		varchar(100) NULL,
	[IP]			varchar(150) NOT NULL,
	CONSTRAINT [PK_bx_RecoverPasswordLogs] PRIMARY KEY ([Id] ASC)
);
GO

GO
EXEC bx_Drop 'bx_Roles';

CREATE	TABLE bx_Roles(
	[RoleID]			int				NOT NULL	Identity(100,1)	CONSTRAINT		[PK_bx_RoleGroup_RoleID]			Primary KEY([RoleID]),
	
	[Name]				nvarchar(50)	NOT NULL					CONSTRAINT		[DF_bx_RoleGroup_Name]				Default(N''),
	
	[Title]				nvarchar(50)	NOT NULL					CONSTRAINT		[DF_bx_RoleGroup_Title]				Default(N''),	
	
	[Color]				varchar(10)		NOT NULL					CONSTRAINT		[DF_bx_RoleGroup_Color]				Default('#000000'),	
		
	[IconUrl]			varchar(100)	NOT NULL					CONSTRAINT		[DF_bx_RoleGroup_IconUrl]			Default(''),
		
	[RoleType]			tinyint			NOT NULL					CONSTRAINT		[DF_bx_RoleGroup_RoleType]			Default(0),
	
	[Level]				int				NOT NULL					CONSTRAINT		[DF_bx_RoleGroup_Level]				Default(0),
	
	[StarLevel]			int				NOT NULL					CONSTRAINT		[DF_bx_RoleGroup_StarLevel]			Default(0),
	
	[UserCount]			int				NOT NULL					CONSTRAINT		[DF_bx_RoleGroup_UserCount]			Default(0),
	
	[RequiredPoint]		int				NOT NULL					CONSTRAINT		[DF_bx_RoleGroup_RequiredPoint]		Default(0),
	
	[CreateTime]		datetime		NOT NULL					CONSTRAINT		[DF_bx_RoleGroup_CreateTime]		Default(getdate())

)

GO

CREATE INDEX [IX_bx_Roles] ON [bx_Roles]	([RoleID] ASC);

GO
GO
EXEC bx_Drop 'bx_SearchPostResults';

CREATE TABLE [bx_SearchPostResults] (
	 [ID]                 uniqueidentifier										  NOT NULL
	,[UserID]             int													  NOT NULL
	,[IP]                 varchar(50)          COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_SearchPostResults_IP]             DEFAULT ('')
    ,[Keyword]			  nvarchar(200)        COLLATE Chinese_PRC_CI_AS_WS       NOT NULL
    ,[SearchMode]         tinyint												  NOT NULL
    ,[IsDesc]             bit													  NOT NULL
    ,[ThreadIDs]	      text                 COLLATE Chinese_PRC_CI_AS_WS       NOT NULL
    ,[PostIDs]			  text                 COLLATE Chinese_PRC_CI_AS_WS       NOT NULL
    ,[ForumIDs]           text                 COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_SearchPostResults_ForumIDs]             DEFAULT ('')
    ,[TargetUserID]       int													  NOT NULL    CONSTRAINT [DF_bx_SearchPostResults_TargetUserID]         DEFAULT (0)
    ,[IsBefore]           bit												      NOT NULL    CONSTRAINT [DF_bx_SearchPostResults_IsBefore]             DEFAULT (0)
    ,[PostDate]           datetime												  NULL
    ,[UpdateDate]         datetime												  NOT NULL    CONSTRAINT [DF_bx_SearchPostResults_UpdateDate]           DEFAULT (GETDATE())
    ,[CreateDate]         datetime											      NOT NULL    CONSTRAINT [DF_bx_SearchPostResults_CreateDate]           DEFAULT (GETDATE())
    ,CONSTRAINT [PK_bx_SearchPostResults] PRIMARY KEY ([ID])
);


GO

EXEC bx_Drop 'IX_bx_SearchPostResults_UserID';
CREATE  INDEX [IX_bx_SearchPostResults_UserID] ON [bx_SearchPostResults]([UserID])

EXEC bx_Drop 'IX_bx_SearchPostResults_IP';
CREATE  INDEX [IX_bx_SearchPostResults_IP] ON [bx_SearchPostResults]([IP])

EXEC bx_Drop 'IX_bx_SearchPostResults_CreateDate';
CREATE  INDEX [IX_bx_SearchPostResults_CreateDate] ON [bx_SearchPostResults]([CreateDate])

EXEC bx_Drop 'IX_bx_SearchPostResults_UpdateDate';
CREATE  INDEX [IX_bx_SearchPostResults_UpdateDate] ON [bx_SearchPostResults]([UpdateDate])
GO
EXEC bx_Drop '[bx_Serials]';

CREATE TABLE [bx_Serials] (
   [Serial]           uniqueidentifier                                 NOT NULL    CONSTRAINT [DF_bx_bx_Serials_Serial]          DEFAULT (NEWID())
  
  ,[UserID]           int                                              NOT NULL
  
  ,[CreateDate]       datetime                                         NOT NULL    CONSTRAINT [DF_bx_bx_Serials_CreateDate]      DEFAULT (GETDATE())
  ,[ExpiresDate]      datetime                                         NOT NULL    
  ,[Type]			  tinyint										   NOT NULL    CONSTRAINT [DF_bx_bx_Serials_Type]			 DEFAULT (0)
  ,[Data]             nvarchar(1000)                                   NULL 
  ,CONSTRAINT [PK_bx_bx_Serials] PRIMARY KEY ([Serial])
  );
  
  CREATE INDEX [IX_bx_Serials_UserType] ON [bx_Serials] ([UserID],[Type]);
GO
EXEC bx_Drop 'bx_Settings';

CREATE TABLE [bx_Settings] (
     [NodeID]      int                                              NULL
     
    ,[Key]         nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Settings_Key]      DEFAULT ('*')
    ,[TypeName]    nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    
    ,[Value]       ntext            COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
);

/*
Name: 系统设置表
Columns:
    [NodeID]      目标节点ID，用在版块等，允许为空

    [Key]         设置项Key，如果是序列化保存，则Key都是*，如果是非序列化保存，则此项是属性名
    [TypeName]    设置项类型名，使用FullName

    [Value]       设置值，如果是序列化保存，保存的值是整个设置对象序列化所得的字符串，如果是非序列化保存，则保存的是对应Key的属性ToString得到的值
*/

GO


GO
EXEC bx_Drop bx_ShareReverters;

GO

CREATE TABLE bx_ShareReverters(
	[ShareID]				int				NOT NULL,
	[ContentReverter]		    nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL

	CONSTRAINT [PK_bx_ShareReverters] PRIMARY KEY([ShareID])
)

CREATE TABLE bx_UserShareReverters(
	[UserShareID]			int				NOT NULL,
	[SubjectReverter]		nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,
	[DescriptionReverter]	ntext                                           NOT NULL

	CONSTRAINT [PK_bx_UserShareReverters] PRIMARY KEY([UserShareID])
)

/*
Name:标签
Columns:
    [ShareID]	                  可恢复的分享ID
	[DescriptionReverter]         内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO



GO
EXEC bx_Drop 'bx_Shares';

CREATE TABLE [bx_Shares] (
     [ShareID]                  int				   IDENTITY (1, 1)                  NOT NULL
    ,[UserID]                   int                                                 NOT NULL    CONSTRAINT [DF_bx_Shares_UserID]               DEFAULT (0)
    ,[TargetID]                 int                                                 NOT NULL    CONSTRAINT [DF_bx_Shares_TargetID]             DEFAULT (0)
    ,[TotalComments]            int                                                 NOT NULL    CONSTRAINT [DF_bx_Shares_TotalComments]        DEFAULT (0)
    
    ,[Type]                     tinyint                                             NOT NULL    CONSTRAINT [DF_bx_Shares_Type]                 DEFAULT (0)
    
    ,[Url]						nvarchar(512)										NOT NULL	CONSTRAINT [DF_bx_Shares_Url]					DEFAULT ('')
    ,[Content]                  nvarchar(2800)     COLLATE Chinese_PRC_CI_AS_WS     NOT NULL    CONSTRAINT [DF_bx_Shares_Content]              DEFAULT ('')

	,[CreateDate]               datetime                                            NOT NULL    CONSTRAINT [DF_bx_Shares_CreateDate]           DEFAULT (GETDATE())
	,[LastCommentDate]          datetime                                            NOT NULL    CONSTRAINT [DF_bx_Shares_LastCommentDate]      DEFAULT ('1753-1-1')
	
    ,[KeywordVersion]           varchar(32)       COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_Shares_KeywordVersion]       DEFAULT ('')
    
    ,[AgreeCount]               int                                                 NOT NULL    CONSTRAINT [DF_bx_Shares_AgreeCount]           DEFAULT (0)
    ,[OpposeCount]              int                                                 NOT NULL    CONSTRAINT [DF_bx_Shares_OpposeCount]          DEFAULT (0)
    ,[ShareCount]				int													NOT NULL	constraint [DF_bx_Shares_ShareCount]			default(0)
    ,[AgreeAndOpposeCount]		int													NOT NULL    CONSTRAINT [DF_bx_Shares_AgreeAndOpposeCount]  DEFAULT (0)
    ,[SortOrder]                bigint                                              not null    constraint [DF_bx_Shares_SortOrder]             default(0)
    
    ,CONSTRAINT [PK_bx_Shares] PRIMARY KEY ([ShareID])
);

CREATE TABLE [bx_UserShares] (
	[UserShareID]		int				IDENTITY (1,1)					NOT NULL
	,[UserID]			int												NOT NULL    CONSTRAINT [DF_bx_UserShares_UserID]		DEFAULT (0)
	,[ShareID]			int												NOT NULL    CONSTRAINT [DF_bx_UserShares_ShareID]		DEFAULT (0)
    ,[PrivacyType]      tinyint                                         NOT NULL    CONSTRAINT [DF_bx_UserShares_PrivacyType]   DEFAULT (0)
    ,[Subject]			nvarchar(100)	collate Chinese_PRC_CI_AS_WS	not null	constraint [DF_bx_UserShares_Subject]		default ('')
	,[Description]		nvarchar(1000)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_UserShares_Decription]	DEFAULT ('')
	,[CreateDate]		datetime										NOT NULL	CONSTRAINT [DF_bx_UserShares_CreateDate]	DEFAULT (GETDATE())
	,[CommentCount]		int												NOT NULL	CONSTRAINT [DF_bx_UserShares_CommentCount]	DEFAULT (0)
	
    ,[KeywordVersion]           varchar(32)       COLLATE Chinese_PRC_CI_AS_WS      NOT NULL    CONSTRAINT [DF_bx_UserShares_KeywordVersion]       DEFAULT ('')
    
    ,CONSTRAINT [PK_bx_UserShares] PRIMARY KEY ([UserShareID])
);

create table [bx_ShareAgreeOrOpposeLogs] (
	[ShareID]		int			not null	constraint [DF_bx_ShareAgreeOrOpposeLogs_ShareID]		default(0),
	[UserID]		int			not null	constraint [DF_bx_ShareAgreeOrOpposeLogs_UserID]		default(0),
	[IsAgree]		bit			not null	constraint [DF_bx_ShareAgreeOrOpposeLogs_IsAgree]		default(0),
	[CreateDate]	datetime	not null	constraint [DF_bx_ShareAgreeOrOpposeLogs_CreateDate]	default(getdate()),
	
	constraint [PK_bx_ShareAgreeOrOpposeLogs] primary key ([ShareID], [UserID])
);

go

CREATE VIEW [bx_SharesView] AS
SELECT 
    B.[UserShareID],
	A.[ShareID], 
	A.[UserID], 
	A.[TotalComments], 
	A.[Type], 
	A.[Url],
	A.[Content], 
	A.[LastCommentDate], 
	A.[KeywordVersion], 
	A.[AgreeCount], 
	A.[OpposeCount],
	A.[ShareCount],
	A.[AgreeAndOpposeCount],
	B.[UserID] as [UserID2],
	B.[PrivacyType], 
	B.[Subject],
	B.[Description],
	B.[CreateDate],
	B.[CommentCount],
	A.[SortOrder]
FROM
	[bx_Shares] A
RIGHT JOIN 
	[bx_UserShares] B ON A.[ShareID] = B.[ShareID]

go

/*
Name: 分享表
Columns:
    [ShareID]              唯一标志
    [UserID]               用户ID
    [TotalComments]        评论数
    
    [Type]                 分享类型(如网址,视频等等)
    [PrivacyType]          隐私类型(0所有人可见1好友可见2自己可见就是收藏了)
    
    [Url]                  网址 如视频网址
    [Content]              内容
    [Description]          分享者的评论
    [DescriptionReverter]  “替换关键字”恢复信息
    
    [CreateDate]           时间
*/

GO

EXEC bx_Drop 'IX_bx_Shares_UserID';
CREATE  INDEX [IX_bx_Shares_UserID] ON [bx_Shares]([UserID],[Type])
EXEC bx_Drop 'IX_bx_Shares_TargetID';
CREATE  INDEX [IX_bx_Shares_TargetID] ON [bx_Shares]([TargetID])
EXEC bx_Drop 'IX_bx_Shares_CreateDate';
CREATE  INDEX [IX_bx_Shares_CreateDate] ON [bx_Shares]([CreateDate])
EXEC bx_Drop 'IX_bx_Shares_AgreeAndOpposeCount';
CREATE  INDEX [IX_bx_Shares_AgreeAndOpposeCount] ON [bx_Shares]([AgreeAndOpposeCount])


CREATE  INDEX [IX_bx_UserShares_UserID] ON [bx_UserShares]([UserID])
GO

GO
EXEC bx_Drop 'bx_SmsCodes';

GO

CREATE TABLE [bx_SmsCodes] (
	[SmsCodeID]             int      IDENTITY(1, 1)                  NOT NULL         CONSTRAINT [PK_bx_SmsCodes]                      PRIMARY KEY ([SmsCodeID])
	,[UserID]				int                                      NOT NULL
	,[IsUsed]				bit										 NOT NULL		  CONSTRAINT [DF_bx_SmsCodes_IsUsed]               DEFAULT(0)
	,[ActionType]           tinyint                                  NOT NULL
	,[CreateDate]			datetime                                 NOT NULL         CONSTRAINT [DF_bx_SmsCodes_CreatDate]			   DEFAULT (GETDATE())
	,[ExpiresDate]			datetime                                 NOT NULL
	,[MobilePhone]			bigint                                   NOT NULL
	,[SmsCode]				varchar(10)								 NOT NULL
	,[ChangedMobilePhone]   bigint									 NOT NULL		  CONSTRAINT [DF_bx_SmsCodes_ChangedMobilePhone]   DEFAULT(0)
	,[ChangedSmsCode]		varchar(10)								 NOT NULL		  CONSTRAINT [DF_bx_SmsCodes_ChangedSmsCode]       DEFAULT('')
)

CREATE INDEX [IX_bx_SmsCodes_Key] ON [bx_SmsCodes] ([UserID]);

CREATE INDEX [IX_bx_SmsCodes_ExpiresDate] ON [bx_SmsCodes] ([ExpiresDate]);

--CREATE INDEX [IX_bx_SmsCodes_] ON [bx_SmsCodes] ([UserID], [ExpiresDate]);

GO


GO
CREATE TABLE [bx_Stats]
(
[StatType] [tinyint] NOT NULL CONSTRAINT [DF_bx_Stats_StatType] DEFAULT ((0)),
[Year] [smallint] NOT NULL,
[Month] [tinyint] NOT NULL,
[Day] [tinyint] NOT NULL,
[Hour] [tinyint] NOT NULL,
[Count] [int] NOT NULL,
[Param] [int] NOT NULL CONSTRAINT [DF_bx_Stats_Param] DEFAULT ((0))
)
GO
EXEC bx_Drop 'bx_StepByStepTasks';

CREATE TABLE [bx_StepByStepTasks] (
     [TaskID]           uniqueidentifier                                 NOT NULL
    ,[Type]             varchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_Type]             DEFAULT ('')
	,[UserID]			int												 NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_UserID]           DEFAULT (0)
	,[Param]			nvarchar(3500)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_Param]            DEFAULT ('')
	,[TotalCount]		int												 NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_TotalCount]       DEFAULT (0)
	,[FinishedCount]	int												 NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_FinishedCount]    DEFAULT (0)
	,[Offset]			bigint											 NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_Offset]           DEFAULT (0)
	,[Title]			nvarchar(100)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_Title]            DEFAULT ('')
	,[LastExecuteTime]  datetime                                         NOT NULL    CONSTRAINT [DF_bx_StepByStepTasks_LastExecuteTime]	 DEFAULT (GETDATE())
	,[InstanceMode]     tinyint											 NOT NULL

    ,CONSTRAINT [PK_bx_StepByStepTasks] PRIMARY KEY ([TaskID])
);

/*
Name: JOB执行时间表
Columns:
    [Type]					任务类型

    [LastExecuteTime]       上次执行时间
*/

GO


GO
EXEC bx_Drop 'bx_StickThreads';

CREATE TABLE [bx_StickThreads] (
	 [ID]           int             IDENTITY (1, 1)                   NOT NULL 
	,[ThreadID]		int				NOT NULL    CONSTRAINT [DF_bx_StickThreads_ThreadID]         DEFAULT (0)
    ,[ForumID]		int				NOT NULL    CONSTRAINT [DF_bx_StickThreads_ForumID]          DEFAULT (0)
	
    ,CONSTRAINT [PK_bx_StickThreads] PRIMARY KEY ([ID])
);

/*
Name: 一般置顶的主题
Columns:
    [ThreadID]				主题ID
	[ForumID]				版块ID
*/

CREATE UNIQUE INDEX [IX_bx_StickThreads_ForumThread] ON [bx_StickThreads]([ThreadID],[ForumID]);
CREATE INDEX [IX_bx_StickThreads_ForumID] ON [bx_StickThreads]([ForumID]);
GO


GO
--系统通知表
EXEC bx_Drop bx_SystemNotifies;
GO
CREATE TABLE [bx_SystemNotifies] (
     [NotifyID]             int                    IDENTITY(1,1)                  NOT NULL   
	,[BeginDate]            datetime                                              NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_BeginDate]       DEFAULT(GETDATE())
	,[Subject]              nvarchar(200)      COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_Subject]		 DEFAULT(N'')
	,[EndDate]              datetime											  NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_EndDate]         DEFAULT('2099-1-1')
	,[ReceiveRoles]         text               COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_ReceiveRoles]    DEFAULT('')
	,[ReceiveUserIDs]       varchar(2000)      COLLATE Chinese_PRC_CI_AS_WS		  NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_ReceiveUserIDs]  DEFAULT('')
	,[Content]				nvarchar(1000)	   COLLATE Chinese_PRC_CI_AS_WS	      NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_Content]		 DEFAULT(N'')	
    ,[DispatcherID]         int													  NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_DispatcherID]    DEFAULT(0)
    ,[DispatcherIP]         varchar(200)       COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_DispatcherIP]    DEFAULT('')
    ,[CreateDate]           datetime											  NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_CreateDate]      DEFAULT(GETDATE())
    ,[ReadUserIDs]          text               COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_SystemNotifies_ReadUserIDs]     DEFAULT(',')  
	,CONSTRAINT [PK_bx_SystemNotifies] PRIMARY KEY ([NotifyID])
);
GO
GO
EXEC bx_Drop bx_TagRelation;

GO

CREATE TABLE [bx_TagRelation](
    [TagID]               int									       NOT NULL    CONSTRAINT  [DF_bx_TagRelation_TagID]              DEFAULT(0)
   ,[Type]                tinyint                                      NOT NULL    CONSTRAINT  [DF_bx_TagRelation_Type]               DEFAULT(0)
   ,[TargetID]            int                                          NOT NULL    CONSTRAINT  [DF_bx_TagRelation_TargetID]           DEFAULT(0)
   
)

/*
Name:标签
Columns:
    [TagID]               标签ID
    [Type]                标签类型
	[TargetID]            使用该标签的对象ID
*/

GO


--创建标签使用对象索引
CREATE INDEX  [IX_bx_TagRelation_TargetID] ON [bx_TagRelation]([TargetID]);

--创建标签类型索引
CREATE INDEX  [IX_bx_TagRelation_Type] ON [bx_TagRelation]([Type]);



GO
EXEC bx_Drop bx_Tags;

GO

CREATE TABLE [bx_Tags](
    [ID]               int              IDENTITY(1, 1)                  NOT NULL    CONSTRAINT  [PK_bx_Tags]                PRIMARY KEY ([ID])
   ,[IsLock]           bit                                              NOT NULL    CONSTRAINT  [DF_bx_Tags_IsLock]         DEFAULT(0)
   ,[Name]             nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Tags_Name]           DEFAULT('')
   ,[TotalElements]    int                                              NOT NULL    CONSTRAINT  [DF_bx_Tags_TotalElements]  DEFAULT(0)
)

/*
Name:标签
Columns:
    [ID]               自动标识ID
    [IsLock]	       是否被锁定
	[Name]             标签
	[TotalElements]    总使用数
*/

GO


--标签名称索引
CREATE INDEX [IX_bx_Tags_Name] ON [bx_Tags]([Name])


GO
EXEC bx_Drop bx_TempUploadFiles;

GO

CREATE TABLE [bx_TempUploadFiles](
    [TempUploadFileID]      int              IDENTITY(1, 1)                  NOT NULL    CONSTRAINT  [PK_bx_TempUploadFiles]              PRIMARY KEY ([TempUploadFileID])
   ,[UserID]                int                                              NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_UserID]		  DEFAULT (0)
   ,[UploadAction]          varchar(100)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_UploadAction] DEFAULT ('')
   ,[SearchInfo]            nvarchar(100)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_SearchInfo]   DEFAULT (N'')
   ,[CustomParams]          nvarchar(3000)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_CustomParams] DEFAULT (N'')
   ,[FileName]              nvarchar(256)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_FileName]     DEFAULT (N'')
   ,[ServerFileName]        varchar(100)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_ServerFileName]   DEFAULT ('')
   ,[MD5]					char(32)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_MD5]          DEFAULT ('')
   ,[FileSize]              bigint                                           NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_FileSize]     DEFAULT (0)
   ,[FileID]				varchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_FileID]       DEFAULT ('')
   ,[CreateDate]            datetime                                         NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_CreateDate]   DEFAULT (GETDATE())
);

/*
Name:
Columns:

*/

--GO

--CREATE INDEX [IX_bx_TempUploadFiles_Key] ON [bx_TempUploadFiles]([MD5], [FileSize]);

GO

CREATE INDEX [IX_bx_TempUploadFiles_Search] ON [bx_TempUploadFiles]([UserID], [UploadAction], [SearchInfo]);

GO

CREATE INDEX [IX_bx_TempUploadFiles_CreateDate] ON [bx_TempUploadFiles]([CreateDate]);

GO
GO
EXEC bx_Drop 'bx_ThreadCatalogs';

CREATE TABLE [bx_ThreadCatalogs]
(
[ThreadCatalogID] [int] NOT NULL IDENTITY(1, 1),
[ThreadCatalogName] [nvarchar] (400) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[LogoUrl] [nvarchar] (512) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
CONSTRAINT [PK_bx_ThreadCatalogs] PRIMARY KEY ([ThreadCatalogID])
)

GO

GO
EXEC bx_Drop 'bx_ThreadCatalogsInForums';

CREATE TABLE [bx_ThreadCatalogsInForums]
(
[ForumID] [int] NOT NULL,
[ThreadCatalogID] [int] NOT NULL,
[TotalThreads] [int]  NOT NULL CONSTRAINT [DF_bx_ThreadCatalogsInForums_TotalThreads] DEFAULT ((0)),
[SortOrder] [int] NOT NULL,
CONSTRAINT [PK_bx_ThreadCatalogsInForums] PRIMARY KEY ([ForumID], [ThreadCatalogID])
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_ThreadCatalogsInForums] ON [bx_ThreadCatalogsInForums] ([ForumID], [SortOrder]);

GO

GO
EXEC bx_Drop 'bx_ThreadCateModelFields';

CREATE TABLE [bx_ThreadCateModelFields] (
	 [FieldID]              int             IDENTITY(1, 1)					NOT NULL    CONSTRAINT [PK_bx_ThreadCateModelFields]                  PRIMARY KEY ([FieldID])
    ,[ModelID]				int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_ModelID]          DEFAULT (0)
    ,[FieldName]			nvarchar(50)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_FieldName]        DEFAULT ('')
    ,[Enable]				bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_Enable]           DEFAULT (1) 
    ,[SortOrder]			int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_SortOrder]        DEFAULT (0)
	,[FieldType]			varchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_FieldType]        DEFAULT ('')
	,[Description]			nvarchar(1000)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_Description]      DEFAULT ('')
    ,[FieldTypeSetting]	    ntext		    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL	CONSTRAINT [DF_bx_ThreadCateModelFields_FieldSetting]     DEFAULT ('')
    ,[Search]				bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_Search]           DEFAULT (0) 
    ,[AdvancedSearch]		bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_AdvancedSearch]   DEFAULT (0) 
    ,[DisplayInList]		bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_DisplayInList]    DEFAULT (0) 
    ,[MustFilled]			bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_MustFilled]       DEFAULT (0)
    --,[ModelID]				int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModelFields_ModelID]          DEFAULT (0) 
);

/*
Name: 分类主题
Columns:
	[Field]		   ID
	[ModelID]	   模板ID
    [FieldName]    名称
    [Enable]       是否启用
	[SortOrder]    排序小的排在前面
	[FieldType]    字段类型
	[FieldTypeSetting]  发帖处显示内容
	[Search]	   可以默认搜索
    [AdvancedSearch]  高级搜索
    [DisplayInList]   是否在帖子列表中显示
    [MustFilled]	  是否必填 
*/

GO

GO
EXEC bx_Drop 'bx_ThreadCateModels';

CREATE TABLE [bx_ThreadCateModels] (
	 [ModelID]              int             IDENTITY(1, 1)					NOT NULL    CONSTRAINT [PK_bx_ThreadCateModels]                  PRIMARY KEY ([ModelID])
    ,[CateID]				int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModels_CateID]           DEFAULT (0)
    ,[ModelName]			nvarchar(50)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_ThreadCateModels_ModelName]        DEFAULT ('')
    ,[Enable]				bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModels_Enable]           DEFAULT (1) 
    ,[SortOrder]			int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCateModels_SortOrder]        DEFAULT (0) 
);

/*
Name: 分类主题
Columns:
	[ModeID]	   ID
    [CateID]       分类主题ID
    [ModelName]     名称
    [Enable]       是否启用
	[SortOrder]    排序小的排在前面
*/

GO

GO
EXEC bx_Drop 'bx_ThreadCates';

CREATE TABLE [bx_ThreadCates] (
	 [CateID]               int                 IDENTITY(1, 1)              NOT NULL    CONSTRAINT [PK_bx_ThreadCates]                  PRIMARY KEY ([CateID])
    ,[CateName]				nvarchar(50)	  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_ThreadCates_CateName]         DEFAULT ('')
    ,[Enable]				bit                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCates_Enable]           DEFAULT (1) 
    ,[SortOrder]			int                                             NOT NULL    CONSTRAINT [DF_bx_ThreadCates_SortOrder]        DEFAULT (0)
);

/*
Name: 分类主题
Columns:
    [CateID]       ID
    [CateName]     名称
    [Enable]       是否启用
	[SortOrder]    排序小的排在前面
    
*/

GO

GO
EXEC bx_Drop 'bx_ThreadExchanges';

CREATE TABLE [bx_ThreadExchanges]
(
	[ThreadID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[Price] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	CONSTRAINT [PK_bx_ThreadExchanges] PRIMARY KEY ([ThreadID], [UserID])
)

GO

GO
EXEC bx_Drop 'bx_ThreadImages';

CREATE TABLE [bx_ThreadImages] (
	 [ThreadID]		int			NOT NULL    CONSTRAINT [DF_bx_ThreadImages_ThreadID]         DEFAULT (0)
	,[AttachmentID] int			NOT NULL    CONSTRAINT [DF_bx_ThreadImages_AttachmentID]     DEFAULT (0)
    ,[ImageUrl]     varchar(200)  COLLATE Chinese_PRC_CI_AS_WS NOT NULL
    ,[ImageCount]   int			NOT NULL    CONSTRAINT [DF_bx_ThreadImages_ImageCount]       DEFAULT (0)
	
    ,CONSTRAINT [PK_bx_ThreadImages] PRIMARY KEY ([ThreadID])
);


GO


GO
EXEC bx_Drop 'bx_ThreadManageLogs';

CREATE TABLE [bx_ThreadManageLogs]
(
[LogID] [int] NOT NULL IDENTITY(1, 1),
[UserID] [int] NOT NULL,
[UserName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[NickName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_NickName] DEFAULT (''),
[IPAddress] [varchar] (15) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[PostUserID] [int] NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_PostUserID] DEFAULT ((0)),
[ActionType] [tinyint] NOT NULL,
[ForumID] [int] NOT NULL,
[ThreadID] [int] NOT NULL,
[ThreadSubject] [nvarchar] (256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_ThreadSubject] DEFAULT (''),
[Reason] [nvarchar] (256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_Reason] DEFAULT (''),
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_CreatDate] DEFAULT (getdate()),
[IsPublic] [bit] NOT NULL CONSTRAINT [DF_bx_ThreadManageLogs_IsPublic] DEFAULT (1),
CONSTRAINT [PK_bx_ThreadManageLogs] PRIMARY KEY ([LogID])
)

GO

CREATE NONCLUSTERED INDEX [IX_ThreadManageLog_ActionType] ON [bx_ThreadManageLogs] ([ActionType]);
CREATE NONCLUSTERED INDEX [IX_ThreadManageLog_UserID_ActionType] ON [bx_ThreadManageLogs] ([UserID], [ActionType]);
CREATE NONCLUSTERED INDEX [IX_ThreadManageLog_UserID] ON [bx_ThreadManageLogs] ([UserID]);
GO

GO
EXEC bx_Drop 'bx_ThreadRanks';

CREATE TABLE [bx_ThreadRanks]
(
[ThreadID] [int] NOT NULL,
[UserID] [int] NOT NULL,
[Rank] [tinyint] NOT NULL CONSTRAINT [DF_bx_ThreadRanks_Rank] DEFAULT ((0)),
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_ThreadRanks_CreateDate] DEFAULT (getdate()),
[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_ThreadRanks_UpdateDate] DEFAULT (getdate()),
CONSTRAINT [PK_bx_ThreadRanks] PRIMARY KEY ([ThreadID], [UserID])
)

GO

CREATE NONCLUSTERED INDEX [IX_ThreadRanks_CreateDate] ON [bx_ThreadRanks] ([CreateDate] DESC);
GO

GO
EXEC bx_Drop bx_ThreadReverters;

GO

CREATE TABLE [bx_ThreadReverters](
	[ThreadID]				int				NOT NULL,
	[SubjectReverter]		nvarchar(4000)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_ThreadReverters] PRIMARY KEY([ThreadID])
)

/*
Name:标签
Columns:
    [AlbumID]		       可恢复的相册ID
	[NameReverter]         相册名复原关键信息，可根据此信息恢复相册名的原始内容
*/

GO
GO
EXEC bx_Drop 'bx_Threads';

CREATE TABLE [bx_Threads]
(
[ThreadID] [int] NOT NULL IDENTITY(1, 1),
[ForumID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_ForumID] DEFAULT ((0)),
[ThreadCatalogID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_ThreadCatalogID] DEFAULT ((0)),
[ThreadType] [tinyint] NOT NULL CONSTRAINT [DF_bx_Threads_ThreadType] DEFAULT ((0)),
--[ThreadStatus] [tinyint] NOT NULL CONSTRAINT [DF_bx_Threads_ThreadStatus] DEFAULT ((0)),
[IconID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_EmoticonID] DEFAULT ((0)),
[Subject] [nvarchar] (256) COLLATE Chinese_PRC_CI_AS_WS NOT NULL,
[SubjectStyle] [nvarchar] (300) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Threads_SubjectStyle] DEFAULT ((0)),
[TotalReplies] [int] NOT NULL CONSTRAINT [DF_bx_Threads_TotalReplies] DEFAULT ((0)),
[TotalViews] [int] NOT NULL CONSTRAINT [DF_bx_Threads_TotalViews] DEFAULT ((0)),
[TotalAttachments] [int] NOT NULL CONSTRAINT [DF_bx_Threads_TotalAttachments] DEFAULT ((0)),
[Price] [int] NOT NULL CONSTRAINT [DF_bx_Threads_Price] DEFAULT ((0)),
[Rank] [tinyint] NOT NULL CONSTRAINT [DF_bx_Threads_Rank] DEFAULT ((0)),
[PostUserID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_PostUserID] DEFAULT ((0)),
[PostNickName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Threads_PostNickName] DEFAULT (''),
[LastPostUserID] [int] NOT NULL CONSTRAINT [DF_bx_Threads_LastPostUserID] DEFAULT ((0)),
[LastPostNickName] [nvarchar] (64) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Threads_LastPostNickName] DEFAULT (''),
[IsLocked] [bit] NOT NULL CONSTRAINT [DF_bx_Threads_IsLocked] DEFAULT ((0)),
[IsValued] [bit] NOT NULL CONSTRAINT [DF_bx_Threads_IsValued] DEFAULT ((0)),
[Perorate] [nvarchar] (32) COLLATE Chinese_PRC_CI_AS_WS NOT NULL CONSTRAINT [DF_bx_Threads_Perorate] DEFAULT (''),
[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_Threads_CreateDate] DEFAULT (getdate()),
[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_bx_Threads_UpdateDate] DEFAULT (getdate()),
[SortOrder] [bigint] NOT NULL,
[ThreadStatus] [tinyint] NOT NULL CONSTRAINT [DF_bx_Posts_ThreadStatus] DEFAULT (1),
[UpdateSortOrder] [bit] NOT NULL CONSTRAINT [DF_bx_Threads_UpdateSortOrder] DEFAULT ((1)),
[ThreadLog] [nvarchar] (128) NOT NULL CONSTRAINT [DF_bx_Threads_ThreadLog] DEFAULT (''),
[JudgementID] [int] CONSTRAINT [DF_bx_Threads_JudgementID] DEFAULT (0),
[ShareCount] [int] CONSTRAINT [DF_bx_Threads_ShareCount] DEFAULT (0),
[CollectionCount] [int] CONSTRAINT [DF_bx_Threads_CollectionCount] DEFAULT (0),
[KeywordVersion]      varchar(32)      COLLATE Chinese_PRC_CI_AS_WS    NULL,
[Words]      nvarchar(400)      COLLATE Chinese_PRC_CI_AS_WS    NULL,
[LastPostID] [int] CONSTRAINT [DF_bx_Threads_LastPostID] DEFAULT (0),
[ContentID] [int] CONSTRAINT [DF_bx_Threads_ContentID] DEFAULT (0),
[PostedCount] [int] NULL, 
[ExtendData] [ntext] COLLATE Chinese_PRC_CI_AS_WS NULL,
[AttachmentType] [tinyint] NOT NULL CONSTRAINT [DF_bx_Posts_AttachmentType] DEFAULT (0), --0没有附件 1一般附件 2有图片附件
CONSTRAINT [PK_bx_Threads] PRIMARY KEY ([ThreadID])
)

GO



CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_SortOrder] ON [bx_Threads] ([SortOrder] DESC);

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_ThreadStatus] ON [bx_Threads] ([ThreadStatus],[SortOrder] DESC);

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Forum] ON [bx_Threads] ([ForumID],[ThreadStatus],[SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Catalog] ON [bx_Threads] ([ThreadCatalogID], [ForumID], [ThreadStatus], [SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Type] ON [bx_Threads] ([ThreadType], [ForumID],[ThreadStatus], [SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_User] ON [bx_Threads] ([PostUserID], [ThreadStatus], [SortOrder] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Valued] ON [bx_Threads] ([IsValued], [ForumID],[ThreadStatus], [SortOrder] DESC);

CREATE INDEX [IX_bx_Threads_CreateDate] ON [bx_Threads] ([CreateDate] DESC);
CREATE INDEX [IX_bx_Threads_UpdateDate] ON [bx_Threads] ([UpdateDate] DESC);
CREATE INDEX [IX_bx_Threads_ForumTotalReplies] ON [bx_Threads] ([ForumID],[ThreadStatus], [TotalReplies] DESC);
CREATE INDEX [IX_bx_Threads_ForumTotalViews] ON [bx_Threads] ([ForumID],[ThreadStatus], [TotalViews] DESC);
CREATE INDEX [IX_bx_Threads_TotalReplies] ON [bx_Threads] ([ThreadStatus], [TotalReplies] DESC);
CREATE INDEX [IX_bx_Threads_TotalViews] ON [bx_Threads] ([ThreadStatus], [TotalViews] DESC);

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_LastPostID] ON [bx_Threads] ([ThreadStatus],[LastPostID] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Catalog2] ON [bx_Threads] ([ThreadCatalogID], [ForumID], [ThreadStatus], [LastPostID] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Forum2] ON [bx_Threads] ([ForumID],[ThreadStatus],[LastPostID] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Type2] ON [bx_Threads] ([ThreadType], [ForumID],[ThreadStatus], [LastPostID] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_User2] ON [bx_Threads] ([PostUserID], [ThreadStatus], [LastPostID] DESC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Threads_Valued2] ON [bx_Threads] ([IsValued], [ForumID],[ThreadStatus], [LastPostID] DESC);

GO
GO
EXEC bx_Drop bx_ThreadWords;

GO

CREATE TABLE [bx_ThreadWords](
    [ThreadID]         int							                    NOT NULL    CONSTRAINT  [DF_bx_ThreadWords_ThreadID]           DEFAULT(0)
   ,[Word]             nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_ThreadWords_Words]              DEFAULT('')
)

/*
Name:标签
Columns:
    [ThreadID]          主题ID
    [Word]				关键字
*/

GO


CREATE INDEX [IX_bx_ThreadWords_ThreadID] ON [bx_ThreadWords]([ThreadID])
CREATE INDEX [IX_bx_ThreadWords_Word] ON [bx_ThreadWords]([Word])


GO
EXEC bx_Drop 'bx_TopicStatus';

CREATE TABLE [bx_TopicStatus] (
	 [ID]           int             IDENTITY (1, 1)                   NOT NULL 
	,[ThreadID]		int				NOT NULL    CONSTRAINT [DF_bx_TopicStatus_ThreadID]         DEFAULT (0)
    ,[Type]			tinyint         NOT NULL    CONSTRAINT [DF_bx_TopicStatus_Type]             DEFAULT (0)
	,[EndDate]		datetime        NOT NULL    CONSTRAINT [DF_bx_TopicStatus_EndDate]			DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_TopicStatus] PRIMARY KEY ([ID])
);

/*
Name: 主题定时状态
Columns:
    [ThreadID]				主题ID
	[Type]					类型 （置顶，高亮，锁定）
    [EndDate]               过期时间
*/

CREATE UNIQUE INDEX [IX_bx_TopicStatus_ThreadType] ON [bx_TopicStatus]([ThreadID],[Type]);
CREATE INDEX [IX_bx_TopicStatus_EndDate] ON [bx_TopicStatus]([EndDate]);

GO


GO
exec bx_Drop 'bx_UnreadNotifies';

CREATE TABLE bx_UnreadNotifies
(
	[UserID]			INT		NOT NULL				
	,[TypeID]			INT		NOT NULL				
	,[UnreadCount]		INT		NOT NULL				CONSTRAINT  [DF_bx_UnreadNotifies_UnreadCount]			DEFAULT(1)
	,CONSTRAINT [PK_bx_UnreadNotifies] PRIMARY KEY ([UserID],[TypeID]) 
);


GO
--CREATE TABLE bx_UserAvatarLocks (
	--[UserID]				int											NOT NULL	PRIMARY KEY CONSTRAINT [DF_bx_UserAvatarLocks_UserID]			DEFAULT(0)	
    --,[AvatarLockAt]			datetime									NOT NULL				CONSTRAINT [DF_bx_UserAvatarLocks_AvatarLockAt]		DEFAULT(GETDATE())
    --,[AvatarLockTo]			datetime									NOT NULL				CONSTRAINT [DF_bx_UserAvatarLocks_AvatarLockTo]		DEFAULT(GETDATE())
    --,[AvatarLockReason]		nvarchar(512) COLLATE Chinese_PRC_CI_AS_WS	NOT NULL				CONSTRAINT [DF_bx_UserAvatarLocks_AvatarLockReason]	DEFAULT('')
    --,[OldAvatarSrc]         nvarchar(200) COLLATE Chinese_PRC_CI_AS_WS	NOT NULL				CONSTRAINT [DF_bx_UserAvatarLocks_OldAvatarSrc]		DEFAULT('')
 --)
GO
EXEC bx_Drop 'bx_UserExtendedValues';

CREATE TABLE [bx_UserExtendedValues] (
     [UserID]            int										      NOT NULL
    ,[ExtendedFieldID]   varchar(36)        COLLATE Chinese_PRC_CI_AS_WS  NOT NULL
    
    ,[Value]             nvarchar(3950)     COLLATE Chinese_PRC_CI_AS_WS  NOT NULL
    ,[PrivacyType]		 tinyint										  NOT NULL     CONSTRAINT [DF_bx_PrivacyType]          DEFAULT (0)
    
    ,CONSTRAINT [PK_bx_UserExtendedValues] PRIMARY KEY ([UserID],[ExtendedFieldID],[PrivacyType])
);

GO


CREATE INDEX [IX_bx_UserExtendedValues] ON [bx_UserExtendedValues] ([ExtendedFieldID],[PrivacyType] DESC);

/*
Name:用户的扩展字段表
Columns:
	[UserID]              用户ID
	[ExtendedFieldID]     扩展字段ID
	[Value]               扩展字段的值
*/
GO
EXEC bx_Drop 'bx_UserFeeds';

CREATE TABLE [bx_UserFeeds] (
	 [ID]				int                 IDENTITY (1, 1)                 NOT NULL 
    ,[FeedID]           int                                                 NOT NULL 
    ,[UserID]           int                                                 NOT NULL
    
    ,[Realname]         nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS       NOT NULL    CONSTRAINT [DF_bx_UserFeeds_Username]       DEFAULT ('') 
    
	,[CreateDate]       datetime                                            NOT NULL    CONSTRAINT [DF_bx_UserFeeds_CreateDate]     DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_UserFeeds] PRIMARY KEY ([UserID],[FeedID])
);

/*
Name: 用户动态表
      记录每条动态相关的用户
      比如 a 评论了f的日志"伤感" b也评论了f的日志"伤感"  那么这里就记a和b的用户ID并且对应同一个FeedID
      如果 a和f成为了好友 b也和f成为了好友 那么这里除了记录a和b的用户ID外 还需记录f的用户ID (就这个特殊与其它不同)
Columns:
	[ID]
    [FeedID]           动态ID
    [UserID]           用户ID  如果是全局动态时为-1
        
    [Realname]         昵称
    
    [CreateDate]       时间
*/

EXEC bx_Drop 'IX_bx_UserFeeds_CreateDate';
CREATE  INDEX [IX_bx_UserFeeds_CreateDate] ON [bx_UserFeeds]([CreateDate])

EXEC bx_Drop 'IX_bx_UserFeeds_ID';
CREATE  UNIQUE  INDEX [IX_bx_UserFeeds_ID] ON [bx_UserFeeds]([ID])

EXEC bx_Drop 'IX_bx_UserFeeds_FeedID';
CREATE  INDEX [IX_bx_UserFeeds_FeedID] ON [bx_UserFeeds]([FeedID])

GO

GO
EXEC bx_Drop 'bx_UserGetPropLogs';

CREATE TABLE [bx_UserGetPropLogs](
	 LogID						int							IDENTITY(1,1)				CONSTRAINT [PK_bx_UserGetPropLogs_LogID]		PRIMARY KEY ([LogID])
	,UserID						int							NOT NULL
	,Username					nvarchar(50)				NOT NULL
	,GetPropType				tinyint						NOT NULL
	,PropID						int							NOT NULL
	,PropName					nvarchar(50)				NOT NULL
	,PropCount					int							NOT NULL					CONSTRAINT [DF_bx_UserGetPropLogs_PropCount]	DEFAULT(0)
	,CreateDate					datetime					NOT NULL					CONSTRAINT [DF_bx_UserGetPropLogs_CreateDate]	DEFAULT(getdate())
)
GO
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
GO
EXEC bx_Drop 'bx_UserMedals';

CREATE TABLE [bx_UserMedals] (
     [UserID]            int										      NOT NULL
    ,[MedalID]           int										      NOT NULL 
    ,[MedalLevelID]      int											  NOT NULL
    ,[Url]               nvarchar(200)	  COLLATE Chinese_PRC_CI_AS_WS    NULL    
    ,[EndDate]           datetime                                         NULL
    ,[CreateDate]        datetime                                         NOT NULL    CONSTRAINT [DF_bx_UserMedals_CreateDate]      DEFAULT (GETDATE())
    
	,CONSTRAINT [PK_bx_UserMedals] PRIMARY KEY ([UserID],[MedalID],[MedalLevelID])
);

GO

CREATE NONCLUSTERED INDEX [IX_bx_UserMedals_EndDate] ON [bx_UserMedals] ([EndDate]);

GO
/*
Name:用户的勋章表
Columns:
	[UserID]              用户ID
	[MedalID]             勋章	
	[MedalID]             勋章等级ID
	
	[CreateDate]          创建时间
	[EndDate]             过期时间
*/
GO
EXEC bx_Drop 'bx_UserMissions';

CREATE TABLE [bx_UserMissions] (
	 [ID]				int                 IDENTITY (1, 1)                 NOT NULL 
    ,[UserID]           int                                                 NOT NULL
    ,[MissionID]        int                                                 NOT NULL
    
    ,[FinishPercent]    float                                               NOT NULL    CONSTRAINT [DF_bx_UserMissions_FinishPercent]  DEFAULT (0)
    
    ,[Status]           tinyint                                             NOT NULL    CONSTRAINT [DF_bx_UserMissions_Status]         DEFAULT (0)
    
    ,[IsPrized]         bit                                                 NOT NULL    CONSTRAINT [DF_bx_UserMissions_IsPrized]       DEFAULT (0)

    ,[FinishDate]       datetime                                            NOT NULL    CONSTRAINT [DF_bx_UserMissions_FinishDate]     DEFAULT (GETDATE())
    ,[CreateDate]       datetime                                            NOT NULL    CONSTRAINT [DF_bx_UserMissions_CreateDate]     DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_UserMissions] PRIMARY KEY ([UserID],[MissionID])
);

/*
Name: 用户任务表
Columns:
    [ID]               
    [UserID]           用户ID
    [MissionID]        任务ID
    
    [FinishPercent]    完成百分比
    
    [Status]           任务状态: 0进行中 1完成 2超时未完成 3放弃任务
    
    [IsPrized]         是否领取过奖励了
    
    [FinishDate]       任务完成时间
    [CreateDate]       申请任务时间
*/

GO


EXEC bx_Drop 'IX_bx_UserMissions_CreateDate';
CREATE  INDEX [IX_bx_UserMissions_CreateDate] ON [bx_UserMissions]([CreateDate] DESC)

EXEC bx_Drop 'IX_bx_UserMissions_ID';
CREATE  UNIQUE  INDEX [IX_bx_UserMissions_ID] ON [bx_UserMissions]([ID])

GO

GO
EXEC bx_Drop 'bx_UserMobileOperationLogs';

CREATE TABLE [bx_UserMobileOperationLogs](
	LogID					int					NOT NULL		IDENTITY(1,1)			CONSTRAINT [PK_bx_UserMobileOperationLogs_LogID]			PRIMARY KEY ([LogID])
	
	,UserID					int					NOT NULL
	
	,Username				nvarchar(50)		NOT NULL
	
	,MobilePhone			bigint				NOT NULL								CONSTRAINT [DF_bx_UserMobileOperationLogs_MobilePhone]		DEFAULT(0)
	
	,OperationType			tinyint				NOT NULL								CONSTRAINT [DF_bx_UserMobileOperationLogs_OperationType]	DEFAULT(0)
	
	,OperationDate			datetime			NOT NULL								CONSTRAINT [DF_bx_UserMobileOperationLogs_OperationDate]	DEFAULT(GETDATE())						

)
GO
EXEC bx_Drop 'bx_UserNoAddFeedApps';

CREATE TABLE [bx_UserNoAddFeedApps] (
     [AppID]        uniqueidentifier         NOT NULL
     
    ,[UserID]       int                      NOT NULL
    
    ,[ActionType]   tinyint                  NOT NULL 
    
    ,[Send]			bit						 NOT NULL
 
    ,CONSTRAINT [PK_bx_UserNoAddFeedApps] PRIMARY KEY ([UserID],[AppID],[ActionType])
);

/*
Name: 用户的该类应用动态不加入通知
Columns:
    [AppID]            应用ID
    
    [UserID]           用户ID
    
    [ActionType]       APP动作枚举值(如"评论日志" "发表日志")
    
    [Send]			   是否发送
*/

GO

GO
EXEC bx_Drop 'bx_UserProps';

CREATE TABLE [bx_UserProps] (
   [UserPropID]   int IDENTITY(1,1) NOT NULL
  ,[UserID]       int               NOT NULL CONSTRAINT [DF_bx_UserProps_UserID]		DEFAULT (0)
  ,[PropID]       int               NOT NULL CONSTRAINT [DF_bx_UserProps_PropID]		DEFAULT (0)
  ,[Count]        int               NOT NULL CONSTRAINT [DF_bx_UserProps_Count]			DEFAULT (0)
  ,[SellingCount] int               NOT NULL CONSTRAINT [DF_bx_UserProps_SellingCount]	DEFAULT (0)
  ,[SellingPrice] int               NOT NULL CONSTRAINT [DF_bx_UserProps_SellingPrice]	DEFAULT (0)
  ,[SellingDate]  datetime          NOT NULL CONSTRAINT [DF_bx_UserProps_SellingDate]	DEFAULT (GETDATE())
  
  ,CONSTRAINT [PK_bx_UserProps] PRIMARY KEY ([UserPropID])
);

CREATE INDEX [IX_bx_UserProps_UserID_PropID_SellingCount] ON [bx_UserProps] ([UserID],[PropID],[SellingCount]);
GO
EXEC bx_Drop bx_UserReverters;

GO

CREATE TABLE bx_UserReverters(
	[UserID]					int				NOT NULL,
	[SignatureReverter]			nvarchar(4000)  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL,

	CONSTRAINT [PK_bx_UserReverters] PRIMARY KEY([UserID])
)

/*
Name:标签
Columns:
    [UserID]	                  ID
	[SignatureReverter]           内容复原关键信息，可根据此信息恢复内容的原始内容
*/

GO



GO
--用户用户组关系表
EXEC bx_Drop 'bx_UserRoles';

CREATE TABLE [bx_UserRoles](
     [UserID]         int                 NOT NULL
    ,[RoleID]         uniqueidentifier    NOT NULL

    ,[BeginDate]      datetime            NOT NULL
    ,[EndDate]        datetime            NOT NULL
    
    ,CONSTRAINT [PK_bx_UserRoles] PRIMARY KEY ([UserID],[RoleID])
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_UserRoles_UserRoles] ON [bx_UserRoles] ([RoleID], [UserID]);
CREATE NONCLUSTERED INDEX [IX_bx_UserRoles_EndDate] ON [bx_UserRoles] ([EndDate]);
GO


GO
EXEC bx_Drop 'bx_Users';

CREATE TABLE [bx_Users] (
     [UserID]                 int              IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_Users]                            PRIMARY KEY ([UserID])

    ,[Username]               nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
    ,[Realname]               nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_Realname]                   DEFAULT ('')
--//    ,[Password]               nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
--//    ,[PasswordFormat]         tinyint                                          NOT NULL    CONSTRAINT [DF_bx_Users_PasswordFormat]             DEFAULT (3)
    ,[Email]                  nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_Email]                      DEFAULT ('')
    ,[EmailValidated]         bit                                              NOT NULL    CONSTRAINT [DF_bx_Users_EmailValidated]             DEFAULT (0)
    ,[PublicEmail]            nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL	   CONSTRAINT [DF_bx_Users_PublicEmail]                DEFAULT ('')

	,[SpaceTheme]             nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_SpaceTheme]                 DEFAULT ('')
    ,[Doing]                  nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_Doing]                      DEFAULT ('')
	,[DoingDate]              datetime                                         NOT NULL    CONSTRAINT [DF_bx_Users_DoingDate]                  DEFAULT ('1753-1-1')
    ,[AvatarSrc]              nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_AvatarSrc]                  DEFAULT ('')

--//	,[LastAvatarUpdateDate]   datetime                                         NOT NULL    CONSTRAINT [DF_bx_Users_LastAvatarUpdateDate]       DEFAULT (GETDATE())

    ,[Gender]                 tinyint                                          NOT NULL    CONSTRAINT [DF_bx_Users_Gender]                     DEFAULT (0)
    ,[Signature]              nvarchar(1500)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_Signature]                  DEFAULT ('')
    ,[SignatureFormat]        tinyint                                          NOT NULL    CONSTRAINT [DF_bx_Users_SignatureFormat]            DEFAULT (0)
    ,[FriendGroups]           nvarchar(500)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_FriendGroups]               DEFAULT ('')
	
    ,[CreateIP]               varchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_CreateIP]                   DEFAULT ('')
    ,[LastVisitIP]            varchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_LastVisitIP]                DEFAULT ('')
	,[LastVisitDate]          datetime                                         NOT NULL    CONSTRAINT [DF_bx_Users_LastVisitDate]              DEFAULT (GETDATE())
	,[LastPostDate]           datetime                                         NOT NULL    CONSTRAINT [DF_bx_Users_LastPostDate]               DEFAULT ('1753-1-1')
    ,[CreateDate]             datetime                                         NOT NULL    CONSTRAINT [DF_bx_Users_CreateDate]                 DEFAULT (GETDATE())
    ,[UpdateDate]             datetime                                         NOT NULL    CONSTRAINT [DF_bx_Users_UpdateDate]                 DEFAULT (GETDATE())

	,[SpaceViews]             int                                              NOT NULL    CONSTRAINT [DF_bx_Users_SpaceViews]                 DEFAULT (0)
	,[LoginCount]             int                                              NOT NULL    CONSTRAINT [DF_bx_Users_LoginCount]                 DEFAULT (0)
    ,[IsActive]               bit                                              NOT NULL    CONSTRAINT [DF_bx_Users_IsActive]                   DEFAULT (1)

    ,[Points]				  int											   NOT NULL    CONSTRAINT [DF_bx_Users_Points]                     DEFAULT (0)
    ,[Point_1]                int											   NOT NULL    CONSTRAINT [DF_bx_Users_Point_1]                    DEFAULT (0)
    ,[Point_2]                int											   NOT NULL    CONSTRAINT [DF_bx_Users_Point_2]                    DEFAULT (0)
    ,[Point_3]                int											   NOT NULL    CONSTRAINT [DF_bx_Users_Point_3]                    DEFAULT (0)
    ,[Point_4]                int											   NOT NULL    CONSTRAINT [DF_bx_Users_Point_4]                    DEFAULT (0)
    ,[Point_5]                int											   NOT NULL    CONSTRAINT [DF_bx_Users_Point_5]                    DEFAULT (0)
    ,[Point_6]                int											   NOT NULL    CONSTRAINT [DF_bx_Users_Point_6]                    DEFAULT (0)
    ,[Point_7]                int											   NOT NULL    CONSTRAINT [DF_bx_Users_Point_7]                    DEFAULT (0)
    ,[Point_8]                int											   NOT NULL    CONSTRAINT [DF_bx_Users_Point_8]                    DEFAULT (0)	

    ,[TotalInvite]            int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalInvite]                DEFAULT (0)
    ,[TotalTopics]            int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalTopics]                DEFAULT (0) 
    ,[TotalPosts]             int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalPosts]                 DEFAULT (0)
    ,[WeekPosts]              int               							   NOT NULL    CONSTRAINT [DF_bx_Users_WeekPosts]                  DEFAULT (0)
    ,[DayPosts]               int               							   NOT NULL    CONSTRAINT [DF_bx_Users_DayPosts]                  DEFAULT (0)
    ,[MonthPosts]             int               							   NOT NULL    CONSTRAINT [DF_bx_Users_MonthPosts]                DEFAULT (0)
    --,[TotalFriends]           int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalFriends]               DEFAULT (0)

    --,[TotalViews]             int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalViews]                 DEFAULT (0)
    ,[TotalComments]          int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalComments]              DEFAULT (0)
    ,[TotalShares]            int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalShares]                DEFAULT (0)    
    ,[TotalCollections]       int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalCollections]           DEFAULT (0)
    ,[ValuedTopics]           int               							   NOT NULL    CONSTRAINT [DF_bx_Users_ValuedTopics]               DEFAULT (0)

    ,[DeletedTopics]          int               							   NOT NULL    CONSTRAINT [DF_bx_Users_DeletedTopics]              DEFAULT (0) 
    ,[DeletedReplies]         int               							   NOT NULL    CONSTRAINT [DF_bx_Users_DeletedReplies]             DEFAULT (0)
    ,[TotalBlogArticles]      int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalBlogArticles]          DEFAULT (0)
	,[TotalAlbums]            int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalAlbums]                DEFAULT (0)   

    ,[TotalPhotos]            int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalPhotos]                DEFAULT (0)
    ,[TotalDoings]            int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalDoings]                DEFAULT (0)
    --,[TotalVisitors]        int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalVisitors]              DEFAULT (0)

    ,[TotalOnlineTime]        int               							   NOT NULL    CONSTRAINT [DF_bx_Users_TotalOnlineTime]            DEFAULT (0)
    ,[MonthOnlineTime]        int               							   NOT NULL    CONSTRAINT [DF_bx_Users_MonthOnlineTime]            DEFAULT (0)
    ,[WeekOnlineTime]         int               							   NOT NULL    CONSTRAINT [DF_bx_Users_WeekOnlineTime]             DEFAULT (0)
    ,[DayOnlineTime]          int               							   NOT NULL    CONSTRAINT [DF_bx_Users_DayOnlineTime]              DEFAULT (0)

    ,[ExtendedData]           ntext            COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_ExtendedData]               DEFAULT ('')
    ,[ExtendedFieldVersion]   nchar(36)        COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_ExtendedFieldVersion]       DEFAULT ('')
    ,[UserInfo]		          varchar(800)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Users_UserInfo]                   DEFAULT ('')
--//    ,[UsedDiskSpaceSize]	  bigint										   NOT NULL    CONSTRAINT [DF_bx_Users_UsedDiskSpace]			   DEFAULT (0)

	,[KeywordVersion]		  varchar(32)	   COLLATE Chinese_PRC_CI_AS_WS	   NOT NULL    CONSTRAINT [DF_bx_Users_KeywordVersion]			   DEFAULT ('')
	
--//,[LastImpressionDate]     datetime                                         NOT NULL                                                        DEFAULT ('1980-1-1')	
	,[MobilePhone]			  bigint										   NOT NULL	   CONSTRAINT [DF_bx_Users_MobilePhone]				   DEFAULT (0)
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Users_Username] ON [bx_Users] ([Username]);
CREATE NONCLUSTERED INDEX [IX_bx_Users_Email] ON [bx_Users] ([Email]);

--CREATE INDEX [IX_bx_Users_Realname] ON [bx_Users] ([Realname]);
CREATE INDEX [IX_bx_Users_SpaceViews] ON [bx_Users] ([SpaceViews]);
CREATE INDEX [IX_bx_Users_Points] ON [bx_Users] ([Points]);


CREATE INDEX [IX_bx_Users_CreateIP] ON [bx_Users] ([CreateIP]);
CREATE INDEX [IX_bx_Users_CreateDate] ON [bx_Users] ([CreateDate]);
CREATE INDEX [IX_bx_Users_WeekOnlineTime] ON [bx_Users] ([WeekOnlineTime]);
CREATE INDEX [IX_bx_Users_DayOnlineTime] ON [bx_Users] ([DayOnlineTime]);
CREATE INDEX [IX_bx_Users_MonthOnlineTime] ON [bx_Users] ([MonthOnlineTime]);
CREATE INDEX [IX_bx_Users_WeekPosts] ON [bx_Users] ([WeekPosts]);
CREATE INDEX [IX_bx_Users_DayPosts] ON [bx_Users] ([DayPosts]);
CREATE INDEX [IX_bx_Users_MonthPosts] ON [bx_Users] ([MonthPosts]);
CREATE INDEX [IX_bx_Users_LastVisitDate] ON [bx_Users] ([LastVisitDate]);
CREATE INDEX [IX_bx_Users_LastPostDate] ON [bx_Users] ([LastPostDate]);

GO

/*
Name:用户表
Columns:    
    [Email]               邮箱
    [Doing]               状态,个性签名 (冗余字段)
    [Avatar]              头像地址    
	[Username]            用户名
    --[Password]            用户密码 
	[Realname]            真实姓名 
    [Signature]           帖子内签名 
    [FriendGroups]        好友分组   (冗余字段)

     
    [CreateIP]            注册IP
    [LastVisitIP]         最后活动IP
    
    [ExtendedFields]      扩展字段
    
    [DoingDate]           记录/心情时间
    [CreateDate]          注册时间      
    [LastPostDate]        最后回复时间
    [LastVisitDate]       最后访问时间  
    [LastAvatarUpdateDate]                           最后更新头像时间     
    
    [Point_1]             扩展积分1
    [Point_2]             扩展积分2
    [Point_3]             扩展积分3
    [Point_4]             扩展积分4
    [Point_5]             扩展积分5
    [LoginCount]          登录次数  
    [TotalTopics]         主题总数
    [TotalPosts]          帖子总数（包括主题和回复）
    [ValuedTopics]        精华帖子数
    [DeletedTopics]       被删主题数
    [DeletedReplies]      被删回复数
    [TotalBlogArticles]   用户的总日志篇数
    [TotalAlbums]         用户的总相册个数
    [TotalPhotos]         用户的总相片张数
    [UnreadMessages]      未读消息数  
    [TotalOnlineTime]     总在线时间
    [MonthOnlineTime]     本月在线时间
    [LastSystemMessageID] 系统消息最后ID
    
     [AddedAlbumSize]                                 除了基本拥有的相册容量外,附加上的相册容量,如:用积分兑换加上的容量
     [UsedAlbumSize]                                  使用了的相册容量
     [TotalBlogArticles]                              用户的总日志篇数
     [TotalAlbums]                                    用户的总相册个数
     [TotalPhotos]                                    用户的总相片张数
     [UnreadMessages]                                 用户未读短消息数
     [UnreadBoardNotifies]                            用户未读的留言通知提醒数
     [UnreadPostNotifies]                             用户未读的评论回复通知提醒数
     [UnreadGroupInviteNotifies]                      用户未读的群组邀请通知提醒数
     [UnreadFriendNotifies]                           用户未读的好友验证通知数
     [UnreadHailNotifies]                             用户未读的打招呼通知数
     [UnreadAppInviteNotifies]                        用户未读的应用邀请通知数
     [UnreadAppActionNotifies]                        用户未读的应用提醒通知数
     [UnreadBidUpNotifies]                            用户未读的积分竞价通知
     [UnreadBirthdayNotifies]                         用户未读的生日提醒通知
    
    [Gender]              性别
    --[PasswordFormat]      密码加密方式
    
    
    [BirthYear]           生日(年)
    [Birthday]            生日(月日)
    
    --[IsAvailable]         用户是否激活,或有没通过实名认证   
    
    [Points]              总积分
    
    [ExtendedFieldVersion] 扩展字段设置的版本，当版本和当前设置版本不一致时将重新检查是否已填写扩展字段必填项
*/
GO
EXEC bx_Drop 'bx_UsersInRoles';

CREATE TABLE [bx_UsersInRoles](
	[UserID]         int                 NOT NULL
    ,[RoleID]         int    NOT NULL

    ,[BeginDate]      datetime            NOT NULL
    ,[EndDate]        datetime            NOT NULL
    
    ,CONSTRAINT [PK_bx_UsersInRoles] PRIMARY KEY ([UserID],[RoleID])
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_UsersInRoles_RoleUsers] ON [bx_UsersInRoles] ([RoleID], [UserID]);
CREATE NONCLUSTERED INDEX [IX_bx_UsersInRoles_EndDate] ON [bx_UsersInRoles] ([EndDate]);
GO



GO
EXEC bx_Drop 'bx_UserTempData'

GO

CREATE TABLE [bx_UserTempData] (
	 [UserID]          int                                      NOT NULL 
	,[DataType]        tinyint	                                 NOT NULL 
	,[CreateDate]      datetime                                 NULL             CONSTRAINT [DF_bx_UserTempData_CreatDate]  DEFAULT (GETDATE())
	,[ExpiresDate]     datetime                                 NULL 
	,[Data]            ntext COLLATE Chinese_PRC_CI_AS_WS NULL 
)
GO


ALTER TABLE [bx_UserTempData] ADD 
	CONSTRAINT [PK_bx_UserTempData] PRIMARY KEY  CLUSTERED 
	(
		[UserID],
		[DataType]
	)
GO
GO
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
GO
EXEC bx_Drop 'bx_ValidateCodeActionRecords';

CREATE TABLE [bx_ValidateCodeActionRecords] (
	 [ID]               int              IDENTITY (1, 1)                 NOT NULL 
	 
    ,[IP]               varchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_ValidateCodeActionRecords_IP]            DEFAULT ('')
    ,[Action]           varchar(200)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_ValidateCodeActionRecords_Action]        DEFAULT ('')

	,[CreateDate]       datetime                                         NOT NULL    CONSTRAINT [DF_bx_ValidateCodeActionRecords_CreateDate]	DEFAULT (GETDATE())
	
    ,CONSTRAINT [PK_bx_ValidateCodeActionRecords] PRIMARY KEY ([ID])
);

/*
Name: 通知表
Columns:
    [ID]               唯一标志
    [IP]			   用户IP
    [Action]		   动作

    [CreateDate]       时间
*/

GO

EXEC bx_Drop 'IX_bx_ValidateCodeActionRecords';
CREATE  INDEX [IX_bx_ValidateCodeActionRecords] ON [bx_ValidateCodeActionRecords]([IP],[Action],[CreateDate])

GO

GO
EXEC bx_Drop 'bx_Vars';

CREATE TABLE [bx_Vars] (
	 [ID]                   int                 IDENTITY(1, 1)              NOT NULL    CONSTRAINT [PK_bx_Vars]                         PRIMARY KEY ([ID])
    ,[MaxPosts]				int                                             NOT NULL    CONSTRAINT [DF_bx_Vars_MaxPosts]                DEFAULT (0)
    ,[NewUserID]			int                                             NOT NULL    CONSTRAINT [DF_bx_Vars_NewUserID]               DEFAULT (0) 
    ,[TotalUsers]			int                                             NOT NULL    CONSTRAINT [DF_bx_Vars_TotalUsers]              DEFAULT (0)
    ,[YestodayPosts]        int												NOT NULL    CONSTRAINT [DF_bx_Vars_YestodayPosts]           DEFAULT (0)
    ,[YestodayTopics]       int												NOT NULL    CONSTRAINT [DF_bx_Vars_YestodayTopics]          DEFAULT (0)
    ,[MaxOnlineCount]       int                                             NOT NULL    CONSTRAINT [DF_bx_Vars_MaxOnlineCount]          DEFAULT (0)
    
    ,[MaxPostDate]          datetime                                        NOT NULL    CONSTRAINT [DF_bx_Vars_MaxPostDate]             DEFAULT (GETDATE())
    ,[MaxOnlineDate]        datetime                                        NOT NULL    CONSTRAINT [DF_bx_Vars_MaxOnlineDate]           DEFAULT (GETDATE())
	,[LastResetDate]        datetime                                        NOT NULL    CONSTRAINT [DF_bx_Vars_LastResetDate]           DEFAULT (GETDATE())

    ,[NewUsername]          nvarchar(50)	  COLLATE Chinese_PRC_CI_AS_WS	NOT NULL    CONSTRAINT [DF_bx_Vars_NewUsername]             DEFAULT ('')
);

/*
Name: 通知表
Columns:
    [MaxPosts]         最大发帖数
    [NewUserID]        新用户ID
    [TotalUsers]       用户总数
	[YestodayPosts]    昨日发帖数
	[YestodayTopics]   昨日主题数
    [MaxOnlineCount]   最大在线用户数
    [MaxPostDate]      最大发帖日期
    
    [MaxOnlineDate]    最大在线时间
    [LastResetDate]    最后更新昨日发帖数时间
    
    [NewUsername]      新用户 用户名
*/

GO

GO
EXEC bx_Drop 'bx_Visitors';

CREATE TABLE bx_Visitors (
    [ID]               int            IDENTITY(1,1)                   NOT NULL  
   ,[UserID]           int                                            NOT NULL
   ,[VisitorUserID]    int                                            NOT NULL
   
   ,[CreateIP]         varchar(50)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL
   
   ,[CreateDate]       datetime                                       NOT NULL    CONSTRAINT  [DF_bx_Visitors_CreateDate]    DEFAULT (GETDATE())
   
   ,CONSTRAINT [PK_bx_Visitors] PRIMARY KEY ([ID])
)

/*
Name:好友访问列表
Columns:
    [UserID]           用户ID
    [VisitorID]        访问者ID
    
    [CreateIP]         IP地址
    
    [CreateDate]       访问时间
*/

GO

--EXEC bx_Drop 'IX_bx_Visitors_UserID';
CREATE NONCLUSTERED INDEX [IX_bx_Visitors_UserID] ON [bx_Visitors]([UserID], [CreateDate])
CREATE NONCLUSTERED INDEX [IX_bx_Visitors_VisitorID] ON [bx_Visitors]([VisitorUserID], [CreateDate])
CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Visitors_Key] ON [bx_Visitors]([UserID], [VisitorUserID])

GO

GO
CREATE VIEW bx_AttacmentsWithForumID
AS
SELECT bx_Attachments.*, bx_Posts.ForumID,bx_Posts.ThreadID
FROM bx_Attachments INNER JOIN
      bx_Posts ON bx_Attachments.PostID = bx_Posts.PostID

GO
CREATE VIEW bx_AuthUsers
AS
SELECT bx_Users.*
, bx_UserVars.Password
, bx_UserVars.PasswordFormat
, bx_UserVars.UnreadMessages
, bx_UserVars.UsedAlbumSize
, bx_UserVars.AddedAlbumSize
, bx_UserVars.TimeZone
, bx_UserVars.EverAvatarChecked
, bx_UserVars.EnableDisplaySidebar
, bx_UserVars.OnlineStatus, bx_UserVars.SkinID
, bx_UserVars.LastReadSystemNotifyID
, bx_UserVars.UsedDiskSpaceSize
, bx_UserVars.TotalDiskFiles
, bx_UserVars.LastAvatarUpdateDate
, bx_UserVars.LastImpressionDate
, bx_UserVars.SelectFriendGroupID
, bx_UserVars.ReplyReturnThreadLastPage
FROM bx_Users Left JOIN
      bx_UserVars ON bx_Users.UserID = bx_UserVars.UserID
GO
CREATE VIEW bx_BanUserViewLogs
AS
SELECT     bx_BanUserLogs.LogID, bx_BanUserLogs.OperationType, bx_BanUserLogs.OperationTime, bx_BanUserLogs.OperatorName, 
                      bx_BanUserLogs.Cause, bx_BanUserLogs.UserID, bx_BanUserLogs.Username, bx_BanUserLogs.UserIP, bx_BanUserLogForumInfos.ForumID, 
                      bx_BanUserLogForumInfos.EndDate, bx_Forums.ForumName, bx_BanUserLogs.AllBanEndDate
FROM         bx_BanUserLogs LEFT OUTER JOIN
                      bx_BanUserLogForumInfos ON bx_BanUserLogs.LogID = bx_BanUserLogForumInfos.LogID LEFT OUTER JOIN
                      bx_Forums ON bx_Forums.ForumID = bx_BanUserLogForumInfos.ForumID
GO
CREATE VIEW bx_FriendsHasImpressions
AS
	SELECT * FROM bx_Friends A where FriendUserID IN (
		SELECT UserID FROM bx_UserVars B WHERE B.LastImpressionDate > '1980-1-1'
	)

GO
CREATE VIEW bx_ImpressionRecordsWithTypeInfo
AS
	SELECT A.RecordID, A.TypeID, A.UserID, A.TargetUserID, A.CreateDate, B.Text, B.KeywordVersion FROM bx_ImpressionRecords AS A LEFT JOIN bx_ImpressionTypes B ON B.TypeID = A.TypeID
GO
CREATE VIEW bx_Members
AS
SELECT bx_Users.*, bx_UserInfos.InviterID, bx_UserInfos.TotalFriends, 
      bx_UserInfos.Birthday, bx_UserInfos.BirthYear, 
      bx_UserInfos.BlogPrivacy, bx_UserInfos.FeedPrivacy, 
      bx_UserInfos.BoardPrivacy, bx_UserInfos.DoingPrivacy, 
      bx_UserInfos.AlbumPrivacy, bx_UserInfos.SpacePrivacy, 
      bx_UserInfos.SharePrivacy, bx_UserInfos.FriendListPrivacy, 
      bx_UserInfos.InformationPrivacy, bx_UserInfos.NotifySetting
FROM bx_UserInfos INNER JOIN
      bx_Users ON bx_UserInfos.UserID = bx_Users.UserID
GO
CREATE VIEW [bx_PointShowUsers]
AS
SELECT bx_PointShows.Price, bx_PointShows.Content, 
      bx_Members.*
FROM bx_PointShows INNER JOIN
      bx_Members ON bx_PointShows.UserID = bx_Members.UserID
GO
CREATE VIEW bx_SellingProps AS
SELECT A.UserID, A.UserPropID, A.SellingCount, A.SellingPrice, A.Count, A.SellingDate, B.* FROM bx_UserProps A LEFT JOIN bx_Props B ON A.PropID = B.PropID AND B.Enable = 1 WHERE A.SellingCount > 0

GO
CREATE VIEW bx_SerialCounter
AS
SELECT TOP 100 PERCENT s_a.UserID, ISNULL(s_0.S0, 0) AS Unused, ISNULL(s_1.S1, 0) 
      AS Used, ISNULL(s_3.S3, 0) AS Expiress, ISNULL(s_2.S2, 0) AS NoRegister, 
      ISNULL(s_0.S0, 0) + ISNULL(s_1.S1, 0) + ISNULL(s_2.S2, 0) + ISNULL(s_3.S3, 0) 
      AS TotalSerial
FROM (SELECT DISTINCT UserID
        FROM bx_InviteSerials) s_a FULL OUTER JOIN
          (SELECT UserID, COUNT(*) AS S3
         FROM bx_InviteSerials
         WHERE ExpiresDate <= GETDATE()
         GROUP BY UserID, Status, ExpiresDate
         HAVING Status = 3) s_3 ON s_a.UserID = s_3.UserID FULL OUTER JOIN
          (SELECT UserID, COUNT(*) AS S2
         FROM bx_InviteSerials
         GROUP BY UserID, Status
         HAVING Status = 2) s_2 ON s_a.UserID = s_2.UserID FULL OUTER JOIN
          (SELECT UserID, COUNT(*) AS S1
         FROM bx_InviteSerials
         GROUP BY UserID, Status
         HAVING Status = 1) s_1 ON s_a.UserID = s_1.UserID FULL OUTER JOIN
          (SELECT UserID, COUNT(*) AS S0
         FROM bx_InviteSerials
         GROUP BY UserID, Status
         HAVING Status = 0) s_0 ON s_a.UserID = s_0.UserID
ORDER BY s_a.UserID
GO
CREATE VIEW bx_SimpleUser 
AS
SELECT UserID, Username, Realname, Gender, AvatarSrc, Doing
FROM bx_Users
GO
CREATE VIEW bx_TopicsWithContents
AS
SELECT bx_Threads.*, bx_Posts.Content, bx_Posts.IPAddress
FROM bx_Threads INNER JOIN
      bx_Posts ON bx_Threads.ThreadID = bx_Posts.ThreadID
WHERE (bx_Posts.PostType = 1)

GO
/*还在使用的文件*/

CREATE VIEW bx_UsedFileIds
AS

	SELECT FileID FROM bx_Photos
	UNION ALL
	SELECT FileID FROM bx_Attachments
	UNION ALL
	SELECT FileID FROM bx_DiskFiles
GO
CREATE VIEW bx_UserEmoticonInfo
AS
SELECT bx_Users.UserID, bx_Users.Username, 
      SUM(bx_EmoticonGroups.TotalSizes) AS TotalSizes, 
      SUM(bx_EmoticonGroups.TotalEmoticons) AS TotalEmoticons
FROM bx_Users INNER JOIN
      bx_EmoticonGroups ON 
      bx_Users.UserID = bx_EmoticonGroups.UserID
GROUP BY bx_Users.UserID, bx_Users.Username
GO
CREATE VIEW bx_UserMissionsView AS
SELECT bx_Missions.IsEnable, bx_UserMissions.*
FROM bx_Missions INNER JOIN
      bx_UserMissions ON bx_Missions.ID = bx_UserMissions.MissionID AND ParentID IS NULL
GO
CREATE VIEW bx_UserPropsView AS
SELECT A.UserID, A.UserPropID, A.SellingCount, A.SellingPrice, A.Count, B.* FROM bx_UserProps A LEFT JOIN bx_Props B ON A.PropID = B.PropID

GO
/*没有验证的头像数据*/

CREATE VIEW bx_UserTempAvatar AS

SELECT bx_Users.UserID, bx_Users.Username, 
      bx_Users.Realname,bx_Users.Gender,
      bx_Users.AvatarSrc, 
      bx_UserTempData.Data, bx_UserTempData.CreateDate
FROM bx_Users INNER JOIN
      bx_UserTempData ON 
      bx_Users.UserID = bx_UserTempData.UserID
WHERE (bx_UserTempData.DataType = 1)

GO
CREATE VIEW bx_UserTempRealname AS
SELECT bx_UserTempData.UserID, bx_Users.Username, 
      bx_Users.Realname, bx_Users.Gender, bx_UserTempData.CreateDate, 
      bx_UserTempData.Data AS TempRealname, 
      0 AS NameChecked
FROM bx_Users INNER JOIN
      bx_UserTempData ON 
      bx_Users.UserID = bx_UserTempData.UserID
WHERE (bx_UserTempData.DataType = 0)
GO
--/*
--用户表与最近访客表的视图
--*/
--CREATE VIEW bx_UserVisitors
--AS
--SELECT u.Gender, u.Points, u.TotalFriends,u.TotalViews,v.*
--FROM dbo.bx_Users u INNER JOIN
      --dbo.bx_Visitors v ON u.ID = v.VisitorUserID 
--GO

GO
IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Emoticon_AdminDeleteEmoticons')
	DROP PROCEDURE [bx_Emoticon_AdminDeleteEmoticons];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Emoticon_AdminDeleteEmoticons
     @UserID             int 
    ,@EmoticonIDs        varchar(8000)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @BeAboutToDelete TABLE( EmoticonID int,ImageSrc varchar(256) COLLATE Chinese_PRC_CI_AS_WS);
    INSERT @BeAboutToDelete SELECT EmoticonID,ImageSrc FROM bx_Emoticons WHERE EmoticonID IN ( SELECT item FROM bx_GetIntTable(@EmoticonIDs,','))  AND GroupID IN ( SELECT GroupID FROM bx_EmoticonGroups WHERE UserID = @UserID);
    DELETE FROM bx_Emoticons WHERE EmoticonID IN ( SELECT EmoticonID FROM @BeAboutToDelete );
    SELECT  ImageSrc FROM @BeAboutToDelete WHERE ImageSrc NOT IN ( SELECT ImageSrc FROM bx_Emoticons WHERE ImageSrc IN ( SELECT ImageSrc FROM @BeAboutToDelete ));
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Emoticon_DeleteUserAllEmoticons')
	DROP PROCEDURE [bx_Emoticon_DeleteUserAllEmoticons];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Emoticon_DeleteUserAllEmoticons
    @UserID     int
AS 
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeleteTable TABLE(TempID int IDENTITY, EmoticonID int, ImageSrc varchar(255) COLLATE Chinese_PRC_CI_AS_WS);
    INSERT  @DeleteTable SELECT EmoticonID, ImageSrc FROM bx_Emoticons WHERE GroupID IN(SELECT GroupID FROM bx_EmoticonGroups WHERE UserID = @UserID)
    DELETE  bx_EmoticonGroups WHERE UserID = @UserID;
    SELECT  ImageSrc FROM @DeleteTable WHERE ImageSrc NOT IN ( SELECT ImageSrc FROM bx_Emoticons WHERE ImageSrc IN ( SELECT ImageSrc FROM @DeleteTable ));
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Emoticon_GetEmoticonsByUserID')
	DROP PROCEDURE [bx_Emoticon_GetEmoticonsByUserID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Emoticon_GetEmoticonsByUserID
    @UserID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_Emoticons WHERE GroupID IN (SELECT GroupID FROM bx_EmoticonGroups WHERE UserID = @UserID);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Emoticon_GetEmoticonsByGroupID')
	DROP PROCEDURE [bx_Emoticon_GetEmoticonsByGroupID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Emoticon_GetEmoticonsByGroupID
    @UserID int
    ,@GroupID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_Emoticons WHERE GroupID = (SELECT GroupID FROM bx_EmoticonGroups WHERE GroupID = @GroupID AND UserID = @UserID);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Emoticon_CreateEmoticonGroup')
	DROP PROCEDURE [bx_Emoticon_CreateEmoticonGroup];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Emoticon_CreateEmoticonGroup
     @UserID         int
    ,@GroupName      nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO bx_EmoticonGroups( UserID, GroupName) VALUES (@UserID, @GroupName);
    SELECT * FROM bx_EmoticonGroups WHERE GroupID = @@IDENTITY;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Emoticon_GetEmoticonGroup')
	DROP PROCEDURE [bx_Emoticon_GetEmoticonGroup];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Emoticon_GetEmoticonGroup
    @UserID int
    ,@GroupID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_EmoticonGroups WHERE GroupID = @GroupID AND UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Emoticon_DeleteEmoticonGroup')
	DROP PROCEDURE [bx_Emoticon_DeleteEmoticonGroup];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Emoticon_DeleteEmoticonGroup
 @UserID         int
,@GroupID        int
AS 
BEGIN
    SET NOCOUNT ON;
    IF EXISTS(SELECT * FROM bx_EmoticonGroups WHERE UserID = @UserID AND GroupID = @GroupID)   BEGIN
        DECLARE @DeleteTable TABLE(TempID int IDENTITY, EmoticonID int, ImageSrc varchar(255) COLLATE Chinese_PRC_CI_AS_WS);
        INSERT  @DeleteTable SELECT EmoticonID, ImageSrc FROM bx_Emoticons WHERE GroupID = @GroupID;
        DELETE  bx_EmoticonGroups WHERE GroupID = @GroupID;
        SELECT  ImageSrc FROM @DeleteTable WHERE ImageSrc NOT IN ( SELECT ImageSrc FROM bx_Emoticons WHERE ImageSrc IN ( SELECT ImageSrc FROM @DeleteTable ) );
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Emoticon_DeleteEmoticons')
	DROP PROCEDURE [bx_Emoticon_DeleteEmoticons];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Emoticon_DeleteEmoticons
 @GroupID            int
,@UserID             int
,@EmoticonIDs        varchar(8000)
AS 
BEGIN 

    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM bx_EmoticonGroups WHERE UserID = @UserID AND GroupID = @GroupID)   BEGIN
        DECLARE @DeleteTable TABLE(TempID int IDENTITY, EmoticonID int, ImageSrc varchar(255) COLLATE Chinese_PRC_CI_AS_WS);
        INSERT  @DeleteTable SELECT EmoticonID, ImageSrc FROM bx_Emoticons WHERE EmoticonID IN ( SELECT item FROM bx_GetIntTable(@EmoticonIDs,',')) AND GroupID = @GroupID;
        DELETE  bx_Emoticons WHERE EmoticonID IN ( SELECT EmoticonID FROM @DeleteTable );
        SELECT  ImageSrc FROM @DeleteTable WHERE ImageSrc NOT IN ( SELECT ImageSrc FROM bx_Emoticons WHERE ImageSrc IN ( SELECT ImageSrc FROM @DeleteTable ) );
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Emoticon_RenameGroup')
	DROP PROCEDURE [bx_Emoticon_RenameGroup];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Emoticon_RenameGroup
 @GroupID            int
,@UserID             int
,@GroupName        nvarchar(50)
AS 
BEGIN 

    SET NOCOUNT ON;

    UPDATE bx_EmoticonGroups SET GroupName = @GroupName WHERE GroupID = @GroupID AND UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Emoticon_GetGroups')
	DROP PROCEDURE [bx_Emoticon_GetGroups];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Emoticon_GetGroups
    @UserID             int
AS 
BEGIN 

    SET NOCOUNT ON;

    SELECT * FROM bx_EmoticonGroups WHERE UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_RenameEmoticonShortcut')
	DROP PROCEDURE [bx_RenameEmoticonShortcut];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_RenameEmoticonShortcut]
	@UserID INT,
	@GroupID INT,
	@EmoticonIDs  VARCHAR(8000),
	@NewShortcuts NVARCHAR(4000)--注意传进来的快捷方式不能相同
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @EmotionIDAndShortcutsTable TABLE
	(
		T_Index INT IDENTITY(1,1),
		T_EmoticonID INT DEFAULT 0,
		T_Shortcut NVARCHAR(64)  COLLATE Chinese_PRC_CI_AS_WS DEFAULT ''
	)	
	INSERT INTO @EmotionIDAndShortcutsTable(T_EmoticonID)
	SELECT item FROM bx_GetIntTable(@EmoticonIDs,N'|') 
	
	UPDATE @EmotionIDAndShortcutsTable
	SET T_Shortcut=item
	FROM bx_GetStringTable_nvarchar(@NewShortcuts,N'|')
	WHERE T_Index=id
	

	IF EXISTS(
	SELECT T_Shortcut FROM [bx_Emoticons] 
	INNER JOIN @EmotionIDAndShortcutsTable 
	ON Shortcut=T_Shortcut AND Shortcut<>''
	WHERE  EmoticonID NOT IN(SELECT T_EmoticonID FROM @EmotionIDAndShortcutsTable))
		RETURN 3

	BEGIN TRANSACTION

	UPDATE [bx_Emoticons]
	SET Shortcut= RAND()+T_EmoticonID
	FROM @EmotionIDAndShortcutsTable AS T
	WHERE EmoticonID=T_EmoticonID
	IF @@ERROR<>0
		ROLLBACK TRANSACTION
	
	UPDATE [bx_Emoticons]
	SET Shortcut=T_Shortcut
	FROM @EmotionIDAndShortcutsTable AS T
	WHERE EmoticonID=T_EmoticonID
	IF @@ERROR<>0
		ROLLBACK TRANSACTION

	COMMIT TRANSACTION
		RETURN 0
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateEmoticonsAndGroups')
	DROP PROCEDURE [bx_CreateEmoticonsAndGroups];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_CreateEmoticonsAndGroups]
	@UserID int,
    @GroupNames ntext,
	@GroupOrders text,
    @Shortcuts ntext,
    @FileNames text,
    @FileSizes text
AS
	SET NOCOUNT ON 

	DECLARE @EmoticonGroupsTable table(
		[ID1] INT IDENTITY(1,1),
		[GroupID1] int,
		[GroupName1] nvarchar(64) COLLATE Chinese_PRC_CI_AS_WS,
		[MaxSortOrder1] bigint
		);
	
	BEGIN TRANSACTION
			
	INSERT INTO @EmoticonGroupsTable(GroupName1)
	SELECT item FROM bx_GetStringTable_ntext(@GroupNames, '|');
	IF(@@error<>0)
		GOTO Cleanup

	
	INSERT INTO [bx_EmoticonGroups] ([GroupName],UserID) 
	SELECT [GroupName1],@UserID FROM @EmoticonGroupsTable WHERE [GroupName1] NOT IN (SELECT GroupName FROM [bx_EmoticonGroups] WITH(NOLOCK) WHERE UserID = @UserID )
	IF(@@error<>0)
		GOTO Cleanup
		
	UPDATE @EmoticonGroupsTable SET GroupID1=T.GroupID,
		 MaxSortOrder1=ISNULL((SELECT MAX(SortOrder) FROM [bx_Emoticons] E WHERE E.GroupID=T.GroupID),0)
		 FROM [bx_EmoticonGroups] T WHERE T.GroupName=GroupName1 AND T.UserID=@UserID
	IF(@@error<>0)
		GOTO Cleanup	

	DECLARE @EmoticonsTable table(
		[ID2] INT IDENTITY(1,1),
		[GroupOrder] int,
		[GroupID2] int,
		[Shortcut] nvarchar(64) COLLATE Chinese_PRC_CI_AS_WS,
		[ImageSrc] nvarchar(256),
		[FileSize] bigint,
		[MaxSortOrder2] bigint
		);
		
	INSERT INTO @EmoticonsTable([GroupOrder])
	SELECT item FROM bx_GetStringTable_text(@GroupOrders, '|');
	IF(@@error<>0)
		GOTO Cleanup		
		
	UPDATE @EmoticonsTable
		SET [Shortcut]=T.item
		FROM (SELECT * FROM bx_GetStringTable_ntext(@Shortcuts, '|')) T
		where T.id=ID2;
	IF(@@error<>0)
		GOTO Cleanup
				
	UPDATE @EmoticonsTable
		SET [ImageSrc]=T.item
		FROM (SELECT * FROM bx_GetStringTable_text(@FileNames, '|')) T
		where T.id=ID2;	
	IF(@@error<>0)
		GOTO Cleanup
				
	UPDATE @EmoticonsTable
		SET [FileSize]=T.item
		FROM (SELECT * FROM bx_GetStringTable_text(@FileSizes, '|')) T
		where T.id=ID2;	
	IF(@@error<>0)
		GOTO Cleanup
		    
    UPDATE @EmoticonsTable SET GroupID2=GroupID1,MaxSortOrder2=MaxSortOrder1 FROM @EmoticonGroupsTable WHERE ID1=GroupOrder
	IF(@@error<>0)
		GOTO Cleanup
	
	INSERT INTO [bx_Emoticons](
			[GroupID],
			[UserID],
			[Shortcut],
			[ImageSrc],
			[FileSize],
			[SortOrder])
		SELECT [GroupID2],
			@UserID,
			[Shortcut],
			[ImageSrc],
			[FileSize],
			[MaxSortOrder2]+ID2
		FROM @EmoticonsTable
	
			IF(@@error<>0)
				GOTO Cleanup		
		

	IF(@@error<>0)
			GOTO Cleanup

		
		COMMIT TRANSACTION
			RETURN (0)

Cleanup:
    BEGIN
    	ROLLBACK TRANSACTION
    	RETURN (-1)
    END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_MoveEmoticons')
	DROP PROCEDURE [bx_MoveEmoticons];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_MoveEmoticons]
	@GroupID int,
	@NewGroupID int,
	@UserID int,
	@EmoticonIdentities varchar(8000)
AS
	SET NOCOUNT ON
	IF (EXISTS (SELECT * FROM [bx_EmoticonGroups] WITH (NOLOCK) WHERE GroupID=@GroupID and UserID=@UserID) AND 
	    EXISTS (SELECT * FROM [bx_EmoticonGroups] WITH (NOLOCK) WHERE GroupID=@NewGroupID and UserID=@UserID))
	BEGIN
		DECLARE @MaxSortOrder int
		SELECT @MaxSortOrder=MAX(SortOrder)+1 FROM [bx_Emoticons] WITH (NOLOCK) WHERE GroupID=@NewGroupID
		IF(@MaxSortOrder IS NULL)
			SET @MaxSortOrder=0
		EXEC('Update [bx_Emoticons] SET GroupID = ' + @NewGroupID + ',SortOrder=SortOrder+'+@MaxSortOrder+' WHERE EmoticonID IN ('+ @EmoticonIdentities +') and GroupID = '+@GroupID);
		RETURN (0)
	END
	ELSE
	RETURN (-1)
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SaveUploadFile')
	DROP PROCEDURE [bx_SaveUploadFile];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SaveUploadFile
 @UserID             int
,@DirectoryID        int
,@FileName           nvarchar(256)
,@FileID             varchar(50)
,@FileSize           bigint
,@CanUseSpaceSize    bigint
,@ReplaceExistFile   int

AS
BEGIN

    SET NOCOUNT ON;

    IF @DirectoryID > 0 BEGIN

        IF NOT EXISTS (SELECT * FROM bx_DiskDirectories WHERE DirectoryID = @DirectoryID AND UserID = @UserID)
            SET @DirectoryID = 0;

    END

    IF @DirectoryID = 0 BEGIN

        SELECT @DirectoryID = DirectoryID FROM bx_DiskDirectories WHERE UserID = @UserID AND ParentID = 0;
        IF @@ROWCOUNT = 0 BEGIN
            INSERT INTO bx_DiskDirectories([ParentID], [Name], UserID) VALUES (0, '\', @UserID);
            SET @DirectoryID = @@IDENTITY;
        END

    END

    IF @ReplaceExistFile = 1 BEGIN

        DECLARE @ExistDiskFileID int;
        DECLARE @ExistFileSize bigint;

        SET @ExistDiskFileID = -1;

        SELECT @ExistDiskFileID = DiskFileID, @ExistFileSize = FileSize FROM bx_DiskFiles WHERE DirectoryID = @DirectoryID AND [FileName] = @FileName;

        IF @ExistDiskFileID <> -1 BEGIN

            DELETE bx_DiskFiles WHERE DiskFileID = @ExistDiskFileID;
            SET @CanUseSpaceSize = @CanUseSpaceSize +  @ExistFileSize - @FileSize;

        END

    END

    IF @CanUseSpaceSize > @FileSize BEGIN

        INSERT INTO bx_DiskFiles( FileID, [FileName],UserID,DirectoryID, [FileSize], ThumbPath) VALUES(@FileID ,@FileName,@UserID,@DirectoryID, @FileSize, '');
        RETURN 1;

    END
    ELSE
        RETURN 0;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetDiskFiles')
	DROP PROCEDURE [bx_GetDiskFiles];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetDiskFiles
@UserID int,
@DirectoryID int

AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @Run bit;

	IF @DirectoryID < 1 BEGIN
		SELECT @DirectoryID = DirectoryID FROM [bx_DiskDirectories] WITH (NOLOCK) WHERE UserID = @UserID AND ParentID = 0 AND Name = N'\';
		IF(@DirectoryID>0)
			SELECT @Run = 1;
		ELSE
		    SELECT @Run=0;
	END
	ELSE BEGIN
		IF EXISTS (SELECT * FROM [bx_DiskDirectories] WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND UserID = @UserID)
			SELECT @Run = 1;
		ELSE
			SELECT @Run = 0;
	END

    IF @Run = 1 BEGIN
		SELECT *
		    FROM bx_DiskDirectories WITH (NOLOCK)
		    WHERE UserID = @UserID AND ParentID = @DirectoryID
		    ORDER BY Name;

		SELECT * FROM bx_DiskFiles WITH (NOLOCK) WHERE (DirectoryID = @DirectoryID) ORDER BY [FileName];


	END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteDiskFiles')
	DROP PROCEDURE [bx_DeleteDiskFiles];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteDiskFiles 
	@UserID int,
	@DirectoryID int,
	@DiskFileIds text
AS
BEGIN

	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM [bx_DiskDirectories] WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND UserID = @UserID) BEGIN
        SELECT FileID FROM  [bx_DiskFiles] WHERE [DiskFileID] IN (SELECT item FROM bx_GetIntTable(@DiskFileIds,',')) AND [DirectoryID] = @DirectoryID;
		EXEC ('DELETE [bx_DiskFiles] WHERE [DiskFileID] IN (' + @DiskFileIds + ') AND [DirectoryID] = ' + @DirectoryID);
		RETURN (0);
	END
	ELSE
		RETURN (1);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Disk_GetDiskFileByUserAndID')
	DROP PROCEDURE [bx_Disk_GetDiskFileByUserAndID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Disk_GetDiskFileByUserAndID
	@UserID int,
	@DiskFileID int
AS
BEGIN

	SET NOCOUNT ON;

    SELECT * FROM bx_DiskFiles WHERE DiskFileID = @DiskFileID AND UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Disk_GetDiskFileByID')
	DROP PROCEDURE [bx_Disk_GetDiskFileByID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Disk_GetDiskFileByID
	@DiskFileID int
AS
BEGIN

	SET NOCOUNT ON;

    SELECT * FROM bx_DiskFiles WHERE DiskFileID = @DiskFileID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Disk_GetDiskFileByFileName')
	DROP PROCEDURE [bx_Disk_GetDiskFileByFileName];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Disk_GetDiskFileByFileName 
	@UserID int,
	@DirectoryID int,
	@FileName nvarchar(256)
AS
BEGIN

	SET NOCOUNT ON;

    IF @DirectoryID < 1
		SELECT @DirectoryID = DirectoryID FROM bx_DiskDirectories WHERE ParentID = 0 AND UserID = @UserID;

    SELECT * FROM bx_DiskFiles WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND [FileName] = @FileName AND UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_RenameDiskFile')
	DROP PROCEDURE [bx_RenameDiskFile];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_RenameDiskFile
	@UserID int,
	@DiskFileID int,
	@NewFileName nvarchar(256)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @DirectoryID int;
	DECLARE @FileName nvarchar(256);

	SELECT @DirectoryID = DirectoryID, @FileName = [FileName] FROM bx_DiskFiles WITH (NOLOCK) WHERE DiskFileID = @DiskFileID;

	IF EXISTS (SELECT * FROM bx_DiskDirectories WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND UserID = @UserID) BEGIN
		IF (@FileName = @NewFileName)
			RETURN (0);
		ELSE BEGIN
			IF EXISTS (SELECT * FROM bx_DiskDirectories WITH (NOLOCK) WHERE [UserID] = @UserID AND ParentID = @DirectoryID AND [Name] = @NewFileName)
				OR EXISTS (SELECT * FROM bx_DiskFiles WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND [FileName] = @NewFileName)
				RETURN (3);
			ELSE BEGIN
				UPDATE bx_DiskFiles SET [FileName] = @NewFileName WHERE DiskFileID = @DiskFileID;
				RETURN (0);
			END
		END
	END
	ELSE
		RETURN (-1);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateDiskDirectory')
	DROP PROCEDURE [bx_CreateDiskDirectory];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateDiskDirectory
	@UserID int,
	@ParentID int,
	@Name nvarchar(256),
	@DirectoryID int output
AS
BEGIN
	SET NOCOUNT ON;

    IF (@ParentID < 1)
		SELECT @ParentID = DirectoryID FROM bx_DiskDirectories WHERE UserID = @UserID AND ParentID = 0;
    IF (@ParentID < 1) BEGIN
		INSERT INTO [bx_DiskDirectories] (UserID, ParentID, Name) VALUES (@UserID, 0, N'\');
		SELECT @ParentID = @@Identity;
    END
	
	IF EXISTS (SELECT * FROM bx_DiskDirectories WITH (NOLOCK) WHERE UserID = @UserID AND ParentID = @ParentID AND [Name] = @Name)
		OR EXISTS (SELECT * FROM [bx_DiskFiles] WITH (NOLOCK) WHERE [DirectoryID] = @ParentID AND [FileName] = @Name)
		RETURN (2);
	ELSE BEGIN
		INSERT INTO [bx_DiskDirectories]([UserID], [ParentID], [Name]) VALUES (@UserID, @ParentID, @Name);
		SELECT @DirectoryID = @@IDENTITY;
		RETURN (1);
	END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_RenameDiskDirectory')
	DROP PROCEDURE [bx_RenameDiskDirectory];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_RenameDiskDirectory
	@UserID int,
	@DirectoryID int,
	@NewName nvarchar(256)
AS
BEGIN

	SET NOCOUNT ON;


	DECLARE @ParentID int;

	SELECT @ParentID = [ParentID] FROM bx_DiskDirectories WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND [UserID] = @UserID;
	IF @@ROWCOUNT > 0 BEGIN
		IF EXISTS (SELECT * FROM bx_DiskDirectories WITH (NOLOCK) WHERE [UserID] = @UserID AND ParentID = @ParentID AND [Name] = @NewName)
			OR EXISTS (SELECT * FROM [bx_DiskFiles] WITH (NOLOCK) WHERE [DirectoryID] = @ParentID AND [FileName] = @NewName)
			RETURN (2);
		ELSE BEGIN

			UPDATE bx_DiskDirectories SET Name = @NewName WHERE DirectoryID = @DirectoryID;
			IF @@ROWCOUNT > 0
				RETURN (1);
			ELSE
				RETURN (-1);
		END
	END
	ELSE
		RETURN (-1);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetDiskDirectoryByID')
	DROP PROCEDURE [bx_GetDiskDirectoryByID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetDiskDirectoryByID 
(
 @DirectoryID int,
 @UserID int
)
AS
BEGIN

    SET NOCOUNT ON;

	IF(@DirectoryID <= 0) BEGIN
	   SELECT * FROM [bx_DiskDirectories] WHERE ParentID = 0 AND UserID = @UserID;
	END 
	ELSE BEGIN
	   SELECT * FROM [bx_DiskDirectories] WHERE DirectoryID = @DirectoryID AND UserID = @UserID;
	END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetCurrentAndParentDirectories')
	DROP PROCEDURE [bx_GetCurrentAndParentDirectories];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetCurrentAndParentDirectories
(
    @UserID int,
    @DirectoryID int
)
AS
BEGIN

    SET NOCOUNT ON;
 
	DECLARE @Run bit;

	IF @DirectoryID < 1 BEGIN
		SELECT @DirectoryID = DirectoryID FROM [bx_DiskDirectories] WITH (NOLOCK) WHERE UserID = @UserID AND ParentID = 0 AND Name = N'\';
		IF(@DirectoryID > 0)
			SELECT @Run = 1;
	    ELSE
			SELECT @Run = 0;
	END
	ELSE BEGIN
		IF EXISTS (SELECT * FROM [bx_DiskDirectories] WITH (NOLOCK) WHERE DirectoryID = @DirectoryID AND UserID = @UserID)
			SELECT @Run = 1;
		ELSE
			SELECT @Run = 0;
	END
	
	IF(@Run = 1) BEGIN
	    DECLARE @ParentID int;
	    SET @ParentID = 1;
	    DECLARE @CurrentDirectoryID int;
	    SET @CurrentDirectoryID = @DirectoryID;

	    WHILE(@ParentID > 0) BEGIN

			SELECT @ParentID = ParentID FROM [bx_DiskDirectories] WHERE DirectoryID = @CurrentDirectoryID;
			
			SELECT *
			FROM
				bx_DiskDirectories WITH (NOLOCK)
			WHERE UserID = @UserID AND ParentID = @CurrentDirectoryID
			ORDER BY Name;

			IF(@ParentID > 0) BEGIN
				SET @CurrentDirectoryID = @ParentID;
			END
			
	    END
	END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteDiskTree')
	DROP PROCEDURE [bx_DeleteDiskTree];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteDiskTree 
@UserID int,
@DirectoryIds varchar(8000)
AS
BEGIN

    SET NOCOUNT ON;


    DECLARE @DirectoryIDTable TABLE(ID int IDENTITY (1,1) ,DirectoryID int ,ParentID int DEFAULT(0), IsSelected int DEFAULT(0));
    
	INSERT INTO @DirectoryIDTable(DirectoryID, IsSelected) 
    SELECT DirectoryID,1 FROM [bx_DiskDirectories] WITH (NOLOCK)
        WHERE [DirectoryID] IN (SELECT item FROM bx_GetIntTable(@DirectoryIds,',')) AND [UserID] = @UserID;
    
	DECLARE @Num int;
	SET @Num = 1;
    
	WHILE(@Num > 0)
	BEGIN
		INSERT INTO @DirectoryIDTable(DirectoryID)
		SELECT DirectoryID FROM [bx_DiskDirectories] WITH (NOLOCK)
		    WHERE UserID = @UserID AND ParentID IN (SELECT DirectoryID FROM @DirectoryIDTable WHERE IsSelected = 1);
	    
		UPDATE @DirectoryIDTable SET IsSelected = IsSelected + 1 WHERE IsSelected <> 2;

		SELECT @Num = COUNT(*) FROM @DirectoryIDTable WHERE IsSelected = 1;
	END


	DECLARE @ParentIds TABLE(ID_P int IDENTITY (1,1),ParentID_P int DEFAULT 0)

	INSERT INTO @ParentIds(ParentID_P)
	SELECT ParentID FROM bx_DiskDirectories WHERE DirectoryID IN (SELECT DirectoryID FROM @DirectoryIDTable)
	
	UPDATE @DirectoryIDTable SET ParentID = ParentID_P FROM @ParentIds WHERE ID = ID_P;

	DECLARE @DirectoryID int;
	SET @DirectoryID = -1;

	SELECT @DirectoryID = DirectoryID FROM @DirectoryIDTable WHERE DirectoryID NOT IN 
	    (SELECT A.DirectoryID FROM @DirectoryIDTable AS A
	        INNER JOIN @DirectoryIDTable AS B
	        ON A.DirectoryID = B.ParentID);

	BEGIN TRANSACTION
	WHILE(@DirectoryID > 0 AND EXISTS(SELECT DirectoryID FROM bx_DiskDirectories WHERE DirectoryID = @DirectoryID))
	BEGIN
		IF(@UserID > 0)
		BEGIN
			DELETE FROM bx_DiskDirectories WHERE DirectoryID = @DirectoryID AND [UserID] = @UserID;
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
			END
		END
		ELSE
		BEGIN
			DELETE FROM bx_DiskDirectories WHERE DirectoryID = @DirectoryID;
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
                RETURN -1;
			END
		END

		DELETE FROM @DirectoryIDTable WHERE DirectoryID = @DirectoryID;

		SET @DirectoryID = -1;

		SELECT @DirectoryID = DirectoryID FROM @DirectoryIDTable WHERE DirectoryID NOT IN 
		    (SELECT A.DirectoryID FROM @DirectoryIDTable AS A
		        INNER JOIN @DirectoryIDTable AS B
		        ON A.DirectoryID = B.ParentID);
	END
	COMMIT TRANSACTION
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_MoveDirectoriesAndFiles')
	DROP PROCEDURE [bx_MoveDirectoriesAndFiles];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_MoveDirectoriesAndFiles
(
 	@UserID int,--user ID
	@DirectoryID int,--the parentID of @DiskFileIDs and @DiskDirectoryIDs 
	@NewDirectoryID int,--move to the directory ID
	@DiskFileIDs text=NULL,--the file ids will be moved
	@DiskDirectoryIDs text = NULL--the directoryIDs will be moved
)
AS
BEGIN
	SET NOCOUNT ON;
    
    IF(@DirectoryID<1) BEGIN
       SELECT @DirectoryID=DirectoryID FROM [bx_DiskDirectories] WHERE UserID=@UserID AND ParentID=0;
    END
    
    IF(@NewDirectoryID<1) BEGIN
       SELECT @NewDirectoryID=DirectoryID FROM [bx_DiskDirectories] WHERE UserID=@UserID AND ParentID=0;
    END
    
    IF(EXISTS(SELECT * FROM [bx_DiskDirectories] WITH(NOLOCK) WHERE DirectoryID=@DirectoryID AND UserID=@UserID)) 
    BEGIN
        
        DECLARE @MoveFileDirectoryNames table([FileDirectoryName] nvarchar(256) COLLATE Chinese_PRC_CI_AS_WS);
        
        IF(datalength(@DiskDirectoryIDs)<> 0) BEGIN
            INSERT INTO @MoveFileDirectoryNames([FileDirectoryName])
            SELECT [Name] AS FileDirectoryName FROM [bx_DiskDirectories] WITH(NOLOCK) WHERE DirectoryID IN (SELECT item FROM bx_GetIntTable(@DiskDirectoryIDs,',')) AND ParentID=@DirectoryID AND UserID=@UserID;          
        END
        
        IF(datalength(@DiskFileIDs)<>0) BEGIN
            INSERT INTO @MoveFileDirectoryNames([FileDirectoryName])
            SELECT [FileName] AS FileDirectoryName FROM [bx_DiskFiles] WITH(NOLOCK) WHERE DiskFileID IN(SELECT item FROM bx_GetIntTable(@DiskFileIDs,',')) AND DirectoryID=@DirectoryID;
        END      
        
        IF(NOT EXISTS(SELECT * FROM [bx_DiskDirectories] WHERE ParentID=@NewDirectoryID AND UserID=@UserID AND [Name] IN (SELECT FileDirectoryName FROM @MoveFileDirectoryNames))
           AND
           NOT EXISTS(SELECT * FROM [bx_DiskFiles] WHERE DirectoryID=@NewDirectoryID AND [FileName] IN (SELECT FileDirectoryName FROM @MoveFileDirectoryNames)))
        BEGIN
            IF(datalength(@DiskFileIDs)<>0) BEGIN
			  UPDATE [bx_DiskFiles]
			  SET DirectoryID=@NewDirectoryID
			  WHERE DiskFileID IN (SELECT item FROM bx_GetIntTable(@DiskFileIDs,','))
	        END
	        
	        IF(datalength(@DiskDirectoryIDs)<> 0) BEGIN    
			  UPDATE [bx_DiskDirectories]
			  SET ParentID=@NewDirectoryID
			  WHERE DirectoryID IN (SELECT item FROM bx_GetIntTable(@DiskDirectoryIDs,','))
            END
            
            RETURN(0);            
        END 
        ELSE BEGIN 
            RETURN(-1);
        END
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetRootDiskDirectory')
	DROP PROCEDURE [bx_GetRootDiskDirectory];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetRootDiskDirectory
(
 	@UserID int
)
AS
BEGIN

	SET NOCOUNT ON;

    SELECT * FROM bx_DiskDirectories WHERE UserID = @UserID AND ParentID = 0;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_RenameDiskDirectoriesAndDiskFiles')
	DROP PROCEDURE [bx_RenameDiskDirectoriesAndDiskFiles];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROC bx_RenameDiskDirectoriesAndDiskFiles
@UserID INT,
	@ParentID INT,
	@DirectoryIDs VARCHAR(8000),
	@DiskFileIDs VARCHAR(8000),
	@DirectoryNames NVARCHAR(4000),
	@DiskFileNames NVARCHAR(4000)
AS
    SET NOCOUNT ON;

	DECLARE @T_Directories TABLE(T_Index INT IDENTITY(1,1),T_DirectoryID INT,T_DirectoryName NVARCHAR(256) COLLATE Chinese_PRC_CI_AS_WS DEFAULT '')
	DECLARE @T_DiskFiles TABLE(T_Index INT IDENTITY(1,1),T_DiskFileID INT,T_DiskFileName NVARCHAR(256) COLLATE Chinese_PRC_CI_AS_WS DEFAULT '')
	
	INSERT INTO @T_Directories(T_DirectoryID)
	SELECT item FROM bx_GetBigIntTable(@DirectoryIDs,'|')

	UPDATE @T_Directories
	SET T_DirectoryName=item
	FROM bx_GetStringTable_nvarchar(@DirectoryNames,'|')
	WHERE T_Index=id


	INSERT INTO @T_DiskFiles(T_DiskFileID)
	SELECT item FROM bx_GetIntTable(@DiskFileIDs,'|')

	UPDATE @T_DiskFiles
	SET T_DiskFileName=item
	FROM bx_GetStringTable_nvarchar(@DiskFileNames,'|')
	WHERE T_Index=id

	IF NOT EXISTS(SELECT * FROM @T_Directories AS M INNER JOIN @T_DiskFiles AS S ON M.T_DirectoryName=S.T_DiskFileName)
	BEGIN
		IF EXISTS (
		SELECT * FROM bx_DiskDirectories 
		INNER JOIN @T_Directories
		ON [Name] = T_DirectoryName
		WHERE [UserID] = @UserID AND ParentID = @ParentID AND DirectoryID NOT IN (SELECT T_DirectoryID FROM @T_Directories))
		OR EXISTS (
		SELECT * FROM [bx_DiskFiles]
		INNER JOIN @T_Directories
		ON [FileName] = T_DirectoryName
		WHERE [DirectoryID] = @ParentID AND DiskFileID NOT IN (SELECT T_DiskFileID FROM @T_DiskFiles))
		OR EXISTS (
		SELECT * FROM bx_DiskDirectories 
		INNER JOIN @T_DiskFiles
		ON [Name] = T_DiskFileName
		WHERE [UserID] = @UserID AND ParentID = @ParentID AND DirectoryID NOT IN (SELECT T_DirectoryID FROM @T_Directories))
		OR EXISTS (
		SELECT * FROM [bx_DiskFiles]
		INNER JOIN @T_DiskFiles
		ON [FileName] = T_DiskFileName
		WHERE [DirectoryID] = @ParentID AND DiskFileID NOT IN (SELECT T_DiskFileID FROM @T_DiskFiles))
		RETURN 3
		BEGIN TRANSACTION
		SELECT * FROM @T_Directories
		UPDATE bx_DiskDirectories 
		SET [Name] = RAND()+T_DirectoryID--临时插入
		FROM @T_Directories
		WHERE DirectoryID = T_DirectoryID
		IF @@ERROR<>0
		BEGIN
			ROLLBACK TRANSACTION
			RETURN -1
		END

		UPDATE bx_DiskDirectories 
		SET [Name] = T_DirectoryName
		FROM @T_Directories
		WHERE DirectoryID = T_DirectoryID
		IF @@ERROR<>0
		BEGIN
			ROLLBACK TRANSACTION
			RETURN -1
		END
		UPDATE bx_DiskFiles 
		SET [FileName] = RAND()+T_DiskFileID--临时插入
		FROM @T_DiskFiles
		WHERE DiskFileID = T_DiskFileID		
		IF @@ERROR<>0
		BEGIN
			ROLLBACK TRANSACTION
			RETURN -1
		END

		UPDATE bx_DiskFiles 
		SET [FileName] = T_DiskFileName
		FROM @T_DiskFiles
		WHERE DiskFileID = T_DiskFileID	
		IF @@ERROR<>0
		BEGIN
			ROLLBACK TRANSACTION
			RETURN -1
		END

		COMMIT TRANSACTION
		RETURN 0
	END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetFriendGroups')
	DROP PROCEDURE [bx_GetFriendGroups];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetFriendGroups
    @UserID int
AS BEGIN

	SET NOCOUNT ON;

    SELECT * FROM [bx_FriendGroups] WITH (NOLOCK) WHERE UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AddFriendGroup')
	DROP PROCEDURE [bx_AddFriendGroup];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AddFriendGroup
    @UserID int,
    @GroupID int,
    @GroupName nvarchar(50),
    @MaxGroupCount int
AS BEGIN

	SET NOCOUNT ON;
    
    IF EXISTS ( SELECT * FROM bx_FriendGroups WHERE UserID = @UserID AND GroupName = @GroupName )
        RETURN(2);

    IF (SELECT COUNT(*) FROM bx_FriendGroups WHERE UserID = @UserID) > @MaxGroupCount
        RETURN(3);

IF @GroupID IS NULL BEGIN
    INSERT INTO bx_FriendGroups ( UserID, GroupName ) VALUES ( @UserID, @GroupName );
    SELECT * FROM bx_FriendGroups WHERE GroupID = @@IDENTITY;
END
ELSE BEGIN 
    INSERT INTO bx_FriendGroups (GroupID, UserID, GroupName ) VALUES (@GroupID, @UserID, @GroupName );
    SELECT * FROM bx_FriendGroups WHERE GroupID = @GroupID;
END
    RETURN (0);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteFriendGroup')
	DROP PROCEDURE [bx_DeleteFriendGroup];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteFriendGroup
    @UserID int,
    @GroupID int,
    @DeleteFriends bit
AS BEGIN

	SET NOCOUNT ON;

    IF @GroupID = 0
        RETURN (2);

    IF @DeleteFriends = 0
        UPDATE bx_Friends SET GroupID = 0 WHERE UserID = @UserID AND GroupID = @GroupID;

    DELETE bx_FriendGroups WHERE UserID = @UserID AND GroupID = @GroupID;

    RETURN (0);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_RenameFriendGroup')
	DROP PROCEDURE [bx_RenameFriendGroup];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_RenameFriendGroup
    @UserID int,
    @GroupID int,
    @NewGroupName nvarchar(50)
AS BEGIN

	SET NOCOUNT ON;

    IF @GroupID = 0
        RETURN (2);

    ELSE IF EXISTS (SELECT * FROM bx_FriendGroups WHERE UserID = @UserID AND GroupName = @NewGroupName AND GroupID <> @GroupID)
        RETURN (3);

    ELSE BEGIN
        UPDATE bx_FriendGroups SET GroupName = @NewGroupName WHERE UserID = @UserID AND GroupID = @GroupID;
        RETURN (0);
    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ShieldFriendGroup')
	DROP PROCEDURE [bx_ShieldFriendGroup];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ShieldFriendGroup
    @UserID int,
    @GroupID int,
    @IsShield bit
AS BEGIN

	SET NOCOUNT ON;

    IF @GroupID = 0
        RETURN (2);

    UPDATE bx_FriendGroups SET IsShield = @IsShield WHERE UserID = @UserID AND GroupID = @GroupID;

    IF @@ROWCOUNT = 1
        RETURN (0);
    ELSE
        RETURN (3);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetFriendsAndBlacklist')
	DROP PROCEDURE [bx_GetFriendsAndBlacklist];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetFriendsAndBlacklist
    @UserID int
AS BEGIN

	SET NOCOUNT ON;

    SELECT * FROM [bx_Friends] WITH (NOLOCK) WHERE UserID = @UserID ORDER BY [Hot] DESC,[CreateDate] ASC;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_MoveFriend')
	DROP PROCEDURE [bx_MoveFriend];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_MoveFriend
    @UserID int,
    @FriendUserID int,
    @FriendGroupID int
AS BEGIN

	SET NOCOUNT ON;

    UPDATE [bx_Friends] SET GroupID = @FriendGroupID WHERE UserID = @UserID AND FriendUserID = @FriendUserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteFriend')
	DROP PROCEDURE [bx_DeleteFriend];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteFriend
    @UserID int,
    @FriendUserID int
AS BEGIN

	SET NOCOUNT ON;

    DELETE FROM [bx_Friends] WHERE UserID = @UserID AND FriendUserID = @FriendUserID;
    DELETE FROM [bx_Friends] WHERE UserID = @FriendUserID AND FriendUserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AcceptAddFriend')
	DROP PROCEDURE [bx_AcceptAddFriend];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AcceptAddFriend
    @UserID int,
    @FriendUserID int,
    @FromFriendGroupID int,
    @FriendGroupID int
AS BEGIN

SET NOCOUNT ON;

UPDATE bx_Friends SET GroupID = @FriendGroupID WHERE UserID = @UserID AND FriendUserID = @FriendUserID

IF @@RowCount=0 BEGIN
   INSERT INTO [bx_Friends] (Hot,UserID,FriendUserID,GroupID) VALUES (0, @UserID, @FriendUserID, @FriendGroupID); 
END

UPDATE bx_Friends SET GroupID = @FromFriendGroupID WHERE UserID = @FriendUserID AND FriendUserID = @UserID

IF @@RowCount=0 BEGIN
  INSERT INTO [bx_Friends] (Hot,UserID,FriendUserID,GroupID) VALUES (0, @FriendUserID, @UserID,@FromFriendGroupID );
END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateFriendHot')
	DROP PROCEDURE [bx_UpdateFriendHot];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateFriendHot
    @UserID int,
    @FriendUserID int,
    @Hot int
AS BEGIN

	SET NOCOUNT ON;

    UPDATE [bx_Friends] SET Hot = Hot + @Hot WHERE UserID = @UserID AND FriendUserID = @FriendUserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AddUsersToBlacklist')
	DROP PROCEDURE [bx_AddUsersToBlacklist];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AddUsersToBlacklist
     @UserID int
    ,@UserIdsToAdd text
AS
BEGIN


    SET NOCOUNT ON;

    DECLARE @AddTable table(TargetUserID int, IsUpdate bit DEFAULT(0));

    INSERT INTO @AddTable (TargetUserID) SELECT item FROM bx_GetIntTable(@UserIdsToAdd,',') AS T WHERE T.item != @UserID;

    UPDATE @AddTable SET IsUpdate = 1 WHERE TargetUserID IN ( SELECT FriendUserID FROM bx_Friends WHERE UserID = @UserID);

    UPDATE bx_Friends SET GroupID = -1 WHERE UserID = @UserID AND FriendUserID IN ( SELECT TargetUserID FROM @AddTable WHERE IsUpdate = 1 );

    INSERT INTO bx_Friends ( UserID, FriendUserID, GroupID, Hot ) SELECT @UserID, TargetUserID, -1, 0 FROM @AddTable T WHERE T.IsUpdate = 0;

    DELETE bx_Friends WHERE FriendUserID = @UserID AND GroupID != -1 AND UserID in(SELECT TargetUserID FROM @AddTable);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Impression_GetLastRecord')
	DROP PROCEDURE [bx_Impression_GetLastRecord];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Impression_GetLastRecord
    @UserID        int,
    @TargetUserID  int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 *, B.Text, B.KeywordVersion FROM bx_ImpressionRecords A LEFT JOIN bx_ImpressionTypes B ON B.TypeID = A.TypeID WHERE UserID = @UserID AND TargetUserID = @TargetUserID ORDER BY RecordID DESC;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Impression_Create')
	DROP PROCEDURE [bx_Impression_Create];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Impression_Create
    @UserID        int,
    @TargetUserID  int,
    @Text          nvarchar(100),
    @TimeLimit     int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now AS datetime;
    DECLARE @Ago AS datetime;

    SET @Now = GETDATE();
    SET @Ago = DATEADD(hh,-@TimeLimit,GETDATE());

    IF NOT EXISTS(SELECT * FROM bx_ImpressionRecords WHERE UserID = @UserID AND TargetUserID = @TargetUserID AND CreateDate >= @Ago AND CreateDate <= @Now)
    BEGIN
        DECLARE @TypeID AS int;

        SELECT @TypeID = TypeID FROM bx_ImpressionTypes WHERE Text = @Text;

        IF @TypeID IS NULL
        BEGIN
            INSERT INTO bx_ImpressionTypes (Text) VALUES (@Text);

            SELECT @TypeID = @@IDENTITY;
        END

        INSERT INTO bx_ImpressionRecords (UserID, TargetUserID, TypeID) VALUES (@UserID, @TargetUserID, @TypeID);

        IF NOT EXISTS(SELECT * FROM bx_Impressions WHERE UserID = @TargetUserID AND TypeID = @TypeID)
        BEGIN
            INSERT INTO bx_Impressions (UserID, TypeID) VALUES (@TargetUserID, @TypeID);
        END

        SELECT 1;
    END
    ELSE
    BEGIN
        SELECT 0;
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Impression_DeleteByType')
	DROP PROCEDURE [bx_Impression_DeleteByType];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Impression_DeleteByType
    @UserID  int,
    @TypeID  int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM bx_Impressions WHERE UserID = @UserID AND TypeID = @TypeID;

    DELETE FROM bx_ImpressionRecords WHERE TargetUserID = @UserID AND TypeID = @TypeID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Impression_UpdateImpressionRecordKeywords')
	DROP PROCEDURE [bx_Impression_UpdateImpressionRecordKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Impression_UpdateImpressionRecordKeywords
    @TypeID                int,
    @KeywordVersion        varchar(32),
    @Text                  nvarchar(100),
    @TextReverter          nvarchar(1000)
AS BEGIN


    SET NOCOUNT ON;

    IF @Text IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL
            UPDATE bx_ImpressionTypes SET Text = @Text, KeywordVersion = @KeywordVersion WHERE TypeID = @TypeID;
        ELSE
            UPDATE bx_ImpressionTypes SET Text = @Text WHERE TypeID = @TypeID;

    END

    IF @TextReverter IS NOT NULL BEGIN

        UPDATE bx_ImpressionTypeReverters SET TextReverter = @TextReverter WHERE TypeID = @TypeID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_ImpressionTypeReverters (TypeID, TextReverter) VALUES (@TypeID, @TextReverter);

    END


END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAllBannedUsers')
	DROP PROCEDURE [bx_GetAllBannedUsers];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAllBannedUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM bx_BannedUsers WHERE EndDate > GETDATE() ORDER BY ForumID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CancelBanByForumID')
	DROP PROCEDURE [bx_CancelBanByForumID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CancelBanByForumID
 @UserIDs          varchar(8000)
,@ForumID          int
,@OperatorName    nvarchar(50)
,@OperationType    varchar(50)
,@UserIP            varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM bx_BannedUsers WHERE UserID IN( SELECT item FROM  bx_GetIntTable(@UserIDs,',')) AND ForumID = @ForumID;
    INSERT INTO bx_BanUserLogs(UserID,Username,UserIP,OperatorName,OperationType) SELECT item,Username,@UserIP,@OperatorName,@OperationType FROM  bx_GetIntTable(@UserIDs,',') LEFT JOIN bx_Users ON item=UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CancelBan')
	DROP PROCEDURE [bx_CancelBan];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CancelBan
 @UserIDs           varchar(8000)
,@OperatorName     nvarchar(50)
,@OperationType     varchar(50)
,@UserIP            varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM bx_BannedUsers WHERE UserID IN(SELECT item FROM bx_GetIntTable(@UserIDs,','));
    INSERT INTO bx_BanUserLogs(UserID,Username,UserIP,OperatorName,OperationType) SELECT item,Username,@UserIP,@OperatorName,@OperationType FROM  bx_GetIntTable(@UserIDs,',') LEFT JOIN bx_Users ON item=UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_BanUsersWholeForum')
	DROP PROCEDURE [bx_BanUsersWholeForum];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_BanUsersWholeForum
 @UserIDs            varchar(8000)
,@OperatorName      nvarchar(50)
,@UserIP            varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM bx_BannedUsers WHERE UserID IN(SELECT item FROM bx_GetIntTable(@UserIDs,','));
    INSERT INTO bx_BannedUsers(UserID,ForumID) SELECT item,0 FROM bx_GetIntTable(@UserIDs,',') INNER JOIN bx_Users ON item=UserID;
    INSERT INTO bx_BanUserLogs(UserID,Username,UserIP,OperatorName,OperationType) SELECT item,Username,@UserIP,@OperatorName,2 FROM bx_GetIntTable(@UserIDs,',') INNER JOIN bx_Users ON item=UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Doing_GetTodayPostDoingCount')
	DROP PROCEDURE [bx_Doing_GetTodayPostDoingCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Doing_GetTodayPostDoingCount
	@UserID		int,
	@Today  	datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_Doings WHERE [UserID] = @UserID AND [CreateDate] >= @Today;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Doing_GetPostDoingCount')
	DROP PROCEDURE [bx_Doing_GetPostDoingCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Doing_GetPostDoingCount
	@UserID		int,
	@BeginDate	datetime,
	@EndDate	datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_Doings WHERE [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Doing_GetDoing')
	DROP PROCEDURE [bx_Doing_GetDoing];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Doing_GetDoing
	@DoingID	int
AS
BEGIN
    SET NOCOUNT ON;
	SELECT * FROM bx_Doings WHERE DoingID = @DoingID
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Doing_Add')
	DROP PROCEDURE [bx_Doing_Add];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Doing_Add 
    @UserID       int,
    @Content      nvarchar(200),
    @CreateIP     varchar(50)
AS BEGIN
    SET NOCOUNT ON;

    IF LEN(@Content)>0 BEGIN
        INSERT INTO bx_Doings([UserID],[TotalComments],[Content],[CreateIP]) VALUES(@UserID,0,@Content,@CreateIP);
        SELECT @@IDENTITY;
    END 
    ELSE BEGIN
        SELECT 0;
    END
    UPDATE bx_Users SET Doing = @Content, DoingDate = getdate() WHERE UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Doing_DeleteDoing')
	DROP PROCEDURE [bx_Doing_DeleteDoing];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Doing_DeleteDoing 
    @DoingID    int
AS BEGIN
    SET NOCOUNT ON;

    DELETE FROM bx_Doings WHERE DoingID = @DoingID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Doing_UpdateDoingKeywords')
	DROP PROCEDURE [bx_Doing_UpdateDoingKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Doing_UpdateDoingKeywords
    @DoingID                  int,
    @KeywordVersion           varchar(32),
    @Content                  nvarchar(200),
    @ContentReverter          nvarchar(4000)
AS BEGIN


    SET NOCOUNT ON;

    IF @Content IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL
            UPDATE bx_Doings SET Content = @Content, KeywordVersion = @KeywordVersion WHERE DoingID = @DoingID;
        ELSE
            UPDATE bx_Doings SET Content = @Content WHERE DoingID = @DoingID;

    END

    IF @ContentReverter IS NOT NULL BEGIN

        UPDATE bx_DoingReverters SET ContentReverter = @ContentReverter WHERE DoingID = @DoingID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_DoingReverters (DoingID, ContentReverter) VALUES (@DoingID, @ContentReverter);

    END


END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SaveAdvert')
	DROP PROCEDURE [bx_SaveAdvert];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SaveAdvert 
 @ADID            int
,@CategoryID      int
,@Position        tinyint
,@Index           int
,@AdType          tinyint
,@Available       bit
,@Title           nvarchar(50)
,@Href            nvarchar(500)
,@Text            nvarchar(200)
,@FontSize        int
,@Color           varchar(50)
,@Src             nvarchar(500)
,@Width           int
,@Height          int
,@BeginDate       datetime
,@EndDate         datetime
,@Targets         ntext
,@Code            ntext
,@Floor           varchar(1000)  
AS
BEGIN
SET NOCOUNT ON;

    IF @ADID IS NULL OR @ADID = 0 BEGIN
        INSERT INTO bx_Adverts( CategoryID, [Index], Position, Title, Href, [Text], Color, FontSize, ADType, Available, ResourceHref, Width, Height, BeginDate, EndDate, Code, Targets,Floor ) VALUES( @CategoryID, @Index, @Position, @Title, @Href, @Text, @Color, @FontSize, @AdType, @Available, @Src, @Width, @Height, @BeginDate, @EndDate, @Code, @Targets, @Floor );
        SET @ADID = (SELECT @@IDENTITY)
    END
    ELSE BEGIN
        UPDATE bx_Adverts SET CategoryID = @CategoryID, [Index] = @Index, Position = @Position, ADType = @AdType, Title = @Title, [Text] = @Text, Available = @Available, Href = @Href, ResourceHref = @Src, FontSize = @FontSize, Color = @Color, Width = @Width, Height = @Height, BeginDate = @BeginDate, EndDate = @EndDate, Code = @Code, Targets = @Targets, Floor = @Floor  WHERE ADID = @ADID;       
    END
    
    SELECT * FROM bx_Adverts WHERE [ADID] = @ADID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetJobExecuteTime')
	DROP PROCEDURE [bx_SetJobExecuteTime];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetJobExecuteTime
     @Type        varchar(200)
    ,@ExecuteTime dateTime
AS
BEGIN
	SET NOCOUNT ON;
    UPDATE [bx_JobStatus] SET [LastExecuteTime] = @ExecuteTime WHERE [Type] = @Type;

    IF @@ROWCOUNT < 1
        INSERT INTO [bx_JobStatus] ([Type], [LastExecuteTime]) VALUES (@Type, @ExecuteTime);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAllJobStatus')
	DROP PROCEDURE [bx_GetAllJobStatus];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAllJobStatus
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_JobStatus];
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_QueryForBeforeRequestIn3M')
	DROP PROCEDURE [bx_QueryForBeforeRequestIn3M];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_QueryForBeforeRequestIn3M
AS
BEGIN

    SET NOCOUNT ON;
	
    SELECT * FROM [bx_TopicStatus] WHERE EndDate <= GETDATE();






	SELECT Q.*, T.PostUserID,T.ForumID FROM [bx_Questions] Q INNER JOIN [bx_Threads] T ON Q.ThreadID = T.ThreadID WHERE Q.IsClosed = 0 AND Q.ExpiresDate < getdate();

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AutoFinalQuestion')
	DROP PROCEDURE [bx_AutoFinalQuestion];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AutoFinalQuestion
	@ThreadID int, 
	@UserID int,
	@RewardCount int,
	@TotalReward int
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @Reward int,@PostCount int
	SELECT @PostCount = COUNT(*) FROM [bx_Posts] WITH (NOLOCK) WHERE ThreadID = @ThreadID AND SortOrder < 4000000000000000 AND UserID <> @UserID;
	
	IF(@PostCount = 0)
	BEGIN
		UPDATE [bx_Questions] SET IsClosed = 1 WHERE ThreadID = @ThreadID
	END
	ELSE BEGIN
        IF EXISTS(SELECT * FROM [bx_Questions] WHERE IsClosed = 1 AND ThreadID = @ThreadID)
            RETURN;
		IF(@PostCount < @RewardCount)
			SET @RewardCount = @PostCount;

		SET @Reward = @TotalReward / @RewardCount;
		
		BEGIN TRANSACTION
		EXEC('INSERT [bx_QuestionRewards](PostID,ThreadID,Reward) SELECT TOP '+@RewardCount+' PostID,'+@ThreadID+','+@Reward+' FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID='+@ThreadID+' AND SortOrder < 4000000000000000 AND UserID <> ' + @UserID + ' ORDER BY PostID')
		IF(@@ERROR <> 0) BEGIN
		   ROLLBACK TRANSACTION
		   RETURN
		END
		
		UPDATE [bx_QuestionRewards] SET Reward = Reward + @TotalReward % @RewardCount WHERE PostID = (SELECT MIN(PostID) FROM [bx_QuestionRewards] WHERE ThreadID = @ThreadID)
		IF(@@ERROR <> 0) BEGIN
		   ROLLBACK TRANSACTION
		   RETURN
		END
		
		UPDATE [bx_Questions] SET IsClosed = 1 WHERE ThreadID = @ThreadID
		IF(@@ERROR <> 0) BEGIN
		   ROLLBACK TRANSACTION
		   RETURN
		END
		
		COMMIT TRANSACTION
		
		SELECT UserID, SUM(Q.Reward) AS Reward FROM bx_Posts P INNER JOIN bx_QuestionRewards Q ON P.PostID = Q.PostID
			WHERE P.ThreadID = @ThreadID GROUP BY UserID;
	END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateMission')
	DROP PROCEDURE [bx_CreateMission];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateMission
     @CycleTime         int
    ,@SortOrder         int

    ,@Type              nvarchar(200)             
    ,@Name              nvarchar(100)
    ,@IconUrl           nvarchar(200)
    ,@DeductPoint       nvarchar(100)

    ,@Prize             ntext
    ,@Description       ntext
    ,@ApplyCondition    ntext
    ,@FinishCondition   ntext

    ,@EndDate           DateTime
    ,@BeginDate         DateTime
    ,@IsEnable          bit

    ,@CategoryID        int
    ,@ParentID          int
AS
BEGIN

	SET NOCOUNT ON;
    INSERT INTO [bx_Missions](
             [CycleTime]
            ,[SortOrder]

            ,[Type]
            ,[Name]
            ,[IconUrl]
            ,[DeductPoint]

            ,[Prize]
            ,[Description]
            ,[ApplyCondition]
            ,[FinishCondition]

            ,[EndDate]
            ,[BeginDate]
            ,[IsEnable]

            ,[CategoryID]
            ,[ParentID]
            ) VALUES (
             @CycleTime
            ,@SortOrder

            ,@Type
            ,@Name
            ,@IconUrl
            ,@DeductPoint

            ,@Prize
            ,@Description
            ,@ApplyCondition
            ,@FinishCondition

            ,@EndDate
            ,@BeginDate
            ,@IsEnable

            ,@CategoryID
            ,@ParentID
            );

    SELECT @@IDENTITY;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateMission')
	DROP PROCEDURE [bx_UpdateMission];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateMission
     @ID                int
    ,@CycleTime         int
    ,@SortOrder         int
           
    ,@Name              nvarchar(100)
    ,@IconUrl           nvarchar(200)
    ,@DeductPoint       nvarchar(100)

    ,@Prize             ntext
    ,@Description       ntext
    ,@ApplyCondition    ntext
    ,@FinishCondition   ntext

    ,@EndDate           DateTime
    ,@BeginDate         DateTime
    ,@IsEnable          bit

    ,@CategoryID        int
AS
BEGIN

	SET NOCOUNT ON;
    UPDATE [bx_Missions] SET
             [CycleTime] = @CycleTime
            ,[SortOrder] = @SortOrder

            ,[Name] = @Name
            ,[IconUrl] = @IconUrl
            ,[DeductPoint] = @DeductPoint

            ,[Prize] = @Prize
            ,[Description] = @Description
            ,[ApplyCondition] = @ApplyCondition
            ,[FinishCondition] = @FinishCondition

            ,[EndDate] = @EndDate
            ,[BeginDate] = @BeginDate
            ,[IsEnable] = @IsEnable
            ,[CategoryID] = @CategoryID
    WHERE 
            [ID]=@ID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAllMissions')
	DROP PROCEDURE [bx_GetAllMissions];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAllMissions
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_Missions] ORDER BY [SortOrder] ASC,[ID] DESC;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ApplyMission')
	DROP PROCEDURE [bx_ApplyMission];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ApplyMission
     @UserID       int
    ,@MissionID    int
    ,@Percent      float
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @CycleTime int;
    SELECT @CycleTime = [CycleTime] FROM [bx_Missions] WHERE [ID] = @MissionID AND getdate() > [BeginDate] AND getdate() < [EndDate] AND IsEnable = 1;

    IF @CycleTime IS NOT NULL BEGIN
        DECLARE @Status tinyint,@IsPrized bit;
        IF @Percent = 1 BEGIN
            SET @Status = 1;
            SET @IsPrized = 1;
        END
        ELSE BEGIN 
            SET @Status = 0;
            SET @IsPrized = 0;
        END

        IF NOT EXISTS (SELECT * FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID) BEGIN
            INSERT INTO [bx_UserMissions](
                         [UserID]
                        ,[MissionID]
                        ,[Status]
                        ,[FinishPercent]
                        ,[IsPrized]
                        ) VALUES (
                         @UserID
                        ,@MissionID
                        ,@Status
                        ,@Percent
                        ,@IsPrized
                        );
            UPDATE [bx_Missions] SET [TotalUsers] = [TotalUsers] + 1 WHERE [ID] = @MissionID
            RETURN 0;
        END
        ELSE IF @CycleTime = 0  BEGIN--不是周期性任务
			DECLARE @TempStatus tinyint;
            SELECT @TempStatus = [Status]
				 FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
				 
			IF @TempStatus = 3 BEGIN--放弃的 可以重新申请
				UPDATE [bx_UserMissions] SET 
                          [Status] = @Status
                        , [FinishPercent] = @Percent
                        , [IsPrized] = @IsPrized
                        , [FinishDate] = getdate()
                        , [CreateDate] = getdate()
                        WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
				UPDATE [bx_Missions] SET [TotalUsers] = [TotalUsers] + 1 WHERE [ID] = @MissionID
				RETURN 0;
			END	
			ELSE
				RETURN 2; --已经申请过  
		END
        ELSE BEGIN --周期性任务
            DECLARE @TempIsPrized bit,@TempCreateDate datetime,@TempFinishPercent float;
            SELECT @TempStatus = [Status]
				 , @TempFinishPercent = [FinishPercent]
				 , @TempIsPrized = [IsPrized]
				 , @TempCreateDate = [CreateDate] 
				 FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
            
			IF @TempStatus<>3 AND DATEADD(second,@CycleTime,@TempCreateDate)>getdate() --下次申请时间未到
				RETURN 2; --已经申请过  

            DECLARE @ReturnValue int,@UpdateUserMission bit
            SET @ReturnValue = 0;
            SET @UpdateUserMission = 0;

            IF @TempIsPrized = 1 BEGIN
                SET @ReturnValue = 0;
                SET @UpdateUserMission = 1;
            END
            ELSE IF @TempFinishPercent = 1 BEGIN --已经完成 未领取奖励
                SET @UpdateUserMission = 1;
                SET @ReturnValue = 3;
            END
            ELSE IF @TempStatus = 0
                RETURN 2;--已经申请过
            ELSE IF @TempStatus > 1 BEGIN --放弃 或者 失败的任务  重新开始
                SET @UpdateUserMission = 1;
                SET @ReturnValue = 0;
            END
            
            IF @UpdateUserMission = 1 BEGIN
                UPDATE [bx_UserMissions] SET 
                          [Status] = @Status
                        , [FinishPercent] = @Percent
                        , [IsPrized] = @IsPrized
                        , [FinishDate] = getdate()
                        , [CreateDate] = getdate()
                        WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
				UPDATE [bx_Missions] SET [TotalUsers] = [TotalUsers] + 1 WHERE [ID] = @MissionID
            END
            RETURN @ReturnValue;
        END
    END
    ELSE
        RETURN 1;--失败 
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserMission')
	DROP PROCEDURE [bx_GetUserMission];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserMission
     @UserID       int
    ,@MissionID    int
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetUserMissionIsPrized')
	DROP PROCEDURE [bx_SetUserMissionIsPrized];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetUserMissionIsPrized
     @UserID       int
    ,@MissionID    int
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @FinishPercent float;
    IF EXISTS(SELECT * FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID AND [IsPrized] = 1)
        RETURN 3; --已经奖励过
    SELECT @FinishPercent = [FinishPercent] FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [MissionID] = @MissionID;
    IF @FinishPercent IS NULL
        RETURN 1; -- 任务不存在
    ELSE IF @FinishPercent < 1
        RETURN 2; -- 任务未完成

    UPDATE [bx_UserMissions] SET [Status] = 1, [IsPrized] = 1, [FinishDate] = getdate() WHERE [UserID] = @UserID AND [MissionID] = @MissionID;

    RETURN 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateUserMissionStatus')
	DROP PROCEDURE [bx_UpdateUserMissionStatus];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateUserMissionStatus
     @UserID       int
    ,@MissionIDs   varchar(8000)
    ,@Status       tinyint
AS
BEGIN
	SET NOCOUNT ON;
    IF CHARINDEX(',',@MissionIDs) = -1
        UPDATE [bx_UserMissions] SET [Status] = @Status WHERE [UserID] = @UserID AND [MissionID] = @MissionIDs;
    ELSE BEGIN
		DECLARE @sql varchar(200);
		SET @sql = 'UPDATE [bx_UserMissions] SET [Status] = ' + CAST(@Status as varchar(5)) + ' WHERE [UserID] = ' + CAST(@UserID as varchar(10)) + ' AND [MissionID] IN('+@MissionIDs+')';
        EXEC(@sql);
	END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AbandonMission')
	DROP PROCEDURE [bx_AbandonMission];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AbandonMission
     @UserID       int
    ,@MissionID    int
AS
BEGIN
	SET NOCOUNT ON;

    DELETE FROM bx_UserMissions WHERE UserID=@UserID AND MissionID = @MissionID;

    DELETE FROM bx_UserMissions WHERE UserID=@UserID AND MissionID IN(SELECT MissionID FROM bx_Missions WHERE ParentID=@MissionID);

    RETURN 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateUserMissionFinishPercents')
	DROP PROCEDURE [bx_UpdateUserMissionFinishPercents];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateUserMissionFinishPercents
     @UserID             int
    ,@MissionIDs         varchar(8000)
    ,@Percents           varchar(8000)
AS
BEGIN
	SET NOCOUNT ON;
    
    IF CHARINDEX(',',@MissionIDs) = -1 BEGIN
        UPDATE [bx_UserMissions] SET [FinishPercent] = @Percents WHERE [UserID] = @UserID AND [MissionID] = @MissionIDs;
    END
    ELSE BEGIN
        DECLARE @MissionPercents table([MID] int IDENTITY(1, 1), [MissionID] int, [Percent] float);
        INSERT INTO @MissionPercents (MissionID) SELECT item FROM bx_GetIntTable(@MissionIDs, ',');
        UPDATE @MissionPercents SET [Percent] = P.[item] FROM bx_GetStringTable_varchar(@Percents, ',') P WHERE P.[id] = [MID];

        UPDATE [bx_UserMissions] SET 
                 [FinishPercent] = ISNULL(M.[Percent],0)  
        FROM @MissionPercents M 
        WHERE [UserID] = @UserID AND [bx_UserMissions].[MissionID] = M.[MissionID];
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetNewUserMission')
	DROP PROCEDURE [bx_GetNewUserMission];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetNewUserMission
     @UserID int
AS BEGIN
	SET NOCOUNT ON;
    
    SELECT TOP 1 * FROM [bx_Missions] WHERE
    getdate() > [BeginDate] AND getdate() < [EndDate] AND [IsEnable] = 1 
    AND
    (
            ([ID] NOT IN (SELECT [MissionID] FROM [bx_UserMissions] WHERE [UserID] = @UserID)
        OR
            ([CycleTime] > 0 AND [ID] IN(
                SELECT [MissionID] FROM [bx_UserMissions] WHERE [UserID] = @UserID AND [FinishPercent]=1 AND [Status]<>3 AND [CreateDate]<DATEADD(second,0-[CycleTime],getdate())
                )
            ))
    ) 
    ORDER BY [SortOrder] ASC,[ID] DESC;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserMissionCount')
	DROP PROCEDURE [bx_GetUserMissionCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserMissionCount
     @UserID   int
    ,@Status   tinyint
AS BEGIN
	SET NOCOUNT ON;
    
    SELECT COUNT(*) FROM bx_UserMissions UM INNER JOIN bx_Missions M ON UM.MissionID = M.ID  WHERE  M.IsEnable = 1 AND UserID = @UserID AND Status = @Status;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Mission_GetCategories')
	DROP PROCEDURE [bx_Mission_GetCategories];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Mission_GetCategories 
AS
BEGIN
    SET NOCOUNT ON

    SELECT * FROM bx_MissionCategories;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Mission_GetCategory')
	DROP PROCEDURE [bx_Mission_GetCategory];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Mission_GetCategory
    @CategoryID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_MissionCategories WHERE [ID] = @CategoryID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Mission_CreateCategory')
	DROP PROCEDURE [bx_Mission_CreateCategory];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Mission_CreateCategory
    @Name nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS(SELECT * FROM bx_MissionCategories WHERE [Name] = @Name) BEGIN
        INSERT INTO bx_MissionCategories ([Name]) VALUES (@Name);
	END
	ELSE BEGIN
		SELECT -1;
	END

    SELECT @@IDENTITY;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Mission_UpdateCategory')
	DROP PROCEDURE [bx_Mission_UpdateCategory];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Mission_UpdateCategory
    @CategoryID      int
   ,@Name            nvarchar(50)
AS BEGIN
    SET NOCOUNT ON;

    UPDATE 
        [bx_MissionCategories]
    SET 
        [Name] = @Name
    WHERE
        [ID] = @CategoryID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateLastVisitor')
	DROP PROCEDURE [bx_UpdateLastVisitor];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateLastVisitor
     @UserID int
    ,@VisitorUserID int
    ,@CreateIP varchar(50)
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @OldCreateIP varchar(50)
	DECLARE @OldCreateDate datetime

	IF(@UserID <> @VisitorUserID AND EXISTS (SELECT * FROM bx_Users WHERE UserID = @VisitorUserID)) BEGIN

		SELECT @OldCreateIP = CreateIP,@OldCreateDate = CreateDate FROM bx_Visitors WHERE UserID = @UserID AND VisitorUserID = @VisitorUserID

		IF (@OldCreateIP IS NOT NULL) AND (@OldCreateDate IS NOT NULL) BEGIN

			UPDATE bx_Visitors SET CreateDate = GETDATE(),CreateIP = @CreateIP WHERE UserID = @UserID AND VisitorUserID = @VisitorUserID;
		END
		ELSE BEGIN
			INSERT INTO bx_Visitors(UserID,VisitorUserID,CreateIP) VALUES (@UserID,@VisitorUserID,@CreateIP);
		END

		UPDATE bx_Users SET SpaceViews = SpaceViews + 1 WHERE UserID = @UserID;

	    SELECT 'ResetUser' AS XCMD, UserID, SpaceViews FROM bx_Users WHERE UserID = @UserID;
	END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateSpaceTheme')
	DROP PROCEDURE [bx_UpdateSpaceTheme];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateSpaceTheme
     @UserID int
    ,@Theme nvarchar(50)
AS
BEGIN

    SET NOCOUNT ON;

    UPDATE bx_Users SET SpaceTheme = @Theme WHERE UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Share_GetPostShareCount')
	DROP PROCEDURE [bx_Share_GetPostShareCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Share_GetPostShareCount
	@UserID		int,
	@BeginDate	datetime,
	@EndDate	datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM [bx_SharesView] WHERE [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Share_GetUserShare')
	DROP PROCEDURE [bx_Share_GetUserShare];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Share_GetUserShare
    @UserShareID int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM bx_SharesView WHERE UserShareID=@UserShareID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateShare')
	DROP PROCEDURE [bx_CreateShare];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateShare
     @UserID               int
    ,@Type                 tinyint
    ,@PrivacyType          tinyint
    ,@Title                nvarchar(100)
    ,@Url                  nvarchar(512)
    ,@Content              nvarchar(2800)
    ,@Description          nvarchar(1000)
    ,@SortOrder            bigint
    ,@TargetID             int
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [bx_Shares] ([UserID], [Type], [Content], [SortOrder],[TargetID],[Url])
    VALUES(@UserID, @Type, @Content, @SortOrder, @TargetID, @Url);

    DECLARE @ShareID int;

    SELECT @ShareID = @@IDENTITY;

    IF @Type = 9 BEGIN --- 主题
        IF @PrivacyType = 2
            UPDATE bx_Threads SET CollectionCount = CollectionCount+1 WHERE ThreadID = @TargetID;
        ELSE
            UPDATE bx_Threads SET ShareCount = ShareCount+1 WHERE ThreadID = @TargetID;
    END 

    INSERT INTO [bx_UserShares] ([UserID], [ShareID], [PrivacyType], [Subject], [Description]) 
    VALUES (@UserID, @ShareID, @PrivacyType, @Title, @Description);

    SELECT @ShareID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetShare')
	DROP PROCEDURE [bx_GetShare];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetShare
     @ShareID   int
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_SharesView] WHERE [ShareID] = @ShareID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Share_UpdateShareKeywords')
	DROP PROCEDURE [bx_Share_UpdateShareKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Share_UpdateShareKeywords
    @ShareID               int,
    @KeywordVersion        varchar(32),
    @Subject               nvarchar(1000),
    @SubjectReverter       nvarchar(4000),
    @Description           ntext,
    @DescriptionReverter   ntext
AS BEGIN

    SET NOCOUNT ON;

    IF @Subject IS NOT NULL OR @Description IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL BEGIN

            IF @Subject IS NOT NULL AND @Description IS NOT NULL
                UPDATE [bx_UserShares] SET Subject = @Subject, Description = @Description, KeywordVersion = @KeywordVersion WHERE UserShareID = @ShareID;
            ELSE IF @Subject IS NOT NULL
                UPDATE [bx_UserShares] SET Subject = @Subject, KeywordVersion = @KeywordVersion WHERE UserShareID = @ShareID;
            ELSE
                UPDATE [bx_UserShares] SET Description = @Description, KeywordVersion = @KeywordVersion WHERE UserShareID = @ShareID;

        END
        ELSE BEGIN

           IF @Subject IS NOT NULL AND @Description IS NOT NULL
                UPDATE [bx_UserShares] SET Subject = @Subject, Description = @Description WHERE UserShareID = @ShareID;
            ELSE IF @Subject IS NOT NULL
                UPDATE [bx_UserShares] SET Subject = @Subject WHERE UserShareID = @ShareID;
            ELSE
                UPDATE [bx_UserShares] SET Description = @Description WHERE UserShareID = @ShareID;

        END

    END

    IF (@SubjectReverter IS NOT NULL AND @DescriptionReverter IS NOT NULL) BEGIN

        UPDATE bx_UserShareReverters SET SubjectReverter = @SubjectReverter, DescriptionReverter = @DescriptionReverter WHERE UserShareID = @ShareID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_UserShareReverters (UserShareID, SubjectReverter, DescriptionReverter) VALUES (@ShareID, @SubjectReverter, @DescriptionReverter);

    END
    ELSE IF (@SubjectReverter IS NOT NULL) BEGIN

        UPDATE bx_UserShareReverters SET SubjectReverter = @SubjectReverter WHERE UserShareID = @ShareID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_UserShareReverters (UserShareID, SubjectReverter, DescriptionReverter) VALUES (@ShareID, @SubjectReverter, N'');

    END
    ELSE IF (@DescriptionReverter IS NOT NULL) BEGIN

        UPDATE bx_UserShareReverters SET DescriptionReverter = @DescriptionReverter WHERE UserShareID = @ShareID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_UserShareReverters (UserShareID, SubjectReverter, DescriptionReverter) VALUES (@ShareID, N'', @DescriptionReverter);

    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Share_UpdateShareKeywords1')
	DROP PROCEDURE [bx_Share_UpdateShareKeywords1];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Share_UpdateShareKeywords1
    @ShareID               int,
    @KeywordVersion        varchar(32),
    @Content               nvarchar(1000),
    @ContentReverter       nvarchar(4000)
AS BEGIN


    SET NOCOUNT ON;

    IF @Content IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL
            UPDATE [bx_Shares] SET Content = @Content, KeywordVersion = @KeywordVersion WHERE ShareID = @ShareID;
        ELSE
            UPDATE [bx_Shares] SET Content = @Content WHERE ShareID = @ShareID;

    END

    IF @ContentReverter IS NOT NULL BEGIN

        UPDATE bx_ShareReverters SET ContentReverter = @ContentReverter WHERE ShareID = @ShareID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_ShareReverters (ShareID, ContentReverter) VALUES (@ShareID, @ContentReverter);

    END


END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Share_Agree')
	DROP PROCEDURE [bx_Share_Agree];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
create procedure bx_Share_Agree
    @ShareID    int,
    @UserID     int
as
begin
    SET NOCOUNT ON

    if not exists(select * from bx_ShareAgreeOrOpposeLogs where ShareID = @ShareID and UserID = @UserID) begin

        insert into bx_ShareAgreeOrOpposeLogs (ShareID, UserID, IsAgree) values (@ShareID, @UserID, 1);

        update 
            bx_Shares
        set
            AgreeAndOpposeCount = AgreeCount + OpposeCount + 1,
            AgreeCount = AgreeCount + 1,
            SortOrder = SortOrder + 1
        where
            ShareID = @ShareID;

    end
    
end
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Share_Oppose')
	DROP PROCEDURE [bx_Share_Oppose];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
create procedure bx_Share_Oppose
    @ShareID    int,
    @UserID     int
as
begin
    SET NOCOUNT ON

    if not exists(select * from bx_ShareAgreeOrOpposeLogs where ShareID = @ShareID and UserID = @UserID) begin

        insert into bx_ShareAgreeOrOpposeLogs (ShareID, UserID, IsAgree) values (@ShareID, @UserID, 0);

        update 
            bx_Shares
        set
            AgreeAndOpposeCount = AgreeCount + OpposeCount + 1,
            OpposeCount = OpposeCount + 1,
            SortOrder = SortOrder + 1
        where
            ShareID = @ShareID;

    end

end
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Share_ReShare')
	DROP PROCEDURE [bx_Share_ReShare];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
create procedure bx_Share_ReShare
    @ShareID        int,
    @UserID         int,
    @PrivacyType    tinyint,
    @Subject        nvarchar(100),
    @Description    nvarchar(1000)
as
begin
    SET NOCOUNT ON
    
    insert into [bx_UserShares] (ShareID, UserID, PrivacyType, Subject, Description) values (@ShareID, @UserID, @PrivacyType, @Subject, @Description);

    update bx_Shares set ShareCount = ShareCount + 1 where ShareID = @ShareID;
    
    DECLARE @Type tinyint,@TargetID int;
    SELECT @Type = Type, @TargetID = TargetID FROM [bx_Shares] WHERE ShareID = @ShareID;
    IF @Type = 9 BEGIN --- 主题
        IF @PrivacyType = 2
            UPDATE bx_Threads SET CollectionCount = CollectionCount+1 WHERE ThreadID = @TargetID;
        ELSE
            UPDATE bx_Threads SET ShareCount = ShareCount+1 WHERE ThreadID = @TargetID;
    END 

end
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Share_GetAgreeStates')
	DROP PROCEDURE [bx_Share_GetAgreeStates];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
create procedure bx_Share_GetAgreeStates
    @UserID     int,
    @ShareIDs   varchar(100)
as
begin
    SET NOCOUNT ON

    declare @sql nvarchar(500);

    set @sql = N'select ShareID, IsAgree from bx_ShareAgreeOrOpposeLogs where UserID = @uid and ShareID IN (' + @ShareIDs + ')';

    exec sp_executesql @sql, N'@uid int', @uid = @UserID;
end
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AddRole')
	DROP PROCEDURE [bx_AddRole];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AddRole
    @RoleID         int,
    @Name           nvarchar(50),
    @Title          nvarchar(50),
    @Color          varchar(50),
    @IconUrl        varchar(200),
    @RoleType       int,
    @Level          int,
    @StarLevel      int,
    @RequiredPoint  int
AS
BEGIN
    
    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM bx_Roles WHERE RoleID=@RoleID)
        
        UPDATE bx_Roles SET [Name]=@Name,Title=@Title,Color=@Color,IconUrl=@IconUrl,RoleType=@RoleType,[Level]=@Level,StarLevel=@StarLevel,RequiredPoint=@RequiredPoint WHERE RoleID=@RoleID;
    
    ELSE

        INSERT INTO [bx_Roles]([Name],Title,Color,IconUrl,RoleType,[Level],StarLevel,RequiredPoint) VALUES(@Name,@Title,@Color,@IconUrl,@RoleType,@Level,@StarLevel,@RequiredPoint);

    END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AddUserToRole_xh')
	DROP PROCEDURE [bx_AddUserToRole_xh];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AddUserToRole_xh
     @UserID      int
    ,@RoleID      int
    ,@BeginDate   datetime
    ,@EndDate     datetime
AS BEGIN

    SET NOCOUNT ON;

    IF EXISTS ( SELECT * FROM bx_Users WHERE UserID = @UserID ) BEGIN

        IF EXISTS ( SELECT * FROM bx_UsersInRoles WHERE UserID = @UserID AND RoleID = @RoleID )
            UPDATE bx_UsersInRoles SET BeginDate = @BeginDate, EndDate = @EndDate WHERE UserID = @UserID AND RoleID = @RoleID;
        ELSE
            INSERT INTO bx_UsersInRoles (UserID, RoleID, BeginDate, EndDate) VALUES (@UserID, @RoleID, @BeginDate, @EndDate);

    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetFeedTemplates')
	DROP PROCEDURE [bx_GetFeedTemplates];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetFeedTemplates
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_FeedTemplates] ORDER BY [AppID],[ActionType];
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateFeed')
	DROP PROCEDURE [bx_CreateFeed];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateFeed
     @IsSpecial        bit 
     
	,@UserID           int
	,@TargetID         int
	,@TargetUserID     int
	
	,@ActionType       tinyint
	,@PrivacyType      tinyint
	
	,@AppID            uniqueidentifier
	
	,@Title            nvarchar(1000)
	,@Realname         nvarchar(50)
	,@Description      nvarchar(2500)
	,@TargetNickname   nvarchar(50)
	,@VisibleUserIDs   varchar(800)
	
	,@CreateDate       datetime
	,@CanJoin		   bit --是否能合并的 如果能将查找1小时内相同的动态进行合并
	
	,@DefaultSendType  tinyint -- 0默认发送 1默认不发送 2强制发送 3强制不发送
	,@CommentTargetID  int
AS
BEGIN

	SET NOCOUNT ON;
	

    IF @DefaultSendType = 3
		RETURN;
	IF @DefaultSendType <> 2 BEGIN
		DECLARE @Send bit
		SELECT @Send = [Send] FROM bx_UserNoAddFeedApps WHERE AppID=@AppID AND UserID=@UserID AND ActionType=@ActionType;
		
		IF @Send IS NULL AND @DefaultSendType = 1 --默认不发送
			RETURN;
		ELSE IF @Send = 0 -- 用户设置不发送
			RETURN;
			
		IF @IsSpecial=1 BEGIN
			SET @Send = NULL;
			SELECT @Send = [Send] FROM bx_UserNoAddFeedApps WHERE AppID=@AppID AND UserID=@TargetUserID AND ActionType=@ActionType;
			
			IF @Send IS NULL AND @DefaultSendType = 1 --默认不发送
				RETURN;
			ELSE IF @Send = 0 -- 用户设置不发送
				RETURN;
		END
	END
	
    DECLARE @FeedID int;
    DECLARE @IsExistUser bit;-- bx_UserFeeds表中是否存在当前用户
    DECLARE @IsExistTargetUser bit;
    
    SET @IsExistUser = 1; 
    SET @IsExistTargetUser = 1; 
    
	IF @CanJoin = 1
		SELECT @FeedID=ID FROM bx_Feeds WHERE AppID=@AppID AND ActionType=@ActionType AND ((TargetID IS NOT NULL AND TargetID=@TargetID)  OR (TargetID IS NULL AND @TargetID IS NULL)) AND TargetUserID=@TargetUserID AND CreateDate>DATEADD(hour,-1,@CreateDate);--查找1小时内相同的动态
		
	IF @FeedID IS NOT NULL BEGIN
		IF EXISTS(SELECT * FROM bx_UserFeeds WHERE FeedID=@FeedID AND UserID=@UserID) BEGIN
			UPDATE bx_UserFeeds SET 
					 Realname=@Realname
					,CreateDate=@CreateDate 
					WHERE FeedID=@FeedID AND UserID=@UserID;
		END
		ELSE
			SET @IsExistUser = 0;
		
		IF @IsSpecial=1 BEGIN
			IF EXISTS(SELECT * FROM bx_UserFeeds WHERE FeedID=@FeedID AND UserID=@TargetUserID)
				UPDATE bx_UserFeeds SET 
						 Realname=@TargetNickname
						,CreateDate=@CreateDate 
						WHERE FeedID=@FeedID AND UserID=@TargetUserID;
			ELSE
				SET @IsExistTargetUser = 0;
		END
		UPDATE [bx_Feeds] SET Title=@Title,Description=@Description,CommentTargetID=@CommentTargetID,CommentInfo=null WHERE [ID]=@FeedID
	END 
	ELSE BEGIN
		SET @IsExistUser = 0;
		SET @IsExistTargetUser = 0;
		
		INSERT INTO bx_Feeds( 
					 TargetID
					,TargetUserID
					,ActionType
					,PrivacyType
					,AppID
					,Title
					,Description
					,TargetNickname
					,VisibleUserIDs
					,CreateDate
					,CommentTargetID
					) VALUES(
					 @TargetID
					,@TargetUserID
					,@ActionType
					,@PrivacyType
					,@AppID
					,@Title
					,@Description
					,@TargetNickname
					,@VisibleUserIDs
					,@CreateDate
					,@CommentTargetID
					);
					
		SELECT @FeedID = @@IDENTITY;
		
		
	END
	
	IF @IsExistUser = 0
		INSERT INTO bx_UserFeeds(
					 FeedID
					,UserID
					,Realname
					,CreateDate
					) VALUES(
					 @FeedID
					,@UserID
					,@Realname
					,@CreateDate
					);
					
	IF @IsSpecial=1 AND @IsExistTargetUser = 0
		INSERT INTO bx_UserFeeds(
					 FeedID
					,UserID
					,Realname
					,CreateDate
					) VALUES(
					 @FeedID
					,@TargetUserID
					,@TargetNickname
					,@CreateDate
					);
	UPDATE bx_Users SET UpdateDate = GETDATE() WHERE [UserID] = @UserID  -- 用户最后更新时间也放在这
	SELECT @FeedID AS FeedID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserNoAddFeedApps')
	DROP PROCEDURE [bx_GetUserNoAddFeedApps];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserNoAddFeedApps
    @UserID int
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_UserNoAddFeedApps] WHERE [UserID]=@UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateFeedFilter')
	DROP PROCEDURE [bx_CreateFeedFilter];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateFeedFilter
	 @AppID           uniqueidentifier
	,@UserID          int
	,@FriendUserID    int
	,@ActionType      tinyint
AS
BEGIN

   SET NOCOUNT ON;
   
   DECLARE @EmptyGuid uniqueidentifier;
   SET @EmptyGuid = '00000000-0000-0000-0000-000000000000'

   BEGIN TRANSACTION
   
   IF @FriendUserID IS NULL BEGIN --屏蔽所有好友的当前应用动作
	   IF EXISTS(SELECT * FROM [bx_FeedFilters] WHERE [AppID]=@AppID AND [ActionType]=@ActionType AND [UserID]=@UserID AND [FriendUserID] IS NULL)
			GOTO CommitTrans;
	   DELETE [bx_FeedFilters] WHERE [UserID]=@UserID AND [AppID] = @AppID
	   IF(@@error<>0)
					GOTO Cleanup;
   END                
   ELSE IF @ActionType IS null BEGIN 
			IF EXISTS(SELECT * FROM [bx_FeedFilters] WHERE [AppID]=@AppID AND [ActionType] IS NULL AND [UserID]=@UserID AND [FriendUserID]=@FriendUserID)
				GOTO CommitTrans; 
	        IF @AppID = @EmptyGuid BEGIN --屏蔽当前好友的所有动态
				DELETE [bx_FeedFilters] WHERE [UserID]=@UserID AND [FriendUserID]=@FriendUserID
				IF(@@error<>0)
					GOTO Cleanup;
		    END
   END
   ELSE BEGIN
		IF EXISTS(SELECT * FROM [bx_FeedFilters] WHERE [AppID]=@AppID AND [ActionType]=@ActionType AND [UserID]=@UserID AND [FriendUserID] IS NULL)
			GOTO CommitTrans;
		IF EXISTS(SELECT * FROM [bx_FeedFilters] WHERE [AppID]=@EmptyGuid AND [UserID]=@UserID AND [FriendUserID]=@FriendUserID)
			GOTO CommitTrans;
		IF EXISTS(SELECT * FROM [bx_FeedFilters] WHERE [AppID]=@AppID AND [ActionType]=@ActionType AND [UserID]=@UserID AND [FriendUserID]=@FriendUserID)
			GOTO CommitTrans;
   END

    INSERT INTO [bx_FeedFilters](
                     [AppID] 
                    ,[UserID]
                    ,[FriendUserID]
                    ,[ActionType]
                    ) VALUES (
                     @AppID 
                    ,@UserID
                    ,@FriendUserID
                    ,@ActionType
                    );
    IF(@@error<>0)
		GOTO Cleanup;
	ELSE BEGIN
		GOTO CommitTrans;
	END
	
	
 CommitTrans:
	BEGIN
		COMMIT TRANSACTION
		RETURN (0);
	END
                    
 Cleanup:
    BEGIN
    	ROLLBACK TRANSACTION
    	RETURN (-1)
    END
     
                    
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetFeedFilters')
	DROP PROCEDURE [bx_GetFeedFilters];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetFeedFilters
    @UserID int
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_FeedFilters] WHERE [UserID]=@UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateFeedPrivacyType')
	DROP PROCEDURE [bx_UpdateFeedPrivacyType];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateFeedPrivacyType
     @PrivacyType     tinyint
    ,@ActionType      tinyint
    ,@TargetID        int
    ,@VisibleUserIDs  varchar(800)
    ,@AppID           uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;
    UPDATE [bx_Feeds] SET 
        [PrivacyType] = @PrivacyType
      , [VisibleUserIDs] = @VisibleUserIDs 
    WHERE [AppID] = @AppID AND [ActionType] = @ActionType AND [TargetID] = @TargetID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateMaxSystemNotifyID')
	DROP PROCEDURE [bx_UpdateMaxSystemNotifyID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateMaxSystemNotifyID
@UserID      int,
@NotifyID    int
AS
BEGIN
    SET NOCOUNT ON
    UPDATE bx_UserVars SET LastReadSystemNotifyID = @NotifyID WHERE UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserNotifySetting')
	DROP PROCEDURE [bx_GetUserNotifySetting];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserNotifySetting
@UserID      int
AS
BEGIN
    SET NOCOUNT ON
    SELECT NotifySetting FROM bx_UserInfos WITH (NOLOCK) WHERE UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_EmailIsExists')
	DROP PROCEDURE [bx_EmailIsExists];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_EmailIsExists 
    @Email    nvarchar(200)
AS BEGIN

    SET NOCOUNT ON;

    IF EXISTS(SELECT [UserID] FROM bx_Users WITH(NOLOCK) WHERE [Email] = @Email) BEGIN
	    SELECT 1;
    END
    ELSE BEGIN
        SELECT 0;
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Register')
	DROP PROCEDURE [bx_Register];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Register
	 @Username               nvarchar(50)
	,@Password               nvarchar(50)
	,@Email                  nvarchar(200)
	,@CreateIP               nvarchar(50)
	,@IPSameInterval         int     
	,@PasswordFormat         int
	,@Serial                 uniqueidentifier
	,@IsActive               bit

	,@BlogPrivacy			tinyint = 0
	,@FeedPrivacy			tinyint = 0
	,@BoardPrivacy			tinyint = 0
	,@DoingPrivacy			tinyint = 0
	,@AlbumPrivacy			tinyint = 0
	,@SpacePrivacy			tinyint = 0
	,@SharePrivacy			tinyint = 0
	,@FriendListPrivacy		tinyint = 0
	,@InformationPrivacy	tinyint = 0

	,@InviterID              int 
	,@Point0                 int
	,@Point1                 int
	,@Point2                 int
	,@Point3                 int
	,@Point4                 int
	,@Point5                 int
	,@Point6                 int
	,@Point7                 int
	
	,@RoleIDs	             text
	,@RoleEndDates           text  
	
	,@UserID                 int OUTPUT
	AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @ErrorCode  int;
	DECLARE @NewUserID  int;
	
	BEGIN TRANSACTION  REGISTER

    IF EXISTS(SELECT [UserID] FROM bx_Users WHERE [Username]=@Username) BEGIN
		SET @ErrorCode = 1;
		GOTO Cleanup;
	END

	IF EXISTS(SELECT [UserID] FROM bx_Users WHERE [Email]=@Email) BEGIN
		SET @ErrorCode = 2;
		GOTO Cleanup;
	END
	
	IF @IPSameInterval>0 BEGIN
		DECLARE @LastSameIPDate DATETIME
		
		SET @LastSameIPDate = (SELECT MAX([CreateDate]) FROM [bx_Users] WHERE [CreateIP] = @CreateIP )
		
		IF NOT @LastSameIPDate IS NULL BEGIN
		
			DECLARE @Interval INT;
			SET @Interval = DATEDIFF(n, @LastSameIPDate, GETDATE());
			IF @Interval <= @IPSameInterval BEGIN
				SET @ErrorCode = 5;
				GOTO Cleanup;
			END
		END
	END

	IF @Serial IS NOT NULL AND @Serial<>'00000000-0000-0000-0000-000000000000' BEGIN
		SELECT @InviterID = UserID FROM [bx_InviteSerials] WHERE Serial = @Serial AND Status = 0 AND ExpiresDate > GETDATE();
		
		IF @InviterID IS NULL OR @InviterID = 0 BEGIN
			SET @ErrorCode = 3;
			GOTO Cleanup;
		END

	END

	IF @UserID>0 BEGIN
		IF EXISTS (SELECT [UserID] FROM [bx_Users] WHERE [UserID] = @UserID)
		BEGIN
			SET @ErrorCode = 4;
			GOTO Cleanup;
		END
		INSERT INTO [bx_Users](
					[UserID]
				   ,[Username]
				   ,[Realname]
				   ,[Email]
                   ,[CreateIP]
                   ,[LastVisitIP]  
                   ,[IsActive]
                   ,[Point_1]
                   ,[Point_2]
                   ,[Point_3]
                   ,[Point_4]
                   ,[Point_5]
                   ,[Point_6]
                   ,[Point_7]
                   ,[Point_8]
				   )
				VALUES (
					@UserID
				   ,@Username
				   ,N''
				   ,@Email
                   ,@CreateIP
                   ,@CreateIP
                   ,@IsActive
                   ,@Point0
                   ,@Point1
                   ,@Point2
                   ,@Point3
                   ,@Point4
                   ,@Point5
                   ,@Point6
                   ,@Point7
				   );
				   
			IF( @@ERROR <> 0 )
			BEGIN
				SET @ErrorCode = -1;
				GOTO Cleanup;
			END		   
		
		SET @NewUserID = @UserID;
	END	
	ELSE BEGIN
		INSERT INTO [bx_Users](
						[Username]
					   ,[Realname]
					   ,[Email]
                       ,[CreateIP]
                       ,[LastVisitIP]
                       ,[IsActive]
                       ,[Point_1]
                       ,[Point_2]
                       ,[Point_3]
                       ,[Point_4]
                       ,[Point_5]
                       ,[Point_6]
                       ,[Point_7]
                       ,[Point_8]
					   )
					VALUES(
						@Username
					   ,N''
					   ,@Email
                       ,@CreateIP
                       ,@CreateIP
                       ,@IsActive
                       ,@Point0
                       ,@Point1
                       ,@Point2
                       ,@Point3
                       ,@Point4
                       ,@Point5
                       ,@Point6
                       ,@Point7
					   );		   
		IF(@@ERROR <> 0)
		BEGIN
			SET @ErrorCode = -1;
			GOTO Cleanup;
		END
		
		SET @NewUserID = @@IDENTITY; 	
	END

	IF @NewUserID<=0
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END

	INSERT INTO [bx_UserVars](
				 UserID
				,Password
				,PasswordFormat
				)
			VALUES(
				 @NewUserID
				,@Password
				,@PasswordFormat
				);
	IF(@@ERROR <> 0)
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END
	
	INSERT INTO [bx_UserInfos](
				 UserID
				,InviterID
				,BlogPrivacy
				,FeedPrivacy
				,BoardPrivacy
				,DoingPrivacy
				,AlbumPrivacy
				,SpacePrivacy
				,SharePrivacy
				,FriendListPrivacy
				,InformationPrivacy
				)
			VALUES(
				 @NewUserID
				,@InviterID
				,@BlogPrivacy
				,@FeedPrivacy
				,@BoardPrivacy
				,@DoingPrivacy
				,@AlbumPrivacy
				,@SpacePrivacy
				,@SharePrivacy
				,@FriendListPrivacy
				,@InformationPrivacy
				);
	IF(@@ERROR <> 0)
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END

	
	IF @InviterID IS NOT NULL AND @InviterID > 0 BEGIN

		IF @Serial IS NOT NULL AND @Serial<>'00000000-0000-0000-0000-000000000000'
			UPDATE [bx_InviteSerials] SET Status = 1, ToUserID = @NewUserID WHERE Serial=@Serial;
	
		INSERT INTO bx_Friends([UserID],[FriendUserID],[GroupID],[Hot],[CreateDate]) 
		VALUES(@NewUserID,@InviterID,0,0,GETDATE());

		INSERT INTO bx_Friends([UserID],[FriendUserID],[GroupID],[Hot],[CreateDate]) 
		VALUES(@InviterID,@NewUserID,0,0,GETDATE());

		UPDATE [bx_Users] SET [TotalInvite] = [TotalInvite] + 1 WHERE [UserID] = @InviterID;

	END
	
	IF(@@ERROR <> 0)
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END


	
	DECLARE @RoleIDsTable table(autoid int, RoleID uniqueidentifier, EndDate datetime);
	
	INSERT @RoleIDsTable (autoid, RoleID) SELECT id,item FROM bx_GetStringTable_text(@RoleIDs,',');
	UPDATE @RoleIDsTable SET EndDate = CAST(t.item as datetime) FROM bx_GetStringTable_text(@RoleEndDates,',') as t WHERE t.id = autoid ;
	
	INSERT bx_UserRoles( UserID, RoleID,BeginDate, EndDate) SELECT @NewUserID, RoleID, '1753-1-1', EndDate FROM @RoleIDsTable;
	
	IF(@@ERROR <> 0)
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END
	
	
	EXEC bx_UpdateUserGeneralPoint @NewUserID;

	SELECT @UserID = @NewUserID;

	IF(@@ERROR <> 0)
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END

	INSERT INTO bx_IPLogs(UserID,Username,NewIP) VALUES(@UserID,@Username,@CreateIP);

EXEC bx_CreatePointLogs @UserID
,@Point0
,@Point1
,@Point2
,@Point3
,@Point4
,@Point5
,@Point6
,@Point7
,@Point0
,@Point1
,@Point2
,@Point3
,@Point4
,@Point5
,@Point6
,@Point7
,N'初始化'
,N'新用户注册'   --创建积分记录
	
	GOTO CommitTrans;

 CommitTrans:
	BEGIN
		COMMIT TRANSACTION REGISTER
		RETURN (0);
	END

 Cleanup:
    BEGIN
    	ROLLBACK TRANSACTION REGISTER
    	RETURN (@ErrorCode);
    END     

END


/*
@ErrorCode
  -1 未知错误
  1  用户名被占用
  2  Email被占用
  3  邀请码错误
  4  ID已经存在
  5  注册间隔时间太频繁
*/
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AdminUpdateUserProfile')
	DROP PROCEDURE [bx_AdminUpdateUserProfile];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AdminUpdateUserProfile 
 @UserID          int
,@Realname        nvarchar(50)
,@Gender          int
,@Email           nvarchar(200)
,@BirthYear       smallint
,@Birthday        smallint
,@EmailValidated  bit
,@SignatureFormat int
,@Signature       nvarchar(1500) 
,@IsActive        bit
AS
BEGIN

    SET NOCOUNT ON;

    UPDATE bx_Users SET Realname = @Realname, Email = @Email, Gender = @Gender,  EmailValidated = @EmailValidated, Signature = @Signature, SignatureFormat = @SignatureFormat, IsActive = @IsActive WHERE [UserID] = @UserID;
    UPDATE bx_UserInfos SET  BirthYear = @BirthYear,  Birthday = @Birthday WHERE UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AdminUpdateUserinfo')
	DROP PROCEDURE [bx_AdminUpdateUserinfo];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AdminUpdateUserinfo
@UserID      int,
@CreateDate  datetime,
@TotalOnlineTime  int,
@MonthOnlineTime  int
AS
BEGIN
    SET NOCOUNT ON
    UPDATE bx_Users SET CreateDate = @CreateDate , TotalOnlineTime = @TotalOnlineTime , MonthOnlineTime = @MonthOnlineTime WHERE [UserID] = @UserID;    
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateLoginUserCount')
	DROP PROCEDURE [bx_UpdateLoginUserCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateLoginUserCount
@UserID      int
,@LoginIP    varchar(50)
AS
BEGIN
    SET NOCOUNT ON
    UPDATE [bx_Users] SET LastVisitIP = @LoginIP,LoginCount = LoginCount + 1  WHERE [UserID] = @UserID;
    
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateUserExtendProfilePrivacy')
	DROP PROCEDURE [bx_UpdateUserExtendProfilePrivacy];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateUserExtendProfilePrivacy
@Key        varchar(36)
,@Privacy   tinyint
AS
BEGIN
    SET NOCOUNT ON
    UPDATE [bx_UserExtendedValues] SET PrivacyType = @Privacy  WHERE [ExtendedFieldID] = @Key;
    
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteUserExtendProfile')
	DROP PROCEDURE [bx_DeleteUserExtendProfile];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteUserExtendProfile
@Key        varchar(36)
AS
BEGIN
    SET NOCOUNT ON
    DELETE [bx_UserExtendedValues]  WHERE [ExtendedFieldID] = @Key;
    
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateUsername')
	DROP PROCEDURE [bx_UpdateUsername];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateUsername 
@UserID     int
,@Username   nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON
    UPDATE bx_Users SET [Username] = @Username WHERE [UserID] = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ActivingUser')
	DROP PROCEDURE [bx_ActivingUser];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ActivingUser
@Serial uniqueidentifier,
@UserID int out
AS
BEGIN
    SET NOCOUNT ON;

    SET @UserID = NULL;
    SELECT @UserID = UserID FROM bx_Serials Where Serial = @Serial AND Type = 1 AND ExpiresDate >= GETDATE();
    IF @UserID IS NULL
        RETURN(2); --无效的激活码

	DELETE FROM bx_Serials WHERE UserID = @UserID AND Type = 1;

	UPDATE bx_Users SET IsActive = 1, EmailValidated = 1 WHERE UserID = @UserID AND IsActive = 0;
	IF @@ROWCOUNT = 0
		RETURN(3);  --用户不需要激活

	RETURN(1);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ValidateUserEmail')
	DROP PROCEDURE [bx_ValidateUserEmail];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ValidateUserEmail
    @UserID   int,
    @Email   nvarchar(50)
AS BEGIN

    SET NOCOUNT ON;

    UPDATE bx_Users SET [Email] = @Email,[EmailValidated] = 1 WHERE [UserID] = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_User_UpdateLastVisitIP')
	DROP PROCEDURE [bx_User_UpdateLastVisitIP];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_User_UpdateLastVisitIP
     @UserID     int
    ,@NewIP      varchar(50)
AS BEGIN

    SET NOCOUNT ON;

    UPDATE bx_Users SET LastVisitIP = @NewIP WHERE [UserID] = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_User_UpdateSkinID')
	DROP PROCEDURE [bx_User_UpdateSkinID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_User_UpdateSkinID
     @UserID     int
    ,@SkinID     nvarchar(256)
AS BEGIN

    SET NOCOUNT ON;

    UPDATE bx_UserVars SET SkinID = @SkinID WHERE [UserID] = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateEmail')
	DROP PROCEDURE [bx_UpdateEmail];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateEmail
     @UserID     int
    ,@Email      nvarchar(200)
AS BEGIN

    SET NOCOUNT ON;

    IF (EXISTS (SELECT * FROM [bx_Users] WHERE Email = @Email AND [UserID] <> @UserID))
        SELECT 0;
    ELSE BEGIN
        UPDATE bx_Users SET Email = @Email WHERE [UserID] = @UserID;
        SELECT 1;
    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetSimpleUser')
	DROP PROCEDURE [bx_GetSimpleUser];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetSimpleUser
    @UserID int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM bx_SimpleUser WITH(NOLOCK) WHERE UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateAllUserPoints')
	DROP PROCEDURE [bx_UpdateAllUserPoints];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateAllUserPoints 
      @MaxPoint1    int
    , @MaxPoint2    int
    , @MaxPoint3    int
    , @MaxPoint4    int
    , @MaxPoint5    int
    , @MaxPoint6    int
    , @MaxPoint7    int
    , @MaxPoint8    int
    , @MinPoint1    int
    , @MinPoint2    int
    , @MinPoint3    int
    , @MinPoint4    int
    , @MinPoint5    int
    , @MinPoint6    int
    , @MinPoint7    int
    , @MinPoint8    int
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [bx_Users] SET 
        [Point_1]=CASE WHEN [Point_1] > @MaxPoint1 THEN @MaxPoint1 ELSE 
            CASE WHEN [Point_1] < @MinPoint1 THEN @MinPoint1 ELSE [Point_1] END 
        END
        ,[Point_2]=CASE WHEN [Point_2] > @MaxPoint2 THEN @MaxPoint2 ELSE 
            CASE WHEN [Point_2] < @MinPoint2 THEN @MinPoint2 ELSE [Point_2] END 
        END
        ,[Point_3]=CASE WHEN [Point_3] > @MaxPoint3 THEN @MaxPoint3 ELSE 
            CASE WHEN [Point_3] < @MinPoint3 THEN @MinPoint3 ELSE [Point_3] END 
        END
        ,[Point_4]=CASE WHEN [Point_4] > @MaxPoint4 THEN @MaxPoint4 ELSE 
            CASE WHEN [Point_4] < @MinPoint4 THEN @MinPoint4 ELSE [Point_4] END 
        END
        ,[Point_5]=CASE WHEN [Point_5] > @MaxPoint5 THEN @MaxPoint5 ELSE 
            CASE WHEN [Point_5] < @MinPoint5 THEN @MinPoint5 ELSE [Point_5] END 
        END
        ,[Point_6]=CASE WHEN [Point_6] > @MaxPoint6 THEN @MaxPoint6 ELSE 
            CASE WHEN [Point_6] < @MinPoint6 THEN @MinPoint6 ELSE [Point_6] END 
        END
        ,[Point_7]=CASE WHEN [Point_7] > @MaxPoint7 THEN @MaxPoint7 ELSE 
            CASE WHEN [Point_7] < @MinPoint7 THEN @MinPoint7 ELSE [Point_7] END 
        END
        ,[Point_8]=CASE WHEN [Point_8] > @MaxPoint8 THEN @MaxPoint8 ELSE 
            CASE WHEN [Point_8] < @MinPoint8 THEN @MinPoint8 ELSE [Point_8] END 
        END;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CheckUserPoint')
	DROP PROCEDURE [bx_CheckUserPoint];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CheckUserPoint 
      @UserID    int  
    , @ThrowOverMinValueError   bit
    , @ThrowOverMaxValueError   bit

    , @Point1    int
    , @Point2    int
    , @Point3    int
    , @Point4    int
    , @Point5    int
    , @Point6    int
    , @Point7    int
    , @Point8    int

    , @MinPoint1    int
    , @MinPoint2    int
    , @MinPoint3    int
    , @MinPoint4    int
    , @MinPoint5    int
    , @MinPoint6    int
    , @MinPoint7    int
    , @MinPoint8    int

    , @MaxPoint1    int
    , @MaxPoint2    int
    , @MaxPoint3    int
    , @MaxPoint4    int
    , @MaxPoint5    int
    , @MaxPoint6    int
    , @MaxPoint7    int
    , @MaxPoint8    int
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserPoint1 int,@UserPoint2 int,@UserPoint3 int,@UserPoint4 int,@UserPoint5 int,@UserPoint6 int,@UserPoint7 int,@UserPoint8 int;
    SELECT @UserPoint1=Point_1,@UserPoint2=Point_2,@UserPoint3=Point_3,@UserPoint4=Point_4,@UserPoint5=Point_5,@UserPoint6=Point_6,@UserPoint7=Point_7,@UserPoint8=Point_8
            FROM [bx_Users] WHERE [UserID]=@UserID;

    IF @ThrowOverMinValueError = 1 BEGIN
        IF @Point1<0 AND (@UserPoint1+@Point1)<@MinPoint1 BEGIN
            SELECT -1 AS Code,@UserPoint1 AS Point;
            RETURN;
        END
        IF @Point2<0 AND (@UserPoint2+@Point2)<@MinPoint2 BEGIN
            SELECT -2 AS Code,@UserPoint2 AS Point;
            RETURN;
        END
        IF @Point3<0 AND (@UserPoint3+@Point3)<@MinPoint3 BEGIN
            SELECT -3 AS Code,@UserPoint3 AS Point;
            RETURN;
        END
        IF @Point4<0 AND (@UserPoint4+@Point4)<@MinPoint4 BEGIN
            SELECT -4 AS Code,@UserPoint4 AS Point;
            RETURN;
        END
        IF @Point5<0 AND (@UserPoint5+@Point5)<@MinPoint5 BEGIN
            SELECT -5 AS Code,@UserPoint5 AS Point;
            RETURN;
        END
        IF @Point6<0 AND (@UserPoint6+@Point6)<@MinPoint6 BEGIN
            SELECT -6 AS Code,@UserPoint6 AS Point;
            RETURN;
        END
        IF @Point7<0 AND (@UserPoint7+@Point7)<@MinPoint7 BEGIN
            SELECT -7 AS Code,@UserPoint7 AS Point;
            RETURN;
        END
        IF @Point8<0 AND (@UserPoint8+@Point8)<@MinPoint8 BEGIN
            SELECT -8 AS Code,@UserPoint8 AS Point;
            RETURN;
        END
    END
    IF @ThrowOverMaxValueError = 1 BEGIN
        IF @Point1>0 AND (@UserPoint1+@Point1)>@MaxPoint1 BEGIN
            SELECT 1 AS Code,@UserPoint1 AS Point;
            RETURN;
        END
        IF @Point2>0 AND (@UserPoint2+@Point2)>@MaxPoint2 BEGIN
            SELECT 2 AS Code,@UserPoint2 AS Point;
            RETURN;
        END
        IF @Point3>0 AND (@UserPoint3+@Point3)>@MaxPoint3 BEGIN
            SELECT 3 AS Code,@UserPoint3 AS Point;
            RETURN;
        END
        IF @Point4>0 AND (@UserPoint4+@Point4)>@MaxPoint4 BEGIN
            SELECT 4 AS Code,@UserPoint4 AS Point;
            RETURN;
        END
        IF @Point5>0 AND (@UserPoint5+@Point5)>@MaxPoint5 BEGIN
            SELECT 5 AS Code,@UserPoint5 AS Point;
            RETURN;
        END
        IF @Point6>0 AND (@UserPoint6+@Point6)>@MaxPoint6 BEGIN
            SELECT 6 AS Code,@UserPoint6 AS Point;
            RETURN;
        END
        IF @Point7>0 AND (@UserPoint7+@Point7)>@MaxPoint7 BEGIN
            SELECT 7 AS Code,@UserPoint7 AS Point;
            RETURN;
        END
        IF @Point8>0 AND (@UserPoint8+@Point8)>@MaxPoint8 BEGIN
            SELECT 8 AS Code,@UserPoint8 AS Point;
            RETURN;
        END
    END
    
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateUserPoint')
	DROP PROCEDURE [bx_UpdateUserPoint];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateUserPoint 
      @UserID    int  
    , @ThrowOverMinValueError   bit
    , @ThrowOverMaxValueError   bit

    , @Point1    int
    , @Point2    int
    , @Point3    int
    , @Point4    int
    , @Point5    int
    , @Point6    int
    , @Point7    int
    , @Point8    int

    , @MinPoint1    int
    , @MinPoint2    int
    , @MinPoint3    int
    , @MinPoint4    int
    , @MinPoint5    int
    , @MinPoint6    int
    , @MinPoint7    int
    , @MinPoint8    int

    , @MaxPoint1    int
    , @MaxPoint2    int
    , @MaxPoint3    int
    , @MaxPoint4    int
    , @MaxPoint5    int
    , @MaxPoint6    int
    , @MaxPoint7    int
    , @MaxPoint8    int
    , @Operate      nvarchar(50)
    , @Remarks      nvarchar(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserPoint1 int,@UserPoint2 int,@UserPoint3 int,@UserPoint4 int,@UserPoint5 int,@UserPoint6 int,@UserPoint7 int,@UserPoint8 int;
    SELECT @UserPoint1=Point_1,@UserPoint2=Point_2,@UserPoint3=Point_3,@UserPoint4=Point_4,@UserPoint5=Point_5,@UserPoint6=Point_6,@UserPoint7=Point_7,@UserPoint8=Point_8
            FROM [bx_Users] WHERE [UserID]=@UserID;

    IF @Point1<0 AND (@UserPoint1+@Point1)<@MinPoint1 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -1 AS Code,@UserPoint1 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint1=@MinPoint1;
    END
    ELSE IF @Point1<0
        SET @UserPoint1=@UserPoint1+@Point1;

    IF @Point2<0 AND (@UserPoint2+@Point2)<@MinPoint2 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -2 AS Code,@UserPoint2 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint2=@MinPoint2;
    END
    ELSE IF @Point2<0
        SET @UserPoint2=@UserPoint2+@Point2;

    IF @Point3<0 AND (@UserPoint3+@Point3)<@MinPoint3 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -3 AS Code,@UserPoint3 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint3=@MinPoint3;
    END
    ELSE IF @Point3<0
        SET @UserPoint3=@UserPoint3+@Point3;

    IF @Point4<0 AND (@UserPoint4+@Point4)<@MinPoint4 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -4 AS Code,@UserPoint4 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint4=@MinPoint4;
    END
    ELSE IF @Point4<0
        SET @UserPoint4=@UserPoint4+@Point4;

    IF @Point5<0 AND (@UserPoint5+@Point5)<@MinPoint5 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -5 AS Code,@UserPoint5 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint5=@MinPoint5;
    END
    ELSE IF @Point5<0
        SET @UserPoint5=@UserPoint5+@Point5;

    IF @Point6<0 AND (@UserPoint6+@Point6)<@MinPoint6 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -6 AS Code,@UserPoint6 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint6=@MinPoint6;
    END
    ELSE IF @Point6<0
        SET @UserPoint6=@UserPoint6+@Point6;

    IF @Point7<0 AND (@UserPoint7+@Point7)<@MinPoint7 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -7 AS Code,@UserPoint7 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint7=@MinPoint7;
    END
    ELSE IF @Point7<0
        SET @UserPoint7=@UserPoint7+@Point7;

    IF @Point8<0 AND (@UserPoint8+@Point8)<@MinPoint8 BEGIN
        IF @ThrowOverMinValueError = 1 BEGIN
            SELECT -8 AS Code,@UserPoint8 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint8=@MinPoint8;
    END
    ELSE IF @Point8<0
        SET @UserPoint8=@UserPoint8+@Point8;


    IF @Point1>0 AND (@UserPoint1+@Point1)>@MaxPoint1 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 1 AS Code,@UserPoint1 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint1=@MaxPoint1;
    END
    ELSE IF @Point1>0
        SET @UserPoint1=@UserPoint1+@Point1;

    IF @Point2>0 AND (@UserPoint2+@Point2)>@MaxPoint2 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 2 AS Code,@UserPoint2 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint2=@MaxPoint2;
    END
    ELSE IF @Point2>0
        SET @UserPoint2=@UserPoint2+@Point2;

    IF @Point3>0 AND (@UserPoint3+@Point3)>@MaxPoint3 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 3 AS Code,@UserPoint3 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint3=@MaxPoint3;
    END
    ELSE IF @Point3>0
        SET @UserPoint3=@UserPoint3+@Point3;

    IF @Point4>0 AND (@UserPoint4+@Point4)>@MaxPoint4 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 4 AS Code,@UserPoint4 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint4=@MaxPoint4;
    END
    ELSE IF @Point4>0
        SET @UserPoint4=@UserPoint4+@Point4;

    IF @Point5>0 AND (@UserPoint5+@Point5)>@MaxPoint5 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 5 AS Code,@UserPoint5 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint5=@MaxPoint5;
    END
    ELSE IF @Point5>0
        SET @UserPoint5=@UserPoint5+@Point5;

    IF @Point6>0 AND (@UserPoint6+@Point6)>@MaxPoint6 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 6 AS Code,@UserPoint6 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint6=@MaxPoint6;
    END
    ELSE IF @Point6>0
        SET @UserPoint6=@UserPoint6+@Point6;

    IF @Point7>0 AND (@UserPoint7+@Point7)>@MaxPoint7 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 7 AS Code,@UserPoint7 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint7=@MaxPoint7;
    END
    ELSE IF @Point7>0
        SET @UserPoint7=@UserPoint7+@Point7;

    IF @Point8>0 AND (@UserPoint8+@Point8)>@MaxPoint8 BEGIN
        IF @ThrowOverMaxValueError = 1 BEGIN
            SELECT 8 AS Code,@UserPoint8 AS UserPoint;
            RETURN;
        END
        ELSE
            SET @UserPoint8=@MaxPoint8;
    END
    ELSE IF @Point8>0
        SET @UserPoint8=@UserPoint8+@Point8;

    UPDATE [bx_Users] SET [Point_1] = @UserPoint1, [Point_2] = @UserPoint2, [Point_3] = @UserPoint3
    , [Point_4] = @UserPoint4, [Point_5] = @UserPoint5, [Point_6] = @UserPoint6, [Point_7] = @UserPoint7, [Point_8] = @UserPoint8 WHERE [UserID]=@UserID;
 

    EXECUTE bx_UpdateUserGeneralPoint @UserID;    

    SELECT [Points],[Point_1],[Point_2],[Point_3],[Point_4],[Point_5],[Point_6],[Point_7],[Point_8] FROM [bx_Users] WHERE [UserID] = @UserID;

    EXEC bx_CreatePointLogs @UserID, @Point1, @Point2 , @Point3, @Point4, @Point5 , @Point6  , @Point7 , @Point8
, @UserPoint1 ,@UserPoint2 ,@UserPoint3 ,@UserPoint4 ,@UserPoint5 ,@UserPoint6 ,@UserPoint7 ,@UserPoint8
, @Operate, @Remarks
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetUserPoint')
	DROP PROCEDURE [bx_SetUserPoint];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetUserPoint
      @UserID       int

    , @Point1       int
    , @Point2       int
    , @Point3       int
    , @Point4       int
    , @Point5       int
    , @Point6       int
    , @Point7       int
    , @Point8       int
    , @Operate      nvarchar(50)
    , @Remarks      nvarchar(200)
AS 
BEGIN
    SET NOCOUNT ON;

    DECLARE @Op0 int,@Op1 int,@Op2 int,@Op3 int,@Op4 int,@Op5 int,@Op6 int,@Op7 int;

    SELECT @Op0 = [Point_1],@Op1 = [Point_2],@Op2 = [Point_3],@Op3 = [Point_4],@Op4 = [Point_5],@Op5 = [Point_6],@Op6 = [Point_7],@Op7 = [Point_8] FROM bx_Users WHERE UserID = @UserID;

     UPDATE [bx_Users] SET [Point_1] = @Point1, [Point_2] = @Point2, [Point_3] = @Point3
    , [Point_4] = @Point4, [Point_5] = @Point5, [Point_6] = @Point6, [Point_7] = @Point7, [Point_8] = @Point8 WHERE [UserID]=@UserID; 

   
    EXECUTE bx_UpdateUserGeneralPoint @UserID;

    SELECT Points FROM [bx_Users] WHERE UserID=@UserID; 

    SET @Op0 = @Point1 - @Op0;
    SET @Op1 = @Point2 - @Op1;
    SET @Op2 = @Point3 - @Op2;
    SET @Op3 = @Point4 - @Op3;
    SET @Op4 = @Point5 - @Op4;
    SET @Op5 = @Point6 - @Op5;
    SET @Op6 = @Point7 - @Op6;
    SET @Op7 = @Point8 - @Op7;
 
    EXEC bx_CreatePointLogs @UserID
    ,@Op0, @Op1, @Op2, @Op3, @Op4, @Op5, @Op6, @Op7 
    ,@Point1, @Point2 , @Point3, @Point4, @Point5 , @Point6  , @Point7 , @Point8
    ,@Operate, @Remarks
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserPassword')
	DROP PROCEDURE [bx_GetUserPassword];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserPassword
	@UserID                 int
AS
BEGIN

	SET NOCOUNT ON;

	SELECT UserID, Password, PasswordFormat FROM [bx_UserVars] WHERE UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAuthUser')
	DROP PROCEDURE [bx_GetAuthUser];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAuthUser
	 @Username               nvarchar(50)
	,@UserID                 int
AS
BEGIN

	SET NOCOUNT ON;

	IF @UserID <= 0
        SELECT @UserID = [UserID] FROM [bx_Users] WHERE [Username] = @Username;

    IF @UserID <= 0
        RETURN;

	SELECT * FROM [bx_AuthUsers] WHERE UserID = @UserID;

	SELECT * FROM [bx_Friends] WITH (NOLOCK) WHERE UserID = @UserID ORDER BY [Hot] DESC,[CreateDate] ASC;

    SELECT * FROM [bx_UnreadNotifies] WHERE UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAuthUserByEmail')
	DROP PROCEDURE [bx_GetAuthUserByEmail];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAuthUserByEmail
     @Email                nvarchar(50)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @RepeatCount int;

    SELECT @RepeatCount = COUNT(*) FROM (SELECT TOP 2 UserID FROM bx_Users WHERE Email =@Email) AS T;
    
    IF @RepeatCount > 1
        RETURN (2);

    ELSE IF @RepeatCount < 1
        RETURN (3);

    ELSE BEGIN


        DECLARE @UserID int;
        SET @UserID = -1;

        SELECT @UserID = [UserID] FROM [bx_Users] WHERE [Email] = @Email;

        IF @UserID > 0 BEGIN

            SELECT * FROM [bx_AuthUsers] WHERE UserID = @UserID;
            SELECT * FROM [bx_Friends] WITH (NOLOCK) WHERE UserID = @UserID ORDER BY [Hot] DESC, [CreateDate] ASC;
            SELECT * FROM [bx_UnreadNotifies] WHERE UserID = @UserID;

        END
        ELSE
            RETURN (3);

    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUser')
	DROP PROCEDURE [bx_GetUser];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUser
	 @Username               nvarchar(50)
	,@UserID                 int
    ,@GetFriends             bit
AS
BEGIN

	SET NOCOUNT ON;

	IF @UserID <= 0
        SELECT @UserID = [UserID] FROM [bx_Users] WITH(NOLOCK) WHERE [Username] = @Username;

    IF @UserID <= 0
        RETURN;

	SELECT * FROM [bx_Users] WITH(NOLOCK) WHERE UserID = @UserID;

    IF @GetFriends = 1
	    SELECT * FROM [bx_Friends] WITH (NOLOCK) WHERE UserID = @UserID ORDER BY [Hot] DESC,[CreateDate] ASC;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetNotUseUserIDs')
	DROP PROCEDURE [bx_GetNotUseUserIDs];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetNotUseUserIDs
     @BeginID     int
    ,@EndID       int
AS BEGIN
    SET NOCOUNT ON;

    SELECT (T1.UserID + 1) FROM bx_Users T1 WITH (NOLOCK)
     WHERE
	    (T1.UserID + 1) NOT IN 
            (SELECT T2.UserID FROM bx_Users T2 WITH (NOLOCK) WHERE T2.UserID > @BeginID AND T2.UserID < @EndID)
	    AND T1.UserID > @BeginID AND T1.UserID < @EndID
    ORDER BY T1.UserID;

    SELECT (T1.UserID - 1) FROM bx_Users T1 WITH (NOLOCK)
     WHERE 
	    (T1.UserID - 1) NOT IN 
            (SELECT T2.UserID FROM bx_Users T2 WITH (NOLOCK) WHERE T2.UserID > @BeginID AND T2.UserID < @EndID)
	    AND T1.UserID > @BeginID AND T1.UserID < @EndID
    ORDER BY T1.UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_IPIsExistInMinutes')
	DROP PROCEDURE [bx_IPIsExistInMinutes];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_IPIsExistInMinutes
     @IP        nvarchar(50)
    ,@TimeSpan  int
AS BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT [UserID] FROM [bx_Users] WITH (NOLOCK) WHERE CreateIP = @IP AND abs(datediff(mi,CreateDate,getdate())) < @TimeSpan AND [UserID]<>0)
	    SELECT 1;
    ELSE
	    SELECT 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteUser')
	DROP PROCEDURE [bx_DeleteUser];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteUser
 @UserID             int 
,@Step               int
AS 
BEGIN
    SET NOCOUNT ON;

DECLARE @TotalCount  int
SET @TotalCount=0;

    IF @Step = 1  BEGIN --动态
	    DELETE [bx_Feeds] WHERE [TargetUserID] = @UserID;
        DELETE [bx_UserFeeds] WHERE [UserID] = @UserID;
        SET @TotalCount= 0;
    END

    IF @Step = 2  BEGIN
	    DELETE [bx_Comments] WHERE  [TargetID] = @UserID OR [UserID] = @UserID;
        SET @TotalCount= 0;
    END

    IF @Step = 3  BEGIN
	   DELETE FROM [bx_Notify] WHERE [UserID] = @UserID;
       SET @TotalCount= 0;
    END	

    IF @Step = 4  BEGIN
	    DELETE FROM [bx_InviteSerials]  WHERE UserID = @UserID OR ToUserID = @UserID;
       SET @TotalCount= 0;
    END

    IF @Step = 5  BEGIN
       UPDATE [bx_UserInfos]  SET InviterID = 0 WHERE InviterID = @UserID;
       SET @TotalCount= 0;
    END

    IF @Step = 6  BEGIN
       DECLARE @TopDirID int
       SET @TopDirID= (SELECT TOP 1 DirectoryID  FROM bx_DiskDirectories WHERE UserID = @UserID Order By DirectoryID DESC);
       DELETE FROM bx_DiskFiles WHERE DirectoryID = @TopDirID;
       DELETE FROM bx_DiskDirectories WHERE DirectoryID = @TopDirID;
       SET @TotalCount= (SELECT COUNT(*) FROM bx_DiskDirectories WHERE UserID = @UserID);
    END

    IF @Step = 7  BEGIN
       DECLARE @TopGroupID int
       SET @TopGroupID= (SELECT TOP 1 GroupID  FROM bx_EmoticonGroups WHERE UserID = @UserID Order By GroupID DESC);
       DELETE FROM bx_Emoticons WHERE GroupID = @TopGroupID;
       DELETE FROM bx_EmoticonGroups WHERE GroupID = @TopGroupID;
       SET @TotalCount= (SELECT COUNT(*) FROM bx_EmoticonGroups WHERE UserID = @UserID);
    END

    IF @Step = 8  BEGIN
       DELETE FROM bx_ChatMessages WHERE MessageID IN(SELECT TOP 200 MessageID  FROM bx_ChatMessages WHERE UserID = @UserID OR TargetUserID = @UserID);
       SET @TotalCount= (SELECT COUNT(*) FROM bx_ChatMessages WHERE UserID = @UserID OR TargetUserID = @UserID);
       IF @TotalCount=0 
        DELETE FROM bx_ChatSessions WHERE  UserID = @UserID OR TargetUserID = @UserID;
    END
    
    IF @Step = 9  BEGIN
       DELETE FROM bx_Doings WHERE DoingID IN(SELECT TOP 200 DoingID  FROM bx_Doings WHERE UserID = @UserID);
       SET @TotalCount= (SELECT COUNT(*) FROM bx_Doings WHERE UserID = @UserID);
    END

    IF @Step = 10  BEGIN
       DELETE FROM bx_Shares WHERE ShareID IN(SELECT TOP 200 ShareID  FROM bx_Shares WHERE UserID = @UserID);
       SET @TotalCount= (SELECT COUNT(*) FROM bx_Shares WHERE UserID = @UserID);
    END

    IF @Step = 11  BEGIN
	   DELETE FROM bx_Visitors WHERE UserID = @UserID OR VisitorUserID =@UserID;
       DELETE FROM bx_BlogArticleVisitors WHERE UserID = @UserID;
       SET @TotalCount= 0;
    END

    IF @Step = 12  BEGIN
	Delete [bx_BlogArticles] WHERE ArticleID IN(SELECT TOP 200 ArticleID FROM [bx_BlogArticles] WHERE  [UserID] = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_BlogArticles] WHERE UserID = @UserID);
    END

    IF @Step = 13  BEGIN
	Delete [bx_Albums] WHERE AlbumID IN(SELECT TOP 200 AlbumID FROM [bx_Albums] WHERE  [UserID] = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_Albums] WHERE UserID = @UserID);
    END

    IF @Step = 14  BEGIN
    Delete [bx_Attachments] WHERE AttachmentID IN(SELECT TOP 200 AttachmentID FROM [bx_Attachments] WHERE  [UserID] = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_Attachments] WHERE UserID = @UserID);
    END

    IF @Step = 15 BEGIN
    Delete [bx_Posts] WHERE PostID IN(SELECT TOP 200 PostID FROM [bx_Posts] WHERE  [UserID] = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_Posts] WHERE UserID = @UserID);
    END

    IF @Step = 16 BEGIN
    Delete [bx_Threads] WHERE ThreadID IN(SELECT TOP 200 ThreadID FROM [bx_Threads] WHERE [PostUserID] = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_Threads] WHERE PostUserID = @UserID);
    END

    IF @Step = 17 BEGIN
    Delete [bx_Friends] WHERE FriendUserID = @UserID OR UserID = @UserID
    SET @TotalCount= 0;
    END    

    IF @Step = 18 BEGIN
    Delete [bx_Visitors] WHERE ID IN(SELECT TOP 200 ID FROM [bx_Visitors] WHERE UserID = @UserID OR VisitorUserID = @UserID);
    SET @TotalCount= (SELECT Count(*) FROM [bx_Visitors] WHERE UserID = @UserID OR VisitorUserID = @UserID);

    END

    IF @Step = 19 BEGIN
    Delete [bx_PointLogs] WHERE LogID IN(SELECT TOP 200 LogID FROM [bx_PointLogs] WHERE UserID = @UserID);
    SET @TotalCount = (SELECT Count(*) FROM [bx_PointLogs] WHERE UserID = @UserID);

    END

    IF @Step = 20 BEGIN
	    DELETE [bx_Users] WHERE [UserID] = @UserID;
        SET @TotalCount = 0;
    END

    IF @Step = 21 BEGIN
        EXECUTE bx_UpdateNewUserVars 0;
        SET @TotalCount = 0;
    END
    SELECT @TotalCount;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetRealnameCheck')
	DROP PROCEDURE [bx_SetRealnameCheck];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetRealnameCheck 
             @OperatorUserID   int
            ,@UserID           int
            ,@IsChecked        bit
            ,@Remark           nvarchar(1000)
            AS
BEGIN
            SET NOCOUNT ON;
            DECLARE @Success bit;
            SET  @Success = 0;
            IF EXISTS( SELECT * FROM bx_AuthenticUsers WHERE UserID = @UserID ) BEGIN 
                IF @IsChecked = 1 BEGIN
                    DECLARE @Realname nvarchar(50);
                    SET @Realname =( SELECT Realname FROM bx_AuthenticUsers WHERE UserID = @UserID) ;
                    UPDATE bx_Users SET Realname = @Realname WHERE UserID = @UserID;
                END
                ELSE BEGIN
                    UPDATE bx_Users SET Realname = '' WHERE UserID = @UserID;
                END

                UPDATE bx_AuthenticUsers SET Verified = @IsChecked, Processed = 1, OperatorUserID = @OperatorUserID,  Remark = @Remark  WHERE UserID = @UserID;
                SET @Success = 1;
            END
    SELECT @Success; 
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SaveAuthenticUserInfo')
	DROP PROCEDURE [bx_SaveAuthenticUserInfo];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SaveAuthenticUserInfo
@UserID             int
,@Realname          nvarchar(50)
,@Birthday          datetime
,@Gender            tinyint
,@IDNumber          varchar(50)
,@IDCardFileFace    nvarchar(100)
,@IDCardFileBack    nvarchar(100)
,@Area              nvarchar(100) 
AS
BEGIN
SET NOCOUNT ON;
IF EXISTS( SELECT * FROM bx_AuthenticUsers WHERE UserID = @UserID) BEGIN
    UPDATE bx_AuthenticUsers SET [Realname] = @Realname, [Gender] = @Gender, [Birthday] = @Birthday, [IDNumber] = @IDNumber, [IDCardFileFace] = @IDCardFileFace, [IDCardFileBack]=@IDCardFileBack, [Area] = @Area, Processed = 0 WHERE UserID = @UserID
    SELECT 1;
END
ELSE BEGIN
    INSERT INTO bx_AuthenticUsers([UserID], [Realname], [Gender], [Birthday], [IDNumber], [IDCardFileFace],[IDCardFileBack], [Area]) VALUES(@UserID, @Realname, @Gender, @Birthday, @IDNumber, @IDCardFileFace,@IDCardFileBack, @Area)
    SELECT 0;
END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CheckIdNumberExist')
	DROP PROCEDURE [bx_CheckIdNumberExist];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CheckIdNumberExist
@IDNumber       varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS(SELECT * FROM bx_AuthenticUsers WHERE IDNumber=@IDNumber AND Verified=1)
        RETURN(1);
    ELSE
        RETURN(0);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ResetUserPassword')
	DROP PROCEDURE [bx_ResetUserPassword];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ResetUserPassword
     @UserID     int
    ,@Password   nvarchar(50)
    ,@Format     tinyint
AS BEGIN
    SET NOCOUNT ON;

    UPDATE [bx_UserVars] SET Password = @Password, PasswordFormat = @Format WHERE UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateAvatar')
	DROP PROCEDURE [bx_UpdateAvatar];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateAvatar
    @UserID           int
   ,@AvatarSrc        nvarchar(200)
   ,@Checked          bit
AS BEGIN
    SET NOCOUNT ON;

    UPDATE [bx_Users] SET [AvatarSrc] = @AvatarSrc WHERE [UserID] = @UserID; 
    UPDATE [bx_UserVars] SET EverAvatarChecked = @Checked WHERE [UserID] = @UserID;
    
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AddUserToRole')
	DROP PROCEDURE [bx_AddUserToRole];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AddUserToRole
     @UserID      int
    ,@RoleID      uniqueIdentifier
    ,@BeginDate   datetime
    ,@EndDate     datetime
AS BEGIN

    SET NOCOUNT ON;

    IF EXISTS ( SELECT * FROM bx_Users WHERE UserID = @UserID ) BEGIN

        IF EXISTS ( SELECT * FROM bx_UserRoles WHERE UserID = @UserID AND RoleID = @RoleID )
            UPDATE bx_UserRoles SET BeginDate = @BeginDate, EndDate = @EndDate WHERE UserID = @UserID AND RoleID = @RoleID;
        ELSE
            INSERT INTO bx_UserRoles (UserID, RoleID, BeginDate, EndDate) VALUES (@UserID, @RoleID, @BeginDate, @EndDate);

    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateUserExtendedFieldVersion')
	DROP PROCEDURE [bx_UpdateUserExtendedFieldVersion];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateUserExtendedFieldVersion
	@UserID					int,
    @ExtendedFieldVersion	nchar(36)
AS BEGIN

	SET NOCOUNT ON;

	UPDATE [bx_Users] SET [ExtendedFieldVersion] = @ExtendedFieldVersion WHERE [UserID] = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserIDsBySearch')
	DROP PROCEDURE [bx_GetUserIDsBySearch];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserIDsBySearch
	@Keyword   nvarchar(100)
AS BEGIN

	SET NOCOUNT ON;

	SELECT [UserID] FROM [bx_Users] WITH (NOLOCK) WHERE [Username] like'%'+@Keyword+'%';

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateUserSelectFriendGroupID')
	DROP PROCEDURE [bx_UpdateUserSelectFriendGroupID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateUserSelectFriendGroupID
	 @GroupID   int
    ,@UserID    int
AS BEGIN

	SET NOCOUNT ON;

	UPDATE [bx_UserVars] SET SelectFriendGroupID = @GroupID WHERE UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateUserReplyReturnThreadLastPage')
	DROP PROCEDURE [bx_UpdateUserReplyReturnThreadLastPage];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateUserReplyReturnThreadLastPage
	 @ReplyReturnThreadLastPage   bit
    ,@UserID    int
AS BEGIN

	SET NOCOUNT ON;

	UPDATE [bx_UserVars] SET ReplyReturnThreadLastPage = @ReplyReturnThreadLastPage WHERE UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateOnlineTime')
	DROP PROCEDURE [bx_UpdateOnlineTime];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_UpdateOnlineTime] 
 	@OnlineTimes varchar(8000),
 	@LastUpdateOnlineTimes varchar(8000),
 	@UserIDs varchar(8000)
AS
BEGIN
	SET NOCOUNT ON;


	DECLARE @i int,@j int,@OnlineTime int,@UserID int,@k int
	DECLARE @NewOnlineTimes varchar(8000),@NewMonthOnlineTimes varchar(8000),@NewWeekOnlineTimes varchar(8000),@NewDayOnlineTimes varchar(8000)
	SET @NewOnlineTimes=''
	SET @NewMonthOnlineTimes=''
	SET @NewWeekOnlineTimes=''
	SET @NewDayOnlineTimes=''

	SET @OnlineTimes = @OnlineTimes + N','
	SELECT @i = CHARINDEX(',', @OnlineTimes)

	SET @UserIDs = @UserIDs + N','
	SELECT @j = CHARINDEX(',', @UserIDs)

	SET @LastUpdateOnlineTimes = @LastUpdateOnlineTimes +N','
	SELECT @k = CHARINDEX(',', @LastUpdateOnlineTimes)
	
	DECLARE @Monday DateTime,@Now DateTime;
	SELECT @Now = GETDATE();
	DECLARE @m int;
	SELECT @m = DATEPART(weekday, @Now);
	IF @m = 1
		SELECT @m = 8;
	SELECT @Monday = CONVERT(varchar(12) , DATEADD(day, 2-@m, @Now),111);
	
	
	WHILE ( @i > 1 ) BEGIN
			SELECT @OnlineTime = SUBSTRING(@OnlineTimes,0, @i)	
			SELECT @UserID = SUBSTRING(@UserIDs,0, @j)	
			
			declare @TotalOnlineTime int,@MonthOnlineTime int,@LastUpdateTime dateTime,@LastVisitDate datetime,@WeekOnlineTime int,@DayOnlineTime int;
			SELECT @LastUpdateTime = SUBSTRING(@LastUpdateOnlineTimes,0, @k)	
			SELECT @TotalOnlineTime=TotalOnlineTime, @MonthOnlineTime=MonthOnlineTime, @WeekOnlineTime = WeekOnlineTime, @DayOnlineTime = DayOnlineTime, @LastVisitDate = LastVisitDate FROM bx_Users WITH(NOLOCK) WHERE UserID=@UserID;
			
			IF (month(@LastVisitDate) <> month(@LastUpdateTime) OR year(@LastVisitDate) <> year(@LastUpdateTime))
				SET @MonthOnlineTime=@OnlineTime;
			ELSE IF(year(@LastUpdateTime)= year(getdate()) and month(@LastUpdateTime)= month(getdate()))
				SET @MonthOnlineTime=@MonthOnlineTime+@OnlineTime;
			ELSE
				SET @MonthOnlineTime=@OnlineTime;
				
			IF (day(@LastVisitDate) <> day(@LastUpdateTime) OR month(@LastVisitDate) <> month(@LastUpdateTime) OR year(@LastVisitDate) <> year(@LastUpdateTime))
				SET @DayOnlineTime=@OnlineTime;
			ELSE IF(year(@LastUpdateTime)= year(getdate()) and month(@LastUpdateTime)= month(getdate()) and day(@LastUpdateTime) = day(getdate()))
				SET @DayOnlineTime=@DayOnlineTime+@OnlineTime;
			else
				SET @DayOnlineTime=@OnlineTime;

			if @LastVisitDate >= @Monday AND @LastUpdateTime>=@Monday
				SET @WeekOnlineTime=@WeekOnlineTime+@OnlineTime;
			else
				SET @WeekOnlineTime=@OnlineTime;
				
				
			Update bx_Users SET TotalOnlineTime=TotalOnlineTime+@OnlineTime
								,MonthOnlineTime = @MonthOnlineTime
								,WeekOnlineTime = @WeekOnlineTime
								,DayOnlineTime = @DayOnlineTime
								,LastVisitDate=@LastUpdateTime WHERE UserID=@UserID;

			SET @NewOnlineTimes=@NewOnlineTimes+str(@TotalOnlineTime+@OnlineTime)+','
			SET @NewMonthOnlineTimes=@NewMonthOnlineTimes+str(@MonthOnlineTime)+','
			SET @NewWeekOnlineTimes = @NewWeekOnlineTimes + str(@WeekOnlineTime)+','
			SET @NewDayOnlineTimes = @NewDayOnlineTimes + str(@DayOnlineTime)+','
			
			SELECT @OnlineTimes = SUBSTRING(@OnlineTimes, @i + 1, LEN(@OnlineTimes) - @i)
			SELECT @UserIDs = SUBSTRING(@UserIDs, @j + 1, LEN(@UserIDs) - @j)

			SELECT @i = CHARINDEX(',',@OnlineTimes)
			SELECT @j = CHARINDEX(',',@UserIDs)
	END
	
	SELECT @NewOnlineTimes,@NewMonthOnlineTimes,@NewWeekOnlineTimes,@NewDayOnlineTimes
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AddMedalUsers')
	DROP PROCEDURE [bx_AddMedalUsers];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AddMedalUsers
     @MedalID        int
    ,@MedalLevelID   int
	,@UserIDs        varchar(8000)
    ,@EndDate        datetime
    ,@Url            nvarchar(200)
AS BEGIN

	SET NOCOUNT ON;

    BEGIN TRANSACTION    

    DECLARE @UserIDsTable table(ID int identity(1,1), UserID int);
    
    INSERT INTO @UserIDsTable (UserID) SELECT item FROM bx_GetIntTable(@UserIDs, ',');
    
    EXEC('DELETE [bx_UserMedals] WHERE MedalID='+@MedalID+' AND UserID in('+@UserIDs+')');
    IF(@@error<>0) BEGIN
        ROLLBACK TRANSACTION
        RETURN -1;
    END
    
    INSERT INTO [bx_UserMedals](MedalID,MedalLevelID,UserID,EndDate,Url) SELECT @MedalID,@MedalLevelID,UserID,@EndDate,@Url FROM @UserIDsTable;
    IF(@@error<>0) BEGIN
        ROLLBACK TRANSACTION
        RETURN -1;
    END

    COMMIT TRANSACTION

    RETURN 0;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AddMedalsToUser')
	DROP PROCEDURE [bx_AddMedalsToUser];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AddMedalsToUser
     @MedalIDs       varchar(8000)
    ,@MedalLevelIDs  varchar(8000)
	,@UserID         int
    ,@EndDates       varchar(8000)
AS BEGIN

	SET NOCOUNT ON;

    BEGIN TRANSACTION    

    DECLARE @tempTable table(TempID int identity(1,1), MedalID int, MedalLevelID int, EndDate datetime);
    
    INSERT INTO @tempTable (MedalID) SELECT item FROM bx_GetIntTable(@MedalIDs, ',');

    UPDATE @tempTable SET
			[MedalLevelID] = T.item
			FROM bx_GetIntTable(@MedalLevelIDs, N',') T
			WHERE TempID = T.id;

    UPDATE @tempTable SET
			[EndDate] = T.item
			FROM bx_GetStringTable_text(@EndDates, N',') T
			WHERE TempID = T.id;
    
    EXEC('DELETE [bx_UserMedals] WHERE UserID = ' + @UserID + ' AND MedalID in(' + @MedalIDs + ')');
    IF(@@error<>0) BEGIN
        ROLLBACK TRANSACTION
        RETURN -1;
    END
    
    INSERT INTO [bx_UserMedals](MedalID,MedalLevelID,UserID,EndDate) SELECT MedalID,MedalLevelID,@UserID,EndDate FROM @tempTable;
    IF(@@error<>0) BEGIN
        ROLLBACK TRANSACTION
        RETURN -1;
    END

    COMMIT TRANSACTION

    RETURN 0;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateUsersDatas')
	DROP PROCEDURE [bx_UpdateUsersDatas];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateUsersDatas
    @StartUserID            int,
    @UpdateCount            int,
    @UpdatePostCount        bit,
    @UpdateBlogCount        bit,
    @UpdateInviteCount      bit,
    @UpdateCommentCount     bit,
    @UpdatePictureCount     bit,
    @UpdateShareCount       bit,
    @UpdateDoingCount       bit,
    @UpdateDiskFileCount    bit
AS BEGIN
    SET NOCOUNT ON;
    
    EXEC('
    DECLARE @UserDatas table(TempID int identity(1,1),UID int,TopicCount int,PostCount int, BlogCount int,InviteCount int,
    CommentCount int,ShareCount int,CollectionCount int,AlbumCount int,PhotoCount int,DoingCount int,DiskFileCount int);

    INSERT INTO @UserDatas(UID)
    SELECT TOP '+ @UpdateCount + ' UserID FROM bx_Users WHERE UserID>='+@StartUserID+' AND [IsActive]=1;
    IF @@RowCount = 0
        SELECT -1;    

    IF '+ @UpdatePostCount + '  = 1 BEGIN
        UPDATE @UserDatas SET TopicCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,PostUserID FROM bx_Threads as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.PostUserID = U.UID WHERE ThreadStatus < 4  Group by PostUserID
        ) AS T RIGHT JOIN @UserDatas AS U ON UID=T.PostUserID;

        UPDATE @UserDatas SET PostCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_Posts as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  WHERE SortOrder<4000000000000000 Group by UserID
        ) AS T RIGHT JOIN @UserDatas AS U ON UID=T.UserID;
    END

    IF '+ @UpdateBlogCount + '  = 1
        UPDATE @UserDatas SET BlogCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_BlogArticles as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  WHERE IsApproved=1 Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

    IF '+ @UpdateInviteCount + '  = 1
        UPDATE @UserDatas SET InviteCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,InviterID FROM bx_UserInfos as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.InviterID = U.UID  Group by InviterID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.InviterID;

    IF '+ @UpdateCommentCount + '  = 1
        UPDATE @UserDatas SET CommentCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_Comments as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  WHERE IsApproved=1 Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

    IF '+ @UpdateShareCount + '  = 1 BEGIN
        UPDATE @UserDatas SET CollectionCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_UserShares as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID WHERE PrivacyType=2 Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

        UPDATE @UserDatas SET ShareCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_UserShares as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID WHERE PrivacyType<>2 Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;
    END

    IF '+ @UpdatePictureCount + '  = 1 BEGIN
        UPDATE @UserDatas SET AlbumCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_Albums as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

        UPDATE @UserDatas SET PhotoCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_Photos as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;
    END

    IF '+ @UpdateDoingCount + '  = 1
        UPDATE @UserDatas SET DoingCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_Doings as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID  Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

    IF '+ @UpdateDiskFileCount + '  = 1
        UPDATE @UserDatas SET DiskFileCount= ISNULL(T.TotalCount,0) FROM(
        SELECT COUNT(*) AS TotalCount,UserID FROM bx_DiskFiles as Temp WITH(NOLOCK) INNER JOIN @UserDatas as U  on Temp.UserID = U.UID Group by UserID
        ) AS T RIGHT JOIN @UserDatas ON UID=T.UserID;

    UPDATE bx_Users
    SET TotalInvite = ISNULL(InviteCount,TotalInvite)
        ,TotalTopics = ISNULL(TopicCount,TotalTopics)
        ,TotalPosts = ISNULL(PostCount,TotalPosts)
        ,TotalComments = ISNULL(CommentCount,TotalComments)
        ,TotalShares = ISNULL(ShareCount,TotalShares)
        ,TotalCollections = ISNULL(CollectionCount,TotalCollections)
        ,TotalBlogArticles = ISNULL(BlogCount,TotalBlogArticles)
        ,TotalAlbums = ISNULL(AlbumCount,TotalAlbums)
        ,TotalPhotos = ISNULL(PhotoCount,TotalPhotos)
        ,TotalDoings = ISNULL(DoingCount,TotalDoings)
    FROM @UserDatas
    WHERE UserID = UID;

    UPDATE bx_UserVars
    SET TotalDiskFiles = ISNULL(DiskFileCount,TotalDiskFiles)
    FROM @UserDatas
    WHERE UserID = UID;


    SELECT Max(UID)+1 FROM @UserDatas;
    ');
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ClearExpiresUserData')
	DROP PROCEDURE [bx_ClearExpiresUserData];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ClearExpiresUserData
AS
BEGIN

    SET NOCOUNT ON;

    DELETE FROM bx_UserRoles WHERE EndDate < GETDATE();

    DELETE FROM bx_UserMedals WHERE EndDate < GETDATE();

    DELETE FROM bx_Moderators WHERE EndDate < GETDATE();

    DELETE FROM bx_BannedUsers WHERE EndDate < GETDATE();

    DELETE FROM bx_Serials WHERE ExpiresDate < GETDATE();
    
    DELETE  FROM bx_SmsCodes WHERE CreateDate <= DATEADD(hour, -24, GETDATE());

    DELETE FROM bx_AdminSessions WHERE UpdateDate < DATEADD(minute, 0 - 60, GETDATE());

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_User_UpdateUserKeywords')
	DROP PROCEDURE [bx_User_UpdateUserKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_User_UpdateUserKeywords
    @UserID               int,
    @KeywordVersion       varchar(32),
    @Signature            nvarchar(1500),
    @SignatureReverter    nvarchar(4000)
AS BEGIN


    SET NOCOUNT ON;

    IF @Signature IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL
            UPDATE bx_Users SET Signature = @Signature, KeywordVersion = @KeywordVersion WHERE UserID = @UserID;
        ELSE
            UPDATE bx_Users SET Signature = @Signature WHERE UserID = @UserID;

    END

    IF @SignatureReverter IS NOT NULL BEGIN

        UPDATE bx_UserReverters SET SignatureReverter = @SignatureReverter WHERE UserID = @UserID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_UserReverters (UserID, SignatureReverter) VALUES (@UserID, @SignatureReverter);

    END


END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateUserGeneralPoint')
	DROP PROCEDURE [bx_UpdateUserGeneralPoint];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateUserGeneralPoint
     @UserID       int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MaxValue int;
	SET @MaxValue = 2147483647;
	SET ARITHABORT OFF;
	SET ANSI_WARNINGS OFF;
		
    UPDATE bx_Users SET Points = ISNULL([Point_1]+[Point_2]*10,@MaxValue) WHERE [UserID] = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_ExistsBlogArticle')
	DROP PROCEDURE [bx_Blog_ExistsBlogArticle];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_ExistsBlogArticle
	@ArticleID	int
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM bx_BlogArticles WHERE ArticleID = @ArticleID)
		SELECT 1;
	ELSE
		SELECT 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_GetPostBlogArticleCount')
	DROP PROCEDURE [bx_Blog_GetPostBlogArticleCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_GetPostBlogArticleCount
	@UserID		int,
	@BeginDate	datetime,
	@EndDate    datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_BlogArticles WHERE [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_GetCommentCountForArticle')
	DROP PROCEDURE [bx_Blog_GetCommentCountForArticle];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_GetCommentCountForArticle
	@UserID		int,
	@ArticleID	int,
	@BeginDate	datetime,
	@EndDate    datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_Comments WHERE [Type] = 2 AND [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate AND [TargetID] = @ArticleID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_GetBlogArticleCount')
	DROP PROCEDURE [bx_Blog_GetBlogArticleCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_GetBlogArticleCount
	@UserID			int,
	@TargetUserID	int,
	@BeginDate		datetime,
	@EndDate    datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_Comments WHERE [Type] = 2 AND [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate AND [TargetID] IN (
		SELECT ArticleID FROM bx_BlogArticles WHERE bx_BlogArticles.UserID = @TargetUserID
	);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_PostBlogArticle')
	DROP PROCEDURE [bx_Blog_PostBlogArticle];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_Blog_PostBlogArticle] 
      @UserID            int
     ,@CategoryID        int
     ,@IsApproved        bit
     ,@EnableComment     bit
     ,@PrivacyType       tinyint
     ,@CreateIP          varchar(50)
     ,@Thumb             nvarchar(200)
     ,@Subject           nvarchar(200)
     ,@Password          nvarchar(50)
     ,@Content           ntext
     ,@ArticleID         int                OUTPUT

AS BEGIN

	SET NOCOUNT ON; 
	
	BEGIN TRANSACTION
	
	IF NOT EXISTS (SELECT [CategoryID] FROM [bx_BlogCategories] WHERE [CategoryID] = @CategoryID AND [UserID] = @UserID) BEGIN  --判断是否有指定分类
	 
		DECLARE @CategoryCount int;
		IF EXISTS (SELECT * FROM [bx_BlogCategories] WHERE [UserID] = @UserID)
			SET @CategoryID = 0; --如果用户有日志分类,或只是传输了错误值的话,插入NULL
		ELSE BEGIN
			SET @CategoryID = 0; --使用刚自动新建的这个新分类
		END
	END
	
	IF(@@error<>0)
	GOTO Cleanup;	
		
	IF @ArticleID > 0 BEGIN --如果有传输日志ID,则表示编辑
	    
		UPDATE 
			[bx_BlogArticles]
		SET 
			[LastEditUserID] = @UserID
           ,[CategoryID] = @CategoryID
           ,[IsApproved] = @IsApproved
           ,[EnableComment] = @EnableComment
           ,[PrivacyType] = @PrivacyType
           ,[CreateIP] = @CreateIP
           ,[Thumb] = @Thumb
           ,[Subject] = @Subject
           ,[Password] = @Password
           ,[Content] = @Content
           ,[UpdateDate] = GETDATE()
           ,[KeywordVersion] = ''
        WHERE 
			ArticleID = @ArticleID;

    END
	ELSE BEGIN
	
		INSERT INTO [bx_BlogArticles](
			[UserID]
			,[LastEditUserID]
			,[CategoryID]
			,[IsApproved]
			,[EnableComment]
			,[PrivacyType]
			,[CreateIP]
			,[Thumb]
			,[Subject]
			,[Password]
			,[Content]
		) VALUES (
			@UserID
			,@UserID
			,@CategoryID
			,@IsApproved
			,@EnableComment
			,@PrivacyType
			,@CreateIP
			,@Thumb
			,@Subject
			,@Password
			,@Content
		);
		
		SET @ArticleID = @@IDENTITY;
	END
		  
	IF(@@error<>0)
		GOTO Cleanup;	
	ELSE BEGIN
		GOTO CommitTrans;
	END	   
		
	
CommitTrans:
	BEGIN
		COMMIT TRANSACTION
		RETURN (0);
	END
                    
Cleanup:
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_PostBlogArticle')
	DROP PROCEDURE [bx_Blog_PostBlogArticle];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_Blog_PostBlogArticle] 
      @UserID            int
     ,@CategoryID        int
     ,@IsApproved        bit
     ,@EnableComment     bit
     ,@PrivacyType       tinyint
     ,@CreateIP          varchar(50)
     ,@Thumb             nvarchar(200)
     ,@Subject           nvarchar(200)
     ,@Password          nvarchar(50)
     ,@Content           ntext
     ,@ArticleID         int                OUTPUT

AS BEGIN

	SET NOCOUNT ON; 
	
	BEGIN TRANSACTION
	
	IF NOT EXISTS (SELECT [CategoryID] FROM [bx_BlogCategories] WHERE [CategoryID] = @CategoryID AND [UserID] = @UserID) BEGIN  --判断是否有指定分类
	 
		DECLARE @CategoryCount int;
		IF EXISTS (SELECT * FROM [bx_BlogCategories] WHERE [UserID] = @UserID)
			SET @CategoryID = 0; --如果用户有日志分类,或只是传输了错误值的话,插入NULL
		ELSE BEGIN
			SET @CategoryID = 0; --使用刚自动新建的这个新分类
		END
	END
	
	IF(@@error<>0)
	GOTO Cleanup;	
		
	IF @ArticleID > 0 BEGIN --如果有传输日志ID,则表示编辑
	    
		UPDATE 
			[bx_BlogArticles]
		SET 
			[LastEditUserID] = @UserID
           ,[CategoryID] = @CategoryID
           ,[IsApproved] = @IsApproved
           ,[EnableComment] = @EnableComment
           ,[PrivacyType] = @PrivacyType
           ,[CreateIP] = @CreateIP
           ,[Thumb] = @Thumb
           ,[Subject] = @Subject
           ,[Password] = @Password
           ,[Content] = @Content
           ,[UpdateDate] = GETDATE()
           ,[KeywordVersion] = ''
        WHERE 
			ArticleID = @ArticleID;

    END
	ELSE BEGIN
	
		INSERT INTO [bx_BlogArticles](
			[UserID]
			,[LastEditUserID]
			,[CategoryID]
			,[IsApproved]
			,[EnableComment]
			,[PrivacyType]
			,[CreateIP]
			,[Thumb]
			,[Subject]
			,[Password]
			,[Content]
		) VALUES (
			@UserID
			,@UserID
			,@CategoryID
			,@IsApproved
			,@EnableComment
			,@PrivacyType
			,@CreateIP
			,@Thumb
			,@Subject
			,@Password
			,@Content
		);
		
		SET @ArticleID = @@IDENTITY;
	END
		  
	IF(@@error<>0)
		GOTO Cleanup;	
	ELSE BEGIN
		GOTO CommitTrans;
	END	   
		
	
CommitTrans:
	BEGIN
		COMMIT TRANSACTION
		RETURN (0);
	END
                    
Cleanup:
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_VisitBlogArticle')
	DROP PROCEDURE [bx_Blog_VisitBlogArticle];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_VisitBlogArticle
      @ArticleID            int
     ,@UserID               int
AS
BEGIN

	SET NOCOUNT ON; 

	BEGIN TRANSACTION
	
	UPDATE [bx_BlogArticles] SET [TotalViews] = [TotalViews] + 1 WHERE [ArticleID] = @ArticleID AND [UserID] <> @UserID;
	IF @@ROWCOUNT > 0 BEGIN
	
	    IF EXISTS (SELECT [UserID] FROM [bx_BlogArticleVisitors] WHERE [BlogArticleID] = @ArticleID AND [UserID] = @UserID)
			UPDATE [bx_BlogArticleVisitors] SET [ViewDate] = GETDATE() WHERE [BlogArticleID] = @ArticleID AND [UserID] = @UserID;
		ELSE BEGIN
			INSERT INTO [bx_BlogArticleVisitors] ([BlogArticleID], [UserID]) VALUES (@ArticleID, @UserID); --写入访问者本次的访问记录
			DELETE FROM [bx_BlogArticleVisitors] WHERE [BlogArticleID] = @ArticleID AND [UserID] NOT IN (SELECT TOP 10 [UserID] FROM [bx_BlogArticleVisitors] WHERE [BlogArticleID] = @ArticleID ORDER BY [ViewDate] DESC); --清除该日志不在前10条的访问记录
		END

	END
	
	IF(@@error<>0)
		GOTO Cleanup;

	GOTO CommitTrans;
	
	
CommitTrans:
	BEGIN
		COMMIT TRANSACTION
		RETURN (0);
	END
                    
Cleanup:
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_GetBlogArticle')
	DROP PROCEDURE [bx_Blog_GetBlogArticle];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_GetBlogArticle
    @ArticleID      int
AS BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM
        [bx_Tags]
    WHERE
        [ID] IN (SELECT [TagID] FROM [bx_TagRelation] WHERE [Type] = 1 AND [TargetID] = @ArticleID)
    AND 
        [IsLock] = 0;

    SELECT * FROM [bx_BlogArticleVisitors] WHERE  [BlogArticleID] = @ArticleID ORDER BY [ViewDate] DESC;

    SELECT * FROM [bx_BlogArticles] WHERE [ArticleID] = @ArticleID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_CreateBlogCategory')
	DROP PROCEDURE [bx_Blog_CreateBlogCategory];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_CreateBlogCategory
    @UserID          int
   ,@Name            nvarchar(50)
AS BEGIN
    SET NOCOUNT ON;

	IF NOT EXISTS (SELECT * FROM bx_BlogCategories WHERE UserID = @UserID AND Name = @Name) BEGIN

		INSERT INTO [bx_BlogCategories]([UserID],[Name]) VALUES (@UserID, @Name);
	END
	ELSE BEGIN
		
		SELECT -1;
	END

    SELECT CAST(@@IDENTITY AS int)
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_EditBlogCategory')
	DROP PROCEDURE [bx_Blog_EditBlogCategory];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_EditBlogCategory
    @CategoryID      int
   ,@Name            nvarchar(50)
AS BEGIN
    SET NOCOUNT ON;

    UPDATE 
        [bx_BlogCategories]
    SET 
        [Name] = @Name,
		[KeywordVersion] = ''
    WHERE 
        [CategoryID] = @CategoryID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_GetBlogCategory')
	DROP PROCEDURE [bx_Blog_GetBlogCategory];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_GetBlogCategory
    @CategoryID     int
AS BEGIN
    SET NOCOUNT ON;

    SELECT * FROM [bx_BlogCategories] WHERE [CategoryID] = @CategoryID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_GetUserBlogCategories')
	DROP PROCEDURE [bx_Blog_GetUserBlogCategories];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_GetUserBlogCategories
    @UserID          int
AS BEGIN
    SET NOCOUNT ON;

    SELECT * FROM [bx_BlogCategories] WHERE  [UserID] = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_UpdateBlogArticleKeywords')
	DROP PROCEDURE [bx_Blog_UpdateBlogArticleKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_UpdateBlogArticleKeywords
    @ArticleID         int,
    @KeywordVersion    varchar(32),
    @Subject           nvarchar(200),
    @SubjectReverter   nvarchar(4000),
    @Content           ntext,
    @ContentReverter   ntext
AS BEGIN

    SET NOCOUNT ON;

    IF @Subject IS NOT NULL OR @Content IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL BEGIN

            IF @Subject IS NOT NULL AND @Content IS NOT NULL
                UPDATE bx_BlogArticles SET Subject = @Subject, Content = @Content, KeywordVersion = @KeywordVersion WHERE ArticleID = @ArticleID;
            ELSE IF @Subject IS NOT NULL
                UPDATE bx_BlogArticles SET Subject = @Subject, KeywordVersion = @KeywordVersion WHERE ArticleID = @ArticleID;
            ELSE
                UPDATE bx_BlogArticles SET Content = @Content, KeywordVersion = @KeywordVersion WHERE ArticleID = @ArticleID;

        END
        ELSE BEGIN

           IF @Subject IS NOT NULL AND @Content IS NOT NULL
                UPDATE bx_BlogArticles SET Subject = @Subject, Content = @Content WHERE ArticleID = @ArticleID;
            ELSE IF @Subject IS NOT NULL
                UPDATE bx_BlogArticles SET Subject = @Subject WHERE ArticleID = @ArticleID;
            ELSE
                UPDATE bx_BlogArticles SET Content = @Content WHERE ArticleID = @ArticleID;

        END

    END

    IF (@SubjectReverter IS NOT NULL AND @ContentReverter IS NOT NULL) BEGIN

        UPDATE bx_BlogArticleReverters SET SubjectReverter = @SubjectReverter, ContentReverter = @ContentReverter WHERE ArticleID = @ArticleID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_BlogArticleReverters (ArticleID, SubjectReverter, ContentReverter) VALUES (@ArticleID, @SubjectReverter, @ContentReverter);

    END
    ELSE IF (@SubjectReverter IS NOT NULL) BEGIN

        UPDATE bx_BlogArticleReverters SET SubjectReverter = @SubjectReverter WHERE ArticleID = @ArticleID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_BlogArticleReverters (ArticleID, SubjectReverter, ContentReverter) VALUES (@ArticleID, @SubjectReverter, N'');

    END
    ELSE IF (@ContentReverter IS NOT NULL) BEGIN

        UPDATE bx_BlogArticleReverters SET ContentReverter = @ContentReverter WHERE ArticleID = @ArticleID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_BlogArticleReverters (ArticleID, SubjectReverter, ContentReverter) VALUES (@ArticleID, N'', @ContentReverter);

    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Blog_UpdateBlogCategoryKeywords')
	DROP PROCEDURE [bx_Blog_UpdateBlogCategoryKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Blog_UpdateBlogCategoryKeywords
    @CategoryID            int,
    @KeywordVersion        varchar(32),
    @Name                  nvarchar(50),
    @NameReverter          nvarchar(4000)
AS BEGIN


    SET NOCOUNT ON;

    IF @Name IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL
            UPDATE bx_BlogCategories SET Name = @Name, KeywordVersion = @KeywordVersion WHERE CategoryID = @CategoryID;
        ELSE
            UPDATE bx_BlogCategories SET Name = @Name WHERE CategoryID = @CategoryID;

    END

    IF @NameReverter IS NOT NULL BEGIN

        UPDATE bx_BlogCategoryReverters SET NameReverter = @NameReverter WHERE CategoryID = @CategoryID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_BlogCategoryReverters (CategoryID, NameReverter) VALUES (@CategoryID, @NameReverter);

    END


END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Tag_GetUserBlogTags')
	DROP PROCEDURE [bx_Tag_GetUserBlogTags];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Tag_GetUserBlogTags 
	@UserID int
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM
        [bx_Tags]
    WHERE
        [ID] IN (
			SELECT [TagID] FROM [bx_TagRelation] WHERE [Type] = 1 AND [TargetID] IN (
				SELECT [ArticleID] FROM [bx_BlogArticles] WHERE [UserID] = @UserID
			)
		)
    AND 
        [IsLock] = 0;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetTag')
	DROP PROCEDURE [bx_GetTag];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetTag
    @ID          int
AS BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [bx_Tags] WHERE [ID] = @ID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetTagsByTargetID')
	DROP PROCEDURE [bx_GetTagsByTargetID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetTagsByTargetID
    @Type      tinyint,
    @TargetID  int
AS BEGIN
    SET NOCOUNT ON;
    SELECT * FROM
        [bx_Tags]
    WHERE
        [ID] IN (SELECT [TagID] FROM [bx_TagRelation] WHERE [Type] = @Type AND [TargetID] = @TargetID)
    AND 
        [IsLock] = 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_LockTag')
	DROP PROCEDURE [bx_LockTag];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_LockTag
     @ID       int
AS BEGIN
    SET NOCOUNT ON;
    UPDATE [bx_Tags] SET [IsLock] = 1 WHERE [ID] = @ID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteTag')
	DROP PROCEDURE [bx_DeleteTag];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteTag
     @ID       int
AS BEGIN
    SET NOCOUNT ON;
    DELETE FROM [bx_Tags] WHERE [ID] = @ID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_LoadAllSettings')
	DROP PROCEDURE [bx_LoadAllSettings];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_LoadAllSettings
AS
BEGIN

    SET NOCOUNT ON;

    SELECT * FROM [bx_Settings] ORDER BY [TypeName];

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CheckOverFlowBuyCount')
	DROP PROCEDURE [bx_CheckOverFlowBuyCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CheckOverFlowBuyCount
    @UserID            int,
    @BeginTime         datetime,
    @EndTime           datetime,
    @BuyCount          int
AS BEGIN

    SET NOCOUNT ON;

    IF (SELECT COUNT(*) FROM bx_InviteSerials WHERE UserID = @UserID AND CreateDate BETWEEN @BeginTime AND @EndTime)>=@BuyCount
        SELECT 1;
    ELSE 
        SELECT 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_BuyInviteSerial')
	DROP PROCEDURE [bx_BuyInviteSerial];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_BuyInviteSerial 
    @UserID        int,
    @BuyCount      int,
    @ExpiresDate   datetime,
    @Remark        varchar(200)
AS BEGIN
    SET NOCOUNT ON;
    DECLARE @i int;
    SET @i = 0;
    WHILE @i < @BuyCount
    BEGIN
        INSERT INTO bx_InviteSerials (UserID, ExpiresDate, Status,Remark) VALUES (@UserID, @ExpiresDate, 0,@Remark);
        SET @i = @i + 1;
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UserInviteSerialStatInfo')
	DROP PROCEDURE [bx_UserInviteSerialStatInfo];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UserInviteSerialStatInfo 
@UserID      int 
AS
BEGIN
SET NOCOUNT ON;
SELECT * FROM bx_SerialCounter WHERE UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateInviteSerials')
	DROP PROCEDURE [bx_CreateInviteSerials];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateInviteSerials
     @AddNum       int
    ,@UserIDs      varchar(8000)
    ,@ExpiresDate  datetime
    ,@Remark       varchar(200)
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @i int;
    SET @i = 0;
    WHILE @i < @AddNum BEGIN

        INSERT INTO [bx_InviteSerials](UserID,ExpiresDate,Remark)
            SELECT item, @ExpiresDate,@Remark FROM bx_GetIntTable(@UserIDs, ',');
        SET @i=@i+1;
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetInviteSerialByToEmail')
	DROP PROCEDURE [bx_GetInviteSerialByToEmail];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetInviteSerialByToEmail
    @ToEmail     nvarchar(200)
AS BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 * FROM [bx_InviteSerials] WHERE ToEmail = @ToEmail;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetInviteSerialByToUserID')
	DROP PROCEDURE [bx_GetInviteSerialByToUserID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetInviteSerialByToUserID
    @ToUserID     int
AS BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 * FROM [bx_InviteSerials] WHERE ToUserID=@ToUserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetInviteSerialBySerial')
	DROP PROCEDURE [bx_GetInviteSerialBySerial];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetInviteSerialBySerial
    @Serial     uniqueidentifier
AS BEGIN
    SET NOCOUNT ON;

    SELECT * FROM [bx_InviteSerials] WHERE Serial = @Serial;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetInviteRelation')
	DROP PROCEDURE [bx_GetInviteRelation];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetInviteRelation
    @UserID     int
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @tempID int,@Serial uniqueidentifier;
    DECLARE @T table(Serial uniqueidentifier)
    WHILE @UserID>0
    BEGIN
	    SET @tempID=0
	    SELECT TOP 1 @tempID=UserID,@Serial=Serial FROM bx_InviteSerials WHERE ToUserID=@UserID
	    SET @UserID=@tempID
	    IF(@UserID>0)
		    INSERT INTO @T SELECT @Serial
    END
    SELECT * FROM bx_InviteSerials WHERE [Serial] IN(SELECT Serial FROM @T)
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetUserInviteSerial')
	DROP PROCEDURE [bx_SetUserInviteSerial];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetUserInviteSerial
 @Serial           uniqueidentifier
,@UserID           int
,@InviteID         int
AS
BEGIN

    SET NOCOUNT ON;

    IF @Serial IS NOT NULL AND @Serial <> '00000000-0000-0000-0000-000000000000'  BEGIN
        UPDATE bx_InviteSerials SET ToUserID = @UserID, [Status] = 1 WHERE Serial = @Serial
        SELECT @InviteID = UserID FROM bx_InviteSerials WHERE Serial = @Serial
    END

    IF @InviteID <> @UserID  BEGIN
        UPDATE bx_UserInfos SET InviterID = @InviteID WHERE [UserID] = @UserID
        UPDATE bx_Users SET [TotalInvite] = [TotalInvite] + 1 WHERE [UserID] = @InviteID
        IF NOT EXISTS(SELECT * FROM bx_Friends WHERE UserID = @UserID AND FriendUserID = @InviteID)
	        INSERT INTO bx_Friends([UserID],[FriendUserID],[GroupID],[Hot],[CreateDate]) 
	        VALUES(@UserID,@InviteID,0,0,GETDATE());

        IF NOT EXISTS(SELECT * FROM bx_Friends WHERE UserID = @InviteID AND FriendUserID = @UserID)
	        INSERT INTO bx_Friends([UserID],[FriendUserID],[GroupID],[Hot],[CreateDate]) 
	        VALUES(@InviteID,@UserID,0,0,GETDATE());

    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAllForums')
	DROP PROCEDURE [bx_GetAllForums];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAllForums
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_Forums WITH (NOLOCK) WHERE [ClubID]=0 ORDER BY [ParentID],[SortOrder] ASC;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_CreateForum')
	DROP PROCEDURE [bx_v5_CreateForum];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_CreateForum
	@ParentID int,
	@ForumType tinyint,
	@CodeName nvarchar(128),
	@ForumName nvarchar(1024),
	@Description ntext,
	@Readme ntext,
	@LogoUrl nvarchar(256),
	@ThemeID nvarchar(64),
	@Password nvarchar(64),
	@ExtendedAttributes [ntext],
	@ForumID int output,
	@ThreadCatalogStatus tinyint,
	@ColumnSpan tinyint,
	@SortOrder int
AS
	SET NOCOUNT ON 
	
	IF(EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE CodeName=@CodeName))
	begin
	set @ForumID = 0
	RETURN (13)	
	end
	
	IF ((@ParentID=0) OR (EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE ForumID=@ParentID)) )
	BEGIN
	
	DECLARE @Condition varchar(50)
	SET @Condition='ParentID='+str(@ParentID)
	
	INSERT INTO [bx_Forums] (
	[ParentID],
	[ForumType],
	[ThreadCatalogStatus],
	[CodeName],
	[ForumName],
	[Description],
	[Readme],
	[LogoSrc],
	[ThemeID],
	[Password],
	[SortOrder],
	[ExtendedAttributes],
	[ColumnSpan]
) VALUES (
	@ParentID,
	@ForumType,
	@ThreadCatalogStatus,
	@CodeName,
	@ForumName,
	@Description,
	@Readme,
	@LogoUrl,
	@ThemeID,
	@Password,
	@SortOrder,
	@ExtendedAttributes,
	@ColumnSpan
)
		set @ForumID = @@IDENTITY;
		RETURN (0)
	END
	
	ELSE
	begin
	set @ForumID = 0
		RETURN (-1)
		end
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_UpdateForum')
	DROP PROCEDURE [bx_v5_UpdateForum];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_UpdateForum
	@ForumID int,
	@ForumType tinyint,
	@CodeName nvarchar(128),
	@ForumName nvarchar(1024),
	@Description ntext,
	@Readme ntext,
	@LogoUrl nvarchar(256),
	@ThemeID nvarchar(64),
	@Password nvarchar(64),
	@ExtendedAttributes [ntext],
	@ColumnSpan tinyint,
	@SortOrder int
AS
	SET NOCOUNT ON 
	IF(EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE CodeName=@CodeName AND ForumID <> @ForumID))
	RETURN (13)	

	
	UPDATE [bx_Forums] SET
	[ForumType] = @ForumType,
	[CodeName] = @CodeName,
	[ForumName] = @ForumName,
	[Description] = @Description,
	[Readme] = @Readme,
	[LogoSrc] = @LogoUrl,
	[ThemeID] = @ThemeID,
	[Password] = @Password,
	[ExtendedAttributes] = @ExtendedAttributes,
	[ColumnSpan] = @ColumnSpan,
	[SortOrder] = @SortOrder
WHERE
	[ForumID] = @ForumID
	
	RETURN (0)
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_DeleteForum')
	DROP PROCEDURE [bx_v5_DeleteForum];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_DeleteForum
	@ForumID int
AS
	SET NOCOUNT ON
	
	BEGIN TRANSACTION
	DECLARE @SortOrder int,@ParentID int,@ErrorNum int
	SET @ErrorNum=0
	
	DELETE [bx_Threads] WHERE ForumID=@ForumID 
	IF @@error<>0
		SET @ErrorNum=@ErrorNum+1
		
	
	SELECT @ParentID=ParentID FROM [bx_Forums] WITH(NOLOCK) WHERE ForumID=@ForumID 
	SELECT @SortOrder=MAX(SortOrder)+1 FROM [bx_Forums] WITH(NOLOCK) WHERE ParentID=@ParentID  
	IF @SortOrder IS NULL
			SET @SortOrder=0
			
	UPDATE [bx_Forums] SET ParentID=@ParentID,SortOrder=SortOrder+@SortOrder WHERE ParentID=@ForumID
	IF @@error<>0
		SET @ErrorNum=@ErrorNum+1

    DELETE [bx_StickThreads] WHERE ForumID=@ForumID;
    IF @@error<>0
		SET @ErrorNum=@ErrorNum+1 	

	DELETE [bx_Forums] WHERE ForumID=@ForumID;
	IF @@error<>0
		SET @ErrorNum=@ErrorNum+1
		
	IF @ErrorNum=0
		BEGIN
			COMMIT TRANSACTION
			RETURN (0)
		END
		ELSE
		BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
		END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateForumReadme')
	DROP PROCEDURE [bx_UpdateForumReadme];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateForumReadme
    @ForumID int,
    @Readme  ntext
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE bx_Forums SET Readme=@Readme WHERE ForumID=@ForumID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateForumData')
	DROP PROCEDURE [bx_UpdateForumData];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateForumData
	@ForumID int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @TotalThreads int;
	DECLARE @TotalPosts int,@LastThreadID INT--@TodayThreads int,@TodayPosts int;

	SELECT @TotalThreads = COUNT(*) FROM [bx_Threads] WITH (NOLOCK) WHERE [ForumID] = @ForumID AND [ThreadStatus] < 4;
	SELECT @TotalPosts = COUNT(*) FROM [bx_Posts] WITH (NOLOCK)  WHERE ForumID = @ForumID AND [SortOrder] < 4000000000000000;
	SELECT @LastThreadID=ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WITH (NOLOCK) WHERE T1.ForumID=@ForumID AND T1.SortOrder < 4000000000000000)
	
	UPDATE [bx_Forums] SET [TotalThreads] = @TotalThreads, [TotalPosts] = @TotalPosts, [LastThreadID]=ISNULL(@LastThreadID,0) WHERE [ForumID] = @ForumID;
	
	SELECT * FROM bx_Forums WITH (NOLOCK) WHERE ForumID = @ForumID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ResetTodayPosts')
	DROP PROCEDURE [bx_ResetTodayPosts];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ResetTodayPosts
AS
BEGIN
	SET NOCOUNT ON;
    
    DECLARE @Today datetime;
    SELECT @Today = CONVERT(DATETIME, CONVERT(varchar(100), GETDATE(), 23));

    UPDATE bx_Forums SET  
         TodayPosts = (SELECT COUNT(*) FROM bx_Posts P WITH(NOLOCK) WHERE ForumID = F.ForumID AND P.SortOrder<4000000000000000 AND P.CreateDate > @Today)
        ,TodayThreads = (SELECT COUNT(*) FROM bx_Threads T WITH(NOLOCK) WHERE ForumID = F.ForumID AND ThreadStatus<4 AND T.CreateDate > @Today)
        FROM bx_Forums F
        WHERE ForumID = F.ForumID

	UPDATE bx_Forums SET 
		YestodayLastThreadID=(SELECT ISNULL(MAX(ThreadID),0) FROM [bx_Threads] T WITH (NOLOCK) WHERE ForumID=F.ForumID AND ThreadStatus<4 AND T.CreateDate<@Today),
		YestodayLastPostID=(SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T2 WITH(NOLOCK) ON P.ThreadID=T2.ThreadID WHERE T2.ForumID=F.ForumID AND P.SortOrder<4000000000000000 AND T2.ThreadStatus<4 AND P.CreateDate<@Today)
		FROM [bx_Forums] F WHERE ForumID=F.ForumID;
/*    
    DECLARE @Today int;
    DECLARE @Yestoday datetime;
    SET @Yestoday = DATEADD(day, -1, GETDATE());
    SELECT @Today = DATEPART(year,@Yestoday)*10000+DATEPART(month,@Yestoday)*100+DATEPART(day,@Yestoday);

    DELETE [bx_DayLastThreads] WHERE [Day] = @Today;
    INSERT INTO [bx_DayLastThreads]([Day],[LastThreadID]) 
	SELECT @Today,(SELECT ISNULL(MAX(ThreadID),0) FROM bx_Threads T WITH(NOLOCK) WHERE CreateDate< CONVERT(DATETIME, CONVERT(varchar(100), GETDATE(), 23)));
*/
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_MoveForum')
	DROP PROCEDURE [bx_MoveForum];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_MoveForum
    @forumID int,
    @parentID int
AS
SET NOCOUNT ON;
IF (EXISTS(SELECT * FROM [bx_Forums] WITH(NOLOCK) WHERE ForumID=@parentID ) or @parentID=0)
	BEGIN
		DECLARE @MaxSortOrder int
		SELECT @MaxSortOrder=MAX(SortOrder) FROM bx_Forums WITH(NOLOCK) WHERE ParentID=@parentID
		IF @MaxSortOrder IS NULL
			SET @MaxSortOrder=0
		UPDATE bx_Forums 
		SET ParentID=@parentID, SortOrder=@MaxSortOrder+1
		WHERE ForumID = @forumID
		RETURN (0)
	END
ELSE
	RETURN (1)
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateForumStatus')
	DROP PROCEDURE [bx_UpdateForumStatus];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateForumStatus
	@ForumIdentities varchar(8000),
	@ForumStatus tinyint
AS
	SET NOCOUNT ON

	BEGIN TRANSACTION
	
	IF @ForumStatus<2 BEGIN
		EXEC ('Alter PROCEDURE bx_GetDisabledTriggerForumIDs
				@ForumIDs nvarchar(64) output
			AS
			BEGIN
				SET NOCOUNT ON;
				set @ForumIDs='''';
			END')
	END
	ELSE BEGIN

		EXEC ('Alter PROCEDURE bx_GetDisabledTriggerForumIDs
				@ForumIDs nvarchar(64) output
			AS
			BEGIN
				SET NOCOUNT ON;
				set @ForumIDs='''+@ForumIdentities+''';
			END')
	END

	IF @@ERROR<>0 BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

	EXEC ('Update [bx_Forums] SET ForumStatus='+@ForumStatus+' WHERE [ForumID] IN (' + @ForumIdentities + ') ') 
	
	IF @@ERROR<>0 BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

COMMIT TRANSACTION
RETURN 0;
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteForumThreads')
	DROP PROCEDURE [bx_DeleteForumThreads];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteForumThreads
	@ForumID int,
	@DeleteCount int
AS
BEGIN
	SET NOCOUNT ON;

	EXEC (N'DELETE bx_Threads WHERE ThreadID IN (SELECT TOP ' + @DeleteCount + N' ThreadID FROM bx_Threads WITH (NOLOCK) WHERE ForumID = ' + @ForumID + N')');
	RETURN @@ROWCOUNT;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_MoveForumThreads')
	DROP PROCEDURE [bx_MoveForumThreads];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_MoveForumThreads
	@OldForumID int,
	@NewForumID int,
	@MoveCount int
AS
BEGIN
	SET NOCOUNT ON 
	EXEC (N'Update bx_Threads SET ForumID='+@NewForumID+' WHERE ThreadID IN (SELECT TOP ' + @MoveCount + N' ThreadID FROM bx_Threads WITH (NOLOCK) WHERE ForumID = ' + @OldForumID + N')');
	RETURN @@ROWCOUNT;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAllThreadCatalogs')
	DROP PROCEDURE [bx_GetAllThreadCatalogs];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAllThreadCatalogs
AS
BEGIN
	SET NOCOUNT ON
	SELECT * FROM bx_ThreadCatalogs WHERE ThreadCatalogID<>0
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetThreadCatalogsInForums')
	DROP PROCEDURE [bx_GetThreadCatalogsInForums];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetThreadCatalogsInForums
AS
BEGIN
	SET NOCOUNT ON
	SELECT * FROM [bx_ThreadCatalogsInForums] ORDER BY SortOrder ASC
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteForumThreadCatalog')
	DROP PROCEDURE [bx_DeleteForumThreadCatalog];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteForumThreadCatalog
    @ForumID            int,
    @ThreadCatalogID    int
AS BEGIN
    SET NOCOUNT ON;
    
    DELETE [bx_ThreadCatalogsInForums] WHERE [ForumID]=@ForumID AND [ThreadCatalogID]=@ThreadCatalogID;

    IF NOT EXISTS(SELECT * FROM [bx_ThreadCatalogsInForums] WHERE [ThreadCatalogID]=@ThreadCatalogID)
        DELETE [bx_ThreadCatalogs] WHERE [ThreadCatalogID] = @ThreadCatalogID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateForumThreadCatalogStatus')
	DROP PROCEDURE [bx_UpdateForumThreadCatalogStatus];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateForumThreadCatalogStatus
    @ForumID                int,
    @ThreadCatalogStatus    tinyint
AS BEGIN
    SET NOCOUNT ON;
    
    UPDATE [bx_Forums] SET ThreadCatalogStatus = @ThreadCatalogStatus WHERE ForumID = @ForumID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AddThreadCatalogToForum')
	DROP PROCEDURE [bx_AddThreadCatalogToForum];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AddThreadCatalogToForum
	@ForumID int,
	@ThreadCatalogIDs varchar(8000),
	@SortOrders varchar(8000)
AS
BEGIN

	SET NOCOUNT ON;
	
	BEGIN TRANSACTION
	IF EXISTS (SELECT * FROM bx_ThreadCatalogsInForums WITH (NOLOCK) WHERE ForumID = @ForumID)
		delete bx_ThreadCatalogsInForums where ForumID=@ForumID
		IF(@@error<>0)
		BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
		END
		DECLARE @ThreadCatalogID int,@i int,@SortOrder int,@Condition varchar(100),@j int
	
		SET @SortOrders=@SortOrders+N','
		SELECT @j=CHARINDEX(',',@SortOrders)
		
		SET @ThreadCatalogIDs=@ThreadCatalogIDs+N','
		SET @Condition='ForumID='+str(@ForumID)
		SELECT @i=CHARINDEX(',',@ThreadCatalogIDs)
		
		WHILE(@i>1)
		BEGIN
			SELECT @ThreadCatalogID=SUBSTRING(@ThreadCatalogIDs,0, @i)
			SELECT @SortOrder=SUBSTRING(@SortOrders,0, @j)
			INSERT INTO bx_ThreadCatalogsInForums (ForumID, ThreadCatalogID,SortOrder)
				VALUES (@ForumID, @ThreadCatalogID,@SortOrder);
			
		
			SELECT @ThreadCatalogIDs=SUBSTRING(@ThreadCatalogIDs,@i+1,LEN(@ThreadCatalogIDs)-@i)
			SELECT @i=CHARINDEX(',',@ThreadCatalogIDs)
			
			SELECT @SortOrders=SUBSTRING(@SortOrders,@j+1,LEN(@SortOrders)-@j)
			SELECT @j=CHARINDEX(',',@SortOrders)
		END
		IF(@@error<>0)
				BEGIN
					ROLLBACK TRANSACTION
					RETURN (-1)
				END
		
		COMMIT TRANSACTION
		RETURN (0);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_UpdateForumThreadCatalogData')
	DROP PROCEDURE [bx_v5_UpdateForumThreadCatalogData];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_UpdateForumThreadCatalogData
	 @ForumID            int
    ,@ThreadCatalogID    int
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [bx_ThreadCatalogsInForums] SET TotalThreads=(
		SELECT COUNT(1) FROM bx_Threads WITH(NOLOCK) WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID AND ThreadStatus<4
		) WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAllModerators')
	DROP PROCEDURE [bx_GetAllModerators];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAllModerators
AS
BEGIN
	SET NOCOUNT ON
	SELECT * FROM [bx_Moderators] WHERE EndDate>GETDATE() ORDER BY ForumID, SortOrder ASC;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Comment_UpdateCommentKeywords')
	DROP PROCEDURE [bx_Comment_UpdateCommentKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Comment_UpdateCommentKeywords
    @CommentID                int,
    @KeywordVersion           varchar(32),
    @Content                  nvarchar(3000),
    @ContentReverter          ntext
AS BEGIN


    SET NOCOUNT ON;

    IF @Content IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL
            UPDATE bx_Comments SET Content = @Content, KeywordVersion = @KeywordVersion WHERE CommentID = @CommentID;
        ELSE
            UPDATE bx_Comments SET Content = @Content WHERE CommentID = @CommentID;

    END

    IF @ContentReverter IS NOT NULL BEGIN

        UPDATE bx_CommentReverters SET ContentReverter = @ContentReverter WHERE CommentID = @CommentID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_CommentReverters (CommentID, ContentReverter) VALUES (@CommentID, @ContentReverter);

    END


END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_CreateAlbum')
	DROP PROCEDURE [bx_Album_CreateAlbum];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_CreateAlbum
      @UserID         int
     ,@PrivacyType    tinyint
     ,@Name           nvarchar(50)
    ,@Description       nvarchar(100)
     ,@Cover          nvarchar(200)
     ,@Password       nvarchar(50)
AS BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS(SELECT * FROM bx_Albums WHERE UserID = @UserID AND Name = @Name) BEGIN
        SELECT -1;
        RETURN;
    END

    INSERT INTO
        [bx_Albums](
             [UserID]
            ,[PrivacyType]
            ,[Name]
            ,[Description]
            ,[Cover]
            ,[Password]
			,[LastEditUserID]
        ) VALUES (
             @UserID
            ,@PrivacyType
            ,@Name
            ,@Description
            ,@Cover
            ,@Password
			,@UserID
        );

    SELECT @@IDENTITY;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_UpdateAlbum')
	DROP PROCEDURE [bx_Album_UpdateAlbum];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_UpdateAlbum
     @AlbumID              int
    ,@Name            nvarchar(50)
    ,@Description       nvarchar(100)
    ,@PrivacyType     tinyint
    ,@Password        nvarchar(50)
    ,@LastEditUserID    int
AS BEGIN
    SET NOCOUNT ON;

    UPDATE 
        [bx_Albums]
    SET
        [Name] = @Name
        ,[Description] = @Description
       ,[KeywordVersion] = N''
       ,[PrivacyType] = @PrivacyType
       ,[Password] = @Password
       ,[UpdateDate] = GETDATE()
       ,[LastEditUserID] = @LastEditUserID
    WHERE
        [AlbumID] = @AlbumID;

	UPDATE
		[bx_Photos]
	SET
		[LastEditUserID] = @LastEditUserID
	WHERE
		[AlbumID] = @AlbumID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_UpdateAlbumCover')
	DROP PROCEDURE [bx_Album_UpdateAlbumCover];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_UpdateAlbumCover
     @AlbumID           int
    ,@PhotoID           int
    ,@LastEditUserID    int
AS BEGIN
    SET NOCOUNT ON;

    UPDATE [bx_Albums] SET [Cover]=[ThumbPath],[LastEditUserID]=@LastEditUserID,[CoverPhotoID]=@PhotoID FROM [bx_Photos] WHERE [bx_Albums].[AlbumID]=@AlbumID AND [PhotoID]=@PhotoID;
     
	UPDATE [bx_Photos] SET [LastEditUserID] = @LastEditUserID WHERE [AlbumID] = @AlbumID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_GetAlbum')
	DROP PROCEDURE [bx_Album_GetAlbum];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_GetAlbum
    @AlbumID              int
AS BEGIN
    SET NOCOUNT ON;

    SELECT 
        *
    FROM
        [bx_Albums]
    WHERE
        [AlbumID]=@AlbumID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_GetAlbumPhotoIDs')
	DROP PROCEDURE [bx_Album_GetAlbumPhotoIDs];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_GetAlbumPhotoIDs
    @AlbumID              int
AS BEGIN
    SET NOCOUNT ON;

    SELECT [PhotoID] FROM [bx_Photos] WHERE [AlbumID] = @AlbumID ORDER BY [PhotoID] DESC;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_GetUserAlbums_All')
	DROP PROCEDURE [bx_Album_GetUserAlbums_All];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_GetUserAlbums_All
    @UserID              int
AS BEGIN
    SET NOCOUNT ON;

    SELECT * FROM [bx_Albums] WHERE [UserID] = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_CreatePhotos')
	DROP PROCEDURE [bx_Album_CreatePhotos];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_CreatePhotos
	@AlbumID				int
	,@UserID				int
	,@UserIP				varchar(50)
	,@PhotoNames			ntext
	,@PhotoDescriptions		ntext
	,@PhotoFileTypes        text
	,@PhotoFileIds			text
    ,@PhotoFileSizes	    text
AS BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM [bx_Albums] WHERE [UserID]=@UserID AND [AlbumID]=@AlbumID) BEGIN

		IF DATALENGTH(@PhotoFileIds) > 0 BEGIN

			DECLARE @PhotoTable table(TempID int identity(1,1), Name nvarchar(50), Description nvarchar(1500) default(''), Type varchar(10), FileID varchar(50), FileSize bigint);

			INSERT INTO @PhotoTable (Name) SELECT item FROM bx_GetStringTable_ntext(@PhotoNames, N'/');

			UPDATE @PhotoTable SET [Description] = T.item FROM bx_GetStringTable_ntext(@PhotoDescriptions, N'/') T WHERE TempID = T.id;

			UPDATE @PhotoTable SET [Type] = T.item FROM bx_GetStringTable_text(@PhotoFileTypes, '/') T WHERE TempID = T.id;

			UPDATE @PhotoTable SET FileID = T.item FROM bx_GetStringTable_text(@PhotoFileIds, '/') T WHERE TempID = T.id;

            UPDATE @PhotoTable SET FileSize = T.item FROM bx_GetStringTable_text(@PhotoFileSizes, '/') T WHERE TempID = T.id;

			INSERT INTO [bx_Photos] ([AlbumID],[UserID],[CreateIP],[Name],[Description],[FileType],[FileID],[FileSize],[LastEditUserID])
			SELECT @AlbumID, @UserID, @UserIP, A.[Name], A.[Description], A.[Type], A.[FileID], A.[FileSize], @UserID FROM @PhotoTable AS A;
		END;

		RETURN 1;
	END;

	RETURN -1;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_UpdatePhoto')
	DROP PROCEDURE [bx_Album_UpdatePhoto];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_UpdatePhoto
    @PhotoID           int
   ,@Name              nvarchar(50)
   ,@Description       nvarchar(1500)
   ,@LastEditUserID    int
AS BEGIN
    SET NOCOUNT ON;

    UPDATE 
        [bx_Photos] 
    SET 
         [Name] = @Name
        ,[Description] = @Description
        ,[KeywordVersion] = N''
        ,[UpdateDate] = GETDATE()
		,[LastEditUserID] = @LastEditUserID
    WHERE
        [PhotoID] = @PhotoID;

	DECLARE @AlbumID int;

	SELECT @AlbumID = AlbumID FROM [bx_Photos] WHERE [PhotoID] = @PhotoID;

	UPDATE [bx_Albums] SET [LastEditUserID] = @LastEditUserID WHERE AlbumID = @AlbumID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_UpdatePhotos')
	DROP PROCEDURE [bx_Album_UpdatePhotos];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_UpdatePhotos
    @AlbumID            int
   ,@PhotoIDs           varchar(8000)
   ,@PhotoNames         ntext
   ,@PhotoDescs         ntext
   ,@LastEditUserID     int
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @TempData table(TempID int identity(1,1), _PhotoID int, _PhotoName varchar(50), _PhotoDesc varchar(1500));

	INSERT INTO @TempData (_PhotoID) SELECT item FROM bx_GetIntTable(@PhotoIDs, '|');

	IF NOT EXISTS(SELECT * FROM (SELECT * FROM [bx_Photos] WHERE PhotoID IN (SELECT _PhotoID FROM @TempData)) A WHERE A.AlbumID != @AlbumID) BEGIN

		UPDATE @TempData SET _PhotoName = T.item FROM bx_GetStringTable_ntext(@PhotoNames, '|') T WHERE TempID = T.id;

		UPDATE @TempData SET _PhotoDesc = T.item FROM bx_GetStringTable_ntext(@PhotoDescs, '|') T WHERE TempID = T.id;

		UPDATE [bx_Photos] SET [Name] = _PhotoName, [Description] = ISNULL(_PhotoDesc,''), [LastEditUserID] = @LastEditUserID FROM @TempData WHERE AlbumID=@AlbumID AND PhotoID = _PhotoID;

		UPDATE [bx_Albums] SET [LastEditUserID] = @LastEditUserID WHERE AlbumID = @AlbumID;

		RETURN 1;
	END;

	RETURN 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_UpdatePhotoThumbInfos')
	DROP PROCEDURE [bx_Album_UpdatePhotoThumbInfos];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_UpdatePhotoThumbInfos
    @PhotoIDs           varchar(8000)
   ,@ThumbPaths         text
   ,@ThumbWidths        varchar(8000)
   ,@ThumbHeights       varchar(8000)
   ,@AsAlbumCovers      varchar(8000)
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @TempData table(TempID int identity(1,1), PID int, Path varchar(256), Width int, Height int, AsAlbumCover int);

	INSERT INTO @TempData (PID) SELECT item FROM bx_GetIntTable(@PhotoIDs, '|');

	UPDATE @TempData SET Path = T.item FROM bx_GetStringTable_text(@ThumbPaths, '|') T WHERE TempID = T.id;

	UPDATE @TempData SET Width = T.item FROM bx_GetIntTable(@ThumbWidths, '|') T WHERE TempID = T.id;

	UPDATE @TempData SET Height = T.item FROM bx_GetIntTable(@ThumbHeights, '|') T WHERE TempID = T.id;

	UPDATE @TempData SET AsAlbumCover = T.item FROM bx_GetIntTable(@AsAlbumCovers, '|') T WHERE TempID = T.id;

	UPDATE [bx_Photos] SET ThumbPath = T.Path, ThumbWidth = T.Width, ThumbHeight = T.Height FROM @TempData T WHERE PhotoID = T.PID;

	DECLARE @Cover AS varchar(256);

	SELECT TOP 1 @Cover = Path FROM @TempData ORDER BY TempID DESC;

	UPDATE [bx_Albums] SET [Cover] = ThumbPath, [CoverPhotoID] = [PhotoID] FROM [bx_Photos] WHERE [PhotoID] IN (SELECT PID FROM @TempData WHERE AsAlbumCover = 1) AND [bx_Albums].AlbumID = [bx_Photos].AlbumID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_MovePhotos')
	DROP PROCEDURE [bx_Album_MovePhotos];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_MovePhotos
    @SrcAlbumID         int
   ,@DesAlbumID         int
   ,@PhotoIDs           varchar(8000)
   ,@LastEditUserID     int
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @TempData table(TempID int identity(1,1), _PhotoID int);

	INSERT INTO @TempData (_PhotoID) SELECT item FROM bx_GetIntTable(@PhotoIDs, '|');


	IF NOT EXISTS(SELECT * FROM (SELECT * FROM [bx_Photos] WHERE PhotoID IN (SELECT _PhotoID FROM @TempData)) A WHERE A.AlbumID != @SrcAlbumID) BEGIN


		UPDATE [bx_Albums] SET [CoverPhotoID] = NULL, [Cover] = '' WHERE AlbumID = @SrcAlbumID AND CoverPhotoID IN (SELECT _PhotoID FROM @TempData);


		UPDATE [bx_Albums] SET [LastEditUserID] = @LastEditUserID WHERE AlbumID IN (@SrcAlbumID, @DesAlbumID);

		UPDATE [bx_Photos] SET [AlbumID] = @DesAlbumID, [LastEditUserID] = @LastEditUserID FROM @TempData WHERE AlbumID = @SrcAlbumID AND PhotoID = _PhotoID;

		RETURN 1;
	END;

	RETURN 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_GetUploadPhotoCount')
	DROP PROCEDURE [bx_Album_GetUploadPhotoCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_GetUploadPhotoCount
	@UserID		int,
	@BeginDate	datetime,
	@EndDate	datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_Photos WHERE [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_GetPhoto')
	DROP PROCEDURE [bx_Album_GetPhoto];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_GetPhoto
    @PhotoID       int
AS BEGIN
    SET NOCOUNT ON;

    SELECT
        *
    FROM
        [bx_Photos]
    WHERE
        [PhotoID] = @PhotoID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_UpdateAlbumKeywords')
	DROP PROCEDURE [bx_Album_UpdateAlbumKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_UpdateAlbumKeywords
    @AlbumID               int,
    @KeywordVersion        varchar(32),
    @Name                  nvarchar(50),
    @Description            nvarchar(100),
    @NameReverter          nvarchar(1500),
    @DescriptionReverter   nvarchar(2500)
AS BEGIN

    SET NOCOUNT ON;

    IF @Name IS NOT NULL OR @Description IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL BEGIN

            IF @Name IS NOT NULL AND @Description IS NOT NULL
                UPDATE bx_Albums SET Name = @Name, Description = @Description, KeywordVersion = @KeywordVersion WHERE AlbumID = @AlbumID;
            ELSE IF @Name IS NOT NULL
                UPDATE bx_Albums SET Name = @Name, KeywordVersion = @KeywordVersion WHERE AlbumID = @AlbumID;
            ELSE
                UPDATE bx_Albums SET Description = @Description, KeywordVersion = @KeywordVersion WHERE AlbumID = @AlbumID;

        END
        ELSE BEGIN

           IF @Name IS NOT NULL AND @Description IS NOT NULL
                UPDATE bx_Albums SET Name = @Name, Description = @Description WHERE AlbumID = @AlbumID;
            ELSE IF @Name IS NOT NULL
                UPDATE bx_Albums SET Name = @Name WHERE AlbumID = @AlbumID;
            ELSE
                UPDATE bx_Albums SET Description = @Description WHERE AlbumID = @AlbumID;

        END

    END

    IF (@NameReverter IS NOT NULL AND @DescriptionReverter IS NOT NULL) BEGIN

        UPDATE bx_AlbumReverters SET NameReverter = @NameReverter, DescriptionReverter = @DescriptionReverter WHERE AlbumID = @AlbumID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_AlbumReverters (AlbumID, NameReverter, DescriptionReverter) VALUES (@AlbumID, @NameReverter, @DescriptionReverter);

    END
    ELSE IF (@NameReverter IS NOT NULL) BEGIN

        UPDATE bx_AlbumReverters SET NameReverter = @NameReverter WHERE AlbumID = @AlbumID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_AlbumReverters (AlbumID, NameReverter, DescriptionReverter) VALUES (@AlbumID, @NameReverter, N'');

    END
    ELSE IF (@DescriptionReverter IS NOT NULL) BEGIN

        UPDATE bx_AlbumReverters SET DescriptionReverter = @DescriptionReverter WHERE AlbumID = @AlbumID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_AlbumReverters (AlbumID, NameReverter, DescriptionReverter) VALUES (@AlbumID, N'', @DescriptionReverter);

    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Album_UpdatePhotoKeywords')
	DROP PROCEDURE [bx_Album_UpdatePhotoKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Album_UpdatePhotoKeywords
    @PhotoID               int,
    @KeywordVersion        varchar(32),
    @Name                  nvarchar(50),
    @NameReverter          nvarchar(1500),
    @Description           nvarchar(1500),
    @DescriptionReverter   nvarchar(2500)
AS BEGIN

    SET NOCOUNT ON;

    IF @Name IS NOT NULL OR @Description IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL BEGIN

            IF @Name IS NOT NULL AND @Description IS NOT NULL
                UPDATE bx_Photos SET Name = @Name, Description = @Description, KeywordVersion = @KeywordVersion WHERE PhotoID = @PhotoID;
            ELSE IF @Name IS NOT NULL
                UPDATE bx_Photos SET Name = @Name, KeywordVersion = @KeywordVersion WHERE PhotoID = @PhotoID;
            ELSE
                UPDATE bx_Photos SET Description = @Description, KeywordVersion = @KeywordVersion WHERE PhotoID = @PhotoID;

        END
        ELSE BEGIN

           IF @Name IS NOT NULL AND @Description IS NOT NULL
                UPDATE bx_Photos SET Name = @Name, Description = @Description WHERE PhotoID = @PhotoID;
            ELSE IF @Name IS NOT NULL
                UPDATE bx_Photos SET Name = @Name WHERE PhotoID = @PhotoID;
            ELSE
                UPDATE bx_Photos SET Description = @Description WHERE PhotoID = @PhotoID;

        END

    END

    IF (@NameReverter IS NOT NULL AND @DescriptionReverter IS NOT NULL) BEGIN

        UPDATE bx_PhotoReverters SET NameReverter = @NameReverter, DescriptionReverter = @DescriptionReverter WHERE PhotoID = @PhotoID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_PhotoReverters (PhotoID, NameReverter, DescriptionReverter) VALUES (@PhotoID, @NameReverter, @DescriptionReverter);

    END
    ELSE IF (@NameReverter IS NOT NULL) BEGIN

        UPDATE bx_PhotoReverters SET NameReverter = @NameReverter WHERE PhotoID = @PhotoID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_PhotoReverters (PhotoID, NameReverter, DescriptionReverter) VALUES (@PhotoID, @NameReverter, N'');

    END
    ELSE IF (@DescriptionReverter IS NOT NULL) BEGIN

        UPDATE bx_PhotoReverters SET DescriptionReverter = @DescriptionReverter WHERE PhotoID = @PhotoID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_PhotoReverters (PhotoID, NameReverter, DescriptionReverter) VALUES (@PhotoID, N'', @DescriptionReverter);

    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateAlbumLogo')
	DROP PROCEDURE [bx_UpdateAlbumLogo];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateAlbumLogo
       @AlbumID        int
     , @Cover     nvarchar(200)
AS BEGIN
    SET NOCOUNT ON;

    UPDATE
        [bx_Albums]
    SET
        [Cover] = @Cover
       ,[UpdateDate] = GETDATE()
    WHERE
        [AlbumID] = @AlbumID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAlbumsByUserID')
	DROP PROCEDURE [bx_GetAlbumsByUserID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAlbumsByUserID
    @UserID         int
AS BEGIN
    SET NOCOUNT ON;

    SELECT * FROM [bx_Albums] WHERE [UserID] = @UserID;    
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserAllPhotos')
	DROP PROCEDURE [bx_GetUserAllPhotos];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserAllPhotos
    @UserID        int
AS BEGIN
    SET NOCOUNT ON;    

    SELECT * FROM [bx_Photos] WHERE [UserID] = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetPhotosAndAlbum')
	DROP PROCEDURE [bx_GetPhotosAndAlbum];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetPhotosAndAlbum
    @PhotoID        int
AS BEGIN
    SET NOCOUNT ON;   
    DECLARE @AlbumID int;
    SELECT @AlbumID = AlbumID FROM [bx_Photos] WHERE [PhotoID] = @PhotoID;
    SELECT * FROM [bx_Photos] WHERE AlbumID=@AlbumID;
    SELECT * FROM [bx_Albums] WHERE AlbumID=@AlbumID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateSerial')
	DROP PROCEDURE [bx_CreateSerial];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateSerial
@UserID         int
,@Type          tinyint
,@ExpiresDate   datetime
,@Data          nvarchar(1000)
,@Success		bit out
AS
BEGIN
    SET NOCOUNT ON;	 
	DECLARE @TodayCount int;
	DECLARE @Today datetime;
    DECLARE @Serial uniqueidentifier;

	SET @Success=1;
    SET @Today = CONVERT(varchar(12) , GETDATE(), 102);
    SET @Serial = NEWID(); 
    SELECT @TodayCount = COUNT(*) FROM (SELECT TOP 3 Serial FROM bx_Serials WHERE Type= @Type AND UserID = @UserID AND CreateDate >= @Today) as t;
	IF(@TodayCount < 3) BEGIN
		INSERT INTO bx_Serials( Serial, UserID, CreateDate, ExpiresDate, Type, Data) Values( @Serial, @UserID, GETDATE(), @ExpiresDate, @Type, @Data);
		SELECT * FROM bx_Serials WHERE Serial = @Serial;
	END
	ELSE
		SET @Success=0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetSerial')
	DROP PROCEDURE [bx_GetSerial];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetSerial
@Serial uniqueidentifier
,@Type tinyint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM bx_Serials Where Serial = @Serial AND ExpiresDate >= GETDATE();
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteSerialByUser')
	DROP PROCEDURE [bx_DeleteSerialByUser];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteSerialByUser
@UserID int
,@Type tinyint
AS
BEGIN
    SET NOCOUNT ON;
    DELETE bx_Serials WHERE UserID = @UserID AND Type = @Type;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_AddPropLog')
	DROP PROCEDURE [bx_Prop_AddPropLog];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_AddPropLog
  @UserID int
 ,@Type   tinyint
 ,@Log    ntext
AS
BEGIN
    SET NOCOUNT ON;
  INSERT INTO bx_PropLogs (UserID, Type, Log) VALUES (@UserID, @Type, @Log);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_Replenish')
	DROP PROCEDURE [bx_Prop_Replenish];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_Replenish
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE bx_Props 
    SET 
        TotalNumber = TotalNumber + ReplenishNumber * DATEDIFF(hh, LastReplenishTime, GETDATE()), 
        LastReplenishTime = GETDATE() 
    WHERE 
        AutoReplenish = 1 AND TotalNumber <= ReplenishLimit AND LastReplenishTime <= DATEADD(hh, 0 - ReplenishTimeSpan, GETDATE());
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_CreateProp')
	DROP PROCEDURE [bx_Prop_CreateProp];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_CreateProp
 @Icon nvarchar(255)
 ,@Name nvarchar(100)
 ,@Price int
 ,@PriceType int
 ,@PropType nvarchar(512)
 ,@PropParam ntext
 ,@Description nvarchar(255)
 ,@PackageSize int
 ,@TotalNumber int
 ,@AllowExchange bit
 ,@AutoReplenish bit
 ,@ReplenishNumber int
 ,@ReplenishTimeSpan int
 ,@BuyCondition ntext
 ,@ReplenishLimit int
 ,@SortOrder int
AS
BEGIN
  SET NOCOUNT ON;
  INSERT INTO bx_Props (
    [Icon]
   ,[Name]
   ,[Price]
   ,[PriceType]
   ,[PropType]
   ,[PropParam]
   ,[Description]
   ,[PackageSize]
   ,[TotalNumber]
   ,[AllowExchange]
   ,[AutoReplenish]
   ,[ReplenishNumber]
   ,[ReplenishTimeSpan]
   ,[BuyCondition]
   ,[ReplenishLimit]
   ,[SortOrder]
  ) VALUES (
    @Icon
   ,@Name
   ,@Price
   ,@PriceType
   ,@PropType
   ,@PropParam
   ,@Description
   ,@PackageSize
   ,@TotalNumber
   ,@AllowExchange
   ,@AutoReplenish
   ,@ReplenishNumber
   ,@ReplenishTimeSpan
   ,@BuyCondition
   ,@ReplenishLimit
   ,@SortOrder
  );

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_UpdateProp')
	DROP PROCEDURE [bx_Prop_UpdateProp];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_UpdateProp
  @PropID int
 ,@Icon nvarchar(255)
 ,@Name nvarchar(100)
 ,@Price int
 ,@PriceType int
 ,@PropType nvarchar(512)
 ,@PropParam ntext
 ,@Description nvarchar(255)
 ,@PackageSize int
 ,@TotalNumber int
 ,@AllowExchange bit
 ,@AutoReplenish bit
 ,@ReplenishNumber int
 ,@ReplenishTimeSpan int
 ,@BuyCondition ntext
 ,@ReplenishLimit int
 ,@SortOrder int
AS
BEGIN
  SET NOCOUNT ON;
  UPDATE bx_Props SET
    [Icon] = @Icon
   ,[Name] = @Name
   ,[Price] = @Price
   ,[PriceType] = @PriceType
   ,[PropType] = @PropType
   ,[PropParam] = @PropParam
   ,[Description] = @Description
   ,[PackageSize] = @PackageSize
   ,[TotalNumber] = @TotalNumber
   ,[AllowExchange] = @AllowExchange
   ,[AutoReplenish] = @AutoReplenish
   ,[ReplenishNumber] = @ReplenishNumber
   ,[ReplenishTimeSpan] = @ReplenishTimeSpan
   ,[BuyCondition] = @BuyCondition
   ,[ReplenishLimit] = @ReplenishLimit
   ,[SortOrder] = @SortOrder
  WHERE
   [PropID] = @PropID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_BuyProp')
	DROP PROCEDURE [bx_Prop_BuyProp];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_BuyProp
  @UserID int
 ,@PropID int
 ,@BuyCount int
 ,@MaxPackageSize int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM bx_Props WHERE PropID = @PropID AND TotalNumber >= @BuyCount) BEGIN

        DECLARE @PackageSize int;

        SELECT @PackageSize = SUM(B.PackageSize * (A.[Count] + A.[SellingCount])) FROM bx_UserProps A LEFT JOIN bx_Props B ON B.PropID = A.PropID WHERE UserID = @UserID;

        SELECT @PackageSize = ISNULL(@PackageSize, 0) + PackageSize * @BuyCount FROM bx_Props WHERE PropID = @PropID;

        IF @PackageSize <= @MaxPackageSize BEGIN
            IF EXISTS(SELECT * FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID) BEGIN
                UPDATE bx_UserProps SET [Count] = [Count] + @BuyCount WHERE UserID = @UserID AND PropID = @PropID;
            END
            ELSE BEGIN
                INSERT INTO bx_UserProps ([UserID],[PropID],[Count]) VALUES (@UserID, @PropID, @BuyCount);
            END
        END
        ELSE BEGIN
            RETURN 3;
        END

        UPDATE bx_Props SET SaledNumber = SaledNumber + @BuyCount, TotalNumber = TotalNumber - @BuyCount WHERE PropID = @PropID;

        UPDATE bx_Props SET TotalNumber = 0 WHERE TotalNumber < 0;

        INSERT INTO [bx_UserGetPropLogs](UserID,[bx_Users].Username,GetPropType,PropID,PropName,PropCount) 
        SELECT @UserID,Username,1,@PropID,[Name],@BuyCount
        FROM [bx_Users] LEFT JOIN [bx_Props] ON [bx_Props].PropID=@PropID
        WHERE [bx_Users].UserID=@UserID;

        RETURN 1;
    END

    RETURN 2;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_GetPropByID')
	DROP PROCEDURE [bx_Prop_GetPropByID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_GetPropByID
  @PropID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_Props WHERE PropID = @PropID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_GetUserPropStatus')
	DROP PROCEDURE [bx_Prop_GetUserPropStatus];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_GetUserPropStatus
    @UserID int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SUM([Count]) AS [Count], SUM([SellingCount]) AS [SellingCount], SUM(B.PackageSize * (A.[Count] + A.[SellingCount])) AS UsedPackageSize FROM bx_UserProps A LEFT JOIN bx_Props B ON B.PropID = A.PropID WHERE UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_GetUserProps')
	DROP PROCEDURE [bx_Prop_GetUserProps];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_GetUserProps
    @UserID int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM bx_Props A
    LEFT JOIN bx_UserProps B ON A.PropID = B.PropID WHERE B.UserID = @UserID ORDER BY UserPropID DESC;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_SaleUserProp')
	DROP PROCEDURE [bx_Prop_SaleUserProp];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_SaleUserProp
    @UserID int
    ,@PropID int
    ,@Count int
    ,@Price int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID AND [Count] + [SellingCount] >= @Count) BEGIN
        UPDATE bx_UserProps
        SET
            [SellingDate] = GETDATE()
        WHERE
            UserID = @UserID AND PropID = @PropID AND SellingCount = 0;

        UPDATE bx_UserProps 
        SET 
            [Count] = [Count] + [SellingCount] - @Count, 
            [SellingCount] = @Count, 
            [SellingPrice] = @Price
        WHERE 
            UserID = @UserID AND PropID = @PropID;
    END
    ELSE BEGIN
        RETURN 2;
    END

    RETURN 1;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_BuyUserProp')
	DROP PROCEDURE [bx_Prop_BuyUserProp];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_BuyUserProp
    @SalerUserID int
    ,@BuyerUserID int
    ,@PropID int
    ,@Count int
    ,@MaxPackageSize int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM bx_UserProps WHERE @Count > 0 AND UserID = @SalerUserID AND PropID = @PropID AND SellingCount >= @Count) BEGIN

        DECLARE @PackageSize int;

        SELECT @PackageSize = SUM(B.PackageSize * (A.[Count] + A.[SellingCount])) FROM bx_UserProps A LEFT JOIN bx_Props B ON B.PropID = A.PropID WHERE UserID = @BuyerUserID;

        SELECT @PackageSize = ISNULL(@PackageSize, 0) + PackageSize * @Count FROM bx_Props WHERE PropID = @PropID;

        IF @PackageSize <= @MaxPackageSize BEGIN
            IF EXISTS (SELECT * FROM bx_UserProps WHERE UserID = @BuyerUserID AND PropID = @PropID)BEGIN
                UPDATE bx_UserProps SET [Count] = [Count] + @Count WHERE UserID = @BuyerUserID AND PropID = @PropID;
            END
            ELSE BEGIN
                INSERT INTO bx_UserProps ([UserID], [PropID], [Count]) VALUES (@BuyerUserID, @PropID, @Count);
            END
        END
        ELSE BEGIN
            RETURN 3;
        END

        UPDATE bx_UserProps SET [SellingCount] = [SellingCount] - @Count WHERE UserID = @SalerUserID AND PropID = @PropID;

        DELETE FROM bx_UserProps WHERE UserID = @SalerUserID AND PropID = @PropID AND [SellingCount] = 0 AND [Count] = 0;

        RETURN 1;
    END

    RETURN 2;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_GetUserSellingProp')
	DROP PROCEDURE [bx_Prop_GetUserSellingProp];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_GetUserSellingProp
    @UserID int
    ,@PropID int
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM bx_UserPropsView WHERE UserID = @UserID AND PropID = @PropID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_GiftProp')
	DROP PROCEDURE [bx_Prop_GiftProp];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_GiftProp
    @UserID int
    ,@TargetUserID int
    ,@PropID int
    ,@Count int
    ,@MaxPackageSize int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM bx_UserProps WHERE @Count > 0 AND UserID = @UserID AND PropID = @PropID AND [Count] >= @Count) BEGIN

        DECLARE @PackageSize int;

        SELECT @PackageSize = SUM(B.PackageSize * (A.[Count] + A.SellingCount)) FROM bx_UserProps A LEFT JOIN bx_Props B ON B.PropID = A.PropID WHERE UserID = @TargetUserID;

        SELECT @PackageSize = ISNULL(@PackageSize, 0) + PackageSize * @Count FROM bx_Props WHERE PropID = @PropID;

        IF @PackageSize <= @MaxPackageSize BEGIN
            IF EXISTS(SELECT * FROM bx_UserProps WHERE UserID = @TargetUserID AND PropID = @PropID) BEGIN
                UPDATE bx_UserProps SET [Count] = [Count] + @Count WHERE UserID = @TargetUserID AND PropID = @PropID;
            END
            ELSE BEGIN
                INSERT INTO bx_UserProps ([UserID], [PropID], [Count]) VALUES (@TargetUserID, @PropID, @Count);
            END
        END
        ELSE BEGIN
            RETURN 3;
        END

        UPDATE bx_UserProps SET [Count] = [Count] - @Count WHERE UserID = @UserID AND PropID = @PropID;

        DELETE FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID AND [SellingCount] = 0 AND [Count] = 0;

        INSERT INTO [bx_UserGetPropLogs](UserID,[bx_Users].Username,GetPropType,PropID,PropName,PropCount) 
        SELECT @UserID,Username,2,@PropID,[Name],@Count
        FROM [bx_Users] LEFT JOIN [bx_Props] ON [bx_Props].PropID=@PropID
        WHERE [bx_Users].UserID=@TargetUserID;

        RETURN 1;
    END

    RETURN 2;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_DropProp')
	DROP PROCEDURE [bx_Prop_DropProp];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_DropProp
    @UserID int
    ,@PropID int
    ,@Count int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM bx_UserProps WHERE @Count > 0 AND UserID = @UserID AND PropID = @PropID AND [Count] >= @Count) BEGIN

        UPDATE bx_UserProps SET [Count] = [Count] - @Count WHERE UserID = @UserID AND PropID = @PropID;

        DELETE FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID AND [SellingCount] = 0 AND [Count] = 0;

        RETURN 1;
    END

    RETURN 2;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_GetUserPropByType')
	DROP PROCEDURE [bx_Prop_GetUserPropByType];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_GetUserPropByType
    @PropType nvarchar(512)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 * FROM bx_UserProps A RIGHT JOIN bx_Props B ON B.PropID = A.PropID WHERE PropType = @PropType;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Prop_UseProp')
	DROP PROCEDURE [bx_Prop_UseProp];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Prop_UseProp
    @UserID int,
    @PropID int
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID AND Count > 0) BEGIN

        UPDATE bx_UserProps SET Count = Count - 1 WHERE UserID = @UserID AND PropID = @PropID AND Count > 0;

        DELETE FROM bx_UserProps WHERE UserID = @UserID AND PropID = @PropID AND [SellingCount] = 0 AND [Count] = 0;
        
        RETURN 1;
    END

    RETURN 2;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_GetClubCategories')
	DROP PROCEDURE [bx_Club_GetClubCategories];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_GetClubCategories
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM bx_ClubCategories;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_CreateClub')
	DROP PROCEDURE [bx_Club_CreateClub];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_CreateClub
	@UserID		int,
	@CategoryID	int,
	@IsApproved	bit,
	@CreateIP	varchar(50),
	@Name		nvarchar(50),
	@NewClubID	int output
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM bx_Clubs WHERE [Name] = @Name) BEGIN
		RETURN 0;
	END

	INSERT INTO bx_Clubs ([UserID],[CategoryID],[IsApproved],[CreateIP],[Name]) VALUES (@UserID, @CategoryID, @IsApproved, @CreateIP, @Name);

	SELECT @NewClubID = @@IDENTITY;

	INSERT INTO bx_ClubMembers (ClubID, UserID, Status) VALUES (@NewClubID, @UserID, 5);

	RETURN 1;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_GetClubByID')
	DROP PROCEDURE [bx_Club_GetClubByID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_GetClubByID
	@ClubID	int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM bx_Clubs WHERE ClubID = @ClubID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_Update')
	DROP PROCEDURE [bx_Club_Update];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_Update
	@ClubID			int,
	@Description	nvarchar(200),
	@JoinMethod		tinyint,
	@AccessMode		tinyint,
	@IsNeedManager	bit
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE bx_Clubs SET 
		Description = @Description, 
		JoinMethod = @JoinMethod, 
		AccessMode = @AccessMode,
		IsNeedManager = @IsNeedManager,
		UpdateDate = GETDATE(),
		KeywordVersion = ''
	WHERE
		ClubID = @ClubID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_GetClubInvokes')
	DROP PROCEDURE [bx_Club_GetClubInvokes];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_GetClubInvokes
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM bx_Clubs WHERE ClubID IN (
		SELECT A.ClubID FROM bx_ClubMembers A WHERE Status = 1 AND UserID = @UserID
	);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_InvokeMember')
	DROP PROCEDURE [bx_Club_InvokeMember];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_InvokeMember
	@ClubID		int,
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT * FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID = @UserID) BEGIN

		INSERT INTO bx_ClubMembers (ClubID, UserID, Status) VALUES (@ClubID, @UserID, 1);
	END
	ELSE BEGIN
		
		UPDATE bx_ClubMembers SET Status = 1 WHERE ClubID = @ClubID AND UserID = @UserID AND Status = 2;
	END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_AcceptInvoke')
	DROP PROCEDURE [bx_Club_AcceptInvoke];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_AcceptInvoke
	@ClubID		int,
	@UserID		int
AS
BEGIN

	SET NOCOUNT ON;
	
	UPDATE bx_ClubMembers SET Status = 0, CreateDate = GETDATE() WHERE ClubID = @ClubID AND UserID = @UserID AND Status = 1;

	UPDATE bx_Clubs SET TotalMembers = TotalMembers + 1 WHERE ClubID = @ClubID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_JoinClub')
	DROP PROCEDURE [bx_Club_JoinClub];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_JoinClub
	@ClubID		int,
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT * FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID = @UserID) BEGIN
		INSERT INTO bx_ClubMembers (ClubID, UserID, Status) VALUES (@ClubID, @UserID, 0);
	END;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_PetitionJoinClub')
	DROP PROCEDURE [bx_Club_PetitionJoinClub];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_PetitionJoinClub
	@ClubID		int,
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT * FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID = @UserID) BEGIN
		INSERT INTO bx_ClubMembers (ClubID, UserID, Status) VALUES (@ClubID, @UserID, 2);
	END;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_PetitionAsManager')
	DROP PROCEDURE [bx_Club_PetitionAsManager];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_PetitionAsManager
	@ClubID		int,
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE bx_ClubMembers SET Status = 6 WHERE ClubID = @ClubID AND UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_IgnoreAllInvokes')
	DROP PROCEDURE [bx_Club_IgnoreAllInvokes];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_IgnoreAllInvokes
	@UserID		int
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM bx_ClubMembers WHERE Status = 1 AND UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_LeaveClub')
	DROP PROCEDURE [bx_Club_LeaveClub];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_LeaveClub
	@ClubID		int,
	@UserID		int
AS
BEGIN

	SET NOCOUNT ON;
	
	DELETE FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID = @UserID;

	UPDATE bx_Clubs SET TotalMembers = TotalMembers - 1 WHERE ClubID = @ClubID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Club_GetClubMemberStatus')
	DROP PROCEDURE [bx_Club_GetClubMemberStatus];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Club_GetClubMemberStatus
	@ClubID		int,
	@UserID		int
AS
BEGIN

	SET NOCOUNT ON;

	SELECT Status FROM bx_ClubMembers WHERE ClubID = @ClubID AND UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Announcement_GetAvailableAnnouncements')
	DROP PROCEDURE [bx_Announcement_GetAvailableAnnouncements];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Announcement_GetAvailableAnnouncements
 @AnnouncementID         int
,@PostUserID             int
,@Subject                nvarchar(200)
,@Content                ntext
,@AnnouncementType       tinyint
,@BeginDate              datetime
,@EndDate                datetime 
,@SortOrder              int
AS
BEGIN

    SET NOCOUNT ON;

    SELECT * FROM bx_Announcements WHERE EndDate > GETDATE() ORDER BY SortOrder ASC;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Announcement_Save')
	DROP PROCEDURE [bx_Announcement_Save];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Announcement_Save
 @AnnouncementID         int
,@PostUserID             int
,@Subject                nvarchar(200)
,@Content                ntext
,@AnnouncementType       tinyint
,@BeginDate              datetime
,@EndDate                datetime 
,@SortOrder              int
AS
BEGIN

    SET NOCOUNT ON;

    IF @AnnouncementID<>0 AND  @AnnouncementID  IS NOT NULL BEGIN
        UPDATE bx_Announcements SET PostUserID = @PostUserID, [Subject] = @Subject ,[Content] = @Content,AnnouncementType = @AnnouncementType, BeginDate = @BeginDate, EndDate = @EndDate,SortOrder = @SortOrder WHERE AnnouncementID = @AnnouncementID;
    END
    ELSE BEGIN
        INSERT INTO bx_Announcements(PostUserID, [Subject], [Content] , AnnouncementType , BeginDate, EndDate, SortOrder) VALUES( @PostUserID, @Subject,@Content, @AnnouncementType, @BeginDate, @EndDate, @SortOrder);
        SET @AnnouncementID = (SELECT @@Identity);
    END

    SELECT * FROM bx_Announcements WHERE AnnouncementID = @AnnouncementID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SaveUserTemporaryData')
	DROP PROCEDURE [bx_SaveUserTemporaryData];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SaveUserTemporaryData
 @Data            ntext
,@UserID          int
,@DataType        smallint
,@OverrideOld     bit
AS

BEGIN
    SET NOCOUNT ON;

    IF @OverrideOld = 1
        DELETE FROM bx_UserTempData WHERE DataType = @DataType AND UserID = @UserID;
    INSERT INTO bx_UserTempData( UserID , DataType , Data) VALUES( @UserID,@DataType , @Data);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SaveBindOrUnbindSmsCode')
	DROP PROCEDURE [bx_SaveBindOrUnbindSmsCode];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SaveBindOrUnbindSmsCode
 @UserID                int
,@Action                tinyint
,@MobilePhone           bigint
,@ChangedMobilePhone    bigint
,@SmsCode             varchar(10)  
,@ChangedSmsCode      varchar(10)
AS

BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TodayCount int;
    DECLARE @Today datetime;
    DECLARE @ExpiresDate datetime;

    IF (@Action=1 AND EXISTS(SELECT * FROM bx_Users WHERE MobilePhone=@MobilePhone)) OR
        (@Action=3 AND EXISTS(SELECT * FROM bx_Users WHERE MobilePhone=@MobilePhone))
        RETURN(2);

    SET @Today = CONVERT(varchar(12) , GETDATE(), 102);
    SET @ExpiresDate = DATEADD(hour, 24, GETDATE());

    SELECT @TodayCount = COUNT(*) FROM (SELECT TOP 3 SmsCodeID FROM [bx_SmsCodes] WHERE UserID = @UserID AND ActionType = @Action AND CreateDate >= @Today) as t;
    
    IF @TodayCount < 3 BEGIN
        INSERT INTO [bx_SmsCodes]( UserID , ActionType , ExpiresDate , MobilePhone, ChangedMobilePhone, SmsCode, ChangedSmsCode) VALUES( @UserID,@Action ,@ExpiresDate, @MobilePhone, @ChangedMobilePhone, @SmsCode, @ChangedSmsCode);
        RETURN (0);
    END
    ELSE
        RETURN (1);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ChangePhoneBySmsCode')
	DROP PROCEDURE [bx_ChangePhoneBySmsCode];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ChangePhoneBySmsCode
@UserID             int,
@Action             tinyint,
@OldMobilePhone     bigint,
@NewMobilePhone     bigint,
@OldSmsCode         varchar(10),
@NewSmsCode         varchar(10),
@OldSuccess         bit out,
@NewSuccess         bit out

AS
BEGIN
    SET NOCOUNT ON;

    SET @OldSuccess=0;
    SET @NewSuccess=0;

    IF EXISTS (SELECT * FROM bx_SmsCodes
            WHERE UserID = @UserID
              AND ActionType = @Action
              AND ExpiresDate >= GETDATE()
              AND IsUsed = 0
              AND ChangedMobilePhone = @OldMobilePhone
              AND ChangedSmsCode = @OldSmsCode)
        SET @OldSuccess=1;

    IF EXISTS (SELECT * FROM bx_SmsCodes
            WHERE UserID = @UserID
              AND ActionType = @Action
              AND ExpiresDate >= GETDATE()
              AND IsUsed = 0
              AND MobilePhone = @NewMobilePhone
              AND SmsCode = @NewSmsCode)
        SET @NewSuccess=1;

    IF(@OldSuccess = 1 AND @NewSuccess = 1) BEGIN
        UPDATE bx_Users SET MobilePhone = @NewMobilePhone WHERE UserID = @UserID;
        UPDATE bx_SmsCodes SET IsUsed = 1 WHERE UserID = @UserID AND IsUsed = 0;
        INSERT INTO bx_UserMobileOperationLogs(UserID, Username, MobilePhone, OperationType) SELECT UserID, Username, @NewMobilePhone, @Action FROM bx_Users WITH (NOLOCK) WHERE UserID = @UserID;
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetPhoneBySmsCode')
	DROP PROCEDURE [bx_SetPhoneBySmsCode];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetPhoneBySmsCode
@UserID             int,
@Action             tinyint,
@MobilePhone        bigint,
@SmsCode            varchar(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
            SELECT * FROM [bx_SmsCodes]
                WHERE UserID = @UserID
                  AND IsUsed = 0
                  AND ActionType = @Action
                  AND MobilePhone = @MobilePhone
                  AND SmsCode = @SmsCode
                  
                  AND ExpiresDate >= GETDATE()
            ) BEGIN

        IF @Action = 2
            UPDATE bx_Users SET MobilePhone = 0 WHERE UserID = @UserID;
        ELSE IF @Action = 1
            UPDATE bx_Users SET MobilePhone = @MobilePhone WHERE UserID = @UserID; 

        UPDATE bx_SmsCodes SET IsUsed = 1 WHERE UserID = @UserID AND IsUsed = 0;
        INSERT INTO bx_UserMobileOperationLogs(UserID,Username,MobilePhone,OperationType) SELECT UserID,Username,@MobilePhone,@Action FROM bx_Users WHERE UserID = @UserID;

        RETURN (1);
    END
    ELSE
        RETURN (0);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreatePointLogs')
	DROP PROCEDURE [bx_CreatePointLogs];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreatePointLogs
@UserID             int
,@Point0            int
,@Point1            int
,@Point2            int
,@Point3            int
,@Point4            int
,@Point5            int
,@Point6            int
,@Point7            int
,@Current0          int 
,@Current1          int 	
,@Current2          int  
,@Current3          int  
,@Current4          int  
,@Current5          int  
,@Current6          int  
,@Current7          int  
,@Operate           nvarchar(50)
,@Remarks           nvarchar(200)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @LogTypeID int;

IF EXISTS( SELECT * FROM bx_PointLogTypes WHERE  OperateName = @Operate)
 SET @LogTypeID = (SELECT OperateID FROM bx_PointLogTypes WHERE OperateName = @Operate);
ELSE BEGIN
 INSERT INTO bx_PointLogTypes(OperateName) VALUES(@Operate);   
 SET @LogTypeID = @@IDENTITY;
END

IF @Point0 IS NULL
    SET @Point0 = 0;
IF @Point1 IS NULL
    SET @Point1 = 0;
IF @Point2 IS NULL
    SET @Point2 = 0;
IF @Point3 IS NULL
    SET @Point3 = 0;
IF @Point4 IS NULL
    SET @Point4 = 0;
IF @Point5 IS NULL
    SET @Point5 = 0;
IF @Point6 IS NULL
    SET @Point6 = 0;
IF @Point7 IS NULL
    SET @Point7 = 0;

IF @Current0 IS NULL
    SET @Current0 = 0;
IF @Current1 IS NULL
    SET @Current1 = 0;
IF @Current2 IS NULL
    SET @Current2 = 0;
IF @Current3 IS NULL
    SET @Current3 = 0;
IF @Current4 IS NULL
    SET @Current4 = 0;
IF @Current5 IS NULL
    SET @Current5 = 0;
IF @Current6 IS NULL
    SET @Current6 = 0;
IF @Current7 IS NULL
    SET @Current7 = 0;

INSERT INTO bx_PointLogs(UserID, Point0, Point1, Point2, Point3, Point4, Point5, Point6, Point7, 
Current0 ,Current1,Current2 ,Current3 ,Current4 ,Current5 ,Current6 ,Current7 ,  
OperateID, Remarks)
VALUES( @UserID, @Point0, @Point1, @Point2, @Point3, @Point4, @Point5, @Point6, @Point7
,@Current0 ,@Current1,@Current2 ,@Current3 ,@Current4 ,@Current5 ,@Current6 ,@Current7 
, @LogTypeID, @Remarks );
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetValidateCodeActionRecords')
	DROP PROCEDURE [bx_GetValidateCodeActionRecords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetValidateCodeActionRecords
    @IP varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_ValidateCodeActionRecords] WHERE [IP]=@IP ORDER BY [ID];
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateValidateCodeActionRecord')
	DROP PROCEDURE [bx_CreateValidateCodeActionRecord];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateValidateCodeActionRecord
     @IP            varchar(50)
    ,@Action        varchar(200)
    ,@CreateDate    datetime
    ,@LimitedTime   datetime
    ,@LimitedCount  int
AS
BEGIN
	SET NOCOUNT ON;
    INSERT INTO [bx_ValidateCodeActionRecords]([IP], [Action], [CreateDate]) VALUES (@IP, @Action, @CreateDate);

    SELECT @@IDENTITY;

        
    EXEC('
    DECLARE @Temp table([ID] int);
    INSERT INTO @Temp SELECT TOP ' + @LimitedCount + ' [ID] FROM [bx_ValidateCodeActionRecords] WHERE [IP]=''' + @IP + ''' AND [Action]=''' + @Action + '''
        AND [CreateDate] > ''' + @LimitedTime + ''' ORDER BY [ID] DESC;

    DELETE [bx_ValidateCodeActionRecords] WHERE [ID]<(SELECT ISNULL(MIN([ID]),2147483647) FROM @Temp) AND  [IP]=''' + @IP + ''' AND [Action]=''' + @Action + ''';
    ');
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserIDByNotifyID')
	DROP PROCEDURE [bx_GetUserIDByNotifyID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserIDByNotifyID
    @NotifyID        int
AS BEGIN
    SET NOCOUNT ON;

    SELECT [UserID] FROM [bx_Notify] WHERE [NotifyID] = @NotifyID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_RegisterNotifyType')
	DROP PROCEDURE [bx_RegisterNotifyType];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_RegisterNotifyType
@TypeName      nvarchar(50)
,@Keep         bit
,@Description  nvarchar(200)
AS
BEGIN
SET NOCOUNT ON;

IF NOT  EXISTS( SELECT * FROM bx_NotifyTypes WHERE TypeName = @TypeName) BEGIN
INSERT INTO bx_NotifyTypes( TypeName, [Keep], Description ) VALUES( @TypeName ,@Keep ,@Description );
END

SELECT * FROM bx_NotifyTypes WHERE TypeName = @TypeName;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetNotify')
	DROP PROCEDURE [bx_GetNotify];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetNotify
    @UserID          int
   ,@NotifyID        int
   ,@SetRead         bit
AS BEGIN
    SET NOCOUNT ON;

    IF @SetRead = 1 BEGIN

        IF @UserID IS NULL
            UPDATE [bx_Notify] SET [IsRead] = 1 WHERE [NotifyID] = @NotifyID;
        ELSE
            UPDATE [bx_Notify] SET [IsRead] = 1 WHERE [UserID] = @UserID AND [NotifyID] = @NotifyID;

    END;

    IF @UserID IS NULL
        SELECT * FROM [bx_Notify] WHERE [NotifyID] = @NotifyID;
    ELSE
        SELECT * FROM [bx_Notify] WHERE [UserID] = @UserID AND [NotifyID] = @NotifyID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_AddNotify')
	DROP PROCEDURE [bx_AddNotify];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_AddNotify
    @TypeID          int
   ,@UserID          int
   ,@Content         nvarchar(1000)
   ,@Keyword         varchar(200)
   ,@NotifyDatas     ntext
   ,@ClientID        int
   ,@Actions         nvarchar(2000)
AS BEGIN
    SET NOCOUNT ON;

DECLARE @ExistNotifyID int;
DECLARE @UniteNotify     bit
SET @ExistNotifyID = ( SELECT TOP 1 NotifyID FROM bx_Notify WHERE [TypeID] = @TypeID AND [UserID] = @UserID AND Keyword = @Keyword);-- AND [Parameters] = @Parameters );

IF @ExistNotifyID IS NOT NULL BEGIN
UPDATE bx_Notify SET Content = @Content,NotifyDatas = @NotifyDatas, UpdateDate = GETDATE(), IsRead = 0 WHERE [TypeID] = @TypeID AND [UserID] = @UserID AND Keyword = @Keyword;
END
ELSE BEGIN
INSERT INTO bx_Notify(
     UserID
    ,TypeID
    ,Content
    ,Keyword
    ,NotifyDatas
    ,ClientID
    ,Actions
) VALUES(
     @UserID
    ,@TypeID
    ,@Content
    ,@Keyword
    ,@NotifyDatas
    ,@ClientID
    ,@Actions
);
END

SELECT * FROM bx_UnreadNotifies WHERE UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteNotifies')
	DROP PROCEDURE [bx_DeleteNotifies];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteNotifies
@UserID    int
,@NotifyIDs  varchar(8000)
AS 
BEGIN
SET NOCOUNT ON;
DECLARE @NotifyIDTable table(NotifyID int);

INSERT @NotifyIDTable SELECT item FROM bx_GetIntTable(@NotifyIDs,',');

IF @UserID IS NULL BEGIN
    DECLARE @TempUserIDs table( userid int);
    INSERT @TempUserIDs SELECT DISTINCT UserID FROM bx_Notify WHERE [NotifyID] IN ( SELECT NotifyID FROM  @NotifyIDTable);
    DELETE FROM [bx_Notify] WHERE [NotifyID] IN ( SELECT NotifyID FROM  @NotifyIDTable);
    SELECT * FROM bx_UnreadNotifies WHERE UserID IN (SELECT userid FROM @TempUserIDs);
END
ELSE BEGIN
    DELETE FROM [bx_Notify] WHERE UserID = @UserID AND [NotifyID] IN ( SELECT NotifyID FROM  @NotifyIDTable); 
    SELECT * FROM bx_UnreadNotifies WHERE UserID = @UserID;
END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_LoadAllNotifyType')
	DROP PROCEDURE [bx_LoadAllNotifyType];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_LoadAllNotifyType 
AS
BEGIN
SET NOCOUNT ON;
SELECT * FROM bx_NotifyTypes;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteNotifysByType')
	DROP PROCEDURE [bx_DeleteNotifysByType];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteNotifysByType
    @UserID             int
   ,@TypeID             tinyint
AS BEGIN
    SET NOCOUNT ON;

    DELETE FROM 
        [bx_Notify] 
    WHERE 
        (@UserID IS NULL OR [UserID] = @UserID) 
    AND 
        [TypeID] = @TypeID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_IgnoreNotifies')
	DROP PROCEDURE [bx_IgnoreNotifies];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_IgnoreNotifies
@UserID    int
,@NotifyIDs  varchar(8000)
AS 
BEGIN
SET NOCOUNT ON;

DECLARE @NotifyIDTable table(NotifyID int);
INSERT @NotifyIDTable SELECT item FROM bx_GetIntTable(@NotifyIDs,',');


DELETE FROM bx_Notify WHERE UserID = @UserID AND NotifyID IN( SELECT NotifyID FROM  @NotifyIDTable) AND TypeID IN( SELECT TypeID FROM bx_NotifyTypes WHERE [Keep] <> 1 );

UPDATE bx_Notify SET IsRead = 1 WHERE UserID = @UserID AND NotifyID IN( SELECT NotifyID FROM  @NotifyIDTable) AND TypeID IN( SELECT TypeID FROM bx_NotifyTypes WHERE [Keep] = 1 );

SELECT * FROM bx_UnreadNotifies WHERE UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_IgnoreNotifyByType')
	DROP PROCEDURE [bx_IgnoreNotifyByType];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_IgnoreNotifyByType
@UserID    int
,@TypeID   int
AS 
BEGIN
SET NOCOUNT ON;

IF @TypeID <>0 BEGIN
DECLARE @Keep bit

SET @Keep = ( SELECT [Keep] FROM bx_NotifyTypes WHERE TypeID = @TypeID );

    IF @Keep <> 1 BEGIN
        DELETE FROM bx_Notify WHERE UserID = @UserID AND TypeID = @TypeID;
        RETURN;
    END
    ELSE BEGIN
        UPDATE bx_Notify SET IsRead = 1 WHERE UserID = @UserID AND TypeID = @TypeID;
        RETURN;
    END
END

UPDATE bx_Notify SET IsRead = 1 WHERE UserID = @UserID AND TypeID IN( SELECT TypeID FROM bx_NotifyTypes WHERE [Keep] = 1);
DELETE FROM bx_Notify WHERE UserID = @UserID AND TypeID IN( SELECT TypeID FROM bx_NotifyTypes WHERE [Keep] <> 1);

SELECT * FROM bx_UnreadNotifies WHERE UserID = @UserID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_CreatePostMark')
	DROP PROCEDURE [bx_v5_CreatePostMark];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_CreatePostMark 
    @PostID             int, 
	@UserID             int,
    @Username           nvarchar(50),
	@CreateDate         datetime,
	@ExtendedPoints_1   int,
	@ExtendedPoints_2   int,
	@ExtendedPoints_3   int,
	@ExtendedPoints_4   int,
	@ExtendedPoints_5   int,
	@ExtendedPoints_6   int,
	@ExtendedPoints_7   int,
	@ExtendedPoints_8   int,
	@Reason             ntext
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM [bx_PostMarks] WITH (NOLOCK) WHERE [UserID] = @UserID AND [PostID] = @PostID)
		RETURN 1;
	ELSE BEGIN
		insert into [bx_PostMarks](
			PostID,
			UserID,
            Username,
			CreateDate,
			ExtendedPoints_1,
			ExtendedPoints_2,
			ExtendedPoints_3,
			ExtendedPoints_4,
			ExtendedPoints_5,
			ExtendedPoints_6,
			ExtendedPoints_7,
			ExtendedPoints_8,
			Reason
			)
		Values
		(
			@PostID, 
			@UserID,
            @Username,
			@CreateDate,
			@ExtendedPoints_1,
			@ExtendedPoints_2,
			@ExtendedPoints_3,
			@ExtendedPoints_4,
			@ExtendedPoints_5,
			@ExtendedPoints_6,
			@ExtendedPoints_7,
			@ExtendedPoints_8,
			@Reason
		);

        
        UPDATE [bx_Posts] SET MarkCount = (SELECT Count(*) FROM [bx_PostMarks] WITH (NOLOCK) WHERE [PostID] = @PostID) WHERE [PostID] = @PostID;
        SELECT * FROM [bx_PostMarks] WITH (NOLOCK) WHERE [UserID] = @UserID AND [PostID] = @PostID;

		RETURN 0;
		
	END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetUserTodayPostMarks')
	DROP PROCEDURE [bx_v5_GetUserTodayPostMarks];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetUserTodayPostMarks
    @UserID      int,
    @DateTime    datetime
AS BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM [bx_PostMarks] WITH (NOLOCK) WHERE [UserID] = @UserID AND [CreateDate] >= @DateTime;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_CreateThreadManageLog')
	DROP PROCEDURE [bx_v5_CreateThreadManageLog];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_CreateThreadManageLog
	@UserID         int,
	@UserName       varchar(64),
	@NickName       varchar(64),
	@IPAddress      varchar(15),
	@PostUserIDs    varchar(8000),
	@ActionType     tinyint,
	@ForumID        int,
	@ThreadIDs      varchar(8000),
	@ThreadSubjects ntext,
	@Reason         nvarchar(256),
	@IsPublic       bit
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @TempTable TABLE(LogID INT IDENTITY(1,1),ThreadID INT,PostUserID INT,ThreadSubject nvarchar(256) COLLATE Chinese_PRC_CI_AS_WS)

	INSERT @TempTable (ThreadID) 
		SELECT item FROM bx_GetIntTable(@ThreadIDs,',')

	UPDATE @TempTable
			SET [ThreadSubject]=T.item
			FROM (SELECT * FROM bx_GetStringTable_ntext(@ThreadSubjects,N',')) T
			WHERE T.id=LogID;

	UPDATE @TempTable
			SET [PostUserID]=T.item
			FROM (SELECT * FROM bx_GetIntTable(@PostUserIDs, ',')) T
			WHERE T.id=LogID;

	INSERT INTO [bx_ThreadManageLogs] (
		[UserID],
		[UserName],
		[NickName],
		[IPAddress],
		[PostUserID],
		[ActionType],
		[ForumID],
		[ThreadID],
		[ThreadSubject],
		[Reason],
		[CreateDate],
		[IsPublic]
	) SELECT @UserID,@UserName,@NickName,@IPAddress,PostUserID,@ActionType,@ForumID,ThreadID,ThreadSubject,@Reason,getdate(),@IsPublic FROM @TempTable;

	IF @ActionType <> 1 AND @ActionType <> 17 AND @ActionType <> 18 AND @IsPublic = 1 BEGIN
		UPDATE bx_Threads Set ThreadLog = @UserName + '|' + CAST(@ActionType as NVARCHAR) + '|' + CAST(getdate() AS NVARCHAR) WHERE ThreadID IN (SELECT ThreadID FROM @TempTable);
        SELECT @UserName + '|' + CAST(@ActionType as NVARCHAR) + '|' + CAST(getdate() AS NVARCHAR) AS ThreadLog;
    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetThreadManageLogs')
	DROP PROCEDURE [bx_v5_GetThreadManageLogs];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetThreadManageLogs
     @ThreadID INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_ThreadManageLogs WITH (NOLOCK) WHERE ThreadID=@ThreadID ORDER BY LogID DESC;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAllStickThreadForums')
	DROP PROCEDURE [bx_GetAllStickThreadForums];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAllStickThreadForums
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_StickThreads  WITH (NOLOCK);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ClearSearchResult')
	DROP PROCEDURE [bx_ClearSearchResult];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ClearSearchResult
AS
BEGIN

    SET NOCOUNT ON;
	
	DELETE bx_SearchPostResults WHERE UpdateDate < DATEADD(minute, -30, getdate());

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetThreadsByPostCreateDate')
	DROP PROCEDURE [bx_GetThreadsByPostCreateDate];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetThreadsByPostCreateDate
    @CreateDate     datetime
AS
BEGIN

    SET NOCOUNT ON;
	
	
	SELECT T1.* 
	 FROM bx_Threads T1 WITH (NOLOCK) WHERE ThreadStatus = 1 AND [UpdateDate]>@CreateDate ORDER BY [UpdateDate] DESC

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdatePostsShielded')
	DROP PROCEDURE [bx_UpdatePostsShielded];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdatePostsShielded
	@PostIDs        varchar(8000),
	@IsShielded     bit
AS
	SET NOCOUNT ON
	EXEC('UPDATE [bx_Posts] SET IsShielded='+@IsShielded+' WHERE PostID IN('+@PostIDs+') AND IsShielded<>'+@IsShielded)
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ReCountTopicsAndPosts')
	DROP PROCEDURE [bx_ReCountTopicsAndPosts];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ReCountTopicsAndPosts
    @RecountToday       bit,
    @RecountYestoday    bit,
    @Today              datetime,
    @Yestoday           datetime
AS BEGIN
    SET NOCOUNT ON;

    IF @RecountYestoday = 1 BEGIN
        DECLARE @YestodayTopics int;
        DECLARE @YestodayPosts int;
        SELECT @YestodayTopics=COUNT(*) FROM bx_Threads WITH(NOLOCK)  WHERE ThreadStatus<4 AND CreateDate>@Yestoday AND CreateDate<@Today;
        SELECT @YestodayPosts=COUNT(*) FROM bx_Posts WITH(NOLOCK)  WHERE SortOrder<4000000000000000 AND CreateDate>@Yestoday AND CreateDate<@Today;

        DECLARE @MaxPosts int
        SELECT top 1 @MaxPosts=MaxPosts FROM bx_Vars;

        IF @YestodayPosts > @MaxPosts
           UPDATE bx_Vars SET  @MaxPosts=@YestodayPosts,MaxPostDate='',LastResetDate=GETDATE(),YestodayPosts=@YestodayPosts,YestodayTopics=@YestodayTopics;
        ELSE
           UPDATE bx_Vars SET  LastResetDate=GETDATE(),YestodayPosts=@YestodayPosts,YestodayTopics=@YestodayTopics;
    END

    
    IF @RecountToday = 1 BEGIN
        BEGIN TRANSACTION
        UPDATE bx_Forums SET TodayThreads=0,TodayPosts=0;
        IF(@@error<>0)
		        GOTO Cleanup;

        UPDATE bx_Forums SET TodayThreads = T.TotalCount FROM(
        SELECT COUNT(*) as TotalCount,ForumID FROM bx_Threads WITH(NOLOCK)  WHERE ThreadStatus<4 AND CreateDate>@Today GROUP BY ForumID
        ) AS T WHERE T.ForumID = bx_Forums.ForumID;
        IF(@@error<>0)
		        GOTO Cleanup;

        UPDATE bx_Forums SET TodayPosts = T.TotalCount FROM(
        SELECT COUNT(*) as TotalCount,ForumID FROM bx_Posts WITH(NOLOCK)  WHERE SortOrder<4000000000000000 AND CreateDate>@Today GROUP BY ForumID
        ) AS T WHERE T.ForumID = bx_Forums.ForumID;
        IF(@@error<>0)
		        GOTO Cleanup;
        

        COMMIT TRANSACTION
            RETURN 0;
        Cleanup:
            BEGIN
    	        ROLLBACK TRANSACTION
    	        RETURN (-1);
            END

    END
    
    RETURN 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetThreadImage')
	DROP PROCEDURE [bx_SetThreadImage];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetThreadImage
    @ThreadID       int,
    @AttachmentID   int,
    @ImageUrl       varchar(200),
    @ImageCount     int
AS BEGIN
    SET NOCOUNT ON;
    IF EXISTS(SELECT * FROM bx_ThreadImages WHERE ThreadID = @ThreadID)
        UPDATE bx_ThreadImages SET AttachmentID = @AttachmentID,ImageUrl = @ImageUrl, ImageCount = @ImageCount WHERE ThreadID = @ThreadID;
    ELSE
        INSERT INTO bx_ThreadImages(ThreadID,AttachmentID,ImageUrl,ImageCount)VALUES(@ThreadID,@AttachmentID,@ImageUrl,@ImageCount);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteThreadImage')
	DROP PROCEDURE [bx_DeleteThreadImage];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteThreadImage
    @ThreadID       int
AS BEGIN
    SET NOCOUNT ON;
    DELETE bx_ThreadImages WHERE ThreadID = @ThreadID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Post_UpdatePostKeywords')
	DROP PROCEDURE [bx_Post_UpdatePostKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Post_UpdatePostKeywords
    @PostID            int,
    @KeywordVersion    varchar(32),
    @Subject           nvarchar(256),
    @SubjectReverter   nvarchar(4000),
    @Content           ntext,
    @ContentReverter   ntext
AS BEGIN

    SET NOCOUNT ON;

    IF @Subject IS NOT NULL OR @Content IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL BEGIN

            IF @Subject IS NOT NULL AND @Content IS NOT NULL
                UPDATE bx_Posts SET Subject = @Subject, Content = @Content, KeywordVersion = @KeywordVersion WHERE PostID = @PostID;
            ELSE IF @Subject IS NOT NULL
                UPDATE bx_Posts SET Subject = @Subject, KeywordVersion = @KeywordVersion WHERE PostID = @PostID;
            ELSE
                UPDATE bx_Posts SET Content = @Content, KeywordVersion = @KeywordVersion WHERE PostID = @PostID;

        END
        ELSE BEGIN

           IF @Subject IS NOT NULL AND @Content IS NOT NULL
                UPDATE bx_Posts SET Subject = @Subject, Content = @Content WHERE PostID = @PostID;
            ELSE IF @Subject IS NOT NULL
                UPDATE bx_Posts SET Subject = @Subject WHERE PostID = @PostID;
            ELSE
                UPDATE bx_Posts SET Content = @Content WHERE PostID = @PostID;

        END

    END

    IF (@SubjectReverter IS NOT NULL AND @ContentReverter IS NOT NULL) BEGIN

        UPDATE bx_PostReverters SET SubjectReverter = @SubjectReverter, ContentReverter = @ContentReverter WHERE PostID = @PostID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_PostReverters (PostID, SubjectReverter, ContentReverter) VALUES (@PostID, @SubjectReverter, @ContentReverter);

    END
    ELSE IF (@SubjectReverter IS NOT NULL) BEGIN

        UPDATE bx_PostReverters SET SubjectReverter = @SubjectReverter WHERE PostID = @PostID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_PostReverters (PostID, SubjectReverter, ContentReverter) VALUES (@PostID, @SubjectReverter, N'');

    END
    ELSE IF (@ContentReverter IS NOT NULL) BEGIN

        UPDATE bx_PostReverters SET ContentReverter = @ContentReverter WHERE PostID = @PostID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_PostReverters (PostID, SubjectReverter, ContentReverter) VALUES (@PostID, N'', @ContentReverter);

    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Thread_UpdateThreadKeywords')
	DROP PROCEDURE [bx_Thread_UpdateThreadKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Thread_UpdateThreadKeywords
    @ThreadID              int,
    @KeywordVersion        varchar(32),
    @Subject               nvarchar(256),
    @SubjectReverter       nvarchar(4000)
AS BEGIN


    SET NOCOUNT ON;

    IF @Subject IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL
            UPDATE bx_Threads SET Subject = @Subject, KeywordVersion = @KeywordVersion WHERE ThreadID = @ThreadID;
        ELSE
            UPDATE bx_Threads SET Subject = @Subject WHERE ThreadID = @ThreadID;

    END

    IF @SubjectReverter IS NOT NULL BEGIN

        UPDATE bx_ThreadReverters SET SubjectReverter = @SubjectReverter WHERE ThreadID = @ThreadID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_ThreadReverters (ThreadID, SubjectReverter) VALUES (@ThreadID, @SubjectReverter);

    END


END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Common_GetRecordsByPage')
	DROP PROCEDURE [bx_Common_GetRecordsByPage];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_Common_GetRecordsByPage]
	@PageIndex int,
	@PageSize int,
	@TableName varchar(256),
	@SelectFields varchar(8000) = '*', --查询字段，默认为 *
	@Condition nvarchar(4000) = N'',     --条件例如"DirectoryID=4"
	@SortField varchar(256) = '[SortOrder]',
	@IsDesc bit = 1, --是否倒序
	@TotalRecords int = -1
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @SQLString nvarchar(4000),@ResetOrder bit----------- 1表示读取数据的时候 排序要反过来
	EXECUTE bx_Common_GetRecordsByPageSQLString
						@PageIndex,
						@PageSize,
						@TableName,
						@SelectFields, --查询字段，默认为 *
						@Condition,     --条件例如"DirectoryID=4"
						@SortField,
						@IsDesc, --是否倒序
						@TotalRecords,
						@ResetOrder OUTPUT,
						@SQLString OUTPUT
	EXEC (@SQLString);
	RETURN @ResetOrder
/*	DECLARE @SQLString nvarchar(4000);
	DECLARE @WhereString1 nvarchar(800);
	DECLARE @WhereString2 nvarchar(800);
	DECLARE @ResetOrder bit----------- 1表示读取数据的时候 排序要反过来
	IF @Condition IS NULL OR @Condition = N'' BEGIN
		SELECT @WhereString1 = N'';
		SELECT @WhereString2 = N'WHERE ';
	END
	ELSE BEGIN
		SELECT @WhereString1 = N'WHERE ' + @Condition;
		SELECT @WhereString2 = N'WHERE ' + @Condition + N' AND ';
	END

	IF @PageIndex = 0 BEGIN
		SELECT @SQLString = N'SELECT TOP ' + STR(@PageSize)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName + ' WITH (NOLOCK)
			' + @WhereString1 + '
			ORDER BY ' + @SortField;
		IF @IsDesc = 1
			SELECT @SQLString = @SQLString + ' DESC';
		SET @ResetOrder=0	
	END
	ELSE BEGIN
		DECLARE @GetFromLast BIT
		IF @TotalRecords=-1
			SET @GetFromLast=0
		ELSE BEGIN
			DECLARE @TotalPage INT,@ResidualCount INT
			SET @ResidualCount=@TotalRecords%@PageSize
			IF @ResidualCount=0
				SET @TotalPage=@TotalRecords/@PageSize
			ELSE
				SET @TotalPage=@TotalRecords/@PageSize+1
			IF @PageIndex>@TotalPage/2 --从最后页算上来
				SET @GetFromLast=1
			ELSE
				SET @GetFromLast=0
			IF @GetFromLast=1 BEGIN
				IF @PageIndex=@TotalPage-1 BEGIN
					IF @ResidualCount=0
						SET @ResidualCount=@PageSize;
					SELECT @SQLString = N'SELECT TOP ' + STR(@ResidualCount)
						+ N' ' + @SelectFields
						+ N' FROM ' + @TableName + ' WITH (NOLOCK)
						' + @WhereString1 + '
						ORDER BY ' + @SortField;
					IF @IsDesc = 0--反过来
						SELECT @SQLString = @SQLString + ' DESC';
					EXEC (@SQLString);
					RETURN 1
				END 
				ELSE IF  @PageIndex>@TotalPage-1 BEGIN --已经超过最大页数
					SELECT @SQLString = N'SELECT TOP ' + @SelectFields
						+ N' FROM ' + @TableName + ' WITH (NOLOCK) WHERE 1=2'
					EXEC (@SQLString);
					RETURN 0
				END
				ELSE BEGIN
					SET @PageIndex=@TotalPage-(@PageIndex+1)---
					IF @IsDesc=1
						SET @IsDesc=0
					ELSE
						SET @IsDesc=1
					SET @ResetOrder=1
				END  
			END
			ELSE 
				SET @ResetOrder=0
		END
		
		DECLARE @TopCount INT
		IF @GetFromLast=1
			SET @TopCount=@PageSize * (@PageIndex-1)+@ResidualCount
		ELSE
			SET @TopCount=@PageSize * @PageIndex
		IF @IsDesc = 1
			SELECT @SQLString = 'SELECT TOP ' + STR(@PageSize)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName+' WITH (NOLOCK) 
			' + @WhereString2 + @SortField + ' <
				(SELECT Min(' + @SortField + ') FROM 
					(SELECT TOP ' + STR(@TopCount) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
						' + @WhereString1 + '
							ORDER BY ' + @SortField + ' DESC) AS IDX)
			ORDER BY ' + @SortField + ' DESC';
		ELSE
			SELECT @SQLString = 'SELECT TOP ' + STR(@PageSize)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName+' WITH (NOLOCK) 
			' + @WhereString2 + @SortField + ' >
				(SELECT Max(' + @SortField + ') FROM 
					(SELECT TOP ' + STR(@TopCount) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
						' + @WhereString1 + '
							ORDER BY ' + @SortField + ' ASC) AS IDX)
			ORDER BY ' + @SortField + ' ASC';
	END
	EXEC (@SQLString);
	RETURN @ResetOrder
*/
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Common_GetRecordsByPageSQLString')
	DROP PROCEDURE [bx_Common_GetRecordsByPageSQLString];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_Common_GetRecordsByPageSQLString]
	@PageIndex int,
	@PageSize int,
	@TableName varchar(256),
	@SelectFields varchar(8000) = '*', --查询字段，默认为 *
	@Condition nvarchar(4000) = N'',     --条件例如"DirectoryID=4"
	@SortField varchar(256) = '[SortOrder]',
	@IsDesc bit = 1, --是否倒序
	@TotalRecords int = -1,
	@ResetOrder bit output,----------- 1表示读取数据的时候 排序要反过来
	@SQLString  nvarchar(4000) output
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @WhereString1 nvarchar(4000);
	DECLARE @WhereString2 nvarchar(4000);
	IF @Condition IS NULL OR @Condition = N'' BEGIN
		SELECT @WhereString1 = N'';
		SELECT @WhereString2 = N'WHERE ';
	END
	ELSE BEGIN
		SELECT @WhereString1 = N'WHERE ' + @Condition;
		SELECT @WhereString2 = N'WHERE ' + @Condition + N' AND ';
	END

	IF @PageIndex = 0 BEGIN
		SELECT @SQLString = N'SELECT TOP ' + STR(@PageSize)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName + ' WITH (NOLOCK)
			' + @WhereString1 + '
			ORDER BY ' + @SortField;
		IF @IsDesc = 1
			SELECT @SQLString = @SQLString + ' DESC';
		SET @ResetOrder=0	
	END
	ELSE BEGIN
		SET @SQLString='';
		DECLARE @GetFromLast BIT
		IF @TotalRecords=-1
			SET @GetFromLast=0
		ELSE BEGIN
			DECLARE @TotalPage INT,@ResidualCount INT
			SET @ResidualCount=@TotalRecords%@PageSize
			IF @ResidualCount=0
				SET @TotalPage=@TotalRecords/@PageSize
			ELSE
				SET @TotalPage=@TotalRecords/@PageSize+1
			IF @PageIndex>@TotalPage/2 --从最后页算上来
				SET @GetFromLast=1
			ELSE
				SET @GetFromLast=0
			IF @GetFromLast=1 BEGIN
				IF @PageIndex=@TotalPage-1 BEGIN
					IF @ResidualCount=0
						SET @ResidualCount=@PageSize;
					SELECT @SQLString = N'SELECT top ' + STR(@ResidualCount)
						+ N' ' + @SelectFields
						+ N' FROM ' + @TableName + ' WITH (NOLOCK)
						' + @WhereString1 + '
						ORDER BY ' + @SortField;
					IF @IsDesc = 0--反过来
						SELECT @SQLString = @SQLString + ' DESC';
					SET @ResetOrder=1
				END 
				ELSE IF  @PageIndex>@TotalPage-1 BEGIN --已经超过最大页数
					SELECT @SQLString = N'SELECT ' + @SelectFields
						+ N' FROM ' + @TableName + ' WITH (NOLOCK) WHERE 1=2'
					SET @ResetOrder=0
				END
				ELSE BEGIN
					SET @PageIndex=@TotalPage-(@PageIndex+1)---
					IF @IsDesc=1
						SET @IsDesc=0
					ELSE
						SET @IsDesc=1
					SET @ResetOrder=1
				END  
			END
			ELSE 
				SET @ResetOrder=0
		END
		
		IF @SQLString='' BEGIN
			DECLARE @TopCount INT
			IF @GetFromLast=1 BEGIN
				IF @ResidualCount > 0
					SET @TopCount=@PageSize * (@PageIndex-1)+@ResidualCount;
				ELSE
					SET @TopCount=@PageSize * (@PageIndex)+@ResidualCount;
				IF @TopCount = 0
					SET @TopCount = @PageSize;
			END
			ELSE
				SET @TopCount=@PageSize * @PageIndex
			IF @IsDesc = 1
				SELECT @SQLString = 'SELECT TOP ' + STR(@PageSize)
				+ N' ' + @SelectFields
				+ N' FROM ' + @TableName+' WITH (NOLOCK) 
				' + @WhereString2 + @SortField + ' <
					(SELECT Min(' + @SortField + ') FROM 
						(SELECT TOP ' + STR(@TopCount) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
							' + @WhereString1 + '
								ORDER BY ' + @SortField + ' DESC) AS IDX)
				ORDER BY ' + @SortField + ' DESC';
			ELSE
				SELECT @SQLString = 'SELECT TOP ' + STR(@PageSize)
				+ N' ' + @SelectFields
				+ N' FROM ' + @TableName+' WITH (NOLOCK) 
				' + @WhereString2 + @SortField + ' >
					(SELECT Max(' + @SortField + ') FROM 
						(SELECT TOP ' + STR(@TopCount) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
							' + @WhereString1 + '
								ORDER BY ' + @SortField + ' ASC) AS IDX)
				ORDER BY ' + @SortField + ' ASC';
		END
	END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DoCreateStat')
	DROP PROCEDURE [bx_DoCreateStat];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_DoCreateStat] 
	@ForumID int,
	@StatType int,
	@Count int
AS
BEGIN
SET NOCOUNT ON;
IF EXISTS (SELECT * FROM bx_Stats WHERE StatType=@StatType AND [Year]=year(getdate()) AND [Month]=month(getdate()) AND [Day]=day(getdate()) AND [Hour]=DATEPART(hh, GETDATE()) AND [Param]=@ForumID )
		UPDATE bx_Stats SET [Count]=[Count]+@Count WHERE StatType=@StatType AND [Year]=year(getdate()) AND [Month]=month(getdate()) AND [Day]=day(getdate()) AND [Hour]=DATEPART(hh, GETDATE()) AND [Param]=@ForumID
		ELSE
		INSERT INTO bx_Stats(StatType,[Year],[Month],[Day],[Hour],[Count],[Param])
		VALUES (@StatType,year(getdate()),month(getdate()),day(getdate()),DATEPART(hh, GETDATE()),@Count,@ForumID)
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetSortOrder')
	DROP PROCEDURE [bx_GetSortOrder];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_GetSortOrder]
	@PostType tinyint,--1正常,2置顶,3总置顶,5待审核,4回收站
	@PostRandNumber bigint,
	@PostDate datetime,
	@IsThread bit,
	@SortOrder bigint output
AS
BEGIN
	SET NOCOUNT ON;
	IF @PostRandNumber > 0
		SELECT @SortOrder = (CAST(DATEDIFF(second, '1970-01-01 00:00:00', @PostDate) AS bigint) * 100000) + (DATEPART(millisecond, @PostDate) * 100) + @PostRandNumber % 100
	ELSE
		SELECT @SortOrder = (CAST(DATEDIFF(second, '1970-01-01 00:00:00', @PostDate) AS bigint) * 100000) + (DATEPART(millisecond, @PostDate) * 100)
	
	IF @IsThread = 1
		SET @SortOrder = @SortOrder;
	ELSE
		SET @SortOrder = @SortOrder+@PostType*1000000000000000;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Common_GetRecordsByPage_LongCondition')
	DROP PROCEDURE [bx_Common_GetRecordsByPage_LongCondition];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Common_GetRecordsByPage_LongCondition
	@PageIndex int,
	@PageSize int,
	@TableName nvarchar(256),
	@SelectFields varchar(8000) = N'*', --查询字段，默认为 *
	@Condition nvarchar(4000) = N'',     --条件例如"DirectoryID=4"
	@SortField nvarchar(256) = N'[SortOrder]',
	@IsDesc bit = 1 --是否倒序
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @SQLString nvarchar(4000);
	DECLARE @WhereString1 nvarchar(4000);
	DECLARE @WhereString2 nvarchar(4000);

	IF @Condition IS NULL OR @Condition = N'' BEGIN
		SELECT @WhereString1 = N'';
		SELECT @WhereString2 = N'WHERE ';
	END
	ELSE BEGIN
		SELECT @WhereString1 = N'WHERE ' + @Condition;
		SELECT @WhereString2 = N'WHERE ' + @Condition + N' AND ';
	END

	IF @PageIndex = 0 BEGIN
		SELECT @SQLString = N'SELECT TOP ' + STR(@PageSize)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName + ' WITH (NOLOCK)
			' + @WhereString1 + '
			ORDER BY ' + @SortField;
		IF @IsDesc = 1
			SELECT @SQLString = @SQLString + ' DESC';
	END
	ELSE BEGIN
		IF @IsDesc = 1
			SELECT @SQLString = 'SELECT TOP ' + STR(@PageSize)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName+' WITH (NOLOCK) 
			' + @WhereString2 + @SortField + ' <
				(SELECT Min(' + @SortField + ') FROM 
					(SELECT TOP ' + STR(@PageSize * @PageIndex) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
						' + @WhereString1 + '
							ORDER BY ' + @SortField + ' DESC) AS IDX)
			ORDER BY ' + @SortField + ' DESC';
		ELSE
			SELECT @SQLString = 'SELECT TOP ' + STR(@PageSize)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName+' WITH (NOLOCK) 
			' + @WhereString2 + @SortField + ' >
				(SELECT Max(' + @SortField + ') FROM 
					(SELECT TOP ' + STR(@PageSize * @PageIndex) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
						' + @WhereString1 + '
							ORDER BY ' + @SortField + ' ASC) AS IDX)
			ORDER BY ' + @SortField + ' ASC';
	END

	EXEC (@SQLString);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateSortOrder')
	DROP PROCEDURE [bx_UpdateSortOrder];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_UpdateSortOrder]
	@PostType tinyint,--1正常,2置顶,3总置顶,4待审核,5回收站 
	@OldSortOrder bigint,
	@SortOrder bigint output
AS
BEGIN
	SET NOCOUNT ON;

	SET @SortOrder = (@OldSortOrder + (@PostType - FLOOR(@OldSortOrder / 1000000000000000)) * 1000000000000000);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetDisabledTriggerForumIDs')
	DROP PROCEDURE [bx_GetDisabledTriggerForumIDs];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_GetDisabledTriggerForumIDs]
				@ForumIDs nvarchar(64) output
			AS
			BEGIN
				SET NOCOUNT ON;
				set @ForumIDs='';
			END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetPolemizePosts')
	DROP PROCEDURE [bx_GetPolemizePosts];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_GetPolemizePosts]
	@ThreadID int,
	@PageIndex int,
	@PageSize int,
	@PostType tinyint
AS
	SET NOCOUNT ON;
	
	DECLARE @TotalCount INT;
	DECLARE @Condition nvarchar(4000);
	
	SELECT @TotalCount=COUNT(*) FROM [bx_Posts] WHERE [ThreadID] = @ThreadID AND  PostType = @PostType AND [SortOrder]<4000000000000000 ;
	
	
	SET @Condition = '[ThreadID]=' + CAST(@ThreadID as varchar(16))+' AND  PostType = '+CAST(@PostType AS VARCHAR(16))+' AND [SortOrder]<4000000000000000 ';
	
	DECLARE @SQLString Nvarchar(4000);
		
	SET @SQLString='';
	
	DECLARE @ResetOrder bit
	EXECUTE bx_Common_GetRecordsByPageSQLString 
				@PageIndex,
				@PageSize,
				N'bx_Posts',
				N'*',
				@Condition,
				N'[SortOrder]',
				1,
				@TotalCount,
				@ResetOrder OUTPUT,
				@SQLString OUTPUT
	
	EXECUTE (@SQLString)
	
	SELECT @TotalCount,@ResetOrder;
	
	
	EXECUTE bx_Common_GetRecordsByPageSQLString 
			@PageIndex,
			@PageSize,
			N'bx_Posts',
			N'PostID,HistoryAttachmentIDs',
			@Condition,
			N'[SortOrder]',
			1,
			@TotalCount,
			@ResetOrder OUTPUT,
			@SQLString OUTPUT
	
	EXEC ('DECLARE @PostIDTable table(ID int identity(1,1), PostID int NOT NULL, HistoryAttachmentIDs varchar(500) NULL);

	INSERT INTO @PostIDTable (PostID,HistoryAttachmentIDs) ' + @SQLString + ';
	
	DECLARE @HistoryAttach table(PostID int,AttachID int);
	DECLARE @Count int,@I int;
	SELECT @Count = COUNT(*) FROM @PostIDTable;
	SET @I = 1;
	
	WHILE(@I < @Count+1) BEGIN
		DECLARE @PID int,@HistoryAttachmentString varchar(500);
		SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs FROM @PostIDTable WHERE ID = @I;
		IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
			INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
		SET @I = @I + 1;
	END
	

	
	SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;
	SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a, @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
	
	SELECT m.* FROM [bx_PostMarks] m WITH (NOLOCK) INNER JOIN @PostIDTable i ON m.PostID = i.PostID ORDER BY m.PostMarkID DESC;')
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetPostFloor')
	DROP PROCEDURE [bx_SetPostFloor];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetPostFloor
	@ThreadID int
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Count int;

    DECLARE @T TABLE(id int identity(1,1),pid int);
    INSERT INTO @T(pid) SELECT PostID FROM bx_Posts WHERE ThreadID = @ThreadID ORDER BY SortOrder;

    SELECT @Count = @@rowcount;

    UPDATE bx_Posts SET FloorNumber = id FROM @T  WHERE bx_Posts.PostID = pid;
    UPDATE bx_Threads SET PostedCount = @Count WHERE ThreadID = @ThreadID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateForumThreadCatalogsData')
	DROP PROCEDURE [bx_UpdateForumThreadCatalogsData];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_UpdateForumThreadCatalogsData]
	@ForumID INT,
	@ThreadCatalogID INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [bx_ThreadCatalogsInForums] SET TotalThreads=(
		SELECT COUNT(1) FROM bx_Threads WITH(NOLOCK) WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID AND SortOrder<4000000000000000
		) WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteThreads')
	DROP PROCEDURE [bx_DeleteThreads];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteThreads
	@ForumID int,
	@ThreadStatus tinyint,
	@ThreadIdentities varchar(8000)
AS
	SET NOCOUNT ON 

	IF @ThreadStatus<4
		EXEC ('DELETE [bx_Threads] WHERE  [ForumID]=' + @ForumID+' AND [ThreadID] IN (' + @ThreadIdentities + ')')
	ELSE IF @ThreadStatus=4
		EXEC ('DELETE [bx_Threads] WHERE  [ForumID]=' + @ForumID+' AND [ThreadID] IN (' + @ThreadIdentities + ') AND ThreadStatus = 4')
	ELSE
		EXEC ('DELETE [bx_Threads] WHERE  [ForumID]=' + @ForumID+' AND [ThreadID] IN (' + @ThreadIdentities + ') AND ThreadStatus = 5')

	DECLARE @RowCount int
	SET @RowCount=@@ROWCOUNT
	IF @RowCount> 0
	BEGIN
		EXEC [bx_DoCreateStat] @ForumID,7, @RowCount
		RETURN (0);
	END
	ELSE
	begin
		RETURN (1)
	end
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetThreadByID')
	DROP PROCEDURE [bx_GetThreadByID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetThreadByID
    @ThreadID int
AS
BEGIN
    SET NOCOUNT ON;

	SELECT  *  FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID = @ThreadID;
     
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetPosts')
	DROP PROCEDURE [bx_GetPosts];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetPosts
	@ThreadID int
AS
	SET NOCOUNT ON
	SELECT * FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID=@ThreadID AND [SortOrder]<4000000000000000 ORDER BY [SortOrder]
	RETURN
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetThreadsLock')
	DROP PROCEDURE [bx_SetThreadsLock];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetThreadsLock
	@ForumID                    int,
	@ThreadIdentities           varchar(8000),
	@LockThread                 bit
AS
BEGIN
	SET NOCOUNT ON 
	EXEC ('UPDATE bx_Threads SET IsLocked='+@LockThread+' WHERE ThreadID IN (' + @ThreadIdentities + ') AND ForumID = ' + @ForumID) 
	RETURN
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_MoveThreads')
	DROP PROCEDURE [bx_MoveThreads];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_MoveThreads 
	@OldForumID                 int,
	@NewForumID                 int,
	@ThreadIdentities           varchar(8000),
	@IsKeepLink                 bit
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE ForumID=@NewForumID AND ParentID<>0)
		BEGIN
			DECLARE @ThreadID int,@SortOrder bigint,@i int
			BEGIN TRANSACTION

			SET @ThreadIdentities=@ThreadIdentities+N','
			SELECT @i=CHARINDEX(',',@ThreadIdentities)
			
			DECLARE @tempTable1 TABLE(TempThreadID INT,TempSortOrder BIGINT)
			DECLARE @tempTable2 TABLE(TempThreadID INT,TempSortOrder BIGINT,TempThreadCatalogID INT,TempThreadType TINYINT,TempThreadStatus TINYINT,TempIconID INT,TempSubject NVARCHAR(256),TempPostUserID INT,TempPostNickName NVARCHAR(64),TempLastPostUserID INT,TempLastPostNickName NVARCHAR(64),TempCreateDate datetime,TempUpdateDate datetime,TempLastPostID int)
			DECLARE @tempTable3 TABLE(TempCatalogID INT,TempTotalThreads INT)

			SELECT @SortOrder=ISNULL(MAX(SortOrder),0) FROM bx_Threads WITH (NOLOCK)

			DECLARE @ThreadCount INT
			SET @ThreadCount=0;
			WHILE(@i>1) BEGIN
				SELECT @ThreadID=SUBSTRING(@ThreadIdentities,0, @i)
				IF(@IsKeepLink=1) BEGIN
                    DECLARE @LSortOrder bigint;
                    DECLARE @LPostID int;
                    SELECT @LSortOrder = ISNULL(MAX(SortOrder),0)+1 FROM bx_Posts WITH (NOLOCK) WHERE SortOrder<2000000000000000;
                    INSERT INTO bx_Posts(ForumID,ThreadID,Subject,Content,NickName,SortOrder) VALUES(@OldForumID,@ThreadID,'','','',@LSortOrder);
                    IF @@error<>0 BEGIN
				        ROLLBACK TRANSACTION
				        RETURN (-1)
			        END

                    SELECT @LPostID = @@IDENTITY;
                    DELETE bx_Posts WHERE PostID = @LPostID;                    
                    IF @@error<>0 BEGIN
				        ROLLBACK TRANSACTION
				        RETURN (-1)
			        END

					INSERT INTO @tempTable2 SELECT @ThreadID,SortOrder,ThreadCatalogID,ThreadType,6,IconID,(CAST(ThreadID as nvarchar(16))+N','+Subject),PostUserID,PostNickName,LastPostUserID,LastPostNickName,CreateDate,UpdateDate,@LPostID FROM  bx_Threads WITH (NOLOCK) WHERE ForumID = @OldForumID AND ThreadID=@ThreadID
					DECLARE @CatalogID int
					SELECT @CatalogID=TempThreadCatalogID FROM @tempTable2 WHERE  TempThreadID=@ThreadID
					IF EXISTS(SELECT * FROM @tempTable3 WHERE TempCatalogID=@CatalogID) 
						UPDATE @tempTable3 SET TempTotalThreads=TempTotalThreads+1 WHERE TempCatalogID=@CatalogID
					ELSE
						INSERT INTO @tempTable3 VALUES(@CatalogID,1)
				END
				SET @SortOrder=@SortOrder+1;
				INSERT INTO @tempTable1 VALUES(@ThreadID,@SortOrder)

				SELECT @ThreadIdentities=SUBSTRING(@ThreadIdentities,@i+1,LEN(@ThreadIdentities)-@i)
				SELECT @i=CHARINDEX(',',@ThreadIdentities)
				
				SET @ThreadCount=@ThreadCount+1;
			END

			UPDATE bx_Threads SET ForumID=@NewForumID,SortOrder=TempSortOrder FROM @tempTable1 WHERE ThreadID = TempThreadID
			IF @@error<>0 BEGIN
				ROLLBACK TRANSACTION
				RETURN (-1)
			END
			IF(@IsKeepLink=1) BEGIN
				INSERT INTO bx_Threads(ForumID,ThreadCatalogID,ThreadType,IconID,Subject,PostUserID,PostNickName,LastPostUserID,LastPostNickName,CreateDate,UpdateDate,SortOrder,LastPostID) select @OldForumID,TempThreadCatalogID,12,TempIconID,TempSubject,TempPostUserID,TempPostNickName,TempLastPostUserID,TempLastPostNickName,TempCreateDate,TempUpdateDate,TempSortOrder,TempLastPostID FROM @tempTable2
				IF @@error<>0 BEGIN
					ROLLBACK TRANSACTION
					RETURN (-1)
				END
				UPDATE bx_Forums SET TotalThreads=TotalThreads+@ThreadCount WHERE  ForumID=@OldForumID
				UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+T3.TempTotalThreads FROM  @tempTable3 T3 WHERE ForumID=@OldForumID AND ThreadCatalogID=T3.TempCatalogID
			END
			COMMIT TRANSACTION
			RETURN 0
		END
	ELSE
		RETURN (-1)

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetThreadsSubjectStyle')
	DROP PROCEDURE [bx_SetThreadsSubjectStyle];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetThreadsSubjectStyle
	@ForumID            int,
	@ThreadIdentities   varchar(8000),
	@Style              nvarchar(300)
AS
BEGIN
	SET NOCOUNT ON
	EXEC ('UPDATE [bx_Threads] SET SubjectStyle='''+@Style+''' WHERE [ThreadID] IN (' + @ThreadIdentities + ') AND ForumID = ' + @ForumID) 
	IF @@ROWCOUNT > 0
		RETURN (0)
	ELSE
		RETURN (1)
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SplitThread')
	DROP PROCEDURE [bx_SplitThread];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SplitThread 
	@ThreadID int,
	@PostIdentities varchar(8000),
	@NewSubject nvarchar(256)
AS
	SET NOCOUNT ON 
	DECLARE @SortOrder bigint,@NewThreadID int,@TotalReplies int,@PostUserID int,@PostNickName nvarchar(64),@LastPostUserID int,@LastPostNickName nvarchar(64)
	
	Declare @ForumID int,@ThreadCatalogID int
	SELECT @ForumID = ForumID,@ThreadCatalogID=ThreadCatalogID FROM bx_Threads WITH(NOLOCK) WHERE ThreadID=@ThreadID
	
	DECLARE @E1 int,@E2 int,@E3 int,@E4 int
	BEGIN TRANSACTION
	SELECT @SortOrder = ISNULL(MAX(SortOrder)+1,0) FROM bx_Threads WITH (NOLOCK) WHERE ThreadStatus = 1;
	INSERT bx_Threads(ForumID,ThreadCatalogID,ThreadType,IconID,Subject,PostUserID,PostNickName,SortOrder,LastPostID,ThreadStatus) select ForumID,ThreadCatalogID,ThreadType,IconID,@NewSubject,PostUserID,PostNickName,@SortOrder,0,ThreadStatus from bx_Threads with(nolock) where ThreadID=@ThreadID
	SET @NewThreadID = @@IDENTITY
    
    
    
	
	EXEC ('UPDATE [bx_Posts] SET ThreadID=' + @NewThreadID + ' WHERE [PostID] IN (' + @PostIdentities + ') AND [ThreadID]=' + @ThreadID) 
	SELECT @E1 = @@error

    DECLARE @LastPostIDTable table(Tid int,TLastPostID int);
    DECLARE @OldLastPostID int,@NewLastPostID int;
    SELECT @OldLastPostID = MAX(PostID) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID AND SortOrder<4000000000000000;
    SELECT @NewLastPostID = MAX(PostID) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @NewThreadID AND SortOrder<4000000000000000;

    INSERT INTO @LastPostIDTable(Tid,TLastPostID) VALUES(@ThreadID,@OldLastPostID);
    INSERT INTO @LastPostIDTable(Tid,TLastPostID) VALUES(@NewThreadID,@NewLastPostID);

    UPDATE bx_Threads SET LastPostID = TLastPostID FROM @LastPostIDTable WHERE ThreadID = Tid;
    SELECT @E2 = @@error
	
	SELECT TOP 1 @PostUserID=UserID,@PostNickName=NickName FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID=@NewThreadID AND SortOrder<4000000000000000 ORDER BY PostID
	SELECT TOP 1 @LastPostUserID=UserID,@LastPostNickName=NickName FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID=@NewThreadID AND SortOrder<4000000000000000 ORDER BY PostID DESC
	
	UPDATE [bx_Posts] SET PostType=1 WHERE PostID=(SELECT MIN(PostID) FROM [bx_Posts] WITH (NOLOCK) WHERE ForumID=@ForumID AND ThreadID=@NewThreadID)
	SELECT @TotalReplies = COUNT(*)-1 FROM [bx_Posts] WITH(NOLOCK) WHERE ForumID=@ForumID AND ThreadID=@NewThreadID AND SortOrder<4000000000000000--第一个Post是主题内容，不属于回复，所以减1
	UPDATE bx_Threads SET TotalReplies = @TotalReplies, TotalViews = @TotalReplies, PostUserID = @PostUserID, PostNickName = @PostNickName, LastPostUserID = @LastPostUserID, LastPostNickName = @LastPostNickName where ThreadID = @NewThreadID
	SELECT @E3 = @@error
	
	SELECT TOP 1 @LastPostUserID = UserID, @LastPostNickName = NickName FROM [bx_Posts] WITH(NOLOCK)  WHERE ThreadID = @ThreadID AND SortOrder<4000000000000000 ORDER BY PostID DESC
	DECLARE @OldTotalReplies int
	SELECT @OldTotalReplies = COUNT(*) - 1 FROM [bx_Posts] WITH (NOLOCK) WHERE ThreadID = @ThreadID AND SortOrder<4000000000000000 --第一个Post是主题内容，不属于回复，所以减1
	UPDATE bx_Threads SET TotalReplies = @OldTotalReplies, TotalViews = TotalViews - (@TotalReplies + 1), LastPostUserID = @LastPostUserID, LastPostNickName = @LastPostNickName WHERE ThreadID = @ThreadID
	SELECT @E4 = @@error
	
	IF(@E1 = 0 AND @E2 = 0 AND @E3 = 0 AND @E4 = 0)
	BEGIN
        EXECUTE bx_SetPostFloor @NewThreadID;
        EXECUTE bx_SetPostFloor @ThreadID;
		COMMIT TRANSACTION
		UPDATE [bx_Forums] SET [TotalThreads] = TotalThreads+1, [TodayThreads]=TodayThreads+1,[LastThreadID]=@NewThreadID WHERE [ForumID] = @ForumID;
		EXECUTE bx_UpdateForumThreadCatalogsData @ForumID,@ThreadCatalogID
		RETURN (0)
	END
	ELSE
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeletePosts')
	DROP PROCEDURE [bx_DeletePosts];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeletePosts 
	@ForumID int,
	@ThreadID int,
	@UserID int,
	@IsDeleteAnyPost bit,
	@PostIdentities varchar(8000)
AS
BEGIN

	SET NOCOUNT ON 

	IF(@IsDeleteAnyPost=1)
	BEGIN
		EXEC ('DELETE [bx_Posts] WHERE [ForumID]=' + @ForumID + ' AND [ThreadID]=' + @ThreadID + ' AND [PostID] IN (' + @PostIdentities + ')')
	END
	ELSE
		BEGIN
			DECLARE @Count int,@SQLString nvarchar(4000)
			SET @SQLString = N'SELECT @Count=count(*) FROM [bx_Posts] WITH(NOLOCK) WHERE UserID <> '+str(@UserID)+' AND [ForumID]=' + str(@ForumID) + ' AND [ThreadID]=' + str(@ThreadID) + ' AND [PostID] IN (' + @PostIdentities + ')'
			EXECUTE sp_executesql @SQLString,N'@Count int output',@Count output
			IF(@Count>0)
				RETURN 101
			ELSE
				EXEC ('DELETE [bx_Posts] WHERE [UserID]=' + @UserID + ' AND [ForumID]=' + @ForumID + ' AND [ThreadID]=' + @ThreadID + ' AND [PostID] IN (' + @PostIdentities + ')') 
		END
	DECLARE @RowCount int
	SET @RowCount = @@ROWCOUNT
	IF @RowCount > 0 BEGIN
		EXEC [bx_DoCreateStat] @ForumID,8, @RowCount
		RETURN (0)
	END
	ELSE
		RETURN (1)

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_JoinThreads')
	DROP PROCEDURE [bx_v5_JoinThreads];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_JoinThreads
	@OldThreadID int,
	@NewThreadID int,
	@IsKeepLink bit
AS
	SET NOCOUNT ON 
	IF EXISTS (SELECT * FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@NewThreadID AND ThreadType<10 AND ThreadStatus<4)
	BEGIN
		DECLARE @NewForumID int,@OldForumID int,@OldThreadCatalogID int
		DECLARE @TotalReplies int,@TotalViews int,@LastPostUserID int,@LastPostNickName nvarchar(64)
		SELECT @OldForumID=ForumID,@OldThreadCatalogID=ThreadCatalogID,@TotalReplies=TotalReplies+1,@TotalViews=TotalViews FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@OldThreadID
		SELECT @NewForumID=ForumID FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@NewThreadID
		
		DECLARE @E1 int,@E2 int,@E3 int,@E4 int
		BEGIN TRANSACTION

        DECLARE @OldThreadContentSortOrder bigint,@NewThreadContentSortOrder bigint;
        DECLARE @OldThreadContentID int,@NewThreadContentID int;
        SELECT @OldThreadContentSortOrder = SortOrder,@OldThreadContentID = PostID FROM [bx_Posts] WITH (NOLOCK) WHERE ThreadID=@OldThreadID AND PostType = 1;
        SELECT @NewThreadContentSortOrder = SortOrder,@NewThreadContentID = PostID FROM [bx_Posts] WITH (NOLOCK) WHERE ThreadID=@NewThreadID AND PostType = 1;

        IF @OldThreadContentSortOrder < @NewThreadContentSortOrder BEGIN
            DECLARE @Temp table(pid int,psortorder bigint);
            INSERT INTO @Temp(pid,psortorder) VALUES(@OldThreadContentID,@NewThreadContentSortOrder);
            INSERT INTO @Temp(pid,psortorder) VALUES(@NewThreadContentID,@OldThreadContentSortOrder);
            UPDATE [bx_Posts] SET SortOrder = psortorder FROM @Temp WHERE PostID = pid;
        END
            

		UPDATE [bx_Posts] SET PostType = 0 WHERE ThreadID=@OldThreadID AND PostType = 1;
		SELECT @E4=@@error
		
		UPDATE [bx_Posts] SET ForumID=@NewForumID, ThreadID=@NewThreadID WHERE ThreadID=@OldThreadID;
		SELECT @E1=@@error
		

		SELECT top 1 @LastPostUserID=UserID,@LastPostNickName=NickName FROM [bx_Posts] WITH (NOLOCK) WHERE ForumID=@NewForumID AND ThreadID=@NewThreadID AND SortOrder<4000000000000000 order by PostID DESC
		
		UPDATE [bx_Threads] SET TotalReplies=TotalReplies+@TotalReplies,TotalViews=TotalViews+@TotalViews,LastPostUserID=@LastPostUserID,LastPostNickName=@LastPostNickName WHERE ThreadID=@NewThreadID
		SELECT @E2=@@error
		
		IF (@IsKeepLink=1)
			BEGIN
			UPDATE [bx_Threads] SET Subject=(CAST(@NewThreadID as nvarchar(16))+N','+Subject),ThreadType=11 WHERE ThreadID=@OldThreadID
			SELECT @E3=@@error
			END
		ELSE
			BEGIN
			DELETE [bx_Threads] WHERE ThreadID=@OldThreadID
			SELECT @E3=@@error
			END
			
		IF(@E1=0 AND @E2=0 AND @E3=0 AND @E4=0)
			BEGIN
            
            EXECUTE bx_SetPostFloor @NewThreadID;
            
			COMMIT TRANSACTION
			IF(@OldForumID<>@NewForumID) BEGIN
				IF (@IsKeepLink=1) BEGIN
					UPDATE [bx_Forums] SET [TotalPosts] = TotalPosts-@TotalReplies WHERE [ForumID] = @OldForumID;
				END
				UPDATE [bx_Forums] SET [TotalPosts] = TotalPosts+@TotalReplies WHERE [ForumID] = @NewForumID;
			END
			
			RETURN (0)
			END
		ELSE
			BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
			END
	END
	ELSE
		RETURN (-1)
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetThreadCount')
	DROP PROCEDURE [bx_GetThreadCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetThreadCount
    @ForumID        int
   ,@IncludeStick   bit
   ,@BeginDate      DateTime
   ,@EndDate        DateTime
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ThreadStatus tinyint;
    IF @IncludeStick = 1
        SET @ThreadStatus = 4;
    ELSE
        SET @ThreadStatus = 2;

    IF @BeginDate IS NULL AND @EndDate IS NULL
        SELECT COUNT(*) FROM [bx_Threads] WITH (NOLOCK) WHERE [ForumID] = @ForumID AND [ThreadStatus] < @ThreadStatus;
    ELSE IF @BeginDate IS NULL
        SELECT COUNT(*) FROM [bx_Threads] WITH (NOLOCK) WHERE [ForumID] = @ForumID AND [ThreadStatus] < @ThreadStatus AND [CreateDate] <= @EndDate;
    ELSE
        SELECT COUNT(*) FROM [bx_Threads] WITH (NOLOCK) WHERE [ForumID] = @ForumID AND [ThreadStatus] < @ThreadStatus AND [CreateDate] >= @BeginDate; 
     
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetStickThreads')
	DROP PROCEDURE [bx_v5_GetStickThreads];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetStickThreads
    @ForumID int
AS
BEGIN
    SET NOCOUNT ON;

	SELECT  *  FROM [bx_Threads] WITH (NOLOCK) WHERE [ThreadStatus] = 2 AND (ForumID = @ForumID OR ThreadID IN(SELECT ThreadID FROM bx_StickThreads WHERE ForumID = @ForumID)) ORDER BY SortOrder DESC;
     
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetGlobalThreads')
	DROP PROCEDURE [bx_GetGlobalThreads];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetGlobalThreads
AS
BEGIN
    SET NOCOUNT ON;

	SELECT  *  FROM [bx_Threads] WITH (NOLOCK) WHERE [ThreadStatus] = 3 ORDER BY [SortOrder] DESC;
     
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUnapprovedPosts')
	DROP PROCEDURE [bx_GetUnapprovedPosts];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_GetUnapprovedPosts]
	@ForumID int
AS
	SET NOCOUNT ON 
	
	IF(@ForumID>0)
	begin
		SELECT * FROM [bx_Threads] WITH(NOLOCK) WHERE ForumID=@ForumID AND ThreadID IN(SELECT DISTINCT(ThreadID) FROM [bx_Posts] WHERE ForumID=@ForumID AND SortOrder >= 5000000000000000)
		SELECT * FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID IN (SELECT ThreadID FROM [bx_Threads]  WITH(NOLOCK) WHERE ForumID=@ForumID) AND ForumID=@ForumID AND SortOrder >= 5000000000000000 ORDER BY ThreadID,SortOrder DESC
	end
	ELSE
	begin
		SELECT * FROM [bx_Threads] WITH(NOLOCK) WHERE ThreadID IN(SELECT DISTINCT(ThreadID) FROM [bx_Posts] WHERE SortOrder >= 5000000000000000)
		SELECT * FROM [bx_Posts] WITH(NOLOCK) WHERE SortOrder >= 5000000000000000 ORDER BY ThreadID,SortOrder DESC
	end
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUnapprovedPostThreads')
	DROP PROCEDURE [bx_GetUnapprovedPostThreads];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUnapprovedPostThreads
	@ForumID int,--大于0时按版块获取
	@UserID int,--大于0时按用户获取
	@PageIndex int,
	@PageSize int
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @Condition varchar(8000),@User nvarchar(100)
	
	IF @UserID>0
		SET @User=' P.UserID='+str(@UserID)+' AND '
	ELSE
		SET @User=''

	IF @ForumID=0
		SET @Condition='ThreadID in (SELECT DISTINCT P.ThreadID FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T WITH (NOLOCK) ON P.ThreadID=T.ThreadID WHERE '+@User+' P.SortOrder >= 5000000000000000 AND T.ThreadStatus<4)'
	ELSE
		SET @Condition='ThreadID in (SELECT DISTINCT P.ThreadID FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T WITH (NOLOCK) ON P.ThreadID=T.ThreadID WHERE '+@User+' T.ForumID='+str(@ForumID)+' AND P.SortOrder >= 5000000000000000 AND T.ThreadStatus<4)'

	DECLARE @TotalCount INT,@SQLString NVARCHAR(4000)
	
	IF @ForumID=0
		SET @SQLString='SELECT @TotalCount=COUNT(DISTINCT P.ThreadID) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T WITH (NOLOCK) ON P.ThreadID=T.ThreadID WHERE '+@User+' P.SortOrder >= 5000000000000000 AND T.ThreadStatus<4'
	ELSE
		SET @SQLString='SELECT @TotalCount=COUNT(DISTINCT P.ThreadID) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T WITH (NOLOCK) ON P.ThreadID=T.ThreadID WHERE '+@User+' P.SortOrder >= 5000000000000000 AND T.ForumID='+str(@ForumID)+' AND T.ThreadStatus<4'
	EXECUTE sp_executesql @SQLString,N'@TotalCount int output',@TotalCount output


	DECLARE @ResetOrder bit----------- 1表示读取数据的时候 排序要反过来
	EXECUTE bx_Common_GetRecordsByPageSQLString
						@PageIndex,
						@PageSize,
						N'bx_Threads',
						N'ThreadID',
						@Condition,
						N'[SortOrder]',
						1,
						@TotalCount,
						@ResetOrder OUTPUT,
						@SQLString OUTPUT
	
	EXEC ('DECLARE @ThreadIDTable table(ThreadID int NOT NULL);

		INSERT INTO @ThreadIDTable ' + @SQLString + ';

		SELECT T.*
		,(SELECT COUNT(1) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID=T.ThreadID  AND SortOrder >= 5000000000000000) as UnApprovedPostsCount FROM bx_Threads T WITH (NOLOCK)  WHERE T.ThreadID in (SELECT ThreadID FROM @ThreadIDTable) ORDER BY SortOrder DESC')
		
	
	
	SELECT @TotalCount; 
     
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetUnapprovedPostThread')
	DROP PROCEDURE [bx_v5_GetUnapprovedPostThread];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetUnapprovedPostThread
	@ThreadID       int,
	@UserID         int,
	@PageIndex      int,
	@PageSize       int,
    @TopMarkCount   int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Condition varchar(8000),@User nvarchar(100)
	IF @UserID IS NOT NULL
		SET @User='UserID='+str(@UserID)+' AND '
	ELSE
		SET @User=''

	SET @Condition=' ('+@User+' ThreadID='+str(@ThreadID)+' AND SortOrder >= 5000000000000000)'

    SELECT  *  FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@ThreadID;

    DECLARE @TotalCount int;
	IF @UserID>0
		SELECT @TotalCount = COUNT(*) FROM bx_Posts WITH (NOLOCK) WHERE UserID = @UserID AND ThreadID= @ThreadID AND SortOrder >= 5000000000000000;
	ELSE
		SELECT @TotalCount = COUNT(*) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID= @ThreadID AND SortOrder >= 5000000000000000;

    SELECT @TotalCount;

    DECLARE @SQLString nvarchar(4000);
    DECLARE @PostFieldsString nvarchar(4000);
    DECLARE @ResetOrder bit;

    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    '
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

        

        SET @PostFieldsString = '

DECLARE @S2 VARCHAR(8000),@D2 DATETIME;

            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
SELECT @D2 = GETDATE();
		    INSERT INTO @PostIDTable (

[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]

) ' + @SQLString + ';

    SET @S2 =''|A1=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
'+
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);

    SET @S2 =@S2 + ''|A8=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            
            SELECT @S2;
'
; EXEC (@PostFieldsString); 
	
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetPolemizeWithReplies')
	DROP PROCEDURE [bx_GetPolemizeWithReplies];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetPolemizeWithReplies
     @ThreadID          int
    ,@PageIndex         int
    ,@PageSize          int
    ,@TotalCount        int
    ,@GetExtendedInfo   bit    
    ,@GetThread         bit 
    ,@GetThreadContent  bit
    ,@CheckThreadType   bit
    ,@PostType          tinyint
    ,@TopMarkCount      int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RealThreadType tinyint;
    SELECT @RealThreadType = ThreadType FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID; 

    IF @CheckThreadType = 1 BEGIN

        IF @RealThreadType >= 10
            SET @RealThreadType = 0;
        SELECT @RealThreadType;
        IF @RealThreadType != 4 
            RETURN;

    END 

    DECLARE @GetPostCondition nvarchar(4000);
    SET @GetPostCondition = '';
    
    IF @GetThreadContent = 1
        SET @GetPostCondition = ' ThreadID = ' + CAST(@ThreadID as varchar(16));

    DECLARE @SQLString nvarchar(4000);
    DECLARE @PostFieldsString nvarchar(4000);

    IF @GetPostCondition <> '' BEGIN
        SELECT 1;
        IF @GetExtendedInfo = 0
            EXEC('SELECT TOP 1 * FROM bx_Posts WITH (NOLOCK) WHERE '+ @GetPostCondition + ' ORDER BY PostID');
        ELSE BEGIN
            SET @SQLString = 'SELECT TOP 1 
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
 FROM bx_Posts WITH (NOLOCK) WHERE ' + @GetPostCondition + ' ORDER BY PostID';
            

        SET @PostFieldsString = '

DECLARE @S2 VARCHAR(8000),@D2 DATETIME;

            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
SELECT @D2 = GETDATE();
		    INSERT INTO @PostIDTable (

[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]

) ' + @SQLString + ';

    SET @S2 =''|A1=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
'+
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);

    SET @S2 =@S2 + ''|A8=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            
            SELECT @S2;
'
; EXEC (@PostFieldsString); 
        END
    END
    ELSE
        SELECT 0;
    
    IF @GetThread = 1 BEGIN
        SELECT  *  FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID AND ThreadStatus < 4;
    END    


    IF @TotalCount IS NULL BEGIN
        IF @PostType IS NULL
            SELECT @TotalCount = TotalReplies + 1 FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID;
        ELSE
            SELECT @TotalCount = Count(*) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID AND PostType = @PostType AND [SortOrder]<4000000000000000;
    END

    SELECT @TotalCount;

    IF (@PageIndex * @PageSize) > @TotalCount BEGIN --如果大于最大页 就取最后页
        SET @PageIndex = @TotalCount / @PageSize - 1;
        IF @TotalCount = 0 OR @TotalCount % @PageSize > 0
            SET @PageIndex = @PageIndex + 1;
    END

	SET @SQLString='';

    DECLARE @Condition nvarchar(4000);
    
    IF @PostType IS NULL
        SET @Condition = '[ThreadID]=' + CAST(@ThreadID as varchar(16)) + ' AND [SortOrder]<4000000000000000 ';
    ELSE
        SET @Condition = '[ThreadID]=' + CAST(@ThreadID as varchar(16)) + ' AND [SortOrder]<4000000000000000 AND [PostType]=' + CAST(@PostType as varchar(16));

	DECLARE @ResetOrder bit;

    IF @GetExtendedInfo = 0 BEGIN
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'*',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

        EXECUTE ('' + @SQLString + '');
        
        SELECT @ResetOrder;
    END
    ELSE BEGIN
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

        

        SET @PostFieldsString = '

DECLARE @S2 VARCHAR(8000),@D2 DATETIME;

            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
SELECT @D2 = GETDATE();
		    INSERT INTO @PostIDTable (

[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]

) ' + @SQLString + ';

    SET @S2 =''|A1=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
'+
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);

    SET @S2 =@S2 + ''|A8=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            
            SELECT @S2;
'
; EXEC (@PostFieldsString); 
    END
     
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetPagePosts')
	DROP PROCEDURE [bx_GetPagePosts];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetPagePosts
     @ThreadID          int
    ,@PageIndex         int
    ,@PageSize          int
    ,@TotalCount        int
    ,@ThreadType        int
    ,@GetExtendedInfo   bit    
    ,@GetThread         bit 
    ,@CheckThreadType   bit
    ,@GetBestPost       bit 
    ,@GetThreadContent  bit
    ,@TopMarkCount      int
    ,@OnlyNormal        bit
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @S VARCHAR(8000);

    DECLARE @BestPostID int;--,@ProcessBestPost bit;
    DECLARE @RealThreadType tinyint;
    DECLARE @TempCount int,@ContentID int;
    
    DECLARE @D1 datetime;
    SELECT @D1 = GETDATE();    

    SELECT @RealThreadType = ThreadType,@TempCount = TotalReplies + 1,@ContentID = ContentID  FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID; 
    
    IF @TempCount < 1 BEGIN
        SELECT @TempCount = COUNT(*) FROM bx_Posts WITH(NOLOCK) WHERE ThreadID = @ThreadID;
        UPDATE bx_Threads SET TotalReplies = @TempCount - 1 WHERE ThreadID = @ThreadID;
    END

    SET @S = '1='+  CONVERT(varchar(100), getdate()-@D1, 14) ;

    SET @BestPostID = 0;

    IF @CheckThreadType = 1 BEGIN

        IF @RealThreadType >= 10
            SET @RealThreadType = 0;
        SELECT @RealThreadType;
        IF @RealThreadType != @ThreadType 
            RETURN;

    END 


    DECLARE @SQLString nvarchar(4000),@SQLString2 nvarchar(4000);
    SET @SQLString = '';
    SET @SQLString2 = '';
    
    DECLARE @PostFieldsString nvarchar(4000);
    

        SET @PostFieldsString = '
            DECLARE @S2 VARCHAR(8000),@D2 DATETIME;
            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
'

    IF @RealThreadType = 2 BEGIN --问题帖
            SELECT @BestPostID = BestPostID FROM bx_Questions WITH (NOLOCK) WHERE ThreadID = @ThreadID;
            IF NOT EXISTS(SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @BestPostID AND SortOrder < 4000000000000000) BEGIN -- 最佳答案被删除了
                SET @BestPostID = 0;
                UPDATE bx_Questions SET BestPostID = 0 WHERE ThreadID = @ThreadID; -- 顺便把BestPostID更新为0 免得每次都来查询
            END
        
            IF @BestPostID <> 0 AND @GetBestPost = 1 BEGIN
                IF @GetExtendedInfo = 0
                    SET @SQLString = 'SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @BID;';-- + CAST(@BestPostID AS varchar(16)) + '';
                ELSE BEGIN
                    SET @PostFieldsString = @PostFieldsString + ' INSERT INTO @PostIDTable(
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
) SELECT 
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
 FROM bx_Posts WITH (NOLOCK) WHERE PostID = @BID; ';
                    
                END
            END
    END

    IF @GetThreadContent = 1  BEGIN ---  都取出来吧 AND (@RealThreadType = 2 OR @RealThreadType = 4) --问题帖 或者 辩论帖 才取出主题内容
        
        IF @GetExtendedInfo = 0
            SET @SQLString2 = ' SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @CID;';-- + CAST(@ContentID as varchar(16));
        ELSE
            SET @PostFieldsString = @PostFieldsString + ' INSERT INTO @PostIDTable(
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
) SELECT 
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
 FROM bx_Posts WITH (NOLOCK) WHERE PostID = @CID';

    END

    IF @GetExtendedInfo = 0 BEGIN
        IF @SQLString <> '' OR @SQLString2 <>'' BEGIN
            SELECT 1;
            IF @SQLString <> ''  BEGIN
                SELECT @D1 = GETDATE();
                EXECUTE sp_executesql 
                  @SQLString,
                  N'@BID INT'
                 ,@BID = @BestPostID 
                SET @S =@S + '|2='+  CONVERT(varchar(100), getdate()-@D1, 14) ;
            END
            IF @SQLString2 <> ''  BEGIN
                SELECT @D1 = GETDATE();
                EXECUTE sp_executesql 
                  @SQLString2,
                  N'@CID INT'
                 ,@CID = @ContentID
                SET @S =@S + '|3='+  CONVERT(varchar(100), getdate()-@D1, 14) ;
            END
            IF @SQLString = '' OR @SQLString2 = ''
                SELECT 0 AS PostID;
        END
        ELSE
            SELECT 0;
    END 
    ELSE
        SELECT 0;
    
    IF @GetThread = 1 BEGIN
        IF @OnlyNormal = 1 BEGIN
            SELECT @D1 = GETDATE();
            SELECT  *  FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID AND ThreadStatus < 4;
            
    SET @S =@S + '|4='+  CONVERT(varchar(100), getdate()-@D1, 14) ;
        END
        ELSE BEGIN
            SELECT @D1 = GETDATE();
            SELECT  *  FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID;
            
    SET @S =@S + '|5='+  CONVERT(varchar(100), getdate()-@D1, 14) ;
        END
    END    


    IF @TotalCount IS NULL OR @OnlyNormal = 0 BEGIN
        IF @OnlyNormal = 0 BEGIN
            SELECT @D1 = GETDATE();
            SELECT @TotalCount = Count(*) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID;
            
    SET @S =@S + '|6='+  CONVERT(varchar(100), getdate()-@D1, 14) ;
        END
        ELSE BEGIN
            SELECT @D1 = GETDATE();
            SELECT @TotalCount = @TempCount;
            
    SET @S =@S + '|7='+  CONVERT(varchar(100), getdate()-@D1, 14) ;
        END
    END
    
    IF @OnlyNormal = 0
        SELECT @TotalCount;

    IF @BestPostID <> 0 -- 减掉最佳答案 
        SET @TotalCount = @TotalCount - 1;

    IF (@PageIndex * @PageSize) > @TotalCount BEGIN --如果大于最大页 就取最后页
        SET @PageIndex = @TotalCount / @PageSize - 1;
        IF  @TotalCount = 0 OR @TotalCount % @PageSize > 0
            SET @PageIndex = @PageIndex + 1;
    END

	SET @SQLString='';

    DECLARE @Condition nvarchar(4000);
    
    IF @BestPostID > 0 
        SET @Condition = '[PostID]<>@BID AND [ThreadID]=@TID ';
    ELSE
        SET @Condition = '[ThreadID]=@TID ';

    IF @OnlyNormal = 1
        SET @Condition = @Condition + ' AND [SortOrder]<4000000000000000 ';

	DECLARE @ResetOrder bit;

    IF @GetExtendedInfo = 0 BEGIN
        
        SELECT @D1 = GETDATE();
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'*',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT
        
    SET @S =@S + '|8='+  CONVERT(varchar(100), getdate()-@D1, 14) ;
        SELECT @D1 = GETDATE();

    IF @BestPostID > 0 BEGIN
        EXECUTE sp_executesql 
          @SQLString,
          N'@TID INT,@BID INT',
          @TID = @ThreadID
         ,@BID = @BestPostID
    END
    ELSE BEGIN
        EXECUTE sp_executesql 
          @SQLString,
          N'@TID INT',
          @TID = @ThreadID
    END

    SET @S =@S + '|9='+  CONVERT(varchar(100), getdate()-@D1, 14) ;
        SELECT @ResetOrder;
    END
    ELSE BEGIN
        SELECT @D1 = GETDATE();
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

    SET @PostFieldsString = @PostFieldsString + ' 
    SELECT @D2 = GETDATE();
INSERT INTO @PostIDTable(
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
)'+@SQLString+'
    SET @S2 =@S2 + ''|A1=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
';

    SET @S =@S + '|10='+  CONVERT(varchar(100), getdate()-@D1, 14) ;
        SELECT @D1 = GETDATE();
    SET @PostFieldsString = @PostFieldsString + 
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);

    SET @S2 =@S2 + ''|A8=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            
            SELECT @S2;
'


    IF @BestPostID > 0 BEGIN
        EXECUTE sp_executesql 
          @PostFieldsString,
          N'@TID INT,@BID INT,@CID INT',
          @TID = @ThreadID
         ,@BID = @BestPostID
         ,@CID = @ContentID
    END
    ELSE BEGIN
        EXECUTE sp_executesql 
          @PostFieldsString,
          N'@TID INT,@CID INT',
          @TID = @ThreadID
         ,@CID = @ContentID
    END


    SET @S =@S + '|11='+  CONVERT(varchar(100), getdate()-@D1, 14) ;
        SET @ResetOrder = 0;
        SELECT @ResetOrder;
    END
    SELECT @S;
    
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetThreadUserPosts')
	DROP PROCEDURE [bx_GetThreadUserPosts];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetThreadUserPosts
     @ThreadID          int
    ,@UserID            int
    ,@PageIndex         int
    ,@PageSize          int
    ,@ThreadType        int
    ,@GetExtendedInfo   bit    
    ,@GetThread         bit 
    ,@CheckThreadType   bit
    ,@TopMarkCount      int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RealThreadType tinyint;
    SELECT @RealThreadType = ThreadType FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID; 

    IF @CheckThreadType = 1 BEGIN

        IF @RealThreadType >= 10
            SET @RealThreadType = 0;
        SELECT @RealThreadType;
        IF @RealThreadType != @ThreadType 
            RETURN;

    END 

    
    IF @GetThread = 1 BEGIN
        SELECT  *  FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID AND ThreadStatus < 4;
    END    


    DECLARE @TotalCount int;
    SELECT @TotalCount = Count(*) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID AND UserID = @UserID;

    SELECT @TotalCount;

    IF (@PageIndex * @PageSize) > @TotalCount BEGIN --如果大于最大页 就取最后页
        SET @PageIndex = @TotalCount / @PageSize - 1;
        IF @TotalCount = 0 or @TotalCount % @PageSize > 0
            SET @PageIndex = @PageIndex + 1;
    END

    DECLARE @Condition nvarchar(4000);

    SET @Condition = '[UserID] = ' + CAST(@UserID as varchar(16)) + ' AND [ThreadID]=' + CAST(@ThreadID as varchar(16)) + ' AND [SortOrder]<4000000000000000 ';

	DECLARE @ResetOrder bit,@SQLString nvarchar(4000);

    IF @GetExtendedInfo = 0 BEGIN
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'*',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

        EXECUTE ('' + @SQLString + '');
        
        SELECT @ResetOrder;
    END
    ELSE BEGIN
        DECLARE @PostFieldsString nvarchar(4000);
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

        

        SET @PostFieldsString = '

DECLARE @S2 VARCHAR(8000),@D2 DATETIME;

            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
SELECT @D2 = GETDATE();
		    INSERT INTO @PostIDTable (

[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]

) ' + @SQLString + ';

    SET @S2 =''|A1=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
'+
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);

    SET @S2 =@S2 + ''|A8=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            
            SELECT @S2;
'
; EXEC (@PostFieldsString); 
    END
     
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetBestPost')
	DROP PROCEDURE [bx_v5_GetBestPost];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetBestPost
     @ThreadID       int
    ,@BestPostID     int
    ,@TopMarkCount   int   
AS
BEGIN
    SET NOCOUNT ON;
    IF @BestPostID<>0 AND NOT EXISTS(SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @BestPostID AND SortOrder<4000000000000000) BEGIN -- 最佳答案被删除了
        SET @BestPostID = 0;
        UPDATE bx_Questions SET BestPostID = 0 WHERE ThreadID = @ThreadID; -- 顺便把BestPostID更新为0 免得每次都来查询
    END

    IF @BestPostID <> 0 BEGIN
        DECLARE @SQLString nvarchar(4000);
        DECLARE @PostFieldsString nvarchar(4000);
        SET @SQLString = 'SELECT 
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
 FROM bx_Posts WITH (NOLOCK) WHERE PostID = ' + CAST(@BestPostID AS varchar(16)) + '';
        

        SET @PostFieldsString = '

DECLARE @S2 VARCHAR(8000),@D2 DATETIME;

            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
SELECT @D2 = GETDATE();
		    INSERT INTO @PostIDTable (

[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]

) ' + @SQLString + ';

    SET @S2 =''|A1=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
'+
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);

    SET @S2 =@S2 + ''|A8=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            
            SELECT @S2;
'
; EXEC (@PostFieldsString); 
    END 
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetPost')
	DROP PROCEDURE [bx_v5_GetPost];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetPost
     @PostID              int
    ,@GetExtendedInfo     bit
    ,@TopMarkCount        int   
AS
BEGIN
    SET NOCOUNT ON;
    IF @GetExtendedInfo = 0
        SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @PostID;
    ELSE BEGIN
        DECLARE @SQLString nvarchar(4000);
        DECLARE @PostFieldsString nvarchar(4000);
        SET @SQLString = 'SELECT 
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
 FROM bx_Posts WITH (NOLOCK) WHERE PostID = ' + CAST(@PostID AS varchar(16)) + '';
        

        SET @PostFieldsString = '

DECLARE @S2 VARCHAR(8000),@D2 DATETIME;

            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
SELECT @D2 = GETDATE();
		    INSERT INTO @PostIDTable (

[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]

) ' + @SQLString + ';

    SET @S2 =''|A1=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
'+
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);

    SET @S2 =@S2 + ''|A8=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            
            SELECT @S2;
'
; EXEC (@PostFieldsString); 
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserPosts')
	DROP PROCEDURE [bx_GetUserPosts];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserPosts
     @UserID               int
    ,@BeginDate            datetime
    ,@EndDate              datetime
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [bx_Posts] WITH (NOLOCK) WHERE [UserID] = @UserID AND [CreateDate] >= @BeginDate AND [CreateDate] <= @EndDate;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetThreadFirstPost')
	DROP PROCEDURE [bx_v5_GetThreadFirstPost];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetThreadFirstPost
     @ThreadID            int
    ,@GetExtendedInfo     bit
    ,@TopMarkCount        int   
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @PostID int;
    SELECT @PostID = ContentID FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID;

    IF @GetExtendedInfo = 0
        SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @PostID;
    ELSE BEGIN
        DECLARE @SQLString nvarchar(4000);
        DECLARE @PostFieldsString nvarchar(4000);
        SET @SQLString = 'SELECT 
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
 FROM bx_Posts WITH (NOLOCK) WHERE PostID = ' + CAST(@PostID AS varchar(16)) + '';
        

        SET @PostFieldsString = '

DECLARE @S2 VARCHAR(8000),@D2 DATETIME;

            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
SELECT @D2 = GETDATE();
		    INSERT INTO @PostIDTable (

[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]

) ' + @SQLString + ';

    SET @S2 =''|A1=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
'+
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);

    SET @S2 =@S2 + ''|A8=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            
            SELECT @S2;
'
; EXEC (@PostFieldsString); 
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetPostsByIdentities')
	DROP PROCEDURE [bx_GetPostsByIdentities];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE [bx_GetPostsByIdentities]
	@PostIdentities varchar(8000)
AS
BEGIN
	SET NOCOUNT ON;
	EXEC ('SELECT * FROM [bx_Posts] WITH (NOLOCK) WHERE PostID in (' + @PostIdentities +  ')');
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_CreateThread')
	DROP PROCEDURE [bx_v5_CreateThread];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_CreateThread
	@ForumID                int,
	@ThreadCatalogID        int,
	@ThreadStatus           tinyint,
	@ThreadType             tinyint,
	@IconID                 int,
	@Subject                nvarchar(256),
	@SubjectStyle           nvarchar(300),
	@Price                  int,
	@UserID                 int,
	@NickName               nvarchar(64),
	@IsLocked               bit,
	@IsValued               bit,

	@Content                ntext,
	@ContentFormat          tinyint,
	@EnableSignature        bit,
	@EnableReplyNotice      bit,
	@IPAddress              nvarchar(64),
	
	@AttachmentIds          varchar(8000),
	@AttachmentFileNames    ntext,
	@AttachmentFileIds      text,
	@AttachmentFileSizes    varchar(8000),
	@AttachmentPrices       varchar(8000),
	@AttachmentFileExtNames ntext,
	@HistoryAttachmentIDs   varchar(500),


	@ThreadRandNumber       int,
	@UserTotalThreads       int output,
	@UserTotalPosts         int output,
    @ThreadID               int output,
    @PostID                 int output,
    @ExtendData             ntext
   ,@TopMarkCount                   int
   ,@TempPostID             int
   ,@AttachmentType         tinyint
   ,@Words                  nvarchar(400)
AS
BEGIN

	SET NOCOUNT ON;

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;


	DECLARE @ReturnValue     int;

	DECLARE @TempSortOrder BIGINT,@PostDate datetime
	
	SET @PostDate = getdate();
	EXEC [bx_GetSortOrder] 1, @ThreadRandNumber, @PostDate, 1, @TempSortOrder OUTPUT;
	
    DECLARE @TempThreadID int
    SELECT @TempThreadID = ThreadID FROM bx_Threads WITH(NOLOCK) WHERE LastPostID = @TempPostID;
    IF @TempThreadID IS NOT NULL BEGIN
        DECLARE @LastPostID int;
        SELECT @LastPostID = MAX(PostID) FROM bx_Posts WITH(NOLOCK) WHERE ThreadID = @TempThreadID AND SortOrder<4000000000000000;
        IF @LastPostID IS NULL BEGIN    
            SELECT @LastPostID = MAX(PostID) FROM bx_Posts WITH(NOLOCK) WHERE ThreadID = @TempThreadID;
            IF @LastPostID IS NULL --说明是不正常的主题  没有一个post
                DELETE bx_Threads WHERE ThreadID = @TempThreadID;
        END
        IF @LastPostID IS NOT NULL BEGIN
            UPDATE bx_Threads SET LastPostID = @LastPostID WHERE ThreadID = @TempThreadID;
            SET @ErrorCount = @ErrorCount + @@error;
        END
    END
    
    BEGIN TRAN

	INSERT INTO [bx_Threads]
			([ForumID]
			,[ThreadCatalogID]
			,[ThreadType]
			,[IconID]
			,[Subject]
			,[SubjectStyle]
			,[Price]
			,[PostUserID]
			,[PostNickName]
			,[LastPostUserID]
			,[LastPostNickName]
			,[IsLocked]
			,[IsValued]
			,[SortOrder]
            ,[LastPostID]
            ,[ThreadStatus]
            ,[AttachmentType]
            ,[ExtendData]
            ,[Words]
            ,[PostedCount])
	 VALUES
			(@ForumID
			,@ThreadCatalogID
			,@ThreadType
			,@IconID
			,@Subject
			,@SubjectStyle
			,@Price
			,@UserID
			,@NickName
			,@UserID
			,@NickName
			,@IsLocked
			,@IsValued
			,@TempSortOrder
            ,@TempPostID
            ,@ThreadStatus
            ,@AttachmentType
            ,@ExtendData
            ,@Words
            ,1)
	
    SET @ErrorCount = @ErrorCount + @@error;
		
	SELECT @ThreadID = @@IDENTITY;
	IF(@ThreadID>0) BEGIN
	    EXEC bx_DoCreateStat @ForumID,3, 1
        SET @ErrorCount = @ErrorCount + @@error;
    END
    IF @Words <> '' BEGIN
        INSERT INTO bx_ThreadWords(ThreadID,Word) SELECT @ThreadID,T.item FROM bx_GetStringTable_nvarchar(@Words, N',') T;
        SET @ErrorCount = @ErrorCount + @@error;
    END

	DECLARE @IsApproved bit
	IF @ThreadStatus=5
		SET @IsApproved=0
	ELSE
		SET @IsApproved=1
		
	EXECUTE @ReturnValue = [bx_v5_CreatePost] 
		 0
		,@ThreadID
		,1
		,@IconID
		,@Subject
		,@Content
		,@ContentFormat
		,@EnableSignature

		,@EnableReplyNotice
		,@ForumID
		,@UserID
		,@NickName
		,@IPAddress

		,@AttachmentIds
		,@AttachmentFileNames
		,@AttachmentFileIds
		,@AttachmentFileSizes
		,@AttachmentPrices
		,@AttachmentFileExtNames
		,@HistoryAttachmentIDs

		,0

		,@IsApproved
		,@ThreadRandNumber
		,0
		,1
        ,0
        ,0
        ,@PostID output
        ,@TopMarkCount
        ,1
        ,1
    
    SET @ErrorCount = @ErrorCount + @@error;

      
    IF @ReturnValue = -1
        SET @ErrorCount = @ErrorCount + 1;

    
	IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
    END  

	IF @PostID > 0 BEGIN
		EXEC bx_DoCreateStat @ForumID,4, 1
        SET @ErrorCount = @ErrorCount + @@error;
	END


    DECLARE @Today DateTime,@Monday DateTime;
	SET @Today = CONVERT(varchar(12) , getdate(), 102);
			
	DECLARE @m int;
	SELECT @m = DATEPART(weekday, @Today);
	IF @m = 1
		SELECT @m = 8;
	SELECT @Monday = DATEADD(day, 2-@m, @Today);


    DECLARE @WeekPosts int,@DayPosts int,@LastPostDate DateTime;
    SELECT @WeekPosts = WeekPosts,@DayPosts = DayPosts,@LastPostDate = LastPostDate FROM bx_Users WHERE UserID = @UserID;

	DECLARE @TempForumID int
	IF @ThreadStatus < 4 AND @UserID<>0 BEGIN
		SET @TempForumID=@ForumID;

        IF @LastPostDate >= @Monday
            SET @WeekPosts = @WeekPosts + 1;
        ELSE
            SET @WeekPosts = 1;

        IF @LastPostDate >= @Today
            SET @DayPosts = @DayPosts + 1;
        ELSE
            SET @DayPosts = 1;


		UPDATE [bx_Users]
		   SET [TotalTopics] = [TotalTopics] + 1
			  ,[TotalPosts] = [TotalPosts] + 1
              ,[WeekPosts] = @WeekPosts
              ,[DayPosts] = @DayPosts
			  ,[LastPostDate] = getdate()

		 WHERE UserID = @UserID;
        SET @ErrorCount = @ErrorCount + @@error;

    END
	ELSE BEGIN
		SET @TempForumID=-2;
        
        DECLARE @MustUpdate bit;
        IF @LastPostDate < @Monday BEGIN
            SET @WeekPosts = 0;
            SET @MustUpdate = 1;
        END
        IF @LastPostDate < @Today BEGIN
            SET @DayPosts = 0;
            SET @MustUpdate = 1;
        END
        IF @MustUpdate = 1 BEGIN
            UPDATE [bx_Users]
		        SET [WeekPosts] = @WeekPosts
                    ,[DayPosts] = @DayPosts
            WHERE UserID = @UserID;
            SET @ErrorCount = @ErrorCount + @@error;
        END
    END

	UPDATE [bx_Forums]
		   SET [TotalThreads] = [TotalThreads] + 1
			  ,[TotalPosts] = [TotalPosts] + 1
			  ,[TodayThreads] = [TodayThreads] + 1
			  ,[TodayPosts] = [TodayPosts] + 1
			  ,[LastThreadID] = @ThreadID
		 WHERE [ForumID] = @TempForumID;
    SET @ErrorCount = @ErrorCount + @@error;


	UPDATE [bx_ThreadCatalogsInForums] SET TotalThreads=TotalThreads+1 WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID;
    SET @ErrorCount = @ErrorCount + @@error;
	
	IF @UserID=0 BEGIN
		SET @UserTotalThreads = 0;
		SET @UserTotalPosts = 0;

	END
	ELSE
		SELECT @UserTotalThreads=[TotalTopics],
			@UserTotalPosts=[TotalPosts]

			FROM [bx_Users] WITH (NOLOCK) WHERE UserID = @UserID;


    RETURN 0;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_CreatePoll')
	DROP PROCEDURE [bx_v5_CreatePoll];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_CreatePoll
    @ForumID                    int,
	@ThreadCatalogID            int,
	@ThreadStatus               tinyint,

	@IconID                     int,
	@Subject                    nvarchar(256),
	@SubjectStyle               nvarchar(300),
	@UserID                     int,
	@NickName                   nvarchar(64),
	@IsLocked                   bit,
	@IsValued                   bit,

	@Content                    ntext,
	@ContentFormat              tinyint,
	@EnableSignature            bit,
	@EnableReplyNotice          bit,
	@IPAddress                  nvarchar(64),

	@PollItems                  nvarchar(4000),
	@Multiple                   int,
	@AlwaysEyeable              bit,
	@ExpiresDate                datetime,

	@AttachmentIds              varchar(8000),
	@AttachmentFileNames        ntext,
	@AttachmentFileIds          text,
	@AttachmentFileSizes        varchar(8000),
	@AttachmentPrices           varchar(8000),
	@AttachmentFileExtNames     ntext,
	@HistoryAttachmentIDs       varchar(500),


	@ThreadRandNumber           int,
	@UserTotalThreads           int output,
	@UserTotalPosts             int output,
    @ExtendData                 ntext
   ,@TopMarkCount                   int
   ,@TempPostID                 int
   ,@AttachmentType             tinyint
   ,@Words                  nvarchar(400)
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN

	DECLARE @ReturnValue     int;
    DECLARE @ThreadID int, @PostID int

	EXECUTE @ReturnValue = [bx_v5_CreateThread]
		@ForumID,
		@ThreadCatalogID,
		@ThreadStatus,
		1,
		@IconID,
		@Subject,
		@SubjectStyle,
		0,
		@UserID,
		@NickName,
		@IsLocked,
		@IsValued,
		@Content,
		@ContentFormat,
		@EnableSignature,
		@EnableReplyNotice,
		@IPAddress,
		@AttachmentIds,
		@AttachmentFileNames,
		@AttachmentFileIds,
		@AttachmentFileSizes,
		@AttachmentPrices,
		@AttachmentFileExtNames,
		@HistoryAttachmentIDs,
		@ThreadRandNumber,
		@UserTotalThreads output,
		@UserTotalPosts output,
        @ThreadID  output,
        @PostID    output,
        @ExtendData,
        @TopMarkCount,
        @TempPostID,
        @AttachmentType,
        @Words

    SET @ErrorCount = @ErrorCount + @@error;

    IF @ReturnValue = -1
        SET @ErrorCount = @ErrorCount + 1;
    INSERT INTO [bx_Polls]
       ([ThreadID]
       ,[Multiple]
       ,[AlwaysEyeable]
       ,[ExpiresDate])
     VALUES
       (@ThreadID
       ,@Multiple
       ,@AlwaysEyeable
       ,@ExpiresDate);

    SET @ErrorCount = @ErrorCount + @@error;

	EXEC [bx_DoCreateStat] @ForumID,5, 1
    SET @ErrorCount = @ErrorCount + @@error;
	DECLARE @ItemName nvarchar(512)
	SET @ItemName = ''
	
	DECLARE @Index int
	SET @Index = 0
	
	WHILE(@PollItems <> '')
	BEGIN
		IF (CharIndex(char(13), @PollItems) = 0) BEGIN
			SET @ItemName = @PollItems
			SET @PollItems  = ''	
		END
		ELSE BEGIN
			SET @ItemName = substring(rtrim(ltrim(@PollItems)), 1, charIndex(char(13), rtrim(ltrim(@PollItems))) - 1)
			SET @PollItems = substring(rtrim(ltrim(@PollItems)), charIndex(char(13), rtrim(ltrim(@PollItems))) + 1, len(rtrim(ltrim(@PollItems)))-charIndex(char(13), rtrim(ltrim(@PollItems))))
		END

		INSERT INTO bx_PollItems(
			ThreadID,
			ItemName
		) VALUES (
			@ThreadID,
			REPLACE(@ItemName, char(10), N'')
		)
        SET @ErrorCount = @ErrorCount + @@error;
		
	END
    
    SELECT * FROM bx_PollItems WITH (NOLOCK) WHERE ThreadID = @ThreadID;

	IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
        RETURN 0;
    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_UpdateThreadExtendData')
	DROP PROCEDURE [bx_v5_UpdateThreadExtendData];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_UpdateThreadExtendData
    @ExtendData     ntext,
    @ThreadID       int
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE bx_Threads SET ExtendData = @ExtendData WHERE ThreadID = @ThreadID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetThreadExtendData')
	DROP PROCEDURE [bx_v5_GetThreadExtendData];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetThreadExtendData
    @ThreadID       int,
    @ThreadType     tinyint
AS
BEGIN
    SET NOCOUNT ON;
    IF @ThreadType = 1 BEGIN -- 投票
        SELECT * FROM bx_Polls WITH (NOLOCK) WHERE ThreadID = @ThreadID;
        SELECT * FROM bx_PollItems WITH (NOLOCK) WHERE ThreadID = @ThreadID;
        SELECT DISTINCT UserID FROM bx_PollItemDetails WITH (NOLOCK) WHERE ItemID IN(SELECT ItemID FROM bx_PollItems WITH (NOLOCK) WHERE ThreadID = @ThreadID);
    END
    ELSE IF @ThreadType = 2 BEGIN
        SELECT * FROM bx_Questions WITH (NOLOCK) WHERE ThreadID = @ThreadID; 
        SELECT * FROM bx_QuestionRewards WITH (NOLOCK) WHERE ThreadID = @ThreadID; 
    END
    ELSE IF @ThreadType = 4 BEGIN
        SELECT * FROM bx_Polemizes WITH (NOLOCK) WHERE ThreadID = @ThreadID; 
        SELECT * FROM bx_PolemizeUsers WITH (NOLOCK) WHERE ThreadID = @ThreadID;
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_CreateQuestion')
	DROP PROCEDURE [bx_v5_CreateQuestion];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_CreateQuestion
    @ForumID                    int,
	@ThreadCatalogID            int,
	@ThreadStatus               tinyint,

	@IconID                     int,
	@Subject                    nvarchar(256),
	@SubjectStyle               nvarchar(300),
	@UserID                     int,
	@NickName                   nvarchar(64),
	@IsLocked                   bit,
	@IsValued                   bit,

	@Content                    ntext,
	@ContentFormat              tinyint,
	@EnableSignature            bit,
	@EnableReplyNotice          bit,
	@IPAddress                  nvarchar(64),

	@Reward                     int,  --本主题的奖励
	@RewardCount                int,  --本主题最多可以奖励给多少帖子
	@AlwaysEyeable              bit,
	@ExpiresDate                datetime,

	@AttachmentIds              varchar(8000),
	@AttachmentFileNames        ntext,
	@AttachmentFileIds          text,
	@AttachmentFileSizes        varchar(8000),
	@AttachmentPrices           varchar(8000),
	@AttachmentFileExtNames     ntext,
	@HistoryAttachmentIDs       varchar(500),

	@ThreadRandNumber           int,
	@UserTotalThreads           int output,
	@UserTotalPosts             int output,
    @ExtendData                 ntext
   ,@TopMarkCount                   int
   ,@TempPostID                 int
   ,@AttachmentType             tinyint
   ,@Words                      nvarchar(400)
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN

	DECLARE @ReturnValue     int;
    DECLARE @ThreadID int, @PostID int

	EXECUTE @ReturnValue = [bx_v5_CreateThread]
		@ForumID,
		@ThreadCatalogID,
		@ThreadStatus,
		2,
		@IconID,
		@Subject,
		@SubjectStyle,
		0,
		@UserID,
		@NickName,
		@IsLocked,
		@IsValued,
		@Content,
		@ContentFormat,
		@EnableSignature,
		@EnableReplyNotice,
		@IPAddress,
		@AttachmentIds,
		@AttachmentFileNames,
		@AttachmentFileIds,
		@AttachmentFileSizes,
		@AttachmentPrices,
		@AttachmentFileExtNames,
		@HistoryAttachmentIDs,
		@ThreadRandNumber,
		@UserTotalThreads output,
		@UserTotalPosts output,
        @ThreadID output, 
        @PostID output,
        @ExtendData,
        @TopMarkCount,
        @TempPostID,
        @AttachmentType,
        @Words

    SET @ErrorCount = @ErrorCount + @@error;
    IF @ReturnValue = -1
        SET @ErrorCount = @ErrorCount + 1;

	INSERT INTO [bx_Questions]
           ([ThreadID]
           ,[Reward]
           ,[RewardCount]
           ,[AlwaysEyeable]
           ,[ExpiresDate])
     VALUES
           (@ThreadID
           ,@Reward
           ,@RewardCount
           ,@AlwaysEyeable
           ,@ExpiresDate)
    SET @ErrorCount = @ErrorCount + @@error;
		EXEC [bx_DoCreateStat] @ForumID,6, 1
        SET @ErrorCount = @ErrorCount + @@error;

    SELECT * FROM [bx_Questions] WITH (NOLOCK) WHERE ThreadID = @ThreadID;

	IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
        RETURN 0;
    END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_CreatePolemize')
	DROP PROCEDURE [bx_v5_CreatePolemize];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_CreatePolemize
    @ForumID                    int,
	@ThreadCatalogID            int,
	@ThreadStatus               tinyint,

	@IconID                     int,
	@Subject                    nvarchar(256),
	@SubjectStyle               nvarchar(300),
	@UserID                     int,
	@NickName                   nvarchar(64),
	@IsLocked                   bit,
	@IsValued                   bit,

	@Content                    ntext,
	@ContentFormat              tinyint,
	@EnableSignature            bit,
	@EnableReplyNotice          bit,
	@IPAddress                  nvarchar(64),

	@AgreeViewPoint             ntext,
	@AgainstViewPoint           ntext,
	@ExpiresDate                datetime,

	@AttachmentIds              varchar(8000),
	@AttachmentFileNames        ntext,
	@AttachmentFileIds          text,
	@AttachmentFileSizes        varchar(8000),
	@AttachmentPrices           varchar(8000),
	@AttachmentFileExtNames     ntext,
	@HistoryAttachmentIDs       varchar(500),

	@ThreadRandNumber           int,
	@UserTotalThreads           int output,
	@UserTotalPosts             int output,
    @ExtendData                 ntext
   ,@TopMarkCount               int
   ,@TempPostID                 int
   ,@AttachmentType             tinyint
   ,@Words                      nvarchar(400)
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN

	DECLARE @ReturnValue     int;
    DECLARE @ThreadID int, @PostID int

	EXECUTE @ReturnValue = [bx_v5_CreateThread]
		@ForumID,
		@ThreadCatalogID,
		@ThreadStatus,
		4,
		@IconID,
		@Subject,
		@SubjectStyle,
		0,
		@UserID,
		@NickName,
		@IsLocked,
		@IsValued,
		@Content,
		@ContentFormat,
		@EnableSignature,
		@EnableReplyNotice,
		@IPAddress,

		@AttachmentIds,
		@AttachmentFileNames,
		@AttachmentFileIds,
		@AttachmentFileSizes,
		@AttachmentPrices,
		@AttachmentFileExtNames,
		@HistoryAttachmentIDs,
		
		@ThreadRandNumber,
		@UserTotalThreads output,
		@UserTotalPosts output,
        @ThreadID output,
        @PostID output,
        @ExtendData,
        @TopMarkCount,
        @TempPostID,
        @AttachmentType,
        @Words
    SET @ErrorCount = @ErrorCount + @@error;

    IF @ReturnValue = -1
        SET @ErrorCount = @ErrorCount + 1;
    INSERT INTO [bx_Polemizes]
       ([ThreadID]
       ,[AgreeViewPoint]
       ,[AgainstViewPoint]
       ,[ExpiresDate])
     VALUES
       (@ThreadID
       ,@AgreeViewPoint
       ,@AgainstViewPoint
       ,@ExpiresDate);
    SET @ErrorCount = @ErrorCount + @@error;

	EXEC [bx_DoCreateStat] @ForumID,30, 1
    SET @ErrorCount = @ErrorCount + @@error;

	IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
        RETURN 0;
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_UpdateThread')
	DROP PROCEDURE [bx_v5_UpdateThread];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_UpdateThread
	@ThreadID                       int,
	@ThreadCatalogID                int,
	@IconID                         int,
	@Subject                        nvarchar(256),
	@PostID                         int,
	@Content                        ntext,
	@ContentFormat                  tinyint,
	@EnableSignature                bit,
	@EnableReplyNotice              bit,
	@IsApproved                     bit,
	@LastEditorID                   int,
	@LastEditor                     nvarchar(64),
	@Price                          int,
	@AttachmentIds                  varchar(8000),
	@AttachmentFileNames            ntext,
	@AttachmentFileIds              text,
	@AttachmentFileSizes            varchar(8000),
	@AttachmentPrices               varchar(8000),
	@AttachmentFileExtNames         ntext,
	@HistoryAttachmentIDs           varchar(500)
   ,@TopMarkCount                   int
   ,@AttachmentType                 tinyint
   ,@Words                          nvarchar(400)
AS

SET NOCOUNT ON


	DECLARE @OldThreadStatus tinyint, @NewThreadStatus tinyint,@OldWords nvarchar(400);
	SELECT @OldThreadStatus = ThreadStatus,@OldWords = Words FROM [bx_Threads] WITH (NOLOCK) WHERE [ThreadID] = @ThreadID;
    IF @IsApproved = 1 AND @OldThreadStatus = 5
        SET @NewThreadStatus = 1;
    ELSE IF @IsApproved = 0 AND @OldThreadStatus < 4
        SET @NewThreadStatus = 5;
    ELSE
        SET @NewThreadStatus = @OldThreadStatus;
        

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN

	UPDATE [bx_Threads] SET
		[ThreadCatalogID] = @ThreadCatalogID,
		[IconID] = @IconID,
		[Subject] = @Subject,
		[Price] = @Price,
		[UpdateDate] = CASE WHEN TotalReplies = 0 THEN getdate() ELSE [UpdateDate] END,
		[ThreadStatus] = @NewThreadStatus,
		[KeywordVersion] = '',
        [AttachmentType] = @AttachmentType,
        [Words] = @Words
	WHERE
		[ThreadID] = @ThreadID;
    SET @ErrorCount = @ErrorCount + @@error;

    IF @Words <> @OldWords BEGIN
        DELETE bx_ThreadWords WHERE ThreadID = @ThreadID;
        SET @ErrorCount = @ErrorCount + @@error;
        IF @Words <> '' BEGIN
            INSERT INTO bx_ThreadWords(ThreadID,Word) SELECT @ThreadID,T.item FROM bx_GetStringTable_nvarchar(@Words, N',') T;
            SET @ErrorCount = @ErrorCount + @@error;
        END
    END

    SELECT  *  FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID = @ThreadID;

Execute bx_v5_UpdatePost
		@PostID,
		@IconID,
		@Subject,
		@Content,
		@ContentFormat,
		@EnableSignature,
		@EnableReplyNotice,
		@IsApproved,
		@LastEditorID,
		@LastEditor,
		@AttachmentIds,
		@AttachmentFileNames,
		@AttachmentFileIds,
		@AttachmentFileSizes,
		@AttachmentPrices,
		@AttachmentFileExtNames,
		@HistoryAttachmentIDs,
        0,
        @TopMarkCount

    SET @ErrorCount = @ErrorCount + @@error;

    IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
        RETURN 0;
    END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_CreatePost')
	DROP PROCEDURE [bx_v5_CreatePost];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_CreatePost
	@ParentID                       int,
	@ThreadID                       int,
	@PostType                       tinyint,
	@IconID                         int,
	@Subject                        nvarchar(256),
	@Content                        ntext,
	@ContentFormat                  tinyint,
	@EnableSignature                bit,
	@EnableReplyNotice              bit,
	@ForumID                        int,
	@UserID                         int,
	@NickName                       nvarchar(64),
	@IPAddress                      nvarchar(32),

	@AttachmentIds                  varchar(8000),
	@AttachmentFileNames            ntext,
	@AttachmentFileIds              text,
	@AttachmentFileSizes            varchar(8000),
	@AttachmentPrices               varchar(8000),
	@AttachmentFileExtNames         ntext,
	@HistoryAttachmentIDs           varchar(500),

	@IsCreatePost                   bit = 1,

	@IsApproved                     bit,
	@PostRandNumber                 tinyint,
	@UserTotalPosts                 int output,
	@UpdateSortOrder                bit,
    @GetExtendedInfo                bit,
    @GetThreadEnableReplyNotice     bit,
    @PostID                         int output
   ,@TopMarkCount                   int
   ,@GetPost                        bit
   ,@GetThread                      bit
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @Type tinyint;
	IF @IsApproved = 1 --BEGIN
		SET @Type=1
	ELSE
		SET @Type=5

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN CreatePost


    DECLARE @RealForumID int,@PostedCount int;
    
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
    BEGIN TRAN UpdatePostedCount

    SELECT @RealForumID = ForumID,@PostedCount = PostedCount FROM bx_Threads with(xlock,rowlock) WHERE ThreadID = @ThreadID;
    IF @PostedCount IS NOT NULL
        SET @PostedCount = @PostedCount + 1;
    

	DECLARE @TempSortOrder BIGINT,@PostDate datetime;

	SET @PostDate = getdate();
	EXEC [bx_GetSortOrder] @Type, @PostRandNumber, @PostDate, 0, @TempSortOrder OUTPUT 
    

    IF @IsCreatePost = 0
        SET @PostedCount = 1;

	INSERT INTO [bx_Posts]
           ([ParentID]
           ,[ForumID]
           ,[ThreadID]
           ,[PostType]
           ,[IconID]
           ,[Subject]
           ,[Content]
           ,[ContentFormat]
           ,[EnableSignature]
           ,[EnableReplyNotice]
           ,[UserID]
           ,[NickName]
           ,[IPAddress]
           ,[HistoryAttachmentIDs]
           ,[SortOrder]
           ,[FloorNumber])
     VALUES
           (@ParentID
           ,@RealForumID
           ,@ThreadID
           ,@PostType
           ,@IconID
           ,@Subject
           ,@Content
           ,@ContentFormat
           ,@EnableSignature
           ,@EnableReplyNotice
           ,@UserID
           ,@NickName
           ,@IPAddress
		   ,@HistoryAttachmentIDs
           ,@TempSortOrder
           ,@PostedCount)

    SET @ErrorCount = @ErrorCount + @@error;

	SELECT @PostID = @@IDENTITY;

    IF @PostType = 1 --主题内容
        UPDATE bx_Threads SET LastPostID = @PostID,ContentID = @PostID, PostedCount = @PostedCount WHERE ThreadID = @ThreadID;
    ELSE IF @IsApproved = 1
        UPDATE bx_Threads SET LastPostID = @PostID, PostedCount = @PostedCount WHERE ThreadID = @ThreadID;
    ELSE 
        UPDATE bx_Threads SET PostedCount = @PostedCount WHERE ThreadID = @ThreadID;
    
    SET @ErrorCount = @ErrorCount + @@error;

    COMMIT TRAN UpdatePostedCount
    
    IF @GetThread = 1
        SELECT  *  FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID = @ThreadID; 

    IF @GetThreadEnableReplyNotice = 1
        SELECT EnableReplyNotice FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID AND PostType = 1;--主题内容

	IF DATALENGTH(@AttachmentIds) > 0 BEGIN

		DECLARE @AttachmentTable table(TempID int identity(1,1), AttachmentID int, FileName nvarchar(256), FileExtName nvarchar(10), FileID varchar(50), FileSize bigint, Price int);

		
		INSERT INTO @AttachmentTable (AttachmentID) SELECT item FROM bx_GetIntTable(@AttachmentIds, '|');
        SET @ErrorCount = @ErrorCount + @@error;

		UPDATE @AttachmentTable SET
			[FileName] = T.item
			FROM bx_GetStringTable_ntext(@AttachmentFileNames, N'|') T
			WHERE TempID = T.id;
        SET @ErrorCount = @ErrorCount + @@error;

		UPDATE @AttachmentTable SET
			FileID = T.item
			FROM bx_GetStringTable_text(@AttachmentFileIds, '|') T
			WHERE TempID = T.id;
        SET @ErrorCount = @ErrorCount + @@error;

		UPDATE @AttachmentTable SET
			FileSize = T.item
			FROM bx_GetBigIntTable(@AttachmentFileSizes, '|') T
			WHERE TempID = T.id;
        SET @ErrorCount = @ErrorCount + @@error;

		UPDATE @AttachmentTable SET
			Price = T.item
			FROM bx_GetIntTable(@AttachmentPrices, '|') T
			WHERE TempID = T.id;
        SET @ErrorCount = @ErrorCount + @@error;
			
		UPDATE @AttachmentTable SET
			FileExtName = T.item
			FROM bx_GetStringTable_ntext(@AttachmentFileExtNames, N'|') T
			WHERE TempID = T.id;
        SET @ErrorCount = @ErrorCount + @@error;


		INSERT INTO bx_Attachments(
			PostID,
			FileID,
			FileName,
			FileSize,
			FileType,
			Price,
			UserID
			) SELECT 
			@PostID,
			T.FileID,
			T.FileName,
			T.FileSize,
			T.FileExtName,
			T.Price,
			@UserID
			FROM @AttachmentTable T
			WHERE T.AttachmentID < 0 AND T.FileID IS NOT NULL;
        SET @ErrorCount = @ErrorCount + @@error; 
		
		SELECT [AttachmentID] FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID=@PostID ORDER BY [AttachmentID] DESC;
		
		INSERT INTO bx_Attachments(
			PostID,
			FileID,
			FileName,
			FileSize,
			FileType,
			Price,
			UserID
			) SELECT 
			@PostID,
			T.FileID,
			T.FileName,
			T.FileSize,
			T.FileExtName,
			T.Price,
			@UserID
			FROM @AttachmentTable T
			WHERE T.AttachmentID = 0 AND T.FileID IS NOT NULL;
        SET @ErrorCount = @ErrorCount + @@error; 
		
		SELECT [AttachmentID],[FileID] FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID=@PostID;
			
	END




    DECLARE @Today DateTime,@Monday DateTime;
    DECLARE @WeekPosts int,@DayPosts int,@MonthPosts int,@LastPostDate DateTime;

    IF @UserID<>0 BEGIN
	    SET @Today = CONVERT(varchar(12) , getdate(), 102);
			
	    DECLARE @m int;
	    SELECT @m = DATEPART(weekday, @Today);
	    IF @m = 1
		    SELECT @m = 8;
	    SELECT @Monday = DATEADD(day, 2-@m, @Today);


        SELECT @WeekPosts = WeekPosts,@DayPosts = DayPosts,@MonthPosts = MonthPosts,@LastPostDate = LastPostDate FROM bx_Users WHERE UserID = @UserID;
    END

    IF @IsApproved=1 AND @IsCreatePost = 1 BEGIN
    
	
		EXEC [bx_DoCreateStat] @ForumID,4, 1
        SET @ErrorCount = @ErrorCount + @@error;
		
	    
		DECLARE @SortOrder bigint
		
		IF @UpdateSortOrder = 1 BEGIN
			
            EXEC [bx_GetSortOrder] 1, @PostID, @PostDate, 1, @SortOrder OUTPUT 
            /*
			IF @SortOrder < 2000000000000000--500000000000000
				EXEC [bx_GetSortOrder] 1, @PostID, @PostDate, @SortOrder OUTPUT 
			ELSE IF @SortOrder < 3000000000000000--800000000000000
				EXEC [bx_GetSortOrder] 2, @PostID, @PostDate, @SortOrder OUTPUT 
			ELSE
				EXEC [bx_GetSortOrder] 3, @PostID, @PostDate, @SortOrder OUTPUT 
            */		

		END

        UPDATE [bx_Forums]
            SET [TotalPosts] = [TotalPosts] + 1,
				[TodayPosts] = [TodayPosts] + 1,
				[LastThreadID] = @ThreadID
            WHERE [ForumID] = @ForumID;
        SET @ErrorCount = @ErrorCount + @@error;
		

        BEGIN TRAN UpdateThread
	    IF @UpdateSortOrder = 1
			UPDATE [bx_Threads] with(updlock,rowlock) 
			   SET [TotalReplies] = [TotalReplies] + 1,
					[LastPostUserID] = @UserID,
					[LastPostNickName] = @NickName,
					[UpdateDate] = getdate(),
					[SortOrder] = @SortOrder
			WHERE ThreadID=@ThreadID;
		ELSE
			UPDATE [bx_Threads] with(updlock,rowlock) 
			   SET [TotalReplies] = [TotalReplies] + 1,
					[LastPostUserID] = @UserID,
					[LastPostNickName] = @NickName,
					[UpdateDate] = getdate()
			WHERE ThreadID=@ThreadID;
        SET @ErrorCount = @ErrorCount + @@error;

        COMMIT TRAN UpdateThread
		IF @UserID<>0 BEGIN

            IF @LastPostDate >= @Monday
                SET @WeekPosts = @WeekPosts + 1;
            ELSE
                SET @WeekPosts = 1;

            IF @LastPostDate >= @Today
                SET @DayPosts = @DayPosts + 1;
            ELSE
                SET @DayPosts = 1;

            IF DATEPART(year, GETDATE()) = DATEPART(year, GETDATE()) AND DATEPART(month, GETDATE()) = DATEPART(month,@LastPostDate)
                SET @MonthPosts = @MonthPosts + 1;
            ELSE
                SET @MonthPosts = 1;

			UPDATE [bx_Users]
			   SET	[LastPostDate] = getdate(),
					[TotalPosts] = [TotalPosts] + 1
                    ,[WeekPosts] = @WeekPosts
                    ,[MonthPosts] = @MonthPosts
                    ,[DayPosts] = @DayPosts
			 WHERE UserID = @UserID;
            SET @ErrorCount = @ErrorCount + @@error;
			 
		END

    END
    ELSE IF @IsCreatePost = 1 AND @UserID<>0 BEGIN
        DECLARE @MustUpdate bit;
        IF @LastPostDate < @Monday BEGIN
            SET @WeekPosts = 0;
            SET @MustUpdate = 1;
        END
        IF @LastPostDate < @Today BEGIN
            SET @DayPosts = 0;
            SET @MustUpdate = 1;
        END
        IF DATEPART(year, GETDATE()) <> DATEPART(year,@LastPostDate) OR DATEPART(month, GETDATE()) <> DATEPART(month,@LastPostDate) BEGIN
            SET @MonthPosts = 1;
        END
        IF @MustUpdate = 1 BEGIN
            UPDATE [bx_Users]
		        SET [WeekPosts] = @WeekPosts
                    ,[DayPosts] = @DayPosts
                    ,[MonthPosts] = @MonthPosts
            WHERE UserID = @UserID;
            SET @ErrorCount = @ErrorCount + @@error;
        END
    END

	IF @UserID<>0 BEGIN
		IF @PostType = 2
			EXEC [bx_v5_AddPolemizeUser] @ThreadID,@UserID,0
		ELSE IF @PostType = 3
			EXEC [bx_v5_AddPolemizeUser] @ThreadID,@UserID,1
		ELSE IF @PostType = 4
			EXEC [bx_v5_AddPolemizeUser] @ThreadID,@UserID,2
        SET @ErrorCount = @ErrorCount + @@error;

	END	

    IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN CreatePost
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN CreatePost
    END

    IF @UserID<>0 BEGIN
		
		SELECT 
			@UserTotalPosts=ISNULL(TotalPosts,0)
			FROM [bx_Users] WHERE UserID = @UserID;
			
	END 
	ELSE BEGIN
			SET  @UserTotalPosts = 0;
	END
    
    IF @GetPost = 1 BEGIN
        IF @GetExtendedInfo = 0
            SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @PostID;
        ELSE BEGIN
            DECLARE @SQLString nvarchar(4000);
            DECLARE @PostFieldsString nvarchar(4000);
            SET @SQLString = 'SELECT 
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
 FROM bx_Posts WITH (NOLOCK) WHERE PostID = '+ CAST(@PostID AS varchar(16));  --'SELECT '+ CAST(@PostID AS varchar(16)) + ',''' + @HistoryAttachmentIDs + '''';
            

        SET @PostFieldsString = '

DECLARE @S2 VARCHAR(8000),@D2 DATETIME;

            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
SELECT @D2 = GETDATE();
		    INSERT INTO @PostIDTable (

[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]

) ' + @SQLString + ';

    SET @S2 =''|A1=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
'+
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);

    SET @S2 =@S2 + ''|A8=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            
            SELECT @S2;
'
; EXEC (@PostFieldsString); 
        END
    END

    
    RETURN 0;
    
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_UpdatePost')
	DROP PROCEDURE [bx_v5_UpdatePost];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_UpdatePost
	@PostID                         int,
	@IconID                         int,
	@Subject                        nvarchar(256),
	@Content                        ntext,
	@ContentFormat                  tinyint,
	@EnableSignature                bit,
	@EnableReplyNotice              bit,
	@IsApproved                     bit,
	@LastEditorID                   int,
	@LastEditor                     nvarchar(65),
	
	@AttachmentIds                  varchar(8000),
	@AttachmentFileNames            ntext,
	@AttachmentFileIds              text,
	@AttachmentFileSizes            varchar(8000),
	@AttachmentPrices               varchar(8000),
	@AttachmentFileExtNames         ntext,
	@HistoryAttachmentIDs           varchar(500),
    @GetExtendedInfo                bit
    ,@TopMarkCount                  int
AS
BEGIN
    SET NOCOUNT ON


    DECLARE @UserID int,@OldSortOrder bigint;
    SELECT @UserID=UserID,@OldSortOrder=SortOrder FROM  bx_Posts WITH (NOLOCK) WHERE PostID=@PostID;---Modify by 帅帅


    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN

    DECLARE @SortOrder bigint;

    IF @IsApproved=1
	    EXEC [bx_UpdateSortOrder] 1, @OldSortOrder, @SortOrder OUTPUT;
    ELSE
	    EXEC [bx_UpdateSortOrder] 5, @OldSortOrder, @SortOrder OUTPUT;

	UPDATE [bx_Posts] SET
		[IconID] = @IconID,
		[Subject] = @Subject,
		[Content] = @Content,
		[ContentFormat] = @ContentFormat,
		[EnableSignature] = @EnableSignature,
		[EnableReplyNotice] = @EnableReplyNotice,
		[LastEditorID]=@LastEditorID,
		[LastEditor]=@LastEditor,
		[UpdateDate] = getdate(),
		[HistoryAttachmentIDs] = @HistoryAttachmentIDs,
		[SortOrder] = @SortOrder,
		[KeywordVersion] = ''
	WHERE
		[PostID] = @PostID
		
    SET @ErrorCount = @ErrorCount + @@error;
	
		IF DATALENGTH(@AttachmentIds) > 0 BEGIN
			DECLARE @AttachmentTable table(TempID int identity(1,1), AttachmentID int, FileName nvarchar(256), FileExtName varchar(10), FileID varchar(50), FileSize bigint, Price int);
		
			INSERT INTO @AttachmentTable (AttachmentID) SELECT item FROM bx_GetIntTable(@AttachmentIds, '|');

			UPDATE @AttachmentTable SET
				[FileName] = T.item
				FROM bx_GetStringTable_ntext(@AttachmentFileNames, N'|') T
				WHERE TempID = T.id;

			UPDATE @AttachmentTable SET
				FileID = T.item
				FROM bx_GetStringTable_text(@AttachmentFileIds, '|') T
				WHERE TempID = T.id;

			UPDATE @AttachmentTable SET
				FileSize = T.item
				FROM bx_GetBigIntTable(@AttachmentFileSizes, '|') T
				WHERE TempID = T.id;

			UPDATE @AttachmentTable SET
				Price = T.item
				FROM bx_GetIntTable(@AttachmentPrices, '|') T
				WHERE TempID = T.id;
				
			UPDATE @AttachmentTable SET
				FileExtName = T.item
				FROM bx_GetStringTable_ntext(@AttachmentFileExtNames, N'|') T
				WHERE TempID = T.id;


			DECLARE @NewAttchmentCount int;

			DECLARE @AttachmentIDsTable table(AttachmentID int);
			INSERT INTO @AttachmentIDsTable SELECT [AttachmentID] FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID=@PostID;

			INSERT INTO bx_Attachments(
				PostID,
				FileID,
				FileName,
				FileSize,
				FileType,
				Price,
				UserID
				) SELECT 
				@PostID,
				T.FileID,
				T.FileName,
				T.FileSize,
				T.FileExtName,
				T.Price,
				@UserID
				FROM @AttachmentTable T
				WHERE T.AttachmentID < 0;
            SET @ErrorCount = @ErrorCount + @@error;
					
			SELECT @NewAttchmentCount = @@ROWCOUNT;
			
			EXEC('SELECT TOP ' + @NewAttchmentCount + ' [AttachmentID] FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID = '+@PostID + ' ORDER BY [AttachmentID] DESC');
				
			DELETE bx_Attachments WHERE PostID=@PostID AND 
				AttachmentID IN(SELECT [AttachmentID] FROM @AttachmentIDsTable) 
				AND AttachmentID NOT IN(SELECT AttachmentID FROM @AttachmentTable);
            SET @ErrorCount = @ErrorCount + @@error;

			UPDATE bx_Attachments SET
				FileName = T.FileName,
				Price = T.Price
				FROM @AttachmentTable T
				WHERE T.AttachmentID > 0 AND T.AttachmentID = bx_Attachments.AttachmentID AND T.FileName<>'' AND T.FileName is not null;
            SET @ErrorCount = @ErrorCount + @@error;

			UPDATE bx_Attachments SET
				Price = T.Price
				FROM @AttachmentTable T
				WHERE T.AttachmentID > 0 AND T.AttachmentID = bx_Attachments.AttachmentID AND (T.FileName = '' OR T.FileName is not null);
		    SET @ErrorCount = @ErrorCount + @@error;

			INSERT INTO bx_Attachments(
				PostID,
				FileID,
				FileName,
				FileSize,
				FileType,
				Price,
				UserID
				) SELECT 
				@PostID,
				T.FileID,
				T.FileName,
				T.FileSize,
				T.FileExtName,
				T.Price,
				@UserID
				FROM @AttachmentTable T
				WHERE T.AttachmentID = 0;
			SET @ErrorCount = @ErrorCount + @@error;

			SELECT [AttachmentID],[FileID] FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID=@PostID;

		END 
		ELSE BEGIN
			DELETE bx_Attachments WHERE PostID = @PostID;
            SET @ErrorCount = @ErrorCount + @@error;
		END

    IF @GetExtendedInfo = 0
        SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @PostID;
    ELSE BEGIN
        DECLARE @SQLString nvarchar(4000);
        DECLARE @PostFieldsString nvarchar(4000);
        SET @SQLString = 'SELECT 
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
 FROM bx_Posts WITH (NOLOCK) WHERE PostID = '+ CAST(@PostID AS varchar(16));  --'SELECT '+ CAST(@PostID AS varchar(16)) + ',''' + @HistoryAttachmentIDs + '''';
        

        SET @PostFieldsString = '

DECLARE @S2 VARCHAR(8000),@D2 DATETIME;

            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
SELECT @D2 = GETDATE();
		    INSERT INTO @PostIDTable (

[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]

) ' + @SQLString + ';

    SET @S2 =''|A1=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
'+
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);

    SET @S2 =@S2 + ''|A8=''+  CONVERT(varchar(100), getdate()-@D2, 14) ;
            
            SELECT @S2;
'
; EXEC (@PostFieldsString); 
    END

	IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
        RETURN 0;
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdatePostContent')
	DROP PROCEDURE [bx_UpdatePostContent];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdatePostContent
     @PostID   int,
     @Content  ntext
AS
BEGIN
	SET NOCOUNT ON;
    UPDATE [bx_Posts] SET [Content] = @Content WHERE [PostID] = @PostID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_RepairTotalReplyCount')
	DROP PROCEDURE [bx_RepairTotalReplyCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_RepairTotalReplyCount
     @ThreadID   int
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @Total int;
    SELECT @Total = COUNT(*)-1 FROM bx_Posts WITH(NOLOCK) WHERE ThreadID = @ThreadID AND SortOrder<4000000000000000;
    UPDATE [bx_Threads] SET [TotalReplies] = @Total WHERE [ThreadID] = @ThreadID; 
    SELECT @Total;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetPostUserIDsFormThreads')
	DROP PROCEDURE [bx_v5_GetPostUserIDsFormThreads];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetPostUserIDsFormThreads
	@ThreadIDs varchar(8000)
AS
BEGIN
	SET NOCOUNT ON;

	EXEC('SELECT DISTINCT UserID FROM bx_Posts WITH (NOLOCK) WHERE SortOrder<4000000000000000 AND ThreadID in('+@ThreadIDs+')');
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetUserTodayAttachments')
	DROP PROCEDURE [bx_v5_GetUserTodayAttachments];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetUserTodayAttachments
    @UserID      int,
    @DateTime    datetime
AS BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM [bx_Attachments] WITH (NOLOCK) WHERE [UserID] = @UserID AND [CreateDate] >= @DateTime;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetUserTodayAttachmentInfo')
	DROP PROCEDURE [bx_v5_GetUserTodayAttachmentInfo];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetUserTodayAttachmentInfo
    @UserID         int,
    @DateTime       datetime,
    @ExcludePostID  int
AS BEGIN
    SET NOCOUNT ON;
    
    IF @ExcludePostID IS NULL
       SELECT Count(*) AS TotalCount,ISNULL(SUM(FileSize),0) AS TotalFileSize FROM [bx_Attachments] WITH (NOLOCK) WHERE [UserID] = @UserID AND [CreateDate] >= @DateTime;
    ELSE
       SELECT Count(*) AS TotalCount,ISNULL(SUM(FileSize),0) AS TotalFileSize FROM [bx_Attachments] WITH (NOLOCK) WHERE [UserID] = @UserID AND PostID!=@ExcludePostID AND [CreateDate] >= @DateTime; 
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetAttachmentsByPostID')
	DROP PROCEDURE [bx_v5_GetAttachmentsByPostID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetAttachmentsByPostID
    @PostID int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID = @PostID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetAttachment')
	DROP PROCEDURE [bx_v5_GetAttachment];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetAttachment
	@AttachmentID               int,
	@UpdateTotalDownloads       bit
AS
BEGIN
	SET NOCOUNT ON;
	IF(@UpdateTotalDownloads = 1)
		UPDATE bx_Attachments SET TotalDownloads=TotalDownloads+1 Where AttachmentID=@AttachmentID
	
	SELECT * FROM [bx_Attachments] WITH (NOLOCK) WHERE AttachmentID =@AttachmentID;

    IF @UpdateTotalDownloads = 1
        SELECT ThreadID FROM [bx_Posts] P INNER JOIN [bx_Attachments] A ON P.PostID = A.PostID WHERE A.AttachmentID =  @AttachmentID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAttachmentByDiskFileID')
	DROP PROCEDURE [bx_GetAttachmentByDiskFileID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAttachmentByDiskFileID
     @DiskFileID  int
    ,@PostID      int
AS BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [bx_Attachments] WITH (NOLOCK) WHERE [PostID] = @PostID AND [FileID] = (SELECT [FileID] FROM [bx_DiskFiles] WITH (NOLOCK) WHERE [DiskFileID] = @DiskFileID);
    SELECT * FROM [bx_Posts] WITH (NOLOCK) WHERE [PostID] = @PostID; 
    SELECT * FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID = (SELECT ThreadID FROM [bx_Posts] WITH (NOLOCK) WHERE [PostID] = @PostID);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_CreateAttachmentExchange')
	DROP PROCEDURE [bx_v5_CreateAttachmentExchange];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_CreateAttachmentExchange
	@AttachmentID       int, 
	@UserID             int,
	@Price              int
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM bx_AttachmentExchanges WITH (NOLOCK) WHERE AttachmentID=@AttachmentID AND UserID=@UserID)
		RETURN 0;
	Insert into bx_AttachmentExchanges(AttachmentID,UserID,Price,CreateDate) values(@AttachmentID,@UserID,@Price,getdate())
	IF(@@ROWCOUNT>0)
		RETURN 0
	ELSE
		RETURN 1

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_GetAttachmentIsBuy')
	DROP PROCEDURE [bx_v5_GetAttachmentIsBuy];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_GetAttachmentIsBuy
	@UserID             INT,
	@AttachmentID       INT
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM bx_AttachmentExchanges WITH (NOLOCK) WHERE AttachmentID=@AttachmentID AND UserID=@UserID)
		RETURN 0
	ELSE
	BEGIN
		IF EXISTS(SELECT * FROM bx_Attachments WITH (NOLOCK) WHERE AttachmentID=@AttachmentID AND Price=0)
			RETURN 0
		ELSE
			RETURN -1
	END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetThreadsUp')
	DROP PROCEDURE [bx_SetThreadsUp];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetThreadsUp
	@ThreadIdentities varchar(8000),
	@ForumID int
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ThreadID int;
	DECLARE @i int
	DECLARE @j int
	
	DECLARE @SortOrder BIGINT,@PostDate datetime
	
	SET @PostDate = getdate();
	
	
	SET @ThreadIdentities = @ThreadIdentities + N','
	SELECT @i = CHARINDEX(',', @ThreadIdentities)
	
	SET @j = 0
	
	WHILE ( @i > 1 ) BEGIN
			SELECT @ThreadID = SUBSTRING(@ThreadIdentities,0, @i)	

			EXEC [bx_GetSortOrder] 1, @j, @PostDate, 1, @SortOrder OUTPUT;

			UPDATE bx_Threads SET SortOrder = @SortOrder WHERE ForumID=@ForumID AND ThreadID=@ThreadID
			
			SELECT @ThreadIdentities = SUBSTRING(@ThreadIdentities, @i + 1, LEN(@ThreadIdentities) - @i)
			SELECT @i = CHARINDEX(',',@ThreadIdentities)
			SELECT @j = @j + 1
	END
	RETURN
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetThreadsValued')
	DROP PROCEDURE [bx_SetThreadsValued];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetThreadsValued 
	@ForumID INT,
	@ThreadIdentities varchar(8000),
	@IsValued bit
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @RowCount int

	EXEC ('UPDATE bx_Threads SET IsValued=' + @IsValued + ' WHERE ThreadID IN (' + @ThreadIdentities + ') AND ForumID = ' + @ForumID + ' AND IsValued<>' + @IsValued) 
	
	SET @RowCount=@@ROWCOUNT
	IF @RowCount> 0
		
		IF @IsValued=0
			SELECT @RowCount= 0 - @RowCount;

		EXEC [bx_DoCreateStat] @ForumID,2, @RowCount
	RETURN
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_ApprovePosts')
	DROP PROCEDURE [bx_v5_ApprovePosts];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_ApprovePosts 
	@PostIdentities varchar(8000)
AS
	SET NOCOUNT ON
	
	EXEC('SELECT DISTINCT(ThreadID) FROM [bx_Posts] WITH (NOLOCK) WHERE PostID IN('+@PostIdentities+') AND SortOrder >= 5000000000000000')
	
	
	DECLARE @TempTable table(tempId int IDENTITY(1, 1),TempPostID int,TempSortOrder bigint);
	
	INSERT INTO @TempTable(TempPostID) SELECT item FROM bx_GetIntTable(@PostIdentities, ',');
	
	UPDATE @TempTable SET TempSortOrder = SortOrder FROM [bx_Posts] WHERE TempPostID = PostID;
	
	DECLARE @i int,@Total int;
	SET @i = 0;
	SELECT @Total = COUNT(*) FROM @TempTable;
	
	WHILE(@i<@Total) BEGIN
		SET @i = @i + 1;
		DECLARE @SortOrder bigint,@OldSortOrder bigint;
	
		SELECT @OldSortOrder = TempSortOrder FROM @TempTable WHERE tempId = @i;
		EXEC [bx_UpdateSortOrder] 1, @OldSortOrder, @SortOrder OUTPUT;
		
		UPDATE @TempTable SET TempSortOrder = @SortOrder WHERE tempId = @i; 
	END
	
	UPDATE [bx_Posts] SET SortOrder=TempSortOrder FROM @TempTable WHERE PostID = TempPostID AND SortOrder >= 5000000000000000;
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetPostLoveHate')
	DROP PROCEDURE [bx_SetPostLoveHate];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetPostLoveHate 
	@UserID         int,
	@PostID         int,
	@IsLove         bit,
    @CanSetMore     bit
AS
BEGIN
	SET NOCOUNT ON
	
    IF NOT EXISTS(SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID=@PostID)
        RETURN 1; --不存在这个帖子

    DECLARE @LoveCount int,@HateCount int;
    IF @IsLove = 1 BEGIN
        SET @LoveCount = 1;
        SET @HateCount = 0;
    END
    ELSE BEGIN
        SET @LoveCount = 0;
        SET @HateCount = 1;
    END

    IF EXISTS(SELECT * FROM bx_PostLoveHates WITH (NOLOCK) WHERE PostID=@PostID AND UserID=@UserID) BEGIN
        IF @CanSetMore = 0
            RETURN 2; --已经支持反对过了
        ELSE
            UPDATE bx_PostLoveHates SET LoveCount = LoveCount + @LoveCount, HateCount = HateCount + @HateCount WHERE PostID=@PostID AND UserID=@UserID; 
    END 
    ELSE
        INSERT INTO bx_PostLoveHates(PostID,UserID,LoveCount,HateCount) VALUES(@PostID,@UserID,@LoveCount,@HateCount);

    UPDATE bx_Posts SET LoveCount = @LoveCount, HateCount = @HateCount WHERE PostID = @PostID;
    
    SELECT ThreadID FROM bx_Posts WITH (NOLOCK) WHERE PostID = @PostID;

    RETURN 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SetThreadRank')
	DROP PROCEDURE [bx_SetThreadRank];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SetThreadRank 
	@ThreadID       int,
	@UserID         int,
	@Rank           tinyint
AS
BEGIN
	SET NOCOUNT ON
	IF EXISTS (SELECT * FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND PostUserID=@UserID)
		RETURN (1)--不能给自己评级
	IF EXISTS (SELECT * FROM [bx_ThreadRanks] WHERE ThreadID=@ThreadID AND UserID=@UserID)
		 UPDATE [bx_ThreadRanks] SET Rank=@Rank,UpdateDate=getdate() WHERE ThreadID=@ThreadID AND UserID=@UserID 
	ELSE
		INSERT INTO [bx_ThreadRanks] (
		                                [ThreadID],
		                                [UserID],
		                                [Rank]
		                                ) VALUES (
		                                @ThreadID,
		                                @UserID,
		                                @Rank
		                                );
	DECLARE @Count int,@TotalRank int
	SELECT @Count=COUNT(*) FROM [bx_ThreadRanks] WITH (NOLOCK) WHERE ThreadID=@ThreadID;
	SELECT @TotalRank=SUM(Rank) FROM [bx_ThreadRanks] WITH (NOLOCK) WHERE ThreadID=@ThreadID;
	UPDATE [bx_Threads] SET Rank=@TotalRank/@Count WHERE ThreadID=@ThreadID;
	
    SELECT @TotalRank/@Count AS NewRank;

	RETURN (0)
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateThreadExchange')
	DROP PROCEDURE [bx_CreateThreadExchange];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateThreadExchange 
	@ThreadID       int,
	@UserID         int,
	@Price          int
AS
BEGIN
	SET NOCOUNT ON
    IF NOT EXISTS(SELECT * FROM bx_ThreadExchanges WITH (NOLOCK) WHERE ThreadID = @ThreadID AND UserID = @UserID)
        INSERT INTO bx_ThreadExchanges (ThreadID,UserID,Price) VALUES (@ThreadID,@UserID,@Price);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_IsBuyThread')
	DROP PROCEDURE [bx_IsBuyThread];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_IsBuyThread 
	@ThreadID       int,
	@UserID         int
AS
BEGIN
	SET NOCOUNT ON
    IF EXISTS(SELECT * FROM bx_ThreadExchanges WITH (NOLOCK) WHERE ThreadID = @ThreadID AND UserID = @UserID)
        SELECT 1;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_Vote')
	DROP PROCEDURE [bx_v5_Vote];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_Vote 
	@ItemIDs            varchar(8000),
	@ThreadID           int,
	@UserID             int,
	@NickName           nvarchar(64)
AS
	SET NOCOUNT ON
	IF EXISTS (SELECT * FROM bx_Polls WITH(NOLOCK) WHERE ThreadID=@ThreadID AND ExpiresDate<getdate()) 
		RETURN (1)--已经过期
	IF EXISTS (SELECT * FROM [bx_PollItemDetails] WITH(NOLOCK) WHERE ItemID IN(SELECT ItemID FROM [bx_PollItems] WITH (NOLOCK) WHERE ThreadID=@ThreadID) AND UserID=@UserID)
		RETURN (2)--	当前用户已经投过票
	IF EXISTS (SELECT * FROM bx_Threads WITH(NOLOCK) WHERE ThreadID=@ThreadID AND IsLocked=1) 
		RETURN (3)--已经锁定
	DECLARE @ItemID int,@i int
	SET @ItemIDs=@ItemIDs+N','
	SELECT @i=CHARINDEX(',',@ItemIDs)
			
	WHILE(@i>1)
		BEGIN
			SELECT @ItemID=SUBSTRING(@ItemIDs,0, @i)
			
			UPDATE bx_PollItems SET PollItemCount=PollItemCount+1 WHERE ItemID=@ItemID
			
			IF(@@ROWCOUNT>0)
				INSERT INTO bx_PollItemDetails(ItemID,UserID,NickName) VALUES(@ItemID,@UserID,@NickName)
			
			SELECT @ItemIDs=SUBSTRING(@ItemIDs,@i+1,LEN(@ItemIDs)-@i)
			SELECT @i=CHARINDEX(',',@ItemIDs)
		END

    EXECUTE bx_v5_GetThreadExtendData @ThreadID,1; 

	RETURN (0)
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetPollItemDetails')
	DROP PROCEDURE [bx_GetPollItemDetails];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetPollItemDetails 
	@ThreadID           int
AS
	SET NOCOUNT ON

    SELECT * FROM [bx_PollItemDetails] WITH(NOLOCK) WHERE ItemID IN(SELECT ItemID FROM [bx_PollItems] WHERE ThreadID=@ThreadID);
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_AddPolemizeUser')
	DROP PROCEDURE [bx_v5_AddPolemizeUser];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_AddPolemizeUser 
    @ThreadID int, 
	@UserID int,
	@ViewPointType tinyint
AS
	SET NOCOUNT ON;
	
	IF(NOT EXISTS (SELECT * FROM [bx_Polemizes] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND ExpiresDate>GETDATE()))
		RETURN (1) --过期了
	
	DECLARE @ViewPoint TINYINT
	SELECT @ViewPoint=ViewPointType FROM [bx_PolemizeUsers] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND UserID=@UserID
	IF @ViewPoint = 0 --已支持过正方观点
		RETURN (2)
	ELSE IF @ViewPoint = 1 --已支持过反方观点
		RETURN (3)
	ELSE IF @ViewPoint = 2 --已支持过中方观点
		RETURN (4)
		
	INSERT INTO [bx_PolemizeUsers](ThreadID,UserID,ViewPointType) VALUES(@ThreadID,@UserID,@ViewPointType)
		
	IF @ViewPointType = 0 BEGIN
		UPDATE [bx_Polemizes] SET AgreeCount=AgreeCount+1 WHERE ThreadID=@ThreadID
	END
	ELSE IF @ViewPointType = 1 BEGIN
		UPDATE [bx_Polemizes] SET AgainstCount=AgainstCount+1 WHERE ThreadID=@ThreadID
	END
	ELSE IF @ViewPointType = 2 BEGIN
		UPDATE [bx_Polemizes] SET NeutralCount=NeutralCount+1 WHERE ThreadID=@ThreadID
	END
	
    EXECUTE bx_v5_GetThreadExtendData @ThreadID,4; 
	RETURN (0);
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_FinalQuestion')
	DROP PROCEDURE [bx_v5_FinalQuestion];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_FinalQuestion 
    @ThreadID               int,
	@BestPostID             int,
	@PostRewards            varchar(8000)
AS
BEGIN

	SET NOCOUNT ON;

    DECLARE @ThreadType tinyint;
	SELECT @ThreadType = ThreadType FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID;

	IF @ThreadType <> 2
		RETURN (1); -- FinalQuestionStatus.NotQuestion
	ELSE BEGIN
		DECLARE @IsClosed bit,@ExpiresDate datetime;
		SELECT @IsClosed = IsClosed,@ExpiresDate=ExpiresDate FROM bx_Questions WITH (NOLOCK) WHERE ThreadID = @ThreadID;
		IF @IsClosed = 1 OR @ExpiresDate<GETDATE()
			RETURN (2);  --FinalQuestionStatus.Finaled
        IF NOT EXISTS(SELECT * FROM bx_Posts WITH(NOLOCK) WHERE PostID=@BestPostID AND ThreadID=@ThreadID)
            RETURN 3; --invilad bestpostid
		ELSE BEGIN
			DECLARE @i int,@j int,@PostID int,@Reward int,@TotalReward int,@RewardCount int,@CanGetRewardCount int;--@ErrorCode int
			SET @PostRewards=@PostRewards+N','
			SET @TotalReward=0
			SET @RewardCount=0
			
			SELECT @j=CHARINDEX(':',@PostRewards)
			SELECT @i=CHARINDEX(',',@PostRewards)
			
			WHILE(@j>0 AND @i>2)
				BEGIN	
					SELECT @PostID=SUBSTRING(@PostRewards,0, @j)
					SELECT @PostRewards=SUBSTRING(@PostRewards,@j+1,len(@PostRewards)-@j)
					
					SELECT @i=CHARINDEX(',',@PostRewards)
					SELECT @Reward=SUBSTRING(@PostRewards,0, @i)
					SELECT @PostRewards=SUBSTRING(@PostRewards,@i+1,len(@PostRewards)-@i)
					
					SELECT @j=CHARINDEX(':',@PostRewards)
					SELECT @i=CHARINDEX(',',@PostRewards)
					
					SET @TotalReward=@TotalReward+@Reward
					SET @RewardCount=@RewardCount+1
					
					INSERT INTO [bx_QuestionRewards](ThreadID,PostID,Reward) VALUES(@ThreadID,@PostID,@Reward)
					
				END
				
			SELECT @Reward=Reward,@CanGetRewardCount=RewardCount FROM [bx_Questions] WITH(NOLOCK) WHERE ThreadID=@ThreadID
			IF(@Reward<>@TotalReward)
				BEGIN
					RETURN 4;
				END
			
			IF(@RewardCount>@CanGetRewardCount)
				BEGIN
					RETURN 5;
				END
				
			UPDATE [bx_Questions] SET IsClosed=1,BestPostID=@BestPostID WHERE ThreadID=@ThreadID

            EXECUTE bx_v5_GetThreadExtendData @ThreadID,2; 
			RETURN (0)
		END
	END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_VoteQuestionBestPost')
	DROP PROCEDURE [bx_VoteQuestionBestPost];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_VoteQuestionBestPost 
	@ThreadID           int,
	@UserID             int,
	@BestPostIsUseful   bit
AS
	SET NOCOUNT ON;
	
	DECLARE @IsClosed bit
	
	SELECT @IsClosed = IsClosed FROM [bx_Questions] WITH (NOLOCK) WHERE ThreadID=@ThreadID
	
	IF @IsClosed IS NULL
		RETURN (1) -- 不存在
	ELSE IF @IsClosed = 0
		RETURN (2) -- 还未揭贴
	
	IF(EXISTS (SELECT * FROM [bx_QuestionUsers] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND UserID=@UserID))
		RETURN (3) -- 已经投过
		
	BEGIN TRANSACTION
		
	INSERT INTO [bx_QuestionUsers](ThreadID,UserID,BestPostIsUseful) VALUES(@ThreadID,@UserID,@BestPostIsUseful)
	IF(@@error<>0)
		GOTO Cleanup;
	
	IF @BestPostIsUseful = 1 BEGIN
		UPDATE [bx_Questions] SET UsefulCount=UsefulCount+1 WHERE ThreadID=@ThreadID
		IF(@@error<>0)
			GOTO Cleanup;
	END
	ELSE BEGIN
		UPDATE [bx_Questions] SET UnusefulCount=UnusefulCount+1 WHERE ThreadID=@ThreadID
		IF(@@error<>0)
			GOTO Cleanup;
	END
	
		COMMIT TRANSACTION;

        EXECUTE bx_v5_GetThreadExtendData @ThreadID,2; 
		RETURN (0);
Cleanup:
    BEGIN
    	ROLLBACK TRANSACTION
    	RETURN (-1)
    END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_v5_IsRepliedThread')
	DROP PROCEDURE [bx_v5_IsRepliedThread];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_v5_IsRepliedThread 
    @ThreadID   int,
	@UserID     int
AS
BEGIN

	SET NOCOUNT ON;
	IF EXISTS (SELECT 1 FROM bx_Posts WITH (NOLOCK) WHERE UserID = @UserID AND ThreadID = @ThreadID AND SortOrder < 5000000000000000)
		RETURN (0);
	ELSE
		RETURN (-1);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetMyPointShowInfo')
	DROP PROCEDURE [bx_GetMyPointShowInfo];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetMyPointShowInfo
@UserID  int
AS 
BEGIN

SET NOCOUNT ON;

    DECLARE @Rank int
    SET @Rank=(SELECT COUNT(Price) FROM bx_PointShows WHERE Price>( SELECT Price FROM bx_PointShows WHERE  UserID = @UserID));
    SELECT *,@Rank+1 AS RANK FROM bx_PointShows WHERE UserID =@UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeductPointShowPoint')
	DROP PROCEDURE [bx_DeductPointShowPoint];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeductPointShowPoint 
@UserID int
AS
BEGIN
SET NOCOUNT ON;
    DECLARE @OldPrice int,@OldPoints int;
    SELECT @OldPoints = ShowPoints, @OldPrice = Price FROM bx_PointShows WHERE UserID = @UserID;
    UPDATE bx_PointShows SET ShowPoints = ShowPoints - Price WHERE UserID = @UserID AND ShowPoints > 0;
    DELETE FROM bx_PointShows WHERE UserID = @UserID AND ShowPoints <= 0; 
    SELECT @OldPoints-@OldPrice,@OldPrice;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_IsPointShowUser')
	DROP PROCEDURE [bx_IsPointShowUser];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_IsPointShowUser

@UserID int 
AS
BEGIN
SET NOCOUNT ON;

SELECT COUNT(UserID) FROM bx_PointShows WHERE UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdatePointShowPrice')
	DROP PROCEDURE [bx_UpdatePointShowPrice];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdatePointShowPrice
 @UserID       int
,@AddPoints    int
,@Price        int
,@Content  nvarchar(500)
AS
BEGIN
SET NOCOUNT ON;
IF EXISTS( SELECT * FROM bx_PointShows WHERE UserID = @UserID ) BEGIN
    EXEC bx_SubjoinPointShowPrice @UserID,@AddPoints
    UPDATE bx_PointShows SET Price = @Price ,Content = @Content WHERE UserID = @UserID  AND ShowPoints >= @Price;
    IF  @@ROWCOUNT>0 BEGIN
    EXEC bx_GetMyPointShowInfo @UserID
        RETURN 0;
    END
    ELSE BEGIN
        RETURN 1;
    END
END
ELSE BEGIN
 RETURN -1;
END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SubjoinPointShowPrice')
	DROP PROCEDURE [bx_SubjoinPointShowPrice];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SubjoinPointShowPrice
 @UserID   int
,@Point    int
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE bx_PointShows SET ShowPoints = ShowPoints + @Point WHERE UserID = @UserID;
    RETURN @@ROWCOUNT;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreatePointShow')
	DROP PROCEDURE [bx_CreatePointShow];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreatePointShow
     @UserID int
    ,@Points int
    ,@Price  int
    ,@Content nvarchar(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF Exists( SELECT * FROM bx_PointShows WHERE UserID = @UserID) BEGIN
        RETURN;
    END
    INSERT INTO bx_PointShows( ShowPoints, Price, UserID, Content) VALUES ( @Points, @Price, @UserID, @Content);
    EXEC bx_GetMyPointShowInfo @UserID
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateFriendPointShow')
	DROP PROCEDURE [bx_UpdateFriendPointShow];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateFriendPointShow
     @UserID int
    ,@Points int
    ,@Username nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ShowUserID int;
    DECLARE @FriendUserID int;

    SELECT @ShowUserID = UserID FROM bx_Users WHERE Username = @Username;

    IF @ShowUserID IS NULL BEGIN
        SELECT 4;--用户不存在
        RETURN;
    END

    SELECT @FriendUserID = FriendUserID FROM bx_Friends WHERE UserID = @UserID AND FriendUserID = @ShowUserID
    
    IF @FriendUserID IS NULL BEGIN
        SELECT 3;--不是好友
        RETURN;
    END


    IF EXISTS(SELECT * FROM bx_PointShows WHERE UserID = @FriendUserID)
        BEGIN
            UPDATE bx_PointShows SET ShowPoints = ShowPoints + @Points WHERE UserID = @FriendUserID
            
            SELECT 1;--更新
        END
    ELSE
        BEGIN
            INSERT INTO bx_PointShows(ShowPoints,UserID,Content) VALUES (@Points,@FriendUserID,'')
           
            SELECT 2;--新上榜
        END

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Denouncing_Create')
	DROP PROCEDURE [bx_Denouncing_Create];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Denouncing_Create
	@UserID			int,
	@TargetID		int,
	@TargetUserID	int,
	@Type			tinyint,
	@Content		nvarchar(50),
	@CreateIP		varchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DenouncingID int;

	IF EXISTS(SELECT * FROM bx_Denouncings WHERE TargetID = @TargetID AND Type = @Type)
	BEGIN
		SELECT @DenouncingID = DenouncingID FROM bx_Denouncings WHERE TargetID = @TargetID AND Type = @Type;
	END
	ELSE
	BEGIN
		INSERT INTO bx_Denouncings(TargetID,TargetUserID,Type) VALUES (@TargetID,@TargetUserID,@Type);

		SELECT @DenouncingID = @@IDENTITY;
	END

	IF NOT EXISTS(SELECT * FROM bx_DenouncingContents WHERE DenouncingID = @DenouncingID AND UserID = @UserID)
	BEGIN
		INSERT INTO bx_DenouncingContents (DenouncingID, UserID,Content) VALUES (@DenouncingID, @UserID,@Content);
	END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Denouncing_Delete')
	DROP PROCEDURE [bx_Denouncing_Delete];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Denouncing_Delete
	@DenouncingID	int
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM bx_DenouncingContents WHERE DenouncingID = @DenouncingID;

	DELETE FROM bx_Denouncings WHERE DenouncingID = @DenouncingID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Denouncing_Ignore')
	DROP PROCEDURE [bx_Denouncing_Ignore];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Denouncing_Ignore
	@DenouncingID	int
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE bx_Denouncings SET IsIgnore = 1 WHERE DenouncingID = @DenouncingID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Denouncing_GetByID')
	DROP PROCEDURE [bx_Denouncing_GetByID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Denouncing_GetByID
	@DenouncingID	int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM bx_Denouncings WHERE DenouncingID = @DenouncingID;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Denouncing_CheckState')
	DROP PROCEDURE [bx_Denouncing_CheckState];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Denouncing_CheckState
	@UserID		int,
	@TargetID	int,
	@TargetType	tinyint
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM bx_Denouncings WHERE IsIgnore = 1 AND TargetID = @TargetID AND Type = @TargetType) 
	BEGIN
		RETURN 2;
	END
	
	IF EXISTS (SELECT * FROM bx_DenouncingContents WHERE UserID = UserID AND DenouncingID IN (
		SELECT A.DenouncingID FROM bx_Denouncings A WHERE  TargetID = @TargetID AND Type = @TargetType))
	BEGIN
		RETURN 3;
	END

	RETURN 1;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Denouncing_GetCount')
	DROP PROCEDURE [bx_Denouncing_GetCount];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Denouncing_GetCount
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 1;  --photo
    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 3;  --article
    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 5;  --share
    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 4;  --user
    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 6;  --topic
    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 7;  --reply
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetRunningStepByStepTask')
	DROP PROCEDURE [bx_GetRunningStepByStepTask];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetRunningStepByStepTask
    @UserID int,
    @TaskID uniqueidentifier
AS
BEGIN

    SET NOCOUNT ON;

    SELECT * FROM bx_StepByStepTasks WHERE TaskID = @TaskID AND (UserID = @UserID OR InstanceMode IN (2, 4));

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetRunningStepByStepTasks')
	DROP PROCEDURE [bx_GetRunningStepByStepTasks];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetRunningStepByStepTasks
    @UserID int
AS
BEGIN

    SET NOCOUNT ON;

    SELECT * FROM bx_StepByStepTasks WHERE UserID = @UserID OR InstanceMode IN (2, 4);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetRunningStepByStepTasksByType')
	DROP PROCEDURE [bx_GetRunningStepByStepTasksByType];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetRunningStepByStepTasksByType
    @UserID int,
    @TaskType varchar(200),
    @Param nvarchar(3500)
AS
BEGIN

    SET NOCOUNT ON;


    IF @Param IS NULL
        SELECT * FROM bx_StepByStepTasks WHERE (UserID = @UserID OR InstanceMode IN (2, 4))
        AND Type = @TaskType;
    ELSE
        SELECT * FROM bx_StepByStepTasks WHERE (UserID = @UserID OR InstanceMode IN (2, 4))
        AND Type = @TaskType AND Param = @Param;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_BeginStepByStepTasks')
	DROP PROCEDURE [bx_BeginStepByStepTasks];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_BeginStepByStepTasks
    @TaskID uniqueidentifier,
    @Type varchar(200),
    @UserID int,
    @Param nvarchar(3500),
    @Offset bigint,
    @TotalCount int,
    @Title nvarchar(100),
    @InstanceMode tinyint
AS
BEGIN

    SET NOCOUNT ON;

    IF @InstanceMode = 1 BEGIN

        IF EXISTS ( SELECT * FROM bx_StepByStepTasks WHERE UserID = @UserID AND Type = @Type )
            RETURN 1;  --用户单实例模式，已经有在运行

    END
    ELSE IF @InstanceMode = 2 BEGIN

        IF EXISTS ( SELECT * FROM bx_StepByStepTasks WHERE Type = @Type )
            RETURN 2;  --系统单实例模式，已经有在运行

    END

    INSERT INTO [bx_StepByStepTasks]
        ([TaskID]
        ,[Type]
        ,[UserID]
        ,[Param]
        ,[Offset]
        ,[TotalCount]
        ,[Title]
        ,[InstanceMode])
     VALUES
        (@TaskID
        ,@Type
        ,@UserID
        ,@Param
        ,@Offset
        ,@TotalCount
        ,@Title
        ,@InstanceMode);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateStepByStepTasks')
	DROP PROCEDURE [bx_UpdateStepByStepTasks];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateStepByStepTasks
    @TaskID uniqueidentifier,
    @Param nvarchar(3500),
    @TotalCount int,
    @FinishedCount int,
    @Offset bigint,
    @Title nvarchar(100)
AS
BEGIN

    SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM bx_StepByStepTasks WHERE TaskID = @TaskID) BEGIN

        UPDATE [bx_StepByStepTasks]
             SET [Param] = @Param,
                 [TotalCount] = @TotalCount,
                 [FinishedCount] = @FinishedCount,
                 [Offset] = @Offset,
                 [Title] = @Title,
                 [LastExecuteTime] = GETDATE()
           WHERE TaskID = @TaskID;

    END
    ELSE
        RETURN (1);

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_FinishTask')
	DROP PROCEDURE [bx_FinishTask];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_FinishTask
    @TaskID uniqueidentifier
AS
BEGIN

    SET NOCOUNT ON;

    DELETE [bx_StepByStepTasks] WHERE TaskID = @TaskID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateTempUploadFile')
	DROP PROCEDURE [bx_CreateTempUploadFile];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateTempUploadFile
    @UserID         int,
    @UploadAction   varchar(100),
    @SearchInfo     nvarchar(100),
    @CustomParams   nvarchar(3000),
    @FileName       nvarchar(256),
    @ServerFileName varchar(100),
    @MD5            char(32),
    @FileSize       bigint,
    @FileID         varchar(50),
    @TempUploadFileID int output
AS BEGIN

    SET NOCOUNT ON;

    INSERT INTO bx_TempUploadFiles (UserID, UploadAction, SearchInfo, CustomParams, FileName, ServerFileName, MD5, FileSize, FileID) VALUES (@UserID, @UploadAction, @SearchInfo, @CustomParams, @FileName, @ServerFileName, @MD5, @FileSize, @FileID);
    SELECT @TempUploadFileID = @@IDENTITY;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserTempUploadFiles')
	DROP PROCEDURE [bx_GetUserTempUploadFiles];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserTempUploadFiles
    @UserID        int,
    @UploadAction  varchar(100),
    @SearchInfo   nvarchar(100),
    @TimeLife   int
AS BEGIN

    SET NOCOUNT ON;

    IF @SearchInfo IS NULL
        SELECT * FROM bx_TempUploadFiles WHERE UserID = @UserID AND UploadAction = @UploadAction AND CreateDate > DATEADD(hh, 0 - @TimeLife, GETDATE());
    ELSE
        SELECT * FROM bx_TempUploadFiles WHERE UserID = @UserID AND UploadAction = @UploadAction AND SearchInfo = @SearchInfo AND CreateDate > DATEADD(hh, 0 - @TimeLife, GETDATE());

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetUserTempUploadFile')
	DROP PROCEDURE [bx_GetUserTempUploadFile];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetUserTempUploadFile
    @UserID        int,
    @TempUploadFileID int
AS BEGIN

    SET NOCOUNT ON;

    SELECT * FROM bx_TempUploadFiles WHERE TempUploadFileID = @TempUploadFileID AND UserID = @UserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeleteUserTempUploadFile')
	DROP PROCEDURE [bx_DeleteUserTempUploadFile];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeleteUserTempUploadFile
    @UserID         int,
    @TempUploadFileID  int
AS BEGIN

    SET NOCOUNT ON;

    SELECT [ServerFileName] FROM bx_TempUploadFiles WHERE UserID = @UserID AND TempUploadFileID = @TempUploadFileID;
    DELETE bx_TempUploadFiles WHERE UserID = @UserID AND TempUploadFileID = @TempUploadFileID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_SaveFile')
	DROP PROCEDURE [bx_SaveFile];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_SaveFile
    @UserID         int,
    @TempUploadFileID  int
AS BEGIN

    SET NOCOUNT ON;

    INSERT INTO bx_Files (FileID, ServerFilePath, MD5, FileSize)
        SELECT T.FileID, REPLACE(T.ServerFileName, '_', '\') AS ServerFilePath, T.MD5, T.FileSize
            FROM bx_TempUploadFiles T LEFT JOIN bx_Files F ON T.FileID = F.FileID
            WHERE T.UserID = @UserID AND T.TempUploadFileID = @TempUploadFileID
                AND F.FileID IS NULL;

    SELECT T.TempUploadFileID AS TempUploadFileID, T.FileName AS TempUploadFileName, T.ServerFileName AS TempUploadServerFileName, F.* FROM bx_TempUploadFiles T LEFT JOIN bx_Files F ON T.FileID = F.FileID WHERE T.UserID = @UserID AND T.TempUploadFileID = @TempUploadFileID;

    DELETE bx_TempUploadFiles WHERE UserID = @UserID AND TempUploadFileID = @TempUploadFileID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetTop300DeletingFiles')
	DROP PROCEDURE [bx_GetTop300DeletingFiles];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetTop300DeletingFiles
AS BEGIN

    SET NOCOUNT ON;

    SELECT TOP 300 * FROM [bx_DeletingFiles] ORDER BY DeletingFileID DESC;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetFileByID')
	DROP PROCEDURE [bx_GetFileByID];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetFileByID
    @FileID    varchar(50)
AS BEGIN

    SET NOCOUNT ON;

    SELECT * FROM [bx_Files] WHERE [FileID] = @FileID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_ClearExpiredTempUploadFiles')
	DROP PROCEDURE [bx_ClearExpiredTempUploadFiles];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_ClearExpiredTempUploadFiles
    @HoursBefore    int
AS BEGIN

    SET NOCOUNT ON;

    DECLARE @ClearTempUploadFileIds table(ID int);

    INSERT INTO @ClearTempUploadFileIds (ID) SELECT TempUploadFileID FROM bx_TempUploadFiles WHERE CreateDate < DATEADD(hh, 0 - @HoursBefore, GETDATE());

    SELECT * FROM bx_TempUploadFiles T INNER JOIN @ClearTempUploadFileIds C ON T.TempUploadFileID = C.ID;

    DELETE bx_TempUploadFiles FROM @ClearTempUploadFileIds C WHERE bx_TempUploadFiles.TempUploadFileID = C.ID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAllCates')
	DROP PROCEDURE [bx_GetAllCates];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAllCates
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_ThreadCates ORDER BY SortOrder;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateThreadCate')
	DROP PROCEDURE [bx_CreateThreadCate];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateThreadCate
     @CateName   nvarchar(50)
    ,@Enable     bit
    ,@SortOrder  int
AS
BEGIN
	SET NOCOUNT ON;
    
    DECLARE @CateID int;
	INSERT INTO bx_ThreadCates(CateName,Enable,SortOrder) VALUES(@CateName,@Enable,@SortOrder);
    SET @CateID = @@IDENTITY;
	INSERT INTO bx_ThreadCateModels(CateID,ModelName,Enable,SortOrder) VALUES(@CateID,'默认模板',1,1);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateThreadCate')
	DROP PROCEDURE [bx_UpdateThreadCate];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateThreadCate
     @CateID     int
    ,@CateName   nvarchar(50)
    ,@Enable     bit
    ,@SortOrder  int
AS
BEGIN
	SET NOCOUNT ON;
    IF NOT EXISTS(SELECT * FROM bx_ThreadCates WHERE CateID = @CateID) BEGIN
        SELECT 0;
    END
    ELSE BEGIN
	    UPDATE bx_ThreadCates SET CateName = @CateName, Enable = @Enable, SortOrder = @SortOrder WHERE CateID = @CateID;
        SELECT 1;
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAllModels')
	DROP PROCEDURE [bx_GetAllModels];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAllModels
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_ThreadCateModels ORDER BY SortOrder;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateModel')
	DROP PROCEDURE [bx_CreateModel];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateModel
     @CateID      int
    ,@ModelName   nvarchar(50)
    ,@Enable      bit
    ,@SortOrder   int
AS
BEGIN
	SET NOCOUNT ON;
    IF EXISTS(SELECT * FROM bx_ThreadCates WHERE CateID = @CateID) BEGIN
	    INSERT INTO bx_ThreadCateModels(CateID,ModelName,Enable,SortOrder) VALUES(@CateID,@ModelName,@Enable,@SortOrder);
        SELECT 1;
    END
    ELSE
        SELECT 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetAllThreadCateModelField')
	DROP PROCEDURE [bx_GetAllThreadCateModelField];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetAllThreadCateModelField
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_ThreadCateModelFields ORDER BY SortOrder;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreateThreadCateModelField')
	DROP PROCEDURE [bx_CreateThreadCateModelField];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreateThreadCateModelField
     @ModelID    int
    ,@FieldName  nvarchar(50)
    ,@Enable     bit
    ,@SortOrder  int
    ,@FieldType  nvarchar(50)
    ,@FieldTypeSetting  ntext
    ,@Search            bit
    ,@AdvancedSearch    bit
    ,@DisplayInList     bit
    ,@MustFilled        bit
    ,@Description       nvarchar(1000)
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM bx_ThreadCateModels WHERE ModelID = @ModelID) BEGIN
	    INSERT INTO bx_ThreadCateModelFields(ModelID,FieldName,Enable,SortOrder,FieldType,FieldTypeSetting,
                    Search,AdvancedSearch,DisplayInList,MustFilled,Description) VALUES(
                        @ModelID,@FieldName,@Enable,@SortOrder,@FieldType,@FieldTypeSetting,
                    @Search,@AdvancedSearch,@DisplayInList,@MustFilled,@Description)
        SELECT 1;
    END
    ELSE
        SELECT 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateThreadCateModelField')
	DROP PROCEDURE [bx_UpdateThreadCateModelField];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateThreadCateModelField
     @FieldID    int
    ,@FieldName  nvarchar(50)
    ,@Enable     bit
    ,@SortOrder  int
    ,@FieldTypeSetting  ntext
    ,@Search            bit
    ,@AdvancedSearch    bit
    ,@DisplayInList     bit
    ,@MustFilled        bit
    ,@Description       nvarchar(1000)
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM bx_ThreadCateModelFields WHERE FieldID = @FieldID) BEGIN
	    UPDATE bx_ThreadCateModelFields SET FieldName = @FieldName, Enable = @Enable, SortOrder = @SortOrder
                    ,FieldTypeSetting = @FieldTypeSetting , Search = @Search, AdvancedSearch = @AdvancedSearch, DisplayInList = @DisplayInList
                    ,MustFilled = @MustFilled, Description = @Description WHERE FieldID = @FieldID;
        SELECT 1;
    END
    ELSE
        SELECT 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_GetVars')
	DROP PROCEDURE [bx_GetVars];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_GetVars
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM [bx_Vars];
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateVars')
	DROP PROCEDURE [bx_UpdateVars];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateVars
     @MaxPosts         int
    ,@NewUserID        int
    ,@TotalUsers       int
    ,@YestodayPosts    int
    ,@YestodayTopics   int
    ,@MaxOnlineCount   int
    ,@MaxPostDate      datetime
    ,@MaxOnlineDate    datetime
    ,@LastResetDate    datetime
    ,@NewUsername      nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS(SELECT * FROM [bx_Vars]) BEGIN
        UPDATE [bx_Vars] SET [MaxPosts]=@MaxPosts,[NewUserID]=@NewUserID,[TotalUsers]=@TotalUsers,[YestodayPosts]=@YestodayPosts,[MaxOnlineCount]=@MaxOnlineCount
                              ,[MaxPostDate]=@MaxPostDate,[MaxOnlineDate]=@MaxOnlineDate,[NewUsername]=@NewUsername
                              ,[YestodayTopics]=@YestodayTopics,[LastResetDate]=@LastResetDate
        WHERE [ID]=(SELECT TOP 1 ID FROM [bx_Vars]) AND (DATEPART(year,[LastResetDate]) <> DATEPART (year, getdate()) OR DATEPART(month,[LastResetDate]) <> DATEPART (month, getdate()) OR DATEPART(day,[LastResetDate]) <> DATEPART (day, getdate()));
    END
    ELSE BEGIN
        INSERT INTO [bx_Vars]([MaxPosts],[NewUserID],[TotalUsers],[YestodayPosts],[YestodayTopics],[MaxOnlineCount],[MaxPostDate],[MaxOnlineDate],[LastResetDate],[NewUsername])VALUES(@MaxPosts,@NewUserID,@TotalUsers,@YestodayPosts,@YestodayTopics,@MaxOnlineCount,@MaxPostDate,@MaxOnlineDate,@LastResetDate,@NewUsername);
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdateNewUserVars')
	DROP PROCEDURE [bx_UpdateNewUserVars];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdateNewUserVars
    @GetVars bit
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NewUserID int,@NewUsername nvarchar(50),@TotalUser int;
    
    SELECT TOP 1 @NewUserID=UserID,@NewUsername=Username FROM [bx_Users] WHERE [IsActive] = 1 AND [UserID]<>0 ORDER BY [UserID] DESC;
    
    SELECT @TotalUser=Count(*) FROM [bx_Users] WHERE [IsActive] = 1 AND [UserID]<>0;

    IF @NewUserID IS NOT NULL BEGIN
        IF EXISTS(SELECT * FROM [bx_Vars])
            UPDATE [bx_Vars] SET  NewUserID = @NewUserID, NewUsername = @NewUsername, TotalUsers = @TotalUser WHERE [ID]=(SELECT TOP 1 ID FROM [bx_Vars]);
        ELSE
            INSERT [bx_Vars] (NewUserID,NewUsername,TotalUsers)VALUES(@NewUserID,@NewUsername,@TotalUser);

        IF @GetVars = 1
            SELECT TOP 1 * FROM [bx_Vars];
        SELECT 'ResetVars' AS XCMD; --, * FROM [bx_Vars];
    END
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_DeletePassportClient')
	DROP PROCEDURE [bx_DeletePassportClient];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_DeletePassportClient
@ClientID        int

AS 
BEGIN
SET NOCOUNT ON;

UPDATE bx_PassportClients SET Deleted = 1 WHERE ClientID = @ClientID;
DELETE FROM bx_Instructs WHERE InstructID IN( SELECT TOP 200 InstructID FROM bx_Instructs WHERE ClientID = @ClientID );

DECLARE @InsCount int ;

SET @InsCount = 0;

    DELETE FROM bx_PassportClients WHERE ClientID = @ClientID

SELECT @InsCount;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_CreatePayItem')
	DROP PROCEDURE [bx_CreatePayItem];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_CreatePayItem
 @UserID      int
,@OrderNo     varchar(50)
,@OrderAmount decimal(18,2)
,@Payment     tinyint
,@PayType     tinyint
,@PayValue    int
,@SubmitIp    varchar(50)
,@Remarks    nvarchar(50)
AS
BEGIN

SET NOCOUNT ON;
    
    IF NOT EXISTS( SELECT * FROM bx_Pay WHERE UserID = @UserID AND OrderNo = @OrderNo) BEGIN
        INSERT INTO bx_Pay(UserID,OrderNo,OrderAmount,Payment,PayType,PayValue,SubmitIp,Remarks) VALUES(@UserID,@OrderNo,@OrderAmount,@Payment,@PayType,@PayValue,@SubmitIp,@Remarks);
        RETURN (1);
    END
    ELSE
        RETURN (0);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_UpdatePayItem')
	DROP PROCEDURE [bx_UpdatePayItem];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_UpdatePayItem
 @BuyerEmail      nvarchar(50)
,@OrderNo         varchar(50)
,@TransactionNo   nvarchar(200)
,@PayIp           varchar(50)
,@PayDate         datetime
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @UserID int;
    DECLARE @PayType tinyint;
    DECLARE @PayValue int;
    DECLARE @State bit;
    SELECT @UserID = UserID, @PayType = PayType, @PayValue=PayValue, @State = [State]  FROM bx_Pay WHERE OrderNo = @OrderNo;
    
    IF @State = 0 BEGIN
        UPDATE bx_Pay SET BuyerEmail=@BuyerEmail,TransactionNo = @TransactionNo,PayIp=@PayIp,PayDate=@PayDate,State=1 WHERE OrderNo = @OrderNo;


        DECLARE @P0 int,@P1 int, @P2 int, @P3 int, @P4 int, @P5 int, @P6 int, @P7 int;

        SET @P0=0;
        SET @P1=0;
        SET @P2=0;
        SET @P3=0;
        SET @P4=0;
        SET @P5=0;
        SET @P6=0;
        SET @P7=0;

        IF @PayType = 0 BEGIN
            UPDATE bx_Users SET Point_1 = Point_1 + @PayValue WHERE UserID = @UserID;
            SET @P0 = @PayValue;
        END
        ELSE IF @PayType = 1 BEGIN
            UPDATE bx_Users SET Point_2 = Point_2 + @PayValue WHERE UserID = @UserID;
            SET @P1 = @PayValue;     
        END
        ELSE IF @PayType = 2 BEGIN
            UPDATE bx_Users SET Point_3 = Point_3 + @PayValue WHERE UserID = @UserID;
            SET @P2 = @PayValue;     
        END
        ELSE IF @PayType = 3 BEGIN
            UPDATE bx_Users SET Point_4 = Point_4 + @PayValue WHERE UserID = @UserID;
            SET @P3 = @PayValue;     
        END
        ELSE IF @PayType = 4 BEGIN
            UPDATE bx_Users SET Point_5 = Point_5 + @PayValue WHERE UserID = @UserID;
            SET @P4 = @PayValue;     
        END
        ELSE IF @PayType = 5 BEGIN
            UPDATE bx_Users SET Point_6 = Point_6 + @PayValue WHERE UserID = @UserID;
            SET @P5 = @PayValue;     
        END
        ELSE IF @PayType = 6 BEGIN
            UPDATE bx_Users SET Point_7 = Point_7 + @PayValue WHERE UserID = @UserID;
            SET @P6 = @PayValue;   
        END  
        ELSE IF @PayType = 7 BEGIN
            UPDATE bx_Users SET Point_8 = Point_8 + @PayValue WHERE UserID = @UserID;
            SET @P7 = @PayValue;     
        END

        DECLARE @Cp0 int,@Cp1 int, @Cp2 int, @Cp3 int, @Cp4 int, @Cp5 int, @Cp6 int, @Cp7 int;


        SELECT @Cp0 = Point_1,@Cp1 = Point_2,@Cp2 = Point_3,@Cp3 = Point_4,@Cp4 = Point_5,
                @Cp5 = Point_6,@Cp6 = Point_7,@Cp7 = Point_8 FROM bx_Users WHERE UserID = @UserID;

        SELECT @UserID;

DECLARE @Remarks nvarchar(200);
SET @Remarks = N'订单号' + @OrderNo;
        EXEC bx_CreatePointLogs @UserID
        ,@P0
        ,@P1
        ,@P2
        ,@P3
        ,@P4
        ,@P5
        ,@P6
        ,@P7
        ,@Cp0
        ,@Cp1
        ,@Cp2
        ,@Cp3
        ,@Cp4
        ,@Cp5
        ,@Cp6
        ,@Cp7
        ,N'积分充值'
        ,@Remarks

    RETURN (1);

    END

    RETURN(0)
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_IsAvailibleClick')
	DROP PROCEDURE [bx_IsAvailibleClick];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_IsAvailibleClick 
     @Ip                          varchar(50)
    ,@UserIdentify                varchar(200)
    ,@AllowUserLastClickDateTime  datetime
    ,@AllowIpLastClickDateTime    datetime
    ,@AllowIpTotalClicks            int
    ,@SourceType                  int
    ,@TargetID                      int
AS
BEGIN

 SET NOCOUNT ON;

DECLARE @Var1 int;

SET @Var1 = (SELECT COUNT(*) FROM bx_ClickLogs WHERE Ip = @Ip AND ClickDate >= @AllowIpLastClickDateTime AND SourceType = @SourceType AND TargetID = @TargetID);

IF @Var1>@AllowIpTotalClicks BEGIN
SELECT 0;
RETURN;
END

IF EXISTS (SELECT * FROM bx_ClickLogs WHERE UserIdentify = @UserIdentify  AND TargetID = @TargetID AND ClickDate > @AllowUserLastClickDateTime AND SourceType = @SourceType) BEGIN
    SELECT 0;
    RETURN;
END

INSERT INTO bx_ClickLogs(Ip,UserIdentify,SourceType, TargetID) VALUES(@Ip, @UserIdentify, @SourceType, @TargetID);
SELECT 1;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Chat_GetLastMessages')
	DROP PROCEDURE [bx_Chat_GetLastMessages];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Chat_GetLastMessages
 @UserID          int
,@TargetUserID    int
,@LastMessageID    int
AS
BEGIN
    SET NOCOUNT ON;

    IF @LastMessageID = 0
        SELECT TOP 20 * FROM bx_ChatMessages WHERE UserID = @UserID AND TargetUserID = @TargetUserID ORDER BY MessageID DESC;
    ELSE
        SELECT TOP 20 * FROM bx_ChatMessages WHERE UserID = @UserID AND TargetUserID = @TargetUserID AND MessageID > @LastMessageID ORDER BY MessageID DESC;

     UPDATE bx_ChatMessages SET IsRead = 1 WHERE UserID = @UserID AND TargetUserID = @TargetUserID AND IsRead = 0;
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Chat_GetChatSession')
	DROP PROCEDURE [bx_Chat_GetChatSession];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Chat_GetChatSession
      @UserID         int
     ,@TargetUserID   int
AS BEGIN

    SET NOCOUNT ON;

    SELECT * FROM bx_ChatSessions WHERE UserID = @UserID AND TargetUserID = @TargetUserID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Chat_SendMessage')
	DROP PROCEDURE [bx_Chat_SendMessage];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Chat_SendMessage
      @UserID         int
     ,@TargetUserID   int
     ,@Content        nvarchar(3000)
     ,@CreateIP       varchar(50)
     ,@GetNewMessages bit
     ,@LastMessageID  int
AS BEGIN

    SET NOCOUNT ON;

    DECLARE @NewMessageID int;

    INSERT INTO
        [bx_ChatMessages](
             [UserID]
            ,[TargetUserID]
            ,[IsReceive]
            ,[IsRead]
            ,[Content]
            ,[CreateIP]
        ) VALUES (
             @UserID
            ,@TargetUserID
            ,0
            ,1
            ,@Content
            ,@CreateIP
        );

    SELECT @NewMessageID = @@IDENTITY;

    INSERT INTO
        [bx_ChatMessages](
             [UserID]
            ,[TargetUserID]
            ,[IsReceive]
            ,[IsRead]
            ,[FromMessageID]
            ,[Content]
            ,[CreateIP]
        ) VALUES (
             @TargetUserID
            ,@UserID
            ,1
            ,0
            ,@NewMessageID
            ,@Content
            ,@CreateIP
        );

    DECLARE @Now datetime;
    SET @Now = GETDATE();

    UPDATE [bx_ChatSessions] SET TotalMessages = TotalMessages + 1, LastMessage = @Content, UpdateDate = @Now WHERE UserID = @UserID AND TargetUserID = @TargetUserID;
    IF @@ROWCOUNT = 0
        INSERT INTO [bx_ChatSessions] (UserID, TargetUserID, TotalMessages, LastMessage, CreateDate) VALUES (@UserID, @TargetUserID, 1, @Content, @Now);

    UPDATE [bx_ChatSessions] SET TotalMessages = TotalMessages + 1, UnreadMessages = UnreadMessages + 1, LastMessage = @Content, UpdateDate = @Now WHERE UserID = @TargetUserID AND TargetUserID = @UserID;
    IF @@ROWCOUNT = 0
        INSERT INTO [bx_ChatSessions] (UserID, TargetUserID, TotalMessages, UnreadMessages, LastMessage, CreateDate) VALUES (@TargetUserID, @UserID, 1, 1, @Content, @Now);

    UPDATE [bx_UserVars] SET UnreadMessages = UnreadMessages + 1 WHERE UserID = @TargetUserID;

    IF @GetNewMessages = 1
        EXEC bx_Chat_GetLastMessages @UserID, @TargetUserID, @LastMessageID;

END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_Chat_UpdateMessageContentKeywords')
	DROP PROCEDURE [bx_Chat_UpdateMessageContentKeywords];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_Chat_UpdateMessageContentKeywords
    @MessageID                int,
    @KeywordVersion           varchar(32),
    @Content                  ntext,
    @ContentReverter          ntext
AS BEGIN


    SET NOCOUNT ON;

    IF @Content IS NOT NULL BEGIN

        IF @KeywordVersion IS NOT NULL
            UPDATE bx_ChatMessages SET Content = @Content, KeywordVersion = @KeywordVersion WHERE MessageID = @MessageID;
        ELSE
            UPDATE bx_ChatMessages SET Content = @Content WHERE MessageID = @MessageID;

    END

    IF @ContentReverter IS NOT NULL BEGIN

        UPDATE bx_ChatMessageReverters SET ContentReverter = @ContentReverter WHERE MessageID = @MessageID;
        IF @@ROWCOUNT = 0
            INSERT INTO bx_ChatMessageReverters (MessageID, ContentReverter) VALUES (@MessageID, @ContentReverter);

    END


END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_OperationLog_Create')
	DROP PROCEDURE [bx_OperationLog_Create];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_OperationLog_Create
	@OperatorID		int,
	@TargetID_1		int,
	@TargetID_2		int,
	@TargetID_3		int,
	@OperatorIP		varchar(50),
	@OperationType	varchar(100),
	@Message		nvarchar(1000),
	@CreateTime		datetime
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [bx_OperationLogs] (
		OperatorID, 
		TargetID_1, 
		TargetID_2, 
		TargetID_3, 
		OperatorIP,
		OperationType, 
		Message, 
		CreateTime
	) VALUES (
		@OperatorID, 
		@TargetID_1, 
		@TargetID_2, 
		@TargetID_3, 
		@OperatorIP, 
		@OperationType, 
		@Message, 
		@CreateTime
	);
END
GO

IF EXISTS (SELECT * FROM [sysobjects] WHERE [type] = 'P' AND [name] = 'bx_IPLogs_Create')
	DROP PROCEDURE [bx_IPLogs_Create];
GO

-- =============================================
-- StoredProcedure For bbsmax 5
-- Copyright: max labs
-- =============================================
CREATE PROCEDURE bx_IPLogs_Create
	@UserID		    int,
	@Username		nvarchar(50),
	@NewIP		    varchar(50),
	@CreateDate		datetime,
    @OldIP          varchar(50),
    @VisitUrl       varchar(200)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [bx_IPLogs] (
		UserID,
        Username,
        NewIP,
		CreateDate,
        OldIP,
        VisitUrl
	) VALUES (
		@UserID,
        @Username,
        @NewIP,
		@CreateDate,
        @OldIP,
        @VisitUrl
	);
END
GO

--删除相册的触发器
EXEC bx_Drop 'bx_Albums_AfterDelete';

GO


CREATE TRIGGER [bx_Albums_AfterDelete]
	ON [bx_Albums]
	AFTER DELETE
AS
BEGIN
	
	SET NOCOUNT ON;
	
	DECLARE @tempTable table(UserID int, TotalAlbums int);

	INSERT INTO @tempTable 
		SELECT DISTINCT UserID,
			(ISNULL((SELECT COUNT(*) FROM [bx_Albums] AS m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
		FROM [DELETED] T;
	
	UPDATE [bx_Users]
		SET
			bx_Users.TotalAlbums = T.TotalAlbums
		FROM @tempTable T
		WHERE
			T.UserID = bx_Users.UserID;

	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, TotalAlbums FROM @tempTable;
END

GO
--新建相册的触发器
EXEC bx_Drop 'bx_Albums_AfterInsert';

GO


CREATE TRIGGER [bx_Albums_AfterInsert]
	ON [bx_Albums]
	AFTER INSERT
AS
BEGIN
	
	SET NOCOUNT ON;

	DECLARE @tempTable table(UserID int, TotalAlbums int);

	INSERT INTO @tempTable 
		SELECT DISTINCT UserID,
			ISNULL((SELECT COUNT(*) FROM [bx_Albums] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0)
		FROM [INSERTED] T;
	
	UPDATE [bx_Users]
		SET
			bx_Users.TotalAlbums = T.TotalAlbums
		FROM @tempTable T
		WHERE
			T.UserID = bx_Users.UserID;

	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, TotalAlbums FROM @tempTable;	
END
GO
--创建新相片的触发器
EXEC bx_Drop 'bx_Albums_AfterUpdate';

GO

CREATE TRIGGER [bx_Albums_AfterUpdate]
	ON [bx_Albums]
	AFTER UPDATE
AS
BEGIN

	SET NOCOUNT ON;
	SELECT 'ResetAlbum' AS XCMD, [INSERTED].* FROM [INSERTED];

END

GO
GO
CREATE TRIGGER bx_Attachments_AfterDelete  ON [bx_Attachments] 
AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;

	DELETE bx_Files
		WHERE FileID IN (SELECT FileID FROM [DELETED])
			AND FileID NOT IN (SELECT FileID FROM [bx_UsedFileIds] WHERE FileID IN (SELECT FileID FROM [DELETED]))

END
GO
--发表新日志的触发器
EXEC bx_Drop 'bx_BlogArticles_AfterDelete';

GO


CREATE TRIGGER [bx_BlogArticles_AfterDelete]
	ON [bx_BlogArticles]
	AFTER DELETE
AS
BEGIN
	
	SET NOCOUNT ON;
	
	DELETE bx_Denouncings WHERE Type=3 AND TargetID IN (SELECT ArticleID FROM [DELETED]);
	
	DECLARE @tempTable table(CategoryID int, TotalArticles int);

	INSERT INTO @tempTable 
		SELECT DISTINCT CategoryID,
				(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.CategoryID = T.CategoryID), 0))
		FROM [DELETED] T;
	
	UPDATE [bx_BlogCategories]
		SET
			bx_BlogCategories.TotalArticles = T.TotalArticles
		FROM @tempTable T
		WHERE
			T.CategoryID = bx_BlogCategories.CategoryID;


	DECLARE @tempTable2 table(UserID int, TotalBlogArticles int);

	INSERT INTO @tempTable2 
		SELECT DISTINCT UserID,
			(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
		FROM [DELETED] T;

	UPDATE [bx_Users]
		SET
			bx_Users.TotalBlogArticles = T.TotalBlogArticles
		FROM @tempTable2 T
		WHERE
			T.UserID = [bx_Users].UserID;
	
	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, TotalBlogArticles FROM @tempTable2;
	
END

GO
--发表新日志的触发器
EXEC bx_Drop 'bx_BlogArticles_AfterInsert';

GO


CREATE TRIGGER [bx_BlogArticles_AfterInsert]
	ON [bx_BlogArticles]
	AFTER INSERT
AS
BEGIN
	
	SET NOCOUNT ON;
	
	DECLARE @tempTable table(CategoryID int, TotalArticles int);

	INSERT INTO @tempTable 
		SELECT DISTINCT CategoryID,
			(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.CategoryID = T.CategoryID), 0))
		FROM [INSERTED] T;
	
	UPDATE [bx_BlogCategories]
		SET
			bx_BlogCategories.TotalArticles = T.TotalArticles
		FROM @tempTable T
		WHERE
			T.CategoryID = bx_BlogCategories.CategoryID;

	
	DECLARE @tempTable2 table(UserID int, TotalBlogArticles int);

	INSERT INTO @tempTable2 
		SELECT DISTINCT UserID,
			(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
		FROM [INSERTED] T;
	
	UPDATE [bx_Users]
		SET
			bx_Users.TotalBlogArticles = T.TotalBlogArticles
		FROM @tempTable2 T
		WHERE
			T.UserID = [bx_Users].UserID;
	
	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, TotalBlogArticles FROM @tempTable2;
	
END

GO
--发表新日志的触发器
EXEC bx_Drop 'bx_BlogArticles_AfterInsert';

GO


CREATE TRIGGER [bx_BlogArticles_AfterUpdate]
	ON [bx_BlogArticles]
	AFTER UPDATE
AS
BEGIN
	
	SET NOCOUNT ON;

	--如果CategoryID字段被更改，需要重新统计受影响的日志分类的TotalArticles值
	IF UPDATE([CategoryID]) BEGIN

		DECLARE @tempTable table(CategoryID int, TotalArticles int);

		INSERT INTO @tempTable 
			SELECT DISTINCT CategoryID,
				(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.CategoryID = T.CategoryID), 0))
			FROM [INSERTED] T;

		INSERT INTO @tempTable 
			SELECT DISTINCT CategoryID,
				(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.CategoryID = T.CategoryID), 0))
			FROM [DELETED] T
			WHERE CategoryID NOT IN (SELECT CategoryID FROM @tempTable);
		
		UPDATE [bx_BlogCategories]
			SET
				bx_BlogCategories.TotalArticles = T.TotalArticles
			FROM @tempTable T
			WHERE
				T.CategoryID = bx_BlogCategories.CategoryID;

	END

	--如果UserID字段被更改，需要重新统计受影响的用户的TotalBlogArticles值
	IF UPDATE([UserID]) BEGIN
		
		DECLARE @tempTable2 table(UserID int, TotalBlogArticles int);

		INSERT INTO @tempTable2 
			SELECT DISTINCT UserID,
				(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
			FROM [INSERTED] T;

		INSERT INTO @tempTable2 
			SELECT DISTINCT UserID,
				(ISNULL((SELECT COUNT(*) FROM [bx_BlogArticles] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
			FROM [DELETED] T
			WHERE UserID NOT IN (SELECT UserID FROM @tempTable2);

		UPDATE [bx_Users]
			SET
				bx_Users.TotalBlogArticles = T.TotalBlogArticles
			FROM @tempTable2 T
			WHERE
				T.UserID = [bx_Users].UserID;
		
		--发出重新填充UserInfo的XCMD命令
		SELECT 'ResetUser' AS XCMD, UserID, TotalBlogArticles FROM @tempTable2;

	END
	
END

GO
EXEC bx_Drop 'bx_ChatMessages_AfterDelete';

GO

CREATE TRIGGER [bx_ChatMessages_AfterDelete]
	ON [bx_ChatMessages]
	AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(UserID int, UnreadMessages int);
	INSERT INTO @tempTable 
		SELECT DISTINCT UserID,
			ISNULL((SELECT COUNT(*) FROM [bx_ChatMessages] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.IsRead = 0), 0)
		FROM [DELETED] T;
	
	UPDATE [bx_UserVars]
		SET
			bx_UserVars.UnreadMessages = T.UnreadMessages
		FROM @tempTable T
		WHERE
			T.UserID = bx_UserVars.UserID;

	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetAuthUser' AS XCMD, UserID, UnreadMessages FROM @tempTable;	

	DECLARE @tempTable2 table(UserID int, TargetUserID int, TotalMessages int, UnreadMessages int);
	INSERT INTO @tempTable2
		SELECT DISTINCT UserID, TargetUserID,
			ISNULL((SELECT COUNT(*) FROM [bx_ChatMessages] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.TargetUserID = T.TargetUserID), 0),
			ISNULL((SELECT COUNT(*) FROM [bx_ChatMessages] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.IsRead = 0), 0)
		FROM [DELETED] T;
	
	UPDATE [bx_ChatSessions]
		SET
			bx_ChatSessions.TotalMessages = T.TotalMessages,
			bx_ChatSessions.UnreadMessages = T.UnreadMessages
		FROM @tempTable2 T
		WHERE
			T.UserID = bx_ChatSessions.UserID
			AND
			T.TargetUserID = bx_ChatSessions.TargetUserID;

END
GO
EXEC bx_Drop 'bx_ChatMessages_AfterUpdate';

GO

CREATE TRIGGER [bx_ChatMessages_AfterUpdate]
	ON [bx_ChatMessages]
	AFTER UPDATE
AS
BEGIN

	SET NOCOUNT ON;
	
	IF UPDATE(IsRead) BEGIN
	
	DECLARE @tempTable table(UserID int, UnreadMessages int);
	INSERT INTO @tempTable 
		SELECT DISTINCT UserID,
			ISNULL((SELECT COUNT(*) FROM [bx_ChatMessages] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.IsRead = 0), 0)
		FROM [DELETED] T;
	
	UPDATE [bx_UserVars]
		SET
			bx_UserVars.UnreadMessages = T.UnreadMessages
		FROM @tempTable T
		WHERE
			T.UserID = bx_UserVars.UserID;

	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetAuthUser' AS XCMD, UserID, UnreadMessages FROM @tempTable;	

	DECLARE @tempTable2 table(UserID int, TargetUserID int, TotalMessages int, UnreadMessages int);
	INSERT INTO @tempTable2
		SELECT DISTINCT UserID, TargetUserID,
			ISNULL((SELECT COUNT(*) FROM [bx_ChatMessages] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.TargetUserID = T.TargetUserID), 0),
			ISNULL((SELECT COUNT(*) FROM [bx_ChatMessages] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.TargetUserID = T.TargetUserID AND m.IsRead = 0), 0)
		FROM [DELETED] T;
	
	UPDATE [bx_ChatSessions]
		SET
			bx_ChatSessions.TotalMessages = T.TotalMessages,
			bx_ChatSessions.UnreadMessages = T.UnreadMessages
		FROM @tempTable2 T
		WHERE
			T.UserID = bx_ChatSessions.UserID
			AND
			T.TargetUserID = bx_ChatSessions.TargetUserID;

	END

END



GO

EXEC bx_Drop 'bx_ChatSessions_After_Delete';

GO


CREATE TRIGGER bx_ChatSessions_After_Delete ON bx_ChatSessions
FOR DELETE 
AS 
SET NOCOUNT ON;

DELETE FROM  bx_ChatMessages  FROM( SELECT * FROM DELETED) as t  WHERE bx_ChatMessages.UserID =t.UserID AND bx_ChatMessages.TargetUserID = t.TargetUserID



GO
--删除评论触发器
EXEC bx_Drop 'bx_Comments_AfterDelete';

GO


CREATE TRIGGER [bx_Comments_AfterDelete]
	ON [bx_Comments]
	AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(Type int,TargetID int,TotalCount int);

	INSERT INTO @tempTable 
		SELECT DISTINCT Type,TargetID,
			(ISNULL((SELECT COUNT(*) FROM [bx_Comments] as m WITH (NOLOCK) WHERE m.TargetID = T.TargetID AND m.IsApproved = 1 AND m.Type = T.Type), 0))
		FROM [DELETED] T;

	IF EXISTS ( SELECT * FROM @tempTable WHERE Type = 1 ) BEGIN

		UPDATE [bx_Users]
			SET
				TotalComments = T.TotalCount
			FROM @tempTable T
			WHERE
				T.TargetID = [bx_Users].UserID AND T.Type = 1;

		--发出重新填充UserInfo的XCMD命令
		SELECT 'ResetUser' AS XCMD, TargetID AS UserID, TotalCount AS TotalComments FROM @tempTable WHERE Type = 1;

	END

	UPDATE [bx_BlogArticles]
		SET
			TotalComments = T.TotalCount
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_BlogArticles].ArticleID AND T.Type = 2;
			
	UPDATE [bx_Doings]
		SET
			TotalComments = T.TotalCount
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_Doings].DoingID AND T.Type = 3;
			
	UPDATE [bx_Photos]
		SET
			TotalComments = T.TotalCount
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_Photos].PhotoID AND T.Type = 4;
			
	--UPDATE [bx_Shares]
		--SET
			--TotalComments = T.TotalCount
		--FROM @tempTable T
		--WHERE
			--T.TargetID = [bx_Shares].ShareID AND T.Type = 5;
			
			
	UPDATE [bx_UserShares]
		SET
			CommentCount = T.TotalCount
			--LastCommentDate = GETDATE()
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_UserShares].UserShareID AND T.Type = 5;
			
			
		-- 动态枚举值 -> 评论类型枚举值
		--Share = 0 -> 5
		------------------UploadPicture = 2 -> 4 不要了
		--UpdateDoing = 5 -> 3
		--WriteArticle = 6 -> 2
		DECLARE @FeedCommentTable table(FType int,FTargetID int,FTotalCount int,Cids varchar(50));
		INSERT INTO @FeedCommentTable SELECT 
			CASE WHEN Type = 5 THEN 0
			--WHEN Type = 4 THEN 2
			WHEN Type = 3 THEN 5
			WHEN Type = 2 THEN 6
			ELSE -1 END
		   ,TargetID
		   ,ISNULL(TotalCount,0)
		   ,(SELECT CAST(ISNULL(MIN(CommentID),0) as varchar(20)) +','+ CAST(ISNULL(MAX(CommentID),0) as varchar(20)) FROM bx_Comments C WHERE C.Type = T.Type AND C.TargetID = T.TargetID AND C.IsApproved = 1)
		   FROM @tempTable T WHERE T.Type in(2,3,5) -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
		
		IF @@ROWCOUNT > 0 BEGIN
			UPDATE bx_Feeds SET CommentInfo = T.Cids + ',' + CAST(T.FTotalCount as varchar(20))	FROM @FeedCommentTable T 
				WHERE CommentTargetID = T.FTargetID AND ActionType = T.FType;  -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
				
			SELECT 'ResetFeedCommentInfo' AS XCMD, ID AS FeedID, CommentInfo FROM bx_Feeds F INNER JOIN @FeedCommentTable T ON F.CommentTargetID = T.FTargetID AND F.ActionType = T.FType;  -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
		END	
END

GO
--添加评论触发器
EXEC bx_Drop 'bx_Comments_AfterInsert';

GO


CREATE TRIGGER [bx_Comments_AfterInsert]
	ON [bx_Comments]
	AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(Type int,TargetID int,TotalCount int);

	INSERT INTO @tempTable 
		SELECT DISTINCT Type,TargetID,
			(ISNULL((SELECT COUNT(*) FROM [bx_Comments] as m WITH (NOLOCK) WHERE m.TargetID = T.TargetID AND m.IsApproved = 1 AND m.Type = T.Type), 0))
		FROM [INSERTED] T;

	IF EXISTS ( SELECT * FROM @tempTable WHERE Type = 1 ) BEGIN

		UPDATE [bx_Users]
			SET
				TotalComments = T.TotalCount
			FROM @tempTable T
			WHERE
				T.TargetID = [bx_Users].UserID AND T.Type = 1;

   		--发出重新填充UserInfo的XCMD命令
		SELECT 'ResetUser' AS XCMD, TargetID AS UserID, TotalCount AS TotalComments FROM @tempTable WHERE Type = 1;

	END

	UPDATE [bx_BlogArticles]
		SET
			TotalComments = T.TotalCount,
			LastCommentDate = GETDATE()
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_BlogArticles].ArticleID AND T.Type = 2;
			
	UPDATE [bx_Doings]
		SET
			TotalComments = T.TotalCount,
			LastCommentDate = GETDATE()
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_Doings].DoingID AND T.Type = 3;
			
	UPDATE [bx_Photos]
		SET
			TotalComments = T.TotalCount,
			LastCommentDate = GETDATE()
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_Photos].PhotoID AND T.Type = 4;
			
	UPDATE [bx_UserShares]
		SET
			CommentCount = T.TotalCount
			--LastCommentDate = GETDATE()
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_UserShares].UserShareID AND T.Type = 5;
			
			
		-- 动态枚举值 -> 评论类型枚举值
		--Share = 0 -> 5
		------------------UploadPicture = 2 -> 4 不要了
		--UpdateDoing = 5 -> 3
		--WriteArticle = 6 -> 2
		DECLARE @FeedCommentTable table(FType int,FTargetID int,FTotalCount int,Cids varchar(50));
		INSERT INTO @FeedCommentTable SELECT 
			CASE WHEN Type = 5 THEN 0
			--WHEN Type = 4 THEN 2
			WHEN Type = 3 THEN 5
			WHEN Type = 2 THEN 6
			ELSE -1 END
		   ,TargetID
		   ,ISNULL(TotalCount,0)
		   ,(SELECT CAST(ISNULL(MIN(CommentID),0) as varchar(20)) +','+ CAST(ISNULL(MAX(CommentID),0) as varchar(20)) FROM bx_Comments C WHERE C.Type = T.Type AND C.TargetID = T.TargetID AND C.IsApproved = 1)
		   FROM @tempTable T WHERE T.Type in(2,3,5) -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
		
		IF @@ROWCOUNT > 0 BEGIN
			UPDATE bx_Feeds SET CommentInfo = T.Cids + ',' + CAST(T.FTotalCount as varchar(20))	FROM @FeedCommentTable T 
				WHERE CommentTargetID = T.FTargetID AND ActionType = T.FType;  -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
				
			SELECT 'ResetFeedCommentInfo' AS XCMD, ID AS FeedID, CommentInfo FROM bx_Feeds F INNER JOIN @FeedCommentTable T ON F.CommentTargetID = T.FTargetID AND F.ActionType = T.FType;  -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
		END	
END
GO
--更新评论触发器
EXEC bx_Drop 'bx_Comments_AfterUpdate';

GO


CREATE TRIGGER [bx_Comments_AfterUpdate]
	ON [bx_Comments]
	AFTER UPDATE
AS
BEGIN
	
	SET NOCOUNT ON;
	
	IF UPDATE([IsApproved]) BEGIN

		DECLARE @tempTable table(Type int,TargetID int,TotalCount int);

		INSERT INTO @tempTable 
		SELECT DISTINCT Type,TargetID,
				(ISNULL((SELECT COUNT(*) FROM [bx_Comments] as m WITH (NOLOCK) WHERE m.TargetID = T.TargetID AND m.IsApproved = 1), 0))
			FROM [INSERTED] T;


		IF EXISTS ( SELECT * FROM @tempTable WHERE Type = 1 ) BEGIN

			UPDATE [bx_Users]
				SET
					TotalComments = T.TotalCount
				FROM @tempTable T
				WHERE
					T.TargetID = [bx_Users].UserID AND T.Type = 1;

   			--发出重新填充UserInfo的XCMD命令
			SELECT 'ResetUser' AS XCMD, TargetID AS UserID, TotalCount AS TotalComments FROM @tempTable WHERE Type = 1;

		END

		
		UPDATE [bx_BlogArticles]
			SET
				TotalComments = T.TotalCount
			FROM @tempTable T
			WHERE
				T.TargetID = [bx_BlogArticles].ArticleID AND T.Type = 2;
			
			
		UPDATE [bx_Doings]
			SET
				TotalComments = T.TotalCount
			FROM @tempTable T
			WHERE
				T.TargetID = [bx_Doings].DoingID AND T.Type = 3;
			

		UPDATE [bx_Photos]
			SET
				TotalComments = T.TotalCount
			FROM @tempTable T
			WHERE
				T.TargetID = [bx_Photos].PhotoID AND T.Type = 4;

			
			
		--UPDATE [bx_Shares]
			--SET
				--TotalComments = T.TotalCount
			--FROM @tempTable T
			--WHERE
				--T.TargetID = [bx_Shares].ShareID AND T.Type = 5;
				
		UPDATE [bx_UserShares]
		SET
			CommentCount = T.TotalCount
			--LastCommentDate = GETDATE()
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_UserShares].UserShareID AND T.Type = 5;
			


		-- 动态枚举值 -> 评论类型枚举值
		--Share = 0 -> 5
		------------------UploadPicture = 2 -> 4 不要了
		--UpdateDoing = 5 -> 3
		--WriteArticle = 6 -> 2
		DECLARE @FeedCommentTable table(FType int,FTargetID int,FTotalCount int,Cids varchar(50));
		INSERT INTO @FeedCommentTable SELECT 
			CASE WHEN Type = 5 THEN 0
			--WHEN Type = 4 THEN 2
			WHEN Type = 3 THEN 5
			WHEN Type = 2 THEN 6
			ELSE -1 END
		   ,TargetID
		   ,ISNULL(TotalCount,0)
		   ,(SELECT CAST(ISNULL(MIN(CommentID),0) as varchar(20)) +','+ CAST(ISNULL(MAX(CommentID),0) as varchar(20)) FROM bx_Comments C WHERE C.Type = T.Type AND C.TargetID = T.TargetID AND C.IsApproved = 1)
		   FROM @tempTable T WHERE T.Type in(2,3,5) -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
		
		IF @@ROWCOUNT > 0 BEGIN
			UPDATE bx_Feeds SET CommentInfo = T.Cids + ',' + CAST(T.FTotalCount as varchar(20))	FROM @FeedCommentTable T 
				WHERE CommentTargetID = T.FTargetID AND ActionType = T.FType;  -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
				
			SELECT 'ResetFeedCommentInfo' AS XCMD, ID AS FeedID, CommentInfo FROM bx_Feeds F INNER JOIN @FeedCommentTable T ON F.CommentTargetID = T.FTargetID AND F.ActionType = T.FType;  -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
		END	
	END

END

GO
CREATE TRIGGER bx_DeletingFiles_AfterInsert  ON [bx_DeletingFiles]
AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP 300 'DeleteFile' AS XCMD, DeletingFileID, ServerFilePath FROM INSERTED;
END
GO
CREATE TRIGGER [bx_Denouncings_AfterDelete]
	ON [bx_Denouncings]
	AFTER DELETE
AS
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT 'ResetDenouncingCount' AS XCMD;
END

GO
CREATE TRIGGER bx_DiskFiles_AfterDelete  ON [bx_DiskFiles] 
AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;

	DELETE bx_Files
		WHERE FileID IN (SELECT FileID FROM [DELETED])
			AND FileID NOT IN (SELECT FileID FROM [bx_UsedFileIds] WHERE FileID IN (SELECT FileID FROM [DELETED]))

-------------------

	DECLARE @tempTable table(DirectoryID int ,TotalSize bigint, TotalFiles int);

	INSERT INTO @tempTable 
		SELECT DISTINCT DirectoryID,
			(ISNULL((SELECT SUM(FileSize) FROM [bx_DiskFiles] f WITH (NOLOCK) WHERE f.DirectoryID = T.DirectoryID), 0))
			,(ISNULL((SELECT COUNT(*) FROM [bx_DiskFiles] f WITH (NOLOCK) WHERE f.DirectoryID = T.DirectoryID), 0))
		FROM [DELETED] T;

	UPDATE [bx_DiskDirectories] SET bx_DiskDirectories.TotalSize = t.TotalSize , TotalFiles = t.TotalFiles FROM @tempTable t WHERE [bx_DiskDirectories].DirectoryID = t.DirectoryID;

-------------------

	DECLARE @tempTable2 table(UserID int, TotalSize bigint, TotalDiskFiles int);

	INSERT INTO @tempTable2
		SELECT DISTINCT UserID,
			(ISNULL((SELECT SUM(TotalSize) FROM [bx_DiskDirectories] d WITH (NOLOCK) WHERE d.UserID = T.UserID), 0)),
			(ISNULL((SELECT SUM(TotalFiles) FROM [bx_DiskDirectories] d WITH (NOLOCK) WHERE d.UserID = T.UserID), 0))
		FROM [DELETED] T;

	UPDATE [bx_UserVars]
		SET
			bx_UserVars.UsedDiskSpaceSize = t.TotalSize,
			bx_UserVars.TotalDiskFiles = t.TotalDiskFiles
		FROM @tempTable2 t
		WHERE
			t.UserID = bx_UserVars.UserID;

-------------------

	SELECT 'ResetAuthUser' AS XCMD, UserID,  TotalSize AS UsedDiskSpaceSize, TotalDiskFiles FROM @tempTable2;

END
GO
CREATE TRIGGER bx_DiskFiles_AfterInsert  ON [bx_DiskFiles] 
FOR INSERT
AS
BEGIN
	SET NOCOUNT ON;

-------------------

	DECLARE @tempTable table(DirectoryID int ,TotalSize bigint, TotalFiles int);

	INSERT INTO @tempTable 
		SELECT DISTINCT DirectoryID,
			(ISNULL((SELECT SUM(FileSize) FROM [bx_DiskFiles] f WITH (NOLOCK) WHERE f.DirectoryID = T.DirectoryID), 0))
			,(ISNULL((SELECT COUNT(*) FROM [bx_DiskFiles] f WITH (NOLOCK) WHERE f.DirectoryID = T.DirectoryID), 0))
		FROM [INSERTED] T;

	UPDATE [bx_DiskDirectories] SET bx_DiskDirectories.TotalSize = t.TotalSize , TotalFiles = t.TotalFiles FROM @tempTable t WHERE [bx_DiskDirectories].DirectoryID = t.DirectoryID;

-------------------

	DECLARE @tempTable2 table(UserID int, TotalSize bigint, TotalDiskFiles int);

	INSERT INTO @tempTable2
		SELECT DISTINCT UserID,
			(ISNULL((SELECT SUM(TotalSize) FROM [bx_DiskDirectories] d WITH (NOLOCK) WHERE d.UserID = T.UserID), 0)),
			(ISNULL((SELECT SUM(TotalFiles) FROM [bx_DiskDirectories] d WITH (NOLOCK) WHERE d.UserID = T.UserID), 0))
		FROM [INSERTED] T;

	UPDATE [bx_UserVars]
		SET
			bx_UserVars.UsedDiskSpaceSize = t.TotalSize,
			bx_UserVars.TotalDiskFiles = t.TotalDiskFiles
		FROM @tempTable2 t
		WHERE
			t.UserID = bx_UserVars.UserID;

-------------------

	SELECT 'ResetAuthUser' AS XCMD, UserID,  TotalSize AS UsedDiskSpaceSize, TotalDiskFiles FROM @tempTable2;

END
GO
CREATE TRIGGER bx_DiskFiles_AfterUpdate ON [bx_DiskFiles] 
FOR  UPDATE
AS
BEGIN

	SET NOCOUNT ON;

	IF (UPDATE(DirectoryID)) BEGIN
	
		DECLARE @DirectoryIDTable table(DirectoryID int);
		
		INSERT @DirectoryIDTable SELECT DISTINCT DirectoryID FROM INSERTED;
		INSERT @DirectoryIDTable SELECT DISTINCT DirectoryID FROM DELETED;

		DECLARE @tempTable table(DirectoryID int, TotalSize bigint, TotalFiles int);
	
		INSERT INTO @tempTable 
			SELECT DISTINCT DirectoryID,
				(ISNULL((SELECT SUM(FileSize) FROM [bx_DiskFiles] f WITH (NOLOCK) WHERE f.DirectoryID = T.DirectoryID), 0))
				,(ISNULL((SELECT COUNT(*) FROM [bx_DiskFiles] f WITH (NOLOCK) WHERE f.DirectoryID = T.DirectoryID), 0))
			FROM @DirectoryIDTable T;
	
		UPDATE [bx_DiskDirectories] SET bx_DiskDirectories.TotalSize = t.TotalSize , TotalFiles = t.TotalFiles FROM @tempTable t WHERE [bx_DiskDirectories].DirectoryID = t.DirectoryID;
	END
END
GO
--删除记录触发器
EXEC bx_Drop 'bx_Doings_AfterDelete';

GO


CREATE TRIGGER [bx_Doings_AfterDelete]
	ON [bx_Doings]
	AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @tempTable table(UserID int,TotalDoings int);

	DELETE [bx_Comments] WHERE TargetID IN (SELECT DoingID FROM [DELETED]) AND Type = 3;

	INSERT INTO @tempTable 
		SELECT DISTINCT T.UserID,
			(ISNULL((SELECT COUNT(*) FROM [bx_Doings] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
		FROM [DELETED] T;

	UPDATE [bx_Users]
		SET
			bx_Users.TotalDoings = T.TotalDoings
		FROM @tempTable T
		WHERE
			T.UserID = bx_Users.UserID;

	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, TotalDoings FROM @tempTable;

END
GO
--添加记录触发器
CREATE TRIGGER [bx_Doings_AfterInsert]
	ON [bx_Doings]
	AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @tempTable table(UserID int,TotalDoings int);

	INSERT INTO @tempTable 
		SELECT DISTINCT T.UserID,
			(ISNULL((SELECT COUNT(*) FROM [bx_Doings] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
		FROM [INSERTED] T;

	UPDATE [bx_Users]
		SET
			bx_Users.TotalDoings = T.TotalDoings
		FROM @tempTable T
		WHERE
			T.UserID = [bx_Users].UserID;

	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, TotalDoings FROM @tempTable;

END
GO
CREATE TRIGGER [bx_Emoticons_After_Delete] ON  [bx_Emoticons] 
FOR DELETE
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @temp Table(GroupID int, TotalSize int, TotalEmoticons int);

	INSERT @temp 
		SELECT DISTINCT GroupID
		,ISNULL((SELECT SUM(FileSize) FROM bx_Emoticons WITH (NOLOCK) WHERE bx_Emoticons.GroupID = t.GroupID), 0)
		,ISNULL((SELECT COUNT(*) FROM bx_Emoticons WITH (NOLOCK) WHERE bx_Emoticons.GroupID = t.GroupID), 0)
	FROM DELETED t;

	UPDATE bx_EmoticonGroups SET TotalSizes = t.TotalSize, TotalEmoticons = t.TotalEmoticons FROM @temp t WHERE bx_EmoticonGroups.GroupID = t.GroupID;

END
GO
CREATE TRIGGER [bx_Emoticons_After_Insert] ON  [bx_Emoticons] 
FOR INSERT 
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @temp Table(GroupID int, TotalSize int, TotalEmoticons int);

	INSERT @temp 
		SELECT DISTINCT GroupID
		,ISNULL((SELECT SUM(FileSize) FROM bx_Emoticons WITH (NOLOCK) WHERE bx_Emoticons.GroupID = t.GroupID), 0)
		,ISNULL((SELECT COUNT(*) FROM bx_Emoticons WITH (NOLOCK) WHERE bx_Emoticons.GroupID = t.GroupID), 0)
	FROM INSERTED t;

	UPDATE bx_EmoticonGroups SET TotalSizes = t.TotalSize, TotalEmoticons = t.TotalEmoticons FROM @temp t WHERE bx_EmoticonGroups.GroupID = t.GroupID;

END
GO
--计算分组总大小和分组表情数
CREATE TRIGGER [bx_Emoticons_After_Update] ON  [bx_Emoticons] 
FOR  UPDATE 
AS
BEGIN

	SET NOCOUNT ON;

	IF UPDATE(GroupID) BEGIN

		DECLARE @GroupTable table(GroupID  int);

		INSERT @GroupTable SELECT DISTINCT GroupID FROM INSERTED;
		INSERT @GroupTable SELECT DISTINCT GroupID FROM DELETED;

		DECLARE @temp Table(GroupID int, TotalSize int, TotalEmoticons int);
		
		INSERT @temp 
		SELECT DISTINCT GroupID
			,ISNULL((SELECT SUM(FileSize) FROM bx_Emoticons WITH (NOLOCK) WHERE bx_Emoticons.GroupID = t.GroupID), 0)
			,ISNULL((SELECT COUNT(*) FROM bx_Emoticons WITH (NOLOCK) WHERE bx_Emoticons.GroupID = t.GroupID), 0)
		FROM @GroupTable  t;

		UPDATE bx_EmoticonGroups SET TotalSizes = t.TotalSize, TotalEmoticons = t.TotalEmoticons FROM @temp t WHERE bx_EmoticonGroups.GroupID = t.GroupID;

	END

END
GO
CREATE TRIGGER bx_Files_AfterDelete  ON [bx_Files]
AFTER DELETE
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO bx_DeletingFiles (ServerFilePath) SELECT ServerFilePath FROM DELETED;
END
GO
EXEC bx_Drop 'bx_Forum_AftertDelete';
GO

CREATE TRIGGER [bx_Forum_AftertDelete] ON [bx_Forums] 
FOR   DELETE 
AS
BEGIN
DELETE FROM bx_BannedUsers WHERE ForumID IN (SELECT ForumID FROM DELETED);
END

GO
GO
--删除好友触发器
CREATE TRIGGER [bx_Friends_AfterDelete]
	ON [bx_Friends]
	AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @tempTable table(UserID int, GroupID int,TotalFriends int);

	INSERT INTO @tempTable 
	SELECT DISTINCT UserID,GroupID,
		(ISNULL((SELECT COUNT(*) FROM [bx_Friends] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.GroupID = T.GroupID), 0))
		FROM [DELETED] T
		WHERE T.GroupID > 0;


	UPDATE [bx_FriendGroups]
		SET
			TotalFriends = T.TotalFriends
		FROM @tempTable T
		WHERE
			T.UserID = bx_FriendGroups.UserID
			AND
			T.GroupID = bx_FriendGroups.GroupID;



	DECLARE @tempTable2 table(UserID int,TotalFriends int);

	INSERT INTO @tempTable2 
	SELECT DISTINCT UserID,
					(ISNULL((SELECT COUNT(*) FROM bx_Friends m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.GroupID >= 0), 0))
		FROM [DELETED] T;
		--WHERE T.GroupID >= 0;


	UPDATE [bx_UserInfos]
		SET
			bx_UserInfos.TotalFriends = T.TotalFriends
		FROM @tempTable2 T
		WHERE
			T.UserID = bx_UserInfos.UserID;

			
	--发出重新填充UserInfo的XCMD命令(已经在bx_UserInfos触发器中统一做)
	--SELECT 'ResetUser' AS XCMD, UserID, TotalFriends FROM @tempTable2;
	
END



GO
--添加好友触发器
CREATE TRIGGER [bx_Friends_AfterInsert]
	ON [bx_Friends]
	AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @tempTable table(UserID int, GroupID int,TotalFriends int);

	INSERT INTO @tempTable 
	SELECT DISTINCT UserID,GroupID,
		(ISNULL((SELECT COUNT(*) FROM [bx_Friends] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.GroupID = T.GroupID), 0))
		FROM [INSERTED] T
		WHERE T.GroupID > 0;


	UPDATE [bx_FriendGroups]
		SET
			TotalFriends = T.TotalFriends
		FROM @tempTable T
		WHERE
			T.UserID = bx_FriendGroups.UserID
			AND
			T.GroupID = bx_FriendGroups.GroupID;



	DECLARE @tempTable2 table(UserID int,TotalFriends int);

	INSERT INTO @tempTable2 
	SELECT DISTINCT UserID,
					(ISNULL((SELECT COUNT(*) FROM bx_Friends m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.GroupID >= 0), 0))
		FROM [INSERTED] T
		WHERE T.GroupID >= 0;


	UPDATE [bx_UserInfos]
		SET
			bx_UserInfos.TotalFriends = T.TotalFriends
		FROM @tempTable2 T
		WHERE
			T.UserID = bx_UserInfos.UserID;

			
	--发出重新填充UserInfo的XCMD命令(已经在bx_UserInfos触发器中统一做)
	--SELECT 'ResetUser' AS XCMD, UserID, TotalFriends FROM @tempTable2;

END
GO
--添加好友触发器
CREATE TRIGGER [bx_Friends_AfterUpdate]
	ON [bx_Friends]
	AFTER UPDATE
AS
BEGIN

	SET NOCOUNT ON;

	IF UPDATE([GroupID]) BEGIN

		DECLARE @tempTable table(UserID int, GroupID int,TotalFriends int);

		INSERT INTO @tempTable
		SELECT DISTINCT UserID,GroupID,
			(ISNULL((SELECT COUNT(*) FROM [bx_Friends] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.GroupID = T.GroupID), 0))
			FROM [INSERTED] T WHERE T.GroupID > 0;

		INSERT INTO @tempTable
		SELECT DISTINCT UserID,GroupID,
			(ISNULL((SELECT COUNT(*) FROM [bx_Friends] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.GroupID = T.GroupID), 0))
			FROM [DELETED] T
			WHERE T.GroupID > 0 AND T.GroupID NOT IN (SELECT GroupID FROM @tempTable);



		UPDATE [bx_FriendGroups]
			SET
				TotalFriends = T.TotalFriends
			FROM @tempTable T
			WHERE
				T.UserID = bx_FriendGroups.UserID
				AND
				T.GroupID = bx_FriendGroups.GroupID;


		DECLARE @tempTable2 table(UserID int,TotalFriends int);

			INSERT INTO @tempTable2 
			SELECT DISTINCT UserID,
							(ISNULL((SELECT COUNT(*) FROM bx_Friends m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.GroupID >= 0), 0))
				FROM [DELETED] T
				WHERE T.GroupID >= 0;


			UPDATE [bx_UserInfos]
				SET
					bx_UserInfos.TotalFriends = T.TotalFriends
				FROM @tempTable2 T
				WHERE
					T.UserID = bx_UserInfos.UserID;

					
			--发出重新填充UserInfo的XCMD命令(已经在bx_UserInfos触发器中统一做)
			--SELECT 'ResetUser' AS XCMD, UserID, TotalFriends FROM @tempTable2;

	END

END
GO
EXEC bx_Drop 'bx_ImpressionRecords_AfterDelete';

GO

CREATE TRIGGER [bx_ImpressionRecords_AfterDelete]
ON [bx_ImpressionRecords] AFTER DELETE
AS
BEGIN

        UPDATE bx_Impressions SET [Count] = [Count] - 1 FROM [DELETED] WHERE bx_Impressions.UserID = [DELETED].TargetUserID AND bx_Impressions.TypeID = [DELETED].TypeID;
     
		UPDATE bx_ImpressionTypes SET RecordCount = RecordCount - 1 FROM [DELETED] WHERE bx_ImpressionTypes.TypeID = [DELETED].TypeID;
END
GO
EXEC bx_Drop 'bx_ImpressionRecords_AfterInsert';

GO

CREATE TRIGGER [bx_ImpressionRecords_AfterInsert]
ON [bx_ImpressionRecords] AFTER INSERT
AS
BEGIN

        UPDATE bx_Impressions SET [Count] = [Count] + 1, [UpdateDate] = GETDATE() FROM [INSERTED] WHERE bx_Impressions.UserID = [INSERTED].TargetUserID AND bx_Impressions.TypeID = [INSERTED].TypeID;
        
		UPDATE bx_ImpressionTypes SET RecordCount = RecordCount + 1 FROM [INSERTED] WHERE bx_ImpressionTypes.TypeID = [INSERTED].TypeID;
		
		UPDATE bx_UserVars SET [LastImpressionDate] = GETDATE() FROM [INSERTED] WHERE bx_UserVars.UserID = [INSERTED].TargetUserID;
END
GO
CREATE TRIGGER bx_Notify_After_Delete ON [bx_Notify] 
FOR DELETE 
AS
BEGIN


DECLARE @UserID int;
DECLARE @TypeID int;
DECLARE @Count int;
DECLARE @RowCount int;
DECLARE @i int;

SET @i=0;
DECLARE @temp TABLE( RowID int  IDENTITY(1,1),  UserID int,TypeID int);

INSERT @temp( UserID , TypeID )  SELECT DISTINCT UserID , TypeID FROM DELETED;

SET @RowCount = @@ROWCOUNT;

WHILE  @i<@RowCount
BEGIN

SELECT @UserID = UserID ,@TypeID = TypeID FROM @temp WHERE RowID = @i + 1;

SET @Count = ( SELECT COUNT(*) FROM bx_Notify WHERE UserID = @UserID AND TypeID = @TypeID  AND IsRead = 0 );

UPDATE bx_UnreadNotifies SET UnreadCount = @Count WHERE UserID = @UserID AND TypeID = @TypeID;

IF @@ROWCOUNT =0 AND  @Count>0  BEGIN
	INSERT INTO bx_UnreadNotifies( UserID , TypeID , UnreadCount ) VALUES(@UserID , @TypeID , @Count);
END 

SET @i = @i + 1;
END
END
GO
CREATE TRIGGER bx_Notify_After_Insert  ON [bx_Notify] 
FOR INSERT 
AS
BEGIN


DECLARE @UserID int;
DECLARE @TypeID int;
DECLARE @Count int;

SELECT @UserID = UserID ,@TypeID = TypeID FROM INSERTED;

SET @Count = ( SELECT COUNT(*) FROM bx_Notify WHERE UserID = @UserID AND TypeID = @TypeID AND IsRead = 0 );

UPDATE bx_UnreadNotifies SET UnreadCount = @Count WHERE UserID = @UserID AND TypeID = @TypeID;

IF @@ROWCOUNT =0 BEGIN
	INSERT INTO bx_UnreadNotifies( UserID , TypeID , UnreadCount ) VALUES(@UserID , @TypeID , @Count);
END 

END

GO
CREATE TRIGGER bx_Notify_After_Update ON [bx_Notify] 
FOR UPDATE 
AS
BEGIN

IF UPDATE(IsRead) BEGIN
	DECLARE @UserID int;
	DECLARE @TypeID int;
	DECLARE @Count int;
	DECLARE @RowCount int;
	DECLARE @i int;

	SET @i=0;
	DECLARE @temp TABLE( RowID int  IDENTITY(1,1),  UserID int,TypeID int);

	INSERT @temp( UserID , TypeID )  SELECT DISTINCT UserID , TypeID FROM DELETED;

	SET @RowCount = @@ROWCOUNT;

	WHILE  @i<@RowCount
	BEGIN

	SELECT @UserID = UserID ,@TypeID = TypeID FROM @temp WHERE RowID = @i + 1;

	SET @Count = ( SELECT COUNT(*) FROM bx_Notify WHERE UserID = @UserID AND TypeID = @TypeID  AND IsRead = 0 );

	UPDATE bx_UnreadNotifies SET UnreadCount = @Count WHERE UserID = @UserID AND TypeID = @TypeID;

	IF @@ROWCOUNT =0 BEGIN
		INSERT INTO bx_UnreadNotifies( UserID , TypeID , UnreadCount ) VALUES(@UserID , @TypeID , @Count);
	END 

	SET @i = @i + 1;
	END
END
END
GO
--删除相片的触发器
EXEC bx_Drop 'bx_Photos_AfterDelete';

GO

CREATE TRIGGER [bx_Photos_AfterDelete] ON [bx_Photos] AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;
	
	DELETE bx_Denouncings WHERE Type=1 AND TargetID IN (SELECT PhotoID FROM [DELETED]);
	
	DELETE bx_Files
		WHERE FileID IN (SELECT FileID FROM [DELETED])
			AND FileID NOT IN (SELECT FileID FROM [bx_UsedFileIds] WHERE FileID IN (SELECT FileID FROM [DELETED]))
	
	--更新用户已用相册空间
	
	DECLARE @TempTable1 table (UserID int, UsedAlbumSize bigint, TotalPhotos int);
	
	INSERT INTO @TempTable1 (UserID, UsedAlbumSize, TotalPhotos) 
	SELECT 
		DISTINCT T.UserID, 
		ISNULL((SELECT SUM(FileSize) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.[UserID] = T.[UserID]), 0),
		ISNULL((SELECT COUNT(*) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.[UserID] = T.[UserID]), 0)
	FROM [DELETED] T;
	
	UPDATE [bx_UserVars] SET [bx_UserVars].[UsedAlbumSize] = T.[UsedAlbumSize]
	FROM @TempTable1 AS T 
	WHERE [bx_UserVars].[UserID] = T.[UserID];
	
	UPDATE [bx_Users] SET [bx_Users].[TotalPhotos] = T.[TotalPhotos]
	FROM @TempTable1 T
	WHERE T.[UserID] = [bx_Users].[UserID];

	SELECT 'ResetUser' AS XCMD, UserID, TotalPhotos, UsedAlbumSize FROM @TempTable1;

	--更新相册总图片数
	
	DECLARE @TempTable2 table(AlbumID int, TotalPhotos int, UpdateDate datetime DEFAULT(GETDATE()));
	
	INSERT INTO @TempTable2 (AlbumID, TotalPhotos)
	SELECT 
		DISTINCT [AlbumID], 
		ISNULL((SELECT COUNT(*) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.[AlbumID] = T.[AlbumID]), 0)
	FROM [DELETED] T;
	
	UPDATE [bx_Albums] SET [bx_Albums].[TotalPhotos] = T.[TotalPhotos], [bx_Albums].[UpdateDate] = T.UpdateDate
	FROM @TempTable2 T
	WHERE [bx_Albums].[AlbumID] = T.[AlbumID];

	--触发XCMD

	--SELECT 'ResetAlbum' AS XCMD, AlbumID, TotalPhotos, UpdateDate FROM @TempTable2;

END



GO
--创建新相片的触发器
EXEC bx_Drop 'bx_Photos_AfterInsert';

GO

CREATE TRIGGER [bx_Photos_AfterInsert] ON [bx_Photos] AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;
	
	--更新用户已用相册空间
	
	DECLARE @TempTable1 table (UserID int, UsedAlbumSize bigint, TotalPhotos int);
	
	INSERT INTO @TempTable1 (UserID, UsedAlbumSize, TotalPhotos) 
	SELECT 
		DISTINCT T.UserID, 
		ISNULL((SELECT SUM(FileSize) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.[UserID] = T.[UserID]), 0),
		ISNULL((SELECT COUNT(*) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.[UserID] = T.[UserID]), 0)
	FROM [INSERTED] T;
	
	UPDATE [bx_UserVars] SET [bx_UserVars].[UsedAlbumSize] = T.[UsedAlbumSize] 
	FROM @TempTable1 AS T 
	WHERE [bx_UserVars].[UserID] = T.[UserID];
	
	UPDATE [bx_Users] SET [bx_Users].[TotalPhotos] = T.[TotalPhotos]
	FROM @TempTable1 T
	WHERE T.[UserID] = [bx_Users].[UserID];
	
	SELECT 'ResetUser' AS XCMD, UserID, TotalPhotos, UsedAlbumSize FROM @TempTable1;

	--更新相册总图片数
	
	DECLARE @TempTable2 table(AlbumID int, TotalPhotos int, UpdateDate datetime DEFAULT(GETDATE()));
	
	INSERT INTO @TempTable2 (AlbumID, TotalPhotos)
	SELECT 
		DISTINCT T.[AlbumID], 
		ISNULL((SELECT COUNT(*) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.[AlbumID] = T.[AlbumID]), 0)
	FROM [INSERTED] T;
	
	UPDATE [bx_Albums] SET [bx_Albums].[TotalPhotos] = T.TotalPhotos, [bx_Albums].[UpdateDate] = T.UpdateDate
	FROM @TempTable2 T
	WHERE [bx_Albums].[AlbumID] = T.[AlbumID];

	--返回结果
	
	SELECT [PhotoID] FROM [INSERTED] ORDER BY [PhotoID] ASC;

	--SELECT 'ResetAlbum' AS XCMD, AlbumID, TotalPhotos FROM @TempTable2;
END



GO
--创建新相片的触发器
EXEC bx_Drop 'bx_Photos_AfterUpdate';

GO

CREATE TRIGGER [bx_Photos_AfterUpdate]
	ON [bx_Photos]
	AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	--如果UserID字段被更改，需要重新统计受影响的用户的TotalPhotos值
	IF UPDATE([UserID]) BEGIN
		DECLARE @tempTable2 table(UserID int, UsedAlbumSize bigint, TotalPhotos int);
		INSERT INTO @tempTable2 (UserID, UsedAlbumSize, TotalPhotos)
			SELECT DISTINCT UserID,
				ISNULL((SELECT SUM(FileSize) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.UserID = T.[UserID]), 0),
				ISNULL((SELECT COUNT(*) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.UserID = T.UserID), 0)
			FROM [INSERTED] T;
		INSERT INTO @tempTable2 (UserID, UsedAlbumSize, TotalPhotos)
			SELECT DISTINCT UserID,
				ISNULL((SELECT SUM(FileSize) FROM [bx_Photos] p WITH (NOLOCK) WHERE p.[UserID] = T.[UserID]), 0),
				ISNULL((SELECT COUNT(*) FROM [bx_Photos] p WITH (NOLOCK) WHERE p.UserID = T.UserID), 0)
			FROM [DELETED] T
			WHERE UserID NOT IN (SELECT UserID FROM @tempTable2);
		
		UPDATE [bx_Users]
			SET
				bx_Users.TotalPhotos = T.TotalPhotos
			FROM @tempTable2 T
			WHERE
				T.UserID = bx_Users.UserID;
		UPDATE [bx_UserVars]
			SET
				bx_UserVars.UsedAlbumSize = T.UsedAlbumSize
			FROM @tempTable2 T
			WHERE
				T.UserID = bx_UserVars.UserID;
		--发出重新填充UserInfo的XCMD命令
		SELECT 'ResetUser' AS XCMD, UserID, TotalPhotos, UsedAlbumSize FROM @tempTable2;

	END


	--如果AlbumID字段被更改，需要重新统计受影响的相册的TotalPhotos值
	IF UPDATE([AlbumID]) BEGIN
		DECLARE @tempTable table(AlbumID int, TotalPhotos int, UpdateDate datetime DEFAULT(GETDATE()));

		INSERT INTO @tempTable (AlbumID, TotalPhotos)
			SELECT DISTINCT AlbumID,
				ISNULL((SELECT COUNT(*) FROM [bx_Photos] as m WITH (NOLOCK) WHERE m.AlbumID = T.AlbumID), 0)
			FROM [INSERTED] T;

		INSERT INTO @tempTable (AlbumID, TotalPhotos)
			SELECT DISTINCT AlbumID,
				ISNULL((SELECT COUNT(*) FROM [bx_Photos] as m WITH (NOLOCK) WHERE m.AlbumID = T.AlbumID), 0)
			FROM [DELETED] T
			WHERE AlbumID NOT IN (SELECT AlbumID FROM @tempTable);
		
		UPDATE [bx_Albums]
			SET
				[bx_Albums].TotalPhotos = T.TotalPhotos,
				[bx_Albums].[UpdateDate] = T.UpdateDate
			FROM @tempTable T
			WHERE
				T.AlbumID = [bx_Albums].AlbumID;
		--SELECT 'ResetAlbum' AS XCMD, AlbumID, TotalPhotos, UpdateDate FROM @TempTable2;
	END
END

GO
GO
--
EXEC bx_Drop 'bx_PointShows_After_Update';

GO

CREATE TRIGGER  bx_PointShows_After_Update  ON [bx_PointShows] 
FOR INSERT, UPDATE 
AS
BEGIN

DECLARE @UserIDs  table(UserID  int)

INSERT @UserIDs SELECT UserID FROM INSERTED;

UPDATE bx_PointShows SET Price = ShowPoints WHERE UserID IN( SELECT UserID FROM @UserIDs ) AND  Price>ShowPoints 

DELETE FROM bx_PointShows WHERE  UserID IN( SELECT UserID FROM @UserIDs ) AND  Price=0;

END

GO
--发表新日志的触发器
EXEC bx_Drop 'bx_Posts_AfterDelete';
GO

CREATE TRIGGER [bx_Posts_AfterDelete] 
   ON  [bx_Posts] 
   AFTER DELETE
AS 
BEGIN
	SET NOCOUNT ON;
	DECLARE @tempTable TABLE(ForumID INT,PostCount INT,ReduceTodayPosts INT)--,LastThreadID INT)

	DECLARE @Today DateTime,@Monday DateTime;
	SET @Today = CONVERT(varchar(12) , getdate(), 102);
	
	DECLARE @m int;
	SELECT @m = DATEPART(weekday, @Today);
	IF @m = 1
		SELECT @m = 8;
	SELECT @Monday = DATEADD(day, 2-@m, @Today);

	--IF NOT EXISTS (SELECT TOP 1 * FROM bx_Posts WHERE ThreadID IN (SELECT DISTINCT ThreadID FROM deleted))
		--RETURN;

	insert into @tempTable(ForumID,PostCount,ReduceTodayPosts)--,LastThreadID)
	select distinct D.ForumID,
				(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
				,(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND D1.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=D.ForumID))
				--,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=D.ForumID AND SortOrder<2000000000000000))
				--,(SELECT ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WITH (NOLOCK) WHERE T1.ForumID = D.ForumID))
				FROM [deleted] D --Inner join [bx_Threads] T on D.ThreadID=T.ThreadID WHERE D.IsApproved=1

	----如果没有，说明这是对直接删除整个主题，可以直接终止 
	--IF NOT EXISTS (SELECT * FROM @tempTable)
			--RETURN;

	UPDATE bx_Forums SET 
					TotalPosts=TotalPosts-t.PostCount
					,TodayPosts=TodayPosts-t.ReduceTodayPosts
					--,LastThreadID=ISNULL(t.LastThreadID,0)
					from @tempTable as t WHERE bx_Forums.ForumID=t.ForumID


	
	 
	SELECT 'RecodeTodayPosts' AS XCMD, 'delete' as AC, T.ForumID AS ForumID,T.ReduceTodayPosts AS ReducePosts,F.TodayPosts,F.CodeName FROM @tempTable AS T INNER JOIN bx_Forums as F on T.ForumID = F.ForumID WHERE T.ReduceTodayPosts>0;



	DECLARE @tempTable2 TABLE(ThreadID INT,PostCount INT,LastPostUserID INT,LastPostUserNickName nvarchar(64))
	insert into @tempTable2(ThreadID,PostCount,LastPostUserID,LastPostUserNickName)
	select distinct D.ThreadID,
				--(SELECT COUNT(1) FROM [deleted] D1  Inner join [bx_Threads] T1 on D1.ThreadID=T1.ThreadID WHERE D1.IsApproved=1 AND T1.ThreadID=T.ThreadID)
				(SELECT COUNT(1) FROM [bx_Posts] P WITH(NOLOCK) WHERE P.ThreadID=D.ThreadID AND P.SortOrder<4000000000000000)
				,(SELECT top 1 UserID FROM [bx_Posts] where ThreadID=D.ThreadID  AND SortOrder<4000000000000000 Order by SortOrder desc)
				,(SELECT top 1 NickName FROM [bx_Posts] where ThreadID=D.ThreadID AND SortOrder<4000000000000000 Order by SortOrder desc)
				FROM [deleted] D --WHERE D.ForumID>0-- Inner join [bx_Threads] T on D.ThreadID=T.ThreadID

	UPDATE bx_Threads SET 
					TotalReplies=t.PostCount-1,--TotalReplies-t.PostCount,
					LastPostUserID=ISNULL(t.LastPostUserID,0),
					LastPostNickName=ISNULL(t.LastPostUserNickName,'') 
					from @tempTable2 as t WHERE bx_Threads.ThreadID=t.ThreadID


	DECLARE @FirstPostSortOrder TABLE(SortOrder BIGINT)
	INSERT INTO @FirstPostSortOrder(SortOrder)
	SELECT MIN(P.SortOrder) FROM [deleted] I INNER JOIN [bx_Posts] P ON I.ThreadID=P.ThreadID  GROUP BY I.ThreadID 

	IF EXISTS(SELECT * FROM [deleted] WHERE SortOrder IN(SELECT SortOrder FROM @FirstPostSortOrder)) BEGIN
		--这表示是删除主题时触发的，这时候只要重新统计主题用户的DeletedPosts，其他用户不重新统计--
		
		DECLARE @tempTable3 TABLE(userID INT,postCount INT,weekPostCount INT,dayPostCount INT, monthPostCount INT)
		insert into @tempTable3(userID,postCount,weekPostCount,dayPostCount,monthPostCount)
		select distinct UserID
						,(SELECT COUNT(*) FROM [deleted] POST /* INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */ WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND CreateDate>=@Monday)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND CreateDate>=@Today)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND DATEPART(year, GETDATE()) = DATEPART(year,CreateDate) AND DATEPART(month, GETDATE()) = DATEPART(month,CreateDate))
				FROM [deleted] p /* inner join [bx_Threads] t on p.ThreadID=t.ThreadID */

		UPDATE bx_Users SET
				TotalPosts=TotalPosts-t.postCount
				,WeekPosts = WeekPosts - t.weekPostCount
				,DayPosts = DayPosts - t.dayPostCount
				,MonthPosts = MonthPosts - t.monthPostCount
			FROM @tempTable3 as t where bx_Users.UserID = t.userID
		
		UPDATE bx_Users SET
				DeletedReplies=DeletedReplies+t.postCount
			FROM @tempTable3 as t where bx_Users.UserID = t.userID AND t.userID IN(SELECT DISTINCT UserID FROM [deleted] WHERE SortOrder IN(SELECT SortOrder FROM @FirstPostSortOrder))
	END 
	ELSE BEGIN
		--这表示是删除回复时触发的--
		DECLARE @tempTable4 TABLE(userID INT,postCount INT,weekPostCount INT,dayPostCount INT, monthPostCount INT)
		insert into @tempTable4(userID,postCount,weekPostCount,dayPostCount,monthPostCount)
		select distinct UserID
						,(SELECT COUNT(*) FROM [deleted] POST /* INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */ WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND CreateDate>=@Monday)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND CreateDate>=@Today)
						,(SELECT COUNT(*) FROM [deleted] POST WHERE  POST.SortOrder<4000000000000000 AND POST.UserID=p.UserID AND DATEPART(year, GETDATE()) = DATEPART(year,CreateDate) AND DATEPART(month, GETDATE()) = DATEPART(month,CreateDate))
				FROM [deleted] p /* inner join [bx_Threads] t on p.ThreadID=t.ThreadID */

		UPDATE bx_Users SET
				TotalPosts=TotalPosts-t.postCount
				,DeletedReplies=DeletedReplies+t.postCount
				,WeekPosts = WeekPosts - t.weekPostCount
				,DayPosts = DayPosts - t.dayPostCount
				,MonthPosts = MonthPosts - t.monthPostCount
			FROM @tempTable4 as t where bx_Users.UserID = t.userID
	END
	
	DELETE bx_Denouncings WHERE Type=7 AND TargetID IN (SELECT PostID FROM [DELETED]);
	
END




GO
--发表新日志的触发器
EXEC bx_Drop 'bx_Posts_AfterUpdate';
GO

CREATE TRIGGER [bx_Posts_AfterUpdate] 
   ON  [bx_Posts] 
   AFTER UPDATE
AS 
BEGIN

	SET NOCOUNT ON;
	IF UPDATE(ForumID) BEGIN
			--如果是合并版面导致的移动主题，就忽略下面的数据统计，直接结束
		DECLARE @ForumIDs nvarchar(64)
		EXEC bx_GetDisabledTriggerForumIDs
				@ForumIDs output
		IF(@ForumIDs<>'') BEGIN
			DECLARE @ForumID int,@ForumIDString nvarchar(64)
			SELECT top 1 @ForumID=ForumID FROM [deleted]
			SET @ForumIDString=Replace(','+STR(@ForumID)+',', ' ', '')
			SET @ForumIDs=','+@ForumIDs+','
			IF(CHARINDEX(@ForumIDString,@ForumIDs)>0) BEGIN
				RETURN;
			END
		END
		--DECLARE @tempForumID1 int,@tempForumID2 int
		--SELECT @tempForumID1 = TOP 1 ForumID FROM [deleted]
		--SELECT @tempForumID2 = TOP 1 ForumID FROM [inserted]
		--IF @tempForumID1 <> -2 AND @tempForumID2<>-2 --说明不是审核相关 退出
			--RETURN
			
		--更新之前的版快
		DECLARE @tempTable TABLE(ForumID INT,PostCount1 INT,PostCount2 INT,TodayPosts1 INT,TodayPosts2 INT)--,LastThreadID INT)--,PostCount2 INT)
		insert into @tempTable(ForumID,PostCount1,PostCount2,TodayPosts1,TodayPosts2)--,LastThreadID)--,PostCount2)
		select distinct ForumID,
			(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [deleted] D1 WHERE  D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND DATEDIFF(day, D1.CreateDate, getdate())=0)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND DATEDIFF(day, D1.CreateDate, getdate())=0)
			--,(SELECT ThreadID FROM [bx_Threads] T WHERE T.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2 WITH(NOLOCK) WHERE T2.ForumID=D.ForumID AND SortOrder<2000000000000000))
			FROM [deleted] D --Inner join [bx_Threads] T  on D.ThreadID=T.ThreadID 

		UPDATE bx_Forums SET 
					TotalPosts=TotalPosts+(t.PostCount2-t.PostCount1)
					,TodayPosts=TodayPosts+(t.TodayPosts2-t.TodayPosts1)
					--,LastThreadID=ISNULL(t.LastThreadID,0)
					from @tempTable as t WHERE bx_Forums.ForumID=t.ForumID

		


		---
		DECLARE @tempTable0 TABLE(ForumID INT,PostCount1 INT,PostCount2 INT,TodayPosts1 INT,TodayPosts2 INT)--,LastThreadID INT)--,PostCount2 INT)
		insert into @tempTable0(ForumID,PostCount1,PostCount2,TodayPosts1,TodayPosts2)--,LastThreadID)--,PostCount2)
		select distinct ForumID,
			(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [deleted] D1 WHERE  D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND DATEDIFF(day, D1.CreateDate, getdate())=0)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE  D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND  DATEDIFF(day, D1.CreateDate, getdate())=0)
			--,(SELECT ThreadID FROM [bx_Threads] T WHERE T.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=D.ForumID AND SortOrder<2000000000000000))
			FROM [inserted] D 
			
		UPDATE bx_Forums SET 
					TotalPosts=TotalPosts+(t.PostCount2-t.PostCount1)
					,TodayPosts=TodayPosts+(t.TodayPosts2-t.TodayPosts1)
					--,LastThreadID=ISNULL(t.LastThreadID,0)
					from @tempTable0 as t WHERE bx_Forums.ForumID=t.ForumID	
			
		
		
		SELECT 'RecodeTodayPosts' AS XCMD,'updateForumID' as AC,T.ForumID as ForumID,(T.TodayPosts1-T.TodayPosts2) as ReducePosts,F.TodayPosts,F.CodeName FROM @tempTable AS T inner join bx_Forums as F on T.ForumID = F.ForumID WHERE T.TodayPosts1>T.TodayPosts2
		UNION ALL 
		SELECT 'RecodeTodayPosts' AS XCMD,'updateForumID' as AC,T.ForumID as ForumID,(T.TodayPosts1-T.TodayPosts2) as ReducePosts,F.TodayPosts,F.CodeName FROM @tempTable0 AS T inner join bx_Forums as F on T.ForumID = F.ForumID WHERE T.TodayPosts1>T.TodayPosts2
		;

	END
	
	IF UPDATE(SortOrder) BEGIN
	
	
		DECLARE @tempTable4 TABLE(ForumID INT,PostCount1 INT,PostCount2 INT,TodayPosts1 INT,TodayPosts2 INT)--,LastThreadID INT)--,PostCount2 INT)
		insert into @tempTable4(ForumID,PostCount1,PostCount2,TodayPosts1,TodayPosts2)--,LastThreadID)--,PostCount2)
		select distinct ForumID,
			(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000)
			,(SELECT COUNT(1) FROM [deleted] D1 WHERE  D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND DATEDIFF(day, D1.CreateDate, getdate())=0)
			,(SELECT COUNT(1) FROM [inserted] D1 WHERE  D1.ForumID=D.ForumID AND D1.SortOrder<4000000000000000 AND  DATEDIFF(day, D1.CreateDate, getdate())=0)
			--,(SELECT ThreadID FROM [bx_Threads] T WHERE T.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=D.ForumID AND SortOrder<2000000000000000))-- 更新最后回复的主题（此处用[bx_Threads]不准确（忽略），只是用于当主题被回收被还原等操作时更新）
			FROM [inserted] D 
			
		UPDATE bx_Forums SET 
					TotalPosts=TotalPosts+(t.PostCount2-t.PostCount1)
					,TodayPosts=TodayPosts+(t.TodayPosts2-t.TodayPosts1)
					--,LastThreadID=ISNULL(t.LastThreadID,0)
					from @tempTable4 as t WHERE bx_Forums.ForumID=t.ForumID	
	
	
		SELECT 'RecodeTodayPosts' AS XCMD,'updateSortOrder' as AC,T.ForumID as ForumID,(T.TodayPosts1-T.TodayPosts2) as ReducePosts,F.TodayPosts,F.CodeName FROM @tempTable4 AS T inner join bx_Forums as F on T.ForumID = F.ForumID WHERE T.TodayPosts1>T.TodayPosts2


	
		DECLARE @FirstPostSortOrder TABLE(SortOrder BIGINT)
		INSERT INTO @FirstPostSortOrder(SortOrder)
		SELECT MIN(P.SortOrder) FROM [inserted] I INNER JOIN [bx_Posts] P ON I.ThreadID=P.ThreadID  GROUP BY I.ThreadID 
		
	
	
		DECLARE @tempTable2 TABLE(ThreadID INT,PostCount INT,LastPostUserID INT,LastPostUserNickName nvarchar(64))
		insert into @tempTable2(ThreadID,PostCount,LastPostUserID,LastPostUserNickName)
		select distinct D.ThreadID,
					(SELECT COUNT(1) FROM [bx_Posts] t1 where /*t1.ForumID = D.ForumID AND*/ t1.ThreadID=D.ThreadID AND t1.SortOrder<5000000000000000) --(此处用5000000000000000是因为回收站的里的主题也统计回复数，而回收站里的主题的回复的SortOrder是大于4000000000000000，而正常的主题回复的SortOrder是不可能有大于4000000000000000小于5000000000000000的）
					--(SELECT COUNT(1) FROM [bx_Posts] p1 inner join [bx_Threads] t1 on p1.ThreadID=t1.ThreadID where t1.ThreadID=T.ThreadID AND p1.IsApproved=1) 
--					(SELECT COUNT(1) FROM [deleted] D1  Inner join [bx_Threads] T1  on D1.ThreadID=T1.ThreadID WHERE T1.ThreadID=T.ThreadID AND ThreadLocation=0 AND IsApproved=1)
--					,(SELECT COUNT(1) FROM [inserted] D1  Inner join [bx_Threads] T1  on D1.ThreadID=T1.ThreadID WHERE T1.ThreadID=T.ThreadID AND ThreadLocation=0 AND IsApproved=1)
					,(SELECT top 1 UserID FROM [bx_Posts]  where /*ForumID = D.ForumID AND*/ ThreadID=D.ThreadID AND SortOrder<5000000000000000 Order by SortOrder desc)
					,(SELECT top 1 NickName FROM [bx_Posts]  where /*ForumID = D.ForumID AND*/ ThreadID=D.ThreadID AND SortOrder<5000000000000000 Order by SortOrder desc)
					FROM [inserted] D WHERE D.ThreadID NOT IN(SELECT ThreadID FROM [inserted] WHERE SortOrder IN(SELECT SortOrder FROM @FirstPostSortOrder)) --Inner join [bx_Threads] T  on D.ThreadID=T.ThreadID --AND D.IsApproved=1
											--WHERE后面 表示如果是主题被回收，还原，等操作则不修改主题的相关信息--
		UPDATE bx_Threads SET 
						TotalReplies=t.PostCount-1,
						LastPostUserID=ISNULL(t.LastPostUserID,0),
						LastPostNickName=ISNULL(t.LastPostUserNickName,'') 
						from @tempTable2 as t WHERE bx_Threads.ThreadID=t.ThreadID 


		--IF EXISTS(SELECT * FROM [deleted] WHERE SortOrder IN(SELECT SortOrder FROM @FirstPostSortOrder)) BEGIN
			--这表示是回收主题时触发的，这时候只要重新统计主题用户的DeletedPosts，其他用户不重新统计--
			
			DECLARE @Today DateTime,@Monday DateTime;
			SET @Today = CONVERT(varchar(12) , getdate(), 102);
			
			DECLARE @m int;
			SELECT @m = DATEPART(weekday, @Today);
			IF @m = 1
				SELECT @m = 8;
			SELECT @Monday = DATEADD(day, 2-@m, @Today);
			
			-- 回收主题，还原主题，审核主题 审核回复 --
			DECLARE @tempTable3 TABLE(userID INT,postCount INT,postCount2 INT,weekPostCount INT,weekPostCount2 INT,dayPostCount INT,dayPostCount2 INT,monthPostCount INT,monthPostCount2 INT,TempLastPostDate DateTime)
			insert into @tempTable3(userID,postCount,postCount2,weekPostCount,weekPostCount2,dayPostCount,dayPostCount2,monthPostCount,monthPostCount2,TempLastPostDate)
			select distinct UserID
								,(SELECT COUNT(*) FROM [deleted] POST /*INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000) 
								,(SELECT COUNT(*) FROM [inserted] POST /*INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000) 
								
								,(SELECT COUNT(*) FROM [deleted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND CreateDate>=@Monday) 
								,(SELECT COUNT(*) FROM [inserted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND CreateDate>=@Monday) 
								
								,(SELECT COUNT(*) FROM [deleted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND CreateDate>=@Today) 
								,(SELECT COUNT(*) FROM [inserted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND CreateDate>=@Today) 
								
								,(SELECT COUNT(*) FROM [deleted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND DATEPART(year, GETDATE()) = DATEPART(year,CreateDate) AND DATEPART(month, GETDATE()) = DATEPART(month,CreateDate)) 
								,(SELECT COUNT(*) FROM [inserted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000 AND DATEPART(year, GETDATE()) = DATEPART(year,CreateDate) AND DATEPART(month, GETDATE()) = DATEPART(month,CreateDate)) 

								,(SELECT MAX(CreateDate) FROM [inserted] POST WHERE POST.UserID=p.UserID AND POST.SortOrder<4000000000000000) 
					FROM [deleted] p  

			UPDATE bx_Users SET
					 TotalPosts=TotalPosts+(t.postCount2-t.postCount)
					,WeekPosts=WeekPosts+(t.weekPostCount2-t.weekPostCount)
					,DayPosts=DayPosts+(t.dayPostCount2-t.dayPostCount)
					,MonthPosts = MonthPosts +(t.monthPostCount2 - t.monthPostCount)
					FROM @tempTable3 as t where bx_Users.UserID=t.userID
			
			UPDATE bx_Users SET
					 LastPostDate = TempLastPostDate
					 FROM @tempTable3 as t WHERE bx_Users.UserID=t.userID AND TempLastPostDate IS NOT NULL AND bx_Users.LastPostDate<TempLastPostDate; 

			UPDATE bx_Users SET
					DeletedReplies=DeletedReplies+t.postCount
					FROM @tempTable3 as t where bx_Users.UserID=t.userID  AND t.userID IN(SELECT DISTINCT UserID FROM [deleted] WHERE SortOrder IN(SELECT SortOrder FROM @FirstPostSortOrder))
		--END 
		--ELSE BEGIN
			----这表示是回收回复时触发的--
			--DECLARE @tempTable5 TABLE(userID INT,postCount INT,postCount2 INT) 
			--insert into @tempTable5(userID,postCount,postCount2)
			--select distinct UserID
								--,(SELECT COUNT(1) FROM [deleted] POST /*INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */WHERE Post.UserID=p.UserID AND POST.SortOrder<4000000000000000) 
								--,(SELECT COUNT(1) FROM [inserted] POST /*INNER JOIN [bx_Threads] THREAD ON POST.ThreadID=THREAD.ThreadID */WHERE Post.UserID=p.UserID AND POST.SortOrder<4000000000000000) 
					--FROM [deleted] p  

			--UPDATE bx_Users SET
					--TotalPosts=TotalPosts+(t.PostCount2-t.PostCount),
					--DeletedPosts=DeletedPosts+t.PostCount
					--FROM @tempTable5 as t where bx_Users.UserID=t.userID
		--END
	END
END

GO
EXEC bx_Drop 'bx_UsersInRoles_AfterDelete';

GO

CREATE TRIGGER [bx_UsersInRoles_AfterDelete] ON [bx_UsersInRoles] AFTER DELETE

AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(RoleID int,TotalUser int);
	
	INSERT INTO @tempTable
		SELECT DISTINCT RoleID,
			ISNULL((SELECT COUNT(*) FROM [bx_UsersInRoles] AS r WITH (NOLOCK) WHERE r.RoleID=T.RoleID),0)
		FROM [DELETED] T;
		
	UPDATE [bx_Roles]
		SET
			bx_Roles.UserCount=T.TotalUser
		FROM @tempTable T
		WHERE
			T.RoleID=bx_Roles.RoleID;
			
END

GO
GO
EXEC bx_Drop 'bx_UsersInRoles_AfterInsert';

GO

CREATE TRIGGER [bx_UsersInRoles_AfterInsert] ON [bx_UsersInRoles] AFTER INSERT

AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(RoleID int,TotalUser int)
	
	INSERT INTO @tempTable
		SELECT DISTINCT RoleID,
			ISNULL((SELECT COUNT(*) FROM [bx_UsersInRoles] as r WITH (NOLOCK) WHERE r.RoleID=T.RoleID),0)
		FROM [INSERTED]	T;
		
	UPDATE [bx_Roles]
		SET
			bx_Roles.UserCount=T.TotalUser
		FROM @tempTable	 T
		WHERE
			T.RoleID=bx_Roles.RoleID;
END			
GO
EXEC bx_Drop 'bx_UsersInRoles_After_Update';

GO

CREATE TRIGGER [bx_UsersInRoles_After_Update]	ON [bx_UsersInRoles] After UPDATE

AS
BEGIN

	SET NOCOUNT ON;
	
	IF UPDATE([RoleID]) BEGIN
	
		DECLARE @tempTable table(RoleID int,TotalUser int);
		
		INSERT INTO @tempTable
			SELECT DISTINCT RoleID,
				ISNULL((SELECT COUNT(*) FROM [bx_UsersInRoles] as r WITH (NOLOCK) WHERE r.RoleID=T.RoleID),0)
			FROM [INSERTED]	T;
			
		INSERT INTO @tempTable
			SELECT DISTINCT RoleID,
				ISNULL((SELECT COUNT(*) FROM [bx_UsersInRoles] AS r WITH (NOLOCK) WHERE r.RoleID=T.RoleID),0)
			FROM [DELETED] T
			WHERE RoleID NOT IN (SELECT RoleID FROM @tempTable);
			
		UPDATE [bx_Roles]
			SET
				bx_Roles.UserCount=T.TotalUser
			FROM @tempTable T
			WHERE
				T.RoleID=bx_Roles.RoleID;
	END
END		
GO

EXEC bx_Drop 'bx_Shares_AfterDelete';

GO


CREATE TRIGGER [bx_Shares_AfterDelete]
	ON [bx_UserShares]
	AFTER DELETE
AS
BEGIN
	
	SET NOCOUNT ON;
			
	DELETE bx_Denouncings WHERE Type=5 AND TargetID IN (SELECT ShareID FROM [DELETED]);
	
	--删除评论
	DELETE [bx_Comments] WHERE [Type] = 5 AND [TargetID] IN (SELECT [ShareID] FROM [DELETED]);
	
	DECLARE @tempTable table(UserID int, ShareCount int,CollectionCount int);

	INSERT INTO @tempTable
		SELECT DISTINCT UserID
			,(SELECT COUNT(*) FROM [bx_UserShares] WITH (NOLOCK) WHERE [PrivacyType] = 2 AND [UserID] = D.UserID)
			,(SELECT COUNT(*) FROM [bx_UserShares] WITH (NOLOCK) WHERE [PrivacyType] < 2 AND [UserID] = D.UserID)
		FROM [DELETED] D
	
	UPDATE [bx_Users]
		SET
			  TotalShares = T.ShareCount
			, TotalCollections = T.CollectionCount
		FROM @tempTable T
		WHERE
			T.UserID = [bx_Users].UserID;
	
	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, ShareCount AS TotalShares ,CollectionCount AS TotalCollections FROM @tempTable;
	
END

GO

EXEC bx_Drop 'bx_Shares_AfterInsert';

GO


CREATE TRIGGER [bx_Shares_AfterInsert]
	ON [bx_UserShares]
	AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(UserID int, ShareCount int,CollectionCount int);

	INSERT INTO @tempTable 
		SELECT DISTINCT UserID
			,(SELECT COUNT(*) FROM [bx_UserShares] WITH (NOLOCK) WHERE [PrivacyType] = 2 AND [UserID] = T.UserID)
			,(SELECT COUNT(*) FROM [bx_UserShares] WITH (NOLOCK) WHERE [PrivacyType] < 2 AND [UserID] = T.UserID)
		FROM [INSERTED] T;
	
	UPDATE [bx_Users]
		SET
			  TotalShares = T.ShareCount
			, TotalCollections = T.CollectionCount
		FROM @tempTable T
		WHERE
			T.UserID = [bx_Users].UserID;
			
	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, ShareCount AS TotalShares ,CollectionCount AS TotalCollections FROM @tempTable;
END

GO
--标签触发器
EXEC bx_Drop 'bx_TagRelation_AfterDelete';

GO

CREATE TRIGGER [bx_TagRelation_AfterDelete]
	ON [bx_TagRelation]
	AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @C        int;
	DECLARE @TagID    nvarchar(50);
	DECLARE @Type     tinyint;
	
	SET @TagID = (SELECT TOP 1 [TagID] FROM [DELETED]);
	SET @Type = (SELECT TOP 1 [Type] FROM [INSERTED]);
	
	IF @Type = 1 BEGIN
		SET @C = (SELECT COUNT(*) FROM [bx_TagRelation] WITH (NOLOCK) WHERE [TagID] = @TagID AND [TargetID] IN (SELECT [ArticleID] FROM [bx_BlogArticles] WHERE [PrivacyType] IN (0,3)));
	  END
	ELSE BEGIN
		SET @C = (SELECT COUNT(*) FROM [bx_TagRelation] WITH (NOLOCK) WHERE [TagID] = @TagID);
	  END
	
	UPDATE [bx_Tags] SET [TotalElements] = @C WHERE [ID] = @TagID;

END
GO
--标签触发器
EXEC bx_Drop 'bx_TagRelation_AfterInsert';

GO

CREATE TRIGGER [bx_TagRelation_AfterInsert]
	ON [bx_TagRelation]
	AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @C        int;
	DECLARE @TagID    nvarchar(50);
	DECLARE @Type     tinyint;
	
	SET @TagID = (SELECT TOP 1 [TagID] FROM [INSERTED]);
	SET @Type = (SELECT TOP 1 [Type] FROM [INSERTED]);
	
	IF @Type = 1 BEGIN
		SET @C = (SELECT COUNT(*) FROM [bx_TagRelation] WITH (NOLOCK) WHERE [TagID] = @TagID AND [TargetID] IN (SELECT [ArticleID] FROM [bx_BlogArticles] WHERE [PrivacyType] IN (0,3)));
	  END
	ELSE BEGIN
		SET @C = (SELECT COUNT(*) FROM [bx_TagRelation] WITH (NOLOCK) WHERE [TagID] = @TagID);
	  END
	
	UPDATE [bx_Tags] SET [TotalElements] = @C WHERE [ID] = @TagID;

END
GO

EXEC bx_Drop 'bx_Threads_AfterDelete';
GO

CREATE TRIGGER [bx_Threads_AfterDelete]
   ON bx_Threads
   AFTER DELETE--INSTEAD OF DELETE
AS 
BEGIN

SET NOCOUNT ON;

	DECLARE @ForumIDs nvarchar(64)
	EXEC bx_GetDisabledTriggerForumIDs
			@ForumIDs output

	IF(@ForumIDs<>'') BEGIN
		--如果是删除版面，就忽略下面的数据统计，直接结束--
		DECLARE @ForumID int,@ForumIDString nvarchar(64)
		SELECT top 1 @ForumID=ForumID FROM [deleted]
		SET @ForumIDString=Replace(','+STR(@ForumID)+',', ' ', '')
		SET @ForumIDs=','+@ForumIDs+','
		IF(CHARINDEX(@ForumIDString,@ForumIDs)>0) BEGIN
			--DELETE [bx_Posts] WHERE ThreadID in(SELECT ThreadID FROM [deleted])
			--delete [bx_Threads] where ThreadID in(SELECT ThreadID FROM [deleted])
			RETURN;
		END
	END
	DECLARE @tempTable2 TABLE(userID INT,threadCount INT,valuedThreadCount int/*,PostCount int*/)
	insert into @tempTable2(userID,threadCount,valuedThreadCount/*,PostCount*/)
	select distinct PostUserID
						,(SELECT COUNT(1) FROM [deleted] WHERE PostUserID=p.PostUserID AND ThreadStatus<4)
						,(SELECT COUNT(1) FROM [deleted] WHERE PostUserID=p.PostUserID AND ThreadStatus<4 AND IsValued=1)
						--,(SELECT COUNT(1) FROM [bx_Posts] POST WITH (NOLOCK) INNER JOIN [deleted] D  ON POST.ThreadID=D.ThreadID WHERE POST.UserID=P.PostUserID AND POST.SortOrder<4000000000000000) 
			FROM [deleted] p

	UPDATE bx_Users SET
			TotalTopics=TotalTopics-t.threadCount,
			--TotalPosts=TotalPosts-t.PostCount,
			--DeletedPosts=DeletedPosts+t.PostCount,--t.threadCount,
			ValuedTopics=ValuedTopics-t.valuedThreadCount FROM @tempTable2 as t where bx_Users.UserID=t.userID


--------------------------
	--delete [bx_Threads] where ThreadID in(SELECT ThreadID FROM [deleted])

	DECLARE @tempTable TABLE(ForumID INT,ThreadCount INT,/*PostCount INT,*/TodayThreads INT/*,TodayPosts INT*/,LastThreadID INT)
	insert into @tempTable(ForumID,ThreadCount,/*PostCount,*/TodayThreads/*,TodayPosts*/,LastThreadID)
	select distinct T.ForumID,
					(SELECT COUNT(1) FROM [deleted] D WITH (NOLOCK) WHERE D.ForumID = T.ForumID AND  D.ThreadStatus<4)
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [deleted] D WITH (NOLOCK) ON P.ThreadID = D.ThreadID WHERE P.ForumID = T.ForumID AND P.SortOrder<4000000000000000)
					,(SELECT COUNT(1) FROM [deleted] D WITH (NOLOCK) WHERE D.ForumID = T.ForumID AND  D.ThreadStatus<4 AND  DATEDIFF(day, D.CreateDate, getdate())=0)--D.ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [deleted] D ON P.ThreadID=D.ThreadID WHERE D.ForumID = T.ForumID AND  P.SortOrder<4000000000000000 AND  DATEDIFF(day, P.CreateDate, getdate())=0)--P.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					--,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=T.ForumID AND SortOrder<2000000000000000))
					--,(SELECT ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WHERE T1.ForumID = T.ForumID AND T1.ThreadID NOT IN(SELECT ThreadID FROM [deleted] WITH(NOLOCK))))
					,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=T.ForumID AND T2.ThreadStatus = 1))
					FROM [deleted] T;

	UPDATE bx_Forums SET 
					TotalThreads=TotalThreads-t.ThreadCount 
					--,TotalPosts=TotalPosts-t.PostCount 
					,bx_Forums.TodayThreads=bx_Forums.TodayThreads-t.TodayThreads
					--,bx_Forums.TodayPosts=bx_Forums.TodayPosts-t.TodayPosts
					,LastThreadID=ISNULL(t.LastThreadID,0)
					from @tempTable as t WHERE bx_Forums.ForumID=t.ForumID
	
	
	--DELETE [bx_Posts] FROM [deleted] d WHERE [bx_Posts].ThreadID = d.ThreadID
	--DELETE [bx_Posts] WHERE ThreadID in(SELECT ThreadID FROM [deleted])
	--delete [bx_Threads] where ThreadID in(SELECT ThreadID FROM [deleted])
	
	DECLARE @tempCatalogTable TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
	INSERT INTO @tempCatalogTable(TempForumID,TempCatalogID,TempThreadCount)
	SELECT distinct ForumID,
					ThreadCatalogID,
					(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadCatalogID=D2.ThreadCatalogID AND D1.ThreadStatus<4) 
					FROM [deleted] D2
	
	UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads-TempThreadCount FROM @tempCatalogTable WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID
	
	DELETE bx_Denouncings WHERE Type=6 AND TargetID IN (SELECT [ThreadID] FROM [DELETED]);
	
END



GO

EXEC bx_Drop 'bx_Threads_AfterUpdate';
GO

CREATE TRIGGER [bx_Threads_AfterUpdate] 
   ON  [bx_Threads] 
   AFTER UPDATE
AS 
BEGIN

	SET NOCOUNT ON;
	

	IF UPDATE(ForumID) BEGIN
		--如果是合并版面导致的移动主题，就忽略下面的数据统计，直接结束
		DECLARE @ForumIDs nvarchar(64)
		EXEC bx_GetDisabledTriggerForumIDs
				@ForumIDs output
		IF(@ForumIDs<>'') BEGIN
			DECLARE @ForumID int,@ForumIDString nvarchar(64)
			SELECT top 1 @ForumID=ForumID FROM [deleted]
			SET @ForumIDString=Replace(','+STR(@ForumID)+',', ' ', '')
			SET @ForumIDs=','+@ForumIDs+','
			IF(CHARINDEX(@ForumIDString,@ForumIDs)>0) BEGIN
				RETURN;
			END
		END

		--移动之前的版块
		DECLARE @tempTable3 TABLE(ForumID INT,ThreadCount INT,/*PostCount INT,*/TodayThreadCount INT/*,TodayPostCount INT*/,LastThreadID INT)
		insert into @tempTable3(ForumID,ThreadCount,TodayThreadCount,LastThreadID)
		select distinct T.ForumID,
					(SELECT COUNT(1) FROM [deleted] D WITH(NOLOCK) WHERE D.ForumID = T.ForumID AND D.ThreadStatus<4)
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [deleted] D ON P.ThreadID=D.ThreadID WHERE D.ForumID = T.ForumID AND P.IsApproved=1)
					,(SELECT COUNT(1) FROM [deleted] D WITH (NOLOCK) WHERE D.ForumID = T.ForumID AND D.ThreadStatus<4 AND  DATEDIFF(day, D.CreateDate, getdate())=0)--D.ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [deleted] D ON P.ThreadID=D.ThreadID WHERE D.ForumID = T.ForumID AND P.IsApproved=1 AND P.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2 WITH(NOLOCK) WHERE T2.ForumID=T.ForumID AND T2.ThreadStatus = 1))
					FROM [deleted] T;
/*
					(SELECT COUNT(1) FROM [bx_Threads] T1 WITH (NOLOCK) WHERE T1.ForumID = T.ForumID),
					(SELECT COUNT(1) FROM [bx_Posts] WITH (NOLOCK) INNER JOIN bx_Threads WITH (NOLOCK) ON bx_Posts.ThreadID = bx_Threads.ThreadID WHERE (bx_Posts.IsApproved=1 AND bx_Threads.ForumID = T.ForumID)),
					(SELECT COUNT(1) FROM [bx_Threads] D WITH (NOLOCK) WHERE D.ForumID = T.ForumID AND D.ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID)),
					(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] D ON P.ThreadID=D.ThreadID WHERE D.ForumID = T.ForumID AND P.IsApproved=1 AND P.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID)),
					--(SELECT COUNT(1) FROM [deleted] D WITH (NOLOCK) WHERE D.ForumID = T.ForumID AND D.ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID)),
					--(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [deleted] D ON P.ThreadID=D.ThreadID WHERE D.ForumID = T.ForumID AND P.IsApproved=1 AND P.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID)),
					(SELECT ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WITH (NOLOCK) INNER JOIN bx_Threads T3 ON T1.ThreadID=T3.ThreadID WHERE T3.ForumID = T.ForumID AND T1.IsApproved=1))
					FROM [deleted] T;
*/

				UPDATE bx_Forums SET 
						TotalThreads=TotalThreads-t.ThreadCount 
						--,TotalPosts=TotalPosts-t.PostCount 
						,TodayThreads=TodayThreads-TodayThreadCount
						--,TodayPosts=TodayPosts-TodayPostCount
						,LastThreadID=ISNULL(t.LastThreadID,0)
						from @tempTable3 as t WHERE bx_Forums.ForumID=t.ForumID

		DECLARE @tempCatalogTable TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
		INSERT INTO @tempCatalogTable(TempForumID,TempCatalogID,TempThreadCount)
		SELECT distinct ForumID,
						ThreadCatalogID,
						(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
						FROM [deleted] D2
		
		UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads-TempThreadCount FROM @tempCatalogTable WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID
		
		

		--移动之后的版块
		DECLARE @tempTable4 TABLE(ForumID INT,ThreadCount INT,TodayThreadCount INT,LastThreadID INT)
		insert into @tempTable4(ForumID,ThreadCount,TodayThreadCount,LastThreadID)
		select distinct T.ForumID,
					(SELECT COUNT(1) FROM [inserted] I WITH(NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4)
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [inserted] I ON P.ThreadID=I.ThreadID WHERE I.ForumID = T.ForumID AND P.IsApproved=1)
					,(SELECT COUNT(1) FROM [inserted] I WITH (NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4 AND DATEDIFF(day, I.CreateDate, getdate())=0) --I.ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					--,(SELECT COUNT(1) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [inserted] I ON P.ThreadID=I.ThreadID WHERE I.ForumID = T.ForumID AND P.IsApproved=1 AND P.PostID>(SELECT YestodayLastPostID FROM bx_Forums F WITH(NOLOCK) WHERE F.ForumID=T.ForumID))
					--,(SELECT ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WITH (NOLOCK) INNER JOIN bx_Threads T3 ON T1.ThreadID=T3.ThreadID WHERE T3.ForumID = T.ForumID AND T1.IsApproved=1))
					,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2 WITH(NOLOCK) WHERE T2.ForumID=T.ForumID AND T2.ThreadStatus=1))
					FROM [inserted] T;

		UPDATE bx_Forums SET 
						TotalThreads=TotalThreads+t.ThreadCount 
						--,TotalPosts=TotalPosts+t.PostCount 
						,TodayThreads=TodayThreads+TodayThreadCount
						--,TodayPosts=TodayPosts+TodayPostCount
						,LastThreadID=ISNULL(t.LastThreadID,0)
						from @tempTable4 as t WHERE bx_Forums.ForumID=t.ForumID


		DECLARE @tempCatalogTable2 TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
		INSERT INTO @tempCatalogTable2(TempForumID,TempCatalogID,TempThreadCount)
		SELECT distinct ForumID,
						ThreadCatalogID,
						(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
						FROM [inserted] D2
		
		UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+TempThreadCount FROM @tempCatalogTable2 WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID


		DECLARE @TempForumIDTable Table(TempThreadID INT,TempNewForumID INT)
		INSERT INTO @TempForumIDTable 
		SELECT ThreadID,ForumID  FROM [inserted]
				--,(SELECT N.ForumID FROM [inserted] N WHERE N.ThreadID=D.ThreadID)
				--FROM [deleted] D
		UPDATE bx_Posts SET ForumID=T.TempNewForumID from @TempForumIDTable T WHERE bx_Posts.ThreadID=T.TempThreadID

	END

	IF UPDATE(IsValued) BEGIN
		DECLARE @tempTable5 TABLE(userID INT,ValuedThreadCount INT,ValuedThreadCount2 INT)
		insert into @tempTable5(userID,ValuedThreadCount,ValuedThreadCount2)
		select distinct PostUserID
							--更新前的精华帖子数
							,(SELECT COUNT(1) FROM [deleted] WHERE PostUserID=p.PostUserID AND ThreadStatus<4 AND IsValued=1)
							--更新后的精华帖子数
							,(SELECT COUNT(1) FROM [inserted] WHERE PostUserID=p.PostUserID AND ThreadStatus<4 AND IsValued=1)
				FROM [deleted] p  --WHERE ThreadLocation=0

		UPDATE bx_Users SET
				ValuedTopics=ValuedTopics+(t.ValuedThreadCount2-t.ValuedThreadCount)
			 FROM @tempTable5 as t where bx_Users.UserID=t.userID
	END
	
	IF UPDATE(ThreadCatalogID) BEGIN
		DECLARE @tempCatalogTable3 TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
		INSERT INTO @tempCatalogTable3(TempForumID,TempCatalogID,TempThreadCount)
		SELECT distinct ForumID,
						ThreadCatalogID,
						(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
						FROM [deleted] D2
		
		UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads-TempThreadCount FROM @tempCatalogTable3 WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID
		
		DECLARE @tempCatalogTable4 TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
		INSERT INTO @tempCatalogTable4(TempForumID,TempCatalogID,TempThreadCount)
		SELECT distinct ForumID,
						ThreadCatalogID,
						(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
						FROM [inserted] D2
		
		UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+TempThreadCount FROM @tempCatalogTable4 WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID
	END
	
	IF UPDATE(ThreadStatus) BEGIN
		DECLARE @OldThreadStatus bigint,@NewThreadStatus bigint
		SELECT top 1 @OldThreadStatus=ThreadStatus FROM [deleted]
		SELECT top 1 @NewThreadStatus=ThreadStatus FROM [inserted]
		IF (@OldThreadStatus<4 AND @NewThreadStatus>=4) or (@OldThreadStatus>=4 AND @NewThreadStatus<4) BEGIN
				 
		    /*
			DECLARE @TempSortOrderTable Table(tempId int IDENTITY(1, 1),TempThreadID INT,TempNewSortOrder BIGINT)
			INSERT INTO @TempSortOrderTable (TempThreadID,TempNewSortOrder)
			SELECT ThreadID,SortOrder  FROM [inserted]
			*/
					--,(SELECT N.SortOrder FROM [inserted] N WHERE N.ThreadID=D.ThreadID)
					--FROM [deleted] D
----------------------------------------------------------------		
			DECLARE @PostTable table(tempPostTableId int IDENTITY(1, 1),TempPostTableThreadID INT,TempPostTablePostID Int,TempPostTableSortOrder BIGINT,Status int)
			INSERT INTO @PostTable(TempPostTableThreadID,TempPostTablePostID,TempPostTableSortOrder)
				SELECT ThreadID,PostID,SortOrder FROM [bx_Posts] WHERE ThreadID in(SELECT ThreadID FROM [inserted])
			
			DECLARE @i int,@Total int;
			/*
			SET @i = 0;
			SELECT @Total = COUNT(*) FROM @TempSortOrderTable;
			
			WHILE(@i<@Total) BEGIN
				SET @i = @i + 1;
				DECLARE @TempOldSortOrder bigint,@TempThreadID int;
			
				SELECT @TempThreadID = TempThreadID, @TempOldSortOrder = TempNewSortOrder FROM @TempSortOrderTable WHERE tempId = @i;
				
				DECLARE @Status int;
				IF @TempOldSortOrder >= 5000000000000000
					SET @Status = 5;
				ELSE IF @TempOldSortOrder>=4000000000000000 AND @TempOldSortOrder<5000000000000000
					SET @Status = 4;
				ELSE
					SET @Status = 1;
				
				UPDATE @PostTable SET Status=@Status WHERE TempPostTableThreadID = @TempThreadID;
				
				
			END
			
			
			SET @i = 0;
			SELECT @Total = COUNT(*) FROM @PostTable;
			WHILE(@i<@Total) BEGIN
				SET @i = @i + 1;
				
				DECLARE @TempOldPostSortOrder bigint,@PostSortOrderStatus int,@TempNewSortOrder bigint;
				SELECT @TempOldPostSortOrder = TempPostTableSortOrder,@PostSortOrderStatus = Status FROM @PostTable WHERE tempPostTableId = @i;
	
				EXEC [bx_UpdateSortOrder] @PostSortOrderStatus, @TempOldPostSortOrder, @TempNewSortOrder OUTPUT;
				
				UPDATE @PostTable SET TempPostTableSortOrder = @TempNewSortOrder WHERE tempPostTableId = @i; 
			END
			*/
			
			SET @i = 0;
			SELECT @Total = COUNT(*) FROM @PostTable;
			WHILE(@i<@Total) BEGIN
				SET @i = @i + 1;
				
				DECLARE @TempOldPostSortOrder bigint,@TempNewSortOrder bigint;
				
				SELECT @TempOldPostSortOrder = TempPostTableSortOrder FROM @PostTable WHERE tempPostTableId = @i;
	
				EXEC [bx_UpdateSortOrder] @NewThreadStatus, @TempOldPostSortOrder, @TempNewSortOrder OUTPUT;
				
				UPDATE @PostTable SET TempPostTableSortOrder = @TempNewSortOrder WHERE tempPostTableId = @i; 
			END
			
			UPDATE bx_Posts SET SortOrder=TempPostTableSortOrder from @PostTable T WHERE bx_Posts.PostID=T.TempPostTablePostID;
			
---------------上面这段 只相当于原来底下这3句 -------------------------------------
			
			--UPDATE bx_Posts SET SortOrder=[dbo].bx_UpdateSortOrder(5,SortOrder) from @TempSortOrderTable T WHERE bx_Posts.ThreadID=T.TempThreadID AND TempNewSortOrder >= 5000000000000000
			
			--UPDATE bx_Posts SET SortOrder=[dbo].bx_UpdateSortOrder(4,SortOrder) from @TempSortOrderTable T WHERE bx_Posts.ThreadID=T.TempThreadID AND TempNewSortOrder>=4000000000000000 AND TempNewSortOrder<5000000000000000
			
			--UPDATE bx_Posts SET SortOrder=[dbo].bx_UpdateSortOrder(1,SortOrder) from @TempSortOrderTable T WHERE bx_Posts.ThreadID=T.TempThreadID AND TempNewSortOrder<4000000000000000
		
				 
			DECLARE @tempForumTable TABLE(ForumID INT,ThreadCount1 INT,TodayThreadCount1 INT,ThreadCount2 INT,TodayThreadCount2 INT,LastThreadID INT)
			insert into @tempForumTable(ForumID,ThreadCount1,TodayThreadCount1,ThreadCount2,TodayThreadCount2,LastThreadID)
			select distinct T.ForumID,
						(SELECT COUNT(1) FROM [deleted] I WITH(NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4)
						,(SELECT COUNT(1) FROM [deleted] I WITH (NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4 AND DATEDIFF(day, I.CreateDate, getdate())=0)
						,(SELECT COUNT(1) FROM [inserted] I WITH(NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4)
						,(SELECT COUNT(1) FROM [inserted] I WITH (NOLOCK) WHERE I.ForumID = T.ForumID AND I.ThreadStatus<4 AND DATEDIFF(day, I.CreateDate, getdate())=0)
						,(SELECT ThreadID FROM [bx_Threads] T1 WHERE T1.SortOrder=(SELECT MAX(SortOrder) FROM [bx_Threads] T2  WITH(NOLOCK) WHERE T2.ForumID=T.ForumID AND T2.ThreadStatus = 1))
						FROM [inserted] T;

			UPDATE bx_Forums SET 
							TotalThreads=TotalThreads+(t.ThreadCount2-t.ThreadCount1)
							,TodayThreads=TodayThreads+(t.TodayThreadCount2-t.TodayThreadCount1)
							,LastThreadID=ISNULL(t.LastThreadID,0)
							from @tempForumTable as t WHERE bx_Forums.ForumID=t.ForumID



			DECLARE @tempTable2 TABLE(userID INT,threadCount INT,valuedThreadCount int,threadCount2 INT,valuedThreadCount2 int)
			insert into @tempTable2(userID,threadCount,valuedThreadCount,threadCount2,valuedThreadCount2)
			select distinct PostUserID
								--更新前的正常的帖子数
								,(SELECT COUNT(1) FROM [deleted]  WHERE PostUserID=p.PostUserID AND ThreadStatus<4)
								,(SELECT COUNT(1) FROM [deleted]  WHERE PostUserID=p.PostUserID AND ThreadStatus<4 AND IsValued=1)
								--,(SELECT COUNT(1) FROM [bx_Posts] POST  INNER JOIN [deleted] D  ON POST.ThreadID=D.ThreadID WHERE POST.UserID=P.PostUserID AND POST.IsApproved=1 AND D.ForumID>0) 
								--更新后的正常的帖子数
								,(SELECT COUNT(1) FROM [inserted]  WHERE PostUserID=p.PostUserID AND ThreadStatus<4)
								,(SELECT COUNT(1) FROM [inserted]  WHERE PostUserID=p.PostUserID AND ThreadStatus<4 AND IsValued=1)
								--,(SELECT COUNT(1) FROM [bx_Posts] POST  INNER JOIN [inserted] I  ON POST.ThreadID=I.ThreadID WHERE POST.UserID=P.PostUserID AND POST.IsApproved=1 AND I.ForumID>0) 
					FROM [deleted] p  

			UPDATE bx_Users SET
					TotalTopics=TotalTopics+(t.threadCount2-t.threadCount),
					--TotalPosts=TotalPosts+(t.PostCount2-t.PostCount),
					--DeletedPosts=DeletedPosts-(t.threadCount2-t.threadCount),
					--DeletedThreads=DeletedThreads-(t.threadCount2-t.threadCount),
					ValuedTopics=ValuedTopics+(t.valuedThreadCount2-t.valuedThreadCount)
				 FROM @tempTable2 as t where bx_Users.UserID=t.userID
				 
				 
				 
			DECLARE @tempCatalogTable9 TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount1 INT,TempThreadCount2 INT)
			INSERT INTO @tempCatalogTable9(TempForumID,TempCatalogID,TempThreadCount1,TempThreadCount2)
			SELECT distinct ForumID,
							ThreadCatalogID,
							(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
							,(SELECT COUNT(1) FROM [deleted] D1 WHERE D1.ForumID=D2.ForumID AND D1.ThreadStatus<4 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
							FROM [deleted] D2
			
			UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+(TempThreadCount1-TempThreadCount2) FROM @tempCatalogTable9 WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID

	/*	
			DECLARE @tempCatalogTable10 TABLE(TempForumID INT,TempCatalogID INT,TempThreadCount INT)
			INSERT INTO @tempCatalogTable10(TempForumID,TempCatalogID,TempThreadCount)
			SELECT distinct ForumID,
							ThreadCatalogID,
							(SELECT COUNT(1) FROM [inserted] D1 WHERE D1.ForumID=D2.ForumID AND D1.SortOrder<4000000000000000 AND D1.ThreadCatalogID=D2.ThreadCatalogID) 
							FROM [inserted] D2
			
			UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+TempThreadCount FROM @tempCatalogTable10 WHERE ForumID=TempForumID AND ThreadCatalogID=TempCatalogID
	*/		
		END	
		
		
		--原来是正常的  现在变为不正常
		IF (@OldThreadStatus<4 AND @NewThreadStatus>=4) BEGIN
			-- 删除举报
			DELETE bx_Denouncings WHERE Type = 6 AND TargetID in(SELECT ThreadID FROM [deleted]); 
		END
	END
END



GO

EXEC bx_Drop 'bx_UserExtendedValues_AfterDelete';

GO


CREATE TRIGGER [bx_UserExtendedValues_AfterDelete]
	ON [bx_UserExtendedValues]
	AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;

	IF (OBJECT_ID('tempdb..#bx_tmp_usrext') IS NULL) BEGIN
		CREATE TABLE #bx_tmp_usrext ( UserID int, Data ntext DEFAULT('') );
	END
	ELSE BEGIN
		TRUNCATE TABLE #bx_tmp_usrext;
	END

	INSERT INTO #bx_tmp_usrext (UserID) SELECT DISTINCT UserID FROM [DELETED];

	DECLARE @UserID int;
	DECLARE @i int;
	DECLARE @ptr binary(16);

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalRoles int;

	DECLARE @userRoles table( ID int IDENTITY(1,1), UserID int, RoleID uniqueidentifier, BeginDate datetime, EndDate datetime );
	INSERT INTO @userRoles SELECT UserID, RoleID, BeginDate, EndDate FROM bx_UserRoles WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [DELETED]);

	SET @TotalRoles = @@ROWCOUNT;

	DECLARE
		@RoleIDString varchar(50),
		@BeginDateString varchar(30),
		@EndDateString varchar(30),
		@UserRoleString varchar(100);
	SET @i = 1;
	WHILE (@i <= @TotalRoles) BEGIN
		SELECT @UserID = UserID, @RoleIDString = CONVERT(varchar(50), RoleID), @BeginDateString = CONVERT(varchar(30), BeginDate, 25), @EndDateString = CONVERT(varchar(30), EndDate, 25) FROM @userRoles WHERE ID = @i;
		
		SET @UserRoleString = 'R' + CAST(LEN(@RoleIDString) AS varchar(10))
								+ ',' + CAST(LEN(@BeginDateString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ':' + @RoleIDString
								+ @BeginDateString
								+ @EndDateString;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserRoleString;

		SET @i = @i + 1;
	END

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalValues int;

	DECLARE @userValues table( ID int IDENTITY(1,1), UserID int, FieldID varchar(36), Value nvarchar(3950),PrivacyType tinyint);
	INSERT INTO @userValues SELECT UserID, ExtendedFieldID AS FieldID, RTRIM(LTRIM(Value)),PrivacyType FROM bx_UserExtendedValues WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [DELETED]);

	SET @TotalValues = @@ROWCOUNT;

	DECLARE
		@FieldIDString varchar(36),
		@ValueString nvarchar(3950),
		@PrivacyType varchar(10),
		@UserValueString varchar(4000);
	SET @i = 1;
	WHILE (@i <= @TotalValues) BEGIN
		SELECT @UserID = UserID, @FieldIDString = FieldID, @ValueString = Value ,@PrivacyType = PrivacyType  FROM @userValues WHERE ID = @i;
		
		SET @UserValueString = 'F' + CAST(LEN(@FieldIDString) AS varchar(10))
								+ ',' + CAST(LEN(@ValueString) AS varchar(10))
								+ ',' + CAST(LEN(@PrivacyType) AS varchar(10))
								+ ':' + @FieldIDString
								+ @ValueString
								+ @PrivacyType;

		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserValueString;

		SET @i = @i + 1;
	END

	--以下是处理用户勋章

	DECLARE @TotalMedals int;

	DECLARE @userMedals table( ID int IDENTITY(1,1), UserID int, MedalID int, MedalLevelID int, EndDate datetime, CreateDate datetime, Url nvarchar(200));
	INSERT INTO @userMedals SELECT UserID, MedalID, MedalLevelID, EndDate, CreateDate, ISNULL(Url,'') FROM bx_UserMedals WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [DELETED]);

	SET @TotalMedals = @@ROWCOUNT;

	DECLARE
		@MedalIDString varchar(50),
		@MedalLevelIDString varchar(50),
		@CreateDateString varchar(30),
		@Url nvarchar(200),
		@UserMedalString nvarchar(500);
	SET @i = 1;
	WHILE (@i <= @TotalMedals) BEGIN
		SELECT @UserID = UserID, @MedalIDString = CONVERT(varchar(50), MedalID), @MedalLevelIDString = CONVERT(varchar(50), MedalLevelID), @EndDateString = CONVERT(varchar(30), EndDate, 25), @CreateDateString = CONVERT(varchar(30), CreateDate, 25),@Url = Url FROM @userMedals WHERE ID = @i;
		
		SET @UserMedalString = 'M' + CAST(LEN(@MedalIDString) AS varchar(10))
								+ ',' + CAST(LEN(@MedalLevelIDString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ',' + CAST(LEN(@CreateDateString) AS varchar(10))
								+','  + CAST(LEN(@Url) AS varchar(10))
								+ ':' + @MedalIDString
								+ @MedalLevelIDString
								+ @EndDateString
								+ @CreateDateString
								+ @Url;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserMedalString;

		SET @i = @i + 1;
	END




	UPDATE bx_Users
	   SET ExtendedData = T.Data
	   FROM #bx_tmp_usrext T
	   WHERE T.UserID = bx_Users.UserID;

END
GO

EXEC bx_Drop 'bx_UserExtendedValues_AfterUpdate';

GO


CREATE TRIGGER [bx_UserExtendedValues_AfterUpdate]
	ON [bx_UserExtendedValues]
	AFTER INSERT, UPDATE
AS
BEGIN

	SET NOCOUNT ON;

	IF (OBJECT_ID('tempdb..#bx_tmp_usrext') IS NULL) BEGIN
		CREATE TABLE #bx_tmp_usrext ( UserID int, Data ntext DEFAULT('') );
	END
	ELSE BEGIN
		TRUNCATE TABLE #bx_tmp_usrext;
	END

	INSERT INTO #bx_tmp_usrext (UserID) SELECT DISTINCT UserID FROM [INSERTED];

	DECLARE @UserID int;
	DECLARE @i int;
	DECLARE @ptr binary(16);

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalRoles int;

	DECLARE @userRoles table( ID int IDENTITY(1,1), UserID int, RoleID uniqueidentifier, BeginDate datetime, EndDate datetime );
	INSERT INTO @userRoles SELECT UserID, RoleID, BeginDate, EndDate FROM bx_UserRoles WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [INSERTED]);

	SET @TotalRoles = @@ROWCOUNT;

	DECLARE
		@RoleIDString varchar(50),
		@BeginDateString varchar(30),
		@EndDateString varchar(30),
		@UserRoleString varchar(100);
	SET @i = 1;
	WHILE (@i <= @TotalRoles) BEGIN
		SELECT @UserID = UserID, @RoleIDString = CONVERT(varchar(50), RoleID), @BeginDateString = CONVERT(varchar(30), BeginDate, 25), @EndDateString = CONVERT(varchar(30), EndDate, 25) FROM @userRoles WHERE ID = @i;
		
		SET @UserRoleString = 'R' + CAST(LEN(@RoleIDString) AS varchar(10))
								+ ',' + CAST(LEN(@BeginDateString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ':' + @RoleIDString
								+ @BeginDateString
								+ @EndDateString;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserRoleString;

		SET @i = @i + 1;
	END

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalValues int;

	DECLARE @userValues table( ID int IDENTITY(1,1), UserID int, FieldID varchar(36), Value nvarchar(3950),PrivacyType tinyint);
	INSERT INTO @userValues SELECT UserID, ExtendedFieldID AS FieldID, RTRIM(LTRIM(Value)),PrivacyType FROM bx_UserExtendedValues WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [INSERTED]);

	SET @TotalValues = @@ROWCOUNT;

	DECLARE
		@FieldIDString varchar(36),
		@ValueString nvarchar(3950),
		@PrivacyType varchar(10),
		@UserValueString varchar(4000);
	SET @i = 1;
	WHILE (@i <= @TotalValues) BEGIN
		SELECT @UserID = UserID, @FieldIDString = FieldID, @ValueString = Value ,@PrivacyType = PrivacyType  FROM @userValues WHERE ID = @i;
		
		SET @UserValueString = 'F' + CAST(LEN(@FieldIDString) AS varchar(10))
								+ ',' + CAST(LEN(@ValueString) AS varchar(10))
								+ ',' + CAST(LEN(@PrivacyType) AS varchar(10))
								+ ':' + @FieldIDString
								+ @ValueString
								+ @PrivacyType;

		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserValueString;

		SET @i = @i + 1;
	END

	--以下是处理用户勋章

	DECLARE @TotalMedals int;

	DECLARE @userMedals table( ID int IDENTITY(1,1), UserID int, MedalID int, MedalLevelID int, EndDate datetime, CreateDate datetime, Url nvarchar(200));
	INSERT INTO @userMedals SELECT UserID, MedalID, MedalLevelID, EndDate, CreateDate, ISNULL(Url,'') FROM bx_UserMedals WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [INSERTED]);

	SET @TotalMedals = @@ROWCOUNT;

	DECLARE
		@MedalIDString varchar(50),
		@MedalLevelIDString varchar(50),
		@CreateDateString varchar(30),
		@Url nvarchar(200),
		@UserMedalString nvarchar(500);
	SET @i = 1;
	WHILE (@i <= @TotalMedals) BEGIN
		SELECT @UserID = UserID, @MedalIDString = CONVERT(varchar(50), MedalID), @MedalLevelIDString = CONVERT(varchar(50), MedalLevelID), @EndDateString = CONVERT(varchar(30), EndDate, 25), @CreateDateString = CONVERT(varchar(30), CreateDate, 25),@Url = Url FROM @userMedals WHERE ID = @i;
		
		SET @UserMedalString = 'M' + CAST(LEN(@MedalIDString) AS varchar(10))
								+ ',' + CAST(LEN(@MedalLevelIDString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ',' + CAST(LEN(@CreateDateString) AS varchar(10))
								+','  + CAST(LEN(@Url) AS varchar(10))
								+ ':' + @MedalIDString
								+ @MedalLevelIDString
								+ @EndDateString
								+ @CreateDateString
								+ @Url;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserMedalString;

		SET @i = @i + 1;
	END




	UPDATE bx_Users
	   SET ExtendedData = T.Data
	   FROM #bx_tmp_usrext T
	   WHERE T.UserID = bx_Users.UserID;

END
GO

EXEC bx_Drop 'bx_UserInfos_AfterUpdate';

GO

CREATE TRIGGER [bx_UserInfos_AfterUpdate]
	ON [bx_UserInfos]
	AFTER INSERT, UPDATE
AS
BEGIN


	SET NOCOUNT ON;

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
	  FROM [INSERTED] T WHERE T.UserID = bx_Users.UserID;
	  
	  SELECT 'ResetUser' AS XCMD, * FROM [INSERTED];
END


GO

EXEC bx_Drop 'bx_UserMedals_AfterDelete';

GO


CREATE TRIGGER [bx_UserMedals_AfterDelete]
	ON [bx_UserMedals]
	AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;

	IF (OBJECT_ID('tempdb..#bx_tmp_usrext') IS NULL) BEGIN
		CREATE TABLE #bx_tmp_usrext ( UserID int, Data ntext DEFAULT('') );
	END
	ELSE BEGIN
		TRUNCATE TABLE #bx_tmp_usrext;
	END

	INSERT INTO #bx_tmp_usrext (UserID) SELECT DISTINCT UserID FROM [DELETED];

	DECLARE @UserID int;
	DECLARE @i int;
	DECLARE @ptr binary(16);

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalRoles int;

	DECLARE @userRoles table( ID int IDENTITY(1,1), UserID int, RoleID uniqueidentifier, BeginDate datetime, EndDate datetime );
	INSERT INTO @userRoles SELECT UserID, RoleID, BeginDate, EndDate FROM bx_UserRoles WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [DELETED]);

	SET @TotalRoles = @@ROWCOUNT;

	DECLARE
		@RoleIDString varchar(50),
		@BeginDateString varchar(30),
		@EndDateString varchar(30),
		@UserRoleString varchar(100);
	SET @i = 1;
	WHILE (@i <= @TotalRoles) BEGIN
		SELECT @UserID = UserID, @RoleIDString = CONVERT(varchar(50), RoleID), @BeginDateString = CONVERT(varchar(30), BeginDate, 25), @EndDateString = CONVERT(varchar(30), EndDate, 25) FROM @userRoles WHERE ID = @i;
		
		SET @UserRoleString = 'R' + CAST(LEN(@RoleIDString) AS varchar(10))
								+ ',' + CAST(LEN(@BeginDateString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ':' + @RoleIDString
								+ @BeginDateString
								+ @EndDateString;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserRoleString;

		SET @i = @i + 1;
	END

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalValues int;

	DECLARE @userValues table( ID int IDENTITY(1,1), UserID int, FieldID varchar(36), Value nvarchar(3950),PrivacyType tinyint);
	INSERT INTO @userValues SELECT UserID, ExtendedFieldID AS FieldID, RTRIM(LTRIM(Value)),PrivacyType FROM bx_UserExtendedValues WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [DELETED]);

	SET @TotalValues = @@ROWCOUNT;

	DECLARE
		@FieldIDString varchar(36),
		@ValueString nvarchar(3950),
		@PrivacyType varchar(10),
		@UserValueString varchar(4000);
	SET @i = 1;
	WHILE (@i <= @TotalValues) BEGIN
		SELECT @UserID = UserID, @FieldIDString = FieldID, @ValueString = Value ,@PrivacyType = PrivacyType  FROM @userValues WHERE ID = @i;
		
		SET @UserValueString = 'F' + CAST(LEN(@FieldIDString) AS varchar(10))
								+ ',' + CAST(LEN(@ValueString) AS varchar(10))
								+ ',' + CAST(LEN(@PrivacyType) AS varchar(10))
								+ ':' + @FieldIDString
								+ @ValueString
								+ @PrivacyType;

		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserValueString;

		SET @i = @i + 1;
	END

	--以下是处理用户勋章

	DECLARE @TotalMedals int;

	DECLARE @userMedals table( ID int IDENTITY(1,1), UserID int, MedalID int, MedalLevelID int, EndDate datetime, CreateDate datetime, Url nvarchar(200));
	INSERT INTO @userMedals SELECT UserID, MedalID, MedalLevelID, EndDate, CreateDate, ISNULL(Url,'') FROM bx_UserMedals WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [DELETED]);

	SET @TotalMedals = @@ROWCOUNT;

	DECLARE
		@MedalIDString varchar(50),
		@MedalLevelIDString varchar(50),
		@CreateDateString varchar(30),
		@Url nvarchar(200),
		@UserMedalString nvarchar(500);
	SET @i = 1;
	WHILE (@i <= @TotalMedals) BEGIN
		SELECT @UserID = UserID, @MedalIDString = CONVERT(varchar(50), MedalID), @MedalLevelIDString = CONVERT(varchar(50), MedalLevelID), @EndDateString = CONVERT(varchar(30), EndDate, 25), @CreateDateString = CONVERT(varchar(30), CreateDate, 25),@Url = Url FROM @userMedals WHERE ID = @i;
		
		SET @UserMedalString = 'M' + CAST(LEN(@MedalIDString) AS varchar(10))
								+ ',' + CAST(LEN(@MedalLevelIDString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ',' + CAST(LEN(@CreateDateString) AS varchar(10))
								+','  + CAST(LEN(@Url) AS varchar(10))
								+ ':' + @MedalIDString
								+ @MedalLevelIDString
								+ @EndDateString
								+ @CreateDateString
								+ @Url;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserMedalString;

		SET @i = @i + 1;
	END




	UPDATE bx_Users
	   SET ExtendedData = T.Data
	   FROM #bx_tmp_usrext T
	   WHERE T.UserID = bx_Users.UserID;

END
GO

EXEC bx_Drop 'bx_UserMedals_AfterUpdate';

GO


CREATE TRIGGER [bx_UserMedals_AfterUpdate]
	ON [bx_UserMedals]
	AFTER INSERT, UPDATE
AS
BEGIN

	SET NOCOUNT ON;

	IF (OBJECT_ID('tempdb..#bx_tmp_usrext') IS NULL) BEGIN
		CREATE TABLE #bx_tmp_usrext ( UserID int, Data ntext DEFAULT('') );
	END
	ELSE BEGIN
		TRUNCATE TABLE #bx_tmp_usrext;
	END

	INSERT INTO #bx_tmp_usrext (UserID) SELECT DISTINCT UserID FROM [INSERTED];

	DECLARE @UserID int;
	DECLARE @i int;
	DECLARE @ptr binary(16);

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalRoles int;

	DECLARE @userRoles table( ID int IDENTITY(1,1), UserID int, RoleID uniqueidentifier, BeginDate datetime, EndDate datetime );
	INSERT INTO @userRoles SELECT UserID, RoleID, BeginDate, EndDate FROM bx_UserRoles WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [INSERTED]);

	SET @TotalRoles = @@ROWCOUNT;

	DECLARE
		@RoleIDString varchar(50),
		@BeginDateString varchar(30),
		@EndDateString varchar(30),
		@UserRoleString varchar(100);
	SET @i = 1;
	WHILE (@i <= @TotalRoles) BEGIN
		SELECT @UserID = UserID, @RoleIDString = CONVERT(varchar(50), RoleID), @BeginDateString = CONVERT(varchar(30), BeginDate, 25), @EndDateString = CONVERT(varchar(30), EndDate, 25) FROM @userRoles WHERE ID = @i;
		
		SET @UserRoleString = 'R' + CAST(LEN(@RoleIDString) AS varchar(10))
								+ ',' + CAST(LEN(@BeginDateString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ':' + @RoleIDString
								+ @BeginDateString
								+ @EndDateString;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserRoleString;

		SET @i = @i + 1;
	END

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalValues int;

	DECLARE @userValues table( ID int IDENTITY(1,1), UserID int, FieldID varchar(36), Value nvarchar(3950),PrivacyType tinyint);
	INSERT INTO @userValues SELECT UserID, ExtendedFieldID AS FieldID, RTRIM(LTRIM(Value)),PrivacyType FROM bx_UserExtendedValues WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [INSERTED]);

	SET @TotalValues = @@ROWCOUNT;

	DECLARE
		@FieldIDString varchar(36),
		@ValueString nvarchar(3950),
		@PrivacyType varchar(10),
		@UserValueString varchar(4000);
	SET @i = 1;
	WHILE (@i <= @TotalValues) BEGIN
		SELECT @UserID = UserID, @FieldIDString = FieldID, @ValueString = Value ,@PrivacyType = PrivacyType  FROM @userValues WHERE ID = @i;
		
		SET @UserValueString = 'F' + CAST(LEN(@FieldIDString) AS varchar(10))
								+ ',' + CAST(LEN(@ValueString) AS varchar(10))
								+ ',' + CAST(LEN(@PrivacyType) AS varchar(10))
								+ ':' + @FieldIDString
								+ @ValueString
								+ @PrivacyType;

		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserValueString;

		SET @i = @i + 1;
	END

	--以下是处理用户勋章

	DECLARE @TotalMedals int;

	DECLARE @userMedals table( ID int IDENTITY(1,1), UserID int, MedalID int, MedalLevelID int, EndDate datetime, CreateDate datetime, Url nvarchar(200));
	INSERT INTO @userMedals SELECT UserID, MedalID, MedalLevelID, EndDate, CreateDate, ISNULL(Url,'') FROM bx_UserMedals WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [INSERTED]);

	SET @TotalMedals = @@ROWCOUNT;

	DECLARE
		@MedalIDString varchar(50),
		@MedalLevelIDString varchar(50),
		@CreateDateString varchar(30),
		@Url nvarchar(200),
		@UserMedalString nvarchar(500);
	SET @i = 1;
	WHILE (@i <= @TotalMedals) BEGIN
		SELECT @UserID = UserID, @MedalIDString = CONVERT(varchar(50), MedalID), @MedalLevelIDString = CONVERT(varchar(50), MedalLevelID), @EndDateString = CONVERT(varchar(30), EndDate, 25), @CreateDateString = CONVERT(varchar(30), CreateDate, 25),@Url = Url FROM @userMedals WHERE ID = @i;
		
		SET @UserMedalString = 'M' + CAST(LEN(@MedalIDString) AS varchar(10))
								+ ',' + CAST(LEN(@MedalLevelIDString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ',' + CAST(LEN(@CreateDateString) AS varchar(10))
								+','  + CAST(LEN(@Url) AS varchar(10))
								+ ':' + @MedalIDString
								+ @MedalLevelIDString
								+ @EndDateString
								+ @CreateDateString
								+ @Url;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserMedalString;

		SET @i = @i + 1;
	END




	UPDATE bx_Users
	   SET ExtendedData = T.Data
	   FROM #bx_tmp_usrext T
	   WHERE T.UserID = bx_Users.UserID;

END
GO

EXEC bx_Drop 'bx_UserRoles_AfterDelete';

GO


CREATE TRIGGER [bx_UserRoles_AfterDelete]
	ON [bx_UserRoles]
	AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;

	IF (OBJECT_ID('tempdb..#bx_tmp_usrext') IS NULL) BEGIN
		CREATE TABLE #bx_tmp_usrext ( UserID int, Data ntext DEFAULT('') );
	END
	ELSE BEGIN
		TRUNCATE TABLE #bx_tmp_usrext;
	END

	INSERT INTO #bx_tmp_usrext (UserID) SELECT DISTINCT UserID FROM [DELETED];

	DECLARE @UserID int;
	DECLARE @i int;
	DECLARE @ptr binary(16);

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalRoles int;

	DECLARE @userRoles table( ID int IDENTITY(1,1), UserID int, RoleID uniqueidentifier, BeginDate datetime, EndDate datetime );
	INSERT INTO @userRoles SELECT UserID, RoleID, BeginDate, EndDate FROM bx_UserRoles WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [DELETED]);

	SET @TotalRoles = @@ROWCOUNT;

	DECLARE
		@RoleIDString varchar(50),
		@BeginDateString varchar(30),
		@EndDateString varchar(30),
		@UserRoleString varchar(100);
	SET @i = 1;
	WHILE (@i <= @TotalRoles) BEGIN
		SELECT @UserID = UserID, @RoleIDString = CONVERT(varchar(50), RoleID), @BeginDateString = CONVERT(varchar(30), BeginDate, 25), @EndDateString = CONVERT(varchar(30), EndDate, 25) FROM @userRoles WHERE ID = @i;
		
		SET @UserRoleString = 'R' + CAST(LEN(@RoleIDString) AS varchar(10))
								+ ',' + CAST(LEN(@BeginDateString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ':' + @RoleIDString
								+ @BeginDateString
								+ @EndDateString;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserRoleString;

		SET @i = @i + 1;
	END

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalValues int;

	DECLARE @userValues table( ID int IDENTITY(1,1), UserID int, FieldID varchar(36), Value nvarchar(3950),PrivacyType tinyint);
	INSERT INTO @userValues SELECT UserID, ExtendedFieldID AS FieldID, RTRIM(LTRIM(Value)),PrivacyType FROM bx_UserExtendedValues WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [DELETED]);

	SET @TotalValues = @@ROWCOUNT;

	DECLARE
		@FieldIDString varchar(36),
		@ValueString nvarchar(3950),
		@PrivacyType varchar(10),
		@UserValueString varchar(4000);
	SET @i = 1;
	WHILE (@i <= @TotalValues) BEGIN
		SELECT @UserID = UserID, @FieldIDString = FieldID, @ValueString = Value ,@PrivacyType = PrivacyType  FROM @userValues WHERE ID = @i;
		
		SET @UserValueString = 'F' + CAST(LEN(@FieldIDString) AS varchar(10))
								+ ',' + CAST(LEN(@ValueString) AS varchar(10))
								+ ',' + CAST(LEN(@PrivacyType) AS varchar(10))
								+ ':' + @FieldIDString
								+ @ValueString
								+ @PrivacyType;

		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserValueString;

		SET @i = @i + 1;
	END

	--以下是处理用户勋章

	DECLARE @TotalMedals int;

	DECLARE @userMedals table( ID int IDENTITY(1,1), UserID int, MedalID int, MedalLevelID int, EndDate datetime, CreateDate datetime, Url nvarchar(200));
	INSERT INTO @userMedals SELECT UserID, MedalID, MedalLevelID, EndDate, CreateDate, ISNULL(Url,'') FROM bx_UserMedals WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [DELETED]);

	SET @TotalMedals = @@ROWCOUNT;

	DECLARE
		@MedalIDString varchar(50),
		@MedalLevelIDString varchar(50),
		@CreateDateString varchar(30),
		@Url nvarchar(200),
		@UserMedalString nvarchar(500);
	SET @i = 1;
	WHILE (@i <= @TotalMedals) BEGIN
		SELECT @UserID = UserID, @MedalIDString = CONVERT(varchar(50), MedalID), @MedalLevelIDString = CONVERT(varchar(50), MedalLevelID), @EndDateString = CONVERT(varchar(30), EndDate, 25), @CreateDateString = CONVERT(varchar(30), CreateDate, 25),@Url = Url FROM @userMedals WHERE ID = @i;
		
		SET @UserMedalString = 'M' + CAST(LEN(@MedalIDString) AS varchar(10))
								+ ',' + CAST(LEN(@MedalLevelIDString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ',' + CAST(LEN(@CreateDateString) AS varchar(10))
								+','  + CAST(LEN(@Url) AS varchar(10))
								+ ':' + @MedalIDString
								+ @MedalLevelIDString
								+ @EndDateString
								+ @CreateDateString
								+ @Url;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserMedalString;

		SET @i = @i + 1;
	END




	UPDATE bx_Users
	   SET ExtendedData = T.Data
	   FROM #bx_tmp_usrext T
	   WHERE T.UserID = bx_Users.UserID;

END
GO

EXEC bx_Drop 'bx_UserRoles_AfterUpdate';

GO


CREATE TRIGGER [bx_UserRoles_AfterUpdate]
	ON [bx_UserRoles]
	AFTER INSERT, UPDATE
AS
BEGIN

	SET NOCOUNT ON;

	IF (OBJECT_ID('tempdb..#bx_tmp_usrext') IS NULL) BEGIN
		CREATE TABLE #bx_tmp_usrext ( UserID int, Data ntext DEFAULT('') );
	END
	ELSE BEGIN
		TRUNCATE TABLE #bx_tmp_usrext;
	END

	INSERT INTO #bx_tmp_usrext (UserID) SELECT DISTINCT UserID FROM [INSERTED];

	DECLARE @UserID int;
	DECLARE @i int;
	DECLARE @ptr binary(16);

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalRoles int;

	DECLARE @userRoles table( ID int IDENTITY(1,1), UserID int, RoleID uniqueidentifier, BeginDate datetime, EndDate datetime );
	INSERT INTO @userRoles SELECT UserID, RoleID, BeginDate, EndDate FROM bx_UserRoles WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [INSERTED]);

	SET @TotalRoles = @@ROWCOUNT;

	DECLARE
		@RoleIDString varchar(50),
		@BeginDateString varchar(30),
		@EndDateString varchar(30),
		@UserRoleString varchar(100);
	SET @i = 1;
	WHILE (@i <= @TotalRoles) BEGIN
		SELECT @UserID = UserID, @RoleIDString = CONVERT(varchar(50), RoleID), @BeginDateString = CONVERT(varchar(30), BeginDate, 25), @EndDateString = CONVERT(varchar(30), EndDate, 25) FROM @userRoles WHERE ID = @i;
		
		SET @UserRoleString = 'R' + CAST(LEN(@RoleIDString) AS varchar(10))
								+ ',' + CAST(LEN(@BeginDateString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ':' + @RoleIDString
								+ @BeginDateString
								+ @EndDateString;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserRoleString;

		SET @i = @i + 1;
	END

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalValues int;

	DECLARE @userValues table( ID int IDENTITY(1,1), UserID int, FieldID varchar(36), Value nvarchar(3950),PrivacyType tinyint);
	INSERT INTO @userValues SELECT UserID, ExtendedFieldID AS FieldID, RTRIM(LTRIM(Value)),PrivacyType FROM bx_UserExtendedValues WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [INSERTED]);

	SET @TotalValues = @@ROWCOUNT;

	DECLARE
		@FieldIDString varchar(36),
		@ValueString nvarchar(3950),
		@PrivacyType varchar(10),
		@UserValueString varchar(4000);
	SET @i = 1;
	WHILE (@i <= @TotalValues) BEGIN
		SELECT @UserID = UserID, @FieldIDString = FieldID, @ValueString = Value ,@PrivacyType = PrivacyType  FROM @userValues WHERE ID = @i;
		
		SET @UserValueString = 'F' + CAST(LEN(@FieldIDString) AS varchar(10))
								+ ',' + CAST(LEN(@ValueString) AS varchar(10))
								+ ',' + CAST(LEN(@PrivacyType) AS varchar(10))
								+ ':' + @FieldIDString
								+ @ValueString
								+ @PrivacyType;

		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserValueString;

		SET @i = @i + 1;
	END

	--以下是处理用户勋章

	DECLARE @TotalMedals int;

	DECLARE @userMedals table( ID int IDENTITY(1,1), UserID int, MedalID int, MedalLevelID int, EndDate datetime, CreateDate datetime, Url nvarchar(200));
	INSERT INTO @userMedals SELECT UserID, MedalID, MedalLevelID, EndDate, CreateDate, ISNULL(Url,'') FROM bx_UserMedals WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM [INSERTED]);

	SET @TotalMedals = @@ROWCOUNT;

	DECLARE
		@MedalIDString varchar(50),
		@MedalLevelIDString varchar(50),
		@CreateDateString varchar(30),
		@Url nvarchar(200),
		@UserMedalString nvarchar(500);
	SET @i = 1;
	WHILE (@i <= @TotalMedals) BEGIN
		SELECT @UserID = UserID, @MedalIDString = CONVERT(varchar(50), MedalID), @MedalLevelIDString = CONVERT(varchar(50), MedalLevelID), @EndDateString = CONVERT(varchar(30), EndDate, 25), @CreateDateString = CONVERT(varchar(30), CreateDate, 25),@Url = Url FROM @userMedals WHERE ID = @i;
		
		SET @UserMedalString = 'M' + CAST(LEN(@MedalIDString) AS varchar(10))
								+ ',' + CAST(LEN(@MedalLevelIDString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ',' + CAST(LEN(@CreateDateString) AS varchar(10))
								+','  + CAST(LEN(@Url) AS varchar(10))
								+ ':' + @MedalIDString
								+ @MedalLevelIDString
								+ @EndDateString
								+ @CreateDateString
								+ @Url;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserMedalString;

		SET @i = @i + 1;
	END




	UPDATE bx_Users
	   SET ExtendedData = T.Data
	   FROM #bx_tmp_usrext T
	   WHERE T.UserID = bx_Users.UserID;

END
GO

EXEC bx_Drop 'bx_Users_AfterInsert';

GO


CREATE TRIGGER [bx_Users_AfterInsert]
	ON [bx_Users]
	AFTER INSERT
AS
BEGIN


	SET NOCOUNT ON;
	
	DECLARE @NewUserID int, @NewUsername nvarchar(50), @Count int;
	
	SELECT @Count = Count(*) FROM [INSERTED] WHERE [IsActive] = 1 AND [UserID] <> 0;
	
	IF @Count = 0
		RETURN;
		
	SELECT TOP 1 @NewUserID = UserID, @NewUsername = Username FROM [INSERTED] WHERE [IsActive] = 1 ORDER BY [UserID] DESC; 

	UPDATE [bx_Vars] SET  NewUserID = @NewUserID, NewUsername = @NewUsername, TotalUsers = TotalUsers + @Count;
	
	IF @@ROWCOUNT = 0 BEGIN
		DECLARE @TotalUsers int;
		SELECT @TotalUsers = COUNT(*) FROM [bx_Users] WITH (NOLOCK) WHERE [IsActive] = 1 AND [UserID]<>0;
		INSERT [bx_Vars] (NewUserID, NewUsername, TotalUsers) VALUES (@NewUserID, @NewUsername, @TotalUsers);
	END
	
	SELECT 'ResetVars' AS XCMD;
	
END



GO
 
EXEC bx_Drop 'bx_Users_Exp_AfterUpdate';

GO


CREATE TRIGGER [bx_Users_Exp_AfterUpdate]
	ON [bx_Users]
	AFTER UPDATE
AS
BEGIN
	-- 注意  修改以下  内容要相应修改 UserDao.cs 里的 UpdatePointsExpression(string expression)

	SET NOCOUNT ON;

	/* 如果是更新积分 不在这里更新总积分
	IF (UPDATE ([Point_1]) OR UPDATE ([Point_2])) BEGIN
		DECLARE @MaxValue int;
		SET @MaxValue = 2147483647;
		SET ARITHABORT OFF;
		SET ANSI_WARNINGS OFF;
		
		UPDATE bx_Users SET Points = ISNULL([Point_1]+[Point_2]*10,@MaxValue) WHERE [UserID] IN(SELECT DISTINCT [UserID] FROM [INSERTED]);
	END
	*/
	
	IF (UPDATE([IsActive])) BEGIN
			DECLARE @NewUserID int,@NewUsername nvarchar(50),@DeletedCount int,@InsertCount int;
			
			SELECT @InsertCount=COUNT(*) FROM [INSERTED] WHERE [IsActive]=1;
			SELECT @DeletedCount=COUNT(*) FROM [DELETED] WHERE [IsActive]=1;
			
			SELECT TOP 1 @NewUserID = UserID,@NewUsername = Username FROM [bx_Users] WITH (NOLOCK) WHERE [IsActive] = 1 ORDER BY [UserID] DESC;
			
			UPDATE [bx_Vars] SET  NewUserID = @NewUserID, NewUsername = @NewUsername, TotalUsers = TotalUsers + @InsertCount - @DeletedCount WHERE [ID]=(SELECT TOP 1 ID FROM [bx_Vars]);

			IF @@ROWCOUNT = 0 BEGIN
				DECLARE @TotalUsers int;
				SELECT @TotalUsers = COUNT(*) FROM [bx_Users] WITH (NOLOCK) WHERE [IsActive] = 1 AND [UserID]<>0;
				INSERT [bx_Vars] (NewUserID,NewUsername,TotalUsers)VALUES(@NewUserID,@NewUsername,@TotalUsers);
			END
			
			SELECT 'ResetVars' AS XCMD;
	END
	
END



GO

EXEC bx_Drop 'bx_Users_InsteadOfDelete';

GO


CREATE TRIGGER [bx_Users_InsteadOfDelete]
	ON [bx_Users]
	INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;
	DELETE [bx_Users] WHERE [UserID] IN (SELECT [UserID] FROM [DELETED]);
	
	
	DELETE bx_Denouncings WHERE Type=4 AND TargetID IN (SELECT [UserID] FROM [DELETED]);
	
END
GO
----最近访客触发器
--CREATE TRIGGER [bx_Visitors_AfterInsert]
	--ON [bx_Visitors]
	--AFTER INSERT
--AS
--BEGIN

	--SET NOCOUNT ON;

	--DECLARE @tempTable table(UserID int, VisitorCount int);

	--INSERT INTO @tempTable 
		--SELECT UserID ,COUNT(*)
		--FROM [INSERTED] T
		--GROUP BY UserID;

	--UPDATE [bx_UserInfos]
		--SET
			--TotalVisitors = TotalVisitors + VisitorCount
		--FROM @tempTable T
		--WHERE
			--T.UserID = [bx_UserInfos].UserID;

--END
GO
ALTER TABLE [bx_AdminSessions] ADD 
	CONSTRAINT [FK_bx_AdminSessions_UserID] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [bx_Users] (
		[UserID]
	) ON DELETE CASCADE 
GO
EXEC bx_Drop 'FK_bx_AlbumReverters_AlbumID';

ALTER TABLE [bx_AlbumReverters] ADD 
CONSTRAINT [FK_bx_AlbumReverters_AlbumID] FOREIGN KEY ([AlbumID]) REFERENCES [bx_Albums] ([AlbumID]) ON UPDATE CASCADE ON DELETE CASCADE

GO
GO
--用户 相册表外键关系
EXEC bx_Drop 'FK_bx_Albums_UserID';

ALTER TABLE [bx_Albums] ADD 
     CONSTRAINT [FK_bx_Albums_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE
     
GO

GO

EXEC bx_Drop 'FK_bx_AttachmentExchanges_AttachmentID';

ALTER TABLE [bx_AttachmentExchanges] ADD 
   CONSTRAINT [FK_bx_AttachmentExchanges_AttachmentID]        FOREIGN KEY ([AttachmentID]) REFERENCES [bx_Attachments] ([AttachmentID]) ON DELETE CASCADE ON UPDATE CASCADE
GO


GO

EXEC bx_Drop 'FK_bx_Attachments_PostID';

ALTER TABLE [bx_Attachments] ADD 
   CONSTRAINT [FK_bx_Attachments_PostID]        FOREIGN KEY ([PostID]) REFERENCES [bx_Posts] ([PostID]) ON DELETE CASCADE ON UPDATE CASCADE
GO


GO
ALTER TABLE [bx_AuthenticUsers]  WITH CHECK ADD  CONSTRAINT [FK_bx_AuthenticUsers_bx_Users] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
EXEC bx_Drop 'FK_bx_BannedUsers_UserID';

ALTER TABLE [bx_BannedUsers] ADD 
CONSTRAINT [FK_bx_BannedUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [bx_Users] ([UserID]) ON DELETE CASCADE

GO
GO
EXEC bx_Drop 'FK_bx_BanUserForumInfoLogs_LogID';

ALTER TABLE [bx_BanUserLogForumInfos] ADD
	CONSTRAINT [FK_bx_BanUserLogForumInfos_LogID]			FOREIGN	KEY([LogID])			REFERENCES [bx_BanUserLogs]		([LogID])	ON UPDATE CASCADE	ON DELETE CASCADE
GO
EXEC bx_Drop 'FK_bx_BlogArticleReverters_ArticleID';

ALTER TABLE [bx_BlogArticleReverters] ADD 
CONSTRAINT [FK_bx_BlogArticleReverters_ArticleID] FOREIGN KEY ([ArticleID]) REFERENCES [bx_BlogArticles] ([ArticleID]) ON UPDATE CASCADE ON DELETE CASCADE

GO
GO
EXEC bx_Drop 'FK_bx_BlogArticles_CategoryID';

ALTER TABLE [bx_BlogArticles] ADD 
CONSTRAINT [FK_bx_BlogArticles_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [bx_BlogCategories] ([CategoryID]) ON UPDATE CASCADE ON DELETE CASCADE

GO
GO
--日志访问者记录表的日志ID外键关系
--日志访问者记录的用户ID外键关系
EXEC bx_Drop 'FK_bx_BlogArticleVisitors_BlogArticleID';
EXEC bx_Drop 'FK_bx_BlogArticleVisitors_UserID';

ALTER TABLE [bx_BlogArticleVisitors] ADD
        CONSTRAINT [FK_bx_BlogArticleVisitors_BlogArticleID]        FOREIGN KEY ([BlogArticleID])      REFERENCES [bx_BlogArticles]     ([ArticleID])         ON DELETE CASCADE   
       ,CONSTRAINT [FK_bx_BlogArticleVisitors_UserID]               FOREIGN KEY ([UserID])             REFERENCES [bx_Users]            ([UserID])          

GO
GO
--日志分类 用户关系表外键关系
EXEC bx_Drop 'FK_bx_BlogCategories_UserID';

ALTER TABLE [bx_BlogCategories] ADD 
      CONSTRAINT [FK_bx_BlogCategories_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO
GO
EXEC bx_Drop 'FK_bx_BlogCategoryReverters_CategoryID';

ALTER TABLE [bx_BlogCategoryReverters] ADD 
CONSTRAINT [FK_bx_BlogCategoryReverters_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [bx_BlogCategories] ([CategoryID]) ON UPDATE CASCADE ON DELETE CASCADE

GO
GO
EXEC bx_Drop 'FK_bx_ChatMessageReverters_MessageID';

ALTER TABLE [bx_ChatMessageReverters]  WITH CHECK ADD  CONSTRAINT [FK_bx_ChatMessageReverters_MessageID] FOREIGN KEY([MessageID])
REFERENCES [bx_ChatMessages] ([MessageID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
EXEC bx_Drop 'FK_bx_ChatMessages_UserID';

ALTER TABLE [bx_ChatMessages]  WITH CHECK ADD  CONSTRAINT [FK_bx_ChatMessages_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
EXEC bx_Drop 'FK_bx_ChatSessions_UserID';

ALTER TABLE [bx_ChatSessions]  WITH CHECK ADD  CONSTRAINT [FK_bx_ChatSessions_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
EXEC bx_Drop 'FK_bx_ClubMembers_ClubID';
EXEC bx_Drop 'FK_bx_ClubMembers_UserID';

ALTER TABLE [bx_ClubMembers] ADD
CONSTRAINT [FK_bx_ClubMembers_ClubID] FOREIGN KEY ([ClubID]) REFERENCES [bx_Clubs] ([ClubID]) ON UPDATE CASCADE ON DELETE CASCADE,
CONSTRAINT [FK_bx_ClubMembers_UserID] FOREIGN KEY ([UserID]) REFERENCES [bx_Users] ([UserID]) ON UPDATE CASCADE ON DELETE CASCADE

GO

GO
EXEC bx_Drop 'FK_bx_Clubs_CategoryID';

ALTER TABLE [bx_Clubs] ADD 
CONSTRAINT [FK_bx_Clubs_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [bx_ClubCategories] ([CategoryID]) ON UPDATE CASCADE ON DELETE CASCADE
GO

GO
EXEC bx_Drop 'FK_bx_CommentReverters_CommentID';

ALTER TABLE [bx_CommentReverters]  WITH CHECK ADD  CONSTRAINT [FK_bx_CommentReverters_CommentID] FOREIGN KEY([CommentID])
REFERENCES [bx_Comments] ([CommentID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
--评论 用户关系表外键关系
EXEC bx_Drop 'FK_bx_Comments_UserID';

ALTER TABLE [bx_Comments] ADD 
      CONSTRAINT [FK_bx_Comments_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO
GO
EXEC bx_Drop 'FK_bx_DenouncingContents_DenouncingID';

ALTER TABLE [bx_DenouncingContents]  WITH CHECK ADD  CONSTRAINT [FK_bx_DenouncingContents_DenouncingID] FOREIGN KEY([DenouncingID])
REFERENCES [bx_Denouncings] ([DenouncingID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
--网络硬盘文件夹表外键关系

EXEC bx_Drop 'FK_bx_DiskDirectories_UserID';

ALTER TABLE [bx_DiskDirectories] ADD
     CONSTRAINT [FK_bx_DiskDirectories_UserID]           FOREIGN KEY ([UserID])          REFERENCES [bx_Users]            ([UserID])      ON UPDATE CASCADE    ON DELETE CASCADE
GO
GO
--网络硬盘文件表外键关系
--EXEC bx_Drop 'FK_bx_DiskFiles_FileID';
EXEC bx_Drop 'FK_bx_DiskFiles_DirectoryID';
--EXEC bx_Drop 'FK_bx_DiskFiles_UserID';

ALTER TABLE [bx_DiskFiles] ADD 
     --CONSTRAINT [FK_bx_DiskFiles_FileID]        FOREIGN KEY ([FileID])         REFERENCES [bx_Files]           ([ID])      ON UPDATE CASCADE    ON DELETE CASCADE,
    CONSTRAINT [FK_bx_DiskFiles_DirectoryID]   FOREIGN KEY ([DirectoryID])    REFERENCES [bx_DiskDirectories] ([DirectoryID])      ON UPDATE CASCADE    ON DELETE CASCADE
GO

GO
EXEC bx_Drop 'FK_bx_DoingReverters_DoingID';

ALTER TABLE [bx_DoingReverters] ADD 
CONSTRAINT [FK_bx_DoingReverters_DoingID] FOREIGN KEY ([DoingID]) REFERENCES [bx_Doings] ([DoingID]) ON UPDATE CASCADE ON DELETE CASCADE

GO
GO
--用户 相册关系表外键关系
EXEC bx_Drop 'FK_bx_Doings_UserID';

ALTER TABLE [bx_Doings] ADD 
      CONSTRAINT [FK_bx_Doings_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO
GO
ALTER TABLE [bx_EmoticonGroups] ADD 
	CONSTRAINT [FK_bx_EmoticonGroup_UserID] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [bx_Users] (
		[UserID]
	) ON DELETE CASCADE 
GO


GO
ALTER TABLE [bx_Emoticons] ADD 
	CONSTRAINT [FK_bx_Emoticons_GroupID] FOREIGN KEY 
	(
		[GroupID]
	) REFERENCES [bx_EmoticonGroups] (
		[GroupID]
	) ON DELETE CASCADE 
GO
GO
--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_FeedFilters_UserID';

ALTER TABLE [bx_FeedFilters] ADD 
   CONSTRAINT [FK_bx_FeedFilters_UserID]        FOREIGN KEY ([UserID])         REFERENCES [bx_Users]         ([UserID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
--好友 用户关系表外键关系
EXEC bx_Drop 'FK_bx_FriendGroups_UserID';

ALTER TABLE [bx_FriendGroups] ADD 
       CONSTRAINT [FK_bx_FriendGroups_UserID]          FOREIGN KEY ([UserID])          REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO
GO
--好友 好友组关系表外键关系
EXEC bx_Drop 'FK_bx_Friends_GroupID';

ALTER TABLE [bx_Friends] ADD 
       CONSTRAINT [FK_bx_Friends_GroupID]        FOREIGN KEY ([GroupID])       REFERENCES [bx_FriendGroups]    ([GroupID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO
GO
EXEC bx_Drop 'FK_bx_ImpressionRecords_TypeID';

ALTER TABLE [bx_ImpressionRecords] ADD 
CONSTRAINT [FK_bx_ImpressionRecords_TypeID] FOREIGN KEY ([TypeID]) REFERENCES [bx_ImpressionTypes] ([TypeID]) ON UPDATE CASCADE ON DELETE CASCADE

EXEC bx_Drop 'FK_bx_ImpressionRecords_UserID';

ALTER TABLE [bx_ImpressionRecords] ADD
CONSTRAINT [FK_bx_ImpressionRecords_UserID] FOREIGN KEY ([UserID]) REFERENCES [bx_Users] ([UserID]) ON UPDATE CASCADE ON DELETE CASCADE

GO

GO
EXEC bx_Drop 'FK_bx_Impressions_TypeID';

ALTER TABLE [bx_Impressions] ADD 
CONSTRAINT [FK_bx_Impressions_TypeID] FOREIGN KEY ([TypeID]) REFERENCES [bx_ImpressionTypes] ([TypeID]) ON UPDATE CASCADE ON DELETE CASCADE

EXEC bx_Drop 'FK_bx_Impressions_UserID';

ALTER TABLE [bx_Impressions] ADD 
CONSTRAINT [FK_bx_Impressions_UserID] FOREIGN KEY ([UserID]) REFERENCES [bx_Users] ([UserID]) ON UPDATE CASCADE ON DELETE CASCADE

GO

GO
EXEC bx_Drop 'FK_bx_InviteSerials_UserID';

ALTER TABLE [bx_InviteSerials]  WITH CHECK ADD  CONSTRAINT [FK_bx_InviteSerials_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
EXEC bx_Drop '[FK_bx_Moderators_bx_Forums]';

ALTER TABLE [bx_Moderators] ADD 
	CONSTRAINT [FK_bx_Moderators_bx_Forums] FOREIGN KEY 
	(
		[ForumID]
	) REFERENCES [bx_Forums] (
		[ForumID]
	) ON DELETE CASCADE ,
	CONSTRAINT [FK_bx_Moderators_bx_Users] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [bx_Users] (
		[UserID]
	) ON DELETE CASCADE 
GO
--外键关系
EXEC bx_Drop 'FK_bx_Notify_UserID';
EXEC bx_Drop 'FK_bx_Notify_TypeID';

ALTER TABLE [bx_Notify] ADD 
     CONSTRAINT [FK_bx_Notify_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE,
     CONSTRAINT [FK_bx_Notify_TypeID]    FOREIGN KEY ([TypeID])    REFERENCES [bx_NotifyTypes] ([TypeID]) ON UPDATE CASCADE  ON DELETE CASCADE,
     CONSTRAINT [FK_bx_Notify_Client]    FOREIGN KEY ([ClientID])  REFERENCES [bx_PassportClients] ([ClientID]) ON UPDATE CASCADE  ON DELETE CASCADE
GO
GO
--用户 相册关系表外键关系
EXEC bx_Drop 'FK_bx_Pay_UserID';

ALTER TABLE [bx_Pay] ADD 
      CONSTRAINT [FK_bx_Pay_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO
GO
EXEC bx_Drop 'FK_bx_PhotoReverters_PhotoID';

ALTER TABLE [bx_PhotoReverters] ADD 
CONSTRAINT [FK_bx_PhotoReverters_PhotoID] FOREIGN KEY ([PhotoID]) REFERENCES [bx_Photos] ([PhotoID]) ON UPDATE CASCADE ON DELETE CASCADE

GO
GO
--相片 相册关系表外键关系
--相片 文件关系表外键关系
EXEC bx_Drop 'FK_bx_Photos_AlbumID';
EXEC bx_Drop 'FK_bx_Photos_FileID';

ALTER TABLE [bx_Photos] ADD 
     
    CONSTRAINT [FK_bx_Photos_AlbumID]             FOREIGN KEY ([AlbumID])    REFERENCES [bx_Albums]       ([AlbumID])    ON UPDATE CASCADE  ON DELETE CASCADE
    --,CONSTRAINT [FK_bx_Photos_FileID]              FOREIGN KEY ([FileID])     REFERENCES [bx_Files]        ([FileID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO

GO
EXEC bx_Drop 'FK_bx_PointLogs_UserID';

ALTER TABLE [bx_PointLogs] ADD 
CONSTRAINT [FK_bx_PointLogs_UserID] FOREIGN KEY ([UserID]) REFERENCES [bx_Users] ([UserID]) ON DELETE CASCADE,
CONSTRAINT [FK_bx_PointLogs_OperateID] FOREIGN KEY ([OperateID]) REFERENCES [bx_PointLogTypes] ([OperateID])

GO
GO
EXEC bx_Drop 'FK_bx_PointShows_UserID';

ALTER TABLE [bx_PointShows]  WITH CHECK ADD  CONSTRAINT [FK_bx_PointShows_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_Polemizes_ThreadID';

ALTER TABLE [bx_Polemizes] ADD 
   CONSTRAINT [FK_bx_Polemizes_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO

EXEC bx_Drop 'FK_bx_PolemizeUsers_ThreadID';

ALTER TABLE [bx_PolemizeUsers] ADD 
   CONSTRAINT [FK_bx_PolemizeUsers_ThreadID] FOREIGN KEY([ThreadID]) REFERENCES [bx_Threads] ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO

EXEC bx_Drop 'FK_bx_PollItemDetails_ItemID';

ALTER TABLE [bx_PollItemDetails] ADD 
   CONSTRAINT [FK_bx_PollItemDetails_ItemID]        FOREIGN KEY ([ItemID]) REFERENCES [bx_PollItems] ([ItemID]) ON DELETE CASCADE ON UPDATE CASCADE

GO


GO

EXEC bx_Drop 'FK_bx_PollItems_ThreadID';

ALTER TABLE [bx_PollItems] ADD 
   CONSTRAINT [FK_bx_PollItems_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Polls]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO

EXEC bx_Drop 'FK_bx_Polls_ThreadID';

ALTER TABLE [bx_Polls] ADD 
   CONSTRAINT [FK_bx_Polls_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
EXEC bx_Drop 'FK_bx_PostLoveHates_UserID';

ALTER TABLE [bx_PostLoveHates]  WITH CHECK ADD  CONSTRAINT [FK_bx_PostLoveHates_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE

EXEC bx_Drop 'FK_bx_PostLoveHates_PostID';

ALTER TABLE [bx_PostLoveHates]  WITH CHECK ADD  CONSTRAINT [FK_bx_PostLoveHates_PostID] FOREIGN KEY([PostID])
REFERENCES [bx_Posts] ([PostID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
EXEC bx_Drop 'FK_bx_PostMarks_PostID';

ALTER TABLE [bx_PostMarks]  WITH CHECK ADD  CONSTRAINT [FK_bx_PostMarks_PostID] FOREIGN KEY([PostID])
REFERENCES [bx_Posts] ([PostID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
--用户用户分享关系表外键关系
EXEC bx_Drop 'FK_bx_PostReverters_PostID';

ALTER TABLE [bx_PostReverters] ADD 
     CONSTRAINT [FK_bx_PostReverters_PostID]        FOREIGN KEY ([PostID])         REFERENCES [bx_Posts]         ([PostID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_Posts_ThreadID';

ALTER TABLE [bx_Posts] ADD 
   CONSTRAINT [FK_bx_Posts_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO

ALTER TABLE bx_PropLogs ADD CONSTRAINT FK_bx_PropLogs_UserID FOREIGN KEY (UserID) REFERENCES bx_Users (UserID) ON UPDATE CASCADE ON DELETE CASCADE

GO

EXEC bx_Drop 'FK_bx_QuestionRewards_ThreadID';

ALTER TABLE [bx_QuestionRewards] ADD 
   CONSTRAINT [FK_bx_QuestionRewards_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Questions]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO

EXEC bx_Drop 'FK_bx_Questions_ThreadID';

ALTER TABLE [bx_Questions] ADD 
   CONSTRAINT [FK_bx_Questions_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO

EXEC bx_Drop 'FK_bx_QuestionUsers_ThreadID';

ALTER TABLE [bx_QuestionUsers] ADD 
   CONSTRAINT [FK_bx_QuestionUsers_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Questions]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
EXEC bx_Drop 'FK_bx_RecoverPasswordLogs_bx_Users';

ALTER TABLE bx_RecoverPasswordLogs  WITH CHECK ADD  CONSTRAINT [FK_bx_RecoverPasswordLogs_bx_Users] FOREIGN KEY([UserID])
REFERENCES bx_Users([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE bx_RecoverPasswordLogs CHECK CONSTRAINT [FK_bx_RecoverPasswordLogs_bx_Users]
GO
GO
EXEC bx_Drop 'FK_bx_Serials_UserID';

ALTER TABLE [bx_Serials]  WITH CHECK ADD  CONSTRAINT [FK_bx_Serials_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
EXEC bx_Drop 'FK_bx_ShareReverters_ShareID';

ALTER TABLE [bx_ShareReverters] ADD 
CONSTRAINT [FK_bx_ShareReverters_ShareID] FOREIGN KEY ([ShareID]) REFERENCES [bx_Shares] ([ShareID]) ON UPDATE CASCADE ON DELETE CASCADE

GO

ALTER TABLE [bx_UserShareReverters] ADD 
CONSTRAINT [FK_bx_UserShareReverters_UserShareID] FOREIGN KEY ([UserShareID]) REFERENCES [bx_UserShares] ([UserShareID]) ON UPDATE CASCADE ON DELETE CASCADE

GO
GO
--用户用户分享关系表外键关系

ALTER TABLE bx_UserShares ADD 
	CONSTRAINT [FK_bx_UserShares_UserID] FOREIGN KEY (UserID) REFERENCES bx_Users (UserID) ON UPDATE CASCADE ON DELETE CASCADE;

go

ALTER TABLE bx_UserShares ADD 
	CONSTRAINT [FK_bx_UserShares_ShareID] FOREIGN KEY (ShareID) REFERENCES bx_Shares (ShareID) ON UPDATE CASCADE ON DELETE CASCADE;

go
GO
----空间 用户关系表外键关系
--EXEC bx_Drop 'FK_bx_Spaces_UserID';

--ALTER TABLE [bx_Spaces] ADD 
      --CONSTRAINT [FK_bx_Spaces_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE

--GO
GO

EXEC bx_Drop 'FK_bx_StickThreads_ThreadID';
EXEC bx_Drop 'FK_bx_StickThreads_ForumID';

ALTER TABLE [bx_StickThreads] ADD 
     CONSTRAINT [FK_bx_StickThreads_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])    ON UPDATE CASCADE  ON DELETE CASCADE
     ,CONSTRAINT [FK_bx_StickThreads_ForumID]        FOREIGN KEY ([ForumID])         REFERENCES [bx_Forums]         ([ForumID])

GO

GO
--标签表外键关系
EXEC bx_Drop 'FK_bx_TagRelation_TagID';

ALTER TABLE [bx_TagRelation] ADD 
     CONSTRAINT [FK_bx_TagRelation_TagID]            FOREIGN KEY ([TagID])            REFERENCES [bx_Tags]            ([ID])    ON UPDATE CASCADE  ON DELETE CASCADE
GO

GO
EXEC bx_Drop 'FK_bx_TempUploadFiles_UserID';

ALTER TABLE [bx_TempUploadFiles]  WITH CHECK ADD  CONSTRAINT [FK_bx_TempUploadFiles_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

EXEC bx_Drop 'FK_bx_ThreadCatalogsInForums_ThreadCatalogID';

ALTER TABLE [bx_ThreadCatalogsInForums] ADD
	CONSTRAINT [FK_bx_ThreadCatalogsInForums_ForumID]        FOREIGN KEY ([ForumID])         REFERENCES [bx_Forums]         ([ForumID])   ON UPDATE CASCADE  ON DELETE CASCADE
	,CONSTRAINT [FK_bx_ThreadCatalogsInForums_ThreadCatalogID]        FOREIGN KEY ([ThreadCatalogID])         REFERENCES [bx_ThreadCatalogs]         ([ThreadCatalogID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
EXEC bx_Drop 'FK_bx_ThreadCateModels_CateID';

ALTER TABLE [bx_ThreadCateModels]  WITH CHECK ADD  CONSTRAINT [FK_bx_ThreadCateModels_CateID] FOREIGN KEY([CateID])
REFERENCES [bx_ThreadCates] ([CateID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO

EXEC bx_Drop 'FK_bx_ThreadCateModelFields_ModelID';

ALTER TABLE [bx_ThreadCateModelFields]  WITH CHECK ADD  CONSTRAINT [FK_bx_ThreadCateModelFields_ModelID] FOREIGN KEY([ModelID])
REFERENCES [bx_ThreadCateModels] ([ModelID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO
GO

EXEC bx_Drop 'FK_bx_ThreadExchanges_ThreadID';

ALTER TABLE [bx_ThreadExchanges] ADD 
   CONSTRAINT [FK_bx_ThreadExchanges_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_ThreadImages_ThreadID';

ALTER TABLE [bx_ThreadImages] ADD 
   CONSTRAINT [FK_bx_ThreadImages_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO

EXEC bx_Drop 'FK_bx_ThreadRanks_ThreadID';

ALTER TABLE [bx_ThreadRanks] ADD 
   CONSTRAINT [FK_bx_ThreadRanks_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
--用户用户分享关系表外键关系
EXEC bx_Drop 'FK_bx_ThreadReverters_ThreadID';

ALTER TABLE [bx_ThreadReverters] ADD 
     CONSTRAINT [FK_bx_ThreadReverters_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO

EXEC bx_Drop 'FK_bx_Threads_ForumID';

ALTER TABLE [bx_Threads] ADD 
   CONSTRAINT [FK_bx_Threads_ForumID]        FOREIGN KEY ([ForumID])         REFERENCES [bx_Forums]         ([ForumID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_ThreadWords_ThreadID';

ALTER TABLE [bx_ThreadWords] ADD 
   CONSTRAINT [FK_bx_ThreadWords_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO

EXEC bx_Drop 'FK_TopicStatus_ThreadID';

ALTER TABLE [bx_TopicStatus] ADD 
     CONSTRAINT [FK_bx_TopicStatus_ThreadID]        FOREIGN KEY ([ThreadID])         REFERENCES [bx_Threads]         ([ThreadID])    ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
--外键关系
EXEC bx_Drop 'FK_bx_UnreadNotifies_UserID';
EXEC bx_Drop 'FK_bx_UnreadNotifies_TypeID';

ALTER TABLE [bx_UnreadNotifies] ADD 
     CONSTRAINT [FK_bx_UnreadNotifies_UserID]    FOREIGN KEY ([UserID])    REFERENCES [bx_Users]    ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE,
     CONSTRAINT [FK_bx_UnreadNotifies_TypeID]    FOREIGN KEY ([TypeID])    REFERENCES [bx_NotifyTypes] ([TypeID]) ON UPDATE CASCADE  ON DELETE CASCADE
GO
GO

--ALTER TABLE bx_UserAvatarLocks ADD CONSTRAINT FK_bx_UserAvatarLocks_UserID FOREIGN KEY (UserID) REFERENCES bx_Users (UserID) ON UPDATE CASCADE ON DELETE CASCADE

GO
--用户的扩展字段表外键关系
EXEC bx_Drop 'FK_bx_UserExtendedValues_UserID';
EXEC bx_Drop 'FK_bx_UserExtendedValues_ExtendedFieldID';

ALTER TABLE [bx_UserExtendedValues] ADD 
     CONSTRAINT [FK_bx_UserExtendedValues_UserID]            FOREIGN KEY ([UserID])          REFERENCES [bx_Users]            ([UserID])  ON UPDATE CASCADE ON DELETE CASCADE

GO


GO
--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_UserFeeds_FeedID';

ALTER TABLE [bx_UserFeeds] ADD 
   CONSTRAINT [FK_bx_UserFeeds_FeedID]        FOREIGN KEY ([FeedID])         REFERENCES [bx_Feeds]         ([ID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
EXEC bx_Drop 'FK_bx_UserInfos_UserID';

ALTER TABLE [bx_UserInfos]  WITH CHECK ADD  CONSTRAINT [FK_bx_UserInfos_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
--用户的扩展字段表外键关系
EXEC bx_Drop 'FK_bx_UserMedals_UserID';

ALTER TABLE [bx_UserMedals] ADD 
     CONSTRAINT [FK_bx_UserMedals_UserID]     FOREIGN KEY ([UserID])     REFERENCES [bx_Users]       ([UserID])  ON UPDATE CASCADE ON DELETE CASCADE

GO


GO
--用户用户任务关系表外键关系
EXEC bx_Drop 'FK_bx_UserMissions_UserID';
EXEC bx_Drop 'FK_bx_UserMissions_MissionID';

ALTER TABLE [bx_UserMissions] ADD 
     CONSTRAINT [FK_bx_UserMissions_UserID]        FOREIGN KEY ([UserID])         REFERENCES [bx_Users]         ([UserID])  ON UPDATE CASCADE  ON DELETE CASCADE
    ,CONSTRAINT [FK_bx_UserMissions_MissionID]     FOREIGN KEY ([MissionID])      REFERENCES [bx_Missions]      ([ID])  ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
EXEC bx_Drop '[FK_bx_SmsCodes_bx_Users]';

ALTER TABLE [bx_SmsCodes] ADD 
	CONSTRAINT [FK_bx_SmsCodes_bx_Users] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [bx_Users] (
		[UserID]
	) ON DELETE CASCADE 
GO
GO
--用户用户动态关系表外键关系
EXEC bx_Drop 'FK_bx_UserNoAddFeedApps_UserID';

ALTER TABLE [bx_UserNoAddFeedApps] ADD 
   CONSTRAINT [FK_bx_UserNoAddFeedApps_UserID]        FOREIGN KEY ([UserID])         REFERENCES [bx_Users]         ([UserID])   ON UPDATE CASCADE  ON DELETE CASCADE

GO


GO
ALTER TABLE bx_UserProps ADD CONSTRAINT FK_bx_UserProps_PropID FOREIGN KEY (PropID) REFERENCES bx_Props (PropID) ON UPDATE CASCADE ON DELETE CASCADE 

ALTER TABLE bx_UserProps ADD CONSTRAINT FK_bx_UserProps_UserID FOREIGN KEY (UserID) REFERENCES bx_Users (UserID) ON UPDATE CASCADE ON DELETE CASCADE

GO
EXEC bx_Drop 'FK_bx_UserReverters_UserID';

ALTER TABLE [bx_UserReverters]  WITH CHECK ADD  CONSTRAINT [FK_bx_UserReverters_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
--用户隶属的用户组和用户表的外键关系
EXEC bx_Drop 'FK_bx_UserRoles_UserID';

ALTER TABLE [bx_UserRoles] ADD 
     CONSTRAINT [FK_bx_UserRoles_UserID]     FOREIGN KEY ([UserID])     REFERENCES [bx_Users]       ([UserID])  ON UPDATE CASCADE ON DELETE CASCADE

GO


GO
--用户隶属的用户组和用户表的外键关系
EXEC bx_Drop 'FK_bx_UsersInRoles_UserID';

ALTER TABLE bx_UsersInRoles ADD 
     CONSTRAINT FK_bx_UsersInRoles_UserID     FOREIGN KEY ([UserID])     REFERENCES [bx_Users]       ([UserID])  ON UPDATE CASCADE ON DELETE CASCADE

GO


GO

EXEC bx_Drop 'FK_bx_UserTempData_bx_Users';


ALTER TABLE [bx_UserTempData] ADD 
	CONSTRAINT [FK_bx_UserTempData_bx_Users] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [bx_Users] (
		[UserID]
	) ON DELETE CASCADE 
GO

GO
EXEC bx_Drop 'FK_bx_UserVars_UserID';

ALTER TABLE [bx_UserVars]  WITH CHECK ADD  CONSTRAINT [FK_bx_UserVars_UserID] FOREIGN KEY([UserID])
REFERENCES [bx_Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
--访问者 用户关系表外键关系
EXEC bx_Drop 'FK_bx_Visitors_UserID';
EXEC bx_Drop 'FK_bx_Visitors_VisitorUserID';

ALTER TABLE [bx_Visitors] ADD 
      CONSTRAINT [FK_bx_Visitors_UserID]           FOREIGN KEY ([UserID])           REFERENCES [bx_Users]         ([UserID])    ON UPDATE CASCADE  ON DELETE CASCADE
     ,CONSTRAINT [FK_bx_Visitors_VisitorUserID]    FOREIGN KEY ([VisitorUserID])    REFERENCES [bx_Users]         ([UserID])   

GO

GO

-- 插入系统需要的一个默认用户 --
-- 插入管理员 --
SET IDENTITY_INSERT [bx_Users] ON;

INSERT INTO [bx_Users] ([UserID], [Username]) VALUES(0, '{#bxguest#}');
INSERT INTO [bx_Users] (UserID, Username, Email, Realname,TotalTopics,TotalPosts) VALUES (1, 'admin',  'admin@site.com', '',1,1);

SET IDENTITY_INSERT [bx_Users] OFF;

INSERT INTO [bx_UserVars]([UserID], [Password], [PasswordFormat]) VALUES (1, 'C4CA4238A0B923820DCC509A6F75849B', 3);

INSERT INTO [bx_UserInfos]([UserID]) VALUES (1);

INSERT INTO [bx_UserRoles] (UserID, RoleID, BeginDate, EndDate) VALUES (1, 'db0ca05e-c107-40f2-a52d-0d486b5d6cb0', '1793-1-1', '9999-12-31');

-- 插入系统需要的一个默认日志分组 --
SET IDENTITY_INSERT [bx_BlogCategories] ON;

INSERT INTO [bx_BlogCategories] ([CategoryID], [UserID], [Name]) VALUES (0, 0, '');

SET IDENTITY_INSERT [bx_BlogCategories] OFF;

-- 插入默认的群组分类 --
INSERT INTO [bx_ClubCategories] ([Name]) VALUES ('粉丝团');
INSERT INTO [bx_ClubCategories] ([Name]) VALUES ('俱乐部');


-- 插入系统需要的一个默认好友分组 --
SET IDENTITY_INSERT [bx_FriendGroups] ON;

INSERT INTO [bx_FriendGroups] ([GroupID], [UserID], [GroupName]) VALUES (0, 0, 'none');
INSERT INTO [bx_FriendGroups] ([GroupID], [UserID], [GroupName]) VALUES (-1, 0, 'blacklist');

SET IDENTITY_INSERT [bx_FriendGroups] OFF;

-- 插入默认版块 --
SET IDENTITY_INSERT [bx_Forums] ON
INSERT [bx_Forums] ([ForumID], [ParentID], [ForumType], [ForumStatus], [ThreadCatalogStatus], [CodeName], [ForumName], [Description], [Readme], [LogoSrc], [ThemeID], [ColumnSpan], [TotalThreads], [TotalPosts], [TodayThreads], [TodayPosts], [LastThreadID], [YestodayLastThreadID], [YestodayLastPostID], [Password], [SortOrder], [ExtendedAttributes]) VALUES (-2, 0, 0, 0, 0, N'UnApproved', N'待审核', N'待审核', N'待审核', N'', N'default', 0, 0, 0, 0, 0, 0, 0, 0, N'', 1, NULL)
INSERT [bx_Forums] ([ForumID], [ParentID], [ForumType], [ForumStatus], [ThreadCatalogStatus], [CodeName], [ForumName], [Description], [Readme], [LogoSrc], [ThemeID], [ColumnSpan], [TotalThreads], [TotalPosts], [TodayThreads], [TodayPosts], [LastThreadID], [YestodayLastThreadID], [YestodayLastPostID], [Password], [SortOrder], [ExtendedAttributes]) VALUES (-1, 0, 0, 0, 0, N'RecycleBin', N'回收站', N'所有被回收的主题都在此处', N'所有被回收的主题都在此处', N'', N'default', 0, 0, 0, 0, 0, 0, 0, 0, N'0', 0, NULL)
INSERT [bx_Forums] ([ForumID], [ParentID], [ForumType], [ForumStatus], [ThreadCatalogStatus], [CodeName], [ForumName], [Description], [Readme], [LogoSrc], [ThemeID], [ColumnSpan], [TotalThreads], [TotalPosts], [TodayThreads], [TodayPosts], [LastThreadID], [YestodayLastThreadID], [YestodayLastPostID], [Password], [SortOrder], [ExtendedAttributes]) VALUES (1, 0, 1, 0, 2, N'defaultCatalog', N'默认分类', N'默认分类', N'默认分类', N'', N'default', 0, 0, 0, 0, 0, 0, 0, 0, N'', 2, NULL)
INSERT [bx_Forums] ([ForumID], [ParentID], [ForumType], [ForumStatus], [ThreadCatalogStatus], [CodeName], [ForumName], [Description], [Readme], [LogoSrc], [ThemeID], [ColumnSpan], [TotalThreads], [TotalPosts], [TodayThreads], [TodayPosts], [LastThreadID], [YestodayLastThreadID], [YestodayLastPostID], [Password], [SortOrder], [ExtendedAttributes]) VALUES (2, 1, 0, 0, 1, N'defaultForum', N'默认版块', N'默认版块', N'默认版块', N'', N'default', 0, 1, 1, 0, 0, 1, 1, 1, N'', 0, NULL)
INSERT [bx_Forums] ([ForumID], [ParentID], [ForumType], [ForumStatus], [ThreadCatalogStatus], [CodeName], [ForumName], [Description], [Readme], [LogoSrc], [ThemeID], [ColumnSpan], [TotalThreads], [TotalPosts], [TodayThreads], [TodayPosts], [LastThreadID], [YestodayLastThreadID], [YestodayLastPostID], [Password], [SortOrder], [ExtendedAttributes]) VALUES (3, 1, 0, 0, 1, N'admin_discussion', N'评论版块', N'只有管理员能发帖，用户只能回帖。', N'只有管理员能发帖，用户只能回帖。', N'', N'default', 0, 0, 0, 0, 0, 0, 0, 0, N'', 2, NULL)
SET IDENTITY_INSERT [bx_Forums] OFF

-- 插入默认帖子 --
SET IDENTITY_INSERT [bx_Threads] ON
INSERT [bx_Threads] ([ThreadID], [ForumID], [ThreadCatalogID], [ThreadType], [IconID], [Subject], [SubjectStyle], [TotalReplies], [TotalViews], [TotalAttachments], [Price], [Rank], [PostUserID], [PostNickName], [LastPostUserID], [LastPostNickName], [IsLocked], [IsValued], [Perorate], [CreateDate], [UpdateDate], [SortOrder], [UpdateSortOrder], [ThreadLog], [JudgementID]) VALUES (1, 2, 1, 0, 0, N'欢迎使用 bbsmax', N'font-size:20px;font-weight:bold;color:red;', 0, 0, 0, 0, 0, 1, N'admin', 1, N'admin', 0, 0, N'', getdate(), getdate(), 111949481600000, 1, N'', 1)
SET IDENTITY_INSERT [bx_Threads] OFF


SET IDENTITY_INSERT [bx_Posts] ON
INSERT [bx_Posts] ([PostID], [ParentID], [ForumID], [ThreadID], [PostType], [IconID], [Subject], [Content], [ContentFormat], [EnableSignature], [EnableReplyNotice], [IsShielded], [UserID], [NickName], [LastEditorID], [LastEditor], [IPAddress], [CreateDate], [UpdateDate], [SortOrder],[MarkCount]) VALUES (1, NULL, 2, 1, 1, 0, N'欢迎加入 bbsmax 大家庭', N'尊敬的用户，您好，欢迎使用&nbsp;bbsmax&nbsp;，感谢您对我们的支持，这是我们继续工作的动力。<br/><br/>bbsmax&nbsp;&nbsp;的前身是&nbsp;nowboard，第一个版本发布于&nbsp;2002&nbsp;年，是中国最早的基于&nbsp;.net&nbsp;的论坛系统。<br/>bbsmax&nbsp;5&nbsp;版本是继4.0版本发布后，耗费1年经历，经过无数个日夜的艰苦奋斗的重大成果。如今，bbsmax已经发展成为非常强大的论坛、社区系统。<br/><br/>如果您有使用上的疑问，欢迎联系我们！<br/><br/>官方网站&nbsp;<a href="http://www.bbsmax.com/" target="_blank">http://www.bbsmax.com</a><br/>官方论坛&nbsp;<a href="http://bbs.bbsmax.com/" target="_blank">http://bbs.bbsmax.com</a><br/><br/><div align="right">bbsmax&nbsp;开发团队<br/></div>', 4, 1, 1, 0, 1, N'admin', 0, N'', N'127.0.0.1', getdate(), getdate(), 1119494816000003,1)
SET IDENTITY_INSERT [bx_Posts] OFF

SET IDENTITY_INSERT [bx_PostMarks] ON
INSERT [bx_PostMarks] ([PostMarkID], [PostID], [UserID], [Username], [CreateDate], [ExtendedPoints_1], [ExtendedPoints_2], [ExtendedPoints_3], [ExtendedPoints_4], [ExtendedPoints_5], [ExtendedPoints_6], [ExtendedPoints_7], [ExtendedPoints_8], [Reason]) VALUES (1, 1, 1, 'admin', CAST(0x00009AE80122251B AS DateTime), 10, 100, 0, 0, 0, 0, 0, 0, N'祝贺您选择了强大的bbsMax')
SET IDENTITY_INSERT [bx_PostMarks] OFF

-- 管理员默认点亮图标 --
INSERT INTO [bx_UserMedals](MedalID,MedalLevelID,UserID,EndDate)values(3,1,1,'9999-12-31');
-- 用户任务 --
DELETE FROM [bx_Missions]
SET IDENTITY_INSERT [bx_Missions] ON
INSERT [bx_Missions] ([ID], [CycleTime], [SortOrder], [TotalUsers], [Type], [Name], [IconUrl], [DeductPoint], [Prize], [Description], [ApplyCondition], [FinishCondition], [EndDate], [BeginDate], [CreateDate]) VALUES (1, 0, 101, 0, N'MaxLabs.bbsMax.Missions.AvatarMission', N'更新一下自己的头像', N'~/max-assets/icon-mission/avatar.gif', N'0,0,0,0,0,0,0,0', N'usergroupactivetimes:0;medalactivetimes:0;prizetypes:5;usergroupids:0;points:16;medalids:0;inviteserialcount:1|Point10,5,0,0,0,0,0,00', N'头像就是你在这里的个人形象。 <br />
设置好了可获得特别惊喜：<br />
本站会自动寻找优秀的异性朋友推荐给您。<br />
您不赶快试试？', N'totalposts:1;maxapplycount:1;othermissionids:0;totalpoint:1;onlinetime:1;usergroupids:0;points:15|00000,0,0,0,0,0,0,0', N'', CAST(0x002D247F018B81FF AS DateTime), getdate(), getdate())
INSERT [bx_Missions] ([ID], [CycleTime], [SortOrder], [TotalUsers], [Type], [Name], [IconUrl], [DeductPoint], [Prize], [Description], [ApplyCondition], [FinishCondition], [EndDate], [BeginDate], [CreateDate]) VALUES (2, 0, 99, 0, N'MaxLabs.bbsMax.Missions.ActiveEmailMission', N'验证激活自己的邮箱', N'~/max-assets/icon-mission/email.gif', N'0,0,0,0,0,0,0,0', N'usergroupactivetimes:0;medalactivetimes:0;prizetypes:5;usergroupids:0;points:16;medalids:0;inviteserialcount:1|Point0,10,0,0,0,0,0,00', N'填写自己真实的邮箱地址并验证通过。
您可以在忘记密码的时候使用该邮箱取回自己的密码；
还可以及时接受站内的好友通知等等。
这对您十分有帮助和必要。', N'totalposts:1;maxapplycount:1;othermissionids:0;totalpoint:1;onlinetime:1;usergroupids:0;points:15|00000,0,0,0,0,0,0,0', N'', CAST(0x002D247F018B81FF AS DateTime), getdate(), getdate())
INSERT [bx_Missions] ([ID], [CycleTime], [SortOrder], [TotalUsers], [Type], [Name], [IconUrl], [DeductPoint], [Prize], [Description], [ApplyCondition], [FinishCondition], [EndDate], [BeginDate], [CreateDate]) VALUES (4, 86400, 88, 1, N'MaxLabs.bbsMax.Missions.LoginMission', N'领取每日积分大礼包', N'~/max-assets/icon-mission/gift.gif', N'0,0,0,0,0,0,0,0', N'medalids:1;inviteserialcount:1;medallevelids:1;points:15;medalactivetimes:5;prizetypes:11;usergroupactivetimes:0;usergroupids:0|2015,0,0,0,0,0,0,086400Point,Medal', N'每天登录访问自己的主页，就可领取积分大礼包', N'totalposts:1;maxapplycount:1;othermissionids:0;totalpoint:1;onlinetime:1;usergroupids:0;points:15|00000,0,0,0,0,0,0,0', N'', CAST(0x002D247F018B81FF AS DateTime), getdate(), getdate())
INSERT [bx_Missions] ([ID], [CycleTime], [SortOrder], [TotalUsers], [Type], [Name], [IconUrl], [DeductPoint], [Prize], [Description], [ApplyCondition], [FinishCondition], [EndDate], [BeginDate], [CreateDate]) VALUES (5, 0, 122, 0, N'MaxLabs.bbsMax.Missions.BlogMission', N'发表自己的第一篇日志', N'~/max-assets/icon-mission/blog.gif', N'0,0,0,0,0,0,0,0', N'usergroupactivetimes:0;medalactivetimes:0;prizetypes:5;usergroupids:0;points:16;medalids:0;inviteserialcount:1|Point10,0,0,0,0,0,0,00', N'现在，就写下自己的第一篇日志吧。
与大家一起分享自己的生活感悟。', N'totalposts:1;maxapplycount:1;othermissionids:0;totalpoint:1;onlinetime:1;usergroupids:0;points:15|00000,0,0,0,0,0,0,0', N'mission_action:1;mission_articleid:0;mission_timeout:1;mission_count:1;mission_userid:0|101', CAST(0x002D247F018B81FF AS DateTime), getdate(), getdate())
INSERT [bx_Missions] ([ID], [CycleTime], [SortOrder], [TotalUsers], [Type], [Name], [IconUrl], [DeductPoint], [Prize], [Description], [ApplyCondition], [FinishCondition], [EndDate], [BeginDate], [CreateDate]) VALUES (6, 86400, 145, 1, N'MaxLabs.bbsMax.Missions.TopicMission', N'每日发帖领取积分', N'~/max-assets/icon-mission/blog.gif', N'0,0,0,0,0,0,0,0', N'usergroupactivetimes:0;medalactivetimes:0;prizetypes:5;usergroupids:0;points:16;medalids:0;inviteserialcount:1|Point10,1,0,0,0,0,0,00', N'每日发帖 就可以轻松获取积分', N'totalposts:1;maxapplycount:1;othermissionids:0;totalpoint:1;onlinetime:1;usergroupids:0;points:15|00000,0,0,0,0,0,0,0', N'mission_topiccount:1;mission_replytopic:0;mission_action:1;mission_replyuser:0;mission_forumids:0;mission_timeout:0|50', CAST(0x002D247F018B81FF AS DateTime), getdate(), getdate())
INSERT [bx_Missions] ([ID], [CycleTime], [SortOrder], [TotalUsers], [Type], [Name], [IconUrl], [DeductPoint], [Prize], [Description], [ApplyCondition], [FinishCondition], [EndDate], [BeginDate], [CreateDate]) VALUES (7, 0, 167, 1, N'MaxLabs.bbsMax.Missions.FriendMission', N'寻找并添加五位好友', N'~/max-assets/icon-mission/friend.gif', N'0,0,0,0,0,0,0,0', N'usergroupactivetimes:0;medalactivetimes:0;prizetypes:5;usergroupids:0;points:16;medalids:0;inviteserialcount:1|Point50,5,0,0,0,0,0,00', N'有了好友，您发的日志、图片等会被好友及时看到并传播出去；
您也会在首页方便及时的看到好友的最新动态。
这会让您在这里的生活变得丰富多彩。', N'totalposts:1;maxapplycount:1;othermissionids:0;totalpoint:1;onlinetime:1;usergroupids:0;points:15|00000,0,0,0,0,0,0,0', N'mission_count:1|5', CAST(0x002D247F018B81FF AS DateTime), getdate(), getdate())
INSERT [bx_Missions] ([ID], [CycleTime], [SortOrder], [TotalUsers], [Type], [Name], [IconUrl], [DeductPoint], [Prize], [Description], [ApplyCondition], [FinishCondition], [EndDate], [BeginDate], [CreateDate]) VALUES (8, 0, 123, 0, N'MaxLabs.bbsMax.Entities.Missions.InviteMission', N'邀请10个新朋友加入', N'~/max-assets/icon-mission/friend.gif', N'0,0,0,0,0,0,0,0', N'usergroupactivetimes:0;medalactivetimes:0;prizetypes:5;usergroupids:0;points:18;medalids:0;inviteserialcount:1|Point100,10,0,0,0,0,0,00', N'邀请一下自己的QQ好友或者邮箱联系人，让亲朋好友一起来加入我们吧。', N'totalposts:1;maxapplycount:1;othermissionids:0;totalpoint:1;onlinetime:1;usergroupids:0;points:15|00000,0,0,0,0,0,0,0', N'mission_count:2|10', CAST(0x002D247F018B81FF AS DateTime), getdate(), getdate())
SET IDENTITY_INSERT [bx_Missions] OFF



SET IDENTITY_INSERT [bx_Props] ON
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (1, N'改名卡', 1000, 0, N'MaxLabs.bbsMax.Web.plugins.ChangeNameProp', N'', N'此可以修改你的论坛用户名', 0, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08CF9 AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/1.gif', 1, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (2, N'染色剂(红色6小时)', 1000, 0, N'MaxLabs.bbsMax.Web.plugins.ThreadHighlightProp', N'1,13,2,1|6color:#FF0000-1h', N'可以把你发表的主题标题加亮成红色6小时', 10, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08CF9 AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/9.gif', 1, 0, 1)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (3, N'染色剂(红色3小时)', 600, 0, N'MaxLabs.bbsMax.Web.plugins.ThreadHighlightProp', N'1,13,2,1|3color:#FF0000-1h', N'可以把你发表的帖子标题加亮成红色3小时', 10, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08CFE AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/9.gif', 1, 0, 1)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (4, N'置顶卡(版块2小时)', 1300, 0, N'MaxLabs.bbsMax.Web.plugins.ThreadStickProp', N'1,6,2,1|2Sticky-1h', N'此道具可以把你发表的帖子在本版块置顶2小时', 20, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08CFE AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/3.gif', 1, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (5, N'置顶卡(版块1小时)', 800, 0, N'MaxLabs.bbsMax.Web.plugins.ThreadStickProp', N'1,6,2,1|1Sticky-1h', N'此道具可以把你发表的帖子在本版块置顶1小时', 20, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08CFE AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/3.gif', 1, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (6, N'置顶卡(全站2小时)', 5000, 0, N'MaxLabs.bbsMax.Web.plugins.ThreadStickProp', N'1,12,2,1|2GlobalSticky-1h', N'此道具可以把你发表的帖子在全站置顶2小时', 50, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08D03 AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/3.gif', 0, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (7, N'置顶卡(全站1小时)', 3000, 0, N'MaxLabs.bbsMax.Web.plugins.ThreadStickProp', N'1,12,2,1|1GlobalSticky-1h', N'此道具可以把你发表的帖子在全站置顶1小时', 30, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08D03 AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/3.gif', 0, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (8, N'猪头术(3小时)', 60, 0, N'MaxLabs.bbsMax.Web.plugins.PigAvatarProp', N'3', N'此道具可以把别人头像变成猪头3小时', 10, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08D03 AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/4.gif', 1, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (9, N'猪头术(6小时)', 100, 0, N'MaxLabs.bbsMax.Web.plugins.PigAvatarProp', N'6', N'此道具可以把别人头像变成猪头6小时', 10, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08D03 AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/4.gif', 1, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (10, N'猪头还原卡', 50, 0, N'MaxLabs.bbsMax.Web.plugins.PigAvatarClearProp', N'', N'此道具可以把被猪头卡设置成猪头的头像还原', 10, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08D07 AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/8.gif', 1, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (11, N'猪头防御盾', 10, 0, N'MaxLabs.bbsMax.Web.plugins.PigAvatarProtectionProp', N'', N'此道具可以在别人对你使用猪头卡的时候自动进行防御，一次防御将用掉一个道具', 5, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08D07 AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/5.gif', 1, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (12, N'侦查术', 100, 0, N'MaxLabs.bbsMax.Web.plugins.ShowOnlineProp', N'', N'此道具可以查看别人是否隐身在线', 10, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08D07 AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/6.gif', 1, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (13, N'侦查防御盾', 10, 0, N'MaxLabs.bbsMax.Web.plugins.OnlineProtectionProp', N'', N'此道具可以在别人对你使用侦查卡并且你正好隐身时自动防御，对方将不知道你处于隐身状态，一次防御将用掉一个道具', 5, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08D0C AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/7.gif', 1, 0, 0)
INSERT [bx_Props] ([PropID], [Name], [Price], [PriceType], [PropType], [PropParam], [Description], [PackageSize], [TotalNumber], [SaledNumber], [AllowExchange], [AutoReplenish], [ReplenishNumber], [ReplenishTimeSpan], [LastReplenishTime], [BuyCondition], [Icon], [Enable], [ReplenishLimit], [SortOrder]) VALUES (14, N'帖子提升卡', 50, 0, N'MaxLabs.bbsMax.Web.plugins.SetThreadUpProp', N'-1', N'此道具可以把你发表的帖子提升到本版的第一页', 5, 999999, 0, 1, 0, 0, 0, CAST(0x00009CC300E08D0C AS DateTime), N'totalposts:1;totalpoint:1;onlinetime:1;usergroupids:0;points:15;releatedmissionids:0|0000,0,0,0,0,0,0,0', N'~/max-assets/icon-prop/2.gif', 1, 0, 0)
SET IDENTITY_INSERT [bx_Props] OFF


----------------内置通知类型
SET IDENTITY_INSERT [bx_NotifyTypes] ON
INSERT INTO bx_NotifyTypes(TypeID , TypeName, [Keep]) VALUES( 1 , N'管理通知', 1 );
INSERT INTO bx_NotifyTypes(TypeID , TypeName, [Keep]) VALUES( 2 , N'好友验证', 1 );
INSERT INTO bx_NotifyTypes(TypeID , TypeName, [Keep]) VALUES( 3 , N'打招呼', 1 );
INSERT INTO bx_NotifyTypes(TypeID , TypeName, [Keep]) VALUES( 101 , N'评论通知', 0 );
INSERT INTO bx_NotifyTypes(TypeID , TypeName, [Keep]) VALUES( 102 , N'道具通知', 0 );
SET IDENTITY_INSERT [bx_NotifyTypes] OFF

----------------内置Passport Client
SET IDENTITY_INSERT [bx_PassportClients] ON
INSERT INTO bx_PassportClients(ClientID , ClientName, Deleted, Url, APIFilePath) VALUES( 0 , N'Passport Server', 1, '', '' );
SET IDENTITY_INSERT [bx_PassportClients] OFF

----------------用户组管理
SET IDENTITY_INSERT [bx_Roles] ON
---------基本组
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(1,'任何人','','',198,0,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(2,'游客','','',198,1,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(3,'注册用户','','',198,2,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(4,'版块屏蔽用户','','',70,3,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(5,'整站屏蔽用户','','',70,4,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(6,'见习用户','见习用户','',6,40,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(7,'未通过实名认证用户','','',134,50,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(8,'头像未认证用户','','',134,60,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(9,'Email未认证用户','','',134,70,0,0,0);

---------管理员组
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(10,'实习版主','实习版主','~/max-assets/icon-role/pips8.gif',98,940,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(11,'版主','版主','~/max-assets/icon-role/pips9.gif',98,950,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(12,'分类版主','分类版主','~/max-assets/icon-role/pips10.gif',98,960,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(13,'超级版主','超级版主','~/max-assets/icon-role/pips10.gif',34,970,64,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(14,'管理员','管理员','~/max-assets/icon-role/pips10.gif',34,980,128,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(15,'创始人','创始人','~/max-assets/icon-role/pips10.gif',34,990,192,0,0);

---------等级组
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(16,'丐帮弟子','丐帮弟子','',74,0,0,0,-2147483648);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(17,'新手上路','新手上路','~/max-assets/icon-role/pips1.gif',73,0,0,0,0);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(18,'侠客','侠客','~/max-assets/icon-role/pips2.gif',73,0,0,0,50);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(19,'圣骑士','圣骑士','~/max-assets/icon-role/pips3.gif',73,0,0,0,200);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(20,'精灵','精灵','~/max-assets/icon-role/pips4.gif',73,0,0,0,500);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(21,'精灵王','精灵王','~/max-assets/icon-role/pips5.gif',73,0,0,0,1000);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(22,'风云使者','风云使者','~/max-assets/icon-role/pips6.gif',73,0,0,0,5000);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(23,'光明使者','光明使者','~/max-assets/icon-role/pips7.gif',73,0,0,0,10000);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(24,'天使','天使','~/max-assets/icon-role/pips8.gif',73,0,0,0,50000);
INSERT INTO bx_Roles(RoleID,[Name],Title,IconUrl,RoleType,[Level],StarLevel,UserCount,RequiredPoint) Values(25,'法老','法老','~/max-assets/icon-role/pips9.gif',73,0,0,0,100000);
SET IDENTITY_INSERT [bx_Roles] OFF
GO