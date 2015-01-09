EXEC bx_Drop 'bx_UsersInRoles_After_Update';

GO

CREATE TRIGGER [bx_UsersInRoles_After_Update]	ON [bx_UsersInRoles] After UPDATE

AS
BEGIN

	SET NOCOUNT ON;
	
	IF UPDATE([RoleID]) BEGIN
	
		DECLARE @tempTable table(RoleID int,TotalUser int);
		
		INSERT INTO @tempTable
			SELECT DISTINCT RoleID,
				ISNULL((SELECT COUNT(*) FROM [bx_UsersInRoles] as r WITH (NOLOCK) WHERE r.RoleID=T.RoleID),0)
			FROM [INSERTED]	T;
			
		INSERT INTO @tempTable
			SELECT DISTINCT RoleID,
				ISNULL((SELECT COUNT(*) FROM [bx_UsersInRoles] AS r WITH (NOLOCK) WHERE r.RoleID=T.RoleID),0)
			FROM [DELETED] T
			WHERE RoleID NOT IN (SELECT RoleID FROM @tempTable);
			
		UPDATE [bx_Roles]
			SET
				bx_Roles.UserCount=T.TotalUser
			FROM @tempTable T
			WHERE
				T.RoleID=bx_Roles.RoleID;
	END
END		