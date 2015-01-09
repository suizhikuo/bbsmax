CREATE VIEW bx_Members
AS
SELECT bx_Users.*, bx_UserInfos.InviterID, bx_UserInfos.TotalFriends, 
      bx_UserInfos.Birthday, bx_UserInfos.BirthYear, 
      bx_UserInfos.BlogPrivacy, bx_UserInfos.FeedPrivacy, 
      bx_UserInfos.BoardPrivacy, bx_UserInfos.DoingPrivacy, 
      bx_UserInfos.AlbumPrivacy, bx_UserInfos.SpacePrivacy, 
      bx_UserInfos.SharePrivacy, bx_UserInfos.FriendListPrivacy, 
      bx_UserInfos.InformationPrivacy, bx_UserInfos.NotifySetting
FROM bx_UserInfos INNER JOIN
      bx_Users ON bx_UserInfos.UserID = bx_Users.UserID