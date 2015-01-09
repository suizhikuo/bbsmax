--===获取热门帖子by gyb
CREATE PROCEDURE [bx_GetThreadsHot]
    @PageIndex INT,
    @PageSize INT,
	@Days INT, --  在几天之内 小于0则获取所有时间
	@ForumIDs VARCHAR(8000), --''为获取所有版块
	@HotThreadRepliesReqired varchar(10)='20'
AS
	
BEGIN 
   SET NOCOUNT ON;
  

  DECLARE @Condition NVARCHAR(1000)

   SET @Condition =' [TotalReplies]>='+@HotThreadRepliesReqired + ' AND [SortOrder]<4000000000000000 ';  
  IF @Days>0 
        SET @Condition='[CreateDate] > (GETDATE()-'+str(@Days)+') AND '+@Condition;
  IF @ForumIDs <> ''
		SET @Condition = ' [ForumID] IN('+@ForumIDs+') AND '+@Condition;
 
  DECLARE @ReturnValue INT
  EXECUTE @ReturnValue = bx_Common_GetRecordsByPage_LongCondition @PageIndex, @PageSize, N'bx_Threads', N'*', @Condition, N'[TotalReplies]', 1
  IF @ReturnValue=1
		SELECT @ReturnValue
END