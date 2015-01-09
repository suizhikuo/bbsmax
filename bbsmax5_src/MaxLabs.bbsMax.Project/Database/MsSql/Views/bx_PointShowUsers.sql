CREATE VIEW [bx_PointShowUsers]
AS
SELECT bx_PointShows.Price, bx_PointShows.Content, 
      bx_Members.*
FROM bx_PointShows INNER JOIN
      bx_Members ON bx_PointShows.UserID = bx_Members.UserID