CREATE PROCEDURE [bx_UpdateForum] 
	@ForumID int,
	--@ParentID int,
	@ForumType tinyint,
	@CodeName nvarchar(128),
	@ForumName nvarchar(1024),
	@Description ntext,
	@Readme ntext,
	@LogoUrl nvarchar(256),
	@ThemeID nvarchar(64),
	@Password nvarchar(64),
	--@ForumSettingID int,
	@PermissionSchemeID int,
	@PointSchemeID int,
	@ExtendedAttributes [ntext],
	@ThreadCatalogStatus tinyint,
	@ColumnSpan tinyint,
	@SortOrder int
AS
	SET NOCOUNT ON 
	IF(EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE CodeName=@CodeName AND ForumID <> @ForumID))
	RETURN (13)	

	--IF ((@ParentID=0) OR (EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE ForumID=@ParentID)) )
	--BEGIN
	--IF(@ParentID=@ForumID)
	--RETURN (14)
	
	UPDATE [bx_Forums] SET
	--[ParentID] = @ParentID,
	[ForumType] = @ForumType,
	[ThreadCatalogStatus] = @ThreadCatalogStatus,
	[CodeName] = @CodeName,
	[ForumName] = @ForumName,
	[Description] = @Description,
	[Readme]=@Readme,
	[LogoSrc] = @LogoUrl,
	[ThemeID] = @ThemeID,
	[Password] = @Password,
	--[ForumSettingID] = @ForumSettingID,
	[PermissionSchemeID] = @PermissionSchemeID,
	[PointSchemeID] = @PointSchemeID,
	[ExtendedAttributes] = @ExtendedAttributes,
	[ColumnSpan] = @ColumnSpan,
	[SortOrder] = @SortOrder
WHERE
	[ForumID] = @ForumID
	
		RETURN (0)
	--END
	
	--ELSE
		--RETURN (-1)


