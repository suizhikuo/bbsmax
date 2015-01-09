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