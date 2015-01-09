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
