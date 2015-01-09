CREATE VIEW bx_AttacmentsWithForumID
AS
SELECT bx_Attachments.*, bx_Posts.ForumID,bx_Posts.ThreadID
FROM bx_Attachments INNER JOIN
      bx_Posts ON bx_Attachments.PostID = bx_Posts.PostID
