
-- =============================================
-- Author:		<sek>
-- Create date: <2007/2/9>
-- Description:	<批量创建表情，如果不存在组则创建>
-- =============================================
CREATE PROCEDURE [bx_CreateEmoticonsAndGroups]
	@UserID int,
    @GroupNames ntext,
	@GroupOrders text,
    @Shortcuts ntext,
    @FileNames text,
    @FileSizes text
AS
	SET NOCOUNT ON 
	--DECLARE @GroupName nvarchar(50),@m int,@n int,@k int,@j int,@ReturnValue int--,@ErrorCode int

	DECLARE @EmoticonGroupsTable table(
		[ID1] INT IDENTITY(1,1),
		[GroupID1] int,
		[GroupName1] nvarchar(64) COLLATE Chinese_PRC_CI_AS_WS,
		[MaxSortOrder1] bigint
		);
	
	BEGIN TRANSACTION
			
	INSERT INTO @EmoticonGroupsTable(GroupName1)
	SELECT item FROM bx_GetStringTable_ntext(@GroupNames, '|');
	IF(@@error<>0)
		GOTO Cleanup

	
	INSERT INTO [bx_EmoticonGroups] ([GroupName],UserID) 
	SELECT [GroupName1],@UserID FROM @EmoticonGroupsTable WHERE [GroupName1] NOT IN (SELECT GroupName FROM [bx_EmoticonGroups] WITH(NOLOCK) WHERE UserID = @UserID )
	IF(@@error<>0)
		GOTO Cleanup
		
	UPDATE @EmoticonGroupsTable SET GroupID1=T.GroupID,
		 MaxSortOrder1=ISNULL((SELECT MAX(SortOrder) FROM [bx_Emoticons] E WHERE E.GroupID=T.GroupID),0)
		 FROM [bx_EmoticonGroups] T WHERE T.GroupName=GroupName1 AND T.UserID=@UserID
	IF(@@error<>0)
		GOTO Cleanup	

	DECLARE @EmoticonsTable table(
		[ID2] INT IDENTITY(1,1),
		[GroupOrder] int,
		[GroupID2] int,
		[Shortcut] nvarchar(64) COLLATE Chinese_PRC_CI_AS_WS,
		[ImageSrc] nvarchar(256),
		[FileSize] bigint,
		[MaxSortOrder2] bigint
		);
		
	INSERT INTO @EmoticonsTable([GroupOrder])
	SELECT item FROM bx_GetStringTable_text(@GroupOrders, '|');
	IF(@@error<>0)
		GOTO Cleanup		
		
	UPDATE @EmoticonsTable
		SET [Shortcut]=T.item
		FROM (SELECT * FROM bx_GetStringTable_ntext(@Shortcuts, '|')) T
		where T.id=ID2;
	IF(@@error<>0)
		GOTO Cleanup
				
	UPDATE @EmoticonsTable
		SET [ImageSrc]=T.item
		FROM (SELECT * FROM bx_GetStringTable_text(@FileNames, '|')) T
		where T.id=ID2;	
	IF(@@error<>0)
		GOTO Cleanup
				
	UPDATE @EmoticonsTable
		SET [FileSize]=T.item
		FROM (SELECT * FROM bx_GetStringTable_text(@FileSizes, '|')) T
		where T.id=ID2;	
	IF(@@error<>0)
		GOTO Cleanup
		    
    UPDATE @EmoticonsTable SET GroupID2=GroupID1,MaxSortOrder2=MaxSortOrder1 FROM @EmoticonGroupsTable WHERE ID1=GroupOrder
	IF(@@error<>0)
		GOTO Cleanup
	--UPDATE @EmoticonsTable SET TempGroupID=T.GroupID,TempSortOrder=(SELECT MAX(SortOrder) FROM [bx_Emoticons] WHERE GroupID=T.GroupID) FROM [bx_EmoticonGroups] T WHERE T.GroupName=TempGroupName
	
	INSERT INTO [bx_Emoticons](
			[GroupID],
			[UserID],
			[Shortcut],
			[ImageSrc],
			[FileSize],
			[SortOrder])
		SELECT [GroupID2],
			@UserID,
			[Shortcut],
			[ImageSrc],
			[FileSize],
			[MaxSortOrder2]+ID2
		FROM @EmoticonsTable
	
			IF(@@error<>0)
				GOTO Cleanup		
		

--	UPDATE [bx_EmoticonGroups] SET [TotalEmoticons] = ISNULL((SELECT COUNT(1) FROM [bx_Emoticons] E WITH (NOLOCK) WHERE E.GroupID=T.GroupID1), 0)
--		, [TotalSizes] = ISNULL((SELECT SUM(FileSize) FROM [bx_Emoticons] E WITH (NOLOCK) WHERE E.GroupID=T.GroupID1),0)
--		FROM @EmoticonGroupsTable T WHERE [GroupID1] = GroupID;
	IF(@@error<>0)
			GOTO Cleanup

		
		COMMIT TRANSACTION
			RETURN (0)

Cleanup:
    BEGIN
    	ROLLBACK TRANSACTION
    	RETURN (-1)
    END

