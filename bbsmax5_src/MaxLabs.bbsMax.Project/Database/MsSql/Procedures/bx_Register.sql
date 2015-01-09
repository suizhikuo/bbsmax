--用户注册
CREATE PROCEDURE bx_Register
	 @Username               nvarchar(50)
	,@Password               nvarchar(50)
	,@Email                  nvarchar(200)
	,@CreateIP               nvarchar(50)
	,@IPSameInterval         int     
	,@PasswordFormat         int
	,@Serial                 uniqueidentifier
	,@IsActive               bit

	,@BlogPrivacy			tinyint = 0
	,@FeedPrivacy			tinyint = 0
	,@BoardPrivacy			tinyint = 0
	,@DoingPrivacy			tinyint = 0
	,@AlbumPrivacy			tinyint = 0
	,@SpacePrivacy			tinyint = 0
	,@SharePrivacy			tinyint = 0
	,@FriendListPrivacy		tinyint = 0
	,@InformationPrivacy	tinyint = 0

	,@InviterID              int 
	,@Point0                 int
	,@Point1                 int
	,@Point2                 int
	,@Point3                 int
	,@Point4                 int
	,@Point5                 int
	,@Point6                 int
	,@Point7                 int
	
	,@RoleIDs	             text
	,@RoleEndDates           text  
	
	,@UserID                 int OUTPUT
	AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @ErrorCode  int;
	DECLARE @NewUserID  int;
	--DECLARE @InviterID int 
	
	--开始事务
	BEGIN TRANSACTION  REGISTER

	--检查用户名重复	
    IF EXISTS(SELECT [UserID] FROM bx_Users WHERE [Username]=@Username) BEGIN
		SET @ErrorCode = 1;
		GOTO Cleanup;
	END

	--检查email重复
	IF EXISTS(SELECT [UserID] FROM bx_Users WHERE [Email]=@Email) BEGIN
		SET @ErrorCode = 2;
		GOTO Cleanup;
	END
	
	--检查两次注册时间间隔
	IF @IPSameInterval>0 BEGIN
		DECLARE @LastSameIPDate DATETIME
		
		SET @LastSameIPDate = (SELECT MAX([CreateDate]) FROM [bx_Users] WHERE [CreateIP] = @CreateIP )
		
		IF NOT @LastSameIPDate IS NULL BEGIN
		
			DECLARE @Interval INT;
			SET @Interval = DATEDIFF(n, @LastSameIPDate, GETDATE());
			IF @Interval <= @IPSameInterval BEGIN
				SET @ErrorCode = 5;
				GOTO Cleanup;
			END
		END
	END

	--只要邀请码不是空的，就根据邀请码得到邀请人，如果不存在邀请码，则出错不继续注册
	IF @Serial IS NOT NULL AND @Serial<>'00000000-0000-0000-0000-000000000000' BEGIN
		SELECT @InviterID = UserID FROM [bx_InviteSerials] WHERE Serial = @Serial AND Status = 0 AND ExpiresDate > GETDATE();
		
		IF @InviterID IS NULL OR @InviterID = 0 BEGIN
			SET @ErrorCode = 3;
			GOTO Cleanup;
		END

	END

	--插入用户表		
	IF @UserID>0 BEGIN
		IF EXISTS (SELECT [UserID] FROM [bx_Users] WHERE [UserID] = @UserID)
		BEGIN
			SET @ErrorCode = 4;
			GOTO Cleanup;
		END
		--SET IDENTITY_INSERT [bx_Users] ON		
		INSERT INTO [bx_Users](
					[UserID]
				   ,[Username]
				   ,[Realname]
				   ,[Email]
                   ,[CreateIP]
                   ,[LastVisitIP]  
                   ,[IsActive]
                   ,[Point_1]
                   ,[Point_2]
                   ,[Point_3]
                   ,[Point_4]
                   ,[Point_5]
                   ,[Point_6]
                   ,[Point_7]
                   ,[Point_8]
				   )
				VALUES (
					@UserID
				   ,@Username
				   ,N''
				   ,@Email
                   ,@CreateIP
                   ,@CreateIP
                   ,@IsActive
                   ,@Point0
                   ,@Point1
                   ,@Point2
                   ,@Point3
                   ,@Point4
                   ,@Point5
                   ,@Point6
                   ,@Point7
				   );
				   
			IF( @@ERROR <> 0 )
			BEGIN
				--SET IDENTITY_INSERT [bx_Users] OFF
				SET @ErrorCode = -1;
				GOTO Cleanup;
			END		   
		--SET IDENTITY_INSERT [bx_Users] OFF
		
		SET @NewUserID = @UserID;
	END	
	ELSE BEGIN
		INSERT INTO [bx_Users](
						[Username]
					   ,[Realname]
					   ,[Email]
                       ,[CreateIP]
                       ,[LastVisitIP]
                       ,[IsActive]
                       ,[Point_1]
                       ,[Point_2]
                       ,[Point_3]
                       ,[Point_4]
                       ,[Point_5]
                       ,[Point_6]
                       ,[Point_7]
                       ,[Point_8]
					   )
					VALUES(
						@Username
					   ,N''
					   ,@Email
                       ,@CreateIP
                       ,@CreateIP
                       ,@IsActive
                       ,@Point0
                       ,@Point1
                       ,@Point2
                       ,@Point3
                       ,@Point4
                       ,@Point5
                       ,@Point6
                       ,@Point7
					   );		   
		IF(@@ERROR <> 0)
		BEGIN
			SET @ErrorCode = -1;
			GOTO Cleanup;
		END
		
		SET @NewUserID = @@IDENTITY; 	
	END

	IF @NewUserID<=0
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END

	--insert bx_UserVars
	INSERT INTO [bx_UserVars](
				 UserID
				,Password
				,PasswordFormat
				)
			VALUES(
				 @NewUserID
				,@Password
				,@PasswordFormat
				);
	IF(@@ERROR <> 0)
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END
	
	--insert bx_UserInfos
	INSERT INTO [bx_UserInfos](
				 UserID
				,InviterID
				,BlogPrivacy
				,FeedPrivacy
				,BoardPrivacy
				,DoingPrivacy
				,AlbumPrivacy
				,SpacePrivacy
				,SharePrivacy
				,FriendListPrivacy
				,InformationPrivacy
				)
			VALUES(
				 @NewUserID
				,@InviterID
				,@BlogPrivacy
				,@FeedPrivacy
				,@BoardPrivacy
				,@DoingPrivacy
				,@AlbumPrivacy
				,@SpacePrivacy
				,@SharePrivacy
				,@FriendListPrivacy
				,@InformationPrivacy
				);
	IF(@@ERROR <> 0)
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END

	--insert bx_Firends 如果有推荐人的话互相成好友，并更新新用户的推荐人字段、推荐人的推荐次数
	
	IF @InviterID IS NOT NULL AND @InviterID > 0 BEGIN

		IF @Serial IS NOT NULL AND @Serial<>'00000000-0000-0000-0000-000000000000'
			UPDATE [bx_InviteSerials] SET Status = 1, ToUserID = @NewUserID WHERE Serial=@Serial;
	
		INSERT INTO bx_Friends([UserID],[FriendUserID],[GroupID],[Hot],[CreateDate]) 
		VALUES(@NewUserID,@InviterID,0,0,GETDATE());

		INSERT INTO bx_Friends([UserID],[FriendUserID],[GroupID],[Hot],[CreateDate]) 
		VALUES(@InviterID,@NewUserID,0,0,GETDATE());

		UPDATE [bx_Users] SET [TotalInvite] = [TotalInvite] + 1 WHERE [UserID] = @InviterID;

	END
	
	IF(@@ERROR <> 0)
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END


	--初始化用户组
	
	DECLARE @RoleIDsTable table(autoid int, RoleID uniqueidentifier, EndDate datetime);
	
	INSERT @RoleIDsTable (autoid, RoleID) SELECT id,item FROM bx_GetStringTable_text(@RoleIDs,',');
	UPDATE @RoleIDsTable SET EndDate = CAST(t.item as datetime) FROM bx_GetStringTable_text(@RoleEndDates,',') as t WHERE t.id = autoid ;
	
	INSERT bx_UserRoles( UserID, RoleID,BeginDate, EndDate) SELECT @NewUserID, RoleID, '1753-1-1', EndDate FROM @RoleIDsTable;
	
	IF(@@ERROR <> 0)
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END
	
	--更新用户的总积分
	
	EXEC bx_UpdateUserGeneralPoint @NewUserID;

	SELECT @UserID = @NewUserID;

	IF(@@ERROR <> 0)
	BEGIN
		SET @ErrorCode = -1;
		GOTO Cleanup;
	END

	--将用户第一次注册的IP值插入IP变更日志表.
	INSERT INTO bx_IPLogs(UserID,Username,NewIP) VALUES(@UserID,@Username,@CreateIP);

EXEC bx_CreatePointLogs @UserID
,@Point0
,@Point1
,@Point2
,@Point3
,@Point4
,@Point5
,@Point6
,@Point7
,@Point0
,@Point1
,@Point2
,@Point3
,@Point4
,@Point5
,@Point6
,@Point7
,N'初始化'
,N'新用户注册'   --创建积分记录
	
	GOTO CommitTrans;

 CommitTrans:
	BEGIN
		COMMIT TRANSACTION REGISTER
		RETURN (0);
	END

 Cleanup:
    BEGIN
    	ROLLBACK TRANSACTION REGISTER
    	RETURN (@ErrorCode);
    END     

END


/*
@ErrorCode
  -1 未知错误
  1  用户名被占用
  2  Email被占用
  3  邀请码错误
  4  ID已经存在
  5  注册间隔时间太频繁
*/