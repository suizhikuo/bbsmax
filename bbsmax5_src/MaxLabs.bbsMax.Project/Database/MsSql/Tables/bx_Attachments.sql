EXEC bx_Drop 'bx_Attachments';

CREATE TABLE bx_Attachments
(
AttachmentID		int				NOT NULL IDENTITY(1, 1),
PostID				int				NOT NULL,
--DiskFileID int NOT NULL,
FileID				varchar(50)		COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT DF_bx_Attachments_FileID				DEFAULT (''),
FileName			nvarchar(256)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT DF_bx_Attachments_FileName			DEFAULT (N''),
FileType			nvarchar (10)	COLLATE Chinese_PRC_CI_AS_WS	NOT NULL	CONSTRAINT DF_bx_Attachments_FileType			DEFAULT (''),
FileSize			bigint											NOT NULL	CONSTRAINT DF_bx_Attachments_FileSize			DEFAULT (0),
TotalDownloads		int												NOT NULL	CONSTRAINT DF_bx_Attachments_TotalDownloads		DEFAULT (0),
TotalDownloadUsers	int												NOT NULL	CONSTRAINT DF_bx_Attachments_TotalDownloadUsers	DEFAULT (0),
Price				int												NOT NULL	CONSTRAINT DF_bx_Attachments_Price				DEFAULT (0),
FileExtendedInfo	nvarchar(1000)	COLLATE Chinese_PRC_CI_AS_WS	NULL		CONSTRAINT DF_bx_Attachments_FileExtendedInfo	DEFAULT (N''),
UserID				int												NOT NULL	CONSTRAINT DF_bx_Attachments_UserID				DEFAULT (0),
CreateDate			datetime										NOT NULL	CONSTRAINT DF_bx_Attachments_CreateDate			DEFAULT (getdate()),

CONSTRAINT PK_bx_Attachments PRIMARY KEY NONCLUSTERED  (AttachmentID)
)
GO

CREATE NONCLUSTERED INDEX IX_bx_Attachments_List ON bx_Attachments (PostID);
CREATE NONCLUSTERED INDEX IX_bx_Attachments_User ON bx_Attachments (UserID);
CREATE NONCLUSTERED INDEX IX_bx_Attachments_FileID ON bx_Attachments (FileID);

CREATE INDEX IX_bx_Attachments_CreateDate ON bx_Attachments (CreateDate);
GO

