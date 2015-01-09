

CREATE PROCEDURE bx_UpdateUserGeneralPoint
     @UserID       int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MaxValue int;
	SET @MaxValue = 2147483647;
	SET ARITHABORT OFF;
	SET ANSI_WARNINGS OFF;
		
    UPDATE bx_Users SET Points = ISNULL([Point_1]+[Point_2]*10,@MaxValue) WHERE [UserID] = @UserID;
END