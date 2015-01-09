/*没有验证的头像数据*/

CREATE VIEW bx_UserTempAvatar AS

SELECT bx_Users.UserID, bx_Users.Username, 
      bx_Users.Realname,bx_Users.Gender,
      bx_Users.AvatarSrc, 
      bx_UserTempData.Data, bx_UserTempData.CreateDate
FROM bx_Users INNER JOIN
      bx_UserTempData ON 
      bx_Users.UserID = bx_UserTempData.UserID
WHERE (bx_UserTempData.DataType = 1)
