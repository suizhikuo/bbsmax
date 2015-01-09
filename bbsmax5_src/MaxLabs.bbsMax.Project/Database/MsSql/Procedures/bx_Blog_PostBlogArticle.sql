--写日志

CREATE PROCEDURE [bx_Blog_PostBlogArticle] 
      @UserID            int
     ,@CategoryID        int
     ,@IsApproved        bit
     ,@EnableComment     bit
     ,@PrivacyType       tinyint
     ,@CreateIP          varchar(50)
     ,@Thumb             nvarchar(200)
     ,@Subject           nvarchar(200)
     ,@Password          nvarchar(50)
     ,@Content           ntext
     ,@ArticleID         int                OUTPUT

AS BEGIN

	SET NOCOUNT ON; 
	
	BEGIN TRANSACTION
	
	--是否有传输用户的日志分类
	IF NOT EXISTS (SELECT [CategoryID] FROM [bx_BlogCategories] WHERE [CategoryID] = @CategoryID AND [UserID] = @UserID) BEGIN  --判断是否有指定分类
	 
		DECLARE @CategoryCount int;
		IF EXISTS (SELECT * FROM [bx_BlogCategories] WHERE [UserID] = @UserID)
			SET @CategoryID = 0; --如果用户有日志分类,或只是传输了错误值的话,插入NULL
		ELSE BEGIN
			SET @CategoryID = 0; --使用刚自动新建的这个新分类
		END
	END
	
	IF(@@error<>0)
	GOTO Cleanup;	
		
	IF @ArticleID > 0 BEGIN --如果有传输日志ID,则表示编辑
	    
		UPDATE 
			[bx_BlogArticles]
		SET 
			[LastEditUserID] = @UserID
           ,[CategoryID] = @CategoryID
           ,[IsApproved] = @IsApproved
           ,[EnableComment] = @EnableComment
           ,[PrivacyType] = @PrivacyType
           ,[CreateIP] = @CreateIP
           ,[Thumb] = @Thumb
           ,[Subject] = @Subject
           ,[Password] = @Password
           ,[Content] = @Content
           ,[UpdateDate] = GETDATE()
           ,[KeywordVersion] = ''
        WHERE 
			ArticleID = @ArticleID;

    END
	ELSE BEGIN
	
		INSERT INTO [bx_BlogArticles](
			[UserID]
			,[LastEditUserID]
			,[CategoryID]
			,[IsApproved]
			,[EnableComment]
			,[PrivacyType]
			,[CreateIP]
			,[Thumb]
			,[Subject]
			,[Password]
			,[Content]
		) VALUES (
			@UserID
			,@UserID
			,@CategoryID
			,@IsApproved
			,@EnableComment
			,@PrivacyType
			,@CreateIP
			,@Thumb
			,@Subject
			,@Password
			,@Content
		);
		
		SET @ArticleID = @@IDENTITY;
	END
		  
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
