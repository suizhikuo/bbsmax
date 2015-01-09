--网络硬盘文件表
EXEC bx_Drop bx_DiskFiles;

CREATE TABLE [bx_DiskFiles] (
     [DiskFileID]          int                     IDENTITY(1,1)                           NOT NULL
    ,[FileID]              varchar(50)             COLLATE Chinese_PRC_CI_AS_WS            NOT NULL     CONSTRAINT [DF_bx_DiskFiles_FileID]           DEFAULT('')
    
    ,[FileName]            nvarchar(256)           COLLATE Chinese_PRC_CI_AS_WS            NOT NULL     CONSTRAINT [DF_bx_DiskFiles_Filename]         DEFAULT(N'')
    ,[Extension]           nvarchar(10)            COLLATE Chinese_PRC_CI_AS_WS            NOT NULL     CONSTRAINT [DF_bx_DiskFiles_Extension]        DEFAULT(N'')
	,[ThumbPath]           nvarchar(256)           COLLATE Chinese_PRC_CI_AS_WS            NOT NULL     CONSTRAINT [DF_bx_DiskFiles_ThumbPath]        DEFAULT(N'')
    ,[FileSize]            bigint                                                          NOT NULL     CONSTRAINT [DF_bx_DiskFiles_FileSize]		  DEFAULT(0)

    ,[UserID]              int                                                             NOT NULL     CONSTRAINT [DF_bx_DiskFiles_UserID]           DEFAULT(0)
    ,[DirectoryID]         int                                                             NOT NULL     CONSTRAINT [DF_bx_DiskFiles_DirectoryID]      DEFAULT(0)
    ,[TotalDownloads]      int                                                             NOT NULL     CONSTRAINT [DF_bx_DiskFiles_TotalDownloads]   DEFAULT(0)
    
    ,[CreateDate]          datetime                                                        NOT NULL     CONSTRAINT [DF_bx_DiskFiles_CreateDate]       DEFAULT(GETDATE())
    ,[UpdateDate]          datetime                                                        NOT NULL     CONSTRAINT [DF_bx_DiskFiles_UpdateDate]       DEFAULT(GETDATE())
    ,[ExtensionInfo]       ntext				   COLLATE Chinese_PRC_CI_AS_WS			   NULL     	 
    ,CONSTRAINT [PK_bx_DiskFiles] PRIMARY KEY ([DiskFileID])
);

/*
Name:      网络硬盘文件表,此为虚拟文件,有与之相对应的真实文件表bx_Files
Columns:
        [ID]
        [FileID]                                      对应的真实文件的数据的ID
        
        [Filename]                                    文件名
        [Extension]                                   文件扩展名,如果如果超过10的不当作后缀名
        
        [UserID]                                      文件所属用户ID
        [DirectoryID]                                 文件所属目录ID
        [TotalDownloads]                              文件下载次数,自己浏览不计算进去
        
        [CreateDate]                                  文件创建时间
        [UpdateDate]                                  文件更新时间
*/

GO

--文件名索引
CREATE INDEX [IX_bx_DiskFiles_FileName] ON [bx_DiskFiles] ([FileName])

--文件夹索引
CREATE INDEX [IX_bx_DiskFiles_DirectoryID] ON [bx_DiskFiles] ([DirectoryID])

--文件ID索引
CREATE INDEX [IX_bx_DiskFiles_FileID] ON [bx_DiskFiles] ([FileID]);

CREATE INDEX [IX_bx_DiskFiles_UserID] ON [bx_DiskFiles] ([UserID]);