-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/22>
-- Description:	<删除>
-- =============================================
CREATE PROCEDURE [bx_DeleteThreadRank]
	@ThreadID int,
	@UserID int
AS
	SET NOCOUNT ON 
	DELETE [bx_ThreadRanks] WHERE  ThreadID=@ThreadID AND UserID=@UserID
	IF @@ROWCOUNT > 0
		RETURN (0)
	ELSE
		RETURN (1)


