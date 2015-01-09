CREATE PROCEDURE [bx_CreatePostMark]
	@PostID int, 
	@UserID int,
	@CreateDate datetime,
	@ExtendedPoints_1 int,
	@ExtendedPoints_2 int,
	@ExtendedPoints_3 int,
	@ExtendedPoints_4 int,
	@ExtendedPoints_5 int,
	@ExtendedPoints_6 int,
	@ExtendedPoints_7 int,
	@ExtendedPoints_8 int,
	@Reason ntext
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM [bx_PostMarks] WHERE [UserID] = @UserID AND [PostID] = @PostID)
		RETURN 1;
	ELSE BEGIN
		insert into [bx_PostMarks](
			PostID,
			UserID,
			CreateDate,
			ExtendedPoints_1,
			ExtendedPoints_2,
			ExtendedPoints_3,
			ExtendedPoints_4,
			ExtendedPoints_5,
			ExtendedPoints_6,
			ExtendedPoints_7,
			ExtendedPoints_8,
			Reason
			)
		Values
		(
			@PostID, 
			@UserID,
			@CreateDate,
			@ExtendedPoints_1,
			@ExtendedPoints_2,
			@ExtendedPoints_3,
			@ExtendedPoints_4,
			@ExtendedPoints_5,
			@ExtendedPoints_6,
			@ExtendedPoints_7,
			@ExtendedPoints_8,
			@Reason
		);

		RETURN 0;
		
	END

END