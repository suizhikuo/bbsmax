CREATE VIEW bx_UserMissionsView AS
SELECT bx_Missions.IsEnable, bx_UserMissions.*
FROM bx_Missions INNER JOIN
      bx_UserMissions ON bx_Missions.ID = bx_UserMissions.MissionID AND ParentID IS NULL