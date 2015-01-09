-- =============================================
-- Author:		SEK
-- Create date: <2006/12/29>
-- Description:	<添加日志>
-- =============================================
CREATE PROCEDURE [bx_CreateThreadManageLog]
	@UserID int,
	@UserName varchar(64),
	@NickName varchar(64),
	@IPAddress varchar(15),
	@PostUserIDs varchar(8000),
	@ActionType tinyint,
	@ForumID int,
	@ThreadIDs varchar(8000),
	@ThreadSubjects ntext,
	@Reason nvarchar(256),
	@IsPublic bit
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @TempTable TABLE(LogID INT IDENTITY(1,1),ThreadID INT,PostUserID INT,ThreadSubject nvarchar(256) COLLATE Chinese_PRC_CI_AS_WS)

	INSERT @TempTable (ThreadID) 
		SELECT item FROM bx_GetIntTable(@ThreadIDs,',')

	UPDATE @TempTable
			SET [ThreadSubject]=T.item
			FROM (SELECT * FROM bx_GetStringTable_ntext(@ThreadSubjects,N',')) T
			WHERE T.id=LogID;

	UPDATE @TempTable
			SET [PostUserID]=T.item
			FROM (SELECT * FROM bx_GetIntTable(@PostUserIDs, ',')) T
			WHERE T.id=LogID;

	INSERT INTO [bx_ThreadManageLogs] (
		[UserID],
		[UserName],
		[NickName],
		[IPAddress],
		[PostUserID],
		[ActionType],
		[ForumID],
		[ThreadID],
		[ThreadSubject],
		[Reason],
		[CreateDate],
		[IsPublic]
	) SELECT @UserID,@UserName,@NickName,@IPAddress,PostUserID,@ActionType,@ForumID,ThreadID,ThreadSubject,@Reason,getdate(),@IsPublic FROM @TempTable;

	----更新主题日志记录------
	IF @ActionType <> 1 AND @ActionType <> 17 AND @ActionType <> 18 AND @IsPublic = 1
		UPDATE bx_Threads Set ThreadLog = @NickName + '|' + CAST(@ActionType as NVARCHAR) + '|' + CAST(getdate() AS NVARCHAR) WHERE ThreadID IN (SELECT ThreadID FROM @TempTable);

END