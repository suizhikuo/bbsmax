CREATE VIEW bx_UserEmoticonInfo
AS
SELECT bx_Users.UserID, bx_Users.Username, 
      SUM(bx_EmoticonGroups.TotalSizes) AS TotalSizes, 
      SUM(bx_EmoticonGroups.TotalEmoticons) AS TotalEmoticons
FROM bx_Users INNER JOIN
      bx_EmoticonGroups ON 
      bx_Users.UserID = bx_EmoticonGroups.UserID
GROUP BY bx_Users.UserID, bx_Users.Username