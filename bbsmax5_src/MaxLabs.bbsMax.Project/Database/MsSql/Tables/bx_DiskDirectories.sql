--网络硬盘目录表 
EXEC bx_Drop bx_DiskDirectories;

CREATE TABLE [bx_DiskDirectories] (
      [DirectoryID]                   int                     IDENTITY(1,1)                       NOT NULL
      
     ,[Name]                 nvarchar(256)           COLLATE Chinese_PRC_CI_AS_WS        NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_Name]                  DEFAULT('')
     ,[Password]             nvarchar(50)            COLLATE Chinese_PRC_CI_AS_WS        NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_Password]              DEFAULT('')
    
     ,[PrivacyType]          tinyint                                                     NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_PrivacyType]           DEFAULT(2)
     
     ,[UserID]               int                                                         NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_UserID]                DEFAULT(0)
     ,[ParentID]             int                                                         NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_ParentID]              DEFAULT(0)
     ,[TotalFiles]           int                                                         NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_TotalFiles]            DEFAULT(0)
     ,[TotalSubDirectories]  int                                                         NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_TotalSubDirectories]   DEFAULT(0)
     
     ,[TotalSize]            bigint                                                      NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_TotalSize]             DEFAULT(0)
     
     ,[CreateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_CreateDate]            DEFAULT(GETDATE())
     ,[UpdateDate]           datetime                                                    NOT NULL    CONSTRAINT [DF_bx_DiskDirectory_UpdateDate]            DEFAULT(GETDATE())
     
     ,CONSTRAINT [PK_bx_DiskDirectories] PRIMARY KEY ([DirectoryID])
);  

/*
Name:     网络硬盘目录表
Columns:
          [ID]     
                      
          [Name]                                          目录名称
          [Password]                                      当目录隐私类型为3时,需要此密码才可以查看此目录
          
          [PrivacyType]                                   目录隐私类型,0 全站可见  1 好友可见  2 只有自己可见 3 提供密码可见
          
          [UserID]                                        目录拥有者ID
          [ParentID]                                      目录的父目录,若顶级目录,则为0
          [TotalFiles]                                    目录子文件个数
          [TotalSubDirectories]                           目录子目录个数
          
          [TotalSize]                                     目录总大小..只存该目录下的文件的总大小,内部用,不显示给用户
          
          [CreateDate]                                    目录创建日期
          [UpdateDate]                                    目录最后更新时间
*/

GO


--ID索引
EXEC bx_Drop 'IX_bx_DiskDirectories_ID';
CREATE INDEX [IX_bx_DiskDirectories_ID] ON [bx_DiskDirectories]([DirectoryID])

--用户ID索引
EXEC bx_Drop 'IX_bx_DiskDirectories_UserID';
CREATE INDEX [IX_bx_DiskDirectories_UserID] ON [bx_DiskDirectories]([UserID])

--父目录ID索引
EXEC bx_Drop 'IX_bx_DiskDirectories_ParentID';
CREATE INDEX [IX_bx_DiskDirectories_ParentID] ON [bx_DiskDirectories]([ParentID])

GO