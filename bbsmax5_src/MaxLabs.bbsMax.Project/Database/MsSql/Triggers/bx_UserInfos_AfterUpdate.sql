
EXEC bx_Drop 'bx_UserInfos_AfterUpdate';

GO

CREATE TRIGGER [bx_UserInfos_AfterUpdate]
	ON [bx_UserInfos]
	AFTER INSERT, UPDATE
AS
BEGIN


	SET NOCOUNT ON;

	UPDATE bx_Users
	   SET UserInfo = (
						CAST(T.[InviterID] AS varchar(10)) + '|'			--0
						
						+ CAST(T.[TotalFriends] AS varchar(10)) + '|'		--1
						
						+ CAST(T.[Birthday] AS varchar(4)) + '|'			--2
						+ CAST(T.[BirthYear] AS varchar(4)) + '|'			--3
						
						+ CAST(T.[BlogPrivacy] AS varchar(10)) + '|'		--4
						+ CAST(T.[FeedPrivacy] AS varchar(10)) + '|'		--5
						+ CAST(T.[BoardPrivacy] AS varchar(10)) + '|'		--6
						+ CAST(T.[DoingPrivacy] AS varchar(10)) + '|'		--7
						+ CAST(T.[AlbumPrivacy] AS varchar(10)) + '|'		--8
						+ CAST(T.[SpacePrivacy] AS varchar(10)) + '|'		--9
						+ CAST(T.[SharePrivacy] AS varchar(10)) + '|'		--10
						
						+ CAST(T.[FriendListPrivacy] AS varchar(10)) + '|'	--11
						+ CAST(T.[InformationPrivacy] AS varchar(10)) + '|'	--12
						
						+ CAST(T.[NotifySetting] AS varchar(4000))			--13
						)
	  FROM [INSERTED] T WHERE T.UserID = bx_Users.UserID;
	  
	  SELECT 'ResetUser' AS XCMD, * FROM [INSERTED];
END

