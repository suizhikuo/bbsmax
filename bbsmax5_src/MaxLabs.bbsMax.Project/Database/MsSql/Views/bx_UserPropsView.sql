CREATE VIEW bx_UserPropsView AS
SELECT A.UserID, A.UserPropID, A.SellingCount, A.SellingPrice, A.Count, B.* FROM bx_UserProps A LEFT JOIN bx_Props B ON A.PropID = B.PropID
