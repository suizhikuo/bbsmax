CREATE PROCEDURE [bx_GetAttachmentIsBuy]
	@UserID INT,
	@AttachmentID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM bx_AttachmentExchanges WHERE AttachmentID=@AttachmentID AND UserID=@UserID)
		RETURN 0
	ELSE
	BEGIN
		IF EXISTS(SELECT * FROM bx_Attachments WHERE AttachmentID=@AttachmentID AND Price=0)
			RETURN 0
		ELSE
			RETURN -1
	END
END