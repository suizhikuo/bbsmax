--创建用户动态

CREATE PROCEDURE bx_CreateFeed
     @IsSpecial        bit 
     
	,@UserID           int
	,@TargetID         int
	,@TargetUserID     int
	
	,@ActionType       tinyint
	,@PrivacyType      tinyint
	
	,@AppID            uniqueidentifier
	
	,@Title            nvarchar(1000)
	,@Realname         nvarchar(50)
	,@Description      nvarchar(2500)
	,@TargetNickname   nvarchar(50)
	,@VisibleUserIDs   varchar(800)
	
	,@CreateDate       datetime
	,@CanJoin		   bit --是否能合并的 如果能将查找1小时内相同的动态进行合并
	
	,@DefaultSendType  tinyint -- 0默认发送 1默认不发送 2强制发送 3强制不发送
	,@CommentTargetID  int
AS
BEGIN

	SET NOCOUNT ON;
	

    --判断是否记录该类型的动态
    IF @DefaultSendType = 3
		RETURN;
	IF @DefaultSendType <> 2 BEGIN
		DECLARE @Send bit
		SELECT @Send = [Send] FROM bx_UserNoAddFeedApps WHERE AppID=@AppID AND UserID=@UserID AND ActionType=@ActionType;
		
		IF @Send IS NULL AND @DefaultSendType = 1 --默认不发送
			RETURN;
		ELSE IF @Send = 0 -- 用户设置不发送
			RETURN;
			
		IF @IsSpecial=1 BEGIN
			SET @Send = NULL;
			SELECT @Send = [Send] FROM bx_UserNoAddFeedApps WHERE AppID=@AppID AND UserID=@TargetUserID AND ActionType=@ActionType;
			
			IF @Send IS NULL AND @DefaultSendType = 1 --默认不发送
				RETURN;
			ELSE IF @Send = 0 -- 用户设置不发送
				RETURN;
		END
	END
	
    DECLARE @FeedID int;
    DECLARE @IsExistUser bit;-- bx_UserFeeds表中是否存在当前用户
    DECLARE @IsExistTargetUser bit;
    
    SET @IsExistUser = 1; 
    SET @IsExistTargetUser = 1; 
    
	IF @CanJoin = 1
		SELECT @FeedID=ID FROM bx_Feeds WHERE AppID=@AppID AND ActionType=@ActionType AND ((TargetID IS NOT NULL AND TargetID=@TargetID)  OR (TargetID IS NULL AND @TargetID IS NULL)) AND TargetUserID=@TargetUserID AND CreateDate>DATEADD(hour,-1,@CreateDate);--查找1小时内相同的动态
		
	IF @FeedID IS NOT NULL BEGIN
		IF EXISTS(SELECT * FROM bx_UserFeeds WHERE FeedID=@FeedID AND UserID=@UserID) BEGIN
			UPDATE bx_UserFeeds SET 
					 Realname=@Realname
					,CreateDate=@CreateDate 
					WHERE FeedID=@FeedID AND UserID=@UserID;
		END
		ELSE
			SET @IsExistUser = 0;
		
		IF @IsSpecial=1 BEGIN
			IF EXISTS(SELECT * FROM bx_UserFeeds WHERE FeedID=@FeedID AND UserID=@TargetUserID)
				UPDATE bx_UserFeeds SET 
						 Realname=@TargetNickname
						,CreateDate=@CreateDate 
						WHERE FeedID=@FeedID AND UserID=@TargetUserID;
			ELSE
				SET @IsExistTargetUser = 0;
		END
		UPDATE [bx_Feeds] SET Title=@Title,Description=@Description,CommentTargetID=@CommentTargetID,CommentInfo=null WHERE [ID]=@FeedID
	END 
	ELSE BEGIN
		SET @IsExistUser = 0;
		SET @IsExistTargetUser = 0;
		
		INSERT INTO bx_Feeds( 
					 TargetID
					,TargetUserID
					,ActionType
					,PrivacyType
					,AppID
					,Title
					,Description
					,TargetNickname
					,VisibleUserIDs
					,CreateDate
					,CommentTargetID
					) VALUES(
					 @TargetID
					,@TargetUserID
					,@ActionType
					,@PrivacyType
					,@AppID
					,@Title
					,@Description
					,@TargetNickname
					,@VisibleUserIDs
					,@CreateDate
					,@CommentTargetID
					);
					
		SELECT @FeedID = @@IDENTITY;
		
		
	END
	
	IF @IsExistUser = 0
		INSERT INTO bx_UserFeeds(
					 FeedID
					,UserID
					,Realname
					,CreateDate
					) VALUES(
					 @FeedID
					,@UserID
					,@Realname
					,@CreateDate
					);
					
	IF @IsSpecial=1 AND @IsExistTargetUser = 0
		INSERT INTO bx_UserFeeds(
					 FeedID
					,UserID
					,Realname
					,CreateDate
					) VALUES(
					 @FeedID
					,@TargetUserID
					,@TargetNickname
					,@CreateDate
					);
	UPDATE bx_Users SET UpdateDate = GETDATE() WHERE [UserID] = @UserID  -- 用户最后更新时间也放在这
	SELECT @FeedID AS FeedID;
END
