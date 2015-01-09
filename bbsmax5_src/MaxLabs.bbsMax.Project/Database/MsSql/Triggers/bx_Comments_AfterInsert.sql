--添加评论触发器
EXEC bx_Drop 'bx_Comments_AfterInsert';

GO


CREATE TRIGGER [bx_Comments_AfterInsert]
	ON [bx_Comments]
	AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(Type int,TargetID int,TotalCount int);

	INSERT INTO @tempTable 
		SELECT DISTINCT Type,TargetID,
			(ISNULL((SELECT COUNT(*) FROM [bx_Comments] as m WITH (NOLOCK) WHERE m.TargetID = T.TargetID AND m.IsApproved = 1 AND m.Type = T.Type), 0))
		FROM [INSERTED] T;

	IF EXISTS ( SELECT * FROM @tempTable WHERE Type = 1 ) BEGIN

		UPDATE [bx_Users]
			SET
				TotalComments = T.TotalCount
			FROM @tempTable T
			WHERE
				T.TargetID = [bx_Users].UserID AND T.Type = 1;

   		--发出重新填充UserInfo的XCMD命令
		SELECT 'ResetUser' AS XCMD, TargetID AS UserID, TotalCount AS TotalComments FROM @tempTable WHERE Type = 1;

	END

	UPDATE [bx_BlogArticles]
		SET
			TotalComments = T.TotalCount,
			LastCommentDate = GETDATE()
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_BlogArticles].ArticleID AND T.Type = 2;
			
	UPDATE [bx_Doings]
		SET
			TotalComments = T.TotalCount,
			LastCommentDate = GETDATE()
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_Doings].DoingID AND T.Type = 3;
			
	UPDATE [bx_Photos]
		SET
			TotalComments = T.TotalCount,
			LastCommentDate = GETDATE()
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_Photos].PhotoID AND T.Type = 4;
			
	UPDATE [bx_UserShares]
		SET
			CommentCount = T.TotalCount
			--LastCommentDate = GETDATE()
		FROM @tempTable T
		WHERE
			T.TargetID = [bx_UserShares].UserShareID AND T.Type = 5;
			
			
		-- 动态枚举值 -> 评论类型枚举值
		--Share = 0 -> 5
		------------------UploadPicture = 2 -> 4 不要了
		--UpdateDoing = 5 -> 3
		--WriteArticle = 6 -> 2
		DECLARE @FeedCommentTable table(FType int,FTargetID int,FTotalCount int,Cids varchar(50));
		INSERT INTO @FeedCommentTable SELECT 
			CASE WHEN Type = 5 THEN 0
			--WHEN Type = 4 THEN 2
			WHEN Type = 3 THEN 5
			WHEN Type = 2 THEN 6
			ELSE -1 END
		   ,TargetID
		   ,ISNULL(TotalCount,0)
		   ,(SELECT CAST(ISNULL(MIN(CommentID),0) as varchar(20)) +','+ CAST(ISNULL(MAX(CommentID),0) as varchar(20)) FROM bx_Comments C WHERE C.Type = T.Type AND C.TargetID = T.TargetID AND C.IsApproved = 1)
		   FROM @tempTable T WHERE T.Type in(2,3,5) -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
		
		IF @@ROWCOUNT > 0 BEGIN
			UPDATE bx_Feeds SET CommentInfo = T.Cids + ',' + CAST(T.FTotalCount as varchar(20))	FROM @FeedCommentTable T 
				WHERE CommentTargetID = T.FTargetID AND ActionType = T.FType;  -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
				
			SELECT 'ResetFeedCommentInfo' AS XCMD, ID AS FeedID, CommentInfo FROM bx_Feeds F INNER JOIN @FeedCommentTable T ON F.CommentTargetID = T.FTargetID AND F.ActionType = T.FType;  -- TODO: AND AppID = 内置应用AppID   当有第三方应用动态时 要加上此判断
		END	
END