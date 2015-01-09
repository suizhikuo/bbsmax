CREATE VIEW bx_TopicsWithContents
AS
SELECT bx_Threads.*, bx_Posts.Content, bx_Posts.IPAddress
FROM bx_Threads INNER JOIN
      bx_Posts ON bx_Threads.ThreadID = bx_Posts.ThreadID
WHERE (bx_Posts.PostType = 1)
