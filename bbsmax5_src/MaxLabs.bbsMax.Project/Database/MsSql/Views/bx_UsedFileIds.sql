/*还在使用的文件*/

CREATE VIEW bx_UsedFileIds
AS

	SELECT FileID FROM bx_Photos
	UNION ALL
	SELECT FileID FROM bx_Attachments
	UNION ALL
	SELECT FileID FROM bx_DiskFiles