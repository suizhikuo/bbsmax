EXEC bx_Drop bx_Albums;

GO

CREATE TABLE [bx_Albums](
    [AlbumID]          int              IDENTITY(1, 1)                  NOT NULL    CONSTRAINT  [PK_bx_Albums]                PRIMARY KEY ([AlbumID])
   ,[UserID]           int                                              NOT NULL    CONSTRAINT  [DF_bx_Albums_UserID]         DEFAULT(0)
   ,[TotalPhotos]      int                                              NOT NULL    CONSTRAINT  [DF_bx_Albums_TotalPhotos]    DEFAULT(0)
   ,[LastEditUserID]   int                                              NOT NULL    CONSTRAINT  [DF_bx_Albums_LastEditUserID] DEFAULT(0)
  
   ,[PrivacyType]      tinyint                                          NOT NULL    CONSTRAINT  [DF_bx_Albums_PrivacyType]    DEFAULT(0)
  
   ,[Name]             nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Albums_Name]           DEFAULT('')
   ,[Description]       nvarchar(100)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT [DF_bx_Albums_Description]       DEFAULT('')

   ,[Cover]            nvarchar(200)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Albums_Cover]          DEFAULT('')
   ,[CoverPhotoID]     int
   
   ,[Password]         nvarchar(50)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Albums_Password]       DEFAULT('')

   ,[CreateDate]       datetime                                         NOT NULL    CONSTRAINT  [DF_bx_Albums_CreateDate]     DEFAULT (GETDATE())
   ,[UpdateDate]       datetime                                         NOT NULL    CONSTRAINT  [DF_bx_Albums_UpdateDate]     DEFAULT (GETDATE())
   ,[KeywordVersion]   varchar(32)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Albums_KeywordVersion] DEFAULT('')
)

/*
Name:相册
Columns:
    [AlbumID]          自动标识ID
    [UserID]           用户ID
    [TotalPhotos]      些相册照片数
  
    [PrivacyType]      权限类型 0.所有用户见 1.全好友可见 2.仅自己可见 3.凭密码查看
     
    [Name]             相册名称
    [Cover]            相册封面图片
    [Password]         凭密码查看时的密码 
    [NameReverter]     相册名称关键字还原信息
   
    [CreateDate]       创建时间
    [UpdateDate]       更新时间
*/

GO

--相册表的用户ID索引
CREATE INDEX [IX_bx_Albums_UserID] ON [bx_Albums]([UserID])

--相册隐私类型的索引
CREATE INDEX [IX_bx_Albums_PrivacyType] ON [bx_Albums]([PrivacyType])

--相册表的创建时间索引
--CREATE INDEX [IX_bx_Albums_CreateDate] ON [bx_Albums]([CreateDate])
