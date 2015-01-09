-- =============================================
-- Author:		<sek>
-- Create date: <2007/1/18>
-- Description:	<获取投票>
-- =============================================
CREATE PROCEDURE [bx_GetPoll]
	@ThreadID int,
	@IsGetVotedUsers bit
AS
	SET NOCOUNT ON
	SELECT * FROM [bx_Polls] WITH(NOLOCK) WHERE ThreadID=@ThreadID
	
	--SEK 2007.2.27---
	SELECT * FROM [bx_PollItems] WITH(NOLOCK) WHERE ThreadID=@ThreadID
	IF(@IsGetVotedUsers=1)
		SELECT PollItemDetail.ItemID,PollItemDetail.UserID,PollItemDetail.NickName FROM [bx_PollItems] PollItem WITH(NOLOCK) INNER JOIN [bx_PollItemDetails] PollItemDetail WITH(NOLOCK) ON PollItem.ItemID=PollItemDetail.ItemID WHERE PollItem.ThreadID=@ThreadID
	/*
	IF(@IsGetVotedUsers=0)
		SELECT * FROM [bx_PollItems] WITH(NOLOCK) WHERE ThreadID=@ThreadID
	ELSE
		SELECT PollItem.*,PollItemDetail.UserID,PollItemDetail.NickName FROM [bx_PollItems] PollItem WITH(NOLOCK) INNER JOIN [bx_PollItemDetails] PollItemDetail WITH(NOLOCK) ON PollItem.ItemID=PollItemDetail.ItemID WHERE PollItem.ThreadID=@ThreadID
		--SELECT PollItemDetail.* FROM [bx_PollItemDetails] PollItemDetail WITH(NOLOCK) INNER JOIN [bx_PollItems] PollItem WITH(NOLOCK) ON PollItemDetail.ItemID=PollItem.ItemID WHERE PollItem.ThreadID=@ThreadID 
		*/
	RETURN


