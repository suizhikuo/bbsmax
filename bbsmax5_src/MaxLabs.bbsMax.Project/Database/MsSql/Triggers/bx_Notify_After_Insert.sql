CREATE TRIGGER bx_Notify_After_Insert  ON [bx_Notify] 
FOR INSERT 
AS
BEGIN


DECLARE @UserID int;
DECLARE @TypeID int;
DECLARE @Count int;

SELECT @UserID = UserID ,@TypeID = TypeID FROM INSERTED;

SET @Count = ( SELECT COUNT(*) FROM bx_Notify WHERE UserID = @UserID AND TypeID = @TypeID AND IsRead = 0 );

UPDATE bx_UnreadNotifies SET UnreadCount = @Count WHERE UserID = @UserID AND TypeID = @TypeID;

IF @@ROWCOUNT =0 BEGIN
	INSERT INTO bx_UnreadNotifies( UserID , TypeID , UnreadCount ) VALUES(@UserID , @TypeID , @Count);
END 

END
