-- =============================================
-- Author:		<sek>
-- Create date: <2007/1/18>
-- Description:	<投票>
-- =============================================
CREATE PROCEDURE [bx_Vote]
	@ItemIDs varchar(8000),
	@ThreadID int,
	@UserID int,
	@NickName nvarchar(64)
	--@ExtendedPoints_1 int output,
	--@ExtendedPoints_2 int output,
	--@ExtendedPoints_3 int output,
	--@ExtendedPoints_4 int output,
	--@ExtendedPoints_5 int output,
	--@ExtendedPoints_6 int output,
	--@ExtendedPoints_7 int output,
	--@ExtendedPoints_8 int output,
	--@Points int output
AS
	SET NOCOUNT ON
	IF EXISTS (SELECT * FROM bx_Polls WITH(NOLOCK) WHERE ThreadID=@ThreadID AND ExpiresDate<getdate()) 
		RETURN (2)--已经过期
	IF EXISTS (SELECT * FROM [bx_PollItemDetails] WITH(NOLOCK) WHERE ItemID IN(SELECT ItemID FROM [bx_PollItems] WHERE ThreadID=@ThreadID) AND UserID=@UserID)
		RETURN (1)--	当前用户已经投过票
	IF EXISTS (SELECT * FROM bx_Threads WITH(NOLOCK) WHERE ThreadID=@ThreadID AND IsLocked=1) 
		RETURN (11)--已经锁定
	DECLARE @ItemID int,@i int
	SET @ItemIDs=@ItemIDs+N','
	SELECT @i=CHARINDEX(',',@ItemIDs)
			
	WHILE(@i>1)
		BEGIN
			SELECT @ItemID=SUBSTRING(@ItemIDs,0, @i)
			
			UPDATE bx_PollItems SET PollItemCount=PollItemCount+1 WHERE ItemID=@ItemID
			
			IF(@@ROWCOUNT>0)
				INSERT INTO bx_PollItemDetails(ItemID,UserID,NickName) VALUES(@ItemID,@UserID,@NickName)
			
			SELECT @ItemIDs=SUBSTRING(@ItemIDs,@i+1,LEN(@ItemIDs)-@i)
			SELECT @i=CHARINDEX(',',@ItemIDs)
		END

	--execute bx_UpdateUserPoints @UserID,
									--@ExtendedPoints_1,
									--@ExtendedPoints_2,
									--@ExtendedPoints_3,
									--@ExtendedPoints_4,
									--@ExtendedPoints_5,
									--@ExtendedPoints_6,
									--@ExtendedPoints_7,
									--@ExtendedPoints_8

	--SELECT	@Points=Points,
			--@ExtendedPoints_1=ExtendedPoints_1,
			--@ExtendedPoints_2=ExtendedPoints_2,
			--@ExtendedPoints_3=ExtendedPoints_3,
			--@ExtendedPoints_4=ExtendedPoints_4,
			--@ExtendedPoints_5=ExtendedPoints_5,
			--@ExtendedPoints_6=ExtendedPoints_6,
			--@ExtendedPoints_7=ExtendedPoints_7,
			--@ExtendedPoints_8=ExtendedPoints_8
		--FROM [bx_UserProfiles] WHERE UserID=@UserID
	RETURN (0)


