EXEC bx_Drop 'bx_Forum_AftertDelete';
GO

CREATE TRIGGER [bx_Forum_AftertDelete] ON [bx_Forums] 
FOR   DELETE 
AS
BEGIN
DELETE FROM bx_BannedUsers WHERE ForumID IN (SELECT ForumID FROM DELETED);
END

GO