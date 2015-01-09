EXEC bx_Drop bx_Files;

CREATE TABLE [bx_Files] (
     [FileID]              varchar(50)          COLLATE Chinese_PRC_CI_AS_WS               NOT NULL    CONSTRAINT  [DF_bx_Files_FileID]				DEFAULT ('')
     
    ,[ServerFilePath]      nvarchar(256)        COLLATE Chinese_PRC_CI_AS_WS               NOT NULL    CONSTRAINT  [DF_bx_Files_ServerFilePath]		DEFAULT (N'')
    
    ,[MD5]                 char(32)             COLLATE Chinese_PRC_CI_AS_WS               NOT NULL    CONSTRAINT  [DF_bx_Files_MD5]				DEFAULT ('')
    ,[FileSize]            bigint                                                          NOT NULL    CONSTRAINT  [DF_bx_Files_FileSize]			DEFAULT (0)
    
    ,CONSTRAINT [PK_bx_Files] PRIMARY KEY ([FileID])
);

/*
Name:    真实文件表
Columns:
        [FileID]                    主键，唯一标识，文件ID
        
        [ServerFilePath]            文件保存路径,相对路径
        
        [MD5]                       文件MD5值
        [FileSize]                  文件大小
*/

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_bx_Files_Key] ON [bx_Files]([MD5], [FileSize])

GO