--
EXEC bx_Drop 'bx_PointShows_After_Update';

GO

CREATE TRIGGER  bx_PointShows_After_Update  ON [bx_PointShows] 
FOR INSERT, UPDATE 
AS
BEGIN

DECLARE @UserIDs  table(UserID  int)

INSERT @UserIDs SELECT UserID FROM INSERTED;

UPDATE bx_PointShows SET Price = ShowPoints WHERE UserID IN( SELECT UserID FROM @UserIDs ) AND  Price>ShowPoints 

DELETE FROM bx_PointShows WHERE  UserID IN( SELECT UserID FROM @UserIDs ) AND  Price=0;

END
