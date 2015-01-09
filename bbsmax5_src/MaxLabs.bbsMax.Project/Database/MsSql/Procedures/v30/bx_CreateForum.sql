CREATE PROCEDURE [bx_CreateForum] 
	@ParentID int,
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
	@ForumID int output,
	@ThreadCatalogStatus tinyint,
	@ColumnSpan tinyint,
	@SortOrder int
AS
	SET NOCOUNT ON 
	
	IF(EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE CodeName=@CodeName))
	begin
	set @ForumID = 0
	RETURN (13)	
	end
	
	IF ((@ParentID=0) OR (EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE ForumID=@ParentID)) )
	BEGIN
	
	DECLARE @Condition varchar(50)
	SET @Condition='ParentID='+str(@ParentID)
	--EXECUTE bbsMax_Common_GetSortOrder 'bx_Forums',@Condition,@SortOrder output
	
	INSERT INTO [bx_Forums] (
	[ParentID],
	[ForumType],
	[ThreadCatalogStatus],
	[CodeName],
	[ForumName],
	[Description],
	[Readme],
	[LogoSrc],
	[ThemeID],
	[Password],
	--[ForumSettingID],
	[PermissionSchemeID],
	[PointSchemeID],
	[SortOrder],
	[ExtendedAttributes],
	[ColumnSpan]
) VALUES (
	@ParentID,
	@ForumType,
	@ThreadCatalogStatus,
	@CodeName,
	@ForumName,
	@Description,
	@Readme,
	@LogoUrl,
	@ThemeID,
	@Password,
	--@ForumSettingID,
	@PermissionSchemeID,
	@PointSchemeID,
	@SortOrder,
	@ExtendedAttributes,
	@ColumnSpan
)
		set @ForumID = @@IDENTITY;
		RETURN (0)
	END
	
	ELSE
	begin
	set @ForumID = 0
		RETURN (-1)
		end


