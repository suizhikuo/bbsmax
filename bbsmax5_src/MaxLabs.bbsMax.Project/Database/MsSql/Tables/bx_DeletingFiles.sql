EXEC bx_Drop bx_DeletingFiles;

CREATE TABLE [bx_DeletingFiles] (
	[DeletingFileID]			int	IDENTITY(1,1)							    NOT NULL
    ,[ServerFilePath]		nvarchar(256)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL
    
    ,CONSTRAINT [PK_bx_DeletingFiles] PRIMARY KEY ([DeletingFileID])
);

/*
Name:    已删除的文件表
Columns:
        [DeletingFileID]             主键，唯一标识，正在删除的文件ID
        [ServerFilePath]            文件保存路径,相对路径
*/

GO