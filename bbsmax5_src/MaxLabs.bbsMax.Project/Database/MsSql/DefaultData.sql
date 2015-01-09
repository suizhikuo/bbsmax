
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