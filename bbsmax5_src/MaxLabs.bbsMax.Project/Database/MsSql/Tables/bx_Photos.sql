EXEC bx_Drop bx_Photos;


GO

CREATE TABLE [bx_Photos] (
    [PhotoID]                int                 IDENTITY(1, 1)                  NOT NULL    CONSTRAINT [PK_bx_Photos]                         PRIMARY KEY ([PhotoID])
   ,[UserID]                 int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_UserID]                 DEFAULT(0)
   ,[AlbumID]                int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_AlbumID]                DEFAULT(0)
   ,[TotalViews]             int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_TotalViews]             DEFAULT(0)
   ,[TotalComments]          int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_TotalComments]          DEFAULT(0)
   
   ,[FileID]                 varchar(50)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_FileID]                 DEFAULT ('')
   ,[FileSize]               bigint                                              NOT NULL    CONSTRAINT  [DF_bx_Photos_FileSize]               DEFAULT (0)
   ,[FileType]               varchar(10)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_FileType]               DEFAULT ('')
   ,[Width]					int													 NOT NULL    CONSTRAINT  [DF_bx_Photos_Width]				   DEFAULT(0)
   ,[Height]				int													 NOT NULL    CONSTRAINT  [DF_bx_Photos_Height]				   DEFAULT(0)
   ,[Exif]                   nvarchar(1500)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_Exif]	               DEFAULT ('')
   ,[Name]                   nvarchar(50)        COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_Name]                   DEFAULT ('')
   ,[Description]            nvarchar(1500)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_Description]            DEFAULT ('')
   
   ,[ThumbPath]              varchar(256)        COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_ThumbPath]              DEFAULT ('')
   ,[ThumbWidth]             int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_ThumbWidth]             DEFAULT(0)
   ,[ThumbHeight]            int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_ThumbHeight]            DEFAULT(0)
   
   ,[CreateIP]               varchar(50)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_CreateIP]               DEFAULT ('')
   
   ,[CreateDate]             datetime                                            NOT NULL    CONSTRAINT  [DF_bx_Photos_CreateDate]             DEFAULT (GETDATE())
   ,[UpdateDate]             datetime                                            NOT NULL    CONSTRAINT  [DF_bx_Photos_UpdateDate]             DEFAULT (GETDATE())
   ,[LastCommentDate]        datetime                                            NOT NULL    CONSTRAINT  [DF_bx_Photos_LastCommentDate]        DEFAULT ('1753-1-1')
   
   ,[LastEditUserID]         int                                                 NOT NULL    CONSTRAINT  [DF_bx_Photos_LastEditUserID]         DEFAULT(0)
   
   ,[KeywordVersion]         varchar(32)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_Photos_KeywordVersion]         DEFAULT ('')
)

/*
Name:相片
Columns:
    [PhotoID]          自动标识
    [AlbumID]          相册ID
    [TotalViews]       总浏览数
    [TotalComments]    总相片数
    
    [FileID]           文件ID:由文件表提供
    
    [Exif]             相片扩展信息
    [Name]             相片名称
    [Description]      相片描述
    [NameReverter]     相片名称关键字还原信息
    [DescnReverter]    相片描述关键字还原信息
    
    [CreateIP]         上传IP
    
    [CreateDate]       上传时间
    [UpdateDate]       最后更新时间
    [LastCommentDate]  最后评论时间
*/

GO

--相片表的用户ID索引
CREATE INDEX [IX_bx_Photos_UserID] ON [bx_Photos]([UserID])

--相片表的相册ID索引
CREATE INDEX [IX_bx_Photos_AlbumID] ON [bx_Photos]([AlbumID])


CREATE INDEX [IX_bx_Photos_FileID] ON [bx_Photos]([FileID])
--相片表的创建时间索引
--CREATE INDEX [IX_bx_Photos_CreateDate] ON [bx_Photos]([CreateDate])
