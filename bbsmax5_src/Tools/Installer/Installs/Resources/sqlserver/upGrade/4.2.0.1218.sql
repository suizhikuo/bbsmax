IF NOT EXISTS(SELECT * FROM syscolumns WHERE [id] =object_id('bx_UserInfos') AND [name] ='UnreadPropNotifies')
	EXEC ('ALTER TABLE [bx_UserInfos] ADD [UnreadPropNotifies] [int] NOT NULL DEFAULT (0)');


IF EXISTS(SELECT * FROM sysobjects WHERE [type]=N'TR' AND [name]=N'bx_UserInfos_AfterUpdate')
	EXEC ('ALTER TABLE bx_UserInfos DISABLE TRIGGER bx_UserInfos_AfterUpdate');

EXEC ('
Update bx_UserInfos SET
  UnreadPropNotifies = 0 --道具
 ,UnreadSystemNotifies = 0 --管理
  ,UnreadBidUpNotifies =0 --竞价排名
  ,UnreadHailNotifies =0 --打招呼
  ,UnreadFriendNotifies = 0 --好友通知
  ,UnreadPostNotifies = 0 --评论
');

EXEC ('
DECLARE @tempTable TABLE(u INT,c INT ,nc INT);

INSERT INTO @tempTable SELECT UserID,Category , count(*) FROM bx_Notifies  GROUP BY bx_Notifies.UserID,Category
UPDATE bx_UserInfos SET UnreadPropNotifies = T.nc  FROM(SELECT u, nc FROM @tempTable WHERE c = 21) AS T WHERE t.u=bx_UserInfos.UserID
UPDATE bx_UserInfos SET UnreadSystemNotifies = T.nc  FROM(SELECT u, nc FROM @tempTable WHERE c = 11) AS T WHERE t.u=bx_UserInfos.UserID
UPDATE bx_UserInfos SET UnreadBidUpNotifies = T.nc  FROM(SELECT u, nc FROM @tempTable WHERE c = 6) AS T WHERE t.u=bx_UserInfos.UserID
UPDATE bx_UserInfos SET UnreadHailNotifies = T.nc  FROM(SELECT u, nc FROM @tempTable WHERE c = 5) AS T WHERE t.u=bx_UserInfos.UserID
UPDATE bx_UserInfos SET UnreadFriendNotifies = T.nc  FROM(SELECT u, nc FROM @tempTable WHERE c = 4) AS T WHERE t.u=bx_UserInfos.UserID
UPDATE bx_UserInfos SET UnreadPostNotifies = T.nc  FROM(SELECT u, nc FROM @tempTable WHERE c = 2) AS T WHERE t.u=bx_UserInfos.UserID
UPDATE bx_UserInfos SET UnreadMessages =a.v FROM (select UserID,count(*) v FROM bx_ChatMessages WHERE IsRead=0 Group By UserID)as a 
WHERE a.UserID = bx_UserInfos.UserID
');

IF EXISTS(SELECT * FROM sysobjects WHERE [type]=N'TR' AND [name]=N'bx_UserInfos_AfterUpdate')
	EXEC ('ALTER TABLE bx_UserInfos ENABLE TRIGGER bx_UserInfos_AfterUpdate');

DELETE bx_Settings WHERE TypeName = 'MaxLabs.bbsMax.Settings.BaiduPageOpJopSettings';

UPDATE bx_Moderators SET ModeratorType=4 WHERE ModeratorType=3