EXEC bx_Drop 'bx_ADCategory';

CREATE TABLE bx_ADCategory (
	 [CategoryID]         int IDENTITY (1, 1)                            NOT NULL 
	,[CategoryName]       nvarchar(50)        COLLATE Chinese_PRC_CI_AS_WS  NOT NULL 
	,[Description]        nvarchar(1000)      COLLATE Chinese_PRC_CI_AS_WS  NULL 
	,[ShowInForum]        bit NULL 
	,[CommomPages]        varchar(500)        COLLATE Chinese_PRC_CI_AS_WS  NULL 
	
	CONSTRAINT [PK_bx_ADCategory] PRIMARY KEY ([CategoryID])
);