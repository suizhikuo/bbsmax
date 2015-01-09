EXEC bx_Drop bx_ClubMembers;

GO

CREATE TABLE [bx_ClubMembers] (
     [ClubID]          int                         NOT NULL    CONSTRAINT [DF_bx_ClubMembers_ClubID]          DEFAULT (0)
    ,[UserID]          int                         NOT NULL    CONSTRAINT [DF_bx_ClubMembers_UserID]          DEFAULT (0)
    ,[SortOrder]       int        identity(1,1)    NOT NULL
    ,[Status]          tinyint                     NOT NULL    CONSTRAINT [DF_bx_ClubMembers_Status]          DEFAULT (0)
    
    ,[CreateDate]      datetime                    NOT NULL    CONSTRAINT [DF_bx_ClubMembers_CreateDate]      DEFAULT (GETDATE())
    
    ,CONSTRAINT        [PK_bx_ClubMembers]   PRIMARY KEY ([ClubID], [UserID])
)

CREATE INDEX [IX_bx_ClubMembers_Status] ON [bx_ClubMembers]([Status]);

/*
Name:群组分类
Columns:
    [ClubID]        群组ID
    [UserID]        用户ID
    [Status]        成员状态：等待验证、禁言、普通会员、管理员、群主
    [CreateDate]    创见时间
*/

GO
