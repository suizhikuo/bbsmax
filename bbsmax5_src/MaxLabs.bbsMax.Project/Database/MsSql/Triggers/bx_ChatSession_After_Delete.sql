
EXEC bx_Drop 'bx_ChatSessions_After_Delete';

GO


CREATE TRIGGER bx_ChatSessions_After_Delete ON bx_ChatSessions
FOR DELETE 
AS 
SET NOCOUNT ON;

DELETE FROM  bx_ChatMessages  FROM( SELECT * FROM DELETED) as t  WHERE bx_ChatMessages.UserID =t.UserID AND bx_ChatMessages.TargetUserID = t.TargetUserID


