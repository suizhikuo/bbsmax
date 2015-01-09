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