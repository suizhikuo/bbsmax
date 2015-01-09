CREATE VIEW bx_SellingProps AS
SELECT A.UserID, A.UserPropID, A.SellingCount, A.SellingPrice, A.Count, A.SellingDate, B.* FROM bx_UserProps A LEFT JOIN bx_Props B ON A.PropID = B.PropID AND B.Enable = 1 WHERE A.SellingCount > 0
