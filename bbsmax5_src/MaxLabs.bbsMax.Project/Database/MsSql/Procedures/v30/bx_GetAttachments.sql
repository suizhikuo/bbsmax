CREATE PROCEDURE [bx_GetAttachments]
	@PostID int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * from bx_Attachments WITH (NOLOCK) where PostID=@PostID
END