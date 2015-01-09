EXEC bx_Drop bx_TempUploadFiles;

GO

CREATE TABLE [bx_TempUploadFiles](
    [TempUploadFileID]      int              IDENTITY(1, 1)                  NOT NULL    CONSTRAINT  [PK_bx_TempUploadFiles]              PRIMARY KEY ([TempUploadFileID])
   ,[UserID]                int                                              NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_UserID]		  DEFAULT (0)
   ,[UploadAction]          varchar(100)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_UploadAction] DEFAULT ('')
   ,[SearchInfo]            nvarchar(100)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_SearchInfo]   DEFAULT (N'')
   ,[CustomParams]          nvarchar(3000)   COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_CustomParams] DEFAULT (N'')
   ,[FileName]              nvarchar(256)    COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_FileName]     DEFAULT (N'')
   ,[ServerFileName]        varchar(100)     COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_ServerFileName]   DEFAULT ('')
   ,[MD5]					char(32)         COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_MD5]          DEFAULT ('')
   ,[FileSize]              bigint                                           NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_FileSize]     DEFAULT (0)
   ,[FileID]				varchar(50)      COLLATE Chinese_PRC_CI_AS_WS    NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_FileID]       DEFAULT ('')
   ,[CreateDate]            datetime                                         NOT NULL    CONSTRAINT  [DF_bx_TempUploadFiles_CreateDate]   DEFAULT (GETDATE())
);

/*
Name:
Columns:

*/

--GO

--CREATE INDEX [IX_bx_TempUploadFiles_Key] ON [bx_TempUploadFiles]([MD5], [FileSize]);

GO

CREATE INDEX [IX_bx_TempUploadFiles_Search] ON [bx_TempUploadFiles]([UserID], [UploadAction], [SearchInfo]);

GO

CREATE INDEX [IX_bx_TempUploadFiles_CreateDate] ON [bx_TempUploadFiles]([CreateDate]);

GO