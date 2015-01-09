EXEC bx_Drop 'bx_UsersInRoles_AfterDelete';

GO

CREATE TRIGGER [bx_UsersInRoles_AfterDelete] ON [bx_UsersInRoles] AFTER DELETE

AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(RoleID int,TotalUser int);
	
	INSERT INTO @tempTable
		SELECT DISTINCT RoleID,
			ISNULL((SELECT COUNT(*) FROM [bx_UsersInRoles] AS r WITH (NOLOCK) WHERE r.RoleID=T.RoleID),0)
		FROM [DELETED] T;
		
	UPDATE [bx_Roles]
		SET
			bx_Roles.UserCount=T.TotalUser
		FROM @tempTable T
		WHERE
			T.RoleID=bx_Roles.RoleID;
			
END

GO