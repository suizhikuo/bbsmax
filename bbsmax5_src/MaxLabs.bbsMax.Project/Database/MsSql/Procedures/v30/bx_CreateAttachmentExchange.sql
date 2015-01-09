CREATE PROCEDURE [bx_CreateAttachmentExchange]
	@AttachmentID int, 
	@UserID int,
	@Price int
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM bx_AttachmentExchanges WHERE AttachmentID=@AttachmentID AND UserID=@UserID)
		RETURN 0;
	Insert into bx_AttachmentExchanges(AttachmentID,UserID,Price,CreateDate) values(@AttachmentID,@UserID,@Price,getdate())
	IF(@@ROWCOUNT>0)
		RETURN 0
	ELSE
		RETURN 1

END