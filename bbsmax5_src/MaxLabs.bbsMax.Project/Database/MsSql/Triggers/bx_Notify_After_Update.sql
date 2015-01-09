CREATE TRIGGER bx_Notify_After_Update ON [bx_Notify] 
FOR UPDATE 
AS
BEGIN

IF UPDATE(IsRead) BEGIN
	DECLARE @UserID int;
	DECLARE @TypeID int;
	DECLARE @Count int;
	DECLARE @RowCount int;
	DECLARE @i int;

	SET @i=0;
	DECLARE @temp TABLE( RowID int  IDENTITY(1,1),  UserID int,TypeID int);

	INSERT @temp( UserID , TypeID )  SELECT DISTINCT UserID , TypeID FROM DELETED;

	SET @RowCount = @@ROWCOUNT;

	WHILE  @i<@RowCount
	BEGIN

	SELECT @UserID = UserID ,@TypeID = TypeID FROM @temp WHERE RowID = @i + 1;

	SET @Count = ( SELECT COUNT(*) FROM bx_Notify WHERE UserID = @UserID AND TypeID = @TypeID  AND IsRead = 0 );

	UPDATE bx_UnreadNotifies SET UnreadCount = @Count WHERE UserID = @UserID AND TypeID = @TypeID;

	IF @@ROWCOUNT =0 BEGIN
		INSERT INTO bx_UnreadNotifies( UserID , TypeID , UnreadCount ) VALUES(@UserID , @TypeID , @Count);
	END 

	SET @i = @i + 1;
	END
END
END