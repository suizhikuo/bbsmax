CREATE PROCEDURE [bx_GetAttachment]
	@AttachmentID INT,
	@UpdateTotalDownloads bit
AS
BEGIN
	SET NOCOUNT ON;
	IF(@UpdateTotalDownloads=1)
		UPDATE bx_Attachments SET TotalDownloads=TotalDownloads+1 Where AttachmentID=@AttachmentID
	
	SELECT * FROM [bx_Attachments] WITH (NOLOCK) WHERE AttachmentID =@AttachmentID

END
