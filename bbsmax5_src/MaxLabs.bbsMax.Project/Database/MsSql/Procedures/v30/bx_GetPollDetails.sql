-- =============================================
-- Author:		<sek>
-- Create date: <2007/1/18>
-- Description:	<获取投票>
-- =============================================
CREATE PROCEDURE [bx_GetPollDetails] 
	@ThreadID int
AS
	SET NOCOUNT ON
	SELECT * FROM [bx_Polls] WITH(NOLOCK) WHERE ThreadID=@ThreadID
	SELECT PollItem.*,PollItemDetail.UserID,PollItemDetail.NickName FROM [bx_PollItems] PollItem WITH(NOLOCK) INNER JOIN [bx_PollItemDetails] PollItemDetail WITH(NOLOCK) ON PollItem.ItemID=PollItemDetail.ItemID WHERE PollItem.ThreadID=@ThreadID

	RETURN


