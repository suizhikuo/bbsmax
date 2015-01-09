--创建动态过滤设置
CREATE PROCEDURE bx_CreateFeedFilter
	 @AppID           uniqueidentifier
	,@UserID          int
	,@FriendUserID    int
	,@ActionType      tinyint
AS
BEGIN

   SET NOCOUNT ON;
   
   DECLARE @EmptyGuid uniqueidentifier;
   SET @EmptyGuid = '00000000-0000-0000-0000-000000000000'

   BEGIN TRANSACTION
   
   IF @FriendUserID IS NULL BEGIN --屏蔽所有好友的当前应用动作
	   IF EXISTS(SELECT * FROM [bx_FeedFilters] WHERE [AppID]=@AppID AND [ActionType]=@ActionType AND [UserID]=@UserID AND [FriendUserID] IS NULL)
			GOTO CommitTrans;
	   DELETE [bx_FeedFilters] WHERE [UserID]=@UserID AND [AppID] = @AppID
	   IF(@@error<>0)
					GOTO Cleanup;
   END                
   ELSE IF @ActionType IS null BEGIN 
			IF EXISTS(SELECT * FROM [bx_FeedFilters] WHERE [AppID]=@AppID AND [ActionType] IS NULL AND [UserID]=@UserID AND [FriendUserID]=@FriendUserID)
				GOTO CommitTrans; 
	        IF @AppID = @EmptyGuid BEGIN --屏蔽当前好友的所有动态
				DELETE [bx_FeedFilters] WHERE [UserID]=@UserID AND [FriendUserID]=@FriendUserID
				IF(@@error<>0)
					GOTO Cleanup;
		    END
   END
   ELSE BEGIN
        --存在过滤所有好友的当前应用动作 设置 
		IF EXISTS(SELECT * FROM [bx_FeedFilters] WHERE [AppID]=@AppID AND [ActionType]=@ActionType AND [UserID]=@UserID AND [FriendUserID] IS NULL)
			GOTO CommitTrans;
	    --存在过滤当前好友的所有动态
		IF EXISTS(SELECT * FROM [bx_FeedFilters] WHERE [AppID]=@EmptyGuid AND [UserID]=@UserID AND [FriendUserID]=@FriendUserID)
			GOTO CommitTrans;
		--存在过滤当前好友的当前应用动作 设置 
		IF EXISTS(SELECT * FROM [bx_FeedFilters] WHERE [AppID]=@AppID AND [ActionType]=@ActionType AND [UserID]=@UserID AND [FriendUserID]=@FriendUserID)
			GOTO CommitTrans;
   END

    INSERT INTO [bx_FeedFilters](
                     [AppID] 
                    ,[UserID]
                    ,[FriendUserID]
                    ,[ActionType]
                    ) VALUES (
                     @AppID 
                    ,@UserID
                    ,@FriendUserID
                    ,@ActionType
                    );
    IF(@@error<>0)
		GOTO Cleanup;
	ELSE BEGIN
		GOTO CommitTrans;
	END
	
	
 CommitTrans:
	BEGIN
		COMMIT TRANSACTION
		RETURN (0);
	END
                    
 Cleanup:
    BEGIN
    	ROLLBACK TRANSACTION
    	RETURN (-1)
    END
     
                    
END
