-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/31>
-- Description:	<移动帖子>
-- =============================================
CREATE PROCEDURE [bx_MoveThreads] 
	@OldForumID int,
	@NewForumID int,
	@ThreadIdentities varchar(8000),
	@IsKeepLink bit
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE ForumID=@NewForumID AND ParentID<>0)
		BEGIN
			DECLARE @ThreadID int,@SortOrder bigint,@i int
			BEGIN TRANSACTION

			SET @ThreadIdentities=@ThreadIdentities+N','
			SELECT @i=CHARINDEX(',',@ThreadIdentities)
			
			DECLARE @tempTable1 TABLE(TempThreadID INT,TempSortOrder BIGINT)
			DECLARE @tempTable2 TABLE(TempThreadID INT,TempSortOrder BIGINT,TempThreadCatalogID INT,TempThreadType TINYINT,TempThreadStatus TINYINT,TempIconID INT,TempSubject NVARCHAR(256),TempPostUserID INT,TempPostNickName NVARCHAR(64),TempLastPostUserID INT,TempLastPostNickName NVARCHAR(64),TempCreateDate datetime,TempUpdateDate datetime)
			DECLARE @tempTable3 TABLE(TempCatalogID INT,TempTotalThreads INT)

			SELECT @SortOrder=ISNULL(MAX(SortOrder),0) FROM bx_Threads WITH (NOLOCK) WHERE SortOrder<2000000000000000

			DECLARE @ThreadCount INT
			SET @ThreadCount=0;
			WHILE(@i>1) BEGIN
				SELECT @ThreadID=SUBSTRING(@ThreadIdentities,0, @i)
				--SET @OldSortOrder=-1
				IF(@IsKeepLink=1) BEGIN
					INSERT INTO @tempTable2 SELECT @ThreadID,SortOrder,ThreadCatalogID,ThreadType,6,IconID,(CAST(ThreadID as nvarchar(16))+N','+Subject),PostUserID,PostNickName,LastPostUserID,LastPostNickName,CreateDate,UpdateDate FROM  bx_Threads WITH (NOLOCK) WHERE ForumID = @OldForumID AND ThreadID=@ThreadID
					--IF EXISTS(SELECT * FROM @tempTable3 WHERE TempCatalogID=(SELECT TempCatalogID WHERE)) 
					DECLARE @CatalogID int
					SELECT @CatalogID=TempThreadCatalogID FROM @tempTable2 WHERE  TempThreadID=@ThreadID
					IF EXISTS(SELECT * FROM @tempTable3 WHERE TempCatalogID=@CatalogID) 
						UPDATE @tempTable3 SET TempTotalThreads=TempTotalThreads+1 WHERE TempCatalogID=@CatalogID
					ELSE
						INSERT INTO @tempTable3 VALUES(@CatalogID,1)
				END
				SET @SortOrder=@SortOrder+1;
				INSERT INTO @tempTable1 VALUES(@ThreadID,@SortOrder)

				SELECT @ThreadIdentities=SUBSTRING(@ThreadIdentities,@i+1,LEN(@ThreadIdentities)-@i)
				SELECT @i=CHARINDEX(',',@ThreadIdentities)
				
				SET @ThreadCount=@ThreadCount+1;
			END

			UPDATE bx_Threads SET ForumID=@NewForumID,SortOrder=TempSortOrder FROM @tempTable1 WHERE ThreadID = TempThreadID
			IF @@error<>0 BEGIN
				ROLLBACK TRANSACTION
				RETURN (-1)
			END
			IF(@IsKeepLink=1) BEGIN
				INSERT INTO bx_Threads(ForumID,ThreadCatalogID,ThreadType,IconID,Subject,PostUserID,PostNickName,LastPostUserID,LastPostNickName,CreateDate,UpdateDate,SortOrder) select @OldForumID,TempThreadCatalogID,12,TempIconID,TempSubject,TempPostUserID,TempPostNickName,TempLastPostUserID,TempLastPostNickName,TempCreateDate,TempUpdateDate,TempSortOrder FROM @tempTable2
				IF @@error<>0 BEGIN
					ROLLBACK TRANSACTION
					RETURN (-1)
				END
				UPDATE bx_Forums SET TotalThreads=TotalThreads+@ThreadCount WHERE  ForumID=@OldForumID
				UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+T3.TempTotalThreads FROM  @tempTable3 T3 WHERE ForumID=@OldForumID AND ThreadCatalogID=T3.TempCatalogID
			END
			COMMIT TRANSACTION
			RETURN 0
/*			
			WHILE(@i>1)
				BEGIN
					SELECT @ThreadID=SUBSTRING(@ThreadIdentities,0, @i)
					
					DECLARE @OldSortOrder bigint,@OldThreadCatalogID int
					SET @OldSortOrder=-1
					IF(@IsKeepLink=1)
						SELECT @OldSortOrder = SortOrder,@OldThreadCatalogID=ThreadCatalogID FROM bx_Threads WITH (NOLOCK) WHERE ThreadID=@ThreadID AND ForumID=@OldForumID
					
					SELECT @SortOrder=ISNULL(MAX(SortOrder)+1,0) FROM bx_Threads WITH (NOLOCK) WHERE SortOrder<500000000000000
					UPDATE [bx_Threads] SET ForumID=@NewForumID,SortOrder=@SortOrder WHERE ThreadID=@ThreadID  AND ForumID=@OldForumID
					SELECT @e2=@@error
					
					IF(@IsKeepLink=1 and @OldSortOrder<>-1)
						BEGIN
							INSERT bx_Threads(ForumID,ThreadCatalogID,ThreadType,ThreadStatus,IconID,Subject,PostUserID,PostNickName,SortOrder) select @OldForumID,@OldThreadCatalogID,ThreadType,6,IconID,(CAST(ThreadID as nvarchar(16))+N','+Subject),PostUserID,PostNickName,@OldSortOrder FROM bx_Threads WITH (NOLOCK) WHERE ThreadID=@ThreadID
							SELECT @e1=@@error
						END
					
					IF(@e1<>0 OR @e2<>0)
						BREAK
					
					SELECT @ThreadIdentities=SUBSTRING(@ThreadIdentities,@i+1,LEN(@ThreadIdentities)-@i)
					SELECT @i=CHARINDEX(',',@ThreadIdentities)
				END
				
			IF (@e1=0 AND @e2=0)
				BEGIN
					COMMIT TRANSACTION
					--更新bx_Forums--
					EXECUTE bx_UpdateForumData @OldForumID
					EXECUTE bx_UpdateForumData @NewForumID
					RETURN (0)
				END
			ELSE
				BEGIN
					ROLLBACK TRANSACTION
					RETURN (-1)
				END
				*/
		END
	ELSE
		RETURN (-1)

END


