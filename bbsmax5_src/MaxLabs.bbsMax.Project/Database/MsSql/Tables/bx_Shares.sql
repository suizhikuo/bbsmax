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
