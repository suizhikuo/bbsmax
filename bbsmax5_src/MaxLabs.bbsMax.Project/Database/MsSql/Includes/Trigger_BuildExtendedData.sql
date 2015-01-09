	SET NOCOUNT ON;

	IF (OBJECT_ID('tempdb..#bx_tmp_usrext') IS NULL) BEGIN
		CREATE TABLE #bx_tmp_usrext ( UserID int, Data ntext DEFAULT('') );
	END
	ELSE BEGIN
		TRUNCATE TABLE #bx_tmp_usrext;
	END

	INSERT INTO #bx_tmp_usrext (UserID) SELECT DISTINCT UserID FROM $table$;

	DECLARE @UserID int;
	DECLARE @i int;
	DECLARE @ptr binary(16);

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalRoles int;

	DECLARE @userRoles table( ID int IDENTITY(1,1), UserID int, RoleID uniqueidentifier, BeginDate datetime, EndDate datetime );
	INSERT INTO @userRoles SELECT UserID, RoleID, BeginDate, EndDate FROM bx_UserRoles WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM $table$);

	SET @TotalRoles = @@ROWCOUNT;

	DECLARE
		@RoleIDString varchar(50),
		@BeginDateString varchar(30),
		@EndDateString varchar(30),
		@UserRoleString varchar(100);
	SET @i = 1;
	WHILE (@i <= @TotalRoles) BEGIN
		SELECT @UserID = UserID, @RoleIDString = CONVERT(varchar(50), RoleID), @BeginDateString = CONVERT(varchar(30), BeginDate, 25), @EndDateString = CONVERT(varchar(30), EndDate, 25) FROM @userRoles WHERE ID = @i;
		
		SET @UserRoleString = 'R' + CAST(LEN(@RoleIDString) AS varchar(10))
								+ ',' + CAST(LEN(@BeginDateString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ':' + @RoleIDString
								+ @BeginDateString
								+ @EndDateString;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserRoleString;

		SET @i = @i + 1;
	END

	--以下是处理用户扩展字段的冗余内容

	DECLARE @TotalValues int;

	DECLARE @userValues table( ID int IDENTITY(1,1), UserID int, FieldID varchar(36), Value nvarchar(3950),PrivacyType tinyint);
	INSERT INTO @userValues SELECT UserID, ExtendedFieldID AS FieldID, RTRIM(LTRIM(Value)),PrivacyType FROM bx_UserExtendedValues WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM $table$);

	SET @TotalValues = @@ROWCOUNT;

	DECLARE
		@FieldIDString varchar(36),
		@ValueString nvarchar(3950),
		@PrivacyType varchar(10),
		@UserValueString varchar(4000);
	SET @i = 1;
	WHILE (@i <= @TotalValues) BEGIN
		SELECT @UserID = UserID, @FieldIDString = FieldID, @ValueString = Value ,@PrivacyType = PrivacyType  FROM @userValues WHERE ID = @i;
		
		SET @UserValueString = 'F' + CAST(LEN(@FieldIDString) AS varchar(10))
								+ ',' + CAST(LEN(@ValueString) AS varchar(10))
								+ ',' + CAST(LEN(@PrivacyType) AS varchar(10))
								+ ':' + @FieldIDString
								+ @ValueString
								+ @PrivacyType;

		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserValueString;

		SET @i = @i + 1;
	END

	--以下是处理用户勋章

	DECLARE @TotalMedals int;

	DECLARE @userMedals table( ID int IDENTITY(1,1), UserID int, MedalID int, MedalLevelID int, EndDate datetime, CreateDate datetime, Url nvarchar(200));
	INSERT INTO @userMedals SELECT UserID, MedalID, MedalLevelID, EndDate, CreateDate, ISNULL(Url,'') FROM bx_UserMedals WITH (NOLOCK) WHERE UserID IN (SELECT UserID FROM $table$);

	SET @TotalMedals = @@ROWCOUNT;

	DECLARE
		@MedalIDString varchar(50),
		@MedalLevelIDString varchar(50),
		@CreateDateString varchar(30),
		@Url nvarchar(200),
		@UserMedalString nvarchar(500);
	SET @i = 1;
	WHILE (@i <= @TotalMedals) BEGIN
		SELECT @UserID = UserID, @MedalIDString = CONVERT(varchar(50), MedalID), @MedalLevelIDString = CONVERT(varchar(50), MedalLevelID), @EndDateString = CONVERT(varchar(30), EndDate, 25), @CreateDateString = CONVERT(varchar(30), CreateDate, 25),@Url = Url FROM @userMedals WHERE ID = @i;
		
		SET @UserMedalString = 'M' + CAST(LEN(@MedalIDString) AS varchar(10))
								+ ',' + CAST(LEN(@MedalLevelIDString) AS varchar(10))
								+ ',' + CAST(LEN(@EndDateString) AS varchar(10))
								+ ',' + CAST(LEN(@CreateDateString) AS varchar(10))
								+','  + CAST(LEN(@Url) AS varchar(10))
								+ ':' + @MedalIDString
								+ @MedalLevelIDString
								+ @EndDateString
								+ @CreateDateString
								+ @Url;
		
		SELECT @ptr = textptr(Data) FROM #bx_tmp_usrext WHERE UserID = @UserID;
		UPDATETEXT #bx_tmp_usrext.Data @ptr NULL NULL @UserMedalString;

		SET @i = @i + 1;
	END




	UPDATE bx_Users
	   SET ExtendedData = T.Data
	   FROM #bx_tmp_usrext T
	   WHERE T.UserID = bx_Users.UserID;