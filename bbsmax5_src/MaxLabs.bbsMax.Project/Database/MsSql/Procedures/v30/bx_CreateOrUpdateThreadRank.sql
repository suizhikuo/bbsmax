-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/26>
-- Description:	<创建或者更新等级>
-- =============================================
CREATE PROCEDURE [bx_CreateOrUpdateThreadRank]
	@ThreadID int,
	@UserID int,
	@Rank tinyint
AS
	SET NOCOUNT ON 
	IF EXISTS (SELECT * FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND PostUserID=@UserID)
		RETURN (10)--不能给自己评级 2007-5-10 sek
	IF EXISTS (SELECT * FROM [bx_ThreadRanks] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND UserID=@UserID)
		 UPDATE [bx_ThreadRanks] SET Rank=@Rank,UpdateDate=getdate() WHERE ThreadID=@ThreadID AND UserID=@UserID 
	ELSE
		 INSERT INTO [bx_ThreadRanks] (
		[ThreadID],
		[UserID],
		[Rank]
		) VALUES (
		@ThreadID,
		@UserID,
		@Rank
		)
	--更新平均值－－－－
	DECLARE @Count int,@TotalRank int
	SELECT @Count=COUNT(*) FROM [bx_ThreadRanks] WITH (NOLOCK) WHERE ThreadID=@ThreadID
	SELECT @TotalRank=SUM(Rank) FROM [bx_ThreadRanks] WITH (NOLOCK) WHERE ThreadID=@ThreadID
	UPDATE [bx_Threads] SET Rank=@TotalRank/@Count WHERE ThreadID=@ThreadID
	
	return (0)


