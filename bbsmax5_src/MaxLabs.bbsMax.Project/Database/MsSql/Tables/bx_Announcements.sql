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