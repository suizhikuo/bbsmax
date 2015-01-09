CREATE VIEW bx_AuthUsers
AS
SELECT bx_Users.*
, bx_UserVars.Password
, bx_UserVars.PasswordFormat
, bx_UserVars.UnreadMessages
, bx_UserVars.UsedAlbumSize
, bx_UserVars.AddedAlbumSize
, bx_UserVars.TimeZone
, bx_UserVars.EverAvatarChecked
, bx_UserVars.EnableDisplaySidebar
, bx_UserVars.OnlineStatus, bx_UserVars.SkinID
, bx_UserVars.LastReadSystemNotifyID
, bx_UserVars.UsedDiskSpaceSize
, bx_UserVars.TotalDiskFiles
, bx_UserVars.LastAvatarUpdateDate
, bx_UserVars.LastImpressionDate
, bx_UserVars.SelectFriendGroupID
, bx_UserVars.ReplyReturnThreadLastPage
FROM bx_Users Left JOIN
      bx_UserVars ON bx_Users.UserID = bx_UserVars.UserID