-- =============================================
-- Author:		<sek>
-- Create date: <2007/1/18>
-- Description:	<获取投票用户>
-- =============================================
CREATE PROCEDURE [bx_GetVotedUsers]
	@ThreadID int
AS
	SET NOCOUNT ON
	SELECT PollItemDetail.* FROM [bx_PollItemDetails] PollItemDetail WITH(NOLOCK) INNER JOIN [bx_PollItems] PollItem WITH(NOLOCK) ON PollItemDetail.ItemID=PollItem.ItemID WHERE PollItem.ThreadID=@ThreadID 
	RETURN


