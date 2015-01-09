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