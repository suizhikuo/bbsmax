CREATE VIEW bx_UserTempRealname AS
SELECT bx_UserTempData.UserID, bx_Users.Username, 
      bx_Users.Realname, bx_Users.Gender, bx_UserTempData.CreateDate, 
      bx_UserTempData.Data AS TempRealname, 
      0 AS NameChecked
FROM bx_Users INNER JOIN
      bx_UserTempData ON 
      bx_Users.UserID = bx_UserTempData.UserID
WHERE (bx_UserTempData.DataType = 0)