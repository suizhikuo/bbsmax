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