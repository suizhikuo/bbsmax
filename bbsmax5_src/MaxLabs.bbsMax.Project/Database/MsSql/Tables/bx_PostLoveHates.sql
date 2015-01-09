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